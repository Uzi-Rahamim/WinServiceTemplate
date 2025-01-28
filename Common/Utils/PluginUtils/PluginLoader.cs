using System.Reflection;
using System.Runtime.Loader;

namespace Utilities.PluginUtils
{
    public class PluginLoader
    {
        //public void LoadPlugins()
        //{
        //    try
        //    {
        //        var assemblyPath = @"C:\Repo\MyRepos\WinServiceTemplate\ServerPlugin\bin\x64\Debug\net8.0\";
        //        ////var exePath = Assembly.GetExecutingAssembly().Location;
        //        //var files = Directory.GetFiles(exePath, "*ExecuterPlugin.dll").ToList();
        //        //var types = files.SelectMany(pluginPath =>
        //        //{
        //        //    var pluginAssembly = LoadPlugin(pluginPath);
        //        //    return GetTypes(typeof(IRequestExecuter), pluginAssembly);
        //        //}).ToList();

        //        foreach (var pluginAssembly in LoadPlugin(assemblyPath, "*ExecuterPlugin.dll"))
        //        {
        //            foreach (var type in GetTypes(typeof(IRequestExecuter), pluginAssembly){

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //}
        public static IEnumerable<Assembly> LoadPlugin(string assemblyPath, string assemblyExtention)
        {
            var loadContext = new AssemblyLoadContext("PluginLoadContext", isCollectible: true);

            var files = Directory.GetFiles(assemblyPath, assemblyExtention).ToList();
            foreach (var file in files)
            {
                var pluginAssembly = loadContext.LoadFromAssemblyPath(file);
                yield return pluginAssembly;
            }
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
    }
}
