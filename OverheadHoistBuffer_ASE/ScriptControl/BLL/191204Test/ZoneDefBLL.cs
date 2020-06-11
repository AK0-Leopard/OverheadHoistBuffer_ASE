using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.sc.Data.DAO;
using NLog;
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.App;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class ZoneDefBLL
    {
        SCApplication scApp = null;
        ZoneDefDao zonedefDao = null;
        ShelfDefDao shelfdefDao = null;
        CassetteDataDao cassetteDao = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public void start(SCApplication scApp)
        {
            this.scApp = scApp;
            zonedefDao = scApp.ZoneDefDao;
            shelfdefDao = scApp.ShelfDefDao;
            cassetteDao = scApp.CassetteDataDao;
        }

        public List<ZoneDef> loadZoneData()
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return zonedefDao.LoadZoneDef(con);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

        }
        public ZoneDef loadZoneDataByID(string zoneid)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return zonedefDao.LoadZoneDefByID(con, zoneid);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

        }

        public bool updateLowWater(string zoneid, int water)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    var zone_def = con.ZoneDef.Where(x => x.ZoneID == zoneid).FirstOrDefault();
                    zone_def.LowWaterMark = water;
                    con.Entry(zone_def).Property(p => p.LowWaterMark).IsModified = true;
                    zonedefDao.UpdateZoneDef(con);
                }

                scApp.TransferService.TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "更新儲位狀態:  zone_id: " + zoneid
                    + " LowWaterMark: " + water
                );
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
            return true;
        }

        public bool updateHighWater(string zoneid, int water)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    var zone_def = con.ZoneDef.Where(x => x.ZoneID == zoneid).FirstOrDefault();
                    zone_def.HighWaterMark = water;
                    con.Entry(zone_def).Property(p => p.HighWaterMark).IsModified = true;
                    zonedefDao.UpdateZoneDef(con);
                }

                scApp.TransferService.TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "更新儲位狀態:  zone_id: " + zoneid
                    + " HighWaterMark: " + water
                );
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
            return true;
        }

        public bool updateColor(string zoneid, string color)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    var zone_def = con.ZoneDef.Where(x => x.ZoneID == zoneid).FirstOrDefault();
                    zone_def.Color = color;
                    con.Entry(zone_def).Property(p => p.Color).IsModified = true;
                    zonedefDao.UpdateZoneDef(con);
                }

                scApp.TransferService.TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "更新儲位狀態:  zone_id: " + zoneid
                    + " color: " + color
                );
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
            return true;
        }

        public int GetZoneCapacity(string zoneid)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {

                    var enable = shelfdefDao.LoadShelfDef(con).Where(x => x.ZoneID == zoneid && x.Enable == "Y").Count();
                    var cassette = cassetteDao.LoadCassetteData(con);
                    int i = 0;
                    cassette.ForEach(x =>
                    {
                        i = i + shelfdefDao.LoadShelfDef(con).Where(y => y.ZoneID == zoneid && y.Enable == "Y" && y.ShelfID == x.Carrier_LOC).Count();
                    });

                    return enable - i;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return 0;
            }

        }



        public int GetZoneTotalSize(string zoneid)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    int result = shelfdefDao.LoadShelfDef(con)
                        .Where(x => x.ZoneID == zoneid && x.Enable == "Y")
                        .ToList()
                        .Count;

                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return 0;
            }

        }

        public List<ShelfDef> GetDisShelf(string zoneid)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    var result = shelfdefDao.LoadDisableShelf(con, zoneid);

                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

        }

        public List<ShelfDef> GetShelfByZoneID(string zoneid)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    var result = shelfdefDao.LoadShelfByZone(con, zoneid);

                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

        }
        public bool IsExist(string zoneid)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    var result = zonedefDao.LoadZoneDefByID(con, zoneid);

                    return result != null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }

        }
    }
}
