using ClientSDK.v1;
using Cocona;
using Serilog;

namespace APIClient.commands.test;

public class SendEchoCommand
{
    [Command]
    public static async Task SentEcho([Argument(Description = "Your message")] string message)
    {
        using (var channel = new ClientChannel())
        {
            if (!await channel.Connect())
            {
                Console.WriteLine("Failed to connect to server");
                return;
            }

            var demoAPI = new DemoApi(channel);
            var echoMsg = await demoAPI.GetEcho(message);
            Log.Information($"Server 2 reply to {message} with: {echoMsg}");
        } 
    }
}