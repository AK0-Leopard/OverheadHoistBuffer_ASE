//*********************************************************************************
//      MESDefaultMapAction.cs
//*********************************************************************************
// File Name: MESDefaultMapAction.cs
// Description: 與EAP通訊的劇本
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2020/02/23    Kevin Wei      N/A            B0.01   功能增加，為了加入Cycle run時，需要額外去更新Carrier location
// 2020/02/27    Kevin Wei      N/A            B0.02   加入多個Reserve詢問的功能。
// 2020/04/17    Jason Wu       N/A            B0.03   加入Vehicle Abort, BCR Read Fail 與InterLock Error 做一次Alarm Set 與 Alarm Clear 以記錄在 MCS
// 2020/04/21    Jason Wu       N/A            B0.04   修改對OHT 31 cmd 之 load unload 命令路徑判定(主要是針對有原地取貨或原地放貨情況)
// 2020/05/04    Jason Wu       N/A            B0.05   新增BoxID更新，但尚未開啟，因為這部分OHT部分之回報要先有修正後才能開啟
// 2020/05/24    Jason Wu       N/A            B0.06   修改回報136 unload complete及 132 command complete 時會判定是否上報，shelf 上報，port 不上報。
// 2020/05/24    Jason Wu       N/A            B0.07   新增funtion "GetVehicleIDByPortID(string portID)" 讓上層能呼叫出目前port ID address 上的車輛ID
// 2020/05/27    Jason Wu       N/A            B0.08   新增funtion "GetVehicleDataByVehicleID(string vehicleID)" 讓上層能呼叫出目前vehicle ID 的vehicle cache 實時資料
// 2020/05/27    Jason Wu       N/A            B0.08.0 新增Task Run 在 132 命令完成之後，會處發TransferRun，使MCS命令可以在多車情形下早於趕車CMD下達。
// 2020/08/27    Kevin Wei      N/A            B0.09   修改選擇避車點邏輯。原本是找目前in mode的CV，改成固定找被標記的CV Port。(目前是固定設定在LOOP-T01、T0A)
// 2020/08/27    Kevin Wei      N/A            B0.10   修改針對OHT進行 data initial的時機。
//                                                     原:一有連線事件觸發，就直接進行
//                                                     改:在連線事件後且詢問143狀態成功更新後。
// 2020/09/05    Kevin Wei      N/A            B0.11   加入在命令結束時，如果沒有MCS命令要準備派送，將會讓他到停等點待命
//**********************************************************************************
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Data.SECS.ASE;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using Google.Protobuf.Collections;
using KingAOP;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using static com.mirle.ibg3k0.sc.App.SCAppConstants;

namespace com.mirle.ibg3k0.sc.Service
{
    public enum AlarmLst
    {
        OHT_INTERLOCK_ERROR = 100010,
        OHT_VEHICLE_ABORT = 100011,
        OHT_BCR_READ_FAIL = 100012,
        PORT_BOXID_READ_FAIL = 100013,
        PORT_CSTID_READ_FAIL = 100014,
        OHT_IDLE_HasCMD_TimeOut = 100015,
        OHT_QueueCmdTimeOut = 100016,
        AGV_HasCmdsAccessTimeOut = 100017,
        AGVStation_DontHaveEnoughEmptyBox = 100018,
        PORT_CIM_OFF = 100019,
        PORT_DOWN = 100020,
        BOX_NumberIsNotEnough = 100021,
        OHT_IDMismatchUNKU = 100022,
        LINE_NotEmptyShelf = 100023,
        PORT_OP_WaitOutTimeOut = 100024,
        PORT_BP1_WaitOutTimeOut = 100025,
        PORT_BP2_WaitOutTimeOut = 100026,
        PORT_BP3_WaitOutTimeOut = 100027,
        PORT_BP4_WaitOutTimeOut = 100028,
        PORT_BP5_WaitOutTimeOut = 100029,
        PORT_LP_WaitOutTimeOut = 100030,
    }
    public class VehicleService : IDynamicMetaObjectProvider
    {
        public const string DEVICE_NAME_OHx = "OHx";
        Logger logger = LogManager.GetCurrentClassLogger();
        TransferService transferService = null;
        SCApplication scApp = null;


        public VehicleService()
        {

        }
        public void Start(SCApplication app)
        {
            scApp = app;
            SubscriptionPositionChangeEvent();

            List<AVEHICLE> vhs = scApp.getEQObjCacheManager().getAllVehicle();

            foreach (var vh in vhs)
            {
                vh.addEventHandler(nameof(VehicleService), nameof(vh.isTcpIpConnect), PublishVhInfo);
                vh.addEventHandler(nameof(VehicleService), vh.VhPositionChangeEvent, PublishVhInfo);
                vh.addEventHandler(nameof(VehicleService), vh.VhExcuteCMDStatusChangeEvent, PublishVhInfo);
                vh.addEventHandler(nameof(VehicleService), vh.VhStatusChangeEvent, PublishVhInfo);
                vh.LocationChange += Vh_LocationChange;
                vh.SegmentChange += Vh_SegementChange;
                vh.AssignCommandFailOverTimes += Vh_AssignCommandFailOverTimes;
                vh.StatusRequestFailOverTimes += Vh_StatusRequestFailOverTimes;
                vh.LongTimeNoCommuncation += Vh_LongTimeNoCommuncation;
                vh.TimerActionStart();
            }

            transferService = app.TransferService;
        }


        private void Vh_AssignCommandFailOverTimes(object sender, int failTimes)
        {
            AVEHICLE vh = (sender as AVEHICLE);
            if (vh.MODE_STATUS == VHModeStatus.AutoRemote)
            {
                scApp.TransferService.OHBC_AlarmSet(vh.VEHICLE_ID,
                    SCAppConstants.SystemAlarmCode.OHT_Issue.RejectCommandAlarm);

                VehicleAutoModeCahnge(vh.VEHICLE_ID, VHModeStatus.AutoLocal);
                string message = $"vh:{vh.VEHICLE_ID}, assign command fail times:{failTimes}, change to auto local mode";
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: message,
                   VehicleID: vh.VEHICLE_ID,
                   CarrierID: vh.CST_ID);
                BCFApplication.onWarningMsg(message);
            }
        }

        private void Vh_LongTimeNoCommuncation(object sender, EventArgs e)
        {
            AVEHICLE vh = sender as AVEHICLE;
            if (vh == null) return;
            //當發生很久沒有通訊的時候，就會發送143去進行狀態的詢問，確保Control還與Vehicle連線著
            bool is_success = VehicleStatusRequest(vh.VEHICLE_ID);
            //如果連續三次 都沒有得到回覆時，就將Port關閉在重新打開
            if (!is_success)
            {
                vh.StatusRequestFailTimes++;
            }
            else
            {
                vh.StatusRequestFailTimes = 0;
            }
        }

        private void Vh_StatusRequestFailOverTimes(object sender, int e)
        {
            try
            {
                AVEHICLE vh = sender as AVEHICLE;
                vh.StatusRequestFailTimes = 0;
                //1.當Status要求失敗超過3次時，要將對應的Port關閉再開啟。
                //var endPoint = vh.getIPEndPoint(scApp.getBCFApplication());
                int port_num = vh.getPortNum(scApp.getBCFApplication());
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: $"Over {AVEHICLE.MAX_STATUS_REQUEST_FAIL_TIMES} times request status fail, begin restart tcpip server port:{port_num}...",
                   VehicleID: vh.VEHICLE_ID,
                   CarrierID: vh.CST_ID);

                stopVehicleTcpIpServer(vh);
                SpinWait.SpinUntil(() => false, 2000);
                startVehicleTcpIpServer(vh);
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: ex);
            }
        }


        public bool stopVehicleTcpIpServer(string vhID)
        {
            AVEHICLE vh = scApp.VehicleBLL.cache.getVhByID(vhID);
            return stopVehicleTcpIpServer(vh);
        }
        private bool stopVehicleTcpIpServer(AVEHICLE vh)
        {
            if (!vh.IsTcpIpListening(scApp.getBCFApplication()))
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: $"vh:{vh.VEHICLE_ID} of tcp/ip server already stopped!,IsTcpIpListening:{vh.IsTcpIpListening(scApp.getBCFApplication())}",
                   VehicleID: vh.VEHICLE_ID,
                   CarrierID: vh.CST_ID);
                return false;
            }

            int port_num = vh.getPortNum(scApp.getBCFApplication());
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
               Data: $"Stop vh:{vh.VEHICLE_ID} of tcp/ip server, port num:{port_num}",
               VehicleID: vh.VEHICLE_ID,
               CarrierID: vh.CST_ID);
            scApp.stopTcpIpServer(port_num);
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
               Data: $"Stop vh:{vh.VEHICLE_ID} of tcp/ip server finish, IsTcpIpListening:{vh.IsTcpIpListening(scApp.getBCFApplication())}",
               VehicleID: vh.VEHICLE_ID,
               CarrierID: vh.CST_ID);
            return true;
        }

        public bool startVehicleTcpIpServer(string vhID)
        {
            AVEHICLE vh = scApp.VehicleBLL.cache.getVhByID(vhID);
            return startVehicleTcpIpServer(vh);
        }

        private bool startVehicleTcpIpServer(AVEHICLE vh)
        {
            if (vh.IsTcpIpListening(scApp.getBCFApplication()))
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: $"vh:{vh.VEHICLE_ID} of tcp/ip server already listening!,IsTcpIpListening:{vh.IsTcpIpListening(scApp.getBCFApplication())}",
                   VehicleID: vh.VEHICLE_ID,
                   CarrierID: vh.CST_ID);
                return false;
            }

            int port_num = vh.getPortNum(scApp.getBCFApplication());
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
               Data: $"Start vh:{vh.VEHICLE_ID} of tcp/ip server, port num:{port_num}",
               VehicleID: vh.VEHICLE_ID,
               CarrierID: vh.CST_ID);
            scApp.startTcpIpServerListen(port_num);
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
               Data: $"Start vh:{vh.VEHICLE_ID} of tcp/ip server finish, IsTcpIpListening:{vh.IsTcpIpListening(scApp.getBCFApplication())}",
               VehicleID: vh.VEHICLE_ID,
               CarrierID: vh.CST_ID);
            return true;
        }



        private void Vh_LocationChange(object sender, LocationChangeEventArgs e)
        {
            AVEHICLE vh = sender as AVEHICLE;
            ASECTION leave_section = scApp.SectionBLL.cache.GetSection(e.LeaveSection);
            ASECTION entry_section = scApp.SectionBLL.cache.GetSection(e.EntrySection);
            leave_section?.Leave(vh.VEHICLE_ID);
            entry_section?.Entry(vh.VEHICLE_ID);

            if (scApp.getEQObjCacheManager().getLine().ServiceMode == AppServiceMode.Active)
                scApp.VehicleBLL.NetworkQualityTest(vh.VEHICLE_ID, e.EntrySection, vh.CUR_ADR_ID, 0);

            if (leave_section != null)
            {
                scApp.ReserveBLL.RemoveManyReservedSectionsByVIDSID(vh.VEHICLE_ID, leave_section.SEC_ID);
                scApp.CMDBLL.removePassSection(vh.VEHICLE_ID, leave_section.SEC_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: $"vh:{vh.VEHICLE_ID} leave section {leave_section.SEC_ID},remove reserved.",
                   VehicleID: vh.VEHICLE_ID);
            }
            //如果在進入該Section後，還有在該Section之前的Section沒有清掉的，就把它全部釋放
            if (entry_section != null)
            {
                List<string> current_resreve_section = scApp.ReserveBLL.loadCurrentReserveSections(vh.VEHICLE_ID);
                int current_section_index_in_reserve_section = current_resreve_section.IndexOf(entry_section.SEC_ID);
                if (current_section_index_in_reserve_section > 0)//代表不是在第一個
                {
                    for (int i = 0; i < current_section_index_in_reserve_section; i++)
                    {
                        scApp.ReserveBLL.RemoveManyReservedSectionsByVIDSID(vh.VEHICLE_ID, current_resreve_section[i]);
                        scApp.CMDBLL.removePassSection(vh.VEHICLE_ID, current_resreve_section[i]);
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                           Data: $"vh:{vh.VEHICLE_ID} force release omission section {current_resreve_section[i]},remove reserved.",
                           VehicleID: vh.VEHICLE_ID);
                    }
                }
            }


        }

        private void Vh_SegementChange(object sender, SegmentChangeEventArgs e)
        {
            AVEHICLE vh = sender as AVEHICLE;
            ASEGMENT leave_segment = scApp.SegmentBLL.cache.GetSegment(e.LeaveSegment);
            ASEGMENT entry_segment = scApp.SegmentBLL.cache.GetSegment(e.EntrySegment);
            leave_segment?.Leave(vh);
            entry_segment?.Entry(vh, scApp.SectionBLL, leave_segment == null);
        }

        private void PublishVhInfo(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                string vh_id = e.PropertyValue as string;
                AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(vh_id);
                if (sender == null) return;
                byte[] vh_Serialize = BLL.VehicleBLL.Convert2GPB_VehicleInfo(vh);
                RecoderVehicleObjInfoLog(vh_id, vh_Serialize);

                scApp.getNatsManager().PublishAsync
                    (string.Format(SCAppConstants.NATS_SUBJECT_VH_INFO_0, vh.VEHICLE_ID.Trim()), vh_Serialize);

                scApp.getRedisCacheManager().ListSetByIndexAsync
                    (SCAppConstants.REDIS_LIST_KEY_VEHICLES, vh.VEHICLE_ID, vh.ToString());
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
            //});
        }

        private static void RecoderVehicleObjInfoLog(string vh_id, byte[] arrayByte)
        {
            string compressStr = SCUtility.CompressArrayByte(arrayByte);
            dynamic logEntry = new JObject();
            logEntry.RPT_TIME = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture);
            logEntry.OBJECT_ID = vh_id;
            logEntry.RAWDATA = compressStr;
            logEntry.Index = "ObjectHistoricalInfo";
            var json = logEntry.ToString(Newtonsoft.Json.Formatting.None);
            json = json.Replace("RPT_TIME", "@timestamp");
            LogManager.GetLogger("ObjectHistoricalInfo").Info(json);
        }

        public static string CompressArrayByte(byte[] arrayByte)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);
            compressedzipStream.Write(arrayByte, 0, arrayByte.Length);
            compressedzipStream.Close();
            string compressStr = (string)(Convert.ToBase64String(ms.ToArray()));
            return compressStr;
        }

        public void SubscriptionPositionChangeEvent()
        {
            //scApp.VehicleBLL.loadAllAndProcPositionReportFromRedis();
            //scApp.getRedisCacheManager().SubscriptionEvent($"{SCAppConstants.REDIS_KEY_WORD_POSITION_REPORT}_*", scApp.VehicleBLL.VehiclePositionChangeHandler);
            scApp.getRedisCacheManager().SubscriptionEvent($"{SCAppConstants.REDIS_KEY_WORD_POSITION_REPORT}#*", scApp.VehicleBLL.VehiclePositionChangeHandler);
        }
        public void UnsubscribePositionChangeEvent()
        {
            //scApp.getRedisCacheManager().UnsubscribeEvent($"{SCAppConstants.REDIS_KEY_WORD_POSITION_REPORT}_*", scApp.VehicleBLL.VehiclePositionChangeHandler);
            scApp.getRedisCacheManager().UnsubscribeEvent($"{SCAppConstants.REDIS_KEY_WORD_POSITION_REPORT}#*", scApp.VehicleBLL.VehiclePositionChangeHandler);
        }

        #region Send Message To Vehicle
        #region Tcp/Ip
        public bool HostBasicVersionReport(string vh_id)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            DateTime crtTime = DateTime.Now;
            ID_101_HOST_BASIC_INFO_VERSION_RESPONSE receive_gpp = null;
            ID_1_HOST_BASIC_INFO_VERSION_REP sned_gpp = new ID_1_HOST_BASIC_INFO_VERSION_REP()
            {
                DataDateTimeYear = "2018",
                DataDateTimeMonth = "10",
                DataDateTimeDay = "25",
                DataDateTimeHour = "15",
                DataDateTimeMinute = "22",
                DataDateTimeSecond = "50",
                CurrentTimeYear = crtTime.Year.ToString(),
                CurrentTimeMonth = crtTime.Month.ToString(),
                CurrentTimeDay = crtTime.Day.ToString(),
                CurrentTimeHour = crtTime.Hour.ToString(),
                CurrentTimeMinute = crtTime.Minute.ToString(),
                CurrentTimeSecond = crtTime.Second.ToString()
            };
            isSuccess = vh.send_Str1(sned_gpp, out receive_gpp);
            isSuccess = isSuccess && receive_gpp.ReplyCode == 0;
            return isSuccess;
        }
        public bool BasicInfoReport(string vh_id)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            DateTime crtTime = DateTime.Now;
            ID_111_BASIC_INFO_RESPONSE receive_gpp = null;
            int travel_base_data_count = 1;
            int section_data_count = 0;
            int address_data_coune = 0;
            int scale_base_data_count = 1;
            int control_data_count = 1;
            int guide_base_data_count = 1;
            section_data_count = scApp.DataSyncBLL.getCount_ReleaseVSections();
            address_data_coune = scApp.MapBLL.getCount_AddressCount();
            ID_11_BASIC_INFO_REP sned_gpp = new ID_11_BASIC_INFO_REP()
            {
                TravelBasicDataCount = travel_base_data_count,
                SectionDataCount = section_data_count,
                AddressDataCount = address_data_coune,
                ScaleDataCount = scale_base_data_count,
                ContrlDataCount = control_data_count,
                GuideDataCount = guide_base_data_count
            };
            isSuccess = vh.sned_S11(sned_gpp, out receive_gpp);
            isSuccess = isSuccess && receive_gpp.ReplyCode == 0;
            return isSuccess;
        }
        public bool TavellingDataReport(string vh_id)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            DateTime crtTime = DateTime.Now;
            AVEHICLE_CONTROL_100 data = scApp.DataSyncBLL.getReleaseVehicleControlData_100(vh_id);

            ID_113_TAVELLING_DATA_RESPONSE receive_gpp = null;
            ID_13_TAVELLING_DATA_REP sned_gpp = new ID_13_TAVELLING_DATA_REP()
            {
                Resolution = (UInt32)data.TRAVEL_RESOLUTION,
                StartStopSpd = (UInt32)data.TRAVEL_START_STOP_SPEED,
                MaxSpeed = (UInt32)data.TRAVEL_MAX_SPD,
                AccelTime = (UInt32)data.TRAVEL_ACCEL_DECCEL_TIME,
                SCurveRate = (UInt16)data.TRAVEL_S_CURVE_RATE,
                OriginDir = (UInt16)data.TRAVEL_HOME_DIR,
                OriginSpd = (UInt32)data.TRAVEL_HOME_SPD,
                BeaemSpd = (UInt32)data.TRAVEL_KEEP_DIS_SPD,
                ManualHSpd = (UInt32)data.TRAVEL_MANUAL_HIGH_SPD,
                ManualLSpd = (UInt32)data.TRAVEL_MANUAL_LOW_SPD,
                TeachingSpd = (UInt32)data.TRAVEL_TEACHING_SPD,
                RotateDir = (UInt16)data.TRAVEL_TRAVEL_DIR,
                EncoderPole = (UInt16)data.TRAVEL_ENCODER_POLARITY,
                PositionCompensation = (UInt16)data.TRAVEL_F_DIR_LIMIT, //TODO 要填入正確的資料
                //FLimit = (UInt16)data.TRAVEL_F_DIR_LIMIT, //TODO 要填入正確的資料
                //RLimit = (UInt16)data.TRAVEL_R_DIR_LIMIT,
                KeepDistFar = (UInt32)data.TRAVEL_OBS_DETECT_LONG,
                KeepDistNear = (UInt32)data.TRAVEL_OBS_DETECT_SHORT,
            };
            isSuccess = vh.sned_S13(sned_gpp, out receive_gpp);
            isSuccess = isSuccess && receive_gpp.ReplyCode == 0;
            return isSuccess;
        }
        public bool SectionDataReport(string vh_id)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            DateTime crtTime = DateTime.Now;
            List<VSECTION_100> vSecs = scApp.DataSyncBLL.loadReleaseVSections();

            ID_15_SECTION_DATA_REP send_gpp = new ID_15_SECTION_DATA_REP();
            ID_115_SECTION_DATA_RESPONSE receive_gpp = null;
            foreach (VSECTION_100 vSec in vSecs)
            {
                var secInfo = new ID_15_SECTION_DATA_REP.Types.Section()
                {
                    DriveDir = (UInt16)vSec.DIRC_DRIV,
                    GuideDir = (UInt16)vSec.DIRC_GUID,
                    AeraSecsor = (UInt16)(UInt16)(vSec.AREA_SECSOR ?? 0),
                    SectionID = vSec.SEC_ID,
                    FromAddr = vSec.FROM_ADR_ID,
                    ToAddr = vSec.TO_ADR_ID,
                    ControlTable = convertvSec2ControlTable(vSec),
                    Speed = (UInt32)vSec.SEC_SPD,
                    Distance = (UInt32)vSec.SEC_DIS,
                    ChangeAreaSensor1 = (UInt16)vSec.CHG_AREA_SECSOR_1,
                    ChangeGuideDir1 = (UInt16)vSec.CDOG_1,
                    ChangeSegNum1 = vSec.CHG_SEG_NUM_1,

                    ChangeAreaSensor2 = (UInt16)vSec.CHG_AREA_SECSOR_2,
                    ChangeGuideDir2 = (UInt16)vSec.CDOG_2,
                    ChangeSegNum2 = vSec.CHG_SEG_NUM_2,
                    AtSegment = vSec.SEG_NUM
                };
                send_gpp.Sections.Add(secInfo);

            }
            isSuccess = vh.sned_S15(send_gpp, out receive_gpp);
            // isSuccess = isSuccess && receive_gpp.ReplyCode == 0;
            return isSuccess;
        }
        private UInt16 convertvSec2ControlTable(VSECTION_100 vSec)
        {
            System.Collections.BitArray bitArray = new System.Collections.BitArray(16);
            bitArray[0] = SCUtility.int2Bool(vSec.PRE_BLO_REQ);
            bitArray[1] = vSec.BRANCH_FLAG;
            bitArray[2] = vSec.HID_CONTROL;
            bitArray[3] = false;
            bitArray[4] = vSec.CAN_GUIDE_CHG;
            bitArray[6] = false;
            bitArray[7] = false;
            bitArray[8] = vSec.IS_ADR_RPT;
            bitArray[9] = false;
            bitArray[10] = false;
            bitArray[11] = false;
            bitArray[12] = SCUtility.int2Bool(vSec.RANGE_SENSOR_F);
            bitArray[13] = SCUtility.int2Bool(vSec.OBS_SENSOR_F);
            bitArray[14] = SCUtility.int2Bool(vSec.OBS_SENSOR_R);
            bitArray[15] = SCUtility.int2Bool(vSec.OBS_SENSOR_L);
            return SCUtility.getUInt16FromBitArray(bitArray);
        }

        public bool AddressDataReport(string vh_id)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            //List<AADDRESS_DATA> adrs = scApp.DataSyncBLL.loadReleaseADDRESS_DATAs(vh_id);
            List<AADDRESS_DATA> adrs = scApp.DataSyncBLL.loadReleaseADDRESS_DATAs(sc.BLL.DataSyncBLL.COMMON_ADDRESS_DATA_INDEX);
            List<string> hid_leave_adr = scApp.HIDBLL.loadAllHIDLeaveAdr();
            string rtnMsg = string.Empty;
            ID_17_ADDRESS_DATA_REP send_gpp = new ID_17_ADDRESS_DATA_REP();
            ID_117_ADDRESS_DATA_RESPONSE receive_gpp = null;
            foreach (AADDRESS_DATA adr in adrs)
            {
                var block_master = scApp.MapBLL.loadBZMByAdrID(adr.ADR_ID.Trim());
                var adrInfo = new ID_17_ADDRESS_DATA_REP.Types.Address()
                {
                    Addr = adr.ADR_ID,
                    Resolution = adr.RESOLUTION,
                    Loaction = adr.LOACTION,
                    BlockRelease = (block_master != null && block_master.Count > 0) ? 1 : 0,
                    HIDRelease = hid_leave_adr.Contains(adr.ADR_ID.Trim()) ? 1 : 0
                };
                send_gpp.Addresss.Add(adrInfo);
            }
            isSuccess = vh.sned_S17(send_gpp, out receive_gpp);
            // isSuccess = isSuccess && receive_gpp.ReplyCode == 0;
            return isSuccess;
        }

        public bool ScaleDataReport(string vh_id)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            SCALE_BASE_DATA data = scApp.DataSyncBLL.getReleaseSCALE_BASE_DATA();

            ID_119_SCALE_DATA_RESPONSE receive_gpp = null;
            ID_19_SCALE_DATA_REP sned_gpp = new ID_19_SCALE_DATA_REP()
            {
                Resolution = (UInt32)data.RESOLUTION,
                InposArea = (UInt32)data.INPOSITION_AREA,
                InposStability = (UInt32)data.INPOSITION_STABLE_TIME,
                ScalePulse = (UInt32)data.TOTAL_SCALE_PULSE,
                ScaleOffset = (UInt32)data.SCALE_OFFSET,
                ScaleReset = (UInt32)data.SCALE_RESE_DIST,
                ReadDir = (UInt16)data.READ_DIR

            };
            isSuccess = vh.sned_S19(sned_gpp, out receive_gpp);
            isSuccess = isSuccess && receive_gpp.ReplyCode == 0;
            return isSuccess;
        }

        public bool ControlDataReport(string vh_id)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);

            CONTROL_DATA data = scApp.DataSyncBLL.getReleaseCONTROL_DATA();
            string rtnMsg = string.Empty;
            ID_121_CONTROL_DATA_RESPONSE receive_gpp;
            ID_21_CONTROL_DATA_REP sned_gpp = new ID_21_CONTROL_DATA_REP()
            {
                TimeoutT1 = (UInt32)data.T1,
                TimeoutT2 = (UInt32)data.T2,
                TimeoutT3 = (UInt32)data.T3,
                TimeoutT4 = (UInt32)data.T4,
                TimeoutT5 = (UInt32)data.T5,
                TimeoutT6 = (UInt32)data.T6,
                TimeoutT7 = (UInt32)data.T7,
                TimeoutT8 = (UInt32)data.T8,
                TimeoutBlock = (UInt32)data.BLOCK_REQ_TIME_OUT
            };
            isSuccess = vh.sned_S21(sned_gpp, out receive_gpp);
            isSuccess = isSuccess && receive_gpp.ReplyCode == 0;
            return isSuccess;
        }

        public bool GuideDataReport(string vh_id)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            AVEHICLE_CONTROL_100 data = scApp.DataSyncBLL.getReleaseVehicleControlData_100(vh_id);
            ID_123_GUIDE_DATA_RESPONSE receive_gpp;
            ID_23_GUIDE_DATA_REP sned_gpp = new ID_23_GUIDE_DATA_REP()
            {
                StartStopSpd = (UInt32)data.GUIDE_START_STOP_SPEED,
                MaxSpeed = (UInt32)data.GUIDE_MAX_SPD,
                AccelTime = (UInt32)data.GUIDE_ACCEL_DECCEL_TIME,
                SCurveRate = (UInt16)data.GUIDE_S_CURVE_RATE,
                NormalSpd = (UInt32)data.GUIDE_RUN_SPD,
                ManualHSpd = (UInt32)data.GUIDE_MANUAL_HIGH_SPD,
                ManualLSpd = (UInt32)data.GUIDE_MANUAL_LOW_SPD,
                LFLockPos = (UInt32)data.GUIDE_LF_LOCK_POSITION,
                LBLockPos = (UInt32)data.GUIDE_LB_LOCK_POSITION,
                RFLockPos = (UInt32)data.GUIDE_RF_LOCK_POSITION,
                RBLockPos = (UInt32)data.GUIDE_RB_LOCK_POSITION,
                ChangeStabilityTime = (UInt32)data.GUIDE_CHG_STABLE_TIME,
            };
            isSuccess = vh.sned_S23(sned_gpp, out receive_gpp);
            isSuccess = isSuccess && receive_gpp.ReplyCode == 0;
            return isSuccess;
        }

        public bool doDataSysc(string vh_id)
        {
            bool isSyscCmp = false;
            DateTime ohtDataVersion = new DateTime(2017, 03, 27, 10, 30, 00);
            if (BasicInfoReport(vh_id) &&
                TavellingDataReport(vh_id) &&
                SectionDataReport(vh_id) &&
                AddressDataReport(vh_id) &&
                ScaleDataReport(vh_id) &&
                ControlDataReport(vh_id) &&
                GuideDataReport(vh_id))
            {
                isSyscCmp = true;
            }
            return isSyscCmp;
        }

        //public bool CSTIDRenameRequest(string vh_id, string new_cst_id)
        //{


        //}

        public bool IndividualUploadRequest(string vh_id)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            ID_161_INDIVIDUAL_UPLOAD_RESPONSE receive_gpp;
            ID_61_INDIVIDUAL_UPLOAD_REQ sned_gpp = new ID_61_INDIVIDUAL_UPLOAD_REQ()
            {

            };
            isSuccess = vh.sned_S61(sned_gpp, out receive_gpp);
            //TODO Set info 2 DB
            if (isSuccess)
            {

            }
            return isSuccess;
        }

        public bool IndividualChangeRequest(string vh_id)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            ID_163_INDIVIDUAL_CHANGE_RESPONSE receive_gpp;
            ID_63_INDIVIDUAL_CHANGE_REQ sned_gpp = new ID_63_INDIVIDUAL_CHANGE_REQ()
            {
                OffsetGuideFL = 1,
                OffsetGuideRL = 2,
                OffsetGuideFR = 3,
                OffsetGuideRR = 4
            };
            isSuccess = vh.sned_S63(sned_gpp, out receive_gpp);
            return isSuccess;
        }

        /// <summary>
        /// 與Vehicle進行資料同步。(通常使用剛與Vehicle連線時)
        /// </summary>
        /// <param name="vh_id"></param>
        public void VehicleInfoSynchronize(string vh_id)
        {
            /*與Vehicle進行狀態同步*/
            bool ask_status_success = VehicleStatusRequest(vh_id, true);
            if (ask_status_success)                                        //B0.10
            {                                                              //B0.10
                scApp.TransferService.iniOHTData(vh_id, "OHT_Connection"); //B0.10
            }                                                              //B0.10
            /*要求Vehicle進行Alarm的Reset，如果成功後會將OHxC上針對該Vh的Alarm清除*/
            if (AlarmResetRequest(vh_id))
            {
                //scApp.AlarmBLL.resetAllAlarmReport(vh_id);
                //scApp.AlarmBLL.resetAllAlarmReport2Redis(vh_id);
            }
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            //if (vh.MODE_STATUS == VHModeStatus.Manual &&
            //    !SCUtility.isEmpty(vh.CUR_ADR_ID) &&
            //    !SCUtility.isMatche(vh.CUR_ADR_ID, MTLService.MTL_ADDRESS))
            var check_is_in_maintain_device = scApp.EquipmentBLL.cache.IsInMaintainDevice(vh.CUR_ADR_ID);
            if (vh.MODE_STATUS == VHModeStatus.Manual &&
                !check_is_in_maintain_device.isIn)
            {
                ModeChangeRequest(vh_id, OperatingVHMode.OperatingAuto);
                if (SpinWait.SpinUntil(() => vh.MODE_STATUS == VHModeStatus.AutoRemote, 5000))
                {
                    ASEGMENT vh_current_seg_obj = scApp.SegmentBLL.cache.GetSegment(vh.CUR_SEG_ID);
                    vh_current_seg_obj?.Entry(vh, scApp.SectionBLL, true);
                }
            }
        }

        public bool VehicleStatusRequest(string vh_id, bool isSync = false)
        {
            bool isSuccess = false;
            try
            {

                string reason = string.Empty;
                AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
                ID_143_STATUS_RESPONSE receive_gpp;
                ID_43_STATUS_REQUEST send_gpp = new ID_43_STATUS_REQUEST()
                {
                    SystemTime = DateTime.Now.ToString(SCAppConstants.TimestampFormat_16)
                };
                SCUtility.RecodeReportInfo(vh.VEHICLE_ID, 0, send_gpp);
                isSuccess = vh.send_S43(send_gpp, out receive_gpp);
                SCUtility.RecodeReportInfo(vh.VEHICLE_ID, 0, receive_gpp, isSuccess.ToString());
                if (isSync && isSuccess)
                {
                    string current_adr_id = receive_gpp.CurrentAdrID;
                    VHModeStatus modeStat = DecideVhModeStatus(vh.VEHICLE_ID, current_adr_id, receive_gpp.ModeStatus);
                    VHActionStatus actionStat = receive_gpp.ActionStatus;
                    VhPowerStatus powerStat = receive_gpp.PowerStatus;
                    string cstID = receive_gpp.CSTID;
                    VhStopSingle obstacleStat = receive_gpp.ObstacleStatus;
                    VhStopSingle blockingStat = receive_gpp.BlockingStatus;
                    VhStopSingle pauseStat = receive_gpp.PauseStatus;
                    VhStopSingle hidStat = receive_gpp.HIDStatus;
                    VhStopSingle errorStat = receive_gpp.ErrorStatus;
                    VhLoadCarrierStatus loadCSTStatus = receive_gpp.HasCst;
                    VhLoadCarrierStatus loadBOXStatus = receive_gpp.HasBox;
                    if (loadBOXStatus == VhLoadCarrierStatus.Exist) //B0.05
                    {
                        vh.BOX_ID = receive_gpp.CarBoxID;
                    }
                    //VhGuideStatus leftGuideStat = recive_str.LeftGuideLockStatus;
                    //VhGuideStatus rightGuideStat = recive_str.RightGuideLockStatus;


                    int obstacleDIST = receive_gpp.ObstDistance;
                    string obstacleVhID = receive_gpp.ObstVehicleID;

                    scApp.VehicleBLL.setAndPublishPositionReportInfo2Redis(vh.VEHICLE_ID, receive_gpp);
                    scApp.VehicleBLL.getAndProcPositionReportFromRedis(vh.VEHICLE_ID);
                    // 0317 Jason 此部分之loadBOXStatus 原為loadCSTStatus ，現在之狀況為暫時解法
                    if (!scApp.VehicleBLL.doUpdateVehicleStatus(vh, cstID,
                                           modeStat, actionStat,
                                           blockingStat, pauseStat, obstacleStat, hidStat, errorStat, loadBOXStatus))
                    {
                        isSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
            return isSuccess;
        }

        public bool ModeChangeRequest(string vh_id, OperatingVHMode mode)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            ID_141_MODE_CHANGE_RESPONSE receive_gpp;
            ID_41_MODE_CHANGE_REQ sned_gpp = new ID_41_MODE_CHANGE_REQ()
            {
                OperatingVHMode = mode
            };
            SCUtility.RecodeReportInfo(vh_id, 0, sned_gpp);
            isSuccess = vh.sned_S41(sned_gpp, out receive_gpp);
            SCUtility.RecodeReportInfo(vh_id, 0, receive_gpp, isSuccess.ToString());
            return isSuccess;
        }

        public bool PowerOperatorRequest(string vh_id, OperatingPowerMode mode)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            ID_145_POWER_OPE_RESPONSE receive_gpp;
            ID_45_POWER_OPE_REQ sned_gpp = new ID_45_POWER_OPE_REQ()
            {
                OperatingPowerMode = mode
            };
            isSuccess = vh.sned_S45(sned_gpp, out receive_gpp);
            return isSuccess;
        }

        public bool AlarmResetRequest(string vh_id)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            ID_191_ALARM_RESET_RESPONSE receive_gpp;
            //scApp.TransferService.OHT_AlarmAllCleared(vh.VEHICLE_ID);
            ID_91_ALARM_RESET_REQUEST sned_gpp = new ID_91_ALARM_RESET_REQUEST()
            {

            };
            isSuccess = vh.sned_S91(sned_gpp, out receive_gpp);
            if (isSuccess)
            {
                isSuccess = receive_gpp?.ReplyCode == 0;
            }
            return isSuccess;
        }


        public bool PauseRequest(string vh_id, PauseEvent pause_event, OHxCPauseType ohxc_pause_type)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            PauseType pauseType = convert2PauseType(ohxc_pause_type);
            ID_139_PAUSE_RESPONSE receive_gpp;
            ID_39_PAUSE_REQUEST send_gpp = new ID_39_PAUSE_REQUEST()
            {
                PauseType = pauseType,
                EventType = pause_event
            };
            SCUtility.RecodeReportInfo(vh.VEHICLE_ID, 0, send_gpp);
            isSuccess = vh.sned_Str39(send_gpp, out receive_gpp);
            SCUtility.RecodeReportInfo(vh.VEHICLE_ID, 0, receive_gpp, isSuccess.ToString());
            return isSuccess;
        }
        public bool OHxCPauseRequest(string vh_id, PauseEvent pause_event, OHxCPauseType ohxc_pause_type)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            using (TransactionScope tx = SCUtility.getTransactionScope())
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {

                    switch (ohxc_pause_type)
                    {
                        case OHxCPauseType.Earthquake:
                            scApp.VehicleBLL.updateVehiclePauseStatus
                                (vh_id, earthquake_pause: pause_event == PauseEvent.Pause);
                            break;
                        case OHxCPauseType.Obstacle:
                            scApp.VehicleBLL.updateVehiclePauseStatus
                                (vh_id, obstruct_pause: pause_event == PauseEvent.Pause);
                            break;
                        case OHxCPauseType.Safty:
                            scApp.VehicleBLL.updateVehiclePauseStatus
                                (vh_id, safyte_pause: pause_event == PauseEvent.Pause);
                            break;
                    }
                    PauseType pauseType = convert2PauseType(ohxc_pause_type);
                    ID_139_PAUSE_RESPONSE receive_gpp;
                    ID_39_PAUSE_REQUEST send_gpp = new ID_39_PAUSE_REQUEST()
                    {
                        PauseType = pauseType,
                        EventType = pause_event
                    };
                    SCUtility.RecodeReportInfo(vh.VEHICLE_ID, 0, send_gpp);
                    isSuccess = vh.sned_Str39(send_gpp, out receive_gpp);
                    SCUtility.RecodeReportInfo(vh.VEHICLE_ID, 0, receive_gpp, isSuccess.ToString());

                    if (isSuccess)
                    {
                        tx.Complete();
                        vh.NotifyVhStatusChange();
                    }
                }
            }
            return isSuccess;
        }



        private PauseType convert2PauseType(OHxCPauseType ohxc_pauseType)
        {
            switch (ohxc_pauseType)
            {
                case OHxCPauseType.Normal:
                case OHxCPauseType.Obstacle:
                    return PauseType.OhxC;
                case OHxCPauseType.Block:
                    return PauseType.Block;
                case OHxCPauseType.Hid:
                    return PauseType.Hid;
                case OHxCPauseType.Earthquake:
                    return PauseType.EarthQuake;
                //case OHxCPauseType.Obstruct:
                //    return PauseType.;
                case OHxCPauseType.Safty:
                    return PauseType.Safety;
                case OHxCPauseType.ManualBlock:
                    return PauseType.ManualBlock;
                case OHxCPauseType.ManualHID:
                    return PauseType.ManualHid;
                case OHxCPauseType.ALL:
                    return PauseType.All;
                default:
                    throw new AggregateException($"enum arg not exist!value: {ohxc_pauseType}");
            }
        }

        public bool doSendOHxCCmdToVh(AVEHICLE assignVH, ACMD_OHTC cmd)
        {
            ActiveType activeType = default(ActiveType);
            string[] routeSections = null;
            string[] cycleRunSections = null;
            string[] minRouteSec_Vh2From = null;
            string[] minRouteSec_From2To = null;
            string[] minRouteAdr_Vh2From = null;
            string[] minRouteAdr_From2To = null;
            bool isSuccess = false;

            //嘗試規劃該筆ACMD_OHTC的搬送路徑
            if (scApp.CMDBLL.tryGenerateCmd_OHTC_Details(cmd, out activeType, out routeSections, out cycleRunSections
                                                                         , out minRouteSec_Vh2From, out minRouteSec_From2To
                                                                         , out minRouteAdr_Vh2From, out minRouteAdr_From2To))
            {
                if (activeType == ActiveType.Scan || activeType == ActiveType.Load || activeType == ActiveType.Loadunload)
                {
                    // B0.04 補上原地取貨狀態之說明
                    // B0.04 若取貨之section address 為空 (原地取貨) 則在該guide section 與 guide address 去補上該車目前之位置資訊(因為目前新架構OHT版本需要至少一段section 去判定
                    if (minRouteSec_Vh2From == null || minRouteAdr_Vh2From == null)
                    {
                        if (assignVH.CUR_SEC_ID != null && assignVH.CUR_ADR_ID != null)
                        {
                            minRouteSec_Vh2From = new string[] { assignVH.CUR_SEC_ID };
                            minRouteAdr_Vh2From = new string[] { assignVH.CUR_ADR_ID };
                        }
                        else
                        {
                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: string.Empty,
                               Data: $"can't generate command road data, something is null,id:{SCUtility.Trim(cmd.CMD_ID)},vh id:{SCUtility.Trim(cmd.VH_ID)} current status not allowed." +
                               $"assignVH.CUR_ADR_ID:{assignVH.CUR_ADR_ID }, assignVH.CUR_SEC_ID:{assignVH.CUR_SEC_ID } , current assign ohtc cmd id:{assignVH.OHTC_CMD}." +
                               $"assignVH.ACT_STATUS:{assignVH.ACT_STATUS}.");
                            return isSuccess;
                        }
                    }
                    // B0.04 補上 LoadUnload 原地放貨狀態之說明 與修改
                    // B0.04 若放貨之section address 為空 (原地放貨) 則在該guide section 與 guide address 去補上該車需要之資訊
                    if (activeType == ActiveType.Loadunload)
                    {
                        if (minRouteSec_From2To == null || minRouteAdr_From2To == null)
                        {
                            // B0.04 對該string array 補上要去 load 路徑資訊的最後一段address與 section 資料
                            minRouteSec_From2To = new string[] { minRouteSec_Vh2From[minRouteSec_Vh2From.Length - 1] };
                            minRouteAdr_From2To = new string[] { minRouteAdr_Vh2From[minRouteAdr_Vh2From.Length - 1] };
                        }
                    }
                }
                // B0.04 補上 Unload 原地放貨狀態之說明 與修改
                // B0.04 若放貨之section address 為空 (原地放貨) 則在該guide section 與 guide address 去補上該車需要之資訊
                if (activeType == ActiveType.Unload) //B0.04 若為單獨放貨命令，在該空值處補上該車當下之位置資訊。
                {
                    if (minRouteSec_From2To == null || minRouteAdr_From2To == null)
                    {
                        if (assignVH.CUR_SEC_ID != null && assignVH.CUR_ADR_ID != null)
                        {
                            minRouteSec_From2To = new string[] { assignVH.CUR_SEC_ID };
                            minRouteAdr_From2To = new string[] { assignVH.CUR_ADR_ID };
                        }
                        else
                        {
                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: string.Empty,
                               Data: $"can't generate command road data, something is null,id:{SCUtility.Trim(cmd.CMD_ID)},vh id:{SCUtility.Trim(cmd.VH_ID)} current status not allowed." +
                               $"assignVH.CUR_ADR_ID:{assignVH.CUR_ADR_ID }, assignVH.CUR_SEC_ID:{assignVH.CUR_SEC_ID } , current assign ohtc cmd id:{assignVH.OHTC_CMD}." +
                               $"assignVH.ACT_STATUS:{assignVH.ACT_STATUS}.");
                            return isSuccess;
                        }
                    }
                }

                //產生成功，則將該命令下達給車子，並更新車子執行命令的狀態
                isSuccess = sendTransferCommandToVh(cmd, assignVH, activeType, minRouteSec_Vh2From, minRouteSec_From2To, minRouteAdr_Vh2From, minRouteAdr_From2To);
                if (isSuccess)
                {
                    assignVH.VehicleAssign();
                    scApp.SysExcuteQualityBLL.updateSysExecQity_PassSecInfo(cmd.CMD_ID_MCS, assignVH.VEHICLE_ID, assignVH.CUR_SEC_ID,
                                            minRouteSec_Vh2From, minRouteSec_From2To);
                    scApp.CMDBLL.setVhExcuteCmdToShow(cmd, assignVH, routeSections,
                                                      minRouteSec_Vh2From.ToList(), minRouteSec_From2To.ToList(),
                                                      cycleRunSections);
                    assignVH.sw_speed.Restart();

                    //在設備確定接收該筆命令，把它從PreInitial改成Initial狀態並上報給MCS
                    if (!SCUtility.isEmpty(cmd.CMD_ID_MCS))
                    {
                        isSuccess &= scApp.CMDBLL.updateCMD_MCS_TranStatus2Initial(cmd.CMD_ID_MCS);
                        isSuccess &= scApp.ReportBLL.newReportTransferInitial(cmd.CMD_ID_MCS, null);
                    }
                }
                else
                {
                    //如果失敗了，則要將該筆命令更新回Queue，並且AbnormalEnd該筆命令
                    if (!SCUtility.isEmpty(cmd.CMD_ID_MCS))
                    {
                        scApp.CMDBLL.updateCMD_MCS_TranStatus2Queue(cmd.CMD_ID_MCS);
                    }
                    scApp.CMDBLL.updateCommand_OHTC_StatusByCmdID(cmd.CMD_ID, E_CMD_STATUS.AbnormalEndByOHT);
                }
            }
            return isSuccess;
        }

        public bool doSendOHxCOverrideCmdToVh(AVEHICLE assignVH, ACMD_OHTC cmd, bool isNeedPauseFirst)
        {
            ActiveType activeType = default(ActiveType);
            string[] routeSections = null;
            string[] cycleRunSections = null;
            string[] minRouteSeg_Vh2From = null;
            string[] minRouteSeg_From2To = null;
            bool isSuccess = false;

            throw new NotImplementedException();
            //如果失敗會將命令改成abonormal End
            //if (scApp.CMDBLL.tryGenerateCmd_OHTC_Details(cmd, out activeType, out routeSections, out cycleRunSections
            //                                                             , out minRouteSeg_Vh2From, out minRouteSeg_From2To))
            //{
            //    isSuccess = sendTransferCommandToVh(cmd, assignVH, ActiveType.Override, routeSections, cycleRunSections);

            //    if (isSuccess)
            //    {
            //        scApp.CMDBLL.setVhExcuteCmdToShow(cmd, assignVH, routeSections, cycleRunSections);
            //        if (isNeedPauseFirst)
            //            PauseRequest(assignVH.VEHICLE_ID, PauseEvent.Continue, OHxCPauseType.Normal);
            //        assignVH.sw_speed.Restart();
            //    }
            //    else
            //    {
            //    }
            //}
            return isSuccess;
        }

        public bool doCancelCommandByMCSCmdIDWithNoReport(string cancel_abort_mcs_cmd_id, CMDCancelType actType, out string ohtc_cmd_id)
        {
            ACMD_MCS mcs_cmd = scApp.CMDBLL.getCMD_MCSByID(cancel_abort_mcs_cmd_id);
            bool is_success = true;
            ohtc_cmd_id = string.Empty;
            switch (actType)
            {
                case CMDCancelType.CmdCancel:
                    //scApp.ReportBLL.newReportTransferCancelInitial(mcs_cmd, null);
                    if (mcs_cmd.TRANSFERSTATE == E_TRAN_STATUS.Queue)
                    {
                        return false;
                    }
                    else if (mcs_cmd.TRANSFERSTATE >= E_TRAN_STATUS.Queue && mcs_cmd.TRANSFERSTATE < E_TRAN_STATUS.Transferring)
                    {
                        AVEHICLE assign_vh = null;
                        assign_vh = scApp.VehicleBLL.getVehicleByExcuteMCS_CMD_ID(cancel_abort_mcs_cmd_id);
                        ohtc_cmd_id = assign_vh.OHTC_CMD;
                        is_success = doAbortCommand(assign_vh, ohtc_cmd_id, actType);
                        return is_success;
                    }
                    else if (mcs_cmd.TRANSFERSTATE >= E_TRAN_STATUS.Transferring) //當狀態變為Transferring時，即代表已經是Load complete
                    {
                        return false;
                    }
                    break;
                case CMDCancelType.CmdAbort:
                    //do nothing
                    break;
            }
            return is_success;
        }

        public (bool isSuccess, string result) ProcessVhCmdCancelAbortRequest(string vh_id)
        {
            bool is_success = false;
            string result = "";
            try
            {
                AVEHICLE assignVH = null;

                assignVH = scApp.VehicleBLL.getVehicleByID(vh_id);

                is_success = assignVH != null;

                if (is_success)
                {
                    string mcs_cmd_id = assignVH.MCS_CMD;
                    if (!string.IsNullOrWhiteSpace(mcs_cmd_id))
                    {
                        ACMD_MCS mcs_cmd = scApp.CMDBLL.getCMD_MCSByID(mcs_cmd_id);
                        if (mcs_cmd == null)
                        {
                            result = $"Can't find MCS command:[{mcs_cmd_id}] in database.";
                        }
                        else
                        {
                            CMDCancelType actType = default(CMDCancelType);
                            if (mcs_cmd.TRANSFERSTATE < sc.E_TRAN_STATUS.Transferring)
                            {
                                actType = CMDCancelType.CmdCancel;
                                is_success = scApp.VehicleService.doCancelOrAbortCommandByMCSCmdID(mcs_cmd_id, actType);
                                if (is_success) result = "OK";
                                else result = "Send command cancel/abort failed.";
                            }
                            else if (mcs_cmd.TRANSFERSTATE < sc.E_TRAN_STATUS.Canceling)
                            {
                                actType = CMDCancelType.CmdAbort;
                                is_success = scApp.VehicleService.doCancelOrAbortCommandByMCSCmdID(mcs_cmd_id, actType);
                                if (is_success) result = "OK";
                                else result = "Send command cancel/abort failed.";
                            }
                            else
                            {
                                result = $"MCS command:[{mcs_cmd_id}] can't excute cancel / abort,\r\ncurrent state:{mcs_cmd.TRANSFERSTATE}";
                            }
                        }
                    }
                    else
                    {
                        string ohtc_cmd_id = assignVH.OHTC_CMD;
                        if (string.IsNullOrWhiteSpace(ohtc_cmd_id))
                        {
                            result = $"Vehicle:[{vh_id}] do not have command.";
                        }
                        else
                        {
                            ACMD_OHTC ohtc_cmd = scApp.CMDBLL.getCMD_OHTCByID(ohtc_cmd_id);
                            if (ohtc_cmd == null)
                            {
                                result = $"Can't find vehicle command:[{ohtc_cmd_id}] in database.";
                            }
                            else
                            {
                                CMDCancelType actType = ohtc_cmd.CMD_STAUS >= E_CMD_STATUS.Execution ? CMDCancelType.CmdAbort : CMDCancelType.CmdCancel;
                                is_success = scApp.VehicleService.doAbortCommand(assignVH, ohtc_cmd_id, actType);
                                if (is_success)
                                {
                                    result = "OK";
                                }
                                else
                                {
                                    result = "Send vehicle status request failed.";
                                }
                            }
                        }
                    }
                }
                else
                {
                    result = $"Vehicle :[{vh_id}] not found!";
                }
            }
            catch (Exception ex)
            {
                is_success = false;
                result = "Execption happend!";
                logger.Error(ex, "Execption:");
            }
            return (is_success, result);
        }

        public bool doCancelOrAbortCommandByMCSCmdID(string cancel_abort_mcs_cmd_id, CMDCancelType actType)
        {
            ACMD_MCS mcs_cmd = scApp.CMDBLL.getCMD_MCSByID(cancel_abort_mcs_cmd_id);

            bool is_success = true;

            switch (actType)
            {
                case CMDCancelType.CmdCancel:
                    if (mcs_cmd.TRANSFERSTATE == E_TRAN_STATUS.Queue)
                    {
                        scApp.CMDBLL.updateCMD_MCS_TranStatus(cancel_abort_mcs_cmd_id, E_TRAN_STATUS.TransferCompleted);
                        scApp.ReportBLL.ReportTransferCancelInitial(cancel_abort_mcs_cmd_id);
                        scApp.ReportBLL.ReportTransferCancelCompleted(cancel_abort_mcs_cmd_id);
                    }
                    else
                    {
                        scApp.ReportBLL.newReportTransferCancelInitial(cancel_abort_mcs_cmd_id, null);

                        if (mcs_cmd.COMMANDSTATE >= ACMD_MCS.COMMAND_STATUS_BIT_INDEX_LOAD_ARRIVE)
                        {
                            scApp.ReportBLL.newReportTransferCancelFailed(cancel_abort_mcs_cmd_id, null);
                        }
                        else
                        {
                            AVEHICLE crane = scApp.VehicleBLL.getVehicleByID(mcs_cmd.CRANE.Trim());
                            if (crane.isTcpIpConnect)
                            {
                                is_success = scApp.VehicleService.cancleOrAbortCommandByMCSCmdID(cancel_abort_mcs_cmd_id, ProtocolFormat.OHTMessage.CMDCancelType.CmdCancel);

                                if (is_success)
                                {
                                    scApp.CMDBLL.updateCMD_MCS_TranStatus(cancel_abort_mcs_cmd_id, E_TRAN_STATUS.Canceling);
                                }
                                else
                                {
                                    scApp.ReportBLL.newReportTransferCancelFailed(cancel_abort_mcs_cmd_id, null);
                                }
                            }
                            else
                            {
                                scApp.TransferService.LocalCmdCancel(cancel_abort_mcs_cmd_id, "車子不在線上");
                            }
                        }
                    }
                    break;
                case CMDCancelType.CmdAbort:
                    bool localDelete = false;
                    string log = "對命令: " + cancel_abort_mcs_cmd_id + " 強制結束";

                    if (mcs_cmd.TRANSFERSTATE == E_TRAN_STATUS.Queue)
                    {
                        localDelete = true;
                        log = log + " mcs_cmd.TRANSFERSTATE:" + mcs_cmd.TRANSFERSTATE;
                    }
                    else
                    {
                        AVEHICLE crane = GetVehicleDataByVehicleID(mcs_cmd.CRANE.Trim());

                        if (crane.isTcpIpConnect)
                        {
                            if (crane.MCS_CMD.Trim() == mcs_cmd.CMD_ID.Trim())
                            {
                                is_success = cancleOrAbortCommandByMCSCmdID(cancel_abort_mcs_cmd_id, ProtocolFormat.OHTMessage.CMDCancelType.CmdAbort);
                                if (is_success)
                                {
                                    scApp.ReportBLL.ReportTransferAbortInitiated(cancel_abort_mcs_cmd_id);
                                    scApp.CMDBLL.updateCMD_MCS_TranStatus(cancel_abort_mcs_cmd_id, E_TRAN_STATUS.Aborting);
                                }
                                else
                                {
                                    scApp.ReportBLL.newReportTransferAbortFailed(cancel_abort_mcs_cmd_id, null);
                                }
                            }
                            else
                            {
                                log = log + " 命令ID不一樣";
                                localDelete = true;
                            }
                        }
                        else
                        {
                            log = log + " " + crane.VEHICLE_ID + " 連線狀態(isTcpIpConnect) : " + crane.isTcpIpConnect;
                            localDelete = true;
                        }
                    }

                    if (localDelete)
                    {
                        transferService.TransferServiceLogger.Info
                        (DateTime.Now.ToString("HH:mm:ss.fff ")
                            + log + "\n"
                            + transferService.GetCmdLog(mcs_cmd)
                        );

                        scApp.CMDBLL.updateCMD_MCS_TranStatus(cancel_abort_mcs_cmd_id, E_TRAN_STATUS.TransferCompleted);
                        scApp.ReportBLL.ReportTransferAbortInitiated(cancel_abort_mcs_cmd_id);
                        scApp.ReportBLL.ReportTransferAbortCompleted(cancel_abort_mcs_cmd_id);

                        //自動 Force finish cmd 可以加在這
                        Task.Run(() =>
                        {
                            scApp.CMDBLL.forceUpdataCmdStatus2FnishByVhID(mcs_cmd.CRANE.Trim()); // Force finish Cmd
                        });
                    }
                    break;
            }
            return is_success;
        }

        public bool doPriorityUpdateCommandByMCSCmdID(string update_mcs_cmd_id, string priority)
        {
            ACMD_MCS mcs_cmd = scApp.CMDBLL.getCMD_MCSByID(update_mcs_cmd_id);
            bool is_success = true;
            int pri = Convert.ToInt32(priority);
            if (mcs_cmd.TRANSFERSTATE == E_TRAN_STATUS.Queue)
            {
                scApp.CMDBLL.updateCMD_MCS_Priority(mcs_cmd, pri);
            }
            return is_success;
        }

        public bool cancleOrAbortCommandByMCSCmdID(string mcsCmdID, CMDCancelType actType)
        {
            scApp.TransferService.TransferServiceLogger.Info(
                DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> OHT | 對 OHT 下 MCS CmdID：" + mcsCmdID + " " + actType + " 動作");

            bool isSuccess = true;
            AVEHICLE assign_vh = null;
            try
            {
                assign_vh = scApp.VehicleBLL.getVehicleByExcuteMCS_CMD_ID(mcsCmdID);
                if (assign_vh == null)
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                       Data: $"command interrupt by mcs command id:{mcsCmdID} fail. current no vh in excute",
                       VehicleID: assign_vh?.VEHICLE_ID,
                       CarrierID: assign_vh?.CST_ID);
                    return false;
                }
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: $"command interrupt by mcs command id:{mcsCmdID},vh:{assign_vh.VEHICLE_ID},ohtc cmd:{assign_vh.OHTC_CMD},interrupt type:{actType}",
                   VehicleID: assign_vh?.VEHICLE_ID,
                   CarrierID: assign_vh?.CST_ID);

                string ohtc_cmd_id = SCUtility.Trim(assign_vh.OHTC_CMD);
                //A0.01 isSuccess = doAbortCommand(assign_vh, mcsCmdID, actType);
                isSuccess = doAbortCommand(assign_vh, ohtc_cmd_id, actType); //A0.01
            }
            catch (Exception ex)
            {
                isSuccess = false;
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: ex,
                   VehicleID: assign_vh?.VEHICLE_ID,
                   CarrierID: assign_vh?.CST_ID,
                   Details: $"abort command fail mcs command id:{mcsCmdID}");
            }

            scApp.TransferService.TransferServiceLogger.Info(
                DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> OHT | 對 OHT 下 MCS CmdID：" + mcsCmdID + " 回傳結果：" + isSuccess);

            return isSuccess;
        }
        public bool doInstallCommandByMCSCmdID(bool has_carrier, string carrier_id, string box_id, string carrier_loc)
        {
            bool is_success = true;
            CassetteData carrier = null;
            carrier = new CassetteData()
            {
                StockerID = "1",
                CSTID = carrier_id,
                BOXID = box_id,
                Carrier_LOC = carrier_loc,
                CSTState = E_CSTState.Installed,
                CSTInDT = DateTime.Now.ToString("yy/MM/dd HH:mm:ss"),
            };

            if (scApp.TransferService.isUnitType(carrier_loc, UnitType.SHELF))
            {
                carrier.CSTState = E_CSTState.Completed;
            }

            if (has_carrier)
            {
                is_success &= scApp.CassetteDataBLL.UpdateCSTDataByID(carrier_id, box_id, carrier_loc);
            }
            else
            {
                is_success &= scApp.CassetteDataBLL.insertCassetteData(carrier);
            }

            is_success &= scApp.ReportBLL.ReportCarrierInstallCompleted(carrier);
            return is_success;
        }

        public bool doRemoveCommandByMCSCmdID(bool has_carrier, string carrier_id, string box_id)
        {
            bool is_success = true;
            if (has_carrier)
            {
                is_success &= scApp.CassetteDataBLL.DeleteCSTDataByID(carrier_id, box_id);
            }
            else
            {
                is_success = false;
            }
            scApp.ReportBLL.ReportCarrierRemovedCompleted(carrier_id, box_id);
            return is_success;
        }

        public bool doChgEnableShelfCommand(string shelf_id, bool enable)
        {
            bool is_success = true;
            ShelfDef shelf = scApp.ShelfDefBLL.loadShelfDataByID(shelf_id);
            is_success &= scApp.ShelfDefBLL.UpdateEnableByID(shelf_id, enable);
            ZoneDef zone = scApp.ZoneDefBLL.loadZoneDataByID(shelf.ZoneID);
            scApp.ReportBLL.ReportShelfStatusChange(zone);
            return is_success;
        }

        public bool doAbortCommand(AVEHICLE assign_vh, string cmd_id, CMDCancelType actType)
        {
            return assign_vh.sned_Str37(cmd_id, actType);
        }

        private bool sendTransferCommandToVh(ACMD_OHTC cmd, AVEHICLE assignVH, ActiveType activeType, string[] minRouteSec_Vh2From,
                                            string[] minRouteSec_From2To, string[] minRouteAdr_Vh2From, string[] minRouteAdr_From2To)
        {
            bool isSuccess = true;
            string cmd_id = cmd.CMD_ID;
            string vh_id = cmd.VH_ID;
            try
            {
                List<AMCSREPORTQUEUE> reportqueues = new List<AMCSREPORTQUEUE>();
                using (var tx = SCUtility.getTransactionScope())
                {
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        //if (assignVH.ACT_STATUS == VHActionStatus.NoCommand)
                        //{
                        isSuccess &= scApp.CMDBLL.updateCommand_OHTC_StatusByCmdID(cmd.CMD_ID, E_CMD_STATUS.Execution);
                        if (activeType != ActiveType.Override)
                        {
                            isSuccess &= scApp.VehicleBLL.updateVehicleExcuteCMD(cmd.VH_ID, cmd.CMD_ID, cmd.CMD_ID_MCS);
                            if (!SCUtility.isEmpty(cmd.CMD_ID_MCS))
                            {
                                //isSuccess &= scApp.VIDBLL.upDateVIDCommandInfo(cmd.VH_ID, cmd.CMD_ID_MCS);
                                isSuccess &= scApp.ReportBLL.newReportBeginTransfer(assignVH.VEHICLE_ID, reportqueues);
                                scApp.ReportBLL.insertMCSReport(reportqueues);
                            }
                        }

                        if (isSuccess)
                        {
                            isSuccess &= TransferRequset
                                (cmd.VH_ID, cmd.CMD_ID, cmd.CMD_ID_MCS, activeType, cmd.CARRIER_ID, cmd.BOX_ID, cmd.LOT_ID
                                , minRouteSec_Vh2From, minRouteSec_From2To, minRouteAdr_Vh2From, minRouteAdr_From2To
                                , cmd.SOURCE, cmd.DESTINATION, cmd.SOURCE_ADR, cmd.DESTINATION_ADR);
                        }
                        if (isSuccess)
                        {
                            tx.Complete();
                        }
                        //}
                        //else
                        //{
                        //    isSuccess = false;
                        //}
                    }
                }
                if (isSuccess)
                {
                    // This function may cause exception if the cmd is not come from MCS or there don't have a MCS cmd.
                    scApp.TransferService.OHT_TransferStatus(cmd_id, vh_id, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_ENROUTE);
                    if (assignVH.HAS_CST == 1)
                    {
                        //scApp.TransferService.OHT_TransferStatus(cmd_id, vh_id, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE);
                    }
                }


                if (isSuccess)
                {
                    scApp.ReportBLL.newSendMCSMessage(reportqueues);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
                isSuccess = false;
            }
            return isSuccess;
        }


        public bool TransferRequset(string vh_id, string cmd_id, string mcs_cmd_id, ActiveType activeType, string cst_id, string box_id, string lot_id,
            string[] minRouteSec_Vh2From, string[] minRouteSec_From2To, string[] minRouteAdr_Vh2From, string[] minRouteAdr_From2To,
            string fromPort_id, string toPort_id, string fromAdr, string toAdr)
        {
            bool isSuccess = false;
            string reason = string.Empty;
            ID_131_TRANS_RESPONSE receive_gpp = null;
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            isSuccess = TransferCommandCheck(activeType, cst_id, minRouteSec_Vh2From, minRouteSec_From2To, minRouteAdr_Vh2From, minRouteAdr_From2To, fromAdr, toAdr, out reason);
            if (isSuccess)
            {
                ID_31_TRANS_REQUEST send_gpb = new ID_31_TRANS_REQUEST()
                {
                    CmdID = cmd_id,
                    ActType = activeType,
                    CSTID = cst_id ?? string.Empty,
                    BOXID = box_id ?? string.Empty,
                    LOTID = lot_id ?? string.Empty,
                    LoadPortID = fromPort_id,
                    UnloadPortID = toPort_id,
                    LoadAdr = fromAdr,
                    ToAdr = toAdr
                };
                if (minRouteSec_Vh2From != null)
                    send_gpb.GuideSectionsStartToLoad.AddRange(minRouteSec_Vh2From);
                if (minRouteSec_From2To != null)
                    send_gpb.GuideSectionsToDestination.AddRange(minRouteSec_From2To);
                if (minRouteAdr_Vh2From != null)
                    send_gpb.GuideAddressStartToLoad.AddRange(minRouteAdr_Vh2From);
                if (minRouteAdr_From2To != null)
                    send_gpb.GuideAddressToDestination.AddRange(minRouteAdr_From2To);
                SCUtility.RecodeReportInfo(vh.VEHICLE_ID, 0, send_gpb);
                isSuccess = vh.sned_Str31(send_gpb, out receive_gpp, out reason);
                SCUtility.RecodeReportInfo(vh.VEHICLE_ID, 0, receive_gpp, isSuccess.ToString());
            }
            if (isSuccess)
            {
                int reply_code = receive_gpp.ReplyCode;
                if (reply_code != 0)
                {
                    isSuccess = false;
                    var return_code_map = scApp.CMDBLL.getReturnCodeMap(vh.NODE_ID, reply_code.ToString());
                    if (return_code_map != null)
                        reason = return_code_map.DESC;
                    bcf.App.BCFApplication.onWarningMsg(string.Format("發送命令失敗,VH ID:{0}, CMD ID:{1}, Reason:{2}",
                                                              vh_id,
                                                              cmd_id,
                                                              reason));
                }
                else
                {
                    // This function may cause exception if the cmd is not come from MCS or there don't have a MCS cmd.
                    //scApp.TransferService.OHT_TransferStatus(cmd_id, vh_id, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_ENROUTE);
                    //if (vh.HAS_CST == 1)
                    //{
                    //    scApp.TransferService.OHT_TransferStatus(cmd_id, vh_id, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE);
                    //}

                    //
                }
            }
            else
            {
                bcf.App.BCFApplication.onWarningMsg(string.Format("發送命令失敗,VH ID:{0}, CMD ID:{1}, Reason:{2}",
                                          vh_id,
                                          cmd_id,
                                          reason));
                VehicleStatusRequest(vh_id, true);
            }

            return isSuccess;
        }

        public bool CarrierIDRenameRequset(string vh_id, string oldCarrierID, string newCarrierID)
        {
            bool isSuccess = true;

            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            ID_135_CARRIER_ID_RENAME_RESPONSE receive_gpp;
            ID_35_CARRIER_ID_RENAME_REQUEST send_gpp = new ID_35_CARRIER_ID_RENAME_REQUEST()
            {
                OLDCSTID = oldCarrierID ?? string.Empty,
                NEWCSTID = newCarrierID ?? string.Empty,
            };
            SCUtility.RecodeReportInfo(vh.VEHICLE_ID, 0, send_gpp);
            isSuccess = vh.sned_Str35(send_gpp, out receive_gpp);
            SCUtility.RecodeReportInfo(vh.VEHICLE_ID, 0, receive_gpp, isSuccess.ToString());
            return isSuccess;
        }

        private bool TransferCommandCheck(ActiveType activeType, string cst_id,
                                        string[] minRouteSec_Vh2From, string[] minRouteSec_From2To, string[] minRouteAdr_Vh2From, string[] minRouteAdr_From2To,
                                        string fromAdr, string toAdr, out string reason)
        {
            reason = "";
            if (activeType == ActiveType.Home || activeType == ActiveType.Mtlhome)
            {
                return true;
            }

            if (activeType == ActiveType.Load || activeType == ActiveType.Unload ||
                (activeType == ActiveType.Loadunload && SCUtility.isMatche(fromAdr, toAdr)))
            {
                //not thing...
            }
            else
            {
                if (minRouteSec_Vh2From == null || minRouteSec_Vh2From.Count() == 0)
                {   //For Test Bypass 2020/01/12
                    //reason = "Pass section is empty !";
                    //return false;
                    return true;
                }
            }

            bool isOK = true;
            switch (activeType)
            {
                case ActiveType.Load:
                    if (SCUtility.isEmpty(fromAdr))
                    {
                        isOK = false;
                        reason = $"Transfer type[{activeType},from adr is empty!]";
                    }
                    break;
                case ActiveType.Unload:
                    if (SCUtility.isEmpty(toAdr))
                    {
                        isOK = false;
                        reason = $"Transfer type[{activeType},from adr is empty!]";
                    }
                    break;
                case ActiveType.Loadunload:
                    if (SCUtility.isEmpty(fromAdr))
                    {
                        isOK = false;
                        reason = $"Transfer type[{activeType},from adr is empty!]";
                    }
                    else if (SCUtility.isEmpty(toAdr))
                    {
                        isOK = false;
                        reason = $"Transfer type[{activeType},toAdr adr is empty!]";
                    }
                    break;

            }

            return isOK;
        }

        public bool TeachingRequest(string vh_id, string from_adr, string to_adr)
        {
            bool isSuccess = false;
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            ID_171_RANGE_TEACHING_RESPONSE receive_gpp;
            ID_71_RANGE_TEACHING_REQUEST send_gpp = new ID_71_RANGE_TEACHING_REQUEST()
            {
                FromAdr = from_adr,
                ToAdr = to_adr
            };

            SCUtility.RecodeReportInfo(vh.VEHICLE_ID, 0, send_gpp);
            isSuccess = vh.send_Str71(send_gpp, out receive_gpp);
            SCUtility.RecodeReportInfo(vh.VEHICLE_ID, 0, receive_gpp, isSuccess.ToString());

            return isSuccess;
        }


        #endregion Tcp/Ip
        #region PLC
        public void PLC_Control_TrunOn(string vh_id)
        {
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            vh.PLC_Control_TrunOn();
        }
        public void PLC_Control_TrunOff(string vh_id)
        {
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            vh.PLC_Control_TrunOff();
        }


        public bool SetVehicleControlItemForPLC(string vh_id, Boolean[] items)
        {
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            return vh.setVehicleControlItemForPLC(items);
        }
        #endregion PLC

        #endregion Send Message To Vehicle

        #region Position Report
        [ClassAOPAspect]
        public void PositionReport(BCFApplication bcfApp, AVEHICLE eqpt, ID_134_TRANS_EVENT_REP recive_str)
        {
            if (scApp.getEQObjCacheManager().getLine().ServerPreStop)
                return;
            //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
            //   seq_num: 0, //由於Position Report的資料可能從很多地方來，例如143、144、PLC、136 因此在此先不考慮其seq_num
            //   Data: recive_str,
            //   VehicleID: eqpt.VEHICLE_ID,
            //   CarrierID: eqpt.CST_ID);
            SCUtility.RecodeReportInfo(eqpt.VEHICLE_ID, 0, recive_str);
            EventType eventType = recive_str.EventType;
            string current_adr_id = SCUtility.isEmpty(recive_str.CurrentAdrID) ? string.Empty : recive_str.CurrentAdrID;
            string current_sec_id = SCUtility.isEmpty(recive_str.CurrentSecID) ? string.Empty : recive_str.CurrentSecID;
            ASECTION current_sec = scApp.SectionBLL.cache.GetSection(current_sec_id);
            string current_seg_id = current_sec == null ? string.Empty : current_sec.SEG_NUM;

            string last_adr_id = eqpt.CUR_ADR_ID;
            string last_sec_id = eqpt.CUR_SEC_ID;
            ASECTION lase_sec = scApp.SectionBLL.cache.GetSection(last_sec_id);
            string last_seg_id = lase_sec == null ? string.Empty : lase_sec.SEG_NUM;
            uint sec_dis = recive_str.SecDistance;
            double x_axis = recive_str.XAxis;
            double y_axis = recive_str.YAxis;
            //double vh_angle = recive_str.Angle;
            double vh_angle = current_sec.PADDING;
            double speed = recive_str.Speed;

            doUpdateVheiclePositionAndCmdSchedule
                (eqpt, current_adr_id, current_sec_id, current_seg_id,
                       last_adr_id, last_sec_id, last_seg_id,
                       sec_dis, x_axis, y_axis, vh_angle, speed);

            //switch (eventType)
            //{
            //    case EventType.AdrPass:
            //    case EventType.AdrOrMoveArrivals:
            //        PositionReport_AdrPassArrivals(bcfApp, eqpt, recive_str, last_adr_id, last_sec_id);
            //        break;
            //}
        }
        [ClassAOPAspect]
        public void PositionReport_100(BCFApplication bcfApp, AVEHICLE eqpt, ID_134_TRANS_EVENT_REP recive_str, int seq_num)
        {
            if (scApp.getEQObjCacheManager().getLine().ServerPreStop)
                return;

            SCUtility.RecodeReportInfo(eqpt.VEHICLE_ID, seq_num, recive_str);
            EventType eventType = recive_str.EventType;
            string current_adr_id = recive_str.CurrentAdrID;
            string current_sec_id = recive_str.CurrentSecID;
            ASECTION current_sec = scApp.SectionBLL.cache.GetSection(current_sec_id);
            string current_seg_id = current_sec == null ? string.Empty : current_sec.SEG_NUM;

            string last_adr_id = eqpt.CUR_ADR_ID;
            string last_sec_id = eqpt.CUR_SEC_ID;
            ASECTION lase_sec = scApp.SectionBLL.cache.GetSection(last_sec_id);
            string last_seg_id = lase_sec == null ? string.Empty : lase_sec.SEG_NUM;
            uint sec_dis = recive_str.SecDistance;
            double x_axis = recive_str.XAxis;
            double y_axis = recive_str.YAxis;
            double vh_angle = recive_str.Angle;
            double speed = recive_str.Speed;

            if (eventType == EventType.AdrOrMoveArrivals)
            {
                List<string> adrs = new List<string>();
                adrs.Add(SCUtility.Trim(current_adr_id));
                List<ASECTION> Secs = scApp.MapBLL.loadSectionByToAdrs(adrs);
                if (Secs.Count > 0)
                {
                    current_sec_id = Secs[0].SEC_ID.Trim();
                }
            }
            doUpdateVheiclePositionAndCmdSchedule
                (eqpt, current_adr_id, current_sec_id, current_seg_id,
                       last_adr_id, last_sec_id, last_seg_id,
                       sec_dis, x_axis, y_axis, vh_angle, speed);

            switch (eventType)
            {
                case EventType.AdrPass:
                case EventType.AdrOrMoveArrivals:
                    PositionReport_AdrPassArrivals(bcfApp, eqpt, recive_str, last_adr_id, last_sec_id);
                    break;
            }
        }

        public void doUpdateVheiclePositionAndCmdSchedule(AVEHICLE vh,
            string current_adr_id, string current_sec_id, string current_seg_id,
            string last_adr_id, string last_sec_id, string last_seg_id,
            uint sec_dis, double x_axis, double y_axis, double vh_angle, double speed)
        {
            lock (vh.PositionRefresh_Sync)
            {
                ALINE line = scApp.getEQObjCacheManager().getLine();
                scApp.VehicleBLL.updateVheiclePosition_CacheManager(vh, current_adr_id, current_sec_id, current_seg_id, sec_dis, x_axis, y_axis);

                //var update_result = scApp.VehicleBLL.updateVheiclePositionToReserveControlModule
                //    (scApp.ReserveBLL, vh, current_sec_id, x_axis, y_axis, 0, vh_angle, speed,
                //     Mirle.Hlts.Utils.HltDirection.Forward, Mirle.Hlts.Utils.HltDirection.None);
                var update_result = scApp.VehicleBLL.updateVheiclePositionToReserveControlModule
                    (scApp.ReserveBLL, vh, current_sec_id, x_axis, y_axis, 0, vh_angle, speed,
                     Mirle.Hlts.Utils.HltDirection.None, Mirle.Hlts.Utils.HltDirection.None);

                if (line.ServiceMode == SCAppConstants.AppServiceMode.Active)
                {
                    if (!SCUtility.isMatche(last_sec_id, current_sec_id))
                    {
                        //TODO 要改成查一次CMD出來然後直接帶入CMD ID
                        if (!SCUtility.isEmpty(vh.OHTC_CMD))
                        {
                            scApp.CMDBLL.update_CMD_DetailEntryTime(vh.OHTC_CMD, current_adr_id, current_sec_id);
                            scApp.CMDBLL.update_CMD_DetailLeaveTime(vh.OHTC_CMD, last_adr_id, last_sec_id);
                            List<string> willPassSecID = null;
                            vh.procProgress_Percen = scApp.CMDBLL.getAndUpdateVhCMDProgress(vh.VEHICLE_ID, out willPassSecID);
                            vh.WillPassSectionID = willPassSecID;
                            //scApp.VehicleBLL.NetworkQualityTest(vh.VEHICLE_ID, current_adr_id, current_sec_id, 0);
                        }
                        vh.onLocationChange(current_sec_id, last_sec_id);
                    }
                    if (!SCUtility.isMatche(current_seg_id, last_seg_id))
                    {
                        vh.onSegmentChange(current_seg_id, last_seg_id);
                    }
                    //if (!SCUtility.isMatche(current_adr_id, last_adr_id) || !SCUtility.isMatche(current_sec_id, last_sec_id))
                    //    scApp.VehicleBLL.updateVheiclePosition(vh.VEHICLE_ID, current_adr_id, current_sec_id, sec_dis, vhPassEvent);
                }

            }
            //tryDriveTheOnWillPassVh(vh.VEHICLE_ID);
        }

        private void tryDriveTheOnWillPassVh(string willPassVhID)
        {
            if (System.Threading.Interlocked.Exchange(ref syncPoint_NotifyVhAvoid, 1) == 0)
            {
                try
                {

                    AVEHICLE will_pass_vh = scApp.VehicleBLL.cache.getVhByID(willPassVhID);
                    //先依據目前在移動的車子往前找到Load/Unload的位置是否有閒置的車子在路上
                    List<string> will_pass_sections = will_pass_vh.WillPassSectionID;

                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                       Data: $"Try to notify vh avoid...,requestVh:{willPassVhID} ,will pass section:{string.Join(",", will_pass_sections)}",
                       VehicleID: willPassVhID);

                    foreach (string will_pass_section in will_pass_sections)
                    {
                        //用預約的方式每段去找看看是不是剛好有擋在路上的車子
                        var reserve_check_result =
                            scApp.ReserveBLL.TryAddReservedSection(willPassVhID, will_pass_section, isAsk: true);
                        if (reserve_check_result.OK == false)
                        {
                            //確認能否把該Vh趕走
                            string in_the_way_vh_id = reserve_check_result.VehicleID;
                            AVEHICLE in_the_way_vh = scApp.VehicleBLL.cache.getVhByID(in_the_way_vh_id);
                            var check_can_creat_avoid_command = canCreatDriveOutCommand(in_the_way_vh);
                            if (check_can_creat_avoid_command.is_can)
                            {
                                var find_result = findAvoidAddressNew(in_the_way_vh);
                                if (find_result.isFind)
                                {
                                    bool is_success = scApp.CMDBLL.doCreatTransferCommand(in_the_way_vh_id,
                                                                                          cmd_type: E_CMD_TYPE.Move,
                                                                                          destination_address: find_result.avoidAdr);
                                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                                       Data: $"Try to notify vh avoid,requestVh:{willPassVhID} reservedVh:{in_the_way_vh_id} avoid address:{find_result.avoidAdr}," +
                                             $" is success :{is_success}.",
                                       VehicleID: willPassVhID);
                                }
                                else
                                {
                                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                                       Data: $"Can't find the avoid address.",
                                       VehicleID: willPassVhID);
                                }
                            }
                            else
                            {
                                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                                   Data: $"In the way vh status not ready vh:{in_the_way_vh_id}.",
                                   VehicleID: willPassVhID);
                            }

                            return; //找到第一台擋路得車子後檢查完就可以結束了
                        }
                        else
                        {
                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                               Data: $"try reserve sec:{will_pass_section} ok.",
                               VehicleID: willPassVhID);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                       Data: ex,
                       Details: $"excute tryNotifyVhAvoid has exception happend.requestVh:{willPassVhID}");
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref syncPoint_NotifyVhAvoid, 0);
                }
            }
        }

        private void tryAskVhToIdlePosition(string findIdlePositionVh)
        {
            try
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                                       Data: $"start try find the vh of idle position:{findIdlePositionVh}...",
                                       VehicleID: findIdlePositionVh);
                bool is_success = findTheVhOfAvoidAddress("", findIdlePositionVh);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                                       Data: $"start try find the vh of idle position:{findIdlePositionVh},result:{is_success}",
                                       VehicleID: findIdlePositionVh);
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: ex);
            }
        }
        private void tryDriveOutTheVh(string willPassVhID, string inTheWayVhID)
        {
            if (System.Threading.Interlocked.Exchange(ref syncPoint_NotifyVhAvoid, 1) == 0)
            {
                try
                {
                    findTheVhOfAvoidAddress(willPassVhID, inTheWayVhID);
                }
                catch (Exception ex)
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                       Data: ex,
                       Details: $"excute tryNotifyVhAvoid has exception happend.requestVh:{willPassVhID}");
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref syncPoint_NotifyVhAvoid, 0);
                }
            }
        }

        private bool findTheVhOfAvoidAddress(string willPassVhID, string inTheWayVhID)
        {
            bool is_success = false;
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                                   Data: $"start try drive out vh:{inTheWayVhID}...",
                                   VehicleID: willPassVhID);

            //確認能否把該Vh趕走
            AVEHICLE in_the_way_vh = scApp.VehicleBLL.cache.getVhByID(inTheWayVhID);
            var check_can_creat_avoid_command = canCreatDriveOutCommand(in_the_way_vh);
            if (check_can_creat_avoid_command.is_can)
            {
                //B0.09 var find_result = findAvoidAddressNew(in_the_way_vh);
                var find_result = findAvoidAddressForFixPort(in_the_way_vh);//B0.09
                if (find_result.isFind)
                {
                    is_success = scApp.CMDBLL.doCreatTransferCommand(inTheWayVhID,
                                                                         cmd_type: E_CMD_TYPE.Move,
                                                                         destination_address: find_result.avoidAdr);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                       Data: $"Try to notify vh avoid,requestVh:{willPassVhID} reservedVh:{inTheWayVhID} avoid address:{find_result.avoidAdr}," +
                             $" is success :{is_success}.",
                       VehicleID: willPassVhID);
                }
                else
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                       Data: $"Can't find the avoid address.",
                       VehicleID: willPassVhID);
                }
            }
            else
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: $"start try drive out vh:{inTheWayVhID},but vh status not ready.",
                   VehicleID: willPassVhID);
            }
            return is_success;
        }

        private (bool isFind, string avoidAdr) findAvoidAddressForFixPort(AVEHICLE willDrivenAwayVh)
        {
            //1.找看看是否有設定的固定避車點。
            //List<PortDef> can_avoid_cv_port = scApp.PortDefBLL.cache.loadCanAvoidCVPortDefs();
            List<PortDef> can_avoid_port = scApp.PortDefBLL.cache.loadCanAvoidPortDefs();
            if (can_avoid_port == null || can_avoid_port.Count == 0)
            {
                //1.2.如果沒有則就把全部的CV納入選擇。
                can_avoid_port = scApp.PortDefBLL.cache.loadCVPortDefs();
            }
            //2.找出離自己最近的一個CV點
            var find_result = findTheNearestCVPort(willDrivenAwayVh, can_avoid_port);
            if (find_result.isFind)
            {
                return (true, find_result.PortDef.ADR_ID);
            }
            else
            {
                return (false, "");
            }
        }
        private (bool isFind, string avoidAdr) findAvoidAddressNew(AVEHICLE willDrivenAwayVh)
        {
            var all_cv_port = scApp.PortDefBLL.cache.loadCVPortDefs();
            //1.嘗試找出目前是in mode且離自己最近的in mode port
            var all_cv_port_in_mode = all_cv_port.Where(port => IsPortInMode(port));
            var find_result = findTheNearestCVPort(willDrivenAwayVh, all_cv_port_in_mode);
            if (find_result.isFind)
            {
                return (true, find_result.PortDef.ADR_ID);
            }
            else
            {
                //如果沒找到，則改找下一個離自己最近的cv port
                find_result = findTheNearestCVPort(willDrivenAwayVh, all_cv_port);
                if (find_result.isFind)
                {
                    return (true, find_result.PortDef.ADR_ID);
                }
                else
                {
                    return (false, "");
                }
            }
        }
        private bool IsPortInMode(PortDef port)
        {
            var transfer_service = scApp.TransferService;
            var plc_port_info = transfer_service.GetPLC_PortData(port.PLCPortID);
            if (plc_port_info == null)
            {
                return false;
            }
            else
            {
                return plc_port_info.IsInputMode;
            }
        }
        private (bool isFind, PortDef PortDef) findTheNearestCVPort(AVEHICLE willDrivenAwayVh, IEnumerable<PortDef> all_cv_port_in_mode)
        {
            int min_distance = int.MaxValue;
            PortDef nearest_cv_port = null;
            foreach (var port_def in all_cv_port_in_mode)
            {
                if (SCUtility.isMatche(port_def.ADR_ID, willDrivenAwayVh.CUR_ADR_ID)) continue;//如果目前所在的Address與要找的CV Port 一樣的話，要濾掉
                var check_result = scApp.GuideBLL.IsRoadWalkable(willDrivenAwayVh.CUR_ADR_ID, port_def.ADR_ID);
                if (check_result.isSuccess && check_result.distance < min_distance)
                {
                    min_distance = check_result.distance;
                    nearest_cv_port = port_def;
                }
            }
            return (nearest_cv_port != null, nearest_cv_port);
        }

        //[ClassAOPAspect]
        //public void TranEventReport(BCFApplication bcfApp, AVEHICLE eqpt, ID_136_TRANS_EVENT_REP recive_str, int seq_num)
        //{
        //    if (scApp.getEQObjCacheManager().getLine().ServerPreStop)
        //        return;

        //    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
        //       seq_num: seq_num,
        //       Data: recive_str,
        //       VehicleID: eqpt.VEHICLE_ID,
        //       CarrierID: eqpt.CST_ID);

        //    SCUtility.RecodeReportInfo(eqpt.VEHICLE_ID, seq_num, recive_str);
        //    EventType eventType = recive_str.EventType;
        //    string current_adr_id = recive_str.CurrentAdrID;
        //    string current_sec_id = recive_str.CurrentSecID;
        //    string carrier_id = recive_str.CSTID;
        //    string last_adr_id = eqpt.CUR_ADR_ID;
        //    string last_sec_id = eqpt.CUR_SEC_ID;
        //    string req_block_id = recive_str.RequestBlockID;
        //    //lock (eqpt.PositionRefresh_Sync)
        //    //{
        //    //    switch (eventType)
        //    //    {
        //    //        case EventType.LoadArrivals:
        //    //        case EventType.UnloadArrivals:
        //    //        case EventType.VhmoveArrivals:
        //    //            scApp.VehicleBLL.deleteRedisOfPositionReport(eqpt.VEHICLE_ID); //為了確保在PositionReportTimerAction要更新位置時，不會拿到舊的
        //    //            break;
        //    //    }
        //    //    doUpdateVheiclePositionAndCmdSchedule(eqpt, current_adr_id, current_sec_id, last_adr_id, last_sec_id, (uint)eqpt.ACC_SEC_DIST, eventType, loadCSTStatus);
        //    //}
        //    scApp.VehicleBLL.updateVehicleActionStatus(eqpt, eventType);

        //    switch (eventType)
        //    {
        //        case EventType.BlockReq:
        //            PositionReport_BlockReq_New(bcfApp, eqpt, seq_num, recive_str.RequestBlockID);
        //            break;
        //        case EventType.Hidreq:
        //            PositionReport_HIDRequest(bcfApp, eqpt, seq_num, recive_str.RequestBlockID);
        //            break;
        //        case EventType.LoadArrivals:
        //        case EventType.LoadComplete:
        //        case EventType.UnloadArrivals:
        //        case EventType.UnloadComplete:
        //        case EventType.VhmoveArrivals:
        //        case EventType.AdrOrMoveArrivals:
        //            PositionReport_ArriveAndComplete(bcfApp, eqpt, seq_num, recive_str.EventType, recive_str.CurrentAdrID, recive_str.CurrentSecID, carrier_id);
        //            break;
        //        case EventType.Vhloading:
        //        case EventType.Vhunloading:
        //            PositionReport_LoadingUnloading(bcfApp, eqpt, recive_str, seq_num, eventType);

        //            break;
        //        case EventType.BlockRelease:
        //            PositionReport_BlockRelease(bcfApp, eqpt, recive_str, seq_num);
        //            break;
        //        case EventType.Hidrelease:
        //            PositionReport_HIDRelease(bcfApp, eqpt, recive_str, seq_num);
        //            break;
        //    }
        //}

        private void PositionReport_LoadingUnloading(BCFApplication bcfApp, AVEHICLE eqpt, ID_136_TRANS_EVENT_REP recive_str, int seq_num, EventType eventType)
        {
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
               Data: $"Process report {eventType}",
               VehicleID: eqpt.VEHICLE_ID,
               CarrierID: eqpt.CST_ID);

            if (!SCUtility.isEmpty(eqpt.MCS_CMD))
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: $"do report {eventType} to mcs.",
                   VehicleID: eqpt.VEHICLE_ID,
                   CarrierID: eqpt.CST_ID);

                List<AMCSREPORTQUEUE> reportqueues = new List<AMCSREPORTQUEUE>();
                using (TransactionScope tx = SCUtility.getTransactionScope())
                {
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        bool isSuccess = true;
                        switch (eventType)
                        {
                            case EventType.Vhloading:
                                //scApp.CMDBLL.updateCMD_MCS_TranStatus2Transferring(eqpt.MCS_CMD);
                                //scApp.CMDBLL.updateCMD_MCS_CmdStatus2Loading(eqpt.MCS_CMD);
                                scApp.ReportBLL.newReportLoading(eqpt.VEHICLE_ID, reportqueues);
                                break;
                            case EventType.Vhunloading:
                                //scApp.CMDBLL.updateCMD_MCS_CmdStatus2Unloading(eqpt.MCS_CMD);
                                scApp.ReportBLL.newReportUnloading(eqpt.VEHICLE_ID, reportqueues);
                                break;
                        }
                        scApp.ReportBLL.insertMCSReport(reportqueues);

                        if (isSuccess)
                        {
                            if (replyTranEventReport(bcfApp, recive_str.EventType, eqpt, seq_num))
                            {
                                //scApp.VehicleBLL.updateVehicleStatus_CacheMangerForAct(eqpt, actionStat);
                                tx.Complete();
                                scApp.ReportBLL.newSendMCSMessage(reportqueues);
                            }
                        }
                    }
                }
            }
            else
            {
                replyTranEventReport(bcfApp, recive_str.EventType, eqpt, seq_num);
            }
            if (eventType == EventType.Vhloading)
            {
                scApp.VehicleBLL.doLoading(eqpt.VEHICLE_ID);
            }
            else if (eventType == EventType.Vhunloading)
            {
                scApp.VehicleBLL.doUnloading(eqpt.VEHICLE_ID);
            }
            Task.Run(() => scApp.FlexsimCommandDao.setVhEventTypeToFlexsimDB(eqpt.VEHICLE_ID, eventType));
        }

        [ClassAOPAspect]
        //public void TranEventReport_100(BCFApplication bcfApp, AVEHICLE eqpt, ID_136_TRANS_EVENT_REP recive_str, int seq_num)
        public void TranEventReport(BCFApplication bcfApp, AVEHICLE eqpt, ID_136_TRANS_EVENT_REP recive_str, int seq_num)
        {
            if (scApp.getEQObjCacheManager().getLine().ServerPreStop)
                return;

            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
               seq_num: seq_num,
               Data: recive_str,
               VehicleID: eqpt.VEHICLE_ID,
               CarrierID: eqpt.CST_ID);

            SCUtility.RecodeReportInfo(eqpt.VEHICLE_ID, seq_num, recive_str);
            EventType eventType = recive_str.EventType;
            string current_adr_id = recive_str.CurrentAdrID;
            string current_sec_id = recive_str.CurrentSecID;
            string carrier_id = recive_str.BOXID;
            string last_adr_id = eqpt.CUR_ADR_ID;
            string last_sec_id = eqpt.CUR_SEC_ID;
            string req_block_id = recive_str.RequestBlockID;
            string cmd_id = eqpt.MCS_CMD;
            BCRReadResult bCRReadResult = recive_str.BCRReadResult;
            string load_port_id = recive_str.LoadPortID;     //B0.01
            string unload_port_id = recive_str.UnloadPortID; //B0.01
            var reserveInfos = recive_str.ReserveInfos;

            scApp.VehicleBLL.updateVehicleActionStatus(eqpt, eventType);


            switch (eventType)
            {
                case EventType.BlockReq:
                case EventType.Hidreq:
                case EventType.BlockHidreq:
                    //PositionReport_BlockReq_HIDReq(bcfApp, eqpt, seq_num, recive_str.RequestBlockID, recive_str.RequestHIDID);
                    ProcessBlockOrHIDReq(bcfApp, eqpt, eventType, seq_num, recive_str.RequestBlockID, recive_str.RequestHIDID);
                    break;
                case EventType.LoadArrivals:
                case EventType.LoadComplete:
                case EventType.UnloadArrivals:
                case EventType.UnloadComplete:
                case EventType.AdrOrMoveArrivals:
                    //B0.01 PositionReport_ArriveAndComplete(bcfApp, eqpt, seq_num, recive_str.EventType, recive_str.CurrentAdrID, recive_str.CurrentSecID, carrier_id);
                    PositionReport_ArriveAndComplete(bcfApp, eqpt, seq_num, recive_str.EventType, recive_str.CurrentAdrID, recive_str.CurrentSecID, carrier_id, //B0.01 
                                                     load_port_id, unload_port_id);                                                                             //B0.01 
                    break;
                case EventType.Vhloading:
                case EventType.Vhunloading:
                    PositionReport_LoadingUnloading(bcfApp, eqpt, recive_str, seq_num, eventType);
                    break;
                case EventType.BlockRelease:
                    PositionReport_BlockRelease(bcfApp, eqpt, recive_str, seq_num);
                    replyTranEventReport(bcfApp, recive_str.EventType, eqpt, seq_num);
                    break;
                case EventType.Hidrelease:
                    PositionReport_HIDRelease(bcfApp, eqpt, recive_str, seq_num);
                    replyTranEventReport(bcfApp, recive_str.EventType, eqpt, seq_num);
                    break;
                case EventType.BlockHidrelease:
                    PositionReport_BlockRelease(bcfApp, eqpt, recive_str, seq_num);
                    PositionReport_HIDRelease(bcfApp, eqpt, recive_str, seq_num);
                    replyTranEventReport(bcfApp, recive_str.EventType, eqpt, seq_num);
                    break;
                case EventType.DoubleStorage:
                    PositionReport_DoubleStorage(bcfApp, eqpt, seq_num, recive_str.EventType, recive_str.CurrentAdrID, recive_str.CurrentSecID, carrier_id);
                    break;
                case EventType.EmptyRetrieval:
                    PositionReport_EmptyRetrieval(bcfApp, eqpt, seq_num, recive_str.EventType, recive_str.CurrentAdrID, recive_str.CurrentSecID, carrier_id);
                    break;
                case EventType.Bcrread:
                    TransferReportBCRRead(bcfApp, eqpt, seq_num, eventType, carrier_id, bCRReadResult);
                    break;
                case EventType.ReserveReq:
                    //TranEventReportPathReserveReq(bcfApp, eqpt, seq_num, reserveInfos);
                    TranEventReportPathReserveReqNew(bcfApp, eqpt, seq_num, reserveInfos);
                    break;
            }
        }

        object reserve_lock = new object();
        private void TranEventReportPathReserveReqNew(BCFApplication bcfApp, AVEHICLE eqpt, int seqNum, RepeatedField<ReserveInfo> reserveInfos)
        {
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
               Data: $"Process path reserve request,request path id:{reserveInfos.ToString()}",
               VehicleID: eqpt.VEHICLE_ID,
               CarrierID: eqpt.CST_ID);

            lock (reserve_lock)
            {
                //B0.02 var ReserveResult = IsReserveSuccessNew(eqpt.VEHICLE_ID, reserveInfos);
                var ReserveResult = IsMultiReserveSuccess(eqpt.VEHICLE_ID, reserveInfos);//B0.02
                if (ReserveResult.isSuccess)
                {
                    //not thing...
                }
                else
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                       Data: $"Reserve section fail,start try drive out vh:{ReserveResult.reservedVhID}",
                       VehicleID: eqpt.VEHICLE_ID,
                       CarrierID: eqpt.CST_ID);
                    //List<string> reserve_fail_sections = reserveInfos.Select(reserve => reserve.ReserveSectionID).ToList();
                    //Task.Run(() => tryNotifyVhAvoid_New(eqpt.VEHICLE_ID, ReserveResult.reservedVhID, reserve_fail_sections));

                    //在預約失敗以後，會嘗試看能不能將車子趕走
                    ALINE line = scApp.getEQObjCacheManager().getLine();
                    if (!IsCMD_MCSCanProcess() ||
                        line.SCStats == ALINE.TSCState.PAUSED)
                    {
                        Task.Run(() => tryDriveOutTheVh(eqpt.VEHICLE_ID, ReserveResult.reservedVhID));
                    }
                }
                //B0.02 replyTranEventReport(bcfApp, EventType.ReserveReq, eqpt, seqNum, canReservePass: ReserveResult.isSuccess, reserveInfos: reserveInfos);
                replyTranEventReport(bcfApp, EventType.ReserveReq, eqpt, seqNum, canReservePass: ReserveResult.isSuccess, reserveInfos: ReserveResult.reserveSuccessInfos);//B0.02
            }
        }

        private long syncPoint_NotifyVhAvoid = 0;
        private void tryNotifyVhAvoid_New(string requestVhID, string reservedVhID, List<string> reserveFailSectionIDs)
        {
            if (System.Threading.Interlocked.Exchange(ref syncPoint_NotifyVhAvoid, 1) == 0)
            {

                try
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                       Data: $"Try to notify vh avoid...,requestVh:{requestVhID} reservedVh:{reservedVhID}",
                       VehicleID: requestVhID);
                    AVEHICLE reserved_vh = scApp.VehicleBLL.cache.getVhByID(reservedVhID);
                    AVEHICLE request_vh = scApp.VehicleBLL.cache.getVhByID(requestVhID);

                    //先確認是否可以進行趕車的確認
                    var check_can_creat_avoid_command = canCreatDriveOutCommand(reserved_vh);
                    if (check_can_creat_avoid_command.is_can)
                    {
                        string reserved_vh_current_section = reserved_vh.CUR_SEC_ID;

                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                           Data: $"start search section:{reserved_vh_current_section}",
                           VehicleID: requestVhID);

                        var findResult = findNotConflictSectionAndAvoidAddressNew
                                         (request_vh, reserved_vh, reserveFailSectionIDs);
                        if (!findResult.isFind)
                        {
                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                               Data: $"find not conflict section fail. reserved vh current section:{reserved_vh_current_section},",
                               VehicleID: requestVhID);
                            return;
                        }
                        else
                        {
                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                               Data: $"find avoid address:{findResult.avoidAdr}",
                               VehicleID: requestVhID);
                        }

                        string avoid_address = findResult.avoidAdr;


                        if (!SCUtility.isEmpty(avoid_address))
                        {
                            bool is_success = scApp.CMDBLL.doCreatTransferCommand(reserved_vh.VEHICLE_ID, string.Empty, string.Empty,
                                                                E_CMD_TYPE.Move,
                                                                source: string.Empty,
                                                                destination: string.Empty,
                                                                source_address: string.Empty,
                                                                destination_address: avoid_address);
                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                               Data: $"Try to notify vh avoid,requestVh:{requestVhID} reservedVh:{reservedVhID}, is success :{is_success}.",
                               VehicleID: requestVhID);
                        }
                        else
                        {
                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                               Data: $"Try to notify vh avoid,requestVh:{requestVhID} reservedVh:{reservedVhID}, fail.",
                               VehicleID: requestVhID);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                       Data: ex,
                       Details: $"excute tryNotifyVhAvoid has exception happend.requestVh:{requestVhID},reservedVh:{reservedVhID}");
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref syncPoint_NotifyVhAvoid, 0);
                }
            }
        }

        private (bool isFind, string avoidAdr) findNotConflictSectionAndAvoidAddressNew
            (AVEHICLE requestVh, AVEHICLE reservedVh, List<string> reserveFailSections)
        {
            if (requestVh.WillPassSectionID == null || requestVh.WillPassSectionID.Count == 0)
            {
                if (reserveFailSections == null || reserveFailSections.Count == 0)
                {
                    return (false, "");
                }
                else
                {
                    string finial_section = reserveFailSections.LastOrDefault();
                    return findAvoidAddress(reservedVh, finial_section);
                }
            }
            else
            {
                List<string> will_pass_sections = requestVh.WillPassSectionID;
                string finial_section = will_pass_sections.LastOrDefault();
                return findAvoidAddress(reservedVh, finial_section);
            }
        }
        private const string VehicleVirtualSymbol = "virtual";
        private (bool isFind, string avoidAdr) findAvoidAddress(AVEHICLE reservedVh, string finialSectionID)
        {
            string virtual_vh_id = $"{VehicleVirtualSymbol}_{reservedVh.VEHICLE_ID}";
            try
            {
                ASECTION finial_section = scApp.SectionBLL.cache.GetSection(finialSectionID);
                scApp.ReserveBLL.TryAddVehicleOrUpdate(virtual_vh_id, finial_section.TO_ADR_ID);
                string to_adr = finial_section.TO_ADR_ID;
                int current_find_avoid_adr_count = 0;
                int MAX_FIND_AVOID_ADR_COUNT = 10;
                do
                {
                    ASECTION next_sections =
                        scApp.SectionBLL.cache.GetSectionsByFromAddress(to_adr).LastOrDefault();
                    if (next_sections != null)
                    {
                        var reserve_check_result =
                            scApp.ReserveBLL.TryAddReservedSection(reservedVh.VEHICLE_ID, next_sections.SEC_ID, isAsk: true);
                        if (SCUtility.isMatche(reservedVh.CUR_ADR_ID, next_sections.TO_ADR_ID))
                        {
                            to_adr = next_sections.TO_ADR_ID;
                            continue;
                        }

                        if (reserve_check_result.OK)
                        {
                            return (true, next_sections.TO_ADR_ID);
                        }
                        else
                        {
                            to_adr = next_sections.TO_ADR_ID;
                        }
                    }
                } while (current_find_avoid_adr_count++ < MAX_FIND_AVOID_ADR_COUNT);
                return (false, "");
            }
            finally
            {
                scApp.ReserveBLL.RemoveVehicle(virtual_vh_id);
            }
        }

        enum CAN_NOT_AVOID_RESULT
        {
            Normal
        }
        private (bool is_can, CAN_NOT_AVOID_RESULT result) canCreatDriveOutCommand(AVEHICLE reservedVh)
        {
            bool is_can = reservedVh.isTcpIpConnect &&
                          !reservedVh.IsError &&
                          (reservedVh.MODE_STATUS == VHModeStatus.AutoRemote ||
                           reservedVh.MODE_STATUS == VHModeStatus.AutoLocal) &&
                           reservedVh.ACT_STATUS == VHActionStatus.NoCommand &&
                           !scApp.CMDBLL.isCMD_OHTCExcuteByVh(reservedVh.VEHICLE_ID);
            return (is_can, CAN_NOT_AVOID_RESULT.Normal);
        }


        private (bool isSuccess, string reservedVhID, string reservedSecID) IsReserveSuccessNew(string vhID, RepeatedField<ReserveInfo> reserveInfos, bool isAsk = false)
        {
            try
            {
                if (DebugParameter.isForcedPassReserve)
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                       Data: "test flag: Force pass reserve is open, will driect reply to vh pass",
                       VehicleID: vhID);
                    return (true, string.Empty, string.Empty);
                }

                //強制拒絕Reserve的要求
                if (DebugParameter.isForcedRejectReserve)
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                       Data: "test flag: Force reject reserve is open, will driect reply to vh can't pass",
                       VehicleID: vhID);
                    return (false, string.Empty, string.Empty);
                }
                AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vhID);
                if (vh.IsPrepareAvoid)
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                       Data: $"vh:{vhID} is prepare excute avoid action, will reject reserve request.",
                       VehicleID: vhID);
                    return (false, string.Empty, string.Empty);
                }
                if (reserveInfos == null || reserveInfos.Count == 0) return (false, string.Empty, string.Empty);
                string reserve_section_id = reserveInfos[0].ReserveSectionID;


                Mirle.Hlts.Utils.HltDirection hltDirection = Mirle.Hlts.Utils.HltDirection.Forward;
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: $"vh:{vhID} Try add reserve section:{reserve_section_id} ,hlt dir:{hltDirection}...",
                   VehicleID: vhID);
                var result = scApp.ReserveBLL.TryAddReservedSection(vhID, reserve_section_id,
                                                                    sensorDir: hltDirection,
                                                                    isAsk: isAsk);

                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: $"vh:{vhID} Try add reserve section:{reserve_section_id},result:{result.ToString()}",
                   VehicleID: vhID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: $"current reserve section:{scApp.ReserveBLL.GetCurrentReserveSection()}",
                   VehicleID: vhID);
                return (result.OK, result.VehicleID, reserve_section_id);
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: ex,
                   Details: $"process function:{nameof(IsReserveSuccessNew)} Exception");
                return (false, string.Empty, string.Empty);
            }
        }
        public void ReserveTest(string vhID, string secID)
        {
            RepeatedField<ReserveInfo> reserveInfos = new RepeatedField<ReserveInfo>()
            {
                new ReserveInfo()
                {
                     DriveDirction =   DriveDirction.DriveDirForward,
                      ReserveSectionID = secID
                },
                new ReserveInfo()
                {
                     DriveDirction =   DriveDirction.DriveDirForward,
                      ReserveSectionID = "00126"
                }
            };
            IsMultiReserveSuccess(vhID, reserveInfos);
        }
        private (bool isSuccess, string reservedVhID, RepeatedField<ReserveInfo> reserveSuccessInfos) IsMultiReserveSuccess
            (string vhID, RepeatedField<ReserveInfo> reserveInfos, bool isAsk = false)
        {
            try
            {
                if (DebugParameter.isForcedPassReserve)
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                       Data: "test flag: Force pass reserve is open, will driect reply to vh pass",
                       VehicleID: vhID);
                    return (true, string.Empty, reserveInfos);
                }

                //強制拒絕Reserve的要求
                if (DebugParameter.isForcedRejectReserve)
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                       Data: "test flag: Force reject reserve is open, will driect reply to vh can't pass",
                       VehicleID: vhID);
                    return (false, string.Empty, null);
                }
                AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vhID);
                if (vh.IsPrepareAvoid)
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                       Data: $"vh:{vhID} is prepare excute avoid action, will reject reserve request.",
                       VehicleID: vhID);
                    return (false, string.Empty, null);
                }
                if (reserveInfos == null || reserveInfos.Count == 0) return (false, string.Empty, null);

                var reserve_success_section = new RepeatedField<ReserveInfo>();
                bool has_success = false;
                string final_blocked_vh_id = string.Empty;
                Mirle.Hlts.Utils.HltResult result = default(Mirle.Hlts.Utils.HltResult);
                foreach (var reserve_info in reserveInfos)
                {
                    string reserve_section_id = reserve_info.ReserveSectionID;
                    //if (SCUtility.isMatche(reserve_section_id, vh.CUR_SEC_ID))
                    //{
                    //    result = new Mirle.Hlts.Utils.HltResult(true, "");
                    //}
                    //else
                    {
                        if (scApp.SectionBLL.cache.IsNeedReserveChcek(reserve_section_id))
                        {
                            Mirle.Hlts.Utils.HltDirection hltDirection = Mirle.Hlts.Utils.HltDirection.Forward;
                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                               Data: $"vh:{vhID} Try add reserve section:{reserve_section_id} ,hlt dir:{hltDirection}...",
                               VehicleID: vhID);
                            result = scApp.ReserveBLL.TryAddReservedSection(vhID, reserve_section_id,
                                                                                sensorDir: hltDirection,
                                                                                isAsk: isAsk);

                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                               Data: $"vh:{vhID} Try add reserve section:{reserve_section_id},result:{result.ToString()}",
                               VehicleID: vhID);
                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                               Data: $"current reserve section:{scApp.ReserveBLL.GetCurrentReserveSection()}",
                               VehicleID: vhID);
                            //如果預約不到的時候，確認目前要的Section是否為CurrentAdr的上一段Section
                            //式的話就代表已經走過了，就給他直接通過
                            if (!result.OK)
                            {
                                string current_adr = vh.CUR_ADR_ID;
                                var last_sections = scApp.SectionBLL.cache.GetSectionsByToAddress(current_adr);
                                if (last_sections.Count > 0)
                                {
                                    ASECTION last_sec = last_sections[0];
                                    if (SCUtility.isMatche(reserve_section_id, last_sec.SEC_ID))
                                    {
                                        result.OK = true;
                                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                                           Data: $"Froce pass reserve section:{reserve_section_id},becuse it is last section of adr:{current_adr}",
                                           VehicleID: vhID);

                                    }
                                }
                            }
                        }
                        else
                        {
                            Mirle.Hlts.Utils.HltDirection hltDirection = Mirle.Hlts.Utils.HltDirection.Forward;
                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                               Data: $"vh:{vhID} Try add(Only ask) reserve section:{reserve_section_id} ,hlt dir:{hltDirection}...",
                               VehicleID: vhID);
                            result = scApp.ReserveBLL.TryAddReservedSection(vhID, reserve_section_id,
                                                                                sensorDir: hltDirection,
                                                                                isAsk: true);
                            //result = new Mirle.Hlts.Utils.HltResult(true, "");
                        }
                    }
                    if (result.OK)
                    {
                        reserve_success_section.Add(reserve_info);
                        has_success |= true;
                    }
                    else
                    {
                        has_success |= false;
                        final_blocked_vh_id = result.VehicleID;
                        break;
                    }
                }

                return (has_success, final_blocked_vh_id, reserve_success_section);
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: ex,
                   Details: $"process function:{nameof(IsMultiReserveSuccess)} Exception");
                return (false, string.Empty, null);
            }
        }
        private void TransferReportBCRRead(BCFApplication bcfApp, AVEHICLE eqpt, int seqNum,
                                             EventType eventType, string read_carrier_id, BCRReadResult bCRReadResult)
        {
            scApp.VehicleBLL.updateVehicleBCRReadResult(eqpt, bCRReadResult);
            AVIDINFO vid_info = scApp.VIDBLL.getVIDInfo(eqpt.VEHICLE_ID);
            string old_carrier_id = SCUtility.Trim(vid_info.CARRIER_ID, true);

            //var port_station = scApp.PortStationBLL.OperateCatch.getPortStationByID(eqpt.CUR_ADR_ID);
            //string port_station_id = port_station == null ? "" : port_station.PORT_ID;
            //LogHelper.LogBCRReadInfo
            //    (eqpt.VEHICLE_ID, port_station_id, eqpt.MCS_CMD, eqpt.OHTC_CMD, old_carrier_id, read_carrier_id, bCRReadResult, SystemParameter.IsEnableIDReadFailScenario);

            bool is_need_report_install = CheckIsNeedReportInstall2MCS(eqpt, vid_info);

            scApp.VIDBLL.upDateVIDCarrierLocInfo(eqpt.VEHICLE_ID, eqpt.Real_ID);
            switch (bCRReadResult)
            {
                case BCRReadResult.BcrMisMatch:
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                       Data: $"BCR miss match happend,start abort command id:{eqpt.OHTC_CMD?.Trim()} and rename cst id:{old_carrier_id}...",
                       VehicleID: eqpt.VEHICLE_ID,
                       CarrierID: eqpt.BOX_ID);
                    if (!checkHasDuplicateHappend(bcfApp, eqpt, seqNum, eventType, read_carrier_id, old_carrier_id))
                    {
                        scApp.VehicleBLL.updataVehicleBOXID(eqpt.VEHICLE_ID, read_carrier_id);
                        if (scApp.CMDBLL.getCMD_OHTCByID(eqpt.OHTC_CMD).CMD_TPYE == E_CMD_TYPE.Scan)
                        {
                            replyTranEventReport(bcfApp, eventType, eqpt, seqNum,
                            renameCarrierID: read_carrier_id,
                            cancelType: CMDCancelType.CmdNone);
                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                               Data: $"BCR miss match happend,but in Scan command id:{eqpt.OHTC_CMD?.Trim()} and rename cst id:{old_carrier_id} to {read_carrier_id}",
                               VehicleID: eqpt.VEHICLE_ID,
                               CarrierID: eqpt.BOX_ID);
                        }
                        else
                        {
                            replyTranEventReport(bcfApp, eventType, eqpt, seqNum,
                            renameCarrierID: read_carrier_id,
                            cancelType: CMDCancelType.CmdCancelIdMismatch);
                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                               Data: $"BCR miss match happend,start abort command id:{eqpt.OHTC_CMD?.Trim()} and rename cst id:{old_carrier_id} to {read_carrier_id}",
                               VehicleID: eqpt.VEHICLE_ID,
                               CarrierID: eqpt.BOX_ID);
                        }
                    }
                    // Task.Run(() => doAbortCommand(eqpt, eqpt.OHTC_CMD, CMDCancelType.CmdCancelIdMismatch));
                    //20200130 Hsinyu Chang
                    scApp.CMDBLL.updateCMD_MCS_BCROnCrane(eqpt.MCS_CMD, read_carrier_id);
                    break;
                case BCRReadResult.BcrReadFail:
                    string new_carrier_id = "";
                    CMDCancelType cancelType = CMDCancelType.CmdNone;
                    if (SystemParameter.IsEnableIDReadFailScenario)
                    {
                        ALINE line = scApp.getEQObjCacheManager().getLine();
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                           Data: $"BCR read fail happend,start abort command id:{eqpt.OHTC_CMD?.Trim()} and rename BOX id...",
                           VehicleID: eqpt.VEHICLE_ID,
                           CarrierID: eqpt.BOX_ID);
                        //string old_carrier_id = SCUtility.Trim(vid_info.CARRIER_ID, true);
                        bool is_unknow_old_name_cst = SCUtility.isEmpty(old_carrier_id);
                        //string new_carrier_id = string.Empty;
                        if (is_unknow_old_name_cst)
                        {
                            new_carrier_id = "ERROR1";
                            scApp.VIDBLL.upDateVIDCarrierID(eqpt.VEHICLE_ID, new_carrier_id);
                        }
                        else
                        {
                            //bool was_renamed = old_carrier_id.StartsWith("UNK");
                            //new_carrier_id = was_renamed ? old_carrier_id : "ERROR1";

                            // Rename the cmd boxID to the readfail ID for the MCS to rename the CST when it pass the OHCV.
                            new_carrier_id = eqpt.BOX_ID;
                        }
                        scApp.VehicleBLL.updataVehicleBOXID(eqpt.VEHICLE_ID, new_carrier_id);
                        if (scApp.CMDBLL.getCMD_OHTCByID(eqpt.OHTC_CMD).CMD_TPYE == E_CMD_TYPE.Scan)
                        {
                            cancelType = CMDCancelType.CmdNone;
                        }
                        else
                        {
                            cancelType = CMDCancelType.CmdNone;
                        }
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                           Data: $"BCR read fail happend,start abort command id:{eqpt.OHTC_CMD?.Trim()} and rename cst id:{old_carrier_id} to {new_carrier_id} ",
                           VehicleID: eqpt.VEHICLE_ID,
                           CarrierID: eqpt.BOX_ID);
                    }
                    else
                    {
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                           Data: $"BCR read fail happend,continue excute command.",
                           VehicleID: eqpt.VEHICLE_ID,
                           CarrierID: eqpt.BOX_ID);
                        cancelType = CMDCancelType.CmdCancelIdReadFailed; // None => CmdCancelIdReadFailed for readFail reply to OHT.
                        if (scApp.CMDBLL.getCMD_OHTCByID(eqpt.OHTC_CMD).CMD_TPYE == E_CMD_TYPE.Scan)
                        {
                            cancelType = CMDCancelType.CmdNone;
                        }
                        else
                        {
                            cancelType = CMDCancelType.CmdNone;
                        }
                        if (!SCUtility.isEmpty(eqpt.MCS_CMD))
                        {
                            ACMD_MCS mcs_cmd = scApp.CMDBLL.getCMD_MCSByID(eqpt.MCS_CMD);
                            if (mcs_cmd != null)
                            {
                                new_carrier_id = SCUtility.Trim(mcs_cmd.CARRIER_ID);
                            }
                            else
                            {
                                new_carrier_id = "";
                                is_need_report_install = false;
                            }
                        }
                        else
                        {
                            new_carrier_id = "";
                            is_need_report_install = false;
                        }
                    }

                    //replyTranEventReport(bcfApp, eventType, eqpt, seqNum,
                    //    renameCarrierID: new_carrier_id, cancelType: CMDCancelType.CmdCancelIdReadFailed);
                    replyTranEventReport(bcfApp, eventType, eqpt, seqNum,
                        renameCarrierID: new_carrier_id,
                        cancelType: cancelType);
                    //     Task.Run(() => doAbortCommand(eqpt, eqpt.OHTC_CMD, CMDCancelType.CmdCancelIdReadFailed));
                    // B0.03
                    scApp.TransferService.OHBC_AlarmSet(eqpt.VEHICLE_ID, ((int)AlarmLst.OHT_BCR_READ_FAIL).ToString());
                    scApp.TransferService.OHBC_AlarmCleared(eqpt.VEHICLE_ID, ((int)AlarmLst.OHT_BCR_READ_FAIL).ToString());
                    //
                    //20200130 Hsinyu Chang
                    scApp.CMDBLL.updateCMD_MCS_BCROnCrane(eqpt.MCS_CMD, new_carrier_id);
                    break;
                case BCRReadResult.BcrNormal:
                    if (!checkHasDuplicateHappend(bcfApp, eqpt, seqNum, eventType, read_carrier_id, old_carrier_id))
                    {
                        scApp.VehicleBLL.updataVehicleBOXID(eqpt.VEHICLE_ID, read_carrier_id);
                        replyTranEventReport(bcfApp, eventType, eqpt, seqNum);
                        //20200130 Hsinyu Chang
                        scApp.CMDBLL.updateCMD_MCS_BCROnCrane(eqpt.MCS_CMD, read_carrier_id);
                    }
                    break;
            }

            if (is_need_report_install)
            {
                List<AMCSREPORTQUEUE> reportqueues = new List<AMCSREPORTQUEUE>();
                scApp.ReportBLL.newReportCarrierIDReadReport(eqpt.VEHICLE_ID, reportqueues);
                scApp.ReportBLL.insertMCSReport(reportqueues);
                scApp.ReportBLL.newSendMCSMessage(reportqueues);
            }

            scApp.TransferService.OHT_IDRead(eqpt.MCS_CMD, eqpt.VEHICLE_ID, read_carrier_id, bCRReadResult);
        }

        private static bool CheckIsNeedReportInstall2MCS(AVEHICLE eqpt, AVIDINFO vid_info)
        {
            //bool is_need_report_install = false;
            bool is_need_report_install = true;
            if (SCUtility.isEmpty(eqpt.MCS_CMD))
                return false;
            //if (vid_info != null)
            //{
            //    if (!SCUtility.isMatche(eqpt.Real_ID, vid_info.CARRIER_LOC))
            //    {
            //        is_need_report_install = true;
            //    }
            //}
            return is_need_report_install;
        }


        private bool checkHasDuplicateHappend(BCFApplication bcfApp, AVEHICLE eqpt, int seqNum, EventType eventType, string read_carrier_id, string oldCarrierID)
        {
            bool is_happend = false;
            //AVEHICLE vh = scApp.VehicleBLL.cache.getVhByCSTID(read_carrier_id);
            int has_carry_this_cst_of_vh = scApp.VehicleBLL.cache.getVhByHasCSTIDCount(read_carrier_id);
            if (DebugParameter.TestDuplicate || has_carry_this_cst_of_vh >= 2)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: $" Carrier duplicate happend,start abort command id:{eqpt.OHTC_CMD?.Trim()},and check is need rename cst id:{oldCarrierID}...",
                   VehicleID: eqpt.VEHICLE_ID,
                   CarrierID: eqpt.CST_ID);
                bool was_renamed = oldCarrierID.StartsWith("UNKNOWNDUP");

                string rename_duplicate_carrier_id = was_renamed ?
                    oldCarrierID :
                    $"UNKNOWNDUP-{read_carrier_id}-{DateTime.Now.ToString(SCAppConstants.TimestampFormat_12)}001";//固定加入001的Sequence
                scApp.VehicleBLL.updataVehicleCSTID(eqpt.VEHICLE_ID, rename_duplicate_carrier_id);
                replyTranEventReport(bcfApp, eventType, eqpt, seqNum,
                            renameCarrierID: rename_duplicate_carrier_id, cancelType: CMDCancelType.CmdCancelIdReadDuplicate);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: $" Carrier duplicate happend,start abort command id:{eqpt.OHTC_CMD?.Trim()},and check is need rename cst id:{oldCarrierID} to {rename_duplicate_carrier_id}",
                   VehicleID: eqpt.VEHICLE_ID,
                   CarrierID: eqpt.CST_ID);

                is_happend = true;
            }
            return is_happend;
        }

        //private void PositionReport_BlockReq(BCFApplication bcfApp, AVEHICLE eqpt, int seqNum, string req_block_id)
        //{
        //    bool isSucess = true;
        //    bool canPass = false;
        //    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
        //       Data: $"Process block request,request block id:{req_block_id}",
        //       VehicleID: eqpt.VEHICLE_ID,
        //       CarrierID: eqpt.CST_ID);

        //    //判斷是否有重覆要這個Block
        //    //if (!isReqBlockAgain)
        //    //{
        //    //    List<BLOCKZONEQUEUE> sameVhNotReleaseblockZoneQueues = null;
        //    //    //判斷是否有要了其他的Block未釋放

        //    //    if (checkHasOrtherBolckZoneQueueNonRelease(eqpt, out sameVhNotReleaseblockZoneQueues))
        //    //    {
        //    //        //using (TransactionScope tx = new TransactionScope())
        //    //        //using (TransactionScope tx = new
        //    //        //    TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
        //    //        List<KeyValuePair<string, string>> ReleaseBlocks = new List<KeyValuePair<string, string>>();
        //    //        using (TransactionScope tx = SCUtility.getTransactionScope())
        //    //        {
        //    //            using (DBConnection_EF con = DBConnection_EF.GetUContext())
        //    //            {
        //    //                //con.BeginTransaction();
        //    //                isSucess = true;
        //    //                foreach (BLOCKZONEQUEUE queue in sameVhNotReleaseblockZoneQueues)
        //    //                {
        //    //                    isSucess &= scApp.MapBLL.updateBlockZoneQueue_AbnormalEnd(queue,
        //    //                        SCAppConstants.BlockQueueState.Abnormal_Release_OrtherNonRelease);
        //    //                    isSucess &= scApp.MapBLL.NoticeBlockVhPassByEntrySecID(queue.ENTRY_SEC_ID);
        //    //                    ReleaseBlocks.Add(new KeyValuePair<string, string>(queue.CAR_ID, queue.ENTRY_SEC_ID));
        //    //                }
        //    //                if (isSucess)
        //    //                {
        //    //                    tx.Complete();
        //    //                    foreach (var keyValue in ReleaseBlocks)
        //    //                    {
        //    //                        scApp.MapBLL.DeleteBlockControlKeyWordToRedis(keyValue.Key);
        //    //                    }
        //    //                }
        //    //                else
        //    //                {
        //    //                    //return;
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    if (DebugParameter.isForcedPassBlockControl)
        //    {
        //        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
        //           Data: "test flag: Force pass block control is open, will driect reply to vh can pass block",
        //           VehicleID: eqpt.VEHICLE_ID,
        //           CarrierID: eqpt.CST_ID);

        //        reply_Trans_Event_Report(bcfApp, EventType.BlockReq, eqpt, seqNum, canBlockPass: true);
        //        return;
        //    }
        //    if (DebugParameter.isForcedRejectBlockControl)
        //    {
        //        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
        //           Data: "test flag: Force reject block control is open, will driect reply to vh can't pass block",
        //           VehicleID: eqpt.VEHICLE_ID,
        //           CarrierID: eqpt.CST_ID);

        //        reply_Trans_Event_Report(bcfApp, EventType.BlockReq, eqpt, seqNum, canBlockPass: false);
        //        return;
        //    }

        //    string current_block_id_status = string.Empty;
        //    bool hasAskOrtherBlock =
        //        scApp.MapBLL.HasOrtherBlockControlAskedFromRedis(eqpt.VEHICLE_ID, req_block_id, out current_block_id_status);
        //    if (hasAskOrtherBlock)
        //    {
        //        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
        //           Data: "this vh has ask orther block,so can't request current block",
        //           VehicleID: eqpt.VEHICLE_ID,
        //           CarrierID: eqpt.CST_ID);

        //        LogCollection.BlockControlLogger.Trace($"vh id:{eqpt.VEHICLE_ID} has ask orther block.");
        //        reply_Trans_Event_Report(bcfApp, EventType.BlockReq, eqpt, seqNum, canBlockPass: false);
        //        return;
        //    }

        //    //bool isBlocking = scApp.MapBLL.isBlockingBlockZoneByVhIDAndCrtBlockSecID(eqpt.VEHICLE_ID, req_block_id);
        //    bool isBlocking = SCUtility.isMatche(current_block_id_status, SCAppConstants.BlockQueueState.Blocking)
        //                      || SCUtility.isMatche(current_block_id_status, SCAppConstants.BlockQueueState.Through);
        //    if (isBlocking)
        //    {
        //        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
        //           Data: $"vh get block id:{req_block_id} again!",
        //           VehicleID: eqpt.VEHICLE_ID,
        //           CarrierID: eqpt.CST_ID);
        //        reply_Trans_Event_Report(bcfApp, EventType.BlockReq, eqpt, seqNum, canBlockPass: true);
        //        return;
        //    }
        //    else
        //    {
        //        //using (TransactionScope tx = new
        //        //    TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = SCAppConstants.ISOLATION_LEVEL }))
        //        using (TransactionScope tx = SCUtility.getTransactionScope())
        //        {
        //            using (DBConnection_EF con = DBConnection_EF.GetUContext())
        //            {
        //                //if (!isReqBlockAgain)
        //                if (!SCUtility.isMatche(current_block_id_status, SCAppConstants.BlockQueueState.Request))
        //                {
        //                    //先確認他所上報的SEC ID 是Block的SEC ID
        //                    ABLOCKZONEMASTER block_master = scApp.MapBLL.getBlockZoneMasterByEntrySecID(req_block_id);
        //                    if (block_master != null)
        //                    {
        //                        //確認VH是否可以通過
        //                        DateTime reqest_time = DateTime.Now;
        //                        canPass = canPassBlockZone(req_block_id);

        //                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
        //                           Data: $"Vh:{eqpt.VEHICLE_ID} ask block:{req_block_id},ask result:{canPass}",
        //                           VehicleID: eqpt.VEHICLE_ID,
        //                           CarrierID: eqpt.CST_ID);

        //                        scApp.MapBLL.doCreatBlockZoneQueueByReqStatus(eqpt.VEHICLE_ID, req_block_id, canPass, reqest_time);
        //                        scApp.MapBLL.CreatBlockControlKeyWordToRedis(eqpt.VEHICLE_ID, req_block_id, canPass, reqest_time);
        //                    }
        //                    else
        //                    {
        //                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
        //                           Data: $"Vh:{eqpt.VEHICLE_ID} ask block:{req_block_id},but this block id not exist!",
        //                           VehicleID: eqpt.VEHICLE_ID,
        //                           CarrierID: eqpt.CST_ID);

        //                        logger.Warn("vh:{0} req block id not exist {1}", eqpt.VEHICLE_ID, req_block_id);

        //                        canPass = false;
        //                    }
        //                }
        //                else
        //                {
        //                    canPass = canPassBlockZone(req_block_id);

        //                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
        //                       Data: $"Vh:{eqpt.VEHICLE_ID} ask again block:{req_block_id},ask result:{canPass}",
        //                       VehicleID: eqpt.VEHICLE_ID,
        //                       CarrierID: eqpt.CST_ID);
        //                    if (canPass)
        //                    {
        //                        if (scApp.MapBLL.IsBlockControlStatus
        //                            (eqpt.VEHICLE_ID, SCAppConstants.BlockQueueState.Request))
        //                        {
        //                            scApp.MapBLL.updateBlockZoneQueue_BlockTime(eqpt.VEHICLE_ID, req_block_id);
        //                            scApp.MapBLL.ChangeBlockControlStatus_Blocking(eqpt.VEHICLE_ID);
        //                        }
        //                    }
        //                }

        //                Boolean resp_cmp = reply_Trans_Event_Report(bcfApp, EventType.BlockReq, eqpt, seqNum, canPass);

        //                if (resp_cmp)
        //                {
        //                    tx.Complete();
        //                }
        //                else
        //                {
        //                    //con.Rollback();
        //                    return;
        //                }
        //            }
        //        }
        //    }
        //}



        private void ProcessBlockOrHIDReq(BCFApplication bcfApp, AVEHICLE eqpt, EventType eventType, int seqNum, string req_block_id, string req_hid_secid)
        {
            bool can_block_pass = true;
            bool can_hid_pass = true;
            bool isSuccess = false;
            using (TransactionScope tx = SCUtility.getTransactionScope())
            {
                if (eventType == EventType.BlockReq || eventType == EventType.BlockHidreq)
                    can_block_pass = ProcessBlockReqNew(bcfApp, eqpt, req_block_id);
                if (eventType == EventType.Hidreq || eventType == EventType.BlockHidreq)
                    can_hid_pass = ProcessHIDRequest(bcfApp, eqpt, req_hid_secid);
                isSuccess = replyTranEventReport(bcfApp, eventType, eqpt, seqNum, canBlockPass: can_block_pass, canHIDPass: can_hid_pass);
                if (isSuccess)
                {
                    tx.Complete();
                }
            }

            //if (isSuccess &&
            //    (eventType == EventType.Hidreq || eventType == EventType.BlockHidreq))
            //{
            //    scApp.HIDBLL.VHEntryHIDZone(req_hid_secid);
            //    Task.Run(() => checkHIDSpaceIsSufficient(eqpt, req_hid_secid));
            //}
        }

        private bool ProcessBlockReq(BCFApplication bcfApp, AVEHICLE eqpt, string req_block_id)
        {
            bool canBlockPass = false;
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
               Data: $"Process block request,request block id:{req_block_id}",
               VehicleID: eqpt.VEHICLE_ID,
               CarrierID: eqpt.CST_ID);
            if (DebugParameter.isForcedPassBlockControl)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: "test flag: Force pass block control is open, will driect reply to vh can pass block",
                   VehicleID: eqpt.VEHICLE_ID,
                   CarrierID: eqpt.CST_ID);
                canBlockPass = true;
            }
            else if (DebugParameter.isForcedRejectBlockControl)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: "test flag: Force reject block control is open, will driect reply to vh can't pass block",
                   VehicleID: eqpt.VEHICLE_ID,
                   CarrierID: eqpt.CST_ID);
                canBlockPass = false;
            }
            else
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //先確認在Redis上是否該台VH 已經有要過的Block
                    bool hasAskedBlock = scApp.MapBLL.HasBlockControlAskedFromRedis
                        (eqpt.VEHICLE_ID, out string current_asked_block_id, out string current_asked_block_status);
                    if (hasAskedBlock)
                    {
                        bool isBlocking = SCUtility.isMatche(current_asked_block_status, SCAppConstants.BlockQueueState.Blocking)
                                       || SCUtility.isMatche(current_asked_block_status, SCAppConstants.BlockQueueState.Through);
                        //確認當前要的Block與目前Redis上存放的是不是同一個
                        if (SCUtility.isMatche(req_block_id, current_asked_block_id))
                        {
                            //如果要的是同一個，則確認是否已經給該台VH
                            if (isBlocking)
                            {
                                //如果已經給過該台VH通行權，則直接讓它通過。
                                canBlockPass = true;
                                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                                   Data: $"Vh:{eqpt.VEHICLE_ID} ask again block:{req_block_id},but it is the owner so ask result:{canBlockPass}",
                                   VehicleID: eqpt.VEHICLE_ID,
                                   CarrierID: eqpt.CST_ID);
                            }
                            else
                            {
                                //如果還沒有給過該台VH通行權，則需再判斷一次該Vh是否已經可以通過
                                canBlockPass = canPassBlockZone(eqpt, req_block_id);
                                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                                   Data: $"Vh:{eqpt.VEHICLE_ID} ask again block:{req_block_id},ask result:{canBlockPass}",
                                   VehicleID: eqpt.VEHICLE_ID,
                                   CarrierID: eqpt.CST_ID);
                                if (canBlockPass)
                                {
                                    scApp.MapBLL.updateBlockZoneQueue_BlockTime(eqpt.VEHICLE_ID, req_block_id);
                                    scApp.MapBLL.ChangeBlockControlStatus_Blocking(eqpt.VEHICLE_ID);
                                }
                            }
                        }
                        else
                        {
                            //如果不是同一個，則要判斷目前asked的Block狀態是否已經是Blocking或Through，                           
                            if (isBlocking)
                            {
                                //如果是的話才可以再進行新的BlockControlRequest的建立流程
                                canBlockPass = tryCreatBlockControlRequest(eqpt, req_block_id);
                                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                                   Data: $"Vh:{eqpt.VEHICLE_ID} already has a block:{current_asked_block_id}," +
                                   $"asking for another one at a time ,block:{req_block_id}, ask result:{canBlockPass}",
                                   VehicleID: eqpt.VEHICLE_ID,
                                   CarrierID: eqpt.CST_ID);
                            }
                            else
                            {
                                //如果不是，則不可以再給他另外一個Block
                                canBlockPass = false;
                                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                                   Data: $"Vh:{eqpt.VEHICLE_ID} already has a block:{current_asked_block_id}," +
                                   $"but the status is Request,so ask block:{req_block_id} result:{canBlockPass}",
                                   VehicleID: eqpt.VEHICLE_ID,
                                   CarrierID: eqpt.CST_ID);
                                DateTime reqest_time = DateTime.Now;
                                //scApp.MapBLL.doCreatBlockZoneQueueByReqStatus(eqpt.VEHICLE_ID, req_block_id, canBlockPass, reqest_time);
                                //scApp.MapBLL.CreatBlockControlKeyWordToRedis(eqpt.VEHICLE_ID, req_block_id, canBlockPass, reqest_time);
                            }
                        }
                    }
                    else
                    {
                        //如果目前Redis上沒有要求的Block的話，則可以嘗試建立新的BlocControlRequest，
                        //並判斷是否可以給其通行權
                        canBlockPass = tryCreatBlockControlRequest(eqpt, req_block_id);
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                           Data: $"Vh:{eqpt.VEHICLE_ID} ask block:{req_block_id},ask result:{canBlockPass}",
                           VehicleID: eqpt.VEHICLE_ID,
                           CarrierID: eqpt.CST_ID);
                    }
                }
            }
            return canBlockPass;
        }

        private bool ProcessBlockReqNew(BCFApplication bcfApp, AVEHICLE eqpt, string req_block_id)
        {
            bool canBlockPass = false;
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
               Data: $"Process block request,request block id:{req_block_id}",
               VehicleID: eqpt.VEHICLE_ID,
               CarrierID: eqpt.CST_ID);
            if (DebugParameter.isForcedPassBlockControl)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: "test flag: Force pass block control is open, will driect reply to vh can pass block",
                   VehicleID: eqpt.VEHICLE_ID,
                   CarrierID: eqpt.CST_ID);
                canBlockPass = true;
            }
            else if (DebugParameter.isForcedRejectBlockControl)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: "test flag: Force reject block control is open, will driect reply to vh can't pass block",
                   VehicleID: eqpt.VEHICLE_ID,
                   CarrierID: eqpt.CST_ID);
                canBlockPass = false;
            }
            else
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //先確認在Redis上是否該台VH 已經有要過的Block
                    //bool hasAskedBlock = scApp.MapBLL.HasBlockControlAskedFromRedis
                    //    (eqpt.VEHICLE_ID, out string current_asked_block_id, out string current_asked_block_status);
                    List<BLOCKZONEQUEUE> ask_block_queues = scApp.MapBLL.loadNonReleaseBlockQueueByCarID(eqpt.VEHICLE_ID);
                    bool hasAskedBlock = ask_block_queues != null && ask_block_queues.Count > 0;
                    if (hasAskedBlock)
                    {
                        //bool isBlocking = SCUtility.isMatche(current_asked_block_status, SCAppConstants.BlockQueueState.Blocking)
                        //               || SCUtility.isMatche(current_asked_block_status, SCAppConstants.BlockQueueState.Through);

                        //確認當前要的Block是否有存在目前的DB中。
                        BLOCKZONEQUEUE current_request_again_block_queue = ask_block_queues.
                                                                     Where(queue => SCUtility.isMatche(queue.ENTRY_SEC_ID, req_block_id)).
                                                                     FirstOrDefault();
                        //if (SCUtility.isMatche(req_block_id, current_asked_block_id))
                        if (current_request_again_block_queue != null)
                        {
                            //如果要的是同一個，則確認是否已經給該台VH
                            //if (isBlocking)
                            if (SCUtility.isMatche(current_request_again_block_queue.STATUS, SCAppConstants.BlockQueueState.Blocking) ||
                                SCUtility.isMatche(current_request_again_block_queue.STATUS, SCAppConstants.BlockQueueState.Through))
                            {
                                //如果已經給過該台VH通行權，則直接讓它通過。
                                canBlockPass = true;
                                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                                   Data: $"Vh:{eqpt.VEHICLE_ID} ask again block:{req_block_id},but it is the owner so ask result:{canBlockPass}",
                                   VehicleID: eqpt.VEHICLE_ID,
                                   CarrierID: eqpt.CST_ID);
                            }
                            else
                            {
                                //如果還沒有給過該台VH通行權，則需再判斷一次該Vh是否已經可以通過
                                canBlockPass = canPassBlockZone(eqpt, req_block_id);
                                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                                   Data: $"Vh:{eqpt.VEHICLE_ID} ask again block:{req_block_id},ask result:{canBlockPass}",
                                   VehicleID: eqpt.VEHICLE_ID,
                                   CarrierID: eqpt.CST_ID);
                                if (canBlockPass)
                                {
                                    scApp.MapBLL.updateBlockZoneQueue_BlockTime(eqpt.VEHICLE_ID, req_block_id);
                                    scApp.MapBLL.ChangeBlockControlStatus_Blocking(eqpt.VEHICLE_ID);
                                }
                            }
                        }
                        else
                        {
                            bool has_in_request = ask_block_queues.Where(queue => SCUtility.isMatche(queue.STATUS, SCAppConstants.BlockQueueState.Request))
                                                                  .Count() > 0;
                            string[] current_using_block_ids = ask_block_queues.Select(queue => queue.ENTRY_SEC_ID).ToArray();
                            //如果不是同一個，則要判斷目前asked的Blocks狀態是否沒有在Request中的                           
                            //if (isBlocking)
                            if (!has_in_request)
                            {
                                //如果是的話才可以再進行新的BlockControlRequest的建立流程
                                canBlockPass = tryCreatBlockControlRequest(eqpt, req_block_id);
                                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                                   Data: $"Vh:{eqpt.VEHICLE_ID} already has a block:{string.Join(",", current_using_block_ids)}," +
                                   $"asking for another one at a time ,block:{req_block_id}, ask result:{canBlockPass}",
                                   VehicleID: eqpt.VEHICLE_ID,
                                   CarrierID: eqpt.CST_ID);
                            }
                            else
                            {
                                //如果不是，則不可以再給他另外一個Block
                                canBlockPass = false;
                                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                                   Data: $"Vh:{eqpt.VEHICLE_ID} already has a block:{string.Join(",", current_using_block_ids)}," +
                                   $"but the status is Request,so ask block:{req_block_id} result:{canBlockPass}",
                                   VehicleID: eqpt.VEHICLE_ID,
                                   CarrierID: eqpt.CST_ID);
                                DateTime reqest_time = DateTime.Now;
                                //scApp.MapBLL.doCreatBlockZoneQueueByReqStatus(eqpt.VEHICLE_ID, req_block_id, canBlockPass, reqest_time);
                                //scApp.MapBLL.CreatBlockControlKeyWordToRedis(eqpt.VEHICLE_ID, req_block_id, canBlockPass, reqest_time);
                            }
                        }
                    }
                    else
                    {
                        //如果目前Redis上沒有要求的Block的話，則可以嘗試建立新的BlocControlRequest，
                        //並判斷是否可以給其通行權
                        canBlockPass = tryCreatBlockControlRequest(eqpt, req_block_id);
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                           Data: $"Vh:{eqpt.VEHICLE_ID} ask block:{req_block_id},ask result:{canBlockPass}",
                           VehicleID: eqpt.VEHICLE_ID,
                           CarrierID: eqpt.CST_ID);
                    }
                }
            }
            return canBlockPass;
        }

        private bool ProcessHIDRequest(BCFApplication bcfApp, AVEHICLE eqpt, string req_hid_secid)
        {
            bool isSuccess = true;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                DateTime req_dateTime = DateTime.Now;

                scApp.HIDBLL.doCreatHIDZoneQueueByReqStatus(eqpt.VEHICLE_ID, req_hid_secid, true, req_dateTime);
            }
            return isSuccess;
        }


        private bool tryCreatBlockControlRequest(AVEHICLE eqpt, string req_block_id)
        {
            bool canPass;
            ABLOCKZONEMASTER block_master = scApp.MapBLL.getBlockZoneMasterByEntrySecID(req_block_id);
            if (block_master != null)
            {
                //確認VH是否可以通過
                DateTime reqest_time = DateTime.Now;
                canPass = canPassBlockZone(eqpt, req_block_id);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: $"Vh:{eqpt.VEHICLE_ID} ask block:{req_block_id},ask result:{canPass}",
                   VehicleID: eqpt.VEHICLE_ID,
                   CarrierID: eqpt.CST_ID);
                scApp.MapBLL.doCreatBlockZoneQueueByReqStatus(eqpt.VEHICLE_ID, req_block_id, canPass, reqest_time);
                scApp.MapBLL.CreatBlockControlKeyWordToRedis(eqpt.VEHICLE_ID, req_block_id, canPass, reqest_time);
            }
            else
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: $"Vh:{eqpt.VEHICLE_ID} ask block:{req_block_id},but this block id not exist!",
                   VehicleID: eqpt.VEHICLE_ID,
                   CarrierID: eqpt.CST_ID);
                logger.Warn("vh:{0} req block id not exist {1}", eqpt.VEHICLE_ID, req_block_id);
                canPass = false;
            }

            return canPass;
        }

        private void PositionReport_HIDRequest(BCFApplication bcfApp, AVEHICLE eqpt, int seqNum, string req_hid_secid)
        {
            using (TransactionScope tx = SCUtility.getTransactionScope())
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    bool canHIDPass = true;
                    DateTime req_dateTime = DateTime.Now;
                    //canPass = scApp.HIDBLL.hasEnoughSeat(req_hid_id);
                    //if (canPass)
                    //{
                    //    scApp.HIDBLL.VHEntryHIDZone(req_hid_id);
                    //}

                    scApp.HIDBLL.doCreatHIDZoneQueueByReqStatus(eqpt.VEHICLE_ID, req_hid_secid, canHIDPass, req_dateTime);

                    Boolean resp_cmp = replyTranEventReport(bcfApp, EventType.Hidreq, eqpt, seqNum, canHIDPass: canHIDPass);
                    if (resp_cmp)
                    {
                        tx.Complete();
                        scApp.HIDBLL.VHEntryHIDZone(req_hid_secid);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            //Boolean resp_cmp = reply_Trans_Event_Report(bcfApp, EventType.Hidrelease, eqpt, seqNum, true);

            Task.Run(() => checkHIDSpaceIsSufficient(eqpt, req_hid_secid));
        }

        private void checkHIDSpaceIsSufficient(AVEHICLE eqpt, string req_hid_secid)
        {
            try
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                             Data: $"Start check HID:{req_hid_secid},has enough seat...",
                             VehicleID: eqpt.VEHICLE_ID,
                             CarrierID: eqpt.CST_ID);
                bool isEnough = scApp.HIDBLL.hasEnoughSeat(req_hid_secid, out long current_vh_count, out int hid_zone_max_load_count);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: $"Check HID:{req_hid_secid} has enough seat,{nameof(current_vh_count)}:{current_vh_count},{nameof(hid_zone_max_load_count)}:{hid_zone_max_load_count},result:{isEnough}",
                   VehicleID: eqpt.VEHICLE_ID,
                   CarrierID: eqpt.CST_ID);
                if (!isEnough)
                {
                    using (TransactionScope tx = SCUtility.getTransactionScope())
                    {
                        using (DBConnection_EF con = DBConnection_EF.GetUContext())
                        {
                            scApp.HIDBLL.updateHIDZoneQueue_Pasue(eqpt.VEHICLE_ID, req_hid_secid, true);
                            //if (eqpt.sned_Str39(PauseEvent.Pause, PauseType.Hid))
                            if (PauseRequest(eqpt.VEHICLE_ID, PauseEvent.Pause, OHxCPauseType.ManualHID))
                            {
                                tx.Complete();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void PositionReport_ArriveAndComplete(BCFApplication bcfApp, AVEHICLE eqpt, int seqNum
                                                    , EventType eventType, string current_adr_id, string current_sec_id, string carrier_id
                                                    , string load_port_id, string unload_port_id) //B0.01 
        {
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
               Data: $"Process report {eventType}",
               VehicleID: eqpt.VEHICLE_ID,
               CarrierID: eqpt.CST_ID);
            //using (TransactionScope tx = new TransactionScope())
            //using (TransactionScope tx = new
            //    TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))


            switch (eventType)
            {
                case EventType.LoadArrivals:
                    if (!SCUtility.isEmpty(eqpt.MCS_CMD))
                    {
                        scApp.CMDBLL.updateCMD_MCS_CmdStatus2LoadArrivals(eqpt.MCS_CMD);
                    }
                    scApp.CMDBLL.setWillPassSectionInfo(eqpt.VEHICLE_ID, eqpt.PredictSectionsToDesination);
                    scApp.VIDBLL.upDateVIDPortID(eqpt.VEHICLE_ID, eqpt.CUR_ADR_ID);
                    scApp.ReserveBLL.RemoveAllReservedSectionsByVehicleID(eqpt.VEHICLE_ID);
                    scApp.ReserveBLL.TryAddReservedSection(eqpt.VEHICLE_ID, eqpt.CUR_SEC_ID);
                    break;
                case EventType.UnloadArrivals:
                    if (!SCUtility.isEmpty(eqpt.MCS_CMD))
                    {
                        scApp.CMDBLL.updateCMD_MCS_CmdStatus2UnloadArrive(eqpt.MCS_CMD);
                    }
                    scApp.VIDBLL.upDateVIDPortID(eqpt.VEHICLE_ID, eqpt.CUR_ADR_ID);
                    scApp.ReserveBLL.RemoveAllReservedSectionsByVehicleID(eqpt.VEHICLE_ID);
                    scApp.ReserveBLL.TryAddReservedSection(eqpt.VEHICLE_ID, eqpt.CUR_SEC_ID);
                    break;
                case EventType.LoadComplete:
                    scApp.CMDBLL.setWillPassSectionInfo(eqpt.VEHICLE_ID, eqpt.PredictSectionsToDesination);
                    scApp.VIDBLL.upDateVIDCarrierLocInfo(eqpt.VEHICLE_ID, eqpt.Real_ID);
                    if (!SCUtility.isEmpty(eqpt.MCS_CMD))
                    {
                        //scApp.CMDBLL.updateCMD_MCS_TranStatus2Transferring(eqpt.MCS_CMD);
                        //scApp.CMDBLL.updateCMD_MCS_CmdStatus2LoadComplete(eqpt.MCS_CMD);
                    }
                    //scApp.PortBLL.OperateCatch.updatePortStationCSTExistStatus(eqpt.CUR_ADR_ID, string.Empty);
                    //CarrierInterfaceSim_LoadComplete(eqpt);
                    break;
                case EventType.UnloadComplete:
                    if (!SCUtility.isEmpty(eqpt.MCS_CMD))
                    {
                        scApp.CMDBLL.updateCMD_MCS_CmdStatus2UnloadComplete(eqpt.MCS_CMD);
                    }
                    var port_station = scApp.MapBLL.getPortByAdrID(current_adr_id);//要考慮到一個Address會有多個Port的問題
                    if (port_station != null)
                    {
                        scApp.VIDBLL.upDateVIDCarrierLocInfo(eqpt.VEHICLE_ID, port_station.PORT_ID);
                    }
                    //scApp.PortBLL.OperateCatch.updatePortStationCSTExistStatus(eqpt.CUR_ADR_ID, carrier_id);
                    // CarrierInterfaceSim_UnloadComplete(eqpt, eqpt.CST_ID);
                    break;
            }

            List<AMCSREPORTQUEUE> reportqueues = new List<AMCSREPORTQUEUE>();
            using (TransactionScope tx = SCUtility.getTransactionScope())
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //using (TransactionScope tx = new
                    //    TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = SCAppConstants.ISOLATION_LEVEL }))
                    //con.BeginTransaction();
                    //if (!SCUtility.isEmpty(eqpt.MCS_CMD))
                    //{
                    //    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                    //       Data: $"do report {eventType} to mcs.",
                    //       VehicleID: eqpt.VEHICLE_ID,
                    //       CarrierID: eqpt.CST_ID);
                    //    bool isCreatReportInfoSuccess = false;
                    //    switch (eventType)
                    //    {
                    //        case EventType.LoadArrivals:
                    //            isCreatReportInfoSuccess = scApp.ReportBLL.newReportLoadArrivals(eqpt.VEHICLE_ID, reportqueues);
                    //            break;
                    //        case EventType.LoadComplete:
                    //            isCreatReportInfoSuccess = true;
                    //            break;
                    //        case EventType.UnloadArrivals:
                    //            isCreatReportInfoSuccess = scApp.ReportBLL.newReportUnloadArrivals(eqpt.VEHICLE_ID, reportqueues);
                    //            break;
                    //        case EventType.UnloadComplete:
                    //            isCreatReportInfoSuccess = true;
                    //            break;
                    //        default:
                    //            isCreatReportInfoSuccess = true;
                    //            break;
                    //    }
                    //    if (!isCreatReportInfoSuccess)
                    //    {
                    //        return;
                    //    }
                    //    scApp.ReportBLL.insertMCSReport(reportqueues);
                    //}

                    Boolean resp_cmp = replyTranEventReport(bcfApp, eventType, eqpt, seqNum);

                    if (resp_cmp)
                    {
                        tx.Complete();
                    }
                    else
                    {
                        //con.Rollback();
                        return;
                    }
                }
            }
            scApp.ReportBLL.newSendMCSMessage(reportqueues);
            //SpinWait.SpinUntil(() => false, 2000);
            switch (eventType)
            {
                case EventType.LoadArrivals:
                    scApp.TransferService.OHT_TransferStatus(eqpt.OHTC_CMD,
                                    eqpt.VEHICLE_ID, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_LOAD_ARRIVE);
                    scApp.VehicleBLL.doLoadArrivals(eqpt.VEHICLE_ID, current_adr_id, current_sec_id);
                    break;
                case EventType.LoadComplete:
                    //scApp.TransferService.BoxLocationChange_LoadComplete(carrier_id, eqpt.VEHICLE_ID); //B0.01 
                    scApp.TransferService.OHT_TransferStatus(eqpt.OHTC_CMD,
                                    eqpt.VEHICLE_ID, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE);
                    scApp.VehicleBLL.doLoadComplete(eqpt.VEHICLE_ID, current_adr_id, current_sec_id, carrier_id);
                    break;
                case EventType.UnloadArrivals:
                    scApp.TransferService.OHT_TransferStatus(eqpt.OHTC_CMD,
                                    eqpt.VEHICLE_ID, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_UNLOAD_ARRIVE);
                    scApp.VehicleBLL.doUnloadArrivals(eqpt.VEHICLE_ID, current_adr_id, current_sec_id);
                    break;
                case EventType.UnloadComplete:
                    //scApp.TransferService.BoxLocationChange_UnloadComplete(carrier_id, unload_port_id); //B0.01 
                    //*********************
                    //B0.06 use for test the ignore the unload complete and command complete when the dest is not shelf
                    if (DebugParameter.ignore136UnloadComplete == true)
                    {
                        string ohtcCmdID = eqpt.OHTC_CMD;
                        ACMD_MCS cmd = new ACMD_MCS();
                        cmd = scApp.CMDBLL.getCMD_ByOHTName(eqpt.VEHICLE_ID).FirstOrDefault();
                        //if the dest is shelf
                        if (cmd.HOSTDESTINATION.StartsWith("10") ||
                            cmd.HOSTDESTINATION.StartsWith("11") ||
                            cmd.HOSTDESTINATION.StartsWith("20") ||
                            cmd.HOSTDESTINATION.StartsWith("21"))
                        {
                            scApp.TransferService.OHT_TransferStatus(ohtcCmdID,
                                    eqpt.VEHICLE_ID, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_UNLOAD_COMPLETE);
                            scApp.VehicleBLL.doUnloadComplete(eqpt.VEHICLE_ID);

                            //SpinWait.SpinUntil(() => false, 100);
                            //Report this for the Wait out signal for MCS
                            //scApp.TransferService.OHT_TransferStatus(ohtcCmdID,
                            //                eqpt.VEHICLE_ID, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH);
                        }
                        else //if the dest isn't shelf
                        {
                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                            Data: $"Enter the port ignore place.");
                            // Don't report the OHT_TransferStatus ;
                        }
                    }
                    else
                    {
                        scApp.TransferService.OHT_TransferStatus(eqpt.OHTC_CMD,
                                    eqpt.VEHICLE_ID, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_UNLOAD_COMPLETE);
                        scApp.VehicleBLL.doUnloadComplete(eqpt.VEHICLE_ID);

                        //SpinWait.SpinUntil(() => false, 100);
                        //Report this for the Wait out signal for MCS
                        //scApp.TransferService.OHT_TransferStatus(eqpt.OHTC_CMD,
                        //                eqpt.VEHICLE_ID, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH);
                    }
                    break;
            }
        }

        private void PositionReport_AdrPassArrivals(BCFApplication bcfApp, AVEHICLE eqpt, ID_134_TRANS_EVENT_REP recive_str, string last_adr_id, string last_sec_id)
        {
            string current_adr_id = recive_str.CurrentAdrID;
            string current_sec_id = recive_str.CurrentSecID;
            switch (recive_str.EventType)
            {
                case EventType.AdrPass:
                    //updateCMDDetailEntryAndLeaveTime(eqpt, current_adr_id, current_sec_id, last_adr_id, last_sec_id);
                    //TODO 要改成直接查詢Queue的Table就好，不用再帶SEC ID進去。
                    lock (eqpt.BlockControl_SyncForRedis)
                    {
                        if (scApp.MapBLL.IsBlockControlStatus
                            (eqpt.VEHICLE_ID, SCAppConstants.BlockQueueState.Blocking))
                        {
                            BLOCKZONEQUEUE throuBlockQueue = null;
                            if (scApp.MapBLL.updateBlockZoneQueue_ThrouTime(eqpt.VEHICLE_ID, out throuBlockQueue))
                            {
                                scApp.MapBLL.ChangeBlockControlStatus_Through(eqpt.VEHICLE_ID);
                            }
                        }
                    }
                    //BLOCKZONEQUEUE throuBlockQueue = null;
                    //scApp.MapBLL.updateBlockZoneQueue_ThrouTime(eqpt.VEHICLE_ID, out throuBlockQueue);
                    //if (throuBlockQueue != null)
                    //    return;
                    break;
                case EventType.AdrOrMoveArrivals:
                    scApp.VehicleBLL.doAdrArrivals(eqpt.VEHICLE_ID, current_adr_id, current_sec_id);
                    break;
            }
        }
        private bool replyTranEventReport(BCFApplication bcfApp, EventType eventType, AVEHICLE eqpt, int seq_num,
                                          bool canBlockPass = false, bool canHIDPass = false, bool canReservePass = false,
                                          string renameCarrierID = "", CMDCancelType cancelType = CMDCancelType.CmdNone,
                                          RepeatedField<ReserveInfo> reserveInfos = null)
        {
            ID_36_TRANS_EVENT_RESPONSE send_str = new ID_36_TRANS_EVENT_RESPONSE
            {
                IsBlockPass = canBlockPass ? PassType.Pass : PassType.Block,
                IsHIDPass = canHIDPass ? PassType.Pass : PassType.Block,
                IsReserveSuccess = canReservePass ? ReserveResult.Success : ReserveResult.Unsuccess,
                ReplyCode = 0,
                RenameBOXID = renameCarrierID,
                ReplyActiveType = cancelType,
            };
            if (reserveInfos != null)
            {
                send_str.ReserveInfos.AddRange(reserveInfos);
            }
            WrapperMessage wrapper = new WrapperMessage
            {
                SeqNum = seq_num,
                ImpTransEventResp = send_str
            };
            //Boolean resp_cmp = ITcpIpControl.sendGoogleMsg(bcfApp, eqpt.TcpIpAgentName, wrapper, true);

            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
              seq_num: seq_num, Data: send_str,
              VehicleID: eqpt.VEHICLE_ID,
              CarrierID: eqpt.CST_ID);
            Boolean resp_cmp = eqpt.sendMessage(wrapper, true);
            SCUtility.RecodeReportInfo(eqpt.VEHICLE_ID, seq_num, send_str, resp_cmp.ToString());
            return resp_cmp;
        }
        private bool checkHasOrtherBolckZoneQueueNonRelease(AVEHICLE eqpt, out List<BLOCKZONEQUEUE> blockZoneQueues)
        {
            blockZoneQueues = scApp.MapBLL.loadNonReleaseBlockQueueByCarID(eqpt.VEHICLE_ID);
            if (blockZoneQueues != null && blockZoneQueues.Count > 0)
            {
                //foreach (BLOCKZONEQUEUE queue in blockZoneQueues)
                //{
                //    scApp.MapBLL.updateBlockZoneQueue_AbnormalEnd(queue,
                //        SCAppConstants.BlockQueueState.Abnormal_Release_TimerCheck);
                //    string entry_sec_id = queue.ENTRY_SEC_ID;
                //    Task.Run(() => scApp.MapBLL.CheckAndNoticeBlockVhPassByEntrySecID(entry_sec_id));
                //}
                return true;
            }
            else
            {
                blockZoneQueues = null;
                return false;
            }
        }

        /// <summary>
        /// 用來確保Block Request、Block Release處理的先後順序
        /// </summary>
        private object block_control_lock_obj = new object();
        private bool canPassBlockZone(AVEHICLE vh, string block_sec_id)
        {
            lock (block_control_lock_obj)
            {

                //要透過VH的Curent Segment來確定他是否為該Segment的當前第一台VH
                ASEGMENT current_segment = scApp.SegmentBLL.cache.GetSegment(vh.CUR_SEG_ID);
                var check_first_vh_is_in_segment = current_segment.IsFirst(vh);
                if (!check_first_vh_is_in_segment.isFirst)
                {
                    AVEHICLE first_vh = check_first_vh_is_in_segment.firstVh;
                    string first_vh_id = first_vh == null ? string.Empty : first_vh.VEHICLE_ID;
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                       Data: $"Vh:{vh.VEHICLE_ID} ask block:{block_sec_id},but not first vh in segment id:{vh.CUR_SEG_ID}," +
                             $"current first vh id:{first_vh_id}",
                       VehicleID: vh.VEHICLE_ID,
                       CarrierID: vh.CST_ID);
                    return false;
                }
                //if (!isRepeatReqBlock)
                //{
                //    //0-1 先確認他所上報的SEC ID 是Block的SEC ID
                //    ABLOCKZONEMASTER block_master = scApp.MapBLL.getBlockZoneMasterByEntrySecID(block_sec_id);
                //    if (block_master == null)
                //    {
                //        Console.WriteLine("not block sec id {0}", block_sec_id);
                //        return false;
                //    }
                //    //0.更新BLOCKZONEQUEUE這個Table，新增一筆ENTRY_SEC_ID並填入CAR_ID、REQ_TIME
                //    scApp.MapBLL.addBlockZoneQueue(eqpt.VEHICLE_ID, block_sec_id);
                //}
                //1.查詢VH所上報的block_sec_id 是哪一個，並利用他去查詢跟他同一組的SEC_ID，
                //  ABLOCKZONEDETAIL 會記錄。
                List<string> lstSecid = scApp.MapBLL.loadBlockZoneDetailSecIDsByEntrySecID(block_sec_id);
                bool hasBlocking = false;
                string orther_vh = "";
                //1.2找出BLOCKZONEQUEUE中是否有這兩個EntrySecID
                //foreach (string sec_id in lstSecid)
                //{
                //    if (scApp.MapBLL.checkBlockZoneQueueIsBlockingByEntrySecID(sec_id))
                //    {
                //        hasBlocking = true;
                //        break;
                //    }
                //}
                //TODO 因為沒有檢查Requestung 所以 可能會有一次給兩台的疑慮
                //if (scApp.MapBLL.checkBlockZoneQueueIsBlockingByEntrySecID(lstSecid))
                if (scApp.MapBLL.checkBlockZoneQueueIsBlockingByEntrySecID(lstSecid, out List<BLOCKZONEQUEUE> queues))
                {
                    foreach (var queue in queues)
                    {
                        if (SCUtility.isMatche(queue.CAR_ID, vh.VEHICLE_ID))
                        {
                            bool isBlocking = SCUtility.isMatche(queue.STATUS, SCAppConstants.BlockQueueState.Blocking)
                                           || SCUtility.isMatche(queue.STATUS, SCAppConstants.BlockQueueState.Through);
                            if (isBlocking)
                            {
                                hasBlocking = false;
                            }
                            else
                            {
                                hasBlocking = true;
                                orther_vh = queue.CAR_ID;
                                break;
                            }
                        }
                        else
                        {
                            hasBlocking = true;
                            orther_vh = queue.CAR_ID;
                            break;
                        }
                    }
                    if (hasBlocking)
                    {
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                           Data: $"Vh:{vh.VEHICLE_ID} ask block:{block_sec_id},but queue has orther vh:{orther_vh} request",
                           VehicleID: vh.VEHICLE_ID,
                           CarrierID: vh.CST_ID);
                    }
                }


                if (!hasBlocking)
                {
                    //2.查詢AVEHICLE中的所有VH的SEC ID 是不是有在這組之中，
                    //  有 - 回復的IS_BLOCK_PASS要填入 1 
                    //  無 - 回復的IS_BLOCK_PASS要填入 0 
                    foreach (string sec_id in lstSecid)
                    {
                        List<AVEHICLE> vehicles = scApp.VehicleBLL.loadVehicleBySEC_ID(sec_id);
                        if (vehicles != null)
                        {
                            if (vehicles.Count == 1)
                            {
                                if (SCUtility.isMatche(vh.VEHICLE_ID, vehicles[0].VEHICLE_ID))
                                {
                                    //如果進來問的是自己已經在Block上的話，還是可以給他通行權。
                                }
                                else
                                {
                                    //如果不是則代表有其他車輛在Block內，就不可以給他通行權
                                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                                       Data: $"Vh:{vh.VEHICLE_ID} ask block:{block_sec_id},but has vh:[{vehicles[0].VEHICLE_ID}] in current block",
                                       VehicleID: vh.VEHICLE_ID,
                                       CarrierID: vh.CST_ID);
                                    hasBlocking = true;
                                    break;
                                }
                            }
                            else if (vehicles.Count > 1)
                            {
                                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                                   Data: $"Vh:{vh.VEHICLE_ID} ask block:{block_sec_id},but has more vh in current block",
                                   VehicleID: vh.VEHICLE_ID,
                                   CarrierID: vh.CST_ID);
                                //通常Block內部最多只能有一台車，大於1台車的話一定是有問題，所以就不給他通行權
                                hasBlocking = true;
                                break;
                            }
                        }
                        //if (vehicles != null && vehicles.Count > 0)
                        //{
                        //    hasBlocking = true;
                        //    break;
                        //}
                    }
                    //foreach (string sec_id in lstSecid)
                    //{
                    //    List<AVEHICLE> vehicles = scApp.VehicleBLL.loadVehicleBySEC_ID(sec_id);
                    //    if (vehicles != null && vehicles.Count > 0)
                    //    {
                    //        foreach (AVEHICLE vh in vehicles)
                    //        {
                    //            if (scApp.MapBLL.HasBlockControlAskedFromRedis
                    //            (vh.VEHICLE_ID))
                    //            {
                    //                hasBlocking = true;
                    //                break;
                    //            }

                    //        }
                    //        if (hasBlocking)
                    //        {
                    //            break;
                    //        }
                    //    }
                    //}
                }
                return !hasBlocking;
            }
        }

        public void blockZoneReleaseScript(string blockmaster_id)
        {
            ABLOCKZONEMASTER blockmaster = scApp.MapBLL.getBlockZoneMasterByEntrySecID(blockmaster_id);
            if (blockmaster != null)
            {
                blockZoneReleaseScript(blockmaster);
            }
        }
        public void blockZoneReleaseScript(ABLOCKZONEMASTER blockmaster)
        {
            //if (System.Threading.Interlocked.Exchange(ref blockZoneScriptSyncPoint, 1) == 0)
            //{
            lock (block_control_lock_obj)
            {
                try
                {
                    //using (DBConnection_EF con = new DBConnection_EF())
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        //if (releaseBlockZone(vh_id, current_adr_id))
                        //{
                        //    Console.WriteLine("release block,leave adr:{0}",
                        //                       current_adr_id);
                        //    scApp.MapBLL.CheckAndNoticeBlockVhPassByAdrID(current_adr_id);
                        //}
                        (bool has_find, BLOCKZONEQUEUE wait_block_queue) = scApp.MapBLL.NoticeBlockVhPassByEntrySecID(blockmaster.ENTRY_SEC_ID);
                        if (has_find)
                        {
                            AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(wait_block_queue.CAR_ID);
                            string block_zone_id = wait_block_queue.ENTRY_SEC_ID;
                            bool canPass = canPassBlockZone(vh, block_zone_id);
                            if (!vh.IsBlocking)
                            {
                                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                                   Data: $"Vh:{vh.VEHICLE_ID} ask block:{block_zone_id} check can pass result:{canPass}," +
                                         $"vh of blocking single:{vh.IsBlocking},not notice vh pass",
                                   VehicleID: vh.VEHICLE_ID,
                                   CarrierID: vh.CST_ID);
                                return;
                            }
                            if (canPass)
                            {
                                //scApp.VehicleBLL.noticeVhPass(wait_block_queue);
                                noticeVhPass(wait_block_queue);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
                finally
                {
                    //System.Threading.Interlocked.Exchange(ref blockZoneScriptSyncPoint, 0);
                }
            }
            //}
        }

        public void noticeVhPass(string vh_id)
        {
            BLOCKZONEQUEUE usingBlockQueue = scApp.MapBLL.getUsingBlockZoneQueueByVhID(vh_id);
            if (usingBlockQueue != null)
            {
                noticeVhPass(usingBlockQueue);
            }
            else
            {
                string reason = string.Empty;
                PauseRequest(vh_id, PauseEvent.Continue, SCAppConstants.OHxCPauseType.Block);
            }
        }

        public bool noticeVhPass(BLOCKZONEQUEUE blockZoneQueue)
        {
            string notice_vh_id = blockZoneQueue.CAR_ID.Trim();
            string req_block_id = blockZoneQueue.ENTRY_SEC_ID.Trim();

            bool isSuccess = false;
            using (TransactionScope tx = SCUtility.getTransactionScope())
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    scApp.MapBLL.updateBlockZoneQueue_BlockTime(notice_vh_id, req_block_id);
                    scApp.MapBLL.ChangeBlockControlStatus_Blocking(notice_vh_id);

                    isSuccess = PauseRequest(notice_vh_id, PauseEvent.Continue, SCAppConstants.OHxCPauseType.Block);
                    if (isSuccess)
                    {
                        tx.Complete();
                    }
                }
            }

            //bool isSuccess = scApp.VehicleService.PauseRequest(notice_vh_id, PauseEvent.Continue, SCAppConstants.OHxCPauseType.Block);
            //if (isSuccess)
            //{
            //    if (scApp.MapBLL.IsBlockControlStatus
            //          (notice_vh_id, SCAppConstants.BlockQueueState.Request))
            //    {
            //        scApp.MapBLL.updateBlockZoneQueue_BlockTime(notice_vh_id, req_block_id);
            //        scApp.MapBLL.ChangeBlockControlStatus_Blocking(notice_vh_id);
            //    }
            //}
            //else
            //{
            //}
            return isSuccess;
        }


        public void hidZoneReleaseScript(AHIDZONEMASTER hidmaster)
        {
            try
            {
                using (TransactionScope tx = SCUtility.getTransactionScope())
                {
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        AHIDZONEQUEUE hid_queue = scApp.HIDBLL.getHIDZoneQueue_FirstReqInPasue(hidmaster.ENTRY_SEC_ID);
                        if (hid_queue != null)
                        {
                            scApp.HIDBLL.updateHIDZoneQueue_Pasue(hid_queue.VEHICLE_ID, hid_queue.ENTRY_SEC_ID, false);
                            string notice_vh_id = hid_queue.VEHICLE_ID;
                            //if (scApp.VehicleBLL.noticeVhPass(hid_queue))
                            if (PauseRequest(notice_vh_id, PauseEvent.Continue, OHxCPauseType.ManualHID))
                            {
                                tx.Complete();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }



        public void CheckBlockControlByVehicleView()
        {
            try
            {
                List<AVEHICLE> lstVH = scApp.getEQObjCacheManager().getAllVehicle();
                //1.先在Redis找出有Req BlockZone的
                foreach (AVEHICLE vh in lstVH)
                {
                    if (vh.isTcpIpConnect &&
                        (vh.MODE_STATUS == VHModeStatus.AutoLocal ||
                        vh.MODE_STATUS == VHModeStatus.AutoRemote) &&
                        vh.IsBlocking
                        )
                    {
                        string block_zone_id = string.Empty;
                        string block_status = string.Empty;
                        //var non_release_block = scApp.MapBLL.loadNonReleaseBlockQueueByCarID(vh.VEHICLE_ID);
                        var requesting_block_zone = scApp.MapBLL.getBlockQueueInRequestByCarID(vh.VEHICLE_ID);
                        if (requesting_block_zone != null)
                        {

                            //if (scApp.MapBLL.tryGetInRequest(vh.VEHICLE_ID, out block_zone_id, out block_status))
                            //{
                            block_zone_id = requesting_block_zone.ENTRY_SEC_ID;
                            //2.透過該BlockZone去找出能否通過
                            bool canPass = canPassBlockZone(vh, block_zone_id);
                            if (canPass)
                            {
                                //3.若可以則再嘗試通知
                                blockZoneReleaseScript(block_zone_id);
                                logger.Warn($"vh id [{vh.VEHICLE_ID}] ,block notice pass by timer-block check. block id [{block_zone_id}]");
                            }
                            //}
                        }

                    }

                }



            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        public bool tryReleaseBlockZone(string vh_id, string release_adr, bool isThrowException, out ABLOCKZONEMASTER releaseBlockMaster)
        {
            bool hasRelease = false;
            //if (SCUtility.isEmpty(eqpt.currentBlockID))
            //    return;
            //1.用VHID 與CurrentBlockID 查詢BLOCKZONEQUEUE 找出為2或3的
            //BLOCKZONEQUEUE blockZoneQueue = scApp.MapBLL.getBlockZoneQueueByVhIDAndCrtBlockSecID(eqpt.VEHICLE_ID, eqpt.currentBlockID);
            //若有 再用Adr找出ABLOCKZONEMASTER是否有符合的
            //if (blockZoneQueue != null)
            AVEHICLE vh_vo = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            lock (vh_vo.BlockControl_SyncForRedis)
            {
                //if (scApp.MapBLL.IsBeforeBlockControlStatus(vh_id, SCAppConstants.BlockQueueState.Release))
                //{
                LogCollection.BlockControlLogger.Trace($"vh[{vh_id}],Release Block Control Step 1");
                //BLOCKZONEQUEUE blockZoneQueue = scApp.MapBLL.getUsingBlockZoneQueueByVhID(vh_id);
                List<BLOCKZONEQUEUE> blockZoneQueues = scApp.MapBLL.loadNonReleaseBlockQueueByCarID(vh_id);
                if (blockZoneQueues == null)
                {
                    if (isThrowException)
                        throw new NullReferenceException($"error function [{nameof(tryReleaseBlockZone)}],msg[{nameof(blockZoneQueues)} is null]");
                    else
                    {
                        releaseBlockMaster = null;
                        return false;
                    }
                }
                List<string> block_zone_ids = blockZoneQueues.Select(queue => queue.ENTRY_SEC_ID.Trim()).ToList();
                LogCollection.BlockControlLogger.Trace($"vh[{vh_id}],Release Block Control Step 2,queue entry sec id[{string.Join(",", block_zone_ids)}");
                //releaseBlockMaster = scApp.MapBLL.getBlockZoneMasterByBlockIDAndAdrID(blockZoneQueue.ENTRY_SEC_ID.Trim(), release_adr);
                releaseBlockMaster = scApp.MapBLL.getCurrentReleaseBlock(block_zone_ids, release_adr);
                if (releaseBlockMaster == null)
                {
                    if (isThrowException)
                        throw new NullReferenceException($"error function [{nameof(tryReleaseBlockZone)}],msg[{nameof(releaseBlockMaster)} is null]");
                    else
                    {
                        releaseBlockMaster = null;
                        return false;
                    }
                }
                if (releaseBlockMaster != null)
                {
                    LogCollection.BlockControlLogger.Trace($"vh[{vh_id}],Release Block Control Step 3,master entry sec id[{releaseBlockMaster.ENTRY_SEC_ID.Trim()}]");
                    LogCollection.BlockControlLogger.Trace($"vh[{vh_id}],Beging Relsase,entry sec id[{releaseBlockMaster.ENTRY_SEC_ID.Trim()}]");
                    using (TransactionScope tx = SCUtility.getTransactionScope())
                    {
                        using (DBConnection_EF con = DBConnection_EF.GetUContext())
                        {
                            scApp.MapBLL.updateBlockZoneQueue_ReleasTime(vh_id, releaseBlockMaster.ENTRY_SEC_ID.Trim());
                            //scApp.MapBLL.DeleteBlockControlKeyWordToRedis(vh_id);
                            scApp.MapBLL.DeleteBlockControlKeyWordToRedis(vh_id, releaseBlockMaster.ENTRY_SEC_ID.Trim());
                            tx.Complete();
                        }
                    }
                    hasRelease = true;
                    LogCollection.BlockControlLogger.Trace($"vh[{vh_id}],End Relsase,entry sec id[{releaseBlockMaster.ENTRY_SEC_ID.Trim()}]");
                }
                //}
                //else
                //{
                //    releaseBlockMaster = null;
                //}
            }
            return hasRelease;
        }

        public bool tryReleaseHIDZone(string vh_id, string release_adr, out AHIDZONEMASTER releaseHIDMaster)
        {
            bool hasRelease = false;
            releaseHIDMaster = null;
            AVEHICLE vh_vo = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
            AHIDZONEQUEUE hidZoneQueue = scApp.HIDBLL.getUsingHIDZoneQueueByVhID(vh_id);
            if (hidZoneQueue == null) { return true; }
            releaseHIDMaster = scApp.HIDBLL.GetHidZoneMaster(hidZoneQueue.ENTRY_SEC_ID, release_adr);
            if (releaseHIDMaster == null) { return true; }

            using (TransactionScope tx = SCUtility.getTransactionScope())
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    scApp.HIDBLL.updateHIDZoneQueue_ReleasTime(vh_id, hidZoneQueue.ENTRY_SEC_ID.Trim());
                    scApp.HIDBLL.VHLeaveHIDZone(hidZoneQueue.ENTRY_SEC_ID.Trim());
                    tx.Complete();
                    hasRelease = true;
                }
            }
            return hasRelease;
        }

        private void PositionReport_BlockRelease(BCFApplication bcfApp, AVEHICLE eqpt, ID_136_TRANS_EVENT_REP recive_str, int seq_num)
        {
            string release_adr = recive_str.ReleaseBlockAdrID;
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
               Data: $"Process block release,release address id:{release_adr}",
               VehicleID: eqpt.VEHICLE_ID,
               CarrierID: eqpt.CST_ID);
            doBlockRelease(eqpt, release_adr);
            //replyTranEventReport(bcfApp, recive_str.EventType, eqpt, seq_num);
        }

        private (bool hasRelease, ABLOCKZONEMASTER releaseBlockMaster) doBlockRelease(AVEHICLE eqpt, string release_adr)
        {
            return doBlockRelease(eqpt, release_adr, true);
        }
        public (bool hasRelease, ABLOCKZONEMASTER releaseBlockMaster) doBlockRelease(string vhID, string release_adr, bool isThrowException)
        {
            AVEHICLE vh = scApp.VehicleBLL.cache.getVhByID(vhID);
            if (vh == null)
            {
                throw new NullReferenceException($"vh:{vhID} not exist.");
            }
            return doBlockRelease(vh, release_adr, isThrowException);
        }
        private (bool hasRelease, ABLOCKZONEMASTER releaseBlockMaster) doBlockRelease(AVEHICLE eqpt, string release_adr, bool isThrowException)
        {
            ABLOCKZONEMASTER releaseBlockMaster = null;
            bool hasRelease = false;
            try
            {
                hasRelease = tryReleaseBlockZone(eqpt.VEHICLE_ID, release_adr, isThrowException, out releaseBlockMaster);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: $"Process block release, release address id:{release_adr}, release result:{hasRelease}",
                   VehicleID: eqpt.VEHICLE_ID,
                   CarrierID: eqpt.CST_ID);
                if (hasRelease)
                {
                    Task.Run(() => blockZoneReleaseScript(releaseBlockMaster));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: ex,
                   VehicleID: eqpt.VEHICLE_ID,
                   CarrierID: eqpt.CST_ID);
                logger.Warn(ex, "Warn");
            }
            return (hasRelease, releaseBlockMaster);
        }

        private void PositionReport_HIDRelease(BCFApplication bcfApp, AVEHICLE eqpt, ID_136_TRANS_EVENT_REP recive_str, int seq_num)
        {
            try
            {
                string release_adr = recive_str.ReleaseHIDAdrID;
                AHIDZONEMASTER releaseHIDMaster = null;
                if (tryReleaseHIDZone(eqpt.VEHICLE_ID, release_adr, out releaseHIDMaster))
                {
                    if (scApp.HIDBLL.hasEnoughSeat(releaseHIDMaster.ENTRY_SEC_ID.Trim(), out long current_vh_count, out int hid_zone_max_load_count))
                    {
                        Task.Run(() => hidZoneReleaseScript(releaseHIDMaster));
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: ex,
                   VehicleID: eqpt.VEHICLE_ID,
                   CarrierID: eqpt.CST_ID);
            }
            // replyTranEventReport(bcfApp, recive_str.EventType, eqpt, seq_num);
        }

        private void PositionReport_DoubleStorage(BCFApplication bcfApp, AVEHICLE eqpt, int seqNum
                                                    , EventType eventType, string current_adr_id, string current_sec_id, string carrier_id)
        {
            try
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                Data: $"Process report {eventType}",
                VehicleID: eqpt.VEHICLE_ID,
                CarrierID: eqpt.CST_ID);

                if (!SCUtility.isEmpty(eqpt.MCS_CMD))
                {
                    bool retryOrAbort = true;
                    retryOrAbort = scApp.TransferService.OHT_TransferStatus(eqpt.OHTC_CMD,
                            eqpt.VEHICLE_ID, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_DOUBLE_STORAGE);
                    Boolean resp_cmp;
                    //if (retryOrAbort == true)
                    //{
                    resp_cmp = replyTranEventReport(bcfApp, eventType, eqpt, seqNum, true, true, true, "", CMDCancelType.CmdCancel);
                    //}
                    //else
                    //{
                    //    resp_cmp = replyTranEventReport(bcfApp, eventType, eqpt, seqNum, true, true, "", CMDCancelType.CmdRetry);
                    //}
                }
                else
                {
                    //if (DebugParameter.Is_136_empty_double_retry)
                    //{
                    //    replyTranEventReport(bcfApp, eventType, eqpt, seqNum, true, true, "", CMDCancelType.CmdRetry);
                    //}
                    //else
                    //{
                    replyTranEventReport(bcfApp, eventType, eqpt, seqNum, true, true, true, "", CMDCancelType.CmdCancel);
                    //}
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: ex,
                   VehicleID: eqpt.VEHICLE_ID,
                   CarrierID: eqpt.CST_ID);
            }
        }

        private void PositionReport_EmptyRetrieval(BCFApplication bcfApp, AVEHICLE eqpt, int seqNum
                                                    , EventType eventType, string current_adr_id, string current_sec_id, string carrier_id)
        {
            try
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                Data: $"Process report {eventType}",
                VehicleID: eqpt.VEHICLE_ID,
                CarrierID: eqpt.CST_ID);

                if (!SCUtility.isEmpty(eqpt.MCS_CMD))
                {
                    bool retryOrAbort = true;
                    retryOrAbort = scApp.TransferService.OHT_TransferStatus(eqpt.OHTC_CMD,
                            eqpt.VEHICLE_ID, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_EMPTY_RETRIEVAL);
                    Boolean resp_cmp;
                    //if (retryOrAbort == true)
                    //{
                    resp_cmp = replyTranEventReport(bcfApp, eventType, eqpt, seqNum, true, true, true, "", CMDCancelType.CmdCancel);
                    //}
                    //else
                    //{
                    //    resp_cmp = replyTranEventReport(bcfApp, eventType, eqpt, seqNum, true, true, "", CMDCancelType.CmdRetry);
                    //}

                }
                else
                {
                    //if (DebugParameter.Is_136_empty_double_retry)
                    //{
                    //    replyTranEventReport(bcfApp, eventType, eqpt, seqNum, true, true, "", CMDCancelType.CmdRetry);
                    //}
                    //else
                    //{
                    replyTranEventReport(bcfApp, eventType, eqpt, seqNum, true, true, true, "", CMDCancelType.CmdCancel);
                    //}
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: ex,
                   VehicleID: eqpt.VEHICLE_ID,
                   CarrierID: eqpt.CST_ID);
            }
        }
        #endregion Position Report

        #region Status Report
        const string VEHICLE_ERROR_REPORT_DESCRIPTION = "Vehicle:{0} ,error happend.";
        [ClassAOPAspect]
        public void StatusReport(BCFApplication bcfApp, AVEHICLE eqpt, ID_144_STATUS_CHANGE_REP recive_str, int seq_num)
        {
            if (scApp.getEQObjCacheManager().getLine().ServerPreStop)
                return;
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
               seq_num: seq_num,
               Data: recive_str,
               VehicleID: eqpt.VEHICLE_ID,
               CarrierID: eqpt.CST_ID);

            SCUtility.RecodeReportInfo(eqpt.VEHICLE_ID, seq_num, recive_str);

            string current_adr = recive_str.CurrentAdrID;
            VHModeStatus modeStat = DecideVhModeStatus(eqpt.VEHICLE_ID, current_adr, recive_str.ModeStatus);
            VHActionStatus actionStat = recive_str.ActionStatus;
            VhPowerStatus powerStat = recive_str.PowerStatus;
            string cstID = recive_str.CSTID;
            VhStopSingle obstacleStat = recive_str.ObstacleStatus;
            VhStopSingle blockingStat = recive_str.BlockingStatus;
            VhStopSingle pauseStat = recive_str.PauseStatus;
            VhStopSingle hidStat = recive_str.HIDStatus;
            VhStopSingle errorStat = recive_str.ErrorStatus;
            VhLoadCarrierStatus loadCSTStatus = recive_str.HasCst;
            VhLoadCarrierStatus loadBOXStatus = recive_str.HasBox;
            if (loadBOXStatus == VhLoadCarrierStatus.Exist) //B0.05
            {
                eqpt.BOX_ID = recive_str.CarBoxID;
            }

            //VhGuideStatus leftGuideStat = recive_str.LeftGuideLockStatus;
            //VhGuideStatus rightGuideStat = recive_str.RightGuideLockStatus;
            // 0317 Jason 此部分之loadBOXStatus 原為loadCSTStatus ，現在之狀況為暫時解法
            bool hasdifferent =
                    !SCUtility.isMatche(eqpt.CST_ID, cstID) ||
                    eqpt.MODE_STATUS != modeStat ||
                    eqpt.ACT_STATUS != actionStat ||
                    eqpt.ObstacleStatus != obstacleStat ||
                    eqpt.BlockingStatus != blockingStat ||
                    eqpt.PauseStatus != pauseStat ||
                    eqpt.HIDStatus != hidStat ||
                    eqpt.ERROR != errorStat ||
                    eqpt.HAS_CST != (int)loadBOXStatus;

            if (eqpt.ERROR != errorStat)
            {
                //todo 在error flag 有變化時，上報S5F1 alarm set/celar
                //string alarm_desc = string.Format(VEHICLE_ERROR_REPORT_DESCRIPTION, eqpt.Real_ID);
                //string alarm_code = $"000{eqpt.Num}";
                //ErrorStatus error_status =
                //    errorStat == VhStopSingle.StopSingleOn ? ErrorStatus.ErrSet : ErrorStatus.ErrReset;
                //scApp.ReportBLL.ReportAlarmHappend(error_status, alarm_code, alarm_desc);
                if (!SCUtility.isEmpty(eqpt.MCS_CMD))
                {
                    scApp.ReportBLL.newReportTransferCommandPaused(eqpt.MCS_CMD, null);
                }
            }


            int obstacleDIST = recive_str.ObstDistance;
            string obstacleVhID = recive_str.ObstVehicleID;
            // 0317 Jason 此部分之loadBOXStatus 原為loadCSTStatus ，現在之狀況為暫時解法
            if (hasdifferent && !scApp.VehicleBLL.doUpdateVehicleStatus(eqpt, cstID,
                                   modeStat, actionStat,
                                   blockingStat, pauseStat, obstacleStat, hidStat, errorStat, loadBOXStatus))
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: $"update vhicle status fail!",
                   VehicleID: eqpt.VEHICLE_ID,
                   CarrierID: eqpt.CST_ID);
                return;
            }

            //if (modeStat == VHModeStatus.AutoMtl)
            //{
            //    var check_is_in_maintain_device = scApp.EquipmentBLL.cache.IsInMaintainDevice(eqpt.CUR_ADR_ID);
            //    if (check_is_in_maintain_device.isIn)
            //    {
            //        var device = check_is_in_maintain_device.device;
            //        if (device is MaintainLift)
            //            scApp.MTLService.carInSafetyAndVehicleStatusCheck(device as MaintainLift);

            //    }
            //}

            //UpdateVehiclePositionFromStatusReport(eqpt, recive_str);

            List<AMCSREPORTQUEUE> reportqueues = null;
            //using (TransactionScope tx = SCUtility.getTransactionScope())
            //{
            //    using (DBConnection_EF con = DBConnection_EF.GetUContext())
            //    {
            //        bool isSuccess = true;
            //        switch (actionStat)
            //        {
            //            case VHActionStatus.Loading:
            //            case VHActionStatus.Unloading:
            //                if (preActionStat != actionStat)
            //                {
            //                    isSuccess = scApp.ReportBLL.ReportLoadingUnloading(eqpt.VEHICLE_ID, actionStat, out reportqueues);
            //                }
            //                break;
            //            default:
            //                isSuccess = true;
            //                break;
            //        }
            //        if (!isSuccess)
            //        {
            //            return;
            //        }
            //        if (reply_status_event_report(bcfApp, eqpt, seq))
            //        {
            //            tx.Complete();
            //        }
            //    }
            //}
            //reply_status_event_report(bcfApp, eqpt, seq_num);

            //if (actionStat == VHActionStatus.Stop)
            //{

            //if (obstacleStat == VhStopSingle.StopSingleOn)
            //{
            //    ASEGMENT seg = scApp.SegmentBLL.cache.GetSegment(eqpt.CUR_SEG_ID);
            //    AVEHICLE next_vh_on_seg = seg.GetNextVehicle(eqpt);
            //    //if (!SCUtility.isEmpty(obstacleVhID))
            //    if (next_vh_on_seg != null)
            //    {
            //        //scApp.VehicleBLL.whenVhObstacle(obstacleVhID);
            //        scApp.VehicleBLL.whenVhObstacle(next_vh_on_seg.VEHICLE_ID);
            //    }
            //}
            //}
        }

        private VHModeStatus DecideVhModeStatus(string vh_id, string current_adr, VHModeStatus vh_current_mode_status)
        {
            AVEHICLE eqpt = scApp.VehicleBLL.getVehicleByID(vh_id);

            LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
              Data: $"current vh mode is:{eqpt.MODE_STATUS} and vh report mode:{vh_current_mode_status}",
              VehicleID: eqpt.VEHICLE_ID,
              CarrierID: eqpt.CST_ID);
            VHModeStatus modeStat = default(VHModeStatus);
            if (vh_current_mode_status == VHModeStatus.AutoRemote)
            {
                if (eqpt.MODE_STATUS == VHModeStatus.AutoLocal ||
                         eqpt.MODE_STATUS == VHModeStatus.AutoMtl ||
                         eqpt.MODE_STATUS == VHModeStatus.AutoMts)
                {
                    modeStat = eqpt.MODE_STATUS;
                }
                else if (scApp.EquipmentBLL.cache.IsInMatainLift(current_adr))
                {
                    modeStat = VHModeStatus.AutoMtl;
                }
                else if (scApp.EquipmentBLL.cache.IsInMatainSpace(current_adr))
                {
                    modeStat = VHModeStatus.AutoMts;
                }
                else
                {
                    modeStat = vh_current_mode_status;
                }
            }
            else
            {
                modeStat = vh_current_mode_status;
            }
            return modeStat;
        }





        //private void whenVhObstacle(string obstacleVhID)
        //{
        //    AVEHICLE obstacleVh = scApp.VehicleBLL.getVehicleByID(obstacleVhID);
        //    if (obstacleVh != null)
        //    {
        //        if (obstacleVh.IS_PARKING &&
        //            !SCUtility.isEmpty(obstacleVh.PARK_ADR_ID))
        //        {
        //            scApp.VehicleBLL.FindParkZoneOrCycleRunZoneForDriveAway(obstacleVh);
        //        }
        //        else if (SCUtility.isEmpty(obstacleVh.OHTC_CMD))
        //        {
        //            string[] nextSections = scApp.MapBLL.loadNextSectionIDBySectionID(obstacleVh.CUR_SEC_ID);
        //            if (nextSections != null && nextSections.Count() > 0)
        //            {
        //                ASECTION nextSection = scApp.MapBLL.getSectiontByID(nextSections[0]);
        //                bool isSuccess = scApp.CMDBLL.doCreatTransferCommand(obstacleVhID
        //                         , string.Empty
        //                         , string.Empty
        //                         , E_CMD_TYPE.Move
        //                         , obstacleVh.CUR_ADR_ID
        //                         , nextSection.TO_ADR_ID, 0, 0);

        //            }
        //        }
        //    }
        //}
        private bool reply_status_event_report(BCFApplication bcfApp, AVEHICLE eqpt, int seq_num)
        {
            ID_44_STATUS_CHANGE_RESPONSE send_str = new ID_44_STATUS_CHANGE_RESPONSE
            {
                ReplyCode = 0
            };
            WrapperMessage wrapper = new WrapperMessage
            {
                SeqNum = seq_num,
                StatusChangeResp = send_str
            };

            //Boolean resp_cmp = ITcpIpControl.sendGoogleMsg(bcfApp, eqpt.TcpIpAgentName, wrapper, true);
            Boolean resp_cmp = eqpt.sendMessage(wrapper, true);
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
              seq_num: seq_num, Data: send_str,
              VehicleID: eqpt.VEHICLE_ID,
              CarrierID: eqpt.CST_ID);
            SCUtility.RecodeReportInfo(eqpt.VEHICLE_ID, seq_num, send_str, resp_cmp.ToString());
            return resp_cmp;
        }
        #endregion Status Report

        #region Command Complete Report
        [ClassAOPAspect]
        public void CommandCompleteReport(string tcpipAgentName, BCFApplication bcfApp, AVEHICLE eqpt, ID_132_TRANS_COMPLETE_REPORT recive_str, int seq_num)
        {
            if (scApp.getEQObjCacheManager().getLine().ServerPreStop)
                return;
            SCUtility.RecodeReportInfo(eqpt.VEHICLE_ID, seq_num, recive_str);
            string vh_id = eqpt.VEHICLE_ID;
            string finish_ohxc_cmd = eqpt.OHTC_CMD;
            string finish_mcs_cmd = eqpt.MCS_CMD;
            string cmd_id = recive_str.CmdID;
            int travel_dis = recive_str.CmdDistance;
            CompleteStatus completeStatus = recive_str.CmpStatus;
            string cur_sec_id = recive_str.CurrentSecID;
            string cur_adr_id = recive_str.CurrentAdrID;
            string cst_id = SCUtility.Trim(recive_str.CSTID, true);
            VhLoadCarrierStatus vhLoadCSTStatus = recive_str.HasCst;
            string car_cst_id = recive_str.BOXID;
            bool isSuccess = true;

            if (scApp.CMDBLL.isCMCD_OHTCFinish(cmd_id))
            {
                replyCommandComplete(eqpt, seq_num, finish_ohxc_cmd, finish_mcs_cmd);

                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: $"commnad id:{cmd_id} has already process. well pass this report.",
                   VehicleID: eqpt.VEHICLE_ID,
                   CarrierID: eqpt.CST_ID);
                return;
            }

            string mcs_cmd_result = SECSConst.convert2MCS(completeStatus);
            scApp.VIDBLL.upDateVIDResultCode(eqpt.VEHICLE_ID, mcs_cmd_result);
            if (recive_str.CmpStatus == CompleteStatus.CmpStatusInterlockError)
            {
                scApp.TransferService.OHT_TransferStatus(finish_ohxc_cmd,
                    eqpt.VEHICLE_ID, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_InterlockError);
                //B0.03
                scApp.TransferService.OHBC_AlarmSet(eqpt.VEHICLE_ID, ((int)AlarmLst.OHT_INTERLOCK_ERROR).ToString());
                scApp.TransferService.OHBC_AlarmCleared(eqpt.VEHICLE_ID, ((int)AlarmLst.OHT_INTERLOCK_ERROR).ToString());
                //
            }
            else if (recive_str.CmpStatus == CompleteStatus.CmpStatusVehicleAbort)
            {
                scApp.TransferService.OHT_TransferStatus(finish_ohxc_cmd,
                    eqpt.VEHICLE_ID, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_VEHICLE_ABORT);
                //B0.03
                scApp.TransferService.OHBC_AlarmSet(eqpt.VEHICLE_ID, ((int)AlarmLst.OHT_VEHICLE_ABORT).ToString());
                scApp.TransferService.OHBC_AlarmCleared(eqpt.VEHICLE_ID, ((int)AlarmLst.OHT_VEHICLE_ABORT).ToString());
                //
            }
            //else if (recive_str.CmpStatus == CompleteStatus.CmpStatusLoadunload || recive_str.CmpStatus == CompleteStatus.CmpStatusUnload)
            //{
            //    // Change the report time to the 136 unloadcomplete
            //    //scApp.TransferService.OHT_TransferStatus(finish_ohxc_cmd,
            //    //eqpt.VEHICLE_ID, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH);
            //}
            else
            {
                scApp.TransferService.OHT_TransferStatus(finish_ohxc_cmd,
                    eqpt.VEHICLE_ID, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH);
            }

            using (TransactionScope tx = SCUtility.getTransactionScope())
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //isSuccess = scApp.VehicleBLL.doTransferCommandFinish(eqpt.VEHICLE_ID, cmd_id);
                    isSuccess &= scApp.VehicleBLL.doTransferCommandFinish(eqpt.VEHICLE_ID, cmd_id, completeStatus);
                    E_CMD_STATUS ohtc_cmd_status = scApp.VehicleBLL.CompleteStatusToCmdStatus(completeStatus);
                    isSuccess &= scApp.CMDBLL.updateCommand_OHTC_StatusByCmdID(cmd_id, ohtc_cmd_status);
                    isSuccess &= scApp.VIDBLL.initialVIDCommandInfo(eqpt.VEHICLE_ID);


                    //當發生Vehicle Abort的時候要確認是否有預下給該Vh的命令，
                    //有的話要將他取消，並把原本的MCS命令切回Queue
                    if (completeStatus == CompleteStatus.CmpStatusVehicleAbort)
                    {
                        var check_result = scApp.CMDBLL.hasCMD_OHTCInQueue(eqpt.VEHICLE_ID);
                        if (check_result.has)
                        {
                            ACMD_OHTC queue_cmd = check_result.cmd_ohtc;
                            scApp.CMDBLL.updateCommand_OHTC_StatusByCmdID(queue_cmd.CMD_ID, E_CMD_STATUS.AbnormalEndByOHTC);
                            if (!SCUtility.isEmpty(queue_cmd.CMD_ID_MCS))
                            {
                                ACMD_MCS pre_initial_cmd_mcs = scApp.CMDBLL.getCMD_MCSByID(queue_cmd.CMD_ID_MCS);
                                if (pre_initial_cmd_mcs != null /*&&
                                    pre_initial_cmd_mcs.TRANSFERSTATE == E_TRAN_STATUS.PreInitial*/)
                                {
                                    scApp.CMDBLL.updateCMD_MCS_TranStatus2Queue(pre_initial_cmd_mcs.CMD_ID);
                                }
                            }
                        }
                    }

                    if (isSuccess)
                    {
                        tx.Complete();
                    }
                    else
                    {
                        return;
                    }
                }
            }

            replyCommandComplete(eqpt, seq_num, finish_ohxc_cmd, finish_mcs_cmd);
            scApp.CMDBLL.removeAllWillPassSection(eqpt.VEHICLE_ID);
            scApp.ReserveBLL.RemoveAllReservedSectionsByVehicleID(eqpt.VEHICLE_ID);
            scApp.ReserveBLL.TryAddReservedSection(eqpt.VEHICLE_ID, eqpt.CUR_SEC_ID);

            //回復結束後，若該筆命令是Mismatch、IDReadFail結束的話則要把原本車上的那顆CST Installed回來。
            if (vhLoadCSTStatus == VhLoadCarrierStatus.Exist)
            {
                scApp.VIDBLL.upDateVIDCarrierID(eqpt.VEHICLE_ID, car_cst_id);
                scApp.VIDBLL.upDateVIDCarrierLocInfo(eqpt.VEHICLE_ID, eqpt.Real_ID);
            }
            List<AMCSREPORTQUEUE> reportqueues = new List<AMCSREPORTQUEUE>();
            switch (completeStatus)
            {
                case CompleteStatus.CmpStatusIdmisMatch:
                case CompleteStatus.CmpStatusIdreadFailed:
                case CompleteStatus.CmpStatusIdreadDuplicate:
                    if (!SCUtility.isEmpty(finish_mcs_cmd))
                    {
                        reportqueues.Clear();
                        scApp.ReportBLL.newReportCarrierIDReadReport(eqpt.VEHICLE_ID, reportqueues);
                        scApp.ReportBLL.insertMCSReport(reportqueues);
                        scApp.ReportBLL.newSendMCSMessage(reportqueues);
                    }
                    break;
                case CompleteStatus.CmpStatusUnload:
                case CompleteStatus.CmpStatusLoadunload:
                    //scApp.PortBLL.OperateCatch.updatePortStationCSTExistStatus(eqpt.CUR_ADR_ID, cst_id);
                    break;
            }

            if (DebugParameter.IsDebugMode && DebugParameter.IsCycleRun)
            {
                SpinWait.SpinUntil(() => false, 3000);
                TestCycleRun(eqpt, cmd_id);
            }

            if (scApp.getEQObjCacheManager().getLine().SCStats == ALINE.TSCState.PAUSING)
            {
                List<ACMD_MCS> cmd_mcs_lst = scApp.CMDBLL.loadACMD_MCSIsUnfinished();
                if (cmd_mcs_lst.Count == 0)
                {
                    scApp.LineService.TSCStateToPause("");
                }
            }
            Task.Run(() =>
            {
                scApp.TransferService.TransferRun();//B0.08.0 處發TransferRun，使MCS命令可以在多車情形下早於趕車CMD下達。
                //tryAskVhToIdlePosition(vh_id);//B0.11
            });
            eqpt.onCommandComplete(completeStatus);
        }

        private bool replyCommandComplete(AVEHICLE eqpt, int seq_num, string finish_ohxc_cmd, string finish_mcs_cmd)
        {
            ID_32_TRANS_COMPLETE_RESPONSE send_str = new ID_32_TRANS_COMPLETE_RESPONSE
            {
                ReplyCode = 0
            };
            WrapperMessage wrapper = new WrapperMessage
            {
                SeqNum = seq_num,
                TranCmpResp = send_str
            };
            //Boolean resp_cmp = ITcpIpControl.sendGoogleMsg(bcfApp, tcpipAgentName, wrapper, true);
            Boolean resp_cmp = eqpt.sendMessage(wrapper, true);
            SCUtility.RecodeReportInfo(eqpt.VEHICLE_ID, seq_num, send_str, finish_ohxc_cmd, finish_mcs_cmd, resp_cmp.ToString());
            return resp_cmp;
        }

        private void TestCycleRun(AVEHICLE vh, string cmd_id)
        {
            ACMD_OHTC cmd = scApp.CMDBLL.getCMD_OHTCByID(cmd_id);
            if (cmd == null) return;
            if (!(cmd.CMD_TPYE == E_CMD_TYPE.LoadUnload || cmd.CMD_TPYE == E_CMD_TYPE.Move)) return;

            string result = string.Empty;
            string cst_id = cmd.CARRIER_ID?.Trim();
            string box_id = cmd.BOX_ID?.Trim();
            string lot_id = cmd.LOT_ID?.Trim();
            string from_port_id = cmd.DESTINATION.Trim();
            string to_port_id = cmd.SOURCE.Trim();
            string from_adr = "";
            string to_adr = "";
            switch (cmd.CMD_TPYE)
            {
                case E_CMD_TYPE.LoadUnload:
                    scApp.MapBLL.getAddressID(from_port_id, out from_adr);
                    scApp.MapBLL.getAddressID(to_port_id, out to_adr);
                    break;
                case E_CMD_TYPE.Move:
                    to_adr = vh.startAdr.Trim();
                    break;
            }
            scApp.CMDBLL.doCreatTransferCommand(cmd.VH_ID,
                                            cst_id: cst_id,
                                            box_id: box_id,
                                            lot_id: lot_id,
                                            cmd_type: cmd.CMD_TPYE,
                                            source: from_port_id,
                                            destination: to_port_id,
                                            source_address: from_adr,
                                            destination_address: to_adr,
                                            gen_cmd_type: SCAppConstants.GenOHxCCommandType.Auto);
        }
        #endregion Command Complete Report

        #region Range Teach

        public void RangeTeachingCompleteReport(string tcpipAgentName, BCFApplication bcfApp, AVEHICLE eqpt, ID_172_RANGE_TEACHING_COMPLETE_REPORT recive_str, int seq_num)
        {
            SCUtility.RecodeReportInfo(eqpt.VEHICLE_ID, seq_num, recive_str);

            string from_adr = recive_str.FromAdr;
            string to_adr = recive_str.ToAdr;
            uint sec_distance = recive_str.SecDistance;
            int cmp_code = recive_str.CompleteCode;
            ID_72_RANGE_TEACHING_COMPLETE_RESPONSE response = null;
            if (cmp_code == 0)
            {
                if (scApp.MapBLL.updateSecDistance(from_adr, to_adr, sec_distance, out ASECTION section))
                {
                    scApp.updateCatchData_Section(section);
                    scApp.VehicleBLL.setAndPublishPositionReportInfo2Redis(eqpt.VEHICLE_ID, recive_str, section.SEC_ID);
                }
            }
            response = new ID_72_RANGE_TEACHING_COMPLETE_RESPONSE()
            {
                ReplyCode = 0
            };

            WrapperMessage wrapper = new WrapperMessage
            {
                SeqNum = seq_num,
                RangeTeachingCmpResp = response
            };
            Boolean resp_cmp = eqpt.sendMessage(wrapper, true);
            SCUtility.RecodeReportInfo(eqpt.VEHICLE_ID, seq_num, response, resp_cmp.ToString());

            AutoTeaching(eqpt.VEHICLE_ID);
        }

        public void AutoTeaching(string vh_id)
        {
            if (!sc.App.SystemParameter.AutoTeching) return;
            //1.找出VH，並得到他目前所在的Address。
            scApp.VehicleBLL.getAndProcPositionReportFromRedis(vh_id);
            AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(vh_id);
            string vh_current_adr = vh.CUR_ADR_ID;
            List<string> base_address = new List<string> { vh.CUR_ADR_ID };
            HashSet<string> choess_sation = new HashSet<string>();

            do
            {
                //接著透過這個Address查詢哪些Section是該Address的From Adr.且還沒有Teching過的(LAST_TECH_TIME = null)
                List<ASECTION> sections = scApp.MapBLL.loadSectionByFromAdrs(base_address);
                base_address.Clear();
                foreach (var section in sections)
                {
                    if (section.SEC_TYPE == SectionType.Mtl) continue;

                    if (section.LAST_TECH_TIME.HasValue)
                    {
                        if (section.DIRC_DRIV == 0)
                            base_address.Add(section.TO_ADR_ID.Trim());
                    }
                    else
                    {
                        TechingAction(vh_id, vh_current_adr, section);
                        base_address.Clear();
                        break;
                    }
                }
                if (!scApp.MapBLL.hasNotYetTeachingSection())
                {
                    sc.App.SystemParameter.AutoTeching = false;
                    bcf.App.BCFApplication.onInfoMsg("All section teching complete.");
                    return;
                }

            } while (base_address.Count != 0);



        }

        private void TechingAction(string vh_id, string vh_current_adr, ASECTION section)
        {
            if (SCUtility.isMatche(section.FROM_ADR_ID, vh_current_adr))
            {
                TeachingRequest(vh_id, section.FROM_ADR_ID, section.TO_ADR_ID);
            }
            else
            {
                string[] ReutrnFromAdr2ToAdr = scApp.RouteGuide.DownstreamSearchSection
                    (vh_current_adr, section.FROM_ADR_ID, 1, true);
                string route = ReutrnFromAdr2ToAdr[0].Split('=')[0];
                string[] routeSection = route.Split(',');
                ASECTION first_sec = scApp.MapBLL.getSectiontByID(routeSection[0]);
                TeachingRequest(vh_id, vh_current_adr, first_sec.TO_ADR_ID);
                //scApp.CMDBLL.doCreatTransferCommand(vh_id
                //                              , string.Empty
                //                              , string.Empty
                //                              , E_CMD_TYPE.Move_Teaching
                //                              , vh_current_adr
                //                              , section.FROM_ADR_ID, 0, 0);
            }
        }
        #endregion Range Teach

        #region Receive Message
        public void BasicInfoVersionReport(BCFApplication bcfApp, AVEHICLE eqpt, ID_102_BASIC_INFO_VERSION_REP recive_str, int seq_num)
        {
            ID_2_BASIC_INFO_VERSION_RESPONSE send_str = new ID_2_BASIC_INFO_VERSION_RESPONSE
            {
                ReplyCode = 0
            };
            WrapperMessage wrapper = new WrapperMessage
            {
                SeqNum = seq_num,
                BasicInfoVersionResp = send_str
            };
            Boolean resp_cmp = eqpt.sendMessage(wrapper, true);
            //SCUtility.RecodeReportInfo(eqpt.VEHICLE_ID, seqNum, send_str, resp_cmp.ToString());
        }
        public void GuideDataUploadRequest(BCFApplication bcfApp, AVEHICLE eqpt, ID_162_GUIDE_DATA_UPLOAD_REP recive_str, int seq_num)
        {
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
               seq_num: seq_num,
               Data: recive_str,
               VehicleID: eqpt.VEHICLE_ID,
               CarrierID: eqpt.CST_ID);

            ID_62_GUID_DATA_UPLOAD_RESPONSE send_str = new ID_62_GUID_DATA_UPLOAD_RESPONSE
            {
                ReplyCode = 0
            };
            WrapperMessage wrapper = new WrapperMessage
            {
                SeqNum = seq_num,
                GUIDEDataUploadResp = send_str
            };
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
               seq_num: seq_num,
               Data: send_str,
               VehicleID: eqpt.VEHICLE_ID,
               CarrierID: eqpt.CST_ID);

            Boolean resp_cmp = eqpt.sendMessage(wrapper, true);
            //SCUtility.RecodeReportInfo(eqpt.VEHICLE_ID, seqNum, send_str, resp_cmp.ToString());
        }
        public void AddressTeachReport(BCFApplication bcfApp, AVEHICLE eqpt, ID_174_ADDRESS_TEACH_REPORT recive_str, int seq_num)
        {
            try
            {
                string adr_id = recive_str.Addr;
                int resolution = recive_str.Position;

                scApp.DataSyncBLL.updateAddressData(eqpt.VEHICLE_ID, adr_id, resolution);

                ID_74_ADDRESS_TEACH_RESPONSE send_str = new ID_74_ADDRESS_TEACH_RESPONSE
                {
                    ReplyCode = 0
                };
                WrapperMessage wrapper = new WrapperMessage
                {
                    SeqNum = seq_num,
                    AddressTeachResp = send_str
                };
                Boolean resp_cmp = eqpt.sendMessage(wrapper, true);
                //SCUtility.RecodeReportInfo(eqpt.VEHICLE_ID, seqNum, send_str, resp_cmp.ToString());
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }
        [ClassAOPAspect]
        public void AlarmReport(BCFApplication bcfApp, AVEHICLE eqpt, ID_194_ALARM_REPORT recive_str, int seq_num)
        {
            LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
              seq_num: seq_num, Data: recive_str,
              VehicleID: eqpt.VEHICLE_ID,
              CarrierID: eqpt.CST_ID);
            try
            {
                SCUtility.RecodeReportInfo(eqpt.VEHICLE_ID, seq_num, recive_str);
                string node_id = eqpt.NODE_ID;
                string eq_id = eqpt.VEHICLE_ID;
                string err_code = recive_str.ErrCode;
                ErrorStatus status = recive_str.ErrStatus;
                if (status == ErrorStatus.ErrSet)
                {
                    //scApp.ReportBLL.ReportAlarmHappend(report_alarm.ALAM_STAT, alarm_code, report_alarm.ALAM_DESC);
                    //scApp.ReportBLL.newReportUnitAlarmSet(eqpt.Real_ID, alarm_code, report_alarm.ALAM_DESC, eqpt.CUR_ADR_ID, reportqueues);
                    scApp.TransferService.OHBC_AlarmSet(eqpt.VEHICLE_ID, err_code);
                }
                else
                {
                    //scApp.ReportBLL.ReportAlarmHappend(report_alarm.ALAM_STAT, alarm_code, report_alarm.ALAM_DESC);
                    //scApp.ReportBLL.newReportUnitAlarmClear(eqpt.Real_ID, alarm_code, report_alarm.ALAM_DESC, eqpt.CUR_ADR_ID, reportqueues);
                    if (err_code != "0")
                    {
                        scApp.TransferService.OHBC_AlarmCleared(eqpt.VEHICLE_ID, err_code);
                    }
                    else
                    {
                        scApp.TransferService.OHBC_AlarmAllCleared(eqpt.VEHICLE_ID);
                    }
                }

                ////List<ALARM> alarms = null;
                //ALARM alarms = new ALARM();
                //AlarmMap alarmMap = scApp.AlarmBLL.GetAlarmMap(eq_id, err_code);
                ////在設備上報Alarm時，如果是第一次上報(之前都沒有Alarm發生時，則要上報S6F11 CEID=51 Alarm Set)
                //bool processBeferHasErrorExist = scApp.AlarmBLL.hasAlarmErrorExist();
                //if (alarmMap != null &&
                //    alarmMap.ALARM_LVL == E_ALARM_LVL.Error &&
                //    status == ErrorStatus.ErrSet &&
                //    //!scApp.AlarmBLL.hasAlarmErrorExist())
                //    !processBeferHasErrorExist)
                //{
                //    scApp.ReportBLL.newReportAlarmSet();
                //}
                //scApp.getRedisCacheManager().BeginTransaction();
                //using (TransactionScope tx = SCUtility.getTransactionScope())
                //{
                //    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                //    {
                //        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                //           Data: $"Process vehicle alarm report.alarm code:{err_code},alarm status{status}",
                //           VehicleID: eqpt.VEHICLE_ID,
                //           CarrierID: eqpt.CST_ID);
                //        ALARM alarm = null;
                //        switch (status)
                //        {
                //            case ErrorStatus.ErrSet:
                //                ////將設備上報的Alarm填入資料庫。
                //                alarm = scApp.AlarmBLL.setAlarmReport(node_id, eq_id, err_code);
                //                ////將其更新至Redis，保存目前所發生的Alarm
                //                //scApp.AlarmBLL.setAlarmReport2Redis(alarm);
                //                //alarms = new List<ALARM>() { alarm };
                //                alarms = alarm;
                //                break;
                //            case ErrorStatus.ErrReset:
                //                if (SCUtility.isMatche(err_code, "0"))
                //                {
                //                    //alarms = scApp.AlarmBLL.resetAllAlarmReport(eq_id);
                //                    //scApp.AlarmBLL.resetAllAlarmReport2Redis(eq_id);
                //                }
                //                else
                //                {
                //                    ////將設備上報的Alarm從資料庫刪除。
                //                    //alarm = scApp.AlarmBLL.resetAlarmReport(eq_id, err_code);
                //                    ////將其更新至Redis，保存目前所發生的Alarm
                //                    //scApp.AlarmBLL.resetAlarmReport2Redis(alarm);
                //                    //alarms = new List<ALARM>() { alarm };
                //                }
                //                break;
                //        }
                //        tx.Complete();
                //    }
                //}
                ////scApp.getRedisCacheManager().ExecuteTransaction();
                //////通知有Alarm的資訊改變。
                ////scApp.getNatsManager().PublishAsync(SCAppConstants.NATS_SUBJECT_CURRENT_ALARM, new byte[0]);


                ////foreach (ALARM report_alarm in alarms)
                ////{
                ////if (report_alarm == null) continue;
                //ALARM report_alarm = alarms;
                //if (report_alarm.ALAM_LVL == E_ALARM_LVL.Warn ||
                //    report_alarm.ALAM_LVL == E_ALARM_LVL.None)
                //{

                //}
                //else
                //{
                //    //需判斷Alarm是否存在如果有的話則需再判斷MCS是否有Disable該Alarm的上報
                //    int ialarm_code = 0;
                //    int.TryParse(report_alarm.ALAM_CODE, out ialarm_code);
                //    string alarm_code = (ialarm_code < 0 ? ialarm_code * -1 : ialarm_code).ToString();
                //    if (scApp.AlarmBLL.IsReportToHost(alarm_code))
                //    {
                //        //scApp.ReportBLL.ReportAlarmHappend(eqpt.VEHICLE_ID, alarm.ALAM_STAT, alarm.ALAM_CODE, alarm.ALAM_DESC, out reportqueues);
                //        List<AMCSREPORTQUEUE> reportqueues = new List<AMCSREPORTQUEUE>();
                //        if (report_alarm.ALAM_STAT == ErrorStatus.ErrSet)
                //        {
                //            //scApp.ReportBLL.ReportAlarmHappend(report_alarm.ALAM_STAT, alarm_code, report_alarm.ALAM_DESC);
                //            //scApp.ReportBLL.newReportUnitAlarmSet(eqpt.Real_ID, alarm_code, report_alarm.ALAM_DESC, eqpt.CUR_ADR_ID, reportqueues);
                //            scApp.TransferService.OHT_AlarmSet(eqpt.VEHICLE_ID, err_code);
                //        }
                //        else
                //        {
                //            //scApp.ReportBLL.ReportAlarmHappend(report_alarm.ALAM_STAT, alarm_code, report_alarm.ALAM_DESC);
                //            //scApp.ReportBLL.newReportUnitAlarmClear(eqpt.Real_ID, alarm_code, report_alarm.ALAM_DESC, eqpt.CUR_ADR_ID, reportqueues);
                //            scApp.TransferService.OHT_AlarmCleared(eqpt.VEHICLE_ID, err_code);
                //        }
                //        //scApp.ReportBLL.newSendMCSMessage(reportqueues);

                //        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                //           Data: $"do report alarm to mcs,alarm code:{err_code},alarm status{status}",
                //           VehicleID: eqpt.VEHICLE_ID,
                //           CarrierID: eqpt.CST_ID);
                //    }
                //}
                ////}
                //////在設備上報取消Alarm，如果已經沒有Alarm(Alarm都已經消除，則要上報S6F11 CEID=52 Alarm Clear)
                ////bool processAfterHasErrorExist = scApp.AlarmBLL.hasAlarmErrorExist();
                ////if (status == ErrorStatus.ErrReset &&
                ////    //!scApp.AlarmBLL.hasAlarmErrorExist())
                ////    processBeferHasErrorExist &&
                ////    !processAfterHasErrorExist)
                ////{
                ////    scApp.ReportBLL.newReportAlarmClear();
                ////}



                ID_94_ALARM_RESPONSE send_str = new ID_94_ALARM_RESPONSE
                {
                    ReplyCode = 0
                };
                WrapperMessage wrapper = new WrapperMessage
                {
                    SeqNum = seq_num,
                    AlarmResp = send_str
                };
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                  seq_num: seq_num, Data: send_str,
                  VehicleID: eqpt.VEHICLE_ID,
                  CarrierID: eqpt.CST_ID);

                Boolean resp_cmp = eqpt.sendMessage(wrapper, true);

                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: $"do reply alarm report ,{resp_cmp}",
                   VehicleID: eqpt.VEHICLE_ID,
                   CarrierID: eqpt.CST_ID);
                SCUtility.RecodeReportInfo(eqpt.VEHICLE_ID, seq_num, send_str, resp_cmp.ToString());

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: ex,
                   VehicleID: eqpt.VEHICLE_ID,
                   CarrierID: eqpt.CST_ID);
            }
        }
        #endregion Receive Message

        #region MTL Handle
        public bool doReservationVhToMaintainsBufferAddress(string vhID, string mtlBufferAdtID)
        {
            bool isSuccess = true;
            using (TransactionScope tx = SCUtility.getTransactionScope())
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    isSuccess = isSuccess && VehicleAutoModeCahnge(vhID, VHModeStatus.AutoMtl);
                    if (isSuccess)
                    {
                        tx.Complete();
                    }
                }
            }
            return isSuccess;
        }
        public bool doReservationVhToMaintainsSpace(string vhID)
        {
            bool isSuccess = true;
            using (TransactionScope tx = SCUtility.getTransactionScope())
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    isSuccess = isSuccess && VehicleAutoModeCahnge(vhID, VHModeStatus.AutoMts);
                    if (isSuccess)
                    {
                        tx.Complete();
                    }
                }
            }
            return isSuccess;
        }
        public bool doAskVhToSystemOutAddress(string vhID, string carOutBufferAdr)
        {
            bool isSuccess = true;
            isSuccess = scApp.CMDBLL.doCreatTransferCommand(vh_id: vhID, cmd_type: E_CMD_TYPE.SystemOut, destination: carOutBufferAdr);
            return isSuccess;
        }

        public bool doAskVhToMaintainsAddress(string vhID, string mtlAdtID)
        {
            bool isSuccess = true;
            isSuccess = isSuccess && scApp.CMDBLL.doCreatTransferCommand(vh_id: vhID, cmd_type: E_CMD_TYPE.MoveToMTL, destination: mtlAdtID);
            return isSuccess;
        }
        public bool doAskVhToCarInBufferAddress(string vhID, string carInBufferAdr)
        {
            bool isSuccess = true;
            isSuccess = scApp.CMDBLL.doCreatTransferCommand(vh_id: vhID, cmd_type: E_CMD_TYPE.MTLHome, destination: carInBufferAdr);
            return isSuccess;
        }

        public bool doAskVhToSystemInAddress(string vhID, string systemInAdr)
        {
            bool isSuccess = true;
            isSuccess = scApp.CMDBLL.doCreatTransferCommand(vh_id: vhID, cmd_type: E_CMD_TYPE.SystemIn, destination: systemInAdr);
            return isSuccess;
        }

        public bool doRecoverModeStatusToAutoRemote(string vh_id)
        {
            return VehicleAutoModeCahnge(vh_id, VHModeStatus.AutoRemote);
        }


        #endregion MTL Handle

        #region Vehicle Change The Path
        public void VhicleChangeThePath(string vh_id, bool isNeedPauseFirst)
        {
            string ohxc_cmd_id = "";
            try
            {
                bool isSuccess = true;
                AVEHICLE need_change_path_vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
                if (need_change_path_vh.VhRecentTranEvent == EventType.Vhloading ||
                    need_change_path_vh.VhRecentTranEvent == EventType.Vhunloading)
                    return;
                //1.先下暫停給該台VH
                if (isNeedPauseFirst)
                    isSuccess = PauseRequest(vh_id, PauseEvent.Pause, OHxCPauseType.Normal);
                //2.送出31執行命令的Override
                //  a.取得執行中的命令
                //  b.重新將該命令改成Ready to rewrite
                ACMD_OHTC cmd_ohtc = null;
                using (TransactionScope tx = SCUtility.getTransactionScope())
                {
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        isSuccess &= scApp.CMDBLL.updateCMD_OHxC_Status2ReadyToReWirte(need_change_path_vh.OHTC_CMD, out cmd_ohtc);
                        isSuccess &= scApp.CMDBLL.update_CMD_Detail_2AbnormalFinsh(need_change_path_vh.OHTC_CMD, need_change_path_vh.WillPassSectionID);
                        if (isSuccess)
                            tx.Complete();
                    }
                }
                ohxc_cmd_id = cmd_ohtc.CMD_ID.Trim();
                scApp.VehicleService.doSendOHxCOverrideCmdToVh(need_change_path_vh, cmd_ohtc, isNeedPauseFirst);
            }
            catch (BLL.VehicleBLL.BlockedByTheErrorVehicleException blockedExecption)
            {
                logger.Warn(blockedExecption, "BlockedByTheErrorVehicleException:");
                VehicleBlockedByTheErrorVehicle();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }
        private void VehicleBlockedByTheErrorVehicle()
        {
            ALARM alarm = scApp.AlarmBLL.setAlarmReport(SCAppConstants.System_ID, SCAppConstants.System_ID, MainAlarmCode.OHxC_BOLCKED_BY_THE_ERROR_VEHICLE_0_1, null);
            if (alarm != null)
            {
                //scApp.AlarmBLL.onMainAlarm(SCAppConstants.MainAlarmCode.OHxC_BOLCKED_BY_THE_ERROR_VEHICLE_0_1,
                //                           vh_id,
                //                           ohxc_cmd_id);
                List<AMCSREPORTQUEUE> reportqueues = null;
                //scApp.ReportBLL.ReportAlarmHappend(alarm.EQPT_ID, alarm.ALAM_STAT, alarm.ALAM_CODE, alarm.ALAM_DESC, out reportqueues);
                scApp.LineBLL.updateHostControlState(LineHostControlState.HostControlState.On_Line_Local);
            }
        }
        #endregion Vehicle Change The Path
        public bool VehicleAutoModeCahnge(string vh_id, VHModeStatus mode_status)
        {
            AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(vh_id);
            if (vh.MODE_STATUS != VHModeStatus.Manual)
            {
                scApp.VehicleBLL.updataVehicleMode(vh_id, mode_status);
                vh.NotifyVhStatusChange();
                return true;
            }
            return false;
        }
        #region Vh connection / disconnention
        [ClassAOPAspect]
        public void Connection(BCFApplication bcfApp, AVEHICLE vh)
        {
            //scApp.getEQObjCacheManager().refreshVh(eqpt.VEHICLE_ID);
            try
            {
                vh.isSynchronizing = true;
                lock (vh.Connection_Sync)
                {
                    vh.VhRecentTranEvent = EventType.AdrPass;

                    vh.isTcpIpConnect = true;

                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                       Data: "Connection ! Begin synchronize with vehicle...",
                       VehicleID: vh.VEHICLE_ID,
                       CarrierID: vh.CST_ID);
                    VehicleInfoSynchronize(vh.VEHICLE_ID);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                       Data: "Connection ! End synchronize with vehicle.",
                       VehicleID: vh.VEHICLE_ID,
                       CarrierID: vh.CST_ID);
                    SCUtility.RecodeConnectionInfo
                        (vh.VEHICLE_ID,
                        SCAppConstants.RecodeConnectionInfo_Type.Connection.ToString(),
                        vh.getDisconnectionIntervalTime(bcfApp));

                    //clear the connection alarm code 99999
                    //scApp.TransferService.OHBC_AlarmCleared(vh.VEHICLE_ID,
                    //    SCAppConstants.SystemAlarmCode.OHT_Issue.OHTAccidentOfflineWarning);

                    //B0.10 scApp.TransferService.iniOHTData(vh.VEHICLE_ID, "OHT_Connection");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                vh.isSynchronizing = false;
            }
        }
        [ClassAOPAspect]
        public void Disconnection(BCFApplication bcfApp, AVEHICLE vh)
        {
            lock (vh.Connection_Sync)
            {

                vh.isTcpIpConnect = false;

                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: "Disconnection !",
                   VehicleID: vh.VEHICLE_ID,
                   CarrierID: vh.CST_ID);
                SCUtility.RecodeConnectionInfo
                    (vh.VEHICLE_ID,
                    SCAppConstants.RecodeConnectionInfo_Type.Disconnection.ToString(),
                    vh.getConnectionIntervalTime(bcfApp));

                //set the connection alarm code 99999
                scApp.TransferService.OHBC_AlarmSet(vh.VEHICLE_ID, SCAppConstants.SystemAlarmCode.OHT_Issue.OHTAccidentOfflineWarning);
            }
            Task.Run(() => scApp.VehicleBLL.web.vehicleDisconnection(scApp));
        }
        #endregion Vh Connection / disconnention

        #region Vehicle Install/Remove
        public void Install(string vhID)
        {
            try
            {

                bool is_success = true;

                is_success = is_success && scApp.VehicleBLL.updataVehicleInstall(vhID);
                if (is_success)
                {
                    AVEHICLE vh_vo = scApp.VehicleBLL.cache.getVhByID(vhID);
                    vh_vo.VehicleInstall();
                }
                List<AMCSREPORTQUEUE> reportqueues = new List<AMCSREPORTQUEUE>();
                is_success = is_success && scApp.ReportBLL.newReportVehicleInstalled(vhID, reportqueues);
                scApp.ReportBLL.newSendMCSMessage(reportqueues);


            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: ex,
                   VehicleID: vhID);
            }
        }
        public void Remove(string vhID)
        {
            try
            {
                bool is_success = true;
                is_success = is_success && scApp.VehicleBLL.updataVehicleRemove(vhID);
                if (is_success)
                {
                    AVEHICLE vh_vo = scApp.VehicleBLL.cache.getVhByID(vhID);
                    vh_vo.VechileRemove();
                }
                List<AMCSREPORTQUEUE> reportqueues = new List<AMCSREPORTQUEUE>();
                is_success = is_success && scApp.ReportBLL.newReportVehicleRemoved(vhID, reportqueues);
                scApp.ReportBLL.newSendMCSMessage(reportqueues);
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: ex,
                   VehicleID: vhID);
            }
        }
        #endregion Vehicle Install/Remove
        public DynamicMetaObject GetMetaObject(Expression parameter)
        {

            return new AspectWeaver(parameter, this);
        }

        #region Specially Control
        public void forceReleaseBlockControl(string vh_id = "")
        {
            List<BLOCKZONEQUEUE> queues = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {

                if (SCUtility.isEmpty(vh_id))
                {
                    queues = scApp.MapBLL.loadAllNonReleaseBlockQueue();
                }
                else
                {
                    queues = scApp.MapBLL.loadNonReleaseBlockQueueByCarID(vh_id);
                }

                foreach (var queue in queues)
                {
                    scApp.MapBLL.updateBlockZoneQueue_AbnormalEnd(queue, SCAppConstants.BlockQueueState.Abnormal_Release_ForceRelease);
                    scApp.MapBLL.DeleteBlockControlKeyWordToRedis(queue.CAR_ID.Trim(), queue.ENTRY_SEC_ID);
                }
            }
        }

        public void reCheckBlockControl(BLOCKZONEQUEUE blockZoneQueue)
        {
            ABLOCKZONEMASTER blockmaster = scApp.MapBLL.getBlockZoneMasterByEntrySecID(blockZoneQueue.ENTRY_SEC_ID);
            if (blockmaster != null)
            {
                List<string> lstSecid = scApp.MapBLL.loadBlockZoneDetailSecIDsByEntrySecID(blockZoneQueue.ENTRY_SEC_ID);
                if (!scApp.VehicleBLL.hasVehicleOnSections(lstSecid))
                {
                    using (TransactionScope tx = SCUtility.getTransactionScope())
                    {
                        using (DBConnection_EF con = DBConnection_EF.GetUContext())
                        {
                            scApp.MapBLL.updateBlockZoneQueue_AbnormalEnd(blockZoneQueue, SCAppConstants.BlockQueueState.Abnormal_Release_ForceRelease);
                            scApp.MapBLL.DeleteBlockControlKeyWordToRedis(blockZoneQueue.CAR_ID, blockZoneQueue.ENTRY_SEC_ID);
                            tx.Complete();
                        }
                    }
                    blockZoneReleaseScript(blockmaster);
                }
            }
        }


        public void PauseAllVehicleByOHxCPause()
        {
            List<AVEHICLE> vhs = scApp.getEQObjCacheManager().getAllVehicle();
            foreach (var vh in vhs)
            {
                PauseRequest(vh.VEHICLE_ID, PauseEvent.Pause, OHxCPauseType.Earthquake);
            }
        }
        public void ResumeAllVehicleByOhxCPause()
        {
            List<AVEHICLE> vhs = scApp.getEQObjCacheManager().getAllVehicle();
            foreach (var vh in vhs)
            {
                PauseRequest(vh.VEHICLE_ID, PauseEvent.Continue, OHxCPauseType.Earthquake);
            }
        }

        #endregion Specially Control


        #region RoadService Mark
        //public ASEGMENT doEnableDisableSegment(string segment_id, E_PORT_STATUS port_status, string laneCutType)
        //{
        //    ASEGMENT segment = null;
        //    try
        //    {
        //        List<APORTSTATION> port_stations = scApp.MapBLL.loadAllPortBySegmentID(segment_id);

        //        using (TransactionScope tx = SCUtility.getTransactionScope())
        //        {
        //            using (DBConnection_EF con = DBConnection_EF.GetUContext())
        //            {
        //                switch (port_status)
        //                {
        //                    case E_PORT_STATUS.InService:
        //                        segment = scApp.RouteGuide.OpenSegment(segment_id);
        //                        break;
        //                    case E_PORT_STATUS.OutOfService:
        //                        segment = scApp.RouteGuide.CloseSegment(segment_id);
        //                        break;
        //                }
        //                foreach (APORTSTATION port_station in port_stations)
        //                {
        //                    scApp.MapBLL.updatePortStatus(port_station.PORT_ID, port_status);
        //                    scApp.getEQObjCacheManager().getPortStation(port_station.PORT_ID).PORT_STATUS = port_status;
        //                }
        //                tx.Complete();
        //            }
        //        }
        //        List<AMCSREPORTQUEUE> reportqueues = new List<AMCSREPORTQUEUE>();
        //        List<ASECTION> sections = scApp.MapBLL.loadSectionsBySegmentID(segment_id);
        //        string segment_start_adr = sections.First().FROM_ADR_ID;
        //        string segment_end_adr = sections.Last().TO_ADR_ID;
        //        switch (port_status)
        //        {
        //            case E_PORT_STATUS.InService:
        //                scApp.ReportBLL.newReportLaneInService(segment_start_adr, segment_end_adr, laneCutType, reportqueues);
        //                break;
        //            case E_PORT_STATUS.OutOfService:
        //                scApp.ReportBLL.newReportLaneOutOfService(segment_start_adr, segment_end_adr, laneCutType, reportqueues);
        //                break;
        //        }
        //        foreach (APORTSTATION port_station in port_stations)
        //        {
        //            switch (port_status)
        //            {
        //                case E_PORT_STATUS.InService:
        //                    scApp.ReportBLL.newReportPortInServeice(port_station.PORT_ID, reportqueues);
        //                    break;
        //                case E_PORT_STATUS.OutOfService:
        //                    scApp.ReportBLL.newReportPortOutOfService(port_station.PORT_ID, reportqueues);
        //                    break;
        //            }
        //        }
        //        scApp.ReportBLL.newSendMCSMessage(reportqueues);
        //    }
        //    catch (Exception ex)
        //    {
        //        segment = null;
        //        logger.Error(ex, "Exception:");
        //    }
        //    return segment;
        //}
        #endregion RoadService
        //************************************************************
        //B0.07 輸入port ID 後 可以回傳在該位置上的 VehicleID 若找不到或者 exception 會回報"Error"
        public string GetVehicleIDByPortID(string portID)
        {
            try
            {
                bool isSuccess = false;
                string portAddressID = null;
                string targetVehicleID = "Error";
                isSuccess = scApp.PortDefBLL.getAddressID(portID, out portAddressID);
                List<AVEHICLE> allVehicleList = scApp.getEQObjCacheManager().getAllVehicle();
                foreach (AVEHICLE vehicle in allVehicleList)
                {
                    AVEHICLE vehicleCache = scApp.VehicleBLL.cache.getVhByID(vehicle.VEHICLE_ID);
                    if (vehicleCache.CUR_ADR_ID == portAddressID)
                    {
                        targetVehicleID = vehicleCache.VEHICLE_ID;
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                        Data: $"Find vehicle {vehicleCache.VEHICLE_ID}, vehicle address Id = {vehicleCache.CUR_ADR_ID}, = port address ID {portAddressID}");
                        break;
                    }

                }
                return targetVehicleID.Trim();
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx,
                   Data: ex);

                scApp.TransferService.TransferServiceLogger.Error(ex, "GetVehicleIDByPortID");
                return "Error";
            }
        }

        //************************************************************
        //B0.08 輸入vehicleID 後 可以回傳該vehicle ID 之車輛目前實時在cache中的資料。若出現異常，則會回傳一空的AVEHICLE 物件。
        public AVEHICLE GetVehicleDataByVehicleID(string vehicleID)
        {
            try
            {
                AVEHICLE vehicleData = new AVEHICLE();
                vehicleData = scApp.VehicleBLL.cache.getVhByID(vehicleID);
                return vehicleData;
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx, Data: ex);
                scApp.TransferService.TransferServiceLogger.Error(ex, "GetVehicleDataByVehicleID");
                AVEHICLE exceptionVehicleData = new AVEHICLE();
                return exceptionVehicleData;
            }
        }

        public bool IsCMD_MCSCanProcess()
        {
            bool isCMD_MCSCanProcess = false;
            try
            {
                List<ACMD_MCS> ACMD_MCSData = scApp.CMDBLL.GetMCSCmdQueue();
                foreach (ACMD_MCS commandMCS in ACMD_MCSData)
                {
                    isCMD_MCSCanProcess = scApp.TransferService.AreSourceAndDestEnable(commandMCS.HOSTSOURCE, commandMCS.HOSTDESTINATION);
                    if (isCMD_MCSCanProcess == true)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                isCMD_MCSCanProcess = false;
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(VehicleService), Device: DEVICE_NAME_OHx, Data: ex);
                scApp.TransferService.TransferServiceLogger.Error(ex, "GetVehicleDataByVehicleID");
            }
            return isCMD_MCSCanProcess;
        }
        #region TEST
        private void CarrierInterfaceSim_LoadComplete(AVEHICLE vh)
        {
            //vh.CatchPLCCSTInterfacelog();
            bool[] bools_01 = new bool[16];
            bool[] bools_02 = new bool[16];
            bool[] bools_03 = new bool[16];
            bool[] bools_04 = new bool[16];
            bool[] bools_05 = new bool[16];
            bool[] bools_06 = new bool[16];
            bool[] bools_07 = new bool[16];
            bool[] bools_08 = new bool[16];
            bool[] bools_09 = new bool[16];
            bool[] bools_10 = new bool[16];

            bools_01[3] = true;

            bools_02[03] = true; bools_02[08] = true; bools_02[12] = true; bools_02[14] = true;
            bools_02[15] = true;

            bools_03[3] = true; bools_03[8] = true; bools_03[10] = true; bools_03[12] = true;
            bools_03[14] = true; bools_03[15] = true;

            bools_04[3] = true; bools_04[4] = true; bools_04[8] = true; bools_04[10] = true;
            bools_04[12] = true; bools_04[14] = true; bools_04[15] = true;

            bools_05[3] = true; bools_05[4] = true; bools_05[8] = true; bools_05[10] = true;
            bools_05[11] = true; bools_05[12] = true; bools_05[14] = true; bools_05[15] = true;

            bools_06[3] = true; bools_06[4] = true; bools_06[5] = true; bools_06[8] = true;
            bools_06[10] = true; bools_06[11] = true; bools_06[12] = true; bools_06[14] = true;
            bools_06[15] = true;

            bools_07[3] = true; bools_07[4] = true; bools_07[5] = true; bools_07[10] = true;
            bools_07[11] = true; bools_07[12] = true; bools_07[14] = true; bools_07[15] = true;

            bools_08[3] = true; bools_08[6] = true; bools_08[10] = true; bools_08[11] = true;
            bools_08[12] = true; bools_08[14] = true; bools_08[15] = true;

            bools_09[3] = true; bools_09[6] = true; bools_09[10] = true; bools_09[12] = true;
            bools_09[14] = true; bools_09[15] = true;

            bools_10[3] = true;

            List<bool[]> lst_bools = new List<bool[]>()
            {
                bools_01,bools_02,bools_03,bools_04,bools_05,bools_06,bools_07,bools_08,bools_09,bools_10,
            };
            if (DebugParameter.isTestCarrierInterfaceError)
            {
                RandomSetCSTInterfaceBool(bools_03);
                RandomSetCSTInterfaceBool(bools_04);
                RandomSetCSTInterfaceBool(bools_05);
                RandomSetCSTInterfaceBool(bools_06);
                RandomSetCSTInterfaceBool(bools_07);
                RandomSetCSTInterfaceBool(bools_08);
                RandomSetCSTInterfaceBool(bools_09);
                //lst_bools[6][11] = false;
            }
            string port_id = "";
            scApp.MapBLL.getPortID(vh.CUR_ADR_ID, out port_id);

            //scApp.PortBLL.OperateCatch.updatePortStationCSTExistStatus(port_id, string.Empty);

            CarrierInterface_LogOut(vh.VEHICLE_ID, port_id, lst_bools);
        }

        private static void RandomSetCSTInterfaceBool(bool[] bools_03)
        {
            Random rnd_Index = new Random(Guid.NewGuid().GetHashCode());
            int rnd_value_1 = rnd_Index.Next(bools_03.Length - 1);
            int rnd_value_2 = rnd_Index.Next(bools_03.Length - 1);
            int rnd_value_3 = rnd_Index.Next(bools_03.Length - 1);
            int rnd_value_4 = rnd_Index.Next(bools_03.Length - 1);
            int rnd_value_5 = rnd_Index.Next(bools_03.Length - 1);
            int rnd_value_6 = rnd_Index.Next(bools_03.Length - 1);
            bools_03[rnd_value_1] = true;
            bools_03[rnd_value_2] = true;
            bools_03[rnd_value_3] = true;
            bools_03[rnd_value_4] = true;
            bools_03[rnd_value_5] = true;
            bools_03[rnd_value_6] = true;
        }

        private void CarrierInterfaceSim_UnloadComplete(AVEHICLE vh, string carrier_id)
        {
            //vh.CatchPLCCSTInterfacelog();
            VehicleCSTInterface vehicleCSTInterface = new VehicleCSTInterface();
            bool[] bools_01 = new bool[16];
            bool[] bools_02 = new bool[16];
            bool[] bools_03 = new bool[16];
            bool[] bools_04 = new bool[16];
            bool[] bools_05 = new bool[16];
            bool[] bools_06 = new bool[16];
            bool[] bools_07 = new bool[16];
            bool[] bools_08 = new bool[16];
            bool[] bools_09 = new bool[16];
            bool[] bools_10 = new bool[16];

            bools_01[3] = true;

            bools_02[03] = true; bools_02[9] = true; bools_02[12] = true; bools_02[14] = true;
            bools_02[15] = true;

            bools_03[3] = true; bools_03[9] = true; bools_03[10] = true; bools_03[12] = true;
            bools_03[14] = true; bools_03[15] = true;

            bools_04[3] = true; bools_04[4] = true; bools_04[9] = true; bools_04[10] = true;
            bools_04[12] = true; bools_04[14] = true; bools_04[15] = true;

            bools_05[3] = true; bools_05[4] = true; bools_05[9] = true; bools_05[10] = true;
            bools_05[11] = true; bools_05[12] = true; bools_05[14] = true; bools_05[15] = true;

            bools_06[3] = true; bools_06[4] = true; bools_06[5] = true; bools_06[9] = true;
            bools_06[10] = true; bools_06[11] = true; bools_06[12] = true; bools_06[14] = true;
            bools_06[15] = true;

            bools_07[3] = true; bools_07[4] = true; bools_07[5] = true; bools_07[10] = true;
            bools_07[11] = true; bools_07[12] = true; bools_07[14] = true; bools_07[15] = true;

            bools_08[3] = true; bools_08[6] = true; bools_08[10] = true; bools_08[11] = true;
            bools_08[12] = true; bools_08[14] = true; bools_08[15] = true;

            bools_09[3] = true; bools_09[6] = true; bools_09[10] = true; bools_09[12] = true;
            bools_09[14] = true; bools_09[15] = true;

            bools_10[3] = true;
            List<bool[]> lst_bools = new List<bool[]>()
            {
                bools_01,bools_02,bools_03,bools_04,bools_05,bools_06,bools_07,bools_08,bools_09,bools_10,
            };
            if (DebugParameter.isTestCarrierInterfaceError)
            {
                RandomSetCSTInterfaceBool(bools_03);
                RandomSetCSTInterfaceBool(bools_04);
                RandomSetCSTInterfaceBool(bools_05);
                RandomSetCSTInterfaceBool(bools_06);
                RandomSetCSTInterfaceBool(bools_07);
                RandomSetCSTInterfaceBool(bools_08);
                RandomSetCSTInterfaceBool(bools_09);
            }
            string port_id = "";
            scApp.MapBLL.getPortID(vh.CUR_ADR_ID, out port_id);
            //scApp.PortBLL.OperateCatch.updatePortStationCSTExistStatus(port_id, carrier_id);

            CarrierInterface_LogOut(vh.VEHICLE_ID, port_id, lst_bools);
        }

        private static void CarrierInterface_LogOut(string vh_id, string port_id, List<bool[]> lst_bools)
        {
            VehicleCSTInterface vehicleCSTInterface = new VehicleCSTInterface();
            foreach (var bools in lst_bools)
            {
                DateTime now_time = DateTime.Now;
                vehicleCSTInterface.Details.Add(new VehicleCSTInterface.CSTInterfaceDetail()
                {
                    EQ_ID = vh_id,
                    //PORT_ID = port_id,
                    LogIndex = $"Recode{nameof(VehicleCSTInterface)}",
                    CSTInterface = bools,
                    Year = (ushort)now_time.Year,
                    Month = (ushort)now_time.Month,
                    Day = (ushort)now_time.Day,
                    Hour = (ushort)now_time.Hour,
                    Minute = (ushort)now_time.Minute,
                    Second = (ushort)now_time.Second,
                    Millisecond = (ushort)now_time.Millisecond,
                });
                SpinWait.SpinUntil(() => false, 100);
            }
            foreach (var detail in vehicleCSTInterface.Details)
            {
                LogManager.GetLogger("RecodeVehicleCSTInterface").Info(detail.ToString());
            }
        }

        #endregion TEST
    }
}
