using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace com.mirle.ibg3k0.sc.Scheduler
{
    public class DBManatainScheduler : IJob
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        SCApplication scApp = SCApplication.getInstance();
        const int BLOCK_QUEUE_KEEP_TIME_N_Day = 7;
        private static long syncPoint = 0;
        public void Execute(IJobExecutionContext context)
        {
            if (Interlocked.Exchange(ref syncPoint, 1) == 0)
            {

                try
                {
                    //using (TransactionScope tx = SCUtility.getTransactionScope())
                    //{
                    //    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    //    {
                    //        MoveACMD_MCSToHCMD_MCS();

                    //        MoveACMD_OHTCToHCMD_OHTC();

                    //        RemoveNDayAgoBlockQueue(BLOCK_QUEUE_KEEP_TIME_N_Day);

                    //        RemoveOHTCCMDDetail();
                    //        tx.Complete();
                    //    }
                    //}
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        MoveACMD_MCSToHCMD_MCS();
                    }
                    SpinWait.SpinUntil(() => false, 5000);
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        MoveACMD_OHTCToHCMD_OHTC();
                    }
                    SpinWait.SpinUntil(() => false, 5000);
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        DeleteHCMD_MCS();
                    }
                    SpinWait.SpinUntil(() => false, 5000);
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        DeleteHCMD_OHTC();
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

        private void MoveACMD_MCSToHCMD_MCS()
        {
            var finish_cmd_mcs_list = scApp.CMDBLL.loadFinishCMD_MCS();
            if (finish_cmd_mcs_list != null && finish_cmd_mcs_list.Count > 0)
            {
                scApp.CMDBLL.remoteCMD_MCSByBatch(finish_cmd_mcs_list);
                List<HCMD_MCS> hcmd_mcs_list = finish_cmd_mcs_list.Select(cmd => cmd.ToHCMD_MCS()).ToList();
                scApp.CMDBLL.CreatHCMD_MCSs(hcmd_mcs_list);
            }
        }
        private void MoveACMD_OHTCToHCMD_OHTC()
        {
            var finish_cmd_ohtc_list = scApp.CMDBLL.loadFinishCMD_OHTC();
            if (finish_cmd_ohtc_list != null && finish_cmd_ohtc_list.Count > 0)
            {
                scApp.CMDBLL.remoteCMD_OHTCByBatch(finish_cmd_ohtc_list);
                List<HCMD_OHTC> hcmd_ohtc_list = finish_cmd_ohtc_list.Select(cmd => cmd.ToHCMD_OHTC()).ToList();
                scApp.CMDBLL.CreatHCMD_OHTCs(hcmd_ohtc_list);
            }
        }

        private void DeleteHCMD_MCS()
        {
            var hcmd_mcs_list = scApp.CMDBLL.loadHCMD_MCSBefore6Months();
            if (hcmd_mcs_list != null && hcmd_mcs_list.Count > 0)
            {
                scApp.CMDBLL.RemoteHCMD_MCSByBatch(hcmd_mcs_list); ;
            }
        }
        private void DeleteHCMD_OHTC()
        {
            var hcmd_ohtc_list = scApp.CMDBLL.loadHCMD_OHTCBefore6Months();
            if (hcmd_ohtc_list != null && hcmd_ohtc_list.Count > 0)
            {
                scApp.CMDBLL.RemoteHCMD_OHTCByBatch(hcmd_ohtc_list); ;
            }
        }

    }

}
