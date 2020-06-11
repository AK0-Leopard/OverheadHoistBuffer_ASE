using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.Data.VO.Interface;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.mirle.ibg3k0.sc.Service
{
    public class OHCVService
    {
        private SCApplication scApp = null;
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public OHCVService()
        {
        }
        public void start(SCApplication app)
        {
            scApp = app;
            List<ANODE> nodes = app.getEQObjCacheManager().getAllNode();
            foreach (var node in nodes)
            {
                if (node.NODE_ID.StartsWith("CV"))
                {
                    node.addEventHandler(nameof(OHCVService), nameof(node.Is_Alive), aliveChange);
                    node.addEventHandler(nameof(OHCVService), nameof(node.DoorClosed), doorClosedChange);
                    node.addEventHandler(nameof(OHCVService), nameof(node.SafetyCheckComplete), safetyCheckCompleteChange);
                }
            }
        }

        public void aliveChange(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                ANODE node = sender as ANODE;
                if (sender == null) return;
                if (!node.Is_Alive)
                {
                    BCFApplication.onWarningMsg($"OHCV:[{node.NODE_ID}] alive is not changing.");
                    //對CV所在路段的OHT下pause Todo by Kevin
                    scApp.RoadControlService.ProcessOHCVAbnormallyScenario(node);
                    //上報Alarm給MCS Todo by Kevin
                    foreach (var ohcv in node.getSubEqptList())
                    {
                        if (!ohcv.Is_Eq_Alive)
                        {
                            scApp.LineService.ProcessAlarmReport(
                                ohcv.NODE_ID, ohcv.EQPT_ID, ohcv.Real_ID, "",
                                SCAppConstants.SystemAlarmCode.OHCV_Issue.CVOfAliveSignalAbnormal,
                                ProtocolFormat.OHTMessage.ErrorStatus.ErrSet);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        public void doorClosedChange(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                ANODE node = sender as ANODE;
                if (sender == null) return;
                if (!node.DoorClosed)
                {
                    if (!node.SafetyCheckComplete)
                    {
                        BCFApplication.onWarningMsg($"OHCV:[{node.NODE_ID}] door has been open without authorization.");
                        //對CV所在路段的OHT下pause Todo by Kevin
                        scApp.RoadControlService.ProcessOHCVAbnormallyScenario(node);
                        //上報Alarm給MCS Todo by Kevin
                        foreach (var ohcv in node.getSubEqptList())
                        {
                            if (!ohcv.DoorClosed)
                            {
                                scApp.LineService.ProcessAlarmReport(
                                    ohcv.NODE_ID, ohcv.EQPT_ID, ohcv.Real_ID, "",
                                    SCAppConstants.SystemAlarmCode.OHCV_Issue.CVDoorAbnormallyOpen,
                                    ProtocolFormat.OHTMessage.ErrorStatus.ErrSet);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }
        public void safetyCheckCompleteChange(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                ANODE node = sender as ANODE;
                if (sender == null) return;
                if (!node.SafetyCheckComplete)
                {
                    if (node.DoorClosed && node.Is_Alive)
                    {
                        //enable 該CV所在路段 Todo by Kevin
                        scApp.RoadControlService.doEnableDisableSegment
                            (node.SegmentLocation, E_PORT_STATUS.InService, ASEGMENT.DisableType.Safety, Data.SECS.CSOT.SECSConst.LANECUTTYPE_LaneCutOnHMI);
                    }
                }
                else
                {
                    //do nothing
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }
    }
}
