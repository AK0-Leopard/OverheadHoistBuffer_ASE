﻿using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.ObjectRelay;
using com.mirle.ibg3k0.bc.winform.App;
using com.mirle.ibg3k0.Utility.ul.Data.VO;
using com.mirle.ibg3k0.sc.Common;
using System.Threading;
using NLog;
using com.mirle.ibg3k0.bc.winform.Common;

namespace com.mirle.ibg3k0.bc.winform.UI
{
    public partial class OHT_FormNew2 : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        BCMainForm mainform = null;
        SCApplication scApp = null;
        //BindingList<VehicleObjToShow> ObjectToShow_list = new BindingList<VehicleObjToShow>();
        BindingSource bindingSource = new BindingSource();
        string[] allAdr_ID = null;
        string[] allPortID = null;
        List<CMD_MCSObjToShow> cmd_mcs_obj_to_show = new List<CMD_MCSObjToShow>();
        BindingSource cmsMCS_bindingSource = new BindingSource();

        List<ALARM> aLARMs = new List<ALARM>();

        public OHT_FormNew2(BCMainForm _form)
        {
            InitializeComponent();
            mainform = _form;
            scApp = mainform.BCApp.SCApplication;
            uctlMapWPFNew1.Start(mainform.BCApp);
            uctlMapWPFNew1.AddressSelected += UctlMapWPFNew1_AddressSelected;
            //ui_Vehicle1.start("OHT001");
            initialComBox();
            initialDataGreadView();
            bindingSource.DataSource = scApp.getEQObjCacheManager().CommonInfo.ObjectToShow_list;
            dgv_vhStatus.DataSource = bindingSource;
            scApp.getEQObjCacheManager().CommonInfo.ObjectToShow_list.Clear();

            dgv_TransferCommand.AutoGenerateColumns = false;



            double distance_scale = 1000;

            foreach (var vh in scApp.getEQObjCacheManager().getAllVehicle())
            {
                VehicleObjToShow vhShowObj = new VehicleObjToShow(vh, distance_scale);

                scApp.getEQObjCacheManager().CommonInfo.ObjectToShow_list.Add(vhShowObj);
            }
            //}
            timer_TimedUpdates.Enabled = true;
            adjustmentDataGridViewWeight();

            initialEvent();
            SetHostControlState(scApp.getEQObjCacheManager().getLine());

        }

        bool isSourceSelected = false;
        private void UctlMapWPFNew1_AddressSelected(object sender, string e)
        {
            if (sc.Common.SCUtility.isEmpty(e)) return;
            if (!isSourceSelected)
            {
                cmb_fromAddress.Text = e;
                isSourceSelected = true;
            }
            else
            {
                cmb_toAddress.Text = e;
                isSourceSelected = false;
            }
        }

        private void initialEvent()
        {
            recordViewLog("");
            ALINE line = scApp.getEQObjCacheManager().getLine();

            line.addEventHandler(this.Name
           , BCFUtility.getPropertyName(() => line.ServiceMode)
           , (s1, e1) =>
           {
               Adapter.Invoke((obj) =>
               {
                   switch (line.ServiceMode)
                   {
                       case SCAppConstants.AppServiceMode.None:
                           lbl_isMaster.BackColor = Color.Gray;
                           recordViewLog("");

                           break;
                       case SCAppConstants.AppServiceMode.Active:
                           lbl_isMaster.BackColor = Color.Green;
                           recordViewLog("");

                           break;
                       case SCAppConstants.AppServiceMode.Standby:
                           lbl_isMaster.BackColor = Color.Yellow;
                           recordViewLog("");

                           break;
                   }
               }, null);
           });
            recordViewLog("");
            line.addEventHandler(this.Name
           , BCFUtility.getPropertyName(() => line.Secs_Link_Stat)
                , (s1, e1) =>
                {
                    lbl_hostconn.BackColor =
                    line.Secs_Link_Stat == SCAppConstants.LinkStatus.LinkOK ? Color.Green : Color.Gray;
                }
                );
            recordViewLog("");

            line.addEventHandler(this.Name
           , BCFUtility.getPropertyName(() => line.Redis_Link_Stat)
                , (s1, e1) =>
                {
                    lbl_RediStat.BackColor =
                    line.Redis_Link_Stat == SCAppConstants.LinkStatus.LinkOK ? Color.Green : Color.Gray;
                }
                );
            recordViewLog("");

            line.addEventHandler(this.Name
                , BCFUtility.getPropertyName(() => line.Host_Control_State)
                    , (s1, e1) =>
                    {
                        SetHostControlState(line);
                    }
                    );
            mainform.TestGuideCompleted += Mainform_TestGuideCompleted;
            recordViewLog("");
        }

        private void Mainform_TestGuideCompleted(object sender, List<string> e)
        {
            uctlMapWPFNew1.setTestGideRail(e);
        }


        private void SetHostControlState(ALINE line)
        {
            Color hostMode_Color = Color.Empty;
            switch (line.Host_Control_State)
            {
                case SCAppConstants.LineHostControlState.HostControlState.EQ_Off_line:
                    hostMode_Color = Color.Gray;
                    recordViewLog("");
                    break;
                case SCAppConstants.LineHostControlState.HostControlState.On_Line_Local:
                    hostMode_Color = Color.Yellow;
                    recordViewLog("");
                    break;
                case SCAppConstants.LineHostControlState.HostControlState.On_Line_Remote:
                    hostMode_Color = Color.Green;
                    break;
                default:
                    hostMode_Color = Color.Gray;
                    break;
            }
            lbl_HoseMode.BackColor = hostMode_Color;
        }
        private void recordViewLog(string msg)
        {

        }

        private void adjustmentDataGridViewWeight()
        {
            dgv_vhStatus.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            foreach (DataGridViewColumn col in dgv_vhStatus.Columns)
            {
                switch (col.Name)
                {
                    case "MCS_CMD":
                        col.FillWeight = 1200;
                        break;
                    case "OHTC_CMD":
                        col.FillWeight = 1200;
                        break;
                    case "ACT_STATUS":
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        col.FillWeight = 1500;
                        recordViewLog("");
                        break;
                    case "PACK_TIME":
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        col.FillWeight = 2400;
                        break;
                    case "CYCLERUN_TIME":
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        col.FillWeight = 2400;
                        break;
                    case "OBS_DIST2Show":
                    case "VEHICLE_ACC_DIST2Show":
                    case "ACC_SEC_DIST2Show":
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        break;
                }
            }
            recordViewLog("");
        }

        void Local_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (INotifyPropertyChanged item in e.NewItems)
                item.PropertyChanged += item_PropertyChanged;
        }

        void item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            recordViewLog("");
        }

        private void initialComBox()
        {
            recordViewLog("");
            allPortID = scApp.PortDefBLL.GetOHB_PortData(scApp.getEQObjCacheManager().getLine().LINE_ID).Select(s => s.PLCPortID).ToArray();

            List<AADDRESS> allAddress_obj = scApp.MapBLL.loadAllAddress();
            allAdr_ID = allAddress_obj.Select(adr => adr.ADR_ID).ToArray();

            BCUtility.setComboboxDataSource(cmb_toAddress, allAdr_ID.ToArray());
            BCUtility.setComboboxDataSource(cmb_fromAddress, allAdr_ID.ToArray());


            string[] allCycleRunZone = scApp.CycleBLL.loadCycleRunMasterByCurrentCycleTypeID
                (scApp.getEQObjCacheManager().getLine().Currnet_Cycle_Type).Select(master => master.ENTRY_ADR_ID).ToArray();
            cmb_cycRunZone.DataSource = allCycleRunZone;

            recordViewLog("");
            List<string> lstVh = new List<string>();
            lstVh.Add(string.Empty);
            lstVh.AddRange(scApp.VehicleBLL.loadAllVehicle().Select(vh => SCUtility.Trim(vh.VEHICLE_ID, true)).ToList());
            string[] allVh = lstVh.ToArray();
            cmb_Vehicle.DataSource = allVh;
            cmb_Vehicle.AutoCompleteCustomSource.AddRange(allVh);
            cmb_Vehicle.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmb_Vehicle.AutoCompleteSource = AutoCompleteSource.ListItems;

            cbm_Action.DataSource = Enum.GetValues(typeof(E_CMD_TYPE)).Cast<E_CMD_TYPE>()
                                                  .Where(e => e == E_CMD_TYPE.Move ||
                                                              e == E_CMD_TYPE.Scan ||
                                                              e == E_CMD_TYPE.Load ||
                                                              e == E_CMD_TYPE.Unload ||
                                                              e == E_CMD_TYPE.LoadUnload).ToList();


            List<string> park_zone_type = scApp.ParkBLL.loadAllParkZoneType();
            cb_parkZoneType.DataSource = park_zone_type;

        }


        private void initialDataGreadView()
        {
            aLARMs.Add(new ALARM());
        }


        private void btn_start_Click(object sender, EventArgs e)
        {
            recordViewLog("");
            string from_adr = cmb_fromAddress.Text;
            string to_adr = cmb_toAddress.Text;
            string vehicle_id = cmb_Vehicle.Text;

            AVEHICLE select_vh = scApp.VehicleBLL.cache.getVhByID(vehicle_id);
            if (select_vh == null)
            {
                MessageBox.Show($"No find vehile.", "Start fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            recordViewLog("");

            if (scApp.EquipmentBLL.cache.IsInMaintainDeviceRangeOfSection(scApp.SegmentBLL, select_vh.CUR_SEC_ID))
            {
                MessageBox.Show($"Can't manual control in maintain device of vehicle:{vehicle_id} current section:{select_vh.CUR_SEC_ID}",
                    "Start fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            recordViewLog("");
            if (scApp.EquipmentBLL.cache.IsInMaintainDeviceRangeOfAddress(scApp.SegmentBLL, select_vh.CUR_ADR_ID))
            {
                MessageBox.Show($"Can't manual control in maintain device of vehicle:{vehicle_id} current address:{select_vh.CUR_ADR_ID}",
                    "Start fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            recordViewLog("");

            if (scApp.EquipmentBLL.cache.IsInMaintainDeviceRangeOfAddress(scApp.SegmentBLL, from_adr))
            {
                MessageBox.Show($"Can't set maintain device range of address:{from_adr}",
                    "Start fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (scApp.EquipmentBLL.cache.IsInMaintainDeviceRangeOfAddress(scApp.SegmentBLL, to_adr))
            {
                MessageBox.Show($"Can't set maintain device range of address:{to_adr}",
                    "Start fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            recordViewLog("");

            E_CMD_TYPE cmd_type;
            Enum.TryParse<E_CMD_TYPE>(cbm_Action.SelectedValue.ToString(), out cmd_type);
            switch (cmd_type)
            {
                case E_CMD_TYPE.Move:
                case E_CMD_TYPE.MoveToMTL:
                case E_CMD_TYPE.SystemOut:
                case E_CMD_TYPE.SystemIn:
                case E_CMD_TYPE.MTLHome:
                    excuteMoveCommand(cmd_type);
                    recordViewLog("");

                    break;
                case E_CMD_TYPE.Round:
                    excuteCycleRunCommand();
                    break;
                case E_CMD_TYPE.LoadUnload:
                    excuteLoadUnloadCommand();
                    recordViewLog("");

                    break;
                case E_CMD_TYPE.Teaching:
                    excuteTeachingCommand();
                    break;
                case E_CMD_TYPE.Home:
                    excuteHomeCommand();
                    break;
                //case E_CMD_TYPE.MTLHome:
                //    excuteMTLHomeCommand();
                //    break;
                case E_CMD_TYPE.Load:
                    excuteLoadCommand();
                    break;
                case E_CMD_TYPE.Unload:
                    excuteUnloadCommand();
                    break;
                case E_CMD_TYPE.Scan:
                    excuteScanCommand();
                    break;
            }
            recordViewLog("");
        }

        private async void excuteLoadUnloadCommand()
        {
            recordViewLog("");

            string fromSection = cmb_fromSection.Text;
            ASECTION asection = scApp.MapBLL.getSectiontByID(fromSection);

            string hostsource_portid = cmb_fromAddress.Text;
            string hostdest_portid = cmb_toAddress.Text;
            string from_adr = string.Empty;
            string to_adr = string.Empty;
            E_VH_TYPE vh_type = E_VH_TYPE.None;
            scApp.PortDefBLL.getAddressID(hostsource_portid, out from_adr, out vh_type);
            scApp.PortDefBLL.getAddressID(hostdest_portid, out to_adr);
            string vehicleId = string.Empty;

            vehicleId = SCUtility.Trim(cmb_Vehicle.Text, true);
            recordViewLog("");

            if (BCFUtility.isEmpty(vehicleId))
            {
                MessageBox.Show("No find idle vehile.");
                return;
            }

            string cst_id = txt_cstID.Text;
            string box_id = txt_boxID.Text;
            string lot_id = "lot_id";
            if (BCFUtility.isEmpty(box_id))
            {
                MessageBox.Show(" 'BOX ID' must not be empty.");
                return;
            }

            sc.BLL.CMDBLL.OHTCCommandCheckResult check_result_info = null;
            recordViewLog("");

            await Task.Run(() =>
             {
                 scApp.CMDBLL.doCreatTransferCommand(vehicleId, string.Empty, cst_id, E_CMD_TYPE.LoadUnload,
                                                 hostsource_portid, hostdest_portid, 0, 0,
                                                 box_id, lot_id, from_adr, to_adr);
                 check_result_info = sc.BLL.CMDBLL.getCallContext<sc.BLL.CMDBLL.OHTCCommandCheckResult>
                    (sc.BLL.CMDBLL.CALL_CONTEXT_KEY_WORD_OHTC_CMD_CHECK_RESULT);
             });
            if (check_result_info != null && !check_result_info.IsSuccess)
            {
                MessageBox.Show(check_result_info.ToString(), "Command create fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            recordViewLog("");

        }

        private void excuteCycleRunCommand()
        {
            string toAdr = string.Empty;
            string vehicleId = string.Empty;

            vehicleId = cmb_Vehicle.Text;
            toAdr = cmb_cycRunZone.Text;
            if (BCFUtility.isEmpty(vehicleId))
            {
                MessageBox.Show("No find vehile.");
                return;
            }

            //scApp.CMDBLL.creatCommand_OHTC(vehicleId, string.Empty, string.Empty,
            //                                E_CMD_TYPE.Round,
            //                                string.Empty,
            //                                toAdr, 0, 0);
            //Task.Run(() => { scApp.CMDBLL.generateCmd_OHTC_Details(); });
            Task.Run(() =>
            scApp.CMDBLL.doCreatTransferCommand(vehicleId, string.Empty, string.Empty,
                                            E_CMD_TYPE.Round,
                                            string.Empty,
                                            toAdr, 0, 0));
            recordViewLog("");
        }

        private async void excuteMoveCommand(E_CMD_TYPE cmd_type)
        {
            recordViewLog("");
            string toAdr = string.Empty;
            string vehicleId = string.Empty;

            vehicleId = cmb_Vehicle.Text;
            toAdr = cmb_toAddress.Text;
            if (BCFUtility.isEmpty(vehicleId))
            {
                MessageBox.Show("No find vehile.");
                return;
            }

            recordViewLog("");
            sc.BLL.CMDBLL.OHTCCommandCheckResult check_result_info = null;
            string box_id = "";
            string lot_id = "";
            await Task.Run(() =>
            {
                string from_adr = string.Empty;
                scApp.CMDBLL.doCreatTransferCommand(vehicleId, string.Empty, string.Empty,
                                                cmd_type,
                                                string.Empty,
                                                toAdr, 0, 0,
                                                box_id, lot_id, from_adr, toAdr);
                check_result_info = sc.BLL.CMDBLL.getCallContext<sc.BLL.CMDBLL.OHTCCommandCheckResult>
                                   (sc.BLL.CMDBLL.CALL_CONTEXT_KEY_WORD_OHTC_CMD_CHECK_RESULT);
            });
            if (check_result_info != null && !check_result_info.IsSuccess)
            {
                MessageBox.Show(check_result_info.ToString(), "Command create fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            recordViewLog("");
        }

        private async void excuteLoadCommand()
        {
            recordViewLog("");
            string vehicleId = string.Empty;
            string hostsource_portid = cmb_fromAddress.Text;
            string hostdest_portid = cmb_toAddress.Text;
            string from_adr = string.Empty;
            string to_adr = string.Empty;

            string cmd_id = string.Empty;
            string cst_id = txt_cstID.Text;
            string box_id = "box_id";
            string lot_id = "lot_id";
            E_VH_TYPE vh_type = E_VH_TYPE.None;
            recordViewLog("");

            vehicleId = SCUtility.Trim(cmb_Vehicle.Text, true);
            scApp.PortDefBLL.getAddressID(hostsource_portid, out from_adr, out vh_type);

            cmd_id = scApp.SequenceBLL.getCommandID(SCAppConstants.GenOHxCCommandType.Manual);
            if (BCFUtility.isEmpty(vehicleId))
            {
                MessageBox.Show("No find vehile.");
                return;
            }
            recordViewLog("");

            if (BCFUtility.isEmpty(box_id))
            {
                MessageBox.Show("box_id can't empty.");
                return;
            }
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vehicleId);
            sc.BLL.CMDBLL.OHTCCommandCheckResult check_result_info = null;
            recordViewLog("");
            await Task.Run(() =>
             {
                 {
                     //scApp.CMDBLL.doCreatTransferCommand(vehicleId, string.Empty, "CST06",
                     scApp.CMDBLL.doCreatTransferCommand(vehicleId, string.Empty, cst_id, E_CMD_TYPE.Load,
                                                      hostsource_portid, "", 0, 0,
                                                      box_id, lot_id, from_adr);
                     check_result_info = sc.BLL.CMDBLL.getCallContext<sc.BLL.CMDBLL.OHTCCommandCheckResult>
                    (sc.BLL.CMDBLL.CALL_CONTEXT_KEY_WORD_OHTC_CMD_CHECK_RESULT);

                 }
             });
            if (check_result_info != null && !check_result_info.IsSuccess)
            {
                MessageBox.Show(check_result_info.ToString(), "Command create fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            recordViewLog("");
        }

        private async void excuteScanCommand()
        {
            recordViewLog("");
            string vehicleId = string.Empty;
            string hostsource_portid = cmb_fromAddress.Text;
            string hostdest_portid = cmb_toAddress.Text;
            string from_adr = string.Empty;
            string to_adr = string.Empty;
            recordViewLog("");

            string cmd_id = string.Empty;
            string cst_id = txt_cstID.Text;
            string box_id = "box_id";
            string lot_id = "lot_id";
            E_VH_TYPE vh_type = E_VH_TYPE.None;

            vehicleId = SCUtility.Trim(cmb_Vehicle.Text, true);
            scApp.PortDefBLL.getAddressID(hostsource_portid, out from_adr, out vh_type);

            cmd_id = scApp.SequenceBLL.getCommandID(SCAppConstants.GenOHxCCommandType.Manual);
            if (BCFUtility.isEmpty(vehicleId))
            {
                MessageBox.Show("No find vehile.");
                return;
            }
            recordViewLog("");

            if (BCFUtility.isEmpty(cst_id))
            {
                MessageBox.Show("cst id can't empty.");
                return;
            }
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vehicleId);
            sc.BLL.CMDBLL.OHTCCommandCheckResult check_result_info = null;
            recordViewLog("");
            await Task.Run(() =>
            {
                {
                    //scApp.CMDBLL.doCreatTransferCommand(vehicleId, string.Empty, "CST06",
                    scApp.CMDBLL.doCreatTransferCommand(vehicleId, string.Empty, cst_id, E_CMD_TYPE.Scan,
                                                     hostsource_portid, "", 0, 0,
                                                     box_id, lot_id, from_adr);
                    check_result_info = sc.BLL.CMDBLL.getCallContext<sc.BLL.CMDBLL.OHTCCommandCheckResult>
                   (sc.BLL.CMDBLL.CALL_CONTEXT_KEY_WORD_OHTC_CMD_CHECK_RESULT);

                }
            });
            if (check_result_info != null && !check_result_info.IsSuccess)
            {
                MessageBox.Show(check_result_info.ToString(), "Command create fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            recordViewLog("");
        }

        private async void excuteUnloadCommand()
        {
            recordViewLog("");
            string vehicleId = cmb_Vehicle.Text;
            string cmd_id = scApp.SequenceBLL.getCommandID(SCAppConstants.GenOHxCCommandType.Manual);

            //string hostsource_portid = cmb_fromAddress.Text;
            string hostdest_portid = cmb_toAddress.Text;
            string to_adr = string.Empty;

            string cst_id = SCUtility.Trim(txt_cstID.Text, true);
            string box_id = "box_id";
            string lot_id = "lot_id";
            recordViewLog("");

            scApp.PortDefBLL.getAddressID(hostdest_portid, out to_adr);
            if (BCFUtility.isEmpty(vehicleId))
            {
                MessageBox.Show("No find vehile.");
                return;
            }
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vehicleId);


            sc.BLL.CMDBLL.OHTCCommandCheckResult check_result_info = null;
            recordViewLog("");
            await Task.Run(() =>
             {

                 {
                     scApp.CMDBLL.doCreatTransferCommand(vehicleId, string.Empty, cst_id, E_CMD_TYPE.Unload,
                                                 "", hostdest_portid, 0, 0,
                                                 box_id, lot_id, "", to_adr);
                     check_result_info = sc.BLL.CMDBLL.getCallContext<sc.BLL.CMDBLL.OHTCCommandCheckResult>
                    (sc.BLL.CMDBLL.CALL_CONTEXT_KEY_WORD_OHTC_CMD_CHECK_RESULT);
                 }
                 if (check_result_info != null && !check_result_info.IsSuccess)
                 {
                     MessageBox.Show(check_result_info.ToString(), "Command create fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 }

             });
            recordViewLog("");
        }


        private void excuteMoveCommandAllVh()
        {
            recordViewLog("");
            List<AVEHICLE> lstVh = scApp.VehicleBLL.cache.loadVhs();
            foreach (AVEHICLE vh in lstVh)
            {
                if (SCUtility.isEmpty(vh.CUR_ADR_ID) || SCUtility.isEmpty(vh.CUR_SEC_ID))
                {
                    continue;
                }

                ASECTION sec = scApp.MapBLL.getSectiontByID(vh.CUR_SEC_ID);

                string vehicleId = vh.VEHICLE_ID.Trim();
                string toAdr = sec.TO_ADR_ID;
                string[] nextSections = scApp.MapBLL.loadNextSectionIDBySectionID(vh.CUR_SEC_ID);
                ASECTION nextSection = null;
                if (nextSections != null && nextSections.Count() > 0)
                    nextSection = scApp.MapBLL.getSectiontByID(nextSections[0]);

                if (BCFUtility.isEmpty(vehicleId))
                {
                    MessageBox.Show("No find vehile.");
                    return;
                }
                recordViewLog("");

                Task.Run(() =>
                scApp.CMDBLL.doCreatTransferCommand(vehicleId, string.Empty, string.Empty,
                                                E_CMD_TYPE.Move,
                                                string.Empty,
                                                nextSection.TO_ADR_ID, 0, 0));
                SpinWait.SpinUntil(() => false, 1000);
            }
            recordViewLog("");
        }

        private void excuteTeachingCommand()
        {
            string vh_id = cmb_Vehicle.Text.Trim();
            string from_adr = cmb_fromAddress.Text;
            string to_adr = cmb_toAddress.Text;

            Task.Run(() => { scApp.VehicleService.TeachingRequest(vh_id, from_adr, to_adr); });
            recordViewLog("");

        }
        private void excuteHomeCommand()
        {
            string toAdr = string.Empty;
            string vehicleId = string.Empty;

            vehicleId = cmb_Vehicle.Text;
            toAdr = cmb_toAddress.Text;
            if (BCFUtility.isEmpty(vehicleId))
            {
                MessageBox.Show("No find vehile.");
                return;
            }
            recordViewLog("");

            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vehicleId);
            recordViewLog("");
            //Task.Run(() =>
            //scApp.VehicleService.TransferRequset(vehicleId, scApp.SequenceBLL.getCommandID(SCAppConstants.GenOHxCCommandType.Manual), ActiveType.Home, "", "", "", 
            //                                    new string[0], new string[0], new string[0], new string[0], "", "", "", ""));
            //vh.sned_Str31(scApp.SequenceBLL.getCommandID_Manual(), ActiveType.Home, "", new string[0], new string[0], "", ""));
        }

        private void excuteMTLHomeCommand()
        {
            //string toAdr = string.Empty;
            //string vehicleId = string.Empty;

            //vehicleId = cmb_Vehicle.Text;
            //toAdr = cmb_toAddress.Text;
            //if (BCFUtility.isEmpty(vehicleId))
            //{
            //    MessageBox.Show("No find vehile.");
            //    return;
            //}

            //AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vehicleId);

            //Task.Run(() =>
            //scApp.VehicleService.TransferRequset(vehicleId, scApp.SequenceBLL.getCommandID(SCAppConstants.GenOHxCCommandType.Manual), ActiveType.Mtlhome, "","","", new string[0], new string[0], new string[0], new string[0], "", "","",""));
            //vh.sned_Str31(scApp.SequenceBLL.getCommandID_Manual(), ActiveType.Home, "", new string[0], new string[0], "", ""));
        }



        private void timer_TimedUpdates_Tick(object sender, EventArgs e)
        {
            //using (DBConnection_EF context = new DBConnection_EF())
            //{
            //    dgv_vhStatus.DataSource = context.AVEHICLE.ToList();
            //}
            //if (currentSelectIndex != -1)
            //    dgv_vhStatus.Rows[currentSelectIndex].Selected = true;
            recordViewLog("");

            dgv_vhStatus.Refresh();
            updateTransferCommand();
            var line = scApp.getEQObjCacheManager().getLine();
            SetSCState(line);
            recordViewLog("");
        }
        private void SetSCState(ALINE line)
        {
            switch (line.SCStats)
            {
                case ALINE.TSCState.NONE:
                    lbl_HoseMode.BackColor = Color.Gray;
                    break;
                case ALINE.TSCState.AUTO:
                    lbl_HoseMode.BackColor = Color.Green;
                    break;
                case ALINE.TSCState.PAUSED:
                case ALINE.TSCState.PAUSING:
                case ALINE.TSCState.TSC_INIT:
                    lbl_HoseMode.BackColor = Color.Yellow;
                    break;
            }
            lbl_HoseMode.Text = line.SCStats.GetDisplayName();
            recordViewLog("");
        }



        private void refreshACMD_MCSInfoList(List<ACMD_MCS> currentExcuteTranCmd)
        {
            try
            {
                List<string> new_current_excute_tran_cmd = currentExcuteTranCmd.Select(cmd => SCUtility.Trim(cmd.CMD_ID, true)).ToList();
                List<string> old_current_excute_tran_cmd = cmd_mcs_obj_to_show.Select(cmd => cmd.CMD_ID).ToList();

                List<string> new_add_mcs_cmds = new_current_excute_tran_cmd.Except(old_current_excute_tran_cmd).ToList();
                //1.新增多出來的命令
                foreach (string new_cmd in new_add_mcs_cmds)
                {
                    var cmd_obj = currentExcuteTranCmd.Where(cmd => SCUtility.isMatche(cmd.CMD_ID, new_cmd)).FirstOrDefault();
                    if (cmd_obj == null) continue;
                    CMD_MCSObjToShow new_cmd_obj = new CMD_MCSObjToShow(mainform.BCApp.SCApplication.VehicleBLL, cmd_obj);
                    cmsMCS_bindingSource.Add(new_cmd_obj);
                }
                //2.刪除以結束的命令
                List<string> will_del_mcs_cmds = old_current_excute_tran_cmd.Except(new_current_excute_tran_cmd).ToList();
                foreach (string old_cmd in will_del_mcs_cmds)
                {
                    var cmd_obj = cmd_mcs_obj_to_show.Where(cmd => SCUtility.isMatche(cmd.CMD_ID, old_cmd)).FirstOrDefault();
                    cmsMCS_bindingSource.Remove(cmd_obj);
                }
                //3.更新現有命令
                foreach (var tran_obj_show_item in cmd_mcs_obj_to_show)
                {
                    var cmd_obj = currentExcuteTranCmd.Where(cmd => SCUtility.isMatche(cmd.CMD_ID, tran_obj_show_item.CMD_ID)).FirstOrDefault();
                    if (cmd_obj == null)
                    {
                        continue;
                    }
                    tran_obj_show_item.put(cmd_obj);
                }
                recordViewLog("");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, "Exception");
            }
        }

        private void updateTransferCommand()
        {
            var current_cmds_mcs = ACMD_MCS.loadCurrentExcuteCMD_MCS();
            refreshACMD_MCSInfoList(current_cmds_mcs);
            dgv_TransferCommand.Refresh();
            recordViewLog("");
            //List<ACMD_MCS> ACMD_MCSs = null;
            //await Task.Run(() => ACMD_MCSs = mainform.BCApp.SCApplication.CMDBLL.loadACMD_MCSIsUnfinished());
            //if (ACMD_MCSs != null)
            //{
            //    cmd_mcs_obj_to_show = ACMD_MCSs.Select(cmd => new CMD_MCSObjToShow(mainform.BCApp.SCApplication.VehicleBLL, cmd)).ToList();
            //    //cmd_mcs_obj_to_show = mainform.BCApp.SCApplication.CMDBLL.loadACMD_MCSIsUnfinishedObjToShow();
            //    cmsMCS_bindingSource.DataSource = cmd_mcs_obj_to_show;
            //    dgv_TransferCommand.Refresh();
            //}
        }


        int currentSelectIndex = -1;
        //Equipment InObservationVh = null;
        AVEHICLE InObservationVh = null;
        string predictPathHandler = "predictPathHandler";
        private void dgv_vhStatus_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                currentSelectIndex = e.RowIndex;
                string vh_id = dgv_vhStatus.Rows[currentSelectIndex].Cells[0].Value as string;

                setMonitorVehicle(vh_id);
            }
            recordViewLog("");
        }

        public void setMonitorVehicle(string vh_id)
        {
            recordViewLog("");
            lock (predictPathHandler)
            {
                if (InObservationVh != null)
                    InObservationVh.removeEventHandler(predictPathHandler);

                resetSpecifyRail();
                resetSpecifyAdr();
                recordViewLog("");

                if (!BCFUtility.isEmpty(vh_id))
                {
                    InObservationVh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);

                    changePredictPathByInObservation();

                    InObservationVh.addEventHandler(predictPathHandler
                                        , BCFUtility.getPropertyName(() => InObservationVh.VhExcuteCMDStatusChangeEvent)
                                        , (s1, e1) => { changePredictPathByInObservation(); });
                    InObservationVh.addEventHandler(predictPathHandler
                                        , BCFUtility.getPropertyName(() => InObservationVh.VhStatusChangeEvent)
                                        , (s1, e1) => { changePredictPathByInObservation(); });
                    recordViewLog("");
                    cmb_Vehicle.Text = vh_id;
                    VehicleObjToShow veicleObjShow = scApp.getEQObjCacheManager().CommonInfo.ObjectToShow_list.Where(o => o.VEHICLE_ID == vh_id).FirstOrDefault();
                    if (veicleObjShow != null)
                    {
                        int selectIndex = scApp.getEQObjCacheManager().CommonInfo.ObjectToShow_list.IndexOf(veicleObjShow);
                        if (selectIndex >= 0)
                        {
                            if ((dgv_vhStatus.SelectedRows.Count > 0 && dgv_vhStatus.SelectedRows[0].Index != selectIndex) ||
                                dgv_vhStatus.SelectedRows.Count == 0)
                            {
                                dgv_vhStatus.Rows[selectIndex].Selected = true;
                                dgv_vhStatus.FirstDisplayedScrollingRowIndex = selectIndex;
                            }
                        }
                    }
                    recordViewLog("");

                }
                else
                {
                    if (dgv_vhStatus.SelectedRows.Count > 0)
                        dgv_vhStatus.SelectedRows[0].Selected = false;
                    cmb_Vehicle.Text = string.Empty;

                }
            }
            recordViewLog("");
        }

        bool setCombBoxFlag = false;
        public void setAdrCombobox(string adr_id)
        {
            if (scApp.EquipmentBLL.cache.IsInMaintainDeviceRangeOfAddress(scApp.SegmentBLL, adr_id))
            {
                MessageBox.Show("Can't set maintain device range of address", "Set fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (setCombBoxFlag)
            {
                cmb_fromAddress.Text = adr_id;
                setCombBoxFlag = false;
            }
            else
            {
                cmb_toAddress.Text = adr_id;
                setCombBoxFlag = true;
            }
            recordViewLog("");

        }



        
        public void LeaveMonitorMode_SectionThroughTimes()
        {
            ck_montor_vh.Checked = true;
            //uctl_Map.LeaveMonitorMode();
            resetSpecifyRail();
            //uctl_Map1.DisplaySectionLables(false);
        }



        private void changePredictPathByInObservation()
        {
            //resetSpecifyRail();
            //resetSpecifyAdr();

            if (InObservationVh.ACT_STATUS == VHActionStatus.CycleRun)
            {
                resetSpecifyRail();
                resetSpecifyAdr();
                setSpecifyRail(InObservationVh.CyclingPath);
            }
            else
            {
                if (InObservationVh.vh_CMD_Status < E_CMD_STATUS.NormalEnd)
                {
                    setSpecifyRail(InObservationVh.PredictPath);
                    setSpecifyAdr();
                }
                else
                {
                    resetSpecifyRail();
                    resetSpecifyAdr();
                }
            }
            recordViewLog("");
        }

        string reqSelectionStartAdr = string.Empty;
        string reqSelectionFromAdr = string.Empty;
        string reqSelectionToAdr = string.Empty;
        private void setSpecifyAdr()
        {

        }
        private void resetSpecifyAdr()
        {

        }


        string[] preSelectionSec = null;
        private void setSpecifyRail(string[] spacifyPath)
        {
            if (spacifyPath == null)
                return;
            //如果之前的只是路徑不是Null，才需要比較兩個有沒有一樣去決定要不要更新只是路徑。
            if (preSelectionSec != null && BCFUtility.isMatche(preSelectionSec, spacifyPath))
                return;
            else
            {
                resetSpecifyRail();
                resetSpecifyAdr();
            }
            preSelectionSec = spacifyPath;
            //uctl_Map.changeSpecifyRailColor(spacifyPath);
        }
        private void resetSpecifyRail()
        {
            if (preSelectionSec != null)
                //uctl_Map.resetRailColor(preSelectionSec);
                preSelectionSec = null;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        private void dgv_vhStatus_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            recordViewLog("");
            logger.Error(e.Exception, "Exception");
            //todo error catch
        }

        private void btn_continuous_Click(object sender, EventArgs e)
        {
            recordViewLog("");
            string vh_id = cmb_Vehicle.Text.Trim();
            Task.Run(() =>
            {
                AVEHICLE noticeCar = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);

                if (noticeCar.IsPause)
                {
                    scApp.VehicleService.PauseRequest(vh_id, PauseEvent.Continue, SCAppConstants.OHxCPauseType.Normal);

                }
                else
                {

                    scApp.VehicleService.noticeVhPass(vh_id);
                }
            });
        }

        private void btn_pause_Click(object sender, EventArgs e)
        {
            recordViewLog("");
            string notice_vh_id = cmb_Vehicle.Text.Trim();
            Task.Run(() =>
            {

                if (scApp.VehicleService.PauseRequest(notice_vh_id, PauseEvent.Pause, SCAppConstants.OHxCPauseType.Normal))
                {

                }
            });
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            mainform.isAutoOpenTip = cb_autoTip.Checked;
        }

        private void cbm_Action_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmb_fromAddress.Enabled = false;
            cmb_toAddress.Visible = false;
            cmb_cycRunZone.Visible = false;
            btn_start.Enabled = false;
            btn_AutoMove.Enabled = false;
            lbl_destinationName.Text = "To Address";
            E_CMD_TYPE cmd_type;
            Enum.TryParse<E_CMD_TYPE>(cbm_Action.SelectedValue.ToString(), out cmd_type);
            switch (cmd_type)
            {
                case E_CMD_TYPE.Move:
                case E_CMD_TYPE.MoveToMTL:
                case E_CMD_TYPE.SystemIn:
                case E_CMD_TYPE.SystemOut:
                    cmb_toAddress.Visible = true;
                    btn_start.Enabled = true;
                    btn_AutoMove.Enabled = true;
                    break;
                case E_CMD_TYPE.Round:
                    cmb_cycRunZone.Visible = true;
                    btn_start.Enabled = true;
                    lbl_destinationName.Text = "Round Entry Adr.";
                    break;
                case E_CMD_TYPE.LoadUnload:
                case E_CMD_TYPE.Teaching:
                    cmb_fromAddress.Enabled = true;
                    cmb_toAddress.Visible = true;
                    btn_start.Enabled = true;
                    break;
                case E_CMD_TYPE.Home:
                case E_CMD_TYPE.MTLHome:
                    btn_start.Enabled = true;
                    break;
                case E_CMD_TYPE.Load:
                case E_CMD_TYPE.Scan:
                    cmb_fromAddress.Enabled = true;
                    btn_start.Enabled = true;
                    break;
                case E_CMD_TYPE.Unload:
                    cmb_toAddress.Visible = true;
                    btn_start.Enabled = true;
                    break;


            }
        }

        private void Raid_PortNameType_CheckedChanged(object sender, EventArgs e)
        {
            string source_name = string.Empty;
            string destination_name = string.Empty;
            if (Raid_PortNameType_AdrID.Checked)
            {
                source_name = "From Address";
                destination_name = "To Address";
                BCUtility.setComboboxDataSource(cmb_toAddress, allAdr_ID);
                BCUtility.setComboboxDataSource(cmb_fromAddress, allAdr_ID.ToArray());
            }
            else if (Raid_PortNameType_PortID.Checked)
            {
                source_name = "From Port";
                destination_name = "To Port";
                BCUtility.setComboboxDataSource(cmb_toAddress, allPortID);
                BCUtility.setComboboxDataSource(cmb_fromAddress, allPortID.ToArray());
            }
            lbl_sourceName.Text = source_name;
            lbl_destinationName.Text = destination_name;
        }

        private void ck_montor_vh_CheckedChanged(object sender, EventArgs e)
        {
            //if (ck_montor_vh.Checked)
            //{
            //    uctl_Map.trunOnMonitorAllVhStatus();
            //}
            //else
            //{
            //    uctl_Map.trunOffMonitorAllVhStatus();
            //}
        }

        private void btn_AutoMove_Click(object sender, EventArgs e)
        {
            Task.Run(() => excuteMoveCommandAllVh());
            //            string vehicleId = cmb_Vehicle.Text;

            //            Task.Run(() =>
            //scApp.VehicleService.TransferRequset(vehicleId, scApp.SequenceBLL.getCommandID(SCAppConstants.GenOHxCCommandType.Auto), ActiveType.Move, "", new string[] { "0402", "0412" }, new string[0], "", ""));

        }

        private void cb_sectionThroughTimes_Click(object sender, EventArgs e)
        {

        }


        private void OHT_Form_Load(object sender, EventArgs e)
        {
            ck_montor_vh.Checked = true;
            cmsMCS_bindingSource.DataSource = cmd_mcs_obj_to_show;
            dgv_TransferCommand.DataSource = cmsMCS_bindingSource;
        }


        private void cb_sectionThroughTimes_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btn_parkZoneTypeChange_Click(object sender, EventArgs e)
        {
            string selected_park_zone_type = cb_parkZoneType.SelectedItem as string;
            if (selected_park_zone_type == null) return;
            scApp.ParkBLL.doParkZoneTypeChange(selected_park_zone_type);
            MessageBox.Show("OK");
        }

        private void uctl_Map_Load(object sender, EventArgs e)
        {

        }

        private void cmb_Vehicle_SelectedIndexChanged(object sender, EventArgs e)
        {
            string vh_id = cmb_Vehicle.Text;
            if (SCUtility.isEmpty(vh_id)) return;
            AVEHICLE vh = scApp.VehicleBLL.cache.getVhByID(vh_id);
            if (vh == null) return;
            txt_cstID.Text = vh.HAS_CST == 1 ? SCUtility.Trim(vh.CST_ID, true) : "Manual_CST";
        }

        private void OHT_FormNew_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}