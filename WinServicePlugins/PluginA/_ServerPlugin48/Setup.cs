﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service_ExecuterPlugin.Worker;
using Types.Types;

namespace Service_ExecuterPlugin
{

    public class PluginSetup : IPluginSetup
    {
        IHostApplicationBuilder _builder;
        ILogger<PluginSetup> _logger;
        public void Configure()
        {
            

            _logger.LogInformation("Configure");
            _builder.Services.AddSingleton<SimpleWorker>();

            var serviceProvider = _builder.Services.BuildServiceProvider();
            var worker = serviceProvider.GetRequiredService<SimpleWorker>();
            worker.Start();
        }

        public PluginSetup(IHostApplicationBuilder builder)
        {
            this._builder = builder;
            var serviceProvider = builder.Services.BuildServiceProvider();
            _logger = serviceProvider.GetRequiredService<ILogger<PluginSetup>>();
        }

   
    }
}
