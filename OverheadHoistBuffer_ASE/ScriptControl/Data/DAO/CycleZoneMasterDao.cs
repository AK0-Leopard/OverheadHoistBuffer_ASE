
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class CycleZoneMasterDao
    {
        public void add(DBConnection_EF con, ACYCLEZONEMASTER cyc_zone_master)
        {
            con.ACYCLEZONEMASTER.Add(cyc_zone_master);
            con.SaveChanges();
        }

        public ACYCLEZONEMASTER getByID(DBConnection_EF con, String _cyc_zone_id)
        {
            var query = from obj in con.ACYCLEZONEMASTER
                        where obj.CYCLE_ZONE_ID == _cyc_zone_id.Trim()
                        select obj;
            return query.FirstOrDefault();
        }

        public ACYCLEZONEMASTER getByEntryAdr(DBConnection_EF con, string entry_adr)
        {
            var query = from mater in con.ACYCLEZONEMASTER
                        where mater.ENTRY_ADR_ID == entry_adr.Trim()
                        select mater;
            return query.SingleOrDefault();
        }
        public List<ACYCLEZONEMASTER> loadAll(DBConnection_EF con)
        {
            var query = from obj in con.ACYCLEZONEMASTER
                        orderby obj.CYCLE_TYPE_ID
                        select obj;
            return query.ToList();
        }


        public List<ACYCLEZONEMASTER> loadByCycleTypeID(DBConnection_EF con, String _cycle_type_id)
        {
            var query = from obj in con.ACYCLEZONEMASTER
                        where obj.CYCLE_TYPE_ID == _cycle_type_id.Trim()
                        select obj;
            return query.ToList();
        }

        public List<ACYCLEZONEMASTER> loadByCycleTypeIDAndHasCycleSpace(DBConnection_EF con, String _cycle_type_id)
        {
            List<ACYCLEZONEMASTER> HasCycleSpaceZoneMaster = null; ;

            var query = from zone_master in con.ACYCLEZONEMASTER
                        join vehicle in con.AVEHICLE
                        on zone_master.CYCLE_ZONE_ID equals vehicle.CYCLERUN_ID into VH_CycleMaters
                        from VH_CycleMater in VH_CycleMaters.DefaultIfEmpty()
                        where zone_master.CYCLE_TYPE_ID == _cycle_type_id.Trim() &&
                              (VH_CycleMater.CYCLERUN_ID == null || VH_CycleMater.CYCLERUN_ID == "")
                        group zone_master by zone_master.CYCLE_ZONE_ID;

            foreach (var q in query)
            {
                ACYCLEZONEMASTER masterTemp = q.First();
                if (q.Count() < masterTemp.TOTAL_BORDER)
                {
                    if (HasCycleSpaceZoneMaster == null)
                        HasCycleSpaceZoneMaster = new List<ACYCLEZONEMASTER>();
                    HasCycleSpaceZoneMaster.Add(masterTemp);
                }
            }
            return HasCycleSpaceZoneMaster;
        }


    }
}