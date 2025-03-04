using Intel.IntelConnect.IPC.Channel;
using Intel.IntelConnect.IPC.Executer;
using Intel.IntelConnect.IPC.Request;
using Intel.IntelConnect.IPC.Events.Client;
using Intel.IntelConnect.IPC.Events.Service;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Intel.IntelConnect.IPC.Listeners
{
    public interface IServerMessageListener 
    {
        Task<bool> StartAsync(CancellationToken cancellationToken, IServerChannel channel, TimeSpan timeout, long endpointId);
    }

    public class ServerMessageListener : IServerMessageListener
    {
        private readonly IEventManager? _clientEventHandler;
        private readonly IClientRequestManager? _clientRequestHandler;
        private readonly IExecuterManager? _executerManager;
        private readonly IEventDispatcher? _eventDispatcher;
        private readonly ILogger<ServerMessageListener> _logger;
        private readonly ConcurrentDictionary<Guid,MessageListener> _messageListeners = new ConcurrentDictionary<Guid, MessageListener>();

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
            _logger.LogInformation("Server waiting for connection {ChannelId}", channel.ChannelId);
            // Wait for a client to connect
            await channel.WaitForConnectionAsync(cancellationToken);
            _logger.LogInformation("Server {clientId} {ChannelId} Client connected.", endpointId, channel.ChannelId);

            if (channel == null)
                return false;

            var messageListener = new MessageListener(_logger, cancellationToken, channel, _clientRequestHandler, _clientEventHandler, _executerManager, _eventDispatcher);
            messageListener.OnDisconnect += () =>
            {
                _logger.LogInformation("Server {clientId} {ChannelId} Client disconnected.", endpointId, channel.ChannelId);
                _messageListeners.TryRemove(channel.ChannelId,out _);
                messageListener.Dispose();
            };
            _messageListeners.TryAdd(channel.ChannelId, messageListener);
            messageListener.StartListen(timeout);

            return true;
        }

    }
}
