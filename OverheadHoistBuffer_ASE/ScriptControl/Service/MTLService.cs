using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.Data.VO.Interface;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Service
{
    public class MTLService
    {
        //public readonly string MTL_CAR_OUT_BUFFER_ADDRESS = "20292";
        //public readonly string MTL_CAR_IN_BUFFER_ADDRESS = "24294";
        //public readonly string MTL_ADDRESS = "20293";
        //public readonly string MTL_SYSTEM_IN_ADDRESS = "20198";
        const ushort CAR_ACTION_MODE_NO_ACTION = 0;
        const ushort CAR_ACTION_MODE_ACTION = 1;
        const ushort CAR_ACTION_MODE_ACTION_FOR_MCS_COMMAND = 2;
        VehicleService VehicleService = null;
        VehicleBLL vehicleBLL = null;
        ReportBLL reportBLL = null;
        private SCApplication scApp = null;
        //MaintainLift mtx = null;
        //string carOutVhID = "";
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public MTLService()
        {
        }
        public void start(SCApplication app)
        {
            scApp = app;
            List<AEQPT> eqpts = app.getEQObjCacheManager().getAllEquipment();

            foreach (var eqpt in eqpts)
            {
                if (eqpt is IMaintainDevice)
                {
                    IMaintainDevice maintainDevice = eqpt as IMaintainDevice;
                    if (maintainDevice is MaintainSpace)
                    {
                        MaintainSpace maintainSpace = eqpt as MaintainSpace;
                        maintainSpace.addEventHandler(nameof(MTLService), nameof(maintainSpace.Plc_Link_Stat), PublishMTSInfo);
                        maintainSpace.addEventHandler(nameof(MTLService), nameof(maintainSpace.Is_Eq_Alive), PublishMTSInfo);
                        maintainSpace.addEventHandler(nameof(MTLService), nameof(maintainSpace.MTxMode), PublishMTSInfo);
                        maintainSpace.addEventHandler(nameof(MTLService), nameof(maintainSpace.Interlock), PublishMTSInfo);
                        maintainSpace.addEventHandler(nameof(MTLService), nameof(maintainSpace.CurrentCarID), PublishMTSInfo);
                        maintainSpace.addEventHandler(nameof(MTLService), nameof(maintainSpace.CurrentPreCarOurDistance), PublishMTSInfo);
                        maintainSpace.addEventHandler(nameof(MTLService), nameof(maintainSpace.SynchronizeTime), PublishMTSInfo);
                    }
                    else if (maintainDevice is MaintainLift)
                    {
                        MaintainLift maintainLift = eqpt as MaintainLift;
                        maintainLift.addEventHandler(nameof(MTLService), nameof(maintainLift.Plc_Link_Stat), PublishMTLInfo);
                        maintainLift.addEventHandler(nameof(MTLService), nameof(maintainLift.Is_Eq_Alive), PublishMTLInfo);
                        maintainLift.addEventHandler(nameof(MTLService), nameof(maintainLift.MTxMode), PublishMTLInfo);
                        maintainLift.addEventHandler(nameof(MTLService), nameof(maintainLift.Interlock), PublishMTLInfo);
                        maintainLift.addEventHandler(nameof(MTLService), nameof(maintainLift.CurrentCarID), PublishMTLInfo);
                        maintainLift.addEventHandler(nameof(MTLService), nameof(maintainLift.MTLLocation), PublishMTLInfo);
                        maintainLift.addEventHandler(nameof(MTLService), nameof(maintainLift.CurrentPreCarOurDistance), PublishMTLInfo);
                        maintainLift.addEventHandler(nameof(MTLService), nameof(maintainLift.SynchronizeTime), PublishMTLInfo);
                    }
                }
            }

            VehicleService = app.VehicleService;
            vehicleBLL = app.VehicleBLL;
            reportBLL = app.ReportBLL;
            //  mtl = app.getEQObjCacheManager().getEquipmentByEQPTID("MTL") as MaintainLift;
        }


        //bool cancelCarOutRequest = false;
        //bool carOurSuccess = false;

        /// <summary>
        /// 處理人員由MTL執行CAR OUT時的流程
        /// </summary>
        /// <param name="vhNum"></param>
        /// <returns></returns>
        public (bool isSuccess, string result) carOutRequset(IMaintainDevice mtx, int vhNum)
        {
            AVEHICLE pre_car_out_vh = vehicleBLL.cache.getVhByNum(vhNum);
            if (pre_car_out_vh == null)
                return (false, $"vh num:{vhNum}, not exist.");
            else
            {
                bool isSuccess = true;
                string result = "";
                var check_result = checkVhAndMTxCarOutStatus(mtx, null, pre_car_out_vh);
                isSuccess = check_result.isSuccess;
                result = check_result.result;
                if (isSuccess)
                {
                    (bool isSuccess, string result) process_result = default((bool isSuccess, string result));
                    if (mtx is MaintainLift)
                    {
                        process_result = processCarOutScenario(mtx as MaintainLift, pre_car_out_vh);
                    }
                    else if (mtx is MaintainSpace)
                    {
                        process_result = processCarOutScenario(mtx as MaintainSpace, pre_car_out_vh);
                    }
                    else
                    {
                        return process_result;
                    }
                    isSuccess = process_result.isSuccess;
                    result = process_result.result;
                }
                return (isSuccess, result);
            }
        }
        /// <summary>
        /// 處理人員由OHTC執行CAR OUT時的流程
        /// </summary>
        /// <param name="vhID"></param>
        /// <returns></returns>
        public (bool isSuccess, string result) carOutRequset(IMaintainDevice mtx, string vhID)
        {
            AVEHICLE pre_car_out_vh = vehicleBLL.cache.getVhByID(vhID);
            bool isSuccess = true;
            string result = "";
            var check_result = checkVhAndMTxCarOutStatus(mtx, null, pre_car_out_vh);
            isSuccess = check_result.isSuccess;
            result = check_result.result;
            //2.向MTL發出Car out request
            //成功後開始向MTL發送該台Vh的當前狀態，並在裡面判斷是否有收到Cancel的命令，有的話要將資料清空
            //Trun on 給MTL的Interlock flag
            //將該台Vh變更為AutoToMtl
            if (isSuccess)
            {
                var send_result = mtx.carOutRequest((UInt16)pre_car_out_vh.Num);
                if (send_result.isSendSuccess && send_result.returnCode == 1)
                {
                    (bool isSuccess, string result) process_result = default((bool isSuccess, string result));
                    if (mtx is MaintainLift)
                    {
                        process_result = processCarOutScenario(mtx as MaintainLift, pre_car_out_vh);
                    }
                    else if (mtx is MaintainSpace)
                    {
                        process_result = processCarOutScenario(mtx as MaintainSpace, pre_car_out_vh);
                    }
                    else
                    {
                        return process_result;
                    }
                    isSuccess = process_result.isSuccess;
                    result = process_result.result;
                }
                else
                {
                    isSuccess = false;
                    result = $"Request car fail,Send result:{send_result.isSendSuccess}, return code:{send_result.returnCode}";
                }
            }
            return (isSuccess, result);
        }

        public (bool isSuccess, string result) checkVhAndMTxCarOutStatus(IMaintainDevice mtx, IMaintainDevice dockingMtx, AVEHICLE car_out_vh)
        {
            bool isSuccess = true;
            string result = "";

            //1.要判斷目前車子的狀態
            if (isSuccess && car_out_vh == null)
            {
                isSuccess = false;
                result = $"vh not exist.";
            }
            string vh_id = car_out_vh.VEHICLE_ID;
            if (isSuccess && !car_out_vh.isTcpIpConnect)
            {
                isSuccess = false;
                result = $"vh id:{vh_id}, not connection.";
            }
            if (isSuccess && car_out_vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.Manual)
            {
                isSuccess = false;
                result = $"Vehicle:{vh_id}, current mode is:{car_out_vh.MODE_STATUS}, can't excute auto car out";
            }
            if (isSuccess && SCUtility.isEmpty(car_out_vh.CUR_SEC_ID))
            {
                isSuccess = false;
                result = $"vh id:{vh_id}, current section is empty.";
            }

            //2.要判斷MTL的 Safety check是否有On且是否為Auto Mode
            if (isSuccess && !SCUtility.isEmpty(mtx.PreCarOutVhID))
            {
                isSuccess = false;
                result = $"MTL:{mtx.DeviceID} Current process car our vh:{mtx.PreCarOutVhID}, can't excute cat out again.";
            }

            if (isSuccess && !mtx.IsAlive)
            {
                isSuccess = false;
                result = $"MTL:{mtx.DeviceID} Current Alive:{mtx.IsAlive}, can't excute cat out requset.";
            }
            if (isSuccess && mtx.MTxMode == ProtocolFormat.OHTMessage.MTxMode.Manual)
            {
                isSuccess = false;
                result = $"MTL:{mtx.DeviceID} Current Mode:{mtx.MTxMode}, can't excute cat out requset.";
            }
            if (isSuccess && !mtx.CarOutSafetyCheck)
            {
                isSuccess = false;
                result = $"MTx:{mtx.DeviceID} CarOutSafetyCheck:{mtx.CarOutSafetyCheck}, can't excute cat out requset.";
            }

            if (dockingMtx != null)
            {
                if (isSuccess && dockingMtx.MTxMode == ProtocolFormat.OHTMessage.MTxMode.Manual)
                {
                    isSuccess = false;
                    result = $"Docking MTx:{dockingMtx.DeviceID} Current Mode:{dockingMtx.MTxMode}, can't excute cat out requset.";
                }
                if (!SCUtility.isMatche(car_out_vh.CUR_ADR_ID, dockingMtx.DeviceAddress))
                {
                    if (isSuccess && !dockingMtx.CarOutSafetyCheck)
                    {
                        isSuccess = false;
                        result = $"Docking MTx:{dockingMtx.DeviceID} CarOutSafetyCheck:{dockingMtx.CarOutSafetyCheck}, can't excute cat out requset.";
                    }
                }
            }

            return (isSuccess, result);
        }

        public (bool isSuccess, string result) CarOurRequest(IMaintainDevice mtx, AVEHICLE car_out_vh)
        {
            bool is_success = false;
            string result = "";
            var send_result = mtx.carOutRequest((UInt16)car_out_vh.Num);
            is_success = send_result.isSendSuccess && send_result.returnCode == 1;
            if (!is_success)
            {
                result = $"MTL:{mtx.DeviceID} reject car our request. return code:{send_result.returnCode}";
            }
            else
            {
                result = "OK";
            }
            return (is_success, result);
        }

        public (bool isSuccess, string result) processCarOutScenario(MaintainLift mtx, AVEHICLE preCarOutVh)
        {
            string pre_car_out_vh_id = preCarOutVh.VEHICLE_ID;
            string pre_car_out_vh_ohtc_cmd_id = preCarOutVh.OHTC_CMD;
            string pre_car_out_vh_cur_adr_id = preCarOutVh.CUR_ADR_ID;
            bool isSuccess;
            string result = "OK";
            mtx.CancelCarOutRequest = false;
            mtx.CarOurSuccess = false;
            mtx.SetCarOutInterlock(true);
            //isSuccess = VehicleService.doReservationVhToMaintainsBufferAddress(pre_car_out_vh_id, MTL_CAR_OUT_BUFFER_ADDRESS);
            isSuccess = VehicleService.doReservationVhToMaintainsBufferAddress(pre_car_out_vh_id, mtx.MTL_SYSTEM_OUT_ADDRESS);
            if (isSuccess && SCUtility.isEmpty(pre_car_out_vh_ohtc_cmd_id))
            {
                //在收到OHT的ID:132-命令結束後或者在變為AutoLocal後此時OHT沒有命令的話則會呼叫此Function來創建一個Transfer command，讓Vh移至移動至System out上
                if (SCUtility.isMatche(pre_car_out_vh_cur_adr_id, mtx.MTL_SYSTEM_OUT_ADDRESS))
                {
                    VehicleService.doAskVhToMaintainsAddress(pre_car_out_vh_id, mtx.MTL_ADDRESS);
                }
                else
                {
                    VehicleService.doAskVhToSystemOutAddress(pre_car_out_vh_id, mtx.MTL_SYSTEM_OUT_ADDRESS);
                }
            }
            if (isSuccess)
            {
                //carOutVhID = pre_car_out_vh_id;
                mtx.PreCarOutVhID = pre_car_out_vh_id;
                Task.Run(() => RegularUpdateRealTimeCarInfo(mtx, preCarOutVh));
            }
            else
            {
                mtx.SetCarOutInterlock(false);
                isSuccess = false;
                result = $"Reservation vh to mtl fail.";
            }

            return (isSuccess, result);
        }


        public (bool isSuccess, string result) processCarOutScenario(MaintainSpace mtx, AVEHICLE preCarOutVh)
        {
            string pre_car_out_vh_id = preCarOutVh.VEHICLE_ID;
            string pre_car_out_vh_ohtc_cmd_id = preCarOutVh.OHTC_CMD;
            string pre_car_out_vh_cur_adr_id = preCarOutVh.CUR_ADR_ID;
            bool isSuccess;
            string result = "";
            mtx.CancelCarOutRequest = false;
            mtx.CarOurSuccess = false;
            mtx.SetCarOutInterlock(true);
            isSuccess = VehicleService.doReservationVhToMaintainsSpace(pre_car_out_vh_id);
            if (isSuccess && SCUtility.isEmpty(pre_car_out_vh_ohtc_cmd_id))
            {
                //在收到OHT的ID:132-命令結束後或者在變為AutoLocal後此時OHT沒有命令的話則會呼叫此Function來創建一個Transfer command，讓Vh移至移動至System out上
                isSuccess = VehicleService.doAskVhToSystemOutAddress(pre_car_out_vh_id, mtx.MTS_ADDRESS);
            }
            if (isSuccess)
            {
                //carOutVhID = pre_car_out_vh_id;
                mtx.PreCarOutVhID = pre_car_out_vh_id;
                Task.Run(() => RegularUpdateRealTimeCarInfo(mtx, preCarOutVh));
            }
            else
            {
                mtx.SetCarOutInterlock(false);
                isSuccess = false;
                result = $"Reservation vh to mtl fail.";
            }

            return (isSuccess, result);
        }

        private void RegularUpdateRealTimeCarInfo(IMaintainDevice mtx, AVEHICLE carOurVh)
        {
            do
            {
                UInt16 car_id = (ushort)carOurVh.Num;
                UInt16 action_mode = 0;
                if (carOurVh.ACT_STATUS == ProtocolFormat.OHTMessage.VHActionStatus.Commanding)
                {
                    if (!SCUtility.isEmpty(carOurVh.MCS_CMD))
                    {
                        action_mode = CAR_ACTION_MODE_ACTION_FOR_MCS_COMMAND;
                    }
                    else
                    {
                        action_mode = CAR_ACTION_MODE_ACTION;
                    }
                }
                else
                {
                    action_mode = CAR_ACTION_MODE_NO_ACTION;
                }
                UInt16 cst_exist = (ushort)carOurVh.HAS_CST;
                UInt16 current_section_id = 0;
                UInt16.TryParse(carOurVh.CUR_SEC_ID, out current_section_id);
                UInt16 current_address_id = 0;
                UInt16.TryParse(carOurVh.CUR_ADR_ID, out current_address_id);
                UInt32 buffer_distance = 0;
                UInt16 speed = (ushort)carOurVh.Speed;

                mtx.CurrentPreCarOurID = car_id;
                mtx.CurrentPreCarOurActionMode = action_mode;
                mtx.CurrentPreCarOurCSTExist = cst_exist;
                mtx.CurrentPreCarOurSectionID = current_section_id;
                mtx.CurrentPreCarOurAddressID = current_address_id;
                mtx.CurrentPreCarOurDistance = buffer_distance;
                mtx.CurrentPreCarOurSpeed = speed;

                mtx.setCarRealTimeInfo(car_id, action_mode, cst_exist, current_section_id, current_address_id, buffer_distance, speed);

                //如果在移動過程中，MTx突然變成手動模式的話，則要將原本在移動的車子取消命令
                if (mtx.MTxMode == MTxMode.Manual ||
                    !mtx.CarOutSafetyCheck)
                {
                    carOutRequestCancle(mtx);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(MTLService), Device: "OHTC",
                             Data: $"Device:{mtx.DeviceID} mtx mode suddenly turned mode:{mtx.MTxMode} or car out safety check change:{mtx.CarOutSafetyCheck}, " +
                             $"so urgent cancel vh:{mtx.PreCarOutVhID} of command.",
                             XID: mtx.DeviceID);
                    break;
                }

                SpinWait.SpinUntil(() => false, 200);
            } while (!mtx.CancelCarOutRequest && !mtx.CarOurSuccess);

            //mtx.setCarRealTimeInfo(0, 0, 0, 0, 0, 0, 0);
        }

        public void carOutComplete(IMaintainDevice mtx)
        {
            mtx.CarOurSuccess = true;
            //carOutVhID = "";
            mtx.SetCarOutInterlock(false);
            mtx.PreCarOutVhID = "";

        }


        public void carOutRequestCancle(IMaintainDevice mtx)
        {
            //將原本的在等待Carout的Vh改回AutoRemote
            mtx.CancelCarOutRequest = true;
            //if (!SCUtility.isEmpty(carOutVhID))
            //{
            //    VehicleService.doRecoverModeStatusToAutoRemote(carOutVhID);
            //}
            //carOutVhID = "";
            if (!SCUtility.isEmpty(mtx.PreCarOutVhID))
            {
                VehicleService.doRecoverModeStatusToAutoRemote(mtx.PreCarOutVhID);
                AVEHICLE pre_car_out_vh = vehicleBLL.cache.getVhByID(mtx.PreCarOutVhID);
                if (!SCUtility.isEmpty(pre_car_out_vh?.OHTC_CMD))
                {
                    VehicleService.doAbortCommand
                        (pre_car_out_vh, pre_car_out_vh.OHTC_CMD, ProtocolFormat.OHTMessage.CMDCancelType.CmdCancel);
                }
            }
            mtx.SetCarOutInterlock(false);
            mtx.PreCarOutVhID = "";
        }


        public void carInRequest()
        {
            //回復MTL的CarIn Request確認目前是否可以進行CarIn
        }
        public void carInSafetyAndVehicleStatusCheck(MaintainLift mtl)
        {
            //在收到MTL的 Car in safety check後，就可以叫Vh移動至Car in 的buffer區(MTL Home)
            //不過要先判斷vh是否已經在Auto模式下如果是則先將它變成AutoLocal的模式
            if (!mtl.CarInSafetyCheck || mtl.MTxMode != ProtocolFormat.OHTMessage.MTxMode.Auto)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(MTLService), Device: "OHTC",
                         Data: $"Device:{mtl.DeviceID} car in safety check in on, but mts mode:{mtl.MTxMode}, can't excute car in.",
                         XID: mtl.DeviceID);
                return;
            }

            AVEHICLE car_in_vh = vehicleBLL.cache.getVhByAddressID(mtl.MTL_ADDRESS);
            if (car_in_vh != null && car_in_vh.isTcpIpConnect)
            {
                if (car_in_vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.Manual)
                {
                    VehicleService.ModeChangeRequest(car_in_vh.VEHICLE_ID, OperatingVHMode.OperatingAuto);
                    if (SpinWait.SpinUntil(() => car_in_vh.MODE_STATUS == VHModeStatus.AutoMtl, 10000))
                    {
                        mtl.SetCarInMoving(true);
                        VehicleService.doAskVhToCarInBufferAddress(car_in_vh.VEHICLE_ID, mtl.MTL_CAR_IN_BUFFER_ADDRESS);
                    }
                    else
                    {
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: "OHTC",
                                 Data: $"vh:{car_in_vh.VEHICLE_ID} request car in, but can't change to auto mode.",
                                 VehicleID: car_in_vh.VEHICLE_ID);
                    }
                }
                else if (car_in_vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoMtl)
                {
                    mtl.SetCarInMoving(true);
                    VehicleService.doAskVhToCarInBufferAddress(car_in_vh.VEHICLE_ID, mtl.MTL_CAR_IN_BUFFER_ADDRESS);
                }
            }
        }


        public (bool isSuccess, string result) checkVhAndMTxCarInStatus(IMaintainDevice mtx, IMaintainDevice dockingMtx, AVEHICLE car_in_vh)
        {
            bool isSuccess = true;
            string result = "";

            string vh_id = car_in_vh.VEHICLE_ID;
            //1.要判斷目前車子的狀態
            if (isSuccess && !car_in_vh.isTcpIpConnect)
            {
                isSuccess = false;
                result = $"vh id:{vh_id}, not connection.";
            }
            //if (isSuccess && car_in_vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.Manual)
            //{
            //    isSuccess = false;
            //    result = $"Vehicle:{vh_id}, current mode is:{car_in_vh.MODE_STATUS}, can't excute auto car out";
            //}
            if (isSuccess && SCUtility.isEmpty(car_in_vh.CUR_SEC_ID))
            {
                isSuccess = false;
                result = $"vh id:{vh_id}, current section is empty.";
            }
            if (isSuccess && !SCUtility.isMatche(car_in_vh.CUR_ADR_ID, mtx.DeviceAddress))
            {
                isSuccess = false;
                result = $"vh id:{vh_id}, current address:{car_in_vh.CUR_ADR_ID} not match mtx device address:{mtx.DeviceAddress}.";
            }

            //2.要判斷MTL的 Safety check是否有On且是否為Auto Mode
            if (isSuccess && mtx.MTxMode == ProtocolFormat.OHTMessage.MTxMode.Manual)
            {
                isSuccess = false;
                result = $"MTx:{mtx.DeviceID} Current Mode:{mtx.MTxMode}, can't excute cat out requset.";
            }

            //3.若有Docking的Device，則需要再判斷一次他的狀態
            if (dockingMtx != null)
            {
                if (isSuccess && dockingMtx.MTxMode == ProtocolFormat.OHTMessage.MTxMode.Manual)
                {
                    isSuccess = false;
                    result = $"Docking MTx:{dockingMtx.DeviceID} Current Mode:{dockingMtx.MTxMode}, can't excute cat out requset.";
                }
                if (isSuccess && !dockingMtx.CarOutSafetyCheck)
                {
                    isSuccess = false;
                    result = $"Docking MTx:{dockingMtx.DeviceID} CarInSafetyCheck:{dockingMtx.CarOutSafetyCheck}, can't excute cat in requset.";
                }
            }
            return (isSuccess, result);
        }

        public void processCarInScenario(MaintainSpace mts)
        {
            //在收到MTL的 Car in safety check後，就可以叫Vh移動至Car in 的buffer區(MTL Home)
            //不過要先判斷vh是否已經在Auto模式下如果是則先將它變成AutoLocal的模式
            if (!SpinWait.SpinUntil(() => mts.CarInSafetyCheck && mts.MTxMode == MTxMode.Auto, 10000))
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: "OHTC",
                         Data: $"mts:{mts.DeviceID}, status not ready  CarInSafetyCheck:{mts.CarInSafetyCheck},Mode:{mts.MTxMode} ,can't excute car in",
                         XID: mts.DeviceID);
                return;
            }
            AVEHICLE car_in_vh = vehicleBLL.cache.getVhByAddressID(mts.MTS_ADDRESS);
            if (car_in_vh != null && car_in_vh.isTcpIpConnect)
            {
                if (car_in_vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.Manual)
                {
                    VehicleService.ModeChangeRequest(car_in_vh.VEHICLE_ID, ProtocolFormat.OHTMessage.OperatingVHMode.OperatingAuto);

                    if (SpinWait.SpinUntil(() => car_in_vh.MODE_STATUS == VHModeStatus.AutoMts, 10000))
                    {
                        mts.SetCarInMoving(true);
                        VehicleService.doAskVhToSystemInAddress(car_in_vh.VEHICLE_ID, mts.MTS_SYSTEM_IN_ADDRESS);
                    }
                    else
                    {
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLService), Device: "OHTC",
                                 Data: $"Process car in scenario:{mts.DeviceID} fail. ask vh change to auto mode time out",
                                 VehicleID: car_in_vh.VEHICLE_ID);
                    }
                }
                else if (car_in_vh.MODE_STATUS == ProtocolFormat.OHTMessage.VHModeStatus.AutoMts)
                {
                    mts.SetCarInMoving(true);
                    VehicleService.doAskVhToSystemInAddress(car_in_vh.VEHICLE_ID, mts.MTS_SYSTEM_IN_ADDRESS);
                }
            }
        }

        public void carInComplete(IMaintainDevice mtx, string vhID)
        {
            VehicleService.doRecoverModeStatusToAutoRemote(vhID);
            mtx.SetCarInMoving(false);
            //List<AMCSREPORTQUEUE> reportqueues = new List<AMCSREPORTQUEUE>();
            //reportBLL.newReportVehicleInstalled(vhID, reportqueues);
        }

        public void PublishMTLMTSInfo(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                AEQPT eqpt = sender as AEQPT;
                if (sender == null) return;
                byte[] line_serialize = BLL.LineBLL.Convert2GPB_MTLMTSInfo(eqpt);
                scApp.getNatsManager().PublishAsync
                    (SCAppConstants.NATS_SUBJECT_MTLMTS, line_serialize);
                //TODO 要改用GPP傳送
                //var line_Serialize = ZeroFormatter.ZeroFormatterSerializer.Serialize(line);
                //scApp.getNatsManager().PublishAsync
                //    (string.Format(SCAppConstants.NATS_SUBJECT_LINE_INFO), line_Serialize);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        public void PublishMTSInfo(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                MaintainSpace eqpt = sender as MaintainSpace;
                if (sender == null) return;
                byte[] line_serialize = BLL.LineBLL.Convert2GPB_MTLMTSInfo(eqpt);
                scApp.getNatsManager().PublishAsync
                    (SCAppConstants.NATS_SUBJECT_MTLMTS, line_serialize);
                //TODO 要改用GPP傳送
                //var line_Serialize = ZeroFormatter.ZeroFormatterSerializer.Serialize(line);
                //scApp.getNatsManager().PublishAsync
                //    (string.Format(SCAppConstants.NATS_SUBJECT_LINE_INFO), line_Serialize);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        public void PublishMTLInfo(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                MaintainLift eqpt = sender as MaintainLift;
                if (sender == null) return;
                byte[] line_serialize = BLL.LineBLL.Convert2GPB_MTLMTSInfo(eqpt);
                scApp.getNatsManager().PublishAsync
                    (SCAppConstants.NATS_SUBJECT_MTLMTS, line_serialize);
                //TODO 要改用GPP傳送
                //var line_Serialize = ZeroFormatter.ZeroFormatterSerializer.Serialize(line);
                //scApp.getNatsManager().PublishAsync
                //    (string.Format(SCAppConstants.NATS_SUBJECT_LINE_INFO), line_Serialize);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

    }
}
