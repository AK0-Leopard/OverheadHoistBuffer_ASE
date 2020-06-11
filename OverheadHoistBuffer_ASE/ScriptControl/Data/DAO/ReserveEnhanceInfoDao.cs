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
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    /// <summary>
    /// Class AlarmMapDao.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.DaoBase" />
    public class ReserveEnhanceInfoDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the alarm map.
        /// </summary>
        /// <param name="object_id">The eqpt_real_id.</param>
        /// <param name="vhID">The alarm_id.</param>
        /// <returns>AlarmMap.</returns>
        public ReserveEnhanceInfo getReserveInfo(SCApplication app, string groupID)
        {
            try
            {
                DataTable dt = app.OHxCConfig.Tables["RESERVEENHANCEINFO"];
                var query = from c in dt.AsEnumerable()
                            where c.Field<string>("GROUP_ID").Trim() == groupID.Trim()
                            select new ReserveEnhanceInfo
                            {
                                GroupID = c.Field<string>("GROUP_ID"),
                                EnhanceControlSections = stringToStringArray(c.Field<string>("ENHAN_CONTROL_SECTION"))
                            };
                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        public List<ReserveEnhanceInfo> loadReserveInfos(SCApplication app)
        {
            try
            {
                DataTable dt = app.OHxCConfig.Tables["RESERVEENHANCEINFO"];
                var query = from c in dt.AsEnumerable()
                            select new ReserveEnhanceInfo
                            {
                                GroupID = c.Field<string>("GROUP_ID"),
                                EnhanceControlSections = stringToStringArray(c.Field<string>("ENHAN_CONTROL_SECTION"))
                            };
                return query.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }


        List<string> stringToStringArray(string value)
        {
            if (value.Contains("-"))
            {
                return value.Split('-').ToList();
            }
            else
            {
                return new List<string>() { value };
            }
        }

    }
}
