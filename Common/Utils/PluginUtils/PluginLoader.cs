using System.Reflection;
using System.Runtime.Loader;

namespace Utilities.PluginUtils
{
    public class PluginLoader
    {
       
        public static IEnumerable<Assembly> LoadPlugin(string pluginFile)
        {
            var loadContext = new AssemblyLoadContext("PluginLoadContext" + Guid.NewGuid(), isCollectible: true);
           
            var pluginAssembly = loadContext.LoadFromAssemblyPath(pluginFile);
            var path = Path.GetDirectoryName(pluginFile);
            if (path == null) 
                yield break;

            foreach (var dependentAssembly in GetReferencedAssemblies(path, pluginAssembly))
            {
                loadContext.LoadFromAssemblyPath(dependentAssembly);
            }

            yield return pluginAssembly;
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

        private static IEnumerable<string> GetReferencedAssemblies(string assemblyPath, Assembly assembly)
        {
            //Alerdy loaded assemblies
            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(asm => !asm.IsDynamic && File.Exists(asm.Location));
            var currentAssemblyMap = new ConcurrentDictionary<string, Assembly>(currentAssemblies.ToDictionary(asm => asm.FullName ?? string.Empty));

            //get the list of referenced assemblies
            AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
            foreach (var referencedAssembly in referencedAssemblies)
            {
                if (currentAssemblyMap.ContainsKey(referencedAssembly.FullName))
                    continue;

                var dependentAssemblyFileName = Directory.GetFiles(assemblyPath, referencedAssembly.Name + ".dll").FirstOrDefault();
                if (dependentAssemblyFileName is not null)
                {
                    yield return dependentAssemblyFileName;
                }
            }

        }

        //public static IEnumerable<Assembly> LoadPlugin(string assemblyPath, string assemblyExtention)
        //{
        //    var loadContext = new AssemblyLoadContext("PluginLoadContext"+Guid.NewGuid(), isCollectible: true);
        //    var pluginFiles = Directory.GetFiles(assemblyPath, assemblyExtention).ToList();
        //    foreach (var pluginFile in pluginFiles)
        //    {
        //        var pluginAssembly = loadContext.LoadFromAssemblyPath(pluginFile);
        //        foreach (var dependentAssembly in GetReferencedAssemblies(assemblyPath, pluginAssembly))
        //        {
        //           loadContext.LoadFromAssemblyPath(dependentAssembly);
        //        }

        //        yield return pluginAssembly;
        //    }
        //}

        //public static IEnumerable<string> GetAllDlls(string assemblyPath)
        //{
        //    //get the list of referenced assemblies
        //    var files = Directory.GetFiles(assemblyPath,"*.dll");
        //    foreach (var file in files)
        //    {
        //         yield return file;
        //    }
        //}
    }
}



