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
        public ClientEventManager EventHandler { get => _clientEventHandler; }
        public ClientRequestsManager RequestHandler { get => _clientRequestHnadler; }

        private readonly IClientMessageListener _clientMessageListener;
        private readonly ClientEventManager _clientEventHandler;
        private readonly ClientRequestsManager _clientRequestHnadler;
        private readonly IClientChannel _channel;

        public ClientChannel()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                //builder.AddConsole(); // This will log to the console
            });
            var logger = loggerFactory.CreateLogger<ClientMessageListener>();

            _channel = new ClientPipeChannel(PipeApiConsts.PipeName);
            _clientEventHandler = new ClientEventManager();
            _clientRequestHnadler = new ClientRequestsManager(new SequenceGenerator(), _channel);

            _clientMessageListener = new ClientMessageListener(
                 logger,
               _channel,
               _clientRequestHnadler,
               _clientEventHandler);
        }

        public Task<bool> Connect()
        {
            CancellationToken cancellationToken = new CancellationToken();
            var success = _clientMessageListener.StartAsync(
                cancellationToken,
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
