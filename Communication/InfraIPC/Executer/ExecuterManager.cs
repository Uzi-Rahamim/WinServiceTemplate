using Intel.IntelConnect.IPC.Channel;
using Intel.IntelConnect.IPC.Exceptions;
using Intel.IntelConnect.IPC.Listeners;
using Microsoft.Extensions.Logging;

namespace Intel.IntelConnect.IPC.Executer
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
                    _logger.LogInformation("Server load executer method {messageType}", cmd.GetMessageType());
                    _executers.Add(cmd.GetMessageType(), cmd);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Server load executer method failed (Make sure Plugin_GetMethodName/GetMessageType is unique)");
                }
              
            }
        }

        public async Task<bool> ExecuteAsync(IChannel pipeServer, string methodName, long requestId, string payload)
        {
            var cmd = CreateExecuter(methodName, requestId, pipeServer.ChannelId);
            if (cmd == null)
            {
                _logger.LogInformation("Server {clientId} executer not found {frame.requestId} ", pipeServer.ChannelId, requestId);
                return false;
            }
            return await cmd.ExecuteAsync(pipeServer, requestId, payload);
        }


        private IRequestExecuter? CreateExecuter(string methodName, long requestId, Guid channelId)
        {
            if (!_executers.ContainsKey(methodName))
            {
                _logger.LogInformation("Server {clientId} command not found {frame.methodName}", channelId, requestId);
                return null;
            }
            return _executers[methodName].Create();
        }

    }
}
