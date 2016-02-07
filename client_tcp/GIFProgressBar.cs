using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;


namespace Jcobs.Controls.ProgressBars
{
    /// <summary>
    /// Time Progress Bar
    /// </summary>
    public partial class GIFProgressBar : Control
    {
        public GIFProgressBar()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.Selectable | ControlStyles.FixedHeight, false);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.BackColor = Color.Transparent;
            this.TabStop = false;

            this.Disposed += new EventHandler(TimeProgressBar_Disposed);
        }

        void TimeProgressBar_Disposed(object sender, EventArgs e)
        {
            StopThread();
        }
    }

    /// <summary>
    /// Method
    /// </summary>
    partial class GIFProgressBar
    {
        public void Start()
        {
            m_StartDateTime = DateTime.Now;
            m_LastFrameIndex = -1;

            //Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:dd:ss ffff") + "   - start");

            StartThread();
        }

        public void Stop()
        {
            StopThread();
        }
    }

    /// <summary>
    /// Event
    /// </summary>
    partial class GIFProgressBar
    {
        public delegate void CompletedProgressHandler(DateTime start, DateTime end);
        public event CompletedProgressHandler CompletedProgress;
        protected void OnCompletedProgress(DateTime start, DateTime end)
        {
            if (CompletedProgress != null) CompletedProgress(start, end);
        }
    }

    /// <summary>
    /// Thread
    /// </summary>
    partial class GIFProgressBar
    {
        Thread thread;
        bool m_StopThread = false; //thread를 내부에서 스스로 죽을수 있도록 하는 플래그
        //bool m_ThreadHasCreated = false; //thread handle 생성된적이 있었나? handle 이 생성된적이 있다면 stop thread 방법이 다르다
        bool m_ThreadIsBusy = false; //현재 thread 가 활동 중인가?

        /// <summary>
        /// Start Thread
        /// </summary>
        void StartThread()
        {
            if (m_ThreadIsBusy == true)
                return;

            if (thread == null)
            {
                m_StopThread = false;
                m_ThreadIsBusy = true;
                //m_ThreadHasCreated = true;
                thread = new Thread(DoThread);
                thread.Start();
            }
            else
            {
                if (thread.ThreadState == ThreadState.Stopped) //Compact Framework이 아니라면 이 코드를 사용한다
                //if (m_ThreadHasCreated == true) //Compact Framework에서는 이 코드를 사용한다.
                {
                    m_ThreadIsBusy = true;
                    m_StopThread = false;
                    thread = new Thread(DoThread);
                    thread.Start();
                }
            }
        }

        /// <summary>
        /// Stop Thread
        /// </summary>
        void StopThread()
        {
            if (thread == null)
                return;

            m_StopThread = true;

            thread.Join(100);

            for (int i = 0; i < 100; i++)
            {
                Application.DoEvents();
                Thread.Sleep(1);

                if (m_ThreadIsBusy == false)
                    break;
            }

            thread.Abort();

            m_ThreadIsBusy = false;
        }

        /// <summary>
        /// Do Thread
        /// </summary>
        void DoThread()
        {
            while (!m_StopThread)
            {
                if (m_StopThread)
                    break; //스레드 안의 수행이 길다면 이 라인을 통해서 탈출할 수 있도록 한다.
                else
                    Thread.Sleep(10);

                double passedMSec = DateTime.Now.Subtract(m_StartDateTime).TotalMilliseconds;//시작 이후 진행된 시간의 크기
                
                if ((m_LastFrameIndex + 1) * m_MSecPerFrame < passedMSec)
                {
                    //Console.WriteLine("Internal Tick : " + passedMSec.ToString() + "[" + (m_LastFrameIndex + 1).ToString() + "]");

                    int nowFrameIndex = m_LastFrameIndex + 1;

                    if (nowFrameIndex >= m_FrameCount)
                    {
                        nowFrameIndex = m_LastFrameIndex;

                        //Console.WriteLine("Internal Expired Time : " + passedMSec.ToString());

                        OnCompletedProgress(m_StartDateTime, DateTime.Now);

                        m_ThreadIsBusy = false;
                        StopThread();
                    }

                    m_LastFrameIndex = nowFrameIndex;

                    this.Invalidate();
                }
            }

            //end thread
            m_ThreadIsBusy = false;
        }
    }

    /// <summary>
    /// Paint
    /// </summary>
    partial class GIFProgressBar
    {
        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;

            DrawFrame(g);
            DrawNoImage(g);

            base.OnPaint(pe);
        }

        void DrawFrame(Graphics g)
        {
            if (Image == null)
                return;

            int nowFrameIndex = 0;

            if (m_LastFrameIndex < 0)
                nowFrameIndex = 0;
            else
                nowFrameIndex = m_LastFrameIndex;

            this.Image.SelectActiveFrame(m_FrameDimension, nowFrameIndex);

            g.DrawImage(this.Image, 0, 0);
        }

        void DrawNoImage(Graphics g)
        {
            if (this.Image != null)
                return;

            Pen pen = new Pen(Color.Gray, 1);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            g.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            g.DrawString("GIF Progress Bar\n\nNo Image", this.Font, Brushes.Gray, this.ClientRectangle, sf);
        }
    }

    /// <summary>
    /// Properties
    /// </summary>
    partial class GIFProgressBar
    {
        int m_FrameCount = 1;
        int m_LastFrameIndex = -1;
        float m_MSecPerFrame = 1;
        FrameDimension m_FrameDimension = null;
        DateTime m_StartDateTime = DateTime.Now;

        private Image m_Image = null;
        /// <summary>
        /// 
        /// </summary>
        public Image Image
        {
            get
            {
                return m_Image;
            }
            set
            {
                if (m_ThreadIsBusy == true)
                {
                    StopThread();

                    m_Image = value;

                    StartThread();
                }
                else
                {
                    m_Image = value;
                }

                if (m_Image == null)
                    return;

                m_FrameDimension = new FrameDimension(this.Image.FrameDimensionsList[0]);
                m_FrameCount = this.Image.GetFrameCount(m_FrameDimension);
                m_MSecPerFrame = this.ExpiredMillisecond / m_FrameCount;

                //Console.WriteLine("Frame Count : " + m_FrameCount.ToString());
                //Console.WriteLine("Expired msec : " + this.ExpiredMillisecond.ToString());
                //Console.WriteLine("msec / frame = " + m_MSecPerFrame.ToString());

                this.Invalidate();
            }
        }

        private int m_ExpiredMillisecond = 1000;
        /// <summary>
        /// 완료 시간, 최소 1,000 미리세컨드, 최대 60,000 미리세컨드
        /// </summary>
        public int ExpiredMillisecond
        {
            get
            {
                return m_ExpiredMillisecond;
            }
            set
            {
                m_ExpiredMillisecond = Math.Max(1000, value);
                m_MSecPerFrame = m_ExpiredMillisecond / m_FrameCount;
            }
        }
    }
}