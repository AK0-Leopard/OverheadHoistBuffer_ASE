//*********************************************************************************
//      UserDao.cs
//*********************************************************************************
// File Name: UserDao.cs
// Description: User DAO
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2014/02/24    Hayes Chen     N/A            N/A     Initial Release
// 2016/02/23    Steven Hong    N/A            A0.01   Add Query By Badge_Number
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Data;
using NLog;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.App;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class UserDao : DaoBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public void insertUser(DBConnection_EF conn, UASUSR user) 
        {
            try
            {
                conn.UASUSR.Add(user);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw ;
            }
        }

        public void updateUser(DBConnection_EF conn, UASUSR user) 
        {
            try
            {
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw ;
            }
        }

        public UASUSR getUser(DBConnection_EF conn, Boolean readLock, string user_id) 
        {
            UASUSR rtnUser = null;
            try
            {
                var query = from user in conn.UASUSR
                            where user.USER_ID == user_id.Trim()
                            select user;
                rtnUser = query.SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw ;
            }
            return rtnUser;
        }

        //A0.01 Start
        public UASUSR getUserByBadge(DBConnection_EF conn, string badge_no)
        {
            UASUSR rtnUser = null;
            try
            {
                var query = from user in conn.UASUSR
                            where user.BADGE_NUMBER == badge_no.Trim()
                            select user;
                rtnUser = query.SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnUser;
        }
        //A0.01 End

        public UASUSR getAdminUser(DBConnection_EF conn) 
        {
            UASUSR rtnUser = null;
            try
            {
                var query = from user in conn.UASUSR
                            where user.ADMIN_FLG == SCAppConstants.YES_FLAG.Trim()
                            select user;
                rtnUser = query.SingleOrDefault();


            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnUser;
        }

        public List<UASUSR> loadAllUser(DBConnection_EF conn) 
        {
            List<UASUSR> userList = new List<UASUSR>();
            try
            {
                var query = from user in conn.UASUSR
                            select user;
                userList = query.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw ;
            }
            return userList;
        }



        public IQueryable getQueryAllSQL(DBConnection_EF con)
        {
            var query = from user in con.UASUSR
                        select user;
            return query;
        }



        public void deleteUserByID(DBConnection_EF conn, string user_id) 
        {
            try 
            {
                UASUSR rtnUser = null;
                var query = from user in conn.UASUSR
                            where user.ADMIN_FLG == SCAppConstants.YES_FLAG.Trim()
                            select user;
                conn.UASUSR.Remove(rtnUser);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw ;
            }
        }
    }
}
