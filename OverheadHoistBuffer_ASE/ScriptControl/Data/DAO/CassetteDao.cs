//*********************************************************************************
//      CassetteDao.cs
//*********************************************************************************
// File Name: CassetteDao.cs
// Description: CassetteDao類別
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2014/03/31    Hayes Chen     N/A            N/A     Initial Release
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
    /// Class CassetteDao.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.DaoBase" />
    public class CassetteDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Inserts the cassette.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="cst">The CST.</param>
        public void insertCassette(DBConnection_EF conn, ACASSETTE cst)
        {
            try
            {
                conn.ACASSETTE.Add(cst);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Updates the cassette.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="cst">The CST.</param>
        public void updateCassette(DBConnection_EF conn, ACASSETTE cst)
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
        /// Gets the cassette.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="readLock">The read lock.</param>
        /// <param name="cst_id">The cst_id.</param>
        /// <returns>Cassette.</returns>
        public ACASSETTE getCassette(DBConnection_EF conn, Boolean readLock, string cst_id)
        {
            ACASSETTE rtnCST = null;
            try
            {
                var query = from cst in conn.ACASSETTE
                            where cst.CST_ID == cst_id.Trim()
                            select cst;
                rtnCST = query.SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnCST;
        }

        /// <summary>
        /// Gets the cassette by port identifier.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="port_id">The port_id.</param>
        /// <returns>Cassette.</returns>
        public ACASSETTE getCassetteByPortID(DBConnection_EF conn, string port_id)
        {
            ACASSETTE rtnCST = null;
            try
            {
                var query = from cst in conn.ACASSETTE
                            where cst.PORT_ID == port_id.Trim()
                            select cst;
                rtnCST = query.SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnCST;
        }


        /// <summary>
        /// Deletes the cassette.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="cst_id">The cst_id.</param>
        public void deleteCassette(DBConnection_EF conn, string cst_id)
        {
            ACASSETTE CSTTemp = null;
            try
            {
                var query = from cst in conn.ACASSETTE
                            where cst.CST_ID == cst_id.Trim()
                            select cst;
                CSTTemp = query.SingleOrDefault();
                conn.ACASSETTE.Remove(CSTTemp);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Deletes all cassette by port.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="port_id">The port_id.</param>
        public void deleteAllCassetteByPort(DBConnection_EF conn, string port_id)
        {
            try
            {
                var query = from cst in conn.ACASSETTE
                            where cst.PORT_ID == port_id.Trim()
                            select cst;
                conn.ACASSETTE.RemoveRange(query);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

    }
}
