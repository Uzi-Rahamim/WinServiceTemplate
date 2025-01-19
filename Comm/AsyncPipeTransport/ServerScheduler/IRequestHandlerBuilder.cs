using AsyncPipeTransport.CommonTypes;
namespace AsyncPipeTransport.ServerScheduler
{
    public interface IRequestHandlerBuilder
    {
        public IRequestHandler Build();
        public Opcode GetMessageType();
    }
}
