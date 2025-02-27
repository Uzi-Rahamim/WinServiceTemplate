using AsyncPipeTransport.Channel;
using AsyncPipeTransport.Exceptions;
using AsyncPipeTransport.Listeners;
using Microsoft.Extensions.Logging;

namespace AsyncPipeTransport.Executer
{

    public class ExecuterManager : IExecuterManager
    {
        private readonly Dictionary<string, IRequestExecuterFactory> _executers = new Dictionary<string, IRequestExecuterFactory>();
        private readonly ILogger<ServerIncomingConnectionListener> _logger;
        public ExecuterManager(ILogger<ServerIncomingConnectionListener> logger,
            IEnumerable<IRequestExecuterFactory> cmdList)
        {
            _logger = logger;
            foreach (var cmd in cmdList)
            {
                try
                {
                    _logger.LogInformation("Server load executer for {messageType}", cmd.GetMessageType());
                    _executers.Add(cmd.GetMessageType(), cmd);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Server load executer failed (Make sure Plugin_GetMessageType/GetMessageType is unique)");
                }
              
            }
        }

        public async Task<bool> Execute(IChannel pipeServer, string msgType, long requestId, string payload, long clientId)
        {
            var cmd = CreateExecuter(msgType, requestId, clientId);
            if (cmd == null)
            {
                _logger.LogInformation("Server {clientId} executer not found {frame.requestId} ", clientId, requestId);
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
