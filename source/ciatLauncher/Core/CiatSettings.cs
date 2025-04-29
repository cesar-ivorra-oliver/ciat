using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

// this class is not declared in a namespace as it is shared between the C# scripts and the C# code
public class CiatSettings
{
  public Solution Solution { get; set; }
  public string FileName { get; set; }

  // Parameterless constructor (required by YamlDotNet)
  public CiatSettings()
  {
    Solution = default!;
    FileName = default!;
  }

  public CiatSettings(string yamlFilePath) : this()
  {
    if (!File.Exists(yamlFilePath))
    {
      throw new FileNotFoundException($"The file '{yamlFilePath}' was not found.");
    }

    // deserialize the yaml file
    string yamlContent  = File.ReadAllText(yamlFilePath);
    var deserializer    = new DeserializerBuilder()
      .WithNamingConvention(CamelCaseNamingConvention.Instance)
      .Build();

    // set the settings
    var ciatSettings  = deserializer.Deserialize<CiatSettings>(yamlContent);
    this.Solution     = ciatSettings.Solution;
    this.FileName     = Path.GetFileName(yamlFilePath);
  }
}

public class Solution
{
  public required string Name { get; set; }
  public required string Description { get; set; }
  public required Projects Projects { get; set; }
}

public class Projects
{
  public required Project Launcher { get; set; }
  public required Project Command { get; set; }

  [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
  public List<Project> SubProjects { get; set; } = [];
}

public class Project
{
  public required string Name { get; set; }
  public required string Framework { get; set; }
  public required string Type { get; set; }

  [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
  public List<Package> Packages { get; set; } = [];
}

public class Package
{
  public required string Name { get; set; }
  public required string Version { get; set; }
}
