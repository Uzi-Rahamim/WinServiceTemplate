using Cocona;

namespace APIClient.commands.test;

[HasSubCommands(typeof(ShowServerCommand), "echo", Description = "Send echo message to the server")]
[HasSubCommands(typeof(MonitorKeepAliveCommand), "monitor", Description = "Monitor server's keep alive")]
[HasSubCommands(typeof(GetAPListCommand), "list", Description = "Get AP list")]

class DemoSubcommand { }
