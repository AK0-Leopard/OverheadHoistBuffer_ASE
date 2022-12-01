namespace AlarmReportMaker
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BT_output = new System.Windows.Forms.Button();
            this.BT_save = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.TB_ReportDataLength = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TB_reportMakeInterval = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgv_DataSource = new System.Windows.Forms.DataGridView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dgv_dataView = new System.Windows.Forms.DataGridView();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_DataSource)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_dataView)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BT_output);
            this.groupBox1.Controls.Add(this.BT_save);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.TB_ReportDataLength);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.TB_reportMakeInterval);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(421, 164);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "報表自動產出設定";
            // 
            // BT_output
            // 
            this.BT_output.Location = new System.Drawing.Point(260, 119);
            this.BT_output.Name = "BT_output";
            this.BT_output.Size = new System.Drawing.Size(155, 33);
            this.BT_output.TabIndex = 13;
            this.BT_output.Text = "手動產生報表";
            this.BT_output.UseVisualStyleBackColor = true;
            this.BT_output.Click += new System.EventHandler(this.BT_output_Click);
            // 
            // BT_save
            // 
            this.BT_save.Location = new System.Drawing.Point(294, 80);
            this.BT_save.Name = "BT_save";
            this.BT_save.Size = new System.Drawing.Size(121, 33);
            this.BT_save.TabIndex = 12;
            this.BT_save.Text = "儲存設定";
            this.BT_save.UseVisualStyleBackColor = true;
            this.BT_save.Visible = false;
            this.BT_save.Click += new System.EventHandler(this.BT_save_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Enabled = false;
            this.label3.Location = new System.Drawing.Point(206, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 22);
            this.label3.TabIndex = 5;
            this.label3.Text = "天";
            // 
            // TB_ReportDataLength
            // 
            this.TB_ReportDataLength.Enabled = false;
            this.TB_ReportDataLength.Location = new System.Drawing.Point(160, 80);
            this.TB_ReportDataLength.Name = "TB_ReportDataLength";
            this.TB_ReportDataLength.Size = new System.Drawing.Size(40, 33);
            this.TB_ReportDataLength.TabIndex = 4;
            this.TB_ReportDataLength.Text = "3";
            this.TB_ReportDataLength.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TB_ReportDataLength_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Enabled = false;
            this.label4.Location = new System.Drawing.Point(50, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 22);
            this.label4.TabIndex = 3;
            this.label4.Text = "報表區間:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(206, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 22);
            this.label2.TabIndex = 2;
            this.label2.Text = "點";
            // 
            // TB_reportMakeInterval
            // 
            this.TB_reportMakeInterval.Enabled = false;
            this.TB_reportMakeInterval.Location = new System.Drawing.Point(160, 41);
            this.TB_reportMakeInterval.Name = "TB_reportMakeInterval";
            this.TB_reportMakeInterval.Size = new System.Drawing.Size(40, 33);
            this.TB_reportMakeInterval.TabIndex = 1;
            this.TB_reportMakeInterval.Text = "24";
            this.TB_reportMakeInterval.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TB_reportMakeInterval_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(6, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 22);
            this.label1.TabIndex = 0;
            this.label1.Text = "幾點產出報表:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgv_DataSource);
            this.groupBox2.Location = new System.Drawing.Point(439, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(718, 164);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "資料目標(點擊可預覽報表資訊)";
            // 
            // dgv_DataSource
            // 
            this.dgv_DataSource.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_DataSource.Location = new System.Drawing.Point(6, 32);
            this.dgv_DataSource.Name = "dgv_DataSource";
            this.dgv_DataSource.ReadOnly = true;
            this.dgv_DataSource.RowTemplate.Height = 24;
            this.dgv_DataSource.Size = new System.Drawing.Size(706, 120);
            this.dgv_DataSource.TabIndex = 0;
            this.dgv_DataSource.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dgv_DataSource_MouseDoubleClick);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.dgv_dataView);
            this.groupBox3.Location = new System.Drawing.Point(12, 182);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1145, 444);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "資料預覽";
            // 
            // dgv_dataView
            // 
            this.dgv_dataView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_dataView.Location = new System.Drawing.Point(10, 32);
            this.dgv_dataView.Name = "dgv_dataView";
            this.dgv_dataView.RowTemplate.Height = 24;
            this.dgv_dataView.Size = new System.Drawing.Size(1129, 406);
            this.dgv_dataView.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1168, 639);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("新細明體", 16F);
            this.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Name = "Form1";
            this.Text = "異常處理售後服務報表自動產出系統";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_DataSource)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_dataView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TB_ReportDataLength;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TB_reportMakeInterval;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridView dgv_DataSource;
        private System.Windows.Forms.DataGridView dgv_dataView;
        private System.Windows.Forms.Button BT_save;
        private System.Windows.Forms.Button BT_output;
    }
}

