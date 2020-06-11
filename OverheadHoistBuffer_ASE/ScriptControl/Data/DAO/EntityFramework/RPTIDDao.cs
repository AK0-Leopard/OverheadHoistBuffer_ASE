using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.DAO.EntityFramework
{
    public class RPTIDDao
    {
        public void add(DBConnection_EF con, ARPTID ceid)
        {
            con.ARPTID.Add(ceid);
            con.SaveChanges();
        }

        public void update(DBConnection_EF con, ARPTID cmd)
        {
            con.SaveChanges();
        }

        public ARPTID getByID(DBConnection_EF con, String rpt_id)
        {
            var query = from rptid in con.ARPTID
                        where rptid.RPTID == rpt_id.Trim()
                        select rptid;
            return query.SingleOrDefault();
        }
        public Dictionary<string, List<ARPTID>> loadAllRPTIDByGroup(DBConnection_EF con)
        {
            var query = from rptid in con.ARPTID
                        group rptid by rptid.RPTID into grp
                        select grp;
            return query.ToDictionary(grp => grp.Key.Trim(), grp => grp.Select(rptid => rptid).ToList());
        }



    }

}
