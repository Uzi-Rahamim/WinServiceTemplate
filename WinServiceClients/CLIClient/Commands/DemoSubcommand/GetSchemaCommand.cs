using ClientSDK.v1;
using Cocona;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Text;

namespace APIClient.commands.test;

public class GetSchemaCommand
{
    [Command]
    public static async Task GetSchema()
    {
        using (var channel = new ClientChannel(LoggerFactory.Create(builder => builder.AddSerilog())))
        {
            if (!await channel.Connect())
            {
                Console.WriteLine("Failed to connect to server");
                return;
            }

            StringBuilder sb = new StringBuilder();
            var demoAPI = new DemoApi(channel);
            var schemaList = demoAPI.GetSchema();
            await foreach (var schema in schemaList)
            {
                sb.AppendLine(schema);
                //Log.Information($"{schema}");
            }
            Log.Information($"{sb.ToString()}");
        } 
    }
}