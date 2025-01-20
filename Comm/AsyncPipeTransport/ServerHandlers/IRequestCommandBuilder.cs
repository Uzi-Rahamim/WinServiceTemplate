using AsyncPipeTransport.CommonTypes;
namespace AsyncPipeTransport.ServerHandlers
{
    public interface IRequestCommandBuilder
    {
        public IRequestCommand Build();
        public Opcode GetMessageType();
    }
}
