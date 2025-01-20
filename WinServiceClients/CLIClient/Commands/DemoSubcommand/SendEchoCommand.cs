using ClientSDK.v1;
using Cocona;
using Serilog;

namespace APIClient.commands.test;

public class ShowServerCommand
{
    [Command]
    public static async Task echo([Argument(Description = "Your message")] string message)
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