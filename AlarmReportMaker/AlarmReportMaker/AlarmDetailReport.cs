using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace AlarmReportMaker
{
    public class AlarmDetailReport
    {
        public string title { get; private set; }
        #region DB Connection
        //預設用本機ASE K24來測試
        public string IP { get; private set; } = "127.0.0.1";
        private string userID = "sa";
        private string password = "p@ssw0rd";
        public string schema { get; private set; } = "OHBC_ASE_K24_v1";
        private string DBScript = @"select top(10)
	                                    'U332MTB0' as 'workingNumber',
	                                    '日月光 K24 BE2 OHT' as 'workingName',
	                                    RPT_DATE_TIME,
	                                    END_TIME,
	                                    EQPT_ID,
	                                    isnull(ALARM_MODULE, '') as 'ALARM_MODULE',
	                                    isnull(CLASS, '') as 'CLASS',
	                                    isnull(IMPORTANCE_LVL, '') as 'IMPORTANCE_LVL',
	                                    ALAM_CODE,
	                                    ALAM_DESC,
	                                    isnull(ADDRESS_ID, '') as 'ADDRESS_ID',
	                                    isnull(PORT_ID, '') as 'PORT_ID',
	                                    isnull(CARRIER_ID, '') as 'CARRIER_ID',
	                                    isnull(REMARK, '') as 'REMARK'
                                    from
	                                    ALARM
                                    where END_TIME is not null
                                    order by RPT_DATE_TIME desc";
        private string selectRangeDeclare
        {
            get
            {
                DateTime dt = DateTime.Now;
                return "declare @start as varchar(25); " +
                        "declare @end as varchar(25); " +
                        "set @start='" + dt.AddDays(-reportDataLength).ToString("yyyy-MM-dd HH:mm:ss:fff") + "'; " +
                        "set @end='" + dt.ToString("yyyy-MM-dd HH:mm:ss:fff") + "';";
            }
        }

        public string ConnectionString
        {
            get
            {
                return "Server=" + IP + ";" +
                        "Database=" + schema + ";" +
                        "User Id=" + userID + ";" +
                        "Password=" + password;
            }
        }
        #endregion
        #region 二次加工
        private bool alarmMapMapping = false; //多國與係轉換
        private bool EQ_Name_Mapping = false; //EQ資訊轉換
        private bool alarmModuleMapping = false; //異常模組轉換
        public List<alarmMap> alarmMapList = null;
        public List<EQMap> EQMapList = null;
        public Dictionary<string, string> alarmModuleList = null;
        #endregion

        #region 共用物件
        public class alarmMap
        {
            public string object_ID { get; private set; }
            public string alarm_ID { get; private set; }
            string alarm_lvl;
            string alarm_desc;
            public string alarm_desc_tw { get; private set; }
            public alarmMap(string _object_ID, string _alarm_ID, string _alarm_lvl, string _alarm_desc, string _alarm_desc_tw)
            {
                object_ID = _object_ID;
                alarm_ID = _alarm_ID;
                alarm_lvl = _alarm_lvl;
                alarm_desc = _alarm_desc;
                alarm_desc_tw = _alarm_desc_tw;
            }
        }
        public class EQMap
        {
            public string EQ_ID { get; private set; }
            public string EQ_Name { get; private set; }
            public string EQ_Number { get; private set; }
            public EQMap(string _EQ_ID, string _EQ_Name, string _EQ_Number)
            {
                EQ_ID = _EQ_ID;
                EQ_Name = _EQ_Name;
                EQ_Number = _EQ_Number;
            }
        }


        #endregion

        public int reportDataLength { get; set; }

        public AlarmDetailReport(string _title, string _IP, string _userID, string _password, string _script, string _schema, int _reportDataLength,
            DataTable _dt_alarmMap = null, DataTable _dt_EQMap = null, DataTable _alarmModuleMap = null)
        {
            title = _title;
            IP = _IP;
            userID = _userID;
            password = _password;
            DBScript = _script;
            schema = _schema;
            reportDataLength = _reportDataLength;
            if (_dt_alarmMap == null)
                alarmMapMapping = false;
            else
            {
                alarmMapMapping = true;
                alarmMapList = new List<alarmMap>();
                foreach (DataRow row in _dt_alarmMap.Rows)
                {
                    alarmMap temp = new alarmMap(
                        row.Field<string>("OBJECT_ID"),
                        row.Field<string>("ALARM_ID"),
                        row.Field<string>("ALARM_LVL"),
                        row.Field<string>("ALARM_DESC"),
                        row.Field<string>("ALARM_DESC_TW"));
                    alarmMapList.Add(temp);
                }

            }
            if (_dt_EQMap == null)
                EQ_Name_Mapping = false;
            else
            {
                EQ_Name_Mapping = true;
                EQMapList = new List<EQMap>();
                foreach (DataRow row in _dt_EQMap.Rows)
                {
                    EQMap temp = new EQMap(
                        row.Field<string>("ID"),
                        row.Field<string>("EQ_NAME"),
                        row.Field<string>("EQ_NUMBER"));
                    EQMapList.Add(temp);
                }
            }
            if (_alarmModuleMap == null)
                alarmModuleMapping = false;
            else
            {
                alarmModuleMapping = true;
                alarmModuleList = new Dictionary<string, string>();
                foreach (DataRow row in _alarmModuleMap.Rows)
                {
                    alarmModuleList.Add(row.Field<string>("NUMBER"), row.Field<string>("MODULE_TW"));
                }
            }
        }
        public AlarmDetailReport()
        {

        }
        public List<MaintenanceAlarm> getReport()
        {
            List<MaintenanceAlarm> result = new List<MaintenanceAlarm>();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    conn.ChangeDatabase(schema);
                    SqlCommand cmd = new SqlCommand(DBScript, conn);
                    var DBData = new SqlCommand(selectRangeDeclare + DBScript, conn).ExecuteReader();
                    if (DBData.HasRows)
                    {
                        int i = 1;
                        while (DBData.Read())
                        {
                            MaintenanceAlarm alarm = new MaintenanceAlarm(
                                _workingNumber: DBData.GetString(0),
                                _workingName: DBData.GetString(1),
                                _colNumber: i,
                                _RPT_DATE_TIME: Convert.ToDateTime(DBData[2]),
                                _END_TIME: Convert.ToDateTime(DBData[3]),
                                _EQPT_Name: DBData.GetString(4),
                                _EQ_Number: DBData.GetString(4),
                                _alarmModule: DBData[5].ToString(),
                                _classification: DBData[6].ToString(),
                                _importance: DBData[7].ToString(),
                                _alarmCode: DBData.GetString(8),
                                _alarmDesc: DBData.GetString(9),
                                _adrID: DBData.GetString(10),
                                _portID: DBData.GetString(11),
                                _boxID: DBData.GetString(12),
                                _remark: DBData.GetString(13)
                                );
                            if (alarmMapMapping)
                            {
                                //string eq = (alarm.EQ_Name=="OHT") ? "CRANE" : alarm.EQ_Name;
                                string eq = getObjectID(alarm.EQ_Name);
                                alarmMap map = alarmMapList.FirstOrDefault(x => x.object_ID == eq && x.alarm_ID == alarm.alarmCode);
                                if (map != null)
                                    alarm.setAlarmDesc(map.alarm_desc_tw);
                            }
                            if (EQ_Name_Mapping)
                            {
                                EQMap eq = EQMapList.FirstOrDefault(x => x.EQ_ID == alarm.EQ_Name);
                                if (eq != null) { alarm.setEQNameAndNumber(eq.EQ_Name, eq.EQ_Number); }
                            }

                            if (alarmModuleMapping)
                            {
                                if (alarmModuleList.TryGetValue(alarm.moduleClassification, out string alarmModule))
                                {
                                    alarm.setAlarmModule(alarmModule);
                                }
                            }
                            i++;
                            result.Add(alarm);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private string getObjectID(string eqID)
        {

            if (eqID.Contains("_CR"))
                return "CRANE";
            else if (isOHCV_7(eqID))
                return "OHCV_7";
            else if (isOHCV_5(eqID))
                return "OHCV_5";
            else if (isNTB(eqID))
                return "NTB";
            else if (eqID.Contains("_A"))
                return "AGV";
            else if (eqID.Contains("AGV")) //AGVC使用的轉型
                return "AGV_NODE";
            else if (eqID.Contains("Charge")) //AGVC使用的轉型
                return eqID;
            else
                return eqID;
        }

        private bool isNTB(string eqID)
        {
            switch (eqID)
            {
                case "B7_OHBLINE1_T01":
                case "B7_OHBLINE2_T01":
                case "B7_OHBLINE3_T01":
                case "B7_OHBLINE3_T02":
                    return true;
                default:
                    return false;
            }
        }
        private bool isOHCV_7(string eqID)
        {
            switch (eqID)
            {
                case "B7_OHBLOOP_T0A":
                    return true;
                default:
                    return false;
            }
        }
        private bool isOHCV_5(string eqID)
        {
            switch (eqID)
            {
                case "B7_OHBLINE2_T02":
                case "B7_OHBLINE2_T03":
                case "B7_OHBLOOP_T01":
                case "B7_OHBLOOP_T02":
                case "B7_OHBLOOP_T03":
                case "B7_OHBLOOP_T04":
                case "B7_OHBLOOP_T05":
                case "B7_OHBLOOP_T06":
                case "B7_OHBLOOP_T07":
                case "B7_OHBLOOP_T08":
                case "B7_OHBLOOP_T09":
                case "B7_OHBLOOP_T0B":
                case "B7_OHBLOOP_T0C":
                    return true;
                default:
                    return false;
            }
        }
    }
}
