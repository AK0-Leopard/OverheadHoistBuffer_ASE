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
// 2020/07/07    Hsinyu Chang   N/A            2020.07.07   Master PLC斷線時發alarm
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
using System.Data;
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
    public enum PortTypeNum
    {
        No_Change = 0,
        Input_Mode = 1,
        OutPut_Mode = 2,
    }
    public class PortINIData
    {
        #region 共有屬性
        public string PortName { get; set; }
        public string UnitType { get; set; }
        public string ZoneName { get; set; }
        public string Group { get; set; }   //上報 MCS ZoneName 不能報 AGVZone (ST01、ST02)，所以另外開一個來記錄
        public DateTime portStateErrorLogTime { get; set; }  //執行的命令 port 狀態異常 10 秒記 Log 一次
        public int ADR_ID { get; set; }
        public bool alarmSetIng { get; set; }
        #endregion
        #region CRANE 才有用到的屬性
        public bool craneLoading { get; set; }
        public bool craneUnLoading { get; set; }
        #endregion
        #region CV_Port 才有用到的屬性

        public int Stage { get; set; }
        public int nowStage { get; set; }
        public string IgnoreModeChange { get; set; }    // Y = 忽略 PLC 訊號，一律 Port 當，N = 讀取 PLC 正常上報

        #endregion
        #region CV_Port、CRANE 才有用到的屬性

        public int timeOutForAutoUD { get; set; }   //卡匣停在 Port 或 車上 的停留時間超過幾秒，就自動搬到儲位
        public string timeOutForAutoInZone { get; set; }    //timeOutForAutoUD 超過時間自動搬到哪個 Zone
        public string timeOutLog { get; set; }

        #endregion 
        #region AGV Port 才有用到的屬性

        public bool openAGV_Station { get; set; }
        public bool openAGV_AutoPortType { get; set; }
        public bool movebackBOXsleep { get; set; }      //0601 士偉提出 AGV 在 OutMode 的時候判斷退BOX時，先延遲300毫秒再檢查一次，若還是退BOX結果再退

        #endregion
        #region AGVZone_Port 才有用到的屬性

        public E_PORT_STATUS openAGVZone { get; set; }
        public bool forceRejectAGVCTrigger { get; set; }
        public bool agvHasCmdsAccess { get; set; }
        public bool oneInOneOutAgvStation { get; set; }
        public DateTime reservePortTime { get; set; }
        #endregion
    }
    public class PortAdr
    {
        public string PortName { get; set; }
        public int ADR_ID { get; set; }
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
        LINE,
    }
    public class OHT_BOXID_MismatchData
    {
        public string BOXID { get; set; }
        public string CmdSourcePort { get; set; }
        public DateTime TriggerTime { get; set; }
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

        #region 系統
        Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public Logger TransferServiceLogger = NLog.LogManager.GetLogger("TransferServiceLogger");
        public Logger AGVCTriggerLogger = NLog.LogManager.GetLogger("TransferServiceLogger");

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
        #endregion

        #region 時間儲存
        DateTime updateTime;    //定時更新狀態
        DateTime ohtTimeout;    //AVEHICLE 裡面沒有閒置的車輛，無法執行1分鐘紀錄Log
        DateTime cmdTimeOut;
        DateTime deleteDBLogTime;   //記錄什麼時候刪除放在資料庫的 LOG
        TimeSpan deleteDBTimeSpan;  //計算 deleteDBLogTime 使用，每日做刪除
        #endregion

        #region TimeOut 設定
        int ohtCmdTimeOut = 0;      //詢問 OHT 命令被拒絕記錄LOG，1分鐘記錄一次
        int ohtIdleTimeOut = 0;
        int cmdIdleTimeOut = 30;    //秒鐘

        public int cstIdle = 120;   //秒鐘，卡匣停在 Port上或車上，超過設定沒搬，自動搬往儲位
        public int queueCmdTimeOut = 1200;  //秒鐘
        public int agvHasCmdsAccessTimeOut = 300;   ///秒鐘
        public int portWaitOutTimeOut = 10; //分鐘，Port WaitOut 過久，報異常
        public int ohtID_MismatchTimeOut = 30;  //分鐘，OHT 對同個來源 Port，設定時間內發生兩次 Mismatch，且兩次讀到的ID都一樣，將CSTID改成UNKU
        public int deleteDBLogTimeOut = 1;      //月份，刪除幾月前的 LOG 資料
        #endregion

        #region 狀態旗標
        bool iniStatus = false; //初始化旗標
        bool iniSetPortINIData = false;
        bool cmdFail = false;   //cmdIdleTimeOut 的旗標
        bool queueCmdFail = false;  //queueCmdTimeOut 的旗標
        bool cmdFailAlarmSet = true;

        public bool requireEmptyBox = true; // 空盒水位要求符號，啟動後才會進行空盒需求
        public bool redisEnable = false;
        public bool agvZone_ConnectedRealAGVPortRunDown = true;
        public bool portTypeChangeOK_CVPort_CstRemove = true;      //Port 轉向成功時，刪除此 Port 的所有卡匣
        public bool agvWaitOutOpenBox = true;                      //AGVPort WaitOut 時，是否做開蓋動作
        public bool autoRemarkBOXCSTData = false;                   //是否開啟自動救帳流程。
        public bool setForMoreOut = true;                           //是否為多出模式。
        public bool agvHasCmdsAccess = false;           //Agv 有命令要搬入與否。
        public bool oneInoneOutMethodUse = true;          //是否使用單取單放流程判定AGV 虛擬Port
        public bool swapTriggerWaitin = false;
        #endregion

        #region 資料暫存
        public Dictionary<string, PortINIData> portINIData = null;
        public Dictionary<string, WaitInOutLog> waitInLog = new Dictionary<string, WaitInOutLog>();
        public List<PortAdr> allPortAdr = new List<PortAdr>();

        //public Dictionary<string, WaitInOutLog> waitOutLog = new Dictionary<string, WaitInOutLog>();

        public Dictionary<string, OHT_BOXID_MismatchData> OHT_MismatchData = new Dictionary<string, OHT_BOXID_MismatchData>();

        public List<string> queueCmdTimeOutCmdID = new List<string>();  //存放 Queue 過久的命令

        public string agvcTriggerResult_ST01 = "無";
        public string agvcTriggerResult_ST02 = "無";
        public string agvcTriggerResult_ST03 = "無";
        #endregion

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

            line.OnLocalDisconnection += OnLocalDisconnected;
            line.OnLocalConnection += OnLocalConnected;

            TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "TransferService >> 初始化結束------------------------------------");
        }
        public void SetPortINIData()
        {
            try
            {
                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "SetPortINIData 開始------------------------------------");
                portINIData = new Dictionary<string, PortINIData>();

                portINIData.Clear();
                PortINIData lineData = new PortINIData();

                lineData.PortName = line.LINE_ID;
                lineData.UnitType = UnitType.LINE.ToString();
                AddPortINIData(lineData);

                foreach (var v in portDefBLL.GetOHB_CVPortData(line.LINE_ID))
                {
                    for (int i = 1; i <= (int)v.Stage; i++)
                    {
                        PortINIData data = new PortINIData();

                        data.UnitType = v.UnitType.Trim();
                        data.ZoneName = v.PLCPortID.Trim();
                        data.Stage = (int)v.Stage;

                        data.openAGV_Station = false;
                        data.openAGV_AutoPortType = false;

                        data.nowStage = i;
                        data.movebackBOXsleep = false;
                        data.timeOutForAutoUD = (int)v.TimeOutForAutoUD;
                        //data.timeOutForAutoInZone = v.TimeOutForAutoInZone;
                        data.openAGVZone = (E_PORT_STATUS)v.AGVState;
                        data.Group = v.ZoneName?.Trim() ?? "";
                        data.ADR_ID = int.Parse(v.ADR_ID);

                        if (i == data.Stage)
                        {
                            data.PortName = v.PLCPortID.Trim();
                        }
                        else
                        {
                            data.PortName = v.PLCPortID.Trim() + ((CassetteData.OHCV_STAGE)i).ToString();
                        }

                        AddPortINIData(data);

                        if (isAGVZone(data.PortName))
                        {
                            OpenAGVZone(data.PortName, data.openAGVZone);
                            data.forceRejectAGVCTrigger = false;
                        }
                    }

                    if (isAGVZone(v.PLCPortID.Trim()) == false)
                    {
                        AddPortAdr(v.PLCPortID.Trim(), int.Parse(v.ADR_ID));
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
                    data.ADR_ID = int.Parse(v.ADR_ID);

                    AddPortINIData(data);

                    AddPortAdr(v.ShelfID.Trim(), int.Parse(v.ADR_ID));
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

                allPortAdr = allPortAdr.OrderBy(data => data.ADR_ID).ToList();

                iniSetPortINIData = true;

                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "SetPortINIData 結束------------------------------------");
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "SetPortData");
            }
        }
        public void AddPortAdr(string portName, int adr)
        {
            PortAdr portAdr = new PortAdr();
            portAdr.PortName = portName;
            portAdr.ADR_ID = adr;

            if (allPortAdr.Where(data => data.ADR_ID == adr).Count() == 0)
            {
                allPortAdr.Add(portAdr);
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
            TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "iniShelfData 開始------------------------------------");
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
            TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "iniShelfData 結束------------------------------------");
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
            TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "AlliniPortData 開始------------------------------------");
            foreach (var v in GetCVPort())
            {
                iniPortData(v.PortName);
            }
            TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "AlliniPortData 結束------------------------------------");
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

                    PortCIM_ON(portValue, "iniPortData");

                    PLC_ReportRunDwon(portValue, "iniPortData");
                    #endregion
                    #region Port卡匣資料處理

                    #region 先檢查是否有殘帳
                    if (portValue.LoadPosition1 == false)
                    {
                        iniDeletePortCstData(portValue.EQ_ID, 1);
                    }

                    if (portValue.LoadPosition2 == false)
                    {
                        iniDeletePortCstData(portValue.EQ_ID, 2);
                    }

                    if (portValue.LoadPosition3 == false)
                    {
                        iniDeletePortCstData(portValue.EQ_ID, 3);
                    }

                    if (portValue.LoadPosition4 == false)
                    {
                        iniDeletePortCstData(portValue.EQ_ID, 4);
                    }

                    if (portValue.LoadPosition5 == false)
                    {
                        iniDeletePortCstData(portValue.EQ_ID, 5);
                    }

                    if (portValue.LoadPosition6 == false)
                    {
                        iniDeletePortCstData(portValue.EQ_ID, 6);
                    }

                    if (portValue.LoadPosition7 == false)
                    {
                        iniDeletePortCstData(portValue.EQ_ID, 7);
                    }
                    #endregion

                    if (portValue.OpAutoMode)
                    {
                        if (portValue.IsInputMode)
                        {
                            if (portValue.LoadPosition1)
                            {
                                if (portValue.PortWaitIn)
                                {
                                    PLC_ReportPortWaitIn(portValue, "iniPortData");
                                }
                                else
                                {
                                    if (isUnitType(portValue.EQ_ID, UnitType.AGV) && portValue.IsCSTPresence == false)
                                    {
                                        CassetteData portCstData = cassette_dataBLL.loadCassetteDataByLoc(portValue.EQ_ID);

                                        if (portCstData != null)
                                        {
                                            DeleteCst(portCstData.CSTID, portCstData.BOXID, "iniPortData 刪除空BOX");
                                        }
                                    }
                                }
                            }
                        }
                    }

                    #endregion
                }

            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "UpDBPortTypeData");
            }
        }
        public void iniDeletePortCstData(string _portName, int stage)
        {
            //TransferServiceLogger.Info
            //(
            //    DateTime.Now.ToString("HH:mm:ss.fff ") +
            //    "OHB >> DB|DeleteCst：cstID:" + cstID + "    boxID:" + boxID + "  誰呼叫:" + cmdSource
            //);

            string portName = GetPositionName(_portName, stage);

            CassetteData portCstData = cassette_dataBLL.loadCassetteDataByLoc(portName);

            if (portCstData != null)
            {
                DeleteCst(portCstData.CSTID, portCstData.BOXID, "iniDeletePortCstData");
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
        public void updateAGVHasCmdsAccessStatus()
        {
            foreach (var v in GetAGVZone())
            {
                if (v.agvHasCmdsAccess)
                {
                    TimeSpan timeSpan = DateTime.Now - v.reservePortTime;

                    if (timeSpan.TotalSeconds >= agvHasCmdsAccessTimeOut)
                    {
                        v.reservePortTime = DateTime.Now;

                        OHBC_AlarmSet(v.PortName, ((int)AlarmLst.AGV_HasCmdsAccessTimeOut).ToString());

                        //OHBC_AlarmCleared("LINE", ((int)AlarmLst.AGV_HasCmdsAccessTimeOut).ToString());
                        //OHBC_AGV_HasCmdsAccessCleared(v.PortName);
                    }
                }
            }
        }
        public void iniOHTData(string craneName, string apiSource)
        {
            TransferServiceLogger.Info
            (DateTime.Now.ToString("HH:mm:ss.fff ")
                + craneName + " iniOHTData 初始化開始，誰呼叫: " + apiSource);

            AVEHICLE vehicle = scApp.VehicleService.GetVehicleDataByVehicleID(craneName);
            //vehicle.CUR_ADR_ID    //存放車子現在位置
            if (vehicle.isTcpIpConnect == false)
            {
                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + craneName + " 連線狀態(isTcpIpConnect) : " + vehicle.isTcpIpConnect + " iniOHTData 初始化失敗 ");
                return;
            }

            #region 命令

            foreach (var mcsCmd in cmdBLL.getCMD_ByOHTName(craneName))
            {
                bool deleteCmd = false;
                string log = "";

                if (vehicle.ACT_STATUS == VHActionStatus.NoCommand)
                {
                    log = " 沒有命令";
                    deleteCmd = true;
                }
                else
                {
                    if (vehicle.MCS_CMD.Trim() != mcsCmd.CMD_ID.Trim())
                    {
                        log = " 命令名稱不一樣，vehicle.MCS_CMD: " + vehicle.MCS_CMD;
                        deleteCmd = true;
                    }
                }

                if (deleteCmd)
                {
                    TransferServiceLogger.Info
                    (DateTime.Now.ToString("HH:mm:ss.fff ")
                        + "iniOHTData " + craneName + log + "，刪除:" + GetCmdLog(mcsCmd)
                    );

                    Manual_DeleteCmd(mcsCmd.CMD_ID, "iniOHTData");

                    Task.Run(() =>
                    {
                        cmdBLL.forceUpdataCmdStatus2FnishByVhID(craneName); // Force finish Cmd
                    });
                }
            }

            if (vehicle.ACT_STATUS == VHActionStatus.NoCommand && string.IsNullOrWhiteSpace(vehicle.MCS_CMD) == false)
            {
                Task.Run(() =>
                {
                    cmdBLL.forceUpdataCmdStatus2FnishByVhID(craneName); // Force finish Cmd
                });
            }
            #endregion

            #region 卡匣
            CassetteData dbOHT_CSTdata = cassette_dataBLL.loadCassetteDataByLoc(craneName);

            if (vehicle.HAS_CST == 1)
            {
                CassetteData nowOHT_CSTdata = new CassetteData();
                nowOHT_CSTdata.CSTID = "ERROR1";
                nowOHT_CSTdata.BOXID = vehicle.BOX_ID.Trim();
                nowOHT_CSTdata.Carrier_LOC = craneName;

                nowOHT_CSTdata = IDRead(nowOHT_CSTdata);

                if (dbOHT_CSTdata != null)
                {
                    if (vehicle.BOX_ID.Trim() != dbOHT_CSTdata.BOXID.Trim())
                    {
                        DeleteCst(dbOHT_CSTdata.CSTID, dbOHT_CSTdata.BOXID, "iniOHTData");

                        OHBC_InsertCassette(nowOHT_CSTdata.CSTID, nowOHT_CSTdata.BOXID, nowOHT_CSTdata.Carrier_LOC, "iniOHTData");
                    }
                }
                else
                {
                    OHBC_InsertCassette(nowOHT_CSTdata.CSTID, nowOHT_CSTdata.BOXID, nowOHT_CSTdata.Carrier_LOC, "iniOHTData");
                }
            }
            else
            {
                if (dbOHT_CSTdata != null)
                {
                    DeleteCst(dbOHT_CSTdata.CSTID, dbOHT_CSTdata.BOXID, "iniOHTData");
                }
            }
            #endregion


            //
            //ACMD_MCS cmd = cmdBLL.getCMD_ByOHTName(craneName).FirstOrDefault();

            //    AVEHICLE.HAS_BOX
            //    AVEHICLE.HAS_CST 車上有沒有料

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
        public void DeleteLog()
        {
            TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "TransferService >> DeleteLog 開始------------------------------------");
            deleteDBLogTime = DateTime.Now;
            cmdBLL.DeleteLog(deleteDBLogTimeOut);
            TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "TransferService >> DeleteLog 結束------------------------------------");
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
                            cmdTimeOut = DateTime.Now;
                            DeleteLog();
                            TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "TransferService >> Port資料初始化結束------------------------------------");

                            iniStatus = true;
                            #endregion
                        }
                        return;
                    }

                    deleteDBTimeSpan = DateTime.Now - deleteDBLogTime;

                    if (deleteDBTimeSpan.TotalDays >= 1)
                    {
                        DeleteLog();
                    }
                    #region Port狀態處理
                    TimeSpan updateTimeSpan = DateTime.Now - updateTime;

                    if (updateTimeSpan.Seconds >= 3.5)
                    {
                        updateAGVStation();
                        updateAGVHasCmdsAccessStatus();
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
                            var queueCmdData = cmdData.Where(data => data.CMDTYPE != CmdType.PortTypeChange.ToString() && data.TRANSFERSTATE == E_TRAN_STATUS.Queue).ToList();
                            var transferCmdData = cmdData.Where(data => data.CMDTYPE != CmdType.PortTypeChange.ToString() && data.TRANSFERSTATE != E_TRAN_STATUS.Queue).ToList();

                            var portTypeChangeCmdData = cmdData.Where(data => data.CMDTYPE == CmdType.PortTypeChange.ToString()).ToList();

                            #region 檢查救資料用AGV Port 狀態是否正確
                            if (autoRemarkBOXCSTData == true)
                            {
                                AutoReMarkCSTBOXDataFromAGVPort();
                            }
                            #endregion

                            queueCmdData = scApp.CMDBLL.doSortMCSCmdDataByDistanceFromHostSourceToVehicle(queueCmdData, vehicleData);

                            if (queueCmdData.Count != 0)
                            {
                                cmdFail = true;
                            }

                            foreach (var v in queueCmdData)
                            {
                                #region 每分鐘權限 + 1
                                if (string.IsNullOrWhiteSpace(v.TIME_PRIORITY.ToString()))
                                {
                                    cmdBLL.updateCMD_MCS_TimePriority(v.CMD_ID, 0);
                                }
                                else
                                {
                                    DateTime nowTime = DateTime.Now;

                                    int addtime = v.TIME_PRIORITY / SystemParameter.cmdPriorityAdd;

                                    DateTime cmdTime = v.CMD_INSER_TIME.AddMinutes(addtime);

                                    TimeSpan span = nowTime - cmdTime;
                                    if (span.Minutes >= 1)
                                    {
                                        TimeSpan span1 = nowTime - v.CMD_INSER_TIME;
                                        cmdBLL.updateCMD_MCS_TimePriority(v.CMD_ID, span1.Minutes * SystemParameter.cmdPriorityAdd);
                                        //cmdBLL.updateCMD_MCS_sumPriority(v.CMD_ID);
                                    }
                                }
                                #endregion                                
                                #region 搬送命令
                                if (TransferCommandHandler(v))
                                {
                                    cmdFail = false;
                                    OHBC_OHT_QueueCmdTimeOutCmdIDCleared(v.CMD_ID);
                                    break;
                                }
                                else
                                {
                                    if (isUnitType(v.HOSTDESTINATION, UnitType.AGV) && agvHasCmdsAccess)
                                    {
                                        cmdFail = false;
                                    }

                                    TimeSpan timeSpan = DateTime.Now - v.CMD_INSER_TIME;

                                    if (timeSpan.TotalSeconds >= queueCmdTimeOut)
                                    {
                                        if (queueCmdTimeOutCmdID.Contains(v.CMD_ID.Trim()) == false)
                                        {
                                            queueCmdTimeOutCmdID.Add(v.CMD_ID.Trim());

                                            OHBC_AlarmSet(line.LINE_ID, ((int)AlarmLst.OHT_QueueCmdTimeOut).ToString());
                                        }
                                    }
                                }
                                #endregion
                            }

                            foreach (var v in portTypeChangeCmdData)
                            {
                                #region PLC控制命令
                                PortPLCInfo portInfo = GetPLC_PortData(v.HOSTSOURCE);

                                if (portInfo.OpAutoMode == false || portInfo.IsModeChangable == false)   // || (int)portData.AGVState == SECSConst.PortState_OutService
                                {
                                    continue;
                                }
                                else
                                {
                                    E_PortType portType = (E_PortType)Enum.Parse(typeof(E_PortType), v.HOSTDESTINATION);

                                    if ((portInfo.IsInputMode && portType == E_PortType.In)
                                     || (portInfo.IsOutputMode && portType == E_PortType.Out)
                                       )
                                    {
                                        ReportPortType(portInfo.EQ_ID, portType, "TransferRun");

                                        cmdBLL.DeleteCmd(v.CMD_ID);
                                    }
                                    else
                                    {
                                        PortTypeChange(v.HOSTSOURCE, portType, "TransferRun");
                                    }
                                }
                                #endregion
                            }

                            foreach (var v in transferCmdData)
                            {
                                TransferCommandHandler(v);
                            }

                            if (cmdFail)
                            {
                                cmdFail = false;

                                TimeSpan timeSpan = DateTime.Now - cmdTimeOut;

                                if (timeSpan.TotalSeconds >= cmdIdleTimeOut)
                                {
                                    cmdTimeOut = DateTime.Now;

                                    TransferServiceLogger.Info
                                    (
                                        DateTime.Now.ToString("HH:mm:ss.fff ")
                                        + "OHB >> OHB| 車子閒置、有命令，超時 " + cmdIdleTimeOut + " 秒鐘，報異常"
                                    );

                                    OHBC_AlarmSet(line.LINE_ID, ((int)AlarmLst.OHT_IDLE_HasCMD_TimeOut).ToString());
                                    cmdFailAlarmSet = true;
                                }
                            }
                            else
                            {
                                cmdTimeOut = DateTime.Now;

                                OHBC_OHT_IDLE_HasCMD_TimeOutCleared();
                            }
                        }
                        else
                        {
                            //若沒有命令時，產生救回Unknown CST 的命令
                            if (autoRemarkBOXCSTData == true)
                            {
                                bool checkForGenerate = CheckAndTryRemarkUnknownCSTInShelf();
                                if (checkForGenerate == false)
                                {
                                    TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> OHB| 產生救回Unknown CST 的命令失敗");
                                }
                            }
                            #region 檢查救資料用AGV Port 狀態是否正確
                            if (autoRemarkBOXCSTData == true)
                            {
                                AutoReMarkCSTBOXDataFromAGVPort();
                            }
                            #endregion

                            OHBC_OHT_IDLE_HasCMD_TimeOutCleared();
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
        /// <summary>
        /// 判斷目前的狀態是否可以將指定Port上的Box退回
        /// </summary>
        private void AutoReMarkCSTBOXDataFromAGVPort()
        {
            try
            {
                // A20.07.12.0
                //檢查特定Port
                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> OHB| Enter AutoReMarkCSTBOXDataFromAGVPort");

                PortDef targetAGVport = FindTargetPort();

                if (targetAGVport.PLCPortID == null)
                {
                    TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> OHB| targetAGVport = null");
                    return;
                }
                PortPLCInfo targetPortPLCStatus = GetPLC_PortData(targetAGVport.PLCPortID.Trim());
                /**若為空盒造成沒有mismatch 自動搬回**/
                if (targetPortPLCStatus.IsInputMode && targetPortPLCStatus.IsReadyToUnload && targetPortPLCStatus.OpAutoMode &&
                    (targetPortPLCStatus.CSTPresenceMismatch || targetPortPLCStatus.AGVPortReady))
                {
                    MovebackBOX(targetPortPLCStatus.CassetteID, targetPortPLCStatus.BoxID, targetPortPLCStatus.EQ_ID, targetPortPLCStatus.IsCSTPresence, "AutoReMarkCSTBOXDataFromAGVPort");
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "AutoReMarkCSTBOXDataFromAGVPort");
            }
        }

        private void CraneCountBy2(List<ACMD_MCS> cmdList)    //當有兩台車在同一條LINE (非LOOP)
        {
            foreach (var cmd1 in cmdList)
            {

            }
            List<AVEHICLE> avehicles = scApp.VehicleBLL.cache.loadVhs();

            foreach (var vh in avehicles)
            {
                if (string.IsNullOrWhiteSpace(vh.OHTC_CMD) == false)
                {
                    ACMD_OHTC ohtCmdData = cmdBLL.getCMD_OHTCByID(vh.OHTC_CMD);
                    if (ohtCmdData != null)
                    {

                    }
                }
            }
            //List<PortINIData> portADR = GetPortADR();
            //portADR.Where(data => data.ADR_ID == 0).FirstOrDefault();
            //int i = portADR.IndexOf(portADR.Where(data => data.ADR_ID == 0).FirstOrDefault());
        }
        /// <summary>
        /// 比較兩筆命令路徑是否重疊
        /// </summary>
        public bool GetSourceDest(string cmd1_source, string cmd1_dest, string cmd2_source, string cmd2_dest)
        {
            #region cmd1 資料確認
            if (isAGVZone(cmd1_source))
            {
                cmd1_source = GetAGVZoneADRPortName(cmd1_source, cmd1_dest);
            }

            if (isAGVZone(cmd1_dest))
            {
                cmd1_dest = GetAGVZoneADRPortName(cmd1_dest, cmd1_source);
            }

            int cmd1_sourceAdr = portINIData[cmd1_source].ADR_ID;
            int cmd1_destAdr = portINIData[cmd1_dest].ADR_ID;
            #endregion

            #region cmd2 資料確認
            if (isAGVZone(cmd2_source))
            {
                cmd2_source = GetAGVZoneADRPortName(cmd2_source, cmd2_dest);
            }

            if (isAGVZone(cmd2_dest))
            {
                cmd2_dest = GetAGVZoneADRPortName(cmd2_dest, cmd2_source);
            }

            int cmd2_SourceAdr = portINIData[cmd2_source].ADR_ID;
            int cmd2_DestAdr = portINIData[cmd2_dest].ADR_ID;
            #endregion

            #region 開始比較
            int minimum = 0;
            int maximum = 0;

            if (cmd2_DestAdr > cmd2_SourceAdr)
            {
                minimum = GetPortMinMaxADR(cmd2_SourceAdr, "min");
                maximum = GetPortMinMaxADR(cmd2_DestAdr, "max");
            }
            else
            {
                minimum = GetPortMinMaxADR(cmd2_DestAdr, "min");
                maximum = GetPortMinMaxADR(cmd2_SourceAdr, "max");
            }

            if (((cmd1_sourceAdr < minimum && cmd1_destAdr < minimum)
              || (cmd1_sourceAdr > maximum && cmd1_destAdr > maximum)
                )
              && minimum != 0 && maximum != 0
              )
            {
                return true;
            }
            else
            {
                return false;
            }
            #endregion
        }

        public int GetPortMinMaxADR(int adr_id, string type)
        {
            int spac = 1;
            int adr = 0;
            List<PortAdr> portAdrs = GetAll_ADR();
            PortAdr portAdr = portAdrs.Where(data => data.ADR_ID == adr_id).FirstOrDefault();

            if (portAdr != null)
            {
                int index = portAdrs.IndexOf(portAdr);

                if (index >= 0)  //portAdrs.IndexOf(portAdr) 找不到值會回傳 -1
                {
                    if (type == "min")
                    {
                        index = index - spac;  //-1 後來要變可調
                    }
                    else if (type == "max")
                    {
                        index = index + spac;  //-1 後來要變可調
                    }

                    if (index <= 0)
                    {
                        adr = portAdrs[0].ADR_ID;
                    }
                    else if (index >= portAdrs.Count)
                    {
                        adr = portAdrs[portAdrs.Count].ADR_ID;
                    }
                    else
                    {
                        adr = portAdrs[index].ADR_ID;
                    }
                }
            }

            return adr;
        }

        public string GetAGVZoneADRPortName(string source, string dest)
        {
            PortINIData sourceAGVData = GetAGVPort(source).FirstOrDefault();
            int defaultVelue = sourceAGVData.ADR_ID;
            string agvPortName = sourceAGVData.PortName;

            foreach (var v in GetAGVPort(source))
            {
                string destPort = dest;
                if (isAGVZone(dest))
                {
                    PortINIData destAGVData = GetAGVPort(dest).FirstOrDefault();
                    destPort = destAGVData.PortName;
                }

                if (portINIData[v.PortName].ADR_ID >= portINIData[destPort].ADR_ID)
                {
                    if (portINIData[v.PortName].ADR_ID > defaultVelue)
                    {
                        defaultVelue = portINIData[v.PortName].ADR_ID;
                        agvPortName = portINIData[v.PortName].PortName;
                    }
                }
                else
                {
                    if (portINIData[v.PortName].ADR_ID < defaultVelue)
                    {
                        defaultVelue = portINIData[v.PortName].ADR_ID;
                        agvPortName = portINIData[v.PortName].PortName;
                    }
                }

                source = agvPortName;
            }
            return agvPortName;
        }
        public List<PortINIData> GetPortADR()
        {
            List<PortINIData> portADR = portINIData.Values.Where(data =>
                                                                    (isCVPort(data.PortName) || isShelfPort(data.PortName))
                                                                    && isAGVZone(data.PortName) == false
                                                                ).OrderBy(data => data.ADR_ID).ToList();

            return portADR;
        }
        public List<PortAdr> GetAll_ADR()
        {
            return allPortAdr;
        }
        private bool TransferCommandHandler(ACMD_MCS mcsCmd)
        {
            bool TransferIng = false;

            switch (mcsCmd.TRANSFERSTATE)
            {
                #region E_TRAN_STATUS.Queue
                case E_TRAN_STATUS.Queue:

                    bool sourcePortType = false;
                    bool destPortType = false;

                    #region 檢查來源狀態
                    if (string.IsNullOrWhiteSpace(mcsCmd.RelayStation))  //檢查命令是否先搬到中繼站
                    {
                        #region 檢查是否有帳
                        if (mcsCmd.CMD_ID.Contains("SCAN") == false)
                        {
                            CassetteData sourceCstData = cassette_dataBLL.loadCassetteDataByLoc(mcsCmd.HOSTSOURCE);

                            if (sourceCstData == null)
                            {
                                sourcePortType = false;
                                TransferServiceLogger.Info
                                (
                                    DateTime.Now.ToString("HH:mm:ss.fff ")
                                    + "OHB >> OHB| 命令來源: " + mcsCmd.HOSTSOURCE + " 找不到帳，刪除命令 "
                                );
                                Manual_DeleteCmd(mcsCmd.CMD_ID, "命令來源找不到帳");

                                return false;
                            }
                        }
                        #endregion

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
                        }
                    }
                    else
                    {
                        sourcePortType = AreSourceEnable(mcsCmd.RelayStation);
                        mcsCmd.HOSTSOURCE = mcsCmd.RelayStation;
                    }
                    #endregion
                    #region 檢查目的狀態
                    if (isUnitType(mcsCmd.HOSTDESTINATION, UnitType.ZONE))  //若 Zone 上沒有儲位，目的 Port 會為 ZoneName，並上報 MCS
                    {
                        string zoneID = mcsCmd.HOSTDESTINATION;
                        List<ShelfDef> shelfData = scApp.ShelfDefBLL.GetEmptyAndEnableShelfByZone(zoneID);//Modify by Kevin

                        if (shelfData == null || shelfData.Count() == 0)
                        {
                            TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|TransferCommandHandler 目的 Zone: " + mcsCmd.HOSTDESTINATION + " 沒有位置");

                            cmdBLL.updateCMD_MCS_TranStatus(mcsCmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);

                            reportBLL.ReportTransferInitiated(mcsCmd.CMD_ID.Trim());
                            reportBLL.ReportTransferCompleted(mcsCmd, null, ResultCode.ZoneIsfull);
                            break;
                        }
                        else
                        {
                            TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|TransferCommandHandler 目的 Zone: " + mcsCmd.HOSTDESTINATION + " 可用儲位數量: " + shelfData.Count);

                            string shelfID = scApp.TransferService.GetShelfRecentLocation(shelfData, mcsCmd.HOSTSOURCE);

                            if (string.IsNullOrWhiteSpace(shelfID) == false)
                            {
                                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|TransferCommandHandler: 目的 Zone: " + mcsCmd.HOSTDESTINATION + " 找到 " + shelfID);
                                mcsCmd.HOSTDESTINATION = shelfID;
                            }
                        }

                        //cmdBLL.updateCMD_MCS_TranStatus(mcsCmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);
                        //reportBLL.ReportTransferInitiated(mcsCmd.CMD_ID.Trim());
                        //reportBLL.ReportTransferCompleted(mcsCmd, null, ResultCode.ZoneIsfull);
                        //break;
                    }

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
                        if (OHT_TransportRequest(mcsCmd))
                        {
                            TransferIng = true;
                            cmdBLL.updateCMD_MCS_Dest(mcsCmd.CMD_ID, mcsCmd.HOSTDESTINATION);
                        }
                    }
                    else if (sourcePortType && isShelfPort(mcsCmd.HOSTSOURCE) == false
                          && destPortType == false && isCVPort(mcsCmd.HOSTDESTINATION))
                    {
                        //來源目的都是 CV Port 且 目的不能搬，觸發將卡匣送至中繼站
                        TimeSpan timeSpan = DateTime.Now - mcsCmd.CMD_INSER_TIME;

                        if (timeSpan.TotalSeconds < SystemParameter.cmdTimeOutToAlternate)  //200806 SCC+ 目的Port不能搬，超過30秒才產生搬往中繼站，防止 AGV Port正準備做退補 BOX 跟 車子剛好放在 CV 上，造成 CV 短暫不能放貨之情況
                        {
                            break;
                        }

                        if (isAGVZone(mcsCmd.HOSTDESTINATION))
                        {
                            string agvName = "";

                            foreach (var v in GetAGVPort(mcsCmd.HOSTDESTINATION))
                            {
                                agvName = v.PortName.Trim();

                                if (GetPLC_PortData(v.PortName).IsOutputMode)
                                {
                                    break;
                                }
                            }

                            if (string.IsNullOrWhiteSpace(agvName))
                            {
                                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|觸發搬往中繼站 AGV Zone: " + mcsCmd.HOSTDESTINATION + " 找不到 Port" + agvName + "跳出");
                                return false;
                            }

                            mcsCmd.HOSTDESTINATION = agvName;
                        }

                        PortPLCInfo plcInfoSource = GetPLC_PortData(mcsCmd.HOSTSOURCE);
                        PortPLCInfo plcInfoDest = GetPLC_PortData(mcsCmd.HOSTDESTINATION);

                        if (plcInfoSource.OpAutoMode && plcInfoSource.IsReadyToUnload
                         && plcInfoDest.OpAutoMode && plcInfoDest.IsReadyToLoad == false
                           )
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

                                    TransferServiceLogger.Info
                                    (
                                        DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> OHB|搬到中繼站: " + cmdRelay.HOSTDESTINATION
                                    );

                                    TransferIng = true;
                                }
                            }
                            else
                            {
                                TransferServiceLogger.Info
                                (
                                    DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> OHB|搬到中繼站，沒有儲位"
                                );
                            }
                        }
                        else
                        {
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> OHB| 觸發將卡匣送至中繼站失敗: "
                                + " plcInfo_Source.EQ_ID: " + plcInfoSource.EQ_ID
                                + " plcInfo_Source.OpAutoMode 要 True 實際是 " + plcInfoSource.OpAutoMode
                                + " plcInfo_Source.IsReadyToUnload 要 True 實際是 " + plcInfoSource.IsReadyToUnload
                                + " plcInfo_Dest.EQ_ID: " + plcInfoDest.EQ_ID
                                + " plcInfo_Dest.OpAutoMode 要 True 實際是 " + plcInfoDest.OpAutoMode
                                + " plcInfo_Dest.IsReadyToLoad 要 false 實際是 " + plcInfoDest.IsReadyToLoad
                            );
                        }
                    }

                    break;
                #endregion
                #region E_TRAN_STATUS.Transferring
                case E_TRAN_STATUS.Transferring:
                    switch (mcsCmd.COMMANDSTATE)
                    {
                        case COMMAND_iIdle:
                            //#region Log
                            //TransferServiceLogger.Info
                            //(
                            //    DateTime.Now.ToString("HH:mm:ss.fff ")
                            //    + "OHT >> OHB|OHT_TransferProcess 發現車子未回應 COMMANDSTATE = COMMAND_iIdle 自動變回 Queue \n"
                            //    + GetCmdLog(mcsCmd)
                            //);
                            //#endregion
                            //cmdBLL.updateCMD_MCS_TranStatus(mcsCmd.CMD_ID, E_TRAN_STATUS.Queue);
                            break;
                        case COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH:

                            if (mcsCmd.TRANSFERSTATE != E_TRAN_STATUS.Queue)
                            {
                                #region Log
                                TransferServiceLogger.Info
                                (
                                    DateTime.Now.ToString("HH:mm:ss.fff ")
                                    + "OHT >> OHB|OHT_TransferProcess 發現殘存 COMMANDSTATE = COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH :\n"
                                    + GetCmdLog(mcsCmd)
                                );
                                #endregion
                                cmdBLL.updateCMD_MCS_TranStatus(mcsCmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);
                            }

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

            return TransferIng;
        }

        private void BoxDataHandler(List<CassetteData> cstDataList)
        {
            try
            {
                foreach (var cst in cstDataList)
                {
                    if (cst.CSTState == E_CSTState.WaitIn || cst.CSTState == E_CSTState.Installed || cst.CSTState == E_CSTState.Transferring)
                    {
                        int cstTimeOut = portINIData[cst.Carrier_LOC.Trim()].timeOutForAutoUD;
                        string zoneName = portINIData[cst.Carrier_LOC.Trim()].ZoneName;

                        bool success = false;

                        if (isCVPort(zoneName))
                        {
                            PortPLCInfo plcInfo = GetPLC_PortData(zoneName);

                            if (plcInfo.IsInputMode && plcInfo.OpAutoMode && plcInfo.LoadPosition1)
                            {
                                success = true;
                            }

                            if (cst.CSTState == E_CSTState.Installed && plcInfo.IsInputMode && plcInfo.PortWaitIn)
                            {
                                TimeSpan timeSpan = DateTime.Now - DateTime.Parse(cst.TrnDT);

                                if (timeSpan.TotalSeconds >= 20)
                                {
                                    TransferServiceLogger.Info
                                    (
                                        DateTime.Now.ToString("HH:mm:ss.fff ")
                                        + "OHB >> OHB| BoxDataHandler 卡匣狀態: " + cst.CSTState
                                        + " 重新上報 IDRead、WaitIn " + GetCstLog(cst)
                                    );

                                    cassette_dataBLL.UpdateCST_DateTime(cst.BOXID, UpdateCassetteTimeType.TrnDT);
                                    reportBLL.ReportCarrierIDRead(cst, cst.ReadStatus);
                                    reportBLL.ReportCarrierWaitIn(cst);
                                    //PLC_ReportPortWaitIn(plcInfo, "CSTState = Installed");
                                }
                            }
                        }

                        if (isUnitType(zoneName, UnitType.CRANE))
                        {
                            success = true;

                            //portINIData[cst.Carrier_LOC.Trim()].timeOutForAutoInZone = scApp.ZoneDefBLL.loadZoneData().FirstOrDefault().ZoneID;
                        }

                        if (cstTimeOut != 0 && success)
                        {
                            TimeSpan cstTimeSpan = DateTime.Now - DateTime.Parse(cst.TrnDT);

                            if (cstTimeSpan.TotalSeconds >= cstTimeOut)   //停在Port上 30秒(之後要設成可調)，自動搬到儲位上
                            {
                                ACMD_MCS cmd = cmdBLL.getCMD_ByBoxID(cst.BOXID);
                                cassette_dataBLL.UpdateCST_DateTime(cst.BOXID, UpdateCassetteTimeType.TrnDT);

                                if (cmd == null)
                                {
                                    List<ShelfDef> shelfData = shelfDefBLL.GetEmptyAndEnableShelf();    // 20/08/08，士偉提出不需要再設定到哪個ZONE，直接找空儲位搬

                                    //string timeOutZone = portINIData[cst.Carrier_LOC.Trim()].timeOutForAutoInZone;
                                    //List<ShelfDef> shelfData = shelfDefBLL.GetEmptyAndEnableShelfByZone(timeOutZone);

                                    string shelfID = GetShelfRecentLocation(shelfData, cst.Carrier_LOC.Trim());

                                    if (string.IsNullOrWhiteSpace(shelfID) == false)
                                    {
                                        TransferServiceLogger.Info
                                        (
                                            DateTime.Now.ToString("HH:mm:ss.fff ")
                                            + "OHB >> OHB| 卡匣停留 " + cstTimeOut + " 秒，尚未搬走，產生自動搬走命令 " + GetCstLog(cst)
                                        );

                                        string cmdSource = cst.Carrier_LOC.Trim();
                                        string cmdDest = shelfID;

                                        Manual_InsertCmd(cmdSource, cmdDest, 5, "cstTimeOut", CmdType.OHBC);
                                        //portINIData[cst.Carrier_LOC.Trim()].timeOutForAutoInZone = "";
                                    }
                                    else
                                    {
                                        //string log = portINIData[cst.Carrier_LOC.Trim()].timeOutForAutoInZone;

                                        TransferServiceLogger.Info
                                        (
                                            DateTime.Now.ToString("HH:mm:ss.fff ")
                                            + "OHB >> OHB| 卡匣停留 " + cstTimeOut + " 秒，尚未搬走，找不到空儲位可搬，停留卡匣為：" + GetCstLog(cst)
                                        );

                                    }
                                }
                                else
                                {
                                    TransferServiceLogger.Info
                                    (
                                        DateTime.Now.ToString("HH:mm:ss.fff ")
                                        + "OHB >> OHB| 卡匣停留 " + cstTimeOut + " 秒，尚未搬走，有命令在執行 " + GetCmdLog(cmd)
                                    );
                                }
                            }
                        }
                    }

                    if (cst.CSTState == E_CSTState.WaitOut)
                    {
                        TimeSpan cstTimeSpan = DateTime.Now - DateTime.Parse(cst.TrnDT);

                        if (cstTimeSpan.TotalMinutes >= portWaitOutTimeOut)
                        {
                            cassette_dataBLL.UpdateCST_DateTime(cst.BOXID, UpdateCassetteTimeType.TrnDT);
                            SetPortWaitOutTimeOutAlarm(cst.Carrier_LOC, 1);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "BoxDataHandler");
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
                //cmdBLL.updateCMD_MCS_Source(cmd.CMD_ID, cmd.HOSTSOURCE);

                ohtCmdTimeOut = 0;

                if (isUnitType(cmd.HOSTSOURCE, UnitType.SHELF) && string.IsNullOrWhiteSpace(cmd.RelayStation) == false)
                {
                    ShelfReserved(cmd.HOSTSOURCE, cmd.HOSTDESTINATION);
                }
            }
            else
            {
                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> OHT|OHT回應不能搬送 " + GetCmdLog(cmd));

                //DateTime cmdTime = cmd.CMD_INSER_TIME;
                //TimeSpan timeSpan = DateTime.Now - cmdTime;
                //if (timeSpan.Minutes >= 1 + ohtCmdTimeOut)
                //{
                //    TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> OHT|OHT回應不能搬送 " + GetCmdLog(cmd));
                //    ohtCmdTimeOut++;
                //}
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

                if (isCVPort(sourceName))
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
                                        sourceState = sourceState + " IsInputMode:" + sourcePort.IsInputMode;
                                    }
                                }
                            }
                            else
                            {
                                if (isUnitType(sourceName, UnitType.AGV) == false
                                    && sourcePort.IsInputMode == false
                                    && sourcePort.IsModeChangable
                                   )
                                {
                                    string cmdID = "PortTypeChange-" + sourcePort.EQ_ID.Trim() + ">>" + E_PortType.In;

                                    if (cmdBLL.getCMD_MCSByID(cmdID) == null)
                                    {
                                        //若來源流向錯誤且沒有流向切換命令，就新建
                                        SetPortTypeCmd(sourcePort.EQ_ID.Trim(), E_PortType.In);  //要測時，把註解拿掉
                                    }
                                }

                                sourceState = sourceState + " IsReadyToUnload: " + sourcePort.IsReadyToUnload + " IsInputMode: " + sourcePort.IsInputMode;
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
                else if (isUnitType(sourceName, UnitType.SHELF))
                {
                    sourcePortType = true;
                }
                else if (isUnitType(sourceName, UnitType.CRANE))
                {
                    AVEHICLE vehicle = scApp.VehicleService.GetVehicleDataByVehicleID(sourceName);

                    if (vehicle.HAS_CST != 0)
                    {
                        sourcePortType = true;
                    }
                    else
                    {
                        CassetteData ohtCST = cassette_dataBLL.loadCassetteDataByLoc(sourceName);

                        if (ohtCST != null)
                        {
                            DeleteCst(ohtCST.CSTID, ohtCST.BOXID, "車子上沒料");
                        }
                    }
                }
                #endregion

                if (sourcePortType == false)
                {
                    sourceState = sourceState + " ";

                    TimeSpan timeSpan = DateTime.Now - portINIData[sourceName].portStateErrorLogTime;

                    if (timeSpan.TotalSeconds >= 10)
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
                if (isAGVZone(destName))
                {
                    string agvPortName = GetAGV_OutModeInServicePortName(destName);
                    if (string.IsNullOrWhiteSpace(agvPortName))
                    {
                        destPortType = false;
                    }
                    else
                    {
                        destPortType = true;
                    }
                }
                else if (isCVPort(destName))
                {
                    if (portINIData[destName].Stage == 1)    //200701 SCC+ MCS 士偉、冠皚提出，目的 Port 只有 1 節時，出現目前命令到相同的 Port 不要執行
                    {
                        if (cmdBLL.GetCmdDataByDest(destName).Where(data => data.TRANSFERSTATE == E_TRAN_STATUS.Transferring).Count() != 0)
                        {
                            return false;
                        }
                    }

                    PortPLCInfo destPort = GetPLC_PortData(destName);

                    if (destPort != null)
                    {
                        if (destPort.OpAutoMode)
                        {
                            if (destPort.IsReadyToLoad || (isUnitType(destName, UnitType.STK) && destPort.preLoadOK))
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
                                        destState = destState + " IsOutputMode:" + destPort.IsOutputMode;
                                    }
                                }
                            }
                            else
                            {
                                if (isUnitType(destName, UnitType.AGV) == false
                                    && destPort.IsOutputMode == false
                                    && destPort.IsModeChangable
                                   )
                                {
                                    string cmdID = "PortTypeChange-" + destPort.EQ_ID.Trim() + ">>" + E_PortType.Out;

                                    if (cmdBLL.getCMD_MCSByID(cmdID) == null)
                                    {
                                        SetPortTypeCmd(destPort.EQ_ID.Trim(), E_PortType.Out);    //要測時，把註解拿掉
                                    }
                                }

                                destState = destState + " IsReadyToLoad: " + destPort.IsReadyToLoad + " IsOutputMode: " + destPort.IsOutputMode;
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
                else if (isUnitType(destName, UnitType.SHELF))
                {
                    ShelfDef shelfData = shelfDefBLL.loadShelfDataByID(destName);
                    if (shelfData != null)
                    {
                        if (shelfData.Enable == "Y")
                        {
                            destPortType = true;
                        }
                        else
                        {
                            destState = destState + " Enable狀態: " + shelfData.Enable;
                        }
                    }
                    else
                    {
                        destState = destState + " shelfData = Null";
                    }
                }
                #endregion

                if (destPortType == false)
                {
                    TimeSpan timeSpan = DateTime.Now - portINIData[destName].portStateErrorLogTime;

                    if (timeSpan.TotalSeconds >= 10)
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

                if (ohtCmdData == null)
                {
                    #region Log
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ")
                        + "OHT >> OHB|找 ACMD_OHTC 的 oht_cmdid: " + oht_cmdid + "  資料為 Null"
                        + " OHTName: " + ohtName
                        + " OHT_Status:" + statusToName(status)
                    );
                    #endregion

                    return true;
                }

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

                ACMD_MCS cmd = cmdBLL.getCMD_MCSByID(ohtCmdData.CMD_ID_MCS.Trim());

                if (cmd == null)
                {
                    #region OHT 手動測試，不會有 MCS_ID

                    TransferServiceLogger.Info
                    (
                        "找不到 ACMD_MCS 命令: " + GetOHTcmdLog(ohtCmdData)
                    );

                    OHT_TestProcess(ohtCmdData, status);

                    #endregion
                }
                else
                {
                    #region 中繼站命令
                    if (status == COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH
                    && cmd.CRANE.Trim() != ohtName
                    && string.IsNullOrWhiteSpace(cmd.RelayStation) == false
                       )
                    {
                        TransferServiceLogger.Info
                        (
                            "此筆變更為中繼站命令不做 COMMNAD_FINISH 狀態改變：" + GetCmdLog(cmd)
                        );

                        reportBLL.ReportCraneIdle(ohtName, cmd.CMD_ID);
                    }
                    #endregion
                    #region 命令已完成
                    else if (cmd.TRANSFERSTATE == E_TRAN_STATUS.TransferCompleted)
                    {
                        isCreatScuess &= cmdBLL.updateCMD_MCS_CmdStatus(cmd.CMD_ID, status);

                        TransferServiceLogger.Info
                        (
                            "此筆命令已完成  " + GetCmdLog(cmd)
                        );

                        if (status == COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH)
                        {
                            reportBLL.ReportCraneIdle(ohtName, cmd.CMD_ID);
                        }
                    }
                    #endregion
                    #region 未完成命令
                    else
                    {
                        #region Log
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ") + "OHT >> OHB|"
                            + "找到命令" + GetCmdLog(cmd)
                        );
                        #endregion

                        if (cmd.COMMANDSTATE == status)
                        {
                            #region Log
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ") + "OHT >> OHB| cmd.COMMANDSTATE 相同跳出"
                            );
                            #endregion
                        }
                        else
                        {
                            isCreatScuess &= cmdBLL.updateCMD_MCS_CmdStatus(cmd.CMD_ID, status);

                            if (cmd.CRANE != ohtName)
                            {
                                cmdBLL.updateCMD_MCS_CRANE(cmd.CMD_ID, ohtName);
                            }

                            if (string.IsNullOrWhiteSpace(cmd.CARRIER_ID_ON_CRANE))
                            {
                                cmd.CARRIER_ID_ON_CRANE = "";
                            }

                            if (cmd.CMD_ID.Contains("SCAN-"))
                            {
                                OHT_ScanProcess(cmd, ohtName, status);
                            }
                            else
                            {
                                OHT_TransferProcess(cmd, ohtCmdData, ohtName, status);
                            }
                        }
                    }
                    #endregion
                }

                if (status == COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH)   //20_0824 冠皚提出車子回 128 結束，直接掃命令，不要等到下次執行緒觸發
                {
                    Task.Run(() =>
                    {
                        TransferRun();
                    });
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

                        if (cmd.RelayStation == ohtCmd.SOURCE && string.IsNullOrWhiteSpace(ohtCmd.SOURCE) == false)
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

                        CassetteData ohtToPort = cassette_dataBLL.loadCassetteDataByLoc(ohtName.Trim());

                        if (ohtToPort != null)
                        {
                            cassette_dataBLL.UpdateCST_DateTime(ohtToPort.BOXID, UpdateCassetteTimeType.TrnDT);

                            if (isCVPort(ohtCmd.DESTINATION) && isAGVZone(ohtCmd.DESTINATION) == false)
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
                        else
                        {
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ")
                                + "OHT >> OHB|COMMAND_STATUS_BIT_INDEX_UNLOAD_ARRIVE 找不到卡匣，卡匣不在 " + ohtName.Trim() + " 的車上"
                            );
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

                            if (isShelfPort(cmd.HOSTDESTINATION))
                            {
                                string dest = cmd.HOSTDESTINATION.Trim();

                                CassetteData destBoxData = new CassetteData();
                                destBoxData.CSTID = CarrierReadFail(dest);
                                destBoxData.BOXID = CarrierReadFail(dest);
                                destBoxData.Carrier_LOC = dest;

                                NotAccountHaveRead(destBoxData);
                            }
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
                        else if (cmd.TRANSFERSTATE == E_TRAN_STATUS.Queue && string.IsNullOrWhiteSpace(cmd.RelayStation) == false)
                        {
                            cmdBLL.updateCMD_MCS_CmdStatus(cmd.CMD_ID, 0);
                            break;
                        }

                        if (cmd.COMMANDSTATE == COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE)
                        {
                            cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);

                            CassetteData dbCstData = cassette_dataBLL.loadCassetteDataByLoc(ohtName.Trim());

                            if (dbCstData != null)
                            {
                                CassetteData ohtBoxData = new CassetteData();
                                ohtBoxData.CSTID = "ERROR1";
                                ohtBoxData.BOXID = cmd.CARRIER_ID_ON_CRANE.Trim();
                                ohtBoxData.Carrier_LOC = ohtName.Trim();
                                ohtBoxData = IDRead(ohtBoxData);

                                OHT_IDRead_Mismatch(cmd, ohtBoxData, dbCstData);
                            }
                            else
                            {
                                #region Log
                                TransferServiceLogger.Info
                                (
                                    DateTime.Now.ToString("HH:mm:ss.fff ")
                                    + "OHT >> OHB|COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH 卡匣不在車上:"
                                    + GetCmdLog(cmd)
                                );
                                #endregion
                            }
                        }

                        EmptyShelf();   //每次命令結束，檢查儲位狀態
                        break;
                    #endregion
                    #region 異常流程
                    case COMMAND_STATUS_BIT_INDEX_DOUBLE_STORAGE: //二重格異常
                        OHBC_AlarmSet(ohtName, SCAppConstants.SystemAlarmCode.OHT_Issue.DoubleStorage);
                        OHBC_AlarmCleared(ohtName, SCAppConstants.SystemAlarmCode.OHT_Issue.DoubleStorage);

                        reportBLL.ReportTransferAbortInitiated(cmd.CMD_ID); //  20/07/15 美微 說不要報 InterlockError 要報AbortInitiated、AbortCompleted
                        reportBLL.ReportTransferAbortCompleted(cmd.CMD_ID);
                        //reportBLL.ReportTransferCompleted(cmd, null, ResultCode.InterlockError);

                        string cstID = CarrierDouble(ohtCmd.DESTINATION.Trim());
                        string boxID = CarrierDouble(ohtCmd.DESTINATION.Trim());
                        string loc = ohtCmd.DESTINATION;

                        OHBC_InsertCassette(cstID, boxID, loc, "二重格異常");

                        cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);
                        break;
                    case COMMAND_STATUS_BIT_INDEX_EMPTY_RETRIEVAL: //空取異常
                        OHBC_AlarmSet(ohtName, SCAppConstants.SystemAlarmCode.OHT_Issue.EmptyRetrieval);
                        OHBC_AlarmCleared(ohtName, SCAppConstants.SystemAlarmCode.OHT_Issue.EmptyRetrieval);

                        reportBLL.ReportTransferAbortInitiated(cmd.CMD_ID); //  20/07/15 美微 說不要報 InterlockError 要報AbortInitiated、AbortCompleted
                        reportBLL.ReportTransferAbortCompleted(cmd.CMD_ID);
                        //reportBLL.ReportTransferCompleted(cmd, null, ResultCode.InterlockError);

                        CassetteData emptyData = cassette_dataBLL.loadCassetteDataByLoc(ohtCmd.SOURCE.Trim());

                        reportBLL.ReportCarrierRemovedCompleted(emptyData.CSTID, emptyData.BOXID);

                        cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);
                        break;
                    case COMMAND_STATUS_BIT_INDEX_InterlockError:
                        reportBLL.ReportCraneIdle(ohtName, cmd.CMD_ID);
                        reportBLL.ReportTransferCompleted(cmd, null, ResultCode.InterlockError);

                        cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);
                        break;
                    case COMMAND_STATUS_BIT_INDEX_VEHICLE_ABORT:
                        reportBLL.ReportCraneIdle(ohtName, cmd.CMD_ID);
                        reportBLL.ReportTransferCompleted(cmd, null, ResultCode.InterlockError);   //  20/04/13 MCS 反應說不要報 1 ，改報64

                        cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);
                        EmptyShelf();
                        break;
                    #endregion
                    default:
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "OHT_TransferProcess");
                return false;
            }
        }
        public void OHT_ScanProcess(ACMD_MCS cmd, string ohtName, int status)
        {
            try
            {
                switch (status)
                {
                    case COMMAND_STATUS_BIT_INDEX_ENROUTE:
                        reportBLL.ReportCraneActive(cmd.CMD_ID, ohtName);
                        cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.Transferring);
                        break;
                    case COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE: //入料完成
                        //CassetteData sourceCstData = cassette_dataBLL.loadCassetteDataByLoc(cmd.HOSTSOURCE.Trim());
                        //cassette_dataBLL.UpdateCSTLoc(sourceCstData.BOXID, ohtName, 1);
                        break;
                    case COMMAND_STATUS_BIT_INDEX_UNLOAD_COMPLETE: //出料完成
                        CassetteData dbCstData = cassette_dataBLL.loadCassetteDataByLoc(cmd.HOSTSOURCE.Trim());

                        #region Log
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ")
                            + "OHT >> OHB|OHT_ScanProcess 找到" + GetCstLog(dbCstData)
                        );
                        #endregion

                        cmdBLL.updateCMD_MCS_TranStatus(cmd.CMD_ID, E_TRAN_STATUS.TransferCompleted);

                        CassetteData ScanCstData = null;

                        if (string.IsNullOrWhiteSpace(cmd.CARRIER_ID_ON_CRANE) == false)  //SCAN 流程，車子給空值表示無料
                        {
                            ScanCstData = new CassetteData();
                            ScanCstData.CSTID = "ERROR1";
                            ScanCstData.BOXID = cmd.CARRIER_ID_ON_CRANE.Trim();
                            ScanCstData.Carrier_LOC = cmd.HOSTDESTINATION.Trim(); ;
                            ScanCstData = IDRead(ScanCstData);
                        }

                        if (ScanCstData != null && dbCstData != null)
                        {
                            OHT_IDRead_Mismatch(cmd, ScanCstData, dbCstData);
                        }
                        else if (ScanCstData != null && dbCstData == null)   //無帳有料
                        {
                            reportBLL.ReportCarrierIDRead(ScanCstData, ScanCstData.ReadStatus);

                            ScanCstData.Carrier_LOC = cmd.HOSTDESTINATION.Trim();

                            NotAccountHaveRead(ScanCstData);
                        }
                        else if (ScanCstData == null && dbCstData != null)   //有帳無料
                        {
                            HaveAccountNotReal(dbCstData);
                        }
                        break;
                    case COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH: //命令完成

                        reportBLL.ReportCraneIdle(ohtName, cmd.CMD_ID);

                        break;
                    case COMMAND_STATUS_BIT_INDEX_DOUBLE_STORAGE: //二重格異常

                        break;
                    case COMMAND_STATUS_BIT_INDEX_EMPTY_RETRIEVAL: //空取異常
                        CassetteData emptyCstData = cassette_dataBLL.loadCassetteDataByLoc(cmd.HOSTSOURCE.Trim());

                        if (emptyCstData != null)
                        {
                            HaveAccountNotReal(emptyCstData);
                        }

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

                        string loadCstID = "";
                        string loadBoxID = "";

                        if (dbCstData != null)
                        {
                            cassette_dataBLL.UpdateCSTLoc(dbCstData.BOXID, ohtName, 1);
                            loadCstID = dbCstData.CSTID;
                            loadBoxID = dbCstData.BOXID.Trim();
                            DeleteCst(dbCstData.CSTID, dbCstData.BOXID, "OHT_TestProcess");
                        }
                        else
                        {
                            loadCstID = CarrierReadFail(ohtName);
                            loadBoxID = ohtCmdData.BOX_ID.Trim();

                            if (ohtCmdData.BOX_ID.Trim().Contains("ERROR1") || string.IsNullOrWhiteSpace(ohtCmdData.BOX_ID.Trim()))
                            {
                                loadBoxID = CarrierReadFail(ohtCmdData.DESTINATION.Trim());
                            }
                        }

                        OHBC_InsertCassette(loadCstID, loadBoxID, ohtName, "test入料完成");

                        break;
                    case COMMAND_STATUS_BIT_INDEX_UNLOAD_ARRIVE:
                        break;
                    case COMMAND_STATUS_BIT_INDEX_UNLOADING:   //出料進行中
                        break;
                    case COMMAND_STATUS_BIT_INDEX_UNLOAD_COMPLETE: //出料完成
                        CassetteData dbDestData = cassette_dataBLL.loadCassetteDataByLoc(ohtName);

                        if (dbDestData != null)
                        {
                            string unLoadCstID = dbDestData?.CSTID ?? "";
                            string unLoadBoxID = dbDestData?.BOXID ?? "";

                            DeleteCst(dbDestData.CSTID, dbDestData.BOXID, "OHT_TestProcess");

                            if (isShelfPort(ohtCmdData.DESTINATION))
                            {
                                OHBC_InsertCassette(unLoadCstID, unLoadBoxID, ohtCmdData.DESTINATION, "test出料完成");
                            }
                        }
                        else
                        {
                            if (isShelfPort(ohtCmdData.DESTINATION))
                            {
                                TransferServiceLogger.Info
                                (
                                    DateTime.Now.ToString("HH:mm:ss.fff ")
                                    + "OHT >> OHB|OHT_TestProcess 出料完成發現沒帳在車上，且目的在 Shelf 自動產生 SCAN 到 " + ohtCmdData.DESTINATION
                                );

                                SetScanCmd("", "", ohtCmdData.DESTINATION);
                            }
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

                    if (cassette_dataBLL.UpdateCSTLoc(loadCstData.BOXID, ohtName, 1))
                    {
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
                                + "OHT_LoadCompleted MCS_CMD = Null"
                            );
                        }
                    }
                }
                else
                {
                    if (ohtCmd == null)
                    {
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ")
                            + "OHT_LoadCompleted ohtCmd = Null"
                        );
                    }

                    if (loadCstData == null)
                    {
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ")
                            + "OHT_LoadCompleted loadCstData = Null"
                        );
                    }
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

                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "OHT_UnLoadCompleted 誰呼叫:" + sourceCmd + " " + ohtName + " UnLoading:" + portINIData[ohtName].craneUnLoading
                );

                if (ohtCmd != null && unLoadCstData != null)
                {
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

                            if (unLoadCstData.CSTID.Contains("UNKF") && unLoadCstData.BOXID.Contains("UNKF"))   //20_0804 冠皚提出，放到儲位 CSTID、BOXID 讀不到時，將 CSTID 改成 UNKKU
                            {
                                CassetteData addCassetteData = unLoadCstData.Clone();
                                reportBLL.ReportCarrierRemovedCompleted(unLoadCstData.CSTID, unLoadCstData.BOXID);

                                addCassetteData.CSTID = CarrierReadFailAtTargetAGV(addCassetteData.Carrier_LOC);
                                OHBC_InsertCassette(addCassetteData.CSTID, addCassetteData.BOXID, addCassetteData.Carrier_LOC, addCassetteData.Carrier_LOC + " OHT_UnLoadCompleted CST、BOXID讀不到");
                            }
                            QueryLotID(unLoadCstData);
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
                    if (ohtCmd == null)
                    {
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ")
                            + "OHT_UnLoadCompleted ohtCmd = Null"
                        );
                    }

                    if (unLoadCstData == null)
                    {
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ")
                            + "OHT_UnLoadCompleted unLoadCstData = Null"
                        );
                    }
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
        public void OHT_IDRead(string mcsCmdID, string ohtName, string readBOXID, BCRReadResult bcrReadResult)
        {
            TransferServiceLogger.Info
            (
                DateTime.Now.ToString("HH:mm:ss.fff ") + "OHT_IDRead"
                + " mcsCmdID: " + mcsCmdID
                + " OHT_IDRead: " + ohtName
                + " ReadBOXID: " + readBOXID
                + " IDReadStatus: " + bcrReadResult
            );

            readBOXID = readBOXID.Trim();

            if (mcsCmdID.Contains("SCAN"))
            {
                return;
            }

            CassetteData dbCstData = cassette_dataBLL.loadCassetteDataByLoc(ohtName);

            if (dbCstData == null)
            {
                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHT_IDRead 沒有帳在:" + ohtName);

                if (string.IsNullOrWhiteSpace(readBOXID) || readBOXID == "ERROR1")
                {
                    readBOXID = CarrierReadFail(ohtName);
                }

                CassetteData ohtBoxData = new CassetteData();
                ohtBoxData.CSTID = "ERROR1";
                ohtBoxData.BOXID = readBOXID;
                ohtBoxData.Carrier_LOC = ohtName.Trim();
                ohtBoxData = IDRead(ohtBoxData);

                NotAccountHaveRead(ohtBoxData);
            }
        }
        public void OHT_IDRead_Mismatch(ACMD_MCS cmd, CassetteData ohtBoxData, CassetteData dbCstData)
        {
            if (dbCstData != null)
            {
                if (ohtBoxData.BOXID != dbCstData.BOXID)
                {
                    if (isCVPort(cmd.HOSTSOURCE.Trim()))
                    {
                        if (OHT_MismatchData.ContainsKey(ohtBoxData.BOXID))
                        {
                            OHT_BOXID_MismatchData mismatchData = OHT_MismatchData[ohtBoxData.BOXID];
                            DateTime mismatchDate = mismatchData.TriggerTime;
                            TimeSpan timeSpan = DateTime.Now - mismatchDate;

                            if (timeSpan.TotalMinutes < ohtID_MismatchTimeOut && mismatchData.CmdSourcePort == cmd.HOSTSOURCE.Trim())
                            {
                                ohtBoxData.CSTID = CarrierReadFailAtTargetAGV(ohtBoxData.Carrier_LOC);
                            }

                            OHT_MismatchData[ohtBoxData.BOXID].TriggerTime = DateTime.Now;
                            OHT_MismatchData[ohtBoxData.BOXID].CmdSourcePort = cmd.HOSTSOURCE.Trim();
                        }
                        else
                        {
                            OHT_BOXID_MismatchData addMismatchData = new OHT_BOXID_MismatchData();
                            addMismatchData.BOXID = ohtBoxData.BOXID;
                            addMismatchData.CmdSourcePort = cmd.HOSTSOURCE.Trim();
                            addMismatchData.TriggerTime = DateTime.Now;

                            OHT_MismatchData.Add(addMismatchData.BOXID, addMismatchData);
                        }
                    }

                    IDreadStatus idReadStatus = (IDreadStatus)int.Parse(ohtBoxData.ReadStatus);
                    string resultCode = ResultCode.Successful;

                    #region Log
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ")
                        + "OHT >> OHB|OHT BOX 讀取異常:" + idReadStatus
                        + "\n" + GetCmdLog(cmd)
                        + "\nDBData:" + GetCstLog(dbCstData)
                        + "\nOHTRead:" + GetCstLog(ohtBoxData)
                    );
                    #endregion


                    if (idReadStatus == IDreadStatus.duplicate)
                    {
                        resultCode = ResultCode.DuplicateID;
                    }
                    else if (idReadStatus == IDreadStatus.failed)
                    {
                        resultCode = ResultCode.IDReadFailed;
                    }
                    else
                    {
                        if (dbCstData.BOXID.Contains("UNKF"))
                        {
                            ohtBoxData.CSTID = dbCstData.CSTID;
                        }

                        resultCode = ResultCode.BoxID_Mismatch;
                        idReadStatus = IDreadStatus.mismatch;
                    }

                    reportBLL.ReportCarrierIDRead(ohtBoxData, ((int)idReadStatus).ToString());

                    if (cmd.CMD_ID.Contains("SCAN") == false)
                    {
                        reportBLL.ReportTransferCompleted(cmd, dbCstData, resultCode);
                    }

                    if (idReadStatus == IDreadStatus.failed)
                    {
                        ohtBoxData.CSTID = dbCstData.CSTID;
                    }

                    HaveAccountHaveReal(dbCstData, ohtBoxData, idReadStatus);
                }
            }
        }
        #endregion

        #region PLC >> OHB

        public void PLC_ReportPortIsModeChangable(PortPLCInfo plcInfo, string sourceCmd)
        {
            try
            {
                if (cassette_dataBLL == null)
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "PLC >> OHB|PLC_ReportPortIsModeChangable cassette_dataBLL == null"
                    );
                    return;
                }
                if (swapTriggerWaitin == false)
                {
                    return;
                }
                // 若為 output mode 空盒 + cst remove check 則轉in 
                if (plcInfo.LoadPosition1 == true && plcInfo.IsCSTPresence == false && plcInfo.IsOutputMode == true && plcInfo.IsModeChangable == true && plcInfo.CSTPresenceMismatch == false)
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ")
                        + "PLC >> OHB|PLC_ReportPortIsModeChangable 誰呼叫:" + sourceCmd + "  " + "自動 out 轉 in "
                    );
                    PortTypeChange(plcInfo.EQ_ID, E_PortType.In, "PLC_ReportPortIsModeChangable");
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "PLC_ReportPortIsModeChangable");
            }
        }
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

                    if (dbCSTData.BOXID == cstData.BOXID)
                    {
                        if (isUnitType(cstData.Carrier_LOC, UnitType.AGV)
                         || isUnitType(cstData.Carrier_LOC, UnitType.NTB)
                          )
                        {
                            if (dbCSTData.CSTID == cstData.CSTID)
                            {
                                TransferServiceLogger.Info
                                (
                                    DateTime.Now.ToString("HH:mm:ss.fff ")
                                    + "PLC >> OHB|CSTID、BOXID 相同跳出 PortWaitIn："
                                    + "\ndatainfo " + GetCstLog(cstData)
                                    + "\ndbCSTData" + GetCstLog(dbCSTData)
                                );

                                return;
                            }
                        }
                        else
                        {
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ")
                                + "PLC >> OHB|BOXID 相同跳出 PortWaitIn："
                                + "\ndatainfo " + GetCstLog(cstData)
                                + "\ndbCSTData" + GetCstLog(dbCSTData)
                            );

                            return;
                        }
                    }

                    ACMD_MCS dbMcsdata = cmdBLL.getCMD_ByBoxID(dbCSTData.BOXID);
                    if (dbMcsdata != null)
                    {
                        if (dbMcsdata.TRANSFERSTATE != E_TRAN_STATUS.Queue)
                        {
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ")
                                + "PLC >> OHB|PLC_ReportPortWaitIn 此筆卡匣已有命令在搬："
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

                if (isUnitType(cstData.Carrier_LOC, UnitType.AGV))
                {
                    string agvZoneName = portINIData[cstData.Carrier_LOC].Group;

                    OHBC_AGV_HasCmdsAccessCleared(agvZoneName);
                }
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
                    + "PLC >> OHB|PortPositionWaitOut 誰呼叫:" + sourceCmd + " outStage: " + outStage
                    + GetCstLog(datainfo)
                );

                datainfo.Carrier_LOC = datainfo.Carrier_LOC.Trim();

                UPStage(datainfo, outStage, "PortPositionWaitOut");
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

            string portLoc = GetPositionName(plcInfo.EQ_ID.Trim(), position);
            SetPortWaitOutTimeOutAlarm(portLoc, 0);
        }
        public void PortCstPositionOFF(PortPLCInfo plcInfo)
        {
            if (isUnitType(plcInfo.EQ_ID.Trim(), UnitType.AGV))
            {
                CassetteData datainfo = new CassetteData();
                datainfo.CSTID = plcInfo.CassetteID.Trim();        //填CSTID
                datainfo.BOXID = plcInfo.BoxID.Trim();        //填BOXID
                datainfo.Carrier_LOC = plcInfo.EQ_ID.Trim();  //填Port 名稱

                PortCarrierRemoved(datainfo, plcInfo.IsAGVMode, "PortCstPositionOFF");
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

                UPStage(cstData, outStage, "PortWaitOut (Demo)");
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "AGVPortWaitOut");
            }
        }
        public bool UPStage(CassetteData outData, int outStage, string sourceCmd)
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
                    + " 誰呼叫:" + sourceCmd
                    + " outStage:" + outStage
                    + " outData :" + GetCstLog(outData)
                );

                List<CassetteData> dbDataList = cassette_dataBLL.LoadCassetteDataByCVPort(outData.Carrier_LOC);

                CassetteData dbData = dbDataList.Where(data => data.BOXID.Trim() == outData.BOXID.Trim()).FirstOrDefault();

                if (dbData == null)
                {
                    //如果沒找到PLC給的BOX卡匣，就依序搬入順序來找卡匣
                    dbData = dbDataList.Where(cst => cst.Stage < outStage)
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
                        if (outStage == 1)
                        {
                            OHTtoPort(outData.Carrier_LOC, outStage, "UPStage");
                        }
                        else
                        {
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ")
                                + outData.Carrier_LOC + "第 2 節後不在去找車子"
                            );
                        }

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
                    "PLC >> OHB|UPStage：PortID:" + portInI.PortName + " 此 CV 節數:" + portStage
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
            else if (stage > portINIData[portName].Stage)
            {
                positionName = "";
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
                string agvZone = portINIData[portName].Group;

                string log = "OHB >> OHB|";

                CassetteData dbData = null;
                CassetteData old_dbData = null; //舊帳，補報 MCS 用

                ACMD_MCS cmd = null;

                if (ohtName != "Error")
                {
                    log = log + ohtName + " 停在 " + portName;

                    dbData = cassette_dataBLL.loadCassetteDataByLoc(ohtName);
                }
                else
                {
                    log = log + " 沒有車子停在 " + portName + " 前面";
                }

                #region 找卡匣
                log = log + "\n 開始找卡匣在哪";

                if (dbData != null)
                {
                    log = log + " 在 " + ohtName + " 車上找到 :" + GetCstLog(dbData);

                    old_dbData = dbData.Clone();
                }
                else
                {
                    log = log + "，找不到帳";
                }

                #endregion

                #region 找命令

                #region 第一，用卡匣找
                if (dbData != null)
                {
                    log = log + "\n用卡匣找命令";
                    cmd = cmdBLL.getCMD_ByBoxID(dbData.BOXID);

                    if (cmd != null)
                    {
                        if (cmd.HOSTDESTINATION.Trim() != portName.Trim())
                        {
                            cmd = null;
                            log = log + "，找到命令，但命令目的錯誤" + GetCmdLog(cmd);
                        }
                        else
                        {
                            log = log + "，找到命令" + GetCmdLog(cmd);
                        }
                    }
                    else
                    {
                        log = log + "，找不到命令";
                    }
                }
                #endregion
                #region 第二，用車子找命令

                if (cmd == null)
                {
                    List<ACMD_MCS> ohtCmdList = cmdBLL.getCMD_ByOHTName(ohtName);
                    log = log + "\n用車子找命令 OHTName: " + ohtName + " 執行: " + ohtCmdList.Count() + " 筆命令";

                    cmd = ohtCmdList.Where(data => (data.HOSTDESTINATION == portName || data.HOSTDESTINATION == agvZone)
                                                 && data.TRANSFERSTATE == E_TRAN_STATUS.Transferring
                                                 && data.COMMANDSTATE >= COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE
                                                   ).FirstOrDefault();

                    if (cmd != null)
                    {
                        log = log + "，找到命令" + GetCmdLog(cmd);
                    }
                }

                #endregion
                #region 第三，找命令目的，發生車子回報位置導致找不到停在 Port 前面

                if (cmd == null)
                {
                    List<ACMD_MCS> cmdByDest = cmdBLL.GetCmdDataByDest(portName)
                                                        .Where(data => data.TRANSFERSTATE == E_TRAN_STATUS.Transferring)
                                                        .ToList();

                    log = log + "\n找搬送命令目的到 " + portName + " 找到 " + cmdByDest.Count().ToString() + " 筆";

                    #region 命令目的只有一個
                    if (cmdByDest.Count() == 1) //只有一筆命令，就找那一筆
                    {
                        cmd = cmdByDest.FirstOrDefault();

                        log = log + "，只有1筆，找到 " + GetCmdLog(cmd);
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

                    if (dbData == null)
                    {
                        dbData = cassette_dataBLL.loadCassetteDataByBoxID(cmd.BOX_ID);

                        if (dbData != null)
                        {
                            log = log + "\n用命令找卡匣 BOXID: " + cmd.BOX_ID + " 找到 :" + GetCstLog(dbData);
                        }
                    }
                }
                else
                {
                    log = log + " 找不到命令";
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

                    ACMD_OHTC ohtData = cmdBLL.getCMD_OHTCByMCScmdID_And_NotFinishByDest(cmd.CMD_ID, cmd.HOSTDESTINATION);

                    OHT_UnLoadCompleted(ohtData, dbData, "OHTtoPort");
                }
                else
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") + log
                    );

                    #region 過帳處理

                    if (dbData != null)
                    {
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ") + "OHTtoPort 有帳，沒命令，自動刪帳"
                        );

                        DeleteCst(dbData.CSTID, dbData.BOXID, "OHTtoPort");

                        //dbData.Carrier_LOC = GetPositionName(portName, 1);
                        //cassette_dataBLL.UpdateCSTLoc(dbData.BOXID, dbData.Carrier_LOC, 1);

                        //PortPLCInfo portInfo = GetPLC_PortData(portName);

                        //if(portInfo.PortWaitOut)
                        //{
                        //    reportBLL.ReportCarrierWaitOut(dbData, "1");
                        //}
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

                        ACMD_OHTC ohtData = cmdBLL.getCMD_OHTCByMCScmdID_And_NotFinishBySource(cmd.CMD_ID, cmd.HOSTSOURCE);

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
                    "PLC >> OHB|PortCarrierRemoved  誰呼叫:" + cmdSource + GetCstLog(cstData)
                );

                int stage = portINIData[cstData.Carrier_LOC.Trim()].Stage;

                CassetteData dbData = cassette_dataBLL.LoadCassetteDataByCVPort(cstData.Carrier_LOC)
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

                ACMD_MCS cmd = cmdBLL.getByCstBoxID(dbData.CSTID, dbData.BOXID);    //0727 發生AGV Port 退BOX，BOX 在席滅掉，OHT再報入料完成，發生刪錯帳
                if (cmd != null)
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "PLC >> OHB|PortCarrierRemoved  刪帳失敗，有命令再執行:" + GetCmdLog(cmd)
                    );
                    return;
                }

                reportBLL.ReportCarrierRemovedFromPort(dbData, HandoffType);

                cassette_dataBLL.DeleteCSTbyCstBoxID(dbData.CSTID, dbData.BOXID);

                if (isUnitType(dbData.Carrier_LOC, UnitType.AGV))
                {
                    if (isAGV)
                    {
                        PLC_AGV_Station(GetPLC_PortData(dbData.Carrier_LOC), "PortCarrierRemoved");
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


            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "ReportPortType");
            }
        }
        public void PLC_ReportRunDwon(PortPLCInfo plcInfo, string cmdSource)
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
                    "PLC >> OHB|PLC_ReportRunDwon"
                    + " PortName:" + portName
                    + " Service:" + service
                    + " 誰呼叫:" + cmdSource
                );

                if (isUnitType(portName, UnitType.AGV) && service == E_PORT_STATUS.InService)
                {
                    PLC_AGV_Station(plcInfo, "PLC_ReportRunDwon");
                }
                else
                {
                    PortInOutService(portName, service, "PLC_ReportRunDwon");
                }

                if (isUnitType(portName, UnitType.AGV))
                {
                    string AGVZone = portINIData[portName].Group;

                    PLC_AGVZone_InOutService(AGVZone);
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "PLC_ReportRunDwon");
            }
        }
        public void PortInOutService(string portName, E_PORT_STATUS service, string apiSource)
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
                            + " PortName:" + portName
                            + " Service:" + service
                            + " 誰呼叫:" + apiSource
                        );

                        if (service == E_PORT_STATUS.InService)
                        {
                            if (isUnitType(portDB.PLCPortID, UnitType.AGV))
                            {
                                if (portDB.PortType == E_PortType.In)
                                {
                                    reportBLL.ReportTypeInput(portDB.PLCPortID.Trim());

                                    ACMD_MCS agvStationCmd = cmdBLL.GetCmdDataByAGVtoSHELF(portDB.PLCPortID);

                                    if (agvStationCmd != null)
                                    {
                                        #region Log
                                        TransferServiceLogger.Info
                                        (
                                            DateTime.Now.ToString("HH:mm:ss.fff ")
                                            + " PLC >> OHB|取消退空 BOX 命令  " + GetCmdLog(agvStationCmd)
                                        );
                                        #endregion

                                        if (agvStationCmd.TRANSFERSTATE == E_TRAN_STATUS.Queue)
                                        {
                                            Manual_DeleteCmd(agvStationCmd.CMD_ID, "AGV 狀態 InService，取消退空 BOX 命令");
                                            //DeleteCst(agvStationCmd.CARRIER_ID, agvStationCmd.BOX_ID, "AGV 狀態 InService，取消退空 BOX 命令");
                                        }
                                    }
                                }

                                if (portDB.PortType == E_PortType.Out)
                                {
                                    reportBLL.ReportPortTypeOutput(portDB.PLCPortID.Trim());

                                    ACMD_MCS agvStationCmd = cmdBLL.GetCmdDataBySHELFtoAGV(portDB.PLCPortID);

                                    if (agvStationCmd != null)
                                    {
                                        #region Log
                                        TransferServiceLogger.Info
                                        (
                                            DateTime.Now.ToString("HH:mm:ss.fff ")
                                            + " PLC >> OHB|取消補空 BOX 命令  " + GetCmdLog(agvStationCmd)
                                        );
                                        #endregion

                                        if (agvStationCmd.TRANSFERSTATE == E_TRAN_STATUS.Queue)
                                        {
                                            Manual_DeleteCmd(agvStationCmd.CMD_ID, "AGV 狀態 InService，取消補空 BOX 命令");
                                        }
                                    }
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
                        if (isUnitType(portName, UnitType.AGV) && service == E_PORT_STATUS.InService)
                        {
                            string AGVZone = portINIData[portName].Group;

                            if (portINIData[AGVZone].openAGVZone == E_PORT_STATUS.InService)
                            {
                                OpenAGV_Station(portName, false, "PortInOutService");
                            }
                        }

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
                TransferServiceLogger.Error(ex, "PortInOutService");
            }
        }

        public void PortCIM_ON(PortPLCInfo plcInfo, string apiSource)
        {
            TransferServiceLogger.Info
            (
                DateTime.Now.ToString("HH:mm:ss.fff ") +
                "OHB >> DB|PortCIM_ON: "
                + "    PortName:" + plcInfo.EQ_ID
                + "    CIM_ON:" + plcInfo.cim_on
                + "    誰呼叫:" + apiSource
            );

            if (plcInfo.cim_on)
            {
                OHBC_AlarmCleared(plcInfo.EQ_ID, ((int)AlarmLst.PORT_CIM_OFF).ToString());

                if (plcInfo.OpError == true)
                {
                    OHBC_AlarmSet(plcInfo.EQ_ID, plcInfo.ErrorCode.ToString());
                }

                if (plcInfo.OpAutoMode)
                {
                    OHBC_AlarmCleared(plcInfo.EQ_ID, ((int)AlarmLst.PORT_DOWN).ToString());
                }
                else
                {
                    OHBC_AlarmSet(plcInfo.EQ_ID, ((int)AlarmLst.PORT_DOWN).ToString());
                }

                PLC_ReportRunDwon(plcInfo, "PortCIM_ON");
            }
            else
            {
                OHBC_AlarmAllCleared(plcInfo.EQ_ID);

                OHBC_AlarmSet(plcInfo.EQ_ID, ((int)AlarmLst.PORT_CIM_OFF).ToString());
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
                    //TransferServiceLogger.Info
                    //(
                    //    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    //    "PLC >> OHB|PLC_AGV_Station PortID:" + plcInfo.EQ_ID
                    //    + " IsReadyToLoad:" + plcInfo.IsReadyToLoad
                    //    + " IsReadyToUnload:" + plcInfo.IsReadyToUnload
                    //);
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
                        PortInOutService(plcInfo.EQ_ID, E_PORT_STATUS.InService, "PLC_AGV_Station");
                    }
                }
                else
                {
                    PortInOutService(plcInfo.EQ_ID, E_PORT_STATUS.OutOfService, "PLC_AGV_Station");
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
                string agvToShelfLog = "";

                E_PORT_STATUS status = E_PORT_STATUS.OutOfService;

                if (plcInfo.LoadPosition1) //portData.LoadPosition1 = BOX 在席                
                {
                    if (plcInfo.IsCSTPresence)  //portData.IsCSTPresence = CST 在席)
                    {
                        if (plcInfo.CSTPresenceMismatch)
                        {
                            agvToShelfLog = "PLC >> OHB|PLC_AGV_Station_InMode "
                                + plcInfo.EQ_ID + " 退實箱"
                                + " LoadPosition1:" + plcInfo.LoadPosition1
                                + " IsCSTPresence:" + plcInfo.IsCSTPresence
                                + " CSTPresenceMismatch:" + plcInfo.CSTPresenceMismatch;

                            agvToShelf = true;
                        }
                        else
                        {
                            if (plcInfo.PortWaitIn)
                            {
                                CassetteData dbCSTData = cassette_dataBLL.loadCassetteDataByLoc(plcInfo.EQ_ID.Trim());

                                if (dbCSTData == null)
                                {
                                    #region Log
                                    TransferServiceLogger.Info
                                    (
                                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                                        "PLC >> OHB|PLC_AGV_Station_InMode " + plcInfo.EQ_ID + " 發現:PortWaitIn = " + plcInfo.PortWaitIn + "，沒帳"
                                    );
                                    #endregion

                                    //PLC_ReportPortWaitIn(plcInfo, "PLC_AGV_Station_InMode");   20_0818 SCC+ 
                                    /*
                                     * PLC_ReportPortWaitIn(plcInfo, "PLC_AGV_Station_InMode"); 
                                       此功能為 0817_1241 之後發生，PortWaitIn 亮的時候，RUN 沒亮，造成沒有上報 WaitIn 給 MCS
                                       ，擔心 PLC_AGV_Station_InMode 與實際觸發 WaitIn 訊號造成衝突，造時註解掉
                                       ，柏裕提出：理論上，PortWaitIn 訊號亮的時候，RUN 訊號一定會亮，此問題待觀察
                                       ，若下次在發生時，PLC 那邊還是無法解決時，此功能再把註解拿掉，另外做預防重複上報的機制
                                    */
                                }
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
                            }

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
                                    + plcInfo.EQ_ID
                                    + " BOXID: " + plcInfo.BoxID
                                    + " BOX ID 讀不到，退空BOX"
                                    + " AGVPortReady:" + plcInfo.AGVPortReady
                                );
                                #endregion

                                agvToShelf = true;
                            }
                            else
                            {
                                status = E_PORT_STATUS.InService;

                                CassetteData dbCSTData = cassette_dataBLL.loadCassetteDataByLoc(plcInfo.EQ_ID.Trim());

                                if (dbCSTData != null)
                                {
                                    DeleteCst(dbCSTData.CSTID, dbCSTData.BOXID, "AGV_Port_InMode_空 BOX 不留帳");
                                }
                            }
                        }
                        else
                        {
                            #region Log
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ") +
                                "PLC >> OHB|PLC_AGV_Station_InMode" + plcInfo.EQ_ID
                                + " AGVPortReady 沒報"
                                + " AGVPortReady:" + plcInfo.AGVPortReady
                                + " LoadPosition1:" + plcInfo.LoadPosition1
                                + " IsCSTPresence:" + plcInfo.IsCSTPresence
                            );
                            #endregion
                        }
                    }
                }
                else
                {
                    shelfToAGV = true;
                }

                PortInOutService(plcInfo.EQ_ID, status, "PLC_AGV_Station_InMode");

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

                    if (DelayMoveBackBox(plcInfo.EQ_ID, E_PortType.In))
                    {
                        PLC_AGV_Station_InMode(GetPLC_PortData(plcInfo.EQ_ID));
                        return;
                    }

                    #region Log
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") + agvToShelfLog
                    );
                    #endregion

                    MovebackBOX(plcInfo.CassetteID, plcInfo.BoxID, plcInfo.EQ_ID, plcInfo.IsCSTPresence, "PLC_AGV_Station_InMode");
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
                    else
                    {
                        //reportBLL.ReportEmptyBoxSupply("1", portData.EQ_ID);
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
                string movebackBOXLog = "";

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

                                        if (agvWaitOutOpenBox && line.LINE_ID.Contains("LINE"))
                                        {
                                            SetAGV_PortOpenBOX(plcInfo.EQ_ID.Trim(), "AGV PortWaitOut");
                                        }

                                        waitOut = true;
                                    }
                                    else
                                    {
                                        movebackBOXLog =
                                            "OHB >> AGV|PLC_AGV_Station_OutMode "
                                            + plcInfo.EQ_ID + " 退BOX:   CSTID不符"
                                            + " AGVRead: " + plcInfo.CassetteID.Trim()
                                            + " dbCstData: " + dbCstData.CSTID.Trim();

                                        movebackBOX = true;

                                        OpenAGV_Station(plcInfo.EQ_ID, true, "AGV Port CSTID 比對異常");
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
                                    "OHB >> AGV|PLC_AGV_Station_OutMode dbCstData = Null ，先建帳 " + plcInfo.EQ_ID
                                    + " PortWaitOut:" + plcInfo.PortWaitOut
                                    + " IsOutputMode:" + plcInfo.IsOutputMode
                                    + " IsReadyToLoad:" + plcInfo.IsReadyToLoad
                                    + " IsReadyToUnload:" + plcInfo.IsReadyToUnload
                                );

                                CassetteData agvCSTData = new CassetteData();
                                agvCSTData.CSTID = plcInfo.CassetteID;
                                agvCSTData.BOXID = plcInfo.BoxID;
                                agvCSTData.Carrier_LOC = plcInfo.EQ_ID;
                                agvCSTData = IDRead(agvCSTData);

                                OHBC_InsertCassette(agvCSTData.CSTID, agvCSTData.BOXID, agvCSTData.Carrier_LOC, "PLC_AGV_Station_OutMode");
                            }
                        }
                        else if (plcInfo.CSTPresenceMismatch)
                        {
                            movebackBOXLog = "OHB >> AGV|PLC_AGV_Station_OutMode "
                                + plcInfo.EQ_ID + " 退BOX:CSTPresenceMismatch:" + plcInfo.CSTPresenceMismatch;

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
                        movebackBOXLog = "OHB >> AGV|PLC_AGV_Station_OutMode " + plcInfo.EQ_ID + "退空BOX";

                        movebackBOX = true;
                    }
                }
                else
                {
                    movebackBOX = false;
                    status = E_PORT_STATUS.InService;

                    CassetteData dbCSTData = cassette_dataBLL.loadCassetteDataByLoc(plcInfo.EQ_ID.Trim());

                    if (dbCSTData != null)
                    {
                        DeleteCst(dbCSTData.CSTID, dbCSTData.BOXID, "AGV_Port_OutMode_BOX 在席沒亮，刪除殘帳");
                    }
                }

                PortInOutService(plcInfo.EQ_ID, status, "PLC_AGV_Station_OutMode");

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

                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") + movebackBOXLog
                );

                if (movebackBOX)    //退BOX
                {
                    if (CheckPortType(plcInfo.EQ_ID))
                    {
                        return;
                    }

                    if (DelayMoveBackBox(plcInfo.EQ_ID, E_PortType.Out))
                    {
                        PLC_AGV_Station_OutMode(GetPLC_PortData(plcInfo.EQ_ID));
                        return;
                    }

                    MovebackBOX(plcInfo.CassetteID, plcInfo.BoxID, plcInfo.EQ_ID, plcInfo.IsCSTPresence, "PLC_AGV_Station_OutMode");

                    string agvZoneName = portINIData[plcInfo.EQ_ID].Group;  //0729 SCC+ 冠皚提出退BOX，檢查是否有命令到此 agvZone ，有就把優先權調最高

                    List<ACMD_MCS> cmdDateToDest = cmdBLL.GetCmdDataByDest(agvZoneName).OrderBy(cmd => cmd.CMD_INSER_TIME).ToList();

                    foreach (var v in cmdDateToDest)
                    {
                        cmdBLL.updateCMD_MCS_Priority(v, 99);
                    }
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "PLC_AGV_Station_OutMode");
            }
        }
        public void PLC_AGVZone_InOutService(string agvZoneName)
        {
            E_PORT_STATUS agvZoneInOutService = E_PORT_STATUS.OutOfService;

            PortDef agvZone = portDefBLL.GetPortData(agvZoneName.Trim());

            if (agvZone_ConnectedRealAGVPortRunDown)
            {
                if (portINIData[agvZone.PLCPortID].openAGVZone == E_PORT_STATUS.InService)
                {
                    foreach (PortINIData agvPort in GetAGVPort(agvZone.PLCPortID))
                    {
                        OpenAGV_Station(agvPort.PortName, false, "PLC_AGVZone_InOutService");

                        PortPLCInfo agvInfo = GetPLC_PortData(agvPort.PortName);

                        if (agvInfo.OpAutoMode)
                        {
                            agvZoneInOutService = E_PORT_STATUS.InService;
                        }
                    }
                }
            }
            else
            {
                agvZoneInOutService = (E_PORT_STATUS)agvZone.AGVState;
            }

            if (agvZone.State != agvZoneInOutService)
            {
                PortInOutService(agvZone.PLCPortID, agvZoneInOutService, "PLC_AGVZone_InOutService");
            }
        }
        #region 補BOX、退BOX，控制處理

        public bool DelayMoveBackBox(string portName, E_PortType portType)
        {
            portName = portName.Trim();

            if (portINIData[portName].movebackBOXsleep)
            {
                portINIData[portName].movebackBOXsleep = false;
                return false;
            }
            else
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHB >> AGV|" + portType + "Mode 退BOX 延遲 300 毫秒再檢查一次 " + portName
                );
                portINIData[portName].movebackBOXsleep = true;
                Thread.Sleep(300);

                return true;
            }
        }
        public void MovebackBOX(string cstID, string boxID, string cstLoc, bool cstPresence, string sourceCmd)
        {
            try
            {
                #region Log
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "退BOX "
                    + " 誰呼叫" + sourceCmd
                    + " cstID:" + cstID
                    + " boxID: " + boxID
                    + " cstLoc:" + cstLoc
                );
                #endregion

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
                        else
                        {
                            return;
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

                        CassetteData dbData = cassette_dataBLL.loadCassetteDataByLoc(emptyBoxData.Carrier_LOC);

                        if (dbData != null)
                        {
                            DeleteCst(dbData.CSTID, dbData.BOXID, "退 BOX 前先刪除舊帳");
                        }

                        OHBC_InsertCassette(emptyBoxData.CSTID, emptyBoxData.BOXID, emptyBoxData.Carrier_LOC, "退BOX");
                    }

                    if (isUnitType(cmdDest, UnitType.AGV))
                    {
                        reportBLL.ReportPortTypeOutput(cmdDest);
                    }

                    Manual_InsertCmd(cmdSource, cmdDest, 45, "BoxMovCmd", CmdType.AGVStation);
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
                    if (isAGVZone(portID))
                    {
                        foreach (var v in GetAGVPort(portID))
                        {
                            PortValueDefMapAction portValueDefMapAction = scApp.getEQObjCacheManager().getPortByPortID(v.PortName).getMapActionByIdentityKey(typeof(PortValueDefMapAction).Name) as PortValueDefMapAction;

                            portValueDefMapAction.Port_OHCV_Commanding(Commanding);
                        }
                    }
                    else
                    {
                        PortValueDefMapAction portValueDefMapAction = scApp.getEQObjCacheManager().getPortByPortID(portID).getMapActionByIdentityKey(typeof(PortValueDefMapAction).Name) as PortValueDefMapAction;

                        portValueDefMapAction.Port_OHCV_Commanding(Commanding);
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "Port_OHCV_Commanding portID:" + portID + " Commanding:" + Commanding);
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
        public bool SetAGV_PortOpenBOX(string portID, string sourceCmd)
        {
            try
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHB >> PLC|SetAGV_PortOpenBOX"
                    + " portID:" + portID
                    + " 誰呼叫:" + sourceCmd
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
                Thread.Sleep(500);
                portValueDefMapAction.Port_BCR_Read(false);
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

        public bool PortBCR_Enable(string portID, bool enable)
        {
            try
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHB >> PLC|PortBCR_Enable"
                    + "    portID:" + portID
                    + "    Enable:" + enable
                );

                PortValueDefMapAction portValueDefMapAction = scApp.getEQObjCacheManager().getPortByPortID(portID).getMapActionByIdentityKey(typeof(PortValueDefMapAction).Name) as PortValueDefMapAction;

                portValueDefMapAction.Port_BCR_Enable(enable);
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
            return "UNKF" + loc.Trim() + GetStDate() + string.Format("{0:00}", DateTime.Now.Second);
        }
        public string CarrierReadFailAtTargetAGV(string loc)   //卡匣讀不到
        {
            return "UNKU" + loc.Trim() + GetStDate() + string.Format("{0:00}", DateTime.Now.Second);
        }
        public string CarrierReadduplicate(string bcrcsid)  //卡匣重複
        {
            return "UNKD" + bcrcsid + GetStDate() + string.Format("{0:00}", DateTime.Now.Second);
        }
        public bool ase_ID_Check(string str)    //ASE CST BOX 帳料命名規則
        {
            str = str.Trim();

            bool b = false;

            if (str.Length == 8)
            {
                string str12 = str.Substring(0, 2); //1、2碼為數字
                string str34 = str.Substring(2, 2); //3、4碼為英文
                string str58 = str.Substring(4, 4); //5~8碼為數字 + 英文混合

                if (IsNumber(str12) && IsEnglish(str34) && IsEnglish_Number(str58) && IsEnglish_Number(str))
                {
                    b = true;
                }
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
            else
            {
                if (cmdData.TRANSFERSTATE == E_TRAN_STATUS.Queue)
                {
                    reportBLL.ReportOperatorInitiatedAction(cmdData.CMD_ID, reportMCSCommandType.Cancel.ToString());
                    scApp.VehicleService.doCancelOrAbortCommandByMCSCmdID(cmdData.CMD_ID, CMDCancelType.CmdCancel);
                }
                else if (cmdData.TRANSFERSTATE == E_TRAN_STATUS.Canceling)
                {
                    ForcedEndCmd(cmdData);
                }
                else if (cmdData.TRANSFERSTATE == E_TRAN_STATUS.Aborting)
                {
                    ForcedEndCmd(cmdData);
                }
                else
                {
                    if (cmdData.COMMANDSTATE < COMMAND_STATUS_BIT_INDEX_UNLOAD_COMPLETE)
                    {
                        reportBLL.ReportOperatorInitiatedAction(cmdData.CMD_ID, reportMCSCommandType.Abort.ToString());
                        scApp.VehicleService.doCancelOrAbortCommandByMCSCmdID(cmdData.CMD_ID, CMDCancelType.CmdAbort);
                    }
                    else
                    {
                        #region Log
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ") +
                            "Manual >> OHB|強制結束，命令狀態(COMMANDSTATE)為： " + cmdData.COMMANDSTATE
                        );
                        #endregion

                        cmdBLL.updateCMD_MCS_TranStatus(cmdData.CMD_ID, E_TRAN_STATUS.TransferCompleted);
                    }
                }
            }
        }
        public void ForcedEndCmd(ACMD_MCS cmdData)
        {
            AVEHICLE vehicle = scApp.VehicleService.GetVehicleDataByVehicleID(cmdData.CRANE.Trim());

            bool deleteCon = false;

            if (vehicle != null)
            {
                if (vehicle.MCS_CMD.Trim() != cmdData.CMD_ID.Trim())
                {
                    #region Log
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "Manual >> OHB| " + vehicle.VEHICLE_ID.Trim()
                        + " 執行：" + vehicle.MCS_CMD.Trim()
                        + " 命令不一致強制結束：" + GetCmdLog(cmdData)
                    );
                    #endregion

                    deleteCon = true;
                }
                else
                {
                    #region Log
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "Manual >> OHB|強制結束 " + vehicle.MCS_CMD.Trim() + " 失敗，" + vehicle.VEHICLE_ID + " 正在執行" + GetCmdLog(cmdData)
                    );
                    #endregion
                }
            }
            else
            {
                #region Log
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "Manual >> OHB| 找不到車子" + cmdData.CRANE.Trim()
                    + "強制結束" + GetCmdLog(cmdData)
                );
                #endregion

                deleteCon = true;
            }

            if (deleteCon)
            {
                cmdBLL.updateCMD_MCS_TranStatus(cmdData.CMD_ID, E_TRAN_STATUS.TransferCompleted);

                if (cmdData.TRANSFERSTATE == E_TRAN_STATUS.Canceling)
                {
                    reportBLL.ReportTransferCancelCompleted(cmdData.CMD_ID);
                }
                else if (cmdData.TRANSFERSTATE == E_TRAN_STATUS.Aborting)
                {
                    scApp.ReportBLL.ReportTransferAbortCompleted(cmdData.CMD_ID);
                }
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
                if (isUnitType(readData.Carrier_LOC, UnitType.AGV)
                 || isUnitType(readData.Carrier_LOC, UnitType.NTB))
                {
                    scApp.TransferService.OHBC_AlarmSet(readData.Carrier_LOC, ((int)AlarmLst.PORT_CSTID_READ_FAIL).ToString());
                    scApp.TransferService.OHBC_AlarmCleared(readData.Carrier_LOC, ((int)AlarmLst.PORT_CSTID_READ_FAIL).ToString());
                }

                readData.CSTID = CarrierReadFail(readData.Carrier_LOC);

                if (isUnitType(readData.Carrier_LOC, UnitType.AGV))
                {
                    PortDef dbPortDef = portDefBLL.GetPortData(readData.Carrier_LOC);
                    if (dbPortDef.AGVState == E_PORT_STATUS.InService)
                    {
                        readData.CSTID = CarrierReadFailAtTargetAGV(readData.Carrier_LOC);
                    }
                }

                carrierIDFail = true;
            }

            if (readData.BOXID.Contains("ERROR1") || readData.BOXID.Contains("NORD01") || string.IsNullOrWhiteSpace(readData.BOXID))
            {
                //B0.03
                scApp.TransferService.OHBC_AlarmSet(readData.Carrier_LOC, ((int)AlarmLst.PORT_BOXID_READ_FAIL).ToString());
                scApp.TransferService.OHBC_AlarmCleared(readData.Carrier_LOC, ((int)AlarmLst.PORT_BOXID_READ_FAIL).ToString());
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
            CassetteData duCarrierID = cassette_dataBLL.loadCassetteDataByDU_CstID(readData);
            CassetteData duBoxID = cassette_dataBLL.loadCassetteDataByDU_BoxID(readData);

            if ((duCarrierID != null && string.IsNullOrWhiteSpace(readData.CSTID) == false) || duBoxID != null)
            {
                bool insertCassetteDuCst = true;

                ACMD_MCS nowCmd = cmdBLL.getCMD_ByBoxID(cstData.BOXID);

                if (nowCmd != null)
                {
                    TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "卡匣重複，發現搬送中命令 " + GetCmdLog(nowCmd));
                }

                if (duCarrierID != null && string.IsNullOrWhiteSpace(cstData.CSTID) == false)
                {
                    if (nowCmd != null || isShelfPort(duCarrierID.Carrier_LOC) == false)
                    {
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ") + " CSTID 重複 "
                            + "\nIDRead:" + GetCstLog(cstData)
                            + "\nDB_CST_Data:" + GetCstLog(duCarrierID)
                        );
                        readData.CSTID = CarrierReadduplicate(cstData.CSTID);
                        readData.BOXID = CarrierReadFail(cstData.Carrier_LOC);
                        insertCassetteDuCst = false;
                    }
                }

                if (duBoxID != null)
                {
                    if (nowCmd != null || isShelfPort(duBoxID.Carrier_LOC) == false)
                    {
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ") + " BOXID 重複 "
                            + "\nIDRead:" + GetCstLog(cstData)
                            + "\nDB_CST_Data:" + GetCstLog(duBoxID)
                        );
                        readData.CSTID = CarrierReadFail(cstData.Carrier_LOC);
                        readData.BOXID = CarrierReadduplicate(cstData.BOXID);
                        insertCassetteDuCst = false;
                    }
                }

                if (insertCassetteDuCst)
                {
                    idReadStatus = IDreadStatus.duplicate;
                }
            }

            #endregion

            readData.ReadStatus = ((int)idReadStatus).ToString();

            return readData;
        }
        public void QueryLotID(CassetteData cstData)
        {
            if (cstData != null)
            {
                if (ase_ID_Check(cstData.CSTID) && string.IsNullOrWhiteSpace(cstData.LotID))
                {
                    reportBLL.ReportQueryLotID(cstData.CSTID);
                }
            }
        }
        #region 異常流程
        public void HaveAccountHaveReal(CassetteData dbData, CassetteData bcrcsid, IDreadStatus idRead)      //有帳有料
        {
            try
            {
                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "CarrierAbnormal: 有帳有料");
                CassetteData newData = new CassetteData();

                newData = bcrcsid.Clone();
                //newData.ReadStatus = ((int)ACMD_MCS.IDreadStatus.successful).ToString();

                if (idRead == IDreadStatus.duplicate)
                {
                    //newData.CSTID = CarrierReadFail(newData.Carrier_LOC.Trim());
                    OHBC_InsertCassette(newData.CSTID, newData.BOXID, newData.Carrier_LOC, "有帳有料 " + idRead);
                    //Duplicate(bcrcsid);
                }
                else if (idRead == IDreadStatus.mismatch
                    || idRead == IDreadStatus.failed
                    || idRead == IDreadStatus.BoxReadFail_CstIsOK
                    || idRead == IDreadStatus.CSTReadFail_BoxIsOK
                        )
                {
                    //if (idRead == IDreadStatus.CSTReadFail_BoxIsOK)
                    //{
                    //    newData.CSTID = CarrierReadFail(newData.Carrier_LOC.Trim());
                    //}

                    //if (idRead == IDreadStatus.BoxReadFail_CstIsOK)
                    //{
                    //    newData.BOXID = CarrierReadFail(newData.Carrier_LOC.Trim());
                    //    newData.CSTID = newData.BOXID;
                    //}

                    //if (idRead == IDreadStatus.mismatch)
                    //{
                    //    newData.BOXID = bcrcsid.BOXID;
                    //    newData.CSTID = CarrierReadFail(newData.Carrier_LOC.Trim());
                    //}

                    if (newData.BOXID.Contains("UNKF") && isUnitType(dbData.Carrier_LOC, UnitType.CRANE)
                      )
                    {
                        cassette_dataBLL.DeleteCSTbyCstBoxID(dbData.CSTID, dbData.BOXID);
                    }
                    else
                    {
                        reportBLL.ReportCarrierRemovedCompleted(dbData.CSTID, dbData.BOXID);
                    }

                    OHBC_InsertCassette(newData.CSTID, newData.BOXID, newData.Carrier_LOC, "有帳有料 " + idRead);
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
            TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "CarrierAbnormal: " + bcrcsid.Carrier_LOC + " 無帳有料");
            OHBC_InsertCassette(bcrcsid.CSTID, bcrcsid.BOXID, bcrcsid.Carrier_LOC, bcrcsid.Carrier_LOC + " 無帳有料");
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
            CassetteData duCarrID = cassette_dataBLL.loadCassetteDataByDU_CstID(bcrData);
            CassetteData duBoxID = cassette_dataBLL.loadCassetteDataByDU_BoxID(bcrData);

            if (duBoxID != null && duBoxID.Carrier_LOC != bcrData.Carrier_LOC)    //BOXID 重複
            {
                newCstData = duBoxID.Clone();
                newCstData.CSTID = CarrierReadFail(newCstData.Carrier_LOC); //20/07/16 美微說 CSTID 要變 UNKF
                newCstData.BOXID = CarrierReadduplicate(duBoxID.BOXID);

                if (duCarrID != null)   //同個BOX，CSTID 重複
                {
                    if (duCarrID.Carrier_LOC == duBoxID.Carrier_LOC)
                    {
                        newCstData.CSTID = CarrierReadduplicate(bcrData.CSTID);
                    }

                    reportBLL.ReportCarrierRemovedCompleted(duBoxID.CSTID, duBoxID.BOXID);
                    OHBC_InsertCassette(newCstData.CSTID, newCstData.BOXID, newCstData.Carrier_LOC, "Duplicate");
                    return;
                }

                reportBLL.ReportCarrierRemovedCompleted(duBoxID.CSTID, duBoxID.BOXID);
                OHBC_InsertCassette(newCstData.CSTID, newCstData.BOXID, newCstData.Carrier_LOC, "BOX Duplicate");
            }

            if (duCarrID != null && duCarrID.Carrier_LOC != bcrData.Carrier_LOC && string.IsNullOrWhiteSpace(bcrData.CSTID) == false)    //CSTID 重複
            {
                newCstData = duCarrID.Clone();
                newCstData.CSTID = CarrierReadduplicate(bcrData.CSTID);
                newCstData.BOXID = CarrierReadFail(newCstData.Carrier_LOC); //20/07/16 美微說 CSTID 要變 UNKF

                reportBLL.ReportCarrierRemovedCompleted(duCarrID.CSTID, duCarrID.BOXID);
                OHBC_InsertCassette(newCstData.CSTID, newCstData.BOXID, newCstData.Carrier_LOC, "CSTID Duplicate");
            }
        }
        #endregion

        #endregion
        #region 卡匣建帳、刪帳

        public string OHBC_InsertCassette(string cstid, string boxid, string loc, string sourceAPI)
        {
            try
            {
                loc = loc.Trim();
                cstid = cstid.Trim();
                boxid = boxid.Trim();
                //lotID = lotID.Trim();

                #region Log
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "PLC >> OHB|OHBC_InsertCassette 誰呼叫: " + sourceAPI
                );
                #endregion

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
                datainfo.CSTID = Redis_GetCstID(cstid, boxid);
                datainfo.BOXID = boxid;
                datainfo.Carrier_LOC = loc;
                datainfo.LotID = "";
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

                        if (cassette_dataBLL.insertCassetteData(datainfo))
                        {
                            reportBLL.ReportCarrierInstallCompleted(datainfo);
                            reportBLL.ReportZoneCapacityChange(portName, null);

                            QueryLotID(datainfo);
                        }
                    }
                    else if (isUnitType(portName, UnitType.CRANE))
                    {
                        datainfo.CSTState = E_CSTState.Installed;
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
        public void DeleteOHCVPortCst(string portName, string apiSource)  //刪除 OHCV Port 上的所有卡匣
        {
            TransferServiceLogger.Info
            (
                DateTime.Now.ToString("HH:mm:ss.fff ") +
                "OHB >> DB|DeleteOHCVPortCst 誰呼叫:" + apiSource + "  刪除: " + portName + "  上所有卡匣"
            );

            if (iniStatus)
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
                            && (data.BOXID.Contains("UNKF") == false)   //200623 SCC+ 不要補 "UNKF" (讀不到) 的 空BOX 到 AGVPort 
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

        //bool GetShelfRecentLocationIng = false;
        private long GetShelfRecentLocationIng = 0;
        public string GetShelfRecentLocation(List<ShelfDef> shelfData, string portLoc)  //取得最近儲位
        {
            if (Interlocked.Exchange(ref GetShelfRecentLocationIng, 1) == 0)
            {
                try
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
                                + "OHB >> OHB|GetShelfRecentLocation 已有命令搬到此 " + v.ShelfID + " 儲位 " + GetCmdLog(cmdData)
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

                        OHBC_AlarmSet(line.LINE_ID, ((int)AlarmLst.LINE_NotEmptyShelf).ToString());
                    }

                    return shelfName;
                }
                catch (Exception ex)
                {
                    TransferServiceLogger.Error(ex, "GetShelfRecentLocation 找離 " + portLoc + "最近儲位");
                    return "";
                }
                finally
                {
                    Interlocked.Exchange(ref GetShelfRecentLocationIng, 0);
                }
            }
            else
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "OHB >> OHB|GetShelfRecentLocation interlock 中 回傳空值"
                );

                return "";
            }
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

        public string OpenAGV_Station(string portName, bool open, string sourceCmd)
        {
            if (portINIData[portName].openAGV_Station != open)
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHB >> OHB|開關自動退補BOX功能 portID:" + portName + " 動作:" + open + " 誰呼叫: " + sourceCmd
                );
            }

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
                bool b = false;

                if (portINIData != null)
                {
                    if (isLocExist(portName))
                    {
                        if (unitType.ToString().Trim() == portINIData[portName.Trim()].UnitType)
                        {
                            b = true;
                        }
                    }
                }

                return b;
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
                 || isAGVZone(portName)
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
                string log = "  ACMD_MCS:";

                if (cmdData != null)
                {
                    log = log + "   CMD_ID:" + cmdData?.CMD_ID.Trim() ?? "";
                    log = log + "   來源:" + cmdData?.HOSTSOURCE.Trim() ?? "";
                    log = log + "   目的:" + cmdData?.HOSTDESTINATION.Trim() ?? "";
                    log = log + "   中繼站:" + cmdData?.RelayStation?.Trim() ?? "";
                    log = log + "   CST_ID:" + cmdData?.CARRIER_ID.Trim() ?? "";
                    log = log + "   BOX_ID:" + cmdData?.BOX_ID.Trim() ?? "";
                    log = log + "   OHT_BCR_Read:" + cmdData?.CARRIER_ID_ON_CRANE?.Trim() ?? "";
                    log = log + "   CMD_TRANSFERSTATE:" + cmdData?.TRANSFERSTATE ?? "";
                    log = log + "   CMD_SOURCE:" + cmdData?.CMDTYPE?.Trim() ?? "";
                    log = log + "   CRANE:" + cmdData?.CRANE?.Trim() ?? "";
                }
                else
                {
                    log = log + "   ACMD_MCS = Null";
                }

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
                string log = "  CassetteData:";
                if (cstData != null)
                {
                    log = log + "   CSTID:" + cstData.CSTID?.Trim() ?? "";
                    log = log + "   BOXID:" + cstData.BOXID?.Trim() ?? "";
                    log = log + "   Carrier_LOC:" + cstData.Carrier_LOC?.Trim() ?? "";
                    log = log + "   Stage:" + cstData?.Stage ?? "";
                    log = log + "   CSTState:" + cstData?.CSTState ?? "";
                }
                else
                {
                    log = log + "   CassetteData = Null";
                }

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
                string log = " ACMD_OHTC:";
                log = log + " OHT_CmdID:" + ohtCmdData?.CMD_ID ?? "";
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
            log = log + " IsReadyToLoad:" + plcInfo?.IsReadyToLoad ?? "";
            log = log + " IsReadyToUnload:" + plcInfo?.IsReadyToUnload ?? "";
            log = log + " AGVPortReady:" + plcInfo?.AGVPortReady ?? "";
            log = log + " CSTPresenceMismatch:" + plcInfo?.CSTPresenceMismatch ?? "";
            log = log + " LoadPosition1:" + plcInfo?.LoadPosition1 ?? "";
            log = log + " IsCSTPresence:" + plcInfo?.IsCSTPresence ?? "";
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
        public void OHBC_AlarmSetIng(string _eqName, bool ing)
        {
            portINIData[_eqName].alarmSetIng = ing;
            TransferServiceLogger.Info
            (
                DateTime.Now.ToString("HH:mm:ss.fff ") +
                "OHB >> OHB|OHBC_AlarmSetIng "
                + "    eqName: " + _eqName
                + "    ing: " + ing
            );
        }
        public void OHBC_AlarmSet(string _eqName, string errCode)
        {
            try
            {
                OHBC_AlarmSetIng(_eqName, true);

                string eqName = _eqName.Trim();
                errCode = errCode.Trim();

                string s = DateTime.Now.ToString() + " " + eqName + " " + errCode;

                if (alarmBug.Contains(s))
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "OHB >> OHB|OHBC_AlarmSet 短時間內觸發"
                        + "    ohtName:" + eqName
                        + "    errCode:" + errCode
                        + "    DateTime.Now:" + s
                    );
                    return;
                }
                else
                {
                    alarmBug = s;
                }

                #region Log
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHT >> OHB|AlarmSet:"
                    + "    EQ_Name:" + eqName.Trim()
                    + "    OHT_AlarmID:" + errCode
                );

                if (errCode == "0")
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "OHT >> OHB|errCode = 0 判斷無異常跳回"
                    );

                    OHBC_AlarmSetIng(_eqName, false);

                    return;
                }
                #endregion

                ACMD_MCS mcsCmdData = cmdBLL.getCMD_ByOHTName(eqName).FirstOrDefault();

                ALARM alarm = scApp.AlarmBLL.setAlarmReport(null, eqName, errCode, mcsCmdData);

                if (alarm != null)
                {
                    //if (isUnitType(alarm.EQPT_ID, UnitType.CRANE) || isUnitType(alarm.EQPT_ID, UnitType.LINE) || alarm.EQPT_ID.Contains("LINE"))
                    //{
                    if (alarm.ALAM_LVL == E_ALARM_LVL.Error)
                    {
                        //if(alarmBLL.loadSetAlarmListByEqName(alarmEq).Count == 1)
                        //{
                        //    reportBLL.ReportAlarmHappend(ErrorStatus.ErrSet, alarm.ALAM_CODE, alarm.ALAM_DESC);
                        //}
                        reportBLL.ReportAlarmHappend(ErrorStatus.ErrSet, alarm.ALAM_CODE, alarm.ALAM_DESC);
                        reportBLL.ReportAlarmSet(mcsCmdData, alarm, alarm.UnitID, alarm.UnitState, alarm.RecoveryOption);
                    }
                    else if (alarm.ALAM_LVL == E_ALARM_LVL.Warn)
                    {
                        reportBLL.ReportUnitAlarmSet(alarm.EQPT_ID, alarm.ALAM_CODE, alarm.ALAM_DESC);
                    }
                    //}
                    //else
                    //{
                    //    reportBLL.ReportUnitAlarmSet(alarm.EQPT_ID, alarm.ALAM_CODE, alarm.ALAM_DESC);
                    //}
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "OHT_AlarmSet   ohtName:" + _eqName + " ErrorCode:" + errCode);
            }
            finally
            {
                OHBC_AlarmSetIng(_eqName, false);
            }
        }
        public void OHBC_AlarmCleared(string _craneName, string errCode)
        {
            try
            {
                SpinWait.SpinUntil(() => portINIData[_craneName].alarmSetIng == false, 5000);

                string craneName = _craneName.Trim();
                errCode = errCode.Trim();

                if (scApp.AlarmBLL == null)
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") + "OHT >> OHB|AlarmBLL = null"
                    );
                    return;
                }

                #region Log
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHT >> OHB|AlarmCleared:"
                    + "    EQ_Name:" + craneName
                    + "    OHT_AlarmID:" + errCode
                );
                #endregion

                ACMD_MCS mcsCmdData = cmdBLL.getCMD_ByOHTName(craneName).FirstOrDefault();

                if (mcsCmdData == null)
                {
                    mcsCmdData = new ACMD_MCS();
                    mcsCmdData.CMD_ID = "";
                }
                string alarmEq = craneName;

                //if (isAGVZone(craneName))
                //{
                //    alarmEq = craneName.Remove(0, 12);
                //}

                ALARM alarm = scApp.AlarmBLL.loadAlarmByAlarmID(alarmEq, errCode);

                if (alarm != null)
                {
                    string eqID = alarm.EQPT_ID.Trim();

                    //if (isUnitType(alarm.EQPT_ID, UnitType.CRANE) || isUnitType(alarm.EQPT_ID, UnitType.LINE) || alarm.EQPT_ID.Contains("LINE"))
                    //{
                    //if (isUnitType(craneName, UnitType.CRANE) == false)
                    //{
                    //    alarm.EQPT_ID = "";
                    //    alarm.UnitID = "";
                    //}

                    if (alarm.ALAM_LVL == E_ALARM_LVL.Error)
                    {
                        reportBLL.ReportAlarmCleared(mcsCmdData, alarm, alarm.UnitID.Trim(), alarm.UnitState.Trim());
                        scApp.ReportBLL.ReportAlarmHappend(ErrorStatus.ErrReset, alarm.ALAM_CODE.Trim(), alarm.ALAM_DESC.Trim());
                        //if (alarmBLL.loadSetAlarmListByEqName(eqID).Count == 1)
                        //{
                        //    scApp.ReportBLL.ReportAlarmHappend(ErrorStatus.ErrReset, alarm.ALAM_CODE.Trim(), alarm.ALAM_DESC.Trim());
                        //}
                    }
                    else if (alarm.ALAM_LVL == E_ALARM_LVL.Warn)
                    {
                        reportBLL.ReportUnitAlarmCleared(alarm.EQPT_ID, alarm.ALAM_CODE, alarm.ALAM_DESC);
                    }
                    //}
                    //else
                    //{
                    //    reportBLL.ReportUnitAlarmCleared(alarm.EQPT_ID, alarm.ALAM_CODE, alarm.ALAM_DESC);
                    //}

                    scApp.AlarmBLL.resetAlarmReport(eqID, alarm.ALAM_CODE);

                    if (alarm.ALAM_CODE.Contains(((int)AlarmLst.OHT_QueueCmdTimeOut).ToString()))
                    {
                        foreach (var v in queueCmdTimeOutCmdID)
                        {
                            queueCmdTimeOutCmdID.Remove(v);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "OHT_AlarmCleared   ohtName:" + _craneName + " ErrorCode:" + errCode);
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
                TransferServiceLogger.Error(ex, "OHT_AlarmAllCleared " + craneName);
                return false;
            }
        }

        public void OHBC_OHT_IDLE_HasCMD_TimeOutCleared()
        {
            if (cmdFailAlarmSet)
            {
                OHBC_AlarmCleared(line.LINE_ID, ((int)AlarmLst.OHT_IDLE_HasCMD_TimeOut).ToString());
                cmdFailAlarmSet = false;
            }
        }

        public void OHBC_AGV_HasCmdsAccessCleared(string agvZoneName)
        {
            if (iniStatus == false)
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "OHBC_AGV_HasCmdsAccessCleared iniStatus == false，初始化未完成"
                );
                return;
            }

            bool b = false;

            foreach (var v in GetAGVZone())
            {
                if (v.PortName == agvZoneName)
                {
                    if (v.agvHasCmdsAccess)
                    {
                        OHBC_AlarmCleared(v.PortName, ((int)AlarmLst.AGV_HasCmdsAccessTimeOut).ToString());
                    }

                    v.agvHasCmdsAccess = false;
                }

                b |= v.agvHasCmdsAccess;
            }

            agvHasCmdsAccess = b;
        }
        public void OHBC_OHT_QueueCmdTimeOutCmdIDCleared(string cmdID)
        {
            cmdID = cmdID.Trim();

            if (queueCmdTimeOutCmdID.Contains(cmdID))
            {
                queueCmdTimeOutCmdID.Remove(cmdID);

                if (queueCmdTimeOutCmdID.Count() == 0)
                {
                    OHBC_AlarmCleared(line.LINE_ID, ((int)AlarmLst.OHT_QueueCmdTimeOut).ToString());
                }
            }
        }

        public void SetPortWaitOutTimeOutAlarm(string locName, int errorStatus) // errorStatus : 0 = 清除，1 = 設置
        {
            string portName = portINIData[locName].ZoneName;
            string waitOutTimeAlarmCode = "";
            int locStage = 0;

            if (portINIData[locName].nowStage == portINIData[locName].Stage)
            {
                locStage = 7;
            }
            else
            {
                locStage = portINIData[locName].nowStage;
            }

            switch (locStage)
            {
                case 1:
                    waitOutTimeAlarmCode = ((int)AlarmLst.PORT_OP_WaitOutTimeOut).ToString();
                    break;
                case 2:
                    waitOutTimeAlarmCode = ((int)AlarmLst.PORT_BP1_WaitOutTimeOut).ToString();
                    break;
                case 3:
                    waitOutTimeAlarmCode = ((int)AlarmLst.PORT_BP2_WaitOutTimeOut).ToString();
                    break;
                case 4:
                    waitOutTimeAlarmCode = ((int)AlarmLst.PORT_BP3_WaitOutTimeOut).ToString();
                    break;
                case 5:
                    waitOutTimeAlarmCode = ((int)AlarmLst.PORT_BP4_WaitOutTimeOut).ToString();
                    break;
                case 6:
                    waitOutTimeAlarmCode = ((int)AlarmLst.PORT_BP5_WaitOutTimeOut).ToString();
                    break;
                case 7:
                    waitOutTimeAlarmCode = ((int)AlarmLst.PORT_LP_WaitOutTimeOut).ToString();
                    break;
                default:
                    break;
            }

            if (errorStatus == 1)
            {
                OHBC_AlarmSet(portName, waitOutTimeAlarmCode);
            }
            else
            {
                OHBC_AlarmCleared(portName, waitOutTimeAlarmCode);
            }
        }
        #endregion
        #region Redis 新增刪除查詢CSTID
        public void Redis_AddCstBox(CassetteData addRedisCstData)   //  20/06/30 SCC+ 建帳的時候去建立 BOX 跟 CST 的關係
        {
            try
            {
                if (ase_ID_Check(addRedisCstData.BOXID) && (ase_ID_Check(addRedisCstData.CSTID) || string.IsNullOrWhiteSpace(addRedisCstData.CSTID)))
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ")
                        + "Redis 新增對應關係 BOXID: " + addRedisCstData.BOXID
                        + " CSTID: " + addRedisCstData.CSTID
                    );
                    cassette_dataBLL.redis.setBoxIDWithCSTID(addRedisCstData.BOXID, addRedisCstData.CSTID); //2020/07/01 修改 交換順序 for 正確的key值
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "Redis_AddCstBox" + GetCstLog(addRedisCstData));
            }
        }
        public void Redis_DeleteCstBox(CassetteData deleteRedisCstData)
        {
            try
            {
                if (isUnitType(deleteRedisCstData.Carrier_LOC, UnitType.AGV))
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ")
                        + "Redis 刪除 BOXID: " + deleteRedisCstData.BOXID
                    );

                    cassette_dataBLL.redis.deleteCSTIDByBoxID(deleteRedisCstData.BOXID);
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "Redis_DeleteCstBox" + GetCstLog(deleteRedisCstData));
            }
        }

        public void Redis_UpdateCstID(CassetteData unkCstData)
        {
            try
            {
                if (redisEnable)
                {
                    if (unkCstData != null)
                    {
                        unkCstData.CSTID = Redis_GetCstID(unkCstData.CSTID, unkCstData.BOXID);

                        cassette_dataBLL.UpdateCSTID(unkCstData.Carrier_LOC, unkCstData.BOXID, unkCstData.CSTID, unkCstData.LotID);
                    }
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "Redis_UpdateCstID" + GetCstLog(unkCstData));
            }
        }
        public string Redis_GetCstID(string cstID, string boxID)
        {
            string redisCstID = cstID;

            if (redisEnable)
            {
                if (ase_ID_Check(boxID) && cstID.Contains("UNKF"))
                {
                    var redis = cassette_dataBLL.redis.tryGetCSTIDByBoxID(boxID);

                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ")
                        + "Redis 找 BOXID: " + boxID
                        + " 結果，hasExist: " + redis.hasExist
                        + " cstID: " + redis.cstID?.Trim() ?? ""
                    );

                    if (redis.hasExist)
                    {
                        redisCstID = redis.cstID;
                    }
                }
            }

            return redisCstID;
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

        public void Manual_SetOneInoneOutMethodUse(bool yn)
        {
            TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "Manual_SetOneInoneOutMethodUse: " + yn);
            oneInoneOutMethodUse = yn;
        }
        #endregion
        #region 命令操作
        public string Manual_InsertCmd(string source, string dest, int priority = 5, string sourceCmd = "UI", CmdType cmdType = CmdType.Manual)   //手動搬送，sourceCmd : 誰呼叫
        {
            try
            {
                #region Log
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHB >> CMD|Manual_InsertCmd"
                    + " 來源: " + source
                    + " 目的: " + dest
                    + " 誰呼叫: " + sourceCmd
                );
                #endregion

                CassetteData sourceData = cassette_dataBLL.loadCassetteDataByLoc(source);

                if (sourceData == null)
                {
                    string returnLog = "Source:" + source + " Cassette is not exist";
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "Manual >> OHB|Manual_InsertCmd " + returnLog
                    );

                    return returnLog;
                }

                CassetteData destData = cassette_dataBLL.loadCassetteDataByLoc(dest);

                if (destData != null)
                {
                    string returnLog = "dest:" + dest + " Has Cassette";
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "Manual >> OHB|Manual_InsertCmd " + returnLog
                    );

                    return returnLog;
                }

                if (portINIData.ContainsKey(source) == false
                 || portINIData.ContainsKey(dest) == false
                   )
                {
                    return "來源或目的不存在";
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

                datainfo.CARRIER_ID = sourceData.CSTID;
                datainfo.BOX_ID = sourceData.BOXID;
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
                datainfo.LOT_ID = sourceData.LotID?.Trim() ?? "";
                datainfo.CMDTYPE = cmdType.ToString();
                datainfo.CRANE = "";

                if (cmdBLL.getCMD_ByBoxID(datainfo.BOX_ID) != null)
                {
                    return datainfo.BOX_ID + " 已存在搬送命令";
                }

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

        public string Manual_UpDateCmdPriority(string cmdID, int priority, string cmdSource)
        {
            string s = "";
            if (cmdBLL.updateCMD_MCS_PortPriority(cmdID, priority))
            {
                s = "OK";
            }
            else
            {
                s = "失敗";
            }

            #region Log
            TransferServiceLogger.Info
            (
                DateTime.Now.ToString("HH:mm:ss.fff ") +
                "Manual >> OHB|Manual_UpDateCmdPriority: " + cmdID + " priority:" + priority + " 誰呼叫：" + cmdSource
            );
            #endregion

            return s;
        }
        #endregion
        #region 卡匣操作
        public string Manual_InsertCassette(string cstid, string boxid, string loc, bool checkCstCmd = false)  //手動建帳
        {
            if (checkCstCmd == true)
            {
                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> OHB| OHBC_InsertCassette checkCstCmd = true");
                OHBC_InsertCassette(cstid, boxid, loc, "Manual_InsertCassette");
            }
            if (string.IsNullOrWhiteSpace(cstid) == false)
            {
                if (boxid.Length != 8)
                {
                    return "CST_ID 不為 8 碼";
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
                return "BOX_ID 不為 8 碼";
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

            if (isShelfPort(loc))
            {
                ShelfDef shelfData = shelfDefBLL.loadShelfDataByID(loc);
                if (shelfData != null)
                {
                    if (shelfData.Enable != "Y")
                    {
                        return loc + " Enable:" + shelfData.Enable;
                    }
                }
                else
                {
                    return loc + " 不存在";
                }
            }

            TransferServiceLogger.Info
            (
                DateTime.Now.ToString("HH:mm:ss.fff ") +
                "OHB >> OHB|Manual_InsertCassette CSTID: " + cstid + "  BOXID:" + boxid + "   LOC:" + loc
            );

            return OHBC_InsertCassette(cstid, boxid, loc, "Manual_InsertCassette");
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
            PortInOutService(portName, service, "UI");
            return "OK";
        }
        public string Manual_SetPortPriority(string portName, int priority)
        {
            portDefBLL.updatePriority(portName, priority);
            return "OK";
        }
        public void Manual_OpenAGV_State(string AGVZone)
        {
            if (isAGVZone(AGVZone))
            {
                portDefBLL.UpdataAGVPortService(AGVZone, E_PORT_STATUS.InService);
                portINIData[AGVZone].openAGVZone = E_PORT_STATUS.InService;
                OpenAGVZone(AGVZone, E_PORT_STATUS.InService);
            }
        }
        public void Manual_CloseAGV_State(string AGVZone)
        {
            if (isAGVZone(AGVZone))
            {
                portDefBLL.UpdataAGVPortService(AGVZone, E_PORT_STATUS.OutOfService);
                portINIData[AGVZone].openAGVZone = E_PORT_STATUS.OutOfService;
                OpenAGVZone(AGVZone, E_PORT_STATUS.OutOfService);
            }
        }

        public void Manual_UseFirst2Port(string AGVZone)
        {
            if (isAGVZone(AGVZone))
            {
                portDefBLL.UpdataAGVSimPortLocationType(AGVZone, 1);
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 切換虛擬 port Zone: " + AGVZone + "使用前 2 Port.");
            }
        }

        public void Manual_UseLast2Port(string AGVZone)
        {
            if (isAGVZone(AGVZone))
            {
                portDefBLL.UpdataAGVSimPortLocationType(AGVZone, 2);
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 切換虛擬 port Zone: " + AGVZone + "使用後 2 Port.");
            }
        }

        public void OpenAGVZone(string AGVZone, E_PORT_STATUS status)
        {
            bool openAGVStation = false;    //是否開始自動退補BOX功能
            string agvZoneName = "";

            if (status == E_PORT_STATUS.OutOfService)
            {
                openAGVStation = true;
            }

            foreach (PortINIData agvPort in GetAGVPort(AGVZone))
            {
                agvZoneName = agvPort.Group;
                OpenAGV_Station(agvPort.PortName, openAGVStation, "OpenAGVZone");
            }

            PLC_AGVZone_InOutService(agvZoneName);
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
        public bool UpdateIgnoreModeChange(string portName, string enable)
        {
            bool IgnoreMode = false;

            if (isCVPort(portName))
            {
                if (portDefBLL.UpdateIgnoreModeChange(portName, enable))
                {
                    portINIData[portName].IgnoreModeChange = enable;
                    IgnoreMode = true;

                    if (enable == "Y")
                    {
                        PortInOutService(portName, E_PORT_STATUS.OutOfService, "UpdateIgnoreModeChange");
                    }
                    else
                    {
                        iniPortData(portName);
                    }
                }
            }

            return IgnoreMode;
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
                if (iniSetPortINIData == false)
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
        public void ReportNowPortType(string portName)  //上報目前 Port 流向
        {
            portName = portName.Trim();
            PortPLCInfo portNameInfo = GetPLC_PortData(portName);

            if (portNameInfo.IsInputMode)
            {
                ReportPortType(portNameInfo.EQ_ID, E_PortType.In, "ReportNowPortType");
            }

            if (portNameInfo.IsOutputMode)
            {
                ReportPortType(portNameInfo.EQ_ID, E_PortType.Out, "ReportNowPortType");
            }
        }
        #endregion
        #region 取得 Port 資料
        public List<PortINIData> GetCVPort()
        {
            return portINIData.Values.Where(data => data.Stage == data.nowStage
                                                 && (data.UnitType == UnitType.OHCV.ToString()
                                                    || data.UnitType == UnitType.STK.ToString()
                                                    || data.UnitType == UnitType.NTB.ToString()
                                                    || data.UnitType == UnitType.AGV.ToString()
                                                    )
                                           ).ToList();
        }
        public List<PortINIData> GetAGVPort(string agvZoneName)
        {
            return portINIData.Values.Where(data => data.Group == agvZoneName.Trim()
                                                 && data.UnitType == UnitType.AGV.ToString()
                                           ).OrderBy(loc => loc.PortName).ToList();
        }

        public List<PortINIData> GetAGVZone()
        {
            return portINIData.Values.Where(data => data.UnitType == UnitType.AGVZONE.ToString()).ToList();
        }
        public string GetAGV_InModeInServicePortName(string agvZone) //取得AGV ZONE 狀態為 InMode 且上面有空 BOX 的 AGV Port 名稱
        {
            string agvPortName = "";
            List<PortINIData> agvZoneData = GetAGVPort(agvZone);

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
                        break;
                    }
                }
            }

            return agvPortName;
        }

        public int GetAGV_InModeInServicePortName_NumberByHasEmptyBox(string agvZone) //取得AGV ZONE 狀態為 InMode 且上面有空 BOX 的 AGV Port 數量
        {
            List<PortINIData> agvZoneData = GetAGVPort(agvZone);
            int count = 0;
            if (agvZoneData.Count() != 0)
            {
                foreach (PortINIData agvPortData in agvZoneData)
                {
                    PortPLCInfo agvInfo = GetPLC_PortData(agvPortData.PortName);
                    if (agvInfo.IsInputMode
                        && agvInfo.IsReadyToUnload
                        && agvInfo.OpAutoMode
                        && agvInfo.LoadPosition1
                        && agvInfo.IsCSTPresence == false
                      )
                    {
                        count = count + 1;
                    }
                }
            }

            return count;
        }

        public string GetAGV_OutModeInServicePortName(string agvZone) //取得AGV ZONE 狀態為 OutMode 且上面沒有空 BOX 的 AGV Port 名稱
        {
            string agvPortName = "";
            List<PortINIData> agvZoneData = GetAGVPort(agvZone);

            if (agvZoneData.Count() != 0)
            {
                foreach (PortINIData agvPortData in agvZoneData)
                {
                    PortPLCInfo agvInfo = GetPLC_PortData(agvPortData.PortName);

                    if (agvInfo.IsOutputMode && AreDestEnable(agvPortData.PortName))
                    {
                        agvPortName = agvPortData.PortName;
                        break;
                    }

                    //PortPLCInfo agvInfo = GetPLC_PortData(agvPortData.PortName);
                    //if (agvInfo.IsOutputMode
                    //    && agvInfo.IsReadyToLoad
                    //    && agvInfo.OpAutoMode
                    //    && agvInfo.LoadPosition1 == false
                    //  )
                    //{
                    //    agvPortName = agvPortData.PortName;
                    //}
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
        /// Call by AGVC Restful API. Use for process the AGV Station. // A20.06.12.0 //A20.06.15.0 
        /// Change the default return from false to true. // A20.07.10.0
        /// </summary>
        /// A20.06.12.0 A20.06.15.0 A20.07.10.0
        /// <param name="AGVStationID"></param>
        /// <param name="AGVCFromEQToStationCmdNum"></param>
        /// <param name="isEmergency"></param>
        /// <returns></returns>
        public bool CanExcuteUnloadTransferAGVStationFromAGVC(string AGVStationID, int AGVCFromEQToStationCmdNum, bool isEmergency)
        {
            PortTypeNum portTypeNum = PortTypeNum.No_Change;
            bool isOK = false; //A20.07.10.0
            try
            {
                bool useFirst2Port = false;     //判斷是否使用前 2 port，若否使用後 2 port
                int numOfAGVStation = GetAGVPort(AGVStationID).Count();
                agvcTriggerAlarmCheck(AGVStationID, AGVCFromEQToStationCmdNum);
                //此AGVStation虛擬port是 Out of service 擇要拒絕AGVC
                PortDef portDefByAGVStationID = scApp.PortDefBLL.GetPortData(AGVStationID);
                if (portDefByAGVStationID.State == E_PORT_STATUS.OutOfService)
                {
                    //若為4個Port 的虛擬Port
                    if (numOfAGVStation == 4 && portDefByAGVStationID.AGVState == E_PORT_STATUS.OutOfService) //A20.07.10.0
                    {
                        isOK = true;
                        return isOK;
                    }
                    isOK = ChangeReturnDueToAGVCCmdNum(AGVCFromEQToStationCmdNum); //A20.07.10.0
                    portTypeNum = PortTypeNum.No_Change;
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " 是 Out of service 一律回復" + isOK);
                    RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                    return isOK;
                }
                //確認要取得的AGVStation Port 為前2還是後2 前為1 後為2
                useFirst2Port = IsUsingFirst2Port(portDefByAGVStationID);
                //取得PLC目前資訊
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Trigger start. Get the AGVSTation Data, " +
                    "AGVStationID = " + AGVStationID + ", AGVCFromEQToStationCmdNum = " + AGVCFromEQToStationCmdNum + ", isEmergency = " + isEmergency.ToString()
                     + " , 線上空盒數量 = " + GetTotalEmptyBoxNumber().emptyBox.Count().ToString());
                List<PortDef> AGVPortDatas = scApp.PortDefBLL.GetAGVPortGroupDataByStationID(line.LINE_ID, AGVStationID);
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Exit GetAGVPortGroupDataByStationID().");

                //確認目前的AGV port 是否有source 為它的取貨命令(若有，則一律回復否，避免先觸發退box後，卻因下一次觸發同意AGV來放貨)
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter CheckIsSourceFromAGVStation().");
                bool haveCmdFromAGVPort = CheckIsSourceFromAGVStation(AGVPortDatas);
                if (haveCmdFromAGVPort == true)
                {
                    isOK = ChangeReturnDueToAGVCCmdNum(AGVCFromEQToStationCmdNum); //A20.07.10.0
                    portTypeNum = PortTypeNum.No_Change;
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Due to there is cmd from target AGV Station Port " + "一律回復" + isOK);
                    RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                    return isOK;
                }

                //確認取得的AGVStationData中的Port都只有可以用的後2個。
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter the filter for AGV port.");
                List<PortDef> accessAGVPortDatas = FilterOfAGVPort(AGVPortDatas, useFirst2Port);
                if (accessAGVPortDatas.Count() == 0) //若沒有任何一個可以用 //A20.07.10.0
                {
                    isOK = ChangeReturnDueToAGVCCmdNum(AGVCFromEQToStationCmdNum); //A20.07.10.0
                    portTypeNum = PortTypeNum.No_Change;
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Due to there is No Port is workable " + "一律回復" + isOK);
                    RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                    return isOK;
                }
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Exit the filter for AGV port.");
                CheckThreeFourPortSituationAndMove(AGVStationID, useFirst2Port, numOfAGVStation, AGVPortDatas);
                //目前先默認取前2個，確認port上Box數量(空與實皆要)
                int emptyBoxNumber, fullBoxNumber;
                bool success, rejectAGVC;
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter the Count Box number for AGV port.");
                (emptyBoxNumber, fullBoxNumber, success, rejectAGVC) = CountAGVStationBoxInfo(accessAGVPortDatas);
                if (success == false)
                {
                    isOK = ChangeReturnDueToAGVCCmdNum(AGVCFromEQToStationCmdNum); //A20.07.10.0
                    portTypeNum = PortTypeNum.No_Change;
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Due to the AGV port is not ready to unload" + "一律回復" + isOK);
                    RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                    return isOK;
                }
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " emptyBoxNumber = " + emptyBoxNumber + ", fullBoxNumber = " + fullBoxNumber);

                //若有實box 則先默認為NG，會稍微影響效率(在一AGV對多個Station時)
                if (fullBoxNumber > 0)
                {
                    //可針對特定細節做特化處理，可進一步優化
                    isOK = ChangeReturnDueToAGVCCmdNum(AGVCFromEQToStationCmdNum); //A20.07.10.0
                    portTypeNum = PortTypeNum.No_Change;
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Due to there is full box on AGV port " + "一律回復" + isOK);
                    RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                    return isOK;
                }
                // 新增在outmode狀態下的port 是否有對該port 的命令可執行，若有，則拒絕。
                else
                {
                    foreach (PortDef AGVPortData in accessAGVPortDatas)
                    {
                        PortPLCInfo portData = GetPLC_PortData(AGVPortData.PLCPortID);
                        if (portData.IsOutputMode && portData.LoadPosition1 != true && portData.IsReadyToLoad) // 若該out mode port 為 無空盒 且 load OK 
                        {
                            List<ACMD_MCS> useCheckCmd = cmdBLL.GetCmdDataByDest(portData.EQ_ID);
                            List<ACMD_MCS> useCheckCmd_1 = cmdBLL.GetCmdDataByDest(AGVStationID);
                            if (useCheckCmd.Count + useCheckCmd_1.Count() > 0)
                            {
                                isOK = ChangeReturnDueToAGVCCmdNum(AGVCFromEQToStationCmdNum);
                                portTypeNum = PortTypeNum.No_Change;
                                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Due to there is going to be a cmd to AGV port " + "一律回復" + isOK);
                                RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                                return isOK;
                            }
                        }
                    }
                }
                //判斷是否強制讓貨出去
                if (portINIData[AGVStationID].forceRejectAGVCTrigger == true)
                {
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter the forceRejectAGVCTrigger狀態");
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " 虛擬 port: " + AGVStationID + " 是 forceRejectAGVCTrigger狀態一律回復NG，並轉為OutPut");
                    //若forceRejectAGVCTrigger狀態，則執行轉至Output Mode 狀態。
                    OutputModeChange(accessAGVPortDatas, AGVStationID);
                    isOK = false;
                    portTypeNum = PortTypeNum.OutPut_Mode;
                    RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                    return isOK;
                }
                else
                {
                    //若無實Box，且判斷是否強制讓貨出去後再行判斷空Box 數量。
                    switch (emptyBoxNumber)
                    {
                        case (int)EmptyBoxNumber.NO_EMPTY_BOX:
                            //若沒有空box，則執行OHBC優先判定。
                            AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter the NO_EMPTY_BOX method");
                            (portTypeNum, isOK) = CheckForChangeAGVPortMode_OHBC(AGVCFromEQToStationCmdNum, accessAGVPortDatas, AGVStationID, isEmergency);
                            break;
                        case (int)EmptyBoxNumber.ONE_EMPTY_BOX:
                            //目前先以執行AGVC優先判定為主，因為若有Cst卡在AGV上並無其餘可去之處。
                            AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter the ONE_EMPTY_BOX method");
                            (portTypeNum, isOK) = CheckForChangeAGVPortMode_AGVC(AGVCFromEQToStationCmdNum, accessAGVPortDatas, AGVStationID, 1, isEmergency);
                            break;
                        case (int)EmptyBoxNumber.TWO_EMPTY_BOX:
                            //若有2空box，則執行AGVC優先判定。
                            AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter the TWO_EMPTY_BOX method");
                            (portTypeNum, isOK) = CheckForChangeAGVPortMode_AGVC(AGVCFromEQToStationCmdNum, accessAGVPortDatas, AGVStationID, 2, isEmergency);
                            break;
                    }
                    RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error
                (ex, "CanExcuteUnloadTransferAGVStationFromAGVC "
                    + " AGVStationID:" + AGVStationID
                    + " AGVCFromEQToStationCmdNum:" + AGVCFromEQToStationCmdNum
                    + " isEmergency:" + isEmergency
                    + "\n"
                );
            }
            return isOK;
        }

        private static bool IsUsingFirst2Port(PortDef portDefByAGVStationID)
        {
            bool _useFirst2Port = false;
            if (portDefByAGVStationID.PortLocationType == 2)
            {
                _useFirst2Port = false;
            }
            else if (portDefByAGVStationID.PortLocationType == 1)
            {
                _useFirst2Port = true;
            }

            return _useFirst2Port;
        }

        /// <summary>
        /// 處理回報邏輯，原本都是NG的部分，目前多一步以AGVC Cmd 數量進行判定，若為 0 則回覆OK。//A20.07.10.0
        /// </summary>
        /// A20.07.10.0
        /// <param name="AGVCFromEQToStationCmdNum"></param>
        /// <returns></returns>
        private static bool ChangeReturnDueToAGVCCmdNum(int AGVCFromEQToStationCmdNum)
        {
            bool isOK;
            if (AGVCFromEQToStationCmdNum == 0)
            {
                isOK = true;
            }
            else
            {
                isOK = false;
            }

            return isOK;
        }

        /// <summary>
        /// 確認目前沒有從該AGV Station出發之命令。有就回NG，以防止在產生退空盒動作後，卻又讓AGV車進行EQ至AGV Station 命令
        /// </summary>
        /// <param name="AGVStationData"></param>
        /// <returns></returns>
        private bool CheckIsSourceFromAGVStation(List<PortDef> AGVStationData)
        {
            foreach (PortDef AGVPortData in AGVStationData)
            {
                ACMD_MCS cmdData_FromPortID = cmdBLL.GetCmdDataBySource(AGVPortData.PLCPortID); //A01 A02
                if (cmdData_FromPortID != null)
                {
                    AGVCTriggerLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ")
                        + " CheckIsSourceFromAGVStation " + AGVPortData.PLCPortID
                        + " 找到命令 " + GetCmdLog(cmdData_FromPortID)
                    );

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 確認該AGVport是否可用
        /// </summary>
        /// A20.06.16.0
        /// <param name="AGVStationData"></param>
        private List<PortDef> FilterOfAGVPort(List<PortDef> AGVStationData, bool use_First2Port)
        {
            int count = 0;
            List<PortDef> accessAGVPortDatas = new List<PortDef>();
            if (use_First2Port == true)
            {
                foreach (PortDef AGVPortData in AGVStationData)
                {
                    //確認可以用的，取前2個直接加入，不管是否可用。(因為若有第3個Port 的AGV Station 會被拿來當作修正Unknown用的Port)
                    count = count + 1;  //A20.07.10.0
                    if (GetPLC_PortData(AGVPortData.PLCPortID).OpAutoMode == true)
                    {
                        accessAGVPortDatas.Add(AGVPortData);
                    }
                    if (count >= 2)
                    {
                        break;
                    }
                }
            }
            else
            {
                for (int i = AGVStationData.Count() - 1; i >= 0; i--)
                {
                    //確認可以用的，取後2個直接加入，不管是否可用。(因為若有第3個Port 的AGV Station 會被拿來當作修正Unknown用的Port)
                    count = count + 1;  //A20.07.10.0
                    if (GetPLC_PortData(AGVStationData[i].PLCPortID).OpAutoMode == true)
                    {
                        accessAGVPortDatas.Add(AGVStationData[i]);
                    }
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
        private (PortTypeNum portTypeNum_Result, bool isOK_Result) CheckForChangeAGVPortMode_AGVC(int AGVCFromEQToStationCmdNum, List<PortDef> AGVStationData, string AGVStationID, int emptyBoxNum_OnPort, bool isEmergency)
        {
            bool _isOK_Result = false;
            PortTypeNum _portTypeNum_Result = PortTypeNum.No_Change;

            try
            {
                if (AGVCFromEQToStationCmdNum > 0)
                {
                    int emptyBoxCount = GetTotalEmptyBoxNumber().emptyBox.Count();
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ")
                        + "AGV >> OHB|CheckForChangeAGVPortMode_AGVC"
                        + "    GetTotalEmptyBoxNumber().emptyBox.Count():" + emptyBoxCount
                        + "    emptyBoxNum_OnPort:" + emptyBoxNum_OnPort
                    );

                    if (emptyBoxCount < 2)
                    {
                        if (emptyBoxNum_OnPort < 2)
                        {
                            _isOK_Result = false;
                            //Set Alarm for no empty box
                            OHBC_AlarmSet(AGVStationID, ((int)AlarmLst.AGVStation_DontHaveEnoughEmptyBox).ToString());

                            OutputModeChange(AGVStationData, AGVStationID);
                            _portTypeNum_Result = PortTypeNum.OutPut_Mode;

                        }
                        else
                        {
                            _isOK_Result = true;

                            portINIData[AGVStationID].agvHasCmdsAccess = true;
                            portINIData[AGVStationID].reservePortTime = DateTime.Now;

                            bool isSuccess = InputModeChange(AGVStationData, isEmergency);
                            _portTypeNum_Result = PortTypeNum.Input_Mode;
                            if (isSuccess == false)
                            {
                                _isOK_Result = false;
                                portINIData[AGVStationID].agvHasCmdsAccess = false;
                            }
                            else
                            {
                                agvHasCmdsAccess = true;
                            }
                        }
                    }
                    else
                    {
                        _isOK_Result = true;
                        //Clear alarm for no empty box

                        OHBC_AlarmCleared(AGVStationID, ((int)AlarmLst.AGVStation_DontHaveEnoughEmptyBox).ToString());

                        portINIData[AGVStationID].agvHasCmdsAccess = true;
                        portINIData[AGVStationID].reservePortTime = DateTime.Now;

                        bool isSuccess = InputModeChange(AGVStationData, isEmergency);
                        _portTypeNum_Result = PortTypeNum.Input_Mode;
                        if (isSuccess == false)
                        {
                            _isOK_Result = false;
                            portINIData[AGVStationID].agvHasCmdsAccess = false;
                        }
                        else
                        {
                            agvHasCmdsAccess = true;
                        }
                    }
                }
                else
                {
                    int OHBCCmdNumber = GetToThisAGVStationMCSCmdNum(AGVStationData, AGVStationID);
                    if (OHBCCmdNumber > 0)
                    {
                        _isOK_Result = false;
                        OutputModeChange(AGVStationData, AGVStationID);
                        _portTypeNum_Result = PortTypeNum.OutPut_Mode;
                    }
                    else
                    {
                        _isOK_Result = ChangeReturnDueToAGVCCmdNum(AGVCFromEQToStationCmdNum); //A20.07.10.0
                        InputModeChange(AGVStationData, isEmergency);
                        _portTypeNum_Result = PortTypeNum.Input_Mode;
                    }
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "CheckForChangeAGVPortMode");
            }
            return (_portTypeNum_Result, _isOK_Result);
        }

        /// <summary>
        /// 優先判斷OHBC是否有命令與其後許處理流程。
        /// </summary>
        /// A20.06.15.0
        /// <param name="AGVCFromEQToStationCmdNum"></param>
        /// <param name="AGVStationData"></param>
        /// <param name="AGVStationID"></param>
        /// <returns></returns>
        private (PortTypeNum portTypeNum_Result, bool isOK_Result) CheckForChangeAGVPortMode_OHBC(int AGVCFromEQToStationCmdNum, List<PortDef> AGVStationData, string AGVStationID, bool isEmergency)
        {
            bool _isOK_Result = false;
            PortTypeNum _portTypeNum_Result = PortTypeNum.No_Change;
            try
            {
                int OHBCCmdNumber = GetToThisAGVStationMCSCmdNum(AGVStationData, AGVStationID);
                if (OHBCCmdNumber > 0)
                {
                    _isOK_Result = false;
                    OutputModeChange(AGVStationData, AGVStationID);
                    _portTypeNum_Result = PortTypeNum.OutPut_Mode;
                }
                else
                {
                    if (AGVCFromEQToStationCmdNum > 0)
                    {
                        //若目前空盒數量過少，且AGVC優先判定，則拒絕AGVC
                        if (GetTotalEmptyBoxNumber().emptyBox.Count() < 2)
                        {
                            _isOK_Result = false;
                            //Set Alarm for no empty box
                            OHBC_AlarmSet(AGVStationID, ((int)AlarmLst.AGVStation_DontHaveEnoughEmptyBox).ToString());
                        }
                        else
                        {
                            _isOK_Result = true;
                            //Clear alarm for no empty box
                            OHBC_AlarmCleared(AGVStationID, ((int)AlarmLst.AGVStation_DontHaveEnoughEmptyBox).ToString());

                            portINIData[AGVStationID].agvHasCmdsAccess = true;
                            portINIData[AGVStationID].reservePortTime = DateTime.Now;

                            bool isSuccess = InputModeChange(AGVStationData, isEmergency);
                            _portTypeNum_Result = PortTypeNum.Input_Mode;
                            if (isSuccess == false) // 若port type 的port mode changeable 為 false 則回false
                            {
                                _isOK_Result = false;
                                portINIData[AGVStationID].agvHasCmdsAccess = false;
                            }
                            else
                            {
                                agvHasCmdsAccess = true;
                            }
                        }
                    }
                    else
                    {
                        _isOK_Result = ChangeReturnDueToAGVCCmdNum(AGVCFromEQToStationCmdNum); //A20.07.10.0
                        OutputModeChange(AGVStationData, AGVStationID);
                        _portTypeNum_Result = PortTypeNum.OutPut_Mode;
                    }
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "CheckForChangeAGVPortMode");
            }

            return (_portTypeNum_Result, _isOK_Result);
        }

        /// <summary>
        /// 計算目前的AGVStation 前2個port 上有多少個空Box 與實Box
        /// </summary>
        /// <param name="AGVStationData">A20.06.15.0 新增</param>
        private (int emptyBoxNumber, int fullBoxNumber, bool success, bool rejectAGVC) CountAGVStationBoxInfo(List<PortDef> AGVStationData)
        {
            int _emptyBoxNumber = 0;
            int _fullBoxNumber = 0;
            int AGVStationNumber = 0;
            bool _success = true;
            bool _rejectAGVC = false;
            foreach (PortDef AgvPortData in AGVStationData)
            {
                PortPLCInfo portPLCdata = GetPLC_PortData(AgvPortData.PLCPortID);
                if (portPLCdata.LoadPosition1 == true)
                {
                    //若LoadPosition On  只需判定是否可以取走作為最終訊號即可
                    if (portPLCdata.IsReadyToUnload == true)
                    {

                    }
                    else
                    {
                        _success = false;
                        return (_emptyBoxNumber, _fullBoxNumber, _success, _rejectAGVC);
                    }
                }
                AGVStationNumber = AGVStationNumber + 1;
                if (portPLCdata.LoadPosition1 == true && portPLCdata.IsCSTPresence == false)
                {
                    _emptyBoxNumber = _emptyBoxNumber + 1;
                }
                else if (portPLCdata.LoadPosition1 == true && portPLCdata.IsCSTPresence == true)
                {
                    _fullBoxNumber = _fullBoxNumber + 1;
                    if (portPLCdata.IsInputMode == true)
                    {
                        _rejectAGVC = true;
                    }
                }
                // 因為目前AGV上只有2儲位，故目前以2個Port為上限。
                if (AGVStationNumber >= 2)
                {
                    break;
                }
            }
            return (_emptyBoxNumber, _fullBoxNumber, _success, _rejectAGVC);
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
                List<ACMD_MCS> cmdData_PortID = cmdBLL.GetCmdDataByDestAndByPassManaul(AGVPortData.PLCPortID); //A01 A02
                cmdNumber = cmdNumber + cmdData_PortID.Count();
            }
            List<ACMD_MCS> cmdData_StationID = cmdBLL.GetCmdDataByDestAndByPassManaul(AGVStationID); //ST01
            cmdNumber = cmdNumber + cmdData_StationID.Count();
            return cmdNumber;
        }

        /// <summary>
        /// 切換該目的地Port為InputMode且執行退補空box
        /// </summary>
        /// A20.06.15.0 新增
        /// <param name="AGVPortData"></param>
        private bool InputModeChange(List<PortDef> AGVPortDatas, bool isEmergency)
        {
            //Todo
            // 需要實作更改該AGVPort為Input 及執行一次退補空box動作
            bool isSuccess = false;
            foreach (PortDef AGVPortData in AGVPortDatas)
            {
                PortPLCInfo portData = GetPLC_PortData(AGVPortData.PLCPortID);
                if (portData.IsModeChangable == false)
                {
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGV " + AGVPortData.PLCPortID + " IsModeChangable 是 false 回復 AGVC NG ");
                    isSuccess = false;
                    return isSuccess;
                }
                if ((AGVPortData.PortType == E_PortType.Out && portData.IsCSTPresence == false) || (isEmergency == true && AGVPortData.PortType == E_PortType.Out))
                {
                    isSuccess = PortTypeChange(AGVPortData.PLCPortID, E_PortType.In, "InputModeChange");
                }
                else if (AGVPortData.PortType == E_PortType.In)
                {
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGV " + AGVPortData.PLCPortID + " 是 Input Mode 不執行切type 只打開退補空BOX Flag. ");
                    isSuccess = true;
                }
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
        private bool OutputModeChange(List<PortDef> AGVPortDatas, string AGVStationID)
        {
            //Todo
            // 需要實作更改該AGVPort為Output 及執行一次退補空box動作
            bool isSuccess = false;
            OHBC_AGV_HasCmdsAccessCleared(AGVStationID);
            foreach (PortDef AGVPortData in AGVPortDatas)
            {
                PortPLCInfo portData = GetPLC_PortData(AGVPortData.PLCPortID);
                if (portData.IsModeChangable == false)
                {
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGV " + AGVPortData.PLCPortID + " IsModeChangable 是 false 回復 AGVC NG ");
                    isSuccess = false;
                    return isSuccess;
                }
                if (AGVPortData.PortType == E_PortType.In && portData.IsCSTPresence == false)
                {
                    isSuccess = PortTypeChange(AGVPortData.PLCPortID, E_PortType.Out, "OutputModeChange");
                }
                else if (AGVPortData.PortType == E_PortType.Out)
                {
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGV " + AGVPortData.PLCPortID + " 是 Output Mode 不執行切type 只打開退補空BOX Flag. ");
                    isSuccess = true;
                }
            }
            SpinWait.SpinUntil(() => false, 200);
            Task.Run(() =>
            {
                CyclingCheckWithdraw(AGVPortDatas);
            });
            return isSuccess;
        }
        /// <summary>
        /// 切換該目的地Port為1 OutputMode 1 InputMode且執行退補空box
        /// </summary>
        /// A20.09.06.0  新增
        /// <param name="AGVPortData"></param>
        private bool InOutModeChange(List<PortDef> AGVPortDatas, string AGVStationID)
        {
            //Todo
            // 需要實作更改該AGVPort為Output 及執行一次退補空box動作
            bool isSuccess = false;
            OHBC_AGV_HasCmdsAccessCleared(AGVStationID);
            List<PortPLCInfo> _AGVPortPLCDatas = new List<PortPLCInfo>();
            bool hasInputMode = false; // 用於判定是否有input mode port 存在。
            getPLCRealInfo(AGVPortDatas, _AGVPortPLCDatas);
            if (_AGVPortPLCDatas.Count() > 1)
            {
                if (_AGVPortPLCDatas[0].IsModeChangable == true && _AGVPortPLCDatas[1].IsModeChangable == true)
                {
                    if (_AGVPortPLCDatas[0].LoadPosition1 == true && _AGVPortPLCDatas[1].LoadPosition1 == true)
                    {
                        AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGV " + AGVStationID + "enter 2 Box InOutMode.");
                        if (_AGVPortPLCDatas[0].IsCSTPresence == true && _AGVPortPLCDatas[1].IsCSTPresence == true && swapTriggerWaitin == false)
                        {
                            // 若2實盒且為 非swap 觸發模式需拒絕
                            AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGV Station " + " two box but have full box.");
                        }
                        else
                        {
                            if (AGVPortDatas[1].PortType == E_PortType.In)
                            {
                                isSuccess = PortTypeChange(AGVPortDatas[1].PLCPortID, E_PortType.In, "InOutModeChange");
                                isSuccess = PortTypeChange(AGVPortDatas[0].PLCPortID, E_PortType.Out, "InOutModeChange");
                            }
                            else
                            {
                                isSuccess = PortTypeChange(AGVPortDatas[0].PLCPortID, E_PortType.In, "InOutModeChange");
                                isSuccess = PortTypeChange(AGVPortDatas[1].PLCPortID, E_PortType.Out, "InOutModeChange");
                            }
                        }
                    }
                    else if (_AGVPortPLCDatas[0].LoadPosition1 == true && _AGVPortPLCDatas[1].LoadPosition1 == false)
                    {
                        AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGV " + AGVStationID + "enter 1 Box right InOutMode.");
                        if (AGVPortDatas[0].PortType == E_PortType.Out && _AGVPortPLCDatas[0].IsCSTPresence == true) // 有實盒out不可轉。
                        {
                            isSuccess = PortTypeChange(AGVPortDatas[0].PLCPortID, E_PortType.Out, "InOutModeChange");
                            isSuccess = PortTypeChange(AGVPortDatas[1].PLCPortID, E_PortType.In, "InOutModeChange");
                        }
                        else // out + empty || in + full 
                        {
                            isSuccess = PortTypeChange(AGVPortDatas[0].PLCPortID, E_PortType.In, "InOutModeChange");
                            isSuccess = PortTypeChange(AGVPortDatas[1].PLCPortID, E_PortType.Out, "InOutModeChange");
                        }
                    }
                    else if (_AGVPortPLCDatas[0].LoadPosition1 == false && _AGVPortPLCDatas[1].LoadPosition1 == true)
                    {
                        AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGV " + AGVStationID + "enter 1 Box left InOutMode.");
                        if (AGVPortDatas[1].PortType == E_PortType.Out && _AGVPortPLCDatas[1].IsCSTPresence == true) // 有實盒out不可轉。
                        {
                            isSuccess = PortTypeChange(AGVPortDatas[1].PLCPortID, E_PortType.Out, "InOutModeChange");
                            isSuccess = PortTypeChange(AGVPortDatas[0].PLCPortID, E_PortType.In, "InOutModeChange");
                        }
                        else // out + empty || in + full 
                        {
                            isSuccess = PortTypeChange(AGVPortDatas[1].PLCPortID, E_PortType.In, "InOutModeChange");
                            isSuccess = PortTypeChange(AGVPortDatas[0].PLCPortID, E_PortType.Out, "InOutModeChange");
                        }
                    }
                    else if (_AGVPortPLCDatas[0].LoadPosition1 == false && _AGVPortPLCDatas[1].LoadPosition1 == false)
                    {
                        AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGV " + AGVStationID + "enter 0 Box InOutMode.");
                        if (AGVPortDatas[1].PortType == E_PortType.In)
                        {
                            isSuccess = PortTypeChange(AGVPortDatas[1].PLCPortID, E_PortType.In, "InOutModeChange");
                            isSuccess = PortTypeChange(AGVPortDatas[0].PLCPortID, E_PortType.Out, "InOutModeChange");
                        }
                        else
                        {
                            isSuccess = PortTypeChange(AGVPortDatas[0].PLCPortID, E_PortType.Out, "InOutModeChange");
                            isSuccess = PortTypeChange(AGVPortDatas[1].PLCPortID, E_PortType.In, "InOutModeChange");
                        }
                    }
                }
                else
                {
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGV Station " + " are not ready for mode change.");
                }
            }
            SpinWait.SpinUntil(() => false, 200);
            Task.Run(() =>
            {
                CyclingCheckWithdraw(AGVPortDatas);
            });
            return isSuccess;
        }

        private void getPLCRealInfo(List<PortDef> AGVPortDatas, List<PortPLCInfo> _AGVPortPLCDatas)
        {
            foreach (PortDef AGVPortData in AGVPortDatas)
            {
                PortPLCInfo portPLCStatus = GetPLC_PortData(AGVPortData.PLCPortID);
                _AGVPortPLCDatas.Add(portPLCStatus);
            }
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
                            continue;
                        }
                    }

                    if (AGVPortReady)
                    {
                        foreach (PortPLCInfo portPLCStatus in portPLCDatas)
                        {
                            //呼叫退補空box 流程。 先將特定port 開啟自動退補，產生完命令後再關閉。
                            OpenAGV_Station(portPLCStatus.EQ_ID, true, "CyclingCheckReplenishment");
                            //PLC_AGV_Station_InMode(portPLCStatus);
                            //portINIData[portPLCStatus.EQ_ID].openAGV_Station = false;
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
                        if ((portPLCStatus.IsReadyToLoad == true) || //若該port為input mode且 is ready to load 為 true; (可以被補空盒)
                            (portPLCStatus.IsReadyToUnload == true)) //或者為input mode 且 is ready to unload 為true;   (上已有盒)
                        {
                            AGVPortReady = true;
                        }
                        else
                        {
                            AGVPortReady = false;
                            continue;
                        }
                    }

                    if (AGVPortReady)
                    {
                        foreach (PortPLCInfo portPLCStatus in portPLCDatas)
                        {
                            //呼叫退補空box 流程。 先將特定port 開啟自動退補，產生完命令後再關閉。
                            OpenAGV_Station(portPLCStatus.EQ_ID, true, "CyclingCheckWithdraw");
                            //PLC_AGV_Station_OutMode(portPLCStatus);
                            //portINIData[portPLCStatus.EQ_ID].openAGV_Station = false;
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
        /// 判斷該寫入畫面的數值
        /// </summary>
        /// <param name="AGVStationName"></param>
        /// <param name="portTypeNum"></param>
        /// <param name="result"></param>
        private void RewriteTheResultOfAGVCTrigger(String AGVStationName, PortTypeNum portTypeNum, bool result)
        {
            switch (portTypeNum)
            {
                case PortTypeNum.No_Change:
                    TargetAGVStationRewrite(AGVStationName, "No Change", result);
                    break;
                case PortTypeNum.Input_Mode:
                    TargetAGVStationRewrite(AGVStationName, "Input Change", result);
                    break;
                case PortTypeNum.OutPut_Mode:
                    TargetAGVStationRewrite(AGVStationName, "OutPut Change", result);
                    break;
            }
        }

        /// <summary>
        /// 寫入畫面更新時使用之String位置
        /// </summary>
        /// <param name="AGVStationName"></param>
        /// <param name="portChangeResult"></param>
        /// <param name="result"></param>
        private void TargetAGVStationRewrite(String AGVStationName, string portChangeResult, bool result)
        {
            string lastNumOfAGVStation = AGVStationName[AGVStationName.Length - 1].ToString();
            switch (lastNumOfAGVStation)
            {
                case ("1"):
                    agvcTriggerResult_ST01 = DateTime.Now.ToString("HH:mm:ss.fff ") + " " + portChangeResult + " " + result.ToString();
                    break;
                case ("2"):
                    agvcTriggerResult_ST02 = DateTime.Now.ToString("HH:mm:ss.fff ") + " " + portChangeResult + " " + result.ToString();
                    break;
                case ("3"):
                    agvcTriggerResult_ST03 = DateTime.Now.ToString("HH:mm:ss.fff ") + " " + portChangeResult + " " + result.ToString();
                    break;
            }
        }
        #endregion

        #region  Call by AGVC Restful API. Use for process the AGV Station. New Method for checking Port and Response.
        public bool CanExcuteUnloadTransferAGVStationFromAGVC_OneInOneOut(string AGVStationID, int AGVCFromEQToStationCmdNum, bool isEmergency)
        {
            PortTypeNum portTypeNum = PortTypeNum.No_Change;
            bool isOK = false; //A20.07.10.0
            try
            {
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + "觸發開始");
                bool useFirst2Port = false;     //判斷是否使用前 2 port，若否使用後 2 port
                int numOfAGVStation = GetAGVPort(AGVStationID).Count();   //確認目前選的AGV Station 有多少個Port
                agvcTriggerAlarmCheck(AGVStationID, AGVCFromEQToStationCmdNum);
                //此AGVStation虛擬port是 Out of service 擇要拒絕AGVC
                PortDef portDefByAGVStationID = scApp.PortDefBLL.GetPortData(AGVStationID);
                if (portDefByAGVStationID.State == E_PORT_STATUS.OutOfService)
                {
                    //若為4個Port 的虛擬Port
                    if (numOfAGVStation == 4 && portDefByAGVStationID.AGVState == E_PORT_STATUS.OutOfService) //A20.07.10.0
                    {
                        isOK = true;
                        return isOK;
                    }
                    isOK = ChangeReturnDueToAGVCCmdNum(AGVCFromEQToStationCmdNum); //A20.07.10.0
                    portTypeNum = PortTypeNum.No_Change;
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " 是 Out of service 一律回復" + isOK);
                    RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                    return isOK;
                }
                //確認要取得的AGVStation Port 為前2還是後2 前為1 後為2
                useFirst2Port = IsUsingFirst2Port(portDefByAGVStationID);
                //取得PLC目前資訊
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Trigger start. Get the AGVSTation Data, " +
                    "AGVStationID = " + AGVStationID + ", AGVCFromEQToStationCmdNum = " + AGVCFromEQToStationCmdNum + ", isEmergency = " + isEmergency.ToString()
                     + " , 線上空盒數量 = " + GetTotalEmptyBoxNumber().emptyBox.Count().ToString());
                List<PortDef> AGVPortDatas = scApp.PortDefBLL.GetAGVPortGroupDataByStationID(line.LINE_ID, AGVStationID);
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Exit GetAGVPortGroupDataByStationID().");

                //確認目前的AGV port 是否有source 為它的取貨命令(若有，則一律回復否，避免先觸發退box後，卻因下一次觸發同意AGV來放貨)
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter CheckIsSourceFromAGVStation().");
                bool haveCmdFromAGVPort = CheckIsSourceFromAGVStation(AGVPortDatas);
                if (haveCmdFromAGVPort == true)
                {
                    isOK = ChangeReturnDueToAGVCCmdNum(AGVCFromEQToStationCmdNum); //A20.07.10.0
                    portTypeNum = PortTypeNum.No_Change;
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Due to there is cmd from target AGV Station Port " + "一律回復" + isOK);
                    RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                    return isOK;
                }
                CheckThreeFourPortSituationAndMove(AGVStationID, useFirst2Port, numOfAGVStation, AGVPortDatas);
                //確認取得的AGVStationData中的Port都只有可以用的後2個。
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter the filter for AGV port.");
                List<PortDef> accessAGVPortDatas = FilterOfAGVPort(AGVPortDatas, useFirst2Port);
                if (accessAGVPortDatas.Count() == 0) //若沒有任何一個可以用 //A20.07.10.0
                {
                    isOK = ChangeReturnDueToAGVCCmdNum(AGVCFromEQToStationCmdNum); //A20.07.10.0
                    portTypeNum = PortTypeNum.No_Change;
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Due to there is No Port is workable " + "一律回復" + isOK);
                    RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                    return isOK;
                }
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Exit the filter for AGV port.");

                //目前先默認取前2個，確認port上Box數量(空與實皆要)
                int emptyBoxNumber, fullBoxNumber;
                bool success, rejectAGVC;
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter the Count Box number for AGV port.");
                (emptyBoxNumber, fullBoxNumber, success, rejectAGVC) = CountAGVStationBoxInfo(accessAGVPortDatas);
                if (success == false)
                {
                    isOK = ChangeReturnDueToAGVCCmdNum(AGVCFromEQToStationCmdNum); //A20.07.10.0
                    portTypeNum = PortTypeNum.No_Change;
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Due to the AGV port is not ready to unload" + "一律回復" + isOK);
                    RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                    return isOK;
                }
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " emptyBoxNumber = " + emptyBoxNumber + ", fullBoxNumber = " + fullBoxNumber);

                //若有實box 則先默認為NG，會稍微影響效率(在一AGV對多個Station時)
                //if (fullBoxNumber > 0)
                //{
                //    //可針對特定細節做特化處理，可進一步優化
                //    isOK = ChangeReturnDueToAGVCCmdNum(AGVCFromEQToStationCmdNum); //A20.07.10.0
                //    portTypeNum = PortTypeNum.No_Change;
                //    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Due to there is full box on AGV port " + "一律回復" + isOK);
                //    RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                //    //return isOK;
                //}
                //// 新增在outmode狀態下的port 是否有對該port 的命令可執行，若有，則拒絕。
                //else
                {
                    bool isThereInputModePort = false;
                    foreach (PortDef AGVPortData in accessAGVPortDatas)
                    {
                        PortPLCInfo portData = GetPLC_PortData(AGVPortData.PLCPortID);
                        if (portData.IsInputMode && portData.LoadPosition1 != true && portData.IsReadyToLoad)
                        {
                            isThereInputModePort = true;
                        }
                    }
                    foreach (PortDef AGVPortData in accessAGVPortDatas)
                    {
                        PortPLCInfo portData = GetPLC_PortData(AGVPortData.PLCPortID);
                        if (portData.IsOutputMode && portData.LoadPosition1 != true && portData.IsReadyToLoad) // 若該out mode port 為 無空盒 且 load OK 
                        {
                            List<ACMD_MCS> useCheckCmd = cmdBLL.GetCmdDataByDest(portData.EQ_ID);
                            List<ACMD_MCS> useCheckCmd_1 = cmdBLL.GetCmdDataByDest(AGVStationID);
                            if (isThereInputModePort != true)
                            {
                                if ((useCheckCmd.Count + useCheckCmd_1.Count() > 0))
                                {
                                    isOK = ChangeReturnDueToAGVCCmdNum(AGVCFromEQToStationCmdNum);
                                    portTypeNum = PortTypeNum.No_Change;
                                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Due to there is going to be a cmd to AGV port " + "一律回復" + isOK);
                                    RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                                    return isOK;
                                }
                            }
                        }
                    }
                }
                //判斷是否強制讓貨出去
                if (portINIData[AGVStationID].forceRejectAGVCTrigger == true)
                {
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter the forceRejectAGVCTrigger狀態");
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " 虛擬 port: " + AGVStationID + " 是 forceRejectAGVCTrigger狀態一律回復NG，並轉為OutPut");
                    //若forceRejectAGVCTrigger狀態，則執行轉至Output Mode 狀態。
                    OutputModeChange(accessAGVPortDatas, AGVStationID);
                    isOK = false;
                    portTypeNum = PortTypeNum.OutPut_Mode;
                    RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                    return isOK;
                }
                else
                {
                    #region 單取單放實做位置
                    //若無實Box，且判斷是否強制讓貨出去後再行判斷空Box 數量。0 
                    if (accessAGVPortDatas.Count() == 1)
                    {
                        if (fullBoxNumber > 0)
                        {
                            isOK = ChangeReturnDueToAGVCCmdNum(AGVCFromEQToStationCmdNum);
                            AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Due to there is full box on AGV port 1 in 1 out " + "一律回復" + isOK);
                            RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                            return isOK;
                        }
                        else
                        {
                            switch (emptyBoxNumber)
                            {
                                case (int)EmptyBoxNumber.NO_EMPTY_BOX:
                                    //若沒有空box，則執行OHBC優先判定。
                                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter the NO_EMPTY_BOX method");
                                    (portTypeNum, isOK) = CheckForChangeAGVPortMode_OHBC(AGVCFromEQToStationCmdNum, accessAGVPortDatas, AGVStationID, isEmergency);
                                    break;
                                case (int)EmptyBoxNumber.ONE_EMPTY_BOX:
                                    //目前先以執行AGVC優先判定為主，因為若有Cst卡在AGV上並無其餘可去之處。
                                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter the ONE_EMPTY_BOX method");
                                    (portTypeNum, isOK) = CheckForChangeAGVPortMode_AGVC(AGVCFromEQToStationCmdNum, accessAGVPortDatas, AGVStationID, 1, isEmergency);
                                    break;
                                case (int)EmptyBoxNumber.TWO_EMPTY_BOX:
                                    //若有2空box，則執行AGVC優先判定。
                                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter the TWO_EMPTY_BOX method");
                                    (portTypeNum, isOK) = CheckForChangeAGVPortMode_AGVC(AGVCFromEQToStationCmdNum, accessAGVPortDatas, AGVStationID, 2, isEmergency);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        int OHBCCmdNumber = GetToThisAGVStationMCSCmdNum(accessAGVPortDatas, AGVStationID);
                        //若目前的AGVC 命令在2筆以上，且OHBC為0筆，此時以2 in 為控制方向去對AGV Station變換。
                        if (AGVCFromEQToStationCmdNum >= 2 && OHBCCmdNumber == 0)
                        {
                            // 若為有超過2個input mode 且為 InServeice (有空Box 且readyToUnload)，則不用2 in 維持 單 in 單 out 就可以。
                            int InMode_InServiceNum = GetAGV_InModeInServicePortName_NumberByHasEmptyBox(AGVStationID);
                            if (InMode_InServiceNum >= 2)
                            {
                                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " InMode_InServiceNum = " + InMode_InServiceNum + " and Already has two Input Mode Inservice Port.");
                                isOK = true;
                            }
                            //else if (fullBoxNumber == 0)
                            //{
                            //    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " InMode_InServiceNum = " + InMode_InServiceNum + " and Enter the Two IN MODE TYPE method");
                            //    InputModeChange(accessAGVPortDatas);
                            //    portTypeNum = PortTypeNum.Input_Mode;
                            //    isOK = true;
                            //}
                            else
                            {
                                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter the InMode_InServiceNum reply OK ");
                                InOutModeChange(accessAGVPortDatas, AGVStationID);
                                isOK = true;
                            }
                        }
                        //其餘狀況中都為 1 In 1 Out 為控制方向去對AGV Station 變換
                        else
                        {
                            //需額外判斷若AGVC 命令為0 要回復NG，用以觸發後續AGV預先移動命令。
                            if (AGVCFromEQToStationCmdNum == 0)
                            {
                                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter the reply true for 0 Agvc Cmd method");
                                InOutModeChange(accessAGVPortDatas, AGVStationID);
                                isOK = true;
                            }
                            else
                            {
                                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter the reply OK ");
                                InOutModeChange(accessAGVPortDatas, AGVStationID);
                                isOK = true;
                            }
                        }
                    }
                    #endregion
                }
                RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error
                (ex, "CanExcuteUnloadTransferAGVStationFromAGVC "
                    + " AGVStationID:" + AGVStationID
                    + " AGVCFromEQToStationCmdNum:" + AGVCFromEQToStationCmdNum
                    + " isEmergency:" + isEmergency
                    + "\n"
                );
            }
            AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + "觸發正常結束");
            return isOK;
        }
        #endregion

        #region Call by AGVC Restful API. Use swap method.
        public (bool is_OK, bool is_More_out) CanExcuteUnloadTransferAGVStationFromAGVC_Swap(string AGVStationID, int AGVCFromEQToStationCmdNum, bool isEmergency)
        {
            PortTypeNum portTypeNum = PortTypeNum.No_Change;
            bool isOK = false; //A20.07.10.0
            bool isMoreOutMode = true;
            bool setMoreOutMode = setForMoreOut; // 這邊可以串接UI控制。
            PortPLCInfo thirdAGVPort = new PortPLCInfo();
            PortDef thirdAGVPort_DB = new PortDef();
            try
            {
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + "觸發開始");
                bool useFirst2Port = false;     //判斷是否使用前 2 port，若否使用後 2 port
                int numOfAGVStation = GetAGVPort(AGVStationID).Count();   //確認目前選的AGV Station 有多少個Port
                agvcTriggerAlarmCheck(AGVStationID, AGVCFromEQToStationCmdNum);
                //此AGVStation虛擬port是 Out of service 擇要拒絕AGVC
                PortDef portDefByAGVStationID = scApp.PortDefBLL.GetPortData(AGVStationID);
                if (portDefByAGVStationID.State == E_PORT_STATUS.OutOfService)
                {
                    isOK = ChangeReturnDueToAGVCCmdNum(AGVCFromEQToStationCmdNum); //A20.07.10.0
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " 是 Out of service 一律回復" + isOK);
                    RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                    return (isOK, isMoreOutMode);
                }
                //確認要取得的AGVStation Port 為前2還是後2 前為1 後為2
                useFirst2Port = IsUsingFirst2Port(portDefByAGVStationID);
                //取得PLC目前資訊
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Trigger start. Get the AGVSTation Data, " + "AGVStationID = " + AGVStationID + ", AGVCFromEQToStationCmdNum = " + AGVCFromEQToStationCmdNum + ", isEmergency = " + isEmergency.ToString() + " , 線上空盒數量 = " + GetTotalEmptyBoxNumber().emptyBox.Count().ToString());
                List<PortDef> AGVPortDatas = scApp.PortDefBLL.GetAGVPortGroupDataByStationID(line.LINE_ID, AGVStationID);
                //確認目前的AGV port 是否有source 為它的取貨命令(若有，則一律回復否，避免先觸發退box後，卻因下一次觸發同意AGV來放貨)
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter CheckIsSourceFromAGVStation().");
                bool haveCmdFromAGVPort = CheckIsSourceFromAGVStation(AGVPortDatas);
                if (haveCmdFromAGVPort == true)
                {
                    isOK = ChangeReturnDueToAGVCCmdNum(AGVCFromEQToStationCmdNum); //A20.07.10.0
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Due to there is cmd from target AGV Station Port " + "一律回復" + isOK);
                    RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                    return (isOK, isMoreOutMode);
                }
                //確認3 4 port 狀態及切換
                CheckThreeFourPortSituationAndMove(AGVStationID, useFirst2Port, numOfAGVStation, AGVPortDatas);
                //若有第3個port 須納入考量
                if (numOfAGVStation == 3)
                {
                    if (useFirst2Port == false) //取3port的第一個
                    {
                        thirdAGVPort_DB = AGVPortDatas.FirstOrDefault();
                        thirdAGVPort = GetPLC_PortData(thirdAGVPort_DB.PLCPortID);
                    }
                    else //取3port的第三個
                    {
                        thirdAGVPort_DB = AGVPortDatas.LastOrDefault();
                        thirdAGVPort = GetPLC_PortData(thirdAGVPort_DB.PLCPortID);
                    }
                }
                //確認取得的AGVStationData中的Port都只有可以用的後2個。
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter the filter for AGV port.");
                List<PortDef> accessAGVPortDatas = FilterOfAGVPort(AGVPortDatas, useFirst2Port);
                if (accessAGVPortDatas.Count() == 0) //若沒有任何一個可以用 //A20.07.10.0
                {
                    isOK = ChangeReturnDueToAGVCCmdNum(AGVCFromEQToStationCmdNum); //A20.07.10.0
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Due to there is No Port is workable " + "一律回復" + isOK);
                    RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                    return (isOK, isMoreOutMode);
                }
                //目前先默認取前2個，確認port上Box數量(空與實皆要)
                int emptyBoxNumber, fullBoxNumber;
                bool success, rejectAGVC;
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter the Count Box number for AGV port.");
                (emptyBoxNumber, fullBoxNumber, success, rejectAGVC) = CountAGVStationBoxInfo(accessAGVPortDatas);
                if (success == false)
                {
                    isOK = ChangeReturnDueToAGVCCmdNum(AGVCFromEQToStationCmdNum); //A20.07.10.0
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Due to the AGV port is not ready to unload" + "一律回復" + isOK);
                    RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                    return (isOK, isMoreOutMode);
                }
                if (rejectAGVC == true)
                {
                    isOK = ChangeReturnDueToAGVCCmdNum(AGVCFromEQToStationCmdNum); //A20.07.10.0
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Due to there is full box on port inmode." + "一律回復" + isOK);
                    RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                    return (isOK, isMoreOutMode);
                }
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " emptyBoxNumber = " + emptyBoxNumber + ", fullBoxNumber = " + fullBoxNumber);

                //// 新增在outmode狀態下的port 是否有對該port 的命令可執行，若有，則拒絕。
                bool isThereInputModePort = false;
                foreach (PortDef AGVPortData in accessAGVPortDatas)
                {
                    PortPLCInfo portData = GetPLC_PortData(AGVPortData.PLCPortID);
                    if (portData.IsInputMode && portData.LoadPosition1 != true && portData.IsReadyToLoad)
                    {
                        isThereInputModePort = true;
                    }
                }
                foreach (PortDef AGVPortData in accessAGVPortDatas)
                {
                    PortPLCInfo portData = GetPLC_PortData(AGVPortData.PLCPortID);
                    if (portData.IsOutputMode && portData.LoadPosition1 != true && portData.IsReadyToLoad) // 若該out mode port 為 無空盒 且 load OK 
                    {
                        List<ACMD_MCS> useCheckCmd = cmdBLL.GetCmdDataByDest(portData.EQ_ID);
                        List<ACMD_MCS> useCheckCmd_1 = cmdBLL.GetCmdDataByDest(AGVStationID);
                        if (isThereInputModePort != true)
                        {
                            if ((useCheckCmd.Count + useCheckCmd_1.Count() > 0))
                            {
                                isOK = ChangeReturnDueToAGVCCmdNum(AGVCFromEQToStationCmdNum);
                                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Due to there is going to be a cmd to AGV port " + "一律回復" + isOK);
                                RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                                return (isOK, isMoreOutMode);
                            }
                        }
                    }
                }

                //判斷是否強制讓貨出去
                if (portINIData[AGVStationID].forceRejectAGVCTrigger == true)
                {
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter the forceRejectAGVCTrigger狀態");
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " 虛擬 port: " + AGVStationID + " 是 forceRejectAGVCTrigger狀態一律回復NG，並轉為OutPut");
                    //若forceRejectAGVCTrigger狀態，則執行轉至Output Mode 狀態。
                    OutputModeChange(accessAGVPortDatas, AGVStationID);
                    isOK = false;
                    portTypeNum = PortTypeNum.OutPut_Mode;
                    RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
                    return (isOK, isMoreOutMode);
                }
                else
                {
                    #region swap功能判定位置
                    //若只有一個port是OK的，則須都回覆"多出模式"
                    int OHBCCmdNumber = GetToThisAGVStationMCSCmdNum(accessAGVPortDatas, AGVStationID);
                    if (accessAGVPortDatas.Count() == 1)
                    {
                        if (isEmergency != true)
                        {
                            //有AGVC cmd 轉in ， 只有 OHBC cmd 轉out 退空 ， 若都無則不動作。
                            if (emptyBoxNumber > 0)
                            {
                                if (AGVCFromEQToStationCmdNum > 0)
                                {
                                    InputModeChange(accessAGVPortDatas, isEmergency);
                                }
                                else if (OHBCCmdNumber > 0)
                                {
                                    OutputModeChange(accessAGVPortDatas, AGVStationID);
                                }
                                isOK = true;
                                isMoreOutMode = true;
                            }
                            // 不可能有實盒 所以非空盒 = 空port。 有OHBC cmd 轉out ， 有AGVC cmd 轉in 補空，  若都無則不動作。
                            else if (fullBoxNumber == 0)
                            {
                                if (OHBCCmdNumber > 0)
                                {
                                    OutputModeChange(accessAGVPortDatas, AGVStationID);
                                }
                                else if (AGVCFromEQToStationCmdNum > 0)
                                {
                                    InputModeChange(accessAGVPortDatas, isEmergency);
                                }
                                isOK = true;
                                isMoreOutMode = true;
                            }
                            else
                            {
                                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " 虛擬 port: " + AGVStationID + " 上有實盒且上僅1可用port 回復NG。");
                                isMoreOutMode = true;
                                isOK = false;
                            }
                        }
                        else
                        {
                            AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter the One port One in One Out swap Emergency = " + isEmergency.ToString());
                            if (AGVCFromEQToStationCmdNum > 0)
                            {
                                InputModeChange(accessAGVPortDatas, isEmergency);
                            }
                            else if (OHBCCmdNumber > 0)
                            {
                                OutputModeChange(accessAGVPortDatas, AGVStationID);
                            }
                            isMoreOutMode = true;
                            isOK = true;
                        }
                    }
                    // 若有 2 Port 或者 3 Port 的第3個Port不為自動狀態，走一般2port 流程。
                    else if (numOfAGVStation == 2 || (numOfAGVStation == 3 && thirdAGVPort.OpAutoMode != true))
                    {
                        //若不緊急 走正常邏輯
                        if (isEmergency != true)
                        {
                            (isOK, isMoreOutMode, portTypeNum) = SwapTwoPortCheck(accessAGVPortDatas, AGVPortDatas, AGVStationID, portTypeNum, OHBCCmdNumber, AGVCFromEQToStationCmdNum, emptyBoxNumber, fullBoxNumber, setMoreOutMode, isOK, isMoreOutMode, isEmergency);
                        }
                        //若為緊急流程 走1 in 1 out 回多入流程
                        else
                        {
                            AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + " Enter the One in One Out swap Emergency = " + isEmergency.ToString());
                            InOutModeChange(accessAGVPortDatas, AGVStationID);
                            isMoreOutMode = true;
                            isOK = true;
                        }
                    }
                    // 若有 3 Port 且為自動模式，走 3 Port 確認流程。只有在第3個port 上為 input mode 且空箱時，下 2 out ，其餘為1 in 1 out (但須注意是否有足夠的out 命令，沒有還是得轉in 補空)
                    // 並且默認為多入模式。(因為必定會有1個空 port)
                    else if (numOfAGVStation == 3 && thirdAGVPort.OpAutoMode == true)
                    {
                        // 若第3 port 已經有空盒 input 走 多出模式 但要回復多入模式。
                        if (thirdAGVPort.IsInputMode == true && thirdAGVPort.IsOutputMode == false && thirdAGVPort.LoadPosition1 == true && thirdAGVPort.IsCSTPresence == false)
                        {
                            setMoreOutMode = true;
                            (isOK, isMoreOutMode, portTypeNum) = SwapTwoPortCheck(accessAGVPortDatas, AGVPortDatas, AGVStationID, portTypeNum, OHBCCmdNumber, AGVCFromEQToStationCmdNum, emptyBoxNumber, fullBoxNumber, setMoreOutMode, isOK, isMoreOutMode, isEmergency);
                            isMoreOutMode = false;
                        }
                        // 其餘狀態需要走多入模式
                        else
                        {
                            setMoreOutMode = false;
                            (isOK, isMoreOutMode, portTypeNum) = SwapTwoPortCheck(accessAGVPortDatas, AGVPortDatas, AGVStationID, portTypeNum, OHBCCmdNumber, AGVCFromEQToStationCmdNum, emptyBoxNumber, fullBoxNumber, setMoreOutMode, isOK, isMoreOutMode, isEmergency);
                            isMoreOutMode = false;
                        }
                    }
                    #endregion
                }

                RewriteTheResultOfAGVCTrigger(AGVStationID, portTypeNum, isOK);
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error
                (ex, "CanExcuteUnloadTransferAGVStationFromAGVC_Swap "
                    + " AGVStationID:" + AGVStationID
                    + " AGVCFromEQToStationCmdNum:" + AGVCFromEQToStationCmdNum
                    + " isEmergency:" + isEmergency
                    + "\n"
                );
            }
            AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + AGVStationID + "觸發正常結束, isOK = " + isOK + " , isMoreOutMode = " + isMoreOutMode);
            return (isOK, isMoreOutMode);
        }

        private (bool isOK_, bool isMoreOutMode_, PortTypeNum portTypeNum_) SwapTwoPortCheck(List<PortDef> _accessAGVPortDatas, List<PortDef> _AGVPortDatas, string _AGVStationID, PortTypeNum _portTypeNum, int _OHBCCmdNumber, int _AGVCFromEQToStationCmdNum, int _emptyBoxNumber, int _fullBoxNumber, bool _setMoreOutMode, bool _isOK, bool _isMoreOutMode, bool _isEmergency)
        {
            if (_OHBCCmdNumber == 0)
            {
                //沒AGVC也沒OHBC命令 一律無動作
                if (_AGVCFromEQToStationCmdNum == 0)
                {
                    _isOK = ChangeReturnDueToAGVCCmdNum(_AGVCFromEQToStationCmdNum); //A20.07.10.0
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + _AGVStationID + " 沒AGVC也沒OHBC命令 一律回復" + _isOK);
                    RewriteTheResultOfAGVCTrigger(_AGVStationID, _portTypeNum, _isOK);
                    _isMoreOutMode = true;
                    return (_isOK, _isMoreOutMode, _portTypeNum);
                }
                //有1筆以上AGVC命令直接都轉in 且不論目前狀態，都回可以大量入庫。
                else if (_AGVCFromEQToStationCmdNum >= 1)
                {
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + _AGVStationID + " Enter the Two IN MODE TYPE swap");
                    InputModeChange(_accessAGVPortDatas, _isEmergency);
                    _portTypeNum = PortTypeNum.Input_Mode;
                    _isMoreOutMode = false;
                    _isOK = true;
                }
            }
            else if (_OHBCCmdNumber == 1)
            {
                //沒AGVC 1 OHBC命令 需判斷一個空盒情形下的動作
                if (_AGVCFromEQToStationCmdNum == 0)
                {
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + _AGVStationID + " Enter the OneEmptyBox0AGVC1OHBC swap");
                    _isMoreOutMode = true;
                    _isOK = OneEmptyBox0AGVC1OHBC(_AGVStationID, _accessAGVPortDatas, _emptyBoxNumber);
                }
                //有2筆AGVC命令 1 OHBC命令 直接走 1 in 1 out 流程 且不論目前狀態，都回可以大量入庫。
                else if (_AGVCFromEQToStationCmdNum >= 1)
                {
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + _AGVStationID + " Enter the One in One Out swap");
                    InOutModeChange(_accessAGVPortDatas, _AGVStationID);
                    _isMoreOutMode = false;
                    _isOK = true;
                }
            }
            else if (_OHBCCmdNumber >= 2)
            {
                //沒AGVC 2 OHBC命令 都轉OUT 回多出
                if (_AGVCFromEQToStationCmdNum == 0)
                {
                    if (_fullBoxNumber > 1)
                    {
                        AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + _AGVStationID + " Enter the OutputModeChange 0A 2B More out swap, but have real box on port.");
                        _isOK = false;
                    }
                    else
                    {
                        AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + _AGVStationID + " Enter the OutputModeChange 0A 2B More out swap");
                        OutputModeChange(_accessAGVPortDatas, _AGVStationID);
                        _isMoreOutMode = true;
                        _isOK = true;
                    }
                }
                //有1 筆以上 AGVC命令 2  OHBC命令 此處需要判斷多進多出流程，及回復AGVC的內容。
                else if (_AGVCFromEQToStationCmdNum >= 1)
                {
                    // 若為設定多出模式 則都走Out Mode
                    if (_setMoreOutMode)
                    {
                        AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + _AGVStationID + " Enter the OutputModeChange 2A 2B  More out swap");
                        OutputModeChange(_accessAGVPortDatas, _AGVStationID);
                        _isMoreOutMode = true;
                        _isOK = true;
                    }
                    // 若為設定多入模式 則走1 in 1 out
                    else
                    {
                        AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " 虛擬 port: " + _AGVStationID + " Enter the OutputModeChange 2A 2B  More in swap");
                        InOutModeChange(_accessAGVPortDatas, _AGVStationID);
                        _isMoreOutMode = false;
                        _isOK = true;
                    }
                }
            }
            return (_isOK, _isMoreOutMode, _portTypeNum);
        }
        private bool OneEmptyBox1AGVC0OHBC(string AGVStationID, List<PortDef> AGVPortDatas, int emptyBoxNumber)
        {
            bool isSuccess = false;
            List<PortPLCInfo> _AGVPortRealPLCDatas = new List<PortPLCInfo>();
            if (_AGVPortRealPLCDatas.Count() > 1)
            {
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGV " + AGVStationID + "enter 1 Empty Box OneEmptyBox1AGVC0OHBC.");
                if (_AGVPortRealPLCDatas[0].IsModeChangable == true && _AGVPortRealPLCDatas[1].IsModeChangable == true)
                {
                    if (emptyBoxNumber == 1)
                    {
                        getPLCRealInfo(AGVPortDatas, _AGVPortRealPLCDatas);
                        if (_AGVPortRealPLCDatas[0].LoadPosition1 == true)
                        {
                            isSuccess = PortTypeChange(AGVPortDatas[0].PLCPortID, E_PortType.In, "OneEmptyBox1AGVC0OHBC_E");
                        }
                        else if (_AGVPortRealPLCDatas[1].LoadPosition1 == true)
                        {
                            isSuccess = PortTypeChange(AGVPortDatas[1].PLCPortID, E_PortType.In, "OneEmptyBox1AGVC0OHBC_E");
                        }

                    }
                    else
                    {
                        isSuccess = PortTypeChange(AGVPortDatas[0].PLCPortID, E_PortType.In, "OneEmptyBox1AGVC0OHBC");
                    }
                }
                else
                {
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGV Station " + " are not ready for mode change. OneEmptyBox1AGVC0OHBC");
                    return isSuccess;
                }
            }
            SpinWait.SpinUntil(() => false, 200);
            Task.Run(() =>
            {
                CyclingCheckWithdraw(AGVPortDatas);
            });
            return isSuccess;
        }

        private bool OneEmptyBox0AGVC1OHBC(string AGVStationID, List<PortDef> AGVPortDatas, int emptyBoxNumber)
        {
            bool isSuccess = false;
            List<PortPLCInfo> _AGVPortRealPLCDatas = new List<PortPLCInfo>();
            getPLCRealInfo(AGVPortDatas, _AGVPortRealPLCDatas);
            if (_AGVPortRealPLCDatas.Count() > 1)
            {
                AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGV " + AGVStationID + "enter 1 Empty Box OneEmptyBox0AGVC1OHBC.");
                if (_AGVPortRealPLCDatas[0].IsModeChangable == true && _AGVPortRealPLCDatas[1].IsModeChangable == true)
                {
                    if (emptyBoxNumber == 1)
                    {
                        if (_AGVPortRealPLCDatas[0].LoadPosition1 == true && _AGVPortRealPLCDatas[1].LoadPosition1 == false)
                        {
                            isSuccess = PortTypeChange(AGVPortDatas[1].PLCPortID, E_PortType.Out, "OneEmptyBox0AGVC1OHBC_E");
                        }
                        else if (_AGVPortRealPLCDatas[1].LoadPosition1 == true && _AGVPortRealPLCDatas[0].LoadPosition1 == false)
                        {
                            isSuccess = PortTypeChange(AGVPortDatas[0].PLCPortID, E_PortType.Out, "OneEmptyBox0AGVC1OHBC_E");
                        }
                        else if (_AGVPortRealPLCDatas[1].LoadPosition1 == true && _AGVPortRealPLCDatas[0].LoadPosition1 == true)
                        {
                            if (_AGVPortRealPLCDatas[0].IsCSTPresence == false)
                            {
                                isSuccess = PortTypeChange(AGVPortDatas[0].PLCPortID, E_PortType.Out, "OneEmptyBox0AGVC1OHBC_E");
                            }
                            else if (_AGVPortRealPLCDatas[1].IsCSTPresence == false)
                            {
                                isSuccess = PortTypeChange(AGVPortDatas[1].PLCPortID, E_PortType.Out, "OneEmptyBox0AGVC1OHBC_E");
                            }
                        }
                    }
                    else
                    {
                        if (_AGVPortRealPLCDatas[0].IsCSTPresence == true && _AGVPortRealPLCDatas[1].IsCSTPresence == false)
                        {
                            isSuccess = PortTypeChange(AGVPortDatas[1].PLCPortID, E_PortType.Out, "OneEmptyBox0AGVC1OHBC_E");
                        }
                        else if (_AGVPortRealPLCDatas[1].IsCSTPresence == true && _AGVPortRealPLCDatas[0].IsCSTPresence == false)
                        {
                            isSuccess = PortTypeChange(AGVPortDatas[0].PLCPortID, E_PortType.Out, "OneEmptyBox0AGVC1OHBC_E");
                        }
                        else if (_AGVPortRealPLCDatas[1].IsCSTPresence == false && _AGVPortRealPLCDatas[0].IsCSTPresence == false)
                        {
                            isSuccess = PortTypeChange(AGVPortDatas[0].PLCPortID, E_PortType.Out, "OneEmptyBox0AGVC1OHBC_E");
                        }
                        else if (_AGVPortRealPLCDatas[1].IsCSTPresence == true && _AGVPortRealPLCDatas[0].IsCSTPresence == true)
                        {
                            AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGV Station " + " are two real box on agv station but no cmd, reject. OneEmptyBox0AGVC1OHBC");
                            return isSuccess;
                        }
                    }
                }
                else
                {
                    AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGV Station " + " are not ready for mode change. OneEmptyBox0AGVC1OHBC");
                    return isSuccess;
                }
            }
            SpinWait.SpinUntil(() => false, 200);
            Task.Run(() =>
            {
                CyclingCheckWithdraw(AGVPortDatas);
            });
            return isSuccess;
        }

        private void CheckThreeFourPortSituationAndMove(string AGVStationID, bool useFirst2Port, int numOfAGVStation, List<PortDef> AGVPortDatas)
        {
            //先判定一次是否有可能有第3個port可以被派送命令。 //若有第3個port 確認其狀態可執行命令後，以其上盒子狀態判定是否要取放貨
            if (numOfAGVStation == 3)
            {
                PortPLCInfo thirdAGVPort = new PortPLCInfo();
                PortDef thirdAGVPort_DB = new PortDef();
                if (useFirst2Port == false) //取3port的第一個
                {
                    thirdAGVPort_DB = AGVPortDatas.FirstOrDefault();
                    thirdAGVPort = GetPLC_PortData(thirdAGVPort_DB.PLCPortID);
                }
                else //取3port的第三個
                {
                    thirdAGVPort_DB = AGVPortDatas.LastOrDefault();
                    thirdAGVPort = GetPLC_PortData(thirdAGVPort_DB.PLCPortID);
                }
                // 當 AgvState 為 OutOfSevice 則可進行取放，因若為 InService 代表當作救帳 Port 使用
                if (thirdAGVPort.OpAutoMode && thirdAGVPort_DB.AGVState == E_PORT_STATUS.OutOfService)
                {
                    if (thirdAGVPort.LoadPosition1 == true && thirdAGVPort.IsCSTPresence == false &&
                        thirdAGVPort.IsReadyToUnload == true && thirdAGVPort.AGVPortReady == true) // 若為空盒，則切為Input Mode
                    {
                        AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGV " + AGVStationID + "enter " + thirdAGVPort_DB.PLCPortID + " third Port Check Method Has box.");
                        PortTypeChange(thirdAGVPort_DB.PLCPortID, E_PortType.In, "Third Port Check Method");
                    }
                    else if (thirdAGVPort.LoadPosition1 == false && thirdAGVPort.IsReadyToLoad == true) //若為空Port 則切為 Output Mode
                    {
                        AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGV " + AGVStationID + "enter " + thirdAGVPort_DB.PLCPortID + " third Port Check Method No box.");
                        PortTypeChange(thirdAGVPort_DB.PLCPortID, E_PortType.Out, "Third Port Check Method");
                    }
                }
            }
            // 在判定是否為 4 port 狀況
            else if (numOfAGVStation == 4)
            {
                PortPLCInfo thirdAGVPort = new PortPLCInfo();
                PortPLCInfo fourAGVPort = new PortPLCInfo();
                PortDef thirdAGVPort_DB = new PortDef();
                PortDef fourAGVPort_DB = new PortDef();
                if (useFirst2Port == false) //取4port的第1 2個
                {
                    thirdAGVPort_DB = AGVPortDatas.Take(2).ToList()[1];
                    fourAGVPort_DB = AGVPortDatas.Take(2).ToList()[0];
                    thirdAGVPort = GetPLC_PortData(thirdAGVPort_DB.PLCPortID);
                    fourAGVPort = GetPLC_PortData(fourAGVPort_DB.PLCPortID);
                }
                else //取4port的第3 4個
                {
                    thirdAGVPort_DB = AGVPortDatas.Take(4).ToList()[2];
                    fourAGVPort_DB = AGVPortDatas.Take(4).ToList()[3];
                    thirdAGVPort = GetPLC_PortData(thirdAGVPort_DB.PLCPortID);
                    fourAGVPort = GetPLC_PortData(fourAGVPort_DB.PLCPortID);
                }
                // 當 AgvState 為 OutOfSevice 則可進行取放，因若為 InService 代表當作救帳 Port 使用
                if (thirdAGVPort.OpAutoMode && thirdAGVPort_DB.AGVState == E_PORT_STATUS.OutOfService)
                {
                    if (thirdAGVPort.LoadPosition1 == true && thirdAGVPort.IsCSTPresence == false &&
                        thirdAGVPort.IsReadyToUnload == true && thirdAGVPort.AGVPortReady == true) // 若為空盒，則切為Input Mode
                    {
                        AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGV " + AGVStationID + "enter " + thirdAGVPort_DB.PLCPortID + " third Port Check Method Has box.");
                        PortTypeChange(thirdAGVPort_DB.PLCPortID, E_PortType.In, "Third Port Check Method");
                    }
                    else if (thirdAGVPort.LoadPosition1 == false && thirdAGVPort.IsReadyToLoad == true) //若為空Port 則切為 Output Mode
                    {
                        AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGV " + AGVStationID + "enter " + thirdAGVPort_DB.PLCPortID + " third Port Check Method No box.");
                        PortTypeChange(thirdAGVPort_DB.PLCPortID, E_PortType.Out, "Third Port Check Method");
                    }
                }
                // 當 AgvState 為 OutOfSevice 則可進行取放，因若為 InService 代表當作救帳 Port 使用
                if (fourAGVPort.OpAutoMode && fourAGVPort_DB.AGVState == E_PORT_STATUS.OutOfService)
                {
                    if (fourAGVPort.LoadPosition1 == true && fourAGVPort.IsCSTPresence == false &&
                        fourAGVPort.IsReadyToUnload == true && fourAGVPort.AGVPortReady == true) // 若為空盒，則切為Input Mode
                    {
                        AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGV " + AGVStationID + "enter " + fourAGVPort_DB.PLCPortID + " fourth Port Check Method Has box.");
                        PortTypeChange(fourAGVPort_DB.PLCPortID, E_PortType.In, "Fourth Port Check Method");
                    }
                    else if (fourAGVPort.LoadPosition1 == false && fourAGVPort.IsReadyToLoad == true) //若為空Port 則切為 Output Mode
                    {
                        AGVCTriggerLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + " AGV " + AGVStationID + "enter " + fourAGVPort_DB.PLCPortID + " fourth Port Check Method No box.");
                        PortTypeChange(fourAGVPort_DB.PLCPortID, E_PortType.Out, "Fourth Port Check Method");
                    }
                }
            }
        }

        private void agvcTriggerAlarmCheck(string AGVStationID, int AGVCFromEQToStationCmdNum)
        {
            //若AGVC 要求0 的時候要清掉異常OHBC_AGV_HasCmdsAccessCleared
            if (AGVCFromEQToStationCmdNum == 0)
            {
                OHBC_AGV_HasCmdsAccessCleared(AGVStationID);
            }
            //若AGVC觸發後，線內空盒數量大於3，則清掉無空盒異常，及線內空盒異常。
            if (GetTotalEmptyBoxNumber().emptyBox.Count() > 3)
            {
                OHBC_AlarmCleared(AGVStationID, ((int)AlarmLst.AGVStation_DontHaveEnoughEmptyBox).ToString());
                OHBC_AlarmCleared(AGVStationID, ((int)AlarmLst.BOX_NumberIsNotEnough).ToString());
            }
            //若AGVC觸發後，線內空盒數量小於3，則觸發線內空盒異常。
            else
            {
                OHBC_AlarmSet(AGVStationID, ((int)AlarmLst.BOX_NumberIsNotEnough).ToString());
            }
        }
        #endregion


        #region Check the unknown CST in the shelf. Try to get the CST ID back. // A20.07.12.0
        public bool CheckAndTryRemarkUnknownCSTInShelf()
        {
            try
            {
                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> OHB| Enter CheckAndTryRemarkUnknownCSTInShelf");
                // 1. 取得目前CST DATA中有異常者
                /**掃描目前BOX CST Data List 確認其中是否有"為UNK 且非UNKU者"若有則選中該CST**/
                List<CassetteData> cassetteData = null;
                cassetteData = cassette_dataBLL.LoadCassetteDataByCSTID_UNKandOnShelf();
                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> OHB| Cst UNK not UNKU on shelf has " + cassetteData.Count());
                // 1.5 找出目前的位置是否為Loop
                if (line.LINE_ID.Contains("LOOP"))
                {
                    foreach (CassetteData CSTData in cassetteData)
                    {
                        PortPLCInfo portInfoT01 = GetPLC_PortData("B7_OHBLOOP_T01");

                        if (cmdBLL.getCMD_ByBoxID(CSTData.BOXID) != null)
                        {
                            continue;
                        }

                        if (BOXMovLine1(portInfoT01, CSTData) == false)
                        {
                            PortPLCInfo portInfoT02 = GetPLC_PortData("B7_OHBLOOP_T02");
                            BOXMovLine1(portInfoT02, CSTData);
                        }
                    }
                    return true;
                }
                // 2. 找出AGV Station 特定Port 
                PortDef targetPort = new PortDef();
                targetPort = FindTargetPort();
                PortPLCInfo targetPortPLCStatus = GetPLC_PortData(targetPort.PLCPortID.Trim());
                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> OHB| FindTargetPort = " + targetPort.PLCPortID);
                // 3. 確認該AGV Port 狀態為Auto 並且為Load OK, 且為Input Type及沒有箱子在上面
                if (targetPortPLCStatus.IsInputMode == true && targetPortPLCStatus.LoadPosition1 == false &&
                    targetPortPLCStatus.OpAutoMode == true && targetPortPLCStatus.IsReadyToLoad == true)
                {
                    // 4. 產生命令由目前位置搬送至指定Port 做確認
                    /****/
                    TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> OHB| Ready to Generate cmd");
                    foreach (CassetteData cstData in cassetteData)
                    {
                        Manual_InsertCmd(cstData.Carrier_LOC, targetPort.PLCPortID, 5, "救回CST資料用");
                    }
                }
                else
                {
                    TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> OHB| targetPort.IsInputMode = " + targetPortPLCStatus.IsInputMode +
                        " targetPort.LoadPosition1 = " + targetPortPLCStatus.LoadPosition1 + " targetPort.OpAutoMode = " +
                        targetPortPLCStatus.OpAutoMode + " targetPort.IsReadyToLoad = " + targetPortPLCStatus.OpAutoMode);
                }
                return true;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "CheckAndTryRemarkUnknownCSTInShelf");
                return false;
            }

        }
        public PortDef FindTargetPort()
        {
            PortDef targetPort = new PortDef();
            foreach (PortDef elementPort in portDefBLL.getAGVPortData())
            {
                if (elementPort.AGVState == E_PORT_STATUS.InService)
                {
                    if (GetPLC_PortData(elementPort.PLCPortID).IsInputMode != true && GetPLC_PortData(elementPort.PLCPortID).IsModeChangable == true)
                    {
                        PortTypeChange(elementPort.PLCPortID, E_PortType.In, "FindTargetPort");
                    }
                    targetPort = elementPort;

                    OpenAGV_Station(targetPort.PLCPortID, false, "FindTargetPort");
                    break;
                }
            }
            return targetPort;
        }

        public bool BOXMovLine1(PortPLCInfo portInfo, CassetteData CSTData)
        {
            bool retResult = false;
            if (portInfo.IsOutputMode)
            {
                string log = "";

                log = Manual_InsertCmd(CSTData.Carrier_LOC, portInfo.EQ_ID, 5, "BOXMovLine1", CmdType.OHBC);

                if (log == "OK")
                {
                    retResult = true;
                }
            }

            return retResult;
        }
        #endregion

        #region disconnection alarm handler
        //2020.07.07
        private void OnLocalDisconnected(object sender, EventArgs e)
        {
            string ohtName = vehicleBLL.cache.loadVhs().FirstOrDefault().VEHICLE_ID;
            OHBC_AlarmSet(ohtName, SCAppConstants.SystemAlarmCode.PLC_Issue.MasterDisconnedted);
        }

        private void OnLocalConnected(object sender, EventArgs e)
        {
            string ohtName = vehicleBLL.cache.loadVhs().FirstOrDefault().VEHICLE_ID;
            OHBC_AlarmCleared(ohtName, SCAppConstants.SystemAlarmCode.PLC_Issue.MasterDisconnedted);
        }

        #endregion
    }
}
