#pragma once
#include "Cworld.h"
#include "network.h"

#ifdef _AI_TEST_
#define AI_MAX_POS_X 256
#define AI_MAX_POS_Y 256

#define MAX_HP_USER 100
#define MAX_MP_USER 100
#define DEF_USER_DMG 20
#define DEF_USER_SHD 0
#define DEF_USER_LEVEL 1
#define MAX_USER_LEVEL 10
#define SKILL_POWER 50

#endif

class Object
{
private:
	int id;

public:
	~Object() {}

	Object(){}

	Cworld myWolrd;
};


#pragma warning(disable : 4996)
class ObjectUser : public Object
{
private:

public:
	bool is_connected = FALSE;
	struct s_State
	{
		USHORT level;
		int damage;
		int shield;
		int hp;
		int mp;
		int experience;
	};

	struct User
	{
		USHORT p_h;
		USHORT p_h_s;
		int id;
		int pos_x;
		int pos_y;
		int  zone;
		s_State state;
		char msg[20];
	};

	User sUser_info;
	~ObjectUser() {}

	void SetPos_X(int x)
	{
		sUser_info.pos_x = x;
	}

	void SetPos_Y(int y)
	{
		sUser_info.pos_y = y;
	}

	void Set_Zone(int zone)
	{
		sUser_info.zone = zone;
	}

	void Set_Msg(char* msg)
	{
		//sprintf(sUser_info.msg, "%s", msg);
		strcpy(sUser_info.msg, msg);
	}

	void Set_Id(int id)
	{
		sUser_info.id = id;
	}

	ObjectUser(){}
	ObjectUser(int inputId){
		Set_Id(inputId);
		SetPos_X(0);
		SetPos_Y(0);
		Set_Msg("    ");
	}
	void notify(char* msg)
	{
		sprintf(sUser_info.msg, "%4s", msg);
		send(sUser_info.id, (char*)&sUser_info, sizeof(User), 0);
		cout << "==========================" << endl;
		cout << "Notified data is incoming\n";
		cout << "Id = " << sUser_info.id << endl;
		cout << "x = " << sUser_info.pos_x << endl;
		cout << "y = " << sUser_info.pos_y << endl;
		cout << "==========================" << endl;
	}

	void notify(BYTE* msg, size_t size)
	{
		try{
			send(sUser_info.id, (char*)msg, size, 0);
		cout << "Notified data(byte) is incoming...\n" << endl;
		} 
		catch (DWORD dwError)
		{
			printf("Error code : %d", dwError);
		}
	}

	void notify_from_monster(BYTE* msg, size_t size)
	{
		try{
			send(sUser_info.id, (char*)msg, size, 0);
			cout << "Notified from monster..." << endl;
		}
		catch (DWORD dwError)
		{
			printf("Error code : %d", dwError);
		}
	}

	void notify_from_other_player(BYTE* msg, size_t size)
	{
		try{
			send(sUser_info.id, (char*)msg, size, 0);
			cout << "Notified from other player..." << endl;
		}
		catch (DWORD dwError)
		{
			printf("Error code : %d", dwError);
		}
	}
};

