using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PluginA.Worker;
using System.Reflection;
using WinService.Plugin.Common;

namespace PluginA
{

    public class PluginSetup : IPluginSetup
    {
        IServiceCollection? _serviceCollection;
        ILogger<PluginSetup>? _logger;
        
        public Version? GetVersion()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            return currentAssembly.GetName().Version;
        }

        public Task<bool> Start()
        {
            _logger?.LogInformation("Starting ... ");

            if (_serviceCollection == null || _logger == null)
                return Task.FromResult(false);

            var serviceProvider = _serviceCollection.BuildServiceProvider();
            var worker = serviceProvider.GetRequiredService<SimpleWorker>();
            worker.Start();

            return Task.FromResult(true);
        }

        public Task Initialize(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            _serviceCollection.AddSingleton<SimpleWorker>();
            var serviceProvider = _serviceCollection.BuildServiceProvider();
            _logger = serviceProvider.GetRequiredService<ILogger<PluginSetup>>();
            return Task.CompletedTask;
        }

        public Task<bool> Stop()
        {
            _logger?.LogInformation("Stoping ... ");
            return Task.FromResult(true);
        }

        public Task Shutdown()
        {
            _logger?.LogInformation("Shutdown");
            return Task.CompletedTask;
        }
    }
}
