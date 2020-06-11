// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S2F38.cs" company="">
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
    /// Class S2F38.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S2F38 : SXFY
    {
        /// <summary>
        /// 0：Accepted. 
        /// 1：Denied. At least one CEID does not exist. 
        /// >1：Other equipment-specific error.
        /// </summary>
        [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 1)]
        public string ERACK;
        /// <summary>
        /// Initializes a new instance of the <see cref="S2F38"/> class.
        /// </summary>
        public S2F38() 
        {
            StreamFunction = "S2F38";
            W_Bit = 0;
        }
    }
}
