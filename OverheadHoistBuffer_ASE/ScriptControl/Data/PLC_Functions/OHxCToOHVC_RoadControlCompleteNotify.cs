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
    public class OHxCToOHVC_RoadControlCompleteNotify : PLC_FunBase
    {
        [PLCElement(ValueName = "ROAD_CONTROL_COMPLETE_NOTIFY", IsHandshakeProp = true)]
        public bool Handshake;
    }
    public class OHxCToOHVC_RoadControlCompleteNotifyReply : PLC_FunBase
    {
        [PLCElement(ValueName = "REPLY_ROAD_CONTROL_COMPLETE_NOTIFY", IsHandshakeProp = true)]
        public bool Handshake;
    }

}
