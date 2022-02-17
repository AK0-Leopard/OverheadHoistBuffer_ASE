using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Data.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.BLL.Interface
{
    public interface ISectionBLL
    {
        List<ASECTION> loadSections();
        ASECTION getSection(string secID);
        ASECTION getSectionByToAdr(string adrID);
        ASECTION getSectionByFromAdr(string adrID);
    }
}
