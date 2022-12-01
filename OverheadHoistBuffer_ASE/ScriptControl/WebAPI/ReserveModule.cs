using com.mirle.ibg3k0.bcf.Data.TimerAction;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Common.Interface;
using com.mirle.ibg3k0.sc.Data.Expansion;
using com.mirle.ibg3k0.sc.WebAPI.Expansion;
using Grpc.Core;
using Mirle.Hlts.Utils;
using Mirle.Protos.ReserveModule;
using NLog;
using RailChangerProtocol;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace com.mirle.ibg3k0.sc.WebAPI
{
    public class ReserveModule : IReserveModule
    {

        double ASK_TIME_OUT_SECONDS = 5;
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        sc.App.SCApplication scApp = null;
        Channel channel = null;
        Mirle.Protos.ReserveModule.ReserveService.ReserveServiceClient client = null;
        RemoveReserveModuleTimerAction removeReserveModuleTimerAction = null;
        ALINE line = null;
        public ReserveModule(sc.App.SCApplication _scApp)
        {
            scApp = _scApp;
            string s_grpc_client_ip = scApp.getString("gRPCClientIP", "127.0.0.1");
            string s_grpc_client_port = scApp.getString("gRPCClientPort", "25000");
            int.TryParse(s_grpc_client_port, out int i_grpc_client_port);
            channel = new Channel(s_grpc_client_ip, i_grpc_client_port, ChannelCredentials.Insecure);
            client = new Mirle.Protos.ReserveModule.ReserveService.ReserveServiceClient(channel);
        }
        public void Start(sc.App.SCApplication _scApp)
        {
            line = scApp.getEQObjCacheManager().getLine();
            if (!Service.VehicleService.IsOneVehicleSystem)
            {
                removeReserveModuleTimerAction = new RemoveReserveModuleTimerAction(this, line, "RemoveReserveModuleTimerAction", 1000);
                removeReserveModuleTimerAction.start();
            }
        }

        public void sendAlive()
        {
            try
            {
                var ask = client.Alive
                                 (new Mirle.Protos.ReserveModule.Empty(),
                                  deadline: DateTime.UtcNow.AddSeconds(ASK_TIME_OUT_SECONDS));
                line.ResetReserveModuleLastAskTime();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }
        #region TryAddReservedSection
        public HltResult TryAddReservedSection(string vhID, string sectionID, HltDirection sensorDir = HltDirection.Forward, HltDirection forkDir = HltDirection.None, bool isAsk = false)
        {
            try
            {
                ReserveInfo reserveInfo = new ReserveInfo()
                {
                    VehicleId = vhID,
                    ReservedSecId = sectionID,
                    SecsorDirection = sensorDir.convert2Direction(),
                    ForkDirection = forkDir.convert2Direction(),
                    IsAsk = isAsk
                };
                var ask = client.TryAddReservedSection
                                 (reserveInfo,
                                  deadline: DateTime.UtcNow.AddSeconds(ASK_TIME_OUT_SECONDS));
                line.ResetReserveModuleLastAskTime();

                return ask.convert2HltResult();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return new HltResult() { OK = false, Description = "WebAPI Exception" };
            }
        }
        #endregion TryAddReservedSection
        #region GetCurrentReserveInfoMap
        public System.Windows.Media.Imaging.BitmapSource GetCurrentReserveInfoMap()
        {
            try
            {
                var result = client.GetCurrentReserveInfoMap
                                    (new Mirle.Protos.ReserveModule.Empty(),
                                     deadline: DateTime.UtcNow.AddSeconds(ASK_TIME_OUT_SECONDS));
                var byte_array = result.MapDate.ToByteArray();
                var bit_map = BytesToBitmap(byte_array);
                var bit_map_source = Convert(bit_map);
                line.ResetReserveModuleLastAskTime();

                return bit_map_source;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        private static Bitmap BytesToBitmap(byte[] b)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(b);
            Bitmap bmp = (Bitmap)Bitmap.FromStream(ms);
            return bmp;
        }
        private static BitmapSource Convert(System.Drawing.Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgr24, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);

            return bitmapSource;
        }
        #endregion GetCurrentReserveInfoMap
        #region TryAddOrUpdateVehicle
        public HltResult TryAddOrUpdateVehicle(AVEHICLE vh)
        {
            try
            {
                var ask = client.TryAddOrUpdateVehicle
                                 (vh.convert2VehicleInfoForReserveModule(),
                                  deadline: DateTime.UtcNow.AddSeconds(ASK_TIME_OUT_SECONDS));
                line.ResetReserveModuleLastAskTime();

                return ask.convert2HltResult();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return new HltResult() { OK = false, Description = "WebAPI Exception" };
            }
        }
        #endregion TryAddOrUpdateVehicle
        #region RemoveManyReservedSectionsByVIDSID
        public void RemoveManyReservedSectionsByVIDSID(string vhID, string sectionID)
        {
            try
            {
                RemoveManyReservedInfo info = new RemoveManyReservedInfo()
                {
                    VehicleId = vhID,
                    SectionId = sectionID
                };
                client.RemoveManyReservedSectionsByVIDSID
                       (info,
                        deadline: DateTime.UtcNow.AddSeconds(ASK_TIME_OUT_SECONDS));
                line.ResetReserveModuleLastAskTime();

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }
        #endregion RemoveManyReservedSectionsByVIDSID
        #region RemoveVehicle
        public void RemoveVehicle(string vhID)
        {
            try
            {
                VehicleID info = new VehicleID()
                {
                    VehicleId = vhID
                };
                client.RemoveVehicle
                       (info,
                        deadline: DateTime.UtcNow.AddSeconds(ASK_TIME_OUT_SECONDS));
                line.ResetReserveModuleLastAskTime();

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }
        #endregion RemoveVehicle
        #region GetCurrentReserveSection
        public List<com.mirle.ibg3k0.sc.Data.VO.ReservedSection> GetCurrentReserveSections(string vhID)
        {
            try
            {
                VehicleID info = new VehicleID()
                {
                    VehicleId = vhID
                };
                var ask = client.GetCurrentReserveSection
                                 (info,
                                  deadline: DateTime.UtcNow.AddSeconds(ASK_TIME_OUT_SECONDS));
                var resered_sections = ask.ReservedSections.
                                           Select(sec => new com.mirle.ibg3k0.sc.Data.VO.ReservedSection(sec.VehicleId, sec.SecionId)).
                                           ToList();
                line.ResetReserveModuleLastAskTime();

                return resered_sections;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return new List<com.mirle.ibg3k0.sc.Data.VO.ReservedSection>();
            }
        }
        #endregion GetCurrentReserveSection
        #region RemoveAllReservedSectionsByVehicleID
        public void RemoveAllReservedSectionsByVehicleID(string vhID)
        {
            try
            {
                VehicleID info = new VehicleID()
                {
                    VehicleId = vhID
                };
                var ask = client.RemoveAllReservedSectionsByVehicleID
                                 (info,
                                  deadline: DateTime.UtcNow.AddSeconds(ASK_TIME_OUT_SECONDS));
                line.ResetReserveModuleLastAskTime();

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }
        #endregion RemoveAllReservedSectionsByVehicleID
        #region RemoveAllReservedSections
        public void RemoveAllReservedSections()
        {
            try
            {
                Mirle.Protos.ReserveModule.Empty info = new Mirle.Protos.ReserveModule.Empty();
                client.RemoveAllReservedSections
                       (info,
                        deadline: DateTime.UtcNow.AddSeconds(ASK_TIME_OUT_SECONDS));
                line.ResetReserveModuleLastAskTime();

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }
        #endregion RemoveAllReservedSections

        public class RemoveReserveModuleTimerAction : ITimerAction
        {
            private static Logger logger = LogManager.GetCurrentClassLogger();
            private static ReserveModule webReserveModule;
            ALINE line = null;
            public RemoveReserveModuleTimerAction(ReserveModule _webReserveModule, ALINE _line, string name, long intervalMilliSec)
                : base(name, intervalMilliSec)
            {
                webReserveModule = _webReserveModule;
                line = _line;
            }

            public override void initStart()
            {
            }

            int INTERVAL_ASK_TIME_MS = 10_000;
            private long syncPoint = 0;
            public override void doProcess(object obj)
            {
                if (System.Threading.Interlocked.Exchange(ref syncPoint, 1) == 0)
                {
                    try
                    {
                        if (!line.ReserveModuleLastAskTime.IsRunning  ||
                            line.ReserveModuleLastAskTime.ElapsedMilliseconds > INTERVAL_ASK_TIME_MS)
                            webReserveModule.sendAlive();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(AVEHICLE), Device: "OHBC",
                           Data: ex);
                    }
                    finally
                    {
                        System.Threading.Interlocked.Exchange(ref syncPoint, 0);
                    }

                }
            }

        }

    }
}
