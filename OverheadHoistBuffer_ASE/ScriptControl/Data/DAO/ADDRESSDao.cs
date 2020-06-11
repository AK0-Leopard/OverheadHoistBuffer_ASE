using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class ADDRESSDao
    {
        public void add(DBConnection_EF con, AADDRESS rail)
        {
            con.AADDRESS.Add(rail);
            con.SaveChanges();
        }
        public void update(DBConnection_EF con, AADDRESS rail)
        {
            //bool isDetached = con.Entry(rail).State == EntityState.Detached;
            //if (isDetached)
                con.AADDRESS.Attach(rail);
            con.SaveChanges();
        }

        public List<AADDRESS> loadAll(DBConnection_EF con)
        {
            return con.AADDRESS
                      .OrderBy(adr => adr.ADR_ID)
                      .ToList();
        }
        public AADDRESS getByID(DBConnection_EF con, String adr_id)
        {
            var query = from b in con.AADDRESS
                        where b.ADR_ID == adr_id
                        orderby b.ADR_ID
                        select b;
            return query.FirstOrDefault();
        }
        public AADDRESS getByPortID(DBConnection_EF con, String port_id)
        {
            var query = from b in con.AADDRESS
                        where b.PORT1_ID == port_id.Trim() ||
                        b.PORT2_ID == port_id.Trim()
                        orderby b.ADR_ID
                        select b;
            return query.FirstOrDefault();
        }
        public int getAdrCount(DBConnection_EF con)
        {
            var query = from b in con.AADDRESS
                        select b;
            return query.Count();
        }
    }

}
