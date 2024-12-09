using FluentTesting.Common.Abstraction;

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
        public string DefautUsername { get; } = "sa";

        /// <summary>
        /// Default password
        /// </summary>
        public string Password { get; set; } = "Strong(!)Password";
    }
}
