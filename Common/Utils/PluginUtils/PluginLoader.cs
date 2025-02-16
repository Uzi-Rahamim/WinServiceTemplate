using System.Reflection;
using System.Runtime.Loader;

namespace Utilities.PluginUtils
{
    public class PluginLoader
    {
   
        public static IEnumerable<AssemblyName> GetReferencedAssemblies(Assembly assembly)
        {
            
            //get the list of referenced assemblies
            AssemblyName[] referencedAssemblyNames = assembly.GetReferencedAssemblies();
            foreach (var assemblyName in referencedAssemblyNames)
            {
                yield return assemblyName;
            }
        }

        public static void LoadAssemblyReference(AssemblyLoadContext loadContext, string dllPath, Assembly assembly)
        {
            foreach (var assemblyName in GetReferencedAssemblies(assembly))
            {
                try
                {
                    try
                    {
                        // Check if the assembly is already loaded in this context:
                        var existingAssembly = loadContext.LoadFromAssemblyName(assemblyName);
                        if (existingAssembly != null)
                        {
                            //Console.WriteLine($"Skip - Already loaded {assemblyName}");
                            continue;
                        }
                    }
                    catch (Exception)
                    {
                        //Ignore
                    }
                  

                    var assemblyPath = Path.Combine(dllPath, assemblyName.Name + ".dll");
                    //if (!File.Exists(assemblyPath))
                    //    assemblyPath = Path.Combine(@"C:\Windows\Microsoft.NET\Framework64\v4.0.30319", assemblyName.Name + ".dll");
                    if (!File.Exists(assemblyPath))
                    {
                        Console.WriteLine($"File not found: {assemblyName}");
                        continue;
                    }
                    var dependentAssembly = loadContext.LoadFromAssemblyPath(assemblyPath);
                    Console.WriteLine($"loading {dependentAssembly} {assemblyPath}");
                    LoadAssemblyReference(loadContext, dllPath, dependentAssembly);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception loading {ex}");
                }
            }

        }

        public static IEnumerable<Assembly> LoadPlugin(string assemblyPath, string assemblyExtention)
        {  
            //// isCollectible allows unloading later
            var loadContext = new AssemblyLoadContext("PluginLoadContext"+Guid.NewGuid(), isCollectible: true);
            var pluginFiles = Directory.GetFiles(assemblyPath, assemblyExtention).ToList();
            foreach (var pluginFile in pluginFiles)
            {
                var pluginAssembly = loadContext.LoadFromAssemblyPath(pluginFile);
                Console.WriteLine($".....loading {pluginAssembly}");
                LoadAssemblyReference(loadContext, assemblyPath, pluginAssembly);
             
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



