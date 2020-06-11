using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.DAO;
using com.mirle.ibg3k0.sc.Data.DAO.EntityFramework;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class HIDBLL
    {
        HIDZoneMasterDao HIDMasterDao = null;
        HIDZoneDetailDao HIDDetailDao = null;
        HIDZoneQueueDao HIDQueueDao = null;
        RedisCacheManager RedisCacheManager = null;
        Dictionary<string, int> AHIDZONEMASTERs = null;
        private SCApplication scApp = null;
        public HIDBLL()
        {

        }
        public void start(SCApplication app)
        {
            scApp = app;
            HIDMasterDao = scApp.HIDZoneMasterDao;
            HIDDetailDao = scApp.HIDZoneDetailDao;
            HIDQueueDao = scApp.HIDZoneQueueDao;
            RedisCacheManager = scApp.getRedisCacheManager();
            AHIDZONEMASTERs = loadAllHIDZoneMAXLoadCount();
        }

        public Dictionary<string, int> loadAllHIDZoneMAXLoadCount()
        {
            List<AHIDZONEMASTER> AHIDZONEMASTERs = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                AHIDZONEMASTERs = HIDMasterDao.loadAllHIDZoneMaster(con);
            }
            return AHIDZONEMASTERs.ToDictionary(master => master.ENTRY_SEC_ID.Trim(), master => master.MAX_LOAD_COUNT);
        }

        #region Master
        public AHIDZONEMASTER GetHidZoneMaster(string entry_sec_id, string leave_adr)
        {
            AHIDZONEMASTER hid_zone_master = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                hid_zone_master = HIDMasterDao.getHidZoneMaster(con, entry_sec_id, leave_adr);
            }
            return hid_zone_master;
        }
        public List<string> loadAllHIDLeaveAdr()
        {
            List<string> adrs = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                adrs = HIDMasterDao.loadAllLeaveAdr(con);
            }
            return adrs;
        }
        #endregion Master

        #region Queue
        public bool doCreatHIDZoneQueueByReqStatus(string vh_id, string entry_sec_id, bool canPass, DateTime req_time)
        {
            bool isSeccess = true;
            E_HIDQueueStatus hidQueueStatus = E_HIDQueueStatus.Request;
            Nullable<System.DateTime> block_time = null;
            if (canPass)
            {
                block_time = req_time;
                hidQueueStatus = E_HIDQueueStatus.Blocking;
            }
            else
            {
                hidQueueStatus = E_HIDQueueStatus.Request;
            }
            AHIDZONEQUEUE hidObj = new AHIDZONEQUEUE
            {
                VEHICLE_ID = vh_id,
                ENTRY_SEC_ID = entry_sec_id,
                REQ_TIME = req_time,
                BLOCK_TIME = block_time,
                STATUS = hidQueueStatus
            };
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                HIDQueueDao.add(con, hidObj);
            }
            return isSeccess;
        }
        public AHIDZONEQUEUE getUsingHIDZoneQueueByVhID(string vh)
        {
            AHIDZONEQUEUE zone_queue = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                zone_queue = HIDQueueDao.getUsingHIDZoneQueue(con, vh);
            }
            return zone_queue;
        }

        public AHIDZONEQUEUE getHIDZoneQueue_FirstReqInPasue(string entry_sec_id)
        {
            AHIDZONEQUEUE zone_queue = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                zone_queue = HIDQueueDao.getHIDZoneQueue_FirstReqInPasue(con, entry_sec_id);
            }
            return zone_queue;
        }


        public bool updateHIDZoneQueue_BlockingTime(string vh_id, string current_sec_id)
        {
            bool isSeccess = true;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                AHIDZONEQUEUE hidObj = HIDQueueDao.getHIDZoneQueue(con, vh_id, current_sec_id);
                if (hidObj != null)
                {
                    hidObj.BLOCK_TIME = DateTime.Now;
                    hidObj.STATUS = E_HIDQueueStatus.Blocking;
                    con.Entry(hidObj).Property(p => p.BLOCK_TIME).IsModified = true;
                    con.Entry(hidObj).Property(p => p.STATUS).IsModified = true;

                    HIDQueueDao.Update(con, hidObj);
                }
            }
            return isSeccess;
        }

        public bool updateHIDZoneQueue_ReleasTime(string car_id, string entry_sec_id)
        {
            bool isSeccess = true;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                AHIDZONEQUEUE hidObj = HIDQueueDao.getHIDZoneQueue(con, car_id, entry_sec_id);
                if (hidObj != null)
                {
                    hidObj.RELEASE_TIME = DateTime.Now;
                    hidObj.STATUS = E_HIDQueueStatus.Release;
                    con.Entry(hidObj).Property(p => p.RELEASE_TIME).IsModified = true;
                    con.Entry(hidObj).Property(p => p.STATUS).IsModified = true;

                    HIDQueueDao.Update(con, hidObj);
                }
            }
            return isSeccess;
        }
        public void updateHIDZoneQueue_Pasue(string car_id, string entry_sec_id, bool isPasue)
        {
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                AHIDZONEQUEUE hidObj = HIDQueueDao.getHIDZoneQueue(con, car_id, entry_sec_id);
                if (hidObj != null)
                {
                    hidObj.IS_PASUE = isPasue;
                    con.Entry(hidObj).Property(p => p.IS_PASUE).IsModified = true;
                    HIDQueueDao.Update(con, hidObj);
                }
            }
        }

        #endregion Queue





        const string REDIS_BLOCK_CONTROL_KEY_VHID = "HID_ZONE_{0}_COUNT";
        public bool hasEnoughSeat(string hid_zone_id, out long current_vh_count,out int hid_zone_max_load_count)
        {
            bool hasEnough = false;
            string check_hid_zone_key = string.Format(REDIS_BLOCK_CONTROL_KEY_VHID, hid_zone_id);
            current_vh_count = (long)RedisCacheManager.StringGet(check_hid_zone_key);
            hid_zone_max_load_count = AHIDZONEMASTERs[hid_zone_id];
            if (current_vh_count <= hid_zone_max_load_count)
            {
                hasEnough = true;
            }
            return hasEnough;
        }

        public void VHEntryHIDZone(string hid_zone_id)
        {
            string check_hid_zone_key = string.Format(REDIS_BLOCK_CONTROL_KEY_VHID, hid_zone_id);
            RedisCacheManager.StringIncrementAsync(check_hid_zone_key);
        }
        public void VHLeaveHIDZone(string hid_zone_id)
        {
            string check_hid_zone_key = string.Format(REDIS_BLOCK_CONTROL_KEY_VHID, hid_zone_id);
            RedisCacheManager.StringDecrementAsync(check_hid_zone_key);
        }

    }
}
