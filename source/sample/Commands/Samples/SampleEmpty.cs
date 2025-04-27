using Ciat.CiatCommand;
using Microsoft.Extensions.Logging;


namespace sample.Commands.Samples
{
  public class SampleEmpty : ICiatCommand
  {
    public void Execute(ILogger logger)
    {
      new List<string>
      {
        $"Executed class name: '{nameof(SampleEmpty)}'",
        "class without properties"
      }.ForEach(line => logger.LogInformation(line));
    }
  }
}
