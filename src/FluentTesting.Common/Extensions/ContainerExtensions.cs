using DotNet.Testcontainers.Containers;

namespace FluentTesting.Common.Extensions;

/// <summary>
/// Container extensions
/// </summary>
public static class ContainerExtensions
{
    /// <summary>
    /// Ensure that container is running and call extension on container 
    /// </summary>
    /// <param name="container"></param>
    /// <param name="executeAfterStart">additional async operation f.e. sql seed</param>
    public static async Task<ExecResult> EnsureContainerAsync(this IContainer container, Func<IContainer, Task<ExecResult>> executeAfterStart, TimeSpan? delay = null)
    {
        if (container.State == TestcontainersStates.Undefined)
        {
            await container.StartAsync().ConfigureAwait(false);

            await Task.Delay(delay ?? TimeSpan.FromSeconds(2)).ConfigureAwait(false);

            return await executeAfterStart.Invoke(container).ConfigureAwait(false);
        }

        if(container.State is TestcontainersStates.Running)
        {
            return await executeAfterStart.Invoke(container).ConfigureAwait(false);
        }

        throw new Exception($"Invalid state of container: {container.State}");
    }

    public static async Task<ExecResult> EnsureContainerAsync(this IContainer container, TimeSpan? delay = null)
    {
        if (container.State == TestcontainersStates.Undefined )
        {
            await container.StartAsync().ConfigureAwait(false);

            await Task.Delay(delay ?? TimeSpan.FromSeconds(2)).ConfigureAwait(false);

            return new ExecResult(string.Empty, string.Empty, 0);
        }

        if(container.State is TestcontainersStates.Running)
        {
            return new ExecResult(string.Empty, string.Empty, 0);
        }

        throw new Exception($"Invalid state of container: {container.State}");
    }
}