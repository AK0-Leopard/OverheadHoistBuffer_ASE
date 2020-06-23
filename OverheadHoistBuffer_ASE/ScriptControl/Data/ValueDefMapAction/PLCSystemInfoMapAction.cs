using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.ValueDefMapAction
{
    class PLCSystemInfoMapAction : IValueDefMapAction
    {
        public const string DEVICE_NAME_PORT = "MASTER_PLC";
        private Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected APORT masterPLC = null;

        protected SCApplication scApp = null;
        protected BCFApplication bcfApp = null;

        public bool PLCHeartbeatSignal { get; private set; }

        public PLCSystemInfoMapAction() : base()
        {
            scApp = SCApplication.getInstance();
            bcfApp = scApp.getBCFApplication();
        }

        public virtual void doInit()
        {
            try
            {
                ValueRead vr = null;
                if (bcfApp.tryGetReadValueEventstring(masterPLC.EqptObjectCate, masterPLC.PORT_ID, "PLC_HEARTBEAT", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => PLCSystem_onHeartBeat(_sender, _e);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        private void PLCSystem_onHeartBeat(object sender, ValueChangedEventArgs e)
        {
            //Hsinyu Chang 20200508 TODO: enable it
            var function = scApp.getFunBaseObj<OHxCToPLC_SetSystemInfo_SetAlive>(masterPLC.PORT_ID) as OHxCToPLC_SetSystemInfo_SetAlive;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, masterPLC.EqptObjectCate, masterPLC.PORT_ID);
                //2.read log
                //function.Timestamp = DateTime.Now;
                //LogManager.GetLogger("com.mirle.ibg3k0.sc.Common.LogHelper").Info(function.ToString());
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQStatusReport), Device: DEVICE_NAME_MTL,
                //    XID: eqpt.EQPT_ID, Data: function.ToString());
                //3.logical (include db save)
                //if (function.PLCHeartBeat == false)
                //{
                //    function.PLCHeartBeat = true;
                //    function.Write(bcfApp, masterPLC.EqptObjectCate, masterPLC.PORT_ID);
                //    function.Timestamp = DateTime.Now;

                //    //write log
                //    NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
                //}
                PLCHeartbeatSignal = function.PLCHeartBeat;
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<OHxCToPLC_SetSystemInfo_SetAlive>(function);
            }
        }

        public void PLC_SetHeartbeat(bool aliveSignal)
        {
            var function = scApp.getFunBaseObj<OHxCToPLC_SetSystemInfo_SetAlive>(masterPLC.PORT_ID) as OHxCToPLC_SetSystemInfo_SetAlive;
            try
            {
                //1.建立各個Function物件
                function.PLCHeartBeat = aliveSignal;
                function.Write(bcfApp, masterPLC.EqptObjectCate, masterPLC.PORT_ID);
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
                scApp.putFunBaseObj<OHxCToPLC_SetSystemInfo_SetAlive>(function);
            }
        }

        public void PLC_SetMCSOnline(bool online)
        {
            var function = scApp.getFunBaseObj<OHxCToPLC_SetSystemInfo_SetMCSOnline>(masterPLC.PORT_ID) as OHxCToPLC_SetSystemInfo_SetMCSOnline;
            try
            {
                //1.建立各個Function物件
                function.MCSOnline = online;
                function.Write(bcfApp, masterPLC.EqptObjectCate, masterPLC.PORT_ID);
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
                scApp.putFunBaseObj<OHxCToPLC_SetSystemInfo_SetMCSOnline>(function);
            }
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
            if (bcfApp.tryGetReadValueEventstring(masterPLC.EqptObjectCate, masterPLC.PORT_ID, "PLC_HEARTBEAT", out vr))
            {
                PLCSystem_onHeartBeat(vr, null);
            }
        }

        public virtual string getIdentityKey()
        {
            return this.GetType().Name;
        }

        public virtual void setContext(BaseEQObject baseEQ)
        {
            this.masterPLC = baseEQ as APORT;
        }

        public virtual void unRegisterEvent()
        {
            //not implement
        }

        public void PLC_SetSystemTime()
        {
            var function = scApp.getFunBaseObj<OHxCToPLC_SetSystemInfo_SetTime>(masterPLC.PORT_ID) as OHxCToPLC_SetSystemInfo_SetTime;
            try
            {
                //1.建立各個Function物件
                function.bcdYearMonth = (UInt16)(intToBCD((DateTime.Now.Year) - 2000) * 256 + intToBCD(DateTime.Now.Month));
                function.bcdDayHour = (UInt16)(intToBCD(DateTime.Now.Day) * 256 + intToBCD(DateTime.Now.Hour));
                function.bcdMinuteSecond = (UInt16)(intToBCD(DateTime.Now.Minute) * 256 + intToBCD(DateTime.Now.Second));
                function.setTime = true;
                function.Write(bcfApp, masterPLC.EqptObjectCate, masterPLC.PORT_ID);
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
                scApp.putFunBaseObj<OHxCToPLC_SetSystemInfo_SetTime>(function);
            }
        }

        public void PLC_FinishTimeCalibration()
        {
            var function = scApp.getFunBaseObj<OHxCToPLC_SetSystemInfo_SetTime>(masterPLC.PORT_ID) as OHxCToPLC_SetSystemInfo_SetTime;
            try
            {
                //1.建立各個Function物件
                function.setTime = false;
                function.Write(bcfApp, masterPLC.EqptObjectCate, masterPLC.PORT_ID);
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
                scApp.putFunBaseObj<OHxCToPLC_SetSystemInfo_SetTime>(function);
            }
        }

        private UInt16 intToBCD(int value)
        {
            if (value < 0 || value > 99)
            {
                return 0;
            }
            UInt16 bcd = (UInt16)(value / 10 * 16 + value % 10);
            return bcd;
        }
    }
}
