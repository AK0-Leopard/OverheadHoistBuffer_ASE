namespace com.mirle.ibg3k0.bc.winform.UI
{
    partial class BOperationForm01
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

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BOperationForm01));
            this.pnlData = new System.Windows.Forms.Panel();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.lblProdCaption = new System.Windows.Forms.Label();
            this.pnlButton = new System.Windows.Forms.Panel();
            this.butCreate = new System.Windows.Forms.Button();
            this.butSave = new System.Windows.Forms.Button();
            this.butPause = new System.Windows.Forms.Button();
            this.butStart = new System.Windows.Forms.Button();
            this.butStop = new System.Windows.Forms.Button();
            this.butRefresh = new System.Windows.Forms.Button();
            this.butReturn = new System.Windows.Forms.Button();
            this.pnlButton.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlData
            // 
            this.pnlData.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.pnlData, "pnlData");
            this.pnlData.Name = "pnlData";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Create");
            this.imageList1.Images.SetKeyName(1, "Close");
            this.imageList1.Images.SetKeyName(2, "Clear");
            this.imageList1.Images.SetKeyName(3, "Start");
            this.imageList1.Images.SetKeyName(4, "Stop");
            this.imageList1.Images.SetKeyName(5, "Save");
            this.imageList1.Images.SetKeyName(6, "Pause");
            this.imageList1.Images.SetKeyName(7, "1448701926_Exit.ico");
            // 
            // lblProdCaption
            // 
            this.lblProdCaption.BackColor = System.Drawing.Color.Orange;
            this.lblProdCaption.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.lblProdCaption, "lblProdCaption");
            this.lblProdCaption.ForeColor = System.Drawing.Color.MediumBlue;
            this.lblProdCaption.Name = "lblProdCaption";
            // 
            // pnlButton
            // 
            this.pnlButton.Controls.Add(this.butCreate);
            this.pnlButton.Controls.Add(this.butSave);
            this.pnlButton.Controls.Add(this.butPause);
            this.pnlButton.Controls.Add(this.butStart);
            this.pnlButton.Controls.Add(this.butStop);
            this.pnlButton.Controls.Add(this.butRefresh);
            this.pnlButton.Controls.Add(this.butReturn);
            resources.ApplyResources(this.pnlButton, "pnlButton");
            this.pnlButton.Name = "pnlButton";
            // 
            // butCreate
            // 
            resources.ApplyResources(this.butCreate, "butCreate");
            this.butCreate.ImageList = this.imageList1;
            this.butCreate.Name = "butCreate";
            this.butCreate.UseVisualStyleBackColor = true;
            this.butCreate.Click += new System.EventHandler(this.butCreate_Click);
            // 
            // butSave
            // 
            resources.ApplyResources(this.butSave, "butSave");
            this.butSave.ImageList = this.imageList1;
            this.butSave.Name = "butSave";
            this.butSave.UseVisualStyleBackColor = true;
            this.butSave.Click += new System.EventHandler(this.butSave_Click);
            // 
            // butPause
            // 
            resources.ApplyResources(this.butPause, "butPause");
            this.butPause.ImageList = this.imageList1;
            this.butPause.Name = "butPause";
            this.butPause.UseVisualStyleBackColor = true;
            this.butPause.Click += new System.EventHandler(this.butPause_Click);
            // 
            // butStart
            // 
            resources.ApplyResources(this.butStart, "butStart");
            this.butStart.ImageList = this.imageList1;
            this.butStart.Name = "butStart";
            this.butStart.UseVisualStyleBackColor = true;
            this.butStart.Click += new System.EventHandler(this.butStart_Click);
            // 
            // butStop
            // 
            resources.ApplyResources(this.butStop, "butStop");
            this.butStop.ImageList = this.imageList1;
            this.butStop.Name = "butStop";
            this.butStop.UseVisualStyleBackColor = true;
            this.butStop.Click += new System.EventHandler(this.butStop_Click);
            // 
            // butRefresh
            // 
            resources.ApplyResources(this.butRefresh, "butRefresh");
            this.butRefresh.ImageList = this.imageList1;
            this.butRefresh.Name = "butRefresh";
            this.butRefresh.UseVisualStyleBackColor = true;
            this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
            // 
            // butReturn
            // 
            this.butReturn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.butReturn, "butReturn");
            this.butReturn.ImageList = this.imageList1;
            this.butReturn.Name = "butReturn";
            this.butReturn.UseVisualStyleBackColor = true;
            this.butReturn.Click += new System.EventHandler(this.butReturn_Click);
            // 
            // BOperationForm01
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.butReturn;
            this.Controls.Add(this.pnlData);
            this.Controls.Add(this.lblProdCaption);
            this.Controls.Add(this.pnlButton);
            this.KeyPreview = true;
            this.Name = "BOperationForm01";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BOperationForm01_FormClosed);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.BOperationForm01_KeyPress);
            this.pnlButton.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel pnlData;
        private System.Windows.Forms.Button butReturn;
        public System.Windows.Forms.Label lblProdCaption;
        public System.Windows.Forms.Button butStart;
        public System.Windows.Forms.Button butSave;
        public System.Windows.Forms.Button butStop;
        internal System.Windows.Forms.Panel pnlButton;
        private System.Windows.Forms.ImageList imageList1;
        public System.Windows.Forms.Button butPause;
        public System.Windows.Forms.Button butRefresh;
        public System.Windows.Forms.Button butCreate;

    }
}

