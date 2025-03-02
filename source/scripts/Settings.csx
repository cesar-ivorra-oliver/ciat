#r "nuget: YamlDotNet, 16.3.0"

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class DirectorySettings {
  public string ScriptsDirectory { get; private set; }
  public string SourceDirectory { get; private set; }
  public string TempDirectory { get; private set; }

  public DirectorySettings() {
    ScriptsDirectory  = GetScriptsDirectory();
    SourceDirectory   = GetSourceDirectory();
    TempDirectory     = GetTempDirectory();
  }

  private string GetSourceDirectory() {
    return Path.GetFullPath(Path.Combine(ScriptsDirectory, "..")); // <repo>/source
  }

  private string GetScriptsDirectory() {
    string scriptsDirectory = Path.GetDirectoryName(
      Path.GetFullPath(
        Environment.GetCommandLineArgs()
          .FirstOrDefault(arg => arg.EndsWith(".csx", StringComparison.OrdinalIgnoreCase))
        ) // <repo>/source/scripts/generateProjects.csx
    ); // <repo>/source/scripts

    if (!scriptsDirectory.EndsWith(Path.Combine("source", "scripts"))) {
      Console.WriteLine("Error: This script must be executed from the 'source/scripts' directory.");
      Environment.Exit(1);
    }

    return scriptsDirectory;
  }

  private string GetTempDirectory() {
    string tempDirectoryName = $".ciat_temp_{Guid.NewGuid()}";
    return Path.Combine(SourceDirectory, tempDirectoryName); // <repo>/source/ciat_temp_<guid>
  }
}

# region CiatSettings
public class CiatSettings {
  public Solution Solution { get; set; }

  // Parameterless constructor (required by YamlDotNet)
  public CiatSettings() {
    Solution = new Solution();
  }

  public CiatSettings(string yamlFilePath) : this() {
    if (!File.Exists(yamlFilePath)) {
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
  }
}

public class Solution {
  public string Name { get; set; }
  public Projects Projects { get; set; }
}

public class Projects {
  public Project Launcher { get; set; }
  public Project Command { get; set; }
  public List<Project> SubProjects { get; set; }
}

public class Project {
  public string Name { get; set; }
  public string Framework { get; set; }
  public string Type { get; set; }
  public List<Package> Packages { get; set; }
}

public class Package {
  public string Name { get; set; }
  public string Version { get; set; }
}
#endregion