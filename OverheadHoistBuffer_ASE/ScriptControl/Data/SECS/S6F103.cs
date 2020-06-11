// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S6F103.cs" company="">
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
    /// Class S6F103.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S6F103 : SXFY
    {
        /// <summary>
        /// The ptid
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string PTID;
        /// <summary>
        /// The ptusetype
        /// </summary>
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
        public string PTUSETYPE;
        /// <summary>
        /// The pttype
        /// </summary>
        [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
        public string PTTYPE;
        /// <summary>
        /// The cstid
        /// </summary>
        [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string CSTID;
        /// <summary>
        /// The qty
        /// </summary>
        [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string QTY;
        /// <summary>
        /// The crateqty
        /// </summary>
        [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CRATEQTY;
        /// <summary>
        /// The slotsel
        /// </summary>
        [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string SLOTSEL;
        /// <summary>
        /// The slotmap
        /// </summary>
        [SecsElement(Index = 8, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string SLOTMAP;
        /// <summary>
        /// The cstendflag
        /// </summary>
        [SecsElement(Index = 9, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string CSTENDFLAG;
        /// <summary>
        /// The glsitems
        /// </summary>
        [SecsElement(Index = 10)]
        public GLSITEM[] GLSITEMS;

        /// <summary>
        /// Initializes a new instance of the <see cref="S6F103"/> class.
        /// </summary>
        public S6F103() 
        {
            StreamFunction = "S6F103";
            W_Bit = 1;
        }

        /// <summary>
        /// Class GLSITEM.
        /// </summary>
        /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
        public class GLSITEM : SXFY
        {
            /// <summary>
            /// The lotid
            /// </summary>
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string LOTID;
            /// <summary>
            /// The operid
            /// </summary>
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string OPERID;
            /// <summary>
            /// The prodid
            /// </summary>
            [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string PRODID;
            /// <summary>
            /// The lotjudge
            /// </summary>
            [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string LOTJUDGE;
            /// <summary>
            /// The GLSST
            /// </summary>
            [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GLSST;
            /// <summary>
            /// The slotno
            /// </summary>
            [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string SLOTNO;
            /// <summary>
            /// The glsid
            /// </summary>
            [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string GLSID;
            /// <summary>
            /// The ppid
            /// </summary>
            [SecsElement(Index = 8, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string PPID;
            /// <summary>
            /// The rglsid
            /// </summary>
            [SecsElement(Index = 9, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string RGLSID;
            /// <summary>
            /// The glstype
            /// </summary>
            [SecsElement(Index = 10, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GLSTYPE;
            /// <summary>
            /// The glsidtype
            /// </summary>
            [SecsElement(Index = 11, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GLSIDTYPE;
            /// <summary>
            /// The glsjudge
            /// </summary>
            [SecsElement(Index = 12, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GLSJUDGE;
            /// <summary>
            /// The glsgrade
            /// </summary>
            [SecsElement(Index = 13, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GLSGRADE;
            /// <summary>
            /// The pairslotno
            /// </summary>
            [SecsElement(Index = 14, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
            public string PAIRSLOTNO;
            /// <summary>
            /// The pairprodid
            /// </summary>
            [SecsElement(Index = 15, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string PAIRPRODID;
            /// <summary>
            /// The pairprodtype
            /// </summary>
            [SecsElement(Index = 16, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string PAIRPRODTYPE;
            /// <summary>
            /// The pairglsid
            /// </summary>
            [SecsElement(Index = 17, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string PAIRGLSID;
            /// <summary>
            /// The pairrglsid
            /// </summary>
            [SecsElement(Index = 18, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string PAIRRGLSID;
            /// <summary>
            /// The pairglsjudge
            /// </summary>
            [SecsElement(Index = 19, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string PAIRGLSJUDGE;
            /// <summary>
            /// The pairglsgrade
            /// </summary>
            [SecsElement(Index = 20, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string PAIRGLSGRADE;
            /// <summary>
            /// The workorder
            /// </summary>
            [SecsElement(Index = 21, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 30)]
            public string WORKORDER;
            /// <summary>
            /// The crateid
            /// </summary>
            [SecsElement(Index = 22, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string CRATEID;
            /// <summary>
            /// The maker
            /// </summary>
            [SecsElement(Index = 23, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string MAKER;
            /// <summary>
            /// The GLSTHK
            /// </summary>
            [SecsElement(Index = 24, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
            public string GLSTHK;
            /// <summary>
            /// The glssize
            /// </summary>
            [SecsElement(Index = 25, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GLSSIZE;
            /// <summary>
            /// The smplflag
            /// </summary>
            [SecsElement(Index = 26, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string SMPLFLAG;
            /// <summary>
            /// The RWKCNT
            /// </summary>
            [SecsElement(Index = 27, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string RWKCNT;
            /// <summary>
            /// The dumusedcnt
            /// </summary>
            [SecsElement(Index = 28, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
            public string DUMUSEDCNT;
            /// <summary>
            /// The maskid
            /// </summary>
            [SecsElement(Index = 29, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 30)]
            public string MASKID;
            /// <summary>
            /// The unititems
            /// </summary>
            [SecsElement(Index = 30)]
            public UNITITEM[] UNITITEMS;
            /// <summary>
            /// The proberid
            /// </summary>
            [SecsElement(Index = 31, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string PROBERID;
            /// <summary>
            /// The gcflag
            /// </summary>
            [SecsElement(Index = 32, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GCFLAG;
            /// <summary>
            /// The gcunit
            /// </summary>
            [SecsElement(Index = 33, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 6)]
            public string GCUNIT;
            /// <summary>
            /// The evasmplflag
            /// </summary>
            [SecsElement(Index = 34, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string EVASMPLFLAG;
            /// <summary>
            /// The paneljudge
            /// </summary>
            [SecsElement(Index = 35, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 800)]
            public string PANELJUDGE;
            /// <summary>
            /// The arrayrepairtype
            /// </summary>
            [SecsElement(Index = 36, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 800)]
            public string ARRAYREPAIRTYPE;
            /// <summary>
            /// The lcvdrepairtype
            /// </summary>
            [SecsElement(Index = 37, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 800)]
            public string LCVDREPAIRTYPE;

            /// <summary>
            /// Class UNITITEM.
            /// </summary>
            /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
            public class UNITITEM : SXFY 
            {
                /// <summary>
                /// The unitid
                /// </summary>
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                public string UNITID;
                /// <summary>
                /// The sunitids
                /// </summary>
                [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_ASCII,
                        ListElementLength = 20)]
                public string[] SUNITIDS;


                //[SecsElement(Index = 2)]
                //public SUBUNITITEM[] SUBUNITITEMS;

                //public class SUBUNITITEM : SXFY 
                //{
                //    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                //    public string SUNITID;
                //}
            }
        }
    }
}
