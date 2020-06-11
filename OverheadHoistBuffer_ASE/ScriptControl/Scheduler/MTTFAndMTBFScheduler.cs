using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace com.mirle.ibg3k0.sc.Scheduler
{
    public class MTTFAndMTBFScheduler : IJob
    {
        public static int RECORD_INTERVAL_MIN = 3;
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        NLog.Logger logger_mttfmtbf_agvc = NLog.LogManager.GetLogger("MTTFMTBF_OHTC_Record");
        NLog.Logger logger_mttfmtbf_agv = NLog.LogManager.GetLogger("MTTFMTBF_OHT_Record");
        SCApplication scApp = SCApplication.getInstance();
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                ReocrdMTTFMTBFForOHxC();
                ReocrdMTTFMTBFForOHT();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }

        private void ReocrdMTTFMTBFForOHxC()
        {
            DateTime nowDateTime = DateTime.Now;
            DeviceStatus status = DeviceStatus.Normal;
            string record_interval_min = RECORD_INTERVAL_MIN.ToString();

            string record_message = $"{nowDateTime.ToString(SCAppConstants.DateTimeFormat_19)},{status},{record_interval_min}";
            logger_mttfmtbf_agvc.Info(record_message);
        }
        private void ReocrdMTTFMTBFForOHT()
        {
            List<AVEHICLE> vhs = scApp.VehicleBLL.cache.loadVhs();
            foreach (var vh in vhs)
            {
                Task.Run(() =>
                {
                    DateTime nowDateTime = DateTime.Now;
                    string device_id = vh.VEHICLE_ID;
                    DeviceStatus status;
                    string record_interval_min = RECORD_INTERVAL_MIN.ToString();
                    if (vh.isTcpIpConnect)
                    {
                        if (vh.isAuto)
                        {
                            bool ask_success = scApp.VehicleService.VehicleStatusRequest(vh.VEHICLE_ID);
                            if (ask_success)
                            {
                                status = DeviceStatus.Normal;
                            }
                            else
                            {
                                status = DeviceStatus.NoResponse;
                            }
                        }
                        else
                        {
                            status = DeviceStatus.NoResponse;
                        }
                    }
                    else
                    {
                        status = DeviceStatus.NoResponse;
                    }
                    string record_message = $"{nowDateTime.ToString(SCAppConstants.DateTimeFormat_19)},{device_id},{status},{record_interval_min}";
                    logger_mttfmtbf_agv.Info(record_message);
                });
            }
        }


        enum DeviceStatus
        {
            Normal,
            NoResponse
        }

    }

}
