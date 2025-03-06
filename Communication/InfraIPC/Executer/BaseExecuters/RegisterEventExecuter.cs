using Intel.IntelConnect.IPC.Channel;
using Intel.IntelConnect.IPC.CommonTypes;
using Intel.IntelConnect.IPC.CommonTypes.InternalMassages;
using Intel.IntelConnect.IPC.Events.Service;
using Intel.IntelConnect.IPC.Executer;
using Microsoft.Extensions.Logging;

namespace Intel.IntelConnect.IPC.v1.Executer
{
    public abstract class RegisterEventExecuter<T> : BaseRequestExecuter<T, EventRegistrationMessage, NullMessage>
    {
        private readonly IEventDispatcher _eventDispatcher;
        protected RegisterEventExecuter(ILogger<T> logger, IEventDispatcher eventDispatcher, CancellationTokenSource cancellationToken) : base(logger, cancellationToken)
        {
            _eventDispatcher = eventDispatcher;
        }

        protected override async Task<NullMessage?> ExecuteAsync(IChannelSender channel, EventRegistrationMessage request, Func<NullMessage, Task> sendNextResponse)
        {

            if (request.start)
            {
                await _eventDispatcher.SafeRegisterForEventsAsync(channel.ChannelId, channel, request.topics,
                     (topics) =>
                     {
                         if (topics.Count() > 0)
                             return StartEventsAsync(topics, _eventDispatcher);
                         return Task.CompletedTask;
                     });
            }
            else
            {
                await _eventDispatcher.SafeUnregisterEventsAsync(channel.ChannelId, request.topics,
                    (topics) =>
                    {
                        if (topics.Count() > 0)
                            return StopEventsAsync(topics);
                        return Task.CompletedTask;
                    });
            }

            return null;
        }

        protected abstract Task StartEventsAsync(IEnumerable<string> topics, IEventDispatcher eventDispatcher);

        protected abstract Task StopEventsAsync(IEnumerable<string> topics);
    }
}
