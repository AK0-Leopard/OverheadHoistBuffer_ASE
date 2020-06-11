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
    class OHxCToMtl_DateTimeSync : PLC_FunBase
    {
        [PLCElement(ValueName = "OHXC_TO_MTL_DATE_TIME_SYNC_COMMAND_YEAR")]
        public uint Year;
        [PLCElement(ValueName = "OHXC_TO_MTL_DATE_TIME_SYNC_COMMAND_MONTH")]
        public uint Month;
        [PLCElement(ValueName = "OHXC_TO_MTL_DATE_TIME_SYNC_COMMAND_DAY")]
        public uint Day;
        [PLCElement(ValueName = "OHXC_TO_MTL_DATE_TIME_SYNC_COMMAND_HOUR")]
        public uint Hour;
        [PLCElement(ValueName = "OHXC_TO_MTL_DATE_TIME_SYNC_COMMAND_MINUTE")]
        public uint Min;
        [PLCElement(ValueName = "OHXC_TO_MTL_DATE_TIME_SYNC_COMMAND_SECOND")]
        public uint Sec;
        [PLCElement(ValueName = "OHXC_TO_MTL_DATE_TIME_SYNC_COMMAND_INDEX", IsIndexProp = true)]
        public uint Index;
    }


}
