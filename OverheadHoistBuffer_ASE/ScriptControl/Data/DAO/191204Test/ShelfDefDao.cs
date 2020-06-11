using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Data;
using NLog;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class ShelfDefDao : DaoBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public void insertShelfDef(DBConnection_EF conn, ShelfDef shelfdef)
        {
            try
            {
                conn.ShelfDef.Add(shelfdef);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public void DeleteShelfDef(DBConnection_EF conn, ShelfDef shelfdef)
        {
            try
            {
                conn.ShelfDef.Remove(shelfdef);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public void UpdateShelfDef(DBConnection_EF conn)
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

        public List<ShelfDef> LoadShelfDef(DBConnection_EF conn)
        {
            try
            {
                var port = from a in conn.ShelfDef
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
                var port = from a in conn.ShelfDef
                           select a;
                return port;
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }


        public List<ShelfDef> LoadDisableShelf(DBConnection_EF conn, string zoneid)
        {
            try
            {
                var result = from a in conn.ShelfDef
                             where a.Enable == "N" && a.ZoneID == zoneid
                             select a;
                return result.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public List<ShelfDef> LoadEnableShelf(DBConnection_EF conn)
        {
            try
            {
                var result = from a in conn.ShelfDef
                             where a.Enable == "Y"
                             select a;
                return result.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public List<ShelfDef> LoadDisableShelf(DBConnection_EF conn)
        {
            try
            {
                var result = from a in conn.ShelfDef
                             where a.Enable == "N"
                             select a;
                return result.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public List<ShelfDef> LoadShelfByZone(DBConnection_EF conn, string zoneid)
        {
            try
            {
                var result = conn.ShelfDef
                    .Where(x => x.Enable == "Y" && x.ZoneID == zoneid)
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public ShelfDef LoadShelfByID(DBConnection_EF conn, string shelfid)
        {
            try
            {
                var result = conn.ShelfDef
                    .Where(x => x.ShelfID == shelfid)
                    .FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        public List<ShelfDef> GetEmptyAndEnableShelfByZone(DBConnection_EF conn, string zoneID)
        {
            try
            {
                var result = conn.ShelfDef
                    .Where(x => x.ZoneID.Trim() == zoneID.Trim() &&
                                x.ShelfState == ShelfDef.E_ShelfState.EmptyShelf &&
                                x.Enable == "Y")
                    .OrderByDescending(x => x.ShelfID).ToList();
                    //.FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        public List<ShelfDef> GetEmptyAndEnableShelf(DBConnection_EF conn)
        {
            try
            {
                var result = conn.ShelfDef
                    .Where(x => x.ShelfState == ShelfDef.E_ShelfState.EmptyShelf &&
                                x.Enable == "Y")
                    .OrderByDescending(x => x.ShelfID).ToList();
                //.FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        public List<ShelfDef> GetReserveShelf(DBConnection_EF conn)  //取得不是空儲位的所有儲位
        {
            try
            {
                var result = conn.ShelfDef
                    .Where(x => x.ShelfState != ShelfDef.E_ShelfState.EmptyShelf)
                    .OrderByDescending(x => x.ShelfID).ToList();
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
