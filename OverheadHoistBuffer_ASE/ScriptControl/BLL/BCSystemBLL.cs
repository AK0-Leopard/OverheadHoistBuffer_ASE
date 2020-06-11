//*********************************************************************************
//      BCSystemBLL.cs
//*********************************************************************************
// File Name: BCSystemBLL.cs
// Description: BCSystemBLL
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
using com.mirle.ibg3k0.bcf.Data.TimerAction;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.DAO;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.sc.Data;

namespace com.mirle.ibg3k0.sc.BLL
{
    /// <summary>
    /// Class BCSystemBLL.
    /// </summary>
    public class BCSystemBLL
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
        /// The bc status DAO
        /// </summary>
        private BCStatusDao bcStatusDao = null;
        /// <summary>
        /// The operation his DAO
        /// </summary>
        private OperationHisDao operationHisDao = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BCSystemBLL"/> class.
        /// </summary>
        public BCSystemBLL()
        {

        }

        /// <summary>
        /// Starts the specified sc application.
        /// </summary>
        /// <param name="scApp">The sc application.</param>
        public void start(SCApplication scApp)
        {
            this.scApp = scApp;
            this.bcStatusDao = scApp.BCStatusDao;
            this.operationHisDao = scApp.OperationHisDao;
        }

        /// <summary>
        /// Initials the bc system.
        /// </summary>
        /// <returns>SCAppConstants.BCSystemInitialRtnCode.</returns>
        public SCAppConstants.BCSystemInitialRtnCode initialBCSystem()
        {
            string bc_id = scApp.getBCFApplication().BC_ID;
            SCAppConstants.BCSystemInitialRtnCode rtnCode = SCAppConstants.BCSystemInitialRtnCode.Normal;
            try
            {
                using (DBConnection_EF conn = new DBConnection_EF())
                {
                    BCSTAT bc = bcStatusDao.getBCStatByID(conn, bc_id);
                    if (bc == null)
                    {
                        bc = new BCSTAT()
                        {
                            BC_ID = bc_id,
                            CLOSE_MODE = SCAppConstants.BCSystemStatus.Default,
                            RUN_TIMESTAMP = BCFUtility.formatDateTime(DateTime.Now, SCAppConstants.TimestampFormat_16)
                        };
                        bcStatusDao.insertBCStat(conn, bc);
                    }
                    else
                    {
                        if (BCFUtility.isMatche(bc.CLOSE_MODE, SCAppConstants.BCSystemStatus.NormalClosed))
                        {
                            logger.Info("Normal shutdown BC System!");
                            rtnCode = SCAppConstants.BCSystemInitialRtnCode.Normal;
                        }
                        else
                        {
                            logger.Info("Non-normal shutdown BC System!");
                            rtnCode = SCAppConstants.BCSystemInitialRtnCode.NonNormalShutdown;
                        }
                        bc.CLOSE_MODE = SCAppConstants.BCSystemStatus.Default;
                        bcStatusDao.updateBCStat(conn, bc);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Warn("Insert Failed to BCSTAT [BC_ID:{0}]", bc_id, ex);
                rtnCode = SCAppConstants.BCSystemInitialRtnCode.Error;
            }
            finally
            {

            }
            return rtnCode;
        }

        /// <summary>
        /// Res the write bc system run time.
        /// </summary>
        public void reWriteBCSystemRunTime()
        {
            string bc_id = scApp.getBCFApplication().BC_ID;
            try
            {
                using (DBConnection_EF conn = new DBConnection_EF())
                {
                    BCSTAT bc = bcStatusDao.getBCStatByID(conn, bc_id);
                    bc.RUN_TIMESTAMP = BCFUtility.formatDateTime(DateTime.Now, SCAppConstants.TimestampFormat_16);
                    bcStatusDao.updateBCStat(conn, bc);
                }
            }
            catch (Exception ex)
            {
                logger.Warn("Update Failed to BCSTAT [BC_ID:{0}]", bc_id, ex);
            }
            finally
            {
            }
        }

        /// <summary>
        /// Closes the bc system.
        /// </summary>
        /// <returns>Boolean.</returns>
        public Boolean closeBCSystem()
        {
            string bc_id = scApp.getBCFApplication().BC_ID;
            try
            {
                using (DBConnection_EF conn = new DBConnection_EF())
                {
                    BCSTAT bc = bcStatusDao.getBCStatByID(conn, bc_id);
                    bc.CLOSE_MODE = SCAppConstants.BCSystemStatus.NormalClosed;
                    bcStatusDao.updateBCStat(conn, bc);
                }
            }
            catch (Exception ex)
            {
                logger.Warn("Update Failed to BCSTAT [BC_ID:{0}]", bc_id, ex);
                return false;
            }
            finally
            {
            }
            return true;
        }


        //EC Data
        /// <summary>
        /// Updates the system parameter.
        /// </summary>
        /// <param name="ecid">The ecid.</param>
        /// <param name="val">The value.</param>
        /// <param name="refreshSecsAgent">The refresh secs agent.</param>
        public void updateSystemParameter(string ecid, string val, Boolean refreshSecsAgent)
        {
            try
            {
                if (BCFUtility.isMatche(ecid, SCAppConstants.ECID_CONVERSATION_TIMEOUT))
                {
                    SystemParameter.setSECSConversactionTimeout(Convert.ToInt16(val));
                }
                else if (BCFUtility.isMatche(ecid, SCAppConstants.ECID_GEM_INITIAL_CONTROL_STATE))
                {
                    SystemParameter.setInitialHostMode(val);
                }
                else if (BCFUtility.isMatche(ecid, SCAppConstants.ECID_CONTROL_STATE_KEEPING_TIME))
                {
                    SystemParameter.setControlStateKeepTime(Convert.ToInt16(val));
                }
                else if (BCFUtility.isMatche(ecid, SCAppConstants.ECID_HEARTBEAT))
                {
                    SystemParameter.setHeartBeatSec(Convert.ToInt32(val));
                    //Restart Timer
                    ITimerAction timer = scApp.getBCFApplication().getTimerAction("SECSHeartBeat");
                    if (timer != null)
                    {
                        timer.start();
                    }
                }
                else if (BCFUtility.isMatche(ecid, SCAppConstants.ECID_DEVICE_ID))
                {
                    scApp.setSECSAgentDeviceID(Convert.ToInt32(val), refreshSecsAgent);
                }
                else if (BCFUtility.isMatche(ecid, SCAppConstants.ECID_T3))
                {
                    scApp.setSECSAgentT3Timeout(Convert.ToInt32(val), refreshSecsAgent);
                }
                else if (BCFUtility.isMatche(ecid, SCAppConstants.ECID_T5))
                {
                    scApp.setSECSAgentT5Timeout(Convert.ToInt32(val), refreshSecsAgent);
                }
                else if (BCFUtility.isMatche(ecid, SCAppConstants.ECID_T6))
                {
                    scApp.setSECSAgentT6Timeout(Convert.ToInt32(val), refreshSecsAgent);
                }
                else if (BCFUtility.isMatche(ecid, SCAppConstants.ECID_T7))
                {
                    scApp.setSECSAgentT7Timeout(Convert.ToInt32(val), refreshSecsAgent);
                }
                else if (BCFUtility.isMatche(ecid, SCAppConstants.ECID_T8))
                {
                    scApp.setSECSAgentT8Timeout(Convert.ToInt32(val), refreshSecsAgent);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        #region Operation History

        /// <summary>
        /// Adds the operation his.
        /// </summary>
        /// <param name="user_id">The user_id.</param>
        /// <param name="formName">Name of the form.</param>
        /// <param name="action">The action.</param>
        public void addOperationHis(string user_id, string formName, string action)
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                string timeStamp = BCFUtility.formatDateTime(DateTime.Now, SCAppConstants.TimestampFormat_19);
                HOPERATION his = new HOPERATION()
                {
                    T_STAMP = timeStamp,
                    USER_ID = user_id,
                    FORM_NAME = formName,
                    ACTION = action
                };
                SCUtility.PrintOperationLog(his);
                operationHisDao.insertOperationHis(conn, his);
                conn.Commit();
            }
            catch (Exception ex)
            {
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Error(ex);
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
        }

        #endregion Operation History
    }
}
