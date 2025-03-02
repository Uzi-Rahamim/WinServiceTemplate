using Intel.IntelConnect.IPC.Channel;
using Intel.IntelConnect.IPC.CommonTypes;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Intel.IntelConnect.IPC.Clients
{
    public interface IEventDispatcher
    {
        public void RegisterForEvents(Guid clientId, IChannelSender clientChannel, IEnumerable<string> topics);
        public void UnregisterAllEvents(Guid clientId);
        public void UnregisterEvents(Guid clientId, IEnumerable<string> topics);

        public Task<bool> DispatchEvent<R>(R eventMessage) where R : MessageHeader;
    }

    public class EventDispatcher : IEventDispatcher
    {
        private readonly ConcurrentDictionary<string, EventTopicChannels> _topics = new ConcurrentDictionary<string, EventTopicChannels>();
        private readonly ILogger<EventDispatcher> _logger;
        public EventDispatcher(ILogger<EventDispatcher> logger)
        {
            _logger = logger;
        }


        public void RegisterForEvents(Guid clientId, IChannelSender clientChannel, IEnumerable<string> topics)
        {
            foreach (var topic in topics)
            {
                _logger.LogInformation($"Registering for topic {topic}");
                var topicChannels = _topics.GetOrAdd(topic, (topic)=> new EventTopicChannels(_logger));
                topicChannels.AddClient(clientId,clientChannel);
            }
        }

        public void UnregisterAllEvents(Guid clientId)
        {
            var topicChannels = _topics.Values.ToList();
            foreach (var topicChannel in topicChannels)
            {
                topicChannel.RemoveClient(clientId);
            }
        }

        public void UnregisterEvents(Guid clientId, IEnumerable<string> topics)
        {
            foreach (var topic in topics)
            {
                _logger.LogInformation($"Unregister topic {topic}");
                if (_topics.TryGetValue(topic,out var topicChannels))
                {
                    topicChannels.RemoveClient(clientId);
                }
            }
        }

        public async Task<bool> DispatchEvent<R>(R eventMessage) where R : MessageHeader
        {
            if (_topics.TryGetValue(eventMessage.msgType, out var topicChannels))
            {
                return await topicChannels.DispatchEvent(eventMessage);    
            }
            else
            {
                _logger.LogInformation($"No clients found - for {eventMessage.msgType} Event" ,eventMessage.msgType);
            }
            return false;
        }
    }
}
