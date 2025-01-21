using App.WindowsService.API.Requests;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.ServerHandlers;
using CommTypes;
using CommunicationMessages;

namespace App.WindowsService.API;

internal class SetupRequestHandlers
{
    private IHostApplicationBuilder _builder;
    
    public void Configure()
    {
        RegisterRequest<OpenSessionRequestHandler>(MessageType.OpenSession);
        RegisterRequest<EchoRequestHandler>(MessageType.Echo);
        RegisterRequest<GetAPListRequestHandler>(MessageType.APList);

        
        _builder.Services.AddTransient<ISequenceGenerator, SequenceGenerator>();
        _builder.Services.AddSingleton<IClientsBroadcast, ClientsBroadcast>();
        _builder.Services.AddSingleton<IServerRequestHandler, ServerRequestHandler>();
        _builder.Services.AddSingleton<ServerRequestListener>();
    }

    public static SetupRequestHandlers Create(IHostApplicationBuilder builder)
    {
        return new SetupRequestHandlers(builder);
    }

    private SetupRequestHandlers(IHostApplicationBuilder builder)
    {
        this._builder = builder;
    }

    private void RegisterRequest<T>(MessageType messageType) where T : class, IRequestCommand
    {
        // Register the IRequestHandler implementation as Transient
        _builder.Services.AddTransient<T>();
        _builder.Services.AddSingleton<IRequestCommandFactory>(serviceProvider =>
        {
            var factory = () => serviceProvider.GetRequiredService<T>();
            return new RequestCommandFactory((Opcode)messageType, factory);
        });
    }

}
