// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S2F37.cs" company="">
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
    /// Enable or Disable Event Report
    /// </summary>
    public class S2F37 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_BOOLEAN, Length = 1)]
        public int[] CEED;
        /// <summary>
        /// A zero-length list means all CEIDs.
        /// </summary>
        [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER,
            ListElementLength = 1)]
        public string[] CEIDS;

        public S2F37()
        {
            StreamFunction = "S2F37";
            W_Bit = 1;
        }
    }

}
