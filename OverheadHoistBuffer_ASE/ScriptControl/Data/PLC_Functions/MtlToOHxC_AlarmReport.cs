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
    class MtlToOHxC_AlarmReport : PLC_FunBase
    {
        public DateTime Timestamp;
        [PLCElement(ValueName = "MTL_TO_OHXC_ALARM_REPORT_ERROR_CODE")]
        public UInt16 ErrorCode;
        [PLCElement(ValueName = "MTL_TO_OHXC_ALARM_REPORT_ERROR_STATUS")]
        public UInt16 ErrorStatus;
        [PLCElement(ValueName = "MTL_TO_OHXC_ALARM_REPORT_HS")]
        public UInt16 Handshake;
    }

    class MtlToOHxC_ReplyAlarmReport : PLC_FunBase
    {
        public DateTime Timestamp;
        [PLCElement(ValueName = "OHXC_TO_MTL_REPLY_ALARM_REPORT_HS")]
        public UInt16 Handshake;
    }

}
