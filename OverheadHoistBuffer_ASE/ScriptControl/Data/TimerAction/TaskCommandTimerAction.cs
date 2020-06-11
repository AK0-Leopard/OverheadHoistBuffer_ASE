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
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using com.mirle.ibg3k0.stc.Common.SECS;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.TimerAction
{
    public class TaskCommandTimerAction : ITimerAction
    {
        const string CALL_CONTEXT_KEY_WORD_SERVICE_ID_TaskCmdTimerAction= "TaskCommandTimerAction Service";

        private static Logger logger = LogManager.GetCurrentClassLogger();
        protected SCApplication scApp = null;

        public TaskCommandTimerAction(string name, long intervalMilliSec)
            : base(name, intervalMilliSec)
        {

        }

        public override void initStart()
        {
            scApp = SCApplication.getInstance();
        }

        //private long wholeSyncPoint = 0;
        public override void doProcess(object obj)
        {
            try
            {
                LogHelper.setCallContextKey_ServiceID(CALL_CONTEXT_KEY_WORD_SERVICE_ID_TaskCmdTimerAction);
                scApp.CMDBLL.checkOHxC_TransferCommand();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
            }
        }

    }

}
