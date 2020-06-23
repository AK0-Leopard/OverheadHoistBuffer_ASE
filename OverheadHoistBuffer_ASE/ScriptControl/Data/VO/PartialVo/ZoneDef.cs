//**********************************************************************************
// Date          Author         Request No.    Tag         Description
// ------------- -------------  -------------  ------      -------------------------
// 2020/06/22    Hsinyu Chang   N/A            A20.06.22a  新增空箱 & 實箱清單
// 2020/06/22    Hsinyu Chang   N/A            A20.06.22b  Box清單改變時，自動重新計算box總數
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
        private List<string> emptyBoxList;
        private List<string> solidBoxList;

        //此zone有幾個shelf
        public int ZoneSize { get; set; }
        //zone內的空箱清單
        public List<string> EmptyBoxList
        {
            get { return emptyBoxList; }
            set
            {
                emptyBoxList = value;
                if (solidBoxList != null)
                {
                    BoxCount = emptyBoxList.Count() + solidBoxList.Count();
                }
                else
                {
                    BoxCount = emptyBoxList.Count();
                }
            }
        }
        //zone內的實箱清單，僅儲存box ID(2020/06/22目前看來不需要cassette ID)
        public List<string> SolidBoxList
        {
            get { return solidBoxList; }
            set
            {
                solidBoxList = value;
                if (emptyBoxList != null)
                {
                    BoxCount = emptyBoxList.Count() + solidBoxList.Count();
                }
                else
                {
                    BoxCount = solidBoxList.Count();
                }
            }
        }
        //zone內的等待回收box清單
        public List<string> WaitForRecycleBoxList { get; set; }
        //zone內的box總數
        public int BoxCount { get; private set; }
    }
}
