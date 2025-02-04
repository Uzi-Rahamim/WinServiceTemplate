using AsyncPipeTransport.Channel;
using AsyncPipeTransport.Clients;
using AsyncPipeTransport.Events;
using AsyncPipeTransport.Executer;
using AsyncPipeTransport.Request;
using Microsoft.Extensions.Logging;

namespace AsyncPipeTransport.Listeners
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
            _logger = logger;
            _clientRequestHandler = clientRequestHandler;
            _clientEventHandler = clientEventHandler;
            _executerManager = executerManager;
            _activeClients = activeClients;
        }

        public async Task<bool> StartAsync(CancellationToken cancellationToken, IServerChannel channel, TimeSpan timeout, long endpointId)
        {
            // Wait for a client to connect
            await channel.WaitForConnectionAsync(cancellationToken);
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
