using com.mirle.ibg3k0.sc.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.PLC_Functions
{
    class VehicleHardwareInfo : PLC_FunBase
    {

        public DateTime Timestamp;
        [PLCElement(ValueName = "VH_INPUTSIGNALX00_X0F")]
        public Boolean[] InputSignalX00_X0F;
        [PLCElement(ValueName = "VH_INPUTSIGNALX10_X1F")]
        public Boolean[] InputSignalX10_X1F;
        [PLCElement(ValueName = "VH_INPUTSIGNALX20_X2F")]
        public Boolean[] InputSignalX20_X2F;
        [PLCElement(ValueName = "VH_INPUTSIGNALX30_X3F")]
        public Boolean[] InputSignalX30_X3F;
        [PLCElement(ValueName = "VH_INPUTSIGNALY40_Y4F")]
        public Boolean[] InputSignalY40_Y4F;
        [PLCElement(ValueName = "VH_INPUTSIGNALY50_Y5F")]
        public Boolean[] InputSignalY50_Y5F;
        [PLCElement(ValueName = "VH_INPUTSIGNALY60_Y6F")]
        public Boolean[] InputSignalY60_Y6F;
        [PLCElement(ValueName = "VH_INPUTSIGNALY70_Y7F")]
        public Boolean[] InputSignalY70_Y7F;
        [PLCElement(ValueName = "VH_AXIS_1_STATUS")]
        public UInt16 Axis_1_Status;
        [PLCElement(ValueName = "VH_AXIS_2_STATUS")]
        public UInt16 Axis_2_Status;
        [PLCElement(ValueName = "VH_AXIS_3_STATUS")]
        public UInt16 Axis_3_Status;
        [PLCElement(ValueName = "VH_AXIS_4_STATUS")]
        public UInt16 Axis_4_Status;
        [PLCElement(ValueName = "VH_AXIS_5_STATUS")]
        public UInt16 Axis_5_Status;
        [PLCElement(ValueName = "VH_AXIS_6_STATUS")]
        public UInt16 Axis_6_Status;
        [PLCElement(ValueName = "VH_AXIS_8_STATUS")]
        public UInt16 Axis_8_Status;
        [PLCElement(ValueName = "VH_AXIS_3_HOME_STATUS")]
        public UInt16 Axis_3_HOME_Status;
        [PLCElement(ValueName = "VH_AXIS_4_HOME_STATUS")]
        public UInt16 Axis_4_HOME_Status;
        [PLCElement(ValueName = "VH_AXIS_5_HOME_STATUS")]
        public UInt16 Axis_5_HOME_Status;
        [PLCElement(ValueName = "VH_AXIS_6_HOME_STATUS")]
        public UInt16 Axis_6_HOME_Status;
        [PLCElement(ValueName = "VH_D530_TRANRECV_STATUS")]
        public UInt16 D530_TranRecv_Status;
        [PLCElement(ValueName = "VH_D531_RECVPROC_STATUS")]
        public UInt16 D531_RecvProc_Status;
        [PLCElement(ValueName = "VH_D532_TRANRECV_STATUS")]
        public UInt16 D532_TranRecv_Status;
        [PLCElement(ValueName = "VH_D533_RECVPROC_STATUS")]
        public UInt16 D533_RecvProc_Status;
        [PLCElement(ValueName = "VH_BCR1_STATUS")]
        public UInt16 BCR1_Status;
        [PLCElement(ValueName = "VH_BCR2_STATUS")]
        public UInt16 BCR2_Status;
        [PLCElement(ValueName = "VH_MODE_STATUS")]
        public UInt16 Mode_Status;
        [PLCElement(ValueName = "VH_STARTUP_STATUS")]
        public UInt16 StartUp_Status;
        [PLCElement(ValueName = "VH_GUIDE_LOCK_STATUS")]
        public UInt16 Guide_Lock_Status;
        [PLCElement(ValueName = "VH_AUTO_TRANSITION_STATUS")]
        public UInt16 Auto_Transition_Status;
        [PLCElement(ValueName = "VH_TRAVELAXIS_HOME_STATUS")]
        public UInt16 TravelAxis_Home_Status;
        [PLCElement(ValueName = "VH_BASICDATA_UPDATE_STATUS")]
        public UInt16 BasicData_Update_Status;
        [PLCElement(ValueName = "VH_BASICDATA_RECVPROC_STATUS")]
        public UInt16 BasicData_RecvProc_Status;
        [PLCElement(ValueName = "VH_MANUAL_STATUS")]
        public UInt16 Manual_Status;
        [PLCElement(ValueName = "VH_TEACHING_STATUS")]
        public UInt16 Teaching_Status;
        [PLCElement(ValueName = "VH_AUTOOPERATION_STATUS")]
        public UInt16 AutoOperation_Status;
        [PLCElement(ValueName = "VH_LOADUNLOAD_STATUS")]
        public UInt16 LoadUnload_Status;
        [PLCElement(ValueName = "VH_LOADING_STATUS")]
        public UInt16 Loading_Status;
        [PLCElement(ValueName = "VH_UNLOADING_STATUS")]
        public UInt16 Unloading_Status;
        [PLCElement(ValueName = "VH_TRAVEL_STATUS")]
        public UInt16 Travel_Status;
        [PLCElement(ValueName = "VH_VHMOVE_STATUS")]
        public UInt16 VhMove_Status;
        [PLCElement(ValueName = "VH_TEMPSTOP_MONITORSTATUS")]
        public UInt16 TempStop_MonitorStatus;
        [PLCElement(ValueName = "VH_SPEEDCHANGE_STATUS")]
        public UInt16 SpeedChange_Status;
        [PLCElement(ValueName = "VH_SPEEDPOSITIONCHANGE_STATUS")]
        public UInt16 SpeedPositionChange_Status;
        [PLCElement(ValueName = "VH_MAGPROC_STATUS")]
        public UInt16 MAGProc_Status;
        [PLCElement(ValueName = "VH_BLOCKADDRESS_STATUS")]
        public UInt16 BlockAddress_Status;
        [PLCElement(ValueName = "VH_LOCKDECIDE_STATUS")]
        public UInt16 LockDecide_Status;
        [PLCElement(ValueName = "VH_LOCKCHANGE_STATUS")]
        public UInt16 LockChange_Status;
        [PLCElement(ValueName = "VH_OBSPROC_STATUS")]
        public UInt16 OBSProc_Status;
        [PLCElement(ValueName = "VH_SECPASS_STATUS")]
        public UInt16 SecPass_Status;
        [PLCElement(ValueName = "VH_BLOCKADDRESSRELEASE_STATUS")]
        public UInt16 BlockAddressRelease_Status;
        [PLCElement(ValueName = "VH_POWEROPERATION_STATUS")]
        public UInt16 PowerOperation_Status;
        [PLCElement(ValueName = "VH_LOGDATAREP_STATUS")]
        public UInt16 LogDataRep_Status;

        public override string ToString()
        {
            string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(this, JsHelper.jsBooleanArrayConverter, JsHelper.jsTimeConverter);
            sJson = sJson.Replace(nameof(Timestamp), "@timestamp");
            return sJson;
        }
    }

    

   
}
