// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="BCStatusDao.cs" company="">
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
    /// Class BCStatusDao.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.DaoBase" />
    public class BCStatusDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Inserts the bc stat.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="bc">The bc.</param>
        public void insertBCStat(DBConnection_EF conn, BCSTAT bc)
        {
            try
            {
                conn.BCSTAT.Add(bc);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Updates the bc stat.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="bc">The bc.</param>
        public void updateBCStat(DBConnection_EF conn, BCSTAT bc)
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
        /// Gets the bc stat by identifier.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="readLock">The read lock.</param>
        /// <param name="bc_id">The bc_id.</param>
        /// <returns>BCStatus.</returns>
        public BCSTAT getBCStatByID(DBConnection_EF conn, string bc_id)
        {
            BCSTAT rtnBC = null;
            try
            {
                var query = from bc in conn.BCSTAT
                            where bc.BC_ID == bc_id.Trim()
                            select bc;
                rtnBC = query.SingleOrDefault();

            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnBC;
        }

        /// <summary>
        /// Deletes the bc stat by identifier.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="bc_id">The bc_id.</param>
        public void deleteBCStatByID(DBConnection_EF conn, string bc_id)
        {
            try
            {
                var query = from bc in conn.BCSTAT
                            where bc.BC_ID == bc_id.Trim()
                            select bc;
                BCSTAT bcStat = query.SingleOrDefault();
                if (bcStat != null)
                {
                    conn.BCSTAT.Remove(bcStat);
                    conn.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex, "Delete BC Status");
                throw;
            }
        }

    }
}
