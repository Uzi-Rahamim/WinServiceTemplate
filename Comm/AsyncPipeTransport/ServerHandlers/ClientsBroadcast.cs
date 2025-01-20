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
    public interface IClientsBroadcast
    {
        public void AddClient(long clientId, IServerChannel client);
        public void RemoveClient(long clientId);
        public void BroadcastEvent<R>(R eventMessage) where R : MessageHeader;
    }

    public class ClientsBroadcast: IClientsBroadcast
    {

        private readonly ConcurrentDictionary<long, IServerChannel> _activeClients = new ConcurrentDictionary<long, IServerChannel>();
        private readonly ILogger<ClientsBroadcast> _logger;
        public ClientsBroadcast(ILogger<ClientsBroadcast> logger)
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
                    await client.SendAsync(eventMessage.BuildServerEventMessage());
                }
                catch (Exception)
                {
                    _logger.LogInformation("Server fail to send event");
                }
            }
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
