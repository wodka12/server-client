#pragma once

#define MAX_HP_MONSTER 100
#define DEF_MONSTER_DMG 10
#define DEF_MONSTER_SHD 0

class pcMonster
{
public:
	struct s_State
	{
		int level;
		int damage;
		int shield;
		int hp;
	};
	struct Monster
	{
		int id;
		int pos_x;
		int pos_y;
		int zone;
		s_State state;
	};
	Monster monster_info;

	pcMonster() {}
	~pcMonster() {}
};

