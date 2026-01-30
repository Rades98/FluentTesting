using FluentTesting.Common.Interfaces;

namespace FluentTesting.Asp.Extensions
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

        /// <summary>
        /// Get http client from ASP factory
        /// </summary>
        /// <param name="factory">application factory</param>
        /// <returns></returns>
        public static HttpClient GetNewClient(this IApplicationFactory factory)
            => factory.AsAspFactory().GetNewClient();
    }
}
