using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc
{
    public partial class PortDef
    {
        public Timestamp Timestamp;
        public bool OpAutoMode;         //D6401.0
        public bool OpManualMode;       //D6401.1 
        public bool OpError;
        public bool IsInputMode;        //D6401.3
        public bool IsOutputMode;       //D6401.4
        public bool IsModeChangable;    //D6401.5
        public bool IsAGVMode;
        public bool IsMGVMode;
        public bool PortWaitIn;         //D6401.8
        public bool PortWaitOut;        //D6401.9
        public bool IsAutoMode;
        public bool IsReadyToLoad;      //D6401.12
        public bool IsReadyToUnload;    //D6401.13
        public bool LoadPosition1;      //D6402.0
        public bool LoadPosition2;      //D6402.1
        public bool LoadPosition3;      //D6402.2
        public bool LoadPosition4;      //D6402.3
        public bool LoadPosition5;      //D6402.4
        public bool LoadPosition6;      //D6402.C
        public bool LoadPosition7;      //D6402.B
        public bool IsCSTPresence;      //D6402.5
        public bool AGVPortReady;       //D6402.6
        public bool CanOpenBox;
        public bool IsBoxOpen;
        public bool BCRReadDone;
        public bool CSTPresenceMismatch;    //D6402.A
        public bool IsTransferComplete;
        public bool CstRemoveCheck;
        public int ErrorCode;
        public string BoxID;
        public string LoadPositionBOX1;
        public string LoadPositionBOX2;
        public string LoadPositionBOX3;
        public string LoadPositionBOX4;
        public string LoadPositionBOX5;
        public string CassetteID;
        public bool FireAlarm;
    }
}
