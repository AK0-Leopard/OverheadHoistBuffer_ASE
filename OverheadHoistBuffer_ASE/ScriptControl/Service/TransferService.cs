//*********************************************************************************
//      MESDefaultMapAction.cs
//*********************************************************************************
// File Name: MESDefaultMapAction.cs
// Description: 與EAP通訊的劇本
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag          Description
// ------------- -------------  -------------  ------       -----------------------------
// 2020/02/23    Kevin Wei      N/A            B0.01        功能Function，更新Carrier location與儲位狀態
// 2020/04/23    Kevin Wei      N/A            B0.02        拿掉料交易的保護
// 2020/04/17    Jason Wu       N/A            B0.03        加入insert Box ID 為 ERROR1 時，做一次Alarm Set 與 Alarm Clear 以記錄在 MCS
// 2020/05/21    Jason Wu       N/A            A20.05.21    嘗試優化派送命令之優先邏輯
// 2020/05/27    Jason Wu       N/A            A20.05.27    嘗試優化退補空box 之shelf 位置尋找
// 2020/05/28    Jason Wu       N/A            A20.05.28.0  用於計算並調整整條線的空box數量
// 2020/05/29    Jason Wu       N/A            A20.05.29.0  用於計算並調整整條線的空box數量(以規劃圖進行修改)
// 2020/06/09    Jason Wu       N/A            A20.06.09.0  修改getAddressID也能從vehicle取得
// 2020/06/12    Jason Wu       N/A            A20.06.12.0  新增CanExcuteUnloadTransferAGVStationFromAGVC()處理判定切換Mode Type流程及觸發命令派送。
// 2020/06/15    Jason Wu       N/A            A20.06.15.0  新增新增CanExcuteUnloadTransferAGVStationFromAGVC()後續處理流程。
// 2020/06/16    Jason Wu       N/A            A20.06.16.0  新增確認該AGVport是否可用的優先流程FilterOfAGVPort()。
//**********************************************************************************

using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Data.SECS.ASE;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using Mirle.Hlts.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using static com.mirle.ibg3k0.sc.ACMD_MCS;
using static com.mirle.ibg3k0.sc.ALINE;
using static com.mirle.ibg3k0.sc.AVEHICLE;

namespace com.mirle.ibg3k0.sc.Service
{
    //TEST
    //A20.06.13.0
    public enum EmptyBoxNumber
    {
        NO_EMPTY_BOX = 0,
        ONE_EMPTY_BOX = 1,
        TWO_EMPTY_BOX = 2
    }

    public class PortINIData
    {
        #region 共有屬性
        public string PortName { get; set; }
        public string UnitType { get; set; }
        public string ZoneName { get; set; }
        public DateTime portStateErrorLogTime { get; set; }  //執行的命令 port 狀態異常 10 秒記 Log 一次
        #endregion        
        #region CRANE 才有用到的屬性
        public bool craneLoading { get; set; }
        public bool craneUnLoading { get; set; }
        #endregion
        #region CV_Port 才有用到的屬性

        public int Stage { get; set; }
        public int nowStage { get; set; }
        public bool openAGV_Station { get; set; }
        public bool openAGV_AutoPortType { get; set; }
        public bool movebackBOXsleep { get; set; }      //0601 士偉提出 AGV 在 OutMode 的時候判斷退BOX時，先延遲300毫秒再檢查一次，若還是退BOX結果再退
        public string IgnoreModeChange { get; set; }    // Y = 忽略 PLC 訊號，一律 Port 當，N = 讀取 PLC 正常上報

        #endregion
        #region CV_Port、CRANE 才有用到的屬性

        public int timeOutForAutoUD { get; set; }   //卡匣停在 Port 或 車上 的停留時間超過幾秒，就自動搬到儲位
        public string timeOutForAutoInZone { get; set; }    //timeOutForAutoUD 超過時間自動搬到哪個 Zone

        #endregion        
    }
    public class WaitInOutLog
    {
        public string time { get; set; }
        public string CSTID { get; set; }
        public string BOXID { get; set; }
        public string LOC { get; set; }
        public E_CSTState type { get; set; }
    }

    public enum UnitType
    {
        OHCV,
        AGV,
        NTB,
        SHELF,
        STK,
        CRANE,
        ZONE,
        AGVZONE,
    }
    public enum reportMCSCommandType
    {
        Cancel = 0,
        Abort = 1,
        Transfer = 2,
    }
    public class TransferService
    {
        #region 屬性

        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public NLog.Logger TransferServiceLogger = NLog.LogManager.GetLogger("TransferServiceLogger");
        public NLog.Logger AGVCTriggerLogger = NLog.LogManager.GetLogger("TransferServiceLogger");

        private SCApplication scApp = null;
        private ReportBLL reportBLL = null;
        private LineBLL lineBLL = null;
        private ALINE line = null;
        private CMDBLL cmdBLL = null;
        private CassetteDataBLL cassette_dataBLL = null;
        //private PortValueDefMapAction portValueDefMapAction = null;
        private PortDefBLL portDefBLL = null;
        private ShelfDefBLL shelfDefBLL = null;
        private ZoneDefBLL zoneBLL = null;
        private AlarmBLL alarmBLL = null;
        private VehicleBLL vehicleBLL = null;

        int ohtCmdTimeOut = 0;  //詢問 OHT 命令被拒絕記錄LOG，1分鐘記錄一次
        int ohtIdleTimeOut = 0;

        bool iniStatus = false; //初始化旗標

        DateTime updateTime;    //定時更新狀態
        DateTime ohtTimeout;    //AVEHICLE 裡面沒有閒置的車輛，無法執行1分鐘紀錄Log

        public Dictionary<string, PortINIData> portINIData = null;
        public Dictionary<string, WaitInOutLog> waitInLog = new Dictionary<string, WaitInOutLog>();
        public Dictionary<string, WaitInOutLog> waitOutLog = new Dictionary<string, WaitInOutLog>();
        public int cstIdle = 600;       //卡匣停在 Port上或車上，超過 30 秒沒搬，自動搬往儲位

        #endregion

        #region 初始化
        public TransferService()
        {

        }

        public void start(SCApplication _app)
        {
            TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "TransferService >> 初始化開始------------------------------------");
            scApp = _app;
            reportBLL = _app.ReportBLL;
            lineBLL = _app.LineBLL;
            line = scApp.getEQObjCacheManager().getLine();
            cmdBLL = _app.CMDBLL;
            cassette_dataBLL = _app.CassetteDataBLL;
            portDefBLL = _app.PortDefBLL;
            shelfDefBLL = _app.ShelfDefBLL;
            zoneBLL = _app.ZoneDefBLL;
            alarmBLL = _app.AlarmBLL;
            vehicleBLL = _app.VehicleBLL;

            line.addEventHandler(nameof(ConnectionInfoService), nameof(line.MCSCommandAutoAssign), PublishTransferInfo);

            initPublish(line);

            TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "TransferService >> 初始化結束------------------------------------");
        }
        public void SetPortINIData()
        {
            try
            {
                portINIData = new Dictionary<string, PortINIData>();
                portINIData.Clear();
                foreach (var v in scApp.PortDefBLL.GetOHB_PortData(line.LINE_ID))
                {
                    if (v.UnitType.Trim() == UnitType.OHCV.ToString()
                     || v.UnitType.Trim() == UnitType.NTB.ToString()
                     || v.UnitType.Trim() == UnitType.AGV.ToString()
                     || v.UnitType.Trim() == UnitType.STK.ToString()
                       )
                    {
                        for (int i = 1; i <= (int)v.Stage; i++)
                        {
                            PortINIData data = new PortINIData();

                            data.UnitType = v.UnitType.Trim();

                            //if (v.UnitType.Trim() == UnitType.AGV.ToString())
                            //{
                            //    data.ZoneName = v.ZoneName.Trim();
                            //}
                            //else  
                            //{
                            //    data.ZoneName = v.PLCPortID.Trim();
                            //}

                            data.ZoneName = v.PLCPortID.Trim();
                            data.Stage = (int)v.Stage;
                            data.openAGV_Station = false;
                            data.openAGV_AutoPortType = false;
                            data.nowStage = i;
                            data.movebackBOXsleep = false;
                            data.timeOutForAutoUD = (int)v.TimeOutForAutoUD;
                            data.timeOutForAutoInZone = v.TimeOutForAutoInZone;

                            if (i == data.Stage)
                            {
                                data.PortName = v.PLCPortID.Trim();
                            }
                            else
                            {
                                data.PortName = v.PLCPortID.Trim() + ((CassetteData.OHCV_STAGE)i).ToString();
                            }

                            AddPortINIData(data);
                            
                            if (v.UnitType.Trim() == UnitType.AGV.ToString())
                            {
                                if (portINIData.ContainsKey(v.ZoneName) == false)
                                {
                                    PortINIData agvdata = new PortINIData();
                                    agvdata.PortName = v.ZoneName.Trim();
                                    agvdata.UnitType = UnitType.AGVZONE.ToString();
                                    agvdata.ZoneName = v.ZoneName.Trim();
                                    agvdata.Stage = 1;

                                    AddPortINIData(data);
                                }
                            }

                        }
                    }
                }

                foreach (var v in vehicleBLL.loadAllVehicle())
                {
                    PortINIData data = new PortINIData();
                    data.PortName = v.VEHICLE_ID.Trim();
                    data.UnitType = UnitType.CRANE.ToString();
                    data.ZoneName = v.VEHICLE_ID.Trim();
                    data.Stage = 1;
                    data.timeOutForAutoUD = cstIdle;
                    //data.timeOutForAutoInZone = v.TimeOutForAutoInZone;

                    AddPortINIData(data);
                }

                foreach (var v in scApp.ShelfDefBLL.LoadShelf())
                {
                    PortINIData data = new PortINIData();
                    data.PortName = v.ShelfID.Trim();
                    data.UnitType = UnitType.SHELF.ToString();
                    data.ZoneName = v.ZoneID.Trim();
                    data.Stage = 1;

                    AddPortINIData(data);
                }

                foreach (var v in scApp.ZoneDefBLL.loadZoneData())
                {
                    PortINIData data = new PortINIData();
                    data.PortName = v.ZoneID.Trim();
                    data.UnitType = UnitType.ZONE.ToString();
                    data.ZoneName = v.ZoneID.Trim();
                    data.Stage = 1;

                    AddPortINIData(data);
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "SetPortData");
            }
        }
        public void AddPortINIData(PortINIData data)
        {
            try
            {
                portINIData.Add(data.PortName, data);
                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "PortID:" + data.PortName + " PortType:" + data.UnitType);
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "AddPortINIData");
            }
        }
        public void iniShelfData()  //檢查目前 Cassette 是否在儲位上，沒有的話，設成空儲位
        {
            List<string> boxLoc = cassette_dataBLL.GetAllBoxLoc();
            foreach (var v in shelfDefBLL.LoadShelf())
            {
                if (boxLoc.Contains(v.ShelfID))
                {
                    shelfDefBLL.updateStatus(v.ShelfID, ShelfDef.E_ShelfState.Stored);
                    TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + $"iniShelfData: {v.ShelfID} has box");
                }
                else
                {
                    shelfDefBLL.updateStatus(v.ShelfID, ShelfDef.E_ShelfState.EmptyShelf);
                }
            }
        }
        public void EmptyShelf()
        {
            foreach (var v in shelfDefBLL.GetReserveShelf())
            {
                CassetteData cstData = cassette_dataBLL.loadCassetteDataByLoc(v.ShelfID);
                ACMD_MCS source = cmdBLL.GetCmdDataBySource(v.ShelfID);
                ACMD_MCS dest = cmdBLL.GetCmdDataByDest(v.ShelfID).FirstOrDefault();

                if (cstData == null && source == null && dest == null)
                {
                    shelfDefBLL.updateStatus(v.ShelfID, ShelfDef.E_ShelfState.EmptyShelf);
                }
            }
        }
        public void AlliniPortData()
        {
            foreach (var v in scApp.PortDefBLL.GetOHB_CVPortData(line.LINE_ID))
            {
                iniPortData(v.PLCPortID);
            }
        }
        public void iniPortData(string portName) //初始 Port 資料
        {
            try
            {
                portName = portName.Trim();
                PortPLCInfo portValue = GetPLC_PortData(portName);

                if (GetIgnoreModeChange(portValue))
                {
                    return;
                }

                if (portValue == null)
                {
                    TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "PortPLCInfo >> PLCPortID: " + portName + " = Null");
                }
                else
                {
                    #region Port流向處理
                    E_PortType portType = E_PortType.In;

                    if (portValue.IsInputMode)  //檢查Port流向
                    {
                        portType = E_PortType.In;
                    }
                    else if (portValue.IsOutputMode)
                    {
                        portType = E_PortType.Out;
                    }

                    ReportPortType(portName, portType, "iniPortData");
                    #endregion
                    #region Port狀態處理                    
                    PLC_ReportPortInOutService(portValue);
                    #endregion
                    #region Port卡匣資料處理

                    if (portValue.OpAutoMode && portValue.PortWaitIn)
                    {
                        PLC_ReportPortWaitIn(portValue, "iniPortData");
                    }
                    #endregion
                }

            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "UpDBPortTypeData");
            }
        }
        public void updateAGVStation()  //定時更新AGV狀態
        {
            foreach (var v in scApp.PortDefBLL.GetOHB_CVPortData(line.LINE_ID))
            {
                string portName = v.PLCPortID.Trim();
                if (isUnitType(portName, UnitType.AGV))
                {
                    PortPLCInfo portValue = GetPLC_PortData(portName);

                    PLC_AGV_Station(portValue, "updateAGVStation");
                }
            }
        }
        public void iniOHTData(string craneName)
        {
            //AVEHICLE vehicle = scApp.VehicleService.GetVehicleDataByVehicleID(craneName);
            AVEHICLE vehicle = scApp.VehicleBLL.cache.getVhByID(craneName);

            //ACMD_MCS cmd = cmdBLL.getCMD_ByOHTName(craneName).FirstOrDefault();
            //    AVEHICLE.HAS_BOX
            //    AVEHICLE.HAS_CST 車上有沒有料

            //if (cmd != null)
            //{
            //}

            //foreach (var v in vehicleBLL.loadAllVehicle())    //暫時註解，發生程式重啟，OHT連上，沒有清除
            //{
            //    if (v.isTcpIpConnect == false)
            //    {
            //        OHBC_AlarmSet(v.VEHICLE_ID, "99999");
            //    }
            //}
        }
        public void iniCstData()    //卡匣資料初始化，刪除殘帳
        {

        }
        public void iniCmdData()    //命令資料初始化，刪除殘帳
        {

        }
        private void initPublish(ALINE line)
        {
            PublishTransferInfo(line, null);
            //PublishOnlineCheckInfo(line, null);
            //PublishPingCheckInfo(line, null);
        }
        private void PublishTransferInfo(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                ALINE line = sender as ALINE;
                if (sender == null) return;
                byte[] line_serialize = BLL.LineBLL.Convert2GPB_TransferInfo(line);
                scApp.getNatsManager().PublishAsync
                    (SCAppConstants.NATS_SUBJECT_TRANSFER, line_serialize);

                //TODO 要改用GPP傳送
                //var line_Serialize = ZeroFormatter.ZeroFormatterSerializer.Serialize(line);
                //scApp.getNatsManager().PublishAsync
                //    (string.Format(SCAppConstants.NATS_SUBJECT_LINE_INFO), line_Serialize);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }
        #endregion

        #region 流程

        private long syncTranCmdPoint = 0;
        public void TransferRun()
        {
            if (Interlocked.Exchange(ref syncTranCmdPoint, 1) == 0)
            {
                try
                {
                    if (iniStatus == false)
                    {
                        if (line.ServiceMode == SCAppConstants.AppServiceMode.Active)
                        {
                            #region Port資料初始化
                            TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "TransferService >> Port資料初始化開始------------------------------------");

                            SetPortINIData();
                            iniShelfData();
                            AlliniPortData();
                            //iniOHTData();

                            updateTime = DateTime.Now;

                            TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "TransferService >> Port資料初始化結束------------------------------------");
                            iniStatus = true;
                            #endregion
                        }
                    }

                    #region Port狀態處理
                    TimeSpan updateTimeSpan = DateTime.Now - updateTime;

                    if (updateTimeSpan.Seconds >= 10)
                    {
                        updateAGVStation();
                        updateTime = DateTime.Now;
                    }
                    #endregion
                    #region 卡匣資料處理
                    var cstDataList = cassette_dataBLL.LoadCassetteDataByNotCompleted();
                    BoxDataHandler(cstDataList);
                    #endregion
                    #region 命令資料處理
                    //var vehicleData = scApp.VehicleBLL.loadAllVehicle();
                    var vehicleData = scApp.VehicleBLL.cache.loadVhs();

                    int ohtIdle = vehicleData.Where(data => string.IsNullOrWhiteSpace(data.OHTC_CMD)).Count();

                    if (ohtIdle != 0)    //有閒置的車輛在開始派命令
                    {
                        var cmdData = cmdBLL.LoadCmdData();

                        if (cmdData.Count != 0)
                        {
                            #region 說明
                            // A20.05.21
                            // 由於目前處理命令的狀況是，由此處確認完是否有車是nocommand 後，就先load 入所有命令後，以priority順序排列命令。
                            //  接著以該順序判定各種條件後，詢問是否有車輛可以進行實作。
                            //  但此方法會發生的問題是，只能達成優先派工，卻無法判斷是否為最佳車輛去執行。
                            //  並且目前過程的趕車命令與MCS命令並無優先序，故有許多無效運作。
                            //  此問題在 盡可能少做更動 的情況下我認為分為2部分：
                            //
                            //  1. 要能判斷目前車輛執行中的是否為趕車命令，並在有MCS命令未被執行時，cancel 趕車命令之車輛，並進入此給予MCS命令之流程。
                            //      (此部分由於趕車是由reserve 部分進行觸發，且此觸發之頻率遠高於MCS Cmd 的timer，故是否有辦法讓特定車輛暫時先不接受趕車命令？
                            //       或者能否加快 MCS Cmd 的 timer ？)
                            //
                            //  2. 給定一個基礎標準點(10001之類的)，並在上述之  
                            //          var cmdData = cmdBLL.LoadCmdData(); 
                            //      load 入所有命令之後，對所有命令的source 與該點進行一次距離計算。
                            //      並對各個點到基準點距離與no command的任一車輛相對基準點的距離計算之後，
                            //      依此為基準sort ，即可得到一個新的cmdData List。
                            //      在只有單一台車no command下，此cmdData List將會是以該車的最佳命令給予的。
                            //      在有多台車no command下，需在後續對單一Cmd v命令詢問OHTC
                            //          ohtReport = cmdBLL.generateOHTCommand(v);   //詢問OHT是否可執行命令
                            //      在此function 內部再做一次判定，須對多台no command 的車皆計算一次，即可得出最佳車輛執行此命令。
                            //      目前已有 findBestSuitableVhStepByNearest() 在做此問題。
                            //      此方法有可能在後續對多台no command 車做計算時，
                            //      最終將命令v 指派給非一開始計算cmdData List 之台車，
                            //      但會是該命令v 最佳的執行車輛。
                            //      然後由於在成功派送後會break 掉此foreach 迴圈，故下一次判斷時，就會繼續對no command 台車派送最佳命令。
                            //
                            //  取消下一行註解即可使用。 但不確定邏輯以及程式部分是否完全沒有問題。
                            #endregion
                            cmdData = scApp.CMDBLL.doSortMCSCmdDataByDistanceFromHostSourceToVehicle(cmdData, vehicleData);

                            foreach (var v in cmdData)
                            {
                                #region 每分鐘權限 + 1
                                if (string.IsNullOrWhiteSpace(v.TIME_PRIORITY.ToString()))
                                {
                                    cmdBLL.updateCMD_MCS_TimePriority(v.CMD_ID, 0);
                                }
                                else
                                {
                                    DateTime nowTime = DateTime.Now;
                                    DateTime cmdTime = v.CMD_INSER_TIME.AddMinutes(v.TIME_PRIORITY);
                                    TimeSpan span = nowTime - cmdTime;
                                    if (span.Minutes >= 1)
                                    {
                                        cmdBLL.updateCMD_MCS_TimePriority(v.CMD_ID, v.TIME_PRIORITY + 1);
                                        cmdBLL.updateCMD_MCS_sumPriority(v.CMD_ID);
                                    }
                                }
                                #endregion
                                #region PLC控制命令
                                if (v.CMDTYPE == CmdType.PortTypeChange.ToString())
                                {
                                    PortPLCInfo portInfo = GetPLC_PortData(v.HOSTSOURCE);

                                    if (portInfo.OpAutoMode == false || portInfo.IsModeChangable == false)   // || (int)portData.AGVState == SECSConst.PortState_OutService
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        PortDef portDef = portDefBLL.GetPortData(v.HOSTSOURCE);
                                        E_PortType portType = (E_PortType)Enum.Parse(typeof(E_PortType), v.HOSTDESTINATION);

                                        if (portDef.PortType != portType)
                                        {
                                            if (portInfo.IsInputMode && portType == E_PortType.In)
                                            {
                                                ReportPortType(portInfo.EQ_ID, E_PortType.In, "TransferRun");
                                            }

                                            if (portInfo.IsOutputMode && portType == E_PortType.Out)
                                            {
                                                ReportPortType(portInfo.EQ_ID, E_PortType.Out, "TransferRun");
                                            }

                                            #region Log
                                            TransferServiceLogger.Info
                                            (
                                                DateTime.Now.ToString("HH:mm:ss.fff ")
                                                + "PortTypeCmd >> PortName:" + v.HOSTSOURCE + "   Type:" + portType
                                            );
                                            #endregion
                                            PortTypeChange(v.HOSTSOURCE, portType, "TransferRun");
                                        }
                                        else
                                        {
                                            cmdBLL.DeleteCmd(v.CMD_ID);
                                        }
                                    }
                                }
                                #endregion
                                #region 搬送命令
                                else
                                {
                                    TransferCommandHandler(v);
                                }
                                #endregion
                            }
                        }

                        ohtTimeout = DateTime.Now;
                        ohtIdleTimeOut = 0;
                    }
                    else
                    {
                        TimeSpan timeSpan = DateTime.Now - ohtTimeout;

                        if (timeSpan.Minutes >= ohtIdleTimeOut)
                        {
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ")
                                + "OHB >> OHT| 資料表 AVEHICLE 沒有閒置的車輛可使用"
                            );

                            ohtIdleTimeOut++;
                        }
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    TransferServiceLogger.Error(ex, "TransferRun");
                }
                finally
                {
                    Interlocked.Exchange(ref syncTranCmdPoint, 0);
                }
            }
        }

        private void TransferCommandHandler(ACMD_MCS mcsCmd)
        {
            switch (mcsCmd.TRANSFERSTATE)
            {
                #region E_TRAN_STATUS.Queue
                case E_TRAN_STATUS.Queue:

                    if (isUnitType(mcsCmd.HOSTDESTINATION, UnitType.ZONE))  //若 Zone 上沒有儲位，目的 Port 會為 ZoneName，並上報 MCS
                    {
                        cmdBLL.updateCMD_MCS_TranStatus(mcsCmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);

                        reportBLL.ReportTransferInitiated(mcsCmd.CMD_ID.Trim());
                        reportBLL.ReportTransferCompleted(mcsCmd, null, ResultCode.ZoneIsfull);
                        break;
                    }

                    bool sourcePortType = false;
                    bool destPortType = false;
                    string source = "";
                    #region 檢查來源狀態
                    if (string.IsNullOrWhiteSpace(mcsCmd.RelayStation))  //檢查命令是否先搬到中繼站
                    {
                        if (isAGVZone(mcsCmd.HOSTSOURCE)) //檢查來源是不是AGVZONE
                        {
                            string agvPortName = GetAGV_InModeInServicePortName(mcsCmd.HOSTSOURCE);
                            if (string.IsNullOrWhiteSpace(agvPortName))
                            {
                                sourcePortType = false;
                            }
                            else
                            {
                                sourcePortType = true;
                                mcsCmd.HOSTSOURCE = agvPortName;
                            }
                        }
                        else
                        {
                            sourcePortType = AreSourceEnable(mcsCmd.HOSTSOURCE);
                            source = mcsCmd.HOSTSOURCE;
                        }
                    }
                    else
                    {
                        sourcePortType = AreSourceEnable(mcsCmd.RelayStation);
                        source = mcsCmd.RelayStation;
                    }
                    #endregion
                    #region 檢查目的狀態
                    if (isAGVZone(mcsCmd.HOSTDESTINATION))
                    {
                        string agvPortName = GetAGV_OutModeInServicePortName(mcsCmd.HOSTDESTINATION);
                        if (string.IsNullOrWhiteSpace(agvPortName))
                        {
                            destPortType = false;
                        }
                        else
                        {
                            destPortType = true;
                            mcsCmd.HOSTDESTINATION = agvPortName;
                        }
                    }
                    else
                    {
                        destPortType = AreDestEnable(mcsCmd.HOSTDESTINATION);
                    }
                    #endregion

                    if (sourcePortType && destPortType)
                    {
                        OHT_TransportRequest(mcsCmd);
                    }
                    else if (sourcePortType && isCVPort(source) && destPortType == false && isCVPort(mcsCmd.HOSTDESTINATION))
                    {
                        //來源目的都是 CV Port 且 目的不能搬，觸發將卡匣送至中繼站
                        PortPLCInfo plcInfo = GetPLC_PortData(mcsCmd.HOSTDESTINATION);

                        if (plcInfo.OpAutoMode == false)
                        {
                            ACMD_MCS cmdRelay = mcsCmd.Clone();

                            List<ShelfDef> shelfData = shelfDefBLL.GetEmptyAndEnableShelf();

                            cmdRelay.HOSTDESTINATION = GetShelfRecentLocation(shelfData, mcsCmd.HOSTDESTINATION);

                            if (string.IsNullOrWhiteSpace(cmdRelay.HOSTDESTINATION) == false)
                            {
                                if (OHT_TransportRequest(cmdRelay))
                                {
                                    ShelfReserved(cmdRelay.HOSTSOURCE, cmdRelay.HOSTDESTINATION);

                                    cmdBLL.updateCMD_MCS_RelayStation(mcsCmd.CMD_ID, cmdRelay.HOSTDESTINATION);
                                }
                            }
                        }
                    }

                    break;
                #endregion
                #region E_TRAN_STATUS.Transferring
                case E_TRAN_STATUS.Transferring:
                    switch (mcsCmd.COMMANDSTATE)
                    {
                        case COMMAND_iIdle:
                            #region Log
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ")
                                + "OHT >> OHB|OHT_TransferProcess 發現車子未回應 COMMANDSTATE = COMMAND_iIdle 自動變回 Queue :\n"
                                + GetCmdLog(mcsCmd)
                            );
                            #endregion
                            cmdBLL.updateCMD_MCS_TranStatus(mcsCmd.CMD_ID, E_TRAN_STATUS.Queue);
                            break;
                        case COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH:
                            #region Log
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ")
                                + "OHT >> OHB|OHT_TransferProcess 發現殘存 COMMANDSTATE = COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH :\n"
                                + GetCmdLog(mcsCmd)
                            );
                            #endregion
                            cmdBLL.updateCMD_MCS_TranStatus(mcsCmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);
                            break;
                    }
                    break;
                #endregion
                #region E_TRAN_STATUS.Paused
                case E_TRAN_STATUS.Paused:
                    break;
                #endregion
                #region E_TRAN_STATUS.Canceling
                case E_TRAN_STATUS.Canceling:
                    break;
                #endregion
                #region E_TRAN_STATUS.Aborting
                case E_TRAN_STATUS.Aborting:
                    break;
                    #endregion
            }
        }

        private void BoxDataHandler(List<CassetteData> cstDataList)
        {
            foreach (var cst in cstDataList)
            {
                int cstTimeOut = portINIData[cst.Carrier_LOC.Trim()].timeOutForAutoUD;
                string zoneName = portINIData[cst.Carrier_LOC.Trim()].ZoneName;

                bool success = false;

                if (isCVPort(zoneName) || isUnitType(zoneName, UnitType.CRANE))
                {
                    success = true;
                }

                if (cstTimeOut != 0 && success)
                {
                    TimeSpan cstTimeSpan = DateTime.Now - DateTime.Parse(cst.TrnDT);

                    if (cstTimeSpan.Seconds >= cstTimeOut)   //停在Port上 30秒(之後要設成可調)，自動搬到儲位上
                    {
                        ACMD_MCS cmd = cmdBLL.getCMD_ByBoxID(cst.BOXID);

                        if (cmd == null)
                        {
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ")
                                + "OHB >> OHB| 卡匣停留 " + cstTimeOut + " 秒，尚未搬走，產生自動搬走命令 " + GetCstLog(cst)
                            );

                            List<ShelfDef> shelfData = null;

                            if (isCVPort(zoneName))
                            {
                                string timeOutZone = portINIData[cst.Carrier_LOC.Trim()].timeOutForAutoInZone;
                                shelfData = shelfDefBLL.GetEmptyAndEnableShelfByZone(timeOutZone);
                            }

                            string shelfID = GetShelfRecentLocation(shelfData, cst.Carrier_LOC.Trim());

                            if (string.IsNullOrWhiteSpace(shelfID) == false)
                            {
                                string cmdSource = cst.Carrier_LOC.Trim();
                                string cmdDest = "";
                                cmdDest = shelfID;

                                Manual_InsertCmd(cmdSource, cmdDest, "", 5, "cstTimeOut", CmdType.OHBC);
                            }
                        }
                    }
                }
            }
        }

        public bool OHT_TransportRequest(ACMD_MCS cmd)  //詢問 OHT 此筆命令是否能執行 
        {
            if (string.IsNullOrWhiteSpace(cmd.RelayStation) == false)
            {
                cmd.HOSTSOURCE = cmd.RelayStation;
            }

            bool ohtReport = cmdBLL.generateOHTCommand(cmd); //OHT回傳是否可執行搬送命令

            if (ohtReport)
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "OHB >> OHT|命令執行成功，" + GetCmdLog(cmd)
                );

                if (isCVPort(cmd.HOSTDESTINATION))
                {
                    PortCommanding(cmd.HOSTDESTINATION, true);
                }

                cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.Transferring);

                ohtCmdTimeOut = 0;

                if (isUnitType(cmd.HOSTSOURCE, UnitType.SHELF) && string.IsNullOrWhiteSpace(cmd.RelayStation) == false)
                {
                    ShelfReserved(cmd.HOSTSOURCE, cmd.HOSTDESTINATION);
                }
            }
            else
            {
                DateTime cmdTime = cmd.CMD_INSER_TIME;
                TimeSpan timeSpan = DateTime.Now - cmdTime;
                if (timeSpan.Minutes >= 1 + ohtCmdTimeOut)
                {
                    TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> OHT|OHT回應不能搬送");
                    ohtCmdTimeOut++;
                }
            }

            return ohtReport;
        }
        public bool AreSourceAndDestEnable(string sourceName, string destName)    //檢查來源目的狀態是否正確
        {
            try
            {
                bool sourcePortType = false;
                bool destPortType = false;

                sourcePortType = AreSourceEnable(sourceName);
                destPortType = AreDestEnable(destName);

                return (sourcePortType && destPortType);
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "AreSourceAndDestEnable");
                return false;
            }
        }
        public bool AreSourceEnable(string sourceName)  //檢查來源狀態是否正確
        {
            try
            {
                sourceName = sourceName.Trim();
                bool sourcePortType = false;
                string sourceState = "";

                #region 檢查來源 Port 流向
                if (isCVPort(sourceName) == false)
                {
                    sourcePortType = true;
                }
                else
                {
                    PortPLCInfo sourcePort = GetPLC_PortData(sourceName);

                    if (sourcePort != null)
                    {
                        if (sourcePort.OpAutoMode)
                        {
                            if (sourcePort.IsReadyToUnload)
                            {
                                if (isUnitType(sourceName, UnitType.AGV))
                                {
                                    sourcePortType = true;
                                }
                                else
                                {
                                    if (sourcePort.IsInputMode)
                                    {
                                        sourcePortType = true;
                                    }
                                    else
                                    {
                                        if (sourcePort.IsModeChangable)
                                        {
                                            string cmdID = "PortTypeChange-" + sourcePort.EQ_ID.Trim() + ">>" + E_PortType.In;

                                            if (cmdBLL.getCMD_MCSByID(cmdID) == null)
                                            {
                                                //若來源流向錯誤且沒有流向切換命令，就新建
                                                SetPortTypeCmd(sourcePort.EQ_ID.Trim(), E_PortType.In);  //要測時，把註解拿掉
                                            }
                                        }

                                        sourceState = sourceState + " IsInputMode:" + sourcePort.IsInputMode;
                                    }
                                }
                            }
                            else
                            {
                                sourceState = sourceState + " IsReadyToUnload:" + sourcePort.IsReadyToUnload;
                            }
                        }
                        else
                        {
                            sourceState = sourceState + " OpAutoMode:" + sourcePort.OpAutoMode;
                        }
                    }
                    else
                    {
                        sourceState = sourceState + " PortPLCInfo " + sourceName + " = null";
                    }
                }
                #endregion

                if (sourcePortType == false)
                {
                    sourceState = sourceState + " ";

                    TimeSpan timeSpan = DateTime.Now - portINIData[sourceName].portStateErrorLogTime;

                    if (timeSpan.Seconds >= 10)
                    {
                        portINIData[sourceName].portStateErrorLogTime = DateTime.Now;

                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ") +
                            "OHB >> PLC|來源 " + sourceName + " 狀態錯誤 " + sourceState
                        );
                    }
                }

                return sourcePortType;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "AreSourceEnable");
                return false;
            }
        }
        public bool AreDestEnable(string destName)    //檢查目的狀態是否正確
        {
            try
            {
                destName = destName.Trim();
                bool destPortType = false;
                string destState = "";

                #region 檢查目的 Port 流向
                if (isCVPort(destName) == false)
                {
                    destPortType = true;
                }
                else
                {
                    PortPLCInfo destPort = GetPLC_PortData(destName);

                    if (destPort != null)
                    {
                        if (destPort.OpAutoMode)
                        {
                            if (destPort.IsReadyToLoad)
                            {
                                if (isUnitType(destName, UnitType.AGV))
                                {
                                    destPortType = true;
                                }
                                else
                                {
                                    if (destPort.IsOutputMode)
                                    {
                                        destPortType = true;
                                    }
                                    else
                                    {
                                        if (destPort.IsModeChangable)
                                        {
                                            string cmdID = "PortTypeChange-" + destPort.EQ_ID.Trim() + ">>" + E_PortType.Out;

                                            if (cmdBLL.getCMD_MCSByID(cmdID) == null)
                                            {
                                                SetPortTypeCmd(destPort.EQ_ID.Trim(), E_PortType.Out);    //要測時，把註解拿掉
                                            }
                                        }

                                        destState = destState + " IsOutputMode:" + destPort.IsOutputMode;
                                    }
                                }
                            }
                            else
                            {
                                destState = destState + " IsReadyToLoad: " + destPort.IsReadyToLoad;
                            }
                        }
                        else
                        {
                            destState = destState + " OpAutoMode:" + destPort.OpAutoMode;
                        }
                    }
                    else
                    {
                        destState = destState + " PortPLCInfo " + destName + " = null";
                    }
                }
                #endregion

                if (destPortType == false)
                {
                    TimeSpan timeSpan = DateTime.Now - portINIData[destName].portStateErrorLogTime;

                    if (timeSpan.Seconds >= 10)
                    {
                        portINIData[destName].portStateErrorLogTime = DateTime.Now;

                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ") +
                            "OHB >> PLC|目的 " + destName + " 狀態錯誤 " + destState
                        );
                    }
                }

                return destPortType;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "AreDestEnable");
                return false;
            }
        }

        #endregion

        #region OHT >> OHB  OHT 呼叫 OHB
        public string statusToName(int status)
        {
            string s = "";
            switch (status)
            {
                case COMMAND_STATUS_BIT_INDEX_ENROUTE:
                    s = status + "_COMMAND_STATUS_BIT_INDEX_ENROUTE";
                    break;
                case COMMAND_STATUS_BIT_INDEX_LOAD_ARRIVE: //入料抵達
                    s = status + "_COMMAND_STATUS_BIT_INDEX_LOAD_ARRIVE";
                    break;
                case COMMAND_STATUS_BIT_INDEX_LOADING: //入料中
                    s = status + "_COMMAND_STATUS_BIT_INDEX_LOADING";
                    break;
                case COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE:
                    s = status + "_COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE";
                    break;
                case COMMAND_STATUS_BIT_INDEX_UNLOAD_ARRIVE:
                    s = status + "_COMMAND_STATUS_BIT_INDEX_UNLOAD_ARRIVE";
                    break;
                case COMMAND_STATUS_BIT_INDEX_UNLOADING:   //出料進行中
                    s = status + "_COMMAND_STATUS_BIT_INDEX_UNLOADING";
                    break;
                case COMMAND_STATUS_BIT_INDEX_UNLOAD_COMPLETE: //出料完成
                    s = status + "_COMMAND_STATUS_BIT_INDEX_UNLOAD_COMPLETE";
                    break;
                case COMMAND_STATUS_BIT_INDEX_DOUBLE_STORAGE: //二重格異常
                    s = status + "_COMMAND_STATUS_BIT_INDEX_DOUBLE_STORAGE";
                    break;
                case COMMAND_STATUS_BIT_INDEX_EMPTY_RETRIEVAL: //空取異常
                    s = status + "_COMMAND_STATUS_BIT_INDEX_EMPTY_RETRIEVAL";
                    break;
                case COMMAND_STATUS_BIT_INDEX_InterlockError:
                    s = status + "_COMMAND_STATUS_BIT_INDEX_InterlockError";
                    break;
                case COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH: //命令完成                                    
                    s = status + "_COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH";
                    break;
                case COMMAND_STATUS_BIT_INDEX_VEHICLE_ABORT: //命令完成                                    
                    s = status + "_COMMAND_STATUS_BIT_INDEX_VEHICLE_ABORT";
                    break;
                default:
                    s = status.ToString();
                    break;
            }
            return s;
        }
        public bool OHT_TransferStatus(string oht_cmdid, string ohtName, int status)   //OHT目前狀態
        {
            try
            {
                ohtName = ohtName.Trim();
                bool isCreatScuess = true;
                ACMD_OHTC ohtCmdData = cmdBLL.getCMD_OHTCByID(oht_cmdid);
                ACMD_MCS cmd = new ACMD_MCS();

                if (ohtCmdData == null)
                {
                    #region Log
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "OHT >> OHB|找 ACMD_OHTC 的 oht_cmdid: " + oht_cmdid + "  資料為 Null"
                    );
                    #endregion

                    //cmd = cmdBLL.getCMD_ByOHTName(ohtName);

                    return true;
                }
                else
                {
                    #region Log
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "OHT >> OHB|OHT_NAME: " + ohtName
                        + " OHT_CMD_ID: " + oht_cmdid.Trim()
                        + " MCS_CMD_ID:" + ohtCmdData.CMD_ID_MCS.Trim()
                        + " OHT_SOURCE:" + ohtCmdData.SOURCE.Trim()
                        + " OHT_DEST:" + ohtCmdData.DESTINATION.Trim()
                        + " OHT_Status:" + statusToName(status)
                    );
                    #endregion

                    cmd = cmdBLL.GetCmdIDFromCmd(ohtCmdData.CMD_ID_MCS.Trim());
                }

                #region OHT 手動測試，不會有 MCS_ID，回報指更新 Loc
                if (cmd == null)
                {
                    TransferServiceLogger.Info
                    (
                        "找不到 MCS_CMDID  " + GetOHTcmdLog(ohtCmdData)
                    );

                    OHT_TestProcess(ohtCmdData, status);

                    return true;
                }
                #endregion

                if (cmd.CRANE != ohtName)
                {
                    cmdBLL.updateCMD_MCS_CRANE(cmd.CMD_ID, ohtName);
                }
                if (string.IsNullOrWhiteSpace(cmd.CARRIER_ID_ON_CRANE))
                {
                    cmd.CARRIER_ID_ON_CRANE = "";
                }

                #region Log
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "OHT >> OHB|"
                    + "找到命令" + GetCmdLog(cmd)
                );
                #endregion

                isCreatScuess &= cmdBLL.updateCMD_MCS_CmdStatus(cmd.CMD_ID, status);

                if (cmd.CMD_ID.Contains("SCAN-"))
                {
                    OHT_ScanProcess(cmd, ohtName, status);
                }
                else
                {
                    OHT_TransferProcess(cmd, ohtCmdData, ohtName, status);
                }

                return isCreatScuess;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "OHT_TransferStatus");
                return false;
            }
        }
        public bool OHT_TransferProcess(ACMD_MCS cmd, ACMD_OHTC ohtCmd, string ohtName, int status)
        {
            try
            {
                ohtCmd.SOURCE = ohtCmd.SOURCE.Trim();
                ohtCmd.DESTINATION = ohtCmd.DESTINATION.Trim();

                switch (status)
                {
                    #region 正常流程
                    case COMMAND_STATUS_BIT_INDEX_ENROUTE: //在路上
                        cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.Transferring);

                        if (cmd.HOSTSOURCE != ohtCmd.SOURCE)
                        {
                            reportBLL.ReportCarrierResumed(cmd.CMD_ID);
                        }
                        else
                        {
                            reportBLL.ReportTransferInitiated(cmd.CMD_ID);
                        }

                        Thread.Sleep(100);

                        reportBLL.ReportCraneActive(cmd.CMD_ID, ohtName);
                        break;
                    case COMMAND_STATUS_BIT_INDEX_LOAD_ARRIVE: //入料抵達
                        break;
                    case COMMAND_STATUS_BIT_INDEX_LOADING: //入料中
                        break;
                    case COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE: //入料完成

                        if (cmd.COMMANDSTATE == status)  //模擬器會重複發，第二次就跳過
                        {
                            break;
                        }

                        cmd.HOSTSOURCE = ohtCmd.SOURCE;
                        CassetteData LoadCSTData = cassette_dataBLL.loadCassetteDataByLoc(cmd.HOSTSOURCE);

                        if (LoadCSTData != null)
                        {
                            OHT_LoadCompleted(ohtCmd, LoadCSTData, ohtName, "OHT_TransferProcess");
                        }
                        else
                        {
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ")
                                + "OHT >> OHB|OHT_LoadCompleted 位置： " + cmd.HOSTSOURCE + " 找不到卡匣資料"
                            );
                        }
                        break;
                    case COMMAND_STATUS_BIT_INDEX_UNLOAD_ARRIVE:   //出料抵達
                        if (isCVPort(ohtCmd.DESTINATION))
                        {
                            CassetteData ohtToPort = cassette_dataBLL.loadCassetteDataByLoc(ohtName.Trim());

                            if (ohtToPort != null)
                            {
                                //2020/2/18 Hsinyu Chang: 搬出到port，要通知port準備搬出哪一筆帳
                                PortValueDefMapAction portValueDefMapAction = scApp.getEQObjCacheManager().getPortByPortID(ohtCmd.DESTINATION).getMapActionByIdentityKey(typeof(PortValueDefMapAction).Name) as PortValueDefMapAction;
                                portValueDefMapAction.Port_WriteBoxCstID(ohtToPort);

                                TransferServiceLogger.Info
                                (
                                    DateTime.Now.ToString("HH:mm:ss.fff ")
                                    + "OHB >> PLC|Port_WriteBoxCstID 對 " + ohtCmd.DESTINATION + " 寫入 " + GetCstLog(ohtToPort)
                                );
                            }
                        }
                        break;
                    case COMMAND_STATUS_BIT_INDEX_UNLOADING:   //出料進行中
                        break;
                    case COMMAND_STATUS_BIT_INDEX_UNLOAD_COMPLETE: //出料完成

                        CassetteData unLoadCSTData = cassette_dataBLL.loadCassetteDataByLoc(ohtName.Trim());

                        if (string.IsNullOrWhiteSpace(cmd.RelayStation))
                        {
                            cmd.RelayStation = "";
                        }

                        ohtCmd.DESTINATION = ohtCmd.DESTINATION.Trim();

                        if (unLoadCSTData != null)
                        {
                            if (cmd.RelayStation == ohtCmd.DESTINATION)
                            {
                                OHT_UnLoadAlternate(cmd, unLoadCSTData);
                            }
                            else
                            {
                                OHT_UnLoadCompleted(ohtCmd, unLoadCSTData, "OHT_TransferProcess");
                            }
                        }
                        else
                        {
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ")
                                + "OHT >> OHB|OHT_UnLoadCompleted 位置： " + ohtName.Trim() + " 找不到卡匣資料"
                            );
                        }

                        break;
                    case COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH: //命令完成

                        reportBLL.ReportCraneIdle(ohtName, cmd.CMD_ID);

                        if (cmd.TRANSFERSTATE == E_TRAN_STATUS.Canceling)
                        {
                            cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);
                            reportBLL.ReportTransferCancelCompleted(cmd.CMD_ID);
                            break;
                        }
                        else if (cmd.TRANSFERSTATE == E_TRAN_STATUS.Aborting)
                        {
                            cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);
                            scApp.ReportBLL.ReportTransferAbortCompleted(cmd.CMD_ID);
                            break;
                        }

                        string resultCode = ResultCode.Successful;
                        IDreadStatus idReadStatus = IDreadStatus.successful;

                        CassetteData dbCstData = cassette_dataBLL.loadCassetteDataByLoc(ohtName.Trim());

                        if (dbCstData != null)  //之前發生 OHT 有給 64 後，128 的 BOX 給 ERROR1 ，加上預防機制，如果卡匣還在車上，在做處理
                        {
                            cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);

                            CassetteData ohtCstData = new CassetteData();   //OHT讀取資料

                            ohtCstData = dbCstData.Clone();
                            ohtCstData.BOXID = cmd.CARRIER_ID_ON_CRANE.Trim();

                            if (ohtCstData.BOXID.Trim() != dbCstData.BOXID.Trim())
                            {
                                if (string.IsNullOrWhiteSpace(ohtCstData.BOXID.Trim())
                                    || ohtCstData.BOXID.Trim().Contains("ERROR1")
                                    || ohtCstData.BOXID.Trim().Contains("NORD01")
                                   )
                                {
                                    ohtCstData.BOXID = CarrierReadFail(ohtCstData.Carrier_LOC);

                                    resultCode = ResultCode.BoxID_ReadFailed;
                                    idReadStatus = IDreadStatus.BoxReadFail_CstIsOK;
                                }
                                else
                                {
                                    //CassetteData cstID_Du = cassette_dataBLL.loadCassetteDataByCSTID(ohtCstData.CSTID);
                                    CassetteData boxID_Du = cassette_dataBLL.loadCassetteDataByBoxID(ohtCstData.BOXID);
                                    bool du = false;

                                    if (boxID_Du != null)
                                    {
                                        if (boxID_Du.Carrier_LOC != dbCstData.Carrier_LOC
                                          )
                                        {
                                            resultCode = ResultCode.DuplicateID;
                                            idReadStatus = IDreadStatus.duplicate;
                                            du = true;
                                        }
                                    }

                                    if (du == false)
                                    {
                                        resultCode = ResultCode.BoxID_Mismatch;
                                        idReadStatus = IDreadStatus.mismatch;
                                    }
                                }

                                reportBLL.ReportCarrierIDRead(ohtCstData, ((int)idReadStatus).ToString());
                            }

                            if (resultCode == ResultCode.Successful)
                            {
                                //OHT_UnLoadCompleted(cmd, dbCstData, ohtCstData);
                            }
                            else
                            {
                                #region Log
                                TransferServiceLogger.Info
                                (
                                    DateTime.Now.ToString("HH:mm:ss.fff ")
                                    + "OHT >> OHB|OHT BOX 讀取異常:" + resultCode + "\n"
                                    + GetCmdLog(cmd) + "\n"
                                    + GetCstLog(dbCstData)
                                );
                                #endregion
                                reportBLL.ReportTransferCompleted(cmd, dbCstData, resultCode);
                                HaveAccountHaveReal(dbCstData, ohtCstData, idReadStatus);
                            }
                        }
                        else
                        {
                            #region Log
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ")
                                + "OHT >> OHB|卡匣不在車上:"
                                + GetCmdLog(cmd)
                            );
                            #endregion
                        }

                        EmptyShelf();   //每次命令結束，檢查儲位狀態
                        break;
                    #endregion
                    #region 異常流程
                    case COMMAND_STATUS_BIT_INDEX_DOUBLE_STORAGE: //二重格異常
                        cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);

                        OHBC_AlarmSet(ohtName, SCAppConstants.SystemAlarmCode.OHT_Issue.DoubleStorage);
                        OHBC_AlarmCleared(ohtName, SCAppConstants.SystemAlarmCode.OHT_Issue.DoubleStorage);

                        string cstID = CarrierDouble(cmd.HOSTDESTINATION.Trim());
                        string boxID = CarrierDouble(cmd.HOSTDESTINATION.Trim());
                        string loc = cmd.HOSTDESTINATION;

                        OHBC_InsertCassette(cstID, boxID, loc);

                        reportBLL.ReportTransferCompleted(cmd, null, ResultCode.InterlockError);
                        break;
                    case COMMAND_STATUS_BIT_INDEX_EMPTY_RETRIEVAL: //空取異常
                        cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);

                        OHBC_AlarmSet(ohtName, SCAppConstants.SystemAlarmCode.OHT_Issue.EmptyRetrieval);
                        OHBC_AlarmCleared(ohtName, SCAppConstants.SystemAlarmCode.OHT_Issue.EmptyRetrieval);

                        CassetteData emptyData = cassette_dataBLL.loadCassetteDataByLoc(cmd.HOSTSOURCE.Trim());

                        reportBLL.ReportCarrierRemovedCompleted(emptyData.CSTID, emptyData.BOXID);

                        reportBLL.ReportTransferCompleted(cmd, null, ResultCode.InterlockError);
                        break;
                    case COMMAND_STATUS_BIT_INDEX_InterlockError:
                        cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);

                        reportBLL.ReportCraneIdle(ohtName, cmd.CMD_ID);
                        reportBLL.ReportTransferCompleted(cmd, null, ResultCode.InterlockError);
                        break;
                    case COMMAND_STATUS_BIT_INDEX_VEHICLE_ABORT:
                        cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);

                        reportBLL.ReportCraneIdle(ohtName, cmd.CMD_ID);
                        reportBLL.ReportTransferCompleted(cmd, null, ResultCode.InterlockError);   //  20/04/13 MCS 反應說不要報 1 ，改報64
                        EmptyShelf();
                        break;
                    #endregion
                    default:
                        break;
                }
                //
                return true;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "OHT_TransferProcess");
                return false;
            }
        }
        CassetteData ScanCstData = null;
        public void OHT_ScanProcess(ACMD_MCS cmd, string ohtName, int status)
        {
            try
            {
                CassetteData dbCstData = cassette_dataBLL.loadCassetteDataByLoc(cmd.HOSTSOURCE.Trim());
                #region Log
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "OHT >> OHB|OHT_ScanProcess dbCstData:\n"
                    + " CST_Data：CST_ID:" + (dbCstData == null ? "" : dbCstData.CSTID)
                    + " BOX_ID:" + (dbCstData == null ? "" : dbCstData.BOXID)
                    + " Loc:" + (dbCstData == null ? "" : dbCstData.Carrier_LOC)
                );
                #endregion
                switch (status)
                {
                    case COMMAND_STATUS_BIT_INDEX_ENROUTE:
                        reportBLL.ReportCraneActive(cmd.CMD_ID, ohtName);
                        cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.Transferring);
                        break;
                    case COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE: //入料完成
                        ScanCstData = new CassetteData();
                        break;
                    case COMMAND_STATUS_BIT_INDEX_UNLOAD_COMPLETE: //出料完成

                        break;
                    case COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH: //命令完成
                        cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);

                        IDreadStatus idReadStatus;
                        if (string.IsNullOrWhiteSpace(cmd.CARRIER_ID_ON_CRANE))  //SCAN 流程，車子給空值表示無料
                        {
                            ScanCstData = null;
                        }
                        else
                        {
                            ScanCstData = new CassetteData();
                            if (dbCstData != null)
                            {
                                ScanCstData = dbCstData.Clone();
                                idReadStatus = IDreadStatus.successful;
                            }
                            else
                            {
                                ScanCstData.Carrier_LOC = cmd.HOSTDESTINATION;
                                idReadStatus = IDreadStatus.CSTReadFail_BoxIsOK;
                            }

                            ScanCstData.BOXID = cmd.CARRIER_ID_ON_CRANE;
                        }

                        string resultCode = ResultCode.Successful;
                        idReadStatus = IDreadStatus.CSTReadFail_BoxIsOK;

                        if (ScanCstData != null && dbCstData != null)
                        {
                            if (ScanCstData.BOXID.Trim() != dbCstData.BOXID.Trim())
                            {
                                if (string.IsNullOrWhiteSpace(ScanCstData.BOXID.Trim())
                                    || ScanCstData.BOXID.Trim().Contains("ERROR1")
                                    || ScanCstData.BOXID.Trim().Contains("NORD01")
                                   )
                                {
                                    if (dbCstData.BOXID.Contains("UNK"))
                                    {
                                        #region Log
                                        TransferServiceLogger.Info
                                        (
                                            DateTime.Now.ToString("HH:mm:ss.fff ")
                                            + "OHT >> OHB|OHT_ScanProcess 原帳 與 OHT 讀到結果為UNK，不做處理:" + "\n"
                                            + GetCmdLog(cmd) + "\n"
                                            + GetCstLog(dbCstData)
                                        );
                                        #endregion
                                        return;    //士偉提出，原BOX如果是 UNK 就不處理
                                    }
                                    else
                                    {
                                        ScanCstData.BOXID = CarrierReadFail(ScanCstData.Carrier_LOC);
                                    }
                                    resultCode = ResultCode.BoxID_ReadFailed;
                                    idReadStatus = IDreadStatus.failed;
                                }
                                else
                                {
                                    resultCode = ResultCode.BoxID_Mismatch;

                                    if (cassette_dataBLL.loadCassetteDataByBoxID(ScanCstData.BOXID) != null
                                        || cassette_dataBLL.loadCassetteDataByBoxID(ScanCstData.CSTID) != null
                                       )
                                    {
                                        idReadStatus = IDreadStatus.duplicate;
                                    }
                                    else
                                    {
                                        idReadStatus = IDreadStatus.mismatch;
                                    }
                                }
                            }

                            reportBLL.ReportCarrierIDRead(ScanCstData, ((int)idReadStatus).ToString());

                            reportBLL.ReportCraneIdle(ohtName, cmd.CMD_ID);

                            if (resultCode != ResultCode.Successful)
                            {
                                HaveAccountHaveReal(dbCstData, ScanCstData, idReadStatus);
                            }
                        }
                        else if (ScanCstData != null && dbCstData == null)   //無帳有料
                        {
                            ScanCstData.CSTID = CarrierReadFail(ScanCstData.Carrier_LOC);

                            if (ScanCstData.BOXID.Trim().Contains("ERROR1"))
                            {
                                ScanCstData.BOXID = CarrierReadFail(ScanCstData.Carrier_LOC);
                                resultCode = ResultCode.BoxID_ReadFailed;
                                idReadStatus = IDreadStatus.failed;
                            }

                            reportBLL.ReportCarrierIDRead(ScanCstData, ((int)idReadStatus).ToString());

                            NotAccountHaveRead(ScanCstData);
                        }
                        else if (ScanCstData == null && dbCstData != null)   //有帳無料
                        {
                            HaveAccountNotReal(dbCstData);
                        }
                        else if (ScanCstData == null)                        //無帳無料
                        {
                            reportBLL.ReportCraneIdle(ohtName, cmd.CMD_ID);
                        }

                        ScanCstData = null;
                        break;
                    case COMMAND_STATUS_BIT_INDEX_DOUBLE_STORAGE: //二重格異常

                        break;
                    case COMMAND_STATUS_BIT_INDEX_EMPTY_RETRIEVAL: //空取異常

                        if (dbCstData != null)
                        {
                            HaveAccountNotReal(dbCstData);
                        }

                        ScanCstData = null;
                        break;
                    case COMMAND_STATUS_BIT_INDEX_VEHICLE_ABORT:
                        cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);

                        reportBLL.ReportCraneIdle(ohtName, cmd.CMD_ID);

                        break;
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "OHT_ScanProcess");
            }
        }
        public void OHT_TestProcess(ACMD_OHTC ohtCmdData, int status)  //OHT 單動測試，不會有 MCS_ID，回報指更新 Loc
        {
            try
            {
                string ohtName = ohtCmdData.VH_ID.Trim();
                CassetteData dbCstData = null;    //資料庫資料

                switch (status)
                {
                    case COMMAND_STATUS_BIT_INDEX_ENROUTE:
                        break;
                    case COMMAND_STATUS_BIT_INDEX_LOAD_ARRIVE: //入料抵達
                        break;
                    case COMMAND_STATUS_BIT_INDEX_LOADING: //入料中
                        break;
                    case COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE: //入料完成
                        dbCstData = cassette_dataBLL.loadCassetteDataByLoc(ohtCmdData.SOURCE.Trim());

                        if (dbCstData != null)
                        {
                            cassette_dataBLL.UpdateCSTLoc(dbCstData.BOXID, ohtName, 1);
                        }
                        else
                        {
                            string cstID = CarrierReadFail(ohtName);
                            string boxID = ohtCmdData.BOX_ID.Trim();

                            if (ohtCmdData.BOX_ID.Trim().Contains("ERROR1") || string.IsNullOrWhiteSpace(ohtCmdData.BOX_ID.Trim()))
                            {
                                boxID = CarrierReadFail(ohtCmdData.DESTINATION.Trim());
                            }

                            OHBC_InsertCassette(cstID, boxID, ohtName);
                        }
                        break;
                    case COMMAND_STATUS_BIT_INDEX_UNLOAD_ARRIVE:
                        break;
                    case COMMAND_STATUS_BIT_INDEX_UNLOADING:   //出料進行中
                        break;
                    case COMMAND_STATUS_BIT_INDEX_UNLOAD_COMPLETE: //出料完成
                        CassetteData dbDestData = cassette_dataBLL.loadCassetteDataByLoc(ohtCmdData.DESTINATION.Trim());

                        if (dbDestData != null)
                        {
                            cassette_dataBLL.DeleteCSTbyCstBoxID(dbDestData.CSTID, dbDestData.BOXID);
                        }

                        dbCstData = cassette_dataBLL.loadCassetteDataByLoc(ohtCmdData.VH_ID.Trim());

                        if (dbCstData != null)
                        {
                            cassette_dataBLL.UpdateCSTLoc(dbCstData.BOXID, ohtCmdData.DESTINATION.Trim(), 0);
                        }
                        else
                        {
                            string cstID = CarrierReadFail(ohtName);
                            string boxID = ohtCmdData.BOX_ID.Trim();

                            if (ohtCmdData.BOX_ID.Trim().Contains("ERROR1") || string.IsNullOrWhiteSpace(ohtCmdData.BOX_ID.Trim()))
                            {
                                boxID = CarrierReadFail(ohtCmdData.DESTINATION.Trim());
                            }

                            OHBC_InsertCassette(cstID, boxID, ohtCmdData.DESTINATION.Trim());
                        }
                        break;
                    case COMMAND_STATUS_BIT_INDEX_DOUBLE_STORAGE:
                        break;
                    case COMMAND_STATUS_BIT_INDEX_EMPTY_RETRIEVAL:
                        break;
                    case COMMAND_STATUS_BIT_INDEX_InterlockError:
                        break;
                    case COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH: //命令完成

                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "OHT_TestProcess");
            }
        }
        public void OHT_LoadCompleted(ACMD_OHTC ohtCmd, CassetteData loadCstData, string ohtName, string sourceCmd)
        {
            try
            {
                if (portINIData[ohtName].craneLoading)
                {
                    return;
                }
                //
                portINIData[ohtName].craneLoading = true;

                if (ohtCmd != null && loadCstData != null)
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ")
                        + "OHT_LoadCompleted 誰呼叫:" + sourceCmd + " " + ohtName + " Loading:" + portINIData[ohtName].craneLoading
                    );

                    CassetteData dbCstData = cassette_dataBLL.loadCassetteDataByLoc(ohtName);

                    if (dbCstData != null)
                    {
                        if (dbCstData.BOXID.Trim() == ohtCmd.BOX_ID.Trim())
                        {
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ")
                                + "OHT_LoadCompleted 發現卡匣已在車上 " + GetCstLog(dbCstData)
                            );
                            return;
                        }
                    }

                    cassette_dataBLL.UpdateCSTLoc(loadCstData.BOXID, ohtName, 1);
                    cassette_dataBLL.UpdateCSTState(loadCstData.BOXID, (int)E_CSTState.Transferring);

                    ACMD_MCS cmd = cmdBLL.GetCmdIDFromCmd(ohtCmd.CMD_ID_MCS.Trim());

                    if (cmd != null)
                    {
                        if (isUnitType(loadCstData.Carrier_LOC, UnitType.CRANE) == false)
                        {
                            loadCstData.Carrier_LOC = ohtName;
                            reportBLL.ReportCarrierTransferring(cmd, loadCstData, ohtName);
                        }

                        if (shelfDefBLL.isExist(cmd.HOSTSOURCE))
                        {
                            reportBLL.ReportZoneCapacityChange(cmd.HOSTSOURCE, null);
                        }
                    }
                    else
                    {
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ")
                            + "OHT_LoadCompleted MCS_CMD = Null 或 loadCstData = Null"
                        );
                    }
                }
                else
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ")
                        + "OHT_LoadCompleted ohtCmd = Null OHT_CMDID:" + ohtCmd.CMD_ID
                    );
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "OHT_LoadCompleted");
            }
            finally
            {
                portINIData[ohtName].craneLoading = false;

                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "OHT_LoadCompleted 誰呼叫:" + sourceCmd + " " + ohtName + " Loading:" + portINIData[ohtName].craneLoading
                );
            }
        }
        public void OHT_UnLoadCompleted(ACMD_OHTC ohtCmd, CassetteData unLoadCstData, string sourceCmd)
        {
            string ohtName = ohtCmd.VH_ID.Trim();

            try
            {
                if (portINIData[ohtName].craneUnLoading)
                {
                    return;
                }

                portINIData[ohtName].craneUnLoading = true;

                if (ohtCmd != null && unLoadCstData != null)
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ")
                        + "OHT_UnLoadCompleted 誰呼叫:" + sourceCmd + " " + ohtName + " UnLoading:" + portINIData[ohtName].craneUnLoading
                    );

                    unLoadCstData.Carrier_LOC = ohtCmd.DESTINATION;
                    string dest = ohtCmd.DESTINATION;

                    if (isCVPort(dest))
                    {
                        unLoadCstData.Carrier_LOC = GetPositionName(unLoadCstData.Carrier_LOC, 1);
                        cassette_dataBLL.UpdateCSTLoc(unLoadCstData.BOXID, unLoadCstData.Carrier_LOC, 1);

                    }
                    else if (isUnitType(dest, UnitType.SHELF))
                    {
                        cassette_dataBLL.UpdateCSTLoc(unLoadCstData.BOXID, dest, 1);
                        cassette_dataBLL.UpdateCSTState(unLoadCstData.BOXID, (int)E_CSTState.Completed);
                    }

                    ACMD_MCS cmd = cmdBLL.GetCmdIDFromCmd(ohtCmd.CMD_ID_MCS.Trim());

                    if (cmd != null)
                    {
                        reportBLL.ReportTransferCompleted(cmd, unLoadCstData, ResultCode.Successful);

                        if (isCVPort(dest))
                        {
                            if (isUnitType(dest, UnitType.AGV))
                            {
                                PortPLCInfo plcPortData = GetPLC_PortData(dest);
                                if (plcPortData != null)
                                {
                                    if (plcPortData.IsInputMode)
                                    {
                                        reportBLL.ReportCarrierRemovedCompleted(unLoadCstData.CSTID, unLoadCstData.BOXID);
                                    }

                                    if (plcPortData.IsOutputMode)
                                    {
                                        if (plcPortData.IsMGVMode)
                                        {
                                            reportBLL.ReportCarrierWaitOut(unLoadCstData, "1");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                reportBLL.ReportCarrierWaitOut(unLoadCstData, "1");
                            }
                        }
                        else if (isUnitType(dest, UnitType.SHELF))
                        {
                            reportBLL.ReportCarrierStored(unLoadCstData);
                            reportBLL.ReportZoneCapacityChange(dest, null);
                        }
                        else
                        {
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ")
                                + "OHT_UnLoadCompleted 卡匣位置在: " + GetCstLog(unLoadCstData)
                            );
                        }

                        cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);
                    }
                    else
                    {
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ")
                            + "OHT_UnLoadCompleted MCS_CMD = Null OHT_CMDID:" + ohtCmd.CMD_ID
                        );
                    }
                }
                else
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ")
                        + "OHT_UnLoadCompleted ohtCmd = Null 或 unLoadCstData = Null"
                    );
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "OHT_UnLoadCompleted");
            }
            finally
            {
                portINIData[ohtName].craneUnLoading = false;

                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "OHT_UnLoadCompleted 誰呼叫:" + sourceCmd + " " + ohtName + " UnLoading:" + portINIData[ohtName].craneUnLoading
                );
            }
        }
        public void OHT_UnLoadAlternate(ACMD_MCS cmd, CassetteData dbCstData)
        {
            if (isUnitType(cmd.RelayStation, UnitType.SHELF))
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "OHT_UnLoadAlternate"
                );

                dbCstData.Carrier_LOC = cmd.RelayStation;

                cassette_dataBLL.UpdateCSTLoc(dbCstData.BOXID, cmd.RelayStation, 1);
                cassette_dataBLL.UpdateCSTState(dbCstData.BOXID, (int)E_CSTState.Alternate);
                //cassette_dataBLL.UpdateCST_DateTime(dbCstData.BOXID, UpdateCassetteTimeType.StoreDT);

                shelfDefBLL.updateStatus(cmd.RelayStation, ShelfDef.E_ShelfState.Stored);

                reportBLL.ReportCarrierStoredAlt(cmd, dbCstData);
                reportBLL.ReportZoneCapacityChange(dbCstData.Carrier_LOC, null);

                cmdBLL.updateCMD_MCS_CRANE(cmd.CMD_ID, "");
                cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.Queue);
            }
        }

        #endregion

        #region PLC >> OHB

        public void PLC_ReportPortWaitIn(PortPLCInfo plcInfo, string sourceCmd)
        {
            try
            {
                if (cassette_dataBLL == null)
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "PLC >> OHB|PLC_ReportPortWaitIn cassette_dataBLL = null"
                    );
                    return;
                }

                CassetteData cstData = new CassetteData();
                //datainfo.CSTID = function.CassetteID.Trim();        //填CSTID
                cstData.CSTID = SCUtility.Trim(plcInfo.CassetteID, true);        //填CSTID
                cstData.BOXID = plcInfo.BoxID.Trim();        //填BOXID
                cstData.Carrier_LOC = plcInfo.EQ_ID.Trim();  //填PortID
                cstData.CSTState = E_CSTState.Installed;
                cstData.StockerID = "1";
                cstData.CSTInDT = DateTime.Now.ToString("yy/MM/dd HH:mm:ss");
                cstData.ReadStatus = ((int)ACMD_MCS.IDreadStatus.successful).ToString();
                cstData.Stage = 1;

                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "PLC >> OHB|PLC_ReportPortWaitIn 誰呼叫:" + sourceCmd + "  " + GetCstLog(cstData)
                );

                CassetteData dbCSTData = cassette_dataBLL.loadCassetteDataByLoc(cstData.Carrier_LOC);

                if (dbCSTData != null)
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "PLC >> OHB|此位置已有帳 " + GetCstLog(dbCSTData)
                    );

                    if (dbCSTData.BOXID == dbCSTData.BOXID)
                    {
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ")
                            + "PLC >> OHB|BOXID 相同跳出："
                            + "\ndatainfo " + GetCstLog(cstData)
                            + "\ndbCSTData" + GetCstLog(dbCSTData)
                        );

                        return;
                    }

                    ACMD_MCS dbMcsdata = cmdBLL.getCMD_ByBoxID(dbCSTData.BOXID);
                    if (dbMcsdata != null)
                    {
                        if (dbMcsdata.TRANSFERSTATE != E_TRAN_STATUS.Queue)
                        {
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ")
                                + "PLC >> OHB|此筆卡匣已有命令在搬："
                                + GetCmdLog(dbMcsdata)
                            );

                            return;
                        }
                        else
                        {
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ") +
                                "PLC >> OHB|發現已有命令在 Queue 狀態："
                                + "    CmdID:" + dbMcsdata.CMD_ID
                                + "    來源:" + dbMcsdata.HOSTSOURCE
                                + "    目的:" + dbMcsdata.HOSTDESTINATION
                                + "    準備重新建帳"
                            );

                            cmdBLL.updateCMD_MCS_TranStatus(dbMcsdata.CMD_ID, E_TRAN_STATUS.TransferCompleted);

                            reportBLL.ReportTransferCompleted(dbMcsdata, dbCSTData, ResultCode.OtherErrors);

                            reportBLL.ReportCarrierRemovedCompleted(dbCSTData.CSTID, dbCSTData.BOXID);
                        }
                    }
                    else
                    {
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ") + "PLC >> OHB|刪掉舊帳"
                        );

                        reportBLL.ReportCarrierRemovedCompleted(dbCSTData.CSTID, dbCSTData.BOXID);
                    }
                }

                if (isUnitType(cstData.Carrier_LOC, UnitType.NTB) || isUnitType(cstData.Carrier_LOC, UnitType.AGV)) //只有 AGV 跟 NTB 會讀 CSID
                {
                    if (string.IsNullOrWhiteSpace(cstData.CSTID))
                    {
                        cstData.CSTID = "ERROR1";
                    }
                }
                else
                {
                    cstData.CSTID = "ERROR1";
                }

                cstData = IDRead(cstData);

                reportBLL.ReportCarrierIDRead(cstData, cstData.ReadStatus);

                if (cstData.ReadStatus == ((int)IDreadStatus.duplicate).ToString())
                {
                    Duplicate(cstData);
                }

                cassette_dataBLL.insertCassetteData(cstData);
                reportBLL.ReportCarrierWaitIn(cstData);
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "PLC_ReportPortWaitIn");
            }
        }
        public void PortPositionWaitOut(CassetteData datainfo, int outStage, string sourceCmd = "PLC")
        {
            try
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "PLC >> OHB|PortPositionWaitOut 誰呼叫:" + sourceCmd
                    + GetCstLog(datainfo)
                );

                datainfo.Carrier_LOC = datainfo.Carrier_LOC.Trim();

                UPStage(datainfo, outStage);
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "PortPositionWaitOut");
            }
        }
        public void PortPositionOFF(PortPLCInfo plcInfo, int position)
        {
            int stage = portINIData[plcInfo.EQ_ID.Trim()].Stage;

            if (stage == position)
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "PLC >> PLC|"
                    + "plcInfo: " + plcInfo.EQ_ID + " PositionOFF:" + position
                );

                CassetteData datainfo = new CassetteData();
                datainfo.CSTID = plcInfo.CassetteID.Trim();        //填CSTID
                datainfo.BOXID = plcInfo.BoxID.Trim();        //填BOXID
                datainfo.Carrier_LOC = plcInfo.EQ_ID.Trim();  //填Port 名稱

                PortCarrierRemoved(datainfo, plcInfo.IsAGVMode, "PortPositionOFF");
            }
        }
        public void PortWaitOut(CassetteData cstData)
        {
            try
            {
                cstData.Carrier_LOC = cstData.Carrier_LOC.Trim();

                int outStage = (int)portINIData[cstData.Carrier_LOC].Stage;

                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "PLC >> OHB|PortPositionWaitOut"
                    + "    LOC:" + cstData.Carrier_LOC
                    + "    STAGE:" + outStage.ToString()
                    + "    BOXID:" + cstData?.BOXID ?? ""
                    + "    CSTID:" + cstData?.CSTID ?? ""
                );

                UPStage(cstData, outStage);
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "AGVPortWaitOut");
            }
        }
        public bool UPStage(CassetteData outData, int outStage)
        {
            try
            {
                if (iniStatus != true)
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") + "UPStage 未完成初始化動作"
                    );
                    return false;
                }

                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "PLC >> OHB|UPStage PortID:" + outData.Carrier_LOC
                    + " outStage:" + outStage
                    + " outData :" + GetCstLog(outData)
                );

                List<CassetteData> dbDataList = cassette_dataBLL.LoadCassetteDataByCVPort(outData.Carrier_LOC);

                CassetteData dbData = dbDataList.Where(data => data.BOXID.Trim() == outData.BOXID.Trim()).FirstOrDefault();

                if (dbData == null)
                {
                    //如果沒找到PLC給的BOX卡匣，就依序搬入順序來找卡匣
                    dbData = dbDataList
                        .Where(cst => cst.Stage < outStage)
                        .OrderByDescending(cst => cst.Stage).FirstOrDefault();

                    if (dbData != null)
                    {
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ") +
                            "PLC >> OHB|PortPositionWaitOut 找不到 BOXID: " + outData.BOXID + " 先進先出在第 " + dbData.Stage + " 節找到 BOXID: " + dbData.BOXID
                        );
                    }
                    else
                    {
                        OHTtoPort(outData.Carrier_LOC, outStage, "UPStage");
                        return true;
                    }
                }
                else
                {
                    string portZoneName = portINIData[dbData.Carrier_LOC].ZoneName;
                    if (portZoneName != outData.Carrier_LOC)
                    {
                        ACMD_MCS cmdData = cmdBLL.getCMD_ByBoxID(outData.BOXID.Trim());

                        if (cmdData != null)
                        {
                            if (cmdData.HOSTDESTINATION == outData.Carrier_LOC)
                            {

                            }
                        }
                        else
                        {
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ") +
                                "PLC >> OHB|PortPositionWaitOut 卡匣不在命令執行中"
                                + " BOXID: " + dbData.BOXID
                                + " Carrier_LOC: " + dbData.Carrier_LOC
                            );
                        }
                    }
                }

                if (dbData == null)
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "PLC >> OHB|PortPositionWaitOut 找不到卡匣"
                    );

                    return false;
                }

                PortINIData portInI = portINIData[outData.Carrier_LOC.Trim()];
                int portStage = portInI.Stage;

                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "PLC >> OHB|UPStage：PortID:" + portInI.PortName + "    portStage:" + portStage
                    + " dbData CSTID:" + dbData.BOXID + "  BOXID:" + dbData.BOXID + "  LOC:" + dbData.Carrier_LOC + "  卡匣目前節數:" + dbData.Stage
                );

                if (outStage > portStage)
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "PLC >> OHB|UPStage：超出 Port 設定節數:" + portInI.PortName
                        + " portStage:" + portStage
                        + " outStage:" + outStage
                    );
                    return false;
                }

                int iStage = (int)dbData.Stage;

                #region 不補報 WaitOut，註：補報與不補報只能擇一，200519 MCS 說不用補報也沒關系

                if (iStage != outStage && outStage > iStage)
                {
                    dbData.Carrier_LOC = GetPositionName(outData.Carrier_LOC, outStage);

                    if (isUnitType(outData.Carrier_LOC, UnitType.AGV) == false)
                    {
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ") +
                            "PLC >> OHB|UPStage WaitOutLoc:" + dbData.Carrier_LOC
                        );

                        reportBLL.ReportCarrierWaitOut(dbData, "1");

                        cassette_dataBLL.UpdateCSTLoc(dbData.BOXID, dbData.Carrier_LOC, outStage);

                        //Task.Run(() =>
                        //{
                        //    cassette_dataBLL.UpdateCSTLoc(dbData.BOXID, dbData.Carrier_LOC, outStage);
                        //});
                    }
                }

                #endregion
                #region 補報 WaitOut ，註：補報與不補報只能擇一

                //for (; iStage <= iWaitOut; iStage++)
                //{
                //    if (iStage != dbData.Stage && iStage != 0)
                //    {
                //        if (iStage == portStage)
                //        {
                //            iStage = (int)CassetteData.OHCV_STAGE.LP;
                //        }

                //        if (((CassetteData.OHCV_STAGE)iStage) != CassetteData.OHCV_STAGE.LP)
                //        {
                //            dbData.Carrier_LOC = outData.Carrier_LOC + ((CassetteData.OHCV_STAGE)iStage).ToString();
                //        }
                //        else
                //        {
                //            dbData.Carrier_LOC = outData.Carrier_LOC;
                //            iStage = portStage;
                //        }

                //        //oldStageData : 檢查下一個位置是否有卡匣
                //        CassetteData oldStageData = dbDataList.Where(data => data.Carrier_LOC.Contains(outData.Carrier_LOC) && data.Stage == iStage).FirstOrDefault();

                //        if (oldStageData != null)
                //        {
                //            TransferServiceLogger.Info
                //            (
                //                DateTime.Now.ToString("HH:mm:ss.fff ") +
                //                "PLC >> OHB|UPStage 下一個位置有卡匣 " + GetCstLog(oldStageData)   //自動往前移
                //            );

                //            return false;

                //            if (portStage == iStage)
                //            {
                //                TransferServiceLogger.Info
                //                (
                //                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                //                    "PLC >> OHB|UPStage 發現上次未刪除卡匣，自動刪除"
                //                );
                //                reportBLL.ReportCarrierRemovedFromPort(oldStageData, "2");
                //            }
                //            else
                //            {
                //                UPStage(oldStageData, iStage + 1);
                //            }
                //        }

                //        cassette_dataBLL.UpdateCSTLoc(dbData.BOXID, dbData.Carrier_LOC, iStage);

                //        if (iStage == portStage) //LP側的Loc不加"LP"字樣，報PortID就好
                //        {
                //            dbData.Carrier_LOC = outData.Carrier_LOC;

                //            cassette_dataBLL.UpdateCST_DateTime(dbData.BOXID, UpdateCassetteTimeType.WaitOutLPDT);

                //            //ACMD_MCS cmddata = cmdBLL.GetBoxFromCmd(dbData.BOXID);

                //            //if (cmddata == null)    //PLC沒給 BOXID 則用Cmd的目的找命令
                //            //{
                //            //    cmddata = cmdBLL.GetCmdDataByDest(dbData.Carrier_LOC);
                //            //}

                //            //if (cmddata != null)     //找不到命令，就不要報，會跳例外
                //            //{
                //            //    //cmdBLL.updateCMD_MCS_TranStatus(cmddata.CMD_ID, E_TRAN_STATUS.TransferCompleted);
                //            //    //reportBLL.ReportTransferCompleted(cmddata.CMD_ID.Trim(), "0");
                //            //}
                //        }

                //        reportBLL.ReportCarrierWaitOut(dbData, "1");
                //    }
                //}

                #endregion
                return true;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "UPStage");
                return false;
            }
        }

        public string GetPositionName(string portName, int stage)
        {
            portName = portName.Trim();
            string positionName = portName;

            if (stage < portINIData[portName].Stage)
            {
                positionName = positionName + ((CassetteData.OHCV_STAGE)stage).ToString();
            }

            return positionName;
        }
        public void OHTtoPort(string portName, int outStage, string cmdSource)   //自動過帳，將車子的帳轉移到目的 Port 上
        {
            try
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "OHB >> OHB|嘗試找停在 " + portName + " 前 OHT 上的 CST，誰呼叫:" + cmdSource
                );

                portName = portName.Trim();
                string ohtName = scApp.VehicleService.GetVehicleIDByPortID(portName);    //取得停在 Port 前的 OHT

                string log = "OHB >> OHB|";

                CassetteData dbData = null;
                CassetteData old_dbData = null; //舊帳，補報 MCS 用

                ACMD_MCS cmd = null;

                if (ohtName != "Error")
                {
                    log = log + ohtName + " 停在 " + portName;
                }
                else
                {
                    log = log + " 沒有車子停在 " + portName + " 前面";
                }

                #region 找命令

                #region 先用車子找命令
                List<ACMD_MCS> ohtCmdList = cmdBLL.getCMD_ByOHTName(ohtName);
                log = log + " OHTName: " + ohtName + " 執行: " + ohtCmdList.Count() + " 筆命令";
                cmd = ohtCmdList.Where(data => data.HOSTDESTINATION == portName && data.TRANSFERSTATE == E_TRAN_STATUS.Transferring).FirstOrDefault();

                if (cmd != null)
                {
                    log = log + " getCMD_ByOHTName 找到 " + ohtName + " 執行的 MCS 命令 :" + GetCmdLog(cmd);
                }
                #endregion
                #region 車子找不到命令，找命令目的，第二道防護，發生車子回報位置導致找不到停在 Port 前面
                else
                {
                    List<ACMD_MCS> cmdByDest = cmdBLL.GetCmdDataByDest(portName)
                                                        .Where(data => data.TRANSFERSTATE == E_TRAN_STATUS.Transferring)
                                                        .ToList();

                    log = log + "\n嘗試找搬送命令目的到 " + portName + " 找到 " + cmdByDest.Count().ToString() + " 筆";

                    #region 命令目的只有一個
                    if (cmdByDest.Count() == 1) //只有一筆命令，就找那一筆
                    {
                        cmd = cmdByDest.FirstOrDefault();

                        log = log + "\n找到命令 " + GetCmdLog(cmd);
                    }
                    #endregion
                    #region 命令目的多個，找 命令執行狀態 (COMMANDSTATE) 為 COMMAND_STATUS_BIT_INDEX_UNLOAD_ARRIVE
                    else
                    {
                        List<ACMD_MCS> cmdByDestUNLOAD_ARRIVE = cmdByDest.Where(data => data.COMMANDSTATE >= COMMAND_STATUS_BIT_INDEX_UNLOAD_ARRIVE).ToList();

                        log = log + "\n命令存在不只 1 筆，找狀態為 出料抵達 有" + cmdByDestUNLOAD_ARRIVE.Count().ToString() + " 筆";

                        if (cmdByDestUNLOAD_ARRIVE.Count() == 1) //只有一筆命令，就找那一筆
                        {
                            cmd = cmdByDestUNLOAD_ARRIVE.FirstOrDefault();

                            log = log + "\n找到命令 " + GetCmdLog(cmd);
                        }
                        else
                        {
                            log = log + "\n 多筆命令目的為 " + portName;
                            foreach (var v in cmdByDest)
                            {
                                log = log + "\n" + GetCmdLog(cmd);
                            }
                        }
                    }
                    #endregion
                }

                #endregion

                if (cmd != null)
                {
                    if (string.IsNullOrWhiteSpace(cmd.CRANE) == false)
                    {
                        ohtName = cmd.CRANE;
                    }
                }

                #endregion

                #region 找卡匣

                dbData = cassette_dataBLL.loadCassetteDataByLoc(ohtName);

                if (dbData != null)
                {
                    log = log + " 在 " + ohtName + " 車上找到 :" + GetCstLog(dbData);
                }
                else
                {
                    if (cmd != null)
                    {
                        log = log + "\n沒有卡匣在車上，換找 命令的 BOXID";
                        dbData = cassette_dataBLL.loadCassetteDataByBoxID(cmd.BOX_ID);

                        if (dbData != null)
                        {
                            log = log + " CMD BOXID: " + cmd.BOX_ID + " 找到 :" + GetCstLog(dbData);
                        }
                    }
                }

                if (dbData != null)
                {
                    old_dbData = dbData.Clone();
                }
                else
                {
                    log = log + " 找不到帳";
                }
                #endregion

                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") + log
                );

                #region 補報流程處理

                if (cmd != null)
                {
                    //異常處理流程，自動補上報
                    cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);

                    if (cmd.COMMANDSTATE < COMMAND_STATUS_BIT_INDEX_ENROUTE)
                    {
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ")
                            + "OHB >> OHB|OHTtoPort 命令"
                            + " OHT_Status:" + statusToName(cmd.COMMANDSTATE)
                            + " 補報 ReportTransferInitiated、ReportCraneActive、ReportCarrierTransferring、ReportZoneCapacityChange"
                        );

                        reportBLL.ReportTransferInitiated(cmd.CMD_ID);
                        reportBLL.ReportCraneActive(cmd.CMD_ID, cmd?.CRANE ?? "");

                        old_dbData.Carrier_LOC = cmd?.CRANE ?? "";
                        reportBLL.ReportCarrierTransferring(cmd, old_dbData, cmd?.CRANE ?? "");

                        if (isShelfPort(cmd.HOSTSOURCE))
                        {
                            reportBLL.ReportZoneCapacityChange(cmd.HOSTSOURCE, null);
                        }
                    }

                    if (cmd.COMMANDSTATE >= COMMAND_STATUS_BIT_INDEX_ENROUTE
                     && cmd.COMMANDSTATE < COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE
                       )
                    {
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ")
                            + "OHB >> OHB|OHTtoPort 命令"
                            + " OHT_Status:" + statusToName(cmd.COMMANDSTATE)
                            + " 補報 ReportCarrierTransferring、ReportZoneCapacityChange"
                        );

                        old_dbData.Carrier_LOC = cmd?.CRANE ?? "";
                        reportBLL.ReportCarrierTransferring(cmd, old_dbData, cmd?.CRANE ?? "");

                        if (isShelfPort(cmd.HOSTSOURCE))
                        {
                            reportBLL.ReportZoneCapacityChange(cmd.HOSTSOURCE, null);
                        }
                    }
                    ACMD_OHTC ohtData = cmdBLL.getCMD_OHTCByMCScmdID(cmd.CMD_ID);

                    OHT_UnLoadCompleted(ohtData, dbData, "OHTtoPort");
                }
                else
                {
                    #region 過帳處理
                    if (dbData != null)
                    {
                        cassette_dataBLL.UpdateCSTLoc(dbData.BOXID, portName, 0);
                        dbData.Carrier_LOC = portName;
                        dbData.Stage = 1;

                        UPStage(dbData, outStage);
                    }
                    #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "OHTtoPort");
            }
        }
        public void PortToOHT(string portName, string cmdSource) //自動過帳，來源 Port 的帳轉到車上
        {
            try
            {
                CassetteData dbCstData = cassette_dataBLL.loadCassetteDataByLoc(portName);

                if (dbCstData != null)
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ")
                        + "OHB >> OHB|誰呼叫:" + cmdSource + " 發現: " + portName + " LoadPosition1 OFF，殘留帳 " + cmdSource + GetCstLog(dbCstData)
                    );

                    ACMD_MCS cmd = cmdBLL.getByCstBoxID(dbCstData.CSTID, dbCstData.BOXID);

                    if (cmd != null)
                    {
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ")
                            + "OHB >> OHB|LoadPosition1 OFF 找到命令:" + GetCmdLog(cmd)
                        );

                        ACMD_OHTC ohtData = cmdBLL.getCMD_OHTCByMCScmdID(cmd.CMD_ID);

                        OHT_LoadCompleted(ohtData, dbCstData, cmd.CRANE, "PortToOHT");
                    }
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "PortToOHT");
            }
        }
        public void PortCarrierRemoved(CassetteData cstData, bool isAGV, string cmdSource)
        {
            try
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "PLC >> OHB|PortCarrierRemoved"
                    + "    CSTID:" + cstData.CSTID
                    + "    BOXID:" + cstData.BOXID
                    + "    Loc:" + cstData.Carrier_LOC
                    + " 誰呼叫:" + cmdSource
                );

                CassetteData dbData = null;

                int stage = portINIData[cstData.Carrier_LOC.Trim()].Stage;

                dbData = cassette_dataBLL.LoadCassetteDataByCVPort(cstData.Carrier_LOC)
                        .Where(cst => cst.Stage == stage)
                        .OrderByDescending(cst => cst.Stage).FirstOrDefault();

                if (dbData == null)
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "PLC >> OHB|PortCarrierRemoved  找不到卡匣資料"
                    );
                    return;
                }

                string HandoffType = "2"; // 1 = manual, 2 = automated

                if (isAGV == false && isUnitType(dbData.Carrier_LOC, UnitType.AGV)) //從MGV Port移除就設為 1
                {
                    HandoffType = "1";
                }

                reportBLL.ReportCarrierRemovedFromPort(dbData, HandoffType);

                cassette_dataBLL.DeleteCSTbyCstBoxID(dbData.CSTID, dbData.BOXID);

                if (isUnitType(dbData.Carrier_LOC, UnitType.AGV))
                {
                    if (isAGV)   //判斷是不是 MGV，註：MGV 跟 一般Port相同，會將BOX取走
                    {
                        PLC_AGV_Station(GetPLC_PortData(dbData.Carrier_LOC), "PortCarrierRemoved");

                        #region 測試用，之後要拿掉，AGV卡匣拿走之後自動切回In
                        //SetPortTypeCmd(dbData.Carrier_LOC, E_PortType.In);
                        //PLC_Report_AGV_PortInOutService(dbData.Carrier_LOC, E_PORT_STATUS.OutOfService);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "PortCarrierRemoved");
            }
        }
        public void ReportPortType(string portID, E_PortType portType, string cmdSource)
        {
            try
            {
                portID = portID.Trim();
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "PLC >> OHB|ReportPortType"
                    + " portID:" + portID
                    + " inout:" + portType
                    + " 誰呼叫:" + cmdSource
                );

                if (portDefBLL != null)
                {
                    if (portDefBLL.GetPortData(portID).PortType != portType)
                    {
                        portDefBLL.UpdataPortType(portID.Trim(), portType);
                    }
                }

                if (reportBLL != null)
                {
                    reportBLL.ReportPortTypeChanging(portID);

                    if (portType == E_PortType.In)
                    {
                        reportBLL.ReportTypeInput(portID);
                    }
                    else if (portType == E_PortType.Out)
                    {
                        reportBLL.ReportPortTypeOutput(portID);
                    }
                }

                if (isUnitType(portID, UnitType.AGV))
                {
                    PLC_AGV_Station(GetPLC_PortData(portID), "ReportPortType");
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "ReportPortType");
            }
        }
        public void PLC_ReportPortInOutService(PortPLCInfo plcInfo)
        {
            try
            {
                string portName = plcInfo.EQ_ID.Trim();
                E_PORT_STATUS service = E_PORT_STATUS.OutOfService;

                if (plcInfo.OpAutoMode)
                {
                    service = E_PORT_STATUS.InService;
                }

                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "PLC >> OHB|PLC_ReportPortInOutService"
                    + "    PortName:" + portName
                    + "    Service:" + service
                );

                if (isUnitType(portName, UnitType.AGV) && service == E_PORT_STATUS.InService)
                {
                    PLC_AGV_Station(plcInfo, "PLC_ReportPortInOutService");
                }
                else
                {
                    PortInOutService(portName, service);
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "PLC_ReportPortInOutService");
            }
        }
        public void PortInOutService(string portName, E_PORT_STATUS service)
        {
            try
            {
                if (portDefBLL != null)
                {
                    PortDef portDB = portDefBLL.GetPortData(portName.Trim());

                    if (portDB.State != service)
                    {
                        portDefBLL.UpdataPortService(portName, service);

                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ") +
                            "OHB >> DB|PortInOutService"
                            + "    PortName:" + portName
                            + "    Service:" + service
                        );

                        if (service == E_PORT_STATUS.InService)
                        {
                            if (isUnitType(portDB.PLCPortID, UnitType.AGV))
                            {
                                if (portDB.PortType == E_PortType.In)
                                {
                                    reportBLL.ReportTypeInput(portDB.PLCPortID.Trim());
                                }

                                if (portDB.PortType == E_PortType.Out)
                                {
                                    reportBLL.ReportPortTypeOutput(portDB.PLCPortID.Trim());
                                }
                            }

                            reportBLL.ReportPortInService(portName);
                        }
                        else if (service == E_PORT_STATUS.OutOfService)
                        {
                            reportBLL.ReportPortOutOfService(portName);
                        }
                    }
                    else
                    {
                        if (iniStatus == false)
                        {
                            portDB.State = service;
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ") +
                                "OHB >> DB|目前 DB Port 狀態: "
                                + "    PortName:" + portName
                                + "    State:" + portDB.State
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "PLC_ReportPortInOutService");
            }
        }

        #region AGV 專有事件

        string agvStationBug = ""; //預防重複上報
        public void PLC_AGV_Station(PortPLCInfo plcInfo, string sourceCmd)   //AGV Port流程
        {
            try
            {
                string s = DateTime.Now.ToString("HH:mm:ss") + " " + plcInfo.EQ_ID.Trim();

                if (agvStationBug.Contains(s))
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "PLC >> OHB|PLC_AGV_Station 短時間內觸發"
                        + "    agvStationBug:" + agvStationBug
                        + "    DateTime.Now:" + s
                    );
                    return;
                }
                else
                {
                    agvStationBug = s;
                }

                if (GetIgnoreModeChange(plcInfo))
                {
                    return;
                }

                if (portINIData == null)
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") + "PLC_AGV_Station PortPLCInfo = Null"
                    );

                    return;
                }

                if (plcInfo.IsReadyToLoad == false && plcInfo.IsReadyToUnload == false)
                {
                    return;
                }

                if (plcInfo.OpAutoMode)
                {
                    if (plcInfo.IsAGVMode)
                    {
                        if (plcInfo.IsInputMode)
                        {
                            PLC_AGV_Station_InMode(plcInfo);
                        }
                        else if (plcInfo.IsOutputMode)
                        {
                            PLC_AGV_Station_OutMode(plcInfo);
                        }
                    }
                    else
                    {
                        PortInOutService(plcInfo.EQ_ID, E_PORT_STATUS.InService);
                    }
                }
                else
                {
                    PortInOutService(plcInfo.EQ_ID, E_PORT_STATUS.OutOfService);
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "PLC_AGV_Station");
            }
        }
        public void PLC_AGV_Station_InMode(PortPLCInfo plcInfo)
        {
            try
            {
                bool agvToShelf = false;
                bool shelfToAGV = false;

                E_PORT_STATUS status = E_PORT_STATUS.OutOfService;

                if (plcInfo.LoadPosition1) //portData.LoadPosition1 = BOX 在席                
                {
                    if (plcInfo.IsCSTPresence)  //portData.IsCSTPresence = CST 在席)
                    {
                        if (plcInfo.CSTPresenceMismatch)
                        {
                            #region Log
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ") +
                                "PLC >> OHB|PLC_AGV_Station_InMode "
                                + plcInfo.EQ_ID + " 退實箱"
                                + " LoadPosition1:" + plcInfo.LoadPosition1
                                + " IsCSTPresence:" + plcInfo.IsCSTPresence
                                + " CSTPresenceMismatch:" + plcInfo.CSTPresenceMismatch
                            );
                            #endregion

                            agvToShelf = true;
                        }
                        else
                        {
                            #region Log
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ") +
                                "PLC >> OHB|PLC_AGV_Station_InMode " + plcInfo.EQ_ID + " Mismatch 沒報，等待 WaitIn"
                            );
                            #endregion

                            status = E_PORT_STATUS.InService;
                        }
                    }
                    else
                    {
                        if (plcInfo.AGVPortReady)
                        {
                            bool idFail = (string.IsNullOrWhiteSpace(plcInfo.BoxID) || plcInfo.BoxID.Contains("ERROR1"));

                            if (idFail)  //portData.IsCSTPresence = CST 在席
                            {
                                status = E_PORT_STATUS.OutOfService;

                                #region Log
                                TransferServiceLogger.Info
                                (
                                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                                    "PLC >> OHB|PLC_AGV_Station_InMode "
                                    + plcInfo.EQ_ID + " BOX ID 讀不到，退空BOX"
                                    + " AGVPortReady:" + plcInfo.AGVPortReady
                                );
                                #endregion

                                agvToShelf = true;
                            }
                            else
                            {
                                status = E_PORT_STATUS.InService;
                            }
                        }
                        else
                        {
                            #region Log
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ") +
                                "PLC >> OHB|PLC_AGV_Station_InMode" + plcInfo.EQ_ID + " AGVPortReady 沒報:" + plcInfo.AGVPortReady
                            );
                            #endregion
                            //status = E_PORT_STATUS.InService;
                        }
                    }
                }
                else
                {
                    shelfToAGV = true;
                }

                PortInOutService(plcInfo.EQ_ID, status);

                if (portINIData[plcInfo.EQ_ID].openAGV_Station == false || plcInfo.OpAutoMode == false)
                {
                    return;
                }

                if (agvToShelf)  //退BOX
                {
                    if (CheckPortType(plcInfo.EQ_ID))
                    {
                        return;
                    }

                    MovebackBOX(plcInfo.CassetteID, plcInfo.BoxID, plcInfo.EQ_ID, plcInfo.IsCSTPresence);
                }

                if (shelfToAGV)  //補BOX
                {
                    if (CheckPortType(plcInfo.EQ_ID))
                    {
                        return;
                    }

                    CassetteData dbCstData = GetNearestEmptyBox(plcInfo.EQ_ID);

                    if (dbCstData != null)
                    {
                        BoxMovCmd(dbCstData, plcInfo.EQ_ID, UnitType.AGV);
                    }
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "PLC_AGV_Station_InMode");
            }
        }
        public void PLC_AGV_Station_OutMode(PortPLCInfo plcInfo)
        {
            try
            {
                E_PORT_STATUS status = E_PORT_STATUS.OutOfService;

                bool movebackBOX = false;
                bool waitOut = false;
                string dbCstID = "";
                CassetteData dbCstData = cassette_dataBLL.loadCassetteDataByLoc(plcInfo.EQ_ID);

                if (plcInfo.LoadPosition1) //portData.LoadPosition1 = BOX 在席
                {
                    if (plcInfo.IsCSTPresence)  //portData.IsCSTPresence = CST 在席
                    {
                        if (plcInfo.PortWaitOut)
                        {
                            if (dbCstData != null)
                            {
                                if (dbCstData.CSTState != E_CSTState.WaitOut)
                                {
                                    dbCstID = dbCstData.CSTID;
                                    if (plcInfo.CassetteID.Trim() == dbCstData.CSTID.Trim())
                                    {
                                        status = E_PORT_STATUS.InService;
                                        //cassette_dataBLL.UpdateCSTState(dbCstData.BOXID, (int)E_CSTState.WaitOut);

                                        waitOut = true;
                                    }
                                    else
                                    {
                                        TransferServiceLogger.Info
                                        (
                                            DateTime.Now.ToString("HH:mm:ss.fff ") +
                                            "OHB >> AGV|PLC_AGV_Station_OutMode "
                                            + plcInfo.EQ_ID + " 退BOX:   CSTID不符"
                                            + " AGVRead: " + plcInfo.CassetteID.Trim()
                                            + " dbCstData: " + dbCstData.CSTID.Trim()
                                        );
                                        movebackBOX = true;
                                    }
                                }

                                if (dbCstData.CSTState == E_CSTState.WaitOut)
                                {
                                    status = E_PORT_STATUS.InService;
                                }
                            }
                            else
                            {
                                TransferServiceLogger.Info
                                (
                                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                                    "OHB >> AGV|PLC_AGV_Station_OutMode dbCstData = Null ，先建帳" + plcInfo.EQ_ID + " PortWaitOut:" + plcInfo.PortWaitOut
                                );

                                CassetteData agvCSTData = new CassetteData();
                                agvCSTData.CSTID = plcInfo.CassetteID;
                                agvCSTData.BOXID = plcInfo.BoxID;
                                agvCSTData.Carrier_LOC = plcInfo.EQ_ID;
                                agvCSTData = IDRead(agvCSTData);

                                OHBC_InsertCassette(agvCSTData.CSTID, agvCSTData.BOXID, agvCSTData.Carrier_LOC);
                            }
                        }
                        else if (plcInfo.CSTPresenceMismatch)
                        {
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ") +
                                "OHB >> AGV|PLC_AGV_Station_OutMode "
                                + plcInfo.EQ_ID + " 退BOX:   CSTPresenceMismatch:" + plcInfo.CSTPresenceMismatch
                            );

                            movebackBOX = true;
                        }
                        else
                        {
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ") +
                                "OHB >> AGV|PLC_AGV_Station_OutMode " + plcInfo.EQ_ID + " 等待WaitOut"
                            );
                        }
                    }
                    else
                    {
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ") +
                            "OHB >> AGV|PLC_AGV_Station_OutMode " + plcInfo.EQ_ID + "退空BOX"
                        );
                        movebackBOX = true;
                    }
                }
                else
                {
                    movebackBOX = false;
                    status = E_PORT_STATUS.InService;
                }

                PortInOutService(plcInfo.EQ_ID, status);

                if (waitOut)
                {
                    if (dbCstData != null)
                    {
                        reportBLL.ReportCarrierWaitOut(dbCstData, "1");
                    }
                }

                if (portINIData[plcInfo.EQ_ID].openAGV_Station == false || plcInfo.OpAutoMode == false)
                {
                    return;
                }

                if (movebackBOX)    //退BOX
                {
                    if (CheckPortType(plcInfo.EQ_ID))
                    {
                        return;
                    }

                    if (portINIData[plcInfo.EQ_ID.Trim()].movebackBOXsleep == false)
                    {
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ") +
                            "OHB >> AGV|OutMode 退BOX 延遲 300 毫秒再檢查一次 " + plcInfo.EQ_ID
                        );
                        portINIData[plcInfo.EQ_ID.Trim()].movebackBOXsleep = true;
                        Thread.Sleep(300);
                        PLC_AGV_Station_OutMode(GetPLC_PortData(plcInfo.EQ_ID));
                    }

                    MovebackBOX(plcInfo.CassetteID, plcInfo.BoxID, plcInfo.EQ_ID, plcInfo.IsCSTPresence);
                }

                portINIData[plcInfo.EQ_ID.Trim()].movebackBOXsleep = false;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "PLC_AGV_Station_OutMode");
            }
        }

        #region 補BOX、退BOX，控制處理

        public void MovebackBOX(string cstID, string boxID, string cstLoc, bool cstPresence)
        {
            try
            {
                CassetteData cstData = new CassetteData();
                cstData.CSTID = cstID.Trim();
                cstData.BOXID = boxID.Trim();
                cstData.Carrier_LOC = cstLoc;

                cstData = IDRead(cstData);

                BoxMovCmd(cstData, cstData.Carrier_LOC, UnitType.SHELF);
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "MovebackBOX");
            }
        }
        public bool CheckPortType(string portName)    //檢查是切換流向
        {
            try
            {
                portName = portName.Trim();
                bool check = false;

                if (portINIData[portName].openAGV_AutoPortType == false)
                {
                    return check;
                }

                List<PortDef> AGVPortGroup = portDefBLL.GetAGVPortGroupData(line.LINE_ID, portName);

                int nowInModeCount = AGVPortGroup.Where(data => data.PortType == E_PortType.In).Count();
                int nowOutModeCount = AGVPortGroup.Where(data => data.PortType == E_PortType.Out).Count();

                int defInModeCount = AGVPortGroup.Where(data => data.PortTypeDef == E_PortType.In).Count();
                int defOutModeCount = AGVPortGroup.Where(data => data.PortTypeDef == E_PortType.Out).Count();

                PortPLCInfo plcInfo = GetPLC_PortData(portName);

                if (plcInfo.IsInputMode && plcInfo.LoadPosition1 == false) //PLC 目前為 InputMode 且上面 無BOX
                {
                    #region Log
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ")
                        + plcInfo.EQ_ID
                        + " 目前為 InputMode 且上面 無BOX"
                        + " IsInputMode : " + plcInfo.IsInputMode
                        + " LoadPosition1 :" + plcInfo.LoadPosition1
                    );
                    #endregion
                    if (nowOutModeCount < defOutModeCount)   //Out，檢查預設流向是否符合
                    {
                        ACMD_MCS cmdData = null;
                        bool isCmdExist = false;

                        foreach (var v in AGVPortGroup)  //其他AGV Port 有沒有在切換 OutMode
                        {
                            string cmdID = "PortTypeChange-" + v.PLCPortID.Trim() + ">>" + E_PortType.Out;
                            cmdData = cmdBLL.getNowCMD_MCSByID(cmdID);

                            if (cmdData != null)
                            {
                                isCmdExist = true;

                                if (portName == v.PLCPortID.Trim())
                                {
                                    check = true;
                                }

                                break;
                            }
                        }

                        if (isCmdExist == false)
                        {
                            SetPortTypeCmd(portName, E_PortType.Out);
                            check = true;

                            #region Log
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ")
                                + "nowOutModeCount < defOutModeCount， "
                                + portName + " 自動切 Out"
                            );
                            #endregion
                        }
                    }

                    foreach (var v in AGVPortGroup)
                    {
                        if (portName == v.PLCPortID.Trim())
                        {
                            continue;
                        }

                        #region Log
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ")
                            + "IsOutputMode: " + v.IsOutputMode
                            + "PortWaitOut: " + v.PortWaitOut
                            + "LoadPosition1: " + v.LoadPosition1
                            + "IsCSTPresence: " + v.IsCSTPresence
                        );
                        #endregion
                        #region 流向對調

                        #region 檢查有沒有命令要搬到其他 AGV Port
                        ACMD_MCS cmdDest = cmdBLL.GetCmdDataByDest(v.PLCPortID).FirstOrDefault();

                        if (cmdDest != null)
                        {
                            if (v.PortType == E_PortType.Out)
                            {
                                //發現有命令要搬到OutMode
                                SetPortTypeCmd(portName, E_PortType.Out);
                                check = true;
                                #region Log
                                TransferServiceLogger.Info
                                (
                                    DateTime.Now.ToString("HH:mm:ss.fff ")
                                    + "有命令要搬到其他 AGV Port， "
                                    + portName + " 自動切 Out"
                                );
                                #endregion
                                break;
                            }
                        }
                        #endregion
                        #region 檢查其他 AGV Port 是否為 OutMode 且 WaitOut
                        if (v.IsOutputMode && v.PortWaitOut)
                        {
                            SetPortTypeCmd(portName, E_PortType.Out);
                            check = true;
                            #region Log
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ")
                                + "其他 AGV Port 是否為 OutMode 且 WaitOut， "
                                + portName + " 自動切 Out"
                            );
                            #endregion
                            break;
                        }
                        #endregion
                        #region 檢查其他 AGV Port 是否為 OutMode ，上面有空 BOX
                        if (v.IsOutputMode && v.LoadPosition1 && v.IsCSTPresence == false)
                        {
                            ACMD_MCS cmdSource = cmdBLL.GetCmdDataBySource(v.PLCPortID);

                            if (cmdSource != null)
                            {
                                if (cmdSource.CMDTYPE == CmdType.AGVStation.ToString())
                                {
                                    if (cmdSource.TRANSFERSTATE == E_TRAN_STATUS.Queue)
                                    {
                                        Manual_DeleteCmd(cmdSource.CMD_ID, "流向對調，取消退");

                                        SetPortTypeCmd(v.PLCPortID, E_PortType.In);
                                        SetPortTypeCmd(portName, E_PortType.Out);
                                        check = true;
                                        #region Log
                                        TransferServiceLogger.Info
                                        (
                                            DateTime.Now.ToString("HH:mm:ss.fff ")
                                            + "刪除退BOX命令，其他 AGV Port 是否為 OutMode ，上面有空 BOX， "
                                            + portName + " 自動切 Out"
                                        );
                                        #endregion
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                SetPortTypeCmd(v.PLCPortID, E_PortType.In);
                                SetPortTypeCmd(portName, E_PortType.Out);
                                check = true;
                                #region Log
                                TransferServiceLogger.Info
                                (
                                    DateTime.Now.ToString("HH:mm:ss.fff ")
                                    + "其他 AGV Port 是否為 OutMode ，上面有空 BOX， "
                                    + portName + " 自動切 Out"
                                );
                                #endregion
                                break;
                            }
                        }
                        #endregion

                        #endregion
                    }

                }

                if (plcInfo.IsOutputMode && plcInfo.LoadPosition1 && plcInfo.IsCSTPresence == false)   //PLC 目前為 InputMode 且上面 有空BOX
                {
                    #region Log
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ")
                        + "目前為 InputMode 且上面 無BOX"
                        + " IsOutputMode : " + plcInfo.IsOutputMode
                        + " LoadPosition1 :" + plcInfo.LoadPosition1
                        + " IsCSTPresence :" + plcInfo.IsCSTPresence
                    );
                    #endregion
                    if (nowInModeCount < defInModeCount)   //In
                    {
                        ACMD_MCS cmdData = null;
                        bool isCmdExist = false;

                        foreach (var v in AGVPortGroup)  //其他AGV Port 有沒有在切換 OutMode
                        {
                            string cmdID = "PortTypeChange-" + v.PLCPortID.Trim() + ">>" + E_PortType.In;
                            cmdData = cmdBLL.getNowCMD_MCSByID(cmdID);

                            if (cmdData != null)
                            {
                                isCmdExist = true;

                                if (portName == v.PLCPortID.Trim())
                                {
                                    check = true;
                                }

                                break;
                            }
                            break;
                        }

                        if (isCmdExist == false)
                        {
                            SetPortTypeCmd(portName, E_PortType.In);
                            check = true;
                            #region Log
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ")
                                + "nowInModeCount < defInModeCount， "
                                + portName + " 自動切 In"
                            );
                            #endregion
                        }
                    }

                    foreach (var v in AGVPortGroup)
                    {
                        if (portName == v.PLCPortID.Trim())
                        {
                            continue;
                        }
                        #region Log
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ")
                            + "IsInputMode: " + v.IsInputMode
                            + "PortWaitIn: " + v.PortWaitIn
                            + "LoadPosition1: " + v.LoadPosition1
                        );
                        #endregion
                        #region 流向對調

                        #region 檢查其他 AGV Port 是否為 InMode 且 WaitIn
                        if (v.IsInputMode && v.PortWaitIn)
                        {
                            SetPortTypeCmd(portName, E_PortType.In);
                            check = true;
                            break;
                        }
                        #endregion
                        #region 檢查其他 AGV Port 是否為 InMode ，上面沒 BOX
                        if (v.IsInputMode && v.LoadPosition1 == false)
                        {
                            ACMD_MCS cmdDest = cmdBLL.GetCmdDataByDest(v.PLCPortID).FirstOrDefault();

                            if (cmdDest != null)
                            {
                                if (cmdDest.CMDTYPE == CmdType.AGVStation.ToString())
                                {
                                    if (cmdDest.TRANSFERSTATE == E_TRAN_STATUS.Queue)
                                    {
                                        Manual_DeleteCmd(cmdDest.CMD_ID, "流向對調，取消補");

                                        SetPortTypeCmd(v.PLCPortID, E_PortType.Out);
                                        SetPortTypeCmd(portName, E_PortType.In);
                                        check = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                SetPortTypeCmd(v.PLCPortID, E_PortType.Out);
                                SetPortTypeCmd(portName, E_PortType.In);
                                check = true;
                                break;
                            }
                        }
                        #endregion

                        #endregion
                    }
                }
                return check;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "CheckPortType");
                return false;
            }
        }

        private long syncTranBoxMovCmdPoint = 0;
        public void BoxMovCmd(CassetteData emptyBoxData, string portLoc, UnitType destUnitType)
        {
            if (Interlocked.Exchange(ref syncTranBoxMovCmdPoint, 1) == 0)
            {
                try
                {
                    string cmdSource = "";
                    string cmdDest = "";

                    if (destUnitType == UnitType.AGV)   //新增命令補空BOX
                    {
                        if (cmdBLL.GetCmdDataBySHELFtoAGV(portLoc) != null)
                        {
                            #region Log
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ")
                                + " PLC >> OHB|GetCmdDataBySHELFtoAGV  "
                                + emptyBoxData.Carrier_LOC + "  已有補BOX命令"
                            );
                            #endregion
                            return;
                        }
                        #region 補BOX   

                        cmdSource = emptyBoxData.Carrier_LOC;
                        cmdDest = portLoc;

                        #endregion
                    }
                    else if (destUnitType == UnitType.SHELF)    //新增命令退BOX
                    {
                        if (cmdBLL.GetCmdDataByAGVtoSHELF(emptyBoxData.Carrier_LOC) != null)
                        {
                            #region Log
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ")
                                + " PLC >> OHB|GetCmdDataByAGVtoSHELF  "
                                + emptyBoxData.Carrier_LOC + "  已有退BOX命令"
                            );
                            #endregion
                            return;
                        }
                        #region 退到儲位

                        cmdSource = emptyBoxData.Carrier_LOC;
                        var shelfData = shelfDefBLL.LoadEnableShelf();

                        string shelfID = scApp.TransferService.GetShelfRecentLocation(shelfData, cmdSource);

                        if (string.IsNullOrWhiteSpace(shelfID) == false)
                        {
                            cmdDest = shelfID;
                        }

                        #endregion
                    }
                    else
                    {
                        return;
                    }

                    if (isUnitType(cmdSource, UnitType.AGV))
                    {
                        reportBLL.ReportTypeInput(cmdSource);

                        OHBC_InsertCassette(emptyBoxData.CSTID, emptyBoxData.BOXID, emptyBoxData.Carrier_LOC);
                    }

                    if (isUnitType(cmdDest, UnitType.AGV))
                    {
                        reportBLL.ReportPortTypeOutput(cmdDest);
                    }

                    Manual_InsertCmd(cmdSource, cmdDest, "", 5, "BoxMovCmd", CmdType.AGVStation);
                }
                catch (Exception ex)
                {
                    TransferServiceLogger.Error(ex, "EmptyBoxMovCmd");
                }
                finally
                {
                    Interlocked.Exchange(ref syncTranBoxMovCmdPoint, 0);
                }
            }
        }
        public void PLC_AGV_CancelCmd(string portID) //取消補退BOX的命令
        {
            string portName = portID.Trim();
            ACMD_MCS cmdAGVtoSHELF = cmdBLL.GetCmdDataByAGVtoSHELF(portName);
            ACMD_MCS cmdSHELFtoAGV = cmdBLL.GetCmdDataBySHELFtoAGV(portName);

            if (cmdAGVtoSHELF != null)
            {
                if (cmdAGVtoSHELF.TRANSFERSTATE == E_TRAN_STATUS.Queue)
                {
                    #region Log
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "PLC >> OHB|PLC_AGV_CancelCmd 取消退 BOX 命令\n"
                        + GetCmdLog(cmdAGVtoSHELF)
                    );
                    #endregion
                    Manual_DeleteCmd(cmdAGVtoSHELF.CMD_ID, "取消退 BOX 命令");
                }
            }

            if (cmdSHELFtoAGV != null)
            {
                if (cmdSHELFtoAGV.TRANSFERSTATE == E_TRAN_STATUS.Queue)
                {
                    #region Log
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "PLC >> OHB|PLC_AGV_CancelCmd 取消補 BOX 命令\n"
                        + GetCmdLog(cmdSHELFtoAGV)
                    );
                    #endregion
                    Manual_DeleteCmd(cmdSHELFtoAGV.CMD_ID, "取消補 BOX 命令");
                }
            }
        }

        #endregion

        #endregion

        #endregion

        #region OHB >> PLC
        public bool PortTypeChange(string portID, E_PortType mode, string apiSource)
        {
            try
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "OHB >> PLC|PortTypeChange"
                    + "    誰呼叫:" + apiSource
                    + "    portID:" + portID
                    + "    inout:" + mode
                );

                PortPLCInfo plcInfo = GetPLC_PortData(portID);

                bool typeEnable = plcInfo.IsModeChangable;

                if (typeEnable == false)
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "PLC >> OHB|目前不能切流向 IsModeChangable = " + typeEnable
                    );

                    string cmdID = "PortTypeChange-" + portID + ">>" + mode;

                    if (cmdBLL.getCMD_MCSByID(cmdID) != null)
                    {
                        return true;
                    }

                    SetPortTypeCmd(portID, mode);

                    return true;
                }

                PortValueDefMapAction portValueDefMapAction = scApp.getEQObjCacheManager().getPortByPortID(portID).getMapActionByIdentityKey(typeof(PortValueDefMapAction).Name) as PortValueDefMapAction;

                if (mode == E_PortType.In)
                {
                    portValueDefMapAction.Port_ChangeToOutput(false);
                    portValueDefMapAction.Port_ChangeToInput(true);
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "OHB >> PLC|對"
                        + " PortID:" + portID
                        + " InMode: true"
                        + " OutMode: False"
                        + " 目前狀態 InputMode:" + plcInfo.IsInputMode + "  OutputMode:" + plcInfo.IsOutputMode
                    );

                    if (plcInfo.IsInputMode)
                    {
                        ReportPortType(portID, mode, "PortTypeChange");
                    }
                }
                else if (mode == E_PortType.Out)
                {
                    portValueDefMapAction.Port_ChangeToInput(false);
                    portValueDefMapAction.Port_ChangeToOutput(true);
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "OHB >> PLC|對"
                        + " PortID:" + portID
                        + " InMode: False"
                        + " OutMode: true "
                        + " 目前狀態 InputMode:" + plcInfo.IsInputMode + "  OutputMode:" + plcInfo.IsOutputMode
                    );

                    if (plcInfo.IsOutputMode)
                    {
                        ReportPortType(portID, mode, "PortTypeChange");
                    }
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "PortTypeChange");
                return false;
            }
        }
        public bool PortCommanding(string portID, bool Commanding)  //通知PLC有命令要過去，不能切換流向
        {
            try
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHB >> PLC|PortCommanding"
                    + "    portID:" + portID
                    + "    Commanding:" + Commanding
                );

                if (isCVPort(portID))
                {
                    PortValueDefMapAction portValueDefMapAction = scApp.getEQObjCacheManager().getPortByPortID(portID).getMapActionByIdentityKey(typeof(PortValueDefMapAction).Name) as PortValueDefMapAction;

                    portValueDefMapAction.Port_OHCV_Commanding(Commanding);
                }

                return true;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "Port_OHCV_Commanding");
                return false;
            }
        }
        public PortPLCInfo GetPLC_PortData(string portID)
        {
            try
            {
                portID = portID.Trim();
                PortValueDefMapAction portValueDefMapAction = scApp.getEQObjCacheManager().getPortByPortID(portID).getMapActionByIdentityKey(typeof(PortValueDefMapAction).Name) as PortValueDefMapAction;

                return portValueDefMapAction.GetPortValue();
            }
            catch (Exception ex)
            {
                string st_details = "";

                try
                {
                    System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
                    st_details = st.ToString();
                }
                catch (Exception eex)
                {
                    TransferServiceLogger.Error(eex);
                }

                TransferServiceLogger.Error(ex, "GetPLC_PortData    portID:" + portID + "\n" + "推疊：" + st_details);
                return null;
            }
        }

        public bool toAGV_Mode(string portID)
        {
            try
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHB >> PLC|toAGV_Mode"
                    + "    portID:" + portID
                );

                if (isUnitType(portID, UnitType.AGV))
                {
                    PortValueDefMapAction portValueDefMapAction = scApp.getEQObjCacheManager().getPortByPortID(portID).getMapActionByIdentityKey(typeof(PortValueDefMapAction).Name) as PortValueDefMapAction;

                    portValueDefMapAction.Port_ChangeToAGVMode();
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "toAGV_Mode");
                return false;
            }
        }
        public bool toMGV_Mode(string portID)
        {
            try
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHB >> PLC|toMGV_Mode"
                    + "    portID:" + portID
                );

                if (isUnitType(portID, UnitType.AGV))
                {
                    PortValueDefMapAction portValueDefMapAction = scApp.getEQObjCacheManager().getPortByPortID(portID).getMapActionByIdentityKey(typeof(PortValueDefMapAction).Name) as PortValueDefMapAction;

                    portValueDefMapAction.Port_ChangeToMGVMode();
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "toAGV_Mode");
                return false;
            }
        }
        public bool SetPortRun(string portID)
        {
            try
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHB >> PLC|SetPortRun"
                    + "    portID:" + portID
                );

                PortValueDefMapAction portValueDefMapAction = scApp.getEQObjCacheManager().getPortByPortID(portID).getMapActionByIdentityKey(typeof(PortValueDefMapAction).Name) as PortValueDefMapAction;

                portValueDefMapAction.Port_RUN();
                return true;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "toAGV_Mode");
                return false;
            }
        }
        public bool SetPortStop(string portID)
        {
            try
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHB >> PLC|SetPortStop"
                    + "    portID:" + portID
                );

                PortValueDefMapAction portValueDefMapAction = scApp.getEQObjCacheManager().getPortByPortID(portID).getMapActionByIdentityKey(typeof(PortValueDefMapAction).Name) as PortValueDefMapAction;

                portValueDefMapAction.Port_STOP();
                return true;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "toAGV_Mode");
                return false;
            }
        }
        public bool PortAlarrmReset(string portID)
        {
            try
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHB >> PLC|PortAlarrmReset"
                    + "    portID:" + portID
                );

                PortValueDefMapAction portValueDefMapAction = scApp.getEQObjCacheManager().getPortByPortID(portID).getMapActionByIdentityKey(typeof(PortValueDefMapAction).Name) as PortValueDefMapAction;

                portValueDefMapAction.Port_PortAlarrmReset(true);
                Thread.Sleep(500);
                portValueDefMapAction.Port_PortAlarrmReset(false);
                return true;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "toAGV_Mode");
                return false;
            }
        }
        public bool SetAGV_PortOpenBOX(string portID)
        {
            try
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHB >> PLC|SetAGV_PortOpenBOX"
                    + "    portID:" + portID
                );

                PortValueDefMapAction portValueDefMapAction = scApp.getEQObjCacheManager().getPortByPortID(portID).getMapActionByIdentityKey(typeof(PortValueDefMapAction).Name) as PortValueDefMapAction;

                portValueDefMapAction.Port_ToggleBoxCover(true);
                Thread.Sleep(500);
                portValueDefMapAction.Port_ToggleBoxCover(false);
                return true;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "toAGV_Mode");
                return false;
            }
        }
        public bool SetAGV_PortBCR_Read(string portID)
        {
            try
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHB >> PLC|SetAGV_PortBCR_Read"
                    + "    portID:" + portID
                );

                PortValueDefMapAction portValueDefMapAction = scApp.getEQObjCacheManager().getPortByPortID(portID).getMapActionByIdentityKey(typeof(PortValueDefMapAction).Name) as PortValueDefMapAction;

                portValueDefMapAction.Port_BCR_Read(true);
                return true;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "toAGV_Mode");
                return false;
            }
        }
        public bool RstAGV_PortBCR_Read(string portID)
        {
            try
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHB >> PLC|RstAGV_PortBCR_Read"
                    + "    portID:" + portID
                );

                PortValueDefMapAction portValueDefMapAction = scApp.getEQObjCacheManager().getPortByPortID(portID).getMapActionByIdentityKey(typeof(PortValueDefMapAction).Name) as PortValueDefMapAction;

                portValueDefMapAction.Port_BCR_Read(false);
                return true;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "toAGV_Mode");
                return false;
            }
        }


        #endregion

        #region OHBC 控制

        #region 命令、卡匣處理

        #region 命名規則
        public string CarrierDouble(string loc)   //二重格
        {
            return "UNKS" + loc + GetStDate() + string.Format("{0:00}", DateTime.Now.Second);
        }
        public string CarrierReadFail(string loc)   //卡匣讀不到
        {
            return "UNKF" + loc + GetStDate() + string.Format("{0:00}", DateTime.Now.Second);
        }
        public string CarrierReadduplicate(string bcrcsid)  //卡匣重複
        {
            return "UNKD" + bcrcsid + GetStDate() + string.Format("{0:00}", DateTime.Now.Second);
        }
        public bool ase_ID_Check(string str)    //ASE CST BOX 帳料命名規則
        {
            bool b = false;
            string str12 = str.Substring(0, 2); //1、2碼為數字
            string str34 = str.Substring(2, 2); //3、4碼為英文
            string str58 = str.Substring(4, 4); //5~8碼為數字 + 英文混合

            if (IsNumber(str12) && IsEnglish(str34) && IsEnglish_Number(str58) && IsEnglish_Number(str))
            {
                b = true;
            }

            return b;
        }
        public bool IsEnglish_Number(string str)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$");
            return reg1.IsMatch(str);
        }
        public bool IsNumber(string str)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[0-9]+$");
            return reg1.IsMatch(str);
        }
        public bool IsEnglish(string str)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[A-Za-z]+$");
            return reg1.IsMatch(str);
        }
        public string GetStDate()
        {
            int Y = DateTime.Now.Year % 100;
            string stDate = string.Format("{0}{1:00}{2:00}{3:00}{4:00}", Y, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute);

            return stDate;
        }
        #endregion

        #region 命令處理

        #region Scan 命令

        public string SetScanCmd(string cstid, string boxid, string loc)
        {
            try
            {
                if (isUnitType(loc, UnitType.SHELF) == false)
                {
                    return "不是 SHELF";
                }

                ACMD_MCS datainfo = new ACMD_MCS();
                bool cmdExist = true;
                int cmdNo = 1;
                string cmdID = "";

                cmdID = "SCAN-" + GetStDate();
                datainfo.CARRIER_ID = cstid;
                datainfo.BOX_ID = boxid;
                datainfo.HOSTSOURCE = loc;
                datainfo.HOSTDESTINATION = loc;
                datainfo.CMDTYPE = CmdType.SCAN.ToString();
                datainfo.PRIORITY = 50;

                while (cmdExist)
                {
                    if (cmdBLL.getCMD_MCSByID(cmdID + cmdNo) == null)
                    {
                        datainfo.CMD_ID = cmdID + cmdNo;
                        cmdExist = false;
                    }
                    else
                    {
                        cmdNo++;
                    }
                }

                datainfo.LOT_ID = "";
                datainfo.CMD_INSER_TIME = DateTime.Now;
                datainfo.TRANSFERSTATE = E_TRAN_STATUS.Queue;
                datainfo.COMMANDSTATE = ACMD_MCS.COMMAND_iIdle;
                datainfo.CHECKCODE = "";
                datainfo.PAUSEFLAG = "";
                datainfo.TIME_PRIORITY = 0;
                datainfo.PORT_PRIORITY = 0;
                datainfo.REPLACE = 1;
                datainfo.PRIORITY_SUM = datainfo.PRIORITY + datainfo.TIME_PRIORITY + datainfo.PORT_PRIORITY;
                datainfo.CRANE = "";

                if (cmdBLL.creatCommand_MCS(datainfo))
                {
                    return "OK";
                }
                else
                {
                    return "失敗";
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "SetScanCmd");
                return "失敗";
            }
        }
        public void ScanALL()   //SCAN 全部
        {
            foreach (var v in portINIData)
            {
                if (isShelfPort(v.Value.PortName))
                {
                    SetScanCmd("", "", v.Value.PortName);
                }
            }
        }
        public void ScanShelfCstData()  //SCAN 既有帳
        {
            foreach (var v in cassette_dataBLL.loadCassetteData())
            {
                if (isShelfPort(v.Carrier_LOC))
                {
                    SetScanCmd("", "", v.Carrier_LOC);
                }
            }
        }

        #endregion

        public void DeleteCmd(ACMD_MCS cmdData)
        {
            if (cmdData.CMDTYPE == CmdType.PortTypeChange.ToString())
            {
                cmdBLL.DeleteCmd(cmdData.CMD_ID);
            }

            if (cmdData.TRANSFERSTATE == E_TRAN_STATUS.Queue)
            {
                reportBLL.ReportOperatorInitiatedAction(cmdData.CMD_ID, reportMCSCommandType.Cancel.ToString());
                scApp.VehicleService.doCancelOrAbortCommandByMCSCmdID(cmdData.CMD_ID, CMDCancelType.CmdCancel);
            }
            else
            {
                reportBLL.ReportOperatorInitiatedAction(cmdData.CMD_ID, reportMCSCommandType.Abort.ToString());
                scApp.VehicleService.doCancelOrAbortCommandByMCSCmdID(cmdData.CMD_ID, CMDCancelType.CmdAbort);
            }
        }

        #endregion
        #region 卡匣處理

        #region 帳料處理
        public CassetteData IDRead(CassetteData cstData)    //太多地方要判斷讀取結果，之後看能不能統一 (找 ERROR1)
        {
            CassetteData readData = cstData.Clone();
            IDreadStatus idReadStatus = IDreadStatus.successful;
            bool carrierIDFail = false;
            bool boxIDFail = false;

            #region 卡匣讀不到檢查
            if (readData.CSTID.Contains("NOCST1") || string.IsNullOrWhiteSpace(readData.CSTID))
            {
                readData.CSTID = "";
            }

            if (readData.CSTID.Contains("ERROR1") || readData.CSTID.Contains("NORD01"))
            {
                readData.CSTID = CarrierReadFail(readData.Carrier_LOC);
                carrierIDFail = true;
            }

            if (readData.BOXID.Contains("ERROR1") || readData.BOXID.Contains("NORD01") || string.IsNullOrWhiteSpace(readData.BOXID))
            {
                //B0.03
                scApp.TransferService.OHBC_AlarmSet(readData.Carrier_LOC, ((int)AlarmLst.EQ_BCR_READ_FAIL).ToString());
                scApp.TransferService.OHBC_AlarmCleared(readData.Carrier_LOC, ((int)AlarmLst.EQ_BCR_READ_FAIL).ToString());
                //

                readData.BOXID = CarrierReadFail(readData.Carrier_LOC);
                boxIDFail = true;
            }

            #region ReadStatus
            if (carrierIDFail)
            {
                idReadStatus = IDreadStatus.CSTReadFail_BoxIsOK;
            }

            if (boxIDFail)
            {
                idReadStatus = IDreadStatus.BoxReadFail_CstIsOK;
            }

            if (carrierIDFail && boxIDFail)
            {
                idReadStatus = IDreadStatus.failed;
            }
            #endregion

            #endregion
            #region 卡匣重複檢查

            CassetteData duCarrierID = cassette_dataBLL.loadCassetteDataByCSTID(readData.CSTID);
            CassetteData duBoxID = cassette_dataBLL.loadCassetteDataByBoxID(readData.BOXID);

            if ((duCarrierID != null && string.IsNullOrWhiteSpace(readData.CSTID) == false) || duBoxID != null)
            {
                idReadStatus = IDreadStatus.duplicate;
            }

            #endregion

            readData.ReadStatus = ((int)idReadStatus).ToString();

            return readData;
        }
        #region 異常流程
        public void HaveAccountHaveReal(CassetteData dbData, CassetteData bcrcsid, ACMD_MCS.IDreadStatus idRead)      //有帳有料
        {
            try
            {
                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "CarrierAbnormal: 有帳有料");
                CassetteData newData = new CassetteData();

                newData = bcrcsid.Clone();
                //newData.ReadStatus = ((int)ACMD_MCS.IDreadStatus.successful).ToString();

                if (idRead == ACMD_MCS.IDreadStatus.duplicate)
                {
                    //newData.CSTID = CarrierReadFail(newData.Carrier_LOC.Trim());
                    OHBC_InsertCassette(newData.CSTID, newData.BOXID, newData.Carrier_LOC);
                    //Duplicate(bcrcsid);
                }
                else if (idRead == ACMD_MCS.IDreadStatus.mismatch
                    || idRead == ACMD_MCS.IDreadStatus.failed
                    || idRead == ACMD_MCS.IDreadStatus.BoxReadFail_CstIsOK
                    || idRead == ACMD_MCS.IDreadStatus.CSTReadFail_BoxIsOK
                        )
                {
                    //isCreatScuess &= cassette_dataBLL.DeleteCSTbyBoxId(dbData.BOXID);
                    if (idRead == ACMD_MCS.IDreadStatus.CSTReadFail_BoxIsOK)
                    {
                        newData.CSTID = CarrierReadFail(newData.Carrier_LOC.Trim());
                    }

                    if (idRead == ACMD_MCS.IDreadStatus.BoxReadFail_CstIsOK)
                    {
                        newData.BOXID = CarrierReadFail(newData.Carrier_LOC.Trim());
                        newData.CSTID = newData.BOXID;
                    }

                    if (idRead == ACMD_MCS.IDreadStatus.mismatch)
                    {
                        newData.BOXID = bcrcsid.BOXID;
                        newData.CSTID = CarrierReadFail(newData.Carrier_LOC.Trim());
                    }

                    if (newData.BOXID.Contains("UNKF") && isUnitType(dbData.Carrier_LOC, UnitType.CRANE)
                      )
                    {
                        cassette_dataBLL.DeleteCSTbyCstBoxID(dbData.CSTID, dbData.BOXID);
                    }
                    else
                    {
                        reportBLL.ReportCarrierRemovedCompleted(dbData.CSTID, dbData.BOXID);
                    }

                    OHBC_InsertCassette(newData.CSTID, newData.BOXID, newData.Carrier_LOC);
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "HaveAccountHaveReal");
                //return null;
            }
        }
        public void NotAccountHaveRead(CassetteData bcrcsid)    //無帳有料
        {
            TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "CarrierAbnormal: 無帳有料");
            OHBC_InsertCassette(bcrcsid.CSTID, bcrcsid.BOXID, bcrcsid.Carrier_LOC);
        }
        public void HaveAccountNotReal(CassetteData dbData)     //有帳無料
        {
            try
            {
                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "CarrierAbnormal: 有帳無料");
                CassetteData cstData = dbData.Clone();
                reportBLL.ReportCarrierRemovedCompleted(dbData.CSTID, dbData.BOXID);

                if (shelfDefBLL.isExist(cstData.Carrier_LOC))
                {
                    reportBLL.ReportZoneCapacityChange(cstData.Carrier_LOC, null);
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "HaveAccountNotReal");
            }
        }
        public void Duplicate(CassetteData bcrData) //卡匣重複處理
        {
            CassetteData newCstData = new CassetteData();
            CassetteData duBoxID = cassette_dataBLL.loadCassetteDataByBoxID(bcrData.BOXID);
            CassetteData duCarrID = cassette_dataBLL.loadCassetteDataByCSTID(bcrData.CSTID);

            if (duBoxID != null && duBoxID.Carrier_LOC != bcrData.Carrier_LOC)    //BOXID 重複
            {
                newCstData = duBoxID.Clone();
                newCstData.BOXID = CarrierReadduplicate(duBoxID.BOXID);

                if (duCarrID != null)   //同個BOX，CSTID 重複
                {
                    if (duCarrID.Carrier_LOC == duBoxID.Carrier_LOC)
                    {
                        newCstData.CSTID = CarrierReadduplicate(bcrData.CSTID);
                    }

                    reportBLL.ReportCarrierRemovedCompleted(duBoxID.CSTID, duBoxID.BOXID);
                    OHBC_InsertCassette(newCstData.CSTID, newCstData.BOXID, newCstData.Carrier_LOC);
                    return;
                }

                reportBLL.ReportCarrierRemovedCompleted(duBoxID.CSTID, duBoxID.BOXID);
                OHBC_InsertCassette(newCstData.CSTID, newCstData.BOXID, newCstData.Carrier_LOC);
            }

            if (duCarrID != null && duCarrID.Carrier_LOC != bcrData.Carrier_LOC && string.IsNullOrWhiteSpace(bcrData.CSTID) == false)    //CSTID 重複
            {
                newCstData = duCarrID.Clone();
                newCstData.CSTID = CarrierReadduplicate(bcrData.CSTID);
                reportBLL.ReportCarrierRemovedCompleted(duCarrID.CSTID, duCarrID.BOXID);
                OHBC_InsertCassette(newCstData.CSTID, newCstData.BOXID, newCstData.Carrier_LOC);
            }
        }
        #endregion

        #endregion
        #region 卡匣建帳、刪帳

        public string OHBC_InsertCassette(string cstid, string boxid, string loc, string lotID = "")
        {
            try
            {
                loc = loc.Trim();
                if (portINIData.ContainsKey(loc))
                {
                    if (isUnitType(portINIData[loc].PortName, UnitType.ZONE))
                    {
                        return "Loc 不存在";
                    }
                }
                else
                {
                    return "Loc 不存在";
                }

                CassetteData datainfo = new CassetteData();

                datainfo.StockerID = "1";
                datainfo.CSTID = cstid.Trim();
                datainfo.BOXID = boxid.Trim();
                datainfo.Carrier_LOC = loc.Trim();
                datainfo.LotID = lotID.Trim();
                datainfo.CSTState = E_CSTState.Installed;
                datainfo.CSTInDT = DateTime.Now.ToString("yy/MM/dd HH:mm:ss");
                datainfo.TrnDT = DateTime.Now.ToString("yy/MM/dd HH:mm:ss");
                datainfo.Stage = 1;

                string portName = datainfo.Carrier_LOC;

                //PortDef portData = portDefBLL.GetPortData(datainfo.Carrier_LOC);

                AVEHICLE vehicle = scApp.VehicleBLL.getVehicleByID(datainfo.Carrier_LOC);

                CassetteData portCSTData = cassette_dataBLL.loadCassetteDataByLoc(loc);  //檢查同個位置是否有帳

                if (portCSTData != null)
                {
                    if (datainfo.BOXID.Contains("UNKF") && isUnitType(portCSTData.Carrier_LOC, UnitType.CRANE))
                    {
                        cassette_dataBLL.DeleteCSTbyCstBoxID(portCSTData.CSTID, portCSTData.BOXID);
                    }
                    else
                    {
                        reportBLL.ReportCarrierRemovedCompleted(portCSTData.CSTID, portCSTData.BOXID);
                    }
                }

                if (isLocExist(portName))
                {
                    if (cassette_dataBLL.loadCassetteDataByBoxID(datainfo.BOXID) != null
                     || (cassette_dataBLL.loadCassetteDataByCSTID(datainfo.CSTID) != null && string.IsNullOrWhiteSpace(datainfo.CSTID) == false)
                      )
                    {
                        Duplicate(datainfo);
                    }

                    if (isUnitType(portName, UnitType.SHELF))
                    {
                        datainfo.CSTState = E_CSTState.Completed;
                        cassette_dataBLL.insertCassetteData(datainfo);
                        reportBLL.ReportCarrierInstallCompleted(datainfo);

                        reportBLL.ReportZoneCapacityChange(portName, null);
                    }
                    else if (isUnitType(portName, UnitType.CRANE))
                    {
                        datainfo.CSTState = E_CSTState.Transferring;
                        cassette_dataBLL.insertCassetteData(datainfo);
                        if (datainfo.BOXID.Contains("UNKF"))
                        {
                            reportBLL.ReportCarrierBoxIDRename(datainfo.CSTID, datainfo.BOXID, datainfo.Carrier_LOC);
                        }
                        else
                        {
                            reportBLL.ReportCarrierInstallCompleted(datainfo);
                        }
                    }
                    //else if (isUnitType(portName, UnitType.AGV))
                    //{
                    //    datainfo.CSTState = E_CSTState.WaitIn;
                    //    cassette_dataBLL.insertCassetteData(datainfo);

                    //    reportBLL.ReportCarrierInstallCompleted(datainfo);

                    //}
                    else
                    {
                        datainfo.CSTState = E_CSTState.Installed;
                        cassette_dataBLL.insertCassetteData(datainfo);
                        reportBLL.ReportCarrierInstallCompleted(datainfo);
                    }
                }
                else if (vehicle != null)
                {
                    if (cassette_dataBLL.loadCassetteDataByBoxID(datainfo.BOXID) != null
                           || cassette_dataBLL.loadCassetteDataByBoxID(datainfo.CSTID) != null
                         )
                    {
                        Duplicate(datainfo);
                    }

                    datainfo.CSTState = E_CSTState.Transferring;
                    cassette_dataBLL.insertCassetteData(datainfo);

                    if (datainfo.BOXID.Contains("UNKF"))
                    {
                        reportBLL.ReportCarrierBoxIDRename(datainfo.CSTID, datainfo.BOXID, datainfo.Carrier_LOC);
                    }
                    else
                    {
                        reportBLL.ReportCarrierInstallCompleted(datainfo);
                    }

                    scApp.VehicleBLL.updataVehicleBOXID(vehicle.VEHICLE_ID, datainfo.BOXID); //Hsinyu Chang 20200312 AVEHICLE上也有存放box ID，要一起更新
                }

                return "OK";
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "OHBC_InsertCassette");

                return "Manual install Cassette failed.";
            }
        }
        public void DeleteOHCVPortCst(string portName)  //刪除 OHCV Port 上的所有卡匣
        {
            List<CassetteData> cstList = cassette_dataBLL.LoadCassetteDataByOHCV(portName);

            if (cstList.Count != 0)
            {
                foreach (CassetteData cstData in cstList)
                {
                    DeleteCst(cstData.CSTID, cstData.BOXID, "DeleteOHCVPortCst");
                }
            }
        }
        public string DeleteCst(string cstID, string boxID, string cmdSource)
        {
            TransferServiceLogger.Info
            (
                DateTime.Now.ToString("HH:mm:ss.fff ") +
                "OHB >> DB|DeleteCst：cstID:" + cstID + "    boxID:" + boxID + "  誰呼叫:" + cmdSource
            );

            ACMD_MCS cmdData = cmdBLL.getByCstBoxID(cstID, boxID);

            if (cmdData != null)
            {
                if (cmdData.TRANSFERSTATE != E_TRAN_STATUS.Transferring)
                {
                    cmdBLL.updateCMD_MCS_TranStatus(cmdData.CMD_ID, E_TRAN_STATUS.TransferCompleted);
                }
                else
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "OHB >> DB|DeleteCst:有命令正在使用此卡匣"
                    );
                    return "有命令正在使用此卡匣";
                }
            }

            if (reportBLL.ReportCarrierRemovedCompleted(cstID, boxID))
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHB >> DB|Manual_DeleteCst:刪帳成功"
                );
                return "OK";
            }
            else
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "Manual >> OHB|Manual_DeleteCst:刪帳失敗"
                );
                return "失敗";
            }
        }

        #endregion

        #endregion

        #endregion
        #region 空 BOX 處理
        //20200525 Hsinyu Chang 帶入目的port，選出離目的最近的空box
        public CassetteData GetNearestEmptyBox(string portID)
        {
            try
            {
                var portData = scApp.PortDefBLL.GetPortData(portID);

                //依離目的port距離，升冪排序
                var dbCstData = cassette_dataBLL.loadCassetteData()
                    .Where(data => data.CSTID == ""
                            && isUnitType(data.Carrier_LOC, UnitType.SHELF)
                            && cmdBLL.GetCmdDataBySource(data.Carrier_LOC) == null
                    ).OrderBy(cst => scApp.ShelfDefBLL.GetDistance(cst.Carrier_LOC, portData.ADR_ID))
                    .ToList();

                if (dbCstData == null)
                {
                    return null;
                }

                #region Log
                string log = "";
                foreach (var v in dbCstData)
                {
                    log = log + v.Carrier_LOC + "， ";
                }

                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHB >> DB|對 " + portID + " 做空 BOX 排序:" + log
                );
                #endregion

                return dbCstData.FirstOrDefault();
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "GetNearestEmptyBox");
                CassetteData emptyBox = GetTotalEmptyBoxNumber().emptyBox.FirstOrDefault();
                return emptyBox;
            }
        }
        public string GetShelfRecentLocation(List<ShelfDef> shelfData, string portLoc)  //取得最近儲位
        {
            string shelfName = "";
            //A20.06.09.0
            shelfData = cmdBLL.doSortShelfDataByDistanceFromHostSource(shelfData, portLoc.Trim())
                                                            .Where(data => data.ShelfState == ShelfDef.E_ShelfState.EmptyShelf).ToList();

            foreach (var v in shelfData)
            {
                ACMD_MCS cmdData = cmdBLL.GetCmdDataByDest(v.ShelfID).FirstOrDefault();
                if (cmdData == null) //cmdList.Count == 0
                {
                    shelfName = v.ShelfID;
                    break;
                }
                else
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ")
                        + "OHB >> OHB|GetShelfRecentLocation 已有命令搬到此 " + v.ShelfID + " 儲位 " + "\n"
                        + " CMDID: " + cmdData.CMD_ID + " 來源:" + cmdData.HOSTSOURCE + " 目的:" + cmdData.HOSTDESTINATION
                        + " CSTID:" + cmdData.CARRIER_ID + " BOXID:" + cmdData.BOX_ID
                    );
                }
            }

            if (string.IsNullOrWhiteSpace(shelfName))
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "OHB >> OHB|GetShelfRecentLocation 沒有儲位可以用"
                );
            }

            return shelfName;
        }
        #endregion
        #region PLC 控制命令

        public bool SetPortTypeCmd(string portName, E_PortType type)    //新增控制流向命令
        {
            try
            {
                portName = portName.Trim();

                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHB >> OHB|SetPortTypeCmd 新增切流向命令 portID:" + portName + "    inout:" + type
                );

                ACMD_MCS datainfo = new ACMD_MCS();

                datainfo.CMD_ID = "PortTypeChange-" + portName + ">>" + type;
                datainfo.CARRIER_ID = "";
                datainfo.BOX_ID = "";

                datainfo.HOSTSOURCE = portName;
                datainfo.HOSTDESTINATION = type.ToString();

                datainfo.CMDTYPE = CmdType.PortTypeChange.ToString();

                if (cmdBLL.getNowCMD_MCSByID(datainfo.CMD_ID) != null)
                {
                    return false;
                }

                datainfo.LOT_ID = "";
                datainfo.CMD_INSER_TIME = DateTime.Now;
                datainfo.TRANSFERSTATE = E_TRAN_STATUS.Queue;
                datainfo.COMMANDSTATE = ACMD_MCS.COMMAND_iIdle;
                datainfo.PRIORITY = 50;
                datainfo.CHECKCODE = "";
                datainfo.PAUSEFLAG = "";
                datainfo.TIME_PRIORITY = 0;
                datainfo.PORT_PRIORITY = 0;
                datainfo.REPLACE = 1;
                datainfo.PRIORITY_SUM = datainfo.PRIORITY + datainfo.TIME_PRIORITY + datainfo.PORT_PRIORITY;
                datainfo.CRANE = "";

                if (cmdBLL.getCMD_MCSByID(datainfo.CMD_ID) == null)
                {
                    cmdBLL.creatCommand_MCS(datainfo);
                    //cmdBLL.DeleteCmd(datainfo.CMD_ID);
                }

                return true;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "SetPortTypeCmd");
                return false;
            }
        }

        public string OpenAGV_Station(string portName, bool open)
        {
            portName = portName.Trim();
            portINIData[portName].openAGV_Station = open;

            return GetAGV_StationStatus(portName);
        }

        public string OpenAGV_AutoPortType(string portName, bool open)
        {
            portName = portName.Trim();
            portINIData[portName].openAGV_AutoPortType = open;

            return GetAGV_AutoPortType(portName);
        }

        public string GetAGV_StationStatus(string portName)
        {
            portName = portName.Trim();
            return portINIData[portName].openAGV_Station.ToString();
        }
        public string GetAGV_AutoPortType(string portName)
        {
            portName = portName.Trim();
            return portINIData[portName].openAGV_AutoPortType.ToString();
        }
        public string GetCVPortHelp(string portName)   //取得狀態說明
        {
            PortPLCInfo plcInof = GetPLC_PortData(portName);
            string log = "狀態：\n";
            if (isUnitType(plcInof.EQ_ID, UnitType.AGV))
            {
                if (plcInof.IsReadyToLoad == false && plcInof.IsReadyToUnload == false)
                {
                    log = log + "IsReadyToLoad 與 IsReadyToLoad 為 False";
                }

                if (plcInof.IsInputMode)
                {

                }

                if (plcInof.IsOutputMode)
                {

                }
            }
            else
            {
                if (plcInof.OpAutoMode)
                {
                    log = log + E_PORT_STATUS.InService.ToString();
                }
                else
                {
                    log = log + E_PORT_STATUS.OutOfService.ToString();
                }
            }
            return log;
        }
        #endregion
        #region 資料判斷
        public bool isUnitType(string portName, UnitType unitType)  //Port種類判斷
        {
            try
            {
                if (portINIData != null)
                {
                    if (unitType.ToString().Trim() == portINIData[portName.Trim()].UnitType)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                string st_details = "";

                try
                {
                    System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
                    st_details = st.ToString();
                }
                catch (Exception eex)
                {
                    TransferServiceLogger.Error(eex);
                }

                TransferServiceLogger.Error(ex, "isUnitType    portName:" + portName + "  unitType:" + unitType + "\n" + " 推疊：" + st_details);
                return false;
            }
        }
        public bool isCVPort(string portName)
        {
            try
            {
                portName = portName.Trim();
                if (portINIData[portName].UnitType == UnitType.OHCV.ToString()
                 || portINIData[portName].UnitType == UnitType.NTB.ToString()
                 || portINIData[portName].UnitType == UnitType.AGV.ToString()
                 || portINIData[portName].UnitType == UnitType.STK.ToString()
                   )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "isCVPort    portName:" + portName);
                return false;
            }
        }
        public bool isShelfPort(string portName)
        {
            return isUnitType(portName, UnitType.SHELF);
        }
        public bool isLocExist(string portName) //Loc 是否存在
        {
            try
            {
                portName = portName.Trim();
                if (portINIData.ContainsKey(portName))
                {
                    return true;
                }
                else
                {
                    TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "isLocExist portName:" + portName + " 不存在");
                    return false;
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "isLocExist    portName:" + portName);
                return false;
            }
        }
        public bool isAGVZone(string portName)
        {
            return isUnitType(portName, UnitType.AGVZONE);
        }
        #endregion
        #region Log
        public string GetCmdLog(ACMD_MCS cmdData)
        {
            try
            {
                string log = "  cmdData  CMD_ID:" + cmdData?.CMD_ID.Trim() ?? "";
                log = log + "    來源:" + cmdData?.HOSTSOURCE.Trim() ?? "";
                log = log + "    目的:" + cmdData?.HOSTDESTINATION.Trim() ?? "";
                log = log + "    中繼站:" + cmdData?.RelayStation?.Trim() ?? "";
                log = log + "    CST_ID:" + cmdData?.CARRIER_ID.Trim() ?? "";
                log = log + "    BOX_ID:" + cmdData?.BOX_ID.Trim() ?? "";
                log = log + "    OHT_BCR_Read:" + cmdData?.CARRIER_ID_ON_CRANE?.Trim() ?? "";
                log = log + "    CMD_TRANSFERSTATE:" + cmdData?.TRANSFERSTATE ?? "";
                log = log + "    CMD_SOURCE:" + cmdData?.CMDTYPE?.Trim() ?? "";
                log = log + "    CRANE:" + cmdData?.CRANE?.Trim() ?? "";
                return log;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "GetCmdLog");
                return "";
            }
        }
        public string GetCstLog(CassetteData cstData)
        {
            try
            {
                string log = "  cstData  CSTID:" + cstData.CSTID?.Trim() ?? "";
                log = log + "   BOXID:" + cstData.BOXID?.Trim() ?? "";
                log = log + "   Carrier_LOC:" + cstData.Carrier_LOC?.Trim() ?? "";
                log = log + "   Stage:" + cstData?.Stage ?? "";
                log = log + "   CSTState:" + cstData?.CSTState ?? "";
                return log;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "GetCstLog");
                return "";
            }
        }
        public string GetOHTcmdLog(ACMD_OHTC ohtCmdData)
        {
            try
            {
                string log = " OHT_CmdID:" + ohtCmdData?.CMD_ID ?? "";
                log = log + " OHT_BOXID:" + ohtCmdData?.BOX_ID ?? "";
                log = log + " 來源:" + ohtCmdData?.SOURCE ?? "";
                log = log + " 目的:" + ohtCmdData?.DESTINATION ?? "";

                return log;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "GetOHTcmdLog");
                return "";
            }
        }
        public string GetPLCInfoLog(PortPLCInfo plcInfo)
        {
            string log = " PLCInfo_PortName:" + plcInfo?.EQ_ID ?? "";
            log = log + " RUN:" + plcInfo?.OpAutoMode ?? "";
            log = log + " DOWN:" + plcInfo?.OpManualMode ?? "";
            log = log + " IsInputMode:" + plcInfo?.IsInputMode ?? "";
            log = log + " IsOutputMode:" + plcInfo?.IsOutputMode ?? "";

            return log;
        }
        public void SetWaitInOutLog(CassetteData cst, E_CSTState type)
        {
            WaitInOutLog log = new WaitInOutLog();
            log.time = DateTime.Now.ToString("yy/MM/dd HH:mm:ss.fff");
            log.CSTID = cst.CSTID;
            log.BOXID = cst.BOXID;
            log.LOC = cst.Carrier_LOC;
            log.type = type;

            if (type == E_CSTState.WaitIn)
            {
                //waitInLog.Add(log.time.ToString(), log);
            }

            if (type == E_CSTState.WaitOut)
            {
                //waitOutLog.Add(log.time.ToString(), log);
            }
        }
        #endregion
        #region 異常處理
        string alarmBug = "";   //防止Alarm 短時間重複上報
        public void OHBC_AlarmSet(string ohtName, string errCode)
        {
            string s = DateTime.Now.ToString() + " " + ohtName + " " + errCode;

            if (alarmBug.Contains(s))
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHB >> OHB|OHBC_AlarmSet 短時間內觸發"
                    + "    ohtName:" + ohtName
                    + "    errCode:" + errCode
                    + "    DateTime.Now:" + s
                );
                return;
            }
            else
            {
                alarmBug = s;
            }

            ohtName = ohtName.Trim();
            errCode = errCode.Trim();

            #region Log
            TransferServiceLogger.Info
            (
                DateTime.Now.ToString("HH:mm:ss.fff ") +
                "OHT >> OHB|AlarmSet:"
                + "    OHT_Name:" + ohtName.Trim()
                + "    OHT_AlarmID:" + errCode
            );

            if (errCode == "0")
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHT >> OHB|errCode = 0 判斷無異常跳回"
                );
                return;
            }
            #endregion

            try
            {
                ACMD_MCS mcsCmdData = cmdBLL.getCMD_ByOHTName(ohtName).FirstOrDefault();

                ALARM alarm = scApp.AlarmBLL.setAlarmReport(null, ohtName, errCode);

                if (alarm != null)
                {
                    if (isUnitType(alarm.EQPT_ID, UnitType.CRANE))
                    {
                        if (alarm.ALAM_LVL == E_ALARM_LVL.Error)
                        {
                            reportBLL.ReportAlarmHappend(ErrorStatus.ErrSet, alarm.ALAM_CODE, alarm.ALAM_DESC);
                            reportBLL.ReportAlarmSet(mcsCmdData, alarm, alarm.UnitID, alarm.UnitState, "ABORT");
                        }
                        else if (alarm.ALAM_LVL == E_ALARM_LVL.Warn)
                        {
                            reportBLL.ReportUnitAlarmSet(alarm.EQPT_ID, alarm.ALAM_CODE, alarm.ALAM_DESC);
                        }
                    }
                    else
                    {
                        reportBLL.ReportUnitAlarmSet(alarm.EQPT_ID, alarm.ALAM_CODE, alarm.ALAM_DESC);
                    }
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "OHT_AlarmSet   ohtName:" + ohtName + " ErrorCode:" + errCode);
            }
        }
        public void OHBC_AlarmCleared(string craneName, string errCode)
        {
            if (scApp.AlarmBLL == null)
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "OHT >> OHB|AlarmBLL = null"
                );
                return;
            }

            craneName = craneName.Trim();
            errCode = errCode.Trim();
            #region Log
            TransferServiceLogger.Info
            (
                DateTime.Now.ToString("HH:mm:ss.fff ") +
                "OHT >> OHB|AlarmCleared:"
                + "    OHT_Name:" + craneName
                + "    OHT_AlarmID:" + errCode
            );
            #endregion

            try
            {
                ACMD_MCS mcsCmdData = cmdBLL.getCMD_ByOHTName(craneName).FirstOrDefault();

                if (mcsCmdData == null)
                {
                    mcsCmdData = new ACMD_MCS();
                    mcsCmdData.CMD_ID = "";
                }

                ALARM alarm = scApp.AlarmBLL.loadAlarmByAlarmID(craneName, errCode);

                if (alarm != null)
                {
                    if (isUnitType(alarm.EQPT_ID, UnitType.CRANE))
                    {
                        if (alarm.ALAM_LVL == E_ALARM_LVL.Error)
                        {
                            reportBLL.ReportAlarmCleared(mcsCmdData, alarm, alarm.UnitID.Trim(), alarm.UnitState.Trim());
                            scApp.ReportBLL.ReportAlarmHappend(ErrorStatus.ErrReset, alarm.ALAM_CODE.Trim(), alarm.ALAM_DESC.Trim());
                        }
                        else if (alarm.ALAM_LVL == E_ALARM_LVL.Warn)
                        {
                            reportBLL.ReportUnitAlarmCleared(alarm.EQPT_ID, alarm.ALAM_CODE, alarm.ALAM_DESC);
                        }
                    }
                    else
                    {
                        reportBLL.ReportUnitAlarmCleared(alarm.EQPT_ID, alarm.ALAM_CODE, alarm.ALAM_DESC);
                    }

                    scApp.AlarmBLL.resetAlarmReport(alarm.EQPT_ID, alarm.ALAM_CODE);
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "OHT_AlarmCleared   ohtName:" + craneName + " ErrorCode:" + errCode);
            }
        }
        public bool OHBC_AlarmAllCleared(string craneName)
        {
            if (scApp.AlarmBLL == null)
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "OHT >> OHB|AlarmBLL = null"
                );
                return true;
            }
            #region Log
            TransferServiceLogger.Info
            (
                DateTime.Now.ToString("HH:mm:ss.fff ") +
                "OHT >> OHB|AlarmAllCleared:"
                + "    OHT_Name:" + craneName.Trim()
            );
            #endregion

            try
            {
                foreach (var v in scApp.AlarmBLL.loadSetAlarmList().Where(data => data.EQPT_ID.Trim() == craneName.Trim() && data.ALAM_STAT == ErrorStatus.ErrSet))
                {
                    OHBC_AlarmCleared(v.EQPT_ID, v.ALAM_CODE);
                }

                if (isUnitType(craneName, UnitType.CRANE))
                {

                }

                return true;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "OHT_AlarmAllCleared");
                return false;
            }
        }
        #endregion

        #endregion

        #region 人員手動操作

        #region OHBC 狀態操作

        public string Manual_OnLineMode()
        {
            if (!scApp.LineService.canOnlineWithHost())
            {
                return "Has vh not ready";
            }
            else if (line.Host_Control_State == SCAppConstants.LineHostControlState.HostControlState.On_Line_Local)
            {
                return "On line Local Ready";
            }
            else
            {
                //Task.Run(() => scApp.LineService.OnlineWithHostOp());
                Task.Run(() => scApp.LineService.OnlineLocalWithHostOp());
                return "OK";
            }
        }
        public string Manual_OnLineRemote()
        {
            if (!scApp.LineService.canOnlineWithHost())
            {
                return "Has vh not ready";
            }
            else if (line.Host_Control_State == SCAppConstants.LineHostControlState.HostControlState.On_Line_Local)
            {
                return "On line Local Ready";
            }
            else
            {
                Task.Run(() => scApp.LineService.OnlineWithHostOp());
                //Task.Run(() => scApp.LineService.OnlineRemote());
                return "OK";
            }
        }
        public string Manual_OFFLineMode()
        {
            if (scApp.getEQObjCacheManager().getLine().SCStats != ALINE.TSCState.PAUSED)
            {
                return "Please change tsc state to pause first.";
            }
            else if (line.Host_Control_State == SCAppConstants.LineHostControlState.HostControlState.EQ_Off_line)
            {
                return "Current is off line";
            }
            else
            {
                Task.Run(() => scApp.LineService.OfflineWithHostByOp());
                return "OK";
            }
        }
        public string Manual_AutoMode()
        {
            Task.Run(() => scApp.getEQObjCacheManager().getLine().ResumeToAuto(scApp.ReportBLL));
            return "OK";
        }
        public string Manual_PauseMode()
        {
            Task.Run(() => scApp.LineService.TSCStateToPause(sc.Data.SECS.CSOT.SECSConst.PAUSE_REASON_OP));
            return "OK";
        }

        #endregion
        #region 命令操作
        public string Manual_InsertCmd(string source, string dest, string lot_id = "", int priority = 5, string sourceCmd = "UI", CmdType cmdType = CmdType.Manual)   //手動搬送，sourceCmd : 誰呼叫
        {
            try
            {
                #region Log
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHB >> CMD|Manual_InsertCmd"
                    + " 來源" + source
                    + " 目的" + dest
                );
                #endregion

                CassetteData cstData = cassette_dataBLL.loadCassetteDataByLoc(source);

                if (cstData == null)
                {
                    string returnLog = "Cassette is not exist";
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "Manual >> OHB|Manual_InsertCmd " + returnLog
                    );

                    return returnLog;
                }

                #region 新增 MCS 命令
                bool cmdExist = true;
                int cmdNo = 1;

                ACMD_MCS datainfo = new ACMD_MCS();

                string cmdID = "MANAUL" + GetStDate();

                while (cmdExist)
                {
                    if (cmdBLL.getCMD_MCSByID(cmdID + cmdNo) == null)
                    {
                        datainfo.CMD_ID = cmdID + cmdNo;
                        cmdExist = false;
                    }
                    else
                    {
                        cmdNo++;
                    }
                }

                datainfo.CARRIER_ID = cstData.CSTID;
                datainfo.BOX_ID = cstData.BOXID;
                datainfo.CMD_INSER_TIME = DateTime.Now;
                datainfo.TRANSFERSTATE = E_TRAN_STATUS.Queue;
                datainfo.COMMANDSTATE = ACMD_MCS.COMMAND_iIdle;
                datainfo.PRIORITY = priority;
                datainfo.HOSTSOURCE = source;
                datainfo.HOSTDESTINATION = dest;
                datainfo.CHECKCODE = "";
                datainfo.PAUSEFLAG = "";
                datainfo.TIME_PRIORITY = 0;
                datainfo.PORT_PRIORITY = 0;
                datainfo.REPLACE = 1;
                datainfo.PRIORITY_SUM = datainfo.PRIORITY + datainfo.TIME_PRIORITY + datainfo.PORT_PRIORITY;
                datainfo.LOT_ID = cstData.LotID?.Trim() ?? "";
                datainfo.CMDTYPE = cmdType.ToString();
                datainfo.CRANE = "";

                if (cmdBLL.creatCommand_MCS(datainfo))
                {
                    reportBLL.ReportOperatorInitiatedAction(datainfo.CMD_ID, reportMCSCommandType.Transfer.ToString());
                    return "OK";
                }
                else
                {
                    #region Log
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ")
                        + "OHBC_InsertCmd: 命令建立失敗 " + GetCmdLog(datainfo)
                    );
                    #endregion

                    return "命令建立失敗";
                }
                #endregion

            }
            catch
            {
                return "命令建立失敗";
            }
        }
        public string Manual_DeleteCmd(string cmdid, string cmdSource)    //刪除命令
        {
            #region Log
            TransferServiceLogger.Info
            (
                DateTime.Now.ToString("HH:mm:ss.fff ") +
                "Manual >> OHB|Manual_DeleteCmd: " + cmdid + " 誰呼叫：" + cmdSource
            );
            #endregion

            ACMD_MCS cmdData = cmdBLL.getCMD_MCSByID(SCUtility.Trim(cmdid, true));

            if (cmdData != null)
            {
                Task.Run(() =>
                {
                    DeleteCmd(cmdData);
                });

                return "OK";
            }
            else
            {
                #region Log
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "Manual >> OHB|Manual_DeleteCmd: " + cmdid + " 命令不存在"
                );
                #endregion
                return "命令不存在";
            }
        }
        public string LocalCmdCancel(string cmdID, string cmdSource)  //對本機命令強制結束
        {
            ACMD_MCS cmdData = cmdBLL.getCMD_MCSByID(cmdID);

            if (cmdData != null)
            {
                #region Log
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "Manual >> OHB|LocalCmdCancel: " + GetCmdLog(cmdData) + " 誰呼叫：" + cmdSource
                );
                #endregion

                if (cmdData.CMDTYPE == CmdType.PortTypeChange.ToString())
                {
                    cmdBLL.DeleteCmd(cmdData.CMD_ID);
                }

                cmdBLL.updateCMD_MCS_TranStatus(cmdData.CMD_ID, E_TRAN_STATUS.TransferCompleted);
                if (cmdData.COMMANDSTATE < COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE)
                {
                    scApp.ReportBLL.ReportTransferCancelInitial(cmdData.CMD_ID);
                    scApp.ReportBLL.ReportTransferCancelCompleted(cmdData.CMD_ID);
                }
                else
                {
                    scApp.ReportBLL.ReportTransferAbortInitiated(cmdData.CMD_ID);
                    scApp.ReportBLL.ReportTransferAbortCompleted(cmdData.CMD_ID);
                }
            }

            return "OK";
        }
        #endregion
        #region 卡匣操作
        public string Manual_InsertCassette(string cstid, string boxid, string loc, string lotID = "")  //手動建帳
        {
            if (string.IsNullOrWhiteSpace(cstid) == false)
            {
                if (boxid.Length != 8)
                {
                    return "CST_ID 不足 8 碼";
                }
                else
                {
                    if (ase_ID_Check(cstid) == false)
                    {
                        return "CST_ID 或 BOX_ID，不符合 1、2碼為數字，3、4碼為英文，5~8碼為數字+英文混合";
                    }
                }
            }

            if (boxid.Length != 8)
            {
                return "BOX_ID 不足 8 碼";
            }
            else
            {
                if (ase_ID_Check(boxid) == false)
                {
                    return "BOX_ID，不符合 1、2碼為數字，3、4碼為英文，5~8碼為數字+英文混合";
                }
            }

            CassetteData duCSTID = cassette_dataBLL.loadCassetteDataByCSTID(cstid);

            if (duCSTID != null && string.IsNullOrWhiteSpace(duCSTID.CSTID) == false)
            {
                return "CSTID 重複，位置在: " + duCSTID.Carrier_LOC + " CSTID:" + duCSTID.CSTID + " BOXID:" + duCSTID.BOXID;
            }

            CassetteData duBOXID = cassette_dataBLL.loadCassetteDataByBoxID(boxid);

            if (duBOXID != null)
            {
                return "BOXID 重複，位置在: " + duBOXID.Carrier_LOC + " CSTID:" + duBOXID.CSTID + " BOXID:" + duBOXID.BOXID; ;
            }

            TransferServiceLogger.Info
            (
                DateTime.Now.ToString("HH:mm:ss.fff ") +
                "OHB >> OHB|Manual_InsertCassette CSTID: " + cstid + "  BOXID:" + boxid + "   LOC:" + loc + "   LOTID:" + lotID
            );

            return OHBC_InsertCassette(cstid, boxid, loc, lotID);
        }
        public string Manual_DeleteCst(string cstID, string boxID)
        {
            return DeleteCst(cstID, boxID, "UI");
        }
        #endregion
        #region Port 狀態設置

        public string Manual_PortTypeChange(string portName, E_PortType type)
        {
            PortTypeChange(portName, type, "UI");
            return "OK";
        }
        public string Manual_SetPortStatus(string portName, E_PORT_STATUS service)
        {
            //PLC_ReportPortInOutService(portName, service);
            PortInOutService(portName, service);
            return "OK";
        }
        public string Manual_SetPortPriority(string portName, int priority)
        {
            portDefBLL.updatePriority(portName, priority);
            return "OK";
        }
        public bool doUpdateTimeOutForAutoUD(string port_id, int timeOutForAutoUD)  //更新Port TimeOut 時間
        {
            bool isSuccess = false;

            isSuccess = portDefBLL.doUpdateTimeOutForAutoUD(port_id, timeOutForAutoUD);

            if (isSuccess)
            {
                portINIData[port_id].timeOutForAutoUD = timeOutForAutoUD;
            }

            return isSuccess;
        }
        public bool doUpdateTimeOutForAutoInZone(string port_id, string timeOutForAutoInZone)   //更新Port TimeOut 後，要搬到哪個Zone
        {
            bool isSuccess = false;
            isSuccess = portDefBLL.doUpdateTimeOutForAutoInZone(port_id, timeOutForAutoInZone);

            if (isSuccess)
            {
                portINIData[port_id].timeOutForAutoInZone = timeOutForAutoInZone;
            }

            return isSuccess;
        }
        public void UpdateIgnoreModeChange(string portName, string enable)
        {
            if (isCVPort(portName))
            {
                portINIData[portName].IgnoreModeChange = enable;

                if (enable == "Y")
                {
                    PortInOutService(portName, E_PORT_STATUS.OutOfService);
                }
                else
                {
                    iniPortData(portName);
                }

                portDefBLL.UpdateIgnoreModeChange(portName, enable);
            }
        }

        #endregion
        #region 儲位操作
        public string Manual_ShelfEnable(string shelfID, bool enable)
        {
            try
            {
                ShelfDef shelf = shelfDefBLL.loadShelfDataByID(shelfID);
                shelfDefBLL.UpdateEnableByID(shelfID, enable);
                ZoneDef zone = zoneBLL.loadZoneDataByID(shelf.ZoneID);
                reportBLL.ReportShelfStatusChange(zone);
                return "OK";
            }
            catch
            {
                return "失敗";
            }
        }

        public string Manual_SetShelfPriority(string shelfID, int priority)
        {
            if (shelfDefBLL.updatePriority(shelfID, priority))
            {
                return "OK";
            }
            else
            {
                return "失敗";
            }
        }

        public void ShelfReserved(string source, string dest)   //預約儲位
        {
            if (isUnitType(source, UnitType.SHELF))
            {
                shelfDefBLL.updateStatus(source, ShelfDef.E_ShelfState.RetrievalReserved);
            }

            if (isUnitType(dest, UnitType.SHELF))
            {
                shelfDefBLL.updateStatus(dest, ShelfDef.E_ShelfState.StorageInReserved);
            }
        }

        #endregion

        #endregion

        #region Port操作

        #region Port 狀態取得

        public void GetPortPositionCstExist(string portName)
        {
            portName = portName.Trim();
            int stage = portINIData[portName].Stage;

            PortPLCInfo plcInfo = GetPLC_PortData(portName);
            CassetteData cstData = null;

            if (plcInfo.IsInputMode)
            {
                if (plcInfo.LoadPosition1)
                {
                    cstData = new CassetteData();
                    cstData.BOXID = plcInfo.LoadPositionBOX1;
                }
            }

            if (plcInfo.IsOutputMode)
            {
                switch (stage)
                {
                    case 1:
                        if (plcInfo.LoadPosition1)
                        {
                            cstData = new CassetteData();
                            cstData.BOXID = plcInfo.LoadPositionBOX1;
                        }
                        break;
                    case 2:
                        if (plcInfo.LoadPosition2)
                        {
                            cstData = new CassetteData();
                            cstData.BOXID = plcInfo.LoadPositionBOX2;
                        }
                        break;
                    case 3:
                        if (plcInfo.LoadPosition3)
                        {
                            cstData = new CassetteData();
                            cstData.BOXID = plcInfo.LoadPositionBOX3;
                        }
                        break;
                    case 4:
                        if (plcInfo.LoadPosition4)
                        {
                            cstData = new CassetteData();
                            cstData.BOXID = plcInfo.LoadPositionBOX4;
                        }
                        break;
                    case 5:
                        if (plcInfo.LoadPosition5)
                        {
                            cstData = new CassetteData();
                            cstData.BOXID = plcInfo.LoadPositionBOX5;
                        }
                        break;
                    case 6:
                        if (plcInfo.LoadPosition6)
                        {
                            cstData = new CassetteData();
                            cstData.BOXID = "ERROR1";
                        }
                        break;
                    case 7:
                        if (plcInfo.LoadPosition7)
                        {
                            cstData = new CassetteData();
                            cstData.BOXID = "ERROR1";
                        }
                        break;
                    default:

                        break;
                }
            }

        }
        public bool GetIgnoreModeChange(PortPLCInfo portPLCInfo)
        {
            try
            {
                if (iniStatus == false)
                {
                    return true;
                }

                string portName = portPLCInfo.EQ_ID.Trim();

                if (portINIData[portName].IgnoreModeChange == "Y")
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") + portName + "   IgnoreModeChange 為   " + portINIData[portName].IgnoreModeChange
                    );

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "GetIgnoreModeChange");
                return false;
            }
        }

        #endregion
        public string GetAGV_InModeInServicePortName(string agvZone) //取得AGV ZONE 狀態為 InMode 且上面有空 BOX 的 AGV Port 名稱
        {
            string agvPortName = "";
            List<PortINIData> agvZoneData = portINIData.Values.Where(data => data.ZoneName == agvZone
                                                                            && data.UnitType == UnitType.AGV.ToString()).ToList();

            if (agvZoneData.Count() != 0)
            {
                foreach (PortINIData agvPortData in agvZoneData)
                {
                    PortPLCInfo agvInfo = GetPLC_PortData(agvPortData.PortName);
                    if (agvInfo.IsInputMode
                        && agvInfo.IsReadyToUnload
                        && agvInfo.OpAutoMode
                        && agvInfo.LoadPosition1
                      )
                    {
                        agvPortName = agvPortData.PortName;
                    }
                }
            }

            return agvPortName;
        }

        public string GetAGV_OutModeInServicePortName(string agvZone) //取得AGV ZONE 狀態為 OutMode 且上面沒有空 BOX 的 AGV Port 名稱
        {
            string agvPortName = "";
            List<PortINIData> agvZoneData = portINIData.Values.Where(data => data.ZoneName == agvZone
                                                                            && data.UnitType == UnitType.AGV.ToString()).ToList();

            if (agvZoneData.Count() != 0)
            {
                foreach (PortINIData agvPortData in agvZoneData)
                {
                    PortPLCInfo agvInfo = GetPLC_PortData(agvPortData.PortName);
                    if (agvInfo.IsOutputMode
                        && agvInfo.IsReadyToLoad
                        && agvInfo.OpAutoMode
                        && agvInfo.LoadPosition1 == false
                      )
                    {
                        agvPortName = agvPortData.PortName;
                    }
                }
            }

            return agvPortName;
        }

        public string GetSTKorOHCV_OutModePortName()    //200617 SCC+ ，心愉要找OutMode的Port，優先順序為 STK、OHCV，要做水位滿了要退出去 
        {
            string portName = "";
            List<PortINIData> stkPortINIDataList = portINIData.Values.Where(data => data.UnitType == UnitType.STK.ToString()).ToList();

            foreach (PortINIData stkData in stkPortINIDataList)  //先找STK
            {
                PortPLCInfo portPLCInfo = GetPLC_PortData(stkData.PortName);
                if (portPLCInfo.OpAutoMode && portPLCInfo.IsOutputMode)
                {
                    portName = stkData.PortName;
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(portName)) //STK 找不到再找 OHCV
            {
                List<PortINIData> ohcvPortINIDataList = portINIData.Values.Where(data => data.UnitType == UnitType.OHCV.ToString()).ToList();

                foreach (PortINIData ohcvData in ohcvPortINIDataList)
                {
                    PortPLCInfo portPLCInfo = GetPLC_PortData(ohcvData.PortName);
                    if (portPLCInfo.OpAutoMode && portPLCInfo.IsOutputMode)
                    {
                        portName = ohcvData.PortName;
                        break;
                    }
                }
            }

            return portName;
        }
        #endregion

        #region Use for check the empty box number and transport for empty box
        public void CheckTheEmptyBoxStockLevel()
        {
            List<ShelfDef> shelfData = scApp.ShelfDefBLL.LoadShelf();
            double highStockLevel = 0.8;
            double emergencyStockLevel = 0.95;
            //A20.05.28.0
            //先確認目前的line 上shelf的空box 是否夠用(目前標準為AGV station 數量)
            var emptyBox = GetTotalEmptyBoxNumber();
            if (emptyBox.isSuccess == true)
            {
                var isEnoughEmptyBox = CheckIsEnoughEmptyBox(emptyBox.emptyBox.Count);
                if (isEnoughEmptyBox.isSuccess == true)
                {
                    //A20.05.28.0
                    //夠用，則確認目前總水位是否過高，若過高則退掉多餘Empty Box到 CV上。
                    if (isEnoughEmptyBox.isEnough == true)
                    {
                        var isStockLevelOfShelfTooHigh = CheckIsStockLevelOfShelfTooHigh(shelfData, highStockLevel);
                        if (isStockLevelOfShelfTooHigh.isSuccess == true)
                        {
                            //A20.05.28.0
                            //水位過高
                            //A20.05.29.0
                            //確認水位是否已達緊急水位
                            if (isStockLevelOfShelfTooHigh.isTooHigh == true)
                            {
                                var isStockLevelOfShelfEmergency = CheckIsStockLevelOfShelfTooHigh(shelfData, emergencyStockLevel);
                                if (isStockLevelOfShelfEmergency.isSuccess)
                                {
                                    if (isStockLevelOfShelfEmergency.isTooHigh == true)
                                    {
                                        DoSendPopEmptyBoxToMCS();
                                    }
                                    else
                                    {
                                        if (CheckMCSCmdCanProcess())
                                        {

                                        }
                                        else
                                        {
                                            DoSendPopEmptyBoxToMCS();
                                        }
                                    }
                                }

                            }
                            //A20.05.28.0
                            //水位沒過高，若目前無命令可執行，則對線內Empty Box 進行不同Zone的調整配置。(與AGV Station 量成比例去配置空Box)
                            else
                            {
                                if (CheckMCSCmdCanProcess())
                                {

                                }
                                else
                                {
                                    DoCheckRegulateEmptyBox();
                                }
                            }
                        }
                    }
                    //A20.05.28.0
                    //不夠，則呼叫MCS補空box動作。(若能給指定zoneID 則指定)
                    else
                    {
                        DoSendRequireEmptyBoxToMCS(isEnoughEmptyBox.requestNumber, emptyBox.emptyBox);
                    }
                }
            }
        }
        //*******************
        //A20.05.28.0 取得目前空BOX數量 並同時回復是否執行成功
        private (List<CassetteData> emptyBox, bool isSuccess) GetTotalEmptyBoxNumber()
        {
            List<CassetteData> emptyBox_ = new List<CassetteData>();
            bool isSuccess_ = false;
            try
            {
                emptyBox_ = cassette_dataBLL.loadCassetteData().
                    Where(data => data.CSTID == "" &&
                    isUnitType(data.Carrier_LOC, UnitType.SHELF) &&
                    cmdBLL.GetCmdDataBySource(data.Carrier_LOC) == null
                    ).ToList();
                if (emptyBox_ != null)
                {
                    isSuccess_ = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                TransferServiceLogger.Error(ex, "GetTotalEmptyBoxNumber");
            }
            return (emptyBox_, isSuccess_);
        }

        //*******************
        //A20.05.28.0 判斷目前的空BOX數量是否滿足需求數量(目前需求數量是用AGV Station 數量判斷)
        private (bool isEnough, int requestNumber, bool isSuccess) CheckIsEnoughEmptyBox(int emptyBoxNumber)
        {
            bool isSuccess_ = false;
            bool isEnough_ = false;
            int requestNumber_ = 0;
            try
            {
                List<PortDef> AGV_station = scApp.PortDefBLL.getAGVPortData();
                int neccessaryEmptyBoxNumber = AGV_station.Count();
                if (neccessaryEmptyBoxNumber <= emptyBoxNumber)
                {
                    isEnough_ = true;
                }
                else
                {
                    requestNumber_ = neccessaryEmptyBoxNumber - emptyBoxNumber;
                }
                isSuccess_ = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                TransferServiceLogger.Error(ex, "CheckIsEnoughEmptyBox");
            }
            return (isEnough_, requestNumber_, isSuccess_);
        }

        //*******************
        //A20.05.28.0 確認目前水位是否過高 可設定數值
        private (bool isTooHigh, bool isSuccess) CheckIsStockLevelOfShelfTooHigh(List<ShelfDef> originShelfData, double highStockLevel)
        {
            double highStockLevel_ = highStockLevel;
            bool isSuccess_ = false;
            bool isTooHigh_ = false;
            try
            {
                List<ShelfDef> shelfData = new List<ShelfDef>(originShelfData);
                int emptyAndEnableShelf = shelfData.Where(data => data.Enable == "Y" && data.ShelfState == "N").Count();
                int enableShelf = shelfData.Where(data => data.Enable == "Y").Count();
                double stockLevel = 1 - (emptyAndEnableShelf / enableShelf);
                if (stockLevel >= highStockLevel_)
                {
                    isTooHigh_ = true;
                }
                isSuccess_ = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                TransferServiceLogger.Error(ex, "CheckIsEnoughEmptyBox");
            }
            return (isTooHigh_, isSuccess_);
        }

        //*******************
        //A20.05.29.0 確認目前是否有可執行的MCS cmd，在目前Queue 中是否有接著可執行的命令。
        private bool CheckMCSCmdCanProcess()
        {
            bool result = false;
            //確認目前MCS cmd狀態。
            //怎樣叫做有可執行的MCS命令?
            List<ACMD_MCS> MCS_Data = cmdBLL.LoadCmdData();
            foreach (ACMD_MCS command in MCS_Data)
            {
                if (AreSourceAndDestEnable(command.HOSTSOURCE, command.HOSTDESTINATION))
                {
                    result = true;
                    return result;
                }
            }
            return result;
        }
        //*******************
        //A20.05.28.0 這邊需要志昌幫忙實作跟MCS"要求"空box的流程。 此部分需要想要如何避免重複要empty box 的流程。
        private void DoSendRequireEmptyBoxToMCS(int requestEmptyBoxNumber, List<CassetteData> emptyBoxes)
        {
            //紀錄log 因此處為實際執行命令之處
            //呼叫MCS需要空box
            //若能指定補到哪個zone 則指定。
            TransferServiceLogger.Info
            (
                DateTime.Now.ToString("HH:mm:ss.fff ") +
                "Manual >> OHB|Manual_InsertCmd"
            );
            //List<ZoneDef> zoneDefs = scApp.ZoneDefBLL.loadZoneData();

            //foreach(CassetteData emptyBox in emptyBoxes)
            //{
            //    if(portINIData[emptyBox.Carrier_LOC.Trim()].ZoneName)
            //}
        }

        //*******************
        //A20.05.28.0 這邊需要志昌幫忙實作跟MCS"退掉"空box的流程。
        private void DoSendPopEmptyBoxToMCS()
        {
            //紀錄log 因此處為實際執行命令之處
            //此部分需先確認目前沒有可執行的MCS命令，才進行要求退BOX動作。
            //Manual_InsertCmd()
        }

        //******************
        //A20.05.28.0 這部分細節還需考慮。
        private void DoCheckRegulateEmptyBox()
        {
            //紀錄log 因此處為實際執行命令之處
            //針對每一靠近zone之AGV station數量 配置對應數量的空BOX
        }
        #endregion

        #region Call by AGVC Restful API. Use for process the AGV Station.
        /// <summary>
        /// Call by AGVC Restful API. Use for process the AGV Station.
        /// </summary>
        /// A20.06.12.0 A20.06.15.0
        /// <param name="AGVStationID"></param>
        /// <param name="AGVCFromEQToStationCmdNum"></param>
        /// <param name="isEmergency"></param>
        /// <returns></returns>
        public bool CanExcuteUnloadTransferAGVStationFromAGVC(string AGVStationID, int AGVCFromEQToStationCmdNum, bool isEmergency)
        {
            bool isOK = false;
            try
            {
                if (scApp.PortDefBLL.GetPortData(AGVStationID).State == E_PORT_STATUS.InService) //此AGVStation虛擬port是 Out of service 一律回復NG
                {
                    return isOK;
                }
                //取得PLC目前資訊
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " Trigger start. Get the AGVSTation Data, " +
                    "AGVStationID = " + AGVStationID + ", AGVCFromEQToStationCmdNum = " + AGVCFromEQToStationCmdNum + ", isEmergency = " + isEmergency.ToString());
                List<PortDef> AGVStationData = scApp.PortDefBLL.GetAGVPortGroupDataByStationID(line.LINE_ID, AGVStationID);
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGVStationID = ");

                //確認取得的AGVStationData中的Port都只有可以用的。
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " Enter the filter for AGV port.");
                List<PortDef> accessAGVPortDatas = FilterOfAGVPort(AGVStationData);
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " Enter the filter for AGV port.");

                //目前先默認取前2個，確認port上Box數量(空與實皆要)
                int emptyBoxNumber, fullBoxNumber;
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " Enter the Count Box number for AGV port.");
                (emptyBoxNumber, fullBoxNumber) = CountAGVStationBoxInfo(accessAGVPortDatas);
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " emptyBoxNumber = " + emptyBoxNumber + ", fullBoxNumber = " + fullBoxNumber);

                //若有實box 則先默認為NG，會稍微影響效率(在一AGV對多個Station時)
                if (fullBoxNumber > 0)
                {
                    //可針對特定細節做特化處理
                    isOK = false;
                }
                //若無實Box 再行判斷空Box 數量。
                switch (emptyBoxNumber)
                {
                    case (int)EmptyBoxNumber.NO_EMPTY_BOX:
                        //若沒有空box，則執行OHBC優先判定。
                        AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " Enter the NO_EMPTY_BOX method");
                        isOK = CheckForChangeAGVPortMode_OHBC(AGVCFromEQToStationCmdNum, accessAGVPortDatas, AGVStationID);
                        break;
                    case (int)EmptyBoxNumber.ONE_EMPTY_BOX:
                        //目前先以執行AGVC優先判定為主，因為若有Cst卡在AGV上並無其餘可去之處。
                        AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " Enter the ONE_EMPTY_BOX method");
                        isOK = CheckForChangeAGVPortMode_AGVC(AGVCFromEQToStationCmdNum, accessAGVPortDatas, AGVStationID);
                        break;
                    case (int)EmptyBoxNumber.TWO_EMPTY_BOX:
                        //若有2空box，則執行AGVC優先判定。
                        AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " Enter the TWO_EMPTY_BOX method");
                        isOK = CheckForChangeAGVPortMode_AGVC(AGVCFromEQToStationCmdNum, accessAGVPortDatas, AGVStationID);
                        break;

                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "CanExcuteUnloadTransferAGVStationFromAGVC");
            }
            return isOK;
        }

        /// <summary>
        /// 確認該AGVport是否可用
        /// </summary>
        /// A20.06.16.0
        /// <param name="AGVStationData"></param>
        private List<PortDef> FilterOfAGVPort(List<PortDef> AGVStationData)
        {
            int count = 0;
            List<PortDef> accessAGVPortDatas = new List<PortDef>();
            foreach (PortDef AGVPortData in AGVStationData)
            {
                //確認可以用的，取前2個加入。
                if (GetPLC_PortData(AGVPortData.PLCPortID).OpAutoMode == true)
                {
                    accessAGVPortDatas.Add(AGVPortData);
                    count = count + 1;
                    if (count >= 2)
                    {
                        break;
                    }
                }
            }
            return accessAGVPortDatas;
        }

        /// <summary>
        /// 優先判斷AGVC是否有命令與其後許處理流程。
        /// </summary>
        /// A20.06.15.0
        /// <param name="AGVCFromEQToStationCmdNum"></param>
        /// <param name="AGVStationData"></param>
        /// <param name="AGVStationID"></param>
        /// <returns></returns>
        private bool CheckForChangeAGVPortMode_AGVC(int AGVCFromEQToStationCmdNum, List<PortDef> AGVStationData, string AGVStationID)
        {
            bool isOK = false;
            try
            {
                if (AGVCFromEQToStationCmdNum > 0)
                {
                    isOK = true;
                    bool isSuccess = InputModeChange(AGVStationData);
                    if (isSuccess == false)
                    {
                        isOK = false;
                    }
                }
                else
                {
                    isOK = false;
                    int OHBCCmdNumber = GetToThisAGVStationMCSCmdNum(AGVStationData, AGVStationID);
                    if (OHBCCmdNumber > 0)
                    {
                        OutputModeChange(AGVStationData);
                    }
                    else
                    {
                        InputModeChange(AGVStationData);
                    }
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "CheckForChangeAGVPortMode");
            }
            return isOK;
        }

        /// <summary>
        /// 優先判斷OHBC是否有命令與其後許處理流程。
        /// </summary>
        /// A20.06.15.0
        /// <param name="AGVCFromEQToStationCmdNum"></param>
        /// <param name="AGVStationData"></param>
        /// <param name="AGVStationID"></param>
        /// <returns></returns>
        private bool CheckForChangeAGVPortMode_OHBC(int AGVCFromEQToStationCmdNum, List<PortDef> AGVStationData, string AGVStationID)
        {
            bool isOK = false;
            try
            {
                int OHBCCmdNumber = GetToThisAGVStationMCSCmdNum(AGVStationData, AGVStationID);
                if (OHBCCmdNumber > 0)
                {
                    isOK = false;
                    OutputModeChange(AGVStationData);
                }
                else
                {
                    if (AGVCFromEQToStationCmdNum > 0)
                    {
                        isOK = true;
                        bool isSuccess = InputModeChange(AGVStationData);
                        if(isSuccess == false) // 若port type 的port mode changeable 為 false 則回false
                        {
                            isOK = false;
                        }
                    }
                    else
                    {
                        isOK = false;
                        OutputModeChange(AGVStationData);
                    }
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "CheckForChangeAGVPortMode");
            }
            return isOK;
        }

        /// <summary>
        /// 計算目前的AGVStation 前2個port 上有多少個空Box 與實Box
        /// </summary>
        /// <param name="AGVStationData">A20.06.15.0 新增</param>
        private (int emptyBoxNumber, int fullBoxNumber) CountAGVStationBoxInfo(List<PortDef> AGVStationData)
        {
            int _emptyBoxNumber = 0;
            int _fullBoxNumber = 0;
            int AGVStationNumber = 0;
            foreach (PortDef AgvPortData in AGVStationData)
            {
                AGVStationNumber = AGVStationNumber + 1;
                if (GetPLC_PortData(AgvPortData.PLCPortID).LoadPosition1 == true && GetPLC_PortData(AgvPortData.PLCPortID).IsCSTPresence == false)
                {
                    _emptyBoxNumber = _emptyBoxNumber + 1;
                }
                else if (GetPLC_PortData(AgvPortData.PLCPortID).LoadPosition1 == true && GetPLC_PortData(AgvPortData.PLCPortID).IsCSTPresence == true)
                {
                    _fullBoxNumber = _fullBoxNumber + 1;
                }
                //先寫死默認取前2個(應該可以改成取前2個可用的，但如何判斷可用)
                if (AGVStationNumber >= 2)
                {
                    break;
                }
            }
            return (_emptyBoxNumber, _fullBoxNumber);
        }

        /// <summary>
        /// 取得目前會到此AGVStation 的OHBC Cmd 數量
        /// </summary>
        /// A20.06.15.0  新增
        /// <param name="AGVStationData"></param>
        /// <param name="AGVStationID"></param>
        private int GetToThisAGVStationMCSCmdNum(List<PortDef> AGVStationData, string AGVStationID)
        {
            //取得目前有多少命令要下至此AGVStation
            int cmdNumber = 0;
            foreach (PortDef AGVPortData in AGVStationData)
            {
                List<ACMD_MCS> cmdData_PortID = cmdBLL.GetCmdDataByDest(AGVPortData.PLCPortID); //A01 A02
                cmdNumber = cmdNumber + cmdData_PortID.Count();
            }
            List<ACMD_MCS> cmdData_StationID = cmdBLL.GetCmdDataByDest(AGVStationID); //ST01
            cmdNumber = cmdNumber + cmdData_StationID.Count();
            return cmdNumber;
        }

        /// <summary>
        /// 切換該目的地Port為InputMode且執行退補空box
        /// </summary>
        /// A20.06.15.0 新增
        /// <param name="AGVPortData"></param>
        private bool InputModeChange(List<PortDef> AGVPortDatas)
        {
            //Todo
            // 需要實作更改該AGVPort為Input 及執行一次退補空box動作
            bool isSuccess = false;
            foreach (PortDef AGVPortData in AGVPortDatas)
            {
                PortPLCInfo portData = GetPLC_PortData(AGVPortData.PLCPortID);
                if (portData.IsModeChangable == false)
                {
                    isSuccess = false;
                    return isSuccess;
                }
                isSuccess = PortTypeChange(AGVPortData.PLCPortID, E_PortType.In, "InputModeChange");
            }
            SpinWait.SpinUntil(() => false, 200);
            Task.Run(() =>
            {
                CyclingCheckReplenishment(AGVPortDatas);
            });
            return isSuccess;
        }

        /// <summary>
        /// 切換該目的地Port為OutputMode且執行退補空box
        /// </summary>
        /// A20.06.15.0  新增
        /// <param name="AGVPortData"></param>
        private bool OutputModeChange(List<PortDef> AGVPortDatas)
        {
            //Todo
            // 需要實作更改該AGVPort為Output 及執行一次退補空box動作
            bool isSuccess = false;
            foreach (PortDef AGVPortData in AGVPortDatas)
            {
                PortPLCInfo portData = GetPLC_PortData(AGVPortData.PLCPortID); 
                if(portData.IsModeChangable == false)
                {
                    isSuccess = false;
                    return isSuccess;
                }
                isSuccess = PortTypeChange(AGVPortData.PLCPortID, E_PortType.Out, "OutputModeChange");
            }
            SpinWait.SpinUntil(() => false, 200);
            Task.Run(() =>
            {
                CyclingCheckWithdraw(AGVPortDatas);
            });
            return isSuccess;
        }

        /// <summary>
        /// 用來重複確認AGV port 狀態，以進行補空盒動作。
        /// </summary>
        /// <param name="AGVPortDatas"></param>
        private void CyclingCheckReplenishment(List<PortDef> AGVPortDatas)
        {
            try
            {
                bool AGVPortReady = false;
                while (AGVPortReady == false)
                {
                    List<PortPLCInfo> portPLCDatas = new List<PortPLCInfo>();
                    //取得目前PLC資料
                    foreach (PortDef AGVPortData in AGVPortDatas)
                    {
                        PortPLCInfo portPLCStatus = GetPLC_PortData(AGVPortData.PLCPortID);
                        portPLCDatas.Add(portPLCStatus);
                    }
                    //以目前資料判斷是否已經轉向完成
                    foreach (PortPLCInfo portPLCStatus in portPLCDatas)
                    {
                        if ((portPLCStatus.IsReadyToLoad == true && portPLCStatus.IsInputMode == true) || //若該port為input mode且 is ready to load 為 true; (可以被補空盒)
                            (portPLCStatus.IsReadyToUnload == true && portPLCStatus.IsInputMode == true)) //或者為input mode 且 is ready to unload 為true;   (上已有盒)
                        {
                            AGVPortReady = true;
                        }
                        else
                        {
                            AGVPortReady = false;
                        }
                    }

                    if (AGVPortReady)
                    {
                        foreach (PortPLCInfo portPLCStatus in portPLCDatas)
                        {
                            //呼叫退補空box 流程。 先將特定port 開啟自動退補，產生完命令後再關閉。
                            portINIData[portPLCStatus.EQ_ID].openAGV_Station = true;
                            PLC_AGV_Station_InMode(portPLCStatus);
                            portINIData[portPLCStatus.EQ_ID].openAGV_Station = false;
                        }
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "CheckForChangeAGVPortMode");
            }
        }

        /// <summary>
        /// 用來重複確認AGV port 狀態，以進行退空盒動作。
        /// </summary>
        /// <param name="AGVPortDatas"></param>
        private void CyclingCheckWithdraw(List<PortDef> AGVPortDatas)
        {
            try
            {
                bool AGVPortReady = false;
                while (AGVPortReady == false)
                {
                    List<PortPLCInfo> portPLCDatas = new List<PortPLCInfo>();
                    //取得目前PLC資料
                    foreach (PortDef AGVPortData in AGVPortDatas)
                    {
                        PortPLCInfo portPLCStatus = GetPLC_PortData(AGVPortData.PLCPortID);
                        portPLCDatas.Add(portPLCStatus);
                    }
                    //以目前資料判斷是否已經轉向完成
                    foreach (PortPLCInfo portPLCStatus in portPLCDatas)
                    {
                        if ((portPLCStatus.IsReadyToLoad == true && portPLCStatus.IsOutputMode == true) || //若該port為input mode且 is ready to load 為 true; (可以被補空盒)
                            (portPLCStatus.IsReadyToUnload == true && portPLCStatus.IsOutputMode == true)) //或者為input mode 且 is ready to unload 為true;   (上已有盒)
                        {
                            AGVPortReady = true;
                        }
                        else
                        {
                            AGVPortReady = false;
                        }
                    }

                    if (AGVPortReady)
                    {
                        foreach (PortPLCInfo portPLCStatus in portPLCDatas)
                        {
                            //呼叫退補空box 流程。 先將特定port 開啟自動退補，產生完命令後再關閉。
                            portINIData[portPLCStatus.EQ_ID].openAGV_Station = true;
                            PLC_AGV_Station_OutMode(portPLCStatus);
                            portINIData[portPLCStatus.EQ_ID].openAGV_Station = false;
                        }
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "CheckForChangeAGVPortMode");
            }
        }
        #endregion
    }
}
