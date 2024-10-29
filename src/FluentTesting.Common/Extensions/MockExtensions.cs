using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Linq.Expressions;
using Testing.Common.Interfaces;

namespace Testing.Common.Extensions
{
	/// <summary>
	/// Mock extensions
	/// </summary>
	public static class MockExtensions
	{
		/// <summary>
		/// Unmock and wait - define mock delegate for Moq, the rest is done by this method
		/// </summary>
		/// <typeparam name="TService">Service/Repo type</typeparam>
		/// <typeparam name="TReturn">Return type</typeparam>
		/// <typeparam name="T1">first argument type</typeparam>
		/// <param name="fixture">fixture</param>
		/// <param name="mock">mock of type</param>
		/// <param name="methodToInvoke">method to invoke</param>
		/// <param name="cts">Cancelation token source to stop when the job is done</param>
		/// <param name="additionalDelay">optional additional delay</param>
		public static void UnmockAndWait<TService, TReturn, T1>(
			this ITestFixture fixture,
			Mock<TService> mock,
			Expression<Func<TService, Task<TReturn>>> methodToInvoke,
			CancellationTokenSource cts, TimeSpan? additionalDelay = null)
			where TService : class
		{
			var methodCall = GetMethodCalExpressionOrThrowError(methodToInvoke);

			mock.Setup(methodToInvoke).Returns(async (T1 t1) =>
				{
					var result = await ((Task<TReturn>)methodCall.Method.Invoke(fixture.GetRealService<TService>(), [t1])!).ConfigureAwait(false);

					cts.CancelAfter(additionalDelay ?? TimeSpan.Zero);

					return result;
				});
		}

		/// <summary>
		/// Unmock and wait - define mock delegate for Moq, the rest is done by this method
		/// </summary>
		/// <typeparam name="TService">Service/Repo type</typeparam>
		/// <typeparam name="TReturn">Return type</typeparam>
		/// <typeparam name="T1">first argument type</typeparam>
		/// <typeparam name="T2">second argument type</typeparam>
		/// <param name="fixture">fixture</param>
		/// <param name="mock">mock of type</param>
		/// <param name="methodToInvoke">method to invoke</param>
		/// <param name="cts">Cancelation token source to stop when the job is done</param>
		/// <param name="additionalDelay">optional additional delay</param>
		public static void UnmockAndWait<TService, TReturn, T1, T2>(
			this ITestFixture fixture,
			Mock<TService> mock,
			Expression<Func<TService, Task<TReturn>>> methodToInvoke,
			CancellationTokenSource cts, TimeSpan? additionalDelay = null)
			where TService : class
		{
			var methodCall = GetMethodCalExpressionOrThrowError(methodToInvoke);

			mock.Setup(methodToInvoke).Returns(async (T1 t1, T2 t2) =>
			{
				var result = await ((Task<TReturn>)methodCall.Method.Invoke(fixture.GetRealService<TService>(), [t1, t2])!).ConfigureAwait(false);

				cts.CancelAfter(additionalDelay ?? TimeSpan.Zero);

				return result;
			});
		}

		/// <summary>
		/// Unmock and wait - define mock delegate for Moq, the rest is done by this method
		/// </summary>
		/// <typeparam name="TService">Service/Repo type</typeparam>
		/// <typeparam name="TReturn">Return type</typeparam>
		/// <typeparam name="T1">first argument type</typeparam>
		/// <typeparam name="T2">second argument type</typeparam>
		/// <typeparam name="T3">third argument type</typeparam>
		/// <param name="fixture">fixture</param>
		/// <param name="mock">mock of type</param>
		/// <param name="methodToInvoke">method to invoke</param>
		/// <param name="cts">Cancelation token source to stop when the job is done</param>
		/// <param name="additionalDelay">optional additional delay</param>
		public static void UnmockAndWait<TService, TReturn, T1, T2, T3>(
			this ITestFixture fixture,
			Mock<TService> mock,
			Expression<Func<TService, Task<TReturn>>> methodToInvoke,
			CancellationTokenSource cts, TimeSpan? additionalDelay = null)
			where TService : class
		{
			var methodCall = GetMethodCalExpressionOrThrowError(methodToInvoke);

			mock.Setup(methodToInvoke).Returns(async (T1 t1, T2 t2, T3 t3) =>
			{
				var result = await ((Task<TReturn>)methodCall.Method.Invoke(fixture.GetRealService<TService>(), [t1, t2, t3])!).ConfigureAwait(false);

				cts.CancelAfter(additionalDelay ?? TimeSpan.Zero);

				return result;
			});
		}

		/// <summary>
		/// Unmock and wait - define mock delegate for Moq, the rest is done by this method
		/// </summary>
		/// <typeparam name="TService">Service/Repo type</typeparam>
		/// <typeparam name="TReturn">Return type</typeparam>
		/// <typeparam name="T1">first argument type</typeparam>
		/// <typeparam name="T2">second argument type</typeparam>
		/// <typeparam name="T3">third argument type</typeparam>
		/// <typeparam name="T4">fourth argument type</typeparam>
		/// <param name="fixture">fixture</param>
		/// <param name="mock">mock of type</param>
		/// <param name="methodToInvoke">method to invoke</param>
		/// <param name="cts">Cancelation token source to stop when the job is done</param>
		/// <param name="additionalDelay">optional additional delay</param>
		public static void UnmockAndWait<TService, TReturn, T1, T2, T3, T4>(
			this ITestFixture fixture,
			Mock<TService> mock,
			Expression<Func<TService, Task<TReturn>>> methodToInvoke,
			CancellationTokenSource cts, TimeSpan? additionalDelay = null)
			where TService : class
		{
			var methodCall = GetMethodCalExpressionOrThrowError(methodToInvoke);

			mock.Setup(methodToInvoke).Returns(async (T1 t1, T2 t2, T3 t3, T4 t4) =>
			{
				var result = await ((Task<TReturn>)methodCall.Method.Invoke(fixture.GetRealService<TService>(), [t1, t2, t3, t4])!).ConfigureAwait(false);

				cts.CancelAfter(additionalDelay ?? TimeSpan.Zero);

				return result;
			});
		}

		/// <summary>
		/// Unmock and wait - define mock delegate for Moq, the rest is done by this method
		/// </summary>
		/// <typeparam name="TService">Service/Repo type</typeparam>
		/// <typeparam name="TReturn">Return type</typeparam>
		/// <typeparam name="T1">first argument type</typeparam>
		/// <typeparam name="T2">second argument type</typeparam>
		/// <typeparam name="T3">third argument type</typeparam>
		/// <typeparam name="T4">fourth argument type</typeparam>
		/// <typeparam name="T5">fifth argument type</typeparam>
		/// <param name="fixture">fixture</param>
		/// <param name="mock">mock of type</param>
		/// <param name="methodToInvoke">method to invoke</param>
		/// <param name="cts">Cancelation token source to stop when the job is done</param>
		/// <param name="additionalDelay">optional additional delay</param>
		public static void UnmockAndWait<TService, TReturn, T1, T2, T3, T4, T5>(
			this ITestFixture fixture,
			Mock<TService> mock,
			Expression<Func<TService, Task<TReturn>>> methodToInvoke,
			CancellationTokenSource cts, TimeSpan? additionalDelay = null)
			where TService : class
		{
			var methodCall = GetMethodCalExpressionOrThrowError(methodToInvoke);

			mock.Setup(methodToInvoke).Returns(async (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) =>
			{
				var result = await ((Task<TReturn>)methodCall.Method.Invoke(fixture.GetRealService<TService>(), [t1, t2, t3, t4, t5])!).ConfigureAwait(false);

				cts.CancelAfter(additionalDelay ?? TimeSpan.Zero);

				return result;
			});
		}

		/// <summary>
		/// Unmock and wait - define mock delegate for Moq, the rest is done by this method
		/// </summary>
		/// <typeparam name="TService">Service/Repo type</typeparam>
		/// <typeparam name="TReturn">Return type</typeparam>
		/// <typeparam name="T1">first argument type</typeparam>
		/// <typeparam name="T2">second argument type</typeparam>
		/// <typeparam name="T3">third argument type</typeparam>
		/// <typeparam name="T4">fourth argument type</typeparam>
		/// <typeparam name="T5">fifth argument type</typeparam>
		/// <typeparam name="T6">sixth argument type</typeparam>
		/// <param name="fixture">fixture</param>
		/// <param name="mock">mock of type</param>
		/// <param name="methodToInvoke">method to invoke</param>
		/// <param name="cts">Cancelation token source to stop when the job is done</param>
		/// <param name="additionalDelay">optional additional delay</param>
		public static void UnmockAndWait<TService, TReturn, T1, T2, T3, T4, T5, T6>(
			this ITestFixture fixture,
			Mock<TService> mock,
			Expression<Func<TService, Task<TReturn>>> methodToInvoke,
			CancellationTokenSource cts, TimeSpan? additionalDelay = null)
			where TService : class
		{
			var methodCall = GetMethodCalExpressionOrThrowError(methodToInvoke);

			mock.Setup(methodToInvoke).Returns(async (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) =>
			{
				var result = await ((Task<TReturn>)methodCall.Method.Invoke(fixture.GetRealService<TService>(), [t1, t2, t3, t4, t5, t6])!).ConfigureAwait(false);

				cts.CancelAfter(additionalDelay ?? TimeSpan.Zero);

				return result;
			});
		}

		/// <summary>
		/// Unmock and wait - define mock delegate for Moq, the rest is done by this method
		/// </summary>
		/// <typeparam name="TService">Service/Repo type</typeparam>
		/// <typeparam name="T1">first argument type</typeparam>
		/// <param name="fixture">fixture</param>
		/// <param name="mock">mock of type</param>
		/// <param name="methodToInvoke">method to invoke</param>
		/// <param name="cts">Cancelation token source to stop when the job is done</param>
		/// <param name="additionalDelay">optional additional delay</param>
		public static void UnmockAndWait<TService, T1>(
			this ITestFixture fixture,
			Mock<TService> mock,
			Expression<Func<TService, Task>> methodToInvoke,
			CancellationTokenSource cts, TimeSpan? additionalDelay = null)
			where TService : class
		{
			var methodCall = GetMethodCalExpressionOrThrowError(methodToInvoke);

			mock.Setup(methodToInvoke).Returns(async (T1 t1) =>
				{
					await ((Task)methodCall.Method.Invoke(fixture.GetRealService<TService>(), [t1])!).ConfigureAwait(false);

					cts.CancelAfter(additionalDelay ?? TimeSpan.Zero);
				});
		}

		/// <summary>
		/// Unmock and wait - define mock delegate for Moq, the rest is done by this method
		/// </summary>
		/// <typeparam name="TService">Service/Repo type</typeparam>
		/// <typeparam name="T1">first argument type</typeparam>
		/// <typeparam name="T2">second argument type</typeparam>
		/// <param name="fixture">fixture</param>
		/// <param name="mock">mock of type</param>
		/// <param name="methodToInvoke">method to invoke</param>
		/// <param name="cts">Cancelation token source to stop when the job is done</param>
		/// <param name="additionalDelay">optional additional delay</param>
		public static void UnmockAndWait<TService, T1, T2>(
			this ITestFixture fixture,
			Mock<TService> mock,
			Expression<Func<TService, Task>> methodToInvoke,
			CancellationTokenSource cts, TimeSpan? additionalDelay = null)
			where TService : class
		{
			var methodCall = GetMethodCalExpressionOrThrowError(methodToInvoke);

			mock.Setup(methodToInvoke).Returns(async (T1 t1, T2 t2) =>
			{
				await ((Task)methodCall.Method.Invoke(fixture.GetRealService<TService>(), [t1, t2])!).ConfigureAwait(false);

				cts.CancelAfter(additionalDelay ?? TimeSpan.Zero);
			});
		}

		/// <summary>
		/// Unmock and wait - define mock delegate for Moq, the rest is done by this method
		/// </summary>
		/// <typeparam name="TService">Service/Repo type</typeparam>
		/// <typeparam name="T1">first argument type</typeparam>
		/// <typeparam name="T2">second argument type</typeparam>
		/// <typeparam name="T3">third argument type</typeparam>
		/// <param name="fixture">fixture</param>
		/// <param name="mock">mock of type</param>
		/// <param name="methodToInvoke">method to invoke</param>
		/// <param name="cts">Cancelation token source to stop when the job is done</param>
		/// <param name="additionalDelay">optional additional delay</param>
		public static void UnmockAndWait<TService, T1, T2, T3>(
			this ITestFixture fixture,
			Mock<TService> mock,
			Expression<Func<TService, Task>> methodToInvoke,
			CancellationTokenSource cts, TimeSpan? additionalDelay = null)
			where TService : class
		{
			var methodCall = GetMethodCalExpressionOrThrowError(methodToInvoke);

			mock.Setup(methodToInvoke).Returns(async (T1 t1, T2 t2, T3 t3) =>
			{
				await ((Task)methodCall.Method.Invoke(fixture.GetRealService<TService>(), [t1, t2, t3])!).ConfigureAwait(false);

				cts.CancelAfter(additionalDelay ?? TimeSpan.Zero);
			});
		}

		/// <summary>
		/// Unmock and wait - define mock delegate for Moq, the rest is done by this method
		/// </summary>
		/// <typeparam name="TService">Service/Repo type</typeparam>
		/// <typeparam name="T1">first argument type</typeparam>
		/// <typeparam name="T2">second argument type</typeparam>
		/// <typeparam name="T3">third argument type</typeparam>
		/// <typeparam name="T4">fourth argument type</typeparam>
		/// <param name="fixture">fixture</param>
		/// <param name="mock">mock of type</param>
		/// <param name="methodToInvoke">method to invoke</param>
		/// <param name="cts">Cancelation token source to stop when the job is done</param>
		/// <param name="additionalDelay">optional additional delay</param>
		public static void UnmockAndWait<TService, T1, T2, T3, T4>(
			this ITestFixture fixture,
			Mock<TService> mock,
			Expression<Func<TService, Task>> methodToInvoke,
			CancellationTokenSource cts, TimeSpan? additionalDelay = null)
			where TService : class
		{
			var methodCall = GetMethodCalExpressionOrThrowError(methodToInvoke);

			mock.Setup(methodToInvoke).Returns(async (T1 t1, T2 t2, T3 t3, T4 t4) =>
			{
				await ((Task)methodCall.Method.Invoke(fixture.GetRealService<TService>(), [t1, t2, t3, t4])!).ConfigureAwait(false);

				cts.CancelAfter(additionalDelay ?? TimeSpan.Zero);
			});
		}

		/// <summary>
		/// Unmock and wait - define mock delegate for Moq, the rest is done by this method
		/// </summary>
		/// <typeparam name="TService">Service/Repo type</typeparam>
		/// <typeparam name="T1">first argument type</typeparam>
		/// <typeparam name="T2">second argument type</typeparam>
		/// <typeparam name="T3">third argument type</typeparam>
		/// <typeparam name="T4">fourth argument type</typeparam>
		/// <typeparam name="T5">fifth argument type</typeparam>
		/// <param name="fixture">fixture</param>
		/// <param name="mock">mock of type</param>
		/// <param name="methodToInvoke">method to invoke</param>
		/// <param name="cts">Cancelation token source to stop when the job is done</param>
		/// <param name="additionalDelay">optional additional delay</param>
		public static void UnmockAndWait<TService, T1, T2, T3, T4, T5>(
			this ITestFixture fixture,
			Mock<TService> mock,
			Expression<Func<TService, Task>> methodToInvoke,
			CancellationTokenSource cts, TimeSpan? additionalDelay = null)
			where TService : class
		{
			var methodCall = GetMethodCalExpressionOrThrowError(methodToInvoke);

			mock.Setup(methodToInvoke).Returns(async (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) =>
			{
				await ((Task)methodCall.Method.Invoke(fixture.GetRealService<TService>(), [t1, t2, t3, t4, t5])!).ConfigureAwait(false);

				cts.CancelAfter(additionalDelay ?? TimeSpan.Zero);
			});
		}

		/// <summary>
		/// Unmock and wait - define mock delegate for Moq, the rest is done by this method
		/// </summary>
		/// <typeparam name="TService">Service/Repo type</typeparam>
		/// <typeparam name="T1">first argument type</typeparam>
		/// <typeparam name="T2">second argument type</typeparam>
		/// <typeparam name="T3">third argument type</typeparam>
		/// <typeparam name="T4">fourth argument type</typeparam>
		/// <typeparam name="T5">fifth argument type</typeparam>
		/// <typeparam name="T6">sixth argument type</typeparam>
		/// <param name="fixture">fixture</param>
		/// <param name="mock">mock of type</param>
		/// <param name="methodToInvoke">method to invoke</param>
		/// <param name="cts">Cancelation token source to stop when the job is done</param>
		/// <param name="additionalDelay">optional additional delay</param>
		public static void UnmockAndWait<TService, T1, T2, T3, T4, T5, T6>(
			this ITestFixture fixture,
			Mock<TService> mock,
			Expression<Func<TService, Task>> methodToInvoke,
			CancellationTokenSource cts, TimeSpan? additionalDelay = null)
			where TService : class
		{
			var methodCall = GetMethodCalExpressionOrThrowError(methodToInvoke);

			mock.Setup(methodToInvoke).Returns(async (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) =>
			{
				await ((Task)methodCall.Method.Invoke(fixture.GetRealService<TService>(), [t1, t2, t3, t4, t5, t6])!).ConfigureAwait(false);

				cts.CancelAfter(additionalDelay ?? TimeSpan.Zero);
			});
		}

		private static TService GetRealService<TService>(this ITestFixture fixture)
			where TService : class
			=> fixture.ApplicationFactory.Services
					.GetServices<TService>()
					.FirstOrDefault(s => s is not Mock<TService>)!;

		private static MethodCallExpression GetMethodCalExpressionOrThrowError<TService, TReturn>(Expression<Func<TService, Task<TReturn>>> methodToInvoke)
				=> methodToInvoke.Body as MethodCallExpression ?? throw new ArgumentException("The provided expression is not a valid method call.", nameof(methodToInvoke));

		private static MethodCallExpression GetMethodCalExpressionOrThrowError<TService>(Expression<Func<TService, Task>> methodToInvoke)
				=> methodToInvoke.Body as MethodCallExpression ?? throw new ArgumentException("The provided expression is not a valid method call.", nameof(methodToInvoke));
	}
}
