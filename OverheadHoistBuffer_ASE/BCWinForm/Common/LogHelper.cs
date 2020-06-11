using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using Google.Protobuf;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace com.mirle.ibg3k0.bc.winform.Common
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
            string VehicleID = null, string CarrierID = null, string LogID = null, string Level = null, string ThreadID = null, string Lot = null, string XID = null, string Details = null,
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

}
