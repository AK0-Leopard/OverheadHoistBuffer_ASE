//*********************************************************************************
//      OHCVValueDefMapAction.cs
//*********************************************************************************
// File Name: OHCVValueDefMapAction.cs
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
using com.mirle.iibg3k0.ttc.Common;
using KingAOP;
using NLog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace com.mirle.ibg3k0.sc.Data.ValueDefMapAction
{
    public class OHCVValueDefMapAction : IValueDefMapAction
    {
        public const string DEVICE_NAME_OHCV = "OHCV";
        Logger logger = NLog.LogManager.GetCurrentClassLogger();
        OHCV eqpt = null;
        ANODE node = null;
        OHCV pair_eqpt = null;

        protected SCApplication scApp = null;
        protected BCFApplication bcfApp = null;
        public OHCVValueDefMapAction()
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
        public virtual void setContext(BaseEQObject baseEQ)
        {
            try
            {
                this.eqpt = baseEQ as OHCV;
                node = scApp.getEQObjCacheManager().getParentNodeByEQPTID(eqpt.EQPT_ID);
                string[] str_arr = eqpt.EQPT_ID.Split('_');
                string code = str_arr[1] == "A" ? "B" : "A";
                //pair_eqpt = scApp.getEQObjCacheManager().getEquipmentByEQPTID(eqpt.EQPT_ID.Split('_')[0] + "_B") as OHCV;
                pair_eqpt = scApp.getEQObjCacheManager().getEquipmentByEQPTID(eqpt.EQPT_ID.Split('_')[0] + "_" + code) as OHCV;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
            }
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
                        initialValueRead();
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

        private void initialValueRead()
        {
            ValueRead OHCV_Alive_vr = null;
            if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "OHCV_TO_OHTC_ALIVE", out OHCV_Alive_vr))
            {
                AliveChange(OHCV_Alive_vr, null);
            }
            InitialSafetyCheckRequest();
            ValueRead Safety_Check_Complete_vr = null;
            if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "SAFETY_CHECK_COMPLETE", out Safety_Check_Complete_vr))
            {
                SafetyCheckCompleteChange(Safety_Check_Complete_vr, null);
            }
            ValueRead Door_Close_vr = null;
            if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "DOOR_CLOSE", out Door_Close_vr))
            {
                DoorCloseChange(Door_Close_vr, null);
            }

        }

        public virtual void DoorCloseChange(object sender, ValueChangedEventArgs args)
        {
            try
            {
                ValueRead vr = sender as ValueRead;
                BCFUtility.writeEquipmentLog(eqpt.EQPT_ID, new List<ValueRead> { vr });

                Boolean doorClosed = (Boolean)vr.getText();
                eqpt.DoorClosed = doorClosed;
                //node.DoorClosed = eqpt.DoorClosed && pair_eqpt.DoorClosed;
                if (pair_eqpt != null)
                {
                    node.DoorClosed = eqpt.DoorClosed && pair_eqpt.DoorClosed;
                }
                else
                {
                    node.DoorClosed = eqpt.DoorClosed;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }

        public virtual void SafetyCheckCompleteChange(object sender, ValueChangedEventArgs args)
        {
            try
            {
                var recevie_function = scApp.getFunBaseObj<OHCVToOHxC_SafetyCheckComplete>(eqpt.EQPT_ID) as OHCVToOHxC_SafetyCheckComplete;
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(OHCVValueDefMapAction), Device: DEVICE_NAME_OHCV,
                         Data: recevie_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                eqpt.SafetyCheckComplete = recevie_function.SafetyCheckComplete;

                //node.SafetyCheckComplete = eqpt.SafetyCheckComplete || pair_eqpt.SafetyCheckComplete;
                if (pair_eqpt != null)
                {
                    node.SafetyCheckComplete = eqpt.SafetyCheckComplete || pair_eqpt.SafetyCheckComplete;
                }
                else
                {
                    node.SafetyCheckComplete = eqpt.SafetyCheckComplete;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }
        public virtual void InitialSafetyCheckRequest()
        {
            try
            {
                var recevie_function = scApp.getFunBaseObj<OHCVToOHxC_SafetyCheckRequest>(eqpt.EQPT_ID) as OHCVToOHxC_SafetyCheckRequest;
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(OHCVValueDefMapAction), Device: DEVICE_NAME_OHCV,
                         Data: recevie_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                eqpt.SafetyCheckRequest = recevie_function.SafetyCheckRequest;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }
        public virtual void SafetyCheckRequestChange(object sender, ValueChangedEventArgs args)
        {
            try
            {
                string pre_control_segment_id = eqpt.SegmentLocation;
                ASEGMENT pre_control_segment = scApp.SegmentBLL.cache.GetSegment(pre_control_segment_id);

                var recevie_function = scApp.getFunBaseObj<OHCVToOHxC_SafetyCheckRequest>(eqpt.EQPT_ID) as OHCVToOHxC_SafetyCheckRequest;
                recevie_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(OHCVValueDefMapAction), Device: DEVICE_NAME_OHCV,
                         Data: recevie_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                eqpt.SafetyCheckRequest = recevie_function.SafetyCheckRequest;
                if (recevie_function.SafetyCheckRequest)
                {
                    //if (!node.DoorClosed)//檢查安全門是不是關閉的
                    if (!eqpt.DoorClosed)//檢查安全門是不是關閉的
                    {
                        BCFApplication.onWarningMsg($"OHCV:[{eqpt.EQPT_ID}] send safety check request to OHTC,but door is open now.");
                        return;
                    }
                    //if (!node.Is_Alive)//檢查Alive有沒有變化
                    if (!eqpt.Is_Eq_Alive)//檢查Alive有沒有變化
                    {
                        BCFApplication.onWarningMsg($"OHCV:[{eqpt.EQPT_ID}] send safety check request to OHTC,but OHCV alive is not changing.");
                        return;
                    }

                    bool initNotifyResult = sendRoadControlInitialNotify();

                    if (initNotifyResult)
                    {
                        if (eqpt.SafetyCheckRequest)//沒有被取消
                        {
                            //如果現在這組CV的狀態是DISABLE而且是由SAFETY_DISABLE而且有通知這組CV已經可以開啟，
                            //就可以直接回覆Complete
                            if (pre_control_segment.DISABLE_FLAG_SAFETY &&
                                pre_control_segment.STATUS == E_SEG_STATUS.Closed &&
                                node.SafetyCheckComplete)
                            {
                                Pre_control_segment_ControlComplete(pre_control_segment, null);
                            }
                            else
                            {
                                //註冊該segment control complete的事件
                                pre_control_segment.ControlComplete += Pre_control_segment_ControlComplete;

                                //開始處理CV的OP in request時，要處理的事項
                                //1.predisable 該CV所在路段 Todo by Kevin
                                //2.確認無車輛會再過該CV路段 Todo by Kevin
                                //3.disable 該CV所在路段 Todo by Kevin
                                //4.當上述條件成功時，會觸發"ControlComplete"的事件
                                scApp.RoadControlService.ProcessCVOpInRequest(eqpt);
                            }
                            //bool completeNotifyResult = sendRoadControlCompleteNotify(); //disable路段後發出Complete Notify
                            //if (completeNotifyResult)
                            //{
                            //    //do nothing
                            //}
                            //else
                            //{
                            //    BCFApplication.onWarningMsg($"OHTC send road control complete notify to OHCV:[{eqpt.EQPT_ID}] with error happend.");
                            //    return;
                            //}

                        }
                        else//被中途取消了
                        {
                            //do nothing
                        }
                    }
                    else
                    {
                        BCFApplication.onWarningMsg($"OHTC send road control initial notify to OHCV:[{eqpt.EQPT_ID}] with error happend.");
                        return;
                    }
                }
                else//SafetyCheckRequest訊號OFF
                {
                    //當SafetyCheckRequest關閉後，就再次取消註冊的Control complete事件，確保已經不會再收到控制完成的通知。
                    pre_control_segment.ControlComplete -= Pre_control_segment_ControlComplete;

                    if (eqpt.SafetyCheckComplete)
                    {
                        //donothing
                    }
                    else
                    {
                        cancelSafetyCheckRequest();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }

        private void Pre_control_segment_ControlComplete(object sender, EventArgs e)
        {
            ASEGMENT control_complete_segment = sender as ASEGMENT;
            try
            {
                //再次確認是否Safety check=true、door closed=true、alive=true
                if (!eqpt.SafetyCheckRequest)
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(OHCVValueDefMapAction), Device: DEVICE_NAME_OHCV,
                             Data: $"want to notify cv:{eqpt.EQPT_ID} segment id:{control_complete_segment.SEG_NUM} is control complete," +
                                   $"but cv:{eqpt.EQPT_ID} of {nameof(eqpt.SafetyCheckRequest)} is {eqpt.SafetyCheckRequest}",
                             VehicleID: eqpt.EQPT_ID);
                    return;
                }

                if (!eqpt.DoorClosed)
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(OHCVValueDefMapAction), Device: DEVICE_NAME_OHCV,
                             Data: $"want to notify cv:{eqpt.EQPT_ID} segment id:{control_complete_segment.SEG_NUM} is control complete," +
                                   $"but cv:{eqpt.EQPT_ID} of {nameof(eqpt.DoorClosed)} is {eqpt.DoorClosed}",
                             VehicleID: eqpt.EQPT_ID);
                    return;
                }
                if (!eqpt.Is_Eq_Alive)
                {
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(OHCVValueDefMapAction), Device: DEVICE_NAME_OHCV,
                             Data: $"want to notify cv:{eqpt.EQPT_ID} segment id:{control_complete_segment.SEG_NUM} is control complete," +
                                   $"but cv:{eqpt.EQPT_ID} of {nameof(eqpt.Is_Eq_Alive)} is {eqpt.Is_Eq_Alive}",
                             VehicleID: eqpt.EQPT_ID);
                    return;
                }


                //foreach (var cv in node.getSubEqptList())
                //{
                //    if (!cv.DoorClosed)
                //    {
                //        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(OHCVValueDefMapAction), Device: DEVICE_NAME_OHCV,
                //                 Data: $"want to notify cv:{eqpt.EQPT_ID} segment id:{control_complete_segment.SEG_NUM} is control complete," +
                //                       $"but cv:{cv.EQPT_ID} of {nameof(cv.DoorClosed)} is {cv.DoorClosed}",
                //                 VehicleID: eqpt.EQPT_ID);
                //        return;
                //    }
                //    if (!cv.Is_Eq_Alive)
                //    {
                //        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(OHCVValueDefMapAction), Device: DEVICE_NAME_OHCV,
                //                 Data: $"want to notify cv:{eqpt.EQPT_ID} segment id:{control_complete_segment.SEG_NUM} is control complete," +
                //                       $"but cv:{cv.EQPT_ID} of {nameof(cv.Is_Eq_Alive)} is {cv.Is_Eq_Alive}",
                //                 VehicleID: eqpt.EQPT_ID);
                //        return;
                //    }
                //}


                bool completeNotifyResult = sendRoadControlCompleteNotify(); //disable路段後發出Complete Notify
                if (completeNotifyResult)
                {
                    //do nothing
                }
                else
                {
                    BCFApplication.onWarningMsg($"OHTC send road control complete notify to OHCV:[{eqpt.EQPT_ID}] with error happend.");
                    //return;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
            }
            finally
            {
                control_complete_segment.ControlComplete -= Pre_control_segment_ControlComplete;
            }
        }

        private void cancelSafetyCheckRequest()
        {
            if (!node.DoorClosed)//檢查安全門是不是關閉的
            {
                BCFApplication.onWarningMsg($"OHCV:[{eqpt.EQPT_ID}] send safety check request cancel to OHTC,but door is open now.");
                return;
            }
            if (!node.Is_Alive)//檢查Alive有沒有變化
            {
                BCFApplication.onWarningMsg($"OHCV:[{eqpt.EQPT_ID}] send safety check request cancel to OHTC,but OHCV alive is not changing.");
                return;
            }
            if (node.SafetyCheckComplete)//確定是否有通知過其它門，現在是可以開啟的
            {
                return;
            }
            //enable 該CV所在路段 Todo by Kevin
            scApp.RoadControlService.doEnableDisableSegment
                (eqpt.SegmentLocation, E_PORT_STATUS.InService, ASEGMENT.DisableType.Safety, Data.SECS.CSOT.SECSConst.LANECUTTYPE_LaneCutOnHMI);
        }

        public virtual bool sendRoadControlInitialNotify()
        {

            bool isSendSuccess = false;
            var send_function =
                scApp.getFunBaseObj<OHxCToOHVC_RoadControlInitNotify>(eqpt.EQPT_ID) as OHxCToOHVC_RoadControlInitNotify;
            var receive_function =
                scApp.getFunBaseObj<OHxCToOHVC_RoadControlInitNotifyReply>(eqpt.EQPT_ID) as OHxCToOHVC_RoadControlInitNotifyReply;
            ValueRead ReplyRoadControlInitialNotifyVW = scApp.getBCFApplication().getReadValueEvent(SCAppConstants.EQPT_OBJECT_CATE_EQPT, eqpt.EQPT_ID, "REPLY_ROAD_CONTROL_INITIAL_NOTIFY");
            ValueWrite RoadControlInitialNotifyVW = scApp.getBCFApplication().getWriteValueEvent(SCAppConstants.EQPT_OBJECT_CATE_EQPT, eqpt.EQPT_ID, "ROAD_CONTROL_INITIAL_NOTIFY");
            try
            {
                //1.準備要發送的資料
                ValueRead vr_reply = receive_function.getValueReadHandshake
                    (bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                //2.紀錄發送資料的Log
                send_function.Handshake = true;
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(OHCVValueDefMapAction), Device: DEVICE_NAME_OHCV,
                         Data: send_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                //3.等待回復
                TrxMPLC.ReturnCode on_returnCode =
                    send_function.SendRecv(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID, vr_reply);
                //4.取得回復的結果
                if (on_returnCode == TrxMPLC.ReturnCode.Normal)
                {
                    receive_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(OHCVValueDefMapAction), Device: DEVICE_NAME_OHCV,
                             Data: receive_function.ToString(),
                             VehicleID: eqpt.EQPT_ID);
                    isSendSuccess = true;
                }
                else
                {
                    isSendSuccess = false;
                }



                send_function.Handshake = false;
                //send_function.resetHandshake(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(OHCVValueDefMapAction), Device: DEVICE_NAME_OHCV,
                Data: send_function.ToString(),
                VehicleID: eqpt.EQPT_ID);
                if (isSendSuccess)
                {
                    TrxMPLC.ReturnCode off_returnCode = send_function.SendRecv(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID, vr_reply);
                    if (off_returnCode == TrxMPLC.ReturnCode.Normal)
                    {
                        receive_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(OHCVValueDefMapAction), Device: DEVICE_NAME_OHCV,
                                 Data: receive_function.ToString(),
                                 VehicleID: eqpt.EQPT_ID);
                        isSendSuccess = true;
                    }
                    else
                    {
                        isSendSuccess = false;
                    }
                }
                else
                {
                    send_function.resetHandshake(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<OHxCToOHVC_RoadControlInitNotify>(send_function);
                scApp.putFunBaseObj<OHxCToOHVC_RoadControlInitNotifyReply>(receive_function);
            }
            return (isSendSuccess);
        }

        public virtual bool sendRoadControlCompleteNotify()
        {

            bool isSendSuccess = false;
            var send_function =
                scApp.getFunBaseObj<OHxCToOHVC_RoadControlCompleteNotify>(eqpt.EQPT_ID) as OHxCToOHVC_RoadControlCompleteNotify;
            var receive_function =
                scApp.getFunBaseObj<OHxCToOHVC_RoadControlCompleteNotifyReply>(eqpt.EQPT_ID) as OHxCToOHVC_RoadControlCompleteNotifyReply;
            ValueRead ReplyRoadControlCompleteNotifyVW = scApp.getBCFApplication().getReadValueEvent(SCAppConstants.EQPT_OBJECT_CATE_EQPT, eqpt.EQPT_ID, "REPLY_ROAD_CONTROL_COMPLETE_NOTIFY");
            ValueWrite RoadControlCompleteNotifyVW = scApp.getBCFApplication().getWriteValueEvent(SCAppConstants.EQPT_OBJECT_CATE_EQPT, eqpt.EQPT_ID, "ROAD_CONTROL_COMPLETE_NOTIFY");
            try
            {
                //1.準備要發送的資料
                ValueRead vr_reply = receive_function.getValueReadHandshake
                    (bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                //2.紀錄發送資料的Log
                send_function.Handshake = true;
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(OHCVValueDefMapAction), Device: DEVICE_NAME_OHCV,
                         Data: send_function.ToString(),
                         VehicleID: eqpt.EQPT_ID);
                //3.等待回復
                TrxMPLC.ReturnCode on_returnCode =
                    send_function.SendRecv(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID, vr_reply);
                //4.取得回復的結果
                if (on_returnCode == TrxMPLC.ReturnCode.Normal)
                {
                    receive_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(OHCVValueDefMapAction), Device: DEVICE_NAME_OHCV,
                             Data: receive_function.ToString(),
                             VehicleID: eqpt.EQPT_ID);
                    isSendSuccess = true;
                }
                else
                {
                    isSendSuccess = false;
                }

                send_function.Handshake = false;
                //send_function.resetHandshake(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(OHCVValueDefMapAction), Device: DEVICE_NAME_OHCV,
                Data: send_function.ToString(),
                VehicleID: eqpt.EQPT_ID);
                if (isSendSuccess)
                {
                    TrxMPLC.ReturnCode off_returnCode = send_function.SendRecv(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID, vr_reply);
                    if (off_returnCode == TrxMPLC.ReturnCode.Normal)
                    {
                        receive_function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(OHCVValueDefMapAction), Device: DEVICE_NAME_OHCV,
                                 Data: receive_function.ToString(),
                                 VehicleID: eqpt.EQPT_ID);
                        isSendSuccess = true;
                    }
                    else
                    {
                        isSendSuccess = false;
                    }
                }
                else
                {
                    send_function.resetHandshake(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<OHxCToOHVC_RoadControlCompleteNotify>(send_function);
                scApp.putFunBaseObj<OHxCToOHVC_RoadControlCompleteNotifyReply>(receive_function);
            }
            return (isSendSuccess);
        }
        public virtual void AliveChange(object sender, ValueChangedEventArgs args)
        {
            try
            {
                ValueRead vr = sender as ValueRead;
                BCFUtility.writeEquipmentLog(eqpt.EQPT_ID, new List<ValueRead> { vr });

                Boolean alive = (Boolean)vr.getText();
                eqpt.Is_Eq_Alive = alive;
                eqpt.Eq_Alive_Last_Change_time = DateTime.Now;
                //node.Is_Alive = eqpt.Is_Eq_Alive && pair_eqpt.Is_Eq_Alive;
                if (pair_eqpt != null)
                {
                    node.Is_Alive = eqpt.Is_Eq_Alive && pair_eqpt.Is_Eq_Alive;
                }
                else
                {
                    node.Is_Alive = eqpt.Is_Eq_Alive;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }

        /// <summary>
        /// Does the initialize.
        /// </summary>
        public virtual void doInit()
        {
            try
            {
                ValueRead Door_Close_vr = null;
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "DOOR_CLOSE", out Door_Close_vr))
                {
                    Door_Close_vr.afterValueChange += (_sender, _e) => DoorCloseChange(_sender, _e);
                }
                ValueRead Safety_Check_Request_vr = null;
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "SAFETY_CHECK_REQUEST", out Safety_Check_Request_vr))
                {
                    Safety_Check_Request_vr.afterValueChange += (_sender, _e) => SafetyCheckRequestChange(_sender, _e);
                }
                ValueRead Safety_Check_Complete_vr = null;
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "SAFETY_CHECK_COMPLETE", out Safety_Check_Complete_vr))
                {
                    Safety_Check_Complete_vr.afterValueChange += (_sender, _e) => SafetyCheckCompleteChange(_sender, _e);
                }
                ValueRead OHCV_Alive_vr = null;
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "OHCV_TO_OHTC_ALIVE", out OHCV_Alive_vr))
                {
                    OHCV_Alive_vr.afterValueChange += (_sender, _e) => AliveChange(_sender, _e);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
            }

        }

    }
}
