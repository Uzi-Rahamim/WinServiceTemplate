using ClientCLI.Commands.PluginA.Commands;
using Cocona;

namespace APIClient.commands.test;

[HasSubCommands(typeof(PluginB_SendEchoCommand), "echo", Description = "Send echo message to the plugin")]
class PluginB_Subcommand { }
