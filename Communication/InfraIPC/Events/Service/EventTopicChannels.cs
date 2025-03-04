using Intel.IntelConnect.IPC.Channel;
using Intel.IntelConnect.IPC.CommonTypes;
using Intel.IntelConnect.IPC.Extensions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Intel.IntelConnect.IPC.Events.Service
{
    class EventTopicChannels
    {
        private readonly ConcurrentDictionary<Guid, IChannelSender> _clients = new ConcurrentDictionary<Guid, IChannelSender>();
        private readonly ILogger<EventDispatcher> _logger;

        public EventTopicChannels(ILogger<EventDispatcher> logger)
        {
            _logger = logger;
        }

        public void AddClient(Guid channelId, IChannelSender client)
        {
            _clients.TryAdd(channelId, client);
        }

        public int Count()
        {
            return _clients.Count();
        }

        public void RemoveClient(Guid channelId)
        {
            _clients.TryRemove(channelId, out _);
        }

        public async Task<bool> DispatchEventAsync<R>(R eventMessage) where R : MessageHeader
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
                    _logger.LogWarning("DispatchEvent - operation was aborted {channelId}", channelId);
                    RemoveClient(channelId);
                }
                catch (IOException ex)
                {
                    _logger.LogError(ex, "DispatchEvent - failed to send event , pipe is broken {channelId}", channelId);
                    RemoveClient(channelId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "DispatchEvent - failed to send event {channelId}", channelId);
                    RemoveClient(channelId);
                }
            }
            return true;
        }
    }
}
