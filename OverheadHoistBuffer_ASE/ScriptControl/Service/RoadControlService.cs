using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using NLog;
using RouteKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace com.mirle.ibg3k0.sc.Service
{
    public class RoadControlService
    {
        private EventHandler<ASEGMENT> segmentListChanged;
        private object _segmentListChangedLock = new object();
        public event EventHandler<ASEGMENT> SegmentListChanged
        {
            add
            {
                lock (_segmentListChangedLock)
                {
                    segmentListChanged -= value;
                    segmentListChanged += value;
                }
            }
            remove
            {
                lock (_segmentListChangedLock)
                {
                    segmentListChanged -= value;
                }
            }
        }

        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        BLL.LineBLL LineBLL = null;
        BLL.CMDBLL CMDBLL = null;
        BLL.VehicleBLL VehicleBLL = null;
        BLL.MapBLL MapBLL = null;
        BLL.SegmentBLL SegmentBLL = null;
        BLL.SectionBLL SectionBLL = null;
        BLL.PortStationBLL PortStationBLL = null;
        BLL.ReportBLL ReportBLL = null;
        BLL.NodeBLL NodeBLL = null;

        Service.VehicleService VehicleService = null;
        Service.LineService LineService = null;
        Guide RouteGuide = null;

        App.SCApplication app = null;
        public void start(SCApplication _app)
        {
            LineBLL = _app.LineBLL;
            CMDBLL = _app.CMDBLL;
            VehicleBLL = _app.VehicleBLL;
            MapBLL = _app.MapBLL;
            SegmentBLL = _app.SegmentBLL;
            SectionBLL = _app.SectionBLL;
            PortStationBLL = _app.PortStationBLL;
            ReportBLL = _app.ReportBLL;
            VehicleService = _app.VehicleService;
            LineService = _app.LineService;

            RouteGuide = _app.RouteGuide;

            NodeBLL = _app.NodeBLL;
            app = _app;

        }

        public void doEnableDisableSegment(string segment_id, E_PORT_STATUS port_status, string laneCutType)
        {
            ASEGMENT segment = null;
            try
            {
                List<APORTSTATION> port_stations = PortStationBLL.OperateCatch.loadAllPortBySegmentID(segment_id, SectionBLL);

                using (TransactionScope tx = SCUtility.getTransactionScope())
                {
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        switch (port_status)
                        {
                            case E_PORT_STATUS.InService:
                                segment = RouteGuide.OpenSegment(segment_id);
                                break;
                            case E_PORT_STATUS.OutOfService:
                                segment = RouteGuide.CloseSegment(segment_id);
                                break;
                        }
                        foreach (APORTSTATION port_station in port_stations)
                        {
                            PortStationBLL.OperateDB.updatePortStationStatus(port_station.PORT_ID, port_status);
                            PortStationBLL.OperateCatch.updatePortStationStatus(port_station.PORT_ID, port_status);
                        }
                        tx.Complete();
                    }
                }
                List<AMCSREPORTQUEUE> reportqueues = new List<AMCSREPORTQUEUE>();
                List<ASECTION> sections = SectionBLL.cache.loadSectionsBySegmentID(segment_id);
                string segment_start_adr = sections.First().FROM_ADR_ID;
                string segment_end_adr = sections.Last().TO_ADR_ID;
                switch (port_status)
                {
                    case E_PORT_STATUS.InService:
                        ReportBLL.newReportLaneInService(segment_start_adr, segment_end_adr, laneCutType, reportqueues);
                        break;
                    case E_PORT_STATUS.OutOfService:
                        ReportBLL.newReportLaneOutOfService(segment_start_adr, segment_end_adr, laneCutType, reportqueues);
                        break;
                }
                foreach (APORTSTATION port_station in port_stations)
                {
                    switch (port_status)
                    {
                        case E_PORT_STATUS.InService:
                            ReportBLL.newReportPortInServeice(port_station.PORT_ID, reportqueues);
                            break;
                        case E_PORT_STATUS.OutOfService:
                            ReportBLL.newReportPortOutOfService(port_station.PORT_ID, reportqueues);
                            break;
                    }
                }
                ReportBLL.newSendMCSMessage(reportqueues);
            }
            catch (Exception ex)
            {
                segment = null;
                logger.Error(ex, "Exception:");
            }
            //return segment;
        }


        public void doEnableDisableSegment(string segment_id, E_PORT_STATUS status, ASEGMENT.DisableType disableType, string laneCutType)
        {
            ASEGMENT seg_do = null;
            try
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(RoadControlService), Device: "OHxC",
                         Data: $"Start process segment:{segment_id} excute:{status} ,disable type:{disableType} ,lane cut type:{laneCutType}");

                bool is_status_change = false;
                ASEGMENT seg_vo = SegmentBLL.cache.GetSegment(segment_id);
                lock (seg_vo)
                {
                    List<APORTSTATION> port_stations = PortStationBLL.OperateCatch.loadAllPortBySegmentID(segment_id, SectionBLL);
                    using (TransactionScope tx = SCUtility.getTransactionScope())
                    {
                        using (DBConnection_EF con = DBConnection_EF.GetUContext())
                        {
                            switch (status)
                            {
                                case E_PORT_STATUS.InService:
                                    seg_do = RouteGuide.OpenSegment(segment_id, disableType);
                                    break;
                                case E_PORT_STATUS.OutOfService:
                                    seg_do = RouteGuide.CloseSegment(segment_id, disableType);
                                    break;
                            }
                            is_status_change = seg_vo.STATUS != seg_do.STATUS;
                            if (is_status_change)
                            {
                                foreach (APORTSTATION port_station in port_stations)
                                {
                                    PortStationBLL.OperateDB.updatePortStationStatus(port_station.PORT_ID, status);
                                    PortStationBLL.OperateCatch.updatePortStationStatus(port_station.PORT_ID, status);
                                }
                            }
                            tx.Complete();
                        }
                    }
                    seg_vo.put(seg_do);
                    segmentListChanged?.Invoke(this, seg_vo);

                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(RoadControlService), Device: "OHxC",
                             Data: $"end process segment:{segment_id} excute:{status} ,disable type:{disableType} ,lane cut type:{laneCutType}" +
                                   $"finial status:{seg_vo.STATUS},DISABLE_FLAG_USER:{seg_vo.DISABLE_FLAG_USER},DISABLE_FLAG_SAFETY:{seg_vo.DISABLE_FLAG_SAFETY}" +
                                   $",DISABLE_FLAG_HID:{seg_vo.DISABLE_FLAG_HID},DISABLE_FLAG_SYSTEM:{seg_vo.DISABLE_FLAG_SYSTEM}");

                    if (is_status_change)
                    {
                        List<AMCSREPORTQUEUE> reportqueues = new List<AMCSREPORTQUEUE>();
                        List<ASECTION> sections = SectionBLL.cache.loadSectionsBySegmentID(segment_id);
                        string segment_start_adr = sections.First().FROM_ADR_ID;
                        string segment_end_adr = sections.Last().TO_ADR_ID;
                        switch (seg_vo.STATUS)
                        {
                            case E_SEG_STATUS.Active:
                                ReportBLL.newReportLaneInService(segment_start_adr, segment_end_adr, laneCutType, reportqueues);
                                break;
                            case E_SEG_STATUS.Closed:
                                ReportBLL.newReportLaneOutOfService(segment_start_adr, segment_end_adr, laneCutType, reportqueues);
                                break;
                        }
                        foreach (APORTSTATION port_station in port_stations)
                        {
                            switch (seg_vo.STATUS)
                            {
                                case E_SEG_STATUS.Active:
                                    ReportBLL.newReportPortInServeice(port_station.PORT_ID, reportqueues);
                                    break;
                                case E_SEG_STATUS.Closed:
                                    ReportBLL.newReportPortOutOfService(port_station.PORT_ID, reportqueues);
                                    break;
                            }
                        }
                        ReportBLL.newSendMCSMessage(reportqueues);
                    }
                }
            }
            catch (Exception ex)
            {
                seg_do = null;
                logger.Error(ex, "Exception:");
            }
            //return seg_do;
        }


        public bool RecoverCVEnable(string segmentID)
        {
            try
            {
                //取得CV所在的Node，By Segment ID
                string seg_id = SCUtility.Trim(segmentID, true);
                ANODE node = NodeBLL.OperateCatch.getNodeBySegment(seg_id);
                doEnableDisableSegment(seg_id,
                                       E_PORT_STATUS.InService, ASEGMENT.DisableType.Safety,
                                       sc.Data.SECS.CSOT.SECSConst.LANECUTTYPE_LaneCutOnHMI);

                foreach (var ohcv in node.getSubEqptList())
                {
                    LineService.ProcessAlarmReport(
                        ohcv.NODE_ID, ohcv.EQPT_ID, ohcv.Real_ID, "",
                        SCAppConstants.SystemAlarmCode.OHCV_Issue.CVALLAlarmReset,
                        ProtocolFormat.OHTMessage.ErrorStatus.ErrReset);
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                return false;
            }
        }


        public void ProcessCVOpInRequest(Data.VO.OHCV ohcv)
        {
            if (ohcv == null)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(RoadControlService), Device: "OHxC",
                         Data: $"Start process ohcv op in request, but ohcv object is null.");
                return;
            }

            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(RoadControlService), Device: "OHxC",
                     Data: $"Start process ohcv:{ohcv?.EQPT_ID} op in request...",
                     VehicleID: ohcv?.EQPT_ID);
            string segment_location = ohcv.SegmentLocation;
            ASEGMENT pre_control_segment_vo = SegmentBLL.cache.GetSegment(segment_location);
            if (System.Threading.Interlocked.Exchange(ref pre_control_segment_vo.segment_prepare_control_SyncPoint, 1) == 0)
            {
                try
                {
                    //將segment 更新成 pre disable
                    ASEGMENT pre_control_segment_do = MapBLL.PreDisableSegment(segment_location);
                    pre_control_segment_vo.put(pre_control_segment_do);
                    segmentListChanged?.Invoke(this, pre_control_segment_vo);

                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(RoadControlService), Device: "OHxC",
                             Data: $"pre disable segment{segment_location} success.",
                             VehicleID: ohcv.EQPT_ID);
                    bool is_road_clear = WaitRoadClear(pre_control_segment_vo, ohcv);
                    if (is_road_clear)
                    {
                        doEnableDisableSegment(segment_location, E_PORT_STATUS.OutOfService, ASEGMENT.DisableType.Safety, Data.SECS.CSOT.SECSConst.LANECUTTYPE_LaneCutOnHMI);
                        pre_control_segment_vo.NotifyControlComplete();
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(RoadControlService), Device: "OHxC",
                                 Data: $"disable segment{segment_location} success.",
                                 VehicleID: ohcv.EQPT_ID);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception:");
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref pre_control_segment_vo.segment_prepare_control_SyncPoint, 0);
                }
            }
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(RoadControlService), Device: "OHxC",
                     Data: $"End process ohcv:{ohcv?.EQPT_ID} op in request.",
                     VehicleID: ohcv?.EQPT_ID);
        }

        public void ProcessOHCVAbnormallyScenario(ANODE node)
        {
            if (node == null)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(RoadControlService), Device: "OHxC",
                         Data: $"Start process ohcv node abnormally scenario, but ohcv node object is null.");
                return;
            }

            string segment_location = node.SegmentLocation;
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(RoadControlService), Device: "OHxC",
                     Data: $"Start process ohcv node id:{node?.NODE_ID} abnormal scenario...",
                     VehicleID: node?.NODE_ID);
            //直接將該段Segment封閉
            doEnableDisableSegment(segment_location, E_PORT_STATUS.OutOfService, ASEGMENT.DisableType.Safety, Data.SECS.CSOT.SECSConst.LANECUTTYPE_LaneCutOnHMI);

            //當CV的門突然被開啟時，要即將經過該CV所在的VH下達Pause的命令
            List<string> will_be_pass_cmd_ids = null;
            bool has_cmd_will_pass = CMDBLL.HasCmdWillPassSegment(segment_location, out will_be_pass_cmd_ids);
            if (has_cmd_will_pass)
            {
                foreach (string cmd_id in will_be_pass_cmd_ids)
                {
                    ACMD_OHTC cmd_obj = CMDBLL.getExcuteCMD_OHTCByCmdID(cmd_id);
                    if (cmd_obj != null)
                    {
                        //要改成一直下達pause命令，直到OHT回復成功為止。
                        Task.Run(() => ProcessUrgentPauseBySafty(cmd_obj.VH_ID));
                    }
                }
            }
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(RoadControlService), Device: "OHxC",
                     Data: $"End process ohcv node id:{node?.NODE_ID} abnormal scenario.",
                     VehicleID: node?.NODE_ID);

        }

        private void ProcessUrgentPauseBySafty(string vhID)
        {
            bool is_success = false;
            AVEHICLE need_urgent_pause_vh = VehicleBLL.cache.getVhByID(vhID);
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(RoadControlService), Device: "OHxC",
                     Data: $"Start urgent pause oht:{vhID}...",
                     VehicleID: vhID);
            do
            {
                is_success = VehicleService.
                        PauseRequest(vhID, ProtocolFormat.OHTMessage.PauseEvent.Pause, SCAppConstants.OHxCPauseType.Safty);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(RoadControlService), Device: "OHxC",
                         Data: $"End urgent pause oht:{vhID}.result:{is_success},is connection:{need_urgent_pause_vh.isTcpIpConnect}",
                         VehicleID: vhID);
            } while (need_urgent_pause_vh.isTcpIpConnect && !is_success);
        }






        private bool WaitRoadClear(ASEGMENT preControlSegment, Data.VO.OHCV requestOHCV)
        {
            try
            {
                bool canDisable = true;
                string pre_disable_seg_id = preControlSegment.SEG_NUM;

                return canDisable;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
                return false;
            }
        }

        private bool IsOHCVNodeStatusNormal(Data.VO.OHCV requestOHCV)
        {
            ANODE ohcv_node = app.getEQObjCacheManager().getNodeByNodeID(requestOHCV.NODE_ID);
            return ohcv_node.Is_Alive && ohcv_node.DoorClosed;
        }


    }

}
