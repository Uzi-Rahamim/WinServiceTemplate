using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service_ExecuterPlugin.Worker;
using Types.Types;

namespace Service_ExecuterPlugin
{

    public class PluginSetup : IPluginSetup
    {
        IServiceCollection _serviceCollection;
        ILogger<PluginSetup> _logger;
        public void Configure()
        {   
            _logger.LogInformation("Configure");
            _serviceCollection.AddSingleton<SimpleWorker>();

            var serviceProvider = _serviceCollection.BuildServiceProvider();
            var worker = serviceProvider.GetRequiredService<SimpleWorker>();
            worker.Start();
        }

        public PluginSetup(IServiceCollection serviceCollection)
        {
            this._serviceCollection = serviceCollection;
            var serviceProvider = _serviceCollection.BuildServiceProvider();
            _logger = serviceProvider.GetRequiredService<ILogger<PluginSetup>>();
        }

        //public void PluginSetup2(IServiceCollection service)
        //{
        //    var services1 = new ServiceCollection();

        //    this._serviceCollection = service;
        //    var serviceProvider = builder.Services.BuildServiceProvider();
        //    _logger = serviceProvider.GetRequiredService<ILogger<PluginSetup>>();
        //}
       


    }
}
