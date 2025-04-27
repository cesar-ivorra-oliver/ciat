using Microsoft.Extensions.Logging;

namespace ciatLauncher.Core
{
  public class ICiatLogger : ILogger
  {
    private readonly string _categoryName;

    public ICiatLogger(string categoryName) => _categoryName = categoryName;
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => throw new NotImplementedException();
    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
      if (!IsEnabled(logLevel))
      {
        return;
      }

      // define the chunks of the log message
      var time      = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      var prefix    = getPrefix(logLevel);
      var category  = $"[{_categoryName}]";
      var message   = formatter(state, exception);

      var logMessages = new List<string> {
        time,
        prefix,
        category,
        message
      }
      .Where(item => !string.IsNullOrEmpty(item))
      .ToList();

      // set console colors
      ConsoleColor currentColor = Console.ForegroundColor;
      Console.ForegroundColor = getLogLevelColor(logLevel);

      // print the log message
      Console.WriteLine(string.Join(" ", logMessages));
      Console.ForegroundColor = currentColor;
    }

    private string getPrefix(LogLevel logLevel) {
      return logLevel switch
      {
        LogLevel.Trace        => "[TRACE]",
        LogLevel.Debug        => "[DEBUG]",
        LogLevel.Information  => "[INFO]",
        LogLevel.Warning      => "[WARN]",
        LogLevel.Error        => "[ERROR]",
        LogLevel.Critical     => "[CRIT]",
        LogLevel.None         => "[NONE]",
        _                     => "[UNKN]"
      };
    }

    private ConsoleColor getLogLevelColor(LogLevel logLevel)
    {
      return logLevel switch
      {
        LogLevel.Trace        => ConsoleColor.Gray,
        LogLevel.Debug        => ConsoleColor.Cyan,
        LogLevel.Information  => ConsoleColor.White,
        LogLevel.Warning      => ConsoleColor.Yellow,
        LogLevel.Error        => ConsoleColor.Red,
        LogLevel.Critical     => ConsoleColor.Magenta,
        LogLevel.None         => ConsoleColor.White,
        _                     => ConsoleColor.White
      };
    }
  }
}
