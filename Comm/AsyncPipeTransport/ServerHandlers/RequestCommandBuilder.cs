using AsyncPipeTransport.CommonTypes;

namespace AsyncPipeTransport.ServerHandlers
{
    public class RequestCommandBuilder : IRequestCommandBuilder
    {
        Func<IRequestCommand> _factory;
        private readonly Opcode _messageType;

        public RequestCommandBuilder(Opcode messageType, Func<IRequestCommand> factory)
        {
            _factory = factory;
            _messageType = messageType;
        }
        public IRequestCommand Build()
        {
            return _factory();
        }
        public Opcode GetMessageType()
        {
            return _messageType;

        }
    }
}
