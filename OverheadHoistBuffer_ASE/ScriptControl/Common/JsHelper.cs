using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Common
{
    public static class JsHelper
    {
        public static JsBooleanArrayConverter jsBooleanArrayConverter = new JsBooleanArrayConverter();
        public static JsTimeConverter jsTimeConverter = new JsTimeConverter();
        public static JsBooleanConverter jsBooleanConverter = new JsBooleanConverter();
        public static JsLogTypeConverter jsLogTypeConverter = new JsLogTypeConverter();
    }

    public class JsBooleanArrayConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Boolean[]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var item = value as Boolean[];
            writer.WriteValue(ConvertToJsBoolean(item));
            writer.Flush();
        }

        private string ConvertToJsBoolean(Boolean[] values)
        {
            if (values != null && values.Count() > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < values.Count(); i++)
                {
                    string sValue = values[i] ? "1" : "0";
                    string keyValue = $"Bit{i:X}:{sValue}";
                    sb.Append(keyValue);
                    if (i != values.Count() - 1)
                        sb.Append(" ");
                }
                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    }

    public class JsLogTypeConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(LogConstants.Type?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var item = (LogConstants.Type)value;
            writer.WriteValue(item.ToString());
            writer.Flush();
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

    }

    public class JsBooleanConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Boolean);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //if (reader.TokenType == JsonToken.None) return null;
            //var time = (long)serializer.Deserialize(reader, typeof(long));
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var item = (Boolean)value;
            writer.WriteValue(ConvertToJsTime(item));
            writer.Flush();
        }

        public int ConvertToJsTime(Boolean value)
        {
            return value ? 1 : 0;
        }
    }

}
