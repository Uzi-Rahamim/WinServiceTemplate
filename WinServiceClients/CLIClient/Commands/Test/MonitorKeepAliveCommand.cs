using Cocona;
using Serilog;

namespace APIClient.commands.test
{
    public class MonitorKeepAliveCommand
    {
        [Command]
        public static void monitor()
        {
            Log.Information($"monitor");
        }
    }
}
