using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.sc.Common;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    public class MainAlarm
    {
        public virtual string CODE { get; set; }
        public virtual string DESCRIPTION { get; set; }
        public virtual string REMARK { get; set; }
        public virtual string ACTION { get; set; }
    }
}
