using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Data;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class UserFuncDao : DaoBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public void createUserFunc(DBConnection_EF conn, UASUFNC userFunc)
        {
            try
            {
                conn.UASUFNC.Add(userFunc);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public List<UASUFNC> loadAllUserFuncByUserGrp(DBConnection_EF conn)
        {
            List<UASUFNC> rtnList = new List<UASUFNC>();
            try
            {
                var query = from userFun in conn.UASUFNC
                            select userFun;
                rtnList = query.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnList;
        }

        public List<UASUFNC> loadUserFuncByUserGrp(DBConnection_EF conn, string user_grp)
        {
            List<UASUFNC> rtnList = new List<UASUFNC>();
            try
            {
                var query = from userFun in conn.UASUFNC
                            where userFun.USER_GRP == user_grp.Trim()
                            select userFun;
                rtnList = query.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnList;
        }

        public UASUFNC getUserFunc(DBConnection_EF conn, string user_grp, string function_code)
        {
            UASUFNC rtnObj = null;
            try
            {
                var query = from userFun in conn.UASUFNC
                            where userFun.USER_GRP == user_grp.Trim()
                            && userFun.FUNC_CODE == function_code.Trim()
                            select userFun;

                rtnObj = query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
            return rtnObj;
        }

        public void deleteUserFunc(DBConnection_EF conn, UASUFNC userFunc)
        {
            try
            {
                conn.UASUFNC.Remove(userFunc);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public void deleteUserFuncByUserGrp(DBConnection_EF conn, string user_grp)
        {
            try
            {
                List<UASUFNC> userFuncList = loadUserFuncByUserGrp(conn, user_grp);
                foreach (UASUFNC userFunc in userFuncList)
                {
                    conn.UASUFNC.Remove(userFunc);
                }
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
            var query = from user_func in con.UASUFNC
                        select user_func;
            return query;
        }

    }
}
