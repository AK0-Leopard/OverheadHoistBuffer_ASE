// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="TraceSetDao.cs" company="">
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
    /// Class TraceSetDao.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.DaoBase" />
    public class TraceSetDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Inserts the trace set.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="traceSet">The trace set.</param>
        public void insertTraceSet(DBConnection_EF conn, ATRACESET traceSet)
        {
            try
            {
                conn.ATRACESET.Add(traceSet);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Updates the trace set.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="traceSet">The trace set.</param>
        public void updateTraceSet(DBConnection_EF conn, ATRACESET traceSet)
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
        /// Gets the trace set.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="readLock">The read lock.</param>
        /// <param name="trace_id">The trace_id.</param>
        /// <returns>TraceSet.</returns>
        public ATRACESET getTraceSet(DBConnection_EF conn, Boolean readLock, string trace_id)
        {
            ATRACESET traceSet = null;
            try
            {
                var query = from trace in conn.ATRACESET
                            where trace.TRACE_ID == trace_id.Trim()
                            select trace;
                traceSet = query.SingleOrDefault();

            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return traceSet;
        }

        /// <summary>
        /// Loads the active trace set.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <returns>List&lt;TraceSet&gt;.</returns>
        public List<ATRACESET> loadActiveTraceSet(DBConnection_EF conn)
        {
            List<ATRACESET> traceSetList = null;
            try
            {
                var query = from trace in conn.ATRACESET
                            where trace.TOTAL_SMP_CNT > trace.SMP_CNT
                            || trace.TOTAL_SMP_CNT == -1
                            select trace;
                traceSetList = query.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return traceSetList;
        }

        /// <summary>
        /// Deletes the trace item.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="trace_id">The trace_id.</param>
        public void deleteTraceItem(DBConnection_EF conn, string trace_id)
        {
            try
            {
                List<ATRACEITEM> traceItems = loadTraceItem(conn, trace_id);
                conn.ATRACEITEM.RemoveRange(traceItems);
                conn.SaveChanges();


            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Inserts the trace item.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="traceItem">The trace item.</param>
        public void insertTraceItem(DBConnection_EF conn, ATRACEITEM traceItem)
        {
            try
            {
                conn.ATRACEITEM.Add(traceItem);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Loads the trace item.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="trace_id">The trace_id.</param>
        /// <returns>List&lt;TraceItem&gt;.</returns>
        public List<ATRACEITEM> loadTraceItem(DBConnection_EF conn, string trace_id)
        {
            List<ATRACEITEM> rtnList = new List<ATRACEITEM>();
            try
            {
                var query = from traceItem in conn.ATRACEITEM
                            where traceItem.TRACE_ID == trace_id.Trim()
                            select traceItem;
                rtnList = query.ToList();

            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnList;
        }

    }
}
