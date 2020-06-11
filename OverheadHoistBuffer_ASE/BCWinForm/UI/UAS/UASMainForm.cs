//*********************************************************************************
//      UASMainForm.cs
//*********************************************************************************
// File Name: UASMainForm.cs
// Description: UAS Main Form
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date               Author       Request No.  Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2016/01/03    Steven Hong    N/A            A0.01   Add User Group Management
// 2016/04/06    Harris Kuo     N/A            A0.02   修改按下"Close"後,可以關閉整個UASMainForm
// 2016/04/08    Harris Kuo     N/A            A0.03   修改form size
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
using com.mirle.ibg3k0.sc.Common;
using NLog;

namespace com.mirle.ibg3k0.bc.winform.UI.UAS
{
    public partial class UASMainForm : Form
    {
        BCApplication bcApp = BCApplication.getInstance();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        //UI
        private UserMngForm userMngForm = null;
        private FunctionMngForm funcMngForm = null;
        private UserGrpMngForm userGrpMngForm = null;  //A0.01

        public UASMainForm()
        {
            InitializeComponent();
            this.Width = 1020; //A0.03

        }

        //A0.01 Start
        private void userManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (userMngForm == null || userMngForm.IsDisposed)
            {
                userMngForm = new UserMngForm(this);  //A0.02
                userMngForm.MdiParent = this;
                userMngForm.Show();
                userMngForm.Focus();
            }
            else
            {
                userMngForm.rework();
                userMngForm.Focus();
            }
        }

        private void userGroupManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (userGrpMngForm == null || userGrpMngForm.IsDisposed)
                {
                    userGrpMngForm = new UserGrpMngForm(this);   //A0.02
                    userGrpMngForm.MdiParent = this;
                    userGrpMngForm.Show();
                    userGrpMngForm.Focus();
                }
                else
                {
                    userGrpMngForm.rework();
                    userGrpMngForm.Focus();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }
        //private void UserMngMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (userMngForm == null || userMngForm.IsDisposed)
        //    {
        //        userMngForm = new UserMngForm();
        //        userMngForm.MdiParent = this;
        //        userMngForm.Show();
        //        userMngForm.Focus();
        //    }
        //    else
        //    {
        //        userMngForm.rework();
        //        userMngForm.Focus();
        //    }
        //}
        //A0.01 End

        public void rework()
        {
            Visible = true;
            //視窗最大化
            this.WindowState = FormWindowState.Normal;
            this.Refresh();
        }

        private void FunctionMngMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (funcMngForm == null || funcMngForm.IsDisposed)
                {
                    funcMngForm = new FunctionMngForm(this);  //A0.02 
                    funcMngForm.MdiParent = this;
                    //funcMngForm.WindowState = FormWindowState.Normal;
                    //funcMngForm.WindowState = FormWindowState.Maximized;
                    funcMngForm.Show();
                    funcMngForm.Focus();
                }
                else
                {
                    funcMngForm.rework();
                    funcMngForm.Focus();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        public void setLoginUserID(string user_id)
        {
            if (SCUtility.isEmpty(user_id))
            {
                this.UASShowLoginUserIDLbl.Text = "";
            }
            else
            {
                this.UASShowLoginUserIDLbl.Text = user_id.Trim();
            }
        }

        private void UASMainForm_Load(object sender, EventArgs e)
        {
            bcApp.addUserToolStripStatusLabel(this.UASShowLoginUserIDLbl);
            openForm_UserMng();
        }

        private void logoffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bcApp.logoff();
            Close();
        }

        private void openForm_UserMng()
        {
            UserMngMenuItem_Click(null, null);
        }

        private void UserMngMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (userMngForm == null || userMngForm.IsDisposed)
                {
                    userMngForm = new UserMngForm(this);
                    userMngForm.MdiParent = this;
                    //userMngForm.WindowState = FormWindowState.Normal;
                    //userMngForm.WindowState = FormWindowState.Maximized;
                    userMngForm.Show();
                    userMngForm.Focus();
                }
                else
                {
                    userMngForm.rework();
                    userMngForm.Focus();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        private void userGroupManagementToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (userGrpMngForm == null || userGrpMngForm.IsDisposed)
            {
                userGrpMngForm = new UserGrpMngForm(this);
                userGrpMngForm.MdiParent = this;
                userGrpMngForm.WindowState = FormWindowState.Normal;
                userGrpMngForm.WindowState = FormWindowState.Maximized;
                userGrpMngForm.Show();
                userGrpMngForm.Focus();
            }
            else
            {
                userGrpMngForm.rework();
                userGrpMngForm.Focus();
            }
        }

        string menuButton_min = "(&N)";
        string menuButton_revert = "(&R)";
        string menuButton_close = "(&C)";
        private void UASMenuBar_ItemAdded(object sender, ToolStripItemEventArgs e)
        {
            if (e.Item.Text.Length == 0 //隐藏子窗体图标  
                || e.Item.Text.Contains(menuButton_min)
                || e.Item.Text.Contains(menuButton_revert)
                || e.Item.Text.Contains(menuButton_close))
            {
                e.Item.Visible = false;
            }
        }

    }
}
