using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Events;
using System.Collections.Concurrent;

namespace AsyncPipeTransport.ClientHandlers
{
    public class ClientEventHandler: IClientEventHandler
    {
        private readonly ConcurrentDictionary<Opcode, IEvent> events = new ConcurrentDictionary<Opcode, IEvent>();

        public bool RegisterEvent(Opcode messageType, IEvent eventAction)
        {
            return events.TryAdd(messageType, eventAction);
        }

        public bool UnregisterEvent(Opcode messageType)
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
