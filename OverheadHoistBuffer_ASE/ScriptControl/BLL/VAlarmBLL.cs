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
    public class VAlarmBLL
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
        private VAlarmDao valarmDao = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmBLL"/> class.
        /// </summary>
        public VAlarmBLL()
        {

        }

        /// <summary>
        /// Starts the specified sc application.
        /// </summary>
        /// <param name="scApp">The sc application.</param>
        public void start(SCApplication scApp)
        {
            this.scApp = scApp;
            valarmDao = scApp.VAlarmDao;
        }

        public List<VALARM> loadAlarms(DateTime startTime, DateTime endTime)
        {
            string start_time = BCFUtility.formatDateTime(startTime, SCAppConstants.TimestampFormat_19);
            string end_time = BCFUtility.formatDateTime(endTime, SCAppConstants.TimestampFormat_19);
            List<VALARM> alarm = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                alarm = valarmDao.getAlarms(con, start_time, end_time);
            }
            return alarm;
        }


    }
}
