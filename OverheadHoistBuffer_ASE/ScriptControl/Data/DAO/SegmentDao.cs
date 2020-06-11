using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class SegmentDao
    {
        public ASEGMENT getByID(DBConnection_EF con, string num)
        {
            if (string.IsNullOrWhiteSpace(num))
                return null;
            var query = from s in con.ASEGMENT
                        where s.SEG_NUM == num.Trim()
                        orderby s.SEG_NUM
                        select s;
            return query.FirstOrDefault();
        }

        public void Update(DBConnection_EF con, ASEGMENT seg)
        {
            //bool isDetached = con.Entry(seg).State == EntityState.Modified;
            //if (isDetached)
            con.SaveChanges();
        }

        public List<ASEGMENT> loadAllSegments(DBConnection_EF con)
        {
            var query = from seg in con.ASEGMENT
                        select seg;
            return query.ToList();
        }
        public List<ASEGMENT> loadPreDisableSegment(DBConnection_EF con)
        {
            var query = from seg in con.ASEGMENT
                        where seg.PRE_DISABLE_FLAG
                        select seg;
            return query.ToList();
        }
        public List<string> loadAllSegmentIDs(DBConnection_EF con)
        {
            var query = from seg in con.ASEGMENT
                        select seg.SEG_NUM;
            return query.ToList();
        }

        public List<string> loadAllNonActiveSegmentNums(DBConnection_EF con)
        {
            var query = from seg in con.ASEGMENT
                        where seg.PRE_DISABLE_FLAG 
                        || seg.STATUS == E_SEG_STATUS.Closed
                        select seg.SEG_NUM.Trim();
            return query.ToList();
        }

        public bool getPreDisableFlag(DBConnection_EF con, string seg_num)
        {
            var query = from seg in con.ASEGMENT
                        where seg.SEG_NUM == seg_num.Trim()
                        select seg.PRE_DISABLE_FLAG;
            return query.SingleOrDefault();
        }
    }
}
