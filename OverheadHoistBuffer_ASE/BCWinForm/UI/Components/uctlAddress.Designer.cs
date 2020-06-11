namespace com.mirle.ibg3k0.bc.winform.UI.Components
{
	partial class uctlAddress
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
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.ctxSubMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSelectFrom = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSelectTo = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxSubMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctxSubMenu
            // 
            this.ctxSubMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSelectFrom,
            this.tsmiSelectTo});
            this.ctxSubMenu.Name = "ctxSubMenu";
            this.ctxSubMenu.Size = new System.Drawing.Size(216, 48);
            // 
            // tsmiSelectFrom
            // 
            this.tsmiSelectFrom.Name = "tsmiSelectFrom";
            this.tsmiSelectFrom.Size = new System.Drawing.Size(215, 22);
            this.tsmiSelectFrom.Text = "Copy To Source Port";
            this.tsmiSelectFrom.Click += new System.EventHandler(this.tsmiSelectFrom_Click);
            // 
            // tsmiSelectTo
            // 
            this.tsmiSelectTo.Name = "tsmiSelectTo";
            this.tsmiSelectTo.Size = new System.Drawing.Size(215, 22);
            this.tsmiSelectTo.Text = "Copy To Destination Port";
            this.tsmiSelectTo.Click += new System.EventHandler(this.tsmiSelectTo_Click);
            // 
            // uctlAddress
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Bold);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "uctlAddress";
            this.Size = new System.Drawing.Size(44, 44);
            this.LocationChanged += new System.EventHandler(this.uctlAddress_LocationChanged);
            this.Click += new System.EventHandler(this.uctlAddress_Click);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.uctlAddress_MouseClick);
            this.Resize += new System.EventHandler(this.uctlAddress_Resize);
            this.ctxSubMenu.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolTip ToolTip;
		private System.Windows.Forms.ContextMenuStrip ctxSubMenu;
		private System.Windows.Forms.ToolStripMenuItem tsmiSelectFrom;
		private System.Windows.Forms.ToolStripMenuItem tsmiSelectTo;



	}
}
