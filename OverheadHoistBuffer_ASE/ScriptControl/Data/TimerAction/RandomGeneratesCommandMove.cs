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
    public class RandomGeneratesCommandMove : ITimerAction
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
        public RandomGeneratesCommandMove(string name, long intervalMilliSec)
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
                    List<AVEHICLE> vhs = scApp.VehicleBLL.cache.loadVhs();
                    foreach (AVEHICLE vh in vhs)
                    {
                        if (vh.isTcpIpConnect &&
                            vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoRemote &&
                            vh.ACT_STATUS == ProtocolFormat.OHTMessage.VHActionStatus.NoCommand &&
                            !SCUtility.isEmpty(vh.CUR_ADR_ID) &&
                            !scApp.CMDBLL.isCMD_OHTCExcuteByVh(vh.VEHICLE_ID))
                        {

                            List<TranTask> selectedTranTasks = null;
                            string adr_id = null;
                            if (SCUtility.isMatche(vh.VEHICLE_ID, "OHx12") ||
                                SCUtility.isMatche(vh.VEHICLE_ID, "OHx14"))
                            {
                                selectedTranTasks = tranTasks;
                            }
                            else
                            {
                                selectedTranTasks = tranTasks.Where(task => task.CarType != "1").ToList();
                            }
                            //do
                            //{
                            int task_RandomIndex = rnd_Index.Next(selectedTranTasks.Count - 1);
                            var next_move_task = selectedTranTasks[task_RandomIndex];
                            adr_id = next_move_task.SourcePort;

                            //SpinWait.SpinUntil(() => false, 1);
                            //} while (scApp.RouteGuide.checkRoadIsWalkable(adr_id, vh.CUR_ADR_ID));

                            if (!SCUtility.isMatche(adr_id, vh.CUR_ADR_ID))
                            {
                                scApp.CMDBLL.doCreatTransferCommand(vh.VEHICLE_ID
                                                              , string.Empty
                                                              , string.Empty
                                                              , E_CMD_TYPE.Move
                                                              , string.Empty
                                                              , adr_id, 0, 0);
                            }
                        }
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
            //scApp.BCSystemBLL.reWriteBCSystemRunTime();
        }
    }

}

