
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class BlockZoneDetailDao
    {
        public void add(DBConnection_EF con, ABLOCKZONEDETAIL blockObj)
        {
            con.ABLOCKZONEDETAIL.Add(blockObj);
            con.SaveChanges();
        }


        public List<ABLOCKZONEDETAIL> loadAll(DBConnection_EF con)
        {
            var query = from block in con.ABLOCKZONEDETAIL
                        orderby block.ENTRY_SEC_ID
                        select block;
            return query.ToList();
        }
        public ABLOCKZONEDETAIL getByID(DBConnection_EF con, String entry_sec_id)
        {
            var query = from block in con.ABLOCKZONEDETAIL
                        where block.ENTRY_SEC_ID == entry_sec_id
                        select block;
            return query.FirstOrDefault();
        }

        public List<string> loadSecIDByEntrySecID(DBConnection_EF con, string entry_sec_id)
        {
            var query = from block in con.ABLOCKZONEDETAIL
                        where block.ENTRY_SEC_ID == entry_sec_id.Trim()
                        select block.SEC_ID;
            return query.ToList();
        }

        public bool IsVHInBlockZoneByEntrySectionID(DBConnection_EF con, string vh_id, string entry_sec_id)
        {
            var query = from vh in con.AVEHICLE
                        where vh.VEHICLE_ID == vh_id.Trim()
                        let bds = from bd in con.ABLOCKZONEDETAIL
                                  where bd.ENTRY_SEC_ID == entry_sec_id.Trim()
                                  select bd.SEC_ID
                        where bds.Contains(vh.CUR_SEC_ID)
                        select vh;
            return query.Count() != 0;
        }

    }
}