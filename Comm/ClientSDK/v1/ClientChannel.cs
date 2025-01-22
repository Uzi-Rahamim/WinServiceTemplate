using AsyncPipeTransport.Channel;
using AsyncPipeTransport.ClientHandlers;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Events;
using CommTypes.Massages;
using CommunicationMessages;

namespace ClientSDK.v1
{
    public class ClientChannel : IDisposable
    {
        internal ClientResponseListener Listener { get => _clientResponseListener; }
        internal ClientEventHandler EventHandler { get => _clientEventHandler; }
        internal ClientRequestHandler RequestHandler { get => _clientRequestHnadler; }

        private readonly ClientResponseListener _clientResponseListener;
        private readonly ClientEventHandler _clientEventHandler;
        private readonly ClientRequestHandler _clientRequestHnadler;
        private readonly IClientChannel _channel;

        public ClientChannel()
        {
            _channel = new ClientPipeChannel(PipeApiConsts.PipeName);
            _clientEventHandler = new ClientEventHandler();
            _clientRequestHnadler = new ClientRequestHandler(new SequenceGenerator(), _channel);

            _clientResponseListener = new ClientResponseListener(
               _channel,
               _clientRequestHnadler,
               _clientEventHandler);
        }

        public Task<bool> Connect()
        {
            _clientResponseListener.Start();
            return SendSecurityMessage();
        }

        private async Task<bool> SendSecurityMessage()
        {
            var response = await _clientRequestHnadler.SendSecurityRequest<ResponseSecurityMessage, RequestSecurityMessage>(new RequestSecurityMessage(string.Empty));
            return response?.isValid ?? false;
        }

        public bool RegisterEvent(Opcode messageType, IEvent eventAction)
        {
            return _clientEventHandler.RegisterEvent(messageType, eventAction);
        }

        public void Dispose()
        {
            _clientResponseListener.Dispose();
        }
    }
}
