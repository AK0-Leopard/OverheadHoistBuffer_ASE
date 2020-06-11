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
    public class HIDZoneMasterDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public AHIDZONEMASTER getHidZoneMaster(DBConnection_EF conn,string entry_sec_id,string leave_adr)
        {
            AHIDZONEMASTER hid_zone_master = null;
            try
            {
                var query = from zone in conn.AHIDZONEMASTER
                            where zone.ENTRY_SEC_ID.Trim() == entry_sec_id.Trim() &&
                                  (zone.LEAVE_ADR_ID_1.Trim() == leave_adr.Trim() || zone.LEAVE_ADR_ID_2.Trim() == leave_adr.Trim())
                            select zone;
                hid_zone_master = query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return hid_zone_master;
        }

        public List<AHIDZONEMASTER> loadAllHIDZoneMaster(DBConnection_EF conn)
        {
            List<AHIDZONEMASTER> rtnList = null;
            try
            {
                var query = from zone in conn.AHIDZONEMASTER
                            select zone;
                rtnList = query.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnList;
        }

        public List<string> loadAllLeaveAdr(DBConnection_EF con)
        {
            List<string> lstAdr = con.AHIDZONEMASTER.Select(zone => zone.LEAVE_ADR_ID_1.Trim()).ToList();
            lstAdr.AddRange(con.AHIDZONEMASTER.Select(zone => zone.LEAVE_ADR_ID_2.Trim()).ToList());
            return lstAdr;
        }


    }
}
