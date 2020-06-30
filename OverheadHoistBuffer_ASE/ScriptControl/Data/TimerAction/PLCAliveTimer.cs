//*********************************************************************************
//      PLCAliveTimer.cs
//*********************************************************************************
// File Name: PLCAliveTimer.cs
// Description: 讓PLC知道現在的連線狀態
// Reference: ASRS ←→PC 通訊手冊 v3.29
//
//(c) Copyright 2020, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag          Description
// ------------- -------------  -------------  ------       -----------------------------
// 2020/06/23    Hsinyu Chang   N/A            2020.6.23    加入MCS online signal
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Data.TimerAction;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction;

namespace com.mirle.ibg3k0.sc.Data.TimerAction
{
    public class PLCAliveTimer : ITimerAction
    {
        protected SCApplication scApp = null;
        private PLCSystemInfoMapAction systemInfoMapAction;
        private int retryCount = 0;
        private bool aliveSignal = false;
        private bool lastAliveSignal = false;
        private const int retryCountThl = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="SECSHeartBeatTimerAction"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="intervalMilliSec">The interval milli sec.</param>
        public PLCAliveTimer(string name, long intervalMilliSec)
            : base(name, intervalMilliSec)
        {
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
                    systemInfoMapAction = scApp.getEQObjCacheManager().getPortByPortID("MASTER_PLC")
                        .getMapActionByIdentityKey(typeof(PLCSystemInfoMapAction).Name) as PLCSystemInfoMapAction;
                    if (systemInfoMapAction == null) return;

                    if (IntervalMilliSec > 0)
                    {
                        aliveSignal = !aliveSignal;
                        systemInfoMapAction.PLC_SetHeartbeat(aliveSignal);

                        //2020.6.23 MCS online
                        bool MCSonline = (scApp.getEQObjCacheManager().getLine().Secs_Link_Stat == SCAppConstants.LinkStatus.LinkOK);
                        systemInfoMapAction.PLC_SetMCSOnline(MCSonline);
                    }

                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref syncPoint, 0);
                }
            }
        }

        /// <summary>
        /// Initializes the start.
        /// </summary>
        public override void initStart()
        {
            scApp = SCApplication.getInstance();
        }
    }
}
