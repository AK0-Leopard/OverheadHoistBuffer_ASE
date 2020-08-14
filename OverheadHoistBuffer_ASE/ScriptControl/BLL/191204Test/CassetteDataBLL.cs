using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.sc.Data.DAO;
using NLog;
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Service;

namespace com.mirle.ibg3k0.sc.BLL
{
    public enum UpdateCassetteTimeType
    {
        StoreDT,
        WaitOutOPDT,
        WaitOutLPDT,
        TrnDT,
    }

    public class CassetteDataBLL
    {
        SCApplication scApp = null;
        CassetteDataDao cassettedataDao = null;
        ZoneDefDao zonedefDao = null;
        ShelfDefDao shelfdefDao = null;
        public Redis redis = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static Logger TransferServiceLogger = LogManager.GetLogger("TransferServiceLogger");



        public void start(SCApplication scApp)
        {
            this.scApp = scApp;
            cassettedataDao = scApp.CassetteDataDao;
            zonedefDao = scApp.ZoneDefDao;
            shelfdefDao = scApp.ShelfDefDao;
            redis = new Redis(scApp.getRedisCacheManager());
        }
        public bool insertCassetteData(CassetteData datainfo)
        {
            bool isSuccess = true;
            try
            {
                datainfo.CSTID = datainfo.CSTID.Trim();
                datainfo.BOXID = datainfo.BOXID.Trim();
                datainfo.Carrier_LOC = datainfo.Carrier_LOC.Trim();
                datainfo.CSTInDT = DateTime.Now.ToString("yy/MM/dd HH:mm:ss");
                datainfo.TrnDT = DateTime.Now.ToString("yy/MM/dd HH:mm:ss");

                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    cassettedataDao.insertCassetteData(con, datainfo);
                }

                if (scApp.TransferService.isShelfPort(datainfo.Carrier_LOC)) //若目的為儲位，設定為有卡匣
                {
                    scApp.ShelfDefBLL.updateStatus(datainfo.Carrier_LOC, ShelfDef.E_ShelfState.Stored);
                }

                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "OHB >> DB|卡匣新增成功：" + scApp.TransferService.GetCstLog(datainfo)
                );

                if (scApp.TransferService.isUnitType(datainfo.Carrier_LOC, Service.UnitType.AGV))
                {
                    scApp.TransferService.Redis_AddCstBox(datainfo);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }
            return isSuccess;
        }
        public bool UpdateCSTState(string boxid, int state)
        {
            bool isSuccess = true;

            try
            {
                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> DB|UpdateCSTState:BoxID:" + boxid + "   State:" + (E_CSTState)state);
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    con.CassetteData.Where(csid => csid.BOXID == boxid).First().CSTState = (E_CSTState)state;
                    cassettedataDao.UpdateCassetteData(con);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }
            return isSuccess;
        }
        public bool UpdateCSTDataByID(string cstid, string boxid, string locid)
        {
            bool isSuccess = true;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    var cassette = con.CassetteData.Where(x => x.BOXID == boxid).FirstOrDefault();
                    cassette.CSTID = cstid;
                    cassette.Carrier_LOC = locid;
                    cassettedataDao.UpdateCassetteData(con);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }
            return isSuccess;
        }
        public bool DeleteCSTDataByID(string cstid, string boxid)
        {
            bool isSuccess = true;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    var quary = con.CassetteData
                        .Where(data => data.CSTID == cstid && data.BOXID == boxid)
                        .FirstOrDefault();
                    cassettedataDao.DeleteCassetteData(con, quary);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }
            return isSuccess;
        }
        public bool UpdateCSTLoc(string boxid, string loc, int stage)
        {
            bool isSuccess = true;
            try
            {
                CassetteData portCSTData = loadCassetteDataByLoc(loc);  //檢查同個位置是否有帳

                if (portCSTData != null)
                {
                    if (portCSTData.BOXID.Trim() == boxid.Trim())
                    {
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ")
                            + "OHB >> DB|UpdateCSTLoc   發現更新位置有帳，BOXID相同不做更新：" + scApp.TransferService.GetCstLog(portCSTData)
                        );

                        return true;
                    }

                    if (portCSTData.Stage == stage)
                    {
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ")
                            + "OHB >> DB|UpdateCSTLoc   發現更新位置有帳，刪除：" + scApp.TransferService.GetCstLog(portCSTData)
                        );

                        scApp.ReportBLL.ReportCarrierRemovedCompleted(portCSTData.CSTID, portCSTData.BOXID);
                    }
                }

                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    CassetteData cstData = cassettedataDao.LoadCassetteDataByBoxID(con, boxid);
                    string time = DateTime.Now.ToString("yy/MM/dd HH:mm:ss");

                    if (scApp.TransferService.isUnitType(cstData.Carrier_LOC, Service.UnitType.SHELF))
                    {
                        scApp.ShelfDefBLL.updateStatus(cstData.Carrier_LOC, ShelfDef.E_ShelfState.EmptyShelf);
                    }

                    if (scApp.TransferService.isUnitType(loc, Service.UnitType.SHELF))
                    {
                        scApp.ShelfDefBLL.updateStatus(loc, ShelfDef.E_ShelfState.Stored);
                        cstData.StoreDT = time;
                    }
                    else if (scApp.TransferService.isCVPort(loc))
                    {
                        if (stage == 1)
                        {
                            cstData.WaitOutOPDT = time;
                        }

                        int portStage = scApp.TransferService.portINIData[loc].Stage;

                        if (stage == portStage)
                        {
                            cstData.WaitOutLPDT = time;
                        }
                    }

                    cstData.Carrier_LOC = loc;  //目前卡匣在哪
                    cstData.Stage = stage;
                    cstData.TrnDT = DateTime.Now.ToString("yy/MM/dd HH:mm:ss");

                    cassettedataDao.UpdateCassetteData(con);

                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ")
                        + "OHB >> DB|UpdateCSTLoc   " + scApp.TransferService.GetCstLog(cstData)
                    );
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "UpdateCSTLoc");
                logger.Error(ex, "Exception");
                isSuccess = false;
            }
            return isSuccess;
        }
        public bool UpdateCSTID(string loc, string boxid, string cstid, string lotID)
        {
            bool isSuccess = true;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    CassetteData cstData = con.CassetteData.Where(data => data.Carrier_LOC == loc && data.BOXID == boxid).First();
                    cstData.CSTID = cstid.Trim();
                    cstData.LotID = lotID.Trim();
                    cassettedataDao.UpdateCassetteData(con);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }
            return isSuccess;
        }
        public bool UpdateCST_DateTime(string boxid, UpdateCassetteTimeType timeType)
        {
            boxid = boxid.Trim();
            bool isSuccess = true;
            string time = DateTime.Now.ToString("yy/MM/dd HH:mm:ss");

            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    switch (timeType)
                    {
                        case UpdateCassetteTimeType.StoreDT:
                            cassettedataDao.LoadCassetteDataByBoxID(con, boxid).StoreDT = time;
                            break;
                        case UpdateCassetteTimeType.WaitOutOPDT:
                            cassettedataDao.LoadCassetteDataByBoxID(con, boxid).WaitOutOPDT = time;
                            break;
                        case UpdateCassetteTimeType.WaitOutLPDT:
                            cassettedataDao.LoadCassetteDataByBoxID(con, boxid).WaitOutLPDT = time;
                            break;
                        case UpdateCassetteTimeType.TrnDT:
                            cassettedataDao.LoadCassetteDataByBoxID(con, boxid).TrnDT = time;
                            break;
                    }
                    cassettedataDao.UpdateCassetteData(con);
                }

                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> DB|UpdateCST_DateTime BoxID:" + boxid + "   " + timeType + ":" + time);
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "UpdateCST_DateTime " + boxid + " " + timeType + " " + time);
                logger.Error(ex, "Exception");
                isSuccess = false;
            }
            return isSuccess;
        }
        public List<CassetteData> loadCassetteData()
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    var result = cassettedataDao.LoadCassetteData(con);
                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }

        public List<CassetteData> LoadCassetteDataByCVPort(string portName)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cassettedataDao.LoadCassetteDataByCVPort(con, portName);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }

        public List<CassetteData> LoadCassetteDataByCSTID_UNK()
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cassettedataDao.LoadCassetteDataByCSTID_UNK(con);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        /// <summary>
        /// 找出是UNK 但不是UNKU且在shelf 上的CST
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public List<CassetteData> LoadCassetteDataByCSTID_UNKandOnShelf()
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cassettedataDao.LoadCassetteDataByCSTID_UNKandOnShelf(con);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        public List<CassetteData> LoadCassetteDataByNotCompleted()
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cassettedataDao.LoadCassetteDataByNotCompleted(con);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }

        public List<CassetteData> LoadCassetteDataByCstAndEmptyLotID()  //取得有 CSID 但沒有LOTID 所有資料
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cassettedataDao.LoadCassetteDataByCSTID_UNK(con);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        public List<CassetteData> loadCassetteDataIsUnfinished()
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cassettedataDao.loadCassetteDataIsUnfinished(con);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }

        public CassetteData loadCassetteDataByShelfID(string shelfid)   //判斷卡匣是否存在Shelf
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cassettedataDao.LoadCassetteDataByShelfID(con, shelfid);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

        }
        
        public CassetteData loadCassetteDataByLoc(string portName)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cassettedataDao.LoadCassetteDataByLoc(con, portName);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }

        public List<CassetteData> LoadCassetteDataByOHCV(string portName)   //取得 OHCV Port 上的所有卡匣
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cassettedataDao.LoadCassetteDataByOHCV(con, portName);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        public CassetteData loadCassetteDataByCSTID(string cstid)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cassettedataDao.LoadCassetteDataByCSTID(con, cstid);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        public CassetteData loadCassetteDataByBoxID(string boxid)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //return con.CassetteData.Where(data => data.BOXID == boxid).FirstOrDefault();
                    return cassettedataDao.LoadCassetteDataByBoxID(con, boxid);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        public CassetteData loadCassetteDataByDU_CstID(CassetteData cstData)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //return con.CassetteData.Where(data => data.BOXID == boxid).FirstOrDefault();
                    return cassettedataDao.LoadCassetteDataByDU_CSTID(con, cstData);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        public CassetteData loadCassetteDataByDU_BoxID(CassetteData cstData)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //return con.CassetteData.Where(data => data.BOXID == boxid).FirstOrDefault();
                    return cassettedataDao.LoadCassetteDataByDU_BOXID(con, cstData);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }

        public CassetteData loadCassetteDataByCstBoxID(string cstid, string boxid)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return con.CassetteData.Where(data => data.CSTID.Trim() == cstid.Trim() && data.BOXID.Trim() == boxid.Trim()).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

        }

        public CassetteData GetEmptyBox(string ohtName)   //取得空BOX，oht
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return con.CassetteData.Where(x => x.CSTID == "" && x.Carrier_LOC.Trim() != ohtName).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
            //using (DBConnection_EF con = new DBConnection_EF())

        }

        public string GetZoneName(string shiefid)
        {
            try
            {
                if (scApp.TransferService.isLocExist(shiefid))
                {
                    return scApp.TransferService.portINIData[shiefid].ZoneName;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return "ex";
            }

        }
        public string GetCassetteLocByBoxID(string boxid)
        {
            string cstLoc = "";

            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    CassetteData cstData = con.CassetteData.Where(data => data.BOXID == boxid).FirstOrDefault();
                    if (cstData != null)
                    {
                        cstLoc = cstData.Carrier_LOC;
                    }

                    return cstLoc;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return cstLoc;
            }
        }
        public bool DeleteCSTbyBoxId(string boxid)
        {
            bool isSuccsess = true;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    CassetteData csidData = con.CassetteData.Where(data => data.BOXID.Trim() == boxid.Trim()).First();
                    cassettedataDao.DeleteCassetteData(con, csidData);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccsess = false;
            }

            return isSuccsess;
        }

        public bool DeleteCSTbyCstBoxID(string cstid, string boxid)
        {
            bool isSuccsess = true;
            try
            {
                CassetteData csidData;
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    csidData = con.CassetteData.Where(data => data.CSTID.Trim() == cstid.Trim() && data.BOXID.Trim() == boxid.Trim()).First();
                    
                    cassettedataDao.DeleteCassetteData(con, csidData);
                }

                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "PLC >> OHB|刪除 " + scApp.TransferService.GetCstLog(csidData)
                );

                if (scApp.TransferService.isUnitType(csidData.Carrier_LOC, UnitType.SHELF))
                {
                    scApp.ShelfDefBLL.updateStatus(csidData.Carrier_LOC, ShelfDef.E_ShelfState.EmptyShelf);
                }

                if (scApp.TransferService.isUnitType(csidData.Carrier_LOC, UnitType.AGV))
                {
                    scApp.TransferService.Redis_DeleteCstBox(csidData);
                }

                if (scApp.TransferService.isCVPort(csidData.Carrier_LOC))
                {
                    scApp.TransferService.SetPortWaitOutTimeOutAlarm(csidData.Carrier_LOC, 0);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccsess = false;
            }

            return isSuccsess;
        }

        public List<string> GetAllBoxLoc()
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cassettedataDao.GetOccupiedLocation(con);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }

        public class Redis
        {
            RedisCacheManager redisCacheManager = null;
            public Redis(RedisCacheManager _redisCacheManager)
            {
                redisCacheManager = _redisCacheManager;
            }
            TimeSpan BOXID_WITH_CSTID_OF_TIME_OUT = new TimeSpan(0, 30, 0);
            public void setBoxIDWithCSTID(string boxID, string cstID)
            {
                try
                {
                    redisCacheManager.stringCommonSetAsync(boxID, cstID, BOXID_WITH_CSTID_OF_TIME_OUT);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
            }

            public (bool hasExist, string cstID) tryGetCSTIDByBoxID(string boxID)
            {
                try
                {
                    var get_result = redisCacheManager.StringCommonGet(boxID);
                    return (get_result.HasValue, (string)get_result);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                    return (false, "");
                }
            }
            public void deleteCSTIDByBoxID(string boxID)
            {
                try
                {
                    var get_result = redisCacheManager.keyCommonDeleteAsync(boxID);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
            }
        }
    }
}
