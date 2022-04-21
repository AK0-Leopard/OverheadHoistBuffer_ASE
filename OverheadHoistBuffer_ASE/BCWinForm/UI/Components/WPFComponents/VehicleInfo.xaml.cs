using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VehicleControl_Viewer.UI.Components
{
    /// <summary>
    /// VehicleInfo.xaml 的互動邏輯
    /// </summary>
    public partial class VehicleInfo : UserControl
    {
        SolidColorBrush Brush_VhMode_Unconnected = new SolidColorBrush(Color.FromRgb(00, 00, 00));
        SolidColorBrush Brush_VhMode_PowerOn = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x88));
        SolidColorBrush Brush_VhMode_Manual = new SolidColorBrush(Color.FromRgb(0xE9, 0x55, 0x13));
        SolidColorBrush Brush_VhMode_AutoLocal = new SolidColorBrush(Color.FromRgb(0x03, 0x61, 0xB7));
        SolidColorBrush Brush_VhMode_AutoRemote = new SolidColorBrush(Color.FromRgb(0x00, 0x98, 0x44));

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


        private static Logger logger = LogManager.GetCurrentClassLogger();



        private E_VEHICLE_STATUS vehicleStatus = E_VEHICLE_STATUS.AUTOREMOTE;
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
                        case E_VEHICLE_STATUS.UNCONNECTIED: vehicleMode.Fill = Brush_VhMode_Unconnected; break;
                        case E_VEHICLE_STATUS.POWERON: vehicleMode.Fill = Brush_VhMode_PowerOn; break;
                        case E_VEHICLE_STATUS.MANUAL: vehicleMode.Fill = Brush_VhMode_Manual; break;
                        case E_VEHICLE_STATUS.AUTOLOCAL:
                        case E_VEHICLE_STATUS.AUTOMTL:
                        case E_VEHICLE_STATUS.AUTOMTS:
                            vehicleMode.Fill = Brush_VhMode_AutoLocal;
                            break;
                        case E_VEHICLE_STATUS.AUTOREMOTE: vehicleMode.Fill = Brush_VhMode_AutoRemote; break;
                    }
                }
            }
        }

        private E_ALERT_STATUS alertStatus;
        public E_ALERT_STATUS AlertStatus
        {
            get { return alertStatus; }
            private set
            {
                if (alertStatus != value)
                {
                    resetAlertStatus();
                    alertStatus = value;
                    switch (value)
                    {
                        case E_ALERT_STATUS.NOTHING: resetAlertStatus(); break;
                        case E_ALERT_STATUS.ERROR: Alarm.Visibility = Visibility.Visible; break;
                        case E_ALERT_STATUS.ALERT: Alarm.Visibility = Visibility.Visible; break;
                        case E_ALERT_STATUS.WARNING: Waring.Visibility = Visibility.Visible; break;
                        case E_ALERT_STATUS.BLOCK: pause_bolcked.Visibility = Visibility.Visible; break;
                        case E_ALERT_STATUS.OBS: paused_obstructed.Visibility = Visibility.Visible; break;
                        case E_ALERT_STATUS.HID: paused_hid.Visibility = Visibility.Visible; break;
                        case E_ALERT_STATUS.PAUSE: paused_normal.Visibility = Visibility.Visible; break;
                        case E_ALERT_STATUS.PAUSE_SAFETY: pauesd_safetyDoor.Visibility = Visibility.Visible; break;
                        case E_ALERT_STATUS.PAUSE_EARTHQUAKE: pauesd_earthQuake.Visibility = Visibility.Visible; break;
                    }
                }
            }
        }
        private void resetAlertStatus()
        {
            Alarm.Visibility = Visibility.Hidden;
            Waring.Visibility = Visibility.Hidden;
            pause_bolcked.Visibility = Visibility.Hidden;
            paused_obstructed.Visibility = Visibility.Hidden;
            paused_hid.Visibility = Visibility.Hidden;
            paused_normal.Visibility = Visibility.Hidden;
            pauesd_safetyDoor.Visibility = Visibility.Hidden;
            pauesd_earthQuake.Visibility = Visibility.Hidden;
        }

        private E_ACTION_STATUS actionStatus;
        public E_ACTION_STATUS ActionStatus
        {
            get { return actionStatus; }
            private set
            {
                if (actionStatus != value)
                {
                    actionStatus = value;
                    switch (value)
                    {
                        case E_ACTION_STATUS.NOTHING: resetActionStatus(); break;
                        case E_ACTION_STATUS.ONLYMOVE: move.Visibility = Visibility.Visible; break;
                        case E_ACTION_STATUS.PARKED: packed.Visibility = Visibility.Visible; break;
                        case E_ACTION_STATUS.PARKING: packing.Visibility = Visibility.Visible; break;
                        case E_ACTION_STATUS.TRAINING: resetActionStatus(); break;
                        case E_ACTION_STATUS.MAINTENANCE: resetActionStatus(); break;
                    }
                }
            }
        }
        private void resetActionStatus()
        {
            move.Visibility = Visibility.Hidden;
            packed.Visibility = Visibility.Hidden;
            packing.Visibility = Visibility.Hidden;
        }
        private E_CST_LOAD_STATUS cstLoadStatus;
        public E_CST_LOAD_STATUS CSTLoadStatus
        {
            get { return cstLoadStatus; }
            private set
            {
                if (cstLoadStatus != value)
                {
                    cstLoadStatus = value;
                    switch (value)
                    {
                        case E_CST_LOAD_STATUS.NOTHING: resetCSTLoadStatus(); break;
                        case E_CST_LOAD_STATUS.LOADED: carrering.Visibility = Visibility.Visible; break;
                        case E_CST_LOAD_STATUS.LOADING: loading.Visibility = Visibility.Visible; break;
                        case E_CST_LOAD_STATUS.UNLOADING: unloadingss.Visibility = Visibility.Visible; break;
                        case E_CST_LOAD_STATUS.GOLOAD: goCarrier.Visibility = Visibility.Visible; break;
                    }
                }
            }
        }
        private void resetCSTLoadStatus()
        {
            carrering.Visibility = Visibility.Hidden;
            loading.Visibility = Visibility.Hidden;
            unloadingss.Visibility = Visibility.Hidden;
            goCarrier.Visibility = Visibility.Hidden;
        }

        public DoubleAnimation doubleAnimation_x { get; private set; } = new DoubleAnimation();
        public DoubleAnimation doubleAnimation_y { get; private set; } = new DoubleAnimation();
        Storyboard moveStoryboard;
        public AVEHICLE vh { get; private set; } = null;
        public ContentControl vhPresenter { get; private set; } = new ContentControl();
        public VehicleInfo(AVEHICLE _vh)
        {
            InitializeComponent();

            vh = _vh;
            txt_vhNum.Text = vh.Num.ToString("00");
            statusInitial();

            vhPresenter = new ContentControl();
            vhPresenter.Name = _vh.VEHICLE_ID;
            vhPresenter.Content = this;
            vhPresenter.RenderTransform = new TranslateTransform();

            initialAnimation();

            //vh.PositionChanged += Vh_PositionChanged;

        }

        private void statusInitial()
        {
            pause_bolcked.Visibility = Visibility.Hidden;
            paused_obstructed.Visibility = Visibility.Hidden;
            paused_hid.Visibility = Visibility.Hidden;
            paused_normal.Visibility = Visibility.Hidden;
            pauesd_safetyDoor.Visibility = Visibility.Hidden;
            pauesd_earthQuake.Visibility = Visibility.Hidden;
            move.Visibility = Visibility.Hidden;
            packed.Visibility = Visibility.Hidden;
            packing.Visibility = Visibility.Hidden;
            goMaintain.Visibility = Visibility.Hidden;
            goCarrier.Visibility = Visibility.Hidden;
            loading.Visibility = Visibility.Hidden;
            unloadingss.Visibility = Visibility.Hidden;
            carrering.Visibility = Visibility.Hidden;
            Waring.Visibility = Visibility.Hidden;
            Alarm.Visibility = Visibility.Hidden;
        }


        private void initialAnimation()
        {
            moveStoryboard = new Storyboard();
            moveStoryboard.Children.Add(doubleAnimation_x);
            moveStoryboard.Children.Add(doubleAnimation_y);
            doubleAnimation_x.Duration = TimeSpan.FromSeconds(2);
            doubleAnimation_y.Duration = TimeSpan.FromSeconds(2);

            //doubleAnimation_x.To = vh.Num * 1500;
            //doubleAnimation_y.To = 5000;
            //vhPresenter.Loaded += delegate (object sender, RoutedEventArgs e)
            //{
            //    moveStoryboard.Begin(this);
            //};
        }

        public void resreshPosition()
        {


            double display_x = 0;
            double display_y = 0;
            if (vh.X_Axis == 0 && vh.Y_Axis == 0)
            {
                display_x = 500 - (this.ActualWidth / 2);
                display_y = -1000 - (this.ActualHeight) - 150;
            }
            else
            {
                display_x = vh.X_Axis - (this.ActualWidth / 2);
                display_y = vh.Y_Axis - (this.ActualHeight) - 150;
            }
            if (display_x != doubleAnimation_x.To || display_y != doubleAnimation_y.To)
            {
                doubleAnimation_x.To = display_x;
                doubleAnimation_y.To = display_y;
                moveStoryboard.Begin(this);
            }

            //doubleAnimation_x.Duration = TimeSpan.FromSeconds(2);
            //doubleAnimation_y.Duration = TimeSpan.FromSeconds(2);
        }

        public void resreshStatus()
        {
            updateVehicleModeStatus();
            updateVehicleActionStatus();
            RefeshAlertStatus();
        }
        private void updateVehicleModeStatus()
        {
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
            if (vh.ERROR == VhStopSingle.StopSingleOn)
            {
                AlertStatus = E_ALERT_STATUS.ERROR;
            }
            else if (vh.BLOCK_PAUSE == VhStopSingle.StopSingleOn &&
                     vh.Speed == 0)
            {
                AlertStatus = E_ALERT_STATUS.BLOCK;
            }
            else if (vh.HID_PAUSE == VhStopSingle.StopSingleOn)
            {
                AlertStatus = E_ALERT_STATUS.HID;
            }
            else if (vh.OBS_PAUSE == VhStopSingle.StopSingleOn)
            {
                AlertStatus = E_ALERT_STATUS.OBS;
            }
            else if (vh.CMD_PAUSE == VhStopSingle.StopSingleOn)
            {
                AlertStatus = E_ALERT_STATUS.PAUSE;
            }
            else
            {
                AlertStatus = E_ALERT_STATUS.NOTHING;
            }

        }

        public List<string> getWillPassSection()
        {
            return vh.WillPassSectionID;
        }

    }
}
