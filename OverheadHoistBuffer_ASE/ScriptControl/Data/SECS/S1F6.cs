// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S1F6.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
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
    /// Formatted Status Data
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S1F6 : SXFY
    {
        /// <summary>
        /// The SFCD
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string SFCD;

        /// <summary>
        /// Initializes a new instance of the <see cref="S1F6"/> class.
        /// </summary>
        public S1F6() 
        {
            StreamFunction = "S1F6";
            W_Bit = 0;
            IsBaseType = true;
        }
    }

    /// <summary>
    /// Equipment Status Request
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S1F6_SFCD1 : SXFY
    {
        /// <summary>
        /// The SFCD
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string SFCD = SECSConst.SFCD_Module_Status_Request;
        /// <summary>
        /// The rptinfo
        /// </summary>
        [SecsElement(Index = 2)]
        public RPTITEM RPTINFO;

        /// <summary>
        /// Initializes a new instance of the <see cref="S1F6_SFCD1"/> class.
        /// </summary>
        public S1F6_SFCD1() 
        {
            StreamFunction = "S1F6";
            W_Bit = 0;
        }

        /// <summary>
        /// Class RPTITEM.
        /// </summary>
        /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
        public class RPTITEM : SXFY
        {
            /// <summary>
            /// The CRST
            /// </summary>
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string CRST;
            /// <summary>
            /// The moduleinfo
            /// </summary>
            [SecsElement(Index = 2)]
            public MODULEITEM MODULEINFO;

            /// <summary>
            /// Class MODULEITEM.
            /// </summary>
            /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
            public class MODULEITEM : SXFY
            {
                /// <summary>
                /// The moduleid
                /// </summary>
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 8)]
                public string MODULEID;
                /// <summary>
                /// The modulest
                /// </summary>
                [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                public string MODULEST;
                /// <summary>
                /// The submodule
                /// </summary>
                [SecsElement(Index = 3)]
                public SUBMODULEITEM[] SUBMODULE;

                /// <summary>
                /// Class SUBMODULEITEM.
                /// </summary>
                /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
                public class SUBMODULEITEM : SXFY
                {
                    /// <summary>
                    /// The moduleid
                    /// </summary>
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 8)]
                    public string MODULEID;
                    /// <summary>
                    /// The modulest
                    /// </summary>
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string MODULEST;
                }
            }
        }
    }

    /// <summary>
    /// Port Status Request (Not Crate Port)
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S1F6_SFCD2 : SXFY
    {
        /// <summary>
        /// The SFCD
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string SFCD = SECSConst.SFCD_Port_Status_Request;
        /// <summary>
        /// The rptinfo
        /// </summary>
        [SecsElement(Index = 2)]
        public RPTITEM[] RPTINFO;

        /// <summary>
        /// Initializes a new instance of the <see cref="S1F6_SFCD2"/> class.
        /// </summary>
        public S1F6_SFCD2()
        {
            StreamFunction = "S1F6";
            W_Bit = 0;
        }

        /// <summary>
        /// If one cassette has more than two lots, ‘LOTID’, ‘PPID’ and ‘LOTST’ should be empty.
        /// </summary>
        /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
        public class RPTITEM : SXFY
        {
            /// <summary>
            /// The ptid
            /// </summary>
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string PTID;
            /// <summary>
            /// The pttype
            /// </summary>
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
            public string PTTYPE;
            /// <summary>
            /// The ptusetype
            /// </summary>
            [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string PTUSETYPE;
            /// <summary>
            /// The trsmode
            /// </summary>
            [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string TRSMODE;
            /// <summary>
            /// The PTST
            /// </summary>
            [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
            public string PTST;
            /// <summary>
            /// The cstid
            /// </summary>
            [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string CSTID;
            /// <summary>
            /// The lotid
            /// </summary>
            [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string LOTID;
            /// <summary>
            /// The ppid
            /// </summary>
            [SecsElement(Index = 8, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string PPID;
            /// <summary>
            /// The lotst
            /// </summary>
            [SecsElement(Index = 9, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string LOTST;
        }
    }

    /// <summary>
    /// Operation Status Request
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S1F6_SFCD3 : SXFY
    {
        /// <summary>
        /// The SFCD
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string SFCD = SECSConst.SFCD_Reticle_Status_Request;
        /// <summary>
        /// The rptinfo
        /// </summary>
        [SecsElement(Index = 2)]
        public RPTITEM RPTINFO;

        /// <summary>
        /// Initializes a new instance of the <see cref="S1F6_SFCD3"/> class.
        /// </summary>
        public S1F6_SFCD3()
        {
            StreamFunction = "S1F6";
            W_Bit = 0;
        }

        /// <summary>
        /// Class RPTITEM.
        /// </summary>
        /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
        public class RPTITEM : SXFY
        {
            /// <summary>
            /// The RTSN
            /// </summary>
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string RTSN;
            /// <summary>
            /// The rtid
            /// </summary>
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string RTID;
            /// <summary>
            /// The RTST
            /// </summary>
            [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string RTST;
            /// <summary>
            /// The rtusecnt
            /// </summary>
            [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
            public string RTUSECNT;
        }
    }

    

}
