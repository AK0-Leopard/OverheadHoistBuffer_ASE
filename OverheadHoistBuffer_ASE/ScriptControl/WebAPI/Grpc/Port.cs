using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMessage.ProtocolFormat.PortFun;
using Grpc.Core;
namespace com.mirle.ibg3k0.sc.WebAPI.Grpc
{
    internal class Port : PortGreeter.PortGreeterBase
    {
        App.SCApplication app;
        public Port(App.SCApplication _app)
        {
            app = _app;
        }

        public override Task<replyPortInfo> getAllPortInfo(Empty empty, ServerCallContext context)
        {
            replyPortInfo result = new replyPortInfo();
            var ports = app.TransferService.GetCVPort();

            foreach (var port in ports)
            {
                string port_id = port.PortName;
                var port_type_get_result = tryGetPortType(port.UnitType);
                if (!port_type_get_result.isExist) continue;

                var temp = app.TransferService.GetPLC_PortData(port_id);
                if (temp is null) continue;
                
                #region PLC_PortData to portInfo
                portInfo info = new portInfo();
                info.PortID = port_id;
                info.PortType = port_type_get_result.portType;
                info.OpAutoMode = temp.OpAutoMode;

                info.OpManualMode = temp.OpManualMode;
                info.OpError = temp.OpError;

                info.IsInputMode = temp.IsInputMode;
                info.IsOutputMode = temp.IsOutputMode;
                info.IsModeChangable = temp.IsModeChangable;

                info.IsAGVMode = temp.IsAGVMode;
                info.IsMGVMode = temp.IsMGVMode;

                info.PortWaitIn = temp.PortWaitIn;
                info.PortWaitOut = temp.PortWaitOut;

                info.IsAutoMode = temp.IsAutoMode;

                info.IsReadyToLoad = temp.IsReadyToLoad;
                info.IsReadyToUnload = temp.IsReadyToUnload;

                info.LoadPosition1 = temp.LoadPosition1;
                info.LoadPosition2 = temp.LoadPosition2;
                info.LoadPosition3 = temp.LoadPosition3;
                info.LoadPosition4 = temp.LoadPosition4;
                info.LoadPosition5 = temp.LoadPosition5;
                info.LoadPosition7 = temp.LoadPosition7;
                info.LoadPosition6 = temp.LoadPosition6;

                info.IsCSTPresence = temp.IsCSTPresence;
                info.AGVPortReady = temp.AGVPortReady;
                info.CanOpenBox = temp.CanOpenBox;
                info.IsBoxOpen = temp.IsBoxOpen;

                info.BCRReadDone = temp.BCRReadDone;
                info.CSTPresenceMismatch = temp.CSTPresenceMismatch;
                info.IsTransferComplete = temp.IsTransferComplete;
                info.CstRemoveCheck = temp.CstRemoveCheck;

                info.ErrorCode = Convert.ToInt32(temp.ErrorCode);

                info.BoxID = temp.BoxID ?? "";

                info.LoadPositionBOX1 = temp.LoadPositionBOX1 ?? "";
                info.LoadPositionCST1 = "";

                info.LoadPositionBOX2 = temp.LoadPositionBOX2 ?? "";
                info.LoadPositionCST2 = "";

                info.LoadPositionBOX3 = temp.LoadPositionBOX3 ?? "";
                info.LoadPositionCST3 = "";

                info.LoadPositionBOX4 = temp.LoadPositionBOX4 ?? "";
                info.LoadPositionCST4 = "";

                info.LoadPositionBOX5 = temp.LoadPositionBOX5 ?? "";
                info.LoadPositionCST5 = "";

                info.CassetteID = temp.CassetteID ?? "";

                info.FireAlarm = temp.FireAlarm;

                info.CimOn = temp.cim_on;

                info.PreLoadOK = temp.preLoadOK;

                info.ADRID = port.ADR_ID;
                info.Stage = port.Stage;
                info.IsInService = app.PortDefBLL.GetPortData(port_id).IsAutoMode;
                info.UnitType = port.UnitType ?? "";
                info.ZoneName = port.ZoneName ?? "";
                info.AGVStationStatus = app.TransferService.GetAGV_StationStatus(port_id) ?? "";
                info.AGVAutoPortType = app.TransferService.GetAGV_AutoPortType(port_id) ?? "";
                #endregion
                result.PortInfoList.Add(info);
            }
            return Task.FromResult(result);
        }

        private (bool isExist, portType portType) tryGetPortType(string unitType)
        {
            bool is_exist = Enum.TryParse<Service.UnitType>(unitType, out Service.UnitType unit_type);
            if (!is_exist) return (false, default(portType));
            switch (unit_type)
            {
                case Service.UnitType.AGV:
                    return (true, portType.Station);
                case Service.UnitType.NTB:
                case Service.UnitType.OHCV:
                case Service.UnitType.STK:
                    return (true, portType.Cv);
                default:
                    return (false, portType.Cv);
            }
        }

        public override Task<replyPortMng> setPortRun(requestPortMng req, ServerCallContext context)
        {
            replyPortMng result = new replyPortMng();
            result.IsSuccess = app.TransferService.SetPortRun(req.PortID);
            result.Result = result.IsSuccess ? "" : $"Exception happened! (TransferServiceLogger)";
            return Task.FromResult(result);
        }

        public override Task<replyPortMng> setPortStop(requestPortMng req, ServerCallContext context)
        {
            replyPortMng result = new replyPortMng();
            result.IsSuccess = app.TransferService.SetPortStop(req.PortID);
            result.Result = result.IsSuccess ? "" : $"Exception happened! (TransferServiceLogger)";
            return Task.FromResult(result);
        }

        public override Task<replyPortMng> resetPortAlarm(requestPortMng req, ServerCallContext context)
        {
            replyPortMng result = new replyPortMng();
            result.IsSuccess = app.TransferService.PortAlarrmReset(req.PortID);
            result.Result = result.IsSuccess ? "" : $"Exception happened! (TransferServiceLogger)";
            return Task.FromResult(result);
        }

        public override Task<replyPortMng> setPortDir(requestPortDir req, ServerCallContext context)
        {
            replyPortMng result = new replyPortMng();
            E_PortType portType = req.PortDir == portDir.In ? E_PortType.In : E_PortType.Out;
            result.IsSuccess = app.TransferService.PortTypeChange(req.PortID, portType, "gRPC PortFun.PortGreeter.setPortDir");
            result.Result = result.IsSuccess ? "" : $"Exception happened! (TransferServiceLogger)";
            return Task.FromResult(result);
        }

        public override Task<replyPortMng> setPortWaitIn(requestPortMng req, ServerCallContext context)
        {
            replyPortMng result = new replyPortMng();
            result.IsSuccess = false;
            result.Result = $"Failed! plcInfo.LoadPosition1 = false";
            Data.PLC_Functions.PortPLCInfo plcInfo = app.TransferService.GetPLC_PortData(req.PortID);
            if (plcInfo.LoadPosition1)
            {
                result.IsSuccess = app.TransferService.PLC_ReportPortWaitIn(plcInfo, "gRPC PortFun.PortGreeter.setPortWaitIn");
                result.Result = result.IsSuccess ? "" : $"Failed! (TransferServiceLogger)";
            }
            return Task.FromResult(result);
        }

        public override Task<replyPortMng> openAgvStation(requestAgvStationOpen req, ServerCallContext context)
        {
            replyPortMng result = new replyPortMng();
            result.IsSuccess = Convert.ToBoolean(app.TransferService.OpenAGV_Station(req.PortID, req.Open, "gRPC PortFun.PortGreeter.openAgvStation"));
            result.Result = result.IsSuccess ? "" : $"Failed! (TransferServiceLogger)";
            return Task.FromResult(result);
        }
    }
}
