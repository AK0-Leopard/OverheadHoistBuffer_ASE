using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.bc.winform.Common;
using com.mirle.ibg3k0.bc.winform.App;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using NLog;
using com.mirle.ibg3k0.bc.winform.Properties;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.bc.winform.UI.Components
{
    public partial class uctl_Map : UserControl
    {
        int space_Height_m = 0;
        int space_Width_m = 0;
        int zoon_Factor = 0;
        int defaultMaxScale = 0;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private uctlRail[] m_objItemRail = null;
        int zInitialIndex = 0;
        Color RailOriginalColor = Color.Empty;

        private uctlAddress[] m_objItemAddr = null;
        private uctlVehicle[] m_objItemVhcl = null;
        private uctlNewVehicle[] m_objItemNewVhcl = null;
        private uctlPortNew[] m_objItemPortNew = null;
        private PictureBox[] m_objItemPic = null;
        List<Label> sectionLables = null;
        List<Label> addressLables = null;
        private Dictionary<string, GroupRails> m_DicSectionGroupRails;
        private Dictionary<string, List<GroupRails>> m_DicSegmentGroupRails;

        public Dictionary<string, GroupRails> p_DicSectionGroupRails { get { return m_DicSectionGroupRails; } }


        BCMainForm mainForm = null;
        public OHT_Form ohtc_Form = null;
        public uctl_Map()
        {
            InitializeComponent();
        }

        public void start(BCMainForm _main, OHT_Form _ohtc_Form)
        {
            mainForm = _main;
            ohtc_Form = _ohtc_Form;
            initialMapSpace();

            lbl_Through_Times_Lv1.BackColor = BCAppConstants.SEC_THROUGH_COLOR_LV1;
            lbl_Through_Times_Lv2.BackColor = BCAppConstants.SEC_THROUGH_COLOR_LV2;
            lbl_Through_Times_Lv3.BackColor = BCAppConstants.SEC_THROUGH_COLOR_LV3;
            lbl_Through_Times_Lv4.BackColor = BCAppConstants.SEC_THROUGH_COLOR_LV4;
            lbl_Through_Times_Lv5.BackColor = BCAppConstants.SEC_THROUGH_COLOR_LV5;
            lbl_Through_Times_Lv6.BackColor = BCAppConstants.SEC_THROUGH_COLOR_LV6;
            lbl_Through_Times_Lv7.BackColor = BCAppConstants.SEC_THROUGH_COLOR_LV7;
            lbl_Through_Times_Lv8.BackColor = BCAppConstants.SEC_THROUGH_COLOR_LV8;
            lbl_Through_Times_Lv9.BackColor = BCAppConstants.SEC_THROUGH_COLOR_LV9;
            lbl_Through_Times_Lv10.BackColor = BCAppConstants.SEC_THROUGH_COLOR_LV10;
            lbl_Through_Times_Lv1.Text = $"{0} ~ {BCAppConstants.SEC_THROUGH_TIMES_LV1}";
            lbl_Through_Times_Lv2.Text = $"{BCAppConstants.SEC_THROUGH_TIMES_LV1} ~ {BCAppConstants.SEC_THROUGH_TIMES_LV2}";
            lbl_Through_Times_Lv3.Text = $"{BCAppConstants.SEC_THROUGH_TIMES_LV2} ~ {BCAppConstants.SEC_THROUGH_TIMES_LV3}";
            lbl_Through_Times_Lv4.Text = $"{BCAppConstants.SEC_THROUGH_TIMES_LV3} ~ {BCAppConstants.SEC_THROUGH_TIMES_LV4}";
            lbl_Through_Times_Lv5.Text = $"{BCAppConstants.SEC_THROUGH_TIMES_LV4} ~ {BCAppConstants.SEC_THROUGH_TIMES_LV5}";
            lbl_Through_Times_Lv6.Text = $"{BCAppConstants.SEC_THROUGH_TIMES_LV5} ~ {BCAppConstants.SEC_THROUGH_TIMES_LV6}";
            lbl_Through_Times_Lv7.Text = $"{BCAppConstants.SEC_THROUGH_TIMES_LV6} ~ {BCAppConstants.SEC_THROUGH_TIMES_LV7}";
            lbl_Through_Times_Lv8.Text = $"{BCAppConstants.SEC_THROUGH_TIMES_LV7} ~ {BCAppConstants.SEC_THROUGH_TIMES_LV8}";
            lbl_Through_Times_Lv9.Text = $"{BCAppConstants.SEC_THROUGH_TIMES_LV8} ~ {BCAppConstants.SEC_THROUGH_TIMES_LV9}";
            lbl_Through_Times_Lv10.Text = $"{BCAppConstants.SEC_THROUGH_TIMES_LV9} ~ ";



            readRailDatas();
            readAddressPortDatas();
            //readVehicleDatas();
            readNewVehicleDatas();
            readPortIconDatas();

            m_DicSectionGroupRails = new Dictionary<string, GroupRails>();
            m_DicSegmentGroupRails = new Dictionary<string, List<GroupRails>>();
            BidingRailPointAndGroupForRail();
            BidingGroupRailsAndAddress();
            BidingGroupRailsLable();
            BidingAddressLable();
            adjustTheLayerOrder();
            if (m_objItemRail != null && m_objItemRail.Count() > 0)
            {
                RailOriginalColor = m_objItemRail[0].p_RailColor;
                zInitialIndex = pnl_Map.Controls.GetChildIndex(m_objItemRail.Last());
            }
            List<string> selectSeg = new List<string>();
            selectSeg.Add("");
            selectSeg.AddRange(m_DicSegmentGroupRails.Keys.ToList());
            cmb_selectSeg.DataSource = selectSeg;
            tmrRefresh.Start();
            BCUtility.setScale(defaultMaxScale, zoon_Factor);
            ratioChanges();

        }

        private void initialMapSpace()
        {
            if (BCFUtility.isMatche(mainForm.BCApp.SCApplication.BC_ID, SCAppConstants.WorkVersion.VERSION_NAME_ASE))
            {
                space_Height_m = 14000;
                space_Width_m = 40000;
                zoon_Factor = 100;
                defaultMaxScale = 10;
                trackBar_scale.SmallChange = 2;
            }
            else if (BCFUtility.isMatche(mainForm.BCApp.SCApplication.BC_ID, SCAppConstants.WorkVersion.VERSION_NAME_ASE_LOOP))
            {
                space_Height_m = 14000;
                space_Width_m = 53000;
                zoon_Factor = 100;
                defaultMaxScale = 10;
                trackBar_scale.SmallChange = 2;
            }
            else if (BCFUtility.isMatche(mainForm.BCApp.SCApplication.BC_ID, SCAppConstants.WorkVersion.VERSION_NAME_ASE_LINE3))
            {
                space_Height_m = 14000;
                space_Width_m = 50000;
                zoon_Factor = 100;
                defaultMaxScale = 10;
                trackBar_scale.SmallChange = 2;
            }
            else if(BCFUtility.isMatche(mainForm.BCApp.SCApplication.BC_ID, SCAppConstants.WorkVersion.VERSION_NAME_ASE_TEST))
            {
                space_Height_m = 14000;
                space_Width_m = 40000;
                zoon_Factor = 100;
                defaultMaxScale = 10;
                trackBar_scale.SmallChange = 2;
            }
            else
            {
                space_Height_m = 14000;
                space_Width_m = 40000;
                zoon_Factor = 100;
                defaultMaxScale = 10;
            }
            trackBar_scale.Maximum = defaultMaxScale;
            trackBar_scale.Value = defaultMaxScale;
            lbl_maxScale.Text = defaultMaxScale.ToString();
            //BCUtility.setScale(trackBar_scale.Value);
            BCUtility.setScale(10, zoon_Factor);
            double space_Height_PixelsHeight = BCUtility.RealLengthToPixelsWidthByScale(space_Height_m);
            double space_Height_PixelsWidth = BCUtility.RealLengthToPixelsWidthByScale(space_Width_m);
            pnl_Map.Height = (int)space_Height_PixelsHeight;
            pnl_Map.Width = (int)space_Height_PixelsWidth;
            this.pnl_Map.Resize += new System.EventHandler(this.pnl_Map_Resize);
            pnl_Map.Tag = pnl_Map.Height + "|" + pnl_Map.Width;

        }

        private bool readRailDatas()
        {
            bool bRet = false;
            int iRailDatasCount = 0;
            try
            {
                IEnumerable<ARAIL> enumerRail = null;
                enumerRail = mainForm.BCApp.SCApplication.MapBLL.loadAllRail();
                iRailDatasCount = enumerRail.Count();
                if (iRailDatasCount > 0)
                {
                    m_objItemRail = new uctlRail[iRailDatasCount];

                    int index = 0;
                    foreach (ARAIL rail in enumerRail)
                    {
                        m_objItemRail[index] = new uctlRail();
                        m_objItemRail[index].p_Num = index + 1;
                        m_objItemRail[index].p_ID = rail.RAIL_ID;
                        m_objItemRail[index].p_RailType = rail.RAILTYPE;
                        m_objItemRail[index].p_StrMapRailInfo = rail;
                        m_objItemRail[index].p_RailWidth = (int)rail.WIDTH;
                        m_objItemRail[index].p_RailColor = BCUtility.ConvStr2Color(rail.COLOR);
                        m_objItemRail[index].p_LocX = rail.LOCATIONX;
                        m_objItemRail[index].p_LocY = rail.LOCATIONY;
                        m_objItemRail[index].p_RailLength = (int)rail.LENGTH;
                        if (m_objItemRail[index].p_RailType == E_RAIL_TYPE.Straight_Vertical)
                        {
                            m_objItemRail[index].Tag = m_objItemRail[index].Top + "|" + m_objItemRail[index].Left + "|"
                                + m_objItemRail[index].Height + "|" + m_objItemRail[index].p_RailWidth;
                        }
                        else
                        {
                            m_objItemRail[index].Tag = m_objItemRail[index].Top + "|" + m_objItemRail[index].Left + "|"
                                + m_objItemRail[index].Width + "|" + m_objItemRail[index].p_RailWidth;
                        }

                        if (m_objItemRail[index].p_RailType == E_RAIL_TYPE.Arrow_Up ||
                            m_objItemRail[index].p_RailType == E_RAIL_TYPE.Arrow_Down ||
                            m_objItemRail[index].p_RailType == E_RAIL_TYPE.Arrow_Left ||
                            m_objItemRail[index].p_RailType == E_RAIL_TYPE.Arrow_Right)
                        {
                            m_objItemRail[index].Visible = false;
                        }
                        index++;
                    }
                    this.pnl_Map.Controls.AddRange(m_objItemRail);
                }
                bRet = true;
            }
            catch (Exception ex)
            {
                bRet = false;
                logger.Error(ex, "Exception");
            }
            return (bRet);
        }
        private bool readAddressPortDatas()
        {
            bool bRet = false;
            int iAddrCount = 0;
            try
            {
                IEnumerable<AADDRESS> enumerAdr = mainForm.BCApp.SCApplication.MapBLL.loadAllAddress();

                iAddrCount = enumerAdr.Count();

                int index = 0;
                m_objItemAddr = new uctlAddress[iAddrCount];
                foreach (AADDRESS adr in enumerAdr)
                {
                    m_objItemAddr[index] = new uctlAddress(this);
                    m_objItemAddr[index].p_AddrPt = index;
                    m_objItemAddr[index].p_Address = adr.ADR_ID.Trim();
                    APOINT point = mainForm.BCApp.SCApplication.MapBLL.getPointByID(adr.ADR_ID);
                    if (point != null)
                    {
                        m_objItemAddr[index].str_Map_Add_Info = point;
                        m_objItemAddr[index].p_LayoutPoint = 0;
                        m_objItemAddr[index].p_Text = string.Empty;
                        m_objItemAddr[index].p_PointType = point.POINTTYPE;
                        m_objItemAddr[index].p_LocX = point.LOCATIONX;
                        m_objItemAddr[index].p_LocY = point.LOCATIONY;
                        m_objItemAddr[index].p_SizeW = (int)point.WIDTH;
                        m_objItemAddr[index].p_SizeH = (int)point.HEIGHT;
                        m_objItemAddr[index].p_Color = BCUtility.ConvStr2Color(point.COLOR);
                        m_objItemAddr[index].p_ZoomLV = adr.ZOOM_LV;
                        m_objItemAddr[index].Visible = adr.ZOOM_LV >= trackBar_scale.Value;
                        m_objItemAddr[index].Tag = m_objItemAddr[index].Top + "|" + m_objItemAddr[index].Left + "|"
                            + m_objItemAddr[index].Height + "|" + m_objItemAddr[index].Width;
                    }
                    index++;
                }

                if (iAddrCount > 0) this.pnl_Map.Controls.AddRange(m_objItemAddr);

                bRet = true;
            }
            catch (Exception ex)
            {
                bRet = false;
                logger.Error(ex, "Exception");
            }
            return (bRet);
        }

        private bool readPortIconDatas()
        {
            bool bRet = false;
            int iPortIconCount = 0;
            try
            {
                IEnumerable<APORTICON> enumerportIcon = mainForm.BCApp.SCApplication.MapBLL.loadAllPortIcon();

                iPortIconCount = enumerportIcon.Count();

                int index = 0;
                m_objItemPortNew = new uctlPortNew[iPortIconCount];
                foreach (APORTICON adr in enumerportIcon)
                {
                    m_objItemPortNew[index] = new uctlPortNew();
                    m_objItemPortNew[index].p_PortName = adr.PORT_ID;
                    m_objItemPortNew[index].p_Address = adr.ADR_ID;
                    m_objItemPortNew[index].p_LocX = adr.LOCATIONX;
                    m_objItemPortNew[index].p_LocY = adr.LOCATIONY;
                    m_objItemPortNew[index].p_SizeH = adr.HEIGHT;
                    m_objItemPortNew[index].p_SizeW = adr.WIDTH;
                    m_objItemPortNew[index].p_Color = BCUtility.ConvStr2Color(adr.COLOR);

                    m_objItemPortNew[index].Tag = m_objItemPortNew[index].Top + "|" + m_objItemPortNew[index].Left + "|"
                                                 + m_objItemPortNew[index].Height + "|" + m_objItemPortNew[index].Width;

                    index++;
                }

                if (iPortIconCount > 0) this.pnl_Map.Controls.AddRange(m_objItemPortNew);

                bRet = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                bRet = false;
            }
            return (bRet);
        }

        private bool readVehicleDatas()
        {
            bool bRet = false;
            int iVhSize = 32;
            int iVhclCount = 0;
            Font objFont = new Font("Verdana", 5, FontStyle.Regular);

            try
            {
                //List<Equipment> lstEq = mainForm.BCApp.SCApplication.getEQObjCacheManager().getAllEquipment();
                List<AVEHICLE> lstEq = mainForm.BCApp.SCApplication.getEQObjCacheManager().getAllVehicle();
                iVhclCount = lstEq.Count;

                m_objItemVhcl = new uctlVehicle[iVhclCount];

                for (int ii = 0; ii < iVhclCount; ii++)
                {
                    m_objItemVhcl[ii] = new uctlVehicle(lstEq[ii], this);
                    m_objItemVhcl[ii].p_VhPt = ii;
                    m_objItemVhcl[ii].p_Num = ii + 65;
                    m_objItemVhcl[ii].p_SizeW = m_objItemVhcl[ii].Width;
                    m_objItemVhcl[ii].p_SizeH = m_objItemVhcl[ii].Height;
                    m_objItemVhcl[ii].p_LocX = iVhSize / 2;
                    m_objItemVhcl[ii].p_LocY = iVhSize / 2;
                    m_objItemVhcl[ii].p_Presence = false;
                    m_objItemVhcl[ii].p_Status = E_MAP_VHSTS.enDisconnect;
                    m_objItemVhcl[ii].p_Font = objFont;
                    m_objItemVhcl[ii].Tag = m_objItemVhcl[ii].Top + "|" + m_objItemVhcl[ii].Left + "|"
                        + m_objItemVhcl[ii].Height + "|" + m_objItemVhcl[ii].Width;
                }
                if (iVhclCount > 0) this.pnl_Map.Controls.AddRange(m_objItemVhcl);
                bRet = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                bRet = false;
            }

            return (bRet);
        }


        private bool readNewVehicleDatas()
        {
            bool bRet = false;
            int iVhclCount = 0;
            try
            {
                List<AVEHICLE> lstEq = mainForm.BCApp.SCApplication.getEQObjCacheManager().getAllVehicle();
                iVhclCount = lstEq.Count;

                m_objItemNewVhcl = new uctlNewVehicle[iVhclCount];
                m_objItemPic = new PictureBox[iVhclCount * 2];

                for (int ii = 0; ii < iVhclCount; ii++)
                {
                    PictureBox pic_AlarmStatus = new PictureBox();
                    //pic_AlarmStatus.Size = new Size(64, 48);
                    pic_AlarmStatus.SizeMode = PictureBoxSizeMode.Zoom;
                    pic_AlarmStatus.BackColor = Color.Transparent;
                    pic_AlarmStatus.Visible = false;


                    PictureBox pic_CstIcon = new PictureBox();
                    pic_CstIcon.Size =
                        new Size(Resources.Action__Cassette_.Size.Width / 2, Resources.Action__Cassette_.Size.Height / 2);
                    pic_CstIcon.SizeMode = PictureBoxSizeMode.Zoom;
                    pic_CstIcon.BackColor = Color.Transparent;
                    pic_CstIcon.Visible = false;
                    pic_CstIcon.Image = Resources.Action__Cassette_;

                    m_objItemNewVhcl[ii] = new uctlNewVehicle(lstEq[ii], this, pic_AlarmStatus, pic_CstIcon);
                    m_objItemNewVhcl[ii].Num = ii + 1;
                    //m_objItemNewVhcl[ii].p_SizeW = m_objItemVhcl[ii].Width;
                    //m_objItemNewVhcl[ii].p_SizeH = m_objItemVhcl[ii].Height;
                    //m_objItemNewVhcl[ii].Width = Resources.Vehicle__Unconnected_.Width / 2;
                    //m_objItemNewVhcl[ii].Height = Resources.Vehicle__Unconnected_.Height / 2;
                    //m_objItemNewVhcl[ii].Left = m_objItemNewVhcl[ii].Width / 2;
                    //m_objItemNewVhcl[ii].Top = m_objItemNewVhcl[ii].Height / 2;
                    // m_objItemNewVhcl[ii].p_Presence = false;
                    m_objItemPic[ii * 2] = pic_AlarmStatus;
                    m_objItemPic[(ii * 2) + 1] = pic_CstIcon;
                }
                if (iVhclCount > 0) this.pnl_Map.Controls.AddRange(m_objItemNewVhcl);
                if (m_objItemPic.Length > 0) this.pnl_Map.Controls.AddRange(m_objItemPic);
                bRet = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                bRet = false;
            }

            return (bRet);
        }

        private void adjustTheLayerOrder()
        {
            if (m_objItemRail != null)
            {
                for (int ii = 0; ii < m_objItemRail.Length; ii++)
                {
                    m_objItemRail[ii].BringToFront();
                }
            }

            if (m_objItemAddr != null)
            {
                for (int ii = 0; ii < m_objItemAddr.Length; ii++)
                {
                    m_objItemAddr[ii].BringToFront();
                }
            }

            if (m_objItemVhcl != null)
            {
                for (int ii = 0; ii < m_objItemVhcl.Length; ii++)
                {
                    m_objItemVhcl[ii].BringToFront();
                }
            }

            if (m_objItemNewVhcl != null)
            {
                for (int ii = 0; ii < m_objItemNewVhcl.Length; ii++)
                {
                    m_objItemNewVhcl[ii].BringToFront();
                }
            }
            if (sectionLables != null)
            {
                for (int ii = 0; ii < sectionLables.Count(); ii++)
                {
                    sectionLables[ii].BringToFront();
                }
            }
            if (addressLables != null)
            {
                for (int ii = 0; ii < addressLables.Count(); ii++)
                {
                    addressLables[ii].BringToFront();
                }
            }
            if (m_objItemPic != null)
            {
                for (int ii = 0; ii < m_objItemPic.Count(); ii++)
                {
                    m_objItemPic[ii].BringToFront();
                }
            }
        }
        private bool BidingRailPointAndGroupForRail()
        {
            bool bRet = false;
            List<AGROUPRAILS> listGroup = mainForm.BCApp.SCApplication.MapBLL.loadAllGroupRail();

            for (int i = 0; i < listGroup.Count - 1; i++)
            {
                if (!BCFUtility.isMatche(listGroup[i].SECTION_ID, listGroup[i + 1].SECTION_ID))
                    continue;
                findMatchRailPoint(listGroup[i].RAIL_ID, listGroup[i + 1].RAIL_ID);
            }

            foreach (AGROUPRAILS group_rail in listGroup)
            {
                uctlRail railTemp = null;
                string section_id = group_rail.SECTION_ID.Trim();
                railTemp = m_objItemRail.Where(r => r.p_ID.Trim() == group_rail.RAIL_ID.Trim()).FirstOrDefault();
                if (railTemp == null)
                    continue;
                if (!m_DicSectionGroupRails.ContainsKey(section_id))
                {

                    ASECTION ASEC_ObJ = mainForm.BCApp.SCApplication.MapBLL.getSectiontByID(section_id);
                    ASEGMENT ASEG_ObJ = mainForm.BCApp.SCApplication.MapBLL.getSegmentByID(ASEC_ObJ.SEG_NUM);
                    m_DicSectionGroupRails.Add(group_rail.SECTION_ID.Trim(), new GroupRails(group_rail.SECTION_ID, ASEC_ObJ.SEC_DIS, group_rail.DIR
                                                                                            , ASEG_ObJ.SEG_NUM, ASEG_ObJ.DIR));
                }
                m_DicSectionGroupRails[section_id].uctlRails.Add(new KeyValuePair<uctlRail, E_RAIL_DIR>(railTemp, group_rail.DIR));
            }

            foreach (GroupRails group in m_DicSectionGroupRails.Values)
            {
                group.RefreshDistance();
                group.DecisionFirstAndLastPoint(true);

            }

            List<ASEGMENT> listSegment = mainForm.BCApp.SCApplication.MapBLL.loadAllSegments();
            foreach (ASEGMENT seg in listSegment)
            {
                List<GroupRails> segSections = m_DicSectionGroupRails.Values
                    .Where(group => group.Segment_Num.Trim() == seg.SEG_NUM.Trim()).ToList();
                m_DicSegmentGroupRails.Add(seg.SEG_NUM.Trim(), segSections);
            }
            return (bRet);
        }
        private bool BidingGroupRailsAndAddress()
        {
            bool bRet = false;
            List<string> secIDs = mainForm.BCApp.SCApplication.MapBLL.loadAllSectionID();
            uctlAddress addressTemp = null;
            foreach (string sec_id in secIDs)
            {
                ASECTION section = mainForm.BCApp.SCApplication.MapBLL.getSectiontByID(sec_id);
                if (section == null)
                    continue;
                addressTemp = m_objItemAddr.Where(a => a.p_Address.Trim() == section.FROM_ADR_ID.Trim()).FirstOrDefault();
                if (addressTemp != null)
                    m_DicSectionGroupRails[section.SEC_ID.Trim()].p_Points[0].BindingAddress(addressTemp, false);
                addressTemp = m_objItemAddr.Where(a => a.p_Address.Trim() == section.TO_ADR_ID.Trim()).FirstOrDefault();
                if (addressTemp != null)
                    m_DicSectionGroupRails[section.SEC_ID.Trim()].p_Points[1].BindingAddress(addressTemp, true);
            }
            return (bRet);
        }
        private bool BidingGroupRailsLable()
        {
            bool bRet = false;
            sectionLables = new List<Label>();

            foreach (GroupRails group in m_DicSectionGroupRails.Values)
            {
                sectionLables.Add(group.getSectionLable());
            }
            if (sectionLables.Count > 0)
                this.pnl_Map.Controls.AddRange(sectionLables.ToArray());

            return (bRet);
        }
        private bool BidingAddressLable()
        {
            bool bRet = false;
            addressLables = new List<Label>();

            if (m_objItemAddr != null)
            {
                for (int ii = 0; ii < m_objItemAddr.Length; ii++)
                {
                    if (m_objItemAddr[ii].p_LocX <= 10)
                        continue;
                    addressLables.Add(m_objItemAddr[ii].getLable());
                }
            }
            if (addressLables.Count > 0)
                this.pnl_Map.Controls.AddRange(addressLables.ToArray());

            return (bRet);
        }

        private void findMatchRailPoint(string rail_id, string next_rail_id)
        {
            uctlRail railTemp = null;
            uctlRail NextrailTemp = null;
            railTemp = m_objItemRail.Where(r => r.p_ID.Trim() == rail_id.Trim()).FirstOrDefault();
            NextrailTemp = m_objItemRail.Where(r => r.p_ID.Trim() == next_rail_id.Trim()).FirstOrDefault();
            if (railTemp == null || NextrailTemp == null)
                return;
            foreach (PointObject point in railTemp.p_Points)
            {
                foreach (PointObject next_point in NextrailTemp.p_Points)
                {
                    bool isMatch = pointIsMatch(point.RealPointf, next_point.RealPointf);
                    if (isMatch)
                    {
                        point.BindingPoint(next_point);
                        next_point.BindingPoint(point);
                    }
                }
            }
        }
        private bool pointIsMatch(PointF point, PointF next_point)
        {
            double basePoint_X = point.X;
            double basePoint_Y = point.Y;
            int rangeValue = 5;
            if (Math.Round(next_point.X) > basePoint_X - rangeValue
                && Math.Round(next_point.X) < basePoint_X + rangeValue)
            {
                if (Math.Round(next_point.Y) > basePoint_Y - rangeValue
                    && Math.Round(next_point.Y) < basePoint_Y + rangeValue)
                {
                    return true;
                }
            }
            //if (Math.Round(point.X) == Math.Round(next_point.X)
            //    && Math.Round(point.Y) == Math.Round(next_point.Y))
            //{
            //    return true;
            //}
            return false;
        }

        private void trackBar_scale_Scroll(object sender, EventArgs e)
        {
            BCUtility.setScale(trackBar_scale.Value, zoon_Factor);
            ratioChanges();
        }
        private void ratioChanges()
        {
            double space_Height_PixelsHeight = BCUtility.RealLengthToPixelsWidthByScale(space_Height_m);
            double space_Height_PixelsWidth = BCUtility.RealLengthToPixelsWidthByScale(space_Width_m);
            pnl_Map.Size = new Size((int)space_Height_PixelsWidth, (int)space_Height_PixelsHeight);
        }
        private void pnl_Map_Resize(object sender, EventArgs e)
        {
            if (pnl_Map.Tag == null || m_objItemRail == null) return;
            double scaleWidth = (pnl_Map.Width / double.Parse(pnl_Map.Tag.ToString().Split('|')[1]));
            double scaleHeigh = (pnl_Map.Height / double.Parse(pnl_Map.Tag.ToString().Split('|')[0]));
            foreach (uctlRail rail in m_objItemRail)
            {
                int rail_width = (int)(double.Parse(rail.Tag.ToString().Split('|')[3]) * scaleWidth);
                rail.setWidthByZoonInZoonOut(rail_width);
                int rail_length = (int)(double.Parse(rail.Tag.ToString().Split('|')[2]) * scaleHeigh);
                rail.setLengthByZoonInZoonOut(rail_length);
                rail.p_LocX = (int)(double.Parse(rail.Tag.ToString().Split('|')[1]) * scaleWidth);
                rail.p_LocY = (int)(double.Parse(rail.Tag.ToString().Split('|')[0]) * scaleHeigh);

            }

            //foreach (uctlEquipment eqpt in m_objItemEquip)
            //{
            //    eqpt.Left = (int)(double.Parse(eqpt.Tag.ToString().Split('|')[1]) * scaleWidth);
            //    eqpt.Top = (int)(double.Parse(eqpt.Tag.ToString().Split('|')[0]) * scaleHeigh);
            //}

            foreach (uctlAddress add in m_objItemAddr)
            {
                add.Visible = add.p_ZoomLV >= trackBar_scale.Value;
                add.p_SizeW = (int)(double.Parse(add.Tag.ToString().Split('|')[3]) * scaleWidth);
                add.p_SizeH = (int)(double.Parse(add.Tag.ToString().Split('|')[2]) * scaleHeigh);
                add.Left = (int)(double.Parse(add.Tag.ToString().Split('|')[1]) * scaleWidth);
                add.Top = (int)(double.Parse(add.Tag.ToString().Split('|')[0]) * scaleHeigh);
            }


            if (m_objItemVhcl != null)
            {
                foreach (uctlVehicle vh in m_objItemVhcl)
                {
                    vh.Width = (int)(double.Parse(vh.Tag.ToString().Split('|')[3]) * scaleWidth);
                    vh.Height = (int)(double.Parse(vh.Tag.ToString().Split('|')[2]) * scaleHeigh);
                    vh.Left = (int)(double.Parse(vh.Tag.ToString().Split('|')[1]) * scaleWidth);
                    vh.Top = (int)(double.Parse(vh.Tag.ToString().Split('|')[0]) * scaleHeigh);
                }
            }

            foreach (uctlPortNew port in m_objItemPortNew)
            {
                port.Width = (int)(double.Parse(port.Tag.ToString().Split('|')[3]) * scaleWidth);
                port.Height = (int)(double.Parse(port.Tag.ToString().Split('|')[2]) * scaleHeigh);
                port.Left = (int)(double.Parse(port.Tag.ToString().Split('|')[1]) * scaleWidth);
                port.Top = (int)(double.Parse(port.Tag.ToString().Split('|')[0]) * scaleHeigh);
            }

            foreach (GroupRails group in m_DicSectionGroupRails.Values)
            {
                group.DecisionFirstAndLastPoint(false);
            }
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        #region Rail Flashing Control
        string current_SelectSegment = "";
        List<string[]> current_FlashingSectionGroup = new List<string[]>();
        Boolean change_flag = false;
        private void tmrRefresh_Tick(object sender, EventArgs e)
        {
            change_flag = !change_flag;
            if (!BCFUtility.isEmpty(current_SelectSegment))
            {
                List<GroupRails> lstGroupRails = m_DicSegmentGroupRails[current_SelectSegment];
                foreach (GroupRails groupRails in lstGroupRails)
                {
                    groupRails.GroupColorChange(change_flag ? RailOriginalColor : Color.Yellow);
                }
            }

            if (current_FlashingSectionGroup != null
                && current_FlashingSectionGroup.Count() > 0)
            {
                foreach (string[] section_ids in current_FlashingSectionGroup)
                {
                    foreach (string section_id in section_ids)
                    {
                        m_DicSectionGroupRails[section_id]
                            .GroupColorChange(change_flag ? RailOriginalColor : Color.Yellow);
                    }
                }
            }
        }

        private void cmb_selectSeg_SelectedIndexChanged(object sender, EventArgs e)
        {
            resetRailColor(current_SelectSegment);

            current_SelectSegment = cmb_selectSeg.Text;

            RailBringToFrontBySegment(current_SelectSegment);
        }


        public void startFlashingSpecifyRail(string[] sectionGroup)
        {
            RailBringToFrontBySectionGroup(sectionGroup);
            current_FlashingSectionGroup.Add(sectionGroup);
        }
        public void stopFlashingSpecifyRail(string[] section_ids)
        {
            current_FlashingSectionGroup.Remove(section_ids);
        }

        public void resetRailColor(string SelectSegment)
        {
            if (!BCFUtility.isEmpty(SelectSegment))
            {
                foreach (GroupRails groupRails in m_DicSegmentGroupRails[SelectSegment.Trim()])
                {
                    groupRails.GroupColorChange(RailOriginalColor);
                }
            }
        }
        public void resetRailColor(string[] lstSec)
        {
            if (!BCFUtility.isEmpty(lstSec))
            {
                foreach (string groupRails in lstSec)
                {
                    if (m_DicSectionGroupRails.ContainsKey(groupRails))
                        m_DicSectionGroupRails[groupRails].GroupColorChange(RailOriginalColor);
                }
            }
        }
        public void resetAllRailColor()
        {
            foreach (var keyValue in m_DicSectionGroupRails)
            {
                keyValue.Value.GroupColorChange(RailOriginalColor);
            }
        }
        private void RailBringToFrontBySegment(string SelectSegment)
        {
            if (SCUtility.isEmpty(SelectSegment))
                return;
            int count = 0;
            foreach (GroupRails groupRails in m_DicSegmentGroupRails[SelectSegment])
            {
                foreach (var rail in groupRails.uctlRails)
                {
                    pnl_Map.Controls.SetChildIndex(rail.Key, zInitialIndex + count++);
                }
            }
        }

        private void RailBringToFrontBySectionGroup(string[] SelectSections)
        {
            int count = 0;
            foreach (string section in SelectSections)
            {
                foreach (var rail in m_DicSectionGroupRails[section].uctlRails)
                {
                    Adapter.Invoke((obj) =>
                    {
                        pnl_Map.Controls.SetChildIndex(rail.Key, zInitialIndex + count++);
                    }, null);

                }
            }
        }
        #endregion Rail Flashing Control

        #region Rail Color Change

        public void changeSpecifyRailColorBySegNum(string seg_num, Color rail_color)
        {
            if (SCUtility.isEmpty(seg_num))
                return;
            List<ASECTION> listSec = mainForm.BCApp.SCApplication.MapBLL.loadSectionsBySegmentID(seg_num);
            foreach (ASECTION sec in listSec)
            {
                changeSpecifyRailColor(sec.SEC_ID.Trim(), rail_color);
            }
        }
        public void changeSpecifyRailColor(string section, Color rail_color)
        {
            if (SCUtility.isEmpty(section))
                return;
            if (m_DicSectionGroupRails.ContainsKey(section))
            {
                GroupRails groupRails = m_DicSectionGroupRails[section];
                groupRails.GroupColorChange(rail_color);
            }
        }

        public void changeSpecifyRailColor(string[] sectionGroup)
        {
            if (sectionGroup == null || sectionGroup.Count() == 0)
                return;
            RailBringToFrontBySectionGroup(sectionGroup);
            foreach (string sec in sectionGroup)
            {
                if (m_DicSectionGroupRails.ContainsKey(sec))
                {
                    GroupRails groupRails = m_DicSectionGroupRails[sec];
                    groupRails.GroupColorChange(Color.Yellow);
                }
            }
        }

        public void changeSpecifyRailColor(List<KeyValuePair<string, bool>> SectionsPassStatus)
        {
            string[] sections = null;
            if (SectionsPassStatus == null || SectionsPassStatus.Count() == 0)
                return;
            else
            {
                sections = SectionsPassStatus.Select(o => o.Key).ToArray();
            }
            RailBringToFrontBySectionGroup(sections);
            foreach (KeyValuePair<string, bool> secAndStatus in SectionsPassStatus)
            {
                if (m_DicSectionGroupRails.ContainsKey(secAndStatus.Key))
                {
                    GroupRails groupRails = m_DicSectionGroupRails[secAndStatus.Key];
                    if (!secAndStatus.Value)
                        groupRails.GroupColorChange(Color.Yellow);
                    else
                        groupRails.GroupColorChange(Color.Gray);
                }
            }
        }
        #endregion Rail Color Change
        #region Address Color change
        public void changeSpecifyAddressColor(string adr_id, Color change_color)
        {
            if (SCUtility.isEmpty(adr_id))
                return;
            uctlAddress uctAdr = m_objItemAddr.
                Where(adr_obj => adr_obj.p_Address == adr_id.Trim()).
                SingleOrDefault();
            if (uctAdr != null)
            {
                uctAdr.p_Color = change_color;
            }
        }
        #endregion Address Color change

        #region Vehicle Position Control
        private void btn_entry_Click(object sender, EventArgs e)
        {
            PrcSetVehicleToSection((int)num_VhID.Value, txt_SegID.Text, txt_Adr.Text, SCAppConstants.PassEvent.Pass);
        }
        public void PrcSetVehicleToSection(int vh_num, string seg_num, string adr_id, SCAppConstants.PassEvent passType)
        {
            uctlVehicle Vhcl = m_objItemVhcl[vh_num];
            GroupRails groupRails = findMatchGroupRail(seg_num, adr_id);
            switch (passType)
            {
                case SCAppConstants.PassEvent.Pass:
                    if (Vhcl.p_CurrentSecID != groupRails.Section_ID)
                    {
                        if (!string.IsNullOrWhiteSpace(Vhcl.p_CurrentSecID))
                            m_DicSectionGroupRails[Vhcl.p_CurrentSecID].VehicleLeave(Vhcl);
                    }
                    groupRails.VehicleEnterSection(Vhcl, adr_id, 0);

                    break;
                case SCAppConstants.PassEvent.ArrivalFromAdr:
                    groupRails.VehicleArrivalsStartAdr(Vhcl);
                    break;
                case SCAppConstants.PassEvent.ArrivalToAdr:
                    m_DicSectionGroupRails[Vhcl.p_CurrentSecID].VehicleLeave(Vhcl);
                    groupRails.VehicleArrivalsEndAdr(Vhcl);
                    break;
            }
        }


        public GroupRails getGroupRailBySecID(string sec_id)
        {
            if (!m_DicSectionGroupRails.ContainsKey(sec_id.Trim()))
            {
                return null;
            }
            return m_DicSectionGroupRails[sec_id.Trim()];
        }
        public uctlAddress getuctAddressByAdrID(string adr_id)
        {
            uctlAddress uctl_adr = m_objItemAddr.Where(obj => obj.p_Address == adr_id).SingleOrDefault();
            return uctl_adr;
        }

        public GroupRails findMatchGroupRail(string seg_num, string adr_id)
        {
            GroupRails groupRails = null;
            foreach (KeyValuePair<string, GroupRails> keyValue in m_DicSectionGroupRails)
            {
                groupRails = keyValue.Value;
                if (groupRails.Segment_Num.Trim() == seg_num)
                {
                    if (groupRails.p_Points[0].BindAddressID.Trim() == adr_id.Trim())
                    {
                        return groupRails;
                    }
                }
            }
            return groupRails;
        }
        #endregion Vehicle Position Control


        #region Monitor Mode
        public void entryMonitorMode()
        {
            tlp_Through_Times.Visible = true;
            pnl_Status_desc.Visible = false;
        }
        public void LeaveMonitorMode()
        {
            tlp_Through_Times.Visible = false;
            pnl_Status_desc.Visible = true;
        }


        #endregion Monitor Mode

        #region Segment Disable Control
        public void RegistRailSelectedEvent(EventHandler eventHandler)
        {
            foreach (uctlRail rail in m_objItemRail)
            {
                rail.RailSelected += eventHandler;
            }
        }
        public void RemoveRailSelectedEvent(EventHandler eventHandler)
        {
            foreach (uctlRail rail in m_objItemRail)
            {
                rail.RailSelected -= eventHandler;
            }
        }

        #endregion Segment Disable Control

        private void cb_railDirection_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb == null)
                return;
            foreach (uctlRail rail in m_objItemRail)
            {
                if (rail.p_RailType == E_RAIL_TYPE.Arrow_Up ||
                    rail.p_RailType == E_RAIL_TYPE.Arrow_Down ||
                    rail.p_RailType == E_RAIL_TYPE.Arrow_Left ||
                    rail.p_RailType == E_RAIL_TYPE.Arrow_Right)
                {
                    rail.Visible = cb.Checked;
                }
            }
        }

        private void uctl_Map_Load(object sender, EventArgs e)
        {
            pnl_Map_Resize(null, null);
        }

        private void cb_SecAdrMark_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb == null)
                return;
            DisplaySectionLables(cb.Checked);
            DisplayAddressLable(cb.Checked);
        }


        private void pnl_Map_DoubleClick(object sender, EventArgs e)
        {
            ohtc_Form.setMonitorVehicle(string.Empty);
        }

        public void trunOnMonitorAllVhStatus()
        {
            foreach (uctlNewVehicle vhOjb in m_objItemNewVhcl)
            {
                vhOjb.turnOnMonitorVh();
            }
        }
        public void trunOffMonitorAllVhStatus()
        {
            foreach (uctlVehicle vhOjb in m_objItemVhcl)
            {
                vhOjb.turnOffMonitorVh();
            }
        }

        public void DisplayAddressLable(bool isDisplay)
        {
            foreach (Label lbl in addressLables)
            {
                lbl.Visible = isDisplay;
            }
        }

        public void DisplaySectionLables(bool isDisplay)
        {
            foreach (Label lbl in sectionLables)
            {
                lbl.Visible = isDisplay;
            }
        }

        private async void btn_EMOPause_Click(object sender, EventArgs e)
        {
            await Task.Run(() => mainForm.BCApp.SCApplication.VehicleService.PauseAllVehicleByOHxCPause());
        }

        private async void btn_EMORecover_Click(object sender, EventArgs e)
        {
            await Task.Run(() => mainForm.BCApp.SCApplication.VehicleService.ResumeAllVehicleByOhxCPause());
        }
    }
}
