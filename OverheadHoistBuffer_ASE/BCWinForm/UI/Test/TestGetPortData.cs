using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.mirle.ibg3k0.bc.winform
{
    public partial class TestGetPortData : Form
    {
        App.BCApplication BCApp;
        ALINE line = null;
        List<PortDef> portList = null;
        PortPLCInfo portData = new PortPLCInfo();

        TransferService transferService = null;
        DateTime openTime = new DateTime();

        public TestGetPortData()
        {
            InitializeComponent();
        }

        private void TestGetPortData_Load(object sender, EventArgs e)
        {
            transferService = BCApp.SCApplication.TransferService;
            line = BCApp.SCApplication.getEQObjCacheManager().getLine();
            portList = BCApp.SCApplication.PortDefBLL.GetOHB_CVPortData(line.LINE_ID);

            comboBox1.Items.Clear();
            comboBox2.Items.Clear();

            foreach (var v in portList)
            {
                if(transferService.isCVPort(v.PLCPortID) && transferService.isAGVZone(v.PLCPortID) == false)
                {
                    comboBox1.Items.Add(v.PLCPortID);
                }
            }

            comboBox1.SelectedIndex = 0;

            comboBox2.Items.Add("In");
            comboBox2.Items.Add("Out");
            comboBox2.SelectedIndex = 0;

            foreach(var v in transferService.GetAGVZone())
            {
                comboBox3.Items.Add(v.PortName);

                comboBox3.SelectedIndex = 0;
            }

            #region dataGridView2
            dataGridView2.Columns.Add("中文說明", "中文說明");
            dataGridView2.Columns.Add("訊號名稱", "訊號名稱");
            dataGridView2.Columns.Add("狀態", "狀態");

            dataGridView2.Rows.Add("運轉狀態", "RUN", "");
            dataGridView2.Rows.Add("自動模式", "IsAutoMode", "");
            dataGridView2.Rows.Add("異常狀態", "ErrorBit", "");
            dataGridView2.Rows.Add("異常代碼", "ErrorCode", "");
            dataGridView2.Rows.Add("流向", "", "");
            dataGridView2.Rows.Add("是否能切換流向", "IsModeChangable", "");
            dataGridView2.Rows.Add("流向:Port 往 OHT", "IsInputMode", "");
            dataGridView2.Rows.Add("流向:OHT 往 Port", "IsOutputMode", "");
            dataGridView2.Rows.Add("投出入說明", "", "");
            dataGridView2.Rows.Add("Port 是否能搬入 BOX ", "IsReadyToLoad", "");
            dataGridView2.Rows.Add("Port 是否能搬出 BOX ", "IsReadyToUnload", "");
            dataGridView2.Rows.Add("等待說明", "", "");
            dataGridView2.Rows.Add("等待 OHT 搬走", "PortWaitIn", "");
            dataGridView2.Rows.Add("等待從 Port 搬走", "PortWaitOut", "");
            dataGridView2.Rows.Add("狀態說明", "", "");
            dataGridView2.Rows.Add("PLC 離線狀態", "CIM_ON", "");
            dataGridView2.Rows.Add("PLC 預先入料完成", "PreLoadOK", "");

            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView2.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            #endregion
            #region dataGridView3
            dataGridView3.Columns.Add("中文說明", "中文說明");                 //0
            dataGridView3.Columns.Add("訊號名稱", "訊號名稱");                 //1
            dataGridView3.Columns.Add("狀態", "狀態");                         //2
            dataGridView3.Columns.Add("BOXID", "BOXID");                       //3

            dataGridView3.Rows.Add("帳移除", "Remove", "");                    //0
            dataGridView3.Rows.Add("", "", "");                                //1
            dataGridView3.Rows.Add("盒子 BCR 讀取狀態", "BCRReadDone", "");    //2
            dataGridView3.Rows.Add("盒子ID", "BoxID", "");                     //3
            dataGridView3.Rows.Add("", "", "");                                //4            
            dataGridView3.Rows.Add("節數 1 是否有盒子", "LoadPosition1", "");  //5
            dataGridView3.Rows.Add("節數 2 是否有盒子", "LoadPosition2", "");  //6
            dataGridView3.Rows.Add("節數 3 是否有盒子", "LoadPosition3", "");  //7
            dataGridView3.Rows.Add("節數 4 是否有盒子", "LoadPosition4", "");  //8
            dataGridView3.Rows.Add("節數 5 是否有盒子", "LoadPosition5", "");  //9
            dataGridView3.Rows.Add("節數 6 是否有盒子", "LoadPosition6", "");  //10
            dataGridView3.Rows.Add("節數 7 是否有盒子", "LoadPosition7", "");  //11

            dataGridView3.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView3.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            #endregion
            #region dataGridView4
            dataGridView4.Columns.Add("中文說明", "中文說明");                 //0
            dataGridView4.Columns.Add("訊號名稱", "訊號名稱");                 //1
            dataGridView4.Columns.Add("狀態", "狀態");                         //2

            dataGridView4.Rows.Add("開啟自動補退盒子功能", "openAGV_Station");  //0
            dataGridView4.Rows.Add("開啟自動切換流向功能", "openAGV_AutoPortType");  //1
            dataGridView4.Rows.Add("AGV 模式", "IsAGVMode");                    //2
            dataGridView4.Rows.Add("MGV 模式", "IsMGVMode");                    //3
            dataGridView4.Rows.Add("", "", "");                                 //4
            dataGridView4.Rows.Add("AGV 能投放", "AGVPortReady");               //5
            dataGridView4.Rows.Add("AGV 不能投放", "AGVPortMismatch");          //6
            dataGridView4.Rows.Add("", "", "");                                 //7
            dataGridView4.Rows.Add("是否能開蓋", "CanOpenBox");                 //8
            dataGridView4.Rows.Add("開蓋狀態", "IsBoxOpen");                    //9
            dataGridView4.Rows.Add("", "", "");                                 //10
            dataGridView4.Rows.Add("卡匣ID", "CassetteID");                     //11
            dataGridView4.Rows.Add("是否有卡匣", "IsCSTPresence");              //12

            dataGridView4.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView4.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            #endregion

            GetPortData();
            openTime = DateTime.Now;
        }
        public void SetApp(App.BCApplication app)
        {
            BCApp = app;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            portData = transferService.GetPLC_PortData(comboBox1.Text);
            #region dataGridView2
            dataGridView2.Rows[0].Cells[2].Value = portData.OpAutoMode.ToString();
            dataGridView2.Rows[1].Cells[2].Value = portData.IsAutoMode.ToString();
            dataGridView2.Rows[2].Cells[2].Value = portData.OpError.ToString();
            dataGridView2.Rows[3].Cells[2].Value = portData.ErrorCode.ToString();
            //dataGridView2.Rows[4].Cells[2].Value = 流向說明;
            dataGridView2.Rows[5].Cells[2].Value = portData.IsModeChangable.ToString();
            dataGridView2.Rows[6].Cells[2].Value = portData.IsInputMode.ToString();
            dataGridView2.Rows[7].Cells[2].Value = portData.IsOutputMode.ToString();
            //dataGridView2.Rows[8].Cells[2].Value = 投出入說明;
            dataGridView2.Rows[9].Cells[2].Value = portData.IsReadyToLoad.ToString();
            dataGridView2.Rows[10].Cells[2].Value = portData.IsReadyToUnload.ToString();
            //dataGridView2.Rows[11].Cells[2].Value = 等待說明;
            dataGridView2.Rows[12].Cells[2].Value = portData.PortWaitIn.ToString();
            dataGridView2.Rows[13].Cells[2].Value = portData.PortWaitOut.ToString();
            //dataGridView2.Rows[14].Cells[2].Value = 等待說明;
            dataGridView2.Rows[15].Cells[2].Value = portData.cim_on.ToString();
            dataGridView2.Rows[16].Cells[2].Value = portData.preLoadOK.ToString();
            #endregion
            #region dataGridView3
            dataGridView3.Rows[0].Cells[2].Value = portData.CstRemoveCheck.ToString();
            //dataGridView3.Rows[1].Cells[2].Value = portData.IsAutoMode.ToString();
            dataGridView3.Rows[2].Cells[2].Value = portData.BCRReadDone.ToString();
            dataGridView3.Rows[3].Cells[2].Value = portData.BoxID.ToString();
            //dataGridView3.Rows[4].Cells[2].Value = "";            
            dataGridView3.Rows[5].Cells[2].Value = portData.LoadPosition1.ToString();
            dataGridView3.Rows[5].Cells[3].Value = portData.LoadPositionBOX1.ToString();
            
            dataGridView3.Rows[6].Cells[2].Value = portData.LoadPosition2.ToString();
            dataGridView3.Rows[6].Cells[3].Value = portData.LoadPositionBOX2.ToString();

            dataGridView3.Rows[7].Cells[2].Value = portData.LoadPosition3.ToString();
            dataGridView3.Rows[7].Cells[3].Value = portData.LoadPositionBOX3.ToString();

            dataGridView3.Rows[8].Cells[2].Value = portData.LoadPosition4.ToString();
            dataGridView3.Rows[8].Cells[3].Value = portData.LoadPositionBOX4.ToString();

            dataGridView3.Rows[9].Cells[2].Value = portData.LoadPosition5.ToString();
            dataGridView3.Rows[9].Cells[3].Value = portData.LoadPositionBOX5.ToString();

            dataGridView3.Rows[10].Cells[2].Value = portData.LoadPosition6.ToString();
            dataGridView3.Rows[11].Cells[2].Value = portData.LoadPosition7.ToString();
            #endregion
            #region dataGridView4
            dataGridView4.Rows[0].Cells[2].Value = transferService.GetAGV_StationStatus(comboBox1.Text);
            dataGridView4.Rows[1].Cells[2].Value = transferService.GetAGV_AutoPortType(comboBox1.Text);
            dataGridView4.Rows[2].Cells[2].Value = portData.IsAGVMode.ToString();
            dataGridView4.Rows[3].Cells[2].Value = portData.IsMGVMode.ToString();
            //dataGridView4.Rows[4].Cells[2].Value = 
            dataGridView4.Rows[5].Cells[2].Value = portData.AGVPortReady.ToString();
            dataGridView4.Rows[6].Cells[2].Value = portData.CSTPresenceMismatch.ToString();
            //dataGridView4.Rows[7].Cells[2].Value = 
            dataGridView4.Rows[8].Cells[2].Value = portData.CanOpenBox.ToString();
            dataGridView4.Rows[9].Cells[2].Value = portData.IsBoxOpen.ToString();
            //dataGridView4.Rows[10].Cells[2].Value = 
            dataGridView4.Rows[11].Cells[2].Value = portData.CassetteID.ToString();
            dataGridView4.Rows[12].Cells[2].Value = portData.IsCSTPresence.ToString();
            #endregion
            #region 待刪除
            //label2.Text =
            //    "Port狀態：\n"
            //    + "運轉狀態_RUN: " + portData.OpAutoMode.ToString() + "\n"
            //    + "自動模式_IsAutoMode: " + portData.IsAutoMode.ToString() + "\n"                
            //    + "異常狀態_ErrorBit: " + portData.OpError.ToString() + "\n"
            //    + "異常代碼_ErrorCode: " + portData.ErrorCode.ToString() + "\n"
            //    ;

            //label3.Text = "搬送資訊：" + "\n"
            //    + "是否能切換流向_IsModeChangable: " + portData.IsModeChangable.ToString() + "\n"
            //    + "流向:AGV 往 OHT_IsInputMode: " + portData.IsInputMode.ToString() + "\n"
            //    + "流向:OHT 往 AGV_IsOutputMode: " + portData.IsOutputMode.ToString() + "\n"
            //    + "\n"
            //    + "AGV Port是否能投入 BOX_IsReadyToLoad: " + portData.IsReadyToLoad.ToString() + "\n"
            //    + "AGV Port是否能投入 BOX_IsReadyToUnload: " + portData.IsReadyToUnload.ToString() + "\n"
            //    + "\n"
            //    + "等待 OHT 搬走_PortWaitIn: " + portData.PortWaitIn.ToString() + "\n"
            //    + "等待從 AGV Port搬走_PortWaitOut: " + portData.PortWaitOut.ToString() + "\n"
            //    + "\n"
            //    + "帳移除_Remove: " + portData.CstRemoveCheck.ToString() + "\n"
            //    + "\n"
            //    + "CassetteID: " + portData.CassetteID.ToString() + "\n"
            //    + "是否有卡匣_IsCSTPresence: " + portData.IsCSTPresence.ToString() + "\n"
            //    + "\n"
            //    + "BCR讀取狀態_BCRReadDone: " + portData.BCRReadDone.ToString() + "\n"
            //    + "\n"
            //    + "BoxID:   " + portData.BoxID.ToString() + "\n"
            //    + "BOX位置1_LoadPosition1: " + portData.LoadPosition1.ToString() + "   BOXID:  " + portData.LoadPositionBOX1.ToString() + "\n"
            //    + "BOX位置2_LoadPosition2: " + portData.LoadPosition2.ToString() + "   BOXID:  " + portData.LoadPositionBOX2.ToString() + "\n"
            //    + "BOX位置3_LoadPosition3: " + portData.LoadPosition3.ToString() + "   BOXID:  " + portData.LoadPositionBOX3.ToString() + "\n"
            //    + "BOX位置4_LoadPosition4: " + portData.LoadPosition4.ToString() + "   BOXID:  " + portData.LoadPositionBOX4.ToString() + "\n"
            //    + "BOX位置5_LoadPosition5: " + portData.LoadPosition5.ToString() + "   BOXID:  " + portData.LoadPositionBOX5.ToString() + "\n"
            //    + "BOX位置6_LoadPosition6: " + portData.LoadPosition6.ToString() + "\n"
            //    + "BOX位置7_LoadPosition7: " + portData.LoadPosition7.ToString() + "\n"
            //    ;

            //label4.Text =
            //    "AGV Port 專有訊號:\n"
            //    + "開啟自動補退 BOX 功能_openAGV_Station:   " + transferService.GetAGV_StationStatus(comboBox1.Text) + "\n"
            //    + "AGV模式_IsAGVMode:                       " + portData.IsAGVMode.ToString() + "\n"
            //    + "\n"
            //    + "AGVPortReady:                            " + portData.AGVPortReady.ToString() + "\n"
            //    + "AGVPortMismatch:                         " + portData.CSTPresenceMismatch.ToString() + "\n"
            //    + "\n"
            //    + "是否能開蓋:                              " + portData.CanOpenBox.ToString() + "\n"
            //    + "開蓋狀態:                                " + portData.IsBoxOpen.ToString() + "\n"
            //    + "\n"
            //    ;
            #endregion

            label6.Text = "關聯實際狀態： " + transferService.agvZone_ConnectedRealAGVPortRunDown.ToString();
            label11.Text = "單取單放狀態： " + transferService.oneInoneOutMethodUse.ToString();

            if (transferService.isAGVZone(comboBox3.Text))
            {
                label7.Text = "強制讓貨先出去: " + transferService.portINIData[comboBox3.Text].forceRejectAGVCTrigger.ToString();
            }
            else
            {
                label7.Text = "AGV Zone 名稱輸入錯誤";
            }

            label8.Text = "ST01: " + transferService.agvcTriggerResult_ST01;
            label9.Text = "ST02: " + transferService.agvcTriggerResult_ST02;
            label10.Text = "ST03: " + transferService.agvcTriggerResult_ST03;

            dataGridView5.DataSource = BCApp.SCApplication.VehicleBLL.cache.loadVhs().Select(data => 
                                    new { data.VEHICLE_ID
                                        , data.HAS_CST
                                        , data.BOX_ID
                                        , data.CST_ID
                                        , data.ACT_STATUS
                                        , data.MCS_CMD
                                        , data.CMD_CST_ID 
                                        }
                                                                                            ).ToList() ;

            TimeSpan timeOut = DateTime.Now - openTime;

            if (timeOut.Minutes > 5)
            {
                timer1.Enabled = false;
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            transferService.PortCommanding(comboBox1.Text ,true);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            transferService.PortCommanding(comboBox1.Text, false);
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            string port_id = comboBox1.Text;
            E_PortType mode = (E_PortType)comboBox2.SelectedIndex;
            await Task.Run(() => transferService.PortTypeChange(port_id, mode, "測試用 UI"));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            transferService.toAGV_Mode(comboBox1.Text);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            transferService.toMGV_Mode(comboBox1.Text);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            transferService.SetPortRun(comboBox1.Text);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            transferService.SetPortStop(comboBox1.Text);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            transferService.SetAGV_PortBCR_Read(comboBox1.Text);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            transferService.SetAGV_PortOpenBOX(comboBox1.Text, "工程UI_TestGetPortData");
        }

        private void button13_Click(object sender, EventArgs e)
        {
            transferService.RstAGV_PortBCR_Read(comboBox1.Text);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            transferService.PortAlarrmReset(comboBox1.Text);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            GetPortData();
        }

        public void GetPortData()
        {
            dataGridView1.DataSource = BCApp.SCApplication.PortDefBLL.GetOHB_CVPortData(line.LINE_ID);
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell v in dataGridView1.SelectedCells)
            {
                string portName = dataGridView1.Rows[v.RowIndex].Cells["PLCPortID"].Value.ToString();
                transferService.UpdateIgnoreModeChange(portName, "Y");
            }
            GetPortData();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell v in dataGridView1.SelectedCells)
            {
                string portName = dataGridView1.Rows[v.RowIndex].Cells["PLCPortID"].Value.ToString();
                transferService.UpdateIgnoreModeChange(portName, "N");
            }
            GetPortData();
        }

        private void button31_Click(object sender, EventArgs e)
        {
            transferService.OpenAGV_Station(comboBox1.Text, true, "UI_TestGetPortData");
        }

        private void button19_Click(object sender, EventArgs e)
        {
            transferService.OpenAGV_Station(comboBox1.Text, false, "UI_TestGetPortData");
        }

        private void button17_Click(object sender, EventArgs e)
        {
            foreach (var v in portList)
            {
                if(transferService.isUnitType(v.PLCPortID, sc.Service.UnitType.AGV))
                {
                    transferService.OpenAGV_Station(v.PLCPortID, true, "UI_TestGetPortData");
                }
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            foreach (var v in portList)
            {
                if (transferService.isUnitType(v.PLCPortID, sc.Service.UnitType.AGV))
                {
                    transferService.OpenAGV_Station(v.PLCPortID, false, "UI_TestGetPortData");
                }
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            PortPLCInfo plcInfo = transferService.GetPLC_PortData(comboBox1.Text);

            if(plcInfo.LoadPosition1)
            {
                transferService.PLC_ReportPortWaitIn(plcInfo, "TestGetPortData");
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            transferService.OpenAGV_AutoPortType(comboBox1.Text, true);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            transferService.OpenAGV_AutoPortType(comboBox1.Text, false);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            foreach (var v in portList)
            {
                if (transferService.isUnitType(v.PLCPortID, sc.Service.UnitType.AGV))
                {
                    transferService.OpenAGV_AutoPortType(v.PLCPortID, true);
                }
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            foreach (var v in portList)
            {
                if (transferService.isUnitType(v.PLCPortID, sc.Service.UnitType.AGV))
                {
                    transferService.OpenAGV_AutoPortType(v.PLCPortID, false);
                }
            }
        }

        private void button26_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell v in dataGridView1.SelectedCells)
            {
                string portName = dataGridView1.Rows[v.RowIndex].Cells["PLCPortID"].Value.ToString();
                transferService.PortInOutService(portName, E_PORT_STATUS.InService, "TestGetPortData");
            }
            GetPortData();            
        }

        private void button27_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell v in dataGridView1.SelectedCells)
            {
                string portName = dataGridView1.Rows[v.RowIndex].Cells["PLCPortID"].Value.ToString();
                transferService.PortInOutService(portName, E_PORT_STATUS.OutOfService, "TestGetPortData");
            }
            GetPortData();
        }

        private void button28_Click(object sender, EventArgs e)
        {
            transferService.Manual_OpenAGV_State(comboBox3.Text);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            transferService.Manual_CloseAGV_State(comboBox3.Text);
        }

        private void button30_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.AlliniPortData();
            List<PortDef> portDefList = BCApp.SCApplication.PortDefBLL.GetOHB_CVPortData(line.LINE_ID);
            foreach (PortDef portDefData in portDefList)
            {
                if (portDefData.State == E_PORT_STATUS.InService)
                {
                    BCApp.SCApplication.ReportBLL.ReportPortInService(portDefData.PLCPortID);
                }
                else if (portDefData.State == E_PORT_STATUS.OutOfService)
                {
                    BCApp.SCApplication.ReportBLL.ReportPortOutOfService(portDefData.PLCPortID);
                }
            }
            dataGridView1.DataSource = portDefList;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void button32_Click(object sender, EventArgs e)
        {
            transferService.agvZone_ConnectedRealAGVPortRunDown = true;
        }

        private void button33_Click(object sender, EventArgs e)
        {
            transferService.agvZone_ConnectedRealAGVPortRunDown = false;
        }

        private void button34_Click(object sender, EventArgs e)
        {
            if (transferService.isAGVZone(comboBox3.Text))
            {
                transferService.portINIData[comboBox3.Text].forceRejectAGVCTrigger = true;
            }            
        }

        private void button35_Click(object sender, EventArgs e)
        {
            if (transferService.isAGVZone(comboBox3.Text))
            {
                transferService.portINIData[comboBox3.Text].forceRejectAGVCTrigger = false;
            }
        }

        private void button36_Click(object sender, EventArgs e)
        {
            if(transferService.portINIData[comboBox3.Text].forceRejectAGVCTrigger == true)
            {
                transferService.TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " forceRejectAGVCTrigger 觸發按鍵點選。");
                transferService.CanExcuteUnloadTransferAGVStationFromAGVC(comboBox3.Text.Trim(), 0, false);
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewCell v in dataGridView5.SelectedCells)
                {
                    string portName = dataGridView5.Rows[v.RowIndex].Cells["VEHICLE_ID"].Value.ToString();
                    transferService.iniOHTData(portName, "UI_TestGetPortData");
                }
            }
            catch
            {

            }
        }

        private void button37_Click(object sender, EventArgs e)
        {
            transferService.PortBCR_Enable(comboBox1.Text, true);
        }

        private void button38_Click(object sender, EventArgs e)
        {
            transferService.PortBCR_Enable(comboBox1.Text, false);
        }

        private void UseFirst2Port_buttom_Click(object sender, EventArgs e)
        {
            if (comboBox3.Text.Contains("LOOP"))
            {
                transferService.Manual_UseFirst2Port(comboBox3.Text);
            }
        }

        private void UseLast2Port_buttom_Click(object sender, EventArgs e)
        {
            if (comboBox3.Text.Contains("LOOP"))
            {
                transferService.Manual_UseLast2Port(comboBox3.Text);
            }
        }

        private void button41_Click(object sender, EventArgs e)
        {
            transferService.doUpdateTimeOutForAutoUD(comboBox1.Text, (int)numericUpDown1.Value);
        }

        private void button42_Click(object sender, EventArgs e)
        {
            transferService.Manual_SetOneInoneOutMethodUse(true);
        }

        private void button43_Click(object sender, EventArgs e)
        {
            transferService.Manual_SetOneInoneOutMethodUse(false);
        }
    }
}
