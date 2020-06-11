// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="CrateDao.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Data;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    /// <summary>
    /// Class CrateDao.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.DaoBase" />
    public class CrateDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Inserts the crate.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="crt">The CRT.</param>
        public void insertCrate(DBConnection_EF conn, ACRATE crt)
        {
            try
            {
                conn.ACRATE.Add(crt);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Updates the crate.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="crt">The CRT.</param>
        public void updateCrate(DBConnection_EF conn, ACRATE crt)
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
        /// Gets the crate.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="readLock">The read lock.</param>
        /// <param name="crate_id">The crate_id.</param>
        /// <returns>Crate.</returns>
        public ACRATE getCrate(DBConnection_EF conn, Boolean readLock, string crate_id)
        {
            ACRATE rtnCRT = null;
            try
            {
                var query = from crate in conn.ACRATE
                            where crate.CRATE_ID == crate_id.Trim()
                            select crate;
                rtnCRT = query.SingleOrDefault();

            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnCRT;
        }

        /// <summary>
        /// Deletes the crate.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="crate_id">The crate_id.</param>
        public void deleteCrate(DBConnection_EF conn, string crate_id)
        {
            ACRATE deleteCRT = null;
            try
            {
                var query = from crate in conn.ACRATE
                            where crate.CRATE_ID == crate_id.Trim()
                            select crate;
                deleteCRT = query.FirstOrDefault();
                conn.ACRATE.Remove(deleteCRT);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
    }
}
