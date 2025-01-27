using App.WindowsService.API.Requests;
using AsyncPipeTransport.ServerHandlers;
using System.Reflection;
using System.Runtime.Loader;
using CommunicationMessages;
using AsyncPipeTransport.CommonTypes;

namespace App.WindowsService.API
{
    internal class PluginManager
    {

        private IHostApplicationBuilder _builder;
        private PluginManager(IHostApplicationBuilder builder)
        {
            _builder = builder;
        }
        public void RegisterRequest<T>(string messageType, Func<string> getSchema) where T : class, IRequestExecuter
        {
            _builder.Services.AddTransient<IRequestSchemaProvider>(serviceProvider =>
            {
                return new RequestSchemaProvider(messageType, getSchema);
            });

            // Register the IRequestExecuter implementation as Transient
            _builder.Services.AddTransient<T>();
            _builder.Services.AddSingleton<IRequestExecuterFactory>(serviceProvider =>
            {
                var factory = () => serviceProvider.GetRequiredService<T>();
                return new RequestExecuterFactory(messageType, factory);
            });
        }


        public static PluginManager Create(IHostApplicationBuilder builder)
        {
            return new PluginManager(builder);
        }


        public void LoadPlugins()
        {
            try
            {
                var exePath = @"C:\Repo\MyRepos\WinServiceTemplate\ServerPlugin\bin\x64\Debug\net8.0";
                //var exePath = Assembly.GetExecutingAssembly().Location;
                var files = Directory.GetFiles(exePath, "*ExecuterPlugin.dll").ToList();
                var types = files.SelectMany(pluginPath =>
                {
                    var pluginAssembly = LoadPlugin(pluginPath);
                    return GetTypes<IRequestExecuter>(pluginAssembly);
                }).ToList();

                foreach (var type in types)
                {
                    //Get static Schema from plugin
                    var schema = (string)(type.GetMethod("Plugin_GetSchema")?.Invoke(null, null) ?? "missing Schema");

                    //Get static MessageType from plugin
                    var messageType = (string?) type.GetMethod("Plugin_GetMessageType")?.Invoke(null, null);
                    if (messageType is null)
                    {
                        throw new ApplicationException("Plugin_GetMessageType is missing");
                    }

                    var registerMethod = typeof(PluginManager).GetMethod("RegisterRequest");
                    // Make the method generic by passing the Type
                    var genericSetMethod = registerMethod?.MakeGenericMethod(type);
                    

                    // Invoke the method
                    genericSetMethod?.Invoke(this, new object[] { messageType, () => schema });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        private Assembly LoadPlugin(string assemblyPath)
        {
            var loadContext = new AssemblyLoadContext("PluginLoadContext", isCollectible: true);
            var assembly = loadContext.LoadFromAssemblyPath(assemblyPath);

            return assembly;

        }

        private IEnumerable<Type> GetTypes<T>(Assembly assembly)
        {
            var found = false;
            foreach (var type in assembly.GetTypes())
            {
                if (type is not null &&
                    type.GetInterfaces().Any(intf => intf.FullName?.Contains(nameof(T)) ?? false))
                {

                    found = true;
                    yield return type;
                }
            }
            if (!found)
            {
                throw new ApplicationException(
                    $"{assembly} from {assembly.Location} doesn't implements plugin interface");
            }
        }
    }
}