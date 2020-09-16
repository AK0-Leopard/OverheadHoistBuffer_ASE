using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.PLC_Functions
{
    public class PortPLCInfo : PLC_FunBase
    {
        public DateTime Timestamp;

        [PLCElement(ValueName = "OP_RUN")]
        public bool OpAutoMode;         //D6401.0
        [PLCElement(ValueName = "OP_DOWN")]
        public bool OpManualMode;       //D6401.1 
        [PLCElement(ValueName = "OP_ERROR")]
        public bool OpError;

        [PLCElement(ValueName = "NOW_INPUT_MODE")]
        public bool IsInputMode;        //D6401.3
        [PLCElement(ValueName = "NOW_OUTPUT_MODE")]
        public bool IsOutputMode;       //D6401.4
        [PLCElement(ValueName = "MODE_CHANGEABLE")]
        public bool IsModeChangable;    //D6401.5

        [PLCElement(ValueName = "IS_AGV_MODE")]
        public bool IsAGVMode;
        [PLCElement(ValueName = "IS_MGV_MODE")]
        public bool IsMGVMode;

        [PLCElement(ValueName = "WAIT_IN")]
        public bool PortWaitIn;         //D6401.8
        [PLCElement(ValueName = "WAIT_OUT")]
        public bool PortWaitOut;        //D6401.9

        [PLCElement(ValueName = "IS_AUTO_MODE")]
        public bool IsAutoMode;

        [PLCElement(ValueName = "READY_TO_LOAD")]
        public bool IsReadyToLoad;      //D6401.12
        [PLCElement(ValueName = "READY_TO_UNLOAD")]
        public bool IsReadyToUnload;    //D6401.13

        [PLCElement(ValueName = "LOAD_POSITION_1")]
        public bool LoadPosition1;      //D6402.0
        [PLCElement(ValueName = "LOAD_POSITION_2")]
        public bool LoadPosition2;      //D6402.1
        [PLCElement(ValueName = "LOAD_POSITION_3")]
        public bool LoadPosition3;      //D6402.2
        [PLCElement(ValueName = "LOAD_POSITION_4")]
        public bool LoadPosition4;      //D6402.3
        [PLCElement(ValueName = "LOAD_POSITION_5")]
        public bool LoadPosition5;      //D6402.4
        [PLCElement(ValueName = "LOAD_POSITION_7")]
        public bool LoadPosition7;      //D6402.B
        [PLCElement(ValueName = "LOAD_POSITION_6")]
        public bool LoadPosition6;      //D6402.C

        [PLCElement(ValueName = "IS_CST_PRESENCE")]
        public bool IsCSTPresence;      //D6402.5
        [PLCElement(ValueName = "AGV_PORT_READY")]
        public bool AGVPortReady;       //D6402.6
        [PLCElement(ValueName = "CAN_OPEN_BOX")]
        public bool CanOpenBox;        
        [PLCElement(ValueName = "IS_BOX_OPEN")]
        public bool IsBoxOpen;

        [PLCElement(ValueName = "BARCODE_READ_DONE")]
        public bool BCRReadDone;
        [PLCElement(ValueName = "CST_PRRESENCE_MISMATCH")]
        public bool CSTPresenceMismatch;    //D6402.A
        [PLCElement(ValueName = "CST_TRANSFER_COMPLETE")]
        public bool IsTransferComplete;
        [PLCElement(ValueName = "CST_REMOVE_CHECK")]
        public bool CstRemoveCheck;

        [PLCElement(ValueName = "ERROR_CODE")]
        public UInt16 ErrorCode;

        [PLCElement(ValueName = "BOX_ID")]
        public string BoxID;

        [PLCElement(ValueName = "LOAD_POSITION_BOX_1")]
        public string LoadPositionBOX1;

        [PLCElement(ValueName = "LOAD_POSITION_BOX_2")]
        public string LoadPositionBOX2;

        [PLCElement(ValueName = "LOAD_POSITION_BOX_3")]
        public string LoadPositionBOX3;

        [PLCElement(ValueName = "LOAD_POSITION_BOX_4")]
        public string LoadPositionBOX4;

        [PLCElement(ValueName = "LOAD_POSITION_BOX_5")]
        public string LoadPositionBOX5;

        [PLCElement(ValueName = "CST_ID")]
        public string CassetteID;

        [PLCElement(ValueName = "FIRE_ALARM")]
        public bool FireAlarm;

        [PLCElement(ValueName = "CIM_ON")]
        public bool cim_on;

        [PLCElement(ValueName = "PreLoadOK")]
        public bool preLoadOK;
    }
}
