using com.mirle.ibg3k0.sc.BLL.Interface;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.Service.Interface;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace com.mirle.ibg3k0.sc.Module
{
    public class LoopTransferEnhance
    {
        static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        IZoneCommandBLL zoneCommandBLL = null;
        IVehicleBLL vehicleBLL = null;
        ISectionBLL sectionBLL = null;
        IPortDefBLL portDefBLL = null;
        IReserveBLL reserveBLL = null;
        ITransferService transferService = null;
        IShelfDefBLL shelfDefBLL = null;
        ICassetteDataBLL cassetteDataBLL = null;
        ICMDBLL cmdBLL = null;

        public void Start(IZoneCommandBLL zoneCommandBLL,
                          IVehicleBLL vehicleBLL,
                          ISectionBLL sectionBLL,
                          IPortDefBLL portDefBLL,
                          IReserveBLL reserveBLL,
                          ITransferService transferService,
                          IShelfDefBLL shelfDefBLL,
                          ICassetteDataBLL cassetteDataBLL,
                          ICMDBLL cmdBLL
                          )
        {
            this.zoneCommandBLL = zoneCommandBLL;
            this.vehicleBLL = vehicleBLL;
            this.sectionBLL = sectionBLL;
            this.portDefBLL = portDefBLL;
            this.reserveBLL = reserveBLL;
            this.transferService = transferService;
            this.shelfDefBLL = shelfDefBLL;
            this.cassetteDataBLL = cassetteDataBLL;
            this.cmdBLL = cmdBLL;
        }

        public void judgeCommandTransferReadyStatus(List<ACMD_MCS> cmdMCSs)
        {
            foreach (var cmd_mcs in cmdMCSs)
            {
                cmd_mcs.ReadyStatus = checkReadyToTransfer(cmd_mcs);

                //if (isReadyToTransfer(cmd_mcs))
                //{
                //    cmd_mcs.IsReadyTransfer = true;
                //}
            }
        }

        private ACMD_MCS.CommandReadyStatus checkReadyToTransfer(ACMD_MCS cmd_mcs)
        {
            try
            {
                //確認是否為AGV Port > Station的特殊命令，是的話就走特別處理流程
                bool is_agv_port_to_station_cmd = transferService.checkAndProcessIsAgvPortToStation(cmd_mcs);
                if (is_agv_port_to_station_cmd) return ACMD_MCS.CommandReadyStatus.NotReady;

                //確認來源是否是可以搬送狀態
                string source = cmd_mcs.CURRENT_LOCATION;

                if (!cmd_mcs.IsScan())//因為如果是Scan命令的話，可能會找不到來源的帳
                {
                    CassetteData sourceCstData = cassetteDataBLL.loadCassetteDataByLoc(source);
                    if (sourceCstData == null)
                    {
                        logger.Info($"OHB >> OHB| 命令:{cmd_mcs.CMD_ID} 來源: {source} 找不到帳，刪除命令 ");
                        transferService.Manual_DeleteCmd(cmd_mcs.CMD_ID, "命令來源找不到帳");
                        return ACMD_MCS.CommandReadyStatus.NotReady;
                    }
                }
                if (!transferService.AreSourceEnable(source))
                {
                    logger.Info($"OHB >> OHB| 命令來源: {source} Port狀態不正確，不繼續往下執行。");
                    return ACMD_MCS.CommandReadyStatus.NotReady;
                }

                //確認目的地是否是可以搬送狀態
                if (cmd_mcs.IsDestination_ShelfZone(transferService))
                {
                    int shelf_count = shelfDefBLL.GetEmptyAndEnableShelfCountByZone(cmd_mcs.HOSTDESTINATION);//Modify by Kevin
                    if (shelf_count == 0)
                    {
                        logger.Info("OHB >> OHB|TransferCommandHandler 目的 Zone: " + cmd_mcs.HOSTDESTINATION + " 沒有位置");
                        transferService.MCSCommandFinishByShelfNotEnough(cmd_mcs);
                        return ACMD_MCS.CommandReadyStatus.NotReady;
                    }
                }
                else if (cmd_mcs.IsDestination_AGVZone(transferService))
                {
                    string agvPortName = transferService.GetAGV_OutModeInServicePortName(cmd_mcs.HOSTDESTINATION);
                    if (SCUtility.isEmpty(agvPortName))
                    {
                        logger.Info("OHB >> OHB|TransferCommandHandler 目的 AGV St: " + cmd_mcs.HOSTDESTINATION + " 沒有準備好可放置的位置");
                        if (IsNeedRealyCommand(cmd_mcs))
                        {
                            return ACMD_MCS.CommandReadyStatus.Realy;
                        }
                        else
                        {
                            return ACMD_MCS.CommandReadyStatus.NotReady;
                        }
                    }
                    else
                    {
                        logger.Info($"OHB >> OHB|TransferCommandHandler 目的 AGV St: {cmd_mcs.HOSTDESTINATION } 選取到Out put port:{agvPortName}");
                    }
                }
                else
                {
                    bool is_dest_ready = transferService.AreDestEnable(cmd_mcs.HOSTDESTINATION, out bool dest_cv_port_is_full);
                    if (!is_dest_ready)
                    {
                        bool is_need_excute_relay_command = IsNeedRealyCommand(cmd_mcs, dest_cv_port_is_full);
                        if (is_need_excute_relay_command)
                        {
                            logger.Info($"OHB >> OHB|TransferCommandHandler 目的 port: { cmd_mcs.HOSTDESTINATION }狀態尚未正確,準備執行中繼站流程搬送");
                            return ACMD_MCS.CommandReadyStatus.Realy;
                        }
                        else
                        {
                            logger.Info($"OHB >> OHB|TransferCommandHandler 目的 port: { cmd_mcs.HOSTDESTINATION }狀態尚未正確");
                            return ACMD_MCS.CommandReadyStatus.NotReady;
                        }
                    }
                }
                return ACMD_MCS.CommandReadyStatus.Ready;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                return ACMD_MCS.CommandReadyStatus.NotReady;
            }
        }
        /// <summary>
        /// 當來源狀態是好的但目的地狀態還沒有準備好時，可以用該Funtion判斷是否需要進行中繼站的搬送
        /// </summary>
        /// <param name="cmd_mcs"></param>
        /// <returns></returns>
        private bool IsNeedRealyCommand(ACMD_MCS cmd_mcs, bool isDestCVPortIsFull = false)
        {
            if (cmd_mcs.IsSource_ShelfPort(transferService))
            {
                return false;
            }
            if (!cmd_mcs.IsDestination_CVPort(transferService))
            {
                return false;
            }
            if (!cmd_mcs.IsTimeOutToAlternate())
            {
                return false;
            }

            //來源地是否有準備好搬送
            if (cmd_mcs.IsSource_CRANE(transferService))
            {
                AVEHICLE vh = vehicleBLL.getVehicle(cmd_mcs.HOSTSOURCE);
                if (!vh.TransferReady(cmdBLL, true))
                {
                    return false;
                }
            }
            else
            {
                //PortPLCInfo source_port_info = portBLL.getPortPLCInfo(cmd_mcs.HOSTSOURCE);
                PortPLCInfo source_port_info = transferService.GetPLC_PortData(cmd_mcs.HOSTSOURCE);
                if (!source_port_info.OpAutoMode)
                {
                    return false;
                }
                if (!source_port_info.IsReadyToUnload)
                {
                    return false;
                }
            }

            if (cmd_mcs.IsDestination_AGVZone(transferService))
            {
                return true;
            }
            else
            {
                //確認目的地是否符合要進行中繼站搬送的條件
                PortPLCInfo dest_port_info = transferService.GetPLC_PortData(cmd_mcs.HOSTDESTINATION);
                if (dest_port_info.OpAutoMode == false ||
                    dest_port_info.IsReadyToLoad == false ||
                    isDestCVPortIsFull)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        const double MAX_CLOSE_DIS_MM = 10_000;
        public (bool hasCommand, string waitPort, ACMD_MCS cmdMCS) tryGetZoneCommand(List<ACMD_MCS> mcsCMDs, string vhID, string zoneCommandID, bool isNeedCheckHasVhClose = true)
        {
            //沒命令就不需要等待
            if (mcsCMDs == null || mcsCMDs.Count == 0) return (false, "", null);
            var zone_group = zoneCommandBLL.getZoneCommandGroup(zoneCommandID);
            List<ACMD_MCS> zone_mcs_cmds = mcsCMDs.Where(cmd => zone_group.PortIDs.Contains(sc.Common.SCUtility.Trim(cmd.CURRENT_LOCATION, true))).
                                                   ToList();
            if (zone_mcs_cmds == null || zone_mcs_cmds.Count == 0) return (false, "", null);
            //var first_cmd = zone_mcs_cmds.FirstOrDefault();
            //return (true, first_cmd.HOSTSOURCE, first_cmd);
            //確認是否有CV貨物準備要Wait 
            //var checkBoxWillWaitIn = tryGetWillWaitInPort(zone_group);
            //if (checkBoxWillWaitIn.has)
            //{

            //}
            //
            if (isNeedCheckHasVhClose && zone_mcs_cmds.Count == 1)
            {
                //1筆
                //	判斷後面是否有空車在距離內
                //		在10m以內
                //			bypass此筆命令
                //		在10m以外
                //			36回有命令
                AVEHICLE vh = vehicleBLL.getVehicle(vhID);
                string ask_vh_sec_id = vh.CUR_SEC_ID;
                var ask_vh_sec_obj = sectionBLL.getSection(ask_vh_sec_id);
                //a.確認同一段Section是否有在後面的車子，有的話代表已經靠近中了
                List<AVEHICLE> cycling_vhs = vehicleBLL.loadCyclingVhs();
                ACMD_MCS cmd_mcs = zone_mcs_cmds.FirstOrDefault();
                foreach (var v in cycling_vhs)
                {
                    if (sc.Common.SCUtility.isMatche(vh.CUR_SEC_ID, v.CUR_SEC_ID))
                    {
                        if (vh.ACC_SEC_DIST > v.ACC_SEC_DIST)
                        {
                            return (false, "", null);
                        }
                    }
                }
                //b.確認往後找10m內有沒有車是在單純移動中
                string ask_vh_sec_from_adr = ask_vh_sec_obj.FROM_ADR_ID;
                double check_dis = 0;
                do
                {
                    ASECTION pre_section = sectionBLL.getSectionByToAdr(ask_vh_sec_from_adr);
                    var on_sec_vhs = cycling_vhs.Where(v => v != vh && sc.Common.SCUtility.isMatche(v.CUR_SEC_ID, pre_section.SEC_ID)).FirstOrDefault();
                    if (on_sec_vhs != null)
                    {
                        return (false, "", null);
                    }
                    check_dis += pre_section.SEC_DIS;
                    ask_vh_sec_from_adr = pre_section.FROM_ADR_ID;
                }
                while (check_dis < MAX_CLOSE_DIS_MM);
                return (true, cmd_mcs.HOSTSOURCE, cmd_mcs);
            }
            else
            {
                //如果有多筆命令在這個Zone，則需要找出最遠的一筆命令
                //讓後面車子來的時候，可以接另外一筆
                var cmd_mcs = getZoneCommandFarthestCommand(zone_group.zoneDir, zone_mcs_cmds);
                return (true, cmd_mcs.CURRENT_LOCATION, cmd_mcs);
            }
        }

        private ACMD_MCS getZoneCommandFarthestCommand(ZoneCommandGroup.ZoneDir zoneDir, List<ACMD_MCS> zone_mcs_cmds)
        {
            if (zoneDir == ZoneCommandGroup.ZoneDir.DIR_1_0)
            {
                //要找出X最大，即為該Zone最遠的Port
                zone_mcs_cmds = zone_mcs_cmds.OrderBy(cmd => cmd.getHostSourceAxis_X(portDefBLL, reserveBLL)).ToList();
            }
            else if (zoneDir == ZoneCommandGroup.ZoneDir.DIR_N1_0)
            {
                //要找出X最小，即為該Zone最遠的Port
                zone_mcs_cmds = zone_mcs_cmds.OrderByDescending(cmd => cmd.getHostSourceAxis_X(portDefBLL, reserveBLL)).ToList();
            }
            return zone_mcs_cmds.Last();
        }

        public bool preAssignMCSCommand(ISequenceBLL sequenceBLL, AVEHICLE vh, ACMD_MCS cmdMCS)
        {
            try
            {
                bool is_success = false;
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    using (TransactionScope tx = SCUtility.getTransactionScope())
                    {

                        //1.變更該命令的狀態
                        is_success = cmdBLL.updateCMD_MCS_TranStatus(cmdMCS.CMD_ID, E_TRAN_STATUS.Transferring);
                        if (!is_success)
                        {
                            logger.Info($"OHB >> OHB|cmd id:{cmdMCS.CMD_ID} 更新至Tranferring失敗");
                            return false;
                        }

                        string original_host_dest = SCUtility.Trim(cmdMCS.HOSTDESTINATION, true);
                        if (cmdMCS.ReadyStatus == ACMD_MCS.CommandReadyStatus.Realy)
                        {
                            //1.找出一個最接近目的地的儲位
                            //2.更新中繼站的欄位
                            List<ShelfDef> shelfData = shelfDefBLL.GetEmptyAndEnableShelf();
                            string empty_shelf = transferService.GetShelfRecentLocation(shelfData, cmdMCS.HOSTDESTINATION);
                            if (!SCUtility.isEmpty(empty_shelf))
                            {
                                logger.Info($"OHB >> OHB|cmd id:{cmdMCS.CMD_ID} 找到中繼站的儲位:{empty_shelf}");
                                cmdMCS.RelayStation = empty_shelf;
                                cmdBLL.updateCMD_MCS_RelayStation(cmdMCS.CMD_ID, empty_shelf);
                            }
                            else
                            {
                                logger.Info($"OHB >> OHB|cmd id:{cmdMCS.CMD_ID} 沒有找到中繼站的儲位");
                                return false;
                            }

                        }
                        else
                        {
                            //1.a-若目的地為儲位或是Station，則需要重新判斷目的地哪個位子可以放置
                            if (cmdMCS.IsDestination_ShelfZone(transferService))
                            {
                                List<ShelfDef> shelfData = shelfDefBLL.GetEmptyAndEnableShelfByZone(cmdMCS.HOSTDESTINATION);

                                if (shelfData == null || shelfData.Count() == 0)
                                {
                                    logger.Info($"OHB >> OHB|TransferCommandHandler 目的 Zone:{ cmdMCS.HOSTDESTINATION } 沒有位置");
                                    return false;
                                }
                                else
                                {
                                    logger.Info($"OHB >> OHB|TransferCommandHandler 目的 Zone: { cmdMCS.HOSTDESTINATION } 可用儲位數量: { shelfData.Count}");

                                    string shelfID = transferService.GetShelfRecentLocation(shelfData, cmdMCS.HOSTSOURCE);
                                    bool is_find_shelf = !SCUtility.isEmpty(shelfID);
                                    if (is_find_shelf)
                                    {
                                        logger.Info($"OHB >> OHB|TransferCommandHandler: 目的 Zone: {cmdMCS.HOSTDESTINATION }找到 { shelfID}");
                                        cmdMCS.HOSTDESTINATION = shelfID;
                                    }
                                    else
                                    {
                                        logger.Info($"OHB >> OHB|TransferCommandHandler: 目的 Zone: { cmdMCS.HOSTDESTINATION } 找不到可用儲位。");
                                        return false;
                                    }
                                }
                            }
                            else if (cmdMCS.IsDestination_AGVZone(transferService))
                            {
                                string agvPortName = transferService.GetAGV_OutModeInServicePortName(cmdMCS.HOSTDESTINATION);
                                if (string.IsNullOrWhiteSpace(agvPortName))
                                {
                                    logger.Info($"OHB >> OHB|TransferCommandHandler: 目的AGV Zone: { cmdMCS.HOSTDESTINATION } 找不到放置的Port。");
                                    return false;
                                }
                                else
                                {
                                    cmdMCS.HOSTDESTINATION = agvPortName;
                                }
                            }
                        }
                        //2.產生該命令的小命令到Queue
                        var cmd_ohtc = cmdMCS.convertToACMD_OHTC(vh, portDefBLL, sequenceBLL, transferService);
                        is_success = is_success && cmdBLL.creatCommand_OHTC(cmd_ohtc);
                        if (!SCUtility.isMatche(original_host_dest, cmdMCS.HOSTDESTINATION))
                        {
                            is_success = is_success && cmdBLL.updateCMD_MCS_Dest(cmdMCS.CMD_ID, cmdMCS.HOSTDESTINATION);
                        }
                        if (is_success)
                        {
                            tx.Complete();
                            cmdMCS.TRANSFERSTATE = E_TRAN_STATUS.Transferring;
                            if (transferService.isCVPort(cmdMCS.HOSTDESTINATION))
                            {
                                transferService.PortCommanding(cmdMCS.HOSTDESTINATION, true);
                            }
                        }
                    }
                }
                return is_success;
            }
            catch (Exception e)
            {
                logger.Error(e, "Exception");
                return false;
            }
        }
        const double IGNORE_RUN_OVER_DISTANCE_mm = 1000;
        public bool IsRunOver(AVEHICLE vh, string portID)
        {
            var get_result = zoneCommandBLL.tryGetZoneCommandGroupByPortID(portID);
            if (!get_result.hasFind)
            {
                logger.Info($"OHB >> OHB|確認 vh:{vh.VEHICLE_ID} 是否跑過頭 port id:{portID},但沒有找到相應的Port Group.");
                return false;
            }
            var port_def_info = portDefBLL.getPortDef(portID);
            var port_adr_obj = reserveBLL.GetHltMapAddress(port_def_info.ADR_ID);
            double port_x_axis = port_adr_obj.x;
            double port_y_axis = port_adr_obj.y;
            double vh_x_axis = vh.X_Axis;
            double vh_y_axis = vh.Y_Axis;
            switch (get_result.zoneCommandGroup.zoneDir)
            {
                case ZoneCommandGroup.ZoneDir.DIR_1_0:
                    if (port_y_axis != vh_y_axis)
                    {
                        return false;
                    }


                    if (port_x_axis + IGNORE_RUN_OVER_DISTANCE_mm > vh_x_axis) return false;
                    else
                    {
                        logger.Info($"OHB >> OHB|確認 vh:{vh.VEHICLE_ID}(x:{vh_x_axis}) 相對於 port id:{portID}(x:{port_x_axis})(包含容許範圍:{IGNORE_RUN_OVER_DISTANCE_mm})  dir:{get_result.zoneCommandGroup.zoneDir},跑過頭了.");
                        return true;
                    }
                case ZoneCommandGroup.ZoneDir.DIR_N1_0:
                    if (port_y_axis != vh_y_axis)
                    {
                        return false;
                    }
                    if (port_x_axis - IGNORE_RUN_OVER_DISTANCE_mm < vh_x_axis) return false;
                    else
                    {
                        logger.Info($"OHB >> OHB|確認 vh:{vh.VEHICLE_ID}(x:{vh_x_axis}) 相對於 port id:{portID}(x:{port_x_axis})(包含容許範圍:{IGNORE_RUN_OVER_DISTANCE_mm}) dir:{get_result.zoneCommandGroup.zoneDir},跑過頭了.");
                        return true;
                    }
                default:
                    return false;
            }
        }

        public (bool hasCommand, string waitPort, ACMD_MCS cmdMCS) tryGetZoneCommandWhenCommandComplete(List<ACMD_MCS> mcsCMDs, string vhID)
        {
            var vh = vehicleBLL.getVehicle(vhID);
            string current_adr_id = vh.CUR_ADR_ID;
            var port = portDefBLL.getPortDefByAdrID(current_adr_id);
            if (port == null)
            {
                return (false, "", null);
            }
            var get_result = zoneCommandBLL.tryGetZoneCommandGroupByPortID(port.PLCPortID);
            if (!get_result.hasFind)
            {
                logger.Info($"OHB >> OHB|確認 vh:{vh.VEHICLE_ID}命令完成後是否有同Zone的命令可以搬送，但Port:{port.PLCPortID}並無對應的ZoneCommand.");
                return (false, "", null);
            }
            var zone_mcs_cmds = mcsCMDs.Where(cmd => get_result.zoneCommandGroup.PortIDs.Contains(sc.Common.SCUtility.Trim(cmd.CURRENT_LOCATION, true)));
            if (zone_mcs_cmds == null || zone_mcs_cmds.Count() == 0)
            {
                return (false, "", null);
            }
            bool is_need_check_has_vh_close = zone_mcs_cmds.Count() == 1;
            //找出還沒跑過頭的命令
            zone_mcs_cmds = zone_mcs_cmds.Where(mcs_cmd => !IsRunOver(vh, mcs_cmd.CURRENT_LOCATION));
            var try_get_result = tryGetZoneCommand
                (zone_mcs_cmds.ToList(), vh.VEHICLE_ID, get_result.zoneCommandGroup.ZoneCommandID, is_need_check_has_vh_close);
            return try_get_result;
        }
    }
}
