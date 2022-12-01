//*********************************************************************************
//      BackgroundPLCWorkProcessData.cs
//*********************************************************************************
// File Name: BackgroundPLCWorkProcessData.cs
// Description: 背景執行上報Process Data至MES的實際作業
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Common.MPLC;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data;
using com.mirle.ibg3k0.bcf.Schedule;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using com.mirle.iibg3k0.ttc.Common;
using NLog;

namespace com.mirle.ibg3k0.sc.Data
{
    /// <summary>
    /// Class BackgroundWorkSample.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Schedule.IBackgroundWork" />
    public class BackgroundWorkProcVehiclePosition : IBackgroundWork
    {
        /// <summary>
        /// The logger
        /// </summary>
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the maximum background queue count.
        /// </summary>
        /// <returns>System.Int64.</returns>
        public long getMaxBackgroundQueueCount()
        {
            return 1000;
        }

        /// <summary>
        /// Gets the name of the driver.
        /// </summary>
        /// <returns>System.String.</returns>
        public string getDriverName()
        {
            return this.GetType().Name;
        }

        public void doWork(string workKey, BackgroundWorkItem item)
        {
            try
            {
                App.SCApplication scapp = item.Param[0] as App.SCApplication;
                AVEHICLE vh = item.Param[1] as AVEHICLE;
                iibg3k0.ttc.Common.TcpIpEventArgs e = item.Param[2] as iibg3k0.ttc.Common.TcpIpEventArgs;
                ID_134_TRANS_EVENT_REP recive_str = (ID_134_TRANS_EVENT_REP)e.objPacket;

                if (!isNeedProcessPositionReport(vh, e))
                {
                    return;
                }

                scapp.VehicleBLL.setAndPublishPositionReportInfo2Redis(vh.VEHICLE_ID, recive_str);
                System.Threading.SpinWait.SpinUntil(() => false, 0);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        private bool isNeedProcessPositionReport(AVEHICLE vh, TcpIpEventArgs e)
        {
            if (!DebugParameter.IsOpenPositionSeqNumCheck)
                return true;

            int current_seq_num = e.iSeqNum;
            int pre_position_seq_num = vh.PrePositionSeqNum;

            bool need_process_position = checkPositionSeqNum(current_seq_num, pre_position_seq_num);
            vh.PrePositionSeqNum = current_seq_num;
            if (!need_process_position)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(BackgroundWorkProcVehiclePosition), Device: Service.VehicleService.DEVICE_NAME_OHx,
                   Data: $"The vehicles updata position report of seq num is old,by pass this one.old seq num;{pre_position_seq_num},current seq num:{current_seq_num}",
                   VehicleID: vh.VEHICLE_ID,
                   CarrierID: vh.CST_ID);
            }

            return need_process_position;
        }

        const int TOLERANCE_SCOPE = 50;
        private const ushort SEQNUM_MAX = 9999;
        private bool checkPositionSeqNum(int currnetNum, int preNum)
        {
            try
            {
                int lower_limit = preNum - TOLERANCE_SCOPE; // 1000 - 50 = 950;
                if (lower_limit >= 0)
                {
                    //如果該次的Num介於上次的值減去容錯值(TOLERANCE_SCOPE = 50) 至 上次的值
                    //就代表是舊的資料
                    if (currnetNum > (lower_limit) && currnetNum < preNum) // 1002 > 950 && 1002 < 1000
                    {
                        return false;
                    }
                }
                else
                {
                    //如果上次的值減去容錯值變成負的，代表要再由SENDSEQNUM_MAX往回推
                    lower_limit = SEQNUM_MAX + lower_limit;
                    if (currnetNum > (lower_limit) && currnetNum < preNum)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "(checkPositionSeqNum) Exception");
                return true;
            }
        }
    }
}
