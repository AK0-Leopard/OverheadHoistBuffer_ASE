//*********************************************************************************
//      uiCircularProgressBar.cs
//*********************************************************************************
// File Name: uiCircularProgressBar.cs
// Description: Circular Progress Bar
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2014/06/21    Hayes Chen     N/A            N/A     Initial Release
//**********************************************************************************
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

namespace com.mirle.ibg3k0.bc.winform.UI.Components
{
    /// <summary>
    /// Class uiCircularProgressBar.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.UserControl" />
    [ToolboxBitmap(typeof(com.mirle.ibg3k0.bc.winform.UI.Components.uiCircularProgressBar), "Images/CircleToolboxBitmap.bmp")]
    public class uiCircularProgressBar : UserControl
    {
        /// <summary>
        /// The timer
        /// </summary>
        private System.Windows.Forms.Timer timer;

        /// <summary>
        /// The seperator angle
        /// </summary>
        private int SeperatorAngle = 5;

        /// <summary>
        /// The _index
        /// </summary>
        private int _index;
        /// <summary>
        /// The _number of arcs
        /// </summary>
        private int _numberOfArcs;
        /// <summary>
        /// The _ring thickness
        /// </summary>
        private int _ringThickness;
        /// <summary>
        /// The _ring color
        /// </summary>
        private Color _ringColor;
        /// <summary>
        /// The _number of tail
        /// </summary>
        private int _numberOfTail;

        /// <summary>
        /// The components
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        }

        /// <summary>
        /// This is the number of pies, following the moving pie.
        /// </summary>
        /// <value>The number of tail.</value>
        /// <exception cref="ArgumentOutOfRangeException">Value can not be zero</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Value can not be less than zero</exception>
        [
        Category("ProgressRing"),
        DefaultValue(4),
        Bindable(true)
        ]
        public int NumberOfTail
        {
            get
            {
                return this._numberOfTail;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Value can not be zero");

                this._numberOfTail = value;

                this.UpdateStyles();
                this.Invalidate();
            }
        }

        /// <summary>
        /// Background color for the moving pie.
        /// </summary>
        /// <value>The color of the ring.</value>
        /// <remarks>Default color is white</remarks>
        [
        Category("ProgressRing"),
        Bindable(true)
        ]
        public Color RingColor
        {
            get
            {
                // Default ring color is White
                if (this._ringColor == Color.Empty)
                    return Color.White;

                return this._ringColor;
            }
            set
            {
                this._ringColor = value;

                // Redraw the control
                this.UpdateStyles();
                this.Invalidate();
            }
        }

        /// <summary>
        /// Background color for the background pies.
        /// </summary>
        /// <value>The color of the fore.</value>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [
        Category("ProgressRing"),
        Bindable(true)
        ]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;

                // Redraw the control
                this.UpdateStyles();
                this.Invalidate();
            }
        }

        /// <summary>
        /// Number of pies that will be places inside the cicle.
        /// </summary>
        /// <value>The number of arcs.</value>
        /// <exception cref="ArgumentOutOfRangeException">Value must be greater than zero</exception>
        /// <exception cref="ArgumentException">360 should be divisible by NumberOfArcs property. 360 is not divisible by  + value.ToString()</exception>
        /// <exception cref="System.ArgumentException">360 should be divisible by the value given.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Value must be greater than zero.</exception>
        /// <remarks>Value should be a divisor of 360 (In other words when 360 is divided to value, the result must be integer).</remarks>
        [
        Category("ProgressRing"),
        DefaultValue(8),
        Bindable(true)
        ]
        public int NumberOfArcs
        {
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Value must be greater than zero");

                // 360 degree is the total angle of a circle.
                if ((360 % value) != 0)
                    throw new ArgumentException("360 should be divisible by NumberOfArcs property. 360 is not divisible by " + value.ToString());

                this._numberOfArcs = value;

                this.UpdateStyles();
                this.Invalidate();
            }
            get
            {
                return this._numberOfArcs;
            }
        }

        /// <summary>
        /// Radius of the circle.
        /// </summary>
        /// <value>The ring thickness.</value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Value must be greater than zero
        /// or
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Value must be greater than zero<br /></exception>
        /// <remarks>Value must be greater than the half of the width or height.</remarks>
        [
        Category("ProgressRing"),
        DefaultValue(5),
        Bindable(true)
        ]
        public int RingThickness
        {
            get
            {
                return this._ringThickness;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Value must be greater than zero");

                // Value cannot be bigger than the rectanle.
                int limit = Math.Min(this.Width, this.Height) / 2;
                if (value >= limit)
                    throw new ArgumentOutOfRangeException(string.Format("Value must be smaller than {0} for this size, {1}", limit, this.ClientRectangle.ToString()));

                this._ringThickness = value;

                // Redraw control
                this.UpdateStyles();
                this.Invalidate();
            }
        }

        /// <summary>
        /// To start the animation, set this true.<br />
        /// To stop, set it false.
        /// </summary>
        /// <value><c>true</c> if rotate; otherwise, <c>false</c>.</value>
        /// <remarks>After stopping the animation, you may clear the rotating part, by calling <c>Clear</c> method.</remarks>
        [
        System.ComponentModel.Browsable(false),
        DefaultValue(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public bool Rotate
        {
            get
            {
                return this.timer.Enabled;
            }
            set
            {
                this.timer.Enabled = value;
            }
        }

        /// <summary>
        /// This value is in miliseconds. Greater interval, slow animation.
        /// </summary>
        /// <value>The interval.</value>
        [
        Category("ProgressRing"),
        DefaultValue(150),
        Bindable(true)
        ]
        public int Interval
        {
            get
            {
                return this.timer.Interval;
            }
            set
            {
                this.timer.Interval = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is transparent.
        /// </summary>
        /// <value><c>true</c> if this instance is transparent; otherwise, <c>false</c>.</value>
        private bool IsTransparent
        {
            get
            {
                return (this.BackColor == Color.Transparent);
            }
        }

        /// <summary>
        /// Gets the pie angle.
        /// </summary>
        /// <value>The pie angle.</value>
        private int PieAngle
        {
            get
            {
                // value is the pie that will be drawn and the seperator angle
                int angleOfPieWithSeperator = 360 / this.NumberOfArcs;

                // This is the pie that will be drawn to the client
                int pieAngle = angleOfPieWithSeperator - this.SeperatorAngle;

                return pieAngle;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public uiCircularProgressBar()
        {
            InitializeComponent();
            // To minimize the flicking
            this.SetStyle(ControlStyles.DoubleBuffer, true);

            // Enable transparent BackColor
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            // Redraw the control after its size is changed
            this.ResizeRedraw = true;

            // Never use this property. This indicated which pie will be drawn.
            this._index = 1;

            this._numberOfArcs = 8;
            this._ringThickness = 5;
            this._ringColor = Color.Empty;
            this._numberOfTail = 4;

            this.timer = new Timer();
            this.timer.Interval = 150; // Each 150 miliseconds, the progress circle will be drawn again
            this.timer.Tick += new EventHandler(timer_Tick);
            this.timer.Enabled = true;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            // Clears the pies with default ring color
            using (Graphics grp = this.CreateGraphics())
                this.FillEmptyArcs(grp);
        }

        /// <summary>
        /// 繪製控制項的背景。
        /// </summary>
        /// <param name="pevent"><see cref="T:System.Windows.Forms.PaintEventArgs" />，含有要繪製的控制項資訊。</param>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (!this.IsTransparent)
                base.OnPaintBackground(pevent);
        }

        /// <summary>
        /// 引發 <see cref="E:System.Windows.Forms.UserControl.Load" /> 事件。
        /// </summary>
        /// <param name="e">包含事件資料的 <see cref="T:System.EventArgs" />。</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.DesignMode)
            {
                this.timer.Enabled = false;
            }
        }

        /// <summary>
        /// Gets the create parameters.
        /// </summary>
        /// <value>The create parameters.</value>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams p = base.CreateParams;
                if (IsTransparent)
                    p.ExStyle |= 0x20;
                return p;
            }
        }

        /// <summary>
        /// 引發 <see cref="E:System.Windows.Forms.Control.Move" /> 事件。
        /// </summary>
        /// <param name="e">包含事件資料的 <see cref="T:System.EventArgs" />。</param>
        protected override void OnMove(EventArgs e)
        {
            if (!IsTransparent)
                base.OnMove(e);
            else
                this.RecreateHandle();
        }

        /// <summary>
        /// 引發 <see cref="E:System.Windows.Forms.Control.BackColorChanged" /> 事件。
        /// </summary>
        /// <param name="e">包含事件資料的 <see cref="T:System.EventArgs" />。</param>
        protected override void OnBackColorChanged(EventArgs e)
        {
            this.UpdateStyles();
            base.OnBackColorChanged(e);
        }


        /// <summary>
        /// 引發 <see cref="E:System.Windows.Forms.Control.Paint" /> 事件。
        /// </summary>
        /// <param name="e">包含事件資料的 <see cref="T:System.Windows.Forms.PaintEventArgs" />。</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            // Fill static ring part
            this.FillEmptyArcs(e.Graphics);

            // Fill animation part
            this.FillPieAndTail();
        }

        /// <summary>
        /// This method draws the static, non-animation part.
        /// </summary>
        /// <param name="grp">The GRP.</param>
        private void FillEmptyArcs(Graphics grp)
        {
            int startAngle = 0;

            for (int i = 0; i < this.NumberOfArcs; i++)
            {
                this.DrawFilledArc(grp, this.RingColor, startAngle);

                startAngle += this.PieAngle + this.SeperatorAngle;
            }
        }

        /// <summary>
        /// Draws the filled arc.
        /// </summary>
        /// <param name="grp">The GRP.</param>
        /// <param name="color">The color.</param>
        /// <param name="startAngle">The start angle.</param>
        private void DrawFilledArc(Graphics grp, Color color, int startAngle)
        {
            grp.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            Rectangle main = this.ClientRectangle;

            // If there is no region to be drawn, then this method terminates itself
            if (main.Width - (2 * this._ringThickness) < 1 || main.Height - (2 * this._ringThickness) <= 1)
                return;

            // Calculates the region that will be filled
            GraphicsPath outerPath = new GraphicsPath();
            outerPath.AddPie(main, startAngle, this.PieAngle);

            Rectangle sub = new Rectangle(main.X + this._ringThickness, main.Y + this._ringThickness, main.Width - (2 * this._ringThickness), main.Height - (2 * this._ringThickness));
            GraphicsPath innerPath = new GraphicsPath();
            innerPath.AddPie(sub, startAngle - 1, this.PieAngle + 2);

            System.Drawing.Region mainRegion = new Region(outerPath);
            System.Drawing.Region subRegion = new Region(innerPath);
            mainRegion.Exclude(subRegion);

            // Fill that region
            grp.FillRegion(new SolidBrush(color), mainRegion);
        }

        /// <summary>
        /// Changes the index.
        /// </summary>
        private void ChangeIndex()
        {
            // Fills the animation part
            this.FillPieAndTail();

            // After the invocation of this method, index is changed. So at another invocation of this method, next pie will be drawn
            this._index = (this._index + 1) % this.NumberOfArcs;
        }

        /// <summary>
        /// Draws the animation part
        /// </summary>
        private void FillPieAndTail()
        {
            Color color = this.ForeColor;

            for (int i = 0; i <= this.NumberOfTail; i++)
            {
                this.FillPieAccordingToTheIndex(this._index - i, color);

                // If there is tail, then the tail color is the lighter of the ForeColor
                color = ControlPaint.Light(color);
            }

            // Background Pie
            this.FillPieAccordingToTheIndex(this._index - (this.NumberOfTail + 1), this.RingColor);
        }

        /// <summary>
        /// Fills the index of the pie according to the.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="color">The color.</param>
        private void FillPieAccordingToTheIndex(int index, Color color)
        {
            int count = index % this.NumberOfArcs;
            int angle = count * (this.PieAngle + this.SeperatorAngle);

            if (!this.IsDisposed)
            {
                using (Graphics grp = this.CreateGraphics())
                {
                    grp.SmoothingMode = SmoothingMode.HighQuality;
                    this.DrawFilledArc(grp, color, angle);
                }
            }

        }

        /// <summary>
        /// Handles the Tick event of the timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void timer_Tick(object sender, EventArgs e)
        {
            this.ChangeIndex();
        }

    }
}
