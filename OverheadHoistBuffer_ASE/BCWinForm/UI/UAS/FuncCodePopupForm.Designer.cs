namespace com.mirle.ibg3k0.bc.winform.UI.UAS
{
    partial class FuncCodePopupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FuncCodePopupForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.FuncCodeDataGridView = new System.Windows.Forms.DataGridView();
            this.RegisterPnl = new System.Windows.Forms.Panel();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.OKBtn = new System.Windows.Forms.Button();
            this.Func_Code_Clmn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Func_Name_Clmn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FuncCodeDataGridView)).BeginInit();
            this.RegisterPnl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.FuncCodeDataGridView, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.RegisterPnl, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // FuncCodeDataGridView
            // 
            this.FuncCodeDataGridView.AllowUserToAddRows = false;
            this.FuncCodeDataGridView.AllowUserToDeleteRows = false;
            this.FuncCodeDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.FuncCodeDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.FuncCodeDataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.FuncCodeDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.FuncCodeDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Func_Code_Clmn,
            this.Func_Name_Clmn});
            resources.ApplyResources(this.FuncCodeDataGridView, "FuncCodeDataGridView");
            this.FuncCodeDataGridView.Name = "FuncCodeDataGridView";
            this.FuncCodeDataGridView.RowTemplate.Height = 24;
            this.FuncCodeDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.FuncCodeDataGridView.ShowEditingIcon = false;
            // 
            // RegisterPnl
            // 
            this.RegisterPnl.Controls.Add(this.CancelBtn);
            this.RegisterPnl.Controls.Add(this.OKBtn);
            resources.ApplyResources(this.RegisterPnl, "RegisterPnl");
            this.RegisterPnl.Name = "RegisterPnl";
            // 
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.CancelBtn, "CancelBtn");
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // OKBtn
            // 
            resources.ApplyResources(this.OKBtn, "OKBtn");
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // Func_Code_Clmn
            // 
            this.Func_Code_Clmn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Func_Code_Clmn.DataPropertyName = "Func_Code";
            resources.ApplyResources(this.Func_Code_Clmn, "Func_Code_Clmn");
            this.Func_Code_Clmn.MaxInputLength = 60;
            this.Func_Code_Clmn.Name = "Func_Code_Clmn";
            // 
            // Func_Name_Clmn
            // 
            this.Func_Name_Clmn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Func_Name_Clmn.DataPropertyName = "Func_Name";
            resources.ApplyResources(this.Func_Name_Clmn, "Func_Name_Clmn");
            this.Func_Name_Clmn.MaxInputLength = 80;
            this.Func_Name_Clmn.Name = "Func_Name_Clmn";
            // 
            // FuncCodePopupForm
            // 
            this.AcceptButton = this.OKBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FuncCodePopupForm";
            this.Load += new System.EventHandler(this.FuncCodePopupForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FuncCodeDataGridView)).EndInit();
            this.RegisterPnl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView FuncCodeDataGridView;
        private System.Windows.Forms.Panel RegisterPnl;
        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Func_Code_Clmn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Func_Name_Clmn;
    }
}