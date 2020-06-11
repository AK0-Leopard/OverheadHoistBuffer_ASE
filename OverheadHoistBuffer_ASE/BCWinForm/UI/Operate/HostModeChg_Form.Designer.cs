namespace com.mirle.ibg3k0.bc.winform.UI
{
    partial class HostModeChg_Form
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
            this.skinGroupBox3 = new CCWin.SkinControl.SkinGroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.butLcsPause = new System.Windows.Forms.Button();
            this.butLcsAuto = new System.Windows.Forms.Button();
            this.lblLCSStatusValue = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.skinGroupBox2 = new CCWin.SkinControl.SkinGroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.butOffline = new System.Windows.Forms.Button();
            this.txtHostMode = new System.Windows.Forms.TextBox();
            this.butOnlineRemote = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.skinGroupBox1 = new CCWin.SkinControl.SkinGroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.butEnable = new System.Windows.Forms.Button();
            this.butDisable = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCommuntion = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.skinGroupBox3.SuspendLayout();
            this.skinGroupBox2.SuspendLayout();
            this.skinGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // skinGroupBox3
            // 
            this.skinGroupBox3.BackColor = System.Drawing.Color.Transparent;
            this.skinGroupBox3.BorderColor = System.Drawing.Color.Black;
            this.skinGroupBox3.Controls.Add(this.label7);
            this.skinGroupBox3.Controls.Add(this.label9);
            this.skinGroupBox3.Controls.Add(this.butLcsPause);
            this.skinGroupBox3.Controls.Add(this.butLcsAuto);
            this.skinGroupBox3.Controls.Add(this.lblLCSStatusValue);
            this.skinGroupBox3.Controls.Add(this.label8);
            this.skinGroupBox3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.skinGroupBox3.Font = new System.Drawing.Font("Arial", 15.75F);
            this.skinGroupBox3.ForeColor = System.Drawing.Color.Black;
            this.skinGroupBox3.Location = new System.Drawing.Point(665, 12);
            this.skinGroupBox3.Name = "skinGroupBox3";
            this.skinGroupBox3.Radius = 20;
            this.skinGroupBox3.RectBackColor = System.Drawing.SystemColors.Control;
            this.skinGroupBox3.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.skinGroupBox3.Size = new System.Drawing.Size(264, 279);
            this.skinGroupBox3.TabIndex = 73;
            this.skinGroupBox3.TabStop = false;
            this.skinGroupBox3.Text = "LFC Status";
            this.skinGroupBox3.TitleBorderColor = System.Drawing.Color.Black;
            this.skinGroupBox3.TitleRadius = 10;
            this.skinGroupBox3.TitleRectBackColor = System.Drawing.Color.LightSkyBlue;
            this.skinGroupBox3.TitleRoundStyle = CCWin.SkinClass.RoundStyle.All;
            // 
            // label7
            // 
            this.label7.Enabled = false;
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(66, 105);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(156, 23);
            this.label7.TabIndex = 79;
            this.label7.Text = "Mode Change";
            // 
            // label9
            // 
            this.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label9.Location = new System.Drawing.Point(65, 36);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(156, 23);
            this.label9.TabIndex = 78;
            this.label9.Text = "Current Status";
            // 
            // butLcsPause
            // 
            this.butLcsPause.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.butLcsPause.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold);
            this.butLcsPause.ForeColor = System.Drawing.Color.Black;
            this.butLcsPause.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.butLcsPause.Location = new System.Drawing.Point(53, 178);
            this.butLcsPause.Name = "butLcsPause";
            this.butLcsPause.Size = new System.Drawing.Size(168, 35);
            this.butLcsPause.TabIndex = 22;
            this.butLcsPause.Text = "Pause";
            this.butLcsPause.UseVisualStyleBackColor = true;
            this.butLcsPause.Click += new System.EventHandler(this.butLcsPause_Click);
            // 
            // butLcsAuto
            // 
            this.butLcsAuto.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.butLcsAuto.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold);
            this.butLcsAuto.ForeColor = System.Drawing.Color.Black;
            this.butLcsAuto.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.butLcsAuto.Location = new System.Drawing.Point(53, 131);
            this.butLcsAuto.Name = "butLcsAuto";
            this.butLcsAuto.Size = new System.Drawing.Size(168, 35);
            this.butLcsAuto.TabIndex = 4;
            this.butLcsAuto.Text = "Auto";
            this.butLcsAuto.UseVisualStyleBackColor = true;
            this.butLcsAuto.Click += new System.EventHandler(this.butLcsAuto_Click);
            // 
            // lblLCSStatusValue
            // 
            this.lblLCSStatusValue.BackColor = System.Drawing.Color.Yellow;
            this.lblLCSStatusValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLCSStatusValue.Font = new System.Drawing.Font("Arial", 14.25F);
            this.lblLCSStatusValue.ForeColor = System.Drawing.Color.Black;
            this.lblLCSStatusValue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblLCSStatusValue.Location = new System.Drawing.Point(43, 60);
            this.lblLCSStatusValue.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.lblLCSStatusValue.Name = "lblLCSStatusValue";
            this.lblLCSStatusValue.Size = new System.Drawing.Size(187, 29);
            this.lblLCSStatusValue.TabIndex = 32;
            this.lblLCSStatusValue.Text = "Pause";
            this.lblLCSStatusValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.DimGray;
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(-6, 97);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(270, 1);
            this.label8.TabIndex = 69;
            this.label8.Text = "label8";
            // 
            // skinGroupBox2
            // 
            this.skinGroupBox2.BackColor = System.Drawing.Color.Transparent;
            this.skinGroupBox2.BorderColor = System.Drawing.Color.Black;
            this.skinGroupBox2.Controls.Add(this.label4);
            this.skinGroupBox2.Controls.Add(this.label6);
            this.skinGroupBox2.Controls.Add(this.butOffline);
            this.skinGroupBox2.Controls.Add(this.txtHostMode);
            this.skinGroupBox2.Controls.Add(this.butOnlineRemote);
            this.skinGroupBox2.Controls.Add(this.label5);
            this.skinGroupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.skinGroupBox2.Font = new System.Drawing.Font("Arial", 15.75F);
            this.skinGroupBox2.ForeColor = System.Drawing.Color.Black;
            this.skinGroupBox2.Location = new System.Drawing.Point(337, 12);
            this.skinGroupBox2.Name = "skinGroupBox2";
            this.skinGroupBox2.Radius = 20;
            this.skinGroupBox2.RectBackColor = System.Drawing.SystemColors.Control;
            this.skinGroupBox2.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.skinGroupBox2.Size = new System.Drawing.Size(264, 279);
            this.skinGroupBox2.TabIndex = 72;
            this.skinGroupBox2.TabStop = false;
            this.skinGroupBox2.Text = "Host Mode";
            this.skinGroupBox2.TitleBorderColor = System.Drawing.Color.Black;
            this.skinGroupBox2.TitleRadius = 10;
            this.skinGroupBox2.TitleRectBackColor = System.Drawing.Color.LightSkyBlue;
            this.skinGroupBox2.TitleRoundStyle = CCWin.SkinClass.RoundStyle.All;
            // 
            // label4
            // 
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(63, 105);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(156, 23);
            this.label4.TabIndex = 79;
            this.label4.Text = "Mode Change";
            // 
            // label6
            // 
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(61, 35);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(156, 23);
            this.label6.TabIndex = 78;
            this.label6.Text = "Current Status";
            // 
            // butOffline
            // 
            this.butOffline.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.butOffline.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold);
            this.butOffline.ForeColor = System.Drawing.Color.Black;
            this.butOffline.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.butOffline.Location = new System.Drawing.Point(49, 225);
            this.butOffline.Name = "butOffline";
            this.butOffline.Size = new System.Drawing.Size(168, 35);
            this.butOffline.TabIndex = 15;
            this.butOffline.Text = "Offline Mode";
            this.butOffline.UseVisualStyleBackColor = true;
            this.butOffline.Click += new System.EventHandler(this.butOffline_Click);
            // 
            // txtHostMode
            // 
            this.txtHostMode.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtHostMode.BackColor = System.Drawing.Color.Red;
            this.txtHostMode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHostMode.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtHostMode.Font = new System.Drawing.Font("Arial", 14.25F);
            this.txtHostMode.ForeColor = System.Drawing.Color.Black;
            this.txtHostMode.Location = new System.Drawing.Point(43, 60);
            this.txtHostMode.Margin = new System.Windows.Forms.Padding(0);
            this.txtHostMode.Name = "txtHostMode";
            this.txtHostMode.ReadOnly = true;
            this.txtHostMode.Size = new System.Drawing.Size(187, 29);
            this.txtHostMode.TabIndex = 13;
            this.txtHostMode.Text = "OffLine";
            this.txtHostMode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // butOnlineRemote
            // 
            this.butOnlineRemote.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.butOnlineRemote.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold);
            this.butOnlineRemote.ForeColor = System.Drawing.Color.Black;
            this.butOnlineRemote.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.butOnlineRemote.Location = new System.Drawing.Point(51, 131);
            this.butOnlineRemote.Name = "butOnlineRemote";
            this.butOnlineRemote.Size = new System.Drawing.Size(168, 35);
            this.butOnlineRemote.TabIndex = 16;
            this.butOnlineRemote.Text = "Online Remote Mode";
            this.butOnlineRemote.UseVisualStyleBackColor = true;
            this.butOnlineRemote.Click += new System.EventHandler(this.butOnlineRemote_Click);
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.DimGray;
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(-6, 97);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(270, 1);
            this.label5.TabIndex = 69;
            this.label5.Text = "label5";
            // 
            // skinGroupBox1
            // 
            this.skinGroupBox1.BackColor = System.Drawing.Color.Transparent;
            this.skinGroupBox1.BorderColor = System.Drawing.Color.Black;
            this.skinGroupBox1.Controls.Add(this.label3);
            this.skinGroupBox1.Controls.Add(this.label1);
            this.skinGroupBox1.Controls.Add(this.butEnable);
            this.skinGroupBox1.Controls.Add(this.butDisable);
            this.skinGroupBox1.Controls.Add(this.label2);
            this.skinGroupBox1.Controls.Add(this.txtCommuntion);
            this.skinGroupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.skinGroupBox1.Font = new System.Drawing.Font("Arial", 15.75F);
            this.skinGroupBox1.ForeColor = System.Drawing.Color.Black;
            this.skinGroupBox1.Location = new System.Drawing.Point(9, 12);
            this.skinGroupBox1.Name = "skinGroupBox1";
            this.skinGroupBox1.Radius = 20;
            this.skinGroupBox1.RectBackColor = System.Drawing.SystemColors.Control;
            this.skinGroupBox1.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.skinGroupBox1.Size = new System.Drawing.Size(264, 279);
            this.skinGroupBox1.TabIndex = 71;
            this.skinGroupBox1.TabStop = false;
            this.skinGroupBox1.Text = "Communication";
            this.skinGroupBox1.TitleBorderColor = System.Drawing.Color.Black;
            this.skinGroupBox1.TitleRadius = 10;
            this.skinGroupBox1.TitleRectBackColor = System.Drawing.Color.LightSkyBlue;
            this.skinGroupBox1.TitleRoundStyle = CCWin.SkinClass.RoundStyle.All;
            // 
            // label3
            // 
            this.label3.Enabled = false;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(62, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(156, 23);
            this.label3.TabIndex = 78;
            this.label3.Text = "Mode Change";
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(61, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 23);
            this.label1.TabIndex = 77;
            this.label1.Text = "Current Status";
            // 
            // butEnable
            // 
            this.butEnable.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.butEnable.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold);
            this.butEnable.ForeColor = System.Drawing.Color.Black;
            this.butEnable.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.butEnable.Location = new System.Drawing.Point(49, 178);
            this.butEnable.Name = "butEnable";
            this.butEnable.Size = new System.Drawing.Size(168, 35);
            this.butEnable.TabIndex = 20;
            this.butEnable.Text = "Enable";
            this.butEnable.UseVisualStyleBackColor = true;
            this.butEnable.Click += new System.EventHandler(this.butEnable_Click);
            // 
            // butDisable
            // 
            this.butDisable.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.butDisable.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold);
            this.butDisable.ForeColor = System.Drawing.Color.Black;
            this.butDisable.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.butDisable.Location = new System.Drawing.Point(49, 131);
            this.butDisable.Name = "butDisable";
            this.butDisable.Size = new System.Drawing.Size(168, 35);
            this.butDisable.TabIndex = 21;
            this.butDisable.Text = "Disable";
            this.butDisable.UseVisualStyleBackColor = true;
            this.butDisable.Click += new System.EventHandler(this.butDisable_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.DimGray;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(-6, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(270, 1);
            this.label2.TabIndex = 69;
            this.label2.Text = "label2";
            // 
            // txtCommuntion
            // 
            this.txtCommuntion.BackColor = System.Drawing.Color.Lime;
            this.txtCommuntion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCommuntion.Font = new System.Drawing.Font("Arial", 14.25F);
            this.txtCommuntion.ForeColor = System.Drawing.Color.Black;
            this.txtCommuntion.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txtCommuntion.Location = new System.Drawing.Point(41, 59);
            this.txtCommuntion.Name = "txtCommuntion";
            this.txtCommuntion.Size = new System.Drawing.Size(187, 29);
            this.txtCommuntion.TabIndex = 36;
            this.txtCommuntion.Text = "Enable";
            this.txtCommuntion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // HostModeChg_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(952, 328);
            this.Controls.Add(this.skinGroupBox3);
            this.Controls.Add(this.skinGroupBox2);
            this.Controls.Add(this.skinGroupBox1);
            this.Name = "HostModeChg_Form";
            this.Text = "HostModeChg_Form";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.HostModeChg_Form_FormClosed);
            this.skinGroupBox3.ResumeLayout(false);
            this.skinGroupBox2.ResumeLayout(false);
            this.skinGroupBox2.PerformLayout();
            this.skinGroupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private CCWin.SkinControl.SkinGroupBox skinGroupBox3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button butLcsPause;
        private System.Windows.Forms.Button butLcsAuto;
        private System.Windows.Forms.Label lblLCSStatusValue;
        private System.Windows.Forms.Label label8;
        private CCWin.SkinControl.SkinGroupBox skinGroupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button butOffline;
        private System.Windows.Forms.TextBox txtHostMode;
        private System.Windows.Forms.Button butOnlineRemote;
        private System.Windows.Forms.Label label5;
        private CCWin.SkinControl.SkinGroupBox skinGroupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button butEnable;
        private System.Windows.Forms.Button butDisable;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label txtCommuntion;
        private System.Windows.Forms.Timer timer1;
    }
}