//*********************************************************************************
//      MESDefaultMapAction.cs
//*********************************************************************************
// File Name: MESDefaultMapAction.cs
// Description: Type 1 Function
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag          Description
// ------------- -------------  -------------  ------       -----------------------------
// 2020/05/28    Jason Wu       N/A            A20.05.28.0  新增function CheckTheEmptyBoxWaterLevel() 用於定時觸發退補整條線的空box
//**********************************************************************************
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.TimerAction;
using com.mirle.ibg3k0.sc.App;
using NLog;
using System;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.TimerAction
{
    public class TransferCommandTimerAction : ITimerAction
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        protected SCApplication scApp = null;
        protected MPLCSMControl smControl;
        private ALINE line;
        double MCS_Auto_Assign_Keep_sec = 300;

        public TransferCommandTimerAction(string name, long intervalMilliSec)
            : base(name, intervalMilliSec)
        {

        }

        public override void initStart()
        {
            scApp = SCApplication.getInstance();
            line = scApp.getEQObjCacheManager().getLine();
        }

        public override void doProcess(object obj)
        {
            try
            {
                if (!line.MCSCommandAutoAssign)
                {
                    if (line.MCSAutoAssignLastOffTime.AddSeconds(MCS_Auto_Assign_Keep_sec) < DateTime.Now)
                    {
                        line.MCSCommandAutoAssign = true;//如果太久沒有重新打開AutoAssign，就自動打開，避免命令一直不執行
                    }
                }
                //檢查是否有MCS搬送命令需要執行
                //scApp.CMDBLL.checkMCS_TransferCommand();
                scApp.TransferService.TransferRun();

                //A20.05.28.0 用於定時觸發退補整條線的空box
                //Task.Run(() => scApp.TransferService.CheckTheEmptyBoxStockLevel());
                //2020/06/01 Hsinyu Chang 這裡是獨立service，判斷以DB中的LowWaterMark/HighWaterMark為準
                //Task.Run(() => scApp.EmptyBoxHandlerService.CheckTheEmptyBoxStockLevel());
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }
    }

}
