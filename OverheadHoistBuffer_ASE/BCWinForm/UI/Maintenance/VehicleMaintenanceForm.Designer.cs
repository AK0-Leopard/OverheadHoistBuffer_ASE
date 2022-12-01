namespace com.mirle.ibg3k0.bc.winform.UI
{
    partial class VehicleMaintenanceForm
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
            this.skinGroupBox1 = new CCWin.SkinControl.SkinGroupBox();
            this.txt_lastMantACCDateTime = new System.Windows.Forms.TextBox();
            this.txt_mantACCDist = new System.Windows.Forms.TextBox();
            this.txt_vhACCDist = new System.Windows.Forms.TextBox();
            this.btn_resetODO = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbl_odo = new System.Windows.Forms.Label();
            this.cmb_VhID = new System.Windows.Forms.ComboBox();
            this.lbl_installed_vh_id = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btn_resetGripCount = new System.Windows.Forms.Button();
            this.skinGroupBox2 = new CCWin.SkinControl.SkinGroupBox();
            this.txt_gripMantDateTime = new System.Windows.Forms.TextBox();
            this.txt_gripMantCount = new System.Windows.Forms.TextBox();
            this.txt_gripCount = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.skinGroupBox1.SuspendLayout();
            this.skinGroupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // skinGroupBox1
            // 
            this.skinGroupBox1.BackColor = System.Drawing.Color.Transparent;
            this.skinGroupBox1.BorderColor = System.Drawing.Color.Black;
            this.skinGroupBox1.Controls.Add(this.txt_lastMantACCDateTime);
            this.skinGroupBox1.Controls.Add(this.txt_mantACCDist);
            this.skinGroupBox1.Controls.Add(this.txt_vhACCDist);
            this.skinGroupBox1.Controls.Add(this.btn_resetODO);
            this.skinGroupBox1.Controls.Add(this.label11);
            this.skinGroupBox1.Controls.Add(this.label10);
            this.skinGroupBox1.Controls.Add(this.label2);
            this.skinGroupBox1.Controls.Add(this.lbl_odo);
            this.skinGroupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.skinGroupBox1.Font = new System.Drawing.Font("Arial", 15.75F);
            this.skinGroupBox1.ForeColor = System.Drawing.Color.Black;
            this.skinGroupBox1.Location = new System.Drawing.Point(8, 48);
            this.skinGroupBox1.Name = "skinGroupBox1";
            this.skinGroupBox1.Radius = 20;
            this.skinGroupBox1.RectBackColor = System.Drawing.SystemColors.Control;
            this.skinGroupBox1.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.skinGroupBox1.Size = new System.Drawing.Size(395, 315);
            this.skinGroupBox1.TabIndex = 71;
            this.skinGroupBox1.TabStop = false;
            this.skinGroupBox1.Text = "OHT 里程保養資訊";
            this.skinGroupBox1.TitleBorderColor = System.Drawing.Color.Black;
            this.skinGroupBox1.TitleRadius = 10;
            this.skinGroupBox1.TitleRectBackColor = System.Drawing.Color.LightSkyBlue;
            this.skinGroupBox1.TitleRoundStyle = CCWin.SkinClass.RoundStyle.All;
            // 
            // txt_lastMantACCDateTime
            // 
            this.txt_lastMantACCDateTime.Location = new System.Drawing.Point(33, 204);
            this.txt_lastMantACCDateTime.Name = "txt_lastMantACCDateTime";
            this.txt_lastMantACCDateTime.ReadOnly = true;
            this.txt_lastMantACCDateTime.Size = new System.Drawing.Size(305, 32);
            this.txt_lastMantACCDateTime.TabIndex = 87;
            this.txt_lastMantACCDateTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_mantACCDist
            // 
            this.txt_mantACCDist.Location = new System.Drawing.Point(33, 130);
            this.txt_mantACCDist.Name = "txt_mantACCDist";
            this.txt_mantACCDist.ReadOnly = true;
            this.txt_mantACCDist.Size = new System.Drawing.Size(305, 32);
            this.txt_mantACCDist.TabIndex = 86;
            this.txt_mantACCDist.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_vhACCDist
            // 
            this.txt_vhACCDist.Location = new System.Drawing.Point(33, 60);
            this.txt_vhACCDist.Name = "txt_vhACCDist";
            this.txt_vhACCDist.ReadOnly = true;
            this.txt_vhACCDist.Size = new System.Drawing.Size(305, 32);
            this.txt_vhACCDist.TabIndex = 85;
            this.txt_vhACCDist.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btn_resetODO
            // 
            this.btn_resetODO.Enabled = false;
            this.btn_resetODO.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.btn_resetODO.Location = new System.Drawing.Point(105, 260);
            this.btn_resetODO.Name = "btn_resetODO";
            this.btn_resetODO.Size = new System.Drawing.Size(159, 37);
            this.btn_resetODO.TabIndex = 59;
            this.btn_resetODO.Text = "重置保養里程";
            this.btn_resetODO.UseVisualStyleBackColor = true;
            this.btn_resetODO.Click += new System.EventHandler(this.btn_resetODO_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial", 15.75F);
            this.label11.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label11.Location = new System.Drawing.Point(6, 177);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(208, 24);
            this.label11.TabIndex = 62;
            this.label11.Text = "上次保養時間           ";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 15.75F);
            this.label10.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label10.Location = new System.Drawing.Point(6, 103);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(208, 24);
            this.label10.TabIndex = 61;
            this.label10.Text = "保養累積里程           ";
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
            // lbl_odo
            // 
            this.lbl_odo.AutoSize = true;
            this.lbl_odo.Font = new System.Drawing.Font("Arial", 15.75F);
            this.lbl_odo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_odo.Location = new System.Drawing.Point(6, 35);
            this.lbl_odo.Name = "lbl_odo";
            this.lbl_odo.Size = new System.Drawing.Size(106, 24);
            this.lbl_odo.TabIndex = 28;
            this.lbl_odo.Text = "總里程     ";
            // 
            // cmb_VhID
            // 
            this.cmb_VhID.Font = new System.Drawing.Font("Arial", 15.75F);
            this.cmb_VhID.FormattingEnabled = true;
            this.cmb_VhID.Location = new System.Drawing.Point(78, 6);
            this.cmb_VhID.Name = "cmb_VhID";
            this.cmb_VhID.Size = new System.Drawing.Size(268, 32);
            this.cmb_VhID.TabIndex = 81;
            this.cmb_VhID.SelectedIndexChanged += new System.EventHandler(this.cmb_VhID_SelectedIndexChanged);
            // 
            // lbl_installed_vh_id
            // 
            this.lbl_installed_vh_id.Font = new System.Drawing.Font("Arial", 15.75F);
            this.lbl_installed_vh_id.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_installed_vh_id.Location = new System.Drawing.Point(14, 9);
            this.lbl_installed_vh_id.Name = "lbl_installed_vh_id";
            this.lbl_installed_vh_id.Size = new System.Drawing.Size(70, 23);
            this.lbl_installed_vh_id.TabIndex = 77;
            this.lbl_installed_vh_id.Text = "Vh ID:";
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
            // btn_resetGripCount
            // 
            this.btn_resetGripCount.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btn_resetGripCount.Enabled = false;
            this.btn_resetGripCount.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold);
            this.btn_resetGripCount.ForeColor = System.Drawing.Color.Black;
            this.btn_resetGripCount.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_resetGripCount.Location = new System.Drawing.Point(118, 260);
            this.btn_resetGripCount.Name = "btn_resetGripCount";
            this.btn_resetGripCount.Size = new System.Drawing.Size(168, 35);
            this.btn_resetGripCount.TabIndex = 20;
            this.btn_resetGripCount.Text = "重置保養次數";
            this.btn_resetGripCount.UseVisualStyleBackColor = true;
            this.btn_resetGripCount.Click += new System.EventHandler(this.btn_resetGripCount_Click);
            // 
            // skinGroupBox2
            // 
            this.skinGroupBox2.BackColor = System.Drawing.Color.Transparent;
            this.skinGroupBox2.BorderColor = System.Drawing.Color.Black;
            this.skinGroupBox2.Controls.Add(this.txt_gripMantDateTime);
            this.skinGroupBox2.Controls.Add(this.btn_resetGripCount);
            this.skinGroupBox2.Controls.Add(this.txt_gripMantCount);
            this.skinGroupBox2.Controls.Add(this.txt_gripCount);
            this.skinGroupBox2.Controls.Add(this.label7);
            this.skinGroupBox2.Controls.Add(this.label1);
            this.skinGroupBox2.Controls.Add(this.label4);
            this.skinGroupBox2.Controls.Add(this.label3);
            this.skinGroupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.skinGroupBox2.Font = new System.Drawing.Font("Arial", 15.75F);
            this.skinGroupBox2.ForeColor = System.Drawing.Color.Black;
            this.skinGroupBox2.Location = new System.Drawing.Point(409, 48);
            this.skinGroupBox2.Name = "skinGroupBox2";
            this.skinGroupBox2.Radius = 20;
            this.skinGroupBox2.RectBackColor = System.Drawing.SystemColors.Control;
            this.skinGroupBox2.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.skinGroupBox2.Size = new System.Drawing.Size(395, 315);
            this.skinGroupBox2.TabIndex = 83;
            this.skinGroupBox2.TabStop = false;
            this.skinGroupBox2.Text = "OHT 夾爪保養資訊";
            this.skinGroupBox2.TitleBorderColor = System.Drawing.Color.Black;
            this.skinGroupBox2.TitleRadius = 10;
            this.skinGroupBox2.TitleRectBackColor = System.Drawing.Color.LightSkyBlue;
            this.skinGroupBox2.TitleRoundStyle = CCWin.SkinClass.RoundStyle.All;
            // 
            // txt_gripMantDateTime
            // 
            this.txt_gripMantDateTime.Location = new System.Drawing.Point(50, 204);
            this.txt_gripMantDateTime.Name = "txt_gripMantDateTime";
            this.txt_gripMantDateTime.ReadOnly = true;
            this.txt_gripMantDateTime.Size = new System.Drawing.Size(305, 32);
            this.txt_gripMantDateTime.TabIndex = 93;
            this.txt_gripMantDateTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_gripMantCount
            // 
            this.txt_gripMantCount.Location = new System.Drawing.Point(50, 130);
            this.txt_gripMantCount.Name = "txt_gripMantCount";
            this.txt_gripMantCount.ReadOnly = true;
            this.txt_gripMantCount.Size = new System.Drawing.Size(305, 32);
            this.txt_gripMantCount.TabIndex = 92;
            this.txt_gripMantCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_gripCount
            // 
            this.txt_gripCount.Location = new System.Drawing.Point(50, 60);
            this.txt_gripCount.Name = "txt_gripCount";
            this.txt_gripCount.ReadOnly = true;
            this.txt_gripCount.Size = new System.Drawing.Size(305, 32);
            this.txt_gripCount.TabIndex = 91;
            this.txt_gripCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 15.75F);
            this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Location = new System.Drawing.Point(23, 177);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 24);
            this.label1.TabIndex = 90;
            this.label1.Text = "上次保養時間";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 15.75F);
            this.label4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label4.Location = new System.Drawing.Point(23, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 24);
            this.label4.TabIndex = 88;
            this.label4.Text = "總伸爪次數";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 15.75F);
            this.label3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label3.Location = new System.Drawing.Point(23, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(142, 24);
            this.label3.TabIndex = 89;
            this.label3.Text = "保養累積次數";
            // 
            // VehicleMaintenanceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 387);
            this.Controls.Add(this.skinGroupBox2);
            this.Controls.Add(this.skinGroupBox1);
            this.Controls.Add(this.cmb_VhID);
            this.Controls.Add(this.lbl_installed_vh_id);
            this.Name = "VehicleMaintenanceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Vehicle Maintenance";
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmb_VhID;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btn_resetGripCount;
        private CCWin.SkinControl.SkinGroupBox skinGroupBox2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lbl_odo;
        private System.Windows.Forms.Button btn_resetODO;
        private System.Windows.Forms.TextBox txt_vhACCDist;
        private System.Windows.Forms.TextBox txt_lastMantACCDateTime;
        private System.Windows.Forms.TextBox txt_mantACCDist;
        private System.Windows.Forms.TextBox txt_gripMantDateTime;
        private System.Windows.Forms.TextBox txt_gripMantCount;
        private System.Windows.Forms.TextBox txt_gripCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}