using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL.Interface;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class ZoneCommandBLL : IZoneCommandBLL
    {
        SCApplication app = null;

        public DB OperateDB { private set; get; }
        public Catch OperateCatch { private set; get; }

        public ZoneCommandBLL()
        {
        }
        public void start(SCApplication _app)
        {
            app = _app;
            OperateDB = new DB();
            OperateCatch = new Catch(_app.getCommObjCacheManager());
        }

        public ZoneCommandGroup getZoneCommandGroup(string zoneCommnadID)
        {
            return OperateCatch.getZoneCommandGroup(zoneCommnadID);
        }


        public class DB
        {

        }
        public class Catch
        {
            CommObjCacheManager CacheManager;
            public Catch(CommObjCacheManager _cache_manager)
            {
                CacheManager = _cache_manager;
            }

            public ZoneCommandGroup getZoneCommandGroup(string zoneID)
            {
                var zoneCommandGroups = CacheManager.getZoneCommandGroups();
                return zoneCommandGroups.Where(group => SCUtility.isMatche(group.ZoneCommandID, zoneID))
                                        .FirstOrDefault();
            }



        }
    }
}