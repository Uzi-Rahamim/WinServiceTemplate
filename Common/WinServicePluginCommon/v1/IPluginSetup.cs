using Microsoft.Extensions.DependencyInjection;

namespace Intel.IntelConnect.PluginCommon.v1
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
