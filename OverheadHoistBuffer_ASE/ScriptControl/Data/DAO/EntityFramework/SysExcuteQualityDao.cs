using com.mirle.ibg3k0.sc.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.DAO.EntityFramework
{
    public class SysExcuteQualityDao
    {
        public void add(DBConnection_EF con, ASYSEXCUTEQUALITY quality)
        {
            con.ASYSEXCUTEQUALITY.Add(quality);
            con.SaveChanges();
        }

        public void update(DBConnection_EF con, ASYSEXCUTEQUALITY quality)
        {
            con.SaveChanges();
        }

        public ASYSEXCUTEQUALITY getByID(DBConnection_EF con, String cmd_id)
        {
            var query = from quality in con.ASYSEXCUTEQUALITY
                        where quality.CMD_ID_MCS == cmd_id.Trim()
                        select quality;
            return query.SingleOrDefault();

        }
        public Dictionary<int, double> loadDicByEachHourInDay(DBConnection_EF con, int year, int month, int day)
        {
            DateTime searchDay = new DateTime(year, month, day, 0, 0, 0);
            Dictionary<int, double> dicQuality = null;
            var query = from quality in con.ASYSEXCUTEQUALITY
                        where quality.CMD_INSERT_TIME.Year == year
                        && quality.CMD_INSERT_TIME.Month == month
                        && quality.CMD_INSERT_TIME.Day == day
                        && quality.CMDQUEUE_TIME != 0
                        group quality by quality.CMD_INSERT_TIME.Hour;
            dicQuality = query.ToDictionary(item => item.Key, item => item.Average(systemQualitys => systemQualitys.CMDQUEUE_TIME));
            return dicQuality;

        }

        public List<ASYSEXCUTEQUALITY> getAllData(DBConnection_EF con)
        {
            var query = from quality in con.ASYSEXCUTEQUALITY
                        select quality;
            return query.ToList();
        }


        #region Redis
        TimeSpan timeSpan_1Hour = new TimeSpan(1, 0, 10);
        public void add(RedisCacheManager redisCache, ASYSEXCUTEQUALITY quality)
        {
            //redisCache.Obj2ByteArraySetAsync(quality.CMD_ID_MCS, quality, timeSpan_1Hour);
            redisCache.Obj2ByteArraySetAsync(quality.CMD_ID_MCS, quality);
        }

        public ASYSEXCUTEQUALITY getByID(RedisCacheManager redisCache, String cmd_id)
        {
            return redisCache.StringGet<ASYSEXCUTEQUALITY>(cmd_id);
        }
        public void update(RedisCacheManager redisCache, ASYSEXCUTEQUALITY quality)
        {
            add(redisCache, quality);
        }
        public void delete(RedisCacheManager redisCache, string mcs_cmd_id)
        {
            redisCache.KeyDelete(mcs_cmd_id);
        }
        #endregion Redis
    }

}
