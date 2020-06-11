namespace com.mirle.ibg3k0.bc.winform.UI
{
    partial class CarrierMaintenanceForm
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
            this.skinGroupBox1 = new CCWin.SkinControl.SkinGroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txt_InstalledCSTID = new System.Windows.Forms.TextBox();
            this.cmb_InstalledVhID = new System.Windows.Forms.ComboBox();
            this.cmb_installedTransferPortID = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_installed_vh_id = new System.Windows.Forms.Label();
            this.btn_installed = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.btn_remove = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmb_removeTransferPortID = new System.Windows.Forms.ComboBox();
            this.cmb_RemoveVhID = new System.Windows.Forms.ComboBox();
            this.txt_RemoveCSTID = new System.Windows.Forms.TextBox();
            this.skinGroupBox2 = new CCWin.SkinControl.SkinGroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.skinGroupBox1.SuspendLayout();
            this.skinGroupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // skinGroupBox1
            // 
            this.skinGroupBox1.BackColor = System.Drawing.Color.Transparent;
            this.skinGroupBox1.BorderColor = System.Drawing.Color.Black;
            this.skinGroupBox1.Controls.Add(this.label8);
            this.skinGroupBox1.Controls.Add(this.txt_InstalledCSTID);
            this.skinGroupBox1.Controls.Add(this.cmb_InstalledVhID);
            this.skinGroupBox1.Controls.Add(this.cmb_installedTransferPortID);
            this.skinGroupBox1.Controls.Add(this.label3);
            this.skinGroupBox1.Controls.Add(this.label1);
            this.skinGroupBox1.Controls.Add(this.lbl_installed_vh_id);
            this.skinGroupBox1.Controls.Add(this.btn_installed);
            this.skinGroupBox1.Controls.Add(this.label2);
            this.skinGroupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.skinGroupBox1.Font = new System.Drawing.Font("Arial", 15.75F);
            this.skinGroupBox1.ForeColor = System.Drawing.Color.Black;
            this.skinGroupBox1.Location = new System.Drawing.Point(9, 12);
            this.skinGroupBox1.Name = "skinGroupBox1";
            this.skinGroupBox1.Radius = 20;
            this.skinGroupBox1.RectBackColor = System.Drawing.SystemColors.Control;
            this.skinGroupBox1.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.skinGroupBox1.Size = new System.Drawing.Size(395, 315);
            this.skinGroupBox1.TabIndex = 71;
            this.skinGroupBox1.TabStop = false;
            this.skinGroupBox1.Text = "Carrier Installed";
            this.skinGroupBox1.TitleBorderColor = System.Drawing.Color.Black;
            this.skinGroupBox1.TitleRadius = 10;
            this.skinGroupBox1.TitleRectBackColor = System.Drawing.Color.LightSkyBlue;
            this.skinGroupBox1.TitleRoundStyle = CCWin.SkinClass.RoundStyle.All;
            // 
            // label8
            // 
            this.label8.ForeColor = System.Drawing.Color.Red;
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(6, 289);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(353, 23);
            this.label8.TabIndex = 83;
            this.label8.Text = "*It can only be sent once a minute.";
            // 
            // txt_InstalledCSTID
            // 
            this.txt_InstalledCSTID.Location = new System.Drawing.Point(10, 118);
            this.txt_InstalledCSTID.Multiline = true;
            this.txt_InstalledCSTID.Name = "txt_InstalledCSTID";
            this.txt_InstalledCSTID.Size = new System.Drawing.Size(305, 66);
            this.txt_InstalledCSTID.TabIndex = 82;
            // 
            // cmb_InstalledVhID
            // 
            this.cmb_InstalledVhID.FormattingEnabled = true;
            this.cmb_InstalledVhID.Location = new System.Drawing.Point(10, 59);
            this.cmb_InstalledVhID.Name = "cmb_InstalledVhID";
            this.cmb_InstalledVhID.Size = new System.Drawing.Size(180, 32);
            this.cmb_InstalledVhID.TabIndex = 81;
            // 
            // cmb_installedTransferPortID
            // 
            this.cmb_installedTransferPortID.FormattingEnabled = true;
            this.cmb_installedTransferPortID.Location = new System.Drawing.Point(10, 213);
            this.cmb_installedTransferPortID.Name = "cmb_installedTransferPortID";
            this.cmb_installedTransferPortID.Size = new System.Drawing.Size(180, 32);
            this.cmb_installedTransferPortID.TabIndex = 80;
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(6, 187);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(156, 23);
            this.label3.TabIndex = 79;
            this.label3.Text = "Transfer Port:";
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(6, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 23);
            this.label1.TabIndex = 78;
            this.label1.Text = "CST ID:";
            // 
            // lbl_installed_vh_id
            // 
            this.lbl_installed_vh_id.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_installed_vh_id.Location = new System.Drawing.Point(6, 35);
            this.lbl_installed_vh_id.Name = "lbl_installed_vh_id";
            this.lbl_installed_vh_id.Size = new System.Drawing.Size(70, 23);
            this.lbl_installed_vh_id.TabIndex = 77;
            this.lbl_installed_vh_id.Text = "Vh ID:";
            // 
            // btn_installed
            // 
            this.btn_installed.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btn_installed.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold);
            this.btn_installed.ForeColor = System.Drawing.Color.Black;
            this.btn_installed.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_installed.Location = new System.Drawing.Point(109, 252);
            this.btn_installed.Name = "btn_installed";
            this.btn_installed.Size = new System.Drawing.Size(168, 35);
            this.btn_installed.TabIndex = 20;
            this.btn_installed.Text = "Installed";
            this.btn_installed.UseVisualStyleBackColor = true;
            this.btn_installed.Click += new System.EventHandler(this.btn_installed_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.DimGray;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(-3, 248);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(490, 1);
            this.label2.TabIndex = 69;
            this.label2.Text = "label2";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.DimGray;
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(-3, 248);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(490, 1);
            this.label7.TabIndex = 69;
            this.label7.Text = "label7";
            // 
            // btn_remove
            // 
            this.btn_remove.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btn_remove.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold);
            this.btn_remove.ForeColor = System.Drawing.Color.Black;
            this.btn_remove.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_remove.Location = new System.Drawing.Point(118, 252);
            this.btn_remove.Name = "btn_remove";
            this.btn_remove.Size = new System.Drawing.Size(168, 35);
            this.btn_remove.TabIndex = 20;
            this.btn_remove.Text = "Remove";
            this.btn_remove.UseVisualStyleBackColor = true;
            this.btn_remove.Click += new System.EventHandler(this.btn_remove_Click);
            // 
            // label6
            // 
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(6, 35);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 23);
            this.label6.TabIndex = 77;
            this.label6.Text = "Vh ID:";
            // 
            // label5
            // 
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(6, 92);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(156, 23);
            this.label5.TabIndex = 78;
            this.label5.Text = "CST ID:";
            // 
            // label4
            // 
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(6, 187);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(156, 23);
            this.label4.TabIndex = 79;
            this.label4.Text = "Transfer Port:";
            // 
            // cmb_removeTransferPortID
            // 
            this.cmb_removeTransferPortID.FormattingEnabled = true;
            this.cmb_removeTransferPortID.Location = new System.Drawing.Point(10, 213);
            this.cmb_removeTransferPortID.Name = "cmb_removeTransferPortID";
            this.cmb_removeTransferPortID.Size = new System.Drawing.Size(180, 32);
            this.cmb_removeTransferPortID.TabIndex = 80;
            // 
            // cmb_RemoveVhID
            // 
            this.cmb_RemoveVhID.FormattingEnabled = true;
            this.cmb_RemoveVhID.Location = new System.Drawing.Point(10, 59);
            this.cmb_RemoveVhID.Name = "cmb_RemoveVhID";
            this.cmb_RemoveVhID.Size = new System.Drawing.Size(180, 32);
            this.cmb_RemoveVhID.TabIndex = 81;
            // 
            // txt_RemoveCSTID
            // 
            this.txt_RemoveCSTID.Location = new System.Drawing.Point(10, 118);
            this.txt_RemoveCSTID.Multiline = true;
            this.txt_RemoveCSTID.Name = "txt_RemoveCSTID";
            this.txt_RemoveCSTID.Size = new System.Drawing.Size(305, 66);
            this.txt_RemoveCSTID.TabIndex = 82;
            // 
            // skinGroupBox2
            // 
            this.skinGroupBox2.BackColor = System.Drawing.Color.Transparent;
            this.skinGroupBox2.BorderColor = System.Drawing.Color.Black;
            this.skinGroupBox2.Controls.Add(this.label9);
            this.skinGroupBox2.Controls.Add(this.txt_RemoveCSTID);
            this.skinGroupBox2.Controls.Add(this.cmb_RemoveVhID);
            this.skinGroupBox2.Controls.Add(this.cmb_removeTransferPortID);
            this.skinGroupBox2.Controls.Add(this.label4);
            this.skinGroupBox2.Controls.Add(this.label5);
            this.skinGroupBox2.Controls.Add(this.label6);
            this.skinGroupBox2.Controls.Add(this.btn_remove);
            this.skinGroupBox2.Controls.Add(this.label7);
            this.skinGroupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.skinGroupBox2.Font = new System.Drawing.Font("Arial", 15.75F);
            this.skinGroupBox2.ForeColor = System.Drawing.Color.Black;
            this.skinGroupBox2.Location = new System.Drawing.Point(410, 12);
            this.skinGroupBox2.Name = "skinGroupBox2";
            this.skinGroupBox2.Radius = 20;
            this.skinGroupBox2.RectBackColor = System.Drawing.SystemColors.Control;
            this.skinGroupBox2.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.skinGroupBox2.Size = new System.Drawing.Size(395, 315);
            this.skinGroupBox2.TabIndex = 83;
            this.skinGroupBox2.TabStop = false;
            this.skinGroupBox2.Text = "Carrier Remove";
            this.skinGroupBox2.TitleBorderColor = System.Drawing.Color.Black;
            this.skinGroupBox2.TitleRadius = 10;
            this.skinGroupBox2.TitleRectBackColor = System.Drawing.Color.LightSkyBlue;
            this.skinGroupBox2.TitleRoundStyle = CCWin.SkinClass.RoundStyle.All;
            // 
            // label9
            // 
            this.label9.ForeColor = System.Drawing.Color.Red;
            this.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label9.Location = new System.Drawing.Point(19, 289);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(353, 23);
            this.label9.TabIndex = 84;
            this.label9.Text = "*It can only be sent once a minute.";
            // 
            // CarrierMaintenanceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(816, 340);
            this.Controls.Add(this.skinGroupBox2);
            this.Controls.Add(this.skinGroupBox1);
            this.Name = "CarrierMaintenanceForm";
            this.Text = "Carrier Installed / Remove";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CarrierMaintenanceForm_FormClosed);
            this.skinGroupBox1.ResumeLayout(false);
            this.skinGroupBox1.PerformLayout();
            this.skinGroupBox2.ResumeLayout(false);
            this.skinGroupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private CCWin.SkinControl.SkinGroupBox skinGroupBox1;
        private System.Windows.Forms.Label lbl_installed_vh_id;
        private System.Windows.Forms.Button btn_installed;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmb_installedTransferPortID;
        private System.Windows.Forms.ComboBox cmb_InstalledVhID;
        private System.Windows.Forms.TextBox txt_InstalledCSTID;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btn_remove;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmb_removeTransferPortID;
        private System.Windows.Forms.ComboBox cmb_RemoveVhID;
        private System.Windows.Forms.TextBox txt_RemoveCSTID;
        private CCWin.SkinControl.SkinGroupBox skinGroupBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
    }
}