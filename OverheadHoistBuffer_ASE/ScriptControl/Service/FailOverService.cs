using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Service
{
    public class FailOverService
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        TimeSpan timeOut_10Sec = new TimeSpan(0, 0, 10);
        TimeSpan timeOut_3Sec = new TimeSpan(0, 0, 3);
        SCApplication scApp = null;
        RedisCacheManager redisCacheManager = null;
        const string REDIS_LOCK_KEY_FAILOVER = "LOCK_KEY_FAILOVER";
        const string REDIS_KEY_WORD_CURRENT_MASTER = "OHxC CURRENT MASTER";
        string REDIS_KEY_WORD_MASTER_TITLE = "MASTER_NAME_{0}";
        string REDIS_KEY_WORD_MASTER_Name;
        public void start(SCApplication app)
        {
            scApp = app;
            redisCacheManager = app.getRedisCacheManager();
            REDIS_KEY_WORD_MASTER_Name = string.Format(REDIS_KEY_WORD_MASTER_TITLE, SCApplication.ServerName);
            //scApp.getEQObjCacheManager().getLine().ServiceMode =
            //    isActive() ? SCAppConstants.AppServiceMode.Active : SCAppConstants.AppServiceMode.Standby;

        }

        public bool isActive()
        {
            bool isMaster = false;
            string timeStemp = DateTime.Now.ToString(SCAppConstants.TimestampFormat_19);
            ALINE line = scApp.getEQObjCacheManager().getLine();
            try
            {
#if DEBUG
                //return true;
#endif
                //1.確認Current host是不是自己
                //  a.是:將自己的直在Set一次並回傳true
                string current_ohxc_id = redisCacheManager.StringGet(REDIS_KEY_WORD_CURRENT_MASTER);
                if (!SCUtility.isEmpty(current_ohxc_id) && SCUtility.isMatche(SCApplication.ServerName, current_ohxc_id))
                {
                    SetMaserHeartbeat();
                    isMaster = true;
                }
                //  b.否:回傳false，並確認目前的Master是不是存在,如果不存在的話將自己設置為Mater
                else
                {
                    string current_master_name = string.Format(REDIS_KEY_WORD_MASTER_TITLE, current_ohxc_id);
                    if (scApp.getRedisCacheManager().KeyExists(current_master_name))
                    {
                        isMaster = false;
                    }
                    else
                    {
                        if (!tryGetRedisLockKey(REDIS_LOCK_KEY_FAILOVER, timeStemp, timeOut_3Sec))
                        {
                            return line.ServiceMode == SCAppConstants.AppServiceMode.Active;
                        }
                        Set2OHxCMaster();
                        SetMaserHeartbeat();
                        isMaster = true;
                        releaseRedisLockKey(REDIS_LOCK_KEY_FAILOVER, timeStemp);
                    }
                }
            }
            catch (StackExchange.Redis.RedisConnectionException ex)
            {
                logger.Error(ex, "Exception:");
                //scApp.getRedisCacheManager().initialRedisConnection();
                return line.ServiceMode == SCAppConstants.AppServiceMode.Active;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                return line.ServiceMode == SCAppConstants.AppServiceMode.Active;
            }
            return isMaster;
        }


        private bool tryGetRedisLockKey(string key, string time_stemp, TimeSpan time_out)
        {
            return scApp.getRedisCacheManager().stringSetAsync(key,
                                                          time_stemp,
                                                          time_out,
                                                          when: StackExchange.Redis.When.NotExists);
        }
        private void releaseRedisLockKey(string key, string time_stemp)
        {
            string lock_time_stemp = scApp.getRedisCacheManager().StringGet(key);
            if (lock_time_stemp.CompareTo(time_stemp) <= 0)
            {
                scApp.getRedisCacheManager().KeyDelete(key);
            }
        }

        private void Set2OHxCMaster()
        {
            redisCacheManager.stringSetAsync(REDIS_KEY_WORD_CURRENT_MASTER
                                           , SCApplication.ServerName);
        }

        private void SetMaserHeartbeat()
        {
            redisCacheManager.stringSetAsync(REDIS_KEY_WORD_MASTER_Name
                                           , string.Empty
                                           , timeOut_10Sec);
        }

        public void DeleteMaserHeartbeat()
        {
            redisCacheManager.KeyDelete(REDIS_KEY_WORD_MASTER_Name);
        }
    }
}
