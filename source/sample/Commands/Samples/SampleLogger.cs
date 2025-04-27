using Ciat.CiatCommand;
using Microsoft.Extensions.Logging;

namespace sample.Commands.Samples
{
  public class SampleLogger : ICiatCommand
  {
    public void Execute(ILogger logger)
    {
      logger.LogInformation("Executed class name: '{ClassName}'", nameof(SampleLogger));
      logger.LogTrace("This is a trace message");
      logger.LogDebug("This is a debug message");
      logger.LogInformation("This is an information message");
      logger.LogWarning("This is a warning message");
      logger.LogError("This is an error message");
      logger.LogCritical("This is a critical message");

    }
  }
}
