using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Extensions;

namespace AsyncPipeTransport.Events
{
    public class EventToAction<T> : IEvent where T : MessageHeader
    {
        private Action<T> _action;
        public EventToAction(Action<T> action) => _action = action;
        public void Execute(FrameHeader frame)
        {
            _action(frame.payload.FromJson<T>());
        }
    }
}
