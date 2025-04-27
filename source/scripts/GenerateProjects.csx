#load "CommandLine.csx"
#load "Settings.csx"

using System.Xml.Linq;


Console.WriteLine("Generating the projects ...");

#region setup
// create the temp directory
Console.WriteLine($"Using the temporary directory '{dirSettings.TempDirectory}'.");
Directory.CreateDirectory(dirSettings.TempDirectory);
Directory.SetCurrentDirectory(dirSettings.TempDirectory);
#endregion

#region launcher project
Project launcher = ciatSettings.Solution.Projects.Launcher;

Generate.Project(launcher.Name, launcher.Type, launcher.Framework, launcher.Packages, out string launcherCsprojFile);
Generate.AddExternalReference(launcherCsprojFile, Path.Combine("..", ciatSettings.FileName));
#endregion

#region command project
Project command = ciatSettings.Solution.Projects.Command;

Generate.Project(command.Name, command.Type, command.Framework, command.Packages, out string commandCsprojFile);
#endregion

#region sub-projects
List<string> subProjectFiles = new();
foreach (Project subProject in ciatSettings.Solution.Projects.SubProjects) {
  Generate.Project(subProject.Name, subProject.Type, subProject.Framework, subProject.Packages, out string subProjectFile);
  Generate.AddReference(subProjectFile, commandCsprojFile);
  Generate.AddReference(launcherCsprojFile, subProjectFile);

  subProjectFiles.Add(subProjectFile);
}
#endregion

#region solution file
Generate.Solution(ciatSettings.Solution.Name, out string slnFile);
Generate.AddProject(slnFile, launcherCsprojFile);
Generate.AddProject(slnFile, commandCsprojFile);
foreach (string subProjectFile in subProjectFiles) {
  Generate.AddProject(slnFile, subProjectFile);
}
#endregion

#region copy projects
File.Copy(slnFile,            Path.Combine(dirSettings.SourceDirectory, slnFile));
File.Copy(launcherCsprojFile, Path.Combine(dirSettings.SourceDirectory, launcherCsprojFile));
File.Copy(commandCsprojFile,  Path.Combine(dirSettings.SourceDirectory, commandCsprojFile));
foreach (string subProjectFile in subProjectFiles) {
  File.Copy(subProjectFile,   Path.Combine(dirSettings.SourceDirectory, subProjectFile));
}

Console.WriteLine("Projects copied to the source directory without affecting the existing code files.");

// clean up the temp directory
Console.WriteLine("Cleaning up the temporary directory ...");
Directory.SetCurrentDirectory(dirSettings.SourceDirectory);
Directory.Delete(dirSettings.TempDirectory, true);
#endregion

Console.WriteLine("Projects generated successfully.");

#region helper classes
public static class Generate {
  public static void Solution(string name, out string slnFilePath) {
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"Generating {name} solution ...");
    Console.ResetColor();

    // solution file
    slnFilePath = $"{name}.sln";

    // generate the solution
    CommandLine.Execute("dotnet", $"new sln -n {name}");
  }
  public static void Project(string name, string type, string framework, IEnumerable<Package> packages, out string csProjFilePath) {
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"Generating {name} project ...");
    Console.ResetColor();

    // project file
    csProjFilePath = Path.Combine(name, $"{name}.csproj");

    // generate the project
    CommandLine.Execute("dotnet", $"new {type} -o {name} --framework {framework}");

    // add the packages
    foreach (Package package in packages) {
      CommandLine.Execute("dotnet", $"add {csProjFilePath} package {package.Name} --version {package.Version}");
    }
  }
  public static void AddProject(string slnFile, string csprojFile) {
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"Adding csproj {csprojFile} to the solution {slnFile} ...");
    Console.ResetColor();

    CommandLine.Execute("dotnet", $"sln {slnFile} add {csprojFile}");
  }
  public static void AddReference(string csprojFile, string referenceFile) {
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"Adding reference {referenceFile} to the project {csprojFile} ...");
    Console.ResetColor();

    CommandLine.Execute("dotnet", $"add {csprojFile} reference {referenceFile}");
  }
  public static void AddExternalReference(string csprojFile, string referenceFile) {
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"Adding external reference {referenceFile} to the project {csprojFile} ...");
    Console.ResetColor();

    // load the project file as XML
    XDocument csproj = XDocument.Load(csprojFile);
    csproj.Root.Add(
      new XElement("ItemGroup",
        new XElement("Content",
          new XAttribute("Include", referenceFile),
          new XElement("CopyToOutputDirectory", "PreserveNewest")
        )
      )
    );

    csproj.Save(csprojFile);
  }
}
#endregion