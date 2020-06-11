using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class MainAlarmDao
    {
        string TableName_MAINALARM = "MAINALARM";
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public List<MainAlarm> loadAllMAINALARM()
        {
            List<MainAlarm> lstMainAlarm = null;
            try
            {
                //查尋DataTable的欄位
                DataTable dt = SCApplication.getInstance().OHxCConfig.Tables[TableName_MAINALARM];
                if (dt != null)
                {
                    var query = from c in dt.AsEnumerable()
                                select new MainAlarm
                                {
                                    CODE = c.Field<string>("CODE"),
                                    DESCRIPTION = c.Field<string>("DESCRIPTION"),
                                    ACTION = c.Field<string>("ACTION"),
                                };
                    lstMainAlarm = query.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
            }
            return lstMainAlarm;
        }

        public MainAlarm getMainAlarmByCode(string code)
        {
            MainAlarm mainAlarm = null;
            try
            {
                //查尋DataTable的欄位
                DataTable dt = SCApplication.getInstance().OHxCConfig.Tables[TableName_MAINALARM];
                if (dt != null)
                {
                    var query = from c in dt.AsEnumerable()
                                where c.Field<string>("CODE").Trim() == code.Trim()
                                select new MainAlarm
                                {
                                    CODE = c.Field<string>("CODE"),
                                    DESCRIPTION = c.Field<string>("DESCRIPTION"),
                                    ACTION = c.Field<string>("ACTION"),
                                };
                    mainAlarm = query.SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
            }
            return mainAlarm;
        }

    }
}