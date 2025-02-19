using ClientCLI.Commands.PluginA.Commands;
using Cocona;

namespace APIClient.commands.test;

[HasSubCommands(typeof(Service_SendEchoCommand), "echo", Description = "Send echo message to the server")]
[HasSubCommands(typeof(Service_MonitorKeepAliveCommand), "monitor", Description = "Monitor server's keep alive")]
[HasSubCommands(typeof(Service_GetSchemaCommand), "schema", Description = "Get the schema list of all server command")]

class Service_SubCommand { }
