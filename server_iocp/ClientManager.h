#pragma once
#include "network.h"
#include "Object.h"

#define MAX_CLIENT 20000

typedef struct ClientInfo {
	SOCKETINFO socket_info;
} s_ClientInfo;

class ClientManager
{
#if 1
	CRITICAL_SECTION cs;
#endif

public:
	unordered_map<int, ObjectUser*> map_user;
	vector<ObjectUser*> vec;
	vector<ObjectUser*>::iterator vec_it;
	ObjectUser* UserInfo[MAX_CLIENT];

	s_ClientInfo ClientInfo[MAX_CLIENT];
	int idx;

#if 1
	CStreamSP* pStreamSP = new CStreamSP;
#endif

	SOCKETINFO *pc_socket_info;

	bool b_client_ping = true;
	int id_client_closed;
private:
	int client_cnt = 0;

public:
	ClientManager(){
		InitializeCriticalSection(&cs);
	}
	~ClientManager(){
		DeleteCriticalSection(&cs);
	}

	void SubtractClientCnt(void);
	
	void send_client_closed(int fd);
	void check_ping_and_cut_user(void);

	void AddClientCnt(void);
	int GetClientCnt(void) {
		return client_cnt;
	}

	void InitClientManager(void);
	SOCKETINFO* GetEmptySocketinfo(void);
	
	void PushUser(int fd);
	void SubstractUser(int fd);
	ObjectUser* FindUser(int fd);
	void UpdateUserInfo(int fd, ObjectUser::User* info);
	void NotifyUserInfo();

	void broadcast_userinfo(int own, ObjectUser::User* info);

	void broadcast_userinfo_zone(int own, ObjectUser::User* info);

	void send_msg(int client_idx);
#ifdef _NET_STREAM_TEST_
	USHORT Read_Stream_Header(CStream* clsStream, BYTE* src);
	USHORT Read_Stream_Header_second(CStream* clsStream);
#endif
	int Recv_Client_Packet(SOCKETINFO* socket_info, BYTE* packet);

	/* Move Process */
	int packet_move_proc(ObjectUser::User*);

	/* Send Msg Process */
	int packet_send_msg_proc(ObjectUser::User*);

	/* User Activity Process */
	int packet_user_activity_proc(ObjectUser::User*);

	void scan_monster_on_same_zone();
	void notify_from_monster(char* msg, size_t size);

	void scan_other_player_on_same_zone();
	void notify_player_to_player(BYTE* msg, size_t size);

	void scan_other_player_on_same_zone_and_notify(ObjectUser* pUser);

	void broadcast_other_player(ObjectUser* pUser);
};


#if 0
class PacketStream {
	int x, y;
public:
	PacketStream() {}
	PacketStream(int i, int j) {
		x = i;
		y = j;
	}
#if 0
	// value to buffer
	PacketStream& operator << (int  value) {
	}
	// buffer to value
	PacketStream& operator >> (int  value) {

	}
#endif

#if 1
	friend ostream &operator<<(ostream &stream, PacketStream ob){
		stream << ob.x << ' ' << ob.y << '\n';

		return stream;
	}
	friend istream &operator>>(istream &stream, PacketStream &ob){
		stream >> ob.x >> ob.y;

		return stream;
	}
#endif
};
#endif