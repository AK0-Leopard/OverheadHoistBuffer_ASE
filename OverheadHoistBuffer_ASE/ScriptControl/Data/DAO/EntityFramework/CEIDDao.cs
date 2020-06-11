using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.DAO.EntityFramework
{
    public class CEIDDao
    {
        public void add(DBConnection_EF con, ACEID ceid)
        {
            con.ACEID.Add(ceid);
            con.SaveChanges();
        }

        public void update(DBConnection_EF con, ACEID cmd)
        {
            con.SaveChanges();
        }

        public ACEID getByID(DBConnection_EF con, String ceid_id)
        {
            var query = from ceid in con.ACEID
                        where ceid.CEID == ceid_id.Trim()
                        select ceid;
            return query.SingleOrDefault();
        }
        public Dictionary<string, List<string>> loadAllCEIDByGroup(DBConnection_EF con)
        {
            var query = from ceid in con.ACEID
                        group ceid by ceid.CEID into grp
                        select grp;
            return query.ToDictionary(grp => grp.Key.Trim(), grp => grp.OrderBy(ceid=>ceid.ORDER_NUM).Select(ceid => ceid.RPTID.Trim()).ToList());
        }



    }

}
