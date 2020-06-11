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

namespace com.mirle.ibg3k0.sc.Data.SECS
{
    /// <summary>
    /// Class S2F42.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S2F42 : SXFY
    {


        /// <summary>
        /// The RCMD
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
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string CPNAME;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 1)]
            public string[] CPACK;

        }


    }
}
