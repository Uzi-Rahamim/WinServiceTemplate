using Intel.IntelConnect.ClientSDK.v1;
using Cocona;
using Microsoft.Extensions.Logging;
using Serilog;
using PluginA.ClientSDK.v1;

namespace APIClient.commands.test;

public class PluginA_ListenToCpuEventsCommand
{
    [Command]
    public static async Task PluginA_ListenToCpuEventsAsync()
    {
        Log.Information($"PluginA_SentEcho");
        using (var channel = new SdkClientChannel(LoggerFactory.Create(builder => builder.AddSerilog())))
        {
            if (!await channel.ConnectAsync())
            {
                Log.Error("Failed to connect to server");
                return;
            }

            var api = new PluginA_Api(channel);
            await api.RegisterCpuEventAsync((value)=>
                Log.Information($"Cpu value is : {value}"));


            Log.Information("Wait For Events (Enter to abort)...");
            Console.ReadLine();

            await api.UnregisterCpuEventAsync();
        }
    }
}