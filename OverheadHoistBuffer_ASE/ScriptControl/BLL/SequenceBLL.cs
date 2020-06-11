// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="SequenceBLL.cs" company="">
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
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.DAO;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;
using com.mirle.ibg3k0.sc.Data;
using System.Transactions;

namespace com.mirle.ibg3k0.sc.BLL
{
    /// <summary>
    /// Class SequenceBLL.
    /// </summary>
    public class SequenceBLL
    {
        /// <summary>
        /// The sc application
        /// </summary>
        private SCApplication scApp = null;
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The seq DAO
        /// </summary>
        private SequenceDao seqDao = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceBLL"/> class.
        /// </summary>
        public SequenceBLL()
        {
        }

        /// <summary>
        /// Starts the specified sc application.
        /// </summary>
        /// <param name="scApp">The sc application.</param>
        public void start(SCApplication scApp)
        {
            this.scApp = scApp;
            seqDao = scApp.SequenceDao;
        }


        public string getCommandID(SCAppConstants.GenOHxCCommandType gen_cmd_type)
        {
            string command_id = string.Empty;
            long newCommandNum = getCommandID_Manual_Number();
            string sGen_cmd_type = ((int)gen_cmd_type).ToString();
            command_id = ((int)gen_cmd_type) + DateTime.Today.ToString(SCAppConstants.TimestampFormat_08) + newCommandNum.ToString("0000");
            return command_id;
        }
        /// <summary>
        /// 取得Command ID Sequence(由手動方式所產生的Command ID 流水號)
        /// </summary>
        /// <returns></returns>
        public long getCommandID_Manual_Number()
        {
            lock (SCAppConstants.SEQUENCE_NAME_COMMANDID_MANUAL)
            {
                long seqVal = 0;
                try
                {
                    using (TransactionScope tx = new TransactionScope
                             (TransactionScopeOption.Suppress))
                    {
                        //using (DBConnection_EF con = new DBConnection_EF())
                        using (DBConnection_EF con = DBConnection_EF.GetUContext())
                        {
                            ASEQUENCE seq = seqDao.getSequence(con, SCAppConstants.SEQUENCE_NAME_COMMANDID_MANUAL);
                            if (seq == null)
                            {
                                seqVal = SCAppConstants.COMMANDID_MANUAL_NUMBER_MIN;
                                seq = new ASEQUENCE()
                                {
                                    SEQ_NAME = SCAppConstants.SEQUENCE_NAME_COMMANDID_MANUAL,
                                    NXT_VAL = seqVal + 1
                                };
                                seqDao.insertSequence(con, seq);
                            }
                            else
                            {
                                seqVal = seq.NXT_VAL;
                                if (seqVal >= SCAppConstants.COMMANDID_MANUAL_NUMBER_MAX)
                                {
                                    seq.NXT_VAL = SCAppConstants.COMMANDID_MANUAL_NUMBER_MIN;
                                }
                                else
                                {
                                    seq.NXT_VAL = seqVal + 1;
                                }
                                seqDao.updateSequence(con, seq);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                    return seqVal;
                }
                finally
                {

                }
                return seqVal;
            }
        }
    }
}
