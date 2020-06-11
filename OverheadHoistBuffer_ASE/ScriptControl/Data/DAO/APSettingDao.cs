// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="AlarmMapDao.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Data;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;
using com.mirle.ibg3k0.bcf.Common;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    /// <summary>
    /// Class AlarmMapDao.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.DaoBase" />
    public class APSettingDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Loads the alarm maps by eq real identifier.
        /// </summary>
        /// <param name="eqpt_real_id">The eqpt_real_id.</param>
        /// <returns>List&lt;AlarmMap&gt;.</returns>
        public APSetting getAPSettingByAPName(string name)
        {
            try
            {
                DataTable dt = SCApplication.getInstance().OHxCConfig.Tables["APSETTING"];
                var query = from c in dt.AsEnumerable()
                            where c.Field<string>("AP_NAME").Trim() == name.Trim()
                            select new APSetting
                            {
                                AP_NAME = c.Field<string>("AP_NAME"),
                                GETWAY_IP = c.Field<string>("GATWAY_IP"),
                                REMOTE_IP = c.Field<string>("REMOTE_IP")
                            };
                return query.SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }


        public List<APSetting> loadAPSettingByAPName()
        {
            try
            {
                DataTable dt = SCApplication.getInstance().OHxCConfig.Tables["APSETTING"];
                var query = from c in dt.AsEnumerable()
                            select new APSetting
                            {
                                AP_NAME = c.Field<string>("AP_NAME"),
                                GETWAY_IP = c.Field<string>("GATWAY_IP"),
                                REMOTE_IP = c.Field<string>("REMOTE_IP")
                            };
                return query.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

    }
}
