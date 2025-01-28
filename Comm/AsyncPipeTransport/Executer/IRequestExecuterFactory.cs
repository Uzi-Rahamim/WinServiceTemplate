using AsyncPipeTransport.CommonTypes;
namespace AsyncPipeTransport.Executer
{
    public interface IRequestExecuterFactory
    {
        public IRequestExecuter Create();
        public string GetMessageType();
    }
}
