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
    public class PLCTimeCalibrationTimer : ITimerAction
    {
        protected SCApplication scApp = null;
        private PLCSystemInfoMapAction systemInfoMapAction;
        private bool lastSignal = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="PLCTimeCalibrationTimer"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="intervalMilliSec">The interval milli sec.</param>
        public PLCTimeCalibrationTimer(string name, long intervalMilliSec)
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
                        if (lastSignal == false)
                        {
                            systemInfoMapAction.PLC_SetSystemTime();
                        }
                        else
                        {
                            systemInfoMapAction.PLC_FinishTimeCalibration();
                        }

                        lastSignal = !lastSignal;
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
