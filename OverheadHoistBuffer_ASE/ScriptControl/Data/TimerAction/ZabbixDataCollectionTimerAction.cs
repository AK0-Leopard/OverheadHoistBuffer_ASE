using com.mirle.ibg3k0.bcf.Common;
//*********************************************************************************
//      MESDefaultMapAction.cs
//*********************************************************************************
// File Name: MESDefaultMapAction.cs
// Description: Type 1 Function
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.TimerAction;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.stc.Common.SECS;
using NLog;
//using Predes.ZabbixSender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.TimerAction
{
    public class ZabbixDataCollectionTimerAction : ITimerAction
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        protected SCApplication scApp = null;
        protected ALINE line = null;
        protected MPLCSMControl smControl;
        private DateTime LastReport_Time = DateTime.MinValue;
        private int RptInterval_MM = 1;
        public ZabbixDataCollectionTimerAction(string name, long intervalMilliSec)
            : base(name, intervalMilliSec)
        {

        }

        public override void initStart()
        {
            scApp = SCApplication.getInstance();
            line = scApp.getEQObjCacheManager().getLine();
        }

        private long syncPoint = 0;
        private int ohxHeartBeat = 0;
        public override void doProcess(object obj)
        {
            if (System.Threading.Interlocked.Exchange(ref syncPoint, 1) == 0)
            {
                try
                {
                    int total_idle_vh_clean = scApp.VehicleBLL.getNoExcuteMcsCmdVhCount(E_VH_TYPE.Clean);
                    int total_idle_vh_Dirty = scApp.VehicleBLL.getNoExcuteMcsCmdVhCount(E_VH_TYPE.Dirty);
                    scApp.ReportBLL.ZabbixPush(SCAppConstants.ZabbixServerInfo.ZABBIX_OHXC_IDLE_DIRTY_CAR, total_idle_vh_Dirty);
                    scApp.ReportBLL.ZabbixPush(SCAppConstants.ZabbixServerInfo.ZABBIX_OHXC_IDLE_CLEAR_CAR, total_idle_vh_clean);
                    //scApp.ZabbixService.Send(SCAppConstants.ZABBIX_SERVER_NAME, "OHxC_IDLE_Dirty_Car", total_idle_vh_Dirty.ToString());
                    //scApp.ZabbixService.Send(SCAppConstants.ZABBIX_SERVER_NAME, "OHxC_IDLE_Clean_Car", total_idle_vh_clean.ToString());
                    scApp.ReportBLL.ZabbixPush
                            (SCAppConstants.ZabbixServerInfo.ZABBIX_OHXC_IS_ACTIVE,
                            line.ServiceMode == SCAppConstants.AppServiceMode.Active ? 1 : 0);

                    ohxHeartBeat = ohxHeartBeat == SCAppConstants.ZabbixOHxCAlive.ZABBIX_OHXC_ALIVE_HEARTBEAT_ON ?
                                                   SCAppConstants.ZabbixOHxCAlive.ZABBIX_OHXC_ALIVE_HEARTBEAT_OFF:
                                                   SCAppConstants.ZabbixOHxCAlive.ZABBIX_OHXC_ALIVE_HEARTBEAT_ON;
                    scApp.ReportBLL.ZabbixPush
                            (SCAppConstants.ZabbixServerInfo.ZABBIX_OHXC_ALIVE, ohxHeartBeat);

                    if (DateTime.Now.AddMinutes(-RptInterval_MM) > LastReport_Time)
                    {
                        LastReport_Time = DateTime.Now;
                        int total_cmd_is_queue_count = scApp.CMDBLL.getCMD_MCSIsQueueCount();
                        int total_cmd_is_running_count = scApp.CMDBLL.getCMD_MCSIsRunningCount();
                        scApp.ReportBLL.ZabbixPush(SCAppConstants.ZabbixServerInfo.ZABBIX_MCS_CMD_QUEUE, total_cmd_is_queue_count);
                        scApp.ReportBLL.ZabbixPush(SCAppConstants.ZabbixServerInfo.ZABBIX_MCS_CMD_RUNNING, total_cmd_is_running_count);

                        //scApp.ZabbixService.Send("3K0Server", "OHxC_IDLE_Dirty_Car", total_inser_time.ToString());
                        //scApp.ZabbixService.Send("3K0Server", "OHxC_IDLE_Clean_Car", total_finish_time.ToString());
                    }

                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref syncPoint, 0);
                }
            }
        }


    }

}
