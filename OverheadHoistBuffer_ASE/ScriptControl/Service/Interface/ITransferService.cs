using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Service.Interface
{
    public interface ITransferService
    {
        bool AreSourceEnable(string sourceName);
        bool isUnitType(string portName, UnitType unitType);
        bool isCVPort(string portName);
        bool checkAndProcessIsAgvPortToStation(ACMD_MCS cmdMCS);
        string Manual_DeleteCmd(string cmdid, string cmdSource);
        void MCSCommandFinishByShelfNotEnough(ACMD_MCS cmdMCS);
        string GetShelfRecentLocation(List<ShelfDef> shelfData, string portLoc);
        string GetShelfRecentLocationForFarthest(List<ShelfDef> shelfData, string portLoc);
        string GetAGV_OutModeInServicePortName(string agvZone);
        bool AreDestEnable(string destName, out bool isDestCvPortFull);
        List<PortINIData> GetAGVPort(string agvZoneName);
        PortPLCInfo GetPLC_PortData(string hOSTSOURCE);
        bool PortCommanding(string portID, bool Commanding);
        (bool isExist, string zoneName) tryGetShelfZoneName(string shelfID);
    }
}
