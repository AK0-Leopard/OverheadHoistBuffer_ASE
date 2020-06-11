using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.DAO.EntityFramework;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.NetworkInformation;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class NetworkQualityBLL
    {

        NetworkQualityDao networkQualityDAO = null;
        private SCApplication scApp = null;
        public NetworkQualityBLL()
        {

        }
        public void start(SCApplication app)
        {
            scApp = app;
            networkQualityDAO = scApp.NetworkQualityDao;
        }

        Random rnd = new Random(Guid.NewGuid().GetHashCode());
        public bool VhNetworkQualityTest(string vh_id, string adr_id, string sec_id, int acc_dist)
        {
            bool isSuccess = false;
            //Equipment eq = scApp.getEQObjCacheManager().getEquipmentByEQPTID(vh_id);
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            //string remote_ip = vh.getIPAddress(scApp.getBCFApplication());
            System.Net.IPEndPoint remote_ip = vh.getIPEndPoint(scApp.getBCFApplication());
            if (remote_ip == null) return false;
            PingReply reply = null;
            if (SCUtility.PingIt(remote_ip.Address, out reply))
            {
                //saveAndUpdate(vh_id, adr_id, sec_id, acc_dist, reply.RoundtripTime);
                //int randomMax = 0;
                //int randomMin = 0;
                //if (SCUtility.isMatche(sec_id, "041") ||
                //    SCUtility.isMatche(sec_id, "043") ||
                //    SCUtility.isMatche(sec_id, "021") ||
                //    SCUtility.isMatche(sec_id, "035"))
                //{
                //    randomMax = 150;
                //    randomMin = 101;
                //}
                //else if (SCUtility.isMatche(sec_id, "006") ||
                //    SCUtility.isMatche(sec_id, "202"))
                //{
                //    randomMax = 99;
                //    randomMin = 51;
                //}
                //else
                //{
                //    randomMax = 49;
                //    randomMin = 0;

                //}
                //if (SCUtility.isMatche(vh_id, "OHT03"))
                //{
                //    randomMax = 150;
                //    randomMin = 101;
                //}

                //addNetworkQuality(vh_id, adr_id, sec_id, acc_dist, rnd.Next(randomMin, randomMax));
                addNetworkQuality(vh_id, adr_id, sec_id, acc_dist, reply.RoundtripTime);
                isSuccess = true;
            }
            return isSuccess;
        }


        public bool saveAndUpdate(string vh_id, string adr_id, string sec_id, int acc_dist, long ping_time)
        {
            bool isSuccess = true;
            ANETWORKQUALITY network_quality = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                network_quality = networkQualityDAO.getByVhAdrAndSecID(con, vh_id, adr_id, sec_id);
                if (network_quality == null)
                {
                    network_quality = new ANETWORKQUALITY()
                    {
                        VEICLE_ID = vh_id,
                        ADR_ID = adr_id,
                        SEC_ID = sec_id,
                        ACC_SEC_DIST = acc_dist,
                        PING_TIME = ping_time,
                        UPD_TIME = DateTime.Now
                    };
                    networkQualityDAO.add(con, network_quality);
                }
                else
                {
                    network_quality.ACC_SEC_DIST = acc_dist;
                    network_quality.PING_TIME = ping_time;
                    network_quality.UPD_TIME = DateTime.Now;
                    networkQualityDAO.update(con);
                }
            }
            return isSuccess;
        }

        public List<ANETWORKQUALITY> loadNetworkQualityBySecID(string sec_id)
        {
            List<ANETWORKQUALITY> lstNetwork = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                lstNetwork = networkQualityDAO.loadBySecID(con, sec_id);
            }
            return lstNetwork;
        }

        public List<ANETWORKQUALITY> loadNetworkQualityByVhID(string vh_id)
        {
            List<ANETWORKQUALITY> lstNetwork = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                lstNetwork = networkQualityDAO.loadByVhID(con, vh_id);
            }
            lstNetwork.Reverse();
            return lstNetwork;
        }

        public List<ANETWORKQUALITY> loadBySecIDAndVhID(string sec_id, string vh_id)
        {
            List<ANETWORKQUALITY> lstNetwork = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                lstNetwork = networkQualityDAO.loadBySecIDAndVhID(con, sec_id, vh_id);
            }
            return lstNetwork;
        }

        int eachSectionOfPingTime_MaxKeepConut = 10;
        public List<ANETWORKQUALITY> addNetworkQuality(string vh_id, string adr_id, string sec_id, int acc_dist, long ping_time)
        {
            List<ANETWORKQUALITY> lstNetwork = null;
            //DBConnection_EF con = DBConnection_EF.GetContext();
            using (DBConnection_EF con = new DBConnection_EF())
            {
                lstNetwork = networkQualityDAO.loadBySecIDAndVhID(con, sec_id, vh_id);
                int networkCount = lstNetwork.Count;
                if (lstNetwork.Count >= eachSectionOfPingTime_MaxKeepConut)
                {
                    for (int i = eachSectionOfPingTime_MaxKeepConut - 1; i < networkCount; i++)
                    {
                        networkQualityDAO.remove(con, lstNetwork[i]);
                        lstNetwork.Remove(lstNetwork[i]);
                    }
                }

                ANETWORKQUALITY network_quality = new ANETWORKQUALITY()
                {
                    VEICLE_ID = vh_id,
                    ADR_ID = adr_id,
                    SEC_ID = sec_id,
                    ACC_SEC_DIST = acc_dist,
                    PING_TIME = ping_time,
                    UPD_TIME = DateTime.Now
                };
                networkQualityDAO.add(con, network_quality);
            }
            return lstNetwork;
        }

        public Dictionary<string, double> loadGroupBySecIDAndAvgPingTime()
        {
            Dictionary<string, double> dicSecAndPingTime = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                dicSecAndPingTime = networkQualityDAO.loadGroupBySecIDAndAvgPingTime(con);
            }
            return dicSecAndPingTime;
        }

        public Dictionary<string, List<ANETWORKQUALITY>> loadNetworkQualityBySecGroupByVhID(string sec_id)
        {
            Dictionary<string, List<ANETWORKQUALITY>> dicVhAndnetwork = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                dicVhAndnetwork = networkQualityDAO.loadBySCEIDGroupByVhID(con, sec_id);
            }
            return dicVhAndnetwork;
        }

        public Dictionary<string, List<ANETWORKQUALITY>> loadNetworkQualityByVhIDGroupBySecID(string vh_id)
        {
            Dictionary<string, List<ANETWORKQUALITY>> dicVhAndnetwork = null;
            using (DBConnection_EF con = new DBConnection_EF())
            {
                dicVhAndnetwork = networkQualityDAO.loadByVhIDGroupBySecID(con, vh_id);
            }
            return dicVhAndnetwork;
        }
    }
}
