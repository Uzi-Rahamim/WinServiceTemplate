
using WinService;
using Serilog;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.Logging.Configuration;
using System.Diagnostics;
using App.WindowsService.API;
using App.WindowsService.API.Requests;
using AsyncPipeTransport.RequestHandler;
using CommunicationMessages;
using AsyncPipeTransport.CommonTypes;
using Microsoft.Extensions.DependencyInjection;


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

//builder.Services.AddTransient<IRequestHandler, EchoRequestHandler>();
//builder.Services.AddTransient<IRequestHandler, GetAPListRequestHandler>();

//// Register IRequestHandlerBuilder using a factory
//builder.Services.AddSingleton<IRequestHandlerBuilder>(serviceProvider =>
//{
//    var factory = () => (IRequestHandler)serviceProvider.GetRequiredService<EchoRequestHandler>();
//    return new RequestHandlerBuilder((Opcode)MessageType.Echo, factory);
//});

RegisterCommand<EchoRequestHandler>(MessageType.Echo);
RegisterCommand<GetAPListRequestHandler>(MessageType.APList);


builder.Services.AddSingleton<ServerMessageScheduler>();
builder.Services.AddHostedService<MyBackgroundService>();

var host = builder.Build();
host.Run();

// Dispose of the logger when the application ends
Log.CloseAndFlush();


void RegisterCommand<T>(MessageType messageType) where T : class, IRequestHandler
{
    // Register the IRequestHandler implementation as Transient
    builder.Services.AddTransient<T>();
    builder.Services.AddSingleton<IRequestHandlerBuilder>(serviceProvider =>
    {
        var factory = () => serviceProvider.GetRequiredService<T>();
        return new RequestHandlerBuilder((Opcode)messageType, factory);
    });
}
