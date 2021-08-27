// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="AlarmMap.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    /// <summary>
    /// Class AlarmMap.
    /// </summary>
    public class AlarmMap
    {
        /// <summary>
        /// The eqp t_ rea l_ identifier
        /// </summary>
        public String EQPT_REAL_ID { get; set; }
        public String ALARM_ID { get; set; }
        public String ALARM_DESC { get; set; }
        public E_ALARM_LVL ALARM_LVL { get; set; }
    }

    public class AlarmMapToShow
    {
        BLL.AlarmBLL alarmBLL = null;
        AlarmMap alarmMap = null;
        public AlarmMapToShow(BLL.AlarmBLL _alarmBLL, AlarmMap _alarmMap)
        {
            alarmBLL = _alarmBLL;
            alarmMap = _alarmMap;
        }
        /// <summary>
        /// The eqp t_ rea l_ identifier
        /// </summary>
        public String EQPT_REAL_ID
        {
            get { return alarmMap == null ? "" : alarmMap.EQPT_REAL_ID; }
        }
        public String ALARM_ID
        {
            get { return alarmMap == null ? "" : alarmMap.ALARM_ID; }
        }

        public String ALARM_DESC
        {
            get { return alarmMap == null ? "" : alarmMap.ALARM_DESC; }
        }

        public E_ALARM_LVL ALARM_LVL
        {
            get { return alarmMap == null ? E_ALARM_LVL.None : alarmMap.ALARM_LVL; }
        }

        public bool IS_REPORT
        {
            get
            {
                return alarmBLL.isReportAlarmReport2MCS(EQPT_REAL_ID, ALARM_ID, true);
            }

        }
    }
}
