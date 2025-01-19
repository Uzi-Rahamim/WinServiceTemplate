using ClientSDK.v1;
using Cocona;
using Serilog;

namespace APIClient.commands.test;

public class GetAPListCommand
{
    [Command]
    public static async Task GetAPList()
    {
        using (var channel = new ClientChannel())
        {
            channel.Connect();


            var demoAPI = new DemoApi(channel);

            Log.Information("Using GetAPListAsync");

            var networks = demoAPI.GetAPListAsync();
            await foreach (var network in networks)
            {
                Log.Information($"via SDK AP: {network.ssid} - {network.signalStrength}");
            }

            //Log.Information("Using GetAPListStream");
            //await testAPI.GetAPListStream((network)=> Log.Information($"via SDK AP: {network.ssid} - {network.signalStrength}"));
        }
    }
}