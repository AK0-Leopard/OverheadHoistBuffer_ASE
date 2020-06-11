using com.mirle.ibg3k0.sc.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.PLC_Functions
{
    public class OHxCToMtl_AlarmResetRequest : PLC_FunBase
    {
        [PLCElement(ValueName = "OHXC_TO_MTL_ALARM_RESET_REQUEST_HS", IsHandshakeProp = true)]
        public UInt16 Handshake;
    }
    public class MtlToOHxC_AlarmResetReply : PLC_FunBase
    {
        [PLCElement(ValueName = "MTL_TO_OHXC_REPLY_ALARM_RESET_HS", IsHandshakeProp = true)]
        public UInt16 Handshake;
    }

}
