using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.DAO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class SegmentBLL
    {
        public SCApplication scApp;
        public Database dataBase { get; private set; }
        public Cache cache { get; private set; }
        public SegmentBLL()
        {
        }
        public void start(SCApplication _app)
        {
            scApp = _app;
            dataBase = new Database(scApp.SegmentDao);
            cache = new Cache(scApp.getCommObjCacheManager());
        }
        public class Database
        {
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            SegmentDao segmentDAO = null;
            public Database(SegmentDao dao)
            {
                segmentDAO = dao;
            }
            public ASEGMENT getSegmentByID(string segment_id)
            {
                ASEGMENT segment = null;
                using (DBConnection_EF con = new DBConnection_EF())
                {
                    segment = segmentDAO.getByID(con, segment_id);
                }
                return segment;
            }
            public List<ASEGMENT> loadAllSegments()
            {
                List<ASEGMENT> lstSeg = null;
                using (DBConnection_EF con = new DBConnection_EF())
                {
                    lstSeg = segmentDAO.loadAllSegments(con);
                }
                return lstSeg;
            }
            public List<string> loadAllSegmentIDs()
            {
                List<string> lstSeg = null;
                using (DBConnection_EF con = new DBConnection_EF())
                {
                    lstSeg = segmentDAO.loadAllSegmentIDs(con);
                }
                return lstSeg;
            }
            public ASEGMENT PreDisableSegment(string seg_num)
            {
                ASEGMENT seg = null;
                try
                {
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        seg = segmentDAO.getByID(con, seg_num);
                        if (seg != null)
                        {
                            seg.PRE_DISABLE_FLAG = true;
                            seg.PRE_DISABLE_TIME = DateTime.Now;
                            seg.DISABLE_TIME = null;
                        }
                        segmentDAO.Update(con, seg);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
                return seg;
            }
            public ASEGMENT DisableSegment(string seg_num)
            {
                ASEGMENT seg = null;
                try
                {
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        seg = segmentDAO.getByID(con, seg_num);
                        if (seg != null)
                        {
                            seg.PRE_DISABLE_FLAG = false;
                            seg.PRE_DISABLE_TIME = null;
                            seg.DISABLE_TIME = DateTime.Now;
                            seg.STATUS = E_SEG_STATUS.Closed;
                            segmentDAO.Update(con, seg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
                return seg;
            }
            public ASEGMENT EnableSegment(string seg_num)
            {
                ASEGMENT seg = null;
                try
                {
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        seg = segmentDAO.getByID(con, seg_num);
                        if (seg != null)
                        {
                            seg.PRE_DISABLE_FLAG = false;
                            seg.PRE_DISABLE_TIME = null;
                            seg.DISABLE_TIME = null;
                            seg.STATUS = E_SEG_STATUS.Active;
                        }
                        segmentDAO.Update(con, seg);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
                return seg;
            }
            public List<ASEGMENT> loadPreDisableSegment()
            {
                List<ASEGMENT> preDisableSegments = null;
                try
                {
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        preDisableSegments = segmentDAO.loadPreDisableSegment(con);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
                return preDisableSegments;
            }

            public bool IsSegmentActive(string seg_num)
            {
                bool isActive = false;
                try
                {
                    ASEGMENT seg = null;
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        seg = segmentDAO.getByID(con, seg_num);
                    }
                    if (seg != null)
                    {
                        isActive = !seg.PRE_DISABLE_FLAG && seg.STATUS == E_SEG_STATUS.Active;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
                return isActive;
            }

            public List<string> loadNonActiveSegmentNum()
            {
                List<string> non_active_seg_nums = null;
                try
                {
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        non_active_seg_nums = segmentDAO.loadAllNonActiveSegmentNums(con);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
                return non_active_seg_nums;
            }
        }
        public class Cache
        {
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            CommObjCacheManager CommObjCacheManager = null;
            public Cache(CommObjCacheManager commObjCacheManager)
            {
                CommObjCacheManager = commObjCacheManager;
            }
            public List<ASEGMENT> GetSegments()
            {
                return CommObjCacheManager.getSegments();
            }
            public ASEGMENT GetSegment(string id)
            {
                if (SCUtility.isEmpty(id)) return null;
                return CommObjCacheManager.getSegments().
                    Where(seg => seg.SEG_NUM.Trim() == id.Trim()).
                    SingleOrDefault();
            }

            public bool IsSegmentActive(string seg_num)
            {
                bool isActive = false;
                ASEGMENT seg = GetSegment(seg_num);
                if (seg != null)
                {
                    isActive = !seg.PRE_DISABLE_FLAG && seg.STATUS == E_SEG_STATUS.Active;
                }
                return isActive;
            }
        }
    }
}
