namespace com.mirle.ibg3k0.bc.winform.UI
{
    partial class MPCInfoMsgDialog
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.m_titleLbl = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.m_MPCTipMsgGBox = new CCWin.SkinControl.SkinGroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.m_TipMsgDGV = new System.Windows.Forms.DataGridView();
            this.m_confirmBtn = new CCWin.SkinControl.SkinButton();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MsgLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Msg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.m_MPCTipMsgGBox.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_TipMsgDGV)).BeginInit();
            this.SuspendLayout();
            // 
            // m_titleLbl
            // 
            this.m_titleLbl.AutoSize = true;
            this.m_titleLbl.BackColor = System.Drawing.Color.Transparent;
            this.m_titleLbl.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold);
            this.m_titleLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.m_titleLbl.Location = new System.Drawing.Point(275, 35);
            this.m_titleLbl.Name = "m_titleLbl";
            this.m_titleLbl.Size = new System.Drawing.Size(449, 29);
            this.m_titleLbl.TabIndex = 3;
            this.m_titleLbl.Text = "Please confirm the following message";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::com.mirle.ibg3k0.bc.winform.Properties.Resources.alert;
            this.pictureBox1.Location = new System.Drawing.Point(191, 15);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(77, 62);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // m_MPCTipMsgGBox
            // 
            this.m_MPCTipMsgGBox.BackColor = System.Drawing.Color.Transparent;
            this.m_MPCTipMsgGBox.BorderColor = System.Drawing.Color.Black;
            this.m_MPCTipMsgGBox.Controls.Add(this.panel1);
            this.m_MPCTipMsgGBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.m_MPCTipMsgGBox.Font = new System.Drawing.Font("Arial", 15.75F);
            this.m_MPCTipMsgGBox.ForeColor = System.Drawing.Color.Black;
            this.m_MPCTipMsgGBox.Location = new System.Drawing.Point(14, 85);
            this.m_MPCTipMsgGBox.Name = "m_MPCTipMsgGBox";
            this.m_MPCTipMsgGBox.Radius = 20;
            this.m_MPCTipMsgGBox.RectBackColor = System.Drawing.SystemColors.Control;
            this.m_MPCTipMsgGBox.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.m_MPCTipMsgGBox.Size = new System.Drawing.Size(866, 240);
            this.m_MPCTipMsgGBox.TabIndex = 5;
            this.m_MPCTipMsgGBox.TabStop = false;
            this.m_MPCTipMsgGBox.Text = "Tip Message";
            this.m_MPCTipMsgGBox.TitleBorderColor = System.Drawing.Color.Black;
            this.m_MPCTipMsgGBox.TitleRadius = 10;
            this.m_MPCTipMsgGBox.TitleRectBackColor = System.Drawing.Color.LightSteelBlue;
            this.m_MPCTipMsgGBox.TitleRoundStyle = CCWin.SkinClass.RoundStyle.All;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.m_TipMsgDGV);
            this.panel1.Font = new System.Drawing.Font("Calibri", 15.75F);
            this.panel1.Location = new System.Drawing.Point(7, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(848, 204);
            this.panel1.TabIndex = 0;
            // 
            // m_TipMsgDGV
            // 
            this.m_TipMsgDGV.AllowUserToAddRows = false;
            this.m_TipMsgDGV.AllowUserToDeleteRows = false;
            this.m_TipMsgDGV.AllowUserToOrderColumns = true;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.PaleTurquoise;
            this.m_TipMsgDGV.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.m_TipMsgDGV.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.m_TipMsgDGV.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.m_TipMsgDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_TipMsgDGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Time,
            this.MsgLevel,
            this.Msg});
            this.m_TipMsgDGV.Location = new System.Drawing.Point(0, 0);
            this.m_TipMsgDGV.Margin = new System.Windows.Forms.Padding(0);
            this.m_TipMsgDGV.Name = "m_TipMsgDGV";
            this.m_TipMsgDGV.ReadOnly = true;
            this.m_TipMsgDGV.RowHeadersVisible = false;
            this.m_TipMsgDGV.RowTemplate.Height = 24;
            this.m_TipMsgDGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.m_TipMsgDGV.ShowEditingIcon = false;
            this.m_TipMsgDGV.Size = new System.Drawing.Size(848, 204);
            this.m_TipMsgDGV.TabIndex = 2;
            // 
            // m_confirmBtn
            // 
            this.m_confirmBtn.BackColor = System.Drawing.Color.Transparent;
            this.m_confirmBtn.BaseColor = System.Drawing.Color.LightGray;
            this.m_confirmBtn.BorderColor = System.Drawing.Color.Black;
            this.m_confirmBtn.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.m_confirmBtn.DownBack = null;
            this.m_confirmBtn.DownBaseColor = System.Drawing.Color.RoyalBlue;
            this.m_confirmBtn.Font = new System.Drawing.Font("Arial", 14.25F);
            this.m_confirmBtn.Image = global::com.mirle.ibg3k0.bc.winform.Properties.Resources.Exit;
            this.m_confirmBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.m_confirmBtn.ImageSize = new System.Drawing.Size(24, 24);
            this.m_confirmBtn.Location = new System.Drawing.Point(753, 345);
            this.m_confirmBtn.MouseBack = null;
            this.m_confirmBtn.Name = "m_confirmBtn";
            this.m_confirmBtn.NormlBack = null;
            this.m_confirmBtn.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.m_confirmBtn.Size = new System.Drawing.Size(127, 32);
            this.m_confirmBtn.TabIndex = 82;
            this.m_confirmBtn.Text = "Close";
            this.m_confirmBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.m_confirmBtn.UseVisualStyleBackColor = false;
            this.m_confirmBtn.Click += new System.EventHandler(this.m_confirmBtn_Click);
            // 
            // Time
            // 
            this.Time.DataPropertyName = "Time";
            this.Time.HeaderText = "Time";
            this.Time.Name = "Time";
            this.Time.ReadOnly = true;
            this.Time.Width = 160;
            // 
            // MsgLevel
            // 
            this.MsgLevel.DataPropertyName = "MsgLevel";
            this.MsgLevel.HeaderText = "Level";
            this.MsgLevel.Name = "MsgLevel";
            this.MsgLevel.ReadOnly = true;
            this.MsgLevel.Width = 70;
            // 
            // Msg
            // 
            this.Msg.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Msg.DataPropertyName = "Msg";
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Msg.DefaultCellStyle = dataGridViewCellStyle4;
            this.Msg.HeaderText = "Message";
            this.Msg.Name = "Msg";
            this.Msg.ReadOnly = true;
            // 
            // MPCInfoMsgDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::com.mirle.ibg3k0.bc.winform.Properties.Resources.main;
            this.ClientSize = new System.Drawing.Size(898, 398);
            this.Controls.Add(this.m_confirmBtn);
            this.Controls.Add(this.m_MPCTipMsgGBox);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.m_titleLbl);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MPCInfoMsgDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "OHT Control Tip Message";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MPCInfoMsgDialog_FormClosed);
            this.Load += new System.EventHandler(this.MPCInfoMsgDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.m_MPCTipMsgGBox.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_TipMsgDGV)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_titleLbl;
        private System.Windows.Forms.PictureBox pictureBox1;
        private CCWin.SkinControl.SkinGroupBox m_MPCTipMsgGBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView m_TipMsgDGV;
        private CCWin.SkinControl.SkinButton m_confirmBtn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn MsgLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn Msg;
    }
}