namespace Intel.IntelConnect.IPC.Executer
{
    public class RequestExecuterFactory : IRequestExecuterFactory
    {
        private readonly Func<IRequestExecuter> _factory;
        private readonly string _messageType;


        public RequestExecuterFactory(string messageType, Func<IRequestExecuter> factory)
        {
            _factory = factory;
            _messageType = messageType;
        }

        public IRequestExecuter Create()
        {
            return _factory();
        }

        public string GetMessageType()
        {
            return _messageType;
        }
    }
}
