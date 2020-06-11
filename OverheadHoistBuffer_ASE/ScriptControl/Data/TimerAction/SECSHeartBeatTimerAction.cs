// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="SECSHeartBeatTimerAction.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Data.TimerAction;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;

namespace com.mirle.ibg3k0.sc.Data.TimerAction
{
    public class SECSHeartBeatTimerAction : ITimerAction
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private SCApplication scApp = null;
        ALINE line = null;
        private int retryCount = 0;
        private int retryCount_ChangeHostStat = 1;      //A0.02 
        private int retryCount_restartEAPSECSAgent = 3; //A0.02 

        /// <summary>
        /// Initializes a new instance of the <see cref="SECSHeartBeatTimerAction"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="intervalMilliSec">The interval milli sec.</param>
        public SECSHeartBeatTimerAction(string name, long intervalMilliSec)
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
            //this.IntervalMilliSec = SystemParameter.HeartBeatSec * 1000;
        }

        /// <summary>
        /// The synchronize point
        /// </summary>
        private long syncPoint = 0;
        /// <summary>
        /// Timer Action的執行動作
        /// </summary>
        /// <param name="obj">The object.</param>
        public override void doProcess(object obj)
        {
            if (System.Threading.Interlocked.Exchange(ref syncPoint, 1) == 0)
            {
                try
                {
                    if (line.ServiceMode != SCAppConstants.AppServiceMode.Active) return;
                    if (IntervalMilliSec > 0)
                    {
                        //A0.01 mesMapAction.sendS1F1_AreYouThere();

                        //A0.01 Start 
                        if (scApp.ReportBLL.AskAreYouThere())
                        {
                            retryCount = 0;
                            //mesMapAction.secsConnected();
                        }
                        else
                        {
                            retryCount++;
                            LogManager.GetLogger("SXFYLogger").Warn(string.Format("Send S1F1 Fail (Hear Bear)!!,fail count:[{0}]", retryCount));
                            if (retryCount >= retryCount_restartEAPSECSAgent)                                       //A0.02
                            {                                                                                       //A0.02
                                LogManager.GetLogger("SXFYLogger").Warn("Beginning restart EAP SECS agent !!");     //A0.02
                                this.stop();                                                                        //A0.02
                                retryCount = 0;                                                                     //A0.02
                                scApp.getBCFApplication().getSECSAgent(scApp.EAPSecsAgentName).refreshConnection(); //A0.02 
                            }                                                                                       //A0.02
                            else if (retryCount == retryCount_ChangeHostStat)                                       //A0.02
                            {                                                                                       //A0.02
                                LogManager.GetLogger("SXFYLogger").Warn("Beginning Change MPC SECS Status !!");     //A0.02
                                //mesMapAction.secsDisconnected();
                            }
                        }
                    }
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref syncPoint, 0);
                }
            }
        }
    }
}
