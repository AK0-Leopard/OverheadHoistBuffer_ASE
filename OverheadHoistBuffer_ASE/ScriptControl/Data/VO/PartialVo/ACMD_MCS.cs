//**********************************************************************************
// Date          Author         Request No.    Tag         Description
// ------------- -------------  -------------  ------      -----------------------------
// 2020/05/22    Jason Wu       N/A            A20.05.22   新增與shelfDef 相同的clone method.
// 2020/06/04    Jason Wu       N/A            A20.06.04   修改priority判定部分(由僅用priority sum大小比較 變為分組比較99 up or 99 down)   
// 2020/06/09    Jason Wu       N/A            A20.06.09.0 修改判定部分(新增判定來源目的地是非shelf的優先)   
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc
{
    public partial class ACMD_MCS
    {

        //**********************************************************************************
        //A20.05.22 給定一個私有變數去儲存2點間距離
        private int _distanceFromVehicleToHostSource;
        /// <summary>
        /// 1 2 4 8 16 32 64 128
        /// 1 1 1 1 1  1  1  1
        /// 1 0 0 0 ...
        /// 1 1 0 0 ....
        /// 1 1 1 0 ....
        /// </summary>
        public const int COMMAND_iIdle = 0;
        public const int COMMAND_STATUS_BIT_INDEX_ENROUTE = 1;
        public const int COMMAND_STATUS_BIT_INDEX_LOAD_ARRIVE = 2;
        public const int COMMAND_STATUS_BIT_INDEX_LOADING = 4;
        public const int COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE = 8;
        public const int COMMAND_STATUS_BIT_INDEX_UNLOAD_ARRIVE = 16;
        public const int COMMAND_STATUS_BIT_INDEX_UNLOADING = 32;
        public const int COMMAND_STATUS_BIT_INDEX_UNLOAD_COMPLETE = 64;
        public const int COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH = 128;

        public const int COMMAND_STATUS_BIT_INDEX_DOUBLE_STORAGE = 256;     //二重格，異常流程
        public const int COMMAND_STATUS_BIT_INDEX_EMPTY_RETRIEVAL = 512;    //空取、異常流程
        public const int COMMAND_STATUS_BIT_INDEX_InterlockError = 1024;    //交握異常
        public const int COMMAND_STATUS_BIT_INDEX_VEHICLE_ABORT = 2048;     //車子異常結束

        public const string Successful = "1";

        //**********************************************************************************
        //A20.05.22 給定呼叫該變數之method
        public int DistanceFromVehicleToHostSource
        {
            get { return _distanceFromVehicleToHostSource; }
            set { _distanceFromVehicleToHostSource = value; }
        }

        public enum CmdType
        {
            MCS,
            Manual,
            SCAN,
            OHBC,       //OHBC 自動產生的命令          
            AGVStation, //AGV 退補 BOX
            PortTypeChange,        //控制Port流向
        }

        public class ResultCode
        {
            public const string Successful = "0";
            public const string OtherErrors = "1";
            public const string ZoneIsfull = "2";
            public const string DuplicateID = "3";
            public const string IDmismatch = "4";
            public const string IDReadFailed = "5";
            public const string BoxID_ReadFailed = "6";
            public const string BoxID_Mismatch = "7";
            public const string InterlockError = "64";
        }

        public enum IDreadStatus
        {
            successful = 0,
            failed = 1,
            duplicate = 2,
            mismatch = 3,
            BoxReadFail_CstIsOK = 4,
            CSTReadFail_BoxIsOK = 5,
        }

        public static string COMMAND_STATUS_BIT_To_String(int commandStatus)
        {
            switch (commandStatus)
            {
                case COMMAND_STATUS_BIT_INDEX_ENROUTE:
                    return "Enroute";
                case COMMAND_STATUS_BIT_INDEX_LOAD_ARRIVE:
                    return "Load arrive";
                case COMMAND_STATUS_BIT_INDEX_LOADING:
                    return "Loading";
                case COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE:
                    return "Load complete";
                case COMMAND_STATUS_BIT_INDEX_UNLOAD_ARRIVE:
                    return "Unload arrive";
                case COMMAND_STATUS_BIT_INDEX_UNLOADING:
                    return "Unloading";
                case COMMAND_STATUS_BIT_INDEX_UNLOAD_COMPLETE:
                    return "Unload complete";
                case COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH:
                    return "Command finish";
            }
            return "";
        }
        //**********************************************************************************
        //A20.05.22 設定與shelfDef相同之clone
        public ACMD_MCS Clone()
        {
            return (ACMD_MCS)this.MemberwiseClone();
        }
        //*******************************************************************
        //A20.05.22 用於List sort 指令 呼叫使用
        public class MCSCmdCompare_LessThan2 : IComparer<ACMD_MCS>
        {
            public int Compare(ACMD_MCS MCSCmd1, ACMD_MCS MCSCmd2)
            {
                //A20.08.04
                // -1. 判斷目的 port 為AGV者優先
                bool isCmd1_SourceTypeAGV = MCSCmd1.IsCmdSourceTypeAGV(MCSCmd1.HOSTDESTINATION);
                bool isCmd2_SourceTypeAGV = MCSCmd1.IsCmdSourceTypeAGV(MCSCmd2.HOSTDESTINATION);

                if ((isCmd1_SourceTypeAGV == true) && (isCmd2_SourceTypeAGV == true) ||
                    (isCmd1_SourceTypeAGV == false) && (isCmd2_SourceTypeAGV == false))
                {
                    //代表兩者相等，不動，且接著判斷距離
                }
                if ((isCmd1_SourceTypeAGV == false) && (isCmd2_SourceTypeAGV == true))
                {
                    return 1;
                    //代表後者較優先，換位
                }
                if ((isCmd1_SourceTypeAGV == true) && (isCmd2_SourceTypeAGV == false))
                {
                    return -1;
                    //代表前者較優先，不動
                }

                //A20.06.09.0
                // 0.判斷命令來源是否為shelf，非shelf者優先進行。
                bool isCmd1_SourceTypeShelf = MCSCmd1.IsCmdSourceTypeShelf(MCSCmd1.HOSTSOURCE);
                bool isCmd2_SourceTypeShelf = MCSCmd1.IsCmdSourceTypeShelf(MCSCmd2.HOSTSOURCE);

                if ((isCmd1_SourceTypeShelf == true) && (isCmd2_SourceTypeShelf == true) ||
                    (isCmd1_SourceTypeShelf == false) && (isCmd2_SourceTypeShelf == false))
                {
                    //代表兩者相等，不動，且接著判斷距離
                }
                if ((isCmd1_SourceTypeShelf == true) && (isCmd2_SourceTypeShelf == false))
                {
                    return 1;
                    //代表後者較優先，換位
                }
                if ((isCmd1_SourceTypeShelf == false) && (isCmd2_SourceTypeShelf == true))
                {
                    return -1;
                    //代表前者較優先，不動
                }

                //A20.06.04
                // 1.先取priority 判斷
                if ((MCSCmd1.PRIORITY_SUM >= 99 && MCSCmd2.PRIORITY_SUM >= 99) ||
                    (MCSCmd1.PRIORITY_SUM < 99 && MCSCmd2.PRIORITY_SUM < 99))
                {
                    //代表兩者相等，不動，且接著判斷距離
                }
                if (MCSCmd1.PRIORITY_SUM < 99 && MCSCmd2.PRIORITY_SUM >= 99)
                {
                    return 1;
                    //代表後者較優先，換位
                }
                if (MCSCmd1.PRIORITY_SUM >= 99 && MCSCmd2.PRIORITY_SUM < 99)
                {
                    return -1;
                    //代表前者較優先，不動
                }

                // 2. 若priority 相同，則獲得各自 shelf 的 address 與起始 address的距離
                if (MCSCmd1.DistanceFromVehicleToHostSource == MCSCmd2.DistanceFromVehicleToHostSource)
                {
                    return 0;
                    //代表兩者相等，不動
                }
                if (MCSCmd1.DistanceFromVehicleToHostSource > MCSCmd2.DistanceFromVehicleToHostSource)
                {
                    return 1;
                    //代表後者較優先，換位
                }
                if (MCSCmd1.DistanceFromVehicleToHostSource < MCSCmd2.DistanceFromVehicleToHostSource)
                {
                    return -1;
                    //代表前者較優先，不動
                }
                return 0;
            }

        }

        public class MCSCmdCompare_MoreThan1 : IComparer<ACMD_MCS>
        {
            public int Compare(ACMD_MCS MCSCmd1, ACMD_MCS MCSCmd2)
            {
                //A20.06.09.0
                // 0.判斷命令來源是否為shelf，非shelf者優先進行。
                bool isCmd1_SourceTypeShelf = MCSCmd1.IsCmdSourceTypeShelf(MCSCmd1.HOSTSOURCE);
                bool isCmd2_SourceTypeShelf = MCSCmd1.IsCmdSourceTypeShelf(MCSCmd2.HOSTSOURCE);

                if ((isCmd1_SourceTypeShelf == true) && (isCmd2_SourceTypeShelf == true) ||
                    (isCmd1_SourceTypeShelf == false) && (isCmd2_SourceTypeShelf == false))
                {
                    //代表兩者相等，不動，且接著判斷距離
                }
                if ((isCmd1_SourceTypeShelf == true) && (isCmd2_SourceTypeShelf == false))
                {
                    return 1;
                    //代表後者較優先，換位
                }
                if ((isCmd1_SourceTypeShelf == false) && (isCmd2_SourceTypeShelf == true))
                {
                    return -1;
                    //代表前者較優先，不動
                }

                //A20.06.04
                // 1.先取priority 判斷
                if ((MCSCmd1.PRIORITY_SUM >= 99 && MCSCmd2.PRIORITY_SUM >= 99) ||
                    (MCSCmd1.PRIORITY_SUM < 99 && MCSCmd2.PRIORITY_SUM < 99))
                {
                    //代表兩者相等，不動，且接著判斷距離
                }
                if (MCSCmd1.PRIORITY_SUM < 99 && MCSCmd2.PRIORITY_SUM >= 99)
                {
                    return 1;
                    //代表後者較優先，換位
                }
                if (MCSCmd1.PRIORITY_SUM >= 99 && MCSCmd2.PRIORITY_SUM < 99)
                {
                    return -1;
                    //代表前者較優先，不動
                }

                // 2. 若priority 相同，則獲得各自 shelf 的 address 與起始 address的距離
                if (MCSCmd1.DistanceFromVehicleToHostSource == MCSCmd2.DistanceFromVehicleToHostSource)
                {
                    return 0;
                    //代表兩者相等，不動
                }
                if (MCSCmd1.DistanceFromVehicleToHostSource > MCSCmd2.DistanceFromVehicleToHostSource)
                {
                    return 1;
                    //代表後者較優先，換位
                }
                if (MCSCmd1.DistanceFromVehicleToHostSource < MCSCmd2.DistanceFromVehicleToHostSource)
                {
                    return -1;
                    //代表前者較優先，不動
                }
                return 0;
            }

        }

        public bool IsCmdSourceTypeShelf(string cmdSource)
        {
            bool isCmdSourceTypeShelf = false;
            if (cmdSource.StartsWith("10") || cmdSource.StartsWith("11") || cmdSource.StartsWith("20") || cmdSource.StartsWith("21"))
            {
                isCmdSourceTypeShelf = true;
            }
            return isCmdSourceTypeShelf;
        }

        public bool IsCmdSourceTypeAGV(string cmdSource)
        {
            bool isCmdSourceTypeAGV = false;
            if (cmdSource.Contains("A0") || cmdSource.Contains("ST0"))
            {
                isCmdSourceTypeAGV = true;
            }
            return isCmdSourceTypeAGV;
        }
    }
}
