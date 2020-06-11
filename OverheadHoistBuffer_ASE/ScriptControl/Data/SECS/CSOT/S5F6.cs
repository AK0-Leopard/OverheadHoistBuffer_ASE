// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S5F6.cs" company="">
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
    /// Equipment Constant Name list Reply
    /// </summary>
    public class S5F6 : SXFY
    {
        [SecsElement(Index = 1, ListSpreadOut = true)]
        public ALID_1[] ALIDS;

        public S5F6()
        {
            StreamFunction = "S5F6";
            W_Bit = 0;
        }

        public class ALID_1 : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 1)]
            public string ALCD;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
            public string ALID;
            //[SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 80)]
            [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]//變更長度為40 Markchou 20190313
            public string ALTX;
        }
    }
}
