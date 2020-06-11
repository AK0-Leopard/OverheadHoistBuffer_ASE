namespace com.mirle.ibg3k0.bc.winform.UI.Components
{
    partial class uctlPortNew
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
            this.lblPort = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblPort
            // 
            this.lblPort.BackColor = System.Drawing.Color.Transparent;
            this.lblPort.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPort.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPort.ForeColor = System.Drawing.Color.Black;
            this.lblPort.Location = new System.Drawing.Point(0, 0);
            this.lblPort.Margin = new System.Windows.Forms.Padding(1);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(50, 30);
            this.lblPort.TabIndex = 0;
            this.lblPort.Text = "OFST14-1";
            this.lblPort.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uctlPortNew
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(this.lblPort);
            this.Font = new System.Drawing.Font("Verdana", 11.25F);
            this.ForeColor = System.Drawing.Color.Black;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "uctlPortNew";
            this.Size = new System.Drawing.Size(50, 30);
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Label lblPort;



	}
}
