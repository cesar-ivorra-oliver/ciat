using Ciat.CiatCommand;


namespace sample.Commands.Samples
{
  public class Empty : IciatCommand
  {
    public void Execute()
    {
      Console.WriteLine(string.Join("\n", new[]
      {
        $"Executed class name: '{nameof(Empty)}'",
        "class without properties"
      }));
    }
  }
}
