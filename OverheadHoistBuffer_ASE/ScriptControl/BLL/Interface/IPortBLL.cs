using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.BLL.Interface
{
    public interface IPortBLL
    {
        List<PortPLCInfo> loadPortPLCInfoByZoneCommnadID(string zoneCommnadID);
        bool isUnitType(string portName, UnitType unitType);
        PortPLCInfo getPortPLCInfo(string portName);

    }
}
