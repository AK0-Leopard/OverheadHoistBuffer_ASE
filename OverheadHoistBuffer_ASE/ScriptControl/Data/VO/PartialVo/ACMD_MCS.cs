//**********************************************************************************
// Date          Author         Request No.    Tag         Description
// ------------- -------------  -------------  ------      -----------------------------
// 2020/05/22    Jason Wu       N/A            A20.05.22   新增與shelfDef 相同的clone method.
// 2020/06/04    Jason Wu       N/A            A20.06.04   修改priority判定部分(由僅用priority sum大小比較 變為分組比較99 up or 99 down)   
// 2020/06/09    Jason Wu       N/A            A20.06.09.0 修改判定部分(新增判定來源目的地是非shelf的優先)   
//**********************************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc
{
    public partial class ACMD_MCS
    {
        //public static List<ACMD_MCS> ACMD_MCS_List = new List<ACMD_MCS>();
        public static ConcurrentDictionary<string, ACMD_MCS> MCS_CMD_InfoList { get; private set; } = new ConcurrentDictionary<string, ACMD_MCS>();

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
        public string CURRENT_LOCATION
        {
            get
            {
                string current_location = HOSTSOURCE;
                if (IsRelayHappend())
                {
                    current_location = RelayStation;
                }
                return current_location;
            }
        }

        public enum CommandReadyStatus
        {
            NotReady,
            Ready,
            Realy
        }
        public enum NotReadyReason
        {
            None,
            Ready,
            SpeciallyProcess,
            NoCSTData,
            SourceNotReady,
            DestZoneIsFull,
            DestAGVZoneNotReady,
            DestAGVZoneNotReadyWillRealy,
            DestPortNotReady,
            DestPoerNotReadyWillRealy,
            ExceptionHappend,
            HosStateNotAuto
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
        public static List<ACMD_MCS> loadReadyTransferOfQueueCMD_MCS()
        {
            if (MCS_CMD_InfoList == null)
            {
                return new List<ACMD_MCS>();
            }
            var ready_transfer_cmd_mcs = MCS_CMD_InfoList.Values.
                                         Where(cmd => (cmd.ReadyStatus == CommandReadyStatus.Ready || cmd.ReadyStatus == CommandReadyStatus.Realy) &&
                                                       cmd.TRANSFERSTATE == E_TRAN_STATUS.Queue).ToList();
            return ready_transfer_cmd_mcs;
        }
        public static List<ACMD_MCS> loadTransferingAndBeforeLoadingCMD_MCS()
        {
            if (MCS_CMD_InfoList == null)
            {
                return new List<ACMD_MCS>();
            }
            var ready_transfer_cmd_mcs = MCS_CMD_InfoList.Values.
                                         Where(cmd => cmd.TRANSFERSTATE == E_TRAN_STATUS.Transferring &&
                                                      cmd.COMMANDSTATE < ACMD_MCS.COMMAND_STATUS_BIT_INDEX_LOADING).ToList();
            return ready_transfer_cmd_mcs;
        }
        public static List<ACMD_MCS> loadCurrentExcuteCMD_MCS()
        {
            if (MCS_CMD_InfoList == null)
            {
                return new List<ACMD_MCS>();
            }
            return MCS_CMD_InfoList.Values.ToList();
        }

        public ACMD_OHTC convertToACMD_OHTC(
            AVEHICLE assignVehicle,
            BLL.Interface.IPortDefBLL portDefBLL,
            BLL.Interface.ISequenceBLL sequenceBLL,
            Service.Interface.ITransferService transferService,
            string relayStation)
        {
            if (IsScan())
            {
                var port_def = portDefBLL.getPortDef(CURRENT_LOCATION);
                return new ACMD_OHTC()
                {
                    CMD_ID = sequenceBLL.getCommandID(App.SCAppConstants.GenOHxCCommandType.Auto),
                    CARRIER_ID = this.CARRIER_ID,
                    BOX_ID = this.BOX_ID,
                    VH_ID = assignVehicle.VEHICLE_ID.Trim(),
                    CMD_ID_MCS = this.CMD_ID,
                    CMD_TPYE = E_CMD_TYPE.Scan,
                    PRIORITY = 50,
                    SOURCE = this.CURRENT_LOCATION,
                    DESTINATION = this.HOSTDESTINATION,
                    CMD_STAUS = 0,
                    CMD_PROGRESS = 0,
                    ESTIMATED_EXCESS_TIME = 0,
                    REAL_CMP_TIME = 0,
                    ESTIMATED_TIME = 50,
                    SOURCE_ADR = port_def.ADR_ID,
                    DESTINATION_ADR = port_def.ADR_ID
                };
            }
            else
            {
                E_CMD_TYPE cmd_type = default(E_CMD_TYPE);
                bool is_source_vh = transferService.isUnitType(CURRENT_LOCATION, Service.UnitType.CRANE);
                string _source_address = string.Empty;
                string _source = string.Empty;


                if (is_source_vh)
                {
                    cmd_type = E_CMD_TYPE.Unload;
                }
                else
                {
                    cmd_type = E_CMD_TYPE.LoadUnload;
                    var port_def = portDefBLL.getPortDef(CURRENT_LOCATION);
                    _source_address = port_def.ADR_ID;
                    _source = CURRENT_LOCATION;
                }

                string real_dest_port = HOSTDESTINATION;
                if (ReadyStatus == CommandReadyStatus.Realy)
                {
                    //real_dest_port = RelayStation;
                    real_dest_port = relayStation;
                }
                var dest_port_def = portDefBLL.getPortDef(real_dest_port);


                return new ACMD_OHTC
                {
                    CMD_ID = sequenceBLL.getCommandID(App.SCAppConstants.GenOHxCCommandType.Auto),
                    VH_ID = assignVehicle.VEHICLE_ID.Trim(),
                    CARRIER_ID = this.CARRIER_ID,
                    CMD_ID_MCS = this.CMD_ID,
                    CMD_TPYE = cmd_type,
                    SOURCE = _source,
                    DESTINATION = real_dest_port,
                    PRIORITY = this.PRIORITY,
                    CMD_STAUS = E_CMD_STATUS.Queue,
                    CMD_PROGRESS = 0,
                    ESTIMATED_TIME = 0,
                    ESTIMATED_EXCESS_TIME = 0,
                    SOURCE_ADR = _source_address,
                    DESTINATION_ADR = dest_port_def.ADR_ID,
                    BOX_ID = this.BOX_ID,
                    LOT_ID = this.LOT_ID
                };
            }
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

        //public bool IsReadyTransfer;
        public CommandReadyStatus ReadyStatus;
        public NotReadyReason ReadyReason = NotReadyReason.None;

        public bool IsScan()
        {
            return CMD_ID.Contains("SCAN");
        }
        public bool IsRelayHappend()
        {
            return !sc.Common.SCUtility.isEmpty(RelayStation);
        }
        public bool IsSource_ShelfPort(Service.Interface.ITransferService transferService)
        {
            //return transferService.isUnitType(HOSTSOURCE, Service.UnitType.SHELF); ;
            return transferService.isUnitType(CURRENT_LOCATION, Service.UnitType.SHELF); ;
        }

        public bool IsSource_CRANE(Service.Interface.ITransferService transferService)
        {
            //return transferService.isUnitType(HOSTSOURCE, Service.UnitType.CRANE); ;
            return transferService.isUnitType(CURRENT_LOCATION, Service.UnitType.CRANE); ;
        }

        public bool IsDestination_ShelfZone(Service.Interface.ITransferService transferService)
        {
            return transferService.isUnitType(HOSTDESTINATION, Service.UnitType.ZONE); ;
        }
        public bool IsDestination_ShelfPort(Service.Interface.ITransferService transferService)
        {
            return transferService.isUnitType(HOSTDESTINATION, Service.UnitType.SHELF); ;
        }
        public bool IsDestination_AGVZone(Service.Interface.ITransferService transferService)
        {
            return transferService.isUnitType(HOSTDESTINATION, Service.UnitType.AGVZONE); ;
        }
        public bool IsDestination_CVPort(Service.Interface.ITransferService transferService)
        {
            //return transferService.isCVPort(HOSTDESTINATION);
            return transferService.isCVPort(CURRENT_LOCATION);
        }
        public bool IsTimeOutToAlternate()
        {
            TimeSpan timeSpan = DateTime.Now - this.CMD_INSER_TIME;
            return timeSpan.TotalSeconds > App.SystemParameter.cmdTimeOutToAlternate;
        }
        public double getHostSourceAxis_X(BLL.Interface.IPortDefBLL portDefBLL, BLL.Interface.IReserveBLL reserveBLL)
        {
            var port_def = portDefBLL.getPortDef(CURRENT_LOCATION);
            if (port_def == null)
            {
                NLog.LogManager.GetCurrentClassLogger().Warn($"port id:{CURRENT_LOCATION},obj [PortDef] no define");
                return double.MaxValue;
            }
            var adr_axis = reserveBLL.GetHltMapAddress(port_def.ADR_ID);
            if (!adr_axis.isExist)
            {
                NLog.LogManager.GetCurrentClassLogger().Warn($"port id:{CURRENT_LOCATION} adr id:{port_def.ADR_ID},obj [HltMapAddress] no define");
                return double.MaxValue;
            }
            return adr_axis.x;
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

        public HCMD_MCS ToHCMD_MCS()
        {
            return new HCMD_MCS()
            {
                CMD_ID = this.CMD_ID,
                CARRIER_ID = this.CARRIER_ID,
                TRANSFERSTATE = this.TRANSFERSTATE,
                COMMANDSTATE = this.COMMANDSTATE,
                HOSTSOURCE = this.HOSTSOURCE,
                HOSTDESTINATION = this.HOSTDESTINATION,
                PRIORITY = this.PRIORITY,
                CHECKCODE = this.CHECKCODE,
                PAUSEFLAG = this.PAUSEFLAG,
                CMD_INSER_TIME = this.CMD_INSER_TIME,
                CMD_START_TIME = this.CMD_START_TIME,
                CMD_FINISH_TIME = this.CMD_FINISH_TIME,
                TIME_PRIORITY = this.TIME_PRIORITY,
                PORT_PRIORITY = this.PORT_PRIORITY,
                PRIORITY_SUM = this.PRIORITY_SUM,
                REPLACE = this.REPLACE,
                BOX_ID = this.BOX_ID,
                CARRIER_LOC = this.CARRIER_LOC,
                LOT_ID = this.LOT_ID,
                CARRIER_ID_ON_CRANE = this.CARRIER_ID_ON_CRANE,
                CMDTYPE = this.CMDTYPE,
                CRANE = this.CRANE,
                RelayStation = this.RelayStation,
            };
        }

        internal bool put(ACMD_MCS ortherObj)
        {
            bool has_change = false;
            if (!sc.Common.SCUtility.isMatche(CMD_ID, ortherObj.CMD_ID))
            {
                CMD_ID = ortherObj.CMD_ID;
                has_change = true;
            }
            if (!sc.Common.SCUtility.isMatche(CARRIER_ID, ortherObj.CARRIER_ID))
            {
                BOX_ID = ortherObj.CARRIER_ID;
                has_change = true;
            }
            if (TRANSFERSTATE != ortherObj.TRANSFERSTATE)
            {
                TRANSFERSTATE = ortherObj.TRANSFERSTATE;
                has_change = true;
            }
            if (COMMANDSTATE != ortherObj.COMMANDSTATE)
            {
                COMMANDSTATE = ortherObj.COMMANDSTATE;
                has_change = true;
            }
            if (!sc.Common.SCUtility.isMatche(HOSTSOURCE, ortherObj.HOSTSOURCE))
            {
                HOSTSOURCE = ortherObj.HOSTSOURCE;
                has_change = true;
            }
            if (!sc.Common.SCUtility.isMatche(HOSTDESTINATION, ortherObj.HOSTDESTINATION))
            {
                HOSTDESTINATION = ortherObj.HOSTDESTINATION;
                has_change = true;
            }
            if (PRIORITY != ortherObj.PRIORITY)
            {
                PRIORITY = ortherObj.PRIORITY;
                has_change = true;
            }
            if (!sc.Common.SCUtility.isMatche(CHECKCODE, ortherObj.CHECKCODE))
            {
                CHECKCODE = ortherObj.CHECKCODE;
                has_change = true;
            }
            if (!sc.Common.SCUtility.isMatche(PAUSEFLAG, ortherObj.PAUSEFLAG))
            {
                PAUSEFLAG = ortherObj.PAUSEFLAG;
                has_change = true;
            }
            if (CMD_INSER_TIME != ortherObj.CMD_INSER_TIME)
            {
                CMD_INSER_TIME = ortherObj.CMD_INSER_TIME;
                has_change = true;
            }
            if (CMD_START_TIME != ortherObj.CMD_START_TIME)
            {
                CMD_START_TIME = ortherObj.CMD_START_TIME;
                has_change = true;
            }
            if (CMD_FINISH_TIME != ortherObj.CMD_FINISH_TIME)
            {
                CMD_FINISH_TIME = ortherObj.CMD_FINISH_TIME;
                has_change = true;
            }
            if (TIME_PRIORITY != ortherObj.TIME_PRIORITY)
            {
                TIME_PRIORITY = ortherObj.TIME_PRIORITY;
                has_change = true;
            }
            if (PORT_PRIORITY != ortherObj.PORT_PRIORITY)
            {
                PORT_PRIORITY = ortherObj.PORT_PRIORITY;
                has_change = true;
            }
            if (REPLACE != ortherObj.REPLACE)
            {
                REPLACE = ortherObj.REPLACE;
                has_change = true;
            }

            if (PRIORITY_SUM != ortherObj.PRIORITY_SUM)
            {
                PRIORITY_SUM = ortherObj.PRIORITY_SUM;
                has_change = true;
            }

            if (!sc.Common.SCUtility.isMatche(BOX_ID, ortherObj.BOX_ID))
            {
                BOX_ID = ortherObj.BOX_ID;
                has_change = true;
            }
            if (!sc.Common.SCUtility.isMatche(CARRIER_LOC, ortherObj.CARRIER_LOC))
            {
                CARRIER_LOC = ortherObj.CARRIER_LOC;
                has_change = true;
            }
            if (!sc.Common.SCUtility.isMatche(LOT_ID, ortherObj.LOT_ID))
            {
                LOT_ID = ortherObj.LOT_ID;
                has_change = true;
            }
            if (!sc.Common.SCUtility.isMatche(CARRIER_ID_ON_CRANE, ortherObj.CARRIER_ID_ON_CRANE))
            {
                CARRIER_ID_ON_CRANE = ortherObj.CARRIER_ID_ON_CRANE;
                has_change = true;
            }
            if (!sc.Common.SCUtility.isMatche(CMDTYPE, ortherObj.CMDTYPE))
            {
                CMDTYPE = ortherObj.CMDTYPE;
                has_change = true;
            }
            if (!sc.Common.SCUtility.isMatche(CRANE, ortherObj.CRANE))
            {
                CRANE = ortherObj.CRANE;
                has_change = true;
            }
            if (!sc.Common.SCUtility.isMatche(RelayStation, ortherObj.RelayStation))
            {
                RelayStation = ortherObj.RelayStation;
                has_change = true;
            }
            if (ReadyStatus != ortherObj.ReadyStatus)
            {
                ReadyStatus = ortherObj.ReadyStatus;
                has_change = true;
            }

            if (ReadyReason != ortherObj.ReadyReason)
            {
                ReadyReason = ortherObj.ReadyReason;
                has_change = true;
            }

            return has_change;
        }
    }
}
