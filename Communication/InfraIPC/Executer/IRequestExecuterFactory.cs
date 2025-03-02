using Intel.IntelConnect.IPC.CommonTypes;
namespace Intel.IntelConnect.IPC.Executer
{
    public interface IRequestExecuterFactory
    {
        public IRequestExecuter Create();
        public string GetMessageType();
    }
}
