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
    public abstract class MTxValueDefMapActionBase : IValueDefMapAction
    {
        public const string DEVICE_NAME_MTL = "MTx";
        Logger logger = NLog.LogManager.GetCurrentClassLogger();
        protected AEQPT eqpt = null;
        public virtual void setContext(BaseEQObject baseEQ)
        {
            this.eqpt = baseEQ as AEQPT;
        }
        protected SCApplication scApp = null;
        protected BCFApplication bcfApp = null;

        public MTxValueDefMapActionBase()
            : base()
        {
            scApp = SCApplication.getInstance();
            bcfApp = scApp.getBCFApplication();
        }
        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new AspectWeaver(parameter, this);
        }

        public virtual string getIdentityKey()
        {
            return this.GetType().Name;
        }
        public virtual void unRegisterEvent()
        {
            //not implement
        }
        public virtual void doShareMemoryInit(BCFAppConstants.RUN_LEVEL runLevel)
        {
            try
            {
                switch (runLevel)
                {
                    case BCFAppConstants.RUN_LEVEL.ZERO:
                        CarOutSafetyChcek(null, null);
                        CarInSafetyChcek(null, null);
                        MTL_LFTStatus(null, null);
                        break;
                    case BCFAppConstants.RUN_LEVEL.ONE:
                        break;
                    case BCFAppConstants.RUN_LEVEL.TWO:
                        break;
                    case BCFAppConstants.RUN_LEVEL.NINE:
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
            }
        }

        object dateTimeSyneObj = new object();
        uint dateTimeIndex = 0;
        public virtual void DateTimeSyncCommand(DateTime dateTime)
        {

            OHxCToMtl_DateTimeSync send_function =
               scApp.getFunBaseObj<OHxCToMtl_DateTimeSync>(eqpt.EQPT_ID) as OHxCToMtl_DateTimeSync;
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
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTxValueDefMapActionBase), Device: DEVICE_NAME_MTL,
                             Data: send_function.ToString(),
                             VehicleID: eqpt.EQPT_ID);
                    //3.發送訊息
                    send_function.Write(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
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
        public virtual void OHxCMessageDownload(string msg)
        {
            OHxCToMtl_MessageDownload send_function =
                scApp.getFunBaseObj<OHxCToMtl_MessageDownload>(eqpt.EQPT_ID) as OHxCToMtl_MessageDownload;
            try
            {
                //1.建立各個Function物件
                send_function.Message = msg;
                if (message_index > 9999)
                { message_index = 0; }
                send_function.Index = ++message_index;
                //2.紀錄發送資料的Log
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTxValueDefMapActionBase), Device: DEVICE_NAME_MTL,
                         Data: send_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                //3.發送訊息
                send_function.Write(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
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
        public virtual void CarRealtimeInfo(UInt16 car_id, UInt16 action_mode, UInt16 cst_exist, UInt16 current_section_id, UInt32 current_address_id,
                                            UInt32 buffer_distance, UInt16 speed)
        {
            OHxCToMtl_CarRealtimeInfo send_function =
                scApp.getFunBaseObj<OHxCToMtl_CarRealtimeInfo>(eqpt.EQPT_ID) as OHxCToMtl_CarRealtimeInfo;
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
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTxValueDefMapActionBase), Device: DEVICE_NAME_MTL,
                         Data: send_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                //3.發送訊息
                send_function.Write(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
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

        public virtual (bool isSendSuccess, UInt16 returnCode) OHxC_CarOutNotify(UInt16 car_id)
        {
            bool isSendSuccess = false;
            var send_function =
                scApp.getFunBaseObj<OHxCToMtl_CarOutNotify>(eqpt.EQPT_ID) as OHxCToMtl_CarOutNotify;
            var receive_function =
                scApp.getFunBaseObj<MtlToOHxC_ReplyCarOutNotify>(eqpt.EQPT_ID) as MtlToOHxC_ReplyCarOutNotify;
            try
            {
                //1.準備要發送的資料
                send_function.CarID = car_id;
                ValueRead vr_reply = receive_function.getValueReadHandshake
                    (bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                //2.紀錄發送資料的Log
                send_function.Handshake = 1;
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTxValueDefMapActionBase), Device: DEVICE_NAME_MTL,
                         Data: send_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                //3.等待回復
                TrxMPLC.ReturnCode returnCode =
                    send_function.SendRecv(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID, vr_reply);
                //4.取得回復的結果
                if (returnCode == TrxMPLC.ReturnCode.Normal)
                {
                    receive_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTxValueDefMapActionBase), Device: DEVICE_NAME_MTL,
                             Data: receive_function.ToString(),
                             VehicleID: eqpt.EQPT_ID);
                    isSendSuccess = true;
                }
                send_function.Handshake = 0;
                send_function.resetHandshake(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTxValueDefMapActionBase), Device: DEVICE_NAME_MTL,
                         Data: send_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
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

        public virtual bool OHxC_AlarmResetRequest()
        {
            bool isSendSuccess = false;
            var send_function =
                scApp.getFunBaseObj<OHxCToMtl_AlarmResetRequest>(eqpt.EQPT_ID) as OHxCToMtl_AlarmResetRequest;
            var receive_function =
                scApp.getFunBaseObj<MtlToOHxC_AlarmResetReply>(eqpt.EQPT_ID) as MtlToOHxC_AlarmResetReply;
            try
            {
                //1.準備要發送的資料
                ValueRead vr_reply = receive_function.getValueReadHandshake
                    (bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                //2.紀錄發送資料的Log
                send_function.Handshake = 1;
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTxValueDefMapActionBase), Device: DEVICE_NAME_MTL,
                         Data: send_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                //3.等待回復
                TrxMPLC.ReturnCode returnCode =
                    send_function.SendRecv(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID, vr_reply);
                //4.取得回復的結果
                if (returnCode == TrxMPLC.ReturnCode.Normal)
                {
                    receive_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTxValueDefMapActionBase), Device: DEVICE_NAME_MTL,
                             Data: receive_function.ToString(),
                             VehicleID: eqpt.EQPT_ID);
                    isSendSuccess = true;
                }
                send_function.Handshake = 0;
                send_function.resetHandshake(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTxValueDefMapActionBase), Device: DEVICE_NAME_MTL,
                         Data: send_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<OHxCToMtl_AlarmResetRequest>(send_function);
                scApp.putFunBaseObj<MtlToOHxC_AlarmResetReply>(receive_function);
            }
            return (isSendSuccess);
        }

        public virtual void OHxC2MTL_CarOutInterface(bool carOutInterlock, bool carOutReady, bool carMoving, bool carMoveComplete)
        {
            try
            {
                ValueWrite vm_carOutInterlock = bcfApp.getWriteValueEvent(eqpt.EqptObjectCate, eqpt.EQPT_ID, "OHXC_TO_MTL_U2D_CAR_OUT_INTERLOCK");
                ValueWrite vm_carOutReady = bcfApp.getWriteValueEvent(eqpt.EqptObjectCate, eqpt.EQPT_ID, "OHXC_TO_MTL_U2D_CAR_OUT_READY");
                ValueWrite vm_carMoving = bcfApp.getWriteValueEvent(eqpt.EqptObjectCate, eqpt.EQPT_ID, "OHXC_TO_MTL_U2D_CAR_MOVING");
                ValueWrite vm_carMoveCmp = bcfApp.getWriteValueEvent(eqpt.EqptObjectCate, eqpt.EQPT_ID, "OHXC_TO_MTL_U2D_CAR_MOVE_COMPLETE");
                string setValue = carOutInterlock ? "1" : "0";
                vm_carOutInterlock.setWriteValue(carOutInterlock ? "1" : "0");
                vm_carOutReady.setWriteValue(carOutReady ? "1" : "0");
                vm_carMoving.setWriteValue(carMoving ? "1" : "0");
                vm_carMoveCmp.setWriteValue(carMoveComplete ? "1" : "0");
                bool result = ISMControl.writeDeviceBlock(bcfApp, vm_carOutInterlock);
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
        public virtual bool setOHxC2MTL_CarOutInterlock(bool carOutInterlock)
        {
            try
            {
                ValueWrite vm_carOutInterlock = bcfApp.getWriteValueEvent(eqpt.EqptObjectCate, eqpt.EQPT_ID, "OHXC_TO_MTL_U2D_CAR_OUT_INTERLOCK");
                string setValue = carOutInterlock ? "1" : "0";
                vm_carOutInterlock.setWriteValue(setValue);
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
        public virtual bool GetOHxC2MTL_CarOutInterlock()
        {
            try
            {
                ValueWrite vm_carOutInterlock = bcfApp.getWriteValueEvent(eqpt.EqptObjectCate, eqpt.EQPT_ID, "OHXC_TO_MTL_U2D_CAR_OUT_INTERLOCK");
                eqpt.Interlock = (Boolean)vm_carOutInterlock.getText();
                return (Boolean)vm_carOutInterlock.getText();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                eqpt.Interlock = false;
                return false;
            }
        }
        public virtual void OHxC2MTL_CarInInterface(bool carMoving, bool carMoveComplete)
        {
            try
            {
                ValueWrite vm_carMoving = bcfApp.getWriteValueEvent(eqpt.EqptObjectCate, eqpt.EQPT_ID, "OHXC_TO_MTL_D2U_CAR_MOVING");
                ValueWrite vm_carMoveCmp = bcfApp.getWriteValueEvent(eqpt.EqptObjectCate, eqpt.EQPT_ID, "OHXC_TO_MTL_D2U_CAR_MOVE_COMPLETE");
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
        public virtual bool setOHxC2MTL_CarInMoving(bool carMoving)
        {
            try
            {
                ValueWrite vm_carMoving = bcfApp.getWriteValueEvent(eqpt.EqptObjectCate, eqpt.EQPT_ID, "OHXC_TO_MTL_D2U_CAR_MOVING");
                vm_carMoving.setWriteValue(carMoving ? "1" : "0");
                return ISMControl.writeDeviceBlock(bcfApp, vm_carMoving);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
        }
        public virtual bool GetOHxC2MTL_CarInMoving()
        {
            try
            {
                ValueWrite vm_carMoving = bcfApp.getWriteValueEvent(eqpt.EqptObjectCate, eqpt.EQPT_ID, "OHXC_TO_MTL_D2U_CAR_MOVING");
                return (Boolean)vm_carMoving.getText();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
        }

        public virtual void GetMTL2OHxC_CarOutInterface(out bool carOutSafelyCheck, out bool carMoveComplete)
        {
            try
            {
                ValueRead vr_safety_check = bcfApp.getReadValueEvent(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_U2D_SAFETY_CHECK");
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
        public virtual void GetMTL2OHxC_CarInInterface(out bool carOutSafelyCheck, out bool carInInterlock)
        {
            try
            {
                ValueRead vr_safety_check = bcfApp.getReadValueEvent(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_D2U_SAFETY_CHECK");
                ValueRead vr_car_in = bcfApp.getReadValueEvent(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_D2U_CAR_IN_INTERLOCK");
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

        public virtual void CarOutSafetyChcek(object sender, ValueChangedEventArgs args)
        {
            var recevie_function =
                scApp.getFunBaseObj<MtlToOHxC_CarOutSafetyCheck>(eqpt.EQPT_ID) as MtlToOHxC_CarOutSafetyCheck;
            try
            {
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTxValueDefMapActionBase), Device: DEVICE_NAME_MTL,
                         Data: recevie_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                eqpt.CarOutSafetyCheck = recevie_function.SafetyCheck;
                eqpt.SynchronizeTime = DateTime.Now;
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
        public virtual void MTL_Alarm_Report(object sender, ValueChangedEventArgs args)
        {
            var recevie_function =
                scApp.getFunBaseObj<MtlToOHxC_AlarmReport>(eqpt.EQPT_ID) as MtlToOHxC_AlarmReport;
            var send_function =
                scApp.getFunBaseObj<MtlToOHxC_ReplyAlarmReport>(eqpt.EQPT_ID) as MtlToOHxC_ReplyAlarmReport;
            try
            {
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTxValueDefMapActionBase), Device: DEVICE_NAME_MTL,
                         Data: recevie_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                UInt16 error_code = recevie_function.ErrorCode;
                ProtocolFormat.OHTMessage.ErrorStatus status = (ProtocolFormat.OHTMessage.ErrorStatus)recevie_function.ErrorStatus;
                ushort hand_shake = recevie_function.Handshake;


                send_function.Handshake = hand_shake;
                send_function.Write(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                eqpt.SynchronizeTime = DateTime.Now;

                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTxValueDefMapActionBase), Device: DEVICE_NAME_MTL,
                         Data: send_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<MtlToOHxC_AlarmReport>(recevie_function);
                scApp.putFunBaseObj<MtlToOHxC_ReplyAlarmReport>(send_function);

            }
        }
        public virtual void MTL_DATETIME(object sender, ValueChangedEventArgs args)
        {

            try
            {
                ValueRead vr = sender as ValueRead;
                UInt16[] dateTime = (UInt16[])vr.getText();
                eqpt.SynchronizeTime = DateTime.Now;
                Console.WriteLine(String.Join(",", new List<UInt16>(dateTime).ConvertAll(i => i.ToString()).ToArray()));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }
        public virtual void MTL_Alive(object sender, ValueChangedEventArgs args)
        {
            var recevie_function =
                scApp.getFunBaseObj<MtlToOHxC_Alive>(eqpt.EQPT_ID) as MtlToOHxC_Alive;
            try
            {
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTxValueDefMapActionBase), Device: DEVICE_NAME_MTL,
                //         Data: recevie_function.ToString(),
                //         VehicleID: eqpt.EQPT_ID);
                eqpt.Eq_Alive_Last_Change_time = DateTime.Now;
                eqpt.SynchronizeTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<MtlToOHxC_Alive>(recevie_function);

            }
        }
        public virtual void MTL_Current_ID(object sender, ValueChangedEventArgs args)
        {
            var recevie_function =
                scApp.getFunBaseObj<MtlToOHxC_CurrentCarID>(eqpt.EQPT_ID) as MtlToOHxC_CurrentCarID;
            try
            {
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTxValueDefMapActionBase), Device: DEVICE_NAME_MTL,
                         Data: recevie_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                eqpt.CurrentCarID = recevie_function.CarID.ToString();
                eqpt.SynchronizeTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<MtlToOHxC_CurrentCarID>(recevie_function);

            }
        }
        public virtual void MTL_TO_OHXC_REPLY_OHXC_CAR_OUT_NOTIFY_HS(object sender, ValueChangedEventArgs args)
        {
            var recevie_function =
                scApp.getFunBaseObj<MtlToOHxC_ReplyCarOutNotify>(eqpt.EQPT_ID) as MtlToOHxC_ReplyCarOutNotify;
            try
            {
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                eqpt.SynchronizeTime = DateTime.Now;
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MTxValueDefMapActionBase), Device: DEVICE_NAME_MTL,
                         Data: recevie_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<MtlToOHxC_ReplyCarOutNotify>(recevie_function);

            }
        }

        public abstract void MTL_LFTStatus(object sender, ValueChangedEventArgs args);
        public abstract void MTL_CarOutRequest(object sender, ValueChangedEventArgs args);
        public abstract void MTL_CarInRequest(object sender, ValueChangedEventArgs args);
        public abstract void CarInSafetyChcek(object sender, ValueChangedEventArgs args);

        /// <summary>
        /// Does the initialize.
        /// </summary>
        public virtual void doInit()
        {
            try
            {
                ValueRead vr = null;

                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_U2D_SAFETY_CHECK", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => CarOutSafetyChcek(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_D2U_SAFETY_CHECK", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => CarInSafetyChcek(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_ALARM_REPORT_HS", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => MTL_Alarm_Report(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_DATETIME", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => MTL_DATETIME(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_ALIVE_INDEX", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => MTL_Alive(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_CURRENT_CAR_ID_INDEX", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => MTL_Current_ID(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_REPLY_OHXC_CAR_OUT_NOTIFY_HS", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => MTL_TO_OHXC_REPLY_OHXC_CAR_OUT_NOTIFY_HS(_sender, _e);
                }


                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_LFT_STATUS_HAS_VEHICLE", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => MTL_LFTStatus(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_LFT_STATUS_STOP_SINGLE", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => MTL_LFTStatus(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_LFT_MODE", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => MTL_LFTStatus(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_LFT_LOCATION", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => MTL_LFTStatus(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_LFT_MOVING_STATUS", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => MTL_LFTStatus(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_LFT_ENCODER", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => MTL_LFTStatus(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_LFT_VEHICLE_PARKED", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => MTL_LFTStatus(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_MTL_CAR_OUT_REQUEST_HS", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => MTL_CarOutRequest(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_REQUEST_CAR_IN_DATA_CHECK_HS", out vr))
                {
                    vr.afterValueChange += (_sender, _e) => MTL_CarInRequest(_sender, _e);
                }
                //if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_U2D_SAFETY_CHECK", out vr))
                //{
                //    vr.afterValueChange += (_sender, _e) => MTL_LFTStatus(_sender, _e);
                //}
                //if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_D2U_SAFETY_CHECK", out vr))
                //{
                //    vr.afterValueChange += (_sender, _e) => MTL_LFTStatus(_sender, _e);
                //}
                //if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MTL_TO_OHXC_D2U_CAR_IN_INTERLOCK", out vr))
                //{
                //    vr.afterValueChange += (_sender, _e) => MTL_LFTStatus(_sender, _e);
                //}
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
            }

        }


    }
}
