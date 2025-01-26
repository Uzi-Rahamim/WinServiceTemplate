using AsyncPipeTransport.Channel;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Extensions;
using Microsoft.Extensions.Logging;


namespace AsyncPipeTransport.ServerHandlers
{
    public class ServerIncomingConnectionListener
    {
        private readonly ILogger<ServerIncomingConnectionListener> _logger;
        private readonly IServerRequestsManager _serverRequestHandler;
        private readonly IServerChannelFactory _serverChannelFactory;
        private readonly ISequenceGenerator _clientIdGenerator;
        public ServerIncomingConnectionListener(ILogger<ServerIncomingConnectionListener> logger,
                                     IServerChannelFactory serverChannelFactory,
                                     IServerRequestsManager serverRequestHandler,
                                     ISequenceGenerator clientIdGenerator)
        {
            _logger = logger;
            _serverChannelFactory = serverChannelFactory;
            _serverRequestHandler = serverRequestHandler;
            _clientIdGenerator = clientIdGenerator;
        }

        public Task Start()
        {
            return Task.Run(() => StartListen());
        }

        private void StartListen()
        {
            while (true)
            {
                ManualResetEvent signal = new ManualResetEvent(false);
                _ = Task.Run(async () =>
                {
                    var clientId = _clientIdGenerator.GetNextId();
                    // Create a NamedPipeServerStream to listen for connections
                    using (IServerChannel pipeServer = _serverChannelFactory.Create())
                    {
                        _logger.LogInformation("Server {clientId}  Waiting for a client to connect...", clientId);

                        // Wait for a client to connect
                        pipeServer.WaitForConnection();
                        _logger.LogInformation("Server {clientId}  Client connected.", clientId);

                        if (pipeServer == null)
                            return;

                        signal.Set();
                        await _serverRequestHandler.HandleClient(pipeServer, clientId);

                        _logger.LogInformation("Server {clientId}  Exit.", clientId);
                    }
                });
                signal.WaitOne(); // Block until signaled
            }
        }
    }

}