using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class CiatSettings
{
  public Solution Solution { get; set; }
  public string FileName { get; set; }

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
  public string Name { get; set; }
  public Projects Projects { get; set; }
}

public class Projects
{
  public Project Launcher { get; set; }
  public Project Command { get; set; }

  [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
  public List<Project> SubProjects { get; set; } = new List<Project>();
}

public class Project
{
  public string Name { get; set; }
  public string Framework { get; set; }
  public string Type { get; set; }

  [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
  public List<Package> Packages { get; set; } = new List<Package>();
}

public class Package
{
  public string Name { get; set; }
  public string Version { get; set; }
}
