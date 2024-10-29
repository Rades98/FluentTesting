using Testing.Common.Abstraction;

namespace Testing.Sql.Options
{
	/// <summary>
	/// Sql options
	/// </summary>
	public class SqlOptions : IContainerOptions
	{
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
