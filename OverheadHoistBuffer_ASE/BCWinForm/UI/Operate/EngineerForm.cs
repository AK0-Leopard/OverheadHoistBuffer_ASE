using com.mirle.ibg3k0.bc.winform;
using com.mirle.ibg3k0.bc.winform.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace com.mirle.ibg3k0.bc.winform.UI
{
    public partial class EngineerForm : Form
    {

        public delegate void SegmentSelectedEventHandler(string[] sSeg_ID);
        public event SegmentSelectedEventHandler evtSegmentSelected;
        public delegate void SectionSelectedEventHandler(string[] sSec_ID, string startAdr, string fromAdr, string toAdr);
        public event SectionSelectedEventHandler evtSectionSelected;
        BCMainForm mainForm;
        BCApplication bcApp;
        /// <summary>
        /// This Form Show
        /// </summary>
        public void PrcShow()
        {
            this.BringToFront();
            this.Show();
        }
        /// <summary>
        /// This Form Hide
        /// </summary>
        public void PrcHide()
        {
            this.Hide();
        }
        private int m_iMapSizeW = 0;
        public int p_MapSizeW
        {
            get { return (this.m_iMapSizeW); }
            set { this.m_iMapSizeW = value; }
        }

        private int m_iMapSizeH = 0;
        public int p_MapSizeH
        {
            get { return (this.m_iMapSizeH); }
            set { this.m_iMapSizeH = value; }
        }
        public EngineerForm(BCMainForm _mainForm)
        {
            InitializeComponent();
            mainForm = _mainForm;
            bcApp = mainForm.BCApp;
            string[] allAdr = loadAllAdr();
            cmb_fromAdr.DataSource = allAdr;
            cmb_fromAdr.AutoCompleteCustomSource.AddRange(allAdr);
            cmb_fromAdr.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmb_fromAdr.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmb_toAdr.DataSource = allAdr.ToArray();
            cmb_toAdr.AutoCompleteCustomSource.AddRange(allAdr);
            cmb_toAdr.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmb_toAdr.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmb_startAdr.DataSource = allAdr.ToArray();
            cmb_startAdr.AutoCompleteCustomSource.AddRange(allAdr);
            cmb_startAdr.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmb_startAdr.AutoCompleteSource = AutoCompleteSource.ListItems;

            string[] allSec = bcApp.SCApplication.MapBLL.loadAllSection().Select(sec => sec.SEC_ID).ToArray();
            cmb_fromSection.DataSource = allSec;
            cmb_fromSection.AutoCompleteCustomSource.AddRange(allSec);
            cmb_fromSection.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmb_fromSection.AutoCompleteSource = AutoCompleteSource.ListItems;



        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            string fromAdr = cmb_fromAdr.Text;
            string toAdr = cmb_toAdr.Text;
            string[] Reutrn = bcApp.SCApplication.RouteGuide.DownstreamSearchSection(fromAdr, toAdr, 1);
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(Reutrn[0]))
                sb.AppendLine("SegmentClosed");
            else
            {
                var allRoute = Reutrn[1].Split(';');
                foreach (string route in allRoute)
                    sb.AppendLine(route);
                sb.AppendLine("<MinRoute>");
                sb.AppendLine(Reutrn[0]);
            }
            txt_Route.Text = sb.ToString();

            var minRoute = Reutrn[0].Split('=');
            string[] minRouteSeg = minRoute[0].Split(',');
            evtSegmentSelected(minRouteSeg);
        }

        private string[] loadAllAdr()
        {
            string[] allAdrID = null;
            allAdrID = bcApp.SCApplication.MapBLL.loadAllAddress().Select(adr => adr.ADR_ID).ToArray();
            return allAdrID;
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            if (evtSegmentSelected != null)
            {
                evtSegmentSelected(new string[0]);
            }
            if (evtSectionSelected != null)
            {
                evtSectionSelected(new string[0], string.Empty, string.Empty, string.Empty);
            }
            this.PrcHide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bcApp.SCApplication.RouteGuide.OpenSegment(txt_open.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bcApp.SCApplication.RouteGuide.CloseSegment(txt_Close.Text);

        }

        private void btn_StartSec_Click(object sender, EventArgs e)
        {
            string startAdr = cmb_startAdr.Text;
            string fromAdr = cmb_fromAdr.Text;
            string toAdr = cmb_toAdr.Text;

            var test = bcApp.SCApplication.GuideBLL.getGuideInfo(fromAdr, toAdr);
            var guide_info = bcApp.SCApplication.CMDBLL.FindGuideInfo(startAdr, fromAdr, toAdr, sc.ProtocolFormat.OHTMessage.ActiveType.Loadunload);

            string[] ReutrnStart2From = new string[0];
            string[] ReutrnFrom2To = bcApp.SCApplication.RouteGuide.DownstreamSearchSection(fromAdr, toAdr, 1);
            StringBuilder sb = new StringBuilder();
            if (startAdr != fromAdr)
            {
                ReutrnStart2From = bcApp.SCApplication.RouteGuide.DownstreamSearchSection(startAdr, fromAdr, 1);
                if (string.IsNullOrEmpty(ReutrnStart2From[0]))
                    sb.AppendLine("ReutrnStart2From,SegmentClosed");
                else
                {
                    var allRoute = ReutrnStart2From[1].Split(';');
                    foreach (string route in allRoute)
                        sb.AppendLine(route);
                    sb.AppendLine("ReutrnStart2From,<MinRoute>");
                    sb.AppendLine(ReutrnStart2From[0]);
                }
            }

            if (string.IsNullOrEmpty(ReutrnFrom2To[0]))
                sb.AppendLine("ReutrnFrom2To,SegmentClosed");
            else
            {
                var allRoute = ReutrnFrom2To[1].Split(';');
                foreach (string route in allRoute)
                    sb.AppendLine(route);
                sb.AppendLine("ReutrnFrom2To,<MinRoute>");
                sb.AppendLine(ReutrnFrom2To[0]);
            }
            txt_Route.Text = sb.ToString();
            string[] minRoute_Start2From = new string[0];
            string[] minRouteSeg_Start2From = new string[0];
            if (ReutrnStart2From.Count() > 0)
            {
                minRoute_Start2From = ReutrnStart2From[0].Split('=');
                minRouteSeg_Start2From = minRoute_Start2From[0].Split(',');
            }

            var minRoute_From2To = ReutrnFrom2To[0].Split('=');
            string[] minRouteSeg_From2To = minRoute_From2To[0].Split(',');

            List<string> minRouteSeg = new List<string>();
            minRouteSeg.AddRange(minRouteSeg_Start2From);
            minRouteSeg.AddRange(minRouteSeg_From2To);

            //if (!string.IsNullOrWhiteSpace(minRoute_From2To[0]))
            //    evtSectionSelected(minRouteSeg.ToArray(), startAdr, fromAdr, toAdr);
        }

        private void btn_seachSec2Adr_Click(object sender, EventArgs e)
        {
            string[] ReutrnVh2FromAdr = bcApp.SCApplication.RouteGuide.DownstreamSearchSection_FromSecToAdr
      (cmb_fromSection.Text, cmb_toAdr.Text, 1);
            string svh2FromAdr = (ReutrnVh2FromAdr != null && ReutrnVh2FromAdr.Count() > 0) ? ReutrnVh2FromAdr[0] : string.Empty;
            txt_Route.Text = svh2FromAdr;
        }

        private void EngineerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainForm.removeForm(typeof(EngineerForm).Name);
        }
    }
}
