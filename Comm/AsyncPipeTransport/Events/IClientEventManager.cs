using AsyncPipeTransport.CommonTypes;

namespace AsyncPipeTransport.Events
{
    public interface IClientEventManager
    {
        public bool RegisterEvent(string messageType, IEvent eventAction);


        public bool UnregisterEvent(string messageType);

        public void HandleEvent(FrameHeader frame);
    }
}
