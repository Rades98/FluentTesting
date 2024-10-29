using Microsoft.Extensions.Logging;

namespace Testing.Common.Logger
{
	/// <summary>
	/// Log entry
	/// </summary>
	public class LogEntry
	{
		/// <summary>
		/// Log level
		/// </summary>
		public LogLevel LogLevel { get; set; }

		/// <summary>
		/// Event id
		/// </summary>
		public EventId EventId { get; set; }

		/// <summary>
		/// Message
		/// </summary>
		public string? Message { get; set; }

		/// <summary>
		/// Exception
		/// </summary>
		public Exception? Exception { get; set; }
	}
}
