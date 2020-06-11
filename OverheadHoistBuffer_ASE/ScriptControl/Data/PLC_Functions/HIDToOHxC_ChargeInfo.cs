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
    public class HIDToOHxC_ChargeInfo : PLC_FunBase
    {

        public DateTime Timestamp;

        [PLCElement(ValueName = "HID_TO_OHXC_ALIVE")]
        public UInt16 Alive;
        [PLCElement(ValueName = "HID_TO_OHXC_STATION_ID")]
        public UInt16 Station_ID;
        [PLCElement(ValueName = "HID_TO_OHXC_HID_ID")]
        public UInt16 HID_ID;
        [PLCElement(ValueName = "HID_TO_OHXC_V_UNIT")]
        public UInt16 V_Unit;
        [PLCElement(ValueName = "HID_TO_OHXC_V_DOT")]
        public UInt16 V_Dot;
        [PLCElement(ValueName = "HID_TO_OHXC_A_UNIT")]
        public UInt16 A_Unit;
        [PLCElement(ValueName = "HID_TO_OHXC_A_DOT")]
        public UInt16 A_Dot;
        [PLCElement(ValueName = "HID_TO_OHXC_W_UNIT")]
        public UInt16 W_Unit;
        [PLCElement(ValueName = "HID_TO_OHXC_W_DOT")]
        public UInt16 W_Dot;
        [PLCElement(ValueName = "HID_TO_OHXC_HOUR_UNIT")]
        public UInt16 Hour_Unit;
        [PLCElement(ValueName = "HID_TO_OHXC_HOUR_DOT")]
        public UInt16 Hour_Dot;
        [PLCElement(ValueName = "HID_TO_OHXC_HOUR_SIGMA_Hi_WORD")]
        public UInt16 Hour_Sigma_High_Word;
        [PLCElement(ValueName = "HID_TO_OHXC_HOUR_SIGMA_Lo_WORD")]
        public UInt16 Hour_Sigma_Low_Word;
        [PLCElement(ValueName = "HID_TO_OHXC_HOUR_POSITIVE_Hi_WORD")]
        public UInt16 Hour_Positive_High_Word;
        [PLCElement(ValueName = "HID_TO_OHXC_HOUR_POSITIVE_Lo_WORD")]
        public UInt16 Hour_Positive_Low_Word;
        [PLCElement(ValueName = "HID_TO_OHXC_HOUR_NEGATIVE_Hi_WORD")]
        public UInt16 Hour_Negative_High_Word;
        [PLCElement(ValueName = "HID_TO_OHXC_HOUR_NEGATIVE_Lo_WORD")]
        public UInt16 Hour_Negative_Low_Word;
        [PLCElement(ValueName = "HID_TO_OHXC_VR")]
        public UInt16 VR_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_VS")]
        public UInt16 VS_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_VT")]
        public UInt16 VT_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_SIGMA_V")]
        public UInt16 Sigma_V_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_AR")]
        public UInt16 AR_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_AS")]
        public UInt16 AS_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_AT")]
        public UInt16 AT_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_SIGMA_A")]
        public UInt16 Sigma_A_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_WR")]
        public UInt16 WR_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_WS")]
        public UInt16 WS_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_WT")]
        public UInt16 WT_Source;
        [PLCElement(ValueName = "HID_TO_OHXC_SIGMA_W")]
        public UInt16 Sigma_W_Source;

        public UInt64 Hour_Sigma_Converted { get { return convertValueTwoWord(Hour_Unit, Hour_Dot, Hour_Sigma_High_Word, Hour_Sigma_Low_Word); } set { } }

        public UInt64 Hour_Positive_Converted { get { return convertValueTwoWord(Hour_Unit, Hour_Dot, Hour_Positive_High_Word, Hour_Positive_Low_Word); } set { } }
        public UInt64 Hour_Negative_Converted { get { return convertValueTwoWord(Hour_Unit, Hour_Dot, Hour_Negative_High_Word, Hour_Negative_Low_Word); } set { } }
        public UInt64 VR_Converted { get { return convertValueOneWord(V_Unit, V_Dot, VR_Source); } set { } }
        public UInt64 VS_Converted { get { return convertValueOneWord(V_Unit, V_Dot, VS_Source); } set { } }
        public UInt64 VT_Converted { get { return convertValueOneWord(V_Unit, V_Dot, VT_Source); } set { } }
        public UInt64 Sigma_V_Converted { get { return convertValueOneWord(V_Unit, V_Dot, Sigma_V_Source); } set { } }
        public UInt64 AR_Converted { get { return convertValueOneWord(A_Unit, A_Dot, AR_Source); } set { } }
        public UInt64 AS_Converted { get { return convertValueOneWord(A_Unit, A_Dot, AS_Source); } set { } }
        public UInt64 AT_Converted { get { return convertValueOneWord(A_Unit, A_Dot, AT_Source); } set { } }
        public UInt64 Sigma_A_Converted { get { return convertValueOneWord(A_Unit, A_Dot, Sigma_A_Source); } set { } }
        public UInt64 WR_Converted { get { return convertValueOneWord(W_Unit, W_Dot, WR_Source); } set { } }
        public UInt64 WS_Converted { get { return convertValueOneWord(W_Unit, W_Dot, WS_Source); } set { } }
        public UInt64 WT_Converted { get { return convertValueOneWord(W_Unit, W_Dot, WT_Source); } set { } }
        public UInt64 Sigma_W_Converted { get { return convertValueOneWord(W_Unit, W_Dot, Sigma_W_Source); } set { } }

        private UInt64 convertValueOneWord(UInt64 unit, UInt64 dot, UInt64 source_value)
        {
            UInt64 convertValue;
            UInt64 temp;
            double unit_d = Convert.ToDouble(unit);
            double dot_d = Convert.ToDouble(dot);
            UInt64 multiplier = Convert.ToUInt64(Math.Pow(10, (unit_d - dot_d)));
            temp = source_value * multiplier;
            convertValue = temp;
            return convertValue;
        }
        private UInt64 convertValueTwoWord(UInt64 unit, UInt64 dot, UInt64 source_value_high_word, UInt64 source_value_low_word)
        {
            UInt64 convertValue;
            UInt64 temp;
            UInt64 source_value = (source_value_high_word * 65536) + source_value_low_word;
            double unit_d = Convert.ToDouble(unit);
            double dot_d = Convert.ToDouble(dot);
            UInt64 multiplier = Convert.ToUInt64(Math.Pow(10, (unit_d - dot_d)));
            temp = source_value * multiplier;
            convertValue = temp;
            return convertValue;
        }

        public override string ToString()
        {
            string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(this, JsHelper.jsBooleanArrayConverter, JsHelper.jsTimeConverter);
            sJson = sJson.Replace(nameof(Timestamp), "@timestamp");
            return sJson;
        }
    }




}
