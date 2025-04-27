#r "nuget: YamlDotNet, 16.3.0"
#load "../ciatLauncher/Core/CiatSettings.cs"

var dirSettings   = new DirectorySettings();
var ciatSettings  = new CiatSettings(Path.Combine(dirSettings.SourceDirectory, "ciatSettings.yaml"));

# region directory settings
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
#endregion
