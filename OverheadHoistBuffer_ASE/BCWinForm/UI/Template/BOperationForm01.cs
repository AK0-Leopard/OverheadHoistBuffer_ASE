//*********************************************************************************
//      BOperationForm01.cs
//*********************************************************************************
// File Name: BOperationForm01.cs
// Description: Basic Operation Form
//
//(c) Copyright 2015, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2015/09/11    Steven Hong    N/A            N/A     Initial Release
//**********************************************************************************

using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using NLog;

namespace com.mirle.ibg3k0.bc.winform.UI
{
    public partial class BOperationForm01 : Form
    {
        // Function & Security Define
        protected string TitleName = string.Empty;
        protected TableLayoutPanel tlpDataLayOut = null;
        private bool pbolPause = false;

        public BOperationForm01()
        {
            InitializeComponent();
        }

        #region Init Form
        private void BOperationForm01_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Return)
                SendKeys.Send("{TAB}");
        }

        protected virtual void SubInit()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            formInit_Function();

            //Frame Set
            subinitTableLayoutPanel();
            if(!string.IsNullOrWhiteSpace(TitleName))
                lblProdCaption.Text = TitleName;


            //Title
            lblProdCaption.Font = new System.Drawing.Font("Cambria", 22, System.Drawing.FontStyle.Bold);
            formInit_Data();
        }

        /// <summary>
        /// 功能按鍵開關 BY Security
        /// </summary>
        private void formInit_Function()
        {
            //關閉功能
            butCreate.Visible = false;
            butRefresh.Visible = false;
            butSave.Visible = false;
            butStart.Visible = false;
            butStop.Visible = false;

            //隱藏功能
            butCreate.Enabled = false;
            butSave.Enabled = false;
            butStart.Enabled = false;
            butStop.Enabled = false;
        }

        protected virtual void formInit_Data()
        {
        }

        protected void subinitTableLayoutPanel()
        {
            try
            {
                if(pnlData.Controls.Count > 0)
                    tlpDataLayOut = pnlData.Controls.OfType<TableLayoutPanel>().ElementAt(0) as TableLayoutPanel;
            }
            catch(Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex, "Exception:"); 
            }
        }
        #endregion

        #region Create Event
        protected virtual void butCreate_Click(object sender, EventArgs e)
        {
            
        }
        #endregion

        #region Refresh Event
        protected virtual void butRefresh_Click(object sender, EventArgs e)
        {
            
        }
        #endregion

        #region Save Event
        protected virtual void butSave_Click(object sender, EventArgs e)
        {
            
        }
        #endregion

        #region Start Event
        protected virtual void butStart_Click(object sender, EventArgs e)
        {
            
        }
        #endregion

        #region Stop Event
        protected virtual void butStop_Click(object sender, EventArgs e)
        {
            
        }
        #endregion

        #region Close
        private void butReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected virtual void BOperationForm01_FormClosed(object sender, FormClosedEventArgs e)
        {
        }
        #endregion

        #region Other
        protected virtual void butPause_Click(object sender, EventArgs e)
        {
            
        }
        
        #endregion
    }
}
