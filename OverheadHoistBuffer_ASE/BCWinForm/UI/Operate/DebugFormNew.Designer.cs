﻿namespace com.mirle.ibg3k0.bc.winform.UI
{
    partial class DebugFormNew
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
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.group_cycleRun = new System.Windows.Forms.GroupBox();
            this.combox_cycle_type = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cb_StartGenAntoCmd = new System.Windows.Forms.CheckBox();
            this.gb_blockControl = new System.Windows.Forms.GroupBox();
            this.cb_FroceReserveReject = new System.Windows.Forms.CheckBox();
            this.cb_FroceReservePass = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.num_section_dis = new System.Windows.Forms.NumericUpDown();
            this.txt_current_sec_id = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbl_listening_status = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.lbl_install_status = new System.Windows.Forms.Label();
            this.label73 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.cb_Abort_Type = new System.Windows.Forms.ComboBox();
            this.lbl_id_37_cmdID_value = new System.Windows.Forms.Label();
            this.lbl_cmdID = new System.Windows.Forms.Label();
            this.button9 = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmb_pauseType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmb_pauseEvent = new System.Windows.Forms.ComboBox();
            this.cmb_tcpipctr_Vehicle = new System.Windows.Forms.ComboBox();
            this.btn_refsh_vh_status = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cb_OperMode = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.btn_changeToAutoRemote = new com.mirle.ibg3k0.bc.winform.UI.Components.uctlButton();
            this.btn_changeToAutoLocal = new com.mirle.ibg3k0.bc.winform.UI.Components.uctlButton();
            this.btn_changeToRemove = new com.mirle.ibg3k0.bc.winform.UI.Components.uctlButton();
            this.btn_changeToInstall = new com.mirle.ibg3k0.bc.winform.UI.Components.uctlButton();
            this.btn_close_tcp_port = new com.mirle.ibg3k0.bc.winform.UI.Components.uctlButton();
            this.btn_open_tcp_port = new com.mirle.ibg3k0.bc.winform.UI.Components.uctlButton();
            this.btn_pause = new com.mirle.ibg3k0.bc.winform.UI.Components.uctlButton();
            this.tabPage1.SuspendLayout();
            this.group_cycleRun.SuspendLayout();
            this.gb_blockControl.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_section_dis)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.Controls.Add(this.group_cycleRun);
            this.tabPage1.Controls.Add(this.gb_blockControl);
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Location = new System.Drawing.Point(4, 31);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1198, 830);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "TcpIp Control";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // group_cycleRun
            // 
            this.group_cycleRun.Controls.Add(this.combox_cycle_type);
            this.group_cycleRun.Controls.Add(this.label4);
            this.group_cycleRun.Controls.Add(this.cb_StartGenAntoCmd);
            this.group_cycleRun.Location = new System.Drawing.Point(790, 98);
            this.group_cycleRun.Name = "group_cycleRun";
            this.group_cycleRun.Size = new System.Drawing.Size(387, 144);
            this.group_cycleRun.TabIndex = 58;
            this.group_cycleRun.TabStop = false;
            this.group_cycleRun.Text = "Cycle Run";
            this.group_cycleRun.Visible = false;
            // 
            // combox_cycle_type
            // 
            this.combox_cycle_type.FormattingEnabled = true;
            this.combox_cycle_type.Location = new System.Drawing.Point(18, 52);
            this.combox_cycle_type.Name = "combox_cycle_type";
            this.combox_cycle_type.Size = new System.Drawing.Size(186, 30);
            this.combox_cycle_type.TabIndex = 27;
            this.combox_cycle_type.SelectedIndexChanged += new System.EventHandler(this.combox_cycle_type_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 27);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 22);
            this.label4.TabIndex = 57;
            this.label4.Text = "Type";
            // 
            // cb_StartGenAntoCmd
            // 
            this.cb_StartGenAntoCmd.AutoSize = true;
            this.cb_StartGenAntoCmd.Location = new System.Drawing.Point(18, 94);
            this.cb_StartGenAntoCmd.Name = "cb_StartGenAntoCmd";
            this.cb_StartGenAntoCmd.Size = new System.Drawing.Size(349, 26);
            this.cb_StartGenAntoCmd.TabIndex = 18;
            this.cb_StartGenAntoCmd.Text = "Start Generates Transfer Command";
            this.cb_StartGenAntoCmd.UseVisualStyleBackColor = true;
            this.cb_StartGenAntoCmd.CheckedChanged += new System.EventHandler(this.cb_StartGenAntoCmd_CheckedChanged);
            // 
            // gb_blockControl
            // 
            this.gb_blockControl.Controls.Add(this.cb_FroceReserveReject);
            this.gb_blockControl.Controls.Add(this.cb_FroceReservePass);
            this.gb_blockControl.Location = new System.Drawing.Point(790, 18);
            this.gb_blockControl.Name = "gb_blockControl";
            this.gb_blockControl.Size = new System.Drawing.Size(257, 73);
            this.gb_blockControl.TabIndex = 23;
            this.gb_blockControl.TabStop = false;
            this.gb_blockControl.Text = "Block Control";
            // 
            // cb_FroceReserveReject
            // 
            this.cb_FroceReserveReject.AutoSize = true;
            this.cb_FroceReserveReject.Location = new System.Drawing.Point(28, 47);
            this.cb_FroceReserveReject.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cb_FroceReserveReject.Name = "cb_FroceReserveReject";
            this.cb_FroceReserveReject.Size = new System.Drawing.Size(229, 26);
            this.cb_FroceReserveReject.TabIndex = 8;
            this.cb_FroceReserveReject.Text = "Force Reserve Reject";
            this.cb_FroceReserveReject.UseVisualStyleBackColor = true;
            this.cb_FroceReserveReject.Visible = false;
            this.cb_FroceReserveReject.CheckedChanged += new System.EventHandler(this.cb_FroceReserveReject_CheckedChanged);
            // 
            // cb_FroceReservePass
            // 
            this.cb_FroceReservePass.AutoSize = true;
            this.cb_FroceReservePass.Location = new System.Drawing.Point(28, 22);
            this.cb_FroceReservePass.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cb_FroceReservePass.Name = "cb_FroceReservePass";
            this.cb_FroceReservePass.Size = new System.Drawing.Size(209, 26);
            this.cb_FroceReservePass.TabIndex = 7;
            this.cb_FroceReservePass.Text = "Force Reserve Pass";
            this.cb_FroceReservePass.UseVisualStyleBackColor = true;
            this.cb_FroceReservePass.CheckedChanged += new System.EventHandler(this.cb_FroceReservePass_CheckedChanged);
            this.cb_FroceReservePass.ChangeUICues += new System.Windows.Forms.UICuesEventHandler(this.cb_FroceReservePass_ChangeUICues);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.groupBox9);
            this.groupBox4.Controls.Add(this.groupBox3);
            this.groupBox4.Controls.Add(this.groupBox1);
            this.groupBox4.Controls.Add(this.lbl_listening_status);
            this.groupBox4.Controls.Add(this.label42);
            this.groupBox4.Controls.Add(this.btn_close_tcp_port);
            this.groupBox4.Controls.Add(this.btn_open_tcp_port);
            this.groupBox4.Controls.Add(this.lbl_install_status);
            this.groupBox4.Controls.Add(this.label73);
            this.groupBox4.Controls.Add(this.groupBox8);
            this.groupBox4.Controls.Add(this.groupBox5);
            this.groupBox4.Controls.Add(this.cmb_tcpipctr_Vehicle);
            this.groupBox4.Controls.Add(this.btn_refsh_vh_status);
            this.groupBox4.Controls.Add(this.button6);
            this.groupBox4.Controls.Add(this.button5);
            this.groupBox4.Controls.Add(this.groupBox2);
            this.groupBox4.Location = new System.Drawing.Point(6, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(744, 806);
            this.groupBox4.TabIndex = 21;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Specify Vehicle Action";
            this.groupBox4.Enter += new System.EventHandler(this.groupBox4_Enter);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.label2);
            this.groupBox9.Controls.Add(this.label1);
            this.groupBox9.Controls.Add(this.num_section_dis);
            this.groupBox9.Controls.Add(this.txt_current_sec_id);
            this.groupBox9.Location = new System.Drawing.Point(364, 648);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(208, 151);
            this.groupBox9.TabIndex = 58;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Position Set Test";
            this.groupBox9.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 90);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 22);
            this.label2.TabIndex = 56;
            this.label2.Text = "Distance";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 26);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 22);
            this.label1.TabIndex = 16;
            this.label1.Text = "Section ID";
            // 
            // num_section_dis
            // 
            this.num_section_dis.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.num_section_dis.Location = new System.Drawing.Point(8, 115);
            this.num_section_dis.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.num_section_dis.Name = "num_section_dis";
            this.num_section_dis.Size = new System.Drawing.Size(172, 30);
            this.num_section_dis.TabIndex = 54;
            this.num_section_dis.ValueChanged += new System.EventHandler(this.num_section_dis_ValueChanged);
            // 
            // txt_current_sec_id
            // 
            this.txt_current_sec_id.Location = new System.Drawing.Point(8, 51);
            this.txt_current_sec_id.Name = "txt_current_sec_id";
            this.txt_current_sec_id.Size = new System.Drawing.Size(172, 30);
            this.txt_current_sec_id.TabIndex = 55;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btn_changeToAutoRemote);
            this.groupBox3.Controls.Add(this.btn_changeToAutoLocal);
            this.groupBox3.Location = new System.Drawing.Point(232, 79);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(215, 149);
            this.groupBox3.TabIndex = 57;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Mode";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_changeToRemove);
            this.groupBox1.Controls.Add(this.btn_changeToInstall);
            this.groupBox1.Location = new System.Drawing.Point(8, 79);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(211, 149);
            this.groupBox1.TabIndex = 56;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Remove/Install";
            // 
            // lbl_listening_status
            // 
            this.lbl_listening_status.AutoSize = true;
            this.lbl_listening_status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_listening_status.Location = new System.Drawing.Point(364, 40);
            this.lbl_listening_status.Name = "lbl_listening_status";
            this.lbl_listening_status.Size = new System.Drawing.Size(122, 24);
            this.lbl_listening_status.TabIndex = 53;
            this.lbl_listening_status.Text = "           ";
            this.lbl_listening_status.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbl_listening_status_MouseDoubleClick);
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(228, 40);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(140, 22);
            this.label42.TabIndex = 52;
            this.label42.Text = "Is Listening:";
            // 
            // lbl_install_status
            // 
            this.lbl_install_status.AutoSize = true;
            this.lbl_install_status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_install_status.Location = new System.Drawing.Point(364, 12);
            this.lbl_install_status.Name = "lbl_install_status";
            this.lbl_install_status.Size = new System.Drawing.Size(122, 24);
            this.lbl_install_status.TabIndex = 49;
            this.lbl_install_status.Text = "           ";
            this.lbl_install_status.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbl_install_status_MouseDoubleClick);
            // 
            // label73
            // 
            this.label73.AutoSize = true;
            this.label73.Location = new System.Drawing.Point(249, 12);
            this.label73.Name = "label73";
            this.label73.Size = new System.Drawing.Size(120, 22);
            this.label73.TabIndex = 48;
            this.label73.Text = "Is Install:";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.cb_Abort_Type);
            this.groupBox8.Controls.Add(this.lbl_id_37_cmdID_value);
            this.groupBox8.Controls.Add(this.lbl_cmdID);
            this.groupBox8.Controls.Add(this.button9);
            this.groupBox8.Location = new System.Drawing.Point(8, 234);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(252, 197);
            this.groupBox8.TabIndex = 25;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Func 37-命令結束";
            // 
            // cb_Abort_Type
            // 
            this.cb_Abort_Type.FormattingEnabled = true;
            this.cb_Abort_Type.Location = new System.Drawing.Point(20, 75);
            this.cb_Abort_Type.Name = "cb_Abort_Type";
            this.cb_Abort_Type.Size = new System.Drawing.Size(213, 30);
            this.cb_Abort_Type.TabIndex = 27;
            // 
            // lbl_id_37_cmdID_value
            // 
            this.lbl_id_37_cmdID_value.AutoSize = true;
            this.lbl_id_37_cmdID_value.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_id_37_cmdID_value.Location = new System.Drawing.Point(22, 42);
            this.lbl_id_37_cmdID_value.Name = "lbl_id_37_cmdID_value";
            this.lbl_id_37_cmdID_value.Size = new System.Drawing.Size(212, 24);
            this.lbl_id_37_cmdID_value.TabIndex = 26;
            this.lbl_id_37_cmdID_value.Text = "                    ";
            // 
            // lbl_cmdID
            // 
            this.lbl_cmdID.AutoSize = true;
            this.lbl_cmdID.Location = new System.Drawing.Point(16, 20);
            this.lbl_cmdID.Name = "lbl_cmdID";
            this.lbl_cmdID.Size = new System.Drawing.Size(70, 22);
            this.lbl_cmdID.TabIndex = 25;
            this.lbl_cmdID.Text = "CMD ID";
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(19, 142);
            this.button9.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(214, 42);
            this.button9.TabIndex = 24;
            this.button9.Text = "Send Func:37";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.cmb_pauseType);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.btn_pause);
            this.groupBox5.Controls.Add(this.cmb_pauseEvent);
            this.groupBox5.Location = new System.Drawing.Point(266, 234);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(226, 197);
            this.groupBox5.TabIndex = 23;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Fun 39-命令暫停";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 75);
            this.label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(110, 22);
            this.label6.TabIndex = 15;
            this.label6.Text = "Pause Type";
            // 
            // cmb_pauseType
            // 
            this.cmb_pauseType.FormattingEnabled = true;
            this.cmb_pauseType.Location = new System.Drawing.Point(12, 100);
            this.cmb_pauseType.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.cmb_pauseType.Name = "cmb_pauseType";
            this.cmb_pauseType.Size = new System.Drawing.Size(199, 30);
            this.cmb_pauseType.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 20);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 22);
            this.label5.TabIndex = 15;
            this.label5.Text = "Event";
            // 
            // cmb_pauseEvent
            // 
            this.cmb_pauseEvent.FormattingEnabled = true;
            this.cmb_pauseEvent.Location = new System.Drawing.Point(12, 42);
            this.cmb_pauseEvent.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.cmb_pauseEvent.Name = "cmb_pauseEvent";
            this.cmb_pauseEvent.Size = new System.Drawing.Size(199, 30);
            this.cmb_pauseEvent.TabIndex = 14;
            // 
            // cmb_tcpipctr_Vehicle
            // 
            this.cmb_tcpipctr_Vehicle.FormattingEnabled = true;
            this.cmb_tcpipctr_Vehicle.Location = new System.Drawing.Point(8, 32);
            this.cmb_tcpipctr_Vehicle.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.cmb_tcpipctr_Vehicle.Name = "cmb_tcpipctr_Vehicle";
            this.cmb_tcpipctr_Vehicle.Size = new System.Drawing.Size(197, 30);
            this.cmb_tcpipctr_Vehicle.TabIndex = 9;
            this.cmb_tcpipctr_Vehicle.SelectedIndexChanged += new System.EventHandler(this.cmb_Vehicle_SelectedIndexChanged);
            // 
            // btn_refsh_vh_status
            // 
            this.btn_refsh_vh_status.Location = new System.Drawing.Point(8, 592);
            this.btn_refsh_vh_status.Name = "btn_refsh_vh_status";
            this.btn_refsh_vh_status.Size = new System.Drawing.Size(252, 42);
            this.btn_refsh_vh_status.TabIndex = 17;
            this.btn_refsh_vh_status.Text = "Refresh Vh Status";
            this.btn_refsh_vh_status.UseVisualStyleBackColor = true;
            this.btn_refsh_vh_status.Click += new System.EventHandler(this.button7_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(8, 530);
            this.button6.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(252, 42);
            this.button6.TabIndex = 16;
            this.button6.Text = "Force finish Cmd";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(8, 453);
            this.button5.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(252, 65);
            this.button5.TabIndex = 16;
            this.button5.Text = "Alarm Reset\r\n(Send Func:91)";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.cb_OperMode);
            this.groupBox2.Controls.Add(this.button3);
            this.groupBox2.Location = new System.Drawing.Point(498, 234);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(220, 149);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Fun 41-狀態變更";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 26);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(150, 22);
            this.label3.TabIndex = 15;
            this.label3.Text = "Operating Mode";
            // 
            // cb_OperMode
            // 
            this.cb_OperMode.FormattingEnabled = true;
            this.cb_OperMode.Location = new System.Drawing.Point(10, 51);
            this.cb_OperMode.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.cb_OperMode.Name = "cb_OperMode";
            this.cb_OperMode.Size = new System.Drawing.Size(199, 30);
            this.cb_OperMode.TabIndex = 14;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(10, 91);
            this.button3.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(138, 42);
            this.button3.TabIndex = 16;
            this.button3.Text = "Send Func:41";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1206, 865);
            this.tabControl1.TabIndex = 23;
            // 
            // btn_changeToAutoRemote
            // 
            this.btn_changeToAutoRemote.Location = new System.Drawing.Point(23, 33);
            this.btn_changeToAutoRemote.Name = "btn_changeToAutoRemote";
            this.btn_changeToAutoRemote.Size = new System.Drawing.Size(166, 45);
            this.btn_changeToAutoRemote.TabIndex = 41;
            this.btn_changeToAutoRemote.Text = "Auto Remote";
            this.btn_changeToAutoRemote.UseVisualStyleBackColor = true;
            this.btn_changeToAutoRemote.Click += new System.EventHandler(this.btn_changeToAutoRemote_Click);
            // 
            // btn_changeToAutoLocal
            // 
            this.btn_changeToAutoLocal.Location = new System.Drawing.Point(23, 90);
            this.btn_changeToAutoLocal.Name = "btn_changeToAutoLocal";
            this.btn_changeToAutoLocal.Size = new System.Drawing.Size(166, 45);
            this.btn_changeToAutoLocal.TabIndex = 44;
            this.btn_changeToAutoLocal.Text = "Auto Local";
            this.btn_changeToAutoLocal.UseVisualStyleBackColor = true;
            this.btn_changeToAutoLocal.Click += new System.EventHandler(this.btn_changeToAutoLocal_Click_1);
            // 
            // btn_changeToRemove
            // 
            this.btn_changeToRemove.Location = new System.Drawing.Point(21, 33);
            this.btn_changeToRemove.Name = "btn_changeToRemove";
            this.btn_changeToRemove.Size = new System.Drawing.Size(169, 43);
            this.btn_changeToRemove.TabIndex = 46;
            this.btn_changeToRemove.Text = "Remove";
            this.btn_changeToRemove.UseVisualStyleBackColor = true;
            this.btn_changeToRemove.Click += new System.EventHandler(this.btn_changeToRemove_Click);
            // 
            // btn_changeToInstall
            // 
            this.btn_changeToInstall.Location = new System.Drawing.Point(21, 90);
            this.btn_changeToInstall.Name = "btn_changeToInstall";
            this.btn_changeToInstall.Size = new System.Drawing.Size(169, 43);
            this.btn_changeToInstall.TabIndex = 45;
            this.btn_changeToInstall.Text = "Install";
            this.btn_changeToInstall.UseVisualStyleBackColor = true;
            this.btn_changeToInstall.Click += new System.EventHandler(this.btn_changeToInstall_Click);
            // 
            // btn_close_tcp_port
            // 
            this.btn_close_tcp_port.Location = new System.Drawing.Point(180, 754);
            this.btn_close_tcp_port.Name = "btn_close_tcp_port";
            this.btn_close_tcp_port.Size = new System.Drawing.Size(166, 45);
            this.btn_close_tcp_port.TabIndex = 51;
            this.btn_close_tcp_port.Text = "Close Tcp Port";
            this.btn_close_tcp_port.UseVisualStyleBackColor = true;
            this.btn_close_tcp_port.Visible = false;
            this.btn_close_tcp_port.Click += new System.EventHandler(this.btn_close_tcp_port_Click);
            // 
            // btn_open_tcp_port
            // 
            this.btn_open_tcp_port.Location = new System.Drawing.Point(8, 754);
            this.btn_open_tcp_port.Name = "btn_open_tcp_port";
            this.btn_open_tcp_port.Size = new System.Drawing.Size(166, 45);
            this.btn_open_tcp_port.TabIndex = 50;
            this.btn_open_tcp_port.Text = "Open Tcp Port";
            this.btn_open_tcp_port.UseVisualStyleBackColor = true;
            this.btn_open_tcp_port.Visible = false;
            this.btn_open_tcp_port.Click += new System.EventHandler(this.btn_open_tcp_port_Click);
            // 
            // btn_pause
            // 
            this.btn_pause.Location = new System.Drawing.Point(12, 142);
            this.btn_pause.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btn_pause.Name = "btn_pause";
            this.btn_pause.Size = new System.Drawing.Size(138, 42);
            this.btn_pause.TabIndex = 13;
            this.btn_pause.Text = "Send Func 39";
            this.btn_pause.UseVisualStyleBackColor = true;
            this.btn_pause.Click += new System.EventHandler(this.btn_pause_Click);
            // 
            // DebugFormNew
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1206, 865);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "DebugFormNew";
            this.Text = "DebugForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DebugForm_FormClosed);
            this.Load += new System.EventHandler(this.DebugForm_Load);
            this.tabPage1.ResumeLayout(false);
            this.group_cycleRun.ResumeLayout(false);
            this.group_cycleRun.PerformLayout();
            this.gb_blockControl.ResumeLayout(false);
            this.gb_blockControl.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_section_dis)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox group_cycleRun;
        private System.Windows.Forms.ComboBox combox_cycle_type;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cb_StartGenAntoCmd;
        private System.Windows.Forms.GroupBox gb_blockControl;
        private System.Windows.Forms.CheckBox cb_FroceReserveReject;
        private System.Windows.Forms.CheckBox cb_FroceReservePass;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown num_section_dis;
        private System.Windows.Forms.TextBox txt_current_sec_id;
        private System.Windows.Forms.GroupBox groupBox3;
        private Components.uctlButton btn_changeToAutoRemote;
        private Components.uctlButton btn_changeToAutoLocal;
        private System.Windows.Forms.GroupBox groupBox1;
        private Components.uctlButton btn_changeToRemove;
        private Components.uctlButton btn_changeToInstall;
        private System.Windows.Forms.Label lbl_listening_status;
        private System.Windows.Forms.Label label42;
        private Components.uctlButton btn_close_tcp_port;
        private Components.uctlButton btn_open_tcp_port;
        private System.Windows.Forms.Label lbl_install_status;
        private System.Windows.Forms.Label label73;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.ComboBox cb_Abort_Type;
        private System.Windows.Forms.Label lbl_id_37_cmdID_value;
        private System.Windows.Forms.Label lbl_cmdID;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmb_pauseType;
        private System.Windows.Forms.Label label5;
        private Components.uctlButton btn_pause;
        private System.Windows.Forms.ComboBox cmb_pauseEvent;
        private System.Windows.Forms.ComboBox cmb_tcpipctr_Vehicle;
        private System.Windows.Forms.Button btn_refsh_vh_status;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cb_OperMode;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TabControl tabControl1;
    }
}