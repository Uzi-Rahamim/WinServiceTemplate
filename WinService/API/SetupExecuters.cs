using Intel.IntelConnect.WindowsService.API.Executers;
using Intel.IntelConnect.IPC.Channel;
using Intel.IntelConnect.IPC.CommonTypes;
using Intel.IntelConnect.IPC.CommonTypes.InternalMassages.Executers;
using Intel.IntelConnect.IPC.Executer;
using Intel.IntelConnect.IPC.Listeners;
using Intel.IntelConnect.IPC.Utils;
using Intel.IntelConnect.IPCCommon.Consts;
using Intel.IntelConnect.IPC.Events.Service;

namespace Intel.IntelConnect.WindowsService.API;

internal class SetupExecuters
{
    private IServiceCollection _serviceCollection ;

    public void Configure(ServiceProvider globalServiceProvider)
    {
        RegisterRequest<SchemaRequestExecuter>(FrameworkMethodName.RequestSchema, SchemaRequestExecuter.GetSchema);
        RegisterRequest<OpenSessionRequestExecuter>(FrameworkMethodName.OpenSession, OpenSessionRequestExecuter.GetSchema);
        RegisterRequest<EchoRequestExecuter>(FrameworkMethodName.Echo, EchoRequestExecuter.GetSchema);

        _serviceCollection.AddTransient<ISequenceGenerator, SequenceGenerator>();
        //_serviceCollection.AddSingleton<IEventDispatcher, EventDispatcher>();
        _serviceCollection.AddSingleton(sp => globalServiceProvider.GetRequiredService<IEventDispatcher>());
        _serviceCollection.AddSingleton<IExecuterManager, ExecuterManager>();
        _serviceCollection.AddTransient<IServerMessageListener, ServerMessageListener>();
        _serviceCollection.AddSingleton<IServerChannelFactory>((provider)=>new ServerChannelFactory(
            provider.GetRequiredService<ILogger<ClientPipeChannel>>(), 
            PipeApiConsts.PipeName));
        _serviceCollection.AddSingleton<ServerIncomingConnectionListener>();
    }

    public static SetupExecuters Create(IServiceCollection serviceCollection)
    {
        return new SetupExecuters(serviceCollection);
    }

    private SetupExecuters(IServiceCollection serviceCollection)
    {
        this._serviceCollection = serviceCollection;
    }

    private void RegisterRequest<T>(string messageType,Func<string > getSchema) where T : class, IRequestExecuter
    {
        ExecuterRegister.RegisterSchema(_serviceCollection, messageType, getSchema);
        ExecuterRegister.RegisterExecuter<T>(_serviceCollection, messageType);
    }

}
