using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.PLC_Functions
{
    class PortPLCControl : PLC_FunBase
    {
        public DateTime Timestamp;

        [PLCElement(ValueName = "FAULT_RESET")]
        public bool PortReset;
        [PLCElement(ValueName = "BUZZER_STOP")]
        public bool PortStopBuzzer;
        [PLCElement(ValueName = "REMOTE_RUN")]
        public bool PortAuto;
        [PLCElement(ValueName = "REMOTE_STOP")]
        public bool PortManual;
        [PLCElement(ValueName = "ID_READ_CMD")]
        public bool PortIDRead;
        [PLCElement(ValueName = "MOVE_BACK")]
        public bool PortMoveBack;
        [PLCElement(ValueName = "BOX_IN_OK_MGV")]
        public bool MGVBoxInOK;
        [PLCElement(ValueName = "TOGGLE_BOX_COVER")]
        public bool ToggleBoxCover;
        [PLCElement(ValueName = "OPEN_DOOR_MGV")]
        public bool MGVOpenDoor;
        [PLCElement(ValueName = "AREA_SENSOR_CTRL")]
        public bool PortAreaSensorCtrl;
        [PLCElement(ValueName = "CHANGE_TO_INPUT_AGV")]
        public bool PortToMGVMode;
        [PLCElement(ValueName = "CHANGE_TO_OUTPUT_AGV")]
        public bool PortToAGVMode;
        [PLCElement(ValueName = "CHANGE_TO_INPUT_MODE")]
        public bool PortToInputMode;
        [PLCElement(ValueName = "CHANGE_TO_OUTPUT_MODE")]
        public bool PortToOutputMode;

        [PLCElement(ValueName = "VEHICLE_1_COMMANDING")]
        public bool VehicleCommanding1;
        [PLCElement(ValueName = "VEHICLE_2_COMMANDING")]
        public bool VehicleCommanding2;
        [PLCElement(ValueName = "VEHICLE_3_COMMANDING")]
        public bool VehicleCommanding3;
        [PLCElement(ValueName = "VEHICLE_4_COMMANDING")]
        public bool VehicleCommanding4;
        [PLCElement(ValueName = "VEHICLE_5_COMMANDING")]
        public bool VehicleCommanding5;

        [PLCElement(ValueName = "CHANGE_TO_AGV_MODE")]
        public bool ToAGVMode;
        [PLCElement(ValueName = "CHANGE_TO_MGV_MODE")]
        public bool ToMGVMode;

        [PLCElement(ValueName = "ASSIGN_BOX_ID")]
        public string AssignBoxID;
        [PLCElement(ValueName = "ASSIGN_CST_ID")]
        public string AssignCassetteID;

        [PLCElement(ValueName = "BCR_ENABLE_CMD")]
        public bool PortBCR_Enable;
    }

    class PortPLCControl_CSTID_BOXID : PLC_FunBase
    {
        public DateTime Timestamp;
        [PLCElement(ValueName = "ASSIGN_BOX_ID")]
        public string AssignBoxID;
        [PLCElement(ValueName = "ASSIGN_CST_ID")]
        public string AssignCassetteID;

    }


    class PortPLCControl_PortRunStop : PLC_FunBase
    {
        public DateTime Timestamp;

        [PLCElement(ValueName = "REMOTE_RUN")]
        public bool PortAuto;
        [PLCElement(ValueName = "REMOTE_STOP")]
        public bool PortManual;

    }
    class PortPLCControl_PortInOutModeChange : PLC_FunBase
    {
        public DateTime Timestamp;

        [PLCElement(ValueName = "CHANGE_TO_INPUT_MODE")]
        public bool PortToInputMode;
        [PLCElement(ValueName = "CHANGE_TO_OUTPUT_MODE")]
        public bool PortToOutputMode;

    }
    class PortPLCControl_VehicleCommanding1 : PLC_FunBase
    {
        public DateTime Timestamp;

        [PLCElement(ValueName = "VEHICLE_1_COMMANDING")]
        public bool VehicleCommanding1;

    }
    class PortPLCControl_AGV_OpenBOX : PLC_FunBase
    {
        public DateTime Timestamp;

        [PLCElement(ValueName = "TOGGLE_BOX_COVER")]
        public bool ToggleBoxCover;
    }
    class PortPLCControl_AGV_BCR_Read : PLC_FunBase
    {
        public DateTime Timestamp;

        [PLCElement(ValueName = "ID_READ_CMD")]
        public bool PortIDRead;
    }
    class PortPLCControl_AGV_BCR_Enable : PLC_FunBase
    {
        public DateTime Timestamp;

        [PLCElement(ValueName = "BCR_ENABLE_CMD")]
        public bool PortBCR_Enable;
    }
    class PortPLCControl_AGV_AGVmode : PLC_FunBase
    {
        public DateTime Timestamp;

        [PLCElement(ValueName = "CHANGE_TO_AGV_MODE")]
        public bool ToAGVMode;
        [PLCElement(ValueName = "CHANGE_TO_MGV_MODE")]
        public bool ToMGVMode;
    }
    class PortPLCControl_AlarmReset : PLC_FunBase
    {
        public DateTime Timestamp;

        [PLCElement(ValueName = "FAULT_RESET")]
        public bool PortReset;
    }
}
