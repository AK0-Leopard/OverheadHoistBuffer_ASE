//*********************************************************************************
//      BaseUnitObject.cs
//*********************************************************************************
// File Name: BaseUnitObject.cs
// Description: BaseUnitObject類別
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
using com.mirle.ibg3k0.bcf.Data.VO;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    /// <summary>
    /// Class BaseUnitObject.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.VO.BaseEQObject" />
    /// <seealso cref="com.mirle.ibg3k0.sc.Data.VO.IAlarmHisList" />
    public abstract class BaseUnitObject : BaseEQObject, IAlarmHisList
    {
        /// <summary>
        /// The alarm his list
        /// </summary>
        private AlarmHisList alarmHisList = new AlarmHisList();
        /// <summary>
        /// Gets the alarm his list.
        /// </summary>
        /// <value>The alarm his list.</value>
        public virtual AlarmHisList AlarmHisList { get { return alarmHisList; } }

        /// <summary>
        /// Resets the alarm his.
        /// </summary>
        /// <param name="AlarmHisList">The alarm his list.</param>
        public virtual void resetAlarmHis(List<ALARM> AlarmHisList)
        {
            alarmHisList.resetAlarmHis(AlarmHisList);
        }
    }
}
