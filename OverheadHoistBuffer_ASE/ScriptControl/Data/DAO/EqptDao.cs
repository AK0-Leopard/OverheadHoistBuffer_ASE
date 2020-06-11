//*********************************************************************************
//      EqptDao.cs
//*********************************************************************************
// File Name: EqptDao.cs
// Description: Equipment DAO
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
    /// Class EqptDao.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.DaoBase" />
    public class EqptDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Inserts the eqpt.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="eqpt">The eqpt.</param>
        public void insertEqpt(DBConnection_EF conn, AEQPT eqpt)
        {
            try
            {
                conn.AEQPT.Add(eqpt);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Updates the eqpt.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="eqpt">The eqpt.</param>
        public void updateEqpt(DBConnection_EF conn, AEQPT eqpt)
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
        /// Gets the eqpt by eqpt identifier.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="readLock">The read lock.</param>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <returns>Equipment.</returns>
        public AEQPT getEqptByEqptID(DBConnection_EF conn, Boolean readLock, string eqpt_id)
        {
            AEQPT rtnEqpt = null;
            try
            {
                var query = from rptid in conn.AEQPT
                            where rptid.EQPT_ID == eqpt_id.Trim()
                            select rptid;
                rtnEqpt = query.SingleOrDefault();

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            return rtnEqpt;
        }

        /// <summary>
        /// Loads the eqpt list by node.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="node_id">The node_id.</param>
        /// <returns>List&lt;Equipment&gt;.</returns>
        public List<AEQPT> loadEqptListByNode(DBConnection_EF conn, string node_id)
        {
            List<AEQPT> rtnList = new List<AEQPT>();
            try
            {
                var query = from rptid in conn.AEQPT
                            where rptid.NODE_ID == node_id.Trim()
                            select rptid;
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
