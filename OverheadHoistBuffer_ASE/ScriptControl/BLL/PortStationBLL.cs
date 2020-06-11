using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.DAO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class PortStationBLL
    {
        public DB OperateDB { private set; get; }
        public Catch OperateCatch { private set; get; }
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public PortStationBLL()
        {
        }
        public void start(SCApplication _app)
        {
            OperateDB = new DB(_app.PortStationDao);
            OperateCatch = new Catch(_app.getEQObjCacheManager());
        }

        public class DB
        {
            PortStationDao portStationDao = null;
            public DB(PortStationDao _portStationDao)
            {
                portStationDao = _portStationDao;
            }
            public APORTSTATION get(string _id)
            {
                APORTSTATION rtnPortStation = null;
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    rtnPortStation = portStationDao.getByID(con, _id);
                }
                return rtnPortStation;
            }
            public bool add(APORTSTATION portStation)
            {
                try
                {
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        portStationDao.add(con, portStation);
                    }
                }
                catch
                {
                    return false;
                }
                return true;
            }

            public bool updatePriority(string portID, int priority)
            {
                try
                {
                    APORTSTATION port_statino = new APORTSTATION();
                    port_statino.PORT_ID = portID;
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        con.APORTSTATION.Attach(port_statino);
                        port_statino.PRIORITY = priority;

                        con.Entry(port_statino).Property(p => p.PRIORITY).IsModified = true;

                        portStationDao.update(con, port_statino);
                        con.Entry(port_statino).State = EntityState.Detached;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                    return false;
                }
                return true;
            }

            public bool updatePortType(APORTSTATION port, int type)
            {
                try
                {
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        con.APORTSTATION.Attach(port);
                        port.PORT_TYPE = type;

                        portStationDao.update(con, port);
                        //con.Entry(port_statino).State = EntityState.Detached;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                    return false;
                }
                return true;
            }

            public bool updateServiceStatus(string portID, int status)
            {
                try
                {
                    APORTSTATION port_statino = new APORTSTATION();
                    port_statino.PORT_ID = portID;
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        con.APORTSTATION.Attach(port_statino);
                        port_statino.PORT_SERVICE_STATUS = status;

                        con.Entry(port_statino).Property(p => p.PORT_SERVICE_STATUS).IsModified = true;

                        portStationDao.update(con, port_statino);
                        con.Entry(port_statino).State = EntityState.Detached;
                    }
                }
                catch
                {
                    return false;
                }
                return true;
            }

            public void updatePortStationStatus(string port_id, E_PORT_STATUS port_status)
            {
                APORTSTATION portTemp = null;
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    portTemp = portStationDao.getByID(con, port_id);
                    if (portTemp != null)
                    {
                        portTemp.PORT_STATUS = port_status;
                        portStationDao.update(con, portTemp);
                    }
                }
            }
            public bool updatePortStatus(string portID, E_PORT_STATUS status)
            {
                try
                {
                    APORTSTATION port_statino = new APORTSTATION();
                    port_statino.PORT_ID = portID;
                    using (DBConnection_EF con = DBConnection_EF.GetUContext())
                    {
                        con.APORTSTATION.Attach(port_statino);
                        port_statino.PORT_STATUS = status;

                        con.Entry(port_statino).Property(p => p.PORT_STATUS).IsModified = true;

                        portStationDao.update(con, port_statino);
                        con.Entry(port_statino).State = EntityState.Detached;
                    }
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }
        public class Catch
        {
            EQObjCacheManager CacheManager;
            public Catch(EQObjCacheManager _cache_manager)
            {
                CacheManager = _cache_manager;
            }
            public List<APORTSTATION> loadPortStations()
            {
                return CacheManager.getALLPortStation();
            }




            public List<APORTSTATION> loadAllPortBySegmentID(string segment_id, BLL.SectionBLL sectionBLL)
            {
                List<APORTSTATION> port_stations = null;
                List<ASECTION> sections = sectionBLL.cache.loadSectionsBySegmentID(segment_id);
                List<string> adrs_from = sections.Select(sec => sec.FROM_ADR_ID.Trim()).ToList();
                List<string> adrs_to = sections.Select(sec => sec.TO_ADR_ID.Trim()).ToList();
                List<string> adrs = adrs_from.Concat(adrs_to).Distinct().ToList();

                var all_port_station = CacheManager.getALLPortStation();
                var query = from port in all_port_station
                            where adrs.Contains(SCUtility.Trim(port.ADR_ID))
                            orderby port.PORT_ID
                            select port;

                return query.ToList();
            }

            public APORTSTATION getPortStation(string port_id)
            {
                APORTSTATION portTemp = CacheManager.getPortStation(port_id);
                return portTemp;
            }
            public APORTSTATION getPortStationByID(string adr_id)
            {
                APORTSTATION portTemp = CacheManager.getALLPortStation().
                                                     Where(p => SCUtility.isMatche(p.ADR_ID, adr_id)).
                                                     SingleOrDefault();
                return portTemp;
            }

            public bool updateServiceStatus(string portID, int status)
            {
                try
                {
                    APORTSTATION port_station = CacheManager.getPortStation(portID);
                    port_station.PORT_SERVICE_STATUS = status;
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                    return false;
                }
                return true;
            }

            public bool updatePriority(string portID, int priority)
            {
                try
                {
                    APORTSTATION port_station = CacheManager.getPortStation(portID);
                    port_station.PRIORITY = priority;
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                    return false;
                }
                return true;
            }

            public void updatePortStationStatus(string portID, E_PORT_STATUS portStatus)
            {
                APORTSTATION port_station = CacheManager.getPortStation(portID);
                if (port_station != null)
                {
                    port_station.PORT_STATUS = portStatus;
                }
            }
            public bool updatePortStatus(string portID, E_PORT_STATUS status)
            {
                try
                {
                    APORTSTATION port_station = CacheManager.getPortStation(portID);
                    port_station.PORT_STATUS = status;
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                    return false;
                }
                return true;
            }
            public void updatePortStationCSTExistStatus(string port_id, string cst_id)
            {
                APORTSTATION port_station = CacheManager.getPortStation(port_id);
                if (port_station != null)
                {
                    port_station.CST_ID = cst_id;
                }
            }

            public bool IsExist(string portID)
            {
                APORTSTATION port_station = CacheManager.getPortStation(portID);
                return port_station != null;
            }

            public bool IsPortInSpecifiedSegment( BLL.SectionBLL sectionBLL, string portID, string segmentID)
            {
                APORTSTATION aPORTSTATION = getPortStation(portID);
                ASECTION aSECTION = sectionBLL.cache.GetSectionsByAddress(aPORTSTATION.ADR_ID.Trim()).First();
                return SCUtility.isMatche(aSECTION.SEG_NUM, segmentID);
            }

            public bool CheckSegmentInActiveByPortID(BLL.SegmentBLL segmentBLL, BLL.SectionBLL sectionBLL, string port_id)
            {
                bool SegmentInActive = true;
                APORTSTATION aPORTSTATION = getPortStation(port_id);
                ASECTION aSECTION = sectionBLL.cache.GetSectionsByAddress(aPORTSTATION.ADR_ID.Trim()).First();
                SegmentInActive = segmentBLL.cache.IsSegmentActive(aSECTION.SEG_NUM);
                return SegmentInActive;
            }



        }
    }
}