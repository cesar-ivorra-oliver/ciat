using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Ciat.Core;
public class CiatSettings
{
  public Solution Solution { get; set; }

  // Parameterless constructor (required by YamlDotNet)
  public CiatSettings()
  {
    Solution = new Solution();
  }

  public CiatSettings(string yamlFilePath) : this()
  {
    if (!File.Exists(yamlFilePath))
    {
      throw new FileNotFoundException($"The file '{yamlFilePath}' was not found.");
    }

    // deserialize the yaml file
    string yamlContent = File.ReadAllText(yamlFilePath);
    var deserializer = new DeserializerBuilder()
      .WithNamingConvention(CamelCaseNamingConvention.Instance)
      .Build();

    // set the settings
    var ciatSettings = deserializer.Deserialize<CiatSettings>(yamlContent);
    this.Solution = ciatSettings.Solution;
  }
}

public class Solution
{
  public string Name { get; set; }
  public Projects Projects { get; set; }
}

public class Projects
{
  public Project Launcher { get; set; }
  public Project Command { get; set; }
  public List<Project> SubProjects { get; set; }
}

public class Project
{
  public string Name { get; set; }
  public string Framework { get; set; }
  public string Type { get; set; }
  public List<Package> Packages { get; set; }
}

public class Package
{
  public string Name { get; set; }
  public string Version { get; set; }
}
