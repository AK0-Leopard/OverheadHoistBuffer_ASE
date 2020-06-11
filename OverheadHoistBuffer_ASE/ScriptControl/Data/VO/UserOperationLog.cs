using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    public class UserOperationLog
    {
        [JsonProperty(propertyName: "@timestamp")]
        public DateTime ActionTime { get; set; }
        public string UserID { get; set; }
        public string Action { get; set; }
        public string FunctionID { get; set; }
        public string FunctionName { get; set; }
        public string CommandID { get; set; }
        public string Source { get; set; }
        public string Dest { get; set; }
        public string CarrierLoc { get; set; }
        public string BOXID { get; set; }
        public string Operation { get; set; }
        public string NewCarrierID { get; set; }
        public string OldCarrierID { get; set; }
        public string OperateData { get; set; }
        public string DataChange { get; set; }
        public string Remark { get; set; }
    }
}
