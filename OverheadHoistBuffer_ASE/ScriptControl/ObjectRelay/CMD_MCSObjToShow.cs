using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.ObjectRelay
{
    public class CMD_MCSObjToShow
    {
        public static App.SCApplication app = App.SCApplication.getInstance();
        public ACMD_MCS cmd_mcs { private set; get; } = null;
        private BLL.VehicleBLL VehicleBLL = null;
        public CMD_MCSObjToShow(BLL.VehicleBLL vehicleBLL, ACMD_MCS cmdMcs)
        {
            cmd_mcs = cmdMcs;
            VehicleBLL = vehicleBLL;
        }
        public string CMD_ID { get { return cmd_mcs.CMD_ID; } }
        public string CARRIER_ID { get { return cmd_mcs.CARRIER_ID; } }
        public string VEHICLE_ID
        {
            get
            {
                if (VehicleBLL != null)
                {
                    var vh = VehicleBLL.cache.getVehicleByMCSCmdID(CMD_ID);
                    return vh == null ? "" : vh.VEHICLE_ID;
                }
                else
                    return "";
            }
        }

        public E_TRAN_STATUS TRANSFERSTATE { get { return cmd_mcs.TRANSFERSTATE; } }
        public string HOSTSOURCE
        {
            get
            {
                var portstation = app.PortStationBLL.OperateCatch.getPortStation(cmd_mcs.HOSTSOURCE);
                return portstation == null ? cmd_mcs.HOSTSOURCE : portstation.ToString();
            }
        }
        public string HOSTDESTINATION
        {
            get
            {
                var portstation = app.PortStationBLL.OperateCatch.getPortStation(cmd_mcs.HOSTDESTINATION);
                return portstation == null ? cmd_mcs.HOSTDESTINATION : portstation.ToString();
            }
        }

        public int PRIORITY { get { return cmd_mcs.PRIORITY; } }
        public System.DateTime CMD_INSER_TIME { get { return cmd_mcs.CMD_INSER_TIME; } }
        public Nullable<System.DateTime> CMD_START_TIME { get { return cmd_mcs.CMD_START_TIME; } }
        public Nullable<System.DateTime> CMD_FINISH_TIME { get { return cmd_mcs.CMD_FINISH_TIME; } }
        public int REPLACE { get { return cmd_mcs.REPLACE; } }

    }
    //public class HCMD_MCSObjToShow
    //{
    //    public static App.SCApplication app = App.SCApplication.getInstance();
    //    public HCMD_MCS cmd_mcs = null;
    //    public HCMD_MCSObjToShow()
    //    {
    //    }
    //    public string CMD_ID { get { return cmd_mcs.CMD_ID; } }
    //    public string CARRIER_ID { get { return cmd_mcs.CARRIER_ID; } }
    //    public E_TRAN_STATUS TRANSFERSTATE { get { return cmd_mcs.TRANSFERSTATE; } }
    //    public string HOSTSOURCE
    //    {
    //        get
    //        {
    //            var portstation = app.PortStationBLL.OperateCatch.getPortStation(cmd_mcs.HOSTSOURCE);
    //            return portstation == null ? cmd_mcs.HOSTSOURCE : portstation.ToString();
    //        }
    //    }
    //    public string HOSTDESTINATION
    //    {
    //        get
    //        {
    //            var portstation = app.PortStationBLL.OperateCatch.getPortStation(cmd_mcs.HOSTDESTINATION);
    //            return portstation == null ? cmd_mcs.HOSTDESTINATION : portstation.ToString();
    //        }
    //    }

    //    public int PRIORITY { get { return cmd_mcs.PRIORITY; } }
    //    public System.DateTime CMD_INSER_TIME { get { return cmd_mcs.CMD_INSER_TIME; } }
    //    public Nullable<System.DateTime> CMD_START_TIME { get { return cmd_mcs.CMD_START_TIME; } }
    //    public Nullable<System.DateTime> CMD_FINISH_TIME { get { return cmd_mcs.CMD_FINISH_TIME; } }
    //    public int REPLACE { get { return cmd_mcs.REPLACE; } }

    //}
}
