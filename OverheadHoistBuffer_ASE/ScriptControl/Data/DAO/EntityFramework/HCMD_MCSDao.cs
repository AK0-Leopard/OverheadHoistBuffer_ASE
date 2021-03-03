using com.mirle.ibg3k0.sc.Data.SECS;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace com.mirle.ibg3k0.sc.Data.DAO.EntityFramework
{
    public class HCMD_MCSDao
    {
        public void AddByBatch(DBConnection_EF con, List<HCMD_MCS> cmd_mcss)
        {
            con.HCMD_MCS.AddRange(cmd_mcss);
            con.SaveChanges();
            //con.BulkInsert(cmd_mcss);

        }
        public List<HCMD_MCS> loadBefore6Months(DBConnection_EF con)
        {
            DateTime before_6_months = DateTime.Now.AddMonths(-6);
            var query = from queue in con.HCMD_MCS
                        where queue.CMD_INSER_TIME < before_6_months
                        select queue;
            return query.ToList();
        }
        public void RemoteByBatch(DBConnection_EF con, List<HCMD_MCS> hCmdMcss)
        {
            hCmdMcss.ForEach(entity => con.Entry(entity).State = EntityState.Deleted);
            con.HCMD_MCS.RemoveRange(hCmdMcss);
            con.SaveChanges();
        }

    }

}
