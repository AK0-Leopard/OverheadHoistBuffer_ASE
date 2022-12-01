using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmReportMaker
{
    public class MaintenanceAlarm
    {
        public string workingNumber { get; private set; } //工令號
        public string workingName { get; private set; }//工令名稱
        public string abnormalOrResidualStirng { get; private set; } //異常 or 殘項

        public int colNumber { get; set; }
        public string OneDayCount { get; set; }
        public string Date { get; private set; }
        public string alarmHappend { get; private set; }
        public string alarmClear { get; private set; }
        public string workTime { get; private set; } //排除時間
        public string schedule { get; private set; } //排班，班別
        public string EQ_Name { get; private set; } //設備名稱
        public string EQ_Number { get; private set; } //設備編號
        public string moduleClassification { get; private set; } //模組分類
        public string alarmClassification { get; private set; }//異常分類
                                                               //public Importance importance { get; } //重要度
        public string importance { get; private set; } //重要度
        public string alarmCode { get; private set; } //異常碼
        public string alarmDesc { get; private set; }//異常說明

        public string location { get; private set; } //位置
        public string portID { get; private set; }//位置
        public string CurrentHigh { get; set; } //卷陽當前高度
        public string TeachModifyData { get; set; } //TEACH修改數據
        public string boxNumber { get; private set; } //盒號
        public string Version { get; set; } //版本號碼

        public string alarmRemark { get; private set; } //異常碼註解


        public enum AbnormalOrResidual
        {
            isAbnormal = 0,
            isResidual = 1,
        }

        public MaintenanceAlarm(string _workingNumber, string _workingName, int _colNumber, DateTime _RPT_DATE_TIME, DateTime _END_TIME,
            string _EQPT_Name, string _EQ_Number, string _alarmModule, string _classification, string _importance,
            string _alarmCode, string _alarmDesc, string _adrID, string _portID, string _boxID, string _remark)
        {
            workingNumber = _workingNumber;
            workingName = _workingName;
            abnormalOrResidualStirng = "異常";
            colNumber = _colNumber;
            Date = _RPT_DATE_TIME.ToString("yyyy/MM/dd");
            alarmHappend = _RPT_DATE_TIME.ToString("yyyy/MM/dd HH:mm");
            alarmClear = _END_TIME.ToString("yyyy/MM/dd HH:mm");
            workTime = (_END_TIME == null) ? "99999" : (_END_TIME - _RPT_DATE_TIME).TotalMinutes.ToString("0");
            EQ_Name = _EQPT_Name;
            EQ_Number = _EQ_Number;
            moduleClassification = _alarmModule;
            switch (_classification)
            {
                case "1":
                    alarmClassification = "1.維修保養";
                    break;
                case "2":
                    alarmClassification = "2.異常衍生";
                    break;
                case "3":
                    alarmClassification = "3.人員誤觸";
                    break;
                case "4":
                    alarmClassification = "4.EQ產生";
                    break;
                case "5":
                    alarmClassification = "5.其他";
                    break;
                case "6":
                    alarmClassification = "6.硬體";
                    break;
                case "7":
                    alarmClassification = "7.軟體";
                    break;
                case "8":
                    alarmClassification = "8.電氣";
                    break;
                default:
                    alarmClassification = "";
                    break;
            }
            switch (_importance)
            {
                case "0":
                    importance = "低";
                    break;
                case "1":
                    importance = "高";
                    break;
            }
            alarmCode = _alarmCode;
            alarmDesc = _alarmDesc;
            location = _adrID;
            portID = _portID;
            boxNumber = _boxID;
            alarmRemark = _remark;
        }
        public void setEQNameAndNumber(string _EQ_Name, string _EQ_Number)
        {
            EQ_Name = _EQ_Name;
            EQ_Number = _EQ_Number;
        }
        public void setAlarmDesc(string _alarmDesc)
        {
            alarmDesc = _alarmDesc;
        }
        public void setAlarmClassification(string _classification)
        {
            alarmClassification = _classification;
        }
        public void setAlarmModule(string _alarmModule)
        {
            moduleClassification = _alarmModule;
        }
    }
}
