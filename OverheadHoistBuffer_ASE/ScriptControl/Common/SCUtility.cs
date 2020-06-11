//*********************************************************************************
//      SCUtility.cs
//*********************************************************************************
// File Name: SCUtility.cs
// Description: ScriptControl 共用工具元件
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.App;
using System.Data;
using System.Reflection;
using NLog;
using com.mirle.ibg3k0.stc.Data.SecsData;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.bcf.App;
using System.Collections;
using System.Diagnostics;
using System.Linq.Expressions;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.Utility.ul.Data.VO;
using System.Net.NetworkInformation;
using System.Threading;
using NLog.LayoutRenderers;
using System.Xml;
using Newtonsoft.Json.Linq;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System.Globalization;
using Google.Protobuf;
using com.mirle.ibg3k0.sc.Data.SECS;
using System.Transactions;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace com.mirle.ibg3k0.sc.Common
{
    /// <summary>
    /// Class SCUtility.
    /// </summary>
    public class SCUtility
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// The secs MSG logger
        /// </summary>
        private static Logger SECSMsgLogger = LogManager.GetLogger("SECSMsgLogger");
        /// <summary>
        /// The CST information logger
        /// </summary>
        private static Logger CSTInfoLogger = LogManager.GetLogger("CSTInfoLogger");
        /// <summary>
        /// The operation logger
        /// </summary>
        private static Logger OperationLogger = LogManager.GetLogger("OperationLogger");
        //private CommonInfo ci;




        /// <summary>
        /// Systems the event log.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="type">The type.</param>
        public static void SystemEventLog(string msg, EventLogEntryType type)
        {
            try
            {
                string src_name = "SC Application";
                if (!EventLog.SourceExists(src_name))
                {
                    EventLog.CreateEventSource(src_name, src_name);
                }
                EventLog eLog = new EventLog();
                eLog.Source = src_name;
                eLog.WriteEntry(msg, type);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        /// <summary>
        /// Determines whether the specified obj1 is matche.
        /// </summary>
        /// <param name="obj1">The obj1.</param>
        /// <param name="obj2">The obj2.</param>
        /// <returns>Boolean.</returns>
        public static Boolean isMatche(Object obj1, Object obj2)
        {
            if (obj1 == null || obj2 == null)
                return false;
            return BCFUtility.isMatche(obj1, obj2);
        }

        /// <summary>
        /// Determines whether the specified object is empty.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Boolean.</returns>
        public static Boolean isEmpty(Object obj)
        {
            return BCFUtility.isEmpty(obj);
        }

        /// <summary>
        /// Trims the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>String.</returns>
        public static String Trim(String source)
        {
            //if (source == null) { return source; }
            //return source.Trim();
            return Trim(source, false);
        }

        /// <summary>
        /// A0.02
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>String.</returns>
        public static String TrimAndToUpper(String source)
        {
            //if (source == null) { return source; }
            //return source.Trim();
            return ToUpper(Trim(source, false), false);
        }

        public static bool int2Bool(int i)
        {
            return i == 1;
        }
        public static UInt16 getUInt16FromBitArray(BitArray bitArray)
        {

            if (bitArray.Length > 16)
                throw new ArgumentException("Argument length shall be at most 32 bits.");

            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return (UInt16)array[0];

        }

        public static bool tryParseUInt16AndRecord(Logger logger, string value_name, string sValue, out UInt16 value)
        {
            if (!UInt16.TryParse(sValue, out value))
            {
                logger.Warn($"{value_name}:{sValue}, parse to UInt16 fail.");
                return false;
            }
            return true;
        }
        public static bool tryParseUInt32AndRecord(Logger logger, string value_name, string sValue, out UInt32 value)
        {
            if (!UInt32.TryParse(sValue, out value))
            {
                logger.Warn($"{value_name}:{sValue}, parse to UInt16 fail.");
                return false;
            }
            return true;
        }
        public static bool tryParseUInt64AndRecord(Logger logger, string value_name, string sValue, out UInt64 value)
        {
            if (!UInt64.TryParse(sValue, out value))
            {
                logger.Warn($"{value_name}:{sValue}, parse to UInt16 fail.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Trims the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="rtnEmptyStr">The RTN empty string.</param>
        /// <returns>String.</returns>
        public static String Trim(string source, Boolean rtnEmptyStr)
        {
            if (source == null) { return (rtnEmptyStr ? string.Empty : source); }
            return source.Trim();
        }

        /// <summary>
        /// A0.02
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="rtnEmptyStr">The RTN empty string.</param>
        /// <returns>String.</returns>
        public static String ToUpper(string source, Boolean rtnEmptyStr)
        {
            if (source == null) { return (rtnEmptyStr ? string.Empty : source); }
            return source.ToUpper();
        }

        /// <summary>
        /// Fills the pad left.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="padChar">The pad character.</param>
        /// <param name="length">The length.</param>
        /// <returns>String.</returns>
        public static String FillPadLeft(string source, char padChar, int length)
        {
            if (source == null) { return null; }
            return Trim(source).PadLeft(length, padChar);
        }

        /// <summary>
        /// Fills the specified logic object.
        /// </summary>
        /// <param name="LogicObject">The logic object.</param>
        /// <param name="Row">The row.</param>
        public static void Fill(object LogicObject, DataRow Row)
        {
            Dictionary<string, PropertyInfo> props = new Dictionary<string, PropertyInfo>();
            foreach (PropertyInfo p in LogicObject.GetType().GetProperties())
                props.Add(p.Name, p);
            foreach (DataColumn col in Row.Table.Columns)
            {
                string name = col.ColumnName;
                if (Row[name] != DBNull.Value && props.ContainsKey(name))
                {
                    object item = Row[name];
                    PropertyInfo p = props[name];
                    if (p.PropertyType != col.DataType)
                        item = Convert.ChangeType(item, p.PropertyType);
                    p.SetValue(LogicObject, item, null);
                }
            }

        }

        /// <summary>
        /// 更改系統時間
        /// </summary>
        /// <param name="hostTime">The host time.</param>
        public static void updateSystemTime(DateTime hostTime)
        {
            SystemTime st = new SystemTime();
            st.FromDateTime(hostTime);
            SystemTime.SetSystemTime(ref st);
            SystemTime.GetSystemTime(ref st);
            logger.Info("Set System Time:{0}", st.ToDateTime().ToString(SCAppConstants.TimestampFormat_16));
        }

        /// <summary>
        /// Strings the list to string.
        /// </summary>
        /// <param name="tagS">The tag s.</param>
        /// <param name="sList">The s list.</param>
        /// <returns>System.String.</returns>
        public static string stringListToString(string tagS, List<String> sList)
        {
            StringBuilder sb = new StringBuilder();
            foreach (String str in sList)
            {
                sb.Append(str);
                if (tagS != null)
                {
                    sb.Append(tagS);
                }
            }
            return sb.ToString();
        }

        public static char[] string2CharArray(string s, int length)
        {
            char[] Chars = null;
            if (s.Length > length) s = s.Substring(0, length);
            Chars = s.PadRight(length, ' ').ToCharArray();
            return Chars;
        }


        /// <summary>
        /// Secses the action record MSG.
        /// </summary>
        /// <param name="scApp">The sc application.</param>
        /// <param name="isReceive">The is receive.</param>
        /// <param name="sxfy">The sxfy.</param>
        public static void secsActionRecordMsg(SCApplication scApp, Boolean isReceive, SXFY sxfy)
        {
            if (sxfy == null) { return; }
            string sDateTime = DateTime.Now.ToString(SCAppConstants.DateTimeFormat_23);           //B0.05
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("[{0}][{1}][{2}][{3}][{4}][{5}]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                (isReceive ? "R" : "S"), sxfy.StreamFunction, sxfy.SystemByte, sxfy.W_Bit, sxfy.SECSAgentName));
            //            sb.AppendLine(sxfy.toSECSString());
            //            string msg = string.Format("{0}{1}", sb.ToString(), sxfy.toSECSString());
            string msg = string.Format("{0}{1}", sb.ToString(), sxfy.GetType().Name);
            scApp.getEQObjCacheManager().CommonInfo.SECS_Msg = msg;// sb.ToString();
            //            SECSMsgLogger.Info(sb.ToString());
            SECSMsgLogger.Info(msg);
            Task.Run(() =>
            {
                setLogInfo_SECS(scApp, isReceive, sxfy, sDateTime);
            });
        }

        public const String FUNCTION_TRANSDFERTYPE_SEND = "Send";
        public const String FUNCTION_TRANSDFERTYPE_RECEIVE = "Receive";
        static com.mirle.ibg3k0.Utility.ul.Data.LogUtility logUtility = com.mirle.ibg3k0.Utility.ul.Data.LogUtility.getInstance();
        public static void setLogInfo_SECS(SCApplication scApp, Boolean isReceive, SXFY sxfy, string sDateTime)
        {
            try
            {

                if (isEmpty(sxfy.SECSAgentName)) return;

                com.mirle.ibg3k0.stc.Common.SECS.SECSAgent secsAgent = scApp.getBCFApplication().getSECSAgent(sxfy.SECSAgentName);
                string device_id = secsAgent == null ? string.Empty : secsAgent.DeviceID.ToString();

                string s = sxfy.getS().ToString();
                string f = sxfy.getF().ToString();

                LogTitle_SECS logTitleTemp = new LogTitle_SECS()
                {
                    EQ_ID = sxfy.SECSAgentName,
                    SendRecive = isReceive ? FUNCTION_TRANSDFERTYPE_RECEIVE
                                            : FUNCTION_TRANSDFERTYPE_SEND,
                    Sx = s,
                    Fy = f,
                    DeviceID = device_id,
                    FunName = sxfy.StreamFunction,
                    Message = sxfy.toSECSString(),
                };
                logTitleTemp.Time = sDateTime;
                logUtility.addLogInfo(logTitleTemp);

                System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(SCApplication.getInstance().LineService.PublishHostMsgInfo), logTitleTemp);
            }
            catch (Exception ex)
            {
                logger.Warn(ex, "Exception:");
            }
        }

        /// <summary>
        /// Secses the action record MSG.
        /// </summary>
        /// <param name="scApp">The sc application.</param>
        /// <param name="isReceive">The is receive.</param>
        /// <param name="systemByte">The system byte.</param>
        /// <param name="secsAgentName">Name of the secs agent.</param>
        /// <param name="msg">The MSG.</param>
        public static void secsActionRecordMsg(SCApplication scApp, Boolean isReceive, int systemByte, string secsAgentName,
            string msg)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("[{0}][{1}][{2}][{3}][{4}]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                (isReceive ? "R" : "S"), systemByte, secsAgentName, msg));
            sb.AppendLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "] " + (isReceive ? "[R]" : "[S]") +
                "[" + msg + "]");
            scApp.getEQObjCacheManager().CommonInfo.SECS_Msg = sb.ToString();
            SECSMsgLogger.Info(sb.ToString());
        }

        /// <summary>
        /// Actions the record MSG.
        /// </summary>
        /// <param name="scApp">The sc application.</param>
        /// <param name="funID">The fun identifier.</param>
        /// <param name="eqID">The eq identifier.</param>
        /// <param name="msg">The MSG.</param>
        /// <param name="result">The result.</param>
        public static void actionRecordMsg(SCApplication scApp, String funID, String eqID, String msg, String result)
        {
            StringBuilder sb = new StringBuilder();
            String dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            sb.Append(String.Format("[{0}][{1}][{2}][{3}][{4}]", dateTime, funID, eqID, msg, result));
            SECSMsgLogger.Info(sb.ToString());
        }


        /// <summary>
        /// A0.01 用於調整Datagridview的顯示大小。
        /// </summary>
        /// <param name="dgvGrd"></param>
        public static void subInitGrdHeader(System.Windows.Forms.DataGridView dgvGrd)
        {
            try
            {
                dgvGrd.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold);
                dgvGrd.RowHeadersDefaultCellStyle.Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold);
                dgvGrd.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 12);
                dgvGrd.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                dgvGrd.RowHeadersWidth = 41;
                dgvGrd.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke;

                //dgvGrd.Columns.Add("", "");
                dgvGrd.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
                dgvGrd.ColumnHeadersDefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
                dgvGrd.Columns[dgvGrd.ColumnCount - 1].AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        public static UInt16 convertToUInt16(string svalue)
        {
            UInt16 value = 0;
            UInt16.TryParse(svalue, out value);
            return value;
        }
        public static char[] CombinedArray(List<char[]> charArrays)
        {
            int total_count = charArrays.Sum(charArray => charArray.Count());
            char[] target_chararray = new char[total_count];
            int start_index = 0;
            int copy_length = 0;
            foreach (char[] charArray in charArrays)
            {
                copy_length = charArray.Count();
                Array.Copy(charArray, 0, target_chararray, start_index, copy_length);
                start_index += copy_length;
            }
            return target_chararray;
        }
        /// <summary>
        /// Converts to int.
        /// </summary>
        /// <param name="int1">The int1.</param>
        /// <param name="iStartIndex1">The i start index1.</param>
        /// <param name="iEndIndex1">The i end index1.</param>
        /// <param name="int2">The int2.</param>
        /// <param name="iStartIndex2">The i start index2.</param>
        /// <param name="iEndIndex2">The i end index2.</param>
        /// <returns>System.Int32.</returns>
        public static int convertToInt(int int1, int iStartIndex1, int iEndIndex1, int int2, int iStartIndex2, int iEndIndex2)
        {
            int shiftInt2 = int2 * (int)Math.Pow(2, 12);
            UInt16[] iArray1 = new UInt16[1] { (UInt16)int1 };
            UInt16[] iArray2 = new UInt16[1] { (UInt16)shiftInt2 };
            Boolean[] bArray1 = convertToBooleans(iArray1, 0, 0);
            Boolean[] bArray2 = convertToBooleans(iArray2, 0, 0);
            BitArray ResultArray = new BitArray(16);

            int boolCount = 0;
            foreach (Boolean b in bArray1)
            {
                ResultArray[boolCount] = bArray1[boolCount] | bArray2[boolCount];
                boolCount++;
            }

            int[] intAry = new int[1];
            ResultArray.CopyTo(intAry, 0);
            return intAry[0];
        }

        /// <summary>
        /// Converts to booleans.
        /// </summary>
        /// <param name="dataTemp">The data temporary.</param>
        /// <param name="iStartIndex">Start index of the i.</param>
        /// <param name="iEndIndex">End index of the i.</param>
        /// <returns>Boolean[].</returns>
        public static Boolean[] convertToBooleans(UInt16[] dataTemp, int iStartIndex, int iEndIndex)
        {
            int[] rangeData = BCFUtility.getArrayRange(dataTemp, iStartIndex, iEndIndex);
            return (Boolean[])BCFUtility.convertInt2TextByType(16, Type.GetType("System.Boolean[]"), rangeData);
        }


        /// <summary>
        /// Gets the length of the properties.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>String.</returns>
        public static String getPropertiesLength(object obj)
        {
            StringBuilder sb = new StringBuilder();
            var type = obj.GetType();

            // Get the PropertyInfo object:
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {

                if (property.PropertyType == typeof(string))
                {
                    String s;
                    if (property.GetValue(obj) != null)
                        s = property.GetValue(obj).ToString();
                    else
                        s = "";
                    sb.AppendFormat("{0}.length = '{1}'", property.Name, s.Length);
                    sb.AppendFormat("    {0}.value = '{1}'", property.Name, property.GetValue(obj));
                    sb.AppendLine();
                }
                else
                {
                    sb.AppendFormat("{0}.value = '{1}'", property.Name, property.GetValue(obj));
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Trims all parameter.
        /// </summary>
        /// <param name="obj">The object.</param>
        public static void TrimAllParameter(object obj)
        {
            var type = obj.GetType();
            // Get the PropertyInfo object:
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                try
                {
                    if (property.PropertyType == typeof(string) && property.CanWrite)
                    {
                        if (property.GetValue(obj) != null)
                        {
                            String temp = property.GetValue(obj).ToString().Trim();
                            property.SetValue(obj, temp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
            }
        }

        /// <summary>
        /// Shows the caller information.
        /// </summary>
        /// <param name="st">The st.</param>
        /// <param name="remark">The remark.</param>
        /// <returns>System.String.</returns>
        public static string ShowCallerInfo(StackTrace st, string remark)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                if (st == null)
                {
                    st = new StackTrace(true);
                }
                sb.AppendLine(new string('=', 80));
                sb.AppendLine(string.Format("Caller Remark: {0}", remark));
                StackFrame sf = st.GetFrame(1);
                MethodBase mb = sf.GetMethod();
                sb.AppendLine(string.Format("Caller Module: {0}", mb.Module.FullyQualifiedName));
                sb.AppendLine(string.Format("Caller Class & Method: {0}.{1}()", mb.ReflectedType.FullName, mb.Name));
                sb.AppendLine(string.Format("File Info: Line {0} in {1}", sf.GetFileLineNumber(), sf.GetFileName()));
                sb.AppendLine(new string('=', 80));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
            return sb.ToString();
        }





        /// <summary>
        /// Prints the operation log.
        /// </summary>
        /// <param name="opHis">The op his.</param>
        public static void PrintOperationLog(HOPERATION opHis)
        {
            try
            {
                if (opHis == null) { return; }
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendLine(string.Format("{0}Time: {1}", new string(' ', 5), opHis.T_STAMP));
                sb.AppendLine(string.Format("{0}User: {1}", new string(' ', 5), opHis.USER_ID));
                sb.AppendLine(string.Format("{0}UI Name: {1}", new string(' ', 5), opHis.FORM_NAME));
                sb.AppendLine(string.Format("{0}Action: ", new string(' ', 5)));
                sb.AppendLine(string.Format("{0}         {1}", new string(' ', 5), opHis.ACTION));
                OperationLogger.Info(sb.ToString());
                sb.Clear();
            }
            catch (Exception ex)
            {
                logger.Warn(ex, "SCUtility.PrintOperationLog Occur Exception[{0}]");
            }
        }
        //public static T ToObject<T>(byte[] buf) where T : Google.Protobuf.IMessage<T>, new()
        //{
        //    if (buf == null)
        //        return default(T);

        //    Google.Protobuf.MessageParser<T> parser = new Google.Protobuf.MessageParser<T>(() => new T());
        //    return parser.ParseFrom(buf);

        //}

        /// <summary>
        /// Pings it.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool PingIt(String ip, out PingReply reply)
        {
            try
            {
                Ping p = new Ping();
                reply = p.Send(ip);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                reply = null;
                return false;
            }
            return reply.Status == IPStatus.Success;
        }
        public static bool PingIt(System.Net.IPAddress ip, out PingReply reply)
        {
            try
            {
                Ping p = new Ping();
                reply = p.Send(ip);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                reply = null;
                return false;
            }
            return reply.Status == IPStatus.Success;
        }

        //物件序列化
        public static byte[] ToByteArray(object source)
        {
            var Formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (var stream = new System.IO.MemoryStream())
            {
                Formatter.Serialize(stream, source);
                return stream.ToArray();
            }
        }
        //序列還原物件
        public static object ToObject(byte[] source)
        {
            var Formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (var stream = new System.IO.MemoryStream(source))
            {
                return Formatter.Deserialize(stream);
            }
        }

        public static void LockWithTimeout(object p_oLock, int p_iTimeout, Action p_aAction)
        {
            Exception eLockException = null;
            bool bLockWasTaken = false;

            try
            {
                Monitor.TryEnter(p_oLock, p_iTimeout, ref bLockWasTaken);
                if (bLockWasTaken)
                    p_aAction();
                else
                    throw new TimeoutException(string.Format("Timeout exceeded, unable to lock.Action name{0}"
                                                            , p_aAction.GetMethodInfo().Name));
            }
            catch (Exception ex)
            {
                // conserver l'exception
                logger.Error(ex, "Exception");
                eLockException = ex;
            }
            finally
            {
                // release le lock
                if (bLockWasTaken)
                    Monitor.Exit(p_oLock);

                // relancer l'exception
                if (eLockException != null)
                    throw new Exception("An exception occured during the lock proces.", eLockException);
            }
        }

        public static void LockWithTimeout<T1, T2>(object p_oLock, int p_iTimeout, Action<T1, T2> p_aAction, T1 p1, T2 p2)
        {
            int threadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Exception eLockException = null;
            bool bLockWasTaken = false;
            try
            {
                Monitor.TryEnter(p_oLock, p_iTimeout, ref bLockWasTaken);
                if (bLockWasTaken)
                {
                    //TODO Get Lock
                    LogManager.GetLogger("LockInfo").Debug(string.Concat("Get Lock", "ThreadID:{0}"), threadID.ToString());
                    p_aAction(p1, p2);
                }
                else
                {
                    //Lock Time Out 
                    LogManager.GetLogger("LockInfo").Debug(string.Concat("Lock Time Out", "ThreadID:{0}"), threadID.ToString());
                    throw new TimeoutException("Timeout exceeded, unable to lock.");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                // conserver l'exception
                eLockException = ex;
            }
            finally
            {
                // release le lock
                if (bLockWasTaken)
                {
                    Monitor.Exit(p_oLock);
                    //Release Lock
                    LogManager.GetLogger("LockInfo").Debug(string.Concat("Release Lock", "ThreadID:{0}"), threadID.ToString());
                }
                // relancer l'exception
                if (eLockException != null)
                    throw new Exception("An exception occured during the lock proces.", eLockException);
            }
        }

        /// <summary>
        /// 將List，每N個分成一組，剩下分不滿的放到最後一個
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Lists"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static List<List<T>> SpiltList<T>(List<T> Lists, int num) //where T:class
        {
            List<List<T>> fz = new List<List<T>>();
            if (Lists.Count >= num)
            {
                int avg = Lists.Count / num; //組的数量
                for (int i = 0; i <= avg; i++)
                {
                    List<T> cList = new List<T>();
                    if (i == avg)
                    {
                        cList = Lists.Skip(num * i).ToList<T>();
                    }
                    else
                    {
                        cList = Lists.Skip(num * i).Take(num).ToList<T>();
                    }
                    fz.Add(cList);
                }
            }
            else
            {
                fz.Add(Lists);//數量小於一組的數量
            }
            return fz;
        }

        public static string getLocalIP()
        {
            System.Net.IPAddress[] localIPs = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());
            foreach (System.Net.IPAddress addr in localIPs)
            {
                if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return addr.ToString();
                }
            }
            return string.Empty;
        }

        public static TransactionScope getTransactionScope()
        {
            var option = new TransactionOptions();
            option.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            option.Timeout = TimeSpan.FromMinutes(5);
            return new TransactionScope(TransactionScopeOption.Required, option);
        }


        public const string MSG_ROLE_VH = "Vehicle";
        public const string MSG_ROLE_MCS = "MCS";
        public const string MSG_ROLE_OHXC = "OHxC";
        public static void RecodeReportInfo(string msg_from, string msg_to,
        string fun_name, int seq_num, string vh_id, string ohtc_cmd_id, string act_type,
        string mcs_cmd_id,
        string adr_id, string sec_id, string event_type, uint sec_dis, string block_sec_id, string release_block_id,
        int is_block_pass, int is_hid_pass,
        int travel_dis,
        string vh_status, string msg_body,
        string result)
        {
            dynamic logEntry = new JObject();
            DateTime nowDt = DateTime.Now;
            //logEntry.RPT_TIME = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture);
            logEntry.RPT_TIME = nowDt.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture);

            logEntry.MSG_FROM = Trim(msg_from, true);
            logEntry.MSG_TO = Trim(msg_to, true);
            logEntry.FUN_NAME = Trim(fun_name, true);
            logEntry.SEQ_NUM = seq_num;

            logEntry.VH_ID = Trim(vh_id, true);
            logEntry.OHTC_CMD_ID = Trim(ohtc_cmd_id, true);
            logEntry.ACT_TYPE = act_type;
            //S2F49
            logEntry.MCS_CMD_ID = Trim(mcs_cmd_id, true);
            //132
            logEntry.TRAVEL_DIS = travel_dis;
            //134 Rep
            logEntry.ADR_ID = Trim(adr_id, true);
            logEntry.SEC_ID = Trim(sec_id, true);
            logEntry.EVENT_TYPE = Trim(event_type, true);
            logEntry.SEC_DIS = sec_dis;
            logEntry.BLOCK_SEC_ID = Trim(block_sec_id, true);
            //134 Reply
            logEntry.IS_BLOCK_PASS = is_block_pass;
            logEntry.IS_HID_PASS = is_hid_pass;
            //144
            logEntry.VH_STATUS = Trim(vh_status, true);

            logEntry.MSG_BODY = msg_body;
            logEntry.RESULT = result;
            logEntry.Index = "RecodeReportInfo";


            var json = logEntry.ToString(Newtonsoft.Json.Formatting.None);
            json = json.Replace("RPT_TIME", "@timestamp");
            LogManager.GetLogger("RecodeReportInfo").Info(json);

            logEntry.RPT_TIME = nowDt.ToString(SCAppConstants.DateTimeFormat_23);
            System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(SCApplication.getInstance().LineService.PublishEQMsgInfo), logEntry);
        }

        static Google.Protobuf.JsonFormatter jsonFormatter = new JsonFormatter(new JsonFormatter.Settings(true).WithFormatDefaultValues(true));
        #region TCPIP Msg Log
        public static void RecodeReportInfo(string vh_id, int seq_num, ID_31_TRANS_REQUEST send_str)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(send_str);

            RecodeReportInfo(MSG_ROLE_VH, MSG_ROLE_OHXC,
                fun_name, seq_num, vh_id, send_str.CmdID, send_str.ActType.ToString(),
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(send_str),
                //send_str.ToString(),
                string.Empty);//144
        }
        public static void RecodeReportInfo(string vh_id, int seq_num, ID_131_TRANS_RESPONSE recive_str, string reply_result)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(recive_str);

            RecodeReportInfo(MSG_ROLE_OHXC, MSG_ROLE_VH,
                fun_name, seq_num, vh_id, recive_str.CmdID, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(recive_str),
                //recive_str.ToString(),
                reply_result);//144          
        }

        public static void RecodeReportInfo(string vh_id, int seq_num, ID_132_TRANS_COMPLETE_REPORT recive_str)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(recive_str);

            RecodeReportInfo(MSG_ROLE_VH, MSG_ROLE_OHXC,
                fun_name, seq_num, vh_id, vh_DO.OHTC_CMD, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                recive_str.CmdDistance / 1000,//轉換成公尺
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(recive_str),
                //recive_str.ToString(),
                string.Empty);//144
        }
        public static void RecodeReportInfo(string vh_id, int seq_num, ID_32_TRANS_COMPLETE_RESPONSE send_str, string finish_ohxc_cmd, string finish_mcs_cmd, string reply_result)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(send_str);
            RecodeReportInfo(MSG_ROLE_OHXC, MSG_ROLE_VH,
                fun_name, seq_num, vh_id, finish_ohxc_cmd, string.Empty,
                finish_mcs_cmd, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(send_str),
                //send_str.ToString(),
                reply_result);//144          
        }

        public static void RecodeReportInfo(string vh_id, int seq_num, ID_134_TRANS_EVENT_REP recive_str)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(recive_str);

            RecodeReportInfo(MSG_ROLE_VH, MSG_ROLE_OHXC
                , fun_name, seq_num, vh_id, vh_DO.OHTC_CMD, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                recive_str.CurrentAdrID, recive_str.CurrentSecID, recive_str.EventType.ToString(), recive_str.SecDistance, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(recive_str),
                //recive_str.ToString(),
                string.Empty);//144          
        }

        public static void RecodeReportInfo(string vh_id, int seq_num, ID_35_CARRIER_ID_RENAME_REQUEST send_str)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(send_str);

            RecodeReportInfo(MSG_ROLE_VH, MSG_ROLE_OHXC,
                fun_name, seq_num, vh_id, vh_DO.OHTC_CMD, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(send_str),
                //send_str.ToString(),
                string.Empty);//144
        }
        public static void RecodeReportInfo(string vh_id, int seq_num, ID_135_CARRIER_ID_RENAME_RESPONSE recive_str, string reply_result)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(recive_str);

            RecodeReportInfo(MSG_ROLE_OHXC, MSG_ROLE_VH,
                fun_name, seq_num, vh_id, vh_DO.OHTC_CMD, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(recive_str),
                //recive_str.ToString(),
                reply_result);//144          
        }

        public static void RecodeReportInfo(string vh_id, int seq_num, ID_136_TRANS_EVENT_REP recive_str)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(recive_str);

            RecodeReportInfo(MSG_ROLE_VH, MSG_ROLE_OHXC,
                fun_name, seq_num, vh_id, vh_DO.OHTC_CMD, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                recive_str.CurrentAdrID, recive_str.CurrentSecID, recive_str.EventType.ToString(), (uint)vh_DO.ACC_SEC_DIST, recive_str.RequestBlockID, recive_str.RequestBlockID, //134
                0, 0,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(recive_str),
                //recive_str.ToString(),
                string.Empty);//144          
        }
        public static void RecodeReportInfo(string vh_id, int seq_num, ID_36_TRANS_EVENT_RESPONSE send_str, string reply_result)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(send_str);

            RecodeReportInfo(MSG_ROLE_OHXC, MSG_ROLE_VH,
                fun_name, seq_num, vh_id, vh_DO.OHTC_CMD, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                (int)send_str.IsBlockPass, (int)send_str.IsHIDPass,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(send_str),
                //send_str.ToString(),
                reply_result);//144          
        }

        public static void RecodeReportInfo(string vh_id, int seq_num, ID_37_TRANS_CANCEL_REQUEST send_str)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(send_str);
            RecodeReportInfo(MSG_ROLE_VH, MSG_ROLE_OHXC,
                fun_name, seq_num, vh_id, send_str.CmdID, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(send_str),
                //send_str.ToString(),
                string.Empty);//144
        }
        public static void RecodeReportInfo(string vh_id, int seq_num, ID_137_TRANS_CANCEL_RESPONSE recive_str, string reply_result)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(recive_str);
            RecodeReportInfo(MSG_ROLE_OHXC, MSG_ROLE_VH,
                (recive_str as IMessage).Descriptor.Name, seq_num, vh_id, recive_str.CmdID, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(recive_str),
                //recive_str.ToString(),
                reply_result);//144          
        }

        public static void RecodeReportInfo(string vh_id, int seq_num, ID_39_PAUSE_REQUEST send_str)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(send_str);

            RecodeReportInfo(MSG_ROLE_VH, MSG_ROLE_OHXC,
                fun_name, seq_num, vh_id, vh_DO.OHTC_CMD, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(send_str),
                //send_str.ToString(),
                string.Empty);//144
        }
        public static void RecodeReportInfo(string vh_id, int seq_num, ID_139_PAUSE_RESPONSE recive_str, string reply_result)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(recive_str);

            RecodeReportInfo(MSG_ROLE_OHXC, MSG_ROLE_VH,
                fun_name, seq_num, vh_id, vh_DO.OHTC_CMD, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(recive_str),
                //recive_str.ToString(),
                reply_result);//144          
        }

        public static void RecodeReportInfo(string vh_id, int seq_num, ID_41_MODE_CHANGE_REQ send_str)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(send_str);

            RecodeReportInfo(MSG_ROLE_VH, MSG_ROLE_OHXC,
                fun_name, seq_num, vh_id, vh_DO.OHTC_CMD, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(send_str),
                //send_str.ToString(),
                string.Empty);//144
        }
        public static void RecodeReportInfo(string vh_id, int seq_num, ID_141_MODE_CHANGE_RESPONSE recive_str, string reply_result)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(recive_str);
            RecodeReportInfo(MSG_ROLE_OHXC, MSG_ROLE_VH,
                fun_name, seq_num, vh_id, vh_DO.OHTC_CMD, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(recive_str),
                //recive_str.ToString(),
                reply_result);//144          
        }

        public static void RecodeReportInfo(string vh_id, int seq_num, ID_143_STATUS_RESPONSE recive_str, string reply_result)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(recive_str);

            RecodeReportInfo(MSG_ROLE_VH, MSG_ROLE_OHXC,
                fun_name, seq_num, vh_id, vh_DO.OHTC_CMD, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                recive_str.ActionStatus.ToString(),
                jsonFormatter.Format(recive_str),
                //recive_str.ToString(),
                string.Empty);//144
        }
        public static void RecodeReportInfo(string vh_id, int seq_num, ID_43_STATUS_REQUEST send_str)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(send_str);
            RecodeReportInfo(MSG_ROLE_OHXC, MSG_ROLE_VH,
                fun_name, seq_num, vh_id, vh_DO.OHTC_CMD, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(send_str),
                //send_str.ToString(),
                string.Empty);//144          
        }

        public static void RecodeReportInfo(string vh_id, int seq_num, ID_144_STATUS_CHANGE_REP recive_str)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(recive_str);

            RecodeReportInfo(MSG_ROLE_VH, MSG_ROLE_OHXC,
                fun_name, seq_num, vh_id, vh_DO.OHTC_CMD, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                recive_str.ActionStatus.ToString(),
                jsonFormatter.Format(recive_str),
                //recive_str.ToString(),
                string.Empty);//144
        }
        public static void RecodeReportInfo(string vh_id, int seq_num, ID_44_STATUS_CHANGE_RESPONSE send_str, string reply_result)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(send_str);
            RecodeReportInfo(MSG_ROLE_OHXC, MSG_ROLE_VH,
                fun_name, seq_num, vh_id, vh_DO.OHTC_CMD, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(send_str),
                //send_str.ToString(),
                reply_result);//144          
        }

        public static void RecodeReportInfo(string vh_id, int seq_num, ID_71_RANGE_TEACHING_REQUEST send_str)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(send_str);

            RecodeReportInfo(MSG_ROLE_VH, MSG_ROLE_OHXC,
                fun_name, seq_num, vh_id, string.Empty, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(send_str),
                //send_str.ToString(),
                string.Empty);//144
        }
        public static void RecodeReportInfo(string vh_id, int seq_num, ID_171_RANGE_TEACHING_RESPONSE recive_str, string reply_result)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(recive_str);

            RecodeReportInfo(MSG_ROLE_OHXC, MSG_ROLE_VH,
                fun_name, seq_num, vh_id, string.Empty, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(recive_str),
                //recive_str.ToString(),
                reply_result);//144          
        }


        public static void RecodeReportInfo(string vh_id, int seq_num, ID_172_RANGE_TEACHING_COMPLETE_REPORT recive_str)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(recive_str);

            RecodeReportInfo(MSG_ROLE_VH, MSG_ROLE_OHXC,
                fun_name, seq_num, vh_id, vh_DO.OHTC_CMD, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(recive_str),
                //recive_str.ToString(),
                string.Empty);//144
        }
        public static void RecodeReportInfo(string vh_id, int seq_num, ID_72_RANGE_TEACHING_COMPLETE_RESPONSE send_str, string reply_result)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(send_str);
            RecodeReportInfo(MSG_ROLE_OHXC, MSG_ROLE_VH,
                fun_name, seq_num, vh_id, vh_DO.OHTC_CMD, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(send_str),
                //send_str.ToString(),
                reply_result);//144          
        }

        public static void RecodeReportInfo(string vh_id, int seq_num, ID_194_ALARM_REPORT recive_str)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(recive_str);

            RecodeReportInfo(MSG_ROLE_VH, MSG_ROLE_OHXC,
                fun_name, seq_num, vh_id, vh_DO.OHTC_CMD, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(recive_str),
                //recive_str.ToString(),
                string.Empty);//144
        }
        public static void RecodeReportInfo(string vh_id, int seq_num, ID_94_ALARM_RESPONSE send_str, string reply_result)
        {
            AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
            string fun_name = RenameFunName(send_str);
            RecodeReportInfo(MSG_ROLE_OHXC, MSG_ROLE_VH,
                fun_name, seq_num, vh_id, vh_DO.OHTC_CMD, string.Empty,
                vh_DO.MCS_CMD, //S2F49
                vh_DO.CUR_ADR_ID, vh_DO.CUR_SEC_ID, string.Empty, (uint)vh_DO.ACC_SEC_DIST, string.Empty, string.Empty, //134
                0, 0,//134 reply
                0,
                vh_DO.ACT_STATUS.ToString(),
                jsonFormatter.Format(send_str),
                //send_str.ToString(),
                reply_result);//144          
        }


        private static string RenameFunName(IMessage send_str)
        {
            string fun_name = send_str.Descriptor.Name;
            string[] fun_nameSplit = fun_name.Split('_');
            fun_name = string.Concat(fun_nameSplit[0], "_", fun_nameSplit[1]);
            return fun_name;
        }


        #endregion TCPIP Msg Log
        #region SECS Msg Log
        public static void RecodeReportInfo(SXFY sxfyMessage)
        {
            string sraeamfun_name = sxfyMessage.StreamFunction;
            string ceid_name = $"[{sxfyMessage.SystemByte}]{sxfyMessage.StreamFunction}-{sxfyMessage.StreamFunctionName}";

            RecodeReportInfo(MSG_ROLE_MCS, MSG_ROLE_OHXC,
                sraeamfun_name, sxfyMessage.SystemByte, string.Empty, string.Empty, string.Empty,
                string.Empty, //S2F49
                string.Empty, string.Empty, ceid_name, 0, string.Empty, string.Empty, //134 rep
                0, 0,//134 reply
                0,
                string.Empty,
                sxfyMessage.toSECSString(),
                string.Empty);//144          
        }
        public static void RecodeReportInfo(SXFY sxfyMessage, string mcsCmdID)
        {
            string sraeamfun_name = sxfyMessage.StreamFunction;
            string ceid_name = $"[{sxfyMessage.SystemByte}]{sxfyMessage.StreamFunction}-{sxfyMessage.StreamFunctionName}";

            RecodeReportInfo(MSG_ROLE_MCS, MSG_ROLE_OHXC,
                sraeamfun_name, sxfyMessage.SystemByte, string.Empty, string.Empty, string.Empty,
                mcsCmdID, //S2F49
                string.Empty, string.Empty, ceid_name, 0, string.Empty, string.Empty, //134 rep
                0, 0,//134 reply
                0,
                string.Empty,
                sxfyMessage.toSECSString(),
                string.Empty);//144          
        }
        public static void RecodeReportInfo(string vh_id, string mcs_cmd_id, SXFY send_str, string ceid)
        {
            string ohtc_cmd = string.Empty;
            string mcs_cmd = string.Empty;
            string cur_adr_id = string.Empty;
            string cur_sec_id = string.Empty;
            uint acc_sec_dist = 0;
            string act_status = string.Empty;
            mcs_cmd = mcs_cmd_id;
            if (!isEmpty(vh_id))
            {
                AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
                switch (ceid)
                {
                    case SECSConst.CEID_Transfer_Completed:
                    case SECSConst.CEID_Vehicle_Unassigned:
                        ohtc_cmd = string.Empty;
                        break;
                    default:
                        ohtc_cmd = vh_DO.OHTC_CMD;
                        break;
                }
                cur_adr_id = vh_DO.CUR_ADR_ID;
                cur_sec_id = vh_DO.CUR_SEC_ID;
                acc_sec_dist = (uint)vh_DO.ACC_SEC_DIST;
                act_status = vh_DO.ACT_STATUS.ToString();
            }

            string sraeamfun_name = send_str.StreamFunction;
            string ceid_name = $"[{send_str.SystemByte}]{send_str.StreamFunction}-{send_str.StreamFunctionName}";
            RecodeReportInfo(MSG_ROLE_OHXC, MSG_ROLE_MCS,
                sraeamfun_name, send_str.SystemByte, vh_id, ohtc_cmd, string.Empty,
                mcs_cmd,                                                                     //S2F49
                cur_adr_id, cur_sec_id, ceid_name, acc_sec_dist, string.Empty, string.Empty, //134 rep
                0, 0,//134 reply
                0,
                act_status,
                send_str.toSECSString(),
                string.Empty);
        }
        public static void RecodeReportInfo(string vh_id, string mcs_cmd_id, SXFY receive_str, string ceid, string reply_result)
        {
            string ohtc_cmd = string.Empty;
            string mcs_cmd = string.Empty;
            string cur_adr_id = string.Empty;
            string cur_sec_id = string.Empty;
            uint acc_sec_dist = 0;
            string act_status = string.Empty;
            mcs_cmd = mcs_cmd_id;
            if (!isEmpty(vh_id))
            {
                AVEHICLE vh_DO = SCApplication.getInstance().getEQObjCacheManager().getVehicletByVHID(vh_id);
                switch (ceid)
                {
                    case SECSConst.CEID_Transfer_Completed:
                    case SECSConst.CEID_Vehicle_Unassigned:
                        ohtc_cmd = string.Empty;
                        break;
                    default:
                        ohtc_cmd = vh_DO.OHTC_CMD;
                        break;
                }
                cur_adr_id = vh_DO.CUR_ADR_ID;
                cur_sec_id = vh_DO.CUR_SEC_ID;
                acc_sec_dist = (uint)vh_DO.ACC_SEC_DIST;
                act_status = vh_DO.ACT_STATUS.ToString();
            }

            string sraeamfun_name = receive_str.StreamFunction;

            RecodeReportInfo(MSG_ROLE_MCS, MSG_ROLE_OHXC,
                sraeamfun_name, receive_str.SystemByte, vh_id, ohtc_cmd, string.Empty,
                mcs_cmd,                                                                     //S2F49
                cur_adr_id, cur_sec_id, string.Empty, acc_sec_dist, string.Empty, string.Empty, //134 rep
                0, 0,//134 reply
                0,
                act_status,
                receive_str.toSECSString(),
                reply_result);
        }
        #endregion SECS Msg Log

        #region Connection Info

        public static void RecodeConnectionInfo(string id, string type, double time_interval)
        {
            String hostName = System.Net.Dns.GetHostName();
            dynamic logEntry = new JObject();
            logEntry.RPT_TIME = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture);

            logEntry.HOST_NAME = hostName;
            logEntry.VH_ID = id;
            logEntry.CONN_TYPE = type;
            logEntry.INTERVAL_TIME = (int)time_interval;

            var json = logEntry.ToString(Newtonsoft.Json.Formatting.None);
            json = json.Replace("RPT_TIME", "@timestamp");
            LogManager.GetLogger("RecodeConnectionInfo").Info(json);

        }
        #endregion Connection Info

        #region 
        /// <summary>
        /// 壓縮ByteArray資料
        /// </summary>
        /// <param name="arrayByte"></param>
        /// <returns></returns>
        public static string CompressArrayByte(byte[] arrayByte)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.IO.Compression.GZipStream compressedzipStream = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress, true);
            compressedzipStream.Write(arrayByte, 0, arrayByte.Length);
            compressedzipStream.Close();
            string compressStr = (string)(Convert.ToBase64String(ms.ToArray()));
            return compressStr;
        }

        /// <summary>
        /// 解壓縮BytyArray資料
        /// </summary>
        /// <param name="compressString">The compress string.</param>
        /// <returns>System.String.</returns>
        public static byte[] unCompressString(string compressString)
        {
            byte[] zippedData = Convert.FromBase64String(compressString.ToString());
            System.IO.MemoryStream ms = new System.IO.MemoryStream(zippedData);
            System.IO.Compression.GZipStream compressedzipStream = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress);
            System.IO.MemoryStream outBuffer = new System.IO.MemoryStream();
            byte[] block = new byte[1024];
            while (true)
            {
                int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                if (bytesRead <= 0)
                    break;
                else
                    outBuffer.Write(block, 0, bytesRead);
            }
            compressedzipStream.Close();
            return outBuffer.ToArray();
        }
        #endregion

        #region Context
        public static T getCallContext<T>(string key)
        {
            object obj = System.Runtime.Remoting.Messaging.CallContext.GetData(key);
            if (obj == null)
            {
                return default(T);
            }
            return (T)obj;
        }
        public static T getOrSetCallContext<T>(string key)
        {
            object obj = System.Runtime.Remoting.Messaging.CallContext.GetData(key);
            if (obj == null)
            {
                obj = Activator.CreateInstance(typeof(T));
                System.Runtime.Remoting.Messaging.CallContext.SetData(key, obj);
            }
            return (T)obj;
        }
        public static void setCallContext<T>(string key, T obj)
        {
            if (obj != null)
            {
                System.Runtime.Remoting.Messaging.CallContext.SetData(key, obj);
            }
        }
        public static void setCallContext(string key, object obj)
        {
            if (obj != null)
            {
                System.Runtime.Remoting.Messaging.CallContext.SetData(key, obj);
            }
        }
        #endregion Context

        public static void UserOperationLog(UserOperationLog log)
        {
            var json = JsonConvert.SerializeObject(log);
            LogManager.GetLogger("UserOperation").Info(json);
        }
    }

    public static class nameOfExtension
    {
        public static String nameof<T, TT>(this Expression<Func<T, TT>> accessor)
        {
            return nameof(accessor.Body);
        }

        public static String nameof<T>(this Expression<Func<T>> accessor)
        {
            return nameof(accessor.Body);
        }

        public static String nameof<T, TT>(this T obj, Expression<Func<T, TT>> propertyAccessor)
        {
            return nameof(propertyAccessor.Body);
        }

        private static String nameof(Expression expression)
        {
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpression = expression as MemberExpression;
                if (memberExpression == null)
                    return null;
                return memberExpression.Member.Name;
            }
            return null;
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }


    public class LogCollection
    {
        public const string DIR_RECIVE = "Recive";
        public const string DIR_SEND = "Send";

        public static Logger VHStateLogger = LogManager.GetLogger("VHState");
        public static Logger BlockControlLogger = LogManager.GetLogger("BlockControlLog");

        public static void VHStateDebug(string msg)
        {
            VHStateLogger.Debug(msg);
        }
        public static void VHStateWarn(string msg)
        {
            VHStateLogger.Warn(msg);
        }

    }

}