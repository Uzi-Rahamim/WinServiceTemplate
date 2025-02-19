using App.WindowsService.API;
using AsyncPipeTransport.Executer;
using CommTypes.Consts;
using Utilities;
using Utilities.PluginUtils;
using WinService.Plugin.Common;

namespace App.WindowsService
{
    internal class SetupPlugins
    {
        private IHostApplicationBuilder _builder;
        private ILogger<SetupPlugins> _logger;
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


        public void LoadPlugins()
        {
            var pluginFileNames = RegistryUtils.GetKeySubStringValues(RegistryConsts.pluginKeyPath);

            foreach (var pluginFileName in pluginFileNames)
                foreach (var pluginAssembly in PluginLoader.LoadPlugin(pluginFileName))
                {
                    try
                    {
                        _logger.LogInformation("Loading plugin Assembly - {pluginAssembly.FullName}", pluginAssembly.FullName);

                        // Load all types that implement IRequestExecuter
                        foreach (var type in PluginLoader.GetTypes(typeof(IRequestExecuter), pluginAssembly))
                        {
                            _logger.LogInformation("Loading plugin Executer - {type.FullName}", type.FullName);
                            LoadExecuters(type);
                        }
            _logger.LogInformation("Loading plugin from - {AssemblyPath}", AssemblyPath);
           
            foreach (var pluginAssembly in PluginLoader.LoadPlugin(AssemblyPath, "*ExecuterPlugin.dll"))
            {
                try
                {
                    //DI Isolation per plugin
                    var serviceCollection = new ServiceCollection();
                    CopyServices(_serviceCollection, serviceCollection);
                    _logger.LogInformation("Loading plugin Assembly - {pluginAssembly.FullName}", pluginAssembly.FullName);
                    foreach (var type in PluginLoader.GetTypes(typeof(IRequestExecuter), pluginAssembly))
                    {
                        _logger.LogInformation("Loading plugin Executer - {type.FullName}", type.FullName);
                        LoadExecuters(serviceCollection, type);
                    }

                    foreach (var type in PluginLoader.GetTypes(typeof(IPluginSetup), pluginAssembly))
                    {
                        _logger.LogInformation("Loading plugin setup - {type.FullName}", type.FullName);
                        // Create an instance by passing constructor arguments
                        IPluginSetup? setupObj = Activator.CreateInstance(type, serviceCollection) as IPluginSetup;
                        setupObj?.Configure();
                    }
                    
                    serviceCollection.BuildServiceProvider();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
           
                        // Load all types that implement IPluginSetup
                        foreach (var type in PluginLoader.GetTypes(typeof(IPluginSetup), pluginAssembly))
                        {
                            _logger.LogInformation("Loading plugin setup - {type.FullName}", type.FullName);
                            // Create an instance by passing constructor arguments
                            IPluginSetup? setupObj = Activator.CreateInstance(type, _builder) as IPluginSetup;
                            setupObj?.Configure();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "load {pluginFileName} failed", pluginFileName);
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

                var registerSchema = typeof(ExecuterRegister).GetMethod("RegisterSchema");
                var registerExecuter = typeof(ExecuterRegister).GetMethod("RegisterExecuter")?.MakeGenericMethod(type); //make generic method

                // Invoke the method
                registerSchema?.Invoke(null, new object[] { _serviceCollection, messageType, () => schema });
                registerExecuter?.Invoke(null, new object[] { serviceCollection, messageType });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void CopyServices(IServiceCollection source, IServiceCollection destination)
        {
            foreach (var serviceDescriptor in source)
            {
                //Console.WriteLine(serviceDescriptor.ServiceType);
                destination.Add(serviceDescriptor);
            }
        }
    }
}