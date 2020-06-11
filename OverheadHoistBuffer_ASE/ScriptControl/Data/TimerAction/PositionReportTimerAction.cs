//*********************************************************************************
//      PositionReportTimerAction.cs
//*********************************************************************************
// File Name: PositionReportTimerAction.cs
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
using System.Threading;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.TimerAction
{
    /// <summary>
    /// Class PositionReportTimerAction.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.TimerAction.ITimerAction" />
    class PositionReportTimerAction : ITimerAction
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// The sc application
        /// </summary>
        protected SCApplication scApp = null;
        List<AVEHICLE> listVh = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="PositionReportTimerAction"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="intervalMilliSec">The interval milli sec.</param>
        public PositionReportTimerAction(string name, long intervalMilliSec)
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
            listVh = scApp.getEQObjCacheManager().getAllVehicle();
        }

        private long checkSyncPoint = 0;
        /// <summary>
        /// Timer Action的執行動作
        /// </summary>
        /// <param name="obj">The object.</param>
        public override void doProcess(object obj)
        {
            if (System.Threading.Interlocked.Exchange(ref checkSyncPoint, 1) == 0)
            {
                //using (DBConnection_EF con = new DBConnection_EF())
                //{
                try
                {
                    foreach (AVEHICLE vh in listVh)
                    {
                        //TODO 並改成Timer 1S一次 ，每次Sleep 50ms
                        if (vh.MODE_STATUS == VHModeStatus.AutoLocal ||
                            vh.MODE_STATUS == VHModeStatus.AutoRemote)
                        {
                            scApp.VehicleBLL.getAndProcPositionReportFromRedis(vh.VEHICLE_ID);
                            SpinWait.SpinUntil(() => false, 5);
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
                //}
            } 
        }


    }
}