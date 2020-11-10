//*********************************************************************************
//      EQType2SecsMapAction.cs
//*********************************************************************************
// File Name: EQType2SecsMapAction.cs
// Description: Type2 EQ Map Action
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Data.TcpIp;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using com.mirle.iibg3k0.ttc;
using com.mirle.iibg3k0.ttc.Common;
using com.mirle.iibg3k0.ttc.Common.TCPIP;
using KingAOP;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using static com.mirle.ibg3k0.sc.Data.PLC_Functions.VehicleCSTInterface;

namespace com.mirle.ibg3k0.sc.Data.ValueDefMapAction
{
    /// <summary>
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.sc.Data.ValueDefMapAction.ValueDefMapActionBase" />
    public class EQTcpIpMapAction : ValueDefMapActionBase, IDynamicMetaObjectProvider
    {

        string tcpipAgentName = string.Empty;
        protected Logger logger_PLCConverLog;

        /// <summary>
        /// Initializes a new instance of the <see cref="EQType2SecsMapAction"/> class.
        /// </summary>
        public EQTcpIpMapAction()
            : base()
        {

        }
        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new AspectWeaver(parameter, this);
        }

        /// <summary>
        /// Gets the identity key.
        /// </summary>
        /// <returns>System.String.</returns>
        public override string getIdentityKey()
        {
            return this.GetType().Name;
        }
        //protected AVEHICLE eqpt = null;

        /// <summary>
        /// Sets the context.
        /// </summary>
        /// <param name="baseEQ">The base eq.</param>
        public override void setContext(BaseEQObject baseEQ)
        {
            this.eqpt = baseEQ as AVEHICLE;

        }
        /// <summary>
        /// Uns the register event.
        /// </summary>
        public override void unRegisterEvent()
        {
            //not implement
        }
        /// <summary>
        /// Does the share memory initialize.
        /// </summary>
        /// <param name="runLevel">The run level.</param>
        public override void doShareMemoryInit(BCFAppConstants.RUN_LEVEL runLevel)
        {
            try
            {
                switch (runLevel)
                {
                    case BCFAppConstants.RUN_LEVEL.ZERO:
                        if (eqpt != null)
                        {
                            if (!SCUtility.isEmpty(eqpt.OHTC_CMD))
                            {
                                ACMD_OHTC aCMD_OHTC = scApp.CMDBLL.getExcuteCMD_OHTCByCmdID(eqpt.OHTC_CMD);
                                string[] PredictPath = scApp.CMDBLL.loadPassSectionByCMDID(eqpt.OHTC_CMD);
                                scApp.CMDBLL.setVhExcuteCmdToShow(aCMD_OHTC, this.eqpt, PredictPath, null, null, null);
                            }
                        }
                        if (eqpt.IS_INSTALLED)
                            eqpt.VehicleInstall();

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

        protected void onStateChange_Initial()
        {

        }

        object str132_lockObj = new object();
        protected void str132_Receive(object sender, TcpIpEventArgs e)
        {
            if (scApp.getEQObjCacheManager().getLine().ServerPreStop)
                return;

            int threadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Stopwatch sw = scApp.StopwatchPool.GetObject();
            string lockTraceInfo = string.Format("VH ID:{0},Pack ID:{1},Seq Num:{2},ThreadID:{3},"
                , eqpt.VEHICLE_ID
                , e.iPacketID
                , e.iSeqNum.ToString()
                , threadID.ToString());
            try
            {
                sw.Start();
                LogManager.GetLogger("LockInfo").Debug(string.Concat(lockTraceInfo, "Wait Lock In"));
                //TODO Wait Lock In
                SCUtility.LockWithTimeout(str132_lockObj, SCAppConstants.LOCK_TIMEOUT_MS, str132_ReceiveProcess, sender, e);
                //Lock Out
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("LockInfo").Error(string.Concat(lockTraceInfo, "Lock Exception"));
                logger.Error(ex, "(str132_Receive) Exception");
            }
            finally
            {
                long ElapsedMilliseconds = sw.ElapsedMilliseconds;
                LogManager.GetLogger("LockInfo").Debug(string.Concat(lockTraceInfo, "Lock Out. ElapsedMilliseconds:", ElapsedMilliseconds.ToString()));
                scApp.StopwatchPool.PutObject(sw);
            }
        }
        protected void str132_ReceiveProcess(object sender, TcpIpEventArgs e)
        {
            ID_132_TRANS_COMPLETE_REPORT recive_str = (ID_132_TRANS_COMPLETE_REPORT)e.objPacket;
            scApp.VehicleBLL.setAndPublishPositionReportInfo2Redis(eqpt.VEHICLE_ID, recive_str);

            dynamic service = scApp.VehicleService;
            service.CommandCompleteReport(tcpipAgentName, bcfApp, eqpt, recive_str, e.iSeqNum);
        }



        protected void str134_Receive(object sender, TcpIpEventArgs e)
        {
            if (scApp.getEQObjCacheManager().getLine().ServerPreStop)
                return;
            try
            {
                connectionCheck(eqpt);
                str134_ReceiveProcess(sender, e);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "(str134_Receive) Exception");
            }
        }

        const int IGNORE_SECTION_DISTANCE = 60;
        protected void str134_ReceiveProcess(object sender, TcpIpEventArgs e)
        {
            ID_134_TRANS_EVENT_REP recive_str = (ID_134_TRANS_EVENT_REP)e.objPacket;
            SCUtility.RecodeReportInfo(eqpt.VEHICLE_ID, 0, recive_str);
            scApp.VehicleBLL.setAndPublishPositionReportInfo2Redis(eqpt.VEHICLE_ID, recive_str);
        }



        object str136_lockObj = new object();
        protected void str136_Receive(object sender, TcpIpEventArgs e)
        {

            if (scApp.getEQObjCacheManager().getLine().ServerPreStop)
                return;
            connectionCheck(eqpt);
            int threadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Stopwatch sw = scApp.StopwatchPool.GetObject();
            string lockTraceInfo = string.Format("VH ID:{0},Pack ID:{1},Seq Num:{2},ThreadID:{3},"
                , eqpt.VEHICLE_ID
                , e.iPacketID
                , e.iSeqNum.ToString()
                , threadID.ToString());
            try
            {
                sw.Start();
                LogManager.GetLogger("LockInfo").Debug(string.Concat(lockTraceInfo, "Wait Lock In"));
                //TODO Wait Lock In
                SCUtility.LockWithTimeout(str136_lockObj, SCAppConstants.LOCK_TIMEOUT_MS, str136_ReceiveProcess, sender, e);
                //Lock Out
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("LockInfo").Error(string.Concat(lockTraceInfo, "Lock Exception"));
                logger.Error(ex, "(str136_Receive) Exception");
            }
            finally
            {
                long ElapsedMilliseconds = sw.ElapsedMilliseconds;
                LogManager.GetLogger("LockInfo").Debug(string.Concat(lockTraceInfo, "Lock Out. ElapsedMilliseconds:", ElapsedMilliseconds.ToString()));
                scApp.StopwatchPool.PutObject(sw);
            }
        }
        protected void str136_ReceiveProcess(object sender, TcpIpEventArgs e)
        {
            //dynamic service = scApp.BlockControlServer;
            dynamic service = scApp.VehicleService;
            ID_136_TRANS_EVENT_REP recive_str = (ID_136_TRANS_EVENT_REP)e.objPacket;
            switch (recive_str.EventType)
            {
                case EventType.BlockRelease:
                case EventType.BlockReq:
                    break;
                default:
                    scApp.VehicleBLL.setAndPublishPositionReportInfo2Redis(eqpt.VEHICLE_ID, recive_str);
                    break;
            }
            service.TranEventReport(bcfApp, eqpt, recive_str, e.iSeqNum);
        }

        object str144_lockObj = new object();
        protected void str144_Receive(object sender, TcpIpEventArgs e)
        {

            if (scApp.getEQObjCacheManager().getLine().ServerPreStop)
                return;

            int threadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Stopwatch sw = scApp.StopwatchPool.GetObject();
            string lockTraceInfo = string.Format("VH ID:{0},Pack ID:{1},Seq Num:{2},ThreadID:{3},"
                , eqpt.VEHICLE_ID
                , e.iPacketID
                , e.iSeqNum.ToString()
                , threadID.ToString());
            connectionCheck(eqpt);
            try
            {
                sw.Start();
                LogManager.GetLogger("LockInfo").Debug(string.Concat(lockTraceInfo, "Wait Lock In"));
                //TODO Wait Lock In
                SCUtility.LockWithTimeout(str144_lockObj, SCAppConstants.LOCK_TIMEOUT_MS, str144_ReceiveProcess, sender, e);
                //Lock Out
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("LockInfo").Error(string.Concat(lockTraceInfo, "Lock Exception"));
                logger.Error(ex, "(str144_Receive) Exception");
            }
            finally
            {
                long ElapsedMilliseconds = sw.ElapsedMilliseconds;
                LogManager.GetLogger("LockInfo").Debug(string.Concat(lockTraceInfo, "Lock Out. ElapsedMilliseconds:", ElapsedMilliseconds.ToString()));
                scApp.StopwatchPool.PutObject(sw);
            }
        }

        protected void str144_ReceiveProcess(object sender, TcpIpEventArgs e)
        {
            dynamic service = scApp.VehicleService;
            ID_144_STATUS_CHANGE_REP recive_str = (ID_144_STATUS_CHANGE_REP)e.objPacket;

            scApp.VehicleBLL.setAndPublishPositionReportInfo2Redis(eqpt.VEHICLE_ID, recive_str);


            service.StatusReport(bcfApp, eqpt, recive_str, e.iSeqNum);

        }

        //todo 需掛上實際資料
        protected void str194_Receive(object sender, TcpIpEventArgs e)
        {
            ID_194_ALARM_REPORT recive_gpp = (ID_194_ALARM_REPORT)e.objPacket;

            dynamic service = scApp.VehicleService;
            service.AlarmReport(bcfApp, eqpt, recive_gpp, e.iSeqNum);

        }
        private void connectionCheck(AVEHICLE vh)
        {
            if (!vh.isTcpIpConnect)
            {
                vh.isTcpIpConnect = true;
                BCFApplication.onWarningMsg($"vh:{vh.VEHICLE_ID} Force change connection status to connection !");
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQTcpIpMapAction), Device: "AGVC",
                   Data: "Force change connection status to connection !",
                   VehicleID: vh.VEHICLE_ID,
                   CarrierID: vh.CST_ID);
            }
        }

        private static TransactionScope BegingTransaction()
        {
            TransactionScope tx = new TransactionScope
                (TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = SCAppConstants.ISOLATION_LEVEL });
            return tx;
        }

        private void whenObstacleFinish()
        {
            AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(eqpt.VEHICLE_ID);
            if (eqpt.ObstacleStatus == VhStopSingle.StopSingleOff &&
                !SCUtility.isEmpty(vh.MCS_CMD))
            {
                double OCSTime_ms = eqpt.watchObstacleTime.ElapsedMilliseconds;
                double OCSTime_s = OCSTime_ms / 1000;
                OCSTime_s = Math.Round(OCSTime_s, 1);
                if (eqpt.HAS_CST == 0)
                {
                    scApp.SysExcuteQualityBLL.updateSysExecQity_OCSTime2SurceOnTheWay(vh.MCS_CMD, OCSTime_s);
                }
                else
                {
                    scApp.SysExcuteQualityBLL.updateSysExecQity_OCSTime2DestnOnTheWay(vh.MCS_CMD, OCSTime_s);
                }
            }
        }
        private void whenBlockFinish()
        {
            AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(eqpt.VEHICLE_ID);
            if (eqpt.BlockingStatus == VhStopSingle.StopSingleOff &&
                !SCUtility.isEmpty(vh.MCS_CMD))
            {
                double BlockTime_ms = eqpt.watchBlockTime.ElapsedMilliseconds;
                double BlockTime_s = BlockTime_ms / 1000;
                BlockTime_s = Math.Round(BlockTime_s, 1);
                if (eqpt.HAS_CST == 0)
                {
                    scApp.SysExcuteQualityBLL.
                        updateSysExecQity_BlockTime2SurceOnTheWay(vh.MCS_CMD, BlockTime_s);
                }
                else
                {
                    scApp.SysExcuteQualityBLL.
                        updateSysExecQity_BlockTime2DestnOnTheWay(vh.MCS_CMD, BlockTime_s);
                }
            }
        }
        private void whenPauseFinish()
        {
            AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(eqpt.VEHICLE_ID);
            if (eqpt.PauseStatus == VhStopSingle.StopSingleOff &&
                !SCUtility.isEmpty(vh.MCS_CMD))
            {
                double PauseTime_ms = eqpt.watchPauseTime.ElapsedMilliseconds;
                double PauseTime_s = PauseTime_ms / 1000;
                PauseTime_s = Math.Round(PauseTime_s, 1);
                scApp.SysExcuteQualityBLL.updateSysExecQity_PauseTime(vh.MCS_CMD, PauseTime_s);
            }
        }




        public override bool send_Str31(ID_31_TRANS_REQUEST send_gpp, out ID_131_TRANS_RESPONSE receive_gpp, out string reason)
        {
            bool isSuccess = false;
            try
            {

                WrapperMessage wrapper = new WrapperMessage
                {
                    ID = VHMSGIF.ID_TRANS_REQUEST,
                    TransReq = send_gpp
                };
                com.mirle.iibg3k0.ttc.Common.TrxTcpIp.ReturnCode result = snedRecv(wrapper, out receive_gpp, out reason);
                isSuccess = result == TrxTcpIp.ReturnCode.Normal;
                reason = receive_gpp.NgReason;
                if (isSuccess)
                    isSuccess = receive_gpp.ReplyCode == 0;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                receive_gpp = null;
                reason = "命令下達時發生錯誤!";
            }
            return isSuccess;
        }

        public override bool send_Str37(string cmd_id, CMDCancelType actType)
        {
            //加入StackTrace，來找出他會下達Cancel的入口 by Kevin
            try
            {
                StackTrace st = new StackTrace(true);
                string trace_msg = SCUtility.ShowCallerInfo(st, $"Call EQTcpIpMapAction.send_Str37(),cmd id:{cmd_id},act type:{actType}");
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(EQTcpIpMapAction), Device: "OHxC",
                   Data: trace_msg,
                   VehicleID: eqpt.VEHICLE_ID,
                   Details: st.ToString(),
                   CarrierID: eqpt.CST_ID);
            }
            catch { }
            bool isScuess = false;
            try
            {

                string rtnMsg = string.Empty;
                ID_37_TRANS_CANCEL_REQUEST stSend;
                ID_137_TRANS_CANCEL_RESPONSE stRecv;
                stSend = new ID_37_TRANS_CANCEL_REQUEST()
                {
                    CmdID = cmd_id,
                    ActType = actType
                };

                WrapperMessage wrapper = new WrapperMessage
                {
                    ID = VHMSGIF.ID_TRANS_CANCEL_REQUEST,
                    TransCancelReq = stSend
                };

                SCUtility.RecodeReportInfo(eqpt.VEHICLE_ID, 0, stSend);
                com.mirle.iibg3k0.ttc.Common.TrxTcpIp.ReturnCode result = snedRecv(wrapper, out stRecv, out rtnMsg);
                SCUtility.RecodeReportInfo(eqpt.VEHICLE_ID, 0, stRecv, result.ToString());
                if (result == TrxTcpIp.ReturnCode.Normal)
                {
                    if (stRecv.ReplyCode == 0)
                    {
                        isScuess = true;
                    }
                    else
                    {
                        isScuess = false;
                    }
                }
                else
                {
                    isScuess = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            return isScuess;
        }

        public override bool send_Str39(ID_39_PAUSE_REQUEST send_gpp, out ID_139_PAUSE_RESPONSE receive_gpp)
        {
            bool isScuess = false;
            try
            {
                string rtnMsg = string.Empty;
                WrapperMessage wrapper = new WrapperMessage
                {
                    ID = VHMSGIF.ID_PAUSE_REQUEST,
                    PauseReq = send_gpp
                };
                com.mirle.iibg3k0.ttc.Common.TrxTcpIp.ReturnCode result = snedRecv(wrapper, out receive_gpp, out rtnMsg);
                isScuess = result == TrxTcpIp.ReturnCode.Normal &&
                           receive_gpp.ReplyCode == 0;


            }
            catch (Exception ex)
            {
                receive_gpp = null;
                logger.Error(ex, "Exception");
            }
            return isScuess;

        }

        public override bool send_Str43(ID_43_STATUS_REQUEST send_gpp, out ID_143_STATUS_RESPONSE receive_gpp)
        {
            bool isScuess = false;
            try
            {
                string rtnMsg = string.Empty;
                WrapperMessage wrapper = new WrapperMessage
                {
                    ID = VHMSGIF.ID_PAUSE_REQUEST,
                    StatusReq = send_gpp
                };
                com.mirle.iibg3k0.ttc.Common.TrxTcpIp.ReturnCode result = snedRecv(wrapper, out receive_gpp, out rtnMsg);
                isScuess = result == TrxTcpIp.ReturnCode.Normal;
            }
            catch (Exception ex)
            {
                receive_gpp = null;
                logger.Error(ex, "Exception");
            }
            return isScuess;

        }

        public override bool send_Str71(ID_71_RANGE_TEACHING_REQUEST send_gpp, out ID_171_RANGE_TEACHING_RESPONSE receive_gpp)
        {
            bool isScuess = false;
            try
            {
                string rtnMsg = string.Empty;
                WrapperMessage wrapper = new WrapperMessage
                {
                    ID = VHMSGIF.ID_SECTION_TEACH_REQUEST,
                    RangeTeachingReq = send_gpp
                };
                com.mirle.iibg3k0.ttc.Common.TrxTcpIp.ReturnCode result = snedRecv(wrapper, out receive_gpp, out rtnMsg);
                isScuess = result == TrxTcpIp.ReturnCode.Normal;
            }
            catch (Exception ex)
            {
                receive_gpp = null;
                logger.Error(ex, "Exception");
            }
            return isScuess;

        }
        public override bool sned_Str41(ID_41_MODE_CHANGE_REQ send_gpp, out ID_141_MODE_CHANGE_RESPONSE receive_gpp)
        {
            bool isScuess = false;
            try
            {
                string rtnMsg = string.Empty;
                WrapperMessage wrapper = new WrapperMessage
                {
                    ID = VHMSGIF.ID_MODE_CHANGE_REQUEST,
                    ModeChangeReq = send_gpp
                };
                com.mirle.iibg3k0.ttc.Common.TrxTcpIp.ReturnCode result = snedRecv(wrapper, out receive_gpp, out rtnMsg);
                isScuess = result == TrxTcpIp.ReturnCode.Normal;
            }
            catch (Exception ex)
            {
                receive_gpp = null;
                logger.Error(ex, "Exception");
            }
            return isScuess;

        }
        public override bool sned_Str91(ID_91_ALARM_RESET_REQUEST send_gpp, out ID_191_ALARM_RESET_RESPONSE receive_gpp)
        {
            bool isScuess = false;
            try
            {
                string rtnMsg = string.Empty;
                WrapperMessage wrapper = new WrapperMessage
                {
                    ID = VHMSGIF.ID_PAUSE_REQUEST,
                    AlarmResetReq = send_gpp
                };
                com.mirle.iibg3k0.ttc.Common.TrxTcpIp.ReturnCode result = snedRecv(wrapper, out receive_gpp, out rtnMsg);
                isScuess = result == TrxTcpIp.ReturnCode.Normal;
            }
            catch (Exception ex)
            {
                receive_gpp = null;
                logger.Error(ex, "Exception");
            }
            return isScuess;
        }


        public override bool snedMessage(WrapperMessage wrapper, bool isReply = false)
        {
            Boolean resp_cmp = ITcpIpControl.sendGoogleMsg(bcfApp, tcpipAgentName, wrapper, true);
            return resp_cmp;
        }
        object sendRecv_LockObj = new object();
        public override com.mirle.iibg3k0.ttc.Common.TrxTcpIp.ReturnCode snedRecv<TSource2>(WrapperMessage wrapper, out TSource2 stRecv, out string rtnMsg)
        {
            //lock (sendRecv_LockObj)
            //{
            //    return ITcpIpControl.sendRecv_Google(bcfApp, tcpipAgentName, wrapper, out stRecv, out rtnMsg);
            //}
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(sendRecv_LockObj, SCAppConstants.LOCK_TIMEOUT_MS, ref lockTaken);
                if (!lockTaken)
                    throw new TimeoutException("snedRecv time out lock happen");
                return ITcpIpControl.sendRecv_Google(bcfApp, tcpipAgentName, wrapper, out stRecv, out rtnMsg);
            }
            finally
            {
                if (lockTaken) Monitor.Exit(sendRecv_LockObj);
            }
        }

        public override void PLC_Control_TrunOff() { /*Nothing...*/ }



        protected void ReplyTimeOutHandler(object sender, TcpIpEventArgs e)
        {
            TcpIpExceptionEventArgs excptionArg = e as TcpIpExceptionEventArgs;
            if (e == null) return;
            scApp.AlarmBLL.onMainAlarm(SCAppConstants.MainAlarmCode.VH_WAIT_REPLY_TIME_OUT_0_1_2
                                       , eqpt.VEHICLE_ID
                                       , e.iPacketID
                                       , e.iSeqNum);
        }
        protected void SendErrorHandler(object sender, TcpIpEventArgs e)
        {
            TcpIpExceptionEventArgs excptionArg = e as TcpIpExceptionEventArgs;
            if (e == null) return;

            scApp.AlarmBLL.onMainAlarm(SCAppConstants.MainAlarmCode.VH_SEND_MSG_ERROR_0_1_2
                           , eqpt.VEHICLE_ID
                           , e.iPacketID
                           , e.iSeqNum);
        }

        protected void SendRecvStateChangeHandler(object sender, TcpIpAgent.E_Msg_STS msg_satae)
        {
            eqpt.TcpIp_Msg_State = msg_satae.ToString();
        }


        public static Google.Protobuf.IMessage unPackWrapperMsg(byte[] raw_data)
        {
            WrapperMessage WarpperMsg = ToObject<WrapperMessage>(raw_data);
            return WarpperMsg;
        }
        public static T ToObject<T>(byte[] buf) where T : Google.Protobuf.IMessage<T>, new()
        {
            if (buf == null)
                return default(T);
            Google.Protobuf.MessageParser<T> parser = new Google.Protobuf.MessageParser<T>(() => new T());
            return parser.ParseFrom(buf);
        }


        string event_id = string.Empty;
        /// <summary>
        /// Does the initialize.
        /// </summary>
        public override void doInit()
        {
            try
            {
                if (eqpt == null)
                    return;
                event_id = "EQTcpIpMapAction_" + eqpt.VEHICLE_ID;
                tcpipAgentName = eqpt.TcpIpAgentName;
                //======================================連線狀態=====================================================
                RegisteredTcpIpProcEvent();




                ITcpIpControl.addTcpIpConnectedHandler(bcfApp, tcpipAgentName, Connection);
                ITcpIpControl.addTcpIpDisconnectedHandler(bcfApp, tcpipAgentName, Disconnection);

                ITcpIpControl.addTcpIpReplyTimeOutHandler(bcfApp, tcpipAgentName, ReplyTimeOutHandler);
                ITcpIpControl.addTcpIpSendErrorHandler(bcfApp, tcpipAgentName, SendErrorHandler);
                ITcpIpControl.addSendRecvStateChangeHandler(bcfApp, tcpipAgentName, SendRecvStateChangeHandler);

                //d.str134_Receive(null, null);
                eqpt.addEventHandler(event_id
                    , BCFUtility.getPropertyName(() => eqpt.ObstacleStatus)
                    , (s1, e1) => { whenObstacleFinish(); });
                eqpt.addEventHandler(event_id
                    , BCFUtility.getPropertyName(() => eqpt.BlockingStatus)
                    , (s1, e1) => { whenBlockFinish(); });
                eqpt.addEventHandler(event_id
                    , BCFUtility.getPropertyName(() => eqpt.PauseStatus)
                    , (s1, e1) => { whenPauseFinish(); });
            }
            catch (Exception ex)
            {
                scApp.getBCFApplication().onSMAppError(0, "MapActionEQType2Secs doInit");
                logger.Error(ex, "Exection:");
            }

        }

        public override void RegisteredTcpIpProcEvent()
        {
            ITcpIpControl.addTcpIpReceivedHandler(bcfApp, tcpipAgentName, VHMSGIF.ID_ALARM_REPORT.ToString(), str194_Receive);

            ITcpIpControl.addTcpIpReceivedHandler(bcfApp, tcpipAgentName, VHMSGIF.ID_TRANS_COMPLETE_REPORT.ToString(), str132_Receive);
            ITcpIpControl.addTcpIpReceivedHandler(bcfApp, tcpipAgentName, VHMSGIF.ID_TRANS_PASS_EVENT_REPORT.ToString(), str134_Receive);
            ITcpIpControl.addTcpIpReceivedHandler(bcfApp, tcpipAgentName, VHMSGIF.ID_TRANS_EVENT_REPORT.ToString(), str136_Receive);
            ITcpIpControl.addTcpIpReceivedHandler(bcfApp, tcpipAgentName, VHMSGIF.ID_STATUS_CHANGE_REPORT.ToString(), str144_Receive);
        }
        public override void UnRgisteredProcEvent()
        {
            ITcpIpControl.removeTcpIpReceivedHandler(bcfApp, tcpipAgentName, VHMSGIF.ID_ALARM_REPORT.ToString(), str194_Receive);

            ITcpIpControl.removeTcpIpReceivedHandler(bcfApp, tcpipAgentName, VHMSGIF.ID_TRANS_COMPLETE_REPORT.ToString(), str132_Receive);
            ITcpIpControl.removeTcpIpReceivedHandler(bcfApp, tcpipAgentName, VHMSGIF.ID_TRANS_PASS_EVENT_REPORT.ToString(), str134_Receive);
            ITcpIpControl.removeTcpIpReceivedHandler(bcfApp, tcpipAgentName, VHMSGIF.ID_STATUS_CHANGE_REPORT.ToString(), str144_Receive);
        }
    }
}
