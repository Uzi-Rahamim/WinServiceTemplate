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

        public Task<bool> Start()
        {
            if (_serviceCollection == null || _logger == null)
                return Task.FromResult(false);
            _logger.LogInformation("Starting ... ");
            _serviceCollection.AddSingleton<SimpleWorker>();

            var serviceProvider = _serviceCollection.BuildServiceProvider();
            var worker = serviceProvider.GetRequiredService<SimpleWorker>();
            worker.Start();

            return Task.FromResult(true);
        }

        public Task<bool> Stop()
        {
            return Task.FromResult(true);
        }

        public Version? GetVersion()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            return currentAssembly.GetName().Version;
        }

        public Task Initialize(IServiceCollection serviceCollection)
        {
            this._serviceCollection = serviceCollection;
            var serviceProvider = _serviceCollection.BuildServiceProvider();
            _logger = serviceProvider.GetRequiredService<ILogger<PluginSetup>>();

            return Task.CompletedTask;
        }
    }
}
