using AsyncPipeTransport.Channel;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Events;
using AsyncPipeTransport.Listeners;
using AsyncPipeTransport.Request;
using CommTypes.Consts;
using CommTypes.Massages;
using Microsoft.Extensions.Logging;

namespace ClientSDK.v1
{
    public class ClientChannel : IDisposable
    {
        internal IClientMessageListener Listener { get => _clientMessageListener; }
        public EventManager EventHandler { get => _clientEventHandler; }
        public ClientRequestsManager RequestHandler { get => _clientRequestHnadler; }

        private readonly IClientMessageListener _clientMessageListener;
        private readonly EventManager _clientEventHandler;
        private readonly ClientRequestsManager _clientRequestHnadler;
        private readonly IClientChannel _channel;
        private readonly CancellationToken _cancellationToken;

        public ClientChannel(ILoggerFactory loggerFactory)
        {
            
            var logger = loggerFactory.CreateLogger<ClientChannel>();
            var cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = cancellationTokenSource.Token;
            ClientRequestFactory _requestFactory = (newRequestId, payload) => new ClientRequest(newRequestId, payload, _cancellationToken);
            _channel = new ClientPipeChannel(loggerFactory.CreateLogger<ClientPipeChannel>(), PipeApiConsts.PipeName);
            _clientEventHandler = new EventManager();
            _clientRequestHnadler = new ClientRequestsManager(new SequenceGenerator(), _channel, _requestFactory);
            _clientMessageListener = new ClientMessageListener(
                 loggerFactory.CreateLogger<ClientMessageListener>(),
               _channel,
               new MessageListener(
               loggerFactory.CreateLogger<MessageListener>(),
               _cancellationToken, 
               _channel, 
               _clientRequestHnadler, 
               _clientEventHandler, 
               null, 
               null));

            _channel.OnDisconnect += () =>
            {

                logger.LogInformation("Channel Disconnect");
                cancellationTokenSource.Cancel();
            };
        }

        public Task<bool> Connect()
        {
            var success = _clientMessageListener.StartAsync(
                _cancellationToken,
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
            _clientMessageListener.Dispose();
        }
    }
}
