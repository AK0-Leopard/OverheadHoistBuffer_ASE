namespace com.mirle.ibg3k0.bc.winform.UI
{
    partial class AlarmEnablePopupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlarmEnablePopupForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.FuncCodeLbl = new System.Windows.Forms.Label();
            this.txt_EqID = new System.Windows.Forms.TextBox();
            this.lbl_describe = new System.Windows.Forms.Label();
            this.UserIDLbl = new System.Windows.Forms.Label();
            this.txt_userID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_AlarmCode = new System.Windows.Forms.TextBox();
            this.txt_reason = new System.Windows.Forms.TextBox();
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
            this.tableLayoutPanel2.Controls.Add(this.FuncCodeLbl, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.txt_EqID, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.lbl_describe, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.UserIDLbl, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.txt_userID, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.txt_AlarmCode, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.txt_reason, 1, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // FuncCodeLbl
            // 
            resources.ApplyResources(this.FuncCodeLbl, "FuncCodeLbl");
            this.FuncCodeLbl.Name = "FuncCodeLbl";
            // 
            // txt_EqID
            // 
            resources.ApplyResources(this.txt_EqID, "txt_EqID");
            this.txt_EqID.Name = "txt_EqID";
            this.txt_EqID.ReadOnly = true;
            // 
            // lbl_describe
            // 
            resources.ApplyResources(this.lbl_describe, "lbl_describe");
            this.lbl_describe.Name = "lbl_describe";
            // 
            // UserIDLbl
            // 
            resources.ApplyResources(this.UserIDLbl, "UserIDLbl");
            this.UserIDLbl.Name = "UserIDLbl";
            // 
            // txt_userID
            // 
            resources.ApplyResources(this.txt_userID, "txt_userID");
            this.txt_userID.Name = "txt_userID";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txt_AlarmCode
            // 
            resources.ApplyResources(this.txt_AlarmCode, "txt_AlarmCode");
            this.txt_AlarmCode.Name = "txt_AlarmCode";
            this.txt_AlarmCode.ReadOnly = true;
            // 
            // txt_reason
            // 
            resources.ApplyResources(this.txt_reason, "txt_reason");
            this.txt_reason.Name = "txt_reason";
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
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // AlarmEnablePopupForm
            // 
            this.AcceptButton = this.OKBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "AlarmEnablePopupForm";
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
        private System.Windows.Forms.Label lbl_describe;
        private System.Windows.Forms.Label FuncCodeLbl;
        private System.Windows.Forms.TextBox txt_EqID;
        private System.Windows.Forms.TextBox txt_userID;
        private System.Windows.Forms.Panel LoginPnl;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_AlarmCode;
        private System.Windows.Forms.TextBox txt_reason;
    }
}