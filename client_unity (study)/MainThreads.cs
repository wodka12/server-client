/******************/
/* Made by Lee.sy */
/******************/

using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using System.IO;
using System.Threading;

using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

public class MainThreads : MonoBehaviour
	
{
	
	#region Public data
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
	
	_User __user = new _User();
	_Monster[] __monster_list = new _Monster[1024];
	_User_other[] __other_player_list = new _User_other[1024];
	public float timeWaiting = 5000000.0f;
	public string labelInitialText = "I`m the console here!";
	Socket server;
	IPEndPoint ipep;
	NetworkStream ns;
	bool connect_flag = false;
	byte[] test_byte = new byte[30];
	
	#endregion
	
	
	
	#region Private data
	
	private string _label;
	private Thread _t1;
	private Thread _t2;
	private bool _t1Paused = false;
	private bool _t2Paused = false;
	
	#endregion
	
	
	
	#region Start

	void Start () {
		
		_label = labelInitialText;
		
		_t1 = new Thread(_func1);
		
		_t2 = new Thread(_func2);

#if true
		string ipAddress = "127.0.0.1"; //"localhost";
		
		int gamePort = 14248;
		
		int policyPort = 4000;
		
		bool isPolicyConnected = false;
		
		string hostname = "127.0.0.1";
		
		IPAddress[] ips = Dns.GetHostAddresses(ipAddress);

		StreamWriter theWriter;
		StreamReader theReader;
		
		foreach (IPAddress ip in ips)
			
		{
			
			ipAddress = ip.ToString();
			
			if (!(isPolicyConnected=Security.PrefetchSocketPolicy(ipAddress, policyPort, 4000))) 
				
				print("policy socket address failed to connect to "+ipAddress+":"+policyPort);
			
			else break;
			
		}

		if(isPolicyConnected)
			
			Application.LoadLevel("mainscene");
		
		else ipAddress=hostname;

		TcpClient mySocket;
		mySocket = new TcpClient(hostname, policyPort);
		ns = mySocket.GetStream();
		theWriter = new StreamWriter(ns);
		theReader = new StreamReader(ns);
		
#else
		ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4000);
		server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		server.Connect(ipep);
		ns = new NetworkStream(server);
#endif
		
		byte[] bByte = new byte[Marshal.SizeOf(__user)];
		
		ns.Read(bByte, 0, Marshal.SizeOf(typeof(_User)));
		GCHandle handle = GCHandle.Alloc(bByte, GCHandleType.Pinned);
		__user = (_User)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(_User));
		handle.Free();
		
	}
	
	#endregion

	public GameObject ThePrefeb;
	Vector3 vec_mon = new Vector3 (20, 10, 10);
	Vector3 vec_pc = new Vector3 (20, 10, 10);
	
	GameObject[] instance = new GameObject[1024];
	Quaternion qua = new Quaternion ();
	void Update () {
		var speed = 5;
		var power = 500;

		vec_pc.x = __user.x - 220;
		vec_pc.z = __user.y - 220;
		vec_pc.y = -20;
		//transform.Translate(vec_pc);
		transform.position = vec_pc;

		for (int id = 0; id < 1024; id++)
		{
			if (__monster_list[id].on_off == true) 
			{
				vec_mon.x = __monster_list[id].x;
				vec_mon.y = 10;
				vec_mon.z = __monster_list[id].y;
				if (instance[id] == null)
				{
					instance[id] = (GameObject)Instantiate (ThePrefeb, vec_mon, qua);
					instance[id].transform.position= vec_mon;
				} else {

					vec_mon.x = (float)0.1;
					vec_mon.y = 0;
					vec_mon.z = (float)0.1;

					var amtMove = speed * Time.deltaTime;
					var ver = Input.GetAxis("Vertical");
					var hor = Input.GetAxis("Horizontal");

					//instance[id].transform.Translate(Vector3.forward * ver * amtMove);
					instance[id].transform.Translate(vec_mon);
				}
			}
		}
	}

	#region Threads
	
	private void _func1()
	{
		
		if(_label == labelInitialText)
			_label = "";

		for (int i = 0; i < 1024; i++) {
			Destroy(instance[i]);
		}

		while(true)
		{
			_label += 1;
			
			for(int i = 0; i < timeWaiting; i ++)
				
			while(_t1Paused){}
			
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
	
	private void _func2()
	{
		
		if(_label == labelInitialText)
			_label = "";
		
		while(connect_flag == true)
		{
			if (0 < ns.Read (test_byte, 0, 30) ) 
			{
				ushort us_header = 0;
				ushort us_s_header = 0;
				int id;
				string rcv_str;
				int target_player_hp;
				int arr_idx = 0;
				byte[] bInt = new byte[4];
				byte[] bUshort = new byte[2];
				byte[] bmsg = new byte[4];
				
				Array.Copy(test_byte, arr_idx, bUshort, 0, 2);
				arr_idx += 2;
				us_header = byte2UShort(bUshort);
				if (us_header == (ushort)e_Packet_header.P_SCAN_MONSTER_ON_SAME_ZONE)
				{
					Array.Copy(test_byte, arr_idx, bInt, 0, 4);
					arr_idx += 4;
					id = byte2Int(bInt);
					__monster_list[id].id = byte2Int(bInt);
					Array.Copy(test_byte, arr_idx, bInt, 0, 4);
					arr_idx += 4;
					__monster_list[id].zone = byte2Int(bInt);
					Array.Copy(test_byte, arr_idx, bInt, 0, 4);
					arr_idx += 4;
					__monster_list[id].x = byte2Int(bInt);
					Array.Copy(test_byte, arr_idx, bInt, 0, 4);
					arr_idx += 4;
					__monster_list[id].y = byte2Int(bInt);
					Array.Copy(test_byte, arr_idx, bInt, 0, 4);
					arr_idx += 4;
					__monster_list[id].state.hp = byte2Int(bInt);

					
					if (__monster_list[id].state.hp > 0) {
						__monster_list[id].on_off = true;
					} else {
						__monster_list[id].on_off = false;
					}
				}
			}		
			
		}
		
	}
	
	#endregion
	
	
	
	#region OnGUI
	
	void OnGUI()
		
	{
		
		//--> Label that servers as a "console"
		GUI.Label(new Rect(0,0, 500, 500), _label);

		
		//--> Button - Start thread 1
		
		if(GUI.Button(new Rect(50, 50, 100, 50), "Text TEST"))
		{	
			if(!_t1.IsAlive)	
				_t1.Start()
			else
				_t1Paused = !_t1Paused;
		}
		//--> Button - Start thread 2
		
		if(GUI.Button(new Rect(50, 120, 100, 50), "Async to server"))
		{
			if(!_t2.IsAlive) {
				connect_flag = true;
				_t2.Start();
			}
			else {
				_t2Paused = !_t2Paused;
			}
		}
	}
	
	#endregion
	
}