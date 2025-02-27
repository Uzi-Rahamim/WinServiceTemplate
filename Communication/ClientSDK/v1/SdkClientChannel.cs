using AsyncPipeTransport.Channel;
using AsyncPipeTransport.Events;
using AsyncPipeTransport.Listeners;
using AsyncPipeTransport.Request;
using AsyncPipeTransport.Utils;
using CommTypes.Consts;
using CommTypes.Massages;
using Microsoft.Extensions.Logging;
using WinServicePluginCommon.Sdk.Types;

namespace ClientSDK.v1
{
    public class SdkClientChannel : ISDKClientChannel, IDisposable
    {
        internal IClientMessageListener Listener { get => _clientMessageListener; }
        public IEventManager EventHandler { get => _clientEventHandler; }
        public IClientRequestManager RequestHandler { get => _clientRequestHnadler; }

        private readonly IClientMessageListener _clientMessageListener;
        private readonly IEventManager _clientEventHandler;
        private readonly IClientRequestManager _clientRequestHnadler;
        private readonly IClientChannel _channel;
        private readonly CancellationToken _cancellationToken;

        public event Action? OnDisconnect;

        public SdkClientChannel(ILoggerFactory loggerFactory)
        {

            var logger = loggerFactory.CreateLogger<SdkClientChannel>();
            var cancellationTokenSource = new CancellationTokenSource();

            _cancellationToken = cancellationTokenSource.Token;
            ClientRequestFactory _requestFactory = (newRequestId, payload) => new ClientRequest(newRequestId, payload, _cancellationToken);
            _channel = new ClientPipeChannel(loggerFactory.CreateLogger<ClientPipeChannel>(), PipeApiConsts.PipeName);
            _clientEventHandler = new EventManager();
            _clientRequestHnadler = new ClientRequestsManager(new SequenceGenerator(), _channel, _requestFactory);

            _channel.OnDisconnect += () =>
            {

                logger.LogInformation("Channel Disconnect");
                cancellationTokenSource.Cancel();
                OnDisconnectInternal();
            };

            //Create listener
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


        private void OnDisconnectInternal()
        {
            var disconnectEvent = OnDisconnect;
            disconnectEvent?.Invoke();
        }

        public void Dispose()
        {
            _clientMessageListener.Dispose();
        }
    }
}
