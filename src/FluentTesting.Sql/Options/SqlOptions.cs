using FluentTesting.Common.Abstraction;
using FluentTesting.Common.Options;

namespace FluentTesting.Sql.Options
{
    /// <summary>
    /// Sql options
    /// </summary>
    public class SqlOptions : IContainerOptions
    {
        internal const string ContainerName = "MsSqlContainer";
        internal const string BackupPath = "/var/opt/mssql/backups";

        /// <inheritdoc/>
        public int? Port { get; set; } = 1433;

        /// <inheritdoc/>
        public bool RunAdminTool { get; set; } = true;

        /// <summary>
        /// Database
        /// </summary>
        public string Database { get; set; } = "master";

        /// <summary>
        /// Default user name
        /// </summary>
        public string DefaultUsername { get; } = "sa";

        /// <summary>
        /// Default password
        /// </summary>
        public string Password { get; set; } = "re!GXz-qta8XWV50u{7sZk!";

        /// <summary>
        /// Run with MSSQL_PID Express
        /// </summary>
        public bool RunInExpressMode { get; set; } = true;

        /// <summary>
        /// Wait strategy
        /// </summary>
        public WaitStrategy? WaitStrategy { get; set; }

        public WaitStrategy InitWaitStrategy { get; set; } = new() { IntervalSeconds = 2, RetryCount = 10 };
    }
}
