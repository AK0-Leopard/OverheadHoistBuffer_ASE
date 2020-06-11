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
using STAN.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.TimerAction
{
    public class FailOverTimerAction : ITimerAction
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        protected SCApplication scApp = null;
        protected ALINE line = null;
        public FailOverTimerAction(string name, long intervalMilliSec)
            : base(name, intervalMilliSec)
        {

        }

        public override void initStart()
        {
            scApp = SCApplication.getInstance();
            line = scApp.getEQObjCacheManager().getLine();
        }

        private long syncPoint = 0;
        public override void doProcess(object obj)
        {
            if (System.Threading.Interlocked.Exchange(ref syncPoint, 1) == 0)
            {
                try
                {

                    if (scApp.FailOverService.isActive())
                    {
                        if (line.ServiceMode != SCAppConstants.AppServiceMode.Active)
                        {
                            line.ServiceMode = SCAppConstants.AppServiceMode.Active;

                            //scApp.getBCFApplication().startTcpIpSecverListen();

                            // scApp.VehicleService.SubscriptionPositionChangeEvent();
                            if (scApp.LineBLL.isSegmentPreDisableExcuting())
                            {
                                scApp.LineBLL.BegingOrEndSegmentPreDisableExcute(true);
                            }
                            scApp.getNatsManager().Unsubscriber(SCAppConstants.NATS_SUBJECT_LINE_INFO);
                            logger.Info($"{ SCApplication.ServerName} become {line.ServiceMode}");
                        }
                    }
                    else
                    {
                        if (line.ServiceMode != SCAppConstants.AppServiceMode.Standby)
                        {
                            line.ServiceMode = SCAppConstants.AppServiceMode.Standby;
                            //scApp.VehicleService.UnsubscribePositionChangeEvent();
                            //scApp.getBCFApplication().ShutdownTcpIpSecverListen();
                            logger.Info($"{ SCApplication.ServerName} become {line.ServiceMode}");
                            scApp.getNatsManager().Subscriber(SCAppConstants.NATS_SUBJECT_LINE_INFO, ProcLineInfo, is_last: true);
                            line.ReStartStateMachine();
                        }
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

        private void ProcLineInfo(object sender, StanMsgHandlerArgs e)
        {
            var bytes = e.Message.Data;
            var line_info_gpb = BLL.LineBLL.Convert2Object_LineInfo(bytes);
            ALINE line = scApp.getEQObjCacheManager().getLine();
            line.Put(line_info_gpb);
        }


    }

}
