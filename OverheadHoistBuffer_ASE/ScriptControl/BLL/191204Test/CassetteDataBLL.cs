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

namespace com.mirle.ibg3k0.sc.BLL
{
    public enum UpdateCassetteTimeType
    {
        StoreDT,
        WaitOutOPDT,
        WaitOutLPDT,
    }

    public class CassetteDataBLL
    {
        SCApplication scApp = null;
        CassetteDataDao cassettedataDao = null;
        ZoneDefDao zonedefDao = null;
        ShelfDefDao shelfdefDao = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static Logger TransferServiceLogger = LogManager.GetLogger("TransferServiceLogger");

        public void start(SCApplication scApp)
        {
            this.scApp = scApp;
            cassettedataDao = scApp.CassetteDataDao;
            zonedefDao = scApp.ZoneDefDao;
            shelfdefDao = scApp.ShelfDefDao;
        }
        public bool insertCassetteData(CassetteData datainfo)
        {
            bool isSuccess = true;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    datainfo.CSTInDT = DateTime.Now.ToString("yy/MM/dd HH:mm:ss");
                    datainfo.TrnDT = DateTime.Now.ToString("yy/MM/dd HH:mm:ss");
                    cassettedataDao.insertCassetteData(con, datainfo);

                    if (scApp.TransferService.isShelfPort(datainfo.Carrier_LOC)) //若目的為儲位，設定為有卡匣
                    {
                        scApp.ShelfDefBLL.updateStatus(datainfo.Carrier_LOC, ShelfDef.E_ShelfState.Stored);
                    }

                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ")
                        + "OHB >> DB|卡匣新增成功：" + scApp.TransferService.GetCstLog(datainfo)
                    );
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
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //CassetteData cstData = con.CassetteData.Where(data => data.BOXID == boxid).First();  //沒透過dao查詢?? kevinwei
                    CassetteData portCSTData = loadCassetteDataByLoc(loc);  //檢查同個位置是否有帳

                    if (portCSTData != null)
                    {
                        if(portCSTData.Stage == stage)
                        {
                            TransferServiceLogger.Info
                            (
                                DateTime.Now.ToString("HH:mm:ss.fff ")
                                + "OHB >> DB|UpdateCSTLoc   發現更新位置有帳，刪除：" + scApp.TransferService.GetCstLog(portCSTData)
                            );

                            scApp.ReportBLL.ReportCarrierRemovedCompleted(portCSTData.CSTID, portCSTData.BOXID);
                        }
                    }

                    CassetteData cstData = cassettedataDao.LoadCassetteDataByBoxID(con, boxid);

                    if (scApp.TransferService.isUnitType(cstData.Carrier_LOC, Service.UnitType.SHELF))
                    {
                        scApp.ShelfDefBLL.updateStatus(cstData.Carrier_LOC, ShelfDef.E_ShelfState.EmptyShelf);
                    }

                    if (scApp.TransferService.isUnitType(loc, Service.UnitType.SHELF))
                    {
                        scApp.ShelfDefBLL.updateStatus(loc, ShelfDef.E_ShelfState.Stored);
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
                    cstData.CSTID = cstid;
                    cstData.LotID = lotID;
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
            bool isSuccess = true;
            string time = DateTime.Now.ToString("yy/MM/dd HH:mm:ss");

            try
            {
                //TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> DB|UpdateCST_StoreDT:BoxID:" + boxid + "   StoreDT:" + time);

                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    switch(timeType)
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
                    }
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
        public List<CassetteData> loadCassetteData()
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cassettedataDao.LoadCassetteData(con);
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
                if(scApp.TransferService.isLocExist(shiefid))
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
                    if(cstData != null)
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
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    CassetteData csidData = con.CassetteData.Where(data => data.CSTID.Trim() == cstid.Trim() && data.BOXID.Trim() == boxid.Trim()).First();
                    cassettedataDao.DeleteCassetteData(con, csidData);

                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "PLC >> OHB|刪除 " + scApp.TransferService.GetCstLog(csidData)
                    );

                    if (scApp.TransferService.isUnitType(csidData.Carrier_LOC, Service.UnitType.SHELF))
                    {
                        scApp.ShelfDefBLL.updateStatus(csidData.Carrier_LOC, ShelfDef.E_ShelfState.EmptyShelf);
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccsess = false;
            }

            return isSuccsess;
        }
    }
}
