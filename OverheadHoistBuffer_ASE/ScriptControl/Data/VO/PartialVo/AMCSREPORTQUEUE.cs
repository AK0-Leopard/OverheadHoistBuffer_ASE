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

namespace com.mirle.ibg3k0.sc
{
    public partial class AMCSREPORTQUEUE
    {

        public AMCSREPORTQUEUE()
        {
            ID = Guid.NewGuid();
        }


        public override string ToString()
        {
            dynamic logEntry = new Newtonsoft.Json.Linq.JObject(this);
            logEntry.RPT_TIME = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture);
            var json = logEntry.ToString(Newtonsoft.Json.Formatting.None);
            json = json.Replace("RPT_TIME", "@timestamp");
            return json.ToString();
        }
    }

}
