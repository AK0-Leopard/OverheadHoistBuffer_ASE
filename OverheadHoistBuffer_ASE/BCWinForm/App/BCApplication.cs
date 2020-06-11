//*********************************************************************************
//      BCApplication.cs
//*********************************************************************************
// File Name: BCApplication.cs
// Description: Type 1 Function
//
//(c) Copyright 2015, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using com.mirle.ibg3k0.sc.App;
using NLog;
using com.mirle.ibg3k0.bcf.Common;
using System.Threading;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc;

namespace com.mirle.ibg3k0.bc.winform.App
{
    /// <summary>
    /// Class BCApplication.
    /// </summary>
    public class BCApplication
    {
        public static string ServerName { get; private set; }

        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The sc application
        /// </summary>
        private SCApplication scApp = null;
        /// <summary>
        /// Gets the sc application.
        /// </summary>
        /// <value>The sc application.</value>
        public SCApplication SCApplication { get { return scApp; } }

        #region UAS
        /// <summary>
        /// The login user identifier
        /// </summary>
        private string loginUserID = null;
        /// <summary>
        /// Gets the login user identifier.
        /// </summary>
        /// <value>The login user identifier.</value>
        public string LoginUserID { get { return loginUserID; } }
        /// <summary>
        /// The status user identifier label dic
        /// </summary>
        private Dictionary<string, System.Windows.Forms.ToolStripStatusLabel> statusUserIDLabelDic =
            new Dictionary<string, System.Windows.Forms.ToolStripStatusLabel>();
        /// <summary>
        /// The refresh_ UI display_ fun dic
        /// </summary>
        private Dictionary<string, Action<object>> refresh_UIDisplay_FunDic =
            new Dictionary<string, Action<object>>(); //A0.01
        #endregion UAS

        /// <summary>
        /// The main form
        /// </summary>
        private static Form mainForm = null;

        /// <summary>
        /// The bc application
        /// </summary>
        private static BCApplication bcApp = null;
        /// <summary>
        /// Prevents a default instance of the <see cref="BCApplication"/> class from being created.
        /// </summary>
        private BCApplication()
        {
            try
            {
                init();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                throw;
            }
        }

        /// <summary>
        /// The _lock
        /// </summary>
        private static Object _lock = new Object();
        /// <summary>
        /// The build value function
        /// </summary>
        private static com.mirle.ibg3k0.bcf.App.BCFApplication.BuildValueEventDelegate buildValueFunc;
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="_mainForm">The _main form.</param>
        /// <param name="_buildValueFunc">The _build value function.</param>
        /// <returns>BCApplication.</returns>
        public static BCApplication getInstance(Form _mainForm,
            com.mirle.ibg3k0.bcf.App.BCFApplication.BuildValueEventDelegate _buildValueFunc)
        {
            mainForm = _mainForm;
            buildValueFunc = _buildValueFunc;
            return getInstance();
        }
        public static BCApplication getInstance(string server_name)
        {
            ServerName = server_name;
            return getInstance();
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns>BCApplication.</returns>
        public static BCApplication getInstance()
        {
            if (bcApp == null)
            {
                lock (_lock)
                {
                    if (bcApp == null)
                    {
                        bcApp = new BCApplication();
                    }
                }
            }
            return bcApp;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void init()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(new string('=', 80));
            sb.AppendLine("Do BCApplication Initialize...");
            sb.AppendLine(new string('=', 80));
            logger.Info(sb.ToString());
            sb.Clear();
            sb = null;

            scApp = SCApplication.getInstance(ServerName, buildValueFunc);
            Boolean recoverFromDB = false;
            try
            {
                scApp.startBuildEqpts(recoverFromDB);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                throw;
            }
        }

        /// <summary>
        /// Determines whether this instance is started.
        /// </summary>
        /// <returns>Boolean.</returns>
        public Boolean isStarted()
        {
            return scApp.Started;
        }

        /// <summary>
        /// Starts the process.
        /// </summary>
        public void startProcess()
        {
            if (scApp.Started)
                return;
            scApp.start();
            scApp.startShareMemory(); // 測試TCPIP時，暫時不開啟PLC的連線
            scApp.startTcpIpServerListen();
            //SpinWait.SpinUntil(() => false, 10000);
            scApp.startSECSAgent();
            scApp.startHAProxyConnectionTest();
        }

        /// <summary>
        /// Starts the secs.
        /// </summary>
        public void startSECS()
        {
            scApp.startSECSAgent();
        }

        /// <summary>
        /// Stops the process.
        /// </summary>
        public void stopProcess()
        {
            scApp.getEQObjCacheManager().getLine().ServerPreStop = true;
            SpinWait.SpinUntil(() => false, 5000);

            scApp.stop();
            scApp.stopShareMemory();
            scApp.stopSECSAgent();
            scApp.stopTcpIpServer();

            scApp.FailOverService.DeleteMaserHeartbeat();
            scApp.CloseRedisConnection();
            scApp.hAProxyConnectionTest.shutDown();

            //scApp.ReportBLL.ZabbixPush(SCAppConstants.ZabbixServerInfo.ZABBIX_OHXC_ALIVE,
            //                           SCAppConstants.ZabbixOHxCAlive.ZABBIX_OHXC_ALIVE_CLOSE);
            //scApp.ReportBLL.ZabbixPush(SCAppConstants.ZabbixServerInfo.ZABBIX_OHXC_IS_ACTIVE, 0);

        }

        /// <summary>
        /// Stops the secs.
        /// </summary>
        public void stopSECS()
        {
            scApp.stopSECSAgent();
        }

        /// <summary>
        /// Gets the message string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.String.</returns>
        public static string getMessageString(string key, params object[] args)
        {
            return SCApplication.getMessageString(key, args);
        }






        #region UAS
        /// <summary>
        /// Logins the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        public void login(UASUSR user)
        {
            login(user.USER_ID);
        }

        /// <summary>
        /// Logins the specified user_id.
        /// </summary>
        /// <param name="user_id">The user_id.</param>
        public void login(string user_id)
        {
            loginUserID = user_id;
            refreshLoginUserInfo();
            refresh_UIDisplayFun();//A0.01
        }

        /// <summary>
        /// Refreshes the login user information.
        /// </summary>
        private void refreshLoginUserInfo()
        {
            foreach (System.Windows.Forms.ToolStripStatusLabel label in statusUserIDLabelDic.Values)
            {
                try
                {
                    if (label == null || label.IsDisposed)
                    {
                        continue;
                    }
                    label.Text = loginUserID;
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception:");
                }
            }
        }

        /// <summary>
        /// A0.01
        /// </summary>
        private void refresh_UIDisplayFun()
        {


            foreach (Action<object> action in refresh_UIDisplay_FunDic.Values)
            {
                try
                {
                    if (action == null)
                    {
                        continue;
                    }
                    action(new object());
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception:");
                }
            }
        }
        /// <summary>
        /// Logoffs this instance.
        /// </summary>
        public void logoff()
        {
            login("");
        }

        /// <summary>
        /// Adds the user tool strip status label.
        /// </summary>
        /// <param name="label">The label.</param>
        public void addUserToolStripStatusLabel(System.Windows.Forms.ToolStripStatusLabel label)
        {
            //statusUserIDLabelDic
            if (statusUserIDLabelDic.ContainsKey(label.Name))
            {
                statusUserIDLabelDic[label.Name] = label;
            }
            else
            {
                statusUserIDLabelDic.Add(label.Name, label);
            }
            refreshLoginUserInfo();
        }
        /// <summary>
        /// A0.01
        /// </summary>
        /// <param name="refreshFun">The refresh fun.</param>
        public void addRefreshUIDisplayFun(Action<object> refreshFun)
        {
            //statusUserIDLabelDic
            if (refresh_UIDisplay_FunDic.ContainsKey(refreshFun.Method.Name))
            {
                //TODO 修改"refreshFun.Method.Name"存入的名稱 Kevin Wei
                refresh_UIDisplay_FunDic[refreshFun.Method.Name] = refreshFun;
            }
            else
            {
                refresh_UIDisplay_FunDic.Add(refreshFun.Method.Name, refreshFun);
            }
        }
        //public void addRe
        #endregion

    }
}
