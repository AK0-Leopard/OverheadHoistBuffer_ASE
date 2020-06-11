using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.Data.VO.Interface;
using com.mirle.ibg3k0.sc.ObjectRelay;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc
{
    public partial class ASEGMENT
    {
        public enum DisableType
        {
            User,
            Safety,
            HID,
            System
        }

        private LinkedList<AVEHICLE> Vehicles = new LinkedList<AVEHICLE>();
        public event EventHandler<AVEHICLE> VehicleLeave;
        public event EventHandler<AVEHICLE> VehicleEntry;
        public static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public List<ASECTION> Sections { get; private set; }
        public event EventHandler ControlInitial;
        //要改成不會同一個Function重複註冊的模式
        //public event EventHandler ControlComplete;
        private EventHandler controlComplete;
        private object _controlCompleteLock = new object();
        public event EventHandler ControlComplete
        {
            add
            {
                lock (_controlCompleteLock)
                {
                    controlComplete -= value;
                    controlComplete += value;
                }
            }
            remove
            {
                lock (_controlCompleteLock)
                {
                    controlComplete -= value;
                }
            }
        }



        public long segment_prepare_control_SyncPoint = 0;



        public void SetSectionList(BLL.SectionBLL sectionBLL)
        {
            var sections_on_segment = sectionBLL.cache.loadSectionsBySegmentID(SEG_NUM);
            Sections = sections_on_segment;
        }

        public void NotifyControlInitial()
        {
            onControlInitial();
        }
        public void NotifyControlComplete()
        {
            onControlComplete();
        }

        public void Leave(AVEHICLE vh)
        {
            if (vh == null) return;
            lock (Vehicles)
            {
                if (Vehicles.Contains(vh))
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEGMENT), Device: "AGVC",
                       Data: $"vh:{vh.VEHICLE_ID} leave segment:{SEG_NUM}",
                       VehicleID: vh.VEHICLE_ID,
                       CarrierID: vh.CST_ID);

                    Vehicles.Remove(vh);
                    onLeave(vh);
                }
            }
        }

        //public void Entry(AVEHICLE vh, BLL.SectionBLL sectionBLL, bool is_need_recalculate)
        //{
        //    if (vh == null) return;
        //    lock (Vehicles)
        //    {
        //        if (!Vehicles.Contains(vh))
        //        {
        //            if (is_need_recalculate)
        //            {
        //                ASECTION current_section = sectionBLL.cache.GetSection(vh.CUR_SEC_ID);
        //                int current_sec_index = Sections.IndexOf(current_section);
        //                if (current_sec_index == -1) return;
        //                if (Vehicles.Count > 0)
        //                {
        //                    AVEHICLE next_vh = null;
        //                    for (int i = current_sec_index; i < Sections.Count; i++)
        //                    {
        //                        string next_sec_id = Sections[i].SEC_ID;
        //                        AVEHICLE vh_temp = Vehicles.Where(v => Common.SCUtility.isMatche(v.CUR_SEC_ID, next_sec_id)).
        //                                                    FirstOrDefault();
        //                        if (vh_temp != null)
        //                        {
        //                            if (i == current_sec_index)
        //                            {
        //                                if (vh_temp.ACC_SEC_DIST > vh.ACC_SEC_DIST)
        //                                {
        //                                    next_vh = vh_temp;
        //                                    break;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                next_vh = vh_temp;
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    if (next_vh == null)
        //                    {
        //                        Vehicles.AddFirst(vh);
        //                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEGMENT), Device: "AGVC",
        //                           Data: $"vh:{vh.VEHICLE_ID} entry segment:{SEG_NUM} ,is first",
        //                           VehicleID: vh.VEHICLE_ID,
        //                           CarrierID: vh.CST_ID);

        //                    }
        //                    else
        //                    {
        //                        var next_vh_node = Vehicles.Find(next_vh);
        //                        Vehicles.AddAfter(next_vh_node, vh);

        //                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEGMENT), Device: "AGVC",
        //                           Data: $"vh:{vh.VEHICLE_ID} entry segment:{SEG_NUM} ,after {next_vh.VEHICLE_ID}",
        //                           VehicleID: vh.VEHICLE_ID,
        //                           CarrierID: vh.CST_ID);
        //                    }
        //                }
        //                else
        //                {
        //                    Vehicles.AddLast(vh);
        //                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEGMENT), Device: "AGVC",
        //                       Data: $"vh:{vh.VEHICLE_ID} entry segment:{SEG_NUM} ,is first",
        //                       VehicleID: vh.VEHICLE_ID,
        //                       CarrierID: vh.CST_ID);
        //                }
        //            }
        //            else
        //            {
        //                Vehicles.AddLast(vh);
        //                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEGMENT), Device: "AGVC",
        //                   Data: $"vh:{vh.VEHICLE_ID} entry segment:{SEG_NUM} ,is last",
        //                   VehicleID: vh.VEHICLE_ID,
        //                   CarrierID: vh.CST_ID);
        //            }
        //            onEntry(vh);
        //        }
        //    }
        //}
        public void Entry(AVEHICLE vh, BLL.SectionBLL sectionBLL, bool is_need_recalculate)
        {
            if (vh == null) return;
            lock (Vehicles)
            {
                bool is_contains = Vehicles.Contains(vh);
                //if (!Vehicles.Contains(vh))
                //{
                if (is_need_recalculate)
                {
                    if (is_contains)
                        Vehicles.Remove(vh);
                    ASECTION current_section = sectionBLL.cache.GetSection(vh.CUR_SEC_ID);
                    int current_sec_index = Sections.IndexOf(current_section);
                    if (current_sec_index == -1) return;
                    if (Vehicles.Count > 0)
                    {
                        AVEHICLE next_vh = null;
                        for (int i = current_sec_index; i < Sections.Count; i++)
                        {
                            string next_sec_id = Sections[i].SEC_ID;
                            AVEHICLE vh_temp = Vehicles.Where(v => Common.SCUtility.isMatche(v.CUR_SEC_ID, next_sec_id)).
                                                        FirstOrDefault();
                            if (vh_temp != null)
                            {
                                if (i == current_sec_index)
                                {
                                    if (vh_temp.ACC_SEC_DIST > vh.ACC_SEC_DIST)
                                    {
                                        next_vh = vh_temp;
                                        break;
                                    }
                                }
                                else
                                {
                                    next_vh = vh_temp;
                                    break;
                                }
                            }
                        }
                        if (next_vh == null)
                        {
                            Vehicles.AddFirst(vh);
                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEGMENT), Device: "AGVC",
                               Data: $"vh:{vh.VEHICLE_ID} entry segment:{SEG_NUM} ,is first",
                               VehicleID: vh.VEHICLE_ID,
                               CarrierID: vh.CST_ID);

                        }
                        else
                        {
                            var next_vh_node = Vehicles.Find(next_vh);
                            Vehicles.AddAfter(next_vh_node, vh);

                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEGMENT), Device: "AGVC",
                               Data: $"vh:{vh.VEHICLE_ID} entry segment:{SEG_NUM} ,after {next_vh.VEHICLE_ID}",
                               VehicleID: vh.VEHICLE_ID,
                               CarrierID: vh.CST_ID);
                        }
                    }
                    else
                    {
                        Vehicles.AddLast(vh);
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEGMENT), Device: "AGVC",
                           Data: $"vh:{vh.VEHICLE_ID} entry segment:{SEG_NUM} ,is first",
                           VehicleID: vh.VEHICLE_ID,
                           CarrierID: vh.CST_ID);
                    }
                }
                else
                {
                    if (!is_contains)
                    {
                        Vehicles.AddLast(vh);
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEGMENT), Device: "AGVC",
                           Data: $"vh:{vh.VEHICLE_ID} entry segment:{SEG_NUM} ,is last",
                           VehicleID: vh.VEHICLE_ID,
                           CarrierID: vh.CST_ID);
                    }
                }
                if (!is_contains)
                    onEntry(vh);
                //}
            }
        }

        private void RefreshVehicleLocalByRelativePosition(AVEHICLE vh, BLL.SectionBLL sectionBLL)
        {
            ASECTION current_section = sectionBLL.cache.GetSection(vh.CUR_SEC_ID);
            int current_sec_index = Sections.IndexOf(current_section);
            if (current_sec_index == -1) return;
            if (Vehicles.Count > 0)
            {
                AVEHICLE next_vh = null;
                for (int i = current_sec_index; i < Sections.Count; i++)
                {
                    string next_sec_id = Sections[i].SEC_ID;
                    AVEHICLE vh_temp = Vehicles.Where(v => Common.SCUtility.isMatche(v.CUR_SEC_ID, next_sec_id)).
                                                FirstOrDefault();
                    if (vh_temp != null)
                    {
                        if (i == current_sec_index)
                        {
                            if (vh_temp.ACC_SEC_DIST > vh.ACC_SEC_DIST)
                            {
                                next_vh = vh_temp;
                                break;
                            }
                        }
                        else
                        {
                            next_vh = vh_temp;
                            break;
                        }
                    }
                }
                if (next_vh == null)
                {
                    Vehicles.AddFirst(vh);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEGMENT), Device: "AGVC",
                       Data: $"vh:{vh.VEHICLE_ID} entry segment:{SEG_NUM} ,is first",
                       VehicleID: vh.VEHICLE_ID,
                       CarrierID: vh.CST_ID);

                }
                else
                {
                    var next_vh_node = Vehicles.Find(next_vh);
                    Vehicles.AddAfter(next_vh_node, vh);

                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEGMENT), Device: "AGVC",
                       Data: $"vh:{vh.VEHICLE_ID} entry segment:{SEG_NUM} ,after {next_vh.VEHICLE_ID}",
                       VehicleID: vh.VEHICLE_ID,
                       CarrierID: vh.CST_ID);
                }
            }
            else
            {
                Vehicles.AddLast(vh);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEGMENT), Device: "AGVC",
                   Data: $"vh:{vh.VEHICLE_ID} entry segment:{SEG_NUM} ,is first",
                   VehicleID: vh.VEHICLE_ID,
                   CarrierID: vh.CST_ID);
            }
        }

        /// <summary>
        /// 將回傳是否為該Segment的第一台車
        /// </summary>
        /// <param name="vh"></param>
        /// <returns></returns>
        public (bool isFirst, AVEHICLE firstVh) IsFirst(AVEHICLE vh)
        {
            if (Vehicles.Count == 0)
            {
                return (true, null);
            }
            else
            {
                AVEHICLE first_vh = Vehicles.First();

                return (first_vh.Equals(vh), first_vh);
            }
        }

        private void onLeave(AVEHICLE vh)
        {
            VehicleLeave?.Invoke(this, vh);
        }
        private void onEntry(AVEHICLE vh)
        {
            VehicleEntry?.Invoke(this, vh);
        }
        private void onControlInitial()
        {
            ControlInitial?.Invoke(this, null);
        }
        private void onControlComplete()
        {
            controlComplete?.Invoke(this, null);
        }

        public void RefreshVhOrder(sc.BLL.VehicleBLL vehicleBLL, sc.BLL.SectionBLL sectionBLL)
        {
            lock (Vehicles)
            {
                Vehicles.Clear();
                List<AVEHICLE> vhs = vehicleBLL.cache.loadVhsBySegmentID(this.SEG_NUM);
                foreach (AVEHICLE vh in vhs)
                {
                    if (!Vehicles.Contains(vh))
                    {
                        ASECTION current_section = sectionBLL.cache.GetSection(vh.CUR_SEC_ID);
                        int current_sec_index = Sections.IndexOf(current_section);
                        if (current_sec_index == -1) return;
                        if (Vehicles.Count > 0)
                        {
                            AVEHICLE next_vh = null;
                            for (int i = current_sec_index; i < Sections.Count; i++)
                            {
                                string next_sec_id = Sections[i].SEC_ID;
                                AVEHICLE vh_temp = Vehicles.Where(v => Common.SCUtility.isMatche(v.CUR_SEC_ID, next_sec_id)).
                                                            FirstOrDefault();
                                if (vh_temp != null)
                                {
                                    if (i == current_sec_index)
                                    {
                                        if (vh_temp.ACC_SEC_DIST > vh.ACC_SEC_DIST)
                                        {
                                            next_vh = vh_temp;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        next_vh = vh_temp;
                                        break;
                                    }
                                }
                            }
                            if (next_vh == null)
                            {
                                Vehicles.AddFirst(vh);
                                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEGMENT), Device: "AGVC",
                                   Data: $"vh:{vh.VEHICLE_ID} entry segment:{SEG_NUM} ,is first",
                                   VehicleID: vh.VEHICLE_ID,
                                   CarrierID: vh.CST_ID);

                            }
                            else
                            {
                                var next_vh_node = Vehicles.Find(next_vh);
                                Vehicles.AddAfter(next_vh_node, vh);

                                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEGMENT), Device: "AGVC",
                                   Data: $"vh:{vh.VEHICLE_ID} entry segment:{SEG_NUM} ,after {next_vh.VEHICLE_ID}",
                                   VehicleID: vh.VEHICLE_ID,
                                   CarrierID: vh.CST_ID);
                            }
                        }
                        else
                        {
                            Vehicles.AddLast(vh);
                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEGMENT), Device: "AGVC",
                               Data: $"vh:{vh.VEHICLE_ID} entry segment:{SEG_NUM} ,is last",
                               VehicleID: vh.VEHICLE_ID,
                               CarrierID: vh.CST_ID);
                        }
                    }
                }
            }
        }
        public string[] GetVehicleOrderInSegment()
        {
            return Vehicles.Select(vh => vh.VEHICLE_ID).ToArray();
        }
        public AVEHICLE GetNextVehicle(AVEHICLE vh)
        {
            if (!Vehicles.Contains(vh)) return null;
            if (Vehicles.First() == vh) return null;

            var vh_linked_node = Vehicles.Find(vh);
            return vh_linked_node.Previous.Value;
        }

        public void put(ASEGMENT newSegmentObject)
        {
            this.STATUS = newSegmentObject.STATUS;
            this.SEG_TYPE = newSegmentObject.SEG_TYPE;
            this.SPECIAL_MARK = newSegmentObject.SPECIAL_MARK;
            this.RESERVE_FIELD = newSegmentObject.RESERVE_FIELD;
            this.NOTE = newSegmentObject.NOTE;
            this.DIR = newSegmentObject.DIR;
            this.PRE_DISABLE_FLAG = newSegmentObject.PRE_DISABLE_FLAG;
            this.PRE_DISABLE_TIME = newSegmentObject.PRE_DISABLE_TIME;
            this.DISABLE_TIME = newSegmentObject.DISABLE_TIME;
            this.DISABLE_FLAG_USER = newSegmentObject.DISABLE_FLAG_USER;
            this.DISABLE_FLAG_SAFETY = newSegmentObject.DISABLE_FLAG_SAFETY;
            this.DISABLE_FLAG_HID = newSegmentObject.DISABLE_FLAG_HID;
            this.DISABLE_FLAG_SYSTEM = newSegmentObject.DISABLE_FLAG_SYSTEM;
        }

    }


}
