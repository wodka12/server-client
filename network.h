#pragma once
#define MAX_BUFF 1024
typedef enum _IO_OPERATION
{
	ClientIoRead,
	ClientIoWrite
} IO_OPERATION, *PIO_OPERATION;

struct SOCKETINFO {
	WSAOVERLAPPED overlapped;
	SOCKET fd;
	WSABUF dataBuf;
	char buf[MAX_BUFF];
	int readn;
	int writen;
	IO_OPERATION IOOperation;
};

template <typename T>
class Singleton
{
private:
	static T *pInstance;
public:
	static T& getInstance()
	{
		if (pInstance == NULL) pInstance = new T;
		return *pInstance;
	}
	static T* getInstancePtr()
	{
		if (pInstance == NULL) pInstance = new T;
		return pInstance;
	}
	static void releaseInstance()
	{
		if (pInstance != NULL) delete pInstance;
	}
};


#ifdef _NET_STREAM_TEST_


namespace MySream
{
	class CStream
	{
	public:
		CStream(VOID) { }
		CStream(BYTE *src) { mr_BufferPointer = src; }
		~CStream(VOID){}

	private:


	public:
		BOOL SetBuffer(BYTE *buffer);
		BYTE *mr_BufferPointer;
		BYTE *mw_BufferPointer;
		DWORD mLength;

	public:
		void StartRead(void) { 
			mr_BufferPointer = new BYTE[30];
			mLength = 0;
		}
		void EndRead(void) { delete[] mr_BufferPointer; }

		template <typename T>
		void StartWrite(T* Data) {
			mw_BufferPointer = Data;
			mLength = 0;
		}
		void EndWrite(void) { }

		template <typename T>
		BOOL ReadData(T* Data)
		{
			CopyMemory(Data, mr_BufferPointer + mLength, sizeof(T));

			mLength += sizeof(T);

			return TRUE;
		}

		template <typename T>
		BOOL WriteData(T* Data)
		{
			CopyMemory(mw_BufferPointer + mLength, Data, sizeof(T));

			mLength += sizeof(T);

			return TRUE;
		}

#if 1
		template <typename T>
		BOOL WriteBuff_for_recv(T Data)
		{
			CopyMemory(mr_BufferPointer + mLength, Data, 30);

			mLength += sizeof(T);

			mLength = 0;

			return TRUE;
		}
		template <typename T>
		BOOL ReadBuff(T* Data, int size)
		{
			CopyMemory(Data, mr_BufferPointer + mLength, size);

			mLength += size;

			return TRUE;
		}

		template <typename T>
		BOOL UpdateBuff(T* Data, int offset, int size)
		{
			CopyMemory(mr_BufferPointer + offset, Data, size);

			return TRUE;
		}

		template <typename T>
		BOOL CopyData(T* Data)
		{
			CopyMemory(Data, mr_BufferPointer, sizeof(T));

			return TRUE;
		}
		template <typename T>
		BOOL WriteString(T Data)
		{
			CopyMemory(mw_BufferPointer + mLength, Data, sizeof(T));

			mLength += sizeof(T);

			return TRUE;
		}
#endif
		DWORD GetLength(VOID)
		{
			return mLength;
		}
	};

	class CStreamSP
	{
	public:
		CStreamSP(VOID)  { Stream = new CStream(); }
		~CStreamSP(VOID) { 
			delete Stream; 
		}

		CStream* operator->(VOID)
		{
			return Stream;
		}
		operator CStream*(VOID)
		{
			return Stream;
		}

	private:
		CStream    *Stream;
	};
}

using namespace MySream;


struct st_NET_PACKET_HEADER {
	BYTE NetHeader;
	BYTE NetSize;
	BYTE NetType;
	BYTE NetBlank;
};
#endif


class network
{
public:
	SOCKET listen_fd, client_fd;

public:
	network(){}
	~network(){}

	void initialize_network();
};


