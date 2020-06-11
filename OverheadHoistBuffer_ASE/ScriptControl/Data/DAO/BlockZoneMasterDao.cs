
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class BlockZoneMasterDao
    {
        public void add(DBConnection_EF con, ABLOCKZONEMASTER vh_id)
        {
            con.ABLOCKZONEMASTER.Add(vh_id);
            con.SaveChanges();
        }


        public List<ABLOCKZONEMASTER> loadAll(DBConnection_EF con)
        {
            var query = from block in con.ABLOCKZONEMASTER
                        orderby block.ENTRY_SEC_ID
                        select block;
            return query.ToList();
        }
        public ABLOCKZONEMASTER getByID(DBConnection_EF con, String vh_id)
        {
            var query = from block in con.ABLOCKZONEMASTER
                        where block.ENTRY_SEC_ID == vh_id
                        select block;
            return query.FirstOrDefault();
        }
        public ABLOCKZONEMASTER getByAdrID(DBConnection_EF con, String adr_id)
        {
            var query = from block in con.ABLOCKZONEMASTER
                        where block.LEAVE_ADR_ID_1 == adr_id
                        || block.LEAVE_ADR_ID_2 == adr_id
                        select block;
            return query.FirstOrDefault();
        }
        public ABLOCKZONEMASTER getByIDAndAdrID(DBConnection_EF con, String sec_id, string adr_id)
        {
            var query = from block in con.ABLOCKZONEMASTER
                        where block.ENTRY_SEC_ID == sec_id
                        && (block.LEAVE_ADR_ID_1 == adr_id || block.LEAVE_ADR_ID_2 == adr_id)
                        select block;
            return query.FirstOrDefault();
        }

        public List<ABLOCKZONEMASTER> loadByIDsAndAdrID(DBConnection_EF con, List<String> sec_ids, string adr_id)
        {
            var query = from block in con.ABLOCKZONEMASTER
                        where sec_ids.Contains(block.ENTRY_SEC_ID.Trim())
                        && (block.LEAVE_ADR_ID_1 == adr_id || block.LEAVE_ADR_ID_2 == adr_id)
                        select block;
            return query.ToList();
        }

        public List<ABLOCKZONEMASTER> loadBZMByAdrID(DBConnection_EF con, string adr_id)
        {
            var query = from block in con.ABLOCKZONEMASTER
                        where block.LEAVE_ADR_ID_1 == adr_id
                        || block.LEAVE_ADR_ID_2 == adr_id
                        select block;
            return query.ToList();
        }

    }
}