
using ciatCommands.Commands;
using System.CommandLine;
using System.Reflection;

namespace ciat.Core
{
  public class CommandFactory
  {
    public List<Command> AvailableCommands { get; private set; }

    private List<Type> _commandTypes;

    public CommandFactory() {
      LoadCommands();
    }

    private void LoadCommands()
    {
      _commandTypes = Assembly.GetAssembly(typeof(ICommand))
                        .GetTypes()
                        .Where(type => !type.IsInterface || !type.IsAbstract)
                        .ToList();

      var commands      = _commandTypes.Select(type => GetCommand(type)).ToList();
      AvailableCommands = commands;
    }

    public Command GetCommand(string commandName)
    {
      Type commandType = _commandTypes.FirstOrDefault(commandType => string.Equals(
                                            commandType.Name,
                                            commandName,
                                            StringComparison.InvariantCultureIgnoreCase)
      );

      return GetCommand(commandType);
    }

    private Command GetCommand(Type commandType)
    {
      // get all public properties with a setter
      List<Option<dynamic>> comandOptions = commandType.GetProperties()
                                              .Where(property => property.SetMethod?.IsPublic == true)
                                              .Select(property => new Option<dynamic>($"--{property.Name}"))
                                              .ToList();

      // create the command
      Command command = new Command(commandType.Name);

      // add the options to the command
      comandOptions.ForEach(option => command.Add(option));

      command.SetHandler((context) =>
      {

        Dictionary<string, string> args = new Dictionary<string, string>();

        foreach (var arg in context.ParseResult.CommandResult.Children)
        {
          var optionName = arg.Symbol.Name;
          var optionValue = arg.Tokens.FirstOrDefault()?.Value; // TODO: allow multiple values in the future

          args.Add(optionName, optionValue);
        }

        ExecuteCommand(commandType, args);
      });


      return command;
    }

    private void ExecuteCommand(Type commandType, Dictionary<string, string> propertyValues) {
      // create an instance of the command
      var instance = Activator.CreateInstance(commandType);

      if (instance == null) {
        Console.WriteLine($"Error creating instance of {commandType.Name}");
        return;
      }

      // set the properties of the command
      foreach (var property in commandType.GetProperties())
      {
        if (propertyValues.TryGetValue(property.Name, out string propertyValue)) {
          object convertedValue = Convert.ChangeType(propertyValue, property.PropertyType);
          property.SetValue(instance, convertedValue);
        }
      }

      // execute the command
      (instance as ICommand).Execute();
    }
  }
}
