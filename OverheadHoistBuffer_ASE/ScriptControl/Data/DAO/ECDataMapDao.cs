// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="ECDataMapDao.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    /// <summary>
    /// Class ECDataMapDao.
    /// </summary>
    public class ECDataMapDao
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Loads the default ec data maps by eq real identifier.
        /// </summary>
        /// <param name="eqpt_real_id">The eqpt_real_id.</param>
        /// <returns>List&lt;ECDataMap&gt;.</returns>
        public List<ECDataMap> loadDefaultECDataMapsByEQRealID(string eqpt_real_id)
        {
            try
            {
                DataTable dt = SCApplication.getInstance().OHxCConfig.Tables["ECDATAMAP"];
                var query = from c in dt.AsEnumerable()
                            where c.Field<string>("EQPT_REAL_ID").Trim() == eqpt_real_id.Trim()
                            select new ECDataMap
                            {
                                EQPT_REAL_ID = c.Field<string>("EQPT_REAL_ID"),
                                ECID = c.Field<string>("ECID"),
                                ECNAME = c.Field<string>("ECNAME"),
                                ECMIN = c.Field<string>("ECMIN"),
                                ECMAX = c.Field<string>("ECMAX"),
                                ECV = c.Field<string>("ECV")
                            };
                return query.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Loads the default by ecid.
        /// </summary>
        /// <param name="ecidList">The ecid list.</param>
        /// <returns>List&lt;ECDataMap&gt;.</returns>
        public List<ECDataMap> loadDefaultByECID(List<string> ecidList)
        {
            try
            {
                DataTable dt = SCApplication.getInstance().OHxCConfig.Tables["ECDATAMAP"];
                var query = from c in dt.AsEnumerable()
                            where ecidList.Contains(c.Field<string>("ECID").Trim())
                            select new ECDataMap
                            {
                                EQPT_REAL_ID = c.Field<string>("EQPT_REAL_ID"),
                                ECID = c.Field<string>("ECID"),
                                ECNAME = c.Field<string>("ECNAME"),
                                ECMIN = c.Field<string>("ECMIN"),
                                ECMAX = c.Field<string>("ECMAX"),
                                ECV = c.Field<string>("ECV")
                            };
                return query.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Loads all default ec data.
        /// </summary>
        /// <returns>List&lt;ECDataMap&gt;.</returns>
        public List<ECDataMap> loadAllDefaultECData()
        {
            try
            {
                DataTable dt = SCApplication.getInstance().OHxCConfig.Tables["ECDATAMAP"];
                var query = from c in dt.AsEnumerable()
                            select new ECDataMap
                            {
                                EQPT_REAL_ID = c.Field<string>("EQPT_REAL_ID"),
                                ECID = c.Field<string>("ECID"),
                                ECNAME = c.Field<string>("ECNAME"),
                                ECMIN = c.Field<string>("ECMIN"),
                                ECMAX = c.Field<string>("ECMAX"),
                                ECV = c.Field<string>("ECV")
                            };
                return query.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the default by ecid.
        /// </summary>
        /// <param name="ecid">The ecid.</param>
        /// <returns>ECDataMap.</returns>
        public ECDataMap getDefaultByECID(string ecid)
        {
            try
            {
                DataTable dt = SCApplication.getInstance().OHxCConfig.Tables["SVDATAMAP"];
                var query = from c in dt.AsEnumerable()
                            where c.Field<string>("ECID").Trim() == ecid.Trim().Trim()
                            select new ECDataMap
                            {
                                EQPT_REAL_ID = c.Field<string>("EQPT_REAL_ID"),
                                ECID = c.Field<string>("ECID"),
                                ECNAME = c.Field<string>("ECNAME"),
                                ECMIN = c.Field<string>("ECMIN"),
                                ECMAX = c.Field<string>("ECMAX"),
                                ECV = c.Field<string>("ECV")
                            };
                return query.SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Updates the ec data.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="ecData">The ec data.</param>
        public void updateECData(DBConnection_EF conn, AECDATAMAP ecData)
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

        /// <summary>
        /// Inserts the ec data.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="ecData">The ec data.</param>
        public void insertECData(DBConnection_EF conn, AECDATAMAP ecData)
        {
            try
            {
                conn.AECDATAMAP.Add(ecData);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Deletes the ec data.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="ecidList">The ecid list.</param>
        public void deleteECData(DBConnection_EF conn, List<string> ecidList)
        {
            try
            {
                var query = from ecData in conn.AECDATAMAP
                            //where ecidList.Any(ecid => ecData.ECID.Contains(ecid))
                            where ecidList.Contains(ecData.ECID.Trim())
                            select ecData;
                conn.AECDATAMAP.RemoveRange(query);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Loads the ec data maps by eq real identifier.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="eqpt_real_id">The eqpt_real_id.</param>
        /// <returns>List&lt;ECDataMap&gt;.</returns>
        public List<AECDATAMAP> loadECDataMapsByEQRealID(DBConnection_EF conn, string eqpt_real_id)
        {
            List<AECDATAMAP> rtnList = new List<AECDATAMAP>();
            try
            {
                var query = from ecData in conn.AECDATAMAP
                            where ecData.EQPT_REAL_ID == eqpt_real_id.Trim()
                            select ecData;
                rtnList = query.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnList;
        }

        /// <summary>
        /// Loads the by ecid.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="ecidList">The ecid list.</param>
        /// <returns>List&lt;ECDataMap&gt;.</returns>
        public List<AECDATAMAP> loadByECID(DBConnection_EF conn, List<string> ecidList)
        {
            List<AECDATAMAP> rtnList = null;
            try
            {
                var query = from ecData in conn.AECDATAMAP
                            //where ecidList.Any(ecid => ecData.ECID.Contains(ecid))
                            where ecidList.Contains(ecData.ECID.Trim())
                            select ecData;
                rtnList = query.ToList();

            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnList;
        }

        /// <summary>
        /// Loads all ec data.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <returns>List&lt;ECDataMap&gt;.</returns>
        public List<AECDATAMAP> loadAllECData(DBConnection_EF conn)
        {
            List<AECDATAMAP> rtnList = new List<AECDATAMAP>();
            try
            {
                var query = from ecData in conn.AECDATAMAP
                            select ecData;
                rtnList = query.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnList;
        }

        /// <summary>
        /// Gets the by ecid.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="readLock">if set to <c>true</c> [read lock].</param>
        /// <param name="ecid">The ecid.</param>
        /// <returns>ECDataMap.</returns>
        public AECDATAMAP getByECID(DBConnection_EF conn, bool readLock, string ecid)
        {
            AECDATAMAP rtnItem = null;
            try
            {
                var query = from ecData in conn.AECDATAMAP
                            where ecData.ECID == ecid.Trim()
                            select ecData;
                rtnItem = query.SingleOrDefault();

            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnItem;
        }
    }
}
