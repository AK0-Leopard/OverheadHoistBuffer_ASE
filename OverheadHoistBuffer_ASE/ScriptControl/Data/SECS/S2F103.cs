// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S2F103.cs" company="">
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
    /// Class S2F103.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S2F103 : SXFY
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
        /// The glasitems
        /// </summary>
        [SecsElement(Index = 8)]
        public GLASITEM[] GLASITEMS;

        /// <summary>
        /// Initializes a new instance of the <see cref="S2F103"/> class.
        /// </summary>
        public S2F103() 
        {
            StreamFunction = "S2F103";
        }

        /// <summary>
        /// Class GLASITEM.
        /// </summary>
        /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
        public class GLASITEM : SXFY
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
            /// The slotno
            /// </summary>
            [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string SLOTNO;
            /// <summary>
            /// The glsid
            /// </summary>
            [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string GLSID;
            /// <summary>
            /// The ppid
            /// </summary>
            [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string PPID;
            /// <summary>
            /// The glstype
            /// </summary>
            [SecsElement(Index = 8, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GLSTYPE;
            /// <summary>
            /// The glsidtype
            /// </summary>
            [SecsElement(Index = 9, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GLSIDTYPE;
            /// <summary>
            /// The glsjudge
            /// </summary>
            [SecsElement(Index = 10, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GLSJUDGE;
            /// <summary>
            /// The glsgrade
            /// </summary>
            [SecsElement(Index = 11, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GLSGRADE;
            /// <summary>
            /// The workorder
            /// </summary>
            [SecsElement(Index = 12, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 30)]
            public string WORKORDER;
            /// <summary>
            /// The maker
            /// </summary>
            [SecsElement(Index = 13, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string MAKER;
            /// <summary>
            /// The GLSTHK
            /// </summary>
            [SecsElement(Index = 14, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
            public string GLSTHK;
            /// <summary>
            /// The glssize
            /// </summary>
            [SecsElement(Index = 15, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GLSSIZE;
            /// <summary>
            /// The smplflag
            /// </summary>
            [SecsElement(Index = 16, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string SMPLFLAG;
            /// <summary>
            /// The RWKCNT
            /// </summary>
            [SecsElement(Index = 17, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string RWKCNT;
            /// <summary>
            /// The dumusedcnt
            /// </summary>
            [SecsElement(Index = 18, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
            public string DUMUSEDCNT;
            /// <summary>
            /// The maskid
            /// </summary>
            [SecsElement(Index = 19, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 30)]
            public string MASKID;
            /// <summary>
            /// The proberid
            /// </summary>
            [SecsElement(Index = 20, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string PROBERID;
            /// <summary>
            /// The paneljudge
            /// </summary>
            [SecsElement(Index = 21, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 800)]
            public string PANELJUDGE;
            /// <summary>
            /// The arrayrepairtype
            /// </summary>
            [SecsElement(Index = 22, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 800)]
            public string ARRAYREPAIRTYPE;
            /// <summary>
            /// The lcvdrepairtype
            /// </summary>
            [SecsElement(Index = 23, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 800)]
            public string LCVDREPAIRTYPE;
            /// <summary>
            /// The expunitid
            /// </summary>
            [SecsElement(Index = 24, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string EXPUNITID;
            /// <summary>
            /// The exprcpid
            /// </summary>
            [SecsElement(Index = 25, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 30)]
            public string EXPRCPID;
        }

    }
}
