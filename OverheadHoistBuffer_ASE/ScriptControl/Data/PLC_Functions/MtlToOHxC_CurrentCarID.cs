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
    class MtlToOHxC_CurrentCarID : PLC_FunBase
    {
        public DateTime Timestamp;
        [PLCElement(ValueName = "MTL_TO_OHXC_CURRENT_CAR_ID_MTL_STATION_ID")]
        public UInt16 MTLStationID;
        [PLCElement(ValueName = "MTL_TO_OHXC_CURRENT_CAR_ID_CAR_ID")]
        public UInt16 CarID;
        [PLCElement(ValueName = "MTL_TO_OHXC_CURRENT_CAR_ID_INDEX")]
        public UInt16 Index;
    }


}
