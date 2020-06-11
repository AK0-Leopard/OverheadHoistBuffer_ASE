using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static com.mirle.ibg3k0.sc.ALINE;
using static com.mirle.ibg3k0.sc.AVEHICLE;

namespace com.mirle.ibg3k0.sc.Service
{
    public class ConnectionInfoService
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private SCApplication scApp = null;
        private ReportBLL reportBLL = null;
        private LineBLL lineBLL = null;
        private ALINE line = null;
        public ConnectionInfoService()
        {

        }
        public void start(SCApplication _app)
        {
            scApp = _app;
            reportBLL = _app.ReportBLL;
            lineBLL = _app.LineBLL;
            line = scApp.getEQObjCacheManager().getLine();

            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.Host_Control_State), PublishLineInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.SCStats), PublishLineInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.Secs_Link_Stat), PublishLineInfo);

            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.CurrentPortStateChecked), PublishOnlineCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.CurrentStateChecked), PublishOnlineCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.EnhancedVehiclesChecked), PublishOnlineCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.TSCStateChecked), PublishOnlineCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.UnitAlarmStateListChecked), PublishOnlineCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.EnhancedTransfersChecked), PublishOnlineCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.EnhancedCarriersChecked), PublishOnlineCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.CurrentEQPortStateChecked), PublishOnlineCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.AlarmSetChecked), PublishOnlineCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.CurrentPortTypesChecked), PublishOnlineCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.LaneCutListChecked), PublishOnlineCheckInfo);

            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.MCSConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.RouterConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.OHT1ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.OHT2ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.OHT3ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.OHT4ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.OHT5ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.OHT6ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.OHT7ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.OHT8ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.OHT9ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.OHT10ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.OHT11ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.OHT12ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.OHT13ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.OHT14ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.MTLConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.MTSConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.MTS2ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.HID1ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.HID2ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.HID3ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.HID4ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.Adam1ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.Adam2ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.Adam3ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.Adam4ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.AP1ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.AP2ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.AP3ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.AP4ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.AP5ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.AP6ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.AP7ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.AP8ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.AP9ConnectionSuccess), PublishPingCheckInfo);
            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.AP10ConnectionSuccess), PublishPingCheckInfo);
            //initPublish(line);
        }
        //private void initPublish(ALINE line)
        //{
        //    PublishLineInfo(line, null);
        //    PublishOnlineCheckInfo(line, null);
        //    PublishPingCheckInfo(line, null);
        //}

        private void PublishLineInfo(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                ALINE line = sender as ALINE;
                if (sender == null) return;
                byte[] line_serialize = BLL.LineBLL.Convert2GPB_ConnectionInfo(line);
                scApp.getNatsManager().PublishAsync
                    (SCAppConstants.NATS_SUBJECT_CONNECTION_INFO, line_serialize);


                //TODO 要改用GPP傳送
                //var line_Serialize = ZeroFormatter.ZeroFormatterSerializer.Serialize(line);
                //scApp.getNatsManager().PublishAsync
                //    (string.Format(SCAppConstants.NATS_SUBJECT_LINE_INFO), line_Serialize);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        private void PublishOnlineCheckInfo(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                ALINE line = sender as ALINE;
                if (sender == null) return;
                byte[] online_serialize = BLL.LineBLL.Convert2GPB_OnlineCheckInfo(line);
                scApp.getNatsManager().PublishAsync
                    (SCAppConstants.NATS_SUBJECT_ONLINE_CHECK_INFO, online_serialize);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        private void PublishPingCheckInfo(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                ALINE line = sender as ALINE;
                if (sender == null) return;
                byte[] online_serialize = BLL.LineBLL.Convert2GPB_PingCheckInfo(line);
                scApp.getNatsManager().PublishAsync
                    (SCAppConstants.NATS_SUBJECT_PING_CHECK_INFO, online_serialize);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        public bool doChangeLinkStatus(string linkStatus, out string result)
        {
            bool isSuccess = true;
            result = string.Empty;
            try
            {
                if (isSuccess)
                {
                    using (TransactionScope tx = SCUtility.getTransactionScope())
                    {
                        using (DBConnection_EF con = DBConnection_EF.GetUContext())
                        {
                            if (linkStatus == SCAppConstants.LinkStatus.LinkOK.ToString())
                            {
                                if (scApp.getEQObjCacheManager().getLine().Secs_Link_Stat == SCAppConstants.LinkStatus.LinkOK)
                                {
                                    result = "Selected already!";
                                }
                                else
                                {
                                    Task.Run(() => scApp.LineService.startHostCommunication());
                                    result = "OK";
                                }

                                tx.Complete();

                            }
                            else if (linkStatus == SCAppConstants.LinkStatus.LinkFail.ToString())
                            {
                                if (scApp.getEQObjCacheManager().getLine().Secs_Link_Stat == SCAppConstants.LinkStatus.LinkFail)
                                {
                                    result = "Not selected already!";
                                }
                                else
                                {
                                    Task.Run(() => scApp.LineService.stopHostCommunication());
                                    result = "OK";
                                }

                                tx.Complete();

                            }
                            else
                            {
                                result = linkStatus + " Not Defined";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error(ex, "Execption:");
            }
            return isSuccess;
        }


        public bool doChangeHostMode(string host_mode, out string result)
        {
            bool isSuccess = true;
            result = string.Empty;
            try
            {
                if (isSuccess)
                {
                    using (TransactionScope tx = SCUtility.getTransactionScope())
                    {
                        using (DBConnection_EF con = DBConnection_EF.GetUContext())
                        {
                            if (host_mode == SCAppConstants.LineHostControlState.HostControlState.On_Line_Remote.ToString())
                            {
                                if (!scApp.LineService.canOnlineWithHost())
                                {
                                    //MessageBox.Show("Has vh not ready");
                                    //回報當無法連線
                                    result = "Has vh not ready";
                                }
                                else if (scApp.getEQObjCacheManager().getLine().Host_Control_State == SCAppConstants.LineHostControlState.HostControlState.On_Line_Remote)
                                {
                                    //MessageBox.Show("On line ready");
                                    result = "OnlineRemote ready";
                                }
                                else
                                {
                                    line.resetOnlieCheckItem();
                                    Task.Run(() => scApp.LineService.OnlineWithHostOp());
                                    result = "OK";
                                }
                                //isSuccess = scApp.PortStationBLL.OperateDB.updatePriority(portID, priority);
                                //if (isSuccess)
                                //{
                                tx.Complete();
                                //    scApp.PortStationBLL.OperateCatch.updatePriority(portID, priority);
                                //}
                            }
                            else if (host_mode == SCAppConstants.LineHostControlState.HostControlState.On_Line_Local.ToString())
                            {
                                if (!scApp.LineService.canOnlineWithHost())
                                {
                                    //MessageBox.Show("Has vh not ready");
                                    //回報當無法連線
                                    result = "Has vh not ready";
                                }
                                else if (scApp.getEQObjCacheManager().getLine().Host_Control_State == SCAppConstants.LineHostControlState.HostControlState.On_Line_Local)
                                {
                                    //MessageBox.Show("On line ready");
                                    result = "OnlineLocal ready";
                                }
                                else
                                {
                                    line.resetOnlieCheckItem();
                                    Task.Run(() => scApp.LineService.OnlineLocalWithHostOp());
                                    result = "OK";
                                }
                                //isSuccess = scApp.PortStationBLL.OperateDB.updatePriority(portID, priority);
                                //if (isSuccess)
                                //{
                                tx.Complete();
                                //    scApp.PortStationBLL.OperateCatch.updatePriority(portID, priority);
                                //}
                            }
                            else
                            {
                                if (scApp.getEQObjCacheManager().getLine().SCStats != TSCState.PAUSED)
                                {
                                    //MessageBox.Show("Please change tsc state to pause first.");
                                    result = "Please change TSC Status to pause first.";
                                }
                                else if (scApp.getEQObjCacheManager().getLine().Host_Control_State == SCAppConstants.LineHostControlState.HostControlState.EQ_Off_line)
                                {
                                    //MessageBox.Show("Current is off line");
                                    result = "Current is off line";
                                }
                                else
                                {
                                    line.resetOnlieCheckItem();
                                    Task.Run(() => scApp.LineService.OfflineWithHostByOp());
                                    result = "OK";
                                }
                                //isSuccess = scApp.PortStationBLL.OperateDB.updatePriority(portID, priority);
                                //if (isSuccess)
                                //{
                                tx.Complete();
                                //    scApp.PortStationBLL.OperateCatch.updatePriority(portID, priority);
                                //}
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error(ex, "Execption:");
            }
            return isSuccess;
        }

        public bool doChangeTSCstate(string tscstate, out string result)
        {
            bool isSuccess = true;
            result = string.Empty;
            try
            {
                if (isSuccess)
                {
                    using (TransactionScope tx = SCUtility.getTransactionScope())
                    {
                        using (DBConnection_EF con = DBConnection_EF.GetUContext())
                        {
                            if (tscstate == ALINE.TSCState.AUTO.ToString())
                            {
                                if (scApp.getEQObjCacheManager().getLine().SCStats == ALINE.TSCState.AUTO)
                                {
                                    result = "AUTO ready";
                                }
                                else
                                {
                                    Task.Run(() => scApp.getEQObjCacheManager().getLine().ResumeToAuto(scApp.ReportBLL));
                                    result = "OK";
                                }
                                //isSuccess = scApp.PortStationBLL.OperateDB.updatePriority(portID, priority);
                                //if (isSuccess)
                                //{
                                tx.Complete();
                                //    scApp.PortStationBLL.OperateCatch.updatePriority(portID, priority);
                                //}
                            }
                            else if (tscstate == ALINE.TSCState.PAUSED.ToString())
                            {
                                if (scApp.getEQObjCacheManager().getLine().SCStats == ALINE.TSCState.PAUSED)
                                {
                                    //MessageBox.Show("Has vh not ready");
                                    //回報當無法連線
                                    result = "PAUSED ready";
                                }
                                else
                                {
                                    Task.Run(() => scApp.LineService.TSCStateToPause(""));
                                    result = "OK";
                                }
                                //isSuccess = scApp.PortStationBLL.OperateDB.updatePriority(portID, priority);
                                //if (isSuccess)
                                //{
                                tx.Complete();
                                //    scApp.PortStationBLL.OperateCatch.updatePriority(portID, priority);
                                //}
                            }
                            else
                            {
                                result = tscstate + " Not Defined";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error(ex, "Execption:");
            }
            return isSuccess;
        }





    }
}
