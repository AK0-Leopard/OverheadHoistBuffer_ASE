using com.mirle.ibg3k0.sc.Data.SECS;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace com.mirle.ibg3k0.sc.Data.DAO.EntityFramework
{
    public class HCMD_OHTCDao
    {
        public void AddByBatch(DBConnection_EF con, List<HCMD_OHTC> cmd_ohtcs)
        {
            con.HCMD_OHTC.AddRange(cmd_ohtcs);
            con.SaveChanges();
            //con.BulkInsert(cmd_ohtcs);
        }

        public List<HCMD_OHTC> loadBefore6Months(DBConnection_EF con)
        {
            DateTime before_6_months = DateTime.Now.AddMonths(-6);
            var query = from queue in con.HCMD_OHTC
                        where queue.CMD_END_TIME < before_6_months
                        select queue;
            return query.ToList();
        }
        public void RemoteByBatch(DBConnection_EF con, List<HCMD_OHTC> hCmdOhtc)
        {
            hCmdOhtc.ForEach(entity => con.Entry(entity).State = EntityState.Deleted);
            con.HCMD_OHTC.RemoveRange(hCmdOhtc);
            con.SaveChanges();
        }

    }

}
