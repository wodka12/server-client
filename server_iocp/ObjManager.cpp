#include "stdafx.h"
#include "ObjManager.h"
#include "pcMonster.h"

void ObjManager::PushMonster(int max_cnt)
{
	while (monster_idx < max_cnt)
	{
		Monster_list[monster_idx] = new pcMonster;
		pcMonster* pMonster = Monster_list[monster_idx];

		pMonster->monster_info.id = monster_idx;
		pMonster->monster_info.pos_x = rand() % AI_MAX_POS_X;
		pMonster->monster_info.pos_y = rand() % AI_MAX_POS_Y;
		pMonster->monster_info.zone = myWolrd.calc_zone(pMonster->monster_info.pos_x, pMonster->monster_info.pos_y);
		/* set state */
		pMonster->monster_info.state.hp = MAX_HP_MONSTER;
		pMonster->monster_info.state.damage = DEF_MONSTER_DMG;
		pMonster->monster_info.state.shield = DEF_MONSTER_SHD;
		pMonster->monster_info.state.level = 1;

		unordered_map<int, pcMonster*>::value_type value(monster_idx, pMonster);
		map_monster.insert(value);
		map_monster[monster_idx] = pMonster;

		monster_idx++;
	}
}

void ObjManager::scan_user_on_same_zone(ObjectUser* pUser)
{
	CStream* pStream = *pStreamSP;
	pcMonster* pMonster;
	BYTE send_packet[22];
	USHORT p_Head = P_SCAN_MONSTER_ON_SAME_ZONE;

	unordered_map<int, pcMonster*>::iterator it_monster = map_monster.begin();
	while (it_monster != map_monster.end())
	{
		pMonster = it_monster->second;
		if (pMonster != NULL)
		{
			if (pMonster->monster_info.zone == pUser->sUser_info.zone)
			{
				/****************/
				/* Open Stream  */
				/****************/
				memset(send_packet, 0, sizeof(send_packet));
				pStream->StartWrite(send_packet);
				pStream->WriteData(&p_Head);
				pStream->WriteData(&pMonster->monster_info.id);
				pStream->WriteData(&pMonster->monster_info.zone);
				pStream->WriteData(&pMonster->monster_info.pos_x);
				pStream->WriteData(&pMonster->monster_info.pos_y);
				pStream->WriteData(&pMonster->monster_info.state.hp);

				pUser->notify_from_monster(send_packet, sizeof(send_packet));

				/****************/
				/* Close Stream */
				/****************/
				pStream->EndWrite();
			}
		}
		it_monster++;
	}
}

void ObjManager::monster_random_moving(void)
{
	pcMonster* pMonster;
	int iran;
	int rand_pos_list[10];
	int seed = 0;

	srand((unsigned int)timeGetTime());
	iran = rand() % 10;

	for (int i = 0; i < 10; i++){
		rand_pos_list[i] = (iran + i) % 5;
	}

	unordered_map<int, pcMonster*>::iterator it_monster = map_monster.begin();
	while (it_monster != map_monster.end())
	{
		iran = rand_pos_list[seed];

		pMonster = it_monster->second;
		if (pMonster != NULL)
		{
			if (iran == 0) {
				pMonster->monster_info.pos_x+=2;
			}
			else if (iran == 1) {
				pMonster->monster_info.pos_y+=2;
			}
			else if (iran == 2) {
				pMonster->monster_info.pos_x-=2;
			}
			else if (iran == 3) {
				pMonster->monster_info.pos_y-=2;
			}
			else {

			}
		}
		it_monster++;

		seed++;
		if (seed > 9) {
			seed = 0;
		}
	}
}