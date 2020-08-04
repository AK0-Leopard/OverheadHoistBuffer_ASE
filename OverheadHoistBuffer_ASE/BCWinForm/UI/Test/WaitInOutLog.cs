using com.mirle.ibg3k0.sc;
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
    public partial class WaitInOutLog : Form
    {
        App.BCApplication BCApp;
        ALINE line = null;

        public WaitInOutLog()
        {
            InitializeComponent();
        }
        public void SetApp(App.BCApplication app)
        {
            BCApp = app;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //dataGridView1.DataSource = BCApp.SCApplication.TransferService.waitInLog.Values.ToList();
            //dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            //dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            //dataGridView2.DataSource = BCApp.SCApplication.TransferService.waitOutLog.Values.ToList();
            //dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            //dataGridView2.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void WaitInOutLog_Load(object sender, EventArgs e)
        {

        }
    }
}
