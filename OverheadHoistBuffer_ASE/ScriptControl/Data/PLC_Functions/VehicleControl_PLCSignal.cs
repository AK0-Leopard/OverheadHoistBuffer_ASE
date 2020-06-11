using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.PLC_Functions
{
    class VehicleControl_PLCSignal
    {
        public bool Bit0_DrivePowerChange;
        public bool Bit1_OperationModeChange;
        public bool Bit2_AlarmClear;
        public bool Bit3_Home;
        public bool Bit4_Spare;
        public bool Bit5_Spare;
        public bool Bit6_LowSpeed_Forward;
        public bool Bit7_HighSpeed_Forward;
        public bool Bit8_Spare;
        public bool Bit9_LowSpeed_Recede;
        public bool BitA_HighSpeed_Recede;
        public bool BitB_Spare;
        public bool BitC_LeftGuide;
        public bool BitD_RightGuide;
        public bool BitE_ServerStart;
        public bool BitF_Spare;
    }
}
