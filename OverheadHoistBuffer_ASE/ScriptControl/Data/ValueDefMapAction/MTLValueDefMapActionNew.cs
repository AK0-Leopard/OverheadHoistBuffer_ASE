//*********************************************************************************
//      MTLValueDefMapAction.cs
//*********************************************************************************
// File Name: MTLValueDefMapAction.cs
// Description: 
//
//(c) Copyright 2018, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Common.MPLC;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Data.VO;
using KingAOP;
using NLog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

namespace com.mirle.ibg3k0.sc.Data.ValueDefMapAction
{
    public class MTLValueDefMapActionNew : MTxValueDefMapActionBase
    {
        Logger logger = NLog.LogManager.GetCurrentClassLogger();
        MaintainLift MTL { get { return this.eqpt as MaintainLift; } }
        public MTLValueDefMapActionNew()
            : base()
        {
        }

        object dateTimeSyneObj = new object();
        uint dateTimeIndex = 0;
        public override void DateTimeSyncCommand(DateTime dateTime)
        {

            OHxCToMtl_DateTimeSync send_function =
               scApp.getFunBaseObj<OHxCToMtl_DateTimeSync>(MTL.EQPT_ID) as OHxCToMtl_DateTimeSync;
            try
            {
                lock (dateTimeSyneObj)
                {
                    //1.準備要發送的資料
                    send_function.Year = Convert.ToUInt16(dateTime.Year.ToString(), 10);
                    send_function.Month = Convert.ToUInt16(dateTime.Month.ToString(), 10);
                    send_function.Day = Convert.ToUInt16(dateTime.Day.ToString(), 10);
                    send_function.Hour = Convert.ToUInt16(dateTime.Hour.ToString(), 10);
                    send_function.Min = Convert.ToUInt16(dateTime.Minute.ToString(), 10);
                    send_function.Sec = Convert.ToUInt16(dateTime.Second.ToString(), 10);
                    if (dateTimeIndex >= 9999)
                        dateTimeIndex = 0;
                    send_function.Index = ++dateTimeIndex;
                    //2.紀錄發送資料的Log
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLValueDefMapActionNew), Device: DEVICE_NAME_MTL,
                             Data: send_function.ToString(),
                             VehicleID: MTL.EQPT_ID);
                    //3.發送訊息
                    send_function.Write(bcfApp, MTL.EqptObjectCate, MTL.EQPT_ID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<OHxCToMtl_DateTimeSync>(send_function);
            }
        }
        uint message_index = 0;
        public override void OHxCMessageDownload(string msg)
        {
            OHxCToMtl_MessageDownload send_function =
                scApp.getFunBaseObj<OHxCToMtl_MessageDownload>(MTL.EQPT_ID) as OHxCToMtl_MessageDownload;
            try
            {
                //1.建立各個Function物件
                send_function.Message = msg;
                if (message_index > 9999)
                { message_index = 0; }
                send_function.Index = ++message_index;
                //2.紀錄發送資料的Log
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLValueDefMapActionNew), Device: DEVICE_NAME_MTL,
                         Data: send_function.ToString(),
                         VehicleID: MTL.EQPT_ID);
                //3.發送訊息
                send_function.Write(bcfApp, MTL.EqptObjectCate, MTL.EQPT_ID);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<OHxCToMtl_MessageDownload>(send_function);
            }
        }
        UInt16 car_realtime_info = 0;
        public override void CarRealtimeInfo(UInt16 car_id, UInt16 action_mode, UInt16 cst_exist, UInt16 current_section_id, UInt32 current_address_id,
                                            UInt32 buffer_distance, UInt16 speed)
        {
            OHxCToMtl_CarRealtimeInfo send_function =
                scApp.getFunBaseObj<OHxCToMtl_CarRealtimeInfo>(MTL.EQPT_ID) as OHxCToMtl_CarRealtimeInfo;
            try
            {
                //1.建立各個Function物件
                send_function.CarID = car_id;
                send_function.ActionMode = action_mode;
                send_function.CSTExist = cst_exist;
                send_function.CurrentSectionID = current_section_id;
                send_function.CurrentAddressID = current_address_id;
                send_function.BufferDistance = buffer_distance;
                send_function.Speed = speed;
                if (car_realtime_info > 9999)
                { car_realtime_info = 0; }
                send_function.Index = ++car_realtime_info;
                //2.紀錄發送資料的Log
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLValueDefMapActionNew), Device: DEVICE_NAME_MTL,
                         Data: send_function.ToString(),
                         VehicleID: MTL.EQPT_ID);
                //3.發送訊息
                send_function.Write(bcfApp, MTL.EqptObjectCate, MTL.EQPT_ID);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<OHxCToMtl_MessageDownload>(send_function);
            }
        }

        public override (bool isSendSuccess, UInt16 returnCode) OHxC_CarOutNotify(UInt16 car_id)
        {
            bool isSendSuccess = false;
            var send_function =
                scApp.getFunBaseObj<OHxCToMtl_CarOutNotify>(MTL.EQPT_ID) as OHxCToMtl_CarOutNotify;
            var receive_function =
                scApp.getFunBaseObj<MtlToOHxC_ReplyCarOutNotify>(MTL.EQPT_ID) as MtlToOHxC_ReplyCarOutNotify;
            try
            {
                //1.準備要發送的資料
                send_function.CarID = car_id;
                ValueRead vr_reply = receive_function.getValueReadHandshake
                    (bcfApp, MTL.EqptObjectCate, MTL.EQPT_ID);
                //2.紀錄發送資料的Log
                send_function.Handshake = 1;
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLValueDefMapActionNew), Device: DEVICE_NAME_MTL,
                         Data: send_function.ToString(),
                         VehicleID: MTL.EQPT_ID);
                //3.等待回復
                TrxMPLC.ReturnCode returnCode =
                    send_function.SendRecv(bcfApp, MTL.EqptObjectCate, MTL.EQPT_ID, vr_reply);
                //4.取得回復的結果
                if (returnCode == TrxMPLC.ReturnCode.Normal)
                {
                    receive_function.Read(bcfApp, MTL.EqptObjectCate, MTL.EQPT_ID);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLValueDefMapActionNew), Device: DEVICE_NAME_MTL,
                             Data: receive_function.ToString(),
                             VehicleID: MTL.EQPT_ID);
                    isSendSuccess = true;
                }
                send_function.Handshake = 0;
                send_function.resetHandshake(bcfApp, MTL.EqptObjectCate, MTL.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLValueDefMapActionNew), Device: DEVICE_NAME_MTL,
                         Data: send_function.ToString(),
                         VehicleID: MTL.EQPT_ID);
                return (isSendSuccess, receive_function.ReturnCode);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<OHxCToMtl_CarOutNotify>(send_function);
                scApp.putFunBaseObj<MtlToOHxC_ReplyCarOutNotify>(receive_function);
            }
            return (isSendSuccess, 0);
        }
        public override void OHxC2MTL_CarOutInterface(bool carOutInterlock, bool carOutReady, bool carMoving, bool carMoveComplete)
        {
            try
            {
                ValueWrite vm_carOutInterlock = bcfApp.getWriteValueEvent(MTL.EqptObjectCate, MTL.EQPT_ID, "OHXC_TO_MTL_U2D_CAR_OUT_INTERLOCK");
                ValueWrite vm_carOutReady = bcfApp.getWriteValueEvent(MTL.EqptObjectCate, MTL.EQPT_ID, "OHXC_TO_MTL_U2D_CAR_OUT_READY");
                ValueWrite vm_carMoving = bcfApp.getWriteValueEvent(MTL.EqptObjectCate, MTL.EQPT_ID, "OHXC_TO_MTL_U2D_CAR_MOVING");
                ValueWrite vm_carMoveCmp = bcfApp.getWriteValueEvent(MTL.EqptObjectCate, MTL.EQPT_ID, "OHXC_TO_MTL_U2D_CAR_MOVE_COMPLETE");
                string setValue = carOutInterlock ? "1" : "0";
                vm_carOutInterlock.setWriteValue(carOutInterlock ? "1" : "0");
                vm_carOutReady.setWriteValue(carOutReady ? "1" : "0");
                vm_carMoving.setWriteValue(carMoving ? "1" : "0");
                vm_carMoveCmp.setWriteValue(carMoveComplete ? "1" : "0");
                bool result =ISMControl.writeDeviceBlock(bcfApp, vm_carOutInterlock);
                ISMControl.writeDeviceBlock(bcfApp, vm_carOutReady);
                ISMControl.writeDeviceBlock(bcfApp, vm_carMoving);
                ISMControl.writeDeviceBlock(bcfApp, vm_carMoveCmp);
                if (result) eqpt.Interlock = setValue == "1" ? true : false;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }
        public override bool setOHxC2MTL_CarOutInterlock(bool carOutInterlock)
        {
            try
            {
                ValueWrite vm_carOutInterlock = bcfApp.getWriteValueEvent(MTL.EqptObjectCate, MTL.EQPT_ID, "OHXC_TO_MTL_U2D_CAR_OUT_INTERLOCK");
                string setValue = carOutInterlock ? "1" : "0";
                vm_carOutInterlock.setWriteValue(setValue);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLValueDefMapActionNew), Device: DEVICE_NAME_MTL,
                         Data: $"setOHxC2MTL_CarOutInterlock:{carOutInterlock}",
                         VehicleID: MTL.EQPT_ID);
                bool result = ISMControl.writeDeviceBlock(bcfApp, vm_carOutInterlock);
                if (result) eqpt.Interlock = setValue == "1" ? true : false;
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
        }
        public override void OHxC2MTL_CarInInterface(bool carMoving, bool carMoveComplete)
        {
            try
            {
                ValueWrite vm_carMoving = bcfApp.getWriteValueEvent(MTL.EqptObjectCate, MTL.EQPT_ID, "OHXC_TO_MTL_D2U_CAR_MOVING");
                ValueWrite vm_carMoveCmp = bcfApp.getWriteValueEvent(MTL.EqptObjectCate, MTL.EQPT_ID, "OHXC_TO_MTL_D2U_CAR_MOVE_COMPLETE");
                vm_carMoving.setWriteValue(carMoving ? "1" : "0");
                vm_carMoveCmp.setWriteValue(carMoveComplete ? "1" : "0");
                ISMControl.writeDeviceBlock(bcfApp, vm_carMoving);
                ISMControl.writeDeviceBlock(bcfApp, vm_carMoveCmp);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }
        public override bool setOHxC2MTL_CarInMoving(bool carMoving)
        {
            try
            {
                ValueWrite vm_carMoving = bcfApp.getWriteValueEvent(MTL.EqptObjectCate, MTL.EQPT_ID, "OHXC_TO_MTL_D2U_CAR_MOVING");
                vm_carMoving.setWriteValue(carMoving ? "1" : "0");
                return ISMControl.writeDeviceBlock(bcfApp, vm_carMoving);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
        }
        public override void GetMTL2OHxC_CarOutInterface(out bool carOutSafelyCheck, out bool carMoveComplete)
        {
            try
            {
                ValueRead vr_safety_check = bcfApp.getReadValueEvent(MTL.EqptObjectCate, MTL.EQPT_ID, "MTL_TO_OHXC_U2D_SAFETY_CHECK");
                // ValueRead vr_move_cmp = bcfApp.getReadValueEvent(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_U2D_MOVE_COMPLETE");
                carOutSafelyCheck = (bool)vr_safety_check.getText();
                // carMoveComplete = (bool)vr_move_cmp.getText();
                carMoveComplete = false;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                carOutSafelyCheck = false;
                carMoveComplete = false;
            }
        }
        public override void GetMTL2OHxC_CarInInterface(out bool carOutSafelyCheck, out bool carInInterlock)
        {
            try
            {
                ValueRead vr_safety_check = bcfApp.getReadValueEvent(MTL.EqptObjectCate, MTL.EQPT_ID, "MTL_TO_OHXC_D2U_SAFETY_CHECK");
                ValueRead vr_car_in = bcfApp.getReadValueEvent(MTL.EqptObjectCate, MTL.EQPT_ID, "MTL_TO_OHXC_D2U_CAR_IN_INTERLOCK");
                carOutSafelyCheck = (bool)vr_safety_check.getText();
                carInInterlock = (bool)vr_car_in.getText();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                carOutSafelyCheck = false;
                carInInterlock = false;
            }
        }

        public override void CarOutSafetyChcek(object sender, ValueChangedEventArgs args)
        {
            var recevie_function =
                scApp.getFunBaseObj<MtlToOHxC_CarOutSafetyCheck>(MTL.EQPT_ID) as MtlToOHxC_CarOutSafetyCheck;
            try
            {
                recevie_function.Read(bcfApp, MTL.EqptObjectCate, MTL.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLValueDefMapActionNew), Device: DEVICE_NAME_MTL,
                         Data: recevie_function.ToString(),
                         VehicleID: MTL.EQPT_ID);
                MTL.CarOutSafetyCheck = recevie_function.SafetyCheck;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<MtlToOHxC_CarOutSafetyCheck>(recevie_function);

            }
        }


        public override void MTL_LFTStatus(object sender, ValueChangedEventArgs args)
        {
            var recevie_function =
                scApp.getFunBaseObj<MtlToOHxC_LFTStatus>(MTL.EQPT_ID) as MtlToOHxC_LFTStatus;
            try
            {
                recevie_function.Read(bcfApp, MTL.EqptObjectCate, MTL.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLValueDefMapActionNew), Device: DEVICE_NAME_MTL,
                         Data: recevie_function.ToString(),
                         VehicleID: MTL.EQPT_ID);
                MTL.HasVehicle = recevie_function.HasVehicle;
                MTL.StopSingle = recevie_function.StopSingle;
                MTL.MTxMode = (ProtocolFormat.OHTMessage.MTxMode)recevie_function.Mode;
                MTL.MTLLocation = (ProtocolFormat.OHTMessage.MTLLocation)recevie_function.LFTLocation;
                MTL.MTLMovingStatus = (ProtocolFormat.OHTMessage.MTLMovingStatus)recevie_function.LFTMovingStatus;
                MTL.Encoder = recevie_function.LFTEncoder;
                MTL.VhInPosition = (ProtocolFormat.OHTMessage.VhInPosition)recevie_function.VhInPosition;
                MTL.SynchronizeTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<MtlToOHxC_LFTStatus>(recevie_function);

            }
        }
        public override void CarInSafetyChcek(object sender, ValueChangedEventArgs args)
        {
            var recevie_function =
                scApp.getFunBaseObj<MtlToOHxC_CarInSafetyCheck>(MTL.EQPT_ID) as MtlToOHxC_CarInSafetyCheck;
            try
            {
                recevie_function.Read(bcfApp, MTL.EqptObjectCate, MTL.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLValueDefMapActionNew), Device: DEVICE_NAME_MTL,
                         Data: recevie_function.ToString(),
                         VehicleID: MTL.EQPT_ID);
                MTL.CarInSafetyCheck = recevie_function.SafetyCheck;
                MTL.SynchronizeTime = DateTime.Now;

                if (MTL.CarInSafetyCheck)
                {
                    scApp.MTLService.carInSafetyAndVehicleStatusCheck(MTL);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<MtlToOHxC_CarInSafetyCheck>(recevie_function);

            }
        }
        public override void MTL_CarOutRequest(object sender, ValueChangedEventArgs args)
        {
            var recevie_function =
               scApp.getFunBaseObj<MtlToOHxC_MtlCarOutRepuest>(MTL.EQPT_ID) as MtlToOHxC_MtlCarOutRepuest;
            var send_function =
                scApp.getFunBaseObj<OHxCToMtl_MtlCarOutReply>(MTL.EQPT_ID) as OHxCToMtl_MtlCarOutReply;
            try
            {
                recevie_function.Read(bcfApp, MTL.EqptObjectCate, MTL.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLValueDefMapActionNew), Device: DEVICE_NAME_MTL,
                         Data: recevie_function.ToString(),
                         VehicleID: MTL.EQPT_ID);
                int pre_car_out_vh_num = recevie_function.CarID;
                ushort hand_shake = recevie_function.Handshake;
                if (hand_shake == 1)
                {
                    send_function.ReturnCode = 1;
                    if (recevie_function.Canacel == 1)
                    {
                        scApp.MTLService.carOutRequestCancle(MTL);
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTSValueDefMapActionNew), Device: DEVICE_NAME_MTL,
                                 Data: $"Process MTL car out cancel",
                                 VehicleID: MTL.EQPT_ID);
                    }
                    else
                    {
                        AVEHICLE pre_car_out_vh = scApp.VehicleBLL.cache.getVhByNum(pre_car_out_vh_num);
                        MaintainSpace maintainSpace = scApp.EquipmentBLL.cache.GetMaintainSpace();//todo 之後會有兩個MTS 要知道是哪個MTS
                        var car_out_check_result = scApp.MTLService.checkVhAndMTxCarOutStatus(this.MTL, maintainSpace, pre_car_out_vh);
                        send_function.ReturnCode = car_out_check_result.isSuccess ? (ushort)1 : (ushort)2;
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLValueDefMapActionNew), Device: DEVICE_NAME_MTL,
                                 Data: $"Process MTL car out request, is success:{car_out_check_result.isSuccess},result:{car_out_check_result.result}",
                                 VehicleID: MTL.EQPT_ID);

                    }
                }
                else
                {
                    send_function.ReturnCode = 0;
                }
                send_function.Handshake = hand_shake == 0 ? (ushort)0 : (ushort)1;
                send_function.Write(bcfApp, MTL.EqptObjectCate, MTL.EQPT_ID);

                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLValueDefMapActionNew), Device: DEVICE_NAME_MTL,
                         Data: send_function.ToString(),
                         VehicleID: MTL.EQPT_ID);
                MTL.SynchronizeTime = DateTime.Now;
                //if (send_function.Handshake == 1 && send_function.ReturnCode == 1)
                if (send_function.Handshake == 1 && send_function.ReturnCode == 1 && recevie_function.Canacel != 1)
                {
                    AVEHICLE pre_car_out_vh = scApp.VehicleBLL.cache.getVhByNum(pre_car_out_vh_num);
                    scApp.MTLService.processCarOutScenario(MTL, pre_car_out_vh);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<MtlToOHxC_MtlCarOutRepuest>(recevie_function);
                scApp.putFunBaseObj<OHxCToMtl_MtlCarOutReply>(send_function);
            }
        }

        public override void MTL_CarInRequest(object sender, ValueChangedEventArgs args)
        {
            var recevie_function =
                  scApp.getFunBaseObj<MtlToOHxC_RequestCarInDataCheck>(MTL.EQPT_ID) as MtlToOHxC_RequestCarInDataCheck;
            var send_function =
                scApp.getFunBaseObj<OHxCToMtl_ReplyCarInDataCheck>(MTL.EQPT_ID) as OHxCToMtl_ReplyCarInDataCheck;
            try
            {
                recevie_function.Read(bcfApp, MTL.EqptObjectCate, MTL.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLValueDefMapActionNew), Device: DEVICE_NAME_MTL,
                         Data: recevie_function.ToString(),
                         VehicleID: MTL.EQPT_ID);
                ushort vh_num = recevie_function.CarID;
                ushort hand_shake = recevie_function.Handshake;

                AVEHICLE pre_car_in_vh = scApp.VehicleBLL.cache.getVhByNum(vh_num);
                if (pre_car_in_vh != null)
                {
                    var check_result = scApp.MTLService.checkVhAndMTxCarInStatus(MTL, null, pre_car_in_vh);
                    send_function.ReturnCode = check_result.isSuccess ? (UInt16)1 : (UInt16)3;
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLValueDefMapActionNew), Device: DEVICE_NAME_MTL,
                             Data: $"check mtl car in result, is success:{check_result.isSuccess},result:{check_result.result}",
                             VehicleID: MTL.EQPT_ID);
                }
                else
                {
                    send_function.ReturnCode = 2;
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLValueDefMapActionNew), Device: DEVICE_NAME_MTL,
                             Data: $"check mts cal in result, vehicle num:{vh_num} not exist.",
                             VehicleID: MTL.EQPT_ID);
                }
                send_function.Handshake = hand_shake;
                send_function.Write(bcfApp, MTL.EqptObjectCate, MTL.EQPT_ID);

                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTLValueDefMapActionNew), Device: DEVICE_NAME_MTL,
                         Data: send_function.ToString(),
                         VehicleID: MTL.EQPT_ID);
                MTL.SynchronizeTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<MtlToOHxC_RequestCarInDataCheck>(recevie_function);
                scApp.putFunBaseObj<OHxCToMtl_ReplyCarInDataCheck>(send_function);

            }
        }

    }
}
