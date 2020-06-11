namespace com.mirle.ibg3k0.bc.winform.UI.Components
{
    partial class uctlNewVehicle
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

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pic_VhStatus = new System.Windows.Forms.PictureBox();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pic_VhStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // pic_VhStatus
            // 
            this.pic_VhStatus.BackColor = System.Drawing.Color.Transparent;
            this.pic_VhStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pic_VhStatus.Image = global::com.mirle.ibg3k0.bc.winform.Properties.Resources.Vehicle__Unconnected_;
            this.pic_VhStatus.Location = new System.Drawing.Point(0, 0);
            this.pic_VhStatus.Margin = new System.Windows.Forms.Padding(0);
            this.pic_VhStatus.Name = "pic_VhStatus";
            this.pic_VhStatus.Size = new System.Drawing.Size(133, 91);
            this.pic_VhStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pic_VhStatus.TabIndex = 3;
            this.pic_VhStatus.TabStop = false;
            this.pic_VhStatus.Click += new System.EventHandler(this.pic_VhStatus_Click);
            this.pic_VhStatus.Paint += new System.Windows.Forms.PaintEventHandler(this.pic_VhStatus_Paint);
            // 
            // ToolTip
            // 
            this.ToolTip.Popup += new System.Windows.Forms.PopupEventHandler(this.ToolTip_Popup);
            // 
            // uctlNewVehicle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.MidnightBlue;
            this.Controls.Add(this.pic_VhStatus);
            this.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "uctlNewVehicle";
            this.Size = new System.Drawing.Size(133, 91);
            ((System.ComponentModel.ISupportInitialize)(this.pic_VhStatus)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox pic_VhStatus;
        private System.Windows.Forms.ToolTip ToolTip;
    }
}
