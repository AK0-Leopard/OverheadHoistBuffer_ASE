﻿// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S1F12.cs" company="">
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
    /// Class S1F12.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S1F12 : SXFY
    {
        /// <summary>
        /// The svids
        /// </summary>
        [SecsElement(Index = 1, ListSpreadOut = true)]
        public SVITEM[] SVIDS;

        /// <summary>
        /// Initializes a new instance of the <see cref="S1F12"/> class.
        /// </summary>
        public S1F12() 
        {
            StreamFunction = "S1F12";
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
            /// The svname
            /// </summary>
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string SVNAME;
        }
    }
}
