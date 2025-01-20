using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Events;

namespace AsyncPipeTransport.ClientHandlers
{
    public interface IClientEventHandler
    {
        public bool RegisterEvent(Opcode messageType, IEvent eventAction);


        public bool UnregisterEvent(Opcode messageType);

        public void HandleEvent(FrameHeader frame);
    }
}
