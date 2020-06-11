using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class SectionDao
    {
        com.mirle.ibg3k0.sc.App.SCApplication scApp = null;

        public SectionDao()
        {

        }

        public SectionDao(com.mirle.ibg3k0.sc.App.SCApplication app)
        {
            scApp = app;
        }

        public void add(DBConnection_EF con, ASECTION section)
        {
            con.ASECTION.Add(section);
            con.SaveChanges();
        }
        public void delete(DBConnection_EF con, ASECTION section)
        {
            con.ASECTION.Remove(section);
            con.SaveChanges();
        }

        public void update(DBConnection_EF con, ASECTION section)
        {
            //bool isDetached = con.Entry(section).State == EntityState.Modified;
            //if (isDetached)
            con.SaveChanges();
        }


        public ASECTION getByID(DBConnection_EF con, String section_id)
        {
            var query = from s in con.ASECTION
                        where s.SEC_ID == section_id.Trim()
                        orderby s.SEC_ID
                        select s;
            return query.FirstOrDefault();
        }
        public ASECTION getByFromToAdr(DBConnection_EF con, string from_adr, string to_adr)
        {
            if (string.IsNullOrWhiteSpace(from_adr)
                || string.IsNullOrWhiteSpace(to_adr))
            {
                return null;
            }
            var query = from s in con.ASECTION
                        where s.FROM_ADR_ID == from_adr.Trim()
                        && s.TO_ADR_ID == to_adr.Trim()
                        orderby s.SEC_ID
                        select s;
            return query.FirstOrDefault();
        }

        public int getAllCount(DBConnection_EF con)
        {
            var query = from section in con.ASECTION
                        orderby section.SEC_ID
                        select section;
            return query.Count();
        }
        public int getCountBySegmentNum(DBConnection_EF con, string num)
        {
            var query = from section in con.ASECTION
                        where section.SEG_NUM.Trim() == num.Trim()
                        select section;
            return query.Count();
        }



        //public string[] loadNextSectionIDBySectionID(DBConnection_EF con, String section_id)
        //{
        //    if (string.IsNullOrWhiteSpace(section_id))
        //        return null;
        //    string[] nextSection_ids = null;
        //    ASECTION sec = null;
        //    sec = getByID(con, section_id);
        //    if (sec != null)
        //    {
        //        List<ASECTION> nextSections = loadByFromAdr(con, sec.TO_ADR_ID);
        //        if (nextSections != null)
        //            nextSection_ids = nextSections.Select(s => s.SEC_ID).ToArray();
        //    }
        //    return nextSection_ids;
        //}
        public string[] loadNextSectionIDBySectionID(DBConnection_EF con, String section_id)
        {
            if (string.IsNullOrWhiteSpace(section_id))
                return null;
            string[] nextSection_ids = null;
            ASECTION sec = null;
            sec = getByID(con, section_id);
            if (sec != null)
            {
                List<ASECTION> nextSections = loadByFromAdr(con, sec.TO_ADR_ID);
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

        public List<ASECTION> loadNextSectionBySectionID(DBConnection_EF con, String section_id)
        {
            if (string.IsNullOrWhiteSpace(section_id))
                return null;
            List<ASECTION> nextSections = null;
            ASECTION sec = null;
            sec = getByID(con, section_id);
            if (sec != null)
            {
                nextSections = loadByFromAdr(con, sec.TO_ADR_ID);
            }
            return nextSections;
        }

        public List<String> loadNextSegmentNumBySegmentNum(DBConnection_EF con, String segment_num)
        {
            if (string.IsNullOrWhiteSpace(segment_num))
                return null;
            List<String> nextSegment_Nums = new List<string>();
            ASECTION lastsec = null;
            lastsec = loadBySegmentNum(con, segment_num).LastOrDefault();
            if (lastsec != null)
            {
                List<ASECTION> nextSections = loadByFromAdr(con, lastsec.TO_ADR_ID);
                List<ASECTION> branchSections = loadByFromAdr(con, lastsec.FROM_ADR_ID);
                //1.移除掉自己的Section
                branchSections.Remove(lastsec);

                //2.過濾掉並非為該Segment 第1條的Section
                List<ASECTION> branchSectionsTemp = branchSections.ToList();
                foreach (ASECTION sec in branchSectionsTemp)
                {
                    if (!isFirstSectionInSegment(con, sec))
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

        public Boolean isFirstSectionInSegment(DBConnection_EF con, ASECTION section)
        {
            List<ASECTION> lstSec = loadBySegmentNum(con, section.SEG_NUM);
            if (lstSec == null || lstSec.Count() <= 0)
                return false;
            ASECTION FirstSection = lstSec.First();
            return FirstSection.SEC_ID == section.SEC_ID;
        }



        public List<string> loadAllSegmentNum(DBConnection_EF con)
        {
            var query = from s in con.ASECTION
                        orderby s.SEG_NUM
                        select s.SEG_NUM;
            return query.Distinct().ToList();
        }

        public List<string> loadAllAdrBySegmentNum(DBConnection_EF con, String segment_num)
        {
            if (string.IsNullOrWhiteSpace(segment_num))
                return null;
            List<ASECTION> sections = loadBySegmentNum(con, segment_num);
            //HashSet<string> hsaddress = new HashSet<string>();
            List<string> lstaddress = new List<string>();
            foreach (ASECTION sec in sections)
            {
                lstaddress.Add(sec.FROM_ADR_ID);
                lstaddress.Add(sec.TO_ADR_ID);
            }
            //return lstaddress.OrderBy(s => s).ToList();
            return lstaddress.Distinct().ToList();
        }


        public List<ASECTION> loadBySegmentNum(DBConnection_EF con, String segment_num)
        {
            if (string.IsNullOrWhiteSpace(segment_num))
                return null;
            var query = from s in con.ASECTION
                        where s.SEG_NUM == segment_num.Trim()
                        orderby s.SEG_ORDER_NUM
                        select s;
            return query.ToList();
        }


        public List<ASECTION> loadAll(DBConnection_EF con)
        {
            var query = from section in con.ASECTION
                        orderby section.SEC_ID
                        select section;
            return query.ToList();
        }

        public List<ASECTION> loadByFromOrToAdr(DBConnection_EF con, string adr)
        {
            if (string.IsNullOrWhiteSpace(adr))
            {
                return null;
            }
            var query = from s in con.ASECTION
                        where s.FROM_ADR_ID == adr.Trim()
                        || s.TO_ADR_ID == adr.Trim()
                        orderby s.SEC_ID
                        select s;
            return query.ToList();
        }


        public List<string> loadSegmentNumByAdr(DBConnection_EF con, string adr)
        {
            List<string> segmentNums = new List<string>();
            List<ASECTION> lst_from_section, lst_to_section;
            loadByFromOrToAdr(con, adr, out lst_from_section, out lst_to_section);
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

        public void loadByFromOrToAdr(DBConnection_EF con, string adr, out List<ASECTION> lst_from_section, out List<ASECTION> lst_to_section)
        {
            lst_from_section = loadByFromAdr(con, adr);
            lst_to_section = loadByToAdr(con, adr);
        }

        public List<ASECTION> loadByFromAdr(DBConnection_EF con, string from_adr)
        {
            if (string.IsNullOrWhiteSpace(from_adr))
                return null;
            var query = from section in con.ASECTION
                        where section.FROM_ADR_ID == from_adr.Trim()
                        orderby section.SEC_ID
                        select section;
            return query.ToList();
        }

        public List<ASECTION> loadByToAdr(DBConnection_EF con, string to_adr)
        {
            if (string.IsNullOrWhiteSpace(to_adr))
                return null;
            var query = from section in con.ASECTION
                        where section.TO_ADR_ID == to_adr.Trim()
                        orderby section.SEC_ID
                        select section;
            return query.ToList();
        }
        public List<ASECTION> loadByToAdrs(DBConnection_EF con, List<string> to_adrs)
        {
            if (to_adrs == null || to_adrs.Count == 0)
                return null;
            var query = from section in con.ASECTION
                            //where to_adrs.Any(adr => section.TO_ADR_ID.Contains(adr.Trim()))
                        where to_adrs.Contains(section.TO_ADR_ID.Trim())
                        orderby section.SEC_ORDER_NUM
                        select section;
            return query.ToList();
        }

        public List<string> loadAllAddress(DBConnection_EF con)
        {
            List<string> addressList = new List<string>();
            var query = from s in con.ASECTION
                        select s.FROM_ADR_ID;
            var _query = from s in con.ASECTION
                         select s.TO_ADR_ID;
            addressList.AddRange(query.ToList());
            addressList.AddRange(_query.ToList());
            return addressList.Distinct().ToList();
        }

        public List<ASECTION> loadSecBySecIds(DBConnection_EF con, List<string> sec_ids)
        {
            var query = from sec in con.ASECTION
                            //where sec_ids.Any(s => sec.SEC_ID.Contains(s.Trim()))
                        where sec_ids.Contains(sec.SEC_ID.Trim())
                        select sec;
            return query.ToList();
        }


        public List<ASECTION> getSectionBySegmentID(DBConnection_EF con, String segment_id)
        {
            List<ASECTION> section = null;
            if (string.IsNullOrWhiteSpace(segment_id))
                return null;
            var query = from s in con.ASECTION
                        where s.SEG_NUM == segment_id.Trim()
                        select s;
            section = query.ToList();
            return section;
        }


        public int getSectionsDistance(DBConnection_EF con, string[] sections)
        {
            var query = from section in con.ASECTION
                        where sections.Contains(section.SEC_ID)
                        select section.SEC_DIS;
            return (int)query.Sum();
        }

        public Dictionary<string, int> loadGroupBySecIDAndThroughTimes(DBConnection_EF con)
        {
            DateTime dateTime = DateTime.Now.AddDays(-1);
            var query = from section in con.ASECTION
                        join cmd_detail in con.ACMD_OHTC_DETAIL
                        on section.SEC_ID equals cmd_detail.SEC_ID into sec_cmd_detail
                        from sec_detail_info in sec_cmd_detail.DefaultIfEmpty()
                        where sec_detail_info.SEC_LEAVE_TIME > dateTime
                        group section by section.SEC_ID;

            Dictionary<string, int> secAndThroughTimes = query.ToDictionary(q => q.Key.Trim(), q => q.Count() / 24);

            return secAndThroughTimes;
        }

        #region get from catch data
        public ASECTION getByID(String section_id)
        {
            var query = from s in scApp.CatchDataFromDB_Section
                        where s.SEC_ID.Trim() == section_id.Trim()
                        orderby s.SEC_ID
                        select s;
            return query.FirstOrDefault();
        }
        public ASECTION getByFromToAdr(string from_adr, string to_adr)
        {
            if (string.IsNullOrWhiteSpace(from_adr)
                || string.IsNullOrWhiteSpace(to_adr))
            {
                return null;
            }
            var query = from s in scApp.CatchDataFromDB_Section
                        where s.FROM_ADR_ID.Trim() == from_adr.Trim()
                        && s.TO_ADR_ID.Trim() == to_adr.Trim()
                        orderby s.SEC_ID
                        select s;
            return query.FirstOrDefault();
        }
        public List<ASECTION> loadSecBySecIds(List<string> sec_ids)
        {
            var query = from sec in scApp.CatchDataFromDB_Section
                            //where sec_ids.Any(s => sec.SEC_ID.Contains(s.Trim()))
                        where sec_ids.Contains(sec.SEC_ID.Trim())
                        select sec;
            return query.ToList();
        }

        public List<ASECTION> loadByFromOrToAdr(string adr)
        {
            if (string.IsNullOrWhiteSpace(adr))
            {
                return null;
            }
            var query = from s in scApp.CatchDataFromDB_Section
                        where s.FROM_ADR_ID.Trim() == adr.Trim()
                        || s.TO_ADR_ID.Trim() == adr.Trim()
                        orderby s.SEC_ID
                        select s;
            return query.ToList();
        }
        public List<ASECTION> loadByFromAdr(string from_adr)
        {
            var query = from section in scApp.CatchDataFromDB_Section
                        where section.FROM_ADR_ID.Trim() == from_adr.Trim()
                        orderby section.SEC_ORDER_NUM
                        select section;
            return query.ToList();
        }

        public List<ASECTION> loadByToAdrs(List<string> to_adrs)
        {
            if (to_adrs == null || to_adrs.Count == 0)
                return null;
            var query = from section in scApp.CatchDataFromDB_Section
                            //where to_adrs.Contains(section.TO_ADR_ID.Trim())
                        where to_adrs.Any(to_adr => to_adr.Trim() == section.TO_ADR_ID.Trim())
                        orderby section.SEC_ORDER_NUM
                        select section;
            return query.ToList();
        }
        public List<ASECTION> loadByFromAdrs(List<string> from_adrs)
        {
            if (from_adrs == null || from_adrs.Count == 0)
                return null;
            var query = from section in scApp.CatchDataFromDB_Section
                            //where to_adrs.Contains(section.TO_ADR_ID.Trim())
                        where from_adrs.Any(from_adr => from_adr.Trim() == section.FROM_ADR_ID.Trim())
                        orderby section.SEC_ORDER_NUM
                        select section;
            return query.ToList();
        }

        public ASECTION getFirstSecBySegmentID(string seg_id)
        {
            var query = from section in scApp.CatchDataFromDB_Section
                        where section.SEG_NUM == seg_id.Trim()
                        orderby section.SEG_ORDER_NUM
                        select section;
            return query.FirstOrDefault();
        }

        public List<ASECTION> loadSectionsBySegmentID(string segment_num)
        {
            var query = from section in scApp.CatchDataFromDB_Section
                        where section.SEG_NUM == segment_num.Trim()
                        orderby section.SEG_ORDER_NUM
                        select section;
            return query.ToList();
        }

        public List<ASECTION> loadAllSection()
        {
            return scApp.CatchDataFromDB_Section;
        }
        #endregion get from catch data
    }
}
