//*********************************************************************************
//      BCMainForm.cs
//*********************************************************************************
// File Name: BCMainForm.cs
// Description: BC Main Form
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date               Author       Request No.  Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using com.mirle.ibg3k0.bc.winform.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.bc.winform.UI;
using NLog;
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using System.Threading;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction;
using com.mirle.ibg3k0.sc.Data.SECS;
using System.Reflection;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.bc.winform.UI.UAS;
using System.Diagnostics;
using com.mirle.ibg3k0.bc.winform.Common;
using com.mirle.ibg3k0.sc.MQTT;
using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.bc.winform.UI.Test;

namespace com.mirle.ibg3k0.bc.winform
{//test
    /// <summary>
    /// Class BCMainForm.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class BCMainForm : Form
    {
        /// <summary>
        /// The MPC tip MSG log
        /// </summary>
        private static Logger mpcTipMsgLog = LogManager.GetLogger("MPCTipMessageLog");
        /// <summary>
        /// The master pc memory log
        /// </summary>
        private static Logger masterPCMemoryLog = LogManager.GetLogger("MasterPCMemory");

        /// <summary>
        /// The bc application
        /// </summary>
        private BCApplication bcApp = null;
        /// <summary>
        /// Gets the bc application.
        /// </summary>
        /// <value>The bc application.</value>
        public BCApplication BCApp
        {
            get { return bcApp; }
        }
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// The open forms
        /// </summary>
        Dictionary<String, Form> openForms = new Dictionary<string, Form>();
        public Dictionary<String, Form> OpenForms { get { return openForms; } }
        /// <summary>
        /// The pic_ lock
        /// </summary>
        Image Pic_Lock = null;
        /// <summary>
        /// The pic_ unlock
        /// </summary>
        Image Pic_Unlock = null;

        /// <summary>
        /// The b c_ identifier
        /// </summary>
        string BC_ID = "";
        string ServerName = "";
        /// <summary>
        /// The ci
        /// </summary>
        CommonInfo ci;
        /// <summary>
        /// The line
        /// </summary>
        ALINE line;


        public bool isAutoOpenTip = true;
        /// <summary>
        /// Sets the information.
        /// </summary>
        /// <param name="setLable">The set lable.</param>
        /// <param name="setColor">Color of the set.</param>
        /// <param name="setForeColor">Color of the set fore.</param>
        private void setInfo(Label setLable, Color setColor, Color setForeColor)
        {
            setLable.BackColor = setColor;
            setLable.ForeColor = setForeColor;
        }

        /// <summary>
        /// Sets the information.
        /// </summary>
        /// <param name="setLable">The set lable.</param>
        /// <param name="setText">The set text.</param>
        /// <param name="setColor">Color of the set.</param>
        /// <param name="setForeColor">Color of the set fore.</param>
        private void setInfo(Label setLable, string setText, Color setColor, Color setForeColor)
        {
            setLable.Text = setText;
            setLable.BackColor = setColor;
            setLable.ForeColor = setForeColor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BCMainForm"/> class.
        /// </summary>
        /// <param name="bcid">The bcid.</param>
        public BCMainForm(string bcid, string server_name)
        {
            InitializeComponent();
            Adapter.Initialize();
            BC_ID = bcid;
            ServerName = server_name;
            Pic_Lock = global::com.mirle.ibg3k0.bc.winform.Properties.Resources.lock1;
            Pic_Unlock = global::com.mirle.ibg3k0.bc.winform.Properties.Resources.unlock;
            logger.Error("Error Test");
            //Application
            BCFApplication.addErrorMsgHandler(errorLogHandler);
            BCFApplication.addWarningMsgHandler(warnLogHandler);
            BCFApplication.addInfoMsgHandler(infoLogHandler);
        }

        #region Tip Message
        /// <summary>
        /// Errors the log handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="LogEventArgs"/> instance containing the event data.</param>
        private void errorLogHandler(Object sender, LogEventArgs args)
        {
            mpcTipMsgLog.Error(args.Message);
            Adapter.BeginInvoke(new SendOrPostCallback((o1) =>
            {
                MPCTipMessage tipMsg = new MPCTipMessage()
                {
                    MsgLevel = sc.ProtocolFormat.OHTMessage.MsgLevel.Error,
                    Msg = args.Message
                };
                ci.addMPCTipMsg(tipMsg);
                popUpMPCTipMessageDialog();
            }), null);
        }

        /// <summary>
        /// Warns the log handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="LogEventArgs"/> instance containing the event data.</param>
        private void warnLogHandler(Object sender, LogEventArgs args)
        {
            mpcTipMsgLog.Warn(args.Message);
            Adapter.BeginInvoke(new SendOrPostCallback((o1) =>
            {
                MPCTipMessage tipMsg = new MPCTipMessage()
                {
                    MsgLevel = sc.ProtocolFormat.OHTMessage.MsgLevel.Warn,
                    Msg = args.Message
                };
                ci.addMPCTipMsg(tipMsg);
                popUpMPCTipMessageDialog();
            }), null);
        }

        /// <summary>
        /// Informations the log handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="LogEventArgs"/> instance containing the event data.</param>
        private void infoLogHandler(Object sender, LogEventArgs args)
        {
            mpcTipMsgLog.Info(args.Message);
            Adapter.BeginInvoke(new SendOrPostCallback((o1) =>
            {
                MPCTipMessage tipMsg = new MPCTipMessage()
                {
                    MsgLevel = sc.ProtocolFormat.OHTMessage.MsgLevel.Info,
                    Msg = args.Message
                };
                ci.addMPCTipMsg(tipMsg);
                popUpMPCTipMessageDialog();
            }), null);
        }

        /// <summary>
        /// MPLCs the handshake timeout.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ErrorEventArgs"/> instance containing the event data.</param>
        private void mplcHandshakeTimeout(object sender, ErrorEventArgs e)
        {
            BCFApplication.onErrorMsg(String.Format("MPLC Handshake Timeout: {0}", e.ErrorMsg));
        }

        /// <summary>
        /// Pops up MPC tip message dialog.
        /// </summary>
        private void popUpMPCTipMessageDialog()
        {
            if (isAutoOpenTip)
                openForm(typeof(MPCInfoMsgDialog).Name, true, false);
        }
        #endregion Tip Message

        #region Initialze
        /// <summary>
        /// Handles the Load event of the BCMainForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void BCMainForm_Load(object sender, EventArgs e)
        {
            try
            {
                ProgressBarDialog progress = new ProgressBarDialog(bcApp);
                System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(callBackDoInitialize), progress);
                if (progress != null && !progress.IsDisposed)
                {
                    progress.ShowDialog();
                }
#if DEBUG
                //openForm(typeof(OHT_Form).Name);
                if (SCUtility.isMatche(BCApp.SCApplication.BC_ID, "ASE_LOOP"))
                {
                    openForm(typeof(OHT_FormNew).Name, true, false);
                }
                else
                {
                    openForm(typeof(OHT_Form).Name);
                }
#endif
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format("Exception: {0}", ex));
                logger.Error(ex, "Exception");
            }
        }
        /// <summary>
        /// Calls the back do initialize.
        /// </summary>
        /// <param name="status">The status.</param>
        private void callBackDoInitialize(Object status)
        {
            ProgressBarDialog progress = status as ProgressBarDialog;
            Adapter.Invoke(new SendOrPostCallback((o1) =>
            {
                progress.Begin();
                progress.SetText("Initialize...");
            }), null);
            bcApp = BCApplication.getInstance(ServerName);

            try
            {

                bcApp.addUserToolStripStatusLabel(tssl_LoginUser_Value);
                bcApp.addRefreshUIDisplayFun(delegate (object o) { UpdateUIDisplayByAuthority(); });
                bcApp.SCApplication.loadECDataToSystem();
                line = bcApp.SCApplication.getEQObjCacheManager().getLine();
                ci = bcApp.SCApplication.getEQObjCacheManager().CommonInfo;

                Adapter.Invoke(new SendOrPostCallback((o1) =>
                {
                    registerEvent();
                    initUI();
                }), null);

                //必須等到UI Event註冊完成後，才可以開啟通訊界面
                //bcApp.startProcess();
                bcApp.SCApplication.ParkBLL.setCurrentParkType();
                bcApp.SCApplication.CycleBLL.setCurrentCycleType();
                //line.addEventHandler(this.Name
                //, BCFUtility.getPropertyName(() => line.ServiceMode)
                //, (s1, e1) => { bcApp.SCApplication.FailOverService.ListenOrShutdownServerPort(); });


            }
            catch (Exception ex)
            {
                Adapter.Invoke(new SendOrPostCallback((o1) =>
                {
                    MessageBox.Show(this, ex.ToString());
                }), null);
                logger.Error(ex, "Exception");
            }
            finally
            {
                Adapter.Invoke(new SendOrPostCallback((o1) =>
                {
                    if (progress != null) { progress.End(); }
                }), null);
            }
        }

        string EventHandleId = "";
        /// <summary>
        /// Registers the event.
        /// </summary>
        private void registerEvent()
        {
            try
            {
                EventHandleId = this.Name;
                line.addEventHandler(EventHandleId, nameof(line.SegmentPreDisableExcuting), SegmentPreDisableExcute);
                ISMControl.addHandshakeTimeoutErrorHandler(mplcHandshakeTimeout);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        private void SegmentPreDisableExcute(object sender, bcf.Common.PropertyChangedEventArgs e)
        {
            string RoadControlFormName = typeof(RoadControlForm).Name;
            if (!openForms.ContainsKey(RoadControlFormName))
            {
                Adapter.Invoke((obj) => openForm(RoadControlFormName, true, false), null);
            }
        }

        /// <summary>
        /// Initializes the UI.
        /// </summary>
        private void initUI()
        {
            //設定 Form Title 顯示的字
            tssl_Version_Value.Text = SCAppConstants.getMainFormVersion("");
            tssl_Build_Date_Value.Text = SCAppConstants.GetBuildDateTime().ToString();

            Login_DefaultUser();
        }
        #endregion Initialze

        /// <summary>
        /// Cpus the memory monitor.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void cpuMemoryMonitor(object obj)
        {
            while (true)
            {
                System.Diagnostics.Process ps = System.Diagnostics.Process.GetCurrentProcess();
                try
                {
                    PerformanceCounter pf1 = new PerformanceCounter("Process", "Working Set - Private", ps.ProcessName);
                    PerformanceCounter pf2 = new PerformanceCounter("Process", "Working Set", ps.ProcessName);

                    masterPCMemoryLog.Debug("{0}:{1}  {2:N}KB", ps.ProcessName, "工作集(Process)", ps.WorkingSet64 / 1024);
                    masterPCMemoryLog.Debug("{0}:{1}  {2:N}KB", ps.ProcessName, "工作集        ", pf2.NextValue() / 1024);
                    masterPCMemoryLog.Debug("{0}:{1}  {2:N}KB", ps.ProcessName, "私有工作集    ", pf1.NextValue() / 1024);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception:");
                }

                Thread.Sleep(10000);
            }
        }




        /// <summary>
        /// Login_s the default user.
        /// </summary>
        private void Login_DefaultUser()
        {
            bcApp.login(BCAppConstants.LOGIN_USER_DEFAULT);
        }


        /// <summary>
        /// Updates the UI display by authority.
        /// </summary>
        private void UpdateUIDisplayByAuthority()
        {
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            MemberInfo[] memberInfos = typeof(BCMainForm).GetMembers(flag);

            var typeSwitch = new TypeSwitch()
                .Case((System.Windows.Forms.ToolStripMenuItem tsm, bool tf) => { tsm.Enabled = tf; })
                .Case((System.Windows.Forms.ComboBox cb, bool tf) => { cb.Enabled = tf; });

            foreach (MemberInfo memberInfo in memberInfos)
            {
                Attribute AuthorityCheck = memberInfo.GetCustomAttribute(typeof(AuthorityCheck));
                if (AuthorityCheck != null)
                {
                    string attribute_FUNName = ((AuthorityCheck)AuthorityCheck).FUNCode;
                    FieldInfo info = (FieldInfo)memberInfo;
                    if (bcApp.SCApplication.UserBLL.checkUserAuthority(bcApp.LoginUserID, attribute_FUNName))
                    {
                        typeSwitch.Switch(info.GetValue(this), true);
                    }
                    else
                    {
                        typeSwitch.Switch(info.GetValue(this), false);
                    }
                }
            }
        }


        /// <summary>
        /// Class TypeSwitch.
        /// </summary>
        public class TypeSwitch
        {
            /// <summary>
            /// The matches
            /// </summary>
            Dictionary<Type, Action<object, bool>> matches = new Dictionary<Type, Action<object, bool>>();
            /// <summary>
            /// Cases the specified action.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="action">The action.</param>
            /// <returns>TypeSwitch.</returns>
            public TypeSwitch Case<T>(Action<T, bool> action) { matches.Add(typeof(T), (x, enabled) => action((T)x, enabled)); return this; }
            /// <summary>
            /// Switches the specified x.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="enabled">if set to <c>true</c> [enabled].</param>
            public void Switch(object x, bool enabled)
            {
                if (matches.ContainsKey(x.GetType()))
                    matches[x.GetType()](x, enabled);
                else
                    logger.Warn("Switch Type:[{0}], Not exist!!!", x.GetType().Name);
            }
        }

        /// <summary>
        /// Does the start connection.
        /// </summary>
        /// <param name="status">The status.</param>
        private void doStartConnection(object status)
        {
            ProgressBarDialog progress = status as ProgressBarDialog;
            progress.Begin();
            progress.SetText(BCApplication.getMessageString("START_CONNECTING"));
            bcApp.startProcess();

            //Do something...

            progress.End();
        }

        /// <summary>
        /// Does the stop connection.
        /// </summary>
        /// <param name="status">The status.</param>
        private void doStopConnection(object status)
        {
            ProgressBarDialog progress = status as ProgressBarDialog;
            progress.Begin();
            progress.SetText(BCApplication.getMessageString("STOP_CONNECTING"));

            bcApp.stopProcess();

            //Do something...

            progress.End();
        }

        #region Sub Form Open Event



        /// <summary>
        /// Opens the form.
        /// </summary>
        /// <param name="formID">The form identifier.</param>
        public void openForm(String formID)
        {
            openForm(formID, false, false);
        }
        /// <summary>
        /// 開啟一般視窗 所使用
        /// </summary>
        /// <param name="formID">The form identifier.</param>
        /// <param name="isPopUp">The is pop up.</param>
        /// <param name="forceConfirm">The force confirm.</param>
        public void openForm(String formID, Boolean isPopUp, Boolean forceConfirm)
        {
            Form form;
            //string FormName = formID.Split('.')[1];
            if (openForms.ContainsKey(formID))
            {
                form = (Form)openForms[formID];
                if (isPopUp)
                {
                    form.Activate();
                    if (forceConfirm)
                    {
                        form.Close();
                        if (form != null && !form.IsDisposed) { form.Dispose(); }
                        removeForm(formID);
                        openForm(formID, isPopUp, forceConfirm);
                        return;
                    }
                    else
                    {
                        form.Show();
                    }
                    form.Focus();
                }
                else
                {
                    form.Activate();
                    form.Show();
                    form.Focus();
                    form.AutoScroll = true;
                    //form.WindowState = FormWindowState.Normal;
                    form.WindowState = FormWindowState.Maximized;
                }
            }
            else
            {
                try
                {
                    Type t = Type.GetType(String.Format("com.mirle.ibg3k0.bc.winform.UI.{0}", formID));
                    Object[] args = { this };
                    form = (Form)Activator.CreateInstance(t, args);
                    openForms.Add(formID, form);
                    if (isPopUp)
                    {
                        if (forceConfirm)
                        {
                            form.ShowDialog();
                        }
                        else
                        {
                            form.Show();
                        }
                        form.Focus();
                    }
                    else
                    {
                        if (!form.IsMdiContainer)
                        {
                            form.MdiParent = this;
                        }
                        form.Show();
                        form.Focus();
                        form.AutoScroll = true;
                        form.WindowState = FormWindowState.Normal;
                        form.WindowState = FormWindowState.Maximized;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception:");
                    MessageBox.Show(this, "This Fuction Not Enable", "Important Message");
                }
            }
        }


        /// <summary>
        /// 用來開啟SECS Massage Log視窗 所使用
        /// </summary>
        /// <param name="formID">The form identifier.</param>
        /// <param name="Stringparameter1">The stringparameter1.</param>
        public void openForm(String formID, string Stringparameter1)
        {
            Form form;
            //string FormName = formID.Split('.')[1];
            if (openForms.ContainsKey(formID))
            {
                form = (Form)openForms[formID];
                form.Activate();
                form.Show();
                form.Focus();
            }
            else
            {
                try
                {
                    Type t = Type.GetType(String.Format("com.mirle.ibg3k0.bc.winform.UI.{0}", formID));
                    Object[] args = { this, Stringparameter1 };
                    form = (Form)Activator.CreateInstance(t, args);
                    openForms.Add(formID, form);
                    if (!form.IsMdiContainer)
                    {
                        form.MdiParent = this;
                    }
                    form.Show();
                    form.Focus();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception:");
                    MessageBox.Show(this, "This Fuction Not Enable", "Important Message");
                }
            }
        }

        /// <summary>
        /// Removes the form.
        /// </summary>
        /// <param name="formID">The form identifier.</param>
        public void removeForm(String formID)
        {
            if (openForms.ContainsKey(formID))
            {
                openForms.Remove(formID);
            }
        }
        #endregion Sub Form Open Event

        /// <summary>
        /// Gets the message string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.String.</returns>
        public string getMessageString(string key, params object[] args)
        {
            return SCApplication.getMessageString(key, args);
        }

        #region Start Connection & Stop Connection
        /// <summary>
        /// Handles the Click event of the startConnectionToolStripMenuItem1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void startConnectionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //if (!BCUtility.doLogin(this, bcApp, BCAppConstants.FUNC_CONNECTION_MANAGEMENT))
            //{
            //    return;
            //}
            try
            {
                ProgressBarDialog progress = new ProgressBarDialog(bcApp);
                System.Threading.ThreadPool.QueueUserWorkItem(
                    new System.Threading.WaitCallback(doStartConnection), progress);
                if (progress != null && !progress.IsDisposed)
                {
                    progress.ShowDialog();
                }
                return;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        /// <summary>
        /// Handles the Click event of the stopConnectionToolStripMenuItem1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void stopConnectionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!BCUtility.doLogin(this, bcApp, BCAppConstants.FUNC_CONNECTION_MANAGEMENT))
            {
                return;
            }
            try
            {
                DialogResult confirmResult = MessageBox.Show(this, BCApplication.getMessageString("Confirm_STOP_CONNECTING"),
                    BCApplication.getMessageString("CONFIRM"), MessageBoxButtons.YesNo);
                if (confirmResult != System.Windows.Forms.DialogResult.Yes)
                {
                    return;
                }

                ProgressBarDialog progress = new ProgressBarDialog(bcApp);
                System.Threading.ThreadPool.QueueUserWorkItem(
                    new System.Threading.WaitCallback(doStopConnection), progress);
                if (progress != null && !progress.IsDisposed)
                {
                    progress.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }
        #endregion Start Connection & Stop Connection



        /// <summary>
        /// Handles the FormClosing event of the BCMainForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FormClosingEventArgs"/> instance containing the event data.</param>
        private void BCMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //TODO 確認Authority
            //if (!BCUtility.doLogin(this, bcApp, BCAppConstants.FUNC_CLOSE_MASTER_PC))
            //{
            //    //e.Cancel = true;
            //    //return;
            //}
            DialogResult confirmResult = MessageBox.Show(this, "Do you want to close OHTC?",
                BCApplication.getMessageString("CONFIRM"), MessageBoxButtons.YesNo);
            if (confirmResult != System.Windows.Forms.DialogResult.Yes)
            {
                e.Cancel = true;
            }
            if (e.Cancel == false)
            {

                try
                {
                    ProgressBarDialog progress = new ProgressBarDialog(bcApp);
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        new System.Threading.WaitCallback(doStopConnection), progress);
                    if (progress != null && !progress.IsDisposed)
                    {
                        progress.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
            }
        }


        #region UAS
        /// <summary>
        /// The uas main form
        /// </summary>
        private UASMainForm uasMainForm = null;

        /// <summary>
        /// Handles the Click event of the uASToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void uASToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!BCUtility.doLogin(this, bcApp, BCAppConstants.FUNC_USER_MANAGEMENT))
            {
                return;
            }
            if (uasMainForm == null || uasMainForm.IsDisposed)
            {
                uasMainForm = new UASMainForm();
                uasMainForm.Show();
                uasMainForm.Focus();
            }
            else
            {
                uasMainForm.rework();
                uasMainForm.Focus();
            }
        }

        /// <summary>
        /// Handles the Click event of the changePasswordToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!BCUtility.isLogin(bcApp))
            {
                if (!BCUtility.doLogin(this, bcApp))
                {
                    return;
                }
            }
            openForm("ChangePwdForm");
        }

        /// <summary>
        /// A0.19
        /// </summary>
        private void closeUasMainForm()
        {
            if (uasMainForm != null && !uasMainForm.IsDisposed)
            {
                uasMainForm.Close();
            }
        }
        #endregion


        /// <summary>
        /// Handles the Click event of the pic_Logout control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void pic_Logout_Click(object sender, EventArgs e)
        {
            BCUtility.doLogout(bcApp);
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the menuStrip1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void menuStrip1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && (ModifierKeys & Keys.Shift) == Keys.Shift && (ModifierKeys & Keys.Alt) == Keys.Alt)
            {
                MessageBox.Show(this, "UAS Fail.");
            }
        }

        private void logInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                BCUtility.doLogin(this, bcApp);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                BCUtility.doLogout(bcApp);
                MessageBox.Show(this, BCApplication.getMessageString("LOGOUT_SUCCESS")
                                , BCApplication.getMessageString("INFO")
                                , MessageBoxButtons.OK
                                , MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Localization.LCH.getLanguage() != "en-US")
            {
                Localization.LCH.changeConfigLanguage("en-US");
                MessageBox.Show("Language will be changed after restart the program.");
            }

        }

        private void zh_twToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Localization.LCH.getLanguage() != "zh-TW")
            {
                Localization.LCH.changeConfigLanguage("zh-TW");
                MessageBox.Show("Language will be changed after restart the program.");
            }

        }

        private void zh_chToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Localization.LCH.getLanguage() != "zh-CN")
            {
                Localization.LCH.changeConfigLanguage("zh-CN");
                MessageBox.Show("Language will be changed after restart the program.");
            }

        }

        private void tipMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openForm(typeof(MPCInfoMsgDialog).Name, true, false);
        }


        private void communectionStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openForm("MonitorForm", true, true);
        }

        public void entryMonitorMode()
        {
            //(openForms["OHT_Form"] as OHT_Form).entryMonitorMode();
        }
        public void LeaveMonitorMode()
        {
            //(openForms["OHT_Form"] as OHT_Form).LeaveMonitorMode();
        }

        private void engineerToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void BCMainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void debugToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            openForm(typeof(DebugForm).Name, true, false);
        }

        private void engineeringModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openForm(typeof(EngineerForm).Name, true, false);
        }

        private void roadControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openForm(typeof(RoadControlForm).Name, true, false);
        }

        private void zhTwToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.UICulture = new System.Globalization.CultureInfo("zh-TW");
            Properties.Settings.Default.Save();
            Thread.CurrentThread.CurrentUICulture = Properties.Settings.Default.UICulture;
        }

        private void enUSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.UICulture = new System.Globalization.CultureInfo("en-US");
            Properties.Settings.Default.Save();
            Thread.CurrentThread.CurrentUICulture = Properties.Settings.Default.UICulture;
        }

        private void sectionDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openForm(typeof(VehicleDataSettingForm).Name);
        }

        private void hostConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openForm(typeof(HostModeChg_Form).Name);
        }

        private void maintainDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openForm(typeof(MaintainDeviceForm).Name);
        }

        private void transferCommandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openForm(typeof(TransferCommandQureyListForm).Name, true, false);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            openForm(typeof(CarrierMaintenanceForm).Name, true, false);
        }

        private void reserveInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openForm(typeof(ReserveSectionInfoForm).Name, true, false);
        }

        private void 搬送模擬ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S6F11Demo demo = new S6F11Demo();
            demo.SetApp(BCApp);
            ShowFrom(demo, null);
        }

        private void port內容值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TestGetPortData demo = new TestGetPortData();
            demo.SetApp(BCApp);
            ShowFrom(demo, null);
        }

        private void 命令卡匣資料ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CMD_CST_DATA demo = new CMD_CST_DATA();
            demo.SetApp(BCApp);
            ShowFrom(demo, null);
        }

        private void waitInOutLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WaitInOutLog demo = new WaitInOutLog();
            demo.SetApp(BCApp);
            ShowFrom(demo, null);
        }
        public static void ShowFrom(Form frm, Form frmThis)
        {
            Form bb = FindForm(frm.Name);
            if (bb != null)
            {
                if (frmThis != null)
                {
                    bb.MdiParent = frmThis;
                }

                bb.BringToFront();
            }
            else
            {
                if (frmThis != null)
                {
                    frm.MdiParent = frmThis;
                }

                frm.Show();
            }
        }
        public static Form FindForm(string p_sFormName)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form.Name == p_sFormName)
                {
                    return form;
                }
            }
            return null;
        }


    }
}