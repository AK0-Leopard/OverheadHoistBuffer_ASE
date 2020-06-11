//*********************************************************************************
//      FlowRelDao.cs
//*********************************************************************************
// File Name: FlowRelDao.cs
// Description: Flow Relation DAO
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
    /// Class FlowRelDao.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.DaoBase" />
    public class FlowRelDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Inserts the flow relative.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="flowRel">The flow relative.</param>
        public void insertFlowRel(DBConnection_EF conn, AFLOW_REL flowRel)
        {
            try
            {
                conn.AFLOW_REL.Add(flowRel);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Updates the flow relative.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="flowRel">The flow relative.</param>
        public void updateFlowRel(DBConnection_EF conn, AFLOW_REL flowRel)
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
        /// Gets the flow relative by identifier.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="readLock">The read lock.</param>
        /// <param name="fr_id">The fr_id.</param>
        /// <returns>FlowRelationItem.</returns>
        public AFLOW_REL getFlowRelByID(DBConnection_EF conn, Boolean readLock, string fr_id)
        {
            AFLOW_REL rtnFR = null;
            try
            {

                var query = from flowRel in conn.AFLOW_REL
                            where flowRel.FR_ID == fr_id.Trim()
                            select flowRel;
                rtnFR= query.SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnFR;
        }

        /// <summary>
        /// Loads the flow relative by upstream.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="upstream_id">The upstream_id.</param>
        /// <returns>List&lt;FlowRelationItem&gt;.</returns>
        public List<AFLOW_REL> loadFlowRelByUpstream(DBConnection_EF conn, string upstream_id)
        {
            List<AFLOW_REL> rtnList = new List<AFLOW_REL>();
            try
            {
                var query = from flowRel in conn.AFLOW_REL
                            where flowRel.UPSTREAM_ID == upstream_id.Trim()
                            select flowRel;
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
        /// Loads all flow relative.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <returns>List&lt;FlowRelationItem&gt;.</returns>
        public List<AFLOW_REL> loadAllFlowRel(DBConnection_EF conn) 
        {
            List<AFLOW_REL> rtnList = new List<AFLOW_REL>();
            try 
            {
                var query = from flowRel in conn.AFLOW_REL
                            select flowRel;
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
