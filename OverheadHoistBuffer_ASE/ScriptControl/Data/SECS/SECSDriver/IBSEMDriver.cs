using com.mirle.ibg3k0.stc.Common;
using System.Collections.Generic;

namespace com.mirle.ibg3k0.sc.Data.SECSDriver
{
    public abstract class IBSEMDriver : SEMDriver
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();


        #region Receive
        protected abstract void S2F49ReceiveEnhancedRemoteCommandExtension(object sender, SECSEventArgs e);
        protected abstract void S2F41ReceiveHostCommand(object sender, SECSEventArgs e);
        #endregion Receive

        #region Send
        #region Transfer Event
        public abstract bool S6F11SendTransferAbortCompleted(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendTransferAbortFailed(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendTransferAbortInitiated(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendTransferCancelCompleted(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null);       
        public abstract bool S6F11SendTransferCancelFailed(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendTransferCancelInitial(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendTransferCompleted(ACMD_MCS cmd, CassetteData cassette, string result_code, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendTransferInitiated(string cmd_id,  List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendTransferPaused(string cmd_id,  List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendTransferResume(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendCarrierTransferring(ACMD_MCS cmd, CassetteData cassette, string ohtName, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendCarrierRemovedCompleted(string cst_id, string box_id, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendCarrierInstallCompleted(CassetteData cst_id, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendCarrierRemovedFromPort(CassetteData cst, string Handoff_Type, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendCarrierResumed(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendCarrierStored(CassetteData cst, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendCarrierStoredAlt(ACMD_MCS cmd, CassetteData cassette, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendCarrierWaitIn(CassetteData cst, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendCarrierWaitOut(CassetteData cst, string portType, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendUnitAlarmSet(string unitID, string alarmID, string alarmTest, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendUnitAlarmCleared(string unitID, string alarmID, string alarmTest, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendCraneActive(string cmdID, string craneID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendCraneIdle(string craneID, string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendCraneInEscape(string craneID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendCraneOutEscape(string craneID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendCraneOutServce(string craneID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendCraneInServce(string craneID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendCarrierIDRead(CassetteData cst, string IDreadStatus, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendZoneCapacityChange(string loc, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendOperatorInitiatedAction(string cmd_id, string cmd_type, List<AMCSREPORTQUEUE> reportQueues = null);

        public abstract bool S6F11SendPortOutOfService(string port_id, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendPortInService(string port_id, List<AMCSREPORTQUEUE> reportQueues = null);

        public abstract bool S6F11SendLoadReq(string port_id, List<AMCSREPORTQUEUE> reportQueues = null);


        public abstract bool S6F11SendUnLoadReq(string port_id, List<AMCSREPORTQUEUE> reportQueues = null);

        public abstract bool S6F11SendNoReq(string port_id, List<AMCSREPORTQUEUE> reportQueues = null);


        public abstract bool S6F11SendPortTypeInput(string port_id, List<AMCSREPORTQUEUE> reportQueues = null);


        public abstract bool S6F11SendPortTypeOutput(string port_id, List<AMCSREPORTQUEUE> reportQueues = null);


        public abstract bool S6F11SendPortTypeChanging(string port_id, List<AMCSREPORTQUEUE> reportQueues = null);


        public abstract bool S6F11SendCarrierBoxIDRename(string cstID, string boxID, string cstLOC, List<AMCSREPORTQUEUE> reportQueues = null);


        public abstract bool S6F11SendEmptyBoxSupply(string ReqCount, string zoneName, List<AMCSREPORTQUEUE> reportQueues = null);


        public abstract bool S6F11SendEmptyBoxRecycling(string boxID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendShelfStatusChange(ZoneDef zone, List<AMCSREPORTQUEUE> reportQueues = null);

        public abstract bool S6F11SendQueryLotID(string cstID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11SendClearBoxMoveReq(string boxID, string portID, List<AMCSREPORTQUEUE> reportQueues = null);

        //public abstract bool S6F11SendTransferring(string vhID, List<AMCSREPORTQUEUE> reportQueues = null);
        //public abstract bool S6F11SendVehicleArrived(string vhID, List<AMCSREPORTQUEUE> reportQueues = null);
        //public abstract bool S6F11SendVehicleAcquireStarted(string vhID, List<AMCSREPORTQUEUE> reportQueues = null);
        //public abstract bool S6F11SendVehicleAcquireCompleted(string vhID, List<AMCSREPORTQUEUE> reportQueues = null);
        //public abstract bool S6F11SendVehicleAssigned(string vhID, List<AMCSREPORTQUEUE> reportQueues = null);
        //public abstract bool S6F11SendVehicleDeparted(string vhID, List<AMCSREPORTQUEUE> reportQueues = null);
        //public abstract bool S6F11SendVehicleDepositStarted(string vhID, List<AMCSREPORTQUEUE> reportQueues = null);
        //public abstract bool S6F11SendVehicleDepositCompleted(string vhID, List<AMCSREPORTQUEUE> reportQueues = null);
        //public abstract bool S6F11SendCarrierInstalled(string vhID, List<AMCSREPORTQUEUE> reportQueues = null);
        //public abstract bool S6F11SendCarrierInstalled(string vhID, string carrierID, string transferPort, List<AMCSREPORTQUEUE> reportQueues = null);
        //public abstract bool S6F11SendCarrierRemoved(string vhID, List<AMCSREPORTQUEUE> reportQueues = null);
        //public abstract bool S6F11SendCarrierRemoved(string vhID, string carrierID, string transferPort, List<AMCSREPORTQUEUE> reportQueues = null);
        //public abstract bool S6F11SendVehicleUnassinged(string vhID, List<AMCSREPORTQUEUE> reportQueues = null);


        //public abstract bool S6F11SendVehicleInstalled(string vhID, List<AMCSREPORTQUEUE> reportQueues = null);
        //public abstract bool S6F11SendVehicleRemoved(string vhID, List<AMCSREPORTQUEUE> reportQueues = null);
        #endregion Transfer Event
        #region Port Event

        #endregion Port Event

        #region TSC State Transition Event
        public abstract bool S6F11SendTSCAutoCompleted();
        public abstract bool S6F11SendTSCAutoInitiated();
        public abstract bool S6F11SendTSCPauseCompleted();
        public abstract bool S6F11SendTSCPaused();
        public abstract bool S6F11SenSCPauseInitiated();
        #endregion TSC State Transition Event
        public abstract bool S6F11SendAlarmCleared(ACMD_MCS CMD_MCS, ALARM ALARM, string unitid, string unitstate);
        public abstract bool S6F11SendAlarmSet(ACMD_MCS CMD_MCS, ALARM ALARM, string unitid, string unitstate, string RecoveryOption);
        #endregion Send

    }

    public class IBSEMDriverEmpty : IBSEMDriver
    {
        public override bool S6F11SendPortInService(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendLoadReq(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        public override bool S6F11SendUnLoadReq(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendNoReq(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        public override bool S6F11SendPortTypeInput(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        public override bool S6F11SendPortTypeOutput(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        public override bool S6F11SendPortTypeChanging(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        public override bool S6F11SendCarrierBoxIDRename(string cstID, string boxID, string cstLOC, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        public override bool S6F11SendEmptyBoxSupply(string ReqCount, string zoneName, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        public override bool S6F11SendEmptyBoxRecycling(string boxID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        public override bool S6F11SendQueryLotID(string cstID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendClearBoxMoveReq(string boxID, string portID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendCarrierTransferring(ACMD_MCS cmd, CassetteData cassette, string ohtName, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendAlarmCleared(ACMD_MCS CMD_MCS, ALARM ALARM, string unitid, string unitstate)
        {
            return true;
        }
        public override bool S6F11SendAlarmSet(ACMD_MCS CMD_MCS, ALARM ALARM, string unitid, string unitstate, string RecoveryOption)
        {
            return true;
        }
        
        public override AMCSREPORTQUEUE S6F11BulibMessage(string ceid, object Vids)
        {
            return null;
        }

        //public override bool S6F11SendCarrierInstalled(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendCarrierInstalled(string vhID, string carrierID, string transferPort, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendCarrierRemoved(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendCarrierRemoved(string vhID, string carrierID, string transferPort, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        public override bool S6F11SendControlStateLocal()
        {
            return true;

        }

        public override bool S6F11SendControlStateRemote()
        {
            return true;

        }

        public override bool S6F11SendEquiptmentOffLine()
        {
            return true;

        }

        public override bool S6F11SendTransferCompleted(ACMD_MCS cmd, CassetteData cassette, string result_code, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendTransferInitiated(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendTransferPaused(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendTransferResume(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendMessage(AMCSREPORTQUEUE queue)
        {
            return true;

        }

        public override bool S6F11SendCarrierRemovedCompleted(string cstid, string boxid, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCarrierInstallCompleted(CassetteData cst, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCarrierRemovedFromPort(CassetteData cst, string Handoff_Type, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCarrierResumed(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCarrierStored(CassetteData cst, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCarrierStoredAlt(ACMD_MCS cmd, CassetteData cassette, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCarrierWaitIn(CassetteData cst, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCarrierWaitOut(CassetteData cst, string portType, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendUnitAlarmSet(string unitID, string alarmID, string alarmTest, List<AMCSREPORTQUEUE> reportQueues = null)

        {
            return true;

        }
        public override bool S6F11SendUnitAlarmCleared(string unitID, string alarmID, string alarmTest, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCraneActive(string cmdID, string craneID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCraneIdle(string craneID, string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCraneInEscape(string craneID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCraneOutEscape(string craneID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCraneOutServce(string craneID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCraneInServce(string craneID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendCarrierIDRead(CassetteData cst, string IDreadStatus, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendZoneCapacityChange(string loc, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendOperatorInitiatedAction(string cmd_id, string cmd_type, List<AMCSREPORTQUEUE> reportQueues = null)

        {
            return true;

        }

        public override bool S6F11SendPortOutOfService(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;

        }
        public override bool S6F11SendShelfStatusChange(ZoneDef zone, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        //public override bool S6F11SendTransferAbortCompleted(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendTransferAbortFailed(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendTransferAbortInitiated(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendTransferCancelCompleted(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendTransferCancelInitial(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}
        public override bool S6F11SendTransferAbortCompleted(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendTransferAbortFailed(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendTransferAbortInitiated(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendTransferCancelCompleted(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendTransferCancelFailed(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }
        public override bool S6F11SendTransferCancelInitial(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        //public override bool S6F11SendTransferring(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        public override bool S6F11SendTSCAutoCompleted()
        {
            return true;
        }

        public override bool S6F11SendTSCAutoInitiated()
        {
            return true;
        }

        public override bool S6F11SendTSCPauseCompleted()
        {
            return true;
        }

        public override bool S6F11SendTSCPaused()
        {
            return true;
        }

        public override bool S6F11SenSCPauseInitiated()
        {
            return true;
        }

        //public override bool S6F11SendVehicleAcquireCompleted(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendVehicleAcquireStarted(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendVehicleArrived(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendVehicleAssigned(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendVehicleDeparted(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendVehicleDepositCompleted(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendVehicleDepositStarted(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendVehicleInstalled(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendVehicleRemoved(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}

        //public override bool S6F11SendVehicleUnassinged(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        //{
        //    return true;
        //}




        protected override void S2F41ReceiveHostCommand(object sender, SECSEventArgs e)
        {
        }

        protected override void S2F49ReceiveEnhancedRemoteCommandExtension(object sender, SECSEventArgs e)
        {
        }
    }
}