using Microsoft.Extensions.DependencyInjection;

namespace Intel.IntelConnect.PluginCommon
{
    public interface IPluginSetup
    {
        Task<bool> Start();
        Task<bool> Stop();

        Version? GetVersion();

        Task Initialize(IServiceCollection serviceCollection);
        Task Shutdown();
    }
}
