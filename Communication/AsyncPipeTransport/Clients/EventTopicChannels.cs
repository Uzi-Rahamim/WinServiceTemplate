using AsyncPipeTransport.Channel;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Extensions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace AsyncPipeTransport.Clients
{
    class EventTopicChannels
    {
        private readonly ConcurrentDictionary<Guid, IChannelSender> _clients = new ConcurrentDictionary<Guid, IChannelSender>();
        private readonly ILogger<EventDispatcher> _logger;

        public EventTopicChannels(ILogger<EventDispatcher> logger)
        {
            _logger = logger;
        }

        public void AddClient(Guid clientId, IChannelSender client)
        {
            _clients.TryAdd(clientId, client);
        }

        public void RemoveClient(Guid clientId)
        {
            _clients.TryRemove(clientId, out _);
        }

        public async Task<bool> DispatchEvent<R>(R eventMessage) where R : MessageHeader
        {
            var channelsKeys = _clients.ToList();
            if (channelsKeys.Count == 0)
            {
                _logger.LogInformation("DispatchEvent - no clients connected");
                return false;
            }
            foreach (var keys in channelsKeys)
            {
                var channel = keys.Value;
                var channelId = keys.Key;
                try
                {
                    _logger.LogDebug("DispatchEvent for client {channelId}", channelId);

                    if (channel.IsConnected())
                    {
                        await channel.SendAsync(eventMessage.BuildServerEventMessage(), CancellationToken.None);
                    }
                    else
                    {
                        _logger.LogInformation("DispatchEvent Remove - client is not connected  {channelId}", channelId);
                        RemoveClient(channelId);
                    }
                }
                catch (TaskCanceledException)
                {
                    _logger.LogWarning("DispatchEvent - operation was aborted");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Server fail to send event");
                }
            }
            return true;
        }
    }
}
