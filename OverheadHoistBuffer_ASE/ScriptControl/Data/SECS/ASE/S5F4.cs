// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S5F4.cs" company="">
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
    /// Class S5F4.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S5F4 : SXFY
    {
        [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 1)]
        public string ACKC5;

        public S5F4()
        {
            StreamFunction = "S5F4";
            W_Bit = 0;
        }
    }
}
