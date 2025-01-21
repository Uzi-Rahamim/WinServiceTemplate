using AsyncPipeTransport.CommonTypes;
namespace AsyncPipeTransport.ServerHandlers
{
    public interface IRequestCommandFactory
    {
        public IRequestCommand Create();
        public Opcode GetMessageType();
    }
}
