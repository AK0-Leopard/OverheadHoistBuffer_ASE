using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;
using com.mirle.ibg3k0.stc.Data.SecsData;

namespace com.mirle.ibg3k0.sc.Data.SECS.CSOT
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
            //public RPTITEM ITEM;//從陣列改成單個 Markchou 20190313
            [Serializable]
            public class RPTITEM : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                public string RPTID;
                [SecsElement(Index = 2)]
                public SXFY[] VIDITEM;

                [Serializable]
                public class VIDITEM_107_EnhancedActiveZones : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_172_ZoneData[] ZoneData;
                }

                [Serializable]
                public class VIDITEM_172_ZoneData: SXFY
                {
                    //[SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    //public string ALARM_ID;
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string ZoneName;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string ZoneCapacity;
                    [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string ZoneTotalSize;
                    [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string ZoneType;
                    [SecsElement(Index = 5)]
                    public VIDITEM_888_DisabledLocations DisabledLocations;

                    public VIDITEM_172_ZoneData()
                    {
                    }
                }
                [Serializable]
                public class VIDITEM_888_DisabledLocations : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_889_DisabledLoc[] DisabledLoc;

                    public VIDITEM_888_DisabledLocations()
                    {
                        //DisabledLoc = new VIDITEM_889_DisabledLoc[2];
                    }
                }
                [Serializable]
                public class VIDITEM_889_DisabledLoc : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string CarrierLoc;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string CarrierID;
                }

                [Serializable]
                public class VIDITEM_51_SV2 : SXFY
                {
                    //[SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    //public string ALARM_ID;
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string CarrierID;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string CarrierLoc;
                    [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string CarrierZoneName;
                    [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string InstallTime;
                    [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string CarrierState;
                    [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string BOXID;

                    public VIDITEM_51_SV2()
                    {
                        CarrierID = string.Empty;
                        CarrierLoc = string.Empty;
                        CarrierZoneName = string.Empty;
                        InstallTime = string.Empty;
                        CarrierState = string.Empty;
                        BOXID = string.Empty;
                        //ALARM_ID = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_01_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string ALARM_ID;
                    public VIDITEM_01_DVVAL()
                    {
                        ALARM_ID = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_02_ECV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string ESTABLISH_COMMU_TIMEOUT;
                    public VIDITEM_02_ECV()
                    {
                        ESTABLISH_COMMU_TIMEOUT = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_03_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, ListElementType = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, ListElementLength = 1)]
                    public string[] ALIDs;
                    public VIDITEM_03_SV()
                    {
                        ALIDs = new string[0];
                    }
                }


                [Serializable]
                public class VIDITEM_04_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, ListElementType = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, ListElementLength = 1)]
                    public string[] ALIDs;
                    public VIDITEM_04_SV()
                    {
                        ALIDs = new string[0];
                    }
                }

                [Serializable]
                public class VIDITEM_05_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 16)]
                    public string CLOCK;
                    public VIDITEM_05_SV()
                    {
                        CLOCK = string.Empty;
                    }
                }


                [Serializable]
                public class VIDITEM_06_SV : SXFY
                {
                    //[SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]//變更Type為TYPE_BINARY MarkChou 20190313
                    public string CONTROLSTATE; //1=Eq.Offline 2=Going Online 3=Host Offline 4=Online-local 5=Online-remote
                    public VIDITEM_06_SV()
                    {
                        CONTROLSTATE = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_07_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, ListElementType = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, ListElementLength = 1)]
                    public string[] CEIDs;
                    public VIDITEM_07_SV()
                    {
                        CEIDs = new string[0];
                    }
                }

                //[Serializable]
                //public class VIDITEM_10 : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true)]
                //    public VIDITEM_54 CARRIER_ID_OBJ;
                //    [SecsElement(Index = 2, ListSpreadOut = true)]
                //    public VIDITEM_56 CARRIER_LOC_OBJ;
                //    [SecsElement(Index = 2, ListSpreadOut = true)]
                //    public VIDITEM_12 INSTALL_TIME_OBJ;
                //    [SecsElement(Index = 3, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //    public string CARRIER_ZONE_NAME;
                //    [SecsElement(Index = 4, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //    public string CARRIER_STATE;

                //    public VIDITEM_10()
                //    {
                //        CARRIER_ID_OBJ = new VIDITEM_54();
                //        CARRIER_LOC_OBJ = new VIDITEM_56();
                //        INSTALL_TIME_OBJ = new VIDITEM_12();
                //        CARRIER_ZONE_NAME = String.Empty;
                //        CARRIER_STATE = String.Empty;
                //    }
                //}

                //[Serializable]
                //public class VIDITEM_11 : SXFY
                //{
                //    [SecsElement(Index = 1)]
                //    public VIDITEM_58 COMMAND_ID;
                //    [SecsElement(Index = 2)]
                //    public VIDITEM_62 PRIORITY;
                //    public VIDITEM_11()
                //    {
                //        COMMAND_ID = new VIDITEM_58();
                //        PRIORITY = new VIDITEM_62();
                //    }
                //}

                //[Serializable]
                //public class VIDITEM_12 : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 16)]
                //    public string INSTALLTIME;
                //    public VIDITEM_12()
                //    {
                //        INSTALLTIME = string.Empty;
                //    }
                //}

                //[Serializable]
                //public class VIDITEM_13 : SXFY
                //{
                //    [SecsElement(Index = 1)]
                //    public VIDITEM_722 TRANSFER_STATE;
                //    [SecsElement(Index = 1)]
                //    public VIDITEM_11 COMMAND_INFO;
                //    [SecsElement(Index = 1)]
                //    public VIDITEM_84 TRANSFER_INFO;
                //    public VIDITEM_13()
                //    {
                //        TRANSFER_STATE = new VIDITEM_722();
                //        COMMAND_INFO = new VIDITEM_11();
                //        TRANSFER_INFO = new VIDITEM_84();
                //    }
                //}


                [Serializable]
                public class VIDITEM_51_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    //public VIDITEM_10[] ENHANCED_CARRIER_INFO;
                    public VIDITEM_55_SV[] CARRIER_INFO_OBJ;
                }

                [Serializable]
                public class VIDITEM_52_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    //public VIDITEM_10[] ENHANCED_CARRIER_INFO;
                    public VIDITEM_66_SV[] TRANSFER_COMMAND_OBJ;
                }

                [Serializable]
                public class VIDITEM_53_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_71_SV[] VEHICLEINFO_OBJ;
                }


                [Serializable]
                public class VIDITEM_54_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string CARRIER_ID;
                    public VIDITEM_54_DVVAL()
                    {
                        CARRIER_ID = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_54_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string CARRIER_ID;
                    public VIDITEM_54_SV()
                    {
                        CARRIER_ID = string.Empty;
                    }
                }


                [Serializable]
                public class VIDITEM_55_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_54_DVVAL CARRIER_ID_OBJ;
                    [SecsElement(Index = 2, ListSpreadOut = true)]
                    public VIDITEM_70_DVVAL VEHICLE_ID_OBJ;
                    [SecsElement(Index = 3, ListSpreadOut = true)]
                    public VIDITEM_56_DVVAL CARRIER_LOC_OBJ;

                    public VIDITEM_55_DVVAL()
                    {
                        CARRIER_ID_OBJ = new VIDITEM_54_DVVAL();
                        VEHICLE_ID_OBJ = new VIDITEM_70_DVVAL();
                        CARRIER_LOC_OBJ = new VIDITEM_56_DVVAL();
                    }

                    public string CARRIER_ID
                    {
                        get { return CARRIER_ID_OBJ?.CARRIER_ID; }
                        set { CARRIER_ID_OBJ.CARRIER_ID = value; }
                    }
                    public string VEHICLE_ID
                    {
                        get { return VEHICLE_ID_OBJ?.VEHILCE_ID; }
                        set { VEHICLE_ID_OBJ.VEHILCE_ID = value; }
                    }
                    public string CARRIER_LOC
                    {
                        get { return CARRIER_LOC_OBJ?.CARRIER_LOC; }
                        set { CARRIER_LOC_OBJ.CARRIER_LOC = value; }
                    }
                }


                [Serializable]
                public class VIDITEM_55_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_54_SV CARRIER_ID_OBJ;
                    [SecsElement(Index = 2, ListSpreadOut = true)]
                    public VIDITEM_70_SV VEHICLE_ID_OBJ;
                    [SecsElement(Index = 3, ListSpreadOut = true)]
                    public VIDITEM_56_SV CARRIER_LOC_OBJ;

                    public VIDITEM_55_SV()
                    {
                        CARRIER_ID_OBJ = new VIDITEM_54_SV();
                        VEHICLE_ID_OBJ = new VIDITEM_70_SV();
                        CARRIER_LOC_OBJ = new VIDITEM_56_SV();
                    }
                }



                [Serializable]
                public class VIDITEM_56_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string CARRIER_LOC;
                    public VIDITEM_56_DVVAL()
                    {
                        CARRIER_LOC = string.Empty;
                    }
                }


                [Serializable]
                public class VIDITEM_56_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string CARRIER_LOC;
                    public VIDITEM_56_SV()
                    {
                        CARRIER_LOC = string.Empty;
                    }
                }


                [Serializable]
                public class VIDITEM_57_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string COMMAND_NAME;
                    public VIDITEM_57_DVVAL()
                    {
                        COMMAND_NAME = string.Empty;
                    }
                }


                [Serializable]
                public class VIDITEM_58_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string COMMAND_ID;
                    public VIDITEM_58_DVVAL()
                    {
                        COMMAND_ID = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_58_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string COMMAND_ID;
                    public VIDITEM_58_SV()
                    {
                        COMMAND_ID = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_59_DVVAL : SXFY
                {
                    [SecsElement(Index = 1)]
                    public VIDITEM_58_DVVAL COMMAND_ID_OBJ;
                    [SecsElement(Index = 2)]
                    public VIDITEM_62_DVVAL PRIORITY_OBJ;
                    [SecsElement(Index = 3)]
                    public VIDITEM_63_DVVAL REPLACE_OBJ;
                    public VIDITEM_59_DVVAL()
                    {
                        COMMAND_ID_OBJ = new VIDITEM_58_DVVAL();
                        PRIORITY_OBJ = new VIDITEM_62_DVVAL();
                        REPLACE_OBJ = new VIDITEM_63_DVVAL();
                    }
                    public string COMMAND_ID
                    {
                        get { return COMMAND_ID_OBJ?.COMMAND_ID; }
                        set { COMMAND_ID_OBJ.COMMAND_ID = value; }
                    }
                    public string PRIORITY
                    {
                        get { return PRIORITY_OBJ?.PRIORITY; }
                        set { PRIORITY_OBJ.PRIORITY = value; }
                    }
                    public string REPLACE
                    {
                        get { return REPLACE_OBJ?.REPLACE; }
                        set { REPLACE_OBJ.REPLACE = value; }
                    }

                }
                [Serializable]
                public class VIDITEM_59_SV : SXFY
                {
                    [SecsElement(Index = 1)]
                    public VIDITEM_58_SV COMMAND_ID_OBJ;
                    [SecsElement(Index = 2)]
                    public VIDITEM_62_SV PRIORITY_OBJ;
                    [SecsElement(Index = 3)]
                    public VIDITEM_63_SV REPLACE_OBJ;
                    public VIDITEM_59_SV()
                    {
                        COMMAND_ID_OBJ = new VIDITEM_58_SV();
                        PRIORITY_OBJ = new VIDITEM_62_SV();
                        REPLACE_OBJ = new VIDITEM_63_SV();
                    }

                }
                [Serializable]
                public class VIDITEM_60_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string DESTINATION_PORT;
                    public VIDITEM_60_DVVAL()
                    {
                        DESTINATION_PORT = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_60_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string DESTINATION_PORT;
                    public VIDITEM_60_SV()
                    {
                        DESTINATION_PORT = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_61_ECV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 80)]
                    public string EQPT_NAME;
                    public VIDITEM_61_ECV()
                    {
                        EQPT_NAME = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_62_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string PRIORITY;
                    public VIDITEM_62_DVVAL()
                    {
                        PRIORITY = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_62_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string PRIORITY;
                    public VIDITEM_62_SV()
                    {
                        PRIORITY = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_63_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string REPLACE;
                    public VIDITEM_63_DVVAL()
                    {
                        REPLACE = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_63_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string REPLACE;
                    public VIDITEM_63_SV()
                    {
                        REPLACE = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_64_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string RESULT_CODE;
                    public VIDITEM_64_DVVAL()
                    {
                        RESULT_CODE = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_65_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string SOURCE_PORT;
                    public VIDITEM_65_DVVAL()
                    {
                        SOURCE_PORT = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_66_DVVAL : SXFY
                {
                    [SecsElement(Index = 1)]
                    public VIDITEM_59_DVVAL COMMAND_INFO;
                    [SecsElement(Index = 2)]
                    public VIDITEM_67_DVVAL[] TRANSFER_INFOS;
                    public VIDITEM_66_DVVAL()
                    {
                        COMMAND_INFO = new VIDITEM_59_DVVAL();
                    }
                }
                [Serializable]
                public class VIDITEM_66_SV : SXFY
                {
                    [SecsElement(Index = 1)]
                    public VIDITEM_59_SV COMMAND_INFO_OBJ;
                    [SecsElement(Index = 2)]
                    public VIDITEM_67_SV[] TRANSFER_INFO_OBJ;
                    public VIDITEM_66_SV()
                    {
                        COMMAND_INFO_OBJ = new VIDITEM_59_SV();
                    }
                }
                [Serializable]
                public class VIDITEM_67_DVVAL : SXFY
                {
                    [SecsElement(Index = 1)]
                    public VIDITEM_54_DVVAL CARRIER_ID_OBJ;
                    [SecsElement(Index = 2)]
                    public VIDITEM_65_DVVAL SOURCE_PORT_OBJ;
                    [SecsElement(Index = 3)]
                    public VIDITEM_60_DVVAL DESTINATION_PORT_OBJ;
                    public VIDITEM_67_DVVAL()
                    {
                        CARRIER_ID_OBJ = new VIDITEM_54_DVVAL();
                        SOURCE_PORT_OBJ = new VIDITEM_65_DVVAL();
                        DESTINATION_PORT_OBJ = new VIDITEM_60_DVVAL();
                    }

                    public string CARRIER_ID
                    {
                        get { return CARRIER_ID_OBJ?.CARRIER_ID; }
                        set { CARRIER_ID_OBJ.CARRIER_ID = value; }
                    }
                    public string SOURCE_PORT
                    {
                        get { return SOURCE_PORT_OBJ?.SOURCE_PORT; }
                        set { SOURCE_PORT_OBJ.SOURCE_PORT = value; }
                    }
                    public string DESTINATION_PORT
                    {
                        get { return DESTINATION_PORT_OBJ?.DESTINATION_PORT; }
                        set { DESTINATION_PORT_OBJ.DESTINATION_PORT = value; }
                    }
                }
                [Serializable]
                public class VIDITEM_67_SV : SXFY
                {
                    [SecsElement(Index = 1)]
                    public VIDITEM_54_SV CARRIER_ID_OBJ;
                    [SecsElement(Index = 2)]
                    public VIDITEM_78_SV SOURCE_PORT_OBJ;
                    [SecsElement(Index = 3)]
                    public VIDITEM_60_SV DESTINATION_PORT_OBJ;
                    public VIDITEM_67_SV()
                    {
                        CARRIER_ID_OBJ = new VIDITEM_54_SV();
                        SOURCE_PORT_OBJ = new VIDITEM_78_SV();
                        DESTINATION_PORT_OBJ = new VIDITEM_60_SV();
                    }

                    public string CARRIER_ID
                    {
                        get { return CARRIER_ID_OBJ?.CARRIER_ID; }
                        set { CARRIER_ID_OBJ.CARRIER_ID = value; }
                    }
                    public string SOURCE_PORT
                    {
                        get { return SOURCE_PORT_OBJ?.SOURCE_PORT; }
                        set { SOURCE_PORT_OBJ.SOURCE_PORT = value; }
                    }
                    public string DESTINATION_PORT
                    {
                        get { return DESTINATION_PORT_OBJ?.DESTINATION_PORT; }
                        set { DESTINATION_PORT_OBJ.DESTINATION_PORT = value; }
                    }

                }
                [Serializable]
                public class VIDITEM_68_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string TRANSFER_PORT;
                    public VIDITEM_68_DVVAL()
                    {
                        TRANSFER_PORT = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_69_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_68_DVVAL[] TRANSFER_PORT_OBJ;
                }
                [Serializable]
                public class VIDITEM_70_DVVAL : SXFY
                {
                    //[SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 6)]//變更長度為6 Markchou 20190313
                    public string VEHILCE_ID;
                    public VIDITEM_70_DVVAL()
                    {
                        VEHILCE_ID = string.Empty;
                    }
                }
                public class VIDITEM_70_SV : SXFY
                {
                    //[SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 6)]//變更長度為6 Markchou 20190313
                    public string VEHILCE_ID;
                    public VIDITEM_70_SV()
                    {
                        VEHILCE_ID = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_71_DVVAL : SXFY
                {
                    [SecsElement(Index = 1)]
                    public VIDITEM_70_DVVAL VEHICLE_ID_OBJ;
                    [SecsElement(Index = 2)]
                    public VIDITEM_72_DVVAL VEHICLE_STATE_OBJ;

                    public VIDITEM_71_DVVAL()
                    {
                        VEHICLE_ID_OBJ = new VIDITEM_70_DVVAL();
                        VEHICLE_STATE_OBJ = new VIDITEM_72_DVVAL();
                    }
                    public string VEHICLE_ID
                    {
                        get { return VEHICLE_ID_OBJ?.VEHILCE_ID; }
                        set { VEHICLE_ID_OBJ.VEHILCE_ID = value; }
                    }

                    public string VEHICLE_STATE
                    {
                        get { return VEHICLE_STATE_OBJ?.VEHICLE_STATE; }
                        set { VEHICLE_STATE_OBJ.VEHICLE_STATE = value; }
                    }

                }
                [Serializable]
                public class VIDITEM_71_SV : SXFY
                {
                    [SecsElement(Index = 1)]
                    public VIDITEM_70_SV VRHICLE_ID_OBJ;
                    [SecsElement(Index = 2)]
                    public VIDITEM_72_SV VRHICLE_STATE_OBJ;

                    public VIDITEM_71_SV()
                    {
                        VRHICLE_ID_OBJ = new VIDITEM_70_SV();
                        VRHICLE_STATE_OBJ = new VIDITEM_72_SV();
                    }
                }
                [Serializable]
                public class VIDITEM_72_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string VEHICLE_STATE;
                    public VIDITEM_72_DVVAL()
                    {
                        VEHICLE_STATE = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_72_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string VEHICLE_STATE;
                    public VIDITEM_72_SV()
                    {
                        VEHICLE_STATE = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_73_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string TSCState;  //1=Init 2=Paused 3=Auto 4=Pausin
                    public VIDITEM_73_SV()
                    {
                        TSCState = string.Empty;
                    }
                }

                public class VIDITEM_74_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string COMMAND_TYPE;
                    public VIDITEM_74_DVVAL()
                    {
                        COMMAND_TYPE = string.Empty;
                    }
                }



                [Serializable]
                public class VIDITEM_75_DVVAL : SXFY
                {
                    [SecsElement(Index = 1)]
                    public VIDITEM_54_DVVAL CARRIER_ID_OBJ;
                    [SecsElement(Index = 2)]
                    public VIDITEM_70_DVVAL VRHICLE_ID_OBJ;
                    [SecsElement(Index = 3)]
                    public VIDITEM_56_DVVAL CARRIER_LOC_OBJ;
                    [SecsElement(Index = 4)]
                    public VIDITEM_204_DVVAL INSTALL_TIME_OBJ;

                    public VIDITEM_75_DVVAL()
                    {
                        CARRIER_ID_OBJ = new VIDITEM_54_DVVAL();
                        VRHICLE_ID_OBJ = new VIDITEM_70_DVVAL();
                        CARRIER_LOC_OBJ = new VIDITEM_56_DVVAL();
                        INSTALL_TIME_OBJ = new VIDITEM_204_DVVAL();
                    }
                    public string CARRIER_ID
                    {
                        get { return CARRIER_ID_OBJ?.CARRIER_ID; }
                        set { CARRIER_ID_OBJ.CARRIER_ID = value; }
                    }
                    public string VRHICLE_ID
                    {
                        get { return VRHICLE_ID_OBJ?.VEHILCE_ID; }
                        set { VRHICLE_ID_OBJ.VEHILCE_ID = value; }
                    }
                    public string CARRIER_LOC
                    {
                        get { return CARRIER_LOC_OBJ?.CARRIER_LOC; }
                        set { CARRIER_LOC_OBJ.CARRIER_LOC = value; }
                    }
                    public string INSTALL_TIME
                    {
                        get { return INSTALL_TIME_OBJ?.INSTALL_TIME; }
                        set { INSTALL_TIME_OBJ.INSTALL_TIME = value; }
                    }


                }

                [Serializable]
                public class VIDITEM_75_SV : SXFY
                {
                    [SecsElement(Index = 1)]
                    public VIDITEM_54_SV CARRIER_ID_OBJ;
                    [SecsElement(Index = 2)]
                    public VIDITEM_70_SV VRHICLE_ID_OBJ;
                    [SecsElement(Index = 3)]
                    public VIDITEM_56_SV CARRIER_LOC_OBJ;
                    [SecsElement(Index = 4)]
                    public VIDITEM_204_SV INSTALL_TIME_OBJ;

                    public VIDITEM_75_SV()
                    {
                        CARRIER_ID_OBJ = new VIDITEM_54_SV();
                        VRHICLE_ID_OBJ = new VIDITEM_70_SV();
                        CARRIER_LOC_OBJ = new VIDITEM_56_SV();
                        INSTALL_TIME_OBJ = new VIDITEM_204_SV();
                    }
                }

                [Serializable]
                public class VIDITEM_76_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_205_SV[] ENHANCED_TRANSFER_COMMAND_INFOS;
                }
                //[Serializable]
                //public class VIDITEM_77 : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true)]
                //    public TRANSFERCOMPLETEINFO[] TRANCOMPLETEINFO;
                //    public VIDITEM_77()
                //    {
                //        TRANCOMPLETEINFO = new TRANSFERCOMPLETEINFO[1];
                //        TRANCOMPLETEINFO[0] = new TRANSFERCOMPLETEINFO();
                //    }
                //    [Serializable]
                //    public class TRANSFERCOMPLETEINFO : SXFY
                //    {
                //        [SecsElement(Index = 1, ListSpreadOut = true)]
                //        public TRANSFERINFO TRANINFO;
                //        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //        public string CARRIER_LOC;
                //        public TRANSFERCOMPLETEINFO()
                //        {
                //            TRANINFO = new TRANSFERINFO();
                //            CARRIER_LOC = string.Empty;
                //        }
                //        [Serializable]
                //        public class TRANSFERINFO : SXFY
                //        {
                //            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //            public string CARRIER_ID;
                //            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //            public string SOURCE_PORT;
                //            [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //            public string DESTINATION_PORT;
                //            public TRANSFERINFO()
                //            {
                //                CARRIER_ID = string.Empty;
                //                SOURCE_PORT = string.Empty;
                //                DESTINATION_PORT = string.Empty;
                //            }
                //        }
                //    }
                //}

                [Serializable]
                public class VIDITEM_78_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string SOURCE_PORT;
                    public VIDITEM_78_SV()
                    {
                        SOURCE_PORT = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_79_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public TRANSFERCOMPLETEINFO[] TRANCOMPLETEINFO;
                    public VIDITEM_79_DVVAL()
                    {
                        TRANCOMPLETEINFO = new TRANSFERCOMPLETEINFO[1];
                        TRANCOMPLETEINFO[0] = new TRANSFERCOMPLETEINFO();
                    }
                    [Serializable]
                    public class TRANSFERCOMPLETEINFO : SXFY
                    {
                        [SecsElement(Index = 1)]
                        public VIDITEM_67_DVVAL TRANSFER_INFO_OBJ;
                        [SecsElement(Index = 2)]
                        public VIDITEM_56_DVVAL CARRIER_LOC_OBJ;
                        public TRANSFERCOMPLETEINFO()
                        {
                            TRANSFER_INFO_OBJ = new VIDITEM_67_DVVAL();
                            CARRIER_LOC_OBJ = new VIDITEM_56_DVVAL();
                        }

                        public string CARRIER_LOC
                        {
                            get { return CARRIER_LOC_OBJ?.CARRIER_LOC; }
                            set { CARRIER_LOC_OBJ.CARRIER_LOC = value; }
                        }
                    }
                }


                //[Serializable]
                //public class VIDITEM_80 : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //    public string COMMAND_TYPE;
                //    public VIDITEM_80()
                //    {
                //        COMMAND_TYPE = string.Empty;
                //    }
                //}
                //[Serializable]
                //public class VIDITEM_81 : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //    public string ALARM_ID;
                //    public VIDITEM_81()
                //    {
                //        ALARM_ID = string.Empty;
                //    }
                //}
                //[Serializable]
                //public class VIDITEM_82 : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //    public string ALARM_TEXT;
                //    public VIDITEM_82()
                //    {
                //        ALARM_TEXT = string.Empty;
                //    }
                //}
                //[Serializable]
                //public class VIDITEM_83 : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //    public string UNIT_ID;
                //    public VIDITEM_83()
                //    {
                //        UNIT_ID = string.Empty;
                //    }
                //}
                //[Serializable]
                //public class VIDITEM_84 : SXFY
                //{
                //    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //    public string CARRIER_ID;
                //    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //    public string SOURCE_PORT;
                //    [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //    public string DESTINATION_PORT;
                //    public VIDITEM_84()
                //    {
                //        CARRIER_ID = string.Empty;
                //        SOURCE_PORT = string.Empty;
                //        DESTINATION_PORT = string.Empty;
                //    }
                //}
                [Serializable]
                public class VIDITEM_91_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_75_DVVAL[] ENHANCED_CARRIER_INFOS;
                    public VIDITEM_91_SV()
                    {

                    }
                }

                [Serializable]
                public class VIDITEM_114_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string SPEC_VERSION;
                    public VIDITEM_114_SV()
                    {
                        SPEC_VERSION = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_115_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string PORT_ID;
                    public VIDITEM_115_DVVAL()
                    {
                        PORT_ID = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_115_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string PORT_ID;
                    public VIDITEM_115_SV()
                    {
                        PORT_ID = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_116_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_54_DVVAL[] CARRIER_ID_OBJ;
                }

                [Serializable]
                public class VIDITEM_117_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string VEHICLE_LOCATION;
                    public VIDITEM_117_DVVAL()
                    {
                        VEHICLE_LOCATION = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_118_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_PORT_INFO[] PORT_INFO;
                }

                [Serializable]
                public class VIDITEM_119_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public ENHANCED_VEHICLE_INFO[] ENHANCED_VEHICLE_INFO_OBJ;
                    public VIDITEM_119_SV()
                    {

                    }
                    public class ENHANCED_VEHICLE_INFO : SXFY
                    {
                        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 6)]
                        public string VehicleID;
                        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                        public string VehicleState;
                        [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                        public string VehicleLocation;
                    }
                }

                [Serializable]
                public class VIDITEM_120_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string UNIT_STATUS_CLEABLE;
                    public VIDITEM_120_DVVAL()
                    {
                        UNIT_STATUS_CLEABLE = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_202_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string TRANSFER_STATE;
                    public VIDITEM_202_DVVAL()
                    {
                        TRANSFER_STATE = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_202_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string TRANSFER_STATE;
                    public VIDITEM_202_SV()
                    {
                        TRANSFER_STATE = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_204_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 16)]
                    public string INSTALL_TIME;
                    public VIDITEM_204_DVVAL()
                    {
                        INSTALL_TIME = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_204_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 16)]
                    public string INSTALL_TIME;
                    public VIDITEM_204_SV()
                    {
                        INSTALL_TIME = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_205_DVVAL : SXFY
                {
                    [SecsElement(Index = 1)]
                    public VIDITEM_59_DVVAL COMMAND_INFO_OBJ;
                    [SecsElement(Index = 2)]
                    public VIDITEM_202_DVVAL TRANSFER_STATE_OBJ;
                    [SecsElement(Index = 3)]
                    public VIDITEM_67_DVVAL[] TRANSFER_INFO_OBJ;
                    public VIDITEM_205_DVVAL()
                    {
                        COMMAND_INFO_OBJ = new VIDITEM_59_DVVAL();
                        TRANSFER_STATE_OBJ = new VIDITEM_202_DVVAL();
                    }
                }

                [Serializable]
                public class VIDITEM_205_SV : SXFY
                {
                    [SecsElement(Index = 1)]
                    public VIDITEM_59_SV COMMAND_INFO_OBJ;
                    [SecsElement(Index = 2)]
                    public VIDITEM_202_SV TRANSFER_STATE_OBJ;
                    [SecsElement(Index = 3)]
                    public VIDITEM_67_SV[] TRANSFER_INFO_OBJ;
                    public VIDITEM_205_SV()
                    {
                        COMMAND_INFO_OBJ = new VIDITEM_59_SV();
                        TRANSFER_STATE_OBJ = new VIDITEM_202_SV();
                    }
                }

                [Serializable]
                public class VIDITEM_211_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string UNIT_ID;
                    public VIDITEM_211_DVVAL()
                    {
                        UNIT_ID = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_212_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                    public string ALARM_TEXT;
                    public VIDITEM_212_DVVAL()
                    {
                        ALARM_TEXT = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_251_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string VEHICLE_CURRENT_POSITION;
                    public VIDITEM_251_DVVAL()
                    {
                        VEHICLE_CURRENT_POSITION = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_252_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public MONITORED_VEHICLE_INFO[] MONITORED_VEHICLE_INFO_OBJ;
                    public VIDITEM_252_SV()
                    {

                    }
                    public class MONITORED_VEHICLE_INFO : SXFY
                    {
                        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 6)]
                        public string VehicleID;
                        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                        public string VehicleCurrentPosition;
                        [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                        public string VehicleDistanceFromCurrentPosition;
                        [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 32)]
                        public string VehicleCurrentDomain;
                        [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                        public string VehicleNextPosition;
                        [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                        public string VehicleOperationState;
                        [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                        public string VehicleCommunicationState;
                        [SecsElement(Index = 8, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                        public string VehicleControlMode;
                        [SecsElement(Index = 9, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                        public string VehicleJamState;
                        [SecsElement(Index = 10, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                        public string AlarmID;
                    }
                }

                [Serializable]
                public class VIDITEM_253_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 32)]
                    public string VEHICLE_CURRENT_DOMAIN;
                    public VIDITEM_253_SV()
                    {
                        VEHICLE_CURRENT_DOMAIN = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_254_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public UNIT_ALARM_INFO[] UNIT_ALARMS_INFO_OBJ;
                    public VIDITEM_254_SV()
                    {

                    }
                    public class UNIT_ALARM_INFO : SXFY
                    {
                        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                        public string UnitID;
                        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                        public string VehicleCurrentPosition;
                        [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                        public string VehicleNextPosition;
                        [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                        public string AlarmID;
                        [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                        public string AlarmText;
                        [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                        public string VehicleCommunicationState;
                        [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                        public string MainteState;
                    }
                }

                [Serializable]
                public class VIDITEM_262_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string VEHICLE_NEXT_POSITION;
                    public VIDITEM_262_DVVAL()
                    {
                        VEHICLE_NEXT_POSITION = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_301_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string PAUSE_REASON;
                    public VIDITEM_301_DVVAL()
                    {
                        PAUSE_REASON = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_330_DVVAL : SXFY
                {
                    [SecsElement(Index = 1)]
                    public LANE_INFO LANE_INFO_OBJ;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string LANE_CUT_TYPE;
                    public VIDITEM_330_DVVAL()
                    {
                        LANE_INFO_OBJ = new LANE_INFO();
                        LANE_CUT_TYPE = string.Empty;
                    }
                    [Serializable]
                    public class LANE_INFO : SXFY
                    {
                        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                        public string StartPoint;
                        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                        public string EndPoint;
                    }
                }


                [Serializable]
                public class VIDITEM_330_SV : SXFY
                {
                    [SecsElement(Index = 1)]
                    public LANE_INFO LANE_INFO_OBJ;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string LANE_CUT_TYPE;
                    public VIDITEM_330_SV()
                    {
                        LANE_INFO_OBJ = new LANE_INFO();
                        LANE_CUT_TYPE = string.Empty;
                    }
                    public class LANE_INFO : SXFY
                    {
                        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                        public string StartPoint;
                        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                        public string EndPoint;
                    }
                }


                //[Serializable]
                //public class VIDITEM_350 : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //    public VIDITEM_354 PORT_INFO;
                //}
                //[Serializable]
                //public class VIDITEM_351 : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true)]
                //    public VIDITEM_115 PORT_ID;
                //    [SecsElement(Index = 2, ListSpreadOut = true)]
                //    public VIDITEM_355 PORT_TRANSFER_STATE;
                //    [SecsElement(Index = 3, ListSpreadOut = true)]
                //    public VIDITEM_352 EQ_REQ_STATUS;
                //    [SecsElement(Index = 4, ListSpreadOut = true)]
                //    public VIDITEM_353 EQ_PRESENCE_STATUS;
                //}

                //[Serializable]
                //public class VIDITEM_352 : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //    public string EQ_REQ_STATUS;
                //    public VIDITEM_352()
                //    {
                //        EQ_REQ_STATUS = string.Empty;
                //    }
                //}

                //[Serializable]
                //public class VIDITEM_353 : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //    public string EQ_PRESENCE_STATUS;
                //    public VIDITEM_353()
                //    {
                //        EQ_PRESENCE_STATUS = string.Empty;
                //    }
                //}

                [Serializable]
                public class VIDITEM_PORT_INFO : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_115_DVVAL PORT_ID_OBJ;
                    [SecsElement(Index = 2, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string PORT_TRANSFER_STATE;
                    public VIDITEM_PORT_INFO()
                    {
                        PORT_ID_OBJ = new VIDITEM_115_DVVAL();
                    }
                    public string PORT_ID
                    {
                        get { return PORT_ID_OBJ.PORT_ID; }
                        set { PORT_ID_OBJ.PORT_ID = value; }
                    }
                }

                [Serializable]
                public class VIDITEM_354_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_115_SV PORT_ID_OBJ;
                    [SecsElement(Index = 2, ListSpreadOut = true)]
                    public VIDITEM_355_SV PORT_TRANSFTER_STATE_OBJ;
                    public VIDITEM_354_SV()
                    {
                        PORT_ID_OBJ = new VIDITEM_115_SV();
                        PORT_TRANSFTER_STATE_OBJ = new VIDITEM_355_SV();
                    }
                }

                [Serializable]
                public class VIDITEM_355_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string PORT_TRANSFER_STATE;
                    public VIDITEM_355_DVVAL()
                    {
                        PORT_TRANSFER_STATE = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_355_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string PORT_TRANSFER_STATE;
                    public VIDITEM_355_SV()
                    {
                        PORT_TRANSFER_STATE = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_360_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_330_SV[] LANE_CUT_INFO;
                    public VIDITEM_360_SV()
                    {
                    }
                }


                //[Serializable]
                //public class VIDITEM_361 : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true)]
                //    public VIDITEM_83 UNIT_ID;
                //    [SecsElement(Index = 2, ListSpreadOut = true)]
                //    public VIDITEM_81 ALARM_ID;
                //    [SecsElement(Index = 2, ListSpreadOut = true)]
                //    public VIDITEM_82 ALARM_TEXT;
                //    [SecsElement(Index = 2, ListSpreadOut = true)]
                //    public VIDITEM_362 MAINT_STATE;
                //    public VIDITEM_361()
                //    {
                //        UNIT_ID = new VIDITEM_83();
                //        ALARM_ID = new VIDITEM_81();
                //        ALARM_TEXT = new VIDITEM_82();
                //        MAINT_STATE = new VIDITEM_362();
                //    }
                //}

                //[Serializable]
                //public class VIDITEM_362 : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //    public string MAINT_STATE;
                //    public VIDITEM_362()
                //    {
                //        MAINT_STATE = string.Empty;
                //    }
                //}
                //[Serializable]
                //public class VIDITEM_363 : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //    public string VEHICLE_CURRENT_POSITION;
                //    public VIDITEM_363()
                //    {
                //        VEHICLE_CURRENT_POSITION = string.Empty;
                //    }
                //}

                [Serializable]
                public class VIDITEM_370_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public STAGE_COMMAND[] STAGE_COMMAND_OBJ;
                    public VIDITEM_370_SV()
                    {
                        STAGE_COMMAND_OBJ = new STAGE_COMMAND[1];
                        STAGE_COMMAND_OBJ[0] = new STAGE_COMMAND();
                    }

                    public class STAGE_COMMAND
                    {
                        [SecsElement(Index = 1)]
                        STAGE_INFO StageInfo;
                        [SecsElement(Index = 2)]
                        VIDITEM_67_SV TransferInfo;
                        public STAGE_COMMAND()
                        {
                            StageInfo = new STAGE_INFO();
                            TransferInfo = new VIDITEM_67_SV();
                        }

                        public class STAGE_INFO
                        {
                            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                            string StageID;
                            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                            string Priority;
                            [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                            string ExpectedDuration;
                            [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                            string NoBlockingTime;
                            [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                            string WaitTimeout;
                            public STAGE_INFO()
                            {
                                StageID = string.Empty;
                                Priority = string.Empty;
                                ExpectedDuration = string.Empty;
                                NoBlockingTime = string.Empty;
                                WaitTimeout = string.Empty;
                            }
                        }
                    }
                }


                [Serializable]
                public class VIDITEM_371_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public STAGE_VEHICLE_INFO[] STAGE_VEHICLE_INFO_OBJ;
                    public VIDITEM_371_SV()
                    {
                        STAGE_VEHICLE_INFO_OBJ = new STAGE_VEHICLE_INFO[1];
                        STAGE_VEHICLE_INFO_OBJ[0] = new STAGE_VEHICLE_INFO();
                    }

                    public class STAGE_VEHICLE_INFO
                    {
                        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 6)]
                        string VehicleID;
                        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                        string StageID;
                        [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                        string VehicleLocation;
                        public STAGE_VEHICLE_INFO()
                        {
                            VehicleID = string.Empty;
                            StageID = string.Empty;
                            VehicleLocation = string.Empty;
                        }
                    }
                }

            }
        }
    }
    public class VIDCollection
    {
        public VIDCollection()
        {
            VIDITEM_01_DVVAL_AlarmID = new S6F11.RPTINFO.RPTITEM.VIDITEM_01_DVVAL();
            VIDITEM_02_ECV_EstablishCommunicationsTimeout = new S6F11.RPTINFO.RPTITEM.VIDITEM_02_ECV();
            VIDITEM_03_SV_AlarmsEnabled = new S6F11.RPTINFO.RPTITEM.VIDITEM_03_SV();
            VIDITEM_04_SV_AlarmsSet = new S6F11.RPTINFO.RPTITEM.VIDITEM_04_SV();
            VIDITEM_05_SV_Clock = new S6F11.RPTINFO.RPTITEM.VIDITEM_05_SV();
            VIDITEM_06_SV_ControlState = new S6F11.RPTINFO.RPTITEM.VIDITEM_06_SV();
            VIDITEM_07_SV_EventsEnabled = new S6F11.RPTINFO.RPTITEM.VIDITEM_07_SV();
            VIDITEM_51_SV_ActiveCarriers = new S6F11.RPTINFO.RPTITEM.VIDITEM_51_SV();
            VIDITEM_52_SV_ActiveTransfers = new S6F11.RPTINFO.RPTITEM.VIDITEM_52_SV();
            VIDITEM_53_SV_ActiveVehicles = new S6F11.RPTINFO.RPTITEM.VIDITEM_53_SV();
            VIDITEM_54_DVVAL_CarrierID = new S6F11.RPTINFO.RPTITEM.VIDITEM_54_DVVAL();
            VIDITEM_54_SV_CarrierID = new S6F11.RPTINFO.RPTITEM.VIDITEM_54_SV();
            VIDITEM_55_DVVAL_CarrierInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_55_DVVAL();
            VIDITEM_55_SV_CarrierInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_55_SV();
            VIDITEM_56_DVVAL_CarrierLoc = new S6F11.RPTINFO.RPTITEM.VIDITEM_56_DVVAL();
            VIDITEM_56_SV_CarrierLoc = new S6F11.RPTINFO.RPTITEM.VIDITEM_56_SV();
            VIDITEM_57_DVVAL_CommandName = new S6F11.RPTINFO.RPTITEM.VIDITEM_57_DVVAL();
            VIDITEM_58_DVVAL_CommandID = new S6F11.RPTINFO.RPTITEM.VIDITEM_58_DVVAL();
            VIDITEM_58_SV_CommandID = new S6F11.RPTINFO.RPTITEM.VIDITEM_58_SV();
            VIDITEM_59_DVVAL_CommandInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_59_DVVAL();
            VIDITEM_59_SV_CommandInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_59_SV();
            VIDITEM_60_DVVAL_DestPort = new S6F11.RPTINFO.RPTITEM.VIDITEM_60_DVVAL();
            VIDITEM_60_SV_DestPort = new S6F11.RPTINFO.RPTITEM.VIDITEM_60_SV();
            VIDITEM_61_ECV_EqpName = new S6F11.RPTINFO.RPTITEM.VIDITEM_61_ECV();
            VIDITEM_62_DVVAL_Priority = new S6F11.RPTINFO.RPTITEM.VIDITEM_62_DVVAL();
            VIDITEM_62_SV_Priority = new S6F11.RPTINFO.RPTITEM.VIDITEM_62_SV();
            VIDITEM_63_DVVAL_Replace = new S6F11.RPTINFO.RPTITEM.VIDITEM_63_DVVAL();
            VIDITEM_63_SV_Replace = new S6F11.RPTINFO.RPTITEM.VIDITEM_63_SV();
            VIDITEM_64_DVVAL_ResultCode = new S6F11.RPTINFO.RPTITEM.VIDITEM_64_DVVAL();
            VIDITEM_65_DVVAL_SourcePort = new S6F11.RPTINFO.RPTITEM.VIDITEM_65_DVVAL();
            VIDITEM_66_DVVAL_TransferCommand = new S6F11.RPTINFO.RPTITEM.VIDITEM_66_DVVAL();
            VIDITEM_66_SV_TransferCommand = new S6F11.RPTINFO.RPTITEM.VIDITEM_66_SV();
            VIDITEM_67_DVVAL_TransferInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_67_DVVAL();
            VIDITEM_67_SV_TransferInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_67_SV();
            VIDITEM_68_DVVAL_TransferPort = new S6F11.RPTINFO.RPTITEM.VIDITEM_68_DVVAL();
            VIDITEM_69_DVVAL_TransferPortList = new S6F11.RPTINFO.RPTITEM.VIDITEM_69_DVVAL();
            VIDITEM_70_DVVAL_VehicleID = new S6F11.RPTINFO.RPTITEM.VIDITEM_70_DVVAL();
            VIDITEM_70_SV_VehicleID = new S6F11.RPTINFO.RPTITEM.VIDITEM_70_SV();
            VIDITEM_71_DVVAL_VehicleInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_71_DVVAL();
            VIDITEM_71_SV_VehicleInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_71_SV();
            VIDITEM_72_DVVAL_VehicleState = new S6F11.RPTINFO.RPTITEM.VIDITEM_72_DVVAL();
            VIDITEM_73_SV_TSCState = new S6F11.RPTINFO.RPTITEM.VIDITEM_73_SV();
            VIDITEM_74_DVVAL_CommandType = new S6F11.RPTINFO.RPTITEM.VIDITEM_74_DVVAL();
            VIDITEM_75_DVVAL_EnhancedCarriInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_75_DVVAL();
            VIDITEM_75_SV_EnhancedCarriInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_75_SV();
            VIDITEM_76_SV_EnhancedTransfers = new S6F11.RPTINFO.RPTITEM.VIDITEM_76_SV();
            VIDITEM_78_SV_SourcePort = new S6F11.RPTINFO.RPTITEM.VIDITEM_78_SV();
            VIDITEM_79_DVVAL_TransferCompleteInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_79_DVVAL();
            VIDITEM_114_SV_SpecVersion = new S6F11.RPTINFO.RPTITEM.VIDITEM_114_SV();
            VIDITEM_115_DVVAL_PortID = new S6F11.RPTINFO.RPTITEM.VIDITEM_115_DVVAL();
            VIDITEM_115_SV_PortID = new S6F11.RPTINFO.RPTITEM.VIDITEM_115_SV();
            VIDITEM_116_DVVAL_CarrierIDList = new S6F11.RPTINFO.RPTITEM.VIDITEM_116_DVVAL();
            VIDITEM_117_DVVAL_VehicleLocation = new S6F11.RPTINFO.RPTITEM.VIDITEM_117_DVVAL();
            VIDITEM_118_DVVAL_CurrentPortStates = new S6F11.RPTINFO.RPTITEM.VIDITEM_118_DVVAL();
            VIDITEM_119_SV_EnhancedVehicles = new S6F11.RPTINFO.RPTITEM.VIDITEM_119_SV();
            VIDITEM_120_DVVAL_UnitStatusCleable = new S6F11.RPTINFO.RPTITEM.VIDITEM_120_DVVAL();
            VIDITEM_202_DVVAL_TransferState = new S6F11.RPTINFO.RPTITEM.VIDITEM_202_DVVAL();
            VIDITEM_202_SV_TransferState = new S6F11.RPTINFO.RPTITEM.VIDITEM_202_SV();
            VIDITEM_204_DVVAL_InstallTime = new S6F11.RPTINFO.RPTITEM.VIDITEM_204_DVVAL();
            VIDITEM_204_SV_InstallTime = new S6F11.RPTINFO.RPTITEM.VIDITEM_204_SV();
            VIDITEM_205_DVVAL_EnhancedTransferCommand = new S6F11.RPTINFO.RPTITEM.VIDITEM_205_DVVAL();
            VIDITEM_205_SV_EnhancedTransferCommand = new S6F11.RPTINFO.RPTITEM.VIDITEM_205_SV();
            VIDITEM_211_DVVAL_UnitID = new S6F11.RPTINFO.RPTITEM.VIDITEM_211_DVVAL();
            VIDITEM_212_DVVAL_AlarmText = new S6F11.RPTINFO.RPTITEM.VIDITEM_212_DVVAL();
            VIDITEM_251_DVVAL_VehicleCurrentPosition = new S6F11.RPTINFO.RPTITEM.VIDITEM_251_DVVAL();
            VIDITEM_252_SV_MonitoredVehicles = new S6F11.RPTINFO.RPTITEM.VIDITEM_252_SV();
            VIDITEM_253_SV_VehicleCurrentDomain = new S6F11.RPTINFO.RPTITEM.VIDITEM_253_SV();
            VIDITEM_254_SV_UnitAlarmStatList = new S6F11.RPTINFO.RPTITEM.VIDITEM_254_SV();
            VIDITEM_262_DVVAL_VehicleNextPosition = new S6F11.RPTINFO.RPTITEM.VIDITEM_262_DVVAL();
            VIDITEM_301_DVVAL_PauseReason = new S6F11.RPTINFO.RPTITEM.VIDITEM_301_DVVAL();
            VIDITEM_330_DVVAL_LaneCutInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_330_DVVAL();
            VIDITEM_330_SV_LaneCutInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_330_SV();
            VIDITEM_354_DVVAL_PortInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_PORT_INFO();
            VIDITEM_354_SV_PortInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_354_SV();
            VIDITEM_355_DVVAL_PortTransferState = new S6F11.RPTINFO.RPTITEM.VIDITEM_355_DVVAL();
            VIDITEM_355_SV_PortTransferState = new S6F11.RPTINFO.RPTITEM.VIDITEM_355_SV();
            VIDITEM_360_SV_LaneCutInfoList = new S6F11.RPTINFO.RPTITEM.VIDITEM_360_SV();
            VIDITEM_370_SV_ActiveStages = new S6F11.RPTINFO.RPTITEM.VIDITEM_370_SV();
            VIDITEM_371_SV_StageVehicles = new S6F11.RPTINFO.RPTITEM.VIDITEM_371_SV();
        }
        public string VH_ID;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_01_DVVAL VIDITEM_01_DVVAL_AlarmID;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_02_ECV VIDITEM_02_ECV_EstablishCommunicationsTimeout;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_03_SV VIDITEM_03_SV_AlarmsEnabled;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_04_SV VIDITEM_04_SV_AlarmsSet;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_05_SV VIDITEM_05_SV_Clock;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_06_SV VIDITEM_06_SV_ControlState;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_07_SV VIDITEM_07_SV_EventsEnabled;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_51_SV VIDITEM_51_SV_ActiveCarriers;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_52_SV VIDITEM_52_SV_ActiveTransfers;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_53_SV VIDITEM_53_SV_ActiveVehicles;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_54_DVVAL VIDITEM_54_DVVAL_CarrierID;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_54_SV VIDITEM_54_SV_CarrierID;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_55_DVVAL VIDITEM_55_DVVAL_CarrierInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_55_SV VIDITEM_55_SV_CarrierInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_56_DVVAL VIDITEM_56_DVVAL_CarrierLoc;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_56_SV VIDITEM_56_SV_CarrierLoc;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_57_DVVAL VIDITEM_57_DVVAL_CommandName;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_58_DVVAL VIDITEM_58_DVVAL_CommandID;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_58_SV VIDITEM_58_SV_CommandID;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_59_DVVAL VIDITEM_59_DVVAL_CommandInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_59_SV VIDITEM_59_SV_CommandInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_60_DVVAL VIDITEM_60_DVVAL_DestPort;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_60_SV VIDITEM_60_SV_DestPort;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_61_ECV VIDITEM_61_ECV_EqpName;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_62_DVVAL VIDITEM_62_DVVAL_Priority;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_62_SV VIDITEM_62_SV_Priority;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_63_DVVAL VIDITEM_63_DVVAL_Replace;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_63_SV VIDITEM_63_SV_Replace;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_64_DVVAL VIDITEM_64_DVVAL_ResultCode;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_65_DVVAL VIDITEM_65_DVVAL_SourcePort;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_66_DVVAL VIDITEM_66_DVVAL_TransferCommand;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_66_SV VIDITEM_66_SV_TransferCommand;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_67_DVVAL VIDITEM_67_DVVAL_TransferInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_67_SV VIDITEM_67_SV_TransferInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_68_DVVAL VIDITEM_68_DVVAL_TransferPort;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_69_DVVAL VIDITEM_69_DVVAL_TransferPortList;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_70_DVVAL VIDITEM_70_DVVAL_VehicleID;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_70_SV VIDITEM_70_SV_VehicleID;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_71_DVVAL VIDITEM_71_DVVAL_VehicleInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_71_SV VIDITEM_71_SV_VehicleInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_72_DVVAL VIDITEM_72_DVVAL_VehicleState;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_73_SV VIDITEM_73_SV_TSCState;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_74_DVVAL VIDITEM_74_DVVAL_CommandType;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_75_DVVAL VIDITEM_75_DVVAL_EnhancedCarriInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_75_SV VIDITEM_75_SV_EnhancedCarriInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_76_SV VIDITEM_76_SV_EnhancedTransfers;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_78_SV VIDITEM_78_SV_SourcePort;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_79_DVVAL VIDITEM_79_DVVAL_TransferCompleteInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_114_SV VIDITEM_114_SV_SpecVersion;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_115_DVVAL VIDITEM_115_DVVAL_PortID;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_115_SV VIDITEM_115_SV_PortID;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_116_DVVAL VIDITEM_116_DVVAL_CarrierIDList;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_117_DVVAL VIDITEM_117_DVVAL_VehicleLocation;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_118_DVVAL VIDITEM_118_DVVAL_CurrentPortStates;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_119_SV VIDITEM_119_SV_EnhancedVehicles;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_120_DVVAL VIDITEM_120_DVVAL_UnitStatusCleable;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_202_DVVAL VIDITEM_202_DVVAL_TransferState;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_202_SV VIDITEM_202_SV_TransferState;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_204_DVVAL VIDITEM_204_DVVAL_InstallTime;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_204_SV VIDITEM_204_SV_InstallTime;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_205_DVVAL VIDITEM_205_DVVAL_EnhancedTransferCommand;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_205_SV VIDITEM_205_SV_EnhancedTransferCommand;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_211_DVVAL VIDITEM_211_DVVAL_UnitID;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_212_DVVAL VIDITEM_212_DVVAL_AlarmText;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_251_DVVAL VIDITEM_251_DVVAL_VehicleCurrentPosition;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_252_SV VIDITEM_252_SV_MonitoredVehicles;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_253_SV VIDITEM_253_SV_VehicleCurrentDomain;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_254_SV VIDITEM_254_SV_UnitAlarmStatList;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_262_DVVAL VIDITEM_262_DVVAL_VehicleNextPosition;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_301_DVVAL VIDITEM_301_DVVAL_PauseReason;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_330_DVVAL VIDITEM_330_DVVAL_LaneCutInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_330_SV VIDITEM_330_SV_LaneCutInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_PORT_INFO VIDITEM_354_DVVAL_PortInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_354_SV VIDITEM_354_SV_PortInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_355_DVVAL VIDITEM_355_DVVAL_PortTransferState;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_355_SV VIDITEM_355_SV_PortTransferState;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_360_SV VIDITEM_360_SV_LaneCutInfoList;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_370_SV VIDITEM_370_SV_ActiveStages;
        public com.mirle.ibg3k0.sc.Data.SECS.CSOT.S6F11.RPTINFO.RPTITEM.VIDITEM_371_SV VIDITEM_371_SV_StageVehicles;
    }
}
