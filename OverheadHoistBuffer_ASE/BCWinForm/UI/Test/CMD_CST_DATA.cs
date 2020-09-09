using com.mirle.ibg3k0.sc;
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

namespace com.mirle.ibg3k0.bc.winform.UI.Test
{
    public partial class CMD_CST_DATA : Form
    {
        App.BCApplication BCApp;
        ALINE line = null;
        List<PortINIData> portInIList = null;

        List<ShelfDef> shelfDefs = null;
        List<AVEHICLE> avehicle = null;

        DateTime openTime = new DateTime();

        public CMD_CST_DATA()
        {
            InitializeComponent();
        }
        public void SetApp(App.BCApplication app)
        {
            BCApp = app;
        }
        private void CMD_CST_DATA_Load(object sender, EventArgs e)
        {
            line = BCApp.SCApplication.getEQObjCacheManager().getLine();
            shelfDefs = BCApp.SCApplication.ShelfDefBLL.LoadEnableShelf();
            avehicle = BCApp.SCApplication.VehicleBLL.loadAllVehicle();
            portInIList = BCApp.SCApplication.TransferService.portINIData.Values.ToList();

            foreach (var v in portInIList)
            {
                if ( (BCApp.SCApplication.TransferService.isCVPort(v.PortName) && v.nowStage == v.Stage)
                    || BCApp.SCApplication.TransferService.isAGVZone(v.PortName)
                   )
                {
                    comboBox1.Items.Add(v.PortName);
                    comboBox2.Items.Add(v.PortName);
                    comboBox3.Items.Add(v.PortName);
                }
            }

            foreach (var v in avehicle)
            {
                comboBox1.Items.Add(v.VEHICLE_ID);
                //comboBox2.Items.Add(v.PLCPortID);
                comboBox3.Items.Add(v.VEHICLE_ID);
            }

            foreach (var v in shelfDefs)
            {
                comboBox1.Items.Add(v.ShelfID);
                comboBox2.Items.Add(v.ShelfID);
            }

            foreach (var v in BCApp.SCApplication.ShelfDefBLL.LoadShelf())
            {
                comboBox3.Items.Add(v.ShelfID);
            }

            foreach (var v in BCApp.SCApplication.ZoneDefBLL.loadZoneData())
            {
                comboBox4.Items.Add(v.ZoneID.Trim());
                comboBox4.SelectedIndex = 0;
            }

            dateTimePicker1.Value = DateTime.Now.AddHours(-1);
            dateTimePicker2.Value = DateTime.Now;

            UpDate_CmdData();
            UpDate_CstData();
            UpDate_AlarmData();

            openTime = DateTime.Now;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            UpDate_CstData();
        }
        #region 命令操作
        public void UpDate_CmdData()
        {
            label13.Text = "";
            dataGridView1.DataSource = BCApp.SCApplication.CMDBLL.LoadCmdData();
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }
        public void GetAllCmdData()
        {
            List<ACMD_MCS> cmdData = BCApp.SCApplication.CMDBLL.LoadCmdDataByStartEnd(dateTimePicker1.Value, dateTimePicker2.Value);
            dataGridView1.DataSource = cmdData;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            label13.Text = "總搬送次數: " + cmdData.Count() + "\n"
                 + "正常搬送次數: " + cmdData.Where(data => data.COMMANDSTATE == 128).Count() + "\n"
                 + "異常搬送次數: " + cmdData.Where(data => data.COMMANDSTATE != 128).Count() + "\n"
                ;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            UpDate_CmdData();
        }
        private void button12_Click(object sender, EventArgs e)
        {
            GetAllCmdData();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult result;

            result = MessageBox.Show("確定結束所選的命令?", "取消確認", MessageBoxButtons.YesNo);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                List<int> listInt = new List<int>();

                foreach (DataGridViewCell v in dataGridView1.SelectedCells)
                {
                    if (listInt.Contains(v.RowIndex))
                    {
                        continue;
                    }
                    else
                    {
                        listInt.Add(v.RowIndex);
                    }

                    string cmdID = dataGridView1.Rows[v.RowIndex].Cells["CMD_ID"].Value.ToString();
                    BCApp.SCApplication.TransferService.LocalCmdCancel(cmdID, "命令結束(本機)");
                }
                UpDate_CmdData();
            }
        }
        
        
        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result;

            result = MessageBox.Show("確定結束所選的命令?", "取消確認", MessageBoxButtons.YesNo);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                List<int> listInt = new List<int>();

                foreach (DataGridViewCell v in dataGridView1.SelectedCells)
                {
                    if (listInt.Contains(v.RowIndex))
                    {
                        continue;
                    }
                    else
                    {
                        listInt.Add(v.RowIndex);
                    }

                    string cmdID = dataGridView1.Rows[v.RowIndex].Cells["CMD_ID"].Value.ToString();
                    BCApp.SCApplication.TransferService.Manual_DeleteCmd(cmdID, "工程UI CMD_CST_DATA");
                }
                UpDate_CmdData();
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            label5.Text = BCApp.SCApplication.TransferService.Manual_InsertCmd(comboBox1.Text, comboBox2.Text);
            UpDate_CmdData();
        }
        private void button9_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.SetScanCmd(textBox1.Text, textBox2.Text, comboBox3.Text);
            UpDate_CmdData();
        }

        #endregion

        #region 卡匣操作
        public void UpDate_CstData()
        {
            dataGridView2.DataSource = BCApp.SCApplication.CassetteDataBLL.loadCassetteData().OrderBy(data => data.Carrier_LOC).ToList();
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView2.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }
        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult result;

            result = MessageBox.Show("確定刪除所選的帳料?", "刪除確認", MessageBoxButtons.YesNo);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                List<int> listInt = new List<int>();
                
                foreach (DataGridViewCell v in dataGridView2.SelectedCells)
                {
                    if (listInt.Contains(v.RowIndex))
                    {
                        continue;
                    }
                    else
                    {
                        listInt.Add(v.RowIndex);
                    }

                    string cstID = dataGridView2.Rows[v.RowIndex].Cells["CSTID"].Value.ToString();
                    string boxID = dataGridView2.Rows[v.RowIndex].Cells["BOXID"].Value.ToString();
                    label10.Text = BCApp.SCApplication.TransferService.Manual_DeleteCst(cstID, boxID);
                }
                UpDate_CstData();
            }            
        }        
        private void button7_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell v in dataGridView2.SelectedCells)
            {
                string loc = dataGridView2.Rows[v.RowIndex].Cells["Carrier_LOC"].Value.ToString();
                comboBox1.Text = loc;
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            label10.Text = BCApp.SCApplication.TransferService.Manual_InsertCassette(textBox1.Text, textBox2.Text, comboBox3.Text);
            UpDate_CstData();
        }
        private void button14_Click(object sender, EventArgs e) //刪除OHCV所有帳
        {
            foreach(var v in BCApp.SCApplication.TransferService.GetCVPort())
            {
                BCApp.SCApplication.TransferService.DeleteOHCVPortCst(v.PortName, "UI: CMD_CST_DATA");
            }

            UpDate_CstData();
        }
        private void button16_Click(object sender, EventArgs e) //SCAN 既有帳
        {
            BCApp.SCApplication.TransferService.ScanShelfCstData();
            UpDate_CmdData();
        }
        private void button15_Click(object sender, EventArgs e) //SCAN 全部
        {
            BCApp.SCApplication.TransferService.ScanALL();
            UpDate_CmdData();
        }
        #endregion

        #region 異常操作
        public void UpDate_AlarmData()
        {
            dataGridView3.DataSource = BCApp.SCApplication.AlarmBLL.loadSetAlarmList();
            dataGridView3.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView3.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }
        public void GetAllAlarmData()
        {
            dataGridView3.DataSource = BCApp.SCApplication.AlarmBLL.loadAllAlarmList();
            dataGridView3.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView3.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }
        private void button10_Click(object sender, EventArgs e)
        {
            UpDate_AlarmData();
        }
        private void button11_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell v in dataGridView3.SelectedCells)
            {
                string portName = dataGridView3.Rows[v.RowIndex].Cells["EQPT_ID"].Value.ToString();
                string alarmID = dataGridView3.Rows[v.RowIndex].Cells["ALAM_CODE"].Value.ToString();
                BCApp.SCApplication.TransferService.OHBC_AlarmCleared(portName, alarmID);
            }
            UpDate_AlarmData();
        }
        private void button13_Click(object sender, EventArgs e)
        {
            GetAllAlarmData();
        }



        #endregion

        private void button17_Click(object sender, EventArgs e)
        {
            DialogResult result;

            result = MessageBox.Show("確定重新建立所選的帳料?", "重新建帳確認", MessageBoxButtons.YesNo);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (DataGridViewCell v in dataGridView2.SelectedCells)
                {
                    if(string.IsNullOrWhiteSpace(dataGridView2.Rows[v.RowIndex].Cells["BOXID"].Value?.ToString() ?? "")
                    || string.IsNullOrWhiteSpace(dataGridView2.Rows[v.RowIndex].Cells["Carrier_LOC"].Value?.ToString() ?? "")
                      )
                    {
                        MessageBox.Show("BOXID 或 Carrier_LOC 輸入錯誤");
                        return;
                    }

                    string cstID = dataGridView2.Rows[v.RowIndex].Cells["CSTID"].Value?.ToString() ?? "";
                    string boxID = dataGridView2.Rows[v.RowIndex].Cells["BOXID"].Value.ToString();
                    string loc = dataGridView2.Rows[v.RowIndex].Cells["Carrier_LOC"].Value.ToString();
                    //string lotID = dataGridView2.Rows[v.RowIndex].Cells["LotID"].Value.ToString();
                    BCApp.SCApplication.TransferService.OHBC_InsertCassette(cstID, boxID, loc, "測試用 CMD_CST_DATA");
                }
                UpDate_CstData();
            }
        }

        private void button30_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.ReportBLL.ReportEmptyBoxSupply(numericUpDown1.Value.ToString(), comboBox4.Text);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            DialogResult result;

            result = MessageBox.Show("確定像 MCS 退掉所選的空BOX?", "退空 BOX 確認", MessageBoxButtons.YesNo);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                List<int> listInt = new List<int>();

                foreach (DataGridViewCell v in dataGridView2.SelectedCells)
                {
                    string cstID = dataGridView2.Rows[v.RowIndex].Cells["CSTID"].Value.ToString();
                    string boxID = dataGridView2.Rows[v.RowIndex].Cells["BOXID"].Value.ToString();

                    if(string.IsNullOrWhiteSpace(cstID) && listInt.Contains(v.RowIndex) == false)
                    {
                        BCApp.SCApplication.ReportBLL.ReportEmptyBoxRecycling(boxID);
                        listInt.Add(v.RowIndex);
                    }
                }
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            DialogResult result;

            result = MessageBox.Show("確定像 MCS 詢問LOTID?", "詢問LOTID", MessageBoxButtons.YesNo);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (CassetteData emptyLotIDdata in BCApp.SCApplication.CassetteDataBLL.LoadCassetteDataByCstAndEmptyLotID())
                {
                    BCApp.SCApplication.TransferService.QueryLotID(emptyLotIDdata);
                }

                //List<int> listInt = new List<int>();
                //foreach (DataGridViewCell v in dataGridView2.SelectedCells)
                //{
                //    string cstID = dataGridView2.Rows[v.RowIndex].Cells["CSTID"].Value.ToString();
                //    //string boxID = dataGridView2.Rows[v.RowIndex].Cells["BOXID"].Value.ToString();

                //    if (listInt.Contains(v.RowIndex) == false)
                //    {
                //        BCApp.SCApplication.ReportBLL.ReportQueryLotID(cstID);
                //        listInt.Add(v.RowIndex);
                //    }
                //}
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            DialogResult result;

            result = MessageBox.Show("確定像 Redis 詢問 CSTID?", "詢問 CSTID", MessageBoxButtons.YesNo);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (CassetteData unkCstData in BCApp.SCApplication.CassetteDataBLL.LoadCassetteDataByCSTID_UNK())
                {
                    BCApp.SCApplication.TransferService.Redis_UpdateCstID(unkCstData);
                }
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.requireEmptyBox = true;
        }

        private void button22_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.requireEmptyBox = false;
        }

        private void button23_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.redisEnable = true;
        }

        private void button24_Click(object sender, EventArgs e)
        {
            BCApp.SCApplication.TransferService.redisEnable = false;
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            DialogResult result;

            result = MessageBox.Show("確定設定所選的命令的優先度?", "取消確認", MessageBoxButtons.YesNo);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                List<int> listInt = new List<int>();

                foreach (DataGridViewCell v in dataGridView1.SelectedCells)
                {
                    if (listInt.Contains(v.RowIndex))
                    {
                        continue;
                    }
                    else
                    {
                        listInt.Add(v.RowIndex);
                    }

                    string cmdID = dataGridView1.Rows[v.RowIndex].Cells["CMD_ID"].Value.ToString();
                    BCApp.SCApplication.TransferService.Manual_UpDateCmdPriority(cmdID, (int)numericUpDown2.Value, "工程UI CMD_CST_DATA");
                }
                UpDate_CmdData();
            }
        }
    }
}
