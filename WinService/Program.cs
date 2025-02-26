
using WinService;
using Serilog;
using App.WindowsService.API;
using App.WindowsService;
using System.Reflection;

internal class Program
{
    private static void Main(string[] args)
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
            builder.Services.AddSingleton<CancellationTokenSource>();

            
            SetupExecuters.Create(builder.Services).Configure();
            SetupPlugins.Create(builder.Services).LoadPlugins().Wait();


            builder.Services.AddHostedService<ServiceMain>();
            //builder.Services.AddWindowsService(options =>
            //{
            //    options.ServiceName = "MyWindowsService33333";
            //});

            var host = builder.Build();
            host.Run();

            Log.Information("\r\n Stoped !!! \r\n\r\n");
            // Dispose of the logger when the application ends
            Log.CloseAndFlush();
        }
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