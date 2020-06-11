// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S2F23.cs" company="">
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
    /// Class S2F23.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S2F23 : SXFY
    {
        /// <summary>
        /// Trace ID
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
        public string TRID;
        /// <summary>
        /// Data Sample Period
        /// Where “hh” is hours, “mm” is minutes, “ss” is seconds.
        /// Time Format: “hhmmss”.
        /// </summary>
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 6)]
        public string DSPER;
        /// <summary>
        /// Total Samples to be made
        /// TOTSMP = -1 means infinite count.
        /// </summary>
        [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
        public string TOTSMP;
        /// <summary>
        /// Reporting Group Size
        /// (Ex)
        /// DSPER = 3 Seconds, REPGSZ = 1: Report S6F1 (1 group) every 3 seconds.
        /// DSPER = 3 Seconds, REPGSZ = 2: Report S6F1 (2 group) every 6 seconds.
        /// </summary>
        [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string REPGSZ;
        /// <summary>
        /// The svidlist
        /// </summary>
        [SecsElement(Index = 5, ListElementType = SecsElement.SecsElementType.TYPE_ASCII,
            ListElementLength = 5)]
        public string[] SVIDLIST;

        /// <summary>
        /// Initializes a new instance of the <see cref="S2F23"/> class.
        /// </summary>
        public S2F23() 
        {
            StreamFunction = "S2F23";
            W_Bit = 1;
        }
    }
}
