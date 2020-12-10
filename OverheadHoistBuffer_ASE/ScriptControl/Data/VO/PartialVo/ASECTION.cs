using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using com.mirle.ibg3k0.sc.Common;
using NLog;

namespace com.mirle.ibg3k0.sc
{
    public partial class ASECTION
    {
        public event EventHandler<string> VehicleLeave;
        public event EventHandler<string> VehicleEntry;

        private void onSectinoLeave(string vh_id)
        {
            VehicleLeave?.Invoke(this, vh_id);
        }
        private void onSectinoEntry(string vh_id)
        {
            VehicleEntry?.Invoke(this, vh_id);
        }

        public void Leave(string vh_id)
        {
            onSectinoLeave(vh_id);
        }
        public void Entry(string vh_id)
        {
            onSectinoEntry(vh_id);
        }
        public string GetOrtherEndPoint(string endPoint)
        {
            if (Common.SCUtility.isMatche(this.FROM_ADR_ID, endPoint))
            {
                return this.TO_ADR_ID;
            }
            else if (Common.SCUtility.isMatche(this.TO_ADR_ID, endPoint))
            {
                return this.FROM_ADR_ID;
            }
            else
            {
                LogHelper.Log(logger: LogManager.GetCurrentClassLogger(), LogLevel: LogLevel.Warn, Class: nameof(ASECTION), Device: "OHBC",
                   Data: $"in section id:{SEC_ID}, unknow endpoint:{endPoint}");
                return "";
            }
        }
        public bool IsActive(BLL.SegmentBLL segmentBLL)
        {
            return segmentBLL.cache.GetSegment(this.SEG_NUM).STATUS == E_SEG_STATUS.Active;
        }

    }
}
