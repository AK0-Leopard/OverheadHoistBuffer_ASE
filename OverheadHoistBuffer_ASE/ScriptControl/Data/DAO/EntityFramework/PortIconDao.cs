using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.DAO.EntityFramework
{
    public class PortIconDao
    {
        public void add(DBConnection_EF con, APORTICON port)
        {
            con.APORTICON.Add(port);
            con.SaveChanges();
        }
        public void Update(DBConnection_EF con, APORTICON port_new)
        {
            con.SaveChanges();
        }
        public APORTICON getByID(DBConnection_EF con, String port_id)
        {
            var query = from point in con.APORTICON
                        where point.PORT_ID == port_id
                        //orderby point.POINT_ID
                        orderby point.ADR_ID
                        select point;
            return query.FirstOrDefault();
        }

        public List<APORTICON> loadAll(DBConnection_EF con)
        {
            var query = from point in con.APORTICON
                        //orderby point.POINT_ID
                        orderby point.ADR_ID
                        select point;
            return query.ToList();
        }

    }

}
