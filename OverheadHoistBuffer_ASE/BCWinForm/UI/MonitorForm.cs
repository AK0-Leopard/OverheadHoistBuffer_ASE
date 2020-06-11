using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.Data.VO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace com.mirle.ibg3k0.bc.winform.UI
{
    public partial class MonitorForm : Form
    {
        BCMainForm form = null;
        string[] allVh = null;
        string[] allSec = null;

        public MonitorForm(BCMainForm _form)
        {
            InitializeComponent();
            form = _form;
            uctlCommStatus1.start(_form);
            uctlTcpIpAgentStatus1.start(_form);
            timer1.Start();
            form.entryMonitorMode();

            chart1.ChartAreas.Add("Base");
            chart1.Series.Add("Level1");
            chart1.Titles.Add("Wifi communication quality (Vehicle)");

            chart2.ChartAreas.Add("Base");
            chart2.Series.Add("Level1");
            chart2.Titles.Add("Wifi communication quality (Section)");

            chart3.ChartAreas.Add("Base");
            chart3.Series.Add("Level1");
            chart3.Titles.Add("Wifi communication quality");

            ChartArea myArea = chart1.ChartAreas["Base"];
            Series mySeries = chart1.Series["Level1"];

            ChartArea myArea2 = chart2.ChartAreas["Base"];
            Series mySeries2 = chart2.Series["Level1"];

            ChartArea myArea3 = chart3.ChartAreas["Base"];
            Series mySeries3 = chart3.Series["Level1"];



            myArea.AxisX.MajorGrid.LineColor = Color.Transparent; // 主軸的 橫線設為透明
            myArea.AxisY.MajorGrid.LineColor = Color.Transparent; // 主軸的 縱線設為透明
            myArea.AxisX.Title = "Vehicle ID";
            myArea.AxisY.Title = "Acknowledge time(ms)";

            myArea2.AxisX.MajorGrid.LineColor = Color.Transparent; // 主軸的 橫線設為透明
            myArea2.AxisY.MajorGrid.LineColor = Color.Transparent; // 主軸的 縱線設為透明
            myArea2.AxisX.Title = "Section ID";
            myArea2.AxisY.Title = "Acknowledge time(ms)";

            myArea3.AxisX.MajorGrid.LineColor = Color.Transparent; // 主軸的 橫線設為透明
            myArea3.AxisY.MajorGrid.LineColor = Color.Transparent; // 主軸的 縱線設為透明
            myArea3.AxisX.Title = "Time sequence";
            myArea3.AxisY.Title = "Acknowledge time(ms)";



            //StripLine lineMean1 = new StripLine();
            //// 設定平均值的 Line
            //lineMean1.Text = "Mean";
            //lineMean1.BorderColor = Color.Blue; // 線條的顏色
            //lineMean1.BorderWidth = 2;

            //StripLine lineMean2 = new StripLine();
            //// 設定平均值的 Line
            //lineMean2.Text = "Mean";
            //lineMean2.BorderColor = Color.Blue; // 線條的顏色
            //lineMean2.BorderWidth = 2;



            //myArea.AxisY.StripLines.Add(lineMean1);
            mySeries.ChartType = SeriesChartType.Point;

            //myArea2.AxisY.StripLines.Add(lineMean1);
            myArea2.AxisX.Interval = 1;
            myArea2.AxisX.IntervalOffset = 1;
            myArea2.AxisX.LabelStyle.IsStaggered = true;
            mySeries2.ChartType = SeriesChartType.Point;

            mySeries3.ChartType = SeriesChartType.Spline;

            List<string> lstVh = new List<string>();
            lstVh.AddRange(form.BCApp.SCApplication.VehicleBLL.loadAllVehicle().Select(vh => vh.VEHICLE_ID).ToList());
            allVh = lstVh.ToArray();
            allSec = form.BCApp.SCApplication.MapBLL.loadAllSectionID().ToArray();

            cmb_vh_id.DataSource = allVh;
            cmb_vh_id.AutoCompleteCustomSource.AddRange(allVh);
            cmb_vh_id.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmb_vh_id.AutoCompleteSource = AutoCompleteSource.ListItems;

            cmb_sec_id.DataSource = allSec;
            cmb_sec_id.AutoCompleteCustomSource.AddRange(allSec);
            cmb_sec_id.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmb_sec_id.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private void drawVhAndPingTimePlot()
        {
            Dictionary<string, List<ANETWORKQUALITY>> dicVhAndnetwork =
                form.BCApp.SCApplication.NetWorkQualityBLL.loadNetworkQualityBySecGroupByVhID(cmb_sec_id.Text);
            //標題 最大數值
            int PointIndex = 0;
            this.chart1.Series["Level1"].Points.Clear();
            for (int i = 0; i < allVh.Count(); i++)
            {
                if (!dicVhAndnetwork.ContainsKey(allVh[i]))
                {
                    PointIndex = this.chart1.Series["Level1"].Points.AddXY(i, 0);
                    this.chart1.Series["Level1"].Points[PointIndex].AxisLabel = allVh[i];
                }
                else
                {
                    List<ANETWORKQUALITY> networks = dicVhAndnetwork[allVh[i]];
                    foreach (ANETWORKQUALITY network in networks)
                    {
                        PointIndex = this.chart1.Series["Level1"].Points.AddXY(i, network.PING_TIME);
                        this.chart1.Series["Level1"].Points[PointIndex].AxisLabel = allVh[i];
                    }
                }
            }
        }

        private void drawSecAndPingTimePlot()
        {
            Dictionary<string, List<ANETWORKQUALITY>> dicSecAndnetwork =
                form.BCApp.SCApplication.NetWorkQualityBLL.loadNetworkQualityByVhIDGroupBySecID(cmb_vh_id.Text);
            //標題 最大數值
            this.chart2.Series["Level1"].Points.Clear();
            int PointIndex = 0;
            for (int i = 0; i < allSec.Count(); i++)
            {
                string sec_id = allSec[i].Trim();
                if (!dicSecAndnetwork.ContainsKey(sec_id))
                {
                    PointIndex = this.chart2.Series["Level1"].Points.AddXY(i, 0);
                    this.chart2.Series["Level1"].Points[PointIndex].AxisLabel = sec_id;
                }
                else
                {
                    List<ANETWORKQUALITY> networks = dicSecAndnetwork[sec_id];
                    foreach (ANETWORKQUALITY network in networks)
                    {
                        PointIndex = this.chart2.Series["Level1"].Points.AddXY(i, network.PING_TIME);
                        this.chart2.Series["Level1"].Points[PointIndex].AxisLabel = sec_id;
                    }
                }
            }
        }

        private void drawVhSecPingTimePlot()
        {
            List<ANETWORKQUALITY> networks =
                form.BCApp.SCApplication.NetWorkQualityBLL.loadNetworkQualityByVhID(cmb_vh_id.Text);
            //標題 最大數值
            this.chart3.Series["Level1"].Points.Clear();
            int PointIndex = 0;
            for (int i = 0; i < networks.Count; i++)
            {
                PointIndex = this.chart3.Series["Level1"].Points.AddXY(i, networks[i].PING_TIME);
                this.chart3.Series["Level1"].Points[PointIndex].AxisLabel = " ";
            }
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            uctlCommStatus1.refresh();
            uctlTcpIpAgentStatus1.refresh();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        private void MonitorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            form.LeaveMonitorMode();
        }

        private void cmb_sec_id_SelectedIndexChanged(object sender, EventArgs e)
        {
            drawVhAndPingTimePlot();
        }

        private void cmb_vh_id_SelectedIndexChanged(object sender, EventArgs e)
        {
            drawSecAndPingTimePlot();
            drawVhSecPingTimePlot();
        }

    }
}
