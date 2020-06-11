// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S2F18.cs" company="">
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
    /// Date and Time Data (Actual time data.)
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S2F18 : SXFY
    {
        /// <summary>
        /// The time
        /// </summary>
        [SecsElement(Index = 1, ListSpreadOut = true ,Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 14)]
        public string TIME;     //yyyyMMddHHmmss

        /// <summary>
        /// Initializes a new instance of the <see cref="S2F18"/> class.
        /// </summary>
        public S2F18() 
        {
            StreamFunction = "S2F18";
            W_Bit = 0;
        }
    }
}
