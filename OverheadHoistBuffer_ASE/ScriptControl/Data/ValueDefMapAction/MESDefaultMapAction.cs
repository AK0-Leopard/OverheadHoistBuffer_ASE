//*********************************************************************************
//      MESDefaultMapAction.cs
//*********************************************************************************
// File Name: MESDefaultMapAction.cs
// Description: 與EAP通訊的劇本
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using System;
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.TimerAction;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.stc.Common;
using com.mirle.ibg3k0.stc.Data.SecsData;
using NLog;

namespace com.mirle.ibg3k0.sc.Data.ValueDefMapAction
{
    /// <summary>
    /// Class MESDefaultMapAction.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.ValueDefMapAction.IValueDefMapAction" />
    public class MESDefaultMapAction : IValueDefMapAction
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// The glass TRN logger
        /// </summary>
        private static Logger GlassTrnLogger = LogManager.GetLogger("GlassTransferRpt_EAP");
        /// <summary>
        /// The logger_ map action log
        /// </summary>
        protected static Logger logger_MapActionLog = LogManager.GetLogger("MapActioLog");
        /// <summary>
        /// The sc application
        /// </summary>
        protected SCApplication scApp = null;
        /// <summary>
        /// The BCF application
        /// </summary>
        protected BCFApplication bcfApp = null;
        /// <summary>
        /// The line
        /// </summary>
        protected ALINE line = null;
        /// <summary>
        /// The event BLL
        /// </summary>
        protected EventBLL eventBLL = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="MESDefaultMapAction"/> class.
        /// </summary>
        public MESDefaultMapAction()
        {
            scApp = SCApplication.getInstance();
            bcfApp = scApp.getBCFApplication();
            eventBLL = scApp.EventBLL;
        }

        /// <summary>
        /// Gets the identity key.
        /// </summary>
        /// <returns>System.String.</returns>
        public virtual string getIdentityKey()
        {
            return this.GetType().Name;
        }

        /// <summary>
        /// Sets the context.
        /// </summary>
        /// <param name="baseEQ">The base eq.</param>
        public virtual void setContext(BaseEQObject baseEQ)
        {
            this.line = baseEQ as ALINE;
        }

        /// <summary>
        /// Uns the register event.
        /// </summary>
        public virtual void unRegisterEvent()
        {

        }

        /// <summary>
        /// Does the share memory initialize.
        /// </summary>
        /// <param name="runLevel">The run level.</param>
        public virtual void doShareMemoryInit(BCFAppConstants.RUN_LEVEL runLevel)
        {
            try
            {
                switch (runLevel)
                {
                    case BCFAppConstants.RUN_LEVEL.ZERO:
                        break;
                    case BCFAppConstants.RUN_LEVEL.ONE:
                        break;
                    case BCFAppConstants.RUN_LEVEL.TWO:
                        break;
                    case BCFAppConstants.RUN_LEVEL.NINE:
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
            }
        }

        /// <summary>
        /// 發送Are You There給MES(S1F1)
        /// </summary>
        /// <returns>Boolean.</returns>
        public Boolean sendS1F1_AreYouThere()
        {
            try
            {
                S1F1 s1f1 = new S1F1()
                {
                    SECSAgentName = scApp.EAPSecsAgentName
                };
                S1F2 s1f2 = null;
                string rtnMsg = string.Empty;
                SXFY abortSecs = null;
                SCUtility.secsActionRecordMsg(scApp, false, s1f1);
                TrxSECS.ReturnCode rtnCode = ISECSControl.sendRecv<S1F2>(bcfApp, s1f1, out s1f2, out abortSecs, out rtnMsg, null);
                SCUtility.actionRecordMsg(scApp, s1f1.StreamFunction, line.Real_ID,
                                "Send Are You There To MES.", rtnCode.ToString());
                if (rtnCode == TrxSECS.ReturnCode.Normal)
                {
                    SCUtility.secsActionRecordMsg(scApp, false, s1f2);  
                    return true;
                }
                else if (rtnCode == TrxSECS.ReturnCode.Abort)
                {
                    SCUtility.secsActionRecordMsg(scApp, false, abortSecs);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
            }
            return false;
        }

        #region SECS Link Status
        /// <summary>
        /// Secses the connected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SECSEventArgs"/> instance containing the event data.</param>
        protected void secsConnected(object sender, SECSEventArgs e)
        {
            //not implement
        }

        /// <summary>
        /// The host status timer
        /// </summary>
        HostStatusTimerAction hostStatusTimer = new HostStatusTimerAction("HostStatusTimerAction",
            SystemParameter.ControlStateKeepTimeSec * 1000);
        /// <summary>
        /// Secses the disconnected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SECSEventArgs"/> instance containing the event data.</param>
        protected void secsDisconnected(object sender, SECSEventArgs e)
        {
            //not implement
        }


        /// <summary>
        /// Class HostStatusTimerAction.
        /// </summary>
        /// <seealso cref="com.mirle.ibg3k0.bcf.Data.TimerAction.ITimerAction" />
        public class HostStatusTimerAction : ITimerAction
        {
            /// <summary>
            /// The is over desconnected time
            /// </summary>
            public Boolean isOverDesconnectedTime = true;
            /// <summary>
            /// The logger
            /// </summary>
            private static Logger logger = LogManager.GetCurrentClassLogger();
            /// <summary>
            /// The line
            /// </summary>
            ALINE line = SCApplication.getInstance().getEQObjCacheManager().getLine();
            /// <summary>
            /// Initializes a new instance of the <see cref="HostStatusTimerAction"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="intervalMilliSec">The interval milli sec.</param>
            public HostStatusTimerAction(string name, long intervalMilliSec)
                : base(name, intervalMilliSec)
            {

            }

            /// <summary>
            /// Initializes the start.
            /// </summary>
            public override void initStart()
            {
                this.IntervalMilliSec = SystemParameter.ControlStateKeepTimeSec * 1000;
                procCnt = 0;
            }

            /// <summary>
            /// The proc count
            /// </summary>
            private int procCnt = 0;
            /// <summary>
            /// Timer Action的執行動作
            /// </summary>
            /// <param name="obj">The object.</param>
            public override void doProcess(object obj)
            {
                if (IntervalMilliSec > 0 && procCnt++ <= 0) { return; }
                logger.Info("Disconnected Over Time[{0}]", IntervalMilliSec);
                //逾時一律Offline
                line.Host_Control_State = SCAppConstants.LineHostControlState.HostControlState.EQ_Off_line;
                isOverDesconnectedTime = true;
                this.stop();
            }
        }
        #endregion SECS Link Status

        /// <summary>
        /// Detecteds the disable over time flag.
        /// </summary>
        private void detectedDisableOverTimeFlag()
        {
            if (line.Host_Control_State != SCAppConstants.LineHostControlState.HostControlState.EQ_Off_line)
            {
                hostStatusTimer.isOverDesconnectedTime = false;
            }
        }

        /// <summary>
        /// Does the initialize.
        /// </summary>
        public virtual void doInit()
        {
            //not implement
        }
    }
}
