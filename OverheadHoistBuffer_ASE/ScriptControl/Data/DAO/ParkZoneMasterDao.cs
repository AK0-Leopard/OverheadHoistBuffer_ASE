using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class ParkZoneMasterDao
    {
        public void add(DBConnection_EF con, APARKZONEMASTER park_zone_master)
        {
            con.APARKZONEMASTER.Add(park_zone_master);
            con.SaveChanges();
        }


        public APARKZONEMASTER getByID(DBConnection_EF con, String _park_zone_id)
        {
            var query = from obj in con.APARKZONEMASTER
                        where obj.PARK_ZONE_ID == _park_zone_id.Trim()
                        select obj;
            return query.FirstOrDefault();
        }

        public void upadate(DBConnection_EF con, APARKZONEMASTER update_obj)
        {
            con.SaveChanges();
        }

        public APARKZONEMASTER getByParkingAdr(DBConnection_EF con, String adr_id)
        {
            var query = from master in con.APARKZONEMASTER
                        join detail in con.APARKZONEDETAIL on master.PARK_ZONE_ID equals detail.PARK_ZONE_ID
                        where detail.ADR_ID == adr_id.Trim()
                        select master;
            return query.FirstOrDefault();
        }

        public List<APARKZONEMASTER> loadAll(DBConnection_EF con)
        {
            var query = from obj in con.APARKZONEMASTER
                        orderby obj.PARK_ZONE_ID
                        select obj;
            return query.ToList();
        }

        public List<APARKZONEMASTER> loadByParkTypeID(DBConnection_EF con, String _park_type_id, E_VH_TYPE vh_type)
        {
            var query = from obj in con.APARKZONEMASTER
                        where obj.PARK_TYPE_ID == _park_type_id.Trim() && obj.VEHICLE_TYPE == vh_type && obj.IS_ACTIVE
                        select obj;
            return query.ToList();
        }
        public List<APARKZONEMASTER> loadByParkTypeID(DBConnection_EF con, String _park_type_id)
        {
            var query = from obj in con.APARKZONEMASTER
                        where obj.PARK_TYPE_ID == _park_type_id.Trim()
                        select obj;
            return query.ToList();
        }

        public List<APARKZONEMASTER> loadByParkTypeIDAndHasVh(DBConnection_EF con, String _park_type_id)
        {
            var query = from park_master in con.APARKZONEMASTER
                        join park_detail in con.APARKZONEDETAIL
                        on park_master.PARK_ZONE_ID equals park_detail.PARK_ZONE_ID into park_masterDetail
                        from masterDetail in park_masterDetail.DefaultIfEmpty()
                        where park_master.PARK_TYPE_ID == _park_type_id.Trim()
                            && (masterDetail.CAR_ID != null && masterDetail.CAR_ID != "")
                        select park_master;
            return query.Distinct().ToList();
        }



        public List<APARKZONEMASTER> loadByParkTypeIDAndNotEnoughtLowerBorder(DBConnection_EF con, String _park_type_id)
        {
            var query = from obj in con.APARKZONEMASTER
                        where obj.PARK_TYPE_ID == _park_type_id.Trim()
                        select obj;
            return query.ToList();
        }


        public List<APARKZONEMASTER> laodParkZoneMasterByAdr(DBConnection_EF con, List<String> adrs)
        {
            var query = from parkzonemaster in con.APARKZONEMASTER
                            //where adrs.Any(s => parkzonemaster.ENTRY_ADR_ID.Contains(s.Trim()))
                        where adrs.Contains(parkzonemaster.ENTRY_ADR_ID.Trim())
                        select parkzonemaster;
            return query.ToList();
        }

    }
}