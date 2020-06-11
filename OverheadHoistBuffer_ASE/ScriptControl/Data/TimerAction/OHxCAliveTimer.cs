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
    class OHxCAliveTimer : ITimerAction
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        protected SCApplication scApp = null;
        protected MPLCSMControl smControl;
        int[] syncPoint = new int[1];
        public OHxCAliveTimer(string name, long intervalMilliSec)
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
            Task.Run(() => AliveToDevice(0, "HID", "OHXC_TO_HID_ALIVE_INDEX"));
        }

        private long syncMTLPoint = 0;
        private void AliveMTL()
        {
            if (System.Threading.Interlocked.Exchange(ref syncMTLPoint, 1) == 0)
            {

                try
                {
                    ValueWrite isAliveIndexVW = scApp.getBCFApplication().getWriteValueEvent(SCAppConstants.EQPT_OBJECT_CATE_EQPT, "MTL", "OHXC_TO_MTL_ALIVE_INDEX");
                    if (isAliveIndexVW == null) return;
                    UInt16 isAliveIndex = (UInt16)isAliveIndexVW.getText();

                    int x = isAliveIndex + 1;
                    if (x > 9999) { x = 1; }
                    isAliveIndexVW.setWriteValue((UInt16)x);
                    ISMControl.writeDeviceBlock(scApp.getBCFApplication(), isAliveIndexVW);


                }
                catch (Exception e)
                {
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref syncMTLPoint, 0);
                }

            }
        }
        private long syncMTSPoint = 0;
        private bool switchFlag = true;
        private void AliveToDevice(int syncIndex, string eqID, string writeName)
        {
            if (System.Threading.Interlocked.Exchange(ref syncPoint[syncIndex], 1) == 0)
            {

                bool isWriteSucess = false;
                try
                {
                    ValueWrite isAliveIndexVW = scApp.getBCFApplication().getWriteValueEvent(SCAppConstants.EQPT_OBJECT_CATE_EQPT, eqID, writeName);
                    if (isAliveIndexVW == null) return;
                    UInt16 isAliveIndex = (UInt16)isAliveIndexVW.getText();

                    int x = isAliveIndex + 1;
                    if (x > 9999) { x = 1; }
                    isAliveIndexVW.setWriteValue((UInt16)x);
                    ISMControl.writeDeviceBlock(scApp.getBCFApplication(), isAliveIndexVW);

                    if (isWriteSucess || switchFlag)
                    {
                        isWriteSucess = false;
                        switchFlag = false;
                        isWriteSucess = ISMControl.writeDeviceBlock(scApp.getBCFApplication(), isAliveIndexVW);
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