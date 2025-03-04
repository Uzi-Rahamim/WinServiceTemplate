using Intel.IntelConnect.ClientSDK.v1;
using Cocona;
using Microsoft.Extensions.Logging;
using Serilog;
using PluginA.ClientSDK.v1;

namespace ClientCLI.Commands.PluginA.Commands;

public class PluginA_GetAPListCommand
{
    [Command]
    public static async Task PluginA_GetAPListAsync()
    {
        try
        {
            Log.Information($"PluginA_GetAPList");
            using (var channel = new SdkClientChannel(LoggerFactory.Create(builder => builder.AddSerilog())))
            {
                if (!await channel.ConnectAsync())
                {
                    Log.Error("Failed to connect to server");
                    return;
                }


                var api = new PluginA_Api(channel);

                Log.Information("Using GetAPListAsync");

                var networks = api.GetAPListAsync();
                await foreach (var network in networks)
                {
                    Log.Information($"via SDK AP: {network.ssid} - {network.signalStrength}");
                }
                //Log.Information("Using GetAPListStream");
                //await testAPI.GetAPListStream((network)=> Log.Information($"via SDK AP: {network.ssid} - {network.signalStrength}"));
            }
        }
        catch (Exception ex) when (
                ex is TimeoutException ||
                ex is OperationCanceledException ||
                ex is IOException)
        {
            Log.Information($"Exception {ex.GetType().Name} in MessageListener ");
        }
        catch (Exception ex)
        {
            Log.Information(ex, "Error in MessageListener");
        }
    }
}