// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S1F11.cs" company="">
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
    /// Class S1F11.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S1F11 : SXFY
    {
        /// <summary>
        /// The svid
        /// </summary>
        [SecsElement(Index = 1, ListSpreadOut = true, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, 
            ListElementLength = 5)]
        public string[] SVID;

        /// <summary>
        /// Initializes a new instance of the <see cref="S1F11"/> class.
        /// </summary>
        public S1F11() 
        {
            StreamFunction = "S1F11";
            W_Bit = 1;
        }
    }
}
