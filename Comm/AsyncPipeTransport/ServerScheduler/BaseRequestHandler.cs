using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Extensions;
using AsyncPipeTransport.Transport;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AsyncPipeTransport.RequestHandler
{

    public interface IRequestHandlerBuilder
    {
        public IRequestHandler Build();
        public Opcode GetMessageType();
    }
    public class RequestHandlerBuilder : IRequestHandlerBuilder
    {
        Func<IRequestHandler> _factory;
        private readonly Opcode _messageType;

        public RequestHandlerBuilder(Opcode messageType, Func<IRequestHandler> factory)
        {
            _factory = factory;
            _messageType = messageType;
        }
        public IRequestHandler Build()
        {
            return _factory();
        }
        public Opcode GetMessageType()
        {
            return _messageType;

        }
    }

    public abstract class BaseRequestHandler<T, Q> : IRequestHandler where Q : MessageHeader
    {
        protected ILogger<T> Log { get; private set; }
        private ITransportSender _transportSender = default!;
        protected long RequestId { get; private set; }

        protected abstract Task ExecuteInternal(Q request);

        public BaseRequestHandler(ILogger<T> logger)
        {
            Log = logger;
            Log.LogInformation("Request Handler Created");
        }

        public Task Execute(ITransportSender sender, long requestId, string requestJson)
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