using App.WindowsService.API.Executers;
using AsyncPipeTransport.Channel;
using AsyncPipeTransport.Clients;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Executer;
using AsyncPipeTransport.Listeners;
using CommTypes.Consts;
using CommTypes.Massages;

namespace App.WindowsService.API;

internal class SetupExecuters
{
    private IHostApplicationBuilder _builder;
    
    public void Configure()
    {
        RegisterRequest<SchemaRequestHandler>(FrameworkMessageTypes.RequestSchema, SchemaRequestHandler.GetSchema);
        RegisterRequest<OpenSessionRequestExecuter>(FrameworkMessageTypes.OpenSession, OpenSessionRequestExecuter.GetSchema);
        //RegisterRequest<EchoRequestHandler>(MessageType.Echo, EchoRequestHandler.GetSchema);
        RegisterRequest<GetAPListRequestExecuter>(MessageType.APList,GetAPListRequestExecuter.GetSchema);

        
        _builder.Services.AddTransient<ISequenceGenerator, SequenceGenerator>();
        _builder.Services.AddSingleton<IClientsManager, ClientsManager>();
        _builder.Services.AddSingleton<IExecuterManager, ExecuterManager>();
        _builder.Services.AddSingleton<IServerMessageListener, ServerMessageListener>();
        _builder.Services.AddSingleton<IServerChannelFactory>((provider)=>new ServerChannelFactory(PipeApiConsts.PipeName));
       
        _builder.Services.AddSingleton<ServerIncomingConnectionListener>();
    }

    public static SetupExecuters Create(IHostApplicationBuilder builder)
    {
        return new SetupExecuters(builder);
    }

    private SetupExecuters(IHostApplicationBuilder builder)
    {
        this._builder = builder;
    }

    private void RegisterRequest<T>(string messageType,Func<string > getSchema) where T : class, IRequestExecuter
    {
        ExecuterRegister.RegisterRequest<T>(_builder, messageType, getSchema);
    }

}
