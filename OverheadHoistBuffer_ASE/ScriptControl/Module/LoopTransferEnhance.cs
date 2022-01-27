using com.mirle.ibg3k0.sc.BLL.Interface;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.Service.Interface;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                if (isReadyToTransfer(cmd_mcs))
                {
                    cmd_mcs.IsReadyTransfer = true;
                }
            }
        }

        private bool isReadyToTransfer(ACMD_MCS cmd_mcs)
        {
            //若不是Scna命令的話，需確認是否有帳
            bool is_agv_port_to_station_cmd = transferService.checkAndProcessIsAgvPortToStation(cmd_mcs);
            if (is_agv_port_to_station_cmd) return false;

            //確認來源是否是可以搬送狀態
            string source = cmd_mcs.HOSTSOURCE;
            if (cmd_mcs.IsRelayHappend())
            {
                source = cmd_mcs.RelayStation;
            }
            if (!cmd_mcs.IsScan())
            {
                CassetteData sourceCstData = cassetteDataBLL.loadCassetteDataByLoc(source);
                if (sourceCstData == null)
                {
                    logger.Info($"OHB >> OHB| 命令:{cmd_mcs.CMD_ID} 來源: {source} 找不到帳，刪除命令 ");
                    transferService.Manual_DeleteCmd(cmd_mcs.CMD_ID, "命令來源找不到帳");
                    return false;
                }
            }
            if (!transferService.AreSourceEnable(source))
            {
                logger.Info($"OHB >> OHB| 命令來源: {source} Port狀態不正確，不繼續往下執行。");
                return false;
            }

            //確認目的地是否是可以搬送狀態
            if (cmd_mcs.IsDestination_ShelfZone(transferService))
            {
                List<ShelfDef> shelfData = shelfDefBLL.GetEmptyAndEnableShelfByZone(cmd_mcs.HOSTDESTINATION);//Modify by Kevin
                if (shelfData == null || shelfData.Count() == 0)
                {
                    logger.Info("OHB >> OHB|TransferCommandHandler 目的 Zone: " + cmd_mcs.HOSTDESTINATION + " 沒有位置");
                    transferService.MCSCommandFinishByShelfNotEnough(cmd_mcs);
                    return false;
                }
                //else
                //{   //todo:
                //    //改到真的要下命令時，再去取得目前可以放置的儲位位置
                //    //若沒有的話再改判NotReady
                //    logger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|TransferCommandHandler 目的 Zone: " + mcsCmd.HOSTDESTINATION + " 可用儲位數量: " + shelfData.Count);

                //    string shelfID = transferService.GetShelfRecentLocation(shelfData, cmd_mcs.HOSTSOURCE);

                //    if (string.IsNullOrWhiteSpace(shelfID) == false)
                //    {
                //        logger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|TransferCommandHandler: 目的 Zone: " + mcsCmd.HOSTDESTINATION + " 找到 " + shelfID);
                //        cmd_mcs.HOSTDESTINATION = shelfID;
                //    }
                //    else
                //    {
                //        logger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|TransferCommandHandler: 目的 Zone: " + mcsCmd.HOSTDESTINATION + " 找不到可用儲位。");
                //        transferService.MCSCommandFinishByShelfNotEnough(cmd_mcs);
                //        continue;
                //    }
                //}
            }
            else if (cmd_mcs.IsDestination_AGVZone(transferService))
            {
                string agvPortName = transferService.GetAGV_OutModeInServicePortName(cmd_mcs.HOSTDESTINATION);
                if (SCUtility.isEmpty(agvPortName))
                {
                    logger.Info("OHB >> OHB|TransferCommandHandler 目的 AGV St: " + cmd_mcs.HOSTDESTINATION + " 沒有準備好可放置的位置");
                    return false;
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
                    }
                    else
                    {
                        logger.Info($"OHB >> OHB|TransferCommandHandler 目的 port: { cmd_mcs.HOSTDESTINATION }狀態尚未正確");
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 當來源狀態是好的但目的地狀態還沒有準備好時，可以用該Funtion判斷是否需要進行中繼站的搬送
        /// </summary>
        /// <param name="cmd_mcs"></param>
        /// <returns></returns>
        private bool IsNeedRealyCommand(ACMD_MCS cmd_mcs, bool isDestCVPortIsFull)
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


        const double MAX_CLOSE_DIS_MM = 10_000;
        public (bool hasCommand, string waitPort, ACMD_MCS cmdMCS) tryGetZoneCommand(List<ACMD_MCS> mcsCMDs, string vhID, string zoneCommandID)
        {
            //沒命令就不需要等待
            if (mcsCMDs == null || mcsCMDs.Count == 0) return (false, "", null);
            var zone_group = zoneCommandBLL.getZoneCommandGroup(zoneCommandID);
            List<ACMD_MCS> zone_mcs_cmds = mcsCMDs.Where(cmd => zone_group.PortIDs.Contains(sc.Common.SCUtility.Trim(cmd.HOSTSOURCE, true))).
                                                   ToList();
            if (zone_mcs_cmds == null || zone_mcs_cmds.Count == 0) return (false, "", null);
            var first_cmd = zone_mcs_cmds.FirstOrDefault();
            return (true, first_cmd.HOSTSOURCE, first_cmd);
            //確認是否有CV貨物準備要Wait 
            //var checkBoxWillWaitIn = tryGetWillWaitInPort(zone_group);
            //if (checkBoxWillWaitIn.has)
            //{

            //}
            //
            if (zone_mcs_cmds.Count == 1)
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
                    var on_sec_vhs = cycling_vhs.Where(v => sc.Common.SCUtility.isMatche(v.CUR_SEC_ID, pre_section.SEC_ID)).FirstOrDefault();
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
                return (true, cmd_mcs.HOSTSOURCE, cmd_mcs);
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
            bool is_success = false;
            //1.變更該命令的狀態
            is_success = cmdBLL.updateCMD_MCS_TranStatus(cmdMCS.CMD_ID, E_TRAN_STATUS.Transferring);

            //2.產生該命令的小命令到Queue
            var cmd_ohtc = cmdMCS.convertToACMD_OHTC(vh, portDefBLL, sequenceBLL, transferService);
            is_success = cmdBLL.creatCommand_OHTC(cmd_ohtc);

            return false;
        }
    }
}
