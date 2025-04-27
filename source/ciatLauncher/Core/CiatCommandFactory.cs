
using System.CommandLine;
using System.Reflection;
using Ciat.CiatCommand;
using ciatLauncher.Core;

namespace Ciat.Core;

public class CiatCommandFactory
{
  private List<Command> _allCommands;
  private List<Type> _commandTypes;
  private CiatSettings _ciatSettings;

  public CiatCommandFactory(CiatSettings settings)
  {
    _ciatSettings = settings;
    LoadCiatCommands();
  }

  public Command GetCommand(string commandName)
  {
    Type? commandType = _commandTypes.FirstOrDefault(
      commandType => string.Equals(
        commandType.Name,
        commandName,
        StringComparison.InvariantCultureIgnoreCase)
    );

    return (commandType == null)
      ? throw new ArgumentException($"Command '{commandName}' not found.")
      : GetCiatCommand(commandType);
  }

  public List<Command> GetAllCommands() => _allCommands;


  private void LoadCiatCommands()
  {
    _commandTypes = [];

    _ciatSettings.Solution.Projects.SubProjects
     .Select(project => Assembly.Load(project.Name))
     .ToList()
     .ForEach(assembly =>
     {
       _commandTypes.AddRange(
          assembly.GetTypes()
            .Where(t => typeof(ICiatCommand).IsAssignableFrom(t) && !t.IsInterface));
     });

    _allCommands = [.. _commandTypes.Select(GetCiatCommand)];
  }

  private Command GetCiatCommand(Type ciatCommandType)
  {
    // get all the kind of properties
    SearchProperties(ciatCommandType.GetProperties(),
      out var regularProperties,
      out var nullableProperties,
      out var requiredProperties
    );

    // create each property as an option
    var options = new List<Option<dynamic>>();
    options.AddRange(regularProperties  .Select(property => new Option<dynamic>($"--{property.Name}") { IsRequired = false }));
    options.AddRange(nullableProperties .Select(property => new Option<dynamic>($"--{property.Name}") { IsRequired = false }));
    options.AddRange(requiredProperties .Select(property => new Option<dynamic>($"--{property.Name}") { IsRequired = true }));

    // create the command
    Command command = new Command(ciatCommandType.Name);

    // add the options to the command
    options.ForEach(command.Add);

    // set the handler for the command
    command.SetHandler((context) =>
    {
      Dictionary<string, string> commandArgumentsDictionary     = GetArgumentDictionary(context.ParseResult?.CommandResult.Children);
      Dictionary<string, object> ciatCommandConvertedProperties = GetConvertedProperties(regularProperties, nullableProperties, requiredProperties, commandArgumentsDictionary);
      ExecuteCiatCommand(ciatCommandType, ciatCommandConvertedProperties);
    });

    return command;
  }


  private static void SearchProperties(PropertyInfo[] properties, out IEnumerable<PropertyInfo> outRegularProperties, out IEnumerable<PropertyInfo> outNullableProperties, out IEnumerable<PropertyInfo> outRequiredProperties)
  {
    var regularProperties   = Enumerable.Empty<PropertyInfo>();
    var nullableProperties  = Enumerable.Empty<PropertyInfo>();
    var requiredProperties  = Enumerable.Empty<PropertyInfo>();

    // get all public properties
    var publicProperties = properties
      .Where(property => property.SetMethod?.IsPublic == true);

    // filter nullable properties
    nullableProperties = publicProperties
      .Where(property => new NullabilityInfoContext()
      .Create(property).WriteState == NullabilityState.Nullable);

    // filter required properties
    requiredProperties = publicProperties
      .Where(property => property.CustomAttributes
        .Any(attribute => attribute.AttributeType.FullName == "System.Runtime.CompilerServices.RequiredMemberAttribute"));

    // filter regular properties
    regularProperties = publicProperties
      .Where(property => !nullableProperties.Contains(property))
      .Where(property => !requiredProperties.Contains(property));

    outRequiredProperties = requiredProperties;
    outNullableProperties = nullableProperties;
    outRegularProperties  = regularProperties;
  }

  private static Dictionary<string, string> GetArgumentDictionary(IEnumerable<System.CommandLine.Parsing.SymbolResult> commandArguments)
  {
    return commandArguments.ToDictionary(
      arg => arg.Symbol.Name,
      arg => arg.Tokens.FirstOrDefault()?.Value ?? string.Empty
    );
  }

  private static Dictionary<string, object> GetConvertedProperties(IEnumerable<PropertyInfo> regularProperties, IEnumerable<PropertyInfo> nullableProperties, IEnumerable<PropertyInfo> requiredProperties, Dictionary<string, string> arguments)
  {
    // get the value for regular properties from the arguments
    Dictionary<string, string> argumentsForRegularProperties = arguments
      .Where(arg => regularProperties
        .Select(property => property.Name)
          .Contains(arg.Key))
      .ToDictionary(arg => arg.Key, arg => arg.Value);

    // get the value for nullable properties from the arguments
    Dictionary<string, string> argumentsForNullableProperties = arguments
      .Where(arg => nullableProperties
        .Select(property => property.Name)
          .Contains(arg.Key))
      .ToDictionary(arg => arg.Key, arg => arg.Value);

    // get the value for required properties from the arguments
    Dictionary<string, string> argumentsForRequiredProperties = arguments
      .Where(arg => requiredProperties
        .Select(property => property.Name)
          .Contains(arg.Key))
      .ToDictionary(arg => arg.Key, arg => arg.Value);

    // convert the values to the correct type
    Dictionary<string, object> convertedProperties = new[]
    {
      GetConvertedProperties(regularProperties, argumentsForRegularProperties),
      GetConvertedProperties(nullableProperties, argumentsForNullableProperties),
      GetConvertedProperties(requiredProperties, argumentsForRequiredProperties, required: true)
    }
    .SelectMany(dict => dict)
    .ToDictionary(pair => pair.Key, pair => pair.Value);

    return convertedProperties;
  }

  private static Dictionary<string, object> GetConvertedProperties(IEnumerable<PropertyInfo> requiredProperties, Dictionary<string, string> propertyValues, bool required = false)
  {
    Dictionary<string, object> convertedProperties = new Dictionary<string, object>();
    
    requiredProperties.ToList().ForEach(property => {
      Type propertyType = required
        ? property.PropertyType
        : Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
      object convertedValue = Convert.ChangeType(propertyValues[property.Name], propertyType);

      convertedProperties.Add(property.Name, convertedValue);
    });

    return convertedProperties;
  }

  private void ExecuteCiatCommand(Type commandType, Dictionary<string, object> propertyValues) {
    // create an instance of the command
    if (Activator.CreateInstance(commandType) is not ICiatCommand instance)
    {
      Console.WriteLine($"Error creating instance of '{commandType.Name}'.");
      return;
    }

    // set the properties of the command
    foreach (var property in commandType.GetProperties())
    {
      property.SetValue(instance, propertyValues[property.Name]);
    }

    // set the logger
    var logger = new ICiatLogger(commandType.Name);

    // execute the command
    instance.Execute(logger);
  }
}
