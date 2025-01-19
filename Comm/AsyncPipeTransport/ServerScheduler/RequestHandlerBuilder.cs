using AsyncPipeTransport.CommonTypes;

namespace AsyncPipeTransport.ServerScheduler
{
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
}
