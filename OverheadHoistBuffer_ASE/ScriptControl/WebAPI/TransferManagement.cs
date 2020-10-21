using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using Nancy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.WebAPI
{
    public class TransferManagement : NancyModule
    {
        //SCApplication app = null;
        const string restfulContentType = "application/json; charset=utf-8";
        const string urlencodedContentType = "application/x-www-form-urlencoded; charset=utf-8";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        Timer ThreadTimer = null;
        public TransferManagement()
        {
            //app = SCApplication.getInstance();
            RegisterTransferManagementEvent();
            After += ctx => ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");

        }

        private void RegisterTransferManagementEvent()
        {

            Post["TransferManagement/MCSQueueSwitch"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                string result = string.Empty;

                try
                {
                    string AutoAssign = Request.Query.AutoAssign.Value ?? Request.Form.AutoAssign.Value ?? string.Empty;
                    bool isAutoAssign = Convert.ToBoolean(AutoAssign);
                    scApp.getEQObjCacheManager().getLine().MCSCommandAutoAssign = isAutoAssign;
                    result = "OK";
                }
                catch (Exception ex)
                {
                    result = "MCS Queue Switch update failed with exception happened";
                }

                var response = (Response)result;
                response.ContentType = restfulContentType;
                return response;
            };

            Post["TransferManagement/CancelAbort"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                bool isSuccess = true;
                string result = string.Empty;

                CMDCancelType cnacel_type = default(CMDCancelType);
                string mcs_cmd_id = Request.Query.mcs_cmd.Value ?? Request.Form.mcs_cmd.Value ?? string.Empty;
                try
                {
                    ACMD_MCS mcs_cmd = scApp.CMDBLL.getCMD_MCSByID(mcs_cmd_id);

                    scApp.TransferService.Manual_DeleteCmd(mcs_cmd_id, "正式 UI");

                    UserOperationLog userOperationLog = new UserOperationLog()
                    {
                        Action = "TransferCancelAbort",
                        ActionTime = DateTime.Now,
                        CommandID = mcs_cmd_id,
                    };
                    SCUtility.UserOperationLog(userOperationLog);
                    //if (mcs_cmd == null)
                    //{
                    //    result = $"Can not find transfer command:[{mcs_cmd_id}].";
                    //}
                    //else
                    //{
                    //    if (mcs_cmd.TRANSFERSTATE < sc.E_TRAN_STATUS.Transferring)
                    //    {
                    //        cnacel_type = CMDCancelType.CmdCancel;
                    //        bool btemp = scApp.VehicleService.doCancelOrAbortCommandByMCSCmdID(mcs_cmd_id, cnacel_type);
                    //        if (btemp)
                    //        {
                    //            result = "OK";
                    //        }
                    //        else
                    //        {
                    //            result = $"Transfer command:[{mcs_cmd_id}] cancel failed.";
                    //        }
                    //    }
                    //    else if (mcs_cmd.TRANSFERSTATE < sc.E_TRAN_STATUS.Canceling)
                    //    {
                    //        cnacel_type = CMDCancelType.CmdAbort;
                    //        bool btemp = scApp.VehicleService.doCancelOrAbortCommandByMCSCmdID(mcs_cmd_id, cnacel_type);
                    //        if (btemp)
                    //        {
                    //            result = "OK";
                    //        }
                    //        else
                    //        {
                    //            result = $"Transfer command:[{mcs_cmd_id}] adort failed.";
                    //        }
                    //    }
                    //    else
                    //    {
                    //        result = $"Command ID:{mcs_cmd.CMD_ID.Trim()} can't excute cancel / abort,\r\ncurrent state:{mcs_cmd.TRANSFERSTATE}";
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    result = "Execption happend!";
                    logger.Error(ex, "Execption:");
                }
                var response = (Response)result;
                response.ContentType = restfulContentType;
                return response;
            };


            Post["TransferManagement/ForceFinish"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                bool isSuccess = true;
                string result = string.Empty;
                string mcs_cmd_id = Request.Query.mcs_cmd.Value ?? Request.Form.mcs_cmd.Value ?? string.Empty;
                AVEHICLE excute_cmd_of_vh = scApp.VehicleBLL.cache.getVehicleByMCSCmdID(mcs_cmd_id);
                ACMD_MCS mcs_cmd = scApp.CMDBLL.getCMD_MCSByID(mcs_cmd_id);
                try
                {
                    if (excute_cmd_of_vh != null)
                    {
                        scApp.VehicleBLL.doTransferCommandFinish(excute_cmd_of_vh.VEHICLE_ID, excute_cmd_of_vh.OHTC_CMD, CompleteStatus.CmpStatusForceFinishByOp);
                        scApp.VIDBLL.initialVIDCommandInfo(excute_cmd_of_vh.VEHICLE_ID);
                    }
                    //scApp.CMDBLL.updateCMD_MCS_TranStatus2Complete(mcs_cmd.CMD_ID, E_TRAN_STATUS.Aborting);
                    scApp.ReportBLL.newReportTransferCommandNormalFinish(mcs_cmd, excute_cmd_of_vh, sc.Data.SECS.CSOT.SECSConst.CMD_Result_Unsuccessful, null);
                    result = "OK";
                }
                catch
                {
                    result = "ForceFinish failed.";
                }

                UserOperationLog userOperationLog = new UserOperationLog()
                {
                    Action = "TransferForceFinish",
                    ActionTime = DateTime.Now,
                    CommandID = mcs_cmd_id,
                };
                SCUtility.UserOperationLog(userOperationLog);

                var response = (Response)result;
                response.ContentType = restfulContentType;
                return response;
            };

            Post["TransferManagement/AssignVehicle"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                string result = string.Empty;

                string mcs_cmd_id = Request.Query.mcs_cmd.Value ?? Request.Form.mcs_cmd.Value ?? string.Empty;
                string vh_id = Request.Query.vh_id.Value ?? Request.Form.vh_id.Value ?? string.Empty;
                try
                {
                    ACMD_MCS mcs_cmd = scApp.CMDBLL.getCMD_MCSByID(mcs_cmd_id);
                    scApp.CMDBLL.assignCommnadToVehicle(mcs_cmd_id, vh_id, out result);
                }
                catch (Exception ex)
                {
                    result = "Assign command to  vehicle failed with exception happened.";
                }

                UserOperationLog userOperationLog = new UserOperationLog()
                {
                    Action = "TransferAssignVehicle",
                    ActionTime = DateTime.Now,
                    CommandID = mcs_cmd_id,
                };
                SCUtility.UserOperationLog(userOperationLog);

                var response = (Response)result;
                response.ContentType = restfulContentType;
                return response;
            };
            Post["TransferManagement/ShiftCommand"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                string result = string.Empty;

                string mcs_cmd_id = Request.Query.mcs_cmd.Value ?? Request.Form.mcs_cmd.Value ?? string.Empty;
                string vh_id = Request.Query.vh_id.Value ?? Request.Form.vh_id.Value ?? string.Empty;

                try
                {
                    ACMD_MCS mcs_cmd = scApp.CMDBLL.getCMD_MCSByID(mcs_cmd_id);
                    scApp.CMDBLL.commandShift(mcs_cmd_id, vh_id, out result);
                }
                catch (Exception ex)
                {
                    result = "Shift command failed with exception happened.";
                }

                UserOperationLog userOperationLog = new UserOperationLog()
                {
                    Action = "TransferShiftCommand",
                    ActionTime = DateTime.Now,
                    CommandID = mcs_cmd_id,
                };
                SCUtility.UserOperationLog(userOperationLog);

                var response = (Response)result;
                response.ContentType = restfulContentType;
                return response;
            };

            Post["TransferManagement/ChangeStatus"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                string result = string.Empty;
                bool isSuccess = true;
                string mcs_cmd_id = Request.Query.mcs_cmd.Value ?? Request.Form.mcs_cmd.Value ?? string.Empty;
                string sstatus = Request.Query.status.Value ?? Request.Form.status.Value ?? string.Empty;
                try
                {
                    ACMD_MCS mcs_cmd = scApp.CMDBLL.getCMD_MCSByID(mcs_cmd_id);
                    E_TRAN_STATUS status = (E_TRAN_STATUS)Enum.Parse(typeof(E_TRAN_STATUS), sstatus, false);

                    if (mcs_cmd != null)
                    {
                        isSuccess = scApp.CMDBLL.updateCMD_MCS_TranStatus(mcs_cmd_id, status);
                        if (isSuccess)
                        {
                            result = "OK";
                        }
                        else
                        {
                            result = "Update status failed.";
                        }
                    }
                    else
                    {
                        result = $"Can not find MCS Command[{mcs_cmd_id}].";
                    }
                }
                catch (Exception ex)
                {
                    result = "Update status failed with exception happened.";
                }

                UserOperationLog userOperationLog = new UserOperationLog()
                {
                    Action = "TransferChangeStatus",
                    ActionTime = DateTime.Now,
                    CommandID = mcs_cmd_id,
                };
                SCUtility.UserOperationLog(userOperationLog);

                //Todo by Mark

                var response = (Response)result;
                response.ContentType = restfulContentType;
                return response;
            };

            Post["TransferManagement/Priority"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                string result = string.Empty;
                bool isSuccess = true;
                string mcs_cmd_id = Request.Query.mcs_cmd.Value ?? Request.Form.mcs_cmd.Value ?? string.Empty;
                string priority = Request.Query.priority.Value ?? Request.Form.priority.Value ?? string.Empty;
                try
                {
                    ACMD_MCS mcs_cmd = scApp.CMDBLL.getCMD_MCSByID(mcs_cmd_id);
                    int iPriority = Convert.ToInt32(priority);
                    if (mcs_cmd != null)
                    {
                        //isSuccess = scApp.CMDBLL.updateCMD_MCS_PrioritySUM(mcs_cmd, iPriority);
                        isSuccess = scApp.CMDBLL.updateCMD_MCS_PortPriority(mcs_cmd.CMD_ID, iPriority);
                        if (isSuccess)
                        {
                            result = "OK";
                        }
                        else
                        {
                            result = "Update priority failed.";
                        }
                    }
                    else
                    {
                        result = $"Can not find MCS Command[{mcs_cmd_id}].";
                    }
                }
                catch (Exception ex)
                {
                    result = "Update priority failed with exception happened.";
                }

                UserOperationLog userOperationLog = new UserOperationLog()
                {
                    Action = "TransferPriority",
                    ActionTime = DateTime.Now,
                    CommandID = mcs_cmd_id,
                };
                SCUtility.UserOperationLog(userOperationLog);

                var response = (Response)result;
                response.ContentType = restfulContentType;
                return response;
            };

            Post["TransferManagement/TransferCreate"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                string result = string.Empty;
                bool isSuccess = true;

                int Y = DateTime.Now.Year % 100;
                string stDate = string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", Y, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                string mcs_cmd_id = "MANAUL-" + stDate;//Request.Query.mcs_cmd.Value ?? Request.Form.mcs_cmd.Value ?? string.Empty;

                string priority = Request.Query.priority.Value ?? Request.Form.priority.Value ?? string.Empty;

                string box_id = Request.Query.box_id.Value ?? Request.Form.box_id.Value ?? string.Empty;
                string cst_id = Request.Query.cst_id.Value ?? Request.Form.cst_id.Value ?? string.Empty;
                string source = Request.Query.source.Value ?? Request.Form.source.Value ?? string.Empty;
                string dest = Request.Query.dest.Value ?? Request.Form.dest.Value ?? string.Empty;
                string lot_id = Request.Query.lot_id.Value ?? Request.Form.lot_id.Value ?? string.Empty;
                result = scApp.TransferService.Manual_InsertCmd(source, dest, 5);

                UserOperationLog userOperationLog = new UserOperationLog()
                {
                    Action = "TransferCreate",
                    ActionTime = DateTime.Now,
                    CommandID = mcs_cmd_id,
                    Source = source,
                    Dest = dest,
                    BOXID = box_id,
                };
                SCUtility.UserOperationLog(userOperationLog);

                var response = (Response)result;
                response.ContentType = restfulContentType;
                return response;
            };

            Get["TransferManagement/TransferCheck/AGVStation/{AGVStationID}"] = (p) =>
            {
                SCApplication scApp = SCApplication.getInstance();
                //string queue_count = 0;
                scApp.TransferService.swapTriggerWaitin = false;
                string agv_station_id = p.AGVStationID;
                string excute_count = Request.Query.unfinishCmdCount.Value ?? Request.Form.unfinishCmdCount.Value ?? string.Empty;
                string is_emergency = Request.Query.isEmergency.Value ?? Request.Form.isEmergency.Value ?? string.Empty;
                bool emergency = false;
                if (is_emergency == "true")
                {
                    emergency = true;
                }
                else if (is_emergency == "false")
                {
                    emergency = false;
                }
                // Change the default return from false to true. 
                // 因為目前回復NG時會產生AGV走行命令回到AGV Station，但目前預設值設定為false 因避免exception情形(AGVC Cmd == 0 時回復True 不會停止觸發OHBC)
                bool is_ok = false;
                //todo 執行確認能否讓AGVC開始進行該AGV Station進貨的流程
                bool check_method = scApp.TransferService.oneInoneOutMethodUse;
                if (check_method)
                {
                    is_ok = scApp.TransferService.CanExcuteUnloadTransferAGVStationFromAGVC_OneInOneOut(agv_station_id.Trim(), Int32.Parse(excute_count), emergency);
                }
                else
                {
                    is_ok = scApp.TransferService.CanExcuteUnloadTransferAGVStationFromAGVC(agv_station_id.Trim(), Int32.Parse(excute_count), emergency);
                }
                var response = (Response)(is_ok ? "OK" : "NG");
                response.ContentType = restfulContentType;

                return response;
            };

            Get["TransferManagement/TransferCheck/Swap/AGVStation/{AGVStationID}"] = (p) =>
            {
                SCApplication scApp = SCApplication.getInstance();
                //string queue_count = 0;
                scApp.TransferService.swapTriggerWaitin = true;
                string agv_station_id = p.AGVStationID;
                string excute_count = Request.Query.unfinishCmdCount.Value ?? Request.Form.unfinishCmdCount.Value ?? string.Empty;
                string is_emergency = Request.Query.isEmergency.Value ?? Request.Form.isEmergency.Value ?? string.Empty;
                bool emergency = false;
                is_emergency = is_emergency.ToUpper();
                if (is_emergency.Contains("T"))
                {
                    emergency = true;
                }
                else if (is_emergency.Contains("F"))
                {
                    emergency = false;
                }
                // Change the default return from false to true. 
                // 因為目前回復NG時會產生AGV走行命令回到AGV Station，但目前預設值設定為false 因避免exception情形(AGVC Cmd == 0 時回復True 不會停止觸發OHBC)
                bool is_ok = false;
                bool is_more_out = false;

                //todo 執行確認能否讓AGVC開始進行該AGV Station進貨的流程
                (is_ok, is_more_out) = scApp.TransferService.CanExcuteUnloadTransferAGVStationFromAGVC_Swap(agv_station_id.Trim(), Int32.Parse(excute_count), emergency);

                string check_result = is_ok ? "OK" : "NG";
                E_AGVStationTranMode tran_mode = is_more_out ? E_AGVStationTranMode.MoreOut : E_AGVStationTranMode.MoreIn;

                int s_tran_mode = (int)tran_mode;
                //var response = (Response)(is_ok ? "OK" : "NG");
                var response = (Response)($"{check_result},{s_tran_mode}");
                response.ContentType = restfulContentType;

                return response;
            };

            Get["TransferManagement/PreOpenAGVStationCover/AGVStationPorts/{AGVStationPortID}"] = (p) =>
            {
                SCApplication scApp = SCApplication.getInstance();
                string agv_station_port_id = p.AGVStationPortID;

                bool is_ok = true;
                is_ok = scApp.TransferService.SetAGV_PortOpenBOX(agv_station_port_id, "TransferManagement");

                var response = (Response)(is_ok ? "OK" : "NG");
                response.ContentType = restfulContentType;

                return response;
            };
        }
        public enum E_AGVStationTranMode
        {
            MoreIn = 1,
            MoreOut = 2
        }

    }
}
