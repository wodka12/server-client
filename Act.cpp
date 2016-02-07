#include "stdafx.h"
#include "Act.h"
#include "network.h"
#include "ClientManager.h"
#include "ObjManager.h"

extern ClientManager* pClientManager;
extern ObjManager* pObjManager;

unsigned int WINAPI Proactor::ThreadProc(LPVOID lpParam)
{
	SOCKETINFO *sInfo;
	int flags;
	int readn;
	Proactor *p_Pa = (Proactor*)lpParam;
	network *p_Net;

	/* initialize networking */
	p_Net = Singleton<network>::getInstancePtr();

	while (1) {
		if ((p_Net->client_fd = WSAAccept(p_Net->listen_fd, NULL, NULL, NULL, 0)) == INVALID_SOCKET) {
			printf("Accept Error\n");
			return 1;
		}
		printf(" Accepted client fd[%d] ", p_Net->client_fd);

		sInfo = pClientManager->GetEmptySocketinfo();
		pClientManager->PushUser(p_Net->client_fd);
		
		sInfo->fd = p_Net->client_fd;
		sInfo->dataBuf.len = MAX_BUFF;
		sInfo->dataBuf.buf = sInfo->buf;
		sInfo->IOOperation = ClientIoRead;

		/**************************/
		/* CreateIoCompletionPort */
		/**************************/
		HANDLE check = CreateIoCompletionPort((HANDLE)p_Net->client_fd, p_Pa->g_hIOCP, (DWORD)sInfo, 0);

		flags = 0;

		if (WSARecv(sInfo->fd, &sInfo->dataBuf, 1, (DWORD *)&readn, (DWORD *)&flags, &(sInfo->overlapped), NULL) == SOCKET_ERROR) {
			if (WSAGetLastError() != WSA_IO_PENDING) {
				printf("[%d] wsarecv error %d\n", __LINE__, WSAGetLastError());
			}
		}
#if 1
		ObjectUser* pObjUser = pClientManager->FindUser(p_Net->client_fd);
		if (send(sInfo->fd,
			(char*)&pObjUser->sUser_info,
			sizeof(pObjUser->sUser_info),
			0) == SOCKET_ERROR){
			printf("Send Error\n");
		}
#endif
	}
	return 1;
}

unsigned int WINAPI Proactor::WorkerThread(LPVOID lpParam)
{
	BOOL bSuccess = TRUE;
	SOCKETINFO* pOverlappedEx;
	DWORD               dwIoSize = 0;
	LPOVERLAPPED	 pOverlap = NULL;
#if 0
	int writen;
#endif
	int readn;
	int flags;
	Proactor *p_Pa = (Proactor*)lpParam;
	network *p_Net;
	SOCKETINFO *sInfo;

	/* initialize networking */
	p_Net = Singleton<network>::getInstancePtr();


	while (p_Pa->bWorkerRun) {
		/*****************************/
		/* GetQueuedCompletionStatus */
		/*****************************/
		bSuccess = GetQueuedCompletionStatus(p_Pa->g_hIOCP, &dwIoSize, (LPDWORD)&sInfo, &pOverlap, INFINITE);

		if (bSuccess == FALSE || dwIoSize == 0) {
			printf("Disconnected..[fd:%d]\n", sInfo->fd);
			shutdown(sInfo->fd, SD_BOTH);
			closesocket(sInfo->fd);

			pClientManager->send_client_closed(sInfo->fd);

			sInfo->fd = INVALID_SOCKET;
			continue;
		}

		if (bSuccess == TRUE && dwIoSize == 0 && pOverlap == NULL) {
			p_Pa->bWorkerRun = FALSE;
			continue;
		}

		pOverlappedEx = (SOCKETINFO*)pOverlap;

		if (sInfo->readn == 0) {
			sInfo->readn = dwIoSize;
			sInfo->writen = 0;
			//printf("Client[%d] size[%d]: %s", sInfo->fd, dwIoSize, pOverlappedEx->dataBuf.buf);
			//printf("Exec Thread Num [%d] \n", p->cur_thread_num);
			printf("Exec fd [%d] \n", sInfo->fd);
		}
		else {
			sInfo->writen += dwIoSize;
		}

		switch (sInfo->IOOperation)
		{
		case ClientIoRead:
			sInfo->IOOperation = ClientIoWrite;
			if (sInfo->readn > sInfo->writen) {
				memset(&(sInfo->overlapped), 0x00, sizeof(WSAOVERLAPPED));
				sInfo->fd = pOverlappedEx->fd;
				sInfo->dataBuf.buf = pOverlappedEx->dataBuf.buf;
				sInfo->dataBuf.len = dwIoSize;

#if 1
				//ObjectUser::User* pOvluser = (ObjectUser::User*)pOverlappedEx->dataBuf.buf;
				//pOvluser->id = sInfo->fd;

				BYTE test_byte[30];
				memcpy(test_byte, pOverlappedEx->dataBuf.buf, dwIoSize);

				pClientManager->Recv_Client_Packet(sInfo, test_byte);
#endif

#if 0
				/* return message by send socket */
				if (WSASend(sInfo->fd, 
					&(sInfo->dataBuf), 
					1, 
					(DWORD *)&writen, 
					0, 
					&(sInfo->overlapped), 
					NULL) == SOCKET_ERROR) {
					if (WSAGetLastError() != WSA_IO_PENDING) {
						printf("WSASend Error\n");
					}
				}
#endif
			}
			break;
		case ClientIoWrite:
			memset(&(sInfo->overlapped), 0x00, sizeof(WSAOVERLAPPED));
			memset(&(sInfo->buf), 0, MAX_BUFF);
			sInfo->readn = 0;
			sInfo->fd = pOverlappedEx->fd;
			sInfo->dataBuf.len = MAX_BUFF;
			sInfo->dataBuf.buf = pOverlappedEx->dataBuf.buf;
			sInfo->IOOperation = ClientIoRead;
			flags = 0;
			if (WSARecv(sInfo->fd, &sInfo->dataBuf, 1, (DWORD *)&readn, (DWORD *)&flags, &(sInfo->overlapped), NULL) == SOCKET_ERROR) {
				if (WSAGetLastError() != WSA_IO_PENDING) {
					printf("wsarecv error %d\n", WSAGetLastError());
				}
			}
			break;
		default:
			break;

		}
	}
	return 1;
}

unsigned int WINAPI Proactor::AiProc(LPVOID lpParam)
{
	static int run_dly = 0;

	pObjManager->PushMonster(MAX_MONSTER);
	while (1) {
		/* monster moving to random position */
		pObjManager->monster_random_moving();

		if (run_dly > 30)
		{
			/* need mutext */
			pClientManager->scan_monster_on_same_zone();
			pClientManager->scan_other_player_on_same_zone();
			/* need mutext */

			if (pClientManager->b_client_ping == FALSE)
			{
				pClientManager->check_ping_and_cut_user();
				pClientManager->b_client_ping = TRUE;
			}
			run_dly = 0;
		}
		run_dly++;
		Sleep(50);
	}
	return 1;
}

void Proactor::begin_thread_proc()
{
	hThread[cnt_thread] = (HANDLE)_beginthreadex(NULL, 0, ThreadProc, (LPVOID)this, 0, (unsigned int*)&dwThreadID[0]);
	ResumeThread(hThread[0]);
}
void Proactor::begin_thread_worker()
{
	DWORD result = 0;
	for (int i = 0; i < max_thread; i++) {
		cur_thread_num = i;
		hWorkerThread[i] = (HANDLE)_beginthreadex(NULL, 0, WorkerThread, (LPVOID)this, CREATE_SUSPENDED, &dwWorkerThreadID[i]);

		if (hWorkerThread[i] == NULL)
		{
			printf(" thread create error\n");
			return;
		}

		ResumeThread(hWorkerThread[i]);
	}

	//result = WaitForMultipleObjects(MAX_TH,hThread,TRUE,INFINITE);
	result = WaitForSingleObject(hWorkerThread[0], INFINITE);
	if (result == WAIT_FAILED) {
		puts("thread error");
		return;
	}
}
void Proactor::begin_thread_AI()
{
	aiThread[0] = (HANDLE)_beginthreadex(NULL, 0, AiProc, (LPVOID)this, 0, (unsigned int*)&dwThreadID[1]);
	ResumeThread(aiThread[0]);
}
