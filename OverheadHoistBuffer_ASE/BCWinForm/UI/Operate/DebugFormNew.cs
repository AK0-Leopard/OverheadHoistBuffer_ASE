// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2020/05/24    Jason Wu       N/A            A0.01   新增DebugParameter.ignore136UnloadComplete 開關
//**********************************************************************************
using com.mirle.ibg3k0.bc.winform.App;
using com.mirle.ibg3k0.bc.winform.Common;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction;
using com.mirle.ibg3k0.sc.Data.VO;
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
using static com.mirle.ibg3k0.sc.App.SCAppConstants;

namespace com.mirle.ibg3k0.bc.winform.UI
{
    public partial class DebugFormNew : Form
    {

        BCMainForm mainForm;
        BCApplication bcApp;
        List<RadioButton> radioButtons = new List<RadioButton>();
        List<BLOCKZONEQUEUE> blocked_queues = null;
        AEQPT MTSMTL = null;
        public DebugFormNew(BCMainForm _mainForm)
        {
            InitializeComponent();
            mainForm = _mainForm;
            bcApp = mainForm.BCApp;

            cb_StartGenAntoCmd.Checked = DebugParameter.CanAutoRandomGeneratesCommand;
            cb_FroceReservePass.Checked = DebugParameter.isForcedPassReserve;
            cb_FroceReserveReject.Checked = DebugParameter.isForcedRejectBlockControl;
            cb_IsHandleBoxPassOff.Checked = DebugParameter.isHandleBoxAbnormalPassOff;
            cb_id_136_retry_test.Checked = DebugParameter.Is_136_retry_test;
            cb_testZoneCommandReqNoReply.Checked = DebugParameter.Is_136_ZoneCommandReq_retry_test;

            num_priorityWatershed.Value = sc.App.SystemParameter.cmdPriorityWatershed;
            num_priorityForBoxMove.Value = sc.App.SystemParameter.BoxMovePriority;
            cb_LoopEnhance.Checked = sc.App.SystemParameter.isLoopTransferEnhance;
            cb_isByPassStraightReserve.Checked = sc.App.SystemParameter.isReserveByPassOnStraight;
            List<string> lstVh = new List<string>();
            lstVh.Add(string.Empty);
            lstVh.AddRange(bcApp.SCApplication.getEQObjCacheManager().getAllVehicle().Select(vh => vh.VEHICLE_ID).ToList());
            string[] allVh = lstVh.ToArray();
            BCUtility.setComboboxDataSource(cmb_tcpipctr_Vehicle, allVh);

            List<ASEGMENT> segments = bcApp.SCApplication.SegmentBLL.cache.GetSegments();
            string[] segment_ids = segments.Select(seg => seg.SEG_NUM).ToArray();


            List<AADDRESS> allAddress_obj = bcApp.SCApplication.MapBLL.loadAllAddress();
            string[] allAdr_ID = allAddress_obj.Select(adr => adr.ADR_ID).ToArray();

            List<AEQPT> maintainDevices = bcApp.SCApplication.EquipmentBLL.cache.loadMaintainDevice();
            string[] maintain_devices_id = maintainDevices.Select(eq => eq.EQPT_ID).ToArray();

            List<AEQPT> ohcvDevices = bcApp.SCApplication.EquipmentBLL.cache.loadOHCVDevices();
            string[] ohcv_devices_id = ohcvDevices.Select(eq => eq.EQPT_ID).ToArray();



            cb_OperMode.DataSource = Enum.GetValues(typeof(sc.ProtocolFormat.OHTMessage.OperatingVHMode));
            cmb_pauseEvent.DataSource = Enum.GetValues(typeof(sc.ProtocolFormat.OHTMessage.PauseEvent));
            cmb_pauseType.DataSource = Enum.GetValues(typeof(OHxCPauseType)).Cast<OHxCPauseType>()
                                           .Where(e => e == OHxCPauseType.ALL ||
                                                       e == OHxCPauseType.Normal).ToList();

            cb_Abort_Type.DataSource = Enum.GetValues(typeof(sc.ProtocolFormat.OHTMessage.CMDCancelType))
                                           .Cast<sc.ProtocolFormat.OHTMessage.CMDCancelType>()
                                           .Where(e => e == sc.ProtocolFormat.OHTMessage.CMDCancelType.CmdCancel).ToList();

            combox_cycle_type.DataSource = Enum.GetValues(typeof(DebugParameter.CycleRunType));

            if (sc.Common.SCUtility.isMatche(bcApp.LoginUserID, BCAppConstants.ADMIN_USER_NAME))
            {
                group_cycleRun.Visible = true;
                groupBox6.Enabled = true;
            }

        }

        private void DebugForm_Load(object sender, EventArgs e)
        {
            DebugParameter.IsDebugMode = true;
        }

        private void DebugForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            TrunOffAllVhPLCControl();
            DebugParameter.IsDebugMode = false;
            mainForm.removeForm(typeof(DebugFormNew).Name);
            cb_id_136_retry_test.Checked = false;
        }



        private void cb_FroceReservePass_CheckedChanged(object sender, EventArgs e)
        {
            DebugParameter.isForcedPassReserve = cb_FroceReservePass.Checked;
        }


        AVEHICLE noticeCar = null;
        string vh_id = null;
        private void cmb_Vehicle_SelectedIndexChanged(object sender, EventArgs e)
        {
            vh_id = cmb_tcpipctr_Vehicle.Text.Trim();

            noticeCar = bcApp.SCApplication.getEQObjCacheManager().getVehicletByVHID(vh_id);
            lbl_id_37_cmdID_value.Text = noticeCar?.OHTC_CMD;
            lbl_install_status.Text = noticeCar?.IS_INSTALLED.ToString();
            lbl_listening_status.Text = noticeCar?.IsTcpIpListening(bcApp.SCApplication.getBCFApplication()).ToString();
        }


        private void asyExecuteAction(Func<string, bool> act)
        {
            Task.Run(() =>
            {
                act(vh_id);
            });
        }



        private void button3_Click(object sender, EventArgs e)
        {
            sc.ProtocolFormat.OHTMessage.OperatingVHMode operatiogMode;
            Enum.TryParse(cb_OperMode.SelectedValue.ToString(), out operatiogMode);

            Task.Run(() =>
            {
                bcApp.SCApplication.VehicleService.ModeChangeRequest(vh_id, operatiogMode);
            });
        }


        private void button5_Click(object sender, EventArgs e)
        {
            asyExecuteAction(bcApp.SCApplication.VehicleService.AlarmResetRequest);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (noticeCar == null)
            {
                MessageBox.Show($"Please select vh first.", "Force finish fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string message = $"Do you want to force finish vh:{noticeCar.VEHICLE_ID} command?";
            DialogResult confirmResult = MessageBox.Show(this, message,
                BCApplication.getMessageString("CONFIRM"), MessageBoxButtons.YesNo);

            BCUtility.recordAction(bcApp, this.Name, message, confirmResult.ToString());
            if (confirmResult != System.Windows.Forms.DialogResult.Yes)
            {
                return;
            }
            Task.Run(() =>
            {
                bcApp.SCApplication.CMDBLL.forceUpdataCmdStatus2FnishByVhID(vh_id);
            });
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                //bcApp.SCApplication.VehicleService.forceResetVHStatus(vh_id);
                bcApp.SCApplication.VehicleService.VehicleStatusRequest(vh_id, true);
            });
        }

        private void cb_StartGenAntoCmd_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            DebugParameter.CanAutoRandomGeneratesCommand = cb.Checked;

            if (!cb.Checked)
            {
                Task.Run(() =>
                {
                    var mcs_cmds = bcApp.SCApplication.CMDBLL.loadMCS_Command_Queue();
                    //foreach (var cmd in mcs_cmds)
                    //{
                    //    //bcApp.SCApplication.CMDBLL.updateCMD_MCS_TranStatus2Complete(cmd.CMD_ID, E_TRAN_STATUS.Canceling);
                    //}
                });
            }
        }


        private void btn_ForceReleaseBlock_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                bcApp.SCApplication.VehicleService.forceReleaseBlockControl(vh_id);
            });
        }

        private void btn_pause_Click(object sender, EventArgs e)
        {
            sc.ProtocolFormat.OHTMessage.PauseEvent pauseEvent;
            OHxCPauseType pauseType;
            Enum.TryParse(cmb_pauseEvent.SelectedValue.ToString(), out pauseEvent);
            Enum.TryParse(cmb_pauseType.SelectedValue.ToString(), out pauseType);
            Task.Run(() =>
            {
                bcApp.SCApplication.VehicleService.PauseRequest(vh_id, pauseEvent, pauseType);
            });

        }







        AVEHICLE plcctrAVEHICLE = null;
        string event_id = "DebugFrom";


        private void TrunOffAllVhPLCControl()
        {
            var vhs = bcApp.SCApplication.getEQObjCacheManager().getAllVehicle();

            foreach (var vh in vhs)
            {
                vh.PLC_Control_TrunOff();
            }

        }

        private void cb_FroceReserveReject_CheckedChanged(object sender, EventArgs e)
        {
            DebugParameter.isForcedRejectReserve = cb_FroceReserveReject.Checked;
        }


        private async void button9_Click(object sender, EventArgs e)
        {
            try
            {
                button9.Enabled = false;
                sc.ProtocolFormat.OHTMessage.CMDCancelType type;
                Enum.TryParse(cb_Abort_Type.SelectedValue.ToString(), out type);
                string message = $"Do you want to cancel vh:{noticeCar.VEHICLE_ID} command?";
                DialogResult confirmResult = MessageBox.Show(this, message,
                    BCApplication.getMessageString("CONFIRM"), MessageBoxButtons.YesNo);

                BCUtility.recordAction(bcApp, this.Name, message, confirmResult.ToString());
                if (confirmResult != System.Windows.Forms.DialogResult.Yes)
                {
                    return;
                }

                bool is_success = false;
                string result = "";
                await Task.Run(() =>
                {
                    ACMD_OHTC ohtc_cmd = bcApp.SCApplication.CMDBLL.getExcuteCMD_OHTCByCmdID(noticeCar.OHTC_CMD);
                    if (ohtc_cmd == null)
                    {
                        is_success = false;
                        result = "command not exist.";
                        return;
                    }
                    if (ohtc_cmd.CMD_TPYE != E_CMD_TYPE.Move)
                    {
                        is_success = false;
                        result = "Curernt excute command type not move,cancel fail.";
                        return;
                    }
                    is_success = noticeCar.sned_Str37(noticeCar.OHTC_CMD, type);
                });
                if (is_success)
                {
                    MessageBox.Show($"Cacnel command sucess.", "Cacel command sucess.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"result:{result}", "Cacel command fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Warn(ex, "Exception");
            }
            finally
            {
                button9.Enabled = true;
            }
        }





        private void dgv_cache_object_data_EditModeChanged(object sender, EventArgs e)
        {

        }




        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void btn_changeToAutoRemote_Click(object sender, EventArgs e)
        {
            bcApp.SCApplication.VehicleService.VehicleAutoModeCahnge(vh_id, sc.ProtocolFormat.OHTMessage.VHModeStatus.AutoRemote);
        }
        private void btn_changeToAutoLocal_Click_1(object sender, EventArgs e)
        {
            bcApp.SCApplication.VehicleService.VehicleAutoModeCahnge(vh_id, sc.ProtocolFormat.OHTMessage.VHModeStatus.AutoLocal);
        }

        private async void btn_changeToRemove_Click(object sender, EventArgs e)
        {
            try
            {

                if (!noticeCar.IS_INSTALLED)
                {
                    MessageBox.Show($"{vh_id} is removed ready!");
                    return;
                }
                btn_changeToRemove.Enabled = false;
                await Task.Run(() => bcApp.SCApplication.VehicleService.Remove(vh_id));
                MessageBox.Show($"{vh_id} remove ok");
                lbl_install_status.Text = noticeCar?.IS_INSTALLED.ToString();
            }
            finally
            {
                btn_changeToRemove.Enabled = true;
            }
        }

        private async void btn_changeToInstall_Click(object sender, EventArgs e)
        {
            try
            {
                if (noticeCar.IS_INSTALLED)
                {
                    MessageBox.Show($"{vh_id} is install ready!");
                    return;
                }

                btn_changeToInstall.Enabled = false;
                await Task.Run(() => bcApp.SCApplication.VehicleService.Install(vh_id));
                MessageBox.Show($"{vh_id} install ok");
                lbl_install_status.Text = noticeCar?.IS_INSTALLED.ToString();
            }
            finally
            {
                btn_changeToInstall.Enabled = true;
            }
        }

        private async void btn_open_tcp_port_Click(object sender, EventArgs e)
        {
            bool is_success = false;
            await Task.Run(() =>
             {
                 is_success = bcApp.SCApplication.VehicleService.startVehicleTcpIpServer(vh_id);
             });
            MessageBox.Show(is_success ? "OK" : "NG");
        }

        private async void btn_close_tcp_port_Click(object sender, EventArgs e)
        {
            bool is_success = false;
            await Task.Run(() =>
            {
                is_success = bcApp.SCApplication.VehicleService.stopVehicleTcpIpServer(vh_id);
            });
            MessageBox.Show(is_success ? "OK" : "NG");
        }

        private void lbl_install_status_MouseDoubleClick(object sender, MouseEventArgs e)
        {
        }

        private void lbl_listening_status_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && (ModifierKeys & Keys.Control) == Keys.Control)
            {
                btn_open_tcp_port.Visible = true;
                btn_close_tcp_port.Visible = true;
                btn_closeSession.Visible = true;
                cb_id_136_retry_test.Visible = true;
            }
        }


        OHCV selectedOHCV = null;

        private void cb_FroceReservePass_ChangeUICues(object sender, UICuesEventArgs e)
        {

        }

        private void num_section_dis_ValueChanged(object sender, EventArgs e)
        {
            string current_sec_id = txt_current_sec_id.Text;
            uint distance = (uint)num_section_dis.Value;
            bcApp.SCApplication.VehicleBLL.setAndPublishPositionReportInfo2Redis(vh_id, current_sec_id, "", distance, 0, 0);
        }

        private void combox_cycle_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            Enum.TryParse(combox_cycle_type.SelectedValue.ToString(), out DebugParameter.CycleRunType type);

            DebugParameter.cycleRunType = type;
        }

        private async void btn_resetODO_Click(object sender, EventArgs e)
        {
            try
            {
                btn_resetODO.Enabled = false;
                await Task.Run(() => bcApp.SCApplication.VehicleService.ResetODO(vh_id));
            }
            catch (Exception ex)
            {

            }
            finally
            {
                btn_resetODO.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task.Run(() => bcApp.SCApplication.EmptyBoxHandlerService.CheckTheEmptyBoxStockLevel());
        }

        private void cb_IsHandleBoxPassOff_CheckedChanged(object sender, EventArgs e)
        {
            DebugParameter.isHandleBoxAbnormalPassOff = cb_IsHandleBoxPassOff.Checked;
        }

        private void num_priorityWatershed_ValueChanged(object sender, EventArgs e)
        {
            sc.App.SystemParameter.setcmdPriorityWatershed((int)num_priorityWatershed.Value);
        }

        private void num_priorityForBoxMove_ValueChanged(object sender, EventArgs e)
        {
            sc.App.SystemParameter.setBoxMovePriority((int)num_priorityForBoxMove.Value);
        }

        private async void btn_closeSession_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                bcApp.SCApplication.VehicleService.stopVehicleTcpIpSessionTest(vh_id);
            });
        }

        private void cb_id_136_retry_test_CheckedChanged(object sender, EventArgs e)
        {
            DebugParameter.Is_136_retry_test = cb_id_136_retry_test.Checked;
        }

        private void cb_LoopEnhance_CheckedChanged(object sender, EventArgs e)
        {
            sc.App.SystemParameter.setIsLoopTransferEnhanceFlag(cb_LoopEnhance.Checked);
        }

        private void cb_testZoneCommandReqNoReply_CheckedChanged(object sender, EventArgs e)
        {
            DebugParameter.Is_136_ZoneCommandReq_retry_test = cb_testZoneCommandReqNoReply.Checked;
        }

        private void label7_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && (ModifierKeys & Keys.Control) == Keys.Control)
            {
                grb_testAear.Visible = true;
            }
        }

        private void cb_isByPassStraightReserve_CheckedChanged(object sender, EventArgs e)
        {
            sc.App.SystemParameter.setIsReserveByPassOnStraight(cb_isByPassStraightReserve.Checked);
        }

        //*************************************
        //A0.01

    }
}
