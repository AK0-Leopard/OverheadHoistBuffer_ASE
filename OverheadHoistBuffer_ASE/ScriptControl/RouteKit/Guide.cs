//*********************************************************************************
//      MESDefaultMapAction.cs
//*********************************************************************************
// File Name: MESDefaultMapAction.cs
// Description: Type 1 Function
//
//(c) Copyright 2018, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2018/03/07    KevinWei       N/A            A0.01   增加尋找路徑時，
//                                                     要不要參考該Segment的Status的Flag。
//**********************************************************************************

using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.DAO;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace RouteKit
{
    public class Guide
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static List<Section> SectionList = new List<Section>();
        static List<Segment> SegmentList = new List<Segment>();
        static NameValueCollection SectionIndex = new NameValueCollection();
        static NameValueCollection SectionListIndex = new NameValueCollection();
        static NameValueCollection SegmentListIndex = new NameValueCollection();
        static NameValueCollection keepAddressArrayIndex = new NameValueCollection();
        static NameValueCollection addressMappingSegment = new NameValueCollection();
        object searchSafe_lockObj = new object();
        private SCApplication scApp = null;
        SectionDao dao = null;
        SegmentDao seg_dao = null;
        MapBLL bll = null;
        //DBConnection_EF con = null;

        public Guide()
        {

        }
        public void start(SCApplication scApp)
        {
            this.scApp = scApp;
            dao = scApp.SectionDao;
            seg_dao = scApp.SegmentDao;
            bll = scApp.MapBLL;
            //con = new DBConnection_EF();
        }


        public bool ImportMap()
        {
            using (DBConnection_EF con = new DBConnection_EF())
            {
                bool flag = true;
                try
                {
                    int i = 0;
                    HashSet<string> fromAddHasSet = new HashSet<string>();
                    List<ASECTION> section = dao.loadAll(con);
                    foreach (ASECTION sec in section)
                    {
                        Section s = new Section(sec.SEC_ID.Trim(), sec.FROM_ADR_ID.Trim(), sec.TO_ADR_ID.Trim(), sec.SEC_DIS, sec.SEG_NUM.Trim());
                        SectionList.Add(s);
                        SectionListIndex.Add(sec.SEC_ID.Trim(), i.ToString());

                        var segment = bll.getSegmentBySectionID(sec.SEC_ID.Trim());
                        if (!fromAddHasSet.Add(sec.FROM_ADR_ID))
                        {
                            if (segment.SEG_TYPE.ToString() == "Station")
                                SectionIndex.Add("none", "none");
                        }
                        else
                        {
                            SectionIndex.Add(sec.FROM_ADR_ID.Trim(), i.ToString());
                        }
                        i++;
                    }
                    foreach (Section sec in SectionList)
                    {
                        var nextSection = dao.loadNextSectionIDBySectionID(con, sec.SectionCode).ToArray();
                        //var nextSection = dao.loadNextSectionIDBySectionID(con, "1229912199").ToArray(); //測試用
                        foreach (string strSection in nextSection)
                        {
                            int index = Convert.ToInt16(SectionListIndex[strSection.Trim()]);
                            SectionDoubleLink(sec, SectionList[index]);
                        }
                    }

                    var allAddress = dao.loadAllAddress(con);
                    foreach (string address in allAddress)
                    {
                        var segmentArray = dao.loadSegmentNumByAdr(con, address.Trim());
                        string strSeg = string.Empty;
                        foreach (string seg in segmentArray)
                            strSeg += "," + seg.Trim();
                        addressMappingSegment.Add(address.Trim(), strSeg.TrimStart(','));
                    }

                    List<List<string>> keepAddressArray = new List<List<string>>();
                    List<string> all_adr_in_seg = null;
                    string strAddress = string.Empty;
                    int arrayIndex = 0;
                    List<string> segments = dao.loadAllSegmentNum(con);
                    //all_adr_in_seg = dao.loadAllAdrBySegmentNum(con, "26");  //測試用
                    foreach (string seg in segments)
                    {
                        all_adr_in_seg = dao.loadAllAdrBySegmentNum(con, seg.Trim());
                        List<string> addressArray = new List<string>();
                        foreach (string adr in all_adr_in_seg)
                            addressArray.Add(adr.Trim());
                        keepAddressArray.Add(addressArray);
                        keepAddressArrayIndex.Add(seg.Trim(), arrayIndex.ToString());
                        arrayIndex++;
                    }

                    var segArr = seg_dao.loadAllSegments(con);
                    int indexCount = 0;
                    foreach (ASEGMENT seg in segArr)
                    {
                        string segType = string.Empty;
                        switch (Convert.ToInt16(seg.SEG_TYPE))
                        {
                            case 1:
                                segType = "Common";
                                break;
                            case 3:
                                if (seg.SPECIAL_MARK == 1)
                                    segType = "Station";
                                else if (seg.SPECIAL_MARK == 2)
                                    segType = "Rotating";
                                break;
                            case 4:
                                segType = "Maintenance";
                                break;
                        }
                        var secArr = dao.loadBySegmentNum(con, seg.SEG_NUM.Trim());
                        double segDistance = 0;
                        foreach (ASECTION sec in secArr)
                            segDistance += Convert.ToDouble(sec.SEC_DIS);
                        int index_AddressArray = Convert.ToInt16(keepAddressArrayIndex[seg.SEG_NUM.Trim()]);
                        Segment s = new Segment(seg.SEG_NUM.Trim(), segType, segDistance, keepAddressArray[index_AddressArray].ToArray());
                        SegmentList.Add(s);
                        SegmentListIndex.Add(seg.SEG_NUM.Trim(), indexCount.ToString());
                        indexCount++;
                    }

                    foreach (Segment seg in SegmentList)
                    {
                        var nextSeg = dao.loadNextSegmentNumBySegmentNum(con, seg.SegmentCode.Trim());
                        foreach (string strSeg in nextSeg)
                        {
                            int segIndex = Convert.ToInt16(SegmentListIndex[strSeg.Trim()]);
                            SegmentDoubleLink(seg, SegmentList[segIndex]);
                        }
                    }

                    foreach (Segment seg in SegmentList)
                    {
                        #region Initiation(Default Segment Status)
                        //switch (seg.SegmentType)
                        //{
                        //    case "Common":
                        //        seg.Status = "Active";
                        //        bll.updateSegStatus(seg.SegmentCode, E_SEG_STATUS.Active);
                        //        break;
                        //    case "Station":
                        //        seg.Status = "Active";  //取消關閉 Station_Type Segment的連動效果，Station的預設狀態為Active
                        //        bll.updateSegStatus(seg.SegmentCode, E_SEG_STATUS.Inactive);
                        //        break;
                        //    case "Rotating":
                        //        seg.Status = "Active";
                        //        bll.updateSegStatus(seg.SegmentCode, E_SEG_STATUS.Active);
                        //        break;
                        //    case "Maintenance":
                        //        seg.Status = "Active";  //2017/5/9，增加往下游找section功能後，Maintenance的預設狀態為Active
                        //        bll.updateSegStatus(seg.SegmentCode, E_SEG_STATUS.Inactive);
                        //        break;
                        //}
                        #endregion
                        var asegment = seg_dao.getByID(con, seg.SegmentCode);
                        switch (Convert.ToInt16(asegment.STATUS))
                        {
                            case 1:
                                seg.Status = "Active";
                                break;
                            case 2:
                                seg.Status = "Inactive";
                                break;
                            case 3:
                                seg.Status = "Closed";
                                break;
                        }
                        var asection = dao.getSectionBySegmentID(con, seg.SegmentCode);
                        foreach (ASECTION sec in asection)
                        {
                            int index = Convert.ToInt16(SectionListIndex[sec.SEC_ID.Trim()]);
                            SectionList[index].Status = seg.Status;
                        }
                    }

                }
                catch (Exception ex)
                {
                    flag = false;
                    logger.Error(ex, "Exception");
                }
                return flag;
            }
        }

        #region Import From Catch
        public bool ImportMap(List<ASECTION> sections, List<ASEGMENT> segments)
        {
            bool flag = true;
            try
            {
                int i = 0;
                HashSet<string> fromAddHasSet = new HashSet<string>();
                // List<ASECTION> section = dao.loadAll(con);
                foreach (ASECTION sec in sections)
                {
                    Section s = new Section(sec.SEC_ID.Trim(), sec.FROM_ADR_ID.Trim(), sec.TO_ADR_ID.Trim(), sec.SEC_DIS, sec.SEG_NUM.Trim());
                    SectionList.Add(s);
                    SectionListIndex.Add(sec.SEC_ID.Trim(), i.ToString());

                    var segment = getSegmentBySectionID(sections, segments, sec.SEC_ID.Trim());
                    if (!fromAddHasSet.Add(sec.FROM_ADR_ID))
                    {
                        if (segment.SEG_TYPE.ToString() == "Station")
                            SectionIndex.Add("none", "none");
                    }
                    else
                    {
                        SectionIndex.Add(sec.FROM_ADR_ID.Trim(), i.ToString());
                    }
                    i++;
                }
                foreach (Section sec in SectionList)
                {
                    var nextSection = loadNextSectionIDBySectionID(sections, sec.SectionCode).ToArray();
                    //var nextSection = dao.loadNextSectionIDBySectionID(con, "1229912199").ToArray(); //測試用
                    foreach (string strSection in nextSection)
                    {
                        int index = Convert.ToInt16(SectionListIndex[strSection.Trim()]);
                        SectionDoubleLink(sec, SectionList[index]);
                    }
                }

                var allAddress = loadAllAddress(sections);
                foreach (string address in allAddress)
                {
                    var segmentArray = loadSegmentNumByAdr(sections, address.Trim());
                    string strSeg = string.Empty;
                    foreach (string seg in segmentArray)
                        strSeg += "," + seg.Trim();
                    addressMappingSegment.Add(address.Trim(), strSeg.TrimStart(','));
                }

                List<List<string>> keepAddressArray = new List<List<string>>();
                List<string> all_adr_in_seg = null;
                string strAddress = string.Empty;
                int arrayIndex = 0;
                List<string> segment_muns = segments.Select(seg => seg.SEG_NUM).ToList();
                //all_adr_in_seg = dao.loadAllAdrBySegmentNum(con, "26");  //測試用
                foreach (string seg in segment_muns)
                {
                    all_adr_in_seg = loadAllAdrBySegmentNum(sections, seg.Trim());
                    List<string> addressArray = new List<string>();
                    foreach (string adr in all_adr_in_seg)
                        addressArray.Add(adr.Trim());
                    keepAddressArray.Add(addressArray);
                    keepAddressArrayIndex.Add(seg.Trim(), arrayIndex.ToString());
                    arrayIndex++;
                }

                //var segArr = seg_dao.loadAllSegments(con);
                int indexCount = 0;
                foreach (ASEGMENT seg in segments)
                {
                    string segType = string.Empty;
                    switch (Convert.ToInt16(seg.SEG_TYPE))
                    {
                        case 1:
                            segType = "Common";
                            break;
                        case 3:
                            if (seg.SPECIAL_MARK == 1)
                                segType = "Station";
                            else if (seg.SPECIAL_MARK == 2)
                                segType = "Rotating";
                            break;
                        case 4:
                            segType = "Maintenance";
                            break;
                    }
                    var secArr = loadBySegmentNum(sections, seg.SEG_NUM.Trim());
                    double segDistance = 0;
                    foreach (ASECTION sec in secArr)
                        segDistance += Convert.ToDouble(sec.SEC_DIS);
                    int index_AddressArray = Convert.ToInt16(keepAddressArrayIndex[seg.SEG_NUM.Trim()]);
                    Segment s = new Segment(seg.SEG_NUM.Trim(), segType, segDistance, keepAddressArray[index_AddressArray].ToArray());
                    SegmentList.Add(s);
                    SegmentListIndex.Add(seg.SEG_NUM.Trim(), indexCount.ToString());
                    indexCount++;
                }

                foreach (Segment seg in SegmentList)
                {
                    var nextSeg = loadNextSegmentNumBySegmentNum(sections, seg.SegmentCode.Trim());
                    foreach (string strSeg in nextSeg)
                    {
                        int segIndex = Convert.ToInt16(SegmentListIndex[strSeg.Trim()]);
                        SegmentDoubleLink(seg, SegmentList[segIndex]);
                    }
                }

                foreach (Segment seg in SegmentList)
                {
                    #region Initiation(Default Segment Status)
                    //switch (seg.SegmentType)
                    //{
                    //    case "Common":
                    //        seg.Status = "Active";
                    //        bll.updateSegStatus(seg.SegmentCode, E_SEG_STATUS.Active);
                    //        break;
                    //    case "Station":
                    //        seg.Status = "Active";  //取消關閉 Station_Type Segment的連動效果，Station的預設狀態為Active
                    //        bll.updateSegStatus(seg.SegmentCode, E_SEG_STATUS.Inactive);
                    //        break;
                    //    case "Rotating":
                    //        seg.Status = "Active";
                    //        bll.updateSegStatus(seg.SegmentCode, E_SEG_STATUS.Active);
                    //        break;
                    //    case "Maintenance":
                    //        seg.Status = "Active";  //2017/5/9，增加往下游找section功能後，Maintenance的預設狀態為Active
                    //        bll.updateSegStatus(seg.SegmentCode, E_SEG_STATUS.Inactive);
                    //        break;
                    //}
                    #endregion
                    var asegment = getSegmentByNum(segments, seg.SegmentCode);
                    switch (Convert.ToInt16(asegment.STATUS))
                    {
                        case 1:
                            seg.Status = "Active";
                            break;
                        case 2:
                            seg.Status = "Inactive";
                            break;
                        case 3:
                            seg.Status = "Closed";
                            break;
                    }
                    var asection = getSectionBySegmentID(sections, seg.SegmentCode);
                    foreach (ASECTION sec in asection)
                    {
                        int index = Convert.ToInt16(SectionListIndex[sec.SEC_ID.Trim()]);
                        SectionList[index].Status = seg.Status;
                    }
                }

            }
            catch (Exception ex)
            {
                flag = false;
                logger.Error(ex, "Exception");
            }
            return flag;
        }
        public List<string> loadAllAddress(List<ASECTION> sections)
        {
            List<string> addressList = new List<string>();
            var query = from s in sections
                        select s.FROM_ADR_ID;
            var _query = from s in sections
                         select s.TO_ADR_ID;
            addressList.AddRange(query.ToList());
            addressList.AddRange(_query.ToList());
            return addressList.Distinct().ToList();
        }
        public ASEGMENT getSegmentByNum(List<ASEGMENT> segments, string num)
        {
            if (string.IsNullOrWhiteSpace(num))
                return null;
            var query = from s in segments
                        where s.SEG_NUM.Trim() == num.Trim()
                        orderby s.SEG_NUM
                        select s;
            return query.FirstOrDefault();
        }
        public ASECTION getSectionByID(List<ASECTION> sections, String section_id)
        {
            var query = from s in sections
                        where s.SEC_ID.Trim() == section_id.Trim()
                        orderby s.SEC_ID
                        select s;
            return query.FirstOrDefault();
        }
        public ASEGMENT getSegmentBySectionID(List<ASECTION> sections, List<ASEGMENT> segments, string id)
        {
            ASEGMENT segment = null;
            ASECTION section = getSectionByID(sections, id);
            if (section != null)
                segment = getSegmentByNum(segments, section.SEG_NUM);
            return segment;
        }

        public string[] loadNextSectionIDBySectionID(List<ASECTION> sections, String section_id)
        {
            if (string.IsNullOrWhiteSpace(section_id))
                return null;
            string[] nextSection_ids = null;
            ASECTION sec = null;
            sec = getSectionByID(sections, section_id);
            if (sec != null)
            {
                List<ASECTION> nextSections = loadByFromAdr(sections, sec.TO_ADR_ID);
                if (nextSections != null)
                {
                    foreach (ASECTION next_sec in nextSections.ToList())
                    {
                        if ((sec.SEG_NUM.Trim() == next_sec.SEG_NUM.Trim()) ||
                            sec.CHG_SEG_NUM_1.Trim() == next_sec.SEG_NUM.Trim() ||
                            sec.CHG_SEG_NUM_2.Trim() == next_sec.SEG_NUM.Trim())
                        {
                            continue;
                        }
                        nextSections.Remove(next_sec);
                    }
                    nextSection_ids = nextSections.Select(s => s.SEC_ID).ToArray();
                }
            }
            return nextSection_ids;
        }
        public List<ASECTION> loadByFromAdr(List<ASECTION> sections, string from_adr)
        {
            if (string.IsNullOrWhiteSpace(from_adr))
                return null;
            var query = from section in sections
                        where section.FROM_ADR_ID.Trim() == from_adr.Trim()
                        orderby section.SEC_ID
                        select section;
            return query.ToList();
        }
        public List<ASECTION> loadByToAdr(List<ASECTION> sections, string to_adr)
        {
            if (string.IsNullOrWhiteSpace(to_adr))
                return null;
            var query = from section in sections
                        where section.TO_ADR_ID.Trim() == to_adr.Trim()
                        orderby section.SEC_ID
                        select section;
            return query.ToList();
        }
        public List<string> loadSegmentNumByAdr(List<ASECTION> sections, string adr)
        {
            List<string> segmentNums = new List<string>();
            List<ASECTION> lst_from_section, lst_to_section;
            loadByFromOrToAdr(sections, adr, out lst_from_section, out lst_to_section);
            if (lst_from_section != null)
            {
                segmentNums.AddRange(lst_from_section.Select(section => section.SEG_NUM).Distinct().ToList());
            }
            if (lst_to_section != null)
            {
                segmentNums.AddRange(lst_to_section.Select(section => section.SEG_NUM).Distinct().ToList());
            }
            return segmentNums;
        }

        public void loadByFromOrToAdr(List<ASECTION> sections, string adr, out List<ASECTION> lst_from_section, out List<ASECTION> lst_to_section)
        {
            lst_from_section = loadByFromAdr(sections, adr);
            lst_to_section = loadByToAdr(sections, adr);
        }
        public List<string> loadAllAdrBySegmentNum(List<ASECTION> sections, String segment_num)
        {
            if (string.IsNullOrWhiteSpace(segment_num))
                return null;
            List<ASECTION> sectionsOnSegment = loadBySegmentNum(sections, segment_num);
            //HashSet<string> hsaddress = new HashSet<string>();
            List<string> lstaddress = new List<string>();
            foreach (ASECTION sec in sectionsOnSegment)
            {
                lstaddress.Add(sec.FROM_ADR_ID);
                lstaddress.Add(sec.TO_ADR_ID);
            }
            //return lstaddress.OrderBy(s => s).ToList();
            return lstaddress.Distinct().ToList();
        }

        public List<ASECTION> loadBySegmentNum(List<ASECTION> sections, String segment_num)
        {
            if (string.IsNullOrWhiteSpace(segment_num))
                return null;
            var query = from s in sections
                        where s.SEG_NUM.Trim() == segment_num.Trim()
                        orderby s.SEG_ORDER_NUM
                        select s;
            return query.ToList();
        }

        public List<String> loadNextSegmentNumBySegmentNum(List<ASECTION> sections, String segment_num)
        {
            if (string.IsNullOrWhiteSpace(segment_num))
                return null;
            List<String> nextSegment_Nums = new List<string>();
            ASECTION lastsec = null;
            lastsec = loadBySegmentNum(sections, segment_num).LastOrDefault();
            if (lastsec != null)
            {
                List<ASECTION> nextSections = loadByFromAdr(sections, lastsec.TO_ADR_ID);
                List<ASECTION> branchSections = loadByFromAdr(sections, lastsec.FROM_ADR_ID);
                //1.移除掉自己的Section
                branchSections.Remove(lastsec);

                //2.過濾掉並非為該Segment 第1條的Section
                List<ASECTION> branchSectionsTemp = branchSections.ToList();
                foreach (ASECTION sec in branchSectionsTemp)
                {
                    if (!isFirstSectionInSegment(sections, sec))
                    {
                        branchSections.Remove(sec);
                    }
                }

                if (nextSections != null && nextSections.Count > 0)
                    nextSegment_Nums.AddRange(nextSections.Select(s => s.SEG_NUM).ToList());
                if (branchSections != null && branchSections.Count > 0)
                    nextSegment_Nums.AddRange(branchSections.Select(s => s.SEG_NUM).ToList());
            }
            return nextSegment_Nums;
        }


        public Boolean isFirstSectionInSegment(List<ASECTION> sections, ASECTION section)
        {
            List<ASECTION> lstSec = loadBySegmentNum(sections, section.SEG_NUM);
            if (lstSec == null || lstSec.Count() <= 0)
                return false;
            ASECTION FirstSection = lstSec.First();
            return FirstSection.SEC_ID == section.SEC_ID;
        }

        public List<ASECTION> getSectionBySegmentID(List<ASECTION> sections, String segment_id)
        {
            List<ASECTION> section = null;
            if (string.IsNullOrWhiteSpace(segment_id))
                return null;
            var query = from s in sections
                        where s.SEG_NUM.Trim() == segment_id.Trim()
                        select s;
            section = query.ToList();
            return section;
        }
        #endregion Import From Catch



        public int[] getCatchSectionCount()
        {
            int[] section_count = new int[] { SectionList.Count, SectionListIndex.Count };
            return section_count;
        }

        List<string> routeList = new List<string>();
        List<double> distanceList = new List<double>();
        List<int> routeDistanceAscIndexList = new List<int>();
        string returnMinRoute;
        string returnAllRoute;
        public string[] DownstreamSearchRoute(string fromAdr, string toAdr, int flag)
        {
            returnMinRoute = string.Empty;
            returnAllRoute = string.Empty;
            int finalFromSegIndex;
            string[] returnArray = new string[2];
            string[] segArray = new string[2];
            try
            {
                segArray = ReturnSegmentPlusIndex(fromAdr, toAdr);
                finalFromSegIndex = JudgeFromSeg(segArray[0], segArray[1]);
                var finalToSegIndex = ReturnToSegIndex(segArray[1]);
                if (fromAdr != toAdr)
                    DownstreamSearchRoute(SegmentList[finalFromSegIndex], finalToSegIndex, fromAdr, toAdr);
                else
                    DownstreamSearchRoute(SegmentList[finalFromSegIndex], SegmentList[finalFromSegIndex]);

                if (distanceList.Count() > 0)
                {
                    int minDistanceIndex = distanceList.IndexOf(distanceList.Min());
                    returnMinRoute = routeList[minDistanceIndex];
                }

                for (int i = 0; i < distanceList.Count(); i++)
                {
                    int minDistanceIndex = distanceList.IndexOf(distanceList.Min());
                    routeDistanceAscIndexList.Add(minDistanceIndex);
                    distanceList[minDistanceIndex] = distanceList[minDistanceIndex] + 999999999999999;
                }
                if (distanceList.Count() > 0)
                    //returnMinRoute = routeList[routeDistanceAscIndexList[0]];
                    returnMinRoute = routeList[routeDistanceAscIndexList[0]] + "=" + (distanceList[routeDistanceAscIndexList[0]] - 999999999999999); ;

                for (int i = 0; i < routeDistanceAscIndexList.Count(); i++)
                    returnAllRoute += ";" + routeList[routeDistanceAscIndexList[i]] + "=" + (distanceList[routeDistanceAscIndexList[i]] - 999999999999999);
                switch (flag)
                {
                    case 0:
                        returnArray[1] = string.Empty;
                        break;
                    case 1:
                        returnArray[1] = returnAllRoute.TrimStart(';');
                        break;
                }
                returnArray[0] = returnMinRoute;
                routeList.Clear();
                distanceList.Clear();
                routeDistanceAscIndexList.Clear();
                routeArray.Clear();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            return returnArray;
        }


        LinkedList<Segment> visited = new LinkedList<Segment>();
        List<Segment[]> routes = new List<Segment[]>();
        List<Segment[]> routeArray = new List<Segment[]>();
        HashSet<Segment> visitedSeg = new HashSet<Segment>();
        private void DownstreamSearchRoute(Segment fromSeg, List<int> toSegIndex, string fromAdr, string toAdr)
        {
            fromAdr = fromAdr.ToUpper();
            toAdr = toAdr.ToUpper();
            toSegIndex = toSegIndex.Distinct().ToList();
            try
            {
                foreach (int index in toSegIndex)
                {
                    visitedSeg.Clear();
                    if (SegmentList[index].Status != "Closed")
                    {
                        int sAdrIndex = Array.IndexOf(SegmentList[index].Address, fromAdr);
                        int eAdrIndex = Array.IndexOf(SegmentList[index].Address, toAdr);
                        if (fromSeg == SegmentList[index] && sAdrIndex < eAdrIndex)
                        {
                            returnAllRoute = SegmentList[index].SegmentCode;
                            returnMinRoute = SegmentList[index].SegmentCode;
                            break;
                        }
                        else
                        {
                            visited.AddLast(fromSeg);
                            Search(SegmentList[index], visited);
                            visited.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }


        private void DownstreamSearchRoute(Segment fromSeg, Segment toSeg)
        {
            try
            {
                visitedSeg.Clear();
                if (toSeg.Status != "Closed")
                {
                    visited.AddLast(fromSeg);
                    Search(toSeg, visited);
                    visited.Clear();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }


        private void Search(Segment endNode, LinkedList<Segment> visited)
        {
            try
            {
                var nodes = visited.Last.Value.DownstreamSegments;
                foreach (var seg in nodes)
                {
                    if (seg == endNode)
                    {
                        if (!visitedSeg.Add(seg))
                            continue;
                        visited.AddLast(seg);
                        routeArray.Add(visited.ToArray());
                        routes.Add(visited.ToArray());
                        foreach (var array in routes)
                        {
                            string strRoute = string.Empty;
                            double distance = 0;
                            foreach (Segment segCode in array)
                            {
                                strRoute += "," + segCode;
                                distance += segCode.Distance;
                            }
                            strRoute = strRoute.TrimStart(',');
                            distanceList.Add(distance);
                            routeList.Add(strRoute);
                        }
                        visited.RemoveLast();
                        visitedSeg.Remove(seg);
                    }
                }
                foreach (var seg in nodes)
                {
                    if (seg.Status == "Active")
                    {
                        if (!visitedSeg.Add(seg))
                            continue;
                        visited.AddLast(seg);
                        Search(endNode, visited);
                        visited.RemoveLast();
                        visitedSeg.Remove(seg);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            routes.Clear();
        }


        private string[] ReturnSegmentPlusIndex(string fromAdr, string toAdr)
        {
            fromAdr = fromAdr.ToUpper();
            toAdr = toAdr.ToUpper();
            string[] segArray = new string[2];
            segArray[0] = addressMappingSegment[fromAdr];
            segArray[1] = addressMappingSegment[toAdr];
            return segArray;
        }

        int finalStartSegIndex;
        private int JudgeFromSeg(string fromSeg, string toSeg)
        {
            string finalStartSeg = string.Empty;
            string[] fSeg = fromSeg.Split(',').ToArray();
            string[] tSeg = toSeg.Split(',').ToArray();
            fSeg = fSeg.Distinct().ToArray();
            tSeg = tSeg.Distinct().ToArray();
            try
            {
                var intersectArray = fSeg.Intersect(tSeg).ToArray();
                if (intersectArray.Count() == 1)
                    finalStartSeg = intersectArray[0];
                else if (intersectArray.Count() > 1)
                {
                    int index;
                    foreach (string seg in intersectArray)
                    {
                        index = Convert.ToInt16(keepAddressArrayIndex[seg]);
                        if (SegmentList[index].SegmentType == "Common")
                            finalStartSeg = seg;
                    }
                }
                else if (intersectArray.Count() == 0)  //From、ToAddress路段不相鄰
                {
                    if (fSeg.Count() > 1)
                    {
                        int sIndex;
                        List<Segment> sCommonSegList = new List<Segment>();
                        foreach (string seg in fSeg)
                        {
                            sIndex = Convert.ToInt16(keepAddressArrayIndex[seg]);
                            if (SegmentList[sIndex].SegmentType == "Common")
                                sCommonSegList.Add(SegmentList[sIndex]);
                        }
                        List<Segment> nextSeg = new List<Segment>();
                        foreach (Segment seg in sCommonSegList)
                        {
                            var temp = seg.DownstreamSegments.ToArray();
                            foreach (var t in temp)
                                if (t.SegmentType == "Common")
                                    nextSeg.Add(t);
                        }
                        if (sCommonSegList.Count() == 2)  //一般路段的分歧點
                        {
                            foreach (Segment next_Seg in nextSeg)
                                sCommonSegList = sCommonSegList.Where(val => val != next_Seg).ToList();
                            finalStartSeg = sCommonSegList[0].SegmentCode;
                        }
                        else if (sCommonSegList.Count() == 1)
                        {
                            finalStartSeg = sCommonSegList[0].SegmentCode;
                        }
                        else
                        {
                            foreach (Segment nSeg in nextSeg)
                                if (sCommonSegList.Contains(nSeg))
                                    finalStartSeg = nSeg.SegmentCode;
                        }
                    }
                    else if (fSeg.Count() == 1)
                    {
                        finalStartSeg = fSeg[0];
                    }
                }
                finalStartSegIndex = Convert.ToInt16(keepAddressArrayIndex[finalStartSeg]);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            return finalStartSegIndex;

        }

        List<string> routeList_Sec = new List<string>();
        List<double> distanceList_Sec = new List<double>();
        List<int> routeDistanceAscIndexList_Sec = new List<int>();
        string returnMinRoute_Sec;
        string returnAllRoute_Sec;
        //public string[] DownstreamSearchSection(string startAdr, string endAdr, int flag)
        public string[] DownstreamSearchSection(string startAdr, string endAdr, int flag, bool isIgnoreStatus = false)//A0.01
        {
            lock (searchSafe_lockObj)
            {
                startAdr = startAdr.ToUpper();
                endAdr = endAdr.ToUpper();
                returnMinRoute_Sec = string.Empty;
                returnAllRoute_Sec = string.Empty;
                string[] returnArray = new string[2];
                List<Section> fromSectionList = new List<Section>();
                List<Section> toSectionList = new List<Section>();
                try
                {
                    foreach (Section sec in SectionList)
                    {
                        if (SCUtility.isMatche(sec.FromAdr, startAdr))
                            fromSectionList.Add(sec);
                        if (SCUtility.isMatche(sec.FromAdr, endAdr))
                            toSectionList.Add(sec);
                    }
                    if (startAdr != endAdr)
                        //A0.01 DownstreamSearchSection(fromSectionList, toSectionList, startAdr, endAdr);
                        DownstreamSearchSection(fromSectionList, toSectionList, startAdr, endAdr, isIgnoreStatus);//A0.01
                    else
                        //A0.01 DownstreamSearchSection(fromSectionList, fromSectionList);
                        DownstreamSearchSection(fromSectionList, fromSectionList, isIgnoreStatus);//A0.01

                    //if (distanceList_Sec.Count() > 0)
                    //{
                    //    int minDistanceIndex = distanceList_Sec.IndexOf(distanceList_Sec.Min());
                    //    returnMinRoute_Sec = routeList_Sec[minDistanceIndex];
                    //}
                    for (int i = 0; i < distanceList_Sec.Count(); i++)
                    {
                        int minDistanceIndex = distanceList_Sec.IndexOf(distanceList_Sec.Min());
                        routeDistanceAscIndexList_Sec.Add(minDistanceIndex);
                        distanceList_Sec[minDistanceIndex] = distanceList_Sec[minDistanceIndex] + 999999999999999;
                    }
                    if (distanceList_Sec.Count() > 0)
                        //returnMinRoute_Sec = routeList_Sec[routeDistanceAscIndexList_Sec[0]];
                        returnMinRoute_Sec = routeList_Sec[routeDistanceAscIndexList_Sec[0]] + "=" + (distanceList_Sec[routeDistanceAscIndexList_Sec[0]] - 999999999999999); ;

                    //for (int i = 0; i < routeList_Sec.Count(); i++)
                    //    returnAllRoute_Sec += ";" + routeList_Sec[i] + "=" + distanceList_Sec[i];
                    for (int i = 0; i < routeDistanceAscIndexList_Sec.Count(); i++)
                        returnAllRoute_Sec += ";" + routeList_Sec[routeDistanceAscIndexList_Sec[i]] + "=" + (distanceList_Sec[routeDistanceAscIndexList_Sec[i]] - 999999999999999);
                    switch (flag)
                    {
                        case 0:
                            returnArray[1] = string.Empty;
                            break;
                        case 1:
                            returnArray[1] = returnAllRoute_Sec.TrimStart(';');
                            break;
                    }
                    returnArray[0] = returnMinRoute_Sec;
                    routeList_Sec.Clear();
                    distanceList_Sec.Clear();
                    routeDistanceAscIndexList_Sec.Clear();
                    routeArray_Sec.Clear();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
                return returnArray;
            }

        }

        LinkedList<Section> visitedSec = new LinkedList<Section>();
        List<Section[]> routes_Sec = new List<Section[]>();
        List<Section[]> routeArray_Sec = new List<Section[]>();
        HashSet<Section> visitedSecHS = new HashSet<Section>();
        private void DownstreamSearchSection(List<Section> fromSection, List<Section> toSection, string startAdr, string endAdr, bool isIgnoreStatus)
        {
            try
            {
                var isFromToSectionEqual = new HashSet<Section>(fromSection).SetEquals(toSection);
                if (!isFromToSectionEqual)
                {
                    foreach (Section toSec in toSection)
                    {
                        //A0.01 if (toSec.Status != "Closed")
                        if (isIgnoreStatus || toSec.Status != "Closed")//A0.01
                        {
                            foreach (Section sSec in fromSection)
                                foreach (Section eSec in toSection)
                                {
                                    visitedSec.AddLast(sSec);
                                    //A0.01 SearchSection(eSec, visitedSec);
                                    SearchSection(eSec, visitedSec, isIgnoreStatus);//A0.01
                                    visitedSec.Clear();
                                }
                        }
                    }
                }
                else
                {
                    var intersectArray = fromSection.Intersect(toSection).ToArray();
                    returnAllRoute_Sec = intersectArray[0].SectionCode;
                    returnMinRoute_Sec = intersectArray[0].SectionCode;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }

        //A0.01 public string[] DownstreamSearchSection_FromSecToSec(string fromSec, string toSec, int flag, bool isIncludeLastSec)
        public string[] DownstreamSearchSection_FromSecToSec(string fromSec, string toSec, int flag, bool isIncludeLastSec, bool isIgnoreStatus = false)//A0.01
        {
            lock (searchSafe_lockObj)
            {
                returnMinRoute_Sec = string.Empty;
                returnAllRoute_Sec = string.Empty;
                string[] returnArray = new string[2];
                List<Section> fromSectionList = new List<Section>();
                List<Section> toSectionList = new List<Section>();
                int fromSecIndex = Convert.ToInt16(SectionListIndex[fromSec]);
                int toSecIndex = Convert.ToInt16(SectionListIndex[toSec]);
                fromSectionList.Add(SectionList[fromSecIndex]);
                toSectionList.Add(SectionList[toSecIndex]);
                try
                {
                    //A0.01 DownstreamSearchSection_FromSecToSec(fromSectionList, toSectionList);
                    DownstreamSearchSection_FromSecToSec(fromSectionList, toSectionList, isIgnoreStatus);//A0.01
                    for (int i = 0; i < distanceList_Sec.Count(); i++)
                    {
                        int minDistanceIndex = distanceList_Sec.IndexOf(distanceList_Sec.Min());
                        routeDistanceAscIndexList_Sec.Add(minDistanceIndex);
                        distanceList_Sec[minDistanceIndex] = distanceList_Sec[minDistanceIndex] + 999999999999999;
                    }
                    if (distanceList_Sec.Count() > 0)
                        returnMinRoute_Sec = routeList_Sec[routeDistanceAscIndexList_Sec[0]];

                    for (int i = 0; i < routeDistanceAscIndexList_Sec.Count(); i++)
                        returnAllRoute_Sec += ";" + routeList_Sec[routeDistanceAscIndexList_Sec[i]] + "=" + (distanceList_Sec[routeDistanceAscIndexList_Sec[i]] - 999999999999999);
                    switch (flag)
                    {
                        case 0:
                            returnArray[1] = string.Empty;
                            break;
                        case 1:
                            returnArray[1] = returnAllRoute_Sec.TrimStart(';');
                            break;
                    }
                    if (!isIncludeLastSec)
                        returnArray[0] = returnMinRoute_Sec;
                    else
                        returnArray[0] = returnMinRoute_Sec + ',' + toSec;
                    routeList_Sec.Clear();
                    distanceList_Sec.Clear();
                    routeDistanceAscIndexList_Sec.Clear();
                    routeArray_Sec.Clear();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
                return returnArray;
            }
        }

        //A0.01 public string[] DownstreamSearchSection_FromSecToAdr(string fromSec, string toAdr, int flag)
        public string[] DownstreamSearchSection_FromSecToAdr(string fromSec, string toAdr, int flag, bool isIgnoreStatus = false)//A0.01
        {
            lock (searchSafe_lockObj)
            {
                fromSec = fromSec.Trim();
                toAdr = toAdr.Trim();
                returnMinRoute_Sec = string.Empty;
                returnAllRoute_Sec = string.Empty;
                string[] returnArray = new string[2];
                List<Section> fromSectionList = new List<Section>();
                List<Section> toSectionList = new List<Section>();
                int fromSecIndex = Convert.ToInt16(SectionListIndex[fromSec]);
                fromSectionList.Add(SectionList[fromSecIndex]);
                foreach (Section sec in SectionList)
                    if (sec.FromAdr == toAdr)
                        toSectionList.Add(sec);
                try
                {
                    //A0.01 DownstreamSearchSection_FromSecToSec(fromSectionList, toSectionList);
                    DownstreamSearchSection_FromSecToSec(fromSectionList, toSectionList, isIgnoreStatus);//A0.01
                    for (int i = 0; i < distanceList_Sec.Count(); i++)
                    {
                        int minDistanceIndex = distanceList_Sec.IndexOf(distanceList_Sec.Min());
                        routeDistanceAscIndexList_Sec.Add(minDistanceIndex);
                        distanceList_Sec[minDistanceIndex] = distanceList_Sec[minDistanceIndex] + 999999999999999;
                    }
                    if (distanceList_Sec.Count() > 0)
                        //returnMinRoute_Sec = routeList_Sec[routeDistanceAscIndexList_Sec[0]];
                        returnMinRoute_Sec = routeList_Sec[routeDistanceAscIndexList_Sec[0]] + "=" + (distanceList_Sec[routeDistanceAscIndexList_Sec[0]] - 999999999999999); ;

                    for (int i = 0; i < routeDistanceAscIndexList_Sec.Count(); i++)
                        returnAllRoute_Sec += ";" + routeList_Sec[routeDistanceAscIndexList_Sec[i]] + "=" + (distanceList_Sec[routeDistanceAscIndexList_Sec[i]] - 999999999999999);
                    switch (flag)
                    {
                        case 0:
                            returnArray[1] = string.Empty;
                            break;
                        case 1:
                            returnArray[1] = returnAllRoute_Sec.TrimStart(';');
                            break;
                    }
                    returnArray[0] = returnMinRoute_Sec;
                    routeList_Sec.Clear();
                    distanceList_Sec.Clear();
                    routeDistanceAscIndexList_Sec.Clear();
                    routeArray_Sec.Clear();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
                return returnArray;
            }
        }

        //A0.01 private void DownstreamSearchSection(List<Section> fromSection, List<Section> toSection)
        private void DownstreamSearchSection(List<Section> fromSection, List<Section> toSection, bool isIgnoreStatus)//A0.01 
        {
            try
            {
                visitedSecHS.Clear();
                foreach (Section toSec in toSection)
                {
                    //A0.01 if (toSec.Status != "Closed")
                    if (isIgnoreStatus || toSec.Status != "Closed")//A0.01
                    {
                        foreach (Section sSec in fromSection)
                            foreach (Section eSec in toSection)
                            {
                                visitedSec.AddLast(sSec);
                                //A0.01 SearchSection(eSec, visitedSec);
                                SearchSection(eSec, visitedSec, isIgnoreStatus);//A0.01
                                visitedSec.Clear();
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }

        //A0.01 private void DownstreamSearchSection_FromSecToSec(List<Section> fromSection, List<Section> toSection)
        private void DownstreamSearchSection_FromSecToSec(List<Section> fromSection, List<Section> toSection, bool isIgnoreStatus)//A0.01
        {
            try
            {
                visitedSecHS.Clear();
                foreach (Section toSec in toSection)
                {
                    //A0.01 if (toSec.Status != "Closed")
                    if (isIgnoreStatus || toSec.Status != "Closed")//A0.01
                    {
                        foreach (Section sSec in fromSection)
                            foreach (Section eSec in toSection)
                            {
                                visitedSec.AddLast(sSec);
                                //A0.01 SearchSection(eSec, visitedSec);
                                SearchSection(eSec, visitedSec, isIgnoreStatus);//A0.01
                                visitedSec.Clear();
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }
        //A0.01 private void SearchSection(Section endNode, LinkedList<Section> visitedSec)
        private void SearchSection(Section endNode, LinkedList<Section> visitedSec, bool isIgnoreStatus)//A0.01
        {
            try
            {
                var nodes = visitedSec.Last.Value.DownstreamSectionLists;
                foreach (var sec in nodes)
                {
                    if (sec == endNode)
                    {
                        if (!visitedSecHS.Add(sec))
                            continue;
                        visitedSec.AddLast(sec);
                        routeArray_Sec.Add(visitedSec.ToArray());
                        routes_Sec.Add(visitedSec.ToArray());
                        foreach (var array in routes_Sec)
                        {
                            string strRoute_Sec = string.Empty;
                            double distance = 0;
                            for (int i = 0; i < array.Count() - 1; i++)
                            {
                                strRoute_Sec += "," + array[i].SectionCode;
                                distance += array[i].Distance;
                            }
                            //foreach (Section mySec in array)
                            //{
                            //    strRoute_Sec += "," + mySec.SectionCode;
                            //    distance += mySec.Distance;
                            //}
                            strRoute_Sec = strRoute_Sec.TrimStart(',');
                            distanceList_Sec.Add(distance);
                            routeList_Sec.Add(strRoute_Sec);
                        }
                        visitedSec.RemoveLast();
                        visitedSecHS.Remove(sec);
                    }
                }
                foreach (var sec in nodes)
                {
                    //A0.01 if (sec.Status == "Active")
                    if (isIgnoreStatus || sec.Status == "Active")//A0.01
                    {
                        if (!visitedSecHS.Add(sec))
                            continue;
                        visitedSec.AddLast(sec);
                        //A0.01 SearchSection(endNode, visitedSec);
                        SearchSection(endNode, visitedSec, isIgnoreStatus);//A0.01
                        visitedSec.RemoveLast();
                        visitedSecHS.Remove(sec);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            routes_Sec.Clear();
        }

        //private void SearchSection_FromSecToSec(Section endNode, LinkedList<Section> visitedSec)
        //{
        //    try
        //    {
        //        var nodes = visitedSec.Last.Value.DownstreamSectionLists;
        //        foreach (var sec in nodes)
        //        {
        //            if (sec == endNode)
        //            {
        //                if (!visitedSecHS.Add(sec))
        //                    continue;
        //                visitedSec.AddLast(sec);
        //                routeArray_Sec.Add(visitedSec.ToArray());
        //                routes_Sec.Add(visitedSec.ToArray());
        //                foreach (var array in routes_Sec)
        //                {
        //                    string strRoute_Sec = string.Empty;
        //                    double distance = 0;
        //                    foreach (Section mySec in array)
        //                    {
        //                        strRoute_Sec += "," + mySec.SectionCode;
        //                        distance += mySec.Distance;
        //                    }
        //                    strRoute_Sec = strRoute_Sec.TrimStart(',');
        //                    distanceList_Sec.Add(distance);
        //                    routeList_Sec.Add(strRoute_Sec);
        //                }
        //                visitedSec.RemoveLast();
        //                visitedSecHS.Remove(sec);
        //            }
        //        }
        //        foreach (var sec in nodes)
        //        {
        //            if (sec.Status == "Active")
        //            {
        //                if (!visitedSecHS.Add(sec))
        //                    continue;
        //                visitedSec.AddLast(sec);
        //                SearchSection_FromSecToSec(endNode, visitedSec);
        //                visitedSec.RemoveLast();
        //                visitedSecHS.Remove(sec);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex, "Exception");
        //    }
        //    routes_Sec.Clear();
        //}

        //private void SearchSection_FromSecToAdr(Section endNode, LinkedList<Section> visitedSec)
        //{
        //    try
        //    {
        //        var nodes = visitedSec.Last.Value.DownstreamSectionLists;
        //        foreach (var sec in nodes)
        //        {
        //            if (sec == endNode)
        //            {
        //                if (!visitedSecHS.Add(sec))
        //                    continue;
        //                visitedSec.AddLast(sec);
        //                routeArray_Sec.Add(visitedSec.ToArray());
        //                routes_Sec.Add(visitedSec.ToArray());
        //                foreach (var array in routes_Sec)
        //                {
        //                    string strRoute_Sec = string.Empty;
        //                    double distance = 0;
        //                    for (int i = 0; i < array.Count() - 1; i++)
        //                    {
        //                        strRoute_Sec += "," + array[i].SectionCode;
        //                        distance += array[i].Distance;
        //                    }
        //                    //foreach (Section mySec in array)
        //                    //{
        //                    //    strRoute_Sec += "," + mySec.SectionCode;
        //                    //    distance += mySec.Distance;
        //                    //}
        //                    strRoute_Sec = strRoute_Sec.TrimStart(',');
        //                    distanceList_Sec.Add(distance);
        //                    routeList_Sec.Add(strRoute_Sec);
        //                }
        //                visitedSec.RemoveLast();
        //                visitedSecHS.Remove(sec);
        //            }
        //        }
        //        foreach (var sec in nodes)
        //        {
        //            if (sec.Status == "Active")
        //            {
        //                if (!visitedSecHS.Add(sec))
        //                    continue;
        //                visitedSec.AddLast(sec);
        //                SearchSection(endNode, visitedSec);
        //                visitedSec.RemoveLast();
        //                visitedSecHS.Remove(sec);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex, "Exception");
        //    }
        //    routes_Sec.Clear();
        //}

        public string UpstreamSearchSection(string fromAddress, int sectionNum, int brakeDistance)
        {
            fromAddress = fromAddress.ToUpper();
            Queue<Section> traversalOrder = new Queue<Section>();
            Queue<Section> queue = new Queue<Section>();
            HashSet<Section> hashSet = new HashSet<Section>();
            string secList = string.Empty;
            int count = 0;
            int addSecNum = 0;
            int secIndex = Convert.ToInt16(SectionIndex[fromAddress]);
            double totalSectionDistance = 0;

            try
            {
                queue.Enqueue(SectionList[secIndex]);
                hashSet.Add(SectionList[secIndex]);
                while (queue.Count() > 0)
                {
                    Section section = queue.Dequeue();
                    traversalOrder.Enqueue(section);
                    section.UpstreamSectionLists.Sort(delegate (Section sec1, Section sec2)
                    {
                        return sec1.Distance.CompareTo(sec2.Distance);
                    });

                    foreach (Section secCode in section.UpstreamSectionLists)
                    {
                        if (!hashSet.Contains(secCode))
                        {
                            queue.Enqueue(secCode);
                            hashSet.Add(secCode);
                        }
                    }
                    while (traversalOrder.Count > 0)
                    {
                        Section secCode = traversalOrder.Dequeue();
                        if (count >= 1)
                        {
                            double extantDistance;
                            totalSectionDistance += secCode.Distance;
                            extantDistance = brakeDistance - totalSectionDistance;
                            if (totalSectionDistance <= brakeDistance)
                                addSecNum++;
                            if (totalSectionDistance > brakeDistance)
                                secList += "," + secCode.SectionCode;
                        }
                    }
                    count++;
                    if (count >= sectionNum + addSecNum + 1)
                        break;
                }
                secList = secList.TrimStart(',');
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            return secList;
        }


        public string ReturnInnerAddress(string SegCode, string fromAdr, string toAdr)
        {
            fromAdr = fromAdr.ToUpper(); toAdr = toAdr.ToUpper();
            string strReturn = string.Empty;
            var addressArray = SegmentList[Convert.ToInt16(SegmentListIndex[SegCode])].Address.ToArray();

            try
            {
                if (addressArray.Count() > 0)
                {
                    var firstAddIndex = Array.IndexOf(addressArray, fromAdr);
                    var secondAddIndex = Array.IndexOf(addressArray, toAdr);
                    if (firstAddIndex >= 0 && secondAddIndex >= 0)
                    {
                        if (secondAddIndex >= firstAddIndex)
                        {
                            var addArray = addressArray.Skip(firstAddIndex).Take(secondAddIndex - firstAddIndex + 1).ToArray();
                            foreach (string add in addArray)
                                strReturn += "," + add;
                        }
                        else
                            strReturn = "WrongOrder";
                    }
                    else
                        strReturn = "DifferentSegment";
                }
                strReturn = strReturn.TrimStart(',');
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            return strReturn;
        }


        public ASEGMENT CloseSegment(string strSegCode)
        {
            ASEGMENT seg_do = null;
            try
            {
                int i = Convert.ToInt16(SegmentListIndex[strSegCode]);
                Segment seg = SegmentList[i];
                seg.Status = "Closed";
                //bll.updateSegStatus(seg.SegmentCode, E_SEG_STATUS.Closed);
                seg_do = bll.DisableSegment(seg.SegmentCode);

                foreach (Section sec in SectionList)
                    if (sec.Segment == strSegCode)
                        sec.Status = seg.Status;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            return seg_do;
        }

        public ASEGMENT CloseSegment(string strSegCode, ASEGMENT.DisableType disableType)
        {
            ASEGMENT seg_vo = null;
            ASEGMENT seg_do = null;
            try
            {
                seg_vo = scApp.SegmentBLL.cache.GetSegment(strSegCode);
                lock (seg_vo)
                {
                    seg_do = bll.DisableSegment(strSegCode, disableType);
                    int i = Convert.ToInt16(SegmentListIndex[strSegCode]);
                    Segment seg = SegmentList[i];
                    seg.Status = "Closed";
                    //bll.updateSegStatus(seg.SegmentCode, E_SEG_STATUS.Closed);

                    foreach (Section sec in SectionList)
                        if (sec.Segment == strSegCode)
                            sec.Status = seg.Status;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            return seg_do;
        }


        public ASEGMENT OpenSegment(string strSegCode)
        {
            ASEGMENT seg_do = null;
            try
            {
                int i = Convert.ToInt16(SegmentListIndex[strSegCode]);
                Segment seg = SegmentList[i];
                if (seg.SegmentType == "Common")
                {
                    seg.Status = "Active";
                    //bll.updateSegStatus(seg.SegmentCode, E_SEG_STATUS.Active);
                    seg_do = bll.EnableSegment(seg.SegmentCode);
                }
                else if (seg.SegmentType == "Rotating")
                {
                    seg.Status = "Active";
                    //bll.updateSegStatus(seg.SegmentCode, E_SEG_STATUS.Active);
                    seg_do = bll.EnableSegment(seg.SegmentCode);
                }
                else if (seg.SegmentType == "Station")
                {
                    seg.Status = "Active";
                    //bll.updateSegStatus(seg.SegmentCode, E_SEG_STATUS.Active);
                    seg_do = bll.EnableSegment(seg.SegmentCode);
                }
                else if (seg.SegmentType == "Maintenance")
                {
                    seg.Status = "Active";
                    //bll.updateSegStatus(seg.SegmentCode, E_SEG_STATUS.Active);
                    seg_do = bll.EnableSegment(seg.SegmentCode);
                }

                foreach (Section sec in SectionList)
                    if (sec.Segment == strSegCode)
                        sec.Status = seg.Status;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            return seg_do;
        }
        public ASEGMENT OpenSegment(string strSegCode, ASEGMENT.DisableType disableType)
        {
            ASEGMENT seg_vo = null;
            ASEGMENT seg_do = null;
            try
            {
                seg_vo = scApp.SegmentBLL.cache.GetSegment(strSegCode);
                lock (seg_vo)
                {
                    seg_do = bll.EnableSegment(strSegCode, disableType);
                    if (seg_do.STATUS == E_SEG_STATUS.Active)
                    {
                        int i = Convert.ToInt16(SegmentListIndex[strSegCode]);
                        Segment seg = SegmentList[i];
                        if (seg.SegmentType == "Common")
                        {
                            seg.Status = "Active";
                        }
                        else if (seg.SegmentType == "Rotating")
                        {
                            seg.Status = "Active";
                        }
                        else if (seg.SegmentType == "Station")
                        {
                            seg.Status = "Active";
                        }
                        else if (seg.SegmentType == "Maintenance")
                        {
                            seg.Status = "Active";
                        }

                        foreach (Section sec in SectionList)
                            if (sec.Segment == strSegCode)
                                sec.Status = seg.Status;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            return seg_do;
        }

        private void SectionDoubleLink(Section Sec1, Section Sec2)
        {
            try
            {
                Sec2.isUpstreamOf(Sec1);
                Sec1.isDownstreamOf(Sec2);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }


        private void SegmentDoubleLink(Segment firstSeg, Segment secondSeg)
        {
            try
            {
                secondSeg.isDownstreamOf(firstSeg);
                firstSeg.isUpstreamOf(secondSeg);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }


        private List<int> ReturnToSegIndex(string endSeg)
        {
            List<int> endSegIndex = new List<int>();
            try
            {
                var segCode = endSeg.Split(',').ToArray();
                foreach (var seg in segCode)
                    endSegIndex.Add(Convert.ToInt16(SegmentListIndex[seg]));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            return endSegIndex;
        }


        public bool checkRoadIsWalkable(string from_adr, string to_adr)
        {
            KeyValuePair<string[], double> route_distance;
            return checkRoadIsWalkable(from_adr, to_adr, out route_distance);
        }
        public bool checkRoadIsWalkable(string from_adr, string to_adr, out KeyValuePair<string[], double> route_distance)
        {
            route_distance = default(KeyValuePair<string[], double>);

            string[] route = DownstreamSearchSection
                                 (from_adr, to_adr, 1);
            if (SCUtility.isEmpty(route[0]))
            {
                return false;
            }
            bool isWalkable = false;
            if (scApp.getEQObjCacheManager().getLine().SegmentPreDisableExcuting)
            {
                List<string> nonActiveSeg = scApp.MapBLL.loadNonActiveSegmentNum();
                string[] AllRoute = route[1].Split(';');
                List<KeyValuePair<string[], double>> routeDetailAndDistance = PaserRoute2SectionsAndDistance(AllRoute);
                foreach (var routeDetial in routeDetailAndDistance.ToList())
                {
                    List<ASECTION> lstSec = scApp.MapBLL.loadSectionBySecIDs(routeDetial.Key.ToList());
                    string[] secOfSegments = lstSec.Select(s => s.SEG_NUM).Distinct().ToArray();
                    bool isIncludePassSeg = secOfSegments.Where(seg => nonActiveSeg.Contains(seg)).Count() != 0;
                    if (isIncludePassSeg)
                    {
                        routeDetailAndDistance.Remove(routeDetial);
                    }
                }
                if (routeDetailAndDistance.Count > 0)
                {
                    route_distance = routeDetailAndDistance.OrderBy(keyValue => keyValue.Value).FirstOrDefault();
                    isWalkable = true;
                }
            }
            else
            {
                string[] routeSection;
                double idistance;
                RouteInfo2KeyValue(route[0], out routeSection, out idistance);
                route_distance = new KeyValuePair<string[], double>(routeSection, idistance);
                isWalkable = true;
            }
            return isWalkable;
        }

        private List<KeyValuePair<string[], double>> PaserRoute2SectionsAndDistance(string[] AllRoute)
        {
            List<KeyValuePair<string[], double>> routeDetailAndDistance = new List<KeyValuePair<string[], double>>();
            foreach (string routeDetial in AllRoute)
            {
                string[] routeSection;
                double idistance;
                RouteInfo2KeyValue(routeDetial, out routeSection, out idistance);
                routeDetailAndDistance.Add(new KeyValuePair<string[], double>(routeSection, idistance));
            }
            return routeDetailAndDistance;
        }

        private void RouteInfo2KeyValue(string routeDetial, out string[] routeSection, out double idistance)
        {
            string route = routeDetial.Split('=')[0];
            routeSection = route.Split(',');
            string distance = routeDetial.Split('=')[1];
            idistance = double.MaxValue;
            if (!double.TryParse(distance, out idistance))
            {
                logger.Warn($"fun:{nameof(PaserRoute2SectionsAndDistance)},parse distance fail.Route:{route},distance:{distance}");
            }
        }
    }
}
