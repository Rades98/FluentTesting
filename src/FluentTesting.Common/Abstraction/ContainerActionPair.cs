using DotNet.Testcontainers.Containers;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace FluentTesting.Common.Abstraction
{
    public sealed record ContainerActionPair(IContainer Container, Func<IContainer, Task<ExecResult>> Ensure);
}
