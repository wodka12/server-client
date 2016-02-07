#ifndef _PACKET_DEF_
#define _PACKET_DEF_

typedef enum e_Packet {
	P_SEND_MSG		= 0x0,
	P_MOVE_POS		= 0x01,
	P_USER_ACTIVITY   = 0x02,
	P_BROAD_CAST_MSG  = 0x04,
	P_SCAN_MONSTER_ON_SAME_ZONE = 0x08,
	P_SCAN_OTHER_PLAYER_ON_SAME_ZONE = 0x10,
	P_MAX			= 0xFF,
} e_packet_def;

typedef enum e_Packet_Move_Pos {
	P_S_MOVE_ZONE = 0x00,
	P_S_MOVE_UP = 0x01,
	P_S_MOVE_DOWN = 0x02,
	P_S_MOVE_LEFT = 0x04,
	P_S_MOVE_RIGHT = 0x08,
	P_S_MOVE_RAND_TELEPORT = 0x10,
	P_S_MOVE_NONE = 0xFF,
} e_packet_move_pos_def;

typedef enum e_Packet_User_Activity {
	P_S_ACTIVITY_ATTACK_TO_MONSTER = 0x00,
	P_S_ACTIVITY_SKILL = 0x01,
	P_S_ACTIVITY_ATTACK_TO_PLAYER = 0x02,
	P_S_ACTIVITY_NONE = 0xFF,
} e_packet_User_Activity_def;

#endif