namespace com.mirle.ibg3k0.bc.winform.UI.Components
{
    partial class uctlCommStatus
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lbl_commLight = new System.Windows.Forms.Label();
            this.lbl_connection = new System.Windows.Forms.Label();
            this.tlp_LightBase = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tlp_LightBase, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(354, 364);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.91931F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37.46398F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.32853F));
            this.tableLayoutPanel2.Controls.Add(this.lbl_commLight, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.lbl_connection, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(348, 44);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // lbl_commLight
            // 
            this.lbl_commLight.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_commLight.AutoSize = true;
            this.lbl_commLight.Location = new System.Drawing.Point(121, 12);
            this.lbl_commLight.Name = "lbl_commLight";
            this.lbl_commLight.Size = new System.Drawing.Size(54, 19);
            this.lbl_commLight.TabIndex = 0;
            this.lbl_commLight.Text = "Alive";
            // 
            // lbl_connection
            // 
            this.lbl_connection.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_connection.AutoSize = true;
            this.lbl_connection.Location = new System.Drawing.Point(217, 12);
            this.lbl_connection.Name = "lbl_connection";
            this.lbl_connection.Size = new System.Drawing.Size(126, 19);
            this.lbl_connection.TabIndex = 0;
            this.lbl_connection.Text = "Communication";
            // 
            // tlp_LightBase
            // 
            this.tlp_LightBase.AutoScroll = true;
            this.tlp_LightBase.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tlp_LightBase.ColumnCount = 3;
            this.tlp_LightBase.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.20749F));
            this.tlp_LightBase.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37.17579F));
            this.tlp_LightBase.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.32853F));
            this.tlp_LightBase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_LightBase.Location = new System.Drawing.Point(3, 53);
            this.tlp_LightBase.Name = "tlp_LightBase";
            this.tlp_LightBase.RowCount = 1;
            this.tlp_LightBase.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 298F));
            this.tlp_LightBase.Size = new System.Drawing.Size(348, 308);
            this.tlp_LightBase.TabIndex = 2;
            // 
            // uctlCommStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "uctlCommStatus";
            this.Size = new System.Drawing.Size(354, 364);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lbl_commLight;
        private System.Windows.Forms.Label lbl_connection;
        private System.Windows.Forms.TableLayoutPanel tlp_LightBase;

    }
}
