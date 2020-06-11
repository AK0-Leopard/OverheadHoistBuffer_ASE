namespace com.mirle.ibg3k0.bc.winform.UI.UAS
{
    partial class UserGrpMngForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserGrpMngForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lbl_userGroupManagement = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.UserGroupGridView = new System.Windows.Forms.DataGridView();
            this.User_Grp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UserListLbl = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.UserIDLbl = new System.Windows.Forms.Label();
            this.UserGroupTbx = new System.Windows.Forms.TextBox();
            this.ActionButtonPnl = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.AddBtn = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.DelBtn = new System.Windows.Forms.Button();
            this.RgsFuncBtn = new System.Windows.Forms.Button();
            this.btn_close = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UserGroupGridView)).BeginInit();
            this.tableLayoutPanel4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.lbl_userGroupManagement, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // lbl_userGroupManagement
            // 
            this.lbl_userGroupManagement.BackColor = System.Drawing.Color.Orange;
            this.lbl_userGroupManagement.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.lbl_userGroupManagement, "lbl_userGroupManagement");
            this.lbl_userGroupManagement.ForeColor = System.Drawing.Color.MediumBlue;
            this.lbl_userGroupManagement.Name = "lbl_userGroupManagement";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.UserGroupGridView, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.UserListLbl, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 0, 2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // UserGroupGridView
            // 
            this.UserGroupGridView.AllowUserToAddRows = false;
            this.UserGroupGridView.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.PaleTurquoise;
            this.UserGroupGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.UserGroupGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.UserGroupGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.UserGroupGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.UserGroupGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.User_Grp});
            resources.ApplyResources(this.UserGroupGridView, "UserGroupGridView");
            this.UserGroupGridView.MultiSelect = false;
            this.UserGroupGridView.Name = "UserGroupGridView";
            this.UserGroupGridView.ReadOnly = true;
            this.UserGroupGridView.RowTemplate.Height = 24;
            this.UserGroupGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.UserGroupGridView.Click += new System.EventHandler(this.UserGroupGridView_Click);
            this.UserGroupGridView.DoubleClick += new System.EventHandler(this.UserGroupGridView_DoubleClick);
            // 
            // User_Grp
            // 
            this.User_Grp.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.User_Grp.DataPropertyName = "User_Grp";
            resources.ApplyResources(this.User_Grp, "User_Grp");
            this.User_Grp.Name = "User_Grp";
            this.User_Grp.ReadOnly = true;
            // 
            // UserListLbl
            // 
            resources.ApplyResources(this.UserListLbl, "UserListLbl");
            this.UserListLbl.Name = "UserListLbl";
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.ActionButtonPnl, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.panel3, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel5, 1, 1);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel3);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.UserIDLbl, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.UserGroupTbx, 1, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // UserIDLbl
            // 
            resources.ApplyResources(this.UserIDLbl, "UserIDLbl");
            this.UserIDLbl.Name = "UserIDLbl";
            // 
            // UserGroupTbx
            // 
            resources.ApplyResources(this.UserGroupTbx, "UserGroupTbx");
            this.UserGroupTbx.Name = "UserGroupTbx";
            // 
            // ActionButtonPnl
            // 
            resources.ApplyResources(this.ActionButtonPnl, "ActionButtonPnl");
            this.ActionButtonPnl.Name = "ActionButtonPnl";
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // tableLayoutPanel5
            // 
            resources.ApplyResources(this.tableLayoutPanel5, "tableLayoutPanel5");
            this.tableLayoutPanel5.Controls.Add(this.AddBtn, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.DelBtn, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.RgsFuncBtn, 2, 0);
            this.tableLayoutPanel5.Controls.Add(this.btn_close, 3, 0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            // 
            // AddBtn
            // 
            resources.ApplyResources(this.AddBtn, "AddBtn");
            this.AddBtn.ImageList = this.imageList1;
            this.AddBtn.Name = "AddBtn";
            this.AddBtn.UseVisualStyleBackColor = true;
            this.AddBtn.Click += new System.EventHandler(this.AddBtn_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Advanced Options.ico");
            this.imageList1.Images.SetKeyName(1, "clear.ico");
            this.imageList1.Images.SetKeyName(2, "MiningModel.ico");
            this.imageList1.Images.SetKeyName(3, "close-24.png");
            this.imageList1.Images.SetKeyName(4, "1448701926_Exit.ico");
            // 
            // DelBtn
            // 
            resources.ApplyResources(this.DelBtn, "DelBtn");
            this.DelBtn.ImageList = this.imageList1;
            this.DelBtn.Name = "DelBtn";
            this.DelBtn.UseVisualStyleBackColor = true;
            this.DelBtn.Click += new System.EventHandler(this.DelBtn_Click);
            // 
            // RgsFuncBtn
            // 
            resources.ApplyResources(this.RgsFuncBtn, "RgsFuncBtn");
            this.RgsFuncBtn.ImageList = this.imageList1;
            this.RgsFuncBtn.Name = "RgsFuncBtn";
            this.RgsFuncBtn.UseVisualStyleBackColor = true;
            this.RgsFuncBtn.Click += new System.EventHandler(this.RgsFuncBtn_Click);
            // 
            // btn_close
            // 
            resources.ApplyResources(this.btn_close, "btn_close");
            this.btn_close.ImageList = this.imageList1;
            this.btn_close.Name = "btn_close";
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // UserGrpMngForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserGrpMngForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UserGroupGridView)).EndInit();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.DataGridView UserGroupGridView;
        private System.Windows.Forms.Label UserListLbl;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label UserIDLbl;
        private System.Windows.Forms.TextBox UserGroupTbx;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Panel ActionButtonPnl;
        private System.Windows.Forms.Button DelBtn;
        private System.Windows.Forms.Button AddBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button RgsFuncBtn;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        public System.Windows.Forms.Label lbl_userGroupManagement;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.DataGridViewTextBoxColumn User_Grp;
        private System.Windows.Forms.Button btn_close;
    }
}