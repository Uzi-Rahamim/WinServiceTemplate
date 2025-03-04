using Intel.IntelConnect.IPC.Channel;
using Intel.IntelConnect.IPC.CommonTypes;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;
using System.Collections.Concurrent;

namespace Intel.IntelConnect.IPC.Events.Service
{ 
    public class EventDispatcher : IEventDispatcher
    {
        private readonly ConcurrentDictionary<string, EventTopicChannels> _topics = new ConcurrentDictionary<string, EventTopicChannels>();
        private readonly ILogger<EventDispatcher> _logger;
        private readonly AsyncSemaphore _asyncLock = new AsyncSemaphore(1);

        public EventDispatcher(ILogger<EventDispatcher> logger)
        {
            _logger = logger;
        }

        public async Task SafeRegisterForEventsAsync(Guid channelId, IChannelSender clientChannel, IEnumerable<string> topics, Func<IEnumerable<string>, Task> callback)
        {
            _logger.LogDebug("SafeRegisterForEventsAsync Enter {channelId}", channelId);
            using (await _asyncLock.EnterAsync())
            {
                var firstClientTopics = new List<string>();
                foreach (var topic in topics)
                {
                    _logger.LogInformation("Registering for topic {topic}", topic);
                    var topicChannels = _topics.GetOrAdd(topic, (topic) => new EventTopicChannels(_logger));
                    topicChannels.AddClient(channelId, clientChannel);
                    if (topicChannels.Count() == 1) //First client register
                        firstClientTopics.Add(topic);
                }
                await callback(firstClientTopics);
            }
            _logger.LogDebug("SafeRegisterForEventsAsync Done");
        }

        public async Task SafeUnregisterEventsAsync(Guid channelId, IEnumerable<string> topics, Func<IEnumerable<string>, Task> callback)
        {
            _logger.LogDebug("SafeUnregisterEventsAsync Enter {channelId}", channelId);
            using (await _asyncLock.EnterAsync())
            {
                var lastClientsTopics = new List<string>();
                foreach (var topic in topics)
                {
                    _logger.LogInformation("Unregister topic {topic}", topic);
                    if (_topics.TryGetValue(topic, out var topicChannels))
                    {
                        topicChannels.RemoveClient(channelId );
                        if (topicChannels.Count() == 0) //Last Cleint Unregister
                            lastClientsTopics.Add(topic);
                    }
                }

                await callback(lastClientsTopics);
            }
            _logger.LogDebug("SafeUnregisterEventsAsync Done");
        }

        public async Task SafeUnregisterAllEventsAsync(Guid clientId)
        {
            using (await _asyncLock.EnterAsync())
            {
                var topicChannels = _topics.Values.ToList();
                foreach (var topicChannel in topicChannels)
                {
                    topicChannel.RemoveClient(clientId);
                }
            }
        }

        public async Task<bool> DispatchEventAsync<R>(R eventMessage) where R : EventMessageHeader
        {
            if (_topics.TryGetValue(eventMessage.topic, out var topicChannels))
            {
                return await topicChannels.DispatchEventAsync(eventMessage);
            }
            else
            {
                _logger.LogInformation("No clients found - for {eventMessage.methodName} Event", eventMessage.topic);
            }
            return false;
        }
    }
}
