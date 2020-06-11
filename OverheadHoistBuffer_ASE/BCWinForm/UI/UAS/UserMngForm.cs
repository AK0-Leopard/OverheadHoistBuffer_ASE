//*********************************************************************************
//      UserMngForm.cs
//*********************************************************************************
// File Name: UserMngForm.cs
// Description: User Management Form
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date               Author       Request No.  Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2016/01/02    Steven Hong    N/A            A0.01   Add User Group
// 2016/04/01    Kevin Wei      N/A            A0.02   調整Datagridview的大小。
// 2016/04/06    Kevin Wei      N/A            A0.03   取消Power User、Disable的使用。
// 2016/04/06    Harris Kuo     N/A            A0.04    修改按下"Close"後,可以關閉整個UASMainForm
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
using com.mirle.ibg3k0.bc.winform.Common;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc;

namespace com.mirle.ibg3k0.bc.winform.UI.UAS
{
    public partial class UserMngForm : Form
    {
        private BCApplication bcApp = BCApplication.getInstance();

        private BindingSource userDataBS = new BindingSource();
        private List<UASUSR> userDataList = new List<UASUSR>();
        //private UASMainForm uasMainForm = new UASMainForm();
        private UASMainForm uasMainForm = null;  //A0.04
        public UserMngForm(UASMainForm _uasmainform)  //A0.04
        {
            InitializeComponent();
            uasMainForm = _uasmainform;  //A0.04
            userDataBS.DataSource = userDataList;
            UserDataGridView.DataSource = userDataBS;

            //A0.01 Start
            List<UASUSRGRP> userGrpList = bcApp.SCApplication.UserBLL.loadAllUserGroup();
            foreach (UASUSRGRP usrGrp in userGrpList)
            {
                userGrpCbx.Items.Add(usrGrp.USER_GRP);
            }
            //A0.01 End

            Refresh();
            SCUtility.subInitGrdHeader(UserDataGridView); //A0.02
        }

        public void rework()
        {
            Visible = true;
            //視窗最大化
            this.WindowState = FormWindowState.Maximized;
            this.Refresh();
        }

        public override void Refresh() 
        {
            base.Refresh();
            reloadUserDataTable();
            clearTextBox();
        }

        private void reloadUserDataTable() 
        {
            this.Cursor = Cursors.WaitCursor;
            List<UASUSR> userList = bcApp.SCApplication.UserBLL.loadAllUser();
            //UserDataGridView.
            userDataList.Clear();
            userDataList.AddRange(userList);
            userDataBS.ResetBindings(false);
            this.Cursor = Cursors.Default;
        }

        private void clearTextBox() 
        {
            UserIDTbx.Clear();
            UserNameTbx.Clear();
            PswdTbx.Clear();
            Disable_Y_RBtn.Checked = true;
        }

        private Boolean checkInputDataForAdd() 
        {
            string user_id = this.UserIDTbx.Text.Trim();
            string user_name = this.UserNameTbx.Text.Trim();
            string passwd = this.PswdTbx.Text.Trim();

            if (SCUtility.isEmpty(user_id) || SCUtility.isEmpty(user_name) || SCUtility.isEmpty(passwd))
            {
                MessageBox.Show(this, BCApplication.getMessageString("USER_ID_NAME_PWD_CANNOT_BE_BLANK"),
                    BCApplication.getMessageString("WARNING"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private Boolean checkInputDataForUpdate()
        {
            string user_id = this.UserIDTbx.Text.Trim();
            string user_name = this.UserNameTbx.Text.Trim();
            string user_grp = this.userGrpCbx.Text.Trim();  //A0.01

            //A0.01 if (SCUtility.isEmpty(user_id) || SCUtility.isEmpty(user_name))
            if (SCUtility.isEmpty(user_id) || SCUtility.isEmpty(user_name) || SCUtility.isEmpty(user_grp))  //A0.01
            {
                MessageBox.Show(this, BCApplication.getMessageString("USER_ID_NAME_CANNOT_BE_BLANK"),
                    BCApplication.getMessageString("WARNING"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            if (!BCUtility.doLogin(this, bcApp, BCAppConstants.FUNC_USER_MANAGEMENT))
            {
                return;
            }
            string user_id = this.UserIDTbx.Text.Trim();
            string user_name = this.UserNameTbx.Text.Trim();
            string passwd = this.PswdTbx.Text.Trim();
            string userGrp = this.userGrpCbx.Text.Trim();  //A0.01
            //A0.03 Boolean isDisable = this.Disable_Y_RBtn.Checked;
            //A0.03 Boolean isPowerUser = this.Power_Y_RBtn.Checked;
            Boolean isDisable = false;   //A0.03
            Boolean isPowerUser = false; //A0.03

            if (!checkInputDataForAdd()) 
            {
                return;
            }

            Boolean createSuccess =
                bcApp.SCApplication.UserBLL.createUser(user_id, user_name, passwd, isDisable, userGrp,"","");  //A0.01
            //A0.01    bcApp.SCApplication.UserBLL.createUser(user_id, user_name, passwd, isDisable, isPowerUser);
            if (createSuccess)
            {
                //MessageBox.Show(this, BCApplication.getMessageString("CREATE_SUCCESS"));
                BCUtility.showMsgBox_Info(this, BCApplication.getMessageString("CREATE_SUCCESS"));
                Refresh();
            }
            else 
            {
                //MessageBox.Show(this, BCApplication.getMessageString("CREATE_FAILED"));
                BCUtility.showMsgBox_Info(this, BCApplication.getMessageString("CREATE_FAILED"));
            }
        }

        private UASUSR getSelectedRowToTextBox()
        {
            int selectedRowCnt = UserDataGridView.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCnt <= 0) 
            {
                return null;
            }
            int selectedIndex = UserDataGridView.SelectedRows[0].Index;
            if (userDataList.Count <= selectedIndex) 
            {
                return null;
            }
            UASUSR selectUser = userDataList[selectedIndex];
            return selectUser;
        }

        //A0.01 private void fillUserDataToTextBox(string user_id, string user_name, string disable_flg, string power_flg)
        private void fillUserDataToTextBox(string user_id, string user_name, string disable_flg, string power_flg, string user_grp)  //A0.01
        {
            UserIDTbx.Text = user_id.Trim();
            UserNameTbx.Text = user_name.Trim();
            userGrpCbx.Text = user_grp.Trim();  //A0.01
            //A0.03if (SCUtility.isMatche(disable_flg, SCAppConstants.YES_FLAG))
            //A0.03{
            //A0.03    Disable_Y_RBtn.Checked = true;
            //A0.03}
            //A0.03else
            //A0.03{
            //A0.03    Disable_N_RBtn.Checked = true;
            //A0.03}
            //A0.03if (SCUtility.isMatche(power_flg, SCAppConstants.YES_FLAG))
            //A0.03{
            //A0.03    Power_Y_RBtn.Checked = true;
            //A0.03}
            //A0.03else 
            //A0.03{
            //A0.03    Power_N_RBtn.Checked = true;
            //A0.03}
        }

        private void UserDataGridView_Click(object sender, EventArgs e)
        {
            UASUSR selectUser = getSelectedRowToTextBox();
            if (selectUser == null) 
            {
                return;
            }
            //A0.01 fillUserDataToTextBox(selectUser.User_ID, selectUser.User_Name, selectUser.Disable_Flg, selectUser.Power_User_Flg);
            fillUserDataToTextBox(selectUser.USER_ID, selectUser.USER_NAME, selectUser.DISABLE_FLG, selectUser.POWER_USER_FLG, selectUser.USER_GRP);  //A0.01
        }

        private void UpdBtn_Click(object sender, EventArgs e)
        {
            if (!BCUtility.doLogin(this, bcApp, BCAppConstants.FUNC_USER_MANAGEMENT)) 
            {
                return;
            }

            string user_id = this.UserIDTbx.Text.Trim();
            string user_name = this.UserNameTbx.Text.Trim();
            string passwd = this.PswdTbx.Text.Trim();
            string user_grp = this.userGrpCbx.Text.Trim();  //A0.01
            Boolean isDisable = this.Disable_Y_RBtn.Checked;
            Boolean isPowerUser = this.Power_Y_RBtn.Checked;

            if (!checkInputDataForUpdate()) 
            {
                return;
            }

            Boolean updateSuccess =
                bcApp.SCApplication.UserBLL.updateUser(user_id, user_name, passwd, isDisable, user_grp,"","");  //A0.01
            //A0.01     bcApp.SCApplication.UserBLL.updateUser(user_id, user_name, passwd, isDisable, isPowerUser);

            if (updateSuccess)
            {
                //MessageBox.Show(this, BCApplication.getMessageString("UPDATE_SUCCESS"));
                BCUtility.showMsgBox_Info(this, BCApplication.getMessageString("UPDATE_SUCCESS"));
                Refresh();
            }
            else
            {
                //MessageBox.Show(this, BCApplication.getMessageString("UPDATE_FAILED"));
                BCUtility.showMsgBox_Info(this, BCApplication.getMessageString("UPDATE_FAILED"));
            }
        }

        private void DelBtn_Click(object sender, EventArgs e)
        {
            UASUSR selectUser = getSelectedRowToTextBox();
            if (selectUser == null)
            {
                return;
            }

            if (!BCUtility.doLogin(this, bcApp, BCAppConstants.FUNC_USER_MANAGEMENT))
            {
                return;
            }

            var confirmResult = MessageBox.Show(this, "Are you sure to delete this item ?",
                        "Confirm Delete!",
                        MessageBoxButtons.YesNo);
            if (confirmResult != DialogResult.Yes) 
            {
                return;
            }
            
            Boolean deleteSuccess = bcApp.SCApplication.UserBLL.deleteUser(selectUser.USER_ID);
            if (deleteSuccess)
            {
                //MessageBox.Show(this, BCApplication.getMessageString("DELETE_SUCCESS"));
                BCUtility.showMsgBox_Info(this, BCApplication.getMessageString("DELETE_SUCCESS"));
                Refresh();
            }
            else 
            {
                //MessageBox.Show(this, BCApplication.getMessageString("DELETE_FAILED"));
                BCUtility.showMsgBox_Info(this, BCApplication.getMessageString("DELETE_FAILED"));
            }
        }

        private void clo_btn_Click(object sender, EventArgs e)
        {
            uasMainForm.Close();  //A0.04
        }

        /*A0.01
        private void RgsFuncBtn_Click(object sender, EventArgs e)
        {
            if (!BCUtility.doLogin(this, bcApp, BCAppConstants.FUNC_USER_MANAGEMENT))
            {
                return;
            }
            User selectUser = getSelectedRowToTextBox();
            if (selectUser == null)
            {
                return;
            }

            FuncCodePopupForm funcCodePopupForm = new FuncCodePopupForm();
            funcCodePopupForm.setUserID(selectUser.User_ID);
            DialogResult result = funcCodePopupForm.ShowDialog(this);
            List<FunctionCode> selectFuncCodeList = new List<FunctionCode>();
            if (result == DialogResult.OK)
            {
                selectFuncCodeList = funcCodePopupForm.getSelectFunctionCodeList();
                funcCodePopupForm.Dispose();
            }
            else 
            {
                funcCodePopupForm.Dispose();
                return;
            }
            if (!BCUtility.doLogin(this, bcApp, BCAppConstants.FUNC_USER_MANAGEMENT))
            {
                return;
            }
            //List<FunctionCode> selectFuncCodeList = funcCodePopupForm.getSelectFunctionCodeList();
            
            List<string> funcCodes = selectFuncCodeList.Select(o => o.Func_Code.Trim()).ToList();
            Boolean registerSuccess = bcApp.SCApplication.UserBLL.registerUserFunc(selectUser.User_ID, funcCodes);
            if (registerSuccess)
            {
                MessageBox.Show(this, BCApplication.getMessageString("REGISTER_SUCCESS"));
                Refresh();
            }
            else
            {
                MessageBox.Show(this, BCApplication.getMessageString("REGISTER_FAILED"));
            }
        }*/

    }
}

