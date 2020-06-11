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
    public partial class CarrierMaintenanceForm : Form
    {
        BCMainForm MainForm;
        SCApplication scApp;


        public CarrierMaintenanceForm(BCMainForm _mainForm)
        {
            InitializeComponent();
            MainForm = _mainForm;
            scApp = _mainForm.BCApp.SCApplication;

            initialCombobox();
        }

        private void initialCombobox()
        {
            List<string> lstVh = new List<string>();
            lstVh.Add(string.Empty);
            lstVh.AddRange(scApp.VehicleBLL.cache.loadVhs().Select(vh => vh.VEHICLE_ID).ToList());
            Common.BCUtility.setComboboxDataSource(cmb_InstalledVhID, lstVh.ToArray());
            Common.BCUtility.setComboboxDataSource(cmb_RemoveVhID, lstVh.ToArray());

            List<string> lstTransferPort = new List<string>();
            lstTransferPort.Add(string.Empty);
            lstTransferPort.AddRange(scApp.PortStationBLL.OperateCatch.loadPortStations().
                           Select(port_station => port_station.PORT_ID).ToList());
            Common.BCUtility.setComboboxDataSource(cmb_installedTransferPortID, lstTransferPort.ToArray());
            Common.BCUtility.setComboboxDataSource(cmb_removeTransferPortID, lstTransferPort.ToArray());

        }

        const int CARRIER_ID_FIX_LENGTH = 17;
        private async void btn_installed_Click(object sender, EventArgs e)
        {
            try
            {
                string vh_id = cmb_InstalledVhID.Text;
                string cst_id = txt_InstalledCSTID.Text;
                string transfer_port = cmb_installedTransferPortID.Text;
                AVEHICLE vh = scApp.VehicleBLL.cache.getVhByID(vh_id);
                if (vh == null)
                {
                    MessageBox.Show($"Plaese select vh.", "Carrier installed fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (vh.HAS_CST == 0)
                {
                    MessageBox.Show($"vh:{vh_id} carrier not exist.can't installed", "Carrier installed fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (SCUtility.isEmpty(transfer_port))
                {
                    MessageBox.Show($"Plaese select transfer port.", "Carrier installed fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cst_id.Length == CARRIER_ID_FIX_LENGTH)
                {
                    MessageBox.Show($"Plaese check carrier id of length, it need {CARRIER_ID_FIX_LENGTH} words", "Carrier installed fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                txt_InstalledCSTID.Text = SCUtility.Trim(vh.CST_ID, true);
                btn_installed.Enabled = false;
                await Task.Run(() =>
                {
                    List<AMCSREPORTQUEUE> reportqueues = new List<AMCSREPORTQUEUE>();
                    scApp.ReportBLL.newReportCarrierInstalled(vh.Real_ID, cst_id, transfer_port, reportqueues);
                    scApp.ReportBLL.insertMCSReport(reportqueues);
                    scApp.ReportBLL.newSendMCSMessage(reportqueues);
                });

                Common.LogHelper.Log(logger: NLog.LogManager.GetCurrentClassLogger(), LogLevel: LogLevel.Info, Class: nameof(CarrierMaintenanceForm), Device: "OHTC",
                  Data: $"Manual carrier installed success, vh id:{vh_id}, cst id:{cst_id}, transfer port:{transfer_port}");

                MessageBox.Show("Carrier installed success", "Carrier installed success.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await Task.Run(() =>
                {
                    SpinWait.SpinUntil(() => false, 60000);
                });
            }
            catch { }
            finally
            {
                btn_installed.Enabled = true;
            }
        }

        private async void btn_remove_Click(object sender, EventArgs e)
        {
            try
            {
                string vh_id = cmb_RemoveVhID.Text;
                string cst_id = txt_RemoveCSTID.Text;
                string transfer_port = cmb_removeTransferPortID.Text;
                AVEHICLE vh = scApp.VehicleBLL.cache.getVhByID(vh_id);
                if (vh == null)
                {
                    MessageBox.Show($"Plaese select vh.", "Carrier remove fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (vh.HAS_CST == 1)
                {
                    MessageBox.Show($"vh:{vh_id} has carrier.can't remove.", "Carrier remove fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cst_id.Length == CARRIER_ID_FIX_LENGTH)
                {
                    MessageBox.Show($"Plaese check carrier id of length, it need {CARRIER_ID_FIX_LENGTH} words", "Carrier remove fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                btn_remove.Enabled = false;
                await Task.Run(() =>
                {
                    List<AMCSREPORTQUEUE> reportqueues = new List<AMCSREPORTQUEUE>();
                    scApp.ReportBLL.newReportCarrierRemoved(vh.Real_ID, cst_id, transfer_port, reportqueues);
                    scApp.ReportBLL.insertMCSReport(reportqueues);
                    scApp.ReportBLL.newSendMCSMessage(reportqueues);
                });
                Common.LogHelper.Log(logger: NLog.LogManager.GetCurrentClassLogger(), LogLevel: LogLevel.Info, Class: nameof(CarrierMaintenanceForm), Device: "OHTC",
                  Data: $"Manual carrier remove success, vh id:{vh_id}, cst id:{cst_id}, transfer port:{transfer_port}");
                MessageBox.Show("Carrier remove success", "Carrier remove success.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await Task.Run(() =>
                {
                    SpinWait.SpinUntil(() => false, 60000);
                });
            }
            catch { }
            finally
            {
                btn_remove.Enabled = true;
            }
        }

        private void CarrierMaintenanceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainForm.removeForm(this.Name);
        }
    }
}
