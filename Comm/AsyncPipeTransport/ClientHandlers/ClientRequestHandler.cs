using AsyncPipeTransport.Channel;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Extensions;
using System.Collections.Concurrent;

namespace AsyncPipeTransport.ClientHandlers
{
    public class ClientRequestHandler : IClientRequestHandler
    {
        private readonly ConcurrentDictionary<long, ClientRequest> _pendingRequests = new ConcurrentDictionary<long, ClientRequest>();
        private readonly ISequenceGenerator _requestIdGenerator;
        private readonly IClientChannel _transport;
        public ClientRequestHandler(ISequenceGenerator requestIdGenerator, IClientChannel transport)
        {
            this._requestIdGenerator = requestIdGenerator;
            this._transport = transport;
        }

        public bool GetPendingRequest(long requestId, out ClientRequest? request)
        {
            return _pendingRequests.TryGetValue(requestId, out request);
        }

        public async IAsyncEnumerable<T> SendLongRequest<T>(Func<long, string> buildPayload) where T : MessageHeader
        {
            var requestId = await SendNonBlock(buildPayload);
            FrameHeader? reply = null;
            do
            {
                reply = await WaitForNextFrame(requestId);

                if (reply == null)
                {
                    throw new TimeoutException();
                    //yield break;
                }

                var response = reply.ExtractMessageHeaders<T>();
                if (response == null)
                {
                    throw new ArgumentNullException("response is invalid");
                }
                yield return response;
            } while (!reply.IsLastFrame());

        }

        public Task<FrameHeader?> Send(Func<long, string> buildPayload)
        {
            long newRequestId = _requestIdGenerator.GetNextId();
            var payload = buildPayload(newRequestId);
            ClientRequest request = new ClientRequest(newRequestId, payload);
            return SendRequest(request);
        }

        private  Task<FrameHeader> WaitForNextFrame(long requestId)
        {
            var request = _pendingRequests[requestId];
            return request.WaitForResponse((isLastFrame) => { if (isLastFrame) RemoveRequest(requestId); });
        }

        private async Task<long> SendNonBlock(Func<long, string> buildPayload)
        {
            long newRequestId = _requestIdGenerator.GetNextId();
            var payload = buildPayload(newRequestId);
            ClientRequest request = new ClientRequest(newRequestId, payload);
            await SendRequest(request, false);
            return newRequestId;
        }

        private Task<FrameHeader?> SendRequest(ClientRequest request, bool waitForRespose = true)
        {
            if (_pendingRequests.TryAdd(request.requestId, request))
            {
                _transport.SendAsync(request.payload).Wait();
                if (waitForRespose)
                {
                    return WaitForNextFrame(request.requestId).
                        ContinueWith(task => (FrameHeader?)task.Result); //convert to task nullable
                }
            }
            return Task.FromResult<FrameHeader?>(null);
        }

        private void RemoveRequest(long requestId)
        {
            _pendingRequests.TryRemove(requestId, out ClientRequest? request);
        }

    }
}
