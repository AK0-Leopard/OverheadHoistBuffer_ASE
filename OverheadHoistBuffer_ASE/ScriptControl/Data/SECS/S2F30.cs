// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S2F30.cs" company="">
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
    /// Equipment Constant Name list Reply
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S2F30 : SXFY
    {
        /// <summary>
        /// The ecitems
        /// </summary>
        [SecsElement(Index = 1, ListSpreadOut = true)]
        public ECITEM[] ECITEMS;

        /// <summary>
        /// Initializes a new instance of the <see cref="S2F30"/> class.
        /// </summary>
        public S2F30() 
        {
            StreamFunction = "S2F30";
            W_Bit = 0;
        }

        /// <summary>
        /// Class ECITEM.
        /// </summary>
        /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
        public class ECITEM : SXFY
        {
            /// <summary>
            /// The ecid
            /// </summary>
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
            public string ECID;
            /// <summary>
            /// The ecname
            /// </summary>
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string ECNAME;
            /// <summary>
            /// The ecmin
            /// </summary>
            [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 10)]
            public string ECMIN;
            /// <summary>
            /// The ecmax
            /// </summary>
            [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 10)]
            public string ECMAX;
            /// <summary>
            /// The ecv
            /// </summary>
            [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 10)]
            public string ECV;
        }
    }
}
