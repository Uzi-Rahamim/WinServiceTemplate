using AsyncPipeTransport.CommonTypes;

namespace AsyncPipeTransport.ClientHandlers
{
    public interface IClientRequestManager
    {
        public bool GetPendingRequest(long requestId, out ClientRequest? request);

        public IAsyncEnumerable<T> SendLongRequest<T, R>(R message) where T : MessageHeader where R : MessageHeader;

        public Task<T?> SendRequest<T, R>(R message) where T : MessageHeader where R : MessageHeader;
        public Task<T?> SendOpenSessionRequest<T, R>(R message) where T : MessageHeader where R : MessageHeader;
    }
}
