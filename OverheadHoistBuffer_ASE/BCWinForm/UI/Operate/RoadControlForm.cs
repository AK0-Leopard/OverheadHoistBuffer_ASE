using com.mirle.ibg3k0.bc.winform.App;
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc;
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
    public partial class RoadControlForm : Form
    {
        BCMainForm mainForm;
        OHT_Form oht_form;
        BCApplication bcApp;
        private Logger logger = LogManager.GetCurrentClassLogger();
        List<ASEGMENT> segment_List = null;
        ALINE line = null;
        public RoadControlForm(BCMainForm _mainForm)
        {
            InitializeComponent();
            this.TopMost = true;

            mainForm = _mainForm;
            oht_form = mainForm.OpenForms[nameof(OHT_Form)] as OHT_Form;
            bcApp = mainForm.BCApp;
            line = bcApp.SCApplication.getEQObjCacheManager().getLine();

            //segment_List = bcApp.SCApplication.MapBLL.loadAllSegments();
            segment_List = bcApp.SCApplication.SegmentBLL.cache.GetSegments();
            dgv_segment.AutoGenerateColumns = false;
            dgv_segment.DataSource = segment_List;

            bcApp.SCApplication.RoadControlService.SegmentListChanged += RoadControlService_SegmentListChanged;

            oht_form.entrySegmentSetMode(SegmentSelectOnMap);
            RefreshMapColor();
        }

        private void RoadControlService_SegmentListChanged(object sender, ASEGMENT e)
        {
            Adapter.Invoke((obj) =>
            {
                dgv_segment.Refresh();
                RefreshMapColor();
            }, null);

        }

        private async void btn_disable_Click(object sender, EventArgs e)
        {
            try
            {
                btn_disable.Enabled = false;
                await Task.Run(() => disableSegment(sc.Data.SECS.CSOT.SECSConst.LANECUTTYPE_LaneCutOnHMI));
                //dgv_segment.Refresh();
                //RefreshMapColor();
                //Task.Run(() => checkSegmentStatus());
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
            }
            finally
            {
                btn_disable.Enabled = true;
            }
        }

        private void disableSegment(string laneCutTypeg)
        {
            foreach (DataGridViewRow row in dgv_segment.SelectedRows)
            {
                ASEGMENT selectSeg = segment_List[row.Index];
                if (!selectSeg.DISABLE_FLAG_USER
                    && !selectSeg.PRE_DISABLE_FLAG)
                {
                    //segment_List[row.Index] = bcApp.SCApplication.MapBLL.PreDisableSegment(selectSeg.SEG_NUM.Trim());
                    //segment_List[row.Index] = bcApp.SCApplication.VehicleService.doEnableDisableSegment(selectSeg.SEG_NUM.Trim(), E_PORT_STATUS.OutOfService, laneCutTypeg);
                    //segment_List[row.Index] = bcApp.SCApplication.RoadControlService.doEnableDisableSegment
                    //    (selectSeg.SEG_NUM.Trim(), E_PORT_STATUS.OutOfService, ASEGMENT.DisableType.User, laneCutTypeg);
                    bcApp.SCApplication.RoadControlService.doEnableDisableSegment
                        (selectSeg.SEG_NUM.Trim(), E_PORT_STATUS.OutOfService, ASEGMENT.DisableType.User, laneCutTypeg);
                }
            }
        }

        private void cb_seg_id_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void btn_enable_Click(object sender, EventArgs e)
        {
            try
            {
                btn_enable.Enabled = false;

                await Task.Run(() => enableSegment());
                //dgv_segment.Refresh();
                //RefreshMapColor();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
            }
            finally
            {
                btn_enable.Enabled = true;
            }
        }

        private void enableSegment()
        {
            foreach (DataGridViewRow row in dgv_segment.SelectedRows)
            {
                ASEGMENT selectSeg = segment_List[row.Index];
                if (selectSeg.DISABLE_FLAG_USER)
                {
                    //  segment_List[row.Index] = bcApp.SCApplication.RouteGuide.OpenSegment(selectSeg.SEG_NUM.Trim());
                    //segment_List[row.Index] = bcApp.SCApplication.VehicleService.doEnableDisableSegment
                    /*segment_List[row.Index] =*/
                    bcApp.SCApplication.RoadControlService.doEnableDisableSegment
                                                  (selectSeg.SEG_NUM.Trim(),
                                                  E_PORT_STATUS.InService, ASEGMENT.DisableType.User,
                                                  sc.Data.SECS.CSOT.SECSConst.LANECUTTYPE_LaneCutOnHMI);
                }
            }
        }

        private async void btn_cancel_Click(object sender, EventArgs e)
        {
            try
            {
                btn_cancel.Enabled = false;
                await Task.Run(() => cancelDisableSegment());
                dgv_segment.Refresh();
                RefreshMapColor();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                btn_cancel.Enabled = true;

            }
        }

        private void cancelDisableSegment()
        {
            foreach (DataGridViewRow row in dgv_segment.SelectedRows)
            {
                ASEGMENT Seg = segment_List[row.Index];
                if (Seg.STATUS == E_SEG_STATUS.Active
                    && Seg.PRE_DISABLE_FLAG)
                {
                    // segment_List[row.Index] = bcApp.SCApplication.RouteGuide.OpenSegment(Seg.SEG_NUM.Trim());
                    //segment_List[row.Index] = bcApp.SCApplication.VehicleService.doEnableDisableSegment
                    /*segment_List[row.Index] = */
                    bcApp.SCApplication.RoadControlService.doEnableDisableSegment
                                                  (Seg.SEG_NUM.Trim(),
                                                  E_PORT_STATUS.InService, ASEGMENT.DisableType.User,
                                                  sc.Data.SECS.CSOT.SECSConst.LANECUTTYPE_LaneCutOnHMI);
                }
            }
        }

        private void BtnRefesh(string seg_id)
        {
            var segment = bcApp.SCApplication.MapBLL.getSegmentByID(seg_id);
            btn_enable.Enabled = false;
            btn_disable.Enabled = false;
            btn_cancel.Enabled = false;
            if (segment != null)
            {
                if (segment.PRE_DISABLE_FLAG)
                {
                    btn_cancel.Enabled = true;
                }
                else
                {
                    if (segment.STATUS == sc.E_SEG_STATUS.Active)
                    {
                        btn_disable.Enabled = true;
                    }
                    else
                    {
                        btn_enable.Enabled = true;
                    }
                }
            }
        }

        //private long checkSyncPoint = 0;
        //private void checkSegmentStatus()
        //{
        //    if (Interlocked.Exchange(ref checkSyncPoint, 1) == 0)
        //    {
        //        try
        //        {
        //            bool hasPrepare = false;
        //            bcApp.SCApplication.LineBLL.BegingOrEndSegmentPreDisableExcute(true);
        //            do
        //            {

        //                foreach (ASEGMENT seg in segment_List)
        //                {
        //                    if (seg.PRE_DISABLE_FLAG)
        //                    {
        //                        bool canDisable = false;
        //                        //1.確認是否有在Disable前的命令還沒執行完
        //                        canDisable = bcApp.SCApplication.CMDBLL.getCMD_MCSIsRunningCount(seg.PRE_DISABLE_TIME.Value) == 0;
        //                        //2.確認是否還有命令還會通過這裡
        //                        if (canDisable)
        //                        {
        //                            List<string> will_be_pass_cmd_ids = null;
        //                            canDisable = !bcApp.SCApplication.CMDBLL.HasCmdWillPassSegment(seg.SEG_NUM, out will_be_pass_cmd_ids);
        //                        }
        //                        //3.確認是否還有VH在管制道路上
        //                        if (canDisable)
        //                        {
        //                            List<AVEHICLE> listVH = bcApp.SCApplication.getEQObjCacheManager().getAllVehicle();
        //                            foreach (AVEHICLE vh in listVH)
        //                            {
        //                                ASECTION vh_current_sec = bcApp.SCApplication.MapBLL.getSectiontByID(vh.CUR_SEC_ID);
        //                                if (vh_current_sec != null && SCUtility.isMatche(vh_current_sec.SEG_NUM, seg.SEG_NUM))
        //                                {
        //                                    if (vh.ACT_STATUS != sc.ProtocolFormat.OHTMessage.VHActionStatus.NoCommand)
        //                                        canDisable = false;
        //                                    break;
        //                                }
        //                            }
        //                        }

        //                        if (canDisable)
        //                        {
        //                            int index = segment_List.IndexOf(seg);
        //                            string seg_num = seg.SEG_NUM;
        //                            var newSeg = bcApp.SCApplication.RouteGuide.CloseSegment(seg_num.Trim());
        //                            var orderSeg = segment_List[index];
        //                            BCFUtility.setValueToPropety(ref newSeg, ref orderSeg);
        //                            Adapter.Invoke((obj) =>
        //                            {
        //                                dgv_segment.Refresh();
        //                                RefreshMapColor();
        //                            }, null);
        //                            hasPrepare = false;
        //                        }
        //                        else
        //                        {
        //                            hasPrepare = true;

        //                        }
        //                    }
        //                }
        //                SpinWait.SpinUntil(() => false, 1000);
        //            }
        //            while (hasPrepare);
        //        }
        //        catch (Exception ex)
        //        {
        //            logger.Error(ex, "Exection:");
        //        }
        //        finally
        //        {
        //            Interlocked.Exchange(ref checkSyncPoint, 0);
        //            bcApp.SCApplication.LineBLL.BegingOrEndSegmentPreDisableExcute(false);
        //        }
        //    }
        //}

        private void SegmentSelectOnMap(object sender, EventArgs _args)
        {
            var args = _args as Components.SelectedRailEventArgs;
            ASEGMENT seg_obj = segment_List.Where(seg => seg.SEG_NUM.Trim() == args.Segment_Num.Trim()).FirstOrDefault();
            if (seg_obj == null) return;
            int index = segment_List.IndexOf(seg_obj);
            dgv_segment.FirstDisplayedScrollingRowIndex = index;
            dgv_segment.Rows[index].Selected = true;
            RefreshMapColor();
        }

        #region 保留
        //private void checkSegmentStatus()
        //{
        //    if (Interlocked.Exchange(ref checkSyncPoint, 1) == 0)
        //    {
        //        try
        //        {
        //            bool hasPrepare = false;
        //            bcApp.SCApplication.LineBLL.BegingOrEndSegmentPreDisableExcute(true);
        //            do
        //            {

        //                foreach (ASEGMENT seg in segment_List)
        //                {
        //                    if (seg.PRE_DISABLE_FLAG)
        //                    {
        //                        bool canDisable = false;
        //                        if (IsRealTime)
        //                        {
        //                            List<string> will_be_pass_cmd_ids = null;
        //                            bool HasCmdWillPass = bcApp.SCApplication.CMDBLL.HasCmdWillPassSegment(seg.SEG_NUM, out will_be_pass_cmd_ids);
        //                            if (HasCmdWillPass)
        //                            {
        //                                foreach (string cmd_id in will_be_pass_cmd_ids)
        //                                {
        //                                    ACMD_OHTC will_pass_cmd = bcApp.SCApplication.CMDBLL.getExcuteCMD_OHTCByCmdID(cmd_id);
        //                                    if (will_pass_cmd == null)
        //                                        continue;
        //                                    AVEHICLE excute_vh = bcApp.SCApplication.getEQObjCacheManager().getVehicletByVHID(will_pass_cmd.VH_ID);
        //                                    sc.ProtocolFormat.OHTMessage.CMDCancelType cMDCancelType = default(sc.ProtocolFormat.OHTMessage.CMDCancelType);
        //                                    E_CMD_STATUS e_CMD_STATUS = default(E_CMD_STATUS);

        //                                    ASECTION crtSection = bcApp.SCApplication.MapBLL.getSectiontByID(excute_vh.CUR_SEC_ID);

        //                                    if (SCUtility.isMatche(crtSection.SEG_NUM, seg.SEG_NUM))
        //                                    {
        //                                        continue;
        //                                    }

        //                                    if (excute_vh.HAS_CST == 0)
        //                                    {
        //                                        e_CMD_STATUS = E_CMD_STATUS.Canceling;
        //                                        cMDCancelType = sc.ProtocolFormat.OHTMessage.CMDCancelType.CmdCancel;
        //                                    }
        //                                    else
        //                                    {
        //                                        e_CMD_STATUS = E_CMD_STATUS.Aborting;
        //                                        cMDCancelType = sc.ProtocolFormat.OHTMessage.CMDCancelType.CmdAbout;
        //                                    }
        //                                    if (excute_vh.sned_Str37(will_pass_cmd.CMD_ID, cMDCancelType))
        //                                    {
        //                                        bcApp.SCApplication.CMDBLL.updateCommand_OHTC_StatusByCmdID(will_pass_cmd.CMD_ID, e_CMD_STATUS);
        //                                    }

        //                                }
        //                            }
        //                            canDisable = !HasCmdWillPass;
        //                            if (canDisable)
        //                            {
        //                                List<ACMD_MCS> lstACMD_MCS = bcApp.SCApplication.CMDBLL.loadCMD_MCS_IsExcute(seg.PRE_DISABLE_TIME.Value);
        //                                if (lstACMD_MCS != null && lstACMD_MCS.Count > 0)
        //                                {
        //                                    List<string> adrs_SourceAndDestination = new List<string>();
        //                                    foreach (ACMD_MCS cmd in lstACMD_MCS)
        //                                    {
        //                                        string fromAdr = string.Empty;
        //                                        string toAdr = string.Empty;
        //                                        if (bcApp.SCApplication.MapBLL.getAddressID(cmd.HOSTSOURCE, out fromAdr))
        //                                            adrs_SourceAndDestination.Add(fromAdr);
        //                                        if (bcApp.SCApplication.MapBLL.getAddressID(cmd.HOSTDESTINATION, out toAdr))
        //                                            adrs_SourceAndDestination.Add(toAdr);
        //                                    }
        //                                    List<ASECTION> sections_SourceAndDestination =
        //                                          bcApp.SCApplication.MapBLL.loadSectionByToAdrs(adrs_SourceAndDestination);
        //                                    List<string> segments_SourceAndDestination = sections_SourceAndDestination.Select(sec => sec.SEG_NUM.Trim()).ToList();
        //                                    canDisable = !segments_SourceAndDestination.Contains(seg.SEG_NUM.Trim());
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            //1.確認是否有在Disable前的命令還沒執行完
        //                            canDisable = bcApp.SCApplication.CMDBLL.getCMD_MCSIsRunningCount(seg.PRE_DISABLE_TIME.Value) == 0;
        //                            //2.確認是否還有命令還會通過這裡
        //                            if (canDisable)
        //                            {

        //                                List<string> will_be_pass_cmd_ids = null;
        //                                canDisable = !bcApp.SCApplication.CMDBLL.HasCmdWillPassSegment(seg.SEG_NUM, out will_be_pass_cmd_ids);
        //                            }
        //                            //3.確認是否還有VH在管制道路上
        //                            if (canDisable)
        //                            {
        //                                List<AVEHICLE> listVH = bcApp.SCApplication.getEQObjCacheManager().getAllVehicle();
        //                                foreach (AVEHICLE vh in listVH)
        //                                {
        //                                    ASECTION vh_current_sec = bcApp.SCApplication.MapBLL.getSectiontByID(vh.CUR_SEC_ID);
        //                                    if (SCUtility.isMatche(vh_current_sec.SEG_NUM, seg.SEG_NUM))
        //                                    {
        //                                        canDisable = false;
        //                                        break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        if (canDisable)
        //                        {
        //                            int index = segment_List.IndexOf(seg);
        //                            string seg_num = seg.SEG_NUM;
        //                            var newSeg = bcApp.SCApplication.RouteGuide.CloseSegment(seg_num.Trim());
        //                            var orderSeg = segment_List[index];
        //                            BCFUtility.setValueToPropety(ref newSeg, ref orderSeg);
        //                            Adapter.Invoke((obj) =>
        //                            {
        //                                dgv_segment.Refresh();
        //                            }, null);
        //                            hasPrepare = false;
        //                        }
        //                        else
        //                        {
        //                            hasPrepare = true;
        //                        }
        //                    }
        //                }
        //                SpinWait.SpinUntil(() => false, 1000);
        //            }
        //            while (hasPrepare);
        //        }
        //        catch (Exception ex)
        //        {
        //            logger.Error(ex, "Exection:");
        //        }
        //        finally
        //        {
        //            Interlocked.Exchange(ref checkSyncPoint, 0);
        //            bcApp.SCApplication.LineBLL.BegingOrEndSegmentPreDisableExcute(false);
        //        }
        //    }
        //}
        #endregion 保留



        private void RoadControlForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bcApp.SCApplication.getEQObjCacheManager().getLine().SegmentPreDisableExcuting)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Segment is changing state to disable. please wait or cancel.")
                    .AppendLine("segment num:");
                foreach (ASEGMENT seg in segment_List)
                {
                    if (seg.PRE_DISABLE_FLAG)
                    {
                        sb.Append(seg.SEG_NUM.Trim()).Append(',');
                    }
                }
                BCFApplication.onWarningMsg(sb.ToString());
                e.Cancel = true;
            }
        }

        private void RoadControlForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainForm.removeForm(this.Name);
            bcApp.SCApplication.RoadControlService.SegmentListChanged -= RoadControlService_SegmentListChanged;
            oht_form.LeaveSegmentSetMode(SegmentSelectOnMap);
            oht_form.ResetAllSegment();
        }



        private void RefreshMapColor()
        {
            foreach (ASEGMENT seg in segment_List)
            {
                int index = segment_List.IndexOf(seg);
                if (dgv_segment.Rows[index].Selected)
                {
                    oht_form.SetSpecifySegmentSelected(seg.SEG_NUM, Color.LightGreen);
                }
                else if (seg.PRE_DISABLE_FLAG)
                {
                    oht_form.SetSpecifySegmentSelected(seg.SEG_NUM, Color.Pink);
                }
                else if (seg.STATUS == E_SEG_STATUS.Closed)
                {
                    oht_form.SetSpecifySegmentSelected(seg.SEG_NUM, Color.Red);
                }
                else
                {
                    oht_form.ResetSpecifySegmentSelected(seg.SEG_NUM);
                }
            }
        }

        private void dgv_segment_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            RefreshMapColor();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private async void btn_disable_vh_Click(object sender, EventArgs e)
        {
            try
            {
                btn_disable_vh.Enabled = false;
                await Task.Run(() => disableSegment(sc.Data.SECS.CSOT.SECSConst.LANECUTTYPE_LaneCutVehicle));
                dgv_segment.Refresh();
                RefreshMapColor();
                //Task.Run(() => checkSegmentStatus());
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
            }
            finally
            {
                btn_disable_vh.Enabled = true;
            }


        }

        private async void btn_segment_enable_cv_Click(object sender, EventArgs e)
        {
            try
            {
                btn_segment_enable_cv.Enabled = false;
                DataGridViewRow row = dgv_segment.SelectedRows.Count > 0 ? dgv_segment.SelectedRows[0] : null;
                if (row == null) return;
                ASEGMENT Seg = segment_List[row.Index];
                List<sc.Data.VO.OHCV> ohcvs = bcApp.SCApplication.EquipmentBLL.cache.loadOHCVDevicesBySegmentLocation(Seg.SEG_NUM);
                if (ohcvs == null || ohcvs.Count == 0)
                {
                    MessageBox.Show($"Segment ID:{Seg.SEG_NUM} not is cv of segemnt.", "fail.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (!Seg.DISABLE_FLAG_SAFETY)
                {
                    MessageBox.Show($"Segment ID:{Seg.SEG_NUM} of safty flag already off.", "fail.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                foreach (sc.Data.VO.OHCV ohcv in ohcvs)
                {
                    if (!ohcv.DoorClosed)
                    {
                        MessageBox.Show($"ohcv ID:{ohcv.EQPT_ID} of door closed:{ohcv.DoorClosed} ,can't enable segment.", "fail.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (!ohcv.Is_Eq_Alive)
                    {
                        MessageBox.Show($"ohcv ID:{ohcv.EQPT_ID} of alive:{ohcv.Is_Eq_Alive} ,can't enable segment.", "fail.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                bool is_success = false;
                await Task.Run(() =>
                   //bcApp.SCApplication.RoadControlService.doEnableDisableSegment
                   //                  (Seg.SEG_NUM.Trim(),
                   //                  E_PORT_STATUS.InService, ASEGMENT.DisableType.Safety,
                   //                  sc.Data.SECS.CSOT.SECSConst.LANECUTTYPE_LaneCutOnHMI)
                   is_success = bcApp.SCApplication.RoadControlService.RecoverCVEnable(Seg.SEG_NUM)
                              );
                string s_is_success = is_success ? "Success" : "Fail";
                MessageBox.Show($"Segment enable is:{s_is_success}", s_is_success, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch { }
            finally
            {
                btn_segment_enable_cv.Enabled = true;
            }
        }
    }
}
