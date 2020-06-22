//**********************************************************************************
// Date          Author         Request No.    Tag         Description
// ------------- -------------  -------------  ------      -----------------------------
// 2020/06/22    Hsinyu Chang   N/A            A20.06.22   新增空箱 & 實箱清單
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc
{
    public partial class ZoneDef
    {
        //此zone有幾個shelf
        public int ZoneSize { get; set; }
        //zone內的空箱清單
        public List<string> EmptyBoxList { get; set; }
        //zone內的實箱清單，僅儲存box ID(2020/06/22目前看來不需要cassette ID)
        public List<string> SolidBoxList { get; set; }
        //zone內的等待回收box清單
        public List<string> WaitForRecycleBoxList { get; set; }
        public int BoxCount { get; set; }
    }
}
