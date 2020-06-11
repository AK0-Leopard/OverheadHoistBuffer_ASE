namespace com.mirle.ibg3k0.bc.winform.UI
{
    partial class MonitorForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lbl_commInfo = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.uctlTcpIpAgentStatus1 = new com.mirle.ibg3k0.bc.winform.UI.Components.uctlTcpIpAgentStatus();
            this.uctlCommStatus1 = new com.mirle.ibg3k0.bc.winform.UI.Components.uctlCommStatus();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.lbl_secid = new System.Windows.Forms.Label();
            this.cmb_vh_id = new System.Windows.Forms.ComboBox();
            this.cmb_sec_id = new System.Windows.Forms.ComboBox();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart3 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.chart4 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart4)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lbl_commInfo
            // 
            this.lbl_commInfo.AutoSize = true;
            this.lbl_commInfo.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_commInfo.Location = new System.Drawing.Point(9, 5);
            this.lbl_commInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_commInfo.Name = "lbl_commInfo";
            this.lbl_commInfo.Size = new System.Drawing.Size(225, 19);
            this.lbl_commInfo.TabIndex = 2;
            this.lbl_commInfo.Text = "Device connection status";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(369, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(198, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "OHT connection status";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(13, 9);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1447, 805);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lbl_commInfo);
            this.tabPage1.Controls.Add(this.uctlTcpIpAgentStatus1);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.uctlCommStatus1);
            this.tabPage1.Location = new System.Drawing.Point(4, 28);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage1.Size = new System.Drawing.Size(1439, 773);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Device     ";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // uctlTcpIpAgentStatus1
            // 
            this.uctlTcpIpAgentStatus1.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uctlTcpIpAgentStatus1.Location = new System.Drawing.Point(373, 32);
            this.uctlTcpIpAgentStatus1.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.uctlTcpIpAgentStatus1.Name = "uctlTcpIpAgentStatus1";
            this.uctlTcpIpAgentStatus1.Size = new System.Drawing.Size(1056, 508);
            this.uctlTcpIpAgentStatus1.TabIndex = 1;
            // 
            // uctlCommStatus1
            // 
            this.uctlCommStatus1.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uctlCommStatus1.Location = new System.Drawing.Point(13, 32);
            this.uctlCommStatus1.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.uctlCommStatus1.Name = "uctlCommStatus1";
            this.uctlCommStatus1.Size = new System.Drawing.Size(356, 508);
            this.uctlCommStatus1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.lbl_secid);
            this.tabPage2.Controls.Add(this.cmb_vh_id);
            this.tabPage2.Controls.Add(this.cmb_sec_id);
            this.tabPage2.Controls.Add(this.chart2);
            this.tabPage2.Controls.Add(this.chart3);
            this.tabPage2.Controls.Add(this.chart1);
            this.tabPage2.Location = new System.Drawing.Point(4, 28);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage2.Size = new System.Drawing.Size(1439, 773);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Scattergram     ";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 19);
            this.label2.TabIndex = 2;
            this.label2.Text = "Vehicle ID";
            // 
            // lbl_secid
            // 
            this.lbl_secid.AutoSize = true;
            this.lbl_secid.Location = new System.Drawing.Point(711, 410);
            this.lbl_secid.Name = "lbl_secid";
            this.lbl_secid.Size = new System.Drawing.Size(99, 19);
            this.lbl_secid.TabIndex = 2;
            this.lbl_secid.Text = "Scetion ID";
            // 
            // cmb_vh_id
            // 
            this.cmb_vh_id.FormattingEnabled = true;
            this.cmb_vh_id.Location = new System.Drawing.Point(7, 57);
            this.cmb_vh_id.Name = "cmb_vh_id";
            this.cmb_vh_id.Size = new System.Drawing.Size(121, 27);
            this.cmb_vh_id.TabIndex = 1;
            this.cmb_vh_id.SelectedIndexChanged += new System.EventHandler(this.cmb_vh_id_SelectedIndexChanged);
            // 
            // cmb_sec_id
            // 
            this.cmb_sec_id.FormattingEnabled = true;
            this.cmb_sec_id.Location = new System.Drawing.Point(715, 432);
            this.cmb_sec_id.Name = "cmb_sec_id";
            this.cmb_sec_id.Size = new System.Drawing.Size(121, 27);
            this.cmb_sec_id.TabIndex = 1;
            this.cmb_sec_id.SelectedIndexChanged += new System.EventHandler(this.cmb_sec_id_SelectedIndexChanged);
            // 
            // chart2
            // 
            this.chart2.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
            this.chart2.Location = new System.Drawing.Point(7, 90);
            this.chart2.Name = "chart2";
            this.chart2.Size = new System.Drawing.Size(1408, 300);
            this.chart2.TabIndex = 0;
            this.chart2.Text = "chart1";
            // 
            // chart3
            // 
            this.chart3.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
            this.chart3.Location = new System.Drawing.Point(7, 465);
            this.chart3.Name = "chart3";
            this.chart3.Size = new System.Drawing.Size(700, 300);
            this.chart3.TabIndex = 0;
            this.chart3.Text = "chart1";
            // 
            // chart1
            // 
            this.chart1.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
            this.chart1.Location = new System.Drawing.Point(715, 465);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(700, 300);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.chart4);
            this.tabPage3.Location = new System.Drawing.Point(4, 28);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1439, 773);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // chart4
            // 
            chartArea1.Name = "ChartArea1";
            this.chart4.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart4.Legends.Add(legend1);
            this.chart4.Location = new System.Drawing.Point(6, 6);
            this.chart4.Name = "chart4";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart4.Series.Add(series1);
            this.chart4.Size = new System.Drawing.Size(300, 300);
            this.chart4.TabIndex = 0;
            this.chart4.Text = "chart4";
            // 
            // MonitorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1473, 826);
            this.Controls.Add(this.tabControl1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MonitorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Communication  Monitor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MonitorForm_FormClosed);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Components.uctlCommStatus uctlCommStatus1;
        private System.Windows.Forms.Timer timer1;
        private Components.uctlTcpIpAgentStatus uctlTcpIpAgentStatus1;
        private System.Windows.Forms.Label lbl_commInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Label lbl_secid;
        private System.Windows.Forms.ComboBox cmb_sec_id;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmb_vh_id;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart3;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart4;

    }
}