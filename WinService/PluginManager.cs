using WinService.Plugin.Common;

namespace App.WindowsService
{
    public class PluginManager
    {
        private readonly List<IPluginSetup> _plugins = new();
        private static PluginManager? _instance = null;
        private PluginManager()
        {

        }

        public static PluginManager GetInstance()   
        {
            if (_instance == null)
            {
                _instance = new PluginManager();
            }
            return _instance;
        }

        public void AddPlugin(IPluginSetup pluginSetup)
        {
            _plugins.Add(pluginSetup);
        }

        public async Task StartPlugins()
        {
            var tasks = _plugins.Select(plugin => plugin.Start()).ToArray();
            await Task.WhenAll(tasks);
        }

        public async Task ShutdownPlugins()
        {
            await StopPlugins();
            var tasks = _plugins.Select(plugin => plugin.Shutdown()).ToArray();
            await Task.WhenAll(tasks);
        }

        private async Task StopPlugins()
        {
            var tasks = _plugins.Select(plugin => plugin.Stop()).ToArray();
            await Task.WhenAll(tasks);
        }
    }
}
