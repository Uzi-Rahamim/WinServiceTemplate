using Intel.IntelConnect.IPC.CommonTypes;

namespace Intel.IntelConnect.IPC.Request
{
    public interface IClientRequestManager
    {
        public bool GetPendingRequest(long requestId, out ClientRequest? request);

        public IAsyncEnumerable<T> SendLongRequestAsync<T, R>(R message) where T : MessageHeader where R : MessageHeader;

        public Task<T?> SendRequestAsync<T, R>(R message) where T : MessageHeader where R : MessageHeader;
        public Task<T?> SendOpenSessionRequestAsync<T, R>(R message) where T : MessageHeader where R : MessageHeader;
    }
}
