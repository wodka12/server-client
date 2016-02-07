#include "stdafx.h"
#include "Cworld.h"

#define CENTER_POS 127
/*
---------------
|  4   |   1  |
|      |      |
---------------
|  3   |   2  |
|      |      |
--------------- */
int Cworld::calc_zone(int pos_x, int pos_y)
{
	int ret_zone;

	if (pos_x > CENTER_POS && pos_y > CENTER_POS) {
		ret_zone = 1;
	}
	else if (pos_x > CENTER_POS && pos_y <= CENTER_POS) {
		ret_zone = 2;
	}
	else if (pos_x <= CENTER_POS && pos_y <= CENTER_POS) {
		ret_zone = 3;
	}
	else if (pos_x <= CENTER_POS && pos_y > CENTER_POS) {
		ret_zone = 4;
	}

	return ret_zone;
}
