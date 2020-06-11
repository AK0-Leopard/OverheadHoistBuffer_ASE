//*********************************************************************************
//      SCUtility.cs
//*********************************************************************************
// File Name: SCUtility.cs
// Description: ScriptControl 共用工具元件
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
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
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bc.winform.Common;
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.sc;

namespace com.mirle.ibg3k0.bc.winform.UI.UAS
{
    public partial class FunctionMngForm : Form
    {
        private BCApplication bcApp = BCApplication.getInstance();
        private UASMainForm uasMainForm = null;   //A0.02
        private BindingSource funcCodeDataBS = new BindingSource();
        private List<UASFNC> funcCodeDataList = new List<UASFNC>();

        public FunctionMngForm(UASMainForm _uasmainform)  //A0.02
        {
            InitializeComponent();
            uasMainForm = _uasmainform;  //A0.02
            funcCodeDataBS.DataSource = funcCodeDataList;
            FuncCodeDataGridView.DataSource = funcCodeDataBS;
            Refresh();
            SCUtility.subInitGrdHeader(FuncCodeDataGridView); //A0.01
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
            reloadFuncCodeDataTable();
            clearTextBox();
        }

        private void clearTextBox() 
        {
            this.FuncCodeTBx.Clear();
            this.FuncNameTBx.Clear();
        }

        private void reloadFuncCodeDataTable() 
        {
            this.Cursor = Cursors.WaitCursor;
            List<UASFNC> funcCodeList = bcApp.SCApplication.UserBLL.loadAllFunctionCode();
            //UserDataGridView.
            funcCodeDataList.Clear();
            funcCodeDataList.AddRange(funcCodeList);
            funcCodeDataBS.ResetBindings(false);
            this.Cursor = Cursors.Default;
        }

        private UASFNC getSelectedRowToTextBox()
        {
            int selectedRowCnt = FuncCodeDataGridView.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCnt <= 0)
            {
                return null;
            }
            int selectedIndex = FuncCodeDataGridView.SelectedRows[0].Index;
            if (funcCodeDataList.Count <= selectedIndex)
            {
                return null;
            }
            UASFNC selectFuncCode = funcCodeDataList[selectedIndex];
            return selectFuncCode;
        }

        private void fillFuncCodeDataToTextBox(string func_code, string func_name)
        {
            FuncCodeTBx.Text = func_code.Trim();
            FuncNameTBx.Text = func_name.Trim();
        }

        private Boolean checkInputData()
        {
            string func_code = FuncCodeTBx.Text.Trim();
            string func_name = FuncNameTBx.Text.Trim();

            if (SCUtility.isEmpty(func_code) || SCUtility.isEmpty(func_name))
            {
                MessageBox.Show(this, BCApplication.getMessageString("FUNCTION_CODE_NAME_CANNOT_BE_BLANK"),
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
            string func_code = FuncCodeTBx.Text.Trim();
            string func_name = FuncNameTBx.Text.Trim();

            if (!checkInputData()) 
            {
                return;
            }

            Boolean createSuccess = 
                bcApp.SCApplication.UserBLL.createFunctionCode(func_code, func_name);
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

        private void UpdBtn_Click(object sender, EventArgs e)
        {
            if (!BCUtility.doLogin(this, bcApp, BCAppConstants.FUNC_USER_MANAGEMENT))
            {
                return;
            }
            string func_code = FuncCodeTBx.Text.Trim();
            string func_name = FuncNameTBx.Text.Trim();

            if (!checkInputData())
            {
                return;
            }

            Boolean updSuccess = 
                bcApp.SCApplication.UserBLL.updateFunctionCodeByCode(func_code, func_name);
            if (updSuccess)
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

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            if (!BCUtility.doLogin(this, bcApp, BCAppConstants.FUNC_USER_MANAGEMENT))
            {
                return;
            }
            UASFNC selectFuncCode = getSelectedRowToTextBox();
            if (selectFuncCode == null)
            {
                return;
            }
            if (BCFUtility.isMatche(BCAppConstants.FUNC_USER_MANAGEMENT, selectFuncCode.FUNC_CODE)) 
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
            
            Boolean deleteSuccess = 
                bcApp.SCApplication.UserBLL.deleteFunctionCodeByCode(selectFuncCode.FUNC_CODE);
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

        private void FuncCodeDataGridView_Click(object sender, EventArgs e)
        {
            UASFNC selectFuncCode = getSelectedRowToTextBox();
            if (selectFuncCode == null)
            {
                return;
            }
            fillFuncCodeDataToTextBox(selectFuncCode.FUNC_CODE, selectFuncCode.FUNC_NAME);
        }

        private void close_btn_Click(object sender, EventArgs e)
        {
            uasMainForm.Close();  //A0.02
        }
    }
}
