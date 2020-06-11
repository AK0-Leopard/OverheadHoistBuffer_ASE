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
    public class ReturnCodeMapDao : DaoBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public List<ReturnCodeMap> loadReturnCodeMaps(SCApplication app,string eqpt_real_id)
        {
            try
            {
                DataTable dt = app.OHxCConfig.Tables["RETURNCODEMAP"];
                var query = from c in dt.AsEnumerable()
                            where c.Field<string>("OBJECT_ID").Trim() == eqpt_real_id.Trim()
                            select new ReturnCodeMap
                            {
                                EQPT_REAL_ID = c.Field<string>("OBJECT_ID"),
                                RETURN_CODE = c.Field<string>("RETURN_CODE"),
                                DESC = c.Field<string>("DESC")
                            };
                return query.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public ReturnCodeMap getReturnCodeMap(SCApplication app, string eqpt_real_id, string return_code)
        {
            try
            {
                DataTable dt = app.OHxCConfig.Tables["RETURNCODEMAP"];
                var query = from c in dt.AsEnumerable()
                            where c.Field<string>("OBJECT_ID").Trim() == eqpt_real_id.Trim() &&
                            c.Field<string>("RETURN_CODE").Trim() == return_code.Trim()
                            select new ReturnCodeMap
                            {
                                EQPT_REAL_ID = c.Field<string>("OBJECT_ID"),
                                RETURN_CODE = c.Field<string>("RETURN_CODE"),
                                DESC = c.Field<string>("DESC")
                            };
                return query.SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        /// <summary>
        /// 將文字的1(Warn)、2(Error)轉換成enum:E_ALARM_LVL
        /// </summary>
        /// <param name="s_lvl"></param>
        /// <returns></returns>
        private E_ALARM_LVL convertALARM_LVL2Enum(string s_lvl)
        {
            int i_lvl = int.Parse(s_lvl);
            return (E_ALARM_LVL)i_lvl;
        }


    }
}
