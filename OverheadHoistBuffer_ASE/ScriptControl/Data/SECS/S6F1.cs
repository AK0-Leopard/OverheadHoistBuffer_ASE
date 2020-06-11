// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S6F1.cs" company="">
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
    /// Class S6F1.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S6F1 : SXFY
    {
        /// <summary>
        /// The trid
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
        public string TRID;
        /// <summary>
        /// The SMPLN
        /// </summary>
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
        public string SMPLN;
        /// <summary>
        /// The stime
        /// </summary>
        [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 14)]
        public string STIME;
        /// <summary>
        /// The svdata
        /// </summary>
        [SecsElement(Index = 4)]
        public SVITEM[] SVDATA;

        /// <summary>
        /// Initializes a new instance of the <see cref="S6F1"/> class.
        /// </summary>
        public S6F1() 
        {
            StreamFunction = "S6F1";
            W_Bit = 0;
        }

        /// <summary>
        /// Class SVITEM.
        /// </summary>
        /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
        public class SVITEM : SXFY
        {
            /// <summary>
            /// The svid
            /// </summary>
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
            public string SVID;
            /// <summary>
            /// The sv
            /// </summary>
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string SV;
        }
    }
}
