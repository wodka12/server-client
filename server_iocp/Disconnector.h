#pragma once
#include "Act.h"
#include "TcpSocket.h"

class Disconnector : public Actor
{
public:
	Disconnector(){}
public:
	void ProcEvent(Act* act, DWORD bytes_transferred)
	{
		TcpAct& tcpact = *dynamic_cast<TcpAct*>(act);
		assert(tcpact.TcpSocket_);
		TcpSocket& tcpsocket = *tcpact.TcpSocket_;
		assert(tcpsocket.Acceptor_);
		tcpsocket.Reuse();
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