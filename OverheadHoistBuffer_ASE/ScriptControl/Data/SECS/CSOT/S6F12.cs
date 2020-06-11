// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S6F12.cs" company="">
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
    /// Class S6F12.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S6F12 : SXFY
    {
        /// <summary>
        /// The ack c6
        /// </summary>
        [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 1)]
        public string    ACKC6;



        /// <summary>
        /// Initializes a new instance of the <see cref="S6F12"/> class.
        /// </summary>
        public S6F12()
        {
            StreamFunction = "S6F12";
            StreamFunctionName = "Event report acknowledge";
            W_Bit = 0;
        }
    }
}
