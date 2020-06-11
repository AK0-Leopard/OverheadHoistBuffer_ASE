//*********************************************************************************
//      UserGroupDao.cs
//*********************************************************************************
// File Name: UserGroupDao.cs
// Description: User Group DAO
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2015/01/02    Steven Hong    N/A            N/A     Initial Release
// 2016/03/02    Kevin Wei      N/A            A0.01   增加Fun:updateUser_ClearGroupByGroupName
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
    public class UserGroupDao : DaoBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public void insertUserGroup(DBConnection_EF conn, UASUSRGRP userGrp)
        {
            try
            {
                conn.UASUSRGRP.Add(userGrp);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public bool IsUserGroupExist(DBConnection_EF conn, string userGrp)
        {
            try
            {
                var result = conn.UASUSRGRP.Where(x => x.USER_GRP == userGrp).FirstOrDefault();
                return result != null;
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public void updateUserGroup(DBConnection_EF conn, UASUSRGRP userGrp)
        {
            try
            {
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public UASUSRGRP getUserGrp(DBConnection_EF conn, string user_grp)
        {
            UASUSRGRP rtnUser = null;
            try
            {
                var query = from grp in conn.UASUSRGRP
                            where grp.USER_GRP == user_grp.Trim()
                            select grp;
                rtnUser = query.SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnUser;
        }

        public List<UASUSRGRP> loadAllUserGroup(DBConnection_EF conn)
        {
            List<UASUSRGRP> userGrpList = new List<UASUSRGRP>();
            try
            {
                var query = from grp in conn.UASUSRGRP
                            select grp;

                userGrpList = query.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return userGrpList;
        }

        public void deleteUserGroupByID(DBConnection_EF conn, string user_grp)
        {
            try
            {
                UASUSRGRP rtnUser = null;
                var query = from grp in conn.UASUSRGRP
                            where grp.USER_GRP == user_grp.Trim()
                            select grp;
                rtnUser = query.SingleOrDefault();
                conn.UASUSRGRP.Remove(rtnUser);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public IQueryable getQueryAllSQL(DBConnection_EF con)
        {
            var query = from user_grp in con.UASUSRGRP
                        select user_grp;
            return query;
        }

        //A0.01
        public void updateUser_ClearGroupByGroupName(DBConnection_EF conn, string user_grp)
        {
            try
            {
               

            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
    }
}
