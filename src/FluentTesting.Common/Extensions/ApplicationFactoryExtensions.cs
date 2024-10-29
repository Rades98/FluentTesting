using Microsoft.Extensions.DependencyInjection;
using Moq;
using Testing.Common.Abstraction;
using Testing.Common.Interfaces;

namespace Testing.Common.Extensions
{
	/// <summary>
	/// application factory extensions
	/// </summary>
	public static class ApplicationFactoryExtensions
	{
		/// <summary>
		/// Wait for shutdown (start application)
		/// </summary>
		/// <param name="factory">Applciation factory</param>
		/// <param name="cancellationToken">cancellation token</param>
		/// <returns></returns>
		public static Task WaitForShutdownAsync(this IApplicationFactory factory, CancellationToken cancellationToken = default)
			=> ((ApplicationFactory)factory).WaitForShutdownAsync(cancellationToken);

		/// <summary>
		/// Get cancellation token source with timeout in seconds
		/// </summary>
		/// <param name="_">application factory object <see cref="IApplicationFactory"/></param>
		/// <param name="timeoutInSeconds">timeout in seconds - default is 60</param>
		/// <returns></returns>
		public static CancellationTokenSource GetCtsWithTimeoutInSeconds(this IApplicationFactory _, int timeoutInSeconds = 60)
		{
			var cts = new CancellationTokenSource();
			cts.CancelAfter(TimeSpan.FromSeconds(timeoutInSeconds));

			return cts;
		}

		/// <summary>
		/// Use base implementation of service
		/// This can be used when you need to determine whether handler was called, but still use notmocked functionality
		/// </summary>
		/// <typeparam name="TService">Service to use</typeparam>
		/// <param name="factory">application factory object <see cref="IApplicationFactory"/></param>
		/// <param name="methodToInvoke">Method to invoke</param>
		/// <returns></returns>
		public static async Task UseBaseImplementation<TService>(this IApplicationFactory factory, Func<TService, Task> methodToInvoke)
		{
			var src = factory.Services.GetServices<TService>().FirstOrDefault(s => s is not Mock)!;

			await methodToInvoke(src).ConfigureAwait(false);
		}

		/// <summary>
		/// Use base implementation of service
		/// This can be used when you need to determine whether handler was called, but still use notmocked functionality
		/// </summary>
		/// <typeparam name="TService">Service to use</typeparam>
		/// <typeparam name="TReturn">Return type of used method</typeparam>
		/// <param name="factory">application factory object <see cref="IApplicationFactory"/></param>
		/// <param name="methodToInvoke">Method to invoke</param>
		/// <returns></returns>
		public static async Task<TReturn> UseBaseImplementation<TService, TReturn>(this IApplicationFactory factory, Func<TService, Task<TReturn>> methodToInvoke)
		{
			var src = factory.Services.GetServices<TService>().FirstOrDefault(s => s is not Mock)!;

			return await methodToInvoke(src).ConfigureAwait(false);
		}

		/// <summary>
		/// Use base implementation and cancel token which is used to wait for consumption
		/// This can be used when you need to determine whether handler was called, but still use notmocked functionality
		/// </summary>
		/// <typeparam name="TService">Service to use</typeparam>
		/// <param name="factory">application factory object <see cref="IApplicationFactory"/></param>
		/// <param name="methodToInvoke">Method to invoke</param>
		/// <param name="cts">Cancellation token source</param>
		/// <param name="cancelAfter">Cancel after - default is 0 seconds</param>
		/// <returns></returns>
		public static async Task UseBaseImplementationAndCancelToken<TService>(this IApplicationFactory factory, Func<TService, Task> methodToInvoke, CancellationTokenSource cts, TimeSpan cancelAfter)
		{
			await factory.UseBaseImplementation(methodToInvoke).ConfigureAwait(false);

			cts.CancelAfter(cancelAfter);
		}

		/// <summary>
		/// Use base implementation and cancel token which is used to wait for consumption
		/// This can be used when you need to determine whether handler was called, but still use notmocked functionality
		/// </summary>
		/// <typeparam name="TService">Service to use</typeparam>
		/// <typeparam name="TReturn">Return type of used method</typeparam>
		/// <param name="factory">application factory object <see cref="IApplicationFactory"/></param>
		/// <param name="methodToInvoke">Method to invoke</param>
		/// <param name="cts">Cancellation token source</param>
		/// <param name="cancelAfter">Cancel after - default is 0 seconds</param>
		/// <returns></returns>
		public static async Task<TReturn> UseBaseImplementationAndCancelToken<TService, TReturn>(this IApplicationFactory factory, Func<TService, Task<TReturn>> methodToInvoke, CancellationTokenSource cts, TimeSpan cancelAfter)
		{
			var res = await factory.UseBaseImplementation(methodToInvoke).ConfigureAwait(false);

			cts.CancelAfter(cancelAfter);

			return res;
		}

		/// <summary>
		/// Executes Task and wait for cancellation.
		/// </summary>
		/// <param name="_"></param>
		/// <param name="action">Task execution</param>
		/// <param name="cancellationTokenSource">cancellation token source to stop waiting</param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static async Task ExecuteAndWait(this IApplicationFactory _, Task action, CancellationTokenSource cancellationTokenSource)
		{
			await action.ContinueWith(task =>
			{
				if (task.IsFaulted)
				{
					throw new Exception(task.Exception?.Message ?? string.Empty);
				}
			}, cancellationTokenSource.Token);

			while (!cancellationTokenSource.IsCancellationRequested)
			{
				await Task.Delay(100).ConfigureAwait(false);
			}
		}
	}
}
