
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class ParkZoneDetailDao
    {
        public void add(DBConnection_EF con, APARKZONEDETAIL park_zone_detail)
        {
            con.APARKZONEDETAIL.Add(park_zone_detail);
            con.SaveChanges();
        }

        public void update(DBConnection_EF con, APARKZONEDETAIL section)
        {
            //bool isDetached = con.Entry(section).State == EntityState.Modified;
            //if (isDetached)
            con.SaveChanges();
        }

        public APARKZONEDETAIL getParkAdrCountByParkTypeAndAdr(DBConnection_EF con, string park_type_id, String adr_id, E_VH_TYPE park_type)
        {
            var query = from master in con.APARKZONEMASTER
                        join detail in con.APARKZONEDETAIL
                        on master.PARK_ZONE_ID equals detail.PARK_ZONE_ID
                        where master.PARK_TYPE_ID == park_type_id.Trim() &&
                        master.VEHICLE_TYPE == park_type &&
                        detail.ADR_ID == adr_id.Trim()
                        select detail;
            return query.FirstOrDefault();
        }


        public APARKZONEDETAIL getByAdrID(DBConnection_EF con, String adr_id)
        {
            var query = from obj in con.APARKZONEDETAIL
                        where obj.ADR_ID == adr_id.Trim()
                        select obj;
            return query.FirstOrDefault();
        }
        public List<APARKZONEDETAIL> loadByVehicleID(DBConnection_EF con, string vh_id)
        {
            var query = from obj in con.APARKZONEDETAIL
                        where obj.CAR_ID == vh_id.Trim()
                        select obj;
            return query.ToList();
        }
        public APARKZONEDETAIL getByZoneIDAndPRIO(DBConnection_EF con, string zone_id, int prio)
        {
            var query = from obj in con.APARKZONEDETAIL
                        where obj.PARK_ZONE_ID == zone_id.Trim() &&
                        obj.PRIO == prio
                        select obj;
            return query.FirstOrDefault();
        }

        public APARKZONEDETAIL getByParkZoneTypeAndAdrID(DBConnection_EF con, string type_id, String adr_id)
        {
            var query = from obj in con.APARKZONEDETAIL
                        join park_master in con.APARKZONEMASTER
                        on obj.PARK_ZONE_ID equals park_master.PARK_ZONE_ID
                        where obj.ADR_ID == adr_id.Trim()
                        && park_master.PARK_TYPE_ID == type_id.Trim()
                        select obj;
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 用來查找VH該ParkZone最後一個可以停車的位置
        /// 但之後已經改成直接把VH叫到該ParkZone的第一個停車格，所以可以不用再用此Fun查找，改用找出該ParkZone第一個
        /// 停車格的即可
        /// </summary>
        /// <param name="con"></param>
        /// <param name="park_zone_id"></param>
        /// <returns></returns>
        public APARKZONEDETAIL getByParkZoneIDPrioAscAndCanParkingAdr(DBConnection_EF con, String park_zone_id)
        {
            var query = from park_detail in con.APARKZONEDETAIL
                        join vh in con.AVEHICLE on park_detail.ADR_ID equals vh.PARK_ADR_ID into table_detailVh
                        from detail_Vh in table_detailVh.DefaultIfEmpty()
                        where park_detail.PARK_ZONE_ID == park_zone_id.Trim() &&
                             (park_detail.CAR_ID == null || park_detail.CAR_ID == string.Empty) &&
                             (detail_Vh.VEHICLE_ID == null || detail_Vh.VEHICLE_ID == string.Empty)
                        orderby park_detail.PRIO
                        select park_detail;
            return query.FirstOrDefault();
        }
        public APARKZONEDETAIL getByParkZoneIDPrioDes(DBConnection_EF con, String park_zone_id)
        {
            var query = from obj in con.APARKZONEDETAIL
                        where obj.PARK_ZONE_ID == park_zone_id.Trim()
                        orderby obj.PRIO descending
                        select obj;
            return query.FirstOrDefault();
        }
        public List<APARKZONEDETAIL> loadAll(DBConnection_EF con)
        {
            var query = from obj in con.APARKZONEDETAIL
                        orderby obj.PARK_ZONE_ID
                        select obj;
            return query.ToList();
        }

        public List<APARKZONEDETAIL> loadByParkZoneID(DBConnection_EF con, string zone_id)
        {
            var query = from obj in con.APARKZONEDETAIL
                        where obj.PARK_ZONE_ID == zone_id.Trim()
                        select obj;
            return query.ToList();
        }


        //public List<APARKZONEDETAIL> loadByParkZoneIDAndVhOnAdrIncludeOnWay(DBConnection_EF con, string zone_id)
        //{
        //    var query = from obj in con.APARKZONEDETAIL
        //                join vh in con.AVEHICLE on obj.ADR_ID equals vh.PARK_ADR_ID into table_detailVh
        //                from detail_Vh in table_detailVh.DefaultIfEmpty()
        //                where obj.PARK_ZONE_ID == zone_id.Trim()
        //                && (!(obj.CAR_ID == string.Empty || obj.CAR_ID == null)
        //                || !(detail_Vh.VEHICLE_ID == string.Empty || detail_Vh.VEHICLE_ID == null))
        //                orderby obj.PARK_ZONE_ID
        //                select obj;
        //    return query.ToList();
        //}

        /// <summary>
        /// 用來計算是否還有足夠的停車格可以使用(包含在途的)
        /// 當Detail有狀態可以查詢時，就不用再去join vh
        /// </summary>
        /// <param name="con"></param>
        /// <param name="zone_id"></param>
        /// <returns></returns>
        public int getCountByParkZoneIDAndVhOnAdrIncludeOnWay(DBConnection_EF con, string zone_id)
        {
            var query = from obj in con.APARKZONEDETAIL
                        join vh in con.AVEHICLE on obj.ADR_ID equals vh.PARK_ADR_ID into table_detailVh
                        from detail_Vh in table_detailVh.DefaultIfEmpty()
                        where obj.PARK_ZONE_ID == zone_id.Trim()
                        && (!(obj.CAR_ID == string.Empty || obj.CAR_ID == null)
                        || !(detail_Vh.VEHICLE_ID == string.Empty || detail_Vh.VEHICLE_ID == null))
                        orderby obj.PARK_ZONE_ID
                        select obj;
            return query.Count();
        }

        public List<APARKZONEDETAIL> loadByParkZoneIDAndVhOnAdr(DBConnection_EF con, string zone_id)
        {
            var query = from obj in con.APARKZONEDETAIL
                        where obj.PARK_ZONE_ID == zone_id.Trim()
                        && (obj.CAR_ID != null && obj.CAR_ID != string.Empty)
                        orderby obj.PRIO
                        select obj;
            return query.ToList();
        }

        public List<APARKZONEDETAIL> loadByEachParkZoneFirstVh(DBConnection_EF con, string park_type_id)
        {
            List<APARKZONEDETAIL> details = null;
            var query = from master in con.APARKZONEMASTER
                        join detail in con.APARKZONEDETAIL
                        on master.PARK_ZONE_ID equals detail.PARK_ZONE_ID
                        where master.PARK_TYPE_ID == park_type_id &&
                        (detail.CAR_ID != null && detail.CAR_ID != string.Empty)
                        orderby detail.PRIO
                        select detail;
            details = query.ToList();
            HashSet<string> hsZoneID = new HashSet<string>();
            foreach (APARKZONEDETAIL detail in details.ToList())
            {
                if (hsZoneID.Contains(detail.PARK_ZONE_ID.Trim()))
                {
                    details.Remove(detail);
                }
                else
                {
                    hsZoneID.Add(detail.PARK_ZONE_ID.Trim());
                }
            }
            return details;
        }

        public List<APARKZONEDETAIL> loadAllParkAdrByParkTypeID(DBConnection_EF con, string park_type_id)
        {
            List<APARKZONEDETAIL> details = null;
            var query = from master in con.APARKZONEMASTER
                        join detail in con.APARKZONEDETAIL
                        on master.PARK_ZONE_ID equals detail.PARK_ZONE_ID
                        where master.PARK_TYPE_ID == park_type_id.Trim()
                        select detail;
            details = query.ToList();
            return details;
        }
    }
}