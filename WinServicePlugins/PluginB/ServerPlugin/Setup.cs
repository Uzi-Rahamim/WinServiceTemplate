﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PluginB.Worker;
using WinService.Plugin.Common;

namespace PluginB
{
    public class PluginSetup : IPluginSetup
    {
        private IServiceCollection? _serviceCollection;
        private ILogger<PluginSetup>? _logger;
       
        public bool Start()
        {
            if (_serviceCollection == null || _logger == null) 
                return false;
            _logger.LogInformation("Starting ... ");
            _serviceCollection.AddSingleton<SimpleWorker>();

            var serviceProvider = _serviceCollection.BuildServiceProvider();
            var worker = serviceProvider.GetRequiredService<SimpleWorker>();
            worker.Start();

            return true;
        }

        public void Initialize(IServiceCollection serviceCollection)
        {
            this._serviceCollection = serviceCollection;
            var serviceProvider = _serviceCollection.BuildServiceProvider();
            _logger = serviceProvider.GetRequiredService<ILogger<PluginSetup>>();
        }
    }
}
