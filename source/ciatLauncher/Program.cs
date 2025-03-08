
using System.CommandLine;
using Ciat.Core;

namespace Ciat.Launcher;

class Program
{
  static int Main(string[] args)
  {
    // core 
    var settings  = new CiatSettings("ciatSettings.yaml");
    var factory   = new CommandFactory(settings);

    // command line root
    var rootCommand = new RootCommand("Cesar Ivorra automation tool.");

    // add all available commands
    factory.GetAllCommands().ForEach(rootCommand.AddCommand);

    // execute the command
    return rootCommand.Invoke(args);
  }
}