
#define	USED_EDITOR_MODE		// CAUTION!!! : if not used [Editor Mode], commennt out.
// Editor Mode = Detail Settings to ToolTip

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using com.mirle.ibg3k0.sc;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using NLog;

namespace com.mirle.ibg3k0.bc.winform.UI.Components
{
    public partial class uctlAddress : UserControl
    {

        #region "Constant"

        #endregion	/* Constant */

        #region "Internal Variable"

        private string m_sAddress;
        private int m_iAddrPt;
        private string m_sSectionID;
        private int m_iLocX;
        private int m_iLocY;
        private Color m_clrColor;
        private string m_sText;
        private int m_iSizeW;
        private int m_iSizeH;
        private int m_iZoom_LV;
        private E_POINT_TYPE m_iPointType;
        Label lbl_AdrID = null;
        private uctl_Map Uctl_Map;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion	/* Internal Variable */

        #region "Event"

        /// <summary>
        /// Port Select Event Handler
        /// </summary>
        /// <param name="iSelectType">0:From, 1:To</param>
        /// <param name="iAddrPt"></param>
        public delegate void PortSelectedEventHandler(int iSelectType, int iAddrPt);
        public event PortSelectedEventHandler evtPortSelected;

        #endregion	/* Event */

        #region "Property"

        /// <summary>
        /// Object Name
        /// </summary>
        public string p_Address
        {
            get { return (m_sAddress); }
            set
            {
                m_sAddress = value;
                _SetAddrToolTip();
            }
        }

        public int p_AddrPt
        {
            get { return (m_iAddrPt); }
            set
            {
                m_iAddrPt = value;
            }
        }

        public string p_sectionID
        {
            get { return m_sSectionID; }
            set
            {
                m_sSectionID = value;
            }
        }

        public int p_LocX
        {
            get { return (m_iLocX); }
            set
            {
                m_iLocX = value;
                str_Map_Add_Info.LOCATIONX = value;
                _ChangeAddrImage();
            }
        }

        public int p_LocY
        {
            get { return (m_iLocY); }
            set
            {
                m_iLocY = value;
                str_Map_Add_Info.LOCATIONY = value;
                _ChangeAddrImage();
            }
        }

        public Color p_Color
        {
            get { return (m_clrColor); }
            set
            {
                m_clrColor = value;
                _ChangeAddrImage();
            }
        }

        public int p_LayoutPoint;
        public string p_Text
        {
            get { return (m_sText); }
            set
            {
                m_sText = value;
                _SetAddrToolTip();
            }
        }
        public E_POINT_TYPE p_PointType
        {
            get { return (m_iPointType); }
            set
            {
                m_iPointType = value;
            }
        }
        public int p_SizeW
        {
            get { return (m_iSizeW); }
            set
            {
                m_iSizeW = value;
                this.Width = value;
#if USED_EDITOR_MODE
                _SetAddrToolTip();
#endif
            }
        }

        public int p_SizeH
        {
            get { return (m_iSizeH); }
            set
            {
                m_iSizeH = value;
                this.Height = value;
#if USED_EDITOR_MODE
                _SetAddrToolTip();
#endif
            }
        }
        public APOINT str_Map_Add_Info;

        public int p_ZoomLV
        {
            get { return (m_iZoom_LV); }
            set
            {
                m_iZoom_LV = value;
            }
        }

        #endregion	/* Property */

        #region "Constructor／Destructor"

        public uctlAddress(uctl_Map _uctl_Map)
        {
            InitializeComponent();
            Uctl_Map = _uctl_Map;
            m_sAddress = "0000";
            m_iLocX = 0;
            m_iLocY = 0;
            m_clrColor = Color.WhiteSmoke;

            _SetInitialAddrToolTip();
            _SetAddrToolTip();

            this.Width = 8;
            this.Height = 8;
        }

        //// 重写OnPaint，每当绘画时发生
        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    base.OnPaint(e);
        //    //try
        //    //{
        //    //    //做一张背景透明的位图当做绘制内容
        //    //    //若已有Image属性被指定，则画该图
        //    //    //由于e的绘制区域总是在变，这里不需要考虑绘制区域，系统会自己算，超出区域的不予以绘制
        //    //    //简单刷原理就是，每当系统要绘制时，先画背景，我们自行处理就需要计算应该画哪个区域（多数都是不规则的区域并集）
        //    //    //每当OnPaint时，绘制自己的Image图（若属性存在），或者是纯透明的位图以提供完全透明效果
        //    //    if (objImage == null)
        //    //        return;
        //    //    switch (m_iPointType)
        //    //    {
        //    //        case E_POINT_TYPE.NoPort:
        //    //            base.OnPaint(e);
        //    //            break;
        //    //        default:
        //    //            var bitMap = new Bitmap(objImage);
        //    //            bitMap.MakeTransparent(Color.White);
        //    //            objImage = bitMap;
        //    //            //绘制模式指定
        //    //            Graphics g = e.Graphics;
        //    //            g.SmoothingMode = SmoothingMode.AntiAlias;
        //    //            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        //    //            g.CompositingQuality = CompositingQuality.GammaCorrected;
        //    //            //透明色变换
        //    //            float[][] mtxItens = {
        //    //                    new float[] {1,0,0,0,0},
        //    //                    new float[] {0,1,0,0,0},
        //    //                    new float[] {0,0,1,0,0},
        //    //                    new float[] {0,0,0,1,0},
        //    //                    new float[] {0,0,0,0,1}};
        //    //            ColorMatrix colorMatrix = new ColorMatrix(mtxItens);
        //    //            ImageAttributes imgAtb = new ImageAttributes();
        //    //            imgAtb.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        //    //            //画
        //    //            g.DrawImage(objImage, ClientRectangle, 0.0f, 0.0f, objImage.Width, objImage.Height, GraphicsUnit.Pixel, imgAtb);

        //    //            break;
        //    //    }
        //    //}
        //    //catch { }
        //}
        ////控件背景绘制
        //protected override void OnPaintBackground(PaintEventArgs e)
        //{
        //    base.OnPaint(e);
        //    //try
        //    //{
        //    //    base.OnPaintBackground(e);
        //    //    if (m_iPointType == E_POINT_TYPE.NoPort)
        //    //        return;
        //    //    Graphics g = e.Graphics;
        //    //    //以下代码提取控件原来应有的背景（视觉上的位图，可能会包括其他空间的部分图形）到位图，进行变换后绘制
        //    //    if (Parent != null)
        //    //    {
        //    //        BackColor = Color.Transparent;
        //    //        //本控件在父控件的子集中的index
        //    //        int index = Parent.Controls.GetChildIndex(this);
        //    //        for (int i = Parent.Controls.Count - 1; i > index; i--)
        //    //        {
        //    //            //对每一个父控件的可视子控件，进行绘制区域交集运算，得到应该绘制的区域
        //    //            Control c = Parent.Controls[i];
        //    //            //如果有交集且可见
        //    //            if (c.Bounds.IntersectsWith(Bounds) && c.Visible)
        //    //            {
        //    //                //矩阵变换
        //    //                Bitmap bmp = new Bitmap(c.Width, c.Height, g);
        //    //                c.DrawToBitmap(bmp, c.ClientRectangle);
        //    //                g.TranslateTransform(c.Left - Left, c.Top - Top);
        //    //                //画图
        //    //                g.DrawImageUnscaled(bmp, Point.Empty);
        //    //                g.TranslateTransform(Left - c.Left, Top - c.Top);
        //    //                bmp.Dispose();
        //    //            }
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        g.Clear(Parent.BackColor);
        //    //        g.FillRectangle(new SolidBrush(Color.FromArgb(255, Color.Transparent)), this.ClientRectangle);
        //    //    }
        //    //}
        //    //catch { }
        //}


        #endregion	/* Constructor／Destructor */

        #region "Publish Process"

        #endregion	/* Publish Process */

        #region "Internal Process"

        private void uctlAddress_Resize(object sender, EventArgs e)
        {
            _ChangeAddrImage();
        }

        private void uctlAddress_LocationChanged(object sender, EventArgs e)
        {
            this.m_iLocX = this.Left + (this.Width / 2);
            this.m_iLocY = this.Top + (this.Height / 2);
        }

        private void _ChangeAddrImage()
        {
            this.Left = this.m_iLocX - (this.Width / 2);
            this.Top = this.m_iLocY - (this.Height / 2);
            switch (p_PointType)
            {
                case E_POINT_TYPE.NoPort:
                    this.BackColor = m_clrColor;
                    break;
                case E_POINT_TYPE.Load:
                case E_POINT_TYPE.Unload:
                case E_POINT_TYPE.LoadUnload:
                    this.BackColor = m_clrColor;
                    //_CreateArc();
                    break;
            }
        }

        Image objImage;
        Graphics objRail;

        private void _CreateArc()
        {
            if (p_SizeW == 0 || p_SizeH == 0)
                return;
            GraphicsPath graphicsPath = null;
            Pen objPen;
            int iX = 0, iY = 0;
            int iBoxWidth, iBoxHeight;
            int iArcWidth = 0, iArcHeight = 0;
            int startAngle = 0;
            try
            {
                this.Height = this.Width;

                iBoxWidth = this.Width;
                iBoxHeight = this.Height;

                objImage = new Bitmap(iBoxWidth, iBoxHeight);
                objRail = Graphics.FromImage(objImage);
                objPen = new Pen(m_clrColor, 5);
                objPen.Alignment = PenAlignment.Inset;

                graphicsPath = new GraphicsPath();
                iX = 0;
                iY = 0;
                iArcWidth = p_SizeW;
                iArcHeight = p_SizeH;
                startAngle = 0;
                graphicsPath.AddArc(iX, iY, iArcWidth, iArcHeight, startAngle, 360);

                objRail.DrawPath(objPen, graphicsPath);

                objRail.Dispose();
                objPen.Dispose();
                this.BackgroundImage = objImage;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                string sEx = ex.Message + "\r\n" + ex.StackTrace;

            }
        }

        private void _SetInitialAddrToolTip()
        {
            this.ToolTip.AutoPopDelay = 30000;			// 30sec
            this.ToolTip.ForeColor = Color.Black;
            this.ToolTip.BackColor = Color.White;
            this.ToolTip.ShowAlways = true;
            this.ToolTip.UseAnimation = false;
            this.ToolTip.UseFading = false;

#if USED_EDITOR_MODE
            this.ToolTip.InitialDelay = 100;
            this.ToolTip.ReshowDelay = 100;
#endif
        }

        private void _SetAddrToolTip()
        {
#if USED_EDITOR_MODE
            this.ToolTip.SetToolTip(this,
                        "Address : " + this.m_sAddress + "\r\n" +
                        "Location : " + this.m_iLocX.ToString() + ", " + this.m_iLocY.ToString() + "\r\n" +
                        "Size : " + this.m_iSizeW.ToString() + ", " + this.m_iSizeH.ToString() + "\r\n" +
                        "Color(RGB) : " + this.m_clrColor.R.ToString().PadLeft(3)
                                    + "," + this.m_clrColor.G.ToString().PadLeft(3)
                                    + "," + this.m_clrColor.B.ToString().PadLeft(3));
#else
			this.ToolTip.SetToolTip( this, 
						"Address : " + this.m_sAddress + "\r\n" +
						"Comment : " + this.m_sText ) ;
#endif
        }

        private void tsmiSelectFrom_Click(object sender, EventArgs e)
        {
            if (evtPortSelected != null)
            {
                evtPortSelected(0, this.m_iAddrPt);
            }
        }

        private void tsmiSelectTo_Click(object sender, EventArgs e)
        {
            if (evtPortSelected != null)
            {
                evtPortSelected(1, this.m_iAddrPt);
            }
        }

        #endregion	/* Internal Process */

        private void uctlAddress_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ctxSubMenu.Show(Cursor.Position);
            }
        }



        public Label getLable()
        {
            if (lbl_AdrID == null)
                buildLable();
            return lbl_AdrID;
        }
        private void buildLable()
        {
            try
            {
                int lbl_width = 60;
                int lbl_height = 20;

                int loacation_x = this.m_iLocX - (lbl_width / 2);
                int loaction_y = this.m_iLocY + (this.Height);

                lbl_AdrID = new Label();
                lbl_AdrID.Text = m_sAddress;
                lbl_AdrID.Font = new System.Drawing.Font("Arial", 7F);
                lbl_AdrID.ForeColor = Color.YellowGreen;
                lbl_AdrID.BackColor = Color.Transparent;
                lbl_AdrID.Margin = new Padding(1);
                lbl_AdrID.Size = new System.Drawing.Size(lbl_width, lbl_height);
                lbl_AdrID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                lbl_AdrID.Location = new System.Drawing.Point(loacation_x, loaction_y);
                lbl_AdrID.Visible = false;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }

        private void uctlAddress_Click(object sender, EventArgs e)
        {
            Uctl_Map.ohtc_Form.setAdrCombobox(p_Address);
        }
    }
}
