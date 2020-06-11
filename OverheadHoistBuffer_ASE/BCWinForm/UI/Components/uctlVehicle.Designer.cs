namespace com.mirle.ibg3k0.bc.winform.UI.Components
{
	partial class uctlVehicle
	{
		/// <summary> 
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region コンポーネント デザイナーで生成されたコード

		/// <summary> 
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.lblPresence = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tlpState = new System.Windows.Forms.TableLayoutPanel();
            this.procBar_Progress = new System.Windows.Forms.ProgressBar();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tlpState.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblPresence
            // 
            this.lblPresence.BackColor = System.Drawing.Color.White;
            this.lblPresence.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPresence.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPresence.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPresence.ForeColor = System.Drawing.Color.Black;
            this.lblPresence.Location = new System.Drawing.Point(5, 3);
            this.lblPresence.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.lblPresence.Name = "lblPresence";
            this.lblPresence.Size = new System.Drawing.Size(24, 20);
            this.lblPresence.TabIndex = 68;
            this.lblPresence.Text = "12";
            this.lblPresence.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblPresence.Click += new System.EventHandler(this.lblPresence_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tlpState, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.procBar_Progress, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(40, 44);
            this.tableLayoutPanel1.TabIndex = 70;
            // 
            // tlpState
            // 
            this.tlpState.BackColor = System.Drawing.Color.Silver;
            this.tlpState.ColumnCount = 1;
            this.tlpState.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpState.Controls.Add(this.lblPresence, 0, 0);
            this.tlpState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpState.Location = new System.Drawing.Point(3, 3);
            this.tlpState.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.tlpState.Name = "tlpState";
            this.tlpState.RowCount = 1;
            this.tlpState.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpState.Size = new System.Drawing.Size(34, 26);
            this.tlpState.TabIndex = 0;
            this.tlpState.Click += new System.EventHandler(this.tlpState_Click);
            // 
            // procBar_Progress
            // 
            this.procBar_Progress.BackColor = System.Drawing.Color.Gray;
            this.procBar_Progress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.procBar_Progress.Location = new System.Drawing.Point(3, 32);
            this.procBar_Progress.Name = "procBar_Progress";
            this.procBar_Progress.Size = new System.Drawing.Size(34, 9);
            this.procBar_Progress.TabIndex = 1;
            this.procBar_Progress.Click += new System.EventHandler(this.procBar_Progress_Click);
            // 
            // ToolTip
            // 
            this.ToolTip.Popup += new System.Windows.Forms.PopupEventHandler(this.ToolTip_Popup);
            // 
            // uctlVehicle
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Gray;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(25, 25);
            this.Name = "uctlVehicle";
            this.Size = new System.Drawing.Size(40, 44);
            this.FontChanged += new System.EventHandler(this.uctlVehicle_FontChanged);
            this.Resize += new System.EventHandler(this.uctlVehicle_Resize);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tlpState.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Label lblPresence;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tlpState;
        private System.Windows.Forms.ProgressBar procBar_Progress;
        private System.Windows.Forms.ToolTip ToolTip;




    }
}
