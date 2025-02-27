using App.WindowsService.API;
using AsyncPipeTransport.Clients;
using AsyncPipeTransport.Executer;
using CommTypes.Consts;
using Serilog;
using Utilities;
using Utilities.PluginUtils;
using WinService.Plugin.Common;

namespace App.WindowsService
{
    internal class SetupPlugins
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly ServiceProvider _globalServiceProvider;
        private readonly ILogger<SetupPlugins> _logger;


        private SetupPlugins(IServiceCollection serviceCollection,
            ServiceProvider globalServiceProvider)
        {
            _serviceCollection = serviceCollection;
            _globalServiceProvider = globalServiceProvider;
            _logger = _globalServiceProvider.GetRequiredService<ILogger<SetupPlugins>>();
        }

        public static SetupPlugins Create(IServiceCollection serviceCollection,
                                          ServiceProvider globalServiceProvider)
        {
            return new SetupPlugins(serviceCollection, globalServiceProvider);
        }

        public async Task LoadPlugins()
        {
            var pluginFileNames = RegistryUtils.GetKeySubStringValues(RegistryConsts.pluginKeyPath);

            foreach (var pluginFileName in pluginFileNames)
            {
                var serviceCollection = CreateNewCollection();
                await LoadPlugin(pluginFileName, serviceCollection);
            }
        }
        private async Task LoadPlugin(string pluginFileName, IServiceCollection serviceCollection)
        {

            _logger.LogInformation("Found plugin  - {pluginFileName}", pluginFileName);
            var pluginAssembly = PluginLoader.LoadPlugin(pluginFileName);
            if (pluginAssembly == null)
                return;

            try
            {
                //DI Isolation per plugin
                _logger.LogInformation("Loading plugin Assembly - {assembly.FullName}", pluginAssembly.FullName);

                // Load Plugin IPluginSetup
                var pluginSetupType = PluginLoader.GetTypes(typeof(IPluginSetup), pluginAssembly).FirstOrDefault();
                if (pluginSetupType == null)
                {
                    _logger.LogError("Plugin setup not found- {assembly.FullName} failed", pluginAssembly.FullName);
                    return;
                }

                _logger.LogInformation("Loading plugin setup - {type.FullName}", pluginSetupType.FullName);
                if (!await LoadPluginSetup(serviceCollection, pluginSetupType))
                {
                    _logger.LogError("Loading plugin setup - {type.FullName} failed", pluginSetupType.FullName);
                    return;
                }

                // Load all Plugin types that implement IRequestExecuter
                foreach (var type in PluginLoader.GetTypes(typeof(IRequestExecuter), pluginAssembly))
                {
                    _logger.LogInformation("Loading plugin Executer - {type.FullName}", type.FullName);
                    LoadExecuters(serviceCollection, type);
                }

               // serviceCollection.BuildServiceProvider();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoadPlugins {pluginFileName} failed", pluginFileName);
            }
        }

        private async Task<bool> LoadPluginSetup(IServiceCollection serviceCollection, Type pluginSetupType)
        {
            // Create an instance by passing constructor arguments
            IPluginSetup? pluginSetup = Activator.CreateInstance(pluginSetupType) as IPluginSetup;
            if (pluginSetup == null)
                return false;

            await pluginSetup.Initialize(serviceCollection);
            var version = pluginSetup.GetVersion();
            _logger.LogInformation("Loading plugin setup - {type.FullName} {version}", pluginSetupType.FullName, version);
            PluginManager.GetInstance().AddPlugin(pluginSetup);

            //return await pluginSetup.Start();


            return true;
        }

        private void LoadExecuters(IServiceCollection serviceCollection, Type type)
        {
            try
            {
                //Get static Schema from plugin
                var schema = (string)(type.GetMethod("Plugin_GetSchema")?.Invoke(null, null) ?? "missing Schema");

                //Get static MessageType from plugin
                var messageType = (string?)type.GetMethod("Plugin_GetMessageType")?.Invoke(null, null);
                if (messageType is null)
                {
                    throw new ApplicationException("Plugin_GetMessageType is missing");
                }
                ExecuterRegister.RegisterSchema(_serviceCollection, messageType, () => schema);

                var registerExecuter = typeof(ExecuterRegister).GetMethod("RegisterPluginExecuter")?.MakeGenericMethod(type); //make generic method
                // Invoke the method
                registerExecuter?.Invoke(null, new object[] { _serviceCollection, serviceCollection, messageType });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoadExecuters {type} failed", type);
            }
        }

        private IServiceCollection CreateNewCollection()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
            {
                builder.AddSerilog(); // Adds Serilog as the logging provider
            });
            serviceCollection.AddSingleton(sp=> _globalServiceProvider.GetRequiredService<IEventDispatcher>());
            serviceCollection.AddSingleton(sp=> _globalServiceProvider.GetRequiredService<CancellationTokenSource>());
            return serviceCollection;
        }
    }
}