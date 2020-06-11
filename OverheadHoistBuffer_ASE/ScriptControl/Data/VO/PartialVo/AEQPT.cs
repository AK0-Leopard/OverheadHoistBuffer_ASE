using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.Data.VO.Interface;
using com.mirle.ibg3k0.sc.ObjectRelay;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc
{
    public partial class AEQPT : BaseEQObject, IAlarmHisList, IVIDCollection
    {
        public AEQPT()
        {
            eqptObjectCate = SCAppConstants.EQPT_OBJECT_CATE_EQPT;
        }

        private AlarmHisList alarmHisList = new AlarmHisList();
        public VIDCollection VID_Collection;

        

        public virtual SCAppConstants.EqptType Type { get; set; }

        public List<AUNIT> UnitList;

        public override void doShareMemoryInit(BCFAppConstants.RUN_LEVEL runLevel)
        {
            foreach (IValueDefMapAction action in valueDefMapActionDic.Values)
            {
                action.doShareMemoryInit(runLevel);
            }
            //對sub eqpt進行初始化
            List<AUNIT> subUnitList = SCApplication.getInstance().getEQObjCacheManager().getUnitListByEquipment(EQPT_ID);
            if (subUnitList != null)
            {
                foreach (AUNIT unit in subUnitList)
                {
                    unit.doShareMemoryInit(runLevel);
                }
            }
            List<APORT> subPortList = SCApplication.getInstance().getEQObjCacheManager().getPortListByEquipment(EQPT_ID);
            if (subPortList != null)
            {
                foreach (APORT port in subPortList)
                {
                    port.doShareMemoryInit(runLevel);
                }
            }
            List<ABUFFER> subBuffList = SCApplication.getInstance().getEQObjCacheManager().getBuffListByEquipment(EQPT_ID);
            if (subBuffList != null)
            {
                foreach (ABUFFER buff in subBuffList)
                {
                    buff.doShareMemoryInit(runLevel);
                }
            }
        }

        public virtual void resetAlarmHis(List<ALARM> AlarmHisList)
        {
            alarmHisList.resetAlarmHis(AlarmHisList);
        }

        public VIDCollection getVIDCollection()
        {
            return VID_Collection;
        }

        public string Process_Data_Format { get; set; }             //A0.11
        private com.mirle.ibg3k0.sc.ConfigHandler.ProcessDataConfigHandler procDataConfigHandler;     //A0.11
        public com.mirle.ibg3k0.sc.ConfigHandler.ProcessDataConfigHandler getProcessDataConfigHandler()
        {
            if (BCFUtility.isEmpty(Process_Data_Format))
            {
                return null;
            }
            if (procDataConfigHandler == null)
            {
                procDataConfigHandler =
                    new com.mirle.ibg3k0.sc.ConfigHandler.ProcessDataConfigHandler(Process_Data_Format);
            }
            return procDataConfigHandler;
        }


        public Data.PLC_Functions.HIDToOHxC_ChargeInfo HID_Info;

        private string currentCarID;
        public string CurrentCarID
        {
            get { return currentCarID; }
            set
            {
                if (currentCarID != value)
                {
                    currentCarID = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.CurrentCarID));
                }
            }
        }
        public bool HasVehicle { get; set; }
        public bool StopSingle { get; set; }
        private MTxMode mTxMode;
        public MTxMode MTxMode
        {
            get { return mTxMode; }
            set
            {
                if (mTxMode != value)
                {
                    mTxMode = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.MTxMode));
                }
            }
        }
        private MTLLocation mTLLocation;
        public MTLLocation MTLLocation
        {
            get { return mTLLocation; }
            set
            {
                if (mTLLocation != value)
                {
                    mTLLocation = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.MTLLocation));
                }
            }
        }
        public MTLMovingStatus MTLMovingStatus;
        public UInt32 Encoder;

        public VhInPosition VhInPosition;
        public bool CarInSafetyCheck { get; set; }
        public bool CarOutSafetyCheck { get; set; }
        private bool is_Eq_Alive;
        public bool Is_Eq_Alive
        {
            get { return is_Eq_Alive; }
            set
            {
                if (is_Eq_Alive != value)
                {
                    is_Eq_Alive = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.Is_Eq_Alive));
                }
            }
        }
        //private bool alive_Signal;
        //public bool Alive_Signal
        //{
        //    get { return alive_Signal; }
        //    set
        //    {
        //        if (alive_Signal != value)
        //        {
        //            alive_Signal = value;
        //            OnPropertyChanged(BCFUtility.getPropertyName(() => this.Alive_Signal));
        //        }
        //    }
        //}

        private bool safetyCheckComplete;
        public bool SafetyCheckComplete
        {
            get { return safetyCheckComplete; }
            set
            {
                if (safetyCheckComplete != value)
                {
                    safetyCheckComplete = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.SafetyCheckComplete));
                }
            }
        }

        private bool safetyCheckRequest;
        public bool SafetyCheckRequest
        {
            get { return safetyCheckRequest; }
            set
            {
                if (safetyCheckRequest != value)
                {
                    safetyCheckRequest = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.SafetyCheckRequest));
                }
            }
        }

        private bool doorClosed;
        public bool DoorClosed
        {
            get { return doorClosed; }
            set
            {
                if (doorClosed != value)
                {
                    doorClosed = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.DoorClosed));
                }
            }
        }


        private bool interlock;
        public bool Interlock
        {
            get { return interlock; }
            set
            {
                if (interlock != value)
                {
                    interlock = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.Interlock));
                }
            }
        }
        public DateTime Eq_Alive_Last_Change_time = DateTime.MinValue;
        public DateTime plc_link_fail_time = DateTime.MinValue;
        private SCAppConstants.LinkStatus plc_link_stat;
        [BaseElement(NonChangeFromOtherVO = true)]
        public virtual SCAppConstants.LinkStatus Plc_Link_Stat
        {
            get { return plc_link_stat; }
            set
            {
                if (plc_link_stat != value)
                {
                    plc_link_stat = value;
                    if (plc_link_stat == SCAppConstants.LinkStatus.LinkFail)
                    {
                        plc_link_fail_time = DateTime.Now;
                    }
                    else
                    {
                        plc_link_fail_time = DateTime.MinValue;
                    }
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.Plc_Link_Stat));
                }
            }
        }
        private DateTime synchronizeTime;
        public DateTime SynchronizeTime
        {
            get { return synchronizeTime; }
            set
            {
                if (synchronizeTime != value)
                {
                    synchronizeTime = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.SynchronizeTime));
                }
            }
        }
        public string test { get; set; }

    }

}
