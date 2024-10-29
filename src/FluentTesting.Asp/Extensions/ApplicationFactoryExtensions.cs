using Testing.Common.Interfaces;

namespace Testing.Asp.Extensions
{
	/// <summary>
	/// Application factory extensions
	/// </summary>
	public static class ApplicationFactoryExtensions
	{
		/// <summary>
		/// retype as ASP factory
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public static AspApplicationFactory AsAspFactory(this IApplicationFactory factory)
			=> (AspApplicationFactory)factory;

		/// <summary>
		/// Get http client from ASP factory
		/// </summary>
		/// <param name="factory">application factory</param>
		/// <returns></returns>
		public static HttpClient GetClient(this IApplicationFactory factory)
			=> factory.AsAspFactory().Client;
	}
}
