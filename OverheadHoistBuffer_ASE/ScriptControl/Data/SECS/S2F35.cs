// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S2F31.cs" company="">
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
    /// Date and Time Set Request (DTS)
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S2F35 : SXFY
    {
        /// <summary>
        /// The time
        /// </summary>
        [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER, Length = 1)]
        public string DATAID;

        [SecsElement(Index = 2)]
        public RPTITEM[] RPTITEMS;


        /// <summary>
        /// Initializes a new instance of the <see cref="S2F35"/> class.
        /// </summary>
        public S2F35()
        {
            StreamFunction = "S2F35";
            W_Bit = 1;
        }

        public class RPTITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
            public string CEID;
            [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, ListElementLength = 1)]
            public string[] RPTIDS;

        }

    }
}
