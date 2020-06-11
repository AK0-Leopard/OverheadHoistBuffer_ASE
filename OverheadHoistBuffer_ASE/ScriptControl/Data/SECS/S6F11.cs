using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;
using com.mirle.ibg3k0.stc.Data.SecsData;

namespace com.mirle.ibg3k0.sc.Data.SECS
{
    /// <summary>
    /// Event Report Send
    /// </summary>
    [Serializable]
    public class S6F11 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11()
        {
            StreamFunction = "S6F11";
            StreamFunctionName = "Transfer Event repor";
            W_Bit = 1;
            IsBaseType = true;
            INFO = new RPTINFO();
        }
        public override string ToString()
        {
            return string.Concat(StreamFunction, "-", CEID);
        }

        [Serializable]
        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1, ListSpreadOut = true)]
            public RPTITEM[] ITEM;
            [Serializable]
            public class RPTITEM : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                public string RPTID;
                [SecsElement(Index = 2)]
                public SXFY[] VIDITEM;

                [Serializable]
                public class VIDITEM_04 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, ListElementType = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, ListElementLength = 1)]
                    public string[] ALIDs;
                    public VIDITEM_04()
                    {
                        ALIDs = new string[0];
                    }
                }

                [Serializable]
                public class VIDITEM_06 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string CONTROLSTATE;
                    public VIDITEM_06()
                    {
                        CONTROLSTATE = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_10 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_54 CARRIER_ID_OBJ;
                    [SecsElement(Index = 2, ListSpreadOut = true)]
                    public VIDITEM_56 CARRIER_LOC_OBJ;
                    [SecsElement(Index = 2, ListSpreadOut = true)]
                    public VIDITEM_12 INSTALL_TIME_OBJ;
                    [SecsElement(Index = 3, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string CARRIER_ZONE_NAME;
                    [SecsElement(Index = 4, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string CARRIER_STATE;

                    public VIDITEM_10()
                    {
                        CARRIER_ID_OBJ = new VIDITEM_54();
                        CARRIER_LOC_OBJ = new VIDITEM_56();
                        INSTALL_TIME_OBJ = new VIDITEM_12();
                        CARRIER_ZONE_NAME = String.Empty;
                        CARRIER_STATE = String.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_11 : SXFY
                {
                    [SecsElement(Index = 1)]
                    public VIDITEM_58 COMMAND_ID;
                    [SecsElement(Index = 2)]
                    public VIDITEM_62 PRIORITY;
                    public VIDITEM_11()
                    {
                        COMMAND_ID = new VIDITEM_58();
                        PRIORITY = new VIDITEM_62();
                    }
                }

                [Serializable]
                public class VIDITEM_12 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 16)]
                    public string INSTALLTIME;
                    public VIDITEM_12()
                    {
                        INSTALLTIME = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_13 : SXFY
                {
                    [SecsElement(Index = 1)]
                    public VIDITEM_722 TRANSFER_STATE;
                    [SecsElement(Index = 1)]
                    public VIDITEM_11 COMMAND_INFO;
                    [SecsElement(Index = 1)]
                    public VIDITEM_84 TRANSFER_INFO;
                    public VIDITEM_13()
                    {
                        TRANSFER_STATE = new VIDITEM_722();
                        COMMAND_INFO = new VIDITEM_11();
                        TRANSFER_INFO = new VIDITEM_84();
                    }
                }


                [Serializable]
                public class VIDITEM_51 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_10[] ENHANCED_CARRIER_INFO;
                }


                [Serializable]
                public class VIDITEM_53 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_71[] VEHICLEINFO;
                }


                [Serializable]
                public class VIDITEM_54 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string CARRIER_ID;
                    public VIDITEM_54()
                    {
                        CARRIER_ID = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_56 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string CARRIER_LOC;
                    public VIDITEM_56()
                    {
                        CARRIER_LOC = string.Empty;
                    }
                }


                [Serializable]
                public class VIDITEM_58 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string COMMAND_ID;
                    public VIDITEM_58()
                    {
                        COMMAND_ID = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_59 : SXFY
                {
                    [SecsElement(Index = 1)]
                    public VIDITEM_58 COMMAND_ID;
                    [SecsElement(Index = 2)]
                    public VIDITEM_62 PRIORITY;
                    public VIDITEM_59()
                    {
                        COMMAND_ID = new VIDITEM_58();
                        PRIORITY = new VIDITEM_62();
                    }
                }
                [Serializable]
                public class VIDITEM_60 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string DESTINATION_PORT;
                    public VIDITEM_60()
                    {
                        DESTINATION_PORT = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_62 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string PRIORITY;
                    public VIDITEM_62()
                    {
                        PRIORITY = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_64 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string RESULT_CODE;
                    public VIDITEM_64()
                    {
                        RESULT_CODE = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_65 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string SOURCE_PORT;
                    public VIDITEM_65()
                    {
                        SOURCE_PORT = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_66 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string HANDOFF_TYPE;
                    public VIDITEM_66()
                    {
                        HANDOFF_TYPE = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_67 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string ID_READ_STATUS;
                    public VIDITEM_67()
                    {
                        ID_READ_STATUS = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_70 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string VEHILCE_ID;
                    public VIDITEM_70()
                    {
                        VEHILCE_ID = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_71 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VEHICLEINFO VHINFO;
                    public VIDITEM_71()
                    {
                        VHINFO = new VEHICLEINFO()
                        {
                            VEHICLE_ID = string.Empty,
                            VEHICLE_STATE = string.Empty
                        };
                    }
                    [Serializable]
                    public class VEHICLEINFO : SXFY
                    {
                        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                        public string VEHICLE_ID;
                        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                        public string VEHICLE_STATE;
                    }
                }
                [Serializable]
                public class VIDITEM_72 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string VEHICLE_STATE;
                    public VIDITEM_72()
                    {
                        VEHICLE_STATE = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_73 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string SCSTATE;
                    public VIDITEM_73()
                    {
                        SCSTATE = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_76 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public VIDITEM_13[] EnhancedTransferCmd;
                }
                [Serializable]
                public class VIDITEM_77 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public TRANSFERCOMPLETEINFO[] TRANCOMPLETEINFO;
                    public VIDITEM_77()
                    {
                        TRANCOMPLETEINFO = new TRANSFERCOMPLETEINFO[1];
                        TRANCOMPLETEINFO[0] = new TRANSFERCOMPLETEINFO();
                    }
                    [Serializable]
                    public class TRANSFERCOMPLETEINFO : SXFY
                    {
                        [SecsElement(Index = 1, ListSpreadOut = true)]
                        public TRANSFERINFO TRANINFO;
                        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                        public string CARRIER_LOC;
                        public TRANSFERCOMPLETEINFO()
                        {
                            TRANINFO = new TRANSFERINFO();
                            CARRIER_LOC = string.Empty;
                        }
                        [Serializable]
                        public class TRANSFERINFO : SXFY
                        {
                            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                            public string CARRIER_ID;
                            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                            public string SOURCE_PORT;
                            [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                            public string DESTINATION_PORT;
                            public TRANSFERINFO()
                            {
                                CARRIER_ID = string.Empty;
                                SOURCE_PORT = string.Empty;
                                DESTINATION_PORT = string.Empty;
                            }
                        }
                    }
                }
                [Serializable]
                public class VIDITEM_80 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string COMMAND_TYPE;
                    public VIDITEM_80()
                    {
                        COMMAND_TYPE = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_81 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string ALARM_ID;
                    public VIDITEM_81()
                    {
                        ALARM_ID = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_82 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string ALARM_TEXT;
                    public VIDITEM_82()
                    {
                        ALARM_TEXT = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_83 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string UNIT_ID;
                    public VIDITEM_83()
                    {
                        UNIT_ID = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_84 : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string CARRIER_ID;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string SOURCE_PORT;
                    [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string DESTINATION_PORT;
                    public VIDITEM_84()
                    {
                        CARRIER_ID = string.Empty;
                        SOURCE_PORT = string.Empty;
                        DESTINATION_PORT = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_114 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string SPEC_VERSION;
                    public VIDITEM_114()
                    {
                        SPEC_VERSION = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_115 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string PORT_ID;
                    public VIDITEM_115()
                    {
                        PORT_ID = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_118 : SXFY
                {
                    [SecsElement(Index = 1)]
                    public VIDITEM_354[] PORT_INFO;
                }

                [Serializable]
                public class VIDITEM_350 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public VIDITEM_354 PORT_INFO;
                }
                [Serializable]
                public class VIDITEM_351 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_115 PORT_ID;
                    [SecsElement(Index = 2, ListSpreadOut = true)]
                    public VIDITEM_355 PORT_TRANSFER_STATE;
                    [SecsElement(Index = 3, ListSpreadOut = true)]
                    public VIDITEM_352 EQ_REQ_STATUS;
                    [SecsElement(Index = 4, ListSpreadOut = true)]
                    public VIDITEM_353 EQ_PRESENCE_STATUS;
                }

                [Serializable]
                public class VIDITEM_353 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string EQ_PRESENCE_STATUS;
                    public VIDITEM_353()
                    {
                        EQ_PRESENCE_STATUS = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_352 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string EQ_REQ_STATUS;
                    public VIDITEM_352()
                    {
                        EQ_REQ_STATUS = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_354 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_115 PORT_ID;
                    [SecsElement(Index = 2, ListSpreadOut = true)]
                    public VIDITEM_355 PORT_TRANSFTER_STATE;
                    public VIDITEM_354()
                    {
                        PORT_ID = new VIDITEM_115();
                        PORT_TRANSFTER_STATE = new VIDITEM_355();
                    }
                }

                [Serializable]
                public class VIDITEM_355 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string PORT_TRANSFER_STATE;
                    public VIDITEM_355()
                    {
                        PORT_TRANSFER_STATE = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_360 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_361[] POER_ID;
                }


                [Serializable]
                public class VIDITEM_361 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_83 UNIT_ID;
                    [SecsElement(Index = 2, ListSpreadOut = true)]
                    public VIDITEM_81 ALARM_ID;
                    [SecsElement(Index = 2, ListSpreadOut = true)]
                    public VIDITEM_82 ALARM_TEXT;
                    [SecsElement(Index = 2, ListSpreadOut = true)]
                    public VIDITEM_362 MAINT_STATE;
                    public VIDITEM_361()
                    {
                        UNIT_ID = new VIDITEM_83();
                        ALARM_ID = new VIDITEM_81();
                        ALARM_TEXT = new VIDITEM_82();
                        MAINT_STATE = new VIDITEM_362();
                    }
                }

                [Serializable]
                public class VIDITEM_362 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string MAINT_STATE;
                    public VIDITEM_362()
                    {
                        MAINT_STATE = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_363 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string VEHICLE_CURRENT_POSITION;
                    public VIDITEM_363()
                    {
                        VEHICLE_CURRENT_POSITION = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_722 : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string TRANSFER_STATE;
                    public VIDITEM_722()
                    {
                        TRANSFER_STATE = string.Empty;
                    }
                }
            }
        }
    }
    public class VIDCollection
    {
        public VIDCollection()
        {
            VID_06_ControlState = new S6F11.RPTINFO.RPTITEM.VIDITEM_06();
            VID_10_EnhancedCarrierInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_10();
            VID_11_CommandInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_11();
            VID_13_EnhancedTransferCmd = new S6F11.RPTINFO.RPTITEM.VIDITEM_13();
            VID_53_ActiveVehicles = new S6F11.RPTINFO.RPTITEM.VIDITEM_53();
            VID_54_CarrierID = new S6F11.RPTINFO.RPTITEM.VIDITEM_54();
            VID_56_CarrierLoc = new S6F11.RPTINFO.RPTITEM.VIDITEM_56();
            VID_58_CommandID = new S6F11.RPTINFO.RPTITEM.VIDITEM_58();
            VID_59_CommandInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_59();
            VID_60_DestinationPort = new S6F11.RPTINFO.RPTITEM.VIDITEM_60();
            VID_62_Priotity = new S6F11.RPTINFO.RPTITEM.VIDITEM_62();
            VID_64_ResultCode = new S6F11.RPTINFO.RPTITEM.VIDITEM_64();
            VID_65_SourcePort = new S6F11.RPTINFO.RPTITEM.VIDITEM_65();
            VID_66_HandoffType = new S6F11.RPTINFO.RPTITEM.VIDITEM_66();
            VID_67_IDReadStatus = new S6F11.RPTINFO.RPTITEM.VIDITEM_67();
            VID_70_VehicleID = new S6F11.RPTINFO.RPTITEM.VIDITEM_70();
            VID_71_VehicleInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_71();
            VID_72_VehicleStatus = new S6F11.RPTINFO.RPTITEM.VIDITEM_72();
            VID_73_SCState = new S6F11.RPTINFO.RPTITEM.VIDITEM_73();
            VID_77_TranCmpInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_77();
            VID_80_CommmandType = new S6F11.RPTINFO.RPTITEM.VIDITEM_80();
            VID_81_AlarmID = new S6F11.RPTINFO.RPTITEM.VIDITEM_81();
            VID_82_AlarmText = new S6F11.RPTINFO.RPTITEM.VIDITEM_82();
            VID_83_UnitID = new S6F11.RPTINFO.RPTITEM.VIDITEM_83();
            VID_84_TransferInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_84();
            VID_114_SpecVersion = new S6F11.RPTINFO.RPTITEM.VIDITEM_114();
            VID_115_PortID = new S6F11.RPTINFO.RPTITEM.VIDITEM_115();
            VID_353_EqPresenceStatus = new S6F11.RPTINFO.RPTITEM.VIDITEM_353();
            VID_354_PortInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_354();
            VID_355_PortTransferState = new S6F11.RPTINFO.RPTITEM.VIDITEM_355();
            VID_361_UnitAlarmInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_361();
            VID_362_MainState = new S6F11.RPTINFO.RPTITEM.VIDITEM_362();
            VID_363_VehicleCurrenyPosition = new S6F11.RPTINFO.RPTITEM.VIDITEM_363();
            VID_722_TransferState = new S6F11.RPTINFO.RPTITEM.VIDITEM_722();
        }
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_06 VID_06_ControlState;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_10 VID_10_EnhancedCarrierInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_11 VID_11_CommandInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_13 VID_13_EnhancedTransferCmd;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_53 VID_53_ActiveVehicles;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_54 VID_54_CarrierID;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_56 VID_56_CarrierLoc;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_58 VID_58_CommandID;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_59 VID_59_CommandInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_60 VID_60_DestinationPort;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_62 VID_62_Priotity;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_64 VID_64_ResultCode;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_65 VID_65_SourcePort;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_66 VID_66_HandoffType;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_67 VID_67_IDReadStatus;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_70 VID_70_VehicleID;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_71 VID_71_VehicleInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_72 VID_72_VehicleStatus;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_73 VID_73_SCState;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_77 VID_77_TranCmpInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_80 VID_80_CommmandType;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_81 VID_81_AlarmID;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_82 VID_82_AlarmText;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_83 VID_83_UnitID;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_84 VID_84_TransferInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_114 VID_114_SpecVersion;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_115 VID_115_PortID;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_353 VID_353_EqPresenceStatus;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_354 VID_354_PortInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_355 VID_355_PortTransferState;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_361 VID_361_UnitAlarmInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_362 VID_362_MainState;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_363 VID_363_VehicleCurrenyPosition;
        public com.mirle.ibg3k0.sc.Data.SECS.S6F11.RPTINFO.RPTITEM.VIDITEM_722 VID_722_TransferState;
    }

}
