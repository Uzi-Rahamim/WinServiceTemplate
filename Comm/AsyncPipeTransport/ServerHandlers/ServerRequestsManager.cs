using AsyncPipeTransport.Channel;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncPipeTransport.ServerHandlers
{
    public  class ServerRequestsManager : IServerRequestsManager
    {
        private readonly Dictionary<string, IRequestExecuterFactory> _executers = new Dictionary<string, IRequestExecuterFactory>();
        private readonly ILogger<ServerIncomingConnectionListener> _logger;
        private readonly IClientsManager _activeClients;
        public ServerRequestsManager(ILogger<ServerIncomingConnectionListener> logger, 
            IClientsManager activeClients, 
            IEnumerable<IRequestExecuterFactory> cmdList)
        {
            _logger = logger;
            _activeClients = activeClients;
            foreach (var cmd in cmdList)
            {
                _executers.Add(cmd.GetMessageType(), cmd);
            }
        }

        public async Task HandleClient(IServerChannel pipeServer, long clientId)
        {
            bool channelIsSecure = false;
            while (true)
            {
                string? clientMessage = await pipeServer.ReceiveAsync();
                if (clientMessage == null)
                    break;

                var frame = clientMessage.ExtractFrameHeaders();
                if (frame == null)
                {
                    _logger.LogInformation("Server {clientId} received an invalid request message", clientId);
                    continue;
                }

                //Expect the first request to be a security request
                if (!channelIsSecure)
                {
                    if (frame.IsOpenSessionFrame())
                    {
                        channelIsSecure = await Execute(pipeServer, FrameworkMessageTypes.OpenSession, frame.requestId, frame.payload, clientId);
                        if (!channelIsSecure)
                            break;

                        _activeClients.AddClient(clientId, pipeServer);
                    }
                    continue;
                }

                _ = Task.Run(async () =>
                {
                    await Execute(pipeServer, frame.msgType, frame.requestId, frame.payload, clientId);
                });

                _logger.LogInformation("Server {clientId} received request: {frame.requestId} ", clientId, frame.requestId);
            }
            _activeClients.RemoveClient(clientId);
        }
     
      

        private async Task<bool> Execute(IServerChannel pipeServer, string msgType, long requestId, string payload, long clientId)
        {
            var cmd = CreateExecuter(msgType, requestId, clientId);
            if (cmd == null)
            {
                _logger.LogInformation("Server {clientId} command handler not found {frame.requestId} ", clientId, requestId);
                return false;
            }
            return await cmd.Execute(pipeServer, requestId, payload);
        }


        private IRequestExecuter? CreateExecuter(string msgType, long requestId, long clientId)
        {
            if (!_executers.ContainsKey(msgType))
            {
                _logger.LogInformation("Server {clientId} command not found {frame.msgType}", clientId, requestId);
                return null;
            }
            return _executers[msgType].Create();
        }

    }
}
