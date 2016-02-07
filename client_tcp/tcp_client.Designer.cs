namespace tcp_client_new
{
    partial class tcp_client
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.sendmsg = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.hp_timer = new System.Windows.Forms.Timer(this.components);
            this.mp_timer = new System.Windows.Forms.Timer(this.components);
            this.position_test_timer = new System.Windows.Forms.Timer(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.mp_progressBar = new Jcobs.Controls.ProgressBars.ProgressBar();
            this.hp_progressBar = new Jcobs.Controls.ProgressBars.ProgressBar();
            this.tab_AI = new tcp_client_new.TabAI_Double_Buffer();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // sendmsg
            // 
            this.sendmsg.Location = new System.Drawing.Point(2, 348);
            this.sendmsg.Name = "sendmsg";
            this.sendmsg.Size = new System.Drawing.Size(523, 19);
            this.sendmsg.TabIndex = 1;
            this.sendmsg.TextChanged += new System.EventHandler(this.sendmsg_TextChanged);
            this.sendmsg.Enter += new System.EventHandler(this.sendmsg_TextChanged);
            this.sendmsg.KeyUp += new System.Windows.Forms.KeyEventHandler(this.sendmsg_KeyUp);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(2, 4);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(293, 53);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "Connecting...";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // hp_timer
            // 
            this.hp_timer.Enabled = true;
            this.hp_timer.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // mp_timer
            // 
            this.mp_timer.Enabled = true;
            this.mp_timer.Tick += new System.EventHandler(this.mp_timer_Tick);
            // 
            // position_test_timer
            // 
            this.position_test_timer.Enabled = true;
            this.position_test_timer.Tick += new System.EventHandler(this.position_test_timer_Tick);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tab_AI);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(2, 63);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(523, 283);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(515, 257);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tcp client";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // textBox1
            // 
            this.textBox1.CausesValidation = false;
            this.textBox1.Location = new System.Drawing.Point(9, 9);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(500, 242);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged_1);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.button1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(515, 257);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "join client";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Join Client";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // mp_progressBar
            // 
            this.mp_progressBar.BackColor = System.Drawing.Color.Transparent;
            this.mp_progressBar.EndColor = System.Drawing.Color.DarkSlateBlue;
            this.mp_progressBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.mp_progressBar.FrameColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(63)))), ((int)(((byte)(102)))));
            this.mp_progressBar.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.mp_progressBar.Location = new System.Drawing.Point(301, 31);
            this.mp_progressBar.Maximum = 100;
            this.mp_progressBar.Name = "mp_progressBar";
            this.mp_progressBar.ShowPercent = true;
            this.mp_progressBar.Size = new System.Drawing.Size(220, 26);
            this.mp_progressBar.StartColor = System.Drawing.Color.DarkBlue;
            this.mp_progressBar.StartPosition = Jcobs.Controls.ProgressBars.ProgressBar.StartPositionType.Left;
            this.mp_progressBar.TabIndex = 5;
            this.mp_progressBar.TabStop = false;
            this.mp_progressBar.Text = "MP";
            this.mp_progressBar.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.mp_progressBar.Value = 30;
            // 
            // hp_progressBar
            // 
            this.hp_progressBar.BackColor = System.Drawing.Color.Transparent;
            this.hp_progressBar.EndColor = System.Drawing.Color.LightCoral;
            this.hp_progressBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.hp_progressBar.FrameColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(63)))), ((int)(((byte)(102)))));
            this.hp_progressBar.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.hp_progressBar.Location = new System.Drawing.Point(301, 4);
            this.hp_progressBar.Maximum = 100;
            this.hp_progressBar.Name = "hp_progressBar";
            this.hp_progressBar.ShowPercent = true;
            this.hp_progressBar.Size = new System.Drawing.Size(220, 26);
            this.hp_progressBar.StartColor = System.Drawing.Color.Red;
            this.hp_progressBar.StartPosition = Jcobs.Controls.ProgressBars.ProgressBar.StartPositionType.Left;
            this.hp_progressBar.TabIndex = 4;
            this.hp_progressBar.TabStop = false;
            this.hp_progressBar.Text = "HP";
            this.hp_progressBar.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.hp_progressBar.Value = 30;
            // 
            // tab_AI
            // 
            this.tab_AI.BackgroundImage = global::tcp_client_new.Properties.Resources.map;
            this.tab_AI.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tab_AI.Location = new System.Drawing.Point(4, 22);
            this.tab_AI.Name = "tab_AI";
            this.tab_AI.Padding = new System.Windows.Forms.Padding(3);
            this.tab_AI.Size = new System.Drawing.Size(515, 257);
            this.tab_AI.TabIndex = 1;
            this.tab_AI.Text = "AI";
            this.tab_AI.UseVisualStyleBackColor = true;
            this.tab_AI.Click += new System.EventHandler(this.tabPage2_Click);
            this.tab_AI.Paint += new System.Windows.Forms.PaintEventHandler(this.tab_AI_Paint);
            // 
            // tcp_client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GrayText;
            this.ClientSize = new System.Drawing.Size(526, 370);
            this.Controls.Add(this.mp_progressBar);
            this.Controls.Add(this.hp_progressBar);
            this.Controls.Add(this.sendmsg);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.tabControl1);
            this.Name = "tcp_client";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "tcp_client";
            this.Activated += new System.EventHandler(this.tcp_client_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.tcp_client_FormClosing);
            this.Load += new System.EventHandler(this.tcp_client_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox sendmsg;
        private System.Windows.Forms.TabControl tabControl1;
        //private tcp_client_new.TabControl_Double_Buffer tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        //private System.Windows.Forms.TabPage tab_AI;
        private tcp_client_new.TabAI_Double_Buffer tab_AI;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        //private System.Windows.Forms.ProgressBar hp_progressBar;
        private Jcobs.Controls.ProgressBars.ProgressBar hp_progressBar;
        private System.Windows.Forms.Timer hp_timer;
        private Jcobs.Controls.ProgressBars.ProgressBar mp_progressBar;
        private System.Windows.Forms.Timer mp_timer;
        private System.Windows.Forms.Timer position_test_timer;
    }
}

