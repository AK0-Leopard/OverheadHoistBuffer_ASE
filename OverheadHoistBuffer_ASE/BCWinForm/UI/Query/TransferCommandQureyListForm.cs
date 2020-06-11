using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.ObjectRelay;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using NLog;

namespace com.mirle.ibg3k0.bc.winform.UI
{
    public partial class TransferCommandQureyListForm : Form
    {
        BCMainForm mainform;
        BindingSource cmsMCS_bindingSource = new BindingSource();
        List<CMD_MCSObjToShow> cmdMCSList = null;
        int selection_index = -1;
        public TransferCommandQureyListForm(BCMainForm _mainForm)
        {
            InitializeComponent();
            dgv_TransferCommand.AutoGenerateColumns = false;
            mainform = _mainForm;

            dgv_TransferCommand.DataSource = cmsMCS_bindingSource;
        }

        private void updateTransferCommand()
        {

            var ACMD_MCSs = mainform.BCApp.SCApplication.CMDBLL.loadACMD_MCSIsUnfinished();
            cmdMCSList = ACMD_MCSs.Select(cmd => new CMD_MCSObjToShow(mainform.BCApp.SCApplication.VehicleBLL, cmd)).ToList();
            cmsMCS_bindingSource.DataSource = cmdMCSList;
            dgv_TransferCommand.Refresh();
        }

        private void btn_refresh_Click(object sender, EventArgs e)
        {
            selection_index = -1;
            updateTransferCommand();
        }


        private void dgv_TransferCommand_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv_TransferCommand.SelectedRows.Count > 0)
                selection_index = dgv_TransferCommand.SelectedRows[0].Index;
        }

        private async void btn_force_finish_Click(object sender, EventArgs e)
        {
            try
            {
                if (selection_index == -1) return;
                btn_force_finish.Enabled = false;
                var mcs_cmd = cmdMCSList[selection_index];

                AVEHICLE excute_cmd_of_vh = mainform.BCApp.SCApplication.VehicleBLL.cache.getVehicleByMCSCmdID(mcs_cmd.CMD_ID);

                await Task.Run(() =>
                {
                    try
                    {
                        if (excute_cmd_of_vh != null)
                        {
                            mainform.BCApp.SCApplication.VehicleBLL.doTransferCommandFinish(excute_cmd_of_vh.VEHICLE_ID, excute_cmd_of_vh.OHTC_CMD, CompleteStatus.CmpStatusForceFinishByOp);
                            mainform.BCApp.SCApplication.VIDBLL.initialVIDCommandInfo(excute_cmd_of_vh.VEHICLE_ID);
                        }
                        //mainform.BCApp.SCApplication.CMDBLL.updateCMD_MCS_TranStatus2Complete(mcs_cmd.CMD_ID, E_TRAN_STATUS.Aborting);
                        mainform.BCApp.SCApplication.ReportBLL.newReportTransferCommandNormalFinish(mcs_cmd.cmd_mcs, excute_cmd_of_vh, sc.Data.SECS.CSOT.SECSConst.CMD_Result_Unsuccessful, null);
                    }
                    catch { }
                }
                );
                Common.LogHelper.Log(logger: NLog.LogManager.GetCurrentClassLogger(), LogLevel: LogLevel.Info, Class: nameof(TransferCommandQureyListForm), Device: "OHTC",
                  Data: $"Fource mcs command finish success, vh id:{SCUtility.Trim(excute_cmd_of_vh?.VEHICLE_ID, true)}, cst id:{SCUtility.Trim(excute_cmd_of_vh?.CST_ID, true)}, " +
                        $"mcs command id:{SCUtility.Trim(mcs_cmd.CMD_ID, true)},source:{SCUtility.Trim(mcs_cmd.HOSTSOURCE, true)},dest:{SCUtility.Trim(mcs_cmd.HOSTDESTINATION, true)}");
                updateTransferCommand();
            }
            catch { }
            finally
            {
                btn_force_finish.Enabled = true;
            }
        }

        private void TransferCommandQureyListForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainform.removeForm(this.Name);
        }
    }
}
