// ***********************************************************************
// Assembly         : BC
// Author           : chou
// Created          : 03-31-2016
//
// Last Modified By : chou
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="FuncCodePopupForm.cs" company="Mirle">
//     Copyright ©2014 MIRLE.3K0
// </copyright>
// <summary></summary>
// ***********************************************************************
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
using com.mirle.ibg3k0.sc;

namespace com.mirle.ibg3k0.bc.winform.UI.UAS
{
    /// <summary>
    /// Class FuncCodePopupForm.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class FuncCodePopupForm : Form
    {
        /// <summary>
        /// The bc application
        /// </summary>
        private BCApplication bcApp = BCApplication.getInstance();

        /// <summary>
        /// The function code data bs
        /// </summary>
        private BindingSource funcCodeDataBS = new BindingSource();
        /// <summary>
        /// The function code data list
        /// </summary>
        private List<UASFNC> funcCodeDataList = new List<UASFNC>();

        /// <summary>
        /// The user_id
        /// </summary>
        private string user_id = null;
        /// <summary>
        /// Sets the user identifier.
        /// </summary>
        /// <param name="user_id">The user_id.</param>
        public void setUserID(string user_id) 
        {
            this.user_id = user_id;
        }
        private string user_grp = null;
        public void setUserGroup(string user_grp)
        {
            this.user_grp = user_grp;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FuncCodePopupForm"/> class.
        /// </summary>
        public FuncCodePopupForm()
        {
            InitializeComponent();

            funcCodeDataBS.DataSource = funcCodeDataList;
            FuncCodeDataGridView.DataSource = funcCodeDataBS;
            Refresh();
        }

        /// <summary>
        /// 強制控制項使其工作區失效，並且立即重繪其本身和任何子控制項。
        /// </summary>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public override void Refresh()
        {
            base.Refresh();
            reloadFuncCodeDataTable();
        }

        /// <summary>
        /// Reloads the function code data table.
        /// </summary>
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

        /// <summary>
        /// Handles the Load event of the FuncCodePopupForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void FuncCodePopupForm_Load(object sender, EventArgs e)
        {
            OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            
            FuncCodeDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.PaleTurquoise;//奇數列顏色

            DataGridViewCheckBoxColumn cbCol = new DataGridViewCheckBoxColumn();
            cbCol.Name = "ck";
            cbCol.Width = 50;   //設定寬度
            //cbCol.HeaderText = "　   Select All";
            cbCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;   //置中
            FuncCodeDataGridView.Columns.Insert(0, cbCol);

            #region 建立全選 CheckBox

            //建立個矩形，等下計算 CheckBox 嵌入 GridView 的位置
            Rectangle rect = FuncCodeDataGridView.GetCellDisplayRectangle(0, -1, true);
            rect.X = rect.Location.X + rect.Width / 4 - 9;
            rect.Y = rect.Location.Y + (rect.Height / 2 - 9);

            CheckBox cbHeader = new CheckBox();
            cbHeader.Name = "checkboxHeader";
            cbHeader.Size = new Size(18, 18);
            cbHeader.Location = rect.Location;
            //全選要設定的事件
            cbHeader.CheckedChanged += new EventHandler(cbHeader_CheckedChanged);

            //將 CheckBox 加入到 dataGridView
            FuncCodeDataGridView.Controls.Add(cbHeader);

            #endregion
            loadResiteredFunc();
        }

        /// <summary>
        /// Loads the resitered function.
        /// </summary>
        private void loadResiteredFunc()
        {
            //A0.01 if (SCUtility.isEmpty(user_id)) 
            if (SCUtility.isEmpty(user_grp))  //A0.01
            {
                return;
            }
            //A0.01 List<UserFunc> regedUserFuncList = bcApp.SCApplication.UserBLL.loadUserFuncByUser(user_id);
            List<UASUFNC> regedUserFuncList = bcApp.SCApplication.UserBLL.loadUserFuncByUser(user_grp);  //A0.01
            foreach (UASUFNC usrFunc in regedUserFuncList)
            {
                int index = 0;
                foreach (var tmp in funcCodeDataList)
                {
                    if (SCUtility.isMatche(tmp.FUNC_CODE, usrFunc.FUNC_CODE))
                    {
                        FuncCodeDataGridView.Rows[index].Cells["ck"].Value = true;
                        break;
                    }
                    ++index;
                }
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the cbHeader control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cbHeader_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in FuncCodeDataGridView.Rows) 
            {
                dr.Cells["ck"].Value = ((CheckBox)FuncCodeDataGridView.Controls.Find("checkboxHeader", true)[0]).Checked;
            }
            FuncCodeDataGridView.EndEdit();
        }

        /// <summary>
        /// Gets the select function code list.
        /// </summary>
        /// <returns>List&lt;FunctionCode&gt;.</returns>
        public List<UASFNC> getSelectFunctionCodeList() 
        {
            List<UASFNC> selectFuncCodeList = new List<UASFNC>();
            foreach (DataGridViewRow dr in FuncCodeDataGridView.Rows)
            {
                if (dr.Cells["ck"].Value != null && (bool)dr.Cells["ck"].Value)
                {
                    
                    int selectIndex = dr.Index;
                    if (selectIndex < 0 || selectIndex >= funcCodeDataList.Count) 
                    {
                        continue;
                    }
                    UASFNC selFunc = funcCodeDataList[selectIndex];
                    selectFuncCodeList.Add(selFunc);
                }
            }
            return selectFuncCodeList;
        }

        /// <summary>
        /// Handles the Click event of the OKBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OKBtn_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Handles the Click event of the CancelBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void CancelBtn_Click(object sender, EventArgs e)
        {

        }

    }
}
