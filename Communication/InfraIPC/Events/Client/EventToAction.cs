using Intel.IntelConnect.IPC.CommonTypes;
using Intel.IntelConnect.IPC.Extensions;

namespace Intel.IntelConnect.IPC.Events.Client
{
    public class EventToAction<T> : IEvent where T : MessageHeader
    {
        Action<T> _action;
        public EventToAction(Action<T> action) => _action = action;
        public void Execute(FrameHeader frame)
        {
            _action(frame.payload.FromJson<T>());
        }
    }
}
