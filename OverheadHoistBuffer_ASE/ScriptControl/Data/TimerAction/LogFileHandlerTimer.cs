using com.mirle.ibg3k0.bcf.Data.TimerAction;
using com.mirle.ibg3k0.sc.App;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Configuration;
using com.mirle.ibg3k0.sc.Common;
using System.Threading;


namespace com.mirle.ibg3k0.sc.Data.TimerAction
{
    class LogFileHandlerTimer : ITimerAction
    {
        protected SCApplication scApp = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public LogFileHandlerTimer(string name, long intervalMilliSec)
            : base(name, intervalMilliSec)
        {
            _DefaultLogFilePath = getString("LogFilePath", @"D:\LogFiles\OHxC");
            _KeepLogDay = getInt("LogKeepData", 60);
        }
        private string getString(string key, string defaultValue)
        {
            string rtn = defaultValue;
            try
            {
                rtn = ConfigurationManager.AppSettings.Get(key);
                if (SCUtility.isEmpty(rtn))
                {
                    rtn = defaultValue;
                }
            }
            catch (Exception e)
            {
                logger.Warn("Get Config error[key:{0}][Exception:{1}]", key, e);
            }
            return rtn;
        }
        private int getInt(string key, int defaultValue)
        {
            int rtn = defaultValue;
            try
            {
                bool is_success = int.TryParse(key, out int value);
                if (is_success)
                    return value;
                else
                    return defaultValue;

            }
            catch (Exception e)
            {
                logger.Warn("Get Config error[key:{0}][Exception:{1}]", key, e);
            }
            return rtn;
        }
        private string _DefaultLogFilePath = @"D:\LogFiles\OHxC";
        private int _CompressLogDay = 1;
        private int _KeepLogDay = 90;
        private DateTime LastProcessDateTime = DateTime.MinValue;
        /// <summary>
        /// The synchronize point
        /// </summary>
        private long syncPoint = 0;
        /// <summary>
        /// Timer Action的執行動作
        /// </summary>
        /// <param name="obj">The object.</param>
        public override void doProcess(object obj)
        {
            if (System.Threading.Interlocked.Exchange(ref syncPoint, 1) == 0)
            {
                try
                {// 2021-09-31 06:00          2021-09-31 06:01 
                    if (LastProcessDateTime > DateTime.Now.AddDays(_CompressLogDay * -1))
                    {
                        return;
                    }
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(LogFileHandlerTimer), Device: string.Empty,
                       Data: $"Start process log handler,path:{_DefaultLogFilePath}");
                    LastProcessDateTime = DateTime.Now;

                    var dirLogPath = new DirectoryInfo(_DefaultLogFilePath);
                    foreach (var directoryInfo in dirLogPath.GetDirectories())
                    {
                        var objDateTime = DateTime.Now;
                        if (!directoryInfo.Name.Contains("_")) continue;
                        string log_data = directoryInfo.Name.Split('_').Last();
                        if (DateTime.TryParse(log_data, out objDateTime))
                        {
                            if (objDateTime <= DateTime.Now.AddDays(_KeepLogDay * -1))
                            {
                                directoryInfo.Delete(true);
                                SpinWait.SpinUntil(() => false, 1000);
                            }
                        }
                    }

                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(LogFileHandlerTimer), Device: string.Empty,
                       Data: $"End process log handler,path:{_DefaultLogFilePath}");
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception:");
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref syncPoint, 0);
                }
            }
        }

        public override void initStart()
        {
            scApp = SCApplication.getInstance();
        }
    }
}
