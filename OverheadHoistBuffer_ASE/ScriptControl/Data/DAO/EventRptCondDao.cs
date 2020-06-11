// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="EventRptCondDao.cs" company="">
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
    /// Class EventRptCondDao.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.DaoBase" />
    public class EventRptCondDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Inserts the RPT cond.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="cond">The cond.</param>
        public void insertRptCond(DBConnection_EF conn, AEVENTRPTCOND cond)
        {
            try
            {
                conn.AEVENTRPTCOND.Add(cond);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Updates the RPT cond.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="cond">The cond.</param>
        public void updateRptCond(DBConnection_EF conn, AEVENTRPTCOND cond)
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
        /// Deletes all RPT cond.
        /// </summary>
        /// <param name="conn">The connection.</param>
        public void deleteAllRptCond(DBConnection_EF conn)
        {
            try
            {
                var query = from rptid in conn.AEVENTRPTCOND
                            select rptid;

                conn.AEVENTRPTCOND.RemoveRange(query);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the RPT cond.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="readLock">The read lock.</param>
        /// <param name="ceid">The ceid.</param>
        /// <returns>EventRptCond.</returns>
        public AEVENTRPTCOND getRptCond(DBConnection_EF conn, string ceid)
        {
            AEVENTRPTCOND cond = null;
            try
            {
                var query = from rptid in conn.AEVENTRPTCOND
                            where rptid.CEID == ceid.Trim()
                            select rptid;
                cond = query.SingleOrDefault();

            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return cond;
        }

    }
}
