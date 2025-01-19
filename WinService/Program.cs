
using WinService;
using Serilog;
using App.WindowsService.API;
using AsyncPipeTransport.ServerScheduler;

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

builder.Services.AddSingleton<ServerMessageScheduler>();
builder.Services.AddHostedService<ServiceMain>();

var host = builder.Build();
host.Run();

// Dispose of the logger when the application ends
Log.CloseAndFlush();

