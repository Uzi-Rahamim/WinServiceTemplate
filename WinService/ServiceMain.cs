using AsyncPipeTransport.ServerScheduler;
using CommunicationMessages;

namespace WinService;

public class ServiceMain : BackgroundService
{
    private readonly ILogger<ServiceMain> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ServiceMain(IServiceProvider serviceProvider,ILogger<ServiceMain> logger)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Worker Start running at: {time}", DateTimeOffset.Now);
            _= StartApi();

            while (!stoppingToken.IsCancellationRequested)
            {

                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
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
        var apiWorker = _serviceProvider.GetRequiredService<ServerMessageScheduler>();
        await apiWorker.Start(PipeApiConsts.PipeName);


    }
}
