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
            await refreshAlarmMap();
        }

        private async Task refreshAlarmMap()
        {
            string selected_eq_id = cb_eqType.Text;
            string error_code = txt_alarmCode.Text;

            List<sc.Data.VO.AlarmMap> alarmMaps = null;
            await Task.Run(() => alarmMaps = MainForm.BCApp.SCApplication.AlarmBLL.loadAlarmMaps(selected_eq_id));
            var alarm_map_to_show = alarmMaps.Select(a => new sc.Data.VO.AlarmMapToShow(MainForm.BCApp.SCApplication.AlarmBLL, a)).ToList();
            if (!sc.Common.SCUtility.isEmpty(error_code))
            {
                alarm_map_to_show = alarm_map_to_show.Where(a => a.ALARM_ID.Contains(error_code)).ToList();
            }
            dgv_alarmList.DataSource = alarm_map_to_show;
        }

        private void AlarmEnableForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainForm.removeForm(typeof(AlarmEnableForm).Name);
        }

        const int IS_REPORT_CHECK_BOX_COLUMN_INDEX = 4;
        const int EQ_ID_COLUMN_INDEX = 0;
        const int ALARM_CODE_COLUMN_INDEX = 1;
        private async void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == IS_REPORT_CHECK_BOX_COLUMN_INDEX)
            {
                var dgv = sender as DataGridView;
                if (dgv == null) return;
                var check_box_column_valus = (bool)dgv[e.ColumnIndex, e.RowIndex].Value;
                string eq_id = dgv[EQ_ID_COLUMN_INDEX, e.RowIndex].Value as string;
                string alarm_code = dgv[ALARM_CODE_COLUMN_INDEX, e.RowIndex].Value as string;
                if (!check_box_column_valus)
                {
                    DialogResult confirmResult = MessageBox.Show(this, $"Do you want to enable this alarm to report?",
                        App.BCApplication.getMessageString("CONFIRM"), MessageBoxButtons.YesNo);
                    if (confirmResult == DialogResult.Yes)
                    {
                        bool is_success = false;
                        await Task.Run(() => is_success = MainForm.BCApp.SCApplication.AlarmBLL.enableAlarmReport(eq_id, alarm_code, true));
                        if (is_success)
                        {
                            MessageBox.Show(this, $"Enable success.",
                                            "Alarm Enable", MessageBoxButtons.OK);
                        }
                        else
                        {
                            MessageBox.Show(this, $"Enable fail.",
                                            "Alarm Enable", MessageBoxButtons.OK);
                        }
                    }
                }
                else
                {
                    DialogResult confirmResult = MessageBox.Show(this, $"Do you want to disable this alarm to report?",
                        App.BCApplication.getMessageString("CONFIRM"), MessageBoxButtons.YesNo);
                    AlarmEnablePopupForm loginForm = new AlarmEnablePopupForm(eq_id, alarm_code);
                    System.Windows.Forms.DialogResult result = loginForm.ShowDialog(this);
                    if (result != DialogResult.OK)
                    {
                        return;
                    }
                    string user_id = loginForm.getUserID();
                    string reason = loginForm.getReason();
                    if (sc.Common.SCUtility.isEmpty(user_id))
                    {
                        MessageBox.Show(this, $"Disable fail. Please enter user id. ",
                                        "Alarm Disable", MessageBoxButtons.OK);
                        return;
                    }
                    if (sc.Common.SCUtility.isEmpty(reason))
                    {
                        MessageBox.Show(this, $"Disable fail. Please enter reason. ",
                                        "Alarm Disable", MessageBoxButtons.OK);
                        return;
                    }
                    if (confirmResult == DialogResult.Yes)
                    {
                        bool is_success = false;
                        await Task.Run(() => is_success = MainForm.BCApp.SCApplication.AlarmBLL.enableAlarmReport(eq_id, alarm_code, false));
                        if (is_success)
                        {
                            MessageBox.Show(this, $"Disable success.",
                                            "Alarm Disable", MessageBoxButtons.OK);
                        }
                        else
                        {
                            MessageBox.Show(this, $"Disable fail.",
                                            "Alarm Disable", MessageBoxButtons.OK);
                        }
                    }
                }
            }
        }

        private async void txt_alarmCode_TextChanged(object sender, EventArgs e)
        {
            await refreshAlarmMap();
        }
    }
}