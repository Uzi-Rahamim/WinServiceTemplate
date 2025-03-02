using Intel.IntelConnect.IPC.Channel;
using Intel.IntelConnect.IPC.CommonTypes;
using Microsoft.Extensions.Logging;

namespace Intel.IntelConnect.IPC.Executer
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
