using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Jcobs.Controls.ProgressBars
{
    /// <summary>
    /// ProgressBar
    /// </summary>
    public partial class ProgressBar : Control
    {
        public ProgressBar()
        {
            InitializeComponent();
            InitializeUi();

            this.ForeColor = Color.FromArgb(50,50,50);
        }
    }

    /// <summary>
    /// Paint
    /// </summary>
    partial class ProgressBar
    {
        StringFormat sf = new StringFormat();

        /// <summary>
        /// Initialize Ui
        /// </summary>
        void InitializeUi()
        {
            //this.ResizeRedraw = true;
            //this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.Selectable | ControlStyles.FixedHeight, false);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.BackColor = Color.Transparent;
            this.TabStop = false;

            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
        }

        /// <summary>
        /// OnPaint
        /// </summary>
        /// <param name="pe"></param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;

            DrawProgressBar(g);
            DrawPercent(g);
            DrawText(g);
            DrawFrame(g);

            base.OnPaint(pe);
        }

        /// <summary>
        /// Draw ProgressBar
        /// </summary>
        /// <param name="g"></param>
        void DrawProgressBar(Graphics g)
        {
            if (this.Value > this.Maximum)
            {
                this.Value = this.Maximum;
                return;
            }

            int maxValue = this.Maximum;

            if (maxValue <= 0)
                return;

            Rectangle rt = this.ClientRectangle;
            LinearGradientBrush br = new LinearGradientBrush(rt, this.StartColor, this.EndColor, this.GradientMode);
            
            float left = 0.0f;
            float top = 0.0f;
            float width = 0.0f;
            float height = 0.0f;

            switch (this.StartPosition)
            {
                case StartPositionType.Left:
                    width = this.Width * Value / maxValue;
                    height = this.Height;
                    left = 0.0f;
                    top = 0.0f;
                    br = new LinearGradientBrush(rt, this.StartColor, this.EndColor, this.GradientMode);
                    break;

                case StartPositionType.Right:
                    width = this.Width * Value / maxValue;
                    height = this.Height;
                    left = this.Width - width;
                    top = 0.0f;
                    br = new LinearGradientBrush(rt, this.EndColor, this.StartColor, this.GradientMode);
                    break;

                case StartPositionType.Top:
                    width = this.Width;
                    height = this.Height * Value / maxValue;
                    left = 0.0f;
                    top = 0.0f;
                    br = new LinearGradientBrush(rt, this.StartColor, this.EndColor, this.GradientMode);
                    break;

                case StartPositionType.Bottom:
                    width = this.Width;
                    height = this.Height * Value / maxValue;
                    left = 0.0f;
                    top = this.Height - height;
                    br = new LinearGradientBrush(rt, this.EndColor, this.StartColor, this.GradientMode);
                    break;

                default:
                    break;
            }


            //RectangleF bounds = new RectangleF(0, 0, this.Width * Value / maxValue, this.Height);
            RectangleF bounds = new RectangleF(left, top, width, height);
            g.FillRectangle(br, bounds);
        }

        /// <summary>
        /// Darw Frame
        /// </summary>
        /// <param name="g"></param>
        void DrawFrame(Graphics g)
        {
            Rectangle bounds = new Rectangle(1, 1, this.Width - 2, this.Height - 2);
            Pen pen = new Pen(this.FrameColor , 2);

            g.DrawRectangle(pen, bounds);
        }

        /// <summary>
        /// Draw Percent
        /// </summary>
        /// <param name="g"></param>
        void DrawPercent(Graphics g)
        {
            if (this.ShowPercent == false)
                return;

            Font font = new Font(this.Font.FontFamily, this.Height / 2, FontStyle.Regular, GraphicsUnit.Pixel);
            SolidBrush br = new SolidBrush(Color.FromArgb(70, 70, 70));


            float percent = Math.Min(100, (Value * 100 / Math.Max(1, Maximum)));
            string strPercent = percent.ToString() + "%";

            g.DrawString(strPercent, font, br, this.ClientRectangle, sf);
        }

        /// <summary>
        /// Draw Text
        /// </summary>
        /// <param name="g"></param>
        void DrawText(Graphics g)
        {
            SolidBrush br = new SolidBrush(this.ForeColor);
            Rectangle bounds = new Rectangle(2, 2, this.Width-4, this.Height - 3);
            StringFormat sf = new StringFormat();
            switch (this.TextAlignment)
            {
                case ContentAlignment.BottomCenter:
                    sf.LineAlignment = StringAlignment.Far;
                    sf.Alignment = StringAlignment.Center;
                    break;

                case ContentAlignment.BottomLeft:
                    sf.LineAlignment = StringAlignment.Far;
                    sf.Alignment = StringAlignment.Near;
                    break;

                case ContentAlignment.BottomRight:
                    sf.LineAlignment = StringAlignment.Far;
                    sf.Alignment = StringAlignment.Far;
                    break;

                case ContentAlignment.MiddleCenter:
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;
                    break;

                case ContentAlignment.MiddleLeft:
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Near;
                    break;

                case ContentAlignment.MiddleRight:
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Far;
                    break;

                case ContentAlignment.TopCenter:
                    sf.LineAlignment = StringAlignment.Near;
                    sf.Alignment = StringAlignment.Center;
                    break;

                case ContentAlignment.TopLeft:
                    sf.LineAlignment = StringAlignment.Near;
                    sf.Alignment = StringAlignment.Near;
                    break;

                case ContentAlignment.TopRight:
                    sf.LineAlignment = StringAlignment.Near;
                    sf.Alignment = StringAlignment.Far;
                    break;

                default: break;
            }

            g.DrawString(this.Text, this.Font, br, bounds, sf);
        }
    }

    /// <summary>
    /// Properties
    /// </summary>
    partial class ProgressBar
    {
        [Flags]
        public enum StartPositionType
        {
            Left,

            Top,

            Right,

            Bottom
        }

        private StartPositionType m_StartPosition = StartPositionType.Left;
        public StartPositionType StartPosition
        {
            get
            {
                return m_StartPosition;
            }
            set
            {
                m_StartPosition = value;
                this.Invalidate();
            }
        }

        private LinearGradientMode m_GradientMode = LinearGradientMode.Horizontal;
        public LinearGradientMode GradientMode
        {
            get
            {
                return m_GradientMode;
            }
            set
            {
                m_GradientMode = value;
                this.Invalidate();
            }
        }

        private Color m_FrameColor = Color.FromArgb(24, 63, 102);
        public Color FrameColor
        {
            get
            {
                return m_FrameColor;
            }
            set
            {
                m_FrameColor = value;
                this.Invalidate();
            }
        }

        private Color m_StartColor = Color.LimeGreen;
        public Color StartColor
        {
            get
            {
                return m_StartColor;
            }
            set
            {
                m_StartColor = value;
                this.Invalidate();
            }
        }

        private Color m_EndColor = Color.OrangeRed;
        public Color EndColor
        {
            get
            {
                return m_EndColor;
            }
            set
            {
                m_EndColor = value;
                this.Invalidate();
            }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                this.Invalidate();
            }
        }

        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                this.Invalidate();
            }
        }

        private ContentAlignment m_TextAlignment = ContentAlignment.MiddleLeft;
        public ContentAlignment TextAlignment
        {
            get
            {
                return m_TextAlignment;
            }
            set
            {
                m_TextAlignment = value;
                this.Invalidate();
            }
        }

        private int m_Maximum = 100;
        /// <summary>
        /// Maxmum
        /// </summary>
        public int Maximum
        {
            get
            {
                return m_Maximum;
            }
            set
            {
                m_Maximum = value;
                this.Invalidate();
            }
        }

        private int m_Value = 30;
        /// <summary>
        /// Value
        /// </summary>
        public int Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
                this.Invalidate();
            }
        }

        private bool m_ShowPercent = true;
        /// <summary>
        /// Show Percent
        /// </summary>
        public bool ShowPercent
        {
            get
            {
                return m_ShowPercent;
            }
            set
            {
                m_ShowPercent = value;
                this.Invalidate();
            }
        }
    }
}