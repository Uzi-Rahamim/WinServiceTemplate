using ClientSDK.v1;
using Cocona;
using Microsoft.Extensions.Logging;
using Serilog;

namespace APIClient.commands.test;

public class GetPlatfrmListCommand
{
    [Command]
    public static async Task GetList()
    {
        using (var channel = new ClientChannel(LoggerFactory.Create(builder => builder.AddSerilog())))
        {
            if (!await channel.Connect())
            {
                Console.WriteLine("Failed to connect to server");
                return;
            }


            //var api = new Api(channel);

            Log.Information("Using GetPlatformCompatibility");

            //var list = await api.GetPlatformCompatibility();
            //foreach (var item in list)
            //{
            //    Log.Information($"{item}");
            //}

            //Log.Information("Using GetAPListStream");
            //await testAPI.GetAPListStream((network)=> Log.Information($"via SDK AP: {network.ssid} - {network.signalStrength}"));
        }
    }
}