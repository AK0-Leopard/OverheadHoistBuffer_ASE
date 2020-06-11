using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using Newtonsoft.Json;

namespace com.mirle.ibg3k0.sc.Data.PLC_Functions
{
    public class VehicleCSTInterface : PLC_FunBase
    {
        [PLCElement(ValueName = "IF_INDEX")]
        public UInt16 Index;
        public List<CSTInterfaceDetail> Details = new List<CSTInterfaceDetail>();

        public override string ToString()
        {
            string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            return sJson;
        }

        public new void Read(BCFApplication bcfApp, string eqObjIDCate, string eq_id, int item_count)
        {
            for (int i = 1; i <= item_count; i++)
            {
                CSTInterfaceDetail interfaceDetail = new CSTInterfaceDetail();
                interfaceDetail.LogIndex = $"Recode{nameof(VehicleCSTInterface)}";
                interfaceDetail.EQ_ID = this.EQ_ID;
                interfaceDetail.Read(bcfApp, eqObjIDCate, eq_id, i);
                Details.Add(interfaceDetail);
            }
            base.Read(bcfApp, eqObjIDCate, eq_id);
        }

        public class CSTInterfaceDetail : PLC_FunBase
        {
            [JsonIgnore]
            [PLCElement(ValueName = "IF_PORT_")]
            public UInt16 PortID;
            [JsonIgnore]
            [PLCElement(ValueName = "IF_YEAR_")]
            public UInt16 Year;
            [JsonIgnore]
            [PLCElement(ValueName = "IF_MONTH_")]
            public UInt16 Month;
            [JsonIgnore]
            [PLCElement(ValueName = "IF_DAY_")]
            public UInt16 Day;
            [JsonIgnore]
            [PLCElement(ValueName = "IF_HOUR_")]
            public UInt16 Hour;
            [JsonIgnore]
            [PLCElement(ValueName = "IF_MINUTE_")]
            public UInt16 Minute;
            [JsonIgnore]
            [PLCElement(ValueName = "IF_SECOND_")]
            public UInt16 Second;
            [JsonIgnore]
            [PLCElement(ValueName = "IF_MILLISECOND_")]
            public UInt16 Millisecond;

            public string PORT_ID
            { get { return PortID.ToString(); } }

            public DateTime Timestamp
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

            [JsonIgnore]
            [PLCElement(ValueName = "IF_INTERFAECSIGNAL_")]
            public Boolean[] CSTInterface;
            public bool VALID { get { return getBoolValue(0); } }
            public bool CS_0 { get { return getBoolValue(1); } }
            public bool CS_1 { get { return getBoolValue(2); } }
            public bool AM_AVBL { get { return getBoolValue(3); } }
            public bool TR_REQ { get { return getBoolValue(4); } }
            public bool BUSY { get { return getBoolValue(5); } }
            public bool COMPT { get { return getBoolValue(6); } }
            public bool CONT { get { return getBoolValue(7); } }
            public bool L_REQ { get { return getBoolValue(8); } }
            public bool U_REQ { get { return getBoolValue(9); } }
            public bool VA { get { return getBoolValue(10); } }
            public bool READY { get { return getBoolValue(11); } }
            public bool VS_0 { get { return getBoolValue(12); } }
            public bool VS_1 { get { return getBoolValue(13); } }
            public bool HO_AVBL { get { return getBoolValue(14); } }
            public bool ES { get { return getBoolValue(15); } }

            [JsonIgnore]
            public override string EQ_ID { get => base.EQ_ID; set => base.EQ_ID = value; }
            public string VH_ID { get => base.EQ_ID; }
            private bool getBoolValue(int index)
            {
                if (CSTInterface == null) return false;
                return CSTInterface[index];
            }

            public override string ToString()
            {
                string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(this, JsHelper.jsBooleanConverter, JsHelper.jsTimeConverter);
                sJson = sJson.Replace(nameof(Timestamp), "@timestamp");
                sJson = sJson.Replace(nameof(LogIndex), "Index");
                return sJson;
            }

        }
    }
}
