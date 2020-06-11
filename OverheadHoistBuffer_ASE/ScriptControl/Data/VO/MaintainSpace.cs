using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction;
using com.mirle.ibg3k0.sc.Data.VO.Interface;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    public class MaintainSpace : AEQPT, IMaintainDevice
    {
        public string MTS_SEGMENT { get; private set; } = "013";
        public string MTS_ADDRESS { get; private set; } = "20292";
        public string MTS_SYSTEM_IN_ADDRESS { get; private set; } = "20199";

        public string DeviceID { get { return EQPT_ID; } set { } }
        public string DeviceSegment { get { return MTS_SEGMENT; } set { } }
        public string DeviceAddress { get { return MTS_ADDRESS; } set { } }
        //public string CurrentCarID { get; set; }
        //public bool HasVehicle { get; set; }
        //public bool StopSingle { get; set; }
        //public MTxMode MTxMode { get; set; }
        //public VhInPosition VhInPosition;
        //public bool CarInSafetyCheck { get; set; }
        //public bool CarOutSafetyCheck { get; set; }
        public string PreCarOutVhID { get; set; }
        public bool CancelCarOutRequest { get; set; }
        public bool CarOurSuccess { get; set; }

        public ushort CurrentPreCarOurID { get; set; }
        public ushort CurrentPreCarOurActionMode { get; set; }
        public ushort CurrentPreCarOurCSTExist { get; set; }
        public ushort CurrentPreCarOurSectionID { get; set; }
        public uint CurrentPreCarOurAddressID { get; set; }
        private uint currentPreCarOurDistance;
        public uint CurrentPreCarOurDistance
        {
            get { return currentPreCarOurDistance; }
            set
            {
                if (currentPreCarOurDistance != value)
                {
                    currentPreCarOurDistance = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.CurrentPreCarOurDistance));
                }
            }
        }
        public ushort CurrentPreCarOurSpeed { get; set; }
        public bool CarOutInterlock
        {
            get { return getExcuteMapAction().GetOHxC2MTL_CarOutInterlock(); }
            set { }
        }
        public bool CarInMoving
        {
            get { return getExcuteMapAction().GetOHxC2MTL_CarInMoving(); }
            set { }
        }
        public bool IsAlive { get { return base.Is_Eq_Alive; } set { } }

        public void setMTSSegment(string adrID)
        {
            MTS_SEGMENT = adrID;
        }
        public void setMTSAddress(string adrID)
        {
            MTS_ADDRESS = adrID;
        }
        public void setMTSSystemInAddress(string adrID)
        {
            MTS_SYSTEM_IN_ADDRESS = adrID;
        }

        public (bool isSendSuccess, UInt16 returnCode) carOutRequest(UInt16 carNum)
        {
            return getExcuteMapAction().OHxC_CarOutNotify(carNum);
        }
        public bool SetCarOutInterlock(bool onOff)
        {
            return getExcuteMapAction().setOHxC2MTL_CarOutInterlock(onOff);
        }
        public bool SetCarInMoving(bool onOff)
        {
            return getExcuteMapAction().setOHxC2MTL_CarInMoving(onOff);
        }

        public void setCarRealTimeInfo(UInt16 car_id, UInt16 action_mode, UInt16 cst_exist, UInt16 current_section_id, UInt32 current_address_id,
                                            UInt32 buffer_distance, UInt16 speed)
        {
            getExcuteMapAction().CarRealtimeInfo(car_id, action_mode, cst_exist, current_section_id, current_address_id, buffer_distance, speed);
        }

        private MTSValueDefMapActionNew getExcuteMapAction()
        {
            MTSValueDefMapActionNew mapAction;
            mapAction = this.getMapActionByIdentityKey(typeof(MTSValueDefMapActionNew).Name) as MTSValueDefMapActionNew;

            return mapAction;
        }

    }
}
