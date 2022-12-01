using Mirle.Hlts.Utils;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace com.mirle.ibg3k0.sc.Common.Interface
{
    public interface IReserveModule
    {
        void Start(App.SCApplication scApp);
        BitmapSource GetCurrentReserveInfoMap();
        List<com.mirle.ibg3k0.sc.Data.VO.ReservedSection> GetCurrentReserveSections(string vhID);
        void RemoveAllReservedSections();
        void RemoveAllReservedSectionsByVehicleID(string vhID);
        void RemoveManyReservedSectionsByVIDSID(string vhID, string sectionID);
        void RemoveVehicle(string vhID);
        HltResult TryAddOrUpdateVehicle(AVEHICLE vh);
        HltResult TryAddReservedSection(string vhID, string sectionID, HltDirection sensorDir, HltDirection forkDir, bool isAsk = false);
    }
}