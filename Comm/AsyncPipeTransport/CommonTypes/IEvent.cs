using AsyncPipe.Transport;
using AsyncPipeTransport.Transport;

namespace AsyncPipeTransport.CommonTypes
{

    public interface IEvent
    {
        public void Execute(TransportFrameHeader frame);
    }
}
