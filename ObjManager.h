#pragma once
#include "Object.h"
#include "pcMonster.h"

#define MAX_MONSTER 12

const int experience_step_for_level_up[MAX_USER_LEVEL]
{
	 0,
	10,
	20,
	30,
	40,
	50,
	60,
	70,
	80,
	90,
};

class ObjManager : public Object
{
public:
	int monster_idx;
	unordered_map<int, pcMonster*> map_monster;
	pcMonster* Monster_list[MAX_MONSTER];

	CStreamSP* pStreamSP = new CStreamSP;

public:
	ObjManager() {
		monster_idx = 0;
	}
	~ObjManager() {}

	void PushMonster(int max_cnt);
	void scan_user_on_same_zone(ObjectUser* pUser);
	void monster_random_moving(void);
};

