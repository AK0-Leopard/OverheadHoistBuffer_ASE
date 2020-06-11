// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S7F23.cs" company="">
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
    /// Formatted Process Program Send (H -&gt; E)
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S7F23 : SXFY
    {
        /// <summary>
        /// The ppid
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
        public string PPID;
        /// <summary>
        /// The pptype
        /// </summary>
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string PPTYPE;
        /// <summary>
        /// The MDLN
        /// </summary>
        [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 6)]
        public string MDLN;
        /// <summary>
        /// The softrev
        /// </summary>
        [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 6)]
        public string SOFTREV;
        /// <summary>
        /// The lctime
        /// </summary>
        [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 14)]
        public string LCTIME;
        /// <summary>
        /// The codeitmes
        /// </summary>
        [SecsElement(Index = 6)]
        public CODEITEM[] CODEITMES;

        /// <summary>
        /// Initializes a new instance of the <see cref="S7F23"/> class.
        /// </summary>
        public S7F23() 
        {
            StreamFunction = "S7F23";
            W_Bit = 1;
        }

        /// <summary>
        /// Class CODEITEM.
        /// </summary>
        /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
        public class CODEITEM : SXFY 
        {
            /// <summary>
            /// The ccode
            /// </summary>
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string CCODE;
            /// <summary>
            /// The rcpstep
            /// </summary>
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string RCPSTEP;
            /// <summary>
            /// The unitid
            /// </summary>
            [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string UNITID;
            /// <summary>
            /// The sunitid
            /// </summary>
            [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string SUNITID;
            /// <summary>
            /// The parmitems
            /// </summary>
            [SecsElement(Index = 5)]
            public PARMITEM[] PARMITEMS;

            /// <summary>
            /// Class PARMITEM.
            /// </summary>
            /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
            public class PARMITEM : SXFY
            {
                /// <summary>
                /// The pparmname
                /// </summary>
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                public string PPARMNAME;
                /// <summary>
                /// The pparmvalue
                /// </summary>
                [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                public string PPARMVALUE;
            }
        }
    }
}
