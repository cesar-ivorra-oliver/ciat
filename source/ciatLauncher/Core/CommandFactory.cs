
using System.CommandLine;
using System.Reflection;
using Ciat.CiatCommand;

namespace CiatLauncher.Core
{
  public class CommandFactory
  {
    public List<Command> AvailableCommands { get; private set; }


    private CiatSettings _ciatSettings;

    private List<Type> _commandTypes;


    public CommandFactory()
    {
      LoadCiatSettings();
      LoadCiatCommands();
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

    private void LoadCiatSettings()
    {
      string settingsYamlPath = Path.Combine(AppContext.BaseDirectory, "ciatSettings.yaml");

      if(!File.Exists(settingsYamlPath))
      {
        Console.WriteLine("Settings file not found.");
        throw new FileNotFoundException("Settings file not found.");
      }

      _ciatSettings = new CiatSettings(settingsYamlPath);
    }

    private void LoadCiatCommands()
    {
      _commandTypes = new List<Type>();

      _ciatSettings.Solution.Projects.SubProjects
       .Select(project => Assembly.Load(project.Name))
       .ToList()
       .ForEach(assembly =>
       {
         _commandTypes.AddRange(
            assembly.GetTypes()
                    .Where(t => typeof(IciatCommand).IsAssignableFrom(t) && !t.IsInterface));
       });

      var commands      = _commandTypes.Select(GetCommand).ToList();
      AvailableCommands = commands;
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
      Dictionary<string, string> args = new Dictionary<string, string>();

      command.SetHandler((context) =>
      {
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
      (instance as IciatCommand).Execute();
    }
  }
}
