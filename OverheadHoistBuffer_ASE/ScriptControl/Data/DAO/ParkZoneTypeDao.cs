
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class ParkZoneTypeDao
    {
        public void add(DBConnection_EF con, APARKZONETYPE park_zone_type)
        {
            con.APARKZONETYPE.Add(park_zone_type);
            con.SaveChanges();
        }

        public void upadate(DBConnection_EF con, APARKZONETYPE update_obj = null)
        {
            con.SaveChanges();
        }

        public List<APARKZONETYPE> loadAll(DBConnection_EF con)
        {
            var query = from obj in con.APARKZONETYPE
                        orderby obj.PARK_TYPE_ID
                        select obj;
            return query.ToList();
        }
        public APARKZONETYPE getByID(DBConnection_EF con, String _id)
        {
            var query = from obj in con.APARKZONETYPE
                        where obj.PARK_TYPE_ID == _id.Trim()
                        select obj;
            return query.FirstOrDefault();
        }
        public APARKZONETYPE getUsingParkType(DBConnection_EF con)
        {
            var query = from obj in con.APARKZONETYPE
                        where obj.IS_DEFAULT == 1
                        select obj;
            return query.SingleOrDefault();
        }
    }
}