// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S5F103.cs" company="">
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
    /// Current Alarm Set List Request (H -&gt; E)
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S5F103 : SXFY
    {
        /// <summary>
        /// The units
        /// </summary>
        [SecsElement(Index = 1, ListSpreadOut = true)]
        public UNITITEM[] UNITS;

        /// <summary>
        /// Initializes a new instance of the <see cref="S5F103"/> class.
        /// </summary>
        public S5F103() 
        {
            StreamFunction = "S5F103";
            W_Bit = 1;
        }

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
        }
    }
}
