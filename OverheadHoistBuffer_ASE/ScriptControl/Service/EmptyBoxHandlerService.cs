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
        private SCApplication scApp = null;
        private CassetteDataBLL cassette_dataBLL = null;
        private ShelfDefBLL shelfDefBLL = null;
        private ZoneDefBLL zoneBLL = null;
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private NLog.Logger emptyBoxLogger = NLog.LogManager.GetLogger("EmptyBoxHandlerServiceLogger");

        //2020.06.16 緊急水位設定(以比例計算，占用儲格超過這個比例就是達緊急水位)
        private double emergencyWaterLevel = 0.95;

        public void start(SCApplication _app)
        {
            emptyBoxLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "===== EmptyBoxHandlerService start =====");
            scApp = _app;
            cassette_dataBLL = _app.CassetteDataBLL;
            shelfDefBLL = _app.ShelfDefBLL;
            zoneBLL = _app.ZoneDefBLL;
        }

        #region Use for check the empty box number and transport for empty box
        public void CheckTheEmptyBoxStockLevel()
        {
            emptyBoxLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "[CheckTheEmptyBoxStockLevel]");

            List<ZoneDef> zoneDatas = zoneBLL.loadZoneData();

            //先確認目前的line 上shelf的空box 是否夠用(目前標準為AGV station 數量)
            var emptyBox = GetTotalEmptyBoxNumber();
            if (emptyBox.isSuccess == true)
            {
                int requriedBoxAGV;
                var isEnoughEmptyBox = CheckIsEnoughEmptyBox(emptyBox.emptyBox.Count, out requriedBoxAGV);
                emptyBoxLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + $"AGV ST needs {requriedBoxAGV} box(es), now has {emptyBox.emptyBox.Count}");
                if (isEnoughEmptyBox.isSuccess == true)
                {
                    //A20.05.28.0
                    //夠用，則確認目前總水位是否過高，若過高則退掉多餘Empty Box到 CV上。
                    //2020/06/15: 如果未達緊急水位，就只向MCS要求退空box，然後等待MCS S2F49搬出(ref: Mirle E88 spec v1.1 P79 "Empty Box Recycling")
                    //如果已達緊急水位，Line直接送到Loop，Loop直接送到Stocker(TODO)
                    if (isEnoughEmptyBox.isEnough == true)
                    {
                        foreach (var zoneData in zoneDatas)
                        {
                            if (CheckIfTooMuchBox(zoneData, out int boxCount))
                            {
                                emptyBoxLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + $"{zoneData.ZoneID} has {boxCount} box(es), reaches high water level: {zoneData.HighWaterMark}, recycling empty box...");
                                if (boxCount > zoneBLL.GetZoneTotalSize(zoneData.ZoneID) * emergencyWaterLevel)
                                {
                                    //已達緊急水位，產生往Loop or STK的manual command退box
                                    emptyBoxLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + $"{zoneData.ZoneID} reaches emergency water level: {zoneBLL.GetZoneTotalSize(zoneData.ZoneID) * emergencyWaterLevel}, force to send empty box to STK or OHCV...");
                                    //TODO: 第二個parameter填入out mode下的STK port，沒有就找out mode下的OHCV port
                                    //scApp.TransferService.Manual_InsertCmd(emptyBox.emptyBox.FirstOrDefault().Carrier_LOC, "OHCV");
                                }
                                else
                                {
                                    //還沒到緊急水位走這邊
                                    //過多box，呼叫MCS退掉(優先退空的)
                                    emptyBoxLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + $"{zoneData.ZoneID} do not reach emergency water level, just notice MCS and wait transfer command to recycling...");
                                    DoSendPopEmptyBoxToMCS(emptyBox.emptyBox.FirstOrDefault().BOXID);
                                }
                            }
                            else if (CheckIfBoxNotEnough(zoneData, out int emptyBoxCount))
                            {
                                emptyBoxLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + $"{zoneData.ZoneID} has {emptyBoxCount} empty box(es), reaches low water level: {zoneData.LowWaterMark}, request for empty box...");
                                //空box不足，呼叫MCS補充
                                DoSendRequireEmptyBoxToMCS(zoneData.ZoneID, (int)(zoneData.LowWaterMark - emptyBoxCount));
                            }
                        }
                    }
                    else //空box不夠，要補
                    {
                        emptyBoxLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + $"Not enough empty box for AGV ST use, request for empty box...");
                        DoSendRequireEmptyBoxToMCS(zoneDatas.FirstOrDefault().ZoneID, requriedBoxAGV);
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
                emptyBoxLogger.Error(ex, DateTime.Now.ToString("HH:mm:ss.fff ") + "[GetTotalEmptyBoxNumber]");
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
                emptyBoxLogger.Error(ex, DateTime.Now.ToString("HH:mm:ss.fff ") + "[CheckIsEnoughEmptyBox]");
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
            scApp.ReportBLL.ReportEmptyBoxSupply(requireBox.ToString(), zoneID);
        }

        //*******************
        //A20.05.28.0 這邊需要志昌幫忙實作跟MCS"退掉"空box的流程。
        private void DoSendPopEmptyBoxToMCS(string boxID)
        {
            //紀錄log 因此處為實際執行命令之處
            //此部分需先確認目前沒有可執行的MCS命令，才進行要求退BOX動作。
            scApp.ReportBLL.ReportEmptyBoxRecycling(boxID);
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
                emptyBoxLogger.Error(ex, DateTime.Now.ToString("HH:mm:ss.fff ") + "[CheckIfBoxNotEnough]");
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
                emptyBoxLogger.Error(ex, DateTime.Now.ToString("HH:mm:ss.fff ") + "[CheckIfTooMuchBox]");
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
                emptyBoxLogger.Error(ex, DateTime.Now.ToString("HH:mm:ss.fff ") + "[IsBelongsZone]");
                return false;
            }
        }
        #endregion
    }
}
