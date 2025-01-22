using AsyncPipeTransport.Channel;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Extensions;
using Microsoft.Extensions.Logging;

namespace AsyncPipeTransport.ServerHandlers
{
    public abstract class BaseRequestCommand<T, Q> : IRequestCommand where Q : MessageHeader
    {
        protected ILogger<T> Log { get; private set; }
        private IChannelSender _sender = default!;
        protected long RequestId { get; private set; }

        protected abstract Task<bool> ExecuteInternal(Q request);

        public BaseRequestCommand(ILogger<T> logger)
        {
            Log = logger;
            Log.LogInformation("Request Handler Created");
        }

        public Task<bool> Execute(IChannelSender sender, long requestId, string requestJson)
        {
            _sender = sender;
            RequestId = requestId;

            var requestMsg = requestJson.FromJson<Q>();
            return ExecuteInternal(requestMsg);
        }

        protected Task SendLastResponse<R>(R responseMessage) where R : MessageHeader
        {
            return _sender.SendAsync(responseMessage.BuildResponseMessage(RequestId));
        }

        protected Task SendContinuingResponse<R>(R responseMessage) where R : MessageHeader
        {
            return _sender.SendAsync(responseMessage.BuildContinuingResponseMessage(RequestId));
        }

        protected Task SendEvent<R>(R eventMessage) where R : MessageHeader
        {
            return _sender.SendAsync(eventMessage.BuildServerEventMessage());
        }
    }
}