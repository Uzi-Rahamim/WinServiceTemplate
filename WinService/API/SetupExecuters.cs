using App.WindowsService.API.Executers;
using AsyncPipeTransport.Channel;
using AsyncPipeTransport.Clients;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Executer;
using AsyncPipeTransport.Listeners;
using CommTypes.Consts;
using CommTypes.Massages;
using Microsoft.Extensions.DependencyInjection;

namespace App.WindowsService.API;

internal class SetupExecuters
{
    private IServiceCollection _serviceCollection ;
    
    public void Configure()
    {
        RegisterRequest<SchemaRequestExecuter>(FrameworkMessageTypes.RequestSchema, SchemaRequestExecuter.GetSchema);
        RegisterRequest<OpenSessionRequestExecuter>(FrameworkMessageTypes.OpenSession, OpenSessionRequestExecuter.GetSchema);
        RegisterRequest<EchoRequestExecuter>(MessageType.Echo, EchoRequestExecuter.GetSchema);
        RegisterRequest<GetAPListRequestExecuter>(MessageType.APList,GetAPListRequestExecuter.GetSchema);

        _serviceCollection.AddTransient<ISequenceGenerator, SequenceGenerator>();
        _serviceCollection.AddSingleton<IClientsManager, ClientsManager>();
        _serviceCollection.AddSingleton<IExecuterManager, ExecuterManager>();
        _serviceCollection.AddSingleton<IServerMessageListener, ServerMessageListener>();
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
