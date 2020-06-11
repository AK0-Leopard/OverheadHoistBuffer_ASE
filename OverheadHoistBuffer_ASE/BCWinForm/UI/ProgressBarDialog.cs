//*********************************************************************************
//      ProgressBarDialog.cs
//*********************************************************************************
// File Name: ProgressBarDialog.cs
// Description: Progress Bar Dialog
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2014/06/21    Hayes Chen     N/A            N/A     Initial Release
// 2016/12/05    Eric Chiang    N/A            A0.01   Change Loading Picture
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
using NLog;

namespace com.mirle.ibg3k0.bc.winform.UI
{
    public partial class ProgressBarDialog : Form
    {
        private System.Threading.ManualResetEvent initEvent = new System.Threading.ManualResetEvent(false);
        private System.Threading.ManualResetEvent abortEvent = new System.Threading.ManualResetEvent(false);
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public delegate void SetTextInvoker(String text);
        private bool requiresClose = true;
        private BCApplication bcApp = null;
        public ProgressBarDialog(BCApplication bcApp)
        {
            this.bcApp = bcApp;
            InitializeComponent();
            //A0.01 SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            m_processLbl.Text = string.Empty;
            //A0.01 this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            //A0.01 this.Width = this.BackgroundImage.Width;
            //A0.01 this.Height = this.BackgroundImage.Height;
            //A0.01 this.TransparencyKey = Color.FromArgb(0, 112, 192);
            //A0.01 m_circularProgressBar1.BackColor = Color.Transparent;
            //A0.01 m_processLbl.BackColor = Color.Transparent;
        }

        Point lastPoint;
        private void ProgressBarDialog_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        private void ProgressBarDialog_MouseMove(object sender, MouseEventArgs e)
        {

        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            //         ControlBox = false;
            initEvent.Set();
        }

        public void Begin()
        {
            initEvent.WaitOne();
        }

        public void SetText(String text)
        {
            try
            {
                Invoke(new SetTextInvoker(DoSetText), new object[] { text });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }

        private void DoSetText(String text)
        {
            m_processLbl.Text = text;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            requiresClose = false;
            AbortWork();
            base.OnClosing(e);
        }

        private void AbortWork()
        {
            abortEvent.Set();
        }

        public void End()
        {
            if (requiresClose)
            {
                Invoke(new MethodInvoker(DoEnd));
            }
        }

        private void DoEnd()
        {
            Close();
            //A0.01 m_circularProgressBar1.Dispose();
            if (!IsDisposed)
            {
                Dispose();
            }
        }

        //A0.01 Start
        private void m_msgTm_Tick(object sender, EventArgs e)
        {
            if (m_processLbl.Visible)
                m_processLbl.Visible = false;
            else
                m_processLbl.Visible = true;
        }
        //A0.01 End
    }
}
