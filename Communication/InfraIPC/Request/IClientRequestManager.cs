using Intel.IntelConnect.IPC.CommonTypes;

namespace Intel.IntelConnect.IPC.Request
{
    public interface IClientRequestManager
    {
        public bool GetPendingRequest(long requestId, out ClientRequest? request);

        public IAsyncEnumerable<T> SendLongRequestAsync<T, R>(string methodName ,R message) where T : IMessageHeader where R : IMessageHeader;

        public Task<T?> SendRequestAsync<T, R>(string methodName, R message) where T : IMessageHeader where R : IMessageHeader;
        public Task<T?> SendOpenSessionRequestAsync<T, R>(R message) where T : IMessageHeader where R : IMessageHeader;
    }
}
