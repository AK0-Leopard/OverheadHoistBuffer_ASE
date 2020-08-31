using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using Nancy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace com.mirle.ibg3k0.sc.WebAPI
{
	public class CSTBoxControl : NancyModule
	{
		private SCApplication app = null;
		private const string restfulContentType = "application/json; charset=utf-8";
		private const string urlencodedContentType = "application/x-www-form-urlencoded; charset=utf-8";
		private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

		public CSTBoxControl() : base("")
		{
			RegisterShelfManagementEvent();
			RegisterPortDefEvent();
			RegisterCassetteEvent();
			RegisterZoneDefEvent();
			After += ctx => ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
		}

		public void RegisterShelfManagementEvent()
		{
			Post["ShelfDef/{Action}/{UserID}"] = (p) =>
			{
				var scApp = SCApplication.getInstance();
				bool isSuccess = true;
				bool allSuccess = true;
				string result = string.Empty;
				string action = p.Action.Value;
				string userid = p.UserID.Value;
				string resultJson = string.Empty;
				using (Stream stream = Request.Body)
				{
					using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("UTF-8")))
					{
						resultJson = reader.ReadToEnd();
					}
				}
				dynamic d = JsonConvert.DeserializeObject(resultJson);
				string shelf_id = d.shelf_id?.Value;
				UserOperationLog userOperationLog = new UserOperationLog()
				{
					Action = action,
					UserID = userid,
					ActionTime = DateTime.Now,
				};
				try
				{
					switch (action)
					{
						case "EnableUpdate":
							bool enable = d.enable?.Value;
							string s = scApp.TransferService.Manual_ShelfEnable(shelf_id, enable);
							isSuccess = s == "OK" ? true : false;
							result = isSuccess ? "OK" : "Update Shlef Enable failed.";
							break;

						case "PriorityUpdate":
							int priority = (int)d.priority?.Value;
							isSuccess = scApp.ShelfService.doUpdatePriority(shelf_id, priority);
							result = isSuccess ? "OK" : "Update Shlef Priority failed.";
							break;

						case "MultiEnableUpdate":
							var enableJson = JsonConvert.DeserializeObject<List<ShelfDef>>(resultJson);
							result = "Update these Shlef Enable failed.\n";
							enableJson.ForEach(x =>
							{
								string enableStr = scApp.TransferService.Manual_ShelfEnable(x.ShelfID, true);
								isSuccess = enableStr == "OK" ? true : false;

								if (!isSuccess)
								{
									result += $"{x.ShelfID}   ";
									allSuccess = false;
								}
							});
							if (allSuccess == true)
							{
								result = "OK";
							}
							break;

						case "MultiDisableUpdate":
							var disableJson = JsonConvert.DeserializeObject<List<ShelfDef>>(resultJson);
							result = "Update these Shlef Disable failed.\n";
							disableJson.ForEach(x =>
							{
								string ss = scApp.TransferService.Manual_ShelfEnable(x.ShelfID, false);
								isSuccess = ss == "OK" ? true : false;

								if (!isSuccess)
								{
									result += $"{x.ShelfID}   ";
									allSuccess = false;
								}
							});
							if (allSuccess == true)
							{
								result = "OK";
							}
							break;

						case "StateUpdate":
							string state = d.state?.Value;
							isSuccess = scApp.ShelfService.doUpdateState(shelf_id, state);
							result = isSuccess ? "OK" : "Update Shlef State failed.";
							break;

						case "RemarkUpdate":
							string remark = d.remark?.Value;
							isSuccess = scApp.ShelfService.doUpdateRemark(shelf_id, remark);
							result = isSuccess ? "OK" : "Update Shlef Remark failed.";
							break;
					}
				}
				catch (Exception ex)
				{
					isSuccess = false;
					result = "Execption happend!";
					logger.Error(ex, "Execption:");
				}
				SCUtility.UserOperationLog(userOperationLog);
				var response = (Response)result;
				response.ContentType = restfulContentType;
				return response;
			};
		}

		private void RegisterCassetteEvent()
		{
			Post["CassetteData/{Action}/{UserID}"] = (p) =>
			{
				var scApp = SCApplication.getInstance();
				bool isSuccess = true;
				bool allSuccess = true;
				string result = string.Empty;
				string action = p.Action.Value;
				string userid = p.UserID.Value;
				string resultJson = string.Empty;
				using (Stream stream = Request.Body)
				{
					using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("UTF-8")))
					{
						resultJson = reader.ReadToEnd();
					}
				}
				dynamic d = JsonConvert.DeserializeObject(resultJson);
				string box_id = d.box_id?.Value;
				string cst_id = d.cst_id?.Value;
				string cst_loc = d.cst_loc?.Value;
				UserOperationLog userOperationLog = new UserOperationLog()
				{
					Action = action,
					UserID = userid,
					OldCarrierID = cst_id ?? string.Empty,
					CarrierLoc = cst_loc ?? string.Empty,
					BOXID = box_id ?? string.Empty,
					ActionTime = DateTime.Now,
				};
				try
				{
					switch (action)
					{
						case "StateUpdate":
							int state = (int)d.state?.Value;
							isSuccess = scApp.CassetteDataBLL.UpdateCSTState(box_id, state);
							result = isSuccess ? "OK" : "Update Cassette State failed.";
							break;

						case "LocationUpdate":
							string s = scApp.TransferService.Manual_DeleteCst(cst_id, box_id);
							string ss = scApp.TransferService.Manual_InsertCassette(cst_id, box_id, cst_loc);
							if (s != "OK" || ss != "OK")
							{
								isSuccess = false;
							}
							result = isSuccess ? "OK" : "Update Cassette Location failed.";

							break;

						case "InstallCST_BOX":
							CassetteData cassette = scApp.CassetteDataBLL.loadCassetteDataByCSTID(cst_id);
							result = scApp.TransferService.Manual_InsertCassette(cst_id, box_id, cst_loc);
							break;

						case "RemoveCST_BOX":
							string sss = scApp.TransferService.Manual_DeleteCst(cst_id, box_id);
							isSuccess = sss == "OK" ? true : false;
							result = isSuccess ? "OK" : "Remove Cassette failed.";
							break;

						case "ScanCST_BOX":
							string ssss = scApp.TransferService.SetScanCmd(cst_id, box_id, cst_loc);
							isSuccess = ssss == "OK" ? true : false;
							result = isSuccess ? "OK" : "Remove Cassette failed.";
							break;
					};
				}
				catch (Exception ex)
				{
					isSuccess = false;
					result = "Execption happend!";
					logger.Error(ex, "Execption:");
				}

				SCUtility.UserOperationLog(userOperationLog);
				var response = (Response)result;
				response.ContentType = restfulContentType;
				return response;
			};
		}

		private void RegisterPortDefEvent()
		{
			Post["PortDef/{Action}/{UserID}"] = (p) =>
			{
				var scApp = SCApplication.getInstance();
				bool isSuccess = true;
				bool allSuccess = true;
				string result = string.Empty;
				string action = p.Action.Value;
				string userid = p.UserID.Value;
				string resultJson = string.Empty;
				string returnStr = string.Empty;
				using (Stream stream = Request.Body)
				{
					using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("UTF-8")))
					{
						resultJson = reader.ReadToEnd();
					}
				}
				dynamic d = JsonConvert.DeserializeObject(resultJson);
				string port_id = d.port_id?.Value;
				UserOperationLog userOperationLog = new UserOperationLog()
				{
					Action = action,
					UserID = userid,
					ActionTime = DateTime.Now,
				};
				try
				{
					switch (action)
					{
						case "PriorityUpdate":
							int priority = (int)d.priority?.Value;
							returnStr = scApp.TransferService.Manual_SetPortPriority(port_id, priority);
							isSuccess = returnStr == "OK" ? true : false;
							result = isSuccess ? "OK" : "Update Port Priority failed.";
							break;

						case "PortTypeUpdate":
							int porttype = (int)d.porttype?.Value;
							returnStr = scApp.TransferService.Manual_PortTypeChange(port_id, (E_PortType)porttype);
							isSuccess = returnStr == "OK" ? true : false;
							result = isSuccess ? "OK" : "Update Port Type failed.";
							break;

						case "PortService":
							int service = (int)d.service?.Value;
							returnStr = scApp.TransferService.Manual_SetPortStatus(port_id, (E_PORT_STATUS)service);
							isSuccess = returnStr == "OK" ? true : false;
							result = isSuccess ? "OK" : "Update Port Serveice failed.";
							break;

						case "PortRun":
							isSuccess = scApp.TransferService.SetPortRun(port_id);
							result = isSuccess ? "OK" : "Set Port Run failed.";
							break;

						case "PortStop":
							isSuccess = scApp.TransferService.SetPortStop(port_id);
							result = isSuccess ? "OK" : "Set Port Stop failed.";
							break;

						case "PortAlarrmReset":
							isSuccess = scApp.TransferService.PortAlarrmReset(port_id);
							result = isSuccess ? "OK" : "Port Alarrm Reset failed.";
							break;

						case "PortCommandingTrue":
							isSuccess = scApp.TransferService.PortCommanding(port_id, true);
							result = isSuccess ? "OK" : "Set Port Commanding failed.";
							break;

						case "PortCommandingFalse":
							isSuccess = scApp.TransferService.PortCommanding(port_id, false);
							result = isSuccess ? "OK" : "Cancel Port Commanding failed.";
							break;

						case "toAGVMode":
							isSuccess = scApp.TransferService.toAGV_Mode(port_id);
							result = isSuccess ? "OK" : "Set AGV Mode failed.";
							break;

						case "toMGVMode":
							isSuccess = scApp.TransferService.toMGV_Mode(port_id);
							result = isSuccess ? "OK" : "Set MGV Mode failed.";
							break;

						case "setAGVBcrRead":
							isSuccess = scApp.TransferService.SetAGV_PortBCR_Read(port_id);
							result = isSuccess ? "OK" : "Set AGV BCR Read failed.";
							break;

						case "resetAGVBcrRead":
							isSuccess = scApp.TransferService.RstAGV_PortBCR_Read(port_id);
							result = isSuccess ? "OK" : "Reset AGV BCR Read failed.";
							break;

						case "rereadBCR":
							isSuccess = true; // todo Reread BCR
							result = isSuccess ? "OK" : "Reread BCR failed.";
							break;

						case "AGVOpenBox":
							isSuccess = scApp.TransferService.SetAGV_PortOpenBOX(port_id, "CSTBoxControl");
							result = isSuccess ? "OK" : "Open AGV Box failed.";
							break;

						case "TimeOutForAutoUDUpdate":
							int timeOutForAutoUD = (int)d.timeOutForAutoUD?.Value;
							isSuccess = scApp.TransferService.doUpdateTimeOutForAutoUD(port_id, timeOutForAutoUD);
							result = isSuccess ? "OK" : "Update Port TimeOutForAutoUD failed.";
							break;

						case "TimeOutForAutoInZoneUpdate":
							string timeOutForAutoInZone = d.timeOutForAutoInZone?.Value;
							isSuccess = scApp.TransferService.doUpdateTimeOutForAutoInZone(port_id, timeOutForAutoInZone);
							result = isSuccess ? "OK" : "Update Port TimeOutForAutoInZone failed.";
							break;

						case "BlockPort":
                            isSuccess = scApp.TransferService.UpdateIgnoreModeChange(port_id, "Y");
                            result = isSuccess ? "OK" : "Block Port failed.";
							break;

						case "UnblockPort":
                            isSuccess = scApp.TransferService.UpdateIgnoreModeChange(port_id, "N");
                            result = isSuccess ? "OK" : "Unblock Port failed.";
							break;

						case "ClearAlarmPort":
                            isSuccess = scApp.TransferService.PortAlarrmReset(port_id);
                            result = isSuccess ? "OK" : "Clear Alarm Port failed.";
							break;
					}
				}
				catch (Exception ex)
				{
					isSuccess = false;
					result = "Execption happend!";
					logger.Error(ex, "Execption:");
				}
				SCUtility.UserOperationLog(userOperationLog);
				var response = (Response)result;
				response.ContentType = restfulContentType;
				return response;
			};
		}

		private void RegisterZoneDefEvent()
		{
			Post["ZoneDef/{Action}/{UserID}"] = (p) =>
			{
				var scApp = SCApplication.getInstance();
				bool isSuccess = true;
				bool allSuccess = true;
				string result = string.Empty;
				string action = p.Action.Value;
				string userid = p.UserID.Value;
				string resultJson = string.Empty;
				string returnStr = string.Empty;
				using (Stream stream = Request.Body)
				{
					using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("UTF-8")))
					{
						resultJson = reader.ReadToEnd();
					}
				}
				dynamic d = JsonConvert.DeserializeObject(resultJson);
				string zone_id = d.zone_id?.Value;
				UserOperationLog userOperationLog = new UserOperationLog()
				{
					Action = action,
					UserID = userid,
					ActionTime = DateTime.Now,
				};
				try
				{
					switch (action)
					{
						case "LowWaterUpdate":
							int lowwater = (int)d.water?.Value;
							isSuccess = scApp.ZoneDefBLL.updateLowWater(zone_id, lowwater);
							result = isSuccess ? "OK" : "Update Zone LowWater failed.";
							break;

						case "HighWaterUpdate":
							int highwater = (int)d.water?.Value;
							isSuccess = scApp.ZoneDefBLL.updateHighWater(zone_id, highwater);
							result = isSuccess ? "OK" : "Update Zone HighWater failed.";
							break;

						case "ColorUpdate":
							string color = d.color?.Value;
							isSuccess = scApp.ZoneDefBLL.updateColor(zone_id, color);
							result = isSuccess ? "OK" : "Update Zone Color failed.";
							break;
					}
				}
				catch (Exception ex)
				{
					isSuccess = false;
					result = "Execption happend!";
					logger.Error(ex, "Execption:");
				}
				SCUtility.UserOperationLog(userOperationLog);
				var response = (Response)result;
				response.ContentType = restfulContentType;
				return response;
			};
		}
	}
}
