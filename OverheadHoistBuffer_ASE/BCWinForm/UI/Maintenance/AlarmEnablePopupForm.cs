// ***********************************************************************
// Assembly         : BC
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="LoginPopupForm.cs" company="Mirle">
//     Copyright ©2014 MIRLE.3K0
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using com.mirle.ibg3k0.bc.winform.App;
using com.mirle.ibg3k0.bcf.Common;

namespace com.mirle.ibg3k0.bc.winform.UI
{
    /// <summary>
    /// Class LoginPopupForm.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class AlarmEnablePopupForm : Form
    {
        string eqID = "";
        string alarmCode = "";
        public AlarmEnablePopupForm(string _eqID, string _alarmCode)
        {
            InitializeComponent();
            eqID = sc.Common.SCUtility.Trim(_eqID, true);
            alarmCode = sc.Common.SCUtility.Trim(_alarmCode, true);
        }

        /// <summary>
        /// Handles the Load event of the LoginPopupForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void LoginPopupForm_Load(object sender, EventArgs e)
        {
            OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.txt_EqID.Text = eqID.Trim();
            this.txt_AlarmCode.Text = alarmCode.Trim();
        }

        public string getUserID()
        {
            return txt_userID.Text;
        }
        public string getReason()
        {
            return txt_reason.Text;
        }


        private void OKBtn_Click(object sender, EventArgs e)
        {

        }
    }
}
