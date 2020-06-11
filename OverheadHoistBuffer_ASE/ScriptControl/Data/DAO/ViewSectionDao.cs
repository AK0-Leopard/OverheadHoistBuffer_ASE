//*********************************************************************************
//      AlarmDao.cs
//*********************************************************************************
// File Name: AlarmDao.cs
// Description: AlarmDao類別
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2014/03/05    Hayes Chen     N/A            N/A     Initial Release
// 2014/04/02    Miles Chen     N/A            A0.01   Modify Functions for UI Use
// 
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Data;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    /// <summary>
    /// Class AlarmDao.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.DaoBase" />
    public class ViewSectionDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();
        

        /// <summary>
        /// Updates the alarm.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="alarm">The alarm.</param>
        public List<VSECTION_100> loadReleaseData(DBConnection_EF conn)
        {
            try
            {
                var query = from entity in conn.VSECTION_100
                            select entity;
                return query.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public int getReleaseDataCount(DBConnection_EF conn)
        {
            try
            {
                var query = from entity in conn.VSECTION_100
                            select entity;
                return query.Count();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }







    }
}
