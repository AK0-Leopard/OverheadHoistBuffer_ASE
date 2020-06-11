namespace com.mirle.ibg3k0.bc.winform.UI.UAS
{
    partial class LoginPopupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginPopupForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.UserIDLbl = new System.Windows.Forms.Label();
            this.PwdLbl = new System.Windows.Forms.Label();
            this.FuncCodeLbl = new System.Windows.Forms.Label();
            this.FuncCodeTBx = new System.Windows.Forms.TextBox();
            this.UserIDTBx = new System.Windows.Forms.TextBox();
            this.PwdTBx = new System.Windows.Forms.TextBox();
            this.LoginPnl = new System.Windows.Forms.Panel();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.OKBtn = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.LoginPnl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LoginPnl, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.UserIDLbl, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.PwdLbl, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.FuncCodeLbl, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.FuncCodeTBx, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.UserIDTBx, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.PwdTBx, 1, 2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // UserIDLbl
            // 
            resources.ApplyResources(this.UserIDLbl, "UserIDLbl");
            this.UserIDLbl.Name = "UserIDLbl";
            // 
            // PwdLbl
            // 
            resources.ApplyResources(this.PwdLbl, "PwdLbl");
            this.PwdLbl.Name = "PwdLbl";
            // 
            // FuncCodeLbl
            // 
            resources.ApplyResources(this.FuncCodeLbl, "FuncCodeLbl");
            this.FuncCodeLbl.Name = "FuncCodeLbl";
            // 
            // FuncCodeTBx
            // 
            resources.ApplyResources(this.FuncCodeTBx, "FuncCodeTBx");
            this.FuncCodeTBx.Name = "FuncCodeTBx";
            this.FuncCodeTBx.ReadOnly = true;
            // 
            // UserIDTBx
            // 
            resources.ApplyResources(this.UserIDTBx, "UserIDTBx");
            this.UserIDTBx.Name = "UserIDTBx";
            this.UserIDTBx.Click += new System.EventHandler(this.UserIDTBx_Click);
            // 
            // PwdTBx
            // 
            resources.ApplyResources(this.PwdTBx, "PwdTBx");
            this.PwdTBx.Name = "PwdTBx";
            this.PwdTBx.UseSystemPasswordChar = true;
            // 
            // LoginPnl
            // 
            this.LoginPnl.Controls.Add(this.CancelBtn);
            this.LoginPnl.Controls.Add(this.OKBtn);
            resources.ApplyResources(this.LoginPnl, "LoginPnl");
            this.LoginPnl.Name = "LoginPnl";
            // 
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.CancelBtn, "CancelBtn");
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // OKBtn
            // 
            resources.ApplyResources(this.OKBtn, "OKBtn");
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.UseVisualStyleBackColor = true;
            // 
            // LoginPopupForm
            // 
            this.AcceptButton = this.OKBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LoginPopupForm";
            this.Load += new System.EventHandler(this.LoginPopupForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.LoginPnl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label UserIDLbl;
        private System.Windows.Forms.Label PwdLbl;
        private System.Windows.Forms.Label FuncCodeLbl;
        private System.Windows.Forms.TextBox FuncCodeTBx;
        private System.Windows.Forms.TextBox UserIDTBx;
        private System.Windows.Forms.TextBox PwdTBx;
        private System.Windows.Forms.Panel LoginPnl;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button OKBtn;
    }
}