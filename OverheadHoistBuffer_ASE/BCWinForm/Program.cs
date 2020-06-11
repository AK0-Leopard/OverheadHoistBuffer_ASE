// ***********************************************************************
// Assembly         : BC
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : chou
// Last Modified On : 03-28-2016
// ***********************************************************************
// <copyright file="Program.cs" company="Mirle">
//     Copyright ©2014 MIRLE.3K0
// </copyright>
// <summary></summary>
// ***********************************************************************
using com.mirle.ibg3k0.bc.winform.UI;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.mirle.ibg3k0.bc.winform
{
    /// <summary>
    /// Class Program.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The application unique identifier
        /// </summary>
        static string appGuid = "{39ba7d51-cc5b-4b38-894e-313f12beb467}";

        static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        static NLog.Logger TransferServiceLogger = NLog.LogManager.GetLogger("TransferServiceLogger");

        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        /// <param name="args">The arguments.</param>
        [STAThread]
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        static void Main(string[] args)
        {
            using (Mutex m = new Mutex(false, "Global\\" + appGuid))
            {
                //MessageBox.Show("Test 1.0.0.2");
                SCUtility.SystemEventLog("Start BC System!!", EventLogEntryType.Information);
                //檢查是否同名Mutex已存在(表示另一份程式正在執行)
                if (!m.WaitOne(0, false))
                {
                    Console.WriteLine("Only one instance is allowed!");
                    SCUtility.SystemEventLog("Can Not Execute Multiple OHBC System!!", EventLogEntryType.Warning);
                    var confirmResult = MessageBox.Show("Can Not Execute Multiple OHBC System!!",
                            "Confirm Exit OHBC System!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                //////檢查是否有重複開啟BC System
                System.Diagnostics.Process crProcess = System.Diagnostics.Process.GetCurrentProcess();
                System.Diagnostics.Process[] myProcess = System.Diagnostics.Process.GetProcessesByName(crProcess.ProcessName);
                if (myProcess.Length > 1)
                {
                    SCUtility.SystemEventLog("Can Not Execute Multiple OHBC System!!", EventLogEntryType.Warning);
                    var confirmResult = MessageBox.Show("Can Not Execute Multiple OHBC System!",
                            "Confirm Exit OHBC System!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "程式執行開始-------------------------------------------");
                string str = Environment.CurrentDirectory;
                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + str);
                string ohbc_args = Environment.GetEnvironmentVariable("OHBCNAME");
                string ohbc_name = "";
                //Console.WriteLine("BCNAME:{0}", bcName);
                string argStr = SCUtility.stringListToString(" ", args.ToList());
                if (BCFUtility.isEmpty(argStr))
                {
                    //args = new string[] { bcName };
                    args = ohbc_args.Split(',');
                }
                else
                {
                    ohbc_args = args[0];
                }
                ohbc_name = args[0];
                //copyConfig(ohbc_args);
                copyConfig(ohbc_name);
                ConfigSystem.Install();

                //var wi = WindowsIdentity.GetCurrent();
                //var wp = new WindowsPrincipal(wi);
                //bool runAsAdmin = wp.IsInRole(WindowsBuiltInRole.Administrator);
                //if (!runAsAdmin)
                //{
                //    Console.WriteLine("Try Change Run As Admin.");
                //    MessageBox.Show("Try Change Run As Admin.");
                //    var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);
                //    processInfo.UseShellExecute = true;
                //    processInfo.Verb = "runas";


                //    processInfo.Arguments = argStr;
                //    // Start the new process
                //    try
                //    {
                //        Process.Start(processInfo);
                //    }
                //    catch (Exception)
                //    {
                //        Console.WriteLine("Sorry, can't to start " +
                //           "this program with administrator rights!");
                //        MessageBox.Show("Sorry, can't to start " +
                //           "this application with administrator rights!");
                //    }
                //    return;
                //}
                AppDomain currentDomain = AppDomain.CurrentDomain;                                  //A0.12
                currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);  //A0.12
                Application.ThreadException += Application_ThreadException;
                Thread.CurrentThread.CurrentUICulture = Properties.Settings.Default.UICulture;
                //Localization.BuildMultiLanguageResources();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new BCMainForm(args[0], args[1]));
                SCUtility.SystemEventLog("Close BC System!!", EventLogEntryType.Information);
                Application.ExitThread();
                //crProcess.Kill();
            }
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs args)
        {
            Exception e = args.Exception;
            NLog.LogManager.GetCurrentClassLogger().Error(e, "UnhandException - Application.ThreadException:");
        }
        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            NLog.LogManager.GetCurrentClassLogger().Error(e, "UnhandledException:");
        }

        /// <summary>
        /// Copies the configuration.
        /// </summary>
        /// <param name="bcid">The bcid.</param>
        private static void copyConfig(string bcid)
        {
            #region Copy App.Config to Run Time Dir
            string curDir = Environment.CurrentDirectory;
            string sourceFile = System.IO.Path.Combine(curDir, "Config", bcid, "App.Config");
            string destFile = System.IO.Path.Combine(curDir, "App.Config");
            System.IO.File.Copy(sourceFile, destFile, true);
            string exeName = Assembly.GetExecutingAssembly().GetName().Name;
            destFile = System.IO.Path.Combine(curDir, exeName + ".exe.config");
            System.IO.File.Copy(sourceFile, destFile, true);
            destFile = System.IO.Path.Combine(curDir, exeName + ".vshost.exe.config");
            System.IO.File.Copy(sourceFile, destFile, true);
            #endregion
        }
    }
}
