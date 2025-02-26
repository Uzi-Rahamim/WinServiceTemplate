using ClientSDK.v1;
using Cocona;
using Microsoft.Extensions.Logging;
using Serilog;
using PluginB.ClientSDK.v1;

namespace APIClient.commands.test;

public class PluginB_SendEchoCommand
{
    [Command]
    public static async Task PluginB_SentEcho([Argument(Description = "Your message")] string message)
    {
        Log.Information($"PluginB_SentEcho");
        using (var channel = new SdkClientChannel(LoggerFactory.Create(builder => builder.AddSerilog())))
        {
            if (!await channel.Connect())
            {
                Log.Error("Failed to connect to server");
                return;
            }

            var api = new PluginB_Api(channel);
            var echoMsg = await api.GetEcho(message) ?? "echo fail";
            Log.Information($"Server reply to {message} with: {echoMsg}");
        }
    }
}