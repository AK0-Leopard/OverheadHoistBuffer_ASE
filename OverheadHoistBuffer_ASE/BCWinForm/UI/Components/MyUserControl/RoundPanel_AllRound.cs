//*********************************************************************************
//      RoundPanel_AllRound.cs
//*********************************************************************************
// File Name: RoundPanel_AllRound.cs
// Description: the custom panel 
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
    class RoundPanel_AllRound : Panel
    {

        private int mMatrixRound = 8;
        private Panel panel1;
        private Color mBack;

        public Color Back
        {
            get { return mBack; }
            set
            {
                if (value == null)
                {
                    mBack = Control.DefaultBackColor;
                }
                else
                {
                    mBack = value;
                }
                base.Refresh();
            }
        }



        public int MatrixRound
        {
            get { return mMatrixRound; }
            set
            {
                mMatrixRound = value;
                base.Refresh();
            }
        }

        private GraphicsPath CreateRound(Rectangle rect, int radius)
        {
            GraphicsPath roundRect = new GraphicsPath();

            //頂端 (x1: x軸起點, y1: y軸起點, x2: x軸終點, y2: y軸終點)
            roundRect.AddLine(rect.Left, rect.Top , rect.Right, rect.Top);

            //右上角
            roundRect.AddArc(rect.Right - radius, rect.Top, radius, radius, 270, 90);

            //右邊
            roundRect.AddLine(rect.Right, rect.Top + radius, rect.Right, rect.Bottom);

            //右下角
            roundRect.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);

            //底邊
            roundRect.AddLine(rect.Right - radius, rect.Bottom, rect.Left + radius, rect.Bottom);

            //左下角
            roundRect.AddArc(rect.Left, rect.Bottom - radius, radius, radius, 90, 90);

            //左邊
            roundRect.AddLine(rect.Left, rect.Top + radius, rect.Left, rect.Bottom);

            //左上角
            roundRect.AddArc(rect.Left, rect.Top, radius, radius, 180, 90);

            return roundRect;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            int width = base.Width - base.Margin.Left - base.Margin.Right;
            int height = base.Height - base.Margin.Top - base.Margin.Bottom;
            Rectangle rec = new Rectangle(base.Margin.Left, base.Margin.Top, width, height);
            GraphicsPath round = CreateRound(rec, mMatrixRound);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.FillPath((Brush)(new SolidBrush(mBack)), round);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.Refresh();
        }




    }
}
