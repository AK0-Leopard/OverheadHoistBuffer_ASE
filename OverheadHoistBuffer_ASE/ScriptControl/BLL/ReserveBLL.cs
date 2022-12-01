using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using Mirle.Hlts.Utils;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using com.mirle.ibg3k0.sc.BLL.Interface;
using com.mirle.ibg3k0.sc.Common.Interface;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class ReserveBLL : IReserveBLL
    {

        public event EventHandler<IReserveModule> ReserveMoudleChange;

        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private Mirle.Hlts.ReserveSection.Map.ViewModels.HltMapViewModel mapAPI { get; set; }

        private sc.Common.CommObjCacheManager commObjCacheManager { get; set; }
        private IReserveModule localReserveModule { get; set; }
        private IReserveModule remoteReserveModule { get; set; }
        private SCApplication scApp= null;

        private EventHandler reserveStatusChange;
        private object _reserveStatusChangeEventLock = new object();
        public event EventHandler ReserveStatusChange
        {
            add
            {
                lock (_reserveStatusChangeEventLock)
                {
                    reserveStatusChange -= value;
                    reserveStatusChange += value;
                }
            }
            remove
            {
                lock (_reserveStatusChangeEventLock)
                {
                    reserveStatusChange -= value;
                }
            }
        }

        private void onReserveStatusChange()
        {
            reserveStatusChange?.Invoke(this, EventArgs.Empty);
        }

        public ReserveBLL()
        {
        }
        public void start(SCApplication _app)
        {
            mapAPI = _app.getReserveSectionAPI();
            commObjCacheManager = _app.getCommObjCacheManager();
            localReserveModule = _app.localReserveModule;
            remoteReserveModule = _app.remoteReserveModule;
            scApp = _app;
        }
        private IReserveModule CurrentUsingReserveModule = null;
        private IReserveModule GetUsingReserveModule()
        {

            if (SystemParameter.IsUsingRemoteReserveModule)
            {
                if (scApp.getEQObjCacheManager().getLine().IsConnectionWithReserveModule)
                {
                    setCurrentUsingReserveModule(remoteReserveModule);
                    return remoteReserveModule;
                }
                else
                {
                    setCurrentUsingReserveModule(localReserveModule);
                    return localReserveModule;
                }
            }
            else
            {
                setCurrentUsingReserveModule(localReserveModule);
                return localReserveModule;
            }
        }
        object lock_obj_set_reserve_module = new object();
        private void setCurrentUsingReserveModule(IReserveModule reserveModule)
        {
            lock (lock_obj_set_reserve_module)
            {
                if (CurrentUsingReserveModule != reserveModule)
                {
                    CurrentUsingReserveModule = reserveModule;
                    OnReserveMoudleChange(reserveModule);
                }
            }
        }

        private void OnReserveMoudleChange(IReserveModule reserveModule)
        {
            ReserveMoudleChange?.Invoke(this, reserveModule);
        }

        public System.Windows.Media.Imaging.BitmapSource GetCurrentReserveInfoMap()
        {
            //DrawAllReserveSectionInfo();
            return GetUsingReserveModule().GetCurrentReserveInfoMap();
        }

        public (bool isExist, double x, double y, bool isTR50) GetHltMapAddress(string adrID)
        {
            try
            {
                var adr_obj = mapAPI.GetAddressObjectByID(adrID);
                if (adr_obj == null)
                    return (false, 0, 0, false);
                return (true, adr_obj.X, adr_obj.Y, adr_obj.IsTR50);
            }
            catch
            {
                return (false, 0, 0, false);
            }
        }
        public HltResult TryAddVehicleOrUpdate(string vhID, string currentSectionID, double vehicleX, double vehicleY, float vehicleAngle, double speedMmPerSecond,
                                           HltDirection sensorDir, HltDirection forkDir)
        {
            LogHelper.Log(logger: logger, LogLevel: NLog.LogLevel.Debug, Class: nameof(ReserveBLL), Device: "AGV",
               Data: $"({getReserveMoudleSymbol()})add vh in reserve system: vh:{vhID},x:{vehicleX},y:{vehicleY},angle:{vehicleAngle},speedMmPerSecond:{speedMmPerSecond},sensorDir:{sensorDir},forkDir:{forkDir}",
               VehicleID: vhID);
            var vh = new AVEHICLE();
            vh.VEHICLE_ID = vhID;
            vh.CUR_ADR_ID = "";
            vh.CUR_SEC_ID = currentSectionID;
            vh.CUR_SEG_ID = "";
            vh.X_Axis = vehicleX;
            vh.Y_Axis = vehicleY;
            vh.Speed = speedMmPerSecond;

            HltResult result = GetUsingReserveModule().TryAddOrUpdateVehicle(vh);
            onReserveStatusChange();
            return result;
        }

        public HltResult TryAddVehicleOrUpdate(AVEHICLE vh)
        {
            LogHelper.Log(logger: logger, LogLevel: NLog.LogLevel.Debug, Class: nameof(ReserveBLL), Device: "AGV",
               Data: $"({getReserveMoudleSymbol()})add vh in reserve system: vh:{vh.VEHICLE_ID},x:{vh.X_Axis},y:{vh.Y_Axis},angle:{0},speedMmPerSecond:{1},sensorDir:{HltDirection.None},forkDir:{HltDirection.None}",
               VehicleID: vh.VEHICLE_ID);
            HltResult result = GetUsingReserveModule().TryAddOrUpdateVehicle(vh);
            onReserveStatusChange();
            return result;
        }

        public void RemoveManyReservedSectionsByVIDSID(string vhID, string sectionID)
        {
            //int sec_id = 0;
            //int.TryParse(sectionID, out sec_id);
            string sec_id = SCUtility.Trim(sectionID);
            GetUsingReserveModule().RemoveManyReservedSectionsByVIDSID(vhID, sec_id);
            onReserveStatusChange();
        }

        public void RemoveVehicle(string vhID)
        {
            GetUsingReserveModule().RemoveVehicle(vhID);
        }

        public string GetCurrentReserveSection()
        {
            StringBuilder sb = new StringBuilder();
            var current_reserve_sections = GetUsingReserveModule().GetCurrentReserveSections("");
            sb.AppendLine($"({getReserveMoudleSymbol()})");
            sb.AppendLine("Current reserve section");
            foreach (var reserve_section in current_reserve_sections)
            {
                sb.AppendLine($"section id:{reserve_section.SectionID} vh id:{reserve_section.VehicleID}");
            }
            return sb.ToString();
        }
        public List<string> loadCurrentReserveSections(string vhID)
        {
            StringBuilder sb = new StringBuilder();
            var current_reserve_sections = GetUsingReserveModule().GetCurrentReserveSections(vhID);
            var sec_ids = current_reserve_sections.Select(reserved_sec => reserved_sec.SectionID).ToList();
            return sec_ids;
        }

        public HltResult TryAddReservedSection(string vhID, string sectionID, HltDirection sensorDir = HltDirection.Forward, HltDirection forkDir = HltDirection.None, bool isAsk = false)
        {
            HltResult result = null;
            string sec_id = SCUtility.Trim(sectionID);
            //如果詢問的Section是Reserve Enhance的section時，
            //則要判斷該區塊且之後的Section是否要得到
            var reserve_enhance_info_check_result = IsReserveEnhanceSection(sectionID);
            if (reserve_enhance_info_check_result.isEnhanceInfo)
            {
                List<string> enhance_control_sections = reserve_enhance_info_check_result.info.EnhanceControlSections;
                int section_index = enhance_control_sections.IndexOf(sectionID);
                for (int i = section_index; i < enhance_control_sections.Count; i++)
                {
                    result = GetUsingReserveModule().TryAddReservedSection(vhID, enhance_control_sections[i], sensorDir, forkDir, true);
                    if (!result.OK)
                    {
                        result.Description += $",section:{sectionID} is reserve enhance group:{reserve_enhance_info_check_result.info.GroupID}," +
                                              $"current has vh:{result.VehicleID}";
                        return result;
                    }
                }
            }
            result = GetUsingReserveModule().TryAddReservedSection(vhID, sec_id, sensorDir, forkDir, isAsk);
            result.Description += $"({getReserveMoudleSymbol()})";
            onReserveStatusChange();

            return result;
        }

        private (bool isEnhanceInfo, Data.VO.ReserveEnhanceInfo info) IsReserveEnhanceSection(string sectionID)
        {
            var ReserveEnhanceInfos = commObjCacheManager.getReserveEnhanceInfos();
            if (ReserveEnhanceInfos == null) return (false, null);
            var reserve_enhance_info = ReserveEnhanceInfos.
                              Where(info => info.EnhanceControlSections.Contains(sectionID)).
                              FirstOrDefault();
            return (reserve_enhance_info != null, reserve_enhance_info);
        }



        public void RemoveAllReservedSectionsByVehicleID(string vhID)
        {
            GetUsingReserveModule().RemoveAllReservedSectionsByVehicleID(vhID);
            onReserveStatusChange();
        }
        public void RemoveAllReservedSections()
        {
            GetUsingReserveModule().RemoveAllReservedSections();
            onReserveStatusChange();
        }



        enum HtlSectionType
        {
            Horizontal,
            Vertical,
            R2000
        }

        public enum ReserveMoudleSymbol
        {
            None,
            Local,
            Remote,
        }
        public ReserveMoudleSymbol getReserveMoudleSymbol()
        {
            if (GetUsingReserveModule() == localReserveModule)
            {
                return ReserveMoudleSymbol.Local;
            }
            else if (GetUsingReserveModule() == remoteReserveModule)
            {
                return ReserveMoudleSymbol.Remote;
            }
            else
            {
                return ReserveMoudleSymbol.None;
            }
        }
    }

}