using APIClient.commands.test;
using Cocona;
using Serilog;


[HasSubCommands(typeof(DemoSubcommand), "demo", Description = "demo for testing server comunucation")]
class Program
{
    public static async Task Main(string[] args)
    {
        // Set up Serilog for logging to the console
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        var app = CoconaLiteApp.Create();
        await app.RunAsync<Program>();
    }
}
