
using WinService;
using Serilog;
using App.WindowsService.API;
using AsyncPipeTransport.ServerHandlers;

string mutexName = "MyWinServiceUniqeName";

using (Mutex mutex = new Mutex(true, mutexName, out bool isNewInstance))
{
    if (!isNewInstance)
    {
        // If the mutex is already acquired, terminate the application
        Console.WriteLine("Another instance is already running.");
        return; // Exit the application
    }

    var builder = Host.CreateApplicationBuilder(args);
    builder.Services.AddWindowsService(options =>
    {
        options.ServiceName = "Uzi 123 Service";
    });

    //Clear Providers 
    builder.Logging.ClearProviders();

    //Read appsettings.json
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();

    // add the provider
    builder.Logging.AddSerilog();


    SetupRequestHandlers.Create(builder).Configure();


    builder.Services.AddHostedService<ServiceMain>();

    var host = builder.Build();
    host.Run();

    // Dispose of the logger when the application ends
    Log.CloseAndFlush();
}
