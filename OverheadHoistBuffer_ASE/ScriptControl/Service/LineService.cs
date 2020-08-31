using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using com.mirle.ibg3k0.Utility.ul.Data.VO;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace com.mirle.ibg3k0.sc.Service
{
    public class LineService
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private SCApplication scApp = null;
        private ReportBLL reportBLL = null;
        private LineBLL lineBLL = null;

        private ALINE line = null;
        public LineService()
        {

        }
        public void start(SCApplication _app)
        {
            scApp = _app;
            reportBLL = _app.ReportBLL;
            lineBLL = _app.LineBLL;
            line = scApp.getEQObjCacheManager().getLine();

            line.addEventHandler(nameof(LineService), nameof(line.Host_Control_State), PublishLineInfo);
            line.addEventHandler(nameof(LineService), nameof(line.SCStats), PublishLineInfo);
            line.addEventHandler(nameof(LineService), nameof(line.Currnet_Park_Type), PublishLineInfo);
            line.addEventHandler(nameof(LineService), nameof(line.Currnet_Cycle_Type), PublishLineInfo);
            line.addEventHandler(nameof(LineService), nameof(line.Secs_Link_Stat), PublishLineInfo);
            line.addEventHandler(nameof(LineService), nameof(line.Redis_Link_Stat), PublishLineInfo);
            line.addEventHandler(nameof(LineService), nameof(line.DetectionSystemExist), PublishLineInfo);
            line.addEventHandler(nameof(LineService), nameof(line.IsEarthquakeHappend), PublishLineInfo);
            line.addEventHandler(nameof(LineService), nameof(line.IsAlarmHappened), PublishLineInfo);
            line.LineStatusChange += Line_LineStatusChange;

        }

        public void startHostCommunication()
        {
            scApp.getBCFApplication().getSECSAgent(scApp.EAPSecsAgentName).refreshConnection();
        }

        public void stopHostCommunication()
        {
            scApp.getBCFApplication().getSECSAgent(scApp.EAPSecsAgentName).stop();
            line.Secs_Link_Stat = SCAppConstants.LinkStatus.LinkFail;
            line.connInfoUpdate_Disconnection();
        }

        private void Line_LineStatusChange(object sender, EventArgs e)
        {
            PublishLineInfo(sender, null);
        }

        public void PublishLineInfo(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                ALINE line = sender as ALINE;
                if (sender == null) return;
                byte[] line_serialize = BLL.LineBLL.Convert2GPB_LineInfo(line);
                scApp.getNatsManager().PublishAsync
                    (SCAppConstants.NATS_SUBJECT_LINE_INFO, line_serialize);
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


        public void OnlineRemoteWithHostOp()
        {
            bool isSuccess = true;
            isSuccess = isSuccess && reportBLL.AskAreYouThere();
            isSuccess = isSuccess && lineBLL.updateHostControlState(SCAppConstants.LineHostControlState.HostControlState.On_Line_Remote);
            isSuccess = isSuccess && reportBLL.ReportControlStateRemote();
            isSuccess = isSuccess && TSCStateToPause("");
        }

        public void OnlineLocalWithHostOp() //OnlineLoca
        {
            bool isSuccess = true;
            isSuccess = isSuccess && reportBLL.AskAreYouThere();
            isSuccess = isSuccess && lineBLL.updateHostControlState(SCAppConstants.LineHostControlState.HostControlState.On_Line_Local);
            //isSuccess = isSuccess && reportBLL.ReportControlStateRemote();
            isSuccess = isSuccess && reportBLL.ReportControlStateLocal();
            isSuccess = isSuccess && TSCStateToPause("");
        }

        public void OnlineWithHostOp()
        {
            bool isSuccess = true;
            isSuccess = isSuccess && reportBLL.AskAreYouThere();
            isSuccess = isSuccess && lineBLL.updateHostControlState(SCAppConstants.LineHostControlState.HostControlState.On_Line_Remote);
            isSuccess = isSuccess && reportBLL.ReportControlStateRemote();
            isSuccess = isSuccess && TSCStateToPause("");
        }
        public void OnlineWithHostByHost()
        {
            bool isSuccess = true;
            isSuccess = isSuccess && lineBLL.updateHostControlState(SCAppConstants.LineHostControlState.HostControlState.On_Line_Remote);
            isSuccess = isSuccess && reportBLL.ReportControlStateRemote();
            isSuccess = isSuccess && TSCStateToPause("");
        }

        public void OfflineWithHostByOp()
        {
            bool isSuccess = true;

            if (line.Secs_Link_Stat != SCAppConstants.LinkStatus.LinkFail)
            {
                isSuccess = isSuccess && reportBLL.ReportEquiptmentOffLine();
            }
            
            isSuccess = isSuccess && lineBLL.updateHostControlState(SCAppConstants.LineHostControlState.HostControlState.EQ_Off_line);            
        }
        public void OfflineWithHostByHost()
        {
            bool isSuccess = true;
            isSuccess = isSuccess && reportBLL.ReportEquiptmentOffLine();
            isSuccess = isSuccess && lineBLL.updateHostControlState(SCAppConstants.LineHostControlState.HostControlState.EQ_Off_line);            
        }

        public bool canOnlineWithHost()
        {
            bool can_not_online = false;
            //1檢查目前沒有Remove的Vhicle，是否都已連線
            List<AVEHICLE> vhs = scApp.getEQObjCacheManager().getAllVehicle();
            List<AVEHICLE> need_check_vhs = vhs.Where(vh => vh.State != VehicleState.Remove).ToList();

            can_not_online = need_check_vhs.Where(vh => !vh.isTcpIpConnect).Count() > 0;
            return !can_not_online;
        }

        public bool TSCStateToPause(string pausrReason)
        {
            bool isSuccess = true;
            ALINE.TSCStateMachine tsc_sm = line.TSC_state_machine;
            if (tsc_sm.State == ALINE.TSCState.NONE)
            {
                isSuccess = isSuccess && line.AGVCInitialComplete(reportBLL);
                //reportBLL.ReportTSCAutoCompleted();
                isSuccess = isSuccess && line.StartUpSuccessed(reportBLL);
            }
            else if (tsc_sm.State == ALINE.TSCState.TSC_INIT)
            {
                isSuccess = isSuccess && line.StartUpSuccessed(reportBLL);
            }
            else if (tsc_sm.State == ALINE.TSCState.AUTO)
            {
                isSuccess = isSuccess && line.RequestToPause(reportBLL, pausrReason);
                //List<ACMD_MCS> cmd_mcs_lst = scApp.CMDBLL.loadACMD_MCSIsUnfinished();
                int in_excute_cmd_count = scApp.CMDBLL.getCMD_MCSIsRunningCount();
                if (in_excute_cmd_count == 0)
                {
                    isSuccess = isSuccess && line.PauseCompleted(reportBLL);
                }
            }
            else if (tsc_sm.State == ALINE.TSCState.PAUSING)
            {
                isSuccess = isSuccess && line.PauseCompleted(reportBLL);
            }
            else if (tsc_sm.State == ALINE.TSCState.PAUSED)
            {
                //do nothing
            }
            else
            {
                //do nothing
            }
            return isSuccess;
        }

        public void ProcessHostCommandResume()
        {
            //todo fire TSC to auto
        }

        object publishSystemMsgLock = new object();
        public void PublishSystemMsgInfo(Object systemLog)
        {
            lock (publishSystemMsgLock)
            {
                try
                {
                    SYSTEMPROCESS_INFO logObj = systemLog as SYSTEMPROCESS_INFO;

                    byte[] systemMsg_Serialize = BLL.LineBLL.Convert2GPB_SystemMsgInfo(logObj);

                    if (systemMsg_Serialize != null)
                    {
                        scApp.getNatsManager().PublishAsync
                            (SCAppConstants.NATS_SUBJECT_SYSTEM_LOG, systemMsg_Serialize);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception:");
                }
            }
        }

        object publishHostMsgLock = new object();
        public void PublishHostMsgInfo(Object secsLog)
        {
            lock (publishHostMsgLock)
            {
                try
                {
                    LogTitle_SECS logSECS = secsLog as LogTitle_SECS;

                    byte[] systemMsg_Serialize = BLL.LineBLL.Convert2GPB_SECSMsgInfo(logSECS);

                    if (systemMsg_Serialize != null)
                    {
                        scApp.getNatsManager().PublishAsync
                            (SCAppConstants.NATS_SUBJECT_SECS_LOG, systemMsg_Serialize);
                        //scApp.getElasticSearchManager().insertLogData(logSECS);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception:");
                }
            }
        }

        object publishEQMsgLock = new object();
        public void PublishEQMsgInfo(Object tcpLog)
        {
            lock (publishEQMsgLock)
            {
                try
                {
                    dynamic logEntry = tcpLog as JObject;

                    byte[] tcpMsg_Serialize = BLL.LineBLL.Convert2GPB_TcpMsgInfo(logEntry);

                    if (tcpMsg_Serialize != null)
                    {
                        scApp.getNatsManager().PublishAsync
                            (SCAppConstants.NATS_SUBJECT_TCPIP_LOG, tcpMsg_Serialize);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception:");
                }
            }
        }

        public void ProcessAlarmReport(string nodeID, string eqID, string eqRealID, string currentAddressID, string errCode, ErrorStatus errorStatus)
        {
            string node_id = nodeID;
            string eq_id = eqID;
            string eq_real_id = eqRealID;
            string current_adr_id = currentAddressID;
            string err_code = errCode;
            ErrorStatus status = errorStatus;

            List<ALARM> alarms = null;
            AlarmMap alarmMap = scApp.AlarmBLL.GetAlarmMap(eq_id, err_code);
            //在設備上報Alarm時，如果是第一次上報(之前都沒有Alarm發生時，則要上報S6F11 CEID=51 Alarm Set)
            bool processBeferHasErrorExist = scApp.AlarmBLL.hasAlarmErrorExist();
            if (alarmMap != null &&
                alarmMap.ALARM_LVL == E_ALARM_LVL.Error &&
                status == ErrorStatus.ErrSet &&
                //!scApp.AlarmBLL.hasAlarmErrorExist())
                !processBeferHasErrorExist)
            {
                scApp.ReportBLL.newReportAlarmSet();
            }
            scApp.getRedisCacheManager().BeginTransaction();
            using (TransactionScope tx = SCUtility.getTransactionScope())
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: "OHxC",
                       Data: $"Process vehicle alarm report.alarm code:{err_code},alarm status{status}",
                       VehicleID: eq_id);
                    ALARM alarm = null;
                    switch (status)
                    {
                        case ErrorStatus.ErrSet:
                            //將設備上報的Alarm填入資料庫。
                            alarm = scApp.AlarmBLL.setAlarmReport(node_id, eq_id, err_code, null);
                            //將其更新至Redis，保存目前所發生的Alarm
                            scApp.AlarmBLL.setAlarmReport2Redis(alarm);
                            alarms = new List<ALARM>() { alarm };
                            break;
                        case ErrorStatus.ErrReset:
                            if (SCUtility.isMatche(err_code, "0"))
                            {
                                alarms = scApp.AlarmBLL.resetAllAlarmReport(eq_id);
                                scApp.AlarmBLL.resetAllAlarmReport2Redis(eq_id);
                            }
                            else
                            {
                                //將設備上報的Alarm從資料庫刪除。
                                alarm = scApp.AlarmBLL.resetAlarmReport(eq_id, err_code);
                                //將其更新至Redis，保存目前所發生的Alarm
                                scApp.AlarmBLL.resetAlarmReport2Redis(alarm);
                                alarms = new List<ALARM>() { alarm };
                            }
                            break;
                    }
                    tx.Complete();
                }
            }
            scApp.getRedisCacheManager().ExecuteTransaction();
            //通知有Alarm的資訊改變。
            scApp.getNatsManager().PublishAsync(SCAppConstants.NATS_SUBJECT_CURRENT_ALARM, new byte[0]);


            foreach (ALARM report_alarm in alarms)
            {
                if (report_alarm == null) continue;
                if (report_alarm.ALAM_LVL == E_ALARM_LVL.Warn ||
                    report_alarm.ALAM_LVL == E_ALARM_LVL.None) continue;
                //需判斷Alarm是否存在如果有的話則需再判斷MCS是否有Disable該Alarm的上報
                int ialarm_code = 0;
                int.TryParse(report_alarm.ALAM_CODE, out ialarm_code);
                string alarm_code = (ialarm_code < 0 ? ialarm_code * -1 : ialarm_code).ToString();
                if (scApp.AlarmBLL.IsReportToHost(alarm_code))
                {
                    //scApp.ReportBLL.ReportAlarmHappend(eqpt.VEHICLE_ID, alarm.ALAM_STAT, alarm.ALAM_CODE, alarm.ALAM_DESC, out reportqueues);
                    List<AMCSREPORTQUEUE> reportqueues = new List<AMCSREPORTQUEUE>();
                    if (report_alarm.ALAM_STAT == ErrorStatus.ErrSet)
                    {
                        scApp.ReportBLL.ReportAlarmHappend(report_alarm.ALAM_STAT, alarm_code, report_alarm.ALAM_DESC);
                        scApp.ReportBLL.newReportUnitAlarmSet(eq_real_id, alarm_code, report_alarm.ALAM_DESC, current_adr_id, reportqueues);
                    }
                    else
                    {
                        scApp.ReportBLL.ReportAlarmHappend(report_alarm.ALAM_STAT, alarm_code, report_alarm.ALAM_DESC);
                        scApp.ReportBLL.newReportUnitAlarmClear(eq_real_id, alarm_code, report_alarm.ALAM_DESC, current_adr_id, reportqueues);
                    }
                    scApp.ReportBLL.newSendMCSMessage(reportqueues);

                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: "OHxC",
                       Data: $"do report alarm to mcs,alarm code:{err_code},alarm status{status}",
                       VehicleID: eq_id);
                }
            }
            //在設備上報取消Alarm，如果已經沒有Alarm(Alarm都已經消除，則要上報S6F11 CEID=52 Alarm Clear)
            bool processAfterHasErrorExist = scApp.AlarmBLL.hasAlarmErrorExist();
            if (status == ErrorStatus.ErrReset &&
                //!scApp.AlarmBLL.hasAlarmErrorExist())
                processBeferHasErrorExist &&
                !processAfterHasErrorExist)
            {
                scApp.ReportBLL.newReportAlarmClear();
            }
        }


    }
}
