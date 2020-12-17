//*********************************************************************************
//      ZoneBlockCheck.cs
//*********************************************************************************
// File Name: ZoneBlockCheck.cs
// Description: 
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.TimerAction;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.TimerAction
{
    /// <summary>
    /// 用來找尋目前是否有發生死結的車子，
    /// 死結定義是當兩台車在要某一段路時，且剛好都被對方佔住
    /// 就代表死結產生了
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.TimerAction.ITimerAction" />
    class DeadlockCheck : ITimerAction
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
        /// Initializes a new instance of the <see cref="DeadlockCheck"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="intervalMilliSec">The interval milli sec.</param>
        public DeadlockCheck(string name, long intervalMilliSec)
            : base(name, intervalMilliSec)
        {

        }

        /// <summary>
        /// Initializes the start.
        /// </summary>
        public override void initStart()
        {
            //do nothing
            scApp = SCApplication.getInstance();

        }

        private long checkSyncPoint = 0;
        /// <summary>
        /// Timer Action的執行動作
        /// </summary>
        /// <param name="obj">The object.</param>
        //public override void doProcess(object obj)
        //{
        //    if (!SystemParameter.AutoOverride)
        //    {
        //        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(DeadlockCheck), Device: "AGVC",
        //           Data: $"auto override is:{SystemParameter.AutoOverride}.");
        //        return;
        //    }
        //    if (System.Threading.Interlocked.Exchange(ref checkSyncPoint, 1) == 0)
        //    {
        //        try
        //        {
        //            //1.找出發生Reserve要不到而停止的車子，找到以後在找是否有下一台車子也是要不到Reserve的
        //            //發現以後再透過兩台車所要不到的Address找出互卡的是哪段Section，接著將其section所屬的Segment Banned後在執行override
        //            var vhs = scApp.VehicleBLL.cache.loadAllVh();
        //            var vhs_ReserveStop = vhs.Where(v => v.IsReservePause)
        //                                  .OrderBy(v => v.VEHICLE_ID)
        //                                  .ToList();
        //            foreach (var vh_active in vhs_ReserveStop)
        //            {
        //                foreach (var vh_passive in vhs_ReserveStop)
        //                {
        //                    if (vh_active == vh_passive) continue;
        //                    if (!vh_active.IsReservePause || !vh_active.IsReservePause) continue;
        //                    //if (SCUtility.isEmpty(vh_active.OHTC_CMD) || SCUtility.isEmpty(vh_passive.OHTC_CMD)) continue;
        //                    if ((vh_active.CanNotReserveInfo != null && vh_passive.CanNotReserveInfo != null) &&
        //                        SCUtility.isMatche(vh_active.CanNotReserveInfo.ReservedVhID, vh_passive.VEHICLE_ID) &&
        //                        SCUtility.isMatche(vh_passive.CanNotReserveInfo.ReservedVhID, vh_active.VEHICLE_ID))
        //                    //if ((vh_active.CanNotReserveInfo != null && vh_passive.CanNotReserveInfo != null))
        //                    {
        //                        if (vh_active.CurrentFailOverrideTimes >= AVEHICLE.MAX_FAIL_OVERRIDE_TIMES_IN_ONE_CASE &&
        //                            vh_passive.CurrentFailOverrideTimes >= AVEHICLE.MAX_FAIL_OVERRIDE_TIMES_IN_ONE_CASE)
        //                        {
        //                            scApp.VehicleService.onDeadLockProcessFail(vh_active, vh_passive);

        //                            string xid = DateTime.Now.ToString(SCAppConstants.TimestampFormat_19);
        //                            string message = $"dead lock happend ,but dead of vehicles:{vh_active.VEHICLE_ID} and {vh_passive.VEHICLE_ID} has been override more than {AVEHICLE.MAX_FAIL_OVERRIDE_TIMES_IN_ONE_CASE} times, stop auto override." +
        //                                             $"please excute manual avoid.";
        //                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(DeadlockCheck), Device: "AGVC",
        //                               Data: message,
        //                               XID: xid,
        //                               VehicleID: $"{ vh_active.VEHICLE_ID},{ vh_passive.VEHICLE_ID}",
        //                               CarrierID: $"{ SCUtility.Trim(vh_active.CST_ID, true)},{ SCUtility.Trim(vh_passive.CST_ID, true)}");
        //                            BCFApplication.onErrorMsg(this, new bcf.Common.LogEventArgs(message, xid));
        //                            System.Threading.SpinWait.SpinUntil(() => false, 5000);//等待觸發把Auto override關掉。
        //                            return;
        //                        }
        //                        //如果符合上述條件，代表著死結發生了
        //                        List<AVEHICLE> sort_vhs = new List<AVEHICLE>() { vh_active, vh_passive };
        //                        sort_vhs.Sort(SortOverrideOfVehicle);
        //                        foreach (AVEHICLE avoid_vh in sort_vhs)
        //                        {
        //                            if (avoid_vh.CurrentFailOverrideTimes >= AVEHICLE.MAX_FAIL_OVERRIDE_TIMES_IN_ONE_CASE)
        //                            {
        //                                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(DeadlockCheck), Device: "AGVC",
        //                                   Data: $"dead lock happend ,but vh:{avoid_vh.VEHICLE_ID} has been override more than {AVEHICLE.MAX_FAIL_OVERRIDE_TIMES_IN_ONE_CASE} times, continue next vh.",
        //                                   VehicleID: avoid_vh.VEHICLE_ID,
        //                                   CarrierID: avoid_vh.CST_ID);
        //                                continue;
        //                            }

        //                            if (avoid_vh.VhAvoidInfo != null)
        //                            {
        //                                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(DeadlockCheck), Device: "AGVC",
        //                                   Data: $"dead lock happend ,but vh:{avoid_vh.VEHICLE_ID} has been avoid command , continue next vh." +
        //                                         $"blocked section:{avoid_vh.VhAvoidInfo.BlockedSectionID} blocked vh id:{avoid_vh.VhAvoidInfo.BlockedVehicleID}",
        //                                   VehicleID: avoid_vh.VEHICLE_ID,
        //                                   CarrierID: avoid_vh.CST_ID);
        //                                continue;
        //                            }

        //                            //string current_section_id = selected_vh.CUR_SEC_ID;
        //                            //var check_is_in_traffic_control_section = scApp.TrafficControlBLL.cache.IsTrafficControlSection(current_section_id);
        //                            //if (check_is_in_traffic_control_section.isTrafficControlInfo)
        //                            //{
        //                            //    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(DeadlockCheck), Device: "AGVC",
        //                            //       Data: $"dead lock happend ,but vh:{selected_vh.VEHICLE_ID} is in traffic control section,traffic control id:{check_is_in_traffic_control_section.trafficControlInfo.ID}" +
        //                            //             $"don't excute override",
        //                            //       VehicleID: selected_vh.VEHICLE_ID,
        //                            //       CarrierID: selected_vh.CST_ID);
        //                            //    continue;
        //                            //}
        //                            AVEHICLE keep_going_vh = avoid_vh == vh_active ? vh_passive : vh_active;
        //                            if (avoid_vh.isTcpIpConnect)
        //                            {
        //                                ACMD_OHTC cmd_ohtc = scApp.CMDBLL.GetCMD_OHTCByID(avoid_vh.OHTC_CMD);
        //                                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(DeadlockCheck), Device: "AGVC",
        //                                   Data: $"dead lock happend ,ask vh:{avoid_vh.VEHICLE_ID} chnage path.",
        //                                   VehicleID: avoid_vh.VEHICLE_ID,
        //                                   CarrierID: avoid_vh.CST_ID);

        //                                //if (scApp.VehicleService.doSendOverrideCommandToVh(vh_active, cmd_ohtc, vh_active.CanNotReserveInfo.ReservedAdrID))
        //                                //bool is_override_success = scApp.VehicleService.trydoOverrideCommandToVh(selected_vh, cmd_ohtc, selected_vh.CanNotReserveInfo.ReservedSectionID);
        //                                bool is_override_success = scApp.VehicleService.trydoAvoidCommandToVh(avoid_vh, keep_going_vh);
        //                                if (is_override_success)
        //                                {
        //                                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(DeadlockCheck), Device: "AGVC",
        //                                       Data: $"dead lock happend ,ask vh:{avoid_vh.VEHICLE_ID} chnage path success.",
        //                                       VehicleID: avoid_vh.VEHICLE_ID,
        //                                       CarrierID: avoid_vh.CST_ID);
        //                                    System.Threading.SpinWait.SpinUntil(() => false, 15000);
        //                                    return;
        //                                }
        //                                else
        //                                {

        //                                    avoid_vh.CurrentFailOverrideTimes++;
        //                                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(DeadlockCheck), Device: "AGVC",
        //                                       Data: $"dead lock happend ,ask vh:{avoid_vh.VEHICLE_ID} chnage path fail, fail times:{avoid_vh.CurrentFailOverrideTimes}.",
        //                                       VehicleID: avoid_vh.VEHICLE_ID,
        //                                       CarrierID: avoid_vh.CST_ID);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            logger.Error(ex, "Exection:");
        //        }
        //        finally
        //        {

        //            System.Threading.Interlocked.Exchange(ref checkSyncPoint, 0);

        //        }
        //    }
        //}
        public override void doProcess(object obj)
        {
            if (!SystemParameter.AutoOverride)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(DeadlockCheck), Device: "OHBC",
                   Data: $"auto override is:{SystemParameter.AutoOverride}.");
                return;
            }
            if (System.Threading.Interlocked.Exchange(ref checkSyncPoint, 1) == 0)
            {
                try
                {
                    //找出發生Reserve要不到而停止的車子，如果兩台都剛好是因為對方的關係拿不路權
                    //就可以開始準備對其中一台車進行Override
                    var vhs = scApp.VehicleBLL.cache.loadVhs();
                    var vhs_ReserveStop = vhs.Where(v => v.IsReservePause)
                                          .OrderBy(v => v.VEHICLE_ID)
                                          .ToList();
                    foreach (var vh_active in vhs_ReserveStop)
                    {
                        foreach (var vh_passive in vhs_ReserveStop)
                        {
                            if (vh_active == vh_passive) continue;
                            if (!vh_active.IsReservePause || !vh_active.IsReservePause) continue;
                            if ((vh_active.CanNotReserveInfo != null && vh_passive.CanNotReserveInfo != null))
                            {
                                List<AVEHICLE> sort_vhs = new List<AVEHICLE>() { vh_active, vh_passive };

                                //將找出來的vh進行排序，用來幫忙決定要讓哪一台車進行退避
                                //比條件有
                                //1.是否有執行MCS命令(沒執行的會先避車)
                                //2.是否有搬送CST(沒載CST的會先避車)
                                sort_vhs.Sort(SortOverrideOfVehicle);
                                foreach (AVEHICLE avoid_vh in sort_vhs)
                                {
                                    if (avoid_vh.CurrentFailOverrideTimes >= AVEHICLE.MAX_FAIL_OVERRIDE_TIMES_IN_ONE_CASE)
                                    {
                                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(DeadlockCheck), Device: "AGVC",
                                           Data: $"dead lock happend ,but vh:{avoid_vh.VEHICLE_ID} has been override more than {AVEHICLE.MAX_FAIL_OVERRIDE_TIMES_IN_ONE_CASE} times, continue next vh.",
                                           VehicleID: avoid_vh.VEHICLE_ID,
                                           CarrierID: avoid_vh.CST_ID);
                                        continue;
                                    }

                                    if (avoid_vh.VhAvoidInfo != null)
                                    {
                                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(DeadlockCheck), Device: "AGVC",
                                           Data: $"dead lock happend ,but vh:{avoid_vh.VEHICLE_ID} has been avoid command , continue next vh." +
                                                 $"blocked section:{avoid_vh.VhAvoidInfo.BlockedSectionID} blocked vh id:{avoid_vh.VhAvoidInfo.BlockedVehicleID}",
                                           VehicleID: avoid_vh.VEHICLE_ID,
                                           CarrierID: avoid_vh.CST_ID);
                                        continue;
                                    }
                                    AVEHICLE pass_vh = avoid_vh == vh_active ? vh_passive : vh_active;


                                    var key_blocked_vh = findTheKeyBlockVhID(avoid_vh, pass_vh);
                                    if (key_blocked_vh == null) continue;
                                    if (avoid_vh.isTcpIpConnect)
                                    {
                                        ACMD_OHTC cmd_ohtc = scApp.CMDBLL.getCMD_OHTCByID(avoid_vh.OHTC_CMD);
                                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(DeadlockCheck), Device: "AGVC",
                                           Data: $"dead lock happend ,ask vh:{avoid_vh.VEHICLE_ID} chnage path.",
                                           VehicleID: avoid_vh.VEHICLE_ID,
                                           CarrierID: avoid_vh.CST_ID);

                                        bool is_override_success = scApp.VehicleService.trydoAvoidCommandToVh(avoid_vh, key_blocked_vh);
                                        if (is_override_success)
                                        {
                                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(DeadlockCheck), Device: "AGVC",
                                               Data: $"dead lock happend ,ask vh:{avoid_vh.VEHICLE_ID} chnage path success.",
                                               VehicleID: avoid_vh.VEHICLE_ID,
                                               CarrierID: avoid_vh.CST_ID);
                                            System.Threading.SpinWait.SpinUntil(() => false, 15000);
                                            return;
                                        }
                                        else
                                        {
                                            avoid_vh.CurrentFailOverrideTimes++;
                                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(DeadlockCheck), Device: "AGVC",
                                               Data: $"dead lock happend ,ask vh:{avoid_vh.VEHICLE_ID} chnage path fail, fail times:{avoid_vh.CurrentFailOverrideTimes}.",
                                               VehicleID: avoid_vh.VEHICLE_ID,
                                               CarrierID: avoid_vh.CST_ID);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exection:");
                }
                finally
                {

                    System.Threading.Interlocked.Exchange(ref checkSyncPoint, 0);

                }
            }
        }
        private AVEHICLE findTheKeyBlockVhID(AVEHICLE avoidVh, AVEHICLE blockedVh)
        {
            if (blockedVh == null) return null;

            if (SCUtility.isMatche(avoidVh.CanNotReserveInfo.ReservedVhID, blockedVh.VEHICLE_ID) &&
                SCUtility.isMatche(blockedVh.CanNotReserveInfo.ReservedVhID, avoidVh.VEHICLE_ID))
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(DeadlockCheck), Device: "OHBC",
                   Data: $"dead lock happend ,find key blocked vh .avoid vh:{avoidVh.VEHICLE_ID} ,blocked vh:{blockedVh.VEHICLE_ID}",
                   VehicleID: avoidVh.VEHICLE_ID,
                   CarrierID: avoidVh.CST_ID);
                return blockedVh;
            }
            else
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(DeadlockCheck), Device: "OHBC",
                   Data: $"dead lock happend ,can't find key blocked vh .avoid vh:{avoidVh.VEHICLE_ID} ,blocked vh:{blockedVh.VEHICLE_ID}. start find orther block vh",
                   VehicleID: avoidVh.VEHICLE_ID,
                   CarrierID: avoidVh.CST_ID);

                AVEHICLE orther_reserved_vh = scApp.VehicleBLL.cache.getVhByID(blockedVh.CanNotReserveInfo.ReservedVhID);
                int find_count = 0;
                return findTheOrtherKeyBlockVhID(avoidVh, orther_reserved_vh, ref find_count);
            }
        }

        const int MAX_FIND_COUNT = 4;
        private AVEHICLE findTheOrtherKeyBlockVhID(AVEHICLE avoidVh, AVEHICLE reservedVh, ref int findCount)
        {
            if (findCount > MAX_FIND_COUNT)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(DeadlockCheck), Device: "OHBC",
                   Data: $"dead lock happend ,find key block fail. over find times:{findCount}",
                   VehicleID: avoidVh.VEHICLE_ID,
                   CarrierID: avoidVh.CST_ID);
                return null;
            }
            if (SCUtility.isMatche(avoidVh.VEHICLE_ID, reservedVh.VEHICLE_ID))
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(DeadlockCheck), Device: "OHBC",
                   Data: $"dead lock happend ,find key block fail. avoid and blocked vh is same.vh id:{avoidVh.VEHICLE_ID}",
                   VehicleID: avoidVh.VEHICLE_ID,
                   CarrierID: avoidVh.CST_ID);
                return null;
            }
            if (reservedVh.CanNotReserveInfo == null) return null;
            if (SCUtility.isMatche(avoidVh.VEHICLE_ID, reservedVh.CanNotReserveInfo.ReservedVhID))
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(DeadlockCheck), Device: "OHBC",
                   Data: $"dead lock happend ,find key blocked vh(orther).avoid vh:{avoidVh.VEHICLE_ID} ,blocked vh:{reservedVh.VEHICLE_ID}",
                   VehicleID: avoidVh.VEHICLE_ID,
                   CarrierID: avoidVh.CST_ID);
                return reservedVh;
            }
            else
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(DeadlockCheck), Device: "OHBC",
                   Data: $"dead lock happend ,no find key blocked vh ," +
                         $"find next block vh id:{reservedVh.CanNotReserveInfo.ReservedVhID} blocking vh:{reservedVh.VEHICLE_ID}",
                   VehicleID: avoidVh.VEHICLE_ID,
                   CarrierID: avoidVh.CST_ID);
                AVEHICLE orther_reserved_vh = scApp.VehicleBLL.cache.getVhByID(reservedVh.CanNotReserveInfo.ReservedVhID);
                findCount++;
                return findTheOrtherKeyBlockVhID(avoidVh, orther_reserved_vh, ref findCount);
            }
        }

        private int SortOverrideOfVehicle(AVEHICLE vh1, AVEHICLE vh2)
        {
            int result;
            //if (vh1.VhAvoidInfo != null)
            //{
            //    return -1;
            //}
            //if (vh2.VhAvoidInfo != null)
            //{
            //    return 1;
            //}


            if (!SCUtility.isEmpty(vh1.MCS_CMD) && !SCUtility.isEmpty(vh2.MCS_CMD))
            {
                if (vh1.HAS_CST == 1 && vh2.HAS_CST == 1)
                {
                    result = 0;
                }
                else if (vh1.HAS_CST == 1)
                {
                    result = 1;
                }
                else if (vh2.HAS_CST == 1)
                {
                    result = -1;
                }
                else
                {
                    result = 0;
                }
            }
            else if (!SCUtility.isEmpty(vh1.MCS_CMD))
            {
                result = 1;
            }
            else if (!SCUtility.isEmpty(vh2.MCS_CMD))
            {
                result = -1;
            }
            else
            {
                result = 0;
            }
            return result;
        }


    }
}