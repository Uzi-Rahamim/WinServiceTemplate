using AsyncPipeTransport.Channel;
using AsyncPipeTransport.CommonTypes;
using Microsoft.Extensions.Logging;

namespace AsyncPipeTransport.Executer
{
    public abstract class SimpleRequestExecuter<T, Rq, Rs> : BaseRequestExecuter<T, Rq, Rs> where Rq : MessageHeader where Rs : MessageHeader
    {
        protected SimpleRequestExecuter(ILogger<T> logger, CancellationTokenSource cancellationToken) : base(logger, cancellationToken) { }
        protected override Task<Rs?> Execute(IChannelSender channel, Rq request, Func<Rs, Task> sendNextResponse)
        {
            return Execute(request);
        }

        protected abstract Task<Rs?> Execute(Rq request);
    }

}
