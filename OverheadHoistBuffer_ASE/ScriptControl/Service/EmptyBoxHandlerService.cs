//*********************************************************************************
//      EmptyBoxHandlerService.cs
//*********************************************************************************
// File Name: EmptyBoxHandlerService.cs
// Description: 處理空盒的service
// Reference: Mirle E88 spec v1.1
//
//(c) Copyright 2020, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag          Description
// ------------- -------------  -------------  ------       -----------------------------
// 2020/06/01    Hsinyu Chang   N/A            A20.06.01a   從TransferService分割出來
// 2020/06/01    Hsinyu Chang   N/A            A20.06.01b   高低水位以database內的定義為準
// 2020/06/22    Hsinyu Chang   N/A            A20.06.22a   新增Zone內的空箱&實箱列表
// 2020/06/22    Hsinyu Chang   N/A            A20.06.22b   重構流程
// 2020/06/23    Hsinyu Chang   N/A            A20.06.23    統計zone內空box時，排除已標記成預約取的
//**********************************************************************************

using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Security.Policy;

namespace com.mirle.ibg3k0.sc.Service
{
    public class EmptyBoxHandlerService
    {
        //2020.6.22 每檢查N次高水位，檢查1次低水位
        private const int lowLevelCheckTime = 4;
        //2020.06.16 緊急水位設定(以比例計算，占用儲格超過這個比例就是達緊急水位)
        private double emergencyWaterLevel = 0.95;

        private SCApplication scApp = null;
        private CassetteDataBLL cassette_dataBLL = null;
        private ShelfDefBLL shelfDefBLL = null;
        private ZoneDefBLL zoneBLL = null;
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private NLog.Logger emptyBoxLogger = NLog.LogManager.GetLogger("EmptyBoxHandlerLogger");

        private static int lowLevelCheckCount = 0;
        private static bool initializedFlag = false;

        private List<ZoneDef> zoneCacheDatas;
        private List<ZoneDef> zoneDatas;
        private List<CassetteData> boxDatas;
        private List<ShelfDef> shelfDatas;

        public void start(SCApplication _app)
        {
            emptyBoxLogger.Info("===== EmptyBoxHandlerService start =====");
            scApp = _app;
            cassette_dataBLL = _app.CassetteDataBLL;
            shelfDefBLL = _app.ShelfDefBLL;
            zoneBLL = _app.ZoneDefBLL;
            var zoneInfo = zoneBLL.loadZoneData();
            if (zoneInfo.FirstOrDefault().ZoneName.Contains("LOOP"))
            {
                emergencyWaterLevel = 0.6;
            }
        }

        #region Use for check the empty box number and transport for empty box
        public void CheckTheEmptyBoxStockLevel()
        {
            emptyBoxLogger.Info("[CheckTheEmptyBoxStockLevel]");

            zoneDatas = zoneBLL.loadZoneData();
            boxDatas = cassette_dataBLL.loadCassetteData();
            shelfDatas = shelfDefBLL.LoadShelf();
            var emptyBox = GetTotalEmptyBoxNumber();
            if (!initializedFlag)
            {
                zoneCacheDatas = new List<ZoneDef>(zoneDatas);
                foreach (var zoneCache in zoneCacheDatas)
                {
                    zoneCache.EmptyBoxList = new List<string>();
                    zoneCache.SolidBoxList = new List<string>();
                    zoneCache.WaitForRecycleBoxList = new List<string>();
                }
                initializedFlag = true;
                emptyBoxLogger.Info("===== EmptyBoxHandlerService initialized =====");
            }

            //更新zone內的空箱實箱列表
            UpdateZoneData();

            //2020/06/22 Hsinyu Chang
            //A. 高水位檢查: 30秒一次
            //A1: zone內有沒有已經標記成待退的空box，有則跳過這次檢查
            //A2: 檢查是否達緊急水位，有則強制送往STK
            // A2-1: 如果沒有STK可以送，就送往其他OHB
            // A2-2: 連其他OHB也送不過去，呼叫MCS幫忙(to A3)
            //A3: 檢查是否達高水位，是則請求MCS幫退空box
            //B. 低水位檢查: 2分鐘一次
            //B1: 先確認目前的line 上shelf的空box 是否夠用(目前標準為AGV station 數量)
            //B2: 檢查各個zone是否需要補空box

            //高水位檢查
            foreach (ZoneDef zoneData in zoneCacheDatas)
            {
                //A1: zone內有沒有已經標記成待退的空box，有則跳過這次檢查
                if (zoneData.WaitForRecycleBoxList.Count() != 0)
                {
                    continue;
                }
                //A2: 檢查是否達緊急水位，有則強制送往STK，沒有STK就送往OHCV
                if (zoneData.BoxCount > zoneData.ZoneSize * emergencyWaterLevel)
                {
                    //已達緊急水位，產生往Loop or STK的manual command退box
                    emptyBoxLogger.Info($"{zoneData.ZoneID} reaches emergency water level: {zoneData.ZoneSize * emergencyWaterLevel}, force to send empty box to STK or OHCV...");
                    //找out mode下的STK port，沒有就找out mode下的OHCV port
                    string dest = scApp.TransferService.GetSTKorOHCV_OutModePortName();
                    if (string.IsNullOrWhiteSpace(dest) == false)
                    {
                        string recycleBoxID = FindBestRecycleBoxID(zoneData, emptyBox.emptyBox);
                        string recycleBoxLoc = cassette_dataBLL.GetCassetteLocByBoxID(recycleBoxID);
                        scApp.TransferService.Manual_InsertCmd(recycleBoxLoc, dest, 5, "CheckTheEmptyBoxStockLevel", ACMD_MCS.CmdType.OHBC);
                    }
                    else
                    {
                        //沒有找到STK、OHCV為OutMode => 請求MCS幫退
                        emptyBoxLogger.Info($"No port is avaliable for recycling box directly, notice MCS and wait transfer command to recycling...");
                        RecycleBoxByMCS(zoneData, zoneData.BoxCount);
                    }
                }
                //A3: 檢查是否達高水位，是則請求MCS幫退空box
                else if (zoneData.BoxCount > zoneData.HighWaterMark)
                {
                    //還沒到緊急水位走這邊
                    //過多box，呼叫MCS退掉(優先退空的)
                    emptyBoxLogger.Info($"{zoneData.ZoneID} do not reach emergency water level, just notice MCS and wait transfer command to recycling...");
                    RecycleBoxByMCS(zoneData, zoneData.BoxCount);
                }
            }

            //低水位檢查
            if (lowLevelCheckCount < lowLevelCheckTime)
            {
                lowLevelCheckCount++;
            }
            else
            {
                lowLevelCheckCount = 0;
                //B1: 先確認目前的line 上shelf的空box 是否夠用(目前標準為AGV station 數量)
                if (emptyBox.isSuccess == true)
                {
                    int requriedBoxAGV;
                    List<CassetteData> emptyBoxList = new List<CassetteData>(emptyBox.emptyBox);
                    var isEnoughEmptyBox = CheckIsEnoughEmptyBox(emptyBoxList.Count, out requriedBoxAGV);
                    if (isEnoughEmptyBox.isEnough == false)
                    {
                        emptyBoxLogger.Info($"Not enough empty box for AGV ST use, request for empty box...");
                        DoSendRequireEmptyBoxToMCS(zoneCacheDatas.FirstOrDefault().ZoneID, requriedBoxAGV);
                    }
                    else
                    {
                        //B2: 檢查各個zone是否需要補空box
                        foreach (ZoneDef zoneData in zoneCacheDatas)
                        {
                            if (zoneData.EmptyBoxList.Count() < zoneData.LowWaterMark)
                            {
                                emptyBoxLogger.Info($"{zoneData.ZoneID} has {zoneData.EmptyBoxList.Count()} empty box(es), reaches low water level: {zoneData.LowWaterMark}, request for empty box...");
                                //空box不足，呼叫MCS補充
                                DoSendRequireEmptyBoxToMCS(zoneData.ZoneID, (int)(zoneData.LowWaterMark - zoneData.EmptyBoxList.Count()));
                            }
                        }
                    }
                }
            }
        }

        private string FindBestRecycleBoxID(ZoneDef zoneData, List<CassetteData> _emptyBoxList)
        {
            string recycleBlockID = "";
            int requriedBoxAGV = 0;
            int emptyBoxNum = zoneData.EmptyBoxList.Count();
            var isEnoughEmptyBox = CheckIsEnoughEmptyBox(_emptyBoxList.Count - 1, out requriedBoxAGV);
            if (emptyBoxNum != 0 && isEnoughEmptyBox.isEnough == true )
            {
                recycleBlockID = zoneData.EmptyBoxList.FirstOrDefault();
            }
            else
            {
                zoneData.SolidBoxList.Sort();
                recycleBlockID = zoneData.SolidBoxList.FirstOrDefault();
            }
            return recycleBlockID;
        }

        private void RecycleBoxByMCS(ZoneDef zoneData, int boxCount)
        {
            List<string> emptyBoxList = new List<string>(zoneData.EmptyBoxList);
            //多幾個，就退幾次
            for (int i = boxCount; i > zoneData.HighWaterMark; i--)
            {
                string recycledBox = emptyBoxList.FirstOrDefault();
                if (string.IsNullOrEmpty(recycledBox) == false)
                {
                    DoSendPopEmptyBoxToMCS(recycledBox);
                    zoneData.WaitForRecycleBoxList.Add(recycledBox);    //加入待回收box清單
                    emptyBoxList.Remove(recycledBox);
                }
                else
                {
                    //已退光所有空box
                    break;
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
                    scApp.TransferService.isUnitType(data.Carrier_LOC, UnitType.SHELF)
                    ).ToList();
                if (emptyBox_ != null)
                {
                    isSuccess_ = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                emptyBoxLogger.Error(ex, "[GetTotalEmptyBoxNumber]");
            }
            return (emptyBox_, isSuccess_);
        }

        //*******************
        //A20.05.28.0 判斷目前的空BOX數量是否滿足需求數量(目前需求數量是用AGV Station 數量判斷)
        private (bool isEnough, bool isSuccess) CheckIsEnoughEmptyBox(int emptyBoxNumber, out int requireBox)
        {
            bool isSuccess_ = false;
            bool isEnough_ = false;
            requireBox = 0;
            try
            {
                List<PortDef> AGV_station = scApp.PortDefBLL.getAGVPortData();
                if (AGV_station.Count() <= emptyBoxNumber)
                {
                    isEnough_ = true;
                }
                else
                {
                    requireBox = AGV_station.Count() - emptyBoxNumber;
                }
                isSuccess_ = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                emptyBoxLogger.Error(ex, "[CheckIsEnoughEmptyBox]");
            }
            return (isEnough_, isSuccess_);
        }

        //*******************
        //A20.05.29.0 確認目前是否有可執行的MCS cmd
        private bool CheckMCSCmdCanProcess()
        {
            bool result = false;
            //確認目前MCS cmd狀態。
            //怎樣叫做有可執行的MCS命令?
            return result;
        }
        //*******************
        //A20.05.28.0 這邊需要志昌幫忙實作跟MCS"要求"空box的流程。 此部分需要想要如何避免重複要empty box 的流程。
        private void DoSendRequireEmptyBoxToMCS(string zoneID, int requireBox)
        {
            //紀錄log 因此處為實際執行命令之處
            //呼叫MCS需要空box
            //若能指定補到哪個zone 則指定。

            if (scApp.TransferService.requireEmptyBox)
            {
                scApp.ReportBLL.ReportEmptyBoxSupply(requireBox.ToString(), zoneID);
            }
        }

        //*******************
        //A20.05.28.0 這邊需要志昌幫忙實作跟MCS"退掉"空box的流程。
        private void DoSendPopEmptyBoxToMCS(string boxID)
        {
            //紀錄log 因此處為實際執行命令之處
            //此部分需先確認目前沒有可執行的MCS命令，才進行要求退BOX動作。

            if(scApp.TransferService.requireEmptyBox)
            {
                scApp.ReportBLL.ReportEmptyBoxRecycling(boxID);
            }
        }

        //******************
        //A20.05.28.0 這部分細節還需考慮。
        private void DoCheckRegulateEmptyBox()
        {
            //紀錄log 因此處為實際執行命令之處
            //針對每一靠近zone之AGV station數量 配置對應數量的空BOX
        }
        #endregion

        #region calculate waterlevel
        //2020/06/01 空箱數量是否低於低水位
        private bool CheckIfBoxNotEnough(ZoneDef zoneData, out int boxCount)
        {
            try
            {
                //指定zone的空box清單
                var emptyBoxList = cassette_dataBLL.loadCassetteData().
                    Where(data => String.IsNullOrWhiteSpace(data.CSTID) && IsBelongsZone(data.Carrier_LOC, zoneData.ZoneID));
                //數量
                boxCount = emptyBoxList.Count();
                return (boxCount < zoneData.LowWaterMark);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                emptyBoxLogger.Error(ex, "[CheckIfBoxNotEnough]");
                boxCount = 0;
                return false;
            }
        }

        //2020/06/01 空箱&實箱總和數量是否高於高水位
        private bool CheckIfTooMuchBox(ZoneDef zoneData, out int boxCount)
        {
            boxCount = 0;
            try
            {
                //被占用shelf的數量(2020/06/01 先定義預約出、預約入、禁用中都不算)
                boxCount = shelfDefBLL.LoadEnableShelf().Where(data => data.ZoneID == zoneData.ZoneID && data.ShelfState == "S").Count();
                return (boxCount > zoneData.HighWaterMark);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                emptyBoxLogger.Error(ex, "[CheckIfTooMuchBox]");
                return false;
            }
        }

        private bool IsBelongsZone(string shelfID, string zoneID)
        {
            try
            {
                ShelfDef shelfData = shelfDefBLL.loadShelfDataByID(shelfID);
                return shelfData.ZoneID == zoneID;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                emptyBoxLogger.Error(ex, "[IsBelongsZone]");
                return false;
            }
        }

        private void UpdateZoneData()
        {
            foreach (ZoneDef z in zoneCacheDatas)
            {
                //update water mark settings
                var zonedata = from zd in zoneDatas
                              where zd.ZoneID == z.ZoneID
                              select zd;
                z.LowWaterMark = Decimal.ToInt32((decimal)zonedata.FirstOrDefault().LowWaterMark);
                z.HighWaterMark = Decimal.ToInt32((decimal)zonedata.FirstOrDefault().HighWaterMark);

                //zone size
                var shelfByZone = from s in shelfDatas
                                  where s.ZoneID == z.ZoneID && s.Enable == "Y"
                                  select s;
                z.ZoneSize = shelfByZone.Count();

                //empty box list
                //2020.06.23 排除已標記成預約取的，以免退box時找不到
                var emptyBoxes = from b in boxDatas
                                 from s in shelfDatas
                                 where b.Carrier_LOC == s.ShelfID &&
                                    string.IsNullOrEmpty(b.CSTID) &&
                                    s.ZoneID == z.ZoneID &&
                                    s.ShelfState != ShelfDef.E_ShelfState.RetrievalReserved
                                 select b.BOXID;
                //box(es) with cassette
                var solidBoxes = from b in boxDatas
                                 from s in shelfDatas
                                 where b.Carrier_LOC == s.ShelfID &&
                                    !string.IsNullOrEmpty(b.CSTID) &&
                                    s.ZoneID == z.ZoneID
                                 orderby b.StoreDT 
                                 select b.BOXID;
                z.EmptyBoxList = emptyBoxes.ToList();
                z.SolidBoxList = solidBoxes.ToList();
                emptyBoxLogger.Info($"{z.ZoneID} Size = {z.ZoneSize}, empty box count = {z.EmptyBoxList.Count()}, solid box count = {z.SolidBoxList.Count()}");
                emptyBoxLogger.Info($"{z.ZoneID} Low waterlevel = {z.LowWaterMark}, high waterlevel = {z.HighWaterMark}," +
                    $" emergency waterlevel = {z.ZoneSize * emergencyWaterLevel}");

                //找出已經回收成功的box
                var recycledBoxes = from x in z.WaitForRecycleBoxList
                                    where !(z.EmptyBoxList.Contains(x))
                                    select x;
                //把已經回收的box移出待回收列表
                foreach (var box in recycledBoxes)
                {
                    emptyBoxLogger.Info($"Box ID {box} is already recycled from {z.ZoneID}");
                    z.WaitForRecycleBoxList.Remove(box);
                }
            }
        }
        #endregion
    }
}
