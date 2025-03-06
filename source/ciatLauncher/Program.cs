
using System.CommandLine;
using Ciat.Core;

namespace Ciat.Launcher;

class Program
{
  static int Main(string[] args)
  {
    var factory = new CommandFactory("ciatSettings.yaml");
    var rootCommand = new RootCommand("Cesar Ivorra automation tool.");

    // add all available commands
    factory.GetAllCommands().ForEach(rootCommand.AddCommand);

    // execute the command
    return rootCommand.Invoke(args);
  }
}