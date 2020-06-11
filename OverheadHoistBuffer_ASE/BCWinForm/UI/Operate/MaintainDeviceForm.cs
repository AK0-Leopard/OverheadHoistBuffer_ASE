using com.mirle.ibg3k0.bc.winform;
using com.mirle.ibg3k0.bc.winform.App;
using com.mirle.ibg3k0.bc.winform.Common;
using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.Data.VO.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.mirle.ibg3k0.bc.winform.UI
{
    public partial class MaintainDeviceForm : Form
    {
        BCMainForm mainForm;
        BCApplication bcApp;

        MaintainLift MTL = null;
        MaintainSpace MTS = null;
        MTxValueDefMapActionBase MTLValueDefMapActionBase = null;
        MTxValueDefMapActionBase MTSValueDefMapActionBase = null;

        public MaintainDeviceForm(BCMainForm _mainForm)
        {
            InitializeComponent();
            mainForm = _mainForm;
            bcApp = mainForm.BCApp;
            timer1.Enabled = true;

        }
        private void cmb_mtl_SelectedIndexChanged(object sender, EventArgs e)
        {
            string device_id = (sender as ComboBox).Text;
            MTL = bcApp.SCApplication.getEQObjCacheManager().getEquipmentByEQPTID(device_id) as MaintainLift;
            MTLValueDefMapActionBase = MTL.getMapActionByIdentityKey(nameof(MTLValueDefMapActionNew)) as MTxValueDefMapActionBase;
        }

        private void cmb_mts_SelectedIndexChanged(object sender, EventArgs e)
        {
            string device_id = (sender as ComboBox).Text;
            MTS = bcApp.SCApplication.getEQObjCacheManager().getEquipmentByEQPTID(device_id) as MaintainSpace;
            MTSValueDefMapActionBase = MTS.getMapActionByIdentityKey(nameof(MTSValueDefMapActionNew)) as MTxValueDefMapActionBase;
        }

        private void btn_mtl_dateTimeSync_Click(object sender, EventArgs e)
        {
            DateTimeSyncCommand(MTLValueDefMapActionBase);
        }
        private void btn_mts_dateTimeSync_Click(object sender, EventArgs e)
        {
            DateTimeSyncCommand(MTSValueDefMapActionBase);
        }
        private void DateTimeSyncCommand(MTxValueDefMapActionBase mTxValueDefMapActionBase)
        {
            Task.Run(() =>
            {
                mTxValueDefMapActionBase.DateTimeSyncCommand(DateTime.Now);
            });
        }

        private void btn_mtl_car_out_notify_Click(object sender, EventArgs e)
        {
            UInt16 car_id = UInt16.Parse(txt_mtl_car_out_notify_car_id.Text);
            CarOutNotify(MTLValueDefMapActionBase, car_id);
        }

        private void btn_mts_car_out_notify_Click(object sender, EventArgs e)
        {
            UInt16 car_id = UInt16.Parse(txt_mts_car_out_notify_car_id.Text);
            CarOutNotify(MTSValueDefMapActionBase, car_id);
        }

        private void CarOutNotify(MTxValueDefMapActionBase mTxValueDefMapActionBase, ushort carNum)
        {
            Task.Run(() =>
            {
                mTxValueDefMapActionBase.OHxC_CarOutNotify(carNum);
            });
        }

        bool mtlcaroutexcuting = false;
        bool MTLCarOutExcuting
        {
            set
            {
                if (mtlcaroutexcuting != value)
                {
                    mtlcaroutexcuting = value;
                    if (mtlcaroutexcuting)
                    {
                        btn_mtlcarOutTest.Enabled = false;
                        mtl_prepare_car_out_info.Enabled = false;
                        btn_mtl_cauout_cancel.Enabled = true;
                    }
                    else
                    {
                        btn_mtlcarOutTest.Enabled = true;
                        mtl_prepare_car_out_info.Enabled = true;
                        btn_mtl_cauout_cancel.Enabled = false;
                    }
                }
            }
            get
            {
                return mtlcaroutexcuting;
            }
        }
        bool mtscaroutexcuting = false;
        bool MTSCarOutExcuting
        {
            set
            {
                if (mtscaroutexcuting != value)
                {
                    mtscaroutexcuting = value;
                    if (mtscaroutexcuting)
                    {
                        btn_mtscarOutTest.Enabled = false;
                        mts_prepare_car_out_info.Enabled = false;
                        btn_mts_cauout_cancel.Enabled = true;
                    }
                    else
                    {
                        btn_mtscarOutTest.Enabled = true;
                        mts_prepare_car_out_info.Enabled = true;
                        btn_mts_cauout_cancel.Enabled = false;
                    }
                }
            }
            get
            {
                return mtlcaroutexcuting;
            }
        }
        private async void btn_mtlcarOutTest_Click(object sender, EventArgs e)
        {
            try
            {
                string pre_car_out_vh = cmb_mtl_car_out_vh.Text;
                btn_mtlcarOutTest.Enabled = false;
                var r = default((bool isSuccess, string result));
                await Task.Run(() => r = AutoCarOutTest(MTL, pre_car_out_vh));

                MessageBox.Show(r.result);
            }
            finally
            {
                btn_mtlcarOutTest.Enabled = true;
            }
        }
        private async void btn_mtscarOutTest_Click(object sender, EventArgs e)
        {
            try
            {
                string pre_car_out_vh = cmb_mts_car_out_vh.Text;
                btn_mtscarOutTest.Enabled = false;
                var r = default((bool isSuccess, string result));
                await Task.Run(() => r = AutoCarOutTest(MTS, pre_car_out_vh));

                MessageBox.Show(r.result);
            }
            finally
            {
                btn_mtscarOutTest.Enabled = true;
            }
        }
        private (bool isSuccess, string result) AutoCarOutTest(IMaintainDevice maintainDevice, string preCarOutVhID)
        {
            var r = default((bool isSuccess, string result));
            try
            {
                //var r = bcApp.SCApplication.MTLService.carOutRequset(maintainDevice, vh_id);
                AVEHICLE pre_car_out_vh = bcApp.SCApplication.VehicleBLL.cache.getVhByID(preCarOutVhID);
                if (maintainDevice is sc.Data.VO.MaintainLift)
                {
                    sc.Data.VO.Interface.IMaintainDevice dockingMTS = bcApp.SCApplication.EquipmentBLL.cache.GetMaintainSpace();
                    r = bcApp.SCApplication.MTLService.checkVhAndMTxCarOutStatus(maintainDevice, dockingMTS, pre_car_out_vh);
                    if (r.isSuccess)
                    {
                        r = bcApp.SCApplication.MTLService.CarOurRequest(maintainDevice, pre_car_out_vh);
                    }
                    if (r.isSuccess)
                    {
                        r = bcApp.SCApplication.MTLService.processCarOutScenario(maintainDevice as sc.Data.VO.MaintainLift, pre_car_out_vh);
                    }
                }
                else if (maintainDevice is sc.Data.VO.MaintainSpace)
                {
                    r = bcApp.SCApplication.MTLService.checkVhAndMTxCarOutStatus(maintainDevice, null, pre_car_out_vh);
                    if (r.isSuccess)
                    {
                        r = bcApp.SCApplication.MTLService.CarOurRequest(maintainDevice, pre_car_out_vh);
                    }
                    if (r.isSuccess)
                    {
                        r = bcApp.SCApplication.MTLService.processCarOutScenario(maintainDevice as sc.Data.VO.MaintainSpace, pre_car_out_vh);
                    }
                }
            }
            catch (Exception ex)
            {
                r = (false, ex.ToString());
            }
            finally
            {
            }
            return r;
        }

        private void btn_mtl_cauout_cancel_Click(object sender, EventArgs e)
        {
            try
            {
                btn_mtl_cauout_cancel.Enabled = false;
                AutoCarOutCancel(MTL);
            }
            finally
            {
                btn_mtl_cauout_cancel.Enabled = true;
            }
        }

        private async void btn_mts_cauout_cancel_Click(object sender, EventArgs e)
        {
            try
            {
                btn_mts_cauout_cancel.Enabled = false;
                await AutoCarOutCancel(MTS);
            }
            finally
            {
                btn_mts_cauout_cancel.Enabled = true;
            }
        }

        private Task AutoCarOutCancel(IMaintainDevice maintainDevice)
        {
            return Task.Run(() => bcApp.SCApplication.MTLService.carOutRequestCancle(maintainDevice));
        }

        private void btn_mtl_vh_realtime_info_Click(object sender, EventArgs e)
        {
            UInt16 car_id = UInt16.Parse(txt_mtl_car_id.Text);
            UInt16 action_mode = UInt16.Parse(txt_mtl_action_mode.Text);
            UInt16 cst_exist = UInt16.Parse(txt_mtl_cst_exist.Text);
            UInt16 current_section_id = UInt16.Parse(txt_mtl_current_sec_id.Text);
            UInt32 current_address_id = UInt32.Parse(txt_mtl_current_adr_id.Text);
            UInt32 buffer_distance = UInt32.Parse(txt_mtl_buffer_distance.Text);
            UInt16 speed = UInt16.Parse(txt_mtl_speed.Text);
            Task.Run(() =>
            {
                CarRealtimeInfo(MTL, car_id, action_mode, cst_exist, current_section_id, current_address_id, buffer_distance, speed);
            }
            );
        }

        private void btn_mts_vh_realtime_info_Click(object sender, EventArgs e)
        {
            UInt16 car_id = UInt16.Parse(txt_mts_car_id.Text);
            UInt16 action_mode = UInt16.Parse(txt_mts_action_mode.Text);
            UInt16 cst_exist = UInt16.Parse(txt_mts_cst_exist.Text);
            UInt16 current_section_id = UInt16.Parse(txt_mts_current_sec_id.Text);
            UInt32 current_address_id = UInt32.Parse(txt_mts_current_adr_id.Text);
            UInt32 buffer_distance = UInt32.Parse(txt_mts_buffer_distance.Text);
            UInt16 speed = UInt16.Parse(txt_mts_speed.Text);
            Task.Run(() =>
            {
                CarRealtimeInfo(MTS, car_id, action_mode, cst_exist, current_section_id, current_address_id, buffer_distance, speed);
            }
            );
        }

        private void CarRealtimeInfo(IMaintainDevice maintainDevice, UInt16 car_id, UInt16 action_mode, UInt16 cst_exist, UInt16 current_section_id, UInt32 current_address_id,
                                            UInt32 buffer_distance, UInt16 speed)
        {
            maintainDevice.setCarRealTimeInfo(car_id, action_mode, cst_exist, current_section_id, current_address_id, buffer_distance, speed);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            lbl_mtl_alive.Text = MTL.Is_Eq_Alive.ToString();
            lbl_mtl_current_car_id.Text = MTL.CurrentCarID;
            lbl_mtl_has_vh.Text = MTL.HasVehicle.ToString();
            lbl_mtl_stop_single.Text = MTL.StopSingle.ToString();
            lbl_mtl_mode.Text = MTL.MTxMode.ToString();
            lbl_mtl_location.Text = MTL.MTLLocation.ToString();
            lbl_mtl_moving_status.Text = MTL.MTLMovingStatus.ToString();
            lbl_mtl_encoder.Text = MTL.Encoder.ToString();
            lbl_mtl_in_position.Text = MTL.VhInPosition.ToString();
            btn_mtl_m2o_u2d_safetycheck.Checked = MTL.CarOutSafetyCheck;
            btn_mtl_m2o_d2u_safetycheck.Checked = MTL.CarInSafetyCheck;
            btn_mtl_o2m_u2d_caroutInterlock.Checked = MTL.CarOutInterlock;
            btn_mtl_o2m_d2u_moving.Checked = MTL.CarInMoving;

            lbl_mts_alive.Text = MTS.Is_Eq_Alive.ToString();
            lbl_mts_current_car_id.Text = MTS.CurrentCarID;
            lbl_mts_has_vh.Text = MTS.HasVehicle.ToString();
            lbl_mts_stop_single.Text = MTS.StopSingle.ToString();
            lbl_mts_mode.Text = MTS.MTxMode.ToString();
            lbl_mts_in_position.Text = MTS.VhInPosition.ToString();
            btn_mts_m2o_u2d_safetycheck.Checked = MTS.CarOutSafetyCheck;
            btn_mts_m2o_d2u_safetycheck.Checked = MTS.CarInSafetyCheck;
            btn_mts_o2m_u2d_caroutInterlock.Checked = MTS.CarOutInterlock;
            btn_mts_o2m_d2u_moving.Checked = MTS.CarInMoving;

            if (!mtl_prepare_car_out_info.Enabled)
            {
                txt_mtl_car_id.Text = MTL.CurrentPreCarOurID.ToString();
                txt_mtl_action_mode.Text = MTL.CurrentPreCarOurActionMode.ToString();
                txt_mtl_cst_exist.Text = MTL.CurrentPreCarOurCSTExist.ToString();
                txt_mtl_current_sec_id.Text = MTL.CurrentPreCarOurSectionID.ToString();
                txt_mtl_current_adr_id.Text = MTL.CurrentPreCarOurAddressID.ToString();
                txt_mtl_buffer_distance.Text = MTL.CurrentPreCarOurDistance.ToString();
                txt_mtl_speed.Text = MTL.CurrentPreCarOurSpeed.ToString();
                //btn_mtl_o2m_u2d_caroutInterlock.Checked = MTL.CarOutInterlock;
                //btn_mtl_o2m_d2u_moving.Checked = MTL.CarInMoving;
            }
            if (!mts_prepare_car_out_info.Enabled)
            {
                txt_mts_car_id.Text = MTS.CurrentPreCarOurID.ToString();
                txt_mts_action_mode.Text = MTS.CurrentPreCarOurActionMode.ToString();
                txt_mts_cst_exist.Text = MTS.CurrentPreCarOurCSTExist.ToString();
                txt_mts_current_sec_id.Text = MTS.CurrentPreCarOurSectionID.ToString();
                txt_mts_current_adr_id.Text = MTS.CurrentPreCarOurAddressID.ToString();
                txt_mts_buffer_distance.Text = MTS.CurrentPreCarOurDistance.ToString();
                txt_mts_speed.Text = MTS.CurrentPreCarOurSpeed.ToString();
                //btn_mts_o2m_u2d_caroutInterlock.Checked = MTS.CarOutInterlock;
                //btn_mts_o2m_d2u_moving.Checked = MTS.CarInMoving;
            }
            MTLCarOutExcuting = !SCUtility.isEmpty(MTL.PreCarOutVhID);
            MTSCarOutExcuting = !SCUtility.isEmpty(MTS.PreCarOutVhID);


        }

        private void MaintainDeviceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainForm.removeForm(this.Name);
            timer1.Enabled = false;
        }

        private void MaintainDeviceForm_Load(object sender, EventArgs e)
        {
            List<AEQPT> maintainDevices = bcApp.SCApplication.EquipmentBLL.cache.loadMaintainLift();
            string[] maintain_Lift_id = maintainDevices.Select(eq => eq.EQPT_ID).ToArray();
            BCUtility.setComboboxDataSource(cmb_mtl, maintain_Lift_id.ToArray());
            maintainDevices = bcApp.SCApplication.EquipmentBLL.cache.loadMaintainSpace();
            string[] maintain_Space_id = maintainDevices.Select(eq => eq.EQPT_ID).ToArray();
            BCUtility.setComboboxDataSource(cmb_mts, maintain_Space_id.ToArray());

            List<AVEHICLE> vhs = bcApp.SCApplication.VehicleBLL.cache.loadVhs();
            string[] vh_ids = vhs.Select(vh => vh.VEHICLE_ID).ToArray();
            BCUtility.setComboboxDataSource(cmb_mtl_car_out_vh, vh_ids.ToArray());
            BCUtility.setComboboxDataSource(cmb_mts_car_out_vh, vh_ids.ToArray());
        }

        private void btn_mtl_o2m_u2d_caroutInterlock_Click(object sender, EventArgs e)
        {
            bool set_true_flase = !(sender as RadioButton).Checked;
            (sender as RadioButton).Checked = set_true_flase;
            MTLValueDefMapActionBase.setOHxC2MTL_CarOutInterlock(set_true_flase);
        }

        private void btn_mts_o2m_u2d_caroutInterlock_Click(object sender, EventArgs e)
        {
            bool set_true_flase = !(sender as RadioButton).Checked;
            (sender as RadioButton).Checked = set_true_flase;
            MTSValueDefMapActionBase.setOHxC2MTL_CarOutInterlock(set_true_flase);
        }


        private void btn_mts_o2m_d2u_moving_Click(object sender, EventArgs e)
        {
            bool set_true_flase = !(sender as RadioButton).Checked;
            (sender as RadioButton).Checked = set_true_flase;
            MTSValueDefMapActionBase.setOHxC2MTL_CarInMoving(set_true_flase);
        }

        private void btn_mtl_o2m_d2u_moving_Click(object sender, EventArgs e)
        {
            bool set_true_flase = !(sender as RadioButton).Checked;
            (sender as RadioButton).Checked = set_true_flase;
            MTLValueDefMapActionBase.setOHxC2MTL_CarInMoving(set_true_flase);
        }

        private async void btn_mtl_alarm_reset_Click(object sender, EventArgs e)
        {
            await Task.Run(() => MTLValueDefMapActionBase.OHxC_AlarmResetRequest());
        }

        private async void btn_mts_alarm_reset_Click(object sender, EventArgs e)
        {
            await Task.Run(() => MTSValueDefMapActionBase.OHxC_AlarmResetRequest());
        }
    }
}
