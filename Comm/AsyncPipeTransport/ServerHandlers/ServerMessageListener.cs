using AsyncPipeTransport.Channel;
using AsyncPipeTransport.ClientHandlers;
using Microsoft.Extensions.Logging;

namespace AsyncPipeTransport.ServerHandlers
{
    public interface IServerMessageListener : IDisposable
    {
        Task<bool> StartAsync(CancellationToken cancellationToken, IServerChannel channel, TimeSpan timeout, long endpointId);
    }

    public class ServerMessageListener : IServerMessageListener
    {
        private MessageListener? _messageListener;
        private readonly IClientEventManager? _clientEventHandler;
        private readonly IClientRequestManager? _clientRequestHandler;
        private readonly IExecuterManager? _executerManager;
        private readonly IClientsManager? _activeClients;
        private readonly ILogger<ServerMessageListener> _logger;

        public ServerMessageListener(
            ILogger<ServerMessageListener> logger,
            IExecuterManager? executerManager,
            IClientsManager? activeClients,
            IClientRequestManager? clientRequestHandler = null,
            IClientEventManager? clientEventHandler = null
            )
        {
            this._logger = logger;
            this._clientRequestHandler = clientRequestHandler;
            this._clientEventHandler = clientEventHandler;
            this._executerManager = executerManager;
            this._activeClients = activeClients;
        }

        public async Task<bool> StartAsync(CancellationToken cancellationToken, IServerChannel channel, TimeSpan timeout, long endpointId)
        {
            // Wait for a client to connect
            await channel.WaitForConnectionAsync();
            _logger.LogInformation("Server {clientId}  Client connected.", endpointId);

            if (channel == null)
                return false;

            _messageListener = new MessageListener(_logger, cancellationToken, channel, _clientRequestHandler, _clientEventHandler, _executerManager, _activeClients);
            _messageListener.StartReadMessageLoop(timeout, endpointId);

            return true;
        }

        public void Dispose()
        {   
            _messageListener?.Dispose();
        }
    }
}
