using Intel.IntelConnect.ClientSDK.v1;
using Cocona;
using Microsoft.Extensions.Logging;
using Serilog;

namespace APIClient.commands.test;

public class Service_SendEchoCommand
{
    [Command]
    public static async Task Service_SentEcho([Argument(Description = "Your message")] string message)
    {
        Log.Information($"Service_SentEcho");
        using (var channel = new SdkClientChannel(LoggerFactory.Create(builder => builder.AddSerilog())))
        {
            if (!await channel.Connect())
            {
                Log.Error("Failed to connect to server");
                return;
            }
          
            var api = new WinServiceApi(channel);
            var echoMsg = await api.GetEcho(message) ?? "echo fail";
            Log.Information($"Server reply to {message} with: {echoMsg}");
        }
    }
}