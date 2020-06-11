// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="BCSystemStatusTimer.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// 2020/04/17    Jason Wu       N/A            A0.01   加入NTB Type 選項與處理內容
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Data.TimerAction;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.DAO;
using com.mirle.ibg3k0.sc.Data.SECS;
using NLog;

namespace com.mirle.ibg3k0.sc.Data.TimerAction
{
    /// <summary>
    /// Class BCSystemStatusTimer.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.TimerAction.ITimerAction" />
    public class RandomGeneratesCommandTimerAction : ITimerAction
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// The sc application
        /// </summary>
        protected SCApplication scApp = null;
        List<ShelfDef> shelfDefs = null;
        List<APORT> AllTestAGVStationPorts = null;
        List<APORT> willTestAGVStationPorts = null;

        Random rnd_Index = new Random(Guid.NewGuid().GetHashCode());

        /// <summary>
        /// Initializes a new instance of the <see cref="BCSystemStatusTimer"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="intervalMilliSec">The interval milli sec.</param>
        public RandomGeneratesCommandTimerAction(string name, long intervalMilliSec)
            : base(name, intervalMilliSec)
        {

        }
        /// <summary>
        /// Initializes the start.
        /// </summary>
        public override void initStart()
        {
            scApp = SCApplication.getInstance();
            AllTestAGVStationPorts = scApp.PortBLL.OperateCatch.loadAGVStationPorts();
        }
        /// <summary>
        /// Timer Action的執行動作
        /// </summary>
        /// <param name="obj">The object.</param>
        private long syncPoint = 0;
        public override void doProcess(object obj)
        {

            if (System.Threading.Interlocked.Exchange(ref syncPoint, 1) == 0)
            {
                try
                {
                    if (!DebugParameter.CanAutoRandomGeneratesCommand)
                    {
                        shelfDefs = null;
                        willTestAGVStationPorts = null;
                        return;
                    }

                    switch (DebugParameter.cycleRunType)
                    {
                        case DebugParameter.CycleRunType.shelf:
                            ShelfTest();
                            break;
                        case DebugParameter.CycleRunType.AGVStation:
                            AGVStationTest();
                            break;
                        case DebugParameter.CycleRunType.CV:
                            CVPortTest();
                            break;
                        case DebugParameter.CycleRunType.NTB:
                            NTBPortTest(); // A0.01
                            break;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref syncPoint, 0);
                }
            }
        }

        private void ShelfTest()
        {
            List<AVEHICLE> vhs = scApp.VehicleBLL.cache.loadVhs();
            foreach (AVEHICLE vh in vhs)
            {
                if (vh.isTcpIpConnect &&
                    vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoRemote &&
                    vh.ACT_STATUS == ProtocolFormat.OHTMessage.VHActionStatus.NoCommand &&
                    !SCUtility.isEmpty(vh.CUR_ADR_ID) &&
                    !scApp.CMDBLL.isCMD_OHTCExcuteByVh(vh.VEHICLE_ID))
                {
                    List<CassetteData> cassetteDatas = scApp.CassetteDataBLL.loadCassetteData();
                    if (cassetteDatas == null || cassetteDatas.Count() == 0) return;
                    //找一份目前儲位的列表
                    if (shelfDefs == null || shelfDefs.Count == 0)
                        shelfDefs = scApp.ShelfDefBLL.LoadEnableShelf();
                    //如果取完還是空的 就跳出去
                    if (shelfDefs == null || shelfDefs.Count == 0)
                        return;
                    //取得目前當前在線內的Carrier
                    //找出在儲位中的Cassette
                    cassetteDatas = cassetteDatas.Where(cst => cst.Carrier_LOC.StartsWith("10") ||
                                                               cst.Carrier_LOC.StartsWith("11") ||
                                                               cst.Carrier_LOC.StartsWith("21") ||
                                                               cst.Carrier_LOC.StartsWith("20")).
                                                               ToList();
                    List<string> current_cst_at_shelf_id = cassetteDatas.
                        Select(cst => SCUtility.Trim(cst.Carrier_LOC, true)).
                        ToList();
                    //刪除目前cst所在的儲位，讓他排除在Cycle Run的列表中
                    foreach (var shelf in shelfDefs.ToList())
                    {
                        if (current_cst_at_shelf_id.Contains(SCUtility.Trim(shelf.ShelfID)))
                        {
                            shelfDefs.Remove(shelf);
                        }
                    }

                    //隨機找出一個要放置的shelf
                    CassetteData process_cst = cassetteDatas[0];
                    int task_RandomIndex = rnd_Index.Next(shelfDefs.Count - 1);
                    ShelfDef target_shelf_def = shelfDefs[task_RandomIndex];
                    scApp.MapBLL.getAddressID(process_cst.Carrier_LOC, out string from_adr);
                    bool isSuccess = true;
                    isSuccess &= scApp.CMDBLL.doCreatTransferCommand(vh.VEHICLE_ID, "", process_cst.CSTID.Trim(),
                                        E_CMD_TYPE.LoadUnload,
                                        process_cst.Carrier_LOC,
                                        target_shelf_def.ShelfID, 0, 0,
                                        process_cst.BOXID.Trim(), process_cst.LotID,
                                        from_adr, target_shelf_def.ADR_ID);
                    shelfDefs.Remove(target_shelf_def);
                }
            }
        }

        private void AGVStationTest()
        {
            List<AVEHICLE> vhs = scApp.VehicleBLL.cache.loadVhs();
            foreach (AVEHICLE vh in vhs)
            {
                if (vh.isTcpIpConnect &&
                    vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoRemote &&
                    vh.ACT_STATUS == ProtocolFormat.OHTMessage.VHActionStatus.NoCommand &&
                    !SCUtility.isEmpty(vh.CUR_ADR_ID) &&
                    !scApp.CMDBLL.isCMD_OHTCExcuteByVh(vh.VEHICLE_ID))
                {

                    //找出目前的AGVStation Port
                    if (willTestAGVStationPorts == null || willTestAGVStationPorts.Count == 0)
                    {
                        willTestAGVStationPorts = scApp.PortBLL.OperateCatch.loadAGVStationPorts();
                        willTestAGVStationPorts = willTestAGVStationPorts.Where(port => scApp.TransferService.GetPLC_PortData(port.PORT_ID).IsReadyToLoad &&
                                                                                        scApp.TransferService.GetPLC_PortData(port.PORT_ID).IsAutoMode).
                                                                          ToList();
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(RandomGeneratesCommandTimerAction), Device: "OHTC",
                                 Data: $"Load ok:{string.Join(",", willTestAGVStationPorts.Select(port => port.PORT_ID).ToList())}");
                    }
                    //如果取完還是空的 就跳出去
                    if (willTestAGVStationPorts == null || willTestAGVStationPorts.Count == 0)
                    {
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(RandomGeneratesCommandTimerAction), Device: "OHTC",
                                 Data: $"no agv station list.");
                        return;
                    }


                    //找出目前Unload Ok的AGV Station
                    var unload_ok_port = AllTestAGVStationPorts.Where(port => scApp.TransferService.GetPLC_PortData(port.PORT_ID).IsReadyToUnload &&
                                                                              scApp.TransferService.GetPLC_PortData(port.PORT_ID).IsAutoMode).
                                                                FirstOrDefault();
                    //var unload_ok_port = AllTestAGVStationPorts.FirstOrDefault();
                    if (unload_ok_port == null)
                    {
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(RandomGeneratesCommandTimerAction), Device: "OHTC",
                                 Data: $"no unload ok of agv station, can't execute cycle run test.");
                        return;
                    }
                    willTestAGVStationPorts.Remove(unload_ok_port);
                    //找出目前load Ok的AGV Station
                    var load_ok_ports = willTestAGVStationPorts.Where(port => scApp.TransferService.GetPLC_PortData(port.PORT_ID).IsReadyToLoad &&
                                                                              scApp.TransferService.GetPLC_PortData(port.PORT_ID).IsAutoMode).
                                                                ToList();
                    if (load_ok_ports == null || load_ok_ports.Count == 0)
                    {
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(RandomGeneratesCommandTimerAction), Device: "OHTC",
                                 Data: $"no load ok of agv station, can't execute cycle run test.");
                        return;
                    }
                    //隨機找出一個要放置的port
                    var source_port_info = scApp.TransferService.GetPLC_PortData(unload_ok_port.PORT_ID);
                    string box_id = SCUtility.isEmpty(source_port_info.BoxID) ? "BOX01" : SCUtility.Trim(source_port_info.BoxID);
                    string cst_id = source_port_info.CassetteID.ToUpper().Contains("NO") ? "" : SCUtility.Trim(source_port_info.CassetteID);
                    int task_RandomIndex = rnd_Index.Next(load_ok_ports.Count - 1);
                    var target_port_def = load_ok_ports[task_RandomIndex];
                    bool isSuccess = true;
                    scApp.MapBLL.getAddressID(unload_ok_port.PORT_ID, out string from_adr);
                    scApp.MapBLL.getAddressID(target_port_def.PORT_ID, out string to_adr);

                    isSuccess &= scApp.CMDBLL.doCreatTransferCommand(vh.VEHICLE_ID, "", cst_id,
                                        E_CMD_TYPE.LoadUnload,
                                        unload_ok_port.PORT_ID,
                                        target_port_def.PORT_ID, 0, 0,
                                        box_id, "",
                                        from_adr, to_adr);
                    willTestAGVStationPorts.Remove(target_port_def);
                }
            }
        }
        private void CVPortTest()
        {
            List<AVEHICLE> vhs = scApp.VehicleBLL.cache.loadVhs();
            foreach (AVEHICLE vh in vhs)
            {
                if (vh.isTcpIpConnect &&
                    vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoRemote &&
                    vh.ACT_STATUS == ProtocolFormat.OHTMessage.VHActionStatus.NoCommand &&
                    vh.HAS_BOX == 0 &&
                    !SCUtility.isEmpty(vh.CUR_ADR_ID) &&
                    !scApp.CMDBLL.isCMD_OHTCExcuteByVh(vh.VEHICLE_ID))
                {
                    var all_cv_port = scApp.PortDefBLL.cache.loadCVPortDefs();
                    //1.嘗試找出目前是in mode
                    var all_cv_port_in_mode = all_cv_port.Where(port => IsGetReady(port)).ToList();
                    foreach (var in_mode_port in all_cv_port_in_mode)
                    {
                        var source_port_info = scApp.TransferService.GetPLC_PortData(in_mode_port.PLCPortID);
                        //if (scApp.CMDBLL.hasExcuteCMDFromAdr(in_mode_port.ADR_ID)) continue;
                        if (scApp.CMDBLL.hasExcuteCMDByBoxID(source_port_info.BoxID)) continue;
                        string finial_port_num = in_mode_port.PLCPortID.Substring((in_mode_port.PLCPortID.Length) - 2);//取得倒數兩個字
                        int port_num = Convert.ToInt32(finial_port_num, 16);
                        //確認是否可被2整除
                        bool is_even_num = port_num % 2 == 0;
                        int target_port_num = is_even_num ? port_num - 1 : port_num + 1;
                        string target_port_id = $"B7_OHBLOOP_T{Convert.ToString(target_port_num, 16).PadLeft(2, '0')}";

                        PortDef target_port = scApp.PortDefBLL.cache.getCVPortDef(target_port_id);
                        string box_id = SCUtility.isEmpty(source_port_info.BoxID) ? "BOX01" : SCUtility.Trim(source_port_info.BoxID);
                        string cst_id = source_port_info.CassetteID.ToUpper().Contains("NO") ? "" : SCUtility.Trim(source_port_info.CassetteID);
                        bool is_success = scApp.CMDBLL.doCreatTransferCommand(vh.VEHICLE_ID, "", cst_id,
                                             E_CMD_TYPE.LoadUnload,
                                             in_mode_port.PLCPortID,
                                             target_port.PLCPortID, 0, 0,
                                             box_id, "",
                                             in_mode_port.ADR_ID, target_port.ADR_ID);
                        if (is_success)
                        {
                            return;
                        }
                    }
                }
            }
        }
        private bool IsGetReady(PortDef port)
        {
            var transfer_service = scApp.TransferService;
            var plc_port_info = transfer_service.GetPLC_PortData(port.PLCPortID);
            if (plc_port_info == null)
            {
                return false;
            }
            else
            {
                return plc_port_info.IsInputMode &&
                       plc_port_info.PortWaitIn &&
                       plc_port_info.IsAutoMode &&
                       !SCUtility.isEmpty(plc_port_info.BoxID);
            }
        }
        private bool IsGetReady_outMode(PortDef port)
        {
            var transfer_service = scApp.TransferService;
            var plc_port_info = transfer_service.GetPLC_PortData(port.PLCPortID);
            if (plc_port_info == null)
            {
                return false;
            }
            else
            {
                return plc_port_info.IsOutputMode &&
                       plc_port_info.IsAutoMode &&
                       !SCUtility.isEmpty(plc_port_info.BoxID);
            }
        }

        private void NTBPortTest() //A0.01 
        {
            //確認卡夾在哪些位置上
            List<CassetteData> cassetteDatas = scApp.CassetteDataBLL.loadCassetteData();
            //NTB算是CVport 的一種
            List<AVEHICLE> vhs = scApp.VehicleBLL.cache.loadVhs();
            //是否有要執行且可執行之 NTB to Shelf命令
            foreach (AVEHICLE vh in vhs)
            {
                if (vh.isTcpIpConnect &&
                    vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoRemote &&
                    vh.ACT_STATUS == ProtocolFormat.OHTMessage.VHActionStatus.NoCommand &&
                    vh.HAS_BOX == 0 &&
                    !SCUtility.isEmpty(vh.CUR_ADR_ID) &&
                    !scApp.CMDBLL.isCMD_OHTCExcuteByVh(vh.VEHICLE_ID))
                {
                    GenerateNtbToShelf(vh, cassetteDatas);
                }
            }
            //是否有要執行且可執行之 Shelf to NTB命令
            foreach (AVEHICLE vh in vhs)
            {
                if (vh.isTcpIpConnect &&
                    vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoRemote &&
                    vh.ACT_STATUS == ProtocolFormat.OHTMessage.VHActionStatus.NoCommand &&
                    vh.HAS_BOX == 0 &&
                    !SCUtility.isEmpty(vh.CUR_ADR_ID) &&
                    !scApp.CMDBLL.isCMD_OHTCExcuteByVh(vh.VEHICLE_ID))
                {
                    GenerateShelfToNtb(vh, cassetteDatas);
                }
            }
        }

        private void GenerateNtbToShelf(AVEHICLE vh, List<CassetteData> cassetteDatas)//A0.01 
        {
            //1.找出所有CVport
            var all_cv_port = scApp.PortDefBLL.cache.loadCVPortDefs();
            //2.嘗試找出在CVport 中目前是in mode 的所有port
            var all_cv_port_in_mode = all_cv_port.Where(port => IsGetReady(port)).ToList();
            foreach (var in_mode_port in all_cv_port_in_mode)
            {
                //3. 若開頭為OHBLOOP的是連接LOOP的CV 而非NTB 故排除
                var source_port_info = scApp.TransferService.GetPLC_PortData(in_mode_port.PLCPortID);
                if (in_mode_port.PLCPortID.StartsWith("B7_OHBLOOP"))
                {
                    continue;
                }
                else
                {
                    //找一份目前儲位的列表
                    if (shelfDefs == null || shelfDefs.Count == 0)
                        shelfDefs = scApp.ShelfDefBLL.LoadEnableShelf();
                    //如果取完還是空的 就跳出去
                    if (shelfDefs == null || shelfDefs.Count == 0)
                        return;
                    //選出要放的shelf位置
                    ShelfDef target_shelf_def = FindRandomEmptyShelf(cassetteDatas);
                    bool isSuccess = true;
                    string box_id = SCUtility.isEmpty(source_port_info.BoxID) ? "BOX01" : SCUtility.Trim(source_port_info.BoxID);
                    string cst_id = source_port_info.CassetteID.ToUpper().Contains("NO") ? "" : SCUtility.Trim(source_port_info.CassetteID);
                    //從該選取的in mode plcPort 搬到要放置的shelf
                    isSuccess &= scApp.CMDBLL.doCreatTransferCommand(vh.VEHICLE_ID, "", cst_id,
                                        E_CMD_TYPE.LoadUnload,
                                        in_mode_port.PLCPortID,
                                        target_shelf_def.ShelfID, 0, 0,
                                        box_id, "",
                                        in_mode_port.ADR_ID, target_shelf_def.ADR_ID);
                    shelfDefs.Remove(target_shelf_def);
                }
            }
        }

        private void GenerateShelfToNtb(AVEHICLE vh, List<CassetteData> cassetteDatas)//A0.01 
        {
            //1.找出所有CVport
            var all_cv_port = scApp.PortDefBLL.cache.loadCVPortDefs();
            //2.嘗試找出在CVport 中目前是out mode 的所有port
            var all_cv_port_out_mode = all_cv_port.Where(port => IsGetReady_outMode(port)).ToList();
            foreach (var out_mode_port in all_cv_port_out_mode)
            {
                //3. 若開頭為OHBLOOP的是連接LOOP的CV 而非NTB 故排除
                var target_port_info = scApp.TransferService.GetPLC_PortData(out_mode_port.PLCPortID);
                if (out_mode_port.PLCPortID.StartsWith("B7_OHBLOOP"))
                {
                    continue;
                }
                else
                {
                    //找出在儲位中的Cassette
                    cassetteDatas = cassetteDatas.Where(cst => cst.Carrier_LOC.StartsWith("10") ||
                                                               cst.Carrier_LOC.StartsWith("11") ||
                                                               cst.Carrier_LOC.StartsWith("21") ||
                                                               cst.Carrier_LOC.StartsWith("20")).
                                                               ToList();
                    //取第一筆CST
                    CassetteData chosenCst = cassetteDatas[0];
                    scApp.MapBLL.getAddressID(chosenCst.Carrier_LOC, out string from_adr);
                    bool isSuccess = true;

                    //從該選取的CST shelf 位置搬到要放置的NTB
                    isSuccess &= scApp.CMDBLL.doCreatTransferCommand(vh.VEHICLE_ID, "", chosenCst.CSTID.Trim(),
                                        E_CMD_TYPE.LoadUnload,
                                        chosenCst.Carrier_LOC,
                                        out_mode_port.PLCPortID, 0, 0,
                                        chosenCst.BOXID.Trim(), chosenCst.LotID,
                                        from_adr, out_mode_port.ADR_ID);
                }
            }
        }

        private ShelfDef FindRandomEmptyShelf(List<CassetteData> cassetteDatas)//A0.01 
        {
            //找出在儲位中的Cassette
            cassetteDatas = cassetteDatas.Where(cst => cst.Carrier_LOC.StartsWith("10") ||
                                                       cst.Carrier_LOC.StartsWith("11") ||
                                                       cst.Carrier_LOC.StartsWith("21") ||
                                                       cst.Carrier_LOC.StartsWith("20")).
                                                       ToList();
            List<string> current_cst_at_shelf_id = cassetteDatas.
                Select(cst => SCUtility.Trim(cst.Carrier_LOC, true)).
                ToList();
            //刪除目前有cst所在的儲位，讓他排除在Cycle Run的列表中
            foreach (var shelf in shelfDefs.ToList())
            {
                if (current_cst_at_shelf_id.Contains(SCUtility.Trim(shelf.ShelfID)))
                {
                    shelfDefs.Remove(shelf);
                }
            }

            //隨機找出一個要放置的shelf
            int task_RandomIndex = rnd_Index.Next(shelfDefs.Count - 1);
            return shelfDefs[task_RandomIndex];
        }


        private void AGVStationWhenTestAGV() //A0.01 
        {
            //1-確認各個AGV Station狀態
            // a-Out put mode
            //   a.1-如果有空box在上面，要將他夾回儲位
            //   a.2-如果沒有Box在上面，則要夾一個實BOX放過去
            // b-In put mode
            //   b.1-如果有實Box在上面，要將他夾回儲位
            //   b.2-如果沒有Box在上面，則要夾一個空BOX過去

            ProcessOurPutModeAndEmptyBoxOnAGVStationScript();


            //確認卡夾在哪些位置上
            List<CassetteData> cassetteDatas = scApp.CassetteDataBLL.loadCassetteData();
            //NTB算是CVport 的一種
            List<AVEHICLE> vhs = scApp.VehicleBLL.cache.loadVhs();
            //是否有要執行且可執行之 NTB to Shelf命令
            foreach (AVEHICLE vh in vhs)
            {
                if (vh.isTcpIpConnect &&
                    vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoRemote &&
                    vh.ACT_STATUS == ProtocolFormat.OHTMessage.VHActionStatus.NoCommand &&
                    vh.HAS_BOX == 0 &&
                    !SCUtility.isEmpty(vh.CUR_ADR_ID) &&
                    !scApp.CMDBLL.isCMD_OHTCExcuteByVh(vh.VEHICLE_ID))
                {
                    GenerateNtbToShelf(vh, cassetteDatas);
                }
            }
            //是否有要執行且可執行之 Shelf to NTB命令
            foreach (AVEHICLE vh in vhs)
            {
                if (vh.isTcpIpConnect &&
                    vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoRemote &&
                    vh.ACT_STATUS == ProtocolFormat.OHTMessage.VHActionStatus.NoCommand &&
                    vh.HAS_BOX == 0 &&
                    !SCUtility.isEmpty(vh.CUR_ADR_ID) &&
                    !scApp.CMDBLL.isCMD_OHTCExcuteByVh(vh.VEHICLE_ID))
                {
                    GenerateShelfToNtb(vh, cassetteDatas);
                }
            }
        }

        private void ProcessOurPutModeAndEmptyBoxOnAGVStationScript()
        {
            List<APORT> AGVStationPorts = scApp.PortBLL.OperateCatch.loadAGVStationPorts();
            AGVStationPorts = AGVStationPorts.Where(port => isOutPutModeAndEmptyBoxOnAGVStation(port)).
                                              ToList();
        }

        private bool isOutPutModeAndEmptyBoxOnAGVStation(APORT port)
        {
            var port_plc_info = scApp.TransferService.GetPLC_PortData(port.PORT_ID);
            bool is_true = port_plc_info.IsOutputMode &&
                           port_plc_info.LoadPosition1 &&
                           !port_plc_info.IsCSTPresence &&
                           port_plc_info.IsReadyToUnload;
            return is_true;
        }
    }
}

