using Microsoft.Extensions.Logging;

namespace AsyncPipeTransport.Channel
{
    public interface IServerChannelFactory
    {
        IServerChannel Create();
    }

    public class ServerChannelFactory: IServerChannelFactory
    {
        private readonly string _pipeName;
        private readonly ILogger<ClientPipeChannel> _logger;
        public ServerChannelFactory(ILogger<ClientPipeChannel> logger, string pipeName)
        {
            this._pipeName = pipeName;
            this._logger = logger;
        }

        public IServerChannel Create()
        {
            return new ServerPipeChannel(_logger, _pipeName);
        }
    }
}
