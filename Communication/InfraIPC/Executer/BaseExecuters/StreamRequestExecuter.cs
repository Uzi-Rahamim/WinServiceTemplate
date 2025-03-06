using Intel.IntelConnect.IPC.Channel;
using Intel.IntelConnect.IPC.CommonTypes;
using Intel.IntelConnect.IPC.Executer;
using Microsoft.Extensions.Logging;

namespace Intel.IntelConnect.IPC.v1.Executer
{
    public abstract class StreamRequestExecuter<T, Rq, Rs> : BaseRequestExecuter<T, Rq, Rs> where Rq : MessageHeader where Rs : MessageHeader
    {
        protected StreamRequestExecuter(ILogger<T> logger, CancellationTokenSource cancellationToken) : base(logger, cancellationToken) { }
        protected override async Task<Rs?> ExecuteAsync(IChannelSender channel, Rq request, Func<Rs, Task> sendNextResponse)
        {
            var responseStream = ExecuteAsync(request);

            await foreach (var response in responseStream)
            {
                await sendNextResponse(response);
            }
            return default(Rs);
        }

        protected abstract IAsyncEnumerable<Rs> ExecuteAsync(Rq request);
    }
}
