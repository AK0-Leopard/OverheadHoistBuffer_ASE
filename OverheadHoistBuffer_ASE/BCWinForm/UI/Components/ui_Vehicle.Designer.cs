namespace com.mirle.ibg3k0.bc.winform.UI.Components
{
    partial class ui_Vehicle
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
            this.lbl_ID_Value = new System.Windows.Forms.Label();
            this.lbl_VHStatus_Name = new System.Windows.Forms.Label();
            this.lbl_VHStatus_Value = new System.Windows.Forms.Label();
            this.lbl_ID_Name = new System.Windows.Forms.Label();
            this.lbl_MsgStatus_Name = new System.Windows.Forms.Label();
            this.lbl_MsgStatus_Vaue = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 43.88489F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 56.11511F));
            this.tableLayoutPanel1.Controls.Add(this.lbl_MsgStatus_Name, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lbl_ID_Value, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbl_VHStatus_Name, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lbl_VHStatus_Value, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lbl_ID_Name, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbl_MsgStatus_Vaue, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(279, 86);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lbl_ID_Value
            // 
            this.lbl_ID_Value.AutoSize = true;
            this.lbl_ID_Value.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_ID_Value.Location = new System.Drawing.Point(126, 1);
            this.lbl_ID_Value.Name = "lbl_ID_Value";
            this.lbl_ID_Value.Size = new System.Drawing.Size(149, 31);
            this.lbl_ID_Value.TabIndex = 3;
            this.lbl_ID_Value.Text = "        ";
            this.lbl_ID_Value.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_VHStatus_Name
            // 
            this.lbl_VHStatus_Name.AutoSize = true;
            this.lbl_VHStatus_Name.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_VHStatus_Name.Location = new System.Drawing.Point(6, 33);
            this.lbl_VHStatus_Name.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbl_VHStatus_Name.Name = "lbl_VHStatus_Name";
            this.lbl_VHStatus_Name.Size = new System.Drawing.Size(111, 31);
            this.lbl_VHStatus_Name.TabIndex = 0;
            this.lbl_VHStatus_Name.Text = "VH State:";
            this.lbl_VHStatus_Name.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbl_VHStatus_Value
            // 
            this.lbl_VHStatus_Value.AutoSize = true;
            this.lbl_VHStatus_Value.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_VHStatus_Value.Location = new System.Drawing.Point(128, 33);
            this.lbl_VHStatus_Value.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbl_VHStatus_Value.Name = "lbl_VHStatus_Value";
            this.lbl_VHStatus_Value.Size = new System.Drawing.Size(145, 31);
            this.lbl_VHStatus_Value.TabIndex = 1;
            this.lbl_VHStatus_Value.Text = "          ";
            this.lbl_VHStatus_Value.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_ID_Name
            // 
            this.lbl_ID_Name.AutoSize = true;
            this.lbl_ID_Name.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_ID_Name.Location = new System.Drawing.Point(4, 1);
            this.lbl_ID_Name.Name = "lbl_ID_Name";
            this.lbl_ID_Name.Size = new System.Drawing.Size(115, 31);
            this.lbl_ID_Name.TabIndex = 2;
            this.lbl_ID_Name.Text = "ID:";
            this.lbl_ID_Name.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbl_MsgStatus_Name
            // 
            this.lbl_MsgStatus_Name.AutoSize = true;
            this.lbl_MsgStatus_Name.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_MsgStatus_Name.Location = new System.Drawing.Point(6, 65);
            this.lbl_MsgStatus_Name.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbl_MsgStatus_Name.Name = "lbl_MsgStatus_Name";
            this.lbl_MsgStatus_Name.Size = new System.Drawing.Size(111, 20);
            this.lbl_MsgStatus_Name.TabIndex = 4;
            this.lbl_MsgStatus_Name.Text = "Msg State:";
            this.lbl_MsgStatus_Name.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbl_MsgStatus_Vaue
            // 
            this.lbl_MsgStatus_Vaue.AutoSize = true;
            this.lbl_MsgStatus_Vaue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_MsgStatus_Vaue.Location = new System.Drawing.Point(128, 65);
            this.lbl_MsgStatus_Vaue.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbl_MsgStatus_Vaue.Name = "lbl_MsgStatus_Vaue";
            this.lbl_MsgStatus_Vaue.Size = new System.Drawing.Size(145, 20);
            this.lbl_MsgStatus_Vaue.TabIndex = 1;
            this.lbl_MsgStatus_Vaue.Text = "          ";
            this.lbl_MsgStatus_Vaue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ui_Vehicle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "ui_Vehicle";
            this.Size = new System.Drawing.Size(279, 86);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lbl_VHStatus_Name;
        private System.Windows.Forms.Label lbl_VHStatus_Value;
        private System.Windows.Forms.Label lbl_ID_Value;
        private System.Windows.Forms.Label lbl_ID_Name;
        private System.Windows.Forms.Label lbl_MsgStatus_Name;
        private System.Windows.Forms.Label lbl_MsgStatus_Vaue;
    }
}
