namespace com.mirle.ibg3k0.bc.winform.UI
{
    partial class BCBasicForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BCBasicForm));
            this.formTitleLb = new CCWin.SkinControl.SkinLabel();
            this.skinPanel1 = new CCWin.SkinControl.SkinPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnlSearch = new CCWin.SkinControl.SkinButton();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnClose = new CCWin.SkinControl.SkinButton();
            this.skinPanel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // formTitleLb
            // 
            this.formTitleLb.BackColor = System.Drawing.Color.Transparent;
            this.formTitleLb.BorderColor = System.Drawing.Color.Transparent;
            this.formTitleLb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.formTitleLb, "formTitleLb");
            this.formTitleLb.ForeColor = System.Drawing.Color.Black;
            this.formTitleLb.Name = "formTitleLb";
            // 
            // skinPanel1
            // 
            resources.ApplyResources(this.skinPanel1, "skinPanel1");
            this.skinPanel1.BackColor = System.Drawing.Color.Transparent;
            this.skinPanel1.BorderColor = System.Drawing.Color.Black;
            this.skinPanel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.skinPanel1.Controls.Add(this.tableLayoutPanel1);
            this.skinPanel1.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinPanel1.DownBack = null;
            this.skinPanel1.MouseBack = null;
            this.skinPanel1.Name = "skinPanel1";
            this.skinPanel1.NormlBack = null;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.btnlSearch, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // btnlSearch
            // 
            resources.ApplyResources(this.btnlSearch, "btnlSearch");
            this.btnlSearch.BackColor = System.Drawing.Color.Transparent;
            this.btnlSearch.BaseColor = System.Drawing.Color.LightGray;
            this.btnlSearch.BorderColor = System.Drawing.Color.Black;
            this.btnlSearch.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btnlSearch.DownBack = null;
            this.btnlSearch.ForeColor = System.Drawing.Color.Black;
            this.btnlSearch.Image = global::com.mirle.ibg3k0.bc.winform.Properties.Resources.se;
            this.btnlSearch.MouseBack = null;
            this.btnlSearch.Name = "btnlSearch";
            this.btnlSearch.NormlBack = null;
            this.btnlSearch.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btnlSearch.UseVisualStyleBackColor = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.btnClose, 5, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.BaseColor = System.Drawing.Color.LightGray;
            this.btnClose.BorderColor = System.Drawing.Color.Black;
            this.btnClose.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btnClose.DownBack = null;
            this.btnClose.DownBaseColor = System.Drawing.Color.RoyalBlue;
            this.btnClose.MouseBack = null;
            this.btnClose.Name = "btnClose";
            this.btnClose.NormlBack = null;
            this.btnClose.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // BCBasicForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::com.mirle.ibg3k0.bc.winform.Properties.Resources.main;
            this.Controls.Add(this.skinPanel1);
            this.Controls.Add(this.formTitleLb);
            this.Name = "BCBasicForm";
            this.ShowIcon = false;
            this.skinPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private CCWin.SkinControl.SkinLabel formTitleLb;
        private CCWin.SkinControl.SkinPanel skinPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private CCWin.SkinControl.SkinButton btnClose;
        private CCWin.SkinControl.SkinButton btnlSearch;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}