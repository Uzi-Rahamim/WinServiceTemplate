﻿using App.WindowsService.API.Executers;
using AsyncPipeTransport.Channel;
using AsyncPipeTransport.Clients;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.CommonTypes.InternalMassages.Executers;
using AsyncPipeTransport.Executer;
using AsyncPipeTransport.Listeners;
using AsyncPipeTransport.Utils;
using CommTypes.Consts;
using CommTypes.Massages;
using Microsoft.Extensions.DependencyInjection;

namespace App.WindowsService.API;

internal class SetupExecuters
{
    private IServiceCollection _serviceCollection ;

    public void Configure(ServiceProvider globalServiceProvider)
    {
        RegisterRequest<SchemaRequestExecuter>(FrameworkMessageTypes.RequestSchema, SchemaRequestExecuter.GetSchema);
        RegisterRequest<OpenSessionRequestExecuter>(FrameworkMessageTypes.OpenSession, OpenSessionRequestExecuter.GetSchema);
        RegisterRequest<EchoRequestExecuter>(FrameworkMessageTypes.Echo, EchoRequestExecuter.GetSchema);

        _serviceCollection.AddTransient<ISequenceGenerator, SequenceGenerator>();
        //_serviceCollection.AddSingleton<IEventDispatcher, EventDispatcher>();
        _serviceCollection.AddSingleton(sp => globalServiceProvider.GetRequiredService<IEventDispatcher>());
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
