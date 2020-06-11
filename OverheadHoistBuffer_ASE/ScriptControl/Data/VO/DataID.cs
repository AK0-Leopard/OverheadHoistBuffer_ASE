// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="DataID.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    /// <summary>
    /// Class DataID.
    /// </summary>
    public class DataID
    {
        /// <summary>
        /// The data_id
        /// </summary>
        private static int data_id = 0;
        /// <summary>
        /// Gets the data_ identifier.
        /// </summary>
        /// <value>The data_ identifier.</value>
        public static string Data_ID { 
            get
            {
                string rtnVal = data_id.ToString();
                try
                {
                    if (++data_id > SCAppConstants.MAX_DATA_ID)
                    {
                        data_id = 0;
                    }
                    rtnVal = SCUtility.FillPadLeft(data_id.ToString(), '0', SCAppConstants.DATA_ID_LENGTH);
                }
                catch (Exception ex)
                {
                    LogManager.GetCurrentClassLogger().Error(ex, "Exception:");
                }
                return rtnVal;
            } 
        }

    }
}
