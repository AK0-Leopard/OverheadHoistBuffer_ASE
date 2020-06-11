//*********************************************************************************
//      ZoneDao.cs
//*********************************************************************************
// File Name: ZoneDao.cs
// Description: Zone DAO
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
    /// Class ZoneDao.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.DaoBase" />
    public class HIDZoneQueueDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public void add(DBConnection_EF con, AHIDZONEQUEUE hid)
        {
            con.AHIDZONEQUEUE.Add(hid);
            con.SaveChanges();
        }

        public void Update(DBConnection_EF con, AHIDZONEQUEUE hid)
        {
            con.SaveChanges();
        }

        public AHIDZONEQUEUE getUsingHIDZoneQueue(DBConnection_EF conn, string vh_id)
        {
            AHIDZONEQUEUE queue = null;
            try
            {
                var query = from zone in conn.AHIDZONEQUEUE
                            where zone.VEHICLE_ID.Trim() == vh_id && zone.STATUS < E_HIDQueueStatus.Release
                            select zone;
                queue = query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return queue;
        }

        public AHIDZONEQUEUE getHIDZoneQueue_FirstReqInPasue(DBConnection_EF conn, string entry_sec_id)
        {
            AHIDZONEQUEUE queue = null;
            try
            {
                var query = from zone in conn.AHIDZONEQUEUE
                            where zone.ENTRY_SEC_ID.Trim() == entry_sec_id.Trim()
                            && zone.IS_PASUE
                            orderby zone.REQ_TIME
                            select zone;
                queue = query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return queue;
        }

        public AHIDZONEQUEUE getHIDZoneQueue(DBConnection_EF conn, string vh_id, string entry_sec_id)
        {
            AHIDZONEQUEUE queue = null;
            try
            {
                var query = from zone in conn.AHIDZONEQUEUE
                            where zone.VEHICLE_ID.Trim() == vh_id && zone.ENTRY_SEC_ID.Trim() == entry_sec_id.Trim()
                            select zone;
                queue = query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return queue;
        }





    }
}
