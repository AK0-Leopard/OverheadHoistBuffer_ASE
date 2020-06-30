using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Data;
using NLog;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class CassetteDataDao : DaoBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public void insertCassetteData(DBConnection_EF conn, CassetteData cassettedata)
        {
            try
            {
                conn.CassetteData.Add(cassettedata);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public void DeleteCassetteData(DBConnection_EF conn, CassetteData cassettedata)
        {
            try
            {
                conn.CassetteData.Remove(cassettedata);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public void UpdateCassetteData(DBConnection_EF conn)
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

        public List<CassetteData> LoadCassetteData(DBConnection_EF conn)
        {
            try
            {
                var port = from a in conn.CassetteData
                           select a;
                return port.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public List<CassetteData> LoadCassetteDataByCVPort(DBConnection_EF conn, string portName)
        {
            try
            {
                var port = from a in conn.CassetteData
                           where a.Carrier_LOC.Contains(portName.Trim())
                           select a;
                return port.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        public List<CassetteData> LoadCassetteDataByCSTID_UNK(DBConnection_EF conn)
        {
            try
            {
                var port = from a in conn.CassetteData
                           where a.CSTID.Contains("UNK")
                           select a;
                return port.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        public List<CassetteData> LoadCassetteDataByNotCompleted(DBConnection_EF conn)  //原本打算是要取得除了在 shelf 的所有帳
        {
            try
            {
                var port = from a in conn.CassetteData
                           where a.CSTState !=  E_CSTState.Completed && a.CSTState != E_CSTState.WaitOut
                           select a;
                return port.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        public List<CassetteData> loadCassetteDataIsUnfinished(DBConnection_EF conn)
        {
            try
            {
                var port = from a in conn.CassetteData.AsNoTracking()
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
                var port = from a in conn.CassetteData
                           select a;
                return port;
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public CassetteData LoadCassetteDataByShelfID(DBConnection_EF conn ,string shelfid)
        {
            try
            {
                var result = conn.CassetteData.Where(x => x.Carrier_LOC == shelfid).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public CassetteData LoadCassetteDataByCSTID(DBConnection_EF conn, string cstid)
        {
            try
            {
                var result = conn.CassetteData.Where(x => x.CSTID.Trim() == cstid.Trim()).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public CassetteData LoadCassetteDataByLoc(DBConnection_EF conn, string portName)
        {
            try
            {
                var result = conn.CassetteData.Where(x => x.Carrier_LOC.Trim() == portName.Trim()).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public List<CassetteData> LoadCassetteDataByOHCV(DBConnection_EF conn, string portName)
        {
            try
            {
                var result = conn.CassetteData.Where(x => x.Carrier_LOC.Contains(portName.Trim())).ToList();

                return result;
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public CassetteData LoadCassetteDataByBoxID(DBConnection_EF conn, string boxid)
        {
            try
            {
                var result = conn.CassetteData.Where(x => x.BOXID.Trim() == boxid.Trim()).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public List<string> GetOccupiedLocation(DBConnection_EF conn)
        {
            try
            {
                var loc = from a in conn.CassetteData
                           select a.Carrier_LOC;
                return loc.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
    }
}
