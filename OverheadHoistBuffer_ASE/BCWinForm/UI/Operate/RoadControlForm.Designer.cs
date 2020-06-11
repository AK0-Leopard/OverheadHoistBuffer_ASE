namespace com.mirle.ibg3k0.bc.winform.UI
{
    partial class RoadControlForm
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
            this.btn_disable = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.btn_enable = new System.Windows.Forms.Button();
            this.dgv_segment = new System.Windows.Forms.DataGridView();
            this.seg_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pre_disable_flag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pre_disable_time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.disable_time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.user_disable_flag = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.safety_disable_flag = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.hid_disable_flag = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.system_disable_flag = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pel_button = new System.Windows.Forms.Panel();
            this.btn_segment_enable_cv = new System.Windows.Forms.Button();
            this.btn_disable_vh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_segment)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.pel_button.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_disable
            // 
            this.btn_disable.Location = new System.Drawing.Point(90, 12);
            this.btn_disable.Name = "btn_disable";
            this.btn_disable.Size = new System.Drawing.Size(75, 23);
            this.btn_disable.TabIndex = 2;
            this.btn_disable.Text = "Disable";
            this.btn_disable.UseVisualStyleBackColor = true;
            this.btn_disable.Click += new System.EventHandler(this.btn_disable_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(171, 12);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_cancel.TabIndex = 3;
            this.btn_cancel.Text = "Cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // btn_enable
            // 
            this.btn_enable.Location = new System.Drawing.Point(9, 12);
            this.btn_enable.Name = "btn_enable";
            this.btn_enable.Size = new System.Drawing.Size(75, 23);
            this.btn_enable.TabIndex = 2;
            this.btn_enable.Text = "Enable";
            this.btn_enable.UseVisualStyleBackColor = true;
            this.btn_enable.Click += new System.EventHandler(this.btn_enable_Click);
            // 
            // dgv_segment
            // 
            this.dgv_segment.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_segment.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.seg_num,
            this.status,
            this.pre_disable_flag,
            this.pre_disable_time,
            this.disable_time,
            this.user_disable_flag,
            this.safety_disable_flag,
            this.hid_disable_flag,
            this.system_disable_flag});
            this.dgv_segment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_segment.Location = new System.Drawing.Point(3, 3);
            this.dgv_segment.MultiSelect = false;
            this.dgv_segment.Name = "dgv_segment";
            this.dgv_segment.ReadOnly = true;
            this.dgv_segment.RowTemplate.Height = 24;
            this.dgv_segment.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_segment.Size = new System.Drawing.Size(1167, 649);
            this.dgv_segment.TabIndex = 5;
            this.dgv_segment.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_segment_CellClick);
            // 
            // seg_num
            // 
            this.seg_num.DataPropertyName = "SEG_NUM";
            this.seg_num.HeaderText = "SEG NUM";
            this.seg_num.Name = "seg_num";
            this.seg_num.ReadOnly = true;
            // 
            // status
            // 
            this.status.DataPropertyName = "STATUS";
            this.status.HeaderText = "STATUS";
            this.status.Name = "status";
            this.status.ReadOnly = true;
            // 
            // pre_disable_flag
            // 
            this.pre_disable_flag.DataPropertyName = "PRE_DISABLE_FLAG";
            this.pre_disable_flag.HeaderText = "PRE DISABLE FLAG";
            this.pre_disable_flag.Name = "pre_disable_flag";
            this.pre_disable_flag.ReadOnly = true;
            this.pre_disable_flag.Width = 200;
            // 
            // pre_disable_time
            // 
            this.pre_disable_time.DataPropertyName = "PRE_DISABLE_TIME";
            this.pre_disable_time.HeaderText = "PRE DISABLE TIME";
            this.pre_disable_time.Name = "pre_disable_time";
            this.pre_disable_time.ReadOnly = true;
            this.pre_disable_time.Width = 200;
            // 
            // disable_time
            // 
            this.disable_time.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.disable_time.DataPropertyName = "DISABLE_TIME";
            this.disable_time.HeaderText = "DISABLE TIME";
            this.disable_time.Name = "disable_time";
            this.disable_time.ReadOnly = true;
            // 
            // user_disable_flag
            // 
            this.user_disable_flag.DataPropertyName = "DISABLE_FLAG_USER";
            this.user_disable_flag.HeaderText = "USER DISABLE FLAG";
            this.user_disable_flag.Name = "user_disable_flag";
            this.user_disable_flag.ReadOnly = true;
            // 
            // safety_disable_flag
            // 
            this.safety_disable_flag.DataPropertyName = "DISABLE_FLAG_SAFETY";
            this.safety_disable_flag.HeaderText = "SAFETY DISABLE FLAG";
            this.safety_disable_flag.Name = "safety_disable_flag";
            this.safety_disable_flag.ReadOnly = true;
            // 
            // hid_disable_flag
            // 
            this.hid_disable_flag.DataPropertyName = "DISABLE_FLAG_HID";
            this.hid_disable_flag.HeaderText = "HID DISABLE FALG";
            this.hid_disable_flag.Name = "hid_disable_flag";
            this.hid_disable_flag.ReadOnly = true;
            // 
            // system_disable_flag
            // 
            this.system_disable_flag.DataPropertyName = "DISABLE_FLAG_SYSTEM";
            this.system_disable_flag.HeaderText = "SYSTEM DISABLE FLAG";
            this.system_disable_flag.Name = "system_disable_flag";
            this.system_disable_flag.ReadOnly = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.dgv_segment, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pel_button, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1173, 731);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // pel_button
            // 
            this.pel_button.Controls.Add(this.btn_segment_enable_cv);
            this.pel_button.Controls.Add(this.btn_disable_vh);
            this.pel_button.Controls.Add(this.btn_enable);
            this.pel_button.Controls.Add(this.btn_cancel);
            this.pel_button.Controls.Add(this.btn_disable);
            this.pel_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pel_button.Location = new System.Drawing.Point(3, 658);
            this.pel_button.Name = "pel_button";
            this.pel_button.Size = new System.Drawing.Size(1167, 70);
            this.pel_button.TabIndex = 6;
            // 
            // btn_segment_enable_cv
            // 
            this.btn_segment_enable_cv.Location = new System.Drawing.Point(9, 41);
            this.btn_segment_enable_cv.Name = "btn_segment_enable_cv";
            this.btn_segment_enable_cv.Size = new System.Drawing.Size(75, 23);
            this.btn_segment_enable_cv.TabIndex = 5;
            this.btn_segment_enable_cv.Text = "Enable(CV)";
            this.btn_segment_enable_cv.UseVisualStyleBackColor = true;
            this.btn_segment_enable_cv.Click += new System.EventHandler(this.btn_segment_enable_cv_Click);
            // 
            // btn_disable_vh
            // 
            this.btn_disable_vh.Location = new System.Drawing.Point(90, 41);
            this.btn_disable_vh.Name = "btn_disable_vh";
            this.btn_disable_vh.Size = new System.Drawing.Size(75, 23);
            this.btn_disable_vh.TabIndex = 4;
            this.btn_disable_vh.Text = "Disable(Vh)";
            this.btn_disable_vh.UseVisualStyleBackColor = true;
            this.btn_disable_vh.Visible = false;
            this.btn_disable_vh.Click += new System.EventHandler(this.btn_disable_vh_Click);
            // 
            // RoadControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1173, 731);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RoadControlForm";
            this.Text = "RoadControlForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RoadControlForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RoadControlForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_segment)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.pel_button.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btn_disable;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.Button btn_enable;
        private System.Windows.Forms.DataGridView dgv_segment;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel pel_button;
        private System.Windows.Forms.Button btn_disable_vh;
        private System.Windows.Forms.DataGridViewTextBoxColumn seg_num;
        private System.Windows.Forms.DataGridViewTextBoxColumn status;
        private System.Windows.Forms.DataGridViewTextBoxColumn pre_disable_flag;
        private System.Windows.Forms.DataGridViewTextBoxColumn pre_disable_time;
        private System.Windows.Forms.DataGridViewTextBoxColumn disable_time;
        private System.Windows.Forms.DataGridViewCheckBoxColumn user_disable_flag;
        private System.Windows.Forms.DataGridViewCheckBoxColumn safety_disable_flag;
        private System.Windows.Forms.DataGridViewCheckBoxColumn hid_disable_flag;
        private System.Windows.Forms.DataGridViewCheckBoxColumn system_disable_flag;
        private System.Windows.Forms.Button btn_segment_enable_cv;
    }
}