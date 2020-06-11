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
    public class RandomGeneratesCommandTimerAction_OHTC : ITimerAction
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// The sc application
        /// </summary>
        protected SCApplication scApp = null;
        private List<TranTask> tranTasks = null;

        public Dictionary<string, List<TranTask>> dicTranTaskSchedule_Clear_Dirty = null;
        public List<String> SourcePorts_None = null;
        public List<String> SourcePorts_Clear = null;
        public List<String> SourcePorts_Dirty = null;


        Random rnd_Index = new Random(Guid.NewGuid().GetHashCode());

        /// <summary>
        /// Initializes a new instance of the <see cref="BCSystemStatusTimer"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="intervalMilliSec">The interval milli sec.</param>
        public RandomGeneratesCommandTimerAction_OHTC(string name, long intervalMilliSec)
            : base(name, intervalMilliSec)
        {

        }
        /// <summary>
        /// Initializes the start.
        /// </summary>
        public override void initStart()
        {
            scApp = SCApplication.getInstance();
            tranTasks = scApp.CMDBLL.loadTranTasks();



        }
        /// <summary>
        /// Timer Action的執行動作
        /// </summary>
        /// <param name="obj">The object.</param>
        private long syncPoint = 0;
        public override void doProcess(object obj)
        {
            if (!DebugParameter.CanAutoRandomGeneratesCommand) return;
            if (System.Threading.Interlocked.Exchange(ref syncPoint, 1) == 0)
            {
                try
                {
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
            //scApp.BCSystemBLL.reWriteBCSystemRunTime();
        }

        private void OHS100()
        {
            if (scApp.VehicleBLL.getNoExcuteMcsCmdVhCount(E_VH_TYPE.Clean) > 0)
                RandomGenerates_TranTask_Clear_Drity("C");
            Thread.Sleep(1000);
            if (scApp.VehicleBLL.getNoExcuteMcsCmdVhCount(E_VH_TYPE.Dirty) > 0)
                RandomGenerates_TranTask_Clear_Drity("D");
        }

        private void RandomGenerates_TranTask_Clear_Drity(string car_type)
        {
            List<TranTask> lstTranTask = dicTranTaskSchedule_Clear_Dirty[car_type];
            int task_RandomIndex = rnd_Index.Next(lstTranTask.Count - 1);
            Console.WriteLine(string.Format("Car Type:{0},Index:{1}", car_type, task_RandomIndex));
            TranTask tranTask = lstTranTask[task_RandomIndex];
            //Task.Run(() => mcsManager.sendTranCmd(tranTask.SourcePort, tranTask.DestinationPort));
            sendTranCmd("CST02", tranTask.SourcePort, tranTask.DestinationPort);
        }

        private void Taichung()
        {
            tryCreatMCSCommand(E_VH_TYPE.None, SourcePorts_None);
            tryCreatMCSCommand(E_VH_TYPE.None, SourcePorts_Clear);
            tryCreatMCSCommand(E_VH_TYPE.None, SourcePorts_Dirty);
            tryCreatMCSCommand(E_VH_TYPE.Clean, SourcePorts_Clear);
            tryCreatMCSCommand(E_VH_TYPE.Dirty, SourcePorts_Dirty);
        }

        private void tryCreatMCSCommand(E_VH_TYPE vh_type, List<string> load_port_lst)
        {
            int vh_count = scApp.VehicleBLL.getActVhCount(vh_type);
            if (vh_count == 0 || load_port_lst == null) return;
            int unfinished_cmd_count = scApp.CMDBLL.getCMD_MCSIsUnfinishedCount(load_port_lst);
            int task_RandomIndex = 0;
            TranTask tranTask = null;
            APORTSTATION source_port_station = null;
            APORTSTATION destination_port_station = null;
            string carrier_id = null;
            string find_task_type = "";
            switch (vh_type)
            {
                case E_VH_TYPE.Clean:
                    find_task_type = "CC";
                    break;
                default:
                    find_task_type = vh_type.ToString().Substring(0, 1);
                    break;
            }
            if (unfinished_cmd_count < vh_count)
            {
                bool is_find = false;
                if (!dicTranTaskSchedule_Clear_Dirty.ContainsKey(find_task_type)) return;
                var task_list_clean = dicTranTaskSchedule_Clear_Dirty[find_task_type].ToList();
                do
                {
                    task_RandomIndex = rnd_Index.Next(task_list_clean.Count - 1);
                    tranTask = task_list_clean[task_RandomIndex];
                    source_port_station = scApp.getEQObjCacheManager().getPortStation(tranTask.SourcePort);
                    destination_port_station = scApp.getEQObjCacheManager().getPortStation(tranTask.DestinationPort);
                    if ((source_port_station != null && !SCUtility.isEmpty(source_port_station.CST_ID)) &&
                        scApp.CMDBLL.getCMD_MCSIsUnfinishedCountByCarrierID(source_port_station.CST_ID) == 0 &&
                        (destination_port_station != null && SCUtility.isEmpty(destination_port_station.CST_ID)))
                    //if ((source_port_station != null) &&
                    //scApp.CMDBLL.getCMD_MCSIsUnfinishedCountByCarrierID(source_port_station.CST_ID) == 0 &&
                    //(destination_port_station != null))
                    {
                        //carrier_id = $"{tranTask.SourcePort}To{tranTask.DestinationPort}";
                        carrier_id = source_port_station.CST_ID;
                        is_find = true;
                    }
                    else
                    {
                        task_list_clean.RemoveAt(task_RandomIndex);
                        if (task_list_clean.Count == 0) return;
                    }
                    SpinWait.SpinUntil(() => false, 1);
                } while (!is_find);

                if (is_find)
                    sendTranCmd(carrier_id, tranTask.SourcePort, tranTask.DestinationPort);
            }
        }

        public void sendTranCmd(string carrier_id, string source_port, string destn_port)
        {
            string cmdType = string.Concat(source_port, "To", destn_port);
            string cmdID = DateTime.Now.ToString("yyyyMMddHHmmssfffff");
            scApp.CMDBLL.doCreatMCSCommand(cmdID, "0", "0", carrier_id, source_port, destn_port,"0", "0","0" ,SECSConst.HCACK_Confirm, false);
            scApp.SysExcuteQualityBLL.creatSysExcuteQuality(cmdID, carrier_id, source_port, destn_port);
            SpinWait.SpinUntil(() => false, 10000);
            scApp.CMDBLL.checkMCS_TransferCommand();
        }
    }

}

