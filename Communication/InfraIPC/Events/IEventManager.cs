using Intel.IntelConnect.IPC.CommonTypes;

namespace Intel.IntelConnect.IPC.Events
{
    public interface IEventManager
    {
        public bool RegisterEvent(string messageType, IEvent eventAction);

        public bool UnregisterEvent(string messageType);

        public void HandleEvent(FrameHeader frame);
    }
}
