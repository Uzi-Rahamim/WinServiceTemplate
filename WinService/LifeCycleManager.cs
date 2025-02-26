using App.WindowsService;
using AsyncPipeTransport.Listeners;

namespace WinService;

public class LifeCycleManager : BackgroundService
{
    private readonly ILogger<LifeCycleManager> _logger;
    private readonly CancellationTokenSource _cts;
    private readonly IServiceProvider _serviceProvider;

    public LifeCycleManager(CancellationTokenSource cts,
        IServiceProvider serviceProvider,
        ILogger<LifeCycleManager> logger)
    {
        _logger = logger;
        _cts = cts;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("LifeCycleManager Start running at: {time}", DateTimeOffset.Now);
            await PluginManager.GetInstance().StartPlugins();
            _ = StartApi();

            //var clientBroadcaster = _serviceProvider.GetRequiredService<IClientsManager>();
            while (!stoppingToken.IsCancellationRequested)
            {
                //clientBroadcaster.BroadcastEvent(new BPulseEventMessage("BroadcastEvent"));
                ////_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }

            _cts.Cancel();
            _logger.LogInformation("LifeCycleManager Shutdown Plugins");
            await PluginManager.GetInstance().ShutdownPlugins();
            _logger.LogInformation("LifeCycleManager Stop running at: {time}", DateTimeOffset.Now);
        }
        catch (OperationCanceledException)
        {
            // When the stopping token is canceled, for example, a call made from services.msc,
            // we shouldn't exit with a non-zero exit code. In other words, this is expected...
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);

            // Terminates this process and returns an exit code to the operating system.
            // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
            // performs one of two scenarios:
            // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
            // 2. When set to "StopHost": will cleanly stop the host, and log errors.
            //
            // In order for the Windows Service Management system to leverage configured
            // recovery options, we need to terminate the process with a non-zero exit code.
            Environment.Exit(1);
        }
    }

    private async Task StartApi()
    {
        try
        {
            var apiWorker = _serviceProvider.GetRequiredService<ServerIncomingConnectionListener>();
            await apiWorker.Start(_cts.Token);

        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Worker Start api worker failed");
        }
    }
}
