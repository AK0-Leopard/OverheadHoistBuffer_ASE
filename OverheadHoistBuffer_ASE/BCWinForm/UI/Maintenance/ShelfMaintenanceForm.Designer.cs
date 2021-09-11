namespace com.mirle.ibg3k0.bc.winform.UI
{
    partial class ShelfMaintenanceForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.dgv_shelfData = new System.Windows.Forms.DataGridView();
            this.ShelfID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Enable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ZoneID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ADR_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lbl_zoneID = new System.Windows.Forms.Label();
            this.cmb_zoneID = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_totalShelfCount = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbl_hightLvl = new System.Windows.Forms.Label();
            this.lbl_DisableCount = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btn_enable = new System.Windows.Forms.Button();
            this.btn_disable = new System.Windows.Forms.Button();
            this.table_HightLvlSet = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_setHightLvl = new System.Windows.Forms.Button();
            this.num_hightLvl = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_shelfData)).BeginInit();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.table_HightLvlSet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_hightLvl)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(640, 760);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Shelf";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.54684F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.64038F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 39.90536F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Controls.Add(this.dgv_shelfData, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.lbl_zoneID, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.cmb_zoneID, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 2, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 26);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.35431F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 88.64569F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(634, 731);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // dgv_shelfData
            // 
            this.dgv_shelfData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_shelfData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ShelfID,
            this.Enable,
            this.ZoneID,
            this.ADR_ID,
            this.Column1});
            this.tableLayoutPanel3.SetColumnSpan(this.dgv_shelfData, 3);
            this.dgv_shelfData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_shelfData.Location = new System.Drawing.Point(3, 86);
            this.dgv_shelfData.MultiSelect = false;
            this.dgv_shelfData.Name = "dgv_shelfData";
            this.dgv_shelfData.ReadOnly = true;
            this.dgv_shelfData.RowTemplate.Height = 24;
            this.dgv_shelfData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_shelfData.Size = new System.Drawing.Size(628, 642);
            this.dgv_shelfData.TabIndex = 0;
            this.dgv_shelfData.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dgv_shelfData_RowPrePaint);
            // 
            // ShelfID
            // 
            this.ShelfID.DataPropertyName = "ShelfID";
            this.ShelfID.HeaderText = "ShelfID";
            this.ShelfID.Name = "ShelfID";
            this.ShelfID.ReadOnly = true;
            // 
            // Enable
            // 
            this.Enable.DataPropertyName = "Enable";
            this.Enable.HeaderText = "Enable";
            this.Enable.Name = "Enable";
            this.Enable.ReadOnly = true;
            // 
            // ZoneID
            // 
            this.ZoneID.DataPropertyName = "ZoneID";
            this.ZoneID.FillWeight = 200F;
            this.ZoneID.HeaderText = "Zone ID";
            this.ZoneID.Name = "ZoneID";
            this.ZoneID.ReadOnly = true;
            this.ZoneID.Width = 200;
            // 
            // ADR_ID
            // 
            this.ADR_ID.DataPropertyName = "ADR_ID";
            this.ADR_ID.HeaderText = "ADR_ID";
            this.ADR_ID.Name = "ADR_ID";
            this.ADR_ID.ReadOnly = true;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.HeaderText = "";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // lbl_zoneID
            // 
            this.lbl_zoneID.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_zoneID.AutoSize = true;
            this.lbl_zoneID.Location = new System.Drawing.Point(24, 30);
            this.lbl_zoneID.Name = "lbl_zoneID";
            this.lbl_zoneID.Size = new System.Drawing.Size(90, 22);
            this.lbl_zoneID.TabIndex = 3;
            this.lbl_zoneID.Text = "Zone ID:";
            // 
            // cmb_zoneID
            // 
            this.cmb_zoneID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmb_zoneID.FormattingEnabled = true;
            this.cmb_zoneID.Location = new System.Drawing.Point(120, 31);
            this.cmb_zoneID.Name = "cmb_zoneID";
            this.cmb_zoneID.Size = new System.Drawing.Size(257, 30);
            this.cmb_zoneID.TabIndex = 4;
            this.cmb_zoneID.SelectedIndexChanged += new System.EventHandler(this.cmb_zoneID_SelectedIndexChanged);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47.56097F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52.43903F));
            this.tableLayoutPanel4.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.lbl_totalShelfCount, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.lbl_hightLvl, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.lbl_DisableCount, 1, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(380, 0);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(254, 83);
            this.tableLayoutPanel4.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 22);
            this.label1.TabIndex = 0;
            this.label1.Text = "Total:";
            // 
            // lbl_totalShelfCount
            // 
            this.lbl_totalShelfCount.AutoSize = true;
            this.lbl_totalShelfCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_totalShelfCount.Location = new System.Drawing.Point(123, 0);
            this.lbl_totalShelfCount.Name = "lbl_totalShelfCount";
            this.lbl_totalShelfCount.Size = new System.Drawing.Size(128, 27);
            this.lbl_totalShelfCount.TabIndex = 2;
            this.lbl_totalShelfCount.Text = "             ";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 22);
            this.label2.TabIndex = 1;
            this.label2.Text = "Hight lvl:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 22);
            this.label3.TabIndex = 4;
            this.label3.Text = "Disable:";
            // 
            // lbl_hightLvl
            // 
            this.lbl_hightLvl.AutoSize = true;
            this.lbl_hightLvl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_hightLvl.Location = new System.Drawing.Point(123, 54);
            this.lbl_hightLvl.Name = "lbl_hightLvl";
            this.lbl_hightLvl.Size = new System.Drawing.Size(128, 29);
            this.lbl_hightLvl.TabIndex = 3;
            this.lbl_hightLvl.Text = "            ";
            // 
            // lbl_DisableCount
            // 
            this.lbl_DisableCount.AutoSize = true;
            this.lbl_DisableCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_DisableCount.Location = new System.Drawing.Point(123, 27);
            this.lbl_DisableCount.Name = "lbl_DisableCount";
            this.lbl_DisableCount.Size = new System.Drawing.Size(128, 27);
            this.lbl_DisableCount.TabIndex = 5;
            this.lbl_DisableCount.Text = "             ";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 92.85714F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142857F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(646, 825);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.btn_enable, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btn_disable, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.table_HightLvlSet, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 769);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(640, 53);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // btn_enable
            // 
            this.btn_enable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_enable.Location = new System.Drawing.Point(3, 3);
            this.btn_enable.Name = "btn_enable";
            this.btn_enable.Size = new System.Drawing.Size(94, 47);
            this.btn_enable.TabIndex = 0;
            this.btn_enable.Text = "Enable";
            this.btn_enable.UseVisualStyleBackColor = true;
            this.btn_enable.Click += new System.EventHandler(this.btn_enable_Click);
            // 
            // btn_disable
            // 
            this.btn_disable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_disable.Location = new System.Drawing.Point(103, 3);
            this.btn_disable.Name = "btn_disable";
            this.btn_disable.Size = new System.Drawing.Size(94, 47);
            this.btn_disable.TabIndex = 1;
            this.btn_disable.Text = "Disable";
            this.btn_disable.UseVisualStyleBackColor = true;
            this.btn_disable.Click += new System.EventHandler(this.btn_disable_Click);
            // 
            // table_HightLvlSet
            // 
            this.table_HightLvlSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.table_HightLvlSet.ColumnCount = 3;
            this.table_HightLvlSet.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 305F));
            this.table_HightLvlSet.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 61F));
            this.table_HightLvlSet.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.table_HightLvlSet.Controls.Add(this.label4, 0, 0);
            this.table_HightLvlSet.Controls.Add(this.btn_setHightLvl, 2, 0);
            this.table_HightLvlSet.Controls.Add(this.num_hightLvl, 1, 0);
            this.table_HightLvlSet.Enabled = false;
            this.table_HightLvlSet.Location = new System.Drawing.Point(203, 3);
            this.table_HightLvlSet.Name = "table_HightLvlSet";
            this.table_HightLvlSet.RowCount = 1;
            this.table_HightLvlSet.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.table_HightLvlSet.Size = new System.Drawing.Size(434, 47);
            this.table_HightLvlSet.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(192, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 22);
            this.label4.TabIndex = 2;
            this.label4.Text = "Hight lvl:";
            // 
            // btn_setHightLvl
            // 
            this.btn_setHightLvl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_setHightLvl.Location = new System.Drawing.Point(374, 8);
            this.btn_setHightLvl.Margin = new System.Windows.Forms.Padding(8);
            this.btn_setHightLvl.Name = "btn_setHightLvl";
            this.btn_setHightLvl.Size = new System.Drawing.Size(52, 31);
            this.btn_setHightLvl.TabIndex = 0;
            this.btn_setHightLvl.Text = "Set";
            this.btn_setHightLvl.UseVisualStyleBackColor = true;
            this.btn_setHightLvl.Click += new System.EventHandler(this.button1_Click);
            // 
            // num_hightLvl
            // 
            this.num_hightLvl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.num_hightLvl.Location = new System.Drawing.Point(308, 8);
            this.num_hightLvl.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.num_hightLvl.Name = "num_hightLvl";
            this.num_hightLvl.Size = new System.Drawing.Size(55, 30);
            this.num_hightLvl.TabIndex = 3;
            // 
            // ShelfMaintenanceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 825);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "ShelfMaintenanceForm";
            this.Text = "ShelfMaintenanceForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ShelfMaintenanceForm_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_shelfData)).EndInit();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.table_HightLvlSet.ResumeLayout(false);
            this.table_HightLvlSet.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_hightLvl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btn_enable;
        private System.Windows.Forms.Button btn_disable;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.DataGridView dgv_shelfData;
        private System.Windows.Forms.Label lbl_zoneID;
        private System.Windows.Forms.ComboBox cmb_zoneID;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbl_totalShelfCount;
        private System.Windows.Forms.Label lbl_hightLvl;
        private System.Windows.Forms.DataGridViewTextBoxColumn ShelfID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Enable;
        private System.Windows.Forms.DataGridViewTextBoxColumn ZoneID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ADR_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbl_DisableCount;
        private System.Windows.Forms.TableLayoutPanel table_HightLvlSet;
        private System.Windows.Forms.Button btn_setHightLvl;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown num_hightLvl;
    }
}