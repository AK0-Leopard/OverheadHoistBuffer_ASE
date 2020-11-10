//*********************************************************************************
//      MESDefaultMapAction.cs
//*********************************************************************************
// File Name: MESDefaultMapAction.cs
// Description: 與EAP通訊的劇本
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2019/11/15    Kevin Wei      N/A            A0.01   修改對於OHT 上報位置的更新，改成僅透過143、132、134的事件，
//                                                     其中143僅有在1.剛連線時、2.命令發送失敗時、3.手動同步時，才會進行更新
// 2020/05/04    Jason Wu       N/A            A0.02   新增BoxID更新，但最後一部寫入資料庫部分尚未開啟，因為這部分OHT部分之回報要先有修正後才能開啟
// 2020/05/27    Jason Wu       N/A            A0.03   修改filterVh部分，使其使用OHTC CMD去判斷是否有命令執行。
//**********************************************************************************
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.DAO;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using com.mirle.iibg3k0.ttc.Common;
using NLog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class VehicleBLL
    {
        VehicleDao vehicleDAO = null;
        private SCApplication scApp = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public Cache cache { get; private set; }
        public Web web { get; private set; }


        public VehicleBLL()
        {

        }
        public void start(SCApplication app)
        {
            scApp = app;
            vehicleDAO = scApp.VehicleDao;
            cache = new Cache(scApp.getEQObjCacheManager());
            web = new Web(scApp.webClientManager);

        }
        public void startMapAction()
        {

            List<AVEHICLE> lstVH = scApp.getEQObjCacheManager().getAllVehicle();
            //foreach (AVEHICLE vh in lstVH)
            //{
            //    if (!vh_update_lock_obj_pool.ContainsKey(vh.VEHICLE_ID))
            //    {
            //        vh_update_lock_obj_pool.Add(vh.VEHICLE_ID, vh.VEHICLE_ID);
            //    }
            //}
        }
        public bool addVehicle(AVEHICLE _vh)
        {
            bool isSuccess = true;

            using (DBConnection_EF con = new DBConnection_EF())
            {
                AVEHICLE vh = new AVEHICLE
                {
                    VEHICLE_ID = _vh.VEHICLE_ID,
                    ACC_SEC_DIST = 0,
                    MODE_STATUS = 0,
                    ACT_STATUS = 0,
                    OHTC_CMD = string.Empty,
                    BLOCK_PAUSE = 0,
                    CMD_PAUSE = 0,
                    OBS_PAUSE = 0,
                    HAS_CST = 0,
                    VEHICLE_ACC_DIST = 0,
                    MANT_ACC_DIST = 0,
                    GRIP_COUNT = 0,
                    GRIP_MANT_COUNT = 0
                };
                vehicleDAO.add(con, vh);
            }
            return isSuccess;
        }
        public bool addVehicle(string vh_id)
        {
            bool isSuccess = true;

            using (DBConnection_EF con = new DBConnection_EF())
            {
                AVEHICLE vh = new AVEHICLE
                {
                    VEHICLE_ID = vh_id
                };
                vehicleDAO.add(con, vh);
            }
            return isSuccess;
        }

        //public void doUpdateVheiclePosition(AVEHICLE vh, string current_adr_id, string current_sec_id, string last_adr_id, string last_sec_id, int sec_dis, EventType vhPassEvent)
        //{
        //    //if (updateVheiclePosition(vh.VEHICLE_ID, current_adr_id, current_sec_id, sec_dis, vhPassEvent))
        //    //{
        //    //    updateVheiclePosition_CacheManager(vh, current_adr_id, current_sec_id, sec_dis, vhPassEvent);
        //    //    return true;
        //    //}
        //    //else
        //    //{
        //    //    return false;
        //    //}
        //    updateVheiclePosition_CacheManager(vh, current_adr_id, current_sec_id, sec_dis, vhPassEvent);
        //    if (!SCUtility.isMatche(current_adr_id, last_adr_id) || !SCUtility.isMatche(current_sec_id, last_sec_id))
        //        updateVheiclePosition(vh.VEHICLE_ID, current_adr_id, current_sec_id, sec_dis, vhPassEvent);

        //}
        public bool updateVheiclePosition_CacheManager(AVEHICLE vh, string adr_id, string sec_id, string seg_id, double sce_dis,
                                                       double xAxis, double yAxis)
        {
            vh.CUR_ADR_ID = adr_id;
            vh.CUR_SEC_ID = sec_id;
            vh.CUR_SEG_ID = seg_id;
            vh.ACC_SEC_DIST = sce_dis;
            vh.X_Axis = xAxis;
            vh.Y_Axis = yAxis;

            //var showObj = scApp.getEQObjCacheManager().CommonInfo.ObjectToShow_list.
            //    Where(o => o.VEHICLE_ID == vh.VEHICLE_ID).SingleOrDefault();
            //showObj.NotifyPropertyChanged(nameof(showObj.ACC_SEC_DIST2Show));
            vh.NotifyVhPositionChange();
            return true;
        }
        public Mirle.Hlts.Utils.HltResult updateVheiclePositionToReserveControlModule(BLL.ReserveBLL reserveBLL, AVEHICLE vh, string currentSectionID, double x_axis, double y_axis, double dirctionAngle, double vehicleAngle, double speed,
                                                                              Mirle.Hlts.Utils.HltDirection sensorDir, Mirle.Hlts.Utils.HltDirection forkDir)
        {
            string vh_id = vh.VEHICLE_ID;
            string section_id = currentSectionID;
            return reserveBLL.TryAddVehicleOrUpdate(vh_id, section_id, x_axis, y_axis, (float)vehicleAngle, speed, sensorDir, forkDir);
        }

        public void updateVehicleActionStatus(AVEHICLE vh, EventType vhPassEvent)
        {
            vh.VhRecentTranEvent = vhPassEvent;
            vh.NotifyVhStatusChange();
        }
        public void updateVehicleBCRReadResult(AVEHICLE vh, BCRReadResult bcrReadResult)
        {
            vh.BCRReadResult = bcrReadResult;
        }
        //public bool updateVheiclePosition(string vh_id, string adr_id, string sec_id, int sce_dis, EventType vhPassEvent)
        public void updateVheiclePosition(string vh_id, string adr_id, string sec_id, double sce_dis, EventType vhPassEvent)
        {
            //SCUtility.LockWithTimeout(vh_update_lock_obj_pool[vh_id], SCAppConstants.LOCK_TIMEOUT_MS, () =>
            //{
            AVEHICLE vh = scApp.VehiclPool.GetObject();
            //DBConnection_EF conn = null;
            try
            {
                //conn = DBConnection_EF.GetContext();
                //conn.BeginTransaction();
                //using (DBConnection_EF con = new DBConnection_EF())
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //vh = vehicleDAO.getByID(con, vh_id);
                    vh.VEHICLE_ID = vh_id;
                    con.AVEHICLE.Attach(vh);
                    //con.Entry(vh).State = EntityState.Modified;
                    vh.CUR_ADR_ID = adr_id;
                    vh.CUR_SEC_ID = sec_id;
                    vh.ACC_SEC_DIST = sce_dis;
                    con.Entry(vh).Property(p => p.CUR_ADR_ID).IsModified = true;
                    con.Entry(vh).Property(p => p.CUR_SEC_ID).IsModified = true;
                    con.Entry(vh).Property(p => p.ACC_SEC_DIST).IsModified = true;
                    //vh.LAST_REPORT_EVENT = vhPassEvent;
                    //vehicleDAO.update(con, vh);
                    vehicleDAO.doUpdate(scApp, con, vh);
                    //conn.Commit();
                    con.Entry(vh).State = EntityState.Detached;
                }
                //return true;
            }
            catch (Exception ex)
            {
                //if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Error(ex, "Exception");
                throw new Exception($"updateVheiclePosition,vh_id:{vh_id},adr:{adr_id},sec:{sec_id}", ex);
                //return false;
            }
            finally
            {
                scApp.VehiclPool.PutObject(vh);
                //if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception"); } }
            }
            //});
        }

        public void updateVhTravelInfo(AVEHICLE eqpt, string current_adr_id)
        {
            AADDRESS crt_Adr_obj = scApp.MapBLL.getAddressByID(current_adr_id);
            if (crt_Adr_obj != null)
            {
                switch (crt_Adr_obj.ADRTYPE)
                {
                    case E_ADR_TYPE.Address:
                    case E_ADR_TYPE.Port:
                        scApp.VehicleBLL.updateVheicleTravelInfo(eqpt.VEHICLE_ID, current_adr_id);
                        break;
                }
            }
        }
        public void updateVheicleTravelInfo(string vh_id, string node_adr)
        {
            //lock (update_lock_obj)
            //{
            //SCUtility.LockWithTimeout(vh_update_lock_obj_pool[vh_id], SCAppConstants.LOCK_TIMEOUT_MS, () =>
            //{
            AVEHICLE vh = scApp.VehiclPool.GetObject();
            string preNodeAdr = string.Empty;
            try
            {
                //DBConnection_EF con = DBConnection_EF.GetContext();
                //using (DBConnection_EF con = new DBConnection_EF())
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //vh = vehicleDAO.getByID(con, vh_id);
                    vh.VEHICLE_ID = vh_id;
                    con.AVEHICLE.Attach(vh);
                    preNodeAdr = vh.NODE_ADR;
                    //ASECTION sce = scApp.SectionDao.getByFromToAdr(con, preNodeAdr, node_adr);
                    ASECTION sce = scApp.SectionDao.getByFromToAdr(preNodeAdr, node_adr);
                    if (sce != null)
                    {
                        double secstion_dist = 0;
                        secstion_dist = sce.SEC_DIS;
                        vh.VEHICLE_ACC_DIST += (int)secstion_dist;
                        vh.MANT_ACC_DIST += (int)secstion_dist;
                        con.Entry(vh).Property(p => p.VEHICLE_ACC_DIST).IsModified = true;
                        con.Entry(vh).Property(p => p.MANT_ACC_DIST).IsModified = true;
                    }
                    vh.NODE_ADR = node_adr;
                    con.Entry(vh).Property(p => p.NODE_ADR).IsModified = true;
                    //bool isDetached = con.Entry(vh).State == EntityState.Modified;
                    //if (isDetached)
                    //vehicleDAO.update(con, vh);
                    vehicleDAO.doUpdate(scApp, con, vh);
                    con.Entry(vh).State = EntityState.Detached;
                }
                //return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                //return false;
            }
            finally
            {
                scApp.VehiclPool.PutObject(vh);
            }
            //});
            //}
        }
        //public bool doUpdateVehicleStatus(AVEHICLE vh,
        //                          VHModeStatus mode_status, VHActionStatus act_status,
        //                          VhStopSingle block_pause, VhStopSingle cmd_pause, VhStopSingle obs_pause,
        //                          int has_cst)
        //{
        //    if (updateVehicleStatus(vh.VEHICLE_ID,
        //                             mode_status, act_status,
        //                             block_pause, cmd_pause, obs_pause,
        //                             has_cst))
        //    {
        //        //updateVehicleStatus_CacheMangerExceptAct(vh,
        //        //                       mode_status,
        //        //                       block_pause, cmd_pause, obs_pause,
        //        //                       has_cst, cst_id);
        //        vh.NotifyVhStatusChange();
        //        return true;
        //    }
        //    return false;
        //}
        public bool doUpdateVehicleStatus(AVEHICLE vh, string cstID,
                                 VHModeStatus mode_status, VHActionStatus act_status,
                                 VhStopSingle block_pause, VhStopSingle cmd_pause, VhStopSingle obs_pause, VhStopSingle hid_pause, VhStopSingle error_status, VhLoadCarrierStatus load_cst_status)
        {
            if (updateVehicleStatus(vh.VEHICLE_ID, cstID, vh.BOX_ID,    //A0.02
                                     mode_status, act_status,
                                     block_pause, cmd_pause, obs_pause, hid_pause, error_status, load_cst_status))
            {
                //updateVehicleStatus_CacheMangerExceptAct(vh,
                //                       mode_status,
                //                       block_pause, cmd_pause, obs_pause,
                //                       has_cst, cst_id);
                vh.NotifyVhStatusChange();
                return true;
            }
            return false;
        }
        private bool updateVehicleStatus(string vh_id, string cstID, string boxID,      //A0.02
                          VHModeStatus mode_status, VHActionStatus act_status,
                          VhStopSingle block_pause, VhStopSingle cmd_pause, VhStopSingle obs_pause, VhStopSingle hid_pause, VhStopSingle error_status, VhLoadCarrierStatus load_cst_status)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.VehiclPool.GetObject();
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    vh.VEHICLE_ID = vh_id;
                    con.AVEHICLE.Attach(vh);
                    vh.CST_ID = cstID;
                    vh.BOX_ID = boxID;       //A0.02
                    vh.MODE_STATUS = mode_status;
                    vh.ACT_STATUS = act_status;
                    vh.BLOCK_PAUSE = block_pause;
                    vh.CMD_PAUSE = cmd_pause;
                    vh.OBS_PAUSE = obs_pause;
                    vh.HID_PAUSE = hid_pause;
                    vh.ERROR = error_status;
                    vh.HAS_CST = (int)load_cst_status;
                    con.Entry(vh).Property(p => p.CST_ID).IsModified = true;
                    con.Entry(vh).Property(p => p.BOX_ID).IsModified = true;  //A0.02
                    con.Entry(vh).Property(p => p.MODE_STATUS).IsModified = true;
                    con.Entry(vh).Property(p => p.ACT_STATUS).IsModified = true;
                    con.Entry(vh).Property(p => p.BLOCK_PAUSE).IsModified = true;
                    con.Entry(vh).Property(p => p.CMD_PAUSE).IsModified = true;
                    con.Entry(vh).Property(p => p.OBS_PAUSE).IsModified = true;
                    con.Entry(vh).Property(p => p.HID_PAUSE).IsModified = true;
                    con.Entry(vh).Property(p => p.ERROR).IsModified = true;
                    con.Entry(vh).Property(p => p.HAS_CST).IsModified = true;
                    vehicleDAO.doUpdate(scApp, con, vh);
                    con.Entry(vh).State = EntityState.Detached;
                    isSuccess = true;
                }
            }
            finally
            {
                scApp.VehiclPool.PutObject(vh);
            }
            return isSuccess;
        }

        public bool updataVehicleMode(string vh_id, VHModeStatus mode_status)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.VehiclPool.GetObject();
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    vh.VEHICLE_ID = vh_id;
                    con.AVEHICLE.Attach(vh);

                    vh.MODE_STATUS = mode_status;
                    con.Entry(vh).Property(p => p.MODE_STATUS).IsModified = true;

                    vehicleDAO.doUpdate(scApp, con, vh);
                    con.Entry(vh).State = EntityState.Detached;
                    isSuccess = true;
                }
            }
            finally
            {
                scApp.VehiclPool.PutObject(vh);
            }
            return isSuccess;
        }

        public bool updateVehiclePauseStatus(string vh_id, bool? earthquake_pause = null, bool? safyte_pause = null, bool? obstruct_pause = null)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.VehiclPool.GetObject();
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    vh.VEHICLE_ID = vh_id;
                    con.AVEHICLE.Attach(vh);

                    if (earthquake_pause.HasValue)
                    {
                        vh.EARTHQUAKE_PAUSE = earthquake_pause.Value ? VhStopSingle.StopSingleOn : VhStopSingle.StopSingleOff;
                        con.Entry(vh).Property(p => p.EARTHQUAKE_PAUSE).IsModified = true;
                    }
                    if (safyte_pause.HasValue)
                    {
                        vh.SAFETY_DOOR_PAUSE = safyte_pause.Value ? VhStopSingle.StopSingleOn : VhStopSingle.StopSingleOff;
                        con.Entry(vh).Property(p => p.SAFETY_DOOR_PAUSE).IsModified = true;
                    }
                    if (obstruct_pause.HasValue)
                    {
                        vh.OHXC_BLOCK_PAUSE = obstruct_pause.Value ? VhStopSingle.StopSingleOn : VhStopSingle.StopSingleOff;
                        con.Entry(vh).Property(p => p.OHXC_BLOCK_PAUSE).IsModified = true;
                    }
                    if (obstruct_pause.HasValue)
                    {
                        vh.OHXC_BLOCK_PAUSE = obstruct_pause.Value ? VhStopSingle.StopSingleOn : VhStopSingle.StopSingleOff;
                        con.Entry(vh).Property(p => p.OHXC_BLOCK_PAUSE).IsModified = true;
                    }

                    vehicleDAO.doUpdate(scApp, con, vh);
                    con.Entry(vh).State = EntityState.Detached;
                    isSuccess = true;
                }
            }
            finally
            {
                scApp.VehiclPool.PutObject(vh);
            }
            return isSuccess;
        }

        public bool updateVehicleExcuteCMD(string vh_id, string cmd_id, string mcs_cmd_id)
        {
            bool isSuccess = false;
            //DBConnection_EF con = DBConnection_EF.GetContext();
            //SCUtility.LockWithTimeout(vh_update_lock_obj_pool[vh_id], SCAppConstants.LOCK_TIMEOUT_MS, () =>
            //{
            AVEHICLE vh = scApp.VehiclPool.GetObject();
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //vh = vehicleDAO.getByID(con, vh_id);
                    vh.VEHICLE_ID = vh_id;
                    con.AVEHICLE.Attach(vh);
                    vh.OHTC_CMD = cmd_id;
                    vh.MCS_CMD = mcs_cmd_id;
                    //vehicleDAO.update(con, vh);
                    con.Entry(vh).Property(p => p.OHTC_CMD).IsModified = true;
                    con.Entry(vh).Property(p => p.MCS_CMD).IsModified = true;
                    vehicleDAO.doUpdate(scApp, con, vh);
                    con.Entry(vh).State = EntityState.Detached;
                    isSuccess = true;
                }
            }
            finally
            {
                scApp.VehiclPool.PutObject(vh);
            }
            //});
            return isSuccess;
        }

        public bool updataVehicleCSTID(string vh_id, string cst_id)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.VehiclPool.GetObject();
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    vh.VEHICLE_ID = vh_id;
                    con.AVEHICLE.Attach(vh);
                    vh.CST_ID = SCUtility.Trim(cst_id);
                    con.Entry(vh).Property(p => p.CST_ID).IsModified = true;
                    vehicleDAO.doUpdate(scApp, con, vh);
                    con.Entry(vh).State = EntityState.Detached;
                    isSuccess = true;
                }
            }
            finally
            {
                scApp.VehiclPool.PutObject(vh);
            }
            return isSuccess;
        }

        public bool updataVehicleBOXID(string vh_id, string box_id)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.VehiclPool.GetObject();
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    vh.VEHICLE_ID = vh_id;
                    con.AVEHICLE.Attach(vh);
                    vh.BOX_ID = SCUtility.Trim(box_id);
                    con.Entry(vh).Property(p => p.BOX_ID).IsModified = true;
                    vehicleDAO.doUpdate(scApp, con, vh);
                    con.Entry(vh).State = EntityState.Detached;
                    isSuccess = true;
                }
            }
            finally
            {
                scApp.VehiclPool.PutObject(vh);
            }
            return isSuccess;
        }

        public bool updataVehicleInstall(string vh_id)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.VehiclPool.GetObject();
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    vh.VEHICLE_ID = vh_id;
                    con.AVEHICLE.Attach(vh);
                    vh.IS_INSTALLED = true;
                    vh.INSTALLED_TIME = DateTime.Now;
                    con.Entry(vh).Property(p => p.IS_INSTALLED).IsModified = true;
                    con.Entry(vh).Property(p => p.INSTALLED_TIME).IsModified = true;
                    vehicleDAO.doUpdate(scApp, con, vh);
                    con.Entry(vh).State = EntityState.Detached;
                    isSuccess = true;
                }
            }
            finally
            {
                scApp.VehiclPool.PutObject(vh);
            }
            return isSuccess;
        }
        public bool updataVehicleRemove(string vh_id)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.VehiclPool.GetObject();
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    vh.VEHICLE_ID = vh_id;
                    con.AVEHICLE.Attach(vh);
                    vh.IS_INSTALLED = false;
                    vh.REMOVED_TIME = DateTime.Now;
                    con.Entry(vh).Property(p => p.IS_INSTALLED).IsModified = true;
                    con.Entry(vh).Property(p => p.REMOVED_TIME).IsModified = true;
                    vehicleDAO.doUpdate(scApp, con, vh);
                    con.Entry(vh).State = EntityState.Detached;
                    isSuccess = true;
                }
            }
            finally
            {
                scApp.VehiclPool.PutObject(vh);
            }
            return isSuccess;
        }




        //object update_lock_obj = new object();
        //Dictionary<string, string> vh_update_lock_obj_pool = new Dictionary<string, string>();


        public bool setVhIsParkingOnWay(string vh_id, string adr_id)
        {
            bool isSuccess = false;
            //lock (update_lock_obj)
            //{
            //SCUtility.LockWithTimeout(vh_update_lock_obj_pool[vh_id], SCAppConstants.LOCK_TIMEOUT_MS, () =>
            //{
            //DBConnection_EF con = DBConnection_EF.GetContext();
            //using (DBConnection_EF con = new DBConnection_EF())
            //vh = vehicleDAO.getByID(con, vh_id);
            AVEHICLE vh = scApp.VehiclPool.GetObject();
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    vh.VEHICLE_ID = vh_id;
                    con.AVEHICLE.Attach(vh);
                    vh.PARK_ADR_ID = adr_id;
                    vh.PARK_TIME = null;
                    vh.IS_PARKING = false;

                    vh.CYCLERUN_ID = string.Empty;
                    vh.IS_CYCLING = false;
                    vh.CYCLERUN_TIME = null;

                    con.Entry(vh).Property(p => p.PARK_ADR_ID).IsModified = true;
                    con.Entry(vh).Property(p => p.PARK_TIME).IsModified = true;
                    con.Entry(vh).Property(p => p.IS_PARKING).IsModified = true;
                    con.Entry(vh).Property(p => p.CYCLERUN_ID).IsModified = true;
                    con.Entry(vh).Property(p => p.IS_CYCLING).IsModified = true;
                    con.Entry(vh).Property(p => p.CYCLERUN_TIME).IsModified = true;

                    //vehicleDAO.update(con, vh);
                    vehicleDAO.doUpdate(scApp, con, vh);
                    con.Entry(vh).State = EntityState.Detached;
                    isSuccess = true;
                }
            }
            finally
            {
                scApp.VehiclPool.PutObject(vh);
            }
            //});
            //}
            return isSuccess;
        }
        public bool setVhIsInPark(string vh_id, string park_adr)
        {
            bool isSuccess = false;
            //lock (update_lock_obj)
            //{
            //SCUtility.LockWithTimeout(vh_update_lock_obj_pool[vh_id], SCAppConstants.LOCK_TIMEOUT_MS, () =>
            //{
            AVEHICLE vh = scApp.VehiclPool.GetObject();
            //DBConnection_EF con = DBConnection_EF.GetContext();
            //using (DBConnection_EF con = new DBConnection_EF())
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //vh = vehicleDAO.getByID(con, vh_id);
                    vh.VEHICLE_ID = vh_id;
                    con.AVEHICLE.Attach(vh);
                    vh.IS_PARKING = true;
                    vh.PARK_TIME = DateTime.Now;
                    vh.PARK_ADR_ID = park_adr;

                    vh.CYCLERUN_ID = string.Empty;
                    vh.IS_CYCLING = false;
                    vh.CYCLERUN_TIME = null;

                    con.Entry(vh).Property(p => p.IS_PARKING).IsModified = true;
                    con.Entry(vh).Property(p => p.PARK_TIME).IsModified = true;
                    con.Entry(vh).Property(p => p.PARK_ADR_ID).IsModified = true;
                    con.Entry(vh).Property(p => p.CYCLERUN_ID).IsModified = true;
                    con.Entry(vh).Property(p => p.IS_CYCLING).IsModified = true;
                    con.Entry(vh).Property(p => p.CYCLERUN_TIME).IsModified = true;

                    //vehicleDAO.update(con, vh);
                    vehicleDAO.doUpdate(scApp, con, vh);
                    con.Entry(vh).State = EntityState.Detached;
                    isSuccess = true;
                }
            }
            finally
            {
                scApp.VehiclPool.PutObject(vh);
            }
            //});
            //}
            return isSuccess;
        }
        public bool resetVhIsInPark(string vh_id)
        {
            bool isSuccess = false;
            //lock (update_lock_obj)
            //{
            //SCUtility.LockWithTimeout(vh_update_lock_obj_pool[vh_id], SCAppConstants.LOCK_TIMEOUT_MS, () =>
            //{
            AVEHICLE vh = scApp.VehiclPool.GetObject();
            //DBConnection_EF con = DBConnection_EF.GetContext();
            //using (DBConnection_EF con = new DBConnection_EF())
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetContext())
                {
                    //vh = vehicleDAO.getByID(con, vh_id);
                    vh.VEHICLE_ID = vh_id;
                    con.AVEHICLE.Attach(vh);
                    //將不再有PARK_ADR_ID 在VH TABLE
                    vh.PARK_ADR_ID = string.Empty;
                    vh.PARK_TIME = null;
                    vh.IS_PARKING = false;
                    //vehicleDAO.update(con, vh);
                    con.Entry(vh).Property(p => p.IS_PARKING).IsModified = true;
                    con.Entry(vh).Property(p => p.PARK_TIME).IsModified = true;
                    con.Entry(vh).Property(p => p.PARK_ADR_ID).IsModified = true;

                    vehicleDAO.doUpdate(scApp, con, vh);
                    con.Entry(vh).State = EntityState.Detached;
                    isSuccess = true;
                }
            }
            finally
            {
                scApp.VehiclPool.PutObject(vh);
            }
            //});
            return isSuccess;
            //}
        }

        public bool setVhIsCycleRunOnWay(string vh_id, string entry_adr)
        {
            //lock (update_lock_obj)
            //{
            bool isSuccess = false;
            //SCUtility.LockWithTimeout(vh_update_lock_obj_pool[vh_id], SCAppConstants.LOCK_TIMEOUT_MS, () =>
            //{
            AVEHICLE vh = scApp.VehiclPool.GetObject();
            //using (DBConnection_EF con = new DBConnection_EF())
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //vh = vehicleDAO.getByID(con, vh_id);
                    vh.VEHICLE_ID = vh_id;
                    con.AVEHICLE.Attach(vh);
                    ACYCLEZONEMASTER masterTemp = scApp.CycleBLL.getCycleZoneMaterByEntryAdr(entry_adr);
                    if (masterTemp != null)
                    {
                        vh.CYCLERUN_ID = masterTemp.CYCLE_ZONE_ID;
                    }
                    else
                    {
                        throw new Exception(string.Format("Cycle Run master not exist,entry adr id:{0}"
                                                         , entry_adr));
                    }
                    vh.IS_CYCLING = false;
                    vh.CYCLERUN_TIME = null;
                    vh.PARK_ADR_ID = string.Empty;
                    vh.PARK_TIME = null;
                    vh.IS_PARKING = false;
                    //vehicleDAO.update(con, vh);
                    con.Entry(vh).Property(p => p.IS_CYCLING).IsModified = true;
                    con.Entry(vh).Property(p => p.CYCLERUN_TIME).IsModified = true;
                    con.Entry(vh).Property(p => p.PARK_ADR_ID).IsModified = true;
                    con.Entry(vh).Property(p => p.PARK_TIME).IsModified = true;
                    con.Entry(vh).Property(p => p.IS_PARKING).IsModified = true;

                    vehicleDAO.doUpdate(scApp, con, vh);
                    con.Entry(vh).State = EntityState.Detached;
                    isSuccess = true;
                }
            }
            finally
            {
                scApp.VehiclPool.PutObject(vh);
            }
            //});
            return isSuccess;
            //}
        }
        public bool setVhIsInCycleRun(string vh_id)
        {
            //lock (update_lock_obj)
            //{
            bool isSuccess = false;
            //SCUtility.LockWithTimeout(vh_update_lock_obj_pool[vh_id], SCAppConstants.LOCK_TIMEOUT_MS, () =>
            //{
            AVEHICLE vh = scApp.VehiclPool.GetObject();
            //DBConnection_EF con = DBConnection_EF.GetContext();
            //using (DBConnection_EF con = new DBConnection_EF())
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //vh = vehicleDAO.getByID(con, vh_id);
                    vh.VEHICLE_ID = vh_id;
                    con.AVEHICLE.Attach(vh);
                    vh.IS_CYCLING = true;
                    vh.CYCLERUN_TIME = DateTime.Now;
                    //將不再有PARK_ADR_ID
                    vh.PARK_ADR_ID = string.Empty;
                    vh.PARK_TIME = null;
                    vh.IS_PARKING = false;
                    //vehicleDAO.update(con, vh);
                    con.Entry(vh).Property(p => p.IS_CYCLING).IsModified = true;
                    con.Entry(vh).Property(p => p.CYCLERUN_TIME).IsModified = true;
                    con.Entry(vh).Property(p => p.PARK_ADR_ID).IsModified = true;
                    con.Entry(vh).Property(p => p.PARK_TIME).IsModified = true;
                    con.Entry(vh).Property(p => p.IS_PARKING).IsModified = true;


                    vehicleDAO.doUpdate(scApp, con, vh);
                    con.Entry(vh).State = EntityState.Detached;
                    isSuccess = true;
                }
            }
            finally
            {
                scApp.VehiclPool.PutObject(vh);
            }
            //resetVhIsInPark(vh_id);
            //});
            return isSuccess;
            //}
        }
        public bool resetVhIsCycleRun(string vh_id)
        {
            //lock (update_lock_obj)
            //{

            bool isSuccess = false;
            //SCUtility.LockWithTimeout(vh_update_lock_obj_pool[vh_id], SCAppConstants.LOCK_TIMEOUT_MS, () =>
            //{
            AVEHICLE vh = scApp.VehiclPool.GetObject();
            //using (DBConnection_EF con = new DBConnection_EF())
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //vh = vehicleDAO.getByID(con, vh_id);
                    vh.VEHICLE_ID = vh_id;
                    con.AVEHICLE.Attach(vh);
                    vh.CYCLERUN_ID = string.Empty;
                    vh.IS_CYCLING = false;
                    vh.CYCLERUN_TIME = null;
                    //vehicleDAO.update(con, vh);
                    con.Entry(vh).Property(p => p.CYCLERUN_ID).IsModified = true;
                    con.Entry(vh).Property(p => p.IS_CYCLING).IsModified = true;
                    con.Entry(vh).Property(p => p.CYCLERUN_TIME).IsModified = true;


                    vehicleDAO.doUpdate(scApp, con, vh);
                    con.Entry(vh).State = EntityState.Detached;
                    isSuccess = true;
                }
            }
            finally
            {
                scApp.VehiclPool.PutObject(vh);
            }
            //});
            return isSuccess;
            //}
        }

        public AVEHICLE getVehicleByIDFromDB(string vh_id, bool isAttached = false)
        {
            AVEHICLE vh = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                vh = vehicleDAO.getByID(con, vh_id);
                if (vh != null && !isAttached)
                {
                    con.Entry(vh).State = EntityState.Detached;
                }
            }
            return vh;
        }

        public AVEHICLE getVehicleByID(string vh_id)
        {
            AVEHICLE vh = null;
            //DBConnection_EF con = DBConnection_EF.GetContext();
            //using (DBConnection_EF con = new DBConnection_EF())
            //using (DBConnection_EF con = DBConnection_EF.GetUContext())
            //{
            //    vh = vehicleDAO.getByID(con, vh_id);
            //    if (!isAttached)
            //    {
            //        con.Entry(vh).State = EntityState.Detached;
            //    }
            //}
            vh = vehicleDAO.getByID(vh_id);

            return vh;
        }
        public AVEHICLE getVehicleByRealID(string vh_id)
        {
            AVEHICLE vh = null;
            //DBConnection_EF con = DBConnection_EF.GetContext();
            //using (DBConnection_EF con = new DBConnection_EF())
            //using (DBConnection_EF con = DBConnection_EF.GetUContext())
            //{
            //    vh = vehicleDAO.getByID(con, vh_id);
            //    if (!isAttached)
            //    {
            //        con.Entry(vh).State = EntityState.Detached;
            //    }
            //}
            vh = vehicleDAO.getByRealID(vh_id);

            return vh;
        }

        public AVEHICLE getVehicleByExcuteMCS_CMD_ID(string mcs_cmd_id)
        {
            AVEHICLE vh = null;
            vh = vehicleDAO.getByMCS_CMD_ID(mcs_cmd_id);

            return vh;
        }

        public AVEHICLE getVehicleByCarrierID(string carrier_id)
        {
            AVEHICLE vh = null;
            vh = vehicleDAO.getByCarrierID(carrier_id);

            return vh;
        }

        public AVEHICLE findBestSuitableVhStepByStepFromAdr(string source, E_VH_TYPE vh_type, bool is_check_has_vh_carry = false)
        {
            AVEHICLE firstVh = null;
            List<AVEHICLE> vhs = null;
            List<String> to_adrs = new List<string>() { source };
            HashSet<string> searchedSection = new HashSet<string>();

            int totalVhCount = scApp.getEQObjCacheManager().getAllVehicle().Count();
            int searchedForVh = 0;
            int eachSearchCount = 3;

            LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
               Data: $"start find best suitable vh step by step from address:{source},vh type:{vh_type},is check has vh to carry:{is_check_has_vh_carry}");
            do
            {
                //1.往上找出適合的車子
                vhs = ListVhByBeginAdr(ref to_adrs, ref searchedSection, eachSearchCount);
                searchedForVh = searchedForVh + vhs.Count;
                //2.過濾掉狀態不符的
                if (!is_check_has_vh_carry)
                    filterVh(ref vhs, vh_type);
                if (searchedForVh >= totalVhCount ||
                    to_adrs.Count == 0)
                    break;
            }
            while (vhs.Count == 0);
            if (vhs != null && vhs.Count > 0)
            {
                firstVh = vhs.FirstOrDefault();
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
                   Data: $"find find best suitable first vh:{firstVh?.VEHICLE_ID} from adr id:{source}");
            }
            else
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
                   Data: $"not find any vehicle, from adr id:{source}");
            }
            return firstVh;
        }


        public AVEHICLE findBestSuitableVhStepByNearest(string source, E_VH_TYPE vh_type, bool is_check_has_vh_carry = false)
        {
            AVEHICLE firstVh = null;
            double dietance = double.MaxValue;
            //1.列出所有車子
            List<AVEHICLE> vhs = cache.loadVhs().ToList();

            //2.過濾掉狀態不符的
            if (!is_check_has_vh_carry)
                filterVh(ref vhs, vh_type);

            (firstVh, dietance) = FindNearestVh(source, vhs);

            return firstVh;
        }

        public (AVEHICLE firstVh, double dietance) findBestSuitableVhStepByNearest(string source)
        {
            //1.列出所有車子
            List<AVEHICLE> vhs = cache.loadVhs().ToList();

            //2.過濾掉狀態不符的
            filterVh(ref vhs, E_VH_TYPE.None);

            return FindNearestVh(source, vhs);
        }

        private (AVEHICLE firstVh, double dietance) FindNearestVh(string source, List<AVEHICLE> vhs)
        {
            AVEHICLE firstVh = null;
            double distance = double.MaxValue;
            foreach (AVEHICLE vh in vhs)
            {
                if (SCUtility.isMatche(vh.CUR_ADR_ID, source))
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
                       Data: $"From source adr:{source}, find vh:{vh.VEHICLE_ID} current adr:{vh.CUR_ADR_ID} of distance:{0}");
                    firstVh = vh;
                    distance = 0;
                    break;
                }
                var check_result = scApp.GuideBLL.IsRoadWalkable(vh.CUR_ADR_ID, source);
                if (check_result.isSuccess)
                {
                    if (check_result.distance < distance)
                    {
                        distance = check_result.distance;
                        firstVh = vh;
                    }
                }
            }
            return (firstVh, distance);
        }


        private void filterVh(ref List<AVEHICLE> vhs, E_VH_TYPE vh_type)
        {
            foreach (AVEHICLE vh in vhs.ToList())
            {
                if (!vh.isTcpIpConnect)
                {
                    vhs.Remove(vh);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
                       Data: $"vh id:{vh.VEHICLE_ID} has ohxc not connection," +
                             $"so filter it out",
                       VehicleID: vh.VEHICLE_ID,
                       CarrierID: vh.CST_ID);
                }
            }
            foreach (AVEHICLE vh in vhs.ToList())
            {
                if (vh.isSynchronizing)
                {
                    vhs.Remove(vh);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
                       Data: $"vh id:{vh.VEHICLE_ID} is synchronizing," +
                             $"so filter it out",
                       VehicleID: vh.VEHICLE_ID,
                       CarrierID: vh.CST_ID);
                }
            }

            foreach (AVEHICLE vh in vhs.ToList())
            {
                if (vh.MODE_STATUS != VHModeStatus.AutoRemote)
                {
                    vhs.Remove(vh);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
                       Data: $"vh id:{vh.VEHICLE_ID} current mode status is {vh.MODE_STATUS}," +
                             $"so filter it out",
                       VehicleID: vh.VEHICLE_ID,
                       CarrierID: vh.CST_ID);
                }
            }
            if (vh_type != E_VH_TYPE.None)
            {
                foreach (AVEHICLE vh in vhs.ToList())
                {
                    if (vh.VEHICLE_TYPE != E_VH_TYPE.None
                        && vh.VEHICLE_TYPE != vh_type)
                    {
                        vhs.Remove(vh);
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
                           Data: $"vh id:{vh.VEHICLE_ID} vh type:{vh.VEHICLE_TYPE}, vehicle type not match current find vh type:{vh_type}," +
                                 $"so filter it out",
                           VehicleID: vh.VEHICLE_ID,
                           CarrierID: vh.CST_ID);
                    }
                }
            }
            foreach (AVEHICLE vh in vhs.ToList())
            {
                if (vh.IsError)
                {
                    vhs.Remove(vh);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
                       Data: $"vh id:{vh.VEHICLE_ID} of error flag is :{vh.IsError}" +
                             $"so filter it out",
                       VehicleID: vh.VEHICLE_ID,
                       CarrierID: vh.CST_ID);
                }
            }
            foreach (AVEHICLE vh in vhs.ToList())
            {
                //if (!SCUtility.isEmpty(vh.OHTC_CMD))
                if (!SCUtility.isEmpty(vh.MCS_CMD))
                {
                    vhs.Remove(vh);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
                       Data: $"vh id:{vh.VEHICLE_ID} has excute mcs command:{vh.MCS_CMD.Trim()}," +
                             $"so filter it out",
                       VehicleID: vh.VEHICLE_ID,
                       CarrierID: vh.CST_ID);
                }
            }
            foreach (AVEHICLE vh in vhs.ToList())
            {
                if (vh.HAS_CST == 1)
                {
                    vhs.Remove(vh);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
                       Data: $"vh id:{vh.VEHICLE_ID} has carry cst,carrier id:{SCUtility.Trim(vh.CST_ID, true)}," +
                             $"so filter it out",
                       VehicleID: vh.VEHICLE_ID,
                       CarrierID: vh.CST_ID);
                }
            }
            foreach (AVEHICLE vh in vhs.ToList())
            {
                if (SCUtility.isEmpty(vh.CUR_ADR_ID))
                {
                    vhs.Remove(vh);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
                       Data: $"vh id:{vh.VEHICLE_ID} current address is empty," +
                             $"so filter it out",
                       VehicleID: vh.VEHICLE_ID,
                       CarrierID: vh.CST_ID);
                }
            }
            //*******************
            //A0.03
            foreach (AVEHICLE vh in vhs.ToList())
            {
                if (!SCUtility.isEmpty(vh.OHTC_CMD))
                {
                    vhs.Remove(vh);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
                       Data: $"vh id:{vh.VEHICLE_ID} has excute OHTC command:{vh.MCS_CMD.Trim()}," +
                             $"so filter it out",
                       VehicleID: vh.VEHICLE_ID,
                       CarrierID: vh.CST_ID);
                }
            }
            //A0.03
            foreach (AVEHICLE vh in vhs.ToList())
            {
                if (scApp.CMDBLL.isCMD_OHTCQueueByVh(vh.VEHICLE_ID))
                {
                    vhs.Remove(vh);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
                       Data: $"vh id:{vh.VEHICLE_ID} has ohxc command in queue," +
                             $"so filter it out",
                       VehicleID: vh.VEHICLE_ID,
                       CarrierID: vh.CST_ID);
                }
            }
            //foreach (AVEHICLE vh in vhs.ToList())
            //{
            //    if (vh.ACT_STATUS == VHActionStatus.Commanding)
            //    {
            //        vhs.Remove(vh);
            //        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
            //           Data: $"vh id:{vh.VEHICLE_ID} is still commanding," +
            //                 $"so filter it out",
            //           VehicleID: vh.VEHICLE_ID,
            //           CarrierID: vh.CST_ID);
            //    }
            //}
        }

        private List<AVEHICLE> ListVhByBeginAdr(ref List<string> to_adrs, ref HashSet<string> searchedSection, int eachSearchCount)
        {
            List<AVEHICLE> vhs = new List<AVEHICLE>();

            do
            {
                List<ASECTION> secs = scApp.MapBLL.loadSectionByToAdrs(to_adrs);
                bool hasNotSearchedSec = false;
                to_adrs.Clear();
                foreach (ASECTION sec in secs)
                {
                    if (!scApp.MapBLL.IsSegmentActive(sec.SEG_NUM))
                    {
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
                           Data: $"By pass segment id:{sec.SEG_NUM}, is inactive");
                        continue;
                    }

                    string section_id = sec.SEC_ID.Trim();
                    if (!searchedSection.Contains(section_id))
                    {
                        hasNotSearchedSec = true;
                        searchedSection.Add(section_id);
                        //List<AVEHICLE> vhs_onSec = loadVehicleOnAutoRemoteBySEC_ID(sec.SEC_ID);
                        List<AVEHICLE> vhs_onSec = loadVehicleBySEC_ID(sec.SEC_ID);
                        if (vhs_onSec != null && vhs_onSec.Count > 0)
                        {
                            string on_sec_vh_id = string.Join(",", vhs_onSec.Select(v => v.VEHICLE_ID));
                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
                               Data: $"find vehicles:{on_sec_vh_id} on {section_id}");
                        }
                        //if (hasErrorVh(vhs_onSec))
                        //{
                        //    if (secs.Last() == sec)
                        //    {
                        //        throw new BlockedByTheErrorVehicleException("Can't find the way to transfer.");
                        //    }
                        //    continue;
                        //}

                        //vhs.AddRange(loadVehicleOnAutoRemoteBySEC_ID(sec.SEC_ID));
                        vhs.AddRange(vhs_onSec);
                        to_adrs.Add(sec.FROM_ADR_ID);
                    }
                }
                if (!hasNotSearchedSec)
                    break;
                //if (adrs.Contains(source))
                //    break;
            } while (vhs.Count < eachSearchCount);
            return vhs;
        }

        public class BlockedByTheErrorVehicleException : Exception
        {
            public BlockedByTheErrorVehicleException(string msg) : base(msg)
            {

            }
        }

        private bool hasErrorVh(List<AVEHICLE> vhs)
        {
            return vhs.Where(vh => vh.IsError).Count() > 0;
        }

        public int getActVhCount()
        {
            int count = 0;
            //using (DBConnection_EF con = new DBConnection_EF())
            //{
            //    count = vehicleDAO.getActVhCount(con);
            //}
            count = vehicleDAO.getActVhCount();
            return count;
        }
        public int getActVhCount(E_VH_TYPE vh_type)
        {
            int count = 0;
            count = vehicleDAO.getActVhCount(vh_type);
            return count;
        }
        public int getIdleVhCount()
        {
            int count = 0;
            //using (DBConnection_EF con = new DBConnection_EF())
            //{
            //    count = vehicleDAO.getIdleVhCount(con);
            //}
            count = vehicleDAO.getIdleVhCount();

            return count;
        }
        public int getIdleVhCount(E_VH_TYPE vh_type)
        {
            int count = 0;
            //using (DBConnection_EF con = new DBConnection_EF())
            //{
            //    count = vehicleDAO.getIdleVhCount(con);
            //}
            count = vehicleDAO.getIdleVhCount(vh_type);

            return count;
        }

        public int getNoExcuteMcsCmdVhCount(E_VH_TYPE vh_type)
        {
            int count = 0;
            //using (DBConnection_EF con = new DBConnection_EF())
            //{
            //    count = vehicleDAO.getIdleVhCount(con);
            //}
            count = vehicleDAO.getNoExcuteMcsCmdVhCount(vh_type);

            return count;
        }

        public int getParkingVhCount()
        {
            int count = 0;
            //using (DBConnection_EF con = new DBConnection_EF())
            //{
            //    count = vehicleDAO.getParkingVhCount(con);
            //}
            count = vehicleDAO.getParkingVhCount();
            return count;
        }
        public int getCyclingVhCount()
        {
            int count = 0;
            //using (DBConnection_EF con = new DBConnection_EF())
            //{
            //    count = vehicleDAO.getCyclingVhCount(con);
            //}
            count = vehicleDAO.getCyclingVhCount();
            return count;
        }

        public bool hasVehicleOnSections(List<string> sections)
        {
            bool has_vh = false;
            List<AVEHICLE> vhs = scApp.getEQObjCacheManager().getAllVehicle();
            var query = from vh in vhs
                        where sections.Contains(vh.CUR_SEC_ID)
                        select vh;
            if (query != null && query.Count() > 0)
            {
                has_vh = true;
            }
            return has_vh;
        }
        public bool DeleteOHTbyVhID(string vh_id)
        {
            bool isSuccsess = true;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    AVEHICLE aVEHICLE = con.AVEHICLE.Where(data => data.VEHICLE_ID.Trim() == vh_id.Trim()).First();
                    vehicleDAO.Delete(con, aVEHICLE);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccsess = false;
            }

            return isSuccsess;
        }
        public List<AVEHICLE> loadAllVehicle()
        {
            List<AVEHICLE> vhs = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                vhs = vehicleDAO.loadAll(con);
            }
            return vhs;
        }
        public List<AVEHICLE> loadVehicleBySEC_ID(string sec_id)
        {
            List<AVEHICLE> vhs = null;
            vhs = vehicleDAO.loadBySEC_ID(sec_id);
            return vhs;
        }
        public List<AVEHICLE> loadVehicleOnAutoRemoteBySEC_ID(string sec_id)
        {
            List<AVEHICLE> vhs = null;
            vhs = vehicleDAO.loadOnAutoRemoteBySEC_ID(sec_id);
            return vhs;
        }

        public List<AVEHICLE> loadParkingVehicle()
        {
            List<AVEHICLE> vhs = null;
            //using (DBConnection_EF con = new DBConnection_EF())
            //{
            //    vhs = vehicleDAO.loadParkingVehicle(con);
            //}
            vhs = vehicleDAO.loadParkingVehicle();
            return vhs;
        }
        public List<AVEHICLE> loadByCycleZoneID(string cyc_zone_id)
        {
            List<AVEHICLE> vhs = null;
            //using (DBConnection_EF con = new DBConnection_EF())
            //{
            //    vhs = vehicleDAO.loadVhByCycleZoneID(con, cyc_zone_id);
            //}
            vhs = vehicleDAO.loadVhByCycleZoneID(cyc_zone_id);
            return vhs;
        }
        public List<AVEHICLE> loadFirstCyclingVhInEachCycleZone()
        {
            List<AVEHICLE> vhs = null;
            //using (DBConnection_EF con = new DBConnection_EF())
            //{
            //    vhs = vehicleDAO.loadFirstCyclingVhInEachCycleZone(con);
            //}
            vhs = vehicleDAO.loadFirstCyclingVhInEachCycleZone();
            return vhs;
        }

        public List<AVEHICLE> loadVhByInSection(string sce_id)
        {
            List<AVEHICLE> vhs = null;
            //using (DBConnection_EF con = new DBConnection_EF())
            //{
            //    vhs = vehicleDAO.loadFirstCyclingVhInEachCycleZone(con);
            //}
            vhs = vehicleDAO.loadFirstCyclingVhInEachCycleZone();
            return vhs;
        }

        public bool hasVhReserveParkAdr(string park_adr)
        {
            AVEHICLE vh = null;
            return hasVhReserveParkAdr(park_adr, out vh);
        }
        public bool hasVhReserveParkAdr(string park_adr, out AVEHICLE vh)
        {
            vh = null;
            //DBConnection_EF con = DBConnection_EF.GetContext();
            //using (DBConnection_EF con = new DBConnection_EF())
            //{
            //    vh = vehicleDAO.getOnWayParkingVhByParkAdr(con, park_adr);
            //}
            vh = vehicleDAO.getOnWayParkingVhByParkAdr(park_adr);
            return vh != null;
        }

        public List<AVEHICLE> loadAllErrorVehicle()
        {
            List<AVEHICLE> vhs = null;
            vhs = vehicleDAO.loadAllErrorVehicle();
            return vhs;
        }

        //private void FindParkZoneOrCycleRunZoneForDriveAway(AVEHICLE vh)
        //{
        //    lock (scApp.pack_lock_obj)
        //    {
        //        APACKZONEDETAIL packDetail = scApp.PackBLL.getPackDetailByAdr(vh.PACK_ADR_ID);
        //        if (packDetail.PRIO > SCAppConstants.FIRST_PARK_PRIO)
        //        {

        //            APACKZONEDETAIL nextPackDetail = scApp.PackBLL.getPackDetailByZoneIDAndPRIO
        //                (packDetail.PACK_ZONE_ID, packDetail.PRIO - 1);
        //            //bool isSuccess = scApp.CMDBLL.creatCommand_OHTC(vh.VEHICLE_ID
        //            //           , string.Empty
        //            //           , string.Empty
        //            //           , E_CMD_TYPE.Move_Pack
        //            //           , vh.CUR_ADR_ID
        //            //           , nextPackDetail.ADR_ID, 0, 0);
        //            //if (isSuccess)
        //            //{
        //            //    isSuccess = scApp.CMDBLL.generateCmd_OHTC_Details();
        //            //    return;
        //            //}
        //            scApp.CMDBLL.doSendTransferCommand(vh.VEHICLE_ID
        //                       , string.Empty
        //                       , string.Empty
        //                       , E_CMD_TYPE.Move_Pack
        //                       , vh.CUR_ADR_ID
        //                       , nextPackDetail.ADR_ID, 0, 0);
        //        }
        //        APACKZONEDETAIL bestPackDetail = null;
        //        APACKZONEMASTER pack_master = null;

        //        int readyComeToVhCountByCMD = 0;
        //        if (!scApp.CMDBLL.hasExcuteCMDFromToAdrIsParkInSpecifyPackZoneID
        //            (packDetail.PACK_ZONE_ID, out readyComeToVhCountByCMD))
        //        {
        //            pack_master = scApp.PackBLL.getPackZoneMasterByAdrID(vh.PACK_ADR_ID);
        //        }
        //        if (scApp.PackBLL.tryFindPackZone(vh, out bestPackDetail, pack_master))
        //        {
        //            bool isSuccess = scApp.CMDBLL.creatCommand_OHTC(vh.VEHICLE_ID
        //                                           , string.Empty
        //                                           , string.Empty
        //                                           , E_CMD_TYPE.Move_Pack
        //                                           , vh.CUR_ADR_ID
        //                                           , bestPackDetail.ADR_ID, 0, 0);
        //            if (isSuccess)
        //            {
        //                isSuccess = scApp.CMDBLL.generateCmd_OHTC_Details();
        //                if (isSuccess)
        //                {
        //                    scApp.PackBLL.tryAdjustTheVhPackingPositionByPackZoneAndPrio(bestPackDetail);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            ACYCLEZONEMASTER bestCycleZone = null;
        //            if (scApp.CycleBLL.tryFindCycleZone(vh, out bestCycleZone))
        //            {
        //                //bool isSuccess = scApp.CMDBLL.creatCommand_OHTC(vh.VEHICLE_ID
        //                //                               , string.Empty
        //                //                               , string.Empty
        //                //                               , E_CMD_TYPE.Round
        //                //                               , vh.CUR_ADR_ID
        //                //                               , bestCycleZone.ENTRY_ADR_ID, 0, 0);
        //                //if (isSuccess)
        //                //{
        //                //    isSuccess = scApp.CMDBLL.generateCmd_OHTC_Details();
        //                //}
        //                scApp.CMDBLL.doSendTransferCommand(vh.VEHICLE_ID
        //                                               , string.Empty
        //                                               , string.Empty
        //                                               , E_CMD_TYPE.Round
        //                                               , vh.CUR_ADR_ID
        //                                               , bestCycleZone.ENTRY_ADR_ID, 0, 0);
        //            }
        //        }
        //    }
        //}
        #region DoSomeThing

        public void DoIdleVehicleHandle_InAction(VhLoadCarrierStatus loadCSTStatus)
        {
            switch (loadCSTStatus)
            {
                case VhLoadCarrierStatus.NotExist:
                    //1.Cancel Command
                    //2.回Home
                    break;
                case VhLoadCarrierStatus.Exist:

                    break;
            }
        }

        public void doLoadArrivals(string eq_id, string current_adr_id, string current_sec_id)
        {
            scApp.CMDBLL.update_CMD_Detail_LoadStartTime(eq_id, current_adr_id, current_sec_id);
            //scApp.VIDBLL.upDateVIDPortID(eq_id, current_adr_id);
            //mcsDefaultMapAction.sendS6F11_common(SECSConst.CEID_Vehicle_Arrived, eq_id);

            AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(eq_id);
            if (!SCUtility.isEmpty(vh.MCS_CMD))
            {
                scApp.SysExcuteQualityBLL.updateSysExecQity_ArrivalSourcePort(vh.MCS_CMD);
            }
            vh.VehicleArrive();
            //NetworkQualityTest(eq_id, current_adr_id, current_sec_id, 0);
        }
        public void doLoading(string eq_id)
        {
            AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(eq_id);
            vh.VehicleAcquireStart();
            //mcsDefaultMapAction.sendS6F11_common(SECSConst.CEID_Vehicle_Acquire_Started, eq_id);
        }
        public void doLoadComplete(string eq_id, string current_adr_id, string current_sec_id, string cst_id)
        {
            scApp.CMDBLL.update_CMD_Detail_LoadEndTime(eq_id, current_adr_id, current_sec_id);
            //updataVehicleCSTID(eq_id, cst_id);
            AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(eq_id);
            //mcsDefaultMapAction.sendS6F11_common(SECSConst.CEID_Carrier_Installed, eq_id);
            //mcsDefaultMapAction.sendS6F11_common(SECSConst.CEID_Vehicle_Acquire_Completed, eq_id);
            //mcsDefaultMapAction.sendS6F11_common(SECSConst.CEID_Vehicle_Departed, eq_id);
            vh.VehilceAcquireComplete();
            vh.VehicleDepart();

            VIDCollection vids = null;
            //if (scApp.VIDBLL.tryGetVIOInfoVIDCollectionByEQID(eq_id, out vids))
            //{
            //    string source_port = vids.VID_65_SourcePort.SOURCE_PORT.Trim();
            //    string cst_id_test = vids.VID_54_CarrierID.CARRIER_ID.Trim();
            //    if (source_port.Length < 10) return;

            //    string ToDevice = source_port.Substring(0, 9);

            //    logger.Trace($"Start Send remove To STK,ToDevice:{ToDevice},Dest Port:{source_port},CST ID:{cst_id_test}");
            //    Task.Run(() => scApp.webClientManager.postInfo2Stock($"{ToDevice}.mirle.com.tw", source_port, cst_id_test, "remove"));
            //    logger.Trace($"End Send remove To STK,ToDevice:{ToDevice},Dest Port:{source_port},CST ID:{cst_id_test}");
            //}
        }
        public void doUnloadArrivals(string eq_id, string current_adr_id, string current_sec_id)
        {
            scApp.CMDBLL.update_CMD_Detail_UnloadStartTime(eq_id, current_adr_id, current_sec_id);
            //scApp.VIDBLL.upDateVIDPortID(eq_id, current_adr_id);
            //mcsDefaultMapAction.sendS6F11_common(SECSConst.CEID_Vehicle_Arrived, eq_id);

            AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(eq_id);
            vh.VehicleArrive();
            if (!SCUtility.isEmpty(vh.MCS_CMD))
            {
                scApp.SysExcuteQualityBLL.updateSysExecQity_ArrivalDestnPort(vh.MCS_CMD);
            }
            //NetworkQualityTest(eq_id, current_adr_id, current_sec_id, 0);
        }
        public void doUnloading(string eq_id)
        {
            AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(eq_id);
            vh.VehicleDepositStart();
        }
        public void doUnloadComplete(string eq_id)
        {
            //updataVehicleCSTID(eq_id, "");
            //mcsDefaultMapAction.sendS6F11_common(SECSConst.CEID_Vehicle_Removed, eq_id);
            //mcsDefaultMapAction.sendS6F11_common(SECSConst.CEID_Vehicle_Deposit_Completed, eq_id);
            //AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(eq_id);
            //if (!SCUtility.isEmpty(vh.MCS_CMD))
            //{
            //    scApp.SysExcuteQualityBLL.updateSysExecQity_CmdFinish(vh.MCS_CMD);
            //}
            AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(eq_id);
            vh.VehicleDepositComplete();

            //VIDCollection vids = null;
            //if (scApp.VIDBLL.tryGetVIOInfoVIDCollectionByEQID(eq_id, out vids))
            //{
            //    string dest_port = vids.VID_60_DestinationPort.DESTINATION_PORT.Trim();
            //    string cst_id_test = vids.VID_54_CarrierID.CARRIER_ID.Trim();
            //    if (dest_port.Length < 10) return;
            //    string ToDevice = dest_port.Substring(0, 9);
            //    logger.Trace($"Start Send waitin To STK,ToDevice:{ToDevice},Dest Port:{dest_port},CST ID:{cst_id_test}");
            //    Task.Run(() => scApp.webClientManager.postInfo2Stock($"{ToDevice}.mirle.com.tw", dest_port, cst_id_test, "waitin"));
            //    logger.Trace($"End Send waitin To STK,ToDevice:{ToDevice},Dest Port:{dest_port},CST ID:{cst_id_test}");
            //}
        }
        //public void doTransferCommandFinish(Equipment eqpt)
        public bool doTransferCommandFinish(string vh_id, string cmd_id, CompleteStatus completeStatus)
        {
            bool isSuccess = true;
            //1.
            try
            {
                scApp.VehicleBLL.getAndProcPositionReportFromRedis(vh_id);
                //TODO 再Update沒有成功這件事情要分成兩個部分，1.沒有找到該筆Command (要回復VH)2.發生Exection(不回復VH)。
                AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(vh_id);
                string mcs_cmd_id = vh.MCS_CMD;

                //vh.startAdr = null;
                vh.FromAdr = null;
                vh.ToAdr = null;
                vh.CMD_CST_ID = null;
                vh.CMD_Priority = 0;
                vh.CmdType = E_CMD_TYPE.Home;

                vh.PredictPath = null;
                vh.WillPassSectionID = null;
                vh.CyclingPath = null;
                vh.procProgress_Percen = 0;
                vh.vh_CMD_Status = E_CMD_STATUS.NormalEnd;

                vh.VehicleUnassign();
                E_CMD_STATUS ohtc_cmd_status = CompleteStatusToCmdStatus(completeStatus);

                ////isSuccess &= scApp.CMDBLL.updateCommand_OHTC_StatusByCmdID(cmd_id, E_CMD_STATUS.NormalEnd);
                //isSuccess &= scApp.CMDBLL.updateCommand_OHTC_StatusByCmdID(cmd_id, ohtc_cmd_status);

                if (!SCUtility.isEmpty(mcs_cmd_id))
                {
                    //E_TRAN_STATUS mcs_cmd_tran_status = CompleteStatusToETransferStatus(completeStatus);
                    //isSuccess &= scApp.SysExcuteQualityBLL.updateSysExecQity_CmdFinish(vh.MCS_CMD);
                    //isSuccess &= scApp.CMDBLL.updateCMD_MCS_TranStatus2Complete(vh.MCS_CMD);
                    ASYSEXCUTEQUALITY quality = null;
                    scApp.SysExcuteQualityBLL.updateSysExecQity_CmdFinish(mcs_cmd_id, ohtc_cmd_status, completeStatus, out quality);
                    //scApp.CMDBLL.updateCMD_MCS_TranStatus2Complete(mcs_cmd_id, E_TRAN_STATUS.Complete);
                    //scApp.CMDBLL.updateCMD_MCS_TranStatus2Complete(mcs_cmd_id, mcs_cmd_tran_status);
                    if (quality != null)
                    {
                        SCUtility.TrimAllParameter(quality);
                        LogManager.GetLogger("SysExcuteQuality").Info(quality.ToString());
                    }
                }
                //isSuccess &= scApp.CMDBLL.updateCommand_OHTC_StatusByVhID(vh_id, E_CMD_STATUS.NormalEnd);
                //updateVehicleExcuteCMD(vh_id, string.Empty, string.Empty);
                vh.NotifyVhExcuteCMDStatusChange();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
                isSuccess = false;
            }
            return isSuccess;
            //2.
            //bool isParkAdr = scApp.ParkBLL.isInPrckAddress(vh.CUR_ADR_ID, vh.VEHICLE_TYPE);
            //if (isParkAdr)
            //{
            //    scApp.VehicleBLL.setVhIsParkingOnWay(eq_id, vh.CUR_ADR_ID);
            //    scApp.ParkBLL.checkAndUpdateVhEntryParkingAdr(eq_id, vh.CUR_ADR_ID);
            //}
            ////bool needMoveToIdle = vh.VEHICLE_TYPE == E_VH_TYPE.Dirty
            ////                   || scApp.CMDBLL.getCMD_MCSisQueueCount() == 0;
            //else
            //{
            //    Task.Run(() =>
            //    {
            //        bool isCmdInQueue = scApp.CMDBLL.isCMD_OHTCQueueByVh(eq_id);
            //        if (!isCmdInQueue)
            //        {
            //            bool isParking = false;
            //            bool isCycleRun = false;
            //            isParking = scApp.ParkBLL.checkAndUpdateVhEntryParkingAdr(eq_id, vh.CUR_ADR_ID);
            //            if (!isParking)
            //            {
            //                isCycleRun = scApp.CycleBLL.checkAndUpdateVhEntryCycleRunAdr(eq_id, vh.CUR_ADR_ID);
            //                if (!isCycleRun)
            //                {
            //                    FindParkZoneOrCycleRunZoneNew(vh);
            //                }
            //            }
            //        }
            //    });
            //}
            //3.
            //mcsDefaultMapAction.sendS6F11_common(SECSConst.CEID_Vehicle_Unassigned, eq_id);
            //mcsDefaultMapAction.sendS6F11_common(SECSConst.CEID_Transfer_Completed, eq_id);
            //scApp.VIDBLL.initialVIDCommandInfo(eq_id);
        }
        public E_CMD_STATUS CompleteStatusToCmdStatus(CompleteStatus completeStatus)
        {
            switch (completeStatus)
            {
                case CompleteStatus.CmpStatusCancel:
                    return E_CMD_STATUS.CancelEndByOHTC;
                case CompleteStatus.CmpStatusAbort:
                    return E_CMD_STATUS.AbnormalEndByOHTC;
                case CompleteStatus.CmpStatusVehicleAbort:
                case CompleteStatus.CmpStatusIdmisMatch:
                case CompleteStatus.CmpStatusIdreadFailed:
                case CompleteStatus.CmpStatusInterlockError:
                case CompleteStatus.CmpStatusLongTimeInaction:
                    return E_CMD_STATUS.AbnormalEndByOHT;
                case CompleteStatus.CmpStatusForceFinishByOp:
                    return E_CMD_STATUS.AbnormalEndByOHTC;
                default:
                    return E_CMD_STATUS.NormalEnd;
            }
        }
        //private E_TRAN_STATUS CompleteStatusToETransferStatus(CompleteStatus completeStatus)
        //{
        //    switch (completeStatus)
        //    {
        //        case CompleteStatus.CmpStatusCancel:
        //            return E_TRAN_STATUS.Canceling;
        //        case CompleteStatus.CmpStatusAbort:
        //        case CompleteStatus.CmpStatusVehicleAbort:
        //            return E_TRAN_STATUS.Aborting;
        //        case CompleteStatus.CmpStatusIdmisMatch:
        //        case CompleteStatus.CmpStatusIdreadFailed:
        //        case CompleteStatus.CmpStatusInterlockError:
        //            return E_TRAN_STATUS.TransferCompleted;
        //        case CompleteStatus.CmpStatusLongTimeInaction:
        //        case CompleteStatus.CmpStatusForceFinishByOp:
        //            return E_TRAN_STATUS.Aborting;
        //        default:
        //            return E_TRAN_STATUS.TransferCompleted;
        //            //return E_TRAN_STATUS.Transferring;
        //    }
        //}




        public bool callVehicleToMove(AVEHICLE vh, string to_adr)
        {
            bool isSuccess = scApp.CMDBLL.doCreatTransferCommand(vh.VEHICLE_ID
                                           , string.Empty
                                           , string.Empty
                                           , E_CMD_TYPE.Move_Park
                                           , vh.CUR_ADR_ID
                                           , to_adr, 0, 0);
            return isSuccess;
        }




        public void doAdrArrivals(string eq_id, string current_adr_id, string current_sec_id)
        {
            // NetworkQualityTest(eq_id, current_adr_id, current_sec_id, 0);
        }
        public void NetworkQualityTest(string eq_id, string current_adr_id, string current_sec_id, int acc_dist)
        {
            if (scApp.getBCFApplication().NetworkQualityTest)
            {
                Task.Run(() => scApp.NetWorkQualityBLL.VhNetworkQualityTest(eq_id, current_adr_id, current_sec_id, acc_dist));
            }
        }




        #endregion




        TimeSpan POSITION_TIMEOUT = new TimeSpan(0, 5, 0);
        public void setAndPublishPositionReportInfo2Redis(string vh_id, string sec_id, string adr_id, uint distance, double x_axis, double y_axis)
        {
            AVEHICLE vh = getVehicleByID(vh_id);
            ID_134_TRANS_EVENT_REP id_134_trans_event_rep = new ID_134_TRANS_EVENT_REP()
            {
                //CSTID = vh.CST_ID,
                CurrentAdrID = adr_id,
                CurrentSecID = sec_id,
                EventType = EventType.AdrPass,
                LeftGuideLockStatus = VhGuideStatus.Unlock,
                RightGuideLockStatus = VhGuideStatus.Unlock,
                SecDistance = distance,
                XAxis = x_axis,
                YAxis = y_axis
            };
            setAndPublishPositionReportInfo2Redis(vh_id, id_134_trans_event_rep);
        }

        public void setAndPublishPositionReportInfo2Redis(string vh_id, ID_143_STATUS_RESPONSE report_obj)
        {
            AVEHICLE vh = getVehicleByID(vh_id);
            ID_134_TRANS_EVENT_REP id_134_trans_event_rep = new ID_134_TRANS_EVENT_REP()
            {
                //CSTID = vh.CST_ID == null ? "" : vh.CST_ID,
                CurrentAdrID = report_obj.CurrentAdrID,
                CurrentSecID = report_obj.CurrentSecID,
                EventType = vh.VhRecentTranEvent,
                LeftGuideLockStatus = VhGuideStatus.Unlock,
                RightGuideLockStatus = VhGuideStatus.Unlock,
                SecDistance = report_obj.SecDistance,
                XAxis = report_obj.XAxis,
                YAxis = report_obj.YAxis
            };
            setAndPublishPositionReportInfo2Redis(vh_id, id_134_trans_event_rep);
        }

        public void setAndPublishPositionReportInfo2Redis(string vh_id, ID_144_STATUS_CHANGE_REP report_obj)
        {
            // Start A0.01 
            //AVEHICLE vh = getVehicleByID(vh_id);
            //ID_134_TRANS_EVENT_REP id_134_trans_event_rep = new ID_134_TRANS_EVENT_REP()
            //{
            //    //CSTID = vh.CST_ID == null ? "" : vh.CST_ID,
            //    CurrentAdrID = report_obj.CurrentAdrID,
            //    CurrentSecID = report_obj.CurrentSecID,
            //    EventType = vh.VhRecentTranEvent,
            //    LeftGuideLockStatus = VhGuideStatus.Unlock,
            //    RightGuideLockStatus = VhGuideStatus.Unlock,
            //    SecDistance = report_obj.SecDistance
            //};
            //setAndPublishPositionReportInfo2Redis(vh_id, id_134_trans_event_rep);
            // End A0.01 

        }
        public void setAndPublishPositionReportInfo2Redis(string vh_id, ID_136_TRANS_EVENT_REP report_obj)
        {
            // Start A0.01 
            //AVEHICLE vh = getVehicleByID(vh_id);
            //ID_134_TRANS_EVENT_REP id_134_trans_event_rep = new ID_134_TRANS_EVENT_REP()
            //{
            //    //CSTID = report_obj.CSTID,
            //    CurrentAdrID = report_obj.CurrentAdrID,
            //    CurrentSecID = report_obj.CurrentSecID,
            //    EventType = report_obj.EventType,
            //    LeftGuideLockStatus = VhGuideStatus.Unlock,
            //    RightGuideLockStatus = VhGuideStatus.Unlock,
            //    //SecDistance = report_obj.SecDistance == 0 ? (uint)vh.ACC_SEC_DIST : report_obj.SecDistance
            //    SecDistance = report_obj.SecDistance
            //};
            //setAndPublishPositionReportInfo2Redis(vh_id, id_134_trans_event_rep);
            // End A0.01 

        }


        public void setAndPublishPositionReportInfo2Redis(string vh_id, ID_172_RANGE_TEACHING_COMPLETE_REPORT report_obj, string sec_id)
        {
            // Start A0.01 
            //AVEHICLE vh = getVehicleByID(vh_id);
            //string from_adr = report_obj.FromAdr;
            //string to_adr = report_obj.ToAdr;
            //ID_134_TRANS_EVENT_REP id_134_trans_event_rep = new ID_134_TRANS_EVENT_REP()
            //{
            //    //CSTID = report_obj.CSTID,
            //    CurrentAdrID = report_obj.ToAdr,
            //    CurrentSecID = sec_id,
            //    EventType = vh.VhRecentTranEvent,
            //    LeftGuideLockStatus = VhGuideStatus.Unlock,
            //    RightGuideLockStatus = VhGuideStatus.Unlock,
            //    SecDistance = report_obj.SecDistance == 0 ? (uint)vh.ACC_SEC_DIST : report_obj.SecDistance
            //};
            //setAndPublishPositionReportInfo2Redis(vh_id, id_134_trans_event_rep);
            // End A0.01 

        }

        public void setAndPublishPositionReportInfo2Redis(string vh_id, ID_132_TRANS_COMPLETE_REPORT report_obj)
        {
            AVEHICLE vh = getVehicleByID(vh_id);
            ID_134_TRANS_EVENT_REP id_134_trans_event_rep = new ID_134_TRANS_EVENT_REP()
            {
                //CSTID = report_obj.CSTID,
                CurrentAdrID = report_obj.CurrentAdrID,
                CurrentSecID = report_obj.CurrentSecID,
                EventType = vh.VhRecentTranEvent,
                LeftGuideLockStatus = VhGuideStatus.Unlock,
                RightGuideLockStatus = VhGuideStatus.Unlock,
                SecDistance = report_obj.SecDistance == 0 ? (uint)vh.ACC_SEC_DIST : report_obj.SecDistance,
                XAxis = vh.X_Axis,
                YAxis = vh.Y_Axis
            };
            setAndPublishPositionReportInfo2Redis(vh_id, id_134_trans_event_rep);
        }
        public void setAndPublishPositionReportInfo2Redis(string vh_id, ID_134_TRANS_EVENT_REP report_obj)
        {
            setPositionReportInfo2Redis(vh_id, report_obj);
            PublishPositionReportInfo2Redis(vh_id, report_obj);
        }
        private void setPositionReportInfo2Redis(string vh_id, ID_134_TRANS_EVENT_REP report_obj)
        {
            //string key_word_position = $"{SCAppConstants.REDIS_KEY_WORD_POSITION_REPORT}_{vh_id}";
            string key_word_position = $"{SCAppConstants.REDIS_KEY_WORD_POSITION_REPORT}#{vh_id}";
            byte[] arrayByte = new byte[report_obj.CalculateSize()];
            report_obj.WriteTo(new Google.Protobuf.CodedOutputStream(arrayByte));
            scApp.getRedisCacheManager().Obj2ByteArraySetAsync(key_word_position, arrayByte, POSITION_TIMEOUT);
        }
        private void PublishPositionReportInfo2Redis(string vh_id, ID_134_TRANS_EVENT_REP report_obj)
        {
            //string key_word_position = $"{SCAppConstants.REDIS_KEY_WORD_POSITION_REPORT}_{vh_id}";
            string key_word_position = $"{SCAppConstants.REDIS_KEY_WORD_POSITION_REPORT}#{vh_id}";
            byte[] arrayByte = new byte[report_obj.CalculateSize()];
            report_obj.WriteTo(new Google.Protobuf.CodedOutputStream(arrayByte));
            scApp.getRedisCacheManager().PublishEvent(key_word_position, arrayByte);
        }


        Google.Protobuf.MessageParser<ID_134_TRANS_EVENT_REP> trans_event_rep_parser = new Google.Protobuf.MessageParser<ID_134_TRANS_EVENT_REP>(() => new ID_134_TRANS_EVENT_REP());
        public void VehiclePositionChangeHandler(RedisChannel channel, RedisValue value)
        {
            //byte[] SerializationPoitionReport = value;
            //ID_134_TRANS_EVENT_REP recive_str = trans_event_rep_parser.ParseFrom(SerializationPoitionReport);
            string vh_id = channel.ToString().Split('#').Last();
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            //lock (vh.PositionRefresh_Sync)
            //{
            //  if (vh.PositionRefreshTimer.ElapsedMilliseconds > SCAppConstants.POSITION_REFRESH_INTERVAL_TIME)
            {
                //vh.PositionRefreshTimer.Restart();
                //dynamic service = scApp.VehicleService;
                //service.PositionReport(scApp.getBCFApplication(), vh, recive_str);
                scApp.VehicleBLL.getAndProcPositionReportFromRedis(vh.VEHICLE_ID);
            }
            //}
        }
        public void loadAllAndProcPositionReportFromRedis()
        {
            var listVh = scApp.getEQObjCacheManager().getAllVehicle();
            foreach (AVEHICLE vh in listVh)
            {
                scApp.VehicleBLL.getAndProcPositionReportFromRedis(vh.VEHICLE_ID);
                SpinWait.SpinUntil(() => false, 5);
            }
        }

        public void getAndProcPositionReportFromRedis(String vh_id)
        {
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            //lock (vh.PositionRefresh_Sync)
            //{
            //string key_word_position = $"{SCAppConstants.REDIS_KEY_WORD_POSITION_REPORT}_{vh_id}";
            string key_word_position = $"{SCAppConstants.REDIS_KEY_WORD_POSITION_REPORT}#{vh_id}";
            byte[] Serialization_PositionRpt = scApp.getRedisCacheManager().StringGet(key_word_position);
            if (Serialization_PositionRpt == null || Serialization_PositionRpt.Count() == 0)
            {
                return;
            }
            ID_134_TRANS_EVENT_REP recive_str = trans_event_rep_parser.ParseFrom(Serialization_PositionRpt);
            dynamic service = scApp.VehicleService;
            //ID_134_TRANS_EVENT_REP recive_str = (ID_134_TRANS_EVENT_REP)vh_position_report_args.objPacket;

            service.PositionReport(scApp.getBCFApplication(), vh, recive_str);
            //}
            //scApp.getRedisCacheManager().KeyDelete(key_word_position);
        }

        public void deleteRedisOfPositionReport(string vh_id)
        {
            //string key_word_position = $"{SCAppConstants.REDIS_KEY_WORD_POSITION_REPORT}_{vh_id}";
            string key_word_position = $"{SCAppConstants.REDIS_KEY_WORD_POSITION_REPORT}#{vh_id}";
            scApp.getRedisCacheManager().KeyDelete(key_word_position);
        }


        #region Vehicle Object Info

        /// <summary>
        /// 將AVEHICLE物件轉換成GBP的VEHICLE_INFO物件
        /// 要用來做物件的序列化所使用
        /// </summary>
        /// <param name="vh"></param>
        /// <returns></returns>
        public static byte[] Convert2GPB_VehicleInfo(AVEHICLE vh)
        {
            int vehicleType = (int)vh.VEHICLE_TYPE;
            int cmd_tpye = (int)vh.CmdType;
            int cmd_status = (int)vh.vh_CMD_Status;
            Timestamp update_time = new Timestamp();
            update_time.Seconds = Convert.ToInt64((((DateTime)vh.UPD_TIME).AddHours(-8) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
            VEHICLE_INFO vh_gpp = new VEHICLE_INFO()
            {
                VEHICLEID = vh.VEHICLE_ID,
                IsTcpIpConnect = vh.isTcpIpConnect,
                VEHICLETYPE = (com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage.VehicleType)vehicleType,
                CURADRID = vh.CUR_ADR_ID == null ? string.Empty : vh.CUR_ADR_ID,
                CURSECID = vh.CUR_SEC_ID == null ? string.Empty : vh.CUR_SEC_ID,
                ACCSECDIST = vh.ACC_SEC_DIST,
                MODESTATUS = vh.MODE_STATUS,
                ACTSTATUS = vh.ACT_STATUS,
                MCSCMD = vh.MCS_CMD == null ? string.Empty : vh.MCS_CMD,
                OHTCCMD = vh.OHTC_CMD == null ? string.Empty : vh.OHTC_CMD,
                CMDPAUSE = vh.CMD_PAUSE,
                BLOCKPAUSE = vh.BLOCK_PAUSE,
                OBSPAUSE = vh.OBS_PAUSE,
                HIDPAUSE = vh.HIDStatus,
                SAFETYDOORPAUSE = vh.SAFETY_DOOR_PAUSE,
                EARTHQUAKEPAUSE = vh.EARTHQUAKE_PAUSE,
                ERROR = vh.ERROR,
                OBSDIST = vh.OBS_DIST,
                HASCST = vh.HAS_CST,
                CSTID = vh.CST_ID == null ? string.Empty : vh.CST_ID,
                VEHICLEACCDIST = vh.VEHICLE_ACC_DIST,
                MANTACCDIST = vh.MANT_ACC_DIST,
                GRIPCOUNT = vh.GRIP_COUNT,
                GRIPMANTCOUNT = vh.GRIP_MANT_COUNT,
                ISPARKING = vh.IS_PARKING,
                PARKADRID = vh.PARK_ADR_ID == null ? string.Empty : vh.PARK_ADR_ID,
                ISCYCLING = vh.IS_CYCLING,
                CYCLERUNID = vh.CYCLERUN_ID == null ? string.Empty : vh.CYCLERUN_ID,

                StartAdr = vh.startAdr == null ? string.Empty : vh.startAdr,
                FromAdr = vh.FromAdr == null ? string.Empty : vh.FromAdr,
                ToAdr = vh.ToAdr == null ? string.Empty : vh.ToAdr,
                CMDPRIOTITY = vh.CMD_Priority,
                CMDCSTID = vh.CMD_CST_ID == null ? string.Empty : vh.CMD_CST_ID,
                Speed = vh.Speed,
                ObsVehicleID = vh.ObsVehicleID == null ? string.Empty : vh.ToAdr,
                CmdType = (com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage.CommandType)cmd_tpye,
                VhCMDStatus = (com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage.CommandStatus)cmd_status,
                VhRecentTranEvent = vh.VhRecentTranEvent,
                ProcProgressPercen = vh.procProgress_Percen,
                State = vh.State,
                UPDTIME = update_time

            };
            if (vh.PredictPath != null)
                vh_gpp.PredictPath.AddRange(vh.PredictPath);
            if (vh.CyclingPath != null)
                vh_gpp.CyclingPath.AddRange(vh.CyclingPath);
            if (vh.WillPassSectionID != null)
                vh_gpp.WillPassSectionID.AddRange(vh.WillPassSectionID);
            if (vh.Alarms != null)
                vh_gpp.Alarms.AddRange(vh.WillPassSectionID);
            LogManager.GetLogger("VehicleHistoricalInfo").Trace(vh_gpp.ToString());

            byte[] arrayByte = new byte[vh_gpp.CalculateSize()];
            vh_gpp.WriteTo(new Google.Protobuf.CodedOutputStream(arrayByte));
            return arrayByte;
        }

        public static VEHICLE_INFO Convert2Object_VehicleInfo(byte[] raw_data)
        {
            return ToObject<VEHICLE_INFO>(raw_data);
        }
        private static T ToObject<T>(byte[] buf) where T : Google.Protobuf.IMessage<T>, new()
        {
            if (buf == null)
                return default(T);
            Google.Protobuf.MessageParser<T> parser = new Google.Protobuf.MessageParser<T>(() => new T());
            return parser.ParseFrom(buf);
        }
        #endregion Vehicle Object Info


        public class Cache
        {
            EQObjCacheManager eqObjCacheManager = null;
            public Cache(EQObjCacheManager eqObjCacheManager)
            {
                this.eqObjCacheManager = eqObjCacheManager;
            }
            public UInt16 getVhCurrentModeInAutoRemoteCount()
            {
                var vhs = eqObjCacheManager.getAllVehicle();
                return (UInt16)vhs.
                       Where(vh => vh.MODE_STATUS == VHModeStatus.AutoRemote).
                       Count();
            }
            public UInt16 getVhCurrentModeInAutoLocalCount()
            {
                var vhs = eqObjCacheManager.getAllVehicle();
                return (UInt16)vhs.
                       Where(vh => vh.MODE_STATUS == VHModeStatus.AutoLocal).
                       Count();
            }
            public UInt16 getVhCurrentStatusInIdleCount()
            {
                var vhs = eqObjCacheManager.getAllVehicle();
                return (UInt16)vhs.
                       Where(vh => vh.MODE_STATUS == VHModeStatus.AutoRemote &&
                                   vh.ACT_STATUS == VHActionStatus.NoCommand &&
                                   !vh.IsError).
                       Count();
            }

            public UInt16 getVhCurrentStatusInErrorCount()
            {
                var vhs = eqObjCacheManager.getAllVehicle();
                return (UInt16)vhs.
                       Where(vh => vh.ERROR == VhStopSingle.StopSingleOn).
                       Count();
            }
            public AVEHICLE getVhByID(string vhID)
            {
                var vh = eqObjCacheManager.getVehicletByVHID(vhID);
                return vh;
            }
            public AVEHICLE getVhByRealID(string vhRealID)
            {
                var vh = eqObjCacheManager.getAllVehicle().
                         Where(v => SCUtility.isMatche(v.Real_ID, vhRealID)).
                         SingleOrDefault();
                return vh;
            }
            public AVEHICLE getVhByNum(int vhNum)
            {
                var vh = eqObjCacheManager.getAllVehicle().Where(v => v.Num == vhNum).FirstOrDefault();

                return vh;
            }
            public AVEHICLE getVhByCSTID(string cstID)
            {
                var vhs = eqObjCacheManager.getAllVehicle();
                return vhs.
                       Where(vh => SCUtility.isMatche(vh.CST_ID, cstID)).
                       FirstOrDefault();
            }


            public List<AVEHICLE> getVhByAddressIDs(string[] adrID)
            {
                var vhs = eqObjCacheManager.getAllVehicle();
                return vhs.
                       Where(vh => adrID.Contains(vh.CUR_ADR_ID)).
                       ToList();
            }
            public AVEHICLE getVhByAddressID(string adrID)
            {
                var vhs = eqObjCacheManager.getAllVehicle();
                return vhs.
                       Where(vh => SCUtility.isMatche(vh.CUR_ADR_ID, adrID)).
                       FirstOrDefault();
            }
            public int getVhByHasCSTIDCount(string cstID)
            {
                var vhs = eqObjCacheManager.getAllVehicle();
                return vhs.
                       Where(vh => vh.HAS_CST == 1
                                    && SCUtility.isMatche(vh.CST_ID, cstID)).
                       Count();
            }

            public List<AVEHICLE> loadVhs()
            {
                var vhs = eqObjCacheManager.getAllVehicle();
                return vhs;
            }
            public List<AVEHICLE> loadVhsBySegmentID(string segmentID)
            {
                var vhs = eqObjCacheManager.getAllVehicle();
                vhs = vhs.Where(vh => SCUtility.isMatche(vh.CUR_SEG_ID, segmentID)).ToList();
                return vhs;
            }
            public List<AVEHICLE> loadVhsByOHTCCommandIDs(List<string> cmdIDs)
            {
                var vhs = eqObjCacheManager.getAllVehicle();
                vhs = vhs.Where(vh => cmdIDs.Contains(SCUtility.Trim(vh.OHTC_CMD, true))).ToList();
                return vhs;
            }

            public AVEHICLE getVehicleByMCSCmdID(string mcsCmdID)
            {
                var vhs = eqObjCacheManager.getAllVehicle();
                AVEHICLE vh = vhs.Where(v => SCUtility.isMatche(v.MCS_CMD, mcsCmdID)).
                    FirstOrDefault();
                return vh;
            }
            public AVEHICLE getVehicleByCSTID(string cstID)
            {
                var vhs = eqObjCacheManager.getAllVehicle();
                AVEHICLE vh = vhs.Where(v => SCUtility.isMatche(v.CST_ID, cstID)).
                    FirstOrDefault();
                return vh;
            }


            public List<AVEHICLE> loadHasCmdVh()
            {
                var vhs = eqObjCacheManager.getAllVehicle();
                return vhs.Where(vh => !SCUtility.isEmpty(vh.OHTC_CMD)).
                           ToList();
            }


        }
        public class Web
        {
            WebClientManager webClientManager = null;
            List<string> notify_urls = new List<string>()
            {
                //"http://stk01.asek21.mirle.com.tw:15000",
                 "http://agvc.asek21.mirle.com.tw:15000"
            };
            const string ERROR_HAPPEND_CONST = "99";

            public Web(WebClientManager _webClient)
            {
                webClientManager = _webClient;
            }

            public void vehicleDisconnection(SCApplication app)
            {
                try
                {
                    string lineName = SCUtility.Trim(app.BC_ID, true);
                    if (SCUtility.isMatche(lineName, "ASE"))
                    {
                        lineName = "line1";
                    }
                    else
                    {
                        if (lineName.Contains("_"))
                        {
                            lineName = lineName.Split('_')[1];
                        }
                        else
                        {
                            return;
                        }
                    }
                    lineName = lineName.ToLower();
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
                       Data: $"Has vh dis connection,notify start.Line name:{lineName}");
                    vehicleDisconnection(lineName);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
                       Data: $"Has vh dis connection,notify end.Line name:{lineName}");
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception:");
                }
            }
            /// <summary>
            /// Notify sample
            ///http://127.0.0.1:15000/weatherforecast/line1_dis
            ///http://127.0.0.1:15000/weatherforecast/loop_dis
            ///http://127.0.0.1:15000/weatherforecast/agv_dis 
            /// </summary>
            /// <param name="lineName"></param>
            public void vehicleDisconnection(string lineName)
            {
                try
                {
                    lineName = lineName.ToLower();
                    string notify_name = $"{lineName}_dis";
                    string[] action_targets = new string[]
                    {
                    "weatherforecast"
                    };
                    string[] param = new string[]
                    {
                        notify_name,
                    };
                    foreach (string notify_url in notify_urls)
                    {
                        string result = webClientManager.GetInfoFromServer(notify_url, action_targets, param);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
            }

        }

    }
}
