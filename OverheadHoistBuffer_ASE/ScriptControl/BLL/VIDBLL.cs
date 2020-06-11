using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.DAO.EntityFramework;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class VIDBLL
    {
        VIDINFODao vidIvfoDAO = null;
        private SCApplication scApp = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public VIDBLL()
        {

        }
        public void start(SCApplication app)
        {
            scApp = app;
            vidIvfoDAO = scApp.VIDINFODao;
        }

        public bool addVIDInfo(string eq_id)
        {
            bool isSuccess = true;
            AVIDINFO vid_info = new AVIDINFO()
            {
                EQ_ID = eq_id
            };
            using (DBConnection_EF con = new DBConnection_EF())
            {
                vidIvfoDAO.add(con, vid_info);
            }
            return isSuccess;
        }

        public AVIDINFO getVIDInfo(string eq_id)
        {
            AVIDINFO vid_info = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                vid_info = vidIvfoDAO.getByID(con, eq_id);
                if (vid_info != null)
                    con.Entry(vid_info).State = EntityState.Detached;
            }
            return vid_info;
        }
        public AVIDINFO getVIDInfoByMCSCmdID(string cmdID)
        {
            AVIDINFO vid_info = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                vid_info = vidIvfoDAO.getByMCSCmdID(con, cmdID);
                if (vid_info != null)
                    con.Entry(vid_info).State = EntityState.Detached;
            }
            return vid_info;
        }
        public bool upDateVIDCarrierID(string vhID, string carrierID)
        {
            bool isSuccess = true;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                AVIDINFO vid_info = vidIvfoDAO.getByID(con, vhID);
                if (vid_info != null)
                {
                    vid_info.CARRIER_ID = carrierID;
                    vidIvfoDAO.update(con);
                }
                else
                {
                    //TODO Exception log
                }
            }
            return isSuccess;
        }
        public bool upDateVIDCommandInfo(string eq_id, string mcs_cmd_id)
        {
            bool isSuccess = true;
            if (SCUtility.isEmpty(mcs_cmd_id))
            {
                return false;
            }
            ACMD_MCS mcs_msc = scApp.CMDBLL.getCMD_MCSByID(mcs_cmd_id);
            if (mcs_msc != null)
            {
                isSuccess = upDateVIDCommandInfo(eq_id, mcs_msc.CARRIER_ID, mcs_msc.CMD_ID, mcs_msc.HOSTSOURCE, mcs_msc.HOSTDESTINATION, mcs_msc.PRIORITY, mcs_msc.REPLACE);
            }

            return isSuccess;
        }
        private bool upDateVIDCommandInfo(string eq_id, string carrier_id, string cmd_id, string source_port, string dest_port, int priority, int replace)
        {
            bool isSuccess = true;
            //using (DBConnection_EF con = new DBConnection_EF())
            using (DBConnection_EF con = DBConnection_EF.GetContext())
            {
                AVIDINFO vid_info = vidIvfoDAO.getByID(con, eq_id);
                if (vid_info != null)
                {
                    vid_info.CARRIER_ID = carrier_id;
                    vid_info.COMMAND_ID = cmd_id;
                    vid_info.SOURCEPORT = source_port;
                    vid_info.DESTPORT = dest_port;
                    vid_info.PRIORITY = priority;
                    vid_info.CARRIER_LOC = source_port;
                    vid_info.REPLACE = replace;
                    //vid_info.CARRIER_LOC = dest_port;
                    vidIvfoDAO.update(con);
                }
                else
                {
                    isSuccess = false;
                    //TODO Exception log
                }
            }
            return isSuccess;
        }
        public bool initialVIDCommandInfo(string eq_id)
        {
            bool isSuccess = true;
            //using (DBConnection_EF con = new DBConnection_EF())
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                //DBConnection_EF con = DBConnection_EF.GetContext();
                //con.BeginTransaction();
                AVIDINFO vid_info = vidIvfoDAO.getByID(con, eq_id);
                if (vid_info != null)
                {
                    vid_info.CARRIER_ID = string.Empty;
                    vid_info.COMMAND_ID = string.Empty;
                    vid_info.SOURCEPORT = string.Empty;
                    vid_info.DESTPORT = string.Empty;
                    vid_info.PRIORITY = 0;
                    vid_info.CARRIER_LOC = string.Empty;
                    vid_info.REPLACE = 0;
                    vidIvfoDAO.update(con);
                }
                else
                {
                    isSuccess = false;
                    //TODO Exception log
                }
                //con.Commit();
            }
            return isSuccess;
        }


        public bool upDateVIDCarrierLocInfo(string eq_id, string carrier_location)
        {
            bool isSuccess = true;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                AVIDINFO vid_info = vidIvfoDAO.getByID(con, eq_id);
                if (vid_info != null)
                {
                    vid_info.CARRIER_LOC = carrier_location;
                    vid_info.CARRIER_INSTALLED_TIME = DateTime.Now;
                    vidIvfoDAO.update(con);
                }
                else
                {
                    isSuccess = false;
                    //TODO Exception log
                }
            }
            return isSuccess;
        }
        public bool upDateVIDAlarmInfo(string eq_id, string alarm_id, string alarm_text, string unit_id)
        {
            bool isSuccess = true;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                AVIDINFO vid_info = vidIvfoDAO.getByID(con, eq_id);
                if (vid_info != null)
                {
                    vid_info.ALARM_ID = alarm_id;
                    vid_info.ALARM_TEXT = alarm_text;
                    vid_info.UNIT_ID = unit_id;
                    vidIvfoDAO.update(con);
                }
                else
                {
                    //TODO Exception log
                }
            }
            return isSuccess;
        }
        public bool upDateVIDPortID(string eq_id, string adr_id)
        {
            bool isSuccess = true;
            //DBConnection_EF con = DBConnection_EF.GetContext();
            try
            {
                //using (DBConnection_EF con = new DBConnection_EF())
                var port_station = scApp.MapBLL.getPortByAdrID(adr_id);

                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    AVIDINFO vid_info = vidIvfoDAO.getByID(con, eq_id);
                    if (vid_info != null)
                    {
                        //con.BeginTransaction();
                        vid_info.PORT_ID = port_station == null ? string.Empty : port_station.PORT_ID;
                        // vid_info.CARRIER_LOC = adr_id;
                        vidIvfoDAO.update(con);
                        //con.Commit();
                    }
                    else
                    {
                        isSuccess = false;
                        //TODO Exception log
                    }
                }
            }
            catch (Exception ex)
            {
                //if (con != null) { try { con.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Error(ex, "Exception");
                isSuccess = false;
            }
            finally
            {
                //if (con != null) { try { con.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception"); } }
            }
            return isSuccess;
        }

        public bool upDateVIDResultCode(string eq_id, string resultCode)
        {
            bool isSuccess = true;
            try
            {
                int iresult_code = 0;
                if (!int.TryParse(resultCode, out iresult_code))
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VIDBLL), Device: "OHxC",
                        Data: $"Parse result code has problem.{nameof(resultCode)} is {resultCode}",
                       VehicleID: eq_id);
                }
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    AVIDINFO vid_info = vidIvfoDAO.getByID(con, eq_id);
                    if (vid_info != null)
                    {
                        vid_info.RESULT_CODE = iresult_code;
                        vidIvfoDAO.update(con);
                        //con.Commit();
                    }
                    else
                    {
                        isSuccess = false;
                        //TODO Exception log
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
                throw ex;
            }
            finally
            {

            }
            return isSuccess;
        }


        public bool tryGetVIOInfoVIDCollectionByEQID(string eq_id, out VIDCollection vid_collection)
        {
            //DBConnection_EF con = DBConnection_EF.GetContext();

            AVIDINFO vid_info = null;
            //using (DBConnection_EF con = new DBConnection_EF())
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                vid_info = vidIvfoDAO.getByID(con, eq_id);
                con.Entry(vid_info).State = EntityState.Detached;
            }
            if (vid_info != null)
            {
                vid_collection = AVIDINFO2VIDCollection(vid_info);
            }
            else
            {
                vid_collection = null;
                //TODO Exception log
            }

            return vid_collection != null;
        }

        private VIDCollection AVIDINFO2VIDCollection(AVIDINFO vid_info)
        {
            if (vid_info == null)
                return null;
            string carrier_loc = string.Empty;
            string port_id = string.Empty;
            //scApp.MapBLL.getPortID(vid_info.CARRIER_LOC, out carrier_loc);
            //scApp.MapBLL.getPortID(vid_info.PORT_ID, out port_id);

            VIDCollection vid_collection = new VIDCollection();


            vid_collection.VID_06_ControlState.CONTROLSTATE = "5"; //todo 

            List<AVEHICLE> vhs = scApp.getEQObjCacheManager().getAllVehicle();

            vid_collection.VID_10_EnhancedCarrierInfo.CARRIER_ID_OBJ.CARRIER_ID = vid_info.CARRIER_ID;
            vid_collection.VID_10_EnhancedCarrierInfo.CARRIER_LOC_OBJ.CARRIER_LOC = vid_info.CARRIER_LOC;
            vid_collection.VID_10_EnhancedCarrierInfo.CARRIER_ZONE_NAME = String.Empty; //todo fill in info

            vid_collection.VID_11_CommandInfo.COMMAND_ID.COMMAND_ID = vid_info.COMMAND_ID;
            vid_collection.VID_11_CommandInfo.PRIORITY.PRIORITY = vid_info.PRIORITY.ToString();

            vid_collection.VID_13_EnhancedTransferCmd.TRANSFER_STATE.TRANSFER_STATE = string.Empty; //todo fill in info
            vid_collection.VID_13_EnhancedTransferCmd.COMMAND_INFO.COMMAND_ID.COMMAND_ID = vid_info.COMMAND_ID;
            vid_collection.VID_13_EnhancedTransferCmd.COMMAND_INFO.PRIORITY.PRIORITY = vid_info.PRIORITY.ToString();

            int vhs_count = vhs.Count;
            S6F11.RPTINFO.RPTITEM.VIDITEM_71[] VEHICLEINFOs = new S6F11.RPTINFO.RPTITEM.VIDITEM_71[vhs_count];
            for (int j = 0; j < vhs_count; j++)
            {
                VEHICLEINFOs[j] = new S6F11.RPTINFO.RPTITEM.VIDITEM_71();
                VEHICLEINFOs[j].VHINFO.VEHICLE_ID = vhs[j].VEHICLE_ID;
                VEHICLEINFOs[j].VHINFO.VEHICLE_STATE = string.Empty; //todo fill in info
            }
            vid_collection.VID_53_ActiveVehicles.VEHICLEINFO = VEHICLEINFOs;

            vid_collection.VID_54_CarrierID.CARRIER_ID = vid_info.CARRIER_ID;

            vid_collection.VID_56_CarrierLoc.CARRIER_LOC = vid_info.CARRIER_LOC;

            vid_collection.VID_58_CommandID.COMMAND_ID = vid_info.COMMAND_ID;

            vid_collection.VID_59_CommandInfo.COMMAND_ID.COMMAND_ID = vid_info.COMMAND_ID;
            vid_collection.VID_59_CommandInfo.PRIORITY.PRIORITY = vid_info.PRIORITY.ToString();

            vid_collection.VID_60_DestinationPort.DESTINATION_PORT = vid_info.DESTPORT;

            vid_collection.VID_62_Priotity.PRIORITY = vid_info.PRIORITY.ToString();

            vid_collection.VID_64_ResultCode.RESULT_CODE = vid_info.RESULT_CODE.ToString();

            vid_collection.VID_65_SourcePort.SOURCE_PORT = vid_info.SOURCEPORT;

            vid_collection.VID_66_HandoffType.HANDOFF_TYPE = string.Empty;//todo fill in info

            vid_collection.VID_70_VehicleID.VEHILCE_ID = vid_info.EQ_ID;

            vid_collection.VID_71_VehicleInfo.VHINFO.VEHICLE_ID = vid_info.EQ_ID;
            vid_collection.VID_71_VehicleInfo.VHINFO.VEHICLE_STATE = vid_info.VEHICLE_STATE.ToString();

            vid_collection.VID_72_VehicleStatus.VEHICLE_STATE = vid_info.VEHICLE_STATE.ToString();

            vid_collection.VID_77_TranCmpInfo.TRANCOMPLETEINFO[0].TRANINFO.CARRIER_ID = vid_info.CARRIER_ID;
            vid_collection.VID_77_TranCmpInfo.TRANCOMPLETEINFO[0].TRANINFO.SOURCE_PORT = vid_info.SOURCEPORT;
            vid_collection.VID_77_TranCmpInfo.TRANCOMPLETEINFO[0].TRANINFO.DESTINATION_PORT = vid_info.DESTPORT;
            vid_collection.VID_77_TranCmpInfo.TRANCOMPLETEINFO[0].CARRIER_LOC = vid_info.CARRIER_LOC;

            vid_collection.VID_80_CommmandType.COMMAND_TYPE = vid_info.COMMAND_TYPE;
            vid_collection.VID_81_AlarmID.ALARM_ID = vid_info.ALARM_ID;
            vid_collection.VID_82_AlarmText.ALARM_TEXT = vid_info.ALARM_TEXT;
            vid_collection.VID_83_UnitID.UNIT_ID = vid_info.UNIT_ID;
            vid_collection.VID_84_TransferInfo.CARRIER_ID = vid_info.CARRIER_ID;
            vid_collection.VID_84_TransferInfo.SOURCE_PORT = vid_info.SOURCEPORT;
            vid_collection.VID_84_TransferInfo.DESTINATION_PORT = vid_info.DESTPORT;

            vid_collection.VID_114_SpecVersion.SPEC_VERSION = string.Empty;//todo fill in info

            vid_collection.VID_115_PortID.PORT_ID = vid_info.PORT_ID;

            vid_collection.VID_353_EqPresenceStatus.EQ_PRESENCE_STATUS = string.Empty;//todo fill in info

            //vid_collection.VID_354_PortInfo.PORT_ID.PORT_ID = string.Empty;//todo fill in info
            //vid_collection.VID_354_PortInfo.PORT_TRANSFTER_STATE.PORT_TRANSFER_STATE = string.Empty;//todo fill in info

            //vid_collection.VID_355_PortTransferState.PORT_TRANSFER_STATE = string.Empty;//todo fill in info

            vid_collection.VID_361_UnitAlarmInfo.UNIT_ID.UNIT_ID = vid_info.UNIT_ID;
            vid_collection.VID_361_UnitAlarmInfo.ALARM_ID.ALARM_ID = vid_info.ALARM_ID;
            vid_collection.VID_361_UnitAlarmInfo.ALARM_TEXT.ALARM_TEXT = vid_info.ALARM_TEXT;
            vid_collection.VID_361_UnitAlarmInfo.MAINT_STATE.MAINT_STATE = string.Empty;//todo fill in info

            vid_collection.VID_362_MainState.MAINT_STATE = string.Empty;//todo fill in info

            vid_collection.VID_363_VehicleCurrenyPosition.VEHICLE_CURRENT_POSITION = string.Empty;//todo fill in info

            vid_collection.VID_722_TransferState.TRANSFER_STATE = string.Empty;//todo fill in info

            return vid_collection;
        }
    }
}
