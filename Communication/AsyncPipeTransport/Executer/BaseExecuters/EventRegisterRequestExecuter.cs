using AsyncPipeTransport.Channel;
using AsyncPipeTransport.Clients;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.CommonTypes.InternalMassages;
using Microsoft.Extensions.Logging;

namespace AsyncPipeTransport.Executer
{
    public abstract class EventRegisterRequestExecuter<T> : BaseRequestExecuter<T, RegisterForEventMessage, NullMessage> 
    {
        private readonly IEventDispatcher _eventDispatcher;
        protected EventRegisterRequestExecuter(ILogger<T> logger, IEventDispatcher eventDispatcher, CancellationTokenSource cancellationToken) : base(logger, cancellationToken)
        {
            _eventDispatcher = eventDispatcher;
        }

        protected override async Task<NullMessage?> Execute(IChannelSender channel, RegisterForEventMessage request, Func<NullMessage, Task> sendNextResponse)
        {
            _eventDispatcher.RegisterForEvents(channel.ChannelId, channel, request.topics);
            await StartEvents(request, _eventDispatcher);
            return null;
        }

        protected abstract Task StartEvents(RegisterForEventMessage request, IEventDispatcher eventDispatcher);
    }
}
