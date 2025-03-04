using Intel.IntelConnectPluginCommon;
using Serilog;
using Intel.IntelConnect.WindowsService.API;
using Intel.IntelConnect.WindowsService;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Intel.IntelConnect.IPC.Events.Service;
using System.Runtime.CompilerServices;

internal class Program
{
    private static async Task Main(string[] args)
    {
        string mutexName = "MyWinServiceUniqeName";

        //https://learn.microsoft.com/en-us/dotnet/core/extensions/windows-service
        //sc.exe create "MyService" binpath= "C:\Path\To\App.WindowsService.exe"
        //sc.exe start "MyService"
        //sc.exe stop "MyService"
        //sc.exe delete "MyService"
        using (Mutex mutex = new Mutex(true, mutexName, out bool isNewInstance))
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddWindowsService(options =>
            {
                options.ServiceName = "Uzi 123 Service";
            });
            Environment.SetEnvironmentVariable("WinDerviceLogDir", AppContext.BaseDirectory);

            
            //Clear Providers 
            builder.Logging.ClearProviders();

            //Read appsettings.json
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            if (!isNewInstance)
            {
                // If the mutex is already acquired, terminate the application
                Log.Error("Another instance is already running.");
                return; // Exit the application
            }

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Log.Fatal(e.ExceptionObject as Exception, "Unhandled exception occurred");
            };


            // Get the version of the current assembly
            Log.Information("\n\r Starting ... \n\r");
            LogServiceVersion();


            // add the provider
            builder.Logging.AddSerilog();

            //Build global service provider to share between providers
            var globalServiceProvider = ConfigureGlobalSharedProvider();

            //Build service provider
            builder.Services.AddSingleton(sp=> globalServiceProvider.GetRequiredService<CancellationTokenSource>());
            builder.Services.AddHostedService<LifeCycleManager>();

            SetupExecuters.Create(builder.Services).Configure(globalServiceProvider);
            
            //Build plugin providers
            await SetupPlugins.Create(builder.Services,globalServiceProvider).LoadPluginsAsync();

            var host = builder.Build();
            host.Run();

            Log.Information("\r\n Stoped !!! \r\n\r\n");
            // Dispose of the logger when the application ends
            await Log.CloseAndFlushAsync();
        }
    }

    private static ServiceProvider ConfigureGlobalSharedProvider()
    {
        var sharedServiceCollection = new ServiceCollection();
        sharedServiceCollection.AddSingleton<CancellationTokenSource>();
        sharedServiceCollection.AddSingleton<IEventDispatcher, EventDispatcher>();
        sharedServiceCollection.AddLogging(sp => sp.AddSerilog()); // Adds Serilog as the logging provider
        return sharedServiceCollection.BuildServiceProvider();
    }

    private static void LogServiceVersion()
    {
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        Version? version = currentAssembly.GetName().Version;
        if (version != null)
        {
            Log.Information("Service Version: {version}", version);
        }
        else
        {
            Log.Warning("Service Version information is not available.");
        }
    }
}