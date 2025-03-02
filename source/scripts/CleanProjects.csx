#load "Settings.csx"

using YamlDotNet.Serialization;


Console.WriteLine("Cleaning the projects ...");

#region setup
DirectorySettings dirSettings = new ();
CiatSettings settings         = new (Path.Combine(dirSettings.SourceDirectory, "ciatSettings.yaml"));
string[] projectFolders = [
  "bin",
  "obj",
  "Properties",
];
#endregion


#region clean
Directory.SetCurrentDirectory(dirSettings.SourceDirectory);
Clean.Preview = Environment.GetCommandLineArgs().Contains("--preview");

// clean solution
Clean.Project([$"{settings.Solution.Name}.sln"], [".vs"]);

// clean projects
Project launcher  = settings.Solution.Projects.Launcher;
Project command   = settings.Solution.Projects.Command;

Clean.Project(
  new [] { Path.Combine(launcher.Name, $"{launcher.Name}.csproj") },
  projectFolders.Select(folder => Path.Combine(launcher.Name, folder))
);
Clean.Project(
  new [] { Path.Combine(command.Name, $"{command.Name}.csproj") },
  projectFolders.Select(folder => Path.Combine(command.Name, folder))
);

// clean sub-projects
settings.Solution.Projects.SubProjects.ForEach(subProject => {
  Clean.Project(
    new [] { Path.Combine(subProject.Name, $"{subProject.Name}.csproj") },
    projectFolders.Select(folder => Path.Combine(subProject.Name, folder))
  );
});
#endregion

Console.WriteLine("Projects cleaned.");

#region helper classes
public static class Clean {
  public static bool Preview { get; set; }

  public static void Project(IEnumerable<string> files, IEnumerable<string> folders) {
    foreach (string file in files) {
      if (File.Exists(file)) {
        if (Preview) {
          Console.ForegroundColor = ConsoleColor.Yellow;
          Console.WriteLine($"[Preview] Would delete file: {file}");
        } else {
          Console.ForegroundColor = ConsoleColor.Cyan;
          Console.WriteLine($"Deleting file: {file}");
          File.Delete(file);
        }
      }
    }
    foreach (string folder in folders) {
      if (Directory.Exists(folder)) {
        if (Preview) {
          Console.ForegroundColor = ConsoleColor.Yellow;
          Console.WriteLine($"[Preview] Would delete directory: {folder}");
        } else {
          Console.ForegroundColor = ConsoleColor.Cyan;
          Console.WriteLine($"Deleting directory: {folder}");
          Directory.Delete(folder, true);
        }
      }
    }
    Console.ResetColor();
  }
}
#endregion