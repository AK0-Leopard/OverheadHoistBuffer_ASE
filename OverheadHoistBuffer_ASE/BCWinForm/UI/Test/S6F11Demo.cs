using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.Data.SECS.ASE;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System.Threading.Tasks;
using com.mirle.ibg3k0.sc.App;

namespace com.mirle.ibg3k0.bc.winform
{
    public partial class S6F11Demo : Form
    {
        App.BCApplication BCApp;
        ALINE line = null;
        List<PortDef> portList = null;
        List<ShelfDef> shelfDefs = null;
        List<AVEHICLE> avehicle = null;

        int[] ohtStatus;

        public S6F11Demo()
        {
            InitializeComponent();
        }

        private void S6F11Demo_Load(object sender, EventArgs e)
        {
            string str = Environment.CurrentDirectory;
            this.Text = this.Text + "   " + str;
            line = BCApp.SCApplication.getEQObjCacheManager().getLine();
            portList = BCApp.SCApplication.PortDefBLL.GetOHB_PortData(line.LINE_ID);
            shelfDefs = BCApp.SCApplication.ShelfDefBLL.LoadEnableShelf();
            avehicle = BCApp.SCApplication.VehicleBLL.loadAllVehicle();
            
            int selindex = 0;

            comboBox11.Items.Add(line.LINE_ID);

            foreach (var s in portList)
            {
                if (s.UnitType == "OHCV")
                {
                    selindex = portList.IndexOf(s);
                }
                else if (s.UnitType == "SHELF")
                {
                    comboBox9.Items.Add(s.PLCPortID);
                }
                else if(s.UnitType == "AGVZONE")
                {
                    comboBox1.Items.Add(s.PLCPortID);
                }

                if(BCApp.SCApplication.TransferService.isCVPort(s.PLCPortID))
                {
                    comboBox11.Items.Add(s.PLCPortID);
                }

                comboBox3.Items.Add(s.PLCPortID);
                comboBox4.Items.Add(s.PLCPortID);
                comboBox5.Items.Add(s.PLCPortID);
                comboBox8.Items.Add(s.PLCPortID);
            }

            foreach (var v in avehicle)
            {
                comboBox11.Items.Add(v.VEHICLE_ID.Trim());
                comboBox12.Items.Add(v.VEHICLE_ID.Trim());
            }
            comboBox12.SelectedIndex = 0;
            foreach (var s in shelfDefs)
            {
                comboBox3.Items.Add(s.ShelfID);
                comboBox4.Items.Add(s.ShelfID);
                comboBox5.Items.Add(s.ShelfID);
            }

            comboBox3.SelectedIndex = selindex;
            comboBox5.SelectedIndex = 1;

            ohtStatus = new int[]
            {
                ACMD_MCS.COMMAND_iIdle,
                ACMD_MCS.COMMAND_STATUS_BIT_INDEX_ENROUTE,
                ACMD_MCS.COMMAND_STATUS_BIT_INDEX_LOAD_ARRIVE,
                ACMD_MCS.COMMAND_STATUS_BIT_INDEX_LOADING,
                ACMD_MCS.COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE,
                ACMD_MCS.COMMAND_STATUS_BIT_INDEX_UNLOAD_ARRIVE,
                ACMD_MCS.COMMAND_STATUS_BIT_INDEX_UNLOADING,
                ACMD_MCS.COMMAND_STATUS_BIT_INDEX_UNLOAD_COMPLETE,
                ACMD_MCS.COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH,
                ACMD_MCS.COMMAND_STATUS_BIT_INDEX_DOUBLE_STORAGE,     //二重格，異常流程
                ACMD_MCS.COMMAND_STATUS_BIT_INDEX_EMPTY_RETRIEVAL,    //空取、異常流程
                ACMD_MCS.COMMAND_STATUS_BIT_INDEX_InterlockError,    //交握異常
            };

            comboBox6.Items.Add("iIdle");
            comboBox6.Items.Add("ENROUTE");
            comboBox6.Items.Add("LOAD_ARRIVE");
            comboBox6.Items.Add("LOADING");
            comboBox6.Items.Add("LOAD_COMPLETE");
            comboBox6.Items.Add("UNLOAD_ARRIVE");
            comboBox6.Items.Add("UNLOADING");
            comboBox6.Items.Add("UNLOAD_COMPLETE");
            comboBox6.Items.Add("COMMNAD_FINISH");
            comboBox6.Items.Add("DOUBLE_STORAGE"); 
            comboBox6.Items.Add("EMPTY_RETRIEVAL");
            comboBox6.Items.Add("InterlockError"); 
            comboBox6.SelectedIndex = 1;

            comboBox2.Items.Add("In");
            comboBox2.Items.Add("Out");
            comboBox2.SelectedIndex = 0;

            comboBox10.Items.Add("In");
            comboBox10.Items.Add("Out");
            comboBox10.SelectedIndex = 0;

            comboBox8.SelectedIndex = 0;
            numericUpDown1.Value = BCApp.SCApplication.TransferService.cstIdle;

            ShowDataList("portINIData");

            label7.Text = "目前狀態:" + BCApp.SCApplication.TransferService.agvWaitOutOpenBox.ToString();
            label17.Text = "目前狀態:" + BCApp.SCApplication.TransferService.portTypeChangeOK_CVPort_CstRemove.ToString();
            label18.Text = "自動救帳狀態:" + BCApp.SCApplication.TransferService.autoRemarkBOXCSTData.ToString();
            label21.Text = "多出狀態:" + BCApp.SCApplication.TransferService.setForMoreOut.ToString();

            Update();
        }

        public void UPStage()
        {
            comboBox7.Items.Clear();

            PortDef port = BCApp.SCApplication.PortDefBLL.GetPortData(comboBox5.Text);
            if (port == null)
            {
                return;
            }

            for (int i = 0; i <= port.Stage; i++)
            {
                if (i == port.Stage)
                {
                    i = (int)CassetteData.OHCV_STAGE.LP;
                }
                comboBox7.Items.Add((CassetteData.OHCV_STAGE)i);
            }

            comboBox7.SelectedIndex = 1;
            label3.Text = "目的Port：\n" + port.PLCPortID;
        }

        public void SetApp(App.BCApplication app)
        {
            BCApp = app;
        }

        public void SendS6F11(string ceid, VIDCollection vids)
        {
            BCApp.SCApplication.ReportBLL.Send(ceid, vids);
        }


        private void button28_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.Manual_InsertCmd(comboBox4.Text, comboBox5.Text);
        }

        private void button30_Click(object sender, EventArgs e)
        {
            var OHT = BCApp.SCApplication.CMDBLL.GetOHTCmd();
            ACMD_OHTC ohtCmdData = OHT.First();

            BCApp.SCApplication.TransferService.OHT_TransferStatus(ohtCmdData.CMD_ID, comboBox12.Text.Trim(), ohtStatus[comboBox6.SelectedIndex]);

            if (ohtStatus[comboBox6.SelectedIndex] == ACMD_MCS.COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH)
            {
                BCApp.SCApplication.CMDBLL.DeleteOHTCmd(ohtCmdData.CMD_ID);
                comboBox6.SelectedIndex = 1;
            }
            else
            {
                comboBox6.SelectedIndex = comboBox6.SelectedIndex + 1;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //CassetteData.OHCV_STAGE stage = (CassetteData.OHCV_STAGE)(comboBox7.SelectedIndex);

            CassetteData datainfo = new CassetteData();
            //datainfo.CSTID = textBox1.Text;
            datainfo.Carrier_LOC = comboBox5.Text;
            datainfo.BOXID = textBox2.Text;

            BCApp.SCApplication.TransferService.PortPositionWaitOut(datainfo, comboBox7.SelectedIndex, "testUI");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //BCApp.SCApplication.TransferService.PortType("PORT_" + comboBox3.Text, "Out");
            BCApp.SCApplication.TransferService.PortTypeChange(comboBox3.Text, (E_PortType)comboBox10.SelectedIndex, "S6F11 Demo");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            CassetteData datainfo = new CassetteData();
            //datainfo.CSTID = textBox1.Text;
            datainfo.Carrier_LOC = comboBox5.Text;
            //datainfo.BOXID = textBox2.Text;

            BCApp.SCApplication.TransferService.PortCarrierRemoved(datainfo, false, "DemoUI");
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            UPStage();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            CassetteData datainfo = new CassetteData();
            datainfo.CSTID = textBox1.Text;
            datainfo.Carrier_LOC = comboBox5.Text;
            datainfo.BOXID = textBox2.Text;

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox4.Text = comboBox3.Text;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            CassetteData datainfo = new CassetteData();
            datainfo.CSTID = textBox1.Text;
            datainfo.Carrier_LOC = comboBox5.Text;
            datainfo.BOXID = textBox2.Text;

            //BCApp.SCApplication.TransferService.PortWaitOut(datainfo);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ACMD_MCS datainfo = BCApp.SCApplication.CMDBLL.LoadCmdData().First();

            #region 新增OHT命令
            ACMD_OHTC cmdohtc = new ACMD_OHTC
            {
                CMD_ID = datainfo.CMD_ID,
                CARRIER_ID = datainfo.CARRIER_ID,
                BOX_ID = datainfo.BOX_ID,
                VH_ID = comboBox12.Text,
                CMD_ID_MCS = datainfo.CMD_ID,
                CMD_TPYE = 0,
                PRIORITY = 50,
                SOURCE = datainfo.HOSTSOURCE,
                DESTINATION = datainfo.HOSTDESTINATION,
                CMD_STAUS = 0,
                CMD_PROGRESS = 0,
                ESTIMATED_EXCESS_TIME = 0,
                REAL_CMP_TIME = 0,
                ESTIMATED_TIME = 50
            };

            BCApp.SCApplication.CMDBLL.creatCommand_OHTC(cmdohtc);
            #endregion
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox3.Text = textBox2.Text;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            UPDataMCS_LinK();
        }
        private void UPDataMCS_LinK()
        {
            string s = "連線狀態：" + line.Secs_Link_Stat.ToString() + "\n"
                     + "連線模式：" + line.Host_Control_State.ToString() + "\n"
                     + "設備狀態：" + line.GetState() + "\n"
                     ;

            label2.Text = s;
        }
        private void button10_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.ReportPortType(comboBox8.Text, (E_PortType)comboBox2.SelectedIndex, "S6F11Demo");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.ReportBLL.ReportPortOutOfService(comboBox8.Text);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.ReportBLL.ReportPortInService(comboBox8.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ACMD_MCS datainfo = BCApp.SCApplication.CMDBLL.LoadCmdData().First();

            if (datainfo != null)
            {
                BCApp.SCApplication.CMDBLL.updateCMD_MCS_BCROnCrane(datainfo.CMD_ID, textBox3.Text);
                BCApp.SCApplication.TransferService.OHT_IDRead(datainfo.CMD_ID, datainfo.CRANE, textBox3.Text, BCRReadResult.BcrMisMatch);
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.Manual_OnLineMode();
            UPDataMCS_LinK();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.Manual_OFFLineMode();
            UPDataMCS_LinK();
        }
        #region 刪除資料表按鈕


        private void button18_Click(object sender, EventArgs e)
        {
            var v1 = BCApp.SCApplication.CMDBLL.GetOHTCmd();

            foreach (var v in BCApp.SCApplication.CMDBLL.GetOHTCmd())
            {
                BCApp.SCApplication.CMDBLL.DeleteOHTCmd(v.CMD_ID);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            foreach (var v in BCApp.SCApplication.VehicleBLL.loadAllVehicle())
            {
                BCApp.SCApplication.VehicleBLL.DeleteOHTbyVhID(v.VEHICLE_ID);
            }
        }

        #endregion

        private void button20_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.Manual_ShelfEnable(comboBox9.Text, true);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.Manual_ShelfEnable(comboBox9.Text, false);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.AlliniPortData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.Manual_InsertCassette(textBox1.Text, textBox2.Text, comboBox3.Text);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.OHBC_AlarmAllCleared(comboBox11.Text);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.OHBC_AlarmCleared(comboBox11.Text, textBox5.Text);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.OHBC_AlarmSet(comboBox11.Text, textBox5.Text);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.Manual_AutoMode();
        }

        private void button29_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.Manual_PauseMode();
        }

        private void button32_Click(object sender, EventArgs e)
        {
            foreach(var v in comboBox11.Items)
            {
                BCApp.SCApplication.TransferService.OHBC_AlarmAllCleared(v.ToString());
            }
        }

        private void button14_Click_1(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.Manual_OnLineRemote();
            UPDataMCS_LinK();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.cstIdle = (int)numericUpDown1.Value;
            numericUpDown1.Value = BCApp.SCApplication.TransferService.cstIdle;
        }

        private void button25_Click(object sender, EventArgs e)
        {
            PortPLCInfo portPLCInfo = new PortPLCInfo();
            portPLCInfo.CassetteID = textBox1.Text;
            portPLCInfo.BoxID = textBox2.Text;
            portPLCInfo.EQ_ID = comboBox3.Text;
            BCApp.SCApplication.TransferService.PLC_ReportPortWaitIn(portPLCInfo, "S6F11 Demo");
        }

        private void button16_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.portTypeChangeOK_CVPort_CstRemove = true;
            label17.Text = "目前狀態:" + BCApp.SCApplication.TransferService.portTypeChangeOK_CVPort_CstRemove.ToString();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.portTypeChangeOK_CVPort_CstRemove = false;
            label17.Text = "目前狀態:" + BCApp.SCApplication.TransferService.portTypeChangeOK_CVPort_CstRemove.ToString();
        }

        private void button31_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.agvWaitOutOpenBox = true;
            label7.Text = "目前狀態:" + BCApp.SCApplication.TransferService.agvWaitOutOpenBox.ToString();
        }

        private void button34_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.agvWaitOutOpenBox = false;
            label7.Text = "目前狀態:" + BCApp.SCApplication.TransferService.agvWaitOutOpenBox.ToString();
        }

        private void button35_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.autoRemarkBOXCSTData = true;
            label18.Text = "自動救帳狀態:" + BCApp.SCApplication.TransferService.autoRemarkBOXCSTData.ToString();
        }

        private void button36_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.autoRemarkBOXCSTData = false ;
            label18.Text = "自動救帳狀態:" + BCApp.SCApplication.TransferService.autoRemarkBOXCSTData.ToString();
        }

        private void button37_Click(object sender, EventArgs e)
        {
            ShowDataList("portINIData");
        }

        private void button38_Click(object sender, EventArgs e)
        {
            ShowDataList("PortADR"); 
        }
        private void button39_Click(object sender, EventArgs e)
        {
            ShowDataList("All_ADR");            
        }
        private void button43_Click(object sender, EventArgs e)
        {
            ShowDataList("GetAGVPort");
        }

        public void ShowDataList(string type)
        {
            switch(type)
            {
                case "portINIData":
                    dataGridView1.DataSource = BCApp.SCApplication.TransferService.portINIData.Values.ToList();
                    break;
                case "PortADR":
                    dataGridView1.DataSource = BCApp.SCApplication.TransferService.GetPortADR();
                    break;
                case "All_ADR":
                    dataGridView1.DataSource = BCApp.SCApplication.TransferService.GetAll_ADR();
                    break;
                case "GetAGVPort":
                    dataGridView1.DataSource = BCApp.SCApplication.TransferService.GetAGVPort(comboBox1.Text);
                    break;
            }

            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            int count = dataGridView1.Rows.Count;

            label14.Text = type + ": 共 " + count.ToString() + " 筆";
        }

        private void button40_Click(object sender, EventArgs e)
        {
            string s1 = comboBox11.Text;
            string s2 = textBox5.Text;
            Task.Run(() => BCApp.SCApplication.TransferService.OHBC_AlarmSet(s1, s2));
            Thread.Sleep(100);
            Task.Run(() => BCApp.SCApplication.TransferService.OHBC_AlarmCleared(s1, s2));
        }

        private void button41_Click(object sender, EventArgs e)
        {
            sc.App.SystemParameter.cmdTimeOutToAlternate = (int)numericUpDown2.Value;
            Update();
        }

        private void button42_Click(object sender, EventArgs e)
        {
            sc.App.SystemParameter.cmdPriorityAdd = (int)numericUpDown3.Value;
            Update();
        }
        public void Update()
        {
            numericUpDown2.Value = sc.App.SystemParameter.cmdTimeOutToAlternate;
            numericUpDown3.Value = sc.App.SystemParameter.cmdPriorityAdd;
        }

        private void button44_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.setForMoreOut = true;
            label21.Text = "多出狀態:" + BCApp.SCApplication.TransferService.setForMoreOut.ToString();
        }

        private void CloseSwapMoreOut_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.setForMoreOut = false;
            label21.Text = "多出狀態:" + BCApp.SCApplication.TransferService.setForMoreOut.ToString();
        }
    }
}
