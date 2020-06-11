using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;
using com.mirle.ibg3k0.stc.Data.SecsData;

namespace com.mirle.ibg3k0.sc.Data.SECS.ASE
{
    public class S2F49 : SXFY
    {
        /// <summary>
        /// Always"0"for this system. 
        /// </summary>
        [SecsElement(Index = 1, ListSpreadOut = true, ListElementType = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
        public string DATAID;
        /// <summary>
        /// Always the empty string for this system.
        /// </summary>
        [SecsElement(Index = 2, ListSpreadOut = true, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
        public string OBJSPEC;
        /// <summary>
        /// 
        /// </summary>
        [SecsElement(Index = 3, ListSpreadOut = true, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
        public string RCMD;
        /// <summary>
        /// 
        /// </summary>
        [SecsElement(Index = 4)]
        public REPITEM[] REPITEMS;
        public class REPITEM : SXFY
        {
            //補上REPITEM內容 Markchou 20190313
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string CPNAME;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_UNKNOWN, Length = 64)]
            public string CPVAL;
        }

        public S2F49()
        {
            StreamFunction = "S2F49";
            StreamFunctionName = "Enhanced Remote Command Extension";
            W_Bit = 1;
        }
    }

    public class S2F49_STAGE : SXFY
    {
        [SecsElement(Index = 1, ListSpreadOut = true, ListElementType = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
        public string DATAID;
        [SecsElement(Index = 2, ListSpreadOut = true, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
        public string OBJSPEC;
        [SecsElement(Index = 3, ListSpreadOut = true, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
        public string RCMD;
        [SecsElement(Index = 4)]
        public REPITEM REPITEMS;
        public class REPITEM : SXFY
        {
            [SecsElement(Index = 1)]
            public REPITEM_STAGEINOF STAGEINOF;

            [SecsElement(Index = 2)]
            public REPITEM_TRANSFERINFO TRANSFERINFO;

            public class REPITEM_STAGEINOF : SXFY
            {
                [SecsElement(Index = 1, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                public string CPNAME;
                [SecsElement(Index = 2)]
                public REPITEM_STAGEINOF_CPVALUE CPVALUE;
                public class REPITEM_STAGEINOF_CPVALUE : SXFY
                {
                    [SecsElement(Index = 1)]
                    public REPITEM_ASCII STAGEID_CP;
                    [SecsElement(Index = 2)]
                    public REPITEM_U2 PRIORITY_CP;
                    [SecsElement(Index = 3)]
                    public REPITEM_U2 REPLACE_CP;
                    [SecsElement(Index = 4)]
                    public REPITEM_U2 EXPECTEDDURATION_CP;
                    [SecsElement(Index = 5)]
                    public REPITEM_U2 NOBLOCKINGTIME_CP;
                    [SecsElement(Index = 6)]
                    public REPITEM_U2 WAITTIMEOUT_CP;
                }
            }
            public class REPITEM_TRANSFERINFO : SXFY
            {
                [SecsElement(Index = 1, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                public string CPNAME;
                [SecsElement(Index = 2)]
                public REPITEM_TRANSFERINFO_CPVALUE CPVALUE;
                public class REPITEM_TRANSFERINFO_CPVALUE : SXFY
                {
                    [SecsElement(Index = 1)]
                    public REPITEM_ASCII CARRIERID_CP;
                    [SecsElement(Index = 2)]
                    public REPITEM_ASCII SOURCEPORT_CP;
                    [SecsElement(Index = 3)]
                    public REPITEM_ASCII DESTPORT_CP;
                }
            }

            public class REPITEM_ASCII : SXFY
            {
                [SecsElement(Index = 1, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                public string CPNAME2;
                [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                public string CPVAL_ASCII;
            }
            public class REPITEM_U2 : SXFY
            {
                [SecsElement(Index = 1, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                public string CPNAME3;
                [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                public string CPVAL_U2;
            }
        }

        public S2F49_STAGE()
        {
            StreamFunction = "S2F49";
            StreamFunctionName = "Enhanced Remote Command Extension";
            W_Bit = 1;
        }


    }

    public class S2F49_TRANSFEREXT : SXFY
    {
        [SecsElement(Index = 1, ListSpreadOut = true, ListElementType = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
        public string DATAID;
        [SecsElement(Index = 2, ListSpreadOut = true, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
        public string OBJSPEC;
        [SecsElement(Index = 3, ListSpreadOut = true, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
        public string RCMD;
        [SecsElement(Index = 4)]
        public REPITEM REPITEMS;

        public S2F49_TRANSFEREXT()
        {
            StreamFunction = "S2F49";
            StreamFunctionName = "Enhanced Remote Command Extension";
            W_Bit = 1;
        }

        public class REPITEM : SXFY
        {
            [SecsElement(Index = 1)]
            public COMM COMMINFO;
            [SecsElement(Index = 2)]
            public TRAN TRANINFO;
            [SecsElement(Index = 3)]
            public CST CSTINFO;

            public class COMM : SXFY
            {
                [SecsElement(Index = 1, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                public string COMMANDINFO;
                [SecsElement(Index = 2)]
                public COMMA COMMAINFO;

                public class COMMA : SXFY
                {
                    [SecsElement(Index = 1)]
                    public COMMANDID COMMANDIDINFO;
                    [SecsElement(Index = 2)]
                    public CP PRIORITY;
                    //[SecsElement(Index = 3)]
                    //public CP REPLACE;

                    public class COMMANDID : SXFY
                    {
                        [SecsElement(Index = 1, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                        public string COMMANDID1;
                        [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)] //Modify Length 1 > 10 By Kevin
                        public string CommandID;
                    }

                    public class CP : SXFY
                    {
                        [SecsElement(Index = 1, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                        public string CPNAME;
                        [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
                        public string CPVAL;
                    }
                }
            }
            public class TRAN : SXFY
            {
                [SecsElement(Index = 1, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                public string TRANSFERINFO;
                [SecsElement(Index = 2)]
                public CARR CARRINFO;

                public class CARR : SXFY
                {
                    [SecsElement(Index = 1)]
                    public CARRIERID CARRIERIDINFO;
                    [SecsElement(Index = 2)]
                    public BOXID BOXIDINFO;
                    [SecsElement(Index = 3)]
                    public SOU SOUINFO;
                    [SecsElement(Index = 4)]
                    public DEST DESTINFO;
                    //[SecsElement(Index = 4)]
                    //public CARRIER_ATTRIBUTE CARRIER_ATTRIBUTEINFO;

                    public class CARRIERID : SXFY
                    {
                        [SecsElement(Index = 1, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                        public string CARRIERID1;
                        [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)] //Modify Length 1 > 10 By Kevin
                        public string CarrierID;
                    }
                    public class BOXID : SXFY
                    {
                        [SecsElement(Index = 1, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                        public string BOXID1;
                        [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)] //Modify Length 1 > 10 By Kevin
                        public string BoxID;
                    }
                    public class SOU : SXFY
                    {
                        [SecsElement(Index = 1, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                        public string SOURCE;
                        [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]//Modify Length 1 > 40 By Kevin
                        public string Source;
                    }

                    public class DEST : SXFY
                    {
                        [SecsElement(Index = 1, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                        public string DEST1;
                        [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]//Modify Length 1 > 40 By Kevin
                        public string Dest;
                    }
                    //public class CARRIER_ATTRIBUTE : SXFY
                    //{
                    //    [SecsElement(Index = 1, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                    //    public string DEST1;
                    //    [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 64)]//Modify Length 1 > 40 By Kevin
                    //    public string Dest;
                    //}

                }
            }

            public class CST : SXFY
            {
                [SecsElement(Index = 1, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                public string TRANSFERINFO_CST;
                [SecsElement(Index = 2)]
                public CARR CARRINFO;

                public class CARR : SXFY
                {
                    [SecsElement(Index = 1)]
                    public LOT_ID LOTIDINFO;

                    public class LOT_ID : SXFY
                    {
                        [SecsElement(Index = 1, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                        public string LOT_ID1;
                        [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)] //Modify Length 1 > 10 By Kevin
                        public string Lot_ID;
                    }
                }
            }
            public class STAGEIDLSIT : SXFY
            {
                [SecsElement(Index = 1, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 12)]
                public string STAGE_ID_LIST_NAME;
                [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, ListElementLength = 10)]
                public string[] STAGE_ID_LIST;
            }

        }
    }
}
