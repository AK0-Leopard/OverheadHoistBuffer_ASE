// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S7F25.cs" company="">
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
    /// Formatted Process Program Request (H -&gt; E)
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S7F25 : SXFY
    {
        /// <summary>
        /// The ppid
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
        public string PPID;
        /// <summary>
        /// The unitid
        /// </summary>
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string UNITID;
        /// <summary>
        /// The sunitid
        /// </summary>
        [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string SUNITID;
        /// <summary>
        /// The pptype
        /// </summary>
        [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string PPTYPE;

        /// <summary>
        /// Initializes a new instance of the <see cref="S7F25"/> class.
        /// </summary>
        public S7F25() 
        {
            StreamFunction = "S7F25";
            W_Bit = 1;
        }
    }
}
