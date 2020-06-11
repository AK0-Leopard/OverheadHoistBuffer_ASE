// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="EqptAliveCheck.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.TimerAction;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.TimerAction
{
    /// <summary>
    /// Class EqptAliveCheck.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.TimerAction.ITimerAction" />
    class VehicleHardwareInfoCollectionTimerAction : ITimerAction
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetLogger("RecodeVehicleHardwareInfo");
        /// <summary>
        /// The sc application
        /// </summary>
        protected SCApplication scApp = null;
        /// <summary>
        /// The line
        /// </summary>
        ALINE line = null;


        /// <summary>
        /// Initializes a new instance of the <see cref="EqptAliveCheck"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="intervalMilliSec">The interval milli sec.</param>
        public VehicleHardwareInfoCollectionTimerAction(string name, long intervalMilliSec)
            : base(name, intervalMilliSec)
        {

        }

        /// <summary>
        /// Initializes the start.
        /// </summary>
        public override void initStart()
        {
            scApp = SCApplication.getInstance();
            line = scApp.getEQObjCacheManager().getLine();
        }


        /// <summary>
        /// Timer Action的執行動作
        /// </summary>
        /// <param name="obj">The object.</param>
        private long syncPoint = 0;
        public override void doProcess(object obj)
        {
            if (System.Threading.Interlocked.Exchange(ref syncPoint, 1) == 0)
            {

                List<AVEHICLE> vhs = scApp.getEQObjCacheManager().getAllVehicle();
                foreach (var vh in vhs)
                {
                    VehicleHardwareInfo receive_data = null;
                    try
                    {
                        receive_data =
                            scApp.getFunBaseObj<VehicleHardwareInfo>(vh.VEHICLE_ID) as VehicleHardwareInfo;
                        receive_data.Read(scApp.getBCFApplication(), vh.EqptObjectCate, vh.VEHICLE_ID);
                        receive_data.Timestamp = DateTime.Now;
                        logger.Info(receive_data.ToString());
                        //SCUtility.RecordLog(logger_PLCConverLog, receive_data);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "Exception:");
                    }
                    finally
                    {
                        scApp.putFunBaseObj<VehicleHardwareInfo>(receive_data);
                        System.Threading.Interlocked.Exchange(ref syncPoint, 0);
                    }
                }
            }
        }
    }
}
