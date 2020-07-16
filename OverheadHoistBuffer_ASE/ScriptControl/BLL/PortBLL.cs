using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class PortBLL
    {
        SCApplication app = null;
        public DB OperateDB { private set; get; }
        public Catch OperateCatch { private set; get; }
        public Redis redis { private set; get; }

        public PortBLL()
        {
        }
        public void start(SCApplication _app)
        {
            app = _app;
            OperateDB = new DB();
            OperateCatch = new Catch(_app.getEQObjCacheManager());
            redis = new Redis(_app.getRedisCacheManager());
        }
        public class DB
        {

        }
        public class Catch
        {
            EQObjCacheManager CacheManager;
            public Catch(EQObjCacheManager _cache_manager)
            {
                CacheManager = _cache_manager;
            }


            public List<APORT> loadAGVStationPorts()
            {
                var ports = CacheManager.getAllPort();
                var ports_agv_station = ports.Where(port => port.PORT_ID.Contains("_A")).ToList();
                return ports_agv_station;
            }
        }
        public class Redis
        {
            private readonly RedisCacheManager redisCacheManager;

            public Redis(RedisCacheManager redisCacheManager)
            {
                this.redisCacheManager = redisCacheManager;
            }
            public void SetPortInfo(string portID, Data.PLC_Functions.PortPLCInfo portPLCInfo)
            {
                if (portPLCInfo == null) return;
                string hash_field = portID;
                byte[] arrayByte = convert2PortInfo(portID, portPLCInfo);

                redisCacheManager.HashSetAsync(SCAppConstants.REDIS_KEY_CURRENT_PORTS_INFO, hash_field, arrayByte);
            }
        }
        public void PublishPortInfo(string portID, PortPLCInfo portPLCInfo)
        {
            if (portPLCInfo == null) return;
            string hash_field = portID;
            byte[] arrayByte = convert2PortInfo(portID, portPLCInfo);

            app.getNatsManager().PublishAsync
                (string.Format(SCAppConstants.NATS_SUBJECT_Port_INFO_0, portID), arrayByte);
        }


        public static byte[] convert2PortInfo(string portID, PortPLCInfo portPLCInfo)
        {
            ProtocolFormat.OHTMessage.PORT_INFO port_info = new PORT_INFO();
            port_info.IsAutoMode = portPLCInfo.IsAutoMode;
            port_info.OpAutoMode = portPLCInfo.OpAutoMode;
            port_info.OpManualMode = portPLCInfo.OpManualMode;
            port_info.OpError = portPLCInfo.OpError;
            port_info.IsInputMode = portPLCInfo.IsInputMode;
            port_info.IsOutputMode = portPLCInfo.IsOutputMode;
            port_info.IsModeChangable = portPLCInfo.IsModeChangable;
            port_info.IsAGVMode = portPLCInfo.IsAGVMode;
            port_info.IsMGVMode = portPLCInfo.IsMGVMode;
            port_info.PortWaitIn = portPLCInfo.PortWaitIn;
            port_info.PortWaitOut = portPLCInfo.PortWaitOut;
            port_info.IsReadyToLoad = portPLCInfo.IsReadyToLoad;
            port_info.IsReadyToUnload = portPLCInfo.IsReadyToUnload;
            port_info.LoadPosition1 = portPLCInfo.LoadPosition1;
            port_info.LoadPosition2 = portPLCInfo.LoadPosition2;
            port_info.LoadPosition3 = portPLCInfo.LoadPosition3;
            port_info.LoadPosition4 = portPLCInfo.LoadPosition4;
            port_info.LoadPosition5 = portPLCInfo.LoadPosition5;
            port_info.LoadPosition7 = portPLCInfo.LoadPosition7;
            port_info.LoadPosition6 = portPLCInfo.LoadPosition6;
            port_info.IsCSTPresence = portPLCInfo.IsCSTPresence;
            port_info.AGVPortReady = portPLCInfo.AGVPortReady;
            port_info.CanOpenBox = portPLCInfo.CanOpenBox;
            port_info.IsBoxOpen = portPLCInfo.IsBoxOpen;
            port_info.BCRReadDone = portPLCInfo.BCRReadDone;
            port_info.CSTPresenceMismatch = portPLCInfo.CSTPresenceMismatch;
            port_info.IsTransferComplete = portPLCInfo.IsTransferComplete;
            port_info.CstRemoveCheck = portPLCInfo.CstRemoveCheck;
            port_info.ErrorCode = portPLCInfo.ErrorCode;
            port_info.BoxID = portPLCInfo.BoxID;
            port_info.CassetteID = portPLCInfo.CassetteID;
            port_info.PortID = portID;

            port_info.LoadPositionBOX1 = portPLCInfo.LoadPositionBOX1;
            port_info.LoadPositionBOX2 = portPLCInfo.LoadPositionBOX2;
            port_info.LoadPositionBOX3 = portPLCInfo.LoadPositionBOX3;
            port_info.LoadPositionBOX4 = portPLCInfo.LoadPositionBOX4;
            port_info.LoadPositionBOX5 = portPLCInfo.LoadPositionBOX5;
            port_info.FireAlarm = portPLCInfo.FireAlarm;
            
            byte[] arrayByte = new byte[port_info.CalculateSize()];
            port_info.WriteTo(new Google.Protobuf.CodedOutputStream(arrayByte));
            return arrayByte;
        }


        public static PORT_INFO Convert2Object_PortInfo(byte[] raw_data)
        {
            return ToObject<PORT_INFO>(raw_data);
        }

        private static T ToObject<T>(byte[] buf) where T : Google.Protobuf.IMessage<T>, new()
        {
            if (buf == null)
                return default(T);
            Google.Protobuf.MessageParser<T> parser = new Google.Protobuf.MessageParser<T>(() => new T());
            return parser.ParseFrom(buf);
        }
    }
}