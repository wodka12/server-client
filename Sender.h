#pragma once
#include "Act.h"
#include "TcpAct.h"
#include "TcpSocket.h"
class Sender : public Actor
{
public:
	Sender(){};
public:
	void ProcEvent(Act* act, DWORD bytes_transferred)
	{
		assert(dynamic_cast<TcpAct*>(act));
		TcpAct& tcpact = *dynamic_cast<TcpAct*>(act);
		assert(tcpact.TcpSocket_);
		TcpSocket& tcpsocket = *tcpact.TcpSocket_;
	}
	void ProcError(Act* act, DWORD error)
	{
		assert(dynamic_cast<TcpAct*>(act));
		TcpAct& tcpact = *dynamic_cast<TcpAct*>(act);
		assert(tcpact.TcpSocket_);
		TcpSocket& tcpsocket = *tcpact.TcpSocket_;
	}
	void Init(Proactor* proactor)
	{
		Proactor_ = proactor;
	}
};