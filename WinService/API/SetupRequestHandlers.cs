using App.WindowsService.API.Requests;
using AsyncPipeTransport.Channel;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.ServerHandlers;
using CommunicationMessages;

namespace App.WindowsService.API;

internal class SetupRequestHandlers
{
    private IHostApplicationBuilder _builder;
    
    public void Configure()
    {
        RegisterRequest<SchemaRequestHandler>(FrameworkMessageTypes.RequestSchema, SchemaRequestHandler.GetSchema);
        RegisterRequest<OpenSessionRequestHandler>(FrameworkMessageTypes.OpenSession, OpenSessionRequestHandler.GetSchema);
        RegisterRequest<EchoRequestHandler>(MessageType.Echo, EchoRequestHandler.GetSchema);
        RegisterRequest<GetAPListRequestHandler>(MessageType.APList,GetAPListRequestHandler.GetSchema);

        
        _builder.Services.AddTransient<ISequenceGenerator, SequenceGenerator>();
        _builder.Services.AddSingleton<IClientsManager, ClientsManager>();
        _builder.Services.AddSingleton<IServerRequestsManager, ServerRequestsManager>();
        _builder.Services.AddSingleton<IServerChannelFactory>((provider)=>new ServerChannelFactory(PipeApiConsts.PipeName));
       
        _builder.Services.AddSingleton<ServerIncomingConnectionListener>();
    }

    public static SetupRequestHandlers Create(IHostApplicationBuilder builder)
    {
        return new SetupRequestHandlers(builder);
    }

    private SetupRequestHandlers(IHostApplicationBuilder builder)
    {
        this._builder = builder;
    }

    private void RegisterRequest<T>(string messageType,Func<string > getSchema) where T : class, IRequestExecuter
    {
        _builder.Services.AddTransient<IRequestSchemaProvider>(serviceProvider =>
        {
            return new RequestSchemaProvider(messageType, getSchema);
        });

        // Register the IRequestExecuter implementation as Transient
        _builder.Services.AddTransient<T>();
        _builder.Services.AddSingleton<IRequestExecuterFactory>(serviceProvider =>
        {
            var factory = () => serviceProvider.GetRequiredService<T>();
            return new RequestExecuterFactory(messageType, factory);
        });
    }

}
