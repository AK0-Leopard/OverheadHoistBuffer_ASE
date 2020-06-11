using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class GROUPRAILSDao
    {
        public void add(DBConnection_EF con, AGROUPRAILS group_rail)
        {
            con.AGROUPRAILS.Add(group_rail);
            con.SaveChanges();
        }
        public void update(DBConnection_EF con, AGROUPRAILS group_rail)
        {
            //bool isDetached = con.Entry(group_rail).State == EntityState.Modified;
            //if (isDetached)
                con.AGROUPRAILS.Attach(group_rail);
            con.SaveChanges();
        }




        public List<string> loadAllSectionID(DBConnection_EF con)
        {
            var query = from g in con.AGROUPRAILS
                        orderby g.SECTION_ID
                        select g.SECTION_ID;
            return query.Distinct().ToList();
        }

        public void getFirstAndLastRailBySecID(DBConnection_EF con, string sec_id, out AGROUPRAILS first_rail, out AGROUPRAILS last_rail)
        {
            var query = from g in con.AGROUPRAILS
                        where g.SECTION_ID == sec_id
                        orderby g.RAIL_NO
                        select g;
            first_rail = query.FirstOrDefault();
            last_rail = query.AsEnumerable().LastOrDefault();
        }


        public List<AGROUPRAILS> loadAll(DBConnection_EF con)
        {
            var query = from group_rail in con.AGROUPRAILS
                        orderby group_rail.SECTION_ID, group_rail.RAIL_NO
                        select group_rail;
            return query.ToList();
        }
        public AGROUPRAILS getByID(DBConnection_EF con, String sec_id)
        {
            var query = from g in con.AGROUPRAILS
                        where g.SECTION_ID == sec_id.Trim()
                        select g;
            return query.FirstOrDefault();
        }
    }

}
