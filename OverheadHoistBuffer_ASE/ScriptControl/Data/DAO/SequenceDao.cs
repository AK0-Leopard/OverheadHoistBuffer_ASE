// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="SequenceDao.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Data;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;
using System.Data.Entity;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    /// <summary>
    /// Class SequenceDao.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.DaoBase" />
    public class SequenceDao : DaoBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public void insertSequence(DBConnection_EF conn, ASEQUENCE seq)
        {
            try
            {
                conn.ASEQUENCE.Add(seq);
                conn.SaveChanges();

            }
            catch (Exception ex)
            {
                logger.Warn(ex);
            }
        }

        /// <summary>
        /// Updates the sequence.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="seq">The seq.</param>
        public void updateSequence(DBConnection_EF conn, ASEQUENCE seq)
        {
            try
            {
                //bool isDetached = conn.Entry(seq).State == EntityState.Modified;
                //if (isDetached)
                conn.SaveChanges();
                conn.Entry(seq).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
            }
        }

        /// <summary>
        /// Gets the sequence.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="readLock">The read lock.</param>
        /// <param name="seq_name">The seq_name.</param>
        /// <returns>Sequence.</returns>
        public ASEQUENCE getSequence(DBConnection_EF conn, string seq_name)
        {
            ASEQUENCE rtnSeq = null;
            try
            {
                var query = from point in conn.ASEQUENCE
                            where point.SEQ_NAME == seq_name.Trim()
                            select point;
                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
            }
            return rtnSeq;
        }

    }
}
