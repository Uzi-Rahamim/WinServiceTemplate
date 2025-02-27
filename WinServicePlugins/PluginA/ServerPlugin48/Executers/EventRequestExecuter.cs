using AsyncPipeTransport.Clients;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Executer;
using Microsoft.Extensions.Logging;
using PluginA.Contract.Massages;

namespace PluginA.Executers
{
    public class EventRequestExecuter : EventRegisterRequestExecuter<EventRequestExecuter> , IRequestExecuter
    {
        public EventRequestExecuter(ILogger<EventRequestExecuter> logger, IEventDispatcher eventDispatcher, CancellationTokenSource cts) : 
            base(logger, eventDispatcher, cts) {
            logger.LogInformation("EventRequestExecuter created");
        }

        public static string Plugin_GetSchema()
        {
            return GetSchema();
        }

        public static string Plugin_GetMessageType()
        {
            return FrameworkMessageTypes.RegisterEvent;
        }

        protected override Task StartEvents(RegisterForEventMessage request, IEventDispatcher eventDispatcher)
        {
            Logger.LogInformation("StartEvents .... ");


            _ = Task.Run(async () => {
                int i = 0;
                
                while (true)
                {
                    Logger.LogInformation("DispatchEvent");
                    var success = await eventDispatcher.DispatchEvent(new GetCpuDataEventMessage(i++));
                    if (!success)
                    {
                        Logger.LogWarning("DispatchEvent stop - no Clients");
                        break;
                    }
                    await Task.Delay(2000);
                }
            });
            return Task.CompletedTask;
        }

    }

}
