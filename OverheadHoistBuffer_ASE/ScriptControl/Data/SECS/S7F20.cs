// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S7F20.cs" company="">
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
    /// Current EPPD Data
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S7F20 : SXFY
    {
        /// <summary>
        /// Equipment ID or Unit ID or Sub-Unit ID
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string UNITID;
        /// <summary>
        /// The pptype
        /// </summary>
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string PPTYPE;
        /// <summary>
        /// The ppidlist
        /// </summary>
        [SecsElement(Index = 3, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, ListElementLength = 40)]
        public string[] PPIDLIST;

        /// <summary>
        /// Initializes a new instance of the <see cref="S7F20"/> class.
        /// </summary>
        public S7F20() 
        {
            StreamFunction = "S7F20";
            W_Bit = 0;
        }
    }
}
