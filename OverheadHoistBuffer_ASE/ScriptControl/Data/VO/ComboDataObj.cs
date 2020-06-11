// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="ComboDataObj.cs" company="">
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
    /// Class ComboDataObj.
    /// </summary>
    public class ComboDataObj
    {
        /// <summary>
        /// The m_display
        /// </summary>
        private string m_display = string.Empty;
        /// <summary>
        /// The m_value
        /// </summary>
        private string m_value = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComboDataObj"/> class.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="value">The value.</param>
        public ComboDataObj(string display, string value)
        {
            this.m_display = display;
            this.m_value = value;
        }
        /// <summary>
        /// Gets or sets the display.
        /// </summary>
        /// <value>The display.</value>
        public string Display
        {
            get { return this.m_display; }
            set { this.m_display = value; }
        }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get { return this.m_value; }
            set { this.m_value = value; }
        }

    }
}
