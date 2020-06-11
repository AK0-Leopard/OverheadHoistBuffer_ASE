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
    public class DataCollectionDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public DataCollectionSetting getDataCollectionItemByMethodAndItemName(SCApplication app, string item_name)
        {
            try
            {
                var query = from c in app.DataCollectionList
                            where c.ItemName == item_name
                            select c;
                return query.SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        public DataCollectionSetting getDataCollectionItemByMethod(SCApplication app, string method_name)
        {
            try
            {
                var query = from c in app.DataCollectionList
                            where c.Method == method_name
                            select c;
                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        public DataCollectionSetting getDataCollectionFirstItem(SCApplication app)
        {
            try
            {
                var query = from c in app.DataCollectionList
                            select c;
                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

    }
}
