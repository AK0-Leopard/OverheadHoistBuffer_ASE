//*********************************************************************************
//      PortDefBLL.cs
//*********************************************************************************
// File Name: PortDefBLL.cs
// Description: 處理對於PortDef資訊的方法
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag          Description
// ------------- -------------  -------------  ------       -----------------------------
// 2020/06/12    Jason Wu       N/A            A20.06.12.0  新增GetAGVPortGroupDataByStationID()處理轉換AGVStationID 為AGVPortID之流程。
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.sc.Data.DAO;
using NLog;
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class PortDefBLL
    {
        SCApplication scApp = null;
        PortDefDao portdefDao = null;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Cache cache { get; private set; }

        ALINE line
        {
            get => scApp.getEQObjCacheManager().getLine();
        }
        public void start(SCApplication scApp)
        {
            this.scApp = scApp;
            portdefDao = scApp.PortDefDao;
            cache = new Cache(scApp.getCommObjCacheManager());
            //line = scApp.getEQObjCacheManager().getLine();
        }
        public void setPortDef(PortDef port)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    portdefDao.insertPortDef(con, port);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }
        public bool UpdataPortType(string portID, E_PortType portType) //更新流向
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    con.PortDef.Where(data => data.PLCPortID == portID).First().PortType = portType;
                    con.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
        }
        public void UpdataPortService(string portID, E_PORT_STATUS service)  //service：1 = OutOfService, 2 = InService
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    con.PortDef.Where(data => data.PLCPortID == portID).First().State = service;
                    con.SaveChanges();
                }

                scApp.TransferService.TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "UpdataPortService:  port_id: " + portID
                    + " service: " + service
                );
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }
        public void UpdataAGVPortService(string portID, E_PORT_STATUS service)  //service：1 = OutOfService, 2 = InService
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    con.PortDef.Where(data => data.PLCPortID == portID).First().AGVState = service;
                    con.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }

        }

        public void UpdataAGVSimPortLocationType(string portID, int service_num)  //service：1 = first 2 port, 2 = Last 2 port
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    con.PortDef.Where(data => data.PLCPortID == portID).First().PortLocationType = service_num;
                    con.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }

        }
        public bool updatePriority(string port_id, int priority)
        {
            try
            {
                //ShelfDef shelf_def = new ShelfDef();
                //shelf_def.ShelfID = shelf_id;
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    var port_def = con.PortDef.Where(x => x.PLCPortID == port_id).FirstOrDefault();
                    port_def.PRIORITY = priority;
                    //shelf_def.SelectPriority = priority;

                    con.Entry(port_def).Property(p => p.PRIORITY).IsModified = true;

                    portdefDao.UpdatePortDef(con);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
            return true;
        }
        public bool UpdateIgnoreModeChange(string portName, string enable)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    con.PortDef.Where(data => data.PLCPortID == portName).First().IgnoreModeChange = enable;
                    con.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
        }
        public List<PortDef> GetOHB_PortData(string ohbName)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return portdefDao.LoadPortDef(con, ohbName);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }

        public List<PortDef> GetOHB_CVPortData(string ohbName)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return portdefDao.LoadCVPort(con, ohbName);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        //A20.06.12
        public List<PortDef> GetAGVPortGroupDataByStationID(string ohbName, string stationID)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    //取station最後一碼 以分辨其為哪一group
                    stationID = stationID.Trim();
                    string AGVStationID = stationID;
                    List<PortDef> agvPortFromStationID = portdefDao.LoadAGVPortByStationID(con, ohbName, AGVStationID);
                    return agvPortFromStationID;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }

        public List<PortDef> GetAGVPortGroupData(string ohbName, string portID)  //取得流向資料，type: 0 = In, 1 = Out
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    List<PortDef> agvPort = portdefDao.LoadPortDef(con, ohbName).Where(data => data.UnitType == "AGV").ToList();
                    PortDef portData = agvPort.Where(data => data.PLCPortID == portID).FirstOrDefault();
                    string group = "";

                    if (portData != null)
                    {
                        group = portData.ZoneName;
                    }

                    return agvPort.Where(data => data.ZoneName == group).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

        }
        public int GetPortTypeDefCount(string ohbName, E_PortType type)    //取得預設流向幾進幾出，type: 0 = In, 1 = Out
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return portdefDao.LoadPortDef(con, ohbName).Where(data => data.PortTypeDef == type).ToList().Count();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return 0;
            }
        }

        public PortDef GetPortData(string PortID)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    return portdefDao.GetPortData(con, PortID, line.LINE_ID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

        }

        public string GetUnitType(string portName)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    string unitName = portdefDao.GetPortData(con, portName, line.LINE_ID).UnitType;

                    if (con.ShelfDef.Where(data => data.ShelfID == portName).Count() > 0)
                    {
                        unitName = "Shelf";
                    }

                    return unitName;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }

        }

        public bool getAddressID(string adr_port_id, out string adr)
        {
            E_VH_TYPE vh_type = E_VH_TYPE.None;
            return getAddressID(adr_port_id, out adr, out vh_type);
        }

        public bool getAddressID(string adr_port_id, out string adr, out E_VH_TYPE vh_type)
        {
            PortDef port = scApp.MapBLL.getPortByPortDefID(adr_port_id);
            vh_type = E_VH_TYPE.None;
            if (port != null)
            {
                adr = port.ADR_ID.Trim();
                return true;
            }
            else
            {
                adr = adr_port_id;
                return false;
            }
        }

        public List<PortDef> getAGVPortData()
        {
            string ohbName = scApp.getEQObjCacheManager().getLine().LINE_ID;
            List<PortDef> AGV_station = scApp.PortDefBLL.GetOHB_CVPortData(ohbName).
                Where(data => data.UnitType == "AGV"
                ).ToList();
            return AGV_station;
        }
        public class Cache
        {
            CommObjCacheManager objCacheManager;
            public Cache(CommObjCacheManager _objCacheManager)
            {
                objCacheManager = _objCacheManager;
            }
            public List<PortDef> loadCanAvoidCVPortDefs()
            {
                var port_defs = objCacheManager.getPortDefs();
                return port_defs.Where(port => SCUtility.isMatche(port.UnitType, "OHCV") &&
                                               port.PortTypeIndex.HasValue && port.PortTypeIndex.Value == 1).
                                 ToList();
            }
            public List<PortDef> loadCanAvoidPortDefs()
            {
                var port_defs = objCacheManager.getPortDefs();
                return port_defs.Where(port => port.PortTypeIndex.HasValue && port.PortTypeIndex.Value == 1).
                                 ToList();
            }

            public List<PortDef> loadCVPortDefs()
            {
                var port_defs = objCacheManager.getPortDefs();
                return port_defs.Where(port => SCUtility.isMatche(port.UnitType, "OHCV")).
                                 ToList();
            }
            public PortDef getCVPortDef(string portID)
            {
                var port_defs = objCacheManager.getPortDefs();
                return port_defs.Where(port => SCUtility.isMatche(port.PLCPortID, portID)).
                                 FirstOrDefault();
            }

        }

        internal bool doUpdateTimeOutForAutoUD(string port_id, int timeOutForAutoUD)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    var port_def = con.PortDef.Where(x => x.PLCPortID == port_id).FirstOrDefault();
                    port_def.TimeOutForAutoUD = timeOutForAutoUD;
                    con.Entry(port_def).Property(p => p.TimeOutForAutoUD).IsModified = true;
                    portdefDao.UpdatePortDef(con);
                }

                scApp.TransferService.TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "更新儲位狀態:  port_id: " + port_id
                    + " TimeOutForAutoUD: " + timeOutForAutoUD
                );
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
            return true;
        }

        internal bool doUpdateTimeOutForAutoInZone(string port_id, string timeOutForAutoInZone)
        {
            try
            {
                using (DBConnection_EF con = DBConnection_EF.GetUContext())
                {
                    var port_def = con.PortDef.Where(x => x.PLCPortID == port_id).FirstOrDefault();
                    port_def.TimeOutForAutoInZone = timeOutForAutoInZone;
                    con.Entry(port_def).Property(p => p.TimeOutForAutoInZone).IsModified = true;
                    portdefDao.UpdatePortDef(con);
                }

                scApp.TransferService.TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "更新儲位狀態:  port_id: " + port_id
                    + " TimeOutForAutoInZone: " + timeOutForAutoInZone
                );
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return false;
            }
            return true;
        }
    }
}
