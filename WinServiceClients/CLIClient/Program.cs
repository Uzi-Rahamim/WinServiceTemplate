using APIClient.commands.test;
using ClientCLI;
using Cocona;
using Serilog;


[HasSubCommands(typeof(TestSubcommand), "test", Description = "test comunucation with the server")]
class Program
{
    public static async Task Main(string[] args)
    {
        // Set up Serilog for logging to the console
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();


        //Resolver.Init();
        //var builder = CoconaLiteApp.CreateBuilder();
        //var app = builder.Build();

        var app = CoconaLiteApp.Create();
        //app.AddCommand("commit", (string message) => { });
        //app.AddCommands<ShowServerCommand>();
        await app.RunAsync<Program>();
    }
}
