using Microsoft.Extensions.DependencyInjection;

namespace WinService.Plugin.Common
{
    public interface IPluginSetup
    {
        bool Start();
        void Initialize(IServiceCollection serviceCollection);
    }
}
