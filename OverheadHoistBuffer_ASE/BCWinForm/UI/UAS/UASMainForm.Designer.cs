namespace com.mirle.ibg3k0.bc.winform.UI.UAS
{
    partial class UASMainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UASMainForm));
            this.UASMenuBar = new System.Windows.Forms.MenuStrip();
            this.UserMngMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userGroupManagementToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.FunctionMngMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UASFormStatusStrip = new System.Windows.Forms.StatusStrip();
            this.blankLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.SPLbl_1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.LoginUserLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.UASShowLoginUserIDLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.UASMenuBar.SuspendLayout();
            this.UASFormStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // UASMenuBar
            // 
            resources.ApplyResources(this.UASMenuBar, "UASMenuBar");
            this.UASMenuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UserMngMenuItem,
            this.userGroupManagementToolStripMenuItem1,
            this.FunctionMngMenuItem});
            this.UASMenuBar.Name = "UASMenuBar";
            this.UASMenuBar.ItemAdded += new System.Windows.Forms.ToolStripItemEventHandler(this.UASMenuBar_ItemAdded);
            // 
            // UserMngMenuItem
            // 
            resources.ApplyResources(this.UserMngMenuItem, "UserMngMenuItem");
            this.UserMngMenuItem.Name = "UserMngMenuItem";
            this.UserMngMenuItem.Click += new System.EventHandler(this.UserMngMenuItem_Click);
            // 
            // userGroupManagementToolStripMenuItem1
            // 
            resources.ApplyResources(this.userGroupManagementToolStripMenuItem1, "userGroupManagementToolStripMenuItem1");
            this.userGroupManagementToolStripMenuItem1.Name = "userGroupManagementToolStripMenuItem1";
            this.userGroupManagementToolStripMenuItem1.Click += new System.EventHandler(this.userGroupManagementToolStripMenuItem1_Click);
            // 
            // FunctionMngMenuItem
            // 
            resources.ApplyResources(this.FunctionMngMenuItem, "FunctionMngMenuItem");
            this.FunctionMngMenuItem.Name = "FunctionMngMenuItem";
            this.FunctionMngMenuItem.Click += new System.EventHandler(this.FunctionMngMenuItem_Click);
            // 
            // UASFormStatusStrip
            // 
            this.UASFormStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.blankLbl,
            this.SPLbl_1,
            this.LoginUserLbl,
            this.UASShowLoginUserIDLbl});
            resources.ApplyResources(this.UASFormStatusStrip, "UASFormStatusStrip");
            this.UASFormStatusStrip.Name = "UASFormStatusStrip";
            // 
            // blankLbl
            // 
            this.blankLbl.Name = "blankLbl";
            resources.ApplyResources(this.blankLbl, "blankLbl");
            this.blankLbl.Spring = true;
            // 
            // SPLbl_1
            // 
            this.SPLbl_1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.SPLbl_1.Name = "SPLbl_1";
            resources.ApplyResources(this.SPLbl_1, "SPLbl_1");
            // 
            // LoginUserLbl
            // 
            resources.ApplyResources(this.LoginUserLbl, "LoginUserLbl");
            this.LoginUserLbl.Name = "LoginUserLbl";
            // 
            // UASShowLoginUserIDLbl
            // 
            resources.ApplyResources(this.UASShowLoginUserIDLbl, "UASShowLoginUserIDLbl");
            this.UASShowLoginUserIDLbl.Name = "UASShowLoginUserIDLbl";
            // 
            // UASMainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.UASFormStatusStrip);
            this.Controls.Add(this.UASMenuBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.UASMenuBar;
            this.Name = "UASMainForm";
            this.Load += new System.EventHandler(this.UASMainForm_Load);
            this.UASMenuBar.ResumeLayout(false);
            this.UASMenuBar.PerformLayout();
            this.UASFormStatusStrip.ResumeLayout(false);
            this.UASFormStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip UASMenuBar;
        private System.Windows.Forms.ToolStripMenuItem UserMngMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FunctionMngMenuItem;
        private System.Windows.Forms.StatusStrip UASFormStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel blankLbl;
        private System.Windows.Forms.ToolStripStatusLabel SPLbl_1;
        private System.Windows.Forms.ToolStripStatusLabel LoginUserLbl;
        private System.Windows.Forms.ToolStripStatusLabel UASShowLoginUserIDLbl;
        private System.Windows.Forms.ToolStripMenuItem userGroupManagementToolStripMenuItem1;
    }
}