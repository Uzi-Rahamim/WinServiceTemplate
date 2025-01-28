using AsyncPipeTransport.Channel;
using AsyncPipeTransport.CommonTypes;
using Microsoft.Extensions.Logging;


namespace AsyncPipeTransport.Listeners
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

        public Task Start(CancellationToken cancellationToken)
        {
            return Task.Run(() => StartListen(cancellationToken));
        }

        private async Task StartListen(CancellationToken cancellationToken)
        {
            while (true)
            {
                ManualResetEvent signal = new ManualResetEvent(false);

                var clientId = _clientIdGenerator.GetNextId();

                // Create a NamedPipeServerStream to listen for connections
                IServerChannel pipeServer = _serverChannelFactory.Create();
                _logger.LogInformation("Server {clientId}  Waiting for a client to connect...", clientId);

                // Wait for a client 
                if (!await _serverMessageListener.StartAsync(cancellationToken, pipeServer, TimeSpan.FromSeconds(10), clientId))
                    return;

            }
        }
    }

}