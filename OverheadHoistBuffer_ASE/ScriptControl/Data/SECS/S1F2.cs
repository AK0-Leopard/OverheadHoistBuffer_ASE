// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S1F2.cs" company="">
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
    /// On Line Data : Data signifying that the equipment is alive.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S1F2 : SXFY
    {
        /// <summary>
        /// Equipment Model Type
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 6)]
        public string MDLN;
        /// <summary>
        /// Software revision code
        /// </summary>
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 6)]
        public string SOFTREV;

        /// <summary>
        /// Initializes a new instance of the <see cref="S1F2"/> class.
        /// </summary>
        public S1F2() 
        {
            StreamFunction = "S1F2";
            W_Bit = 0;
            IsBaseType = true;
            ValidateFormat = false;         //Host 傳送給 EQPT時，允許List是空的(可以沒有MDLN、SOFTREV)
        }

    }

    /// <summary>
    /// On Line Data : Data signifying that the equipment is alive.
    /// 參照AIM的Spec發現，如果是發給EQ的話，S1F2的內容要是空的才行，所以新增一個S1F2的Format 2014/10/06 Steven
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S1F2_Empty : SXFY    {

        /// <summary>
        /// The empty
        /// </summary>
        [SecsElement(Index = 1, ListSpreadOut = true)]
        public string[] EMPTY;

        /// <summary>
        /// Initializes a new instance of the <see cref="S1F2_Empty"/> class.
        /// </summary>
        public S1F2_Empty()
        {
            StreamFunction = "S1F2";
            W_Bit = 0;
            IsBaseType = true;
            EMPTY = new string[0];
        }

    }
}
