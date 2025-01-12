using DotNet.Testcontainers.Containers;
using Nito.AsyncEx;

namespace FluentTesting.Common.Extensions;

/// <summary>
/// Container extensions
/// </summary>
public static class ContainerExtensions
{
	/// <summary>
	/// Ensure that container is running
	/// </summary>
	/// <param name="container"></param>
	/// <param name="additionalAsyncOperation">additional async operation f.e. sql seed</param>
	public static void EnsureContainer(this IContainer container, Task? additionalAsyncOperation = null)
	{
		if (container.State == TestcontainersStates.Undefined)
		{
			AsyncContext.Run(async () =>
				{
					await container.StartAsync().ConfigureAwait(false);

					await Task.Delay(2000).ConfigureAwait(false);

					if (container.State == TestcontainersStates.Running)
					{
						if (additionalAsyncOperation is not null)
						{
							await additionalAsyncOperation.ConfigureAwait(false);
						}
					}
				});
		}
	}

	/// <summary>
	/// Ensure that container is running and call extension on container 
	/// </summary>
	/// <param name="container"></param>
	/// <param name="executeAfterStart">additional async operation f.e. sql seed</param>
	public static ExecResult EnsureContainer(this IContainer container, Func<IContainer, Task<ExecResult>> executeAfterStart, TimeSpan? timeout = null)
	{
		if (container.State == TestcontainersStates.Undefined)
		{
			return AsyncContext.Run(async () =>
				{
					await container.StartAsync().ConfigureAwait(false);

					await Task.Delay(timeout ?? TimeSpan.FromSeconds(2)).ConfigureAwait(false);

					return await executeAfterStart.Invoke(container).ConfigureAwait(false);
				});
		}
		throw new Exception($"Invalid state of container: {container.State}");
	}
}