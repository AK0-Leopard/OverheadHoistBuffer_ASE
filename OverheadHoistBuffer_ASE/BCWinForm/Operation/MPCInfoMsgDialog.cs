using com.mirle.ibg3k0.bc.winform.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;
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

namespace com.mirle.ibg3k0.bc.winform.UI
{
    public partial class MPCInfoMsgDialog : Form
    {
        private BCMainForm mainForm = null;
        private BCApplication bcApp = null;
        private CommonInfo commonInfo = null;
        public readonly static string HandlerID = "MPCInfoMsgDialog";
        private BindingSource dataBS = new BindingSource();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MPCInfoMsgDialog(BCMainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            bcApp = mainForm.BCApp;
            commonInfo = bcApp.SCApplication.getEQObjCacheManager().CommonInfo;
        }

        private void MPCInfoMsgDialog_Load(object sender, EventArgs e)
        {
            //            m_confirmBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            m_TipMsgDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.PaleTurquoise;//奇數列顏色
            init();
            m_TipMsgDGV.Columns[0].MinimumWidth = 185;
            com.mirle.ibg3k0.sc.Common.SCUtility.subInitGrdHeader(m_TipMsgDGV);
            m_TipMsgDGV.AutoResizeColumns();
            m_TipMsgDGV.AutoResizeRows();
        }

        private void init()
        {
            //註冊監聽事件
            commonInfo.addEventHandler(HandlerID, BCFUtility.getPropertyName(() => commonInfo.MPCTipMsgList),
                (s1, e1) => refreshTable());
            //取得MPC Tip Messages，並載入到Tip Message DGV
            //this.m_TipMsgDGV.DataSource
            dataBS.DataSource = commonInfo.MPCTipMsgList;
            m_TipMsgDGV.DataSource = dataBS;
            dataBS.ResetBindings(false);
        }

        private void refreshTable()
        {
            Adapter.BeginInvoke(
                new SendOrPostCallback((o1) =>
                {
                    dataBS.ResetBindings(false);
                    if (m_TipMsgDGV.ColumnCount > 0)
                    {
                        DataGridViewColumn lastColumi = m_TipMsgDGV.Columns[m_TipMsgDGV.ColumnCount - 1];
                        lastColumi.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        lastColumi.MinimumWidth = lastColumi.Width;
                        lastColumi.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        m_TipMsgDGV.AutoResizeColumns();
                        m_TipMsgDGV.AutoResizeRows();

                        foreach (DataGridViewColumn dc in m_TipMsgDGV.Columns)
                        {
                            dc.Width = dc.Width + 10;
                        }
                    }

                }), null);


        }

        private void MPCInfoMsgDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            commonInfo.removeEventHandler(HandlerID);
            mainForm.removeForm("MPCInfoMsgDialog");
        }

        private void m_confirmBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
