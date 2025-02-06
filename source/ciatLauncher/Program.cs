
using ciat.Core;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.CommandLine;
using System.Reflection;

namespace ciat;

class Program
{
  static int Main(string[] args)
  {
    var factory = new CommandFactory();
    var rootCommand = new RootCommand("Cesar Ivorra automation tool.");


    if (args.Length == 0)
    {
      Console.WriteLine("No command provided.");
      factory.AvailableCommands.ForEach(rootCommand.AddCommand);
      return rootCommand.Invoke(["--help"]);
    }

    // add the requested command
    rootCommand.AddCommand(factory.GetCommand(args[0]));

    // execute the command
    return rootCommand.Invoke(args);
  }
}