//*********************************************************************************
//      AlarmBLL.cs
//*********************************************************************************
// File Name: AlarmBLL.cs
// Description: 業務邏輯：Alarm
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.DAO;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.sc.Data;
using Newtonsoft.Json;
using com.mirle.ibg3k0.sc.Service;
using com.mirle.ibg3k0.sc.Data.DAO.EntityFramework;

namespace com.mirle.ibg3k0.sc.BLL
{
    /// <summary>
    /// Class AlarmBLL.
    /// </summary>
    public class AlarmBLL
    {

        /// <summary>
        /// The sc application
        /// </summary>
        private SCApplication scApp = null;
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The alarm DAO
        /// </summary>
        private AlarmDao alarmDao = null;

        private CMD_MCSDao cmd_mcsDao = null;
        /// <summary>
        /// The line DAO
        /// </summary>
        private LineDao lineDao = null;
        /// <summary>
        /// The alarm RPT cond DAO
        /// </summary>
        private AlarmRptCondDao alarmRptCondDao = null;
        /// <summary>
        /// The alarm map DAO
        /// </summary>
        private AlarmMapDao alarmMapDao = null;
        private MainAlarmDao mainAlarmDao = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmBLL"/> class.
        /// </summary>
        public AlarmBLL()
        {

        }

        /// <summary>
        /// Starts the specified sc application.
        /// </summary>
        /// <param name="scApp">The sc application.</param>
        public void start(SCApplication scApp)
        {
            this.scApp = scApp;
            alarmDao = scApp.AlarmDao;
            lineDao = scApp.LineDao;
            alarmRptCondDao = scApp.AlarmRptCondDao;
            alarmMapDao = scApp.AlarmMapDao;
            mainAlarmDao = scApp.MainAlarmDao;
            cmd_mcsDao = scApp.CMD_MCSDao;
        }

        #region Alarm Map
        //public AlarmMap getAlarmMap(string eqpt_real_id, string alarm_id)
        //{
        //    DBConnection conn = null;
        //    AlarmMap alarmMap = null;
        //    try
        //    {
        //        conn = scApp.getDBConnection();
        //        conn.BeginTransaction();

        //        alarmMap = alarmMapDao.getAlarmMap(conn, eqpt_real_id, alarm_id);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Warn("getAlarmMap Exception!", ex);
        //    }
        //    return alarmMap;
        //}
        public AlarmMap GetAlarmMap(string eq_id, string error_code)
        {
            AlarmMap alarmMap = alarmMapDao.getAlarmMap(eq_id, error_code);
            return alarmMap;
        }
        #endregion Alarm Map



        object lock_obj_alarm = new object();
        public ALARM setAlarmReport(string node_id, string eq_id, string error_code, ACMD_MCS mcsCmdData)
        {
            lock (lock_obj_alarm)
            {
                string alarmEq = eq_id;
                //if (scApp.TransferService.isUnitType(eq_id, Service.UnitType.AGVZONE))
                //{
                //    alarmEq = eq_id.Remove(0, 12);
                //}

                if (IsAlarmExist(alarmEq, error_code)) return null;
                string alarmUnitType = "LINE";

                if (scApp.TransferService.isUnitType(eq_id, Service.UnitType.AGV))
                {
                    alarmUnitType = "AGV";
                }
                else if (scApp.TransferService.isUnitType(eq_id, Service.UnitType.CRANE))
                {
                    alarmUnitType = "CRANE";
                }
                else if (scApp.TransferService.isUnitType(eq_id, Service.UnitType.NTB))
                {
                    alarmUnitType = "NTB";
                }
                else if (scApp.TransferService.isUnitType(eq_id, Service.UnitType.OHCV)
                      || scApp.TransferService.isUnitType(eq_id, Service.UnitType.STK)
                   )
                {
                    int stage = scApp.TransferService.portINIData[eq_id].Stage;

                    if(stage == 7)
                    {
                        alarmUnitType = "OHCV_7";
                    }
                    else
                    {
                        alarmUnitType = "OHCV_5";
                    }
                }
                else if (scApp.TransferService.isUnitType(eq_id, Service.UnitType.AGVZONE))
                {
                    //B7_OHBLINE1_ST01
                    alarmUnitType = "LINE";
                    //eq_id = eq_id.Remove(0, 12);
                }

                AlarmMap alarmMap = alarmMapDao.getAlarmMap(alarmUnitType, error_code);

                if(alarmMap == null)
                {
                    scApp.TransferService.TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "OHT >> OHB|AlarmMap 不存在:"
                        + "    EQ_Name:" + eq_id
                        + "    Error_code:" + error_code
                    );
                }

                string strNow = BCFUtility.formatDateTime(DateTime.Now, SCAppConstants.TimestampFormat_19);
                
                ALARM alarm = new ALARM()
                {
                    EQPT_ID = eq_id,
                    RPT_DATE_TIME = strNow,
                    ALAM_CODE = error_code,
                    ALAM_LVL = alarmMap == null ? E_ALARM_LVL.Warn : alarmMap.ALARM_LVL,
                    ALAM_STAT = ProtocolFormat.OHTMessage.ErrorStatus.ErrSet,
                    ALAM_DESC = alarmMap == null ? $"unknow alarm code:{error_code}" : $"{eq_id} {alarmMap.ALARM_DESC}(error code:{error_code})",
                    ERROR_ID = error_code,  //alarmMap?.ALARM_ID ?? "0",
                    UnitID = eq_id,
                    UnitState = "3",
                    RecoveryOption = "",
                    CMD_ID = "",
                };

                if (mcsCmdData != null)
                {
                    alarm.CMD_ID = mcsCmdData.CMD_ID.Trim();
                }

                if (scApp.TransferService.isUnitType(eq_id, UnitType.CRANE))
                {
                    if (error_code == SCAppConstants.SystemAlarmCode.OHT_Issue.DoubleStorage)
                    {
                        alarm.UnitState = "1";
                        alarm.RecoveryOption = "ABORT";
                    }

                    if (error_code == SCAppConstants.SystemAlarmCode.OHT_Issue.EmptyRetrieval)
                    {
                        alarm.UnitState = "2";
                        alarm.RecoveryOption = "ABORT";
                    }
                }

                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    if (alarmDao.insertAlarm(con, alarm) == false)
                    {
                        alarm = null;
                    }

                    CheckSetAlarm();
                }

                //if (scApp.TransferService.isUnitType(eq_id, UnitType.CRANE) == false)
                //{
                //    alarm.EQPT_ID = "";
                //    alarm.UnitID = "";
                //}

                return alarm;
            }
        }

        public void setAlarmReport2Redis(ALARM alarm)
        {
            if (alarm == null) return;
            string hash_field = $"{alarm.EQPT_ID}_{alarm.ALAM_CODE}";
            scApp.getRedisCacheManager().AddTransactionCondition(StackExchange.Redis.Condition.HashNotExists(SCAppConstants.REDIS_KEY_CURRENT_ALARM, hash_field));
            scApp.getRedisCacheManager().HashSetProductOnlyAsync(SCAppConstants.REDIS_KEY_CURRENT_ALARM, hash_field, JsonConvert.SerializeObject(alarm));
        }

        public List<ALARM> getCurrentAlarmsFromRedis()
        {
            List<ALARM> alarms = new List<ALARM>();
            var redis_values_alarms = scApp.getRedisCacheManager().HashValuesAsync(SCAppConstants.REDIS_KEY_CURRENT_ALARM).Result;
            foreach (string redis_value_alarm in redis_values_alarms)
            {
                ALARM alarm_obj = (ALARM)JsonConvert.DeserializeObject(redis_value_alarm, typeof(ALARM));
                alarms.Add(alarm_obj);
            }
            return alarms;
        }

        public bool hasAlarmErrorExist()
        {

            //var redis_values_alarms = scApp.getRedisCacheManager().HashValuesAsync(SCAppConstants.REDIS_KEY_CURRENT_ALARM).Result;
            //if (redis_values_alarms.Count() > 0)
            //{
            //    return true;
            //}
            int count = 0;

            lock (lock_obj_alarm)
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    count = alarmDao.GetSetAlarmErrorCount(con);
                }
            }
            return count != 0;
        }

        public ALARM resetAlarmReport(string eq_id, string error_code)
        {
            lock (lock_obj_alarm)
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ALARM alarm = alarmDao.getSetAlarm(con, eq_id, error_code);
                    if (alarm != null)
                    {
                        string strNow = BCFUtility.formatDateTime(DateTime.Now, SCAppConstants.TimestampFormat_19);
                        alarm.ALAM_STAT = ProtocolFormat.OHTMessage.ErrorStatus.ErrReset;
                        alarm.END_TIME = strNow;
                        alarmDao.updateAlarm(con, alarm);

                        CheckSetAlarm();
                    }
                    return alarm;
                }
            }
        }

        public void resetAlarmReport2Redis(ALARM alarm)
        {
            if (alarm == null) return;
            string hash_field = $"{alarm.EQPT_ID.Trim()}_{alarm.ALAM_CODE.Trim()}";
            //scApp.getRedisCacheManager().AddTransactionCondition(StackExchange.Redis.Condition.HashExists(SCAppConstants.REDIS_KEY_CURRENT_ALARM, hash_field));
            scApp.getRedisCacheManager().HashDeleteAsync(SCAppConstants.REDIS_KEY_CURRENT_ALARM, hash_field);
        }

        public List<ALARM> resetAllAlarmReport(string eq_id)
        {
            List<ALARM> alarms = null;
            lock (lock_obj_alarm)
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    alarms = alarmDao.loadSetAlarm(con, eq_id);

                    if (alarms != null)
                    {
                        foreach (ALARM alarm in alarms)
                        {
                            alarm.ALAM_STAT = ProtocolFormat.OHTMessage.ErrorStatus.ErrReset;

                            alarmDao.updateAlarm(con, alarm);
                        }

                        CheckSetAlarm();
                    }
                }
            }
            return alarms;
        }

        public void resetAllAlarmReport2Redis(string vh_id)
        {
            var current_all_alarm = scApp.getRedisCacheManager().HashKeys(SCAppConstants.REDIS_KEY_CURRENT_ALARM);
            var vh_all_alarm = current_all_alarm.Where(redisKey => ((string)redisKey).Contains(vh_id)).ToArray();
            scApp.getRedisCacheManager().HashDeleteAsync(SCAppConstants.REDIS_KEY_CURRENT_ALARM, vh_all_alarm);
        }
        private bool IsAlarmExist(string eq_id, string code)
        {
            bool isExist = false;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                isExist = alarmDao.getSetAlarmCountByEQAndCode(con, eq_id, code) > 0;
            }
            return isExist;
        }
        public bool IsReportToHost(string code)
        {
            return true;
        }

        public bool enableAlarmReport(string alarm_id, Boolean isEnable)
        {
            bool isSuccess = true;
            try
            {
                string enable_flag = (isEnable ? SCAppConstants.YES_FLAG : SCAppConstants.NO_FLAG);

                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ALARMRPTCOND cond = null;
                    cond = alarmRptCondDao.getRptCond(con, alarm_id);
                    if (cond != null)
                    {
                        cond.ENABLE_FLG = enable_flag;
                        alarmRptCondDao.insertRptCond(con, cond);
                    }
                    else
                    {
                        cond = new ALARMRPTCOND()
                        {
                            ALAM_CODE = alarm_id,
                            ENABLE_FLG = enable_flag
                        };
                        alarmRptCondDao.insertRptCond(con, cond);
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = true;
                logger.Error(ex, "Exception");
            }
            return isSuccess;
        }

        public List<AlarmMap> loadAlarmMaps()
        {
            List<AlarmMap> alarmMaps = alarmMapDao.loadAlarmMaps();
            return alarmMaps;
        }

        public string onMainAlarm(string mAlarmCode, params object[] args)
        {
            MainAlarm mainAlarm = mainAlarmDao.getMainAlarmByCode(mAlarmCode);
            bool isAlarm = false;
            string msg = string.Empty;
            try
            {
                if (mainAlarm != null)
                {
                    isAlarm = mainAlarm.CODE.StartsWith("A");
                    msg = string.Format(mainAlarm.DESCRIPTION, args);
                    if (isAlarm)
                    {
                        msg = string.Format("[{0}]{2}", mainAlarm.CODE, Environment.NewLine, msg);
                        BCFApplication.onErrorMsg(msg);
                    }
                    else
                    {
                        msg = string.Format("[{0}]{2}", mainAlarm.CODE, Environment.NewLine, msg);
                        BCFApplication.onWarningMsg(msg);
                    }
                }
                else
                {
                    logger.Warn(string.Format("LFC alarm/warm happen, but no defin remark code:[{0}] !!!", mAlarmCode));
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
            return msg;
        }

        object lock_obj_alarm_happen = new object();
        public void CheckSetAlarm()
        {
            lock (lock_obj_alarm_happen)
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ALINE line = scApp.getEQObjCacheManager().getLine();
                    List<ALARM> alarmLst = alarmDao.loadSetAlarm(con);

                    if(alarmLst != null && alarmLst.Count > 0)
                    {
                        line.IsAlarmHappened = true;
                    }else
                    {
                        line.IsAlarmHappened = false;
                    }
                }
            }
        }
        public List<ALARM> loadAllAlarmList()
        {
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                return alarmDao.loadAllAlarm(con);
            }
        }

        public List<ALARM> loadSetAlarmList()
        {
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                return alarmDao.loadSetAlarm(con);
            }
        }
        public List<ALARM> loadSetAlarmListByError()
        {
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                return alarmDao.loadSetAlarmByError(con);
            }
        }
        public List<ALARM> loadSetAlarmListByWarn()
        {
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                return alarmDao.loadSetAlarmByWarn(con);
            }
        }
        public List<ALARM> loadSetAlarmListByEqName(string eqName)
        {
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                return alarmDao.loadSetAlarmByEqName(con, eqName);
            }
        }
        public ALARM loadAlarmByAlarmID(string eqid, string alarmId)
        {
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                return alarmDao.loadSetAlarm(con).Where(data => data.EQPT_ID.Trim() == eqid.Trim() && data.ALAM_CODE.Trim() == alarmId.Trim()).FirstOrDefault();
            }
        }

        public bool DeleteAlarmByAlarmID(string alarmId)
        {
            bool isSuccess = true;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    var quary = con.ALARM
                        .Where(data => data.ALAM_CODE == alarmId)
                        .FirstOrDefault();

                    if(quary != null)
                    {
                        alarmDao.DeleteAlarmByAlarmID(con, quary);
                    }                    
                }
            }
            catch
            {
                isSuccess = false;
            }
            return isSuccess;
        }
    }
}
