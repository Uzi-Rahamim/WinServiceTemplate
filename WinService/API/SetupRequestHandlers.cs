using App.WindowsService.API.Requests;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.ServerScheduler;
using CommunicationMessages;

namespace App.WindowsService.API;

internal class SetupRequestHandlers
{
    private IHostApplicationBuilder _builder;
    
    public void Configure()
    {
        RegisterRequest<EchoRequestHandler>(MessageType.Echo);
        RegisterRequest<GetAPListRequestHandler>(MessageType.APList);
    }

    public static SetupRequestHandlers Create(IHostApplicationBuilder builder)
    {
        return new SetupRequestHandlers(builder);
    }

    private SetupRequestHandlers(IHostApplicationBuilder builder)
    {
        this._builder = builder;
    }

    private void RegisterRequest<T>( MessageType messageType) where T : class, IRequestHandler
    {
        // Register the IRequestHandler implementation as Transient
        _builder.Services.AddTransient<T>();
        _builder.Services.AddSingleton<IRequestHandlerBuilder>(serviceProvider =>
        {
            var factory = () => serviceProvider.GetRequiredService<T>();
            return new RequestHandlerBuilder((Opcode)messageType, factory);
        });
    }

}
