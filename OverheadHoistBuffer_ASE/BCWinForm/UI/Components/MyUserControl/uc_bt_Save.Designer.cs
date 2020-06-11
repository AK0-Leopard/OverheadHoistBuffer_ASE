namespace com.mirle.ibg3k0.bc.winform.UI.Components.MyUserControl
{
    partial class uc_bt_Save
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
            this.btn_Button = new RoundPanel.RoundButton_AllRound();
            this.roundPanel_AllRound1 = new RoundPanel.RoundPanel_AllRound();
            this.roundPanel_AllRound1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Button
            // 
            this.btn_Button.BackColor = System.Drawing.Color.Transparent;
            this.btn_Button.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Button.EnterBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(106)))), ((int)(((byte)(111)))), ((int)(((byte)(123)))));
            this.btn_Button.EnterForeColor = System.Drawing.Color.White;
            this.btn_Button.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btn_Button.FlatAppearance.BorderSize = 0;
            this.btn_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Button.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_Button.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(39)))), ((int)(((byte)(58)))));
            this.btn_Button.HoverForeColor = System.Drawing.Color.White;
            this.btn_Button.Location = new System.Drawing.Point(2, 2);
            this.btn_Button.Margin = new System.Windows.Forms.Padding(0);
            this.btn_Button.Name = "btn_Button";
            this.btn_Button.PressBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(106)))), ((int)(((byte)(111)))), ((int)(((byte)(123)))));
            this.btn_Button.PressForeColor = System.Drawing.Color.White;
            this.btn_Button.Radius = 35;
            this.btn_Button.Size = new System.Drawing.Size(120, 35);
            this.btn_Button.TabIndex = 2;
            this.btn_Button.Text = "Save";
            this.btn_Button.UseVisualStyleBackColor = false;
            // 
            // roundPanel_AllRound1
            // 
            this.roundPanel_AllRound1.Back = System.Drawing.Color.Gray;
            this.roundPanel_AllRound1.BackColor = System.Drawing.Color.Transparent;
            this.roundPanel_AllRound1.Controls.Add(this.btn_Button);
            this.roundPanel_AllRound1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roundPanel_AllRound1.Location = new System.Drawing.Point(0, 0);
            this.roundPanel_AllRound1.Margin = new System.Windows.Forms.Padding(0);
            this.roundPanel_AllRound1.MatrixRound = 40;
            this.roundPanel_AllRound1.Name = "roundPanel_AllRound1";
            this.roundPanel_AllRound1.Padding = new System.Windows.Forms.Padding(1);
            this.roundPanel_AllRound1.Size = new System.Drawing.Size(122, 39);
            this.roundPanel_AllRound1.TabIndex = 3;
            // 
            // uc_bt_Save
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.roundPanel_AllRound1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "uc_bt_Save";
            this.Size = new System.Drawing.Size(122, 39);
            this.roundPanel_AllRound1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private RoundPanel.RoundButton_AllRound btn_Button;
        private RoundPanel.RoundPanel_AllRound roundPanel_AllRound1;
    }
}
