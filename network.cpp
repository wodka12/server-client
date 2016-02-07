#include "stdafx.h"
#include "network.h"
#include "ClientManager.h"
#include "TcpSocket.h"

extern u_short g_port;
TcpSocket* pTcpSocket = new TcpSocket;

void network::initialize_network()
{
	WSADATA wsaData;
	sockaddr_in addr;
	//HANDLE sem_handle;

#if 1
	pTcpSocket->Init();
#endif
#if 0
	/**************************/
	/* CreateIoCompletionPort */
	/**************************/
	g_hIOCP = CreateIoCompletionPort(INVALID_HANDLE_VALUE, NULL, 0, 0);
	if (NULL == g_hIOCP) {
		printf("CreateIoCompletionPort failed: %d\n", GetLastError());
		return;
	}
#endif

	if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
		return;
	}

	if ((listen_fd = WSASocket(AF_INET, SOCK_STREAM, IPPROTO_TCP, NULL, 0, WSA_FLAG_OVERLAPPED)) == INVALID_SOCKET) {
		return;
	}
	//sem_handle = CreateSemaphore(NULL, 1, 1000, (LPCWSTR)"Test");
	addr.sin_family = AF_INET;
	addr.sin_addr.s_addr = htonl(INADDR_ANY);
	addr.sin_port = htons(g_port);

	if (bind(listen_fd, (struct sockaddr *)&addr, sizeof(addr)) == SOCKET_ERROR) {
		printf("[%d] bind Error\n", __LINE__);
		return;
	}
	if (listen(listen_fd, 5) == SOCKET_ERROR) {
		printf("[%d] listen Error\n", __LINE__);
		return;
	}
}

