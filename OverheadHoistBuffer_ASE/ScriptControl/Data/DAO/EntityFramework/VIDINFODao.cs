using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.DAO.EntityFramework
{
    public class VIDINFODao
    {
        public void add(DBConnection_EF con, AVIDINFO ceid)
        {
            con.AVIDINFO.Add(ceid);
            con.SaveChanges();
        }

        public void update(DBConnection_EF con)
        {
            con.SaveChanges();
        }

        public AVIDINFO getByID(DBConnection_EF con, String eq_id)
        {
            var query = from vid in con.AVIDINFO
                        where vid.EQ_ID == eq_id.Trim()
                        select vid;
            return query.SingleOrDefault();
        }
        public AVIDINFO getByMCSCmdID(DBConnection_EF con, String cmdID)
        {
            var query = from vid in con.AVIDINFO
                        where vid.COMMAND_ID == cmdID.Trim()
                        select vid;
            return query.SingleOrDefault();
        }



    }

}
