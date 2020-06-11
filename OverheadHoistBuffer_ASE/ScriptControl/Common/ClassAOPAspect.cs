using KingAOP.Aspects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Common
{
    public class ClassAOPAspect : OnMethodBoundaryAspect
    {
        Stopwatch sw = new Stopwatch();
        string SERVICE_ID_VEHICLE_SERVICE = "Vehicle Service";
        public override void OnEntry(MethodExecutionArgs args)
        {
            CallContext.SetData(LogHelper.CALL_CONTEXT_KEY_WORD_SERVICE_ID, SERVICE_ID_VEHICLE_SERVICE);

            Console.WriteLine();
            sw.Start();
            //string logData = CreateLogData("Entering", args);
            //Console.WriteLine(sw.GetHashCode());
            //Console.WriteLine(logData);

        }

        public override void OnExit(MethodExecutionArgs args)
        {
            sw.Stop();
            RecodeMethodExecuteInfo(args);
            Console.WriteLine(string.Format("Spend {0}ms", sw.ElapsedMilliseconds));
            sw.Reset();
        }

        private void RecodeMethodExecuteInfo(MethodExecutionArgs args)
        {
            dynamic logEntry = new JObject();
            logEntry.RPT_TIME = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture);
            logEntry.CLASS_NAME = args.Method.DeclaringType.Name;
            logEntry.METHOD_NAME = args.Method.Name;
            logEntry.PROC_TIME = sw.ElapsedMilliseconds;
            logEntry.EXECTION = args.Exception != null ? args.Exception.ToString() : string.Empty;
            var json = logEntry.ToString(Formatting.None);
            json= json.Replace("RPT_TIME", "@timestamp");
            LogManager.GetLogger("AOP_MethodExecuteInfo").Info(json);
        }

        public override void OnException(MethodExecutionArgs args)
        {
            Exception ex = args.Exception;
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            Console.WriteLine("OnSuccess");

        }

        private string CreateLogData(string methodStage, MethodExecutionArgs args)
        {
            var str = new StringBuilder();
            //str.AppendLine();
            if (args.Instance != null)
            {
                var obj = args.Instance;
            }
            str.AppendLine(string.Format("Class: {0} ", args.Instance));
            str.AppendLine(string.Format("Method: {0} ", args.Method));

            foreach (var argument in args.Arguments)
            {
                var argType = argument.GetType();
                str.Append(argType.Name + ": ");

                if (argType == typeof(string) || argType.IsPrimitive)
                {
                    str.Append(argument);
                }
                else
                {
                    foreach (var property in argType.GetProperties())
                    {
                        str.AppendFormat("{0} = {1}; ",
                            property.Name, property.GetValue(argument, null));
                    }
                }
            }
            return str.ToString();
        }
    }
}
