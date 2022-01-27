using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.BLL.Interface
{
    public interface IShelfDefBLL
    {
        List<ShelfDef> GetEmptyAndEnableShelfByZone(string zoneID);
        int GetEmptyAndEnableShelfCountByZone(string hOSTDESTINATION);
    }
}
