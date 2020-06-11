using com.mirle.ibg3k0.sc.Data.SECS;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class VCMD_MCSDao
    {
        public List<VACMD_MCS> loadAllVACMD(DBConnection_EF con)
        {
            var query = from cmd in con.VACMD_MCS.AsNoTracking()
                        select cmd;
            return query.ToList();
        }

        //public IQueryable getQueryAllSQL(DBConnection_EF con)
        //{
        //    var query = from vacmd_mcs in con.VACMD_MCS
        //                select vacmd_mcs;
        //    return query;
        //}

        public VACMD_MCS getVCMDByID(DBConnection_EF con, String cmd_id)
        {
            var query = from cmd in con.VACMD_MCS
                        where cmd.CMD_ID.Trim() == cmd_id.Trim()
                        select cmd;
            return query.SingleOrDefault();
        }

        public List<VACMD_MCS> getCMD_OHTCByConditionss(DBConnection_EF conn, DateTime startDatetime, DateTime endDatetime,
                                                                                                string CSTID = null, string CmdID = null)
        {
            var query = conn.VACMD_MCS.Where(x => x.CMD_INSER_TIME > startDatetime
                && x.CMD_INSER_TIME < endDatetime
                && x.TRANSFERSTATE != E_TRAN_STATUS.Queue
                && x.TRANSFERSTATE != E_TRAN_STATUS.Transferring);

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