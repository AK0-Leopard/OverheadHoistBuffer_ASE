//*********************************************************************************
//      BCBasicForm.cs
//*********************************************************************************
// File Name: BCBasicForm.cs
// Description: BC Basic Form類別
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************

using com.mirle.ibg3k0.bc.winform.App;
using com.mirle.ibg3k0.bc.winform.Common;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using com.mirle.ibg3k0.bcf.Common;

namespace com.mirle.ibg3k0.bc.winform.UI
{
    public partial class BCBasicForm : Form
    {
        protected string TitleName = string.Empty;

        public BCBasicForm()
        {
            InitializeComponent();
        }

        protected virtual void SubInit()
        {
            this.StartPosition = FormStartPosition.CenterScreen;

            if (!SCUtility.isEmpty(TitleName))
            {
                formTitleLb.Text = TitleName;
                this.Text = TitleName;
            }
        }

        private void btnlSearch_Click(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
