// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S1F1.cs" company="">
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
    /// Are You There Requeset
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S1F1 : SXFY
    {
        //Header Only.

        /// <summary>
        /// Initializes a new instance of the <see cref="S1F1"/> class.
        /// </summary>
        public S1F1() 
        {
            StreamFunction = "S1F1";
            W_Bit = 1;
        }
    }

    /// <summary>
    /// Class S1F1_ErrorFormat.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S1F1_ErrorFormat : SXFY
    {
        /// <summary>
        /// The value
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 10)]
        public string VAL;
        /// <summary>
        /// The va l2
        /// </summary>
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
        public string VAL2;

        /// <summary>
        /// Initializes a new instance of the <see cref="S1F1_ErrorFormat"/> class.
        /// </summary>
        public S1F1_ErrorFormat()
        {
            StreamFunction = "S1F1";
            W_Bit = 1;
        }
    }

}
