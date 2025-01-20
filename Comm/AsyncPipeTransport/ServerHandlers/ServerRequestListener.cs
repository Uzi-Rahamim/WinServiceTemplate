using AsyncPipeTransport.Channel;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Extensions;
using Microsoft.Extensions.Logging;


namespace AsyncPipeTransport.ServerHandlers
{
    public class ServerRequestListener
    {
        private readonly ILogger<ServerRequestListener> _logger;
        private readonly IServerRequestHandler _serverRequestHandler;
        private readonly ISequenceGenerator _clientIdGenerator;
        public ServerRequestListener(ILogger<ServerRequestListener> logger, 
                                     IServerRequestHandler serverRequestHandler,
                                     ISequenceGenerator clientIdGenerator)
        {
            _logger = logger;
            _serverRequestHandler = serverRequestHandler;
            _clientIdGenerator = clientIdGenerator;
        }

        public Task Start(string pipeName)
        {
            return Task.Run(() => StartListen(pipeName));
        }

        private void StartListen(string pipeName)
        {
            while (true)
            {
                ManualResetEvent signal = new ManualResetEvent(false);
                _ = Task.Run(async () =>
                {
                    var clientId = _clientIdGenerator.GetNextId();
                    // Create a NamedPipeServerStream to listen for connections
                    using (IServerChannel pipeServer = new ServerPipeChannel(pipeName))
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