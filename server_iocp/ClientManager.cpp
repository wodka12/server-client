#include "stdafx.h"
#include "ClientManager.h"
#include "ObjManager.h"

extern ObjManager* pObjManager;

void ClientManager::InitClientManager()
{
#if 1
	memset(ClientInfo, 0, sizeof(ClientInfo));
	for (DWORD i = 0; i < MAX_CLIENT; i++) {
		ClientInfo[i].socket_info.fd = INVALID_SOCKET;
	}
#else
	memset(ClientInfo, 0, sizeof(ClientInfo));
	for (DWORD i = 0; i < MAX_CLIENT; i++) {
		ClientInfo[i].socket_info.fd = INVALID_SOCKET;
		vec.push_back(ClientInfo[i]);
	}
#endif
}

void ClientManager::AddClientCnt()
{
	if (client_cnt < MAX_CLIENT){
		client_cnt++;
	}
}

void ClientManager::SubtractClientCnt()
{
	if (client_cnt > 0){
		client_cnt--;
	}
}

SOCKETINFO* ClientManager::GetEmptySocketinfo(void)
{
	for (int i = 0; i < MAX_CLIENT; i++) {
		if (ClientInfo[i].socket_info.fd == INVALID_SOCKET) {
			printf("Client Number[%d]\n", i);
			return &ClientInfo[i].socket_info;
		}
	}
	return FALSE;
}

HANDLE        m_hMutex;
#define SHARE_MUTEX "Share_Mutex"


#pragma warning(disable : 4996)
void ClientManager::PushUser(int fd)
{
	int ClientCnt = GetClientCnt();
	UserInfo[ClientCnt] = new ObjectUser;
	ObjectUser* user = UserInfo[ClientCnt];
#ifdef _AI_TEST_
	user->Set_Id(fd);
	srand((unsigned int)timeGetTime());
	user->SetPos_X(rand() % AI_MAX_POS_X);
	user->SetPos_Y(rand() % AI_MAX_POS_Y);
	user->Set_Msg(" !  ");
	user->Set_Zone(user->myWolrd.calc_zone(user->sUser_info.pos_x, user->sUser_info.pos_y));
	/* set state */
	user->sUser_info.state.hp = MAX_HP_USER;
	user->sUser_info.state.mp = MAX_MP_USER;
	user->sUser_info.state.damage = DEF_USER_DMG;
	user->sUser_info.state.shield = DEF_USER_SHD;
	user->sUser_info.state.level = DEF_USER_LEVEL;
	user->sUser_info.state.experience = 0;

	user->is_connected = TRUE;
	vec.push_back(user);

	unordered_map<int, ObjectUser*>::value_type value(fd, user);
	map_user.insert(value);
	map_user[fd] = user;
#if 0
	hash_map<int, User>::iterator it_user;
	map_user.find(fd);
	if (it_user != map_user.end())
	{
		std::cout << std::endl;
		std::cout << it_user->first << " " << it_user->second.zone << " " << std::endl;
	}
#endif
#endif
	AddClientCnt();
}

void ClientManager::SubstractUser(int fd)
{
#if 0
	/* Mutex Open */
	m_hMutex = OpenMutex(MUTEX_ALL_ACCESS, TRUE, (LPCWSTR)SHARE_MUTEX);
	if (m_hMutex == NULL)
	{
		printf("Mutex Handle is NULL.. \n");
		m_hMutex = CreateMutex(NULL, FALSE, (LPCWSTR)SHARE_MUTEX);
	}
	WaitForSingleObject(m_hMutex, INFINITE);
#endif
	/* Mutex Open */
	unordered_map<int, ObjectUser*>::iterator it_user;

	it_user = map_user.find(fd);
	if (it_user != map_user.end()) {
		map_user.erase(it_user);
		delete UserInfo[fd];
		SubtractClientCnt();
	};

#if 0
	/* Mutex Close */
	ReleaseMutex(m_hMutex);
	//CloseHandle(m_hMutex);

	m_hMutex = NULL;
	/* Mutex Close */
#endif
}

void ClientManager::send_client_closed(int fd)
{
	/* Mutex Open */
	ObjectUser* pUser;
	unordered_map<int, ObjectUser*>::iterator it_user;

	it_user = map_user.find(fd);
	if (it_user != map_user.end())
	{
		pUser = it_user->second;
		pUser->is_connected = FALSE;
		b_client_ping = false;
	}
}

void ClientManager::check_ping_and_cut_user(void)
{
	vector<int> vec;
	vector<int>::iterator vec_it;
	int close_client_id;

	ObjectUser* pUser;
	unordered_map<int, ObjectUser*>::iterator it_user = map_user.begin();
	while (it_user != map_user.end())
	{
		pUser = it_user->second;
		if (pUser->is_connected == FALSE)
		{
			vec.push_back(pUser->sUser_info.id);
		}
		it_user++;
	}

	for (vec_it = vec.begin(); vec_it != vec.end(); vec_it++) {
		close_client_id = *vec_it;
		SubstractUser(close_client_id);
	}
}

ObjectUser* ClientManager::FindUser(int fd)
{
	ObjectUser* pObjUser = NULL;
	unordered_map<int, ObjectUser*>::iterator it_user;

	if (fd > 0) {
		it_user = map_user.find(fd);
		pObjUser = it_user->second;
	}
	return pObjUser;
}

void ClientManager::UpdateUserInfo(int fd, ObjectUser::User* info)
{
	unordered_map<int, ObjectUser*>::iterator it_user;
	ObjectUser* pUser;

	it_user = map_user.find(fd);

	pUser = it_user->second;
	pUser->Set_Id(fd);
	pUser->SetPos_X(info->pos_x);
	pUser->SetPos_Y(info->pos_y);
	pUser->Set_Msg(info->msg);
	pUser->Set_Zone(pUser->myWolrd.calc_zone(info->pos_x, info->pos_y));

	info = &pUser->sUser_info;
}

void ClientManager::NotifyUserInfo()
{
	unordered_map<int, ObjectUser*>::iterator it_user = map_user.begin();
	while (it_user != map_user.end())
	{
		//updateData(*idx);
		//(*idx)->notify();
		it_user++;
	}
}

void ClientManager::scan_monster_on_same_zone()
{
	EnterCriticalSection(&cs);

	ObjectUser* p_User;
	unordered_map<int, ObjectUser*>::iterator it_user = map_user.begin();
	while (it_user != map_user.end())
	{
		p_User = it_user->second;
		if (p_User != NULL)
		{
			pObjManager->scan_user_on_same_zone(p_User);
		}
		it_user++;
	}
	LeaveCriticalSection(&cs);
}

void ClientManager::scan_other_player_on_same_zone()
{
	EnterCriticalSection(&cs);

	ObjectUser* p_User;
	unordered_map<int, ObjectUser*>::iterator it_user = map_user.begin();
	while (it_user != map_user.end())
	{
		p_User = it_user->second;
		if (p_User != NULL)
		{
			scan_other_player_on_same_zone_and_notify(p_User);
		}
		it_user++;
	}
	LeaveCriticalSection(&cs);
}

void ClientManager::scan_other_player_on_same_zone_and_notify(ObjectUser* pUser)
{
	CStream* pStream = *pStreamSP;
	BYTE send_packet[22];
	USHORT p_Head = P_SCAN_OTHER_PLAYER_ON_SAME_ZONE;

	ObjectUser* p_User;
	unordered_map<int, ObjectUser*>::iterator it_user = map_user.begin();
	while (it_user != map_user.end())
	{
		p_User = it_user->second;
		if ( p_User != NULL && 
			p_User->sUser_info.id != pUser->sUser_info.id) /* not for self */
		{
			if (p_User->sUser_info.zone == pUser->sUser_info.zone) {
				/****************/
				/* Open Stream  */
				/****************/
				memset(send_packet, 0, sizeof(send_packet));
				pStream->StartWrite(send_packet);
				pStream->WriteData(&p_Head);
				pStream->WriteData(&p_User->sUser_info.id);
				pStream->WriteData(&p_User->sUser_info.zone);
				pStream->WriteData(&p_User->sUser_info.pos_x);
				pStream->WriteData(&p_User->sUser_info.pos_y);
				pStream->WriteData(&p_User->sUser_info.state.hp);

				pUser->notify_from_other_player(send_packet, sizeof(send_packet));

				/****************/
				/* Close Stream */
				/****************/
				pStream->EndWrite();
			}
		}
		it_user++;
	}
}

void ClientManager::broadcast_userinfo(int own, ObjectUser::User* info)
{
	BYTE packet_broadcasting[10];
	CStream* pStream = *pStreamSP;
	USHORT p_Head = P_BROAD_CAST_MSG;

	try{
		memset(packet_broadcasting, 0, sizeof(packet_broadcasting));
		/****************/
		/* Open Stream  */
		/****************/
		pStream->StartWrite(packet_broadcasting);

		pStream->WriteData(&p_Head);
		pStream->WriteData(&own);
		pStream->WriteString(info->msg);

		unordered_map<int, ObjectUser*>::iterator it_user = map_user.begin();
		ObjectUser* p_User;

		while (it_user != map_user.end())
		{
			p_User = it_user->second;
			if ((p_User != NULL) && (p_User->sUser_info.id != own))
			{
				p_User->notify(packet_broadcasting, sizeof(packet_broadcasting));
			}
			it_user++;
		}

		/****************/
		/* Close Stream */
		/****************/
		pStream->EndWrite();
	}
	catch (DWORD dwError)
	{
		printf("Error code : %d", dwError);
	}
}

void ClientManager::broadcast_other_player(ObjectUser* p_user_self)
{
	CStream* pStream = *pStreamSP;
	BYTE send_packet[22];
	USHORT p_Head = P_SCAN_OTHER_PLAYER_ON_SAME_ZONE;

	ObjectUser* p_User;
	unordered_map<int, ObjectUser*>::iterator it_user = map_user.begin();
	while (it_user != map_user.end())
	{
		p_User = it_user->second;
		if (p_User != NULL &&
			p_User->sUser_info.id != p_user_self->sUser_info.id) /* not for self */
		{
			if (p_User->sUser_info.zone == p_user_self->sUser_info.zone) {
				/****************/
				/* Open Stream  */
				/****************/
				memset(send_packet, 0, sizeof(send_packet));
				pStream->StartWrite(send_packet);
				pStream->WriteData(&p_Head);
				pStream->WriteData(&p_user_self->sUser_info.id);
				pStream->WriteData(&p_user_self->sUser_info.zone);
				pStream->WriteData(&p_user_self->sUser_info.pos_x);
				pStream->WriteData(&p_user_self->sUser_info.pos_y);
				pStream->WriteData(&p_user_self->sUser_info.state.hp);

				//p_User->notify_from_other_player(send_packet, sizeof(send_packet));
				send(p_User->sUser_info.id, (char*)send_packet, sizeof(send_packet), 0);

				/****************/
				/* Close Stream */
				/****************/
				pStream->EndWrite();
			}
		}
		it_user++;
	}
}

void ClientManager::broadcast_userinfo_zone(int own, ObjectUser::User* info)
{
	unordered_map<int, ObjectUser*>::iterator it_user = map_user.begin();
	ObjectUser* p_User;

	while (it_user != map_user.end())
	{
		p_User = it_user->second;
		if (p_User->sUser_info.id != own &&
			p_User->sUser_info.zone == info->zone) {
			p_User->notify(info->msg);
		}
		it_user++;
	}
}

#define AI_BUFF_MAX 40
//#pragma warning(disable : 4996)
void ClientManager::send_msg(int Client_idx)
{
#if 0
	int ret;
#endif
	char buff[AI_BUFF_MAX] = "";
	int fd;
	ObjectUser::User* pUser;

	fd = ClientInfo[Client_idx].socket_info.fd;

	if (fd > 0)
	{
		pUser = &FindUser(fd)->sUser_info;
		if (pUser != NULL)
		{
			broadcast_userinfo(fd, pUser);
		}
	}
#if 0
	if (fd != INVALID_SOCKET) {
		ret = send(fd, 
			(char*)&FindUser(fd)->sUser_info, 
			sizeof(UserInfo[Client_idx]->sUser_info),
			0);

		if (ret == SOCKET_ERROR)
		{
			printf("WSA Error.. [%d]\n", WSAGetLastError());
		}
		//SendPacket(fd, out, out.);

	}
#endif
}


#ifdef _NET_STREAM_TEST_

USHORT ClientManager::Read_Stream_Header(CStream* clsStream, BYTE* src)
{
	USHORT Packet_Header = 0x0;

	clsStream->WriteBuff_for_recv(src);
	clsStream->ReadData(&Packet_Header);

	return Packet_Header;
}

USHORT ClientManager::Read_Stream_Header_second(CStream* clsStream)
{
	USHORT Packet_Header_second = 0x0;

	clsStream->ReadData(&Packet_Header_second);

	return Packet_Header_second;
}
#endif

int ClientManager::Recv_Client_Packet(SOCKETINFO* socket_info, BYTE* packet)
{
	pc_socket_info = socket_info;

	EnterCriticalSection(&cs);
	ObjectUser *pObjectUser = FindUser(socket_info->fd);

	if (pObjectUser != NULL) {

		CStream* pStream = *pStreamSP;
		/****************/
		/* Open Stream  */
		/****************/
		pStream->StartRead();
		USHORT P_Header = Read_Stream_Header(*pStreamSP, packet);

		switch (P_Header)
		{
		case P_SEND_MSG:
			packet_send_msg_proc(&pObjectUser->sUser_info);
			break;
		case P_MOVE_POS:
			packet_move_proc(&pObjectUser->sUser_info);
			break;
		case P_BROAD_CAST_MSG:
			/* broad casting */
			broadcast_userinfo(pObjectUser->sUser_info.id, &pObjectUser->sUser_info);
			//pClientManager->broadcast_userinfo_zone(sInfo->fd, pOvluser);
			break;
		case P_USER_ACTIVITY:
			packet_user_activity_proc(&pObjectUser->sUser_info);
			break;
		case P_MAX:
			break;
		}
		/****************/
		/* Close Stream */
		/****************/
		pStream->EndRead();
	}
	/* need mutext */
	LeaveCriticalSection(&cs);
	return true;
}

int ClientManager::packet_move_proc(ObjectUser::User* p_user)
{
	ObjectUser *pObjectUser = FindUser(p_user->id);

	if (pObjectUser != NULL) {
		USHORT packet_header_second = Read_Stream_Header_second(*pStreamSP);
		/* TODO : get user position from data stream */
		CStream *pStream = *pStreamSP;
		int client_pos_x;
		int client_pos_y;
		int client_zone;

		int pos_x;
		int pos_y;

		pStream->ReadData(&client_pos_x);
		pStream->ReadData(&client_pos_y);
		pStream->ReadData(&client_zone);
		/* TODO : end */

		p_user->id = pObjectUser->sUser_info.id;
		p_user->zone = pObjectUser->sUser_info.zone;

		pos_x = pObjectUser->sUser_info.pos_x;
		pos_y = pObjectUser->sUser_info.pos_y;

		switch (packet_header_second)
		{
		case P_S_MOVE_ZONE:
			break;
		case P_S_MOVE_UP:
			if ((pos_y + 1) <= 255)
			{
				pObjectUser->sUser_info.pos_y++;
			}
			break;
		case P_S_MOVE_DOWN:
			if ((pos_y - 1) >= 0)
			{
				pObjectUser->sUser_info.pos_y--;
			}
			break;
		case P_S_MOVE_LEFT:
			if ((pos_x - 1) >= 0)
			{
				pObjectUser->sUser_info.pos_x--;
			}
			break;
		case P_S_MOVE_RIGHT:
			if ((pos_x + 1) <= 255)
			{
				pObjectUser->sUser_info.pos_x++;
			}
			break;
		case P_S_MOVE_RAND_TELEPORT:
			srand((unsigned int)timeGetTime());
			pObjectUser->sUser_info.pos_x = rand() % AI_MAX_POS_X;
			pObjectUser->sUser_info.pos_y = rand() % AI_MAX_POS_Y;

			break;
		case P_S_MOVE_NONE:
		default:
			break;

		}
#if 1
		DWORD writen = 0;

		BYTE buff[30];

		UpdateUserInfo(p_user->id, &pObjectUser->sUser_info);
		
		pStream->UpdateBuff(&pObjectUser->sUser_info.pos_x, 4, sizeof(pObjectUser->sUser_info.pos_x));
		pStream->UpdateBuff(&pObjectUser->sUser_info.pos_y, 8, sizeof(pObjectUser->sUser_info.pos_y));

		pObjectUser->Set_Zone( pObjectUser->myWolrd.calc_zone(pObjectUser->sUser_info.pos_x, pObjectUser->sUser_info.pos_y) );
		pStream->UpdateBuff(&pObjectUser->sUser_info.zone, 12, sizeof(pObjectUser->sUser_info.zone));

		pStream->CopyData(&buff);

		memcpy(pc_socket_info->dataBuf.buf, buff, sizeof(buff));

		/* return message by send socket */
		if (WSASend(p_user->id,
			(WSABUF*)&pc_socket_info->dataBuf,
			1,
			(DWORD *)&writen,
			0,
			&pc_socket_info->overlapped,
			NULL) == SOCKET_ERROR) {
			if (WSAGetLastError() != WSA_IO_PENDING) {
				printf("WSASend Error.. [%d] \n", WSAGetLastError());
			}
		}

		//broadcast_userinfo(p_user->id, &pObjectUser->sUser_info);
		broadcast_other_player(pObjectUser);
#endif
	}

	return true;
}

int ClientManager::packet_send_msg_proc(ObjectUser::User* p_user)
{
	ObjectUser *pObjectUser = FindUser(p_user->id);

	if (pObjectUser != NULL)
	{
		p_user->id = pObjectUser->sUser_info.id;
		pObjectUser->Set_Msg("org\n");

#if 1
		DWORD writen = 0;

		/* return message by send socket */
		if (WSASend(p_user->id,
			(WSABUF*)&pc_socket_info->dataBuf,
			1,
			(DWORD *)&writen,
			0,
			&pc_socket_info->overlapped,
			NULL) == SOCKET_ERROR) {
			if (WSAGetLastError() != WSA_IO_PENDING) {
				printf("WSASend Error.. [%d] \n", WSAGetLastError());
			}
		}
#endif
		broadcast_userinfo(p_user->id, &pObjectUser->sUser_info);
	}
	return true;
}

int ClientManager::packet_user_activity_proc(ObjectUser::User* p_user)
{
	ObjectUser *pObjectUser = FindUser(p_user->id);
	unordered_map<int, pcMonster*>::iterator it_monster;
	pcMonster* pMonster;
	int target_monster_idx;
	bool is_dead_monster = FALSE;

	int target_player_idx;
	unordered_map<int, ObjectUser*>::iterator it_target_player;
	ObjectUser* pTargetPlayer;
	bool is_existence_target_player = FALSE;

	/* for subtract monster */
	vector<int> vec_erase_monster_list;
	vector<int>::iterator vec_it_erase_monster_list;

	if (pObjectUser != NULL)
	{
		/************************************************/
		/*                                              */
		/*               Packet receive                 */
		/*                                              */
		/************************************************/
		USHORT packet_header_second = Read_Stream_Header_second(*pStreamSP);
		CStream *pStream = *pStreamSP;

		p_user->id = pObjectUser->sUser_info.id;
		p_user->zone = pObjectUser->sUser_info.zone;

		switch (packet_header_second)
		{
		case P_S_ACTIVITY_ATTACK_TO_MONSTER:
			pStream->ReadData(&target_monster_idx);
			pObjectUser->Set_Msg("Attack Monster!!\n");

			it_monster = pObjManager->map_monster.find(target_monster_idx);

			if (it_monster != pObjManager->map_monster.end())
			{
				pMonster = it_monster->second;
				pMonster->monster_info.state.hp = (pMonster->monster_info.state.hp + pMonster->monster_info.state.shield) - pObjectUser->sUser_info.state.damage;
				pObjectUser->sUser_info.state.hp = (pObjectUser->sUser_info.state.hp + pObjectUser->sUser_info.state.shield) - pMonster->monster_info.state.damage;
				/* monster is dead */
				if (pMonster->monster_info.state.hp < 1){
					pObjManager->map_monster.erase(it_monster);
					delete pObjManager->Monster_list[target_monster_idx];
					/* player gain an experience */
					pObjectUser->sUser_info.state.experience += 10;
					if (pObjectUser->sUser_info.state.level < MAX_USER_LEVEL &&
						experience_step_for_level_up[pObjectUser->sUser_info.state.level] <= pObjectUser->sUser_info.state.experience) {
						/* player level up */
						pObjectUser->sUser_info.state.level++;
						/* player add damage */
						pObjectUser->sUser_info.state.damage += 10;
					}
					is_dead_monster = TRUE;
				}
				/* player is dead */
				if (pObjectUser->sUser_info.state.hp < 1) {
					/* player loss an experience */
					pObjectUser->sUser_info.state.experience -= 10;
					/* correction player's experience */
					if (pObjectUser->sUser_info.state.experience < 0) {
						pObjectUser->sUser_info.state.experience = 0;
					}
					if (experience_step_for_level_up[pObjectUser->sUser_info.state.level] > pObjectUser->sUser_info.state.experience) {
						/* player level down */
						pObjectUser->sUser_info.state.level--;
						/* correction player's level */
						if (pObjectUser->sUser_info.state.level < 1) {
							pObjectUser->sUser_info.state.level = 1;
						}
						/* player subtraction damage */
						pObjectUser->sUser_info.state.damage -= 10;
						/* correction player's damage */
						if (pObjectUser->sUser_info.state.damage < DEF_USER_DMG) {
							pObjectUser->sUser_info.state.damage = DEF_USER_DMG;
						}
						/* rebirth(HP) player */
						pObjectUser->sUser_info.state.hp = MAX_HP_USER;
					}
				}
			}
			else
			{
				is_dead_monster = TRUE;
			}
			break;
		case P_S_ACTIVITY_SKILL:
			/* need mutext */
			int recv_hp;
			USHORT recv_level;
			pStream->ReadData(&recv_level);
			pStream->ReadData(&recv_hp);

			/* subtraction player's mp */
			pObjectUser->sUser_info.state.mp -= 10;

			it_monster = pObjManager->map_monster.begin();
			while (it_monster != pObjManager->map_monster.end())
			{
				pMonster = it_monster->second;
				if (pMonster->monster_info.zone == pObjectUser->sUser_info.zone)
				{
					pMonster->monster_info.state.hp = pMonster->monster_info.state.hp - SKILL_POWER;
					/* monster is dead */
					if (pMonster->monster_info.state.hp < 1){
						vec_erase_monster_list.push_back(pMonster->monster_info.id);
						/* player gain an experience */
						pObjectUser->sUser_info.state.experience += 10;
						if (pObjectUser->sUser_info.state.level < MAX_USER_LEVEL &&
							experience_step_for_level_up[pObjectUser->sUser_info.state.level] <= pObjectUser->sUser_info.state.experience) {
							/* player level up */
							pObjectUser->sUser_info.state.level++;
							/* player add damage */
							pObjectUser->sUser_info.state.damage += 10;
						}
						is_dead_monster = TRUE;
					}
				}
				it_monster++;
			}
			for (vec_it_erase_monster_list = vec_erase_monster_list.begin(); vec_it_erase_monster_list != vec_erase_monster_list.end(); vec_it_erase_monster_list++) {
				it_monster = pObjManager->map_monster.find(*vec_it_erase_monster_list);
				pMonster = it_monster->second;
				pObjManager->map_monster.erase(it_monster);
				delete pObjManager->Monster_list[pMonster->monster_info.id];
			}
			break;
		case P_S_ACTIVITY_ATTACK_TO_PLAYER:
			pStream->ReadData(&target_player_idx);
			pObjectUser->Set_Msg("Attack other player!!\n");

			it_target_player = map_user.find(target_player_idx);
			if (it_target_player != map_user.end())
			{
				is_existence_target_player = TRUE;
				pTargetPlayer = it_target_player->second;
				pTargetPlayer->sUser_info.state.hp -= pObjectUser->sUser_info.state.damage;
				/* target player is dead */
				if (pTargetPlayer->sUser_info.state.hp < 1) {
					/* target player loss an experience */
					pTargetPlayer->sUser_info.state.experience -= 10;
					if (experience_step_for_level_up[pTargetPlayer->sUser_info.state.level] > pTargetPlayer->sUser_info.state.experience) {
						/* target player level down */
						pTargetPlayer->sUser_info.state.level--;
						/* correction target player's level */
						if (pTargetPlayer->sUser_info.state.level < 1) {
							pTargetPlayer->sUser_info.state.level = 1;
						}
						/* target player subtraction damage */
						pTargetPlayer->sUser_info.state.damage -= 10;
						/* correction player's damage */
						if (pTargetPlayer->sUser_info.state.damage < DEF_USER_DMG) {
							pTargetPlayer->sUser_info.state.damage = DEF_USER_DMG;
						}
						/* rebirth(HP) target player */
						pTargetPlayer->sUser_info.state.hp = MAX_HP_USER;
						/* correction target player's experience */
						if (pTargetPlayer->sUser_info.state.experience < 0) {
							pTargetPlayer->sUser_info.state.experience = 0;
						}
					}
				}
			}
			else
			{
				is_existence_target_player = FALSE;
			}
			break;
		case P_S_ACTIVITY_NONE:
			break;
		default:
			break;
		}

		/************************************************/
		/*                                              */
		/*               Packet return                  */
		/*                                              */
		/************************************************/
#if 1
		USHORT p_Head = P_USER_ACTIVITY;
		USHORT p_s_Head;
		CStream* p_wStream = *pStreamSP;
		BYTE send_packet[24];
		int hp_zero = 0;

		switch (packet_header_second)
		{
		case P_S_ACTIVITY_ATTACK_TO_MONSTER:
			p_s_Head = P_S_ACTIVITY_ATTACK_TO_MONSTER;
			/****************/
			/* Open Stream  */
			/****************/
			memset(send_packet, 0, sizeof(send_packet));
			p_wStream->StartWrite(send_packet);
			p_wStream->WriteData(&p_Head);
			p_wStream->WriteData(&p_s_Head);
			p_wStream->WriteData(&target_monster_idx);
			if (is_dead_monster == TRUE)
			{
				p_wStream->WriteData(&hp_zero);
			}
			else
			{
				p_wStream->WriteData(&pMonster->monster_info.state.hp);
			}
			p_wStream->WriteData(&pObjectUser->sUser_info.state.hp);
			p_wStream->WriteData(&pObjectUser->sUser_info.state.level);
			p_wStream->WriteData(&pObjectUser->sUser_info.state.experience);

			/****************/
			/* Close Stream */
			/****************/
			p_wStream->EndWrite();
			memcpy(pc_socket_info->dataBuf.buf, send_packet, sizeof(send_packet));

			break;
		case P_S_ACTIVITY_ATTACK_TO_PLAYER:
			p_s_Head = P_S_ACTIVITY_ATTACK_TO_PLAYER;
			/****************/
			/* Open Stream  */
			/****************/
			memset(send_packet, 0, sizeof(send_packet));
			p_wStream->StartWrite(send_packet);
			p_wStream->WriteData(&p_Head);
			p_wStream->WriteData(&p_s_Head);
			p_wStream->WriteData(&target_player_idx);
			if (is_existence_target_player == TRUE) {
				p_wStream->WriteData(&pTargetPlayer->sUser_info.state.hp);
			}
			else
			{
				p_wStream->WriteData(&hp_zero);
			}
			p_wStream->WriteData(&pObjectUser->sUser_info.state.hp);
			p_wStream->WriteData(&pObjectUser->sUser_info.state.level);
			p_wStream->WriteData(&pObjectUser->sUser_info.state.experience);
			/****************/
			/* Close Stream */
			/****************/
			p_wStream->EndWrite();
			memcpy(pc_socket_info->dataBuf.buf, send_packet, sizeof(send_packet));

			break;
		case P_S_ACTIVITY_SKILL:
			p_s_Head = P_S_ACTIVITY_SKILL;
			/****************/
			/* Open Stream  */
			/****************/
			memset(send_packet, 0, sizeof(send_packet));
			p_wStream->StartWrite(send_packet);
			p_wStream->WriteData(&p_Head);
			p_wStream->WriteData(&p_s_Head);
			p_wStream->WriteData(&pObjectUser->sUser_info.state.mp);
			p_wStream->WriteData(&pObjectUser->sUser_info.state.level);
			p_wStream->WriteData(&pObjectUser->sUser_info.state.experience);
			/****************/
			/* Close Stream */
			/****************/
			p_wStream->EndWrite();
			memcpy(pc_socket_info->dataBuf.buf, send_packet, sizeof(send_packet));

			break;
		}
#endif

		DWORD writen = 0;
		/* return message by send socket */
		if (WSASend(p_user->id,
			(WSABUF*)&pc_socket_info->dataBuf,
			1,
			(DWORD *)&writen,
			0,
			&pc_socket_info->overlapped,
			NULL) == SOCKET_ERROR) {
			if (WSAGetLastError() != WSA_IO_PENDING) {
				printf("WSASend Error.. [%d] \n", WSAGetLastError());
			}
		}

	}
	return true;
}