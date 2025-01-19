//using AsyncPipeTransport.CommonTypes;
//using AsyncPipeTransport.Transport;
//using Microsoft.Extensions.Logging;

//namespace ServiceAPI;

//public abstract class BaseRequestHandler<T,Q> : IRequestHandler where Q : MessageHeader
//{
//    protected ILogger<T> Log { get; private set; }
//    private readonly Opcode _messageType;
//    private ITransportSender _transportSender = default!;
//    protected long RequestId { get; private set; }

//    protected abstract Task ExecuteInternal(Q request);

//    public BaseRequestHandler(ILogger<T> logger, Opcode messageType)
//    {
//        _messageType = messageType;
//        Log = logger;
//        Log.LogInformation($"Server handler for message: {messageType} is ready");
//    }

//    public Opcode GetMessageType()
//    {
//        return _messageType;
//    }

//    public Task Execute(ITransportSender sender, long requestId, string requestJson)
//    {
//        _transportSender = sender;
//        RequestId = requestId;

//        var requestMsg = requestJson.FromJson<Q>();
//        return ExecuteInternal(requestMsg);
//    }

//    protected Task SendLastResponse<R>(R responseMessage) where R : MessageHeader
//    {
//        return _transportSender.SendAsync(responseMessage.BuildResponseMessage(RequestId));
//    }

//    protected Task SendContinuingResponse<R>(R responseMessage) where R : MessageHeader
//    {
//        return _transportSender.SendAsync(responseMessage.BuildContinuingResponseMessage(RequestId));
//    }

//    protected Task SendEvent<R>(R eventMessage) where R : MessageHeader
//    {
//        return _transportSender.SendAsync(eventMessage.BuildServerEventMessage());
//    }

//}
