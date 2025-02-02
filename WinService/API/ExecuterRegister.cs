using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Executer;

namespace App.WindowsService.API
{
    internal class ExecuterRegister
    {
        public static void RegisterRequest<T>(IHostApplicationBuilder builder, string messageType, Func<string> getSchema) where T : class, IRequestExecuter
        {   
            builder.Services.AddTransient<IRequestSchemaProvider>(serviceProvider =>
            {
                return new RequestSchemaProvider(messageType, getSchema);
            });

            // Register the IRequestExecuter implementation as Transient
            builder.Services.AddTransient<T>();
            builder.Services.AddSingleton<IRequestExecuterFactory>(serviceProvider =>
            {
                var factory = () => serviceProvider.GetRequiredService<T>();
                return new RequestExecuterFactory(messageType, factory);
            });
        }
    }
}
