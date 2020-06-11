namespace com.mirle.ibg3k0.bc.winform.UI
{
    partial class ChangePwdForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangePwdForm));
            this.m_tableLayoutPnl = new System.Windows.Forms.TableLayoutPanel();
            this.m_newPwdVerTxb = new System.Windows.Forms.TextBox();
            this.m_idLbl = new System.Windows.Forms.Label();
            this.m_pwdLbl = new System.Windows.Forms.Label();
            this.m_pwdVerifyLbl = new System.Windows.Forms.Label();
            this.m_yourIDTxb = new System.Windows.Forms.TextBox();
            this.m_newPwdTxb = new System.Windows.Forms.TextBox();
            this.m_oldpwdLbl = new System.Windows.Forms.Label();
            this.m_oldPwdTxb = new System.Windows.Forms.TextBox();
            this.pnlData.SuspendLayout();
            this.m_tableLayoutPnl.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlData
            // 
            this.pnlData.Controls.Add(this.m_tableLayoutPnl);
            resources.ApplyResources(this.pnlData, "pnlData");
            // 
            // lblProdCaption
            // 
            resources.ApplyResources(this.lblProdCaption, "lblProdCaption");
            // 
            // butStart
            // 
            resources.ApplyResources(this.butStart, "butStart");
            // 
            // butSave
            // 
            resources.ApplyResources(this.butSave, "butSave");
            // 
            // butStop
            // 
            resources.ApplyResources(this.butStop, "butStop");
            // 
            // butPause
            // 
            resources.ApplyResources(this.butPause, "butPause");
            // 
            // butRefresh
            // 
            resources.ApplyResources(this.butRefresh, "butRefresh");
            // 
            // butCreate
            // 
            resources.ApplyResources(this.butCreate, "butCreate");
            // 
            // m_tableLayoutPnl
            // 
            resources.ApplyResources(this.m_tableLayoutPnl, "m_tableLayoutPnl");
            this.m_tableLayoutPnl.Controls.Add(this.m_newPwdVerTxb, 1, 3);
            this.m_tableLayoutPnl.Controls.Add(this.m_idLbl, 0, 0);
            this.m_tableLayoutPnl.Controls.Add(this.m_pwdLbl, 0, 2);
            this.m_tableLayoutPnl.Controls.Add(this.m_pwdVerifyLbl, 0, 3);
            this.m_tableLayoutPnl.Controls.Add(this.m_yourIDTxb, 1, 0);
            this.m_tableLayoutPnl.Controls.Add(this.m_newPwdTxb, 1, 2);
            this.m_tableLayoutPnl.Controls.Add(this.m_oldpwdLbl, 0, 1);
            this.m_tableLayoutPnl.Controls.Add(this.m_oldPwdTxb, 1, 1);
            this.m_tableLayoutPnl.Name = "m_tableLayoutPnl";
            // 
            // m_newPwdVerTxb
            // 
            resources.ApplyResources(this.m_newPwdVerTxb, "m_newPwdVerTxb");
            this.m_newPwdVerTxb.Name = "m_newPwdVerTxb";
            // 
            // m_idLbl
            // 
            resources.ApplyResources(this.m_idLbl, "m_idLbl");
            this.m_idLbl.Name = "m_idLbl";
            // 
            // m_pwdLbl
            // 
            resources.ApplyResources(this.m_pwdLbl, "m_pwdLbl");
            this.m_pwdLbl.Name = "m_pwdLbl";
            // 
            // m_pwdVerifyLbl
            // 
            resources.ApplyResources(this.m_pwdVerifyLbl, "m_pwdVerifyLbl");
            this.m_pwdVerifyLbl.Name = "m_pwdVerifyLbl";
            // 
            // m_yourIDTxb
            // 
            resources.ApplyResources(this.m_yourIDTxb, "m_yourIDTxb");
            this.m_yourIDTxb.Name = "m_yourIDTxb";
            this.m_yourIDTxb.ReadOnly = true;
            // 
            // m_newPwdTxb
            // 
            resources.ApplyResources(this.m_newPwdTxb, "m_newPwdTxb");
            this.m_newPwdTxb.Name = "m_newPwdTxb";
            // 
            // m_oldpwdLbl
            // 
            resources.ApplyResources(this.m_oldpwdLbl, "m_oldpwdLbl");
            this.m_oldpwdLbl.Name = "m_oldpwdLbl";
            // 
            // m_oldPwdTxb
            // 
            resources.ApplyResources(this.m_oldPwdTxb, "m_oldPwdTxb");
            this.m_oldPwdTxb.Name = "m_oldPwdTxb";
            // 
            // ChangePwdForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "ChangePwdForm";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ChangePwdForm_FormClosed);
            this.Load += new System.EventHandler(this.ChangePwdForm_Load);
            this.pnlData.ResumeLayout(false);
            this.m_tableLayoutPnl.ResumeLayout(false);
            this.m_tableLayoutPnl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel m_tableLayoutPnl;
        private System.Windows.Forms.Label m_idLbl;
        private System.Windows.Forms.Label m_pwdLbl;
        private System.Windows.Forms.Label m_pwdVerifyLbl;
        private System.Windows.Forms.TextBox m_yourIDTxb;
        private System.Windows.Forms.TextBox m_newPwdTxb;
        private System.Windows.Forms.Label m_oldpwdLbl;
        private System.Windows.Forms.TextBox m_oldPwdTxb;
        private System.Windows.Forms.TextBox m_newPwdVerTxb;
    }
}