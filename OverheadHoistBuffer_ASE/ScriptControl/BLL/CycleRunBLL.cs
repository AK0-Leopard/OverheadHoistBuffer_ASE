using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.DAO;
using com.mirle.ibg3k0.sc.Data.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class CycleRunBLL
    {
        CycleZoneTypeDao cycleZoneTypeDao = null;
        CycleZoneMasterDao cycleZoneMasterDao = null;
        CycleZoneDetailDao cycleZoneDetailDao = null;
        private SCApplication scApp = null;
        public CycleRunBLL()
        {

        }
        public void start(SCApplication app)
        {
            scApp = app;
            cycleZoneTypeDao = scApp.CycleZoneTypeDao;
            cycleZoneMasterDao = scApp.CycleZoneMasterDao;
            cycleZoneDetailDao = scApp.CycleZoneDetailDao;
        }

        public Boolean setCurrentCycleType()
        {
            bool isSuccess = false;
            ACYCLEZONETYPE parkZoneType = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                parkZoneType = cycleZoneTypeDao.getUsingCycleType(con);
            }

            if (parkZoneType != null)
            {
                scApp.getEQObjCacheManager().getLine().
                    Currnet_Cycle_Type = parkZoneType.CYCLE_TYPE_ID.Trim();
            }
            return isSuccess;
        }

        public ACYCLEZONEMASTER getCycleZoneMasterByZoneID(string cyc_zone_id)
        {
            ACYCLEZONEMASTER parkZoneMaster = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                parkZoneMaster = cycleZoneMasterDao.getByID(con, cyc_zone_id);
            }
            return parkZoneMaster;
        }


 

        public List<ACYCLEZONEMASTER> loadCycleRunMasterByCurrentCycleTypeID(string cyc_zone_type_id)
        {
            List<ACYCLEZONEMASTER> cycleZoneMaster = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                cycleZoneMaster = cycleZoneMasterDao.loadByCycleTypeID(con, cyc_zone_type_id);
            }
            return cycleZoneMaster;
        }

        public List<ACYCLEZONEMASTER> loadByCycleTypeIDAndHasCycleSpace(string cyc_zone_type_id)
        {
            List<ACYCLEZONEMASTER> HasCycleSpaceZoneMaster = null;

            using (DBConnection_EF con = new DBConnection_EF())
            {
                HasCycleSpaceZoneMaster = new List<ACYCLEZONEMASTER>();
                List<ACYCLEZONEMASTER> cyc_masters_temp = cycleZoneMasterDao.loadByCycleTypeID(con, cyc_zone_type_id);
                foreach (ACYCLEZONEMASTER cyc_master in cyc_masters_temp)
                {
                    List<AVEHICLE> inCycleRunVHs = scApp.VehicleBLL.loadByCycleZoneID(cyc_master.CYCLE_ZONE_ID);
                    if (inCycleRunVHs.Count() < cyc_master.TOTAL_BORDER)
                    {
                        HasCycleSpaceZoneMaster.Add(cyc_master);
                    }
                }
            }
            return HasCycleSpaceZoneMaster;
        }

        public string[] loadCycleRunSecsByEntryAdr(string entry_adr)
        {
            string[] sec_ids = null;
            //using (DBConnection_EF con = new DBConnection_EF())
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                sec_ids = cycleZoneDetailDao.loadSecsByEntryAdr(con, entry_adr);
            }
            return sec_ids;
        }
        public ACYCLEZONEMASTER getCycleZoneMaterByEntryAdr(string entry_adr)
        {
            ACYCLEZONEMASTER cyc_zone_master = null;
            //using (DBConnection_EF con = new DBConnection_EF())
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                cyc_zone_master = cycleZoneMasterDao.getByEntryAdr(con, entry_adr);
            }
            return cyc_zone_master;
        }

        public bool checkAndUpdateVhEntryCycleRunAdr(string vh_id, string adr_id)
        {
            bool isCyclingAdr = false;
            ACYCLEZONEMASTER cycleZoneMaster = null;
            ALINE line = scApp.getEQObjCacheManager().getLine();
            AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(vh_id);
            if (!SCUtility.isEmpty(vh.CYCLERUN_ID) &&
                !vh.IS_CYCLING)
            {
                //DBConnection_EF con = DBConnection_EF.GetContext();
                //using (DBConnection_EF con = new DBConnection_EF())
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    cycleZoneMaster = cycleZoneMasterDao.getByEntryAdr(con, adr_id);
                    if (cycleZoneMaster == null)
                        return false;
                    if (SCUtility.isMatche(vh.CYCLERUN_ID, cycleZoneMaster.CYCLE_ZONE_ID))
                    {
                        scApp.VehicleBLL.setVhIsInCycleRun(vh_id); //TODO 討論移到144判斷
                        isCyclingAdr = true;
                    }
                }
            }

            return isCyclingAdr;
        }

        public bool checkAndUpdateVhLeavetCycleRunZone(string vh_id)
        {
            bool isLeaveCycling = false;
            AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(vh_id);
            if (vh.IS_CYCLING)
            {
                scApp.VehicleBLL.resetVhIsCycleRun(vh_id);
                isLeaveCycling = true;
            }

            return isLeaveCycling;
        }


    }
}
