// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="AlarmRptCondDao.cs" company="">
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
    /// Class AlarmRptCondDao.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.DaoBase" />
    public class AlarmRptCondDao : DaoBase
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
        public void insertRptCond(DBConnection_EF conn, ALARMRPTCOND cond)
        {
            try
            {
                conn.ALARMRPTCOND.Add(cond);
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
        public void updateRptCond(DBConnection_EF conn, ALARMRPTCOND cond)
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
                var query = from rptCond in conn.ALARMRPTCOND
                            select rptCond;
                conn.ALARMRPTCOND.RemoveRange(query);
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
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <param name="alarm_code">The alarm_code.</param>
        /// <returns>AlarmRptCond.</returns>
        public ALARMRPTCOND getRptCond(DBConnection_EF conn, string alarm_code)
        {

            ALARMRPTCOND cond = null;
            try
            {
                var query = from rptCond in conn.ALARMRPTCOND
                            where rptCond.ALAM_CODE == alarm_code.Trim()
                            select rptCond;
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
