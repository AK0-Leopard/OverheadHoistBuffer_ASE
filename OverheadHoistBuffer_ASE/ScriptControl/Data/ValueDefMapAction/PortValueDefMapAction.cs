using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Service;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.ValueDefMapAction
{
    public class PortValueDefMapAction : IValueDefMapAction
    {
        public const string DEVICE_NAME_PORT = "PORT";
        public bool IsInService { get; private set; } = false;
        public bool IsAGVMode { get; private set; } = true;
        private Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected APORT port = null;

        protected SCApplication scApp = null;
        protected BCFApplication bcfApp = null;

        public PortValueDefMapAction() : base()
        {
            scApp = SCApplication.getInstance();
            bcfApp = scApp.getBCFApplication();
        }

        public virtual void doInit()
        {
            try
            {
                ValueRead vr = null;
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "MODE_CHANGEABLE", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onModeChangable(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "WAIT_IN", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onWaitIn(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "WAIT_OUT", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onWaitOut(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "NOW_INPUT_MODE", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onDirectionStatusChangeToInput(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "NOW_OUTPUT_MODE", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onDirectionStatusChangeToOutput(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "BARCODE_READ_DONE", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onBarcodeReadDone(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "CST_TRANSFER_COMPLETE", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onTransferComplete(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "CST_REMOVE_CHECK", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onCSTRemoveCheck(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "LOAD_POSITION_1", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onLoadPosition(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "LOAD_POSITION_2", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onLoadPosition2(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "LOAD_POSITION_3", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onLoadPosition3(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "LOAD_POSITION_4", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onLoadPosition4(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "LOAD_POSITION_5", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onLoadPosition5(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "LOAD_POSITION_6", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onLoadPosition6(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "LOAD_POSITION_7", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onLoadPosition7(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "OP_RUN", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onInService(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "OP_DOWN", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onOutOfService(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "AGV_PORT_READY", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onAGVPortReadyStatusChange(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "IS_CST_PRESENCE", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onCstPresenceStatusChange(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "OP_ERROR", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onAlarmHppened(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "IS_AGV_MODE", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onChangeToAGVMode(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "IS_MGV_MODE", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onChangeToMGVMode(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "CST_PRRESENCE_MISMATCH", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onCSTPresenceMismatch(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "FIRE_ALARM", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onFireAlarm(_sender, _e);
                }

                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "PORTALLINFO", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => PortInfoChange(_sender, _e);
                }

                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "CIM_ON", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onCIM_ON(_sender, _e);
                }

                if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "PreLoadOK", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => Port_onPreLoadOK(_sender, _e);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        private void Port_onFireAlarm(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)
                if (function.FireAlarm == true)
                {
                    scApp.TransferService.OHBC_AlarmSet(port.PORT_ID, SCAppConstants.SystemAlarmCode.PLC_Issue.FireAlarm);
                }
                else
                {
                    scApp.TransferService.OHBC_AlarmAllCleared(port.PORT_ID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }

        private void Port_onCSTPresenceMismatch(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)
                if (function.CSTPresenceMismatch)
                {
                    //TODO
                    if(function.IsInputMode)
                    {
                        scApp.TransferService.OpenAGV_Station(function.EQ_ID.Trim(), true, "CSTPresenceMismatch");
                    }
                    
                    scApp.TransferService.PLC_AGV_Station(function, "CSTPresenceMismatch");
                }
                else
                {
                    //0506，士偉、冠皚討論說，若 Mismatch OFF，如果有在執行的退補BOX，就將其把命取消 註：防止 Mismatch 上報後再報 WaitIn 的機制
                    scApp.TransferService.PLC_AGV_CancelCmd(function.EQ_ID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }

        private void Port_onChangeToMGVMode(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)
                if (function.IsMGVMode == true)
                {
                    IsAGVMode = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }

        private void Port_onChangeToAGVMode(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)
                if (function.IsAGVMode == true)
                {
                    IsAGVMode = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }

        private void Port_onAlarmHppened(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)
                if (function.OpError == true)
                {
                    //pass function.ErrorCode
                    scApp.TransferService.OHBC_AlarmSet(function.EQ_ID, function.ErrorCode.ToString());
                    //PLC_AlarmSet(port.PORT_ID, function.ErrorCode);
                }
                else
                {
                    scApp.TransferService.OHBC_AlarmAllCleared(function.EQ_ID);
                    //PLC_AlarmAllCleared(port.PORT_ID);
                    scApp.TransferService.PortCIM_ON(function, "Port_Error_OFF");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }

        private void Port_onCstPresenceStatusChange(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)

                scApp.TransferService.TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "PLC >> PLC|AGV卡匣在席變化  PortName: " + function.EQ_ID
                    + " IsReadyToLoad: " + function.IsReadyToLoad
                    + " IsReadyToUnload:" + function.IsReadyToUnload
                    + " IsCSTPresence:" + function.IsCSTPresence
                    + " CstRemoveCheck:" + function.CstRemoveCheck
                    + " IsInputMode:" + function.IsInputMode
                    + " IsOutputMode:" + function.IsOutputMode
                    + " PortWaitIn:" + function.PortWaitIn
                    + " PortWaitOut:" + function.PortWaitOut
                );

                if (function.IsCSTPresence == false)
                {
                    //scApp.TransferService.PortCstPositionOFF(function);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }

        private void Port_onAGVPortReadyStatusChange(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)
                if (function.AGVPortReady == true)
                {
                    //TODO
                    scApp.TransferService.PLC_AGV_Station(function, "AGVPortReady");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }
        private void Port_onCIM_ON(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)

                if (scApp.TransferService.GetIgnoreModeChange(function))
                {
                    return;
                }

                scApp.TransferService.PortCIM_ON(function, "Port_CIM_ON 訊號 ON_OFF");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }
        private void Port_onPreLoadOK(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)

                if (scApp.TransferService.GetIgnoreModeChange(function))
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }
        private void Port_onOutOfService(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)

                //if (scApp.TransferService.GetIgnoreModeChange(function))
                //{
                //    return;
                //}

                //if (function.OpManualMode == true || function.OpAutoMode == false)
                //{
                //    IsInService = false;
                //    scApp.TransferService.PLC_ReportPortInOutService(function);
                //}
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }

        private void Port_onInService(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)

                if (scApp.TransferService.GetIgnoreModeChange(function))
                {
                    return;
                }

                scApp.TransferService.PLC_ReportRunDwon(function, "PLC_RUN:" + function.OpAutoMode);

                if (function.OpAutoMode)
                {
                    IsInService = true;
                    scApp.TransferService.OHBC_AlarmCleared(function.EQ_ID, ((int)AlarmLst.PORT_DOWN).ToString());
                }
                else
                {
                    scApp.TransferService.OHBC_AlarmSet(function.EQ_ID, ((int)AlarmLst.PORT_DOWN).ToString());
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }

        private void PortInfoChange(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //發佈到Redis
                scApp.PortBLL.redis.SetPortInfo(port.PORT_ID, function);
                byte[] port_Serialize = PortBLL.convert2PortInfo(port.PORT_ID, function);
                scApp.getNatsManager().PublishAsync
                   (string.Format(SCAppConstants.NATS_SUBJECT_Port_INFO_0, port.PORT_ID.Trim()), port_Serialize);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }

        private void PublishVhInfo(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                string vh_id = e.PropertyValue as string;
                AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(vh_id);
                if (sender == null) return;
                byte[] vh_Serialize = BLL.VehicleBLL.Convert2GPB_VehicleInfo(vh);
                //RecoderVehicleObjInfoLog(vh_id, vh_Serialize);

                scApp.getNatsManager().PublishAsync
                    (string.Format(SCAppConstants.NATS_SUBJECT_VH_INFO_0, vh.VEHICLE_ID.Trim()), vh_Serialize);

                scApp.getRedisCacheManager().ListSetByIndexAsync
                    (SCAppConstants.REDIS_LIST_KEY_VEHICLES, vh.VEHICLE_ID, vh.ToString());
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
            //});
        }

        public virtual void doShareMemoryInit(BCFAppConstants.RUN_LEVEL runLevel)
        {
            try
            {
                switch (runLevel)
                {
                    case BCFAppConstants.RUN_LEVEL.ZERO:
                        initialValueRead();
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

        private void initialValueRead()
        {
            ValueRead vr = null;
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "WAIT_IN", out vr))
            {
                Port_onWaitIn(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "WAIT_OUT", out vr))
            {
                Port_onWaitOut(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "NOW_INPUT_MODE", out vr))
            {
                Port_onDirectionStatusChangeToInput(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "NOW_OUTPUT_MODE", out vr))
            {
                Port_onDirectionStatusChangeToOutput(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "BARCODE_READ_DONE", out vr))
            {
                Port_onBarcodeReadDone(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "CST_TRANSFER_COMPLETE", out vr))
            {
                Port_onTransferComplete(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "CST_REMOVE_CHECK", out vr))
            {
                Port_onCSTRemoveCheck(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "LOAD_POSITION_1", out vr))
            {
                Port_onLoadPosition(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "LOAD_POSITION_2", out vr))
            {
                Port_onLoadPosition2(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "LOAD_POSITION_3", out vr))
            {
                Port_onLoadPosition3(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "LOAD_POSITION_4", out vr))
            {
                Port_onLoadPosition4(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "LOAD_POSITION_5", out vr))
            {
                Port_onLoadPosition5(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "LOAD_POSITION_6", out vr))
            {
                Port_onLoadPosition6(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "LOAD_POSITION_7", out vr))
            {
                Port_onLoadPosition7(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "OP_RUN", out vr))
            {
                Port_onInService(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "OP_DOWN", out vr))
            {
                Port_onOutOfService(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "AGV_PORT_READY", out vr))
            {
                Port_onAGVPortReadyStatusChange(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "IS_CST_PRESENCE", out vr))
            {
                Port_onCstPresenceStatusChange(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "OP_ERROR", out vr))
            {
                Port_onAlarmHppened(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "IS_AGV_MODE", out vr))
            {
                Port_onChangeToAGVMode(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "IS_MGV_MODE", out vr))
            {
                Port_onChangeToMGVMode(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "CST_PRRESENCE_MISMATCH", out vr))
            {
                Port_onCSTPresenceMismatch(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "FIRE_ALARM", out vr))
            {
                Port_onFireAlarm(vr, null);
            }

            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "PORTALLINFO", out vr))
            {
                PortInfoChange(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "CIM_ON", out vr))
            {
                Port_onCIM_ON(vr, null);
            }
            if (bcfApp.tryGetReadValueEventstring(port.EqptObjectCate, port.PORT_ID, "PreLoadOK", out vr))
            {
                Port_onPreLoadOK(vr, null);
            }
        }

        public virtual string getIdentityKey()
        {
            return this.GetType().Name;
        }

        public virtual void setContext(BaseEQObject baseEQ)
        {
            this.port = baseEQ as APORT;
        }

        public virtual void unRegisterEvent()
        {
            //not implement
        }
        public virtual void Port_onModeChangable(object sender, ValueChangedEventArgs args)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)

                scApp.TransferService.TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "PLC >> OHB|Port_onWaitInOut"
                    + " PORT_ID:" + port.PORT_ID
                    + " Port_onModeChangable:" + function.IsModeChangable
                );

                if (scApp.TransferService.GetIgnoreModeChange(function))
                {
                    return;
                }

                if (function.IsModeChangable)
                {
                    Console.WriteLine("IsModeChangable");
                    //TODO: wait in

                    if (function.OpAutoMode)
                    {
                        scApp.TransferService.PLC_ReportPortIsModeChangable(function, "PLC");
                    }
                    else
                    {
                        scApp.TransferService.TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ") +
                            "PLC >> OHB|Port 狀態錯誤，不能報 IsModeChangable "
                            + " PORT_ID:" + port.PORT_ID
                            + " Run:" + function.OpAutoMode
                        );
                    }
                }

            }
            catch (Exception ex)
            {
                scApp.TransferService.TransferServiceLogger.Error(ex, "Port_onModeChangable");
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }
        public virtual void Port_onWaitIn(object sender, ValueChangedEventArgs args)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)

                scApp.TransferService.TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "PLC >> OHB|Port_onWaitInOut"
                    + " PORT_ID:" + port.PORT_ID
                    + " PortWaitIn:" + function.PortWaitIn
                );

                if (scApp.TransferService.GetIgnoreModeChange(function))
                {
                    return;
                }

                if (function.PortWaitIn)
                {
                    Console.WriteLine("wait in");
                    //TODO: wait in

                    if (function.OpAutoMode)
                    {
                        scApp.TransferService.PLC_ReportPortWaitIn(function, "PLC");
                    }
                    else
                    {
                        scApp.TransferService.TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ") +
                            "PLC >> OHB|Port 狀態錯誤，不能報 WaitIn "
                            + " PORT_ID:" + port.PORT_ID
                            + " Run:" + function.OpAutoMode
                        );
                    }
                }

            }
            catch (Exception ex)
            {
                scApp.TransferService.TransferServiceLogger.Error(ex, "Port_onWaitIn");
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }
        public virtual void Port_onWaitOut(object sender, ValueChangedEventArgs args)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)

                scApp.TransferService.TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "PLC >> OHB|Port_onWaitInOut"
                    + " PORT_ID:" + port.PORT_ID
                    + " PortWaitOut:" + function.PortWaitOut
                );

                if (scApp.TransferService.GetIgnoreModeChange(function))
                {
                    return;
                }

                if (function.PortWaitOut)
                {

                }
                else
                {
                    //if (scApp.TransferService.isUnitType(function.EQ_ID, Service.UnitType.AGV))
                    //{
                    //    CassetteData datainfo = new CassetteData();
                    //    datainfo.CSTID = function.CassetteID.Trim();        //填CSTID
                    //    datainfo.BOXID = function.BoxID.Trim();        //填BOXID
                    //    datainfo.Carrier_LOC = function.EQ_ID.Trim();  //填Port 名稱
                    //    scApp.TransferService.PortCarrierRemoved(datainfo, function.IsAGVMode, "PortWaitOutOFF");
                    //}
                }
            }
            catch (Exception ex)
            {
                scApp.TransferService.TransferServiceLogger.Error(ex, "Port_onWaitOut");
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }
        public virtual void Port_onDirectionStatusChangeToInput(object sender, ValueChangedEventArgs args)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)

                if (scApp.TransferService.GetIgnoreModeChange(function))
                {
                    return;
                }

                if (function.IsInputMode)
                {
                    scApp.TransferService.ReportPortType(function.EQ_ID, E_PortType.In, "PLC");

                    bool cstDelete = scApp.TransferService.portTypeChangeOK_CVPort_CstRemove;
                    string log = "PLC IsInputMode: " + function.IsInputMode.ToString();

                    if (cstDelete)
                    {
                        scApp.TransferService.DeleteOHCVPortCst(function.EQ_ID, log);
                    }

                    if (scApp.TransferService.isUnitType(function.EQ_ID, Service.UnitType.AGV))
                    {
                        scApp.TransferService.PLC_AGV_Station(function, log);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }

        public virtual void Port_onDirectionStatusChangeToOutput(object sender, ValueChangedEventArgs args)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)

                if (scApp.TransferService.GetIgnoreModeChange(function))
                {
                    return;
                }

                if (function.IsOutputMode)
                {
                    scApp.TransferService.ReportPortType(function.EQ_ID, E_PortType.Out, "PLC");

                    bool cstDelete = scApp.TransferService.portTypeChangeOK_CVPort_CstRemove;
                    string log = "PLC IsOutputMode:" + function.IsOutputMode.ToString();

                    if (cstDelete)
                    {
                        scApp.TransferService.DeleteOHCVPortCst(function.EQ_ID, log);
                    }

                    if (scApp.TransferService.isUnitType(function.EQ_ID, Service.UnitType.AGV))
                    {
                        scApp.TransferService.PLC_AGV_Station(function, log);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }

        public PortPLCInfo GetPortValue()
        {

            PortPLCInfo portData = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            portData.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
            return portData;
        }

        public void Port_onBarcodeReadDone(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)

                if (scApp.TransferService.GetIgnoreModeChange(function))
                {
                    return;
                }

                if (function.BCRReadDone)
                {
                    //get box ID here
                    //box ID is stored in function.BoxID
                    if (scApp.TransferService.isUnitType(function.EQ_ID, Service.UnitType.AGV))
                    {
                        scApp.TransferService.TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ") +
                            "PLC >> PLC|Port_onBarcodeReadDone  PortName: " + function.EQ_ID
                            + " BCRReadDone: " + function.BCRReadDone
                            + " CassetteID: " + function.CassetteID
                            + " BoxID: " + function.BoxID
                            + " IsReadyToLoad: " + function.IsReadyToLoad
                            + " IsReadyToUnload:" + function.IsReadyToUnload
                            + " IsCSTPresence:" + function.IsCSTPresence
                            + " PortWaitOut:" + function.PortWaitOut
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }

        private void Port_onCSTRemoveCheck(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)

                if (scApp.TransferService.GetIgnoreModeChange(function))
                {
                    return;
                }

                if (function.CstRemoveCheck)
                {
                    if (scApp.TransferService.isUnitType(function.EQ_ID, Service.UnitType.AGV))
                    {
                        CassetteData datainfo = new CassetteData();
                        datainfo.CSTID = function.CassetteID.Trim();        //填CSTID
                        datainfo.BOXID = function.BoxID.Trim();        //填BOXID
                        datainfo.Carrier_LOC = function.EQ_ID.Trim();  //填Port 名稱
                        scApp.TransferService.PortCarrierRemoved(datainfo, function.IsAGVMode, "PortRemove");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }

        private void Port_onTransferComplete(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)
                if (function.IsTransferComplete)
                {
                    //TODO
                    Console.WriteLine("wait out LP");
                    //TODO: wait out

                    //CassetteData datainfo = new CassetteData();
                    //datainfo.CSTID = function.CassetteID;        //填CSTID
                    //datainfo.BOXID = function.BoxID.Trim();        //填BOXID
                    //datainfo.Carrier_LOC = port.PORT_ID.Trim();  //填Port 名稱

                    //scApp.TransferService.PortTransferCompleted(datainfo);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }
        #region LoadPosition
        private void Port_onLoadPosition(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)

                scApp.TransferService.TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "PLC >> PLC|PortName: " + function.EQ_ID
                    + " LoadPosition1: " + function.LoadPosition1
                    + " LoadPositionBOX1:" + function.LoadPositionBOX1
                );

                if (scApp.TransferService.GetIgnoreModeChange(function))
                {
                    return;
                }

                if (function.OpAutoMode && function.IsInputMode && scApp.TransferService.isUnitType(port.PORT_ID, Service.UnitType.AGV))
                {
                    if (function.LoadPosition1)
                    {
                        CassetteData datainfo = new CassetteData();
                        datainfo.CSTID = function.CassetteID;        //填CSTID
                        datainfo.BOXID = function.LoadPositionBOX1.Trim();        //填BOXID
                        datainfo.Carrier_LOC = port.PORT_ID.Trim();  //填Port 名稱

                        scApp.TransferService.PortPositionWaitOut(datainfo, 1);
                    }
                }

                if (function.OpAutoMode && function.IsInputMode && function.LoadPosition1 == false)
                {
                    scApp.TransferService.PortToOHT(function.EQ_ID.Trim(), "LoadPosition1 OFF");
                }

                if (function.OpAutoMode && function.IsOutputMode)  //2020/2/18 Hsinyu Chang: input mode時忽略position資訊的更新
                {
                    CassetteData datainfo = new CassetteData();
                    datainfo.CSTID = function.CassetteID;        //填CSTID
                    datainfo.BOXID = function.LoadPositionBOX1.Trim();        //填BOXID
                    datainfo.Carrier_LOC = function.EQ_ID.Trim();  //填Port 名稱

                    if (function.LoadPosition1)
                    {
                        scApp.TransferService.PortPositionWaitOut(datainfo, 1);
                    }
                    else
                    {
                        scApp.TransferService.PortPositionOFF(function, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }
        private void Port_onLoadPosition2(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)

                scApp.TransferService.TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "PLC >> PLC|PortName: " + function.EQ_ID
                    + " LoadPosition2: " + function.LoadPosition2
                    + " LoadPositionBOX2:" + function.LoadPositionBOX2
                );

                if (scApp.TransferService.GetIgnoreModeChange(function))
                {
                    return;
                }

                if (function.IsOutputMode == true)  //2020/2/18 Hsinyu Chang: input mode時忽略position資訊的更新
                {
                    CassetteData datainfo = new CassetteData();
                    datainfo.CSTID = function.CassetteID;        //填CSTID
                    datainfo.BOXID = function.LoadPositionBOX2.Trim();        //填BOXID
                    datainfo.Carrier_LOC = port.PORT_ID.Trim();  //填Port 名稱

                    if (function.LoadPosition2)
                    {
                        scApp.TransferService.PortPositionWaitOut(datainfo, 2);
                    }
                    else
                    {
                        scApp.TransferService.PortPositionOFF(function, 2);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }
        private void Port_onLoadPosition3(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)

                scApp.TransferService.TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "PLC >> PLC|PortName: " + function.EQ_ID
                    + " LoadPosition3: " + function.LoadPosition3
                    + " LoadPositionBOX3:" + function.LoadPositionBOX3
                );

                if (scApp.TransferService.GetIgnoreModeChange(function))
                {
                    return;
                }

                if (function.IsOutputMode == true)  //2020/2/18 Hsinyu Chang: input mode時忽略position資訊的更新
                {
                    CassetteData datainfo = new CassetteData();
                    datainfo.CSTID = function.CassetteID;        //填CSTID
                    datainfo.BOXID = function.LoadPositionBOX3.Trim();        //填BOXID
                    datainfo.Carrier_LOC = port.PORT_ID.Trim();  //填Port 名稱

                    if (function.LoadPosition3)
                    {
                        scApp.TransferService.PortPositionWaitOut(datainfo, 3);
                    }
                    else
                    {
                        scApp.TransferService.PortPositionOFF(function, 3);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }
        private void Port_onLoadPosition4(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)

                scApp.TransferService.TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "PLC >> PLC|PortName: " + function.EQ_ID
                    + " LoadPosition4: " + function.LoadPosition4
                    + " LoadPositionBOX4:" + function.LoadPositionBOX4
                );

                if (scApp.TransferService.GetIgnoreModeChange(function))
                {
                    return;
                }

                if (function.IsOutputMode == true)  //2020/2/18 Hsinyu Chang: input mode時忽略position資訊的更新
                {
                    CassetteData datainfo = new CassetteData();
                    datainfo.CSTID = function.CassetteID;        //填CSTID
                    datainfo.BOXID = function.LoadPositionBOX4.Trim();        //填BOXID
                    datainfo.Carrier_LOC = port.PORT_ID.Trim();  //填Port 名稱

                    if (function.LoadPosition4)
                    {
                        scApp.TransferService.PortPositionWaitOut(datainfo, 4);
                    }
                    else
                    {
                        scApp.TransferService.PortPositionOFF(function, 4);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }
        private void Port_onLoadPosition5(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)

                scApp.TransferService.TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "PLC >> PLC|PortName: " + function.EQ_ID
                    + " LoadPosition5: " + function.LoadPosition5
                    + " LoadPositionBOX5:" + function.LoadPositionBOX5
                );

                if (scApp.TransferService.GetIgnoreModeChange(function))
                {
                    return;
                }

                if (function.IsOutputMode == true)  //2020/2/18 Hsinyu Chang: input mode時忽略position資訊的更新
                {
                    CassetteData datainfo = new CassetteData();
                    datainfo.CSTID = function.CassetteID;        //填CSTID
                    datainfo.BOXID = function.LoadPositionBOX5.Trim();        //填BOXID
                    datainfo.Carrier_LOC = port.PORT_ID.Trim();  //填Port 名稱

                    if (function.LoadPosition5)
                    {
                        scApp.TransferService.PortPositionWaitOut(datainfo, 5);
                    }
                    else
                    {
                        scApp.TransferService.PortPositionOFF(function, 5);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }
        private void Port_onLoadPosition6(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)

                scApp.TransferService.TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "PLC >> PLC|PortName: " + function.EQ_ID
                    + " LoadPosition6: " + function.LoadPosition6
                );

                if (scApp.TransferService.GetIgnoreModeChange(function))
                {
                    return;
                }

                if (function.IsOutputMode == true)  //2020/2/18 Hsinyu Chang: input mode時忽略position資訊的更新
                {
                    CassetteData datainfo = new CassetteData();
                    datainfo.CSTID = function.CassetteID;        //填CSTID
                    datainfo.BOXID = function.BoxID.Trim();        //填BOXID
                    datainfo.Carrier_LOC = port.PORT_ID.Trim();  //填Port 名稱

                    if (function.LoadPosition6)
                    {
                        scApp.TransferService.PortPositionWaitOut(datainfo, 6);
                    }
                    else
                    {
                        scApp.TransferService.PortPositionOFF(function, 6);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }
        private void Port_onLoadPosition7(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<PortPLCInfo>(port.PORT_ID) as PortPLCInfo;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, port.EqptObjectCate, port.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)

                scApp.TransferService.TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "PLC >> PLC|PortName: " + function.EQ_ID
                    + " LoadPosition7: " + function.LoadPosition7
                );

                if (scApp.TransferService.GetIgnoreModeChange(function))
                {
                    return;
                }

                if (function.IsOutputMode == true)  //2020/2/18 Hsinyu Chang: input mode時忽略position資訊的更新
                {
                    CassetteData datainfo = new CassetteData();
                    datainfo.CSTID = function.CassetteID;        //填CSTID
                    datainfo.BOXID = function.BoxID.Trim();        //填BOXID
                    datainfo.Carrier_LOC = port.PORT_ID.Trim();  //填Port 名稱

                    if (function.LoadPosition7)
                    {
                        scApp.TransferService.PortPositionWaitOut(datainfo, 7);
                    }
                    else
                    {
                        scApp.TransferService.PortPositionOFF(function, 7);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCInfo>(function);
            }
        }
        #endregion

        public void Port_WriteBoxCstID(CassetteData cassetteData)
        {
            var function = scApp.getFunBaseObj<PortPLCControl_CSTID_BOXID>(port.PORT_ID) as PortPLCControl_CSTID_BOXID;
            try
            {
                //1.建立各個Function物件
                function.AssignBoxID = SCUtility.Trim(cassetteData.BOXID, true);

                if (function.AssignBoxID.Contains("UNK"))
                {
                    function.AssignBoxID = "ERROR1";
                }

                function.AssignCassetteID = SCUtility.Trim(cassetteData.CSTID, true);

                if (function.AssignCassetteID.Contains("UNK"))
                {
                    function.AssignCassetteID = "ERROR1";
                }

                function.Write(bcfApp, port.EqptObjectCate, port.PORT_ID);
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
                scApp.putFunBaseObj<PortPLCControl_CSTID_BOXID>(function);
            }
        }

        #region OHB >> PLC
        public void Port_ChangeToInput(bool isInput)
        {
            //var function = scApp.getFunBaseObj<PortPLCControl>(port.PORT_ID) as PortPLCControl;
            var function = scApp.getFunBaseObj<PortPLCControl_PortInOutModeChange>(port.PORT_ID) as PortPLCControl_PortInOutModeChange;
            try
            {
                //1.建立各個Function物件
                function.PortToInputMode = isInput;
                function.Write(bcfApp, port.EqptObjectCate, port.PORT_ID);
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
                //scApp.putFunBaseObj<PortPLCControl>(function);
                scApp.putFunBaseObj<PortPLCControl_PortInOutModeChange>(function);
            }
        }

        public void Port_ChangeToOutput(bool isOutput)
        {
            //var function = scApp.getFunBaseObj<PortPLCControl>(port.PORT_ID) as PortPLCControl;
            var function = scApp.getFunBaseObj<PortPLCControl_PortInOutModeChange>(port.PORT_ID) as PortPLCControl_PortInOutModeChange;
            try
            {
                //1.建立各個Function物件
                function.PortToOutputMode = isOutput;
                function.Write(bcfApp, port.EqptObjectCate, port.PORT_ID);
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
                //scApp.putFunBaseObj<PortPLCControl>(function);
                scApp.putFunBaseObj<PortPLCControl_PortInOutModeChange>(function);
            }
        }
        public void Port_ChangeToOutputTEST(bool isOutput)  //PLC單點控制，參考
        {
            try
            {
                ValueWrite out_mode = bcfApp.getWriteValueEvent(port.EqptObjectCate, port.PORT_ID, "CHANGE_TO_OUTPUT_MODE");
                out_mode.setWriteValue(isOutput ? "1" : "0");
                ISMControl.writeDeviceBlock(bcfApp, out_mode);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }

        public void Port_OHCV_Commanding(bool isCmd)    //D6401(?) OHCV Commanding，通知有卡匣要搬過去，流向不能變更意思
        {
            //var function = scApp.getFunBaseObj<PortPLCControl>(port.PORT_ID) as PortPLCControl;
            var function = scApp.getFunBaseObj<PortPLCControl_VehicleCommanding1>(port.PORT_ID) as PortPLCControl_VehicleCommanding1;
            try
            {
                //1.建立各個Function物件
                function.VehicleCommanding1 = isCmd;
                function.Write(bcfApp, port.EqptObjectCate, port.PORT_ID);
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
                //scApp.putFunBaseObj<PortPLCControl>(function);
                scApp.putFunBaseObj<PortPLCControl_VehicleCommanding1>(function);
            }
        }
        #region AGV 專有
        public void Port_ToggleBoxCover(bool open)
        {
            var function = scApp.getFunBaseObj<PortPLCControl_AGV_OpenBOX>(port.PORT_ID) as PortPLCControl_AGV_OpenBOX;
            try
            {
                //1.建立各個Function物件
                function.ToggleBoxCover = open;
                function.Write(bcfApp, port.EqptObjectCate, port.PORT_ID);
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
                scApp.putFunBaseObj<PortPLCControl_AGV_OpenBOX>(function);
            }
        }
        public void Port_BCR_Read(bool read)
        {
            var function = scApp.getFunBaseObj<PortPLCControl_AGV_BCR_Read>(port.PORT_ID) as PortPLCControl_AGV_BCR_Read;
            try
            {
                //1.建立各個Function物件
                function.PortIDRead = read;

                function.Write(bcfApp, port.EqptObjectCate, port.PORT_ID);
                function.Timestamp = DateTime.Now;

                //2.write log
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //3.logical (include db save)
                //Port_RstBCR_Read();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCControl_AGV_BCR_Read>(function);
            }
        }
        public void Port_BCR_Enable(bool enable)
        {
            var function = scApp.getFunBaseObj<PortPLCControl_AGV_BCR_Enable>(port.PORT_ID) as PortPLCControl_AGV_BCR_Enable;
            try
            {
                //1.建立各個Function物件
                function.PortBCR_Enable = enable;

                function.Write(bcfApp, port.EqptObjectCate, port.PORT_ID);
                function.Timestamp = DateTime.Now;

                //2.write log
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //3.logical (include db save)
                //Port_RstBCR_Read();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<PortPLCControl_AGV_BCR_Enable>(function);
            }
        }
        public void Port_ChangeToAGVMode()
        {
            var function = scApp.getFunBaseObj<PortPLCControl_AGV_AGVmode>(port.PORT_ID) as PortPLCControl_AGV_AGVmode;
            try
            {
                //1.建立各個Function物件
                function.ToMGVMode = false;
                function.ToAGVMode = true;
                function.Write(bcfApp, port.EqptObjectCate, port.PORT_ID);
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
                scApp.putFunBaseObj<PortPLCControl_AGV_AGVmode>(function);
            }
        }
        public void Port_ChangeToMGVMode()
        {
            var function = scApp.getFunBaseObj<PortPLCControl_AGV_AGVmode>(port.PORT_ID) as PortPLCControl_AGV_AGVmode;
            try
            {
                //1.建立各個Function物件
                function.ToAGVMode = false;
                function.ToMGVMode = true; ;
                function.Write(bcfApp, port.EqptObjectCate, port.PORT_ID);
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
                scApp.putFunBaseObj<PortPLCControl_AGV_AGVmode>(function);
            }
        }
        #endregion

        public void Port_RUN()
        {
            var function = scApp.getFunBaseObj<PortPLCControl_PortRunStop>(port.PORT_ID) as PortPLCControl_PortRunStop;
            try
            {
                //1.建立各個Function物件
                function.PortManual = false;
                function.PortAuto = true;

                function.Write(bcfApp, port.EqptObjectCate, port.PORT_ID);
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
                scApp.putFunBaseObj<PortPLCControl_PortRunStop>(function);
            }
        }
        public void Port_STOP()
        {
            var function = scApp.getFunBaseObj<PortPLCControl_PortRunStop>(port.PORT_ID) as PortPLCControl_PortRunStop;
            try
            {
                //1.建立各個Function物件
                function.PortAuto = false;
                function.PortManual = true;

                function.Write(bcfApp, port.EqptObjectCate, port.PORT_ID);
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
                scApp.putFunBaseObj<PortPLCControl_PortRunStop>(function);
            }
        }
        public void Port_PortAlarrmReset(bool reset)
        {
            var function = scApp.getFunBaseObj<PortPLCControl_AlarmReset>(port.PORT_ID) as PortPLCControl_AlarmReset;
            try
            {
                //1.建立各個Function物件
                function.PortReset = reset;

                function.Write(bcfApp, port.EqptObjectCate, port.PORT_ID);
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
                scApp.putFunBaseObj<PortPLCControl_AlarmReset>(function);
            }
        }
        #endregion
    }
}
