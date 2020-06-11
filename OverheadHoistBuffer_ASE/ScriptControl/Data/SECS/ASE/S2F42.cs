// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S2F42.cs" company="">
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
    /// Class S2F42.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S2F42 : SXFY
    {
        /// <summary>
        /// 0：Acknowledge. Command has been performed. 
        /// 1：Command does not exist. 
        /// 2：Cannot perform now. 
        /// 3：At least one parameter is invalid. 
        /// 4：Acknowledge. Command will be performed with completion signaled later by an event.
        /// 5：Rejected. Already in desired condition.
        /// 6：No such object exists
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 1)]
        public string HCACK;
        /// <summary>
        /// The hcack
        /// </summary>
        [SecsElement(Index = 2)]
        public RPYITEM[] RPYITEMS;

        /// <summary>
        /// Initializes a new instance of the <see cref="S2F42"/> class.
        /// </summary>
        public S2F42()
        {
            StreamFunction = "S2F42";
            W_Bit = 0;
        }

        public class RPYITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string CPNAME;
            /// <summary>
            /// 1：Parameter name (CPNAME) does not exist
            /// 2：Illegal value specified for CPVAL. 
            /// 3：Illegal format specified for CPVAL. 
            /// >3：Other equipment-specific error
            /// </summary>
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 1)]
            public string CPACK;
        }
    }
}
