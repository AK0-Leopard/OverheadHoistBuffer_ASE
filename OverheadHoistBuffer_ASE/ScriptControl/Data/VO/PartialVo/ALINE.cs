using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using Stateless;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace com.mirle.ibg3k0.sc
{
    public partial class ALINE : BaseEQObject, IAlarmHisList
    {
        public ALINE()
        {
            //StopWatch_mcsConnectionTime = new Stopwatch();
            //StopWatch_mcsDisconnectionTime = new Stopwatch();
            //TSC_state_machine = new TSCStateMachine(TSCState.NONE);
            TSC_state_machine = new TSCStateMachine(() => SCStats, (state) => SCStats = state);
            TSC_state_machine.OnTransitioned(TransitionedHandler);

        }
        public void ReStartStateMachine()
        {
            TSC_state_machine = new TSCStateMachine(() => this.SCStats, (state) => this.SCStats = state);
            TSC_state_machine.OnTransitioned(TransitionedHandler);
        }

        public TSCStateMachine TSC_state_machine;

        public event EventHandler<EventArgs> LineStatusChange;
        public event EventHandler<EventArgs> OnLocalDisconnection;
        public event EventHandler<EventArgs> OnLocalConnection;

        #region MCS Online Check Item
        private bool alarmSetChecked = false;
        public bool AlarmSetChecked
        {
            get
            { return alarmSetChecked; }
            set
            {
                if (alarmSetChecked != value)
                {
                    alarmSetChecked = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.AlarmSetChecked));
                }
            }
        }
        private bool currentPortTypesChecked = false;
        public bool CurrentPortTypesChecked
        {
            get
            { return currentPortTypesChecked; }
            set
            {
                if (currentPortTypesChecked != value)
                {
                    currentPortTypesChecked = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.CurrentPortTypesChecked));
                }
            }
        }

        private bool currentEQPortStateChecked = false;
        public bool CurrentEQPortStateChecked
        {
            get
            { return currentEQPortStateChecked; }
            set
            {
                if (currentEQPortStateChecked != value)
                {
                    currentEQPortStateChecked = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.CurrentEQPortStateChecked));
                }
            }
        }
        private bool currentPortStateChecked = false;
        public bool CurrentPortStateChecked
        {
            get
            { return currentPortStateChecked; }
            set
            {
                if (currentPortStateChecked != value)
                {
                    currentPortStateChecked = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.CurrentPortStateChecked));
                }
            }
        }
        private bool currentStateChecked = false;
        public bool CurrentStateChecked
        {
            get
            { return currentStateChecked; }
            set
            {
                if (currentStateChecked != value)
                {
                    currentStateChecked = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.CurrentStateChecked));
                }
            }
        }
        private bool enhancedVehiclesChecked = false;
        public bool EnhancedVehiclesChecked
        {
            get
            { return enhancedVehiclesChecked; }
            set
            {
                if (enhancedVehiclesChecked != value)
                {
                    enhancedVehiclesChecked = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.EnhancedVehiclesChecked));
                }
            }
        }
        private bool tSCStateChecked = false;
        public bool TSCStateChecked
        {
            get
            { return tSCStateChecked; }
            set
            {
                if (tSCStateChecked != value)
                {
                    tSCStateChecked = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.TSCStateChecked));
                }
            }
        }
        private bool unitAlarmStateListChecked = false;
        public bool UnitAlarmStateListChecked
        {
            get
            { return unitAlarmStateListChecked; }
            set
            {
                if (unitAlarmStateListChecked != value)
                {
                    unitAlarmStateListChecked = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.UnitAlarmStateListChecked));
                }
            }
        }
        private bool enhancedTransfersChecked = false;
        public bool EnhancedTransfersChecked
        {
            get
            { return enhancedTransfersChecked; }
            set
            {
                if (enhancedTransfersChecked != value)
                {
                    enhancedTransfersChecked = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.EnhancedTransfersChecked));
                }
            }
        }
        private bool enhancedCarriersChecked = false;
        public bool EnhancedCarriersChecked
        {
            get
            { return enhancedCarriersChecked; }
            set
            {
                if (enhancedCarriersChecked != value)
                {
                    enhancedCarriersChecked = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.EnhancedCarriersChecked));
                }
            }
        }
        private bool laneCutListChecked = false;
        public bool LaneCutListChecked
        {
            get
            { return laneCutListChecked; }
            set
            {
                if (laneCutListChecked != value)
                {
                    laneCutListChecked = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.LaneCutListChecked));
                }
            }
        }
        public void resetOnlieCheckItem()
        {
            CurrentPortStateChecked = false;
            CurrentStateChecked = false;
            EnhancedVehiclesChecked = false;
            TSCStateChecked = false;
            UnitAlarmStateListChecked = false;
            EnhancedTransfersChecked = false;
            EnhancedCarriersChecked = false;
            LaneCutListChecked = false;
            CurrentEQPortStateChecked = false;
            CurrentPortTypesChecked = false;
            AlarmSetChecked = false;
        }
        #endregion MCS Online Check Item

        #region Ping Check Item
        private bool mCSConnectionSuccess = false;
        public bool MCSConnectionSuccess
        {
            get
            { return mCSConnectionSuccess; }
            set
            {
                if (mCSConnectionSuccess != value)
                {
                    mCSConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.MCSConnectionSuccess));
                }
            }
        }
        private bool routerConnectionSuccess = false;
        public bool RouterConnectionSuccess
        {
            get
            { return routerConnectionSuccess; }
            set
            {
                if (routerConnectionSuccess != value)
                {
                    routerConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.RouterConnectionSuccess));
                }
            }
        }
        private bool oHT1ConnectionSuccess = false;
        public bool OHT1ConnectionSuccess
        {
            get
            { return oHT1ConnectionSuccess; }
            set
            {
                if (oHT1ConnectionSuccess != value)
                {
                    oHT1ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.OHT1ConnectionSuccess));
                }
            }
        }
        private bool oHT2ConnectionSuccess = false;
        public bool OHT2ConnectionSuccess
        {
            get
            { return oHT2ConnectionSuccess; }
            set
            {
                if (oHT2ConnectionSuccess != value)
                {
                    oHT2ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.OHT2ConnectionSuccess));
                }
            }
        }
        private bool oHT3ConnectionSuccess = false;
        public bool OHT3ConnectionSuccess
        {
            get
            { return oHT3ConnectionSuccess; }
            set
            {
                if (oHT3ConnectionSuccess != value)
                {
                    oHT3ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.OHT3ConnectionSuccess));
                }
            }
        }
        private bool oHT4ConnectionSuccess = false;
        public bool OHT4ConnectionSuccess
        {
            get
            { return oHT4ConnectionSuccess; }
            set
            {
                if (oHT4ConnectionSuccess != value)
                {
                    oHT4ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.OHT4ConnectionSuccess));
                }
            }
        }
        private bool oHT5ConnectionSuccess = false;
        public bool OHT5ConnectionSuccess
        {
            get
            { return oHT5ConnectionSuccess; }
            set
            {
                if (oHT5ConnectionSuccess != value)
                {
                    oHT5ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.OHT5ConnectionSuccess));
                }
            }
        }
        private bool oHT6ConnectionSuccess = false;
        public bool OHT6ConnectionSuccess
        {
            get
            { return oHT6ConnectionSuccess; }
            set
            {
                if (oHT6ConnectionSuccess != value)
                {
                    oHT6ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.OHT6ConnectionSuccess));
                }
            }
        }
        private bool oHT7ConnectionSuccess = false;
        public bool OHT7ConnectionSuccess
        {
            get
            { return oHT7ConnectionSuccess; }
            set
            {
                if (oHT7ConnectionSuccess != value)
                {
                    oHT7ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.OHT7ConnectionSuccess));
                }
            }
        }
        private bool oHT8ConnectionSuccess = false;
        public bool OHT8ConnectionSuccess
        {
            get
            { return oHT8ConnectionSuccess; }
            set
            {
                if (oHT8ConnectionSuccess != value)
                {
                    oHT8ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.OHT8ConnectionSuccess));
                }
            }
        }
        private bool oHT9ConnectionSuccess = false;
        public bool OHT9ConnectionSuccess
        {
            get
            { return oHT9ConnectionSuccess; }
            set
            {
                if (oHT9ConnectionSuccess != value)
                {
                    oHT9ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.OHT9ConnectionSuccess));
                }
            }
        }
        private bool oHT10ConnectionSuccess = false;
        public bool OHT10ConnectionSuccess
        {
            get
            { return oHT10ConnectionSuccess; }
            set
            {
                if (oHT10ConnectionSuccess != value)
                {
                    oHT10ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.OHT10ConnectionSuccess));
                }
            }
        }
        private bool oHT11ConnectionSuccess = false;
        public bool OHT11ConnectionSuccess
        {
            get
            { return oHT11ConnectionSuccess; }
            set
            {
                if (oHT11ConnectionSuccess != value)
                {
                    oHT11ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.OHT11ConnectionSuccess));
                }
            }
        }
        private bool oHT12ConnectionSuccess = false;
        public bool OHT12ConnectionSuccess
        {
            get
            { return oHT12ConnectionSuccess; }
            set
            {
                if (oHT12ConnectionSuccess != value)
                {
                    oHT12ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.OHT12ConnectionSuccess));
                }
            }
        }
        private bool oHT13ConnectionSuccess = false;
        public bool OHT13ConnectionSuccess
        {
            get
            { return oHT13ConnectionSuccess; }
            set
            {
                if (oHT13ConnectionSuccess != value)
                {
                    oHT13ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.OHT13ConnectionSuccess));
                }
            }
        }
        private bool oHT14ConnectionSuccess = false;
        public bool OHT14ConnectionSuccess
        {
            get
            { return oHT14ConnectionSuccess; }
            set
            {
                if (oHT14ConnectionSuccess != value)
                {
                    oHT14ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.OHT14ConnectionSuccess));
                }
            }
        }

        private bool mTLConnectionSuccess = false;
        public bool MTLConnectionSuccess
        {
            get
            { return mTLConnectionSuccess; }
            set
            {
                if (mTLConnectionSuccess != value)
                {
                    mTLConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.MTLConnectionSuccess));

                    if (value == false)
                    {
                        //TODO: alarm set
                        OnLocalDisconnection?.Invoke("PLC", null);
                    }
                    else
                    {
                        //TODO: alarm clear
                        OnLocalConnection?.Invoke("PLC", null);
                    }
                }
            }
        }

        private bool mTSConnectionSuccess = false;
        public bool MTSConnectionSuccess
        {
            get
            { return mTSConnectionSuccess; }
            set
            {
                if (mTSConnectionSuccess != value)
                {
                    mTSConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.MTSConnectionSuccess));
                }
            }
        }

        private bool mTS2ConnectionSuccess = false;
        public bool MTS2ConnectionSuccess
        {
            get
            { return mTS2ConnectionSuccess; }
            set
            {
                if (mTS2ConnectionSuccess != value)
                {
                    mTS2ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.MTS2ConnectionSuccess));
                }
            }
        }

        private bool hID1ConnectionSuccess = false;
        public bool HID1ConnectionSuccess
        {
            get
            { return hID1ConnectionSuccess; }
            set
            {
                if (hID1ConnectionSuccess != value)
                {
                    hID1ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.HID1ConnectionSuccess));
                }
            }
        }

        private bool hID2ConnectionSuccess = false;
        public bool HID2ConnectionSuccess
        {
            get
            { return hID2ConnectionSuccess; }
            set
            {
                if (hID2ConnectionSuccess != value)
                {
                    hID2ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.HID2ConnectionSuccess));
                }
            }
        }


        private bool hID3ConnectionSuccess = false;
        public bool HID3ConnectionSuccess
        {
            get
            { return hID3ConnectionSuccess; }
            set
            {
                if (hID3ConnectionSuccess != value)
                {
                    hID3ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.HID3ConnectionSuccess));
                }
            }
        }


        private bool hID4ConnectionSuccess = false;
        public bool HID4ConnectionSuccess
        {
            get
            { return hID4ConnectionSuccess; }
            set
            {
                if (hID4ConnectionSuccess != value)
                {
                    hID4ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.HID4ConnectionSuccess));
                }
            }
        }



        private bool adam1ConnectionSuccess = false;
        public bool Adam1ConnectionSuccess
        {
            get
            { return adam1ConnectionSuccess; }
            set
            {
                if (adam1ConnectionSuccess != value)
                {
                    adam1ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.Adam1ConnectionSuccess));
                }
            }
        }

        private bool adam2ConnectionSuccess = false;
        public bool Adam2ConnectionSuccess
        {
            get
            { return adam2ConnectionSuccess; }
            set
            {
                if (adam2ConnectionSuccess != value)
                {
                    adam2ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.Adam2ConnectionSuccess));
                }
            }
        }


        private bool adam3ConnectionSuccess = false;
        public bool Adam3ConnectionSuccess
        {
            get
            { return adam3ConnectionSuccess; }
            set
            {
                if (adam3ConnectionSuccess != value)
                {
                    adam3ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.Adam3ConnectionSuccess));
                }
            }
        }


        private bool adam4ConnectionSuccess = false;
        public bool Adam4ConnectionSuccess
        {
            get
            { return adam4ConnectionSuccess; }
            set
            {
                if (adam4ConnectionSuccess != value)
                {
                    adam4ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.Adam4ConnectionSuccess));
                }
            }
        }


        private bool ap1ConnectionSuccess = false;
        public bool AP1ConnectionSuccess
        {
            get
            { return ap1ConnectionSuccess; }
            set
            {
                if (ap1ConnectionSuccess != value)
                {
                    ap1ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.AP1ConnectionSuccess));
                }
            }
        }

        private bool ap2ConnectionSuccess = false;
        public bool AP2ConnectionSuccess
        {
            get
            { return ap2ConnectionSuccess; }
            set
            {
                if (ap2ConnectionSuccess != value)
                {
                    ap2ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.AP2ConnectionSuccess));
                }
            }
        }

        private bool ap3ConnectionSuccess = false;
        public bool AP3ConnectionSuccess
        {
            get
            { return ap3ConnectionSuccess; }
            set
            {
                if (ap3ConnectionSuccess != value)
                {
                    ap3ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.AP3ConnectionSuccess));
                }
            }
        }


        private bool ap4ConnectionSuccess = false;
        public bool AP4ConnectionSuccess
        {
            get
            { return ap4ConnectionSuccess; }
            set
            {
                if (ap4ConnectionSuccess != value)
                {
                    ap4ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.AP4ConnectionSuccess));
                }
            }
        }


        private bool ap5ConnectionSuccess = false;
        public bool AP5ConnectionSuccess
        {
            get
            { return ap5ConnectionSuccess; }
            set
            {
                if (ap5ConnectionSuccess != value)
                {
                    ap5ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.AP5ConnectionSuccess));
                }
            }
        }


        private bool ap6ConnectionSuccess = false;
        public bool AP6ConnectionSuccess
        {
            get
            { return ap6ConnectionSuccess; }
            set
            {
                if (ap6ConnectionSuccess != value)
                {
                    ap6ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.AP6ConnectionSuccess));
                }
            }
        }


        private bool ap7ConnectionSuccess = false;
        public bool AP7ConnectionSuccess
        {
            get
            { return ap7ConnectionSuccess; }
            set
            {
                if (ap7ConnectionSuccess != value)
                {
                    ap7ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.AP7ConnectionSuccess));
                }
            }
        }


        private bool ap8ConnectionSuccess = false;
        public bool AP8ConnectionSuccess
        {
            get
            { return ap8ConnectionSuccess; }
            set
            {
                if (ap8ConnectionSuccess != value)
                {
                    ap8ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.AP8ConnectionSuccess));
                }
            }
        }


        private bool ap9ConnectionSuccess = false;
        public bool AP9ConnectionSuccess
        {
            get
            { return ap9ConnectionSuccess; }
            set
            {
                if (ap9ConnectionSuccess != value)
                {
                    ap9ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.AP9ConnectionSuccess));
                }
            }
        }


        private bool ap10ConnectionSuccess = false;
        public bool AP10ConnectionSuccess
        {
            get
            { return ap10ConnectionSuccess; }
            set
            {
                if (ap10ConnectionSuccess != value)
                {
                    ap10ConnectionSuccess = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.AP10ConnectionSuccess));
                }
            }
        }



        public void setConnectionInfo(Dictionary<string, CommuncationInfo> dicCommInfo)
        {
            foreach (KeyValuePair<string, CommuncationInfo> keyPair in dicCommInfo)
            {
                CommuncationInfo Info = keyPair.Value;

                switch (keyPair.Key)
                {
                    case "MCS":
                        MCSConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "ROUTER":
                        RouterConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "OHT1":
                        OHT1ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "OHT2":
                        OHT2ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "OHT3":
                        OHT3ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "OHT4":
                        OHT4ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "OHT5":
                        OHT5ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "OHT6":
                        OHT6ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "OHT7":
                        OHT7ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "OHT8":
                        OHT8ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "OHT9":
                        OHT9ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "OHT10":
                        OHT10ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "OHT11":
                        OHT11ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "OHT12":
                        OHT12ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "OHT13":
                        OHT13ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "OHT14":
                        OHT14ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "MTL":
                        MTLConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "MTS":
                        MTSConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "MTS2":
                        MTS2ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "HID1":
                        HID1ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "HID2":
                        HID2ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "HID3":
                        HID3ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "HID4":
                        HID4ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "ADAM6050_1":
                        Adam1ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "ADAM6050_2":
                        Adam2ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "ADAM6050_3":
                        Adam3ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "ADAM6050_4":
                        Adam4ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "AP1":
                        AP1ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "AP2":
                        AP2ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "AP3":
                        AP3ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "AP4":
                        AP4ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "AP5":
                        AP5ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "AP6":
                        AP6ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "AP7":
                        AP7ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "AP8":
                        AP8ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "AP9":
                        AP9ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    case "AP10":
                        AP10ConnectionSuccess = Info.IsConnectinoSuccess;
                        break;
                    //case "CHARGER_PLC":
                    //    ChargePLCConnectionSuccess = Info.IsConnectinoSuccess;
                    //    break;
                    //case "ADAM_1":
                    //    ADAM1ConnectionSuccess = Info.IsConnectinoSuccess;
                    //    break;
                    //case "ADAM_2":
                    //    ADAM2ConnectionSuccess = Info.IsConnectinoSuccess;
                    //    break;
                    //case "ADAM_3":
                    //    ADAM3ConnectionSuccess = Info.IsConnectinoSuccess;
                    //    break;
                    //case "ADAM_4":
                    //    ADAM4ConnectionSuccess = Info.IsConnectinoSuccess;
                    //    break;
                    //case "ADAM_5":
                    //    ADAM5ConnectionSuccess = Info.IsConnectinoSuccess;
                    //    break;
                }
            }
        }
        #endregion Ping Check Item


        #region Transfer
        private bool mCSCommandAutoAssign = true;
        public bool MCSCommandAutoAssign
        {
            get
            { return mCSCommandAutoAssign; }
            set
            {
                if (mCSCommandAutoAssign != value)
                {
                    mCSCommandAutoAssign = value;
                    if (mCSCommandAutoAssign == false) MCSAutoAssignLastOffTime = DateTime.Now;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.MCSCommandAutoAssign));
                }
            }
        }
        private DateTime mCSAutoAssignLastOffTime = DateTime.MinValue;
        public DateTime MCSAutoAssignLastOffTime
        {
            get
            { return mCSAutoAssignLastOffTime; }
            set
            {
                if (mCSAutoAssignLastOffTime != value)
                {
                    mCSAutoAssignLastOffTime = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.MCSAutoAssignLastOffTime));
                }
            }
        }
        #endregion Transfer

        #region DisplayModeParameter
        public bool isDisplayMode;
        public bool isCMDIndiSetChanged;
        public bool isDisplayLastCMD;
        public CMDIndiSettings DisplayLoopSetting;
        public int CMDIndiPriortySetting;
        public enum CMDIndiSettings
        {
            All,
            MCS,
            OHxC,
            Priority
        }
        #endregion

        private AlarmHisList alarmHisList = new AlarmHisList();
        private string current_park_type;
        public virtual string Currnet_Park_Type
        {
            get { return current_park_type; }
            set
            {
                if (current_park_type != value)
                {
                    current_park_type = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.Currnet_Park_Type));
                }
            }
        }
        private string current_cycle_type;
        public virtual string Currnet_Cycle_Type
        {
            get { return current_cycle_type; }
            set
            {
                if (current_cycle_type != value)
                {
                    current_cycle_type = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.Currnet_Cycle_Type));
                }
            }
        }

        private SCAppConstants.AppServiceMode servicemode;
        public SCAppConstants.AppServiceMode ServiceMode
        {
            get { return servicemode; }
            set
            {
                if (servicemode != value)
                {
                    servicemode = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.ServiceMode));
                }
            }
        }
        private bool serverPreStop = false;
        public bool ServerPreStop
        {
            get { return serverPreStop; }
            set
            {
                if (serverPreStop != value)
                {
                    serverPreStop = value;
                }
            }
        }
        private SCAppConstants.LinkStatus secs_link_stat;
        [BaseElement(NonChangeFromOtherVO = true)]
        public virtual SCAppConstants.LinkStatus Secs_Link_Stat
        {
            get { return secs_link_stat; }
            set
            {
                if (secs_link_stat != value)
                {
                    secs_link_stat = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.Secs_Link_Stat));
                }
            }
        }

        private SCAppConstants.LinkStatus redis_link_stat;
        [BaseElement(NonChangeFromOtherVO = true)]
        public virtual SCAppConstants.LinkStatus Redis_Link_Stat
        {
            get { return redis_link_stat; }
            set
            {
                if (redis_link_stat != value)
                {
                    redis_link_stat = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.Redis_Link_Stat));
                }
            }
        }

        private SCAppConstants.ExistStatus detectionsystemexist = SCAppConstants.ExistStatus.NoExist;
        [BaseElement(NonChangeFromOtherVO = true)]
        public virtual SCAppConstants.ExistStatus DetectionSystemExist
        {
            get { return detectionsystemexist; }
            set
            {
                if (detectionsystemexist != value)
                {
                    detectionsystemexist = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.DetectionSystemExist));
                }
            }
        }

        private bool isearthquakehappend;
        [BaseElement(NonChangeFromOtherVO = true)]
        public virtual bool IsEarthquakeHappend
        {
            get { return isearthquakehappend; }
            set
            {
                if (isearthquakehappend != value)
                {
                    isearthquakehappend = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.IsEarthquakeHappend));
                }
            }
        }


        private bool segmentpredisableexcuting;
        [BaseElement(NonChangeFromOtherVO = true)]
        public virtual bool SegmentPreDisableExcuting
        {
            get { return segmentpredisableexcuting; }
            set
            {
                if (segmentpredisableexcuting != value)
                {
                    segmentpredisableexcuting = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.SegmentPreDisableExcuting));
                }
            }
        }


        private Boolean establishComm;
        public virtual Boolean EstablishComm
        {
            get { return establishComm; }
            set
            {
                establishComm = value;
                OnPropertyChanged(BCFUtility.getPropertyName(() => this.EstablishComm));
            }
        }
        public DateTime mcsConnectionTime { get; private set; }
        public DateTime mcsDisconnectionTime { get; private set; }
        public Stopwatch StopWatch_mcsConnectionTime { get; set; }
        public Stopwatch StopWatch_mcsDisconnectionTime { get; set; }
        public void connInfoUpdate_Connection()
        {
            mcsConnectionTime = DateTime.Now;
            StopWatch_mcsConnectionTime.Restart();
            StopWatch_mcsDisconnectionTime.Stop();
        }
        public void connInfoUpdate_Disconnection()
        {
            mcsDisconnectionTime = DateTime.Now;
            StopWatch_mcsConnectionTime.Stop();
            StopWatch_mcsDisconnectionTime.Restart();
        }

        private SCAppConstants.LineHostControlState.HostControlState host_control_state = SCAppConstants.LineHostControlState.HostControlState.EQ_Off_line;
        public virtual SCAppConstants.LineHostControlState.HostControlState Host_Control_State
        {
            get { return host_control_state; }
            set
            {
                if (host_control_state != value)
                {
                    host_control_state = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.Host_Control_State));
                }
            }
        }
        public TSCState SCStats = TSCState.NONE;
        //public TSCState SCStats = TSCState.PAUSED;
        void TransitionedHandler(Stateless.StateMachine<TSCState, TSCTrigger>.Transition transition)
        {
            string Destination = transition.Destination.ToString();
            string Source = transition.Source.ToString();
            string Trigger = transition.Trigger.ToString();
            string IsReentry = transition.IsReentry.ToString();
            OnPropertyChanged(BCFUtility.getPropertyName(() => this.SCStats));
        }
        //public virtual TSCState SCStats
        //{
        //    get { return TSC_state_machine.State; }
        //}

        public const string CONTEXT_KEY_WORD_LINE_STATUS_HAS_CHANGE = "LineStatusChange";

        //private UInt16 currentShelfEnableCount = 0;
        //public UInt16 CurrentShelfEnableCount
        //{
        //    get
        //    {
        //        return currentShelfEnableCount;
        //    }
        //    set
        //    {
        //        if (currentShelfEnableCount != value)
        //        {
        //            currentShelfEnableCount = value;
        //            SCUtility.setCallContext(CONTEXT_KEY_WORD_LINE_STATUS_HAS_CHANGE, true);
        //        }
        //    }
        //}

        //private UInt16 currentShelfDisableCount = 0;
        //public UInt16 CurrentShelfDisableCount
        //{
        //    get
        //    {
        //        return currentShelfDisableCount;
        //    }
        //    set
        //    {
        //        if (currentShelfDisableCount != value)
        //        {
        //            currentShelfDisableCount = value;
        //            SCUtility.setCallContext(CONTEXT_KEY_WORD_LINE_STATUS_HAS_CHANGE, true);
        //        }
        //    }
        //}

        //private UInt16 currentShelfTotalSize = 0;
        //public UInt16 CurrentShelfTotalSize
        //{
        //    get
        //    {
        //        return currentShelfTotalSize;
        //    }
        //    set
        //    {
        //        if (currentShelfTotalSize != value)
        //        {
        //            currentShelfTotalSize = value;
        //            SCUtility.setCallContext(CONTEXT_KEY_WORD_LINE_STATUS_HAS_CHANGE, true);
        //        }
        //    }
        //}

        //private UInt16 currentShelfCapacity = 0;
        //public UInt16 CurrentShelfCapacity
        //{
        //    get
        //    {
        //        return currentShelfCapacity;
        //    }
        //    set
        //    {
        //        if (currentShelfCapacity != value)
        //        {
        //            currentShelfCapacity = value;
        //            SCUtility.setCallContext(CONTEXT_KEY_WORD_LINE_STATUS_HAS_CHANGE, true);
        //        }
        //    }
        //}

        private UInt16 currntVehicleModeAutoRemoteCount = 0;
        public UInt16 CurrntVehicleModeAutoRemoteCount
        {
            get
            { return currntVehicleModeAutoRemoteCount; }
            set
            {
                if (currntVehicleModeAutoRemoteCount != value)
                {
                    currntVehicleModeAutoRemoteCount = value;
                    SCUtility.setCallContext(CONTEXT_KEY_WORD_LINE_STATUS_HAS_CHANGE, true);
                }
            }
        }
        private UInt16 currntVehicleModeAutoLoaclCount = 0;
        public UInt16 CurrntVehicleModeAutoLoaclCount
        {
            get
            { return currntVehicleModeAutoLoaclCount; }
            set
            {
                if (currntVehicleModeAutoLoaclCount != value)
                {
                    currntVehicleModeAutoLoaclCount = value;
                    SCUtility.setCallContext(CONTEXT_KEY_WORD_LINE_STATUS_HAS_CHANGE, true);
                }
            }
        }
        private UInt16 currntVehicleStatusIdelCounr = 0;
        public UInt16 CurrntVehicleStatusIdelCount
        {
            get
            { return currntVehicleStatusIdelCounr; }
            set
            {
                if (currntVehicleStatusIdelCounr != value)
                {
                    currntVehicleStatusIdelCounr = value;
                    SCUtility.setCallContext(CONTEXT_KEY_WORD_LINE_STATUS_HAS_CHANGE, true);
                }
            }
        }
        private UInt16 currntVehicleStatusErrorCounr = 0;
        public UInt16 CurrntVehicleStatusErrorCount
        {
            get
            { return currntVehicleStatusErrorCounr; }
            set
            {
                if (currntVehicleStatusErrorCounr != value)
                {
                    currntVehicleStatusErrorCounr = value;
                    SCUtility.setCallContext(CONTEXT_KEY_WORD_LINE_STATUS_HAS_CHANGE, true);
                }
            }
        }

        private UInt16 currntCSTStatueTransferCount = 0;
        public UInt16 CurrntCSTStatueTransferCount
        {
            get
            { return currntCSTStatueTransferCount; }
            set
            {
                if (currntCSTStatueTransferCount != value)
                {
                    currntCSTStatueTransferCount = value;
                    SCUtility.setCallContext(CONTEXT_KEY_WORD_LINE_STATUS_HAS_CHANGE, true);
                }
            }
        }
        private UInt16 currntCSTStatueWaitingCount = 0;
        public UInt16 CurrntCSTStatueWaitingCount
        {
            get
            { return currntCSTStatueWaitingCount; }
            set
            {
                if (currntCSTStatueWaitingCount != value)
                {
                    currntCSTStatueWaitingCount = value;
                    SCUtility.setCallContext(CONTEXT_KEY_WORD_LINE_STATUS_HAS_CHANGE, true);
                }
            }
        }

        private UInt16 currntHostCommandTransferStatueAssignedCount = 0;
        public UInt16 CurrntHostCommandTransferStatueAssignedCount
        {
            get
            { return currntHostCommandTransferStatueAssignedCount; }
            set
            {
                if (currntHostCommandTransferStatueAssignedCount != value)
                {
                    currntHostCommandTransferStatueAssignedCount = value;
                    SCUtility.setCallContext(CONTEXT_KEY_WORD_LINE_STATUS_HAS_CHANGE, true);
                }
            }
        }
        private UInt16 currntHostCommandTransferStatueWaitingCounr = 0;
        public UInt16 CurrntHostCommandTransferStatueWaitingCounr
        {
            get
            { return currntHostCommandTransferStatueWaitingCounr; }
            set
            {
                if (currntHostCommandTransferStatueWaitingCounr != value)
                {
                    currntHostCommandTransferStatueWaitingCounr = value;
                    SCUtility.setCallContext(CONTEXT_KEY_WORD_LINE_STATUS_HAS_CHANGE, true);
                }
            }
        }

        public List<DeviceConnectionInfo> DeviceConnectionInfos;

        public void NotifyLineStatusChange()
        {
            LineStatusChange?.Invoke(this, null);
        }

        public override void doShareMemoryInit(BCFAppConstants.RUN_LEVEL runLevel)
        {
            foreach (IValueDefMapAction action in valueDefMapActionDic.Values)
            {
                action.doShareMemoryInit(runLevel);
            }
            //對sub eqpt進行初始化
            List<AZONE> subZoneList = SCApplication.getInstance().getEQObjCacheManager().getZoneListByLine();
            if (subZoneList != null)
            {
                foreach (AZONE zone in subZoneList)
                {
                    zone.doShareMemoryInit(runLevel);
                }
            }
        }

        public virtual void addAlarmHis(ALARM AlarmHis)
        {
            alarmHisList.addAlarmHis(AlarmHis);
        }

        public virtual void resetAlarmHis(List<ALARM> AlarmHisList)
        {
            alarmHisList.resetAlarmHis(AlarmHisList);
        }


        public override string Version { get { return base.Version; } }
        public override string EqptObjectCate { get { return base.EqptObjectCate; } }
        [BaseElement(NonChangeFromOtherVO = true)]
        public override string SECSAgentName { get { return base.SECSAgentName; } set { base.SECSAgentName = value; } }
        [BaseElement(NonChangeFromOtherVO = true)]
        public override string TcpIpAgentName { get { return base.TcpIpAgentName; } set { base.TcpIpAgentName = value; } }
        [BaseElement(NonChangeFromOtherVO = true)]
        public override string Real_ID { get; set; }

        private bool isAlarmHappened = false;
        public bool IsAlarmHappened
        {
            get { return isAlarmHappened; }
            set
            {
                if (isAlarmHappened != value)
                {
                    isAlarmHappened = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.IsAlarmHappened));
                }
            }
        }



        #region TSC state machine

        public class TSCStateMachine : StateMachine<TSCState, TSCTrigger>
        {
            public TSCStateMachine(Func<TSCState> stateAccessor, Action<TSCState> stateMutator)
                : base(stateAccessor, stateMutator)
            {
                TSCStateMachineConfigInitial();
            }
            public TSCStateMachine(TSCState state)
                : base(state)
            {
                TSCStateMachineConfigInitial();
            }
            internal IEnumerable<TSCTrigger> getPermittedTriggers()//回傳當前狀態可以進行的Trigger，且會檢查GaurdClause。
            {
                return this.PermittedTriggers;
            }

            internal TSCState getCurrentState()//回傳當前的狀態
            {
                return this.State;
            }
            public List<string> getNextStateStrList()
            {
                List<string> nextStateStrList = new List<string>();
                foreach (TSCTrigger item in this.PermittedTriggers)
                {
                    nextStateStrList.Add(item.ToString());
                }
                return nextStateStrList;
            }
            private void TSCStateMachineConfigInitial()
            {
                this.Configure(TSCState.NONE)
                    .PermitIf(TSCTrigger.AGVCInitial, TSCState.TSC_INIT, () => AGVCInitialGC());//guardClause為真才會執行狀態變化
                this.Configure(TSCState.TSC_INIT)
                    .PermitIf(TSCTrigger.StartUpSuccess, TSCState.PAUSED, () => StartUpSuccessGC())//guardClause為真才會執行狀態變化
                    .PermitIf(TSCTrigger.ResumeAuto, TSCState.AUTO, () => ResumeAutoGC());
                this.Configure(TSCState.PAUSING)
                    .PermitIf(TSCTrigger.ResumeAuto, TSCState.AUTO, () => ResumeAutoGC())//guardClause為真才會執行狀態變化
                    .PermitIf(TSCTrigger.PauseComplete, TSCState.PAUSED, () => PauseCompleteGC());//guardClause為真才會執行狀態變化
                this.Configure(TSCState.PAUSED)
                    .PermitIf(TSCTrigger.ResumeAuto, TSCState.AUTO, () => ResumeAutoGC());//guardClause為真才會執行狀態變化
                this.Configure(TSCState.AUTO)
                    .PermitIf(TSCTrigger.RequestPause, TSCState.PAUSING, () => RequestPauseGC());//guardClause為真才會執行狀態變化
            }

            private bool AGVCInitialGC()
            {
                return true;
            }
            private bool StartUpSuccessGC()
            {
                return true;
            }
            private bool ResumeAutoGC()
            {
                return true;
            }
            private bool PauseCompleteGC()
            {
                return true;
            }
            private bool RequestPauseGC()
            {
                return true;
            }
        }

        public enum TSCState //有哪些State
        {
            [System.ComponentModel.DataAnnotations.Display(Name = "None")]
            NONE = 0,
            [System.ComponentModel.DataAnnotations.Display(Name = "Init")]
            TSC_INIT = 1,
            [System.ComponentModel.DataAnnotations.Display(Name = "Paused")]
            PAUSED = 2,
            [System.ComponentModel.DataAnnotations.Display(Name = "Auto")]
            AUTO = 3,
            [System.ComponentModel.DataAnnotations.Display(Name = "Pausing")]
            PAUSING = 4
        }

        public enum TSCTrigger //有哪些Trigger
        {
            AGVCInitial,
            StartUpSuccess,
            ResumeAuto,
            RequestPause,
            PauseComplete
        }
        public string GetState()
        {
            return TSC_state_machine.State.ToString();
        }
        public bool AGVCInitialComplete(ReportBLL reportBLL)
        {
            try
            {
                if (TSC_state_machine.CanFire(TSCTrigger.AGVCInitial))//檢查當前狀態能否進行這個Trigger
                {
                    TSC_state_machine.Fire(TSCTrigger.AGVCInitial);//進行Trigger
                    //reportBLL.ReportTSCAutoInitiated();
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
        public bool StartUpSuccessed(ReportBLL reportBLL)
        {
            try
            {
                if (TSC_state_machine.CanFire(TSCTrigger.StartUpSuccess))//檢查當前狀態能否進行這個Trigger
                {
                    TSC_state_machine.Fire(TSCTrigger.StartUpSuccess);//進行Trigger
                    reportBLL.ReportTSCPaused();
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
        public bool ResumeToAuto(ReportBLL reportBLL)
        {
            try
            {
                if (TSC_state_machine.CanFire(TSCTrigger.ResumeAuto))//檢查當前狀態能否進行這個Trigger
                {
                    TSC_state_machine.Fire(TSCTrigger.ResumeAuto);//進行Trigger
                    //reportBLL.ReportTSCAutoInitiated();
                    reportBLL.ReportTSCAutoCompleted();
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
        public bool RequestToPause(ReportBLL reportBLL, string pausrReason)
        {
            try
            {
                if (TSC_state_machine.CanFire(TSCTrigger.RequestPause))//檢查當前狀態能否進行這個Trigger
                {
                    TSC_state_machine.Fire(TSCTrigger.RequestPause);//進行Trigger
                    reportBLL.ReportSCPauseInitiated();
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
        public bool PauseCompleted(ReportBLL reportBLL)
        {
            try
            {
                if (TSC_state_machine.CanFire(TSCTrigger.PauseComplete))//檢查當前狀態能否進行這個Trigger
                {
                    TSC_state_machine.Fire(TSCTrigger.PauseComplete);//進行Trigger
                    reportBLL.ReportTSCPauseCompleted();
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
        #endregion TSC state machine


        public void Put(LINE_INFO lineInfoGpb)
        {
            switch (lineInfoGpb.TSCState)
            {
                case ProtocolFormat.OHTMessage.TSCState.Tscnone:
                    SCStats = TSCState.NONE;
                    break;
                case ProtocolFormat.OHTMessage.TSCState.Tscint:
                    SCStats = TSCState.TSC_INIT;
                    break;
                case ProtocolFormat.OHTMessage.TSCState.Paused:
                    SCStats = TSCState.PAUSED;
                    break;
                case ProtocolFormat.OHTMessage.TSCState.Auto:
                    SCStats = TSCState.AUTO;
                    break;
                case ProtocolFormat.OHTMessage.TSCState.Pausing:
                    SCStats = TSCState.PAUSING;
                    break;
            }

            switch (lineInfoGpb.HostMode)
            {
                case  HostMode.Offline:
                    Host_Control_State = SCAppConstants.LineHostControlState.HostControlState.EQ_Off_line;
                    break;
                case  HostMode.OnlineLocal:
                    Host_Control_State = SCAppConstants.LineHostControlState.HostControlState.On_Line_Local;
                    break;
                case  HostMode.OnlineRemote:
                    Host_Control_State = SCAppConstants.LineHostControlState.HostControlState.On_Line_Remote;
                    break;
            }

        }


    }
}
