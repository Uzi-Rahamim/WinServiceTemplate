using APIClient.commands.test;
using Cocona;
using Microsoft.Extensions.Logging;
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

        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            // Add Serilog as a logging provider
            builder.AddSerilog();
        });

        var app = CoconaLiteApp.Create();
        await app.RunAsync<Program>();
    }
}
