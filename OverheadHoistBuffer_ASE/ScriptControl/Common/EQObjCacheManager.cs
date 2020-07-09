//*********************************************************************************
//      EQObjCacheManager.cs
//*********************************************************************************
// File Name: EQObjCacheManager.cs
// Description: Equipment Cache Manager
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.ConfigHandler;
using com.mirle.ibg3k0.bcf.Data.FlowRule;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;
using com.mirle.ibg3k0.sc.ConfigHandler;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;

namespace com.mirle.ibg3k0.sc.Common
{

    /// <summary>
    /// Class EQObjCacheManager.
    /// </summary>
    public class EQObjCacheManager
    {
        private List<VEHICLEMAP> VEHICLEMAPs = new List<VEHICLEMAP>()
        {
            new VEHICLEMAP(){  ID ="OHx01",REAL_ID="M101"},
            new VEHICLEMAP(){  ID ="OHx02",REAL_ID="M102"},
            new VEHICLEMAP(){  ID ="OHx03",REAL_ID="M103"},
            new VEHICLEMAP(){  ID ="OHx04",REAL_ID="M104"},
            new VEHICLEMAP(){  ID ="OHx05",REAL_ID="M105"},
            new VEHICLEMAP(){  ID ="OHx06",REAL_ID="M106"},
            new VEHICLEMAP(){  ID ="OHx07",REAL_ID="M107"},
            new VEHICLEMAP(){  ID ="OHx08",REAL_ID="M108"},
            new VEHICLEMAP(){  ID ="OHx09",REAL_ID="M109"},
            new VEHICLEMAP(){  ID ="OHx10",REAL_ID="M110"},
            new VEHICLEMAP(){  ID ="OHx11",REAL_ID="M111"},
            new VEHICLEMAP(){  ID ="OHx12",REAL_ID="M112"},
            new VEHICLEMAP(){  ID ="OHx13",REAL_ID="M113"},
            new VEHICLEMAP(){  ID ="OHx14",REAL_ID="M114"},
        };
        private List<MTSSetting> MTSSettings = new List<MTSSetting>()
        {
            new MTSSetting(){ID = "MTS",MTSSegment="013", MTSAddress ="20292", SystemInAddress ="20199" },
            new MTSSetting(){ID = "MTS2",MTSSegment="019", MTSAddress ="20296", SystemInAddress ="20037" }
        };

        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The instance
        /// </summary>
        private static EQObjCacheManager instance = null;
        /// <summary>
        /// The _lock
        /// </summary>
        private static Object _lock = new Object();
        /// <summary>
        /// The sc application
        /// </summary>
        private SCApplication scApp = null;

        //Cache Object
        /// <summary>
        /// The line
        /// </summary>
        private ALINE line = null;
        /// <summary>
        /// The _lock line
        /// </summary>
        private Object _lockLine = new Object();
        /// <summary>
        /// The zone list
        /// </summary>
        private List<AZONE> zoneList = new List<AZONE>();
        /// <summary>
        /// The _lock zone dic
        /// </summary>
        private Dictionary<string, Object> _lockZoneDic = new Dictionary<string, object>();
        /// <summary>
        /// The node list
        /// </summary>
        private List<ANODE> nodeList = new List<ANODE>();
        /// <summary>
        /// The _lock node dic
        /// </summary>
        private Dictionary<string, Object> _lockNodeDic = new Dictionary<string, object>();
        /// <summary>
        /// The eqpt list
        /// </summary>
        private List<AEQPT> eqptList = new List<AEQPT>();
        private List<AVEHICLE> vhList = new List<AVEHICLE>();
        /// <summary>
        /// The _lock eqpt dic
        /// </summary>
        private Dictionary<string, Object> _lockEqptDic = new Dictionary<string, object>();
        private Dictionary<string, Object> _lockVhDic = new Dictionary<string, object>();
        /// <summary>
        /// The unit list
        /// </summary>
        private List<AUNIT> unitList = new List<AUNIT>();
        /// <summary>
        /// The _lock unit dic
        /// </summary>
        private Dictionary<string, Object> _lockUnitDic = new Dictionary<string, object>();
        /// <summary>
        /// The port list
        /// </summary>
        private List<APORT> portList = new List<APORT>();
        private Dictionary<string, Object> _lockPorStationtDic = new Dictionary<string, object>();
        private List<APORTSTATION> portStationList = new List<APORTSTATION>();
        /// <summary>
        /// The _lock port dic
        /// </summary>
        private Dictionary<string, Object> _lockPortDic = new Dictionary<string, object>();
        /// <summary>
        /// The buff list
        /// </summary>
        private List<ABUFFER> buffList = new List<ABUFFER>();
        /// <summary>
        /// The _lock buff dic
        /// </summary>
        private Dictionary<string, Object> _lockBuffDic = new Dictionary<string, object>();
        /// <summary>
        /// The eqpt CSS
        /// </summary>
        private EQPTConfigSections eqptCss = null;

        //
        //private List<FlowRelation> flowRelationList = new List<FlowRelation>();
        /// <summary>
        /// The flow relation dic
        /// </summary>
        private Dictionary<string, FlowRelation> flowRelationDic = new Dictionary<string, FlowRelation>();
        /// <summary>
        /// The node flow relative CSS
        /// </summary>
        private NodeFlowRelConfigSections nodeFlowRelCss = null;

        /// <summary>
        /// The common information
        /// </summary>
        private CommonInfo commonInfo = new CommonInfo();                   //A0.08
        /// <summary>
        /// Gets the common information.
        /// </summary>
        /// <value>The common information.</value>
        public CommonInfo CommonInfo { get { return commonInfo; } }         //A0.08

        /// <summary>
        /// Prevents a default instance of the <see cref="EQObjCacheManager"/> class from being created.
        /// </summary>
        private EQObjCacheManager() { }
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns>EQObjCacheManager.</returns>
        public static EQObjCacheManager getInstance()
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = new EQObjCacheManager();
                }
                return instance;
            }
        }

        //public Boolean hasLineDataExist()
        //{
        //    scApp = SCApplication.getInstance();
        //    string line_id = eqptCss.ConfigSections[0].Line_ID.Trim();
        //    Line tmpLine = scApp.LineBLL.getLineByIDAndDeleteOtherLine(line_id);
        //    if (tmpLine != null)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        /// <summary>
        /// Sets the context.
        /// </summary>
        /// <param name="eqptCss">The eqpt CSS.</param>
        /// <param name="nodeFlowRelCss">The node flow relative CSS.</param>
        public void setContext(EQPTConfigSections eqptCss, NodeFlowRelConfigSections nodeFlowRelCss)
        {
            this.eqptCss = eqptCss;
            this.nodeFlowRelCss = nodeFlowRelCss;
        }

        /// <summary>
        /// Starts the specified recover from database.
        /// </summary>
        /// <param name="recoverFromDB">The recover from database.</param>
        public void start(/*EQPTConfigSections eqptCss, NodeFlowRelConfigSections nodeFlowRelCss,*/ Boolean recoverFromDB)
        {
            scApp = SCApplication.getInstance();
            //this.eqptCss = eqptCss;
            //this.nodeFlowRelCss = nodeFlowRelCss;
            //... TODO: start to build line, zone, node, eqpt, and unit...
            //buildEQObj();
            buildEQObjFromConfig();

            //            commonInfo.resetAlarmHis(scApp.AlarmBLL.loadAlarm(true, false));
            //            commonInfo.resetLotList(scApp.SheetBLL.loadInPorcLotList());

            insertToDB(recoverFromDB);
            buildFlowRelationFromConfig();
            //registerAlarmHandler();
            //registerLotHandler();


            List<APSetting> lstAPSetting = scApp.LineBLL.loadAPSettiong();
            commonInfo.dicCommunactionInfo = new Dictionary<string, CommuncationInfo>();
            foreach (APSetting ap in lstAPSetting)
            {
                commonInfo.dicCommunactionInfo.Add(
                    ap.AP_NAME
                    , new CommuncationInfo()
                    {
                        Name = ap.AP_NAME,
                        Getway_IP = ap.GETWAY_IP,
                        Remote_IP = ap.REMOTE_IP
                    });
            }

            //List<APORTSTATION> PORTSTATIONs = scApp.MapBLL.loadAllPort();
            //portStationList = PORTSTATIONs;

        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void stop()
        {
            clearCache();
            clearFlowRelCache();
        }

        /// <summary>
        /// Clears the flow relative cache.
        /// </summary>
        private void clearFlowRelCache()
        {
            //flowRelationList.Clear();
            flowRelationDic.Clear();
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        private void clearCache()
        {
            line = null;
            zoneList.Clear();
            _lockZoneDic.Clear();
            nodeList.Clear();
            _lockNodeDic.Clear();
            eqptList.Clear();
            _lockEqptDic.Clear();
            unitList.Clear();
            _lockUnitDic.Clear();
            portList.Clear();
            _lockPortDic.Clear();
            buffList.Clear();
            _lockBuffDic.Clear();
            portStationList.Clear();
            _lockPorStationtDic.Clear();
        }

        /// <summary>
        /// Builds the flow relation from configuration.
        /// </summary>
        private void buildFlowRelationFromConfig()
        {
            clearFlowRelCache();
            if (nodeFlowRelCss != null)
            {
                List<NodeFlowRelConfigSection> configs = nodeFlowRelCss.ConfigSections;
                foreach (NodeFlowRelConfigSection config in configs)
                {
                    string upstream_id = config.Upstream_ID.Trim();
                    string flow_rule_name = config.Flow_Rule.Trim();
                    Boolean isDonotCareFlowRule = config.isDonotCareFlowRule();

                    IFlowRule flowRule = null;
                    if (!isDonotCareFlowRule)
                    {
                        Type flowRuleType = Type.GetType(flow_rule_name);
                        flowRule =
                            (IFlowRule)Activator.CreateInstance(flowRuleType);
                    }

                    List<AFLOW_REL> items = new List<AFLOW_REL>();
                    foreach (Downstream ds in config.DownstreamList)
                    {
                        string downstream_id = ds.ID.Trim();
                        AFLOW_REL item = new AFLOW_REL()
                        {
                            UPSTREAM_ID = upstream_id,
                            DOWNSTREAM_ID = downstream_id,

                            FR_ID = upstream_id,
                            REL_TYPE = SCAppConstants.REL_TYPE_NODE
                        };
                        items.Add(item);
                    }
                    FlowRelation flowRel = new FlowRelation(upstream_id, flowRule, items, isDonotCareFlowRule);
                    //flowRelationList.Add(flowRel);
                    flowRelationDic.Add(upstream_id, flowRel);
                }
            }
        }

        /// <summary>
        /// Builds the eq object from configuration.
        /// </summary>
        private void buildEQObjFromConfig()
        {
            clearCache();
            //Line
            string line_id = eqptCss.ConfigSections[0].Line_ID.Trim();

            List<DeviceConnectionInfo> device_connection_satuses = new List<DeviceConnectionInfo>();
            var aps_seting = scApp.LineBLL.loadAPSettiong();
            foreach (var device_info in aps_seting)
            {
                device_connection_satuses.Add(new DeviceConnectionInfo()
                {
                    Name = device_info.AP_NAME,
                    Type = DeviceConnectionType.Ap
                });
            }
            var plc_devices = scApp.getBCFApplication().getMPLCSMControlDic();
            foreach (var device_info in plc_devices)
            {
                device_connection_satuses.Add(new DeviceConnectionInfo()
                {
                    Name = device_info.Key,
                    Type = DeviceConnectionType.Plc
                });
            }
            var secs_agent = scApp.getBCFApplication().getSECSAgent(scApp.EAPSecsAgentName);
            device_connection_satuses.Add(new DeviceConnectionInfo()
            {
                Name = scApp.EAPSecsAgentName,
                Type = DeviceConnectionType.Mcs
            });

            line = new ALINE()
            {
                LINE_ID = line_id,
                Real_ID = line_id,
                //                Host_Mode = 1,          //Hayes Test
                SECSAgentName = eqptCss.ConfigSections[0].SECSAgentName,        //A0.01
                TcpIpAgentName = eqptCss.ConfigSections[0].TcpIpAgentName,
                StopWatch_mcsConnectionTime = new System.Diagnostics.Stopwatch(),
                StopWatch_mcsDisconnectionTime = new System.Diagnostics.Stopwatch(),
                DeviceConnectionInfos = device_connection_satuses
            };
            //Zone
            List<ZoneConfigSection> zoneConfigs = eqptCss.ConfigSections[0].ZoneConfigList;
            foreach (ZoneConfigSection zoneConfig in zoneConfigs)
            {
                string zone_id = zoneConfig.Zone_ID.Trim();
                _lockZoneDic.Add(zone_id, new Object());
                zoneList.Add(new AZONE()
                {
                    ZONE_ID = zone_id,
                    //Real_ID = zone_id,
                    LINE_ID = line_id,
                    SECSAgentName = zoneConfig.SECSAgentName,            //A0.01
                    TcpIpAgentName = zoneConfig.TcpIpAgentName
                });
                //Node
                foreach (NodeConfigSection nodeConfig in zoneConfig.NodeConfigList)
                {
                    string node_id = nodeConfig.Node_ID.Trim();
                    int recipe_index = nodeConfig.Recipe_Index;
                    int node_num = nodeConfig.Node_Num;
                    _lockNodeDic.Add(node_id, new Object());
                    nodeList.Add(new ANODE()
                    {
                        NODE_ID = node_id,
                        Real_ID = node_id,
                        ZONE_ID = zone_id,
                        //Recipe_Index = recipe_index,
                        //Node_Num = node_num,
                        SECSAgentName = nodeConfig.SECSAgentName,            //A0.01
                        TcpIpAgentName = nodeConfig.TcpIpAgentName
                    });
                    //Eqpt
                    foreach (EQPTConfigSection eqptConfig in nodeConfig.EQPTConfigList)
                    {
                        string eqpt_id = eqptConfig.EQPT_ID.Trim();
                        _lockEqptDic.Add(eqpt_id, new Object());
                        int max_sht_cnt = eqptConfig.Max_Sht_Cnt;
                        int min_sht_cnt = eqptConfig.Min_Sht_Cnt;
                        string alarmListFile = eqptConfig.Alarm_List_File;
                        string procDataFormat = eqptConfig.Process_Data_Format;
                        string svDataFormat = eqptConfig.SV_Data_Format;                    //A0.02
                        string recipeParameterFormat = eqptConfig.Recipe_Parameter_Format;                    //A0.03
                        string ecidFormat = eqptConfig.ECID_Format;                    //A0.04
                        int Type = eqptConfig.EQPT_Type;

                        List<AUNIT> unitListOfEQPT = new List<AUNIT>();

                        if (unitListOfEQPT.Count > 0)
                        {
                            unitList.AddRange(unitListOfEQPT);
                        }
                        AEQPT eqTemp = null;
                        SCAppConstants.EqptType eqptType = (SCAppConstants.EqptType)Type;
                        if (eqptType == SCAppConstants.EqptType.MTL)
                        {
                            eqTemp = new MaintainLift();
                        }
                        else if (eqptType == SCAppConstants.EqptType.MTS)
                        {
                            eqTemp = new MaintainSpace();
                            MTSSetting mTSSetting = getMTSSetting(eqpt_id);
                            if (mTSSetting == null)
                            {
                                throw new Exception($"No setting mts:{eqpt_id} of addresses.");
                            }
                            (eqTemp as MaintainSpace).setMTSSegment(mTSSetting.MTSSegment);
                            (eqTemp as MaintainSpace).setMTSAddress(mTSSetting.MTSAddress);
                            (eqTemp as MaintainSpace).setMTSSystemInAddress(mTSSetting.SystemInAddress);
                        }
                        else if (eqptType == SCAppConstants.EqptType.OHCV)
                        {
                            eqTemp = new OHCV();

                            string segment_location = getOHCVLocation(eqpt_id);
                            if (SCUtility.isEmpty(segment_location))
                            {
                                throw new Exception($"No setting ohcv:{eqpt_id} of location.");
                            }
                            (eqTemp as OHCV).setSegmentLocation(segment_location);
                        }
                        else
                        {
                            eqTemp = new AEQPT();
                        }
                        eqTemp.EQPT_ID = eqpt_id;
                        eqTemp.CIM_MODE = "";
                        eqTemp.OPER_MODE = "";
                        eqTemp.INLINE_MODE = "";
                        eqTemp.EQPT_STAT = "";
                        eqTemp.EQPT_PROC_STAT = "";
                        eqTemp.Real_ID = eqpt_id;
                        eqTemp.NODE_ID = node_id;
                        eqTemp.MAX_SHT_CNT = max_sht_cnt;
                        eqTemp.MIN_SHT_CNT = min_sht_cnt;
                        eqTemp.SECSAgentName = eqptConfig.SECSAgentName;
                        eqTemp.TcpIpAgentName = eqptConfig.TcpIpAgentName;
                        eqTemp.UnitList = unitListOfEQPT;
                        eqTemp.Type = eqptType;

                        //eqTemp.startStateMachine();
                        eqptList.Add(eqTemp);
                        //AVEHICLE vhTemp = new AVEHICLE()
                        //{
                        //    VEHICLE_ID = eqpt_id,
                        //    TcpIpAgentName = eqptConfig.TcpIpAgentName,
                        //    CUR_ADR_ID = "",
                        //    CUR_SEC_ID = "",
                        //    NODE_ID = node_id
                        //};
                        //vhList.Add(vhTemp);
                        foreach (PortConfigSection portConfig in eqptConfig.PortConfigList)
                        {
                            string port_id = portConfig.Port_ID.Trim();
                            _lockPortDic.Add(port_id, new Object());
                            string port_type = portConfig.Port_Type.Trim();
                            int capacity = portConfig.Capacity;
                            portList.Add(new APORT()
                            {
                                PORT_ID = port_id,
                                Real_ID = port_id,
                                UNIT_NUM = portConfig.Unit_Num,
                                PORT_NUM = portConfig.Port_Num,
                                EQPT_ID = eqpt_id,
                                PORT_TYPE = port_type,
                                CAPACITY = capacity,
                                PORT_STAT = "",
                                PORT_ENABLE = "",
                                SECSAgentName = portConfig.SECSAgentName,         //A0.01
                                TcpIpAgentName = portConfig.TcpIpAgentName
                            });
                        }

                        foreach (BufferConfigSection buffConfig in eqptConfig.BuffConfigList)
                        {
                            string buff_id = buffConfig.Buff_ID.Trim();
                            _lockBuffDic.Add(buff_id, new Object());
                            int capacity = buffConfig.Capacity;
                            buffList.Add(new ABUFFER()
                            {
                                BUFF_ID = buff_id,
                                Real_ID = buff_id,
                                UNIT_NUM = buffConfig.Unit_Num,
                                EQPT_ID = eqpt_id,
                                CAPACITY = capacity,
                                SECSAgentName = buffConfig.SECSAgentName,         //A0.01
                                TcpIpAgentName = buffConfig.TcpIpAgentName
                            });
                        }
                        foreach (PortStationConfigSection portStationConfig in eqptConfig.PortStationConfigList)
                        {
                            string port_id = portStationConfig.Port_ID.Trim();
                            string adr_id = portStationConfig.Address_ID.Trim();
                            _lockPorStationtDic.Add(port_id, new Object());
                            E_VH_TYPE load_vh_type = (E_VH_TYPE)portStationConfig.Load_Vh_Type;
                            E_VH_TYPE unload_vh_type = (E_VH_TYPE)portStationConfig.Unload_Vh_Type;
                            portStationList.Add(new APORTSTATION()
                            {
                                EQPT_ID = eqpt_id,
                                PORT_ID = port_id,
                                ADR_ID = adr_id,
                                LD_VH_TYPE = load_vh_type,
                                ULD_VH_TYPE = unload_vh_type,
                                PORT_STATUS = E_PORT_STATUS.InService

                            });
                        }
                    }
                    int vh_nmu = 1;
                    foreach (VehicleConfigSection vehicleConfig in nodeConfig.VehilceConfigList)
                    {
                        string vh_id = vehicleConfig.Vh_ID;
                        AVEHICLE vhTemp = new AVEHICLE()
                        {
                            VEHICLE_ID = vh_id,
                            Num = vh_nmu++,
                            Real_ID = getVhRealID(vh_id),
                            TcpIpAgentName = vehicleConfig.TcpIpAgentName,
                            CUR_ADR_ID = "",
                            CUR_SEC_ID = "",
                            NODE_ID = node_id
                        };
                        vhList.Add(vhTemp);
                    }
                }
            }
        }


        private string getVhRealID(string vhID)
        {
            var map = VEHICLEMAPs.Where(id => id.ID.Trim() == vhID.Trim()).SingleOrDefault();
            string real_id = vhID;
            if (map != null)
            {
                real_id = map.REAL_ID;
            }
            return real_id;
        }
        private MTSSetting getMTSSetting(string mtsID)
        {
            var setting = MTSSettings.Where(id => id.ID.Trim() == mtsID.Trim()).SingleOrDefault();

            return setting;
        }
        private string getOHCVLocation(string ohcvID)
        {
            var setting = scApp.EqptLocationInfoDao.getEqptLocationInfo(scApp, ohcvID);
            if (setting == null)
            {
                return "";
            }
            return setting.SEGMENT_ID;
        }

        /// <summary>
        /// 如果要以MPLC目前資料為主，則需先清除DB內舊有資料，再重新rebuild，並透過腳本更新
        /// </summary>
        private void removeFromDB()
        {
            //not implement yet.
        }

        /// <summary>
        /// 當「reloadDataFromDB」為true時，會把DB現有資料放回cache中
        /// ...TODO: 還要把附掛上去的資料一起放回才完整
        /// </summary>
        /// <param name="recoverFromDB">The recover from database.</param>
        private void insertToDB(Boolean recoverFromDB)
        {
            try
            {
                using (DBConnection_EF con = new DBConnection_EF())
                {
                    recoverFromDB = false;  //A0.06  由於目前在RecoverFromDB此功能尚未完全，因此目前一律不使用 2015/04/13 by Kevin Wei

                    ALINE tmpLine = scApp.LineBLL.getLineByIDAndDeleteOtherLine(line.LINE_ID);
                    if (tmpLine == null)
                    {
                        if (!scApp.LineBLL.createLine(line))
                        {
                            logger.Error("insert Line[{0}] Failed.", line.LINE_ID);
                        }
                    }
                    else
                    {
                        if (recoverFromDB)
                        {
                            put(tmpLine);
                        }
                    }
                    foreach (AZONE zone in zoneList)
                    {
                        AZONE tmpZone = scApp.LineBLL.getZoneByZoneID(zone.ZONE_ID);
                        if (tmpZone == null)
                        {
                            if (!scApp.LineBLL.createZone(zone))
                            {
                                logger.Error("insert Zone[{0}] Failed.", zone.ZONE_ID);
                            }
                        }
                        else
                        {
                            if (recoverFromDB)
                            {
                                put(tmpZone);
                                //1. get "Lot" by Lot ID
                                if (!BCFUtility.isEmpty(tmpZone.LOT_ID))
                                {
                                    logger.Error("tmpZone.Lot_ID is Empty.");
                                }
                            }
                        }
                    }
                    foreach (ANODE node in nodeList)
                    {
                        ANODE tmpNode = scApp.LineBLL.getNodeByNodeID(node.NODE_ID);
                        if (tmpNode == null)
                        {
                            if (!scApp.LineBLL.createNode(node))
                            {
                                logger.Error("insert Node[{0}] Failed.", node.NODE_ID);
                            }
                        }
                        else
                        {
                            if (recoverFromDB)
                            {
                                put(tmpNode);
                            }
                        }
                    }
                    foreach (AEQPT eqpt in eqptList)
                    {
                        AEQPT tmpEQPT = scApp.LineBLL.getEqptByEqptID(eqpt.EQPT_ID);
                        if (tmpEQPT == null)
                        {
                            var _eqpt = new AEQPT()
                            {
                                EQPT_ID = eqpt.EQPT_ID,
                                NODE_ID = eqpt.NODE_ID,
                                CIM_MODE = eqpt.CIM_MODE,
                                OPER_MODE = eqpt.OPER_MODE,
                                INLINE_MODE = eqpt.INLINE_MODE,
                                EQPT_STAT = eqpt.EQPT_STAT,
                                EQPT_PROC_STAT = eqpt.EQPT_PROC_STAT,
                                CR_RECIPE = eqpt.CR_RECIPE,
                                MAX_SHT_CNT = eqpt.MAX_SHT_CNT,
                                MIN_SHT_CNT = eqpt.MIN_SHT_CNT,
                                ALARM_STAT = eqpt.ALARM_STAT,
                                WARN_STAT = eqpt.WARN_STAT
                            };
                            if (!scApp.LineBLL.createEquipment(_eqpt))
                            {
                                logger.Error("insert EQPT[{0}] Failed.", _eqpt.EQPT_ID);
                            }
                        }
                        else
                        {
                            if (recoverFromDB)
                            {
                                put(tmpEQPT);
                            }
                        }
                    }
                    //var db = scApp.RedisConnection.GetDatabase();
                    //var batch = db.CreateBatch();
                    //int vh_count = 0;
                    foreach (AVEHICLE vh in vhList)
                    {
                        restoreVhDataFromDB(vh);
                        AVIDINFO tmpVid = scApp.VIDBLL.getVIDInfo(vh.VEHICLE_ID);
                        if (tmpVid == null)
                        {
                            if (!scApp.VIDBLL.addVIDInfo(vh.VEHICLE_ID))
                            {
                                logger.Error("insert VID Info[{0}] Failed.", vh.VEHICLE_ID);
                            }
                        }
                        //var taskRedisValue = db.ListGetByIndexAsync(SCAppConstants.REDIS_LIST_KEY_VEHICLES, vh_count);
                        //if (taskRedisValue.Result.IsNull)
                        //{
                        //    byte[] Serialization_Vh = SCUtility.ToByteArray(vh);
                        //     db.ListSetByIndexAsync(SCAppConstants.REDIS_LIST_KEY_VEHICLES, vh_count, Serialization_Vh);
                        //}
                        //db.ListRightPushAsync(SCAppConstants.REDIS_LIST_KEY_VEHICLES, Serialization_Vh);
                        //vh_count++;
                    }
                    if (!scApp.getRedisCacheManager().KeyExists(SCAppConstants.REDIS_LIST_KEY_VEHICLES))
                    {
                        scApp.getRedisCacheManager().ListRightPush(SCAppConstants.REDIS_LIST_KEY_VEHICLES, vhList);
                    }
                    else
                    {
                        //目前將資料保存在Redis上面，因此不再從DB抓取資料更新至Redis進行初始化
                        //foreach (AVEHICLE vh in vhList)
                        //{
                        //    scApp.getRedisCacheManager().ListSetByIndex
                        //        (SCAppConstants.REDIS_LIST_KEY_VEHICLES, vh.VEHICLE_ID, vh.ToString());
                        //}
                    }
                    //batch.Execute();

                    foreach (AUNIT unit in unitList)
                    {
                        AUNIT tmpUnit = scApp.LineBLL.getUnitByUnitID(unit.UNIT_ID);
                        if (tmpUnit == null)
                        {
                            if (!scApp.LineBLL.createUnit(unit))
                            {
                                logger.Error("insert Unit[{0}] Failed.", unit.UNIT_ID);
                            }
                        }
                        else
                        {
                            if (recoverFromDB)
                            {
                                put(tmpUnit);
                            }
                        }
                    }
                    foreach (APORT port in portList)
                    {
                        APORT tmpPort = scApp.LineBLL.getPortByPortID(port.PORT_ID);
                        if (tmpPort == null)
                        {
                            if (!scApp.LineBLL.createPort(port))
                            {
                                logger.Error("insert Port[{0}] Failed.", port.PORT_ID);
                            }
                        }
                        else
                        {
                            if (recoverFromDB)
                            {
                                put(tmpPort);
                                //get CST by port id
                            }
                        }
                    }
                    foreach (ABUFFER buff in buffList)
                    {
                        ABUFFER tmpBuff = scApp.LineBLL.getBufferPortByPortID(buff.BUFF_ID);
                        if (tmpBuff == null)
                        {
                            if (!scApp.LineBLL.createBufferPort(buff))
                            {
                                logger.Error("insert Buffer[{0}] Failed.", buff.BUFF_ID);
                            }
                        }
                        else
                        {
                            if (recoverFromDB)
                            {
                                put(tmpBuff);
                            }
                        }
                    }
                    foreach (APORTSTATION portstation in portStationList)
                    {
                        APORTSTATION tmpPortStation = scApp.PortStationBLL.OperateDB.get(portstation.PORT_ID);
                        if (tmpPortStation == null)
                        {
                            if (!scApp.PortStationBLL.OperateDB.add(portstation))
                            {
                                logger.Error("insert port station[{0}] Failed.", portstation.PORT_ID);
                            }
                        }
                        else
                        {
                            //if (recoverFromDB)
                            {
                                put(tmpPortStation);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        public void restoreVhDataFromDB(AVEHICLE vh)
        {
            AVEHICLE tmpVh = scApp.VehicleBLL.getVehicleByIDFromDB(vh.VEHICLE_ID);
            if (tmpVh == null)
            {
                if (!scApp.VehicleBLL.addVehicle(vh))
                {
                    logger.Error("insert EQPT[{0}] Failed.", vh.VEHICLE_ID);
                }
            }
            else
            {
                // if (recoverFromDB)
                {
                    //put(tmpVh);
                    update(tmpVh);
                }

                //vh.VEHICLE_TYPE = tmpVh.VEHICLE_TYPE;
            }
        }

        /// <summary>
        /// Builds the eq object.
        /// </summary>
        private void buildEQObj()
        {
            clearCache();

            //Line
            line = scApp.LineBLL.getFirstLine();
            if (line == null)
            {
                logger.Warn("Not Found Line Setting.");
                return;
            }

            //Zone
            zoneList.AddRange(scApp.LineBLL.loadZoneListByLine(line.LINE_ID).ToList());

            //Node
            foreach (AZONE zone in zoneList)
            {
                _lockZoneDic.Add(zone.ZONE_ID, new Object());
                nodeList.AddRange(scApp.LineBLL.loadNodeListByZone(zone.ZONE_ID).ToList());
            }
            //EQPT
            foreach (ANODE node in nodeList)
            {
                _lockNodeDic.Add(node.NODE_ID, new Object());
                eqptList.AddRange(scApp.LineBLL.loadEqptListByNode(node.NODE_ID).ToList());
            }
            //Unit
            foreach (AEQPT eqpt in eqptList)
            {
                _lockEqptDic.Add(eqpt.EQPT_ID, new Object());
                unitList.AddRange(scApp.LineBLL.loadUnitListByEqpt(eqpt.EQPT_ID).ToList());
            }
            //VH
            foreach (AVEHICLE vh in vhList)
            {
                _lockVhDic.Add(vh.VEHICLE_ID, new Object());
            }


            foreach (AUNIT unit in unitList)
            {
                _lockUnitDic.Add(unit.UNIT_ID, new Object());
            }
        }
        //private void registerLotHandler()
        //{
        //    foreach (AZONE zone in zoneList)
        //    {
        //        zone.LotLoader.addEventHandler("", BCFUtility.getPropertyName(() => zone.LotLoader.LotItem),
        //        (s1, e1) => lotHandler(s1, e1));
        //    }
        //}

        /// <summary>
        /// Lots the handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void lotHandler(object sender, PropertyChangedEventArgs args)
        {
            // not implement yet
        }




        /// <summary>
        /// 取得此Node(Upstream)可以流向的Downstream List
        /// </summary>
        /// <param name="upstream_id">The upstream_id.</param>
        /// <returns>List&lt;Node&gt;.</returns>
        public List<ANODE> getDownstreamList(string upstream_id)
        {
            List<ANODE> downstreams = new List<ANODE>();
            if (!flowRelationDic.ContainsKey(upstream_id.Trim())
                || flowRelationDic[upstream_id.Trim()] == null)
            {
                return downstreams;
            }
            foreach (string downstreamID in flowRelationDic[upstream_id.Trim()].getDownstreamIDList())
            {
                downstreams.Add(getNodeByNodeID(downstreamID));
            }
            return downstreams;
        }

        /// <summary>
        /// 根據流向設定，以及指定的判斷規則進行選擇下一個流向的Node
        /// </summary>
        /// <param name="upstream_id">當前的Node ID</param>
        /// <returns>下一個流向的Node。如果沒有適合的Node，將會回傳null</returns>
        public ANODE getDownstream(string upstream_id)
        {
            if (!flowRelationDic.ContainsKey(upstream_id.Trim()))
            {
                return null;
            }
            ANODE upstream = getNodeByNodeID(upstream_id);
            List<BaseEQObject> downstreams = new List<BaseEQObject>();
            foreach (string downstreamID in flowRelationDic[upstream_id.Trim()].getDownstreamIDList())
            {
                downstreams.Add(getNodeByNodeID(downstreamID));
            }
            int selIndex = 0;
            if (!flowRelationDic[upstream_id.Trim()].IsDonotCareFlowRule)
            {
                selIndex = flowRelationDic[upstream_id.Trim()].Flow_Rule.doDecide(upstream, downstreams);
            }
            //            int selIndex = flowRelationDic[upstream_id.Trim()].Flow_Rule.doDecide(upstream, downstreams);
            if (selIndex >= 0 && downstreams.Count > selIndex)
            {
                return downstreams[selIndex] as ANODE;
            }
            return null;
        }

        #region 取得各種EQ Object的方法
        //
        /// <summary>
        /// Gets the line.
        /// </summary>
        /// <returns>Line.</returns>
        public ALINE getLine()
        {
            return line;
        }

        /// <summary>
        /// Gets the zone by zone identifier.
        /// </summary>
        /// <param name="zone_id">The zone_id.</param>
        /// <returns>Zone.</returns>
        public AZONE getZoneByZoneID(string zone_id)
        {
            return zoneList.Where(z => z.ZONE_ID.Trim() == zone_id.Trim()).FirstOrDefault();
        }

        /// <summary>
        /// Gets the zone list by line.
        /// </summary>
        /// <returns>List&lt;Zone&gt;.</returns>
        public List<AZONE> getZoneListByLine()
        {
            return zoneList.Where(z => z.LINE_ID.Trim() == line.LINE_ID.Trim()).ToList();
        }

        /// <summary>
        /// Gets the node list by zone.
        /// </summary>
        /// <param name="zone_id">The zone_id.</param>
        /// <returns>List&lt;Node&gt;.</returns>
        public List<ANODE> getNodeListByZone(string zone_id)
        {
            return nodeList.Where(n => n.ZONE_ID.Trim() == zone_id.Trim()).ToList();
        }

        /// <summary>
        /// Gets the node by node identifier.
        /// </summary>
        /// <param name="node_id">The node_id.</param>
        /// <returns>Node.</returns>
        public ANODE getNodeByNodeID(string node_id)
        {
            return nodeList.Where(n => n.NODE_ID.Trim() == node_id.Trim()).FirstOrDefault();
        }



        /// <summary>
        /// Gets the parent node by eqptid.
        /// </summary>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <returns>Node.</returns>
        public ANODE getParentNodeByEQPTID(string eqpt_id)
        {
            AEQPT eqpt = getEquipmentByEQPTID(eqpt_id);
            if (eqpt == null)
            {
                return null;
            }
            return nodeList.Where(n => n.NODE_ID.Trim() == eqpt.NODE_ID.Trim()).FirstOrDefault();
        }

        /// <summary>
        /// Gets the equipment by eqptid.
        /// </summary>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <returns>Equipment.</returns>
        public AEQPT getEquipmentByEQPTID(string eqpt_id)
        {
            AEQPT eqpt = null;
            try
            {
                eqpt = eqptList.Where(e => e.EQPT_ID.Trim() == eqpt_id.Trim()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
            return eqpt;
        }
        public AVEHICLE getVehicletByVHID(string vh_id)
        {
            AVEHICLE vh = null;
            try
            {
                vh = vhList.Where(e => e.VEHICLE_ID.Trim() == vh_id.Trim()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
            return vh;
        }

        /// <summary>
        /// Gets the equipment by eqpt real identifier.
        /// </summary>
        /// <param name="eqpt_real_id">The eqpt_real_id.</param>
        /// <returns>Equipment.</returns>
        public AEQPT getEquipmentByEQPTRealID(string eqpt_real_id)
        {
            return eqptList.Where(e => e.Real_ID.Trim() == eqpt_real_id.Trim()).FirstOrDefault();
        }

        /// <summary>
        /// Gets the euipment list by node.
        /// </summary>
        /// <param name="node_id">The node_id.</param>
        /// <returns>List&lt;Equipment&gt;.</returns>
        public List<AEQPT> getEuipmentListByNode(string node_id)
        {
            return eqptList.Where(e => e.NODE_ID.Trim() == node_id.Trim()).ToList();
        }

        public List<AEQPT> getAllEquipment()
        {
            return eqptList;
        }
        public List<AVEHICLE> getAllVehicle()
        {
            return vhList.OrderBy(data => data.VEHICLE_ID).ToList();
        }

        /// <summary>
        /// Gets the unit by unit identifier.
        /// </summary>
        /// <param name="unit_id">The unit_id.</param>
        /// <returns>Unit.</returns>
        public AUNIT getUnitByUnitID(string unit_id)
        {
            return unitList.Where(u => u.UNIT_ID.Trim() == unit_id.Trim()).FirstOrDefault();
        }

        /// <summary>
        /// Gets the unit by unit real identifier.
        /// </summary>
        /// <param name="unit_real_id">The unit_real_id.</param>
        /// <returns>Unit.</returns>
        public AUNIT getUnitByUnitRealID(string unit_real_id)
        {
            return unitList.Where(u => u.Real_ID.Trim() == unit_real_id.Trim()).FirstOrDefault();
        }

        /// <summary>
        /// Gets all unit.
        /// </summary>
        /// <returns>List&lt;Unit&gt;.</returns>
        public List<AUNIT> getAllUnit()
        {
            return unitList.ToList();
        }

        /// <summary>
        /// Gets all node.
        /// </summary>
        /// <returns>List&lt;Node&gt;.</returns>
        public List<ANODE> getAllNode()
        {
            return nodeList.ToList();
        }

        /// <summary>
        /// Gets all port.
        /// </summary>
        /// <returns>List&lt;Port&gt;.</returns>
        public List<APORT> getAllPort()
        {
            return portList.ToList();
        }
        public List<APORTSTATION> getALLPortStation()
        {
            return portStationList;
        }

        /// <summary>
        /// Gets all buffer.
        /// </summary>
        /// <returns>List&lt;BufferPort&gt;.</returns>
        public List<ABUFFER> getAllBuffer()
        {
            return buffList.ToList();
        }

        /// <summary>
        /// Gets the unit by unit number.
        /// </summary>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <param name="Unit_num">The unit_num.</param>
        /// <returns>Unit.</returns>
        public AUNIT getUnitByUnitNumber(string eqpt_id, int Unit_num)
        {
            return unitList.Where(u => u.EQPT_ID.Trim() == eqpt_id.Trim() && u.UNIT_NUM == Unit_num).FirstOrDefault();
        }

        /// <summary>
        /// Gets the base unit object by unit number.
        /// </summary>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <param name="unit_num">The unit_num.</param>
        /// <returns>BaseUnitObject.</returns>
        public BaseUnitObject getBaseUnitObjByUnitNumber(string eqpt_id, int unit_num)
        {
            AUNIT unit = unitList.Where(u => u.EQPT_ID.Trim() == eqpt_id.Trim() && u.UNIT_NUM == unit_num).FirstOrDefault();
            if (unit != null)
            {
                return unit;
            }
            ABUFFER buff = buffList.Where(b => b.EQPT_ID.Trim() == eqpt_id.Trim() && b.UNIT_NUM == unit_num).FirstOrDefault();
            if (buff != null)
            {
                return buff;
            }
            APORT port = portList.Where(p => p.EQPT_ID.Trim() == eqpt_id.Trim() && p.UNIT_NUM == unit_num).FirstOrDefault();
            return port;
        }

        /// <summary>
        /// Gets the unit list by equipment.
        /// </summary>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <returns>List&lt;Unit&gt;.</returns>
        public List<AUNIT> getUnitListByEquipment(string eqpt_id)
        {
            return unitList.Where(u => u.EQPT_ID.Trim() == eqpt_id.Trim()).ToList();
        }

        /// <summary>
        /// Gets the port by port identifier.
        /// </summary>
        /// <param name="port_id">The port_id.</param>
        /// <returns>Port.</returns>
        public APORT getPortByPortID(string port_id)
        {
            return portList.Where(p => p.PORT_ID.Trim() == port_id.Trim()).SingleOrDefault();
        }

        /// <summary>
        /// Gets the port by unit number.
        /// </summary>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <param name="unit_num">The unit_num.</param>
        /// <returns>Port.</returns>
        public APORT getPortByUnitNumber(string eqpt_id, int unit_num)
        {
            return portList.Where(p => p.EQPT_ID.Trim() == eqpt_id.Trim() && p.UNIT_NUM == unit_num).SingleOrDefault();
        }

        /// <summary>
        /// Gets the port by port number.
        /// </summary>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <param name="port_num">The port_num.</param>
        /// <returns>Port.</returns>
        public APORT getPortByPortNumber(string eqpt_id, int port_num)
        {
            return portList.Where(p => p.EQPT_ID.Trim() == eqpt_id.Trim() && p.PORT_NUM == port_num).SingleOrDefault();
        }


        /// <summary>
        /// Gets the port by cstid.
        /// </summary>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <param name="cst_id">The cst_id.</param>
        /// <returns>Port.</returns>
        public APORT getPortByCSTID(string eqpt_id, string cst_id)
        {
            return portList.Where(p => p.EQPT_ID.Trim() == eqpt_id.Trim()
                && SCUtility.Trim(p.CassetteLoader.CassetteItem.CST_ID) == cst_id.Trim()).SingleOrDefault();
        }

        /// <summary>
        /// Gets the port by lot identifier.
        /// </summary>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <param name="lot_id">The lot_id.</param>
        /// <returns>Port.</returns>
        public APORT getPortByLotID(string eqpt_id, string lot_id)
        {
            return portList.Where(p => p.EQPT_ID.Trim() == eqpt_id.Trim()
                && SCUtility.Trim(p.CassetteLoader.LotItem.LOT_ID) == lot_id.Trim()).SingleOrDefault();
        }

        /// <summary>
        /// Gets the port list by equipment.
        /// </summary>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <returns>List&lt;Port&gt;.</returns>
        public List<APORT> getPortListByEquipment(string eqpt_id)
        {
            return portList.Where(p => p.EQPT_ID.Trim() == eqpt_id.Trim()).ToList();
        }

        public APORTSTATION getPortStation(string port_id)
        {
            return portStationList.Where(p => p.PORT_ID.Trim() == port_id.Trim()).SingleOrDefault();
        }
        public APORTSTATION getPortStationByAdrID(string adrID)
        {
            return portStationList.Where(p => p.ADR_ID.Trim() == adrID.Trim()).SingleOrDefault();
        }


        /// <summary>
        /// Gets the buff by buff identifier.
        /// </summary>
        /// <param name="buff_id">The buff_id.</param>
        /// <returns>BufferPort.</returns>
        public ABUFFER getBuffByBuffID(string buff_id)
        {
            return buffList.Where(b => b.BUFF_ID.Trim() == buff_id.Trim()).SingleOrDefault();
        }

        /// <summary>
        /// Gets the buff by unit number.
        /// </summary>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <param name="unit_num">The unit_num.</param>
        /// <returns>BufferPort.</returns>
        public ABUFFER getBuffByUnitNumber(string eqpt_id, int unit_num)
        {
            return buffList.Where(b => b.EQPT_ID.Trim() == eqpt_id.Trim() && b.UNIT_NUM == unit_num).SingleOrDefault();
        }

        /// <summary>
        /// Gets the buff list by equipment.
        /// </summary>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <returns>List&lt;BufferPort&gt;.</returns>
        public List<ABUFFER> getBuffListByEquipment(string eqpt_id)
        {
            return buffList.Where(b => b.EQPT_ID.Trim() == eqpt_id.Trim()).ToList();
        }

        /// <summary>
        /// Gets the parent zone by node.
        /// </summary>
        /// <param name="node_id">The node_id.</param>
        /// <returns>Zone.</returns>
        public AZONE getParentZoneByNode(string node_id)
        {
            ANODE node = nodeList.Where(n => n.NODE_ID.Trim() == node_id.Trim()).FirstOrDefault();
            if (node == null) { return null; }
            return getZoneByZoneID(node.ZONE_ID);
        }

        /// <summary>
        /// Gets the parent zone by eqpt.
        /// </summary>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <returns>Zone.</returns>
        public AZONE getParentZoneByEQPT(string eqpt_id)
        {
            AEQPT eqpt = getEquipmentByEQPTID(eqpt_id);
            ANODE node = getNodeByNodeID(eqpt.NODE_ID);
            AZONE zone = getZoneByZoneID(node.ZONE_ID);
            return zone;
        }

        /// <summary>
        /// Gets the parent zone by port.
        /// </summary>
        /// <param name="port_id">The port_id.</param>
        /// <returns>Zone.</returns>
        public AZONE getParentZoneByPort(string port_id)
        {
            APORT port = getPortByPortID(port_id);
            AEQPT eqpt = getEquipmentByEQPTID(port.EQPT_ID);
            ANODE node = getNodeByNodeID(eqpt.NODE_ID);
            AZONE zone = getZoneByZoneID(node.ZONE_ID);
            return zone;
        }
        #endregion



        /// <summary>
        /// Sets the value to propety.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceObj">The source object.</param>
        /// <param name="destinationObj">The destination object.</param>
        private void setValueToPropety<T>(ref T sourceObj, ref T destinationObj)
        {
            BCFUtility.setValueToPropety(ref sourceObj, ref destinationObj);
        }

        #region 將最新物件資料，放置入Cache的方法

        /// <summary>
        /// Puts the specified source_line.
        /// </summary>
        /// <param name="source_line">The source_line.</param>
        public void put(ALINE source_line)
        {
            if (source_line == null) { return; }
            if (line == null)
            {
                //line = source_line;
                return;
            }
            if (!BCFUtility.isMatche(line.LINE_ID, source_line.LINE_ID))
            {
                return;
            }
            lock (_lockLine)
            {
                setValueToPropety<ALINE>(ref source_line, ref line);
            }
        }

        /// <summary>
        /// Puts the specified source_zone.
        /// </summary>
        /// <param name="source_zone">The source_zone.</param>
        public void put(AZONE source_zone)
        {
            if (source_zone == null) { return; }
            AZONE zone = getZoneByZoneID(source_zone.ZONE_ID);
            if (zone == null)
            {
                //zoneList.Add(source_zone);
                //_lockZoneDic.Add(source_zone.Zone_ID, new Object());
                return;
            }
            lock (_lockZoneDic[zone.ZONE_ID])
            {
                setValueToPropety<AZONE>(ref source_zone, ref zone);
            }
        }

        /// <summary>
        /// Puts the specified source_node.
        /// </summary>
        /// <param name="source_node">The source_node.</param>
        public void put(ANODE source_node)
        {
            if (source_node == null) { return; }
            ANODE node = getNodeByNodeID(source_node.NODE_ID);
            if (node == null)
            {
                //nodeList.Add(source_node);
                //_lockNodeDic.Add(source_node.Node_ID, new Object());
                return;
            }
            lock (_lockNodeDic[node.NODE_ID])
            {
                setValueToPropety<ANODE>(ref source_node, ref node);
            }
        }

        /// <summary>
        /// Puts the specified source_eqpt.
        /// </summary>
        /// <param name="source_eqpt">The source_eqpt.</param>
        public void put(AEQPT source_eqpt)
        {
            if (source_eqpt == null) { return; }
            AEQPT eqpt = getEquipmentByEQPTID(source_eqpt.EQPT_ID);
            if (eqpt == null)
            {
                //eqptList.Add(source_eqpt);
                //_lockEqptDic.Add(source_eqpt.Eqpt_ID, new Object());
                return;
            }
            lock (_lockEqptDic[eqpt.EQPT_ID])
            {
                if (eqpt != source_eqpt)
                {
                    setValueToPropety<AEQPT>(ref source_eqpt, ref eqpt);
                }
                scApp.LineBLL.updateEQPT(eqpt);////////////Hayes To Do..
            }
        }

        /// <summary>
        /// Puts the specified source_eqpt.
        /// </summary>
        /// <param name="source_vh">The source_eqpt.</param>
        public void put(AVEHICLE source_vh)
        {
            if (source_vh == null) { return; }
            AVEHICLE vh = getVehicletByVHID(source_vh.VEHICLE_ID);
            if (vh == null)
            {
                //eqptList.Add(source_eqpt);
                //_lockEqptDic.Add(source_eqpt.Eqpt_ID, new Object());
                return;
            }
            //lock (_lockVhDic[vh.VEHICLE_ID])
            //{
            if (vh != source_vh)
            {
                setValueToPropety<AVEHICLE>(ref source_vh, ref vh);
            }
            //}
        }
        public void update(AVEHICLE vh_bo)
        {
            if (vh_bo == null) { return; }
            AVEHICLE vh_vo = getVehicletByVHID(vh_bo.VEHICLE_ID);
            if (vh_vo == null)
            {
                //eqptList.Add(source_eqpt);
                //_lockEqptDic.Add(source_eqpt.Eqpt_ID, new Object());
                return;
            }
            //lock (_lockVhDic[vh.VEHICLE_ID])
            //{
            vh_vo.VEHICLE_TYPE = vh_bo.VEHICLE_TYPE;
            //vh_vo.CUR_ADR_ID = vh_bo.CUR_ADR_ID;
            //vh_vo.CUR_SEC_ID = vh_bo.CUR_SEC_ID;
            vh_vo.SEC_ENTRY_TIME = vh_bo.SEC_ENTRY_TIME;
            vh_vo.ACC_SEC_DIST = vh_bo.ACC_SEC_DIST;
            vh_vo.MODE_STATUS = vh_bo.MODE_STATUS;
            vh_vo.ACT_STATUS = vh_bo.ACT_STATUS;
            vh_vo.MCS_CMD = vh_bo.MCS_CMD;
            vh_vo.OHTC_CMD = vh_bo.OHTC_CMD;
            vh_vo.BLOCK_PAUSE = vh_bo.BLOCK_PAUSE;
            vh_vo.CMD_PAUSE = vh_bo.CMD_PAUSE;
            vh_vo.OBS_PAUSE = vh_bo.OBS_PAUSE;
            vh_vo.HID_PAUSE = vh_bo.HID_PAUSE;
            vh_vo.ERROR = vh_bo.ERROR;
            vh_vo.OBS_DIST = vh_bo.OBS_DIST;
            vh_vo.HAS_CST = vh_bo.HAS_CST;
            vh_vo.CST_ID = vh_bo.CST_ID;
            vh_vo.UPD_TIME = vh_bo.UPD_TIME;
            vh_vo.VEHICLE_ACC_DIST = vh_bo.VEHICLE_ACC_DIST;
            vh_vo.MANT_ACC_DIST = vh_bo.MANT_ACC_DIST;
            vh_vo.MANT_DATE = vh_bo.MANT_DATE;
            vh_vo.GRIP_COUNT = vh_bo.GRIP_COUNT;
            vh_vo.GRIP_MANT_COUNT = vh_bo.GRIP_MANT_COUNT;
            vh_vo.GRIP_MANT_DATE = vh_bo.GRIP_MANT_DATE;
            vh_vo.NODE_ADR = vh_bo.NODE_ADR;
            vh_vo.IS_PARKING = vh_bo.IS_PARKING;
            vh_vo.PARK_TIME = vh_bo.PARK_TIME;
            vh_vo.PARK_ADR_ID = vh_bo.PARK_ADR_ID;
            vh_vo.IS_CYCLING = vh_bo.IS_CYCLING;
            vh_vo.CYCLERUN_TIME = vh_bo.CYCLERUN_TIME;
            vh_vo.CYCLERUN_ID = vh_bo.CYCLERUN_ID;
            vh_vo.IS_INSTALLED = vh_bo.IS_INSTALLED;
            vh_vo.INSTALLED_TIME = vh_bo.INSTALLED_TIME;
            vh_vo.REMOVED_TIME = vh_bo.REMOVED_TIME;
            //if (vh != source_vh)
            //{
            //    setValueToPropety<AVEHICLE>(ref source_vh, ref vh);
            //}
            //}
        }
        /// <summary>
        /// Puts the specified source_unit.
        /// </summary>
        /// <param name="source_unit">The source_unit.</param>
        public void put(AUNIT source_unit)
        {
            if (source_unit == null) { return; }
            AUNIT unit = getUnitByUnitID(source_unit.UNIT_ID);
            if (unit == null)
            {
                //unitList.Add(source_unit);
                //_lockUnitDic.Add(source_unit.Unit_ID, new Object());
                return;
            }
            lock (_lockUnitDic[unit.UNIT_ID])
            {
                setValueToPropety<AUNIT>(ref source_unit, ref unit);
            }
        }

        /// <summary>
        /// Puts the specified source_port.
        /// </summary>
        /// <param name="source_port">The source_port.</param>
        public void put(APORT source_port)
        {
            if (source_port == null) { return; }
            APORT port = getPortByPortID(source_port.PORT_ID);
            if (port == null)
            {
                //portList.Add(source_port);
                //_lockPortDic.Add(source_port.Port_ID, new Object());
                return;
            }
            lock (_lockPortDic[port.PORT_ID])
            {
                setValueToPropety<APORT>(ref source_port, ref port);
            }
        }

        /// <summary>
        /// Puts the specified source_buff.
        /// </summary>
        /// <param name="source_buff">The source_buff.</param>
        public void put(ABUFFER source_buff)
        {
            if (source_buff == null) { return; }
            ABUFFER buff = getBuffByBuffID(source_buff.BUFF_ID);
            if (buff == null) { return; }
            lock (_lockBuffDic[buff.BUFF_ID])
            {
                setValueToPropety<ABUFFER>(ref source_buff, ref buff);
            }
        }
        public void put(APORTSTATION portStation)
        {
            if (portStation == null) { return; }
            APORTSTATION port_station = getPortStation(portStation.PORT_ID);
            if (port_station == null) { return; }
            lock (_lockPorStationtDic[port_station.PORT_ID])
            {
                setValueToPropety<APORTSTATION>(ref portStation, ref port_station);
            }
        }

        #endregion

        /// <summary>
        /// Removes the line.
        /// </summary>
        private void removeLine()
        {
            line = null;
        }

        #region 從DB取得最新EQ Object，並更新Cache
        /// <summary>
        /// Refreshes the line.
        /// </summary>
        public void refreshLine()
        {
            ALINE tmpLine = scApp.LineBLL.getLineByIDAndDeleteOtherLine(line.LINE_ID);
            //if (tmpLine == null) 
            //{
            //    removeLine();
            //    tmpLine = scApp.LineBLL.getFirstLine();
            //}
            put(tmpLine);
        }

        /// <summary>
        /// Refreshes the zone.
        /// </summary>
        /// <param name="zone_id">The zone_id.</param>
        public void refreshZone(string zone_id)
        {
            put(scApp.LineBLL.getZoneByZoneID(zone_id));
        }

        /// <summary>
        /// Refreshes the node.
        /// </summary>
        /// <param name="node_id">The node_id.</param>
        public void refreshNode(string node_id)
        {
            put(scApp.LineBLL.getNodeByNodeID(node_id));
        }

        /// <summary>
        /// Refreshes the eqpt.
        /// </summary>
        /// <param name="eqpt_id">The eqpt_id.</param>
        public void refreshEqpt(string eqpt_id)
        {
            put(scApp.LineBLL.getEqptByEqptID(eqpt_id));
        }

        /// <summary>
        /// Refreshes the eqpt.
        /// </summary>
        /// <param name="vh_id">The eqpt_id.</param>
        public void refreshVh(string vh_id)
        {
            update(scApp.VehicleBLL.getVehicleByIDFromDB(vh_id));
        }

        /// <summary>
        /// Refreshes the unit.
        /// </summary>
        /// <param name="unit_id">The unit_id.</param>
        public void refreshUnit(string unit_id)
        {
            put(scApp.LineBLL.getUnitByUnitID(unit_id));
        }

        /// <summary>
        /// Refreshes the port.
        /// </summary>
        /// <param name="port_id">The port_id.</param>
        public void refreshPort(string port_id)
        {
            put(scApp.LineBLL.getPortByPortID(port_id));
        }

        /// <summary>
        /// Refreshes the buffer.
        /// </summary>
        /// <param name="buff_id">The buff_id.</param>
        public void refreshBuffer(string buff_id)
        {
            put(scApp.LineBLL.getBufferPortByPortID(buff_id));
        }

        /// <summary>
        /// Refreshes all.
        /// </summary>
        public void refreshAll()
        {
            refreshLine();
            foreach (AZONE zone in zoneList)
            {
                refreshZone(zone.ZONE_ID);
            }
            foreach (ANODE node in nodeList)
            {
                refreshNode(node.NODE_ID);
            }
            foreach (AEQPT eqpt in eqptList)
            {
                refreshEqpt(eqpt.EQPT_ID);
            }
            foreach (AUNIT unit in unitList)
            {
                refreshUnit(unit.UNIT_ID);
            }
            foreach (APORT port in portList)
            {
                refreshPort(port.PORT_ID);
            }
            foreach (ABUFFER buff in buffList)
            {
                refreshBuffer(buff.BUFF_ID);
            }
        }
        #endregion
        /// <summary>
        /// A0.07
        /// 重新載入Alarm Desc File
        /// </summary>
        public void reloadAlarmDescFile()
        {
            foreach (AEQPT eqpt in eqptList)
            {
                //eqpt.reloadAlarmDesc();
            }
        }





    }
}
