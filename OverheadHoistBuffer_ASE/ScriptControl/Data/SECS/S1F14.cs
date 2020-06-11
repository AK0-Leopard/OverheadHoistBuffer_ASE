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
    public class S1F14 : SXFY
    {
        /// <summary>
        /// Equipment Model Type
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 1)]
        public string COMMACK;
        /// <summary>
        /// Software revision code
        /// </summary>
        [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_ASCII,
            ListElementLength = 3)]
        public string[] VERSION_INFO;

        public S1F14()
        {
            StreamFunction = "S1F14";
            W_Bit = 0;
            IsBaseType = true;
            ValidateFormat = false;         //Host 傳送給 EQPT時，允許List是空的(可以沒有MDLN、SOFTREV)
        }
    }

    /// <summary>
    /// On Line Data : Data signifying that the equipment is alive.
    /// </summary>
    public class S1F14_Empty : SXFY
    {
        /// <summary>
        /// Equipment Model Type
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 1)]
        public string COMMACK;
        /// <summary>
        /// Software revision code
        /// </summary>
        [SecsElement(Index = 2, ListSpreadOut = true)]
        public string[] EMPTY;

        public S1F14_Empty()
        {
            StreamFunction = "S1F14";
            W_Bit = 0;
            EMPTY = new string[0];
        }

    }
}
