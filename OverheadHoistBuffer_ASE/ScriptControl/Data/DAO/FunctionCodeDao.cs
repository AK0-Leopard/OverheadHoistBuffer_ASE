// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="FunctionCodeDao.cs" company="">
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
    /// Class FunctionCodeDao.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.DaoBase" />
    public class FunctionCodeDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Creates the function code.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="funcCode">The function code.</param>
        public void createFunctionCode(DBConnection_EF conn, UASFNC funcCode)
        {
            try
            {
                conn.UASFNC.Add(funcCode);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="funcCode">The function code.</param>
        public void updateUser(DBConnection_EF conn, UASFNC funcCode)
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
        /// Gets the function code.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="readLock">The read lock.</param>
        /// <param name="func_code">The func_code.</param>
        /// <returns>FunctionCode.</returns>
        public UASFNC getFunctionCode(DBConnection_EF conn, Boolean readLock, string func_code)
        {
            UASFNC funcCode = null;
            try
            {
                var query = from fun_code in conn.UASFNC
                            where fun_code.FUNC_CODE == func_code.Trim()
                            select fun_code;
                funcCode = query.SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return funcCode;
        }

        /// <summary>
        /// Loads all function code.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <returns>List&lt;FunctionCode&gt;.</returns>
        public List<UASFNC> loadAllFunctionCode(DBConnection_EF conn)
        {
            List<UASFNC> funcList = new List<UASFNC>();
            try
            {
                var query = from fun_code in conn.UASFNC
                            select fun_code;

                funcList = query.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return funcList;
        }

        /// <summary>
        /// Deletes the function code by code.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="func_code">The func_code.</param>
        public void deleteFunctionCodeByCode(DBConnection_EF conn, string func_code)
        {
            UASFNC funcCode = null;
            try
            {
                var query = from fun_code in conn.UASFNC
                            where fun_code.FUNC_CODE == func_code.Trim()
                            select fun_code;
                funcCode = query.SingleOrDefault();

                conn.UASFNC.Remove(funcCode);
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
