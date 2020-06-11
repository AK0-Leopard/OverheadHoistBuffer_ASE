// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="EventBLL.cs" company="">
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
using com.mirle.ibg3k0.sc.Data.DAO;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Common;

namespace com.mirle.ibg3k0.sc.BLL
{
    /// <summary>
    /// Class EventBLL.
    /// </summary>
    public class EventBLL
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
        /// The event RPT cond DAO
        /// </summary>
        private EventRptCondDao eventRptCondDao = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventBLL"/> class.
        /// </summary>
        public EventBLL()
        {
        }

        /// <summary>
        /// Starts the specified sc application.
        /// </summary>
        /// <param name="scApp">The sc application.</param>
        public void start(SCApplication scApp)
        {
            this.scApp = scApp;
            eventRptCondDao = scApp.EventRptCondDao;
        }

        /// <summary>
        /// Enables all event report.
        /// </summary>
        /// <param name="isEnable">The is enable.</param>
        public bool enableAllEventReport(Boolean isEnable)
        {
            bool isSuccess = true;
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetUContext();
                conn.BeginTransaction();
                eventRptCondDao.deleteAllRptCond(conn);
                if (!isEnable)
                {
                    string[] ceidArray = SECSConst.CEID_ARRAY;
                    for (int i = 0; i < ceidArray.Length; i++)
                    {
                        AEVENTRPTCOND cond = new AEVENTRPTCOND()
                        {
                            CEID = ceidArray[i],
                            ENABLE_FLG = SCAppConstants.NO_FLAG
                        };
                        eventRptCondDao.updateRptCond(conn, cond);
                    }
                }
                conn.Commit();
            }
            catch (Exception ex)
            {
                isSuccess = false;
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Error(ex, "Exception");
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception"); } }
            }
            return isSuccess;
        }

        /// <summary>
        /// Enables the event report.
        /// </summary>
        /// <param name="ceid">The ceid.</param>
        /// <param name="isEnable">The is enable.</param>
        public bool enableEventReport(string ceid, Boolean isEnable)
        {
            bool isSuccess = true;
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                AEVENTRPTCOND cond = eventRptCondDao.getRptCond(conn, ceid);
                if (cond != null)
                {
                    cond.ENABLE_FLG = (isEnable ? SCAppConstants.YES_FLAG : SCAppConstants.NO_FLAG);
                    eventRptCondDao.updateRptCond(conn, cond);
                }
                else
                {
                    cond = new AEVENTRPTCOND()
                    {
                        CEID = ceid.Trim(),
                        ENABLE_FLG = (isEnable ? SCAppConstants.YES_FLAG : SCAppConstants.NO_FLAG)
                    };
                    eventRptCondDao.insertRptCond(conn, cond);
                }

                conn.Commit();
            }
            catch (Exception ex)
            {
                isSuccess = false;
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Error(ex, "Exception");
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception"); } }
            }
            return isSuccess;
        }

        /// <summary>
        /// Determines whether [is enable report] [the specified ceid].
        /// </summary>
        /// <param name="ceid">The ceid.</param>
        /// <returns>Boolean.</returns>
        public Boolean isEnableReport(string ceid)
        {
            Boolean isEnable = false;
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                AEVENTRPTCOND cond = eventRptCondDao.getRptCond(conn, ceid);
                if (cond == null)
                {
                    isEnable = true;
                }
                else
                {
                    isEnable = BCFUtility.isMatche(cond.ENABLE_FLG, SCAppConstants.YES_FLAG);
                }
                conn.Commit();
            }
            catch (Exception ex)
            {
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Error(ex, "Exception");
                return isEnable;
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception"); } }
            }
            return isEnable;
        }

    }
}
