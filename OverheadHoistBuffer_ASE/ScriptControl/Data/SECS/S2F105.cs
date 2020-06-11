// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S2F105.cs" company="">
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
    /// Empty CST Permission (H -&gt; E)
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S2F105 : SXFY
    {
        /// <summary>
        /// Empty CST Permission
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string PTID;
        /// <summary>
        /// The cstid
        /// </summary>
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string CSTID;
        /// <summary>
        /// The emptycstpms
        /// </summary>
        [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string EMPTYCSTPMS;
        /// <summary>
        /// The hostmsg
        /// </summary>
        [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 80)]
        public string HOSTMSG;

        /// <summary>
        /// Initializes a new instance of the <see cref="S2F105"/> class.
        /// </summary>
        public S2F105() 
        {
            StreamFunction = "S2F105";
            W_Bit = 0;
        }
    }
}
