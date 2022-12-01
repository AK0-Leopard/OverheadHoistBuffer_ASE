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
using System.Diagnostics;
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
        private static Logger BCMemoryLog = LogManager.GetLogger("BCMemoryLog");
        PerformanceCounter pf1 = null;
        PerformanceCounter pf2 = null;
        PerformanceCounter cpu = null;
        PerformanceCounter bc_cpu = null;
        PerformanceCounter memory = null;
        System.Diagnostics.Process ps = null;

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
        Dictionary<string, List<string>> cycleRunRecord_VhAndShelf = new Dictionary<string, List<string>>();
        Random rnd_Index = new Random(Guid.NewGuid().GetHashCode());

        /// <summary>
        /// Initializes a new instance of the <see cref="BCSystemStatusTimer"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="intervalMilliSec">The interval milli sec.</param>
        public RandomGeneratesCommandTimerAction(string name, long intervalMilliSec)
            : base(name, intervalMilliSec)
        {
            iniPerformanceCounter();
        }
        private void iniPerformanceCounter()
        {
            ps = System.Diagnostics.Process.GetCurrentProcess();
            pf1 = new PerformanceCounter("Process", "Working Set - Private", ps.ProcessName);
            pf2 = new PerformanceCounter("Process", "Working Set", ps.ProcessName);
            bc_cpu = new PerformanceCounter("Process", "% Processor Time", ps.ProcessName, ".");

            cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            memory = new PerformanceCounter("Memory", "% Committed Bytes in Use");
        }

        /// <summary>
        /// Initializes the start.
        /// </summary>
        public override void initStart()
        {
            scApp = SCApplication.getInstance();
            AllTestAGVStationPorts = scApp.PortBLL.OperateCatch.loadAGVStationPorts();
            List<AVEHICLE> vhs = scApp.VehicleBLL.cache.loadVhs();
            foreach (var v in vhs)
            {
                cycleRunRecord_VhAndShelf.Add(SCUtility.Trim(v.VEHICLE_ID), new List<string>());
            }
        }
        /// <summary>
        /// Timer Action的執行動作
        /// </summary>
        /// <param name="obj">The object.</param>
        private long syncPoint = 0;
        public override void doProcess(object obj)
        {
            cpuMemoryMonitor();

            if (System.Threading.Interlocked.Exchange(ref syncPoint, 1) == 0)
            {
                try
                {
                    if (!DebugParameter.CanAutoRandomGeneratesCommand)
                    {
                        shelfDefs = null;
                        willTestAGVStationPorts = null;
                        foreach (var value in cycleRunRecord_VhAndShelf.Values)
                        {
                            value.Clear();
                        }
                        return;
                    }

                    switch (DebugParameter.cycleRunType)
                    {
                        case DebugParameter.CycleRunType.shelf:
                            ShelfTest();
                            break;
                        case DebugParameter.CycleRunType.shelfByOrder:
                            ShelfTestByOrder();
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
        private void cpuMemoryMonitor()
        {
            try
            {
                string 工作集_Process = $"{ ps.WorkingSet64 / 1024}";
                string 工作集 = $"{ pf2.NextValue() / 1024}";
                string 私有工作集 = $"{ pf1.NextValue() / 1024}";
                //string BC_CPU = $"{bc_cpu.NextValue():n1}";
                string BC_CPU = Math.Round(bc_cpu.NextValue(), 2).ToString();
                string CPU = $"{cpu.NextValue():n1}";
                string Memory = $"{memory.NextValue():n0}";
                var thread_pool_info = getThreadPoolInfo();
                string record_message = $"{DateTime.Now.ToString(SCAppConstants.DateTimeFormat_19)},{工作集_Process},{工作集},{私有工作集},{BC_CPU},{CPU},{Memory}," +
                                        $"{thread_pool_info.availableThreads},{thread_pool_info.availableThreadsAsyncIO},{thread_pool_info.maxThreads},{thread_pool_info.maxThreadsAsyncIO},{thread_pool_info.minThreads},{thread_pool_info.minThreadsAsyncIO}";
                BCMemoryLog.Info(record_message);

                //BCMemoryLog.Debug("{0}:{1}  {2:N}KB", ps.ProcessName, "工作集(Process)", ps.WorkingSet64 / 1024);
                //BCMemoryLog.Debug("{0}:{1}  {2:N}KB", ps.ProcessName, "工作集        ", pf2.NextValue() / 1024);
                //BCMemoryLog.Debug("{0}:{1}  {2:N}KB", ps.ProcessName, "私有工作集     ", pf1.NextValue() / 1024);
                //BCMemoryLog.Debug("{0}:{1}  {2:n1}%", ps.ProcessName, "BC CPU       ", bc_cpu.NextValue());

                //BCMemoryLog.Debug("CPU: {0:n1}%", cpu.NextValue());
                //BCMemoryLog.Debug("Memory: {0:n0}%", memory.NextValue());
            }
            catch (Exception ex) { }
        }

        public (string availableThreads, string availableThreadsAsyncIO, string maxThreads, string maxThreadsAsyncIO, string minThreads, string minThreadsAsyncIO)
                getThreadPoolInfo()
        {
            int workThreads, completionPortThreads;
            System.Threading.ThreadPool.GetAvailableThreads(out workThreads, out completionPortThreads);
            //Console.WriteLine($"GetAvailableThreads => workThreads:{workThreads};completionPortThreads:{completionPortThreads}");
            //scApp.TransferService.TransferServiceLogger.Info($"GetAvailableThreads => workThreads:{workThreads};completionPortThreads:{completionPortThreads}");
            string available_threads = workThreads.ToString();
            string available_threads_async_io = completionPortThreads.ToString();

            System.Threading.ThreadPool.GetMaxThreads(out workThreads, out completionPortThreads);
            //Console.WriteLine($"GetMaxThreads => workThreads:{workThreads};completionPortThreads:{completionPortThreads}");
            //scApp.TransferService.TransferServiceLogger.Info($"GetMaxThreads => workThreads:{workThreads};completionPortThreads:{completionPortThreads}");
            string max_threads = workThreads.ToString();
            string max_threads_async_io = completionPortThreads.ToString();

            System.Threading.ThreadPool.GetMinThreads(out workThreads, out completionPortThreads);
            //Console.WriteLine($"GetMinThreads => workThreads:{workThreads};completionPortThreads:{completionPortThreads}");
            //scApp.TransferService.TransferServiceLogger.Info($"GetMinThreads => workThreads:{workThreads};completionPortThreads:{completionPortThreads}");
            string min_threads = workThreads.ToString();
            string min_threads_async_io = completionPortThreads.ToString();

            return (available_threads, available_threads_async_io,
                    max_threads, max_threads_async_io,
                    min_threads, min_threads_async_io);

        }


        private void ShelfTest()
        {
            List<AVEHICLE> vhs = scApp.VehicleBLL.cache.loadVhs();
            foreach (AVEHICLE vh in vhs)
            {
                if (vh.isTcpIpConnect &&
                    vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoRemote &&
                    vh.ACT_STATUS == ProtocolFormat.OHTMessage.VHActionStatus.NoCommand &&
                    vh.HAS_CST == 0 &&
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
                    foreach (var cst in cassetteDatas.ToList())
                    {
                        if (scApp.CMDBLL.hasExcuteCMDByBoxID(cst.BOXID))
                        {
                            cassetteDatas.Remove(cst);
                        }
                    }

                    List<string> current_cst_at_shelf_id = cassetteDatas.
                        Select(cst => SCUtility.Trim(cst.Carrier_LOC, true)).
                        ToList();

                    //刪除目前cst所在的儲位，讓他排除在Cycle Run的列表中
                    foreach (var shelf in shelfDefs.ToList())
                    {
                        if (scApp.CMDBLL.hasExcuteCMDByTargetPort(shelf.ShelfID))
                        {
                            shelfDefs.Remove(shelf);
                        }
                    }
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
        private void ShelfTestByOrder()
        {
            if (ACMD_MCS.MCS_CMD_InfoList != null && ACMD_MCS.MCS_CMD_InfoList.Count >= 8)
                return;
            List<CassetteData> cassetteDatas = scApp.CassetteDataBLL.loadCassetteData();
            if (cassetteDatas == null || cassetteDatas.Count() == 0) return;
            //找一份目前儲位的列表
            shelfDefs = scApp.ShelfDefBLL.LoadEnableShelf();
            //如果取完還是空的 就跳出去
            if (shelfDefs == null || shelfDefs.Count == 0)
                return;

            //取得目前當前在線內的Carrier
            //找出在儲位中的Cassette

            cassetteDatas = cassetteDatas.
                Where(cst => scApp.TransferService.isShelfPort(cst.Carrier_LOC) &&
                             SCUtility.isEmpty(cst.CSTID)).ToList();
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
            foreach (var cst in cassetteDatas.ToList())
            {
                //if (cst.hasCommandExcute(scApp.CMDBLL))
                if (scApp.CMDBLL.getCMD_ByBoxID(cst.BOXID) != null)
                {
                    cassetteDatas.Remove(cst);
                }
            }
            //隨機找出一個要放置的Zone
            CassetteData process_cst = cassetteDatas[0];
            string cst_zone_id = process_cst.getZoneID(scApp.TransferService);

            shelfDefs = shelfDefs.Where(shelf => !SCUtility.isMatche(shelf.ZoneID, cst_zone_id)).ToList();
            List<string> orther_zone_ids = shelfDefs.Select(shelf => shelf.ZoneID).Distinct().ToList();
            bool is_creat_zone_id = true;
            string desc_name = "";
            if (is_creat_zone_id)
            {
                int task_RandomIndex = rnd_Index.Next(orther_zone_ids.Count - 1);
                desc_name = orther_zone_ids[task_RandomIndex];
            }
            else
            {
                int task_RandomIndex = rnd_Index.Next(shelfDefs.Count - 1);
                ShelfDef target_shelf_def = shelfDefs[task_RandomIndex];
                scApp.MapBLL.getAddressID(process_cst.Carrier_LOC, out string from_adr);
                desc_name = from_adr;
            }

            bool isSuccess = true;
            isSuccess = scApp.TransferService.Manual_InsertCmd(process_cst.Carrier_LOC, desc_name) == "OK";
            //isSuccess &= scApp.CMDBLL.doCreatTransferCommand(vh.VEHICLE_ID, "", process_cst.CSTID.Trim(),
            //                    E_CMD_TYPE.LoadUnload,
            //                    process_cst.Carrier_LOC,
            //                    target_shelf_def.ShelfID, 0, 0,
            //                    process_cst.BOXID.Trim(), process_cst.LotID,
            //                    from_adr, target_shelf_def.ADR_ID);
            //shelfDefs.Remove(target_shelf_def);
        }
        private void ShelfTestByOrder_old()
        {
            if (ACMD_MCS.MCS_CMD_InfoList != null && ACMD_MCS.MCS_CMD_InfoList.Count >= 8)
                return;
            List<AVEHICLE> vhs = scApp.VehicleBLL.cache.loadVhs();
            foreach (AVEHICLE vh in vhs)
            {
                if (vh.isTcpIpConnect &&
                    vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoRemote &&
                    //vh.ACT_STATUS == ProtocolFormat.OHTMessage.VHActionStatus.NoCommand &&
                    SCUtility.isEmpty(vh.MCS_CMD) &&
                    !SCUtility.isEmpty(vh.CUR_ADR_ID))
                {
                    List<string> vh_has_been_excuted_shelf = cycleRunRecord_VhAndShelf[vh.VEHICLE_ID];
                    List<CassetteData> cassetteDatas = scApp.CassetteDataBLL.loadCassetteData();
                    if (cassetteDatas == null || cassetteDatas.Count() == 0) return;
                    //找一份目前儲位的列表
                    shelfDefs = scApp.ShelfDefBLL.LoadEnableShelf();
                    //如果取完還是空的 就跳出去
                    if (shelfDefs == null || shelfDefs.Count == 0)
                        return;
                    //去除掉該vh已經跑過的shelf
                    shelfDefs = shelfDefs.Where(s => !vh_has_been_excuted_shelf.Contains(s.ShelfID))
                                         .OrderBy(s => s.ShelfID)
                                         .ToList();


                    //取得目前當前在線內的Carrier
                    //找出在儲位中的Cassette
                    cassetteDatas = cassetteDatas.
                        Where(cst => scApp.TransferService.isShelfPort(cst.Carrier_LOC)).ToList();
                    List<string> current_cst_at_shelf_id = cassetteDatas.
                        Select(cst => SCUtility.Trim(cst.Carrier_LOC, true)).
                        ToList();
                    //刪除目前cst所在的儲位，讓他排除在Cycle Run的列表中
                    //foreach (var shelf in shelfDefs.ToList())
                    //{
                    //    if (current_cst_at_shelf_id.Contains(SCUtility.Trim(shelf.ShelfID)))
                    //    {
                    //        shelfDefs.Remove(shelf);
                    //    }
                    //}
                    foreach (var cst in cassetteDatas.ToList())
                    {
                        if (cst.hasCommandExcute(scApp.CMDBLL))
                        {
                            cassetteDatas.Remove(cst);
                        }
                    }
                    //隨機找出一個要放置的shelf
                    CassetteData process_cst = cassetteDatas[0];
                    int task_RandomIndex = rnd_Index.Next(shelfDefs.Count - 1);
                    ShelfDef target_shelf_def = shelfDefs[task_RandomIndex];
                    scApp.MapBLL.getAddressID(process_cst.Carrier_LOC, out string from_adr);
                    bool isSuccess = true;
                    isSuccess = scApp.TransferService.Manual_InsertCmd(process_cst.Carrier_LOC, target_shelf_def.ShelfID) == "OK";
                    //isSuccess &= scApp.CMDBLL.doCreatTransferCommand(vh.VEHICLE_ID, "", process_cst.CSTID.Trim(),
                    //                    E_CMD_TYPE.LoadUnload,
                    //                    process_cst.Carrier_LOC,
                    //                    target_shelf_def.ShelfID, 0, 0,
                    //                    process_cst.BOXID.Trim(), process_cst.LotID,
                    //                    from_adr, target_shelf_def.ADR_ID);
                    if (isSuccess)
                    {
                        cycleRunRecord_VhAndShelf[vh.VEHICLE_ID].Add(target_shelf_def.ShelfID);
                    }
                    //shelfDefs.Remove(target_shelf_def);
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

                    ////找出目前的AGVStation Port
                    //if (willTestAGVStationPorts == null || willTestAGVStationPorts.Count == 0)
                    //{
                    //    willTestAGVStationPorts = scApp.PortBLL.OperateCatch.loadAGVStationPorts();
                    //    willTestAGVStationPorts = willTestAGVStationPorts.Where(port => scApp.TransferService.GetPLC_PortData(port.PORT_ID).IsReadyToLoad &&
                    //                                                                    scApp.TransferService.GetPLC_PortData(port.PORT_ID).IsAutoMode &&
                    //                                                                    scApp.TransferService.GetPLC_PortData(port.PORT_ID).cim_on).
                    //                                                      ToList();
                    //    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(RandomGeneratesCommandTimerAction), Device: "OHTC",
                    //             Data: $"Load ok:{string.Join(",", willTestAGVStationPorts.Select(port => port.PORT_ID).ToList())}");
                    //}
                    ////如果取完還是空的 就跳出去
                    //if (willTestAGVStationPorts == null || willTestAGVStationPorts.Count == 0)
                    //{
                    //    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(RandomGeneratesCommandTimerAction), Device: "OHTC",
                    //             Data: $"no agv station list.");
                    //    return;
                    //}


                    //找出目前Unload Ok的AGV Station
                    var unload_ok_ports = AllTestAGVStationPorts.Where(port => scApp.TransferService.GetPLC_PortData(port.PORT_ID).IsReadyToUnload &&
                                                                              scApp.TransferService.GetPLC_PortData(port.PORT_ID).IsAutoMode &&
                                                                              scApp.TransferService.GetPLC_PortData(port.PORT_ID).cim_on).
                                                                ToList();
                    foreach (var port in unload_ok_ports.ToList())
                    {
                        if (scApp.CMDBLL.hasExcuteCMDBySourcePort(port.PORT_ID))
                        {
                            unload_ok_ports.Remove(port);
                        }
                    }
                    var unload_ok_port = unload_ok_ports.FirstOrDefault();
                    //var unload_ok_port = AllTestAGVStationPorts.FirstOrDefault();
                    if (unload_ok_port == null)
                    {
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(RandomGeneratesCommandTimerAction), Device: "OHTC",
                                 Data: $"no unload ok of agv station, can't execute cycle run test.");
                        return;
                    }
                    //找出目前load Ok的AGV Station
                    var load_ok_ports = AllTestAGVStationPorts.Where(port => scApp.TransferService.GetPLC_PortData(port.PORT_ID).IsReadyToLoad &&
                                                                              scApp.TransferService.GetPLC_PortData(port.PORT_ID).IsAutoMode &&
                                                                              scApp.TransferService.GetPLC_PortData(port.PORT_ID).cim_on).
                                                                ToList();
                    foreach (var port in load_ok_ports.ToList())
                    {
                        if (scApp.CMDBLL.hasExcuteCMDByTargetPort(port.PORT_ID))
                        {
                            load_ok_ports.Remove(port);
                        }
                    }
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
                    vh.HAS_CST == 0 &&
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

                        PortDef target_port = scApp.PortDefBLL.cache.getPortDef(target_port_id);
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
                    vh.HAS_CST == 0 &&
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
                    vh.HAS_CST == 0 &&
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

