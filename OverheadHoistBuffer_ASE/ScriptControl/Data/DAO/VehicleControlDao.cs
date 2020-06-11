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
    public class VehicleControlDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Inserts the alarm.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="entity">The alarm.</param>
        public void insert(DBConnection_EF conn, AVEHICLE_CONTROL_100 entity)
        {
            try
            {
                conn.AVEHICLE_CONTROL_100.Add(entity);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// Updates the alarm.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="alarm">The alarm.</param>
        public void update(DBConnection_EF conn, AVEHICLE_CONTROL_100 entity)
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
        /// Updates the alarm.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="alarm">The alarm.</param>
        public AVEHICLE_CONTROL_100 getReleaseDataByVhID(DBConnection_EF conn, string vh_id)
        {
            try
            {
                var query = from entity in conn.AVEHICLE_CONTROL_100
                            where entity.VEHICLE_ID == vh_id.Trim()
                            select entity;
                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        /// <summary>
        /// Updates the alarm.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="alarm">The alarm.</param>
        public List<AVEHICLE_CONTROL_100> loadAllReleaseData(DBConnection_EF conn)
        {
            try
            {
                var query = from entity in conn.AVEHICLE_CONTROL_100
                            select entity;
                return query.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }







    }
}
