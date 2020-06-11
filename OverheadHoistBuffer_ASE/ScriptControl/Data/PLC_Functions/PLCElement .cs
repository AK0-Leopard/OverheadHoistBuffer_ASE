using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.PLC_Functions
{
    public class PLCElement : Attribute
    {
        public string ValueName { get; set; }
        public bool IsHandshakeProp { get; set; }
        public bool IsIndexProp { get; set; }
    }
}
