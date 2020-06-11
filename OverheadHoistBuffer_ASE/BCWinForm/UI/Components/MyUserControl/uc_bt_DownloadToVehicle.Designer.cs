namespace com.mirle.ibg3k0.bc.winform.UI.Components.MyUserControl
{
    partial class uc_bt_DownloadToVehicle
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
            this.roundButton_AllRound1 = new RoundPanel.RoundButton_AllRound();
            this.roundPanel_AllRound1 = new RoundPanel.RoundPanel_AllRound();
            this.roundPanel_AllRound1.SuspendLayout();
            this.SuspendLayout();
            // 
            // roundButton_AllRound1
            // 
            this.roundButton_AllRound1.BackColor = System.Drawing.Color.Transparent;
            this.roundButton_AllRound1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.roundButton_AllRound1.EnterBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(144)))), ((int)(((byte)(165)))));
            this.roundButton_AllRound1.EnterForeColor = System.Drawing.Color.White;
            this.roundButton_AllRound1.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.roundButton_AllRound1.FlatAppearance.BorderSize = 0;
            this.roundButton_AllRound1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.roundButton_AllRound1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.roundButton_AllRound1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.roundButton_AllRound1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.roundButton_AllRound1.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(89)))), ((int)(((byte)(121)))));
            this.roundButton_AllRound1.HoverForeColor = System.Drawing.Color.White;
            this.roundButton_AllRound1.Location = new System.Drawing.Point(2, 2);
            this.roundButton_AllRound1.Margin = new System.Windows.Forms.Padding(0);
            this.roundButton_AllRound1.Name = "roundButton_AllRound1";
            this.roundButton_AllRound1.PressBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(144)))), ((int)(((byte)(165)))));
            this.roundButton_AllRound1.PressForeColor = System.Drawing.Color.White;
            this.roundButton_AllRound1.Radius = 35;
            this.roundButton_AllRound1.Size = new System.Drawing.Size(190, 35);
            this.roundButton_AllRound1.TabIndex = 2;
            this.roundButton_AllRound1.Text = "Download to Vehicle";
            this.roundButton_AllRound1.UseVisualStyleBackColor = false;
            // 
            // roundPanel_AllRound1
            // 
            this.roundPanel_AllRound1.Back = System.Drawing.Color.Gray;
            this.roundPanel_AllRound1.BackColor = System.Drawing.Color.Transparent;
            this.roundPanel_AllRound1.Controls.Add(this.roundButton_AllRound1);
            this.roundPanel_AllRound1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roundPanel_AllRound1.Location = new System.Drawing.Point(0, 0);
            this.roundPanel_AllRound1.Margin = new System.Windows.Forms.Padding(0);
            this.roundPanel_AllRound1.MatrixRound = 40;
            this.roundPanel_AllRound1.Name = "roundPanel_AllRound1";
            this.roundPanel_AllRound1.Padding = new System.Windows.Forms.Padding(1);
            this.roundPanel_AllRound1.Size = new System.Drawing.Size(192, 39);
            this.roundPanel_AllRound1.TabIndex = 3;
            // 
            // uc_bt_DownloadToVehicle
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.roundPanel_AllRound1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "uc_bt_DownloadToVehicle";
            this.Size = new System.Drawing.Size(192, 39);
            this.roundPanel_AllRound1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private RoundPanel.RoundButton_AllRound roundButton_AllRound1;
        private RoundPanel.RoundPanel_AllRound roundPanel_AllRound1;
    }
}
