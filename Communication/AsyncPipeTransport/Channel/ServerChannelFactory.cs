namespace AsyncPipeTransport.Channel
{
    public interface IServerChannelFactory
    {
        IServerChannel Create();
    }

    public class ServerChannelFactory: IServerChannelFactory
    {
        string _pipeName;
        public ServerChannelFactory(string pipeName)
        {
            this._pipeName = pipeName;
        }

        public IServerChannel Create()
        {
            return new ServerPipeChannel(_pipeName);
        }
    }
}
