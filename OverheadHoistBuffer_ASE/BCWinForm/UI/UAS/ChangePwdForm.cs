//*********************************************************************************
//      ChangePwdForm.cs
//*********************************************************************************
// File Name: ChangePwdForm.cs
// Description: 
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2015/06/17    Kevin Wei       N/A            A0.01  增加舊密碼的確認。
// 2016/04/08    Harris Kuo      N/A            A0.02   調整打開視窗的大小。
//**********************************************************************************
using com.mirle.ibg3k0.bc.winform.App;
using com.mirle.ibg3k0.bc.winform.Common;
using com.mirle.ibg3k0.sc.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.mirle.ibg3k0.bc.winform.UI
{
    public partial class ChangePwdForm : BOperationForm01
    {

        BCApplication bcApp = null;
        BCMainForm mainForm = null;


        public ChangePwdForm(BCMainForm _mainForm)
        {
            InitializeComponent();
            mainForm = _mainForm;
            bcApp = BCApplication.getInstance();
            this.Width = 650; //A0.02
            base.TitleName = "Change Password";

        }

        private void ChangePwdForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainForm.removeForm("ChangePwdForm");
        }

        private void ChangePwdForm_Load(object sender, EventArgs e)
        {
            if (!BCUtility.isLogin(bcApp))
            {
                return;
            }
            m_yourIDTxb.Text = bcApp.LoginUserID;

        }

        protected override void butSave_Click(object sender, EventArgs e)
        {
            updateBtn();
        }


        //private void m_updateBtn_Click(object sender, EventArgs e)
        private void updateBtn()
        {
            string oldpasswd = SCUtility.Trim(m_oldPwdTxb.Text);
            string passwd = SCUtility.Trim(m_newPwdTxb.Text);
            string verPasswd = SCUtility.Trim(m_newPwdVerTxb.Text);

            if (!bcApp.SCApplication.UserBLL.checkUserPassword(bcApp.LoginUserID, oldpasswd))                   //A0.01
            {                                                                                                   //A0.01
                MessageBox.Show(this, BCApplication.getMessageString("Please_Check_Old_Password"),              //A0.01
                    BCApplication.getMessageString("INFO"), MessageBoxButtons.OK, MessageBoxIcon.Information);  //A0.01
                return;                                                                                         //A0.01
            }                                                                                                   //A0.01

            if (SCUtility.isEmpty(passwd) || SCUtility.isEmpty(verPasswd))
            {
                MessageBox.Show(this, BCApplication.getMessageString("Please_Input_New_Password"),
                    BCApplication.getMessageString("INFO"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!SCUtility.isMatche(passwd, verPasswd))
            {
                MessageBox.Show(this, BCApplication.getMessageString("Please_Check_New_Password"),
                    BCApplication.getMessageString("INFO"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Boolean updSuccess = bcApp.SCApplication.UserBLL.updatePassword(bcApp.LoginUserID, passwd);
            if (updSuccess)
            {
                //MessageBox.Show(this, BCApplication.getMessageString("UPDATE_SUCCESS"));
                BCUtility.showMsgBox_Info(this, BCApplication.getMessageString("UPDATE_SUCCESS"));
            }
            else
            {
                //MessageBox.Show(this, BCApplication.getMessageString("UPDATE_FAILED"));
                BCUtility.showMsgBox_Info(this, BCApplication.getMessageString("UPDATE_FAILED"));
            }
        }

        //private void close_btn_Click(object sender, EventArgs e)
        //{
        //    this.Close();
        //}
    }
}
