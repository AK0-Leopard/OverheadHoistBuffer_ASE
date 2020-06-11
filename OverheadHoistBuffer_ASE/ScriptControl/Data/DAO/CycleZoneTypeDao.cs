
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class CycleZoneTypeDao
    {
        public void add(DBConnection_EF con, ACYCLEZONETYPE cyc_zone_type)
        {
            con.ACYCLEZONETYPE.Add(cyc_zone_type);
            con.SaveChanges();
        }
        public List<ACYCLEZONETYPE> loadAll(DBConnection_EF con)
        {
            var query = from obj in con.ACYCLEZONETYPE
                        orderby obj.CYCLE_TYPE_ID
                        select obj;
            return query.ToList();
        }
        public ACYCLEZONETYPE getByID(DBConnection_EF con, String _id)
        {
            var query = from obj in con.ACYCLEZONETYPE
                        where obj.CYCLE_TYPE_ID == _id.Trim()
                        select obj;
            return query.FirstOrDefault();
        }
        public ACYCLEZONETYPE getUsingCycleType(DBConnection_EF con)
        {
            var query = from obj in con.ACYCLEZONETYPE
                        where obj.IS_DEFAULT == 1
                        select obj;
            return query.SingleOrDefault();
        }
    }
}