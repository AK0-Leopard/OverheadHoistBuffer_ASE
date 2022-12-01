using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.IO;
using GenericParsing;
using System.Xml;

using Quartz;
using Quartz.Impl;

namespace AlarmReportMaker
{
    public partial class Form1 : Form
    {
        List<AlarmDetailReport> alarmReports;
        string filePath;
        jobSchedule JobSchedule;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            loadConfig();
            iniJob(TB_reportMakeInterval.Text.Trim());
        }

        private void loadConfig()
        {
            XmlDocument xml = new XmlDocument();
            XmlNode commonNode;
            XmlNodeList reportNode;
            xml.Load("config\\config.xml");
            commonNode = xml.SelectSingleNode("AlarmReportMaker");
            reportNode = commonNode.SelectSingleNode("Reports").SelectNodes("Report");
            filePath = commonNode.SelectSingleNode("ReportOutputPath").InnerText;
            TB_ReportDataLength.Text = commonNode.SelectSingleNode("ReportDataLength_Days").InnerText;
            TB_reportMakeInterval.Text = commonNode.SelectSingleNode("ReportMakeInterval_Hour").InnerText;

            alarmReports = new List<AlarmDetailReport>();
            foreach (XmlNode node in reportNode)
            {
                XmlElement xe = node as XmlElement;
                string config = xe.GetAttribute("Config");
                string script;
                using (StreamReader sr = new StreamReader("config\\" + config + "\\script.txt"))
                {
                    script = sr.ReadToEnd();
                }

                DataTable dt_alarmClassification = loadCSVToDataset("config\\" + config + "\\ALARMMODULE.csv");
                DataTable dt_alarmMap = loadCSVToDataset("config\\" + config + "\\ALARMMAP.csv");
                DataTable dt_eqMap = loadCSVToDataset("config\\" + config + "\\EQMAP.csv");



                AlarmDetailReport alarm = new AlarmDetailReport(
                    xe.GetAttribute("Title"),
                    xe.GetAttribute("DB_IP"),
                    xe.GetAttribute("DB_UserID"),
                    xe.GetAttribute("DB_Password"),
                    script,
                    xe.GetAttribute("schema"),
                    Convert.ToInt32(TB_ReportDataLength.Text),
                    dt_alarmMap,
                    dt_eqMap,
                    dt_alarmClassification
                    );
                alarmReports.Add(alarm);
            }
            dgv_DataSource.DataSource = alarmReports;
            dgv_DataSource.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
        private async void iniJob(string HH)
        {
            JobSchedule = jobSchedule.getInstance($"0 30 {HH} * * ?");
            jobSchedule.jobWorking += makeReport;
        }
        private List<MaintenanceAlarm> getReport(string title)
        {
            var report = alarmReports.FirstOrDefault(x => x.title == title);
            if (report != null)
                return report.getReport();
            else
                return null;
        }

        private void makeReport()
        {
            DateTime dt = DateTime.Now;
            foreach(var alarmReport in alarmReports)
            {
                List<MaintenanceAlarm> report = alarmReport.getReport();
                string path = filePath + dt.ToString("yyyy-MM-dd hh") + "\\"  + alarmReport.title + ".xlsx";
                //建立 xlxs 轉換物件
                XSLXHelper helper = new XSLXHelper();
                //取得轉為 xlsx 的物件
                var xlsx = helper.Export(report);
                //存檔至指定位置
                xlsx.SaveAs(path);
            }
        }

        private void makeReport(object o, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            foreach (var alarmReport in alarmReports)
            {
                List<MaintenanceAlarm> report = alarmReport.getReport();
                string path = filePath + dt.ToString("yyyy-MM-dd") + "\\" + alarmReport.title + "_" + dt.ToString("yyyy_MM_dd HH_mm_ss") + ".xlsx";
                //建立 xlxs 轉換物件
                XSLXHelper helper = new XSLXHelper();
                //取得轉為 xlsx 的物件
                var xlsx = helper.Export(report);
                //存檔至指定位置
                xlsx.SaveAs(path);
            }
        }


        private void saveConfig()
        {
            XmlDocument xml = new XmlDocument();
            XmlNode dbNode;
            //讀取
            xml.Load("config\\config.xml");
            xml.SelectSingleNode("AlarmReportMaker").SelectSingleNode("ReportDataLength_Days").InnerText = TB_ReportDataLength.Text;
            xml.SelectSingleNode("AlarmReportMaker").SelectSingleNode("ReportMakeInterval_Hour").InnerText = TB_reportMakeInterval.Text;
            xml.Save("config\\config.xml");
            foreach (var alarm in alarmReports)
            {
                alarm.reportDataLength = Convert.ToInt32(TB_ReportDataLength.Text);
            }
        }

        private DataTable loadCSVToDataset(string csvPath)
        {
            if (!File.Exists(csvPath)) return null;
            using (GenericParser parser = new GenericParser())
            {
                parser.SetDataSource(csvPath, System.Text.Encoding.Default);
                parser.ColumnDelimiter = ',';
                parser.FirstRowHasHeader = true;
                parser.MaxBufferSize = 1024;
                DataTable dt = new DataTable();

                bool isfirst = true;
                while (parser.Read())
                {

                    int cs = parser.ColumnCount;
                    if (isfirst)
                    {

                        for (int i = 0; i < cs; i++)
                        {
                            dt.Columns.Add(parser.GetColumnName(i), typeof(string));
                        }
                        isfirst = false;
                    }


                    DataRow dr = dt.NewRow();

                    for (int i = 0; i < cs; i++)
                    {
                        string val = parser[i];
                        dr[i] = val;
                    }
                    dt.Rows.Add(dr);
                }
                return dt;
            }
        }

        private void dgv_DataSource_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var v = getReport(dgv_DataSource.SelectedCells[0].Value.ToString());
            if(v != null)
            {
                dgv_dataView.DataSource = null;
                dgv_dataView.DataSource = v;
            }
            else
            {
                dgv_dataView.DataSource = null;
                dgv_dataView.Rows.Clear();
            }
        }

        private void BT_save_Click(object sender, EventArgs e)
        {
            saveConfig();
        }

        private void TB_reportMakeInterval_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)48 || e.KeyChar == (Char)49 ||
               e.KeyChar == (Char)50 || e.KeyChar == (Char)51 ||
               e.KeyChar == (Char)52 || e.KeyChar == (Char)53 ||
               e.KeyChar == (Char)54 || e.KeyChar == (Char)55 ||
               e.KeyChar == (Char)56 || e.KeyChar == (Char)57 || e.KeyChar == (Char)8)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
                MessageBox.Show("請確實輸入數字");
                TB_reportMakeInterval.Text = "24";
            }
        }

        private void TB_ReportDataLength_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)48 || e.KeyChar == (Char)49 ||
               e.KeyChar == (Char)50 || e.KeyChar == (Char)51 ||
               e.KeyChar == (Char)52 || e.KeyChar == (Char)53 ||
               e.KeyChar == (Char)54 || e.KeyChar == (Char)55 ||
               e.KeyChar == (Char)56 || e.KeyChar == (Char)57 || e.KeyChar == (Char)8)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
                MessageBox.Show("請確實輸入數字");
                TB_ReportDataLength.Text = "3";
            }
        }

        private void BT_output_Click(object sender, EventArgs e)
        {
            makeReport();
        }
    }
}
