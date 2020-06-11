// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S2F15.cs" company="">
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
    /// New Equipment Constants Send
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S2F15 : SXFY
    {
        /// <summary>
        /// The ecvitems
        /// </summary>
        [SecsElement(Index = 1, ListSpreadOut = true)]
        //[SecsElement(Index = 1, ListElementType = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER,
        //     ListElementLength = 1)]
        public ECVITEM[] ECVITEMS;

        /// <summary>
        /// Initializes a new instance of the <see cref="S2F15"/> class.
        /// </summary>
        public S2F15() 
        {
            StreamFunction = "S2F15";
            W_Bit = 1;
        }

        /// <summary>
        /// Class ECVITEM.
        /// </summary>
        /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
        public class ECVITEM : SXFY
        {
            /// <summary>
            /// The ecid
            /// </summary>
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 64)]
            public string ECID;
            /// <summary>
            /// The ecv
            /// </summary>
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
            public string ECV;
        }
    }
}
