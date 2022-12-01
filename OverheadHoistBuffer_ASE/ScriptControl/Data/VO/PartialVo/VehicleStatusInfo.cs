using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.VO.PartialVo
{
    public class VehicleStatusInfo
    {
        enum ControlStatus
        {
            None,
            Local,
            Remote
        }
        enum VehicleStatus
        {
            Manual,
            Auto
        }
        enum CommandStatus
        {
            NoCommand,
            MCS,
            OHTC,
            CycleMove,
            PreAssign
        }
        enum VehicleState
        {
            Remove,
            Install
        }
        enum RepairStatus
        {
            None,
            Repair,
            Matain
        }
        enum VhErrorStatus
        {
            NoAlarm,
            AlarmSet,
            WaittingAlarmConfirm
        }
        enum ChargeStatus
        {
            NoCharge,
            Charging
        }
        enum OpreationsTime
        {
            Idle,
            Run,
            ScheduledDowntime,
            UnscheduleDowntime
        }




        public VehicleStatusInfo(AVEHICLE _vh)
        {
            vh = _vh;
        }
        AVEHICLE vh;
        bool IsConnected { get { return vh.isTcpIpConnect; } }
        ControlStatus controlStatus
        {
            get
            {
                switch (vh.MODE_STATUS)
                {
                    case ProtocolFormat.OHTMessage.VHModeStatus.AutoLocal:
                        return ControlStatus.Local;
                    case ProtocolFormat.OHTMessage.VHModeStatus.AutoRemote:
                        return ControlStatus.Remote;
                    default:
                        return ControlStatus.None;
                }
            }
        }
        //
        VehicleStatus vehicleStatus
        {
            get
            {
                switch (vh.MODE_STATUS)
                {
                    case ProtocolFormat.OHTMessage.VHModeStatus.AutoLocal:
                    case ProtocolFormat.OHTMessage.VHModeStatus.AutoRemote:
                        return VehicleStatus.Auto;
                    default:
                        return VehicleStatus.Manual;
                }
            }
        }

        CommandStatus commandStatus
        {
            get
            {
                var cmd_ohtc = ACMD_OHTC.getExcuteCmdOhtcOfCmdObj(vh.VEHICLE_ID);
                if (vh.ACT_STATUS == ProtocolFormat.OHTMessage.VHActionStatus.Commanding && cmd_ohtc != null)
                {
                    if (sc.Common.SCUtility.isEmpty(cmd_ohtc.CMD_ID_MCS))
                    {
                        if (!Common.SCUtility.isEmpty(vh.PreAssignMCSCommandID))
                            return CommandStatus.PreAssign;
                        else if (cmd_ohtc.CMD_TPYE == E_CMD_TYPE.Round)
                            return CommandStatus.CycleMove;
                        else
                            return CommandStatus.OHTC;
                    }
                    else
                    {
                        return CommandStatus.MCS;
                    }
                }
                else
                {
                    return CommandStatus.NoCommand;
                }
            }
        }


        VehicleState vehicleState
        {
            get
            {
                if (vh.IS_INSTALLED)
                    return VehicleState.Install;
                else
                    return VehicleState.Remove;
            }
        }
        RepairStatus repairStatus
        {
            get
            {
                if (vh.IS_INSTALLED)
                    return RepairStatus.None;
                else
                    return RepairStatus.Matain;
            }
        }

        //Error Status
        VhErrorStatus errorStatus
        {
            get
            {
                switch (vh.errorState)
                {
                    case AVEHICLE.VehicleErrorState.AlarmConfirm:
                        return VhErrorStatus.WaittingAlarmConfirm;
                    case AVEHICLE.VehicleErrorState.AlarmHappending:
                        return VhErrorStatus.AlarmSet;
                    case AVEHICLE.VehicleErrorState.NoAlarm:
                        return VhErrorStatus.NoAlarm;
                    default:
                        return VhErrorStatus.NoAlarm;
                }

            }
        }

        ChargeStatus chargeStatus
        {
            get
            {
                return ChargeStatus.NoCharge;

            }
        }
        bool IsLongCharging
        {
            get
            {
                return false;
            }
        }
        bool IsCSTInstall
        {
            get
            {
                if (vh.HAS_CST == 1)
                {
                    return true;
                }
                else
                    return false;
            }
        }
        bool IsObstacleStop
        {
            get
            {
                if (vh.IsObstacle)
                {
                    return true;
                }
                else
                    return false;
            }
        }
        bool IsReserveStop
        {
            get
            {
                if (vh.IsBlocking)
                {
                    return true;
                }
                else
                    return false;
            }
        }
        OpreationsTime opreationsTime
        {
            get
            {
                switch (vehicleState)
                {
                    case VehicleState.Install:
                        if (IsConnected &&
                            errorStatus == VhErrorStatus.NoAlarm &&
                            vehicleStatus == VehicleStatus.Auto)
                        {
                            if (IsIdle)
                            {
                                return OpreationsTime.Idle;
                            }
                            else
                            {
                                return OpreationsTime.Run;
                            }
                        }
                        else
                        {
                            return OpreationsTime.UnscheduleDowntime;
                        }
                    case VehicleState.Remove:
                        return OpreationsTime.UnscheduleDowntime;
                    default:
                        return OpreationsTime.UnscheduleDowntime;
                }
            }
        }

        private bool IsIdle
        {
            get
            {
                if (IsConnected &&
                     errorStatus == VhErrorStatus.NoAlarm &&
                   controlStatus == ControlStatus.Remote &&
                   vehicleStatus == VehicleStatus.Auto &&
                   commandStatus == CommandStatus.NoCommand)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }



        StringBuilder sb = new StringBuilder();
        /// <summary>
        //<column name="Time" layout="${message}" />
        //<column name="VehicleID" layout="" />
        //<column name="IsConnected" layout="" />
        //<column name="controlStatus" layout="" />
        //<column name="vehicleStatus" layout="" />
        //<column name="commandStatus" layout="" />
        //<column name="vehicleState" layout="" />
        //<column name="repairStatus" layout="" />
        //<column name="errorStatus" layout="" />
        //<column name="chargeStatus" layout="" />
        //<column name="IsLongCharging" layout="" />
        //<column name="IsCSTInstall" layout="" />
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            sb.Clear();
            sb.Append(DateTime.Now.ToString(App.SCAppConstants.DateTimeFormat_19)).Append(",");
            sb.Append(vh.VEHICLE_ID).Append(",");
            sb.Append(IsConnected).Append(",");
            sb.Append(controlStatus).Append(",");
            sb.Append(vehicleStatus).Append(",");
            sb.Append(commandStatus).Append(",");
            sb.Append(vehicleState).Append(",");
            sb.Append(repairStatus).Append(",");
            sb.Append(errorStatus).Append(",");
            sb.Append(chargeStatus).Append(",");
            sb.Append(IsLongCharging).Append(",");
            sb.Append(IsCSTInstall).Append(",");
            sb.Append(opreationsTime).Append(",");
            sb.Append(IsObstacleStop).Append(",");
            sb.Append(IsReserveStop);
            string record_message = sb.ToString();
            return record_message;
        }

    }
}
