using AspectInjector.Broker;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace com.mirle.ibg3k0.sc.Common.AOP
{
    [Aspect(Scope.Global)]
    [Injection(typeof(TeaceMethodAspectAttribute))]
    public class TeaceMethodAspectAttribute : Attribute
    {
        public TeaceMethodAspectAttribute()
        {

        }

        NLog.Logger logger = LogManager.GetLogger("AOP_MethodExecuteInfo");
        StopWatchPool stopWatchPool = new StopWatchPool();

        [Advice(Kind.Before, Targets = Target.Method)]
        public void Before([Argument(Source.Name)] string name, [Argument(Source.Metadata)] System.Reflection.MethodBase methodBase, [Argument(Source.Arguments)] object[] arguments)
        {
            //LogDebug("On Before", name, methodBase);
        }

        private void LogDebug(string action, string name, MethodBase methodBase)
        {
            string name_full_name = tryGetNameSpace(methodBase);
            string thread_id = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
            string time = DateTime.Now.ToString(App.SCAppConstants.DateTimeFormat_19);
            logger.Debug($"{time},{action},{name_full_name},{name},{thread_id}");
        }
        private void LogWarn(string action, string name, MethodBase methodBase, long processTime)
        {
            string name_full_name = tryGetNameSpace(methodBase);
            string thread_id = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
            string time = DateTime.Now.ToString(App.SCAppConstants.DateTimeFormat_19);
            logger.Warn($"{time},{action},{name_full_name},{name},{thread_id},{processTime}");
        }
        private string tryGetNameSpace(MethodBase methodBase)
        {
            if (methodBase == null)
                return "";
            return methodBase.DeclaringType.FullName;
        }
        const int MAX_ALLOW_PROCESS_TIME_ms = 1_000;
        [Advice(Kind.After, Targets = Target.Method)]
        public void After([Argument(Source.Name)] string name, [Argument(Source.Metadata)] System.Reflection.MethodBase methodBase, [Argument(Source.Arguments)] object[] arguments, [Argument(Source.ReturnValue)] object returnValue)
        {
            //LogDebug("On After", name, methodBase);
        }


        [Advice(Kind.Around, Targets = Target.Method)]
        public object Around(
            [Argument(Source.Name)] string name,
            [Argument(Source.Metadata)] System.Reflection.MethodBase methodBase,
            [Argument(Source.Arguments)] object[] arguments,
            [Argument(Source.Target)] Func<object[], object> target)
        {
            var sw = stopWatchPool.GetObject();
            sw.Restart();
            try
            {
                var result = target(arguments);
                sw.Stop();
                if (sw.ElapsedMilliseconds > MAX_ALLOW_PROCESS_TIME_ms)
                {
                    LogWarn("On Around After", name, methodBase, sw.ElapsedMilliseconds);
                }
                return result;

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                throw ex;
            }
            finally
            {
                sw.Reset();
                stopWatchPool.PutObject(sw);
            }
        }

        private class StopWatchPool
        {
            private ConcurrentBag<Stopwatch> swObjects = new ConcurrentBag<Stopwatch>();
            public Stopwatch GetObject()
            {

                Stopwatch item;
                if (swObjects.TryTake(out item)) return item;
                return new Stopwatch();
            }
            public void PutObject(Stopwatch item)
            {
                if (item == null)
                    return;

                if (!(item is Stopwatch))
                    return;
                {
                    Stopwatch sw = item;
                    if (sw.IsRunning) sw.Stop();
                    sw.Reset();
                }
            }
        }

    }
}
