// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="SECSConst.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common.SECS;

namespace com.mirle.ibg3k0.sc.Data.SECS.ASE
{
    /// <summary>
    /// Class SECSConst.
    /// </summary>
    public class SECSConst
    {
        #region Sample Flag
        /// <summary>
        /// The smplfla g_ selected
        /// </summary>
        public static readonly string SMPLFLAG_Selected = "Y";
        /// <summary>
        /// The smplfla g_ not_ selected
        /// </summary>
        public static readonly string SMPLFLAG_Not_Selected = "N";
        #endregion Sample Flag

        #region Host Control Mode
        public static readonly string HostCrtMode_EQ_Off_line = "1";
        public static readonly string HostCrtMode_Going_Online = "2";
        public static readonly string HostCrtMode_Host_Online = "3";
        public static readonly string HostCrtMode_On_Line_Local = "4";
        public static readonly string HostCrtMode_On_Line_Remote = "5";

        public static readonly string[] HOST_CRT_MODE =
        {
            HostCrtMode_EQ_Off_line,
            HostCrtMode_Going_Online,
            HostCrtMode_Host_Online,
            HostCrtMode_On_Line_Local,
            HostCrtMode_On_Line_Remote
        };
        #endregion Host Control Mode

        #region VEHICLE COMMUNICATION STATE
        public static readonly string VEHICLE_COMMUNICATION_STATE_Disconnected = "0";
        public static readonly string VEHICLE_COMMUNICATION_STATE_Communicating = "1";
        public static readonly string VEHICLE_COMMUNICATION_STATE_NotCommunicating = "2";
        #endregion VEHICLE COMMUNICATION STATE
        #region MAINTE STATE
        public static readonly string MAINTE_STATE_Undefined = "0";
        public static readonly string MAINTE_STATE_Mainteance = "1";
        public static readonly string MAINTE_STATE_NotMainteance = "2";
        #endregion MAINTE STATE


        #region Vehicle State
        public static readonly string VHSTATE_NotRelated = "0";
        public static readonly string VHSTATE_Removed = "1";
        public static readonly string VHSTATE_NotAssigned = "2";
        public static readonly string VHSTATE_Enroute = "3";
        public static readonly string VHSTATE_Parked = "4";
        public static readonly string VHSTATE_Acquiring = "5";
        public static readonly string VHSTATE_Depositiong = "6";
        #endregion Vehicle State

        #region TRSMODE
        /// <summary>
        /// The trsmod e_ automatic
        /// </summary>
        public static readonly string TRSMODE_AUTO = "1";
        /// <summary>
        /// The trsmod e_ manual
        /// </summary>
        public static readonly string TRSMODE_Manual = "2";
        #endregion TRSMODE

        #region PPTYPE
        /// <summary>
        /// The pptyp e_ equipment
        /// </summary>
        public static readonly string PPTYPE_Equipment = "E";
        /// <summary>
        /// The pptyp e_ unit
        /// </summary>
        public static readonly string PPTYPE_Unit = "U";
        /// <summary>
        /// The pptyp e_ sub_ unit
        /// </summary>
        public static readonly string PPTYPE_Sub_Unit = "S";
        #endregion PPTYPE

        #region ONLACK
        public static readonly string ONLACK_Accepted = "0";
        public static readonly string ONLACK_Not_Accepted = "1";
        public static readonly string ONLACK_Equipment_Already_On_Line = "2";
        #endregion ONLACK

        #region SFCD
        /// <summary>
        /// The SFC d_ module_ status_ request
        /// </summary>
        public static readonly string SFCD_Module_Status_Request = "1";
        /// <summary>
        /// The SFC d_ port_ status_ request
        /// </summary>
        public static readonly string SFCD_Port_Status_Request = "2";
        /// <summary>
        /// The SFC d_ reticle_ status_ request
        /// </summary>
        public static readonly string SFCD_Reticle_Status_Request = "3";
        /// <summary>
        /// The SFC d_ unit_ status_ request
        /// </summary>
        public static readonly string SFCD_Unit_Status_Request = "4";
        /// <summary>
        /// The SFC d_ sub_ unit_ status_ request
        /// </summary>
        public static readonly string SFCD_Sub_Unit_Status_Request = "5";
        /// <summary>
        /// The SFC d_ mask_ status_ request
        /// </summary>
        public static readonly string SFCD_Mask_Status_Request = "6";
        /// <summary>
        /// The SFC d_ material_ status_ request
        /// </summary>
        public static readonly string SFCD_Material_Status_Request = "7";
        /// <summary>
        /// The SFC d_ sorter_ job_ list_ request
        /// </summary>
        public static readonly string SFCD_Sorter_Job_List_Request = "8";
        /// <summary>
        /// The SFC d_ crate_ port_ status_ request
        /// </summary>
        public static readonly string SFCD_Crate_Port_Status_Request = "9";
        /// <summary>
        /// The SFC d_ finish
        /// </summary>
        public static readonly string SFCD_Finish = "F";
        #endregion SFCD

        #region CIACK
        /// <summary>
        /// The ciac k_ accepted
        /// </summary>
        public static readonly string CIACK_Accepted = "0";
        /// <summary>
        /// The ciac k_ busy
        /// </summary>
        public static readonly string CIACK_Busy = "1";
        /// <summary>
        /// The ciac k_ csti d_is_ invalid
        /// </summary>
        public static readonly string CIACK_CSTID_is_Invalid = "2";
        /// <summary>
        /// The ciac k_ ppi d_is_ invalid
        /// </summary>
        public static readonly string CIACK_PPID_is_Invalid = "3";
        /// <summary>
        /// The ciac k_ slo t_ information_ mismatch
        /// </summary>
        public static readonly string CIACK_SLOT_Information_Mismatch = "4";
        /// <summary>
        /// The ciac k_ already_ received
        /// </summary>
        public static readonly string CIACK_Already_Received = "5";
        /// <summary>
        /// The ciac k_ pai r_ lo t_ mismatch
        /// </summary>
        public static readonly string CIACK_PAIR_LOT_Mismatch = "6";
        /// <summary>
        /// The ciac k_ pro d_ i d_ invalid
        /// </summary>
        public static readonly string CIACK_PROD_ID_Invalid = "7";
        /// <summary>
        /// The ciac k_ glass_ type_ invalid
        /// </summary>
        public static readonly string CIACK_Glass_Type_Invalid = "8";
        /// <summary>
        /// The ciac k_ other_ error
        /// </summary>
        public static readonly string CIACK_Other_Error = "9";
        #endregion CIACK

        #region RCMD
        public const string RCMD_Resume = "RESUME";
        public const string RCMD_Cancel = "CANCEL";
        public const string RCMD_Pause = "PAUSE";
        public const string RCMD_Scan = "SCAN";
        public const string RCMD_PriorityUpdate = "PRIORITYUPDATE";
        public const string RCMD_PortTypeChange = "PORTTYPECHG";
        public const string RCMD_Abort = "ABORT";
        public const string RCMD_Retry = "RETRY";
        public const string RCMD_Install = "INSTALL";
        public const string RCMD_Remove = "REMOVE";
        public const string RCMD_DisableShelf = "DISABLESHELF";
        public const string RCMD_EnbleShelf = "ENABLESHELF";
        public const string RCMD_ReName = "RENAME";
        public const string RCMD_CARRIERLOTIDUPDATE = "CARRIERLOTIDUPDATE";
        //public const string RCMD_TransferEXt = "TRANSFEREXT";
        //public const string RCMD_StageDelete = "STAGEDELETE";
        //public const string RCMD_ConfirmRoute = "CONFIRMROUTE";
        #endregion RCMD
        #region CPNAME
        public const string CPNAME_CommandID = "COMMANDID";
        public const string CPNAME_CarrierID = "CARRIERID";
        public const string CPNAME_BoxID = "BOXID";
        public const string CPNAME_CarrierLoc = "CARRIERLOC";
        public const string CPNAME_Priority = "PRIORITY";
        public const string CPNAME_PortID = "PORTID";
        public const string CPNAME_PortunitType = "PORTUNITTYPE";
        public const string CPNAME_ErrorID = "ERRORID";
        public const string CPNAME_ShelfID = "SHELFID";
        public const string CPNAME_LotID = "LOT_ID";

        public const string CPNAME_SourcePort = "SOURCEPORT";
        public const string CPNAME_DestPort = "DESTPORT";
        #endregion CPNAME

        #region HCACK
        public static readonly string HCACK_Confirm_Executed = "0";
        public static readonly string HCACK_Command_Not_Exist = "1";
        public static readonly string HCACK_Not_Able_Execute = "2";
        public static readonly string HCACK_Param_Invalid = "3";
        public static readonly string HCACK_Confirm = "4";
        public static readonly string HCACK_Rejected_Already_Requested = "5";
        public static readonly string HCACK_Obj_Not_Exist = "6";
        //public static readonly string HCACK_Enabled_Route_Does_Not_Exist = "7";
        #endregion HCACK

        #region CMD Result Code
        public const string CMD_Result_Successful = "0";
        public const string CMD_Result_Unsuccessful = "1";
        public const string CMD_Result_IDDuplicate = "4";
        public const string CMD_Result_IDMismatch = "5";
        public const string CMD_Result_IDReadFailed = "6";
        public const string CMD_Result_TransferCanceled = "8";
        public const string CMD_Result_TransferAborted = "9";
        //public const string CMD_Result_InterlockError = "64";
        public static string convert2MCS(ProtocolFormat.OHTMessage.CompleteStatus tran_cmp_status)
        {
            switch (tran_cmp_status)
            {
                case ProtocolFormat.OHTMessage.CompleteStatus.CmpStatusMove:
                case ProtocolFormat.OHTMessage.CompleteStatus.CmpStatusScan:
                case ProtocolFormat.OHTMessage.CompleteStatus.CmpStatusLoad:
                case ProtocolFormat.OHTMessage.CompleteStatus.CmpStatusUnload:
                case ProtocolFormat.OHTMessage.CompleteStatus.CmpStatusLoadunload:
                case ProtocolFormat.OHTMessage.CompleteStatus.CmpStatusHome:
                case ProtocolFormat.OHTMessage.CompleteStatus.CmpStatusOverride:
                case ProtocolFormat.OHTMessage.CompleteStatus.CmpStatusCstIdrenmae:
                case ProtocolFormat.OHTMessage.CompleteStatus.CmpStatusMtlhome:
                case ProtocolFormat.OHTMessage.CompleteStatus.CmpStatusMoveToMtl:
                case ProtocolFormat.OHTMessage.CompleteStatus.CmpStatusSystemOut:
                case ProtocolFormat.OHTMessage.CompleteStatus.CmpStatusSystemIn:
                case ProtocolFormat.OHTMessage.CompleteStatus.CmpStatusTechingMove:
                    return CMD_Result_Successful;
                case ProtocolFormat.OHTMessage.CompleteStatus.CmpStatusCancel:
                case ProtocolFormat.OHTMessage.CompleteStatus.CmpStatusAbort:
                case ProtocolFormat.OHTMessage.CompleteStatus.CmpStatusVehicleAbort:
                case ProtocolFormat.OHTMessage.CompleteStatus.CmpStatusInterlockError:
                    return CMD_Result_Unsuccessful;
                //return CMD_Result_InterlockError;
                case ProtocolFormat.OHTMessage.CompleteStatus.CmpStatusIdmisMatch:
                    return CMD_Result_IDMismatch;
                case ProtocolFormat.OHTMessage.CompleteStatus.CmpStatusIdreadFailed:
                    return CMD_Result_IDReadFailed;
                case ProtocolFormat.OHTMessage.CompleteStatus.CmpStatusIdreadDuplicate:
                    return CMD_Result_IDDuplicate;
                default:
                    throw new Exception("參數錯誤"); //TODO 要帶入正確的Exception。
            }
        }
        #endregion CMD Result Code
        #region ACK
        /// <summary>
        /// The ac k_ accepted
        /// </summary>
        public static readonly string ACK_Accepted = "0";
        /// <summary>
        /// The ac k_ not_ accepted
        /// </summary>
        public static readonly string ACK_Not_Accepted = "1";
        #endregion ACK

        #region Pause Reason
        public const string PAUSE_REASON_MCSRequest = "0";
        public const string PAUSE_REASON_OP = "1";
        public const string PAUSE_REASON_PM = "2";
        public const string PAUSE_REASON_ErrorRecovery = "3";
        public const string PAUSE_REASON_Othert = "9";
        #endregion Pause Reason


        #region CEID
        /// <summary>
        /// 用來代表所有的CEID（於Enable、Disable All CEID時會使用到）。
        /// </summary>
        //public const string CEID_ALL_CEID = "000";
        //CEID Control Related Events
        public const string CEID_Equipment_OFF_LINE = "001";
        public const string CEID_Control_Status_Local = "002";
        public const string CEID_Control_Status_Remote = "003";
        //SC Transition Events
        public const string CEID_Alarm_Cleared = "051";
        public const string CEID_Alarm_Set = "052";
        public const string CEID_TSC_Auto_Completed = "053";
        public const string CEID_TSC_Auto_Initiated = "054";
        public const string CEID_TSC_Pause_Completed = "055";
        public const string CEID_TSC_Paused = "056";
        public const string CEID_TSC_Pause_Initiated = "057";
        //Transfer Command Status Transition Events
        public const string CEID_Transfer_Abort_Completed = "101";
        public const string CEID_Transfer_Abort_Failed = "102";
        public const string CEID_Transfer_Abort_Initiated = "103";
        public const string CEID_Transfer_Cancel_Completed = "104";
        public const string CEID_Transfer_Cancel_Failed = "105";
        public const string CEID_Transfer_Cancel_Initiated = "106";
        public const string CEID_Transfer_Completed = "107";
        public const string CEID_Transfer_Initiated = "108";
        public const string CEID_Transfer_Pause = "109";
        public const string CEID_Transfer_Resumed = "110";
        public const string CEID_Carrier_Transferring = "111";
        //Carrier Status Transition Events (CEID 151 ~ 152)
        public const string CEID_Carrier_Installed_Completed = "151";
        public const string CEID_Carrier_Removed_Completed = "152";
        //OHBC
        public const string CEID_Carrier_Removed_Port = "153";
        public const string CEID_Carrier_Resumed = "154";
        public const string CEID_Carrier_Stored = "155";
        public const string CEID_Carrier_Stored_Alt = "156";
        public const string CEID_Shelf_Status_Change = "157";
        public const string CEID_Carrier_Wait_In = "158";
        public const string CEID_Carrier_Wait_Out = "161";
        public const string CEID_Unit_Alarm_Set = "162";
        public const string CEID_Unit_Alarm_Cleared = "163";
        //
        public const string CEID_Crane_Active  = "201";
        public const string CEID_Crane_Idle  = "202";
        public const string CEID_Crane_In_Escape = "203";
        public const string CEID_Crane_Out_Escape = "204";
        public const string CEID_Crane_Out_Servce = "205";
        public const string CEID_Crane_In_Servce = "206";
        //
        public const string CEID_Carrier_ID_Read = "251";
        public const string CEID_Zone_Capacity_Change = "252";
        public const string CEID_Carrier_Box_ID_Rename = "253";
        public const string CEID_Operator_Initiated_Action = "254";
        public const string CEID_Empty_Box_Supply  = "255";
        public const string CEID_Empty_Box_Recycling = "256";
        //
        public const string CEID_Port_Out_Of_Service = "260";
        public const string CEID_Port_In_Service = "261";
        public const string CEID_QueryLotID = "264";
        //
        public const string CEID_Load_Req = "602";
        public const string CEID_Unload_Req = "603";
        public const string CEID_No_Req = "604";
        //Port Type
        public const string CEID_Port_Type_Input = "701";
        public const string CEID_Port_Type_Output = "702";
        public const string CEID_Port_Type_Changing = "703";
        //public const string CEID_Establish_Interval_Time_Change = "011";
        //public const string CEID_Unit_Alarm_Cleared = "503";
        //public const string CEID_Unit_Alarm_Set = "504";

        //public const string CEID_LaneInService = "570";
        //public const string CEID_LaneOutOfService = "571";


        //CEID Remark End
        #region CEID Array

        #region VID
        //OHBC
        public const string VID_Enhanced_Carrier_Info = "10";
        public const string VID_Command_Info = "11";
        public const string VID_Install_Time = "12";
        public const string VID_Enhanced_Transfer_Cmd = "13";
        public const string VID_Empty_Carrier = "15";

        public const string VID_Carrier_ID = "54";
        public const string VID_Carrier_Info = "55";
        public const string VID_Carrier_Loc = "56";
        public const string VID_Command_ID = "58";

        public const string VID_Dest_Port = "60";
        public const string VID_Eqp_Name = "61";
        public const string VID_Priority = "62";
        public const string VID_Replace = "63";
        public const string VID_Result_Code = "64";
        public const string VID_Source_ID = "65";
        public const string VID_Handoff_Type = "66";
        public const string VID_IDread_Status = "67";
        public const string VID_Recoery_Option = "68";

        public const string VID_Crane_ID = "70";
        public const string VID_Unit_Info = "72";
        public const string VID_Unit_State = "74";

        public const string VID_Command_Type= "80";
        public const string VID_Alarm_ID = "81";
        public const string VID_Alarm_Text = "82";
        public const string VID_Unit_ID = "83";
        public const string VID_Error_Number = "84";

        public const string VID_Spec_Version = "114";
        public const string VID_Port_ID = "115";
        public const string VID_Port_Type = "116";

        public const string VID_Zone_Data = "172";
        //public const string VID_Zone_Name = "173";
        public const string VID_Zone_Capacity = "174";
        public const string VID_Zone_Total_Size = "175";
        public const string VID_Zone_Type = "176";
        public const string VID_Enhanced_ZoneData = "177";
        public const string VID_BOX_ID = "179";

        public const string VID_Carrier_State = "203";

        public const string VID_Eq_Req_Satus = "352";
        public const string VID_Eq_Presence_Status = "353";
        public const string VID_Port_Info = "354";
        public const string VID_Port_TransferState = "355";
        public const string VID_Eq_Port_Info = "356";

        public const string VID_Unit_Alarm_Info = "361";
        public const string VID_Maint_State = "362";

        public const string VID_Carrier_Zone_Name = "370";

        public const string VID_Port_Type_Info = "601";
        public const string VID_Port_Unit_Type = "602";

        public const string VID_Transfer_Info = "720";
        public const string VID_Transfer_State = "722";

        public const string VID_Carrier_Locations = "886";
        public const string VID_Disabled_Locations = "888";
        public const string VID_Disabled_Loc = "889";
        public const string VID_Request_Count = "890";
        public const string VID_Crane_Current_Position = "891";
        public const string VID_Crane_Total_Distance = "892";
        public const string VID_Monitored_CraneInfo = "893";
        //-------Synchronize---------
        public const string VID_Alarms_Set = "4";
        public const string VID_Control_State = "6";
        public const string VID_Enhanced_Carriers = "51";
        public const string VID_SC_State = "73";
        public const string VID_Enhanced_Transfers = "76";
        //public const string VID_Carrier_Locations = "94";
        public const string VID_Enhanced_Active_Zones = "107";
        public const string VID_Current_Port_States = "118";
        public const string VID_CurrEq_Port_Status = "350";
        public const string VID_Port_Types = "351";
        //public const string VID_Enhanced_Zone_Datas = "352";
        public const string VID_Unit_Alarm_List = "360";



        #endregion VID



        public static readonly string[] CEID_ARRAY =
        {
             CEID_Equipment_OFF_LINE,
             CEID_Control_Status_Local,
             CEID_Control_Status_Remote,

             CEID_Alarm_Cleared ,
             CEID_Alarm_Set,
             CEID_TSC_Auto_Completed ,
             CEID_TSC_Auto_Initiated,
             CEID_TSC_Pause_Completed,
             CEID_TSC_Paused,
             CEID_TSC_Pause_Initiated,

             CEID_Transfer_Abort_Completed,
             CEID_Transfer_Abort_Failed,
             CEID_Transfer_Abort_Initiated,
             CEID_Transfer_Cancel_Completed,
             CEID_Transfer_Cancel_Failed,
             CEID_Transfer_Cancel_Initiated,
             CEID_Transfer_Completed ,
             CEID_Transfer_Initiated,
             CEID_Transfer_Pause,
             CEID_Transfer_Resumed ,
             CEID_Carrier_Transferring ,

             CEID_Carrier_Installed_Completed,
             CEID_Carrier_Removed_Completed,

             CEID_Carrier_Removed_Port,
             CEID_Carrier_Resumed,
             CEID_Carrier_Stored,
             CEID_Carrier_Stored_Alt,
             CEID_Shelf_Status_Change,
             CEID_Carrier_Wait_In ,
             CEID_Carrier_Wait_Out,
             CEID_Unit_Alarm_Set,
             CEID_Unit_Alarm_Cleared,

             CEID_Crane_Active,
             CEID_Crane_Idle,
             CEID_Crane_In_Escape ,
             CEID_Crane_Out_Escape ,
             CEID_Crane_Out_Servce,
             CEID_Crane_In_Servce,

             CEID_Carrier_ID_Read,
             CEID_Zone_Capacity_Change,
             CEID_Carrier_Box_ID_Rename,
             CEID_Operator_Initiated_Action,
             CEID_Empty_Box_Supply,
             CEID_Empty_Box_Recycling,


             CEID_Port_Out_Of_Service,
             CEID_Port_In_Service,

             CEID_Load_Req,
             CEID_Unload_Req,
             CEID_No_Req,

             CEID_Port_Type_Input,
             CEID_Port_Type_Output,
             CEID_Port_Type_Changing
        };
        public static Dictionary<string, string> CEID_Dictionary = new Dictionary<string, string>()
        {
            {CEID_Equipment_OFF_LINE,"Equipment_OFF_LINE" },
            {CEID_Control_Status_Local,"Equipment_OFF_LINE" },
            {CEID_Control_Status_Remote,"Control_Status_Remote" },
            {CEID_Alarm_Cleared,"Alarm_Cleared" },
            {CEID_Alarm_Set,"Alarm_Set" },
            {CEID_TSC_Auto_Completed,"TSC_Auto_Completed" },
            {CEID_TSC_Auto_Initiated,"TSC_Auto_Initiated" },
            {CEID_TSC_Pause_Completed,"TSC_Pause_Completed" },
            {CEID_TSC_Paused,"TSC_Paused" },
            {CEID_TSC_Pause_Initiated,"TSC_Pause_Initiated" },
            {CEID_Transfer_Abort_Completed,"Transfer_Abort_Completed" },
            {CEID_Transfer_Abort_Failed,"Transfer_Abort_Failed" },
            {CEID_Transfer_Abort_Initiated,"Transfer_Abort_Initiated" },
            {CEID_Transfer_Cancel_Completed,"Transfer_Cancel_Completed" },
            {CEID_Transfer_Cancel_Failed,"Transfer_Cancel_Failed" },
            {CEID_Transfer_Cancel_Initiated,"Transfer_Cancel_Initiated" },
            {CEID_Transfer_Completed,"Transfer_Completed" },
            {CEID_Transfer_Initiated,"Transfer_Initiated" },
            {CEID_Transfer_Pause,"Transfer_Pause" },
            {CEID_Transfer_Resumed,"Transfer_Resumed" },
            {CEID_Carrier_Transferring,"Carrier_Transferring" },
            
            {CEID_Carrier_Installed_Completed,"Carrier_Installed_Completed"},
            {CEID_Carrier_Removed_Completed,"Carrier_Removed_Completed"},

            {CEID_Carrier_Removed_Port,"Carrier_Removed_Port"},
            {CEID_Carrier_Resumed,"_Carrier_Resumed"},
            {CEID_Carrier_Stored,"Carrier_Stored"},
            {CEID_Carrier_Stored_Alt,"Carrier_Stored_Alt"},
            {CEID_Shelf_Status_Change,"Shelf_Status_Change"},
            {CEID_Carrier_Wait_In,"Carrier_Wait_In"},
            {CEID_Carrier_Wait_Out,"Carrier_Wait_Out"},
            {CEID_Unit_Alarm_Set,"Unit_Alarm_Set"},
            {CEID_Unit_Alarm_Cleared,"Unit_Alarm_Cleared"},

            {CEID_Crane_Active,"Crane_Active"},
            {CEID_Crane_Idle,"Crane_Idle"},
            {CEID_Crane_In_Escape,"Crane_In_Escape"},
            {CEID_Crane_Out_Escape,"Crane_Out_Escape"},
            {CEID_Crane_Out_Servce,"Crane_Out_Servce"},
            {CEID_Crane_In_Servce,"Crane_In_Servce"},

            {CEID_Carrier_ID_Read,"Carrier_ID_Read"},
            {CEID_Zone_Capacity_Change,"Zone_Capacity_Change"},
            {CEID_Carrier_Box_ID_Rename,"Carrier_Box_ID_Rename"},
            {CEID_Operator_Initiated_Action,"Operator_Initiated_Action"},
            {CEID_Empty_Box_Supply,"Empty_Box_Supply"},
            {CEID_Empty_Box_Recycling,"Empty_Box_Recycling"},

            {CEID_Port_Out_Of_Service,"Port_Out_Of_Service"},
            {CEID_Port_In_Service,"Port_In_Service"},

            {CEID_Load_Req,"Load_Req"},
            {CEID_Unload_Req,"Unload_Req"},
            {CEID_No_Req,"No_Req"},

            {CEID_Port_Type_Input,"Port_Type_Input"},
            {CEID_Port_Type_Output,"Port_Type_Output"},
            {CEID_Port_Type_Changing,"Port_Type_Changing"},
        };
        #endregion CEID Array
        #endregion CEID

        #region LANECUTTYPE
        public static readonly string LANECUTTYPE_LaneCutOnHMI = "0";
        public static readonly string LANECUTTYPE_LaneCutVehicle = "1";
        #endregion LANECUTTYPE
        
        #region ACKC6
        /// <summary>
        /// The ack C6_ accepted
        /// </summary>
        public static readonly string ACKC6_Accepted = "0";
        /// <summary>
        /// The ack C6_ not accepted
        /// </summary>
        public static readonly string ACKC6_NotAccepted = "1";
        #endregion ACKC6

        #region TIACK
        /// <summary>
        /// The tiac k_ accepted
        /// </summary>
        public static readonly string TIACK_Accepted = "0";
        /// <summary>
        /// The tiac k_ error_not_done
        /// </summary>
        public static readonly string TIACK_Error_not_done = "1";
        #endregion TIACK

        #region OFLACK
        /// <summary>
        /// The oflac k_ accepted
        /// </summary>
        public static readonly string OFLACK_Accepted = "0";
        /// <summary>
        /// The oflac k_ not_ accepted
        /// </summary>
        public static readonly string OFLACK_Not_Accepted = "1";
        #endregion OFLACK

        #region EAC
        /// <summary>
        /// The ea c_ accept
        /// </summary>
        public static readonly string EAC_Accept = "0";
        /// <summary>
        /// The ea c_ denied_ at_ least_one_constant_does_not_exist
        /// </summary>
        public static readonly string EAC_Denied_At_Least_one_constant_does_not_exist = "1";
        /// <summary>
        /// The ea c_ denied_ busy
        /// </summary>
        public static readonly string EAC_Denied_Busy = "2";
        /// <summary>
        /// The ea c_ denied_ at_least_one_constant_out_of_range
        /// </summary>
        public static readonly string EAC_Denied_At_least_one_constant_out_of_range = "3";
        /// <summary>
        /// The ea c_ other_equipment_specific_error
        /// </summary>
        public static readonly string EAC_Other_equipment_specific_error = "4";
        #endregion EAC

        #region TIAACK
        /// <summary>
        /// The tiaac k_ everything_correct
        /// </summary>
        public static readonly string TIAACK_Everything_correct = "0";
        /// <summary>
        /// The tiaac k_ too_many_ svi ds
        /// </summary>
        public static readonly string TIAACK_Too_many_SVIDs = "1";
        /// <summary>
        /// The tiaac k_ no_more_traces_allowed
        /// </summary>
        public static readonly string TIAACK_No_more_traces_allowed = "2";
        /// <summary>
        /// The tiaac k_ invalid_period
        /// </summary>
        public static readonly string TIAACK_Invalid_period = "3";
        /// <summary>
        /// The tiaac k_ equipment_specified_error
        /// </summary>
        public static readonly string TIAACK_Equipment_specified_error = "4";
        #endregion TIAACK

        #region ERACK
        /// <summary>
        /// The erac k_ accepted
        /// </summary>
        public static readonly string ERACK_Accepted = "0";
        /// <summary>
        /// The erac k_ denied_ at_least_one_ cei d_dose_not_exist
        /// </summary>
        public static readonly string ERACK_Denied_At_least_one_CEID_dose_not_exist = "1";
        /// <summary>
        /// The erac k_ other_ errors
        /// </summary>
        public static readonly string ERACK_Other_Errors = "2";
        #endregion ERACK

        #region ACKC5
        /// <summary>
        /// The ack C5_ accepted
        /// </summary>
        public static readonly string ACKC5_Accepted = "0";
        /// <summary>
        /// The ack C5_ not_ accepted
        /// </summary>
        public static readonly string ACKC5_Not_Accepted = "1";
        #endregion ACKC5

        #region ACKC7
        /// <summary>
        /// The ack C7_ accepted
        /// </summary>
        public static readonly string ACKC7_Accepted = "0";
        /// <summary>
        /// The ack C7_ not_ accepted
        /// </summary>
        public static readonly string ACKC7_Not_Accepted = "1";
        /// <summary>
        /// The ack C7_ unit_ i d_is_not_exist
        /// </summary>
        public static readonly string ACKC7_Unit_ID_is_not_exist = "2";
        /// <summary>
        /// The ack C7_ pptyp e_is_not_match
        /// </summary>
        public static readonly string ACKC7_PPTYPE_is_not_match = "3";
        /// <summary>
        /// The ack C7_ ppi d_is_not_match
        /// </summary>
        public static readonly string ACKC7_PPID_is_not_match = "4";
        #endregion ACKC7

        #region ACKC10
        /// <summary>
        /// The ack C10_ accepted
        /// </summary>
        public static readonly string ACKC10_Accepted = "0";
        /// <summary>
        /// The ack C10_ not_ accepted
        /// </summary>
        public static readonly string ACKC10_Not_Accepted = "1";
        #endregion ACKC10

        #region CEED
        /// <summary>
        /// The cee d_ enable
        /// </summary>
        public static readonly string CEED_Enable = "0";
        /// <summary>
        /// The cee d_ disable
        /// </summary>
        public static readonly string CEED_Disable = "1";
        #endregion CEED

        #region ALED
        /// <summary>
        /// The ale d_ enable
        /// </summary>
        public static readonly string ALED_Enable = "1";
        /// <summary>
        /// The ale d_ disable
        /// </summary>
        public static readonly string ALED_Disable = "128";
        #endregion ALED


        #region PPCINFO
        /// <summary>
        /// A new PPID is created and registered
        /// </summary>
        public static readonly string PPCINFO_Created = "1";
        /// <summary>
        /// Some parameters of a PPID are modified
        /// </summary>
        public static readonly string PPCINFO_Modified = "2";
        /// <summary>
        /// Any PPID is deleted
        /// </summary>
        public static readonly string PPCINFO_Deleted = "3";
        /// <summary>
        /// Equipment sets up any PPID which different from current PPID
        /// </summary>
        public static readonly string PPCINFO_Changed = "4";
        #endregion PPCINFO

        #region ALST
        /// <summary>
        /// The als t_ set
        /// </summary>
        public static readonly string ALST_SET = "1";
        /// <summary>
        /// The als t_ clear
        /// </summary>
        public static readonly string ALST_CLEAR = "2";
        #endregion ALST

        #region ALCD
        /// <summary>
        /// The alc d_ light_ alarm
        /// </summary>
        public static readonly string ALCD_Alarm_Set = "80";
        /// <summary>
        /// The alc d_ serious_ alarm
        /// </summary>
        public static readonly string ALCD_Alarm_Clear = "0";
        #endregion ALCD


        #region SCACK
        /// <summary>
        /// The scac k_ accepted
        /// </summary>
        public static readonly string SCACK_Accepted = "0";
        /// <summary>
        /// The scac k_ busy
        /// </summary>
        public static readonly string SCACK_Busy = "1";
        /// <summary>
        /// The scac k_ csti d_is_ invalid
        /// </summary>
        public static readonly string SCACK_CSTID_is_Invalid = "2";
        /// <summary>
        /// The scac k_ already_ received
        /// </summary>
        public static readonly string SCACK_Already_Received = "3";
        /// <summary>
        /// The scac k_ slo t_ information_ mismatch
        /// </summary>
        public static readonly string SCACK_SLOT_Information_Mismatch = "4";
        /// <summary>
        /// The scac k_ net yet_ prepared_ for_ this_ sorter_ job
        /// </summary>
        public static readonly string SCACK_NetYet_Prepared_For_This_Sorter_Job = "5";
        #endregion


        #region CPACK
        public static readonly string CPACK_No_Error = "0";
        public static readonly string CPACK_Name_Not_Exist = "1";
        public static readonly string CPACK_Invalid_Value = "2";
        public static readonly string CPACK_Invalid_Format = "3";
        public static readonly string CPACK_Other_Error = "4";
        #endregion

        #region SCSTATE
        public static readonly string SCSTATE_Init = "1";
        public static readonly string SCSTATE_Paused = "2";
        public static readonly string SCSTATE_Auto = "3";
        public static readonly string SCSTATE_Pausing = "4";
        #endregion SCSTATE

        #region TRANSFERSTATE
        public static readonly string TRANSFERSTATE_Queued = "1";
        public static readonly string TRANSFERSTATE_Transsfring = "2";
        public static readonly string TRANSFERSTATE_Paused = "3";
        public static readonly string TRANSFERSTATE_Canceling = "4";
        public static readonly string TRANSFERSTATE_Aborting = "5";
        public static readonly string TRANSFERSTATE_Waiting = "6";
        public static string convert2MES(E_TRAN_STATUS tran_status)
        {
            switch (tran_status)
            {
                case E_TRAN_STATUS.Queue:
                    return SECSConst.TRANSFERSTATE_Queued;
                //case E_TRAN_STATUS.Initial:
                //case E_TRAN_STATUS.PreInitial:
                //    return SECSConst.TRANSFERSTATE_Waiting;
                case E_TRAN_STATUS.Transferring:
                    return SECSConst.TRANSFERSTATE_Transsfring;
                case E_TRAN_STATUS.Canceling:
                    return SECSConst.TRANSFERSTATE_Canceling;
                case E_TRAN_STATUS.Aborting:
                    return SECSConst.TRANSFERSTATE_Aborting;
                default:
                    return SECSConst.TRANSFERSTATE_Transsfring;
            }
        }
        #endregion TRANSFERSTATE

        #region COMMACK
        public static readonly string COMMACK_ACK = "0x0";
        public static readonly string COMMACK_Denide_Retry = "0x1";
        #endregion COMMACK

        #region Carrier ID Read Status
        public const string IDREADSTATUS_Successful = "0";
        public const string IDREADSTATUS_Failed = "1";
        public const string IDREADSTATUS_Duplicate = "2";
        public const string IDREADSTATUS_Mismatch = "3";
        #endregion Carrier ID Read Status

        #region port
        public const string PortType_Input = "0";
        public const string PortType_Output = "1";
        public const int PortState_OutService = 1;
        public const int PortState_InService = 2;
        #endregion
        //#region PPTYPE
        //public static readonly string PPTYPE_Equipment = "E";
        //public static readonly string PPTYPE_Unit = "U";
        //public static readonly string PPTYPE_SubUnit = "S";
        //#endregion PPTYPE

        /// <summary>
        /// Checks the data value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>com.mirle.ibg3k0.stc.Common.SECS.SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.</returns>
        public static com.mirle.ibg3k0.stc.Common.SECS.SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT checkDataValue(
            string name, string value)
        {
            com.mirle.ibg3k0.stc.Common.SECS.SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT result =
                SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.Recognize;

            //if (name.Trim().Equals("CRST"))
            //{
            //    //SECSConst.CRST
            //    if (!SECSConst.CRST.Contains(value.Trim()))
            //    {
            //        return SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.Illegal_Data_Value_Format;
            //    }
            //}

            return result;
        }


        /// <summary>
        /// The stream identifier array
        /// </summary>
        public static readonly int[] StreamIDArray = { 1, 2, 5, 6, 7, 9, 10 };
        /// <summary>
        /// The function identifier array
        /// </summary>
        public static readonly int[] FunctionIDArray =
        {
            0, 1, 2, 3, 4, 5, 6, 7, 9,
            11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
            23, 24, 25, 26, 29,
            30, 31, 32, 33, 34, 35, 36, 37, 38,
            41, 42, 49, 50,
            53, 54 ,
            103, 104, 105, 106, 107, 108, 109, 110, 110, 112,
            203, 204
        };

        static Dictionary<string, List<string>> DicCEIDAndRPTID { get; set; }
        static Dictionary<string, List<ARPTID>> DicRPTIDAndVID { get; set; }

        static object CEIDAndRPTID_lock = new object();
        static object RPTIDAndVID_lock = new object();
        public static void setDicCEIDAndRPTID(Dictionary<string, List<string>> _dic)
        {
            lock (CEIDAndRPTID_lock)
            {
                DicCEIDAndRPTID = _dic;
            }
        }
        public static void setDicRPTIDAndVID(Dictionary<string, List<ARPTID>> _dic)
        {
            lock (RPTIDAndVID_lock)
            {
                DicRPTIDAndVID = _dic;
            }
        }
        public static List<string> getDicCEIDAndRPTID(string ceid)
        {
            lock (CEIDAndRPTID_lock)
            {
                return DicCEIDAndRPTID[ceid];
            }
        }
        public static List<ARPTID> getDicRPTIDAndVID(string rptID)
        {
            lock (RPTIDAndVID_lock)
            {
                return DicRPTIDAndVID[rptID];
            }
        }

        /// <summary>
        /// Checks the type of the sf.
        /// </summary>
        /// <param name="S">The s.</param>
        /// <param name="F">The f.</param>
        /// <returns>com.mirle.ibg3k0.stc.Common.SECS.SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.</returns>
        public static com.mirle.ibg3k0.stc.Common.SECS.SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT checkSFType(int S, int F)
        {
            com.mirle.ibg3k0.stc.Common.SECS.SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT result =
                SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.Recognize;
            string streamFunction = string.Format("S{0}F{1}", S, F);

            if (!StreamIDArray.Contains(S))
            {
                result = SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.Unrecognized_Stream_Type;
            }
            else if (!FunctionIDArray.Contains(F))
            {
                result = SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.Unrecognized_Function_Type;
            }
            else
            {
                Type type = Type.GetType("com.mirle.ibg3k0.sc.Data.SECS." + streamFunction);
                Type typeBase = Type.GetType("com.mirle.ibg3k0.stc.Data.SecsData." + streamFunction);
                if (type == null && typeBase == null && F != 0)
                {
                    result = SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.Unrecognized_Stream_Type;
                }
            }
            return result;
        }



    }
}
