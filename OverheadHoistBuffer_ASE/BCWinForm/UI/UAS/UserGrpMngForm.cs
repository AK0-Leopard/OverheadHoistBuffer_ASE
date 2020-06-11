//*********************************************************************************
//      UserGrpMngForm.cs
//*********************************************************************************
// File Name: UserGrpMngForm.cs
// Description: User Group Management Form
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date               Author       Request No.  Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2016/01/02    Steven Hong    N/A            N/A     Initial Release
// 2016/04/01    Kevin Wei      N/A            A0.01   調整Datagridview的大小。
// 2016/04/06    Harris Kuo     N/A            A0.02   修改按下"Close"後,可以關閉整個UASMainForm
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
    public partial class UserGrpMngForm : Form
    {
        private BCApplication bcApp = BCApplication.getInstance();

        private BindingSource userGroupDataBS = new BindingSource();
        private List<UASUSRGRP> userGroupList = new List<UASUSRGRP>();
        private UASMainForm uasMainForm = null; //A0.02

        public UserGrpMngForm(UASMainForm _uasmainform)  //A0.02
        {
            InitializeComponent();
            uasMainForm = _uasmainform;  //A0.02
            userGroupDataBS.DataSource = userGroupList;
            UserGroupGridView.DataSource = userGroupDataBS;

            Refresh();
            SCUtility.subInitGrdHeader(UserGroupGridView); //A0.01
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
            reloadUserGroupTable();
            UserGroupTbx.Clear();
        }

        private void reloadUserGroupTable() 
        {
            this.Cursor = Cursors.WaitCursor;
            List<UASUSRGRP> groupList = bcApp.SCApplication.UserBLL.loadAllUserGroup();
            //UserDataGridView.
            userGroupList.Clear();
            userGroupList.AddRange(groupList);
            userGroupDataBS.ResetBindings(false);
            this.Cursor = Cursors.Default;
        }

        private Boolean checkInputData() 
        {
            string user_group = this.UserGroupTbx.Text.Trim();

            if (SCUtility.isEmpty(user_group))
            {
                MessageBox.Show(this, BCApplication.getMessageString("USER_GROUP_CANNOT_BE_BLANK"),
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
            string user_group = this.UserGroupTbx.Text.Trim();

            if (!checkInputData()) 
            {
                return;
            }

            Boolean createSuccess = bcApp.SCApplication.UserBLL.addUserGroup(user_group); 
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

        private UASUSRGRP getSelectedRowToTextBox()
        {
            int selectedRowCnt = UserGroupGridView.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCnt <= 0) 
            {
                return null;
            }
            int selectedIndex = UserGroupGridView.SelectedRows[0].Index;
            if (userGroupList.Count <= selectedIndex) 
            {
                return null;
            }
            UASUSRGRP selectUserGroup = userGroupList[selectedIndex];
            return selectUserGroup;
        }

        private void DelBtn_Click(object sender, EventArgs e)
        {
            UASUSRGRP selectUserGroup = getSelectedRowToTextBox();
            if (selectUserGroup == null)
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

            Boolean deleteSuccess = bcApp.SCApplication.UserBLL.deleteUserGroup(selectUserGroup.USER_GRP);
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

        private void RgsFuncBtn_Click(object sender, EventArgs e)
        {
            registerFunction();
            //if (!BCUtility.doLogin(this, bcApp, BCAppConstants.FUNC_USER_MANAGEMENT))
            //{
            //    return;
            //}
            //UserGroup selectUserGroup = getSelectedRowToTextBox();
            //if (selectUserGroup == null)
            //{
            //    return;
            //}

            //FuncCodePopupForm funcCodePopupForm = new FuncCodePopupForm();
            //funcCodePopupForm.setUserGroup(selectUserGroup.User_Grp);
            //DialogResult result = funcCodePopupForm.ShowDialog(this);
            //List<FunctionCode> selectFuncCodeList = new List<FunctionCode>();
            //if (result == DialogResult.OK)
            //{
            //    selectFuncCodeList = funcCodePopupForm.getSelectFunctionCodeList();
            //    funcCodePopupForm.Dispose();
            //}
            //else 
            //{
            //    funcCodePopupForm.Dispose();
            //    return;
            //}
            //if (!BCUtility.doLogin(this, bcApp, BCAppConstants.FUNC_USER_MANAGEMENT))
            //{
            //    return;
            //}
            
            //List<string> funcCodes = selectFuncCodeList.Select(o => o.Func_Code.Trim()).ToList();
            //Boolean registerSuccess = bcApp.SCApplication.UserBLL.registerUserFunc(selectUserGroup.User_Grp, funcCodes);
            //if (registerSuccess)
            //{
            //    MessageBox.Show(this, BCApplication.getMessageString("REGISTER_SUCCESS"));
            //    Refresh();
            //}
            //else
            //{
            //    MessageBox.Show(this, BCApplication.getMessageString("REGISTER_FAILED"));
            //}
        }

        private void registerFunction()
        {
            if (!BCUtility.doLogin(this, bcApp, BCAppConstants.FUNC_USER_MANAGEMENT))
            {
                return;
            }
            UASUSRGRP selectUserGroup = getSelectedRowToTextBox();
            if (selectUserGroup == null)
            {
                return;
            }

            FuncCodePopupForm funcCodePopupForm = new FuncCodePopupForm();
            funcCodePopupForm.setUserGroup(selectUserGroup.USER_GRP);
            DialogResult result = funcCodePopupForm.ShowDialog(this);
            List<UASFNC> selectFuncCodeList = new List<UASFNC>();
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

            List<string> funcCodes = selectFuncCodeList.Select(o => o.FUNC_CODE.Trim()).ToList();
            Boolean registerSuccess = bcApp.SCApplication.UserBLL.registerUserFunc(selectUserGroup.USER_GRP, funcCodes);
            if (registerSuccess)
            {
                //MessageBox.Show(this, BCApplication.getMessageString("REGISTER_SUCCESS"));
                BCUtility.showMsgBox_Info(this, BCApplication.getMessageString("REGISTER_SUCCESS"));
                Refresh();
            }
            else
            {
                //MessageBox.Show(this, BCApplication.getMessageString("REGISTER_FAILED"));
                BCUtility.showMsgBox_Info(this, BCApplication.getMessageString("REGISTER_FAILED"));
            }
        }

        private void UserGroupGridView_Click(object sender, EventArgs e)
        {
            UASUSRGRP selectUserGroup = getSelectedRowToTextBox();
            if (selectUserGroup == null)
            {
                return;
            }

            fillGroupDataToTextBox(selectUserGroup.USER_GRP); 
        }

        private void fillGroupDataToTextBox(string user_grp)
        {
            UserGroupTbx.Text = user_grp.Trim(); 
        }

        private void UserGroupGridView_DoubleClick(object sender, EventArgs e)
        {
            UASUSRGRP selectUserGroup = getSelectedRowToTextBox();
            if (selectUserGroup == null)
            {
                return;
            }
            registerFunction();
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            uasMainForm.Close();  //A0.02
        }

    }
}

