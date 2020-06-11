//*********************************************************************************
//      UserBLL.cs
//*********************************************************************************
// File Name: UserBLL.cs
// Description: 業務邏輯：User、Group、Function Code
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2014/02/24    Hayes Chen     N/A            N/A     Initial Release
// 2016/01/02    Steven Hong    N/A            A0.01   Add User Group Dao and
//                                                     Adjust User Function to User Froup Function
// 2016/02/23    Steven Hong    N/A            A0.02   Add Query By Badge_Number
// 2016/03/30    Kevin Wei      N/A            A0.03   新增當User Group被刪除時，要將其User有於其相關的給清空。
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.DAO;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;
using com.mirle.ibg3k0.sc.Data;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class UserBLL
    {
        private SCApplication scApp = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private UserDao userDao = null;
        private FunctionCodeDao functionCodeDao = null;
        private UserFuncDao userFuncDao = null;
        private UserGroupDao userGroupDao = null;  //A0.02

        public UserBLL()
        {

        }

        public void start(SCApplication scApp)
        {
            this.scApp = scApp;
            userDao = scApp.UserDao;
            functionCodeDao = scApp.FunctionCodeDao;
            userFuncDao = scApp.UserFuncDao;
            userGroupDao = scApp.UserGroupDao;
        }

        public Boolean createUser(UASUSR user)
        {
            DBConnection_EF conn = null;
            try
            {
                if (user == null)
                {
                    return false;
                }
                conn = DBConnection_EF.GetContext();
                userDao.insertUser(conn, user);
            }
            catch (Exception ex)
            {
                logger.Warn("Insert Failed to UASUSR [user_id:{0}]", user.USER_ID, ex);
                if (conn != null)
                {
                    try
                    {
                        conn.Rollback();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Rollback Failed.", exception);
                    }
                }
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return true;
        }

        //A0.01 public Boolean createUser(string user_id, string user_name, string passwd, Boolean isDisable, Boolean isPowerUser) 
        public Boolean createUser(string user_id, string user_name, string passwd, Boolean isDisable, string user_grp, string badge_no, string department)  //A0.01
        {
            DBConnection_EF conn = null;
            try
            {
                UASUSR newUser = new UASUSR
                {
                    USER_ID = user_id.ToUpper(),
                    USER_NAME = user_name,
                    PASSWD = passwd,
                    DISABLE_FLG = (isDisable ? SCAppConstants.YES_FLAG : SCAppConstants.NO_FLAG),
                    //POWER_USER_FLG = (isPowerUser ? SCAppConstants.YES_FLAG : SCAppConstants.NO_FLAG),
                    POWER_USER_FLG = SCAppConstants.NO_FLAG,
                    USER_GRP = user_grp,  //A0.01
                    BADGE_NUMBER = badge_no,
                    //ADMIN_FLG = (isAdmin ? SCAppConstants.YES_FLAG : SCAppConstants.NO_FLAG),
                    ADMIN_FLG = SCAppConstants.NO_FLAG,
                    DEPARTMENT = department
                };

                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                userDao.insertUser(conn, newUser);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Insert Failed to UASUSR [user_id:{0}]", user_id, ex);
                if (conn != null)
                {
                    try
                    {
                        conn.Rollback();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Rollback Failed.", exception);
                    }
                }
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return true;
        }

        public Boolean updatePassword(string user_id, string passwd)
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                UASUSR updUser = userDao.getUser(conn, true, user_id);
                if (passwd != null && passwd.Trim().Length > 0)
                {
                    updUser.PASSWD = passwd;
                }
                userDao.updateUser(conn, updUser);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Update Failed to UASUSR [user_id:{0}]", user_id, ex);
                if (conn != null)
                {
                    try
                    {
                        conn.Rollback();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Rollback Failed.", exception);
                    }
                }
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return true;
        }

        //A0.01 public Boolean updateUser(string user_id, string user_name, string passwd, Boolean isDisable, Boolean isPowerUser) 
        public Boolean updateUser(string user_id, string user_name, string passwd, Boolean isDisable, string user_grp, string badgeNo, string department)   //A0.01
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                UASUSR updUser = userDao.getUser(conn, true, user_id);
                updUser.USER_NAME = user_name;
                if (passwd != null && passwd.Trim().Length > 0)
                {
                    updUser.PASSWD = passwd;
                }
                updUser.DISABLE_FLG = (isDisable ? SCAppConstants.YES_FLAG : SCAppConstants.NO_FLAG);
                //updUser.POWER_USER_FLG = (isPowerUser ? SCAppConstants.YES_FLAG : SCAppConstants.NO_FLAG);
                //updUser.ADMIN_FLG = (isAdmin ? SCAppConstants.YES_FLAG : SCAppConstants.NO_FLAG);
                updUser.USER_GRP = user_grp;
                updUser.BADGE_NUMBER = badgeNo;
                updUser.DEPARTMENT = department;
                userDao.updateUser(conn, updUser);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Update Failed to UASUSR [user_id:{0}]", user_id, ex);
                if (conn != null)
                {
                    try
                    {
                        conn.Rollback();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Rollback Failed.", exception);
                    }
                }
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return true;
        }

        //A0.02 Start
        public UASUSR getUserByBadge(string badge_no)
        {
            UASUSR rtnUser = new UASUSR();
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                rtnUser = userDao.getUserByBadge(conn, badge_no);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Load User Failed from UASUSR", ex);
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return rtnUser;
        }
        //A0.02 End

        public List<UASUSR> loadAllUser()
        {
            List<UASUSR> userList = new List<UASUSR>();
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                userList = userDao.loadAllUser(conn);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Load User Failed from UASUSR", ex);
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return userList;
        }

        public UASUSR getAdminUser()
        {
            UASUSR admin = null;
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                admin = userDao.getAdminUser(conn);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Get Admin User Failed from UASUSR", ex);
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return admin;
        }

        public Boolean deleteUser(string user_id)
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                UASUSR delUser = userDao.getUser(conn, true, user_id);
                if (!delUser.isAdmin())
                {
                    userDao.deleteUserByID(conn, user_id);
                }
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Delete Failed From UASUSR [user_id:{0}]", user_id, ex);
                if (conn != null)
                {
                    try
                    {
                        conn.Rollback();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Rollback Failed.", exception);
                    }
                }
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return true;
        }

        public Boolean createFunctionCode(UASFNC functionCode)
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                functionCodeDao.createFunctionCode(conn, functionCode);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Insert Failed to UASFNC [Func_Code:{0}]", functionCode.FUNC_CODE, ex);
                if (conn != null)
                {
                    try
                    {
                        conn.Rollback();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Rollback Failed.", exception);
                    }
                }
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return true;
        }

        public Boolean createFunctionCode(string func_code, string func_name)
        {
            UASFNC funcCode = new UASFNC();
            funcCode.FUNC_CODE = func_code;
            funcCode.FUNC_NAME = func_name;
            return createFunctionCode(funcCode);
        }

        public Boolean updateFunctionCodeByCode(string func_code, string func_name)
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();

                UASFNC funcCode = functionCodeDao.getFunctionCode(conn, true, func_code);
                funcCode.FUNC_NAME = func_name;
                functionCodeDao.updateUser(conn, funcCode);

                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Update Failed From UASFNC [func_code:{0}]", func_code, ex);
                if (conn != null)
                {
                    try
                    {
                        conn.Rollback();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Rollback Failed.", exception);
                    }
                }
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return true;
        }

        public Boolean deleteFunctionCodeByCode(string func_code)
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                functionCodeDao.deleteFunctionCodeByCode(conn, func_code);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Delete Failed from UASFNC [func_code:{0}]", func_code, ex);
                if (conn != null)
                {
                    try
                    {
                        conn.Rollback();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Rollback Failed.", exception);
                    }
                }
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return true;
        }

        public List<UASFNC> loadAllFunctionCode()
        {
            DBConnection_EF conn = null;
            List<UASFNC> rtnCodeList = new List<UASFNC>();
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                rtnCodeList = functionCodeDao.loadAllFunctionCode(conn);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Load Function Code Failed from UASFNC", ex);
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return rtnCodeList;
        }

        //A0.01 public Boolean createUserFunc(string user_id, string func_code) 
        public Boolean createUserFunc(string user_grp, string func_code)  //A).01
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();

                UASUFNC userFunc = new UASUFNC();
                //A0.01 userFunc.UserFuncPK.User_ID = user_id.Trim();
                userFunc.USER_GRP = user_grp.Trim();  //A0.01
                userFunc.FUNC_CODE = func_code.Trim();

                userFuncDao.createUserFunc(conn, userFunc);

                conn.Commit();
            }
            catch (Exception ex)
            {
                //A0.01 Start
                logger.Warn("Insert User Function Failed to UASUFNC [user_grp:{0}][func_code:{1}]",
                    user_grp, func_code, ex);
                //logger.Warn("Insert User Function Failed to UASUFNC [user_id:{0}][func_code:{1}]", 
                //    user_id, func_code, ex);
                //A0.01 End
                if (conn != null)
                {
                    try
                    {
                        conn.Rollback();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Rollback Failed.", exception);
                    }
                }
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return true;
        }

        //A0.01 public Boolean deleteUserFunc(string user_id, string func_code) 
        public Boolean deleteUserFunc(string user_grp, string func_code)  //A0.01
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                UASUFNC userFunc = new UASUFNC();
                //A0.01 userFunc.UserFuncPK.User_ID = user_id.Trim();
                userFunc.USER_GRP = user_grp.Trim();  //A0.01
                userFunc.FUNC_CODE = func_code.Trim();
                userFuncDao.deleteUserFunc(conn, userFunc);
                conn.Commit();
            }
            catch (Exception ex)
            {
                //A0.01 Start
                logger.Warn("Delete User Function Failed from UASUFNC [user_grp:{0}][func_code:{1}]",
                    user_grp, func_code, ex);
                //logger.Warn("Delete User Function Failed from UASUFNC [user_id:{0}][func_code:{1}]",
                //    user_id, func_code, ex);
                //A0.01 End
                if (conn != null)
                {
                    try
                    {
                        conn.Rollback();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Rollback Failed.", exception);
                    }
                }
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return true;
        }

        //A0.01 public Boolean registerUserFunc(string user_id, List<string> funcCodeList) 
        public Boolean registerUserFunc(string user_grp, List<string> funcCodeList)  //A0.01
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();

                userFuncDao.deleteUserFuncByUserGrp(conn, user_grp);

                foreach (string funcCode in funcCodeList)
                {
                    userFuncDao.createUserFunc(conn,
                        new UASUFNC
                        {
                            USER_GRP = user_grp,
                            FUNC_CODE = funcCode
                        });
                }
                conn.Commit();

            }
            catch (Exception ex)
            {
                logger.Warn("Register User Function Failed to UASUFNC [user_grp:{0}]",
                    user_grp, ex);
                if (conn != null)
                {
                    try
                    {
                        conn.Rollback();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Rollback Failed.", exception);
                    }
                }
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return true;
        }

        public bool IsGrpExist(string grp)
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                bool result = userGroupDao.IsUserGroupExist(conn, grp);
                return result;
            }
            catch
            {
                return false;
            }
        }

        public List<UASUFNC> loadAllUserFuncByUser()
        {
            List<UASUFNC> rtnList = new List<UASUFNC>();
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                rtnList = userFuncDao.loadAllUserFuncByUserGrp(conn);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Load User Function Failed from UASUFNC", ex);
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return rtnList;
        }
        public List<UASUFNC> loadUserFuncByUser(string user_grp)  //A0.01
        {
            List<UASUFNC> rtnList = new List<UASUFNC>();
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                //A0.01 rtnList = userFuncDao.loadUserFuncByUserID(conn, user_id);
                rtnList = userFuncDao.loadUserFuncByUserGrp(conn, user_grp);  //A0.01
                conn.Commit();
            }
            catch (Exception ex)
            {
                //A0.01 Start
                logger.Warn("Load User Function Failed from UASUFNC [user_grp:{0}]",
                    user_grp, ex);
                //logger.Warn("Load User Function Failed from UASUFNC [user_id:{0}]",
                //    user_id, ex);
                //A0.01 End
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return rtnList;
        }

        public Boolean checkUserPassword(string user_id, string password)
        {
            DBConnection_EF conn = null;
            Boolean result = false;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                UASUSR loginUser = userDao.getUser(conn, false, user_id);
                if (loginUser == null)
                {
                    result = false;
                }
                else if (SCUtility.isMatche(loginUser.PASSWD, password))
                {
                    result = true;
                }
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Load User Function Failed from UASUFNC [user_id:{0}]",
                    user_id, ex);
                result = false;
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return result;
        }

        public Boolean checkUserAuthority(string user_id, string function_code)
        {
            DBConnection_EF conn = null;
            Boolean result = true;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                UASUSR loginUser = userDao.getUser(conn, false, user_id);
                if (loginUser == null)
                {
                    result = false;
                }
                else if (loginUser.isPowerUser())
                {
                    result = true;
                }
                else
                {
                    //A0.01 UserFunc userFunc = userFuncDao.getUserFunc(conn, user_id, function_code);
                    UASUFNC userFunc = userFuncDao.getUserFunc(conn, loginUser.USER_GRP, function_code);
                    if (userFunc == null)
                    {
                        result = false;
                    }
                }
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Load User Function Failed from UASUFNC [user_id:{0}]",
                    user_id, ex);
                result = false;
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return result;
        }
        public Boolean checkUserEnable(string user_id)
        {
            DBConnection_EF conn = null;
            Boolean result = true;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                UASUSR loginUser = userDao.getUser(conn, false, user_id);
                if (loginUser == null)
                {
                    result = false;
                }
                else if (loginUser.isDisable())
                {
                    result = false;
                }
                else
                {
                    result = true;
                    //A0.01 UserFunc userFunc = userFuncDao.getUserFunc(conn, user_id, function_code);
                }
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Load User Function Failed from UASUFNC [user_id:{0}]",
                    user_id, ex);
                result = false;
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return result;
        }
        //A0.01 Start
        public List<UASUSRGRP> loadAllUserGroup()
        {
            DBConnection_EF conn = null;
            List<UASUSRGRP> userGrp = new List<UASUSRGRP>();
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                userGrp = userGroupDao.loadAllUserGroup(conn);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Load All User Group Function Failed from UASUFNC ", ex);
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return userGrp;
        }

        public Boolean addUserGroup(string user_grp)
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                UASUSRGRP userGrp = new UASUSRGRP();
                userGrp.USER_GRP = user_grp.Trim();
                userGroupDao.insertUserGroup(conn, userGrp);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Add User Group Failed from UASUFNC [user_grp:{0}]",
                    user_grp, ex);
                if (conn != null)
                {
                    try
                    {
                        conn.Rollback();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Rollback Failed.", exception);
                    }
                }
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return true;
        }

        public Boolean deleteUserGroup(string user_grp)
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                userGroupDao.deleteUserGroupByID(conn, user_grp);
                userGroupDao.updateUser_ClearGroupByGroupName(conn, user_grp); //A0.03
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Delete User Group Failed from UASUFNC [user_grp:{0}]", user_grp, ex);
                if (conn != null)
                {
                    try
                    {
                        conn.Rollback();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Rollback Failed.", exception);
                    }
                }
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Close Connection Failed.", exception);
                    }
                }
            }
            return true;
        }
        //A0.01 End
    }
}
