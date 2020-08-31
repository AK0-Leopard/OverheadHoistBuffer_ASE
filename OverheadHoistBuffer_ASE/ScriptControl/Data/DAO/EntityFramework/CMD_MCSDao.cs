//**********************************************************************************
// Date          Author         Request No.    Tag         Description
// ------------- -------------  -------------  ------      -----------------------------
// 2020/06/04    Jason Wu       N/A            A20.06.04.0 新增LoadCmdData_WithoutComplete方法，避免一次撈取全部命令。
//**********************************************************************************
using com.mirle.ibg3k0.sc.Data.SECS;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.DAO.EntityFramework
{
    public class CMD_MCSDao
    {
        public void add(DBConnection_EF con, ACMD_MCS rail)
        {
            con.ACMD_MCS.Add(rail);
            con.SaveChanges();
        }
        public void DeleteCmdData(DBConnection_EF conn, ACMD_MCS cmddata)
        {
            try
            {
                conn.ACMD_MCS.Remove(cmddata);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                //logger.Warn(ex);
                throw;
            }
        }
        public void DeleteLOG_ByACMD_MCS(DBConnection_EF conn, int deleteMonths)
        {
            try
            {
                DateTime dateTime = DateTime.Now.AddMonths(-deleteMonths);
                var deleteData = conn.ACMD_MCS.Where(x => x.CMD_INSER_TIME < dateTime);
                conn.ACMD_MCS.RemoveRange(deleteData);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                //logger.Warn(ex);
                throw;
            }
        }
        public void DeleteLOG_ByACMD_OHTC(DBConnection_EF conn, int deleteMonths)
        {
            try
            {
                DateTime dateTime = DateTime.Now.AddMonths(-deleteMonths);
                var deleteData = conn.ACMD_OHTC.Where(x => x.CMD_INSER_TIME < dateTime);
                conn.ACMD_OHTC.RemoveRange(deleteData);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                //logger.Warn(ex);
                throw;
            }
        }
        public void DeleteLOG_ByACMD_AMCSREPORTQUEUE(DBConnection_EF conn, int deleteMonths)
        {
            try
            {
                DateTime dateTime = DateTime.Now.AddMonths(-deleteMonths);
                var deleteData = conn.AMCSREPORTQUEUE.Where(x => x.INTER_TIME < dateTime);
                conn.AMCSREPORTQUEUE.RemoveRange(deleteData);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                //logger.Warn(ex);
                throw;
            }
        }
        public void update(DBConnection_EF con, ACMD_MCS cmd)
        {
            con.SaveChanges();
        }

        public ACMD_MCS getByID(DBConnection_EF con, String cmd_id)
        {
            var query = from cmd in con.ACMD_MCS
                        where cmd.CMD_ID == cmd_id.Trim()
                        select cmd;
            return query.SingleOrDefault();
        }
        public ACMD_MCS getNowCMD_MCSByID(DBConnection_EF con, String cmd_id)
        {
            var query = from cmd in con.ACMD_MCS
                        where cmd.CMD_ID == cmd_id.Trim() && cmd.TRANSFERSTATE != E_TRAN_STATUS.TransferCompleted
                        select cmd;
            return query.SingleOrDefault();
        }
        public ACMD_MCS getByBoxID(DBConnection_EF con, String box_id)
        {
            var query = from cmd in con.ACMD_MCS
                        where cmd.BOX_ID.Trim() == box_id.Trim() && cmd.TRANSFERSTATE != E_TRAN_STATUS.TransferCompleted
                        select cmd;
            return query.SingleOrDefault();
        }
        public ACMD_MCS getByCstBoxID(DBConnection_EF con, string cst_id, string box_id)
        {
            var query = from cmd in con.ACMD_MCS
                        where cmd.CARRIER_ID.Trim() == cst_id.Trim()
                           && cmd.BOX_ID.Trim() == box_id.Trim() 
                           && cmd.TRANSFERSTATE != E_TRAN_STATUS.TransferCompleted
                        select cmd;
            return query.SingleOrDefault();
        }
        public ACMD_MCS getByExcuteCmd(DBConnection_EF con)
        {
            var query = from cmd in con.ACMD_MCS
                        where cmd.TRANSFERSTATE == E_TRAN_STATUS.Transferring
                        select cmd;
            return query.SingleOrDefault();
        }
        public ACMD_MCS GetCmdDataByAGVtoSHELF(DBConnection_EF con, String SourcePortName)
        {
            var query = from cmd in con.ACMD_MCS
                        where cmd.CMDTYPE == ACMD_MCS.CmdType.AGVStation.ToString()
                           && cmd.HOSTSOURCE == SourcePortName
                           && cmd.TRANSFERSTATE != E_TRAN_STATUS.TransferCompleted
                        select cmd;
            return query.SingleOrDefault();
        }
        public ACMD_MCS GetCmdDataBySHELFtoAGV(DBConnection_EF con, String DestPortName)
        {
            var query = from cmd in con.ACMD_MCS
                        where cmd.CMDTYPE == ACMD_MCS.CmdType.AGVStation.ToString()
                           && cmd.HOSTDESTINATION == DestPortName
                           && cmd.TRANSFERSTATE != E_TRAN_STATUS.TransferCompleted
                        select cmd;
            return query.SingleOrDefault();
        }
        public List<ACMD_MCS> loadACMD_MCSIsQueue(DBConnection_EF con)
        {
            var query = from cmd in con.ACMD_MCS.AsNoTracking()
                        where (cmd.TRANSFERSTATE == E_TRAN_STATUS.Queue)
                        && cmd.CHECKCODE.Trim() == SECSConst.HCACK_Confirm
                        orderby cmd.PRIORITY_SUM descending, cmd.CMD_INSER_TIME
                        select cmd;

            return query.ToList();
        }
        public List<ACMD_MCS> loadCMD_ByTransferring(DBConnection_EF con)
        {
            var query = from cmd in con.ACMD_MCS.AsNoTracking()
                        where (cmd.TRANSFERSTATE == E_TRAN_STATUS.Transferring)
                        orderby cmd.PRIORITY_SUM descending, cmd.CMD_INSER_TIME
                        select cmd;

            return query.ToList();
        }
        public List<ACMD_MCS> loadACMD_MCSIsUnfinished(DBConnection_EF con)
        {
            var query = from cmd in con.ACMD_MCS.AsNoTracking()
                        where cmd.TRANSFERSTATE >= E_TRAN_STATUS.Queue && cmd.TRANSFERSTATE < E_TRAN_STATUS.Canceling
                        //&& cmd.CHECKCODE.Trim() == SECSConst.HCACK_Confirm
                        select cmd;
          
            return query.ToList();
        }

        public IQueryable getQueryAllSQL(DBConnection_EF con)
        {
            var query = from cmd_mcs in con.ACMD_MCS
                        select cmd_mcs;
            return query;
        }
        public List<ACMD_MCS> getCMD_ByOHTName(DBConnection_EF con, string craneName)
        {
            var query = from cmd in con.ACMD_MCS
                        where cmd.CRANE.Trim() == craneName.Trim() && cmd.TRANSFERSTATE != E_TRAN_STATUS.TransferCompleted
                        select cmd;
            return query.ToList();
        }
        public int getCMD_MCSIsQueueCount(DBConnection_EF con)
        {
            var query = from cmd in con.ACMD_MCS
                        where cmd.TRANSFERSTATE == E_TRAN_STATUS.Queue
                        select cmd;
            return query.Count();
        }
        public int getCMD_MCSIsExcuteCount(DBConnection_EF con)
        {
            var query = from cmd in con.ACMD_MCS
                        where cmd.TRANSFERSTATE > E_TRAN_STATUS.Queue
                        && cmd.TRANSFERSTATE < E_TRAN_STATUS.Canceling
                        select cmd;
            return query.Count();
        }
        public int getCMD_MCSIsExcuteCount(DBConnection_EF con, DateTime defore_time)
        {
            var query = from cmd in con.ACMD_MCS
                        where cmd.TRANSFERSTATE > E_TRAN_STATUS.Queue
                        && cmd.TRANSFERSTATE < E_TRAN_STATUS.Canceling
                        && cmd.CMD_INSER_TIME < defore_time
                        select cmd;
            return query.Count();
        }
        public List<string> loadIsExcuteCMD_MCS_ID(DBConnection_EF con, DateTime defore_time)
        {
            var query = from cmd in con.ACMD_MCS
                        where cmd.TRANSFERSTATE > E_TRAN_STATUS.Queue
                        && cmd.TRANSFERSTATE < E_TRAN_STATUS.Canceling
                        && cmd.CMD_INSER_TIME < defore_time
                        select cmd.CMD_ID;
            return query.ToList();
        }
        public int getCMD_MCSIsUnfinishedCount(DBConnection_EF con, List<string> port_ids)
        {
            var query = from cmd in con.ACMD_MCS
                        where port_ids.Contains(cmd.HOSTSOURCE.Trim()) &&
                        cmd.TRANSFERSTATE >= E_TRAN_STATUS.Queue
                        && cmd.TRANSFERSTATE < E_TRAN_STATUS.Canceling
                        select cmd;
            return query.Count();
        }

        public int getCMD_MCSInserCountLastHour(DBConnection_EF con, int hours)
        {
            DateTime nowTime = DateTime.Now;
            DateTime lastTime = nowTime.AddHours(-hours);

            var query = from cmd in con.ACMD_MCS
                        where cmd.CMD_INSER_TIME < nowTime &&
                        cmd.CMD_INSER_TIME > lastTime
                        select cmd;
            return query.Count();
        }
        public int getCMD_MCSFinishCountLastHours(DBConnection_EF con, int hours)
        {
            DateTime nowTime = DateTime.Now;
            DateTime lastTime = nowTime.AddHours(-hours);
            var query = from cmd in con.ACMD_MCS
                        where cmd.CMD_FINISH_TIME < nowTime &&
                        cmd.CMD_FINISH_TIME > lastTime
                        select cmd;
            return query.Count();
        }

        public int getCMD_MCSIsUnfinishedCountByCarrierID(DBConnection_EF con, string carrier_id)
        {
            var query = from cmd in con.ACMD_MCS
                        where cmd.CARRIER_ID.Trim() == carrier_id.Trim() &&
                        cmd.TRANSFERSTATE >= E_TRAN_STATUS.Queue
                        && cmd.TRANSFERSTATE < E_TRAN_STATUS.Canceling
                        select cmd;
            return query.Count();
        }

        public int getCMD_MCSMaxPrioritySum(DBConnection_EF con)
        {
            var query = from cmd in con.ACMD_MCS
                        where cmd.TRANSFERSTATE == E_TRAN_STATUS.Queue
                        orderby cmd.PRIORITY_SUM descending
                        select cmd.PRIORITY_SUM;
            List<int> prorityList = query.ToList();
            if (prorityList.Count == 0)
            {
                return 0;
            }
            else
            {
                return prorityList[0];
            }
        }
        public int getCMD_MCSMinPrioritySum(DBConnection_EF con)
        {
            var query = from cmd in con.ACMD_MCS
                        where cmd.TRANSFERSTATE == E_TRAN_STATUS.Queue
                        orderby cmd.PRIORITY_SUM ascending
                        select cmd.PRIORITY_SUM;
            List<int> prorityList = query.ToList();
            if (prorityList.Count == 0)
            {
                return 0;
            }
            else
            {
                return prorityList[0];
            }
        }

        public int getCMD_MCSTotalCount(DBConnection_EF con)
        {
            var query = from cmd in con.ACMD_MCS
                        select cmd;
            return query.Count();
        }

        public List<ACMD_MCS> LoadCmdData(DBConnection_EF conn)
        {
            try
            {
                var port = from a in conn.ACMD_MCS
                           select a;
                return port.ToList();
            }
            catch (Exception ex)
            {
                //logger.Warn(ex);
                throw;
            }
        }
        //A20.06.04.0
        //將判斷是否為transferComplete 換到資料庫實作，避免撈出所有資料花費時間過久。
        public List<ACMD_MCS> LoadCmdData_WithoutComplete(DBConnection_EF conn)
        {
            try
            {
                var port = from a in conn.ACMD_MCS
                           where a.TRANSFERSTATE != E_TRAN_STATUS.TransferCompleted
                           select a;
                return port.ToList();
            }
            catch (Exception ex)
            {
                //logger.Warn(ex);
                throw;
            }
        }
        public List<ACMD_MCS> LoadAllCmdData(DBConnection_EF conn)  //歷史記錄
        {
            try
            {
                var port = from a in conn.ACMD_MCS
                           orderby a.CMD_INSER_TIME descending
                           select a;
                return port.Take(1000).ToList();
            }
            catch (Exception ex)
            {
                //logger.Warn(ex);
                throw;
            }
        }
        public List<ACMD_MCS> LoadCmdDataByStartEnd(DBConnection_EF conn, DateTime startTime, DateTime endTime)  //歷史記錄
        {
            try
            {
                var port = from a in conn.ACMD_MCS
                           //orderby a.CMD_INSER_TIME descending
                           where a.CMD_INSER_TIME > startTime && a.CMD_INSER_TIME < endTime
                           && a.TRANSFERSTATE == E_TRAN_STATUS.TransferCompleted
                           orderby a.CMD_INSER_TIME 
                           select a;
                return port.ToList();
            }
            catch (Exception ex)
            {
                //logger.Warn(ex);
                throw;
            }
        }
        public List<ACMD_MCS> getCMD_OHTCByConditionss(DBConnection_EF conn, DateTime startDatetime, DateTime endDatetime,
                                                       string CSTID = null, string CmdID = null)
        {
            var query = conn.ACMD_MCS.Where(x => x.CMD_START_TIME > startDatetime
                && x.CMD_START_TIME < endDatetime
                && x.TRANSFERSTATE == E_TRAN_STATUS.TransferCompleted);

            if (!string.IsNullOrEmpty(CSTID))
            {
                query = query.Where(x => x.CARRIER_ID == CSTID);
            }
            if (!string.IsNullOrEmpty(CmdID))
            {
                query = query.Where(x => x.CMD_ID == CmdID);
            }
            return query.ToList();
        }
    }

}
