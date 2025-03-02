using Intel.IntelConnect.IPC.Channel;
using Intel.IntelConnect.IPC.Clients;
using Intel.IntelConnect.IPC.Events;
using Intel.IntelConnect.IPC.Executer;
using Intel.IntelConnect.IPC.Request;
using Microsoft.Extensions.Logging;

namespace Intel.IntelConnect.IPC.Listeners
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
        private readonly IEventDispatcher? _eventDispatcher;
        private readonly ILogger<ServerMessageListener> _logger;

        public ServerMessageListener(
            ILogger<ServerMessageListener> logger,
            IExecuterManager? executerManager,
            IEventDispatcher? eventDispatcher,
            IClientRequestManager? clientRequestHandler = null,
            IEventManager? clientEventHandler = null
            )
        {
            _logger = logger;
            _clientRequestHandler = clientRequestHandler;
            _clientEventHandler = clientEventHandler;
            _executerManager = executerManager;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<bool> StartAsync(CancellationToken cancellationToken, IServerChannel channel, TimeSpan timeout, long endpointId)
        {
            _logger.LogInformation("Server waiting for connection");
            // Wait for a client to connect
            await channel.WaitForConnectionAsync(cancellationToken);
            _logger.LogInformation("Server {clientId}  Client connected.", endpointId);

            if (channel == null)
                return false;

            _messageListener = new MessageListener(_logger, cancellationToken, channel, _clientRequestHandler, _clientEventHandler, _executerManager, _eventDispatcher);
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
