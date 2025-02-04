using ClientSDK.v1;
using Cocona;
using Serilog;

namespace APIClient.commands.test;

public class SendEchoCommand
{
    [Command]
    public static async Task SentEcho([Argument(Description = "version 1,2,3")] int ver, [Argument(Description = "Your message")] string message)
    {
        using (var channel = new ClientChannel())
        {
            if (!await channel.Connect())
            {
                Console.WriteLine("Failed to connect to server");
                return;
            }

            
            string echoMsg = "";
            switch (ver)
            {
                case 1:
                    var api = new DemoApi(channel);
                    echoMsg = await api.GetEcho(message) ?? "v1 fail";
                    break;
                case 2:
                    var api2 = new Service_ExecuterPlugin.ClientSDK.v1.Api(channel);
                    echoMsg = await api2.GetEcho(message) ?? "v2 fail";
                   
                    break;
                case 3:
                    var api3 = new Service_48_ExecuterPlugin.ClientSDK.v1.Api(channel);
                    echoMsg = await api3.GetEcho(message) ?? "v3 fail";
                    break;
                default:
                    Log.Information("Invalid version");
                    break;
            }


            Log.Information($"Server 2 reply to {message} with: {echoMsg}");
        }
    }
}