using Microsoft.Extensions.Logging;

namespace Ciat.CiatCommand
{
  public interface ICiatCommand
  {
    void Execute(ILogger logger);
  }
}
