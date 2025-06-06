﻿using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.TimerAction;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Common.AOP;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction;
using com.mirle.ibg3k0.sc.Data.VO.Interface;
using com.mirle.ibg3k0.sc.Data.VO.PartialVo;
using com.mirle.ibg3k0.sc.ObjectRelay;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using Newtonsoft.Json;
using NLog;
using Stateless;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static com.mirle.ibg3k0.sc.App.SCAppConstants;

namespace com.mirle.ibg3k0.sc
{
    public class LocationChangeEventArgs : EventArgs
    {
        public string EntrySection;
        public string LeaveSection;
        public LocationChangeEventArgs(string entrySection, string leaveSection)
        {
            EntrySection = entrySection;
            LeaveSection = leaveSection;
        }
    }
    public class SegmentChangeEventArgs : EventArgs
    {
        public string EntrySegment;
        public string LeaveSegment;
        public SegmentChangeEventArgs(string entrySegment, string leaveSegment)
        {
            EntrySegment = entrySegment;
            LeaveSegment = leaveSegment;
        }
    }
    //[TeaceMethodAspectAttribute]
    public partial class AVEHICLE : BaseEQObject, IConnectionStatusChange
    {
        public VehicleStateMachine vhStateMachine;
        public VehicleErrorStateMachine vhErrorStateMachine;
        VehicleStatusInfo vehicleStatusInfo;

        public const string DEVICE_NAME_OHx = "OHx";

        /// <summary>
        /// 最大發送命令失敗的次數，當大於該次數時，會將該VH切換成AutoLocal模式。
        /// </summary>
        public const int MAX_ASSIGN_COMMAND_FAIL_TIMES = 3;
        /// <summary>
        /// 最大發送命令失敗的次數，當大於該次數時，會將該VH切換成AutoLocal模式。
        /// </summary>
        public const int MAX_STATUS_REQUEST_FAIL_TIMES = 3;
        /// <summary>
        /// 最大允許沒有通訊的時間
        /// </summary>
        public static UInt16 MAX_ALLOW_NO_COMMUNICATION_TIME_SECOND { get; private set; } = 60;
        /// <summary>
        /// 單筆命令，最大允許的搬送時間
        /// </summary>
        public static UInt16 MAX_ALLOW_ACTION_TIME_SECOND { get; private set; } = 300;
        public static UInt16 MAX_ALLOW_IMPORTANT_EVENT_RETRY_COUNT { get; private set; } = 3;
        public static UInt16 MAX_ALLOW_VH_IDLE_TIME_MMILLI_SECOND { get; private set; } = 5_000;
        public static UInt16 LONG_TIME_DISCONNECTION_JUDGE_TIME_SECOND { get; private set; } = 300;
        public static UInt16 MAX_ALLOW_OBSTACLING_TIME_SECOND { get; private set; } = 180;

        public event EventHandler<LocationChangeEventArgs> LocationChange;
        public event EventHandler<SegmentChangeEventArgs> SegmentChange;
        public event EventHandler<CompleteStatus> CommandComplete;
        public event EventHandler<int> AssignCommandFailOverTimes;
        public event EventHandler<int> StatusRequestFailOverTimes;
        public event EventHandler LongTimeNoCommuncation;
        public event EventHandler<string> LongTimeInaction;
        public event EventHandler<VhStopSingle> ErrorStatusChange;
        public event EventHandler<VhStopSingle> ReserveStatusChange;
        public event EventHandler<int> HasBoxStatusChange;
        public event EventHandler<EventType> HasImportantEventReportRetryOverTimes;
        public event EventHandler IdleTimeIsEnough;
        public event EventHandler<string> BoxIdleOnVh;
        public event EventHandler CycleMovePausing;
        public event EventHandler<ACMD_OHTC> ExcuteCommandStatusNotMatch;
        public event EventHandler HasReserveRequestRetryOverTimes;
        public event EventHandler<bool> LongTimeReserveRequestFailHappend;
        public event EventHandler<bool> LongTimeDisconnectionHappend;
        public event EventHandler LongTimeObstacling;
        public event EventHandler LongTimeObstacleFinish;


        VehicleTimerAction vehicleTimer = null;
        private Stopwatch CurrentCommandExcuteTime;
        private Stopwatch IdleTime;
        private Stopwatch CurrentObstaclingTime;

        public void onCommandComplete(CompleteStatus cmpStatus)
        {
            CommandComplete?.Invoke(this, cmpStatus);
        }
        public void onLocationChange(string entrySection, string leaveSection)
        {
            LocationChange?.Invoke(this, new LocationChangeEventArgs(entrySection, leaveSection));
        }
        public void onSegmentChange(string entrySegemnt, string leaveSegment)
        {
            SegmentChange?.Invoke(this, new SegmentChangeEventArgs(entrySegemnt, leaveSegment));
        }
        public void onLongTimeNoCommuncation()
        {
            LongTimeNoCommuncation?.Invoke(this, EventArgs.Empty);
        }
        public void onLongTimeInaction(string cmdID)
        {
            LongTimeInaction?.Invoke(this, cmdID);
        }
        public void onErrorStatusChange(VhStopSingle vhStopSingle)
        {
            ErrorStatusChange?.Invoke(this, vhStopSingle);
        }
        public void onReserveStatusChange(VhStopSingle vhStopSingle)
        {
            ReserveStatusChange?.Invoke(this, vhStopSingle);
        }
        public void onHasBoxStatusChange(int hasBoxStaus)
        {
            HasBoxStatusChange?.Invoke(this, hasBoxStaus);
        }
        public void onIdleTimeIsEnough()
        {
            IdleTimeIsEnough?.Invoke(this, EventArgs.Empty);
        }
        public void onBoxIdleOnVh(string boxID)
        {
            BoxIdleOnVh?.Invoke(this, boxID);
        }
        public void onCycleMovePausing()
        {
            CycleMovePausing?.Invoke(this, EventArgs.Empty);
        }
        public void onLongTimeReserveRequestFailHappend(bool isHappend)
        {
            LongTimeReserveRequestFailHappend?.Invoke(this, isHappend);
        }

        public void onExcuteCommandStatusNotMatch(ACMD_OHTC cmd)
        {
            ExcuteCommandStatusNotMatch?.Invoke(this, cmd);
        }
        public void onLongTimeDisconnection(bool isHappend)
        {
            LongTimeDisconnectionHappend?.Invoke(this, isHappend);
        }
        public void onLongTimeObstacling()
        {
            LongTimeObstacling?.Invoke(this, EventArgs.Empty);
        }
        public void onLongTimeObstacleFinish()
        {
            LongTimeObstacleFinish?.Invoke(this, EventArgs.Empty);
        }


        public AVEHICLE()
        {
            eqptObjectCate = SCAppConstants.EQPT_OBJECT_CATE_EQPT;
            PositionRefreshTimer.Restart();
            vhStateMachine = new VehicleStateMachine(() => State, (state) => State = state);
            vhStateMachine.OnTransitioned(TransitionedHandler);
            vhStateMachine.OnUnhandledTrigger(UnhandledTriggerHandler);
            initialVhErrorStateMachine();
            CurrentCommandExcuteTime = new Stopwatch();
            IdleTime = new Stopwatch();
            CurrentObstaclingTime = new Stopwatch();
            vehicleStatusInfo = new VehicleStatusInfo(this);
            //VhRecentRequestSection = new Google.Protobuf.Collections.RepeatedField<ReserveInfo>();
        }
        private void initialVhErrorStateMachine()
        {
            vhErrorStateMachine = new VehicleErrorStateMachine(() => errorState, (errorstate) => errorState = errorstate);
            vhErrorStateMachine.OnTransitioned(TransitionedHandler);
            vhErrorStateMachine.OnUnhandledTrigger(UnhandledTriggerHandler);
        }

        public void TimerActionStart()
        {
            vehicleTimer = new VehicleTimerAction(this, "VehicleTimerAction", 1000);
            vehicleTimer.start();
        }

        public override string ToString()
        {
            string json = JsonConvert.SerializeObject(this);
            return json;
        }
        /// <summary>
        /// 測試使用
        /// </summary>
        [JsonIgnore]
        public virtual Stopwatch sw_speed { get; set; } = new Stopwatch();
        [JsonIgnore]
        public virtual string[] PredictPath { get; set; }
        public virtual List<string> PredictSectionsStartToLoad { get; set; }
        public virtual List<string> PredictSectionsToDesination { get; set; }

        public int PrePositionSeqNum = 0;
        [JsonIgnore]
        public virtual string[] CyclingPath { get; set; }
        [JsonIgnore]
        public virtual string startAdr { get; set; } = string.Empty;
        [JsonIgnore]
        public virtual string FromAdr { get; set; } = string.Empty;
        [JsonIgnore]
        public virtual string ToAdr { get; set; } = string.Empty;
        [JsonIgnore]
        public virtual string CMD_CST_ID { get; set; } = string.Empty;
        [JsonIgnore]
        public virtual bool IsPrepareAvoid { get; set; }
        [JsonIgnore]
        public virtual int CMD_Priority { get; set; } = 0;
        public virtual string CUR_SEG_ID { get; set; } = string.Empty;
        private int assigncommandfailtimes = 0;
        public virtual bool isLongTimeInaction { get; private set; }
        public virtual string isLongTimeInactionCMDID { get; private set; }
        public virtual int AssignCommandFailTimes
        {
            get { return assigncommandfailtimes; }
            set
            {
                assigncommandfailtimes = value;
                if (assigncommandfailtimes >= MAX_ASSIGN_COMMAND_FAIL_TIMES)
                {
                    AssignCommandFailOverTimes?.Invoke(this, AssignCommandFailTimes);
                }
            }
        }
        private int statusRequestFailTimes = 0;
        public virtual int StatusRequestFailTimes
        {
            get { return statusRequestFailTimes; }
            set
            {
                statusRequestFailTimes = value;
                if (statusRequestFailTimes >= MAX_STATUS_REQUEST_FAIL_TIMES)
                {
                    StatusRequestFailOverTimes?.Invoke(this, statusRequestFailTimes);
                }
            }
        }
        [JsonIgnore]
        public virtual double X_Axis { get; set; }
        [JsonIgnore]
        public virtual double Y_Axis { get; set; }
        [JsonIgnore]
        public virtual string CurrentExcuteCmdID { get; set; }

        [JsonIgnore]
        public virtual List<string> WillPassSectionID { get; set; }


        [JsonIgnore]
        public virtual ReserveUnsuccessInfo CanNotReserveInfo { get; set; }
        public class ReserveUnsuccessInfo
        {
            public ReserveUnsuccessInfo(string vhID, string adrID, string secID)
            {
                ReservedVhID = vhID;
                ReservedAdrID = SCUtility.Trim(adrID);
                ReservedSectionID = SCUtility.Trim(secID);
            }
            public string ReservedVhID { get; }
            public string ReservedAdrID { get; }
            public string ReservedSectionID { get; }
        }

        [JsonIgnore]
        public virtual AvoidInfo VhAvoidInfo { get; set; }
        public class AvoidInfo
        {
            public string BlockedSectionID { get; }
            public string BlockedVehicleID { get; }
            public AvoidInfo(string blockedSectionID, string blockedVehicleID)
            {
                BlockedSectionID = SCUtility.Trim(blockedSectionID, true);
                BlockedVehicleID = SCUtility.Trim(blockedVehicleID, true);
            }
        }

        public com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage.VhStopSingle RESERVE_PAUSE { get; set; }
        [JsonIgnore]
        public virtual bool IsReservePause
        {
            get { return RESERVE_PAUSE == VhStopSingle.StopSingleOn; }
            set { }
        }
        [JsonIgnore]
        public virtual double Speed { get; set; }
        [JsonIgnore]
        public virtual string ObsVehicleID { get; set; }
        [JsonIgnore]
        public virtual List<string> Alarms { get; set; }

        [JsonIgnore]
        public virtual bool IsNeedAttentionBoxStatus { get; set; }

        [JsonIgnore]
        public virtual E_CMD_TYPE CmdType { get; set; } = default(E_CMD_TYPE);

        [JsonIgnore]
        public virtual E_CMD_STATUS vh_CMD_Status { get; set; }

        [JsonIgnore]
        [BaseElement(NonChangeFromOtherVO = true)]
        public object BlockControl_SyncForRedis = new object();

        [JsonIgnore]
        [BaseElement(NonChangeFromOtherVO = true)]
        public object PositionRefresh_Sync = new object();

        [JsonIgnore]
        [BaseElement(NonChangeFromOtherVO = true)]
        public object DoCreatTransferCommand_Sync = new object();
        [JsonIgnore]
        [BaseElement(NonChangeFromOtherVO = true)]
        public object Connection_Sync = new object();

        [JsonIgnore]
        [BaseElement(NonChangeFromOtherVO = true)]
        public Stopwatch PositionRefreshTimer = new Stopwatch();

        public int Pixel_Loaction_X = 0;
        public int Pixel_Loaction_Y = 0;

        private EventType vhRecentTranEcent = EventType.AdrPass;
        public virtual EventType VhRecentTranEvent
        {
            get { return vhRecentTranEcent; }
            set
            {
                vhRecentTranEcent = value;
                switch (value)
                {
                    case EventType.LoadComplete:
                    case EventType.UnloadComplete:
                    case EventType.LoadArrivals:
                    case EventType.UnloadArrivals:
                    case EventType.AdrOrMoveArrivals:
                    case EventType.Vhloading:
                    case EventType.Vhunloading:
                        sw_speed.Restart();
                        break;

                }
            }
        }

        //public Google.Protobuf.Collections.RepeatedField<ReserveInfo> VhRecentRequestSection = new Google.Protobuf.Collections.RepeatedField<ReserveInfo>();
        public Google.Protobuf.Collections.RepeatedField<ReserveInfo> VhRecentRequestSection = null;

        [JsonIgnore]
        [BaseElement(NonChangeFromOtherVO = true)]
        private int repeatReceiveImportantEventCount = 0;
        public int RepeatReceiveImportantEventCount
        {
            get { return repeatReceiveImportantEventCount; }
            set
            {
                repeatReceiveImportantEventCount = value;
                if (repeatReceiveImportantEventCount > MAX_ALLOW_IMPORTANT_EVENT_RETRY_COUNT)
                {
                    HasImportantEventReportRetryOverTimes?.Invoke(this, vhRecentTranEcent);
                }
            }
        }

        public Stopwatch ReserveRequestFailDuration { get; private set; } = new Stopwatch();

        /// <summary>
        /// 重複收到已經要求成功的Section路段的路權詢問
        /// </summary>
        [JsonIgnore]
        [BaseElement(NonChangeFromOtherVO = true)]
        private int repeatReceiveReserveRequestSuccessSection = 0;
        public int RepeatReceiveReserveRequestSuccessSection
        {
            get { return repeatReceiveReserveRequestSuccessSection; }
            set
            {
                repeatReceiveReserveRequestSuccessSection = value;
                if (repeatReceiveReserveRequestSuccessSection > MAX_ALLOW_IMPORTANT_EVENT_RETRY_COUNT)
                {
                    HasReserveRequestRetryOverTimes?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public BCRReadResult BCRReadResult = BCRReadResult.BcrNormal;

        public VehicleState State = VehicleState.Remove;
        public VehicleErrorState errorState = VehicleErrorState.NoAlarm;

        private string tcpip_msg_satae;
        [JsonIgnore]
        [BaseElement(NonChangeFromOtherVO = true)]
        public string TcpIp_Msg_State
        {
            get
            {
                return tcpip_msg_satae;
            }
            set
            {
                if (tcpip_msg_satae != value)
                {
                    tcpip_msg_satae = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.TcpIp_Msg_State));
                }
            }
        }

        private VehicleInfoFromPLC status_info_plc;
        [JsonIgnore]
        [BaseElement(NonChangeFromOtherVO = true)]
        public VehicleInfoFromPLC Status_Info_PLC
        {
            get
            {
                return status_info_plc;
            }
            set
            {
                status_info_plc = value;
                OnPropertyChanged(BCFUtility.getPropertyName(() => this.Status_Info_PLC));
            }
        }


        private int procprogress_percen;
        [JsonIgnore]
        [BaseElement(NonChangeFromOtherVO = true)]
        public virtual int procProgress_Percen
        {
            get { return procprogress_percen; }
            set
            {
                if (procprogress_percen != value)
                {
                    procprogress_percen = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.procProgress_Percen));
                }
            }
        }


        [BaseElement(NonChangeFromOtherVO = true)]
        public bool isSynchronizing;

        [BaseElement(NonChangeFromOtherVO = true)]
        public bool isLongTimeReserveRequestFailHappend;


        private bool istcpipconnect;
        [BaseElement(NonChangeFromOtherVO = true)]
        public virtual bool isTcpIpConnect
        {
            get { return istcpipconnect; }
            set
            {
                if (istcpipconnect != value)
                {
                    sw_speed.Restart();
                    istcpipconnect = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.isTcpIpConnect), VEHICLE_ID);
                    ConnectionStatusChange?.Invoke(this, istcpipconnect);
                }
            }
        }
        public bool isLongTimeDisconnection { get; private set; }



        [BaseElement(NonChangeFromOtherVO = true)]
        public virtual bool isAuto
        {
            get
            {
                return MODE_STATUS == VHModeStatus.AutoMtl ||
                       MODE_STATUS == VHModeStatus.AutoMts ||
                       MODE_STATUS == VHModeStatus.AutoLocal ||
                       MODE_STATUS == VHModeStatus.AutoRemote;
            }
        }


        //public bool IsPresence;
        public Stopwatch watchObstacleTime = new Stopwatch();
        [JsonIgnore]
        [BaseElement(NonChangeFromOtherVO = true)]
        public VhStopSingle ObstacleStatus
        {
            get { return OBS_PAUSE; }
            set
            {
                if (OBS_PAUSE != value)
                {
                    OBS_PAUSE = value;
                    if (OBS_PAUSE == VhStopSingle.StopSingleOn)
                    {
                        watchObstacleTime.Restart();
                    }
                    else
                    {
                        sw_speed.Restart();
                        watchObstacleTime.Stop();
                    }
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.ObstacleStatus));
                }
            }
        }
        [JsonIgnore]
        public virtual bool IsObstacle
        {
            get { return OBS_PAUSE == VhStopSingle.StopSingleOn; }
        }
        public Stopwatch watchBlockTime = new Stopwatch();
        [JsonIgnore]
        [BaseElement(NonChangeFromOtherVO = true)]
        public VhStopSingle BlockingStatus
        {
            get { return BLOCK_PAUSE; }
            set
            {
                if (BLOCK_PAUSE != value)
                {
                    BLOCK_PAUSE = value;
                    if (BLOCK_PAUSE == VhStopSingle.StopSingleOn)
                    {
                        watchBlockTime.Restart();
                    }
                    else
                    {
                        sw_speed.Restart();
                        watchBlockTime.Stop();
                    }
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.BlockingStatus));
                }
            }
        }
        [JsonIgnore]
        public virtual bool IsBlocking
        {
            get { return BLOCK_PAUSE == VhStopSingle.StopSingleOn; }
        }
        public Stopwatch watchPauseTime = new Stopwatch();
        [JsonIgnore]
        [BaseElement(NonChangeFromOtherVO = true)]
        public VhStopSingle PauseStatus
        {
            get { return CMD_PAUSE; }
            set
            {
                if (CMD_PAUSE != value)
                {
                    CMD_PAUSE = value;
                    if (CMD_PAUSE == VhStopSingle.StopSingleOn)
                    {
                        watchPauseTime.Restart();
                    }
                    else
                    {
                        sw_speed.Restart();
                        watchPauseTime.Stop();
                    }
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.PauseStatus));
                }
            }
        }
        [JsonIgnore]
        public virtual bool IsPause
        {
            get { return CMD_PAUSE == VhStopSingle.StopSingleOn; }
            set { }
        }
        [JsonIgnore]
        public virtual bool IsError
        {
            get { return ERROR == VhStopSingle.StopSingleOn; }
            set { }
        }
        private bool isLongTimeObstacling { get; set; }

        public Stopwatch watchHIDTime = new Stopwatch();
        [JsonIgnore]
        [BaseElement(NonChangeFromOtherVO = true)]
        public VhStopSingle HIDStatus
        {
            get { return HID_PAUSE; }
            set
            {
                if (HID_PAUSE != value)
                {
                    HID_PAUSE = value;
                    if (HID_PAUSE == VhStopSingle.StopSingleOn)
                    {
                        watchHIDTime.Restart();
                    }
                    else
                    {
                        sw_speed.Restart();
                        watchHIDTime.Stop();
                    }
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.HIDStatus));
                }
            }
        }
        [JsonIgnore]
        public virtual bool IsHIDPause
        {
            get { return HID_PAUSE == VhStopSingle.StopSingleOn; }
            set { }
        }
        public bool IsCycleMove(ConcurrentDictionary<string, ACMD_OHTC> currentExCmdOhtc)
        {
            if (ACT_STATUS != VHActionStatus.Commanding)
            {
                return false;
            }
            if (SCUtility.isEmpty(OHTC_CMD))
            {
                bool is_exist = currentExCmdOhtc.TryGetValue(sc.Common.SCUtility.Trim(CurrentExcuteCmdID, true), out ACMD_OHTC cmd_ohtc);
                if (!is_exist) return false;
                if (cmd_ohtc.CMD_TPYE == E_CMD_TYPE.Round)
                    return true;
                else
                    return false;
            }
            else
            {
                bool is_exist = currentExCmdOhtc.TryGetValue(sc.Common.SCUtility.Trim(OHTC_CMD, true), out ACMD_OHTC cmd_ohtc);
                if (!is_exist) return false;
                if (cmd_ohtc.CMD_TPYE == E_CMD_TYPE.Round)
                    return true;
                else
                    return false;
            }
        }
        public virtual string NODE_ID { get; set; }
        public virtual string PreAssignMCSCommandID { get; set; }
        [JsonIgnore]
        public virtual bool IsCommandSending { get; set; }
        [JsonIgnore]
        public virtual int CheckCommandIDFromCommandIDRequestFailTimes { get; set; }

        //public ACMD_OHTC currentExcuteCmd = null;
        [JsonIgnore]
        public string VhExcuteCMDStatusChangeEvent = "VhExcuteCMDStatusChangeEvent";
        public void NotifyVhExcuteCMDStatusChange()
        {
            OnPropertyChanged(BCFUtility.getPropertyName(() => this.VhExcuteCMDStatusChangeEvent), VEHICLE_ID);
        }
        [JsonIgnore]
        public string VhStatusChangeEvent = "VhStatusChangeEvent";
        public void NotifyVhStatusChange()
        {
            OnPropertyChanged(BCFUtility.getPropertyName(() => this.VhStatusChangeEvent), VEHICLE_ID);
        }
        [JsonIgnore]
        public string VhPositionChangeEvent = "VhPositionChangeEvent";
        public void NotifyVhPositionChange()
        {
            OnPropertyChanged(BCFUtility.getPropertyName(() => this.VhPositionChangeEvent), VEHICLE_ID);
        }



        public void Action()
        {
            CurrentCommandExcuteTime.Restart();
        }
        public void Stop()
        {
            CurrentCommandExcuteTime.Reset();
        }


        public string getCurrentSegment(BLL.SectionBLL sectionBLL)
        {
            return sectionBLL.cache.GetSection(CUR_SEC_ID)?.SEG_NUM;
        }


        public bool send_Str1(ID_1_HOST_BASIC_INFO_VERSION_REP sned_gpp, out ID_101_HOST_BASIC_INFO_VERSION_RESPONSE receive_gpp)
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            return mapAction.sned_Str1(sned_gpp, out receive_gpp);
        }
        public bool sned_S11(ID_11_ZONE_COMMAND_INFO_REP sned_gpp, out ID_111_ZONE_COMMAND_INFO_RESPONSE receive_gpp)
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            return mapAction.sned_Str11(sned_gpp, out receive_gpp);
        }
        public bool sned_S13(ID_13_TAVELLING_DATA_REP sned_gpp, out ID_113_TAVELLING_DATA_RESPONSE receive_gpp)
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            return mapAction.sned_Str13(sned_gpp, out receive_gpp);
        }
        public bool sned_S15(ID_15_SECTION_DATA_REP send_gpp, out ID_115_SECTION_DATA_RESPONSE receive_gpp)
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            return mapAction.sned_Str15(send_gpp, out receive_gpp);
        }
        public bool sned_S17(ID_17_ADDRESS_DATA_REP send_gpp, out ID_117_ADDRESS_DATA_RESPONSE receive_gpp)
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            return mapAction.sned_Str17(send_gpp, out receive_gpp);
        }
        public bool sned_S19(ID_19_SCALE_DATA_REP send_gpp, out ID_119_SCALE_DATA_RESPONSE receive_gpp)
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            return mapAction.sned_Str19(send_gpp, out receive_gpp);
        }

        public bool sned_S21(ID_21_CONTROL_DATA_REP send_gpp, out ID_121_CONTROL_DATA_RESPONSE receive_gpp)
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            return mapAction.sned_Str21(send_gpp, out receive_gpp);
        }
        public bool sned_S23(ID_23_GUIDE_DATA_REP send_gpp, out ID_123_GUIDE_DATA_RESPONSE receive_gpp)
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            return mapAction.sned_Str23(send_gpp, out receive_gpp);
        }


        public bool sned_S61(ID_61_INDIVIDUAL_UPLOAD_REQ send_gpp, out ID_161_INDIVIDUAL_UPLOAD_RESPONSE receive_gpp)
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            return mapAction.sned_Str61(send_gpp, out receive_gpp);
        }
        public bool sned_S63(ID_63_INDIVIDUAL_CHANGE_REQ send_gpp, out ID_163_INDIVIDUAL_CHANGE_RESPONSE receive_gpp)
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            return mapAction.sned_Str63(send_gpp, out receive_gpp);
        }
        public bool sned_S41(ID_41_MODE_CHANGE_REQ send_gpp, out ID_141_MODE_CHANGE_RESPONSE receive_gpp)
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            return mapAction.sned_Str41(send_gpp, out receive_gpp);
        }
        public bool send_S43(ID_43_STATUS_REQUEST send_gpp, out ID_143_STATUS_RESPONSE receive_gpp)
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            return mapAction.send_Str43(send_gpp, out receive_gpp);
        }
        public bool sned_S45(ID_45_POWER_OPE_REQ send_gpp, out ID_145_POWER_OPE_RESPONSE receive_gpp)
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            return mapAction.sned_Str45(send_gpp, out receive_gpp);
        }
        public bool sned_S91(ID_91_ALARM_RESET_REQUEST send_gpp, out ID_191_ALARM_RESET_RESPONSE receive_gpp)
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            return mapAction.sned_Str91(send_gpp, out receive_gpp);
        }


        public void registeredProcEvent()
        {
            getExcuteMapAction().RegisteredTcpIpProcEvent();
        }
        public void unRegisteredProcEvent()
        {
            getExcuteMapAction().UnRgisteredProcEvent();
        }

        public (bool isSuccess, SCAppConstants.SEND_CMD_OHTC_NG_TYPE ngType) sned_Str31(ID_31_TRANS_REQUEST send_gpp, out ID_131_TRANS_RESPONSE receive_gpp, out string reason)
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            return mapAction.send_Str31(send_gpp, out receive_gpp, out reason);
        }

        public bool sned_Str35(ID_35_CARRIER_ID_RENAME_REQUEST send_gpp, out ID_135_CARRIER_ID_RENAME_RESPONSE receive_gpp)
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            return mapAction.send_Str35(send_gpp, out receive_gpp);
        }

        public bool sned_Str37(string cmd_id, CMDCancelType actType)
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            return mapAction.send_Str37(cmd_id, actType);
        }
        public bool sned_Str39(ID_39_PAUSE_REQUEST sned_gpp, out ID_139_PAUSE_RESPONSE receive_gpp)
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            return mapAction.send_Str39(sned_gpp, out receive_gpp);
        }
        public void CatchPLCCSTInterfacelog()
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            mapAction.doCatchPLCCSTInterfaceLog();
        }

        public bool send_Str71(ID_71_RANGE_TEACHING_REQUEST send_gpp, out ID_171_RANGE_TEACHING_RESPONSE receive_gpp)
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            return mapAction.send_Str71(send_gpp, out receive_gpp);
        }

        private ValueDefMapActionBase getExcuteMapAction()
        {
            ValueDefMapActionBase mapAction;
            switch (SCApplication.getInstance().BC_ID)
            {
                default:
                    mapAction = this.getMapActionByIdentityKey(typeof(EQTcpIpMapAction).Name) as EQTcpIpMapAction;
                    break;
            }

            return mapAction;
        }

        public bool sendMessage(WrapperMessage wrapper, bool isReply = false)
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            return mapAction.snedMessage(wrapper, isReply);
        }


        #region PLC Control
        public void PLC_Control_TrunOn()
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            isPLCInControl = true;
            mapAction.PLC_Control_TrunOn();
        }
        public void PLC_Control_TrunOff()
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            isPLCInControl = false;
            VehicleControlItemForPLC = new Boolean[16];
            mapAction.PLC_Control_TrunOff();
        }

        public bool setVehicleControlItemForPLC(Boolean[] items)
        {
            ValueDefMapActionBase mapAction = null;
            mapAction = getExcuteMapAction();
            VehicleControlItemForPLC = items;
            return mapAction.setVehicleControlItemForPLC(items);
        }

        public bool isPLCInControl { get; private set; }
        public Boolean[] VehicleControlItemForPLC { get; private set; }
        #endregion PLC Control


        #region TcpIpAgentInfo
        int CommunicationInterval_ms = 15000;
        public bool IsCommunication(BCFApplication bcfApp)
        {
            bool is_communication = false;
            Stopwatch fromLastCommTime = ITcpIpControl.StopWatch_FromTheLastCommTime(bcfApp, TcpIpAgentName);
            is_communication = fromLastCommTime.IsRunning ?
                fromLastCommTime.ElapsedMilliseconds < CommunicationInterval_ms : false;
            return is_communication;
        }
        public event EventHandler<bool> ConnectionStatusChange;

        public void getAgentInfo(BCFApplication bcfApp,
            out bool IsListening, out bool IsCommunication, out bool IsConnections,
            out DateTime connTime, out TimeSpan accConnTime,
            out DateTime disConnTime, out TimeSpan accDisConnTime,
            out int disconnTimes, out int lostPackets)
        {
            Stopwatch fromLastCommTime = ITcpIpControl.StopWatch_FromTheLastCommTime(bcfApp, TcpIpAgentName);
            IsCommunication = fromLastCommTime.IsRunning ?
                fromLastCommTime.ElapsedMilliseconds < CommunicationInterval_ms : false;
            IsConnections = ITcpIpControl.IsConnection(bcfApp, TcpIpAgentName);
            connTime = ITcpIpControl.ConnectionTime(bcfApp, TcpIpAgentName);
            accConnTime = ITcpIpControl.StopWatch_ConnectionTime(bcfApp, TcpIpAgentName).Elapsed;
            disConnTime = ITcpIpControl.DisconnectionTime(bcfApp, TcpIpAgentName);
            accDisConnTime = ITcpIpControl.StopWatch_DisconnectionTime(bcfApp, TcpIpAgentName).Elapsed;
            disconnTimes = ITcpIpControl.DisconnectionTimes(bcfApp, TcpIpAgentName);
            lostPackets = ITcpIpControl.NumberOfPacketsLost(bcfApp, TcpIpAgentName);
            //取得目前VH 使用的TCPIPAgent所對應的TCPIPServer是否有在進行聆聽中
            IsListening = IsTcpIpListening(bcfApp);
        }


        public bool IsTcpIpListening(BCFApplication bcfApp)
        {
            bool IsListening = false;
            int local_port = ITcpIpControl.getLocalPortNum(bcfApp, TcpIpAgentName);
            if (local_port != 0)
            {
                iibg3k0.ttc.Common.TCPIP.TcpIpServer tcpip_server = bcfApp.getTcpIpServerByPortNum(local_port);
                if (tcpip_server != null)
                {
                    IsListening = tcpip_server.IsListening;
                }
            }
            return IsListening;
        }

        public bool TransferReady(BLL.Interface.ICMDBLL cmdBLL, bool isUnload = false)
        {
            if (!isTcpIpConnect)
            {
                return false;
            }
            if (!IS_INSTALLED)
            {
                return false;
            }
            if (isSynchronizing)
            {
                return false;
            }
            if (MODE_STATUS != VHModeStatus.AutoRemote)
            {
                return false;
            }
            if (IsError)
            {
                return false;
            }
            if (!SCUtility.isEmpty(MCS_CMD))
            {
                return false;
            }
            if (!isUnload && HAS_CST == 1)
            {
                return false;
            }
            if (SCUtility.isEmpty(CUR_ADR_ID))
            {
                return false;
            }
            if (IsCommandSending)
            {
                return false;
            }
            if (cmdBLL.isCMD_OHTCWillSending(VEHICLE_ID))
            {
                return false;
            }
            return true;
        }

        public bool TransferReadyForTest(BLL.Interface.ICMDBLL cmdBLL, bool isUnload = false)
        {
            if (!isTcpIpConnect)
            {
                return false;
            }

            if (isSynchronizing)
            {
                return false;
            }
            if (MODE_STATUS != VHModeStatus.AutoRemote)
            {
                return false;
            }
            if (IsError)
            {
                return false;
            }
            if (!SCUtility.isEmpty(MCS_CMD))
            {
                return false;
            }
            if (!isUnload && HAS_CST == 1)
            {
                return false;
            }
            if (SCUtility.isEmpty(CUR_ADR_ID))
            {
                return false;
            }
            if (IsCommandSending)
            {
                return false;
            }
            if (cmdBLL.isCMD_OHTCWillSending(VEHICLE_ID))
            {
                return false;
            }
            return true;
        }



        public int getPortNum(BCFApplication bcfApp)
        {
            return ITcpIpControl.getLocalPortNum(bcfApp, TcpIpAgentName);
        }

        internal string getIPAddress(BCFApplication bcfApp)
        {
            if (SCUtility.isEmpty(TcpIpAgentName))
            {
                return string.Empty;
            }
            return ITcpIpControl.getRemoteIPAddress(bcfApp, TcpIpAgentName);
        }
        internal System.Net.IPEndPoint getIPEndPoint(BCFApplication bcfApp)
        {
            if (SCUtility.isEmpty(TcpIpAgentName))
            {
                return null;
            }
            return ITcpIpControl.RemoteEndPoint(bcfApp, TcpIpAgentName);
        }

        internal double getConnectionIntervalTime(BCFApplication bcfApp)
        {
            return ITcpIpControl.StopWatch_ConnectionTime(bcfApp, TcpIpAgentName).Elapsed.TotalSeconds;
        }
        internal double getDisconnectionIntervalTime(BCFApplication bcfApp)
        {
            return ITcpIpControl.StopWatch_DisconnectionTime(bcfApp, TcpIpAgentName).Elapsed.TotalSeconds;
        }

        internal double getFromTheLastCommTime(BCFApplication bcfApp)
        {
            return ITcpIpControl.StopWatch_FromTheLastCommTime(bcfApp, TcpIpAgentName).Elapsed.TotalSeconds;
        }
        internal void StopTcpIpConnection(BCFApplication bcfApp)
        {
            bcfApp.getTcpIpAgent(TcpIpAgentName).stop();
        }



        #endregion TcpIpAgentInfo

        public void OnBeforeUpdate()
        {
            VehicleObjToShow showObj = SCApplication.getInstance().getEQObjCacheManager().CommonInfo.ObjectToShow_list.
                Where(o => o.VEHICLE_ID == VEHICLE_ID).SingleOrDefault();
            if (showObj != null)
            {
                //showObj.cUR_ADR_ID = CUR_ADR_ID;
                //showObj.cUR_SEC_ID = CUR_SEC_ID;
                showObj.ACC_SEC_DIST = ACC_SEC_DIST;
                showObj.MODE_STATUS = MODE_STATUS;
                showObj.ACT_STATUS = ACT_STATUS;
                showObj.MCS_CMD = MCS_CMD;
                showObj.OHTC_CMD = OHTC_CMD;
                //showObj.bLOCK_PAUSE = BLOCK_PAUSE == ProtocolFormat.OHTMessage.VhStopSingle.StopSingleOn;
                //showObj.cMD_PAUSE = CMD_PAUSE == ProtocolFormat.OHTMessage.VhStopSingle.StopSingleOn;
                //showObj.oBS_PAUSE = OBS_PAUSE == ProtocolFormat.OHTMessage.VhStopSingle.StopSingleOn;
                showObj.BLOCK_PAUSE = BLOCK_PAUSE;
                showObj.CMD_PAUSE = CMD_PAUSE;
                showObj.OBS_PAUSE = OBS_PAUSE;
                showObj.OBS_DIST = OBS_DIST;
                //showObj.hAS_CST = HAS_CST;
                //showObj.cST_ID = CST_ID;
                showObj.UPD_TIME = UPD_TIME;
                showObj.VEHICLE_ACC_DIST = VEHICLE_ACC_DIST;
                //showObj.mANT_ACC_DIST = MANT_ACC_DIST;
                //showObj.mANT_DATE = MANT_DATE;
                //showObj.gRIP_COUNT = GRIP_COUNT;
                //showObj.gRIP_MANT_COUNT = GRIP_MANT_COUNT;
                //showObj.gRIP_MANT_DATE = GRIP_MANT_DATE;
                //showObj.nODE_ADR = NODE_ADR;
                showObj.IS_PARKING = IS_PARKING;
                showObj.PARK_TIME = PARK_TIME;
                //showObj.pACK_ADR_ID = PACK_ADR_ID;
                showObj.IS_CYCLING = IS_CYCLING;
                showObj.CYCLERUN_TIME = CYCLERUN_TIME;
                //showObj.cYCLERUN_ID = CYCLERUN_ID;
            }
        }

        public string getVhStatusInfo()
        {
            return vehicleStatusInfo.ToString();
        }
        public override void doShareMemoryInit(BCFAppConstants.RUN_LEVEL runLevel)
        {
            foreach (IValueDefMapAction action in valueDefMapActionDic.Values)
            {
                action.doShareMemoryInit(runLevel);
            }

        }


        public void initialParameter()
        {
            this.VEHICLE_ID = null;
            this.VEHICLE_TYPE = default(E_VH_TYPE);
            this.CUR_ADR_ID = null;
            this.CUR_SEC_ID = null;
            this.SEC_ENTRY_TIME = null;
            this.ACC_SEC_DIST = 0;
            this.MODE_STATUS = default(VHModeStatus);
            this.ACT_STATUS = default(VHActionStatus);
            this.MCS_CMD = null;
            this.OHTC_CMD = null;
            this.BLOCK_PAUSE = default(VhStopSingle);
            this.CMD_PAUSE = default(VhStopSingle);
            this.OBS_PAUSE = default(VhStopSingle);
            this.ERROR = default(VhStopSingle);
            this.OBS_DIST = 0;
            this.HAS_CST = 0;
            this.CST_ID = null;
            this.UPD_TIME = null;
            this.VEHICLE_ACC_DIST = 0;
            this.MANT_ACC_DIST = 0;
            this.MANT_DATE = null;
            this.GRIP_COUNT = 0;
            this.GRIP_MANT_COUNT = 0;
            this.GRIP_MANT_DATE = null;
            this.NODE_ADR = null;
            this.IS_PARKING = false;
            this.PARK_TIME = null;
            this.PARK_ADR_ID = null;
            this.IS_CYCLING = false;
            this.CYCLERUN_TIME = null;
            this.CYCLERUN_ID = null;
        }
        [JsonIgnore]
        public override string Version { get { return base.Version; } }
        [JsonIgnore]
        public override string EqptObjectCate { get { return base.EqptObjectCate; } }
        [JsonIgnore]
        [BaseElement(NonChangeFromOtherVO = true)]
        public override string SECSAgentName { get { return base.SECSAgentName; } set { base.SECSAgentName = value; } }
        [JsonIgnore]
        [BaseElement(NonChangeFromOtherVO = true)]
        public override string TcpIpAgentName { get { return base.TcpIpAgentName; } set { base.TcpIpAgentName = value; } }
        //
        // 摘要:
        //     真實的ID
        [JsonIgnore]
        [BaseElement(NonChangeFromOtherVO = true)]
        public override string Real_ID { get; set; }
        [JsonIgnore]
        [BaseElement(NonChangeFromOtherVO = true)]
        public int Num { get; set; }


        //void TransitionedHandler(Stateless.StateMachine<VehicleState, VehicleTrigger>.Transition transition)
        //{
        //    string Destination = transition.Destination.ToString();
        //    string Source = transition.Source.ToString();
        //    string Trigger = transition.Trigger.ToString();
        //    string IsReentry = transition.IsReentry.ToString();

        //    LogHelper.Log(logger: NLog.LogManager.GetCurrentClassLogger(), LogLevel: NLog.LogLevel.Debug, Class: nameof(AVEHICLE), Device: DEVICE_NAME_OHx,
        //                   Data: $"Vh:{VEHICLE_ID} message state,From:{Source} to:{Destination} by:{Trigger}.IsReentry:{IsReentry}",
        //                   VehicleID: VEHICLE_ID,
        //                   CarrierID: CST_ID);
        //}

        //void UnhandledTriggerHandler(VehicleState state, VehicleTrigger trigger)
        //{
        //    string SourceState = state.ToString();
        //    string Trigger = trigger.ToString();

        //    LogHelper.Log(logger: NLog.LogManager.GetCurrentClassLogger(), LogLevel: NLog.LogLevel.Debug, Class: nameof(AVEHICLE), Device: DEVICE_NAME_OHx,
        //                   Data: $"Vh:{VEHICLE_ID} message state ,unhandled trigger happend ,source state:{SourceState} trigger:{Trigger}",
        //                   VehicleID: VEHICLE_ID,
        //                   CarrierID: CST_ID);
        //}

        void TransitionedHandler<TState, TTrigger>(Stateless.StateMachine<TState, TTrigger>.Transition transition)
        {
            string Destination = transition.Destination.ToString();
            string Source = transition.Source.ToString();
            string Trigger = transition.Trigger.ToString();
            string IsReentry = transition.IsReentry.ToString();

            LogHelper.Log(logger: NLog.LogManager.GetCurrentClassLogger(), LogLevel: NLog.LogLevel.Debug, Class: nameof(AVEHICLE), Device: DEVICE_NAME_OHx,
                           Data: $"Vh:{VEHICLE_ID}  state,From:{Source} to:{Destination} by:{Trigger}.IsReentry:{IsReentry}",
                           VehicleID: VEHICLE_ID,
                           CarrierID: CST_ID);
        }

        void UnhandledTriggerHandler<TState, TTrigger>(TState state, TTrigger trigger)
        {
            string SourceState = state.ToString();
            string Trigger = trigger.ToString();

            LogHelper.Log(logger: NLog.LogManager.GetCurrentClassLogger(), LogLevel: NLog.LogLevel.Debug, Class: nameof(AVEHICLE), Device: DEVICE_NAME_OHx,
                           Data: $"Vh:{VEHICLE_ID}  state ,unhandled trigger happend ,source state:{SourceState} trigger:{Trigger}",
                           VehicleID: VEHICLE_ID,
                           CarrierID: CST_ID);
        }
        #region Vehicle state machine

        public class VehicleStateMachine : StateMachine<VehicleState, VehicleTrigger>
        {
            public VehicleStateMachine(Func<VehicleState> stateAccessor, Action<VehicleState> stateMutator)
                : base(stateAccessor, stateMutator)
            {
                VehicleStateMachineConfigInitial();
            }
            internal IEnumerable<VehicleTrigger> getPermittedTriggers()//回傳當前狀態可以進行的Trigger，且會檢查GaurdClause。
            {
                return this.PermittedTriggers;
            }


            internal VehicleState getCurrentState()//回傳當前的狀態
            {
                return this.State;
            }
            public List<string> getNextStateStrList()
            {
                List<string> nextStateStrList = new List<string>();
                foreach (VehicleTrigger item in this.PermittedTriggers)
                {
                    nextStateStrList.Add(item.ToString());
                }
                return nextStateStrList;
            }
            private void VehicleStateMachineConfigInitial()
            {
                this.Configure(VehicleState.NotAssigned)
                    .PermitIf(VehicleTrigger.VehicleAssign, VehicleState.Assigned, () => VehicleAssignGC())//guardClause為真才會執行狀態變化
                    .PermitIf(VehicleTrigger.VechileRemove, VehicleState.Remove, () => VechileRemoveGC());//guardClause為真才會執行狀態變化
                this.Configure(VehicleState.Assigned).OnEntry(() => this.Fire(VehicleTrigger.VehicleAssign))
                    .PermitIf(VehicleTrigger.VehicleAssign, VehicleState.Enroute)
                    .PermitIf(VehicleTrigger.VehicleUnassign, VehicleState.NotAssigned);
                this.Configure(VehicleState.Enroute).SubstateOf(VehicleState.Assigned)
                    .PermitIf(VehicleTrigger.VehicleArrive, VehicleState.Parked, () => VehicleArriveGC());//guardClause為真才會執行狀態變化
                                                                                                          //.PermitIf(VehicleTrigger.VehicleUnassign, VehicleState.NOT_ASSIGNED, () => VehicleUnassignGC());//guardClause為真才會執行狀態變化
                this.Configure(VehicleState.Parked).SubstateOf(VehicleState.Assigned)
                    .PermitIf(VehicleTrigger.VehicleDepart, VehicleState.Enroute, () => VehicleDepartGC())//guardClause為真才會執行狀態變化
                    .PermitIf(VehicleTrigger.VehicleAcquireStart, VehicleState.Acquiring, () => VehicleAcquireStartGC())//guardClause為真才會執行狀態變化
                    .PermitIf(VehicleTrigger.VehicleDepositStart, VehicleState.Depositing, () => VehicleDepositStartGC());//guardClause為真才會執行狀態變化
                this.Configure(VehicleState.Acquiring).SubstateOf(VehicleState.Assigned)
                    .PermitIf(VehicleTrigger.VehilceAcquireComplete, VehicleState.Parked, () => VehilceAcquireCompleteGC())//guardClause為真才會執行狀態變化
                    .PermitIf(VehicleTrigger.VehicleDepositStart, VehicleState.Depositing, () => VehicleDepositStartGC());//guardClause為真才會執行狀態變化
                this.Configure(VehicleState.Depositing).SubstateOf(VehicleState.Assigned)
                    .PermitIf(VehicleTrigger.VehicleDepositComplete, VehicleState.Parked, () => VehicleDepositCompleteGC())//guardClause為真才會執行狀態變化
                    .PermitIf(VehicleTrigger.VehicleAcquireStart, VehicleState.Acquiring, () => VehicleAcquireStartGC());//guardClause為真才會執行狀態變化
                this.Configure(VehicleState.Remove)
                    .PermitIf(VehicleTrigger.VehicleInstall, VehicleState.NotAssigned, () => VehicleInstallGC());//guardClause為真才會執行狀態變化

            }

            private bool VehicleArriveGC()
            {
                return true;
            }
            private bool VehicleUnassignGC()
            {
                return true;
            }
            private bool VehicleDepartGC()
            {
                return true;
            }
            private bool VehicleAcquireStartGC()
            {
                return true;
            }
            private bool VehicleDepositStartGC()
            {
                return true;
            }
            private bool VehilceAcquireCompleteGC()
            {
                return true;
            }
            private bool VehicleDepositCompleteGC()
            {
                return true;
            }
            private bool VehicleAssignGC()
            {
                return true;
            }
            private bool VechileRemoveGC()
            {
                return true;
            }
            private bool VehicleInstallGC()
            {
                return true;
            }
        }


        public enum VehicleTrigger //有哪些Trigger
        {
            VehicleArrive,
            VehicleDepart,
            VehicleAcquireStart,
            VehilceAcquireComplete,
            VehicleDepositStart,
            VehicleDepositComplete,
            VehicleUnassign,
            VehicleAssign,
            VechileRemove,
            VehicleInstall
        }
        public bool VehicleArrive()
        {
            try
            {
                if (vhStateMachine.CanFire(VehicleTrigger.VehicleArrive))//檢查當前狀態能否進行這個Trigger
                {
                    vhStateMachine.Fire(VehicleTrigger.VehicleArrive);//進行Trigger

                    //可以在這邊做事情

                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool VehicleDepart()
        {
            try
            {
                if (vhStateMachine.CanFire(VehicleTrigger.VehicleDepart))//檢查當前狀態能否進行這個Trigger
                {
                    vhStateMachine.Fire(VehicleTrigger.VehicleDepart);//進行Trigger

                    //可以在這邊做事情

                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool VehicleAcquireStart()
        {
            try
            {
                if (vhStateMachine.CanFire(VehicleTrigger.VehicleAcquireStart))//檢查當前狀態能否進行這個Trigger
                {
                    vhStateMachine.Fire(VehicleTrigger.VehicleAcquireStart);//進行Trigger

                    //可以在這邊做事情

                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool VehilceAcquireComplete()
        {
            try
            {
                if (vhStateMachine.CanFire(VehicleTrigger.VehilceAcquireComplete))//檢查當前狀態能否進行這個Trigger
                {
                    vhStateMachine.Fire(VehicleTrigger.VehilceAcquireComplete);//進行Trigger

                    //可以在這邊做事情

                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool VehicleDepositStart()
        {
            try
            {
                if (vhStateMachine.CanFire(VehicleTrigger.VehicleDepositStart))//檢查當前狀態能否進行這個Trigger
                {
                    vhStateMachine.Fire(VehicleTrigger.VehicleDepositStart);//進行Trigger

                    //可以在這邊做事情

                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool VehicleDepositComplete()
        {
            try
            {
                if (vhStateMachine.CanFire(VehicleTrigger.VehicleDepositComplete))//檢查當前狀態能否進行這個Trigger
                {
                    vhStateMachine.Fire(VehicleTrigger.VehicleDepositComplete);//進行Trigger

                    //可以在這邊做事情

                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool VehicleUnassign()
        {
            try
            {
                if (vhStateMachine.CanFire(VehicleTrigger.VehicleUnassign))//檢查當前狀態能否進行這個Trigger
                {
                    vhStateMachine.Fire(VehicleTrigger.VehicleUnassign);//進行Trigger

                    //可以在這邊做事情

                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool VehicleAssign()
        {
            try
            {
                if (vhStateMachine.CanFire(VehicleTrigger.VehicleAssign))//檢查當前狀態能否進行這個Trigger
                {
                    vhStateMachine.Fire(VehicleTrigger.VehicleAssign);//進行Trigger

                    //可以在這邊做事情

                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool VechileRemove()
        {
            try
            {
                if (vhStateMachine.CanFire(VehicleTrigger.VechileRemove))//檢查當前狀態能否進行這個Trigger
                {
                    vhStateMachine.Fire(VehicleTrigger.VechileRemove);//進行Trigger

                    //可以在這邊做事情

                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool VehicleInstall()
        {
            try
            {
                if (vhStateMachine.CanFire(VehicleTrigger.VehicleInstall))//檢查當前狀態能否進行這個Trigger
                {
                    vhStateMachine.Fire(VehicleTrigger.VehicleInstall);//進行Trigger

                    //可以在這邊做事情

                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion Vehicle state machine

        #region Vehicle Error Status

        public class VehicleErrorStateMachine : StateMachine<VehicleErrorState, VehicleErrorTrigger>
        {
            public VehicleErrorStateMachine(Func<VehicleErrorState> stateAccessor, Action<VehicleErrorState> stateMutator)
                : base(stateAccessor, stateMutator)
            {
                VehicleErrorStateMachineConfigInitial();
            }
            internal IEnumerable<VehicleErrorTrigger> getPermittedTriggers()//回傳當前狀態可以進行的Trigger，且會檢查GaurdClause。
            {
                return this.PermittedTriggers;
            }


            internal VehicleErrorState getCurrentState()//回傳當前的狀態
            {
                return this.State;
            }
            public List<string> getNextStateStrList()
            {
                List<string> nextStateStrList = new List<string>();
                foreach (VehicleTrigger item in this.PermittedTriggers)
                {
                    nextStateStrList.Add(item.ToString());
                }
                return nextStateStrList;
            }
            private void VehicleErrorStateMachineConfigInitial()
            {
                this.Configure(VehicleErrorState.NoAlarm)
                    .PermitIf(VehicleErrorTrigger.VehicleAlarmSet, VehicleErrorState.AlarmHappending);//guardClause為真才會執行狀態變化
                this.Configure(VehicleErrorState.AlarmHappending)
                    .PermitIf(VehicleErrorTrigger.VehicleAlarmClean, VehicleErrorState.AlarmConfirm)
                    .PermitIf(VehicleErrorTrigger.VehicleConfirmComplete, VehicleErrorState.NoAlarm);
                this.Configure(VehicleErrorState.AlarmConfirm)
                    .PermitIf(VehicleErrorTrigger.VehicleConfirmComplete, VehicleErrorState.NoAlarm)
                    .PermitIf(VehicleErrorTrigger.VehicleAlarmSet, VehicleErrorState.AlarmHappending);
            }
        }

        public enum VehicleErrorState //有哪些State
        {
            NoAlarm = 0,
            AlarmHappending = 1,
            AlarmConfirm = 2,
        }

        public enum VehicleErrorTrigger //有哪些Trigger
        {
            VehicleAlarmClean,
            VehicleAlarmSet,
            VehicleConfirmComplete
        }

        public bool VechileAlarmClean()
        {
            try
            {
                if (vhErrorStateMachine.CanFire(VehicleErrorTrigger.VehicleAlarmClean))
                {
                    vhErrorStateMachine.Fire(VehicleErrorTrigger.VehicleAlarmClean);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool VehicleAlarmSet()
        {
            try
            {
                if (vhErrorStateMachine.CanFire(VehicleErrorTrigger.VehicleAlarmSet))
                {
                    vhErrorStateMachine.Fire(VehicleErrorTrigger.VehicleAlarmSet);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool VehicleAlarmConfirmComplete()
        {
            try
            {
                if (vhErrorStateMachine.CanFire(VehicleErrorTrigger.VehicleConfirmComplete))
                {
                    vhErrorStateMachine.Fire(VehicleErrorTrigger.VehicleConfirmComplete);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion Vehicle Error Status
        //[TeaceMethodAspectAttribute]
        public class VehicleTimerAction : ITimerAction
        {
            private static Logger VehicleStatusInfoLogger = NLog.LogManager.GetLogger("VehicleStatusInfo");
            private static Logger logger = LogManager.GetCurrentClassLogger();
            AVEHICLE vh = null;
            SCApplication scApp = null;
            public VehicleTimerAction(AVEHICLE _vh, string name, long intervalMilliSec)
                : base(name, intervalMilliSec)
            {
                vh = _vh;
            }

            public override void initStart()
            {
                scApp = SCApplication.getInstance();
            }

            private long syncPoint = 0;
            public override void doProcess(object obj)
            {
                if (vh.IS_INSTALLED)
                    VehicleStatusInfoLogger.Info(vh.getVhStatusInfo());

                if (System.Threading.Interlocked.Exchange(ref syncPoint, 1) == 0)
                {
                    try
                    {

                        //if (!vh.isTcpIpConnect) return;
                        //1.檢查是否已經大於一定時間沒有進行通訊
                        double from_last_comm_time = vh.getFromTheLastCommTime(scApp.getBCFApplication());
                        //if (from_last_comm_time > AVEHICLE.MAX_ALLOW_NO_COMMUNICATION_TIME_SECOND)
                        if (from_last_comm_time > AVEHICLE.MAX_ALLOW_NO_COMMUNICATION_TIME_SECOND &&
                            vh.StatusRequestFailTimes <= AVEHICLE.MAX_STATUS_REQUEST_FAIL_TIMES)
                        {
                            vh.onLongTimeNoCommuncation();
                        }

                        checkIsLongTimeDisconnection();

                        if (!vh.isTcpIpConnect) return;
                        //檢查是否這台車是否有命令是處於Sending狀態
                        if (!vh.IsCommandSending)
                        {
                            var cmd = ACMD_OHTC.getCmdOhtcListOfCmdObj(vh.VEHICLE_ID, E_CMD_STATUS.Sending);
                            if (cmd != null)
                                vh.onExcuteCommandStatusNotMatch(cmd);
                        }
                        checkHasCommandNotFinishedInTime();
                        checkIsIdleEnough();
                        checkHasBoxOnVhWhenNoCommand();
                        checkIsCycleMovePausing();
                        checkHasReserveRequestFailLongTimeHappend();
                        checkObstacleStateAndIsLongTime();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(AVEHICLE), Device: "AGVC",
                           Data: ex,
                           VehicleID: vh.VEHICLE_ID,
                           CarrierID: vh.CST_ID);
                    }
                    finally
                    {
                        System.Threading.Interlocked.Exchange(ref syncPoint, 0);
                    }

                }
            }

            private void checkIsLongTimeDisconnection()
            {
                double disconnection_time = vh.getDisconnectionIntervalTime(scApp.getBCFApplication());
                //if (from_last_comm_time > AVEHICLE.MAX_ALLOW_NO_COMMUNICATION_TIME_SECOND)
                if (disconnection_time > AVEHICLE.LONG_TIME_DISCONNECTION_JUDGE_TIME_SECOND)
                {
                    if (!vh.isLongTimeDisconnection)
                    {
                        vh.isLongTimeDisconnection = true;
                        vh.onLongTimeDisconnection(true);
                    }
                }
                else
                {
                    if (vh.isLongTimeDisconnection)
                    {
                        vh.isLongTimeDisconnection = false;
                        vh.onLongTimeDisconnection(false);
                    }
                }
            }

            private void checkHasReserveRequestFailLongTimeHappend()
            {
                if (vh.ReserveRequestFailDuration.ElapsedMilliseconds > SystemParameter.MaxAllowReserveRequestFailTimeMS)
                {
                    if (!vh.isLongTimeReserveRequestFailHappend)
                    {
                        vh.isLongTimeReserveRequestFailHappend = true;
                        vh.onLongTimeReserveRequestFailHappend(true);
                    }
                }
                else
                {
                    if (vh.isLongTimeReserveRequestFailHappend)
                    {
                        vh.isLongTimeReserveRequestFailHappend = false;
                        vh.onLongTimeReserveRequestFailHappend(false);
                    }
                }
            }

            private void checkIsCycleMovePausing()
            {
                if (vh.IsCycleMove(ACMD_OHTC.CMD_OHTC_InfoList) &&
                   vh.IsPause)
                {
                    vh.onCycleMovePausing();
                }
            }

            private void checkHasBoxOnVhWhenNoCommand()
            {
                if (!sc.Common.SCUtility.isEmpty(vh.OHTC_CMD))
                {
                    return;
                }
                if (vh.ACT_STATUS == VHActionStatus.Commanding)
                {
                    return;
                }
                //if (vh.HAS_BOX == 1)
                if (vh.HAS_CST == 1)
                {
                    vh.onBoxIdleOnVh(vh.BOX_ID);
                }
                else
                {
                    return;
                }
            }

            private void checkHasCommandNotFinishedInTime()
            {
                double action_time = vh.CurrentCommandExcuteTime.Elapsed.TotalSeconds;
                if (action_time > AVEHICLE.MAX_ALLOW_ACTION_TIME_SECOND)
                {
                    if (!vh.isLongTimeInaction)
                    {
                        vh.isLongTimeInaction = true;
                        vh.isLongTimeInactionCMDID = vh.OHTC_CMD;
                        vh.onLongTimeInaction(vh.OHTC_CMD);
                    }
                    else
                    {
                        //do nothing
                    }
                }
                else
                {
                    if (vh.isLongTimeInaction)
                    {
                        //clear
                        scApp.TransferService.OHBC_AlarmCleared(scApp.getEQObjCacheManager().getLine().LINE_ID, ((int)Service.AlarmLst.OHT_CommandNotFinishedInTime).ToString());

                    }
                    vh.isLongTimeInaction = false;
                }
            }

            private void checkIsIdleEnough()
            {
                if (!vh.IsError &&
                    vh.MODE_STATUS == VHModeStatus.AutoRemote &&
                    vh.ACT_STATUS == VHActionStatus.NoCommand)
                {
                    if (!vh.IdleTime.IsRunning)
                        vh.IdleTime.Restart();
                    else
                    {
                        if (vh.IdleTime.ElapsedMilliseconds > AVEHICLE.MAX_ALLOW_VH_IDLE_TIME_MMILLI_SECOND)
                        {
                            vh.IdleTime.Restart();
                            vh.onIdleTimeIsEnough();
                        }
                    }
                }
                else
                {
                    if (vh.IdleTime.IsRunning)
                    {
                        vh.IdleTime.Reset();
                        vh.IdleTime.Stop();
                    }
                }
            }
            private void checkObstacleStateAndIsLongTime()
            {
                checkObstacleState();
                LongTimeObstaclingCheck();
            }

            private void checkObstacleState()
            {
                try
                {
                    if (vh.IsObstacle)
                    {

                        if (!vh.CurrentObstaclingTime.IsRunning)
                        {
                            vh.CurrentObstaclingTime.Restart();
                        }
                    }
                    else
                    {
                        vh.CurrentObstaclingTime.Reset();
                        vh.CurrentObstaclingTime.Stop();
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception:");
                }
            }
            private void LongTimeObstaclingCheck()
            {
                double obstacling_time = vh.CurrentObstaclingTime.Elapsed.TotalSeconds;
                if (obstacling_time > AVEHICLE.MAX_ALLOW_OBSTACLING_TIME_SECOND)
                {
                    if (!vh.isLongTimeObstacling)
                    {
                        vh.isLongTimeObstacling = true;
                        vh.onLongTimeObstacling();
                    }
                }
                else
                {
                    if (vh.isLongTimeObstacling)
                    {
                        vh.isLongTimeObstacling = false;
                        vh.onLongTimeObstacleFinish();
                    }
                }
            }
        }

    }

}
