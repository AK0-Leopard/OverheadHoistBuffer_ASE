using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using Nancy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using com.mirle.ibg3k0.sc.Data.VO;

namespace com.mirle.ibg3k0.sc.WebAPI
{
    public class UserControl : NancyModule
    {
        //SCApplication app = null;
        const string restfulContentType = "application/json; charset=utf-8";
        const string urlencodedContentType = "application/x-www-form-urlencoded; charset=utf-8";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public UserControl()
        {
            //app = SCApplication.getInstance();
            RegisterUserControlEvent();
            After += ctx => ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");

        }
        private void RegisterUserControlEvent()
        {
            Post["UserControl/LogIn"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                bool isSuccess = true;
                string result = string.Empty;

                string userID = Request.Query.userID.Value ?? Request.Form.userID.Value ?? string.Empty;
                string password = Request.Query.password.Value ?? Request.Form.password.Value ?? string.Empty;
                try
                {
                    Boolean loginSuccess = false;
                    Boolean hasAuth = false;
                    if(!SCUtility.isEmpty(userID))
                    {
                        loginSuccess = scApp.UserBLL.checkUserEnable(userID);
                    }
                    if (loginSuccess)
                    {
                        loginSuccess = scApp.UserBLL.checkUserPassword(userID, password);
                    }
                    if (loginSuccess)
                    {
                        hasAuth = scApp.UserBLL.checkUserAuthority(userID, SCAppConstants.FUNC_LOGIN);
                    }
                    if (!hasAuth)
                    {
                        result = "No authority!";
                    }
                    else
                    {
                        result = "OK";
                    }
                    //isSuccess = scApp.UserControlService.doChangeLinkStatus(linkStatus, out result);
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    result = "Execption happend!";
                    logger.Error(ex, "Execption:");
                }

                UserOperationLog userOperationLog = new UserOperationLog()
                {
                    Action = "LogIn",
                    ActionTime = DateTime.Now,
                    UserID = userID,
                };
                SCUtility.UserOperationLog(userOperationLog);

                var response = (Response)result;
                response.ContentType = restfulContentType;
                return response;
            };


            Post["UserControl/Exit"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                bool isSuccess = true;
                string result = string.Empty;

                string userID = Request.Query.userID.Value ?? Request.Form.userID.Value ?? string.Empty;
                string password = Request.Query.password.Value ?? Request.Form.password.Value ?? string.Empty;
                try
                {
                    Boolean loginSuccess = false;
                    Boolean hasAuth = false;
                    if (!SCUtility.isEmpty(userID))
                    {
                        loginSuccess = scApp.UserBLL.checkUserPassword(userID, password);
                    }
                    if (loginSuccess)
                    {
                        hasAuth = scApp.UserBLL.checkUserAuthority(userID, SCAppConstants.FUNC_CLOSE_SYSTEM);
                    }
                    if (!hasAuth)
                    {
                        result = "No authority!";
                    }
                    else
                    {
                        result = "OK";
                    }
                    //isSuccess = scApp.UserControlService.doChangeLinkStatus(linkStatus, out result);
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    result = "Execption happend!";
                    logger.Error(ex, "Execption:");
                }

                UserOperationLog userOperationLog = new UserOperationLog()
                {
                    Action = "LogOut",
                    ActionTime = DateTime.Now,
                    UserID = userID,
                };
                SCUtility.UserOperationLog(userOperationLog);

                var response = (Response)result;
                response.ContentType = restfulContentType;
                return response;
            };

            Post["UserControl/UpdatePassword"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                bool isSuccess = true;
                string result = string.Empty;

                string userID = Request.Query.userID.Value ?? Request.Form.userID.Value ?? string.Empty;
                string password_o = Request.Query.password_O.Value ?? Request.Form.password_O.Value ?? string.Empty;
                string password_n = Request.Query.password_N.Value ?? Request.Form.password_N.Value ?? string.Empty;
                string password_v = Request.Query.password_V.Value ?? Request.Form.password_V.Value ?? string.Empty;
                try
                {
                    Boolean loginSuccess = false;
                    if (!SCUtility.isEmpty(userID))
                    {
                        loginSuccess = scApp.UserBLL.checkUserPassword(userID, password_o);
                    }
                    else
                    {
                        result = "User ID can not be is empty!";
                    }
                    if (loginSuccess)
                    {
                        if (SCUtility.isEmpty(password_v) || SCUtility.isEmpty(password_n))
                        {
                            result = "New password is empty!";
                        }
                        else
                        {
                            if (!SCUtility.isMatche(password_v, password_n))
                            {
                                result = "New password is not match!";
                            }
                            else
                            {
                                bool pdSuccess = scApp.UserBLL.updatePassword(userID, password_n);
                                if (pdSuccess)
                                {
                                    result = "OK";
                                }
                                else
                                {
                                    result = "Password update Fail.";
                                }
                            }
                        }
                    }
                    else
                    {
                        result = "Old password is not correct!";

                    }
                    //isSuccess = scApp.UserControlService.doChangeLinkStatus(linkStatus, out result);
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


            Post["UserControl/UserAccountAdd"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                bool isSuccess = true;
                string result = string.Empty;

                string userID = Request.Query.userID.Value ?? Request.Form.userID.Value ?? string.Empty;
                string userName = Request.Query.userName.Value ?? Request.Form.userName.Value ?? string.Empty;
                string password = Request.Query.password.Value ?? Request.Form.password.Value ?? string.Empty;
                string isDisable = Request.Query.isDisable.Value ?? Request.Form.isDisable.Value ?? string.Empty;
                //string isPowerUser = Request.Query.isPowerUser.Value ?? Request.Form.isPowerUser.Value ?? string.Empty;
                string userGrp = Request.Query.userGrp.Value ?? Request.Form.userGrp.Value ?? string.Empty;
                string badgeNo = Request.Query.badgeNo.Value ?? Request.Form.badgeNo.Value ?? string.Empty;
                //string isAdmin = Request.Query.isAdmin.Value ?? Request.Form.isAdmin.Value ?? string.Empty;
                string department = Request.Query.department.Value ?? Request.Form.department.Value ?? string.Empty;
                bool bIsDisable = isDisable == true.ToString();
                //bool bIsPowerUser = isPowerUser == true.ToString();
                //bool bIsAdmin = isAdmin == true.ToString();
                try
                {
                    if (SCUtility.isEmpty(userID))  //A0.01
                    {
                        result = "User ID is empty!";
                    }
                    else if (SCUtility.isEmpty(userName))  //A0.01
                    {
                        result = "User name is empty!";
                    }
                    else if (SCUtility.isEmpty(userGrp))  //A0.01
                    {
                        result = "User Group is empty!";
                    }
                    else
                    {
                        Boolean createSuccess =
                        scApp.UserBLL.createUser(userID, userName, password, bIsDisable, userGrp, badgeNo, department);  //A0.01
                        if (createSuccess)
                        {
                            result = "OK";
                        }
                        else
                        {
                            result = "CREATE_FAILED";
                        }
                    }
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

            Post["UserControl/UserAccountUpdate"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                bool isSuccess = true;
                string result = string.Empty;

                string userID = Request.Query.userID.Value ?? Request.Form.userID.Value ?? string.Empty;
                string userName = Request.Query.userName.Value ?? Request.Form.userName.Value ?? string.Empty;
                string password = Request.Query.password.Value ?? Request.Form.password.Value ?? string.Empty;
                string isDisable = Request.Query.isDisable.Value ?? Request.Form.isDisable.Value ?? string.Empty;
                //string isPowerUser = Request.Query.isPowerUser.Value ?? Request.Form.isPowerUser.Value ?? string.Empty;
                string userGrp = Request.Query.userGrp.Value ?? Request.Form.userGrp.Value ?? string.Empty;
                string badgeNo = Request.Query.badgeNo.Value ?? Request.Form.badgeNo.Value ?? string.Empty;
                //string isAdmin = Request.Query.isAdmin.Value ?? Request.Form.isAdmin.Value ?? string.Empty;
                string department = Request.Query.department.Value ?? Request.Form.department.Value ?? string.Empty;
                bool bIsDisable = isDisable == true.ToString();
                //bool bIsPowerUser = isPowerUser == true.ToString();
                //bool bIsAdmin = isAdmin == true.ToString();
                try
                {
                    if (SCUtility.isEmpty(userID))  //A0.01
                    {
                        result = "User ID is empty!";
                    }
                    else if (SCUtility.isEmpty(userName))  //A0.01
                    {
                        result = "User name is empty!";
                    }
                    else if (SCUtility.isEmpty(userGrp))  //A0.01
                    {
                        result = "User Group is empty!";
                    }
                    else
                    {
                        Boolean updateSuccess =
                        scApp.UserBLL.updateUser(userID, userName, password, bIsDisable, userGrp, badgeNo, department);  //A0.01
                        if (updateSuccess)
                        {
                            result = "OK";
                        }
                        else
                        {
                            result = "UPDATE_FAILED";
                        }
                    }
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

            Post["UserControl/UserAccountDelete"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                bool isSuccess = true;
                string result = string.Empty;

                string userID = Request.Query.userID.Value ?? Request.Form.userID.Value ?? string.Empty;
                try
                {
                    Boolean deleteSuccess = scApp.UserBLL.deleteUser(userID);
                    if (deleteSuccess)
                    {
                        result = "OK";
                    }
                    else
                    {
                        result = "DELETE_FAILED";
                    }
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

            Post["UserControl/UserGroupAdd"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                bool isSuccess = true;
                string result = string.Empty;
                string resuleJson = string.Empty;

                using (Stream stream = Request.Body)
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("UTF-8")))
                    {
                        resuleJson = reader.ReadToEnd();
                    }
                }
                var fun_enable = JsonConvert.DeserializeObject<Dictionary<string, bool>>(resuleJson);
                string userGrp = fun_enable.FirstOrDefault().Key;
                //string userGrp = Request.Query.userGrp.Value ?? Request.Form.userGrp.Value ?? string.Empty;
                //string funcCloseSystem = Request.Query.funcCloseSystem.Value ?? Request.Form.funcCloseSystem.Value ?? string.Empty;
                //string funcSystemControlMode = Request.Query.funcSystemControlMode.Value ?? Request.Form.funcSystemControlMode.Value ?? string.Empty;
                //string funcLogin = Request.Query.funcLogin.Value ?? Request.Form.funcLogin.Value ?? string.Empty;
                //string funcAccountManagement = Request.Query.funcAccountManagement.Value ?? Request.Form.funcAccountManagement.Value ?? string.Empty;
                //string funcVehicleManagement = Request.Query.funcVehicleManagement.Value ?? Request.Form.funcVehicleManagement.Value ?? string.Empty;
                //string funcTransferManagement = Request.Query.funcTransferManagement.Value ?? Request.Form.funcTransferManagement.Value ?? string.Empty;
                //string funcMTLMTSMaintenance = Request.Query.funcMTLMTSMaintenance.Value ?? Request.Form.funcMTLMTSMaintenance.Value ?? string.Empty;
                //string funcPortMaintenance = Request.Query.funcPortMaintenance.Value ?? Request.Form.funcPortMaintenance.Value ?? string.Empty;
                //string funcDebug = Request.Query.funcDebug.Value ?? Request.Form.funcDebug.Value ?? string.Empty;
                //string funcAdvancedSettings = Request.Query.funcAdvancedSettings.Value ?? Request.Form.funcAdvancedSettings.Value ?? string.Empty;

                //bool bfuncCloseSystem = funcCloseSystem == true.ToString();
                //bool bfuncSystemControlMode = funcSystemControlMode == true.ToString();
                //bool bfuncLogin = funcLogin == true.ToString();
                //bool bfuncAccountManagement = funcAccountManagement == true.ToString();
                //bool bfuncVehicleManagement = funcVehicleManagement == true.ToString();
                //bool bfuncTransferManagement = funcTransferManagement == true.ToString();
                //bool bfuncMTLMTSMaintenance = funcMTLMTSMaintenance == true.ToString();
                //bool bfuncPortMaintenance = funcPortMaintenance == true.ToString();
                //bool bfunDebug = funcDebug == true.ToString();
                //bool bfunAdvancedSettings = funcAdvancedSettings == true.ToString();

                try
                {
                    if (SCUtility.isEmpty(userGrp))
                    {
                        result = "User Group is empty!";
                    }
                    else
                    {
                        Boolean createSuccess = scApp.UserBLL.addUserGroup(userGrp);

                        if (createSuccess)
                        {
                            List<string> functionList = new List<string>();
                            foreach (var item in fun_enable)
                            {
                                if (item.Value == true)
                                {
                                    functionList.Add(item.Key);
                                }
                            }

                            //if (bfuncCloseSystem) functionList.Add(SCAppConstants.FUNC_CLOSE_SYSTEM);
                            //if (bfuncSystemControlMode) functionList.Add(SCAppConstants.FUNC_SYSTEM_CONCROL_MODE);
                            //if (bfuncLogin) functionList.Add(SCAppConstants.FUNC_LOGIN);
                            //if (bfuncAccountManagement) functionList.Add(SCAppConstants.FUNC_ACCOUNT_MANAGEMENT);
                            //if (bfuncVehicleManagement) functionList.Add(SCAppConstants.FUNC_VEHICLE_MANAGEMENT);
                            //if (bfuncTransferManagement) functionList.Add(SCAppConstants.FUNC_TRANSFER_MANAGEMENT);
                            //if (bfuncMTLMTSMaintenance) functionList.Add(SCAppConstants.FUNC_MTS_MTL_MAINTENANCE);
                            //if (bfuncPortMaintenance) functionList.Add(SCAppConstants.FUNC_PORT_MAINTENANCE);
                            //if (bfunDebug) functionList.Add(SCAppConstants.FUNC_DEBUG);
                            //if (bfunAdvancedSettings) functionList.Add(SCAppConstants.FUNC_ADVANCED_SETTINGS);

                            createSuccess = scApp.UserBLL.registerUserFunc(userGrp, functionList);
                            if (createSuccess)
                            {
                                result = "OK";
                            }
                            else
                            {
                                result = "Create user function failed.";
                            }
                        }
                        else
                        {
                            result = "Create user group failed.";
                        }
                    }
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

            Post["UserControl/UserGroupUpdate"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                bool isSuccess = true;
                string result = string.Empty;
                string resuleJson = string.Empty;

                using (Stream stream = Request.Body)
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("UTF-8")))
                    {
                        resuleJson = reader.ReadToEnd();
                    }
                }

                var fun_enable = JsonConvert.DeserializeObject<Dictionary<string, bool>>(resuleJson);
                string userGrp = fun_enable.FirstOrDefault().Key;
                //string funcCloseSystem = Request.Query.funcCloseSystem.Value ?? Request.Form.funcCloseSystem.Value ?? string.Empty;
                //string funcSystemControlMode = Request.Query.funcSystemControlMode.Value ?? Request.Form.funcSystemControlMode.Value ?? string.Empty;
                //string funcLogin = Request.Query.funcLogin.Value ?? Request.Form.funcLogin.Value ?? string.Empty;
                //string funcAccountManagement = Request.Query.funcAccountManagement.Value ?? Request.Form.funcAccountManagement.Value ?? string.Empty;
                //string funcVehicleManagement = Request.Query.funcVehicleManagement.Value ?? Request.Form.funcVehicleManagement.Value ?? string.Empty;
                //string funcTransferManagement = Request.Query.funcTransferManagement.Value ?? Request.Form.funcTransferManagement.Value ?? string.Empty;
                //string funcMTLMTSMaintenance = Request.Query.funcMTLMTSMaintenance.Value ?? Request.Form.funcMTLMTSMaintenance.Value ?? string.Empty;
                //string funcPortMaintenance = Request.Query.funcPortMaintenance.Value ?? Request.Form.funcPortMaintenance.Value ?? string.Empty;
                //string funcDebug = Request.Query.funcDebug.Value ?? Request.Form.funcDebug.Value ?? string.Empty;
                //string funcAdvancedSettings = Request.Query.funcAdvancedSettings.Value ?? Request.Form.funcAdvancedSettings.Value ?? string.Empty;

                //bool bfuncCloseSystem = funcCloseSystem == true.ToString();
                //bool bfuncSystemControlMode = funcSystemControlMode == true.ToString();
                //bool bfuncLogin = funcLogin == true.ToString();
                //bool bfuncAccountManagement = funcAccountManagement == true.ToString();
                //bool bfuncVehicleManagement = funcVehicleManagement == true.ToString();
                //bool bfuncTransferManagement = funcTransferManagement == true.ToString();
                //bool bfuncMTLMTSMaintenance = funcMTLMTSMaintenance == true.ToString();
                //bool bfuncPortMaintenance = funcPortMaintenance == true.ToString();
                //bool bfunDebug = funcDebug == true.ToString();
                //bool bfunAdvancedSettings = funcAdvancedSettings == true.ToString();

                try
                {
                    if (SCUtility.isEmpty(userGrp))
                    {
                        result = "User Group is empty!";
                    }
                    else
                    {
                        List<string> functionList = new List<string>();
                        foreach (var item in fun_enable)
                        {
                            if(item.Value == true)
                            {
                                functionList.Add(item.Key);
                            }
                        }
                        //if (bfuncCloseSystem) functionList.Add(SCAppConstants.FUNC_CLOSE_SYSTEM);
                        //if (bfuncSystemControlMode) functionList.Add(SCAppConstants.FUNC_SYSTEM_CONCROL_MODE);
                        //if (bfuncLogin) functionList.Add(SCAppConstants.FUNC_LOGIN);
                        //if (bfuncAccountManagement) functionList.Add(SCAppConstants.FUNC_ACCOUNT_MANAGEMENT);
                        //if (bfuncVehicleManagement) functionList.Add(SCAppConstants.FUNC_VEHICLE_MANAGEMENT);
                        //if (bfuncTransferManagement) functionList.Add(SCAppConstants.FUNC_TRANSFER_MANAGEMENT);
                        //if (bfuncMTLMTSMaintenance) functionList.Add(SCAppConstants.FUNC_MTS_MTL_MAINTENANCE);
                        //if (bfuncPortMaintenance) functionList.Add(SCAppConstants.FUNC_PORT_MAINTENANCE);
                        //if (bfunDebug) functionList.Add(SCAppConstants.FUNC_DEBUG);
                        //if (bfunAdvancedSettings) functionList.Add(SCAppConstants.FUNC_ADVANCED_SETTINGS);

                        bool updateSuccess = false;
                        if (scApp.UserBLL.IsGrpExist(userGrp))
                        {
                            updateSuccess = scApp.UserBLL.registerUserFunc(userGrp, functionList);
                        }
                        //scApp.UserBLL.addUserGroup(userGrp);

                        if (updateSuccess)
                        {
                            result = "OK";
                        }
                        else
                        {
                            result = "UPDATE_FAILED";
                        }
                    }
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

            Post["UserControl/UserGroupDelete"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                bool isSuccess = true;
                string result = string.Empty;

                string userGrp = Request.Query.userGrp.Value ?? Request.Form.userGrp.Value ?? string.Empty;
                try
                {
                    Boolean deleteSuccess = scApp.UserBLL.deleteUserGroup(userGrp);
                    if (deleteSuccess)
                    {
                        result = "OK";
                    }
                    else
                    {
                        result = "DELETE_FAILED";
                    }
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

        }
    }
}
