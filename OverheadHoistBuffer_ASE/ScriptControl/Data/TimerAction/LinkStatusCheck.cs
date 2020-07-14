// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="LinkStatusCheck.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.TimerAction;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.stc.Common.SECS;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.TimerAction
{
    /// <summary>
    /// Class LinkStatusCheck.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.TimerAction.ITimerAction" />
    public class LinkStatusCheck : ITimerAction
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// The sc application
        /// </summary>
        protected SCApplication scApp = null;
        /// <summary>
        /// The sm control
        /// </summary>
        protected MPLCSMControl smControl;
        Dictionary<string, CommuncationInfo> dicCommInfo = null;
        ValueWrite isAliveIndexVW = null;
        ValueRead isAliveIndexVR = null;
        ALINE line = null;
        List<AEQPT> eqpts = null;
        double eqAlive_Min_Change_Interval_sec = 100;
        double OHCVAlive_Min_Change_Interval_sec = 5;
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkStatusCheck"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="intervalMilliSec">The interval milli sec.</param>
        public LinkStatusCheck(string name, long intervalMilliSec)
            : base(name, intervalMilliSec)
        {

        }

        /// <summary>
        /// Initializes the start.
        /// </summary>
        public override void initStart()
        {
            //not implement
            scApp = SCApplication.getInstance();
            //smControl = scApp.getBCFApplication().getMPLCSMControl("EQ") as MPLCSMControl;
            //isAliveIndexVW = scApp.getBCFApplication().getWriteValueEvent(SCAppConstants.EQPT_OBJECT_CATE_LINE, "VH_LINE", "OHxC_Alive_W");
            //isAliveIndexVR = scApp.getBCFApplication().getReadValueEvent(SCAppConstants.EQPT_OBJECT_CATE_LINE, "VH_LINE", "OHxC_Alive_R");
            //dicCommInfo = scApp.getEQObjCacheManager().CommonInfo.dicCommunactionInfo;
            dicCommInfo = scApp.getEQObjCacheManager().CommonInfo.dicCommunactionInfo;
            line = scApp.getEQObjCacheManager().getLine();
            eqpts = scApp.getEQObjCacheManager().getAllEquipment();
        }

        private long synPoint = 0;
        /// <summary>
        /// Timer Action的執行動作
        /// </summary>
        /// <param name="obj">The object.</param>
        public override void doProcess(object obj)
        {
            if (System.Threading.Interlocked.Exchange(ref synPoint, 1) == 0)
            {

                try
                {
                    doCheckIPLinkStatus();
                    //doCheckIPLinkStatusParallel();
                    doCheckEQAliveStatus();
                    scApp.CheckSystemEventHandler.CheckCheckSystemIsExist();

                    ALINE line = scApp.getEQObjCacheManager().getLine();
                    InlineEfficiencyMonitor(line);
                    Task.Run(() =>
                    {
                        CheckLinkStatus(line);
                    });
                    if (SCUtility.getCallContext<bool>(ALINE.CONTEXT_KEY_WORD_LINE_STATUS_HAS_CHANGE))
                    {
                        line.NotifyLineStatusChange();
                        SCUtility.setCallContext(ALINE.CONTEXT_KEY_WORD_LINE_STATUS_HAS_CHANGE, null);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref synPoint, 0);
                }
            }
        }



        private void InlineEfficiencyMonitor(ALINE line)
        {
            VehicleBLL vehicleBLL = scApp.VehicleBLL;
            CMDBLL cmdBLL = scApp.CMDBLL;
            line.CurrntVehicleModeAutoRemoteCount = vehicleBLL.cache.getVhCurrentModeInAutoRemoteCount();
            line.CurrntVehicleModeAutoLoaclCount = vehicleBLL.cache.getVhCurrentModeInAutoLocalCount();
            line.CurrntVehicleStatusIdelCount = vehicleBLL.cache.getVhCurrentStatusInIdleCount();
            line.CurrntVehicleStatusErrorCount = vehicleBLL.cache.getVhCurrentStatusInErrorCount();
            var host_cmds = cmdBLL.loadACMD_MCSIsUnfinished();
            var cst = scApp.CassetteDataBLL.loadCassetteData();
            UInt16 carrier_transferring_count = (UInt16)cst.
                                        Where(x => x.CSTState == E_CSTState.Alternate ||
                                            x.CSTState == E_CSTState.WaitIn ||
                                            x.CSTState == E_CSTState.WaitOut ||
                                            x.CSTState == E_CSTState.Transferring).
                                        Count();
            UInt16 carrier_watting_count = (UInt16)cst.
                                        Where(x => x.CSTState == E_CSTState.Installed ||
                                            x.CSTState == E_CSTState.Completed).
                                        Count();

            UInt16 host_cmd_assigned_count = (UInt16)host_cmds.
                                        Where(cmd => cmd.TRANSFERSTATE >= E_TRAN_STATUS.Transferring).
                                        Count();
            UInt16 host_cmd_watting_count = (UInt16)host_cmds.
                                        Where(cmd => cmd.TRANSFERSTATE < E_TRAN_STATUS.Transferring).
                                        Count();

            line.CurrntCSTStatueTransferCount = carrier_transferring_count;
            line.CurrntCSTStatueWaitingCount = carrier_watting_count;
            line.CurrntHostCommandTransferStatueAssignedCount = host_cmd_assigned_count;
            line.CurrntHostCommandTransferStatueWaitingCounr = host_cmd_watting_count;
        }
        private long syncCheckLink_Point = 0;
        private void CheckLinkStatus(ALINE line)
        {
            if (System.Threading.Interlocked.Exchange(ref syncCheckLink_Point, 1) == 0)
            {
                try
                {
                    bool is_connection_success = false;
                    foreach (var device in line.DeviceConnectionInfos)
                    {

                        switch (device.Type)
                        {
                            case ProtocolFormat.OHTMessage.DeviceConnectionType.Ap:
                                var ap_setting_info = scApp.LineBLL.getAPSetting(device.Name);
                                is_connection_success = PingIt(ap_setting_info.REMOTE_IP);
                                break;
                            case ProtocolFormat.OHTMessage.DeviceConnectionType.Mcs:
                                var secs_agent = scApp.getBCFApplication().getSECSAgent(device.Name);
                                is_connection_success = secs_agent.IsSelected;
                                break;
                            case ProtocolFormat.OHTMessage.DeviceConnectionType.Plc:
                                var plc_agent = scApp.getBCFApplication().getMPLCSMControl(device.Name);
                                is_connection_success = plc_agent.isAlive();
                                break;
                        }
                        ProtocolFormat.OHTMessage.ConnectionStatus new_status =
                                        is_connection_success ? ProtocolFormat.OHTMessage.ConnectionStatus.Success
                                                              : ProtocolFormat.OHTMessage.ConnectionStatus.Unsuccess;
                        if (device.Status != new_status)
                        {
                            device.Status = new_status;
                            SCUtility.setCallContext(ALINE.CONTEXT_KEY_WORD_LINE_STATUS_HAS_CHANGE, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref syncCheckLink_Point, 0);
                }
            }
        }
        private long syncCheckIP_Point = 0;
        private void doCheckIPLinkStatus()
        {
            if (System.Threading.Interlocked.Exchange(ref syncCheckIP_Point, 1) == 0)
            {
                try
                {
                    foreach (KeyValuePair<string, CommuncationInfo> keyPair in dicCommInfo)
                    {
                        CommuncationInfo Info = keyPair.Value;
                        if (!SCUtility.isEmpty(Info.Getway_IP))
                        {
                            Info.IsCommunactionSuccess = PingIt(Info.Getway_IP);
                        }
                        if (!SCUtility.isEmpty(Info.Remote_IP))
                        {
                            Info.IsConnectinoSuccess = PingIt(Info.Remote_IP);
                        }
                    }
                    line.setConnectionInfo(dicCommInfo);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref syncCheckIP_Point, 0);
                }
            }
        }

        //2020.07.14 Hsinyu Chang: doCheckIPLinkStatus的平行迴圈改寫
        //因 1)foreach中的每個元素沒有資料相依
        //   2)ping為I/O bound，用sequential loop會有大部分時間在空等
        //   3)ping結果有時效性
        //Note: 目前作法要等到parallel foreach的每個元素都ping完才會更新資料，尚有優化空間
        private void doCheckIPLinkStatusParallel()
        {
            if (System.Threading.Interlocked.Exchange(ref syncCheckIP_Point, 1) == 0)
            {
                try
                {
                    Parallel.ForEach(dicCommInfo, keyPair => {
                        CommuncationInfo Info = keyPair.Value;
                        if (!SCUtility.isEmpty(Info.Getway_IP))
                        {
                            Info.IsCommunactionSuccess = PingIt(Info.Getway_IP);
                        }
                        if (!SCUtility.isEmpty(Info.Remote_IP))
                        {
                            Info.IsConnectinoSuccess = PingIt(Info.Remote_IP);
                        }
                    });
                    line.setConnectionInfo(dicCommInfo);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref syncCheckIP_Point, 0);
                }
            }
        }

        private long syncCheckEQAlive_Point = 0;
        private void doCheckEQAliveStatus()
        {
            if (System.Threading.Interlocked.Exchange(ref syncCheckEQAlive_Point, 1) == 0)
            {
                try
                {
                    foreach (AEQPT eqpt in eqpts)
                    {
                        //if (eqpt.EQPT_ID.StartsWith("CV"))
                        //{
                        //    ANODE node = scApp.getEQObjCacheManager().getParentNodeByEQPTID(eqpt.EQPT_ID);
                        //    List<AEQPT> ohcvs = scApp.getEQObjCacheManager().getEuipmentListByNode(node.NODE_ID);
                        //    if (eqpt.Eq_Alive_Last_Change_time.AddSeconds(OHCVAlive_Min_Change_Interval_sec) < DateTime.Now)
                        //    {
                        //        eqpt.Is_Eq_Alive = false;

                        //    }
                        //    else
                        //    {
                        //        eqpt.Is_Eq_Alive = true;
                        //    }
                        //    bool temp = true;
                        //    foreach(AEQPT ohcv in ohcvs)
                        //    {
                        //        temp = temp && ohcv.Is_Eq_Alive;
                        //    }
                        //    node.Is_Alive = temp;
                        //}
                        if (eqpt.EQPT_ID.StartsWith("CV"))
                        {
                            //如果是CV，不需要靠上次變化時間來改變是否存在，CV存在時，則會常ON
                        }
                        else
                        {
                            if (eqpt.Eq_Alive_Last_Change_time.AddSeconds(eqAlive_Min_Change_Interval_sec) < DateTime.Now)
                            {
                                eqpt.Is_Eq_Alive = false;
                            }
                            else
                            {
                                eqpt.Is_Eq_Alive = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref syncCheckEQAlive_Point, 0);
                }
            }
        }


        /// <summary>
        /// Pings it.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool PingIt(String ip)
        {
            PingReply r;
            try
            {
                if (SCUtility.isEmpty(ip)) return false;

                Ping p = new Ping();
                r = p.Send(ip);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                return false;
            }
            return r.Status == IPStatus.Success;
        }
    }
}