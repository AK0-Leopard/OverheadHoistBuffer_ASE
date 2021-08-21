using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.mirle.ibg3k0.bc.winform.UI
{
    public partial class AlarmEnableForm : Form
    {
        public AlarmEnableForm(BCMainForm mainForm)
        {
            InitializeComponent();

            var alarm_map = mainForm.BCApp.SCApplication.AlarmBLL.loadAlarmMaps();
            var eq_ids = alarm_map.Select(a => a.EQPT_REAL_ID).Distinct();
            List<string> eqs = new List<string>();
            eqs.Add("");
            eqs.AddRange(eq_ids);
            cb_eqType.DataSource = eqs;
            MainForm = mainForm;
        }

        public BCMainForm MainForm { get; }

        private async void cb_eqType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected_eq_id = cb_eqType.Text;
            List<sc.Data.VO.AlarmMap> alarmMaps = null;
            await Task.Run(() => alarmMaps = MainForm.BCApp.SCApplication.AlarmBLL.loadAlarmMaps(selected_eq_id));
            dgv_alarmList.DataSource = alarmMaps;
        }

        private void AlarmEnableForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainForm.removeForm(typeof(AlarmEnableForm).Name);
        }

        const int IS_REPORT_CHECK_BOX_COLUMN_INDEX = 4;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                var dgv = sender as DataGridView;
                if (dgv == null) return;
                var check_box_column_valus = (bool)dgv[e.ColumnIndex, e.RowIndex].Value;
                if (check_box_column_valus)
                {
                    DialogResult confirmResult = MessageBox.Show(this, $"Do you want to enable this alarm to report?",
                        App.BCApplication.getMessageString("CONFIRM"), MessageBoxButtons.YesNo);
                    if (confirmResult == DialogResult.Yes)
                    {

                    }
                }
                else
                {
                    DialogResult confirmResult = MessageBox.Show(this, $"Do you want to disable this alarm to report?",
                        App.BCApplication.getMessageString("CONFIRM"), MessageBoxButtons.YesNo);
                    if (confirmResult == DialogResult.Yes)
                    {

                    }
                }
            }
        }
    }
}
