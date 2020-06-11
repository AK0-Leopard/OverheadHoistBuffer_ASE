//*********************************************************************************
//      AlarmHisList.cs
//*********************************************************************************
// File Name: AlarmHisList.cs
// Description: Alarm History List類別
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

namespace com.mirle.ibg3k0.sc.Data.VO
{
    /// <summary>
    /// Class AlarmHisList.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Common.PropertyChangedVO" />
    public class AlarmHisList : PropertyChangedVO
    {
        /// <summary>
        /// The alarm list
        /// </summary>
        private List<ALARM> alarmList = new List<ALARM>();
        /// <summary>
        /// Gets the alarm list.
        /// </summary>
        /// <value>The alarm list.</value>
        public virtual List<ALARM> AlarmList { get { return alarmList; } }

        /// <summary>
        /// The alarm lock
        /// </summary>
        private Object alarmLock = new Object();
        /// <summary>
        /// Resets the alarm his.
        /// </summary>
        /// <param name="dbAlarmList">The database alarm list.</param>
        public virtual void resetAlarmHis(List<ALARM> dbAlarmList)
        {
            lock (alarmLock)
            {
                alarmList.Clear();
                alarmList.AddRange(dbAlarmList);
                OnPropertyChanged(BCFUtility.getPropertyName(() => this.AlarmList));
            }
        }

        /// <summary>
        /// Adds the alarm his.
        /// </summary>
        /// <param name="alarm">The alarm.</param>
        public virtual void addAlarmHis(ALARM alarm) 
        {
            lock (alarmLock)
            {
                alarmList.Add(alarm);
                OnPropertyChanged(BCFUtility.getPropertyName(() => this.AlarmList));
            }
        }

    }

    /// <summary>
    /// Interface IAlarmHisList
    /// </summary>
    public interface IAlarmHisList
    {
        /// <summary>
        /// Resets the alarm his.
        /// </summary>
        /// <param name="dbAlarmList">The database alarm list.</param>
        void resetAlarmHis(List<ALARM> dbAlarmList);
    }
}
