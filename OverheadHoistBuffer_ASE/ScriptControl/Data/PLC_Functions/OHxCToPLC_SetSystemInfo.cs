using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.PLC_Functions
{
    class OHxCToPLC_SetSystemInfo : PLC_FunBase
    {
        public DateTime Timestamp;

        [PLCElement(ValueName = "BCD_YEAR_MONTH")]
        public UInt16 bcdYearMonth;
        [PLCElement(ValueName = "BCD_DAY_HOUR")]
        public UInt16 bcdDayHour;
        [PLCElement(ValueName = "BCD_MINUTE_SECOND")]
        public UInt16 bcdMinuteSecond;
        [PLCElement(ValueName = "TIME_CALIBRATION")]
        public bool setTime;

        [PLCElement(ValueName = "PLC_HEARTBEAT")]
        public bool PLCHeartBeat;
    }

    class OHxCToPLC_SetSystemInfo_SetTime : PLC_FunBase
    {
        public DateTime Timestamp;

        [PLCElement(ValueName = "BCD_YEAR_MONTH")]
        public UInt16 bcdYearMonth;
        [PLCElement(ValueName = "BCD_DAY_HOUR")]
        public UInt16 bcdDayHour;
        [PLCElement(ValueName = "BCD_MINUTE_SECOND")]
        public UInt16 bcdMinuteSecond;

        [PLCElement(ValueName = "TIME_CALIBRATION")]
        public bool setTime;
    }

    class OHxCToPLC_SetSystemInfo_SetAlive : PLC_FunBase
    {
        public DateTime Timestamp;

        [PLCElement(ValueName = "PLC_HEARTBEAT")]
        public bool PLCHeartBeat;
    }

    class OHxCToPLC_SetSystemInfo_SetMCSOnline : PLC_FunBase
    {
        public DateTime Timestamp;

        [PLCElement(ValueName = "MCS_ONLINE")]
        public bool MCSOnline;
    }
}

