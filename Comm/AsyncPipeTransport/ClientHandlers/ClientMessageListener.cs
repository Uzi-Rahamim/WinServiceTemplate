using AsyncPipeTransport.Channel;
using AsyncPipeTransport.ServerHandlers;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System.Threading.Channels;

namespace AsyncPipeTransport.ClientHandlers
{
    public interface IClientMessageListener: IDisposable
    {
        Task<bool> StartAsync(CancellationToken cancellationToken, TimeSpan timeout, long endpointId = 0);
    }

    public class ClientMessageListener : IClientMessageListener
    {
        private MessageListener? _messageListener;
        private readonly IClientChannel _channel;
        private readonly IClientEventManager? _clientEventHandler;
        private readonly IClientRequestManager? _clientRequestHandler;
        private readonly IExecuterManager? _executerManager;
        private readonly IClientsManager? _activeClients;
        private readonly ILogger<ClientMessageListener> _logger;

    
        public ClientMessageListener(
            ILogger<ClientMessageListener> logger,
            IClientChannel channel,
            IClientRequestManager? clientRequestHandler,
            IClientEventManager? clientEventHandler,
            IExecuterManager? executerManager = null,
            IClientsManager? activeClients=null)
        {
            this._logger = logger;
            this._channel = channel;
            this._clientRequestHandler = clientRequestHandler;
            this._clientEventHandler = clientEventHandler;
            this._executerManager = executerManager;
            this._activeClients = activeClients;
            
        }

        public void Dispose()
        {
            _messageListener?.Dispose();
        }

        public async Task<bool> StartAsync(CancellationToken cancellationToken, TimeSpan timeout, long endpointId = 0)
        {
            Console.WriteLine("StartAsync");
            var channel = _channel;
            try
            {
                await channel.ConnectAsync(timeout);
            }
            catch (TimeoutException)
            {
                return false;
            }

            _messageListener = new MessageListener(_logger, cancellationToken, channel, _clientRequestHandler, _clientEventHandler, _executerManager, _activeClients);
            _messageListener.StartReadMessageLoop(timeout, endpointId);
            return true;
        }

    }
}
