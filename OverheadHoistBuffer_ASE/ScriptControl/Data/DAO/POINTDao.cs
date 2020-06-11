
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class POINTDao
    {
        public void add(DBConnection_EF con, APOINT point)
        {
            con.APOINT.Add(point);
            con.SaveChanges();
        }

        public void Update(DBConnection_EF con, APOINT point)
        {
            //bool isDetached = con.Entry(point).State == EntityState.Modified;
            //if (isDetached)
                con.SaveChanges();
        }

        public List<APOINT> loadAll(DBConnection_EF con)
        {
            var query = from point in con.APOINT
                        orderby point.ADR_ID
                        select point;
            return query.ToList();
        }
        public APOINT getByID(DBConnection_EF con, String point_id)
        {
            var query = from point in con.APOINT
                        where point.POINT_ID == point_id.Trim()
                        orderby point.ADR_ID
                        select point;
            return query.FirstOrDefault();
        }
    }
}
