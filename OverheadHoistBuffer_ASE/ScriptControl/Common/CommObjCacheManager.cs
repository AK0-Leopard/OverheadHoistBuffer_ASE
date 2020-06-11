//*********************************************************************************
//      EQObjCacheManager.cs
//*********************************************************************************
// File Name: EQObjCacheManager.cs
// Description: Equipment Cache Manager
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.ConfigHandler;
using com.mirle.ibg3k0.bcf.Data.FlowRule;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;
using com.mirle.ibg3k0.sc.ConfigHandler;
using com.mirle.ibg3k0.sc.Data;

namespace com.mirle.ibg3k0.sc.Common
{

    public class CommObjCacheManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static CommObjCacheManager instance = null;
        private static Object _lock = new Object();
        private SCApplication scApp = null;
        //Cache Object
        //Section
        private List<ASECTION> Sections;
        //Segment
        private List<ASEGMENT> Segments;
        private List<PortDef> PortDefs;
        private List<ReserveEnhanceInfo> ReserveEnhanceInfos;
        private CommonInfo CommonInfo;

        private CommObjCacheManager() { }
        public static CommObjCacheManager getInstance()
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = new CommObjCacheManager();
                }
                return instance;
            }
        }


        public void setContext()
        {
        }

        public void start(SCApplication _app)
        {
            scApp = _app;

            Segments = scApp.MapBLL.loadAllSegments();
            Sections = scApp.MapBLL.loadAllSection();
            ReserveEnhanceInfos = scApp.ReserveEnhanceInfoDao.loadReserveInfos(scApp);
            foreach (ASEGMENT segment in Segments)
            {
                segment.SetSectionList(scApp.SectionBLL);
            }

            CommonInfo = new CommonInfo();
        }


        public void setPortDefsInfo()
        {
            PortDefs = scApp.PortDefBLL.GetOHB_CVPortData(scApp.getEQObjCacheManager().getLine().LINE_ID);
        }


        public void stop()
        {
            clearCache();
        }


        private void clearCache()
        {
            Sections.Clear();
        }


        private void removeFromDB()
        {
            //not implement yet.
        }

        #region 取得各種EQ Object的方法
        //Section
        public ASECTION getSection(string sec_id)
        {
            return Sections.Where(z => z.SEC_ID.Trim() == sec_id.Trim()).FirstOrDefault();
        }
        public ASECTION getSection(string adr1, string adr2)
        {
            return Sections.Where(s => (s.FROM_ADR_ID.Trim() == adr1.Trim() && s.TO_ADR_ID.Trim() == adr2.Trim())
                                    || (s.FROM_ADR_ID.Trim() == adr2.Trim() && s.TO_ADR_ID.Trim() == adr1.Trim())).FirstOrDefault();
        }
        public List<ASECTION> getSections()
        {
            return Sections;
        }
        //Segment
        public List<ASEGMENT> getSegments()
        {
            return Segments;
        }
        public List<PortDef> getPortDefs()
        {
            return PortDefs;
        }
        public List<ReserveEnhanceInfo> getReserveEnhanceInfos()
        {
            return ReserveEnhanceInfos;
        }

        #endregion


        private void setValueToPropety<T>(ref T sourceObj, ref T destinationObj)
        {
            BCFUtility.setValueToPropety(ref sourceObj, ref destinationObj);
        }

        #region 將最新物件資料，放置入Cache的方法
        //NotImplemented
        #endregion


        #region 從DB取得最新EQ Object，並更新Cache
        //NotImplemented
        #endregion



    }
}
