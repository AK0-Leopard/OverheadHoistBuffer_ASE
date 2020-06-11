using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;
using com.mirle.ibg3k0.stc.Data.SecsData;

namespace com.mirle.ibg3k0.sc.Data.SECS.ASE
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

                #region VID Synchronize 
                [Serializable]
                public class VIDITEM_04_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public ALID_DVVAL[] ALIDs;

                    public VIDITEM_04_SV()
                    {
                        ALIDs = new ALID_DVVAL[0];
                    }
                }
                public class ALID_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER,Length =1)]
                    public string ALID;
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
                public class VIDITEM_51_SV : SXFY
                {
                    [SecsElement(Index = 1,ListSpreadOut = true)]
                    public VIDITEM_10_SV[] ENHANCED_CARRIER_INFO;

                    public VIDITEM_51_SV()
                    {
                        ENHANCED_CARRIER_INFO = new VIDITEM_10_SV[0];
                    }
                }

                [Serializable]
                public class VIDITEM_73_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true,Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string SC_STATE; //1=Init 2=Paused 3=Auto 4=Pausing

                    public VIDITEM_73_SV()
                    {
                        SC_STATE = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_76_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_13_SV[] ENHANCED_TRANSFER_CMD;

                    public VIDITEM_76_SV()
                    {
                        ENHANCED_TRANSFER_CMD = new VIDITEM_13_SV[0];
                    }
                }

                [Serializable]
                public class VIDITEM_107_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_172_SV[] ZoneData;
                    public VIDITEM_107_SV()
                    {
                        ZoneData = new VIDITEM_172_SV[0];
                    }
                }

                [Serializable]
                public class VIDITEM_118_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_354_SV[] PORT_INFO_OBJ;

                    public VIDITEM_118_SV()
                    {
                        PORT_INFO_OBJ = new VIDITEM_354_SV[0];
                    }
                }

                [Serializable]
                public class VIDITEM_350_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_356_SV[] EQ_PORT_INFO_OBJ;

                    public VIDITEM_350_SV()
                    {
                        EQ_PORT_INFO_OBJ = new VIDITEM_356_SV[0];
                    }
                }

                [Serializable]
                public class VIDITEM_351_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_601_SV[] PORT_TYPE_INFO_OBJ;

                    public VIDITEM_351_SV()
                    {
                        PORT_TYPE_INFO_OBJ = new VIDITEM_601_SV[0];
                    }
                }

                [Serializable]
                public class VIDITEM_360_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_361_SV[] UNIT_ALARM_INFO_OBJ;

                    public VIDITEM_360_SV()
                    {
                        UNIT_ALARM_INFO_OBJ = new VIDITEM_361_SV[0];
                    }
                }
                #endregion
                #region VID Numbering
                [Serializable]
                public class VIDITEM_10_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_54_DVVAL CARRIER_ID_OBJ;
                    [SecsElement(Index = 2, ListSpreadOut = true)]
                    public VIDITEM_56_DVVAL CARRIER_LOC_OBJ;
                    [SecsElement(Index = 3, ListSpreadOut = true)]
                    public VIDITEM_370_DVVAL CARRIER_ZONE_NAME_OBJ;
                    [SecsElement(Index = 4, ListSpreadOut = true)]
                    public VIDITEM_12_DVVAL INSTALL_TIME_OBJ;
                    [SecsElement(Index = 5, ListSpreadOut = true)]
                    public VIDITEM_203_DVVAL CARRIER_STATE;
                    [SecsElement(Index = 6, ListSpreadOut = true)]
                    public VIDITEM_179_DVVAL BOX_ID_OBJ;

                    public VIDITEM_10_SV()
                    {
                        CARRIER_ID_OBJ = new VIDITEM_54_DVVAL();
                        CARRIER_LOC_OBJ = new VIDITEM_56_DVVAL();
                        CARRIER_ZONE_NAME_OBJ = new VIDITEM_370_DVVAL();
                        INSTALL_TIME_OBJ = new VIDITEM_12_DVVAL();
                        CARRIER_STATE = new VIDITEM_203_DVVAL();
                        BOX_ID_OBJ = new VIDITEM_179_DVVAL();
                    }
                }

                [Serializable]
                public class VIDITEM_11_SV : SXFY
                {
                    [SecsElement(Index = 1)]
                    public VIDITEM_58_DVVAL COMMAND_ID_OBJ;
                    [SecsElement(Index = 2)]
                    public VIDITEM_62_DVVAL PRIORITY_OBJ;
                    public VIDITEM_11_SV()
                    {
                        COMMAND_ID_OBJ = new VIDITEM_58_DVVAL();
                        PRIORITY_OBJ = new VIDITEM_62_DVVAL();
                    }
                }

                [Serializable]
                public class VIDITEM_12_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 16)]
                    public string INSTALLTIME;
                    public VIDITEM_12_DVVAL()
                    {
                        INSTALLTIME = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_13_SV : SXFY
                {
                    [SecsElement(Index = 1)]
                    public VIDITEM_722_DVVAL TRANSFER_STATE_OBJ;
                    [SecsElement(Index = 2)]
                    public VIDITEM_11_SV COMMAND_INFO_OBJ;
                    [SecsElement(Index = 3)]
                    public VIDITEM_720_SV TRANSFER_INFO_OBJ;
                    public VIDITEM_13_SV()
                    {
                        TRANSFER_STATE_OBJ = new VIDITEM_722_DVVAL();
                        COMMAND_INFO_OBJ = new VIDITEM_11_SV();
                        TRANSFER_INFO_OBJ = new VIDITEM_720_SV();
                    }
                }

                [Serializable]
                public class VIDITEM_15_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string ENPTY_CARRIER; //0:Empty 1:Not Empty 
                    public VIDITEM_15_DVVAL()
                    {
                        ENPTY_CARRIER = string.Empty;
                    }
                }
               //[Serializable]
               //public class VIDITEM_51_SV : SXFY
               //{
               //    [SecsElement(Index = 1, ListSpreadOut = true)]
               //    //public VIDITEM_10[] ENHANCED_CARRIER_INFO;
               //    public VIDITEM_55_SV[] CARRIER_INFO_OBJ;
               //}

               //[Serializable]
               //public class VIDITEM_52_SV : SXFY
               //{
               //    [SecsElement(Index = 1, ListSpreadOut = true)]
               //    //public VIDITEM_10[] ENHANCED_CARRIER_INFO;
               //    public VIDITEM_66_SV[] TRANSFER_COMMAND_OBJ;
               //}

               //[Serializable]
               //public class VIDITEM_53_SV : SXFY
               //{
               //    [SecsElement(Index = 1, ListSpreadOut = true)]
               //    public VIDITEM_71_SV[] VEHICLEINFO_OBJ;
               //}


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
                    [SecsElement(Index = 3, ListSpreadOut = true)]
                    public VIDITEM_56_DVVAL CARRIER_LOC_OBJ;
                    [SecsElement(Index = 2, ListSpreadOut = true)]
                    public VIDITEM_370_DVVAL CARRIER_ZONE_NAME_OBJ;

                    public VIDITEM_55_DVVAL()
                    {
                        CARRIER_ID_OBJ = new VIDITEM_54_DVVAL();
                        CARRIER_LOC_OBJ = new VIDITEM_56_DVVAL();
                        CARRIER_ZONE_NAME_OBJ = new VIDITEM_370_DVVAL();
                    }

                    public string CARRIER_ID
                    {
                        get { return CARRIER_ID_OBJ?.CARRIER_ID; }
                        set { CARRIER_ID_OBJ.CARRIER_ID = value; }
                    }
                    //public string VEHICLE_ID
                    //{
                    //    get { return VEHICLE_ID_OBJ?.VEHILCE_ID; }
                    //    set { VEHICLE_ID_OBJ.VEHILCE_ID = value; }
                    //}
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
                    public VIDITEM_56_SV CARRIER_LOC_OBJ;
                    [SecsElement(Index = 3, ListSpreadOut = true)]
                    public VIDITEM_370_SV CARRIER_ZONE_NAME_OBJ;

                    public VIDITEM_55_SV()
                    {
                        CARRIER_ID_OBJ = new VIDITEM_54_SV();
                        CARRIER_LOC_OBJ = new VIDITEM_56_SV();
                        CARRIER_ZONE_NAME_OBJ = new VIDITEM_370_SV();
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


                //[Serializable]
                //public class VIDITEM_57_DVVAL : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                //    public string COMMAND_NAME;
                //    public VIDITEM_57_DVVAL()
                //    {
                //        COMMAND_NAME = string.Empty;
                //    }
                //}


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
                //[Serializable]
                //public class VIDITEM_58_SV : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //    public string COMMAND_ID;
                //    public VIDITEM_58_SV()
                //    {
                //        COMMAND_ID = string.Empty;
                //    }
                //}
                //[Serializable]
                //public class VIDITEM_59_DVVAL : SXFY
                //{
                //    [SecsElement(Index = 1)]
                //    public VIDITEM_58_DVVAL COMMAND_ID_OBJ;
                //    [SecsElement(Index = 2)]
                //    public VIDITEM_62_DVVAL PRIORITY_OBJ;
                //    [SecsElement(Index = 3)]
                //    public VIDITEM_63_DVVAL REPLACE_OBJ;
                //    public VIDITEM_59_DVVAL()
                //    {
                //        COMMAND_ID_OBJ = new VIDITEM_58_DVVAL();
                //        PRIORITY_OBJ = new VIDITEM_62_DVVAL();
                //        REPLACE_OBJ = new VIDITEM_63_DVVAL();
                //    }
                //    public string COMMAND_ID
                //    {
                //        get { return COMMAND_ID_OBJ?.COMMAND_ID; }
                //        set { COMMAND_ID_OBJ.COMMAND_ID = value; }
                //    }
                //    public string PRIORITY
                //    {
                //        get { return PRIORITY_OBJ?.PRIORITY; }
                //        set { PRIORITY_OBJ.PRIORITY = value; }
                //    }
                //    public string REPLACE
                //    {
                //        get { return REPLACE_OBJ?.REPLACE; }
                //        set { REPLACE_OBJ.REPLACE = value; }
                //    }

                //}
                //[Serializable]
                //public class VIDITEM_59_SV : SXFY
                //{
                //    [SecsElement(Index = 1)]
                //    public VIDITEM_58_SV COMMAND_ID_OBJ;
                //    [SecsElement(Index = 2)]
                //    public VIDITEM_62_SV PRIORITY_OBJ;
                //    [SecsElement(Index = 3)]
                //    public VIDITEM_63_SV REPLACE_OBJ;
                //    public VIDITEM_59_SV()
                //    {
                //        COMMAND_ID_OBJ = new VIDITEM_58_SV();
                //        PRIORITY_OBJ = new VIDITEM_62_SV();
                //        REPLACE_OBJ = new VIDITEM_63_SV();
                //    }

                //}
                [Serializable]
                public class VIDITEM_60_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string DESTINATION_ID;
                    public VIDITEM_60_DVVAL()
                    {
                        DESTINATION_ID = string.Empty;
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
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
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
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string ERRORID;
                    public VIDITEM_63_DVVAL()
                    {
                        ERRORID = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_63_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
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
                    public string SOURCE_ID;
                    public VIDITEM_65_DVVAL()
                    {
                        SOURCE_ID = string.Empty;
                    }
                }
                //[Serializable]
                //public class VIDITEM_66_DVVAL : SXFY
                //{
                //    [SecsElement(Index = 1)]
                //    public VIDITEM_59_DVVAL COMMAND_INFO;
                //    [SecsElement(Index = 2)]
                //    public VIDITEM_67_DVVAL[] TRANSFER_INFOS;
                //    public VIDITEM_66_DVVAL()
                //    {
                //        COMMAND_INFO = new VIDITEM_59_DVVAL();
                //    }
                //}
                //[Serializable]
                //public class VIDITEM_66_SV : SXFY
                //{
                //    [SecsElement(Index = 1)]
                //    public VIDITEM_59_SV COMMAND_INFO_OBJ;
                //    [SecsElement(Index = 2)]
                //    public VIDITEM_67_SV[] TRANSFER_INFO_OBJ;
                //    public VIDITEM_66_SV()
                //    {
                //        COMMAND_INFO_OBJ = new VIDITEM_59_SV();
                //    }
                //}
                [Serializable]
                public class VIDITEM_66_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string Handoff_Type; //1=manual, 2=automated
                }
                //[Serializable]
                //public class VIDITEM_67_SV : SXFY
                //{
                //    [SecsElement(Index = 1)]
                //    public VIDITEM_54_SV CARRIER_ID_OBJ;
                //    [SecsElement(Index = 2)]
                //    public VIDITEM_78_SV SOURCE_PORT_OBJ;
                //    [SecsElement(Index = 3)]
                //    public VIDITEM_60_SV DESTINATION_PORT_OBJ;
                //    public VIDITEM_67_SV()
                //    {
                //        CARRIER_ID_OBJ = new VIDITEM_54_SV();
                //        SOURCE_PORT_OBJ = new VIDITEM_78_SV();
                //        DESTINATION_PORT_OBJ = new VIDITEM_60_SV();
                //    }

                //    public string CARRIER_ID
                //    {
                //        get { return CARRIER_ID_OBJ?.CARRIER_ID; }
                //        set { CARRIER_ID_OBJ.CARRIER_ID = value; }
                //    }
                //    public string SOURCE_PORT
                //    {
                //        get { return SOURCE_PORT_OBJ?.SOURCE_PORT; }
                //        set { SOURCE_PORT_OBJ.SOURCE_PORT = value; }
                //    }
                //    public string DESTINATION_PORT
                //    {
                //        get { return DESTINATION_PORT_OBJ?.DESTINATION_PORT; }
                //        set { DESTINATION_PORT_OBJ.DESTINATION_PORT = value; }
                //    }

                //}
                [Serializable]
                public class VIDITEM_67_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string Carrier_ID_Read_Status ; //0: successful; 1: failed; 2: duplicate; 3: mismatch
                    public VIDITEM_67_DVVAL()
                    {
                        Carrier_ID_Read_Status = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_68_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string Recoery_Option;
                    public VIDITEM_68_DVVAL()
                    {
                        Recoery_Option = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_70_DVVAL : SXFY
                {
                    //[SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 32)]//變更長度為6 Markchou 20190313
                    public string Crane_ID;
                    public VIDITEM_70_DVVAL()
                    {
                        Crane_ID = string.Empty;
                    }
                }
                public class VIDITEM_70_SV : SXFY
                {
                    //[SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 32)]//變更長度為6 Markchou 20190313
                    public string Crane_ID;
                    public VIDITEM_70_SV()
                    {
                        Crane_ID = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_72_SV : SXFY
                {
                    [SecsElement(Index = 1)]
                    public VIDITEM_83_DVVAL UNITID;
                    [SecsElement(Index = 2)]
                    public VIDITEM_74_DVVAL UNITSTATE;
                    public VIDITEM_72_SV()
                    {
                        UNITSTATE = new VIDITEM_74_DVVAL();
                    }
                }
                [Serializable]

                public class VIDITEM_74_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string Unit_State;
                    public VIDITEM_74_DVVAL()
                    {
                        Unit_State = string.Empty;
                    }
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

                //[Serializable]
                //public class VIDITEM_78_SV : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //    public string SOURCE_PORT;
                //    public VIDITEM_78_SV()
                //    {
                //        SOURCE_PORT = string.Empty;
                //    }
                //}

                //[Serializable]
                //public class VIDITEM_79_DVVAL : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true)]
                //    public TRANSFERCOMPLETEINFO[] TRANCOMPLETEINFO;
                //    public VIDITEM_79_DVVAL()
                //    {
                //        TRANCOMPLETEINFO = new TRANSFERCOMPLETEINFO[1];
                //        TRANCOMPLETEINFO[0] = new TRANSFERCOMPLETEINFO();
                //    }
                //    [Serializable]
                //    public class TRANSFERCOMPLETEINFO : SXFY
                //    {
                //        [SecsElement(Index = 1)]
                //        public VIDITEM_67_DVVAL TRANSFER_INFO_OBJ;
                //        [SecsElement(Index = 2)]
                //        public VIDITEM_56_DVVAL CARRIER_LOC_OBJ;
                //        public TRANSFERCOMPLETEINFO()
                //        {
                //            TRANSFER_INFO_OBJ = new VIDITEM_67_DVVAL();
                //            CARRIER_LOC_OBJ = new VIDITEM_56_DVVAL();
                //        }

                //        public string CARRIER_LOC
                //        {
                //            get { return CARRIER_LOC_OBJ?.CARRIER_LOC; }
                //            set { CARRIER_LOC_OBJ.CARRIER_LOC = value; }
                //        }
                //    }
                //}


                [Serializable]
                public class VIDITEM_80_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string COMMAND_TYPE;  // Cancel; Abort; Transfer 
                    public VIDITEM_80_DVVAL()
                    {
                        COMMAND_TYPE = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_81_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string ALARM_ID;
                    public VIDITEM_81_DVVAL()
                    {
                        ALARM_ID = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_82_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 128)]
                    public string ALARM_TEXT;
                    public VIDITEM_82_DVVAL()
                    {
                        ALARM_TEXT = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_83_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string UNIT_ID;
                    public VIDITEM_83_DVVAL()
                    {
                        UNIT_ID = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_84_DVVAL : SXFY
                {
                    [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string ERROR_NUNBER;
                    public VIDITEM_84_DVVAL()
                    {
                        ERROR_NUNBER = string.Empty;
                    }
                }
                //[Serializable]
                //public class VIDITEM_91_SV : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true)]
                //    public VIDITEM_75_DVVAL[] ENHANCED_CARRIER_INFOS;
                //    public VIDITEM_91_SV()
                //    {

                //    }
                //}

                [Serializable]
                public class VIDITEM_114_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string SPEC_VERSION;
                    public VIDITEM_114_DVVAL()
                    {
                        SPEC_VERSION = string.Empty;
                    }
                }

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
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string PORT_TYPE; //0=input, 1=Output
                    public VIDITEM_116_DVVAL()
                    {
                        PORT_TYPE = string.Empty;
                    }
                        
                }

                [Serializable]
                public class VIDITEM_172_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string ZONE_NAME;
                    [SecsElement(Index = 2, ListSpreadOut = true)]
                    public VIDITEM_174_DVVAL ZONE_CAPACITY_OBJ;
                    [SecsElement(Index = 3, ListSpreadOut = true)]
                    public VIDITEM_175_DVVAL ZONE_TOTAL_SIZE_OBJ;
                    [SecsElement(Index = 4, ListSpreadOut = true)]
                    public VIDITEM_176_DVVAL ZONE_TYPE_OBJ;
                    [SecsElement(Index = 5, ListSpreadOut = true)]
                    public VIDITEM_888_SV DISABLE_LOCATIONS_OBJ;
                    public VIDITEM_172_SV()
                    {
                        ZONE_NAME = string.Empty;
                        ZONE_CAPACITY_OBJ = new VIDITEM_174_DVVAL();
                        ZONE_TOTAL_SIZE_OBJ = new VIDITEM_175_DVVAL();
                        ZONE_TYPE_OBJ = new VIDITEM_176_DVVAL();
                        DISABLE_LOCATIONS_OBJ = new VIDITEM_888_SV();
                    }
                }
                [Serializable]
                public class VIDITEM_174_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string Zone_Capacity;
                    public VIDITEM_174_DVVAL()
                    {
                        Zone_Capacity = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_175_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string Zone_Total_Size;
                    public VIDITEM_175_DVVAL()
                    {
                        Zone_Total_Size = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_176_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string Zone_Type; //1: Shelf 2: Port 3: Other 9: HandOff
                    public VIDITEM_176_DVVAL()
                    {
                        Zone_Type = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_177_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string Zone_Name; //1: Shelf 2: Port 3: Other 9: HandOff
                    [SecsElement(Index = 2)]
                    VIDITEM_174_DVVAL Zone_Capacity_OBJ;
                    [SecsElement(Index = 3)]
                    VIDITEM_175_DVVAL Zone_Total_Size_OBJ;
                    [SecsElement(Index = 4)]
                    VIDITEM_176_DVVAL Zone_Type_OBJ;
                    public VIDITEM_177_SV()
                    {
                        Zone_Name = string.Empty;
                        Zone_Capacity_OBJ = new VIDITEM_174_DVVAL();
                        Zone_Total_Size_OBJ = new VIDITEM_175_DVVAL();
                        Zone_Type_OBJ = new VIDITEM_176_DVVAL();
                    }
                }

                [Serializable]
                public class VIDITEM_179_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string BOX_ID;
                    public VIDITEM_179_DVVAL()
                    {
                        BOX_ID = string.Empty;
                    }
                }
                //[Serializable]
                //public class VIDITEM_117_DVVAL : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //    public string VEHICLE_LOCATION;
                //    public VIDITEM_117_DVVAL()
                //    {
                //        VEHICLE_LOCATION = string.Empty;
                //    }
                //}

                //[Serializable]
                //public class VIDITEM_118_DVVAL : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true)]
                //    public VIDITEM_PORT_INFO[] PORT_INFO;
                //}

                //[Serializable]
                //public class VIDITEM_119_SV : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true)]
                //    public ENHANCED_VEHICLE_INFO[] ENHANCED_VEHICLE_INFO_OBJ;
                //    public VIDITEM_119_SV()
                //    {

                //    }
                //    public class ENHANCED_VEHICLE_INFO : SXFY
                //    {
                //        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 6)]
                //        public string VehicleID;
                //        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //        public string VehicleState;
                //        [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //        public string VehicleLocation;
                //    }
                //}

                //[Serializable]
                //public class VIDITEM_120_DVVAL : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                //    public string UNIT_STATUS_CLEABLE;
                //    public VIDITEM_120_DVVAL()
                //    {
                //        UNIT_STATUS_CLEABLE = string.Empty;
                //    }
                //}

                //[Serializable]
                //public class VIDITEM_202_DVVAL : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //    public string TRANSFER_STATE;
                //    public VIDITEM_202_DVVAL()
                //    {
                //        TRANSFER_STATE = string.Empty;
                //    }
                //}
                //[Serializable]
                //public class VIDITEM_202_SV : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //    public string TRANSFER_STATE;
                //    public VIDITEM_202_SV()
                //    {
                //        TRANSFER_STATE = string.Empty;
                //    }
                //}
                [Serializable]
                public class VIDITEM_203_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string Carrier_State; //1: Wait in 2: Transferring 3: Completed 4: Alternate 5: Wait out 6: Installed
                    public VIDITEM_203_DVVAL()
                    {
                        Carrier_State = string.Empty;
                    }
                }

                //[Serializable]
                //public class VIDITEM_204_DVVAL : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 16)]
                //    public string INSTALL_TIME;
                //    public VIDITEM_204_DVVAL()
                //    {
                //        INSTALL_TIME = string.Empty;
                //    }
                //}

                //[Serializable]
                //public class VIDITEM_204_SV : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 16)]
                //    public string INSTALL_TIME;
                //    public VIDITEM_204_SV()
                //    {
                //        INSTALL_TIME = string.Empty;
                //    }
                //}

                //[Serializable]
                //public class VIDITEM_205_DVVAL : SXFY
                //{
                //    [SecsElement(Index = 1)]
                //    public VIDITEM_59_DVVAL COMMAND_INFO_OBJ;
                //    [SecsElement(Index = 2)]
                //    public VIDITEM_202_DVVAL TRANSFER_STATE_OBJ;
                //    [SecsElement(Index = 3)]
                //    public VIDITEM_67_DVVAL[] TRANSFER_INFO_OBJ;
                //    public VIDITEM_205_DVVAL()
                //    {
                //        COMMAND_INFO_OBJ = new VIDITEM_59_DVVAL();
                //        TRANSFER_STATE_OBJ = new VIDITEM_202_DVVAL();
                //    }
                //}

                //[Serializable]
                //public class VIDITEM_205_SV : SXFY
                //{
                //    [SecsElement(Index = 1)]
                //    public VIDITEM_59_SV COMMAND_INFO_OBJ;
                //    [SecsElement(Index = 2)]
                //    public VIDITEM_202_SV TRANSFER_STATE_OBJ;
                //    [SecsElement(Index = 3)]
                //    public VIDITEM_67_SV[] TRANSFER_INFO_OBJ;
                //    public VIDITEM_205_SV()
                //    {
                //        COMMAND_INFO_OBJ = new VIDITEM_59_SV();
                //        TRANSFER_STATE_OBJ = new VIDITEM_202_SV();
                //    }
                //}

                //[Serializable]
                //public class VIDITEM_211_DVVAL : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //    public string UNIT_ID;
                //    public VIDITEM_211_DVVAL()
                //    {
                //        UNIT_ID = string.Empty;
                //    }
                //}

                //[Serializable]
                //public class VIDITEM_212_DVVAL : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                //    public string ALARM_TEXT;
                //    public VIDITEM_212_DVVAL()
                //    {
                //        ALARM_TEXT = string.Empty;
                //    }
                //}

                //[Serializable]
                //public class VIDITEM_251_DVVAL : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //    public string VEHICLE_CURRENT_POSITION;
                //    public VIDITEM_251_DVVAL()
                //    {
                //        VEHICLE_CURRENT_POSITION = string.Empty;
                //    }
                //}

                //[Serializable]
                //public class VIDITEM_252_SV : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true)]
                //    public MONITORED_VEHICLE_INFO[] MONITORED_VEHICLE_INFO_OBJ;
                //    public VIDITEM_252_SV()
                //    {

                //    }
                //    public class MONITORED_VEHICLE_INFO : SXFY
                //    {
                //        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 6)]
                //        public string VehicleID;
                //        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //        public string VehicleCurrentPosition;
                //        [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //        public string VehicleDistanceFromCurrentPosition;
                //        [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 32)]
                //        public string VehicleCurrentDomain;
                //        [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //        public string VehicleNextPosition;
                //        [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //        public string VehicleOperationState;
                //        [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //        public string VehicleCommunicationState;
                //        [SecsElement(Index = 8, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //        public string VehicleControlMode;
                //        [SecsElement(Index = 9, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //        public string VehicleJamState;
                //        [SecsElement(Index = 10, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //        public string AlarmID;
                //    }
                //}

                //[Serializable]
                //public class VIDITEM_253_SV : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 32)]
                //    public string VEHICLE_CURRENT_DOMAIN;
                //    public VIDITEM_253_SV()
                //    {
                //        VEHICLE_CURRENT_DOMAIN = string.Empty;
                //    }
                //}

                //[Serializable]
                //public class VIDITEM_254_SV : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true)]
                //    public UNIT_ALARM_INFO[] UNIT_ALARMS_INFO_OBJ;
                //    public VIDITEM_254_SV()
                //    {

                //    }
                //    public class UNIT_ALARM_INFO : SXFY
                //    {
                //        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //        public string UnitID;
                //        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //        public string VehicleCurrentPosition;
                //        [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //        public string VehicleNextPosition;
                //        [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //        public string AlarmID;
                //        [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                //        public string AlarmText;
                //        [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //        public string VehicleCommunicationState;
                //        [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //        public string MainteState;
                //    }
                //}

                //[Serializable]
                //public class VIDITEM_262_DVVAL : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //    public string VEHICLE_NEXT_POSITION;
                //    public VIDITEM_262_DVVAL()
                //    {
                //        VEHICLE_NEXT_POSITION = string.Empty;
                //    }
                //}

                //[Serializable]
                //public class VIDITEM_301_DVVAL : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //    public string PAUSE_REASON;
                //    public VIDITEM_301_DVVAL()
                //    {
                //        PAUSE_REASON = string.Empty;
                //    }
                //}

                //[Serializable]
                //public class VIDITEM_330_DVVAL : SXFY
                //{
                //    [SecsElement(Index = 1)]
                //    public LANE_INFO LANE_INFO_OBJ;
                //    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //    public string LANE_CUT_TYPE;
                //    public VIDITEM_330_DVVAL()
                //    {
                //        LANE_INFO_OBJ = new LANE_INFO();
                //        LANE_CUT_TYPE = string.Empty;
                //    }
                //    [Serializable]
                //    public class LANE_INFO : SXFY
                //    {
                //        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //        public string StartPoint;
                //        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //        public string EndPoint;
                //    }
                //}


                //[Serializable]
                //public class VIDITEM_330_SV : SXFY
                //{
                //    [SecsElement(Index = 1)]
                //    public LANE_INFO LANE_INFO_OBJ;
                //    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //    public string LANE_CUT_TYPE;
                //    public VIDITEM_330_SV()
                //    {
                //        LANE_INFO_OBJ = new LANE_INFO();
                //        LANE_CUT_TYPE = string.Empty;
                //    }
                //    public class LANE_INFO : SXFY
                //    {
                //        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //        public string StartPoint;
                //        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //        public string EndPoint;
                //    }
                //}


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

                [Serializable]
                public class VIDITEM_352_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string EQ_REQ_STATUS; //0: Load REQ & Unload REQ off 1: Load REQ on 2: Unload REQ on 
                    public VIDITEM_352_DVVAL()
                    {
                        EQ_REQ_STATUS = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_353_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string EQ_PRESENCE_STATUS; //0: No presence 1: Presence 
                    public VIDITEM_353_DVVAL()
                    {
                        EQ_PRESENCE_STATUS = string.Empty;
                    }
                }

                //[Serializable]
                //public class VIDITEM_PORT_INFO : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true)]
                //    public VIDITEM_115_DVVAL PORT_ID_OBJ;
                //    [SecsElement(Index = 2, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //    public string PORT_TRANSFER_STATE;
                //    public VIDITEM_PORT_INFO()
                //    {
                //        PORT_ID_OBJ = new VIDITEM_115_DVVAL();
                //    }
                //    public string PORT_ID
                //    {
                //        get { return PORT_ID_OBJ.PORT_ID; }
                //        set { PORT_ID_OBJ.PORT_ID = value; }
                //    }
                //}

                [Serializable]
                public class VIDITEM_354_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_115_DVVAL PORT_ID_OBJ;
                    [SecsElement(Index = 2, ListSpreadOut = true)]
                    public VIDITEM_355_DVVAL PORT_TRANSFTER_STATE_OBJ;
                    public VIDITEM_354_SV()
                    {
                        PORT_ID_OBJ = new VIDITEM_115_DVVAL();
                        PORT_TRANSFTER_STATE_OBJ = new VIDITEM_355_DVVAL();
                    }
                }

                [Serializable]
                public class VIDITEM_355_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string PORT_TRANSFER_STATE; //1=OutOfService 2=InService 
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
                public class VIDITEM_356_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_115_DVVAL PORT_ID_OBJ;
                    [SecsElement(Index = 2, ListSpreadOut = true)]
                    public VIDITEM_355_DVVAL PORT_TRANSFTER_STATE_OBJ;
                    [SecsElement(Index = 3, ListSpreadOut = true)]
                    public VIDITEM_352_DVVAL EQ_REQ_SATUS_OBJ;
                    [SecsElement(Index = 4, ListSpreadOut = true)]
                    public VIDITEM_353_DVVAL EQ_PRESENCE_STATUS_OBJ;
                    public VIDITEM_356_SV()
                    {
                        PORT_ID_OBJ = new VIDITEM_115_DVVAL();
                        PORT_TRANSFTER_STATE_OBJ = new VIDITEM_355_DVVAL();
                        EQ_REQ_SATUS_OBJ = new VIDITEM_352_DVVAL();
                        EQ_PRESENCE_STATUS_OBJ = new VIDITEM_353_DVVAL();
                    }
                }

                //[Serializable]
                //public class VIDITEM_360_SV : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true)]
                //    public VIDITEM_330_SV[] LANE_CUT_INFO;
                //    public VIDITEM_360_SV()
                //    {
                //    }
                //}


                [Serializable]
                public class VIDITEM_361_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_83_DVVAL UNIT_ID_OBJ;
                    [SecsElement(Index = 2, ListSpreadOut = true)]
                    public VIDITEM_81_DVVAL ALARM_ID_OBJ;
                    [SecsElement(Index = 3, ListSpreadOut = true)]
                    public VIDITEM_82_DVVAL ALARM_TEXT_OBJ;
                    [SecsElement(Index = 4, ListSpreadOut = true)]
                    public VIDITEM_362_DVVAL MAINT_STATE_OBJ;
                    public VIDITEM_361_SV()
                    {
                        UNIT_ID_OBJ = new VIDITEM_83_DVVAL();
                        ALARM_ID_OBJ = new VIDITEM_81_DVVAL();
                        ALARM_TEXT_OBJ = new VIDITEM_82_DVVAL();
                        MAINT_STATE_OBJ = new VIDITEM_362_DVVAL();
                    }
                }

                [Serializable]
                public class VIDITEM_362_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string MAINT_STATE; //1=Maintenance 2=Not Maintenance 
                    public VIDITEM_362_DVVAL()
                    {
                        MAINT_STATE = string.Empty;
                    }
                }
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

                //[Serializable]
                //public class VIDITEM_370_SV : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true)]
                //    public STAGE_COMMAND[] STAGE_COMMAND_OBJ;
                //    public VIDITEM_370_SV()
                //    {
                //        STAGE_COMMAND_OBJ = new STAGE_COMMAND[1];
                //        STAGE_COMMAND_OBJ[0] = new STAGE_COMMAND();
                //    }

                //    public class STAGE_COMMAND
                //    {
                //        [SecsElement(Index = 1)]
                //        STAGE_INFO StageInfo;
                //        [SecsElement(Index = 2)]
                //        VIDITEM_67_SV TransferInfo;
                //        public STAGE_COMMAND()
                //        {
                //            StageInfo = new STAGE_INFO();
                //            TransferInfo = new VIDITEM_67_SV();
                //        }

                //        public class STAGE_INFO
                //        {
                //            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //            string StageID;
                //            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //            string Priority;
                //            [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //            string ExpectedDuration;
                //            [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //            string NoBlockingTime;
                //            [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                //            string WaitTimeout;
                //            public STAGE_INFO()
                //            {
                //                StageID = string.Empty;
                //                Priority = string.Empty;
                //                ExpectedDuration = string.Empty;
                //                NoBlockingTime = string.Empty;
                //                WaitTimeout = string.Empty;
                //            }
                //        }
                //    }
                //}
                [Serializable]
                public class VIDITEM_370_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true,Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string CARRIER_ZONE_NAME;
                    public VIDITEM_370_DVVAL()
                    {
                        CARRIER_ZONE_NAME = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_370_SV : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    string CARRIER_ZONE_NAME;
                    public VIDITEM_370_SV()
                    {
                        CARRIER_ZONE_NAME = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_601_SV : SXFY
                {
                    [SecsElement(Index = 1)]
                    public VIDITEM_115_DVVAL PORT_ID_OBJ;
                    [SecsElement(Index = 2)]
                    public VIDITEM_602_DVVAL PORT_UNIT_TYPE_OBJ;
                    public VIDITEM_601_SV()
                    {
                        PORT_ID_OBJ = new VIDITEM_115_DVVAL();
                        PORT_UNIT_TYPE_OBJ = new VIDITEM_602_DVVAL();
                    }
                }

                [Serializable]
                public class VIDITEM_602_DVVAL : SXFY
                {
                    [SecsElement(Index = 1,ListSpreadOut =true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string PORT_UNIT_TYPE; //0= Input 1= Output
                    public VIDITEM_602_DVVAL()
                    {
                        PORT_UNIT_TYPE = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_720_SV : SXFY
                {
                    [SecsElement(Index = 1)]
                    public VIDITEM_54_DVVAL CARRIER_ID_OBJ;
                    [SecsElement(Index = 2)]
                    public VIDITEM_179_DVVAL BOX_ID_OBJ; 
                    [SecsElement(Index = 3)]
                    public VIDITEM_65_DVVAL SOURCE_ID_OBJ ;
                    [SecsElement(Index = 4)]
                    public VIDITEM_60_DVVAL DESTINATION_ID_OBJ ;
                    public VIDITEM_720_SV()
                    {
                        CARRIER_ID_OBJ = new VIDITEM_54_DVVAL();
                        BOX_ID_OBJ = new VIDITEM_179_DVVAL();
                        SOURCE_ID_OBJ = new VIDITEM_65_DVVAL();
                        DESTINATION_ID_OBJ = new VIDITEM_60_DVVAL();
                    }
                }
                [Serializable]
                public class VIDITEM_722_DVVAL : SXFY
                {
                    [SecsElement(Index = 1,ListSpreadOut =true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string Transfer_State; //1=Queued 2=Transferring 3=Paused 4=Canceling 5=Aborting
                    public VIDITEM_722_DVVAL()
                    {
                        Transfer_State = string.Empty;
                    }
                }
                [Serializable]
                public class VIDITEM_886_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    VIDITEM_55_SV[] CARRIER_INFO; //1=Queued 2=Transferring 3=Paused 4=Canceling 5=Aborting
                    public VIDITEM_886_SV()
                    {
                        CARRIER_INFO = new VIDITEM_55_SV[0];
                    }
                }

                [Serializable]
                public class VIDITEM_888_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_889_SV[] DISABLE_LOC_OBJ;
                    public VIDITEM_888_SV()
                    {
                        DISABLE_LOC_OBJ = new VIDITEM_889_SV[0];
                    }
                }

                [Serializable]
                public class VIDITEM_889_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    public VIDITEM_56_DVVAL CARRIER_LOC_OBJ; 
                    [SecsElement(Index = 2, ListSpreadOut = true)]
                    public VIDITEM_54_DVVAL CARRIER_ID_OBJ; 
                    public VIDITEM_889_SV()
                    {
                        CARRIER_LOC_OBJ = new VIDITEM_56_DVVAL();
                        CARRIER_ID_OBJ = new VIDITEM_54_DVVAL();
                    }
                }
                [Serializable]
                public class VIDITEM_890_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                    public string REQUEST_COUNT;
                    public VIDITEM_890_DVVAL()
                    {
                        REQUEST_COUNT = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_891_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string CRANE_CURRENT_POSITION;
                    public VIDITEM_891_DVVAL()
                    {
                        CRANE_CURRENT_POSITION = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_892_DVVAL : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                    public string DISTANCE_FROM_HP_TO_OP;
                    public VIDITEM_892_DVVAL()
                    {
                        DISTANCE_FROM_HP_TO_OP = string.Empty;
                    }
                }

                [Serializable]
                public class VIDITEM_893_SV : SXFY
                {
                    [SecsElement(Index = 1, ListSpreadOut = true)]
                    VIDITEM_70_DVVAL CRANE_ID_OBJ;
                    [SecsElement(Index = 2, ListSpreadOut = true)]
                    VIDITEM_891_DVVAL CRANE_CURRENT_POSITION_OBJ;
                    [SecsElement(Index = 3, ListSpreadOut = true)]
                    VIDITEM_892_DVVAL DISTANCE_FROM_HP_TO_OP_OBJ;
                    public VIDITEM_893_SV()
                    {
                        CRANE_ID_OBJ = new VIDITEM_70_DVVAL();
                        CRANE_CURRENT_POSITION_OBJ = new VIDITEM_891_DVVAL();
                        DISTANCE_FROM_HP_TO_OP_OBJ = new VIDITEM_892_DVVAL();
                    }
                }
                //[Serializable]
                //public class VIDITEM_371_SV : SXFY
                //{
                //    [SecsElement(Index = 1, ListSpreadOut = true)]
                //    public STAGE_VEHICLE_INFO[] STAGE_VEHICLE_INFO_OBJ;
                //    public VIDITEM_371_SV()
                //    {
                //        STAGE_VEHICLE_INFO_OBJ = new STAGE_VEHICLE_INFO[1];
                //        STAGE_VEHICLE_INFO_OBJ[0] = new STAGE_VEHICLE_INFO();
                //    }

                //    public class STAGE_VEHICLE_INFO
                //    {
                //        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 6)]
                //        string VehicleID;
                //        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //        string StageID;
                //        [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
                //        string VehicleLocation;
                //        public STAGE_VEHICLE_INFO()
                //        {
                //            VehicleID = string.Empty;
                //            StageID = string.Empty;
                //            VehicleLocation = string.Empty;
                //        }
                //    }
                //}
                #endregion
            }
        }
    }
    public class VIDCollection
    {
        public VIDCollection()
        {
            VIDITEM_04_SV_AlarmsSet = new S6F11.RPTINFO.RPTITEM.VIDITEM_04_SV();
            VIDITEM_06_SV_ControlState = new S6F11.RPTINFO.RPTITEM.VIDITEM_06_SV();
            VIDITEM_51_SV_EnhancedCarriers = new S6F11.RPTINFO.RPTITEM.VIDITEM_51_SV();
            VIDITEM_73_DVVAL_SCState = new S6F11.RPTINFO.RPTITEM.VIDITEM_73_SV();
            VIDITEM_76_SV_EnhancedTransfers = new S6F11.RPTINFO.RPTITEM.VIDITEM_76_SV();
            VIDITEM_107_SV_EnhancedActiveZones = new S6F11.RPTINFO.RPTITEM.VIDITEM_107_SV();
            VIDITEM_118_SV_CurrentPortStates = new S6F11.RPTINFO.RPTITEM.VIDITEM_118_SV();
            VIDITEM_350_SV_CurrEqPortStatus = new S6F11.RPTINFO.RPTITEM.VIDITEM_350_SV();
            VIDITEM_351_SV_PortTypes = new S6F11.RPTINFO.RPTITEM.VIDITEM_351_SV();
            VIDITEM_360_SV_UnitAlarmList = new S6F11.RPTINFO.RPTITEM.VIDITEM_360_SV();

            VIDITEM_10_SV_EnhancedCarrierInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_10_SV();
            VIDITEM_11_SV_CommandInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_11_SV();
            VIDITEM_12_DVVAL_InstallTime = new S6F11.RPTINFO.RPTITEM.VIDITEM_12_DVVAL();
            VIDITEM_13_SV_EnhancedTransferCmd = new S6F11.RPTINFO.RPTITEM.VIDITEM_13_SV();
            VIDITEM_15_DVVAL_EmptyCarrier = new S6F11.RPTINFO.RPTITEM.VIDITEM_15_DVVAL();

            VIDITEM_54_DVVAL_CarrierID = new S6F11.RPTINFO.RPTITEM.VIDITEM_54_DVVAL();
            VIDITEM_54_SV_CarrierID = new S6F11.RPTINFO.RPTITEM.VIDITEM_54_SV();
            VIDITEM_55_DVVAL_CarrierInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_55_DVVAL();
            VIDITEM_55_SV_CarrierInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_55_SV();
            VIDITEM_56_DVVAL_CarrierLoc = new S6F11.RPTINFO.RPTITEM.VIDITEM_56_DVVAL();
            VIDITEM_56_SV_CarrierLoc = new S6F11.RPTINFO.RPTITEM.VIDITEM_56_SV();
            VIDITEM_58_DVVAL_CommandID = new S6F11.RPTINFO.RPTITEM.VIDITEM_58_DVVAL();

            VIDITEM_60_DVVAL_DestPort = new S6F11.RPTINFO.RPTITEM.VIDITEM_60_DVVAL();
            VIDITEM_60_SV_DestPort = new S6F11.RPTINFO.RPTITEM.VIDITEM_60_SV();
            VIDITEM_61_ECV_EqpName = new S6F11.RPTINFO.RPTITEM.VIDITEM_61_ECV();
            VIDITEM_62_DVVAL_Priority = new S6F11.RPTINFO.RPTITEM.VIDITEM_62_DVVAL();
            VIDITEM_62_SV_Priority = new S6F11.RPTINFO.RPTITEM.VIDITEM_62_SV();
            VIDITEM_63_DVVAL_ErrorId = new S6F11.RPTINFO.RPTITEM.VIDITEM_63_DVVAL();
            VIDITEM_63_SV_ErrorId = new S6F11.RPTINFO.RPTITEM.VIDITEM_63_SV();
            VIDITEM_64_DVVAL_ResultCode = new S6F11.RPTINFO.RPTITEM.VIDITEM_64_DVVAL();
            VIDITEM_65_DVVAL_SourceID = new S6F11.RPTINFO.RPTITEM.VIDITEM_65_DVVAL();
            VIDITEM_66_DVVAL_HandoffType = new S6F11.RPTINFO.RPTITEM.VIDITEM_66_DVVAL();
            VIDITEM_67_DVVAL_IDreadStatus = new S6F11.RPTINFO.RPTITEM.VIDITEM_67_DVVAL();
            VIDITEM_68_DVVAL_RecoeryOption = new S6F11.RPTINFO.RPTITEM.VIDITEM_68_DVVAL();

            VIDITEM_70_DVVAL_CraneID = new S6F11.RPTINFO.RPTITEM.VIDITEM_70_DVVAL();
            VIDITEM_70_SV_CraneID = new S6F11.RPTINFO.RPTITEM.VIDITEM_70_SV();
            VIDITEM_72_SV_UnitInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_72_SV();
            VIDITEM_74_DVVAL_UnitState = new S6F11.RPTINFO.RPTITEM.VIDITEM_74_DVVAL();

            VIDITEM_80_DVVAL_CommandType = new S6F11.RPTINFO.RPTITEM.VIDITEM_80_DVVAL();
            VIDITEM_81_DVVAL_AlarmID = new S6F11.RPTINFO.RPTITEM.VIDITEM_81_DVVAL();
            VIDITEM_82_DVVAL_AlarmText = new S6F11.RPTINFO.RPTITEM.VIDITEM_82_DVVAL();
            VIDITEM_83_DVVAL_UnitID = new S6F11.RPTINFO.RPTITEM.VIDITEM_83_DVVAL();
            VIDITEM_84_DVVAL_ErrorNumber = new S6F11.RPTINFO.RPTITEM.VIDITEM_84_DVVAL();

            VIDITEM_114_DVVAL_SpecVersion = new S6F11.RPTINFO.RPTITEM.VIDITEM_114_DVVAL();
            VIDITEM_115_DVVAL_PortID = new S6F11.RPTINFO.RPTITEM.VIDITEM_115_DVVAL();
            VIDITEM_115_SV_PortID = new S6F11.RPTINFO.RPTITEM.VIDITEM_115_SV();
            VIDITEM_116_DVVAL_PortType = new S6F11.RPTINFO.RPTITEM.VIDITEM_116_DVVAL();

            VIDITEM_172_SV_ZoneData = new S6F11.RPTINFO.RPTITEM.VIDITEM_172_SV();
            VIDITEM_174_DVVAL_ZoneCapacity = new S6F11.RPTINFO.RPTITEM.VIDITEM_174_DVVAL();
            VIDITEM_175_DVVAL_ZoneTotalSize = new S6F11.RPTINFO.RPTITEM.VIDITEM_175_DVVAL();
            VIDITEM_176_DVVAL_ZoneType = new S6F11.RPTINFO.RPTITEM.VIDITEM_176_DVVAL();
            VIDITEM_177_SV_EnhancedZoneData = new S6F11.RPTINFO.RPTITEM.VIDITEM_177_SV();
            VIDITEM_179_DVVAL_BOXID = new S6F11.RPTINFO.RPTITEM.VIDITEM_179_DVVAL();

            VIDITEM_203_DVVAL_CarrierState = new S6F11.RPTINFO.RPTITEM.VIDITEM_203_DVVAL();

            VIDITEM_352_DVVAL_EqReqSatus = new S6F11.RPTINFO.RPTITEM.VIDITEM_352_DVVAL();
            VIDITEM_353_DVVAL_EqPresenceStatus = new S6F11.RPTINFO.RPTITEM.VIDITEM_353_DVVAL();
            VIDITEM_354_SV_PortInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_354_SV();
            VIDITEM_355_DVVAL_PortTransferState = new S6F11.RPTINFO.RPTITEM.VIDITEM_355_DVVAL();
            VIDITEM_355_SV_PortTransferState = new S6F11.RPTINFO.RPTITEM.VIDITEM_355_SV();
            VIDITEM_356_SV_EqPortInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_356_SV();

            VIDITEM_361_SV_UnitAlarmInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_361_SV();
            VIDITEM_362_DVVAL_MaintState = new S6F11.RPTINFO.RPTITEM.VIDITEM_362_DVVAL();

            VIDITEM_370_DVVAL_CarrierZoneName = new S6F11.RPTINFO.RPTITEM.VIDITEM_370_DVVAL();
            VIDITEM_370_SV_CarrierZoneName = new S6F11.RPTINFO.RPTITEM.VIDITEM_370_SV();

            VIDITEM_601_SV_PortTypeInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_601_SV();
            VIDITEM_602_DVVAL_PortUnitType = new S6F11.RPTINFO.RPTITEM.VIDITEM_602_DVVAL();

            VIDITEM_720_SV_TransferInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_720_SV();
            VIDITEM_722_DVVAL_TransferState = new S6F11.RPTINFO.RPTITEM.VIDITEM_722_DVVAL();

            VIDITEM_886_SV_CarrierLocations = new S6F11.RPTINFO.RPTITEM.VIDITEM_886_SV();
            VIDITEM_888_SV_DisabledLocations = new S6F11.RPTINFO.RPTITEM.VIDITEM_888_SV();
            VIDITEM_889_SV_DisabledLoc = new S6F11.RPTINFO.RPTITEM.VIDITEM_889_SV();
            VIDITEM_890_DVVAL_RequestCount = new S6F11.RPTINFO.RPTITEM.VIDITEM_890_DVVAL();
            VIDITEM_891_DVVAL_CraneCurrentPosition = new S6F11.RPTINFO.RPTITEM.VIDITEM_891_DVVAL();
            VIDITEM_892_DVVAL_CraneTotalDistance = new S6F11.RPTINFO.RPTITEM.VIDITEM_892_DVVAL();
            VIDITEM_893_SV_MonitoredCraneInfo = new S6F11.RPTINFO.RPTITEM.VIDITEM_893_SV();
        }
        public string VH_ID;

        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_04_SV VIDITEM_04_SV_AlarmsSet;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_06_SV VIDITEM_06_SV_ControlState;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_51_SV VIDITEM_51_SV_EnhancedCarriers;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_73_SV VIDITEM_73_DVVAL_SCState;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_76_SV VIDITEM_76_SV_EnhancedTransfers;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_107_SV VIDITEM_107_SV_EnhancedActiveZones;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_118_SV VIDITEM_118_SV_CurrentPortStates;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_350_SV VIDITEM_350_SV_CurrEqPortStatus;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_351_SV VIDITEM_351_SV_PortTypes;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_360_SV VIDITEM_360_SV_UnitAlarmList;

        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_10_SV VIDITEM_10_SV_EnhancedCarrierInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_11_SV VIDITEM_11_SV_CommandInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_12_DVVAL VIDITEM_12_DVVAL_InstallTime;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_13_SV VIDITEM_13_SV_EnhancedTransferCmd;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_15_DVVAL VIDITEM_15_DVVAL_EmptyCarrier;

        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_54_DVVAL VIDITEM_54_DVVAL_CarrierID;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_54_SV VIDITEM_54_SV_CarrierID;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_55_DVVAL VIDITEM_55_DVVAL_CarrierInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_55_SV VIDITEM_55_SV_CarrierInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_56_DVVAL VIDITEM_56_DVVAL_CarrierLoc;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_56_SV VIDITEM_56_SV_CarrierLoc;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_58_DVVAL VIDITEM_58_DVVAL_CommandID;

        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_60_DVVAL VIDITEM_60_DVVAL_DestPort;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_60_SV VIDITEM_60_SV_DestPort;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_61_ECV VIDITEM_61_ECV_EqpName;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_62_DVVAL VIDITEM_62_DVVAL_Priority;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_62_SV VIDITEM_62_SV_Priority;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_63_DVVAL VIDITEM_63_DVVAL_ErrorId;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_63_SV VIDITEM_63_SV_ErrorId;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_64_DVVAL VIDITEM_64_DVVAL_ResultCode;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_65_DVVAL VIDITEM_65_DVVAL_SourceID;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_66_DVVAL VIDITEM_66_DVVAL_HandoffType;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_67_DVVAL VIDITEM_67_DVVAL_IDreadStatus;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_68_DVVAL VIDITEM_68_DVVAL_RecoeryOption;
      
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_70_DVVAL VIDITEM_70_DVVAL_CraneID;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_70_SV VIDITEM_70_SV_CraneID;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_72_SV VIDITEM_72_SV_UnitInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_74_DVVAL VIDITEM_74_DVVAL_UnitState;

        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_80_DVVAL VIDITEM_80_DVVAL_CommandType;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_81_DVVAL VIDITEM_81_DVVAL_AlarmID;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_82_DVVAL VIDITEM_82_DVVAL_AlarmText;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_83_DVVAL VIDITEM_83_DVVAL_UnitID;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_84_DVVAL VIDITEM_84_DVVAL_ErrorNumber;
        
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_114_DVVAL VIDITEM_114_DVVAL_SpecVersion;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_115_DVVAL VIDITEM_115_DVVAL_PortID;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_115_SV VIDITEM_115_SV_PortID;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_116_DVVAL VIDITEM_116_DVVAL_PortType;

        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_172_SV VIDITEM_172_SV_ZoneData;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_174_DVVAL VIDITEM_174_DVVAL_ZoneCapacity;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_175_DVVAL VIDITEM_175_DVVAL_ZoneTotalSize;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_176_DVVAL VIDITEM_176_DVVAL_ZoneType;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_177_SV VIDITEM_177_SV_EnhancedZoneData;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_179_DVVAL VIDITEM_179_DVVAL_BOXID;

        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_203_DVVAL VIDITEM_203_DVVAL_CarrierState;

        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_352_DVVAL VIDITEM_352_DVVAL_EqReqSatus;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_353_DVVAL VIDITEM_353_DVVAL_EqPresenceStatus;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_354_SV VIDITEM_354_SV_PortInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_355_DVVAL VIDITEM_355_DVVAL_PortTransferState;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_355_SV VIDITEM_355_SV_PortTransferState;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_356_SV VIDITEM_356_SV_EqPortInfo;

        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_361_SV VIDITEM_361_SV_UnitAlarmInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_362_DVVAL VIDITEM_362_DVVAL_MaintState;

        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_370_DVVAL VIDITEM_370_DVVAL_CarrierZoneName;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_370_SV VIDITEM_370_SV_CarrierZoneName;

        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_601_SV VIDITEM_601_SV_PortTypeInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_602_DVVAL VIDITEM_602_DVVAL_PortUnitType;

        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_720_SV VIDITEM_720_SV_TransferInfo;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_722_DVVAL VIDITEM_722_DVVAL_TransferState;

        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_886_SV VIDITEM_886_SV_CarrierLocations;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_888_SV VIDITEM_888_SV_DisabledLocations;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_889_SV VIDITEM_889_SV_DisabledLoc;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_890_DVVAL VIDITEM_890_DVVAL_RequestCount;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_891_DVVAL VIDITEM_891_DVVAL_CraneCurrentPosition;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_892_DVVAL VIDITEM_892_DVVAL_CraneTotalDistance;
        public com.mirle.ibg3k0.sc.Data.SECS.ASE.S6F11.RPTINFO.RPTITEM.VIDITEM_893_SV VIDITEM_893_SV_MonitoredCraneInfo;

    }
}
