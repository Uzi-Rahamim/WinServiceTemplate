using Intel.IntelConnect.IPC.CommonTypes;

namespace Intel.IntelConnect.IPC.Events.Client
{
    public interface IEventManager
    {
        public bool RegisterEvent(string topic, IEvent eventAction);

        public bool UnregisterEvent(string topic);

        public void HandleEvent(FrameHeader frame);
    }
}
