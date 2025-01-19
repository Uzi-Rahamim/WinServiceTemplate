using AsyncPipeTransport.CommonTypes;

namespace AsyncPipeTransport.Events
{
    public interface IEvent
    {
        public void Execute(FrameHeader frame);
    }
}
