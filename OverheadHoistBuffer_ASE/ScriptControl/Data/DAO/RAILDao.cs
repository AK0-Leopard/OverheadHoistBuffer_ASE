using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class RAILDao
    {
        public void add(DBConnection_EF con, ARAIL rail)
        {
            con.ARAIL.Add(rail);
            con.SaveChanges();
        }

        public void update(DBConnection_EF con, ARAIL rail, int l)
        {
            con.ARAIL.Attach(rail);
            rail.LENGTH = l;
            con.SaveChanges();
        }

        public List<ARAIL> loadAll(DBConnection_EF con)
        {
            return con.ARAIL.ToList();
        }
        public ARAIL getByID(DBConnection_EF con, String RAIL_ID)
        {
            var query = from b in con.ARAIL
                        where b.RAIL_ID == RAIL_ID
                        orderby b.RAIL_ID
                        select b;
            return query.FirstOrDefault();
        }
    }

}
