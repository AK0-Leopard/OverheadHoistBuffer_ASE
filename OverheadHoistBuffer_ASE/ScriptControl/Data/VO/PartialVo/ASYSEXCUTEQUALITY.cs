using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.Data.VO.Interface;
using com.mirle.ibg3k0.sc.ObjectRelay;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace com.mirle.ibg3k0.sc
{
    [Serializable]
    public partial class ASYSEXCUTEQUALITY
    {
        public string Index = "SysExcuteQuality";
        public string CST_ID;
        public ProtocolFormat.OHTMessage.CompleteStatus CompleteStatus;


        static JsTimeConverter jsTimeConverter = new JsTimeConverter();
        static JsNullableTimeConverter  jsNullableTimeConverter = new JsNullableTimeConverter();
        public override string ToString()
        {
            string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(this, jsTimeConverter, jsNullableTimeConverter);
            sJson = sJson.Replace(nameof(CMD_INSERT_TIME), "@timestamp");
            sJson = sJson.Replace(nameof(CMD_ID_MCS), "MCS_CMD_ID");
            return sJson;
        }        
    }
    

    public class JsTimeConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //if (reader.TokenType == JsonToken.None) return null;
            //var time = (long)serializer.Deserialize(reader, typeof(long));
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var item = (DateTime)value;
            writer.WriteValue(ConvertToJsTime(item));
            writer.Flush();
        }

        public string ConvertToJsTime(DateTime value)
        {
            return value.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture); ;
        }

        public static DateTime ConvertJsTimeToNormalTime(long ticks, bool isTaiwanTime = true)
        {
            return new DateTime(1970, 1, 1).AddMilliseconds(ticks).AddHours(isTaiwanTime ? 8 : 0);
        }       
    }

    public class JsNullableTimeConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Nullable<System.DateTime>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //if (reader.TokenType == JsonToken.None) return null;
            //var time = (long)serializer.Deserialize(reader, typeof(long));
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var item = (DateTime)value;
            writer.WriteValue(ConvertToJsTime(item));
            writer.Flush();
        }

        public string ConvertToJsTime(Nullable<System.DateTime> value)
        {
            if (value.HasValue)
            {
                return value.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture);
            }
            else
            {
                return string.Empty;
            }            
        }

        public static DateTime ConvertJsTimeToNormalTime(long ticks, bool isTaiwanTime = true)
        {
            return new DateTime(1970, 1, 1).AddMilliseconds(ticks).AddHours(isTaiwanTime ? 8 : 0);
        }
    }


}
