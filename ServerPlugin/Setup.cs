using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service_ExecuterPlugin.Types;
using Service_ExecuterPlugin.Worker;
using System.Runtime.InteropServices;
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

            IXtuSdkWrapper xtuSdkWrapper = LoadCom();
            _builder.Services.AddSingleton<IXtuSdkWrapper>(provider => xtuSdkWrapper);

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

        public IXtuSdkWrapper LoadCom()
        {
            try
            {
                // Creating instance of COM object (MyComClass) via GUID.
                Type comType = Type.GetTypeFromProgID("ComWrapper.XtuSdkWrapper");
                if (comType == null)
                {
                    throw new InvalidOperationException("COM object not found.");
                }
                return Activator.CreateInstance(comType) as IXtuSdkWrapper;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return null;
        }
    }
}
