// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S2F13.cs" company="">
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

namespace com.mirle.ibg3k0.sc.Data.SECS.ASE
{
    /// <summary>
    /// Class S2F13.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S2F13 : SXFY
    {
        /// <summary>
        /// The ecids
        /// </summary>
        [SecsElement(Index = 1, ListSpreadOut = true, ListElementType = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER,
            ListElementLength = 4)]
        public string[] ECIDS;

        /// <summary>
        /// Initializes a new instance of the <see cref="S2F13"/> class.
        /// </summary>
        public S2F13() 
        {
            StreamFunction = "S2F13";
            W_Bit = 1;
        }
    }
}
