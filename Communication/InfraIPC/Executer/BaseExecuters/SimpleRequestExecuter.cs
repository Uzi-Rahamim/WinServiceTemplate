using Intel.IntelConnect.IPC.Channel;
using Intel.IntelConnect.IPC.CommonTypes;
using Intel.IntelConnect.IPC.Executer;
using Microsoft.Extensions.Logging;

namespace Intel.IntelConnect.IPC.v1.Executer
{
    public abstract class SimpleRequestExecuter<T, Rq, Rs> : BaseRequestExecuter<T, Rq, Rs> where Rq : IMessageHeader where Rs : IMessageHeader
    {
        protected SimpleRequestExecuter(ILogger<T> logger, CancellationTokenSource cancellationToken) : base(logger, cancellationToken) { }
        protected override Task<Rs?> ExecuteAsync(IChannelSender channel, Rq request, Func<Rs, Task> sendNextResponse)
        {
            return ExecuteAsync(request);
        }

        protected abstract Task<Rs?> ExecuteAsync(Rq request);
    }

}
