using ClientSDK.v1;
using Cocona;
using Microsoft.Extensions.Logging;
using Serilog;
using PluginA.ClientSDK.v1;

namespace APIClient.commands.test;

public class PluginA_SendEchoCommand
{
    [Command]
    public static async Task PluginA_SentEcho([Argument(Description = "Your message")] string message)
    {
        Log.Information($"PluginA_SentEcho");
        using (var channel = new SdkClientChannel(LoggerFactory.Create(builder => builder.AddSerilog())))
        {
            if (!await channel.Connect())
            {
                Log.Error("Failed to connect to server");
                return;
            }

            var api = new PluginA_Api(channel);
            var echoMsg = await api.GetEcho(message) ?? "echo fail";
            Log.Information($"Server 2 reply to {message} with: {echoMsg}");
        }
    }
}