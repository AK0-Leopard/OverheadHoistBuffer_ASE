using com.mirle.ibg3k0.sc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    public class LogObj
    {
        public DateTime dateTime { get; set; }
        public string LogLevel { get; set; }
        /// <summary>
        /// 用來存放此筆紀錄的Log，是由哪一個DLL產生
        /// </summary>
        public string Process { get; set; }
        /// <summary>
        /// 用來存放此筆紀錄的Log，是由哪一個Service產生
        /// </summary>
        public string Class { get; set; }
        public string Method { get; set; }
        public string Device{ get; set; }
        public string LogID{ get; set; }
        public string ThreadID{ get; set; }
        public string Data{ get; set; }
        public string VH_ID{ get; set; }
        public string CarrierID{ get; set; }

        public LogConstants.Type? Type{ get; set; }
        public string Lot{ get; set; }
        public string Level{ get; set; }
        public string XID{ get; set; }
        public UInt64 Sequence{ get; set; }
        public string TransactionID{ get; set; }
        public string Details{ get; set; }
        public string Index{ get; set; }

        public void reset()
        {
            dateTime = DateTime.MinValue;
            LogLevel = null;
            Process = null;
            Class = null;
            Method = null;
            Device = null;
            LogID = null;
            ThreadID = null;
            Data = null;
            Lot = null;
            Level = null;
            XID = null;
            Sequence = 0;
            TransactionID = null;
            Details = null;
        }

        public override string ToString()
        {
            string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(this, JsHelper.jsBooleanConverter, JsHelper.jsTimeConverter, JsHelper.jsLogTypeConverter);
            sJson = sJson.Replace(nameof(dateTime), "@timestamp");
            return sJson;
        }

    }
}
