
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class CMD_OHTCDao
    {
        public void add(DBConnection_EF con, ACMD_OHTC blockObj)
        {
            blockObj.CMD_INSER_TIME = DateTime.Now;
            con.ACMD_OHTC.Add(blockObj);
            con.SaveChanges();
        }

        public void Update(DBConnection_EF con, ACMD_OHTC cmd)
        {
            con.SaveChanges();
        }
        public void Update(DBConnection_EF con, List<ACMD_OHTC> cmds)
        {
            con.SaveChanges();
        }

        public void DeleteCmdData(DBConnection_EF conn, ACMD_OHTC cmddata)
        {
            try
            {
                conn.ACMD_OHTC.Remove(cmddata);
                conn.SaveChanges();
            }
            catch (Exception ex)
            {
                //logger.Warn(ex);
                throw;
            }
        }

        public IQueryable getQueryAllSQL(DBConnection_EF con)
        {
            var query = from cmd_mcs in con.ACMD_OHTC
                        select cmd_mcs;
            return query;
        }

        public List<ACMD_OHTC> loadAll(DBConnection_EF con)
        {
            var query = from block in con.ACMD_OHTC
                        orderby block.CMD_START_TIME
                        select block;
            return query.ToList();
        }

        public List<ACMD_OHTC> loadAllQueue_Auto(DBConnection_EF con)
        {
            string sGen_type = ((int)(App.SCAppConstants.GenOHxCCommandType.Auto)).ToString();
            var query = from cmd in con.ACMD_OHTC.AsNoTracking()
                        where (cmd.CMD_STAUS == E_CMD_STATUS.Queue) &&
                              cmd.CMD_ID.StartsWith(sGen_type)
                        orderby cmd.CMD_START_TIME
                        select cmd;
            return query.ToList();
        }


        public List<ACMD_OHTC> loadExecuteCmd(DBConnection_EF con, string vh_id)
        {
            var query = from cmd in con.ACMD_OHTC
                        where cmd.CMD_STAUS < E_CMD_STATUS.NormalEnd &&
                              cmd.VH_ID.Trim() == vh_id
                        orderby cmd.CMD_START_TIME
                        select cmd;
            return query.ToList();
        }

        public ACMD_OHTC getByID(DBConnection_EF con, String cmd_id)
        {
            var query = from cmd in con.ACMD_OHTC
                        where cmd.CMD_ID == cmd_id.Trim()
                        select cmd;
            return query.FirstOrDefault();
        }

        public ACMD_OHTC getQueueByVhID(DBConnection_EF con, String vh_id)
        {
            var query = from cmd in con.ACMD_OHTC
                        where cmd.VH_ID == vh_id.Trim()
                        && cmd.CMD_STAUS == E_CMD_STATUS.Queue
                        select cmd;
            return query.FirstOrDefault();
        }
        public ACMD_OHTC getExecuteByVhID(DBConnection_EF con, String vh_id)
        {
            var query = from cmd in con.ACMD_OHTC
                        where cmd.VH_ID == vh_id.Trim()
                        && cmd.CMD_STAUS >= E_CMD_STATUS.Sending
                        && cmd.CMD_STAUS < E_CMD_STATUS.NormalEnd
                        select cmd;
            return query.FirstOrDefault();
        }
        public ACMD_OHTC getCMD_OHTCByStatusSending(DBConnection_EF con)
        {
            var query = from cmd in con.ACMD_OHTC
                        where cmd.CMD_STAUS == E_CMD_STATUS.Sending
                        orderby cmd.CMD_START_TIME
                        select cmd;
            return query.FirstOrDefault();

        }
        public ACMD_OHTC getCMD_OHTCByVehicleID(DBConnection_EF con, string vh_id)
        {
            var query = from cmd in con.ACMD_OHTC
                        where cmd.VH_ID == vh_id.Trim()
                        orderby cmd.CMD_START_TIME
                        select cmd;
            return query.FirstOrDefault();
        }
        public ACMD_OHTC getExcuteCMD_OHTCByCmdID(DBConnection_EF con, string cmd_id)
        {
            var query = from cmd in con.ACMD_OHTC
                        where cmd.CMD_ID == cmd_id.Trim()
                        && cmd.CMD_STAUS <= E_CMD_STATUS.Execution
                        select cmd;
            return query.FirstOrDefault();
        }

        public int getVhQueueCMDConut(DBConnection_EF con, string vh_id)
        {
            var query = from cmd in con.ACMD_OHTC.AsNoTracking()
                        where cmd.VH_ID == vh_id.Trim() &&
                        cmd.CMD_STAUS == E_CMD_STATUS.Queue
                        select cmd;
            return query.Count();
        }


        public int getVhExcuteCMDConut(DBConnection_EF con, string vh_id)
        {
            var query = from cmd in con.ACMD_OHTC
                        where cmd.VH_ID == vh_id.Trim() &&
                        cmd.CMD_STAUS >= E_CMD_STATUS.Queue &&
                        cmd.CMD_STAUS < E_CMD_STATUS.NormalEnd
                        select cmd;
            return query.Count();
        }


        public int getExecuteByFromAdrIsParkAdr(DBConnection_EF con, string adr)
        {
            var query = from cmd in con.ACMD_OHTC
                        join vh in con.AVEHICLE
                        on cmd.CMD_ID equals vh.OHTC_CMD
                        where cmd.SOURCE == adr.Trim() &&
                              vh.HAS_CST == 0
                        select cmd;
            return query.Count();
        }
        public int getExecuteByToAdrIsParkAdr(DBConnection_EF con, string adr)
        {
            var query = from cmd in con.ACMD_OHTC
                        join vh in con.AVEHICLE
                        on cmd.CMD_ID equals vh.OHTC_CMD
                        where cmd.DESTINATION == adr.Trim()
                        select cmd;
            return query.Count();
        }

        public int getExecuteByFromAdr(DBConnection_EF con, string adr)
        {
            var query = from cmd in con.ACMD_OHTC
                        where cmd.DESTINATION_ADR == adr.Trim() &&
                        cmd.CMD_STAUS < E_CMD_STATUS.NormalEnd
                        select cmd;
            return query.Count();
        }
        public int getExecuteByBoxID(DBConnection_EF con, string boxID)
        {
            var query = from cmd in con.ACMD_OHTC
                        where cmd.BOX_ID == boxID.Trim() &&
                        cmd.CMD_STAUS < E_CMD_STATUS.NormalEnd
                        select cmd;
            return query.Count();
        }
        public int getExecuteByToAdrIsPark(DBConnection_EF con, string adr)
        {
            var query = from cmd in con.ACMD_OHTC
                        join vh in con.AVEHICLE
                        on cmd.CMD_ID equals vh.OHTC_CMD
                        where cmd.DESTINATION == adr.Trim() &&
                        cmd.CMD_TPYE == E_CMD_TYPE.Move_Park &&
                        cmd.CMD_STAUS < E_CMD_STATUS.NormalEnd
                        select cmd;
            return query.Count();
        }

        public ACMD_OHTC getCMD_OHTCByMCScmdID(DBConnection_EF con, string mcs_cmd_id)
        {
            var query = from cmd in con.ACMD_OHTC
                        where cmd.CMD_ID_MCS.Trim() == mcs_cmd_id.Trim()
                        select cmd;
            return query.FirstOrDefault();
        }
        public ACMD_OHTC getCMD_OHTCByMCScmdID_And_NotFinishByDest(DBConnection_EF con, string mcs_cmd_id, string dest)
        {
            var query = from cmd in con.ACMD_OHTC
                        where cmd.CMD_ID_MCS.Trim() == mcs_cmd_id.Trim() 
                            && cmd.CMD_END_TIME == null
                            && cmd.DESTINATION.Trim() == dest.Trim()
                        select cmd;

            return query.FirstOrDefault();
        }
        public ACMD_OHTC getCMD_OHTCByMCScmdID_And_NotFinishBySource(DBConnection_EF con, string mcs_cmd_id, string source)
        {
            var query = from cmd in con.ACMD_OHTC
                        where cmd.CMD_ID_MCS.Trim() == mcs_cmd_id.Trim()
                            && cmd.CMD_END_TIME == null
                            && cmd.SOURCE.Trim() == source.Trim()
                        select cmd;

            return query.FirstOrDefault();
        }
        public void update(DBConnection_EF con, ACMD_OHTC ohtCmd)
        {
            con.SaveChanges();
        }
        public List<ACMD_OHTC> getCMD_OHTCByConditionss(DBConnection_EF conn, DateTime startDatetime, DateTime endDatetime,
              string CSTID = null, string VhID = null)
        {
            var query = conn.ACMD_OHTC.Where(x => x.CMD_START_TIME > startDatetime
                && x.CMD_START_TIME < endDatetime
                && x.CMD_STAUS == E_CMD_STATUS.NormalEnd);

            if (!string.IsNullOrEmpty(CSTID))
            {
                query = query.Where(x => x.CARRIER_ID == CSTID);
            }
            if (!string.IsNullOrEmpty(VhID))
            {
                query = query.Where(x => x.VH_ID == VhID);
            }
            return query.ToList();
        }
    }
}