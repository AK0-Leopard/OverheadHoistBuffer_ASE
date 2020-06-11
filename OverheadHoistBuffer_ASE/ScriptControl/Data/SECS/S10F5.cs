// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S10F5.cs" company="">
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
    /// Terminal Display, Multi-block (H -&gt; E)
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S10F5 : SXFY
    {
        /// <summary>
        /// The tid
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
        public string TID;
        /// <summary>
        /// The texts
        /// </summary>
        [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_ASCII,
            ListElementLength = 40)]
        public string[] TEXTS;

        /// <summary>
        /// Initializes a new instance of the <see cref="S10F5"/> class.
        /// </summary>
        public S10F5() 
        {
            StreamFunction = "S10F5";
            W_Bit = 1;
        }
    }
}
