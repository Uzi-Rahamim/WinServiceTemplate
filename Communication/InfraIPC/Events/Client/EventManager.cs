using Intel.IntelConnect.IPC.CommonTypes;
using System.Collections.Concurrent;

namespace Intel.IntelConnect.IPC.Events.Client
{
    public class EventManager : IEventManager
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
            if (!events.ContainsKey(frame.methodName))
            {
                return;
            }
            var cmd = events[frame.methodName];
            cmd.Execute(frame);
        }
    }
}
