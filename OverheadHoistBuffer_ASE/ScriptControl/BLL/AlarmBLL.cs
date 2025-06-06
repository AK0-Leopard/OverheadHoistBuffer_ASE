﻿//*********************************************************************************
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

        public string getAlarmDeviceType(string eq_id)
        {
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

                if (stage == 7)
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
            return alarmUnitType;
        }

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
                string adr_id = "";
                string port_id = "";

                if (IsAlarmExist(alarmEq, error_code)) return null;
                string alarmUnitType = "LINE";

                if (scApp.TransferService.isUnitType(eq_id, Service.UnitType.AGV))
                {
                    alarmUnitType = "AGV";
                }
                else if (scApp.TransferService.isUnitType(eq_id, Service.UnitType.CRANE))
                {
                    alarmUnitType = "CRANE";
                    (adr_id, port_id) = trySetVehicleinfoWhenAlarmHappend(eq_id, mcsCmdData);
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

                    if (stage == 7)
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

                if (alarmMap == null)
                {
                    scApp.TransferService.TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "OHT >> OHB|AlarmMap 不存在:"
                        + "    EQ_Name:" + eq_id
                        + "    Error_code:" + error_code
                    );
                }


                //string strNow = BCFUtility.formatDateTime(DateTime.Now, SCAppConstants.TimestampFormat_19);
                DateTime date_time_now = DateTime.Now;

                ALARM alarm = new ALARM()
                {
                    EQPT_ID = eq_id,
                    RPT_DATE_TIME = date_time_now,
                    ALAM_CODE = error_code,
                    ALAM_LVL = alarmMap == null ? E_ALARM_LVL.Warn : alarmMap.ALARM_LVL,
                    ALAM_STAT = ProtocolFormat.OHTMessage.ErrorStatus.ErrSet,
                    ALAM_DESC = alarmMap == null ? $"unknow alarm code:{error_code}" : $"{eq_id} {alarmMap.ALARM_DESC}(error code:{error_code})",
                    //ERROR_ID = error_code,  //alarmMap?.ALARM_ID ?? "0",
                    ERROR_ID = SCAppConstants.NO_FLAG,
                    UnitID = eq_id,
                    UnitState = "3",
                    RecoveryOption = "",
                    CMD_ID = "",
                    ADDRESS_ID = adr_id,
                    PORT_ID = port_id,
                };

                if (mcsCmdData != null)
                {
                    alarm.CMD_ID = mcsCmdData.CMD_ID.Trim();
                    alarm.CARRIER_ID = SCUtility.Trim(mcsCmdData.BOX_ID, true);
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

        private (string adr_id, string port_id) trySetVehicleinfoWhenAlarmHappend(string eq_id, ACMD_MCS cmdMCS)
        {
            try
            {
                var vh = scApp.VehicleBLL.cache.getVhByID(eq_id);
                if (vh == null)
                    return ("", "");
                string current_adr_id = SCUtility.Trim(vh.CUR_ADR_ID);
                string current_port_id = "";
                if (cmdMCS == null)
                {
                    current_port_id = "";
                }
                else
                {
                    //var port_stations = scApp.PortStationBLL.OperateCatch.loadPortStationsByAdrID(current_adr_id);
                    var port_stations = scApp.PortDefBLL.cache.loadPortDefByAdrID(current_adr_id);
                    var port_stations_id = port_stations.Select(p => p.PLCPortID).ToList();
                    if (port_stations_id.Contains(cmdMCS.HOSTSOURCE))
                    {
                        current_port_id = cmdMCS.HOSTSOURCE;
                    }
                    else if (port_stations_id.Contains(cmdMCS.HOSTDESTINATION))
                    {
                        current_port_id = cmdMCS.HOSTDESTINATION;
                    }
                    else if (port_stations_id.Contains(cmdMCS.RelayStation))
                    {
                        current_port_id = cmdMCS.RelayStation;
                    }
                    else
                    {
                        current_port_id = "";
                    }
                }
                return (current_adr_id, current_port_id);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                return ("", "");
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
                        //string strNow = BCFUtility.formatDateTime(DateTime.Now, SCAppConstants.TimestampFormat_19);
                        DateTime date_time_now = DateTime.Now;
                        alarm.ALAM_STAT = ProtocolFormat.OHTMessage.ErrorStatus.ErrReset;
                        alarm.END_TIME = date_time_now;
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

        public bool enableAlarmReport(string eqID, string alarm_id, Boolean isEnable, string userID = "", string reason = "")
        {
            bool isSuccess = true;
            try
            {
                string enable_flag = (isEnable ? SCAppConstants.YES_FLAG : SCAppConstants.NO_FLAG);

                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ALARMRPTCOND cond = null;
                    DateTime? disable_time = null;
                    if (isEnable)
                    {
                        disable_time = null;
                    }
                    else
                    {
                        disable_time = DateTime.Now;
                    }
                    cond = alarmRptCondDao.getRptCond(con, eqID, alarm_id);
                    if (cond != null)
                    {
                        cond.ENABLE_FLG = enable_flag;
                        cond.USER_ID = userID;
                        cond.REASON = reason;
                        cond.DISABLE_TIME = disable_time;
                        alarmRptCondDao.updateRptCond(con, cond);
                    }
                    else
                    {
                        cond = new ALARMRPTCOND()
                        {
                            EQPT_ID = eqID,
                            ALAM_CODE = alarm_id,
                            ENABLE_FLG = enable_flag,
                            USER_ID = userID,
                            REASON = reason,
                            DISABLE_TIME = disable_time
                        };
                        alarmRptCondDao.insertRptCond(con, cond);
                    }
                }
                scApp.getCommObjCacheManager().RefreshAlarmReportCond();
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error(ex, "Exception");
            }
            return isSuccess;
        }

        public List<ALARMRPTCOND> loadAllAlarmRptCond()
        {
            List<ALARMRPTCOND> alarm_rpt_conds = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                alarm_rpt_conds = alarmRptCondDao.loadAllRptCond(con);
            }
            return alarm_rpt_conds;
        }

        public bool isReportAlarmReport2MCS(string eqID, string alarmID, bool isDeviceTpye = false)
        {
            try
            {
                string device_type = eqID;
                if (!isDeviceTpye)
                    device_type = getAlarmDeviceType(eqID);
                var alarm_report_conds = scApp.getCommObjCacheManager().getAlarmReportConds();
                if (alarm_report_conds == null) return true;
                var alarm_report_cond = alarm_report_conds.Where(cond => SCUtility.isMatche(cond.EQPT_ID, device_type) &&
                                                                        SCUtility.isMatche(cond.ALAM_CODE, alarmID))
                                                          .FirstOrDefault();
                if (alarm_report_cond == null) return true;
                return SCUtility.isMatche(alarm_report_cond.ENABLE_FLG, SCAppConstants.YES_FLAG);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                return false;
            }
        }

        public string getAlarmReportCondUserID(string deviceID, string alarmID)
        {
            try
            {
                var alarm_report_conds = scApp.getCommObjCacheManager().getAlarmReportConds();
                if (alarm_report_conds == null) return "";
                var alarm_report_cond = alarm_report_conds.Where(cond => SCUtility.isMatche(cond.EQPT_ID, deviceID) &&
                                                                        SCUtility.isMatche(cond.ALAM_CODE, alarmID))
                                                          .FirstOrDefault();
                if (alarm_report_cond == null) return "";
                return alarm_report_cond.USER_ID;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                return "";
            }
        }
        public string getAlarmReportCondReason(string deviceID, string alarmID)
        {
            try
            {
                var alarm_report_conds = scApp.getCommObjCacheManager().getAlarmReportConds();
                if (alarm_report_conds == null) return "";
                var alarm_report_cond = alarm_report_conds.Where(cond => SCUtility.isMatche(cond.EQPT_ID, deviceID) &&
                                                                        SCUtility.isMatche(cond.ALAM_CODE, alarmID))
                                                          .FirstOrDefault();
                if (alarm_report_cond == null) return "";
                return alarm_report_cond.REASON;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                return "";
            }
        }
        public string getAlarmReportCondDisableTime(string deviceID, string alarmID)
        {
            try
            {
                var alarm_report_conds = scApp.getCommObjCacheManager().getAlarmReportConds();
                if (alarm_report_conds == null) return "";
                var alarm_report_cond = alarm_report_conds.Where(cond => SCUtility.isMatche(cond.EQPT_ID, deviceID) &&
                                                                        SCUtility.isMatche(cond.ALAM_CODE, alarmID))
                                                          .FirstOrDefault();
                if (alarm_report_cond == null) return "";
                var disable_dateTime = alarm_report_cond.DISABLE_TIME.HasValue ?
                                       alarm_report_cond.DISABLE_TIME.Value.ToString(SCAppConstants.DateTimeFormat_22) :
                                       "";
                return disable_dateTime;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                return "";
            }
        }


        public List<AlarmMap> loadAlarmMaps()
        {
            List<AlarmMap> alarmMaps = alarmMapDao.loadAlarmMaps();
            return alarmMaps;
        }
        public List<AlarmMap> loadAlarmMaps(string eqObject)
        {
            List<AlarmMap> alarmMaps = alarmMapDao.loadAlarmMapsByEQRealID(eqObject);
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

                    if (alarmLst != null && alarmLst.Count > 0)
                    {
                        line.IsAlarmHappened = true;
                    }
                    else
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

        public List<ALARM> loadSetAlarmAndNonReportAndTenAgo()
        {
            List<ALARM> alarms = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                alarms = alarmDao.loadSetAlarmAndNoReportAndTenAgo(con);
            }
            return alarms;
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

                    if (quary != null)
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
        public void DeleteBefer6MonthAlarmByAlarmIDBatch()
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    alarmDao.DeleteBefer6MonthAlarmByAlarmIDBatch(con);
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex, "Exception:");
            }
        }


        public List<ALARM> loadAlarms(DateTime startTime, DateTime endTime)
        {
            string start_time = BCFUtility.formatDateTime(startTime, SCAppConstants.TimestampFormat_19);
            string end_time = BCFUtility.formatDateTime(endTime, SCAppConstants.TimestampFormat_19);
            List<ALARM> alarm = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                alarm = alarmDao.getAlarms(con, start_time, end_time);
            }
            return alarm;
        }

        public bool SetAlarmAlreadyReport(ALARM sourceAlarm)
        {
            bool isSuccess = false;
            ALARM alarm = new ALARM();
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    alarm.EQPT_ID = sourceAlarm.EQPT_ID;
                    alarm.ALAM_CODE = sourceAlarm.ALAM_CODE;
                    alarm.RPT_DATE_TIME = sourceAlarm.RPT_DATE_TIME;

                    con.ALARM.Attach(alarm);
                    alarm.ERROR_ID = SCAppConstants.YES_FLAG;
                    con.Entry(alarm).Property(p => p.ERROR_ID).IsModified = true;
                    alarmDao.updateAlarm(con, alarm);
                    con.Entry(alarm).State = System.Data.Entity.EntityState.Detached;
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, "Exception:");
            }
            return isSuccess;
        }


    }
}
