//*********************************************************************************
//      MESDefaultMapAction.cs
//*********************************************************************************
// File Name: MESDefaultMapAction.cs
// Description: 與EAP通訊的劇本
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2019/11/05    Kevin Wei      N/A            A0.01   在"loadBlockQueueBySecIds"，
//                                                     多增加Order by request time。
//**********************************************************************************
using com.mirle.ibg3k0.sc.App;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class BlockZoneQueueDao
    {
        public void add(DBConnection_EF con, BLOCKZONEQUEUE block)
        {
            con.BLOCKZONEQUEUE.Add(block);
            con.SaveChanges();
        }

        public void Update(DBConnection_EF con, BLOCKZONEQUEUE block)
        {
            //bool isDetached = con.Entry(block).State == EntityState.Modified;
            //if (isDetached)
            con.SaveChanges();
        }

        public List<BLOCKZONEQUEUE> loadAll(DBConnection_EF con)
        {
            var query = from point in con.BLOCKZONEQUEUE
                        orderby point.ENTRY_SEC_ID
                        select point;
            return query.ToList();
        }

        public BLOCKZONEQUEUE getUsingBlockQueueByCarIDSecID(DBConnection_EF con, string car_id, String entry_sec_id)
        {
            var query = from block in con.BLOCKZONEQUEUE
                        where block.ENTRY_SEC_ID == entry_sec_id.Trim()
                              && block.CAR_ID == car_id.Trim()
                              //&& block.STATUS != SCAppConstants.BlockQueueState.Release
                              && block.STATUS.CompareTo(SCAppConstants.BlockQueueState.Release) < 0
                        orderby block.ENTRY_SEC_ID
                        select block;
            return query.FirstOrDefault();
        }

        public BLOCKZONEQUEUE getThrouTimeNullBlockQueueByCarIDSecID(DBConnection_EF con, string car_id, String entry_sec_id)
        {
            var query = from block in con.BLOCKZONEQUEUE
                        where block.THROU_TIME == null
                              && block.CAR_ID == car_id.Trim()
                              && block.STATUS.CompareTo(SCAppConstants.BlockQueueState.Release) < 0
                        let bds = from bd in con.ABLOCKZONEDETAIL
                                  where bd.ENTRY_SEC_ID == block.ENTRY_SEC_ID
                                  select bd.SEC_ID
                        where bds.Contains(entry_sec_id)
                        select block;
            return query.FirstOrDefault();
        }
        public BLOCKZONEQUEUE getThrouTimeNullBlockQueueByCarID(DBConnection_EF con, string car_id)
        {
            var query = from block in con.BLOCKZONEQUEUE
                        where block.THROU_TIME == null
                              && block.CAR_ID == car_id.Trim()
                              && block.STATUS.CompareTo(SCAppConstants.BlockQueueState.Release) < 0
                        select block;
            return query.FirstOrDefault();
        }


        public int getCountUsingBlockQueueByCarIDSecID(DBConnection_EF con, string car_id, String entry_sec_id)
        {
            var query = from block in con.BLOCKZONEQUEUE
                        where block.ENTRY_SEC_ID == entry_sec_id.Trim()
                              && block.CAR_ID == car_id.Trim()
                              //&& block.STATUS != SCAppConstants.BlockQueueState.Release
                              && block.STATUS.CompareTo(SCAppConstants.BlockQueueState.Release) < 0
                        orderby block.ENTRY_SEC_ID
                        select block;
            return query.Count();
        }

        public int getCountReqBlockQueueByCarIDSecID(DBConnection_EF con, string car_id, String entry_sec_id)
        {
            var query = from block in con.BLOCKZONEQUEUE
                        where block.ENTRY_SEC_ID == entry_sec_id.Trim()
                              && block.CAR_ID == car_id.Trim()
                            && block.STATUS == SCAppConstants.BlockQueueState.Request
                        orderby block.ENTRY_SEC_ID
                        select block;
            return query.Count();
        }

        public int getCountBlockingBlockQueueByCarIDSecID(DBConnection_EF con, string car_id, String entry_sec_id)
        {
            var query = from block in con.BLOCKZONEQUEUE
                        where block.ENTRY_SEC_ID == entry_sec_id.Trim()
                              && block.CAR_ID == car_id.Trim()
                            && (block.STATUS == SCAppConstants.BlockQueueState.Blocking ||
                            block.STATUS == SCAppConstants.BlockQueueState.Through)
                        orderby block.ENTRY_SEC_ID
                        select block;
            return query.Count();
        }

        public BLOCKZONEQUEUE getUsingBlockQueueByCarID(DBConnection_EF con, string car_id)
        {
            var query = from block in con.BLOCKZONEQUEUE
                        where block.CAR_ID == car_id
                              && block.STATUS.CompareTo(SCAppConstants.BlockQueueState.Release) < 0
                        orderby block.REQ_TIME
                        select block;
            return query.FirstOrDefault();
        }
        public BLOCKZONEQUEUE getBlockQueueInRequestByCarID(DBConnection_EF con, string car_id)
        {
            var query = from block in con.BLOCKZONEQUEUE
                        where block.CAR_ID == car_id
                              && block.STATUS == SCAppConstants.BlockQueueState.Request
                        select block;
            return query.FirstOrDefault();
        }


        public BLOCKZONEQUEUE getReqBlockQueueBySecID(DBConnection_EF con, String entry_sec_id)
        {
            var query = from block in con.BLOCKZONEQUEUE
                        where block.ENTRY_SEC_ID == entry_sec_id.Trim()
                              && block.STATUS == SCAppConstants.BlockQueueState.Request
                        orderby block.REQ_TIME
                        select block;
            return query.FirstOrDefault();
        }

        public int getCountBlockingQueueBySecID(DBConnection_EF con, String entry_sec_id)
        {
            int usingBlockCount = 0;
            //var query = from block in con.BLOCKZONEQUEUE
            //            where block.ENTRY_SEC_ID == entry_sec_id.Trim()
            //                  && (block.STATUS.CompareTo(SCAppConstants.BlockQueueState.Blocking) >= 0
            //                  && block.STATUS.CompareTo(SCAppConstants.BlockQueueState.Release) < 0)
            //            orderby block.REQ_TIME
            //            select block;
            var query = from block in con.BLOCKZONEQUEUE
                        where block.ENTRY_SEC_ID == entry_sec_id.Trim()
                              && (block.STATUS == SCAppConstants.BlockQueueState.Blocking ||
                                  block.STATUS == SCAppConstants.BlockQueueState.Through)
                        orderby block.REQ_TIME
                        select block;
            usingBlockCount = query.Count();
            return usingBlockCount;
        }
        public int getCountBlockingQueueBySecID(DBConnection_EF con, List<String> entry_sec_ids)
        {
            int usingBlockCount = 0;
            var query = from block in con.BLOCKZONEQUEUE
                        where entry_sec_ids.Contains(block.ENTRY_SEC_ID.Trim())
                              && (block.STATUS == SCAppConstants.BlockQueueState.Blocking ||
                                  block.STATUS == SCAppConstants.BlockQueueState.Through)
                        orderby block.REQ_TIME
                        select block;
            usingBlockCount = query.Count();
            return usingBlockCount;
        }

        public List<BLOCKZONEQUEUE> loadBlockingQueueBySecID(DBConnection_EF con, List<String> entry_sec_ids)
        {
            var query = from block in con.BLOCKZONEQUEUE
                        where entry_sec_ids.Contains(block.ENTRY_SEC_ID.Trim())
                              && (block.STATUS == SCAppConstants.BlockQueueState.Blocking ||
                                  block.STATUS == SCAppConstants.BlockQueueState.Through)
                        orderby block.REQ_TIME
                        select block;

            return query.ToList();
        }

        public BLOCKZONEQUEUE getFirstReqBlockQueueBySecIds(DBConnection_EF con, List<string> sec_ids)
        {
            var query = from block in con.BLOCKZONEQUEUE
                            //where sec_ids.Any(sec => block.ENTRY_SEC_ID.Contains(sec.Trim()))
                        where sec_ids.Contains(block.ENTRY_SEC_ID.Trim())
                           && block.STATUS == SCAppConstants.BlockQueueState.Request
                        orderby block.REQ_TIME
                        select block;
            return query.FirstOrDefault();
        }
        public List<BLOCKZONEQUEUE> loadBlockQueueBySecIds(DBConnection_EF con, List<string> sec_ids)
        {
            var query = from block in con.BLOCKZONEQUEUE
                            //where sec_ids.Any(sec => block.ENTRY_SEC_ID.Contains(sec.Trim()))
                        where sec_ids.Contains(block.ENTRY_SEC_ID.Trim())
                           && block.STATUS == SCAppConstants.BlockQueueState.Request
                        orderby block.REQ_TIME //A0.01
                        select block;
            return query.ToList();
        }


        public List<BLOCKZONEQUEUE> loadAllProblematicUsingBlockQueue(DBConnection_EF con, int warnTime)
        {
            DateTime ReferenceTime = DateTime.Now.AddSeconds(-warnTime);
            var query = from block in con.BLOCKZONEQUEUE
                        where (block.STATUS == SCAppConstants.BlockQueueState.Through)
                           && block.THROU_TIME.Value < ReferenceTime
                        orderby block.REQ_TIME
                        select block;
            return query.ToList();
        }
        public List<BLOCKZONEQUEUE> loadAllNonReleaseBlockQueue(DBConnection_EF con)
        {
            var query = from block in con.BLOCKZONEQUEUE
                        where (block.STATUS.CompareTo(SCAppConstants.BlockQueueState.Release) < 0)
                        orderby block.REQ_TIME
                        select block;
            return query.ToList();
        }
        public List<BLOCKZONEQUEUE> loadAllUsingBlockQueue(DBConnection_EF con)
        {
            var query = from block in con.BLOCKZONEQUEUE
                        where (block.STATUS.CompareTo(SCAppConstants.BlockQueueState.Request) >= 0) &&
                        (block.STATUS.CompareTo(SCAppConstants.BlockQueueState.Release) < 0)
                        orderby block.REQ_TIME
                        select block;
            return query.ToList();
        }


        public List<BLOCKZONEQUEUE> loadReqBlockQueueBySecIds(DBConnection_EF con, List<string> sec_ids)
        {
            var query = from block in con.BLOCKZONEQUEUE
                            //where sec_ids.Any(sec => block.ENTRY_SEC_ID.Contains(sec.Trim()))
                        where sec_ids.Contains(block.ENTRY_SEC_ID.Trim())
                           && block.STATUS == SCAppConstants.BlockQueueState.Request
                        orderby block.REQ_TIME
                        select block;
            return query.ToList();
        }

        public List<BLOCKZONEQUEUE> loadNonReleaseBlockQueueBySecIds(DBConnection_EF con, List<string> sec_ids)
        {
            var query = from block in con.BLOCKZONEQUEUE
                            //where sec_ids.Any(sec => block.ENTRY_SEC_ID.Contains(sec.Trim()))
                        where sec_ids.Contains(block.ENTRY_SEC_ID.Trim())
                           && block.STATUS.CompareTo(SCAppConstants.BlockQueueState.Release) <= 0
                        orderby block.REQ_TIME
                        select block;
            return query.ToList();
        }




        public List<BLOCKZONEQUEUE> loadUsingBlockQueueByCarID(DBConnection_EF con, string car_id)
        {
            var query = from block in con.BLOCKZONEQUEUE
                        where block.CAR_ID == car_id
                              //&& block.STATUS != SCAppConstants.BlockQueueState.Release
                              && block.STATUS.CompareTo(SCAppConstants.BlockQueueState.Release) < 0
                        select block;
            return query.ToList();
        }


    }
}
