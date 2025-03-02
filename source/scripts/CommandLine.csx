#r "System.Diagnostics.Process"

using System.Diagnostics;

public class CommandLine {
  public static void Execute(string command, string args = "") {
    try {
      string commandResul = ExecuteAndGetOutput(command, args);
      Console.WriteLine(commandResul);
    }
    catch (Exception ex) {
      Console.WriteLine($"Error: {ex.Message}");
      throw;
    }
  }

  public static string ExecuteAndGetOutput(string command, string args = "") {
    var processInfo = GetProcessStartInfo(command, args);

    using (var process = new Process { StartInfo = processInfo }) {
      process.Start();
      string output = process.StandardOutput.ReadToEnd();
      string error  = process.StandardError.ReadToEnd();
      process.WaitForExit();

      if (process.ExitCode != 0) {
        throw new Exception($"The command '${command} failed, error: {error}");
      }

      return output;
    }
  }

  private static ProcessStartInfo GetProcessStartInfo(string command, string args) {
    return new ProcessStartInfo {
      FileName  = command,
      Arguments = args,
      RedirectStandardOutput  = true,
      RedirectStandardError   = true,
      UseShellExecute = false,
      CreateNoWindow  = true
    };
  }
}