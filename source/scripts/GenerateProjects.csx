#load "CommandLine.csx"
#load "Settings.csx"


using System.Xml.Linq;

Console.WriteLine("Generating the projects ...");

#region setup
// load the settings
string ciatSettingsFileName = "ciatSettings.yaml";
DirectorySettings dirSettings = new DirectorySettings();
CiatSettings settings = new CiatSettings(Path.Combine(dirSettings.SourceDirectory, ciatSettingsFileName));

// create the temp directory
Directory.CreateDirectory(dirSettings.TempDirectory);
Directory.SetCurrentDirectory(dirSettings.TempDirectory);
Console.WriteLine($"Temporary directory created at '{dirSettings.TempDirectory}'.");
#endregion

#region launcher project
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Generating launcher project ...");
Console.ResetColor();
Project launcher = settings.Solution.Projects.Launcher;
string launcherProjectFile = Path.Combine(launcher.Name, $"{launcher.Name}.csproj");

CommandLine.Execute("dotnet", $"new {launcher.Type} -o {launcher.Name} --framework {launcher.Framework}");
launcher.Packages?.ForEach(package => CommandLine.Execute("dotnet", $"add {launcherProjectFile} package {package.Name} --version {package.Version}"));

// add reference to ciatSettings.yaml
XDocument launchercsproj = XDocument.Load(launcherProjectFile);
launchercsproj.Root.Add(
  new XElement("ItemGroup",
    new XElement("Content",
      new XAttribute("Include", Path.Combine("..", ciatSettingsFileName)),
      new XElement("CopyToOutputDirectory", "PreserveNewest")
    )
  )
);
launchercsproj.Save(launcherProjectFile);

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Launcher project generated.");
Console.ResetColor();
#endregion

#region command project
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Generating command project ...");
Console.ResetColor();
Project command = settings.Solution.Projects.Command;
string commandProjectFile = Path.Combine(command.Name, $"{command.Name}.csproj");

CommandLine.Execute("dotnet", $"new {command.Type} -o {command.Name} --framework {command.Framework}");
command.Packages?.ForEach(package => CommandLine.Execute("dotnet", $"add {commandProjectFile} package {package.Name} --version {package.Version}"));
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Command project generated.");
Console.ResetColor();
#endregion

#region sub-projects
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Generating sub-projects ...");
Console.ResetColor();
List<string> subProjectFiles = [];
settings.Solution.Projects.SubProjects?.ForEach(subProject => {
  string subProjectProjectFile = Path.Combine(subProject.Name, $"{subProject.Name}.csproj");
  subProjectFiles.Add(subProjectProjectFile);

  CommandLine.Execute("dotnet", $"new {subProject.Type} -o {subProject.Name} --framework {subProject.Framework}");
  subProject.Packages?.ForEach(package => CommandLine.Execute("dotnet", $"add {subProjectProjectFile} package {package.Name} --version {package.Version}"));

  // each sub-project must get reference of the command project
  CommandLine.Execute("dotnet", $"add {subProjectProjectFile} reference {commandProjectFile}");

  // each sub-project must be referenced by the launcher project
  CommandLine.Execute("dotnet", $"add {launcherProjectFile} reference {subProjectProjectFile}");
});
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Sub-projects generated.");
Console.ResetColor();
#endregion

#region solution file
string vsSolutionFile = $"{settings.Solution.Name}.sln";
CommandLine.Execute("dotnet", $"new sln -n {settings.Solution.Name}");
CommandLine.Execute("dotnet", $"sln {vsSolutionFile} add {launcherProjectFile}");
CommandLine.Execute("dotnet", $"sln {vsSolutionFile} add {commandProjectFile}");
subProjectFiles.ForEach(subProjectFile => CommandLine.Execute("dotnet", $"sln {vsSolutionFile} add {subProjectFile}"));

Console.WriteLine("Projects generated in the temporary directory.");
#endregion

#region copy projects
Directory.SetCurrentDirectory(dirSettings.SourceDirectory);
File.Copy(Path.Combine(dirSettings.TempDirectory, vsSolutionFile),      Path.Combine(dirSettings.SourceDirectory, vsSolutionFile));
File.Copy(Path.Combine(dirSettings.TempDirectory, launcherProjectFile), Path.Combine(dirSettings.SourceDirectory, launcherProjectFile));
File.Copy(Path.Combine(dirSettings.TempDirectory, commandProjectFile),  Path.Combine(dirSettings.SourceDirectory, commandProjectFile));
subProjectFiles.ForEach(subProjectFile => File.Copy(Path.Combine(dirSettings.TempDirectory, subProjectFile), Path.Combine(dirSettings.SourceDirectory, subProjectFile)));
Console.WriteLine("Projects copied to the source directory without affecting the existing code files.");

// clean up the temp directory
Console.WriteLine("Cleaning up the temporary directory ...");
Directory.Delete(dirSettings.TempDirectory, true);
Console.WriteLine("Temporary directory deleted.");
#endregion

Console.WriteLine("Projects generated successfully.");