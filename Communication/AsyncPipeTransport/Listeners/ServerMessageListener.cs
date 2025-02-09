using AsyncPipeTransport.Channel;
using AsyncPipeTransport.Clients;
using AsyncPipeTransport.Events;
using AsyncPipeTransport.Executer;
using AsyncPipeTransport.Request;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace AsyncPipeTransport.Listeners
{
    public interface IServerMessageListener : IDisposable
    {
        Task<bool> StartAsync(CancellationToken cancellationToken, IServerChannel channel, TimeSpan timeout, long endpointId);
    }

    public class ServerMessageListener : IServerMessageListener
    {
        private MessageListener? _messageListener;
        private readonly IEventManager? _clientEventHandler;
        private readonly IClientRequestManager? _clientRequestHandler;
        private readonly IExecuterManager? _executerManager;
        private readonly IClientsManager? _activeClients;
        private readonly ILogger<ServerMessageListener> _logger;

        public ServerMessageListener(
            ILogger<ServerMessageListener> logger,
            IExecuterManager? executerManager,
            IClientsManager? activeClients,
            IClientRequestManager? clientRequestHandler = null,
            IEventManager? clientEventHandler = null
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
            _logger.LogInformation("Server waiting for connection");
            // Wait for a client to connect
            await channel.WaitForConnectionAsync(cancellationToken);
            _logger.LogInformation("Server {clientId}  Client connected.", endpointId);

            if (channel == null)
                return false;

            _messageListener = new MessageListener(_logger, cancellationToken, channel, _clientRequestHandler, _clientEventHandler, _executerManager, _activeClients);
            _messageListener.OnDisconnect += () =>
            {
                _logger.LogInformation("Server {clientId}  Client disconnected.", endpointId);
                _messageListener.Dispose();
            };
            _messageListener.StartListen(timeout, endpointId);

            return true;
        }

        public void Dispose()
        {
            _messageListener?.Dispose();
        }
    }
}
