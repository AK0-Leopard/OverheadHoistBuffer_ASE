using com.mirle.ibg3k0.sc.Data.ValueDefMapAction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.TcpIp
{
    class VHMSGIF
    {
        public const int LEN_MESSAGE_SIZE = 100;
        public const int LEN_ITEM_CSTID = 16;
        public const int LEN_ITEM_DATETIME = 14;
        public const int LEN_ITEM_TESTDATA = 8;
        //public const int LEN_ITEM_PASSSEGMENT = 4;
        public const int LEN_ITEM_PASSSEGMENT = 6;

        public const ushort ITEM_RESPCODE_OK = 0;
        public const ushort ITEM_RESPCODE_NG = 1;

        public const ushort ITEM_MULTIFLAG_LAST = 0;
        public const ushort ITEM_MULTIFLAG_CONT = 1;

        // Controller -> Vehicle Packet ID = [1 - 99]
        // Vehicle -> Controller Packet ID = [101 - 199]
        public const int ID_NONE = 0;
        public const int ID_HOST_KISO_VERSION_REPORT = 1;
        public const int ID_HOST_KISO_VERSION_RESPONSE = 101;

        public const int ID_VHCL_KISO_VERSION_REPORT = 102;
        public const int ID_VHCL_KISO_VERSION_RESPONSE = 2;

        public const int ID_KISO_LIST_COUNT_REPORT = 11;
        public const int ID_KISO_LIST_COUNT_RESPONSE = 111;

        public const int ID_KISO_TRAVEL_REPORT = 13;
        public const int ID_KISO_TRAVEL_RESPONSE = 113;

        public const int ID_KISO_SECTION_REPORT = 15;
        public const int ID_KISO_SECTION_RESPONSE = 115;

        public const int ID_KISO_ADDRESS_REPORT = 17;
        public const int ID_KISO_ADDRESS_RESPONSE = 117;

        public const int ID_KISO_SCALE_REPORT = 19;
        public const int ID_KISO_SCALE_RESPONSE = 119;

        public const int ID_KISO_CONTROL_REPORT = 21;
        public const int ID_KISO_CONTROL_RESPONSE = 121;

        public const int ID_KISO_GUIDE_REPORT = 23;
        public const int ID_KISO_GUIDE_RESPONSE = 123;

        public const int ID_KISO_GRIPPER_REPORT = 25;
        public const int ID_KISO_GRIPPER_RESPONSE = 125;

        public const int ID_TRANS_REQUEST = 31;
        public const int ID_TRANS_REQUEST_RESPONSE = 131;

        public const int ID_TRANS_COMPLETE_REPORT = 132;
        public const int ID_TRANS_COMPLETE_RESPONSE = 32;

        public const int ID_TRANS_PASS_EVENT_REPORT = 134;
        public const int ID_TRANS_PASS_EVENT_RESPONSE = 34;

        public const int ID_CSTID_RENAME_REQUEST = 35;
        public const int ID_CSTID_RENAME_RESPONSE = 135;

        public const int ID_TRANS_EVENT_REPORT = 136;
        public const int ID_TRANS_EVENT_RESPONSE = 36;

        public const int ID_TRANS_CANCEL_REQUEST = 37;
        public const int ID_TRANS_CANCEL_RESPONSE = 137;

        public const int ID_PAUSE_REQUEST = 39;
        public const int ID_PAUSE_RESPONSE = 139;

        public const int ID_MODE_CHANGE_REQUEST = 41;
        public const int ID_MODE_CHANGE_RESPONSE = 141;

        public const int ID_STATUS_REQUEST = 43;
        public const int ID_STATUS_RESPONSE = 143;

        public const int ID_STATUS_CHANGE_REPORT = 144;
        public const int ID_STATUS_CHANGE_RESPONSE = 44;

        public const int ID_POWER_OPE_REQUEST = 45;
        public const int ID_POWER_OPE_RESPONSE = 145;

        public const int ID_INDIVIDUAL_DATA_UPLOAD_REQUEST = 61;
        public const int ID_INDIVIDUAL_DATA_UPLOAD_REPORT = 161;

        public const int ID_GUIDE_DATA_UPLOAD_REQUEST = 162;
        public const int ID_GUIDE_DATA_UPLOAD_REPORT = 62;

        public const int ID_INDIVIDUAL_DATA_CHANGE_REQUEST = 63;
        public const int ID_INDIVIDUAL_DATA_CHANGE_RESPONSE = 163;

        public const int ID_SECTION_TEACH_REQUEST = 71;
        public const int ID_SECTION_TEACH_RESPONSE = 171;

        public const int ID_SECTION_TEACH_COMPLETE_REPORT = 172;
        public const int ID_SECTION_TEACH_COMPLETE_RESPONSE = 72;

        public const int ID_ADDRESS_TEACH_REPORT = 174;
        public const int ID_ADDRESS_TEACH_RESPONSE = 74;

        public const int ID_ALARM_RESET_REQUEST = 91;
        public const int ID_ALARM_RESET_RESPONSE = 191;

        public const int ID_ALARM_REPORT = 194;
        public const int ID_ALARM_RESPONSE = 94;

        public const int ID_LOG_UPLOAD_REQUEST = 95;
        public const int ID_LOG_UPLOAD_RESPONSE = 195;

        public const int ID_LOG_DATA_REPORT = 196;
        public const int ID_LOG_DATA_RESPONSE = 96;

        public const int ID_VHCL_COMM_TEST_REQUEST = 198;
        public const int ID_VHCL_COMM_TEST_REPORT = 98;

        public const int ID_HOST_COMM_TEST_REQUEST = 99;
        public const int ID_HOST_COMM_TEST_REPORT = 199;

        public static string[] ID_NAMES;
        public static void PrcInitializeIDNames()
        {
            string sWk = "";

            ID_NAMES = new string[256];
            for (int ii = 0; ii < ID_NAMES.Length; ii++)
            {
                switch (ii)
                {
                    case ID_HOST_KISO_VERSION_REPORT: sWk = "[  1]Host Version Rep."; break;
                    case ID_HOST_KISO_VERSION_RESPONSE: sWk = "[101]Host Version Resp."; break;
                    case ID_VHCL_KISO_VERSION_REPORT: sWk = "[102]Vehicle Version Rep."; break;
                    case ID_VHCL_KISO_VERSION_RESPONSE: sWk = "[  2]Vehicle Version Resp."; break;
                    case ID_KISO_LIST_COUNT_REPORT: sWk = "[ 11]Kiso ListCount Rep."; break;
                    case ID_KISO_LIST_COUNT_RESPONSE: sWk = "[111]Kiso ListCount Resp."; break;
                    case ID_KISO_TRAVEL_REPORT: sWk = "[ 13]Kiso Travel Rep."; break;
                    case ID_KISO_TRAVEL_RESPONSE: sWk = "[113]Kiso Travel Resp."; break;
                    case ID_KISO_SECTION_REPORT: sWk = "[ 15]Kiso Section Rep."; break;
                    case ID_KISO_SECTION_RESPONSE: sWk = "[115]Kiso Section Resp."; break;
                    case ID_KISO_ADDRESS_REPORT: sWk = "[ 17]Kiso Address Rep."; break;
                    case ID_KISO_ADDRESS_RESPONSE: sWk = "[117]Kiso Address Resp."; break;
                    case ID_KISO_SCALE_REPORT: sWk = "[ 19]Kiso Scale Rep."; break;
                    case ID_KISO_SCALE_RESPONSE: sWk = "[119]Kiso Scale Resp."; break;
                    case ID_KISO_CONTROL_REPORT: sWk = "[ 21]Kiso Control Rep."; break;
                    case ID_KISO_CONTROL_RESPONSE: sWk = "[121]Kiso Control Resp."; break;
                    case ID_KISO_GUIDE_REPORT: sWk = "[ 23]Kiso Guide Rep."; break;
                    case ID_KISO_GUIDE_RESPONSE: sWk = "[123]Kiso Guide Resp."; break;
                    case ID_TRANS_REQUEST: sWk = "[ 31]Trans Req."; break;
                    case ID_TRANS_REQUEST_RESPONSE: sWk = "[131]Trans Resp."; break;
                    case ID_TRANS_COMPLETE_REPORT: sWk = "[132]TransComp Rep."; break;
                    case ID_TRANS_COMPLETE_RESPONSE: sWk = "[ 32]TransComp Resp."; break;
                    case ID_TRANS_PASS_EVENT_REPORT: sWk = "[134]TransEvent Rep."; break;
                    case ID_TRANS_PASS_EVENT_RESPONSE: sWk = "[ 34]TransEvent Resp."; break;
                    case ID_CSTID_RENAME_REQUEST: sWk = "[ 35]TransChange Req."; break;
                    case ID_CSTID_RENAME_RESPONSE: sWk = "[135]TransChange Resp."; break;
                    case ID_TRANS_CANCEL_REQUEST: sWk = "[ 37]TransCancel Req."; break;
                    case ID_TRANS_CANCEL_RESPONSE: sWk = "[137]TransCancel Resp."; break;
                    case ID_PAUSE_REQUEST: sWk = "[ 39]Pause Req."; break;
                    case ID_PAUSE_RESPONSE: sWk = "[139]Pause Resp."; break;
                    case ID_MODE_CHANGE_REQUEST: sWk = "[ 41]ModeChange Req."; break;
                    case ID_MODE_CHANGE_RESPONSE: sWk = "[141]ModeChange Resp."; break;
                    case ID_STATUS_REQUEST: sWk = "[ 43]Status Req."; break;
                    case ID_STATUS_RESPONSE: sWk = "[143]Status Resp."; break;
                    case ID_STATUS_CHANGE_REPORT: sWk = "[144]StatusChange Rep."; break;
                    case ID_STATUS_CHANGE_RESPONSE: sWk = "[ 44]StatusChange Resp."; break;
                    case ID_POWER_OPE_REQUEST: sWk = "[ 45]PowerOpe Req."; break;
                    case ID_POWER_OPE_RESPONSE: sWk = "[145]PowerOpe Resp."; break;
                    case ID_INDIVIDUAL_DATA_UPLOAD_REQUEST: sWk = "[ 61]IndividualUp Req."; break;
                    case ID_INDIVIDUAL_DATA_UPLOAD_REPORT: sWk = "[161]IndividualUp Rep."; break;
                    case ID_GUIDE_DATA_UPLOAD_REQUEST: sWk = "[162]IndividualDown Req."; break;
                    case ID_GUIDE_DATA_UPLOAD_REPORT: sWk = "[ 62]IndividualDown Rep."; break;
                    case ID_INDIVIDUAL_DATA_CHANGE_REQUEST: sWk = "[ 63]IndividualChange Req."; break;
                    case ID_INDIVIDUAL_DATA_CHANGE_RESPONSE: sWk = "[163]IndividualChange Resp."; break;
                    case ID_SECTION_TEACH_REQUEST: sWk = "[ 71]SectionTeach Req."; break;
                    case ID_SECTION_TEACH_RESPONSE: sWk = "[171]SectionTeach Resp."; break;
                    case ID_SECTION_TEACH_COMPLETE_REPORT: sWk = "[172]SectionTeachComp Rep."; break;
                    case ID_SECTION_TEACH_COMPLETE_RESPONSE: sWk = "[ 72]SectionTeachComp Resp."; break;
                    case ID_ADDRESS_TEACH_REPORT: sWk = "[174]AddressTeach Rep."; break;
                    case ID_ADDRESS_TEACH_RESPONSE: sWk = "[ 74]AddressTeach Resp."; break;
                    case ID_ALARM_RESET_REQUEST: sWk = "[ 91]AlarmReset Req."; break;
                    case ID_ALARM_RESET_RESPONSE: sWk = "[191]AlarmReset Resp."; break;
                    case ID_ALARM_REPORT: sWk = "[194]Alarm Rep."; break;
                    case ID_ALARM_RESPONSE: sWk = "[ 94]Alarm Resp."; break;
                    case ID_LOG_UPLOAD_REQUEST: sWk = "[ 95]LogUpload Req."; break;
                    case ID_LOG_UPLOAD_RESPONSE: sWk = "[195]LogUpload Resp."; break;
                    case ID_LOG_DATA_REPORT: sWk = "[196]LogData Rep."; break;
                    case ID_LOG_DATA_RESPONSE: sWk = "[ 96]LogData Resp."; break;
                    case ID_VHCL_COMM_TEST_REQUEST: sWk = "[198]Vehicle CommTest Req."; break;
                    case ID_VHCL_COMM_TEST_REPORT: sWk = "[ 98]Vehicle CommTest Resp."; break;
                    case ID_HOST_COMM_TEST_REQUEST: sWk = "[ 99]Host CommTest Req."; break;
                    case ID_HOST_COMM_TEST_REPORT: sWk = "[199]Host CommTest Resp."; break;
                    default: sWk = "[   ]Not Defined"; break;
                }
                ID_NAMES[ii] = sWk.PadRight(32);
            }
        }

        public static int[] NeedToConfirmPacketIDCollection
            = new List<int>
            {
                ID_VHCL_KISO_VERSION_REPORT,
                ID_TRANS_COMPLETE_REPORT,
                ID_TRANS_PASS_EVENT_REPORT,
                ID_STATUS_CHANGE_REPORT,
                ID_GUIDE_DATA_UPLOAD_REQUEST,
                ID_SECTION_TEACH_COMPLETE_REPORT,
                ID_ADDRESS_TEACH_REPORT,
                ID_ALARM_REPORT,
                ID_LOG_DATA_REPORT,
                ID_VHCL_COMM_TEST_REQUEST
            }.ToArray();

        public static string[] NeedToConfirmPacketIDCollection_String
        = new List<string>
            {
                ID_VHCL_KISO_VERSION_REPORT.ToString(),
                ID_TRANS_COMPLETE_REPORT.ToString(),
                ID_TRANS_PASS_EVENT_REPORT.ToString(),
                ID_STATUS_CHANGE_REPORT.ToString(),
                ID_GUIDE_DATA_UPLOAD_REQUEST.ToString(),
                ID_SECTION_TEACH_COMPLETE_REPORT.ToString(),
                ID_ADDRESS_TEACH_REPORT.ToString(),
                ID_ALARM_REPORT.ToString(),
                ID_LOG_DATA_REPORT.ToString(),
                ID_VHCL_COMM_TEST_REQUEST.ToString()
            }.ToArray();
    }


    /// <summary>
    /// ホスト～台車間メッセージフォーマット
    /// Message Format between Host and Vehicle
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_MSG_FORMAT
    {
        /// <summary>
        /// [2W(4B)] Preamble Code = "@PKT"
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] Preamble;
        /// <summary>
        /// [1W(2B)] Packet Length
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketLen;
        /// <summary>
        /// [27W(94B)] Packet Data
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 94)]
        public byte[] PacketDatas;

        public STR_MSG_FORMAT(int iPacketLen)
        {
            Preamble = new char[] { '@', 'P', 'K', 'T' };
            PacketLen = (UInt16)iPacketLen;
            PacketDatas = new byte[94];
        }
    }


    #region "Host Vehicle Data Version Report(1)[ホスト台車データバージョン報告] / Response(101)[応答]"

    /// <summary>
    /// [1] ホスト台車データバージョン報告
    /// [1] Host Vehicle Data Version Report
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_HOST_KISO_VERSION_REP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [7W(14B)] Vehicle Data Version
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
        public char[] DataDateTime;
        /// <summary>
        /// [1W(2B)] Local DateTime:Year
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 LocalYear;
        /// <summary>
        /// [1W(2B)] Local DateTime:Month
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 LocalMonth;
        /// <summary>
        /// [1W(2B)] Local DateTime:Day
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 LocalDay;
        /// <summary>
        /// [1W(2B)] Local DateTime:Hour
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 LocalHour;
        /// <summary>
        /// [1W(2B)] Local DateTime:Minute
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 LocalMinute;
        /// <summary>
        /// [1W(2B)] Local DateTime:Second
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 LocalSecond;
    }

    /// <summary>
    /// [101] ホスト台車データバージョン報告応答
    /// [101] Host Vehicle Data Version Response
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_HOST_KISO_VERSION_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code [0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Host Vehicle Data Version Report(1)[ホスト台車データバージョン報告] / Response(101)[応答] */

    #region "Vehicle Data Version Report(102)[台車データバージョン報告] / Response(2)[応答]"

    /// <summary>
    /// [102] 台車データバージョン報告
    /// [102] Host Vehicle Data Version Report
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_VHCL_KISO_VERSION_REP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [7W(14B)] Vehicle Data Version
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
        public char[] VerionStr;
    }

    /// <summary>
    /// [2] 台車データバージョン報告応答
    /// [2] Vehicle Data Version Response
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_VHCL_KISO_VERSION_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code [0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/*  */

    #region "Vehicle Data List Count Reqest(11)[台車データ数報告] / Response(111)[応答]"

    /// <summary>
    /// [11] 台車データ数報告
    /// [11] Vehicle Data List Count Reqest
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_KISO_LIST_COUNT_REP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Report List Table
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ListTable;
        /// <summary>
        /// [1W(2B)] List1 Table Count
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ListCount1;
        /// <summary>
        /// [1W(2B)] List2 Table Count
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ListCount2;
        /// <summary>
        /// [1W(2B)] List3 Table Count
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ListCount3;
        /// <summary>
        /// [1W(2B)] List4 Table Count
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ListCount4;
        /// <summary>
        /// [1W(2B)] List5 Table Count
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ListCount5;
        /// <summary>
        /// [1W(2B)] List6 Table Count
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ListCount6;

    }

    /// <summary>
    /// [111] 台車データ数報告応答
    /// [111] Vehicle Data List Count Response
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_KISO_LIST_COUNT_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code [0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/*  */

    #region "Vehcile Tavelling Data Report(13)[走行基礎データ報告] / Response(113)[応答]"

    /// <summary>
    /// [13] 走行基礎データ報告
    /// [13] Vehicle Tavelling Data Report
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_KISO_TRAVEL_REP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [2W(4B)] Resolution
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 Resolution;
        /// <summary>
        /// [2W(4B)] Start / Stop Speed
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 StartStopSpd;
        /// <summary>
        /// [2W(4B)] Max Speed
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 MaxSpeed;
        /// <summary>
        /// [2W(4B)] Accel / Deccel Speed
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 AccelTime;
        /// <summary>
        /// [1W(2B)] S Curve Rate
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SCurveRate;
        /// <summary>
        /// [1W(2B)] Origin Direction
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 OriginDir;
        /// <summary>
        /// [2W(4B)] Origin Speed
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 OriginSpd;
        /// <summary>
        /// [2W(4B)] Beam Speed
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 BeaemSpd;
        /// <summary>
        /// [2W(4B)] Manual High Speed
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 ManualHSpd;
        /// <summary>
        /// [2W(4B)] Manual Low Speed
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 ManualLSpd;
        /// <summary>
        /// [2W(4B)] Teaching Speed
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 TeachingSpd;
        /// [1W(2B)] Direction of Axis Rotate
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RotateDir;
        /// [1W(2B)] Paole of Encoder
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 EncoderPole;
        /// [1W(2B)] 位置的補償
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 PositionCompensation;
        /// <summary>
        /// [2W(4B)] Keep Distantce : Far
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 KeepDistFar;
        /// <summary>
        /// [2W(4B)] Keep Distantce : Near
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 KeepDistNear;
    }

    /// <summary>
    /// [113] 走行基礎データ報告応答
    /// [113] Vehicle Tavelling Data Response
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_KISO_TRAVEL_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code [0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;

    }

    #endregion	/* Vehcile Tavelling Data Report(13)[走行基礎データ報告] / Response(113)[応答] */

    #region "Vehcile Section Data Report(15)[走行区間データ報告] / Response(115)[応答]"

    /// <summary>
    /// [15] 走行区間データ報告
    /// [15] Vehicle Section Data Report
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_KISO_SECTION_REP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Drive Direction	[0:Forward, 1:Backward]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 DriveDir;
        /// <summary>
        /// [1W(2B)] Guide Direciton	[0:Left, 1:Right]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 AeraSecsor;
        /// <summary>
        /// [1W(2B)] Guide Direciton	[0:Left, 1:Right]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 GuideDir;
        /// <summary>
        /// [1W(2B)] Segment
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SectionID;
        /// <summary>
        /// [1W(2B)] From Address
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 FromAddr;
        /// <summary>
        /// [1W(2B)] To Address
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 ToAddr;
        /// <summary>
        /// [1W(2B)] ControlTable
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ControlTable;
        /// <summary>
        /// [1W(2B)] Resv1
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Resv1;
        /// <summary>
        /// [2W(4B)] Speed
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 Speed;
        /// <summary>
        /// [2W(4B)] Distance
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 Distance;

        /// <summary>
        /// [1W(2B)] Change Area Sensor
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ChangeAreaSensor1;
        /// <summary>
        /// [1W(2B)] Change Guide Direction
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ChangeGuideDir1;
        /// <summary>
        /// [1W(2B)] Change Segment Num
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ChangeSegNum1;
        /// <summary>
        /// [1W(2B)] Change Area Sensor
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ChangeAreaSensor2;
        /// <summary>
        /// [1W(2B)] Change Guide Direction
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ChangeGuideDir2;
        /// <summary>
        /// [1W(2B)] Change Segment Num
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ChangeSegNum2;
        /// <summary>
        /// [1W(2B)] Multi Packet Flag
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 AtSegment;
        /// <summary>
        /// [1W(2B)] Multi Packet Flag
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 MultiFlag;
    }

    /// <summary>
    /// [115] 台車区間データ報告応答
    /// [115] Vehicle Section Data Response
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_KISO_SECTION_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code [0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Vehcile Section Data Report(15)[走行区間データ報告] / Response(115)[応答] */

    #region "Vehcile Address Data Report(17)[アドレスデータ報告] / Response(117)[応答]"

    /// <summary>
    /// [17] アドレスデータ報告
    /// [17] Vehicle Address Data Report
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_KISO_ADDRESS_REP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [2W(4B)] Port Address
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 Addr;
        /// <summary>
        /// [1W(2B)] PortCSNum
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 Resolution;
        /// <summary>
        /// [2W(4B)] Total Scale Pulse
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 Loaction;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 BlockRelease;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 HIDRelease;

        /// <summary>
        /// [1W(2B)] Multi Packet Flag
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 MultiFlag;
    }

    /// <summary>
    /// [117] アドレスデータ報告応答
    /// [117] Vehicle Address Data Response
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_KISO_ADDRESS_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code [0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Vehcile Address Data Report(17)[アドレスデータ報告] / Response(117)[応答] */

    #region "Vehcile Scale Data Report(19)[スケール基礎データ報告] / Response(119)[応答]"

    /// <summary>
    /// [19] スケール基礎データ報告
    /// [19] Vehicle Scale Data Report
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_KISO_SCALE_REP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [2W(4B)] Resolution
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 Resolution;
        /// <summary>
        /// [2W(4B)] Inposition Area
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 InposArea;
        /// <summary>
        /// [2W(4B)] Inposition Stability Time
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 InposStability;
        /// <summary>
        /// [2W(4B)] Scale Pulse
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 ScalePulse;
        /// <summary>
        /// [2W(4B)] Scale Offset
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 ScaleOffset;
        /// <summary>
        /// [2W(4B)] Scale Reset
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 ScaleReset;
        /// <summary>
        /// [1W(2B)] Scacle Read Direction
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ReadDir;
    }

    /// <summary>
    /// [119] スケール基礎データ報告
    /// [119] Vehicle Scale Data Report
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_KISO_SCALE_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code [0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Vehcile Scale Data Report(19)[スケール基礎データ報告] / Response(119)[応答] */

    #region "Vehcile Control Data Report(21)[コントロールデータ報告] / Response(121)[応答]"

    /// <summary>
    /// [21] コントロールデータ報告
    /// [21] Vehicle Control Data Report
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_KISO_CONTROL_REP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [2W(4B)] T1 Timeout
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 TimeoutT1;
        /// <summary>
        /// [2W(4B)] T2 Timeout
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 TimeoutT2;
        /// <summary>
        /// [2W(4B)] T3 Timeout
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 TimeoutT3;
        /// <summary>
        /// [2W(4B)] T4 Timeout
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 TimeoutT4;
        /// <summary>
        /// [2W(4B)] T5 Timeout
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 TimeoutT5;
        /// <summary>
        /// [2W(4B)] T6 Timeout
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 TimeoutT6;
        /// <summary>
        /// [2W(4B)] T7 Timeout
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 TimeoutT7;
        /// <summary>
        /// [2W(4B)] T8 Timeout
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 TimeoutT8;
        /// <summary>
        /// [2W(4B)] Blockade Timeout
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 TimeoutBlock;
    }

    /// <summary>
    /// [121] コントロールデータ報告応答
    /// [121] Vehicle Control Data Response
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_KISO_CONTROL_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code [0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Vehcile Control Data Report(21)[コントロールデータ報告] / Response(121)[応答] */

    #region "Vehcile Guide Data Report(23)[ガイド基礎データ報告] / Response(123)[応答]"

    /// <summary>
    /// [23] ガイド基礎データ報告
    /// [23] Vehicle Guide Data Report
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_KISO_GUIDE_REP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [2W(4B)] Start / Stop Speed
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 StartStopSpd;
        /// <summary>
        /// [2W(4B)] Max Speed
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 MaxSpeed;
        /// <summary>
        /// [2W(4B)] Accel / Deccel Speed
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 AccelTime;
        /// <summary>
        /// [1W(2B)] S Curve Rate
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SCurveRate;
        /// <summary>
        /// [1W(2B)] Reserve
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Resv1;
        /// <summary>
        /// [2W(4B)] Normal Speed
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 NormalSpd;
        /// <summary>
        /// [2W(4B)] Manual High Speed
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 ManualHSpd;
        /// <summary>
        /// [2W(4B)] Manual Low Speed
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 ManualLSpd;
        /// <summary>
        /// [2W(4B)] Left-Forward Lock Position
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 LFLockPos;
        /// <summary>
        /// [2W(4B)] Left-Backward Lock Position
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 LBLockPos;
        /// <summary>
        /// [2W(4B)] Right-Forward Lock Position
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 RFLockPos;
        /// <summary>
        /// [2W(4B)] Right-Backward Lock Position
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 RBLockPos;
        /// <summary>
        /// [2W(4B)] Change Guide Stability Time
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 ChangeStabilityTime;
    }

    /// <summary>
    /// [123] ガイド基礎データ報告応答
    /// [123] Vehicle Guide Data Response
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_KISO_GUIDE_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code [0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Vehcile Guide Data Report(23)[ガイド基礎データ報告] / Response(123)[応答] */

    #region "Vehcile Gripper Data Report(25)[Gripper基礎資料報告] / Response(125)[応答]"

    /// <summary>
    /// [25] Gripper基礎資料報告
    /// [25] Vehicle Gripper Data Report
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_KISO_GRIPPER_REP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] 完成站點區分
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 CmpStationDiff;
        /// <summary>
        /// [7W(14B)] Station ID
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
        public char[] StationID;
        /// <summary>
        /// [1W(2B)] Gripper Z值
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Gripper_Z;
        /// <summary>
        /// [1W(2B)] Gripper R值
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Gripper_R;
        /// <summary>
        /// [1W(2B)] Gripper Theta值
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Gripper_Theta;
        /// <summary>
        /// [1W(2B)] 手動高速度
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ManualHighSpeed;
        /// <summary>
        /// [2W(4B)] 手動低速度
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 ManualLowSpeed;

    }

    /// <summary>
    /// [125] Gripper基礎資料報告應答
    /// [125] Vehicle Gripper Data Response
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_KISO_GRIPPER_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code [0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Vehcile Guide Data Report(23)[ガイド基礎データ報告] / Response(123)[応答] */


    #region "Individual Data Upload Request(61)[個体差アップロード要求] / Report(161)[報告]"

    /// <summary>
    /// [61] 個体差アップロード要求
    /// [61] Individual Data Upload Request
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_INDIVIDUAL_UPLOAD_REQ
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
    }

    /// <summary>
    /// [161] 個体差アップロード報告
    /// [161] Individual Data Upload Report
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_INDIVIDUAL_UPLOAD_REP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [2W(4B)] Guide FL Offset
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 OffsetGuideFL;
        /// <summary>
        /// [2W(4B)] Guide RL Offset
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 OffsetGuideRL;
        /// <summary>
        /// [2W(4B)] Guide FR Offset
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 OffsetGuideFR;
        /// <summary>
        /// [2W(4B)] Guide RR Offset
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 OffsetGuideRR;
    }

    #endregion	/* Individual Data Upload Request(61)[個体差アップロード要求] / Report(161)[報告] */

    #region "Individual Data Download Request(162)[個体差ダウンロード要求] / Report(62)[報告]"

    /// <summary>
    /// [162] 個体差ダウンロード要求
    /// [162] Individual Data Download Request
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_GUIDE_DATA_UPLOAD_REQ
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [2W(4B)] Guide FL Offset
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 OffsetGuideFL;
        /// <summary>
        /// [2W(4B)] Guide RL Offset
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 OffsetGuideRL;
        /// <summary>
        /// [2W(4B)] Guide FR Offset
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 OffsetGuideFR;
        /// <summary>
        /// [2W(4B)] Guide RR Offset
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 OffsetGuideRR;
    }

    /// <summary>
    /// [62] 個体差ダウンロード要求応答
    /// [62] Individual Data Download Report
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_GUIDE_DATA_UPLOAD_REP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code [0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Individual Data Download Request(162)[個体差ダウンロード要求] / Report(62)[報告] */

    #region "Indivisual Data Change Request(63)[個体差データ変更要求] / Response(163)[応答]"

    /// <summary>
    /// [63] 個体差データ変更要求
    /// [63] Indivisual Data Change Request
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_INDIVIDUAL_CHANGE_REQ
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [2W(4B)] Guide FL Offset
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 OffsetGuideFL;
        /// <summary>
        /// [2W(4B)] Guide RL Offset
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 OffsetGuideRL;
        /// <summary>
        /// [2W(4B)] Guide FR Offset
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 OffsetGuideFR;
        /// <summary>
        /// [2W(4B)] Guide RR Offset
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 OffsetGuideRR;
    }

    /// <summary>
    /// [163] 個体差データ変更要求応答
    /// [163] Indivisual Data Change Response
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_INDIVIDUAL_CHANGE_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code	[0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Indivisual Data Change Request(63)[個体差データ変更要求] / Response(163)[応答] */

    #region "Mode Change Request(41)[モード切替要求] / Response(141)[応答]"

    /// <summary>
    /// [41] モード切替要求
    /// [41] Mode Change Request
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_MODE_CHANGE_REQ
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Mode	[0:Auto, 1:Manual]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Mode;
    }

    /// <summary>
    /// [141] モード切替要求応答
    /// [141] Mode Change Response
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_MODE_CHANGE_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code [0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Mode Change Request(41)[モード切替要求] / Response(141)[応答] */

    #region "Status Request(43)[状態要求] / Response(143)[応答]"

    /// <summary>
    /// [43] 状態要求
    /// [43] Status Change Request
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_STATUS_REQ
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
    }

    /// <summary>
    /// [143] 状態要求応答
    /// [143] Status Response
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_STATUS_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Current Segment
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 CurrentSecID;
        /// <summary>
        /// [1W(2B)] Reserve
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Resv1;
        /// <summary>
        /// [1W(2B)] Current Address
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 CurrentAdrID;
        /// <summary>
        /// [1W(2B)] Current Power Status
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PwrSts;
        /// <summary>
        /// [1W(2B)] Current Vehicle Mode
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Mode;
        /// <summary>
        /// [1W(2B)] Current Action Status
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ActSts;
        /// <summary>
        /// [1W(2B)] Current Blockade Status
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ObstacleSts;
        /// <summary>
        /// [1W(2B)] Current Blockade Status
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 BlockSts;
        /// <summary>
        /// [1W(2B)] Current Pause Status
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PauseSts;
        /// <summary>
        /// [1W(2B)] Current Pause Status
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 HIDSts;
        /// <summary>
        /// [1W(2B)] Reserve
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ErrorFlag;
        /// <summary>
        /// [2W(4B)] Keep Distance
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 KeepDistance;
        /// <summary>
        /// [2W(4B)] Keep Distance
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 TravelDistance;
        /// <summary>
        /// [1W(2B)] Current Pause Status
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 StoppedBlockID;
        /// <summary>
        /// [1W(2B)] Current Pause Status
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 StoppedHIDID;
        /// <summary>
        /// [1W(2B)] Current Blockade Status
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 EarthquakePauseSts;
        /// <summary>
        /// [1W(2B)] Current Blockade Status
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SafetyPauseSts;
        /// <summary>
        /// [1W(2B)] Current Address
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 HasCst;
        /// <summary>
        /// [1W(2B)] Reserve
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Resv2;
        /// <summary>
        /// Command ID
        /// </summary>
        [MarshalAs(UnmanagedType.I8, SizeConst = 1)]
        public UInt64 CmdID;
        /// <summary>
        /// [8W(16B)] Cassette ID
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] CstID;
    }

    #endregion	/* Status Request(43)[状態要求] / Response(143)[応答] */

    #region "Status Change Report(144)[状態変化報告] / Response(44)[応答]"

    /// <summary>
    /// [144] 状態変化報告
    /// [144] Status Change Report
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_STATUS_CHANGE_REP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Current Segment
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 CurrentSecID;
        /// <summary>
        /// [1W(2B)] Reserve
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Resv1;
        /// <summary>
        /// [1W(2B)] Current Address
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 CurrentAdrID;
        /// <summary>
        /// [1W(2B)] Current Power Status
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PwrSts;
        /// <summary>
        /// [1W(2B)] Current Vehicle Mode
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Mode;
        /// <summary>
        /// [1W(2B)] Current Action Status
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ActSts;
        /// <summary>
        /// [1W(2B)] Current Blockade Status
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ObstacleSts;
        /// <summary>
        /// [1W(2B)] Current Blockade Status
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 BlockSts;
        /// <summary>
        /// [1W(2B)] Current Pause Status
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PauseSts;
        /// <summary>
        /// [1W(2B)] Current Pause Status
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 HIDSts;
        /// <summary>
        /// [1W(2B)] Reserve
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ErrorFlag;
        /// <summary>
        /// [2W(4B)] Keep Distance
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 KeepDistance;
        /// <summary>
        /// [2W(4B)] Keep Distance
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 TravelDistance;
        /// <summary>
        /// [1W(2B)] Current Pause Status
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 StoppedBlockID;
        /// <summary>
        /// [1W(2B)] Current Pause Status
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 StoppedHIDID;
        /// <summary>
        /// [1W(2B)] Current Blockade Status
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 EarthquakePauseSts;
        /// <summary>
        /// [1W(2B)] Current Blockade Status
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SafetyPauseSts;
        /// <summary>
        /// [1W(2B)] Current Address
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 HasCst;
        /// <summary>
        /// [1W(2B)] Reserve
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Resv2;
        /// <summary>
        /// Command ID
        /// </summary>
        [MarshalAs(UnmanagedType.I8, SizeConst = 1)]
        public UInt64 CmdID;
        /// <summary>
        /// [8W(16B)] Cassette ID
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] CstID;

    }

    /// <summary>
    /// [44] 状態変化報告
    /// [44] Status Change Response
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_STATUS_CHANGE_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code [0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Status Change Report(144)[状態変化要求] / Response(44)[応答] */

    #region "Power Operation Request(45)[動力操作要求] / Response(145)[応答]"

    /// <summary>
    /// [45] 動力操作要求
    /// [45] Power Operation Request
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_POWER_OPE_REQ
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Power Mode	[0:OFF, 1:ON]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PwrMode;
    }

    /// <summary>
    /// [145] 動力操作要求応答
    /// [145] Power Operation Response
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_POWER_OPE_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code [0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Power Operation Request(45)[動力操作要求] / Response(145)[応答] */

    #region "Transfer Request(31)[搬送指示要求] / Response(131)[応答]"

    /// <summary>
    /// [31] 搬送指示要求
    /// [31] Transfer Request
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_TRANS_REQ
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Request Type	[0:Move, 1:Load, 2:Unload, 3:From/To, 4:Home]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ReqType;
        /// <summary>
        /// [1W(2B)] Section Count
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SecCount;
        /// <summary>
        /// [8W(16B)] Cassette ID
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] CstID;
        /// <summary>
        /// Command ID
        /// </summary>
        [MarshalAs(UnmanagedType.I8, SizeConst = 1)]
        public UInt64 CmdID;
        /// <summary>
        /// [1W(2B)] From Address
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 FromAddr;
        /// <summary>
        /// [1W(2B)] To Address
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 ToAddr;
        ///// <summary>
        ///// [1W(2B)] Pass Segment2
        ///// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 23)]
        public UInt16[] Sections;
        /// <summary>
        /// [1W(2B)] Multi Packet Flag
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 MultiFlag;

    }

    /// <summary>
    /// [131] 搬送指示要求応答
    /// [131] Transfer Response
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_TRANS_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Rev1
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Rev1;
        /// <summary>
        /// [1W(2B)] Rev2
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Rev2;
        /// <summary>
        /// Command ID
        /// </summary>
        [MarshalAs(UnmanagedType.I8, SizeConst = 1)]
        public UInt64 CmdID;
        /// <summary>
        /// [1W(2B)] Response Code [0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Transfer Request(31)[状態要求] / Response(131)[応答] */

    #region "Transfer Complete Report(132)[搬送完了報告] / Response(32)[応答]"

    /// <summary>
    /// [132] 搬送完了報告
    /// [132] Transfer Complete Report
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_TRANS_COMP_REP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Rev1
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Rev1;
        /// <summary>
        /// [1W(2B)] Rev2
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Rev2;
        /// <summary>
        /// Command ID
        /// </summary>
        [MarshalAs(UnmanagedType.I8, SizeConst = 1)]
        public UInt64 CmdID;
        /// <summary>
        /// [1W(2B)] Complete Status	[0:Normal, 1:Cancel Comp, 2:Abort Comp]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 CompSts;
        /// <summary>
        /// [1W(2B)] Segment
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Section;
        /// <summary>
        /// [1W(2B)] Address
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 Address;
        /// <summary>
        /// [8W(16B)] Cassette ID
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] CstID;
        /// <summary>
        /// Command ID
        /// </summary>
        [MarshalAs(UnmanagedType.I8, SizeConst = 1)]
        public UInt64 TotalTravelDis;
    }

    /// <summary>
    /// [32] 搬送完了報告応答
    /// [32] Transfer Complete Response
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_TRANS_COMP_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code [0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Transfer Complete Report(132)[搬送完了報告] / Response(32)[応答] */


    #region "Transfer Pass Event Report(134)[搬送イベント報告] / Response(34)[応答]"

    /// <summary>
    /// [134] 搬送イベント報告
    /// [134] Transfer Event Report
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_TRANS_PASS_EVENT_REP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Event Type		[0:From Arrived, 1:Load Comp, 2:To Arrived, 3:Drive Stop, 4:Drive Restart, 5:Blockade Req, 6:Pass Address]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 EventType;
        /// <summary>
        /// [2W(4B)] Keep Distance
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 CurrentSecID;
        /// <summary>
        /// [2W(4B)] Keep Distance
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 CurrentAdrID;
        /// <summary>
        /// [1W(2B)] Current distance on section
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 Sec_Distance;
    }
    //no 34 [34] Transfer Pass Event Response

    #endregion	/* Transfer Pass Event Report(134)[搬送イベント報告] / Response(34)[応答] */

    #region "Pause Request(35)[一時停止要求] / Response(135)[応答]"

    /// <summary>
    /// [35] CST ID 重新命名
    /// [35] CST ID Rename
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_CSTID_RENAME_REQ
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [8W(16B)] Cassette ID
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] CstID;
    }

    /// <summary>
    /// [135] CST ID 重新命名要求応答
    /// [135] CST ID Rename Response
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_CSTID_RENAME_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code	[0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Pause Request(39)[一時停止要求] / Response(139)[応答] */


    #region "Transfer Event Report(136)[搬送イベント報告] / Response(34)[応答]"

    /// <summary>
    /// [136] 搬送イベント報告
    /// [136] Transfer Event Report
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_TRANS_EVENT_REP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Event Type		[0:From Arrived, 1:Load Comp, 2:To Arrived, 3:Drive Stop, 4:Drive Restart, 5:Blockade Req, 6:Pass Address]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 EventType;
        /// <summary>
        /// [2W(4B)] Keep Distance
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 CurrentSecID;
        /// <summary>
        /// [2W(4B)] Keep Distance
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 CurrentAdrID;
        /// <summary>
        /// [2W(4B)] Keep Distance
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Request_Block_ID;
        /// <summary>
        /// [2W(4B)] Keep Distance
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Request_HID_ID;
        /// <summary>
        /// [1W(2B)] Current Have Cassette Status
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] CstID;
        /// <summary>
        /// [2W(4B)] Keep Distance
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 BlockReleaseAdrID;
        /// <summary>
        /// [2W(4B)] Keep Distance
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 HIDReleaseAdrID;
    }

    /// <summary>
    /// [36] 搬送イベント報告応答
    /// [36] Transfer Event Response
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_TRANS_EVENT_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code	[0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Is_Block_Pass;
        /// <summary>
        /// [1W(2B)] Response Code	[0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Is_HID_Pass;
        /// <summary>
        /// [1W(2B)] Response Code	[0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ReplyCode;

    }

    #endregion	/* Transfer Event Report(136)[搬送イベント報告] / Response(34)[応答] */




    #region "Transfer Cancel Request(37)[搬送中止要求] / Response(137)[応答]"

    /// <summary>
    /// [37] 搬送中止要求
    /// [37] Transfer Cancel Request
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_TRANS_CANCEL_REQ
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Request Type	[0:Cancel, 1:Abort]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Rev1;
        /// <summary>
        /// [1W(2B)] Request Type	[0:Cancel, 1:Abort]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Rev2;
        /// <summary>
        /// Command ID
        /// </summary>
        [MarshalAs(UnmanagedType.I8, SizeConst = 1)]
        public UInt64 CmdID;
        /// <summary>
        /// [1W(2B)] Request Type	[0:Cancel, 1:Abort]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ReqType;
    }

    /// <summary>
    /// [137] 搬送中止要求応答
    /// [137] Transfer Change Response
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_TRANS_CANCEL_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Request Type	[0:Cancel, 1:Abort]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Rev1;
        /// <summary>
        /// [1W(2B)] Request Type	[0:Cancel, 1:Abort]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Rev2;
        /// <summary>
        /// Command ID
        /// </summary>
        [MarshalAs(UnmanagedType.I8, SizeConst = 1)]
        public UInt64 CmdID;
        /// <summary>
        /// [1W(2B)] Request Type	[0:Cancel, 1:Abort]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ReqType;
        /// <summary>
        /// [1W(2B)] Response Code	[0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Transfer Cancel Request(37)[搬送中止要求] / Response(137)[応答] */

    #region "Pause Request(39)[一時停止要求] / Response(139)[応答]"

    /// <summary>
    /// [39] 一時停止要求
    /// [39] Pause Request
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_PAUSE_REQ
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Request Type	[0:Restart, 1:Pause]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 EventType;
        /// <summary>
        /// [1W(2B)] Request Type	[0:Restart, 1:Pause]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PauseType;
    }

    /// <summary>
    /// [139] 一時停止要求応答
    /// [139] Pause Response
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_PAUSE_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code	[0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Pause Request(39)[一時停止要求] / Response(139)[応答] */

    #region "Section Teaching Request(71)[区間ティーチング要求] / Response(171)[応答]"

    /// <summary>
    /// [71] 区間ティーチング要求
    /// [71] Section Teaching Request
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_TEACHING_REQ
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Start Address
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 FromAddress;
        /// <summary>
        /// [1W(2B)] End Address
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 ToAddress;
    }

    /// <summary>
    /// [171] 区間ティーチング要求応答
    /// [171] Section Teaching Response
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_TEACHING_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code	[0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Section Teaching Request(71)[区間ティーチング要求] / Response(171)[応答] */

    #region "Section Teaching Complete Report(172)[区間ティーチング完了報告] / Response(72)[応答]"

    /// <summary>
    /// [172] 区間ティーチング完了報告
    /// [172] Section Teaching Complete Report
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_TEACHING_COMP_REP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Address
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 FromAddress;
        /// <summary>
        /// [1W(2B)] Address
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 ToAddress;
        /// <summary>
        /// [2W(4B)] Distance
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 Distance;
        /// <summary>
        /// [1W(2B)] Complete Code		[0:Normal, Not 0:Error]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 CompCode;
    }

    /// <summary>
    /// [72] 区間ティーチング完了報告応答
    /// [72] Section Teaching Complete Response
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_TEACHING_COMP_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code [0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Section Teaching Complete Report(172)[区間ティーチング完了報告] / Response(72)[応答] */

    #region "Address Teaching Report(174)[位置決め位置変更報告] / Response(74)[応答]"

    /// <summary>
    /// [174] 位置変更報告
    /// [174] Address Teaching Report
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_ADDRESS_TEACH_REP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Address
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 Address;
        /// <summary>
        /// [2W(4B)] Reserve
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 Position;
    }

    /// <summary>
    /// [74] 位置決め位置変更報告応答
    /// [74] Address Teaching Response
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_ADDRESS_TEACH_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code [0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Address Teaching Report(174)[位置決め位置変更報告] / Response(74)[応答] */

    #region "Alarm Reset Request(91)[異常解除要求] / Response(191)[応答]"

    /// <summary>
    /// [91] 異常解除要求
    /// [91] Alarm Reset Request
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_ALARM_RESET_REQ
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
    }

    /// <summary>
    /// [191] 異常解除要求応答
    /// [191] Alarm Reset Response
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_ALARM_RESET_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code	[0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Alarm Reset Request(91)[異常解除要求] / Response(191)[応答] */

    #region "Alarm Report(194)[異常報告] / Response(94)[応答]"

    /// <summary>
    /// [194] 異常報告
    /// [194] Alarm Report
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_ALARM_REP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Error Code		[0:Normal, Not 0:Error]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ErrCode;
        /// <summary>
        /// [1W(2B)] Error Statis		[0:Reset, a:Set]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 ErrStatus;
    }

    /// <summary>
    /// [94] 異常報告応答
    /// [94] Alarm Response
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_ALARM_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code [0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Alarm Report(194)[異常報告] / Response(94)[応答] */

    #region "Log Upload Request(95)[ログアップロード要求] / Response(195)[応答]"

    /// <summary>
    /// [95] ログアップロード要求
    /// [95] Log Upload Request
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_LOG_UPLOAD_REQ
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
    }

    /// <summary>
    /// [195] ログアップロード要求応答
    /// [195] Log Upload Response
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_LOG_UPLOAD_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code	[0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Log Upload Request(95)[ログアップロード要求] / Response(195)[応答] */

    #region "Log Data Report(196)[ログデータ報告] / Response(96)[応答]"

    /// <summary>
    /// [196] ログデータ報告
    /// [196] Log Data Report
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_LOGDATA_REP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [32W(64B)] Log Data
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public UInt16[] LogDatas;
        /// <summary>
        /// [1W(2B)] Multi Packet
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 MultiFlag;
    }

    /// <summary>
    /// [96] ログデータ報告応答
    /// [96] Log Data Response
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_LOGDATA_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code	[0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Log Data Report(196)[ログデータ報告] / Response(96)[応答] */

    #region "Vehicle Communication Test Request(198)[台車通信テスト要求] / Response(98)[応答]"

    /// <summary>
    /// [198] 台車通信テスト要求
    /// [198] Vehicle Communication Test Request
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_VHCL_COMM_TEST_REQ
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Test Word Data
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 TestDataW;
        /// <summary>
        /// [1W(2B)] Reserve
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Resv1;
        /// <summary>
        /// [2W(4B)] Test DWord Data
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 TestDataDW;
        /// <summary>
        /// [4W(8B)] Test Ascii Data
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public char[] TestDataASC;
    }

    /// <summary>
    /// [98] 台車通信テスト要求応答
    /// [98] Vehicle Communication Test Response
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_VHCL_COMM_TEST_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code [0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Vehicle Communication Test Request(198)[台車通信テスト要求] / Response(98)[応答] */

    #region "Host Communication Test Request(99)[ホスト通信テスト要求] / Response(199)[応答]"

    /// <summary>
    /// [99] ホスト通信テスト要求
    /// [99] Host Communication Test Request
    /// (C) -> (V)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_HOST_COMM_TEST_REQ
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Test Word Data
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 TestDataW;
        /// <summary>
        /// [1W(2B)] Reserve
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 Resv1;
        /// <summary>
        /// [2W(4B)] Test DWord Data
        /// </summary>
        [MarshalAs(UnmanagedType.I4, SizeConst = 1)]
        public UInt32 TestDataDW;
        /// <summary>
        /// [4W(8B)] Test Ascii Data
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public char[] TestDataASC;
    }

    /// <summary>
    /// [199] ホスト通信テスト要求応答
    /// [199] Host Communication Test Response
    /// (V) -> (C)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_VHMSG_HOST_COMM_TEST_RESP
    {
        /// <summary>
        /// [1W(2B)] Packet ID
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 PacketID;
        /// <summary>
        /// [1W(2B)] Sequence Number
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 SeqNum;
        /// <summary>
        /// [1W(2B)] Response Code [0:OK, 1:NG]
        /// </summary>
        [MarshalAs(UnmanagedType.I2, SizeConst = 1)]
        public UInt16 RespCode;
    }

    #endregion	/* Host Communication Test Request(99)[ホスト通信テスト要求] / Response(199)[応答] */

}
