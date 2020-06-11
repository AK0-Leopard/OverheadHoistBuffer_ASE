//*********************************************************************************
//      PortDao.cs
//*********************************************************************************
// File Name: PortDao.cs
// Description: Port DAO
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
    /// Class PortDao.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.DaoBase" />
    public class PortDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Inserts the port.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="port">The port.</param>
        public void insertPort(DBConnection_EF conn, APORT port)
        {
            try
            {
                conn.APORT.Add(port);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Updates the port.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="port">The port.</param>
        public void updatePort(DBConnection_EF conn, APORT port)
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
        /// Gets the port by port identifier.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="readLock">The read lock.</param>
        /// <param name="port_id">The port_id.</param>
        /// <returns>Port.</returns>
        public APORT getPortByPortID(DBConnection_EF conn, Boolean readLock, string port_id)
        {
            APORT rtnPort = null;
            try
            {
                var query = from port in conn.APORT
                            where port.PORT_ID == port_id.Trim()
                            select port;
                rtnPort = query.SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnPort;
        }

        /// <summary>
        /// Loads the port list by eqpt.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <returns>List&lt;Port&gt;.</returns>
        public List<APORT> loadPortListByEqpt(DBConnection_EF conn, string eqpt_id)
        {
            List<APORT> rtnList = new List<APORT>();
            try
            {
                var query = from port in conn.APORT
                            where port.EQPT_ID == eqpt_id.Trim()
                            select port;
                rtnList = query.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnList;
        }

        public void add(DBConnection_EF con, APORTSTATION port)
        {
            con.APORTSTATION.Add(port);
            con.SaveChanges();
        }

        public void update(DBConnection_EF con, APORTSTATION port)
        {
            con.SaveChanges();
        }

        public APORTSTATION getByID(DBConnection_EF con, String port_id)
        {
            var query = from port in con.APORTSTATION
                        where port.PORT_ID == port_id.Trim()
                        select port;
            return query.SingleOrDefault();
        }

        public APORTSTATION getByAdrID(DBConnection_EF con, String adr_id)
        {
            var query = from port in con.APORTSTATION
                        where port.ADR_ID == adr_id.Trim()
                        select port;
            return query.FirstOrDefault();
        }

        public List<APORTSTATION> loadAll(DBConnection_EF con)
        {
            var query = from port in con.APORTSTATION
                        orderby port.PORT_ID
                        select port;
            return query.ToList();
        }

        public List<APORTSTATION> loadPortStationByAdrs(DBConnection_EF con, List<string> adrs)
        {
            var query = from port in con.APORTSTATION
                        where adrs.Contains(port.ADR_ID)
                        orderby port.PORT_ID
                        select port;
            return query.ToList();
        }
    }
}
