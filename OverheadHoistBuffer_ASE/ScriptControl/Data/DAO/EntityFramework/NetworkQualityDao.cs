using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.DAO.EntityFramework
{
    public class NetworkQualityDao
    {
        public void add(DBConnection_EF con, ANETWORKQUALITY network_quality)
        {
            con.ANETWORKQUALITY.Add(network_quality);
            con.SaveChanges();
        }

        public void update(DBConnection_EF con)
        {
            con.SaveChanges();
        }
        public void remove(DBConnection_EF con, ANETWORKQUALITY network_qualitys)
        {
            con.ANETWORKQUALITY.Remove(network_qualitys);
            con.SaveChanges();
        }


        public ANETWORKQUALITY getByVhAdrAndSecID(DBConnection_EF con, String vh_id, string adr_id, string sec_id)
        {
            var query = from network_quality in con.ANETWORKQUALITY
                        where network_quality.VEICLE_ID == vh_id.Trim() &&
                        network_quality.ADR_ID == adr_id.Trim() &&
                        network_quality.SEC_ID == sec_id.Trim()
                        select network_quality;
            return query.SingleOrDefault();
        }

        public List<ANETWORKQUALITY> loadBySecID(DBConnection_EF con, string sec_id)
        {
            var query = from network_quality in con.ANETWORKQUALITY
                        where network_quality.SEC_ID == sec_id.Trim()
                        orderby network_quality.UPD_TIME descending
                        select network_quality;
            return query.ToList();
        }
        public List<ANETWORKQUALITY> loadByVhID(DBConnection_EF con, string vh_id)
        {
            var query = from network_quality in con.ANETWORKQUALITY
                        where network_quality.VEICLE_ID == vh_id.Trim()
                        orderby network_quality.UPD_TIME descending
                        select network_quality;
            return query.Take(100).ToList();
        }
        public List<ANETWORKQUALITY> loadBySecIDAndVhID(DBConnection_EF con, string sec_id, string vh_id)
        {
            var query = from network_quality in con.ANETWORKQUALITY
                        where network_quality.SEC_ID == sec_id.Trim() &&
                        network_quality.VEICLE_ID == vh_id.Trim()
                        orderby network_quality.UPD_TIME
                        select network_quality;
            return query.ToList();
        }


        public Dictionary<string, double> loadGroupBySecIDAndAvgPingTime(DBConnection_EF con)
        {
            var query = from networkQuality in con.ANETWORKQUALITY
                        group networkQuality by networkQuality.SEC_ID;

            Dictionary<string, double> dicSecAndPingTime = new Dictionary<string, double>();
            foreach (var q in query)
            {
                string secid = q.Key.Trim();
                double avgPingtime = q.Average(networkQuaality => networkQuaality.PING_TIME);
                dicSecAndPingTime.Add(secid, avgPingtime);
            }
            return dicSecAndPingTime;
        }

        public Dictionary<string, List<ANETWORKQUALITY>> loadBySCEIDGroupByVhID(DBConnection_EF con, string sec_id)
        {
            var query = from networkQuality in con.ANETWORKQUALITY
                        where networkQuality.SEC_ID == sec_id.Trim()
                        group networkQuality by networkQuality.VEICLE_ID;
            return query.ToDictionary(item => item.Key.Trim(), item => item.ToList());

        }


        public Dictionary<string, List<ANETWORKQUALITY>> loadByVhIDGroupBySecID(DBConnection_EF con, string vh_id)
        {
            var query = from networkQuality in con.ANETWORKQUALITY
                        where networkQuality.VEICLE_ID == vh_id.Trim()
                        group networkQuality by networkQuality.SEC_ID;
            return query.ToDictionary(item => item.Key.Trim(), item => item.ToList());

        }

    }

}
