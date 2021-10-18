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
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System.Globalization;
using System.Data.Entity;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    /// <summary>
    /// Class AlarmDao.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.DaoBase" />
    public class VAlarmDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public List<VALARM> getAlarms(DBConnection_EF conn, string startTime, string endTime)
        {
            var alarm = from b in conn.VALARM
                        where b.RPT_DATE_TIME.CompareTo(startTime) > 0 &&
                              b.RPT_DATE_TIME.CompareTo(endTime) < 0
                        orderby b.RPT_DATE_TIME descending
                        select b;
            return alarm.ToList();
        }
        public int getAlarmsCount(DBConnection_EF conn, string startTime, string endTime)
        {
            var alarm = from b in conn.VALARM
                        where b.RPT_DATE_TIME.CompareTo(startTime) > 0 &&
                              b.RPT_DATE_TIME.CompareTo(endTime) < 0
                        select b;
            return alarm.Count();
        }


    }
}
