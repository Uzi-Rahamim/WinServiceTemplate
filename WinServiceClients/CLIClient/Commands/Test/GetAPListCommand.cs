using Cocona;
using Serilog;
using Serilog.Core;
using ServerSDK.v1;

namespace APIClient.commands.test;

public class GetAPListCommand
{
    [Command]
    public static async void GetAPList()
    {
        var testAPI = new TestAPI();
        var networks = testAPI.GetAPList();
        await foreach (var network in networks)
        {
            Log.Information($"via SDK AP: {network.ssid} - {network.signalStrength}");
        }
      
    }
}