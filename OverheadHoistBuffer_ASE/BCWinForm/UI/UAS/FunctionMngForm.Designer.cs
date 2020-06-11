namespace com.mirle.ibg3k0.bc.winform.UI.UAS
{
    partial class FunctionMngForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FunctionMngForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.FuncCodeListLbl = new System.Windows.Forms.Label();
            this.FuncCodeMngPnl = new System.Windows.Forms.Panel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.FuncCodeLbl = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.FuncCodeTBx = new System.Windows.Forms.TextBox();
            this.FuncNameTBx = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.close_btn = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.AddBtn = new System.Windows.Forms.Button();
            this.DeleteBtn = new System.Windows.Forms.Button();
            this.UpdBtn = new System.Windows.Forms.Button();
            this.FuncCodeDataGridView = new System.Windows.Forms.DataGridView();
            this.Func_Code_Clmn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Func_Name_Clmn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.lbl_userGroupManagement = new System.Windows.Forms.Label();
            this.tableLayoutPanel2.SuspendLayout();
            this.FuncCodeMngPnl.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FuncCodeDataGridView)).BeginInit();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.FuncCodeListLbl, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.FuncCodeMngPnl, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.FuncCodeDataGridView, 0, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // FuncCodeListLbl
            // 
            resources.ApplyResources(this.FuncCodeListLbl, "FuncCodeListLbl");
            this.FuncCodeListLbl.Name = "FuncCodeListLbl";
            // 
            // FuncCodeMngPnl
            // 
            this.FuncCodeMngPnl.Controls.Add(this.tableLayoutPanel5);
            resources.ApplyResources(this.FuncCodeMngPnl, "FuncCodeMngPnl");
            this.FuncCodeMngPnl.Name = "FuncCodeMngPnl";
            // 
            // tableLayoutPanel5
            // 
            resources.ApplyResources(this.tableLayoutPanel5, "tableLayoutPanel5");
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.FuncCodeLbl, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.FuncCodeTBx, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.FuncNameTBx, 1, 1);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // FuncCodeLbl
            // 
            resources.ApplyResources(this.FuncCodeLbl, "FuncCodeLbl");
            this.FuncCodeLbl.Name = "FuncCodeLbl";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // FuncCodeTBx
            // 
            resources.ApplyResources(this.FuncCodeTBx, "FuncCodeTBx");
            this.FuncCodeTBx.Name = "FuncCodeTBx";
            // 
            // FuncNameTBx
            // 
            resources.ApplyResources(this.FuncNameTBx, "FuncNameTBx");
            this.FuncNameTBx.Name = "FuncNameTBx";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.close_btn);
            this.panel2.Controls.Add(this.AddBtn);
            this.panel2.Controls.Add(this.DeleteBtn);
            this.panel2.Controls.Add(this.UpdBtn);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // close_btn
            // 
            resources.ApplyResources(this.close_btn, "close_btn");
            this.close_btn.ImageList = this.imageList1;
            this.close_btn.Name = "close_btn";
            this.close_btn.UseVisualStyleBackColor = true;
            this.close_btn.Click += new System.EventHandler(this.close_btn_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "clear.ico");
            this.imageList1.Images.SetKeyName(1, "MiningModel.ico");
            this.imageList1.Images.SetKeyName(2, "Save.ICO");
            this.imageList1.Images.SetKeyName(3, "close-24.png");
            this.imageList1.Images.SetKeyName(4, "1448701926_Exit.ico");
            // 
            // AddBtn
            // 
            resources.ApplyResources(this.AddBtn, "AddBtn");
            this.AddBtn.ImageList = this.imageList1;
            this.AddBtn.Name = "AddBtn";
            this.AddBtn.UseVisualStyleBackColor = true;
            this.AddBtn.Click += new System.EventHandler(this.AddBtn_Click);
            // 
            // DeleteBtn
            // 
            resources.ApplyResources(this.DeleteBtn, "DeleteBtn");
            this.DeleteBtn.ImageList = this.imageList1;
            this.DeleteBtn.Name = "DeleteBtn";
            this.DeleteBtn.UseVisualStyleBackColor = true;
            this.DeleteBtn.Click += new System.EventHandler(this.DeleteBtn_Click);
            // 
            // UpdBtn
            // 
            resources.ApplyResources(this.UpdBtn, "UpdBtn");
            this.UpdBtn.ImageList = this.imageList1;
            this.UpdBtn.Name = "UpdBtn";
            this.UpdBtn.UseVisualStyleBackColor = true;
            this.UpdBtn.Click += new System.EventHandler(this.UpdBtn_Click);
            // 
            // FuncCodeDataGridView
            // 
            this.FuncCodeDataGridView.AllowUserToAddRows = false;
            this.FuncCodeDataGridView.AllowUserToDeleteRows = false;
            resources.ApplyResources(this.FuncCodeDataGridView, "FuncCodeDataGridView");
            this.FuncCodeDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.FuncCodeDataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.FuncCodeDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.FuncCodeDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Func_Code_Clmn,
            this.Func_Name_Clmn});
            this.FuncCodeDataGridView.MultiSelect = false;
            this.FuncCodeDataGridView.Name = "FuncCodeDataGridView";
            this.FuncCodeDataGridView.ReadOnly = true;
            this.FuncCodeDataGridView.RowTemplate.Height = 24;
            this.FuncCodeDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.FuncCodeDataGridView.Click += new System.EventHandler(this.FuncCodeDataGridView_Click);
            // 
            // Func_Code_Clmn
            // 
            this.Func_Code_Clmn.DataPropertyName = "Func_Code";
            resources.ApplyResources(this.Func_Code_Clmn, "Func_Code_Clmn");
            this.Func_Code_Clmn.Name = "Func_Code_Clmn";
            this.Func_Code_Clmn.ReadOnly = true;
            // 
            // Func_Name_Clmn
            // 
            this.Func_Name_Clmn.DataPropertyName = "Func_Name";
            resources.ApplyResources(this.Func_Name_Clmn, "Func_Name_Clmn");
            this.Func_Name_Clmn.Name = "Func_Name_Clmn";
            this.Func_Name_Clmn.ReadOnly = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel4);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.lbl_userGroupManagement, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // lbl_userGroupManagement
            // 
            this.lbl_userGroupManagement.BackColor = System.Drawing.Color.Orange;
            this.lbl_userGroupManagement.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.lbl_userGroupManagement, "lbl_userGroupManagement");
            this.lbl_userGroupManagement.ForeColor = System.Drawing.Color.MediumBlue;
            this.lbl_userGroupManagement.Name = "lbl_userGroupManagement";
            // 
            // FunctionMngForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.Name = "FunctionMngForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.FuncCodeMngPnl.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FuncCodeDataGridView)).EndInit();
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label FuncCodeListLbl;
        private System.Windows.Forms.DataGridView FuncCodeDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Func_Code_Clmn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Func_Name_Clmn;
        private System.Windows.Forms.Panel FuncCodeMngPnl;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label FuncCodeLbl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox FuncCodeTBx;
        private System.Windows.Forms.TextBox FuncNameTBx;
        private System.Windows.Forms.Button UpdBtn;
        private System.Windows.Forms.Button AddBtn;
        private System.Windows.Forms.Button DeleteBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        public System.Windows.Forms.Label lbl_userGroupManagement;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button close_btn;
    }
}