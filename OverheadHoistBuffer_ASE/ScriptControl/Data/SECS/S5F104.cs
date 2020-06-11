// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S5F104.cs" company="">
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
    /// Class S5F104.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S5F104 : SXFY
    {
        /// <summary>
        /// The rptdata
        /// </summary>
        [SecsElement(Index = 1, ListSpreadOut = true)]
        public RPTITEM[] RPTDATA;

        /// <summary>
        /// Initializes a new instance of the <see cref="S5F104"/> class.
        /// </summary>
        public S5F104() 
        {
            StreamFunction = "S5F104";
            W_Bit = 0;
        }

        /// <summary>
        /// Class RPTITEM.
        /// </summary>
        /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
        public class RPTITEM : SXFY
        {
            /// <summary>
            /// The unitid
            /// </summary>
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string UNITID;
            /// <summary>
            /// The alarmids
            /// </summary>
            [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_ASCII,
                ListElementLength = 10)]
            public string[] ALARMIDS;
        }
    }
}
