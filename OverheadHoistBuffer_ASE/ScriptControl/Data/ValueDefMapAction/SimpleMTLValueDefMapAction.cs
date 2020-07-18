//*********************************************************************************
//      MTLValueDefMapAction.cs
//*********************************************************************************
// File Name: MTLValueDefMapAction.cs
// Description: 
//
//(c) Copyright 2018, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Common.MPLC;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Data.VO;
using KingAOP;
using NLog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

namespace com.mirle.ibg3k0.sc.Data.ValueDefMapAction
{
    public class SimpleMTLValueDefMapAction : IValueDefMapAction
    {
        public const string DEVICE_NAME_MTL = "MTL";
        Logger logger = NLog.LogManager.GetCurrentClassLogger();
        protected AEQPT eqpt = null;
        public virtual void setContext(BaseEQObject baseEQ)
        {
            this.eqpt = baseEQ as AEQPT;
        }
        protected SCApplication scApp = null;
        protected BCFApplication bcfApp = null;

        public SimpleMTLValueDefMapAction()
            : base()
        {
            scApp = SCApplication.getInstance();
            bcfApp = scApp.getBCFApplication();
        }
        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new AspectWeaver(parameter, this);
        }

        public virtual string getIdentityKey()
        {
            return this.GetType().Name;
        }
        public virtual void unRegisterEvent()
        {
            //not implement
        }
        public virtual void doShareMemoryInit(BCFAppConstants.RUN_LEVEL runLevel)
        {
            try
            {
                switch (runLevel)
                {
                    case BCFAppConstants.RUN_LEVEL.ZERO:
                        MTLTrackClosedChange(null, null);
                        break;
                    case BCFAppConstants.RUN_LEVEL.ONE:
                        break;
                    case BCFAppConstants.RUN_LEVEL.TWO:
                        break;
                    case BCFAppConstants.RUN_LEVEL.NINE:
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
            }
        }

        /// <summary>
        /// Does the initialize.
        /// </summary>
        public virtual void doInit()
        {
            try
            {
                ValueRead vr = null;

                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TRACK_CLOSED", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => MTLTrackClosedChange(_sender, _e);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
            }

        }

        private void MTLTrackClosedChange(object sender, ValueChangedEventArgs e)
        {
            try
            {
                ValueRead mtl_track_closed = bcfApp.getReadValueEvent(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TRACK_CLOSED");
                bool is_closed = (bool)mtl_track_closed.getText();
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(SimpleMTLValueDefMapAction), Device: DEVICE_NAME_MTL,
                         Data: $"MTL of Track Single:{is_closed}",
                         VehicleID: eqpt.EQPT_ID);
                if(is_closed == true)
                {
                    return;
                }
                List<AVEHICLE> vhs = scApp.VehicleBLL.cache.loadVhs();
                foreach (AVEHICLE vh in vhs)
                {
                    scApp.VehicleService.PauseRequest(vh.VEHICLE_ID, ProtocolFormat.OHTMessage.PauseEvent.Pause, SCAppConstants.OHxCPauseType.Safty);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }
    }
}
