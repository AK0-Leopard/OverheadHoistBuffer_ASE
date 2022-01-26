using com.mirle.ibg3k0.sc.BLL.Interface;
using com.mirle.ibg3k0.sc.Data.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Module
{
    public class LoopTransferEnhance
    {
        IPortBLL portBLL = null;
        IZoneCommandBLL zoneCommandBLL = null;
        IVehicleBLL vehicleBLL = null;
        ISectionBLL sectionBLL = null;
        IPortDefBLL portDefBLL = null;
        IReserveBLL reserveBLL = null;

        public void Start(IPortBLL portBLL,
                          IZoneCommandBLL zoneCommandBLL,
                          IVehicleBLL vehicleBLL,
                          ISectionBLL sectionBLL,
                          IPortDefBLL portDefBLL,
                          IReserveBLL reserveBLL)
        {
            this.portBLL = portBLL;
            this.zoneCommandBLL = zoneCommandBLL;
            this.vehicleBLL = vehicleBLL;
            this.sectionBLL = sectionBLL;
            this.portDefBLL = portDefBLL;
            this.reserveBLL = reserveBLL;
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

        private (bool has, List<string> portIDs) tryGetWillWaitInPort(ZoneCommandGroup zoneGroup)
        {
            var cv_port_ids = zoneGroup.PortIDs.Where(id => portBLL.isUnitType(id, Service.UnitType.OHCV)).ToList();
            foreach (var cv_port_id in cv_port_ids.ToArray())
            {
                var port_plc_info = portBLL.getPortPLCInfo(cv_port_id);
                if (port_plc_info.IsInputMode)
                {
                    var has_box_will_wait_in = port_plc_info.hasBoxInPosition();
                    if (!has_box_will_wait_in.hasBox)
                    {
                        cv_port_ids.Remove(cv_port_id);
                    }
                }
            }
            bool has = cv_port_ids.Count > 0;

            return (has, cv_port_ids);
        }


        //private (bool has, AVEHICLE vh) hasVhClosing(AVEHICLE vh)
        //{

        //}
    }
}
