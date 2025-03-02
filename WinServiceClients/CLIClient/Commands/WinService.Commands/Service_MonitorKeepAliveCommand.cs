using Intel.IntelConnect.ClientSDK.v1;
using Cocona;
using Microsoft.Extensions.Logging;
using Serilog;

namespace APIClient.commands.test
{
    public class Service_MonitorKeepAliveCommand
    {
        [Command]
        public static async Task Service_Monitor()
        {
            Log.Information($"Service_Monitor");
            using (var channel = new SdkClientChannel(LoggerFactory.Create(builder => builder.AddSerilog())))
            {
                if (!await channel.Connect())
                {
                    Log.Error("Failed to connect to server");
                    return;
                }

                var demoAPI = new WinServiceApi(channel);

                //demoAPI.RegisterPulsEvent((msg) => { Console.WriteLine($"Pulse Event {msg}"); });

                Log.Error("Press any key to exit");
                Console.ReadKey();
            }
        }
    }
}
