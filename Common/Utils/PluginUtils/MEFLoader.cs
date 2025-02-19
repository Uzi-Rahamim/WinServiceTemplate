namespace Utilities.PluginUtils
{
    class MEFLoader
    {
        //[ImportMany] // Import all plugins that implement IPlugin
        //public static IEnumerable<IPlugin> Plugins { get; set; }
        //public void Load(string pluginPath)
        //{
        //    if (Directory.Exists(pluginPath))
        //    {
        //        // Use DirectoryCatalog to scan for DLLs in the specified directory
        //        var catalog = new DirectoryCatalog(pluginPath);

        //        // Create the CompositionContainer to manage the parts (plugins)
        //        var container = new CompositionContainer(catalog);

        //        // Compose the parts
        //        container.ComposeParts();

        //        // Execute all discovered plugins
        //        foreach (var plugin in Plugins)
        //        {
        //            //plugin.Execute();
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("Plugin directory not found.");
        //    }
        //}
    }
}
