//*********************************************************************************
//      MESDefaultMapAction.cs
//*********************************************************************************
// File Name: MESDefaultMapAction.cs
// Description: 與EAP通訊的劇本
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;
using com.mirle.ibg3k0.stc.Data.SecsData;
using com.mirle.ibg3k0.sc.App;

namespace com.mirle.ibg3k0.sc.Data.SECS
{
    /// <summary>
    /// Class S6F3.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S6F3 : SXFY
    {
        /// <summary>
        /// The ceid
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;

        /// <summary>
        /// Initializes a new instance of the <see cref="S6F3"/> class.
        /// </summary>
        public S6F3()
        {
            StreamFunction = "S6F3";
            W_Bit = 0;
            IsBaseType = true;
        }
    }

    /// <summary>
    /// Class S6F3_CEID500_501.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S6F3_CEID500_501 : SXFY
    {
        /// <summary>
        /// The ceid
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        /// <summary>
        /// The data
        /// </summary>
        [SecsElement(Index = 2)]
        public PROCDATA DATA;

        /// <summary>
        /// Initializes a new instance of the <see cref="S6F3_CEID500_501"/> class.
        /// </summary>
        public S6F3_CEID500_501()
        {
            StreamFunction = "S6F3";
            W_Bit = 0;
        }

        /// <summary>
        /// Class PROCDATA.
        /// </summary>
        /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
        public class PROCDATA : SXFY
        {
            /// <summary>
            /// 如果是Lot Process Data，則留空白
            /// </summary>
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string UNITID;
            /// <summary>
            /// 如果是Lot Process Data，則留空白
            /// </summary>
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string SUNITID;
            /// <summary>
            /// The lotid
            /// </summary>
            [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string LOTID;
            /// <summary>
            /// The cstid
            /// </summary>
            [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string CSTID;
            /// <summary>
            /// 如果是Lot Process Data，則留空白
            /// </summary>
            [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string GLSID;
            /// <summary>
            /// The operid
            /// </summary>
            [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string OPERID;
            /// <summary>
            /// The prodid
            /// </summary>
            [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string PRODID;
            /// <summary>
            /// The ppid
            /// </summary>
            [SecsElement(Index = 8, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string PPID;
            /// <summary>
            /// The dvdatas
            /// </summary>
            [SecsElement(Index = 9)]
            public DVDATA[] DVDATAS;
        }

        /// <summary>
        /// Class DVDATA.
        /// </summary>
        /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
        public class DVDATA : SXFY
        {
            /// <summary>
            /// The dvname
            /// </summary>
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string DVNAME;
            /// <summary>
            /// The subitems
            /// </summary>
            [SecsElement(Index = 2)]
            public SUBITEM[] SUBITEMS;
        }

        /// <summary>
        /// Class SUBITEM.
        /// </summary>
        /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
        public class SUBITEM : SXFY
        {
            /// <summary>
            /// The sitename
            /// </summary>
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string SITENAME;
            /// <summary>
            /// The dv
            /// </summary>
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string DV;
        }

    }
}
