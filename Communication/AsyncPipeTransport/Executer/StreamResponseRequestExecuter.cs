using AsyncPipeTransport.CommonTypes;
using Microsoft.Extensions.Logging;

namespace AsyncPipeTransport.Executer
{
    public abstract class StreamResponseRequestExecuter<T, Rq, Rs> : BaseRequestExecuter<T, Rq, Rs> where Rq : MessageHeader where Rs : MessageHeader
    {
        protected StreamResponseRequestExecuter(ILogger<T> logger, CancellationTokenSource cancellationToken) : base(logger, cancellationToken) { }
        protected override async Task<Rs?> Execute(Rq request, Func<Rs, Task> sendPage)
        {
            var responseStream = Execute(request);

            await foreach (var response in responseStream)
            {
                await sendPage(response);
            }
            return null;
        }

        protected abstract IAsyncEnumerable<Rs> Execute(Rq request);
    }
}
