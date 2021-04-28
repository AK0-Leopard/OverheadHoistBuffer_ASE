using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Common
{
    public class CountDownTimerByStopwatch : Stopwatch
    {
        public int countDownTime_ms { get; private set; }
        public void StartCountDown(int _countDownTime_ms)
        {
            countDownTime_ms = _countDownTime_ms;
            this.Restart();
        }
        public bool isTimeout
        {
            get
            {
                if (App.SystemParameter.PreStageWatingTime_ms == 0)
                    return true;
                return this.ElapsedMilliseconds > countDownTime_ms;
            }
        }
    }
}
