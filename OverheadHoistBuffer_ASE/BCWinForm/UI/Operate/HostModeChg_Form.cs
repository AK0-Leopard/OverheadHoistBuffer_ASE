using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.App;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static com.mirle.ibg3k0.sc.ALINE;
using static com.mirle.ibg3k0.sc.App.SCAppConstants.LineHostControlState;

namespace com.mirle.ibg3k0.bc.winform.UI
{
    public partial class HostModeChg_Form : Form
    {
        BCMainForm MainForm;
        SCApplication scApp;

        bool hsmsconn_stat;
        bool HSMSConn_STAT
        {
            get { return hsmsconn_stat; }
            set
            {
                hsmsconn_stat = value;
                setInfo(txtCommuntion,
                    hsmsconn_stat ? "Enable" : "Disable",
                    hsmsconn_stat ? Color.Lime : Color.Red,
                    hsmsconn_stat ? Color.Black : Color.White);
            }
        }

        HostControlState hostconn_stat;
        HostControlState HostConn_STAT
        {
            get { return hostconn_stat; }
            set
            {
                hostconn_stat = value;
                if (hostconn_stat == HostControlState.EQ_Off_line)
                {
                    setInfo(txtHostMode, "Off Line", Color.Red, Color.White);
                }
                else if (hostconn_stat == HostControlState.Going_Online)
                {
                    setInfo(txtHostMode, "Going on line", Color.Lime, Color.Black);
                }
                else if (hostconn_stat == HostControlState.Host_Online)
                {
                    setInfo(txtHostMode, "Host online", Color.Lime, Color.Black);
                }
                else if (hostconn_stat == HostControlState.On_Line_Local)
                {
                    setInfo(txtHostMode, "On-Line Local", Color.Yellow, Color.Black);
                }
                else if (hostconn_stat == HostControlState.On_Line_Remote)
                {
                    setInfo(txtHostMode, "On-Line Remote", Color.Lime, Color.Black);
                }
            }
        }
        TSCState sc_stat;
        TSCState SCStat_STAT
        {
            get { return sc_stat; }
            set
            {
                sc_stat = value;
                if (sc_stat == TSCState.TSC_INIT)
                {
                    setInfo(lblLCSStatusValue, "Initiate", Color.Red, Color.White);
                }
                else if (sc_stat == TSCState.NONE)
                {
                    setInfo(lblLCSStatusValue, "None", Color.Red, Color.White);
                }
                else if (sc_stat == TSCState.PAUSING)
                {
                    setInfo(lblLCSStatusValue, "Pausing", Color.Yellow, Color.Black);
                }
                else if (sc_stat == TSCState.PAUSED)
                {
                    setInfo(lblLCSStatusValue, "Pause", Color.Yellow, Color.Black);
                }
                else if (sc_stat == TSCState.AUTO)
                {
                    setInfo(lblLCSStatusValue, "Auto", Color.Lime, Color.Black);
                }
            }
        }
        private void setInfo(Label setLable, string setText, Color setColor, Color setForeColor)
        {
            Adapter.BeginInvoke(new SendOrPostCallback((o1) =>
            {
                setLable.Text = setText;
            }), null);
            setLable.BackColor = setColor;
            setLable.ForeColor = setForeColor;
        }

        private void setInfo(TextBox setTextBox, string setText, Color setColor, Color setForeColor)
        {
            setTextBox.Text = setText;
            setTextBox.BackColor = setColor;
            setTextBox.ForeColor = setForeColor;
        }

        public HostModeChg_Form(BCMainForm _mainForm)
        {
            InitializeComponent();
            MainForm = _mainForm;
            scApp = MainForm.BCApp.SCApplication;
            timer1.Enabled = true;
        }

        private void butOnlineRemote_Click(object sender, EventArgs e)
        {
            if (!scApp.LineService.canOnlineWithHost())
            {
                MessageBox.Show("Has vh not ready");
            }
            else if (scApp.getEQObjCacheManager().getLine().Host_Control_State == HostControlState.On_Line_Remote)
            {
                MessageBox.Show("On line ready");
            }
            else
            {
                Task.Run(() => scApp.LineService.OnlineWithHostOp());

            }
        }

        private void butOffline_Click(object sender, EventArgs e)
        {
            if (scApp.getEQObjCacheManager().getLine().SCStats != TSCState.PAUSED)
            {
                MessageBox.Show("Please change tsc state to pause first.");
            }
            else if (scApp.getEQObjCacheManager().getLine().Host_Control_State == HostControlState.EQ_Off_line)
            {
                MessageBox.Show("Current is off line");
            }
            else
            {
                Task.Run(() => scApp.LineService.OfflineWithHostByOp());
            }
        }

        private void butLcsAuto_Click(object sender, EventArgs e)
        {
            Task.Run(() => scApp.getEQObjCacheManager().getLine().ResumeToAuto(scApp.ReportBLL));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            SCApplication scApp = SCApplication.getInstance();
            HostConn_STAT = scApp.getEQObjCacheManager().getLine().Host_Control_State;
            SCStat_STAT = scApp.getEQObjCacheManager().getLine().SCStats;
        }

        private void HostModeChg_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainForm.removeForm(this.Name);
            timer1.Enabled = false;

        }

        private void butDisable_Click(object sender, EventArgs e)
        {
            scApp.LineService.stopHostCommunication();
        }

        private void butEnable_Click(object sender, EventArgs e)
        {
            scApp.LineService.startHostCommunication();
        }

        private void butLcsPause_Click(object sender, EventArgs e)
        {
            Task.Run(() => scApp.LineService.TSCStateToPause(sc.Data.SECS.CSOT.SECSConst.PAUSE_REASON_OP));
        }
    }
}
