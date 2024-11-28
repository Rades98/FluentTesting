using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FluentTesting.Common.Logger
{
    /// <summary>
    /// In memory logger
    /// </summary>
    public class InMemoryLogger : ILogger
    {
        private readonly List<LogEntry> _logEntries = [];

        public IReadOnlyList<LogEntry> LogEntries => _logEntries;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var entry = new LogEntry
            {
                LogLevel = logLevel,
                EventId = eventId,
                Message = formatter(state, exception),
                Exception = exception
            };

            if (Debugger.IsAttached)
            {
                Debug.WriteLine(entry);
            }

            _logEntries.Add(entry);
        }
    }
}
