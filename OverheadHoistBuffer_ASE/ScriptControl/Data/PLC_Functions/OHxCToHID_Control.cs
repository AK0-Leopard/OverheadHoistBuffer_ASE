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
    class OHxCToHID_Control : PLC_FunBase
    {

        public DateTime Timestamp;

        [PLCElement(ValueName = "OHXC_TO_HID_CONTROL")]
        public Boolean Control;

        public override string ToString()
        {
            string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(this, JsHelper.jsBooleanArrayConverter, JsHelper.jsTimeConverter);
            sJson = sJson.Replace(nameof(Timestamp), "@timestamp");
            return sJson;
        }
    }
}
