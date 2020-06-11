using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.PLC_Functions
{
    public class VehicleInfoFromPLC : PLC_FunBase
    {

        public DateTime Timestamp;
        [JsonIgnore]
        [PLCElement(ValueName = "VH_ID")]
        public UInt16 vh_id;
        public String VH_ID { get { return vh_id.ToString(); } }
        [JsonIgnore]
        [PLCElement(ValueName = "VH_CUR_SEC_ID")]
        public UInt16 cur_sec_id;
        public String CUR_SEC_ID { get { return cur_sec_id.ToString(); } }
        [PLCElement(ValueName = "VH_CUR_SEC_DIST")]
        public UInt32 CUR_SEC_DIST;
        [JsonIgnore]
        [PLCElement(ValueName = "VH_CUR_ADR_ID")]
        public UInt32 cur_adr_id;
        public String CUR_ADR_ID { get { return cur_adr_id.ToString(); } }
        [JsonIgnore]
        [PLCElement(ValueName = "VH_TRAN_CMD_ID")]
        public UInt64 tran_cmd_id;
        public String TRAN_CMD_ID { get { return tran_cmd_id.ToString(); } }

        [PLCElement(ValueName = "VH_ACTION_STATUS")]
        public UInt16 ACTION_STATUS;

        [PLCElement(ValueName = "VH_HAS_CST")]
        public UInt16 HAS_CST;
        [PLCElement(ValueName = "VH_CST_ID")]
        public string CST_ID;
        [PLCElement(ValueName = "VH_OBS_PAUSE")]
        public UInt16 OBS_PAUSE;
        [PLCElement(ValueName = "VH_BLOCK_PAUSE")]
        public UInt16 BLOCK_PAUSE;
        [PLCElement(ValueName = "VH_NORMAL_PAUSE")]
        public UInt16 NORMAL_PAUSE;
        [PLCElement(ValueName = "VH_HID_PAUSE")]
        public UInt16 HID_PAUSE;
        [PLCElement(ValueName = "VH_ERROR_PAUSE")]
        public UInt16 ERROR_PAUSE;

        [JsonIgnore]
        [PLCElement(ValueName = "VH_CUR_BLOCK_ID")]
        public UInt16 cur_block_id;
        public String CUR_BLOCK_ID { get { return cur_block_id.ToString(); } }
        [JsonIgnore]
        [PLCElement(ValueName = "VH_CUR_HID_ID")]
        public UInt16 cur_hid_id;
        public String CUR_HID_ID { get { return cur_hid_id.ToString(); } }
        [PLCElement(ValueName = "VH_VH_MODE_STATUS")]
        public UInt16 VH_MODE_STATUS;
        [PLCElement(ValueName = "VH_VH_SPEED_MIN")]
        public UInt32 VH_SPEED_MIN;

        [PLCElement(ValueName = "VH_VH_ENCODER_VALUE")]
        public UInt32 VH_ENCODER_VALUE;
        /// <summary>
        /// Magnetometer Value(磁力尺的值)
        /// </summary>
        [PLCElement(ValueName = "VH_VH_MAG_VALUE")]
        public UInt32 VH_MAG_VALUE;
        [PLCElement(ValueName = "VH_SPEED_LIMIT")]
        public UInt32 SPEED_LIMIT;
        [PLCElement(ValueName = "VH_LEFT_GUIDE_STATUS")]
        public UInt16 LEFT_GUIDE_STATUS;
        [PLCElement(ValueName = "VH_RIGHT_GUIDE_STATUS")]
        public UInt16 RIGHT_GUIDE_STATUS;
        /// <summary>
        /// Distance deviation(Section 的偏差)
        /// </summary>
        [PLCElement(ValueName = "VH_SEC_DIST_DEV")]
        //public UInt32 SEC_DIST_DEV;
        public Int32 SEC_DIST_DEV;
        /// <summary>
        /// 偏差的SEC ID
        /// </summary>
        [JsonIgnore]
        [PLCElement(ValueName = "VH_DEV_SEC_ID")]
        public UInt16 dev_sec_id;
        public String DEV_SEC_ID { get { return dev_sec_id.ToString(); } }

        /// <summary>
        /// 0: Power OFF、1:Power ON
        /// </summary>
        [PLCElement(ValueName = "VH_POWER_STATUS")]
        public UInt16 POWER_STATUS;

        /// <summary>
        /// 從當前位址開始走行距離
        /// </summary>
        [PLCElement(ValueName = "VH_ACC_SEC_DIST")]
        public UInt32 ACC_SEC_DIST;

        [JsonIgnore]
        [PLCElement(ValueName = "VH_YEAR")]
        public UInt16 Year;
        [JsonIgnore]
        [PLCElement(ValueName = "VH_MONTH")]
        public UInt16 Month;
        [JsonIgnore]
        [PLCElement(ValueName = "VH_DAY")]
        public UInt16 Day;
        [JsonIgnore]
        [PLCElement(ValueName = "VH_HOUR")]
        public UInt16 Hour;
        [JsonIgnore]
        [PLCElement(ValueName = "VH_MINUTE")]
        public UInt16 Minute;
        [JsonIgnore]
        [PLCElement(ValueName = "VH_SECOND")]
        public UInt16 Second;
        [JsonIgnore]
        [PLCElement(ValueName = "VH_MILLISECOND")]
        public UInt16 Millisecond;
        public DateTime PLC_Datetime
        {
            get
            {
                string year = $"{Year:0000}";
                string minth = $"{Month:00}";
                string day = $"{Day:00}";
                string hour = $"{Hour:00}";
                string minute = $"{Minute:00}";
                string second = $"{Second:00}";
                string millisecion = $"{Millisecond:000}";
                string dateTime = $"{year}{minth}{day}{hour}{minute}{second}{millisecion}";
                DateTime parseDateTime = default(DateTime);
                DateTime.TryParseExact(dateTime, SCAppConstants.TimestampFormat_17, CultureInfo.InvariantCulture, DateTimeStyles.None, out parseDateTime);
                return parseDateTime;
            }
        }



        public override string ToString()
        {
            string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(this, JsHelper.jsBooleanArrayConverter, JsHelper.jsTimeConverter);
            sJson = sJson.Replace(nameof(Timestamp), "@timestamp");
            return sJson;
        }
    }
}
