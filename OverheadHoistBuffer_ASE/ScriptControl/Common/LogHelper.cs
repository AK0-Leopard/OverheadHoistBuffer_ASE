using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using com.mirle.ibg3k0.stc.Data.SecsData;
using Google.Protobuf;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace com.mirle.ibg3k0.sc.Common
{
    public static class LogHelper
    {
        public const string CALL_CONTEXT_KEY_WORD_SERVICE_ID = "SERVICE_ID";
        static ObjectPool<LogObj> LogObjPool = new ObjectPool<LogObj>(() => new LogObj());
        static Logger logger = LogManager.GetCurrentClassLogger();


        public static void setCallContextKey_ServiceID(string service_id)
        {
            string xid = System.Runtime.Remoting.Messaging.CallContext.GetData(CALL_CONTEXT_KEY_WORD_SERVICE_ID) as string;
            if (SCUtility.isEmpty(xid))
            {
                System.Runtime.Remoting.Messaging.CallContext.SetData(LogHelper.CALL_CONTEXT_KEY_WORD_SERVICE_ID, service_id);
            }
        }

        public static void Log(Logger logger, NLog.LogLevel LogLevel,
            string Class, string Device, SXFY Data,
            string VehicleID = null, string CarrierID = null, string LogID = null, string Level = null, string ThreadID = null, string Lot = null, string XID = null, string Transaction = null,
            [CallerMemberName] string Method = "")
        {
            //如果被F'Y'，Y可以被2整除的話代表是收到的
            bool isReceive = Data.getF() % 2 == 0;
            LogConstants.Type type = isReceive ? LogConstants.Type.Receive : LogConstants.Type.Send;
            Log(logger, LogLevel, Class, Device,
                Data: $"[{Data.SystemByte}]{Data.StreamFunction}-{Data.StreamFunctionName}",
                VehicleID: VehicleID,
                CarrierID: CarrierID,
                Type: type,
                LogID: LogID,
                Level: Level,
                ThreadID: ThreadID,
                Lot: Lot,
                XID: XID,
                //Details: Data.toSECSString(),
                Details: "",
                Method: Method
                );
        }


        public static void Log(Logger logger, NLog.LogLevel LogLevel,
            string Class, string Device, int seq_num, IMessage Data,
            string VehicleID = null, string CarrierID = null, string LogID = null, string Level = null, string ThreadID = null, string Lot = null, string XID = null, string Transaction = null,
            [CallerMemberName] string Method = "")
        {
            string function_name = $"[{seq_num}]{Data.Descriptor.Name}";

            LogConstants.Type? type = null;
            if (function_name.Contains("_"))
            {
                int packet_id = 0;
                string[] function_name_splil = function_name.Split('_');
                if (int.TryParse(function_name_splil[1], out packet_id))
                {
                    type = packet_id > 100 ? LogConstants.Type.Receive : LogConstants.Type.Send;
                }
            }
            Log(logger, LogLevel, Class, Device,
            Data: function_name,
            VehicleID: VehicleID,
            CarrierID: CarrierID,
            Type: type,
            LogID: LogID,
            Level: Level,
            ThreadID: ThreadID,
            Lot: Lot,
            XID: XID,
            Details: Data.ToString(),
            Method: Method
            );
        }

        public static void Log(Logger logger, NLog.LogLevel LogLevel,
            string Class, string Device, Exception Data,
            string VehicleID = null, string CarrierID = null, string LogID = null, string Level = null, string ThreadID = null, string Lot = null, string XID = null, string Details = null,
            [CallerMemberName] string Method = "")
        {
            Log(logger, LogLevel, Class, Device,
                Data: Data.ToString(),
                VehicleID: VehicleID,
                CarrierID: CarrierID,
                LogID: LogID,
                Level: Level,
                ThreadID: ThreadID,
                Lot: Lot,
                XID: XID,
                Details: Details,
                Method: Method
                );
        }

        public static void Log(Logger logger, NLog.LogLevel LogLevel,
            string Class, string Device, string Data,
            string VehicleID = null, string CarrierID = null, LogConstants.Type? Type = null, string LogID = null, string Level = null, string ThreadID = null, string Lot = null, string XID = null, string Details = null,
            [CallerMemberName] string Method = "")
        {
            LogObj logObj = LogObjPool.GetObject();
            try
            {
                logObj.dateTime = DateTime.Now;
                logObj.Sequence = getSequence();
                logObj.LogLevel = LogLevel.Name;
                logObj.Class = Class;
                logObj.Method = Method;
                logObj.Device = Device;
                logObj.Data = Data;
                logObj.VH_ID = VehicleID;
                logObj.CarrierID = CarrierID;

                logObj.Type = Type;
                logObj.LogID = LogID;
                logObj.ThreadID = ThreadID != null ?
                    ThreadID : Thread.CurrentThread.ManagedThreadId.ToString();
                logObj.Lot = Lot;
                logObj.Level = Level;

                string xid = System.Runtime.Remoting.Messaging.CallContext.GetData(CALL_CONTEXT_KEY_WORD_SERVICE_ID) as string;
                logObj.XID = xid;

                Transaction Transaction = getCurrentTransaction();
                logObj.TransactionID = Transaction == null ?
                    string.Empty : Transaction.TransactionInformation.LocalIdentifier.ToString();
                logObj.Details = Details;
                logObj.Index = "SystemProcessLog";

                LogHelper.logger.Log(LogLevel, logObj.ToString());

            }
            catch (Exception e)
            {
                LogHelper.logger.Error($"{e}, Exception");
            }
            finally
            {
                LogObjPool.PutObject(logObj);
            }
        }
        static Google.Protobuf.JsonFormatter jsonFormatter =
        new JsonFormatter(new JsonFormatter.Settings(true).
        WithFormatDefaultValues(true));

        public static void RecordReportInfoNew(AVEHICLE vh, IMessage message, int seqNum, [CallerMemberName] string Method = "")
        {
            if (message == null)
                return;
            dynamic logEntry = new JObject();
            DateTime nowDt = DateTime.Now;
            string vhID = vh.VEHICLE_ID;
            string current_excute_cmd_id = SCUtility.Trim(vh.OHTC_CMD, true);
            string current_excute_cmd_mcs_id = SCUtility.Trim(vh.MCS_CMD, true);
            string box_id = SCUtility.Trim(vh.BOX_ID, true);
            //string detail = PrintMessage(message, current_excute_cmd_id, current_excute_cmd_mcs_id);
            string detail = jsonFormatter.Format(message);
            string function = getIMessageName(message);
            logEntry.dateTime = nowDt.ToString(SCAppConstants.DateTimeFormat_23);

            logEntry.eq_id = vhID;
            logEntry.task_id = current_excute_cmd_id;
            logEntry.transfer_id = current_excute_cmd_mcs_id;
            logEntry.carrier_id = box_id;
            logEntry.eq_id = vhID;
            logEntry.name = function;
            logEntry.seq_no = seqNum.ToString("000");
            logEntry.type = "TcpIpTrx";
            logEntry.detail = detail;
            //detail = detail.Replace("\\\"", "\"");

            var json = logEntry.ToString(Newtonsoft.Json.Formatting.None);
            json = json.Replace("dateTime", "@t");
            LogManager.GetLogger("RecordReportInfo").Info(json);
        }
        public static string getIMessageName(IMessage message)
        {
            var descriptor = message.Descriptor;
            string name = descriptor.Name;
            if (name.Contains('_'))
            {
                string[] name_temp = name.Split('_');
                if (name_temp.Length >= 2)
                {
                    name = $"{name_temp[0]}_{name_temp[1].PadLeft(3, '0')}";
                }
            }
            return name;
        }
        public static void LogBCRReadInfo(string VehicleID, string portID, string mcsCmdID, string ohtcCmdID, string carrierID, string readCarrierID, ProtocolFormat.OHTMessage.BCRReadResult bCRReadResult,
                                          bool IsEnableIDReadFailScenario, [CallerMemberName] string Method = "")
        {
            try
            {
                dynamic logEntry = new Newtonsoft.Json.Linq.JObject();
                logEntry.dateTime = DateTime.Now;
                logEntry.Method = Method;
                logEntry.VH_ID = VehicleID;
                logEntry.PortID = SCUtility.Trim(portID);
                logEntry.CarrierID = SCUtility.Trim(carrierID);
                logEntry.ReadCarrierID = SCUtility.Trim(readCarrierID);
                logEntry.MCS_CMD_ID = SCUtility.Trim(mcsCmdID);
                logEntry.OHTC_CMD_ID = SCUtility.Trim(ohtcCmdID);
                logEntry.BCRReadResult = bCRReadResult.ToString();
                logEntry.IsEnableIDReadFailScenario = IsEnableIDReadFailScenario;
                logEntry.Index = "BCRReadInfo";

                var json = logEntry.ToString(Newtonsoft.Json.Formatting.None);
                json = json.Replace("dateTime", "@timestamp");
                LogManager.GetLogger("BCRReadInfo").Info(json);
            }
            catch (Exception e)
            {
                LogHelper.logger.Error($"{e}, Exception");
            }
        }

        private static Transaction getCurrentTransaction()
        {
            try
            {
                Transaction Transaction = Transaction.Current;
                return Transaction;
            }
            catch { return null; }
        }


        static object sequence_lock = new object();
        static UInt64 NextSequence = 1;
        static private UInt64 getSequence()
        {
            lock (sequence_lock)
            {
                UInt64 currentSeq = NextSequence;
                NextSequence++;
                return currentSeq;
            }

        }


    }

    public static class LogConstants
    {
        public enum Type
        {
            Send,
            Receive
        }
    }
}
