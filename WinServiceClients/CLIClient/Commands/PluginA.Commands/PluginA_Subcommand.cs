using ClientCLI.Commands.PluginA.Commands;
using Cocona;

namespace APIClient.commands.test;


[HasSubCommands(typeof(PluginA_GetAPListCommand), "list", Description = "Get AP list")]
[HasSubCommands(typeof(PluginA_SendEchoCommand), "echo", Description = "Send echo message to the plugin")]
class PluginA_Subcommand { }
