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
    class MtlToOHxC_RequestCarInDataCheck : PLC_FunBase
    {
        public DateTime Timestamp;
        [PLCElement(ValueName = "MTL_TO_OHXC_REQUEST_CAR_IN_DATA_CHECK_MTL_STATION_ID")]
        public UInt16 MTLStationID;
        [PLCElement(ValueName = "MTL_TO_OHXC_REQUEST_CAR_IN_DATA_CHECK_CAR_ID")]
        public UInt16 CarID;
        [PLCElement(ValueName = "MTL_TO_OHXC_REQUEST_CAR_IN_DATA_CHECK_HS")]
        public UInt16 Handshake;
    }

    class OHxCToMtl_ReplyCarInDataCheck : PLC_FunBase
    {
        public DateTime Timestamp;
        [PLCElement(ValueName = "OHXC_TO_MTL_REPLY_CAR_IN_DATA_CHECK_RETURN_CODE")]
        public UInt16 ReturnCode;
        [PLCElement(ValueName = "OHXC_TO_MTL_REPLY_CAR_IN_DATA_CHECK_HS",IsHandshakeProp = true)]
        public UInt16 Handshake;
    }

}
