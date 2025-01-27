using AsyncPipeTransport.Channel;
using AsyncPipeTransport.ClientHandlers;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Events;
using CommTypes.Massages;
using CommunicationMessages;
using System.Threading;

namespace ClientSDK.v1
{
    public class ClientChannel : IDisposable
    {
        internal MessageListener Listener { get => _clientResponseListener; }
        internal ClientEventManager EventHandler { get => _clientEventHandler; }
        internal ClientRequestsManager RequestHandler { get => _clientRequestHnadler; }

        private readonly MessageListener _clientResponseListener;
        private readonly ClientEventManager _clientEventHandler;
        private readonly ClientRequestsManager _clientRequestHnadler;
        private readonly IClientChannel _channel;

        public ClientChannel()
        {
            _channel = new ClientPipeChannel(PipeApiConsts.PipeName);
            _clientEventHandler = new ClientEventManager();
            _clientRequestHnadler = new ClientRequestsManager(new SequenceGenerator(), _channel);

            _clientResponseListener = new MessageListener(
               _channel,
               _clientRequestHnadler,
               _clientEventHandler);
        }

        public Task<bool> Connect()
        {
            //Blocking call
            var success = _clientResponseListener.StartAsync(
                TimeSpan.FromSeconds(PipeApiConsts.ConnectTimeoutInSec)).Result;
            if (success)
            {
                return SendSecurityMessage();
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        private async Task<bool> SendSecurityMessage()
        {
            var response = await _clientRequestHnadler.SendOpenSessionRequest<ResponseSecurityMessage, RequestSecurityMessage>(new RequestSecurityMessage(string.Empty));
            return response?.isValid ?? false;
        }

        public bool RegisterEvent(string messageType, IEvent eventAction)
        {
            return _clientEventHandler.RegisterEvent(messageType, eventAction);
        }

        public void Dispose()
        {
            _clientResponseListener.Dispose();
        }
    }
}
