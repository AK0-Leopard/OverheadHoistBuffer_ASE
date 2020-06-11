namespace com.mirle.ibg3k0.bc.winform.UI
{
    partial class VehicleDataSettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VehicleDataSettingForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.labEx_UpdateTime_Name = new com.mirle.ibg3k0.bc.winform.UI.Components.MyUserControl.LabelExt();
            this.labEx_UpdateUser_Name = new com.mirle.ibg3k0.bc.winform.UI.Components.MyUserControl.LabelExt();
            this.lab_Time_Value = new System.Windows.Forms.Label();
            this.lab_UpdateUser_Value = new System.Windows.Forms.Label();
            this.tabControlEx11 = new WindowsFormsApplication8.TabControlEx1();
            this.Travel_Base_Data = new System.Windows.Forms.TabPage();
            this.uc_TravelBaseData1 = new com.mirle.ibg3k0.bc.winform.UI.Components.MyUserControl.uc_TravelBaseData();
            this.Guide_Data = new System.Windows.Forms.TabPage();
            this.uc_GuideData1 = new com.mirle.ibg3k0.bc.winform.UI.Components.MyUserControl.uc_GuideData();
            this.Address_Data = new System.Windows.Forms.TabPage();
            this.uc_AddressData1 = new com.mirle.ibg3k0.bc.winform.UI.Components.MyUserControl.uc_AddressData();
            this.Section_Data = new System.Windows.Forms.TabPage();
            this.uc_SectionData1 = new com.mirle.ibg3k0.bc.winform.UI.Components.MyUserControl.uc_SectionData();
            this.Scale_Data = new System.Windows.Forms.TabPage();
            this.uc_ScaleData1 = new com.mirle.ibg3k0.bc.winform.UI.Components.MyUserControl.uc_ScaleData();
            this.Control_Data = new System.Windows.Forms.TabPage();
            this.lab_Close = new System.Windows.Forms.Label();
            this.uc_ControlData1 = new com.mirle.ibg3k0.bc.winform.UI.Components.MyUserControl.uc_ControlData();
            this.uc_ControlData2 = new com.mirle.ibg3k0.bc.winform.UI.Components.MyUserControl.uc_ControlData();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tabControlEx11.SuspendLayout();
            this.Travel_Base_Data.SuspendLayout();
            this.Guide_Data.SuspendLayout();
            this.Address_Data.SuspendLayout();
            this.Section_Data.SuspendLayout();
            this.Scale_Data.SuspendLayout();
            this.Control_Data.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tabControlEx11, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lab_Close, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(61)))), ((int)(((byte)(70)))));
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.labEx_UpdateTime_Name, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.labEx_UpdateUser_Name, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.lab_Time_Value, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.lab_UpdateUser_Value, 5, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel2_Paint);
            // 
            // labEx_UpdateTime_Name
            // 
            this.labEx_UpdateTime_Name.DisplayName = "Update_Time";
            resources.ApplyResources(this.labEx_UpdateTime_Name, "labEx_UpdateTime_Name");
            this.labEx_UpdateTime_Name.ForeColor = System.Drawing.Color.White;
            this.labEx_UpdateTime_Name.Name = "labEx_UpdateTime_Name";
            // 
            // labEx_UpdateUser_Name
            // 
            resources.ApplyResources(this.labEx_UpdateUser_Name, "labEx_UpdateUser_Name");
            this.labEx_UpdateUser_Name.DisplayName = "Update_User";
            this.labEx_UpdateUser_Name.ForeColor = System.Drawing.Color.White;
            this.labEx_UpdateUser_Name.Name = "labEx_UpdateUser_Name";
            // 
            // lab_Time_Value
            // 
            resources.ApplyResources(this.lab_Time_Value, "lab_Time_Value");
            this.lab_Time_Value.ForeColor = System.Drawing.Color.White;
            this.lab_Time_Value.Name = "lab_Time_Value";
            // 
            // lab_UpdateUser_Value
            // 
            resources.ApplyResources(this.lab_UpdateUser_Value, "lab_UpdateUser_Value");
            this.lab_UpdateUser_Value.ForeColor = System.Drawing.Color.White;
            this.lab_UpdateUser_Value.Name = "lab_UpdateUser_Value";
            // 
            // tabControlEx11
            // 
            resources.ApplyResources(this.tabControlEx11, "tabControlEx11");
            this.tabControlEx11.ControlBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(40)))), ((int)(((byte)(58)))));
            this.tabControlEx11.Controls.Add(this.Travel_Base_Data);
            this.tabControlEx11.Controls.Add(this.Guide_Data);
            this.tabControlEx11.Controls.Add(this.Address_Data);
            this.tabControlEx11.Controls.Add(this.Section_Data);
            this.tabControlEx11.Controls.Add(this.Scale_Data);
            this.tabControlEx11.Controls.Add(this.Control_Data);
            this.tabControlEx11.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControlEx11.Multiline = true;
            this.tabControlEx11.Name = "tabControlEx11";
            this.tabControlEx11.SelectedIndex = 0;
            this.tabControlEx11.SelectedTabColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(132)))), ((int)(((byte)(246)))));
            this.tabControlEx11.SelectedTabTextColor = System.Drawing.Color.White;
            this.tabControlEx11.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControlEx11.TabTextColor = System.Drawing.Color.White;
            this.tabControlEx11.UnselectedTabColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(40)))), ((int)(((byte)(58)))));
            // 
            // Travel_Base_Data
            // 
            this.Travel_Base_Data.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(39)))), ((int)(((byte)(58)))));
            this.Travel_Base_Data.Controls.Add(this.uc_TravelBaseData1);
            resources.ApplyResources(this.Travel_Base_Data, "Travel_Base_Data");
            this.Travel_Base_Data.Name = "Travel_Base_Data";
            // 
            // uc_TravelBaseData1
            // 
            this.uc_TravelBaseData1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(39)))), ((int)(((byte)(58)))));
            resources.ApplyResources(this.uc_TravelBaseData1, "uc_TravelBaseData1");
            this.uc_TravelBaseData1.Name = "uc_TravelBaseData1";
            // 
            // Guide_Data
            // 
            this.Guide_Data.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(39)))), ((int)(((byte)(58)))));
            this.Guide_Data.Controls.Add(this.uc_GuideData1);
            resources.ApplyResources(this.Guide_Data, "Guide_Data");
            this.Guide_Data.Name = "Guide_Data";
            // 
            // uc_GuideData1
            // 
            this.uc_GuideData1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(39)))), ((int)(((byte)(58)))));
            resources.ApplyResources(this.uc_GuideData1, "uc_GuideData1");
            this.uc_GuideData1.Name = "uc_GuideData1";
            // 
            // Address_Data
            // 
            this.Address_Data.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(39)))), ((int)(((byte)(58)))));
            this.Address_Data.Controls.Add(this.uc_AddressData1);
            resources.ApplyResources(this.Address_Data, "Address_Data");
            this.Address_Data.Name = "Address_Data";
            // 
            // uc_AddressData1
            // 
            this.uc_AddressData1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(39)))), ((int)(((byte)(58)))));
            resources.ApplyResources(this.uc_AddressData1, "uc_AddressData1");
            this.uc_AddressData1.Name = "uc_AddressData1";
            // 
            // Section_Data
            // 
            this.Section_Data.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(39)))), ((int)(((byte)(58)))));
            this.Section_Data.Controls.Add(this.uc_SectionData1);
            resources.ApplyResources(this.Section_Data, "Section_Data");
            this.Section_Data.Name = "Section_Data";
            // 
            // uc_SectionData1
            // 
            this.uc_SectionData1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(39)))), ((int)(((byte)(58)))));
            resources.ApplyResources(this.uc_SectionData1, "uc_SectionData1");
            this.uc_SectionData1.Name = "uc_SectionData1";
            // 
            // Scale_Data
            // 
            this.Scale_Data.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(39)))), ((int)(((byte)(58)))));
            this.Scale_Data.Controls.Add(this.uc_ScaleData1);
            resources.ApplyResources(this.Scale_Data, "Scale_Data");
            this.Scale_Data.Name = "Scale_Data";
            // 
            // uc_ScaleData1
            // 
            this.uc_ScaleData1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(39)))), ((int)(((byte)(58)))));
            resources.ApplyResources(this.uc_ScaleData1, "uc_ScaleData1");
            this.uc_ScaleData1.Name = "uc_ScaleData1";
            // 
            // Control_Data
            // 
            this.Control_Data.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(39)))), ((int)(((byte)(58)))));
            this.Control_Data.Controls.Add(this.uc_ControlData2);
            this.Control_Data.Controls.Add(this.uc_ControlData1);
            resources.ApplyResources(this.Control_Data, "Control_Data");
            this.Control_Data.Name = "Control_Data";
            // 
            // lab_Close
            // 
            resources.ApplyResources(this.lab_Close, "lab_Close");
            this.lab_Close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lab_Close.ForeColor = System.Drawing.Color.White;
            this.lab_Close.Name = "lab_Close";
            this.lab_Close.Click += new System.EventHandler(this.lab_Close_Click);
            // 
            // uc_ControlData1
            // 
            this.uc_ControlData1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(39)))), ((int)(((byte)(58)))));
            resources.ApplyResources(this.uc_ControlData1, "uc_ControlData1");
            this.uc_ControlData1.Name = "uc_ControlData1";
            // 
            // uc_ControlData2
            // 
            this.uc_ControlData2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(39)))), ((int)(((byte)(58)))));
            resources.ApplyResources(this.uc_ControlData2, "uc_ControlData2");
            this.uc_ControlData2.Name = "uc_ControlData2";
            // 
            // VehicleDataSettingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(39)))), ((int)(((byte)(58)))));
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "VehicleDataSettingForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SectionDataEditForm_FormClosed);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tabControlEx11.ResumeLayout(false);
            this.Travel_Base_Data.ResumeLayout(false);
            this.Guide_Data.ResumeLayout(false);
            this.Address_Data.ResumeLayout(false);
            this.Section_Data.ResumeLayout(false);
            this.Scale_Data.ResumeLayout(false);
            this.Control_Data.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private Components.MyUserControl.LabelExt labEx_UpdateTime_Name;
        private WindowsFormsApplication8.TabControlEx1 tabControlEx11;
        private System.Windows.Forms.TabPage Travel_Base_Data;
        private System.Windows.Forms.TabPage Guide_Data;
        private System.Windows.Forms.TabPage Address_Data;
        private System.Windows.Forms.TabPage Section_Data;
        private System.Windows.Forms.TabPage Scale_Data;
        private System.Windows.Forms.TabPage Control_Data;
        private System.Windows.Forms.Label lab_Close;
        private Components.MyUserControl.LabelExt labEx_UpdateUser_Name;
        private System.Windows.Forms.Label lab_Time_Value;
        private System.Windows.Forms.Label lab_UpdateUser_Value;
        private Components.MyUserControl.uc_TravelBaseData uc_TravelBaseData1;
        private Components.MyUserControl.uc_GuideData uc_GuideData1;
        private Components.MyUserControl.uc_AddressData uc_AddressData1;
        private Components.MyUserControl.uc_SectionData uc_SectionData1;
        private Components.MyUserControl.uc_ScaleData uc_ScaleData1;
        private Components.MyUserControl.uc_ControlData uc_ControlData2;
        private Components.MyUserControl.uc_ControlData uc_ControlData1;
    }
}