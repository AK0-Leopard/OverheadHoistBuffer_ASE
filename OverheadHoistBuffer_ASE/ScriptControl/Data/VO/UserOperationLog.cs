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
		[Nest.Date(Name = "@timestamp")]
        public DateTime ActionTime { get; set; }

		[Nest.Date(Name = "UserID")]
		public string UserID { get; set; } = string.Empty;

		[Nest.Date(Name = "Action")]
		public string Action { get; set; } = string.Empty;

		[Nest.Date(Name = "FunctionID")]
		public string FunctionID { get; set; } = string.Empty;

		[Nest.Date(Name = "FunctionName")]
		public string FunctionName { get; set; } = string.Empty;

		[Nest.Date(Name = "CommandID")]
		public string CommandID { get; set; } = string.Empty;

		[Nest.Date(Name = "Source")]
		public string Source { get; set; } = string.Empty;

		[Nest.Date(Name = "Dest")]
		public string Dest { get; set; } = string.Empty;

		[Nest.Date(Name = "CarrierLoc")]
		public string CarrierLoc { get; set; } = string.Empty;

		[Nest.Date(Name = "BOXID")]
		public string BOXID { get; set; } = string.Empty;

		[Nest.Date(Name = "Operation")]
		public string Operation { get; set; } = string.Empty;

		[Nest.Date(Name = "NewCarrierID")]
		public string NewCarrierID { get; set; } = string.Empty;

		[Nest.Date(Name = "OldCarrierID")]
		public string OldCarrierID { get; set; } = string.Empty;

		[Nest.Date(Name = "OperateData")]
		public string OperateData { get; set; } = string.Empty;

		[Nest.Date(Name = "DataChange")]
		public string DataChange { get; set; } = string.Empty;

		[Nest.Date(Name = "Remark")]
		public string Remark { get; set; } = string.Empty;

		[Nest.Date(Name = "Index")]
		public string Index { get; set; } = "OperationInfo";
    }
}
