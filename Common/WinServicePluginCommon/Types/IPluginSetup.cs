using Microsoft.Extensions.DependencyInjection;

namespace WinService.Plugin.Common
{
    public interface IPluginSetup
    {
        Task<bool> Start();
        Task<bool> Stop();

        Version? GetVersion();

        Task Initialize(IServiceCollection serviceCollection);
    }
}
