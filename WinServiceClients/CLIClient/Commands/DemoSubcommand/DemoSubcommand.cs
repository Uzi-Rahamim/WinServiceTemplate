using Cocona;

namespace APIClient.commands.test;

[HasSubCommands(typeof(SendEchoCommand), "echo", Description = "Send echo message to the server")]
[HasSubCommands(typeof(MonitorKeepAliveCommand), "monitor", Description = "Monitor server's keep alive")]
[HasSubCommands(typeof(GetAPListCommand), "list", Description = "Get AP list")]
[HasSubCommands(typeof(GetSchemaCommand), "schema", Description = "Get the schema list of all server command")]
[HasSubCommands(typeof(GetPlatfrmListCommand), "platform", Description = "Get the ....")]


class DemoSubcommand { }
