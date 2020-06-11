using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.DAO;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.Data.VO.Interface;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using com.mirle.iibg3k0.ttc.Common;
using NLog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class EquipmentBLL
    {
        private SCApplication scApp = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public Cache cache { get; private set; }


        public EquipmentBLL()
        {

        }
        public void start(SCApplication app)
        {
            scApp = app;
            cache = new Cache(scApp.getEQObjCacheManager());

        }
        public void startMapAction()
        {

            List<AVEHICLE> lstVH = scApp.getEQObjCacheManager().getAllVehicle();
        }



        public class Cache
        {
            EQObjCacheManager eqObjCacheManager = null;
            public Cache(EQObjCacheManager eqObjCacheManager)
            {
                this.eqObjCacheManager = eqObjCacheManager;
            }

            public bool IsInMatainSpace(string adrID)
            {
                int eqpt_count = eqObjCacheManager.getAllEquipment().
                            Where(eq => eq is MaintainSpace && SCUtility.isMatche((eq as MaintainSpace).MTS_ADDRESS, adrID)).
                            Count();
                return eqpt_count != 0;
            }
            public bool IsInMatainLift(string adrID)
            {
                int eqpt_count = eqObjCacheManager.getAllEquipment().
                            Where(eq => eq is MaintainLift && SCUtility.isMatche((eq as MaintainLift).MTL_ADDRESS, adrID)).
                            Count();
                return eqpt_count != 0;
            }
            public (bool isIn, IMaintainDevice device) IsInMaintainDevice(string vhCurrentAdr)
            {
                var eqpt = eqObjCacheManager.getAllEquipment().
                                  Where(eq => eq is MaintainLift && (eq as MaintainLift).MTL_ADDRESS == vhCurrentAdr.Trim() ||
                                        eq is MaintainSpace && (eq as MaintainSpace).MTS_ADDRESS == vhCurrentAdr.Trim()).
                                  SingleOrDefault();
                return (eqpt != null, eqpt as IMaintainDevice);
            }

            public bool IsInMaintainDeviceRangeOfSection(BLL.SegmentBLL segmentBLL, string sectionID)
            {
                string sectiin_id = SCUtility.Trim(sectionID, true);
                var maintain_device = eqObjCacheManager.getAllEquipment().
                                                        Where(device => device is IMaintainDevice).
                                                        ToList();
                foreach (IMaintainDevice device in maintain_device)
                {
                    ASEGMENT device_segment = segmentBLL.cache.GetSegment(device.DeviceSegment);
                    string[] segment_of_section = device_segment.Sections.Select(section => section.SEC_ID.Trim()).ToArray();
                    if (segment_of_section.Contains(sectiin_id))
                    {
                        return true;
                    }
                }
                return false;
            }
            public bool IsInMaintainDeviceRangeOfAddress(BLL.SegmentBLL segmentBLL, string adrID)
            {
                string adr_id = SCUtility.Trim(adrID, true);
                var maintain_device = eqObjCacheManager.getAllEquipment().
                                                        Where(device => device is IMaintainDevice).
                                                        ToList();
                foreach (IMaintainDevice device in maintain_device)
                {
                    ASEGMENT device_segment = segmentBLL.cache.GetSegment(device.DeviceSegment);
                    string[] segment_of_section_from_address = device_segment.Sections.Select(section => section.FROM_ADR_ID.Trim()).ToArray();
                    string[] segment_of_section_to_address = device_segment.Sections.Select(section => section.TO_ADR_ID.Trim()).ToArray();
                    string[] segment_include_address = segment_of_section_from_address.Concat(segment_of_section_to_address).ToArray();
                    if (segment_include_address.Contains(adr_id))
                    {
                        return true;
                    }
                }
                return false;
            }

            public IMaintainDevice getMaintainDevice(string eqID)
            {
                var eqpt = eqObjCacheManager.getAllEquipment().
                            Where(eq => SCUtility.isMatche(eq.EQPT_ID, eqID)).
                            Single();
                return eqpt as IMaintainDevice;
            }
            public List<AEQPT> loadMaintainDevice()
            {
                var eqpts = eqObjCacheManager.getAllEquipment().
                            Where(eq => eq is MaintainLift || eq is MaintainSpace).
                            ToList();
                return eqpts;
            }
            public List<AEQPT> loadMaintainLift()
            {
                var eqpts = eqObjCacheManager.getAllEquipment().
                            Where(eq => eq is MaintainLift).
                            ToList();
                return eqpts;
            }
            public List<AEQPT> loadMaintainSpace()
            {
                var eqpts = eqObjCacheManager.getAllEquipment().
                            Where(eq => eq is MaintainSpace).
                            ToList();
                return eqpts;
            }

            public MaintainLift GetMaintainLiftBySystemOutAdr(string systemOutAdr)
            {
                var eqpt = eqObjCacheManager.getAllEquipment().
                            Where(eq => eq is MaintainLift && (eq as MaintainLift).MTL_SYSTEM_OUT_ADDRESS == systemOutAdr.Trim()).
                            SingleOrDefault();
                return eqpt as MaintainLift;
            }

            public MaintainLift GetMaintainLift()
            {
                var eqpt = eqObjCacheManager.getAllEquipment().
                            Where(eq => eq is MaintainLift).
                            SingleOrDefault();
                return eqpt as MaintainLift;
            }
            public MaintainSpace GetMaintainSpace()
            {
                var eqpt = eqObjCacheManager.getAllEquipment().
                            Where(eq => eq is MaintainSpace &&
                                        SCUtility.isMatche(eq.EQPT_ID, "MTS")).
                            SingleOrDefault();
                return eqpt as MaintainSpace;
            }
            public MaintainLift GetMaintainLiftByMTLAdr(string mtlAdr)
            {
                var eqpt = eqObjCacheManager.getAllEquipment().
                            Where(eq => eq is MaintainLift && (eq as MaintainLift).MTL_ADDRESS == mtlAdr.Trim()).
                            SingleOrDefault();
                return eqpt as MaintainLift;
            }
            public MaintainLift GetMaintainLiftByMTLHomeAdr(string homeAdr)
            {
                var eqpt = eqObjCacheManager.getAllEquipment().
                            Where(eq => eq is MaintainLift && (eq as MaintainLift).MTL_CAR_IN_BUFFER_ADDRESS == homeAdr.Trim()).
                            SingleOrDefault();
                return eqpt as MaintainLift;
            }
            public MaintainSpace GetMaintainSpaceBySystemOutAdr(string systemOutAdr)
            {
                var eqpt = eqObjCacheManager.getAllEquipment().
                            Where(eq => eq is MaintainSpace && (eq as MaintainSpace).MTS_ADDRESS == systemOutAdr.Trim()).
                            SingleOrDefault();
                return eqpt as MaintainSpace;
            }
            public IMaintainDevice GetMaintainDeviceBySystemInAdr(string systemInAdr)
            {
                var eqpt = eqObjCacheManager.getAllEquipment().
                            Where(eq => eq is MaintainLift && (eq as MaintainLift).MTL_SYSTEM_IN_ADDRESS == systemInAdr.Trim() ||
                                        eq is MaintainSpace && (eq as MaintainSpace).MTS_SYSTEM_IN_ADDRESS == systemInAdr.Trim()).
                            SingleOrDefault();
                return eqpt as IMaintainDevice;
            }

            public MaintainLift GetExcuteCarOutMTL(string vhID)
            {
                var eqpt = eqObjCacheManager.getAllEquipment().
                                  Where(eq => eq is MaintainLift && (eq as MaintainLift).PreCarOutVhID == vhID.Trim()).
                                  SingleOrDefault();
                return eqpt as MaintainLift;
            }
            public MaintainSpace GetExcuteCarOutMTS(string vhID)
            {
                var eqpt = eqObjCacheManager.getAllEquipment().
                                  Where(eq => eq is MaintainSpace && (eq as MaintainSpace).PreCarOutVhID == vhID.Trim()).
                                  SingleOrDefault();
                return eqpt as MaintainSpace;
            }



            public List<AEQPT> loadOHCVDevices()
            {
                var eqpts = eqObjCacheManager.getAllEquipment().
                            Where(eq => eq is OHCV).
                            ToList();
                return eqpts;
            }
            public List<OHCV> loadOHCVDevicesBySegmentLocation(string segmentID)
            {
                var eqpts = eqObjCacheManager.getAllEquipment().
                            Where(eq => (eq is OHCV) && SCUtility.isMatche((eq as OHCV).SegmentLocation, segmentID)).
                            Select(eq => eq as OHCV).
                            ToList();
                return eqpts;
            }
            public OHCV getOHCV(string eqID)
            {
                var eqpt = eqObjCacheManager.getAllEquipment().
                            Where(eq => eq is OHCV && SCUtility.isMatche(eq.EQPT_ID, eqID)).
                            SingleOrDefault();
                return eqpt as OHCV;
            }
        }

    }
}
