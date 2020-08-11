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
        /// <summary>
        /// 找出是UNK 但不是UNKU且在shelf 上的CST
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public List<CassetteData> LoadCassetteDataByCSTID_UNKandOnShelf(DBConnection_EF conn)
        {
            try
            {
                var port = from a in conn.CassetteData
                           where a.CSTID.Contains("UNK") && !a.CSTID.Contains("UNKU") 
                           &&  (a.Carrier_LOC.StartsWith("10") ||
                               a.Carrier_LOC.StartsWith("11") ||
                               a.Carrier_LOC.StartsWith("21") ||
                               a.Carrier_LOC.StartsWith("20"))
                           select a;
                return port.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        public List<CassetteData> LoadCassetteDataByCstAndEmptyLotID(DBConnection_EF conn)
        {
            try
            {
                var port = from a in conn.CassetteData
                           where string.IsNullOrWhiteSpace(a.CSTID) == false && string.IsNullOrWhiteSpace(a.LotID)
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
                           where a.CSTState !=  E_CSTState.Completed
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
                if(string.IsNullOrWhiteSpace(cstid))
                {
                    return null;
                }
                else
                {
                    var result = conn.CassetteData.Where(x => x.CSTID.Trim() == cstid.Trim()).FirstOrDefault();
                    return result;
                }
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

        public CassetteData LoadCassetteDataByDU_CSTID(DBConnection_EF conn, CassetteData cstData)
        {
            try
            {
                if (cstData == null)
                {
                    return null;
                }
                else
                {
                    var result = conn.CassetteData.Where
                        (x => x.CSTID.Trim() == cstData.CSTID.Trim() 
                        && x.Carrier_LOC.Trim() != cstData.Carrier_LOC.Trim()
                        ).FirstOrDefault();
                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        public CassetteData LoadCassetteDataByDU_BOXID(DBConnection_EF conn, CassetteData cstData)
        {
            try
            {
                if (cstData == null)
                {
                    return null;
                }
                else
                {
                    var result = conn.CassetteData.Where
                        (x => x.BOXID.Trim() == cstData.BOXID.Trim()
                        && x.Carrier_LOC.Trim() != cstData.Carrier_LOC.Trim()
                        ).FirstOrDefault();
                    return result;
                }
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
