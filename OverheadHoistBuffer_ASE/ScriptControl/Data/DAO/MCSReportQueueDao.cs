
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class MCSReportQueueDao
    {
        public IQueryable getQuerySQL(DBConnection_EF con)
        {
            IQueryable query = from queue in con.AMCSREPORTQUEUE
                               where queue.REPORT_TIME == null
                               orderby queue.INTER_TIME
                               select queue;
            return query;
        }

        public void add(DBConnection_EF con, AMCSREPORTQUEUE queue)
        {
            con.AMCSREPORTQUEUE.Add(queue);
            con.SaveChanges();
        }
        public void AddByBatch(DBConnection_EF con, List<AMCSREPORTQUEUE> queues)
        {
            con.AMCSREPORTQUEUE.AddRange(queues);
            con.SaveChanges();
        }


        public void Update(DBConnection_EF con, AMCSREPORTQUEUE queue)
        {
            //bool isDetached = con.Entry(point).State == EntityState.Modified;
            //if (isDetached)
            con.SaveChanges();
        }

        public List<APOINT> loadAll(DBConnection_EF con)
        {
            var query = from point in con.APOINT
                        orderby point.ADR_ID
                        select point;
            return query.ToList();
        }
        public List<AMCSREPORTQUEUE> loadByNonReport(DBConnection_EF con)
        {
            var query = from queue in con.AMCSREPORTQUEUE.AsNoTracking()
                        where queue.REPORT_TIME == null
                        orderby queue.INTER_TIME
                        select queue;
            return query.ToList();
        }
    }
}
