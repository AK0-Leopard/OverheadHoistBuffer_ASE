//*********************************************************************************
//      SheetDao.cs
//*********************************************************************************
// File Name: SheetDao.cs
// Description: SheetDao類別
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2018/05/04    Kevin Wei       NONE          A0.01   增加Log的紀錄。
//**********************************************************************************
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Common.MPLC;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.sc.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.PLC_Functions
{
    public class PLC_FunBase
    {
        protected FieldInfo[] fieldInfos = null;
        protected string HandshakePropName;
        protected string IndexPropName;
        public virtual string EQ_ID { get; set; } = string.Empty;
        public virtual string LogIndex { get; set; } = string.Empty;

        public PLC_FunBase()
        {
            fieldInfos = GetPLCElementFields(this.GetType());
            LogIndex = $"Recode{(fieldInfos[0] as MemberInfo).DeclaringType.Name}";
        }
        public void initial(string eq_id)
        {
            EQ_ID = eq_id;
        }

        public virtual void Read(BCFApplication bcfApp, string eqObjIDCate, string eq_id)
        {
            List<ValueRead> listVR = new List<ValueRead>();
            foreach (FieldInfo info in fieldInfos)
            {
                ValueRead vr = null;
                PLCElement element = getPLCElementAttr(info);
                if (bcfApp.tryGetReadValueEventstring(eqObjIDCate, EQ_ID, element.ValueName, out vr))
                {
                    if (info.FieldType == typeof(Int32))
                        info.SetValue(this, Convert.ToInt32(vr.getText()));
                    else
                        info.SetValue(this, vr.getText());
                    listVR.Add(vr);
                }
                else
                {

                }
            }
            if (listVR.Count > 0)
            {
                BCFUtility.writeEquipmentLog(eq_id, listVR);
            }
        }

        public virtual void Read(BCFApplication bcfApp, string eqObjIDCate, string eq_id, int index)
        {
            List<ValueRead> listVR = new List<ValueRead>();
            foreach (FieldInfo info in fieldInfos)
            {
                ValueRead vr = null;
                PLCElement element = getPLCElementAttr(info);
                string value_name = $"{element.ValueName}{index}";
                if (bcfApp.tryGetReadValueEventstring(eqObjIDCate, EQ_ID, value_name, out vr))
                {
                    info.SetValue(this, vr.getText());
                    listVR.Add(vr);
                }
                else
                {

                }
            }
            if (listVR.Count > 0)
            {
                BCFUtility.writeEquipmentLog(eq_id, listVR);
            }
        }
        public virtual void Write(BCFApplication bcfApp, string eqObjIDCate, string eq_id)
        {
            ValueWrite ve_handshake = null;
            Write(bcfApp, eqObjIDCate, eq_id, out ve_handshake);
            if (ve_handshake != null)
            {
                SpinWait.SpinUntil(() => false, 500);
                ISMControl.writeDeviceBlock(bcfApp, ve_handshake);
            }
        }

        private void Write(BCFApplication bcfApp, string eqObjIDCate, string eq_id, out ValueWrite vw_handshake)
        {
            vw_handshake = null;
            EQ_ID = eq_id;
            List<ValueWrite> listVW = new List<ValueWrite>();
            foreach (FieldInfo info in fieldInfos)
            {
                PLCElement element = getPLCElementAttr(info);

                string value = string.Empty;
                UInt16[] ivalueArray = null;
                object valueObj = info.GetValue(this);
                if (valueObj is Enum)
                {
                    value = ((int)valueObj).ToString();
                }
                else if (valueObj is bool)
                {
                    value = ((bool)valueObj) ? "1" : "0";
                }
                else if (valueObj is UInt16[])
                {
                    ivalueArray = valueObj as UInt16[];
                }
                else
                {
                    object obj = info.GetValue(this);
                    if (obj != null)
                        value = info.GetValue(this).ToString();
                }

                ValueWrite vw = null;
                vw = bcfApp.getWriteValueEvent(eqObjIDCate, eq_id, element.ValueName);
                if (vw == null)
                {
                    throw new NullReferenceException($"Get ValueWrite:{element.ValueName} null,eqObjIDCate:{eqObjIDCate},eqptObjectID:{eq_id}");
                }
                if (valueObj is UInt16[])
                {
                    vw.setWriteValue(ivalueArray);
                }
                else
                {
                    vw.setWriteValue(value);
                }
                if (element.IsHandshakeProp
                    || element.IsIndexProp)
                {
                    vw_handshake = vw;
                }
                else
                {
                    ISMControl.writeDeviceBlock(bcfApp, vw);
                }
                listVW.Add(vw);
            }
            if (listVW.Count > 0)
            {
                BCFUtility.writeEquipmentLog(eq_id, listVW);
            }
        }
        public TrxMPLC.ReturnCode SendRecv(BCFApplication bcfApp, string eqObjIDCate, string eq_id, ValueRead replyMsg)
        {
            ValueWrite ve_handshake = null;
            Write(bcfApp, eqObjIDCate, eq_id, out ve_handshake);
            if (ve_handshake == null)
            {
                throw new NullReferenceException();
                //TODO Log
            }
            SpinWait.SpinUntil(() => false, 1000);
            return ISMControl.sendRecv(bcfApp, ve_handshake, replyMsg, 20);
            //return TrxMPLC.ReturnCode.Normal;
        }

        public bool resetHandshake(BCFApplication bcfApp, string eqObjIDCate, string eq_id)
        {
            ValueWrite handshake_vw = getValueWriteHandshake(bcfApp, eqObjIDCate, eq_id);
            List<ValueWrite> vws = new List<ValueWrite>(); //A0.01
            handshake_vw.initWriteValue();
            vws.Add(handshake_vw);                         //A0.01
            BCFUtility.writeEquipmentLog(eq_id, vws);      //A0.01
            return ISMControl.writeDeviceBlock(bcfApp, handshake_vw);
        }

        public ValueRead getValueReadHandshake(BCFApplication bcfApp, string eqObjIDCate, string eq_id)
        {
            ValueRead vr = null;
            if (!SCUtility.isEmpty(HandshakePropName))
            {
                if (!bcfApp.tryGetReadValueEventstring(eqObjIDCate, eq_id, HandshakePropName, out vr))
                {

                }
            }
            else if (!SCUtility.isEmpty(IndexPropName))
            {
                if (!bcfApp.tryGetReadValueEventstring(eqObjIDCate, eq_id, IndexPropName, out vr))
                {

                }
            }

            return vr;
        }

        public ValueWrite getValueWriteHandshake(BCFApplication bcfApp, string eqObjIDCate, string eq_id)
        {
            ValueWrite vw = null;
            vw = bcfApp.getWriteValueEvent(eqObjIDCate, eq_id, HandshakePropName);
            return vw;
        }

        public void reset()
        {
            foreach (FieldInfo info in fieldInfos)
            {
                info.SetValue(this, null);
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            //builder.AppendLine();
            if (fieldInfos == null || fieldInfos.Count() == 0) return string.Empty;
            string function_name = fieldInfos[0].DeclaringType.Name;

            builder.Append(string.Format(" {0} : [{1}]", "Obj", EQ_ID)).AppendLine();
            builder.Append(string.Format("{0} : [{1}]", "Func", function_name)).AppendLine();

            foreach (FieldInfo field in fieldInfos)
            {
                string name = field.Name;
                string sValue = string.Empty;
                if (field.FieldType == typeof(char[]))
                {
                    sValue = string.Join("", (char[])field.GetValue(this));
                }
                else if (field.FieldType == typeof(UInt16[]))
                {
                    sValue = string.Join(" ", (UInt16[])field.GetValue(this));
                }
                else
                {
                    object obj = field.GetValue(this);
                    sValue = obj == null ? string.Empty : obj.ToString();
                }
                builder.Append(" -");
                builder.Append(string.Format("{0} : {1}", name, sValue)).AppendLine();
            }
            return builder.ToString();
        }

        public FieldInfo[] GetPLCElementFields(Type type)
        {
            List<FieldInfo> rtnList = new List<FieldInfo>();
            FieldInfo[] tmpFieldAry = null;
            tmpFieldAry = type.GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach (FieldInfo field in tmpFieldAry)
            {
                if (field.FieldType.IsInterface) { continue; }
                PLCElement attr = getPLCElementAttr(field);
                if (attr == null) { continue; }
                if (attr.IsHandshakeProp)
                    HandshakePropName = attr.ValueName;
                else if (attr.IsIndexProp)
                    IndexPropName = attr.ValueName;
                rtnList.Add(field);
            }
            return rtnList.ToArray();
        }

        public PLCElement getPLCElementAttr(FieldInfo fieldInfo)
        {
            System.Attribute[] attrs = System.Attribute.GetCustomAttributes(fieldInfo);
            PLCElement attr = null;
            foreach (System.Attribute a in attrs)
            {
                if (a is PLCElement)
                {
                    attr = (PLCElement)a;
                    break;
                }
            }
            return attr;
        }




    }
    //public class JsBooleanArrayConverter : Newtonsoft.Json.JsonConverter
    //{
    //    public override bool CanConvert(Type objectType)
    //    {
    //        return objectType == typeof(Boolean[]);
    //    }

    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        return null;
    //    }

    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        var item = value as Boolean[];
    //        writer.WriteValue(ConvertToJsTime(item));
    //        writer.Flush();
    //    }

    //    public string ConvertToJsTime(Boolean[] values)
    //    {
    //        if (values != null && values.Count() > 0)
    //        {
    //            StringBuilder sb = new StringBuilder();
    //            for (int i = 0; i < values.Count(); i++)
    //            {
    //                string sValue = values[i] ? "1" : "0";
    //                string keyValue = $"Bit{i:X}:{sValue}";
    //                sb.Append(keyValue);
    //                if (i != values.Count() - 1)
    //                    sb.Append(" ");
    //            }
    //            return sb.ToString();
    //        }
    //        else
    //        {
    //            return string.Empty;
    //        }
    //    }

    //}

    //public class JsTimeConverter : Newtonsoft.Json.JsonConverter
    //{
    //    public override bool CanConvert(Type objectType)
    //    {
    //        return objectType == typeof(DateTime);
    //    }

    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        //if (reader.TokenType == JsonToken.None) return null;
    //        //var time = (long)serializer.Deserialize(reader, typeof(long));
    //        return null;
    //    }

    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        var item = (DateTime)value;
    //        writer.WriteValue(ConvertToJsTime(item));
    //        writer.Flush();
    //    }

    //    public string ConvertToJsTime(DateTime value)
    //    {
    //        return value.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture); ;
    //    }

    //}

    //public class JsBooleanConverter : Newtonsoft.Json.JsonConverter
    //{
    //    public override bool CanConvert(Type objectType)
    //    {
    //        return objectType == typeof(Boolean);
    //    }

    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        //if (reader.TokenType == JsonToken.None) return null;
    //        //var time = (long)serializer.Deserialize(reader, typeof(long));
    //        return null;
    //    }

    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        var item = (Boolean)value;
    //        writer.WriteValue(ConvertToJsTime(item));
    //        writer.Flush();
    //    }

    //    public int ConvertToJsTime(Boolean value)
    //    {
    //        return value ? 1 : 0;
    //    }

    //}

}
