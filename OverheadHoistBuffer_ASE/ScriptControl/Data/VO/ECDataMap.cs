// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="ECDataMap.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    /// <summary>
    /// Class ECDataMap.
    /// </summary>
    public class ECDataMap
    {
        /// <summary>
        /// Gets or sets the eqp t_ rea l_ identifier.
        /// </summary>
        /// <value>The eqp t_ rea l_ identifier.</value>
        public virtual string EQPT_REAL_ID { get; set; }
        /// <summary>
        /// Gets or sets the ecid.
        /// </summary>
        /// <value>The ecid.</value>
        public virtual string ECID { get; set; }
        /// <summary>
        /// Gets or sets the ecname.
        /// </summary>
        /// <value>The ecname.</value>
        public virtual string ECNAME { get; set; }
        /// <summary>
        /// Gets or sets the ecmin.
        /// </summary>
        /// <value>The ecmin.</value>
        public virtual string ECMIN { get; set; }
        /// <summary>
        /// Gets or sets the ecmax.
        /// </summary>
        /// <value>The ecmax.</value>
        public virtual string ECMAX { get; set; }
        /// <summary>
        /// Gets or sets the ecv.
        /// </summary>
        /// <value>The ecv.</value>
        public virtual string ECV { get; set; }

    }
}
