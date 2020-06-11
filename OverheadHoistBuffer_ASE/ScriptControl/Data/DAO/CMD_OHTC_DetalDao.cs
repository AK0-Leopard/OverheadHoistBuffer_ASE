
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class CMD_OHTC_DetailDao
    {
        public void add(DBConnection_EF con, ACMD_OHTC_DETAIL Obj)
        {
            con.ACMD_OHTC_DETAIL.Add(Obj);
            con.SaveChanges();
        }
        public void AddByBatch(DBConnection_EF con, List<ACMD_OHTC_DETAIL> cmd_ohtcs)
        {
            con.ACMD_OHTC_DETAIL.AddRange(cmd_ohtcs);
            con.SaveChanges();
        }


        public void Update(DBConnection_EF con, ACMD_OHTC_DETAIL cmd_detail)
        {
            //bool isDetached = con.Entry(cmd_detal).State == EntityState.Modified;
            //if (isDetached)
            con.SaveChanges();
        }



        public void UpdateIsPassFlag(DBConnection_EF con, string cmd_id, int seq_no)
        {
            string sql = "Update [ACMD_OHTC_DETAIL] SET [IS_PASS] = 1 WHERE [CMD_ID] = {0} AND [SEQ_NO] <= {1} AND [IS_PASS] = 0";
            con.Database.ExecuteSqlCommand(sql, cmd_id, seq_no);
        }

        public void DeleteByBatch(DBConnection_EF con, string cmd_id)
        {
            string sql = "DELETE [ACMD_OHTC_DETAIL] WHERE [CMD_ID] = {0}";
            con.Database.ExecuteSqlCommand(sql, cmd_id);
        }


        public ACMD_OHTC_DETAIL getByID(DBConnection_EF con, String cmd_id)
        {
            var query = from cmd in con.ACMD_OHTC_DETAIL
                        where cmd.CMD_ID == cmd_id.Trim()
                        select cmd;
            return query.FirstOrDefault();
        }
        public ACMD_OHTC_DETAIL getLastByID(DBConnection_EF con, String cmd_id)
        {
            var query = from cmd in con.ACMD_OHTC_DETAIL
                        where cmd.CMD_ID == cmd_id.Trim()
                        orderby cmd.SEQ_NO descending
                        select cmd;
            return query.FirstOrDefault();
        }
        public ACMD_OHTC_DETAIL getByCmdIDAndAdrID(DBConnection_EF con, String cmd_id, string adr_id)
        {
            var query = from cmd in con.ACMD_OHTC_DETAIL
                        where cmd.CMD_ID == cmd_id.Trim()
                        && cmd.ADD_ID == adr_id.Trim()
                        select cmd;
            return query.FirstOrDefault();
        }
        public ACMD_OHTC_DETAIL getByCmdIDAndSecID(DBConnection_EF con, String cmd_id, string sec_id)
        {
            var query = from cmd in con.ACMD_OHTC_DETAIL
                        where cmd.CMD_ID == cmd_id.Trim()
                        && cmd.SEC_ID == sec_id.Trim()
                        select cmd;
            return query.FirstOrDefault();
        }
        public ACMD_OHTC_DETAIL getByCmdIDSECIDAndEntryTimeEmpty(DBConnection_EF con, String cmd_id, string sec_id)
        {
            var query = from cmd in con.ACMD_OHTC_DETAIL
                        where cmd.CMD_ID == cmd_id.Trim()
                        && cmd.SEC_ID == sec_id.Trim()
                        && cmd.ADD_ENTRY_TIME == null
                        orderby cmd.SEQ_NO
                        select cmd;
            return query.FirstOrDefault();
        }
        public ACMD_OHTC_DETAIL getByCmdIDSECIDAndLeaveTimeEmpty(DBConnection_EF con, String cmd_id, string sec_id)
        {
            var query = from cmd in con.ACMD_OHTC_DETAIL
                        where cmd.CMD_ID == cmd_id.Trim()
                        && cmd.SEC_ID == sec_id.Trim()
                        && cmd.SEC_ENTRY_TIME != null
                        && cmd.SEC_LEAVE_TIME == null
                        orderby cmd.SEQ_NO
                        select cmd;
            return query.FirstOrDefault();
        }

        public string[] loadAllSecIDByCmdID(DBConnection_EF con, string cmd_id)
        {
            var query = from detail in con.ACMD_OHTC_DETAIL
                        where detail.CMD_ID == cmd_id.Trim()
                        orderby detail.SEQ_NO
                        select detail.SEC_ID.Trim();
            return query.ToArray();
        }

        public int getAllDetailCountByCmdID(DBConnection_EF con, string cmd_id)
        {
            var query = from detail in con.ACMD_OHTC_DETAIL
                        where detail.CMD_ID == cmd_id.Trim()
                        select detail;
            return query.Count();
        }
        public int getAllPassDetailCountByCmdID(DBConnection_EF con, string cmd_id)
        {
            var query = from detail in con.ACMD_OHTC_DETAIL
                        where detail.CMD_ID == cmd_id.Trim()
                           && detail.ADD_ENTRY_TIME != null
                        select detail;
            return query.Count();
        }


        public List<string> loadAllNonPassDetailSecIDByCmdID(DBConnection_EF con, string cmd_id)
        {
            var query = from detail in con.ACMD_OHTC_DETAIL
                        where detail.CMD_ID == cmd_id.Trim() &&
                        !detail.IS_PASS
                        orderby detail.SEQ_NO
                        select detail.SEC_ID;
            return query.ToList();
        }

        public List<string> loadAllWillPassDetailCmdID(DBConnection_EF con, string segment_num)
        {
            var query = from detail in con.ACMD_OHTC_DETAIL
                        where detail.SEG_NUM == segment_num.Trim()
                           && !detail.IS_PASS
                        select detail.CMD_ID.Trim();
            return query.Distinct().ToList();
        }
    }
}