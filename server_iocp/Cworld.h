#pragma once
#include "pcMonster.h"

#define MAX_ZONE 4
class Cworld
{

public:
	Cworld() {}
	~Cworld() {}

	int calc_zone(int pos_x, int pos_y);
};

