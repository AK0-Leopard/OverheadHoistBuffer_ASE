using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Data;
using NLog;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class ZoneDefDao:DaoBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public void insertZoneDef(DBConnection_EF conn, ZoneDef zonedef)
        {
            try
            {
                conn.ZoneDef.Add(zonedef);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public void DeleteZoneDef(DBConnection_EF conn, ZoneDef zonedef)
        {
            try
            {
                conn.ZoneDef.Remove(zonedef);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public void UpdateZoneDef(DBConnection_EF conn)
        {
            try
            {
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public List<ZoneDef> LoadZoneDef(DBConnection_EF conn)
        {
            try
            {
                var port = from a in conn.ZoneDef
                           select a;
                return port.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public IQueryable getQueryAllSQL(DBConnection_EF conn)
        {
            try
            {
                var port = from a in conn.ZoneDef
                           select a;
                return port;
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public ZoneDef LoadZoneDefByID(DBConnection_EF conn, string zoneid)
        {
            try
            {
                var result = conn.ZoneDef
                    .Where(x => x.ZoneID == zoneid)
                    .FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
    }
}
