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
        private SetupPlugins(IHostApplicationBuilder builder)
        {
            _builder = builder;
            var serviceProvider = builder.Services.BuildServiceProvider();
            _logger = serviceProvider.GetRequiredService<ILogger<SetupPlugins>>();

        }

        public static SetupPlugins Create(IHostApplicationBuilder builder)
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

        public void LoadExecuters(Type type)
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


                var registerMethod = typeof(ExecuterRegister).GetMethod("RegisterRequest");
                // Make the method generic by passing the Type
                var genericRegisterMethod = registerMethod?.MakeGenericMethod(type);
                // Invoke the method
                genericRegisterMethod?.Invoke(null, new object[] { _builder, messageType, () => schema });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}