//*********************************************************************************
//      SCApplication.cs
//*********************************************************************************
// File Name: SCApplication.cs
// Description: SCApplication
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2020/02/10    Kevin Wei      N/A            A0.01   加入Reserve模組
// 2020/05/24    Jason Wu       N/A            A0.02   新增DebugParameter.ignore136UnloadComplete 開關
//**********************************************************************************

using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.ConfigHandler;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.InitAction;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.bcf.Schedule;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.ConfigHandler;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.DAO;
using com.mirle.ibg3k0.sc.Data.DAO.EntityFramework;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.MQTT;
using com.mirle.ibg3k0.sc.RouteKit;
using com.mirle.ibg3k0.sc.Scheduler;
using com.mirle.ibg3k0.sc.Service;
using com.mirle.ibg3k0.sc.WIF;
using com.mirle.ibg3k0.stc.Common.SECS;
using ExcelDataReader;
using GenericParsing;
using Nancy;
using Nancy.Hosting.Self;
using NLog;
//using Predes.ZabbixSender;
using Quartz;
using Quartz.Impl;
using RouteKit;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace com.mirle.ibg3k0.sc.App
{
    /// <summary>
    /// Class SCApplication.
    /// </summary>
    public class SCApplication
    {


        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The eqpt CSS
        /// </summary> 
        private EQPTConfigSections eqptCss = null;
        /// <summary>
        /// The node flow relative CSS
        /// </summary>
        private NodeFlowRelConfigSections nodeFlowRelCss = null;

        private DataCollectionConfigSections dataCollectionCss = null;
        /// <summary>
        /// The eap secs agent name
        /// </summary>
        private string eapSecsAgentName;
        /// <summary>
        /// Gets the name of the eap secs agent.
        /// </summary>
        /// <value>The name of the eap secs agent.</value>
        public string EAPSecsAgentName { get { return eapSecsAgentName; } }
        /// <summary>
        /// Gets the b c_ identifier.
        /// </summary>
        /// <value>The b c_ identifier.</value>
        public string BC_ID { get; private set; }
        public static string ServerName { get; private set; }

        /// <summary>
        /// The _lock
        /// </summary>
        private static Object _lock = new Object();
        /// <summary>
        /// The application
        /// </summary>
        private static SCApplication application;
        /// <summary>
        /// The BCF application
        /// </summary>
        private static BCFApplication bcfApplication;
        /// <summary>
        /// The eq object cache manager
        /// </summary>
        private EQObjCacheManager eqObjCacheManager;
        private CommObjCacheManager commObjCacheManager;
        private RedisCacheManager redisCacheManager;
        private NatsManager natsManager;
        private ElasticSearchManager elasticSearchManager;
        private Mirle.Hlts.ReserveSection.Map.MapAPI _reserveSectionAPI { get; set; } //A0.01
        private Mirle.Hlts.ReserveSection.Map.ViewModels.HltMapViewModel reserveSectionAPI { get; set; }//A0.01

        public HAProxyConnectionTest hAProxyConnectionTest { get; private set; }
        //        const string REDIS_SERVER_CONFIGURATION = "redis.ohxc.mirle.com.tw:6379";
        public NancyHost NancyHost { get; private set; }
        public WebClientManager webClientManager { get; private set; }



        //DAO
        /// <summary>
        /// The line DAO
        /// </summary>
        private LineDao lineDao = null;
        /// <summary>
        /// Gets the line DAO.
        /// </summary>
        /// <value>The line DAO.</value>
        public LineDao LineDao { get { return lineDao; } }
        /// <summary>
        /// The zone DAO
        /// </summary>
        private ZoneDao zoneDao = null;
        /// <summary>
        /// Gets the zone DAO.
        /// </summary>
        /// <value>The zone DAO.</value>
        public ZoneDao ZoneDao { get { return zoneDao; } }
        /// <summary>
        /// The node DAO
        /// </summary>
        private NodeDao nodeDao = null;
        /// <summary>
        /// Gets the node DAO.
        /// </summary>
        /// <value>The node DAO.</value>
        public NodeDao NodeDao { get { return nodeDao; } }
        /// <summary>
        /// The eqpt DAO
        /// </summary>
        private EqptDao eqptDao = null;
        /// <summary>
        /// Gets the eqpt DAO.
        /// </summary>
        /// <value>The eqpt DAO.</value>
        public EqptDao EqptDao { get { return eqptDao; } }
        /// <summary>
        /// The flow relative DAO
        /// </summary>
        private FlowRelDao flowRelDao = null;
        /// <summary>
        /// Gets the flow relative DAO.
        /// </summary>
        /// <value>The flow relative DAO.</value>
        public FlowRelDao FlowRelDao { get { return flowRelDao; } }
        /// <summary>
        /// The unit DAO
        /// </summary>
        private UnitDao unitDao = null;
        /// <summary>
        /// Gets the unit DAO.
        /// </summary>
        /// <value>The unit DAO.</value>
        public UnitDao UnitDao { get { return unitDao; } }
        /// <summary>
        /// The port DAO
        /// </summary>
        private PortDao portDao = null;
        /// <summary>
        /// Gets the port DAO.
        /// </summary>
        /// <value>The port DAO.</value>
        public PortDao PortDao { get { return portDao; } }
        private PortStationDao portStationDao = null;
        public PortStationDao PortStationDao { get { return portStationDao; } }
        /// <summary>
        /// The buffer port DAO
        /// </summary>
        private BufferPortDao bufferPortDao = null;
        /// <summary>
        /// Gets the buffer port DAO.
        /// </summary>
        /// <value>The buffer port DAO.</value>
        public BufferPortDao BufferPortDao { get { return bufferPortDao; } }
        /// <summary>
        /// The user DAO
        /// </summary>
        private UserDao userDao = null;
        /// <summary>
        /// Gets the user DAO.
        /// </summary>
        /// <value>The user DAO.</value>
        public UserDao UserDao { get { return userDao; } }
        /// <summary>
        /// The function code DAO
        /// </summary>
        private FunctionCodeDao functionCodeDao = null;
        /// <summary>
        /// Gets the function code DAO.
        /// </summary>
        /// <value>The function code DAO.</value>
        public FunctionCodeDao FunctionCodeDao { get { return functionCodeDao; } }
        /// <summary>
        /// The user function DAO
        /// </summary>
        private UserFuncDao userFuncDao = null;
        /// <summary>
        /// Gets the user function DAO.
        /// </summary>
        /// <value>The user function DAO.</value>
        public UserFuncDao UserFuncDao { get { return userFuncDao; } }
        /// <summary>
        /// The alarm DAO
        /// </summary>
        private AlarmDao alarmDao = null;
        /// <summary>
        /// Gets the alarm DAO.
        /// </summary>
        /// <value>The alarm DAO.</value>
        public AlarmDao AlarmDao { get { return alarmDao; } }

        /// <summary>
        /// The alarm DAO
        /// </summary>
        private MainAlarmDao mainalarmDao = null;
        /// <summary>
        /// Gets the alarm DAO.
        /// </summary>
        /// <value>The alarm DAO.</value>
        public MainAlarmDao MainAlarmDao { get { return mainalarmDao; } }



        /// <summary>
        /// The cassette DAO
        /// </summary>
        private CassetteDao cassetteDao = null;
        /// <summary>
        /// Gets the cassette DAO.
        /// </summary>
        /// <value>The cassette DAO.</value>
        public CassetteDao CassetteDao { get { return cassetteDao; } }
        /// <summary>
        /// The bc status DAO
        /// </summary>
        private BCStatusDao bcStatusDao = null;
        /// <summary>
        /// Gets the bc status DAO.
        /// </summary>
        /// <value>The bc status DAO.</value>
        public BCStatusDao BCStatusDao { get { return bcStatusDao; } }
        /// <summary>
        /// The sequence DAO
        /// </summary>
        private SequenceDao sequenceDao = null;
        /// <summary>
        /// Gets the sequence DAO.
        /// </summary>
        /// <value>The sequence DAO.</value>
        public SequenceDao SequenceDao { get { return sequenceDao; } }
        /// <summary>
        /// The event RPT cond DAO
        /// </summary>
        private EventRptCondDao eventRptCondDao = null;
        /// <summary>
        /// Gets the event RPT cond DAO.
        /// </summary>
        /// <value>The event RPT cond DAO.</value>
        public EventRptCondDao EventRptCondDao { get { return eventRptCondDao; } }
        /// <summary>
        /// The crate DAO
        /// </summary>
        private CrateDao crateDao = null;
        /// <summary>
        /// Gets the crate DAO.
        /// </summary>
        /// <value>The crate DAO.</value>
        public CrateDao CrateDao { get { return crateDao; } }
        /// <summary>
        /// The alarm RPT cond DAO
        /// </summary>
        private AlarmRptCondDao alarmRptCondDao = null;
        /// <summary>
        /// Gets the alarm RPT cond DAO.
        /// </summary>
        /// <value>The alarm RPT cond DAO.</value>
        public AlarmRptCondDao AlarmRptCondDao { get { return alarmRptCondDao; } }
        /// <summary>
        /// The trace set DAO
        /// </summary>
        private TraceSetDao traceSetDao = null;
        /// <summary>
        /// Gets the trace set DAO.
        /// </summary>
        /// <value>The trace set DAO.</value>
        public TraceSetDao TraceSetDao { get { return traceSetDao; } }
        /// <summary>
        /// Gets the operation his DAO.
        /// </summary>
        /// <value>The operation his DAO.</value>
        public OperationHisDao OperationHisDao { get; private set; } = null;


        public ReturnCodeMapDao ReturnCodeMapDao { get; private set; } = null;





        /// <summary>
        /// The alarm map DAO
        /// </summary>
        private AlarmMapDao alarmMapDao = null;
        /// <summary>
        /// Gets the alarm map DAO.
        /// </summary>
        /// <value>The alarm map DAO.</value>
        public AlarmMapDao AlarmMapDao { get { return alarmMapDao; } }
        private UserGroupDao userGroupDao = null;
        public UserGroupDao UserGroupDao { get { return userGroupDao; } }
        /// <summary>
        /// The ec data map DAO
        /// </summary>
        private ECDataMapDao ecDataMapDao = null;
        /// <summary>
        /// Gets the ec data map DAO.
        /// </summary>
        /// <value>The ec data map DAO.</value>
        public ECDataMapDao ECDataMapDao { get { return ecDataMapDao; } }
        private CEIDDao ceidDao = null;
        public CEIDDao CEIDDao { get { return ceidDao; } }
        private RPTIDDao rptidDao = null;
        public RPTIDDao RPTIDDao { get { return rptidDao; } }



        private RAILDao railDao = null;
        public RAILDao RailDao { get { return railDao; } }
        private ADDRESSDao addressDao = null;
        public ADDRESSDao AddressDao { get { return addressDao; } }
        private PortIconDao porticonDao = null;
        public PortIconDao PortIconDao { get { return porticonDao; } }

        private POINTDao pointDao = null;
        public POINTDao PointDao { get { return pointDao; } }
        private GROUPRAILSDao groupRailDao = null;
        public GROUPRAILSDao GroupRailDao { get { return groupRailDao; } }

        private SectionDao sectionDao = null;
        public SectionDao SectionDao { get { return sectionDao; } }
        private SegmentDao segmentDao = null;
        public SegmentDao SegmentDao { get { return segmentDao; } }
        private VehicleDao vehicleDao = null;
        public VehicleDao VehicleDao { get { return vehicleDao; } }

        private CMD_OHTCDao cmd_ohtcDao = null;
        public CMD_OHTCDao CMD_OHTCDao { get { return cmd_ohtcDao; } }
        private CMD_OHTC_DetailDao cmd_ohtc_detailDao = null;
        public CMD_OHTC_DetailDao CMD_OHT_DetailDao { get { return cmd_ohtc_detailDao; } }


        private BlockZoneDetailDao bolckZoneDetaiDao = null;
        public BlockZoneDetailDao BolckZoneDetaiDao { get { return bolckZoneDetaiDao; } }
        private BlockZoneMasterDao blockZoneMasterDao = null;
        public BlockZoneMasterDao BlockZoneMasterDao { get { return blockZoneMasterDao; } }
        private BlockZoneQueueDao blockZoneQueueDao = null;
        public BlockZoneQueueDao BlockZoneQueueDao { get { return blockZoneQueueDao; } }


        private ParkZoneDetailDao parkZoneDetailDao = null;
        public ParkZoneDetailDao ParkZoneDetailDao { get { return parkZoneDetailDao; } }
        private ParkZoneMasterDao parkZoneMasterDao = null;
        public ParkZoneMasterDao ParkZoneMasterDao { get { return parkZoneMasterDao; } }
        private ParkZoneTypeDao parkZoneTypeDao = null;
        public ParkZoneTypeDao ParkZoneTypeDao { get { return parkZoneTypeDao; } }


        private CycleZoneDetailDao cyclezoneDdetailDao = null;
        public CycleZoneDetailDao CycleZoneDetailDao { get { return cyclezoneDdetailDao; } }
        private CycleZoneMasterDao cyclezonemasterDao = null;
        public CycleZoneMasterDao CycleZoneMasterDao { get { return cyclezonemasterDao; } }
        private CycleZoneTypeDao cyclezonetypeDao = null;
        public CycleZoneTypeDao CycleZoneTypeDao { get { return cyclezonetypeDao; } }
        private CMD_MCSDao cmd_mcsDao = null;
        public CMD_MCSDao CMD_MCSDao { get { return cmd_mcsDao; } }
        private VIDINFODao vidinfoDao = null;
        public VIDINFODao VIDINFODao { get { return vidinfoDao; } }
        private NetworkQualityDao networkqualityDao = null;
        public NetworkQualityDao NetworkQualityDao { get { return networkqualityDao; } }
        private APSettingDao apsettiongDao = null;
        public APSettingDao APSettingDao { get { return apsettiongDao; } }
        private SysExcuteQualityDao sysexcutequalityDao = null;
        public SysExcuteQualityDao SysExcuteQualityDao { get { return sysexcutequalityDao; } }
        private MCSReportQueueDao mcsreportqueueDao = null;
        public MCSReportQueueDao MCSReportQueueDao { get { return mcsreportqueueDao; } }
        private FlexsimCommandDao flexsimcommandDao = null;
        public FlexsimCommandDao FlexsimCommandDao { get { return flexsimcommandDao; } }

        private PortDefDao portdefDao = null;
        public PortDefDao PortDefDao { get { return portdefDao; } }
        private ZoneDefDao zonedefDao = null;
        public ZoneDefDao ZoneDefDao { get { return zonedefDao; } }
        private ShelfDefDao shelfdefDao = null;
        public ShelfDefDao ShelfDefDao { get { return shelfdefDao; } }
        private CassetteDataDao cassettedataDao = null;
        public CassetteDataDao CassetteDataDao { get { return cassettedataDao; } }


        private ViewSectionDao vSection100Dao = null;
        public ViewSectionDao VSection100Dao { get { return vSection100Dao; } }
        private AddressDataDao addressDataDao = null;
        public AddressDataDao AddressDataDao { get { return addressDataDao; } }
        private ScaleBaseDataDao scaleBaseDataDao = null;
        public ScaleBaseDataDao ScaleBaseDataDao { get { return scaleBaseDataDao; } }
        private ControlDataDao controlDataDao = null;
        public ControlDataDao ControlDataDao { get { return controlDataDao; } }
        private VehicleControlDao vehicleControlDao = null;
        public VehicleControlDao VehicleControlDao { get { return vehicleControlDao; } }
        private DataCollectionDao dataCollectionDao = null;
        public DataCollectionDao DataCollectionDao { get { return dataCollectionDao; } }
        private EqptLocationInfoDao eqptLocationInfoDao = null;
        public EqptLocationInfoDao EqptLocationInfoDao { get { return eqptLocationInfoDao; } }

        private HIDZoneMasterDao hidzonemasterDao = null;
        public HIDZoneMasterDao HIDZoneMasterDao { get { return hidzonemasterDao; } }
        private HIDZoneDetailDao hidzonedetailDao = null;
        public HIDZoneDetailDao HIDZoneDetailDao { get { return hidzonedetailDao; } }
        private HIDZoneQueueDao hidzonequeueDao = null;
        public HIDZoneQueueDao HIDZoneQueueDao { get { return hidzonequeueDao; } }

        private TestTranTaskDao testtrantaskDao = null;
        public TestTranTaskDao TestTranTaskDao { get { return testtrantaskDao; } }
        private ReserveEnhanceInfoDao rserveEnhanceInfoDao = null;
        public ReserveEnhanceInfoDao ReserveEnhanceInfoDao { get { return rserveEnhanceInfoDao; } }

        private HCMD_MCSDao hcmd_mcsDao = null;
        public HCMD_MCSDao HCMD_MCSDao { get { return hcmd_mcsDao; } }
        private HCMD_OHTCDao hcmd_ohtcDao = null;
        public HCMD_OHTCDao HCMD_OHTCDao { get { return hcmd_ohtcDao; } }

        //BLL
        /// <summary>
        /// The user BLL
        /// </summary>
        private UserBLL userBLL = null;
        /// <summary>
        /// Gets the user BLL.
        /// </summary>
        /// <value>The user BLL.</value>
        public UserBLL UserBLL { get { return userBLL; } }
        /// <summary>
        /// The bc system BLL
        /// </summary>
        private BCSystemBLL bcSystemBLL = null;
        /// <summary>
        /// Gets the bc system BLL.
        /// </summary>
        /// <value>The bc system BLL.</value>
        public BCSystemBLL BCSystemBLL { get { return bcSystemBLL; } }
        /// <summary>
        /// Gets the line BLL.
        /// </summary>
        /// <value>The line BLL.</value>
        public LineBLL LineBLL { get { return lineBLL; } }
        /// <summary>
        /// The line BLL
        /// </summary>
        private LineBLL lineBLL = null;
        /// <summary>
        /// Gets the alarm BLL.
        /// </summary>
        /// <value>The alarm BLL.</value>
        public AlarmBLL AlarmBLL { get { return alarmBLL; } }
        /// <summary>
        /// The alarm BLL
        /// </summary>
        private AlarmBLL alarmBLL = null;
        /// <summary>
        /// The sequence BLL
        /// </summary>
        private SequenceBLL sequenceBLL = null;
        /// <summary>
        /// Gets the sequence BLL.
        /// </summary>
        /// <value>The sequence BLL.</value>
        public SequenceBLL SequenceBLL { get { return sequenceBLL; } }
        /// <summary>
        /// The event BLL
        /// </summary>
        private EventBLL eventBLL = null;
        /// <summary>
        /// Gets the event BLL.
        /// </summary>
        /// <value>The event BLL.</value>
        public EventBLL EventBLL { get { return eventBLL; } }

        /// <summary>
        /// The report BLL
        /// </summary>
        private ReportBLL reportBLL = null;
        /// <summary>
        /// Gets the report BLL.
        /// </summary>
        /// <value>The report BLL.</value>
        public ReportBLL ReportBLL { get { return reportBLL; } }

        private MapBLL mapBLL = null;
        public MapBLL MapBLL { get { return mapBLL; } }

        private VehicleBLL vehicleBLL = null;
        public VehicleBLL VehicleBLL { get { return vehicleBLL; } }
        private CMDBLL cmdBLL = null;
        public CMDBLL CMDBLL { get { return cmdBLL; } }
        private ParkBLL parkBLL = null;
        public ParkBLL ParkBLL { get { return parkBLL; } }
        private CycleRunBLL cycleBLL = null;
        public CycleRunBLL CycleBLL { get { return cycleBLL; } }
        private CEIDBLL ceidBLL = null;
        public CEIDBLL CEIDBLL { get { return ceidBLL; } }
        private VIDBLL vidBLL = null;
        public VIDBLL VIDBLL { get { return vidBLL; } }
        private NetworkQualityBLL networkqualityBLL = null;
        public NetworkQualityBLL NetWorkQualityBLL { get { return networkqualityBLL; } }
        private SysExcuteQualityBLL sysexcutequalityBLL = null;
        public SysExcuteQualityBLL SysExcuteQualityBLL { get { return sysexcutequalityBLL; } }
        private BlockControlBLL blockcontrolBLL = null;
        public BlockControlBLL BlockControlBLL { get { return blockcontrolBLL; } }
        private SectionBLL sectinoBLL = null;
        public SectionBLL SectionBLL { get { return sectinoBLL; } }
        private SegmentBLL segmentBLL = null;
        public SegmentBLL SegmentBLL { get { return segmentBLL; } }
        private EquipmentBLL equipmentBLL = null;
        public EquipmentBLL EquipmentBLL { get { return equipmentBLL; } }
        private GuideBLL guideBLL = null;
        public GuideBLL GuideBLL { get { return guideBLL; } }

        /// <summary>
        /// 在OHT尚未改成新格式前先保留
        /// </summary>
        //private BlockControlService blockcontrolServer = null;
        //public BlockControlService BlockControlServer { get { return blockcontrolServer; } }

        private VehicleService vehicleService = null;
        public VehicleService VehicleService { get { return vehicleService; } }
        private LineService lineService = null;
        public LineService LineService { get { return lineService; } }
        private PortStationService portStationService = null;
        public PortStationService PortStationService { get { return portStationService; } }
        private ConnectionInfoService connectionInfoService = null;
        public ConnectionInfoService ConnectionInfoService { get { return connectionInfoService; } }
        private UserControlService userControlService = null;
        public UserControlService UserControlService { get { return userControlService; } }
        private TransferService transferService = null;
        public TransferService TransferService { get { return transferService; } }
        private FailOverService failOverService = null;
        public FailOverService FailOverService { get { return failOverService; } }
        private MTLService mtlService = null;
        public MTLService MTLService { get { return mtlService; } }
        private RoadControlService roadControlService = null;
        public RoadControlService RoadControlService { get { return roadControlService; } }
        private BlockControlService blockControlService = null;
        public BlockControlService BlockControlService { get { return blockControlService; } }
        private ShelfService shelfService = null;
        public ShelfService ShelfService { get => shelfService; }

        private OHCVService ohcvService = null;
        public OHCVService OHCVService { get { return ohcvService; } }

        private EmptyBoxHandlerService emptyBoxHandlerService = null;
        public EmptyBoxHandlerService EmptyBoxHandlerService { get { return emptyBoxHandlerService; } }

        private DataSyncBLL datasynBLL = null;
        public DataSyncBLL DataSyncBLL { get { return datasynBLL; } }


        private HIDBLL hidBLL = null;
        public HIDBLL HIDBLL { get { return hidBLL; } }
        public CheckSystemEventHandler CheckSystemEventHandler { get; private set; } = null;

        public PortBLL PortBLL { get; private set; } = null;
        public PortStationBLL PortStationBLL { get; private set; } = null;
        public NodeBLL NodeBLL { get; private set; } = null;
        public PortDefBLL PortDefBLL { get; private set; } = null;
        public ZoneDefBLL ZoneDefBLL { get; private set; } = null;
        public ShelfDefBLL ShelfDefBLL { get; private set; } = null;
        public CassetteDataBLL CassetteDataBLL { get; private set; } = null;
        public ReserveBLL ReserveBLL { get; private set; } = null; //A0.01

        //WIF
        /// <summary>
        /// The bc system wif
        /// </summary>
        private BCSystemWIF bcSystemWIF = null;
        /// <summary>
        /// Gets the bc system wif.
        /// </summary>
        /// <value>The bc system wif.</value>
        public BCSystemWIF BCSystemWIF { get { return bcSystemWIF; } }
        /// <summary>
        /// The line wif
        /// </summary>
        private LineWIF lineWIF = null;
        /// <summary>
        /// Gets the line wif.
        /// </summary>
        /// <value>The line wif.</value>
        public LineWIF LineWIF { get { return lineWIF; } }


        private Guide routeGuide = null;
        public Guide RouteGuide { get { return routeGuide; } }
        private IRouteGuide newrouteGuide = null;
        public IRouteGuide NewRouteGuide { get { return newrouteGuide; } }

        //config
        /// <summary>
        /// The indxer configuration
        /// </summary>
        private DataSet ohxcConfig = null;
        /// <summary>
        /// Gets the indxer configuration.
        /// </summary>
        /// <value>The indxer configuration.</value>
        public DataSet OHxCConfig { get { return ohxcConfig; } }
        public DataSet TranCmdPeriodicDataSet;

        public List<DataCollectionSetting> DataCollectionList { get; private set; }
        public List<ASECTION> CatchDataFromDB_Section { get; private set; }


        //BackgroundPLCWorkDriver
        /// <summary>
        /// Gets the background work sample.
        /// </summary>
        /// <value>The background work sample.</value>
        public BackgroundWorkDriver BackgroundWorkSample { get; private set; }              //A0.03


        public IScheduler Scheduler { get; private set; }


        //pool
        /// <summary>
        /// The v event list pool
        /// </summary>
        public ObjectPool<List<ValueRead>> vEventListPool = new ObjectPool<List<ValueRead>>(() => new List<ValueRead>());
        /// <summary>
        /// The v write list
        /// </summary>
        public ObjectPool<List<ValueWrite>> vWriteList = new ObjectPool<List<ValueWrite>>(() => new List<ValueWrite>());
        /// <summary>
        /// The convert value
        /// </summary>
        public ObjectPool<Dictionary<string, string>> convertValue = new ObjectPool<Dictionary<string, string>>(() => new Dictionary<string, string>());

        public ObjectPool<Stopwatch> StopwatchPool = new ObjectPool<Stopwatch>(() => new Stopwatch());
        public ObjectPool<AVEHICLE> VehiclPool = new ObjectPool<AVEHICLE>(() => new AVEHICLE());

        private ConcurrentDictionary<string, ObjectPool<PLC_FunBase>> dicPLC_FunBasePool =
         new ConcurrentDictionary<string, ObjectPool<PLC_FunBase>>();

        public PLC_FunBase getFunBaseObj<T>(string _id)
        {
            string type_name = typeof(T).Name;

            ObjectPool<PLC_FunBase> plc_funbase = dicPLC_FunBasePool.GetOrAdd(type_name, new ObjectPool<PLC_FunBase>(() => (PLC_FunBase)Activator.CreateInstance(typeof(T))));

            PLC_FunBase fun_base = plc_funbase.GetObject();
            fun_base.initial(_id);
            return fun_base;
        }
        public void putFunBaseObj<T>(PLC_FunBase put_obj)
        {
            if (put_obj == null) return;
            string type_name = typeof(T).Name;
            if (!dicPLC_FunBasePool.Keys.Contains(type_name))
            {
                return;
            }
            dicPLC_FunBasePool[type_name].PutObject(put_obj);
        }



        //public SenderService ZabbixService { get; private set; }
        /// <summary>
        /// The string builder
        /// </summary>
        public ObjectPool<StringBuilder> stringBuilder = new ObjectPool<StringBuilder>(() => new StringBuilder(""));
        public string mqttTopic;
        public string mqttMsg;
        public MQTTControl mqttControl;
        public object park_lock_obj = new object();
        /// <summary>
        /// Prevents a default instance of the <see cref="SCApplication"/> class from being created.
        /// </summary>
        private SCApplication()
        {
            init();
        }
        /// <summary>
        /// The build value function
        /// </summary>
        private static BCFApplication.BuildValueEventDelegate buildValueFunc;
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="_buildValueFunc">The _build value function.</param>
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// <returns>SCApplication.</returns>
        public static SCApplication getInstance(string server_name, BCFApplication.BuildValueEventDelegate _buildValueFunc)
        {
            ServerName = server_name;
            buildValueFunc = _buildValueFunc;
            return getInstance();
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns>SCApplication.</returns>
        public static SCApplication getInstance()
        {
            if (application == null)
            {
                lock (_lock)
                {
                    if (application == null)
                    {
                        application = new SCApplication();
                    }
                }
            }
            return application;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void init()
        {
            //mqttControl = new MQTTControl();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(new string('=', 80));
            sb.AppendLine("Do SCApplication Initialize...");
            sb.AppendLine(string.Format("Version: {0}", SCAppConstants.getMainFormVersion("")));
            sb.AppendLine(string.Format("Build Date: {0}", SCAppConstants.GetBuildDateTime().ToString()));
            sb.AppendLine(new string('=', 80));
            logger.Info(sb.ToString());
            sb.Clear();
            sb = null;

            bcfApplication = BCFApplication.getInstance(buildValueFunc);

            eapSecsAgentName = getString("EAPSecsAgentName", "");
            BC_ID = getString("BC_ID", "BC_ID");

            SystemParameter.SECSConversactionTimeout = getInt("SECSConversactionTimeout", 60);

            SystemParameter.setIsEnableIDReadFailScenarioFlag(getBoolean("IsEnableIDReadFailScenario", false));

            initDao();      //Initial DAO
            initBLL();      //Initial BLL
            initServer();
            initConfig();   //Initial Config
            initialTransferCommandPeriodicDataSet();

            eqptCss = bcfApplication.getEQPTConfigSections();
            //            mapActionCss = bcfApplication.getMapActionConfigSections();
            nodeFlowRelCss = bcfApplication.getNodeFlowRelConfigSections();
            eqObjCacheManager = EQObjCacheManager.getInstance();
            eqObjCacheManager.setContext(eqptCss, nodeFlowRelCss);
            commObjCacheManager = CommObjCacheManager.getInstance();
            elasticSearchManager = ElasticSearchManager.getInstance();

            redisCacheManager = new RedisCacheManager(this, BC_ID);
            natsManager = new NatsManager(this, BC_ID, "nats-cluster", SCApplication.ServerName);
            webClientManager = WebClientManager.getInstance();

            dataCollectionCss = (DataCollectionConfigSections)ConfigurationManager.GetSection(SCAppConstants.CONFIG_DATA_COLLECTION_SETTING);

            initialReserveSectionAPI();//A0.01
            iniOHBC_Data();
            startBLL();
            initWIF();      //Initial WIF   //A0.01
            initialCatchDataFromDB();
            commObjCacheManager.start(this);

            var IpAndPort = ReportBLL.getZabbixServerIPAndPort();

            //ZabbixService = new SenderService(IpAndPort.Item1, IpAndPort.Item2);
            //ReportBLL.ZabbixPush
            //                (SCAppConstants.ZabbixServerInfo.ZABBIX_OHXC_ALIVE, SCAppConstants.ZabbixOHxCAlive.ZABBIX_OHXC_ALIVE_INITIAL);
            routeGuide = new Guide();
            routeGuide.start(this);
            bool importMapFlag = routeGuide.ImportMap();
            //            startBLL();
            initialFloydAlgorithm();

            initBackgroundWork();               //A0.03
            initScheduler();
            bcfApplication.injectDataIllegalCheck(com.mirle.ibg3k0.sc.Data.SECS.SECSConst.checkDataValue);
            bcfApplication.injectSFTypeCheck(com.mirle.ibg3k0.sc.Data.SECS.SECSConst.checkSFType);
            //bcfApplication.injectUnPackWrapperMsg(com.mirle.ibg3k0.sc.Data.ValueDefMapAction.EQTcpIpMapAction.unPackWrapperMsg);
            switch (BC_ID)
            {
                default:
                    bcfApplication.injectTcpIpDecoder(new com.mirle.iibg3k0.ttc.Common.TCPIP.DecodRawData.DecodeRawData_Google(
    com.mirle.ibg3k0.sc.Data.ValueDefMapAction.EQTcpIpMapAction.unPackWrapperMsg));
                    break;
            };
            bcfApplication.setTcpIpNeedToBoConfirmReceivePacketID(com.mirle.ibg3k0.sc.Data.TcpIp.VHMSGIF.NeedToConfirmPacketIDCollection_String);
            hAProxyConnectionTest = new HAProxyConnectionTest(this);

            initialRestfulServer();


            //
            //loadECDataToSystem();
            //bdTableWatcher = new DBTableWatcher(this);


        }

        //A0.01
        private void initialReserveSectionAPI()
        {
            _reserveSectionAPI = new Mirle.Hlts.ReserveSection.Map.MapAPI();
            reserveSectionAPI = _reserveSectionAPI.MapVM;

            setHltVehicleInfo();

            LoadMapFiles();
        }

        private void iniOHBC_Data()
        {
            SystemParameter.cmdPriorityAdd = getInt("cmdPriorityAdd", 1);

            if (SystemParameter.cmdPriorityAdd <= 0)
            {
                SystemParameter.cmdPriorityAdd = 1;
            }

            SystemParameter.cmdTimeOutToAlternate = getInt("cmdTimeOutToAlternate", 30);

            if (SystemParameter.cmdTimeOutToAlternate <= 0)
            {
                SystemParameter.cmdTimeOutToAlternate = 30;
            }
        }

        //A0.01
        private void setHltVehicleInfo()
        {
            int vh_highi = getInt("VehicleHeight", 1800);
            int vh_width = getInt("VehicleWidth", 3200);
            int vh_sensor_wlength = getInt("SensorWLength", 1200);
            reserveSectionAPI.VehicleHeight = vh_highi;
            reserveSectionAPI.VehicleWidth = vh_width;
            reserveSectionAPI.SensorLength = vh_sensor_wlength;
        }
        //A0.01
        private void LoadMapFiles(string addressPath = null, string sectionPath = null)
        {
            try
            {
                string map_info_path = Environment.CurrentDirectory + this.getString("CsvConfig", "");
                if (addressPath == null) addressPath = $@"{map_info_path}\MapInfo\AADDRESS.csv";
                reserveSectionAPI.LoadMapAddresses(addressPath);

                if (sectionPath == null) sectionPath = $@"{map_info_path}\MapInfo\ASECTION.csv";
                reserveSectionAPI.LoadMapSections(sectionPath);
            }
            finally { }
        }


        private void initialFloydAlgorithm()
        {
            double moveCostForward = getDouble("MoveCostForward", 1);
            double moveCostReverse = getDouble("MoveCostReverse", 1);
            string algorithm = getString("ShortestPathAlgorithm", "DIJKSTRA");
            FloydAlgorithmRouteGuide routeGuideTemp =
                new FloydAlgorithmRouteGuide(commObjCacheManager.getSections(),
                                             moveCostForward, moveCostReverse, BC_ID, algorithm);
            newrouteGuide = routeGuideTemp;
            NewRouteGuide.resetBanRoute();
        }

        private void initialRestfulServer()
        {
            HostConfiguration hostConfigs = new HostConfiguration()
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true }
            };
            NancyHost = new NancyHost(new Uri("http://localhost:3280"), new DefaultNancyBootstrapper(), hostConfigs);

            //NancyHost = new NancyHost(new Uri("http://localhost:9527"), hostConfigs);
        }

        //DBTableWatcher bdTableWatcher = null;


        private void initialCatchDataFromDB()
        {
            CatchDataFromDB_Section = mapBLL.loadAllSection();

            DataCollectionList = loadDataCollectionSetting();
        }

        public void updateCatchData_Section(ASECTION section)
        {
            var catch_vo = CatchDataFromDB_Section.Where(sec => sec.SEC_ID == section.SEC_ID).SingleOrDefault();
            catch_vo.SEC_DIS = section.SEC_DIS;
            catch_vo.LAST_TECH_TIME = section.LAST_TECH_TIME;

        }

        private List<DataCollectionSetting> loadDataCollectionSetting()
        {
            List<DataCollectionSetting> DataCollectionList = new List<DataCollectionSetting>();
            foreach (DataCollectionConfigSection section in dataCollectionCss.DataCollectionConfigSectionList)
            {
                foreach (DataCollectionConfigItem item in section.DataCollectionItemList)
                {
                    DataCollectionSetting setting = new DataCollectionSetting()
                    {
                        Method = section.Name,
                        IP = section.IP,
                        Port = section.Port,
                        ItemName = item.Name,
                        Period = item.Period,
                        IsReport = item.IsReport
                    };
                    DataCollectionList.Add(setting);
                }
            }
            return DataCollectionList;
        }

        /// <summary>
        /// Loads the ec data to system.
        /// </summary>
        public void loadECDataToSystem()
        {
            //not implement
        }

        /// <summary>
        /// Loads the eqpt parameters to system.
        /// </summary>
        public void loadEQPTParametersToSystem()
        {
            //not implement
        }


        #region SECS Agent Parameter Change
        /// <summary>
        /// Sets the secs agent device identifier.
        /// </summary>
        /// <param name="deviceID">The device identifier.</param>
        /// <param name="refresh">The refresh.</param>
        public void setSECSAgentDeviceID(int deviceID, Boolean refresh)
        {
            SECSAgent agent = bcfApplication.getSECSAgent(eapSecsAgentName);
            agent.setDeviceID(deviceID);
            if (refresh)
            {
                agent.refreshConnection();
            }
        }

        /// <summary>
        /// Sets the secs agent t3 timeout.
        /// </summary>
        /// <param name="t3Timeout">The t3 timeout.</param>
        /// <param name="refresh">The refresh.</param>
        public void setSECSAgentT3Timeout(int t3Timeout, Boolean refresh)
        {
            SECSAgent agent = bcfApplication.getSECSAgent(eapSecsAgentName);
            agent.setT3(t3Timeout);
            if (refresh)
            {
                agent.refreshConnection();
            }
        }

        /// <summary>
        /// Sets the secs agent t5 timeout.
        /// </summary>
        /// <param name="t5Timeout">The t5 timeout.</param>
        /// <param name="refresh">The refresh.</param>
        public void setSECSAgentT5Timeout(int t5Timeout, Boolean refresh)
        {
            SECSAgent agent = bcfApplication.getSECSAgent(eapSecsAgentName);
            agent.setT5(t5Timeout);
            if (refresh)
            {
                agent.refreshConnection();
            }
        }

        /// <summary>
        /// Sets the secs agent t6 timeout.
        /// </summary>
        /// <param name="t6Timeout">The t6 timeout.</param>
        /// <param name="refresh">The refresh.</param>
        public void setSECSAgentT6Timeout(int t6Timeout, Boolean refresh)
        {
            SECSAgent agent = bcfApplication.getSECSAgent(eapSecsAgentName);
            agent.setT6(t6Timeout);
            if (refresh)
            {
                agent.refreshConnection();
            }
        }

        /// <summary>
        /// Sets the secs agent t7 timeout.
        /// </summary>
        /// <param name="t7Timeout">The t7 timeout.</param>
        /// <param name="refresh">The refresh.</param>
        public void setSECSAgentT7Timeout(int t7Timeout, Boolean refresh)
        {
            SECSAgent agent = bcfApplication.getSECSAgent(eapSecsAgentName);
            agent.setT7(t7Timeout);
            if (refresh)
            {
                agent.refreshConnection();
            }
        }

        /// <summary>
        /// Sets the secs agent t8 timeout.
        /// </summary>
        /// <param name="t8Timeout">The t8 timeout.</param>
        /// <param name="refresh">The refresh.</param>
        public void setSECSAgentT8Timeout(int t8Timeout, Boolean refresh)
        {
            SECSAgent agent = bcfApplication.getSECSAgent(eapSecsAgentName);
            agent.setT8(t8Timeout);
            if (refresh)
            {
                agent.refreshConnection();
            }
        }
        #endregion SECS Agent Parameter Change

        /// <summary>
        /// Initializes the background work.
        /// </summary>
        private void initBackgroundWork()
        {
            BackgroundWorkSample = new BackgroundWorkDriver(new BackgroundWorkSample());            //A0.03
        }
        private void initScheduler()
          {
            Scheduler = StdSchedulerFactory.GetDefaultScheduler();

            //IJobDetail zabbix_data_collection = JobBuilder.Create<ZabbixDataCollectionScheduler>().Build();
            IJobDetail mttf_mtbf_scheduler = JobBuilder.Create<MTTFAndMTBFScheduler>().Build();
            IJobDetail db_manatain_scheduler = JobBuilder.Create<DBManatainScheduler>().Build();
            //ITrigger zabbix_trigger = TriggerBuilder.Create()
            //       .WithIdentity("news", "TelegramGroup")
            //       .WithCronSchedule("0 0 0/1 * * ? ")//even 1 hour
            //       .StartAt(DateTime.UtcNow)
            //       .WithPriority(1)
            //       .Build();

            ITrigger three_min_trigger = TriggerBuilder.Create()
                   .WithIdentity("news1", "TelegramGroup")
                   .WithCronSchedule("0 0/3 * * * ? ")//even 5 min
                   .StartAt(DateTime.UtcNow)
                   .WithPriority(1)
                   .Build();
            ITrigger one_min_trigger = TriggerBuilder.Create()
                   .WithIdentity("news2", "TelegramGroup")
                   .WithCronSchedule("0 0/1 * * * ? ")//even 1 min
                   .StartAt(DateTime.UtcNow)
                   .WithPriority(1)
                   .Build();

            //Scheduler.ScheduleJob(zabbix_data_collection, zabbix_trigger);
            //Scheduler.ScheduleJob(mttf_mtbf_scheduler, three_min_trigger);
            Scheduler.ScheduleJob(db_manatain_scheduler, one_min_trigger);

        }

        /// <summary>
        /// Initializes the DAO.
        /// </summary>
        private void initDao()
        {
            lineDao = new LineDao();
            zoneDao = new ZoneDao();
            nodeDao = new NodeDao();
            eqptDao = new EqptDao();
            unitDao = new UnitDao();
            portDao = new PortDao();
            portStationDao = new PortStationDao();
            bufferPortDao = new BufferPortDao();
            flowRelDao = new FlowRelDao();
            userDao = new UserDao();
            functionCodeDao = new FunctionCodeDao();
            userFuncDao = new UserFuncDao();
            alarmDao = new AlarmDao();
            mainalarmDao = new MainAlarmDao();
            cassetteDao = new CassetteDao();
            bcStatusDao = new BCStatusDao();
            sequenceDao = new SequenceDao();
            eventRptCondDao = new EventRptCondDao();
            crateDao = new CrateDao();
            alarmRptCondDao = new AlarmRptCondDao();
            traceSetDao = new TraceSetDao();
            OperationHisDao = new OperationHisDao();    //A0.02
            userGroupDao = new UserGroupDao();
            alarmMapDao = new AlarmMapDao();
            ecDataMapDao = new ECDataMapDao();
            rptidDao = new RPTIDDao();
            ceidDao = new CEIDDao();
            ReturnCodeMapDao = new ReturnCodeMapDao();

            railDao = new RAILDao();
            addressDao = new ADDRESSDao();
            porticonDao = new Data.DAO.EntityFramework.PortIconDao();
            pointDao = new POINTDao();
            groupRailDao = new GROUPRAILSDao();
            sectionDao = new SectionDao(this);
            segmentDao = new SegmentDao();
            vehicleDao = new VehicleDao();

            cmd_ohtcDao = new CMD_OHTCDao();
            cmd_ohtc_detailDao = new CMD_OHTC_DetailDao();

            blockZoneMasterDao = new BlockZoneMasterDao();
            bolckZoneDetaiDao = new BlockZoneDetailDao();
            blockZoneQueueDao = new BlockZoneQueueDao();

            parkZoneDetailDao = new ParkZoneDetailDao();
            parkZoneMasterDao = new ParkZoneMasterDao();
            parkZoneTypeDao = new ParkZoneTypeDao();

            cyclezoneDdetailDao = new CycleZoneDetailDao();
            cyclezonemasterDao = new CycleZoneMasterDao();
            cyclezonetypeDao = new CycleZoneTypeDao();
            cmd_mcsDao = new CMD_MCSDao();
            vidinfoDao = new VIDINFODao();
            networkqualityDao = new NetworkQualityDao();
            sysexcutequalityDao = new SysExcuteQualityDao();
            mcsreportqueueDao = new MCSReportQueueDao();

            vSection100Dao = new ViewSectionDao();
            addressDataDao = new AddressDataDao();
            scaleBaseDataDao = new ScaleBaseDataDao();
            controlDataDao = new ControlDataDao();
            vehicleControlDao = new VehicleControlDao();
            dataCollectionDao = new DataCollectionDao();
            eqptLocationInfoDao = new EqptLocationInfoDao();
            apsettiongDao = new APSettingDao();

            hidzonemasterDao = new HIDZoneMasterDao();
            hidzonedetailDao = new HIDZoneDetailDao();
            hidzonequeueDao = new HIDZoneQueueDao();

            testtrantaskDao = new TestTranTaskDao();
            rserveEnhanceInfoDao = new ReserveEnhanceInfoDao();
            flexsimcommandDao = new FlexsimCommandDao();

            portdefDao = new PortDefDao();
            zonedefDao = new ZoneDefDao();
            shelfdefDao = new ShelfDefDao();
            cassettedataDao = new CassetteDataDao();

            hcmd_mcsDao = new HCMD_MCSDao();
            hcmd_ohtcDao = new HCMD_OHTCDao();
            flexsimcommandDao = new FlexsimCommandDao();
        }

        /// <summary>
        /// Initializes the configuration.
        /// </summary>
        private void initConfig()
        {
            logger.Info("init bc_Config");
            if (ohxcConfig == null)
            {
                ohxcConfig = new DataSet();
                logger.Info("new bc_Config");
                loadCSVToDataset(ohxcConfig, "ALARMMAP");
                loadCSVToDataset(ohxcConfig, "MAINALARM");
                loadCSVToDataset(ohxcConfig, "APSETTING");
                loadCSVToDataset(ohxcConfig, "RETURNCODEMAP");
                loadCSVToDataset(ohxcConfig, "EQPTLOCATIONINFO");
                loadCSVToDataset(ohxcConfig, "RESERVEENHANCEINFO");
                loadMapInfoCSVToDataset(ohxcConfig, "AADDRESS");
                loadMapInfoCSVToDataset(ohxcConfig, "ASECTION");
                logger.Info("init bc_Config success");
            }
            else
            {
                logger.Info("already init bc_Config");
            }
        }

        private void initialTransferCommandPeriodicDataSet()
        {
            string excelPath = Environment.CurrentDirectory + Path.Combine("\\config", BC_ID, "CSTTranSchedule.xlsx");

            loadExcel2DataTable(ref TranCmdPeriodicDataSet, excelPath);
        }

        /// <summary>
        /// Gets the CSV configuration path.
        /// </summary>
        /// <returns>System.String.</returns>
        public string getCsvConfigPath()
        {
            return this.getString("CsvConfig", "");
        }

        /// <summary>
        /// Loads the CSV to dataset.
        /// </summary>
        /// <param name="ds">The ds.</param>
        /// <param name="tableName">Name of the table.</param>
        private void loadCSVToDataset(DataSet ds, string tableName)
        {
            using (GenericParser parser = new GenericParser())
            {
                if (SCUtility.isMatche(tableName, "MAINALARM"))
                {
                    parser.SetDataSource(Environment.CurrentDirectory + @"\Config\" + tableName + ".csv", System.Text.Encoding.Default);
                }
                else
                {
                    parser.SetDataSource(Environment.CurrentDirectory + this.getString("CsvConfig", "") + tableName + ".csv", System.Text.Encoding.Default);
                }
                parser.ColumnDelimiter = ',';
                parser.FirstRowHasHeader = true;
                //parser.SkipStartingDataRows = 1;
                parser.MaxBufferSize = 1024;
                //parser.MaxRows = 500;
                //parser.TextQualifier = '\"';


                DataTable dt = new System.Data.DataTable(tableName);

                bool isfirst = true;
                while (parser.Read())
                {

                    int cs = parser.ColumnCount;
                    if (isfirst)
                    {

                        for (int i = 0; i < cs; i++)
                        {
                            dt.Columns.Add(parser.GetColumnName(i), typeof(string));
                        }
                        isfirst = false;
                    }


                    DataRow dr = dt.NewRow();

                    for (int i = 0; i < cs; i++)
                    {
                        string val = parser[i];
                        //ALARM 要可以接受 16進制的 2015.02.23 by Kevin Wei
                        //if (dt.Columns[i] != null && BCFUtility.isMatche(dt.Columns[i].ColumnName, "ALARM_ID"))
                        //{
                        //    int valInt = Convert.ToInt32(val);
                        //    val = val;
                        //}
                        dr[i] = val;
                        //                        dr[i] = parser[i];
                    }
                    dt.Rows.Add(dr);
                }
                ds.Tables.Add(dt);
            }
        }
        private void loadMapInfoCSVToDataset(DataSet ds, string tableName)
        {
            using (GenericParser parser = new GenericParser())
            {
                if (SCUtility.isMatche(tableName, "MAINALARM"))
                {
                    parser.SetDataSource(Environment.CurrentDirectory + @"\Config\MapInfo\" + tableName + ".csv", System.Text.Encoding.Default);
                }
                else
                {
                    parser.SetDataSource(Environment.CurrentDirectory + this.getString("CsvConfig", "") + @"MapInfo\" + tableName + ".csv", System.Text.Encoding.Default);
                }
                parser.ColumnDelimiter = ',';
                parser.FirstRowHasHeader = true;
                //parser.SkipStartingDataRows = 1;
                parser.MaxBufferSize = 1024;
                //parser.MaxRows = 500;
                //parser.TextQualifier = '\"';


                DataTable dt = new System.Data.DataTable(tableName);

                bool isfirst = true;
                while (parser.Read())
                {

                    int cs = parser.ColumnCount;
                    if (isfirst)
                    {

                        for (int i = 0; i < cs; i++)
                        {
                            dt.Columns.Add(parser.GetColumnName(i), typeof(string));
                        }
                        isfirst = false;
                    }


                    DataRow dr = dt.NewRow();

                    for (int i = 0; i < cs; i++)
                    {
                        string val = parser[i];
                        //ALARM 要可以接受 16進制的 2015.02.23 by Kevin Wei
                        //if (dt.Columns[i] != null && BCFUtility.isMatche(dt.Columns[i].ColumnName, "ALARM_ID"))
                        //{
                        //    int valInt = Convert.ToInt32(val);
                        //    val = val;
                        //}
                        dr[i] = val;
                        //                        dr[i] = parser[i];
                    }
                    dt.Rows.Add(dr);
                }
                ds.Tables.Add(dt);
            }
        }
        private void loadExcel2DataTable(ref DataSet dt, string filePath)
        {
            if (!File.Exists(filePath)) return;
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {

                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {

                    // Choose one of either 1 or 2:

                    // 1. Use the reader methods
                    //do
                    //{
                    //    while (reader.Read())
                    //    {
                    //        // reader.GetDouble(0);
                    //    }
                    //} while (reader.NextResult());

                    // 2. Use the AsDataSet extension method
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {

                        // Gets or sets a value indicating whether to set the DataColumn.DataType 
                        // property in a second pass.
                        UseColumnDataType = false,

                        // Gets or sets a callback to obtain configuration options for a DataTable. 
                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                        {

                            // Gets or sets a value indicating the prefix of generated column names.
                            EmptyColumnNamePrefix = "Column",

                            // Gets or sets a value indicating whether to use a row from the 
                            // data as column names.
                            UseHeaderRow = true,

                            // Gets or sets a callback to determine which row is the header row. 
                            // Only called when UseHeaderRow = true.
                            //ReadHeaderRow = (rowReader) =>
                            //{
                            //    // F.ex skip the first row and use the 2nd row as column headers:
                            //    rowReader.Read();
                            //}
                        }
                    });
                    dt = result;
                    // The result of each spreadsheet is in result.Tables
                }
            }
            //FileStream stream = System.IO.File.Open(@".\zn01.xlsx", FileMode.Open, FileAccess.Read);

            //IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            //DataSet ds = excelReader.AsDataSet();
            //excelReader.Close();
        }
        /// <summary>
        /// Initializes the BLL.
        /// </summary>
        private void initBLL()
        {
            bcSystemBLL = new BCSystemBLL();
            lineBLL = new LineBLL();
            alarmBLL = new AlarmBLL();
            sequenceBLL = new SequenceBLL();
            eventBLL = new EventBLL();
            reportBLL = new ReportBLL();
            userBLL = new UserBLL();

            mapBLL = new MapBLL();
            cmdBLL = new CMDBLL();
            parkBLL = new ParkBLL();
            cycleBLL = new CycleRunBLL();
            ceidBLL = new CEIDBLL();
            vidBLL = new VIDBLL();
            vehicleBLL = new VehicleBLL();
            networkqualityBLL = new NetworkQualityBLL();
            sysexcutequalityBLL = new SysExcuteQualityBLL();
            blockcontrolBLL = new BlockControlBLL();
            sectinoBLL = new SectionBLL();
            segmentBLL = new SegmentBLL();
            equipmentBLL = new EquipmentBLL();
            datasynBLL = new DataSyncBLL();

            guideBLL = new GuideBLL();

            hidBLL = new HIDBLL();

            CheckSystemEventHandler = new CheckSystemEventHandler();

            PortBLL = new PortBLL();
            PortStationBLL = new PortStationBLL();
            NodeBLL = new NodeBLL();

            PortDefBLL = new PortDefBLL();
            ZoneDefBLL = new ZoneDefBLL();
            ShelfDefBLL = new ShelfDefBLL();
            CassetteDataBLL = new CassetteDataBLL();
            ReserveBLL = new ReserveBLL(); //A0.01
        }


        public void initServer()
        {
            vehicleService = new VehicleService();
            lineService = new LineService();
            failOverService = new FailOverService();
            mtlService = new MTLService();
            ohcvService = new OHCVService();
            roadControlService = new RoadControlService();
            portStationService = new PortStationService();
            connectionInfoService = new ConnectionInfoService();
            userControlService = new UserControlService();
            transferService = new TransferService();
            blockControlService = new BlockControlService();
            shelfService = new ShelfService();
            emptyBoxHandlerService = new EmptyBoxHandlerService();
        }

        /// <summary>
        /// Starts the BLL.
        /// </summary>
        private void startBLL()
        {
            bcSystemBLL.start(this);
            lineBLL.start(this);
            alarmBLL.start(this);
            sequenceBLL.start(this);
            eventBLL.start(this);
            reportBLL.start(this);
            userBLL.start(this);

            mapBLL.start(this);
            cmdBLL.start(this);
            parkBLL.start(this);
            cycleBLL.start(this);
            ceidBLL.start(this);
            vidBLL.start(this);
            vehicleBLL.start(this);
            networkqualityBLL.start(this);
            sysexcutequalityBLL.start(this);
            blockcontrolBLL.start(this);
            datasynBLL.start(this);

            hidBLL.start(this);

            CheckSystemEventHandler.Start(this);

            PortBLL.start(this);
            PortStationBLL.start(this);
            SectionBLL.start(this);
            SegmentBLL.start(this);
            equipmentBLL.start(this);
            guideBLL.start(this);
            NodeBLL.start(this);

            PortDefBLL.start(this);
            ZoneDefBLL.start(this);
            ShelfDefBLL.start(this);
            CassetteDataBLL.start(this);
            ReserveBLL.start(this); //A0.01
        }

        private void startService()
        {
            vehicleService.Start(this);
            lineService.start(this);
            failOverService.start(this);
            mtlService.start(this);
            ohcvService.start(this);
            roadControlService.start(this);
            portStationService.start(this);
            connectionInfoService.start(this);
            userControlService.start(this);
            transferService.start(this);
            blockControlService.start(this);
            shelfService.start(this);
            emptyBoxHandlerService.start(this);
        }

        /// <summary>
        /// A0.01
        /// </summary>
        private void initWIF()
        {
            bcSystemWIF = new BCSystemWIF(this);
            lineWIF = new LineWIF(this);
        }

        /// <summary>
        /// Gets the BCF application.
        /// </summary>
        /// <returns>BCFApplication.</returns>
        public BCFApplication getBCFApplication()
        {
            return bcfApplication;
        }

        public void sendMQTTMessage()
        {
            mqttControl.MQTTPub(mqttTopic, mqttMsg);
        }



        /// <summary>
        /// 從AppSetting取得設定值，如果找不到該Key的設定值，將會回傳參數指定的預設值
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">預設值</param>
        /// <returns>Boolean.</returns>
        public Boolean getBoolean(string key, Boolean defaultValue)
        {
            Boolean rtn = defaultValue;
            try
            {
                string val = ConfigurationManager.AppSettings.Get(key);
                if (val != null)
                {
                    if (BCFUtility.isMatche(val, "Y"))
                    {
                        rtn = true;
                    }
                    else
                    {
                        rtn = false;
                    }
                }
                else
                {
                    return defaultValue;
                }
            }
            catch (Exception e)
            {
                logger.Warn("Get Config error[key:{0}][Exception:{1}]", key, e);
            }
            return rtn;
        }

        /// <summary>
        /// Gets the int.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Int32.</returns>
        private int getInt(string key, int defaultValue)
        {
            int rtn = defaultValue;
            try
            {
                rtn = Convert.ToInt32(ConfigurationManager.AppSettings.Get(key));
            }
            catch (Exception e)
            {
                logger.Warn("Get Config error[key:{0}][Exception:{1}]", key, e);
            }
            return rtn;
        }
        private double getDouble(string key, double defaultValue)
        {
            double rtn = defaultValue;
            try
            {
                string value = ConfigurationManager.AppSettings.Get(key);
                if (SCUtility.isEmpty(value))
                {
                    return defaultValue;
                }
                else
                {
                    rtn = Convert.ToDouble(value);
                }
            }
            catch (Exception e)
            {
                logger.Warn("Get Config error[key:{0}][Exception:{1}]", key, e);
            }
            return rtn;
        }
        /// <summary>
        /// Gets the long.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Int64.</returns>
        private long getLong(string key, long defaultValue)
        {
            long rtn = defaultValue;
            try
            {
                rtn = long.Parse(ConfigurationManager.AppSettings.Get(key));
            }
            catch (Exception e)
            {
                logger.Warn("Get Config error[key:{0}][Exception:{1}]", key, e);
            }
            return rtn;
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.String.</returns>
        private string getString(string key, string defaultValue)
        {
            string rtn = defaultValue;
            try
            {
                rtn = ConfigurationManager.AppSettings.Get(key);
                if (SCUtility.isEmpty(rtn))
                {
                    rtn = defaultValue;
                }
            }
            catch (Exception e)
            {
                logger.Warn("Get Config error[key:{0}][Exception:{1}]", key, e);
            }
            return rtn;
        }


        /// <summary>
        /// Gets the database connection.
        /// </summary>
        /// <returns>DBConnection.</returns>
        public DBConnection getDBConnection()
        {
            return getBCFApplication().getDBConnection();
        }

        /// <summary>
        /// Gets the database stateless connection.
        /// </summary>
        /// <returns>DBConnection.</returns>
        public DBConnection getDBStatelessConnection()
        {
            return getBCFApplication().getDBStatelessConnection();
        }

        /// <summary>
        /// The share memory initialize proc
        /// </summary>
        private IShareMemoryInitProcess shareMemInitProc;

        /// <summary>
        /// Injects the value definition map action.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="baseEQObject">The base eq object.</param>
        /// <exception cref="Exception"></exception>
        private void injectValueDefMapAction(BaseMapActionConfigElement config, ref BaseEQObject baseEQObject)
        {
            List<string> subMapActs = config.ValueDefMapActionClasses.Split(
                            new string[] { ";" },
                            StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (string mapAct in subMapActs)
            {
                try
                {
                    Type mapActType = Type.GetType(mapAct.Trim(), true);

                    IValueDefMapAction valMapAct =
                        (IValueDefMapAction)Activator.CreateInstance(mapActType);
                    baseEQObject.injectValueDefMapAction(valMapAct);
                }
                catch (Exception ex)
                {
                    logger.Warn(ex, String.Format("Not Found ValueDefMapAction Class : {0}", mapAct));
                    throw new Exception(String.Format("Not Found ValueDefMapAction Class : {0}", mapAct));
                }
            }
        }

        /// <summary>
        /// Determines whether this instance [can sel revert system].
        /// </summary>
        /// <returns>Boolean.</returns>
        /// <exception cref="Exception">Initial BC System Occur Error !!</exception>
        public Boolean canSelRevertSystem()
        {
            SCAppConstants.BCSystemInitialRtnCode rtnCode = application.bcSystemBLL.initialBCSystem();
            if (rtnCode == SCAppConstants.BCSystemInitialRtnCode.Error)
            {
                throw new Exception("Initial BC System Occur Error !!");
            }
            if (rtnCode == SCAppConstants.BCSystemInitialRtnCode.NonNormalShutdown
                /* && eqObjCacheManager.hasLineDataExist()*/)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 必須呼叫此method進行建立Equipment
        /// </summary>
        /// <param name="recoverFromDB">The recover from database.</param>
        public void startBuildEqpts(Boolean recoverFromDB)
        {

            eqObjCacheManager.start(/*eqptCss, nodeFlowRelCss, */recoverFromDB);      //啟動EQ Object Cache.. 將從DB取得Line資訊建立EQ Object
            commObjCacheManager.setPortDefsInfo();
            string shareMemoryInitClass = eqptCss.ShareMemoryInitClass;
            try
            {
                Type shareMemoryInitType = Type.GetType(shareMemoryInitClass.Trim(), true);
                shareMemInitProc =
                    (IShareMemoryInitProcess)Activator.CreateInstance(shareMemoryInitType);
            }
            catch (Exception ex)
            {
                logger.Warn(ex, "Not Found ShareMemoryInitProcess Class : {0}", shareMemoryInitClass);
            }

            foreach (var config in eqptCss.ConfigSections)
            {
                string line_id = config.Line_ID;
                BaseEQObject line = eqObjCacheManager.getLine();
                if (line == null || !BCFUtility.isMatche(line_id, (line as ALINE).LINE_ID))
                {
                    logger.Warn("MapActionDefs occur error: System Not Define Line[{0}]", line_id);
                    break;
                }
                injectValueDefMapAction(config, ref line);
                foreach (ZoneConfigSection zoneConfig in config.ZoneConfigList)
                {
                    string zone_id = zoneConfig.Zone_ID;
                    BaseEQObject zone = eqObjCacheManager.getZoneByZoneID(zone_id);
                    injectValueDefMapAction(zoneConfig, ref zone);
                    foreach (NodeConfigSection nodeConfig in zoneConfig.NodeConfigList)
                    {
                        string node_id = nodeConfig.Node_ID;
                        BaseEQObject node = eqObjCacheManager.getNodeByNodeID(node_id);
                        injectValueDefMapAction(nodeConfig, ref node);
                        foreach (EQPTConfigSection eqptConfig in nodeConfig.EQPTConfigList)
                        {
                            string eqpt_id = eqptConfig.EQPT_ID;
                            BaseEQObject eqpt = eqObjCacheManager.getEquipmentByEQPTID(eqpt_id);
                            injectValueDefMapAction(eqptConfig, ref eqpt);
                            foreach (UnitConfigSection unitConfig in eqptConfig.UnitConfigList)
                            {
                                string unit_id = unitConfig.Unit_ID;
                                BaseEQObject unit = eqObjCacheManager.getUnitByUnitID(unit_id);
                                injectValueDefMapAction(unitConfig, ref unit);
                            }
                            foreach (PortConfigSection portConfig in eqptConfig.PortConfigList)
                            {
                                string port_id = portConfig.Port_ID;
                                BaseEQObject port = eqObjCacheManager.getPortByPortID(port_id);
                                injectValueDefMapAction(portConfig, ref port);
                            }
                            foreach (BufferConfigSection buffConfig in eqptConfig.BuffConfigList)
                            {
                                string buff_id = buffConfig.Buff_ID;
                                BaseEQObject buff = eqObjCacheManager.getBuffByBuffID(buff_id);
                                injectValueDefMapAction(buffConfig, ref buff);
                            }
                            //BaseEQObject vh = eqObjCacheManager.getVehicletByVHID(eqpt_id);
                            //injectValueDefMapAction(eqptConfig, ref vh);
                        }
                        foreach (VehicleConfigSection vhConfig in nodeConfig.VehilceConfigList)
                        {
                            string vh_id = vhConfig.Vh_ID;
                            BaseEQObject eqpt = eqObjCacheManager.getVehicletByVHID(vh_id);
                            injectValueDefMapAction(vhConfig, ref eqpt);
                        }
                    }
                }
            }
            VehicleBLL.startMapAction();
            reportBLL.startMapAction();

            vehicleDao.start(eqObjCacheManager.getAllVehicle());
        }

        /// <summary>
        /// Gets the eq object cache manager.
        /// </summary>
        /// <returns>EQObjCacheManager.</returns>
        public EQObjCacheManager getEQObjCacheManager()
        {
            return eqObjCacheManager;
        }
        public CommObjCacheManager getCommObjCacheManager()
        {
            return commObjCacheManager;
        }

        public RedisCacheManager getRedisCacheManager()
        {
            return redisCacheManager;
        }

        public NatsManager getNatsManager()
        {
            return natsManager;
        }
        public ElasticSearchManager getElasticSearchManager()
        {
            return elasticSearchManager;
        }
        public Mirle.Hlts.ReserveSection.Map.ViewModels.HltMapViewModel getReserveSectionAPI()//A0.01
        {
            return reserveSectionAPI;
        }


        //A0.07 Begin
        /// <summary>
        /// 根據Equipment的腳本進行初始化
        /// </summary>
        private void initScriptForEquipment()
        {
            try
            {
                if (shareMemInitProc != null)
                {
                    shareMemInitProc.doInit();
                }
                ALINE line = eqObjCacheManager.getLine();
                line.Redis_Link_Stat = redisCacheManager.IsConnection ? SCAppConstants.LinkStatus.LinkOK : SCAppConstants.LinkStatus.LinkFail;

                //foreach (Line line in lineDic.Values) 
                //{
                foreach (BCFAppConstants.RUN_LEVEL runLevel in Enum.GetValues(typeof(BCFAppConstants.RUN_LEVEL)))
                {
                    //mainLine.doShareMemoryInit(runLevel);
                    line.doShareMemoryInit(runLevel);
                }
                //}

                alarmBLL.CheckSetAlarm();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                throw;
            }
        }

        //public Line getLine() 
        //{
        //    return mainLine;
        //}

        /// <summary>
        /// The started
        /// </summary>
        private Boolean started = false;
        /// <summary>
        /// 是否已啟動SC Timer
        /// </summary>
        /// <value>The started.</value>
        public Boolean Started { get { return started; } }

        /// <summary>
        /// Starts the process.
        /// </summary>
        private void startProcess()
        {
            initScriptForEquipment();
            startService();
            Scheduler.Start();

            //Scheduler.Start();
        }

        /// <summary>
        /// 啟動Share Memory，與指定MPLC通訊
        /// </summary>
        /// <param name="mplcName">Name of the MPLC.</param>
        public void startShareMemory(string mplcName)
        {
            lock (_lock)
            {
                if (started == false)
                    return;
                bcfApplication.startShareMemory(mplcName);
                logger.Info("Start Share Memory");
            }
        }

        /// <summary>
        /// 啟動Share Memory，與所有MPLC通訊
        /// </summary>
        public void startShareMemory()
        {
            lock (_lock)
            {
                if (started == false)
                    return;
                bcfApplication.startShareMemory();
                logger.Info("Start Share Memory");
            }
        }

        /// <summary>
        /// 啟動SECS Agent
        /// </summary>
        public void startSECSAgent()
        {
            lock (_lock)
            {
                if (started == false)
                    return;
                bcfApplication.startAllSECSAgent();
                logger.Info("Start SECS Agent");
            }
        }



        /// <summary>
        /// 啟動TcpIp Agent
        /// </summary>
        public void startTcpIpServerListen()
        {
            lock (_lock)
            {
                if (started == false)
                    return;
                //hAProxyConnectionTest.listen();
                bcfApplication.startTcpIpSecverListen();
                //logger.Info("Start TcpIp Agent");

                //if (FailOverService.isActive())
                //{
                //    hAProxyConnectionTest.listen();
                //}
                //else
                //{
                //    hAProxyConnectionTest.shutDown();
                //}
            }
        }

        /// <summary>
        /// 啟動指定的 TcpIpServer
        /// </summary>
        public void startTcpIpServerListen(int portNum)
        {
            lock (_lock)
            {
                if (started == false)
                    return;
                bcfApplication.startTcpIpSecverListen(portNum);
                logger.Info($"Start TcpIp Agent,Port Num:{portNum}");
            }
        }

        public void startHAProxyConnectionTest()
        {
            lock (_lock)
            {
                if (started == false)
                    return;
                hAProxyConnectionTest.listen();
            }
        }

        /// <summary>
        /// 啟動TcpIp Agent
        /// </summary>
        public void startTcpIpAgent()
        {
            lock (_lock)
            {
                if (started == false)
                    return;
                bcfApplication.startAllTcpIpAgent();
                logger.Info("Start TcpIp Agent");
            }
        }
        /// <summary>
        /// 開始執行
        /// </summary>
        public void start()
        {
            lock (_lock)
            {
                if (started == true)
                    return;
                bcfApplication.start(startProcess);

                NancyHost.Start();

                logger.Info("Start Application");
                started = true;
            }
        }

        /// <summary>
        /// 停止Share Memory，與指定MPLC停止通訊
        /// </summary>
        /// <param name="mplcName">Name of the MPLC.</param>
        public void stopShareMemory(string mplcName)
        {
            lock (_lock)
            {
                bcfApplication.stopShareMemory(mplcName);
                logger.Info("Stop Share Memory");
            }
        }

        /// <summary>
        /// 停止Share Memory，與所有MPLC停止通訊
        /// </summary>
        public void stopShareMemory()
        {
            lock (_lock)
            {
                bcfApplication.stopShareMemory();
                logger.Info("Stop Share Memory");
            }
        }

        /// <summary>
        /// 停止SECS Agent
        /// </summary>
        public void stopSECSAgent()
        {
            lock (_lock)
            {
                bcfApplication.stopAllSECSAgent();
                logger.Info("Stop SECS Agent");
            }
        }

        /// <summary>
        /// 停止TcpIp Agent
        /// </summary>
        public void stopTcpIpServer()
        {
            lock (_lock)
            {
                bcfApplication.ShutdownTcpIpSecverListen();
                logger.Info("Stop TcpIp Agent");
            }
        }
        /// <summary>
        /// 停止指定TcpIp Server
        /// </summary>
        public void stopTcpIpServer(int portNum)
        {
            lock (_lock)
            {
                bcfApplication.ShutdownTcpIpSecverListen(portNum);
                logger.Info($"Stop TcpIp Agent,Port Num:{portNum}");
            }
        }


        /// <summary>
        /// Stops the process.
        /// </summary>
        private void stopProcess()
        {
            //not implement
            Scheduler.Shutdown(false);

        }

        /// <summary>
        /// 停止執行
        /// </summary>
        public void stop()
        {
            lock (_lock)
            {
                if (started == false)
                    return;
                bcfApplication.stop(stopProcess);
                //RedisConnection.Close();
                NancyHost.Stop();
                natsManager.close();
                logger.Info("Stop Application");
                started = false;
            }
        }
        public void CloseRedisConnection()
        {
            redisCacheManager.CloseRedisConnection();
        }
        /// <summary>
        /// Gets the message string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.String.</returns>
        public static string getMessageString(string key, params object[] args)
        {
            return BCFApplication.getMessageString(key, args);
        }
    }

    /// <summary>
    /// Class SystemParameter.
    /// </summary>
    public class SystemParameter
    {
        //System EC Data 
        /// <summary>
        /// The secs conversaction timeout
        /// </summary>
        public static int SECSConversactionTimeout = 60;
        /// <summary>
        /// The initial control state
        /// </summary>
        public static string InitialControlState = SECSConst.HostCrtMode_EQ_Off_line;
        /// <summary>
        /// The control state keep time sec
        /// </summary>
        public static int ControlStateKeepTimeSec = 0;
        /// <summary>
        /// The heart beat sec
        /// </summary>
        public static int HeartBeatSec = 0;

        public static bool AutoTeching = false;
        public static double PassAxisDistance = 250;
        public static int PreStageWatingTime_ms = 3000;

        public static bool IsEnableIDReadFailScenario { private set; get; } = false;

        public static int cmdPriorityAdd = 1;
        public static int cmdTimeOutToAlternate = 30;
        /// <summary>
        /// Sets the secs conversaction timeout.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        public static void setSECSConversactionTimeout(int timeout)
        {
            SECSConversactionTimeout = timeout;
        }

        /// <summary>
        /// Sets the initial host mode.
        /// </summary>
        /// <param name="hostMode">The host mode.</param>
        public static void setInitialHostMode(string hostMode)
        {
            InitialControlState = hostMode;
        }

        /// <summary>
        /// Sets the control state keep time.
        /// </summary>
        /// <param name="keepTimeSec">The keep time sec.</param>
        public static void setControlStateKeepTime(int keepTimeSec)
        {
            ControlStateKeepTimeSec = keepTimeSec;
        }

        /// <summary>
        /// Sets the heart beat sec.
        /// </summary>
        /// <param name="heartBeatSec">The heart beat sec.</param>
        public static void setHeartBeatSec(int heartBeatSec)
        {
            HeartBeatSec = heartBeatSec;
        }

        /// <summary>
        /// Sets is enable id read fail secnario flag
        /// </summary>
        /// <param name="heartBeatSec">The heart beat sec.</param>
        public static void setIsEnableIDReadFailScenarioFlag(bool isEnable)
        {
            IsEnableIDReadFailScenario = isEnable;
        }
        public static void setPassAxisDistance(double passDistance)
        {
            PassAxisDistance = passDistance;
        }
        public static void setPreStageWatingTime_ms(int _preStageWatingTime_ms)
        {
            PreStageWatingTime_ms = _preStageWatingTime_ms;
        }

    }

    public class HAProxyConnectionTest
    {
        iibg3k0.ttc.Common.TCPIP.TcpIpServer tcpIpServer;
        Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public HAProxyConnectionTest(SCApplication app)
        {
            tcpIpServer = new iibg3k0.ttc.Common.TCPIP.TcpIpServer(6000, true, iibg3k0.ttc.Common.AppConstants.FrameBuilderType.PC_TYPE_MIRLE);
            tcpIpServer.SessionCreat += TcpIpServer_SessionCreat;
        }

        private void TcpIpServer_SessionCreat(object sender, object e)
        {
            try
            {
                string testString = "OK";
                Byte[] byteArray = new byte[testString.Length];
                for (int i = 0; i < testString.Length; i++)
                {
                    byteArray[i] = Convert.ToByte(testString[i]);
                }
                tcpIpServer.SendRawData(sender, byteArray);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                tcpIpServer.CloseSession(sender);
            }
        }

        public void shutDown()
        {
            tcpIpServer.Shutdown();
        }
        public void listen()
        {
            tcpIpServer.Listen();
        }

    }

    /// <summary>
    /// Class DebugParameter.
    /// </summary>
    public class DebugParameter
    {
        /// <summary> A0.02
        /// The is test ignore 136 unload complete and 132 command complete
        /// </summary>
        public static Boolean ignore136UnloadComplete = false;
        /// <summary>
        /// The is test eap mode
        /// </summary>
        public static Boolean IsTestEAPMode = false;
        /// <summary>
        /// The disable synchronize time
        /// </summary>
        public static Boolean DisableSyncTime = false;
        /// <summary>
        /// The is debug mode
        /// </summary>
        public static Boolean IsDebugMode = false;
        /// <summary>
        /// The reject eq cim on req
        /// </summary>
        public static Boolean RejectEQCimOnReq = false;
        /// <summary>
        /// The reject reply eq s1 f1
        /// </summary>
        public static Boolean RejectReplyEQS1F1 = false;
        /// <summary>
        /// The reject eap online
        /// </summary>
        public static Boolean RejectEAPOnline = false;

        public static Boolean TestDuplicate = false;

        public static Boolean CanAutoRandomGeneratesCommand = false;
        public static Boolean IsCycleRun = false;

        public static Boolean Is_136_empty_double_retry = false;
        public static Boolean Is_136_retry_test = false;

        public static CycleRunType cycleRunType;

        private static Boolean isforcedpassblockcontrol = false;
        public static Boolean isForcedPassBlockControl
        {
            set { isforcedpassblockcontrol = value; }
            get { return isforcedpassblockcontrol; }
        }
        public static Boolean isForcedRejectBlockControl = false;
        public static Boolean isTestCarrierInterfaceError = false;
        /// <summary>
        /// To reject or accept the reserve forced.
        /// </summary>
        public static Boolean isForcedPassReserve = false;
        public static Boolean isForcedRejectReserve = false;

        public enum CycleRunType
        {
            AGVStation,
            shelf,
            CV,
            NTB
        }

    }
}
