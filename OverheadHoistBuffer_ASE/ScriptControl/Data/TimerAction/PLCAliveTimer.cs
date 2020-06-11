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
                        //lastAliveSignal = aliveSignal;
                        //aliveSignal = systemInfoMapAction.PLCHeartbeatSignal;
                        //if (lastAliveSignal == aliveSignal)
                        //{
                        //    retryCount++;
                        //    if (retryCount > retryCountThl)
                        //    {
                        //        //TODO: PLC disconnected
                        //    }
                        //}
                        //else
                        //{
                        //    retryCount = 0;
                        //}
                        //systemInfoMapAction.PLC_SetHeartbeat(!aliveSignal);
                        aliveSignal = !aliveSignal;
                        systemInfoMapAction.PLC_SetHeartbeat(aliveSignal);
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
