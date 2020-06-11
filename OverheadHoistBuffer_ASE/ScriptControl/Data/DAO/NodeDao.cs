//*********************************************************************************
//      NodeDao.cs
//*********************************************************************************
// File Name: NodeDao.cs
// Description: Node DAO
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
    /// Class NodeDao.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.DaoBase" />
    public class NodeDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Inserts the node.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="node">The node.</param>
        public void insertNode(DBConnection_EF conn, ANODE node)
        {
            try
            {
                conn.ANODE.Add(node);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Updates the node.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="node">The node.</param>
        public void updateNode(DBConnection_EF conn, ANODE node)
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
        /// Gets the node by node identifier.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="readLock">The read lock.</param>
        /// <param name="node_id">The node_id.</param>
        /// <returns>Node.</returns>
        public ANODE getNodeByNodeID(DBConnection_EF conn, Boolean readLock, string node_id)
        {
            ANODE rtnNode = null;
            try
            {
                var query = from node in conn.ANODE
                            where node.NODE_ID == node_id.Trim()
                            select node;
                rtnNode = query.SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnNode;
        }

        /// <summary>
        /// Loads the node list by zone.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="zone_id">The zone_id.</param>
        /// <returns>List&lt;Node&gt;.</returns>
        public List<ANODE> loadNodeListByZone(DBConnection_EF conn, string zone_id) 
        {
            List<ANODE> rtnList = new List<ANODE>();
            try 
            {
                var query = from node in conn.ANODE
                            where node.ZONE_ID == zone_id.Trim()
                            select node;
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
