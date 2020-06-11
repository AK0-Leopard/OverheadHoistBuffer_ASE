using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.Data.VO.Interface;
using com.mirle.ibg3k0.sc.ObjectRelay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc
{
    public partial class ATRACESET : PropertyChangedVO
    {
        public virtual List<ATRACEITEM> TraceItemList { get; set; }


        /// <summary>
        /// Calculates the next SMP time.
        /// </summary>
        /// <returns>DateTime.</returns>
        public DateTime calcNextSmpTime()
        {
            int hours = Convert.ToInt32(SMP_PERIOD.Substring(0, 2));
            int minutes = Convert.ToInt32(SMP_PERIOD.Substring(2, 2));
            int seconds = Convert.ToInt32(SMP_PERIOD.Substring(4, 2));
            long totalSeconds =(long) new TimeSpan(hours, minutes, seconds).TotalSeconds;
            SMP_PERIOD_SEC = totalSeconds;
            if (SMP_TIME == null)
            {
                SMP_TIME = DateTime.Now;
            }
            NX_SMP_TIME = SMP_TIME.AddSeconds(totalSeconds);
            return NX_SMP_TIME;
        }

    }

}
