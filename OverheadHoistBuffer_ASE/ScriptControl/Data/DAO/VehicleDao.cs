using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.ObjectRelay;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class VehicleDao
    {
        List<AVEHICLE> lstVh = null;

        public void start(List<AVEHICLE> _lstVh)
        {
            lstVh = _lstVh;

        }

        public void add(DBConnection_EF con, AVEHICLE vh_id)
        {
            con.AVEHICLE.Add(vh_id);
            con.SaveChanges();
        }

        public void doUpdate(SCApplication app, DBConnection_EF con, AVEHICLE vh)
        {
            vh.UPD_TIME = DateTime.Now;
            updateCatchManager(app, con, vh);
            update(con, vh);
        }

        //private void updateCatchManager(DBConnection_EF con, AVEHICLE vh)
        //{
        //    var changedEntity = con.Entry(vh);
        //    AVEHICLE vh_vo = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh.VEHICLE_ID);
        //    VehicleObjToShow showObj = SCApplication.getInstance().getEQObjCacheManager().CommonInfo.ObjectToShow_list.
        //        Where(o => o.VEHICLE_ID == vh.VEHICLE_ID).SingleOrDefault();
        //    foreach (string propertyName in changedEntity.CurrentValues.PropertyNames)
        //    {
        //        if (changedEntity.Property(propertyName).IsModified)
        //        {
        //            #region 更新CatchManager的資料
        //            string setPropertyName = string.Empty;
        //            switch (propertyName)
        //            {
        //                case nameof(vh_vo.OBS_PAUSE):
        //                    setPropertyName = nameof(vh_vo.ObstacleStatus);
        //                    break;
        //                case nameof(vh_vo.BLOCK_PAUSE):
        //                    setPropertyName = nameof(vh_vo.BlockingStatus);
        //                    break;
        //                case nameof(vh_vo.CMD_PAUSE):
        //                    setPropertyName = nameof(vh_vo.PauseStatus);
        //                    continue;
        //                default:
        //                    setPropertyName = propertyName;
        //                    break;
        //            }
        //            var prop = typeof(AVEHICLE).GetProperty(setPropertyName);
        //            if (prop != null)
        //            {
        //                prop.SetValue(vh_vo, changedEntity.Property(propertyName).CurrentValue);
        //            }
        //            #endregion 更新CatchManager的資料

        //            #region 更新Show在畫面上 DGV的資料
        //            var prop_for_showObj = typeof(VehicleObjToShow).GetProperty(propertyName);
        //            if (prop_for_showObj != null)
        //            {
        //                prop_for_showObj.SetValue(showObj, changedEntity.Property(propertyName).CurrentValue);
        //            }
        //            #endregion 更新Show在畫面上 DGV的資料
        //        }
        //    }

        //}

        private void updateCatchManager(SCApplication app, DBConnection_EF con, AVEHICLE vh)
        {
            var changedEntity = con.Entry(vh);
            AVEHICLE vh_vo = app.getEQObjCacheManager().getVehicletByVHID(vh.VEHICLE_ID);
            //VehicleObjToShow showObj = SCApplication.getInstance().getEQObjCacheManager().CommonInfo.ObjectToShow_list.
            //    Where(o => o.VEHICLE_ID == vh.VEHICLE_ID).SingleOrDefault();
            foreach (string propertyName in changedEntity.CurrentValues.PropertyNames)
            {
                if (changedEntity.Property(propertyName).IsModified)
                {
                    #region 更新CatchManager的資料
                    string setPropertyName = string.Empty;
                    switch (propertyName)
                    {
                        case nameof(vh_vo.OBS_PAUSE):
                            setPropertyName = nameof(vh_vo.ObstacleStatus);
                            break;
                        case nameof(vh_vo.BLOCK_PAUSE):
                            setPropertyName = nameof(vh_vo.BlockingStatus);
                            break;
                        case nameof(vh_vo.CMD_PAUSE):
                            setPropertyName = nameof(vh_vo.PauseStatus);
                            break;
                        default:
                            setPropertyName = propertyName;
                            break;
                    }
                    var prop = typeof(AVEHICLE).GetProperty(setPropertyName);
                    if (prop != null)
                    {
                        prop.SetValue(vh_vo, changedEntity.Property(propertyName).CurrentValue);
                    }
                    #endregion 更新CatchManager的資料

                    #region 更新Show在畫面上 DGV的資料

                    var prop_for_showObj = typeof(VehicleObjToShow).GetProperty(propertyName);
                    if (prop_for_showObj != null)
                    {
                        //prop_for_showObj.SetValue(showObj, changedEntity.Property(propertyName).CurrentValue);
                        //showObj.NotifyPropertyChanged(propertyName);
                    }
                    #endregion 更新Show在畫面上 DGV的資料
                }
            }

            //var vh_Serialize_ = ZeroFormatter.ZeroFormatterSerializer.Serialize(vh_vo);
            //app.getNatsManager().Publish
            //    (string.Format(SCAppConstants.NATS_SUBJECT_VH_INFO_0, vh_vo.VEHICLE_ID.Trim()), vh_Serialize_);

            //app.getRedisCacheManager().ListSetByIndex
            //    (SCAppConstants.REDIS_LIST_KEY_VEHICLES, vh.VEHICLE_ID, vh_vo.ToString());
        }

        private void update(DBConnection_EF con, AVEHICLE vh)
        {
            //vh_id.UPD_TIME = DateTime.Now.ToString(SCAppConstants.DateTimeFormat_22);
            //bool isDetached = con.Entry(vh_id).State == EntityState.Modified;
            //if (isDetached)
            {
                con.SaveChanges();
            }
        }

        public void Delete(DBConnection_EF conn, AVEHICLE aVEHICLE)
        {
            try
            {
                conn.AVEHICLE.Remove(aVEHICLE);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<AVEHICLE> loadAll(DBConnection_EF con)
        {
            var query = from vh in con.AVEHICLE
                        orderby vh.VEHICLE_ID
                        select vh;
            return query.ToList();
        }

        public AVEHICLE getByID(DBConnection_EF con, String vehicleID)
        {
            var query = from vh in con.AVEHICLE
                        where vh.VEHICLE_ID == vehicleID.Trim()
                        select vh;
            return query.FirstOrDefault();
        }

        public int getActVhCount(DBConnection_EF con)
        {
            var query = from vh in con.AVEHICLE
                        where vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoLocal ||
                        vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoRemote
                        select vh;
            return query.Count();
        }

        public int getIdleVhCount(DBConnection_EF con)
        {
            var query = from vh in con.AVEHICLE
                        where (vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoLocal ||
                        vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoRemote)
                        && vh.ACT_STATUS == ProtocolFormat.OHTMessage.VHActionStatus.NoCommand
                        select vh;
            return query.Count();
        }
        public int getParkingVhCount(DBConnection_EF con)
        {
            var query = from vh in con.AVEHICLE
                        where vh.IS_PARKING
                        select vh;
            return query.Count();
        }
        public int getCyclingVhCount(DBConnection_EF con)
        {
            var query = from vh in con.AVEHICLE
                        where vh.IS_CYCLING
                        select vh;
            return query.Count();
        }
        public List<AVEHICLE> loadBySEC_ID(DBConnection_EF con, String sec_id)
        {
            var query = from point in con.AVEHICLE
                        where point.CUR_SEC_ID == sec_id.Trim()
                        orderby point.ACC_SEC_DIST descending
                        select point;
            return query.ToList();
        }


        public List<AVEHICLE> loadParkingVehicle(DBConnection_EF con)
        {
            var query = from vh in con.AVEHICLE
                        where vh.IS_PARKING
                        select vh;
            return query.ToList();
        }
        public List<AVEHICLE> loadVhByCycleZoneID(DBConnection_EF con, string cyc_zone_id)
        {
            var query = from vh in con.AVEHICLE
                        where vh.CYCLERUN_ID == cyc_zone_id.Trim()
                        select vh;
            return query.ToList();
        }
        public List<AVEHICLE> loadFirstCyclingVhInEachCycleZone(DBConnection_EF con)
        {
            List<AVEHICLE> firstCyclingVh = null; ;

            var query = from vh in con.AVEHICLE
                        where vh.IS_CYCLING &&
                        (vh.CYCLERUN_ID != null && vh.CYCLERUN_ID != string.Empty)
                        group vh by vh.CYCLERUN_ID;
            foreach (var q in query)
            {
                AVEHICLE firstVh = q.OrderBy(vh => vh.CYCLERUN_ID).First();
                if (firstCyclingVh == null)
                    firstCyclingVh = new List<AVEHICLE>();
                firstCyclingVh.Add(firstVh);
            }
            return firstCyclingVh;
        }
        public IQueryable getQueryAllSQL(DBConnection_EF con)
        {
            var query = from vh in con.AVEHICLE
                        select vh;
            return query;
        }
        #region Catch Manage

        //public AVEHICLE getByCondition(String vh_id)
        //{
        //    //var query = lstVh.Where
        //    return query.FirstOrDefault();
        //}

        public AVEHICLE getByID(String vh_id)
        {
            var query = from vh in lstVh
                        where vh.VEHICLE_ID.Trim() == vh_id.Trim()
                        select vh;
            return query.FirstOrDefault();
        }
        public AVEHICLE getByRealID(String vhRealID)
        {
            var query = from vh in lstVh
                        where vh.Real_ID.Trim() == vhRealID.Trim()
                        select vh;
            return query.FirstOrDefault();
        }
        public AVEHICLE getByMCS_CMD_ID(String mcs_cmd_id)
        {
            var query = from vh in lstVh
                        where vh.MCS_CMD != null && vh.MCS_CMD.Trim() == mcs_cmd_id.Trim()
                        select vh;
            return query.FirstOrDefault();
        }

        public AVEHICLE getByCarrierID(String carrier_id)
        {
            var query = from vh in lstVh
                        where vh.CST_ID != null && vh.CST_ID.Trim() == carrier_id.Trim()
                        select vh;
            return query.FirstOrDefault();
        }
        public int getActVhCount()
        {
            var query = from vh in lstVh
                        where vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoLocal
                        || vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoRemote
                        select vh;
            return query.Count();
        }
        public int getActVhCount(E_VH_TYPE vh_type)
        {
            var query = from vh in lstVh
                        where (vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoRemote) &&
                               vh.isTcpIpConnect &&
                               vh.VEHICLE_TYPE == vh_type
                        select vh;
            return query.Count();
        }

        public int getIdleVhCount()
        {
            var query = from vh in lstVh
                        where IsIdle(vh)
                        select vh;
            return query.Count();
        }

        public int getIdleVhCount(E_VH_TYPE vh_type)
        {
            var query = from vh in lstVh
                        where vh.VEHICLE_TYPE == vh_type && IsIdle(vh)
                        select vh;
            return query.Count();
        }

        private static bool IsIdle(AVEHICLE vh)
        {
            bool is_idle = true;
            //1.一定要是Auto的狀態
            is_idle &= vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoLocal ||
                       vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoRemote;
            //2.是處於沒有命令的狀態
            is_idle &= vh.ACT_STATUS == ProtocolFormat.OHTMessage.VHActionStatus.NoCommand;
            //3.不是處於沒有命令的狀態的話就是正在執行Move_Park或Move
            is_idle |= vh.ACT_STATUS == ProtocolFormat.OHTMessage.VHActionStatus.Commanding &&
                       (vh.CmdType == E_CMD_TYPE.Move_Park || vh.CmdType == E_CMD_TYPE.Move);
            return is_idle;
        }

        public int getNoExcuteMcsCmdVhCount(E_VH_TYPE vh_type)
        {
            var query = from vh in lstVh
                        where vh.VEHICLE_TYPE == vh_type &&
                        (vh.MCS_CMD == null || vh.MCS_CMD.Trim() == string.Empty)
                        select vh;
            return query.Count();
        }

        public int getParkingVhCount()
        {
            var query = from vh in lstVh
                        where vh.IS_PARKING
                        select vh;
            return query.Count();
        }
        public int getCyclingVhCount()
        {
            var query = from vh in lstVh
                        where vh.IS_CYCLING
                        select vh;
            return query.Count();
        }
        public List<AVEHICLE> loadBySEC_ID(String sec_id)
        {
            var query = from vh in lstVh
                        where vh.CUR_SEC_ID.Trim() == sec_id.Trim()
                        orderby vh.ACC_SEC_DIST descending
                        select vh;
            return query.ToList();
        }
        public List<AVEHICLE> loadOnAutoRemoteBySEC_ID(String sec_id)
        {
            var query = from vh in lstVh
                        where vh.CUR_SEC_ID.Trim() == sec_id.Trim() &&
                              vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoRemote
                        orderby vh.ACC_SEC_DIST descending
                        select vh;
            return query.ToList();
        }



        //由於該Fun僅用在1.查詢是否已經有人預約了該ParkAdr此功能可以搬到直接查詢ParkDetail的狀態即可
        //而且由於目前多是叫VH直接往ParkZone的第一格去，所以不用在乎該Adr是否有人停，只需要知道該ParkZone位置夠不夠
        public AVEHICLE getOnWayParkingVhByParkAdr(String park_adr_id)
        {
            var query = from vh in lstVh
                        where vh.PARK_ADR_ID.Trim() == park_adr_id.Trim()
                        && vh.IS_PARKING == false
                        select vh;
            return query.FirstOrDefault();
        }

        public List<AVEHICLE> loadParkingVehicle()
        {
            var query = from vh in lstVh
                        where vh.IS_PARKING
                        select vh;
            return query.ToList();
        }
        public List<AVEHICLE> loadVhByCycleZoneID(string cyc_zone_id)
        {
            var query = from vh in lstVh
                        where vh.CYCLERUN_ID.Trim() == cyc_zone_id.Trim()
                        select vh;
            return query.ToList();
        }
        public List<AVEHICLE> loadFirstCyclingVhInEachCycleZone()
        {
            List<AVEHICLE> firstCyclingVh = null;

            var query = from vh in lstVh
                        where vh.IS_CYCLING &&
                        (vh.CYCLERUN_ID != null && vh.CYCLERUN_ID.Trim() != string.Empty)
                        group vh by vh.CYCLERUN_ID;
            foreach (var q in query)
            {
                AVEHICLE firstVh = q.OrderBy(vh => vh.CYCLERUN_ID).First();
                if (firstCyclingVh == null)
                    firstCyclingVh = new List<AVEHICLE>();
                firstCyclingVh.Add(firstVh);
            }
            return firstCyclingVh;
        }
        public List<AVEHICLE> loadParkVehicleByParkAdrID(List<string> park_adrs)
        {
            var query = from vh in lstVh
                        where park_adrs.Contains(vh.PARK_ADR_ID.Trim())
                        select vh;
            return query.ToList();
        }
        public List<AVEHICLE> loadAllErrorVehicle()
        {
            var query = from vh in lstVh
                        where vh.ERROR == ProtocolFormat.OHTMessage.VhStopSingle.StopSingleOn
                        select vh;
            return query.ToList();
        }
        #endregion Catch Manage
    }
}
