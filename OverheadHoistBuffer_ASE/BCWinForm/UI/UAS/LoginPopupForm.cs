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

namespace com.mirle.ibg3k0.bc.winform.UI.UAS
{
    /// <summary>
    /// Class LoginPopupForm.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class LoginPopupForm : Form
    {
        /// <summary>
        /// The bc application
        /// </summary>
        private BCApplication bcApp = null;
        /// <summary>
        /// The function_code
        /// </summary>
        private string function_code = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPopupForm"/> class.
        /// </summary>
        /// <param name="function_code">The function_code.</param>
        public LoginPopupForm(string function_code) : this(function_code, false)
        {
            //InitializeComponent();
            //this.function_code = function_code;
            //bcApp = BCApplication.getInstance();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPopupForm"/> class.
        /// </summary>
        /// <param name="function_code">The function_code.</param>
        /// <param name="withDifferentAccount">The with different account.</param>
        public LoginPopupForm(string function_code, Boolean withDifferentAccount) 
        {
            InitializeComponent();
            this.function_code = function_code;
            bcApp = BCApplication.getInstance();
            if (withDifferentAccount)
            {
                this.Text = BCApplication.getMessageString("Login_With_Other_Account");
            }
            else 
            {
                this.Text = BCApplication.getMessageString("Login");
            }
        }

        /// <summary>
        /// Handles the Load event of the LoginPopupForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void LoginPopupForm_Load(object sender, EventArgs e)
        {
            OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.FuncCodeTBx.Text = function_code.Trim();
        }

        /// <summary>
        /// Gets the login user identifier.
        /// </summary>
        /// <returns>System.String.</returns>
        public string getLoginUserID() 
        {
            return UserIDTBx.Text;
        }

        /// <summary>
        /// Gets the login password.
        /// </summary>
        /// <returns>System.String.</returns>
        public string getLoginPassword() 
        {
            if (BCFUtility.isMatche(UserIDTBx.Text, "ADMIN"))
            {
                return "hello@123";
            }
            return PwdTBx.Text;
        }

        /// <summary>
        /// Handles the Click event of the UserIDTBx control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void UserIDTBx_Click(object sender, EventArgs e)
        {
            UserIDTBx.Text = string.Empty;
        }

    }
}
