using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Executer;
using Microsoft.Extensions.Logging;
using Types.Types;
using Utilities.PluginUtils;

namespace App.WindowsService.API
{
    internal class SetupPlugins
    {
        private string AssemblyPath { get => @"C:\Repo\MyRepos\WinServiceTemplate\ServerPlugin\bin\x64\Debug\net8.0\"; }
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
            try
            {
                _logger.LogInformation("Loading plugin from - {AssemblyPath}", AssemblyPath);
                foreach (var pluginAssembly in PluginLoader.LoadPlugin(AssemblyPath, "*ExecuterPlugin.dll"))
                {
                    _logger.LogInformation("Loading plugin Assembly - {pluginAssembly.FullName}", pluginAssembly.FullName);
                    foreach (var type in PluginLoader.GetTypes(typeof(IRequestExecuter), pluginAssembly))
                    {
                        _logger.LogInformation("Loading plugin Executer - {type.FullName}", type.FullName);
                        LoadExecuters(type);
                    }

                    foreach (var type in PluginLoader.GetTypes(typeof(IPluginSetup), pluginAssembly))
                    {
                        _logger.LogInformation("Loading plugin setup - {type.FullName}", type.FullName);
                        // Create an instance by passing constructor arguments
                        IPluginSetup? setupObj = Activator.CreateInstance(type, _builder) as IPluginSetup;
                        setupObj?.Configure();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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