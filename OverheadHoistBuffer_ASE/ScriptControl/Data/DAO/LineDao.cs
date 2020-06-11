//*********************************************************************************
//      LineDao.cs
//*********************************************************************************
// File Name: LineDao.cs
// Description: Line DAO
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2014/02/19    Hayes Chen     N/A            N/A     Initial Release
//
//**********************************************************************************
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
    /// Class LineDao.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.DaoBase" />
    public class LineDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Inserts the line.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="line">The line.</param>
        public void insertLine(DBConnection_EF conn, ALINE line)
        {
            try
            {
                //session.SaveOrUpdate(line);
                conn.ALINE.Add(line);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Updates the line.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="line">The line.</param>
        public void updateLine(DBConnection_EF conn, ALINE line)
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
        /// Gets the first line.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="readLock">The read lock.</param>
        /// <returns>Line.</returns>
        public ALINE getFirstLine(DBConnection_EF conn, Boolean readLock)
        {
            ALINE rtnLine = null;
            try
            {
                var query = from line in conn.ALINE
                            select line;
                rtnLine = query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnLine;
        }

        /// <summary>
        /// Gets the line by identifier.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="readLock">The read lock.</param>
        /// <param name="line_id">The line_id.</param>
        /// <returns>Line.</returns>
        public ALINE getLineByID(DBConnection_EF conn, Boolean readLock, string line_id)
        {
            //IQuery query = session.CreateQuery("From ALINE");
            ALINE rtnLine = null;
            try
            {
                var query = from line in conn.ALINE
                            where line.LINE_ID == line_id.Trim()
                            select line;
                rtnLine = query.SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnLine;
        }

        /// <summary>
        /// Loads all line in database.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <returns>List&lt;Line&gt;.</returns>
        public List<ALINE> loadAllLineInDB(DBConnection_EF conn)
        {
            List<ALINE> rtnLine = new List<ALINE>();
            try
            {
                var query = from line in conn.ALINE
                            select line;
                rtnLine = query.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnLine;
        }

        /// <summary>
        /// Updates the line host mode.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="line_id">The line_id.</param>
        /// <param name="host_mode">The host_mode.</param>
        public void updateLineHostMode(DBConnection_EF conn, string line_id, int host_mode)
        {
            try
            {
                ALINE line = getLineByID(conn, true, line_id);
                line.HOST_MODE = host_mode;
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Updates the line status.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="line_id">The line_id.</param>
        /// <param name="line_stat">The line_stat.</param>
        public void updateLineStatus(DBConnection_EF conn, string line_id, int line_stat)
        {
            try
            {
                ALINE line = getLineByID(conn, true, line_id);
                line.LINE_STAT = line_stat;
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Deletes the line by line identifier.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="line_id">The line_id.</param>
        public void deleteLineByLineID(DBConnection_EF conn, string line_id)
        {
            try
            {
                ALINE line = getLineByID(conn, false, line_id);
                conn.ALINE.Remove(line);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Deletes all line.
        /// </summary>
        /// <param name="conn">The connection.</param>
        public void deleteAllLine(DBConnection_EF conn)
        {
            try
            {
                List<ALINE> allLines = loadAllLineInDB(conn);
                foreach (ALINE line in allLines)
                {
                    conn.ALINE.Remove(line);
                    conn.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
    }
}
