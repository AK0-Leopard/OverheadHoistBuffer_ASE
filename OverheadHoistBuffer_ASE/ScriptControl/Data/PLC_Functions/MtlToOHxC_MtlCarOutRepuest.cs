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
    class MtlToOHxC_MtlCarOutRepuest : PLC_FunBase
    {
        public DateTime Timestamp;
        [PLCElement(ValueName = "MTL_TO_OHXC_MTL_CAR_OUT_REQUEST_MTL_STATION_ID")]
        public UInt16 MTLStationID;
        [PLCElement(ValueName = "MTL_TO_OHXC_MTL_CAR_OUT_REQUEST_CAR_ID")]
        public UInt16 CarID;
        [PLCElement(ValueName = "MTL_TO_OHXC_MTL_CAR_OUT_CANCEL")]
        public UInt16 Canacel;
        [PLCElement(ValueName = "MTL_TO_OHXC_MTL_CAR_OUT_REQUEST_HS")]
        public UInt16 Handshake;
    }

    class OHxCToMtl_MtlCarOutReply : PLC_FunBase
    {
        public DateTime Timestamp;
        [PLCElement(ValueName = "OHXC_TO_MTL_CAR_OUT_REPLY_RETURN_CODE")]
        public UInt16 ReturnCode;
        [PLCElement(ValueName = "OHXC_TO_MTL_CAR_OUT_REPLY_HS")]
        public UInt16 Handshake;
    }

}
