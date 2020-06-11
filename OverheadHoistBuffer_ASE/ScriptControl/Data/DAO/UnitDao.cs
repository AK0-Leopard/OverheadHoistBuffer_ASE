//*********************************************************************************
//      UnitDao.cs
//*********************************************************************************
// File Name: UnitDao.cs
// Description: Unit DAO
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
    /// Class UnitDao.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.DaoBase" />
    public class UnitDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Inserts the unit.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="unit">The unit.</param>
        public void insertUnit(DBConnection_EF conn, AUNIT unit)
        {
            try
            {
                conn.AUNIT.Add(unit);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Updates the unit.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="unit">The unit.</param>
        public void updateUnit(DBConnection_EF conn, AUNIT unit)
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
        /// Gets the unit by unit identifier.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="readLock">The read lock.</param>
        /// <param name="unit_id">The unit_id.</param>
        /// <returns>Unit.</returns>
        public AUNIT getUnitByUnitID(DBConnection_EF conn, Boolean readLock, string unit_id)
        {
            AUNIT rtnUnit = null;
            try
            {
                var query = from unit in conn.AUNIT
                            where unit.UNIT_ID == unit_id.Trim()
                            select unit;
                rtnUnit = query.SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnUnit;
        }

        /// <summary>
        /// Loads the unit list by eqpt.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <returns>List&lt;Unit&gt;.</returns>
        public List<AUNIT> loadUnitListByEqpt(DBConnection_EF conn, string eqpt_id)
        {
            List<AUNIT> rtnList = new List<AUNIT>();
            try
            {
                var query = from unit in conn.AUNIT
                            where unit.EQPT_ID == eqpt_id.Trim()
                            select unit;
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
