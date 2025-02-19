using App.WindowsService.API;
using AsyncPipeTransport.Executer;
using CommTypes.Consts;
using System.Reflection;
using Utilities;
using Utilities.PluginUtils;
using WinService.Plugin.Common;

namespace App.WindowsService
{
    internal class SetupPlugins
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly ILogger<SetupPlugins> _logger;
        private SetupPlugins(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            var serviceProvider = serviceCollection.BuildServiceProvider();
            _logger = serviceProvider.GetRequiredService<ILogger<SetupPlugins>>();
        }

        public static SetupPlugins Create(IServiceCollection builder)
        {
            return new SetupPlugins(builder);
        }

        public static IEnumerable<Type> GetTypes(Type interfaceType, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type is not null &&
                    interfaceType.FullName is not null &&
                    type.GetInterfaces().Any(intf => intf.FullName?.Contains(interfaceType.FullName) ?? false))
                {
                    yield return type;
                }
            }
        }

        public void LoadPlugins()
        {
            var pluginFileNames = RegistryUtils.GetKeySubStringValues(RegistryConsts.pluginKeyPath);
            foreach (var pluginFileName in pluginFileNames)
            {
                _logger.LogInformation("Found plugin  - {pluginFileName}", pluginFileName);
                var pluginAssembly = PluginLoader.LoadPlugin(pluginFileName);
                if (pluginAssembly == null)
                    continue;

                try
                {
                    //DI Isolation per plugin
                    var serviceCollection = new ServiceCollection();
                    CopyServices(_serviceCollection, serviceCollection);
                    _logger.LogInformation("Loading plugin Assembly - {assembly.FullName}", pluginAssembly.FullName);
   
                    // Load all types that implement IRequestExecuter
                    foreach (var type in GetTypes(typeof(IRequestExecuter), pluginAssembly))
                    {
                        _logger.LogInformation("Loading plugin Executer - {type.FullName}", type.FullName);
                        LoadExecuters(serviceCollection, type);
                    }

                    // Load all types that implement IPluginSetup
                    foreach (var type in PluginLoader.GetTypes(typeof(IPluginSetup), pluginAssembly))
                    {
                        _logger.LogInformation("Loading plugin setup - {type.FullName}", type.FullName);
                        // Create an instance by passing constructor arguments
                        IPluginSetup? setupObj = Activator.CreateInstance(type) as IPluginSetup;
                        setupObj?.Initialize(serviceCollection);
                        setupObj?.Start();
                    }

                    serviceCollection.BuildServiceProvider();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "LoadPlugins {pluginFileName} failed", pluginFileName);
                }

            }
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

        private void CopyServices(IServiceCollection source, IServiceCollection destination)
        {
            foreach (var serviceDescriptor in source)
            {
                //_logger.LogDebug("CopyServices {serviceDescriptor.ServiceType}", serviceDescriptor.ServiceType);
                destination.Add(serviceDescriptor);
            }
        }
    }
}