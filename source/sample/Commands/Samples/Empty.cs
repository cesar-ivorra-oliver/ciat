using Ciat.CiatCommand;
using Microsoft.Extensions.Logging;


namespace sample.Commands.Samples
{
  public class Empty : ICiatCommand
  {
    public void Execute(ILogger logger)
    {
      new List<string>
      {
        $"Executed class name: '{nameof(Empty)}'",
        "class without properties"
      }.ForEach(line => logger.LogInformation(line));
    }
  }
}
