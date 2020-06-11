//*********************************************************************************
//      ZoneBlockCheck.cs
//*********************************************************************************
// File Name: ZoneBlockCheck.cs
// Description: 
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.TimerAction;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.TimerAction
{
    /// <summary>
    /// Class ZoneBlockCheck.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.TimerAction.ITimerAction" />
    class ZoneBlockCheck : ITimerAction
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// The sc application
        /// </summary>
        protected SCApplication scApp = null;


        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneBlockCheck"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="intervalMilliSec">The interval milli sec.</param>
        public ZoneBlockCheck(string name, long intervalMilliSec)
            : base(name, intervalMilliSec)
        {

        }

        /// <summary>
        /// Initializes the start.
        /// </summary>
        public override void initStart()
        {
            //do nothing
            scApp = SCApplication.getInstance();

        }

        private long checkSyncPoint = 0;
        /// <summary>
        /// Timer Action的執行動作
        /// </summary>
        /// <param name="obj">The object.</param>
        public override void doProcess(object obj)
        {
            if (System.Threading.Interlocked.Exchange(ref checkSyncPoint, 1) == 0)
            {
                //using (DBConnection_EF con = new DBConnection_EF())
                //{
                try
                {
                    scApp.VehicleService.CheckBlockControlByVehicleView();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exection:");
                }
                finally
                {

                    System.Threading.Interlocked.Exchange(ref checkSyncPoint, 0);

                }
                //}
            }


            //if (System.Threading.Interlocked.Exchange(ref checkSyncPoint, 1) == 0)
            //{
            //    try
            //    {
            //        //1.找出目前BlockZoneQueue中還沒釋放的Zone(距離要求時間大於15秒的)
            //        List<BLOCKZONEQUEUE> queues = scApp.MapBLL.loadAllProblematicUsingBlockQueue();
            //        //2.確認該台車是否還在這個Block Zone中
            //        foreach (BLOCKZONEQUEUE block_queue in queues)
            //        {
            //            string entry_sec_id = block_queue.ENTRY_SEC_ID.Trim();
            //            string car_id = block_queue.CAR_ID.Trim();

            //            //AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(car_id);
            //            //List<string> lstSecid = scApp.MapBLL.loadZoneDetailSecIDsByEntrySecID(entry_sec_id);

            //            bool isInBlockZone = scApp.MapBLL.IsVHInBlockZoneByEntrySectionID(car_id, entry_sec_id);
            //            //if (!SCUtility.isMatche(vh.CUR_SEC_ID, entry_sec_id))
            //            //if (!lstSecid.Contains(vh.CUR_SEC_ID))
            //            if (!isInBlockZone)
            //            {
            //                scApp.MapBLL.updateBlockZoneQueue_AbnormalEnd(block_queue);
            //                scApp.MapBLL.CheckAndNoticeBlockVhPassByEntrySecID(entry_sec_id);
            //                //foreach (string sec_id in lstSecid)
            //                //{
            //                //    BLOCKZONEQUEUE blockZoneQueue = scApp.MapBLL.getReqBlockQueueBySecID(sec_id);
            //                //    if (blockZoneQueue != null)
            //                //    {
            //                //        //Equipment noticeCar = scApp.getEQObjCacheManager().getEquipmentByEQPTID(blockZoneQueue.CAR_ID.Trim());
            //                //        AVEHICLE noticeCar = scApp.getEQObjCacheManager().getVehicletByVHID(blockZoneQueue.CAR_ID.Trim());
            //                //        //noticeCar.sned_Str31(ActiveType.Continue, string.Empty, new string[0], new string[0], string.Empty, string.Empty);
            //                //        break;
            //                //    }
            //                //}
            //            }
            //        }
            //        //3.若不是則將Realase Time填入當作已經通過，並呼叫CheckAndNoticeBlockCarPass
            //    }
            //    finally
            //    {
            //        System.Threading.Interlocked.Exchange(ref checkSyncPoint, 0);
            //    }
            //}
        }


    }
}