using AsyncPipeTransport.CommonTypes;

namespace AsyncPipeTransport.ServerHandlers
{
    public class RequestCommandFactory : IRequestCommandFactory
    {
        Func<IRequestCommand> _factory;
        private readonly Opcode _messageType;

        public RequestCommandFactory(Opcode messageType, Func<IRequestCommand> factory)
        {
            _factory = factory;
            _messageType = messageType;
        }
        public IRequestCommand Create()
        {
            return _factory();
        }
        public Opcode GetMessageType()
        {
            return _messageType;

        }
    }
}
