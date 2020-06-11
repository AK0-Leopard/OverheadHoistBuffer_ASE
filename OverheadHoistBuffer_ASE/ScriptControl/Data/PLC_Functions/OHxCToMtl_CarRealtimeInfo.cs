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
    class OHxCToMtl_CarRealtimeInfo : PLC_FunBase
    {
        [PLCElement(ValueName = "OHXC_TO_MTL_CAR_OUT_REALTIME_INFO_CAR_ID")]
        public UInt16 CarID;
        [PLCElement(ValueName = "OHXC_TO_MTL_CAR_OUT_REALTIME_INFO_ACTION_MODE")]
        public UInt16 ActionMode;
        [PLCElement(ValueName = "OHXC_TO_MTL_CAR_OUT_REALTIME_INFO_CST_EXIST")]
        public UInt16 CSTExist;
        [PLCElement(ValueName = "OHXC_TO_MTL_CAR_OUT_REALTIME_INFO_CURRENT_SECTION_ID")]
        public UInt16 CurrentSectionID;
        [PLCElement(ValueName = "OHXC_TO_MTL_CAR_OUT_REALTIME_INFO_CURRENT_ADDRESS_ID")]
        public UInt32 CurrentAddressID;
        [PLCElement(ValueName = "OHXC_TO_MTL_CAR_OUT_REALTIME_INFO_CURRENT_BUFFER_DISTANCE")]
        public UInt32 BufferDistance;
        [PLCElement(ValueName = "OHXC_TO_MTL_CAR_OUT_REALTIME_INFO_CURRENT_SPEED")]
        public UInt16 Speed;
        [PLCElement(ValueName = "OHXC_TO_MTL_CAR_OUT_REALTIME_INFO_INDEX")]
        public UInt16 Index;
    }


}
