using AsyncPipeTransport.Channel;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Extensions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace AsyncPipeTransport.Clients
{
    public interface IClientsManager
    {
        public void AddClient(long clientId, IServerChannel client);
        public void RemoveClient(long clientId);
        void BroadcastEvent<R>(R eventMessage) where R : MessageHeader;
        IServerChannel? TryGetClient(long clientId);
    }

    public class ClientsManager : IClientsManager
    {

        private readonly ConcurrentDictionary<long, IServerChannel> _activeClients = new ConcurrentDictionary<long, IServerChannel>();
        private readonly ILogger<ClientsManager> _logger;
        public ClientsManager(ILogger<ClientsManager> logger)
        {
            _logger = logger;
        }

        public async void BroadcastEvent<R>(R eventMessage) where R : MessageHeader
        {
            var clients = _activeClients.Values.ToList();
            foreach (var client in clients)
            {
                try
                {
                    _logger.LogDebug("Server sending event");
                    await client.SendAsync(eventMessage.BuildServerEventMessage(), CancellationToken.None);
                }
                catch (Exception)
                {
                    _logger.LogInformation("Server fail to send event");
                }
            }
        }

        public IServerChannel? TryGetClient(long clientId)
        {
            _activeClients.TryGetValue(clientId, out var client);
            return client;
        }

        public void AddClient(long clientId, IServerChannel client)
        {
            _activeClients.TryAdd(clientId, client);
        }

        public void RemoveClient(long clientId)
        {
            _activeClients.TryRemove(clientId, out _);
        }
    }
}
