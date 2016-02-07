#pragma once

#define BUFSIZE 8192
class Actor;
class Act : public OVERLAPPED
{
public:
	Act()
	{
		hEvent = NULL;
		Internal = 0;
		InternalHigh = 0;
		Offset = 0;
		OffsetHigh = 0;
		Actor_ = NULL;
	}
	~Act() {}
	virtual void Complete(DWORD bytes_transferred) = 0;
	virtual void Error(DWORD error) = 0;
public:
	Actor* Actor_;
};

class Proactor
{
public:
	HANDLE hThread[1];
	DWORD dwThreadID[1];
	HANDLE hWorkerThread[8];
	unsigned int dwWorkerThreadID[8];
	HANDLE aiThread[1];

	int max_thread;
	DWORD cnt_thread = 0;
	int cur_thread_num;
public:
	Proactor(){}
	~Proactor(){}

public:
	static UINT WINAPI ThreadProc(LPVOID pProactor);
	static UINT WINAPI WorkerThread(LPVOID lpParam);
	static UINT WINAPI AiProc(LPVOID lpParam);

	void begin_threads() {
		begin_thread_proc();
		//begin_thread_AI();
		begin_thread_worker();
	};
	void begin_thread_proc();
	void begin_thread_worker();
	void begin_thread_AI();
public:
	void Init(int numofthreads) {
		max_thread = numofthreads;
		g_hIOCP = CreateIoCompletionPort(INVALID_HANDLE_VALUE, NULL, 0, 0);
		if (NULL == g_hIOCP) {
			printf("CreateIoCompletionPort failed: %d\n", GetLastError());
			return;
		}
	}
	void Register(HANDLE handle);
	void PostPrivateEvent(DWORD pId, Act* pActor);
	void ProcEvents();
public:
	BOOL bWorkerRun = TRUE;
	HANDLE g_hIOCP;
	DWORD TimeOut_;
	DWORD NumOfThreads_;
};

class Actor
{
public:
	Actor()
	{
		Proactor_ = NULL;
	}
	virtual void ProcEvent(Act* act, DWORD bytes_transferred) = 0;
	virtual void ProcError(Act* act, DWORD error) = 0;
public:
	Proactor* Proactor_;
};


class CTaskMutex
{
public:
	CTaskMutex(HANDLE hMutex)
	{
		m_hMutex = hMutex;
		WaitForSingleObject(m_hMutex, INFINITE);
	}

	~CTaskMutex()
	{
		ReleaseMutex(m_hMutex);
	}

private:
	HANDLE m_hMutex;
};

class CTaskCSLock
{
public:
	CTaskCSLock(CRITICAL_SECTION& hCSLock)
	{
		m_pcsLock = NULL;
		m_pcsLock = &hCSLock;
		EnterCriticalSection(m_pcsLock);
	}

	~CTaskCSLock()
	{
		LeaveCriticalSection(m_pcsLock);
	}

private:
	CRITICAL_SECTION* m_pcsLock;
};


