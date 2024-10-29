using Microsoft.Extensions.Logging;

namespace Testing.Common.Logger
{
	/// <summary>
	/// In memory logger provider
	/// 
	/// 
	/// USAGE:
	/// 
	/// services.AddLogging(builder =>
	///		{
	///			builder.ClearProviders();
	///			builder.AddProvider(InMemoryLoggerProvider);
	///		});
	/// 
	/// </summary>
	public class InMemoryLoggerProvider : ILoggerProvider
	{
		private readonly InMemoryLogger _logger = new();

		public ILogger CreateLogger(string categoryName) => _logger;

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		public InMemoryLogger GetLogger() => _logger;
	}
}
