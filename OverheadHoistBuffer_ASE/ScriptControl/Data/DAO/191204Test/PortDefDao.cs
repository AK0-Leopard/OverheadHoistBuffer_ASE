//*********************************************************************************
//      PortDefDao.cs
//*********************************************************************************
// File Name: PortDefDao.cs
// Description: 讀取PortDef Table資料庫之方法
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag          Description
// ------------- -------------  -------------  ------       -----------------------------
// 2020/06/12    Jason Wu       N/A            A20.06.12.0  新增LoadAGVPortByStationID()讀取資料庫獲得目前AGV type的port之資訊
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Data;
using NLog;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class PortDefDao : DaoBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public void insertPortDef(DBConnection_EF conn, PortDef portdef)
        {
            try
            {
                conn.PortDef.Add(portdef);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public void DeletePortDef(DBConnection_EF conn, PortDef portdef)
        {
            try
            {
                conn.PortDef.Remove(portdef);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public void UpdatePortDef(DBConnection_EF conn)
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

        public List<PortDef> LoadPortDef(DBConnection_EF conn, string ohbName)
        {
            try
            {
                var port = from a in conn.PortDef
                           where a.OHBName == ohbName
                           select a;
                return port.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public List<PortDef> LoadCVPort(DBConnection_EF conn, string ohbName)
        {
            try
            {
                var port = from a in conn.PortDef
                           where a.OHBName == ohbName
                                  && (a.UnitType == "OHCV"
                                   || a.UnitType == "AGV"
                                   || a.UnitType == "NTB"
                                   || a.UnitType == "STK"
                                   || a.UnitType == "AGVZONE"
                                     )
                           select a;
                return port.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
        //A20.06.12
        public List<PortDef> LoadAGVPortByStationID(DBConnection_EF conn, string ohbName, string AGVStationID)
        {
            try
            {
                var port = from a in conn.PortDef
                           where a.OHBName == ohbName
                                  && a.UnitType == "AGV"
                                  && a.ZoneName == AGVStationID
                           select a;
                return port.ToList();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public PortDef GetPortData(DBConnection_EF conn, string PortID, string ohtName)
        {
            try
            {
                var port = from a in conn.PortDef
                           where a.PLCPortID.Trim() == PortID.Trim() && a.OHBName.Trim() == ohtName.Trim()
                           select a;
                return port.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }

        public IQueryable getQueryAllSQL(DBConnection_EF conn)
        {
            try
            {
                var port = from a in conn.PortDef
                           select a;
                return port;
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
            }
        }
    }
}
