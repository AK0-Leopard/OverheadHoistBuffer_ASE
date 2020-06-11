using com.mirle.ibg3k0.sc.Data.VO.Interface;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    public class DeviceConnectionInfo : IConnectionStatusChange
    {
        public string Name;
        public DeviceConnectionType Type;
        private ConnectionStatus status;
        public ConnectionStatus Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    ConnectionStatusChange?.Invoke(this, status == ConnectionStatus.Success);
                }
            }
        }

        public event EventHandler<bool> ConnectionStatusChange;
    }
}
