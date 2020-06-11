// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S5F3.cs" company="">
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
    /// Enable/Disable Alarm Send(EAS) (H -&gt; E)
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S5F3 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 1)]
        public string ALED;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 4)]
        public string ALID;

        public S5F3()
        {
            StreamFunction = "S5F3";
            W_Bit = 1;
        }
    }
}
