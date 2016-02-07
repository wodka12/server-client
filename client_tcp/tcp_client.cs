using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

using System.Reflection;

namespace tcp_client_new
{
    [StructLayout(LayoutKind.Sequential)]
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    // or     
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack=1)]

    public struct _state
    {
        public UInt16 level;
        public int damage;
        public int shield;
        public int hp;
        public int mp;
        public int experience;
    }
    public struct _User
    {
        public UInt16 p_h;
        public UInt16 p_h_sec;
        public int id;
        public int x;
        public int y;
        public int zone;
        public _state state;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string msg;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        //public char[] msg;
    }

    public struct _User_other
    {
        public bool on_off;
        public int id;
        public int x;
        public int y;
        public int zone;
        public _state state;
    }

    public struct _monster_state
    {
        public UInt16 level;
        public int damage;
        public int shield;
        public int hp;
        public int mp;
        public int experience;
    }
    public struct _Monster
    {
        public bool on_off;
        public int id;
        public int x;
        public int y;
        public int zone;
        public _monster_state state;
    }

    enum e_Packet_header
    {
        P_SEND_MSG = 0x00,
        P_MOVE_POS = 0x01,
        P_USER_ACTIVITY = 0x02,
        P_BROAD_CAST_MSG = 0x04,
        P_SCAN_MONSTER_ON_SAME_ZONE = 0x08,
        P_SCAN_OTHER_PLAYER_ON_SAME_ZONE = 0x10,
        P_MAX = 0xFF,
    };

    enum e_Packet_move_pos
    {
        P_S_MOVE_ZONE = 0x00,
        P_S_MOVE_UP = 0x01,
        P_S_MOVE_DOWN = 0x02,
        P_S_MOVE_LEFT = 0x04,
        P_S_MOVE_RIGHT = 0x08,
        P_S_MOVE_RAND_TELEPORT = 0x10,
        P_S_MOVE_NONE = 0xFF,
    };

    enum e_Packet_user_activity_pos
    {
        P_S_ACTIVITY_ATTACK_TO_MONSTER = 0x00,
        P_S_ACTIVITY_SKILL = 0x01,
        P_S_ACTIVITY_ATTACK_TO_PLAYER = 0x02,
        P_S_ACTIVITY_NONE = 0xFF,
    };

    public partial class tcp_client : Form
    {
#if true
        Socket server;
        IPEndPoint ipep;
#else
        TcpClient server;
#endif
        bool connect_flag = false;
        NetworkStream ns;
        StreamReader streader;
        StreamWriter stwriter;
        Thread _thread_receive;
        delegate void D_receive(string data, int client_num); /* deligate */
        public const int client_max = 20000;

        _User __user = new _User();
        _Monster[] __monster_list = new _Monster[1024];

        _User_other[] __other_player_list = new _User_other[1024];

        Bitmap bm_hero;
        Bitmap bm_gong;

        public tcp_client()
        {
            byte[] data = new byte[1024];
            InitializeComponent();
            try
            {
                Assembly assem = this.GetType().Assembly;
                bm_hero = new Bitmap(tcp_client_new.Properties.Resources.hero);
                bm_gong = new Bitmap(tcp_client_new.Properties.Resources.gong);

                for (int i = 0; i < 1024; i++) {
                    __monster_list[i].on_off = false;
                    __other_player_list[i].on_off = false;
                }

                if (!connect_flag)
                {
                    this.SetStyle(ControlStyles.ResizeRedraw, true);
                    this.SetStyle(ControlStyles.UserPaint, true);
                    this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                    this.SetStyle(ControlStyles.DoubleBuffer, true);
                    this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
                    this.UpdateStyles();

#if true
                    ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4000);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    server.Connect(ipep);
                    ns = new NetworkStream(server);
#else

                    server = new TcpClient("127.0.0.1", 4000);
                    ns = server.GetStream();
#endif

                    streader = new StreamReader(ns);
                    stwriter = new StreamWriter(ns);
                    //server.ReceiveTimeout = 1000; // Time Out = 1s

                    string receive_data;
                    byte[] bByte = new byte[Marshal.SizeOf(__user)];

                    ns.Read(bByte, 0, Marshal.SizeOf(typeof(_User)));


                    GCHandle handle = GCHandle.Alloc(bByte, GCHandleType.Pinned);
                    __user = (_User)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(_User));
                    handle.Free();

                    receive_data = "[Joined]" + __user.id.ToString() + ": " + __user.zone.ToString() + " X:" + __user.x.ToString() + " Y:" + __user.y.ToString() + "[" + __user.msg + "]";

                    _thread_receive = new Thread(receive);
                    _thread_receive.Start();
                    connect_flag = true;
                    textBox1.Font = new Font("Tahoma", 8, FontStyle.Bold);
                    textBox1.Text += ("connected server" + Environment.NewLine);
                    textBox1.Text += (receive_data + Environment.NewLine);

                    richTextBox1.Text = "";
                    richTextBox1.Font = new Font("Tahoma", 8, FontStyle.Bold);
                    richTextBox1.Text += (receive_data + Environment.NewLine);

                    
                }
            }
            catch (SocketException)
            {
                connect_flag = false;
                textBox1.Text += ("Unable to connect to server" + Environment.NewLine);
                return;
            }
        }


        public static int byte2Int(byte[] src)
        {
            int s1 = src[0] & 0xFF;
            int s2 = src[1] & 0xFF;
            int s3 = src[2] & 0xFF;
            int s4 = src[3] & 0xFF;

            return ((s1 << 0) + (s2 << 8) + (s3 << 16) + (s4 << 24));
        }
        public static ushort byte2UShort(byte[] src)
        {
            int s1 = src[0] & 0xFF;
            int s2 = src[1] & 0xFF;

            return (ushort)((s1 << 0) + (s2 << 8));
        }

        void receive()
        {
            try
            {
                byte[] bByte = new byte[Marshal.SizeOf(__user)];
                byte[] bInt = new byte[4];
                byte[] bUshort = new byte[2];
                byte[] bmsg = new byte[4];

                while (connect_flag)
                {
                    string receive_data = "";
                    int arr_idx = 0;

                    if (0 < ns.Read(bByte, 0, Marshal.SizeOf(typeof(_User))))
                    {
                        ushort us_header = 0;
                        ushort us_s_header = 0;
                        int id;
                        string rcv_str;
                        int target_player_hp;

                        Array.Copy(bByte, arr_idx, bUshort, 0, 2);
                        arr_idx += 2;
                        us_header = byte2UShort(bUshort);

                        switch (us_header)
                        {
                            case (ushort)e_Packet_header.P_MOVE_POS:
                                Array.Copy(bByte, arr_idx, bUshort, 0, 2);
                                arr_idx += 2;
                                us_s_header = byte2UShort(bUshort);
                                Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                arr_idx += 4;
                                __user.x = byte2Int(bInt);
                                Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                arr_idx += 4;
                                __user.y = byte2Int(bInt);
                                Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                arr_idx += 4;
                                __user.zone = byte2Int(bInt);

                                if (us_s_header == (ushort)e_Packet_move_pos.P_S_MOVE_RAND_TELEPORT)
                                {
                                    for (int i = 0; i < 1024; i++) {
                                        __monster_list[i].on_off = false;
                                        __other_player_list[i].on_off = false;
                                    }
                                }

                                receive_data = "move position user!! " + "X: " + __user.x.ToString() + ", Y: " + __user.y.ToString();
                                break;
                            case (ushort)e_Packet_header.P_SEND_MSG:
                                Array.Copy(bByte, arr_idx, bmsg, 0, 4);
                                rcv_str = Encoding.Default.GetString(bmsg);   // byte -> string
                                receive_data = "sent message to other user !!";
                                break;
                            case (ushort)e_Packet_header.P_BROAD_CAST_MSG:
                                Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                arr_idx += 4;
                                id = byte2Int(bInt);
                                Array.Copy(bByte, arr_idx, bmsg, 0, 4);
                                rcv_str = Encoding.Default.GetString(bmsg);   // byte -> string
                                receive_data = "Notify[" + id.ToString() + "]" + ": " + rcv_str;
                                break;
                            case (ushort)e_Packet_header.P_SCAN_MONSTER_ON_SAME_ZONE:
                                Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                arr_idx += 4;
                                id = byte2Int(bInt);
                                __monster_list[id].id = byte2Int(bInt);
                                Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                arr_idx += 4;
                                __monster_list[id].zone = byte2Int(bInt);
                                Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                arr_idx += 4;
                                __monster_list[id].x = byte2Int(bInt);
                                Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                arr_idx += 4;
                                __monster_list[id].y = byte2Int(bInt);
                                Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                arr_idx += 4;
                                __monster_list[id].state.hp = byte2Int(bInt);

                                if (__monster_list[id].state.hp > 0)
                                {
                                    __monster_list[id].on_off = true;
                                }
                                else
                                {
                                    __monster_list[id].on_off = false;
                                    this.Invalidate();
                                }

                                receive_data = "Incoming monster[" + id.ToString() + "]" + " zone: " + __monster_list[id].zone.ToString() + " X: " + __monster_list[id].x.ToString() + ", Y: " + __monster_list[id].y.ToString() + " HP: " + __monster_list[id].state.hp.ToString();
                                break;
                            case (ushort)e_Packet_header.P_SCAN_OTHER_PLAYER_ON_SAME_ZONE:
                                Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                arr_idx += 4;
                                id = byte2Int(bInt);
                                __other_player_list[id].id = byte2Int(bInt);
                                Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                arr_idx += 4;
                                __other_player_list[id].zone = byte2Int(bInt);
                                Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                arr_idx += 4;
                                __other_player_list[id].x = byte2Int(bInt);
                                Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                arr_idx += 4;
                                __other_player_list[id].y = byte2Int(bInt);
                                Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                arr_idx += 4;
                                __other_player_list[id].state.hp = byte2Int(bInt);

                                if (__other_player_list[id].state.hp > 0)
                                {
                                    __other_player_list[id].on_off = true;
                                }
                                else
                                {
                                    __other_player_list[id].on_off = false;
                                    this.Invalidate();
                                }

                                receive_data = "find other player in same zone[" + id.ToString() + "]";

                                break;
                            case (ushort)e_Packet_header.P_USER_ACTIVITY:
                                Array.Copy(bByte, arr_idx, bUshort, 0, 2);
                                arr_idx += 2;
                                us_s_header = byte2UShort(bUshort);
                                if (us_s_header == (ushort)e_Packet_user_activity_pos.P_S_ACTIVITY_ATTACK_TO_MONSTER)
                                {
                                    Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                    arr_idx += 4;
                                    id = byte2Int(bInt);
                                    Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                    arr_idx += 4;
                                    __monster_list[id].state.hp = byte2Int(bInt);
                                    Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                    arr_idx += 4;
                                    __user.state.hp = byte2Int(bInt);
                                    Array.Copy(bByte, arr_idx, bUshort, 0, 2);
                                    arr_idx += 2;
                                    __user.state.level = byte2UShort(bUshort);
                                    Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                    arr_idx += 4;
                                    __user.state.experience = byte2Int(bInt);

                                    if (__monster_list[id].state.hp > 0)
                                    {
                                        __monster_list[id].on_off = true;
                                    }
                                    else
                                    {
                                        __monster_list[id].on_off = false;
                                        this.Invalidate();
                                    }

                                    /* displayer received data */
                                    receive_data = "Attacked! Monster ID [" + id.ToString() + "] \r\n";
                                    receive_data += "Monster HP [" + __monster_list[id].state.hp.ToString() + "] \r\n";
                                    receive_data += "Your HP    [" + __user.state.hp.ToString() + "]\r\n";
                                    receive_data += "Your LV    [" + __user.state.level.ToString() + "]\r\n";
                                    receive_data += "Your ExP   [" + __user.state.experience.ToString() + "]";
                                }
                                else if (us_s_header == (ushort)e_Packet_user_activity_pos.P_S_ACTIVITY_ATTACK_TO_PLAYER)
                                {
                                    Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                    arr_idx += 4;
                                    id = byte2Int(bInt);
                                    Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                    arr_idx += 4;
                                    target_player_hp = byte2Int(bInt);
                                    Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                    arr_idx += 4;
                                    __user.state.hp = byte2Int(bInt);
                                    Array.Copy(bByte, arr_idx, bUshort, 0, 2);
                                    arr_idx += 2;
                                    __user.state.level = byte2UShort(bUshort);
                                    Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                    arr_idx += 4;
                                    __user.state.experience = byte2Int(bInt);
                                    /* displayer received data */
                                    receive_data = "Attacked! Player ID [" + id.ToString() + "] \r\n";
                                    receive_data += "Target Player HP [" + target_player_hp.ToString() + "] \r\n";
                                    receive_data += "Your HP    [" + __user.state.hp.ToString() + "]\r\n";
                                    receive_data += "Your LV    [" + __user.state.level.ToString() + "]\r\n";
                                    receive_data += "Your ExP   [" + __user.state.experience.ToString() + "]";
                                }
                                else if (us_s_header == (ushort)e_Packet_user_activity_pos.P_S_ACTIVITY_SKILL)
                                {
                                    Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                    arr_idx += 4;
                                    __user.state.mp = byte2Int(bInt);
                                    Array.Copy(bByte, arr_idx, bUshort, 0, 2);
                                    arr_idx += 2;
                                    __user.state.level = byte2UShort(bUshort);
                                    Array.Copy(bByte, arr_idx, bInt, 0, 4);
                                    arr_idx += 4;
                                    __user.state.experience = byte2Int(bInt);
                                    /* displayer received data */
                                    receive_data = "Invoke Skill!";
                                }
                                break;
                            default:
                                receive_data = "";
                                break;
                        }

#if false
                    //string receive_data = streader.ReadLine();

                    string receive_data;

                    ns.Read(bByte, 0, Marshal.SizeOf(typeof(_User)));


                    GCHandle handle = GCHandle.Alloc(bByte, GCHandleType.Pinned);
                    __user = (_User)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(_User));
                    handle.Free();

                    switch ((e_Packet_header)__user.p_h)
                    {
                        case e_Packet_header.P_SEND_MSG:
                            break;
                        case e_Packet_header.P_MOVE_POS:
                            break;
                        case e_Packet_header.P_BROAD_CAST_MSG:
                            break;
                        case e_Packet_header.P_MAX:
                            break;

                    }
                    receive_data = "[M]" + __user.id.ToString() + ": " + __user.zone.ToString() + " X:" + __user.x.ToString() + " Y:" + __user.y.ToString() + "[" + __user.msg + "]";
#endif

                        if (receive_data != null)
                        {
                            tb_recevie_text(receive_data, 7777);
                            //stringData = Encoding.ASCII.GetString(data, 0, recv);
                        }
                    }
                }
            }
            catch (IOException)
            {
                if (connect_flag)
                {
                    _thread_receive = new Thread(receive);
                    _thread_receive.Start();
                }
                else
                {
                    connect_flag = false;
                }
            }
            catch (Exception ex)
            {
                connect_flag = false;
                //MessageBox.Show(ex.ToString());
                textBox1.AppendText(ex.ToString() + "\r\n"); /* or Environment.NewLine */
            }
        }

        void tb_recevie_text(string data, int cnum)
        {
            try
            {
                this.Invoke(new MethodInvoker(
                 delegate()
                 {
                     if (data != null)
                     {
                         textBox1.AppendText(data + "\r\n"); /* or Environment.NewLine */
                     }
                 }
                 ));
#if false
                if (this.InvokeRequired)
                {
                    D_receive d = new D_receive(tb_recevie_text);
                    //this.Invoke(new D_receive(tb_recevie_text), data);
                    this.Invoke(d, new object[] { data, cnum });
                }
                else
                {
                    if (data != null)
                    {
                        textBox1.AppendText(data + "\r\n"); /* or Environment.NewLine */
                    }
                }
#endif
            }
            catch (Exception ex)
            {
                textBox1.AppendText(ex.ToString() + "\r\n"); /* or Environment.NewLine */
            }
        }
        private void sendmsg_KeyUp(object sender, KeyEventArgs e)
        {
            Random random = new Random();

            if (e.KeyCode == Keys.Enter)
            {
                byte[] bByte = new byte[Marshal.SizeOf(__user)];
                try
                {
                    if (connect_flag)
                    {
                        if (sendmsg.Text != string.Empty)
                        {
                            byte[] test_byte = new byte[30];
                            MemoryStream m = new MemoryStream(test_byte);
                            BinaryWriter bw = new BinaryWriter(m);
                            switch (sendmsg.Text)
                            {
                                case "up":
                                    __user.p_h = (ushort)e_Packet_header.P_MOVE_POS;
                                    __user.p_h_sec = (ushort)e_Packet_move_pos.P_S_MOVE_UP;
                                    break;
                                case "down":
                                    __user.p_h = (ushort)e_Packet_header.P_MOVE_POS;
                                    __user.p_h_sec = (ushort)e_Packet_move_pos.P_S_MOVE_DOWN;
                                    break;
                                case "left":
                                    __user.p_h = (ushort)e_Packet_header.P_MOVE_POS;
                                    __user.p_h_sec = (ushort)e_Packet_move_pos.P_S_MOVE_LEFT;
                                    break;
                                case "right":
                                    __user.p_h = (ushort)e_Packet_header.P_MOVE_POS;
                                    __user.p_h_sec = (ushort)e_Packet_move_pos.P_S_MOVE_RIGHT;
                                    break;
                                case "rand":
                                    __user.p_h = (ushort)e_Packet_header.P_MOVE_POS;
                                    __user.p_h_sec = (ushort)e_Packet_move_pos.P_S_MOVE_RAND_TELEPORT;
                                    break;
                                case "rmsg":
                                    __user.p_h = (ushort)e_Packet_header.P_SEND_MSG;
                                    break;
                                default:
                                    __user.p_h = (ushort)e_Packet_header.P_MOVE_POS;
                                    __user.p_h_sec = (ushort)e_Packet_move_pos.P_S_MOVE_NONE;
                                    break;
                            }

                            /* Attack Command to monster */
                            if (sendmsg.Text.Contains("attack") == true)
                            {
                                __user.msg = sendmsg.Text.ToString();

                                string str_monster_idx;
                                int int_monster_idx;
                                str_monster_idx = sendmsg.Text.Substring(7);

                                __user.p_h = (ushort)e_Packet_header.P_USER_ACTIVITY;
                                __user.p_h_sec = (ushort)e_Packet_user_activity_pos.P_S_ACTIVITY_ATTACK_TO_MONSTER;
                                bw.Write(__user.p_h);
                                bw.Write(__user.p_h_sec);
                                bool result = Int32.TryParse(str_monster_idx, out int_monster_idx);
                                if (result == false)
                                {
                                    sendmsg.Text = "";
                                    return;
                                }
                                bw.Write(int_monster_idx);
                                bw.Close();
                                ns.Write(test_byte
                                   , 0
                                   , 30);
                                ns.Flush();
                            }

                            /* Attack Command to other user */
                            if (sendmsg.Text.Contains("atp") == true)
                            {
                                __user.msg = sendmsg.Text.ToString();

                                string str_other_player_idx;
                                int int_other_player_idx;
                                str_other_player_idx = sendmsg.Text.Substring(4);

                                __user.p_h = (ushort)e_Packet_header.P_USER_ACTIVITY;
                                __user.p_h_sec = (ushort)e_Packet_user_activity_pos.P_S_ACTIVITY_ATTACK_TO_PLAYER;
                                bw.Write(__user.p_h);
                                bw.Write(__user.p_h_sec);
                                bool result = Int32.TryParse(str_other_player_idx, out int_other_player_idx);
                                if (result == false)
                                {
                                    sendmsg.Text = "";
                                    return;
                                }
                                bw.Write(int_other_player_idx);
                                bw.Close();
                                ns.Write(test_byte
                                   , 0
                                   , 30);
                                ns.Flush();
                            }

                            if (sendmsg.Text.Contains("sk") == true)
                            {
                                __user.msg = sendmsg.Text.ToString();

                                __user.p_h = (ushort)e_Packet_header.P_USER_ACTIVITY;
                                __user.p_h_sec = (ushort)e_Packet_user_activity_pos.P_S_ACTIVITY_SKILL;
                                bw.Write(__user.p_h);
                                bw.Write(__user.p_h_sec);
                                bw.Write(__user.state.level);
                                bw.Write(__user.state.hp);
                                bw.Close();
                                ns.Write(test_byte
                                   , 0
                                   , 30);
                                ns.Flush();
                            }
                            if (__user.p_h == (ushort)e_Packet_header.P_MOVE_POS)
                            {
                                __user.msg = sendmsg.Text.ToString();

                                bw.Write(__user.p_h);
                                bw.Write(__user.p_h_sec);
                                bw.Write(__user.x);
                                bw.Write(__user.y);
                                bw.Write(__user.zone);
                                bw.Close();

                                ns.Write(test_byte
                                   , 0
                                   , 30);
                                ns.Flush();
                            }
                            else if (__user.p_h == (ushort)e_Packet_header.P_SEND_MSG)
                            {
                                __user.msg = sendmsg.Text.ToString();

                                bw.Write(__user.p_h);
                                bw.Write(__user.msg);
                                bw.Close();
                                ns.Write(test_byte
   , 0
   , 30);
                                ns.Flush();
                            }
                            else
                            {

                            }
                            sendmsg.Text = "";

#if false
                            IntPtr Ptr = Marshal.AllocHGlobal(Marshal.SizeOf(__user));
                            Marshal.StructureToPtr(__user, Ptr, false);
                            Marshal.Copy(Ptr, bByte, 0, Marshal.SizeOf(__user));
                            ns.Write(bByte
                                , 0
                                , Marshal.SizeOf(typeof(_User)));
                            ns.Flush();
                            sendmsg.Text = "";
#endif
                        }
                    }
                }
                catch (Exception ex)
                {
                    connect_flag = false;
                    MessageBox.Show(ex.ToString());

                }
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }
        private void tcp_client_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (connect_flag)
            {
                ns.Close();
                server.Close();
                connect_flag = false;
                //_thread_receive.Abort();  // exit now
                bool ret = _thread_receive.Join(1000); // waiting and exit
                if (ret == false) // thread closing failed..
                {
                    _thread_receive.Abort(); // exit now
                }
                for (int i = 0; i < c_num; i++)
                {
                    server_mult[i].Close();
                    ns_mult[i].Close();
                    _thread_multi[i].Join(1000); // waiting and exit
                }
            }
        }

        private void tcp_client_Load(object sender, EventArgs e)
        {
            sendmsg.Focus();
        }

        private void sendmsg_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }


        Socket[] server_mult = new Socket[client_max];
        NetworkStream[] ns_mult = new NetworkStream[client_max];
        StreamReader[] streader_mult = new StreamReader[client_max];
        StreamWriter[] stwriter_mult = new StreamWriter[client_max];
        Thread[] _thread_multi = new Thread[client_max];
        _User[] __user_mult = new _User[client_max];
        int c_num = 0;

        void receive_mult(object cnum)
        {
            int inum = (int)cnum;
            try
            {
#if false
                byte[] bByte = new byte[Marshal.SizeOf(__user)];
                string receive_data;
#endif

                while (connect_flag)
                {
#if false
                    ns_mult[(int)cnum].Read(bByte, 0, Marshal.SizeOf(typeof(_User)));

                    GCHandle handle = GCHandle.Alloc(bByte, GCHandleType.Pinned);
                    __user_mult[(int)cnum] = (_User)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(_User));
                    handle.Free();

                    receive_data = "[G]" + __user_mult[(int)cnum].id.ToString() + ": " + __user_mult[(int)cnum].zone.ToString() + " X:" + __user_mult[(int)cnum].x.ToString() + " Y:" + __user_mult[(int)cnum].y.ToString() + "  [" + __user_mult[(int)cnum].msg + "]";
                    //DrawEllipse(__user_mult[(int)cnum].x, __user_mult[(int)cnum].y);

                    if (receive_data != null)
                    {
                        tb_recevie_text(receive_data, (int)cnum);
                    }
#endif
                }

            }
            catch (IOException)
            {
                if (connect_flag)
                {
                    _thread_multi[(int)cnum] = new Thread(new ParameterizedThreadStart(receive_mult));
                    _thread_multi[(int)cnum].Start(cnum);
                }
                else
                {
                    connect_flag = false;
                }
            }
            catch (Exception ex)
            {
                connect_flag = false;
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                server_mult[c_num] = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server_mult[c_num].Connect(ipep);
                ns_mult[c_num] = new NetworkStream(server_mult[c_num]);

                streader_mult[c_num] = new StreamReader(ns_mult[c_num]);
                stwriter_mult[c_num] = new StreamWriter(ns_mult[c_num]);
                _thread_multi[c_num] = new Thread(new ParameterizedThreadStart(receive_mult));
                _thread_multi[c_num].Start(c_num);
                c_num++;
            }
            catch (Exception ex)
            {
                connect_flag = false;
                MessageBox.Show(ex.ToString());

            }
        }
        private void DrawRectangle(int x, int y)
        {
            System.Drawing.Pen myPen;
            myPen = new System.Drawing.Pen(System.Drawing.Color.MediumBlue);
            System.Drawing.Graphics formGraphics = this.tab_AI.CreateGraphics();
            formGraphics.DrawRectangle(myPen, new Rectangle(x, y, 5, 5));
            SolidBrush _brush = new SolidBrush(Color.MediumBlue);
            formGraphics.FillRectangle(_brush, x, y, 5, 5);
            myPen.Dispose();
            formGraphics.Dispose();
        }

        private void DrawEllipse(int x, int y)
        {
            System.Drawing.Pen myPen;
            myPen = new System.Drawing.Pen(System.Drawing.Color.Red);
            System.Drawing.Graphics formGraphics = this.tab_AI.CreateGraphics();
            formGraphics.DrawEllipse(myPen, new Rectangle(x, y, 10, 10));

            SolidBrush _brush = new SolidBrush(Color.Tomato);
            formGraphics.FillEllipse(_brush, x, y, 10, 10);
            myPen.Dispose();
            formGraphics.Dispose();
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string display_data;
            display_data = __user.id.ToString() + ": " + "Lv." + __user.state.level + " X:" + __user.x.ToString() + " Y:" + __user.y.ToString() + "  Last Order: [" + __user.msg + "]\r\n";
            display_data += "HP: " + __user.state.hp + " Zone: " + __user.zone + " Exp: " + __user.state.experience;

            richTextBox1.Text = "";
            richTextBox1.Text += (display_data + Environment.NewLine);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            hp_progressBar.Value = __user.state.hp;
            if (hp_progressBar.Value >= 100)
            {
                hp_progressBar.Value = 100;
            }
            return;
        }

        private void mp_timer_Tick(object sender, EventArgs e)
        {
            mp_progressBar.Value = __user.state.mp;
            if (mp_progressBar.Value >= 100)
            {
                mp_progressBar.Value = 100;
            }
            return;
        }

        static int s_before_x = 0;
        static int s_before_y = 0;
        static int[] s_before_x_monster = new int[1024];
        static int[] s_before_y_monster = new int[1024];
        static int[] s_before_x_other_player = new int[1024];
        static int[] s_before_y_other_player = new int[1024];
        private void position_test_timer_Tick(object sender, EventArgs e)
        {
            if (s_before_x != __user.x || s_before_y != __user.y)
            {
                s_before_x = __user.x;
                s_before_y = __user.y;

                this.Refresh();
                return;
            }

            for (int i = 0; i < 1024; i++)
            {
                if (__monster_list[i].on_off == true)
                {
                    if (__monster_list[i].x != s_before_x_monster[i] ||
                        __monster_list[i].y != s_before_y_monster[i])
                    {
                        //DrawRectangle((__monster_list[i].x * 2), __monster_list[i].y);
                        s_before_x_monster[i] = __monster_list[i].x;
                        s_before_y_monster[i] = __monster_list[i].y;
                        this.Refresh();
                        return;
                    }
                }

                if (__other_player_list[i].on_off == true)
                {
                    if (__other_player_list[i].x != s_before_x_other_player[i] ||
                        __other_player_list[i].y != s_before_y_other_player[i])
                    {
                        s_before_x_other_player[i] = __other_player_list[i].x;
                        s_before_y_other_player[i] = __other_player_list[i].y;
                        this.Refresh();
                        return;
                    }
                }
            }
#if false
            Image imgtest = Image.FromFile("object_i.jpg");
            System.Drawing.Graphics formGraphics = this.tab_AI.CreateGraphics();
            formGraphics.DrawImage(imgtest, __monster_list[0].x, __monster_list[0].y, 5, 5);
#endif

#if false
            Image image = new Image(object_i.jpg);
            bmp = new Bitmap(tcp_client_new.Properties.Resources.object_i1.Width, tcp_client_new.Properties.Resources.object_i1.Height);
            bmpgraphics = Graphics.FromImage(bmp);

            x++;
            y++;
            Point point = new Point((x * 2), y);
            bmpgraphics.TranslateTransform(x, y);

            bmpgraphics.DrawImage(bmp,point);
            //DrawAxis();
#endif
        }

        private void tab_AI_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawImage(bm_hero, (__user.x * 2), __user.y, 10, 10);

            for (int i = 0; i < 1024; i++)
            {
                if (__monster_list[i].on_off == true)
                {
                    //DrawRectangle((__monster_list[i].x * 2), __monster_list[i].y);
                    g.DrawImage(bm_gong, (__monster_list[i].x * 2), __monster_list[i].y, 10, 10);
                }

                if (__other_player_list[i].on_off == true)
                {
                    g.DrawImage(bm_hero, (__other_player_list[i].x * 2), __other_player_list[i].y, 10, 10);
                }
            }
        }
    }
}
