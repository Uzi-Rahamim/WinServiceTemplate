using Intel.IntelConnect.ClientSDK.v1;
using Cocona;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Text;

namespace APIClient.commands.test;

public class Service_GetSchemaCommand
{
    [Command]
    public static async Task Service_GetSchemaAsync()
    {
        Log.Information($"Service_GetSchema");
        using (var channel = new SdkClientChannel(LoggerFactory.Create(builder => builder.AddSerilog())))
        {
            if (!await channel.ConnectAsync())
            {
                Log.Error("Failed to connect to server");
                return;
            }

            StringBuilder sb = new StringBuilder();
            var demoAPI = new WinServiceApi(channel);
            var schemaList = demoAPI.GetSchemaAsync();
            await foreach (var schema in schemaList)
            {
                sb.AppendLine(schema);
                //Log.Information($"{schema}");
            }
            Log.Information($"{sb.ToString()}");
        } 
    }
}