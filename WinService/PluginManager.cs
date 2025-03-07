﻿using Intel.IntelConnect.PluginCommon.v1;

namespace Intel.IntelConnect.WindowsService
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

        public async Task StartPluginsAsync()
        {
            var tasks = _plugins.Select(plugin => plugin.Start()).ToArray();
            await Task.WhenAll(tasks);
        }

        public async Task ShutdownPluginsAsync()
        {
            await StopPluginsAsync();
            var tasks = _plugins.Select(plugin => plugin.Shutdown()).ToArray();
            await Task.WhenAll(tasks);
        }

        private async Task StopPluginsAsync()
        {
            var tasks = _plugins.Select(plugin => plugin.Stop()).ToArray();
            await Task.WhenAll(tasks);
        }
    }
}
