// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="BCSystemWIF.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;

namespace com.mirle.ibg3k0.sc.WIF
{
    /// <summary>
    /// Class BCSystemWIF.
    /// </summary>
    public class BCSystemWIF
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
        /// The eq object cache manager
        /// </summary>
        private EQObjCacheManager eqObjCacheManager = null;
        /// <summary>
        /// The bc system BLL
        /// </summary>
        private BCSystemBLL bcSystemBLL = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BCSystemWIF"/> class.
        /// </summary>
        /// <param name="scApp">The sc application.</param>
        public BCSystemWIF(SCApplication scApp) 
        {
            this.scApp = scApp;
            eqObjCacheManager = scApp.getEQObjCacheManager();
            bcSystemBLL = scApp.BCSystemBLL;
        }



        /// <summary>
        /// 更改系統時間
        /// </summary>
        /// <param name="hostTime">The host time.</param>
        public void updateSystemTime(DateTime hostTime) 
        {
            SystemTime st = new SystemTime();
            st.FromDateTime(hostTime);
            SystemTime.SetSystemTime(ref st);
            SystemTime.GetSystemTime(ref st);
            logger.Info("Set System Time:{0}", st.ToDateTime().ToString(SCAppConstants.TimestampFormat_16));
        }

    }
}
