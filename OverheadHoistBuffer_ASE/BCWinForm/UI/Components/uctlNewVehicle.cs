using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using com.mirle.ibg3k0.bc.winform.Properties;
using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using com.mirle.ibg3k0.bcf.Common;
using NLog;
using System.Diagnostics;
using com.mirle.ibg3k0.sc.Common;
using System.Collections.Concurrent;

namespace com.mirle.ibg3k0.bc.winform.UI.Components
{


    public partial class uctlNewVehicle : UserControl
    {
        #region Enum
        public enum E_VEHICLE_STATUS
        {
            UNCONNECTIED,
            POWERON,
            MANUAL,
            AUTOLOCAL,
            AUTOMTL,
            AUTOMTS,
            AUTOREMOTE
        }

        public enum E_ALERT_STATUS
        {
            NOTHING,
            WARNING,
            ALERT,
            ERROR,
            OBS,
            HID,
            PAUSE,
            BLOCK,
            PAUSE_SAFETY,
            PAUSE_EARTHQUAKE
        }
        public enum E_ACTION_STATUS
        {
            NOTHING,
            ONLYMOVE,
            PARKED,
            PARKING,
            TRAINING,
            MAINTENANCE
        }
        public enum E_SPEED_STATUS
        {
            SLOW,
            MEDIUM,
            FAST
        }
        public enum E_CST_LOAD_STATUS
        {
            NOTHING,
            GOLOAD,
            LOADED,
            LOADING,
            UNLOADING
        }
        #endregion Enum

        #region Const
        const int SPEED_INTERVAL_MM_SLOW = 20;
        const int SPEED_INTERVAL_MM_MEDIUM = 40;
        #endregion Const

        #region Parameter
        private static Logger logger = LogManager.GetCurrentClassLogger();


        private uctl_Map Uctl_Map;
        private AVEHICLE vh;
        private PictureBox PicAlarmStatus;
        private PictureBox PicCSTLoadStatus;


        private Image ImgVehicleStatus = Resources.Vehicle__Unconnected_;
        private E_VEHICLE_STATUS vehicleStatus;
        public E_VEHICLE_STATUS VehicleStatus
        {
            get { return vehicleStatus; }
            private set
            {
                if (vehicleStatus != value)
                {
                    vehicleStatus = value;
                    switch (value)
                    {
                        case E_VEHICLE_STATUS.UNCONNECTIED: ImgVehicleStatus = Resources.Vehicle__Unconnected_; break;
                        case E_VEHICLE_STATUS.POWERON: ImgVehicleStatus = Resources.Vehicle__Power_on_; break;
                        case E_VEHICLE_STATUS.MANUAL: ImgVehicleStatus = Resources.Vehicle__Manual_; break;
                        case E_VEHICLE_STATUS.AUTOLOCAL:
                        case E_VEHICLE_STATUS.AUTOMTL:
                        case E_VEHICLE_STATUS.AUTOMTS:
                            ImgVehicleStatus = Resources.Vehicle__Auto_Local_;
                            break;
                        case E_VEHICLE_STATUS.AUTOREMOTE: ImgVehicleStatus = Resources.Vehicle__Auto_Remote_; break;
                    }
                    pic_VhStatus.Image = ImgVehicleStatus;
                }
            }
        }

        private Image ImgAlertStatus;
        private E_ALERT_STATUS alertStatus;
        public E_ALERT_STATUS AlertStatus
        {
            get { return alertStatus; }
            private set
            {
                alertStatus = value;
                switch (value)
                {
                    case E_ALERT_STATUS.NOTHING: ImgAlertStatus = null; break;
                    case E_ALERT_STATUS.ERROR: ImgAlertStatus = Resources.Alarm__Error_; break;
                    case E_ALERT_STATUS.ALERT: ImgAlertStatus = Resources.Alarm__Alert_; break;
                    case E_ALERT_STATUS.WARNING: ImgAlertStatus = Resources.Alarm__Warning_; break;
                    case E_ALERT_STATUS.BLOCK: ImgAlertStatus = Resources.Pause__Block_; break;
                    case E_ALERT_STATUS.OBS: ImgAlertStatus = Resources.Pause__Obstructed_; break;
                    case E_ALERT_STATUS.HID: ImgAlertStatus = Resources.Pause__HID_; break;
                    case E_ALERT_STATUS.PAUSE: ImgAlertStatus = Resources.Pause__Pause_; break;
                    case E_ALERT_STATUS.PAUSE_SAFETY: ImgAlertStatus = Resources.Pause__Safety_; break;
                    case E_ALERT_STATUS.PAUSE_EARTHQUAKE: ImgAlertStatus = Resources.Pause__Earthquake_; break;
                }
            }
        }

        private Image ImgActionStatus;
        private E_ACTION_STATUS actionStatus;
        public E_ACTION_STATUS ActionStatus
        {
            get { return actionStatus; }
            private set
            {
                actionStatus = value;
                switch (value)
                {
                    case E_ACTION_STATUS.NOTHING: ImgActionStatus = null; break;
                    case E_ACTION_STATUS.ONLYMOVE: ImgActionStatus = Resources.Action__Moving_; break;
                    case E_ACTION_STATUS.PARKED: ImgActionStatus = Resources.Action__Parked_; break;
                    case E_ACTION_STATUS.PARKING: ImgActionStatus = Resources.Action__Parking_; break;
                    case E_ACTION_STATUS.TRAINING: ImgActionStatus = Resources.Action__Correcting_action_; break;
                    case E_ACTION_STATUS.MAINTENANCE: ImgActionStatus = Resources.Action__Maintenance_; break;
                }
            }
        }

        private Image ImgSpeedStatus = Resources.Speed__Slow_;
        private E_SPEED_STATUS speedStatus;
        public E_SPEED_STATUS SpeedStatus
        {
            get { return speedStatus; }
            private set
            {
                speedStatus = value;
                switch (value)
                {
                    case E_SPEED_STATUS.SLOW: ImgSpeedStatus = Resources.Speed__Slow_; break;
                    case E_SPEED_STATUS.MEDIUM: ImgSpeedStatus = Resources.Speed__Medium_; break;
                    case E_SPEED_STATUS.FAST: ImgSpeedStatus = Resources.Speed__Fast_; break;
                }
            }
        }


        private Image ImgCSTLoadStatus = null;
        private E_CST_LOAD_STATUS cstLoadStatus;
        public E_CST_LOAD_STATUS CSTLoadStatus
        {
            get { return cstLoadStatus; }
            private set
            {
                cstLoadStatus = value;
                switch (value)
                {
                    case E_CST_LOAD_STATUS.NOTHING: ImgCSTLoadStatus = null; break;
                    case E_CST_LOAD_STATUS.LOADED: ImgCSTLoadStatus = Resources.Action__Cassette_; break;
                    case E_CST_LOAD_STATUS.LOADING: ImgCSTLoadStatus = Resources.Action__Loading_; break;
                    case E_CST_LOAD_STATUS.UNLOADING: ImgCSTLoadStatus = Resources.Action__Unloading_; break;
                    case E_CST_LOAD_STATUS.GOLOAD: ImgCSTLoadStatus = Resources.Action__Receive_command_; break;
                }
                Adapter.Invoke((obj) =>
                {
                    if (ImgCSTLoadStatus != null)
                    {
                        PicCSTLoadStatus.Image = ImgCSTLoadStatus;
                        PicCSTLoadStatus.Visible = true;
                    }
                    else
                    {
                        PicCSTLoadStatus.Visible = false;
                    }
                }, null);
            }
        }

        private int currentSpeed;
        private int CurrentSpeed
        {
            get { return currentSpeed; }
            set
            {
                currentSpeed = value;
                sCurrentSpeed = currentSpeed.ToString("000");
                if (value < SPEED_INTERVAL_MM_SLOW)
                {
                    SpeedStatus = E_SPEED_STATUS.SLOW;
                }
                else if (SPEED_INTERVAL_MM_SLOW < value && value < SPEED_INTERVAL_MM_MEDIUM)
                {
                    SpeedStatus = E_SPEED_STATUS.MEDIUM;
                }
                else
                {
                    SpeedStatus = E_SPEED_STATUS.FAST;
                }
            }
        }
        public string sCurrentSpeed = "000";

        private int num = 0;
        public int Num
        {
            get { return (num); }
            set
            {
                num = value;
                sNum = num.ToString("00");
            }
        }
        public string sNum = "00";

        public string CurrentSecID { get; set; } = "";
        #endregion Parameter


        public uctlNewVehicle()
        {
            InitializeComponent();
            //  this.Size = Resources.Vehicle__Unconnected_.Size;
        }
        public uctlNewVehicle(AVEHICLE _vh, uctl_Map uctl_Map, PictureBox alarmStatus, PictureBox cstloadStatus)
        {
            InitializeComponent();
            //  this.Size = Resources.Vehicle__Unconnected_.Size;
            Uctl_Map = uctl_Map;
            vh = _vh;

            this.Width = this.Width / icon_scale;
            this.Height = this.Height / icon_scale;

            this.Left = this.Width / 2;
            this.Top = this.Height / 2;

            PicAlarmStatus = alarmStatus;
            PicCSTLoadStatus = cstloadStatus;
            PicAlarmStatus.Size =
    new Size(Resources.Alarm__Error_.Width / icon_scale, Resources.Alarm__Error_.Height / icon_scale);
            PicCSTLoadStatus.Size =
                new Size(Resources.Action__Cassette_.Size.Width / icon_scale, Resources.Action__Cassette_.Size.Height / icon_scale);

            font = new System.Drawing.Font("Consolas", 20F / icon_scale, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            font_Numbering = new System.Drawing.Font("Arial", 28F / icon_scale, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            PicAlarmStatus.VisibleChanged += PicAlarmStatus_VisibleChanged;
            PicCSTLoadStatus.VisibleChanged += PicCSTLoadStatus_VisibleChanged;
            this.BackColor = Color.FromArgb(29, 36, 60);
            registerEvent();
            _SetInitialVhToolTip();
            _SetRailToolTip();
        }

        private void PicCSTLoadStatus_VisibleChanged(object sender, EventArgs e)
        {
            refreshCSTIconPosition();
        }


        private void PicAlarmStatus_VisibleChanged(object sender, EventArgs e)
        {
            if ((sender as PictureBox).Visible)
            {
                //PicAlarmStatus.Left = this.Left + (this.Width / 2) - (PicAlarmStatus.Width / 2);
                //PicAlarmStatus.Top = this.Top - PicAlarmStatus.Height - 7;
            }
        }

        public void turnOnMonitorVh()
        {
            if (!SCUtility.isEmpty(vh.CUR_ADR_ID))
            {
                updateVehicleModeStatus();
                updateVehicleActionStatus();
                updateVehiclePosition();
            }
        }

        #region Proc
        string event_id = string.Empty;
        private void registerEvent()
        {
            event_id = this.Name + Num;
            vh.addEventHandler(event_id
                                , nameof(vh.isTcpIpConnect)
                                , (s1, e1) =>
                                {
                                    updateVehicleModeStatus();
                                    updateVehicleActionStatus();
                                    Adapter.Invoke((obj) =>
                                    {
                                        pic_VhStatus.Refresh();
                                    }, null);
                                });
            vh.addEventHandler(event_id
                                , nameof(vh.VhPositionChangeEvent)
                                , (s1, e1) =>
                                {
                                    updateVehiclePosition();
                                });
            vh.addEventHandler(event_id
                                , nameof(vh.VhStatusChangeEvent)
                                , (s1, e1) =>
                                {
                                    updateVehicleModeStatus();
                                    updateVehicleActionStatus();
                                    Adapter.Invoke((obj) =>
                                    {
                                        pic_VhStatus.Refresh();
                                    }, null);
                                });
            //vh.addEventHandler(event_id
            //                    , nameof(vh.procProgress_Percen)
            //                    , (s1, e1) => { updateVhCurrentProcProgress(); });
        }
        private void removeEvent()
        {
            vh.removeEventHandler(event_id);
        }
        #endregion Proc

        private void updateVehicleModeStatus()
        {
            TESTSpeedDisplay();
            if (!vh.isTcpIpConnect)
            {
                VehicleStatus = E_VEHICLE_STATUS.UNCONNECTIED;
            }
            else
            {
                switch (vh.MODE_STATUS)
                {
                    case VHModeStatus.InitialPowerOff:
                    case VHModeStatus.Manual:
                        VehicleStatus = E_VEHICLE_STATUS.MANUAL;
                        break;
                    case VHModeStatus.InitialPowerOn:
                        VehicleStatus = E_VEHICLE_STATUS.POWERON;
                        break;
                    case VHModeStatus.AutoLocal:
                        VehicleStatus = E_VEHICLE_STATUS.AUTOLOCAL;
                        break;
                    case VHModeStatus.AutoMts:
                        VehicleStatus = E_VEHICLE_STATUS.AUTOMTS;
                        break;
                    case VHModeStatus.AutoMtl:
                        VehicleStatus = E_VEHICLE_STATUS.AUTOMTL;
                        break;
                    case VHModeStatus.AutoRemote:
                        VehicleStatus = E_VEHICLE_STATUS.AUTOREMOTE;
                        break;
                    default:
                        VehicleStatus = E_VEHICLE_STATUS.MANUAL;
                        break;
                }
            }
        }

        private void updateVehicleActionStatus()
        {
            TESTSpeedDisplay();

            RefeshAlertStatus();


            if (!vh.isTcpIpConnect)
            {
                VehicleStatus = E_VEHICLE_STATUS.UNCONNECTIED;
            }
            else
            {
                switch (vh.ACT_STATUS)
                {
                    case VHActionStatus.NoCommand:
                        {
                            //CSTLoadStatus = E_CST_LOAD_STATUS.NOTHING;
                            if (vh.IS_PARKING)
                            {
                                ActionStatus = E_ACTION_STATUS.PARKED;
                            }
                            else
                            {
                                ActionStatus = E_ACTION_STATUS.NOTHING;
                            }
                            if (vh.HAS_CST == 1)
                            {
                                CSTLoadStatus = E_CST_LOAD_STATUS.LOADED;
                            }
                            else
                            {
                                CSTLoadStatus = E_CST_LOAD_STATUS.NOTHING;
                            }
                        }
                        break;
                    case VHActionStatus.Teaching:
                        {
                            CSTLoadStatus = E_CST_LOAD_STATUS.NOTHING;
                            ActionStatus = E_ACTION_STATUS.TRAINING;
                        }
                        break;
                    case VHActionStatus.Commanding:


                        bool isDisplayAlertIcon = true;
                        if (ImgAlertStatus != null)
                        {
                            PicAlarmStatus.Image = ImgAlertStatus;
                            isDisplayAlertIcon = true;
                        }
                        else
                        {
                            isDisplayAlertIcon = false;
                        }
                        Adapter.Invoke((obj) =>
                        {
                            PicAlarmStatus.Visible = isDisplayAlertIcon;
                        }, null);

                        //TODO 少地震的訊號

                        if (vh.MODE_STATUS == VHModeStatus.AutoLocal ||
                            vh.MODE_STATUS == VHModeStatus.AutoMtl ||
                            vh.MODE_STATUS == VHModeStatus.AutoMts)
                        {
                            ActionStatus = E_ACTION_STATUS.MAINTENANCE;
                        }
                        else if (!string.IsNullOrWhiteSpace(vh.PARK_ADR_ID))
                        {
                            ActionStatus = E_ACTION_STATUS.PARKING;
                        }
                        else if (vh.IS_PARKING)
                        {
                            ActionStatus = E_ACTION_STATUS.PARKED;
                        }
                        else
                        {
                            switch (vh.CmdType)
                            {
                                case E_CMD_TYPE.Move:
                                    ActionStatus = E_ACTION_STATUS.ONLYMOVE;
                                    break;
                                default:
                                    ActionStatus = E_ACTION_STATUS.NOTHING;
                                    break;

                            }

                            ActionStatus = E_ACTION_STATUS.NOTHING;
                        }

                        if (vh.VhRecentTranEvent == EventType.Vhloading)
                        {
                            CSTLoadStatus = E_CST_LOAD_STATUS.LOADING;
                        }
                        else if (vh.VhRecentTranEvent == EventType.Vhunloading)
                        {
                            CSTLoadStatus = E_CST_LOAD_STATUS.UNLOADING;
                        }
                        else if (vh.HAS_CST == 1)
                        {
                            CSTLoadStatus = E_CST_LOAD_STATUS.LOADED;
                        }
                        else
                        {
                            switch (vh.CmdType)
                            {
                                case E_CMD_TYPE.Load:
                                case E_CMD_TYPE.LoadUnload:
                                    CSTLoadStatus = E_CST_LOAD_STATUS.GOLOAD;
                                    break;
                                default:
                                    CSTLoadStatus = E_CST_LOAD_STATUS.NOTHING;
                                    break;

                            }
                        }


                        break;
                    case VHActionStatus.GripperTeaching:
                        //TODO 是不是GripperTeaching也要有自己的圖片?
                        break;
                    case VHActionStatus.CycleRun:
                        //TODO 是不是CycleRun也要有自己的圖片?
                        break;
                }
            }
        }

        private void RefeshAlertStatus()
        {
            if (vh.IsError)
            {
                AlertStatus = E_ALERT_STATUS.ERROR;
            }
            else if (vh.IsBlocking)
            {
                AlertStatus = E_ALERT_STATUS.BLOCK;
            }
            else if (vh.IsHIDPause)
            {
                AlertStatus = E_ALERT_STATUS.HID;
            }
            else if (vh.IsObstacle)
            {
                AlertStatus = E_ALERT_STATUS.OBS;
            }
            else if (vh.IsPause)
            {
                AlertStatus = E_ALERT_STATUS.PAUSE;
            }
            else
            {
                AlertStatus = E_ALERT_STATUS.NOTHING;
            }

            bool isDisplayAlertIcon = true;
            if (ImgAlertStatus != null)
            {
                PicAlarmStatus.Image = ImgAlertStatus;
                isDisplayAlertIcon = true;
            }
            else
            {
                isDisplayAlertIcon = false;
            }
            Adapter.Invoke((obj) =>
            {
                PicAlarmStatus.Visible = isDisplayAlertIcon;
            }, null);

        }

        private void updateVehiclePosition()
        {
            TESTSpeedDisplay();

            Adapter.Invoke((obj) =>
            {
                try
                {

                    GroupRails groupRails = Uctl_Map.getGroupRailBySecID(vh.CUR_SEC_ID);
                    if (groupRails != null)
                    {
                        //switch (vh.VhRecentTranEvent)
                        //{
                        //    case EventType.AdrPass:
                        //        if (vh.ACC_SEC_DIST == groupRails.Distance)
                        //        {
                        //            uctlAddress uctlAdr1 = Uctl_Map.getuctAddressByAdrID(vh.CUR_ADR_ID);
                        //            PrcSetLocation(uctlAdr1.p_LocX, uctlAdr1.p_LocY);
                        //        }
                        //        else if (CurrentSecID != groupRails.Section_ID)
                        //        {
                        //            if (!string.IsNullOrWhiteSpace(CurrentSecID))
                        //                Uctl_Map.p_DicSectionGroupRails[CurrentSecID.Trim()].VehicleLeave(this);
                        //        }
                        //        groupRails.VehicleEnterSection(this, vh.CUR_ADR_ID, vh.ACC_SEC_DIST);
                        //        break;
                        //    case EventType.LoadArrivals:
                        //    case EventType.UnloadArrivals:
                        //    case EventType.UnloadComplete:
                        //    case EventType.AdrOrMoveArrivals:
                        //    case EventType.VhmoveArrivals:
                        //        uctlAddress uctlAdr = Uctl_Map.getuctAddressByAdrID(vh.CUR_ADR_ID);
                        //        PrcSetLocation(uctlAdr.p_LocX, uctlAdr.p_LocY);
                        //        //uctlAddress uctlAdr = Uctl_Map.getuctAddressByAdrID(vh.CUR_ADR_ID);
                        //        //Uctl_Map.stopFlashingSpecifyRail(vh.PredictPath);
                        //        break;
                        //}
                        //if (vh.ACC_SEC_DIST == groupRails.Distance)
                        //{
                        //    uctlAddress uctlAdr1 = Uctl_Map.getuctAddressByAdrID(vh.CUR_ADR_ID);
                        //    PrcSetLocation(uctlAdr1.p_LocX, uctlAdr1.p_LocY);
                        //}
                        //else if (CurrentSecID != groupRails.Section_ID)
                        //{
                        //    if (!string.IsNullOrWhiteSpace(CurrentSecID))
                        //        Uctl_Map.p_DicSectionGroupRails[CurrentSecID.Trim()].VehicleLeave(this);
                        //}
                        groupRails.VehicleEnterSection(this, vh.CUR_ADR_ID, vh.ACC_SEC_DIST);

                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
            }, null);
        }

        private void TESTSpeedDisplay()
        {
            if (vh.sw_speed.ElapsedMilliseconds <= 3000) CurrentSpeed = 0;
            else if (vh.sw_speed.ElapsedMilliseconds > 3000 && vh.sw_speed.ElapsedMilliseconds < 5000) CurrentSpeed = 10;
            else if (vh.sw_speed.ElapsedMilliseconds >= 5000 && vh.sw_speed.ElapsedMilliseconds < 10000) CurrentSpeed = 30;
            else CurrentSpeed = 50;
        }

        public void PrcSetLocation(int iLocX, int iLocY)
        {
            this.Left = iLocX - (this.Width / 2);
            this.Top = iLocY - (this.Height + 5);
            refreshCSTIconPosition();
            //PicAlarmStatus.Left = this.Left + (this.Width / 2) - (PicAlarmStatus.Width / 2);
            //PicAlarmStatus.Top = this.Top - PicAlarmStatus.Height - 5;
            //PicCSTLoadunloadStatus.Left = this.Left + (this.Width / 2) - (PicCSTLoadunloadStatus.Width / 2);
            //PicCSTLoadunloadStatus.Top = this.Top + this.Height;
        }

        private void refreshCSTIconPosition()
        {
            //if (PicCSTLoadStatus.Visible)
            //{
            PicCSTLoadStatus.Left = this.Left + ((this.Width / 2) - (PicCSTLoadStatus.Width / 2));
            PicCSTLoadStatus.Top = this.Top - (PicCSTLoadStatus.Height / 3);

            PicAlarmStatus.Left = this.Left + (this.Width / 2) - (PicAlarmStatus.Width / 2);
            PicAlarmStatus.Top = this.Top - PicAlarmStatus.Height - 7;
            //}
        }

        #region DrawImage

        static SolidBrush white_objBrush = new SolidBrush(Color.White);
        static SolidBrush black_objBrush = new SolidBrush(Color.Black);
        static Font font = new System.Drawing.Font("Consolas", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        static Font font_Numbering = new System.Drawing.Font("Arial", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        static ConcurrentDictionary<string, RectangleF> dicRectagles = new ConcurrentDictionary<string, RectangleF>();
        string RECTANGLE_KEY_SPEEDSTATUS = "SPEED_STATUS";
        string RECTANGLE_KEY_ACTIONSTATS = "ACTION_STATUS";
        string RECTANGLE_KEY_CSTLOADSTATIUS = "CST_LOAD_STATIUS";
        string RECTANGLE_KEY_CURSPEED = "CUR_SPEED";
        string RECTANGLE_KEY_NUMBERING = "NUMBERING";
        int icon_scale = 3;
        private void pic_VhStatus_Paint(object sender, PaintEventArgs e)
        {

            Graphics g = e.Graphics;
            //pic_VhStatus.Image = ImgVehicleStatus;
            setVehicleNumbering(g);

            g.DrawImage(ImgSpeedStatus,
                GetRectangle(RECTANGLE_KEY_SPEEDSTATUS, new Rectangle(0, this.pic_VhStatus.Height - (ImgSpeedStatus.Size.Height / icon_scale),
                                                                    ImgSpeedStatus.Size.Height / icon_scale,
                                                                    ImgSpeedStatus.Size.Width / icon_scale)));

            setVhCurrentSpeed(g);

            if (ImgActionStatus != null)
                g.DrawImage(ImgActionStatus,
                    GetRectangle(RECTANGLE_KEY_ACTIONSTATS, new Rectangle((this.Width / 2) - ((Resources.Action__Parked_.Width / icon_scale) / 2), 0,
                                                                        Resources.Action__Parked_.Size.Height / icon_scale,
                                                                        Resources.Action__Parked_.Size.Width / icon_scale)));
            //else if (ImgCSTLoadStatus != null)
            //    g.DrawImage(ImgCSTLoadStatus,
            //        GetRectangle(RECTANGLE_KEY_CSTLOADSTATIUS, new Rectangle((this.Width / 2) - 17, 0,
            //                                                                ImgCSTLoadStatus.Size.Height / icon_scale,
            //                                                                ImgCSTLoadStatus.Size.Width / icon_scale)));
        }

        private void setVhCurrentSpeed(Graphics g)
        {
            var font_size = g.MeasureString(sCurrentSpeed, font, int.MaxValue);
            float speed_status_height = this.pic_VhStatus.Height - (ImgSpeedStatus.Size.Height / icon_scale);
            float crt_speed_height = speed_status_height + ((ImgSpeedStatus.Size.Height / icon_scale) / 2) - (font_size.Height / 2);
            g.DrawString(sCurrentSpeed, font, white_objBrush, GetRectangle(RECTANGLE_KEY_CURSPEED, new RectangleF(0, crt_speed_height,
                                                                                                                  font_size.Width, font_size.Height)));
        }
        private void setVehicleNumbering(Graphics g)
        {
            SolidBrush objBrush = null;
            if (VehicleStatus == E_VEHICLE_STATUS.POWERON)
                objBrush = black_objBrush;
            else
                objBrush = white_objBrush;
            var num_font_size = g.MeasureString(sNum, font_Numbering, int.MaxValue);
            g.DrawString(sNum, font_Numbering, objBrush, GetRectangle(RECTANGLE_KEY_NUMBERING, new RectangleF(pic_VhStatus.Width - (num_font_size.Width), pic_VhStatus.Height - num_font_size.Height - 3,
                                                                                                              num_font_size.Width, num_font_size.Height)));
        }

        private RectangleF GetRectangle(string key, RectangleF value)
        {
            return dicRectagles.GetOrAdd(key, value);
        }

        #endregion DrawImage

        private void _SetRailToolTip()
        {
            this.ToolTip.SetToolTip(this.pic_VhStatus,
                        "Current Adr : " + SCUtility.Trim(vh.CUR_ADR_ID) + "\r\n" +
                        "Current SEC : " + SCUtility.Trim(vh.CUR_SEC_ID) + "\r\n" +
                        "Current CST : " + (vh.HAS_CST == 1 ? SCUtility.Trim(vh.CST_ID, true) : "") + "\r\n" +
                        "Action : " + vh.ACT_STATUS.ToString());
        }
        private void _SetInitialVhToolTip()
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

        private void pic_VhStatus_Click(object sender, EventArgs e)
        {
            Uctl_Map.ohtc_Form.setMonitorVehicle(vh.VEHICLE_ID);
        }
    }
}
