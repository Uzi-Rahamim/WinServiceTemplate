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
    public  class ServerRequestHandler : IServerRequestHandler
    {
        private readonly Dictionary<Opcode, IRequestCommandFactory> _commands = new Dictionary<Opcode, IRequestCommandFactory>();
        private readonly ILogger<ServerIncomingConnectionListener> _logger;
        private readonly IClientsBroadcast _activeClients;
        public ServerRequestHandler(ILogger<ServerIncomingConnectionListener> logger, 
            IClientsBroadcast activeClients, 
            IEnumerable<IRequestCommandFactory> cmdList)
        {
            _logger = logger;
            _activeClients = activeClients;
            foreach (var cmd in cmdList)
            {
                _commands.Add(cmd.GetMessageType(), cmd);
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
                    channelIsSecure = await ExecuteCommand(pipeServer, Opcode.OpenSession , frame.requestId, frame.payload, clientId);
                    if (!channelIsSecure)
                        break;
                    _activeClients.AddClient(clientId, pipeServer);
                    continue;
                }


                _ = Task.Run(async () =>
                {
                    await ExecuteCommand(pipeServer, frame.msgType, frame.requestId, frame.payload, clientId);
                });

                _logger.LogInformation("Server {clientId} received request: {frame.requestId} ", clientId, frame.requestId);
            }
            _activeClients.RemoveClient(clientId);
        }
     
      

        private async Task<bool> ExecuteCommand(IServerChannel pipeServer, Opcode msgType, long requestId, string payload, long clientId)
        {
            var cmd = GetCommand(msgType, requestId, clientId);
            if (cmd == null)
            {
                _logger.LogInformation("Server {clientId} command handler not found {frame.requestId} ", clientId, requestId);
                return false;
            }
            return await cmd.Execute(pipeServer, requestId, payload);
        }


        private IRequestCommand? GetCommand(Opcode msgType, long requestId, long clientId)
        {
            if (!_commands.ContainsKey(msgType))
            {
                _logger.LogInformation("Server {clientId} command not found {frame.msgType}", clientId, requestId);
                return null;
            }
            return _commands[msgType].Create();
        }


    }
}
