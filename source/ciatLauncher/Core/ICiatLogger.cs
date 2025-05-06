using Microsoft.Extensions.Logging;

namespace Ciat.Core;
public class ICiatLogger : ILogger
{
  private readonly string _categoryName;

  public ICiatLogger(string categoryName)
  {
    _categoryName = categoryName;
  }

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
    var prefix    = GetPrefix(logLevel);
    var category  = $"[{_categoryName}]";
    var message   = formatter(state, exception);

    // create a list of message chunks
    var chunks = new List<string> {
      time,
      prefix,
      category,
      message
    };

    // set console colors
    ConsoleColor originalColor  = Console.ForegroundColor; // save original color
    Console.ForegroundColor     = GetLogLevelColor(logLevel); // set color based on log level

    // print the log message
    Console.WriteLine(string.Join(" ", chunks));
    Console.ForegroundColor = originalColor; // restore original color
  }

  private static string GetPrefix(LogLevel logLevel) {
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

  private static ConsoleColor GetLogLevelColor(LogLevel logLevel)
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
