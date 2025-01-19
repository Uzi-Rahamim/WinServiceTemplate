using AsyncPipeTransport.Channel;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Extensions;
using Microsoft.Extensions.Logging;

namespace AsyncPipeTransport.ServerScheduler
{
    public abstract class BaseRequestHandler<T, Q> : IRequestHandler where Q : MessageHeader
    {
        protected ILogger<T> Log { get; private set; }
        private ISender _transportSender = default!;
        protected long RequestId { get; private set; }

        protected abstract Task ExecuteInternal(Q request);

        public BaseRequestHandler(ILogger<T> logger)
        {
            Log = logger;
            Log.LogInformation("Request Handler Created");
        }

        public Task Execute(ISender sender, long requestId, string requestJson)
        {
            _transportSender = sender;
            RequestId = requestId;

            var requestMsg = requestJson.FromJson<Q>();
            return ExecuteInternal(requestMsg);
        }

        protected Task SendLastResponse<R>(R responseMessage) where R : MessageHeader
        {
            return _transportSender.SendAsync(responseMessage.BuildResponseMessage(RequestId));
        }

        protected Task SendContinuingResponse<R>(R responseMessage) where R : MessageHeader
        {
            return _transportSender.SendAsync(responseMessage.BuildContinuingResponseMessage(RequestId));
        }

        protected Task SendEvent<R>(R eventMessage) where R : MessageHeader
        {
            return _transportSender.SendAsync(eventMessage.BuildServerEventMessage());
        }

    }
}