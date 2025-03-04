using Intel.IntelConnect.IPC.CommonTypes;
using System.Collections.Concurrent;

namespace Intel.IntelConnect.IPC.Events.Client
{
    public class EventManager : IEventManager
    {
        private readonly ConcurrentDictionary<string, IEvent> events = new ConcurrentDictionary<string, IEvent>();

        public bool RegisterEvent(string topic, IEvent eventAction)
        {
            return events.TryAdd(topic, eventAction);
        }

        public bool UnregisterEvent(string topic)
        {
            return events.TryRemove(topic, out _);
        }

        public void HandleEvent(FrameHeader frame)
        {
            Console.WriteLine($"HandleEvent {frame.methodName}");
            if (!events.ContainsKey(frame.methodName))
            {
                return;
            }
            var cmd = events[frame.methodName];
            cmd.Execute(frame);
        }
    }
}
