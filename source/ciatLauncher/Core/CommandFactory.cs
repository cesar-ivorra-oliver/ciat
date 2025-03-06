
using System.CommandLine;
using System.Reflection;
using Ciat.CiatCommand;

namespace Ciat.Core;
public class CommandFactory
{
  private List<Command> _allCommands;
  private List<Type> _commandTypes;
  private CiatSettings _ciatSettings;

  public CommandFactory(string ciatSettingsFilePath)
  {
    LoadCiatSettings(ciatSettingsFilePath);
    LoadCiatCommands();
  }

  public Command GetCommand(string commandName)
  {
    Type commandType = _commandTypes.FirstOrDefault(
      commandType => string.Equals(
        commandType.Name,
        commandName,
        StringComparison.InvariantCultureIgnoreCase)
    );

    return GetCommand(commandType);
  }

  public List<Command> GetAllCommands() => _allCommands;


  private void LoadCiatSettings(string ciatSettingsFilePath)
  {
    string settingsYamlPath = Path.Combine(AppContext.BaseDirectory, ciatSettingsFilePath);

    if(!File.Exists(settingsYamlPath))
    {
      Console.WriteLine("Settings file not found.");
      throw new FileNotFoundException("Settings file not found.");
    }

    _ciatSettings = new CiatSettings(settingsYamlPath);
  }

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

    _allCommands = [.. _commandTypes.Select(GetCommand)];
  }

  private Command GetCommand(Type ciatCommandType)
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
      Dictionary<string, string> args = GetArgumentDictionary(context.ParseResult?.CommandResult.Children);
      Dictionary<string, object> ciatCommandConvertedProperties = GetConvertedProperties(regularProperties, nullableProperties, requiredProperties, args);
      ExecuteCiatCommand(ciatCommandType, ciatCommandConvertedProperties);
    });

    return command;
  }


  private static void SearchProperties(PropertyInfo[] properties, out List<PropertyInfo> outRegularProperties, out List<PropertyInfo> outNullableProperties, out List<PropertyInfo> outRequiredProperties)
  {
    var regularProperties   = Enumerable.Empty<PropertyInfo>();
    var nullableProperties  = Enumerable.Empty<PropertyInfo>();
    var requiredProperties  = Enumerable.Empty<PropertyInfo>();

    // get all public properties
    var publicProperties = properties
      .Where(property => property.SetMethod?.IsPublic == true);

    // filter nullable properties
    nullableProperties = publicProperties
      .Where(property => new NullabilityInfoContext().Create(property).WriteState == NullabilityState.Nullable)
      .ToList();

    // filter required properties
    requiredProperties = publicProperties
      .Where(property => property.CustomAttributes
        .Any(attribute => attribute.AttributeType.FullName == "System.Runtime.CompilerServices.RequiredMemberAttribute"))
      .ToList();

    // filter regular properties
    regularProperties = publicProperties
      .Where(property => !nullableProperties.Contains(property))
      .Where(property => !requiredProperties.Contains(property))
      .ToList();

    outRequiredProperties = requiredProperties.ToList();
    outNullableProperties = nullableProperties.ToList();
    outRegularProperties  = regularProperties.ToList();
  }

  private static Dictionary<string, string> GetArgumentDictionary(IEnumerable<System.CommandLine.Parsing.SymbolResult> children)
  {
    Dictionary<string, string> args = [];
    foreach (var arg in children)
    {
      string optionName   = arg.Symbol.Name;
      string optionValue  = arg.Tokens.FirstOrDefault()?.Value; // TODO: allow multiple values in the future

      args.Add(optionName, optionValue);
    }
    return args;
  }

  private Dictionary<string, object> GetConvertedProperties(List<PropertyInfo> regularProperties, List<PropertyInfo> nullableProperties, List<PropertyInfo> requiredProperties, Dictionary<string, string> arguments)
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
      GetConvertedRegularProperties(regularProperties, argumentsForRegularProperties),
      GetConvertedNullableProperties(nullableProperties, argumentsForNullableProperties),
      GetConvertedRequiredProperties(requiredProperties, argumentsForRequiredProperties)
    }
    .SelectMany(dict => dict)
    .ToDictionary(pair => pair.Key, pair => pair.Value);

    return convertedProperties;
  }

  private Dictionary<string, object> GetConvertedRegularProperties(List<PropertyInfo> regularProperties, Dictionary<string, string> propertyValues)
  {
    Dictionary<string, object> convertedProperties = new Dictionary<string, object>();

    regularProperties.ForEach(property => {
      Type underlyngType    = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
      object convertedValue = Convert.ChangeType(propertyValues[property.Name], underlyngType);
      convertedProperties.Add(property.Name, convertedValue);
    });


    return convertedProperties;
  }

  private Dictionary<string, object> GetConvertedNullableProperties(List<PropertyInfo> nullableProperties, Dictionary<string, string> propertyValues)
  {
    Dictionary<string, object> convertedProperties = new Dictionary<string, object>();

    nullableProperties.ForEach(property =>
    {
      Type underlyngType    = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
      object convertedValue = Convert.ChangeType(propertyValues[property.Name], underlyngType);
      convertedProperties.Add(property.Name, convertedValue);
    });

    return convertedProperties;
  }

  private Dictionary<string, object> GetConvertedRequiredProperties(List<PropertyInfo> requiredProperties, Dictionary<string, string> propertyValues)
  {
    Dictionary<string, object> convertedProperties = new Dictionary<string, object>();

    requiredProperties.ForEach(property => {
      object convertedValue = Convert.ChangeType(propertyValues[property.Name], property.PropertyType);
      convertedProperties.Add(property.Name, convertedValue);
    });

    return convertedProperties;
  }

  private void ExecuteCiatCommand(Type commandType, Dictionary<string, object> propertyValues) {
    // create an instance of the command
    var instance = Activator.CreateInstance(commandType) as ICiatCommand;

    if (instance == null) {
      Console.WriteLine($"Error creating instance of {commandType.Name}");
      return;
    }

    // set the properties of the command
    foreach (var property in commandType.GetProperties())
    {
      property.SetValue(instance, propertyValues[property.Name]);
    }

    // execute the command
    instance.Execute();
  }
}
