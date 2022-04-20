using com.mirle.ibg3k0.bc.winform.App;
using com.mirle.ibg3k0.bc.winform.Common;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.mirle.ibg3k0.bc.winform.UI
{
    public partial class VehicleMaintenanceForm : Form
    {
        BCMainForm MainForm;
        SCApplication scApp;


        public VehicleMaintenanceForm(BCMainForm _mainForm)
        {
            InitializeComponent();
            MainForm = _mainForm;
            scApp = _mainForm.BCApp.SCApplication;

            initialCombobox();

            if (sc.Common.SCUtility.isMatche(MainForm.BCApp.LoginUserID, BCAppConstants.ADMIN_USER_NAME))
            {
                btn_resetODO.Enabled = true;
                btn_resetGripCount.Enabled = true;
            }
        }

        private void initialCombobox()
        {
            List<string> lstVh = new List<string>();
            lstVh.Add(string.Empty);
            lstVh.AddRange(scApp.VehicleBLL.cache.loadVhs().Select(vh => vh.VEHICLE_ID).ToList());
            Common.BCUtility.setComboboxDataSource(cmb_VhID, lstVh.ToArray());


        }



        private void CarrierMaintenanceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainForm.removeForm(this.Name);
        }


        double METER2KM = 1000;
        private void cmb_VhID_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshVhMantInfo();
        }

        private void refreshVhMantInfo()
        {
            string vh_id = cmb_VhID.Text;
            var vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            //里程資訊
            double VEHICLE_ACC_DIST2Show = Math.Round((vh.VEHICLE_ACC_DIST / METER2KM), 2, MidpointRounding.AwayFromZero);
            double MANT_ACC_DIST2Show = Math.Round((vh.MANT_ACC_DIST / METER2KM), 2, MidpointRounding.AwayFromZero);
            string MANT_ACC_DATETIME = "";
            if (vh.MANT_DATE.HasValue)
            {
                MANT_ACC_DATETIME = vh.MANT_DATE.Value.ToString(SCAppConstants.DateTimeFormat_19);
            }

            //夾爪資訊
            int grip_count = vh.GRIP_COUNT;
            int grip_mant_count = vh.GRIP_MANT_COUNT;
            string grip_mant_date_time = "";
            if (vh.GRIP_MANT_DATE.HasValue)
            {
                grip_mant_date_time = vh.GRIP_MANT_DATE.Value.ToString(SCAppConstants.DateTimeFormat_19);
            }

            txt_vhACCDist.Text = $"{VEHICLE_ACC_DIST2Show.ToString()} km";
            txt_mantACCDist.Text = $"{MANT_ACC_DIST2Show.ToString()} km";
            txt_lastMantACCDateTime.Text = MANT_ACC_DATETIME;

            txt_gripCount.Text = grip_count.ToString();
            txt_gripMantCount.Text = grip_mant_count.ToString();
            txt_gripMantDateTime.Text = grip_mant_date_time;
        }

        private async void btn_resetODO_Click(object sender, EventArgs e)
        {
            try
            {
                string vh_id = cmb_VhID.Text;
                btn_resetODO.Enabled = false;

                string message = $"是否要重置 OHT:{vh_id} 的保養累積里程?";
                DialogResult confirmResult = MessageBox.Show(this, message,
                    BCApplication.getMessageString("CONFIRM"), MessageBoxButtons.YesNo);

                BCUtility.recordAction(MainForm.BCApp, this.Name, message, confirmResult.ToString());
                if (confirmResult != System.Windows.Forms.DialogResult.Yes)
                {
                    return;
                }


                await Task.Run(() => scApp.VehicleService.ResetMantAccDist(vh_id));
                refreshVhMantInfo();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Warn(ex, "Exception");
            }
            finally
            {
                btn_resetODO.Enabled = true;
            }
        }

        private async void btn_resetGripCount_Click(object sender, EventArgs e)
        {
            try
            {
                string vh_id = cmb_VhID.Text;
                btn_resetGripCount.Enabled = false;

                string message = $"是否要重置 OHT:{vh_id} 的夾爪保養累積次數?";
                DialogResult confirmResult = MessageBox.Show(this, message,
                    BCApplication.getMessageString("CONFIRM"), MessageBoxButtons.YesNo);

                BCUtility.recordAction(MainForm.BCApp, this.Name, message, confirmResult.ToString());
                if (confirmResult != System.Windows.Forms.DialogResult.Yes)
                {
                    return;
                }

                await Task.Run(() => scApp.VehicleService.ResetGripMantCount(vh_id));
                refreshVhMantInfo();

            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Warn(ex, "Exception");
            }
            finally
            {
                btn_resetGripCount.Enabled = true;
            }
        }
    }
}
