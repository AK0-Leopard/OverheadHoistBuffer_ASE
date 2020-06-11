// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S2F32.cs" company="">
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
    /// Class S2F32.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S2F36 : SXFY
    {


        /// <summary>
        /// The tiack
        /// </summary>
        [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 1)]
        public string LRACK;

        /// <summary>
        /// Initializes a new instance of the <see cref="S2F36"/> class.
        /// </summary>
        public S2F36() 
        {
            StreamFunction = "S2F36";
            W_Bit = 0;
        }
    }
}
