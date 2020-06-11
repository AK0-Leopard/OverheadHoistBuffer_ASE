//*********************************************************************************
//      RoundButton_AllRound.cs
//*********************************************************************************
// File Name: RoundButton_AllRound.cs
// Description: the custom button 
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date                 Author              Request No.    Tag                  Description
// ------------------   ------------------   ------------------   ------------------   ------------------
// 2018/08/13       Boan                N/A                  N/A                  Initialize.
//**********************************************************************************
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RoundPanel
{
    public partial class RoundButton_AllRound : Button
    {

        enum model
        {
            hover,
            enter,
            press,
            enable
        }

        public Color HoverBackColor { get; set; }
        public Color EnterBackColor { get; set; }
        public Color PressBackColor { get; set; }
        public Color HoverForeColor { get; set; }
        public Color EnterForeColor { get; set; }
        public Color PressForeColor { get; set; }

        public int Radius { get; set; }

        model paintmodel = model.hover;
        public RoundButton_AllRound()
        {
            //InitializeComponent();
            //這些得帶上，不然會有黑邊
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.BorderColor = Color.FromArgb(0, 0, 0, 0);
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            FlatAppearance.MouseOverBackColor = Color.Transparent;

            SetDefaultColor();


        }

        public void SetDefaultColor()
        {//給個初始值
            HoverBackColor = Color.LightBlue;
            EnterBackColor = Color.Blue;
            PressBackColor = Color.DarkBlue;
            HoverForeColor = Color.White;
            EnterForeColor = Color.White;
            PressForeColor = Color.White;
            Radius = 18;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);//這個不能去，而且得放在前面，不然會有黑框之類的莫名其妙的東西
            var colorback = HoverBackColor;
            var colorfore = HoverForeColor;
            switch (paintmodel)
            {
                case model.hover:
                    colorback = HoverBackColor;
                    colorfore = HoverForeColor;
                    break;
                case model.enter:
                    colorback = EnterBackColor;
                    colorfore = EnterForeColor;
                    break;
                case model.press:
                    colorback = PressBackColor;
                    colorfore = PressForeColor;
                    break;
                case model.enable:
                    colorback = Color.LightGray;
                    break;
                default:
                    colorback = HoverBackColor;
                    colorfore = HoverForeColor;
                    break;
            }
            Draw(e.ClipRectangle, e.Graphics, false, colorback);
            DrawText(e.ClipRectangle, e.Graphics, colorfore);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            paintmodel = model.enter;
            base.OnMouseEnter(e);


        }
        protected override void OnMouseLeave(EventArgs e)
        {
            paintmodel = model.hover;
            base.OnMouseLeave(e);

        }
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            paintmodel = model.press;
            base.OnMouseDown(mevent);
        }
        protected override void OnEnabledChanged(EventArgs e)
        {
            paintmodel = Enabled ? model.hover : model.enable;
            Invalidate();//false 轉換為true的時候不會刷新 這裏強制刷新下
            base.OnEnabledChanged(e);

        }

        void Draw(Rectangle rectangle, Graphics g, bool cusp, Color begin_color, Color? end_colorex = null)
        {
            Color end_color = end_colorex == null ? begin_color : (Color)end_colorex;
            int span = 2;
            //抗鋸齒
            g.SmoothingMode = SmoothingMode.AntiAlias;
            //漸變填充
            LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush(rectangle, begin_color, end_color, LinearGradientMode.Vertical);
            //畫尖角
            if (cusp)
            {
                span = 10;
                PointF p1 = new PointF(rectangle.Width - 12, rectangle.Y + 10);
                PointF p2 = new PointF(rectangle.Width - 12, rectangle.Y + 30);
                PointF p3 = new PointF(rectangle.Width, rectangle.Y + 20);
                PointF[] ptsArray = { p1, p2, p3 };
                g.FillPolygon(myLinearGradientBrush, ptsArray);
            }
            //填充
            g.FillPath(myLinearGradientBrush, DrawRoundRect(rectangle.X, rectangle.Y, rectangle.Width - span, rectangle.Height - 1, Radius));

        }
        void DrawText(Rectangle rectangle, Graphics g, Color color)
        {
            SolidBrush sbr = new SolidBrush(color);
            var rect = new RectangleF();
            switch (TextAlign)
            {
                case ContentAlignment.MiddleCenter:
                    rect = getTextRec(rectangle, g);
                    break;
                default:
                    rect = getTextRec(rectangle, g);
                    break;
            }
            g.DrawString(Text, Font, sbr, rect);
        }
        RectangleF getTextRec(Rectangle rectangle, Graphics g)
        {
            var rect = new RectangleF();
            var size = g.MeasureString(Text, Font);
            if (size.Width > rectangle.Width || size.Height > rectangle.Height)
            {
                rect = rectangle;
            }
            else
            {
                rect.Size = size;
                rect.Location = new PointF(rectangle.X + (rectangle.Width - size.Width) / 2, rectangle.Y + (rectangle.Height - size.Height) / 2);
            }
            return rect;
        }

        GraphicsPath DrawRoundRect(int x, int y, int width, int height, int radius)
        {
            //四邊圓角
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(x, y, radius, radius, 180, 90);
            gp.AddArc(width - radius, y, radius, radius, 270, 90);
            gp.AddArc(width - radius, height - radius, radius, radius, 0, 90);
            gp.AddArc(x, height - radius, radius, radius, 90, 90);
            gp.CloseAllFigures();
            return gp;
        }





        


    }
}
