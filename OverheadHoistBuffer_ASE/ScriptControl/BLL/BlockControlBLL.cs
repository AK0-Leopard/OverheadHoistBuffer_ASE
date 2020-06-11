using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.DAO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class BlockControlBLL
    {
        SCApplication scApp = null;

        public Redis redis { get; private set; }
        public Database dataBase { get; private set; }


        public void start(SCApplication app)
        {
            scApp = app;


        }

        public class Redis
        {
            RedisCacheManager redisCacheManager = null;
            public Redis(SCApplication _scApp)
            {
                redisCacheManager = _scApp.getRedisCacheManager();
            }
            const string REDIS_BLOCK_CONTROL_KEY_VHID = "BLOCK_CONTROL_{0}";
            const string REDIS_BLOCK_CONTROL_VALUE_SECID_STATUS = "{0},{1}";
            TimeSpan timeOut_5min = new TimeSpan(0, 5, 0);
            public void CreatBlockControlKeyWordToRedis(string vh_id, string block_zone_id, bool can_pass, DateTime req_time)
            {
                string status = can_pass ? SCAppConstants.BlockQueueState.Blocking : SCAppConstants.BlockQueueState.Request;
                block_zone_id = SCUtility.Trim(block_zone_id);
                string set_key_word = string.Format(REDIS_BLOCK_CONTROL_KEY_VHID, vh_id);
                string set_value_word = string.Format(REDIS_BLOCK_CONTROL_VALUE_SECID_STATUS, block_zone_id, status);
                redisCacheManager.stringSetAsync
                    (set_key_word, set_value_word);
                LogCollection.BlockControlLogger.Trace($"creat block zone to redis, vh id:{vh_id},block id:{block_zone_id},block status:{status}");
            }
            public void ChangeBlockControlStatus_Blocking(string vh_id)
            {
                string set_key_word = string.Format(REDIS_BLOCK_CONTROL_KEY_VHID, vh_id);
                var blockInfo = redisCacheManager.StringGet(set_key_word);
                if (blockInfo.HasValue)
                {
                    string[] blockInfos = ((string)blockInfo).Split(',');
                    string sec_id = blockInfos[0];
                    string status = blockInfos[1];
                    string set_value_word = string.Format(REDIS_BLOCK_CONTROL_VALUE_SECID_STATUS, sec_id, SCAppConstants.BlockQueueState.Blocking);
                    redisCacheManager.stringSetAsync(set_key_word, set_value_word);
                    LogCollection.BlockControlLogger.Trace($"change redis block zone status , vh id:{vh_id},block id:{sec_id},block status:{status} change to {SCAppConstants.BlockQueueState.Blocking}");
                }
                else
                {
                    LogCollection.BlockControlLogger.Warn($"vh id[{vh_id}] change redis block zone status to {SCAppConstants.BlockQueueState.Blocking}, but not exists.");
                }
            }
            public void ChangeBlockControlStatus_Through(string vh_id)
            {
                string set_key_word = string.Format(REDIS_BLOCK_CONTROL_KEY_VHID, vh_id);
                var blockInfo = redisCacheManager.StringGet(set_key_word);
                if (blockInfo.HasValue)
                {
                    string[] blockInfos = ((string)blockInfo).Split(',');
                    string sec_id = blockInfos[0];
                    string status = blockInfos[1];
                    string set_value_word = string.Format(REDIS_BLOCK_CONTROL_VALUE_SECID_STATUS, sec_id, SCAppConstants.BlockQueueState.Through);
                    redisCacheManager.stringSetAsync(set_key_word, set_value_word);
                    LogCollection.BlockControlLogger.Trace($"change redis block zone status , vh id:{vh_id},block id:{sec_id},block status:{status} change to {SCAppConstants.BlockQueueState.Through}");
                }
            }
            public void DeleteBlockControlKeyWordToRedis(string vh_id, string entry_sec_id)
            {
                string set_key_word = string.Format(REDIS_BLOCK_CONTROL_KEY_VHID, vh_id.Trim());
                string statue = string.Empty;
                if (tryGetBlock(vh_id, entry_sec_id, out statue))
                {
                    redisCacheManager.KeyDelete(set_key_word);
                    LogCollection.BlockControlLogger.Trace($"delete redis block zone , vh id:{vh_id}");
                }
            }
            public bool HasBlockControlAskedFromRedis(string vh_id, out string block_id, out string block_status)
            {
                bool hasBlockControl = false;
                string set_key_word = string.Format(REDIS_BLOCK_CONTROL_KEY_VHID, vh_id);
                var blockInfo = redisCacheManager.StringGet(set_key_word);
                block_id = string.Empty;
                block_status = string.Empty;
                if (blockInfo.HasValue)
                {
                    string[] blockInfos = ((string)blockInfo).Split(',');
                    block_id = blockInfos[0];
                    block_status = blockInfos[1];
                    hasBlockControl = true;
                }
                return hasBlockControl;
            }

            public bool IsBeforeBlockControlStatus(string vh_id, string queue_status)
            {
                bool isBefore = false;
                string set_key_word = string.Format(REDIS_BLOCK_CONTROL_KEY_VHID, vh_id);
                var blockInfo = redisCacheManager.StringGet(set_key_word);
                string sec_id = string.Empty;
                string status = string.Empty;
                if (blockInfo.HasValue)
                {
                    string[] blockInfos = ((string)blockInfo).Split(',');
                    sec_id = blockInfos[0];
                    status = blockInfos[1];
                    if (queue_status.CompareTo(status) > 0)
                    {
                        isBefore = true;
                    }
                }
                LogCollection.BlockControlLogger.Trace
                ($"check block zone status is before, VH_ID [{vh_id}],block status [{queue_status}] " +
                $"is before [{isBefore}],redis block id:{sec_id},redis block status:{status}");
                return isBefore;
            }
            public bool IsBlockControlStatus(string vh_id, string queue_status)
            {
                string set_key_word = string.Format(REDIS_BLOCK_CONTROL_KEY_VHID, vh_id);
                var blockInfo = redisCacheManager.StringGet(set_key_word);
                string sec_id = string.Empty;
                string status = string.Empty;
                bool isInStatus = false;
                if (blockInfo.HasValue)
                {
                    string[] blockInfos = ((string)blockInfo).Split(',');
                    sec_id = blockInfos[0];
                    status = blockInfos[1];
                    isInStatus = SCUtility.isMatche(queue_status, status);
                }
                LogCollection.BlockControlLogger.Trace
                ($"Check block zone status is in status, VH_ID [{vh_id}],block status [{queue_status}] " +
                $"is in stauts [{isInStatus}],redis block id:{sec_id},redis block status:{status}");
                return isInStatus;
            }
            public bool tryGetInRequest(string vh_id, out string block_zone_id, out string status)
            {
                bool isSuccess = false;
                block_zone_id = string.Empty;
                status = string.Empty;
                string set_key_word = string.Format(REDIS_BLOCK_CONTROL_KEY_VHID, vh_id);
                if (redisCacheManager.KeyExists(set_key_word))
                {
                    var blockInfo = redisCacheManager.StringGet(set_key_word);
                    if (blockInfo.HasValue)
                    {
                        string[] blockInfos = ((string)blockInfo).Split(',');
                        block_zone_id = blockInfos[0];
                        status = blockInfos[1];
                        if (SCUtility.isMatche(status, SCAppConstants.BlockQueueState.Request))
                            isSuccess = true;
                    }
                }
                return isSuccess;
            }
            public bool tryGetBlock(string vh_id, string block_zone_id, out string status)
            {
                bool isSuccess = false;
                status = string.Empty;
                string set_key_word = string.Format(REDIS_BLOCK_CONTROL_KEY_VHID, vh_id);
                if (redisCacheManager.KeyExists(set_key_word))
                {
                    var blockInfo = redisCacheManager.StringGet(set_key_word);
                    if (blockInfo.HasValue)
                    {
                        string[] blockInfos = ((string)blockInfo).Split(',');
                        string current_redis_block_id = blockInfos[0];
                        status = blockInfos[1];
                        if (SCUtility.isMatche(current_redis_block_id, block_zone_id))
                            isSuccess = true;
                    }
                }
                return isSuccess;
            }

        }

        public class Database
        {
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            BlockZoneMasterDao blockZoneMasterDao = null;
            BlockZoneDetailDao blockZoneDetaiDao = null;
            BlockZoneQueueDao blockZoneQueueDao = null;
            public Database(SCApplication _scApp)
            {
                blockZoneMasterDao = _scApp.BlockZoneMasterDao;
                blockZoneDetaiDao = _scApp.BolckZoneDetaiDao;
                blockZoneQueueDao = _scApp.BlockZoneQueueDao;
            }

            public bool doCreatBlockZoneQueueByReqStatus(string car_id, string entry_sec_id, bool canPass, DateTime req_time)
            {
                bool isSeccess = true;
                string blockQueueStatus = string.Empty;
                Nullable<System.DateTime> block_time = null;
                if (canPass)
                {
                    block_time = req_time;
                    blockQueueStatus = SCAppConstants.BlockQueueState.Blocking;
                }
                else
                {
                    blockQueueStatus = SCAppConstants.BlockQueueState.Request;
                }
                BLOCKZONEQUEUE blockObj = new BLOCKZONEQUEUE
                {
                    CAR_ID = car_id,
                    ENTRY_SEC_ID = entry_sec_id,
                    REQ_TIME = DateTime.Now,
                    BLOCK_TIME = block_time,
                    STATUS = blockQueueStatus
                };
                //   using (TransactionScope tx = new TransactionScope
                //(TransactionScopeOption.Suppress))
                //   {
                //using (DBConnection_EF con = new DBConnection_EF())
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    blockZoneQueueDao.add(con, blockObj);
                }
                //}
                return isSeccess;
            }

            public bool addBlockZoneQueue(string car_id, string entry_sec_id)
            {
                bool isSeccess = true;
                BLOCKZONEQUEUE blockObj = new BLOCKZONEQUEUE
                {
                    CAR_ID = car_id,
                    ENTRY_SEC_ID = entry_sec_id,
                    REQ_TIME = DateTime.Now,
                    STATUS = SCAppConstants.BlockQueueState.Request
                };
                using (DBConnection_EF con = new DBConnection_EF())
                {
                    blockZoneQueueDao.add(con, blockObj);
                }
                return isSeccess;
            }
            public bool updateBlockZoneQueue_BlockTime(string car_id, string current_sec_id)
            {
                bool isSeccess = true;

                //DBConnection_EF con = DBConnection_EF.GetContext(out isNew);
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    BLOCKZONEQUEUE blockObj = blockZoneQueueDao.getUsingBlockQueueByCarIDSecID(con, car_id, current_sec_id);
                    blockObj.BLOCK_TIME = DateTime.Now;
                    blockObj.STATUS = SCAppConstants.BlockQueueState.Blocking;
                    blockZoneQueueDao.Update(con, blockObj);
                    con.Release();
                }

                return isSeccess;
            }
            //public bool updateBlockZoneQueue_ThrouTime(string car_id, string current_sec_id, out BLOCKZONEQUEUE throu_block_queue)
            public bool updateBlockZoneQueue_ThrouTime(string car_id, out BLOCKZONEQUEUE throu_block_queue)
            {
                bool isSeccess = true;
                //DBConnection_EF con = DBConnection_EF.GetContext();
                try
                {
                    //using (DBConnection_EF con = new DBConnection_EF())
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        //BLOCKZONEQUEUE blockObj = blockZoneQueueDao.getUsingBlockQueueByCarIDSecID(con, car_id, current_sec_id);
                        //throu_block_queue = blockZoneQueueDao.getThrouTimeNullBlockQueueByCarIDSecID(con, car_id, current_sec_id);

                        throu_block_queue = blockZoneQueueDao.getThrouTimeNullBlockQueueByCarID(con, car_id);
                        if (throu_block_queue != null)
                        {

                            throu_block_queue.THROU_TIME = DateTime.Now;
                            throu_block_queue.STATUS = SCAppConstants.BlockQueueState.Through;
                            con.Entry(throu_block_queue).Property(p => p.THROU_TIME).IsModified = true;
                            con.Entry(throu_block_queue).Property(p => p.STATUS).IsModified = true;

                            //bool isDetached = con.Entry(blockObj).State == EntityState.Modified;
                            //if (isDetached)
                            blockZoneQueueDao.Update(con, throu_block_queue);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                    isSeccess = false;
                    throu_block_queue = null;
                }
                return isSeccess;
            }
            public bool updateBlockZoneQueue_ReleasTime(string car_id, string current_sec_id)
            {
                bool isSeccess = true;
                //DBConnection_EF con = DBConnection_EF.GetContext();
                //using (DBConnection_EF con = new DBConnection_EF())
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    BLOCKZONEQUEUE blockObj = blockZoneQueueDao.getUsingBlockQueueByCarIDSecID(con, car_id, current_sec_id);
                    if (blockObj != null)
                    {
                        blockObj.RELEASE_TIME = DateTime.Now;
                        blockObj.STATUS = SCAppConstants.BlockQueueState.Release;
                        con.Entry(blockObj).Property(p => p.RELEASE_TIME).IsModified = true;
                        con.Entry(blockObj).Property(p => p.STATUS).IsModified = true;

                        blockZoneQueueDao.Update(con, blockObj);
                    }
                }
                return isSeccess;
            }
            public bool updateBlockZoneQueue_AbnormalEnd(BLOCKZONEQUEUE blockZoneQueue, string status)
            {
                bool isSeccess = true;
                //DBConnection_EF con = DBConnection_EF.GetContext(out isNwe);
                if (blockZoneQueue != null)
                {
                    //DBConnection_EF con = DBConnection_EF.GetContext();
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        con.BLOCKZONEQUEUE.Attach(blockZoneQueue);
                        blockZoneQueue.RELEASE_TIME = DateTime.Now;
                        blockZoneQueue.STATUS = status;
                        con.Entry(blockZoneQueue).Property(p => p.RELEASE_TIME).IsModified = true;
                        con.Entry(blockZoneQueue).Property(p => p.STATUS).IsModified = true;

                        //con.Entry(blockZoneQueue).State = EntityState.Modified;
                        blockZoneQueueDao.Update(con, blockZoneQueue);
                        //con.Commit();
                    }
                }
                else
                {
                    isSeccess = false;
                }

                return isSeccess;
            }

            public bool isRepeatRequestBlockZoneByVhIDAndCrtBlockSecID(string vh_id, string sec_id)
            {
                int blockZoneQueue = 0;
                //DBConnection_EF con = DBConnection_EF.GetContext();
                //using (DBConnection_EF con = new DBConnection_EF())
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    blockZoneQueue = blockZoneQueueDao.getCountReqBlockQueueByCarIDSecID(con, vh_id, sec_id);
                }
                return blockZoneQueue != 0;
            }
            public bool isBlockingBlockZoneByVhIDAndCrtBlockSecID(string vh_id, string sec_id)
            {
                int blockZoneQueue = 0;
                //DBConnection_EF con = DBConnection_EF.GetContext();
                //using (DBConnection_EF con = new DBConnection_EF())
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    blockZoneQueue = blockZoneQueueDao.getCountBlockingBlockQueueByCarIDSecID(con, vh_id, sec_id);
                }
                return blockZoneQueue != 0;

            }

            public BLOCKZONEQUEUE getReqBlockQueueBySecID(string sec_id)
            {
                BLOCKZONEQUEUE blockZoneQueue = null;
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    blockZoneQueue = blockZoneQueueDao.getReqBlockQueueBySecID(con, sec_id);
                }
                return blockZoneQueue;
            }


            public bool checkBlockZoneQueueIsBlockingByEntrySecID(List<string> entry_sec_ids)
            {
                bool isBlocking = false;
                //DBConnection_EF con = DBConnection_EF.GetContext();
                //using (DBConnection_EF con = new DBConnection_EF())
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    isBlocking = blockZoneQueueDao.getCountBlockingQueueBySecID(con, entry_sec_ids) != 0;
                }
                return isBlocking;
            }
            public bool checkBlockZoneQueueIsBlockingByEntrySecID(List<string> entry_sec_ids, out List<BLOCKZONEQUEUE> queues)
            {
                bool isBlocking = false;
                //DBConnection_EF con = DBConnection_EF.GetContext();
                //using (DBConnection_EF con = new DBConnection_EF())
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    queues = blockZoneQueueDao.loadBlockingQueueBySecID(con, entry_sec_ids);
                    isBlocking = (queues != null && queues.Count > 0);
                }
                return isBlocking;
            }

            public List<BLOCKZONEQUEUE> loadAllProblematicUsingBlockQueue()
            {
                int BlockWarnTime_s = 15;
                List<BLOCKZONEQUEUE> blockZoneQueues = null;
                //DBConnection_EF con = DBConnection_EF.GetContext();
                //using (DBConnection_EF con = new DBConnection_EF())
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    blockZoneQueues = blockZoneQueueDao.loadAllProblematicUsingBlockQueue(con, BlockWarnTime_s);
                }
                return blockZoneQueues;
            }
            public List<BLOCKZONEQUEUE> loadAllNonReleaseBlockQueue()
            {
                List<BLOCKZONEQUEUE> blockZoneQueues = null;
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    blockZoneQueues = blockZoneQueueDao.loadAllNonReleaseBlockQueue(con);
                }
                return blockZoneQueues;
            }
            public List<BLOCKZONEQUEUE> loadAllUsingBlockQueue()
            {
                List<BLOCKZONEQUEUE> blockZoneQueues = null;
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    blockZoneQueues = blockZoneQueueDao.loadAllUsingBlockQueue(con);
                }
                return blockZoneQueues;
            }
            public List<BLOCKZONEQUEUE> loadNonReleaseBlockQueueBySecIds(List<string> entry_sections)
            {
                List<BLOCKZONEQUEUE> blockZoneQueues = null;
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    blockZoneQueues = blockZoneQueueDao.loadNonReleaseBlockQueueBySecIds(con, entry_sections);
                }
                return blockZoneQueues;
            }

            public List<BLOCKZONEQUEUE> loadNonReleaseBlockQueueByCarID(string vh_id)
            {
                List<BLOCKZONEQUEUE> blockZoneQueues = null;
                //DBConnection_EF con = DBConnection_EF.GetContext();
                //using (DBConnection_EF con = new DBConnection_EF())
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    blockZoneQueues = blockZoneQueueDao.loadUsingBlockQueueByCarID(con, vh_id);
                }
                return blockZoneQueues;
            }

            public List<ABLOCKZONEDETAIL> loadAllBlockZoneDetail()
            {
                List<ABLOCKZONEDETAIL> blockZoneDetails = null;
                //using (DBConnection_EF con = new DBConnection_EF())
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    blockZoneDetails = blockZoneDetaiDao.loadAll(con);
                }
                return blockZoneDetails;
            }

            public List<string> loadBlockZoneDetailSecIDsByEntrySecID(string entry_sec_id)
            {
                List<string> lstSecID = null;
                //using (DBConnection_EF con = new DBConnection_EF())
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    lstSecID = blockZoneDetaiDao.loadSecIDByEntrySecID(con, entry_sec_id);
                }
                return lstSecID;
            }

            public ABLOCKZONEMASTER getBlockZoneMasterByEntrySecID(string entry_sec_id)
            {
                ABLOCKZONEMASTER block = null;
                //DBConnection_EF con = DBConnection_EF.GetContext(out isNew);
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    block = blockZoneMasterDao.getByID(con, entry_sec_id);
                    con.Release();
                }
                return block;

            }
            public ABLOCKZONEMASTER getBlockZoneMasterByAdrID(string adr_id)
            {
                ABLOCKZONEMASTER obj = null;
                using (DBConnection_EF con = new DBConnection_EF())
                {
                    obj = blockZoneMasterDao.getByAdrID(con, adr_id);
                }
                return obj;
            }

            public ABLOCKZONEMASTER getCurrentReleaseBlock(List<string> entry_sec_ids, string adr_id)
            {
                List<ABLOCKZONEMASTER> objs = null;
                //DBConnection_EF con = DBConnection_EF.GetContext();
                //using (DBConnection_EF con = new DBConnection_EF())
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    objs = blockZoneMasterDao.loadByIDsAndAdrID(con, entry_sec_ids, adr_id);
                }
                return objs.SingleOrDefault();
            }

            public ABLOCKZONEMASTER getBlockZoneMasterByBlockIDsAndAdrID(string entry_sec_id, string adr_id)
            {
                ABLOCKZONEMASTER obj = null;
                //DBConnection_EF con = DBConnection_EF.GetContext();
                //using (DBConnection_EF con = new DBConnection_EF())
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    obj = blockZoneMasterDao.getByIDAndAdrID(con, entry_sec_id, adr_id);
                }
                return obj;
            }

            public List<ABLOCKZONEMASTER> loadBZMByAdrID(string adr_id)
            {
                List<ABLOCKZONEMASTER> lsrbzm = null;
                //DBConnection_EF con = DBConnection_EF.GetContext(out isNew);
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    lsrbzm = blockZoneMasterDao.loadBZMByAdrID(con, adr_id);
                }

                return lsrbzm;
            }




            public BLOCKZONEQUEUE findNextBlockZonePassVh_Branch(List<ABLOCKZONEMASTER> bzms)
            {
                BLOCKZONEQUEUE lstBZQ = null;

                //由於有發生deadlock的問題所以改用獨立的connection。
                //DBConnection_EF con = DBConnection_EF.GetContext(); 
                //using (TransactionScope tx = new TransactionScope
                //    (TransactionScopeOption.Suppress))
                //{
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //con.Configuration.AutoDetectChangesEnabled = false;
                    lstBZQ = blockZoneQueueDao.getFirstReqBlockQueueBySecIds(con, bzms.Select(bzm => bzm.ENTRY_SEC_ID.Trim()).ToList());
                    con.Release();
                }
                //}
                //}
                return lstBZQ;
            }


            public BLOCKZONEQUEUE findNextBlockZonePassVh_Merge(List<ABLOCKZONEMASTER> bzms)
            {
                //1.找出BlockZoneMaster所有正在等待通過的Queue
                BLOCKZONEQUEUE BZQ = null;
                //由於有發生deadlock的問題所以改用獨立的connection。
                //DBConnection_EF con = DBConnection_EF.GetContext();
                //using (TransactionScope tx = new TransactionScope
                //        (TransactionScopeOption.Suppress))
                //{
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //con.Configuration.AutoDetectChangesEnabled = false;
                    BZQ = blockZoneQueueDao.getFirstReqBlockQueueBySecIds(con, bzms.Select(bzm => bzm.ENTRY_SEC_ID.Trim()).ToList());
                    con.Release();
                }
                //}
                //2.ToDo按照這些車輛是否有貨做排序

                //3.ToDo按照MSC所給的命令優先順序做排序

                return BZQ;
            }

            public (bool, BLOCKZONEQUEUE) CheckAndNoticeBlockVhPassByAdrID(string leave_adr_id)
            {
                List<ABLOCKZONEMASTER> lstBZM = loadBZMByAdrID(leave_adr_id);
                BLOCKZONEQUEUE waitblockZoneVH = null;
                if (lstBZM == null || lstBZM.Count == 0)
                    return (false, null);

                //foreach (ABLOCKZONEMASTER master in lstBZM)
                //{
                //switch (master.BLOCK_ZONE_TYPE)
                switch (lstBZM[0].BLOCK_ZONE_TYPE)
                {
                    case E_BLOCK_ZONE_TYPE.Branch:
                        waitblockZoneVH = findNextBlockZonePassVh_Branch(lstBZM);
                        return (waitblockZoneVH != null, waitblockZoneVH);
                    case E_BLOCK_ZONE_TYPE.Merge:
                        waitblockZoneVH = findNextBlockZonePassVh_Merge(lstBZM);
                        return (waitblockZoneVH != null, waitblockZoneVH);
                }

                return (false, null);
            }
            // public bool CheckAndNoticeBlockVhPassByAdrID(string leave_adr_id)
            //{
            //    bool isSuccess = true;
            //    List<ABLOCKZONEMASTER> lstBZM = scApp.MapBLL.loadBZMByAdrID(leave_adr_id);
            //    BLOCKZONEQUEUE waitblockZoneVH = null;
            //    if (lstBZM == null || lstBZM.Count == 0)
            //        return true;

            //    //foreach (ABLOCKZONEMASTER master in lstBZM)
            //    //{
            //    //switch (master.BLOCK_ZONE_TYPE)
            //    switch (lstBZM[0].BLOCK_ZONE_TYPE)
            //    {
            //        case E_BLOCK_ZONE_TYPE.Branch:
            //            waitblockZoneVH = scApp.MapBLL.findNextBlockZonePassVh_Branch(lstBZM);
            //            break;
            //        case E_BLOCK_ZONE_TYPE.Merge:
            //            waitblockZoneVH = scApp.MapBLL.findNextBlockZonePassVh_Merge(lstBZM);
            //            break;
            //    }
            //    if (waitblockZoneVH != null)
            //    {
            //        isSuccess = scApp.VehicleBLL.noticeVhPass(waitblockZoneVH);
            //    }
            //    else
            //    {
            //        isSuccess = false;
            //    }
            //    //}
            //    return isSuccess;
            //}

            public (bool, BLOCKZONEQUEUE) NoticeBlockVhPassByEntrySecID(string entry_sec_id)
            {
                ABLOCKZONEMASTER blockMaster = getBlockZoneMasterByEntrySecID(entry_sec_id);
                if (!SCUtility.isEmpty(blockMaster.LEAVE_ADR_ID_1))
                {
                    return CheckAndNoticeBlockVhPassByAdrID(blockMaster.LEAVE_ADR_ID_1);

                }
                if (!SCUtility.isEmpty(blockMaster.LEAVE_ADR_ID_2))
                {
                    return CheckAndNoticeBlockVhPassByAdrID(blockMaster.LEAVE_ADR_ID_2);
                }
                return (false, null);
            }
            //public bool NoticeBlockVhPassByEntrySecID(string entry_sec_id)
            //{
            //    bool isSuccess = true;
            //    ABLOCKZONEMASTER blockMaster = scApp.MapBLL.getBlockZoneMasterByEntrySecID(entry_sec_id);
            //    if (!SCUtility.isEmpty(blockMaster.LEAVE_ADR_ID_1))
            //    {
            //        isSuccess &= scApp.MapBLL.CheckAndNoticeBlockVhPassByAdrID(blockMaster.LEAVE_ADR_ID_1);
            //        if (isSuccess) return isSuccess;
            //    }
            //    if (!SCUtility.isEmpty(blockMaster.LEAVE_ADR_ID_2))
            //    {
            //        isSuccess &= scApp.MapBLL.CheckAndNoticeBlockVhPassByAdrID(blockMaster.LEAVE_ADR_ID_2);
            //    }
            //    return isSuccess;
            //}

            public bool IsVHInBlockZoneByEntrySectionID(string vh_id, string entry_sec_id)
            {
                bool isInBlockZone = false;
                //DBConnection_EF con = DBConnection_EF.GetContext();
                //using (DBConnection_EF con = new DBConnection_EF())
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    isInBlockZone = blockZoneDetaiDao.IsVHInBlockZoneByEntrySectionID(con, vh_id, entry_sec_id);
                }
                return isInBlockZone;
            }

        }
    }
}
