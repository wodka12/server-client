#pragma once
#include "Act.h"
#include "TcpAct.h"
#include "TcpSocket.h"

class Receiver : public Actor
{
public:
	Receiver(){};
public:
	void ProcEvent(Act* act, DWORD bytes_transferred)
	{
		assert(dynamic_cast<TcpAct*>(act));
		TcpAct& tcpact = *dynamic_cast<TcpAct*>(act);
		assert(tcpact.TcpSocket_);
		TcpSocket& tcpsocket = *tcpact.TcpSocket_;
		if (bytes_transferred == 0)
		{
			tcpsocket.Disconnect();
		}
		else
		{
			//printf("¹ÞÀº °ª = %s\n", tcpsocket.RecvBuf_);
			tcpsocket.Recv();
			tcpsocket.Send((BYTE*)tcpsocket.RecvBuf_, bytes_transferred);
		}
	}
	void ProcError(Act* act, DWORD error)
	{
		assert(dynamic_cast<TcpAct*>(act));
		TcpAct& tcpact = *dynamic_cast<TcpAct*>(act);
		assert(tcpact.TcpSocket_);
		TcpSocket& tcpsocket = *tcpact.TcpSocket_;
		tcpsocket.Disconnect();
	}

	void Init(Proactor* proactor)
	{
		Proactor_ = proactor;
	}
};