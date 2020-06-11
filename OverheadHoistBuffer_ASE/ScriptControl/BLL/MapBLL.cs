//*********************************************************************************
//(c) Copyright 2017, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag          Description
// ------------- -------------  -------------  ------       -----------------------------
// 2020/06/09    Jason Wu       N/A            A20.06.09.0  修改getAddressID也能從vehicle取得
//**********************************************************************************

using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.DAO;
using com.mirle.ibg3k0.sc.Data.DAO.EntityFramework;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class MapBLL
    {
        private SCApplication scApp = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        RAILDao railDAO = null;
        ADDRESSDao adrDAO = null;
        PortDao portDAO = null;
        POINTDao pointDAO = null;
        PortIconDao portIconDAO = null;
        GROUPRAILSDao groupRailDAO = null;
        SectionDao sectionDAO = null;
        SegmentDao segmentDAO = null;
        VehicleDao vehicleDAO = null;
        BlockZoneMasterDao blockZoneMasterDao = null;
        BlockZoneDetailDao blockZoneDetaiDao = null;
        BlockZoneQueueDao blockZoneQueueDao = null;
        PortDefDao portDefDAO = null;
        ALINE line
        {
            get => scApp.getEQObjCacheManager().getLine();
        }
        public MapBLL()
        {

        }
        public void start(SCApplication app)
        {
            scApp = app;
            railDAO = scApp.RailDao;
            adrDAO = scApp.AddressDao;
            portDAO = scApp.PortDao;
            portIconDAO = scApp.PortIconDao;
            pointDAO = scApp.PointDao;
            groupRailDAO = scApp.GroupRailDao;
            sectionDAO = scApp.SectionDao;
            segmentDAO = scApp.SegmentDao;
            vehicleDAO = scApp.VehicleDao;
            blockZoneMasterDao = scApp.BlockZoneMasterDao;
            blockZoneDetaiDao = scApp.BolckZoneDetaiDao;
            blockZoneQueueDao = scApp.BlockZoneQueueDao;
            portDefDAO = scApp.PortDefDao;
        }
        #region Rail
        public List<ARAIL> loadAllRail()
        {
            List<ARAIL> Rails = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                Rails = railDAO.loadAll(con);
            }
            return Rails;
        }
        #endregion Rail
        #region Point
        public List<APOINT> loadAllPoint()
        {
            List<APOINT> points = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                points = pointDAO.loadAll(con);
            }
            return points;
        }

        public APOINT getPointByID(string point_id)
        {
            APOINT point = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                point = pointDAO.getByID(con, point_id);
            }
            return point;
        }
        #endregion Point
        #region GROUPRAILS
        public AGROUPRAILS getGroupRailsBySectionID(string sec_id)
        {
            AGROUPRAILS groupRails = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                groupRails = groupRailDAO.getByID(con, sec_id);
            }
            return groupRails;
        }
        public List<AGROUPRAILS> loadAllGroupRail()
        {
            List<AGROUPRAILS> lstGroupRail = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                lstGroupRail = groupRailDAO.loadAll(con);
            }
            return lstGroupRail;
        }
        public List<string> loadAllSectionID()
        {
            List<string> sec_ids = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                sec_ids = groupRailDAO.loadAllSectionID(con);
            }
            return sec_ids;
        }
        public void getFirstAndLastRailBySecID(string sec_id, out AGROUPRAILS first_rail, out AGROUPRAILS last_rail)
        {
            using (DBConnection_EF con = new DBConnection_EF())
            {
                groupRailDAO.getFirstAndLastRailBySecID(con, sec_id, out first_rail, out last_rail);
            }
        }


        #endregion GROUPRAILS

        #region Address
        public List<AADDRESS> loadAllAddress()
        {
            List<AADDRESS> adrs = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                adrs = adrDAO.loadAll(con);
            }
            return adrs;
        }
        public AADDRESS getAddressByID(string adr_id)
        {
            AADDRESS adr = null;
            //DBConnection_EF con = DBConnection_EF.GetContext();
            using (DBConnection_EF con = new DBConnection_EF())
            {
                adr = adrDAO.getByID(con, adr_id);
            }
            return adr;
        }
        public AADDRESS getAddressByPortID(string port_id)
        {
            AADDRESS adr = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                adr = adrDAO.getByPortID(con, port_id);
            }
            return adr;
        }
        public int getCount_AddressCount()
        {
            int count = 0;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                count = adrDAO.getAdrCount(con);
            }
            return count;
        }
        #endregion Address
        #region Section
        Dictionary<string, List<ASECTION>> dicNextSection = new Dictionary<string, List<ASECTION>>();

        public ASEGMENT getSegmentBySectionID(string id)
        {
            ASEGMENT segment = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                //ASECTION section = sectionDAO.getByID(con, id);
                ASECTION section = sectionDAO.getByID(id);
                if (section != null)
                    segment = segmentDAO.getByID(con, section.SEG_NUM);
            }

            return segment;
        }

        public bool updateSegStatus(string id, E_SEG_STATUS status)
        {
            ASEGMENT segment = null;
            try
            {
                using (DBConnection_EF con = new DBConnection_EF())
                {
                    segment = segmentDAO.getByID(con, id);
                    segment.STATUS = status;
                    //bool isDetached = con.Entry(segment).State == EntityState.Modified;
                    //if (isDetached)
                    segmentDAO.Update(con, segment);
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
        }

        public bool updateSecDistance(string from_id, string to_id, double distance, out ASECTION section)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    section = sectionDAO.getByFromToAdr(con, from_id, to_id);
                    section.SEC_DIS = distance;
                    section.LAST_TECH_TIME = DateTime.Now;
                    sectionDAO.update(con, section);
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                section = null;
                return false;
            }
        }
        public bool updateSecDistance(string id, int distance)
        {
            ASECTION section = null;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    section = sectionDAO.getByID(con, id);
                    section.SEC_DIS = distance;
                    //bool isDetached = con.Entry(section).State == EntityState.Modified;
                    //if (isDetached)
                    sectionDAO.update(con, section);
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
        }
        public bool resetSecTechingTime(string id)
        {
            ASECTION section = null;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    section = sectionDAO.getByID(con, id);
                    section.LAST_TECH_TIME = null;
                    //bool isDetached = con.Entry(section).State == EntityState.Modified;
                    //if (isDetached)
                    sectionDAO.update(con, section);
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
        }

        public bool updateSecDISFromOriginBySections(string id, string[] sections)
        {
            ASECTION section = null;
            try
            {
                using (DBConnection_EF con = new DBConnection_EF())
                {
                    section = sectionDAO.getByID(con, id);
                    int distance = sectionDAO.getSectionsDistance(con, sections);
                    section.DIS_FROM_ORIGIN = distance;
                    //bool isDetached = con.Entry(section).State == EntityState.Modified;
                    //if (isDetached)
                    sectionDAO.update(con, section);
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
        }




        public List<ASECTION> loadSectionBySecIDs(List<string> section_ids)
        {
            if (section_ids == null || section_ids.Count == 0)
            {
                return null;
            }
            List<ASECTION> sections = null;
            //using (DBConnection_EF con = new DBConnection_EF())
            //{
            //    sections = sectionDAO.loadSecBySecIds(con, section_ids);
            //}
            sections = sectionDAO.loadSecBySecIds(section_ids);

            return sections;
        }

        public List<ASECTION> loadSectionsByFromOrToAdr(string adr)
        {
            if (SCUtility.isEmpty(adr))
            {
                return null;
            }
            List<ASECTION> sections = null;
            sections = sectionDAO.loadByFromOrToAdr(adr);

            return sections;
        }



        public ASECTION getSectiontByID(string section_id)
        {
            ASECTION section = null;
            //using (DBConnection_EF con = new DBConnection_EF())
            //{
            //    section = sectionDAO.getByID(con, section_id);
            //}
            section = sectionDAO.getByID(section_id);

            return section;
        }
        public List<ASECTION> loadAllSection()
        {
            List<ASECTION> sections = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                sections = sectionDAO.loadAll(con);
            }
            return sections;
        }

        public List<ASECTION> loadSectionByFromAdr(string from_adr)
        {
            List<ASECTION> sections = null;
            sections = sectionDAO.loadByFromAdr(from_adr);
            return sections;
        }
        public List<ASECTION> loadSectionByFromAdrs(List<string> from_adrs)
        {
            List<ASECTION> sections = null;
            sections = sectionDAO.loadByFromAdrs(from_adrs);
            return sections;
        }
        public List<ASECTION> loadSectionByToAdrs(List<string> to_adrs)
        {
            List<ASECTION> sections = null;
            //using (DBConnection_EF con = new DBConnection_EF())
            //{
            //    sections = sectionDAO.loadByToAdrs(con, to_adrs);
            //}
            sections = sectionDAO.loadByToAdrs(to_adrs);
            return sections;
        }
        public string[] loadNextSectionIDBySectionID(String section_id)
        {
            string[] nextSections = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                nextSections = sectionDAO.loadNextSectionIDBySectionID(con, section_id);
            }
            return nextSections;

        }

        public Dictionary<string, int> loadGroupBySecAndThroughTimes()
        {
            Dictionary<string, int> secAndThroughTimes = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                secAndThroughTimes = sectionDAO.loadGroupBySecIDAndThroughTimes(con);
            }
            return secAndThroughTimes;
        }
        public string getFirstSecIDBySegmentID(string seg_id)
        {
            ASECTION sec = sectionDAO.getFirstSecBySegmentID(seg_id);
            return sec == null ? string.Empty : sec.SEC_ID.Trim();
        }
        public List<ASECTION> loadSectionsBySegmentID(string seg_num)
        {
            List<ASECTION> secs = sectionDAO.loadSectionsBySegmentID(seg_num);
            return secs;
        }

        public bool hasNotYetTeachingSection()
        {
            List<ASECTION> sections = sectionDAO.loadAllSection();
            int not_yet_thaching_count = sections.
                                         Where(sec => sec.DIRC_DRIV == 0 &&
                                                     !sec.LAST_TECH_TIME.HasValue &&
                                                      sec.SEC_TYPE != ProtocolFormat.OHTMessage.SectionType.Mtl).Count();
            return not_yet_thaching_count != 0;
        }
        #endregion Section
        #region Segment
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
        public ASEGMENT DisableSegment(string seg_num, ASEGMENT.DisableType disableType)
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
                        switch (disableType)
                        {
                            case ASEGMENT.DisableType.User:
                                seg.DISABLE_FLAG_USER = true;
                                break;
                            case ASEGMENT.DisableType.Safety:
                                seg.DISABLE_FLAG_SAFETY = true;
                                break;
                            case ASEGMENT.DisableType.HID:
                                seg.DISABLE_FLAG_HID = true;
                                break;
                            case ASEGMENT.DisableType.System:
                                seg.DISABLE_FLAG_SYSTEM = true;
                                break;

                        }
                        if (!IsSegmentEnable(seg))
                        {
                            seg.DISABLE_TIME = DateTime.Now;
                            seg.STATUS = E_SEG_STATUS.Closed;
                        }
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

        public ASEGMENT EnableSegment(string seg_num, ASEGMENT.DisableType disableType)
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
                        switch (disableType)
                        {
                            case ASEGMENT.DisableType.User:
                                seg.DISABLE_FLAG_USER = false;
                                break;
                            case ASEGMENT.DisableType.Safety:
                                seg.DISABLE_FLAG_SAFETY = false;
                                break;
                            case ASEGMENT.DisableType.HID:
                                seg.DISABLE_FLAG_HID = false;
                                break;
                            case ASEGMENT.DisableType.System:
                                seg.DISABLE_FLAG_SYSTEM = false;
                                break;
                        }
                        if (IsSegmentEnable(seg))
                        {
                            seg.DISABLE_TIME = null;
                            seg.STATUS = E_SEG_STATUS.Active;
                        }
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
        private bool IsSegmentEnable(ASEGMENT seg)
        {
            bool is_disable = seg.DISABLE_FLAG_USER || seg.DISABLE_FLAG_SAFETY || seg.DISABLE_FLAG_HID || seg.DISABLE_FLAG_SYSTEM;
            return !is_disable;
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

        public bool CheckSegmentInActiveByPortID(string port_id)
        {
            bool SegmentInActive = true;
            APORTSTATION aPORTSTATION = scApp.MapBLL.getPortByPortID(port_id);
            ASECTION aSECTION = scApp.SectionDao.loadByFromOrToAdr(aPORTSTATION.ADR_ID.Trim()).First();
            SegmentInActive = scApp.MapBLL.IsSegmentActive(aSECTION.SEG_NUM);
            return SegmentInActive;
        }

        #endregion
        #region Port
        public APORTSTATION getPortByPortID(string port_id)
        {
            APORTSTATION portTemp = null;
            //using (DBConnection_EF con = new DBConnection_EF())
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                portTemp = portDAO.getByID(con, port_id);
            }
            return portTemp;
        }

        public PortDef getPortByPortDefID(string port_id)
        {
            PortDef portTemp = null;
            //using (DBConnection_EF con = new DBConnection_EF())
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                portTemp = portDefDAO.GetPortData(con, port_id, line.LINE_ID);
            }
            return portTemp;
        }

        public APORTSTATION getPortByAdrID(string adr_id)
        {
            APORTSTATION portTemp = null;
            //using (DBConnection_EF con = new DBConnection_EF())
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                portTemp = portDAO.getByAdrID(con, adr_id);
            }
            return portTemp;
        }
        public List<APORTSTATION> loadAllPort()
        {
            List<APORTSTATION> portTemp = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                portTemp = portDAO.loadAll(con);
            }
            return portTemp;
        }
        public List<APORTSTATION> loadAllPortBySegmentID(string segment_id)
        {
            List<APORTSTATION> port_stations = null;
            List<ASECTION> sections = loadSectionsBySegmentID(segment_id);
            List<string> adrs_from = sections.Select(sec => sec.FROM_ADR_ID.Trim()).ToList();
            List<string> adrs_to = sections.Select(sec => sec.TO_ADR_ID.Trim()).ToList();
            List<string> adrs = adrs_from.Concat(adrs_to).Distinct().ToList();

            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                port_stations = portDAO.loadPortStationByAdrs(con, adrs);
            }
            return port_stations;
        }




        public bool getAddressID(string adr_port_id, out string adr)
        {
            E_VH_TYPE vh_type = E_VH_TYPE.None;
            return getAddressID(adr_port_id, out adr, out vh_type);
        }
        public bool getAddressID(string adr_port_id, out string adr, out E_VH_TYPE vh_type)
        {
            PortDef port = scApp.MapBLL.getPortByPortDefID(adr_port_id);
            vh_type = E_VH_TYPE.None;
            if (port != null)
            {
                adr = port.ADR_ID.Trim();
                return true;
            }
            else
            {
                //2020/06/02 Hsinyu Chang: not a port, try to query shelf
                ShelfDef shelf = scApp.ShelfDefBLL.loadShelfDataByID(adr_port_id);
                if (shelf != null)
                {
                    adr = shelf.ADR_ID.Trim();
                    return true;
                }
                else
                {
                    //A20.06.09.0
                    if (scApp.TransferService.isUnitType(adr_port_id, Service.UnitType.CRANE))
                    {
                        adr = scApp.VehicleService.GetVehicleDataByVehicleID(adr_port_id).CUR_ADR_ID;
                        return true;
                    }
                    else
                    {
                        adr = adr_port_id;
                        return false;
                    }
                }
            }
        }

        public bool getPortID(string adr_id, out string portid)
        {
            APORTSTATION port = scApp.MapBLL.getPortByAdrID(adr_id);
            if (port != null)
            {
                portid = port.PORT_ID.Trim();
                return true;
            }
            else
            {
                portid = adr_id;
                return false;
            }
        }


        public void updatePortStatus(string port_id, E_PORT_STATUS port_status)
        {
            APORTSTATION portTemp = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                portTemp = portDAO.getByID(con, port_id);
                //portTemp.PORT_STATUS = port_status;
                portDAO.update(con, portTemp);
            }
        }



        #endregion Port
        #region PortIcon
        public List<APORTICON> loadAllPortIcon()
        {
            List<APORTICON> portIcons = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                portIcons = portIconDAO.loadAll(con);
            }
            return portIcons;
        }
        #endregion

        #region Block Control
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
        public BLOCKZONEQUEUE getUsingBlockZoneQueueByVhID(string vh_id)
        {
            BLOCKZONEQUEUE blockZoneQueue = null;
            //DBConnection_EF con = DBConnection_EF.GetContext();
            //using (DBConnection_EF con = new DBConnection_EF())
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                blockZoneQueue = blockZoneQueueDao.getUsingBlockQueueByCarID(con, vh_id);
            }
            return blockZoneQueue;

        }
        public BLOCKZONEQUEUE getBlockQueueInRequestByCarID(string vh_id)
        {
            BLOCKZONEQUEUE blockZoneQueue = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                blockZoneQueue = blockZoneQueueDao.getBlockQueueInRequestByCarID(con, vh_id);
            }
            return blockZoneQueue;

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


        public List<ABLOCKZONEMASTER> loadAllBlockZoneMaster()
        {
            List<ABLOCKZONEMASTER> masters = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                masters = blockZoneMasterDao.loadAll(con);
            }
            return masters;
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

        public BLOCKZONEQUEUE findNextBlockZonePassVh_Branch_New(List<ABLOCKZONEMASTER> bzms)
        {
            List<BLOCKZONEQUEUE> lstBzqs = null;
            BLOCKZONEQUEUE next_pass_bzq = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                lstBzqs = blockZoneQueueDao.loadBlockQueueBySecIds(con, bzms.Select(bzm => bzm.ENTRY_SEC_ID.Trim()).ToList());
            }

            foreach (var bzq in lstBzqs)
            {
                AVEHICLE requesting_block_control_vh = scApp.VehicleBLL.cache.getVhByID(bzq.CAR_ID);
                ASEGMENT current_segment = scApp.SegmentBLL.cache.GetSegment(requesting_block_control_vh.CUR_SEG_ID);
                var check_first_vh_is_in_segment = current_segment.IsFirst(requesting_block_control_vh);
                if (!check_first_vh_is_in_segment.isFirst)
                {
                    AVEHICLE first_vh = check_first_vh_is_in_segment.firstVh;
                    string first_vh_id = first_vh == null ? string.Empty : first_vh.VEHICLE_ID;
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(MapBLL), Device: "OHTC",
                       Data: $"find block zone id:{bzq.ENTRY_SEC_ID} next pass vh," +
                             $" but vh:{bzq.CAR_ID} not first vh in segment:{requesting_block_control_vh.CUR_SEG_ID}," +
                             $" first is {first_vh_id}");
                    continue;
                }
                else
                {
                    next_pass_bzq = bzq;
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(MapBLL), Device: "OHTC",
                       Data: $"find block zone id:{bzq.ENTRY_SEC_ID} next pass vh," +
                             $" vh:{bzq.CAR_ID} is first vh in segment:{requesting_block_control_vh.CUR_SEG_ID}," +
                             $" start notify.");
                    break;
                }
            }
            return next_pass_bzq;
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
        public BLOCKZONEQUEUE findNextBlockZonePassVh_Merge_New(List<ABLOCKZONEMASTER> bzms)
        {
            List<BLOCKZONEQUEUE> lstBzqs = null;
            BLOCKZONEQUEUE next_pass_bzq = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                lstBzqs = blockZoneQueueDao.loadBlockQueueBySecIds(con, bzms.Select(bzm => bzm.ENTRY_SEC_ID.Trim()).ToList());
            }

            foreach (var bzq in lstBzqs)
            {
                AVEHICLE requesting_block_control_vh = scApp.VehicleBLL.cache.getVhByID(bzq.CAR_ID);
                ASEGMENT current_segment = scApp.SegmentBLL.cache.GetSegment(requesting_block_control_vh.CUR_SEG_ID);
                var check_first_vh_is_in_segment = current_segment.IsFirst(requesting_block_control_vh);
                if (!check_first_vh_is_in_segment.isFirst)
                {
                    AVEHICLE first_vh = check_first_vh_is_in_segment.firstVh;
                    string first_vh_id = first_vh == null ? string.Empty : first_vh.VEHICLE_ID;
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(MapBLL), Device: "OHTC",
                       Data: $"find block zone id:{bzq.ENTRY_SEC_ID} next pass vh," +
                             $" but vh:{bzq.CAR_ID} not first vh in segment:{requesting_block_control_vh.CUR_SEG_ID}," +
                             $" first is {first_vh_id}");
                    continue;
                }
                else
                {
                    next_pass_bzq = bzq;
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(MapBLL), Device: "OHTC",
                       Data: $"find block zone id:{bzq.ENTRY_SEC_ID} next pass vh," +
                             $" vh:{bzq.CAR_ID} is first vh in segment:{requesting_block_control_vh.CUR_SEG_ID}," +
                             $" start notify.");
                    break;
                }
            }
            return next_pass_bzq;
        }

        public (bool, BLOCKZONEQUEUE) CheckAndNoticeBlockVhPassByAdrID(string leave_adr_id)
        {
            List<ABLOCKZONEMASTER> lstBZM = scApp.MapBLL.loadBZMByAdrID(leave_adr_id);
            BLOCKZONEQUEUE waitblockZoneVH = null;
            if (lstBZM == null || lstBZM.Count == 0)
                return (false, null);

            //foreach (ABLOCKZONEMASTER master in lstBZM)
            //{
            //switch (master.BLOCK_ZONE_TYPE)
            switch (lstBZM[0].BLOCK_ZONE_TYPE)
            {
                case E_BLOCK_ZONE_TYPE.Branch:
                    waitblockZoneVH = scApp.MapBLL.findNextBlockZonePassVh_Branch_New(lstBZM);
                    return (waitblockZoneVH != null, waitblockZoneVH);
                case E_BLOCK_ZONE_TYPE.Merge:
                    waitblockZoneVH = scApp.MapBLL.findNextBlockZonePassVh_Merge_New(lstBZM);
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
            ABLOCKZONEMASTER blockMaster = scApp.MapBLL.getBlockZoneMasterByEntrySecID(entry_sec_id);
            if (!SCUtility.isEmpty(blockMaster.LEAVE_ADR_ID_1))
            {
                return scApp.MapBLL.CheckAndNoticeBlockVhPassByAdrID(blockMaster.LEAVE_ADR_ID_1);
            }
            if (!SCUtility.isEmpty(blockMaster.LEAVE_ADR_ID_2))
            {
                return scApp.MapBLL.CheckAndNoticeBlockVhPassByAdrID(blockMaster.LEAVE_ADR_ID_2);
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

        const string REDIS_BLOCK_CONTROL_KEY_VHID = "BLOCK_CONTROL_{0}";
        const string REDIS_BLOCK_CONTROL_VALUE_SECID_STATUS = "{0},{1}";
        TimeSpan timeOut_5min = new TimeSpan(0, 5, 0);
        public void CreatBlockControlKeyWordToRedis(string vh_id, string block_zone_id, bool can_pass, DateTime req_time)
        {
            string status = can_pass ? SCAppConstants.BlockQueueState.Blocking : SCAppConstants.BlockQueueState.Request;
            block_zone_id = SCUtility.Trim(block_zone_id);
            string set_key_word = string.Format(REDIS_BLOCK_CONTROL_KEY_VHID, vh_id);
            string set_value_word = string.Format(REDIS_BLOCK_CONTROL_VALUE_SECID_STATUS, block_zone_id, status);
            scApp.getRedisCacheManager().stringSetAsync
                (set_key_word, set_value_word);
            LogCollection.BlockControlLogger.Trace($"creat block zone to redis, vh id:{vh_id},block id:{block_zone_id},block status:{status}");
        }
        public void ChangeBlockControlStatus_Blocking(string vh_id)
        {
            string set_key_word = string.Format(REDIS_BLOCK_CONTROL_KEY_VHID, vh_id);
            var blockInfo = scApp.getRedisCacheManager().StringGet(set_key_word);
            if (blockInfo.HasValue)
            {
                string[] blockInfos = ((string)blockInfo).Split(',');
                string sec_id = blockInfos[0];
                string status = blockInfos[1];
                string set_value_word = string.Format(REDIS_BLOCK_CONTROL_VALUE_SECID_STATUS, sec_id, SCAppConstants.BlockQueueState.Blocking);
                scApp.getRedisCacheManager().stringSetAsync(set_key_word, set_value_word);
                LogCollection.BlockControlLogger.Trace($"change redis block zone status , vh id:{vh_id},block id:{sec_id},block status:{status} change to {SCAppConstants.BlockQueueState.Blocking}");
            }
            else
            {
                LogCollection.BlockControlLogger.Warn($"vh id[{vh_id}] change redis block zone status to {SCAppConstants.BlockQueueState.Blocking}, but not exists.");
            }
        }
        public void ChangeBlockControlStatus_Through(string vh_id)
        {
            AVEHICLE vh_vo = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            string set_key_word = string.Format(REDIS_BLOCK_CONTROL_KEY_VHID, vh_id);
            var blockInfo = scApp.getRedisCacheManager().StringGet(set_key_word);
            if (blockInfo.HasValue)
            {
                string[] blockInfos = ((string)blockInfo).Split(',');
                string sec_id = blockInfos[0];
                string status = blockInfos[1];
                string set_value_word = string.Format(REDIS_BLOCK_CONTROL_VALUE_SECID_STATUS, sec_id, SCAppConstants.BlockQueueState.Through);
                scApp.getRedisCacheManager().stringSetAsync(set_key_word, set_value_word);
                LogCollection.BlockControlLogger.Trace($"change redis block zone status , vh id:{vh_id},block id:{sec_id},block status:{status} change to {SCAppConstants.BlockQueueState.Through}");
            }
        }
        public void DeleteBlockControlKeyWordToRedis(string vh_id, string entry_sec_id)
        {
            string set_key_word = string.Format(REDIS_BLOCK_CONTROL_KEY_VHID, vh_id.Trim());
            string statue = string.Empty;
            if (tryGetBlock(vh_id, entry_sec_id, out statue))
            {
                scApp.getRedisCacheManager().KeyDelete(set_key_word);
                LogCollection.BlockControlLogger.Trace($"delete redis block zone , vh id:{vh_id}");
            }
            else
            {
                LogCollection.BlockControlLogger.Trace($"delete redis block zone fail, vh id:{vh_id} zone id:{entry_sec_id}");
            }
        }
        public bool HasBlockControlAskedFromRedis(string vh_id, out string block_id, out string block_status)
        {
            bool hasBlockControl = false;
            string set_key_word = string.Format(REDIS_BLOCK_CONTROL_KEY_VHID, vh_id);
            var blockInfo = scApp.getRedisCacheManager().StringGet(set_key_word);
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
            var blockInfo = scApp.getRedisCacheManager().StringGet(set_key_word);
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
            var blockInfo = scApp.getRedisCacheManager().StringGet(set_key_word);
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
            if (scApp.getRedisCacheManager().KeyExists(set_key_word))
            {
                var blockInfo = scApp.getRedisCacheManager().StringGet(set_key_word);
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
            vh_id = SCUtility.Trim(vh_id, true);
            status = string.Empty;
            string set_key_word = string.Format(REDIS_BLOCK_CONTROL_KEY_VHID, vh_id);
            LogCollection.BlockControlLogger.Trace($"delete redis block zone , vh id:{vh_id},redis key:{set_key_word} ,1");

            if (scApp.getRedisCacheManager().KeyExists(set_key_word))
            {
                LogCollection.BlockControlLogger.Trace($"delete redis block zone , vh id:{vh_id},redis key:{set_key_word} ,2");

                var blockInfo = scApp.getRedisCacheManager().StringGet(set_key_word);
                if (blockInfo.HasValue)
                {
                    LogCollection.BlockControlLogger.Trace($"delete redis block zone , vh id:{vh_id},redis key:{set_key_word} ,3");

                    string[] blockInfos = ((string)blockInfo).Split(',');
                    string current_redis_block_id = blockInfos[0];
                    LogCollection.BlockControlLogger.Trace($"delete redis block zone , vh id:{vh_id},redis key:{set_key_word} ,4,{current_redis_block_id}");
                    status = blockInfos[1];
                    if (SCUtility.isMatche(current_redis_block_id, block_zone_id))
                        isSuccess = true;
                }
            }
            return isSuccess;
        }
        //public bool tryGetUsingBlockQueue(string vh_id, out string block_zone_id)
        //{
        //    string get_key_word = string.Concat(REDIS_BLOCK_CONTROL_KEY_WORD_VHID, vh_id);

        //}
        #endregion

    }
}
