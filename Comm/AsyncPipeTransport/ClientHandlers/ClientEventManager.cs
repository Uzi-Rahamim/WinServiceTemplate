using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Events;
using System.Collections.Concurrent;

namespace AsyncPipeTransport.ClientHandlers
{
    public class ClientEventManager: IClientEventManager
    {
        private readonly ConcurrentDictionary<string, IEvent> events = new ConcurrentDictionary<string, IEvent>();

        public bool RegisterEvent(string messageType, IEvent eventAction)
        {
            return events.TryAdd(messageType, eventAction);
        }

        public bool UnregisterEvent(string messageType)
        {
            return events.TryRemove(messageType, out _);
        }

        public void HandleEvent(FrameHeader frame)
        {
            if (!events.ContainsKey(frame.msgType))
            { 
                return;
            }
            var cmd = events[frame.msgType];
            cmd.Execute(frame);
        }
    }
}
