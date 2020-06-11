using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction;
using com.mirle.ibg3k0.sc.Data.VO.Interface;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    public class OHCV : AEQPT
    {
        public string SegmentLocation { get; private set; }
        public void setSegmentLocation(string segID)
        {
            SegmentLocation = segID;
        }
    }
}
