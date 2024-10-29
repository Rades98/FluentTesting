using Testing.Common.Interfaces;

namespace Testing.Common.Extensions
{
	public static class TestFixtureExtensions
	{
		/// <summary>
		/// Get cancellation token source with timeout in seconds
		/// </summary>
		/// <param name="fixture">fixture object implementing <see cref="ITestFixture"/></param>
		/// <param name="timeoutInSeconds">timeout in seconds - default is 60</param>
		/// <returns></returns>
		public static CancellationTokenSource GetCtsWithTimeoutInSeconds(this ITestFixture fixture, int timeoutInSeconds = 60)
			=> fixture.ApplicationFactory.GetCtsWithTimeoutInSeconds(timeoutInSeconds);

		/// <summary>
		/// Use base implementation of service
		/// This can be used when you need to determine whether handler was called, but still use notmocked functionality
		/// </summary>
		/// <typeparam name="TService">Service to mock</typeparam>
		/// <typeparam name="TReturn">Return type of method</typeparam>
		/// <param name="fixture">fixture object implementing <see cref="ITestFixture"/></param>
		/// <param name="methodToInvoke">Method to invoke</param>
		/// <returns></returns>
		public static Task<TReturn> UseBaseImplementation<TService, TReturn>(this ITestFixture fixture, Func<TService, Task<TReturn>> methodToInvoke)
			=> fixture.ApplicationFactory.UseBaseImplementation(methodToInvoke);

		/// <summary>
		/// Use base implementation and cancel token which is used to wait for consumption
		/// </summary>
		/// <typeparam name="TService">Service to mock</typeparam>
		/// <param name="fixture">fixture object implementing <see cref="ITestFixture"/></param>
		/// <param name="methodToInvoke">Method to invoke</param>
		/// <param name="cts">Cancellation token source</param>
		/// <param name="cancelAfter">Cancel after - default is 0 seconds</param>
		/// <returns></returns>
		public static Task UseBaseImplementationAndCancelToken<TService>(this ITestFixture fixture,
			Func<TService, Task> methodToInvoke, CancellationTokenSource cts, TimeSpan? cancelAfter = null)
			=> fixture.ApplicationFactory.UseBaseImplementationAndCancelToken(methodToInvoke, cts, cancelAfter ?? TimeSpan.FromSeconds(0));
		
		/// <summary>
		/// Use base implementation and cancel token which is used to wait for consumption
		/// </summary>
		/// <typeparam name="TService">Service to mock</typeparam>
		/// <typeparam name="TReturn">Return type of method</typeparam>
		/// <param name="fixture">fixture object implementing <see cref="ITestFixture"/></param>
		/// <param name="methodToInvoke">Method to invoke</param>
		/// <param name="cts">Cancellation token source</param>
		/// <param name="cancelAfter">Cancel after - default is 0 seconds</param>
		/// <returns></returns>
		public static Task<TReturn> UseBaseImplementationAndCancelToken<TService, TReturn>(this ITestFixture fixture,
			Func<TService, Task<TReturn>> methodToInvoke, CancellationTokenSource cts, TimeSpan? cancelAfter = null)
			=> fixture.ApplicationFactory.UseBaseImplementationAndCancelToken(methodToInvoke, cts, cancelAfter ?? TimeSpan.FromSeconds(0));

		/// <summary>
		/// Publish message (Kafka/Rabbit) and wait for cancellation.
		/// </summary>
		/// <param name="fixture">fixture object implementing <see cref="ITestFixture"/></param>
		/// <param name="publish">Task performing publishing message</param>
		/// <param name="cancellationTokenSource">cancellation token source to stop waiting</param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static Task PublishAndWait(this ITestFixture fixture, Task publish, CancellationTokenSource cancellationTokenSource)
			=> fixture.ApplicationFactory.ExecuteAndWait(publish, cancellationTokenSource);
	}
}
