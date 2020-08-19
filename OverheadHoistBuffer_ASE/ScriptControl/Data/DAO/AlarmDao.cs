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
    public class AlarmDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        

        /// <summary>
        /// Inserts the alarm.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="alarm">The alarm.</param>
        public bool insertAlarm(DBConnection_EF conn, ALARM alarm)
        {
            try
            {
                conn.ALARM.Add(alarm);
                conn.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                return false;
                throw;                
            }
        }

        /// <summary>
        /// Updates the alarm.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="alarm">The alarm.</param>
        public void updateAlarm(DBConnection_EF conn, ALARM alarm)
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

        public void UpdateAllAlarmStatus2ClearByVhID(DBConnection_EF con, string vh_id)
        {
            string sql = "Update [ALARM] SET [ALAM_STAT] = {0} WHERE [EQPT_ID] = {1}";
            //con.Database.ExecuteSqlCommand(sql,, vh_id, seq_no);
        }

        public int getSetAlarmCountByEQAndCode(DBConnection_EF conn, string eq_id, string code)
        {
            try
            {
                var alarm = from b in conn.ALARM
                            where b.ALAM_CODE == code.Trim() &&
                                  b.EQPT_ID == eq_id &&
                                  b.ALAM_STAT == ProtocolFormat.OHTMessage.ErrorStatus.ErrSet
                            select b;
                return alarm.Count();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        public ALARM getSetAlarm(DBConnection_EF conn, string eq_id, string code)
        {
            var alarm = from b in conn.ALARM
                        where b.ALAM_CODE.Trim() == code.Trim() &&
                         b.EQPT_ID.Trim() == eq_id.Trim() &&
                         b.ALAM_STAT == ProtocolFormat.OHTMessage.ErrorStatus.ErrSet
                        select b;
            return alarm.FirstOrDefault();
        }
        public List<ALARM> loadAllAlarm(DBConnection_EF conn)
        {
            try
            {
                var alarm = from a in conn.ALARM
                            orderby a.RPT_DATE_TIME descending
                            select a;
                return alarm.Take(1000).ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        public List<ALARM> loadSetAlarm(DBConnection_EF conn, string eq_id)
        {
            try
            {
                var alarm = from a in conn.ALARM
                            where a.EQPT_ID == eq_id &&
                                  a.ALAM_STAT == ProtocolFormat.OHTMessage.ErrorStatus.ErrSet
                            select a;
                return alarm.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        public List<ALARM> loadSetAlarm(DBConnection_EF conn)
        {
            try
            {
                var alarm = from a in conn.ALARM
                            where a.ALAM_STAT == ProtocolFormat.OHTMessage.ErrorStatus.ErrSet
                            select a;
                return alarm.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        public List<ALARM> loadSetAlarmByError(DBConnection_EF conn)
        {
            try
            {
                var alarm = from a in conn.ALARM
                            where a.ALAM_STAT == ProtocolFormat.OHTMessage.ErrorStatus.ErrSet
                            && a.ALAM_LVL == E_ALARM_LVL.Error
                            select a;
                return alarm.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        public List<ALARM> loadSetAlarmByWarn(DBConnection_EF conn)
        {
            try
            {
                var alarm = from a in conn.ALARM
                            where a.ALAM_STAT == ProtocolFormat.OHTMessage.ErrorStatus.ErrSet
                            && a.ALAM_LVL == E_ALARM_LVL.Warn
                            select a;
                return alarm.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        public List<ALARM> loadSetAlarmByEqName(DBConnection_EF conn, string eqName)
        {
            try
            {
                var alarm = from a in conn.ALARM
                            where a.ALAM_STAT == ProtocolFormat.OHTMessage.ErrorStatus.ErrSet
                                && a.EQPT_ID == eqName
                            select a;
                return alarm.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        public int GetSetAlarmErrorCount(DBConnection_EF conn)
        {
            try
            {
                var alarm = from a in conn.ALARM
                            where a.ALAM_STAT == ProtocolFormat.OHTMessage.ErrorStatus.ErrSet &&
                                  a.ALAM_LVL == E_ALARM_LVL.Error
                            select a;
                return alarm.Count();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        public void DeleteAlarmByAlarmID(DBConnection_EF conn, ALARM alarm)
        {
            try
            {
                conn.ALARM.Remove(alarm);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        public IQueryable getQueryAllSQL(DBConnection_EF conn)
        {
            try
            {
                var alarm = from a in conn.ALARM
                            select a;
                return alarm;
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        //public List<ALARM> loadAlarmByConditions(DBConnection_EF conn, DateTime startDatetime, DateTime endDatetime,
        //        bool includeClear = false, string eqptID = null, string alarmCode = null)
        //{
        //    try
        //    {
        //        var query = conn.ALARM.Where(x => x.RPT_DATE_TIME > startDatetime
        //                            && x.RPT_DATE_TIME < endDatetime
        //                            && x.ALAM_STAT == (includeClear == true ? ErrorStatus.ErrReset : ErrorStatus.ErrSet));
        //        if (!string.IsNullOrEmpty(eqptID))
        //        {
        //            query = query.Where(x => x.EQPT_ID == eqptID);
        //        }
        //        //if (!string.IsNullOrEmpty(alarmCode))
        //        //{
        //        //    query = query.Where(x => x.ALAM_CODE == alarmCode);
        //        //}
        //        return query.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Warn(ex);
        //        throw;
        //    }
        //}

        public bool comparetStartTime(string str, DateTime startTime)
        {
            var compareTime = DateTime.ParseExact(str, "yyyyMMddHHmmss", null, DateTimeStyles.AllowWhiteSpaces);

            return compareTime > startTime;
        }

        public bool comparetEndTime(string str, DateTime endTime)
        {
            var compareTime = DateTime.ParseExact(str, "yyyyMMddHHmmss", null, DateTimeStyles.AllowWhiteSpaces);

            return compareTime > endTime;
        }




    }
}
