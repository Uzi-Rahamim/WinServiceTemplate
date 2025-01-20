using AsyncPipeTransport.CommonTypes;

namespace AsyncPipeTransport.ClientHandlers
{
    public interface IClientRequestHandler
    {
        public bool GetPendingRequest(long requestId, out ClientRequest? request);

        public  IAsyncEnumerable<T> SendLongRequest<T>(Func<long, string> buildPayload) where T : MessageHeader;

        public Task<FrameHeader?> Send(Func<long, string> buildPayload);
    }
}
