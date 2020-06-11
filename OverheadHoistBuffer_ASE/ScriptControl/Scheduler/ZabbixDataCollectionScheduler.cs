using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Scheduler
{
    public class ZabbixDataCollectionScheduler : IJob
    {
        SCApplication scApp = SCApplication.getInstance();
        int interval_time_hour = 1;
        public void Execute(IJobExecutionContext context)
        {
            int total_inser_count = scApp.CMDBLL.getCMD_MCSInserCountLastHour(interval_time_hour);
            int total_finish_count = scApp.CMDBLL.getCMD_MCSFinishCountLastHour(interval_time_hour);
            scApp.ReportBLL.ZabbixPush(SCAppConstants.ZabbixServerInfo.ZABBIX_MCS_CMD_RECIVED_HOUR, total_inser_count);
            scApp.ReportBLL.ZabbixPush( SCAppConstants.ZabbixServerInfo.ZABBIX_MCS_CMD_FINISHED_HOUR, total_finish_count);
        }
    }

}
