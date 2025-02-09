using ClientSDK.v1;
using Cocona;
using Microsoft.Extensions.Logging;
using Serilog;

namespace APIClient.commands.test
{
    public class MonitorKeepAliveCommand
    {
        [Command]
        public static async Task  monitor()
        {
            using (var channel = new ClientChannel(LoggerFactory.Create(builder => builder.AddSerilog())))
            {
                if (!await channel.Connect())
                {
                    Console.WriteLine("Failed to connect to server");
                    return;
                }

                var demoAPI = new DemoApi(channel);

                demoAPI.RegisterPulsEvent((msg) => { Console.WriteLine($"Pulse Event {msg}"); });

                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
        }
    }
}
