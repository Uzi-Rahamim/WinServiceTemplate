using Intel.IntelConnect.IPC.CommonTypes;
using Intel.IntelConnect.IPC.Executer;
using Intel.IntelConnect.IPC.Events.Service;
using Microsoft.Extensions.Logging;
using PluginA.Contract.Massages;
using Intel.IntelConnect.IPC.v1.Executer;
using Intel.IntelConnect.IPC.Attributes;

namespace PluginA.Executers
{
    [Executer(MethodName.PluginA_EventRegistration)]
    public class EventRequestExecuter : RegisterEventExecuter<EventRequestExecuter> , IRequestExecuter
    {
        public EventRequestExecuter(ILogger<EventRequestExecuter> logger, IEventDispatcher eventDispatcher, CancellationTokenSource cts) : 
            base(logger, eventDispatcher, cts) {
            logger.LogInformation("EventRequestExecuter created");
        }

     
        protected override Task StartEventsAsync(IEnumerable<string> topics, IEventDispatcher eventDispatcher)
        {
            Logger.LogInformation("StartEvents .... ");


            _ = Task.Run(async () => {
                int i = 0;
                
                while (true)
                {
                    Logger.LogInformation("DispatchEvent");
                    var success = await eventDispatcher.DispatchEventAsync( new CpuDataEventMessage(i++));
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

        protected override Task StopEventsAsync(IEnumerable<string> topics)
        {
            Logger.LogInformation("StopEvents .... ");
            return Task.CompletedTask;
        }
    }

}
