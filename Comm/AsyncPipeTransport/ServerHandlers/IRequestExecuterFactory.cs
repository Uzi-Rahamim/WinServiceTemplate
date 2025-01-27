using AsyncPipeTransport.CommonTypes;
namespace AsyncPipeTransport.ServerHandlers
{
    public interface IRequestExecuterFactory
    {
        public IRequestExecuter Create();
        public string GetMessageType();
    }
}
