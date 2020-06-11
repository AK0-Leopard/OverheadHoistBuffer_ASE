//*********************************************************************************
//      AGVCAliveTimer.cs
//*********************************************************************************
// File Name: AGVCAliveTimer.cs
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
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace com.mirle.ibg3k0.sc.Data.TimerAction
{
    class OHCVAliveTimer : ITimerAction
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        protected SCApplication scApp = null;
        protected MPLCSMControl smControl;
        int[] syncPoint = new int[8];
        public OHCVAliveTimer(string name, long intervalMilliSec)
            : base(name, intervalMilliSec)
        {

        }

        public override void initStart()
        {
            scApp = SCApplication.getInstance();
            //smControl = scApp.getBCFApplication().getMPLCSMControl("Charger") as MPLCSMControl;
        }
        public override void doProcess(object obj)
        {
            //Task.Run(() => AliveMTL());
            Task.Run(() => AliveToDevice(0, "CV31_A", "OHTC_TO_OHCV_ALIVE"));
            Task.Run(() => AliveToDevice(1, "CV31_B", "OHTC_TO_OHCV_ALIVE"));
            Task.Run(() => AliveToDevice(2, "CV32_A", "OHTC_TO_OHCV_ALIVE"));
            Task.Run(() => AliveToDevice(3, "CV32_B", "OHTC_TO_OHCV_ALIVE"));
            Task.Run(() => AliveToDevice(4, "CV61_A", "OHTC_TO_OHCV_ALIVE"));
            Task.Run(() => AliveToDevice(5, "CV61_B", "OHTC_TO_OHCV_ALIVE"));
            Task.Run(() => AliveToDevice(6, "CV62_A", "OHTC_TO_OHCV_ALIVE"));
            Task.Run(() => AliveToDevice(7, "CV62_B", "OHTC_TO_OHCV_ALIVE"));
        }

        private bool switchFlag = true;
        private void AliveToDevice(int syncIndex, string eqID, string writeName)
        {
            if (System.Threading.Interlocked.Exchange(ref syncPoint[syncIndex], 1) == 0)
            {

                bool isWriteSucess = false;
                try
                {
                    ValueWrite isAliveVW = scApp.getBCFApplication().getWriteValueEvent(SCAppConstants.EQPT_OBJECT_CATE_EQPT, eqID, writeName);
                    if (isAliveVW == null) return;
                    Boolean aliveSignal = (Boolean)isAliveVW.getText();

                    int writeSigmal = aliveSignal ? 0 : 1;
                    isAliveVW.setWriteValue(writeSigmal.ToString());
                    ISMControl.writeDeviceBlock(scApp.getBCFApplication(), isAliveVW);

                    if (isWriteSucess || switchFlag)
                    {
                        isWriteSucess = false;
                        switchFlag = false;
                        isWriteSucess = ISMControl.writeDeviceBlock(scApp.getBCFApplication(), isAliveVW);
                        switchFlag = true;
                    }
                    else
                    {
                        switchFlag = false;
                        isWriteSucess = false;
                    }

                }
                catch (Exception e)
                {
                    switchFlag = true;
                    isWriteSucess = false;
                }
                finally
                {
                    AEQPT eqpt = scApp.getEQObjCacheManager().getEquipmentByEQPTID(eqID);
                    if (eqpt != null)
                        eqpt.Plc_Link_Stat = isWriteSucess ? SCAppConstants.LinkStatus.LinkOK : SCAppConstants.LinkStatus.LinkFail;
                    System.Threading.Interlocked.Exchange(ref syncPoint[syncIndex], 0);
                }

            }
        }
    }
}