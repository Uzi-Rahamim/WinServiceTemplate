using Intel.IntelConnect.IPC.Channel;
using Intel.IntelConnect.IPC.CommonTypes;
using Intel.IntelConnect.IPC.CommonTypes.InternalMassages;
using Intel.IntelConnect.IPC.Exceptions;
using Intel.IntelConnect.IPC.Extensions;
using Intel.IntelConnect.IPC.Utils;
using System.Collections.Concurrent;

namespace Intel.IntelConnect.IPC.Request
{
    public delegate ClientRequest ClientRequestFactory(long requestId, string payload);

    public class ClientRequestsManager : IClientRequestManager
    {
        private readonly ConcurrentDictionary<long, ClientRequest> _pendingRequests = new ConcurrentDictionary<long, ClientRequest>();
        private readonly ISequenceGenerator _requestIdGenerator;
        private readonly IChannel _channel;
        private ClientRequestFactory _requestFactory;
        public ClientRequestsManager(ISequenceGenerator requestIdGenerator, IClientChannel transport, ClientRequestFactory requestFactory)
        {
            _requestIdGenerator = requestIdGenerator;
            _channel = transport;
            _requestFactory = requestFactory;
        }

        public bool GetPendingRequest(long requestId, out ClientRequest? request)
        {
            return _pendingRequests.TryGetValue(requestId, out request);
        }

        public async IAsyncEnumerable<T> SendLongRequestAsync<T, R>(R message) where T : MessageHeader where R : MessageHeader
        {
            var requestId = await SendNonBlockAsync((requestId) => message.BuildRequestMessage(requestId));
            FrameHeader? reply = null;
            do
            {
                reply = await WaitForNextFrameAsync(requestId);
                if (reply.IsLastFrame() && !reply.IsErrorFrame()) //terminate 
                {
                    yield break;
                }

                yield return ExtructResponse<T>(reply);
            } while (!reply.IsLastFrame());
        }

        public async Task<T?> SendRequestAsync<T, R>(R message) where T : MessageHeader where R : MessageHeader
        {
            var reply = await SendAsync((requestId) => message.BuildRequestMessage(requestId));
            return ExtructResponse<T>(reply);
        }

        public async Task<T?> SendOpenSessionRequestAsync<T, R>(R message) where T : MessageHeader where R : MessageHeader
        {
            var reply = await SendAsync((requestId) => message.BuildOpenSessionRequestMessage(requestId));
            return ExtructResponse<T>(reply);
        }

        private T ExtructResponse<T>(FrameHeader? frame) where T : MessageHeader
        {
            if (frame == null)
            {
                throw new TimeoutException();
            }
            else if (frame.IsErrorFrame())
            {
                var errorMsg = frame.ExtractMessageHeaders<ErrorMessage>();
                if (errorMsg == null)
                {
                    throw new ErrorResponseException("Error Response is invalid",(int) ErrorCode.InternalServerError);
                }
                throw new ErrorResponseException(errorMsg.Message, errorMsg.Code);
            }

            var response = frame.ExtractMessageHeaders<T>();
            if (response == null)
            {
                throw new ErrorResponseException("Response is invalid", (int)ErrorCode.InternalServerError);
            }

            return response;
        }

        private Task<FrameHeader?> SendAsync(Func<long, string> buildPayload)
        {
            long newRequestId = _requestIdGenerator.GetNextId();
            var payload = buildPayload(newRequestId);
            ClientRequest request = _requestFactory(newRequestId, payload); //new ClientRequest(newRequestId, payload);
            return SendRequestAsync(request);
        }

        private Task<FrameHeader> WaitForNextFrameAsync(long requestId)
        {
            var request = _pendingRequests[requestId];
            return request.WaitForResponseAsync((isLastFrame) => { if (isLastFrame) RemoveRequest(requestId); });
        }

        private async Task<long> SendNonBlockAsync(Func<long, string> buildPayload)
        {
            long newRequestId = _requestIdGenerator.GetNextId();
            var payload = buildPayload(newRequestId);
            ClientRequest request = _requestFactory(newRequestId, payload); //new ClientRequest(newRequestId, payload);
            await SendRequestAsync(request, false);
            return newRequestId;
        }

        private async Task<FrameHeader?> SendRequestAsync(ClientRequest request, bool waitForRespose = true)
        {
            if (_pendingRequests.TryAdd(request.requestId, request))
            {
                await _channel.SendAsync(request.payload,CancellationToken.None);
                if (waitForRespose)
                {
                    var frameHeader = await WaitForNextFrameAsync(request.requestId);
                        //ContinueWith(task => (FrameHeader?)task.Result); //convert to task nullable

                    return frameHeader;
                }
            }
            return null;
        }

        private void RemoveRequest(long requestId)
        {
            _pendingRequests.TryRemove(requestId, out ClientRequest? request);
        }

    }
}
