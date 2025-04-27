#load "Settings.csx"


#region setup
string[] projectFolders = [
  "bin",
  "obj",
  "Properties",
];
#endregion

#region clean
Console.WriteLine("Cleaning the projects ...");
Directory.SetCurrentDirectory(dirSettings.SourceDirectory);
Clean.Preview = Environment.GetCommandLineArgs().Contains("--preview");

// clean solution
Clean.Project([$"{ciatSettings.Solution.Name}.sln"], [".vs"]);

// clean projects
var launcher  = ciatSettings.Solution.Projects.Launcher;
var command   = ciatSettings.Solution.Projects.Command;

Clean.Project(
  new [] { Path.Combine(launcher.Name, $"{launcher.Name}.csproj") },
  projectFolders.Select(folder => Path.Combine(launcher.Name, folder))
);
Clean.Project(
  new [] { Path.Combine(command.Name, $"{command.Name}.csproj") },
  projectFolders.Select(folder => Path.Combine(command.Name, folder))
);

// clean sub-projects
ciatSettings.Solution.Projects.SubProjects.ForEach(subProject => {
  Clean.Project(
    new [] { Path.Combine(subProject.Name, $"{subProject.Name}.csproj") },
    projectFolders.Select(folder => Path.Combine(subProject.Name, folder))
  );
});

Console.WriteLine("Projects cleaned.");
#endregion


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