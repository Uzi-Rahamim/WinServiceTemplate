﻿using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Executer;

namespace App.WindowsService.API
{
    internal class ExecuterRegister
    {
        public static void RegisterSchema(
            IServiceCollection hostServiceCollection,
            string messageType,
            Func<string> getSchema) 
        {
            hostServiceCollection.AddTransient<IRequestSchemaProvider>(serviceProvider =>
            {
                return new RequestSchemaProvider(messageType, getSchema);
            });
        }

        public static void RegisterExecuter<T>(
            IServiceCollection pluginsServiceCollection,
            string messageType) where T : class, IRequestExecuter
        {
            // Register the IRequestExecuter implementation as Transient
            pluginsServiceCollection.AddTransient<T>();
            pluginsServiceCollection.AddSingleton<IRequestExecuterFactory>(serviceProvider =>
            {
                var factory = () => serviceProvider.GetRequiredService<T>();
                return new RequestExecuterFactory(messageType, factory);
            });
        }

        public static void RegisterPluginExecuter<T>(
            IServiceCollection mainCollection,
            IServiceCollection pluginsCollection,
            string messageType) where T : class, IRequestExecuter
        {
            // Register the IRequestExecuter implementation as Transient
            pluginsCollection.AddTransient<T>();
            mainCollection.AddSingleton<IRequestExecuterFactory>(serviceProvider =>
            {
                var pluginProvider = pluginsCollection.BuildServiceProvider();
                var factory = () => pluginProvider.GetRequiredService<T>();
                return new RequestExecuterFactory(messageType, factory);
            });
        }
    }
}
