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
    public class S1F13 : SXFY
    {
        /// <summary>
        /// Equipment Model Type
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string MDLN;
        /// <summary>
        /// Software revision code
        /// </summary>
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string SOFTREV;

        public S1F13()
        {
            StreamFunction = "S1F13";
            W_Bit = 1;
            IsBaseType = true;
            ValidateFormat = false;         //Host 傳送給 EQPT時，允許List是空的(可以沒有MDLN、SOFTREV)
        }

    }

    /// <summary>
    /// On Line Data : Data signifying that the equipment is alive.
    /// 參照AIM的Spec發現，如果是發給EQ的話，S1F2的內容要是空的才行，所以新增一個S1F2的Format 2014/10/06 Steven
    /// </summary>
    public class S1F13_Empty : SXFY
    {

        [SecsElement(Index = 1, ListSpreadOut = true)]
        public string[] EMPTY;

        public S1F13_Empty()
        {
            StreamFunction = "S1F13";
            W_Bit = 1;
            IsBaseType = true;
            EMPTY = new string[0];
        }

    }
}
