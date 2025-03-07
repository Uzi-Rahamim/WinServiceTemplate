﻿using Intel.IntelConnect.IPC.Channel;
using Intel.IntelConnect.IPC.Utils;
using Microsoft.Extensions.Logging;


namespace Intel.IntelConnect.IPC.Listeners
{
    public class ServerIncomingConnectionListener
    {
        private readonly ILogger<ServerIncomingConnectionListener> _logger;
        private readonly IServerMessageListener _serverMessageListener;
        private readonly IServerChannelFactory _serverChannelFactory;
        private readonly ISequenceGenerator _clientIdGenerator;
        public ServerIncomingConnectionListener(ILogger<ServerIncomingConnectionListener> logger,
                                     IServerChannelFactory serverChannelFactory,
                                     IServerMessageListener serverMessageListener,
                                     ISequenceGenerator clientIdGenerator)
        {
            _logger = logger;
            _serverChannelFactory = serverChannelFactory;
            _serverMessageListener = serverMessageListener;
            _clientIdGenerator = clientIdGenerator;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => StartListenAsync(cancellationToken));
        }

        private async Task StartListenAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var clientId = _clientIdGenerator.GetNextId();

                    // Create a NamedPipeServerStream to listen for connections
                    IServerChannel pipeServer = _serverChannelFactory.Create();
                    _logger.LogInformation("Server {ChannelId} Waiting for a client {clientId} to connect...", pipeServer.ChannelId, clientId);

                    // Wait for a client 
                    if (!await _serverMessageListener.StartAsync(cancellationToken, pipeServer, TimeSpan.FromSeconds(10), clientId))
                        return;

                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogDebug("Listener was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ServerIncomingConnectionListener");
                throw;
            }
            finally
            {
                _logger.LogInformation("Listener was stopped");

            }
        }
    }
}