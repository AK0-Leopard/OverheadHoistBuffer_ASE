using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using com.mirle.ibg3k0.bc.winform.App;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using com.mirle.ibg3k0.sc.Common;
using System.Threading;
using com.mirle.ibg3k0.sc;
using NLog;

public enum E_MAP_VHSTS
{
    enNone,
    enManual,
    enAutoLocal,
    enAutoRemote,
    enTransfer,
    enMove2ClearPark,
    enMove2DirtyPark,
    enMoveOnAutoLocal,
    enBlocking,
    enPause,
    enObstacle,
    enHIDPause,
    enAlarm,
    enDisconnect,
    enTerminate
}

namespace com.mirle.ibg3k0.bc.winform.UI.Components
{
    public partial class uctlVehicle : UserControl
    {

        #region "Constant"

        #endregion	/* Constant */

        #region "Internal Variable"

        private int m_iVhPt;
        private int m_iNum;
        private E_MAP_VHSTS m_iStatus = E_MAP_VHSTS.enDisconnect;
        private bool m_bPresence;
        private int m_iLocX;
        private int m_iLocY;
        private int m_iMemStatus = 0;
        private bool m_bMemPresence = false;
        private int m_iMemLocX = 0;
        private int m_iMemLocY = 0;
        private int m_iSizeW;
        private int m_iSizeH;
        private Font m_objFont;
        //private Equipment eqpt;
        private AVEHICLE eqpt;
        private uctl_Map Uctl_Map;
        private string m_sCurrentSecID = "";
        BCApplication bcApp = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion	/* Internal Variable */

        #region "Event"

        /// <summary>
        /// Vehicle Select Event Handler
        /// </summary>
        /// <param name="iVhPt"></param>
        public delegate void VehicleSelectedEventHandler(int iVhPt);
        public event VehicleSelectedEventHandler evtVehicleSelected;

        #endregion	/* Event */

        #region "Property"

        /// <summary>
        /// Vehicle Number
        /// </summary>
        public int p_VhPt
        {
            get { return (m_iVhPt); }
            set
            {
                m_iVhPt = value;
            }
        }

        /// <summary>
        /// Vehicle Number
        /// </summary>
        public int p_Num
        {
            get { return (m_iNum); }
            set
            {
                m_iNum = value;
                lblPresence.Text = ((char)m_iNum).ToString();
            }
        }

        /// <summary>
        /// Vehicle Status
        /// </summary>
        public E_MAP_VHSTS p_Status
        {
            get { return (m_iStatus); }
            set
            {
                if (m_iStatus != value)
                {
                    m_iStatus = value;
                    switch (m_iStatus)
                    {
                        case E_MAP_VHSTS.enManual:
                            tlpState.BackColor = BCAppConstants.CLR_MAP_VHSTS_MANUAL_BACK;
                            break;
                        case E_MAP_VHSTS.enAutoRemote:
                            tlpState.BackColor = BCAppConstants.CLR_MAP_VHSTS_AUTOREMOTE_BACK;
                            break;
                        case E_MAP_VHSTS.enAutoLocal:
                            tlpState.BackColor = BCAppConstants.CLR_MAP_VHSTS_AUTOLOCAL_BACK;
                            break;
                        case E_MAP_VHSTS.enTransfer:
                            tlpState.BackColor = BCAppConstants.CLR_MAP_VHSTS_TRANSFER_BACK;
                            break;
                        case E_MAP_VHSTS.enMove2ClearPark:
                            tlpState.BackColor = BCAppConstants.CLR_MAP_VHSTS_MOVE2CLEARPARK_BACK;
                            break;
                        case E_MAP_VHSTS.enMove2DirtyPark:
                            tlpState.BackColor = BCAppConstants.CLR_MAP_VHSTS_MOVE2DIRTY_BACK;
                            break;
                        case E_MAP_VHSTS.enMoveOnAutoLocal:
                            tlpState.BackColor = BCAppConstants.CLR_MAP_VHSTS_MOVEONAUTOLOCAL_BACK;
                            break;
                        case E_MAP_VHSTS.enBlocking:
                            tlpState.BackColor = BCAppConstants.CLR_MAP_VHSTS_BLOCK_BACK;
                            break;
                        case E_MAP_VHSTS.enPause:
                            tlpState.BackColor = BCAppConstants.CLR_MAP_VHSTS_PAUSE_BACK;
                            break;
                        case E_MAP_VHSTS.enObstacle:
                            tlpState.BackColor = BCAppConstants.CLR_MAP_VHSTS_OBSTACLE_BACK;
                            break;
                        case E_MAP_VHSTS.enHIDPause:
                            tlpState.BackColor = BCAppConstants.CLR_MAP_VHSTS_HID_PAUSE_BACK;
                            break;
                        case E_MAP_VHSTS.enAlarm:
                            tlpState.BackColor = BCAppConstants.CLR_MAP_VHSTS_ALARM_BACK;
                            break;
                        case E_MAP_VHSTS.enDisconnect:
                            tlpState.BackColor = BCAppConstants.CLR_MAP_VHSTS_DISCONNECT_BACK;
                            break;
                        default:
                            tlpState.BackColor = BCAppConstants.CLR_MAP_VHSTS_NONE_BACK;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Vehicle Presence [false:OFF, True:ON]
        /// </summary>
        public bool p_Presence
        {
            get { return (m_bPresence); }
            set
            {
                if (m_bPresence != value)
                {
                    m_bPresence = value;
                    if (m_bPresence)
                    {
                        lblPresence.BackColor = BCAppConstants.CLR_MAP_VHCST_ON;
                    }
                    else
                    {
                        lblPresence.BackColor = BCAppConstants.CLR_MAP_VHCST_OFF;
                    }
                }
            }
        }

        public int p_LocX
        {
            get { return (m_iLocX); }
            set
            {
                m_iLocX = value;
            }
        }

        public int p_LocY
        {
            get { return (m_iLocY); }
            set
            {
                m_iLocY = value;
            }
        }

        public int p_SizeW
        {
            get { return (m_iSizeW); }
            set
            {
                m_iSizeW = value;
                //this.Width = value;
            }
        }

        public int p_SizeH
        {
            get { return (m_iSizeH); }
            set
            {
                m_iSizeH = value;
                //this.Height = value;
            }
        }

        public Font p_Font
        {
            get { return (m_objFont); }
            set
            {
                m_objFont = value;
                this.Font = value;
            }
        }

        public string p_CurrentSecID
        {
            get { return (m_sCurrentSecID); }
            set
            {
                m_sCurrentSecID = value;
            }
        }

        #endregion	/* Property */

        #region "Constructor／Destructor"

        //public uctlVehicle(Equipment eqpt, uctl_Map _uctl_Map)
        public uctlVehicle(AVEHICLE eqpt, uctl_Map _uctl_Map)
        {
            InitializeComponent();
            bcApp = BCApplication.getInstance();
            this.eqpt = eqpt;
            this.Uctl_Map = _uctl_Map;
            m_iVhPt = -1;
            m_iNum = 0;
            this.p_Status = E_MAP_VHSTS.enDisconnect;
            this.p_Presence = false;
            //registerEvent();
            _SetInitialRailToolTip();
            _SetRailToolTip();
        }

        #endregion	/* Constructor／Destructor */

        #region "Publish Process"

        public void PrcSetLocation(int iLocX, int iLocY)
        {
            m_iLocX = iLocX;
            m_iLocY = iLocY;

            if ((m_iLocX != m_iMemLocX) || (m_iLocY != m_iMemLocY))
            {
                m_iMemLocX = m_iLocX;
                m_iMemLocY = m_iLocY;
                _SetLoaction();
            }
            this.Tag = this.Top + "|" + this.Left + "|" + this.p_SizeH + "|" + this.p_SizeW;
        }
        public void turnOnMonitorVh()
        {
            if (!SCUtility.isEmpty(eqpt.CUR_ADR_ID))
            {
                changeVehicleStatus();
                moveVehiclePosition();
                updateVhCurrentProcProgress();
            }
            registerEvent();
            this.Visible = true;
        }
        public void turnOffMonitorVh()
        {
            removeEvent();
            this.Visible = false;
            //SpinWait.SpinUntil(() => false, 1000);
            //Adapter.Invoke((obj) =>
            //    {

            //    }, null);
        }

        #endregion	/* Publish Process */

        #region "Internal Process"
        string event_id = string.Empty;
        private void registerEvent()
        {
            event_id = this.Name + m_iNum;
            eqpt.addEventHandler(event_id
                                , BCFUtility.getPropertyName(() => eqpt.isTcpIpConnect)
                                , (s1, e1) => { changeVehicleStatus(); });
            eqpt.addEventHandler(event_id
                                , BCFUtility.getPropertyName(() => eqpt.VhPositionChangeEvent)
                                , (s1, e1) => { moveVehiclePosition(); });
            //eqpt.addEventHandler(event_id
            //                    , BCFUtility.getPropertyName(() => eqpt.PredictPath)
            //                    , (s1, e1) => { setPredictPath(); });
            eqpt.addEventHandler(event_id
                                , BCFUtility.getPropertyName(() => eqpt.VhStatusChangeEvent)
                                , (s1, e1) => { changeVehicleStatus(); });
            eqpt.addEventHandler(event_id
                                , BCFUtility.getPropertyName(() => eqpt.procProgress_Percen)
                                , (s1, e1) => { updateVhCurrentProcProgress(); });
        }
        private void removeEvent()
        {
            eqpt.removeEventHandler(event_id);
        }
        private void moveVehiclePosition()
        {
            Adapter.Invoke((obj) =>
            {
                try
                {

                    GroupRails groupRails = Uctl_Map.getGroupRailBySecID(eqpt.CUR_SEC_ID);
                    if (groupRails != null)
                    {
                        switch (eqpt.VhRecentTranEvent)
                        {
                            case EventType.AdrPass:
                                if (p_CurrentSecID != groupRails.Section_ID)
                                {
                                    if (!string.IsNullOrWhiteSpace(p_CurrentSecID))
                                        Uctl_Map.p_DicSectionGroupRails[p_CurrentSecID.Trim()].VehicleLeave(this);
                                }
                                groupRails.VehicleEnterSection(this, eqpt.CUR_ADR_ID, eqpt.ACC_SEC_DIST);
                                break;
                            case EventType.LoadArrivals:
                            //groupRails.VehicleArrivalsStartAdr(this);
                            //break;
                            case EventType.UnloadArrivals:
                            case EventType.UnloadComplete:
                            case EventType.AdrOrMoveArrivals:
                                uctlAddress uctlAdr = Uctl_Map.getuctAddressByAdrID(eqpt.CUR_ADR_ID);
                                //point = bcApp.SCApplication.MapBLL.getPointByID(eqpt.currentAddress);
                                PrcSetLocation(uctlAdr.p_LocX, uctlAdr.p_LocY);
                                //groupRails.VehicleArrivalsEndAdr(this);
                                Uctl_Map.stopFlashingSpecifyRail(eqpt.PredictPath);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
            }, null);
        }

        public void refreshVehicleStatus()
        {
            changeVehicleStatus();
        }
        bool isExecuteLoadUnload = false;
        bool isVehicleIdle = false;
        private void changeVehicleStatus()
        {
            isExecuteLoadUnload = false;
            isVehicleIdle = false;

            p_Presence = eqpt.HAS_CST == 1;

            if (!eqpt.isTcpIpConnect)
            {
                p_Status = E_MAP_VHSTS.enDisconnect;
            }
            else
            {
                switch (eqpt.ACT_STATUS)
                {
                    case VHActionStatus.NoCommand:
                        //if (eqpt.BlockingStatus == VhStopSingle.StopSingleOn)
                        //{
                        //    p_Status = E_MAP_VHSTS.enBlocking;
                        //}
                        //else if (eqpt.IsPause)
                        //{
                        //    p_Status = E_MAP_VHSTS.enPause;
                        //}
                        //else if (eqpt.IsObstacle)
                        //{
                        //    p_Status = E_MAP_VHSTS.enObstacle;
                        //}
                        //else
                        {
                            switch (eqpt.MODE_STATUS)
                            {
                                case VHModeStatus.AutoLocal:
                                case VHModeStatus.AutoRemote:
                                    isVehicleIdle = true;
                                    if (eqpt.MODE_STATUS == VHModeStatus.AutoLocal &&
                                        SCUtility.isEmpty(eqpt.MCS_CMD))
                                    {
                                        p_Status = E_MAP_VHSTS.enAutoLocal;
                                    }
                                    else
                                    {
                                        p_Status = E_MAP_VHSTS.enAutoRemote;
                                    }
                                    //Task.Run(() => flashingVehicleStaus());
                                    break;
                                case VHModeStatus.InitialPowerOff:
                                case VHModeStatus.InitialPowerOn:
                                case VHModeStatus.Manual:
                                    p_Status = E_MAP_VHSTS.enManual;
                                    break;
                                default:
                                    p_Status = E_MAP_VHSTS.enNone;
                                    break;
                            }
                        }
                        break;
                    case VHActionStatus.Teaching:
                    case VHActionStatus.Commanding:
                        if (eqpt.IsPause)
                        {
                            p_Status = E_MAP_VHSTS.enPause;
                        }
                        else if (eqpt.IsObstacle)
                        {
                            p_Status = E_MAP_VHSTS.enObstacle;
                        }
                        else if (eqpt.BlockingStatus == VhStopSingle.StopSingleOn)
                        {
                            p_Status = E_MAP_VHSTS.enBlocking;
                        }
                        else if (eqpt.IsHIDPause)
                        {
                            p_Status = E_MAP_VHSTS.enHIDPause;
                        }
                        else if (SCUtility.isEmpty(eqpt.MCS_CMD))
                        {
                            if (eqpt.MODE_STATUS == VHModeStatus.AutoLocal)
                            {
                                p_Status = E_MAP_VHSTS.enMoveOnAutoLocal;
                            }
                            else
                            {
                                switch (eqpt.VEHICLE_TYPE)
                                {
                                    case E_VH_TYPE.Clean:
                                        p_Status = E_MAP_VHSTS.enMove2ClearPark;
                                        break;
                                    case E_VH_TYPE.Dirty:
                                        p_Status = E_MAP_VHSTS.enMove2DirtyPark;
                                        break;
                                    default:
                                        p_Status = E_MAP_VHSTS.enMove2ClearPark;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            p_Status = E_MAP_VHSTS.enTransfer;
                        }

                        if (eqpt.VhRecentTranEvent == EventType.Vhloading ||
                            eqpt.VhRecentTranEvent == EventType.Vhunloading)
                        {
                            isExecuteLoadUnload = true;
                            Task.Run(() => flashingPresenceStaus());
                        }
                        break;
                    //case VHActionStatus.Loading:
                    //case VHActionStatus.Unloading:
                    case VHActionStatus.GripperTeaching:
                        p_Status = E_MAP_VHSTS.enAutoRemote;
                        isExecuteLoadUnload = true;
                        Task.Run(() => flashingPresenceStaus());
                        break;
                    case VHActionStatus.CycleRun:
                        isVehicleIdle = true;
                        p_Status = E_MAP_VHSTS.enAutoRemote;
                        Task.Run(() => flashingVehicleStaus());
                        break;
                }
            }
        }

        private long syncflashingPresencePoint = 0;
        private void flashingPresenceStaus()
        {
            if (System.Threading.Interlocked.Exchange
                (ref syncflashingPresencePoint, 1) == 0)
            {
                try
                {
                    while (isExecuteLoadUnload)
                    {
                        p_Presence = !p_Presence;
                        SpinWait.SpinUntil(() => !isExecuteLoadUnload, 1000);
                    }
                    p_Presence = eqpt.HAS_CST == 1;
                }
                catch (Exception ex)
                {
                    //todo recode log
                }
                finally
                {
                    System.Threading.Interlocked.Exchange
                        (ref syncflashingPresencePoint, 0);
                }
            }
        }

        private long syncflashingVehicleStatusPoint = 0;
        private E_MAP_VHSTS crtVhStatus;
        private void flashingVehicleStaus()
        {
            crtVhStatus = p_Status;
            if (System.Threading.Interlocked.Exchange
                (ref syncflashingVehicleStatusPoint, 1) == 0)
            {
                try
                {
                    while (isVehicleIdle)
                    {
                        p_Status = p_Status == crtVhStatus ?
                            E_MAP_VHSTS.enNone : crtVhStatus;
                        SpinWait.SpinUntil(() => !isVehicleIdle, 1000);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                    //todo recode log
                }
                finally
                {
                    System.Threading.Interlocked.Exchange
                        (ref syncflashingVehicleStatusPoint, 0);
                }
            }
        }

        //private void setPredictPath()
        //{
        //    Uctl_Map.startFlashingSpecifyRail(eqpt.PredictPath);
        //}

        private void updateVhCurrentProcProgress()
        {
            Adapter.Invoke((obj) =>
            {
                if (eqpt.procProgress_Percen >= procBar_Progress.Minimum &&
                    eqpt.procProgress_Percen <= procBar_Progress.Maximum)
                {
                    procBar_Progress.Value = eqpt.procProgress_Percen;
                }
            }, null);
        }



        private void uctlVehicle_Resize(object sender, EventArgs e)
        {
            //this.Height = this.Width;

            //_SetVehicleSize();
            _SetLoaction();
        }

        private void uctlVehicle_FontChanged(object sender, EventArgs e)
        {
            //this.lblPresence.Font = this.Font;
        }

        private void _SetVehicleSize()
        {
            int iSize = this.Width - 12;

            if (iSize > 0)
            {
                this.Visible = false;
                this.lblPresence.Width = iSize;
                this.lblPresence.Height = iSize;
                this.Visible = true;
            }
        }

        private void _SetLoaction()
        {
            //this.Visible = false;
            this.Left = this.m_iLocX - (this.Width / 2);
            this.Top = this.m_iLocY - (this.Height / 2);
            eqpt.Pixel_Loaction_X = Left;
            eqpt.Pixel_Loaction_Y = Top;
            //this.Visible = true;
        }

        private void lblName_Click(object sender, EventArgs e)
        {
            if (evtVehicleSelected != null)
            {
                evtVehicleSelected(m_iVhPt);
            }
        }

        #endregion	/* Internal Process */

        private void _SetRailToolTip()
        {
            //AVEHICLE vh = bcApp.SCApplication.VehicleBLL.getVehicleByID(eqpt.VEHICLE_ID);
            AVEHICLE vh = bcApp.SCApplication.getEQObjCacheManager().getVehicletByVHID(eqpt.VEHICLE_ID);
            this.ToolTip.SetToolTip(this.lblPresence,
                        "Current Adr : " + SCUtility.Trim(vh.CUR_ADR_ID) + "\r\n" +
                        "Current SEC : " + SCUtility.Trim(vh.CUR_SEC_ID) + "\r\n" +
                        "Action : " + vh.ACT_STATUS.ToString());
        }
        private void _SetInitialRailToolTip()
        {
            this.ToolTip.AutoPopDelay = 30000;
            this.ToolTip.ForeColor = Color.Black;
            this.ToolTip.BackColor = Color.White;
            this.ToolTip.ShowAlways = true;
            this.ToolTip.UseAnimation = false;
            this.ToolTip.UseFading = false;

            this.ToolTip.InitialDelay = 100;
            this.ToolTip.ReshowDelay = 100;
        }
        private long syncPoint = 0;
        private void ToolTip_Popup(object sender, PopupEventArgs e)
        {
            if (System.Threading.Interlocked.Exchange(ref syncPoint, 1) == 0)
            {
                try
                {
                    _SetRailToolTip();
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref syncPoint, 0);
                }
            }
        }

        private void lblPresence_Click(object sender, EventArgs e)
        {
            Uctl_Map.ohtc_Form.setMonitorVehicle(eqpt.VEHICLE_ID);
        }

        private void tlpState_Click(object sender, EventArgs e)
        {
            Uctl_Map.ohtc_Form.setMonitorVehicle(eqpt.VEHICLE_ID);
        }

        private void procBar_Progress_Click(object sender, EventArgs e)
        {
            Uctl_Map.ohtc_Form.setMonitorVehicle(eqpt.VEHICLE_ID);
        }
    }
}
