namespace com.mirle.ibg3k0.bc.winform.UI
{
    partial class EngineerForm
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
            this.lbl_fromAdr = new System.Windows.Forms.Label();
            this.cmb_fromAdr = new System.Windows.Forms.ComboBox();
            this.lbl_toAdr = new System.Windows.Forms.Label();
            this.cmb_toAdr = new System.Windows.Forms.ComboBox();
            this.btn_StartSeg = new System.Windows.Forms.Button();
            this.txt_Route = new System.Windows.Forms.TextBox();
            this.btn_close = new System.Windows.Forms.Button();
            this.txt_open = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.txt_Close = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.btn_StartSec = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmb_startAdr = new System.Windows.Forms.ComboBox();
            this.cmb_fromSection = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_seachSec2Adr = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbl_fromAdr
            // 
            this.lbl_fromAdr.AutoSize = true;
            this.lbl_fromAdr.Location = new System.Drawing.Point(20, 34);
            this.lbl_fromAdr.Name = "lbl_fromAdr";
            this.lbl_fromAdr.Size = new System.Drawing.Size(54, 12);
            this.lbl_fromAdr.TabIndex = 0;
            this.lbl_fromAdr.Text = "Form Adr:";
            // 
            // cmb_fromAdr
            // 
            this.cmb_fromAdr.FormattingEnabled = true;
            this.cmb_fromAdr.Location = new System.Drawing.Point(17, 49);
            this.cmb_fromAdr.Name = "cmb_fromAdr";
            this.cmb_fromAdr.Size = new System.Drawing.Size(121, 20);
            this.cmb_fromAdr.TabIndex = 1;
            // 
            // lbl_toAdr
            // 
            this.lbl_toAdr.AutoSize = true;
            this.lbl_toAdr.Location = new System.Drawing.Point(176, 41);
            this.lbl_toAdr.Name = "lbl_toAdr";
            this.lbl_toAdr.Size = new System.Drawing.Size(42, 12);
            this.lbl_toAdr.TabIndex = 0;
            this.lbl_toAdr.Text = "To Adr:";
            // 
            // cmb_toAdr
            // 
            this.cmb_toAdr.FormattingEnabled = true;
            this.cmb_toAdr.Location = new System.Drawing.Point(178, 56);
            this.cmb_toAdr.Name = "cmb_toAdr";
            this.cmb_toAdr.Size = new System.Drawing.Size(121, 20);
            this.cmb_toAdr.TabIndex = 1;
            // 
            // btn_StartSeg
            // 
            this.btn_StartSeg.Location = new System.Drawing.Point(17, 199);
            this.btn_StartSeg.Name = "btn_StartSeg";
            this.btn_StartSeg.Size = new System.Drawing.Size(96, 30);
            this.btn_StartSeg.TabIndex = 2;
            this.btn_StartSeg.Text = "Search Segment";
            this.btn_StartSeg.UseVisualStyleBackColor = true;
            this.btn_StartSeg.Click += new System.EventHandler(this.btn_Start_Click);
            // 
            // txt_Route
            // 
            this.txt_Route.Location = new System.Drawing.Point(16, 80);
            this.txt_Route.Multiline = true;
            this.txt_Route.Name = "txt_Route";
            this.txt_Route.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txt_Route.Size = new System.Drawing.Size(624, 104);
            this.txt_Route.TabIndex = 3;
            // 
            // btn_close
            // 
            this.btn_close.Location = new System.Drawing.Point(401, 226);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(75, 23);
            this.btn_close.TabIndex = 4;
            this.btn_close.Text = "Close";
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // txt_open
            // 
            this.txt_open.Location = new System.Drawing.Point(459, 12);
            this.txt_open.Name = "txt_open";
            this.txt_open.Size = new System.Drawing.Size(100, 22);
            this.txt_open.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(565, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Open";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txt_Close
            // 
            this.txt_Close.Location = new System.Drawing.Point(459, 54);
            this.txt_Close.Name = "txt_Close";
            this.txt_Close.Size = new System.Drawing.Size(100, 22);
            this.txt_Close.TabIndex = 5;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(565, 52);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Close";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btn_StartSec
            // 
            this.btn_StartSec.Location = new System.Drawing.Point(141, 199);
            this.btn_StartSec.Name = "btn_StartSec";
            this.btn_StartSec.Size = new System.Drawing.Size(98, 30);
            this.btn_StartSec.TabIndex = 2;
            this.btn_StartSec.Text = "Search Section";
            this.btn_StartSec.UseVisualStyleBackColor = true;
            this.btn_StartSec.Click += new System.EventHandler(this.btn_StartSec_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, -3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Start Adr:";
            // 
            // cmb_startAdr
            // 
            this.cmb_startAdr.FormattingEnabled = true;
            this.cmb_startAdr.Location = new System.Drawing.Point(17, 12);
            this.cmb_startAdr.Name = "cmb_startAdr";
            this.cmb_startAdr.Size = new System.Drawing.Size(121, 20);
            this.cmb_startAdr.TabIndex = 1;
            // 
            // cmb_fromSection
            // 
            this.cmb_fromSection.FormattingEnabled = true;
            this.cmb_fromSection.Location = new System.Drawing.Point(178, 18);
            this.cmb_fromSection.Name = "cmb_fromSection";
            this.cmb_fromSection.Size = new System.Drawing.Size(121, 20);
            this.cmb_fromSection.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(176, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "From Section:";
            // 
            // btn_seachSec2Adr
            // 
            this.btn_seachSec2Adr.Location = new System.Drawing.Point(267, 199);
            this.btn_seachSec2Adr.Name = "btn_seachSec2Adr";
            this.btn_seachSec2Adr.Size = new System.Drawing.Size(128, 30);
            this.btn_seachSec2Adr.TabIndex = 2;
            this.btn_seachSec2Adr.Text = "Search Section2Adr";
            this.btn_seachSec2Adr.UseVisualStyleBackColor = true;
            this.btn_seachSec2Adr.Click += new System.EventHandler(this.btn_seachSec2Adr_Click);
            // 
            // EngineerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 261);
            this.Controls.Add(this.cmb_fromSection);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txt_Close);
            this.Controls.Add(this.txt_open);
            this.Controls.Add(this.btn_close);
            this.Controls.Add(this.txt_Route);
            this.Controls.Add(this.btn_seachSec2Adr);
            this.Controls.Add(this.btn_StartSec);
            this.Controls.Add(this.btn_StartSeg);
            this.Controls.Add(this.cmb_toAdr);
            this.Controls.Add(this.cmb_startAdr);
            this.Controls.Add(this.cmb_fromAdr);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbl_toAdr);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbl_fromAdr);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EngineerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmRouteSearchTool";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EngineerForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_fromAdr;
        private System.Windows.Forms.ComboBox cmb_fromAdr;
        private System.Windows.Forms.Label lbl_toAdr;
        private System.Windows.Forms.ComboBox cmb_toAdr;
        private System.Windows.Forms.Button btn_StartSeg;
        private System.Windows.Forms.TextBox txt_Route;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.TextBox txt_open;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txt_Close;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btn_StartSec;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmb_startAdr;
        private System.Windows.Forms.ComboBox cmb_fromSection;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_seachSec2Adr;
    }
}