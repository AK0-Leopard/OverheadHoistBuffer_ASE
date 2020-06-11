
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class CycleZoneDetailDao
    {
        public void add(DBConnection_EF con, ACYCLEZONEDETAIL cyc_zone_detail)
        {
            con.ACYCLEZONEDETAIL.Add(cyc_zone_detail);
            con.SaveChanges();
        }

        public void update(DBConnection_EF con, ACYCLEZONEDETAIL section)
        {
            //bool isDetached = con.Entry(section).State == EntityState.Modified;
            //if (isDetached)
                con.SaveChanges();
        }


        public string[] loadSecsByEntryAdr(DBConnection_EF con, string entry_adr)
        {
            var query = from mater in con.ACYCLEZONEMASTER
                        join detail in con.ACYCLEZONEDETAIL
                        on mater.CYCLE_ZONE_ID equals detail.CYCLE_ZONE_ID
                        where mater.ENTRY_ADR_ID == entry_adr.Trim()
                        orderby detail.SEC_ORDER
                        select detail.SEC_ID.Trim();
            return query.ToArray();
        }

    }
}