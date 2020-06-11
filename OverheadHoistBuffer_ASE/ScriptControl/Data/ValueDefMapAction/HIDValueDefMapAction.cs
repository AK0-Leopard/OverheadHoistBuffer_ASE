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
using KingAOP;
using NLog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

namespace com.mirle.ibg3k0.sc.Data.ValueDefMapAction
{
    public class HIDValueDefMapAction : IValueDefMapAction
    {
        public const string DEVICE_NAME_MTL = "HID";
        Logger logger = NLog.LogManager.GetCurrentClassLogger();
        AEQPT eqpt = null;
        protected SCApplication scApp = null;
        protected BCFApplication bcfApp = null;

        public HIDValueDefMapAction()
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
        public virtual void setContext(BaseEQObject baseEQ)
        {
            this.eqpt = baseEQ as AEQPT;

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
                        initHID_ChargeInfo();
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

        public virtual void initHID_ChargeInfo()
        {
            var function = scApp.getFunBaseObj<HIDToOHxC_ChargeInfo>(eqpt.EQPT_ID) as HIDToOHxC_ChargeInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                //2.read log
                function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<HIDToOHxC_ChargeInfo>(function);
            }
        }

        public virtual void HID_ChargeInfo(object sender, ValueChangedEventArgs args)
        {
            var function = scApp.getFunBaseObj<HIDToOHxC_ChargeInfo>(eqpt.EQPT_ID) as HIDToOHxC_ChargeInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                //2.read log
                function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                eqpt.HID_Info = function;

                eqpt.Eq_Alive_Last_Change_time = DateTime.Now;

                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<HIDToOHxC_ChargeInfo>(function);
            }
        }

        public virtual void PowerAlarm(object sender, ValueChangedEventArgs args)
        {
            var recevie_function = scApp.getFunBaseObj<HIDToOHxC_PowerAlarm>(eqpt.EQPT_ID) as HIDToOHxC_PowerAlarm;
            try
            {
                //1.建立各個Function物件
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                //2.read log
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(HIDValueDefMapAction), Device: DEVICE_NAME_MTL,
                         Data: recevie_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                NLog.LogManager.GetLogger("HIDAlarm").Info(recevie_function.ToString());
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<HIDToOHxC_PowerAlarm>(recevie_function);
            }
        }
        public virtual void TempAlarm(object sender, ValueChangedEventArgs args)
        {
            var recevie_function = scApp.getFunBaseObj<HIDToOHxC_TempAlarm>(eqpt.EQPT_ID) as HIDToOHxC_TempAlarm;
            try
            {
                //1.建立各個Function物件
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                //2.read log
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(HIDValueDefMapAction), Device: DEVICE_NAME_MTL,
                         Data: recevie_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                NLog.LogManager.GetLogger("HIDAlarm").Info(recevie_function.ToString());
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<HIDToOHxC_TempAlarm>(recevie_function);
            }
        }

        public virtual void HID_Control(bool control)
        {
            var function = scApp.getFunBaseObj<OHxCToHID_Control>(eqpt.EQPT_ID) as OHxCToHID_Control;
            try
            {
                //1.建立各個Function物件

                function.Control = control;
                function.Write(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                function.Timestamp = DateTime.Now;


                //2.write log
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());

                //3.logical (include db save)
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<OHxCToHID_Control>(function);
            }
        }



        string event_id = string.Empty;
        /// <summary>
        /// Does the initialize.
        /// </summary>
        public virtual void doInit()
        {
            try
            {
                ValueRead vr = null;
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "HID_TO_OHXC_TRIGGER", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => HID_ChargeInfo(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "HID_TO_OHXC_POWER_ALARM", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => PowerAlarm(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "HID_TO_OHXC_TEMP_ALARM", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => TempAlarm(_sender, _e);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
            }

        }


    }
}
