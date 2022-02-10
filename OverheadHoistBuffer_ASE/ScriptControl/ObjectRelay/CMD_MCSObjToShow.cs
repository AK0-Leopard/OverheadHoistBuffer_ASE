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
                List<ACMD_OHTC> cmd_ohtc = ACMD_OHTC.ACMD_OHTC_List.ToList();
                if (cmd_ohtc == null || cmd_ohtc.Count == 0)
                {
                    return "";
                }
                var cms_ohtc = cmd_ohtc.Where(cmd => sc.Common.SCUtility.isMatche(cmd.CMD_ID_MCS, CMD_ID)).FirstOrDefault();
                if (cms_ohtc == null)
                {
                    return "";
                }
                if (cms_ohtc.CMD_STAUS == E_CMD_STATUS.Queue)
                {
                    return $"{Common.SCUtility.Trim(cms_ohtc.VH_ID)}(P)";
                }
                return Common.SCUtility.Trim(cms_ohtc.VH_ID);

                //    if (VehicleBLL != null)
                //{
                //    var vh = VehicleBLL.cache.getVehicleByMCSCmdID(CMD_ID);
                //    return vh == null ? "" : vh.VEHICLE_ID;
                //}
                //else
                //    return "";
            }
        }

        public E_TRAN_STATUS TRANSFERSTATE { get { return cmd_mcs.TRANSFERSTATE; } }
        public string HOSTSOURCE
        {
            get
            {
                var portstation = app.PortDefBLL.getPortDef(cmd_mcs.HOSTSOURCE);
                return portstation == null ? cmd_mcs.HOSTSOURCE : portstation.ToString(app.ZoneCommandBLL);
            }
        }
        public string HOSTDESTINATION
        {
            get
            {
                var portstation = app.PortDefBLL.getPortDef(cmd_mcs.HOSTDESTINATION);
                return portstation == null ? cmd_mcs.HOSTDESTINATION : portstation.ToString(app.ZoneCommandBLL);
            }
        }
        public string RelayStation { get { return cmd_mcs.RelayStation; } }

        public int PRIORITY { get { return cmd_mcs.PRIORITY; } }
        public System.DateTime CMD_INSER_TIME { get { return cmd_mcs.CMD_INSER_TIME; } }
        public Nullable<System.DateTime> CMD_START_TIME { get { return cmd_mcs.CMD_START_TIME; } }
        public Nullable<System.DateTime> CMD_FINISH_TIME { get { return cmd_mcs.CMD_FINISH_TIME; } }
        public int REPLACE { get { return cmd_mcs.REPLACE; } }

    }


    public class HCMD_MCSObjToShow
    {
        public App.SCApplication app = null;
        public HCMD_MCS cmd_mcs = null;
        public HCMD_MCSObjToShow(App.SCApplication _app, HCMD_MCS _cmdMcs)
        {
            app = _app;
            cmd_mcs = _cmdMcs;

        }
        public string CMD_ID { get { return cmd_mcs.CMD_ID; } }
        public string CARRIER_ID { get { return cmd_mcs.CARRIER_ID; } }
        public E_TRAN_STATUS TRANSFERSTATE { get { return cmd_mcs.TRANSFERSTATE; } }
        public CompleteStatus COMMANDSTATE { get { return CompleteStatus.CmpStatusOverride; } }

        public string HOSTSOURCE
        {
            get
            {
                return cmd_mcs.HOSTSOURCE;

                //var portstation = app.PortStationBLL.OperateCatch.getPortStation(cmd_mcs.HOSTSOURCE);
                //return portstation == null ? cmd_mcs.HOSTSOURCE : portstation.ToString();
            }
        }
        public string HOSTDESTINATION
        {
            get
            {
                return cmd_mcs.HOSTDESTINATION;
                //var portstation = app.PortStationBLL.OperateCatch.getPortStation(cmd_mcs.HOSTDESTINATION);
                //return portstation == null ? cmd_mcs.HOSTDESTINATION : portstation.ToString();
            }
        }
        public string RelayStation { get { return cmd_mcs.RelayStation; } }

        public int PRIORITY { get { return cmd_mcs.PRIORITY; } }
        public string CHECKCODE { get { return cmd_mcs.CHECKCODE; } }

        public System.DateTime CMD_INSER_TIME { get { return cmd_mcs.CMD_INSER_TIME; } }
        public Nullable<System.DateTime> CMD_START_TIME { get { return cmd_mcs.CMD_START_TIME; } }
        public Nullable<System.DateTime> CMD_FINISH_TIME { get { return cmd_mcs.CMD_FINISH_TIME; } }
        public int TIME_PRIORITY { get { return cmd_mcs.TIME_PRIORITY; } }
        public int PORT_PRIORITY { get { return cmd_mcs.PORT_PRIORITY; } }
        public int REPLACE { get { return cmd_mcs.REPLACE; } }
        public int PRIORITY_SUM { get { return cmd_mcs.PRIORITY_SUM; } }
        public string BOX_ID { get { return cmd_mcs.BOX_ID; } }
        public string CARRIER_LOC { get { return cmd_mcs.CARRIER_LOC; } }
        public string LOT_ID { get { return cmd_mcs.LOT_ID; } }
        public string CARRIER_ID_ON_CRANE { get { return cmd_mcs.CARRIER_ID_ON_CRANE; } }
        public string CMDTYPE { get { return cmd_mcs.CMDTYPE; } }
        public string CRANE { get { return cmd_mcs.CRANE; } }

    }
}
