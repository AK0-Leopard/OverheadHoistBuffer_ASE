using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.ObjectRelay;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.mirle.ibg3k0.bc.winform.UI
{
    public partial class HistoryAlarmsForm : Form
    {
        BCMainForm mainform;
        List<VALARM> valarmList = null;
        List<ALARMObjToShow> showAlarmList = null;

        public HistoryAlarmsForm(BCMainForm _mainForm)
        {
            InitializeComponent();
            dgv_TransferCommand.AutoGenerateColumns = false;
            mainform = _mainForm;
            showAlarmList = new List<ALARMObjToShow>();
            dgv_TransferCommand.DataSource = showAlarmList;

            var device_ids = new List<string>();
            device_ids.Add("");
            var port_ids = _mainForm.BCApp.SCApplication.TransferService.GetCVPort().Select(s => s.PortName);
            var crane_ids = _mainForm.BCApp.SCApplication.TransferService.GetCRANEPort().Select(s => s.PortName);
            device_ids.AddRange(crane_ids);
            device_ids.AddRange(port_ids);
            m_EqptIDCbx.DataSource = device_ids;
            m_StartDTCbx.Value = DateTime.Today;
            m_EndDTCbx.Value = DateTime.Now;
        }

        const int MAX_ALALM_QUERY_COUNT = 10000;
        DateTime preStartDateTime = DateTime.MinValue;
        DateTime preEndDateTime = DateTime.MinValue;
        private async void updateAlarms()
        {
            DateTime start_time = m_StartDTCbx.Value;
            DateTime end_time = m_EndDTCbx.Value;
            string alarm_code = m_AlarmCodeTbl.Text;
            string device_id = m_EqptIDCbx.Text;
            string box_id = txt_boxID.Text;
            string cst_id = txt_CSTID.Text;
            string lot_id = txt_LotID.Text;
            if (preStartDateTime != start_time || preEndDateTime != end_time)
            {
                try
                {
                    tableLayoutPanel6.Enabled = false;
                    int alarms_count = 0;
                    await Task.Run(() =>
                    {
                        alarms_count = mainform.BCApp.SCApplication.VAlarmBLL.getAlarmCount(start_time, end_time);
                    });
                    if (alarms_count > MAX_ALALM_QUERY_COUNT)
                    {
                        MessageBox.Show(this, $"Alarm query 數量超過:{MAX_ALALM_QUERY_COUNT}，請重新調整搜尋區間。"
                                        , "Alarm Query"
                                        , MessageBoxButtons.OK
                                        , MessageBoxIcon.Information);
                        return;
                    }

                    await Task.Run(() =>
                     {
                         var alarms = mainform.BCApp.SCApplication.VAlarmBLL.loadAlarms(start_time, end_time);
                         valarmList = alarms.ToList();
                     });
                    preStartDateTime = start_time;
                    preEndDateTime = end_time;
                }
                catch (Exception ex)
                {
                    return;
                }
                finally
                {
                    tableLayoutPanel6.Enabled = true;
                }
            }

            try
            {
                //tableLayoutPanel6.Enabled = false;
                await Task.Run(() =>
                 {

                     if (valarmList != null && valarmList.Count > 0)
                     {
                         var alarm_temp = valarmList.ToList();
                         if (!SCUtility.isEmpty(alarm_code))
                         {
                             alarm_temp = alarm_temp.Where(alarm => alarm.ALAM_CODE.Contains(alarm_code)).ToList();
                         }
                         if (!SCUtility.isEmpty(device_id))
                         {
                             alarm_temp = alarm_temp.Where(alarm => SCUtility.isMatche(alarm.EQPT_ID, device_id)).ToList();
                         }
                         if (!SCUtility.isEmpty(box_id))
                         {
                             alarm_temp = alarm_temp.Where(alarm => alarm.BOX_ID != null && alarm.BOX_ID.Contains(box_id)).ToList();
                         }
                         if (!SCUtility.isEmpty(cst_id))
                         {
                             alarm_temp = alarm_temp.Where(alarm => alarm.CARRIER_ID != null && alarm.CARRIER_ID.Contains(cst_id)).ToList();
                         }
                         if (!SCUtility.isEmpty(lot_id))
                         {
                             alarm_temp = alarm_temp.Where(alarm => alarm.LOT_ID != null && alarm.LOT_ID.Contains(lot_id)).ToList();
                         }
                         showAlarmList = alarm_temp.Select(alarm => new ALARMObjToShow(alarm)).ToList();
                     }
                     else
                     {
                         showAlarmList = new List<ALARMObjToShow>();
                     }
                 });
                dgv_TransferCommand.DataSource = showAlarmList;
                dgv_TransferCommand.Refresh();

            }
            catch (Exception ex)
            {

            }
            finally
            {
                //tableLayoutPanel6.Enabled = true;
            }
        }

        private void TransferCommandQureyListForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainform.removeForm(this.Name);
        }

        private void btnlSearch_Click(object sender, EventArgs e)
        {
            updateAlarms();
        }

        private async void m_exportBtn_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "Alarm files (*.xlsx)|*.xlsx";
                if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK || bcf.Common.BCFUtility.isEmpty(dlg.FileName))
                {
                    return;
                }
                string filename = dlg.FileName;
                //建立 xlxs 轉換物件
                Common.XSLXHelper helper = new Common.XSLXHelper();
                //取得轉為 xlsx 的物件
                ClosedXML.Excel.XLWorkbook xlsx = null;
                await Task.Run(() => xlsx = helper.Export(showAlarmList));
                if (xlsx != null)
                    xlsx.SaveAs(filename);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Warn(ex, "Exception");
            }
        }

        private void m_AlarmCodeTbl_TextChanged(object sender, EventArgs e)
        {
            updateAlarms();
        }

        private void m_EqptIDCbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateAlarms();
        }

        private void txt_boxID_TextChanged(object sender, EventArgs e)
        {
            updateAlarms();
        }

        private void txt_CSTID_TextChanged(object sender, EventArgs e)
        {
            updateAlarms();
        }

        private void txt_LotID_TextChanged(object sender, EventArgs e)
        {
            updateAlarms();
        }

        private void dgv_TransferCommand_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }
    }
}
