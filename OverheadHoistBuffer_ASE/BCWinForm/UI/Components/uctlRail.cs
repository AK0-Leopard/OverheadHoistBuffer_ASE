
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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using com.mirle.ibg3k0.bc.winform.Common;
using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.bcf.Common;
using NLog;

namespace com.mirle.ibg3k0.bc.winform.UI.Components
{
    public partial class uctlRail : UserControl
    {

        #region "Constant"

        #endregion	/* Constant */

        #region "Internal Variable"

        private string m_sID;
        private int m_iNum;
        private int m_iGourpNum;
        private string m_sSectionID;
        private string m_SegNum;
        private string m_SegNumDisplay;
        private int m_iLocX;
        private int m_iLocY;
        private int m_iRailWidth;
        private int m_iRailLength;
        private Color m_clrRailColor;
        private E_RAIL_TYPE m_eRailType;


        private PointObject[] m_pPoints;
        private PointF m_pCenterPoint;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion	/* Internal Variable */

        #region "Event"
        //public new event MouseEventHandler MouseMove
        //{
        //    add
        //    {
        //        pctRail.MouseMove += value;
        //        base.MouseMove += value;
        //    }
        //    remove
        //    {
        //        pctRail.MouseMove -= value;
        //        base.MouseMove -= value;
        //    }
        //}
        //public new event MouseEventHandler MouseDown
        //{
        //    add
        //    {
        //        pctRail.MouseMove += value;
        //        base.MouseMove += value;
        //    }
        //    remove
        //    {
        //        pctRail.MouseMove -= value;
        //        base.MouseMove -= value;
        //    }
        //}

        public event EventHandler RailSelected;
        #endregion	/* Event */

        #region "Property"

        /// <summary>
        /// Object Name
        /// </summary>
        public string p_ID
        {
            get { return (m_sID); }
            set
            {
                m_sID = value;
            }
        }
        public int p_Num
        {
            get { return (m_iNum); }
            set
            {
                m_iNum = value;
                _SetRailToolTip();
            }
        }

        public int p_GourpNum
        {
            get { return (m_iGourpNum); }
            set
            {
                m_iGourpNum = value;
                _SetRailToolTip();
            }
        }

        public string p_SectionID
        {
            get { return (m_sSectionID); }
            set
            {
                m_sSectionID = value;
                _SetRailToolTip();
            }
        }


        public string p_SegNum
        {
            get { return (m_SegNum); }
            set
            {
                m_SegNum = value;
                _SetRailToolTip();
            }
        }

        public string p_SegNumDisplay
        {
            get { return (m_SegNumDisplay); }
            set
            {
                m_SegNumDisplay = value;
                _SetRailToolTip();
            }
        }

        public int p_LocX
        {
            get { return (m_iLocX); }
            set
            {
                m_iLocX = value;
                this.Left = value;
                p_StrMapRailInfo.LOCATIONX = value;
            }
        }

        public int p_LocY
        {
            get { return (m_iLocY); }
            set
            {
                m_iLocY = value;
                this.Top = value;
                p_StrMapRailInfo.LOCATIONY = value;
            }
        }

        private int m_semiRailWidth = 0;
        public int p_RailWidth
        {
            get { return (m_iRailWidth); }
            set
            {
                m_iRailWidth = value;
                m_semiRailWidth = m_iRailWidth / 2;
                _SetRailImage();
            }
        }

        private double m_RailLength_Pixel = 0;
        public int p_RailLength
        {
            get { return (m_iRailLength); }
            set
            {
                m_iRailLength = value;

                switch (p_RailType)
                {
                    case E_RAIL_TYPE.Straight_Vertical:
                    case E_RAIL_TYPE.Arrow_Up:
                    case E_RAIL_TYPE.Arrow_Down:
                        m_RailLength_Pixel = BCUtility.RealLengthToPixelsWidthByScale(m_iRailLength);
                        this.Height = (int)m_RailLength_Pixel;
                        break;
                    case E_RAIL_TYPE.Straight_Horizontal:
                    case E_RAIL_TYPE.Arrow_Left:
                    case E_RAIL_TYPE.Arrow_Right:
                        m_RailLength_Pixel = BCUtility.RealLengthToPixelsWidthByScale(m_iRailLength);
                        this.Width = (int)m_RailLength_Pixel;
                        break;
                    case E_RAIL_TYPE.Curve_0to90:
                    case E_RAIL_TYPE.Curve_90to180:
                    case E_RAIL_TYPE.Curve_180to270:
                    case E_RAIL_TYPE.Curve_270to360:
                        m_RailLength_Pixel = BCUtility.RealLengthToPixelsWidthByScale(m_iRailLength);
                        int S = (int)m_RailLength_Pixel;
                        double radius = S * (2 / Math.PI);
                        this.Width = (int)radius;
                        break;
                    default:

                        break;
                }

#if USED_EDITOR_MODE
                _SetRailToolTip();
#endif
            }
        }

        public ARAIL p_StrMapRailInfo;

        public Color p_RailColor
        {
            get { return (m_clrRailColor); }
            set
            {
                if (m_clrRailColor != value)
                {
                    m_clrRailColor = value;
                    _SetRailImage();
                }
            }
        }

        public E_RAIL_TYPE p_RailType
        {
            get { return (m_eRailType); }
            set
            {
                m_eRailType = value;
                _SetRailImage();
            }
        }


        public PointObject[] p_Points
        {
            get { return (m_pPoints); }
        }
        //public PointF p_CenterPoint
        //{
        //    get { return (m_pCenterPoint); }
        //}
        public PointF p_RealCenterPoint
        {
            get
            {
                return new PointF(m_pCenterPoint.X + this.p_LocX, m_pCenterPoint.Y + this.p_LocY);
            }
        }

        #endregion	/* Property */

        #region "Constructor／Destructor"

        public uctlRail()
        {
            InitializeComponent();
            m_pPoints = new PointObject[2];

            m_iNum = -1;
            m_iRailWidth = 6;
            m_eRailType = E_RAIL_TYPE.Straight_Horizontal;
            m_clrRailColor = Color.RoyalBlue;

            _SetInitialRailToolTip();
            _SetRailToolTip();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            //_SetRailImage();
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
        //    //    if (objImage == null) return;
        //    //    var bitMap = new Bitmap(objImage);
        //    //    bitMap.MakeTransparent(Color.White);
        //    //    objImage = bitMap;
        //    //    //绘制模式指定
        //    //    Graphics g = e.Graphics;
        //    //    g.SmoothingMode = SmoothingMode.AntiAlias;
        //    //    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        //    //    g.CompositingQuality = CompositingQuality.GammaCorrected;
        //    //    //透明色变换
        //    //    float[][] mtxItens = {
        //    //                new float[] {1,0,0,0,0},
        //    //                new float[] {0,1,0,0,0},
        //    //                new float[] {0,0,1,0,0},
        //    //                new float[] {0,0,0,1,0},
        //    //                new float[] {0,0,0,0,1}};
        //    //    ColorMatrix colorMatrix = new ColorMatrix(mtxItens);
        //    //    ImageAttributes imgAtb = new ImageAttributes();
        //    //    imgAtb.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        //    //    //画
        //    //    g.DrawImage(objImage, ClientRectangle, 0.0f, 0.0f, objImage.Width, objImage.Height, GraphicsUnit.Pixel, imgAtb);
        //    //}
        //    //catch { }
        //}
        ////控件背景绘制
        //protected override void OnPaintBackground(PaintEventArgs e)
        //{
        //    base.OnPaintBackground(e);
        //    //try
        //    //{
        //    //    base.OnPaintBackground(e);
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

        public void UnbindingPointObj()
        {
            foreach (PointObject obj in m_pPoints)
            {

                obj.UnBindPoint();
            }
        }


        public void setWidthByZoonInZoonOut(int Width)
        {
            m_iRailWidth = Width;
            m_semiRailWidth = m_iRailWidth / 2;
            _SetRailImage();
        }
        public void setLengthByZoonInZoonOut(int Length)
        {
            switch (p_RailType)
            {
                case E_RAIL_TYPE.Straight_Vertical:
                    m_RailLength_Pixel = Length;
                    this.Height = Length;
                    break;
                case E_RAIL_TYPE.Straight_Horizontal:
                    m_RailLength_Pixel = Length;
                    this.Width = Length;
                    break;
                case E_RAIL_TYPE.Curve_0to90:
                case E_RAIL_TYPE.Curve_90to180:
                case E_RAIL_TYPE.Curve_180to270:
                case E_RAIL_TYPE.Curve_270to360:
                    this.Width = Length;
                    break;
                default:

                    break;
            }
        }
        #endregion	/* Publish Process */

        #region "Internal Process"

        private void uctlRail_Resize(object sender, EventArgs e)
        {
            _SetRailImage();
        }

        private void _SetRailImage()
        {
            switch (m_eRailType)
            {
                case E_RAIL_TYPE.Straight_Horizontal:
                case E_RAIL_TYPE.Straight_Vertical:
                    _CreateLine();
                    break;
                case E_RAIL_TYPE.Curve_0to90:
                case E_RAIL_TYPE.Curve_90to180:
                case E_RAIL_TYPE.Curve_180to270:
                case E_RAIL_TYPE.Curve_270to360:
                    _CreateCurve();
                    break;
                case E_RAIL_TYPE.Arrow_Up:
                case E_RAIL_TYPE.Arrow_Down:
                case E_RAIL_TYPE.Arrow_Left:
                case E_RAIL_TYPE.Arrow_Right:
                    _CreateArrow();
                    break;

            }
        }
        Image objImage;
        Graphics objRail;

        private void _CreateLine()
        {
            SolidBrush objBrush;
            int iX, iY;
            int iWidth, iHeight;

            try
            {
                iX = 0;
                iY = 0;

                if (m_eRailType == E_RAIL_TYPE.Straight_Horizontal)
                {
                    this.Height = m_iRailWidth;
                    if (m_RailLength_Pixel != 0)
                        this.Width = (int)m_RailLength_Pixel;
                }
                else
                {
                    this.Width = m_iRailWidth;
                    if (m_RailLength_Pixel != 0)
                        this.Height = (int)m_RailLength_Pixel;
                }
                iWidth = this.Width;
                iHeight = this.Height;

                objImage = new Bitmap(iWidth, iHeight);
                objRail = Graphics.FromImage(objImage);
                objBrush = new SolidBrush(m_clrRailColor);
                if (m_eRailType == E_RAIL_TYPE.Straight_Horizontal)
                {
                    objRail.FillRectangle(objBrush, iX, iY, iWidth, m_iRailWidth);

                    setLineEndPoint(ref m_pPoints[0], new PointF(iX, iY + m_semiRailWidth), PointType.Left);
                    setLineEndPoint(ref m_pPoints[1], new PointF(iWidth, iY + m_semiRailWidth), PointType.Right);

                    //m_pPoints[0] = new PointObject(this, new PointF(iX, iY + m_semiRailWidth), PointType.Left);
                    //m_pPoints[1] = new PointObject(this, new PointF(iWidth, iY + m_semiRailWidth), PointType.Right);
                }
                else
                {
                    objRail.FillRectangle(objBrush, iX, iY, m_iRailWidth, iHeight);

                    setLineEndPoint(ref m_pPoints[0], new PointF(iX + m_semiRailWidth, iY), PointType.Up);
                    setLineEndPoint(ref m_pPoints[1], new PointF(iX + m_semiRailWidth, iHeight), PointType.Down);

                    //m_pPoints[0] = new PointObject(this, new PointF(iX + m_semiRailWidth, iY), PointType.Up);
                    //m_pPoints[1] = new PointObject(this, new PointF(iX + m_semiRailWidth, iHeight), PointType.Down);
                }

                objRail.Dispose();
                objBrush.Dispose();

                this.BackgroundImage = objImage;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                string sEx = ex.Message + "\r\n" + ex.StackTrace;
            }
        }

        private void setLineEndPoint(ref PointObject pointObj, PointF point, PointType type)
        {
            if (pointObj == null)
                pointObj = new PointObject(this, point, type);
            else
                pointObj.updatePoint(point);
        }


        private void _CreateCurve()
        {
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
                objPen = new Pen(m_clrRailColor, m_iRailWidth);
                objPen.Alignment = PenAlignment.Inset;
                graphicsPath = new GraphicsPath();
                switch (m_eRailType)
                {
                    case E_RAIL_TYPE.Curve_0to90:
                        iX = -iBoxWidth;
                        iY = (m_iRailWidth / 2);
                        iArcWidth = iBoxWidth * 2 - (m_iRailWidth / 2);
                        iArcHeight = iBoxHeight * 2 - (m_iRailWidth / 2);
                        startAngle = 270;
                        drawGraphicsPathAndSetEndPoint(ref graphicsPath, iX, iY, iArcWidth, iArcHeight, startAngle
                          , PointType.Left, PointType.Down);
                        break;
                    case E_RAIL_TYPE.Curve_90to180:
                        iX = (m_iRailWidth / 2);
                        iY = (m_iRailWidth / 2);
                        iArcWidth = iBoxWidth * 2 - (m_iRailWidth / 2);
                        iArcHeight = iBoxHeight * 2 - (m_iRailWidth / 2);
                        startAngle = 180;
                        drawGraphicsPathAndSetEndPoint(ref graphicsPath, iX, iY, iArcWidth, iArcHeight, startAngle
                          , PointType.Down, PointType.Right);
                        break;
                    case E_RAIL_TYPE.Curve_180to270:
                        iX = (m_iRailWidth / 2);
                        iY = -iBoxHeight;
                        iArcWidth = iBoxWidth * 2 - (m_iRailWidth / 2);
                        iArcHeight = iBoxHeight * 2 - (m_iRailWidth / 2);
                        startAngle = 90;
                        drawGraphicsPathAndSetEndPoint(ref graphicsPath, iX, iY, iArcWidth, iArcHeight, startAngle
                          , PointType.Up, PointType.Right);
                        break;
                    case E_RAIL_TYPE.Curve_270to360:
                        iX = -iBoxWidth;
                        iY = -iBoxHeight;
                        iArcWidth = iBoxWidth * 2 - (m_iRailWidth / 2);
                        iArcHeight = iBoxHeight * 2 - (m_iRailWidth / 2);
                        startAngle = 0;
                        drawGraphicsPathAndSetEndPoint(ref graphicsPath, iX, iY, iArcWidth, iArcHeight, startAngle
                          , PointType.Left, PointType.Up);
                        break;
                }

                if (graphicsPath != null)
                {
                    objRail.DrawPath(objPen, graphicsPath);

                    graphicsPath.Reset();
                    graphicsPath.AddArc(iX, iY, iArcWidth, iArcHeight, startAngle, 45);
                    m_pCenterPoint = graphicsPath.GetLastPoint();
                }
                objRail.Dispose();
                objPen.Dispose();
                this.BackgroundImage = objImage;
            }
            catch (Exception ex)
            {
                string sEx = ex.Message + "\r\n" + ex.StackTrace;

            }
        }

        private void _CreateArrow()
        {
            int iX, iY;
            int iWidth, iHeight;
            Pen objPen;

            try
            {
                iX = 0;
                iY = 0;

                if (m_eRailType == E_RAIL_TYPE.Arrow_Left
                    || m_eRailType == E_RAIL_TYPE.Arrow_Right)
                {
                    this.Height = m_iRailWidth;
                    if (m_RailLength_Pixel != 0)
                        this.Width = (int)m_RailLength_Pixel;
                }
                else
                {
                    this.Width = m_iRailWidth;
                    if (m_RailLength_Pixel != 0)
                        this.Height = (int)m_RailLength_Pixel;
                }
                iWidth = this.Width;
                iHeight = this.Height;

                objImage = new Bitmap(iWidth, iHeight);
                objRail = Graphics.FromImage(objImage);
                objPen = new Pen(m_clrRailColor, m_iRailWidth / 2);
                objPen.Alignment = PenAlignment.Inset;
                objPen.StartCap = LineCap.ArrowAnchor;
                if (m_eRailType == E_RAIL_TYPE.Arrow_Up)
                {
                    objRail.DrawLine(objPen, m_iRailWidth / 2, iY, m_iRailWidth / 2, iHeight);
                }
                else if (m_eRailType == E_RAIL_TYPE.Arrow_Down)
                {
                    objRail.DrawLine(objPen, m_iRailWidth / 2, iHeight, m_iRailWidth / 2, iY);
                }
                else if (m_eRailType == E_RAIL_TYPE.Arrow_Left)
                {
                    objRail.DrawLine(objPen, iX, m_iRailWidth / 2, iWidth, m_iRailWidth / 2);
                }
                else if (m_eRailType == E_RAIL_TYPE.Arrow_Right)
                {
                    objRail.DrawLine(objPen, iWidth, m_iRailWidth / 2, iX, m_iRailWidth / 2);
                }
                objRail.Dispose();
                objPen.Dispose();

                this.BackgroundImage = objImage;
            }
            catch (Exception ex)
            {
                string sEx = ex.Message + "\r\n" + ex.StackTrace;
            }
        }


        private void drawGraphicsPathAndSetEndPoint(ref GraphicsPath graphicsPath, int iX, int iY, int iArcWidth, int iArcHeight
            , int startAngle, PointType startPointType, PointType endPointType)
        {
            graphicsPath.AddArc(iX, iY, iArcWidth, iArcHeight, startAngle, 90);

            creatOrUpdateEndPoint(ref m_pPoints[0], graphicsPath.PathPoints[0], startPointType);
            creatOrUpdateEndPoint(ref m_pPoints[1], graphicsPath.GetLastPoint(), endPointType);
        }

        private void creatOrUpdateEndPoint(ref PointObject point, PointF graphicsPath, PointType _type)
        {
            if (point == null)
            {
                point = new PointObject(this, graphicsPath, _type);
            }
            else
            {
                point.updatePoint(graphicsPath);
            }
        }



        private void _SetInitialRailToolTip()
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

        private void _SetRailToolTip()
        {
#if USED_EDITOR_MODE
            string center_point1 = this.m_pPoints[0] != null ? this.m_pPoints[0].RealPointf.ToString() : "";
            string center_point2 = this.m_pPoints[1] != null ? this.m_pPoints[1].RealPointf.ToString() : "";
            //this.ToolTip.SetToolTip(this.pctRail,
            this.ToolTip.SetToolTip(this,
                        "Rail No. : " + this.m_iNum.ToString() + "\r\n" +
                                                "Segment Num: " + this.p_SegNumDisplay + "\r\n" +
                        "Section ID : " + this.m_sSectionID + "\r\n" +
                        //"Location : " + this.m_iLocX.ToString() + ", " + this.m_iLocY.ToString() + "\r\n" +
                        "Size : " + /*this.m_iRailWidth.ToString() + ", " + */this.m_iRailLength.ToString() + " mm" + "\r\n"
                        //"Center Point (1): " + center_point1 + "\r\n" +
                        //"Center Point (2): " + center_point2 + "\r\n" +
                        //"Color(RGB) : " + this.m_clrRailColor.R.ToString().PadLeft(3)
                        //            + "," + this.m_clrRailColor.G.ToString().PadLeft(3)
                        //            + "," + this.m_clrRailColor.B.ToString().PadLeft(3)
                        );
#else
			this.ToolTip.SetToolTip( this.pctRail, 
						"Rail No. : " + this.m_iNum.ToString() + "\r\n" +
						"Group : " + this.m_iGourpNum.ToString() + "\r\n" +
						"Segment : " + this.m_iSegNum.ToString() ) ;
#endif
        }

        #endregion	/* Internal Process */

        private void uctlRail_DoubleClick(object sender, EventArgs e)
        {
            if (RailSelected != null)
            {
                SelectedRailEventArgs arge = new SelectedRailEventArgs()
                { Segment_Num = this.m_SegNum.ToString() };
                RailSelected(this, arge);
            }
        }
    }

    public class PointObject : ICloneable
    {
        public uctlRail rail { get; private set; }
        public PointObject BindPointObj { get; private set; }
        public string BindAddressID = "";
        uctlAddress BindAddress = null;

        public PointObject()
        {
        }

        public PointObject(uctlRail _rail, PointF _point, PointType _type)
        {
            rail = _rail;
            //pointf = _point;
            pointf = reCheckPoint(rail, _point);
            type = _type;
        }
        public PointF pointf { get; private set; }
        public PointF RealPointf
        {
            get
            {
                return new PointF(pointf.X + rail.p_LocX, pointf.Y + rail.p_LocY);
            }
        }
        public PointType type { get; private set; }

        public void BindingPoint(PointObject _pointObj)
        {
            BindPointObj = _pointObj;
        }


        public void BindingAddress(uctlAddress _addressObj, bool isLast)
        {

            BindAddressID = _addressObj.p_Address;


            if (isLast)
            {
                BindAddress = _addressObj;
                BindAddress.p_LocX = (int)RealPointf.X;
                BindAddress.p_LocY = (int)RealPointf.Y;
                //ToDo CTLIF.g_objMapIf.p_AddressLayout[BindAddress.p_LayoutPoint] = BindAddress.str_Map_Add_Info;
            }
            else
            {
                _addressObj.p_sectionID = rail.p_SectionID;
            }
        }

        public void UnBindPoint()
        {
            if (BindPointObj != null)
            {
                if (BindPointObj.BindPointObj == this)
                    BindPointObj.BindPointObj = null;
                BindPointObj = null;
            }
        }

        public void UnBindAddress()
        {
            if (BindAddress != null)
            {
                BindAddress = null;
            }
        }

        public void updatePoint(PointF _point)
        {
            pointf = reCheckPoint(rail, _point);
        }

        public void refreshBindPoint()
        {
            if (BindPointObj != null)
            {
                BindPointObj.rail.p_LocY = (int)(rail.p_LocY + pointf.Y - BindPointObj.pointf.Y);
                BindPointObj.rail.p_LocX = (int)(rail.p_LocX + pointf.X - BindPointObj.pointf.X);
            }
        }

        private PointF reCheckPoint(uctlRail ucrlRail, PointF sorcePoint)
        {
            if (sorcePoint.X < 0)
                sorcePoint.X = 0;
            else if (sorcePoint.X > ucrlRail.Width)
                sorcePoint.X = ucrlRail.Width;

            if (sorcePoint.Y < 0)
                sorcePoint.Y = 0;
            else if (sorcePoint.Y > ucrlRail.Height)
                sorcePoint.Y = ucrlRail.Height;
            return sorcePoint;
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }

    }

    public class PointObjectNull : PointObject
    {
        public PointObjectNull()
        {

        }

    }
    public enum PointType
    {
        Up,
        Down,
        Left,
        Right
    }


    public class GroupRails
    {
        Label lbl_SectionID = null;
        public string Section_ID;

        public double Distance { get; private set; }
        public double RealDistance { get; private set; }


        public string Segment_Num;
        public E_RAIL_DIR SegDircetion;
        public E_RAIL_DIR RailDircetion;

        //public List<uctlRail> uctlRails;
        public List<KeyValuePair<uctlRail, E_RAIL_DIR>> uctlRails;
        private PointObject[] m_pPoints;
        private List<uctlVehicle> vehicles;
        private Dictionary<string, List<uctlVehicle>> dicVehicles;
        uctlRail firstRail = null;
        uctlRail lastRail = null;
        private int vehicleInterval = 0;//mm

        public PointObject[] p_Points
        {
            get { return (m_pPoints); }
        }

        public List<uctlVehicle> p_Vehicles
        {
            get { return (vehicles); }
        }



        public void GroupMove(int offset_x, int offset_Y)
        {
            foreach (var ucrlRail in uctlRails)
            {
                ucrlRail.Key.p_LocY = ucrlRail.Key.p_LocY + offset_Y;
                ucrlRail.Key.p_LocX = ucrlRail.Key.p_LocX + offset_x;
            }
        }
        public void GroupColorChange(Color change_color)
        {
            foreach (var ucrlRail in uctlRails)
            {
                ucrlRail.Key.p_RailColor = change_color;
            }
        }

        public GroupRails(string section_id, double real_sec_distance, E_RAIL_DIR rail_dir, string seg_num, E_RAIL_DIR seg_dir)
        {
            Section_ID = section_id;
            RealDistance = real_sec_distance;

            RailDircetion = rail_dir;

            Segment_Num = seg_num;
            SegDircetion = seg_dir;

            //uctlRails = new List<uctlRail>();
            uctlRails = new List<KeyValuePair<uctlRail, E_RAIL_DIR>>();
            m_pPoints = new PointObject[2];
            vehicles = new List<uctlVehicle>();
            dicVehicles = new Dictionary<string, List<uctlVehicle>>();
        }

        public void RefreshDistance()
        {
            Distance = uctlRails.Select(kayValue => kayValue.Key.p_RailLength).Sum();
        }

        public Label getSectionLable()
        {
            if (lbl_SectionID == null)
                buildSectionLable();
            return lbl_SectionID;
        }
        private void buildSectionLable()
        {
            try
            {
                int lbl_width = 30;
                int lbl_height = 20;
                int loacation_x = 0;
                int loaction_y = 0;
                uctlRail curveRail = null;
                if (isCurve(out curveRail))
                {
                    switch (curveRail.p_RailType)
                    {
                        case E_RAIL_TYPE.Curve_0to90:
                        case E_RAIL_TYPE.Curve_270to360:
                            loacation_x = (int)curveRail.p_RealCenterPoint.X -
                                (curveRail.p_RailWidth) - lbl_width;

                            loaction_y = (int)curveRail.p_RealCenterPoint.Y;
                            break;
                        case E_RAIL_TYPE.Curve_90to180:
                        case E_RAIL_TYPE.Curve_180to270:
                            loacation_x = (int)curveRail.p_RealCenterPoint.X + lbl_width;

                            loaction_y = (int)curveRail.p_RealCenterPoint.Y;
                            break;
                    }
                }
                else
                {
                    if (RailDircetion == E_RAIL_DIR.F)
                    {
                        loacation_x = (int)m_pPoints[0].RealPointf.X;
                        loaction_y = (int)m_pPoints[0].RealPointf.Y -
                                        (firstRail.p_RailWidth / 2) -
                                        lbl_height;
                    }
                    else
                    {
                        loacation_x = (int)m_pPoints[0].RealPointf.X - lbl_width;
                        loaction_y = (int)m_pPoints[0].RealPointf.Y -
                                        (firstRail.p_RailWidth / 2) -
                                        lbl_height;
                    }
                }
                lbl_SectionID = new Label();
                lbl_SectionID.Text = Section_ID;
                lbl_SectionID.Font = new System.Drawing.Font("Arial", 7F);
                lbl_SectionID.ForeColor = Color.White;
                lbl_SectionID.BackColor = Color.Transparent;
                lbl_SectionID.Margin = new Padding(1);
                lbl_SectionID.Size = new System.Drawing.Size(lbl_width, lbl_height);
                lbl_SectionID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                lbl_SectionID.Location = new System.Drawing.Point(loacation_x, loaction_y);
                lbl_SectionID.Visible = false;
            }
            catch (Exception ex)
            {

            }
        }

        private bool isCurve(out uctlRail curveRail)
        {
            curveRail = null;
            foreach (var ucrlRail in uctlRails)
            {
                if (ucrlRail.Key.p_RailType == E_RAIL_TYPE.Straight_Horizontal ||
                    ucrlRail.Key.p_RailType == E_RAIL_TYPE.Straight_Vertical)
                {
                    continue;
                }
                else
                {
                    curveRail = ucrlRail.Key;
                    return true;
                }
            }
            return false;
        }


        public void DecisionFirstAndLastPoint(bool isfirst)
        {
            firstRail = uctlRails.First().Key;
            lastRail = uctlRails.Last().Key;
            if (firstRail == lastRail)
            {
                if (RailDircetion == E_RAIL_DIR.F)
                {
                    m_pPoints[0] = (PointObject)firstRail.p_Points[0].Clone();
                    m_pPoints[1] = (PointObject)firstRail.p_Points[1].Clone();

                }
                else
                {
                    m_pPoints[0] = (PointObject)firstRail.p_Points[1].Clone();
                    m_pPoints[1] = (PointObject)firstRail.p_Points[0].Clone();
                }
            }
            else
            {
                m_pPoints[0] = (PointObject)findUnboundedPoint(firstRail).Clone();
                m_pPoints[1] = (PointObject)findUnboundedPoint(lastRail).Clone();
            }
            if (isfirst)
            {
                foreach (var rail in uctlRails)
                {
                    rail.Key.p_SectionID = Section_ID;
                    rail.Key.p_SegNumDisplay = rail.Key.p_SegNumDisplay + string.Concat(Segment_Num, "-(", SegDircetion.ToString(), ") ");
                    rail.Key.p_SegNum = Segment_Num;
                }
            }
        }

        public void VehicleArrivalsStartAdr(uctlVehicle vh)
        {
            vh.PrcSetLocation((int)m_pPoints[0].RealPointf.X, (int)m_pPoints[0].RealPointf.Y);
        }

        public void VehicleArrivalsEndAdr(uctlVehicle vh)
        {
            vh.PrcSetLocation((int)m_pPoints[1].RealPointf.X, (int)m_pPoints[1].RealPointf.Y);
        }


        public void VehicleEnterSection(uctlNewVehicle vh, string address_id, double sec_dis)
        {
            //vehicles.Add(vh);
            if (!dicVehicles.ContainsKey(address_id))
            {
                dicVehicles.Add(address_id, new List<uctlVehicle>());
            }
           // dicVehicles[address_id].Add(vh);
            vh.CurrentSecID = Section_ID;
            refreshVehicleLoaction(vh, sec_dis);
        }
        public void VehicleEnterSection(uctlVehicle vh, string address_id, double sec_dis)
        {
            //vehicles.Add(vh);
            if (!dicVehicles.ContainsKey(address_id))
            {
                dicVehicles.Add(address_id, new List<uctlVehicle>());
            }
            dicVehicles[address_id].Add(vh);
            vh.p_CurrentSecID = Section_ID;
            refreshVehicleLoaction(vh, sec_dis);
        }

        private void refreshVehicleLoaction(uctlNewVehicle vh, double sec_dis)
        {

            double distance_scale = 1;
            double distanceTemp = 0;
            if (RealDistance > 10)
            {
                distance_scale = sec_dis / RealDistance;
            }
            else
            {

            }
            distanceTemp = Distance * distance_scale;
            int railInterval = 0;


            uctlRail matchRail = findTheMatchingRail((int)distanceTemp, out railInterval);
            int railInterval_Pix = (int)BCUtility.RealLengthToPixelsWidthByScale((double)railInterval);

            int Location_X = 0;
            int Location_Y = 0;

            KeyValuePair<uctlRail, E_RAIL_DIR> keyValuePairTemp = uctlRails.Where(keyValue => keyValue.Key == matchRail).SingleOrDefault();
            if (keyValuePairTemp.Equals(default(KeyValuePair<uctlRail, E_RAIL_DIR>)))
            {
                return;
            }
            E_RAIL_DIR railDIR = keyValuePairTemp.Value;
            //switch (RailDircetion)
            switch (railDIR)
            {
                case E_RAIL_DIR.F:
                    Location_X = (int)matchRail.p_Points[0].RealPointf.X;
                    Location_Y = (int)matchRail.p_Points[0].RealPointf.Y;
                    if (matchRail.p_RailType == E_RAIL_TYPE.Straight_Horizontal)
                        vh.PrcSetLocation(Location_X + railInterval_Pix, Location_Y);
                    else if (matchRail.p_RailType == E_RAIL_TYPE.Straight_Vertical)
                        vh.PrcSetLocation(Location_X, Location_Y + railInterval_Pix);
                    else
                    {
                        if (distance_scale == 1)
                        {
                            vh.PrcSetLocation((int)matchRail.p_Points[1].RealPointf.X, (int)matchRail.p_Points[1].RealPointf.Y);
                        }
                        else
                        {
                            vh.PrcSetLocation((int)matchRail.p_RealCenterPoint.X, (int)matchRail.p_RealCenterPoint.Y);
                        }                        
                    }
                    break;
                case E_RAIL_DIR.R:
                    Location_X = (int)matchRail.p_Points[1].RealPointf.X;
                    Location_Y = (int)matchRail.p_Points[1].RealPointf.Y;
                    if (matchRail.p_RailType == E_RAIL_TYPE.Straight_Horizontal)
                        vh.PrcSetLocation(Location_X - railInterval_Pix, Location_Y);
                    else if (matchRail.p_RailType == E_RAIL_TYPE.Straight_Vertical)
                        vh.PrcSetLocation(Location_X, Location_Y - railInterval_Pix);
                    else
                    {
                        if (distance_scale == 1)
                        {
                            vh.PrcSetLocation((int)matchRail.p_Points[1].RealPointf.X, (int)matchRail.p_Points[1].RealPointf.Y);
                        }
                        else
                        {
                            vh.PrcSetLocation((int)matchRail.p_RealCenterPoint.X, (int)matchRail.p_RealCenterPoint.Y);
                        }
                    }
                    break;
            }
        }
        private void refreshVehicleLoaction(uctlVehicle vh, double sec_dis)
        {

            double distance_scale = 1;
            double distanceTemp = 0;
            if (RealDistance > 10)
            {
                distance_scale = sec_dis / RealDistance;
            }
            else
            {

            }
            distanceTemp = Distance * distance_scale;
            int railInterval = 0;


            uctlRail matchRail = findTheMatchingRail((int)distanceTemp, out railInterval);
            int railInterval_Pix = (int)BCUtility.RealLengthToPixelsWidthByScale((double)railInterval);

            int Location_X = 0;
            int Location_Y = 0;

            KeyValuePair<uctlRail, E_RAIL_DIR> keyValuePairTemp = uctlRails.Where(keyValue => keyValue.Key == matchRail).SingleOrDefault();
            if (keyValuePairTemp.Equals(default(KeyValuePair<uctlRail, E_RAIL_DIR>)))
            {
                return;
            }
            E_RAIL_DIR railDIR = keyValuePairTemp.Value;
            //switch (RailDircetion)
            switch (railDIR)
            {
                case E_RAIL_DIR.F:
                    Location_X = (int)matchRail.p_Points[0].RealPointf.X;
                    Location_Y = (int)matchRail.p_Points[0].RealPointf.Y;
                    if (matchRail.p_RailType == E_RAIL_TYPE.Straight_Horizontal)
                        vh.PrcSetLocation(Location_X + railInterval_Pix, Location_Y);
                    else if (matchRail.p_RailType == E_RAIL_TYPE.Straight_Vertical)
                        vh.PrcSetLocation(Location_X, Location_Y + railInterval_Pix);
                    else
                    {
                        vh.PrcSetLocation((int)matchRail.p_RealCenterPoint.X, (int)matchRail.p_RealCenterPoint.Y);
                    }
                    break;
                case E_RAIL_DIR.R:
                    Location_X = (int)matchRail.p_Points[1].RealPointf.X;
                    Location_Y = (int)matchRail.p_Points[1].RealPointf.Y;
                    if (matchRail.p_RailType == E_RAIL_TYPE.Straight_Horizontal)
                        vh.PrcSetLocation(Location_X - railInterval_Pix, Location_Y);
                    else if (matchRail.p_RailType == E_RAIL_TYPE.Straight_Vertical)
                        vh.PrcSetLocation(Location_X, Location_Y - railInterval_Pix);
                    else
                    {
                        vh.PrcSetLocation((int)matchRail.p_RealCenterPoint.X, (int)matchRail.p_RealCenterPoint.Y);
                    }
                    break;
            }
        }

        private void refreshVehicleLoaction_Dic()
        {
            foreach (string adr_id in dicVehicles.Keys)
            {
                for (int i = 0; i < dicVehicles[adr_id].Count(); i++)
                {
                    int railInterval = 0;
                    uctlRail matchRail = findTheMatchingRail(dicVehicles[adr_id].Count() - i, out railInterval);
                    int railInterval_Pix = (int)BCUtility.RealLengthToPixelsWidthByScale((double)railInterval);

                    int Location_X = 0;
                    int Location_Y = 0;
                    switch (RailDircetion)
                    {
                        case E_RAIL_DIR.F:
                            Location_X = (int)matchRail.p_Points[0].RealPointf.X;
                            Location_Y = (int)matchRail.p_Points[0].RealPointf.Y;
                            if (matchRail.p_RailType == E_RAIL_TYPE.Straight_Horizontal)
                                dicVehicles[adr_id][i].PrcSetLocation(Location_X + railInterval_Pix, Location_Y);
                            else if (matchRail.p_RailType == E_RAIL_TYPE.Straight_Vertical)
                                dicVehicles[adr_id][i].PrcSetLocation(Location_X, Location_Y + railInterval_Pix);
                            else
                            {
                                dicVehicles[adr_id][i].PrcSetLocation((int)matchRail.p_RealCenterPoint.X, (int)matchRail.p_RealCenterPoint.Y);
                            }
                            break;
                        case E_RAIL_DIR.R:
                            Location_X = (int)matchRail.p_Points[1].RealPointf.X;
                            Location_Y = (int)matchRail.p_Points[1].RealPointf.Y;
                            if (matchRail.p_RailType == E_RAIL_TYPE.Straight_Horizontal)
                                dicVehicles[adr_id][i].PrcSetLocation(Location_X - railInterval_Pix, Location_Y);
                            else if (matchRail.p_RailType == E_RAIL_TYPE.Straight_Vertical)
                                dicVehicles[adr_id][i].PrcSetLocation(Location_X, Location_Y - railInterval_Pix);
                            else
                            {
                                dicVehicles[adr_id][i].PrcSetLocation((int)matchRail.p_RealCenterPoint.X, (int)matchRail.p_RealCenterPoint.Y);
                            }
                            break;
                    }
                    dicVehicles[adr_id][i].Tag = dicVehicles[adr_id][i].Top + "|" + dicVehicles[adr_id][i].Left + "|"
                        + dicVehicles[adr_id][i].Height + "|" + dicVehicles[adr_id][i].Width;
                }
            }
        }

        private uctlRail findTheMatchingRail(double sec_dis, out int _railInterval)
        {
            uctlRail matchRail = null;
            //int Interval = vehicleInterval * vhOrder;
            _railInterval = 0;

            foreach (var rail in uctlRails)
            {
                if (rail.Key.p_RailLength >= sec_dis)
                {
                    matchRail = rail.Key;
                    _railInterval = (int)sec_dis;
                    break;
                }
                else
                {
                    if (uctlRails.Count == 1)
                    {
                        matchRail = rail.Key;
                        _railInterval = matchRail.p_RailLength;
                        break;
                    }
                    sec_dis = sec_dis - rail.Key.p_RailLength;
                    continue;
                }
            }
            if (matchRail == null)
            {
                matchRail = uctlRails.Last().Key;
                _railInterval = matchRail.p_RailLength;
            }
            return matchRail;
        }

        public void VehicleLeave(uctlVehicle vh)
        {
            //vehicles.Remove(vh);
        }
        public void VehicleLeave(uctlNewVehicle vh)
        {
            //vehicles.Remove(vh);
        }


        private PointObject findUnboundedPoint(uctlRail rail)
        {
            if (rail.p_Points[0].BindPointObj == null)
                return rail.p_Points[0];
            else if (rail.p_Points[1].BindPointObj == null)
                return rail.p_Points[1];
            return null;
        }
    }
    public class SelectedRailEventArgs : EventArgs
    {
        public string Segment_Num { get; set; }
    }


}
