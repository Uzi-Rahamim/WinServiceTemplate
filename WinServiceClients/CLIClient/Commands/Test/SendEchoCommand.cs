using Cocona;
using Serilog;
using ServerSDK.v1;

namespace APIClient.commands.test;

public class ShowServerCommand
{
    [Command]
    public static void echo([Argument(Description = "Your message")] string message)
    {
        var testAPI = new TestAPI();
        var echoMsg = testAPI.GetEcho(message).Result;
        Log.Information($"Server 2 reply to {message} with: {echoMsg}");
    }
}