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
                portType portType = port_type_get_result.portType;
                ////app.ManualPortControlService.GetPortPlcState(portID, out var temp);
                //#region ManuPortPLCInfo to manualPort(Proto type)
                //data.AlarmCode = temp.AlarmCode;
                //data.CarrierIdOfStage1 = temp.CarrierIdOfStage1;
                //data.CarrierIdReadResult = temp.CarrierIdReadResult;
                //data.CstTypes = temp.CstTypes;
                //data.ErrorIndex = temp.ErrorIndex;
                //data.IsAlarm = temp.IsAlarm;
                //data.IsBcrReadDone = temp.IsBcrReadDone;
                //data.IsDirectionChangable = temp.IsDirectionChangable;
                //data.IsDoorOpen = temp.IsDoorOpen;
                //data.IsDown = temp.IsDown;
                //data.IsHeartBeatOn = temp.IsHeartBeatOn;
                //data.IsInMode = temp.IsInMode;
                //data.IsLoadOK = temp.IsLoadOK;
                //data.IsOutMode = temp.IsOutMode;
                //data.IsRemoveCheck = temp.IsRemoveCheck;
                //data.IsRun = temp.IsRun;
                //data.IsTransferComplete = temp.IsTransferComplete;
                //data.IsUnloadOK = temp.IsUnloadOK;
                //data.IsWaitIn = temp.IsWaitIn;
                //data.IsWaitOut = temp.IsWaitOut;
                //data.LoadPosition1 = temp.LoadPosition1;
                //data.LoadPosition2 = temp.LoadPosition2;
                //data.LoadPosition3 = temp.LoadPosition3;
                //data.LoadPosition4 = temp.LoadPosition4;
                //data.LoadPosition5 = temp.LoadPosition5;
                //data.ManualPortId = port.PORT_ID;
                //data.RunEnable = temp.RunEnable;
                //data.AddressID = port.ADR_ID;
                //#endregion

                //result.ManualPortInfo.Add(data);
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
    }
}
