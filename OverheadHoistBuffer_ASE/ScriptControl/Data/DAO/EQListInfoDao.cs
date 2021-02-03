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
    public class EQListInfoDao : DaoBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public EQListInfo getEQListInfo(SCApplication app, string eqpt_id)
        {
            try
            {
                DataTable dt = app.OHxCConfig.Tables["EQLIST"];
                var query = from c in dt.AsEnumerable()
                            where c.Field<string>("EQPT_ID").Trim() == eqpt_id.Trim()
                            select new EQListInfo
                            {
                                EQPT_ID = c.Field<string>("EQPT_ID"),
                            };
                return query.SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
    }
}
