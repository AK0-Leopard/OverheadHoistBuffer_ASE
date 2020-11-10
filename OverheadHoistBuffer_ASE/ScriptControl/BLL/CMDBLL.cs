//**********************************************************************************
// Date          Author         Request No.    Tag         Description
// ------------- -------------  -------------  ------      -----------------------------
// 2020/05/15    Jason Wu       N/A            A20.05.15   嘗試優化對Zone選擇shelf之邏輯
// 2020/05/21    Jason Wu       N/A            A20.05.21   嘗試優化派送命令之優先邏輯
// 2020/05/27    Jason Wu       N/A            A20.05.27.0 修改先後順序，以讓找出之位置正確
// 2020/05/27    Jason Wu       N/A            A20.05.27   在優化shelf 的選擇中加入log紀錄
// 2020/06/04    Jason Wu       N/A            A20.06.04.0 修改load MCS CMD方法，避免一次撈取全部命令。
// 2020/06/09    Jason Wu       N/A            A20.06.09.0 修改紀錄於CombineMCSLogData中之log內容。
// 2020/06/15    Jason Wu       N/A            A20.06.15.0 修改GetCmdDataByDest 中呼叫Dao取得Cmd內容的function，先行於Dao濾掉已完成的命令。
//**********************************************************************************
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.DAO;
using com.mirle.ibg3k0.sc.Data.DAO.EntityFramework;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using Mirle.Hlts.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using static com.mirle.ibg3k0.sc.ACMD_MCS;
using static com.mirle.ibg3k0.sc.ShelfDef; //A20.05.15

namespace com.mirle.ibg3k0.sc.BLL
{
    public class CMDBLL
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        CMD_OHTCDao cmd_ohtcDAO = null;
        CMD_OHTC_DetailDao cmd_ohtc_detailDAO = null;
        CMD_MCSDao cmd_mcsDao = null;
        TestTranTaskDao testTranTaskDao = null;
        ReturnCodeMapDao return_code_mapDao = null;
        ShelfDefDao shelfdefDao = null;
        CassetteDataDao cassettedataDao = null;

        protected static Logger logger_VhRouteLog = LogManager.GetLogger("VhRoute");
        NLog.Logger TransferServiceLogger = NLog.LogManager.GetLogger("TransferServiceLogger");

        private string[] ByPassSegment = null;
        ParkZoneTypeDao parkZoneTypeDao = null;
        private SCApplication scApp = null;



        ALINE line
        {
            get => scApp.getEQObjCacheManager().getLine();
        }

        public CMDBLL()
        {

        }
        public void start(SCApplication app)
        {
            scApp = app;
            cmd_ohtcDAO = scApp.CMD_OHTCDao;
            cmd_ohtc_detailDAO = scApp.CMD_OHT_DetailDao;
            cmd_mcsDao = scApp.CMD_MCSDao;
            parkZoneTypeDao = scApp.ParkZoneTypeDao;
            testTranTaskDao = scApp.TestTranTaskDao;
            return_code_mapDao = scApp.ReturnCodeMapDao;
            shelfdefDao = scApp.ShelfDefDao;
            cassettedataDao = scApp.CassetteDataDao;
            initialByPassSegment();
        }

        private void initialByPassSegment()
        {
            //if (SCUtility.isMatche(scApp.BC_ID, SCAppConstants.WorkVersion.VERSION_NAME_OHS100))
            //{
            //    ByPassSegment = new string[] { "003", "030", "025-026-027", "043", "042", "038", "034" };
            //}
            //else if (SCUtility.isMatche(scApp.BC_ID, SCAppConstants.WorkVersion.VERSION_NAME_TAICHUNG))
            //{
            //    ByPassSegment = new string[] { };
            //}
            //else
            //{
            //    ByPassSegment = new string[] { };
            //}
            ByPassSegment = new string[] { };
        }
        public void initialMapAction()
        {

        }

        public bool SourceDestExist(string existName)   //確認來源目的是否存在
        {
            return scApp.TransferService.isLocExist(existName);
        }

        public bool isZone(string zoneid)   //是不是zone
        {
            //return scApp.ZoneDefBLL.IsExist(zoneid);
            return scApp.TransferService.isUnitType(zoneid, Service.UnitType.ZONE);
        }

        #region CMD_MCS
        public List<ACMD_MCS> LoadCmdData()
        {
            //using (DBConnection_EF con = new DBConnection_EF())
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                //A20.06.04.0
                //已增加新方法，LoadCmdData_WithoutComplete取代原本此處的LoadCmdData，將判斷是否為transferComplete 換到資料庫實作，避免撈出所有資料花費時間過久。
                return cmd_mcsDao.LoadCmdData_WithoutComplete(con)
                    .Where(data => data.TRANSFERSTATE != E_TRAN_STATUS.TransferCompleted)
                    .OrderByDescending(data => data.PRIORITY_SUM)
                    .ThenBy(data => data.CMD_INSER_TIME)
                    .ToList();
            }
        }

        public List<ACMD_MCS> LoadAllCmdData()  //歷史紀錄
        {
            //using (DBConnection_EF con = new DBConnection_EF())
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                return cmd_mcsDao.LoadAllCmdData(con);
            }
        }
        public List<ACMD_MCS> LoadCmdDataByStartEnd(DateTime startTime, DateTime endTime)  //歷史紀錄
        {
            //using (DBConnection_EF con = new DBConnection_EF())
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                return cmd_mcsDao.LoadCmdDataByStartEnd(con, startTime, endTime);
            }
        }
        public ACMD_MCS GetCmdIDFromCmd(string cmdid)   //取得有此CmdID的命令
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cmd_mcsDao.LoadCmdData(con).Where(cmdData => cmdData.CMD_ID.Trim() == cmdid).First();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

        }

        public ACMD_MCS GetBoxFromCmd(string boxid) //取得有此BOXID的命令
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cmd_mcsDao.LoadCmdData(con).Where(cmdData => cmdData.BOX_ID.Trim() == boxid
                                                            && cmdData.TRANSFERSTATE != E_TRAN_STATUS.TransferCompleted).First();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

        }

        public ACMD_MCS GetCmdDataBySource(string portName) //取得來源為Port的命令
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cmd_mcsDao.LoadCmdData_WithoutComplete(con).Where(cmdData =>
                                                                  (cmdData.HOSTSOURCE.Trim() == portName.Trim() || (cmdData.RelayStation?.Trim() ?? "") == portName.Trim())
                                                                && cmdData.TRANSFERSTATE != E_TRAN_STATUS.TransferCompleted).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

        }

        public List<ACMD_MCS> GetCmdDataByDest(string portName) //取得目的為AGV Port的命令
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //A20.06.15.0
                    return cmd_mcsDao.LoadCmdData_WithoutComplete(con).Where(cmdData =>
                                                                  (cmdData.HOSTDESTINATION.Trim() == portName.Trim() || (cmdData.RelayStation?.Trim() ?? "") == portName.Trim())
                                                                && cmdData.TRANSFERSTATE != E_TRAN_STATUS.TransferCompleted).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        public List<ACMD_MCS> GetCmdDataByDestAndByPassManaul(string portName) //取得目的為AGV Port的命令
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //A20.06.15.0
                    return cmd_mcsDao.LoadCmdData_WithoutComplete(con).Where(cmdData => cmdData.HOSTDESTINATION.Trim() == portName.Trim()
                                                                                        && cmdData.TRANSFERSTATE != E_TRAN_STATUS.TransferCompleted
                                                                                        && !cmdData.CMD_ID.Contains("MANAUL")).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }


        public ACMD_MCS GetCmdDataByAGVtoSHELF(string SourcePortName)  //取得退BOX的命令
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cmd_mcsDao.GetCmdDataByAGVtoSHELF(con, SourcePortName);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        public ACMD_MCS GetCmdDataBySHELFtoAGV(string DestPortName)  //取得補BOX的命令
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cmd_mcsDao.GetCmdDataBySHELFtoAGV(con, DestPortName);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        public string doCheckMCSCommand(string command_id, string Priority, string cstID, string box_id, string lotID,
                                        ref string HostSource, ref string HostDestination,
                                        out string check_result, out bool isFromVh)
        {
            check_result = string.Empty;
            string checkcode = SECSConst.HCACK_Confirm;
            int ipriority = 0;
            string from_adr = string.Empty;
            string to_adr = string.Empty;
            E_VH_TYPE vh_type = E_VH_TYPE.None;
            isFromVh = false;

            //if (SCUtility.isEmpty(HostSource) || SCUtility.isEmpty(HostDestination)) //add by Kevin
            //{
            //    check_result = $"來源或目的Port為空";
            //    TransferServiceLogger.Info(check_result);
            //    return SECSConst.HCACK_Param_Invalid;
            //}

            try
            {
                bool isSourceOnVehicle = false;
                if (HostSource != null)
                {
                    isSourceOnVehicle = scApp.VehicleBLL.getVehicleByRealID(HostSource) != null;
                }
                //else//應該不會執行到這行 kevin
                //{
                //    isSourceOnVehicle = scApp.VehicleBLL.getVehicleByRealID(cstData.Carrier_LOC) != null;
                //}

                #region 卡匣是否存在
                CassetteData cstData = scApp.CassetteDataBLL.loadCassetteDataByBoxID(box_id);

                if (cstData == null)
                {
                    TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S2F50: BOXID: " + box_id + " 不存在");
                    return SECSConst.HCACK_Obj_Not_Exist;
                }
                #endregion
                #region 確認命令ID是否重複 
                var cmd_obj = scApp.CMDBLL.getCMD_MCSByID(command_id);
                if (cmd_obj != null)
                {
                    check_result = $"MCS command id:{command_id} 命令已存在.";

                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ")
                        + "MCS >> OHB|S2F50: 命令已存在，"
                        + " CmdID: " + cmd_obj.CMD_ID
                        + " CARRIER_ID: " + cmd_obj.CARRIER_ID
                        + " BOX_ID: " + cmd_obj.BOX_ID
                        + " 來源: " + cmd_obj.HOSTSOURCE
                        + " 目的: " + cmd_obj.HOSTDESTINATION
                        + " 狀態: " + cmd_obj.TRANSFERSTATE
                    );

                    return SECSConst.HCACK_Rejected;
                }

                var cmdbyBox = scApp.CMDBLL.getCMD_ByBoxID(cstData.BOXID);//.getByCstBoxID(cstData.CSTID ,cstData.BOXID);

                if (cmdbyBox != null)
                {
                    if (cmdbyBox.COMMANDSTATE < 64)
                    {
                        check_result = $"MCS boxid:{cstData.BOXID} 已有命令在執行.";

                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ")
                            + "MCS >> OHB|S2F50: BOXID: " + cstData.BOXID + " 已有命令在執行 " + scApp.TransferService.GetCmdLog(cmdbyBox)
                        );

                        if (SCUtility.isMatche(cstData.Carrier_LOC, cmdbyBox.HOSTSOURCE))
                        {
                            return SECSConst.HCACK_Confirm_Executed;    //如果卡匣還沒進車子，要報0
                        }
                        else
                        {
                            return SECSConst.HCACK_Rejected;
                        }
                    }
                }
                #endregion
                #region 來源目的是否存在

                if (string.IsNullOrWhiteSpace(HostSource))   //沒給來源就尋找卡匣在哪 
                {
                    HostSource = cstData.Carrier_LOC;
                }

                if (SourceDestExist(HostSource) == false)
                {
                    TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S2F50: 來源Port: " + HostSource + " 不存在");
                    return SECSConst.HCACK_Obj_Not_Exist;
                }

                if (scApp.TransferService.isCVPort(HostSource))
                {
                    if (HostSource.Trim() != cstData.Carrier_LOC.Trim())
                    {
                        return SECSConst.HCACK_Not_Able_Execute;
                    }
                }

                if (SourceDestExist(HostDestination) == false)
                {
                    TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S2F50: 目的Port: " + HostDestination + " 不存在");
                    return SECSConst.HCACK_Obj_Not_Exist;
                }
                #endregion
                #region 判斷來源目的是不是Zone
                if (isZone(HostSource)) //來源
                {
                    HostSource = cstData.Carrier_LOC;
                }
                #region 目的是Zone，判斷Zone有沒有空的的Shelf
                //if (isZone(HostDestination))
                //{
                //    string zoneID = HostDestination;

                //    List<ShelfDef> shelfData = scApp.ShelfDefBLL.GetEmptyAndEnableShelfByZone(zoneID);//Modify by Kevin

                //    if (shelfData == null)
                //    {
                //        check_result = $"MCS command of source port:{HostSource} and destination port:{HostDestination} is same.";
                //        TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S2F50: 目的 Zone: " + HostDestination + " 沒有位置");
                //        //return SECSConst.HCACK_Rejected;//add by Kevin
                //    }
                //    else
                //    {
                //        TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S2F50: 目的 Zone: " + HostDestination + " 可用儲位數量: " + shelfData.Count);

                //        string shelfID = scApp.TransferService.GetShelfRecentLocation(shelfData, HostSource);

                //        if(string.IsNullOrWhiteSpace(shelfID) == false)
                //        {
                //            TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S2F50: 目的 Zone: " + HostDestination + " 找到 " + shelfID);
                //            HostDestination = shelfID;
                //        }
                //    }
                //}
                #endregion
                #endregion
                bool isSuccess = true;
                if (isSuccess)
                {
                    if (isSourceOnVehicle)
                    {
                        AVEHICLE carray_vh = scApp.VehicleBLL.getVehicleByRealID(HostSource);
                        if (carray_vh.HAS_CST == 0)
                        {
                            check_result = $"Vh:{HostSource.Trim()},not carray cst.";
                            TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S2F50: 車子相關:" + check_result);
                            return SECSConst.HCACK_Rejected;
                        }
                        else
                        {
                            if (!SCUtility.isMatche(carray_vh.BOX_ID, box_id))
                            {
                                check_result = $"Vh:{HostSource.Trim()}, current carray cst id:{carray_vh.BOX_ID} ,not matche host carrier id:{box_id}.";
                                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S2F50: 車子相關:" + check_result);
                                return SECSConst.HCACK_Rejected;
                            }
                            else
                            {
                                isFromVh = true;
                            }
                        }
                    }
                    //else //下面已經會檢查如果不在車上時的狀態 kevin
                    //{
                    //    if (!scApp.MapBLL.getAddressID(HostSource, out from_adr, out vh_type))
                    //    {
                    //        isSuccess = false;
                    //        checkcode = SECSConst.HCACK_Param_Invalid;
                    //        check_result = $"No find {nameof(HostSource)}={HostSource} of adr.";
                    //        TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S2F50: 車子相關:" + check_result);
                    //    }
                    //}
                    else if (scApp.MapBLL.getAddressID(HostSource, out from_adr, out vh_type) == false)
                    {
                        isSuccess = false;
                        checkcode = SECSConst.HCACK_Param_Invalid;
                        check_result = $"No find {nameof(HostSource)}={HostSource} of adr.";
                        TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S2F50: 車子相關:" + check_result);
                    }

                    if (!scApp.MapBLL.getAddressID(HostDestination, out to_adr))
                    {
                        isSuccess = false;
                        checkcode = SECSConst.HCACK_Param_Invalid;
                        check_result = $"No find {nameof(HostDestination)}={HostDestination} of adr.";
                        TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S2F50: 車子相關:" + check_result);
                    }
                }
                if (isSuccess)
                {
                    if (!int.TryParse(Priority, out ipriority))
                    {
                        isSuccess = false;
                        checkcode = SECSConst.HCACK_Param_Invalid;
                        check_result = $"The {nameof(Priority)}:[{Priority}] is invalid ";
                        TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S2F50: 車子相關:" + check_result);
                    }
                }
                if (isSuccess)
                {
                    bool IsSegmentInActive_Source = true;
                    bool IsSegmentInActive_Destination = true;
                    if (!isSourceOnVehicle)
                    {
                        //For Test Bypass 2020/01/12
                        //IsSegmentInActive_Source = scApp.MapBLL.CheckSegmentInActiveByPortID(HostSource);
                        //if (!IsSegmentInActive_Source)
                        //{
                        //    check_result = $"{nameof(HostSource)} : {HostSource} of segment is disable";
                        //}
                    }
                    //For Test Bypass 2020/01/12
                    //IsSegmentInActive_Destination = scApp.MapBLL.CheckSegmentInActiveByPortID(HostDestination);
                    //if (!IsSegmentInActive_Destination)
                    //{
                    //    if (!SCUtility.isEmpty(check_result))
                    //        check_result += "\n";
                    //    check_result += $"{nameof(HostDestination)} : {HostDestination} of segment is disable";
                    //}
                    //if (!IsSegmentInActive_Source || !IsSegmentInActive_Destination)
                    //{
                    //    isSuccess = false;
                    //    checkcode = SECSConst.HCACK_Rejected;
                    //}
                }
                //確認有VH可以派送
                if (!isSourceOnVehicle && isSuccess)
                {
                    scApp.MapBLL.getAddressID(HostSource, out from_adr, out vh_type);
                    //AVEHICLE may_be_can_carry_vh = scApp.VehicleBLL.findBestSuitableVhStepByStepFromAdr(from_adr, vh_type
                    //                                                                                    , is_check_has_vh_carry: true);
                    AVEHICLE may_be_can_carry_vh = scApp.VehicleBLL.findBestSuitableVhStepByNearest(from_adr, vh_type
                                                                                                    , is_check_has_vh_carry: true);
                    if (may_be_can_carry_vh == null)
                    {
                        isSuccess = false;
                        checkcode = SECSConst.HCACK_Rejected;
                        check_result = "Can't find vehicle to carry";
                        TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S2F50: 車子相關:" + check_result);
                    }
                }
                //確認Source 2 Traget可以通
                if (!isSourceOnVehicle && isSuccess)
                {
                    if (!scApp.GuideBLL.IsRoadWalkable(from_adr, to_adr).isSuccess)
                    {
                        isSuccess = false;
                        checkcode = SECSConst.HCACK_Rejected;
                        check_result = "The road is not walkable, source to destination ";
                        TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S2F50: 車子相關:" + check_result);
                    }
                }

                checkcode = SECSConst.HCACK_Confirm;

                return checkcode;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                TransferServiceLogger.Error(ex, "doCheckMCSCommand");
                check_result = "程式跳出例外";
                return SECSConst.HCACK_Rejected;
            }
        }
        //*************************************************
        //A20.05.27
        public string CombineShelfLogData(List<ShelfDef> shelfData)
        {
            string shelfSort = "";
            foreach (var v in shelfData.Take(5).ToList())
            {
                shelfSort = shelfSort + v.ShelfID + " distance：" + v.distanceFromHostSource + "，";
            }

            return shelfSort;
        }
        //*************************************************
        //A20.05.27
        //A20.06.09.0
        public string CombineMCSLogData(List<ACMD_MCS> cmdMCSData)
        {
            string cmdMCSSort = "";
            foreach (var v in cmdMCSData.Take(5).ToList())
            {
                cmdMCSSort = cmdMCSSort + v.CMD_ID + " Source ：" + v.HOSTSOURCE + " Priority_SUM ：" + v.PRIORITY_SUM + " distance：" + v.DistanceFromVehicleToHostSource + "，";
            }

            return cmdMCSSort;
        }

        public bool doCreatMCSCommand(string command_id, string Priority, string replace, string carrier_id, string HostSource, string HostDestination, string Box_ID, string LOT_ID, string box_Loc, string checkcode, bool isFromVh)
        {
            bool isSuccess = true;
            int ipriority = 0;
            if (!int.TryParse(Priority, out ipriority))
            {
                logger.Warn("command id :{0} of priority parse fail. priority valus:{1}"
                            , command_id
                            , Priority);
            }
            int ireplace = 0;
            //if (!int.TryParse(replace, out ireplace))
            //{
            //    logger.Warn("command id :{0} of priority parse fail. priority valus:{1}"
            //                , command_id
            //                , replace);
            //}


            //ACMD_MCS mcs_com = creatCommand_MCS(command_id, ipriority, carrier_id, HostSource, HostDestination, checkcode);

            creatCommand_MCS(command_id, ipriority, ireplace, carrier_id, HostSource, HostDestination, Box_ID, LOT_ID, box_Loc, checkcode, isFromVh);

            CassetteData cstData = scApp.CassetteDataBLL.loadCassetteDataByBoxID(Box_ID);

            if (cstData != null)
            {
                scApp.CassetteDataBLL.UpdateCSTID(cstData.Carrier_LOC, cstData.BOXID, carrier_id.Trim(), LOT_ID.Trim());
            }

            //if (mcs_com != null)
            //{
            //    isSuccess = true;
            //    scApp.SysExcuteQualityBLL.creatSysExcuteQuality(mcs_com);
            //    //mcsDefaultMapAction.sendS6F11_TranInit(command_id);
            //    scApp.ReportBLL.doReportTransferInitial(command_id);
            //    checkMCS_TransferCommand();
            //}

            return isSuccess;

        }

        public ACMD_MCS creatCommand_MCS(string command_id, int Priority, int replace, string carrier_id, string HostSource, string HostDestination, string Box_ID, string LOT_ID, string carrier_Loc, string checkcode, bool isFromVh)
        {
            int port_priority = 0;

            try
            {
                if (!SCUtility.isEmpty(HostSource))
                {
                    APORTSTATION source_portStation = scApp.getEQObjCacheManager().getPortStation(HostSource);

                    if (source_portStation == null)
                    {
                        logger.Warn($"MCS cmd of hostsource port[{HostSource} not exist.]");
                    }
                    else
                    {
                        port_priority = source_portStation.PRIORITY;
                    }
                }
                ACMD_MCS cmd = new ACMD_MCS()
                {
                    CARRIER_ID = carrier_id,
                    CMD_ID = command_id,
                    TRANSFERSTATE = E_TRAN_STATUS.Queue,
                    //COMMANDSTATE = SCAppConstants.TaskCmdStatus.Queue,
                    COMMANDSTATE = 0,
                    HOSTSOURCE = HostSource,
                    HOSTDESTINATION = HostDestination,
                    PRIORITY = Priority,
                    CHECKCODE = checkcode,
                    PAUSEFLAG = "0",
                    CMD_INSER_TIME = DateTime.Now,
                    TIME_PRIORITY = 0,
                    PORT_PRIORITY = port_priority,
                    PRIORITY_SUM = Priority + port_priority,
                    REPLACE = replace,
                    BOX_ID = Box_ID,
                    LOT_ID = LOT_ID,
                    CARRIER_LOC = carrier_Loc,
                    CMDTYPE = ACMD_MCS.CmdType.MCS.ToString(),
                    CRANE = "",
                };
                if (isFromVh)   //box一開始就在車上，不會掃barcode，直接假設barcode符合
                {
                    cmd.CARRIER_ID_ON_CRANE = Box_ID;
                }
                if (creatCommand_MCS(cmd))
                {
                    return cmd;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        public bool creatCommand_MCS(ACMD_MCS cmd_mcs)
        {
            bool isSuccess = true;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    cmd_mcsDao.add(con, cmd_mcs);
                    con.Entry(cmd_mcs).State = EntityState.Detached;
                }

                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "OHT >> OHB|"
                    + "新增命令: " + scApp.TransferService.GetCmdLog(cmd_mcs)
                );

                if (cmd_mcs.CMDTYPE != CmdType.PortTypeChange.ToString())
                {
                    scApp.TransferService.ShelfReserved(cmd_mcs.HOSTSOURCE, cmd_mcs.HOSTDESTINATION);
                }

                Task.Run(() =>  //20_0824 冠皚提出車子回 128 結束，直接掃命令，不要等到下次執行緒觸發
                {
                    scApp.TransferService.TransferRun();
                });
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "creatCommand_MCS");
                logger.Error(ex, "Exception");
                isSuccess = false;
            }

            return isSuccess;
        }
        //*************************************************
        //A20.05.15 此目的為得出一個以source到各shelf的距離大小的升冪list，讓原邏輯可以直接接續使用。
        //  doSortShelfDataByDistanceFromHostSource()
        //   判斷目前此筆命令的起點得到最近的port(若為loop 則需判斷順向問題)
        //   => 使用 address 或 X Y 座標去得到 shelfData 這個 list 中所有的 shelf 到 HostSource 的路徑長度對原本的 shelfData 進行排序去得到新的shelfData list
        public List<ShelfDef> doSortShelfDataByDistanceFromHostSource(List<ShelfDef> originShelfData, string hostSource)
        {
            try
            {
                // ************************************************************
                // A20.05.15
                // 目的地選擇之想法：
                //   判斷目前此筆命令的起點得到最近的port(若為loop 則需判斷順向問題)
                //   => 使用 address 或 X Y 座標去得到 shelfData 這個 list 中所有的 shelf 到 HostSource 的路徑長度，
                //      再對原本的 shelfData 進行排序去得到新的shelfData list ，
                //      最後接著執行原本的後續判斷。
                // ************************************************************

                //************
                //A20.05.27
                string shelfSort = "";
                shelfSort = scApp.CMDBLL.CombineShelfLogData(originShelfData);
                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> DB|可用儲位排序前 前 5 筆: " + shelfSort);

                string hostSourceAdr = "";
                scApp.MapBLL.getAddressID(hostSource, out hostSourceAdr);
                #region Deep clone the origin list to the new list
                //A20.05.15     O(N)
                //  1. 先 deep clone 出一份新的list   
                //  p.s  partial 的那一份 shelfDef.cs 中已有 Clone 但不確定其使用結果，目前是土法煉鋼使用它
                List<ShelfDef> sortedShelfData = new List<ShelfDef>();
                foreach (var elementt in originShelfData)
                {
                    sortedShelfData.Add((ShelfDef)elementt.Clone());
                }
                #endregion
                #region Calculate the distance from hostSource to each shelf by address num, and store the distance in remark for sort.
                //A20.05.15      O(N)
                // 2. 再來將各shelf到hostSource的距離計算完後，先填入目前未使用的distanceFromHostSource欄位中，以讓接下來的sort可以運行。
                // 目前不須額外讀取資料庫的想法是使用目前的shelf ID的中3位進行與host取絕對值(因為第一位是方向，2 3 4 位是shelf 順序編號，5 6 位是高度但目前並未使用)
                foreach (var elementt in sortedShelfData)
                {
                    int roughltyDistance = CalculateRoughlyDistance(elementt.ADR_ID, hostSourceAdr);
                    elementt.distanceFromHostSource = roughltyDistance;
                }
                #endregion
                #region Sort this new shelfData
                //A20.05.15      O(N^2) 根據MSDN 的 list sort
                // 3. 再來呼叫list的sort 去處理此list
                // 此處之處理方式指定在partial ShelfDef.cs 中的 IComparer
                sortedShelfData.Sort(new ShelfDefCompareByAddressDistance());
                #endregion


                shelfSort = scApp.CMDBLL.CombineShelfLogData(sortedShelfData);
                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> DB|可用儲位排序後 前 5 筆: " + shelfSort);
                return sortedShelfData;
            }
            catch (Exception ex)
            {
                //************
                //A20.05.27
                TransferServiceLogger.Error(ex, "doSortShelfDataByDistanceFromHostSource");
                logger.Error(ex, "Exception");
                return originShelfData;
            }

        }

        //*************************************************
        //A20.05.15     此目的為算出各點與source的距離。
        //  CalculateRoughlyDistance()
        //   使用shelfAdr估計與hostSource 之大概距離。
        //   若需精準距離，目前想法為在上一層以讀取ASECTION的資料並傳入X Y座標進行計算。
        //   但目前的做法在loop的貨源來源為line2時 會有問題 因為那一段的address並不是順序的動作。
        //   這邊不知道是否有現成的function可以使用?
        // 20200526 Hsinyu Chang: 有現成API scApp.GuildBLL.GetDistance()
        public int CalculateRoughlyDistance(string shelfAdr, string hostSourceAdr)
        {
            int roughlyDistance = int.MaxValue;
            var check_result = scApp.GuideBLL.IsRoadWalkable(hostSourceAdr, shelfAdr); //A20.05.27.0 修改先後順序，起始與終止位置在原本狀況下放相反，導致在有方向性的Loop中會反而找到最遠的shelf。
            roughlyDistance = check_result.distance;
            return roughlyDistance;
        }

        //*************************************************
        //A20.05.21     此目的為得出一個以no command 車到各Cmd的距離大小的升冪list，讓原邏輯可以直接接續使用
        //  doSortMCSCmdDataByDistanceFromHostSourceToVehicle()
        //   先確認哪台車為no command 後，以該車到基準點的值及各命令的source到基準點的值加上priority 去計算出每一命令的distance的值，
        //   再以此值對 MCS cmd 的 list 進行sort。

        string oldBeforeSortingLog = "";
        string oldAfterSortingLog = "";
        public List<ACMD_MCS> doSortMCSCmdDataByDistanceFromHostSourceToVehicle(List<ACMD_MCS> originMCSCmdData, List<AVEHICLE> vehicleData)
        {
            try
            {
                //************
                //A20.05.27
                string cmdMCSSort = "";
                cmdMCSSort = scApp.CMDBLL.CombineMCSLogData(originMCSCmdData);
                bool isCmdPriorityMoreThan99 = false;
                if (cmdMCSSort != oldBeforeSortingLog)
                {
                    TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> DB|MCS排序前 前 5 筆: " + cmdMCSSort);
                    oldBeforeSortingLog = cmdMCSSort;
                }

                #region Deep clone the origin list to the new list
                //List<ACMD_MCS> sortedMCSData = new List<ACMD_MCS>();
                //2020/05/28 Hsinyu Chang: 這行就可以deep clone list
                List<ACMD_MCS> sortedMCSData = new List<ACMD_MCS>(originMCSCmdData);
                ////A20.05.21    
                ////先clone 出一個新的list
                //foreach (var elementt in originMCSCmdData)
                //{
                //    sortedMCSData.Add((ACMD_MCS)elementt.Clone()); // 此處是否會有問題？
                //}
                #endregion

                #region Get the no command vehicle
                //A20.05.21
                //找出沒有命令的車輛
                //2020/06/02 Hsinyu Chang: 這邊不需要了
                //AVEHICLE vehicle_nocmd = new AVEHICLE();
                //foreach (var vehicle in vehicleData)
                //{
                //    if (string.IsNullOrWhiteSpace(vehicle.OHTC_CMD))
                //    {
                //        //2020/5/27 Hsinyu Chang: AVEHICLE為不可序列化物件，且後面不會對vehicle_nocmd做任何修改，不需要deep clone它
                //        //vehicle_nocmd = vehicle.DeepClone();
                //        vehicle_nocmd = vehicle;
                //        break;
                //    }
                //}
                #endregion

                #region Sort the new list of MCS cmd by the distance of vehicle to the host source of the mcs cmd.
                //A20.05.21    
                //用車子到命令起始點距離的長度來排序目前的MCS cmd list
                //foreach (var cmdMCS in sortedMCSData)
                //{
                //    cmdMCS.DistanceFromVehicleToHostSource = calculateDistanceFromVehicleToMCSHostSource(vehicle_nocmd, cmdMCS.HOSTSOURCE);
                //}
                //2020/06/02 Hsinyu Chang: 改成計算命令起點和最近idle vehicle的距離
                foreach (var cmdMCS in sortedMCSData)
                {
                    string sourceAddr;
                    if (!scApp.MapBLL.getAddressID(cmdMCS.HOSTSOURCE, out sourceAddr))
                    {
                        cmdMCS.DistanceFromVehicleToHostSource = int.MaxValue;
                    }
                    else
                    {
                        (_, double distance) = scApp.VehicleBLL.findBestSuitableVhStepByNearest(sourceAddr);
                        cmdMCS.DistanceFromVehicleToHostSource = (int)distance;
                    }
                    if (cmdMCS.PRIORITY_SUM >= 99)
                    {
                        isCmdPriorityMoreThan99 = true;
                    }
                }
                bool isAGVCmdNumMoreThan1 = false;
                if (isCmdPriorityMoreThan99 != true)
                {
                    isAGVCmdNumMoreThan1 = IsAGVCmdNumMoreThanOne(originMCSCmdData);
                    if (isAGVCmdNumMoreThan1 == true)
                    {
                        sortedMCSData.Sort(MCSCmdCompare_MoreThan1);
                    }
                    else
                    {
                        sortedMCSData.Sort(MCSCmdCompare_LessThan2);
                    }
                }
                else
                {
                    TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> DB|MCS排序因有命令之Priority >= 99 ");
                }
                #endregion

                cmdMCSSort = scApp.CMDBLL.CombineMCSLogData(sortedMCSData);

                if (cmdMCSSort != oldAfterSortingLog)
                {
                    TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> DB|MCS排序後 前 5 筆: " + cmdMCSSort);
                    oldAfterSortingLog = cmdMCSSort;
                }

                return sortedMCSData;
            }
            catch (Exception ex)
            {
                //************
                //A20.05.27
                TransferServiceLogger.Error(ex, "doSortMCSCmdDataByDistanceFromHostSourceToVehicle");
                logger.Error(ex, "Exception");
                return originMCSCmdData;
            }
        }
        public int MCSCmdCompare_MoreThan1(ACMD_MCS MCSCmd1, ACMD_MCS MCSCmd2)
        {
            //A20.06.09.0
            // 0.判斷命令來源是否為shelf，非shelf者優先進行。
            bool isCmd1_SourceTypeShelf = MCSCmd1.IsCmdSourceTypeShelf(MCSCmd1.HOSTSOURCE);
            bool isCmd2_SourceTypeShelf = MCSCmd1.IsCmdSourceTypeShelf(MCSCmd2.HOSTSOURCE);

            if ((isCmd1_SourceTypeShelf == true) && (isCmd2_SourceTypeShelf == true) ||
                (isCmd1_SourceTypeShelf == false) && (isCmd2_SourceTypeShelf == false))
            {
                //代表兩者相等，不動，且接著判斷距離
            }
            if ((isCmd1_SourceTypeShelf == true) && (isCmd2_SourceTypeShelf == false))
            {
                return 1;
                //代表後者較優先，換位
            }
            if ((isCmd1_SourceTypeShelf == false) && (isCmd2_SourceTypeShelf == true))
            {
                return -1;
                //代表前者較優先，不動
            }

            //A20.06.04
            // 1.先取priority 判斷
            if ((MCSCmd1.PRIORITY_SUM >= 99 && MCSCmd2.PRIORITY_SUM >= 99) ||
                (MCSCmd1.PRIORITY_SUM < 99 && MCSCmd2.PRIORITY_SUM < 99))
            {
                //代表兩者相等，不動，且接著判斷距離
            }
            if (MCSCmd1.PRIORITY_SUM < 99 && MCSCmd2.PRIORITY_SUM >= 99)
            {
                return 1;
                //代表後者較優先，換位
            }
            if (MCSCmd1.PRIORITY_SUM >= 99 && MCSCmd2.PRIORITY_SUM < 99)
            {
                return -1;
                //代表前者較優先，不動
            }

            // 2. 若priority 相同，則獲得各自 shelf 的 address 與起始 address的距離
            if (MCSCmd1.DistanceFromVehicleToHostSource == MCSCmd2.DistanceFromVehicleToHostSource)
            {
                return 0;
                //代表兩者相等，不動
            }
            if (MCSCmd1.DistanceFromVehicleToHostSource > MCSCmd2.DistanceFromVehicleToHostSource)
            {
                return 1;
                //代表後者較優先，換位
            }
            if (MCSCmd1.DistanceFromVehicleToHostSource < MCSCmd2.DistanceFromVehicleToHostSource)
            {
                return -1;
                //代表前者較優先，不動
            }
            return 0;
        }
        public int MCSCmdCompare_LessThan2(ACMD_MCS MCSCmd1, ACMD_MCS MCSCmd2)
        {
            //A20.08.04
            // -1. 判斷目的 port 為AGV者優先
            bool isCmd1_SourceTypeAGV = MCSCmd1.IsCmdSourceTypeAGV(MCSCmd1.HOSTDESTINATION);
            bool isCmd2_SourceTypeAGV = MCSCmd1.IsCmdSourceTypeAGV(MCSCmd2.HOSTDESTINATION);

            if ((isCmd1_SourceTypeAGV == true) && (isCmd2_SourceTypeAGV == true) ||
                (isCmd1_SourceTypeAGV == false) && (isCmd2_SourceTypeAGV == false))
            {
                //代表兩者相等，不動，且接著判斷距離
            }
            if ((isCmd1_SourceTypeAGV == false) && (isCmd2_SourceTypeAGV == true))
            {
                return 1;
                //代表後者較優先，換位
            }
            if ((isCmd1_SourceTypeAGV == true) && (isCmd2_SourceTypeAGV == false))
            {
                return -1;
                //代表前者較優先，不動
            }

            //A20.06.09.0
            // 0.判斷命令來源是否為shelf，非shelf者優先進行。
            bool isCmd1_SourceTypeShelf = MCSCmd1.IsCmdSourceTypeShelf(MCSCmd1.HOSTSOURCE);
            bool isCmd2_SourceTypeShelf = MCSCmd1.IsCmdSourceTypeShelf(MCSCmd2.HOSTSOURCE);

            if ((isCmd1_SourceTypeShelf == true) && (isCmd2_SourceTypeShelf == true) ||
                (isCmd1_SourceTypeShelf == false) && (isCmd2_SourceTypeShelf == false))
            {
                //代表兩者相等，不動，且接著判斷距離
            }
            if ((isCmd1_SourceTypeShelf == true) && (isCmd2_SourceTypeShelf == false))
            {
                return 1;
                //代表後者較優先，換位
            }
            if ((isCmd1_SourceTypeShelf == false) && (isCmd2_SourceTypeShelf == true))
            {
                return -1;
                //代表前者較優先，不動
            }

            //A20.06.04
            // 1.先取priority 判斷
            if ((MCSCmd1.PRIORITY_SUM >= 99 && MCSCmd2.PRIORITY_SUM >= 99) ||
                (MCSCmd1.PRIORITY_SUM < 99 && MCSCmd2.PRIORITY_SUM < 99))
            {
                //代表兩者相等，不動，且接著判斷距離
            }
            if (MCSCmd1.PRIORITY_SUM < 99 && MCSCmd2.PRIORITY_SUM >= 99)
            {
                return 1;
                //代表後者較優先，換位
            }
            if (MCSCmd1.PRIORITY_SUM >= 99 && MCSCmd2.PRIORITY_SUM < 99)
            {
                return -1;
                //代表前者較優先，不動
            }

            // 2. 若priority 相同，則獲得各自 shelf 的 address 與起始 address的距離
            if (MCSCmd1.DistanceFromVehicleToHostSource == MCSCmd2.DistanceFromVehicleToHostSource)
            {
                return 0;
                //代表兩者相等，不動
            }
            if (MCSCmd1.DistanceFromVehicleToHostSource > MCSCmd2.DistanceFromVehicleToHostSource)
            {
                return 1;
                //代表後者較優先，換位
            }
            if (MCSCmd1.DistanceFromVehicleToHostSource < MCSCmd2.DistanceFromVehicleToHostSource)
            {
                return -1;
                //代表前者較優先，不動
            }
            return 0;
        }
        private static bool IsAGVCmdNumMoreThanOne(List<ACMD_MCS> originMCSCmdData)
        {
            bool _checkAGVCmdNumMoreThan1 = false;
            if (originMCSCmdData.Where(data => data.HOSTDESTINATION.Contains("ST01")).Count() > 1)
            {
                _checkAGVCmdNumMoreThan1 = true;
            }
            else if (originMCSCmdData.Where(data => data.HOSTDESTINATION.Contains("ST02")).Count() > 1)
            {
                _checkAGVCmdNumMoreThan1 = true;
            }
            else if (originMCSCmdData.Where(data => data.HOSTDESTINATION.Contains("ST03")).Count() > 1)
            {
                _checkAGVCmdNumMoreThan1 = true;
            }

            return _checkAGVCmdNumMoreThan1;
        }

        //*************************************************
        //A20.05.21    
        //計算車子到命令起始點距離
        public int calculateDistanceFromVehicleToMCSHostSource(AVEHICLE vehicle_DB, string source)
        {
            //2020/06/02 Hsinyu Chang: arguments "source" means port/shelf ID, convert to address
            string sourceAddr;
            int resultDistance = int.MaxValue;
            if (!scApp.MapBLL.getAddressID(source, out sourceAddr))
            {
                return resultDistance;
            }

            AVEHICLE vehicleCache = scApp.VehicleBLL.cache.getVhByID(vehicle_DB.VEHICLE_ID);

            if (SCUtility.isMatche(vehicleCache.CUR_ADR_ID, sourceAddr))
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(CMDBLL), Device: "OHxC",
                   Data: $"From source adr:{source}, find vh:{vehicleCache.VEHICLE_ID} current adr:{vehicleCache.CUR_ADR_ID} of distance:{0}");
                resultDistance = 0;
                return resultDistance;
            }
            var check_result = scApp.GuideBLL.IsRoadWalkable(vehicleCache.CUR_ADR_ID, sourceAddr);
            if (check_result.isSuccess)
            {
                if (check_result.distance < resultDistance)
                {
                    resultDistance = check_result.distance;
                }
            }
            return resultDistance;
        }

        //public bool creatCommand_OHTC(ACMD_OHTC cmd_mcs)
        //{
        //    bool isSuccess = true;
        //    //using (DBConnection_EF con = new DBConnection_EF())
        //    using (DBConnection_EF con = DBConnection_EF.GetUContext())
        //    {
        //        //cmd_mcsDao.add(con, cmd_mcs);
        //        cmd_ohtcDAO.add(con, cmd_mcs);
        //        con.Entry(cmd_mcs).State = EntityState.Detached;
        //    }
        //    return isSuccess;
        //}

        public void DeleteCmd(string cmdid)
        {
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                //var cmdData = LoadCmdData();
                ACMD_MCS cmd = con.ACMD_MCS.Where(cmdd => cmdd.CMD_ID.Trim() == cmdid.Trim()).First();
                cmd_mcsDao.DeleteCmdData(con, cmd);
            }
        }
        public void DeleteLog(int deleteMonths)
        {
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                cmd_mcsDao.DeleteLOG_ByACMD_MCS(con, deleteMonths);
                cmd_mcsDao.DeleteLOG_ByACMD_OHTC(con, deleteMonths);
                //cmd_mcsDao.DeleteLOG_ByACMD_AMCSREPORTQUEUE(con, deleteMonths);
            }
        }
        public bool updateCMD_MCS_CmdStatus(string cmd_id, int status)
        {
            bool isSuccess = true;

            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    int i = status;
                    ACMD_MCS cmd = cmd_mcsDao.getByID(con, cmd_id);
                    cmd.COMMANDSTATE = i;
                    cmd_mcsDao.update(con, cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }

            return isSuccess;
        }
        public bool updateCMD_MCS_TranStatus(string cmd_id, E_TRAN_STATUS status)
        {
            bool isSuccess = true;

            try
            {
                ACMD_MCS cmd = null;

                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    cmd = cmd_mcsDao.getByID(con, cmd_id);
                    cmd.TRANSFERSTATE = status;
                    cmd.CMD_FINISH_TIME = DateTime.Now;

                    if (status == E_TRAN_STATUS.Queue)
                    {
                        cmd.COMMANDSTATE = 0;
                    }

                    cmd_mcsDao.update(con, cmd);
                }

                if (status == E_TRAN_STATUS.TransferCompleted)
                {
                    if (scApp.TransferService.isUnitType(cmd.HOSTSOURCE, Service.UnitType.SHELF))
                    {
                        if (scApp.CassetteDataBLL.loadCassetteDataByLoc(cmd.HOSTSOURCE) != null)
                        {
                            scApp.ShelfDefBLL.updateStatus(cmd.HOSTSOURCE, ShelfDef.E_ShelfState.Stored);
                        }
                        else
                        {
                            ACMD_MCS destCmd = GetCmdDataByDest(cmd.HOSTSOURCE).Where(cmdData => cmdData.CMD_ID.Trim() != cmd.CMD_ID.Trim()).FirstOrDefault();

                            if (destCmd == null)
                            {
                                scApp.ShelfDefBLL.updateStatus(cmd.HOSTSOURCE, ShelfDef.E_ShelfState.EmptyShelf);
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(cmd.RelayStation) == false)
                    {
                        if (scApp.TransferService.isUnitType(cmd.RelayStation, Service.UnitType.SHELF))
                        {
                            if (scApp.CassetteDataBLL.loadCassetteDataByLoc(cmd.RelayStation) != null)
                            {
                                scApp.ShelfDefBLL.updateStatus(cmd.RelayStation, ShelfDef.E_ShelfState.Stored);
                            }
                            else
                            {
                                ACMD_MCS destCmd = GetCmdDataByDest(cmd.RelayStation).Where(cmdData => cmdData.CMD_ID.Trim() != cmd.CMD_ID.Trim()).FirstOrDefault();

                                if (destCmd == null)
                                {
                                    scApp.ShelfDefBLL.updateStatus(cmd.RelayStation, ShelfDef.E_ShelfState.EmptyShelf);
                                }
                            }
                        }
                    }

                    if (scApp.TransferService.isUnitType(cmd.HOSTDESTINATION, Service.UnitType.SHELF))
                    {
                        if (scApp.CassetteDataBLL.loadCassetteDataByLoc(cmd.HOSTDESTINATION) != null)
                        {
                            scApp.ShelfDefBLL.updateStatus(cmd.HOSTDESTINATION, ShelfDef.E_ShelfState.Stored);
                        }
                        else
                        {
                            scApp.ShelfDefBLL.updateStatus(cmd.HOSTDESTINATION, ShelfDef.E_ShelfState.EmptyShelf);
                        }
                    }

                    if (scApp.TransferService.isCVPort(cmd.HOSTDESTINATION))
                    {
                        scApp.TransferService.PortCommanding(cmd.HOSTDESTINATION, false);
                    }

                    scApp.TransferService.OHBC_OHT_QueueCmdTimeOutCmdIDCleared(cmd.CMD_ID);
                }

                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "OHB >> DB|updateCMD_MCS_TranStatus cmd_id: " + cmd_id + " status:" + status
                );
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }

            return isSuccess;
        }
        public bool updateCMD_MCS_CRANE(string cmd_id, string craneName)
        {
            bool isSuccess = true;

            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_MCS cmd = cmd_mcsDao.getByID(con, cmd_id);
                    cmd.CRANE = craneName;
                    cmd_mcsDao.update(con, cmd);
                }

                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "OHB >> DB|updateCMD_MCS_CRANE cmd_id: " + cmd_id + " craneName:" + craneName
                );
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }

            return isSuccess;
        }
        public bool updateCMD_MCS_RelayStation(string cmd_id, string relayStation)
        {
            bool isSuccess = true;

            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_MCS cmd = cmd_mcsDao.getByID(con, cmd_id);
                    cmd.RelayStation = relayStation;
                    cmd_mcsDao.update(con, cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }

            return isSuccess;
        }
        public bool updateCMD_MCS_Source(string cmd_id, string Source)
        {
            bool isSuccess = true;

            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_MCS cmd = cmd_mcsDao.getByID(con, cmd_id);
                    cmd.HOSTSOURCE = Source;
                    cmd_mcsDao.update(con, cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }

            return isSuccess;
        }
        public bool updateCMD_MCS_Dest(string cmd_id, string Dest)
        {
            bool isSuccess = true;

            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_MCS cmd = cmd_mcsDao.getByID(con, cmd_id);
                    cmd.HOSTDESTINATION = Dest;
                    cmd_mcsDao.update(con, cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }

            return isSuccess;
        }
        public bool updateCMD_MCS_TimePriority(string cmd_id, int priority)
        {
            bool isSuccess = true;

            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_MCS cmd = cmd_mcsDao.getByID(con, cmd_id);
                    cmd.TIME_PRIORITY = priority;
                    cmd.PRIORITY_SUM = cmd.PRIORITY + cmd.TIME_PRIORITY + cmd.PORT_PRIORITY;
                    cmd_mcsDao.update(con, cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }

            return isSuccess;
        }
        public bool updateCMD_MCS_PortPriority(string cmd_id, int priority)
        {
            bool isSuccess = true;

            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_MCS cmd = cmd_mcsDao.getByID(con, cmd_id);
                    cmd.PORT_PRIORITY = priority;
                    cmd.PRIORITY_SUM = cmd.PRIORITY + cmd.TIME_PRIORITY + cmd.PORT_PRIORITY;
                    cmd_mcsDao.update(con, cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }

            return isSuccess;
        }
        public bool updateCMD_MCS_sumPriority(string cmd_id)
        {
            bool isSuccess = true;

            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_MCS cmd = cmd_mcsDao.getByID(con, cmd_id);
                    cmd.PRIORITY_SUM = cmd.PRIORITY + cmd.TIME_PRIORITY + cmd.PORT_PRIORITY;
                    cmd_mcsDao.update(con, cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }

            return isSuccess;
        }
        public bool updateCMD_MCS_TranStatus2Initial(string cmd_id)
        {
            bool isSuccess = true;
            //using (DBConnection_EF con = new DBConnection_EF())
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_MCS cmd = cmd_mcsDao.getByID(con, cmd_id);
                    //cmd.TRANSFERSTATE = E_TRAN_STATUS.Initial;
                    cmd.COMMANDSTATE = cmd.COMMANDSTATE | ACMD_MCS.COMMAND_STATUS_BIT_INDEX_ENROUTE;
                    cmd.CMD_START_TIME = DateTime.Now;
                    cmd_mcsDao.update(con, cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
                isSuccess = false;
            }
            return isSuccess;
        }
        public bool updateCMD_MCS_TranStatus2PreInitial(string cmd_id)
        {
            bool isSuccess = true;
            //using (DBConnection_EF con = new DBConnection_EF())
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_MCS cmd = cmd_mcsDao.getByID(con, cmd_id);
                    //cmd.TRANSFERSTATE = E_TRAN_STATUS.PreInitial;
                    cmd_mcsDao.update(con, cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
                isSuccess = false;
            }
            return isSuccess;
        }
        public bool updateCMD_MCS_TranStatus2Paused(string cmd_id)
        {
            bool isSuccess = true;
            //using (DBConnection_EF con = new DBConnection_EF())
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_MCS cmd = cmd_mcsDao.getByID(con, cmd_id);
                    cmd.TRANSFERSTATE = E_TRAN_STATUS.Paused;
                    cmd_mcsDao.update(con, cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
                isSuccess = false;
            }
            return isSuccess;
        }

        public bool updateCMD_MCS_TranStatus2Queue(string cmd_id)
        {
            bool isSuccess = true;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_MCS cmd = cmd_mcsDao.getByID(con, cmd_id);
                    cmd.TRANSFERSTATE = E_TRAN_STATUS.Queue;
                    cmd.COMMANDSTATE = 0;
                    cmd_mcsDao.update(con, cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }


            return isSuccess;
        }
        //public bool updateCMD_MCS_TranStatus2Complete(string cmd_id, E_TRAN_STATUS tran_status)
        //{
        //    bool isSuccess = true;

        //    try
        //    {
        //        using (DBConnection_EF con = DBConnection_EF.GetUContext())
        //        {
        //            ACMD_MCS cmd = cmd_mcsDao.getByID(con, cmd_id);
        //            if (cmd != null)
        //            {
        //                cmd.TRANSFERSTATE = tran_status;
        //                cmd.COMMANDSTATE = cmd.COMMANDSTATE | ACMD_MCS.COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH;
        //                cmd.CMD_FINISH_TIME = DateTime.Now;
        //                cmd_mcsDao.update(con, cmd);
        //            }
        //            else
        //            {
        //                //isSuccess = false;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex, "Exception");
        //        isSuccess = false;
        //    }

        //    return isSuccess;
        //}

        public bool updateCMD_MCS_PrioritySUM(ACMD_MCS mcs_cmd, int priority_sum)
        {
            bool isSuccess = true;
            //using (DBConnection_EF con = new DBConnection_EF())
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    con.ACMD_MCS.Attach(mcs_cmd);
                    mcs_cmd.PRIORITY_SUM = priority_sum;
                    cmd_mcsDao.update(con, mcs_cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
                isSuccess = false;
            }
            return isSuccess;
        }
        public bool updateCMD_MCS_Priority(ACMD_MCS mcs_cmd, int priority)
        {
            bool isSuccess = true;
            //using (DBConnection_EF con = new DBConnection_EF())
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    con.ACMD_MCS.Attach(mcs_cmd);
                    mcs_cmd.PRIORITY = priority;
                    cmd_mcsDao.update(con, mcs_cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
                isSuccess = false;
            }
            return isSuccess;
        }
        public bool updateCMD_MCS_TimePriority(ACMD_MCS mcs_cmd, int time_priority)
        {
            bool isSuccess = true;
            //using (DBConnection_EF con = new DBConnection_EF())
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    con.ACMD_MCS.Attach(mcs_cmd);
                    mcs_cmd.TIME_PRIORITY = time_priority;
                    cmd_mcsDao.update(con, mcs_cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
                isSuccess = false;
            }
            return isSuccess;
        }
        public bool updateCMD_MCS_CmdStatus2LoadArrivals(string cmdID)
        {
            bool isSuccess = true;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_MCS cmd = cmd_mcsDao.getByID(con, cmdID);
                    cmd.COMMANDSTATE = cmd.COMMANDSTATE | ACMD_MCS.COMMAND_STATUS_BIT_INDEX_LOAD_ARRIVE;
                    cmd_mcsDao.update(con, cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
                isSuccess = false;
            }
            return isSuccess;
        }
        public bool updateCMD_MCS_CmdStatus2Loading(string cmdID)
        {
            bool isSuccess = true;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_MCS cmd = cmd_mcsDao.getByID(con, cmdID);
                    cmd.COMMANDSTATE = cmd.COMMANDSTATE | ACMD_MCS.COMMAND_STATUS_BIT_INDEX_LOADING;
                    cmd_mcsDao.update(con, cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
                isSuccess = false;
            }
            return isSuccess;
        }
        public bool updateCMD_MCS_CmdStatus2LoadComplete(string cmdID)
        {
            bool isSuccess = true;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_MCS cmd = cmd_mcsDao.getByID(con, cmdID);
                    cmd.COMMANDSTATE = cmd.COMMANDSTATE | ACMD_MCS.COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE;
                    cmd_mcsDao.update(con, cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
                isSuccess = false;
            }
            return isSuccess;
        }
        public bool updateCMD_MCS_CmdStatus2UnloadArrive(string cmdID)
        {
            bool isSuccess = true;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_MCS cmd = cmd_mcsDao.getByID(con, cmdID);
                    cmd.COMMANDSTATE = cmd.COMMANDSTATE | ACMD_MCS.COMMAND_STATUS_BIT_INDEX_UNLOAD_ARRIVE;
                    cmd_mcsDao.update(con, cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
                isSuccess = false;
            }
            return isSuccess;
        }
        public bool updateCMD_MCS_CmdStatus2Unloading(string cmdID)
        {
            bool isSuccess = true;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_MCS cmd = cmd_mcsDao.getByID(con, cmdID);
                    cmd.COMMANDSTATE = cmd.COMMANDSTATE | ACMD_MCS.COMMAND_STATUS_BIT_INDEX_UNLOADING;
                    cmd_mcsDao.update(con, cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
                isSuccess = false;
            }
            return isSuccess;
        }
        public bool updateCMD_MCS_CmdStatus2UnloadComplete(string cmdID)
        {
            bool isSuccess = true;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_MCS cmd = cmd_mcsDao.getByID(con, cmdID);
                    cmd.COMMANDSTATE = cmd.COMMANDSTATE | ACMD_MCS.COMMAND_STATUS_BIT_INDEX_UNLOAD_COMPLETE;
                    cmd_mcsDao.update(con, cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
                isSuccess = false;
            }
            return isSuccess;
        }
        public bool updateCMD_MCS_BCROnCrane(string cmdID, string barcodeOnCrane)
        {
            bool isSuccess = true;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_MCS cmd = cmd_mcsDao.getByID(con, cmdID);
                    cmd.CARRIER_ID_ON_CRANE = barcodeOnCrane;
                    cmd_mcsDao.update(con, cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
                isSuccess = false;
            }
            return isSuccess;
        }
        public ACMD_MCS getCMD_MCSByExcuteCmd()
        {
            ACMD_MCS cmd_mcs = null;

            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    cmd_mcs = cmd_mcsDao.getByExcuteCmd(con);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

            return cmd_mcs;
        }
        public ACMD_MCS getCMD_MCSByID(string cmd_id)
        {
            ACMD_MCS cmd_mcs = null;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    cmd_mcs = cmd_mcsDao.getByID(con, cmd_id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

            return cmd_mcs;
        }
        public ACMD_MCS getNowCMD_MCSByID(string cmd_id)
        {
            ACMD_MCS cmd_mcs = null;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    cmd_mcs = cmd_mcsDao.getNowCMD_MCSByID(con, cmd_id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

            return cmd_mcs;
        }
        public ACMD_MCS getCMD_ByBoxID(string box_id)
        {
            ACMD_MCS cmd_mcs = null;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    cmd_mcs = cmd_mcsDao.getByBoxID(con, box_id.Trim());
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

            return cmd_mcs;
        }
        public ACMD_MCS getByCstBoxID(string cst_id, string box_id)
        {
            ACMD_MCS cmd_mcs = null;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    cmd_mcs = cmd_mcsDao.getByCstBoxID(con, cst_id, box_id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

            return cmd_mcs;
        }
        public List<ACMD_MCS> getCMD_ByOHTName(string ohtName)
        {
            List<ACMD_MCS> cmd_mcs = null;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //return con.ACMD_MCS.Where(data => data.CRANE.Trim() == ohtName.Trim() && data.TRANSFERSTATE != E_TRAN_STATUS.TransferCompleted).ToList();
                    return cmd_mcsDao.getCMD_ByOHTName(con, ohtName);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

            return cmd_mcs;
        }
        public int getCMD_MCSIsQueueCount()
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cmd_mcsDao.getCMD_MCSIsQueueCount(con);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return 0;
            }


        }

        public bool IsCMD_MCSInQueue()
        {
            bool has_cmd_in_queue = false;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    has_cmd_in_queue = cmd_mcsDao.getCMD_MCSIsQueueCount(con) > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                has_cmd_in_queue = false;
            }
            return has_cmd_in_queue;
        }

        public List<ACMD_MCS> GetMCSCmdQueue()
        {
            List<ACMD_MCS> MCS_Cmd_Queue = new List<ACMD_MCS>();
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    MCS_Cmd_Queue = cmd_mcsDao.loadACMD_MCSIsQueue(con);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            return MCS_Cmd_Queue;
        }

        public int getCMD_MCSIsRunningCount()
        {

            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cmd_mcsDao.getCMD_MCSIsExcuteCount(con);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return 0;
            }


        }
        public int getCMD_MCSIsRunningCount(DateTime befor_time)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cmd_mcsDao.getCMD_MCSIsExcuteCount(con, befor_time);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return 0;
            }

        }
        public int getCMD_MCSIsUnfinishedCount(List<string> port_ids)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cmd_mcsDao.getCMD_MCSIsUnfinishedCount(con, port_ids);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return 0;
            }

        }
        public int getCMD_MCSIsUnfinishedCountByCarrierID(string carrier_id)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cmd_mcsDao.getCMD_MCSIsUnfinishedCountByCarrierID(con, carrier_id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return 0;
            }

        }
        public List<ACMD_MCS> loadACMD_MCSIsUnfinished()
        {
            //using (DBConnection_EF con = new DBConnection_EF())

            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cmd_mcsDao.loadACMD_MCSIsUnfinished(con);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }


        }
        public List<ACMD_MCS> loadMcsCmd_ByTransferring()
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cmd_mcsDao.loadCMD_ByTransferring(con);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        public List<ACMD_MCS> loadMCS_Command_Queue()
        {
            try
            {
                List<ACMD_MCS> ACMD_MCSs = list();
                ACMD_MCSs = Sort(ACMD_MCSs);
                return ACMD_MCSs;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        private List<ACMD_MCS> list()
        {
            try
            {
                List<ACMD_MCS> ACMD_MCSs = null;
                //using (DBConnection_EF con = new DBConnection_EF())
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_MCSs = cmd_mcsDao.loadACMD_MCSIsQueue(con);
                }
                return ACMD_MCSs;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        private List<ACMD_MCS> Sort(List<ACMD_MCS> list_cmd_mcs)
        {
            try
            {
                list_cmd_mcs = list_cmd_mcs.OrderByDescending(cmd => cmd.PRIORITY_SUM)
                                                       .OrderBy(cmd => cmd.CMD_INSER_TIME)
                                                       .ToList();
                return list_cmd_mcs;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }



        }
        public int getCMD_MCSInserCountLastHour(int hours)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cmd_mcsDao.getCMD_MCSInserCountLastHour(con, hours);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return 0;
            }

        }
        public int getCMD_MCSFinishCountLastHour(int hours)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cmd_mcsDao.getCMD_MCSFinishCountLastHours(con, hours);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return 0;
            }
        }

        private long syncTranCmdPoint = 0;
        public void checkMCS_TransferCommand()
        {
            try
            {
#if SIMULATOR    //ONLY FOR TEST
            if (System.Threading.Interlocked.Exchange(ref syncTranCmdPoint, 1) == 0)
            {
                try
                {
                    //找出目前Queue的命令
                    List<ACMD_MCS> ACMD_MCSs = scApp.CMDBLL.loadMCS_Command_Queue();
                    if (ACMD_MCSs != null && ACMD_MCSs.Count > 0)
                    {
                        foreach (ACMD_MCS waitting_excute_mcs_cmd in ACMD_MCSs)
                        {
                            string hostsource = waitting_excute_mcs_cmd.HOSTSOURCE;
            string hostdest = waitting_excute_mcs_cmd.HOSTDESTINATION;
                           string from_adr = waitting_excute_mcs_cmd.HOSTSOURCE;
                            string to_adr = waitting_excute_mcs_cmd.HOSTDESTINATION;
                            string assignedCrane = "OHT01";
                            //AVEHICLE bestSuitableVh = null;
                            //E_VH_TYPE vh_type = E_VH_TYPE.None;
                            E_CMD_TYPE cmd_type = E_CMD_TYPE.LoadUnload;

                            //如果命令的目的地已經有指定是在哪台車上，則直接取得該VH來決定是否要對他下命令。
                            //bool isSourceOnVehicle = scApp.VehicleBLL.getVehicleByRealID(hostsource) != null;
                            //if (isSourceOnVehicle)
                            //{
                            //    bestSuitableVh = scApp.VehicleBLL.getVehicleByRealID(hostsource);
                            //    if (bestSuitableVh.IsError || bestSuitableVh.MODE_STATUS != VHModeStatus.AutoRemote)
                            //    {
                            //        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
                            //           Data: $"vh id:{bestSuitableVh.VEHICLE_ID} current mode status is {bestSuitableVh.MODE_STATUS},is error flag:{bestSuitableVh.IsError}." +
                            //                 $"can't excute mcs command:{SCUtility.Trim(waitting_excute_mcs_cmd.CMD_ID)}",
                            //           VehicleID: bestSuitableVh.VEHICLE_ID,
                            //           CarrierID: bestSuitableVh.CST_ID);
                            //        continue;
                            //    }
                            //    cmd_type = E_CMD_TYPE.Unload;
                            //}
                            ////沒有指定的話，則需要找出離Source Port最近的一台VH來搬送
                            //else
                            //{
                            //    scApp.MapBLL.getAddressID(hostsource, out from_adr, out vh_type);
                            //    bestSuitableVh = scApp.VehicleBLL.findBestSuitableVhStepByNearest(from_adr, vh_type);
                            //    cmd_type = E_CMD_TYPE.LoadUnload;
                            //}
                            //scApp.MapBLL.getAddressID(hostdest, out to_adr);

                            //string best_suitable_vehicle_id = string.Empty;
                            ////如果有找到的話，則就產生命令派給VH
                            //if (bestSuitableVh != null)
                            //    best_suitable_vehicle_id = bestSuitableVh.VEHICLE_ID.Trim();
                            ////沒有則代表找不到車，因此就更新該筆命令的Time Priority
                            //else
                            //{
                            //    int AccumulateTime_minute = 1;
                            //    int current_time_priority = (DateTime.Now - waitting_excute_mcs_cmd.CMD_INSER_TIME).Minutes / AccumulateTime_minute;
                            //    if (current_time_priority != waitting_excute_mcs_cmd.TIME_PRIORITY)
                            //    {
                            //        int change_priority = current_time_priority - waitting_excute_mcs_cmd.TIME_PRIORITY;
                            //        updateCMD_MCS_TimePriority(waitting_excute_mcs_cmd, current_time_priority);
                            //        updateCMD_MCS_PrioritySUM(waitting_excute_mcs_cmd, waitting_excute_mcs_cmd.PRIORITY_SUM + change_priority);
                            //    }
                            //    continue;
                            //}

                            //如果有找到車子則將產生一筆Transfer command
                            using (TransactionScope tx = SCUtility.getTransactionScope())
                            {
                                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                                {

                                    bool isSuccess = true;
                                    isSuccess &= scApp.CMDBLL.doCreatTransferCommand(assignedCrane, waitting_excute_mcs_cmd.CMD_ID, waitting_excute_mcs_cmd.CARRIER_ID,
                                                        cmd_type,
                                                        from_adr,
                                                        to_adr, waitting_excute_mcs_cmd.PRIORITY_SUM, 0);
                                    //在找到車子後先把它改成PreInitial，防止Timer再找到該筆命令
                                    if (isSuccess)
                                    {
                                        isSuccess &= scApp.CMDBLL.updateCMD_MCS_TranStatus2PreInitial(waitting_excute_mcs_cmd.CMD_ID);
                                    }
                                    //如果產生完命令後，發現該Vh正在執行OHTC的移動命令時，則需要將該命令Cancel
                                    //if (isSuccess && !SCUtility.isEmpty(bestSuitableVh.OHTC_CMD))
                                    //{
                                    //    AVEHICLE VhCatchObj = scApp.getEQObjCacheManager().getVehicletByVHID(bestSuitableVh.VEHICLE_ID);
                                    //    isSuccess = bestSuitableVh.sned_Str37(bestSuitableVh.OHTC_CMD, CMDCancelType.CmdCancel);
                                    //}
                                    if (isSuccess)
                                    {
                                        tx.Complete();
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                            }
                            //checkOHxC_TransferCommand();

                            Task.Run(() =>
                            {
            //scApp.TransferService.OHT_TransferStatus(waitting_excute_mcs_cmd.CMD_ID, waitting_excute_mcs_cmd.CARRIER_ID,
                                //    scApp.EAPSecsAgentName, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_ENROUTE);
                                scApp.TransferService.OHT_TransferStatus(waitting_excute_mcs_cmd.CMD_ID, waitting_excute_mcs_cmd.CARRIER_ID,
                                    assignedCrane, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_ENROUTE);
                                Thread.Sleep(3000);
                                scApp.TransferService.OHT_TransferStatus(waitting_excute_mcs_cmd.CMD_ID, waitting_excute_mcs_cmd.CARRIER_ID,
                                    assignedCrane, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_LOAD_ARRIVE);
                                Thread.Sleep(500);
                                scApp.TransferService.OHT_TransferStatus(waitting_excute_mcs_cmd.CMD_ID, waitting_excute_mcs_cmd.CARRIER_ID,
                                    assignedCrane, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_LOADING);
                                Thread.Sleep(1500);
                                scApp.TransferService.OHT_TransferStatus(waitting_excute_mcs_cmd.CMD_ID, waitting_excute_mcs_cmd.CARRIER_ID,
                                    assignedCrane, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE);
                                Thread.Sleep(10000);
                                scApp.TransferService.OHT_TransferStatus(waitting_excute_mcs_cmd.CMD_ID, waitting_excute_mcs_cmd.CARRIER_ID,
                                    assignedCrane, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_UNLOAD_ARRIVE);
                                Thread.Sleep(500);
                                scApp.TransferService.OHT_TransferStatus(waitting_excute_mcs_cmd.CMD_ID, waitting_excute_mcs_cmd.CARRIER_ID,
                                    assignedCrane, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_UNLOADING);
                                Thread.Sleep(1500);
                                scApp.TransferService.OHT_TransferStatus(waitting_excute_mcs_cmd.CMD_ID, waitting_excute_mcs_cmd.CARRIER_ID,
                                    assignedCrane, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_UNLOAD_COMPLETE);
                                Thread.Sleep(500);
                                scApp.TransferService.OHT_TransferStatus(waitting_excute_mcs_cmd.CMD_ID, waitting_excute_mcs_cmd.CARRIER_ID,
                                    assignedCrane, ACMD_MCS.COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH);
                            }).Wait();
                        }
                    }
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref syncTranCmdPoint, 0);
                }
            }
#else
                if (System.Threading.Interlocked.Exchange(ref syncTranCmdPoint, 1) == 0)
                {
                    try
                    {
                        //如果目前的Control不是Active，則不需要檢查
                        if (scApp.getEQObjCacheManager().getLine().ServiceMode
                            != SCAppConstants.AppServiceMode.Active)
                            return;
                        //如果目前System control不為Auto狀態，則不需要檢查
                        if (scApp.getEQObjCacheManager().getLine().SCStats != ALINE.TSCState.AUTO)
                            return;
                        //如果MCSCommandAutoAssign是關閉的，則不需要檢查
                        if (!scApp.getEQObjCacheManager().getLine().MCSCommandAutoAssign)
                            return;
                        //找出目前Queue的命令
                        List<ACMD_MCS> ACMD_MCSs = scApp.CMDBLL.loadMCS_Command_Queue();
                        if (ACMD_MCSs != null && ACMD_MCSs.Count > 0)
                        {
                            foreach (ACMD_MCS waitting_excute_mcs_cmd in ACMD_MCSs)
                            {
#if true
                                string hostsource = waitting_excute_mcs_cmd.HOSTSOURCE;
                                string hostdest = waitting_excute_mcs_cmd.HOSTDESTINATION;
                                string from_adr = string.Empty;
                                string to_adr = string.Empty;
                                AVEHICLE bestSuitableVh = null;
                                E_VH_TYPE vh_type = E_VH_TYPE.None;
                                E_CMD_TYPE cmd_type = default(E_CMD_TYPE);

                                //如果命令的目的地已經有指定是在哪台車上，則直接取得該VH來決定是否要對他下命令。
                                bool isSourceOnVehicle = scApp.VehicleBLL.getVehicleByRealID(hostsource) != null;
                                if (isSourceOnVehicle)
                                {
                                    bestSuitableVh = scApp.VehicleBLL.getVehicleByRealID(hostsource);
                                    if (bestSuitableVh.IsError || bestSuitableVh.MODE_STATUS != VHModeStatus.AutoRemote)
                                    {
                                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
                                           Data: $"vh id:{bestSuitableVh.VEHICLE_ID} current mode status is {bestSuitableVh.MODE_STATUS},is error flag:{bestSuitableVh.IsError}." +
                                                 $"can't excute mcs command:{SCUtility.Trim(waitting_excute_mcs_cmd.CMD_ID)}",
                                           VehicleID: bestSuitableVh.VEHICLE_ID,
                                           CarrierID: bestSuitableVh.CST_ID);
                                        continue;
                                    }
                                    cmd_type = E_CMD_TYPE.Unload;
                                }
                                //沒有指定的話，則需要找出離Source Port最近的一台VH來搬送
                                else
                                {
                                    scApp.MapBLL.getAddressID(hostsource, out from_adr, out vh_type);
                                    bestSuitableVh = scApp.VehicleBLL.findBestSuitableVhStepByNearest(from_adr, vh_type);
                                    cmd_type = E_CMD_TYPE.LoadUnload;
                                }
                                scApp.MapBLL.getAddressID(hostdest, out to_adr);

                                string best_suitable_vehicle_id = string.Empty;
                                //如果有找到的話，則就產生命令派給VH
                                if (bestSuitableVh != null)
                                    best_suitable_vehicle_id = bestSuitableVh.VEHICLE_ID.Trim();
                                //沒有則代表找不到車，因此就更新該筆命令的Time Priority
                                else
                                {
                                    int AccumulateTime_minute = 1;
                                    int current_time_priority = (DateTime.Now - waitting_excute_mcs_cmd.CMD_INSER_TIME).Minutes / AccumulateTime_minute;
                                    if (current_time_priority != waitting_excute_mcs_cmd.TIME_PRIORITY)
                                    {
                                        int change_priority = current_time_priority - waitting_excute_mcs_cmd.TIME_PRIORITY;
                                        updateCMD_MCS_TimePriority(waitting_excute_mcs_cmd, current_time_priority);
                                        //updateCMD_MCS_PrioritySUM(waitting_excute_mcs_cmd, waitting_excute_mcs_cmd.PRIORITY_SUM + change_priority);
                                        updateCMD_MCS_PortPriority(waitting_excute_mcs_cmd.CMD_ID, change_priority);
                                    }
                                    continue;
                                }

                                //如果有找到車子則將產生一筆Transfer command
                                using (TransactionScope tx = SCUtility.getTransactionScope())
                                {
                                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                                    {

                                        bool isSuccess = true;
                                        isSuccess &= scApp.CMDBLL.doCreatTransferCommand(best_suitable_vehicle_id, waitting_excute_mcs_cmd.CMD_ID, waitting_excute_mcs_cmd.CARRIER_ID,
                                                            cmd_type,
                                                            hostsource,
                                                            hostdest, waitting_excute_mcs_cmd.PRIORITY_SUM, 0,
                                                            waitting_excute_mcs_cmd.BOX_ID, waitting_excute_mcs_cmd.LOT_ID,
                                                            from_adr, to_adr);
                                        //在找到車子後先把它改成PreInitial，防止Timer再找到該筆命令
                                        if (isSuccess)
                                        {
                                            //isSuccess &= scApp.CMDBLL.updateCMD_MCS_TranStatus2Paused(waitting_excute_mcs_cmd.CMD_ID);  //20200220改成Paused WARNING
                                        }
                                        //如果產生完命令後，發現該Vh正在執行OHTC的移動命令時，則需要將該命令Cancel
                                        if (isSuccess && !SCUtility.isEmpty(bestSuitableVh.OHTC_CMD))
                                        {
                                            AVEHICLE VhCatchObj = scApp.getEQObjCacheManager().getVehicletByVHID(bestSuitableVh.VEHICLE_ID);
                                            isSuccess = bestSuitableVh.sned_Str37(bestSuitableVh.OHTC_CMD, CMDCancelType.CmdCancel);
                                        }
                                        if (isSuccess)
                                        {
                                            tx.Complete();
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                }
                                checkOHxC_TransferCommand();
#else
                                bool isSuccess = generateOHTCommand(waitting_excute_mcs_cmd);
                                //目前無法執行此命令，更新該筆命令的Time Priority
                                if (!isSuccess)
                                {
                                    int AccumulateTime_minute = 1;
                                    int current_time_priority = (DateTime.Now - waitting_excute_mcs_cmd.CMD_INSER_TIME).Minutes / AccumulateTime_minute;
                                    if (current_time_priority != waitting_excute_mcs_cmd.TIME_PRIORITY)
                                    {
                                        int change_priority = current_time_priority - waitting_excute_mcs_cmd.TIME_PRIORITY;
                                        updateCMD_MCS_TimePriority(waitting_excute_mcs_cmd, current_time_priority);
                                        updateCMD_MCS_PrioritySUM(waitting_excute_mcs_cmd, waitting_excute_mcs_cmd.PRIORITY_SUM + change_priority);
                                    }
                                }
#endif
                            }
                        }
                    }
                    finally
                    {
                        System.Threading.Interlocked.Exchange(ref syncTranCmdPoint, 0);
                    }
                }
#endif
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }
        private long syncTranOHTCommandPoint = 0;
        public bool generateOHTCommand(ACMD_MCS mcs_cmd)
        {
            if (Interlocked.Exchange(ref syncTranOHTCommandPoint, 1) == 0)
            {
                try
                {
                    string hostsource = mcs_cmd.HOSTSOURCE;
                    string hostdest = mcs_cmd.HOSTDESTINATION;
                    string from_adr = string.Empty;
                    string to_adr = string.Empty;
                    AVEHICLE bestSuitableVh = null;
                    E_VH_TYPE vh_type = E_VH_TYPE.None;

                    if (mcs_cmd.CMD_ID.StartsWith("SCAN-"))
                    {
                        ShelfDef targetShelf = scApp.ShelfDefBLL.loadShelfDataByID(mcs_cmd.HOSTSOURCE);

                        scApp.MapBLL.getAddressID(hostsource, out from_adr, out vh_type);
                        bestSuitableVh = scApp.VehicleBLL.findBestSuitableVhStepByNearest(from_adr, vh_type);
                        if (bestSuitableVh == null)
                        {
                            return false;
                        }

                        ACMD_OHTC cmdohtc = new ACMD_OHTC
                        {
                            CMD_ID = scApp.SequenceBLL.getCommandID(SCAppConstants.GenOHxCCommandType.Auto),
                            CARRIER_ID = mcs_cmd.CARRIER_ID,
                            BOX_ID = mcs_cmd.BOX_ID,
                            VH_ID = bestSuitableVh.VEHICLE_ID.Trim(),
                            CMD_ID_MCS = mcs_cmd.CMD_ID,
                            CMD_TPYE = E_CMD_TYPE.Scan,
                            PRIORITY = 50,
                            SOURCE = mcs_cmd.HOSTSOURCE,
                            DESTINATION = mcs_cmd.HOSTDESTINATION,
                            CMD_STAUS = 0,
                            CMD_PROGRESS = 0,
                            ESTIMATED_EXCESS_TIME = 0,
                            REAL_CMP_TIME = 0,
                            ESTIMATED_TIME = 50,
                            SOURCE_ADR = targetShelf.ADR_ID,
                            DESTINATION_ADR = targetShelf.ADR_ID
                        };

                        creatCommand_OHTC(cmdohtc);
                        return true;
                    }
                    else
                    {
                        E_CMD_TYPE cmd_type = default(E_CMD_TYPE);

                        //如果命令的目的地已經有指定是在哪台車上，則直接取得該VH來決定是否要對他下命令。
                        bool isSourceOnVehicle = scApp.VehicleBLL.getVehicleByRealID(hostsource) != null;
                        if (isSourceOnVehicle)
                        {
                            bestSuitableVh = scApp.VehicleBLL.getVehicleByRealID(hostsource);
                            if (bestSuitableVh.IsError || bestSuitableVh.MODE_STATUS != VHModeStatus.AutoRemote)
                            {
                                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(VehicleBLL), Device: "OHxC",
                                   Data: $"vh id:{bestSuitableVh.VEHICLE_ID} current mode status is {bestSuitableVh.MODE_STATUS},is error flag:{bestSuitableVh.IsError}." +
                                         $"can't excute mcs command:{SCUtility.Trim(mcs_cmd.CMD_ID)}",
                                   VehicleID: bestSuitableVh.VEHICLE_ID,
                                   CarrierID: bestSuitableVh.CST_ID);
                                return false;
                            }
                            cmd_type = E_CMD_TYPE.Unload;
                        }
                        //沒有指定的話，則需要找出離Source Port最近的一台VH來搬送
                        else
                        {
                            scApp.MapBLL.getAddressID(hostsource, out from_adr, out vh_type);
                            bestSuitableVh = scApp.VehicleBLL.findBestSuitableVhStepByNearest(from_adr, vh_type);
                            cmd_type = E_CMD_TYPE.LoadUnload;
                        }

                        //if (isZone(hostdest))   //hsinyuchang	2020/4/30 15:32:36	搬送目的為zone時，計算搬送到哪個shelf的methos
                        //{
                        //    List<ShelfDef> shelfData = scApp.ShelfDefBLL.GetEmptyAndEnableShelfByZone(hostdest);
                        //    string shelfID = scApp.TransferService.GetShelfRecentLocation(shelfData, hostsource);
                        //    hostdest = shelfID;
                        //    //hostdest = getEmptyShelfForMoveIn(hostdest);
                        //}
                        //if (hostdest == null)
                        //{
                        //    return false;   //沒有空shelf，暫緩執行
                        //}

                        scApp.MapBLL.getAddressID(hostdest, out to_adr);

                        string best_suitable_vehicle_id = string.Empty;
                        //如果有找到的話，則就產生命令派給VH
                        if (bestSuitableVh != null)
                        {
                            best_suitable_vehicle_id = bestSuitableVh.VEHICLE_ID.Trim();
                        }
                        //沒有則代表找不到車，因此就更新該筆命令的Time Priority (此步驟上層處理)
                        else
                        {
                            return false;
                        }

                        //如果有找到車子則將產生一筆Transfer command
                        bool isSuccess = true;
                        isSuccess &= scApp.CMDBLL.doCreatTransferCommand(best_suitable_vehicle_id, mcs_cmd.CMD_ID, mcs_cmd.CARRIER_ID,
                                            cmd_type,
                                            hostsource,
                                            hostdest, mcs_cmd.PRIORITY_SUM, 0,
                                            mcs_cmd.BOX_ID, mcs_cmd.LOT_ID,
                                            from_adr, to_adr);
                        //在找到車子後先把它改成PreInitial，防止Timer再找到該筆命令
                        if (isSuccess)
                        {
                            //isSuccess &= scApp.CMDBLL.updateCMD_MCS_TranStatus2Paused(mcs_cmd.CMD_ID);  //20200220改成Paused WARNING
                            if (mcs_cmd.CRANE != best_suitable_vehicle_id)
                            {
                                updateCMD_MCS_CRANE(mcs_cmd.CMD_ID, best_suitable_vehicle_id);
                            }
                        }
                        //如果產生完命令後，發現該Vh正在執行OHTC的移動命令時，則需要將該命令Cancel
                        //20200515 不要取消了，讓他做完
                        //if (isSuccess && !SCUtility.isEmpty(bestSuitableVh.OHTC_CMD))
                        //{
                        //    AVEHICLE VhCatchObj = scApp.getEQObjCacheManager().getVehicletByVHID(bestSuitableVh.VEHICLE_ID);
                        //    isSuccess = bestSuitableVh.sned_Str37(bestSuitableVh.OHTC_CMD, CMDCancelType.CmdCancel);
                        //}

                        //if (mcs_cmd.CRANE != best_suitable_vehicle_id)
                        //{
                        //    updateCMD_MCS_CRANE(mcs_cmd.CMD_ID, best_suitable_vehicle_id);
                        //}

                        //return true;
                        return isSuccess;
                    }
                }
                catch (Exception ex)
                {
                    TransferServiceLogger.Error(ex, "generateOHTCommand");
                    return false;
                }
                finally
                {
                    Interlocked.Exchange(ref syncTranOHTCommandPoint, 0);
                }
            }
            else
            {
                return false;
            }
        }

        private string getEmptyShelfForMoveIn(string zoneID)    //hsinyuchang	2020/4/30 15:32:36	搬送目的為zone時，計算搬送到哪個shelf的methos
        {
            List<ShelfDef> shelfData = scApp.ShelfDefBLL.GetEmptyAndEnableShelfByZone(zoneID);//Modify by Kevin
            string targetShelf;

            if (shelfData == null)
            {
                return null;
            }
            else
            {
                foreach (var v in shelfData)    //第2道防護機制，防止已有命令搬往此 ShelfID ，造成二重格發生
                {
                    ACMD_MCS cmdData = GetCmdDataByDest(v.ShelfID).FirstOrDefault();
                    if (cmdData == null)
                    {
                        targetShelf = v.ShelfID;
                        return targetShelf;
                    }
                }
                return null;
            }
        }

        public bool assignCommnadToVehicleForCmdShift(string mcs_id, string vh_id, out string result)
        {
            try
            {
                ACMD_MCS ACMD_MCS = scApp.CMDBLL.getCMD_MCSByID(mcs_id);
                if (ACMD_MCS != null)
                {
                    bool check_result = true;
                    result = "OK";
                    //ACMD_MCS excute_cmd = ACMD_MCSs[0];
                    string hostsource = ACMD_MCS.HOSTSOURCE;
                    string hostdest = ACMD_MCS.HOSTDESTINATION;
                    string from_adr = string.Empty;
                    string to_adr = string.Empty;
                    AVEHICLE vh = null;
                    E_VH_TYPE vh_type = E_VH_TYPE.None;
                    E_CMD_TYPE cmd_type = default(E_CMD_TYPE);

                    //確認 source 是否為Port
                    bool source_is_a_port = scApp.PortDefBLL.GetPortData(hostsource) != null;
                    if (source_is_a_port)
                    {
                        scApp.MapBLL.getAddressID(hostsource, out from_adr, out vh_type);
                        vh = scApp.VehicleBLL.getVehicleByID(vh_id);
                        cmd_type = E_CMD_TYPE.LoadUnload;
                    }
                    else
                    {
                        result = "Source must be a port.";
                        return false;
                    }
                    scApp.MapBLL.getAddressID(hostdest, out to_adr);
                    if (vh != null)
                    {
                        string vehicleId = vh.VEHICLE_ID.Trim();
                        List<AMCSREPORTQUEUE> reportqueues = null;
                        using (TransactionScope tx = SCUtility.getTransactionScope())
                        {
                            using (DBConnection_EF con = DBConnection_EF.GetUContext())
                            {

                                bool isSuccess = true;
                                int total_priority = ACMD_MCS.PRIORITY + ACMD_MCS.TIME_PRIORITY + ACMD_MCS.PORT_PRIORITY;
                                isSuccess &= scApp.CMDBLL.doCreatTransferCommand(vehicleId, ACMD_MCS.CMD_ID, ACMD_MCS.CARRIER_ID,
                                                    cmd_type,
                                                    from_adr,
                                                    to_adr, total_priority, 0);

                                if (isSuccess && !SCUtility.isEmpty(vh.OHTC_CMD))
                                {
                                    AVEHICLE VhCatchObj = scApp.getEQObjCacheManager().getVehicletByVHID(vh.VEHICLE_ID);
                                    isSuccess = vh.sned_Str37(vh.OHTC_CMD, CMDCancelType.CmdCancel);
                                }
                                if (isSuccess)
                                {
                                    tx.Complete();
                                }
                                else
                                {
                                    result = "Assign command to vehicle failed.";
                                    return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        result = $"Can not find vehicle:{vh_id}.";
                        return false;
                    }
                    return true;
                }
                else
                {
                    result = $"Can not find command:{mcs_id}.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                result = $"Can not find command:{mcs_id}.";
                return false;
            }
            finally
            {
                System.Threading.Interlocked.Exchange(ref syncTranCmdPoint, 0);
            }
        }

        public bool assignCommnadToVehicle(string mcs_id, string vh_id, out string result)
        {
            try
            {
                ACMD_MCS ACMD_MCS = scApp.CMDBLL.getCMD_MCSByID(mcs_id);
                if (ACMD_MCS != null)
                {
                    bool check_result = true;
                    result = "OK";
                    //ACMD_MCS excute_cmd = ACMD_MCSs[0];
                    string hostsource = ACMD_MCS.HOSTSOURCE;
                    string hostdest = ACMD_MCS.HOSTDESTINATION;
                    string from_adr = string.Empty;
                    string to_adr = string.Empty;
                    AVEHICLE vh = null;
                    E_VH_TYPE vh_type = E_VH_TYPE.None;
                    E_CMD_TYPE cmd_type = default(E_CMD_TYPE);

                    //確認 source 是否為Port
                    bool source_is_a_port = scApp.PortDefBLL.GetPortData(hostsource) != null;
                    if (source_is_a_port)
                    {
                        scApp.MapBLL.getAddressID(hostsource, out from_adr, out vh_type);
                        vh = scApp.VehicleBLL.getVehicleByID(vh_id);
                        cmd_type = E_CMD_TYPE.LoadUnload;
                    }
                    else
                    {
                        result = "Source must be a port.";
                        return false;
                    }
                    scApp.MapBLL.getAddressID(hostdest, out to_adr);
                    if (vh != null)
                    {

                        string vehicleId = vh.VEHICLE_ID.Trim();



                        List<AMCSREPORTQUEUE> reportqueues = null;
                        using (TransactionScope tx = SCUtility.getTransactionScope())
                        {
                            using (DBConnection_EF con = DBConnection_EF.GetUContext())
                            {

                                bool isSuccess = true;
                                int total_priority = ACMD_MCS.PRIORITY + ACMD_MCS.TIME_PRIORITY + ACMD_MCS.PORT_PRIORITY;
                                isSuccess &= scApp.CMDBLL.doCreatTransferCommand(vehicleId, ACMD_MCS.CMD_ID, ACMD_MCS.CARRIER_ID,
                                                    cmd_type,
                                                    from_adr,
                                                    to_adr, total_priority, 0);
                                //在找到車子後先把它改成PreInitial，防止Timer再找到該筆命令
                                if (isSuccess)
                                {
                                    //isSuccess &= scApp.CMDBLL.updateCMD_MCS_TranStatus2Initial(waitting_excute_mcs_cmd.CMD_ID);
                                    //isSuccess &= scApp.ReportBLL.newReportTransferInitial(waitting_excute_mcs_cmd.CMD_ID, reportqueues);
                                    isSuccess &= scApp.CMDBLL.updateCMD_MCS_TranStatus2PreInitial(ACMD_MCS.CMD_ID);

                                }
                                if (isSuccess && !SCUtility.isEmpty(vh.OHTC_CMD))
                                {
                                    AVEHICLE VhCatchObj = scApp.getEQObjCacheManager().getVehicletByVHID(vh.VEHICLE_ID);
                                    isSuccess = vh.sned_Str37(vh.OHTC_CMD, CMDCancelType.CmdCancel);
                                }
                                if (isSuccess)
                                {
                                    tx.Complete();
                                }
                                else
                                {
                                    result = "Assign command to vehicle failed.";
                                    return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        result = $"Can not find vehicle:{vh_id}.";
                        return false;
                    }
                    return true;
                }
                else
                {
                    result = $"Can not find command:{mcs_id}.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                result = $"Can not find command:{mcs_id}.";
                return false;
            }
            finally
            {
                System.Threading.Interlocked.Exchange(ref syncTranCmdPoint, 0);
            }
        }

        public bool commandShift(string mcs_id, string vh_id, out string result)
        {
            //result = "Not implement yet.";
            //return false;
            try
            {
                //1. Cancel命令
                CMDCancelType cnacel_type = default(CMDCancelType);
                cnacel_type = CMDCancelType.CmdCancel;
                bool btemp = scApp.VehicleService.doCancelCommandByMCSCmdIDWithNoReport(mcs_id, cnacel_type, out string ohxc_cmd_id);
                if (btemp)
                {
                    //2. 等命令Cancel完成
                    int loop_time = 20;
                    for (int i = 0; i < loop_time; i++)
                    {
                        ACMD_OHTC cmd = getCMD_OHTCByID(ohxc_cmd_id);
                        if (cmd == null)
                        {
                            result = $"Can not find vehicle command:{ohxc_cmd_id}.";
                            return false;
                        }
                        else if (cmd.CMD_STAUS == E_CMD_STATUS.CancelEndByOHTC)
                        {
                            break;//表示該命令已經被Cancel了，可以進行下一步Assign命令的部分
                        }
                        else if (i == loop_time - 1)
                        {
                            //已到迴圈最後一次，命令仍未被cancel，視為timeout。
                            result = $"Cancel command timeout. Command ID:{ohxc_cmd_id}.";
                            return false;
                        }
                        Thread.Sleep(500);//等500毫秒再跑下一輪
                    }
                    //3. 分派命令給新車(不能報command initial)
                    ACMD_MCS ACMD_MCS = scApp.CMDBLL.getCMD_MCSByID(mcs_id);
                    if (ACMD_MCS != null)
                    {
                        assignCommnadToVehicleForCmdShift(mcs_id, vh_id, out result);
                        if (result == "OK")
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
                        result = $"Can not find command:{mcs_id}.";
                        return false;
                    }
                }
                else
                {
                    result = $"Transfer command:[{mcs_id}] cancel failed.";
                    return false;
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                result = $"Can not find command:{mcs_id}.";
                return false;
            }
            finally
            {
                System.Threading.Interlocked.Exchange(ref syncTranCmdPoint, 0);
            }
        }


        public List<TranTask> loadTranTasks()
        {
            try
            {
                return testTranTaskDao.loadTransferTasks_ACycle(scApp.PortStationBLL, scApp.TranCmdPeriodicDataSet.Tables[0]);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

        }

        public Dictionary<int, List<TranTask>> loadTranTaskSchedule_24Hour()
        {
            try
            {
                List<TranTask> allTranTaskType = testTranTaskDao.loadTransferTasks_24Hour(scApp.TranCmdPeriodicDataSet.Tables[1]);
                Dictionary<int, List<TranTask>> dicTranTaskSchedule = new Dictionary<int, List<TranTask>>();
                var query = from tranTask in allTranTaskType
                            group tranTask by tranTask.Min;

                dicTranTaskSchedule = query.OrderBy(item => item.Key).ToDictionary(item => item.Key, item => item.ToList());

                return dicTranTaskSchedule;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        public Dictionary<string, List<TranTask>> loadTranTaskSchedule_Clear_Dirty()
        {
            try
            {
                List<TranTask> allTranTaskType = testTranTaskDao.loadTransferTasks_24Hour(scApp.TranCmdPeriodicDataSet.Tables[1]);
                Dictionary<string, List<TranTask>> dicTranTaskSchedule = new Dictionary<string, List<TranTask>>();
                var query = from tranTask in allTranTaskType
                            group tranTask by tranTask.CarType;

                dicTranTaskSchedule = query.OrderBy(item => item.Key).ToDictionary(item => item.Key, item => item.ToList());

                return dicTranTaskSchedule;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }

        #endregion CMD_MCS

        #region CMD_OHTC
        public const string CALL_CONTEXT_KEY_WORD_OHTC_CMD_CHECK_RESULT = "OHTC_CMD_CHECK_RESULT";
        public class OHTCCommandCheckResult
        {
            public OHTCCommandCheckResult()
            {
                Num = DateTime.Now.ToString(SCAppConstants.TimestampFormat_19);
                IsSuccess = true;
            }
            public string Num { get; private set; }
            public bool IsSuccess = false;
            public StringBuilder Result = new StringBuilder();
            public override string ToString()
            {
                string message = "Alarm No.:" + Num + Environment.NewLine + Environment.NewLine + Result.ToString();
                return message;
            }
        }

        public bool doCreatTransferCommand(string vh_id, string cmd_id_mcs = "", string cst_id = "", E_CMD_TYPE cmd_type = E_CMD_TYPE.Move,
                                   string source = "", string destination = "", int priority = 0, int estimated_time = 0,
                                   string box_id = "", string lot_id = "", string source_address = "", string destination_address = "",
                                   SCAppConstants.GenOHxCCommandType gen_cmd_type = SCAppConstants.GenOHxCCommandType.Auto)
        {
            ACMD_OHTC cmd_obj = null;
            return doCreatTransferCommand(vh_id, out cmd_obj, cmd_id_mcs, cst_id, cmd_type,
                                    source, destination, priority, estimated_time, gen_cmd_type,
                                    box_id, lot_id, source_address, destination_address);
        }
        public bool doCreatTransferCommand(string vhID, out ACMD_OHTC cmd_obj, string cmd_id_mcs = "", string cst_id = "", E_CMD_TYPE cmd_type = E_CMD_TYPE.Move,
                                       string source = "", string destination = "", int priority = 0, int estimated_time = 0,
                                       SCAppConstants.GenOHxCCommandType gen_cmd_type = SCAppConstants.GenOHxCCommandType.Auto,
                                       string box_id = "", string lot_id = "", string source_address = "", string destination_address = "")
        {
            cmd_obj = null;
            try
            {
#if SIMULATOR
            OHTCCommandCheckResult check_result = getOrSetCallContext<OHTCCommandCheckResult>(CALL_CONTEXT_KEY_WORD_OHTC_CMD_CHECK_RESULT);

            check_result.IsSuccess &= creatCommand_OHTC(vh_id, cmd_id_mcs, carrier_id, cmd_type, source, destination, priority, estimated_time, gen_cmd_type, out cmd_obj);
            if (!check_result.IsSuccess)
            {
                check_result.Result.AppendLine($" vh:{vh_id} creat command to db unsuccess.");
                check_result.Result.AppendLine("");
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(CMDBLL), Device: string.Empty,
                    Data: check_result.Result.ToString(),
                    XID: check_result.Num);
            }
            setCallContext(CALL_CONTEXT_KEY_WORD_OHTC_CMD_CHECK_RESULT, check_result);

            return check_result.IsSuccess;
#else
                OHTCCommandCheckResult check_result = getOrSetCallContext<OHTCCommandCheckResult>(CALL_CONTEXT_KEY_WORD_OHTC_CMD_CHECK_RESULT);

                //不是MCS Cmd，要檢查檢查有沒有在執行中的，有則不能Creat
                string vh_id = SCUtility.Trim(vhID, true);
                AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vh_id);
                lock (vh.DoCreatTransferCommand_Sync)
                {
                    string vh_current_adr = vh.CUR_ADR_ID;
                    string vh_current_section = vh.CUR_SEC_ID;
                    if (cmd_type == E_CMD_TYPE.MTLHome ||
                       cmd_type == E_CMD_TYPE.MoveToMTL ||
                       cmd_type == E_CMD_TYPE.SystemOut ||
                       cmd_type == E_CMD_TYPE.SystemIn)
                    {
                        //not thing...
                    }
                    else
                    {
                        if (scApp.EquipmentBLL.cache.IsInMaintainDeviceRangeOfAddress(scApp.SegmentBLL, source))
                        {
                            check_result.Result.AppendLine($"vh:{vh_id} want to excute:{cmd_type} ,but source is maintain device range of address:{source}");
                            check_result.Result.AppendLine("");
                            check_result.IsSuccess &= false;
                        }
                        if (scApp.EquipmentBLL.cache.IsInMaintainDeviceRangeOfAddress(scApp.SegmentBLL, destination))
                        {
                            check_result.Result.AppendLine($"vh:{vh_id} want to excute:{cmd_type} ,but destination is maintain device range of address:{destination}");
                            check_result.Result.AppendLine("");
                            check_result.IsSuccess &= false;
                        }
                        if (scApp.EquipmentBLL.cache.IsInMaintainDeviceRangeOfAddress(scApp.SegmentBLL, vh.CUR_ADR_ID))
                        {
                            check_result.Result.AppendLine($"vh:{vh_id} want to excute:{cmd_type} ,but current vh in maintain device range of address:{vh.CUR_ADR_ID}");
                            check_result.Result.AppendLine("");
                            check_result.IsSuccess &= false;
                        }
                        if (scApp.EquipmentBLL.cache.IsInMaintainDeviceRangeOfSection(scApp.SegmentBLL, vh.CUR_SEC_ID))
                        {
                            check_result.Result.AppendLine($"vh:{vh_id} want to excute:{cmd_type} ,but current vh in maintain device range of section:{vh.CUR_SEC_ID}");
                            check_result.Result.AppendLine("");
                            check_result.IsSuccess &= false;
                        }
                    }

                    if (vh == null)
                    {
                        check_result.Result.AppendLine($" please check vh id.");
                        check_result.Result.AppendLine("");
                        check_result.IsSuccess &= false;
                    }

                    if (!vh.isTcpIpConnect)
                    {
                        check_result.Result.AppendLine($" vh:{vh_id} no connection");
                        check_result.Result.AppendLine($" please check IPC.");
                        check_result.Result.AppendLine("");
                        check_result.IsSuccess &= false;
                    }

                    if (vh.isSynchronizing)
                    {
                        check_result.Result.AppendLine($" vh:{vh_id} is synchronizing");
                        check_result.Result.AppendLine($" please wait.");
                        check_result.Result.AppendLine("");
                        check_result.IsSuccess &= false;
                    }

                    if (vh.MODE_STATUS == VHModeStatus.Manual || vh.MODE_STATUS == VHModeStatus.None)
                    {
                        check_result.Result.AppendLine($" vh:{vh_id} not is auto mode");
                        check_result.Result.AppendLine($" please change to auto mode.");
                        check_result.Result.AppendLine("");
                        check_result.IsSuccess &= false;
                    }

                    if (SCUtility.isEmpty(vh_current_adr))
                    {
                        check_result.Result.AppendLine($" vh:{vh_id} current address is empty");
                        check_result.Result.AppendLine($" please excute home command.");
                        check_result.Result.AppendLine("");
                        check_result.IsSuccess &= false;
                    }
                    else
                    {
                        string result = "";
                        if (!IsCommandWalkable(vh_id, cmd_type, vh_current_adr, source_address, destination_address, out result))
                        {
                            check_result.Result.AppendLine(result);
                            check_result.Result.AppendLine($" please check the segment is enable.");
                            check_result.Result.AppendLine("");
                            check_result.IsSuccess &= false;
                        }
                    }
                    //如果該筆Command是MCS Cmd，只需要檢查有沒有已經在Queue中的，有則不能Creat
                    if (!SCUtility.isEmpty(cmd_id_mcs) ||
                        cmd_type == E_CMD_TYPE.Move_MTPort)
                    {
                        if (isCMD_OHTCQueueByVh(vh_id))
                        {
                            check_result.IsSuccess &= false;
                            check_result.Result.AppendLine($" want to creat mcs transfer command:{cmd_id_mcs} of ACMD_OHTC, " +
                                                           $"but vh:{vh_id} has ACMD_OHTC in queue");
                            check_result.Result.AppendLine("");
                        }
                    }
                    else
                    {
                        if (isCMD_OHTCExcuteByVh(vh_id))
                        {
                            check_result.IsSuccess &= false;
                            check_result.Result.AppendLine($" want to creat non mcs transfer command " +
                                                           $"but vh:{vh_id} has ACMD_OHTC in excute");
                            check_result.Result.AppendLine("");
                        }
                    }
                    if (check_result.IsSuccess)
                    {
                        check_result.IsSuccess &= creatCommand_OHTC(vh_id, cmd_id_mcs, cst_id, cmd_type, source,
                                                                        destination, priority, estimated_time, gen_cmd_type, out cmd_obj,
                                                                        box_id, lot_id, source_address, destination_address);
                        if (!check_result.IsSuccess)
                        {
                            check_result.Result.AppendLine($" vh:{vh_id} creat command to db unsuccess.");
                            check_result.Result.AppendLine("");
                        }
                    }
                    if (!check_result.IsSuccess)
                    {
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(CMDBLL), Device: string.Empty,
                                      Data: check_result.Result.ToString(),
                                      XID: check_result.Num);
                    }
                    setCallContext(CALL_CONTEXT_KEY_WORD_OHTC_CMD_CHECK_RESULT, check_result);
                }
                return check_result.IsSuccess;
#endif
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }

        }




        private bool IsCommandWalkable(string vh_id, E_CMD_TYPE cmd_type, string vh_current_adr, string source, string destination, out string result)
        {
            result = "";
            try
            {
                bool is_walk_able = true;
                switch (cmd_type)
                {
                    case E_CMD_TYPE.Move:
                    case E_CMD_TYPE.Unload:
                    case E_CMD_TYPE.Move_Park:
                    case E_CMD_TYPE.MoveToMTL:
                    case E_CMD_TYPE.MTLHome:
                    case E_CMD_TYPE.SystemIn:
                    case E_CMD_TYPE.SystemOut:
                        if (!scApp.GuideBLL.IsRoadWalkable(vh_current_adr, destination).isSuccess)
                        {
                            result = $" vh:{vh_id},want excute cmd type:{cmd_type}, current address:[{vh_current_adr}] to destination address:[{destination}] no find path";
                            is_walk_able = false;
                        }
                        else
                        {
                            result = "";
                        }
                        break;
                    case E_CMD_TYPE.Scan:
                    case E_CMD_TYPE.Load:
                        if (!scApp.GuideBLL.IsRoadWalkable(vh_current_adr, source).isSuccess)
                        {
                            result = $" vh:{vh_id},want excute cmd type:{cmd_type}, current address:[{vh_current_adr}] to destination address:[{source}] no find path";
                            is_walk_able = false;
                        }
                        else
                        {
                            result = "";
                        }
                        break;
                    case E_CMD_TYPE.LoadUnload:
                        if (!scApp.GuideBLL.IsRoadWalkable(vh_current_adr, source).isSuccess)
                        {
                            result = $" vh:{vh_id},want excute cmd type:{cmd_type}, current address:{vh_current_adr} to source address:{source} no find path";
                            is_walk_able = false;
                        }
                        else if (!scApp.GuideBLL.IsRoadWalkable(source, destination).isSuccess)
                        {
                            result = $" vh:{vh_id},want excute cmd type:{cmd_type}, source address:{source} to destination address:{destination} no find path";
                            is_walk_able = false;
                        }
                        else
                        {
                            result = "";
                        }
                        break;
                    default:
                        result = $"Incorrect of command type:{cmd_type}";
                        is_walk_able = false;
                        break;
                }

                return is_walk_able;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
        }



        public ACMD_OHTC doCreatTransferCommandObj(string vh_id, string cmd_id_mcs, string cst_id, E_CMD_TYPE cmd_type,
                                    string source, string destination, int priority, int estimated_time, SCAppConstants.GenOHxCCommandType gen_cmd_type,
                                    string box_id, string lot_id, string source_address, string destination_address)
        {

            try
            {
                if (SCUtility.isEmpty(vh_id))
                {
                    return null;
                }
                else if (SCUtility.isEmpty(cmd_id_mcs))
                {

                    if (isCMD_OHTCExcuteByVh(vh_id))
                    {
                        return null;
                    }
                }
                //如果該筆Command是MCS Cmd，只需要檢查有沒有已經在Queue中的，有則不能Creat
                else
                {
                    if (isCMD_OHTCQueueByVh(vh_id))
                    {
                        return null;
                    }
                }

                return buildCommand_OHTC(vh_id, cmd_id_mcs, cmd_type,
                                             box_id, cst_id, lot_id,
                                             source, destination, source_address, destination_address,
                                             priority, estimated_time, gen_cmd_type, false);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

        }



        private bool creatCommand_OHTC(string vh_id, string cmd_id_mcs, string cst_id, E_CMD_TYPE cmd_type,
                                        string source, string destination, int priority, int estimated_time,
                                        SCAppConstants.GenOHxCCommandType gen_cmd_type, out ACMD_OHTC cmd_ohtc,
                                        string box_id, string lot_id, string source_address, string destination_address)
        {
            try
            {
#if SIMULATOR
            cmd_ohtc = new ACMD_OHTC
            {
                CMD_ID = cmd_id_mcs,
                CARRIER_ID = carrier_id,
                VH_ID = "OHT01",
                CMD_ID_MCS = cmd_id_mcs,
                CMD_TPYE = cmd_type,
                PRIORITY = priority,
                SOURCE = "T01",
                DESTINATION = "T02",
                CMD_STAUS = 0,
                CMD_PROGRESS = 0,
                ESTIMATED_EXCESS_TIME = 0,
                REAL_CMP_TIME = 0,
                ESTIMATED_TIME = 50
            };
#else
                cmd_ohtc = buildCommand_OHTC(vh_id, cmd_id_mcs, cmd_type,
                                             box_id, cst_id, lot_id,
                                             source, destination, source_address, destination_address,
                                             priority, estimated_time, gen_cmd_type);
#endif

                return creatCommand_OHTC(cmd_ohtc);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                cmd_ohtc = null;
                return false;
            }

        }

        public List<ACMD_OHTC> GetOHTCmd()
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return cmd_ohtcDAO.loadAll(con);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }


        }

        public void DeleteOHTCmd(string cmdid)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_OHTC cmd = con.ACMD_OHTC.Where(cmdd => cmdd.CMD_ID.Trim() == cmdid.Trim()).First();
                    cmd_ohtcDAO.DeleteCmdData(con, cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }

        }

        public bool creatCommand_OHTC(ACMD_OHTC cmd)
        {
            bool isSuccess = true;
            try
            {
                //DBConnection_EF con = DBConnection_EF.GetContext();
                //using (DBConnection_EF con = new DBConnection_EF())
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //同步修改至台車資料
                    //AVEHICLE vh = scApp.VehicleDao.getByID(con, cmd.VH_ID);
                    //if (vh != null)
                    //    vh.OHTC_CMD = cmd.CMD_ID;
                    cmd_ohtcDAO.add(con, cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }
            return isSuccess;
        }

        private ACMD_OHTC buildCommand_OHTC(string vh_id, string cmd_id_mcs, E_CMD_TYPE cmd_type,
                                            string box_id, string cst_id, string lot_id,
                                            string source, string destination, string source_address, string destination_address,
                                            int priority, int estimated_time,
                                            SCAppConstants.GenOHxCCommandType gen_cmd_type,
                                            bool is_generate_cmd_id = true)
        {
            try
            {
                string _source_address = string.Empty;
                string _source = string.Empty;
                string commandID = string.Empty;
                if (is_generate_cmd_id)
                {
                    commandID = scApp.SequenceBLL.getCommandID(gen_cmd_type);
                }
                if (cmd_type == E_CMD_TYPE.LoadUnload
                    || cmd_type == E_CMD_TYPE.Load || cmd_type == E_CMD_TYPE.Scan)
                {
                    _source = source;
                    _source_address = source_address;
                }
                ACMD_OHTC cmd = new ACMD_OHTC
                {
                    CMD_ID = commandID,
                    VH_ID = vh_id,
                    CARRIER_ID = cst_id,
                    CMD_ID_MCS = cmd_id_mcs,
                    CMD_TPYE = cmd_type,
                    SOURCE = _source,
                    DESTINATION = destination,
                    PRIORITY = priority,
                    CMD_STAUS = E_CMD_STATUS.Queue,
                    CMD_PROGRESS = 0,
                    ESTIMATED_TIME = estimated_time,
                    ESTIMATED_EXCESS_TIME = estimated_time,
                    SOURCE_ADR = _source_address,
                    DESTINATION_ADR = destination_address,
                    BOX_ID = box_id,
                    LOT_ID = lot_id
                };
                return cmd;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        /// <summary>
        /// 根據Command ID更新OHTC的Command狀態
        /// </summary>
        /// <param name="cmd_id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool updateCommand_OHTC_StatusByCmdID(string cmd_id, E_CMD_STATUS status)
        {
            bool isSuccess = false;
            //using (DBConnection_EF con = new DBConnection_EF())

            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_OHTC cmd = cmd_ohtcDAO.getByID(con, cmd_id);
                    if (cmd != null)
                    {
                        if (status == E_CMD_STATUS.Execution)
                        {
                            cmd.CMD_START_TIME = DateTime.Now;
                        }
                        else if (status >= E_CMD_STATUS.NormalEnd)
                        {
                            cmd.CMD_END_TIME = DateTime.Now;
                            cmd_ohtc_detailDAO.DeleteByBatch(con, cmd.CMD_ID);
                        }
                        cmd.CMD_STAUS = status;
                        cmd_ohtcDAO.Update(con, cmd);

                        if (status >= E_CMD_STATUS.NormalEnd)
                            scApp.VehicleBLL.updateVehicleExcuteCMD(cmd.VH_ID, string.Empty, string.Empty);

                    }
                    isSuccess = true;
                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }

        }


        public bool updateCMD_OHxC_Status2ReadyToReWirte(string cmd_id, out ACMD_OHTC cmd_ohtc)
        {
            bool isSuccess = false;
            //using (DBConnection_EF con = new DBConnection_EF())
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    cmd_ohtc = cmd_ohtcDAO.getByID(con, cmd_id);
                    //if (cmd != null)
                    //{
                    //    cmd_ohtc = cmd;
                    //    //cmd.CMD_STAUS = E_CMD_STATUS.Queue;
                    //    //cmd.CMD_TPYE = E_CMD_TYPE.Override;
                    //    //cmd.CMD_TPYE = E_CMD_TYPE.
                    //}
                    //else
                    isSuccess = true;
                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                cmd_ohtc = null;
                return false;
            }

        }


        public List<ACMD_OHTC> loadCMD_OHTCMDStatusIsQueue()
        {
            List<ACMD_OHTC> acmd_ohtcs = null;
            //using (DBConnection_EF con = new DBConnection_EF())

            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    acmd_ohtcs = cmd_ohtcDAO.loadAllQueue_Auto(con);
                }
                return acmd_ohtcs;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }


        }

        public ACMD_OHTC getCMD_OHTCByStatusSending()
        {
            ACMD_OHTC cmd_ohtc = null;
            try
            {
                using (DBConnection_EF con = new DBConnection_EF())
                {
                    cmd_ohtc = cmd_ohtcDAO.getCMD_OHTCByStatusSending(con);
                }
                return cmd_ohtc;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }


        }
        public ACMD_OHTC getCMD_OHTCByVehicleID(string vh_id)
        {
            ACMD_OHTC cmd_ohtc = null;
            try
            {
                using (DBConnection_EF con = new DBConnection_EF())
                {
                    cmd_ohtc = cmd_ohtcDAO.getCMD_OHTCByVehicleID(con, vh_id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

            return cmd_ohtc;
        }
        public ACMD_OHTC getExcuteCMD_OHTCByCmdID(string cmd_id)
        {
            ACMD_OHTC cmd_ohtc = null;
            try
            {
                using (DBConnection_EF con = new DBConnection_EF())
                {
                    cmd_ohtc = cmd_ohtcDAO.getExcuteCMD_OHTCByCmdID(con, cmd_id);
                }
                return cmd_ohtc;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

        }
        public ACMD_OHTC getCMD_OHTCByID(string cmdID)
        {
            ACMD_OHTC cmd_ohtc = null;
            try
            {
                using (DBConnection_EF con = new DBConnection_EF())
                {
                    cmd_ohtc = cmd_ohtcDAO.getByID(con, cmdID);
                }
                return cmd_ohtc;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(ex, "getCMD_OHTCByID");
                logger.Error(ex, "Exception");
                return null;
            }

        }

        public ACMD_OHTC getCMD_OHTCByMCScmdID(string mcs_cmd_id)
        {
            ACMD_OHTC cmd_ohtc = null;
            try
            {
                using (DBConnection_EF con = new DBConnection_EF())
                {
                    cmd_ohtc = cmd_ohtcDAO.getCMD_OHTCByMCScmdID(con, mcs_cmd_id);
                }
                return cmd_ohtc;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

        }
        public ACMD_OHTC getCMD_OHTCByMCScmdID_And_NotFinishByDest(string mcs_cmd_id, string dest)
        {
            try
            {
                using (DBConnection_EF con = new DBConnection_EF())
                {
                    return cmd_ohtcDAO.getCMD_OHTCByMCScmdID_And_NotFinishByDest(con, mcs_cmd_id, dest);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        public ACMD_OHTC getCMD_OHTCByMCScmdID_And_NotFinishBySource(string mcs_cmd_id, string source)
        {
            try
            {
                using (DBConnection_EF con = new DBConnection_EF())
                {
                    return cmd_ohtcDAO.getCMD_OHTCByMCScmdID_And_NotFinishBySource(con, mcs_cmd_id, source);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        public bool isCMD_OHTCQueueByVh(string vh_id)
        {
            int count = 0;

            //DBConnection_EF con = DBConnection_EF.GetContext();
            //using (DBConnection_EF con = new DBConnection_EF())
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    count = cmd_ohtcDAO.getVhQueueCMDConut(con, vh_id);
                }
                return count != 0;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }

        }

        public bool isCMD_OHTCExcuteByVh(string vh_id)
        {
            int count = 0;
            //DBConnection_EF con = DBConnection_EF.GetContext();
            //using (DBConnection_EF con = new DBConnection_EF())
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    count = cmd_ohtcDAO.getVhExcuteCMDConut(con, vh_id);
                }
                return count != 0;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }

        }

        public bool hasExcuteCMDFromToAdrIsParkInSpecifyParkZoneID(string park_zone_id, out int ready_come_to_count)
        {
            ready_come_to_count = 0;
            bool hasCarComeTo = false;
            List<APARKZONEDETAIL> park_zone_detail = null;
            //using (DBConnection_EF con = new DBConnection_EF())
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    park_zone_detail = scApp.ParkZoneDetailDao.loadByParkZoneID(con, park_zone_id);
                    if (park_zone_detail != null && park_zone_detail.Count > 0)
                    {
                        foreach (APARKZONEDETAIL detail in park_zone_detail)
                        {
                            int cmd_ohtc_count = cmd_ohtcDAO.getExecuteByFromAdrIsParkAdr(con, detail.ADR_ID);
                            if (cmd_ohtc_count > 0)
                            {
                                ready_come_to_count++;
                                hasCarComeTo = true;
                                continue;
                            }
                            cmd_ohtc_count = cmd_ohtcDAO.getExecuteByToAdrIsParkAdr(con, detail.ADR_ID);
                            if (cmd_ohtc_count > 0)
                            {
                                ready_come_to_count++;
                                hasCarComeTo = true;
                                continue;
                            }
                        }
                    }
                }
                return hasCarComeTo;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }

        }


        public (bool has, ACMD_OHTC cmd_ohtc) hasCMD_OHTCInQueue(string vhID)
        {
            ACMD_OHTC cmd_ohtc = null;
            try
            {
                using (DBConnection_EF con = new DBConnection_EF())
                {
                    cmd_ohtc = cmd_ohtcDAO.getQueueByVhID(con, vhID);
                }
                return (cmd_ohtc != null, cmd_ohtc);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return (false, null);
            }
        }

        public bool hasExcuteCMDFromAdr(string adr_id)
        {
            int count = 0;
            try
            {
                using (DBConnection_EF con = new DBConnection_EF())
                {
                    count = cmd_ohtcDAO.getExecuteByFromAdr(con, adr_id);
                }
                return count != 0;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
        }
        public bool hasExcuteCMDByBoxID(string boxID)
        {
            int count = 0;
            try
            {
                using (DBConnection_EF con = new DBConnection_EF())
                {
                    count = cmd_ohtcDAO.getExecuteByBoxID(con, boxID);
                }
                return count != 0;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
        }
        public bool hasExcuteCMDWantToParkAdr(string adr_id)
        {
            int count = 0;
            try
            {
                using (DBConnection_EF con = new DBConnection_EF())
                {
                    count = cmd_ohtcDAO.getExecuteByToAdrIsPark(con, adr_id);
                }
                return count != 0;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }

        }

        public bool forceUpdataCmdStatus2FnishByVhID(string vh_id)
        {
            int count = 0;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    List<ACMD_OHTC> cmds = cmd_ohtcDAO.loadExecuteCmd(con, vh_id);
                    if (cmds != null && cmds.Count > 0)
                    {
                        foreach (ACMD_OHTC cmd in cmds)
                        {
                            if (cmd.CMD_STAUS > E_CMD_STATUS.Queue)
                            {
                                updateCommand_OHTC_StatusByCmdID(cmd.CMD_ID, E_CMD_STATUS.AbnormalEndByOHTC);
                                if (!SCUtility.isEmpty(cmd.CMD_ID_MCS))
                                {
                                    //scApp.CMDBLL.updateCMD_MCS_TranStatus2Complete(cmd.CMD_ID_MCS, E_TRAN_STATUS.Aborting);
                                }
                            }
                            else
                            {
                                ACMD_OHTC queue_cmd = cmd;
                                updateCommand_OHTC_StatusByCmdID(queue_cmd.CMD_ID, E_CMD_STATUS.AbnormalEndByOHTC);
                                if (!SCUtility.isEmpty(queue_cmd.CMD_ID_MCS))
                                {
                                    ACMD_MCS pre_initial_cmd_mcs = getCMD_MCSByID(queue_cmd.CMD_ID_MCS);
                                    if (pre_initial_cmd_mcs != null /*&&
                                    pre_initial_cmd_mcs.TRANSFERSTATE == E_TRAN_STATUS.PreInitial*/)
                                    {
                                        scApp.CMDBLL.updateCMD_MCS_TranStatus2Queue(pre_initial_cmd_mcs.CMD_ID);
                                    }
                                }
                            }
                        }
                        cmd_ohtcDAO.Update(con, cmds);
                    }
                }
                return count != 0;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }

        }

        public bool isCMCD_OHTCFinish(string cmdID)
        {
            ACMD_OHTC cmd_ohtc = null;
            try
            {
                using (DBConnection_EF con = new DBConnection_EF())
                {
                    cmd_ohtc = cmd_ohtcDAO.getByID(con, cmdID);
                }
                return cmd_ohtc != null &&
                       cmd_ohtc.CMD_STAUS >= E_CMD_STATUS.NormalEnd;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }

        }

        //public bool FourceResetVhCmd()
        //{
        //    int count = 0;
        //    using (DBConnection_EF con = new DBConnection_EF())
        //    {
        //        count = cmd_ohtcDAO.getExecuteByToAdrIsPark(con, adr_id);
        //    }
        //    return count != 0;

        //}

        private long ohxc_cmd_SyncPoint = 0;
        public void checkOHxC_TransferCommand()
        {
            if (System.Threading.Interlocked.Exchange(ref ohxc_cmd_SyncPoint, 1) == 0)
            {
                try
                {
                    if (scApp.getEQObjCacheManager().getLine().ServiceMode
                        != SCAppConstants.AppServiceMode.Active)
                        return;
                    //找出目前再Queue的ACMD_OHTC
                    List<ACMD_OHTC> CMD_OHTC_Queues = scApp.CMDBLL.loadCMD_OHTCMDStatusIsQueue();
                    if (CMD_OHTC_Queues == null || CMD_OHTC_Queues.Count == 0)
                        return;
                    foreach (ACMD_OHTC cmd in CMD_OHTC_Queues)
                    {
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(CMDBLL), Device: string.Empty,
                           Data: $"Start process ohxc of command ,id:{SCUtility.Trim(cmd.CMD_ID)},vh id:{SCUtility.Trim(cmd.VH_ID)},from:{SCUtility.Trim(cmd.SOURCE)},to:{SCUtility.Trim(cmd.DESTINATION)}");

                        string vehicle_id = cmd.VH_ID.Trim();
                        AVEHICLE assignVH = scApp.VehicleBLL.getVehicleByID(vehicle_id);
                        if (!assignVH.isTcpIpConnect || assignVH.IsError || !SCUtility.isEmpty(assignVH.OHTC_CMD) || (assignVH.ACT_STATUS == VHActionStatus.Commanding))
                        {
                            LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(CMDBLL), Device: string.Empty,
                               Data: $"can't send command ,id:{SCUtility.Trim(cmd.CMD_ID)},vh id:{SCUtility.Trim(cmd.VH_ID)} current status not allowed." +
                               $"is connect:{assignVH.isTcpIpConnect },is error:{assignVH.IsError }, current assign ohtc cmd id:{assignVH.OHTC_CMD}." +
                               $"assignVH.ACT_STATUS:{assignVH.ACT_STATUS}.");
                            continue;
                        }

                        bool is_send_success = scApp.VehicleService.doSendOHxCCmdToVh(assignVH, cmd);
                        if (is_send_success)
                        {
                            assignVH.AssignCommandFailTimes = 0;
                        }
                        else
                        {
                            assignVH.AssignCommandFailTimes++;
                            // function to tell OHBC that the cmd is unable to execute.
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(CMDBLL), Device: "OHxC",
                       Data: ex);
                    throw ex;
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref ohxc_cmd_SyncPoint, 0);
                }
            }
        }

        public static T getOrSetCallContext<T>(string key)
        {
            //object obj = System.Runtime.Remoting.Messaging.CallContext.GetData(key);
            //if (obj == null)
            //{
            //    obj = Activator.CreateInstance(typeof(T));
            //    System.Runtime.Remoting.Messaging.CallContext.SetData(key, obj);
            //}
            object obj = Activator.CreateInstance(typeof(T));
            System.Runtime.Remoting.Messaging.CallContext.SetData(key, obj);
            return (T)obj;
        }
        public static T getCallContext<T>(string key)
        {
            object obj = System.Runtime.Remoting.Messaging.CallContext.GetData(key);
            if (obj == null)
            {
                return default(T);
            }
            return (T)obj;
        }
        public static void setCallContext<T>(string key, T obj)
        {
            if (obj != null)
            {
                System.Runtime.Remoting.Messaging.CallContext.SetData(key, obj);
            }
        }

        #endregion CMD_OHTC

        #region CMD_OHTC_DETAIL
        public bool tryGenerateCmd_OHTC_Details(ACMD_OHTC acmd_ohtc, out ActiveType active_type, out string[] route_sections, out string[] cycle_run_sections
                                                                                            , out string[] minRouteSec_Vh2From, out string[] minRouteSec_From2To
                                                                                            , out string[] minRouteAdr_Vh2From, out string[] minRouteAdr_From2To)
        {
            active_type = default(ActiveType);
            route_sections = null;
            cycle_run_sections = null;
            minRouteSec_Vh2From = null;
            minRouteSec_From2To = null;
            minRouteAdr_Vh2From = null;
            minRouteAdr_From2To = null;
            try
            {
                if (acmd_ohtc == null)
                {
                    return false;
                }

                string Reason = string.Empty;
                SCUtility.TrimAllParameter(acmd_ohtc);
                scApp.VehicleBLL.getAndProcPositionReportFromRedis(acmd_ohtc.VH_ID);
                AVEHICLE vehicle = scApp.VehicleBLL.getVehicleByID(acmd_ohtc.VH_ID);
                SCUtility.TrimAllParameter(vehicle);

                List<string> minRouteSeg = new List<string>();
                string[] minRouteSec_Vh2FromTemp = null;
                string[] minRouteSec_From2ToTemp = null;
                string[] minRouteAdr_Vh2FromTemp = null;
                string[] minRouteAdr_From2ToTemp = null;

                string cmd_id = acmd_ohtc.CMD_ID;
                string vh_current_adr = vehicle.CUR_ADR_ID;
                string source_adr = acmd_ohtc.SOURCE_ADR;
                string dest_adr = acmd_ohtc.DESTINATION_ADR;
                ActiveType activeType = ActiveType.Move;
                activeType = convert_E_CMD_TYPE2ActiveType(acmd_ohtc);

                var guide_info = FindGuideInfo(vh_current_adr, source_adr, dest_adr, activeType);
                if (guide_info.isSuccess)
                {
                    if (guide_info.guide_start_to_from_section_ids != null)
                    {
                        minRouteSec_Vh2FromTemp = guide_info.guide_start_to_from_section_ids.ToArray();
                        minRouteAdr_Vh2FromTemp = guide_info.guide_start_to_from_address_ids.ToArray();
                        minRouteSeg.AddRange(guide_info.guide_start_to_from_section_ids);
                    }
                    if (guide_info.guide_to_dest_section_ids != null)
                    {
                        minRouteSec_From2ToTemp = guide_info.guide_to_dest_section_ids.ToArray();
                        minRouteAdr_From2ToTemp = guide_info.guide_to_dest_address_ids.ToArray();
                        minRouteSeg.AddRange(guide_info.guide_to_dest_section_ids);
                    }
                }
                else
                {
                    throw new Exception(string.Format("can't find from to of route.cmd id:{0}", acmd_ohtc.CMD_ID));
                }

                //找出路徑後，則將該路徑所有經過的Section存入資料庫中，並且將ACMD_OHTC的命令改為發送中
                if (creatCmd_OHTC_DetailByBatch(acmd_ohtc.CMD_ID, minRouteSeg))
                {
                    scApp.CMDBLL.updateCommand_OHTC_StatusByCmdID(acmd_ohtc.CMD_ID, E_CMD_STATUS.Sending);
                }
                active_type = activeType;
                route_sections = minRouteSeg.ToArray();
                minRouteSec_Vh2From = minRouteSec_Vh2FromTemp;
                minRouteSec_From2To = minRouteSec_From2ToTemp;
                minRouteAdr_Vh2From = minRouteAdr_Vh2FromTemp;
                minRouteAdr_From2To = minRouteAdr_From2ToTemp;
                return true;
            }
            catch (VehicleBLL.BlockedByTheErrorVehicleException blockedException)
            {
                throw blockedException;
            }
            catch (Exception ex)
            {
                updateCommand_OHTC_StatusByCmdID(acmd_ohtc.CMD_ID, E_CMD_STATUS.AbnormalEndByOHTC);
                logger_VhRouteLog.Error(ex, "generateCmd_OHTC_Details happend");
                return false;
            }
        }

        private void tryFilterFirstSection(AVEHICLE vehicle, ref List<string> minRouteSeg)
        {
            try
            {
                if (minRouteSeg == null || minRouteSeg.Count == 0)
                {
                    return;
                }
                string first_sec = minRouteSeg[0];
                ASECTION first_sec_obj = scApp.MapBLL.getSectiontByID(first_sec);
                if (first_sec_obj != null &&
                    //vehicle.ACC_SEC_DIST == vh_current_section.SEC_DIS)
                    SCUtility.isMatche(vehicle.CUR_ADR_ID, first_sec_obj.TO_ADR_ID))
                {
                    minRouteSeg.RemoveAt(0);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }

        public string[] findBestFitRoute(string vh_crt_sec, string[] AllRouteInfo, string targetAdr)
        {
            try
            {

                string[] FitRouteSec = null;
                //try
                //{
                List<string> crtByPassSeg = ByPassSegment.ToList();
                filterByPassSec_VhAlreadyOnSec(vh_crt_sec, crtByPassSeg);
                filterByPassSec_TargetAdrOnSec(targetAdr, crtByPassSeg);
                string[] AllRoute = AllRouteInfo[1].Split(';');
                List<KeyValuePair<string[], double>> routeDetailAndDistance = PaserRoute2SectionsAndDistance(AllRoute);
                //if (scApp.getEQObjCacheManager().getLine().SegmentPreDisableExcuting)
                //{
                //    List<string> nonActiveSeg = scApp.MapBLL.loadNonActiveSegmentNum();
                //filterByPassSec_VhAlreadyOnSec(vh_crt_sec, nonActiveSeg);
                //filterByPassSec_TargetAdrOnSec(targetAdr, nonActiveSeg);
                foreach (var routeDetial in routeDetailAndDistance.ToList())
                {
                    List<ASECTION> lstSec = scApp.MapBLL.loadSectionBySecIDs(routeDetial.Key.ToList());
                    if (scApp.getEQObjCacheManager().getLine().SegmentPreDisableExcuting)
                    {
                        List<string> nonActiveSeg = scApp.MapBLL.loadNonActiveSegmentNum();
                        string[] secOfSegments = lstSec.Select(s => s.SEG_NUM).Distinct().ToArray();
                        bool isIncludePassSeg = secOfSegments.Where(seg => nonActiveSeg.Contains(seg)).Count() != 0;
                        if (isIncludePassSeg)
                        {
                            routeDetailAndDistance.Remove(routeDetial);
                        }
                    }
                }
                foreach (var routeDetial in routeDetailAndDistance.ToList())
                {
                    List<ASECTION> lstSec = scApp.MapBLL.loadSectionBySecIDs(routeDetial.Key.ToList());
                    List<AVEHICLE> vhs = scApp.VehicleBLL.loadAllErrorVehicle();
                    foreach (AVEHICLE vh in vhs)
                    {
                        bool IsErrorVhOnPassSection = lstSec.Where(sec => sec.SEC_ID.Trim() == vh.CUR_SEC_ID.Trim()).Count() > 0;
                        if (IsErrorVhOnPassSection)
                        {
                            routeDetailAndDistance.Remove(routeDetial);
                            if (routeDetailAndDistance.Count == 0)
                            {
                                throw new VehicleBLL.BlockedByTheErrorVehicleException
                                    ($"Can't find the way to transfer.Because block by error vehicle [{vh.VEHICLE_ID}] on sec [{vh.CUR_SEC_ID}]");
                            }
                        }
                    }
                }
                //}

                if (routeDetailAndDistance.Count == 0)
                {
                    return null;
                }

                foreach (var routeDetial in routeDetailAndDistance)
                {
                    List<ASECTION> lstSec = scApp.MapBLL.loadSectionBySecIDs(routeDetial.Key.ToList());
                    string[] secOfSegments = lstSec.Select(s => s.SEG_NUM).Distinct().ToArray();
                    bool isIncludePassSeg = secOfSegments.Where(seg => crtByPassSeg.Contains(seg)).Count() != 0;
                    if (isIncludePassSeg)
                    {
                        continue;
                    }
                    else
                    {
                        FitRouteSec = routeDetial.Key;
                        break;
                    }
                }
                if (FitRouteSec == null)
                {
                    routeDetailAndDistance = routeDetailAndDistance.OrderBy(o => o.Value).ToList();
                    FitRouteSec = routeDetailAndDistance.First().Key;
                }
                //}
                //catch (Exception ex)
                //{
                //    logger_VhRouteLog.Error(ex, "Exception");
                //}
                return FitRouteSec;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

        }

        //public string[] findBestFitRoute(string vh_crt_sec, string[] AllRouteInfo, string targetAdr)
        //{
        //    string[] FitRouteSec = null;
        //    try
        //    {
        //        List<string> crtByPassSeg = ByPassSegment.ToList();
        //        ASECTION vh_current_sec = scApp.MapBLL.getSectiontByID(vh_crt_sec);
        //        if (vh_current_sec != null)
        //        {
        //            if (crtByPassSeg.Contains(vh_current_sec.SEG_NUM))
        //            {
        //                crtByPassSeg.Remove(vh_current_sec.SEG_NUM);
        //            }
        //        }
        //        List<ASECTION> adrOfSecs = scApp.MapBLL.loadSectionsByFromOrToAdr(targetAdr);
        //        string[] adrSecOfSegments = adrOfSecs.Select(s => s.SEG_NUM).Distinct().ToArray();
        //        if (adrSecOfSegments != null && adrSecOfSegments.Count() > 0)
        //        {
        //            foreach (string seg in adrSecOfSegments)
        //            {
        //                if (crtByPassSeg.Contains(seg))
        //                {
        //                    crtByPassSeg.Remove(seg);
        //                }
        //            }
        //        }

        //        string[] AllRoute = AllRouteInfo[1].Split(';');
        //        foreach (string routeDetial in AllRoute)
        //        {
        //            string route = routeDetial.Split('=')[0];
        //            string[] routeSection = route.Split(',');
        //            List<ASECTION> lstSec = scApp.MapBLL.loadSectionBySecIDs(routeSection.ToList());
        //            //if (passSegment.Contains(lstSec[0].SEG_NUM.Trim()))
        //            //{
        //            //    FitRouteSec = routeSection;
        //            //    break;
        //            //}
        //            string[] secOfSegments = lstSec.Select(s => s.SEG_NUM).Distinct().ToArray();
        //            bool isIncludePassSeg = secOfSegments.Where(seg => crtByPassSeg.Contains(seg)).Count() != 0;
        //            if (isIncludePassSeg)
        //            {
        //                //List<ASECTION> adrOfSecs = scApp.MapBLL.loadSectionsByFromOrToAdr(targetAdr);
        //                //string[] adrSecOfSegments = adrOfSecs.Select(s => s.SEG_NUM).Distinct().ToArray();
        //                //isIncludePassSeg = adrSecOfSegments.Where(seg => crtByPassSeg.Contains(seg)).Count() != 0;
        //                //if (isIncludePassSeg)
        //                //{
        //                //    FitRouteSec = routeSection;
        //                //    break;
        //                //}
        //                //else
        //                //{
        //                continue;
        //                //}
        //            }
        //            else
        //            {
        //                FitRouteSec = routeSection;
        //                break;
        //            }
        //        }
        //        if (FitRouteSec == null)
        //        {
        //            string[] minRoute = AllRouteInfo[0].Split('=');
        //            FitRouteSec = minRoute[0].Split(',');
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger_VhRouteLog.Error(ex, "Exception");
        //        string[] minRoute = AllRouteInfo[0].Split('=');
        //        FitRouteSec = minRoute[0].Split(',');
        //    }
        //    return FitRouteSec;
        //}

        private void filterByPassSec_TargetAdrOnSec(string targetAdr, List<string> crtByPassSeg)
        {
            try
            {
                if (SCUtility.isEmpty(targetAdr)) return;
                List<ASECTION> adrOfSecs = scApp.MapBLL.loadSectionsByFromOrToAdr(targetAdr);
                string[] adrSecOfSegments = adrOfSecs.Select(s => s.SEG_NUM).Distinct().ToArray();
                if (adrSecOfSegments != null && adrSecOfSegments.Count() > 0)
                {
                    foreach (string seg in adrSecOfSegments)
                    {
                        if (crtByPassSeg.Contains(seg))
                        {
                            crtByPassSeg.Remove(seg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }

        }

        private void filterByPassSec_VhAlreadyOnSec(string vh_crt_sec, List<string> crtByPassSeg)
        {
            ASECTION vh_current_sec = scApp.MapBLL.getSectiontByID(vh_crt_sec);
            if (vh_current_sec != null)
            {
                if (crtByPassSeg.Contains(vh_current_sec.SEG_NUM))
                {
                    crtByPassSeg.Remove(vh_current_sec.SEG_NUM);
                }
            }
        }

        private List<KeyValuePair<string[], double>> PaserRoute2SectionsAndDistance(string[] AllRoute)
        {
            try
            {
                List<KeyValuePair<string[], double>> routeDetailAndDistance = new List<KeyValuePair<string[], double>>();
                foreach (string routeDetial in AllRoute)
                {
                    string route = routeDetial.Split('=')[0];
                    string[] routeSection = route.Split(',');
                    string distance = routeDetial.Split('=')[1];
                    double idistance = double.MaxValue;
                    if (!double.TryParse(distance, out idistance))
                    {
                        logger.Warn($"fun:{nameof(PaserRoute2SectionsAndDistance)},parse distance fail.Route:{route},distance:{distance}");
                    }
                    routeDetailAndDistance.Add(new KeyValuePair<string[], double>(routeSection, idistance));
                }
                return routeDetailAndDistance;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }

        //private static void setVhExcuteCmdToShow(ACMD_OHTC acmd_ohtc, AVEHICLE vehicle, Equipment eqpt, string[] min_route_seq)
        public void setVhExcuteCmdToShow(ACMD_OHTC acmd_ohtc, AVEHICLE vehicle, string[] min_route_seq,
                                        List<string> guideSectionStartToLoad, List<string> guideSectionToDestination,
                                        string[] cycle_run_sections)
        {
            try
            {
                AVEHICLE _vhCatchObject = scApp.getEQObjCacheManager().getVehicletByVHID(acmd_ohtc.VH_ID);
                _vhCatchObject.MCS_CMD = acmd_ohtc.CMD_ID_MCS;
                _vhCatchObject.OHTC_CMD = acmd_ohtc.CMD_ID;
                _vhCatchObject.startAdr = vehicle.CUR_ADR_ID;
                _vhCatchObject.FromAdr = acmd_ohtc.SOURCE;
                _vhCatchObject.ToAdr = acmd_ohtc.DESTINATION;
                _vhCatchObject.CMD_CST_ID = acmd_ohtc.CARRIER_ID;
                _vhCatchObject.CMD_Priority = acmd_ohtc.PRIORITY;
                _vhCatchObject.CmdType = acmd_ohtc.CMD_TPYE;

                _vhCatchObject.PredictPath = min_route_seq;
                _vhCatchObject.PredictSectionsStartToLoad = guideSectionStartToLoad;
                _vhCatchObject.PredictSectionsToDesination = guideSectionToDestination;


                //_vhCatchObject.WillPassSectionID = min_route_seq.ToList();
                if (guideSectionStartToLoad != null && guideSectionStartToLoad.Count > 0)
                {
                    setWillPassSectionInfo(acmd_ohtc.VH_ID, guideSectionStartToLoad);
                }
                else
                {
                    setWillPassSectionInfo(acmd_ohtc.VH_ID, guideSectionToDestination);
                }
                _vhCatchObject.CyclingPath = cycle_run_sections;
                _vhCatchObject.vh_CMD_Status = E_CMD_STATUS.Execution;
                _vhCatchObject.NotifyVhExcuteCMDStatusChange();
                //_vhCatchObject.VID_Collection.VID_58_CommandID.COMMAND_ID = acmd_ohtc.CMD_ID_MCS;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }

        }

        public void setWillPassSectionInfo(string vhID, List<string> willPassSection)
        {
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vhID);
            if (vh != null)
                vh.WillPassSectionID = willPassSection;
        }

        public void removeAllWillPassSection(string vhID)
        {
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vhID);
            if (vh != null)
                vh.WillPassSectionID = null;
        }
        public void removePassSection(string vhID, string passSection)
        {
            AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(vhID);
            if (vh != null && vh.WillPassSectionID != null)
                vh.WillPassSectionID.Remove(passSection);
        }

        public void initialVhExcuteCmdToShow(AVEHICLE vehicle)
        {
            try
            {
                //vehicle.startAdr = null;
                vehicle.FromAdr = null;
                vehicle.ToAdr = null;
                vehicle.CMD_CST_ID = null;
                vehicle.CMD_Priority = 0;
                vehicle.CmdType = E_CMD_TYPE.Home;

                vehicle.PredictPath = null;
                vehicle.WillPassSectionID = null;
                vehicle.CyclingPath = null;
                vehicle.vh_CMD_Status = E_CMD_STATUS.NormalEnd;
                vehicle.NotifyVhExcuteCMDStatusChange();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }

        }

        private static ActiveType convert_E_CMD_TYPE2ActiveType(ACMD_OHTC acmd_ohtc)
        {
            ActiveType activeType;
            switch (acmd_ohtc.CMD_TPYE)
            {
                case E_CMD_TYPE.Move:
                case E_CMD_TYPE.Move_Park:
                case E_CMD_TYPE.Move_MTPort:
                    activeType = ActiveType.Move;
                    break;
                case E_CMD_TYPE.MoveToMTL:
                    activeType = ActiveType.Movetomtl;
                    break;
                case E_CMD_TYPE.SystemIn:
                    activeType = ActiveType.Systemin;
                    break;
                case E_CMD_TYPE.SystemOut:
                    activeType = ActiveType.Systemout;
                    break;
                case E_CMD_TYPE.Scan:
                    activeType = ActiveType.Scan;
                    break;
                case E_CMD_TYPE.Load:
                    activeType = ActiveType.Load;
                    break;
                case E_CMD_TYPE.Unload:
                    activeType = ActiveType.Unload;
                    break;
                case E_CMD_TYPE.LoadUnload:
                    activeType = ActiveType.Loadunload;
                    break;
                case E_CMD_TYPE.Teaching:
                    activeType = ActiveType.Home;
                    break;
                case E_CMD_TYPE.MTLHome:
                    activeType = ActiveType.Mtlhome;
                    break;
                //case E_CMD_TYPE.Round:
                //    activeType = ActiveType.Round;
                //    break;
                case E_CMD_TYPE.Override:
                    activeType = ActiveType.Override;
                    break;

                default:
                    throw new Exception(string.Format("OHT Command type:{0} , not in the definition"
                                                     , acmd_ohtc.CMD_TPYE.ToString()));
            }
            return activeType;
        }


        //private void findTransferRoute(ACMD_OHTC acmd_ohtc, AVEHICLE vehicle, ref string[] ReutrnVh2FromAdr, ref string[] ReutrnFromAdr2ToAdr, bool isIgnoreSegStatus)
        //{
        //    AADDRESS from_adr = null;
        //    if (SCUtility.isEmpty(acmd_ohtc.SOURCE) ||
        //        (!SCUtility.isEmpty(acmd_ohtc.SOURCE) && vehicle.HAS_CST == 1))
        //    {
        //        from_adr = scApp.MapBLL.getAddressByID(vehicle.CUR_ADR_ID);
        //    }
        //    else
        //    {
        //        from_adr = scApp.MapBLL.getAddressByID(acmd_ohtc.SOURCE);
        //    }

        //    AADDRESS to_adr = scApp.MapBLL.getAddressByID(acmd_ohtc.DESTINATION);
        //    switch (from_adr.ADRTYPE)
        //    {
        //        case E_ADR_TYPE.Address:
        //        case E_ADR_TYPE.Port:
        //            //if (!SCUtility.isEmpty(acmd_ohtc.SOURCE) && !SCUtility.isMatche(acmd_ohtc.SOURCE, vehicle.CUR_ADR_ID))
        //            if (needVh2FromAddressOfGuide(acmd_ohtc, vehicle))
        //            //if (!SCUtility.isMatche(acmd_ohtc.SOURCE, vehicle.CUR_ADR_ID) ||
        //            //    (!SCUtility.isEmpty(acmd_ohtc.SOURCE) && vehicle.HAS_CST == 0))
        //            {
        //                ReutrnVh2FromAdr = scApp.RouteGuide.DownstreamSearchSection_FromSecToAdr
        //                (vehicle.CUR_SEC_ID, from_adr.ADR_ID, 1, isIgnoreSegStatus);
        //            }
        //            if (to_adr != null)
        //            {
        //                switch (to_adr.ADRTYPE)
        //                {
        //                    case E_ADR_TYPE.Address:
        //                    case E_ADR_TYPE.Port:
        //                        if (acmd_ohtc.CMD_TPYE == E_CMD_TYPE.Move)
        //                        {
        //                            ReutrnFromAdr2ToAdr = scApp.RouteGuide.DownstreamSearchSection_FromSecToAdr
        //                            (vehicle.CUR_SEC_ID, to_adr.ADR_ID, 1, isIgnoreSegStatus);
        //                        }
        //                        else
        //                        {
        //                            if (!SCUtility.isMatche(from_adr.ADR_ID, to_adr.ADR_ID))
        //                            {
        //                                ReutrnFromAdr2ToAdr = scApp.RouteGuide.DownstreamSearchSection
        //                                (from_adr.ADR_ID, to_adr.ADR_ID, 1, isIgnoreSegStatus);
        //                            }
        //                        }
        //                        //if (vehicle.HAS_CST == 0)
        //                        //{
        //                        //    ReutrnFromAdr2ToAdr = scApp.RouteGuide.DownstreamSearchSection
        //                        //        (from_adr.ADR_ID, to_adr.ADR_ID, 1, isIgnoreSegStatus);
        //                        //}
        //                        //else
        //                        //{
        //                        //    ReutrnFromAdr2ToAdr = scApp.RouteGuide.DownstreamSearchSection_FromSecToAdr
        //                        //        (vehicle.CUR_SEC_ID, to_adr.ADR_ID, 1, isIgnoreSegStatus);
        //                        //}
        //                        break;
        //                    case E_ADR_TYPE.Control:
        //                        ASECTION to_sec = scApp.MapBLL.getSectiontByID(to_adr.SEC_ID);
        //                        ReutrnFromAdr2ToAdr = scApp.RouteGuide.DownstreamSearchSection
        //                            (from_adr.ADR_ID, to_sec.TO_ADR_ID, 1, isIgnoreSegStatus);
        //                        break;
        //                }
        //            }
        //            break;
        //        case E_ADR_TYPE.Control:
        //            if (!SCUtility.isEmpty(acmd_ohtc.SOURCE) && !SCUtility.isMatche(acmd_ohtc.SOURCE, vehicle.CUR_ADR_ID))
        //            //if (!SCUtility.isMatche(acmd_ohtc.SOURCE, vehicle.CUR_ADR_ID) ||
        //            //    (!SCUtility.isEmpty(acmd_ohtc.SOURCE) && vehicle.HAS_CST == 0))
        //            {
        //                ReutrnVh2FromAdr = scApp.RouteGuide.DownstreamSearchSection_FromSecToSec
        //                    (vehicle.CUR_SEC_ID, from_adr.SEC_ID, 1, false, isIgnoreSegStatus);
        //            }
        //            if (to_adr != null)
        //            {
        //                switch (to_adr.ADRTYPE)
        //                {
        //                    case E_ADR_TYPE.Address:
        //                    case E_ADR_TYPE.Port:
        //                        ReutrnFromAdr2ToAdr = scApp.RouteGuide.DownstreamSearchSection_FromSecToAdr
        //                            (from_adr.SEC_ID, to_adr.ADR_ID, 1, isIgnoreSegStatus);
        //                        break;
        //                    case E_ADR_TYPE.Control:
        //                        ReutrnFromAdr2ToAdr = scApp.RouteGuide.DownstreamSearchSection_FromSecToSec
        //                            (from_adr.SEC_ID, to_adr.SEC_ID, 1, false, isIgnoreSegStatus);
        //                        break;
        //                }
        //            }
        //            break;
        //    }
        //    string vh_id = vehicle.VEHICLE_ID;
        //    string vh_crt_adr = vehicle.CUR_ADR_ID;
        //    string sfrom_adr = from_adr == null ? string.Empty : from_adr.ADR_ID;
        //    string sto_adr = to_adr == null ? string.Empty : to_adr.ADR_ID;
        //    string svh2FromAdr = (ReutrnVh2FromAdr != null && ReutrnVh2FromAdr.Count() > 0) ? ReutrnVh2FromAdr[0] : string.Empty;
        //    string sFromAdr2ToAdr = (ReutrnFromAdr2ToAdr != null && ReutrnFromAdr2ToAdr.Count() > 0) ? ReutrnFromAdr2ToAdr[0] : string.Empty;
        //    logger_VhRouteLog.Debug(string.Format("Vh ID [{0}], vh crt adr[{1}] ,from adr [{2}],to adr [{3}] \r vh2FromAdr sec[{4}]\r FromAdr2ToAdr sec[{5}]"
        //        , vh_id
        //        , vh_crt_adr
        //        , sfrom_adr
        //        , sto_adr
        //        , svh2FromAdr
        //        , sFromAdr2ToAdr));

        //}

        public (bool isSuccess, int total_code,
            List<string> guide_start_to_from_section_ids,
            List<string> guide_start_to_from_address_ids,
             List<string> guide_to_dest_section_ids,
             List<string> guide_to_dest_address_ids)
            FindGuideInfo(string vh_current_address, string source_adr, string dest_adr, ActiveType active_type, bool has_carray = false, List<string> byPassSectionIDs = null)
        {
            bool isSuccess = true;
            List<string> guide_start_to_from_segment_ids = null;
            List<string> guide_start_to_from_section_ids = null;
            List<string> guide_start_to_from_address_ids = null;
            List<string> guide_to_dest_segment_ids = null;
            List<string> guide_to_dest_section_ids = null;
            List<string> guide_to_dest_address_ids = null;
            int total_cost = 0;
            //1.取得行走路徑的詳細資料
            try
            {
                switch (active_type)
                {
                    case ActiveType.Loadunload:
                        if (has_carray)
                        {
                            if (!SCUtility.isMatche(vh_current_address, dest_adr))
                            {
                                (isSuccess, guide_to_dest_segment_ids, guide_to_dest_section_ids, guide_to_dest_address_ids, total_cost)
                                    = scApp.GuideBLL.getGuideInfo(vh_current_address, dest_adr, byPassSectionIDs);
                            }
                        }
                        else
                        {
                            if (!SCUtility.isMatche(vh_current_address, source_adr))
                            {
                                (isSuccess, guide_start_to_from_segment_ids, guide_start_to_from_section_ids, guide_start_to_from_address_ids, total_cost)
                                    = scApp.GuideBLL.getGuideInfo(vh_current_address, source_adr, byPassSectionIDs);
                            }
                            if (isSuccess && !SCUtility.isMatche(source_adr, dest_adr))
                            {
                                (isSuccess, guide_to_dest_segment_ids, guide_to_dest_section_ids, guide_to_dest_address_ids, total_cost)
                                    = scApp.GuideBLL.getGuideInfo(source_adr, dest_adr, null);
                            }
                        }
                        break;
                    case ActiveType.Load:
                        if (!SCUtility.isMatche(vh_current_address, source_adr))
                        {
                            (isSuccess, guide_start_to_from_segment_ids, guide_start_to_from_section_ids, guide_start_to_from_address_ids, total_cost)
                                = scApp.GuideBLL.getGuideInfo(vh_current_address, source_adr, byPassSectionIDs);
                        }
                        else
                        {
                            isSuccess = true; //如果相同 代表是在同一個點上
                        }
                        break;
                    case ActiveType.Scan:
                        if (!SCUtility.isMatche(vh_current_address, source_adr))
                        {
                            (isSuccess, guide_start_to_from_segment_ids, guide_start_to_from_section_ids, guide_start_to_from_address_ids, total_cost)
                                = scApp.GuideBLL.getGuideInfo(vh_current_address, source_adr, byPassSectionIDs);
                        }
                        else
                        {
                            isSuccess = true; //如果相同 代表是在同一個點上
                        }
                        break;
                    case ActiveType.Unload:
                        if (!SCUtility.isMatche(vh_current_address, dest_adr))
                        {
                            (isSuccess, guide_to_dest_segment_ids, guide_to_dest_section_ids, guide_to_dest_address_ids, total_cost)
                                = scApp.GuideBLL.getGuideInfo(vh_current_address, dest_adr, byPassSectionIDs);
                        }
                        else
                        {
                            isSuccess = true;//如果相同 代表是在同一個點上
                        }
                        break;
                    case ActiveType.Move:
                        if (!SCUtility.isMatche(vh_current_address, dest_adr))
                        {
                            (isSuccess, guide_to_dest_segment_ids, guide_to_dest_section_ids, guide_to_dest_address_ids, total_cost)
                                = scApp.GuideBLL.getGuideInfo(vh_current_address, dest_adr, byPassSectionIDs);
                        }
                        else
                        {
                            isSuccess = false;
                        }
                        break;
                }
                return (isSuccess, total_cost, guide_start_to_from_section_ids, guide_start_to_from_address_ids, guide_to_dest_section_ids, guide_to_dest_address_ids);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return (isSuccess, total_cost, guide_start_to_from_section_ids, guide_start_to_from_address_ids, guide_to_dest_section_ids, guide_to_dest_address_ids);
            }


        }

        private bool needVh2FromAddressOfGuide(ACMD_OHTC acmd_ohtc, AVEHICLE vehicle)
        {
            bool is_need = true;
            string cmd_source_adr = acmd_ohtc.SOURCE;
            string vh_current_adr = vehicle.CUR_ADR_ID;
            string vh_current_sec = vehicle.CUR_SEC_ID;
            double vh_sec_dist = vehicle.ACC_SEC_DIST;

            try
            {
                if (SCUtility.isEmpty(cmd_source_adr)
                || (!SCUtility.isEmpty(cmd_source_adr) && vehicle.HAS_CST == 1))
                {
                    is_need = false;
                }

                if (is_need && SCUtility.isMatche(cmd_source_adr, vh_current_adr))
                {
                    is_need = false;
                    //var last_and_next_sections = scApp.MapBLL.loadSectionsByFromOrToAdr(vh_current_adr);
                    //foreach (ASECTION sec in last_and_next_sections)
                    //{
                    //    //如果車子在該段Section的開頭時，就可以不用給他From到Source的Sec
                    //    if (SCUtility.isMatche(sec.FROM_ADR_ID, vh_current_adr))
                    //    {
                    //        if (SCUtility.isMatche(sec.SEC_ID, vh_current_sec))
                    //        {
                    //            if (vh_sec_dist == 0)
                    //                is_need = false;
                    //        }
                    //    }
                    //}
                }
                return is_need;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
        }
        //private bool isVhInSectionStartingPoint(AVEHICLE vh)
        //{

        //}

        public bool creatCmd_OHTC_Details(string cmd_id, List<string> sec_ids)
        {
            bool isSuccess = false;
            ASECTION section = null;
            try
            {
                //List<ASECTION> lstSce = scApp.MapBLL.loadSectionBySecIDs(sec_ids);
                for (int i = 0; i < sec_ids.Count; i++)
                {
                    section = scApp.MapBLL.getSectiontByID(sec_ids[i]);
                    creatCommand_OHTC_Detail(cmd_id, i + 1, section.FROM_ADR_ID, section.SEC_ID, section.SEG_NUM, 0);
                }
                isSuccess = true;
            }
            catch (Exception ex)
            {
                logger_VhRouteLog.Error(ex, "Exception");
                throw ex;
            }
            return isSuccess;
        }

        public bool creatCommand_OHTC_Detail(string cmd_id, int seq_no, string add_id,
                                      string sec_id, string seg_num, int estimated_time)
        {
            bool isSuccess = false;
            //using (DBConnection_EF con = new DBConnection_EF())
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_OHTC_DETAIL cmd = new ACMD_OHTC_DETAIL
                    {
                        CMD_ID = cmd_id,
                        SEQ_NO = seq_no,
                        ADD_ID = add_id,
                        SEC_ID = sec_id,
                        SEG_NUM = seg_num,
                        ESTIMATED_TIME = estimated_time
                    };
                    cmd_ohtc_detailDAO.add(con, cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                ACMD_OHTC_DETAIL cmd = new ACMD_OHTC_DETAIL
                {
                    CMD_ID = cmd_id,
                    SEQ_NO = seq_no,
                    ADD_ID = add_id,
                    SEC_ID = sec_id,
                    SEG_NUM = seg_num,
                    ESTIMATED_TIME = estimated_time
                };
                cmd_ohtc_detailDAO.add(con, cmd);
            }
            return isSuccess;
        }

        public bool creatCmd_OHTC_DetailByBatch(string cmd_id, List<string> sec_ids)
        {
            //using (DBConnection_EF con = new DBConnection_EF())
            int start_seq_no = 0;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    ACMD_OHTC_DETAIL last_cmd_detail = cmd_ohtc_detailDAO.getLastByID(con, cmd_id);
                    if (last_cmd_detail != null)
                    {
                        start_seq_no = last_cmd_detail.SEQ_NO;
                    }
                }
                List<ACMD_OHTC_DETAIL> cmd_details = new List<ACMD_OHTC_DETAIL>();
                foreach (string sec_id in sec_ids)
                {
                    ASECTION section = scApp.MapBLL.getSectiontByID(sec_id);
                    ACMD_OHTC_DETAIL cmd_detail = new ACMD_OHTC_DETAIL()
                    {
                        CMD_ID = cmd_id,
                        SEQ_NO = ++start_seq_no,
                        ADD_ID = section.FROM_ADR_ID,
                        SEC_ID = section.SEC_ID,
                        SEG_NUM = section.SEG_NUM,
                        ESTIMATED_TIME = 0
                    };
                    cmd_details.Add(cmd_detail);

                }
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    cmd_ohtc_detailDAO.AddByBatch(con, cmd_details);
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }


        }

        public bool update_CMD_DetailEntryTime(string cmd_id,
                                               string add_id,
                                               string sec_id)
        {
            bool isSuccess = false;
            //DBConnection_EF con = DBConnection_EF.GetContext();
            //using (DBConnection_EF con = new DBConnection_EF())
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //ACMD_OHTC cmd_oht = cmd_ohtcDAO.getExecuteByVhID(con, vh_id);
                    ACMD_OHTC_DETAIL cmd_detail = cmd_ohtc_detailDAO.
                        getByCmdIDSECIDAndEntryTimeEmpty(con, cmd_id, sec_id);
                    if (cmd_detail != null)
                    {
                        DateTime nowTime = DateTime.Now;
                        cmd_detail.ADD_ENTRY_TIME = nowTime;
                        cmd_detail.SEC_ENTRY_TIME = nowTime;
                        cmd_ohtc_detailDAO.Update(con, cmd_detail);
                        isSuccess = true;
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }
            return isSuccess;
        }
        public bool update_CMD_DetailLeaveTime(string cmd_id,
                                              string add_id,
                                              string sec_id)
        {
            bool isSuccess = false;
            //DBConnection_EF con = DBConnection_EF.GetContext();
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //ACMD_OHTC cmd_oht = cmd_ohtcDAO.getExecuteByVhID(con, vh_id);
                    //if (cmd_oht != null)
                    //{
                    ACMD_OHTC_DETAIL cmd_detail = cmd_ohtc_detailDAO.
                        getByCmdIDSECIDAndLeaveTimeEmpty(con, cmd_id, sec_id);
                    if (cmd_detail == null)
                    {
                        return false;
                    }
                    DateTime nowTime = DateTime.Now;
                    cmd_detail.SEC_LEAVE_TIME = nowTime;

                    cmd_ohtc_detailDAO.Update(con, cmd_detail);
                    cmd_ohtc_detailDAO.UpdateIsPassFlag(con, cmd_detail.CMD_ID, cmd_detail.SEQ_NO);
                    isSuccess = true;
                    //}
                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }
            return isSuccess;
        }

        public bool update_CMD_Detail_LoadStartTime(string vh_id,
                                                   string add_id,
                                                   string sec_id)
        {
            bool isSuccess = true;
            //DBConnection_EF con = DBConnection_EF.GetContext();
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //ACMD_OHTC cmd_oht = cmd_ohtcDAO.getExecuteByVhID(con, vh_id);
                    AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(vh_id);
                    if (!SCUtility.isEmpty(vh.OHTC_CMD))
                    {
                        //ACMD_OHTC_DETAL cmd_detal = cmd_ohtc_detalDAO.getByCmdIDAndAdrID(con, cmd_oht.CMD_ID, add_id);
                        //ACMD_OHTC_DETAIL cmd_detail = cmd_ohtc_detailDAO.getByCmdIDAndSecID(con, cmd_oht.CMD_ID, sec_id);
                        ACMD_OHTC_DETAIL cmd_detail = cmd_ohtc_detailDAO.getByCmdIDAndSecID(con, vh.OHTC_CMD, sec_id);
                        if (cmd_detail == null)
                            return false;
                        DateTime nowTime = DateTime.Now;
                        cmd_detail.LOAD_START_TIME = nowTime;
                        cmd_ohtc_detailDAO.Update(con, cmd_detail);
                    }
                    else
                    {
                        isSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }

            return isSuccess;
        }
        public bool update_CMD_Detail_LoadEndTime(string vh_id,
                                         string add_id,
                                         string sec_id)
        {
            bool isSuccess = true;
            //DBConnection_EF con = DBConnection_EF.GetContext();
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //ACMD_OHTC cmd_oht = cmd_ohtcDAO.getExecuteByVhID(con, vh_id);
                    AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(vh_id);
                    if (!SCUtility.isEmpty(vh.OHTC_CMD))
                    {
                        //ACMD_OHTC_DETAL cmd_detal = cmd_ohtc_detalDAO.getByCmdIDAndAdrID(con, cmd_oht.CMD_ID, add_id);
                        //ACMD_OHTC_DETAIL cmd_detail = cmd_ohtc_detailDAO.getByCmdIDAndSecID(con, cmd_oht.CMD_ID, sec_id);
                        ACMD_OHTC_DETAIL cmd_detail = cmd_ohtc_detailDAO.getByCmdIDAndSecID(con, vh.OHTC_CMD, sec_id);
                        if (cmd_detail == null)
                            return false;
                        DateTime nowTime = DateTime.Now;
                        cmd_detail.LOAD_END_TIME = nowTime;
                        cmd_ohtc_detailDAO.Update(con, cmd_detail);
                        //}
                    }
                    else
                    {
                        isSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }

            return isSuccess;
        }
        public bool update_CMD_Detail_UnloadStartTime(string vh_id,
                                       string add_id,
                                       string sec_id)
        {
            bool isSuccess = true;
            //DBConnection_EF con = DBConnection_EF.GetContext();
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //ACMD_OHTC cmd_oht = cmd_ohtcDAO.getExecuteByVhID(con, vh_id);
                    AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(vh_id);
                    if (!SCUtility.isEmpty(vh.OHTC_CMD))
                    {
                        ACMD_OHTC_DETAIL cmd_detail = cmd_ohtc_detailDAO.getByCmdIDAndSecID(con, vh.OHTC_CMD, sec_id);
                        if (cmd_detail == null)
                            return false;
                        DateTime nowTime = DateTime.Now;
                        cmd_detail.UNLOAD_START_TIME = nowTime;
                        cmd_ohtc_detailDAO.Update(con, cmd_detail);
                    }
                    else
                    {
                        isSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }

            return isSuccess;
        }


        public bool update_CMD_Detail_UnloadEndTime(string vh_id)
        {
            bool isSuccess = true;
            //DBConnection_EF con = DBConnection_EF.GetContext();
            //using (DBConnection_EF con = new DBConnection_EF())
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //ACMD_OHTC cmd_oht = cmd_ohtcDAO.getExecuteByVhID(con, vh_id);
                    AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(vh_id);
                    if (!SCUtility.isEmpty(vh.OHTC_CMD))
                    {
                        ACMD_OHTC_DETAIL cmd_detail = cmd_ohtc_detailDAO.getLastByID(con, vh.OHTC_CMD);
                        if (cmd_detail != null)
                        {
                            cmd_detail.UNLOAD_END_TIME = DateTime.Now;
                            cmd_ohtc_detailDAO.Update(con, cmd_detail);
                        }
                        else
                        {
                            isSuccess = false;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }


            return isSuccess;
        }

        //public bool update_CMD_Detail_2AbnormalFinsh(string cmd_id, List<string> sec_ids)
        //{
        //    bool isSuccess = false;
        //    using (DBConnection_EF con = DBConnection_EF.GetUContext())
        //    {
        //        foreach (string sec_id in sec_ids)
        //        {
        //            ACMD_OHTC_DETAIL cmd_detail = new ACMD_OHTC_DETAIL();
        //            cmd_detail.CMD_ID = cmd_id;
        //            con.ACMD_OHTC_DETAIL.Attach(cmd_detail);
        //            cmd_detail.SEC_ID = sec_id;
        //            cmd_detail.SEC_ENTRY_TIME = DateTime.MaxValue;
        //            cmd_detail.SEC_LEAVE_TIME = DateTime.MaxValue;
        //            cmd_detail.ADD_ID = "";
        //            cmd_detail.SEG_NUM = "";

        //            //con.Entry(cmd_detail).Property(p => p.CMD_ID).IsModified = true;
        //            //con.Entry(cmd_detail).Property(p => p.SEC_ID).IsModified = true;
        //            con.Entry(cmd_detail).Property(p => p.SEC_ENTRY_TIME).IsModified = true;
        //            con.Entry(cmd_detail).Property(p => p.SEC_LEAVE_TIME).IsModified = true;
        //            con.Entry(cmd_detail).Property(p => p.ADD_ID).IsModified = false;
        //            con.Entry(cmd_detail).Property(p => p.SEG_NUM).IsModified = false;
        //            cmd_ohtc_detailDAO.Update(con, cmd_detail);
        //            con.Entry(cmd_detail).State = EntityState.Detached;
        //        }
        //        isSuccess = true;
        //    }
        //    return isSuccess;
        //}
        public bool update_CMD_Detail_2AbnormalFinsh(string cmd_id, List<string> sec_ids)
        {
            bool isSuccess = false;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    foreach (string sec_id in sec_ids)
                    {
                        ACMD_OHTC_DETAIL cmd_detail = cmd_ohtc_detailDAO.getByCmdIDSECIDAndEntryTimeEmpty(con, cmd_id, sec_id);
                        if (cmd_detail != null)
                        {
                            cmd_detail.SEC_ENTRY_TIME = DateTime.MaxValue;
                            cmd_detail.SEC_LEAVE_TIME = DateTime.MaxValue;
                            cmd_detail.IS_PASS = true;

                            cmd_ohtc_detailDAO.Update(con, cmd_detail);
                        }
                    }
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                isSuccess = false;
            }

            return isSuccess;
        }
        public int getAndUpdateVhCMDProgress(string vh_id, out List<string> willPassSecID)
        {
            int procProgress_percen = 0;
            willPassSecID = null;
            //DBConnection_EF con = DBConnection_EF.GetContext();
            //using (DBConnection_EF con = new DBConnection_EF())
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(vh_id);
                    if (!SCUtility.isEmpty(vh.OHTC_CMD))
                    {
                        ACMD_OHTC cmd_oht = cmd_ohtcDAO.getByID(con, vh.OHTC_CMD);
                        if (cmd_oht == null) return 0;
                        double totalDetailCount = 0;
                        double procDetailCount = 0;
                        //List<ACMD_OHTC_DETAIL> lstcmd_detail = cmd_ohtc_detailDAO.loadAllByCmdID(con, cmd_oht.CMD_ID);
                        //totalDetalCount = lstcmd_detail.Count();
                        //procDetalCount = lstcmd_detail.Where(cmd => cmd.ADD_ENTRY_TIME != null).Count();
                        totalDetailCount = cmd_ohtc_detailDAO.getAllDetailCountByCmdID(con, cmd_oht.CMD_ID);
                        procDetailCount = cmd_ohtc_detailDAO.getAllPassDetailCountByCmdID(con, cmd_oht.CMD_ID);
                        willPassSecID = cmd_ohtc_detailDAO.loadAllNonPassDetailSecIDByCmdID(con, cmd_oht.CMD_ID);
                        procProgress_percen = (int)((procDetailCount / totalDetailCount) * 100);
                        cmd_oht.CMD_PROGRESS = procProgress_percen;
                        cmd_ohtcDAO.Update(con, cmd_oht);
                    }
                }
                return procProgress_percen;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return 0;
            }

        }

        public bool HasCmdWillPassSegment(string segment_num, out List<string> will_pass_cmd_id)
        {
            bool hasCmd = false;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    will_pass_cmd_id = cmd_ohtc_detailDAO.loadAllWillPassDetailCmdID(con, segment_num);
                }
                hasCmd = will_pass_cmd_id != null && will_pass_cmd_id.Count > 0;
                return hasCmd;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                will_pass_cmd_id = null;
                return false;
            }
        }

        public string[] loadPassSectionByCMDID(string cmd_id)
        {
            string[] sections = null;
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    sections = cmd_ohtc_detailDAO.loadAllSecIDByCmdID(con, cmd_id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }

            return sections;
        }

        #endregion CMD_OHTC_DETAIL

        #region Return Code Map
        public ReturnCodeMap getReturnCodeMap(string eq_id, string return_code)
        {
            try
            {
                return return_code_mapDao.getReturnCodeMap(scApp, eq_id, return_code);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        #endregion Return Code Map

    }
}
