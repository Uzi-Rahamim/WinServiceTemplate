using Intel.IntelConnect.IPC.Channel;
using Intel.IntelConnect.IPC.Clients;
using Intel.IntelConnect.IPC.CommonTypes;
using Intel.IntelConnect.IPC.CommonTypes.InternalMassages;
using Microsoft.Extensions.Logging;

namespace Intel.IntelConnect.IPC.Executer
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

            if (request.start)
            {
                _eventDispatcher.RegisterForEvents(channel.ChannelId, channel, request.topics);
                await StartEvents(request, _eventDispatcher);
            }
            else
            {
                await StopEvents(request);
                _eventDispatcher.UnregisterEvents(channel.ChannelId, request.topics);
            }
            
            return null;
        }

        protected abstract Task StartEvents(RegisterForEventMessage request, IEventDispatcher eventDispatcher);

        protected abstract Task StopEvents(RegisterForEventMessage request);
    }
}
