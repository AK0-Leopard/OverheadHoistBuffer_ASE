//*********************************************************************************
//      BufferDao.cs
//*********************************************************************************
// File Name: BufferDao.cs
// Description: Buffer DAO
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2014/02/26    Hayes Chen     N/A            N/A     Initial Release
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
    /// Class BufferPortDao.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.DaoBase" />
    public class BufferPortDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Inserts the buffer.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="buff">The buff.</param>
        public void insertBuffer(DBConnection_EF conn, ABUFFER buff)
        {
            try
            {
                conn.ABUFFER.Add(buff);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Updates the buffer.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="buff">The buff.</param>
        public void updateBuffer(DBConnection_EF conn, ABUFFER buff)
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
        /// Gets the buffer by buffer identifier.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="readLock">The read lock.</param>
        /// <param name="buff_id">The buff_id.</param>
        /// <returns>BufferPort.</returns>
        public ABUFFER getBufferByBufferID(DBConnection_EF conn, Boolean readLock, string buff_id)
        {
            ABUFFER rtnBufferPort = null;
            try
            {
                var query = from buffer in conn.ABUFFER
                            where buffer.BUFF_ID == buff_id.Trim()
                            select buffer;
                rtnBufferPort = query.SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnBufferPort;
        }

        /// <summary>
        /// Loads the buffer list by eqpt.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <returns>List&lt;BufferPort&gt;.</returns>
        public List<ABUFFER> loadBufferListByEqpt(DBConnection_EF conn, string eqpt_id)
        {
            List<ABUFFER> rtnList = new List<ABUFFER>();
            try
            {
                var query = from buffer in conn.ABUFFER
                            where buffer.EQPT_ID == eqpt_id.Trim()
                            select buffer;
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
