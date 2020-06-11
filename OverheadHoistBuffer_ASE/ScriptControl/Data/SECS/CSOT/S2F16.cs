// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S2F16.cs" company="">
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

namespace com.mirle.ibg3k0.sc.Data.SECS.CSOT
{
    /// <summary>
    /// Acknowledge or error. If EAC constants a non-zero error code,
    /// the equipment should be change any of the ECIDs specified in S2F15.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S2F16 : SXFY
    {
        /// <summary>
        /// The eac
        /// </summary>
        //[SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 1)]//變更Type為TYPE_BINARY MarkChou 20190313
        public string EAC;

        /// <summary>
        /// Initializes a new instance of the <see cref="S2F16"/> class.
        /// </summary>
        public S2F16() 
        {
            StreamFunction = "S2F16";
            W_Bit = 0;
        }
    }
}
